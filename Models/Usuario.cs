using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vertacnik_TallerMecanico2025.Models
{
    public enum RolUsuario
    {
        Administrador,
        Mecanico,
        Recepcionista
    }

    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(10, MinimumLength = 7, ErrorMessage = "El DNI debe tener entre 7 y 10 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El DNI solo puede contener números.")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$", ErrorMessage = "El nombre solo puede contener letras.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ\s]+$", ErrorMessage = "El apellido solo puede contener letras.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(60, ErrorMessage = "El correo electrónico no puede superar los 60 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 15 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo puede contener números.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [Display(Name = "Contraseña")]
        [StringLength(100, ErrorMessage = "La clave no puede superar los 100 caracteres.")]
        public string ClaveHash { get; set; }

        [Required(ErrorMessage = "Debe asignarse un rol al usuario.")]
        public RolUsuario Rol { get; set; }

        [DefaultValue(true)]
        public bool Estado { get; set; }

        [Display(Name = "Foto de perfil")]
        public string? Avatar { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}