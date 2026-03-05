using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.Crowdsourcing.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoRechazoToPropuesta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PropuestaCrowdsourcing_Necesidad_Id",
                table: "PropuestaCrowdsourcing");

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "PropuestaCrowdsourcing",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaCrowdsourcing_Necesidad_User",
                table: "PropuestaCrowdsourcing",
                columns: new[] { "Necesidad_Id", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PropuestaCrowdsourcing_Necesidad_User",
                table: "PropuestaCrowdsourcing");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "PropuestaCrowdsourcing");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaCrowdsourcing_Necesidad_Id",
                table: "PropuestaCrowdsourcing",
                column: "Necesidad_Id");
        }
    }
}
