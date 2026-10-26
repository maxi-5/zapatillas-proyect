using ZapatillasProject.Models;

namespace ZapatillasProject.Repositories
{
    public interface IVentaRepository : IRepository<Ventum>
    {
        Task<IEnumerable<Ventum>> GetAllConDetalleAsync(int? idUser);
        Task<Ventum?> GetConDetalleAsync(int id);
        Task<int> SiguienteNumeroVentaAsync();
        // registra la venta y descuenta el stock en una sola transaccion
        Task RegistrarVentaAsync(Ventum venta, IEnumerable<StockTalla> stockADescontar);
    }
}
