using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize]
public class ClientesController : Controller
{
    private readonly ILogger<ClientesController> _logger;
    private readonly ClienteRepo _repo;
    private readonly IConfiguration _config;

    public ClientesController(IConfiguration config, ILogger<ClientesController> logger)
    {
        _config = config;
        _repo = new ClienteRepo(config);
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "RecepcionOnly")]
    public IActionResult Guardar(Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            if (cliente.IdCliente > 0)
            {
                _repo.Modificacion(cliente);
            }
            else
            {
                _repo.Alta(cliente);
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
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return Json(new { success = false, errors = errorDict });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }

    [HttpPost]
    [Authorize(Policy = "RecepcionOnly")]
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
    public IActionResult Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Json(new List<object>());

        var verificarSiFunciona = _repo.BuscarPorNombreODni(q);

        var resultados = _repo.BuscarPorNombreODni(q)
            .Select(c => new
            {
                idCliente = c.IdCliente,
                nombreCompleto = $"{c.Nombre} {c.Apellido} ({c.Dni})"
            }).ToList();

        return Json(resultados);
    }

    [HttpGet]
    public IActionResult GetPaged(int pagina = 1, int tamanioPagina = 10)
    {
        var (lista, total) = _repo.ObtenerPaginado(pagina, tamanioPagina);
        return Json(new { data = lista, total });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}