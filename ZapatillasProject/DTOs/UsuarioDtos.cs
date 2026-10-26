using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es valido")]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es valido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = null!;
    }

    public class CambiarRolDto
    {
        /// <summary>Nuevo rol del usuario: 1 Admin, 2 Cliente.</summary>
        /// <example>2</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un rol valido")]
        public int IdRol { get; set; }
    }

    public class UsuarioDto
    {
        /// <summary>Id del usuario (se usa en /User/{id}).</summary>
        public int IdUser { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        /// <summary>Id del rol: 1 Admin, 2 Cliente.</summary>
        public int IdRol { get; set; }
        public string? Rol { get; set; }
    }

    public class TokenDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expira { get; set; }
        public string Nombre { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
