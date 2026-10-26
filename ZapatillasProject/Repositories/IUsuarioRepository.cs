using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetAllConRolAsync();
    }
}
