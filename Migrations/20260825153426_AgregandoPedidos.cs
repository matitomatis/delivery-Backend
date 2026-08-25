using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class AgregandoPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DireccionEnvio",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "FormaEntrega",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "FormaPago",
                table: "pedidos");

            migrationBuilder.RenameColumn(
                name: "FechaPedido",
                table: "pedidos",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "Cod_cliente",
                table: "pedidos",
                newName: "CodTipoEnvio");

            migrationBuilder.RenameColumn(
                name: "Nro_pedido",
                table: "pedidos",
                newName: "CodPedido");

            migrationBuilder.AddColumn<int>(
                name: "CodCliente",
                table: "pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CodFormaPago",
                table: "pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_CodCliente",
                table: "pedidos",
                column: "CodCliente");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_CodFormaPago",
                table: "pedidos",
                column: "CodFormaPago");

            migrationBuilder.CreateIndex(
                name: "IX_pedidos_CodTipoEnvio",
                table: "pedidos",
                column: "CodTipoEnvio");

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_clientes_CodCliente",
                table: "pedidos",
                column: "CodCliente",
                principalTable: "clientes",
                principalColumn: "CodCliente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_formas_pago_CodFormaPago",
                table: "pedidos",
                column: "CodFormaPago",
                principalTable: "formas_pago",
                principalColumn: "CodFormaPago",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_pedidos_tipos_envio_CodTipoEnvio",
                table: "pedidos",
                column: "CodTipoEnvio",
                principalTable: "tipos_envio",
                principalColumn: "CodTipoEnvio",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_clientes_CodCliente",
                table: "pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_formas_pago_CodFormaPago",
                table: "pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_pedidos_tipos_envio_CodTipoEnvio",
                table: "pedidos");

            migrationBuilder.DropIndex(
                name: "IX_pedidos_CodCliente",
                table: "pedidos");

            migrationBuilder.DropIndex(
                name: "IX_pedidos_CodFormaPago",
                table: "pedidos");

            migrationBuilder.DropIndex(
                name: "IX_pedidos_CodTipoEnvio",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "CodCliente",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "CodFormaPago",
                table: "pedidos");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "pedidos",
                newName: "FechaPedido");

            migrationBuilder.RenameColumn(
                name: "CodTipoEnvio",
                table: "pedidos",
                newName: "Cod_cliente");

            migrationBuilder.RenameColumn(
                name: "CodPedido",
                table: "pedidos",
                newName: "Nro_pedido");

            migrationBuilder.AddColumn<string>(
                name: "DireccionEnvio",
                table: "pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "pedidos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormaEntrega",
                table: "pedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormaPago",
                table: "pedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
