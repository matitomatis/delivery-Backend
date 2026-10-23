using delivery.Repositories;
using delivery.Models; // Ajustalo a tu namespace
using delivery.Repositories; // Ajustalo a tu namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace delivery.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _context.ConfiguracionesLocales.FirstOrDefaultAsync();
            if (config == null) return Ok(new ConfiguracionesLocal());
            return Ok(config);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateConfig([FromBody] ConfiguracionesLocal nuevaConfig)
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
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Configuración guardada" });
        }
    }
}