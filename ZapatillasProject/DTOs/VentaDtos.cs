using System.ComponentModel.DataAnnotations;

namespace ZapatillasProject.DTOs
{
    public class VentaCreateDto
    {
        /// <summary>Forma de pago: Efectivo, Debito, Credito o Transferencia.</summary>
        /// <example>Debito</example>
        [Required(ErrorMessage = "El tipo de pago es obligatorio")]
        [RegularExpression("^(Efectivo|Debito|Credito|Transferencia)$",
            ErrorMessage = "Tipo de pago valido: Efectivo, Debito, Credito o Transferencia")]
        public string TipoPago { get; set; } = null!;

        /// <summary>Pares que se compran (uno por cada elemento de la lista).</summary>
        [Required]
        [MinLength(1, ErrorMessage = "La venta debe tener al menos una zapatilla")]
        public List<DetalleVentaCreateDto> Detalles { get; set; } = new();
    }

    // Cada detalle corresponde a 1 par de zapatillas de una talla
    public class DetalleVentaCreateDto
    {
        /// <summary>Id de la zapatilla que se compra (ver GET /Zapatillas).</summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una zapatilla valida")]
        public int IdZapatilla { get; set; }

        /// <summary>Talla que se compra; debe tener stock (ver GET /StockTallas/zapatilla/{idZapatilla}).</summary>
        /// <example>42</example>
        [Range(typeof(decimal), "15", "50", ErrorMessage = "La talla debe estar entre 15 y 50")]
        public decimal Talla { get; set; }
    }

    public class VentaDto
    {
        /// <summary>Id interno de la venta (se usa en GET y DELETE /Ventas/{id}).</summary>
        public int IdVenta { get; set; }
        /// <summary>Numero de boleta visible para el cliente (parte en 1001).</summary>
        public int NumeroVenta { get; set; }
        public string TipoPago { get; set; } = null!;
        /// <summary>Total de la venta, calculado por el servidor.</summary>
        public int Monto { get; set; }
        public DateOnly Fecha { get; set; }
        /// <summary>Id del usuario que hizo la compra.</summary>
        public int? IdUser { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaDto
    {
        /// <summary>Id del detalle (cada par comprado).</summary>
        public int IdDetalle { get; set; }
        /// <summary>Id de la zapatilla comprada.</summary>
        public int IdZapatilla { get; set; }
        public string? Modelo { get; set; }
        /// <summary>Precio de la zapatilla al momento de la compra.</summary>
        public int Precio { get; set; }
    }
}
