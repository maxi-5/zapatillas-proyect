using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public class ResenaRepository : Repository<Resena>, IResenaRepository
    {
        public ResenaRepository(ZapatillasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Resena>> GetByZapatillaAsync(int idZapatilla)
        {
            return await _context.Resenas
                .Include(r => r.IdUserNavigation)
                .Where(r => r.IdZapatilla == idZapatilla)
                .OrderByDescending(r => r.IdResena)
                .ToListAsync();
        }
    }
}
