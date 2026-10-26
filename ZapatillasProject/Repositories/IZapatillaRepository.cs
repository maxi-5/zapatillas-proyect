using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public interface IZapatillaRepository : IRepository<Zapatilla>
    {
        // trae las zapatillas con su marca, categoria y stock (con filtros opcionales)
        Task<IEnumerable<Zapatilla>> GetAllConDetalleAsync(int? idMarca, int? idCategoria);
        Task<Zapatilla?> GetConDetalleAsync(int id);
        Task<bool> TieneVentasAsync(int id);
    }
}
