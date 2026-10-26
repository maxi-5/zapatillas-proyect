using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    public class ResenaCreateDto
    {
        /// <summary>Id de la zapatilla que se reseña (ver GET /Zapatillas).</summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una zapatilla valida")]
        public int IdZapatilla { get; set; }

        /// <summary>Texto de la opinion.</summary>
        /// <example>Muy comodas</example>
        [Required(ErrorMessage = "La reseña es obligatoria")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "La reseña debe tener entre 3 y 1000 caracteres")]
        public string Resena { get; set; } = null!;

        /// <summary>Nota de 1 a 5.</summary>
        /// <example>5</example>
        [Range(1, 5, ErrorMessage = "La valoracion debe estar entre 1 y 5")]
        public int Valoracion { get; set; }
    }

    public class ResenaDto
    {
        /// <summary>Id de la reseña (se usa en DELETE /Resenas).</summary>
        public int IdResena { get; set; }
        /// <summary>Id de la zapatilla reseñada.</summary>
        public int IdZapatilla { get; set; }
        public string Resena { get; set; } = null!;
        public int Valoracion { get; set; }
        /// <summary>Nombre del usuario que escribio la reseña.</summary>
        public string? Usuario { get; set; }
    }
}
