using delivery.Data;
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
                .Select(p => new
                {
                    id = p.CodPedido,
                    fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    cliente = "Cliente", // (Opcional: Si tenés la relación navegacional, podés usar p.Cliente.Nombre)
                    total = p.Total
                })
                .OrderByDescending(p => p.id)
                .ToListAsync();

            return Ok(pedidos);
        }

        // POST: api/Pedidos/Nuevo
        [HttpPost("Nuevo")]
        public async Task<IActionResult> CrearPedido([FromBody] PedidoNuevoDto dto)
        {
            // 1. Creamos al cliente en la base de datos
            var nuevoCliente = new Cliente
            {
                Nombre = dto.Cliente
            };

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            // 2. Armamos el pedido vinculando el ID del cliente y la forma de pago
            var nuevoPedido = new Pedido
            {
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                Total = dto.Total,

                // Vinculamos el cliente que acabamos de crear
                CodCliente = nuevoCliente.CodCliente,

                // ¡LA SOLUCIÓN AL ERROR! Le pasamos un ID válido de forma de pago
                CodFormaPago = 1,
                CodTipoEnvio = 1
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            return Ok();
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

    // Fuera del controlador para mantener el código ordenado
    public class PedidoNuevoDto
    {
        public string Cliente { get; set; }
        public decimal Total { get; set; }
    }
}