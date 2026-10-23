using delivery.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using delivery.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DE BASE DE DATOS FIJADA EN SQL SERVER
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// REPOSITORIOS UNIFICADOS EN UN SOLO PROYECTO
builder.Services.AddScoped<IArticuloRepository, ArticuloRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPromoRepository, PromoRepository>();
builder.Services.AddScoped<IFormaPagoRepository, FormaPagoRepository>();
builder.Services.AddScoped<ITipoEnvioRepository, TipoEnvioRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IGustoRepository, GustoRepository>();

builder.Services.AddControllers();
//builder.Services.AddOpenApi();

// CONFIGURACIÓN DE AUTENTICACIÓN JWT (De nueva-funcionalidad)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LaDolceSuperSecretaClaveParaElToken2026!")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

// CONFIGURACIÓN CORS SEGURA
app.UseCors(options => options
    .WithOrigins(
        "https://somosladolce.com.ar",
        "http://localhost:5000",
        "https://localhost:7001"
    )
    .AllowAnyMethod()
    .AllowAnyHeader());

// ORDEN CORRECTO DE SEGURIDAD
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/adminladolce", "admin.html");

app.Run();
