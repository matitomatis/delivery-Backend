using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    /// <inheritdoc />
    public partial class MakeCodDetalleIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos");

            // SQL puro para recrear la columna como IDENTITY (ALTER COLUMN no permite cambiar a IDENTITY)
            migrationBuilder.Sql(@"
IF COL_LENGTH('detalle_pedidos','CodDetalle') IS NOT NULL
BEGIN
    -- Añadimos columna temporal con IDENTITY
    ALTER TABLE detalle_pedidos ADD CodDetalle_tmp INT IDENTITY(1,1) NOT NULL;

    -- Eliminamos la columna antigua y renombramos la temporal
    ALTER TABLE detalle_pedidos DROP COLUMN CodDetalle;
    EXEC sp_rename 'detalle_pedidos.CodDetalle_tmp', 'CodDetalle', 'COLUMN';
END
");

            migrationBuilder.AlterColumn<int>(
                name: "CodPromo",
                table: "detalle_pedidos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos",
                column: "CodDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_pedidos_CodPedido",
                table: "detalle_pedidos",
                column: "CodPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos",
                column: "CodPromo",
                principalTable: "promos",
                principalColumn: "CodPromo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos");

            migrationBuilder.DropIndex(
                name: "IX_detalle_pedidos_CodPedido",
                table: "detalle_pedidos");

            migrationBuilder.AlterColumn<int>(
                name: "CodPromo",
                table: "detalle_pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Revertir la columna IDENTITY: crear columna temporal, copiar valores, restaurar nombre
            migrationBuilder.Sql(@"
IF COL_LENGTH('detalle_pedidos','CodDetalle') IS NOT NULL
BEGIN
    ALTER TABLE detalle_pedidos ADD CodDetalle_old INT NULL;
    UPDATE detalle_pedidos SET CodDetalle_old = CodDetalle;
    ALTER TABLE detalle_pedidos DROP COLUMN CodDetalle;
    EXEC sp_rename 'detalle_pedidos.CodDetalle_old', 'CodDetalle', 'COLUMN';
    ALTER TABLE detalle_pedidos ALTER COLUMN CodDetalle INT NOT NULL;
END
");

            migrationBuilder.AddPrimaryKey(
                name: "PK_detalle_pedidos",
                table: "detalle_pedidos",
                columns: new[] { "CodPedido", "CodPromo" });

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_pedidos_promos_CodPromo",
                table: "detalle_pedidos",
                column: "CodPromo",
                principalTable: "promos",
                principalColumn: "CodPromo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
