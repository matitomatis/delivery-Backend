using delivery.Data;
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para obtener siempre la hora de Argentina
        private DateTime ObtenerHoraArgentina()
        {
            TimeZoneInfo argTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, argTimeZone);
        }

        [HttpGet("Pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.Estado == "Pendiente")
                .Select(p => new
                {
                    id = p.CodPedido,
                    fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    cliente = p.Cliente != null ? "#" + p.CodPedido + " - " + p.Cliente.Nombre : "#" + p.CodPedido + " - Sin Datos",
                    total = p.Total
                })
                .OrderByDescending(p => p.id)
                .ToListAsync();

            return Ok(pedidos);
        }

        [HttpGet("Historial")]
        public async Task<IActionResult> GetHistorial(int anio, int mes)
        {
            var historial = await _context.Pedidos
                .Include(p => p.Cliente)
                .Where(p => p.Estado != "Pendiente"
                         && p.Fecha.Year == anio
                         && p.Fecha.Month == mes)
                .Select(p => new
                {
                    id = p.CodPedido,
                    fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    cliente = p.Cliente != null ? "#" + p.CodPedido + " - " + p.Cliente.Nombre : "#" + p.CodPedido + " - Sin Datos",
                    estado = p.Estado,
                    total = p.Total
                })
                .OrderByDescending(p => p.id)
                .ToListAsync();

            return Ok(historial);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.CodPedido == id);

            if (pedido == null) return NotFound();

            var detalles = await _context.Set<DetallePedido>()
                .Include(d => d.Articulo)
                .Include(d => d.Promo)
                .Where(d => d.CodPedido == id)
                .Select(d => new
                {
                    cantidad = d.Cantidad,
                    precioUnitario = d.PrecioUnitario,
                    articulo = d.Articulo != null ? new { descripcion = d.Articulo.Descripcion } : null,
                    promo = d.Promo != null ? new { nombre = d.Promo.Nombre } : null
                })
                .ToListAsync();

            return Ok(new
            {
                id = pedido.CodPedido,
                fecha = pedido.Fecha.ToString("dd/MM/yyyy HH:mm"),
                cliente = pedido.Cliente?.Nombre ?? "Consumidor Final",
                total = pedido.Total,
                detalles = detalles
            });
        }

        [AllowAnonymous]
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
                Fecha = ObtenerHoraArgentina(), // <-- OBLIGAMOS A USAR LA HORA ARGENTINA
                Estado = "Pendiente",
                Total = dto.Total,
                CodCliente = nuevoCliente.CodCliente,
                CodFormaPago = 1,
                CodTipoEnvio = 1
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            if (dto.Detalles != null && dto.Detalles.Any())
            {
                foreach (var item in dto.Detalles)
                {
                    var detalle = new DetallePedido
                    {
                        CodPedido = nuevoPedido.CodPedido,
                        CodArticulo = item.CodArticulo,
                        CodPromo = item.CodPromo,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio
                    };
                    _context.Set<DetallePedido>().Add(detalle);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { id = nuevoPedido.CodPedido });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarPedido(int id, [FromBody] PedidoNuevoDto dto)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.CodPedido == id);

            if (pedido == null) return NotFound();

            pedido.Total = dto.Total;

            if (pedido.Cliente != null)
            {
                pedido.Cliente.Nombre = dto.Cliente;
            }

            if (dto.Detalles != null)
            {
                var detallesViejos = await _context.Set<DetallePedido>().Where(d => d.CodPedido == id).ToListAsync();
                _context.Set<DetallePedido>().RemoveRange(detallesViejos);

                foreach (var item in dto.Detalles)
                {
                    var detalle = new DetallePedido
                    {
                        CodPedido = pedido.CodPedido,
                        CodArticulo = item.CodArticulo,
                        CodPromo = item.CodPromo,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.Precio
                    };
                    _context.Set<DetallePedido>().Add(detalle);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { id = pedido.CodPedido });
        }

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
        public string? Cliente { get; set; }
        public decimal Total { get; set; }
        public List<DetalleDto>? Detalles { get; set; }
    }

    public class DetalleDto
    {
        public int? CodArticulo { get; set; }
        public int? CodPromo { get; set; }
        public short Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}