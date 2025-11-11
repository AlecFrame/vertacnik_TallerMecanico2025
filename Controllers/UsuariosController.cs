using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UsuariosController : Controller
{
    private readonly ILogger<UsuariosController> _logger;
    private readonly UsuarioRepo _repo;
    private readonly IConfiguration _config;

    public UsuariosController(IConfiguration config, ILogger<UsuariosController> logger)
    {
        _config = config;
        _repo = new UsuarioRepo(config);
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Guardar(Usuario usuario, IFormFile? foto)
    {
        // Leer el valor raw que vino en el formulario (si vino)
        var claveRaw = Request.Form["ClaveHash"].ToString();

        Console.WriteLine("Clave raw recibida: " + claveRaw);
        Console.WriteLine("Clave model state: " + usuario.ClaveHash);

        // Si estamos editando y no se ingresó clave nueva, mantener la clave existente
        Usuario? usuarioExistente = null;
        if (usuario.IdUsuario > 0)
        {
            usuarioExistente = _repo.ObtenerPorId(usuario.IdUsuario);

            if (string.IsNullOrWhiteSpace(claveRaw))
            {
                // evitar la validación requerida sobre Clave cuando es edición y no se cambia
                ModelState.Remove(nameof(usuario.ClaveHash));
                if (usuarioExistente != null)
                {
                    usuario.ClaveHash = usuarioExistente.ClaveHash; // ya está hasheada en la BDD
                }
            }
        }

        // comprobar si viene la instrucción de eliminar foto
        var eliminarFotoFlag = Request.Form["eliminarFoto"].ToString().ToLower() == "true";

        if (ModelState.IsValid)
        {
            if (foto != null && foto.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Usuarios");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"usuario_{usuario.IdUsuario}{Path.GetExtension(foto.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    foto.CopyTo(stream);
                }

                usuario.Avatar = $"~/Uploads/Usuarios/{fileName}";
            }
            else if (usuarioExistente != null)
            {
                // Mantener la foto existente si no se subió una nueva y no se solicitó eliminarla
                if (!eliminarFotoFlag)
                {
                    usuario.Avatar = usuarioExistente.Avatar;
                }
                else
                {
                    // eliminar fichero físico si existe
                    if (!string.IsNullOrEmpty(usuarioExistente.Avatar))
                    {
                        try
                        {
                            var fullPath = usuarioExistente.Avatar.Replace("~/", "").Replace('/', Path.DirectorySeparatorChar);
                            var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fullPath.TrimStart(Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
                        }
                        catch { /* ignore errors al borrar */ }
                    }
                    usuario.Avatar = null;
                }
            }

            if (usuario.IdUsuario > 0)
            {
                _repo.Modificar(usuario);
            }
            else
            {
                _repo.Alta(usuario, usuario.ClaveHash);
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
            // Si es AJAX, devolver los errores de validación en JSON (por campo)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errorDict = new Dictionary<string, List<string>>();
                foreach (var kv in ModelState.Where(kv => kv.Value.Errors != null && kv.Value.Errors.Count > 0))
                {
                    // tomar sólo el nombre de la propiedad (p.ej. "Nombre" en vez de "usuario.Nombre")
                    var key = kv.Key;
                    if (key.Contains('.')) key = key.Substring(key.LastIndexOf('.') + 1);
                    var list = kv.Value.Errors.Select(e => e.ErrorMessage).ToList();
                    errorDict[key] = list;
                }
                return Json(new { success = false, errors = errorDict });
            }
            else
            {
                // Guardamos el modelo y errores en TempData
                TempData["UsuarioConErrores"] = System.Text.Json.JsonSerializer.Serialize(usuario);
                TempData["ErroresValidacion"] = System.Text.Json.JsonSerializer.Serialize(
                    ModelState.Where(ms => ms.Value.Errors.Any())
                            .ToDictionary(
                                kv => kv.Key,
                                kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToList()
                            )
                );

                // Redirigimos a Index
                return RedirectToAction("Index");
            }
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}