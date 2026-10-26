using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public class StockTallaRepository : Repository<StockTalla>, IStockTallaRepository
    {
        public StockTallaRepository(ZapatillasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StockTalla>> GetByZapatillaAsync(int idZapatilla)
        {
            return await _context.StockTallas
                .Where(s => s.IdZapatilla == idZapatilla)
                .OrderBy(s => s.Talla)
                .ToListAsync();
        }

        public async Task<StockTalla?> GetByZapatillaYTallaAsync(int idZapatilla, decimal talla)
        {
            return await _context.StockTallas
                .FirstOrDefaultAsync(s => s.IdZapatilla == idZapatilla && s.Talla == talla);
        }
    }
}
