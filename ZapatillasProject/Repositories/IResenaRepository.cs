using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public interface IResenaRepository : IRepository<Resena>
    {
        Task<IEnumerable<Resena>> GetByZapatillaAsync(int idZapatilla);
    }
}
