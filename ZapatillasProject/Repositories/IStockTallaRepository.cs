using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public interface IStockTallaRepository : IRepository<StockTalla>
    {
        Task<IEnumerable<StockTalla>> GetByZapatillaAsync(int idZapatilla);
        Task<StockTalla?> GetByZapatillaYTallaAsync(int idZapatilla, decimal talla);
    }
}
