using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WePlayRises.Crowdsourcing.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddAcuerdosEntregablesAndMilestones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Acuerdo_Id_Ref",
                table: "PropuestaCrowdsourcing",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioRechazo",
                table: "AcuerdoCrowdsourcing_Entregable",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "AcuerdoCrowdsourcing_Entregable",
                type: "datetime2(3)",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanceladoPor",
                table: "AcuerdoCrowdsourcing",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelacion",
                table: "AcuerdoCrowdsourcing",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaestraEstadoAcuerdo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraEstadoAcuerdo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaestraEstadoEntregable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestraEstadoEntregable", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MaestraEstadoAcuerdo",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Acuerdo en curso", "Activo" },
                    { 2, "Acuerdo finalizado exitosamente", "Completado" },
                    { 3, "Acuerdo cancelado", "Cancelado" }
                });

            migrationBuilder.InsertData(
                table: "MaestraEstadoEntregable",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Subido por el profesional, pendiente revision", "Entregado" },
                    { 2, "Aprobado por el artista", "Aprobado" },
                    { 3, "Rechazado, requiere nueva version", "Rechazado" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Entregable_MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable",
                column: "MaestraEstadoEntregableId");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing",
                column: "MaestraEstadoAcuerdoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcuerdoCrowdsourcing_MaestraEstadoAcuerdo_MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing",
                column: "MaestraEstadoAcuerdoId",
                principalTable: "MaestraEstadoAcuerdo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcuerdoCrowdsourcing_Entregable_MaestraEstadoEntregable_MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable",
                column: "MaestraEstadoEntregableId",
                principalTable: "MaestraEstadoEntregable",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcuerdoCrowdsourcing_MaestraEstadoAcuerdo_MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing");

            migrationBuilder.DropForeignKey(
                name: "FK_AcuerdoCrowdsourcing_Entregable_MaestraEstadoEntregable_MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropTable(
                name: "MaestraEstadoAcuerdo");

            migrationBuilder.DropTable(
                name: "MaestraEstadoEntregable");

            migrationBuilder.DropIndex(
                name: "IX_AcuerdoCrowdsourcing_Entregable_MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropIndex(
                name: "IX_AcuerdoCrowdsourcing_MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing");

            migrationBuilder.DropColumn(
                name: "Acuerdo_Id_Ref",
                table: "PropuestaCrowdsourcing");

            migrationBuilder.DropColumn(
                name: "ComentarioRechazo",
                table: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropColumn(
                name: "MaestraEstadoEntregableId",
                table: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropColumn(
                name: "CanceladoPor",
                table: "AcuerdoCrowdsourcing");

            migrationBuilder.DropColumn(
                name: "MaestraEstadoAcuerdoId",
                table: "AcuerdoCrowdsourcing");

            migrationBuilder.DropColumn(
                name: "MotivoCancelacion",
                table: "AcuerdoCrowdsourcing");
        }
    }
}
