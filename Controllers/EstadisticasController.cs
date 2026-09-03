using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using delivery.Repositories; // Asegurate de que apunte a donde está tu ApplicationDbContext
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace delivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Ventas")]
        public async Task<IActionResult> GetVentasMensuales([FromQuery] int anio)
        {
            // 1. Buscamos pedidos de ese año que estén "Completados"
            var ventasDb = await _context.Pedidos
                .Where(p => p.Estado == "Completado" && p.Fecha.Year == anio)
                .GroupBy(p => p.Fecha.Month)
                .Select(g => new
                {
                    mes = g.Key,
                    total = g.Sum(p => p.Total)
                })
                .ToListAsync();

            // 2. Armamos la lista de los 12 meses asegurando que los que no tienen ventas devuelvan 0
            var resultado = new List<object>();
            for (int i = 1; i <= 12; i++)
            {
                var mesData = ventasDb.FirstOrDefault(v => v.mes == i);
                resultado.Add(new
                {
                    mes = i,
                    total = mesData != null ? mesData.total : 0
                });
            }

            return Ok(resultado);
        }
    }
}