using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapatillasProject.DTOs;
using ZapatillasProject.Models;
using ZapatillasProject.Repositories;

namespace ZapatillasProject.Controllers
{
    // RECURSO PRINCIPAL: CRUD completo de zapatillas
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ZapatillasController : ControllerBase
    {
        private readonly IZapatillaRepository _zapatillas;
        private readonly IRepository<Marca> _marcas;
        private readonly IRepository<Categorium> _categorias;

        public ZapatillasController(IZapatillaRepository zapatillas, IRepository<Marca> marcas, IRepository<Categorium> categorias)
        {
            _zapatillas = zapatillas;
            _marcas = marcas;
            _categorias = categorias;
        }

        /// <summary>Lista el catalogo de zapatillas (publico). Se puede filtrar por marca y/o categoria.</summary>
        /// <param name="idMarca">Opcional. Id de la marca: 1 Nike, 2 Adidas, 3 Puma, 4 Vans.</param>
        /// <param name="idCategoria">Opcional. Id de la categoria: 1 Running, 2 Urbana, 3 Basketball, 4 Skate.</param>
        // GET: api/v1/Zapatillas?idMarca=1&idCategoria=2
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ZapatillaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ZapatillaDto>>> GetZapatillas([FromQuery] int? idMarca, [FromQuery] int? idCategoria)
        {
            var lista = await _zapatillas.GetAllConDetalleAsync(idMarca, idCategoria);
            return Ok(lista.Select(ToDto));
        }

        /// <summary>Obtiene una zapatilla por su id (publico).</summary>
        // GET: api/v1/Zapatillas/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ZapatillaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ZapatillaDto>> GetZapatilla(int id)
        {
            var zapatilla = await _zapatillas.GetConDetalleAsync(id);

            if (zapatilla == null)
            {
                return NotFound(new { mensaje = $"No existe la zapatilla con id {id}" });
            }

            return Ok(ToDto(zapatilla));
        }

        /// <summary>Crea una zapatilla (solo Admin).</summary>
        // POST: api/v1/Zapatillas
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ZapatillaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ZapatillaDto>> PostZapatilla(ZapatillaCreateDto dto)
        {
            var error = await ValidarRelaciones(dto);
            if (error != null) return error;

            var zapatilla = new Zapatilla
            {
                Modelo = dto.Modelo.Trim(),
                Precio = dto.Precio,
                IdCategoria = dto.IdCategoria,
                IdMarca = dto.IdMarca,
                Descripcion = dto.Descripcion
            };

            await _zapatillas.AddAsync(zapatilla);

            var creada = await _zapatillas.GetConDetalleAsync(zapatilla.IdZapatilla);
            return CreatedAtAction(nameof(GetZapatilla), new { id = zapatilla.IdZapatilla, version = "1" }, ToDto(creada!));
        }

        /// <summary>Actualiza una zapatilla (solo Admin).</summary>
        // PUT: api/v1/Zapatillas/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutZapatilla(int id, ZapatillaCreateDto dto)
        {
            var zapatilla = await _zapatillas.GetByIdAsync(id);
            if (zapatilla == null)
            {
                return NotFound(new { mensaje = $"No existe la zapatilla con id {id}" });
            }

            var error = await ValidarRelaciones(dto);
            if (error != null) return error;

            zapatilla.Modelo = dto.Modelo.Trim();
            zapatilla.Precio = dto.Precio;
            zapatilla.IdCategoria = dto.IdCategoria;
            zapatilla.IdMarca = dto.IdMarca;
            zapatilla.Descripcion = dto.Descripcion;

            await _zapatillas.UpdateAsync(zapatilla);

            return NoContent();
        }

        /// <summary>Elimina una zapatilla (solo Admin). No se puede eliminar si ya tiene ventas.</summary>
        // DELETE: api/v1/Zapatillas/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteZapatilla(int id)
        {
            var zapatilla = await _zapatillas.GetConDetalleAsync(id);
            if (zapatilla == null)
            {
                return NotFound(new { mensaje = $"No existe la zapatilla con id {id}" });
            }

            if (await _zapatillas.TieneVentasAsync(id))
            {
                return Conflict(new { mensaje = "No se puede eliminar: la zapatilla tiene ventas registradas" });
            }

            await _zapatillas.DeleteAsync(zapatilla);

            return NoContent();
        }

        // valida que la marca y la categoria existan antes de guardar
        private async Task<ActionResult?> ValidarRelaciones(ZapatillaCreateDto dto)
        {
            if (!await _marcas.ExistsAsync(m => m.IdMarca == dto.IdMarca))
            {
                ModelState.AddModelError(nameof(dto.IdMarca), $"No existe la marca con id {dto.IdMarca}");
            }
            if (!await _categorias.ExistsAsync(c => c.IdCategoria == dto.IdCategoria))
            {
                ModelState.AddModelError(nameof(dto.IdCategoria), $"No existe la categoria con id {dto.IdCategoria}");
            }
            return ModelState.IsValid ? null : ValidationProblem(ModelState);
        }

        private static ZapatillaDto ToDto(Zapatilla z)
        {
            return new ZapatillaDto
            {
                IdZapatilla = z.IdZapatilla,
                Modelo = z.Modelo,
                Precio = z.Precio,
                Descripcion = z.Descripcion,
                IdMarca = z.IdMarca,
                Marca = z.IdMarcaNavigation?.Nombre,
                IdCategoria = z.IdCategoria,
                Categoria = z.IdCategoriaNavigation?.Nombre,
                StockTotal = z.StockTallas.Sum(s => s.Cantidad),
                Tallas = z.StockTallas
                    .OrderBy(s => s.Talla)
                    .Select(s => new StockTallaDto { IdStock = s.IdStock, IdZapatilla = s.IdZapatilla, Talla = s.Talla, Cantidad = s.Cantidad })
                    .ToList()
            };
        }
    }
}
