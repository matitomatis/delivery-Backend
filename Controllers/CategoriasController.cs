using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            // Apagamos la validación estricta de las relaciones
            ModelState.Remove("Articulos");
            ModelState.Remove("Banner");
            ModelState.Remove("Rubro");

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.CodCategoria }, categoria);
        }

        // --- MÉTODO AGREGADO PARA EDITAR ---
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCategoria(int id, [FromBody] Categoria catEditada)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = catEditada.Nombre;
            categoria.CodRubro = catEditada.CodRubro;

            await _context.SaveChangesAsync();
            return Ok();
        }
        // -----------------------------------

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}