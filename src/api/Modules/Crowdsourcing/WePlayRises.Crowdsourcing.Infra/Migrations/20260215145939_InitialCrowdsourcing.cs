using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WePlayRises.Crowdsourcing.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCrowdsourcing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaestrasCategoriaRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestrasCategoriaRol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NecesidadCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoArtistico_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoNecesidad_Id = table.Column<int>(type: "int", nullable: false),
                    EstadoNecesidad_Id = table.Column<int>(type: "int", nullable: false),
                    ModalidadTrabajo_Id = table.Column<int>(type: "int", nullable: false),
                    PresupuestoMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PresupuestoMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    UbicacionCiudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UbicacionPais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaLimitePropuestas = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaInicioPrevista = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NecesidadCrowdsourcing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasProyecto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasProyecto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaestrasRolProfesional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoriaRolId = table.Column<int>(type: "int", nullable: false),
                    ModalidadCobro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaestrasRolProfesional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaestrasRolProfesional_MaestrasCategoriaRol_CategoriaRolId",
                        column: x => x.CategoriaRolId,
                        principalTable: "MaestrasCategoriaRol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropuestaCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Necesidad_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PerfilProfesional_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MensajePropuesta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecioPropuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    DiasEstimados = table.Column<int>(type: "int", nullable: true),
                    EstadoPropuesta_Id = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestaCrowdsourcing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestaCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id",
                        column: x => x.Necesidad_Id,
                        principalTable: "NecesidadCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasProyectoNecesidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantillaProyectoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fase = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RolProfesionalId = table.Column<int>(type: "int", nullable: false),
                    PrecioMinOrientativo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PrecioMaxOrientativo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonedaId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Prioridad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Media"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasProyectoNecesidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantillasProyectoNecesidades_MaestrasRolProfesional_RolProfesionalId",
                        column: x => x.RolProfesionalId,
                        principalTable: "MaestrasRolProfesional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantillasProyectoNecesidades_PlantillasProyecto_PlantillaProyectoId",
                        column: x => x.PlantillaProyectoId,
                        principalTable: "PlantillasProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcuerdoCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Necesidad_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Propuesta_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Artista_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdProveedor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PerfilProfesional_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TituloInterno = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoAcuerdo_Id = table.Column<int>(type: "int", nullable: false),
                    Moneda_Id = table.Column<int>(type: "int", nullable: true),
                    ImporteTotalPactado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImporteAnticipo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PorcentajeAnticipo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaFinPrevista = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcuerdoCrowdsourcing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcuerdoCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id",
                        column: x => x.Necesidad_Id,
                        principalTable: "NecesidadCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcuerdoCrowdsourcing_PropuestaCrowdsourcing_Propuesta_Id",
                        column: x => x.Propuesta_Id,
                        principalTable: "PropuestaCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AcuerdoCrowdsourcing_Milestone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acuerdo_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    ImporteParcial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PorcentajeParcial = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FechaLimite = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcuerdoCrowdsourcing_Milestone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcuerdoCrowdsourcing_Milestone_AcuerdoCrowdsourcing_Acuerdo_Id",
                        column: x => x.Acuerdo_Id,
                        principalTable: "AcuerdoCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversacionCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Necesidad_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Acuerdo_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserIdArtista = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserIdProveedor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false),
                    FechaUltimoMensaje = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversacionCrowdsourcing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversacionCrowdsourcing_AcuerdoCrowdsourcing_Acuerdo_Id",
                        column: x => x.Acuerdo_Id,
                        principalTable: "AcuerdoCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConversacionCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id",
                        column: x => x.Necesidad_Id,
                        principalTable: "NecesidadCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ValoracionCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acuerdo_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdAutor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserIdValorado = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TipoValoracion_Id = table.Column<int>(type: "int", nullable: false),
                    Puntuacion = table.Column<byte>(type: "tinyint", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoracionCrowdsourcing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValoracionCrowdsourcing_AcuerdoCrowdsourcing_Acuerdo_Id",
                        column: x => x.Acuerdo_Id,
                        principalTable: "AcuerdoCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcuerdoCrowdsourcing_Entregable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Acuerdo_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Milestone_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlRecurso = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstadoEntregable_Id = table.Column<int>(type: "int", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    ComentarioAprobacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcuerdoCrowdsourcing_Entregable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcuerdoCrowdsourcing_Entregable_AcuerdoCrowdsourcing_Acuerdo_Id",
                        column: x => x.Acuerdo_Id,
                        principalTable: "AcuerdoCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcuerdoCrowdsourcing_Entregable_AcuerdoCrowdsourcing_Milestone_Milestone_Id",
                        column: x => x.Milestone_Id,
                        principalTable: "AcuerdoCrowdsourcing_Milestone",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MensajeCrowdsourcing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Conversacion_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdRemitente = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlAdjunto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Leido = table.Column<bool>(type: "bit", nullable: false),
                    FechaLeido = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajeCrowdsourcing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajeCrowdsourcing_ConversacionCrowdsourcing_Conversacion_Id",
                        column: x => x.Conversacion_Id,
                        principalTable: "ConversacionCrowdsourcing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MaestrasCategoriaRol",
                columns: new[] { "Id", "Icono", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, "music", "Produccion Musical", 1 },
                    { 2, "sliders", "Ingenieria de Audio", 2 },
                    { 3, "palette", "Diseno y Creatividad", 3 },
                    { 4, "megaphone", "Marketing y Promocion", 4 },
                    { 5, "briefcase", "Gestion y Legal", 5 },
                    { 6, "mic", "Produccion de Eventos / Live", 6 }
                });

            migrationBuilder.InsertData(
                table: "PlantillasProyecto",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "Icono", "Nombre", "Orden" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), true, "Todas las fases para grabar tu primer disco: desde pre-produccion hasta el master final", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "music", "Grabar un Album / EP", 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, "Todo lo necesario para producir un videoclip profesional", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "video", "Produccion de Videoclip", 2 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), true, "Planifica y ejecuta una gira de conciertos", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "map", "Organizar Gira / Tour", 3 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), true, "Estrategia completa de promocion y marketing musical", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "megaphone", "Campana de Marketing", 4 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), true, "Todo lo necesario para lanzar un single con impacto", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "disc", "Lanzamiento de Single", 5 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), true, "Construye tu identidad digital y presencia en redes", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "globe", "Presencia Online", 6 }
                });

            migrationBuilder.InsertData(
                table: "MaestrasRolProfesional",
                columns: new[] { "Id", "Activo", "CategoriaRolId", "Descripcion", "ModalidadCobro", "Nombre" },
                values: new object[,]
                {
                    { 1, true, 1, "Dirige la vision sonora del proyecto completo", "Por proyecto o por cancion", "Productor musical" },
                    { 2, true, 1, "Crea arreglos instrumentales y vocales", "Por cancion", "Arreglista / Compositor" },
                    { 3, true, 1, "Crea bases ritmicas y beats", "Por beat", "Beatmaker" },
                    { 4, true, 1, "Coordina ensayos y direccion artistica en vivo", "Por proyecto", "Director musical" },
                    { 5, true, 1, "Interpreta instrumentos para grabaciones", "Por sesion", "Musico de sesion" },
                    { 6, true, 1, "Prepara y entrena vocalmente al artista", "Por sesion", "Vocal coach" },
                    { 7, true, 2, "Captura audio de alta calidad en estudio", "Por dia", "Ingeniero de grabacion" },
                    { 8, true, 2, "Equilibra y procesa todas las pistas grabadas", "Por cancion", "Ingeniero de mezcla" },
                    { 9, true, 2, "Optimiza el audio final para distribucion", "Por cancion", "Ingeniero de mastering" },
                    { 10, true, 2, "Gestiona el sonido en eventos en vivo", "Por evento", "Tecnico de sonido (live)" },
                    { 11, true, 2, "Crea efectos de sonido y ambientes", "Por proyecto", "Disenador sonoro" },
                    { 12, true, 3, "Crea portadas, logos y material visual", "Por proyecto", "Disenador grafico" },
                    { 13, true, 3, "Sesiones fotograficas profesionales", "Por sesion", "Fotografo" },
                    { 14, true, 3, "Dirige la produccion audiovisual del videoclip", "Por video", "Director de videoclip" },
                    { 15, true, 3, "Edita y post-produce material audiovisual", "Por video", "Editor de video" },
                    { 16, true, 3, "Crea animaciones y graficos en movimiento", "Por proyecto", "Animador / Motion graphics" },
                    { 17, true, 3, "Define la estetica visual del proyecto", "Por proyecto", "Director de arte" },
                    { 18, true, 3, "Disena la imagen y vestuario del artista", "Por sesion", "Estilista / Vestuarista" },
                    { 19, true, 4, "Gestiona redes sociales y comunidad online", "Por mes", "Community manager" },
                    { 20, true, 4, "Gestiona prensa y relaciones publicas", "Por proyecto", "Publicista musical" },
                    { 21, true, 4, "Campanas de publicidad digital (Spotify, Meta, etc.)", "Por mes", "Especialista en ads" },
                    { 22, true, 4, "Produce contenido para redes sociales y plataformas", "Por proyecto", "Creador de contenido" },
                    { 23, true, 4, "Gestiona la distribucion en plataformas de streaming", "Por proyecto", "Experto en distribucion digital" },
                    { 24, true, 5, "Gestiona la carrera y negocios del artista", "Por mes", "Manager artistico" },
                    { 25, true, 5, "Asesoria legal en contratos y derechos", "Por hora", "Abogado musical" },
                    { 26, true, 5, "Registro y gestion de derechos de propiedad intelectual", "Por proyecto", "Especialista en derechos de autor" },
                    { 27, true, 6, "Organiza y promueve conciertos y eventos", "Por evento", "Promotor de eventos" },
                    { 28, true, 6, "Coordina la logistica del escenario", "Por evento", "Stage manager" },
                    { 29, true, 6, "Disena y opera la iluminacion de eventos", "Por evento", "Tecnico de iluminacion" },
                    { 30, true, 6, "Gestiona la logistica de giras", "Por dia", "Roadie / Tour manager" }
                });

            migrationBuilder.InsertData(
                table: "PlantillasProyectoNecesidades",
                columns: new[] { "Id", "Descripcion", "Fase", "FechaCreacion", "MonedaId", "Orden", "PlantillaProyectoId", "PrecioMaxOrientativo", "PrecioMinOrientativo", "Prioridad", "RolProfesionalId", "Titulo" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "Crear arreglos instrumentales y vocales", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("11111111-1111-1111-1111-111111111111"), 1500m, 200m, "Alta", 2, "Composicion y arreglos musicales" },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "Definir la vision sonora del proyecto", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("11111111-1111-1111-1111-111111111111"), 3000m, 500m, "Alta", 1, "Produccion musical" },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "Preparacion vocal para grabacion", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("11111111-1111-1111-1111-111111111111"), 500m, 100m, "Media", 6, "Coaching vocal" },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "Captura de audio en estudio", "Grabacion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("11111111-1111-1111-1111-111111111111"), 1500m, 300m, "Alta", 7, "Ingeniero de grabacion" },
                    { new Guid("11111111-1111-1111-1111-111111111105"), "Instrumentistas para grabacion", "Grabacion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("11111111-1111-1111-1111-111111111111"), 1000m, 200m, "Media", 5, "Musicos de sesion" },
                    { new Guid("11111111-1111-1111-1111-111111111106"), "Equilibrar y procesar todas las pistas", "Mezcla y Master", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 6, new Guid("11111111-1111-1111-1111-111111111111"), 2000m, 300m, "Alta", 8, "Mezcla de pistas" },
                    { new Guid("11111111-1111-1111-1111-111111111107"), "Optimizar audio final para distribucion", "Mezcla y Master", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 7, new Guid("11111111-1111-1111-1111-111111111111"), 1000m, 200m, "Alta", 9, "Mastering" },
                    { new Guid("11111111-1111-1111-1111-111111111108"), "Crear portada del album", "Diseno y Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8, new Guid("11111111-1111-1111-1111-111111111111"), 800m, 150m, "Media", 12, "Diseno de portada" },
                    { new Guid("11111111-1111-1111-1111-111111111109"), "Fotos promocionales del artista", "Diseno y Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 9, new Guid("11111111-1111-1111-1111-111111111111"), 1000m, 200m, "Media", 13, "Sesion fotografica" },
                    { new Guid("11111111-1111-1111-1111-111111111110"), "Subir a plataformas de streaming", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 10, new Guid("11111111-1111-1111-1111-111111111111"), 300m, 50m, "Alta", 23, "Distribucion digital" },
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Campana de prensa y publicity", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 11, new Guid("11111111-1111-1111-1111-111111111111"), 1500m, 200m, "Media", 20, "Publicidad y prensa" },
                    { new Guid("22222222-2222-2222-2222-222222222201"), "Concepto creativo y storyboard", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("22222222-2222-2222-2222-222222222222"), 3000m, 500m, "Alta", 14, "Direccion de videoclip" },
                    { new Guid("22222222-2222-2222-2222-222222222202"), "Definir estetica visual del video", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("22222222-2222-2222-2222-222222222222"), 1500m, 300m, "Alta", 17, "Director de arte" },
                    { new Guid("22222222-2222-2222-2222-222222222203"), "Imagen y vestuario del artista", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("22222222-2222-2222-2222-222222222222"), 800m, 150m, "Media", 18, "Estilista / Vestuarista" },
                    { new Guid("22222222-2222-2222-2222-222222222204"), "Camaras, iluminacion y equipo tecnico", "Grabacion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("22222222-2222-2222-2222-222222222222"), 5000m, 800m, "Alta", 14, "Equipo de grabacion video" },
                    { new Guid("22222222-2222-2222-2222-222222222205"), "Diseno y operacion de iluminacion", "Grabacion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("22222222-2222-2222-2222-222222222222"), 1000m, 200m, "Media", 29, "Iluminacion" },
                    { new Guid("22222222-2222-2222-2222-222222222206"), "Montaje y edicion del videoclip", "Post-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 6, new Guid("22222222-2222-2222-2222-222222222222"), 2000m, 300m, "Alta", 15, "Edicion de video" },
                    { new Guid("22222222-2222-2222-2222-222222222207"), "Efectos visuales y graficos animados", "Post-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 7, new Guid("22222222-2222-2222-2222-222222222222"), 2000m, 300m, "Media", 16, "Animacion / Motion graphics" },
                    { new Guid("22222222-2222-2222-2222-222222222208"), "Color grading profesional", "Post-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8, new Guid("22222222-2222-2222-2222-222222222222"), 800m, 150m, "Media", 15, "Correccion de color" },
                    { new Guid("22222222-2222-2222-2222-222222222209"), "Teasers y clips promocionales", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 9, new Guid("22222222-2222-2222-2222-222222222222"), 500m, 100m, "Media", 22, "Contenido para redes" },
                    { new Guid("22222222-2222-2222-2222-222222222210"), "Estrategia de lanzamiento del video", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 10, new Guid("22222222-2222-2222-2222-222222222222"), 1000m, 200m, "Alta", 21, "Campana de lanzamiento" },
                    { new Guid("22222222-2222-2222-2222-222222222211"), "YouTube, Vevo y redes sociales", "Distribucion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 11, new Guid("22222222-2222-2222-2222-222222222222"), 200m, 50m, "Baja", 23, "Distribucion en plataformas" },
                    { new Guid("33333333-3333-3333-3333-333333333301"), "Planificacion y logistica de la gira", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("33333333-3333-3333-3333-333333333333"), 3000m, 500m, "Alta", 30, "Tour manager" },
                    { new Guid("33333333-3333-3333-3333-333333333302"), "Conseguir venues y negociar fechas", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("33333333-3333-3333-3333-333333333333"), 2000m, 300m, "Alta", 27, "Promotor de eventos" },
                    { new Guid("33333333-3333-3333-3333-333333333303"), "Preparacion del show en vivo", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("33333333-3333-3333-3333-333333333333"), 1500m, 300m, "Alta", 4, "Director musical" },
                    { new Guid("33333333-3333-3333-3333-333333333304"), "Revision de contratos con venues", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("33333333-3333-3333-3333-333333333333"), 1000m, 200m, "Media", 25, "Abogado musical" },
                    { new Guid("33333333-3333-3333-3333-333333333305"), "Sonido en vivo para cada show", "Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("33333333-3333-3333-3333-333333333333"), 1000m, 200m, "Alta", 10, "Tecnico de sonido" },
                    { new Guid("33333333-3333-3333-3333-333333333306"), "Iluminacion para cada show", "Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 6, new Guid("33333333-3333-3333-3333-333333333333"), 800m, 150m, "Media", 29, "Tecnico de iluminacion" },
                    { new Guid("33333333-3333-3333-3333-333333333307"), "Coordinacion de escenario", "Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 7, new Guid("33333333-3333-3333-3333-333333333333"), 800m, 150m, "Media", 28, "Stage manager" },
                    { new Guid("33333333-3333-3333-3333-333333333308"), "Banda para el show en vivo", "Produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8, new Guid("33333333-3333-3333-3333-333333333333"), 3000m, 500m, "Alta", 5, "Musicos de sesion" },
                    { new Guid("33333333-3333-3333-3333-333333333309"), "Promocion en redes de cada fecha", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 9, new Guid("33333333-3333-3333-3333-333333333333"), 800m, 200m, "Media", 19, "Community manager" },
                    { new Guid("33333333-3333-3333-3333-333333333310"), "Documentar la gira visualmente", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 10, new Guid("33333333-3333-3333-3333-333333333333"), 1500m, 300m, "Baja", 13, "Fotografo de gira" },
                    { new Guid("33333333-3333-3333-3333-333333333311"), "Contenido backstage y making of", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 11, new Guid("33333333-3333-3333-3333-333333333333"), 1000m, 200m, "Baja", 22, "Creador de contenido" },
                    { new Guid("44444444-4444-4444-4444-444444444401"), "Plan de comunicacion y prensa", "Estrategia", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("44444444-4444-4444-4444-444444444444"), 2000m, 300m, "Alta", 20, "Publicista musical" },
                    { new Guid("44444444-4444-4444-4444-444444444402"), "Campanas de publicidad digital", "Estrategia", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("44444444-4444-4444-4444-444444444444"), 2000m, 300m, "Alta", 21, "Especialista en ads" },
                    { new Guid("44444444-4444-4444-4444-444444444403"), "Gestion de redes sociales", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("44444444-4444-4444-4444-444444444444"), 800m, 200m, "Alta", 19, "Community manager" },
                    { new Guid("44444444-4444-4444-4444-444444444404"), "Contenido visual y audiovisual", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("44444444-4444-4444-4444-444444444444"), 1000m, 200m, "Alta", 22, "Creador de contenido" },
                    { new Guid("44444444-4444-4444-4444-444444444405"), "Sesion de fotos promocionales", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("44444444-4444-4444-4444-444444444444"), 1000m, 200m, "Media", 13, "Fotografo" },
                    { new Guid("44444444-4444-4444-4444-444444444406"), "Material grafico para campana", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 6, new Guid("44444444-4444-4444-4444-444444444444"), 800m, 150m, "Media", 12, "Disenador grafico" },
                    { new Guid("44444444-4444-4444-4444-444444444407"), "Plataformas de streaming y tiendas", "Distribucion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 7, new Guid("44444444-4444-4444-4444-444444444444"), 300m, 50m, "Alta", 23, "Distribucion digital" },
                    { new Guid("44444444-4444-4444-4444-444444444408"), "Registro de obra en entidades", "Legal", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8, new Guid("44444444-4444-4444-4444-444444444444"), 500m, 100m, "Media", 26, "Registro de derechos" },
                    { new Guid("44444444-4444-4444-4444-444444444409"), "Coordinacion general de la campana", "Seguimiento", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 9, new Guid("44444444-4444-4444-4444-444444444444"), 1500m, 300m, "Media", 24, "Manager artistico" },
                    { new Guid("55555555-5555-5555-5555-555555555501"), "Produccion del single", "Pre-produccion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("55555555-5555-5555-5555-555555555555"), 2000m, 300m, "Alta", 1, "Produccion musical" },
                    { new Guid("55555555-5555-5555-5555-555555555502"), "Grabacion del single", "Grabacion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("55555555-5555-5555-5555-555555555555"), 1000m, 200m, "Alta", 7, "Ingeniero de grabacion" },
                    { new Guid("55555555-5555-5555-5555-555555555503"), "Mezcla del single", "Mezcla y Master", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("55555555-5555-5555-5555-555555555555"), 1000m, 200m, "Alta", 8, "Mezcla" },
                    { new Guid("55555555-5555-5555-5555-555555555504"), "Mastering del single", "Mezcla y Master", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("55555555-5555-5555-5555-555555555555"), 500m, 100m, "Alta", 9, "Mastering" },
                    { new Guid("55555555-5555-5555-5555-555555555505"), "Portada del single", "Diseno", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("55555555-5555-5555-5555-555555555555"), 500m, 100m, "Media", 12, "Diseno de portada" },
                    { new Guid("55555555-5555-5555-5555-555555555506"), "Subida a plataformas", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 6, new Guid("55555555-5555-5555-5555-555555555555"), 200m, 50m, "Alta", 23, "Distribucion digital" },
                    { new Guid("55555555-5555-5555-5555-555555555507"), "Publicidad digital del single", "Promocion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 7, new Guid("55555555-5555-5555-5555-555555555555"), 1000m, 200m, "Media", 21, "Campana de ads" },
                    { new Guid("55555555-5555-5555-5555-555555555508"), "Registro del single", "Legal", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 8, new Guid("55555555-5555-5555-5555-555555555555"), 300m, 50m, "Media", 26, "Registro de derechos" },
                    { new Guid("66666666-6666-6666-6666-666666666601"), "Logo, paleta de colores, branding", "Identidad", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new Guid("66666666-6666-6666-6666-666666666666"), 1000m, 200m, "Alta", 12, "Disenador grafico" },
                    { new Guid("66666666-6666-6666-6666-666666666602"), "Sesion de fotos para perfiles", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, new Guid("66666666-6666-6666-6666-666666666666"), 800m, 150m, "Alta", 13, "Fotografo" },
                    { new Guid("66666666-6666-6666-6666-666666666603"), "Configuracion y gestion de redes", "Redes", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 3, new Guid("66666666-6666-6666-6666-666666666666"), 800m, 200m, "Alta", 19, "Community manager" },
                    { new Guid("66666666-6666-6666-6666-666666666604"), "Contenido inicial para perfiles", "Contenido", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 4, new Guid("66666666-6666-6666-6666-666666666666"), 600m, 150m, "Media", 22, "Creador de contenido" },
                    { new Guid("66666666-6666-6666-6666-666666666605"), "Perfiles en plataformas de streaming", "Distribucion", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, 5, new Guid("66666666-6666-6666-6666-666666666666"), 200m, 50m, "Media", 23, "Distribucion digital" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Artista_Estado",
                table: "AcuerdoCrowdsourcing",
                columns: new[] { "Artista_Id", "EstadoAcuerdo_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Necesidad",
                table: "AcuerdoCrowdsourcing",
                column: "Necesidad_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Propuesta_Id",
                table: "AcuerdoCrowdsourcing",
                column: "Propuesta_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Entregable_Acuerdo_Id",
                table: "AcuerdoCrowdsourcing_Entregable",
                column: "Acuerdo_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Entregable_Milestone_Id",
                table: "AcuerdoCrowdsourcing_Entregable",
                column: "Milestone_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AcuerdoCrowdsourcing_Milestone_Acuerdo_Id",
                table: "AcuerdoCrowdsourcing_Milestone",
                column: "Acuerdo_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionCrowdsourcing_Acuerdo_Id",
                table: "ConversacionCrowdsourcing",
                column: "Acuerdo_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ConversacionCrowdsourcing_Necesidad_Id",
                table: "ConversacionCrowdsourcing",
                column: "Necesidad_Id");

            migrationBuilder.CreateIndex(
                name: "IX_MaestrasCategoriaRol_Orden",
                table: "MaestrasCategoriaRol",
                column: "Orden");

            migrationBuilder.CreateIndex(
                name: "IX_MaestrasRolProfesional_Activo",
                table: "MaestrasRolProfesional",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_MaestrasRolProfesional_CategoriaRolId",
                table: "MaestrasRolProfesional",
                column: "CategoriaRolId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeCrowdsourcing_Conversacion_Id",
                table: "MensajeCrowdsourcing",
                column: "Conversacion_Id");

            migrationBuilder.CreateIndex(
                name: "IX_NecesidadCrowdsourcing_Artista_Estado",
                table: "NecesidadCrowdsourcing",
                columns: new[] { "Artista_Id", "EstadoNecesidad_Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasProyecto_Activo",
                table: "PlantillasProyecto",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasProyecto_Orden",
                table: "PlantillasProyecto",
                column: "Orden");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasProyectoNecesidades_PlantillaProyectoId",
                table: "PlantillasProyectoNecesidades",
                column: "PlantillaProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasProyectoNecesidades_Prioridad",
                table: "PlantillasProyectoNecesidades",
                column: "Prioridad");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasProyectoNecesidades_RolProfesionalId",
                table: "PlantillasProyectoNecesidades",
                column: "RolProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaCrowdsourcing_Necesidad_Id",
                table: "PropuestaCrowdsourcing",
                column: "Necesidad_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionCrowdsourcing_Acuerdo_Id",
                table: "ValoracionCrowdsourcing",
                column: "Acuerdo_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcuerdoCrowdsourcing_Entregable");

            migrationBuilder.DropTable(
                name: "MensajeCrowdsourcing");

            migrationBuilder.DropTable(
                name: "PlantillasProyectoNecesidades");

            migrationBuilder.DropTable(
                name: "ValoracionCrowdsourcing");

            migrationBuilder.DropTable(
                name: "AcuerdoCrowdsourcing_Milestone");

            migrationBuilder.DropTable(
                name: "ConversacionCrowdsourcing");

            migrationBuilder.DropTable(
                name: "MaestrasRolProfesional");

            migrationBuilder.DropTable(
                name: "PlantillasProyecto");

            migrationBuilder.DropTable(
                name: "AcuerdoCrowdsourcing");

            migrationBuilder.DropTable(
                name: "MaestrasCategoriaRol");

            migrationBuilder.DropTable(
                name: "PropuestaCrowdsourcing");

            migrationBuilder.DropTable(
                name: "NecesidadCrowdsourcing");
        }
    }
}
