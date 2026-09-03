using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using delivery.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Pedidos/Pendientes
        [HttpGet("Pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.Estado == "Pendiente")
                // Si tenés el Cliente relacionado, podés hacer un .Include(p => p.Cliente) acá
                .Select(p => new
                {
                    id = p.CodPedido, // Cambiar a p.CodPedido si así lo tenés en el model
                    fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    cliente = "Cliente", // Acá engancharías p.Cliente.Nombre si tenés la relación
                    total = p.Total
                })
                .OrderByDescending(p => p.id) // Los más nuevos primero
                .ToListAsync();

            return Ok(pedidos);
        }

        // PUT: api/Pedidos/5/Estado
        [HttpPut("{id}/Estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            pedido.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado actualizado" });
        }
    }
}