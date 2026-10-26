using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    public class MarcaDto
    {
        /// <summary>Id de la marca (lo genera la base; al crear se puede dejar en 0).</summary>
        /// <example>0</example>
        public int IdMarca { get; set; }

        /// <example>New Balance</example>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar 150 caracteres")]
        public string Nombre { get; set; } = null!;

        /// <example>Estados Unidos</example>
        [Required(ErrorMessage = "El pais es obligatorio")]
        [StringLength(50, ErrorMessage = "El pais no puede superar 50 caracteres")]
        public string Pais { get; set; } = null!;
    }

    public class CategoriaDto
    {
        /// <summary>Id de la categoria (lo genera la base; al crear se puede dejar en 0).</summary>
        /// <example>0</example>
        public int IdCategoria { get; set; }

        /// <example>Trekking</example>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres")]
        public string Nombre { get; set; } = null!;
    }

    public class RolDto
    {
        /// <summary>Id del rol: 1 Admin, 2 Cliente.</summary>
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
    }
}
