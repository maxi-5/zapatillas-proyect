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
    public class ResenasController : ControllerBase
    {
        private readonly IResenaRepository _resenas;
        private readonly IRepository<Zapatilla> _zapatillas;

        public ResenasController(IResenaRepository resenas, IRepository<Zapatilla> zapatillas)
        {
            _resenas = resenas;
            _zapatillas = zapatillas;
        }

        /// <summary>Reseñas de una zapatilla (publico).</summary>
        // GET: api/v1/Resenas/zapatilla/5
        [HttpGet("zapatilla/{idZapatilla:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ResenaDto>>> GetPorZapatilla(int idZapatilla)
        {
            if (!await _zapatillas.ExistsAsync(z => z.IdZapatilla == idZapatilla))
            {
                return NotFound(new { mensaje = $"No existe la zapatilla con id {idZapatilla}" });
            }
            var lista = await _resenas.GetByZapatillaAsync(idZapatilla);
            return Ok(lista.Select(r => new ResenaDto
            {
                IdResena = r.IdResena,
                IdZapatilla = r.IdZapatilla,
                Resena = r.Resena1,
                Valoracion = r.Valoracion,
                Usuario = r.IdUserNavigation?.Nombre
            }));
        }

        /// <summary>Publica una reseña (Cliente o Admin logueado). El usuario se toma del token.</summary>
        // POST: api/v1/Resenas
        [HttpPost]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<ResenaDto>> PostResena(ResenaCreateDto dto)
        {
            if (!await _zapatillas.ExistsAsync(z => z.IdZapatilla == dto.IdZapatilla))
            {
                return BadRequest(new { mensaje = $"No existe la zapatilla con id {dto.IdZapatilla}" });
            }

            var resena = new Resena
            {
                IdZapatilla = dto.IdZapatilla,
                Resena1 = dto.Resena.Trim(),
                Valoracion = dto.Valoracion,
                IdUser = int.Parse(User.FindFirst("id_user")!.Value)
            };
            await _resenas.AddAsync(resena);

            return StatusCode(StatusCodes.Status201Created, new ResenaDto
            {
                IdResena = resena.IdResena,
                IdZapatilla = resena.IdZapatilla,
                Resena = resena.Resena1,
                Valoracion = resena.Valoracion,
                Usuario = User.FindFirst("nombre")?.Value
            });
        }

        /// <summary>Elimina una reseña (solo Admin, para moderar).</summary>
        // DELETE: api/v1/Resenas/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteResena(int id)
        {
            var resena = await _resenas.GetByIdAsync(id);
            if (resena == null)
            {
                return NotFound(new { mensaje = $"No existe la reseña con id {id}" });
            }
            await _resenas.DeleteAsync(resena);
            return NoContent();
        }
    }
}
