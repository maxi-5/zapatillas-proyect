using ZapatillasProject.DTOs;
using ZapatillasProject.Models;

namespace ZapatillasProject.Services
{
    public interface ITokenService
    {
        TokenDto GenerarToken(Usuario user);
    }
}
