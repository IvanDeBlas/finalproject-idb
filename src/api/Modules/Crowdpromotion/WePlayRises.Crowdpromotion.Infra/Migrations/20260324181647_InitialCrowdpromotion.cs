using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.Crowdpromotion.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCrowdpromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromoPrograma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoArtistico_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaniaCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoPromo_Id = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    UrlLanding = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CodigoTrackingBase = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImporteComisionPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ImporteComisionFija = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PresupuestoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ComisionPorConversion = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ComisionPorClick = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoPrograma", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoPromotor_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FanProfile_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NombrePublico = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailContacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UrlSitioWeb = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UrlInstagram = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UrlTikTok = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UrlTwitter = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UrlYouTube = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SeguidoresTotales = table.Column<int>(type: "int", nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromoTarea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Programa_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstruccionesUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoReward_Id = table.Column<int>(type: "int", nullable: true),
                    ImporteRecompensa = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    PuntosRecompensa = table.Column<int>(type: "int", nullable: true),
                    TipoEventoPromo_Id = table.Column<int>(type: "int", nullable: false),
                    EsRepetible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MaxRepeticiones = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoTarea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoTarea_PromoPrograma_Programa_Id",
                        column: x => x.Programa_Id,
                        principalTable: "PromoPrograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Programa_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Promotor_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampaniaCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PedidoCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AportacionCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoEvento_Id = table.Column<int>(type: "int", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    ImporteAsociado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CodigoReferido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IpOrigen = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgentOrigen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PromoPrograma_Promotor_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserIdAfectado = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UrlOrigen = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UrlReferer = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UtmSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UtmMedium = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UtmCampaign = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoEvento_PromoPrograma_Programa_Id",
                        column: x => x.Programa_Id,
                        principalTable: "PromoPrograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PromoEvento_Promotor_Promotor_Id",
                        column: x => x.Promotor_Id,
                        principalTable: "Promotor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PromoProgramaPromotor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Programa_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Promotor_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoReferido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UrlReferido = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalClicks = table.Column<int>(type: "int", nullable: false),
                    TotalConversiones = table.Column<int>(type: "int", nullable: false),
                    TotalComisionesGeneradas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    EsAprobado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EsBloqueado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoProgramaPromotor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoProgramaPromotor_PromoPrograma_Programa_Id",
                        column: x => x.Programa_Id,
                        principalTable: "PromoPrograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoProgramaPromotor_Promotor_Promotor_Id",
                        column: x => x.Promotor_Id,
                        principalTable: "Promotor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotorWallet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Promotor_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    SaldoDisponible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalGanado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRetirado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotorWallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotorWallet_Promotor_Promotor_Id",
                        column: x => x.Promotor_Id,
                        principalTable: "Promotor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoTareaPromotor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tarea_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramaPromotor_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadoTarea_Id = table.Column<int>(type: "int", nullable: false),
                    UrlPruebaCompletado = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ComentarioPromotor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentarioValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaValidado = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoTareaPromotor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoTareaPromotor_PromoProgramaPromotor_ProgramaPromotor_Id",
                        column: x => x.ProgramaPromotor_Id,
                        principalTable: "PromoProgramaPromotor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoTareaPromotor_PromoTarea_Tarea_Id",
                        column: x => x.Tarea_Id,
                        principalTable: "PromoTarea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PromotorWalletTransaccion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Wallet_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoReward_Id = table.Column<int>(type: "int", nullable: true),
                    EstadoTransaccion_Id = table.Column<int>(type: "int", nullable: false),
                    CampaniaPayout_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReferenciaExterna = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    EsCredito = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PromoEvento_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotorWalletTransaccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotorWalletTransaccion_PromoEvento_PromoEvento_Id",
                        column: x => x.PromoEvento_Id,
                        principalTable: "PromoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PromotorWalletTransaccion_PromotorWallet_Wallet_Id",
                        column: x => x.Wallet_Id,
                        principalTable: "PromotorWallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromoEvento_CodigoReferido",
                table: "PromoEvento",
                column: "CodigoReferido");

            migrationBuilder.CreateIndex(
                name: "IX_PromoEvento_FechaCreacion",
                table: "PromoEvento",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_PromoEvento_Programa_Fecha",
                table: "PromoEvento",
                columns: new[] { "Programa_Id", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoEvento_ProgramaPromotor",
                table: "PromoEvento",
                column: "PromoPrograma_Promotor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromoEvento_Promotor_Tipo_Fecha",
                table: "PromoEvento",
                columns: new[] { "Promotor_Id", "TipoEvento_Id", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoPrograma_Artista_Activo",
                table: "PromoPrograma",
                columns: new[] { "Artista_Id", "EsActivo" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoPrograma_Artista_CodigoTracking",
                table: "PromoPrograma",
                columns: new[] { "Artista_Id", "CodigoTrackingBase" },
                unique: true,
                filter: "[CodigoTrackingBase] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PromoProgramaPromotor_CodigoReferido",
                table: "PromoProgramaPromotor",
                column: "CodigoReferido");

            migrationBuilder.CreateIndex(
                name: "IX_PromoProgramaPromotor_Programa_Promotor",
                table: "PromoProgramaPromotor",
                columns: new[] { "Programa_Id", "Promotor_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoProgramaPromotor_Promotor_Id",
                table: "PromoProgramaPromotor",
                column: "Promotor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromoTarea_Programa_Id",
                table: "PromoTarea",
                column: "Programa_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromoTareaPromotor_ProgramaPromotor_Id",
                table: "PromoTareaPromotor",
                column: "ProgramaPromotor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromoTareaPromotor_Tarea_Id",
                table: "PromoTareaPromotor",
                column: "Tarea_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Promotor_UserId",
                table: "Promotor",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotorWallet_Promotor_Moneda",
                table: "PromotorWallet",
                columns: new[] { "Promotor_Id", "Moneda_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromotorWalletTransaccion_FechaCreacion",
                table: "PromotorWalletTransaccion",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_PromotorWalletTransaccion_PromoEvento_Id",
                table: "PromotorWalletTransaccion",
                column: "PromoEvento_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PromotorWalletTransaccion_Wallet_EsCredito_Estado",
                table: "PromotorWalletTransaccion",
                columns: new[] { "Wallet_Id", "EsCredito", "EstadoTransaccion_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PromotorWalletTransaccion_Wallet_Fecha",
                table: "PromotorWalletTransaccion",
                columns: new[] { "Wallet_Id", "FechaCreacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoTareaPromotor");

            migrationBuilder.DropTable(
                name: "PromotorWalletTransaccion");

            migrationBuilder.DropTable(
                name: "PromoProgramaPromotor");

            migrationBuilder.DropTable(
                name: "PromoTarea");

            migrationBuilder.DropTable(
                name: "PromoEvento");

            migrationBuilder.DropTable(
                name: "PromotorWallet");

            migrationBuilder.DropTable(
                name: "PromoPrograma");

            migrationBuilder.DropTable(
                name: "Promotor");
        }
    }
}
