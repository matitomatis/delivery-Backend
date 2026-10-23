using delivery.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync();
        Task<Cliente> GetByIdAsync(int id);
        Task SaveAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}
