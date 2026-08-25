using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromosController : ControllerBase
    {
        private readonly IPromoRepository _repository;

        public PromosController(IPromoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Promo>>> GetPromos()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promo>> GetPromo(int id)
        {
            var promo = await _repository.GetByIdAsync(id);
            if (promo == null) return NotFound();
            return Ok(promo);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarPromo(Promo promo)
        {
            await _repository.SaveAsync(promo);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> BorrarPromo(int id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}