using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize]
public class DetalleRepuestosController : Controller
{
    private readonly ILogger<RepuestosController> _logger;
    private readonly DetalleRepuestoRepo _repo;
    private readonly RepuestoRepo _repoRepuesto;
    private readonly ServicioRepo _repoServicio;
    private readonly PedidoRepo _repoPedido;
    private readonly IConfiguration _config;

    public DetalleRepuestosController(IConfiguration config, ILogger<RepuestosController> logger)
    {
        _config = config;
        _repo = new DetalleRepuestoRepo(config);
        _repoRepuesto = new RepuestoRepo(config);
        _repoServicio = new ServicioRepo(config);
        _repoPedido = new PedidoRepo(config);
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "MecanicOnly")]
    public IActionResult Guardar(DetalleRepuesto detalle)
    {
        //Console.WriteLine(detalle.Ver);

        if (ModelState.IsValid)
        {
            if (detalle.IdDetalleRepuesto > 0)
            {
                _repo.Modificacion(detalle);
                Servicio? servicio = _repoServicio.ObtenerPorId(detalle.IdServicio);
                if (servicio!=null) {
                    _repoServicio.ActualizarCostoBase(servicio.IdServicio);
                    _repoPedido.ActualizarCostoEstimado(servicio.IdPedido);
                }
            }
            else
            {
                _repo.Alta(detalle);
                Servicio? servicio = _repoServicio.ObtenerPorId(detalle.IdServicio);
                if (servicio!=null) {
                    _repoServicio.ActualizarCostoBase(servicio.IdServicio);
                    _repoPedido.ActualizarCostoEstimado(servicio.IdPedido);
                }
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
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
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
    [Authorize(Policy = "MecanicOnly")]
    public IActionResult Eliminar(int id)
    {
        DetalleRepuesto? detalle = _repo.ObtenerPorId(id);

        if (detalle!=null) {

            _repo.Baja(id);
            Servicio? servicio = _repoServicio.ObtenerPorId(detalle.IdServicio);
            
            if (servicio!=null) {
                _repoServicio.ActualizarCostoBase(servicio.IdServicio);
                _repoPedido.ActualizarCostoEstimado(servicio.IdPedido);
            }
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetPaged(int idServicio, int pagina = 1, int tamanioPagina = 10)
    {
        var (lista, total) = _repo.ObtenerPaginadoPorServicio(idServicio, pagina, tamanioPagina);
        return Json(new { data = lista, total });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}