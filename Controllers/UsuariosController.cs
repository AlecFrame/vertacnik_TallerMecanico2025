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
        var lista = _repo.ObtenerTodos();

        if (TempData.ContainsKey("UsuarioConErrores"))
        {
            var usuarioJson = TempData["UsuarioConErrores"]?.ToString();
            var erroresJson = TempData["ErroresValidacion"]?.ToString();

            if (!string.IsNullOrEmpty(usuarioJson))
            {
                var usuarioConErrores = System.Text.Json.JsonSerializer.Deserialize<Usuario>(usuarioJson);
                ViewBag.UsuarioConErrores = usuarioConErrores;
                // Si también tenemos errores, reconstruir ModelState con ellos para renderizar el partial con mensajes
                if (!string.IsNullOrEmpty(erroresJson))
                {
                    var errores = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(erroresJson);
                    if (errores != null)
                    {
                        // limpiar ModelState actual y poblar con los errores almacenados
                        ModelState.Clear();
                        foreach (var kv in errores)
                        {
                            foreach (var msg in kv.Value)
                            {
                                // Las claves deben coincidir con los nombres de los inputs (p.ej. "Nombre")
                                ModelState.AddModelError(kv.Key, msg);
                            }
                        }

                        // Renderizar el partial en el servidor con el modelo y ModelState poblado
                        try
                        {
                            var html = RenderPartialViewToString("_FormularioUsuario", usuarioConErrores);
                            ViewBag.ModalHtml = html;
                        }
                        catch
                        {
                            ViewBag.ModalHtml = null;
                        }
                    }
                }
            }

            // Si no se renderizó el partial, mantener los errores en ViewBag por compatibilidad
            if (ViewBag.ModalHtml == null && !string.IsNullOrEmpty(erroresJson))
            {
                var errores = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, List<string>>>(erroresJson);
                ViewBag.ErroresValidacion = errores;
            }
        }

        return View(lista);
    }

    // Helper para renderizar un partial a string (usa el ModelState actual)
    private string RenderPartialViewToString(string viewName, object model)
    {
        var viewEngine = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine)) as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;
        var tempDataProvider = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider)) as Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider;
        var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

        if (viewEngine == null || tempDataProvider == null)
            return string.Empty;

        using (var sw = new StringWriter())
        {
            var viewResult = viewEngine.FindView(actionContext, viewName, false);
            if (!viewResult.Success)
                return string.Empty;

            var viewDictionary = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), ModelState) { Model = model };
            var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(actionContext, viewResult.View, viewDictionary, new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(HttpContext, tempDataProvider), sw, new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());
            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
            return sw.ToString();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Guardar(Usuario usuario, IFormFile? foto)
    {
        // Leer el valor raw que vino en el formulario (si vino)
        var claveRaw = Request.Form["Clave"].ToString();

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
            // Si es AJAX, devolver los errores de validación en JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
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

    public IActionResult Crear()
    {
        return PartialView("_FormularioUsuario", new Usuario());
    }
    public IActionResult Editar(int id)
    {
        var usuario = _repo.ObtenerPorId(id);
        return PartialView("_FormularioUsuario", usuario);
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

    public IActionResult CargarTabla()
    {
        var lista = _repo.ObtenerTodos();
        return PartialView("_TablaUsuarios", lista);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}