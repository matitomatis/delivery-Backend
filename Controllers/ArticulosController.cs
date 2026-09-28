using delivery.DTOs;
using delivery.Helpers; // <-- AGREGADO: Para poder usar ImageOptimizer
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace delivery.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<ArticuloGetDTO>>> Get()
        {
            var articulos = await _repository.GetAllAsync();

            var articulosDto = articulos.Select(a => new ArticuloGetDTO
            {
                CodArticulo = a.CodArticulo,
                Descripcion = a.Descripcion,
                Costo = a.Costo,
                Stock = a.Stock,
                UrlImagen = a.UrlImagen,
                CategoriaId = a.CategoriaId,
                Activo = a.Activo,
                MaxGustos = a.MaxGustos,
                ImagenesExtras = a.ImagenesExtras != null
                    ? a.ImagenesExtras.Select(img => img.Url).ToList()
                    : new List<string>()
            }).ToList();

            return Ok(articulosDto);
        }

        // GET: api/Articulos/5
        [AllowAnonymous]
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
                CategoriaId = articuloDto.CategoriaId,
                MaxGustos = articuloDto.MaxGustos,

                // --- MAGIA APLICADA: Comprimimos la foto principal ---
                UrlImagen = ImageOptimizer.OptimizeToBase64Webp(articuloDto.UrlImagen),

                // --- MAGIA APLICADA: Comprimimos la lista de fotos extra ---
                ImagenesExtras = articuloDto.ImagenesExtras != null
                    ? articuloDto.ImagenesExtras.Select(url => new ImagenArticulo
                    {
                        Url = ImageOptimizer.OptimizeToBase64Webp(url)
                    }).ToList()
                    : new List<ImagenArticulo>()
            };

            await _repository.SaveAsync(nuevoArticulo);
            return Ok();
        }

        // --- MÉTODO PUT PARA EDITAR ---
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarArticulo(int id, ArticuloCreateDTO articuloDto)
        {
            // Buscamos el artículo original (el repositorio ya trae las ImagenesExtras gracias al Include)
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
            articuloExistente.MaxGustos = articuloDto.MaxGustos;

            // --- MAGIA APLICADA: Comprimimos la foto principal si es que subió una nueva ---
            if (!string.IsNullOrEmpty(articuloDto.UrlImagen))
            {
                articuloExistente.UrlImagen = ImageOptimizer.OptimizeToBase64Webp(articuloDto.UrlImagen);
            }

            // --- MAGIA APLICADA: Comprimimos las fotos de la galería ---
            if (articuloDto.ImagenesExtras != null)
            {
                // Limpiamos las fotos viejas de la memoria (EF Core las borrará de SQL Server al guardar)
                articuloExistente.ImagenesExtras.Clear();

                // Comprimimos e insertamos las nuevas URLs que llegaron desde el frontend
                foreach (var url in articuloDto.ImagenesExtras)
                {
                    articuloExistente.ImagenesExtras.Add(new ImagenArticulo
                    {
                        Url = ImageOptimizer.OptimizeToBase64Webp(url)
                    });
                }
            }

            // Guardamos los cambios en la base de datos
            await _repository.SaveAsync(articuloExistente);

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

        [HttpPut("{id}/Estado/{activo}")]
        public async Task<IActionResult> CambiarEstadoArticulo(int id, bool activo)
        {
            var articulo = await _repository.GetByIdAsync(id);
            if (articulo == null) return NotFound("El artículo no existe.");

            articulo.Activo = activo;
            await _repository.SaveAsync(articulo);

            return NoContent();
        }
    }
}