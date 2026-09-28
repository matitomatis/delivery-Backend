using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using delivery.DTOs;
using Microsoft.AspNetCore.Authorization;
using delivery.Helpers; // <-- AGREGADO: Para usar el compresor

namespace delivery.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PromosController : ControllerBase
    {
        private readonly IPromoRepository _repository;

        public PromosController(IPromoRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Promo>>> GetPromos()
        {
            return Ok(await _repository.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Promo>> GetPromo(int id)
        {
            var promo = await _repository.GetByIdAsync(id);
            if (promo == null) return NotFound();
            return Ok(promo);
        }

        [HttpPost]
        public async Task<ActionResult> GuardarPromo(PromoCreateDTO promoDto)
        {
            var nuevaPromo = new Promo
            {
                Nombre = promoDto.Nombre,
                Descripcion = promoDto.Descripcion,
                Categoria = promoDto.Categoria,

                // --- COMPRESIÓN APLICADA A LA FOTO DE LA PROMO ---
                UrlImagen = ImageOptimizer.OptimizeToBase64Webp(promoDto.UrlImagen),

                PrecioVenta = promoDto.PrecioVenta,
                Activa = true,

                // Mapeamos los artículos elegidos en el front hacia la tabla detalle_promos
                DetallePromos = promoDto.Articulos.Select(a => new DetallePromo
                {
                    CodArticulo = a.CodArticulo,
                    Cantidad = a.Cantidad
                }).ToList()
            };

            await _repository.SaveAsync(nuevaPromo);
            return Ok();
        }

        // --- MÉTODO PUT PARA EDITAR ---
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarPromo(int id, PromoCreateDTO promoDto)
        {
            var promoExistente = await _repository.GetByIdAsync(id);
            if (promoExistente == null) return NotFound();

            // 1. Pisamos los datos básicos
            promoExistente.Nombre = promoDto.Nombre;
            promoExistente.Descripcion = promoDto.Descripcion;
            promoExistente.Categoria = promoDto.Categoria;
            promoExistente.PrecioVenta = promoDto.PrecioVenta;

            // 2. Solo actualizamos y comprimimos la foto si subiste una nueva
            if (!string.IsNullOrEmpty(promoDto.UrlImagen))
            {
                // --- COMPRESIÓN APLICADA AL EDITAR ---
                promoExistente.UrlImagen = ImageOptimizer.OptimizeToBase64Webp(promoDto.UrlImagen);
            }

            // 3. Actualizamos los artículos que contiene el combo
            // Vaciamos la lista vieja y la volvemos a llenar con los datos del formulario
            if (promoExistente.DetallePromos != null)
            {
                promoExistente.DetallePromos.Clear();
                foreach (var art in promoDto.Articulos)
                {
                    promoExistente.DetallePromos.Add(new DetallePromo
                    {
                        CodArticulo = art.CodArticulo,
                        Cantidad = art.Cantidad
                    });
                }
            }

            await _repository.SaveAsync(promoExistente);
            return Ok();
        }
        // ------------------------------

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