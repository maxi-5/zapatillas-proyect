using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    public class StockTallaCreateDto
    {
        /// <summary>Id de la zapatilla a la que se le agrega stock (ver GET /Zapatillas).</summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una zapatilla valida")]
        public int IdZapatilla { get; set; }

        /// <summary>Talla (numero chileno, puede llevar .5).</summary>
        /// <example>42</example>
        [Range(typeof(decimal), "15", "50", ErrorMessage = "La talla debe estar entre 15 y 50")]
        public decimal Talla { get; set; }

        /// <summary>Cantidad de pares disponibles en esa talla.</summary>
        /// <example>5</example>
        [Range(0, 10000, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }
    }

    public class StockTallaDto
    {
        /// <summary>Id del registro de stock (se usa en PUT y DELETE de /StockTallas).</summary>
        public int IdStock { get; set; }
        /// <summary>Id de la zapatilla a la que pertenece este stock.</summary>
        public int IdZapatilla { get; set; }
        public decimal Talla { get; set; }
        public int Cantidad { get; set; }
    }
}
