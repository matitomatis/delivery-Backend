using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articulos",
                columns: table => new
                {
                    CodArticulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<short>(type: "smallint", nullable: false),
                    StockMinimo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_articulos", x => x.CodArticulo);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    CodCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreApellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<long>(type: "bigint", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.CodCliente);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    Nro_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cod_cliente = table.Column<int>(type: "int", nullable: false),
                    FechaPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormaEntrega = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.Nro_pedido);
                });

            migrationBuilder.CreateTable(
                name: "promos",
                columns: table => new
                {
                    CodPromo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlImagen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promos", x => x.CodPromo);
                });

            migrationBuilder.CreateTable(
                name: "detalle_promos",
                columns: table => new
                {
                    CodPromo = table.Column<int>(type: "int", nullable: false),
                    CodArticulo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_promos", x => new { x.CodPromo, x.CodArticulo });
                    table.ForeignKey(
                        name: "FK_detalle_promos_articulos_CodArticulo",
                        column: x => x.CodArticulo,
                        principalTable: "articulos",
                        principalColumn: "CodArticulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_promos_promos_CodPromo",
                        column: x => x.CodPromo,
                        principalTable: "promos",
                        principalColumn: "CodPromo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalles_pedido",
                columns: table => new
                {
                    NroPedido = table.Column<int>(type: "int", nullable: false),
                    CodPromo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<short>(type: "smallint", nullable: false),
                    PreUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalles_pedido", x => new { x.NroPedido, x.CodPromo });
                    table.ForeignKey(
                        name: "FK_detalles_pedido_pedidos_NroPedido",
                        column: x => x.NroPedido,
                        principalTable: "pedidos",
                        principalColumn: "Nro_pedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalles_pedido_promos_CodPromo",
                        column: x => x.CodPromo,
                        principalTable: "promos",
                        principalColumn: "CodPromo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_promos_CodArticulo",
                table: "detalle_promos",
                column: "CodArticulo");

            migrationBuilder.CreateIndex(
                name: "IX_detalles_pedido_CodPromo",
                table: "detalles_pedido",
                column: "CodPromo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "detalle_promos");

            migrationBuilder.DropTable(
                name: "detalles_pedido");

            migrationBuilder.DropTable(
                name: "articulos");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "promos");
        }
    }
}
