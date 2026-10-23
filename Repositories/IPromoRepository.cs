using delivery.Models;

namespace delivery.Repositories
{
    public interface IPromoRepository
    {
        Task<List<Promo>> GetAllAsync();
        Task<Promo> GetByIdAsync(int id);
        Task SaveAsync(Promo promo);
        Task DeleteAsync(int id);
    }
}
