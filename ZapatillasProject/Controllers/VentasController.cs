using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapatillasProject.DTOs;
using ZapatillasProject.Models;
using ZapatillasProject.Repositories;

namespace ZapatillasProject.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize] // todas las ventas requieren token
    public class VentasController : ControllerBase
    {
        private readonly IVentaRepository _ventas;
        private readonly IRepository<Zapatilla> _zapatillas;
        private readonly IStockTallaRepository _stock;

        public VentasController(IVentaRepository ventas, IRepository<Zapatilla> zapatillas, IStockTallaRepository stock)
        {
            _ventas = ventas;
            _zapatillas = zapatillas;
            _stock = stock;
        }

        /// <summary>Lista todas las ventas (solo Admin).</summary>
        // GET: api/v1/Ventas
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas()
        {
            var lista = await _ventas.GetAllConDetalleAsync(null);
            return Ok(lista.Select(ToDto));
        }

        /// <summary>Lista las compras del usuario logueado.</summary>
        // GET: api/v1/Ventas/mis-compras
        [HttpGet("mis-compras")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetMisCompras()
        {
            var lista = await _ventas.GetAllConDetalleAsync(IdUsuarioActual());
            return Ok(lista.Select(ToDto));
        }

        /// <summary>Detalle de una venta. El Cliente solo puede ver sus propias ventas.</summary>
        // GET: api/v1/Ventas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaDto>> GetVenta(int id)
        {
            var venta = await _ventas.GetConDetalleAsync(id);
            if (venta == null)
            {
                return NotFound(new { mensaje = $"No existe la venta con id {id}" });
            }
            if (!User.IsInRole("Admin") && venta.IdUser != IdUsuarioActual())
            {
                return Forbid();
            }
            return Ok(ToDto(venta));
        }

        /// <summary>Registra una compra: calcula el monto con los precios actuales y descuenta el stock por talla.</summary>
        // POST: api/v1/Ventas
        [HttpPost]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<VentaDto>> PostVenta(VentaCreateDto dto)
        {
            var venta = new Ventum
            {
                NumeroVenta = await _ventas.SiguienteNumeroVentaAsync(),
                TipoPago = dto.TipoPago,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                IdUser = IdUsuarioActual()
            };

            var stockModificado = new Dictionary<int, StockTalla>();

            foreach (var item in dto.Detalles)
            {
                var zapatilla = await _zapatillas.GetByIdAsync(item.IdZapatilla);
                if (zapatilla == null)
                {
                    return BadRequest(new { mensaje = $"No existe la zapatilla con id {item.IdZapatilla}" });
                }

                var stock = await _stock.GetByZapatillaYTallaAsync(item.IdZapatilla, item.Talla);
                if (stock != null && stockModificado.ContainsKey(stock.IdStock))
                {
                    stock = stockModificado[stock.IdStock];
                }
                if (stock == null || stock.Cantidad <= 0)
                {
                    return Conflict(new { mensaje = $"Sin stock de {zapatilla.Modelo} en talla {item.Talla}" });
                }

                stock.Cantidad -= 1;
                stockModificado[stock.IdStock] = stock;

                // el precio se toma de la base de datos, no del cliente
                venta.DetalleVenta.Add(new DetalleVentum { IdZapatilla = zapatilla.IdZapatilla, Precio = zapatilla.Precio });
            }

            venta.Monto = venta.DetalleVenta.Sum(d => d.Precio);

            await _ventas.RegistrarVentaAsync(venta, stockModificado.Values);

            var creada = await _ventas.GetConDetalleAsync(venta.IdVenta);
            return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta, version = "1" }, ToDto(creada!));
        }

        /// <summary>Anula (elimina) una venta (solo Admin).</summary>
        // DELETE: api/v1/Ventas/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _ventas.GetConDetalleAsync(id);
            if (venta == null)
            {
                return NotFound(new { mensaje = $"No existe la venta con id {id}" });
            }
            await _ventas.DeleteAsync(venta);
            return NoContent();
        }

        private int IdUsuarioActual()
        {
            return int.Parse(User.FindFirst("id_user")!.Value);
        }

        private static VentaDto ToDto(Ventum v)
        {
            return new VentaDto
            {
                IdVenta = v.IdVenta,
                NumeroVenta = v.NumeroVenta,
                TipoPago = v.TipoPago,
                Monto = v.Monto,
                Fecha = v.Fecha,
                IdUser = v.IdUser,
                Detalles = v.DetalleVenta.Select(d => new DetalleVentaDto
                {
                    IdDetalle = d.IdDetalle,
                    IdZapatilla = d.IdZapatilla,
                    Modelo = d.IdZapatillaNavigation?.Modelo,
                    Precio = d.Precio
                }).ToList()
            };
        }
    }
}
