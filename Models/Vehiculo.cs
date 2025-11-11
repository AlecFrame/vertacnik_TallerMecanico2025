using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    [Table("vehiculos")]
    public class Vehiculo
    {
        [Key]
        public int IdVehiculo { get; set; }

        [Required, Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "La Patente es obligatoria.")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "La patente DNI debe tener entre 6 y 10 caracteres.")]
        public string Patente { get; set; }

        [Required(ErrorMessage = "La Marca es obligatoria.")]
        [StringLength(50, ErrorMessage = "La marca no puede superar los 50 caracteres.")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El Modelo es obligatorio.")]
        [StringLength(60, ErrorMessage = "El modelo no puede superar los 60 caracteres.")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "El Año es obligatorio.")]
        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100.")]
        [Display(Name = "Año")]
        public int Anio { get; set; }

        [Required(ErrorMessage = "El Color es obligatorio.")]
        [StringLength(60, ErrorMessage = "El color no puede superar los 60 caracteres.")]
        public string Color { get; set; }

        [Display(Name = "Foto del Vehículo")]
        public string? Foto { get; set; }

        [DefaultValue(true)]
        public bool EnElTaller { get; set; }

        public Cliente? Cliente { get; set; }

        [NotMapped]
        public string DuenoNombre
        {
            get { return Cliente != null ? Cliente.NombreCompleto : "Desconocido"; }
        }

        [NotMapped]
        public string FotoRelativePath
        {
            get
            {
                return !string.IsNullOrEmpty(Foto) ? Foto.Replace('~', ' ').Trim() : string.Empty;
            }
        }

        [NotMapped]
        public string DescripcionCorta
        {
            get
            {
                return $"{Patente}, {Marca} {Modelo} {Color}";
            }
        }

        [NotMapped]
        public string DescripcionCompleta
        {
            get
            {
                if (Cliente != null)
                {
                    return $"{Marca} {Modelo} {Color} ({Anio}) - Patente: {Patente} - Dueño: {Cliente.NombreCompleto}";
                }
                return $"{Marca} {Modelo} {Color} ({Anio}) - Patente: {Patente}";
            }
        }
    }
}