using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace delivery.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<ConfiguracionesLocal> ConfiguracionesLocales { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<DetallePromo> DetallePromos { get; set; }

    public virtual DbSet<FormaPago> FormasPagos { get; set; }

    public virtual DbSet<Gusto> Gustos { get; set; }

    public virtual DbSet<ImagenesArticulo> ImagenesArticulos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Promo> Promos { get; set; }

    public virtual DbSet<Rubro> Rubros { get; set; }

    public virtual DbSet<TarifaEnvio> TarifasEnvios { get; set; }

    public virtual DbSet<TipoEnvio> TiposEnvios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=delivery;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.CodArticulo);

            entity.ToTable("articulos");

            entity.HasIndex(e => e.CategoriaId, "IX_articulos_CategoriaId");

            entity.Property(e => e.Costo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UrlImagen).HasDefaultValue("");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Articulos).HasForeignKey(d => d.CategoriaId);
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.CodBanner);

            entity.ToTable("banners");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CodCategoria);

            entity.ToTable("categorias");

            entity.HasIndex(e => e.BannerId, "IX_categorias_BannerId");

            entity.HasIndex(e => e.CodRubro, "IX_categorias_CodRubro");

            entity.HasOne(d => d.Banner).WithMany(p => p.Categoria).HasForeignKey(d => d.BannerId);

            entity.HasOne(d => d.CodRubroNavigation).WithMany(p => p.Categoria).HasForeignKey(d => d.CodRubro);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.ToTable("clientes");

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Cuit).HasColumnName("cuit");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.NombreLocal).HasColumnName("nombre_local");
        });

        modelBuilder.Entity<ConfiguracionesLocal>(entity =>
        {
            entity.Property(e => e.Alias).HasDefaultValue("");
            entity.Property(e => e.TarifaEnvio).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.CodDetalle);

            entity.ToTable("detalle_pedidos");

            entity.HasIndex(e => e.CodArticulo, "IX_detalle_pedidos_CodArticulo");

            entity.HasIndex(e => e.CodPedido, "IX_detalle_pedidos_CodPedido");

            entity.HasIndex(e => e.CodPromo, "IX_detalle_pedidos_CodPromo");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CodArticuloNavigation).WithMany(p => p.DetallePedidos).HasForeignKey(d => d.CodArticulo);

            entity.HasOne(d => d.CodPedidoNavigation).WithMany(p => p.DetallePedidos).HasForeignKey(d => d.CodPedido);

            entity.HasOne(d => d.CodPromoNavigation).WithMany(p => p.DetallePedidos).HasForeignKey(d => d.CodPromo);
        });

        modelBuilder.Entity<DetallePromo>(entity =>
        {
            entity.HasKey(e => new { e.CodPromo, e.CodArticulo });

            entity.ToTable("detalle_promos");

            entity.HasIndex(e => e.CodArticulo, "IX_detalle_promos_CodArticulo");

            entity.HasOne(d => d.CodArticuloNavigation).WithMany(p => p.DetallePromos).HasForeignKey(d => d.CodArticulo);

            entity.HasOne(d => d.CodPromoNavigation).WithMany(p => p.DetallePromos).HasForeignKey(d => d.CodPromo);
        });

        modelBuilder.Entity<FormaPago>(entity =>
        {
            entity.HasKey(e => e.CodFormaPago);

            entity.ToTable("formas_pago");

            entity.Property(e => e.Descripcion).HasMaxLength(50);
        });

        modelBuilder.Entity<Gusto>(entity =>
        {
            entity.ToTable("gustos");
        });

        modelBuilder.Entity<ImagenesArticulo>(entity =>
        {
            entity.HasKey(e => e.CodImagen);

            entity.ToTable("imagenes_articulos");

            entity.HasIndex(e => e.CodArticulo, "IX_imagenes_articulos_CodArticulo");

            entity.HasOne(d => d.CodArticuloNavigation).WithMany(p => p.ImagenesArticulos).HasForeignKey(d => d.CodArticulo);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.CodPedido);

            entity.ToTable("pedidos");

            entity.HasIndex(e => e.CodCliente, "IX_pedidos_CodCliente");

            entity.HasIndex(e => e.CodFormaPago, "IX_pedidos_CodFormaPago");

            entity.HasIndex(e => e.CodTipoEnvio, "IX_pedidos_CodTipoEnvio");

            entity.Property(e => e.Estado).HasDefaultValue("");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.CodClienteNavigation).WithMany(p => p.Pedidos).HasForeignKey(d => d.CodCliente);

            entity.HasOne(d => d.CodFormaPagoNavigation).WithMany(p => p.Pedidos).HasForeignKey(d => d.CodFormaPago);

            entity.HasOne(d => d.CodTipoEnvioNavigation).WithMany(p => p.Pedidos).HasForeignKey(d => d.CodTipoEnvio);
        });

        modelBuilder.Entity<Promo>(entity =>
        {
            entity.HasKey(e => e.CodPromo);

            entity.ToTable("promos");

            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Rubro>(entity =>
        {
            entity.HasKey(e => e.CodRubro);
        });

        modelBuilder.Entity<TarifaEnvio>(entity =>
        {
            entity.ToTable("tarifa_envio");

            entity.Property(e => e.Costo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<TipoEnvio>(entity =>
        {
            entity.HasKey(e => e.CodTipoEnvio);

            entity.ToTable("tipos_envio");

            entity.Property(e => e.Descripcion).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
