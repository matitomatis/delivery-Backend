using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public class PromoRepository : IPromoRepository
    {
        private readonly ApplicationDbContext _context;

        public PromoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Promo>> GetAllAsync()
        {
            // El Include une la tabla Promos con DetallePromos
            return await _context.Promos
                .Include(p => p.DetallePromos)
                .ToListAsync();
        }

        public async Task<Promo> GetByIdAsync(int id)
        {
            return await _context.Promos
                .Include(p => p.DetallePromos)
                .FirstOrDefaultAsync(p => p.CodPromo == id);
        }

        public async Task SaveAsync(Promo promo)
        {
            if (promo.CodPromo == 0)
            {
                _context.Promos.Add(promo);
            }
            else
            {
                _context.Promos.Update(promo);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var promo = await _context.Promos.FindAsync(id);
            if (promo != null)
            {
                _context.Promos.Remove(promo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
