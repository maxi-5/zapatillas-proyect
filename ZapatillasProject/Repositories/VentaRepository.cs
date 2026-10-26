using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public class VentaRepository : Repository<Ventum>, IVentaRepository
    {
        public VentaRepository(ZapatillasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ventum>> GetAllConDetalleAsync(int? idUser)
        {
            var query = _context.Venta
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.IdZapatillaNavigation)
                .AsQueryable();

            if (idUser.HasValue)
                query = query.Where(v => v.IdUser == idUser.Value);

            return await query.OrderByDescending(v => v.IdVenta).ToListAsync();
        }

        public async Task<Ventum?> GetConDetalleAsync(int id)
        {
            return await _context.Venta
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.IdZapatillaNavigation)
                .FirstOrDefaultAsync(v => v.IdVenta == id);
        }

        public async Task<int> SiguienteNumeroVentaAsync()
        {
            var ultimo = await _context.Venta.MaxAsync(v => (int?)v.NumeroVenta) ?? 1000;
            return ultimo + 1;
        }

        // al anular una venta se eliminan sus detalles
        public override async Task DeleteAsync(Ventum entity)
        {
            var detalles = await _context.DetalleVenta.Where(d => d.IdVenta == entity.IdVenta).ToListAsync();
            _context.DetalleVenta.RemoveRange(detalles);
            _context.Venta.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarVentaAsync(Ventum venta, IEnumerable<StockTalla> stockADescontar)
        {
            // SaveChanges ejecuta todo (venta + detalles + stock) en una sola transaccion
            foreach (var stock in stockADescontar)
            {
                _context.StockTallas.Update(stock);
            }
            await _context.Venta.AddAsync(venta);
            await _context.SaveChangesAsync();
        }
    }
}
