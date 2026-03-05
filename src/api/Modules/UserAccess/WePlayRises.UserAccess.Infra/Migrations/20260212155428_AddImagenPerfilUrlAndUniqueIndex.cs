using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.UserAccess.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenPerfilUrlAndUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenPerfilUrl",
                table: "Artista",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artista_UserIdPropietario",
                table: "Artista",
                column: "UserIdPropietario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Artista_UserIdPropietario",
                table: "Artista");

            migrationBuilder.DropColumn(
                name: "ImagenPerfilUrl",
                table: "Artista");
        }
    }
}
