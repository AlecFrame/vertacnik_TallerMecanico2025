using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{

    public enum TipoMovimiento
    {
        Entrada,
        Salida,
        Inspeccion,
        Prueba,
        Otro
    }

    [Table("movimientovehiculos")]
    public class MovimientoVehiculo
    {
        [Key]
        public int IdMovimiento { get; set; }

        [Required, Display(Name = "Vehículo")]
        public int IdVehiculo { get; set; }

        [Required, Display(Name = "Usuario")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El Tipo de Movimiento es obligatorio.")]
        [Display(Name = "Tipo de Movimiento")]
        public TipoMovimiento TipoMovimiento { get; set; }

        [Required(ErrorMessage = "La Fecha de Movimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [Required(ErrorMessage = "La Observación es obligatoria.")]
        [StringLength(400, ErrorMessage = "La observación no puede superar los 400 caracteres.")]
        public string Observacion { get; set; }

        public Vehiculo? Vehiculo { get; set; }
        public Usuario? Usuario { get; set; }
    }
}