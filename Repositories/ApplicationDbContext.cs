using delivery.Models;
using Microsoft.EntityFrameworkCore;

namespace delivery.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // Definimos los DbSet para cada entidad (equivalente a las tablas)
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Promo> Promos { get; set; }
        public DbSet<DetallePromo> DetallePromos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<FormaPago> FormasPago { get; set; }
        public DbSet<TipoEnvio> TiposEnvio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave primaria compuesta para DetallePromo
            modelBuilder.Entity<DetallePromo>()
                .HasKey(dp => new { dp.CodPromo, dp.CodArticulo });

            // DetallePedido usa la clave primaria definida en la entidad (CodDetalle).
            // Eliminamos la configuración de clave compuesta que causaba conflicto
            // con la relación opcional a Promo (CodPromo puede ser null).
        }
    }
}
