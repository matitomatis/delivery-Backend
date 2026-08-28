using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using delivery.DTOs;
using System.Linq; // Necesario para el .Select()

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
        public async Task<ActionResult<List<ArticuloGetDTO>>> Get()
        {
            // Traemos los datos crudos
            var articulos = await _repository.GetAllAsync();

            // Los traducimos al DTO seguro (incluyendo la imagen)
            var articulosDto = articulos.Select(a => new ArticuloGetDTO
            {
                CodArticulo = a.CodArticulo,
                Descripcion = a.Descripcion,
                Costo = a.Costo,
                Stock = a.Stock,
                UrlImagen = a.UrlImagen // <--- Agregado acá
            }).ToList();

            return Ok(articulosDto);
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
        public async Task<ActionResult> Post(ArticuloCreateDTO articuloDto)
        {
            // Armamos la Entidad real usando los datos del formulario DTO (incluyendo la foto)
            var nuevoArticulo = new Articulo
            {
                Descripcion = articuloDto.Descripcion,
                Costo = articuloDto.Costo,
                Stock = articuloDto.Stock,
                UrlImagen = articuloDto.UrlImagen // <--- Agregado acá
            };

            // La mandamos a guardar
            await _repository.SaveAsync(nuevoArticulo);
            return Ok();
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