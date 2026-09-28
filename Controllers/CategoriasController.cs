using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace delivery.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Inyectamos el entorno (env) para saber dónde está la carpeta wwwroot
        public CategoriasController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria([FromForm] CategoriaFormDto dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                CodRubro = dto.CodRubro
            };

            // MAGIA: Si viene un archivo, lo guardamos físicamente
            if (dto.ArchivoImagen != null && dto.ArchivoImagen.Length > 0)
            {
                categoria.UrlImagen = await GuardarImagenFisica(dto.ArchivoImagen);
            }

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.CodCategoria }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCategoria(int id, [FromForm] CategoriaFormDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = dto.Nombre;
            categoria.CodRubro = dto.CodRubro;

            // Si subieron una nueva foto al editar, la reemplazamos
            if (dto.ArchivoImagen != null && dto.ArchivoImagen.Length > 0)
            {
                categoria.UrlImagen = await GuardarImagenFisica(dto.ArchivoImagen);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- MÉTODO PRIVADO PARA MANEJAR EL ARCHIVO ---
        private async Task<string> GuardarImagenFisica(IFormFile archivo)
        {
            // Apuntamos a la carpeta wwwroot/imagenes
            string uploadsFolder = Path.Combine(_env.WebRootPath, "imagenes");

            // Si la carpeta no existe, la creamos
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Le ponemos un código único (Guid) al nombre para que no se pisen si subís dos fotos con el mismo nombre
            string nombreUnico = Guid.NewGuid().ToString() + "_" + archivo.FileName;
            string rutaCompleta = Path.Combine(uploadsFolder, nombreUnico);

            // Copiamos el archivo al servidor
            using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            // Devolvemos la ruta relativa que se va a guardar en MySQL
            return "/imagenes/" + nombreUnico;
        }
    }

    // --- DTO: EL MOLDE PARA RECIBIR LOS DATOS DEL FRONTEND ---
    public class CategoriaFormDto
    {
        public int CodCategoria { get; set; }
        public string Nombre { get; set; }
        public int CodRubro { get; set; }
        public IFormFile? ArchivoImagen { get; set; } // Acá llega la foto
    }
}