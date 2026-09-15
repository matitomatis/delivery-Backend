using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delivery.Migrations // (Tu namespace real)
{
    public partial class AgregaGustosYDetalles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==============================================================
            // 1. DEJAR INTACTO: Los ajustes de decimales y primary keys
            // ==============================================================
            // Vas a ver muchos bloques como este. ¡NO LOS BORRES! 
            // Son los fixes de los decimal(18,2) y la clave de DetallePromo.
            //migrationBuilder.AlterColumn<decimal>(
            //    name: "TarifaEnvio",
            //    table: "ConfiguracionLocal",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,0)");
            // (Dejá el resto de los AlterColumn que haya generado tu Visual Studio abajo de este)


            // ==============================================================
            // 2. COMENTAR / APAGAR: La creación de tablas que YA EXISTEN
            // ==============================================================
            // Envolvé todos los CreateTable de tus tablas viejas entre /* y */
            /*
            migrationBuilder.CreateTable(
                name: "banners",
                columns: table => new { ... }
            );

            migrationBuilder.CreateTable(
                name: "articulos",
                columns: table => new { ... }
            );
            
            // ... MÁS TABLAS VIEJAS COMENTADAS ...
            */


            // ==============================================================
            // 3. DEJAR INTACTO: La creación de la tabla NUEVA
            // ==============================================================
            // Este bloque tiene que quedar suelto, sin comentar, para que se ejecute.
            migrationBuilder.CreateTable(
         name: "gustos",
         columns: table => new
         {
             Id = table.Column<int>(type: "int", nullable: false)
                 .Annotation("SqlServer:Identity", "1, 1"),
             Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
             HayStock = table.Column<bool>(type: "bit", nullable: false)
         },
         constraints: table =>
         {
             table.PrimaryKey("PK_gustos", x => x.Id);
         });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ==============================================================
            // 4. EL MÉTODO DOWN: Solo borramos la tabla nueva
            // ==============================================================
            migrationBuilder.DropTable(
                name: "gustos");

            // Comentá el borrado de las tablas viejas por seguridad
            /*
            migrationBuilder.DropTable(name: "banners");
            migrationBuilder.DropTable(name: "articulos");
            */
        }
    }
}