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
            var articulos = await _repository.GetAllAsync();

            var articulosDto = articulos.Select(a => new ArticuloGetDTO
            {
                CodArticulo = a.CodArticulo,
                Descripcion = a.Descripcion,
                Costo = a.Costo,
                Stock = a.Stock,
                UrlImagen = a.UrlImagen,
                CategoriaId = a.CategoriaId,

                // --- LO NUEVO ---
                MaxGustos = a.MaxGustos,
                // Si ImagenesExtras es null, devolvemos una lista vacía, sino extraemos las URLs
                ImagenesExtras = a.ImagenesExtras != null
                    ? a.ImagenesExtras.Select(img => img.Url).ToList()
                    : new List<string>()
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
                CategoriaId = articuloDto.CategoriaId,

                // --- LO NUEVO ---
                MaxGustos = articuloDto.MaxGustos,
                // Convertimos la lista de strings (URLs) en la entidad ImagenArticulo
                ImagenesExtras = articuloDto.ImagenesExtras != null
                    ? articuloDto.ImagenesExtras.Select(url => new ImagenArticulo { Url = url }).ToList()
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

            // --- 1. LO NUEVO: Actualizamos el límite de gustos ---
            articuloExistente.MaxGustos = articuloDto.MaxGustos;

            // Solo actualizamos la foto de portada si subiste una nueva en el administrador
            if (!string.IsNullOrEmpty(articuloDto.UrlImagen))
            {
                articuloExistente.UrlImagen = articuloDto.UrlImagen;
            }

            // --- 2. LO NUEVO: Actualizamos la galería de fotos extra ---
            if (articuloDto.ImagenesExtras != null)
            {
                // Limpiamos las fotos viejas de la memoria (EF Core las borrará de SQL Server al guardar)
                articuloExistente.ImagenesExtras.Clear();

                // Insertamos las nuevas URLs que llegaron desde el frontend
                foreach (var url in articuloDto.ImagenesExtras)
                {
                    articuloExistente.ImagenesExtras.Add(new ImagenArticulo { Url = url });
                }
            }

            // Guardamos los cambios en la base de datos
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