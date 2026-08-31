using delivery.Models;
using delivery.DTOs;
using Microsoft.AspNetCore.Mvc;
using delivery.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoRepository _repository; // Asegurate de tener inyectado el repositorio correspondiente

        public PedidosController(IPedidoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult> CrearPedido(PedidoCreateDTO pedidoDto)
        {
            // Mapeamos el DTO a la Entidad Real
            var nuevoPedido = new Pedido
            {
                CodCliente = pedidoDto.CodCliente,
                CodFormaPago = pedidoDto.CodFormaPago,
                CodTipoEnvio = pedidoDto.CodTipoEnvio,
                Fecha = DateTime.Now,

                // Calculamos el total de forma segura en el backend
                Total = pedidoDto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),

                // Iteramos la lista que llegó y armamos el detalle
                Detalles = pedidoDto.Detalles.Select(d => new DetallePedido
                {
                    CodArticulo = d.CodArticulo,
                    CodPromo = d.CodPromo,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            await _repository.SaveAsync(nuevoPedido);
            return Ok();
        }
    }
}