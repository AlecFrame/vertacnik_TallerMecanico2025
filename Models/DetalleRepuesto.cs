using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("detallerepuestos")]
    public class DetalleRepuesto
    {
        [Key]
        public int IdDetalleRepuesto { get; set; }

        [Required, Display(Name = "Servicio")]
        public int IdServicio { get; set; }

        [Required, Display(Name = "Repuesto")]
        public int IdRepuesto { get; set; }

        [Required(ErrorMessage = "La Cantidad es obligatoria.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cantidad solo puede contener números.")]
        [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
        public int Cantidad { get; set; }

        public Servicio? Servicio { get; set; }
        public Repuesto? Repuesto { get; set; }

        [NotMapped]
        public decimal Total
        {
            get
            {
                return Repuesto != null ? Repuesto.CostoRepuestoBase * Cantidad : 0;
            }
        }

        [NotMapped]
        public string Ver
        {
            get
            {
                return "detalleRepuesto #"+IdDetalleRepuesto+": idServicio("+IdServicio+"), idRepuesto("+IdRepuesto+"), cantidad("+Cantidad+")";
            }
        }
    }
}