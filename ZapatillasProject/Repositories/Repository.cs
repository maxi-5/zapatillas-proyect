using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    // Implementacion generica con Entity Framework Core.
    // Los controladores NO usan el DbContext directamente, usan el repositorio.
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ZapatillasDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ZapatillasDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filtro)
        {
            return await _dbSet.AnyAsync(filtro);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
