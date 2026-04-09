using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WePlayRises.Crowdfunding.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artista_MembershipPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombrePlan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteMensual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artista_MembershipPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artista_PayoutCuenta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCuenta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProveedorPayout = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IdentificadorExterno = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NombreTitular = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    EsPorDefecto = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artista_PayoutCuenta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoArtistico_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subtitulo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DescripcionCorta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VideoPrincipalUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenPrincipalUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    ImporteObjetivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImportePledgedActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoFinanciacion_Id = table.Column<int>(type: "int", nullable: false),
                    EstadoCampania_Id = table.Column<int>(type: "int", nullable: false),
                    PermiteAportacionesAnonimas = table.Column<bool>(type: "bit", nullable: false),
                    PermitePropinas = table.Column<bool>(type: "bit", nullable: false),
                    PorcentajeComisionPlataforma = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    Borrado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artista_MembershipSuscripcion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipPlan_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    ProximoCargo = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    EsActiva = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artista_MembershipSuscripcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artista_MembershipSuscripcion_Artista_MembershipPlan_MembershipPlan_Id",
                        column: x => x.MembershipPlan_Id,
                        principalTable: "Artista_MembershipPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding_Comentario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ComentarioPadre_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsRespuestaArtista = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    Borrado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding_Comentario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Comentario_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Comentario_CampaniaCrowdfunding_Comentario_ComentarioPadre_Id",
                        column: x => x.ComentarioPadre_Id,
                        principalTable: "CampaniaCrowdfunding_Comentario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding_Payout",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistaPayoutCuenta_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    ImporteBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPlataforma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPasarela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuestosRetenidos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteNetoArtista = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstadoPayout_Id = table.Column<int>(type: "int", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    NotasInternas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding_Payout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Payout_Artista_PayoutCuenta_ArtistaPayoutCuenta_Id",
                        column: x => x.ArtistaPayoutCuenta_Id,
                        principalTable: "Artista_PayoutCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Payout_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding_Reward",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoReward_Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    EsAddOn = table.Column<bool>(type: "bit", nullable: false),
                    CantidadMaxima = table.Column<int>(type: "int", nullable: true),
                    CantidadPorBacker = table.Column<int>(type: "int", nullable: true),
                    IncluyeEnvioFisico = table.Column<bool>(type: "bit", nullable: false),
                    TiempoEntregaEstimado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding_Reward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Reward_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding_StretchGoal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteObjetivo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Alcanzado = table.Column<bool>(type: "bit", nullable: false),
                    FechaAlcanzado = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding_StretchGoal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_StretchGoal_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaCrowdfunding_Update",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsPublico = table.Column<bool>(type: "bit", nullable: false),
                    SoloBackers = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaCrowdfunding_Update", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaniaCrowdfunding_Update_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoCrowdfunding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Campania_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FanProfile_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EstadoPedido_Id = table.Column<int>(type: "int", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    ImporteSubtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportePropina = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteEnvio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuestos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PermitirMostrarNombre = table.Column<bool>(type: "bit", nullable: false),
                    ComentarioBacker = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DireccionEnvio_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoCrowdfunding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoCrowdfunding_CampaniaCrowdfunding_Campania_Id",
                        column: x => x.Campania_Id,
                        principalTable: "CampaniaCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Artista_MembershipPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipSuscripcion_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    MetodoPago_Id = table.Column<int>(type: "int", nullable: false),
                    EstadoAportacion_Id = table.Column<int>(type: "int", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuestos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPlataforma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPasarela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteNetoArtista = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CodigoOperacionPasarela = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoOperacionProveedor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCaptura = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCancelacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artista_MembershipPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artista_MembershipPago_Artista_MembershipSuscripcion_MembershipSuscripcion_Id",
                        column: x => x.MembershipSuscripcion_Id,
                        principalTable: "Artista_MembershipSuscripcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AportacionCrowdfunding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: false),
                    MetodoPago_Id = table.Column<int>(type: "int", nullable: false),
                    EstadoAportacion_Id = table.Column<int>(type: "int", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteImpuestos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPlataforma = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteComisionPasarela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteNetoArtista = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CodigoOperacionPasarela = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoOperacionProveedor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCaptura = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCancelacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AportacionCrowdfunding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AportacionCrowdfunding_PedidoCrowdfunding_PedidoCrowdfunding_Id",
                        column: x => x.PedidoCrowdfunding_Id,
                        principalTable: "PedidoCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoCrowdfunding_Linea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoCrowdfunding_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reward_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteLinea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EsRewardPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoCrowdfunding_Linea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoCrowdfunding_Linea_CampaniaCrowdfunding_Reward_Reward_Id",
                        column: x => x.Reward_Id,
                        principalTable: "CampaniaCrowdfunding_Reward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoCrowdfunding_Linea_PedidoCrowdfunding_PedidoCrowdfunding_Id",
                        column: x => x.PedidoCrowdfunding_Id,
                        principalTable: "PedidoCrowdfunding",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AportacionCrowdfunding_Pedido_Estado",
                table: "AportacionCrowdfunding",
                columns: new[] { "PedidoCrowdfunding_Id", "EstadoAportacion_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Artista_MembershipPago_Suscripcion",
                table: "Artista_MembershipPago",
                columns: new[] { "MembershipSuscripcion_Id", "EstadoAportacion_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Artista_MembershipPlan_Artista_NombrePlan",
                table: "Artista_MembershipPlan",
                columns: new[] { "Artista_Id", "NombrePlan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artista_MembershipSuscripcion_Artista_User",
                table: "Artista_MembershipSuscripcion",
                columns: new[] { "Artista_Id", "UserId", "EsActiva" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Artista_MembershipSuscripcion_MembershipPlan_Id",
                table: "Artista_MembershipSuscripcion",
                column: "MembershipPlan_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Artista_PayoutCuenta_Artista_Id",
                table: "Artista_PayoutCuenta",
                columns: new[] { "Artista_Id", "EsPorDefecto" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Artista_Estado",
                table: "CampaniaCrowdfunding",
                columns: new[] { "Artista_Id", "EstadoCampania_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Comentario_Campania_Id",
                table: "CampaniaCrowdfunding_Comentario",
                column: "Campania_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Comentario_ComentarioPadre_Id",
                table: "CampaniaCrowdfunding_Comentario",
                column: "ComentarioPadre_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Payout_ArtistaPayoutCuenta_Id",
                table: "CampaniaCrowdfunding_Payout",
                column: "ArtistaPayoutCuenta_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Payout_Campania_Estado",
                table: "CampaniaCrowdfunding_Payout",
                columns: new[] { "Campania_Id", "EstadoPayout_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Reward_Campania_Orden",
                table: "CampaniaCrowdfunding_Reward",
                columns: new[] { "Campania_Id", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_StretchGoal_Campania_Id",
                table: "CampaniaCrowdfunding_StretchGoal",
                column: "Campania_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaCrowdfunding_Update_Campania_Id",
                table: "CampaniaCrowdfunding_Update",
                column: "Campania_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoCrowdfunding_Campania_Estado",
                table: "PedidoCrowdfunding",
                columns: new[] { "Campania_Id", "EstadoPedido_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoCrowdfunding_Linea_Pedido",
                table: "PedidoCrowdfunding_Linea",
                column: "PedidoCrowdfunding_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoCrowdfunding_Linea_Reward_Id",
                table: "PedidoCrowdfunding_Linea",
                column: "Reward_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AportacionCrowdfunding");

            migrationBuilder.DropTable(
                name: "Artista_MembershipPago");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding_Comentario");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding_Payout");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding_StretchGoal");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding_Update");

            migrationBuilder.DropTable(
                name: "PedidoCrowdfunding_Linea");

            migrationBuilder.DropTable(
                name: "Artista_MembershipSuscripcion");

            migrationBuilder.DropTable(
                name: "Artista_PayoutCuenta");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding_Reward");

            migrationBuilder.DropTable(
                name: "PedidoCrowdfunding");

            migrationBuilder.DropTable(
                name: "Artista_MembershipPlan");

            migrationBuilder.DropTable(
                name: "CampaniaCrowdfunding");
        }
    }
}
