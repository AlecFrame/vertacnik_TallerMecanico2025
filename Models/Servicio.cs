using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("servicios")]
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        [Required, Display(Name = "Pedido")]
        public int IdPedido { get; set; }

        [Required, Display(Name = "Mecánico")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La Fecha del servicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del servicio")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La Descripción del Servicio es obligatoria.")]
        [StringLength(400, ErrorMessage = "La descripción del servicio no puede superar los 400 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El Costo del Servicio es obligatorio.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El costo debe estar entre 0.01 y 10,000,000.00.")]
        [Display(Name = "Costo Base del servicio")]
        public decimal CostoBase { get; set; }

        [DefaultValue(true)]
        public bool Estado { get; set; }

        public Pedido? Pedido { get; set; }
        public Usuario? Usuario { get; set; }
    }
}