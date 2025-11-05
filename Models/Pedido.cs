using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{

    public enum EstadoPedido
    {
        Pendiente,
        EnProceso,
        Finalizado,
        Pagado,
        Cancelado
    }

    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required, Display(Name = "Usuario")]
        public int IdUsuario{ get; set; }

        [Required, Display(Name = "Vehículo")]
        public int IdVehiculo { get; set; }

        [Required(ErrorMessage = "La observación del Cliente es obligatoria.")]
        [StringLength(400, ErrorMessage = "La observación del Cliente no puede superar los 400 caracteres.")]
        [Display(Name = "Observación del Cliente")]
        public string ObservacionCliente { get; set; }

        [Required(ErrorMessage = "La Fecha del Ingreso del pedido es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Ingreso del pedido")]
        public DateTime FechaIngreso { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de finalización del pedido")]
        public DateTime? FechaFinalizacion { get; set; }

        [StringLength(400, ErrorMessage = "La observación del Final no puede superar los 400 caracteres.")]
        [Display(Name = "Observación final")]
        public string? ObservacionFinal { get; set; }

        [Required(ErrorMessage = "El Costo Estimado es obligatorio.")]
        [Range(0.01, 500000000.00, ErrorMessage = "El costo estimado debe estar entre 0.01 y 500,000,000.00.")]
        [Display(Name = "Costo Estimado")]
        public decimal CostoEstimado { get; set; }

        [Display(Name = "Costo Final")]
        public decimal? CostoFinal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de pago del pedido")]
        public DateTime? FechaPago { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio.")]
        public EstadoPedido Estado { get; set; }

        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
    }
}