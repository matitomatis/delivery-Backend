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

        public async Task<List<FormaPago>> GetAllAsync() => await _context.FormasPago.ToListAsync();

        public async Task<FormaPago> GetByIdAsync(int id) => await _context.FormasPago.FindAsync(id);

        public async Task SaveAsync(FormaPago formaPago)
        {
            if (formaPago.CodFormaPago == 0) _context.FormasPago.Add(formaPago);
            else _context.FormasPago.Update(formaPago);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.FormasPago.FindAsync(id);
            if (entity != null)
            {
                _context.FormasPago.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
