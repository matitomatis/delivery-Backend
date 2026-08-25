using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class FixTablaDetallePedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalles_pedido_pedidos_NroPedido",
                table: "detalles_pedido");

            migrationBuilder.DropForeignKey(
                name: "FK_detalles_pedido_promos_CodPromo",
                table: "detalles_pedido");

            migrationBuilder.DropPrimaryKey(
                name: "PK_detalles_pedido",
                table: "detalles_pedido");

            migrationBuilder.RenameTable(
                name: "detalles_pedido",
                newName: "detalle_pedidos");

            migrationBuilder.RenameColumn(
                name: "PreUnitario",
                table: "detalle_pedidos",
                newName: "PrecioUnitario");

            migrationBuilder.RenameColumn(
                name: "NroPedido",
                table: "detalle_pedidos",
                newName: "CodDetalle");

            migrationBuilder.RenameIndex(
                name: "IX_detalles_pedido_CodPromo",
                table: "detalle_pedidos",
                newName: "IX_detalle_pedidos_CodPromo");

            migrationBuilder.AddColumn<int>(
                name: "CodPedido",
                table: "detalle_pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CodArticulo",
                table: "detalle_pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos",
                columns: new[] { "CodPedido", "CodPromo" });

            migrationBuilder.CreateIndex(
                name: "IX_detalle_pedidos_CodArticulo",
                table: "detalle_pedidos",
                column: "CodArticulo");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_pedidos_articulos_CodArticulo",
                table: "detalle_pedidos",
                column: "CodArticulo",
                principalTable: "articulos",
                principalColumn: "CodArticulo");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_pedidos_pedidos_CodPedido",
                table: "detalle_pedidos",
                column: "CodPedido",
                principalTable: "pedidos",
                principalColumn: "CodPedido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos",
                column: "CodPromo",
                principalTable: "promos",
                principalColumn: "CodPromo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalle_pedidos_articulos_CodArticulo",
                table: "detalle_pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_detalle_pedidos_pedidos_CodPedido",
                table: "detalle_pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos");

            migrationBuilder.DropIndex(
                name: "IX_detalle_pedidos_CodArticulo",
                table: "detalle_pedidos");

            migrationBuilder.DropColumn(
                name: "CodPedido",
                table: "detalle_pedidos");

            migrationBuilder.DropColumn(
                name: "CodArticulo",
                table: "detalle_pedidos");

            migrationBuilder.RenameTable(
                name: "detalle_pedidos",
                newName: "detalles_pedido");

            migrationBuilder.RenameColumn(
                name: "PrecioUnitario",
                table: "detalles_pedido",
                newName: "PreUnitario");

            migrationBuilder.RenameColumn(
                name: "CodDetalle",
                table: "detalles_pedido",
                newName: "NroPedido");

            migrationBuilder.RenameIndex(
                name: "IX_detalle_pedidos_CodPromo",
                table: "detalles_pedido",
                newName: "IX_detalles_pedido_CodPromo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_detalles_pedido",
                table: "detalles_pedido",
                columns: new[] { "NroPedido", "CodPromo" });

            migrationBuilder.AddForeignKey(
                name: "FK_detalles_pedido_pedidos_NroPedido",
                table: "detalles_pedido",
                column: "NroPedido",
                principalTable: "pedidos",
                principalColumn: "CodPedido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_detalles_pedido_promos_CodPromo",
                table: "detalles_pedido",
                column: "CodPromo",
                principalTable: "promos",
                principalColumn: "CodPromo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
