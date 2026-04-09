using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.Crowdsourcing.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddMensajeriaColumnRenames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Id",
                table: "MensajeCrowdsourcing");

            migrationBuilder.RenameColumn(
                name: "UserIdProveedor",
                table: "ConversacionCrowdsourcing",
                newName: "UserIdDestinatario");

            migrationBuilder.RenameColumn(
                name: "UserIdArtista",
                table: "ConversacionCrowdsourcing",
                newName: "UserIdCreador");

            migrationBuilder.AlterColumn<string>(
                name: "UrlAdjunto",
                table: "MensajeCrowdsourcing",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Leido",
                table: "MensajeCrowdsourcing",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Asunto",
                table: "ConversacionCrowdsourcing",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Fecha",
                table: "MensajeCrowdsourcing",
                columns: new[] { "Conversacion_Id", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Leido_Remitente",
                table: "MensajeCrowdsourcing",
                columns: new[] { "Conversacion_Id", "Leido", "UserIdRemitente" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionCrowdsourcing_Creador_Fecha",
                table: "ConversacionCrowdsourcing",
                columns: new[] { "UserIdCreador", "FechaUltimoMensaje" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionCrowdsourcing_Destinatario_Fecha",
                table: "ConversacionCrowdsourcing",
                columns: new[] { "UserIdDestinatario", "FechaUltimoMensaje" });

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionCrowdsourcing_FechaUltimoMensaje",
                table: "ConversacionCrowdsourcing",
                column: "FechaUltimoMensaje");

            migrationBuilder.CreateIndex(
                name: "UX_ConversacionCrowdsourcing_Usuarios_Acuerdo",
                table: "ConversacionCrowdsourcing",
                columns: new[] { "UserIdCreador", "UserIdDestinatario", "Acuerdo_Id" },
                unique: true,
                filter: "[Acuerdo_Id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_ConversacionCrowdsourcing_Usuarios_Necesidad",
                table: "ConversacionCrowdsourcing",
                columns: new[] { "UserIdCreador", "UserIdDestinatario", "Necesidad_Id" },
                unique: true,
                filter: "[Necesidad_Id] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Fecha",
                table: "MensajeCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Leido_Remitente",
                table: "MensajeCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "IX_ConversacionCrowdsourcing_Creador_Fecha",
                table: "ConversacionCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "IX_ConversacionCrowdsourcing_Destinatario_Fecha",
                table: "ConversacionCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "IX_ConversacionCrowdsourcing_FechaUltimoMensaje",
                table: "ConversacionCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "UX_ConversacionCrowdsourcing_Usuarios_Acuerdo",
                table: "ConversacionCrowdsourcing");

            migrationBuilder.DropIndex(
                name: "UX_ConversacionCrowdsourcing_Usuarios_Necesidad",
                table: "ConversacionCrowdsourcing");

            migrationBuilder.RenameColumn(
                name: "UserIdDestinatario",
                table: "ConversacionCrowdsourcing",
                newName: "UserIdProveedor");

            migrationBuilder.RenameColumn(
                name: "UserIdCreador",
                table: "ConversacionCrowdsourcing",
                newName: "UserIdArtista");

            migrationBuilder.AlterColumn<string>(
                name: "UrlAdjunto",
                table: "MensajeCrowdsourcing",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Leido",
                table: "MensajeCrowdsourcing",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Asunto",
                table: "ConversacionCrowdsourcing",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Id",
                table: "MensajeCrowdsourcing",
                column: "Conversacion_Id");
        }
    }
}
