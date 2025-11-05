using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("detalletiposervicios")]
    public class DetalleTipoServicio
    {
        [Key]
        public int IdDetalleTipoServicio { get; set; }

        [Required, Display(Name = "Servicio")]
        public int IdServicio { get; set; }

        [Required, Display(Name = "Tipo de Servicio")]
        public int IdTipoServicio { get; set; }

        [Required(ErrorMessage = "El Costo Extra es obligatorio.")]
        [Range(0.00, 1000000.00, ErrorMessage = "El costo extra debe estar entre 0.00 y 1,000,000.00.")]
        public decimal CostoExtra { get; set; }

        [StringLength(400, ErrorMessage = "La motivo no puede superar los 400 caracteres.")]
        public string? Motivo { get; set; }

        public Servicio? Servicio { get; set; }
        public TipoServicio? TipoServicio { get; set; }

        public string DescripcionCompleta()
        {
            if (TipoServicio != null)
            {
                return $"{TipoServicio.Nombre} - Costo Extra: {CostoExtra:C}";
            }
            return $"Tipo de Servicio ID: {IdTipoServicio} - Costo Extra: {CostoExtra:C}";
        }
    }
}