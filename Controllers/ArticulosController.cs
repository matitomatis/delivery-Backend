using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly IArticuloRepository _repository;

        // Inyectamos el repositorio
        public ArticulosController(IArticuloRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Articulos
        [HttpGet]
        public async Task<ActionResult<List<Articulo>>> GetArticulos()
        {
            var articulos = await _repository.GetAllAsync();
            return Ok(articulos); // Devuelve un 200 OK con la lista
        }

        // GET: api/Articulos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Articulo>> GetArticulo(int id)
        {
            var articulo = await _repository.GetByIdAsync(id);
            if (articulo == null)
            {
                return NotFound(); // Devuelve un 404 si no existe
            }
            return Ok(articulo);
        }

        // POST: api/Articulos
        [HttpPost]
        public async Task<ActionResult> GuardarArticulo(Articulo articulo)
        {
            await _repository.SaveAsync(articulo);
            return Ok(); // Devuelve un 200 OK cuando termina de guardar
        }

        // DELETE: api/Articulos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> BorrarArticulo(int id)
        {
            var articuloExistente = await _repository.GetByIdAsync(id);
            if (articuloExistente == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}