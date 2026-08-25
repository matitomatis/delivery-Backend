using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _repository;

        public ClientesController(IClienteRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetClientes()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarCliente(Cliente cliente)
        {
            await _repository.SaveAsync(cliente);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> BorrarCliente(int id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}