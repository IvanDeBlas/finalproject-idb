using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WePlayRises.Core.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maestra_EstadoAcuerdo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoAcuerdo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoAportacionCrowd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoAportacionCrowd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoCampaniaCrowd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoCampaniaCrowd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoEntregable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoEntregable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoNecesidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoNecesidad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoPayoutCrowd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoPayoutCrowd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoPedidoCrowd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoPedidoCrowd", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoPropuesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoPropuesta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoProyecto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoProyecto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoTareaPromo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoTareaPromo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_EstadoWalletTransaccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_EstadoWalletTransaccion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_MetodoPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_MetodoPago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_ModalidadTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_ModalidadTrabajo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_Moneda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    Simbolo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_Moneda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_RolMiembroArtista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_RolMiembroArtista", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoEventoPromo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoEventoPromo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoFinanciacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoFinanciacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoNecesidad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoNecesidad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoPromo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoPromo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoPromotor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoPromotor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoProyecto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoProyecto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoReward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoReward", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoSkill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestra_TipoValoracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestra_TipoValoracion", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Maestra_EstadoAportacionCrowd",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "PENDIENTE", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pendiente", 1 },
                    { 2, "AUTORIZADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Autorizada", 2 },
                    { 3, "CAPTURADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Capturada", 3 },
                    { 4, "CANCELADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cancelada", 4 },
                    { 5, "REEMBOLSADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reembolsada", 5 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_EstadoCampaniaCrowd",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "BORRADOR", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Borrador", 1 },
                    { 2, "EN_REVISION", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "En Revision", 2 },
                    { 3, "ACTIVA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Activa", 3 },
                    { 4, "FINALIZADA_EXITO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Finalizada con Exito", 4 },
                    { 5, "FINALIZADA_FRACASO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Finalizada sin Exito", 5 },
                    { 6, "CANCELADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cancelada", 6 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_EstadoPayoutCrowd",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "PENDIENTE", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pendiente", 1 },
                    { 2, "EN_PROCESO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "En Proceso", 2 },
                    { 3, "COMPLETADO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Completado", 3 },
                    { 4, "FALLIDO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fallido", 4 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_EstadoPedidoCrowd",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "PENDIENTE", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pendiente", 1 },
                    { 2, "CONFIRMADO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Confirmado", 2 },
                    { 3, "CANCELADO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cancelado", 3 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_MetodoPago",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "TARJETA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tarjeta de Credito", 1 },
                    { 2, "PAYPAL", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PayPal", 2 },
                    { 3, "STRIPE", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stripe", 3 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_Moneda",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden", "Simbolo" },
                values: new object[,]
                {
                    { 1, "EUR", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Euro", 1, "EUR" },
                    { 2, "USD", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US Dollar", 2, "$" },
                    { 3, "GBP", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "British Pound", 3, "GBP" }
                });

            migrationBuilder.InsertData(
                table: "Maestra_TipoFinanciacion",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "TODO_O_NADA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Todo o Nada", 1 },
                    { 2, "FLEXIBLE", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flexible", 2 }
                });

            migrationBuilder.InsertData(
                table: "Maestra_TipoReward",
                columns: new[] { "Id", "Codigo", "Descripcion", "EsActivo", "FechaCreacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "FISICO", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fisico", 1 },
                    { 2, "DIGITAL", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Digital", 2 },
                    { 3, "EXPERIENCIA", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Experiencia", 3 },
                    { 4, "NO_REWARD", null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sin Recompensa", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Maestra_EstadoAcuerdo");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoAportacionCrowd");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoCampaniaCrowd");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoEntregable");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoNecesidad");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoPayoutCrowd");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoPedidoCrowd");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoPropuesta");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoProyecto");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoTareaPromo");

            migrationBuilder.DropTable(
                name: "Maestra_EstadoWalletTransaccion");

            migrationBuilder.DropTable(
                name: "Maestra_MetodoPago");

            migrationBuilder.DropTable(
                name: "Maestra_ModalidadTrabajo");

            migrationBuilder.DropTable(
                name: "Maestra_Moneda");

            migrationBuilder.DropTable(
                name: "Maestra_RolMiembroArtista");

            migrationBuilder.DropTable(
                name: "Maestra_TipoEventoPromo");

            migrationBuilder.DropTable(
                name: "Maestra_TipoFinanciacion");

            migrationBuilder.DropTable(
                name: "Maestra_TipoNecesidad");

            migrationBuilder.DropTable(
                name: "Maestra_TipoPromo");

            migrationBuilder.DropTable(
                name: "Maestra_TipoPromotor");

            migrationBuilder.DropTable(
                name: "Maestra_TipoProyecto");

            migrationBuilder.DropTable(
                name: "Maestra_TipoReward");

            migrationBuilder.DropTable(
                name: "Maestra_TipoSkill");

            migrationBuilder.DropTable(
                name: "Maestra_TipoValoracion");
        }
    }
}
