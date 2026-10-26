using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    // Datos que se reciben para crear o editar una zapatilla (POST / PUT)
    public class ZapatillaCreateDto
    {
        /// <summary>Nombre del modelo de la zapatilla.</summary>
        /// <example>Air Max 90</example>
        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El modelo debe tener entre 2 y 150 caracteres")]
        public string Modelo { get; set; } = null!;

        /// <summary>Precio en pesos chilenos, sin puntos.</summary>
        /// <example>89990</example>
        [Range(1, 10000000, ErrorMessage = "El precio debe ser mayor a 0")]
        public int Precio { get; set; }

        /// <summary>Id de la categoria (ver GET /Categorias). Iniciales: 1 Running, 2 Urbana, 3 Basketball, 4 Skate.</summary>
        /// <example>2</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una categoria valida")]
        public int IdCategoria { get; set; }

        /// <summary>Id de la marca (ver GET /Marcas). Iniciales: 1 Nike, 2 Adidas, 3 Puma, 4 Vans.</summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una marca valida")]
        public int IdMarca { get; set; }

        /// <summary>Descripcion opcional de la zapatilla.</summary>
        /// <example>Zapatilla clasica urbana</example>
        [StringLength(1000, ErrorMessage = "La descripcion no puede superar 1000 caracteres")]
        public string? Descripcion { get; set; }
    }

    // Datos que se devuelven al cliente (GET)
    public class ZapatillaDto
    {
        /// <summary>Id de la zapatilla (lo genera la base de datos).</summary>
        public int IdZapatilla { get; set; }
        public string Modelo { get; set; } = null!;
        public int Precio { get; set; }
        public string? Descripcion { get; set; }
        /// <summary>Id de la marca a la que pertenece.</summary>
        public int IdMarca { get; set; }
        /// <summary>Nombre de la marca.</summary>
        public string? Marca { get; set; }
        /// <summary>Id de la categoria a la que pertenece.</summary>
        public int IdCategoria { get; set; }
        /// <summary>Nombre de la categoria.</summary>
        public string? Categoria { get; set; }
        /// <summary>Suma de los pares disponibles en todas las tallas.</summary>
        public int StockTotal { get; set; }
        /// <summary>Stock disponible por talla.</summary>
        public List<StockTallaDto> Tallas { get; set; } = new();
    }
}
