using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapatillasProject.DTOs;
using ZapatillasProject.Models;
using ZapatillasProject.Repositories;

namespace ZapatillasProject.Controllers
{
    // Stock de cada zapatilla por talla
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class StockTallasController : ControllerBase
    {
        private readonly IStockTallaRepository _stock;
        private readonly IRepository<Zapatilla> _zapatillas;

        public StockTallasController(IStockTallaRepository stock, IRepository<Zapatilla> zapatillas)
        {
            _stock = stock;
            _zapatillas = zapatillas;
        }

        /// <summary>Stock por talla de una zapatilla (publico).</summary>
        // GET: api/v1/StockTallas/zapatilla/5
        [HttpGet("zapatilla/{idZapatilla:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StockTallaDto>>> GetPorZapatilla(int idZapatilla)
        {
            if (!await _zapatillas.ExistsAsync(z => z.IdZapatilla == idZapatilla))
            {
                return NotFound(new { mensaje = $"No existe la zapatilla con id {idZapatilla}" });
            }
            var lista = await _stock.GetByZapatillaAsync(idZapatilla);
            return Ok(lista.Select(ToDto));
        }

        // GET: api/v1/StockTallas/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<StockTallaDto>> GetStock(int id)
        {
            var stock = await _stock.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound(new { mensaje = $"No existe el stock con id {id}" });
            }
            return Ok(ToDto(stock));
        }

        /// <summary>Agrega una talla con su cantidad a una zapatilla (solo Admin).</summary>
        // POST: api/v1/StockTallas
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StockTallaDto>> PostStock(StockTallaCreateDto dto)
        {
            if (!await _zapatillas.ExistsAsync(z => z.IdZapatilla == dto.IdZapatilla))
            {
                return BadRequest(new { mensaje = $"No existe la zapatilla con id {dto.IdZapatilla}" });
            }
            if (await _stock.GetByZapatillaYTallaAsync(dto.IdZapatilla, dto.Talla) != null)
            {
                return Conflict(new { mensaje = $"La talla {dto.Talla} ya existe para esta zapatilla, use PUT para modificarla" });
            }

            var stock = new StockTalla { IdZapatilla = dto.IdZapatilla, Talla = dto.Talla, Cantidad = dto.Cantidad };
            await _stock.AddAsync(stock);
            return CreatedAtAction(nameof(GetStock), new { id = stock.IdStock, version = "1" }, ToDto(stock));
        }

        /// <summary>Modifica la cantidad de una talla (solo Admin).</summary>
        // PUT: api/v1/StockTallas/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutStock(int id, StockTallaCreateDto dto)
        {
            var stock = await _stock.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound(new { mensaje = $"No existe el stock con id {id}" });
            }
            if (stock.IdZapatilla != dto.IdZapatilla)
            {
                return BadRequest(new { mensaje = "No se puede cambiar la zapatilla de un registro de stock" });
            }

            stock.Talla = dto.Talla;
            stock.Cantidad = dto.Cantidad;
            await _stock.UpdateAsync(stock);
            return NoContent();
        }

        // DELETE: api/v1/StockTallas/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _stock.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound(new { mensaje = $"No existe el stock con id {id}" });
            }
            await _stock.DeleteAsync(stock);
            return NoContent();
        }

        private static StockTallaDto ToDto(StockTalla s)
        {
            return new StockTallaDto { IdStock = s.IdStock, IdZapatilla = s.IdZapatilla, Talla = s.Talla, Cantidad = s.Cantidad };
        }
    }
}
