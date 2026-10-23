using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public class GustoRepository : IGustoRepository
    {
        private readonly ApplicationDbContext _context;
        public GustoRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Gusto>> GetAllAsync() => await _context.Gustos.ToListAsync();

        public async Task<Gusto> GetByIdAsync(int id) => await _context.Gustos.FindAsync(id);
        public async Task DeleteAsync(int id)
        {
            var gusto = await _context.Gustos.FindAsync(id);
            if (gusto != null)
            {
                _context.Gustos.Remove(gusto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync(Gusto gusto)
        {
            if (gusto.Id == 0) _context.Gustos.Add(gusto);
            else _context.Gustos.Update(gusto);
            await _context.SaveChangesAsync();
        }
    }
}
