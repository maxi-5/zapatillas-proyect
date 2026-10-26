using System.Linq.Expressions;

namespace ZapatillasProject.Repositories
{
    // Repositorio generico: operaciones CRUD comunes para cualquier entidad
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filtro);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
