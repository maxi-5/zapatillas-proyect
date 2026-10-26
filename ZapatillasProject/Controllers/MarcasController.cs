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
    [Authorize] // bloquea todos los endpoints, se necesita token
    public class MarcasController : ControllerBase
    {
        private readonly IRepository<Marca> _repo;
        private readonly IRepository<Zapatilla> _zapatillas;

        public MarcasController(IRepository<Marca> repo, IRepository<Zapatilla> zapatillas)
        {
            _repo = repo;
            _zapatillas = zapatillas;
        }

        // GET: api/v1/Marcas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcaDto>>> GetMarcas()
        {
            var lista = await _repo.GetAllAsync();
            return Ok(lista.Select(m => new MarcaDto { IdMarca = m.IdMarca, Nombre = m.Nombre, Pais = m.Pais }));
        }

        // GET: api/v1/Marcas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MarcaDto>> GetMarca(int id)
        {
            var marca = await _repo.GetByIdAsync(id);
            if (marca == null)
            {
                return NotFound(new { mensaje = $"No existe la marca con id {id}" });
            }
            return Ok(new MarcaDto { IdMarca = marca.IdMarca, Nombre = marca.Nombre, Pais = marca.Pais });
        }

        // POST: api/v1/Marcas
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MarcaDto>> PostMarca(MarcaDto dto)
        {
            var marca = new Marca { Nombre = dto.Nombre.Trim(), Pais = dto.Pais.Trim() };
            await _repo.AddAsync(marca);
            dto.IdMarca = marca.IdMarca;
            return CreatedAtAction(nameof(GetMarca), new { id = marca.IdMarca, version = "1" }, dto);
        }

        // PUT: api/v1/Marcas/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutMarca(int id, MarcaDto dto)
        {
            if (dto.IdMarca != 0 && id != dto.IdMarca)
            {
                return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo" });
            }

            var marca = await _repo.GetByIdAsync(id);
            if (marca == null)
            {
                return NotFound(new { mensaje = $"No existe la marca con id {id}" });
            }

            marca.Nombre = dto.Nombre.Trim();
            marca.Pais = dto.Pais.Trim();
            await _repo.UpdateAsync(marca);
            return NoContent();
        }

        // DELETE: api/v1/Marcas/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMarca(int id)
        {
            var marca = await _repo.GetByIdAsync(id);
            if (marca == null)
            {
                return NotFound(new { mensaje = $"No existe la marca con id {id}" });
            }
            if (await _zapatillas.ExistsAsync(z => z.IdMarca == id))
            {
                return Conflict(new { mensaje = "No se puede eliminar: hay zapatillas de esta marca" });
            }

            await _repo.DeleteAsync(marca);
            return NoContent();
        }
    }
}
