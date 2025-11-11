using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize]
public class VehiculosController : Controller
{
    private readonly ILogger<ClientesController> _logger;
    private readonly VehiculoRepo _repo;
    private readonly IConfiguration _config;

    public VehiculosController(IConfiguration config, ILogger<ClientesController> logger)
    {
        _config = config;
        _repo = new VehiculoRepo(config);
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Guardar(Vehiculo vehiculo, IFormFile? foto)
    {
        if (ModelState.IsValid)
        {
            Vehiculo? vehiculoExistente = _repo.ObtenerPorId(vehiculo.IdVehiculo);
            // ¿El cliente solicitó eliminar la foto actual?
            var eliminarFotoFlag = Request.Form["EliminarFoto"].FirstOrDefault();
            var eliminarFoto = string.Equals(eliminarFotoFlag, "true", StringComparison.OrdinalIgnoreCase);

            if (eliminarFoto)
            {
                // borrar archivo físico si existía
                if (vehiculoExistente != null && !string.IsNullOrEmpty(vehiculoExistente.Foto))
                {
                    try
                    {
                        // vehiculoExistente.Foto tiene formato '~/Uploads/Vehiculos/filename.ext'
                        var relative = vehiculoExistente.Foto.Replace('~', ' ').Trim();
                        relative = relative.TrimStart('/', '\\');
                        var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relative.Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(physical))
                        {
                            System.IO.File.Delete(physical);
                        }
                    }
                    catch
                    {
                        // ignorar errores al borrar archivo
                    }
                }
                vehiculo.Foto = null;
            }
            else if (foto != null && foto.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Vehiculos");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Use a GUID-based filename so we don't depend on the DB-generated Id being available yet.
                var fileName = $"vehiculo_{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    foto.CopyTo(stream);
                }

                vehiculo.Foto = $"~/Uploads/Vehiculos/{fileName}";
            }
            else if (vehiculoExistente != null)
            {
                // conservar la foto existente si no se subió nueva y no se pidió eliminarla
                vehiculo.Foto = vehiculoExistente.Foto;
            }

            if (vehiculo.IdVehiculo > 0)
            {
                _repo.Modificacion(vehiculo);
            }
            else
            {
                _repo.Alta(vehiculo);
            }
            // Si es AJAX, devolver JSON, si no, redirigir
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        else
        {
            // Si es AJAX, devolver JSON con errores, si no, redirigir
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Construir un diccionario campo -> lista de errores para que el cliente Vue pueda
                // mostrar los mensajes debajo de cada campo (por ejemplo: errores.Nombre => ["Requerido"])
                var errorDict = ModelState
                    .Where(kvp => kvp.Value.Errors != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new { success = false, errors = errorDict });
            }

            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult CambiarEstado(int id, bool activo)
    {
        _repo.CambiarEstado(id, activo);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var lista = _repo.ObtenerTodos();
        return Json(lista);
    }

    [HttpGet]
    public IActionResult GetPaged(int pagina = 1, int tamanioPagina = 10)
    {
        var (lista, total) = _repo.ObtenerPaginado(pagina, tamanioPagina);
        return Json(new { data = lista, total });
    }

    [HttpGet]
    public IActionResult Search(int idCliente, string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Json(new List<object>());

        var verificarSiFunciona = _repo.BuscarPorPatenteMarcaModeloColorYFiltrarCliente(idCliente, q);

        var resultados = _repo.BuscarPorPatenteMarcaModeloColorYFiltrarCliente(idCliente, q)
            .Select(v => new
            {
                idVehiculo = v.IdVehiculo,
                descripcionCorta = $"{v.DescripcionCorta}"
            }).ToList();

        return Json(resultados);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}