using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public class ZapatillaRepository : Repository<Zapatilla>, IZapatillaRepository
    {
        public ZapatillaRepository(ZapatillasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Zapatilla>> GetAllConDetalleAsync(int? idMarca, int? idCategoria)
        {
            var query = _context.Zapatillas
                .Include(z => z.IdMarcaNavigation)
                .Include(z => z.IdCategoriaNavigation)
                .Include(z => z.StockTallas)
                .AsQueryable();

            if (idMarca.HasValue)
                query = query.Where(z => z.IdMarca == idMarca.Value);

            if (idCategoria.HasValue)
                query = query.Where(z => z.IdCategoria == idCategoria.Value);

            return await query.OrderBy(z => z.IdZapatilla).ToListAsync();
        }

        public async Task<Zapatilla?> GetConDetalleAsync(int id)
        {
            return await _context.Zapatillas
                .Include(z => z.IdMarcaNavigation)
                .Include(z => z.IdCategoriaNavigation)
                .Include(z => z.StockTallas)
                .FirstOrDefaultAsync(z => z.IdZapatilla == id);
        }

        // al eliminar una zapatilla se eliminan tambien su stock por talla y sus reseñas
        public override async Task DeleteAsync(Zapatilla entity)
        {
            var stock = await _context.StockTallas.Where(s => s.IdZapatilla == entity.IdZapatilla).ToListAsync();
            var resenas = await _context.Resenas.Where(r => r.IdZapatilla == entity.IdZapatilla).ToListAsync();
            _context.StockTallas.RemoveRange(stock);
            _context.Resenas.RemoveRange(resenas);
            _context.Zapatillas.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneVentasAsync(int id)
        {
            return await _context.DetalleVenta.AnyAsync(d => d.IdZapatilla == id);
        }
    }
}
