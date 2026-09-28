using delivery.Data.Repositories;
using delivery.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace delivery.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GustosController : ControllerBase
    {
        private readonly IGustoRepository _repository;
        public GustosController(IGustoRepository repository) => _repository = repository;
        [AllowAnonymous]
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
        [HttpPut("{id}/Estado/{hayStock}")]
        public async Task<IActionResult> CambiarEstadoGusto(int id, bool hayStock)
        {
            var gusto = await _repository.GetByIdAsync(id);
            if (gusto == null) return NotFound("El sabor no existe.");

            gusto.HayStock = hayStock;
            await _repository.SaveAsync(gusto);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> BorrarGusto(int id)
        {
            var gustoExistente = await _repository.GetByIdAsync(id);
            if (gustoExistente == null)
            {
                return NotFound("El sabor no existe.");
            }

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}
