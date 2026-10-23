using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace delivery.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TiposEnvioController : ControllerBase
    {
        private readonly ITipoEnvioRepository _repository;

        public TiposEnvioController(ITipoEnvioRepository repository)
        {
            _repository = repository;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<TipoEnvio>>> Get() => Ok(await _repository.GetAllAsync());
        
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoEnvio>> Get(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Post(TipoEnvio tipoEnvio)
        {
            await _repository.SaveAsync(tipoEnvio);
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