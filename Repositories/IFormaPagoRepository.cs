using delivery.Models;

namespace delivery.Repositories
{
    public interface IFormaPagoRepository
    {
        Task<List<FormaPago>> GetAllAsync();
        Task<FormaPago> GetByIdAsync(int id);
        Task SaveAsync(FormaPago formaPago);
        Task DeleteAsync(int id);
    }
}
