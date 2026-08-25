using delivery.Models;

namespace delivery.Repositories
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllAsync();
        Task<Pedido> GetByIdAsync(int id);
        Task SaveAsync(Pedido pedido);
        Task DeleteAsync(int id);
    }
}
