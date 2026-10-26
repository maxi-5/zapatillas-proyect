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
    [Authorize(Roles = "Admin")] // solo el administrador gestiona roles
    public class RolesController : ControllerBase
    {
        private readonly IRepository<Rol> _repo;

        public RolesController(IRepository<Rol> repo)
        {
            _repo = repo;
        }

        // GET: api/v1/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDto>>> GetRols()
        {
            var lista = await _repo.GetAllAsync();
            return Ok(lista.Select(r => new RolDto { IdRol = r.IdRol, Nombre = r.Nombre }));
        }

        // GET: api/v1/Roles/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RolDto>> GetRol(int id)
        {
            var rol = await _repo.GetByIdAsync(id);
            if (rol == null)
            {
                return NotFound(new { mensaje = $"No existe el rol con id {id}" });
            }
            return Ok(new RolDto { IdRol = rol.IdRol, Nombre = rol.Nombre });
        }
    }
}
