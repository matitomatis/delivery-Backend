using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using delivery.DTOs;
using System.Linq;

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
                UrlImagen = a.UrlImagen,
                CategoriaId = a.CategoriaId
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
                return NotFound();
            }
            return Ok(articulo);
        }

        // POST: api/Articulos
        [HttpPost]
        public async Task<ActionResult> Post(ArticuloCreateDTO articuloDto)
        {
            var nuevoArticulo = new Articulo
            {
                Descripcion = articuloDto.Descripcion,
                Costo = articuloDto.Costo,
                Stock = articuloDto.Stock,
                UrlImagen = articuloDto.UrlImagen,
                CategoriaId = articuloDto.CategoriaId
            };

            await _repository.SaveAsync(nuevoArticulo);
            return Ok();
        }

        // --- MÉTODO PUT PARA EDITAR ---
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarArticulo(int id, ArticuloCreateDTO articuloDto)
        {
            // Buscamos el artículo original
            var articuloExistente = await _repository.GetByIdAsync(id);
            if (articuloExistente == null)
            {
                return NotFound();
            }

            // Le pisamos los datos viejos con los nuevos
            articuloExistente.Descripcion = articuloDto.Descripcion;
            articuloExistente.Costo = articuloDto.Costo;
            articuloExistente.Stock = articuloDto.Stock;
            articuloExistente.CategoriaId = articuloDto.CategoriaId;

            // Solo actualizamos la foto si subiste una nueva en el administrador
            if (!string.IsNullOrEmpty(articuloDto.UrlImagen))
            {
                articuloExistente.UrlImagen = articuloDto.UrlImagen;
            }

            // Nota: Asumo que en tu repositorio el "SaveAsync" sirve tanto para crear como para guardar cambios. 
            // Si tenés un método "UpdateAsync" creado en tu repositorio, cambialo acá abajo:
            await _repository.SaveAsync(articuloExistente);

            return Ok();
        }
        // ------------------------------

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