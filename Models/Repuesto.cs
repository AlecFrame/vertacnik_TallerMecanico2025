using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("repuestos")]
    public class Repuesto
    {
        [Key]
        public int IdRepuesto { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Descripción es obligatoria.")]
        [StringLength(400, ErrorMessage = "La descripción no puede superar los 400 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El Costo es obligatorio.")]
        [Range(0.01, 2000000.00, ErrorMessage = "El costo debe estar entre 0.01 y 2,000,000.00.")]
        [Display(Name = "Costo Repuesto Base")]
        public decimal CostoRepuestoBase { get; set; }

        [DefaultValue(true)]
        public bool Estado { get; set; }
    }
}