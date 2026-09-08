using delivery.Data;
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RubrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RubrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Rubros
        [HttpGet]
        public async Task<IActionResult> GetRubros()
        {
            var rubros = await _context.Rubros.ToListAsync();
            return Ok(rubros);
        }

        // POST: api/Rubros
        [HttpPost]
        public async Task<IActionResult> CrearRubro([FromBody] Rubro rubro)
        {
            _context.Rubros.Add(rubro);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarRubro(int id, [FromBody] Rubro rubroEditado)
        {
            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro == null) return NotFound();

            rubro.Nombre = rubroEditado.Nombre;
            await _context.SaveChangesAsync();
            return Ok();
        }
        // DELETE: api/Rubros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRubro(int id)
        {
            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro == null) return NotFound();

            _context.Rubros.Remove(rubro);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}