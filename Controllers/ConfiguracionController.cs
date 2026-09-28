using delivery.Data.Models;
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using delivery.Data;
using Microsoft.AspNetCore.Authorization;

namespace delivery.Controllers
{
    [Authorize] // Protege todo el controlador por defecto
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous] // <--- LA LLAVE MÁGICA PARA QUE TUS CLIENTES PUEDAN LEER LOS DATOS
        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _context.ConfiguracionesLocales.FirstOrDefaultAsync();
            if (config == null) return Ok(new ConfiguracionLocal());
            return Ok(config);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateConfig([FromBody] ConfiguracionLocal nuevaConfig)
        {
            var config = await _context.ConfiguracionesLocales.FirstOrDefaultAsync();

            if (config == null)
            {
                _context.ConfiguracionesLocales.Add(nuevaConfig);
            }
            else
            {
                config.WhatsApp = nuevaConfig.WhatsApp;
                config.Instagram = nuevaConfig.Instagram;
                config.Facebook = nuevaConfig.Facebook;
                config.GoogleMaps = nuevaConfig.GoogleMaps;
                config.Alias = nuevaConfig.Alias;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Configuración guardada" });
        }
    }
}