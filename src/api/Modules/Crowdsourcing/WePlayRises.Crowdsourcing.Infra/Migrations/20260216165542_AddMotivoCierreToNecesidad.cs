using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.Crowdsourcing.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoCierreToNecesidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoCierre",
                table: "NecesidadCrowdsourcing",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoCierre",
                table: "NecesidadCrowdsourcing");
        }
    }
}
