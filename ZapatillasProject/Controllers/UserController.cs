using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapatillasProject.DTOs;
using ZapatillasProject.Models;
using ZapatillasProject.Repositories;
using ZapatillasProject.Services;

namespace ZapatillasProject.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const int ROL_ADMIN = 1;
        private const int ROL_CLIENTE = 2;

        private readonly IUsuarioRepository _usuarios;
        private readonly IRepository<Rol> _roles;
        private readonly ITokenService _tokenService;

        public UserController(IUsuarioRepository usuarios, IRepository<Rol> roles, ITokenService tokenService)
        {
            _usuarios = usuarios;
            _roles = roles;
            _tokenService = tokenService;
        }

        /// <summary>Registra un nuevo usuario con rol Cliente (publico).</summary>
        // POST: api/v1/User/registro
        [HttpPost("registro")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            if (await _usuarios.ExistsAsync(u => u.Email == email))
                return Conflict(new { mensaje = "El email ya está registrado" });

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = email,
                //Encriptamos el password a utilizar
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRol = ROL_CLIENTE
            };
            await _usuarios.AddAsync(usuario);

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUser, version = "1" },
                new UsuarioDto { IdUser = usuario.IdUser, Nombre = usuario.Nombre, Email = usuario.Email, IdRol = usuario.IdRol, Rol = "Cliente" });
        }

        /// <summary>Inicia sesion y devuelve el token JWT (publico).</summary>
        // POST: api/v1/User/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
        {
            var user = await _usuarios.GetByEmailAsync(dto.Email.Trim().ToLower());
            if (user == null)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            //verificamos si la contraseña es válida
            bool pwdValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!pwdValid)
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });

            return Ok(_tokenService.GenerarToken(user));
        }

        /// <summary>Devuelve los datos del usuario logueado.</summary>
        // GET: api/v1/User/perfil
        [HttpGet("perfil")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> Perfil()
        {
            var idUser = int.Parse(User.FindFirst("id_user")!.Value);
            var usuario = (await _usuarios.GetAllConRolAsync()).FirstOrDefault(u => u.IdUser == idUser);
            if (usuario == null) return NotFound();
            return Ok(ToDto(usuario));
        }

        // GET: api/v1/User
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var lista = await _usuarios.GetAllConRolAsync();
            return Ok(lista.Select(ToDto));
        }

        // GET: api/v1/User/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = (await _usuarios.GetAllConRolAsync()).FirstOrDefault(u => u.IdUser == id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No existe el usuario con id {id}" });
            }
            return Ok(ToDto(usuario));
        }

        /// <summary>Cambia el rol de un usuario (solo Admin).</summary>
        // PUT: api/v1/User/5/rol
        [HttpPut("{id:int}/rol")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CambiarRol(int id, CambiarRolDto dto)
        {
            var usuario = await _usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No existe el usuario con id {id}" });
            }
            if (!await _roles.ExistsAsync(r => r.IdRol == dto.IdRol))
            {
                return BadRequest(new { mensaje = $"No existe el rol con id {dto.IdRol}" });
            }

            usuario.IdRol = dto.IdRol;
            await _usuarios.UpdateAsync(usuario);
            return NoContent();
        }

        // DELETE: api/v1/User/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No existe el usuario con id {id}" });
            }
            if (usuario.IdRol == ROL_ADMIN && usuario.IdUser == int.Parse(User.FindFirst("id_user")!.Value))
            {
                return BadRequest(new { mensaje = "No puedes eliminar tu propio usuario administrador" });
            }

            await _usuarios.DeleteAsync(usuario);
            return NoContent();
        }

        private static UsuarioDto ToDto(Usuario u)
        {
            return new UsuarioDto
            {
                IdUser = u.IdUser,
                Nombre = u.Nombre,
                Email = u.Email,
                IdRol = u.IdRol,
                Rol = u.IdRolNavigation?.Nombre
            };
        }
    }
}
