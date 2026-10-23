using delivery.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace delivery.Repositories
{
    public interface IGustoRepository
    {
        Task<List<Gusto>> GetAllAsync();
        Task<Gusto> GetByIdAsync(int id);
        Task SaveAsync(Gusto gusto);
        Task DeleteAsync(int id);
    }

    
}
