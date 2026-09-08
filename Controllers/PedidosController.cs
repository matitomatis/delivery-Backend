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
                .Include(p => p.Cliente) // <-- Agregamos esto para traer los datos del cliente unidos al pedido
                .Where(p => p.Estado == "Pendiente")
                .Select(p => new
                {
                    id = p.CodPedido,
                    fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),

                    // Acá armamos la etiqueta del Admin usando el ID real y los datos guardados
                    cliente = p.Cliente != null ? "#" + p.CodPedido + " - " + p.Cliente.Nombre : "#" + p.CodPedido + " - Sin Datos",

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
            var nuevoCliente = new Cliente
            {
                Nombre = dto.Cliente
            };

            _context.Clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            var nuevoPedido = new Pedido
            {
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                Total = dto.Total,
                CodCliente = nuevoCliente.CodCliente,
                CodFormaPago = 1,
                CodTipoEnvio = 1
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            // <-- ¡MAGIA ACÁ! Devolvemos el ID que SQL le acaba de asignar al pedido
            return Ok(new { id = nuevoPedido.CodPedido });
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

    public class PedidoNuevoDto
    {
        public string Cliente { get; set; }
        public decimal Total { get; set; }
    }
}