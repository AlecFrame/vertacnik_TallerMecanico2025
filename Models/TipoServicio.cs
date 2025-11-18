using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("tiposervicios")]
    public class TipoServicio
    {
        [Key]
        public int IdTipoServicio { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del servicio no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción del servicio es obligatoria.")]
        [StringLength(400, ErrorMessage = "La descripción del servicio no puede superar los 400 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El costo del servicio es obligatorio.")]
        [Range(0.01, 100000000.00, ErrorMessage = "El costo debe estar entre 0.01 y 100,000,000.00.")]
        [Display(Name = "Costo Tipo Servicio Base")]
        public decimal CostoTipoServicioBase { get; set; }

        [DefaultValue(true)]
        public bool Estado { get; set; }

        [NotMapped]
        public string DescripcionLarga
        {
            get
            {
                return Nombre+", "+Descripcion;
            }
        }
    }
}