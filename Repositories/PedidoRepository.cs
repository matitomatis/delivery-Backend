using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pedido>> GetAllAsync()
        {
            // Traemos el pedido con TODOS sus datos relacionados
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.FormaPago)
                .Include(p => p.TipoEnvio)
                .Include(p => p.Detalles)
                .ToListAsync();
        }

        public async Task<Pedido> GetByIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.FormaPago)
                .Include(p => p.TipoEnvio)
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.CodPedido == id);
        }

        public async Task SaveAsync(Pedido pedido)
        {
            if (pedido.CodPedido == 0) _context.Pedidos.Add(pedido);
            else _context.Pedidos.Update(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Pedidos.FindAsync(id);
            if (entity != null)
            {
                _context.Pedidos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
