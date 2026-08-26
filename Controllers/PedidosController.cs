using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using delivery.DTOs;


namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoRepository _repository;

        public PedidosController(IPedidoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PedidoGetDTO>>> Get()
        {
            // 1. Buscamos los datos crudos en la base de datos
            var pedidos = await _repository.GetAllAsync(); // O el nombre que tenga tu método en el repo

            // 2. Los traducimos a nuestro formato seguro (DTO)
            var pedidosDto = pedidos.Select(p => new PedidoGetDTO
            {
                CodPedido = p.CodPedido,
                Fecha = p.Fecha,
                Total = p.Total,
                CodCliente = p.CodCliente,
                CodFormaPago = p.CodFormaPago,
                CodTipoEnvio = p.CodTipoEnvio,

                Detalles = p.Detalles != null ? p.Detalles.Select(d => new DetallePedidoGetDTO
                {
                    // Usamos .Value porque CodArticulo era opcional (nullable)
                    CodArticulo = d.CodArticulo ?? 0,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList() : new List<DetallePedidoGetDTO>()
            }).ToList();

            // 3. Devolvemos la lista ya formateada
            return Ok(pedidosDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> Get(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PedidoCreateDTO pedidoDto)
        {
            // Acá ocurre la traducción: Pasamos los datos del formulario (DTO) a la Entidad real
            var nuevoPedido = new Pedido
            {
                CodCliente = pedidoDto.CodCliente,
                CodFormaPago = pedidoDto.CodFormaPago,
                CodTipoEnvio = pedidoDto.CodTipoEnvio,

                // La Fecha se genera automáticamente acá en el servidor, imposible de falsificar.
                // El Total se va a calcular en tu Repository, así que ni lo tocamos.

                Detalles = pedidoDto.Detalles.Select(d => new DetallePedido
                {
                    CodArticulo = d.CodArticulo,
                    CodPromo = d.CodPromo,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            // Mandamos el pedido ya traducido al repositorio para que haga la lógica y lo guarde
            await _repository.SaveAsync(nuevoPedido);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}