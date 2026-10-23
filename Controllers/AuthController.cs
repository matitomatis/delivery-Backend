using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LaDolce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validamos las credenciales del administrador
            if (request.Usuario == "deliveryladolce" && request.Password == "Sarmiento323")
            {
                var key = Encoding.UTF8.GetBytes("LaDolceSuperSecretaClaveParaElToken2026!");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Usuario) }),
                    Expires = DateTime.UtcNow.AddHours(8), // El token dura 8 horas
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHandler.WriteToken(token) });
            }

            return Unauthorized("Usuario o contraseña incorrectos");
        }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}