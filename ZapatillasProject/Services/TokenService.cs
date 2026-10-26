using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZapatillasProject.DTOs;
using ZapatillasProject.Models;

namespace ZapatillasProject.Services
{
    // Genera el JWT. Es Singleton porque no guarda estado y solo lee la configuracion.
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public TokenDto GenerarToken(Usuario user)
        {
            var rol = user.IdRolNavigation?.Nombre ?? "Cliente";

            // incorporamos los datos reales del usuario en los claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("id_user", user.IdUser.ToString()),
                new Claim("nombre", user.Nombre),
                new Claim(ClaimTypes.Role, rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddMinutes(60);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expira,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return new TokenDto
            {
                Token = tokenHandler.WriteToken(token),
                Expira = expira,
                Nombre = user.Nombre,
                Rol = rol
            };
        }
    }
}
