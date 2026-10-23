using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public class FormaPagoRepository : IFormaPagoRepository
    {
        private readonly ApplicationDbContext _context;

        public FormaPagoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FormaPago>> GetAllAsync() => await _context.FormasPagos.ToListAsync();

        public async Task<FormaPago> GetByIdAsync(int id) => await _context.FormasPagos.FindAsync(id);

        public async Task SaveAsync(FormaPago formaPago)
        {
            if (formaPago.CodFormaPago == 0) _context.FormasPagos.Add(formaPago);
            else _context.FormasPagos.Update(formaPago);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.FormasPagos.FindAsync(id);
            if (entity != null)
            {
                _context.FormasPagos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
