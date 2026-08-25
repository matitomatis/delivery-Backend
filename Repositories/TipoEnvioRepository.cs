using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public class TipoEnvioRepository : ITipoEnvioRepository // <-- ¡Ojo acá que coincida!
    {
        private readonly ApplicationDbContext _context;

        public TipoEnvioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoEnvio>> GetAllAsync() => await _context.TiposEnvio.ToListAsync();

        public async Task<TipoEnvio> GetByIdAsync(int id) => await _context.TiposEnvio.FindAsync(id);

        public async Task SaveAsync(TipoEnvio tipoEnvio)
        {
            if (tipoEnvio.CodTipoEnvio == 0) _context.TiposEnvio.Add(tipoEnvio);
            else _context.TiposEnvio.Update(tipoEnvio);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.TiposEnvio.FindAsync(id);
            if (entity != null)
            {
                _context.TiposEnvio.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}