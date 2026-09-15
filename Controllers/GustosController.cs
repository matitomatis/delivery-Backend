using delivery.Data.Repositories;
using delivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GustosController : ControllerBase
    {
        private readonly IGustoRepository _repository;
        public GustosController(IGustoRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Post(Gusto gusto)
        {
            await _repository.SaveAsync(gusto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Gusto gusto)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return NotFound();

            existente.Nombre = gusto.Nombre;
            existente.HayStock = gusto.HayStock;

            await _repository.SaveAsync(existente);
            return Ok();
        }
    }
}
