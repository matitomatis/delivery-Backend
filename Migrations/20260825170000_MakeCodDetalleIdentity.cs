using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations
{
    public partial class MakeCodDetalleIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('detalle_pedidos','CodDetalle') IS NOT NULL
BEGIN
    -- Añadimos una columna temporal con IDENTITY
    ALTER TABLE detalle_pedidos ADD CodDetalle_tmp INT IDENTITY(1,1) NOT NULL;

    -- Eliminamos la columna antigua y renombramos la temporal
    ALTER TABLE detalle_pedidos DROP COLUMN CodDetalle;
    EXEC sp_rename 'detalle_pedidos.CodDetalle_tmp', 'CodDetalle', 'COLUMN';
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('detalle_pedidos','CodDetalle') IS NOT NULL
BEGIN
    -- Restauramos una columna sin IDENTITY conservando valores actuales
    ALTER TABLE detalle_pedidos ADD CodDetalle_old INT NULL;
    UPDATE detalle_pedidos SET CodDetalle_old = CodDetalle;
    ALTER TABLE detalle_pedidos DROP COLUMN CodDetalle;
    EXEC sp_rename 'detalle_pedidos.CodDetalle_old', 'CodDetalle', 'COLUMN';
    ALTER TABLE detalle_pedidos ALTER COLUMN CodDetalle INT NOT NULL;
END
");
        }
    }
}
