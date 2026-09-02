using delivery.Data; // Revisá que coincida con tu namespace
using delivery.Models;
using delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifaEnvioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TarifaEnvioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<decimal>> GetTarifa()
        {
            // Buscamos el primer registro sin forzar el Id
            var tarifa = await _context.TarifasEnvio.FirstOrDefaultAsync();

            if (tarifa == null)
            {
                return 0;
            }

            return tarifa.Costo;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTarifa([FromBody] decimal nuevoCosto)
        {
            // Buscamos el primer registro
            var tarifa = await _context.TarifasEnvio.FirstOrDefaultAsync();

            if (tarifa == null)
            {
                // Lo creamos SIN asignarle el Id a mano. SQL lo hace solo.
                tarifa = new TarifaEnvio { Costo = nuevoCosto };
                _context.TarifasEnvio.Add(tarifa);
            }
            else
            {
                // Si ya existe, pisamos el costo
                tarifa.Costo = nuevoCosto;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tarifa actualizada con éxito", costo = tarifa.Costo });
        }
    }
}