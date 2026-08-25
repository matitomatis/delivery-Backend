using delivery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public interface ITipoEnvioRepository
    {
        Task<List<TipoEnvio>> GetAllAsync();
        Task<TipoEnvio> GetByIdAsync(int id);
        Task SaveAsync(TipoEnvio tipoEnvio);
        Task DeleteAsync(int id);
    }
}
