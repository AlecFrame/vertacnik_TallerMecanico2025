using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using vertacnik_TallerMecanico2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace vertacnik_TallerMecanico2025.Controllers;

[Authorize]
public class PedidosController : Controller
{
    private readonly ILogger<PedidosController> _logger;
    private readonly PedidoRepo _repo;
    private readonly ServicioRepo servicioRepo;
    private readonly IConfiguration _config;

    public PedidosController(IConfiguration config, ILogger<PedidosController> logger)
    {
        _config = config;
        _repo = new PedidoRepo(config);
        servicioRepo = new ServicioRepo(config);
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Guardar(Pedido pedido)
    {
        if (ModelState.IsValid)
        {
            if (pedido.IdPedido > 0)
            {
                Pedido? pedidoExistente = _repo.ObtenerPorId(pedido.IdPedido);

                if (pedidoExistente != null)
                {
                    pedidoExistente.IdVehiculo = pedido.IdVehiculo;
                    pedidoExistente.ObservacionCliente = pedido.ObservacionCliente;

                    _repo.Modificacion(pedidoExistente);
                }
            }
            else
            {
                // usuario que lo creo (convertir claim string a int, tomar 0 si no está o no se puede convertir)
                var idUsuarioClaim = User.FindFirst("IdUsuario")?.Value;
                if (!string.IsNullOrEmpty(idUsuarioClaim) && int.TryParse(idUsuarioClaim, out var idUsuario))
                {
                    pedido.IdUsuario = idUsuario;
                }
                else
                {
                    pedido.IdUsuario = 0;
                }

                pedido.FechaIngreso = DateTime.Now;
                pedido.CostoEstimado = 0;
                pedido.CostoFinal = 0;

                _repo.Alta(pedido);
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
    public IActionResult CancelarPedido(int id, bool cancelar)
    {
        Pedido? pedido = _repo.ObtenerPorId(id);

        if (pedido == null)
            return NotFound();
        
        if (pedido.Estado != EstadoPedido.Pagado)
        {
            if (cancelar)
            {
                _repo.CambiarEstado(id, EstadoPedido.Cancelado);
            }
            else
            {
                if (pedido.FechaFinalizacion != null)
                {
                    _repo.CambiarEstado(id, EstadoPedido.Finalizado);
                }
                else
                {
                   IList<Servicio> servicios = servicioRepo.ObtenerPorIdPedido(id);

                    if (servicios.Count > 0)
                    {
                        _repo.CambiarEstado(id, EstadoPedido.EnProceso);
                    }else
                    {
                        _repo.CambiarEstado(id, EstadoPedido.Pendiente);
                    } 
                }
            }
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult PagarOVolverPedido(int id, bool volver)
    {
        Pedido? pedido = _repo.ObtenerPorId(id);

        if (pedido == null)
            return NotFound();

        if (pedido.Estado != EstadoPedido.Pagado)
        {
            if (pedido.Estado == EstadoPedido.Finalizado)
            {
                if (volver)
                {
                    pedido.FechaFinalizacion = null;
                    pedido.ObservacionFinal = null;
                    pedido.CostoFinal = null;
                    pedido.Estado = EstadoPedido.EnProceso;
                    _repo.Modificacion(pedido);

                }
                else
                {
                    pedido.FechaPago = DateTime.Now;
                    pedido.Estado = EstadoPedido.Pagado;
                    _repo.Modificacion(pedido);
                }
            }
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = true });
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Finalizar(Pedido pedido)
    {
        // ❗ Eliminar validaciones que NO aplican en este contexto
        ModelState.Remove("IdUsuario");
        ModelState.Remove("IdVehiculo");
        ModelState.Remove("ObservacionCliente");
        ModelState.Remove("FechaIngreso");
        ModelState.Remove("CostoEstimado");
        ModelState.Remove("FechaPago");

        ModelState.Remove("Estado"); // Los seteo yo
        ModelState.Remove("FechaFinalizacion");

        // 🔥 Validaciones MANUALES para este proceso
        if (string.IsNullOrWhiteSpace(pedido.ObservacionFinal))
            ModelState.AddModelError("ObservacionFinal", "Debe ingresar la observación de finalización.");

        if (pedido.CostoFinal == null)
            ModelState.AddModelError("CostoFinal", "Debe ingresar el costo final.");

        // Validación numérica
        if (pedido.CostoFinal != null && pedido.CostoFinal < 0)
            ModelState.AddModelError("CostoFinal", "El costo final no puede ser negativo.");
        
        if (ModelState.IsValid)
        {
            if (pedido.IdPedido > 0)
            {
                Pedido? pedidoExistente = _repo.ObtenerPorId(pedido.IdPedido);

                if (pedidoExistente != null)
                {
                    pedidoExistente.FechaFinalizacion = DateTime.Now;
                    pedidoExistente.ObservacionFinal = pedido.ObservacionFinal;
                    pedidoExistente.CostoFinal = pedido.CostoFinal;
                    pedidoExistente.Estado = EstadoPedido.Finalizado;

                    _repo.Modificacion(pedidoExistente);
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
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
    public IActionResult DetallePedido(int id)
    {
        var pedido = _repo.ObtenerPorId(id);

        if (pedido == null)
            return NotFound();

        // Si querés incluir datos relacionados (vehículo, cliente, servicios, etc.)
        ServicioRepo servicioRepo = new ServicioRepo(_config);
        pedido.Servicios = servicioRepo.ObtenerPorIdPedido(id);

        return View(pedido);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        Pedido? pedido = _repo.ObtenerPorId(id);

        if (pedido == null)
            return NotFound();

        return Json(new {
            idPedido = pedido.IdPedido,
            costoEstimado = pedido.CostoEstimado,
            costoFinal = pedido.CostoFinal,
            estado = (int)pedido.Estado,
            fechaFinalizacion = pedido.FechaFinalizacion,
            observacionFinal = pedido.ObservacionFinal,
            vehiculo = new {
                descripcionCorta = pedido.Vehiculo?.DescripcionCorta,
                duenoNombre = pedido.Vehiculo?.DuenoNombre
            },
            creadoPor = pedido.Usuario?.NombreCompleto
        });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}