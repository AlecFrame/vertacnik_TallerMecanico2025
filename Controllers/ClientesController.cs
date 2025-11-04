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

    public ClientesController(ClienteRepo repo, IConfiguration config, ILogger<ClientesController> logger)
    {
        _config = config;
        _repo = new ClienteRepo(config);
        _logger = logger;
    }

    public IActionResult Index()
    {
        var lista = _repo.ObtenerTodos();
        return View(lista);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errores });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }

    public IActionResult Crear()
    {
        return PartialView("_FormularioCliente", new Cliente());
    }

    public IActionResult Editar(int id)
    {
        var cliente = _repo.ObtenerPorId(id);
        if (cliente == null)
        {
            return NotFound();
        }
        return PartialView("_FormularioCliente", cliente);
    }

    public IActionResult Inactivar(int id)
    {
        _repo.CambiarEstado(id, false);
        return RedirectToAction("Index");
    }

    public IActionResult Activar(int id)
    {
        _repo.CambiarEstado(id, true);
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}