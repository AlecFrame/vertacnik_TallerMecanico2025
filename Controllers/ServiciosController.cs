using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize]
public class ServiciosController : Controller
{
    private readonly ILogger<ServiciosController> _logger;
    private readonly ServicioRepo _repo;
    private readonly TipoServicioRepo _repoTipo;
    private readonly DetalleRepuestoRepo _repoDetalle;
    private readonly PedidoRepo _repoPedido;
    private readonly IConfiguration _config;

    public ServiciosController(IConfiguration config, ILogger<ServiciosController> logger)
    {
        _config = config;
        _repo = new ServicioRepo(config);
        _repoTipo = new TipoServicioRepo(config);
        _repoDetalle = new DetalleRepuestoRepo(config);
        _repoPedido = new PedidoRepo(config);
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "MecanicOnly")]
    public IActionResult Guardar(Servicio servicio)
    {
        //Console.WriteLine("1er, servicio: "+servicio.DescripcionCompleta);

        if (ModelState.IsValid)
        {
            if (servicio.IdServicio > 0)
            {
                Servicio? servicioExistente = _repo.ObtenerPorId(servicio.IdServicio);

                if (servicioExistente != null)
                {
                    bool flag = false;

                    if (servicio.IdTipoServicio != 0 && servicio.IdTipoServicio != servicioExistente.IdTipoServicio) {
                        servicioExistente.IdTipoServicio = servicio.IdTipoServicio;
                        flag = true;
                    }

                    servicioExistente.Descripcion = servicio.Descripcion;

                    _repo.Modificacion(servicioExistente);

                    if (flag) 
                    {
                        _repo.ActualizarCostoBase(servicioExistente.IdServicio);
                        if (servicioExistente.Estado)
                            _repoPedido.ActualizarCostoEstimado(servicioExistente.IdPedido);
                    }
                }
            }
            else
            {
                var idUsuarioClaim = User.FindFirst("IdUsuario")?.Value;
                if (!string.IsNullOrEmpty(idUsuarioClaim) && int.TryParse(idUsuarioClaim, out var idUsuario))
                {
                    servicio.IdUsuario = idUsuario;
                }
                else
                {
                    servicio.IdUsuario = 0;
                }

                servicio.Fecha = DateTime.Now;
                TipoServicio? tipo = _repoTipo.ObtenerPorId(servicio.IdTipoServicio);
                if (tipo != null)
                {
                    servicio.CostoBase = tipo.CostoTipoServicioBase;
                }else
                {
                    servicio.CostoBase = 0;
                }

                _repo.Alta(servicio);

                Pedido? pedido = _repoPedido.ObtenerPorId(servicio.IdPedido);
                if (pedido != null) // pasa seguir con el flujo, al crear un servicio en un pedido este cambia su estado a en proceso
                {
                    if (pedido.Estado==EstadoPedido.Pendiente) {
                        _repoPedido.CambiarEstado(pedido.IdPedido, EstadoPedido.EnProceso);
                    }

                    _repoPedido.ActualizarCostoEstimado(pedido.IdPedido);
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
    public IActionResult CambiarEstado(int id, bool activo)
    {
        _repo.CambiarEstado(id, activo);

        Servicio? servicio = _repo.ObtenerPorId(id);
        if (servicio != null)
        {
            _repoPedido.ActualizarCostoEstimado(servicio.IdPedido);
        }

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
    public IActionResult GetPagedID(int idPedido, int pagina = 1, int tamanioPagina = 10)
    {
        var (lista, total) = _repo.ObtenerPaginadoID(idPedido, pagina, tamanioPagina);
        return Json(new { data = lista, total });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}