using Microsoft.EntityFrameworkCore;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ZapatillasDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        // al eliminar un usuario sus reseñas y ventas quedan sin usuario (id_user acepta NULL)
        public override async Task DeleteAsync(Usuario entity)
        {
            var resenas = await _context.Resenas.Where(r => r.IdUser == entity.IdUser).ToListAsync();
            var ventas = await _context.Venta.Where(v => v.IdUser == entity.IdUser).ToListAsync();
            resenas.ForEach(r => r.IdUser = null);
            ventas.ForEach(v => v.IdUser = null);
            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Usuario>> GetAllConRolAsync()
        {
            return await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .ToListAsync();
        }
    }
}
