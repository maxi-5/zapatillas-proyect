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
    public class CategoriasController : ControllerBase
    {
        private readonly IRepository<Categorium> _repo;
        private readonly IRepository<Zapatilla> _zapatillas;

        public CategoriasController(IRepository<Categorium> repo, IRepository<Zapatilla> zapatillas)
        {
            _repo = repo;
            _zapatillas = zapatillas;
        }

        // GET: api/v1/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var lista = await _repo.GetAllAsync();
            return Ok(lista.Select(c => new CategoriaDto { IdCategoria = c.IdCategoria, Nombre = c.Nombre }));
        }

        // GET: api/v1/Categorias/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _repo.GetByIdAsync(id);
            if (categoria == null)
            {
                return NotFound(new { mensaje = $"No existe la categoria con id {id}" });
            }
            return Ok(new CategoriaDto { IdCategoria = categoria.IdCategoria, Nombre = categoria.Nombre });
        }

        // POST: api/v1/Categorias
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoriaDto>> PostCategoria(CategoriaDto dto)
        {
            var categoria = new Categorium { Nombre = dto.Nombre.Trim() };
            await _repo.AddAsync(categoria);
            dto.IdCategoria = categoria.IdCategoria;
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.IdCategoria, version = "1" }, dto);
        }

        // PUT: api/v1/Categorias/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDto dto)
        {
            if (dto.IdCategoria != 0 && id != dto.IdCategoria)
            {
                return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo" });
            }

            var categoria = await _repo.GetByIdAsync(id);
            if (categoria == null)
            {
                return NotFound(new { mensaje = $"No existe la categoria con id {id}" });
            }

            categoria.Nombre = dto.Nombre.Trim();
            await _repo.UpdateAsync(categoria);
            return NoContent();
        }

        // DELETE: api/v1/Categorias/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _repo.GetByIdAsync(id);
            if (categoria == null)
            {
                return NotFound(new { mensaje = $"No existe la categoria con id {id}" });
            }
            if (await _zapatillas.ExistsAsync(z => z.IdCategoria == id))
            {
                return Conflict(new { mensaje = "No se puede eliminar: hay zapatillas en esta categoria" });
            }

            await _repo.DeleteAsync(categoria);
            return NoContent();
        }
    }
}
