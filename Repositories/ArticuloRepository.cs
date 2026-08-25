using delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace delivery.Repositories
{
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly ApplicationDbContext _context;

        // Inyectamos el DbContext por el constructor
        public ArticuloRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Articulo>> GetAllAsync()
        {
            return await _context.Articulos.ToListAsync();
        }

        public async Task<Articulo> GetByIdAsync(int id)
        {
            // Busca por clave primaria
            return await _context.Articulos.FindAsync(id);
        }

        public async Task SaveAsync(Articulo articulo)
        {
            // Si el código es 0, significa que es un artículo nuevo
            if (articulo.CodArticulo == 0)
            {
                _context.Articulos.Add(articulo);
            }
            else
            {
                // Si ya tiene código, lo actualizamos
                _context.Articulos.Update(articulo);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo != null)
            {
                _context.Articulos.Remove(articulo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
