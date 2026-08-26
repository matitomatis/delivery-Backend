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
            // 1. Ponemos el total en 0
            pedido.Total = 0;

            // 2. Recorremos cada detalle que el cliente pidió
            if (pedido.Detalles != null && pedido.Detalles.Count > 0)
            {
                foreach (var item in pedido.Detalles)
                {
                    // Sumamos al total del pedido
                    pedido.Total += (item.Cantidad * item.PrecioUnitario);

                    // --- LÓGICA DE STOCK ---
                    if (item.CodArticulo.HasValue)
                    {
                        var articulo = await _context.Articulos.FindAsync(item.CodArticulo.Value);
                        if (articulo != null)
                        {
                            articulo.Stock -= item.Cantidad;
                        }
                    }
                }
            }

            // 3. Guardamos todo junto
            if (pedido.CodPedido == 0)
                _context.Pedidos.Add(pedido);
            else
                _context.Pedidos.Update(pedido);

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
