using delivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            // MAGIA APLICADA: Hacemos el JOIN con la tabla de imágenes extra
            return await _context.Articulos
                                 .Include(a => a.ImagenesArticulos)
                                 .ToListAsync();
        }

        public async Task<Articulo> GetByIdAsync(int id)
        {
            // MAGIA APLICADA: Cambiamos FindAsync por FirstOrDefaultAsync para que nos permita usar el Include
            return await _context.Articulos
                                 .Include(a => a.ImagenesArticulos)
                                 .FirstOrDefaultAsync(a => a.CodArticulo == id);
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