using delivery.Models;

namespace delivery.Repositories
{
    public interface IArticuloRepository
    {
        Task<List<Articulo>> GetAllAsync();
        Task<Articulo> GetByIdAsync(int id);
        Task SaveAsync(Articulo articulo);
        Task DeleteAsync(int id);
    }
}
