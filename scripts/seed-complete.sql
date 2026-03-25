SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

-- ============================================================================
-- WePlay Rises - Complete Seed Script
-- Generated: 2026-03-24
-- Purpose: Idempotent seed for Docker and Azure SQL demo environments
-- ============================================================================

PRINT '=== WePlay Rises Seed Script - START ==='
PRINT ''

-- ============================================================================
-- SECTION 1: DELETE ALL DATA (FK-safe order)
-- ============================================================================

PRINT '--- Deleting existing data (FK-safe order) ---'

-- Crowdpromotion children first
DELETE FROM PromotorWalletTransaccion;
DELETE FROM PromotorWallet;
DELETE FROM PromoTareaPromotor;
DELETE FROM PromoEvento;
DELETE FROM PromoTarea;
DELETE FROM PromoProgramaPromotor;
DELETE FROM PromoPrograma;
DELETE FROM Promotor;

-- Crowdsourcing children first
DELETE FROM MensajeCrowdsourcing;
DELETE FROM ConversacionCrowdsourcing;
DELETE FROM ValoracionCrowdsourcing;
DELETE FROM AcuerdoCrowdsourcing_Entregable;
DELETE FROM AcuerdoCrowdsourcing_Milestone;
DELETE FROM AcuerdoCrowdsourcing;
DELETE FROM PropuestaCrowdsourcing;
DELETE FROM NecesidadCrowdsourcing;

-- Crowdfunding children
DELETE FROM AportacionCrowdfunding;
DELETE FROM PedidoCrowdfunding_Linea;
DELETE FROM PedidoCrowdfunding;
DELETE FROM CampaniaCrowdfunding_Comentario;
DELETE FROM CampaniaCrowdfunding_Update;
DELETE FROM CampaniaCrowdfunding_StretchGoal;
DELETE FROM CampaniaCrowdfunding_Payout;
DELETE FROM CampaniaCrowdfunding_Reward;
DELETE FROM CampaniaCrowdfunding;

-- Membership / Payout
DELETE FROM Artista_MembershipPago;
DELETE FROM Artista_MembershipSuscripcion;
DELETE FROM Artista_MembershipPlan;
DELETE FROM Artista_PayoutCuenta;

-- UserAccess children
DELETE FROM PerfilProfesional_Skill;
DELETE FROM PerfilProfesional_PortfolioItem;
DELETE FROM PerfilProfesional;
DELETE FROM ProyectoArtistico;
DELETE FROM Artista_Fan;
DELETE FROM Artista_Miembro;
DELETE FROM FanProfile;
DELETE FROM DireccionPostal;
DELETE FROM Artista;

-- Identity (but NOT AspNetRoles or AspNetRoleClaims)
DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUserTokens;
DELETE FROM AspNetUserLogins;
DELETE FROM AspNetUserClaims;
DELETE FROM AspNetUsers;

PRINT '--- Data deletion complete ---'
GO

-- ============================================================================
-- SECTION 2: POPULATE EMPTY MAESTRA TABLES (Core module)
-- ============================================================================

PRINT '--- Populating empty Maestra tables ---'

-- Maestra_EstadoNecesidad (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoNecesidad)
BEGIN
    INSERT INTO Maestra_EstadoNecesidad (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'ABIERTA',     N'Abierta',     N'Necesidad abierta para propuestas',    1, 1, '2026-01-01'),
    (2, N'EN_PROCESO',  N'En Proceso',  N'Propuesta aceptada, trabajo en curso', 1, 2, '2026-01-01'),
    (3, N'CERRADA',     N'Cerrada',     N'Necesidad completada',                 1, 3, '2026-01-01'),
    (4, N'CANCELADA',   N'Cancelada',   N'Necesidad cancelada',                  1, 4, '2026-01-01');
END

-- Maestra_EstadoPropuesta (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoPropuesta)
BEGIN
    INSERT INTO Maestra_EstadoPropuesta (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'PENDIENTE',  N'Pendiente',  N'Propuesta enviada, esperando revision', 1, 1, '2026-01-01'),
    (2, N'ACEPTADA',   N'Aceptada',   N'Propuesta aceptada por el artista',     1, 2, '2026-01-01'),
    (3, N'RECHAZADA',  N'Rechazada',  N'Propuesta rechazada',                   1, 3, '2026-01-01'),
    (4, N'RETIRADA',   N'Retirada',   N'Propuesta retirada por el profesional', 1, 4, '2026-01-01');
END

-- Maestra_ModalidadTrabajo (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_ModalidadTrabajo)
BEGIN
    INSERT INTO Maestra_ModalidadTrabajo (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'REMOTO',      N'Remoto',      N'Trabajo completamente remoto',     1, 1, '2026-01-01'),
    (2, N'PRESENCIAL',  N'Presencial',  N'Trabajo presencial en ubicacion',  1, 2, '2026-01-01'),
    (3, N'HIBRIDO',     N'Hibrido',     N'Combinacion remoto y presencial',  1, 3, '2026-01-01');
END

-- Maestra_TipoNecesidad (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoNecesidad)
BEGIN
    INSERT INTO Maestra_TipoNecesidad (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'CONTRATACION',  N'Contratacion',  N'Servicio profesional remunerado',  1, 1, '2026-01-01'),
    (2, N'COLABORACION',  N'Colaboracion',  N'Colaboracion con participacion',   1, 2, '2026-01-01'),
    (3, N'VOLUNTARIADO',  N'Voluntariado',  N'Contribucion voluntaria',          1, 3, '2026-01-01');
END

-- Maestra_TipoProyecto (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoProyecto)
BEGIN
    INSERT INTO Maestra_TipoProyecto (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'ALBUM',          N'Album',          N'Grabacion de album o EP',       1, 1, '2026-01-01'),
    (2, N'SINGLE',         N'Single',         N'Lanzamiento de single',         1, 2, '2026-01-01'),
    (3, N'VIDEOCLIP',      N'Videoclip',      N'Produccion de videoclip',       1, 3, '2026-01-01'),
    (4, N'GIRA',           N'Gira',           N'Organizacion de gira',          1, 4, '2026-01-01'),
    (5, N'DOCUMENTAL',     N'Documental',     N'Produccion de documental',      1, 5, '2026-01-01'),
    (6, N'MERCHANDISING',  N'Merchandising',  N'Linea de merchandising',        1, 6, '2026-01-01');
END

-- Maestra_EstadoProyecto (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoProyecto)
BEGIN
    INSERT INTO Maestra_EstadoProyecto (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'PLANIFICACION',  N'Planificacion',  N'Proyecto en fase de planificacion',  1, 1, '2026-01-01'),
    (2, N'EN_CURSO',       N'En Curso',       N'Proyecto en desarrollo activo',      1, 2, '2026-01-01'),
    (3, N'COMPLETADO',     N'Completado',     N'Proyecto finalizado',                1, 3, '2026-01-01'),
    (4, N'CANCELADO',      N'Cancelado',      N'Proyecto cancelado',                 1, 4, '2026-01-01');
END

-- Maestra_TipoValoracion (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoValoracion)
BEGIN
    INSERT INTO Maestra_TipoValoracion (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'ARTISTA_A_PROFESIONAL',    N'Artista a Profesional',    N'Valoracion del artista al profesional',  1, 1, '2026-01-01'),
    (2, N'PROFESIONAL_A_ARTISTA',    N'Profesional a Artista',    N'Valoracion del profesional al artista',  1, 2, '2026-01-01');
END

-- Maestra_TipoPromotor (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoPromotor)
BEGIN
    INSERT INTO Maestra_TipoPromotor (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'INDIVIDUAL',          N'Individual',          N'Promotor independiente',         1, 1, '2026-01-01'),
    (2, N'AGENCIA',             N'Agencia',             N'Agencia de promocion',           1, 2, '2026-01-01'),
    (3, N'MEDIO_COMUNICACION',  N'Medio de Comunicacion', N'Medio de comunicacion',        1, 3, '2026-01-01');
END

-- Maestra_TipoPromo (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoPromo)
BEGIN
    INSERT INTO Maestra_TipoPromo (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'REDES_SOCIALES',  N'Redes Sociales',  N'Promocion en redes sociales',     1, 1, '2026-01-01'),
    (2, N'BLOG',            N'Blog',            N'Promocion en blogs',              1, 2, '2026-01-01'),
    (3, N'RADIO',           N'Radio',           N'Promocion en radio',              1, 3, '2026-01-01'),
    (4, N'PRENSA',          N'Prensa',          N'Promocion en prensa escrita',     1, 4, '2026-01-01'),
    (5, N'INFLUENCER',      N'Influencer',      N'Promocion via influencers',       1, 5, '2026-01-01');
END

-- Maestra_EstadoTareaPromo (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoTareaPromo)
BEGIN
    INSERT INTO Maestra_EstadoTareaPromo (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'PENDIENTE',     N'Pendiente',     N'Tarea pendiente de inicio',    1, 1, '2026-01-01'),
    (2, N'EN_PROGRESO',   N'En Progreso',   N'Tarea en ejecucion',          1, 2, '2026-01-01'),
    (3, N'COMPLETADA',    N'Completada',    N'Tarea completada',             1, 3, '2026-01-01'),
    (4, N'VALIDADA',      N'Validada',      N'Tarea validada por artista',   1, 4, '2026-01-01'),
    (5, N'RECHAZADA',     N'Rechazada',     N'Tarea rechazada',              1, 5, '2026-01-01');
END

-- Maestra_TipoEventoPromo (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoEventoPromo)
BEGIN
    INSERT INTO Maestra_TipoEventoPromo (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'CLICK',       N'Click',       N'Evento de click',        1, 1, '2026-01-01'),
    (2, N'IMPRESION',   N'Impresion',   N'Evento de impresion',    1, 2, '2026-01-01'),
    (3, N'CONVERSION',  N'Conversion',  N'Evento de conversion',   1, 3, '2026-01-01'),
    (4, N'COMPARTIR',   N'Compartir',   N'Evento de compartir',    1, 4, '2026-01-01');
END

-- Maestra_EstadoWalletTransaccion (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoWalletTransaccion)
BEGIN
    INSERT INTO Maestra_EstadoWalletTransaccion (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'PENDIENTE',     N'Pendiente',     N'Transaccion pendiente',      1, 1, '2026-01-01'),
    (2, N'COMPLETADA',    N'Completada',    N'Transaccion completada',     1, 2, '2026-01-01'),
    (3, N'FALLIDA',       N'Fallida',       N'Transaccion fallida',        1, 3, '2026-01-01'),
    (4, N'REEMBOLSADA',   N'Reembolsada',   N'Transaccion reembolsada',   1, 4, '2026-01-01');
END

-- Maestra_TipoSkill (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_TipoSkill)
BEGIN
    INSERT INTO Maestra_TipoSkill (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'TECNICA',   N'Tecnica',   N'Habilidad tecnica',    1, 1, '2026-01-01'),
    (2, N'CREATIVA',  N'Creativa',  N'Habilidad creativa',   1, 2, '2026-01-01'),
    (3, N'GESTION',   N'Gestion',   N'Habilidad de gestion', 1, 3, '2026-01-01');
END

-- Maestra_RolMiembroArtista (if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_RolMiembroArtista)
BEGIN
    INSERT INTO Maestra_RolMiembroArtista (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'PROPIETARIO',   N'Propietario',   N'Propietario del perfil de artista',  1, 1, '2026-01-01'),
    (2, N'MANAGER',       N'Manager',       N'Manager del artista',                1, 2, '2026-01-01'),
    (3, N'COLABORADOR',   N'Colaborador',   N'Colaborador del artista',            1, 3, '2026-01-01');
END

-- Maestra_EstadoAcuerdo (Core, if empty - distinct from MaestraEstadoAcuerdo in Crowdsourcing)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoAcuerdo)
BEGIN
    INSERT INTO Maestra_EstadoAcuerdo (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'ACTIVO',      N'Activo',      N'Acuerdo activo',        1, 1, '2026-01-01'),
    (2, N'COMPLETADO',  N'Completado',  N'Acuerdo completado',    1, 2, '2026-01-01'),
    (3, N'CANCELADO',   N'Cancelado',   N'Acuerdo cancelado',     1, 3, '2026-01-01');
END

-- Maestra_EstadoEntregable (Core, if empty)
IF NOT EXISTS (SELECT 1 FROM Maestra_EstadoEntregable)
BEGIN
    INSERT INTO Maestra_EstadoEntregable (Id, Codigo, Nombre, Descripcion, EsActivo, Orden, FechaCreacion) VALUES
    (1, N'ENTREGADO',  N'Entregado',  N'Entregable subido, pendiente revision',  1, 1, '2026-01-01'),
    (2, N'APROBADO',   N'Aprobado',   N'Entregable aprobado por el artista',     1, 2, '2026-01-01'),
    (3, N'RECHAZADO',  N'Rechazado',  N'Entregable rechazado',                   1, 3, '2026-01-01');
END

PRINT '--- Maestra tables populated ---'
GO

-- ============================================================================
-- SECTION 3: DECLARE ALL GUID VARIABLES
-- ============================================================================

PRINT '--- Creating users, artists, campaigns, and all related data ---'

-- User IDs (nvarchar - ASP.NET Identity style)
DECLARE @userVetusta    NVARCHAR(450) = LOWER(NEWID());
DECLARE @userRosalia    NVARCHAR(450) = LOWER(NEWID());
DECLARE @userTangana    NVARCHAR(450) = LOWER(NEWID());
DECLARE @userBadBunny   NVARCHAR(450) = LOWER(NEWID());
DECLARE @userBillie     NVARCHAR(450) = LOWER(NEWID());
DECLARE @userArctic     NVARCHAR(450) = LOWER(NEWID());
DECLARE @userMuse       NVARCHAR(450) = LOWER(NEWID());
DECLARE @userArcade     NVARCHAR(450) = LOWER(NEWID());
DECLARE @userMaria      NVARCHAR(450) = LOWER(NEWID());
DECLARE @userCarlos     NVARCHAR(450) = LOWER(NEWID());
DECLARE @userAna        NVARCHAR(450) = LOWER(NEWID());
DECLARE @userPablo      NVARCHAR(450) = LOWER(NEWID());
DECLARE @userEmma       NVARCHAR(450) = LOWER(NEWID());

-- Artista IDs
DECLARE @artVetusta     UNIQUEIDENTIFIER = NEWID();
DECLARE @artRosalia     UNIQUEIDENTIFIER = NEWID();
DECLARE @artTangana     UNIQUEIDENTIFIER = NEWID();
DECLARE @artBadBunny    UNIQUEIDENTIFIER = NEWID();
DECLARE @artBillie      UNIQUEIDENTIFIER = NEWID();
DECLARE @artArctic      UNIQUEIDENTIFIER = NEWID();
DECLARE @artMuse        UNIQUEIDENTIFIER = NEWID();
DECLARE @artArcade      UNIQUEIDENTIFIER = NEWID();

-- FanProfile IDs
DECLARE @fanMaria       UNIQUEIDENTIFIER = NEWID();
DECLARE @fanCarlos      UNIQUEIDENTIFIER = NEWID();
DECLARE @fanAna         UNIQUEIDENTIFIER = NEWID();
DECLARE @fanPablo       UNIQUEIDENTIFIER = NEWID();
DECLARE @fanEmma        UNIQUEIDENTIFIER = NEWID();

-- Campaign IDs
DECLARE @campVetusta    UNIQUEIDENTIFIER = NEWID();
DECLARE @campRosalia    UNIQUEIDENTIFIER = NEWID();
DECLARE @campTangana    UNIQUEIDENTIFIER = NEWID();
DECLARE @campBadBunny   UNIQUEIDENTIFIER = NEWID();
DECLARE @campBillie     UNIQUEIDENTIFIER = NEWID();
DECLARE @campArctic     UNIQUEIDENTIFIER = NEWID();
DECLARE @campMuse       UNIQUEIDENTIFIER = NEWID();
DECLARE @campArcade     UNIQUEIDENTIFIER = NEWID();

-- Reward IDs (4 per campaign = 32 total)
DECLARE @rw_vet1 UNIQUEIDENTIFIER = NEWID(), @rw_vet2 UNIQUEIDENTIFIER = NEWID(), @rw_vet3 UNIQUEIDENTIFIER = NEWID(), @rw_vet4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_ros1 UNIQUEIDENTIFIER = NEWID(), @rw_ros2 UNIQUEIDENTIFIER = NEWID(), @rw_ros3 UNIQUEIDENTIFIER = NEWID(), @rw_ros4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_tan1 UNIQUEIDENTIFIER = NEWID(), @rw_tan2 UNIQUEIDENTIFIER = NEWID(), @rw_tan3 UNIQUEIDENTIFIER = NEWID(), @rw_tan4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_bad1 UNIQUEIDENTIFIER = NEWID(), @rw_bad2 UNIQUEIDENTIFIER = NEWID(), @rw_bad3 UNIQUEIDENTIFIER = NEWID(), @rw_bad4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_bil1 UNIQUEIDENTIFIER = NEWID(), @rw_bil2 UNIQUEIDENTIFIER = NEWID(), @rw_bil3 UNIQUEIDENTIFIER = NEWID(), @rw_bil4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_arc1 UNIQUEIDENTIFIER = NEWID(), @rw_arc2 UNIQUEIDENTIFIER = NEWID(), @rw_arc3 UNIQUEIDENTIFIER = NEWID(), @rw_arc4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_mus1 UNIQUEIDENTIFIER = NEWID(), @rw_mus2 UNIQUEIDENTIFIER = NEWID(), @rw_mus3 UNIQUEIDENTIFIER = NEWID(), @rw_mus4 UNIQUEIDENTIFIER = NEWID();
DECLARE @rw_acd1 UNIQUEIDENTIFIER = NEWID(), @rw_acd2 UNIQUEIDENTIFIER = NEWID(), @rw_acd3 UNIQUEIDENTIFIER = NEWID(), @rw_acd4 UNIQUEIDENTIFIER = NEWID();

-- Pedido (Order) IDs - 20 backings
DECLARE @ped01 UNIQUEIDENTIFIER = NEWID(), @ped02 UNIQUEIDENTIFIER = NEWID(), @ped03 UNIQUEIDENTIFIER = NEWID(), @ped04 UNIQUEIDENTIFIER = NEWID(), @ped05 UNIQUEIDENTIFIER = NEWID();
DECLARE @ped06 UNIQUEIDENTIFIER = NEWID(), @ped07 UNIQUEIDENTIFIER = NEWID(), @ped08 UNIQUEIDENTIFIER = NEWID(), @ped09 UNIQUEIDENTIFIER = NEWID(), @ped10 UNIQUEIDENTIFIER = NEWID();
DECLARE @ped11 UNIQUEIDENTIFIER = NEWID(), @ped12 UNIQUEIDENTIFIER = NEWID(), @ped13 UNIQUEIDENTIFIER = NEWID(), @ped14 UNIQUEIDENTIFIER = NEWID(), @ped15 UNIQUEIDENTIFIER = NEWID();
DECLARE @ped16 UNIQUEIDENTIFIER = NEWID(), @ped17 UNIQUEIDENTIFIER = NEWID(), @ped18 UNIQUEIDENTIFIER = NEWID(), @ped19 UNIQUEIDENTIFIER = NEWID(), @ped20 UNIQUEIDENTIFIER = NEWID();

-- Aportacion IDs
DECLARE @apo01 UNIQUEIDENTIFIER = NEWID(), @apo02 UNIQUEIDENTIFIER = NEWID(), @apo03 UNIQUEIDENTIFIER = NEWID(), @apo04 UNIQUEIDENTIFIER = NEWID(), @apo05 UNIQUEIDENTIFIER = NEWID();
DECLARE @apo06 UNIQUEIDENTIFIER = NEWID(), @apo07 UNIQUEIDENTIFIER = NEWID(), @apo08 UNIQUEIDENTIFIER = NEWID(), @apo09 UNIQUEIDENTIFIER = NEWID(), @apo10 UNIQUEIDENTIFIER = NEWID();
DECLARE @apo11 UNIQUEIDENTIFIER = NEWID(), @apo12 UNIQUEIDENTIFIER = NEWID(), @apo13 UNIQUEIDENTIFIER = NEWID(), @apo14 UNIQUEIDENTIFIER = NEWID(), @apo15 UNIQUEIDENTIFIER = NEWID();
DECLARE @apo16 UNIQUEIDENTIFIER = NEWID(), @apo17 UNIQUEIDENTIFIER = NEWID(), @apo18 UNIQUEIDENTIFIER = NEWID(), @apo19 UNIQUEIDENTIFIER = NEWID(), @apo20 UNIQUEIDENTIFIER = NEWID();

-- Pedido Linea IDs
DECLARE @lin01 UNIQUEIDENTIFIER = NEWID(), @lin02 UNIQUEIDENTIFIER = NEWID(), @lin03 UNIQUEIDENTIFIER = NEWID(), @lin04 UNIQUEIDENTIFIER = NEWID(), @lin05 UNIQUEIDENTIFIER = NEWID();
DECLARE @lin06 UNIQUEIDENTIFIER = NEWID(), @lin07 UNIQUEIDENTIFIER = NEWID(), @lin08 UNIQUEIDENTIFIER = NEWID(), @lin09 UNIQUEIDENTIFIER = NEWID(), @lin10 UNIQUEIDENTIFIER = NEWID();
DECLARE @lin11 UNIQUEIDENTIFIER = NEWID(), @lin12 UNIQUEIDENTIFIER = NEWID(), @lin13 UNIQUEIDENTIFIER = NEWID(), @lin14 UNIQUEIDENTIFIER = NEWID(), @lin15 UNIQUEIDENTIFIER = NEWID();
DECLARE @lin16 UNIQUEIDENTIFIER = NEWID(), @lin17 UNIQUEIDENTIFIER = NEWID(), @lin18 UNIQUEIDENTIFIER = NEWID(), @lin19 UNIQUEIDENTIFIER = NEWID(), @lin20 UNIQUEIDENTIFIER = NEWID();

-- ProyectoArtistico IDs (1 per artist = 8 total)
DECLARE @proy1 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy2 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy3 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy4 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy5 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy6 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy7 UNIQUEIDENTIFIER = NEWID();
DECLARE @proy8 UNIQUEIDENTIFIER = NEWID();

-- NecesidadCrowdsourcing IDs
DECLARE @nec1 UNIQUEIDENTIFIER = NEWID(), @nec2 UNIQUEIDENTIFIER = NEWID(), @nec3 UNIQUEIDENTIFIER = NEWID(), @nec4 UNIQUEIDENTIFIER = NEWID(), @nec5 UNIQUEIDENTIFIER = NEWID();
DECLARE @nec6 UNIQUEIDENTIFIER = NEWID(), @nec7 UNIQUEIDENTIFIER = NEWID();

-- PerfilProfesional IDs
DECLARE @perfCarlos UNIQUEIDENTIFIER = NEWID();
DECLARE @perfAna    UNIQUEIDENTIFIER = NEWID();
DECLARE @perfEmma   UNIQUEIDENTIFIER = NEWID();

-- PropuestaCrowdsourcing IDs
DECLARE @prop1 UNIQUEIDENTIFIER = NEWID(), @prop2 UNIQUEIDENTIFIER = NEWID(), @prop3 UNIQUEIDENTIFIER = NEWID();
DECLARE @prop4 UNIQUEIDENTIFIER = NEWID(), @prop5 UNIQUEIDENTIFIER = NEWID(), @prop6 UNIQUEIDENTIFIER = NEWID();

-- AcuerdoCrowdsourcing IDs
DECLARE @acuerdo1 UNIQUEIDENTIFIER = NEWID();
DECLARE @acuerdo2 UNIQUEIDENTIFIER = NEWID();

-- Milestone IDs
DECLARE @mile1 UNIQUEIDENTIFIER = NEWID(), @mile2 UNIQUEIDENTIFIER = NEWID(), @mile3 UNIQUEIDENTIFIER = NEWID();

-- Entregable IDs
DECLARE @entre1 UNIQUEIDENTIFIER = NEWID(), @entre2 UNIQUEIDENTIFIER = NEWID();

-- Conversacion IDs
DECLARE @conv1 UNIQUEIDENTIFIER = NEWID(), @conv2 UNIQUEIDENTIFIER = NEWID(), @conv3 UNIQUEIDENTIFIER = NEWID(), @conv4 UNIQUEIDENTIFIER = NEWID();

-- Mensaje IDs
DECLARE @msg01 UNIQUEIDENTIFIER = NEWID(), @msg02 UNIQUEIDENTIFIER = NEWID(), @msg03 UNIQUEIDENTIFIER = NEWID(), @msg04 UNIQUEIDENTIFIER = NEWID(), @msg05 UNIQUEIDENTIFIER = NEWID();
DECLARE @msg06 UNIQUEIDENTIFIER = NEWID(), @msg07 UNIQUEIDENTIFIER = NEWID(), @msg08 UNIQUEIDENTIFIER = NEWID(), @msg09 UNIQUEIDENTIFIER = NEWID(), @msg10 UNIQUEIDENTIFIER = NEWID();
DECLARE @msg11 UNIQUEIDENTIFIER = NEWID(), @msg12 UNIQUEIDENTIFIER = NEWID();

-- Valoracion IDs
DECLARE @val1 UNIQUEIDENTIFIER = NEWID(), @val2 UNIQUEIDENTIFIER = NEWID();

-- Promotor IDs
DECLARE @promCarlos UNIQUEIDENTIFIER = NEWID();
DECLARE @promAna    UNIQUEIDENTIFIER = NEWID();
DECLARE @promEmma   UNIQUEIDENTIFIER = NEWID();

-- PromoPrograma IDs
DECLARE @progVetusta  UNIQUEIDENTIFIER = NEWID();
DECLARE @progMuse     UNIQUEIDENTIFIER = NEWID();
DECLARE @progBadBunny UNIQUEIDENTIFIER = NEWID();
DECLARE @progBillie   UNIQUEIDENTIFIER = NEWID();

-- PromoProgramaPromotor IDs
DECLARE @ppp1 UNIQUEIDENTIFIER = NEWID(), @ppp2 UNIQUEIDENTIFIER = NEWID(), @ppp3 UNIQUEIDENTIFIER = NEWID();
DECLARE @ppp4 UNIQUEIDENTIFIER = NEWID(), @ppp5 UNIQUEIDENTIFIER = NEWID();
DECLARE @ppp6 UNIQUEIDENTIFIER = NEWID(), @ppp7 UNIQUEIDENTIFIER = NEWID();

-- PromoTarea IDs
DECLARE @ptarea1 UNIQUEIDENTIFIER = NEWID(), @ptarea2 UNIQUEIDENTIFIER = NEWID(), @ptarea3 UNIQUEIDENTIFIER = NEWID();
DECLARE @ptarea4 UNIQUEIDENTIFIER = NEWID(), @ptarea5 UNIQUEIDENTIFIER = NEWID(), @ptarea6 UNIQUEIDENTIFIER = NEWID();
DECLARE @ptarea7 UNIQUEIDENTIFIER = NEWID(), @ptarea8 UNIQUEIDENTIFIER = NEWID();

-- PromoTareaPromotor IDs
DECLARE @ptp1 UNIQUEIDENTIFIER = NEWID(), @ptp2 UNIQUEIDENTIFIER = NEWID(), @ptp3 UNIQUEIDENTIFIER = NEWID(), @ptp4 UNIQUEIDENTIFIER = NEWID();
DECLARE @ptp5 UNIQUEIDENTIFIER = NEWID(), @ptp6 UNIQUEIDENTIFIER = NEWID(), @ptp7 UNIQUEIDENTIFIER = NEWID(), @ptp8 UNIQUEIDENTIFIER = NEWID();

-- PromotorWallet IDs
DECLARE @walCarlos UNIQUEIDENTIFIER = NEWID();
DECLARE @walAna    UNIQUEIDENTIFIER = NEWID();
DECLARE @walEmma   UNIQUEIDENTIFIER = NEWID();

-- PromotorWalletTransaccion IDs
DECLARE @wtx1 UNIQUEIDENTIFIER = NEWID(), @wtx2 UNIQUEIDENTIFIER = NEWID(), @wtx3 UNIQUEIDENTIFIER = NEWID();
DECLARE @wtx4 UNIQUEIDENTIFIER = NEWID(), @wtx5 UNIQUEIDENTIFIER = NEWID(), @wtx6 UNIQUEIDENTIFIER = NEWID();

-- PromoEvento IDs
DECLARE @pevt01 UNIQUEIDENTIFIER = NEWID(), @pevt02 UNIQUEIDENTIFIER = NEWID(), @pevt03 UNIQUEIDENTIFIER = NEWID(), @pevt04 UNIQUEIDENTIFIER = NEWID(), @pevt05 UNIQUEIDENTIFIER = NEWID();
DECLARE @pevt06 UNIQUEIDENTIFIER = NEWID(), @pevt07 UNIQUEIDENTIFIER = NEWID(), @pevt08 UNIQUEIDENTIFIER = NEWID(), @pevt09 UNIQUEIDENTIFIER = NEWID(), @pevt10 UNIQUEIDENTIFIER = NEWID();

-- Role IDs (existing, DO NOT INSERT)
DECLARE @roleAdmin    NVARCHAR(450) = N'33b3cefd-1be5-4f36-95b8-3864a768d08a';
DECLARE @roleFan      NVARCHAR(450) = N'a9a8e79e-f7df-4105-96b7-3312b663f2ac';
DECLARE @roleArtista  NVARCHAR(450) = N'bd089a0d-033e-41d3-ba0a-28e7f13cd323';

-- Password hashes
DECLARE @pwArtist NVARCHAR(MAX) = N'AQAAAAIAAYagAAAAEOBTp8pnDsFmlQW036pEQ3XIVqsB5n1kQlGaBv9Uof2IvbcjeZUxWksGlb57zjcN0w==';
DECLARE @pwFan    NVARCHAR(MAX) = N'AQAAAAIAAYagAAAAEJlRV/UPESx6LyFEyOx3wIVVUZNGYA4uiL0UrTjsTDQaNhOi2EMZQqAV52cNPHQPsw==';

-- Dates
DECLARE @now         DATETIME2(3) = GETUTCDATE();
DECLARE @twoWeeksAgo DATETIME2(3) = DATEADD(DAY, -14, GETUTCDATE());
DECLARE @oneWeekAgo  DATETIME2(3) = DATEADD(DAY, -7, GETUTCDATE());
DECLARE @threeDAgo   DATETIME2(3) = DATEADD(DAY, -3, GETUTCDATE());
DECLARE @fiveDaysAgo DATETIME2(3) = DATEADD(DAY, -5, GETUTCDATE());
DECLARE @tenDaysAgo  DATETIME2(3) = DATEADD(DAY, -10, GETUTCDATE());
DECLARE @in30Days    DATETIME2(3) = DATEADD(DAY, 30, GETUTCDATE());
DECLARE @in45Days    DATETIME2(3) = DATEADD(DAY, 45, GETUTCDATE());
DECLARE @in60Days    DATETIME2(3) = DATEADD(DAY, 60, GETUTCDATE());
DECLARE @monthAgo    DATETIME2(3) = DATEADD(DAY, -30, GETUTCDATE());

-- ============================================================================
-- SECTION 4: ASPNET USERS
-- ============================================================================

-- Artist users (password: WePlay2026!)
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount) VALUES
(@userVetusta,  N'vetusta@weplay-test.com',     N'VETUSTA@WEPLAY-TEST.COM',     N'vetusta@weplay-test.com',     N'VETUSTA@WEPLAY-TEST.COM',     1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD4444', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userRosalia,  N'rosalia@weplay-test.com',     N'ROSALIA@WEPLAY-TEST.COM',     N'rosalia@weplay-test.com',     N'ROSALIA@WEPLAY-TEST.COM',     1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD5555', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userTangana,  N'tangana@weplay-test.com',     N'TANGANA@WEPLAY-TEST.COM',     N'tangana@weplay-test.com',     N'TANGANA@WEPLAY-TEST.COM',     1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD6666', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userBadBunny, N'badbunny@weplay-test.com',    N'BADBUNNY@WEPLAY-TEST.COM',    N'badbunny@weplay-test.com',    N'BADBUNNY@WEPLAY-TEST.COM',    1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD7777', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userBillie,   N'billie@weplay-test.com',      N'BILLIE@WEPLAY-TEST.COM',      N'billie@weplay-test.com',      N'BILLIE@WEPLAY-TEST.COM',      1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD8888', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userArctic,   N'arctic@weplay-test.com',      N'ARCTIC@WEPLAY-TEST.COM',      N'arctic@weplay-test.com',      N'ARCTIC@WEPLAY-TEST.COM',      1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDD9999', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userMuse,     N'muse@weplay-test.com',        N'MUSE@WEPLAY-TEST.COM',        N'muse@weplay-test.com',        N'MUSE@WEPLAY-TEST.COM',        1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDDAAAA', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userArcade,   N'arcade@weplay-test.com',      N'ARCADE@WEPLAY-TEST.COM',      N'arcade@weplay-test.com',      N'ARCADE@WEPLAY-TEST.COM',      1, @pwArtist, N'AAAA1111BBBB2222CCCC3333DDDDBBBB', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0);

-- Fan users (password: Test123!)
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount) VALUES
(@userMaria,    N'maria.garcia@weplay-test.com',    N'MARIA.GARCIA@WEPLAY-TEST.COM',    N'maria.garcia@weplay-test.com',    N'MARIA.GARCIA@WEPLAY-TEST.COM',    1, @pwFan, N'FFFF1111AAAA2222BBBB3333CCCC4444', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userCarlos,   N'carlos.lopez@weplay-test.com',    N'CARLOS.LOPEZ@WEPLAY-TEST.COM',    N'carlos.lopez@weplay-test.com',    N'CARLOS.LOPEZ@WEPLAY-TEST.COM',    1, @pwFan, N'FFFF1111AAAA2222BBBB3333CCCC5555', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userAna,      N'ana.martinez@weplay-test.com',    N'ANA.MARTINEZ@WEPLAY-TEST.COM',    N'ana.martinez@weplay-test.com',    N'ANA.MARTINEZ@WEPLAY-TEST.COM',    1, @pwFan, N'FFFF1111AAAA2222BBBB3333CCCC6666', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userPablo,    N'pablo.ruiz@weplay-test.com',      N'PABLO.RUIZ@WEPLAY-TEST.COM',      N'pablo.ruiz@weplay-test.com',      N'PABLO.RUIZ@WEPLAY-TEST.COM',      1, @pwFan, N'FFFF1111AAAA2222BBBB3333CCCC7777', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0),
(@userEmma,     N'emma.wilson@weplay-test.com',     N'EMMA.WILSON@WEPLAY-TEST.COM',     N'emma.wilson@weplay-test.com',     N'EMMA.WILSON@WEPLAY-TEST.COM',     1, @pwFan, N'FFFF1111AAAA2222BBBB3333CCCC8888', LOWER(NEWID()), NULL, 0, 0, NULL, 1, 0);

-- ============================================================================
-- SECTION 5: USER ROLES
-- ============================================================================

-- Artists get Artista + Fan roles
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
(@userVetusta,  @roleArtista), (@userVetusta,  @roleFan),
(@userRosalia,  @roleArtista), (@userRosalia,  @roleFan),
(@userTangana,  @roleArtista), (@userTangana,  @roleFan),
(@userBadBunny, @roleArtista), (@userBadBunny, @roleFan),
(@userBillie,   @roleArtista), (@userBillie,   @roleFan),
(@userArctic,   @roleArtista), (@userArctic,   @roleFan),
(@userMuse,     @roleArtista), (@userMuse,     @roleFan),
(@userArcade,   @roleArtista), (@userArcade,   @roleFan);

-- Fans get Fan role
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
(@userMaria,  @roleFan),
(@userCarlos, @roleFan),
(@userAna,    @roleFan),
(@userPablo,  @roleFan),
(@userEmma,   @roleFan);

-- First artist also has Admin role (for demo)
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
(@userVetusta, @roleAdmin);

-- ============================================================================
-- SECTION 6: ARTISTAS
-- ============================================================================

INSERT INTO Artista (Id, UserIdPropietario, NombreArtistico, Descripcion, Pais, Ciudad, UrlSitioWeb, UrlInstagram, UrlYouTube, UrlSpotify, FechaCreacion, FechaActualizacion, ImagenPerfilUrl) VALUES
(@artVetusta, @userVetusta, N'Vetusta Morla',
 N'Banda de indie rock formada en Madrid en 2000. Conocidos por su sonido epico y letras poeticas, Vetusta Morla se ha convertido en una de las bandas mas influyentes del rock en espanol. Con discos como "Un Dia en el Mundo" y "La Deriva", han llenado estadios en toda Espana y Latinoamerica.',
 N'Espana', N'Madrid',
 N'https://www.vetustamorla.com', N'https://instagram.com/vetustamorla', N'https://youtube.com/@VetustaMorla', N'https://open.spotify.com/artist/4A0bZ6oMjCPcMHhEN7bMJF',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/167636/pexels-photo-167636.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artRosalia, @userRosalia, N'Rosalia',
 N'Rosalia Vila Tobella, conocida como ROSALIA, es una cantante y compositora espanola que ha revolucionado la musica fusionando flamenco con pop, electronica y R&B. Ganadora de multiples premios Grammy y Latin Grammy, su album "MOTOMAMI" fue aclamado mundialmente.',
 N'Espana', N'Barcelona',
 N'https://www.rosalia.com', N'https://instagram.com/rosalia.vt', N'https://youtube.com/@Rosalia', N'https://open.spotify.com/artist/7ltDVBr6mKbRvohxheJ9h1',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/3771836/pexels-photo-3771836.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artTangana, @userTangana, N'C. Tangana',
 N'Antono Jose Puerta Sanchez, mas conocido como C. Tangana, es un artista espanol que mezcla trap, flamenco, rumba y pop. Su album "El Madrileno" (2021) fue un fenomeno cultural que revitalizo la musica tradicional espanola con una perspectiva contemporanea.',
 N'Espana', N'Madrid',
 N'https://www.ctangana.com', N'https://instagram.com/c.tangana', N'https://youtube.com/@CTangana', N'https://open.spotify.com/artist/2cSz3cjRTCosMqSkM8WJRO',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/1699161/pexels-photo-1699161.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artBadBunny, @userBadBunny, N'Bad Bunny',
 N'Benito Antonio Martinez Ocasio, conocido como Bad Bunny, es un cantante puertorriqueno que ha transformado el reggaeton y la musica latina. Con multiples albums numero uno y records de streaming en Spotify, es uno de los artistas mas escuchados del mundo.',
 N'Puerto Rico', N'San Juan',
 N'https://www.badbunny.com', N'https://instagram.com/badbunnypr', N'https://youtube.com/@BadBunny', N'https://open.spotify.com/artist/4q3ewBCX7sLwd24euuV69X',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/1047442/pexels-photo-1047442.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artBillie, @userBillie, N'Billie Eilish',
 N'Billie Eilish Pirate Baird O''Connell es una cantante y compositora estadounidense que redefinio el pop con su estetica oscura y produccion minimalista. A los 18 anos barrio los Grammy con su debut "When We All Fall Asleep, Where Do We Go?" y continua siendo una de las artistas mas influyentes de su generacion.',
 N'Estados Unidos', N'Los Angeles',
 N'https://www.billieeilish.com', N'https://instagram.com/billieeilish', N'https://youtube.com/@BillieEilish', N'https://open.spotify.com/artist/6qqNVTkY8uBg9cP3Jd7DAH',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/3756766/pexels-photo-3756766.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artArctic, @userArctic, N'Arctic Monkeys',
 N'Arctic Monkeys es una banda de rock formada en Sheffield, Inglaterra, en 2002. Liderados por Alex Turner, han evolucionado desde el indie rock energico de su debut hasta el sonido sofisticado de "Tranquility Base Hotel & Casino" y "The Car". Son una de las bandas britanicas mas importantes del siglo XXI.',
 N'Reino Unido', N'Sheffield',
 N'https://www.arcticmonkeys.com', N'https://instagram.com/arcticmonkeys', N'https://youtube.com/@ArcticMonkeys', N'https://open.spotify.com/artist/7Ln80lUS6He07XvHI8qqHH',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/995301/pexels-photo-995301.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artMuse, @userMuse, N'Muse',
 N'Muse es una banda britanica de rock alternativo formada en Teignmouth, Devon, en 1994. Conocidos por sus espectaculares shows en vivo y su fusion de rock progresivo, electronica y musica clasica, Matthew Bellamy, Chris Wolstenholme y Dominic Howard han vendido mas de 30 millones de albums.',
 N'Reino Unido', N'Teignmouth',
 N'https://www.muse.mu', N'https://instagram.com/maboringassnamethat', N'https://youtube.com/@maboringassnamethat', N'https://open.spotify.com/artist/12Chz98pHFMPJEknJQMWvI',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/1763075/pexels-photo-1763075.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop'),

(@artArcade, @userArcade, N'Arcade Fire',
 N'Arcade Fire es una banda canadiense de indie rock formada en Montreal en 2001 por Win Butler y Regine Chassagne. Su musica combina rock orquestal con influencias del art rock y la musica mundial. Su album "The Suburbs" gano el Grammy a Album del Ano en 2011.',
 N'Canada', N'Montreal',
 N'https://www.arcadefire.com', N'https://instagram.com/arcadefire', N'https://youtube.com/@ArcadeFire', N'https://open.spotify.com/artist/3kjuyTCjPG1WMFCiyc5IuB',
 @monthAgo, @now,
 N'https://images.pexels.com/photos/1540406/pexels-photo-1540406.jpeg?auto=compress&cs=tinysrgb&w=400&h=400&fit=crop');

-- ============================================================================
-- SECTION 7: FAN PROFILES
-- ============================================================================

INSERT INTO FanProfile (Id, UserId, Apodo, Pais, Ciudad, FechaCreacion) VALUES
(@fanMaria,  @userMaria,  N'MariaG',        N'Espana',       N'Sevilla',      @monthAgo),
(@fanCarlos, @userCarlos, N'CarlosMusic',    N'Mexico',       N'Ciudad de Mexico', @monthAgo),
(@fanAna,    @userAna,    N'AnaCreativa',    N'Argentina',    N'Buenos Aires', @monthAgo),
(@fanPablo,  @userPablo,  N'PabloRock',      N'Espana',       N'Valencia',     @monthAgo),
(@fanEmma,   @userEmma,   N'EmmaSound',      N'Reino Unido',  N'Londres',      @monthAgo);

-- ============================================================================
-- SECTION 8: CAMPAIGNS (8, all PUBLICADA = EstadoCampania_Id 2 to match frontend constants)
-- ============================================================================

-- Campaign amounts will be set after backings to match actual pledged amounts
-- We'll define ImportePledgedActual after computing backing totals

INSERT INTO CampaniaCrowdfunding (Id, Artista_Id, ProyectoArtistico_Id, Titulo, Subtitulo, DescripcionCorta, VideoPrincipalUrl, ImagenPrincipalUrl, Moneda_Id, ImporteObjetivo, ImporteMinimo, ImportePledgedActual, TipoFinanciacion_Id, EstadoCampania_Id, PermiteAportacionesAnonimas, PermitePropinas, PorcentajeComisionPlataforma, FechaInicio, FechaFin, FechaPublicacion, FechaCierre, Borrado, FechaCreacion, FechaActualizacion) VALUES
-- 1. Vetusta Morla: nuevo album
(@campVetusta, @artVetusta, @proy1,
 N'Vetusta Morla - Nuevo Album de Estudio',
 N'Ayudanos a grabar nuestro septimo disco con produccion independiente',
 N'Despues de dos anos de composicion, estamos listos para entrar al estudio. Este album sera nuestro proyecto mas ambicioso: 14 canciones grabadas en analogico con una orquesta de 30 musicos. Queremos hacerlo de forma independiente y para eso necesitamos vuestra ayuda.',
 N'https://www.youtube.com/watch?v=EXAMPLE_VM', N'https://images.pexels.com/photos/164938/pexels-photo-164938.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 1, 50000.00, 25000.00, 0, 1, 2, 1, 1, 5.00, @twoWeeksAgo, @in60Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 2. Rosalia: videoclip
(@campRosalia, @artRosalia, @proy2,
 N'ROSALIA - Videoclip "Abismo" Interactivo',
 N'Un videoclip revolucionario con realidad aumentada y flamenco contemporaneo',
 N'Quiero crear algo que nunca se ha hecho: un videoclip interactivo donde el espectador pueda elegir diferentes caminos narrativos usando AR. El tema "Abismo" fusiona cante jondo con produccion electronica y la visual tiene que estar a la altura.',
 N'https://www.youtube.com/watch?v=EXAMPLE_ROS', N'https://images.pexels.com/photos/3062541/pexels-photo-3062541.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 1, 75000.00, 40000.00, 0, 1, 3, 0, 1, 5.00, @twoWeeksAgo, @in60Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 3. C. Tangana: gira acustica
(@campTangana, @artTangana, @proy4,
 N'C. Tangana - Gira Acustica "Sin Cantar ni Afinar"',
 N'10 ciudades, formato intimo, solo voz y guitarra espanola',
 N'Quiero volver a lo esencial. Una gira por salas pequenas de 300 personas donde tocare versiones acusticas de "El Madrileno" y temas nuevos. Sin autotune, sin produccion, solo la verdad de la musica. Los fondos cubren alquiler de salas y logistica.',
 N'https://www.youtube.com/watch?v=EXAMPLE_CT', N'https://images.pexels.com/photos/1407322/pexels-photo-1407322.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 1, 35000.00, 15000.00, 0, 2, 2, 1, 1, 5.00, @twoWeeksAgo, @in45Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 4. Bad Bunny: documental
(@campBadBunny, @artBadBunny, @proy5,
 N'Bad Bunny - Documental "De Vega Baja al Mundo"',
 N'La historia detras del fenomeno contada por quienes estuvieron ahi desde el principio',
 N'Un documental que cuenta mi historia desde Almirante Sur en Vega Baja hasta los estadios mas grandes del mundo. Entrevistas con mi familia, amigos de infancia y colaboradores. Dirigido por un cineasta puertorriqueno. Todo el material sera subtitulado en 5 idiomas.',
 N'https://www.youtube.com/watch?v=EXAMPLE_BB', N'https://images.pexels.com/photos/66134/pexels-photo-66134.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, 60000.00, 30000.00, 0, 1, 3, 0, 1, 5.00, @twoWeeksAgo, @in60Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 5. Billie Eilish: album ecologico
(@campBillie, @artBillie, @proy6,
 N'Billie Eilish - "Echoes" Eco-Friendly Album Production',
 N'El primer album con huella de carbono cero en la historia de la musica',
 N'I want to prove that you can make a world-class album while being completely carbon neutral. Solar-powered studio, recycled vinyl, digital-first distribution. Every backing helps offset the remaining carbon footprint and funds innovative green recording tech.',
 N'https://www.youtube.com/watch?v=EXAMPLE_BE', N'https://images.pexels.com/photos/1105666/pexels-photo-1105666.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, 45000.00, 20000.00, 0, 1, 2, 1, 1, 5.00, @twoWeeksAgo, @in45Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 6. Arctic Monkeys: EP experimental
(@campArctic, @artArctic, @proy3,
 N'Arctic Monkeys - "Lunar Surface" EP',
 N'Un EP experimental grabado en un estudio subterraneo de Sheffield',
 N'We want to go back to our roots in Sheffield and record a 6-track EP in the basement studio where it all started. Raw, unpolished, no overdubs. The EP will be released on limited edition vinyl with artwork by local Sheffield artists. All profits support local music venues.',
 N'https://www.youtube.com/watch?v=EXAMPLE_AM', N'https://images.pexels.com/photos/1763075/pexels-photo-1763075.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 3, 30000.00, 15000.00, 0, 1, 2, 1, 1, 5.00, @twoWeeksAgo, @in45Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 7. Muse: concierto sinfonica
(@campMuse, @artMuse, @proy7,
 N'Muse - Symphonic Experience at Royal Albert Hall',
 N'Nuestros mayores exitos reimaginados con la London Symphony Orchestra',
 N'A one-night-only event: Muse classics performed with a 60-piece orchestra at the Royal Albert Hall. "Hysteria", "Starlight", "Uprising" and more, arranged by a team of classical composers. The concert will be filmed in 4K and released as a live album. Help make this dream a reality.',
 N'https://www.youtube.com/watch?v=EXAMPLE_MUSE', N'https://images.pexels.com/photos/210922/pexels-photo-210922.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 3, 70000.00, 35000.00, 0, 1, 3, 0, 1, 5.00, @twoWeeksAgo, @in60Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now),

-- 8. Arcade Fire: album comunitario
(@campArcade, @artArcade, @proy8,
 N'Arcade Fire - "Neighborhoods Vol. 2" Community Album',
 N'Un album donde los fans eligen las canciones y colaboran en la produccion',
 N'We have recorded 25 demos and we want YOU to decide which 12 make it to the final album. Backers get access to all demos, can vote on the tracklist, and the top-tier backers can even contribute vocals or instruments to bonus tracks. This is community-driven music at its best.',
 N'https://www.youtube.com/watch?v=EXAMPLE_AF', N'https://images.pexels.com/photos/1540406/pexels-photo-1540406.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, 40000.00, 20000.00, 0, 2, 2, 1, 1, 5.00, @twoWeeksAgo, @in45Days, @twoWeeksAgo, NULL, 0, @twoWeeksAgo, @now);

-- ============================================================================
-- SECTION 9: REWARDS (4 per campaign)
-- ============================================================================

-- TipoReward: 1=Fisico, 2=Digital, 3=Experiencia, 4=Sin Recompensa

-- Vetusta Morla rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_vet1, @campVetusta, 4, N'Apoyo Basico',            N'Tu nombre en los creditos digitales del album y acceso al diario de grabacion', 10.00, 1, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_vet2, @campVetusta, 2, N'Album Digital Firmado',    N'Descarga digital del album en alta resolucion (FLAC) + booklet digital firmado + 3 temas extra exclusivos', 25.00, 1, 0, NULL, 1, 0, N'Al lanzamiento del album', 2, 1, @twoWeeksAgo, NULL),
(@rw_vet3, @campVetusta, 1, N'Vinilo Edicion Coleccionista', N'Doble vinilo 180g en edicion limitada numerada + poster A2 + descarga digital + stickers', 50.00, 1, 0, 500, 1, 1, N'3-4 meses tras grabacion', 3, 1, @twoWeeksAgo, NULL),
(@rw_vet4, @campVetusta, 3, N'Sesion de Estudio VIP',   N'Asiste a una sesion de grabacion en el estudio + meet & greet + vinilo firmado por toda la banda', 200.00, 1, 0, 20, 1, 0, N'Durante la grabacion', 4, 1, @twoWeeksAgo, NULL);

-- Rosalia rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_ros1, @campRosalia, 4, N'Fan Supporter',            N'Acceso anticipado al videoclip 48h antes del lanzamiento publico + agradecimiento en redes', 15.00, 1, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_ros2, @campRosalia, 2, N'Digital Experience Pack',   N'Filtro AR exclusivo para Instagram + making-of completo + 3 wallpapers artisticos del videoclip', 35.00, 1, 0, NULL, 1, 0, N'Al lanzamiento', 2, 1, @twoWeeksAgo, NULL),
(@rw_ros3, @campRosalia, 1, N'Art Print Collection',      N'Set de 5 art prints en papel fotografico de los frames mas iconicos del videoclip + caja coleccionista', 75.00, 1, 0, 200, 1, 1, N'2 meses tras lanzamiento', 3, 1, @twoWeeksAgo, NULL),
(@rw_ros4, @campRosalia, 3, N'Set Visit + Cena con ROSALIA', N'Visita al set de grabacion del videoclip + cena privada con ROSALIA y el equipo creativo', 500.00, 1, 0, 5, 1, 0, N'Durante rodaje', 4, 1, @twoWeeksAgo, NULL);

-- C. Tangana rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_tan1, @campTangana, 4, N'Madrileno de Corazon',     N'Acceso a playlist curada personal de C. Tangana + nombre en web de la gira', 10.00, 1, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_tan2, @campTangana, 2, N'Live Recording Digital',    N'Grabacion en vivo de la primera noche de la gira en audio HD + setlist exclusivo', 30.00, 1, 0, NULL, 1, 0, N'1 mes tras primera fecha', 2, 1, @twoWeeksAgo, NULL),
(@rw_tan3, @campTangana, 1, N'Camiseta Gira Exclusiva',  N'Camiseta edicion limitada disenada por el propio Tangana + entrada preferente a una fecha', 60.00, 1, 0, 300, 1, 1, N'Antes de la gira', 3, 1, @twoWeeksAgo, NULL),
(@rw_tan4, @campTangana, 3, N'Cena Post-Concierto',      N'Cena privada con C. Tangana despues de un concierto + foto firmada', 250.00, 1, 0, 10, 1, 0, N'Durante la gira', 4, 1, @twoWeeksAgo, NULL);

-- Bad Bunny rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_bad1, @campBadBunny, 4, N'Boricua Pride',            N'Tu nombre en los creditos del documental + wallpaper exclusivo', 15.00, 2, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_bad2, @campBadBunny, 2, N'Digital Premiere Access',   N'Acceso al estreno digital privado 1 semana antes + behind-the-scenes extras', 40.00, 2, 0, NULL, 1, 0, N'Al estreno', 2, 1, @twoWeeksAgo, NULL),
(@rw_bad3, @campBadBunny, 1, N'Collector Box',             N'Blu-ray del documental + libro fotografico de 100 paginas + gorra edicion limitada', 80.00, 2, 0, 500, 1, 1, N'3 meses tras estreno', 3, 1, @twoWeeksAgo, NULL),
(@rw_bad4, @campBadBunny, 3, N'Premiere VIP + Meet',       N'Asiento VIP en el estreno presencial en San Juan + meet & greet + afterparty', 300.00, 2, 0, 15, 1, 0, N'En el estreno', 4, 1, @twoWeeksAgo, NULL);

-- Billie Eilish rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_bil1, @campBillie, 4, N'Eco Supporter',              N'Digital thank you card + carbon offset certificate with your name', 10.00, 2, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_bil2, @campBillie, 2, N'Echoes Digital Bundle',       N'Album download (lossless) + exclusive acoustic demos + monthly studio diary videos', 30.00, 2, 0, NULL, 1, 0, N'At album release', 2, 1, @twoWeeksAgo, NULL),
(@rw_bil3, @campBillie, 1, N'Recycled Vinyl Special',      N'Album on 100% recycled vinyl in eco-packaging + seed paper booklet that grows wildflowers', 55.00, 2, 0, 1000, 1, 1, N'2 months after release', 3, 1, @twoWeeksAgo, NULL),
(@rw_bil4, @campBillie, 3, N'Green Studio Session',        N'Visit the solar-powered studio + private acoustic performance + signed vinyl', 350.00, 2, 0, 8, 1, 0, N'During recording', 4, 1, @twoWeeksAgo, NULL);

-- Arctic Monkeys rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_arc1, @campArctic, 4, N'Sheffield Soul',              N'Name on the EP liner notes + exclusive playlist by Alex Turner', 10.00, 3, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_arc2, @campArctic, 2, N'Demo Vault Access',           N'All 6 EP tracks in lossless + 4 unreleased demos from the basement sessions', 25.00, 3, 0, NULL, 1, 0, N'At release', 2, 1, @twoWeeksAgo, NULL),
(@rw_arc3, @campArctic, 1, N'Sheffield Limited Vinyl',     N'Colored vinyl EP (300 copies worldwide) + hand-numbered Sheffield artist prints', 45.00, 3, 0, 300, 1, 1, N'3 months after recording', 3, 1, @twoWeeksAgo, NULL),
(@rw_arc4, @campArctic, 3, N'Basement Gig Experience',     N'Attend the intimate basement recording session + pint at the local pub with the band', 400.00, 3, 0, 10, 1, 0, N'During recording', 4, 1, @twoWeeksAgo, NULL);

-- Muse rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_mus1, @campMuse, 4, N'Resistance Supporter',         N'Digital program + name in the concert film credits', 15.00, 3, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_mus2, @campMuse, 2, N'Symphonic Live Album',         N'Full concert recording in Dolby Atmos + 4K concert film digital access + rehearsal footage', 40.00, 3, 0, NULL, 1, 0, N'1 month after concert', 2, 1, @twoWeeksAgo, NULL),
(@rw_mus3, @campMuse, 1, N'Orchestral Score Box',         N'Triple vinyl live album + full orchestral score book + concert poster signed by the band', 85.00, 3, 0, 500, 1, 1, N'3 months after concert', 3, 1, @twoWeeksAgo, NULL),
(@rw_mus4, @campMuse, 3, N'Royal Albert Hall VIP',        N'Front row seat at the concert + backstage pass + dinner with the band + signed guitar', 750.00, 3, 0, 5, 1, 0, N'Concert night', 4, 1, @twoWeeksAgo, NULL);

-- Arcade Fire rewards
INSERT INTO CampaniaCrowdfunding_Reward (Id, Campania_Id, TipoReward_Id, Nombre, Descripcion, ImporteMinimo, Moneda_Id, EsAddOn, CantidadMaxima, CantidadPorBacker, IncluyeEnvioFisico, TiempoEntregaEstimado, Orden, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@rw_acd1, @campArcade, 4, N'Neighborhood Citizen',       N'Vote on the tracklist + access to all 25 demos + digital liner notes', 10.00, 2, 0, NULL, 1, 0, NULL, 1, 1, @twoWeeksAgo, NULL),
(@rw_acd2, @campArcade, 2, N'Community Producer',         N'All 25 demos + final album + exclusive commentary track by Win Butler on each song', 30.00, 2, 0, NULL, 1, 0, N'At album release', 2, 1, @twoWeeksAgo, NULL),
(@rw_acd3, @campArcade, 1, N'Vinyl + Zine Bundle',        N'Gatefold vinyl + community zine featuring fan-submitted artwork + enamel pin set', 60.00, 2, 0, 400, 1, 1, N'2 months after release', 3, 1, @twoWeeksAgo, NULL),
(@rw_acd4, @campArcade, 3, N'Record a Bonus Track',       N'Contribute vocals/instruments to a bonus track + studio visit + album credit as collaborator', 500.00, 2, 0, 5, 1, 0, N'During recording', 4, 1, @twoWeeksAgo, NULL);

-- ============================================================================
-- SECTION 10: BACKINGS (20 total = Pedido + Linea + Aportacion)
-- ============================================================================

-- Backing plan: distribute 20 backings across 8 campaigns so each has 2-3 backings
-- Campaign pledged totals (must match sum of backing ImporteTotal):
-- Vetusta:  ped01(25) + ped02(50) + ped03(200) = 275
-- Rosalia:  ped04(35) + ped05(75) = 110
-- Tangana:  ped06(30) + ped07(60) + ped08(250) = 340
-- BadBunny: ped09(40) + ped10(80) = 120
-- Billie:   ped11(30) + ped12(55) + ped13(350) = 435
-- Arctic:   ped14(25) + ped15(45) = 70
-- Muse:     ped16(40) + ped17(85) + ped18(750) = 875
-- Arcade:   ped19(30) + ped20(60) = 90

-- PEDIDOS
INSERT INTO PedidoCrowdfunding (Id, Campania_Id, UserId, FanProfile_Id, EstadoPedido_Id, Moneda_Id, ImporteSubtotal, ImportePropina, ImporteEnvio, ImporteImpuestos, ImporteTotal, PermitirMostrarNombre, ComentarioBacker, DireccionEnvio_Id, FechaCreacion, FechaActualizacion) VALUES
-- Vetusta (EUR)
(@ped01, @campVetusta, @userMaria,  @fanMaria,  2, 1, 25.00,  0.00, 0.00, 0.00, 25.00,  1, N'Mucho animo con el nuevo disco!',           NULL, @tenDaysAgo, @tenDaysAgo),
(@ped02, @campVetusta, @userCarlos, @fanCarlos, 2, 1, 50.00,  5.00, 5.00, 0.00, 60.00,  1, N'Desde Mexico con amor! Quiero ese vinilo',  NULL, @oneWeekAgo, @oneWeekAgo),
(@ped03, @campVetusta, @userPablo,  @fanPablo,  2, 1, 200.00, 0.00, 0.00, 0.00, 200.00, 1, N'Soy fan desde el primer disco. Nos vemos en el estudio!', NULL, @fiveDaysAgo, @fiveDaysAgo),

-- Rosalia (EUR)
(@ped04, @campRosalia, @userAna,    @fanAna,    2, 1, 35.00,  0.00, 0.00, 0.00, 35.00,  1, N'MOTOMAMI fan forever! El videoclip va a ser increible', NULL, @tenDaysAgo, @tenDaysAgo),
(@ped05, @campRosalia, @userEmma,   @fanEmma,   2, 1, 75.00,  0.00, 8.00, 0.00, 83.00,  1, N'Love the AR concept! Shipping to UK please', NULL, @oneWeekAgo, @oneWeekAgo),

-- C. Tangana (EUR)
(@ped06, @campTangana, @userMaria,  @fanMaria,  2, 1, 30.00,  0.00, 0.00, 0.00, 30.00,  1, N'Tangana acustico va a ser una locura',      NULL, @tenDaysAgo, @tenDaysAgo),
(@ped07, @campTangana, @userCarlos, @fanCarlos, 2, 1, 60.00,  0.00, 10.00, 0.00, 70.00, 1, N'Envio a Mexico por favor! Necesito esa camiseta', NULL, @oneWeekAgo, @oneWeekAgo),
(@ped08, @campTangana, @userPablo,  @fanPablo,  2, 1, 250.00, 10.00, 0.00, 0.00, 260.00, 1, N'La cena post-concierto es un sueno', NULL, @fiveDaysAgo, @fiveDaysAgo),

-- Bad Bunny (USD)
(@ped09, @campBadBunny, @userCarlos, @fanCarlos, 2, 2, 40.00,  0.00, 0.00, 0.00, 40.00,  1, N'YHLQMDLG cambio mi vida! Quiero ver el documental', NULL, @tenDaysAgo, @tenDaysAgo),
(@ped10, @campBadBunny, @userAna,    @fanAna,    2, 2, 80.00,  5.00, 12.00, 0.00, 97.00, 1, N'Bad Bunny representa a toda Latinoamerica', NULL, @oneWeekAgo, @oneWeekAgo),

-- Billie (USD)
(@ped11, @campBillie, @userEmma,   @fanEmma,   2, 2, 30.00,  0.00, 0.00, 0.00, 30.00,  1, N'So important to support eco-friendly music!', NULL, @tenDaysAgo, @tenDaysAgo),
(@ped12, @campBillie, @userMaria,  @fanMaria,  2, 2, 55.00,  0.00, 10.00, 0.00, 65.00, 1, N'The seed paper booklet concept is amazing',   NULL, @oneWeekAgo, @oneWeekAgo),
(@ped13, @campBillie, @userPablo,  @fanPablo,  2, 2, 350.00, 15.00, 0.00, 0.00, 365.00, 1, N'Visiting a solar studio is a dream come true', NULL, @fiveDaysAgo, @fiveDaysAgo),

-- Arctic (GBP)
(@ped14, @campArctic, @userEmma,   @fanEmma,   2, 3, 25.00,  0.00, 0.00, 0.00, 25.00,  1, N'Sheffield band recording in Sheffield - perfect!', NULL, @tenDaysAgo, @tenDaysAgo),
(@ped15, @campArctic, @userPablo,  @fanPablo,  2, 3, 45.00,  0.00, 8.00, 0.00, 53.00,  1, N'I need that colored vinyl! Ship to Spain please', NULL, @oneWeekAgo, @oneWeekAgo),

-- Muse (GBP)
(@ped16, @campMuse, @userEmma,   @fanEmma,   2, 3, 40.00,  0.00, 0.00, 0.00, 40.00,   1, N'Muse with an orchestra will be epic!',        NULL, @tenDaysAgo, @tenDaysAgo),
(@ped17, @campMuse, @userCarlos, @fanCarlos, 2, 3, 85.00,  0.00, 15.00, 0.00, 100.00,  1, N'Need the orchestral score book!',            NULL, @oneWeekAgo, @oneWeekAgo),
(@ped18, @campMuse, @userAna,    @fanAna,    2, 3, 750.00, 50.00, 0.00, 0.00, 800.00,  1, N'Front row + dinner with Muse. Once in a lifetime!', NULL, @fiveDaysAgo, @fiveDaysAgo),

-- Arcade Fire (USD)
(@ped19, @campArcade, @userAna,    @fanAna,    2, 2, 30.00,  0.00, 0.00, 0.00, 30.00,   1, N'I want to vote on the tracklist!',            NULL, @tenDaysAgo, @tenDaysAgo),
(@ped20, @campArcade, @userMaria,  @fanMaria,  2, 2, 60.00,  0.00, 8.00, 0.00, 68.00,   1, N'Love the community concept. Need that zine!', NULL, @oneWeekAgo, @oneWeekAgo);

-- PEDIDO LINEAS
INSERT INTO PedidoCrowdfunding_Linea (Id, PedidoCrowdfunding_Id, Reward_Id, Cantidad, PrecioUnitario, ImporteLinea, EsRewardPrincipal, FechaCreacion) VALUES
(@lin01, @ped01, @rw_vet2, 1, 25.00,  25.00,  1, @tenDaysAgo),
(@lin02, @ped02, @rw_vet3, 1, 50.00,  50.00,  1, @oneWeekAgo),
(@lin03, @ped03, @rw_vet4, 1, 200.00, 200.00, 1, @fiveDaysAgo),
(@lin04, @ped04, @rw_ros2, 1, 35.00,  35.00,  1, @tenDaysAgo),
(@lin05, @ped05, @rw_ros3, 1, 75.00,  75.00,  1, @oneWeekAgo),
(@lin06, @ped06, @rw_tan2, 1, 30.00,  30.00,  1, @tenDaysAgo),
(@lin07, @ped07, @rw_tan3, 1, 60.00,  60.00,  1, @oneWeekAgo),
(@lin08, @ped08, @rw_tan4, 1, 250.00, 250.00, 1, @fiveDaysAgo),
(@lin09, @ped09, @rw_bad2, 1, 40.00,  40.00,  1, @tenDaysAgo),
(@lin10, @ped10, @rw_bad3, 1, 80.00,  80.00,  1, @oneWeekAgo),
(@lin11, @ped11, @rw_bil2, 1, 30.00,  30.00,  1, @tenDaysAgo),
(@lin12, @ped12, @rw_bil3, 1, 55.00,  55.00,  1, @oneWeekAgo),
(@lin13, @ped13, @rw_bil4, 1, 350.00, 350.00, 1, @fiveDaysAgo),
(@lin14, @ped14, @rw_arc2, 1, 25.00,  25.00,  1, @tenDaysAgo),
(@lin15, @ped15, @rw_arc3, 1, 45.00,  45.00,  1, @oneWeekAgo),
(@lin16, @ped16, @rw_mus2, 1, 40.00,  40.00,  1, @tenDaysAgo),
(@lin17, @ped17, @rw_mus3, 1, 85.00,  85.00,  1, @oneWeekAgo),
(@lin18, @ped18, @rw_mus4, 1, 750.00, 750.00, 1, @fiveDaysAgo),
(@lin19, @ped19, @rw_acd2, 1, 30.00,  30.00,  1, @tenDaysAgo),
(@lin20, @ped20, @rw_acd3, 1, 60.00,  60.00,  1, @oneWeekAgo);

-- APORTACIONES (EstadoAportacion_Id=3 = Capturada)
INSERT INTO AportacionCrowdfunding (Id, PedidoCrowdfunding_Id, Moneda_Id, MetodoPago_Id, EstadoAportacion_Id, ImporteTotal, ImporteImpuestos, ImporteComisionPlataforma, ImporteComisionPasarela, ImporteNetoArtista, CodigoOperacionPasarela, CodigoOperacionProveedor, FechaAutorizacion, FechaCaptura, FechaCancelacion, FechaCreacion, FechaActualizacion) VALUES
(@apo01, @ped01, 1, 1, 3, 25.00,  0.00, 1.25,  0.75,  23.00,  N'pi_vet01', N'ch_vet01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo02, @ped02, 1, 1, 3, 60.00,  0.00, 3.00,  1.80,  55.20,  N'pi_vet02', N'ch_vet02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo03, @ped03, 1, 3, 3, 200.00, 0.00, 10.00, 6.00,  184.00, N'pi_vet03', N'ch_vet03', @fiveDaysAgo, @fiveDaysAgo, NULL, @fiveDaysAgo, @fiveDaysAgo),
(@apo04, @ped04, 1, 1, 3, 35.00,  0.00, 1.75,  1.05,  32.20,  N'pi_ros01', N'ch_ros01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo05, @ped05, 1, 2, 3, 83.00,  0.00, 4.15,  2.49,  76.36,  N'pi_ros02', N'ch_ros02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo06, @ped06, 1, 1, 3, 30.00,  0.00, 1.50,  0.90,  27.60,  N'pi_tan01', N'ch_tan01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo07, @ped07, 1, 1, 3, 70.00,  0.00, 3.50,  2.10,  64.40,  N'pi_tan02', N'ch_tan02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo08, @ped08, 1, 3, 3, 260.00, 0.00, 13.00, 7.80,  239.20, N'pi_tan03', N'ch_tan03', @fiveDaysAgo, @fiveDaysAgo, NULL, @fiveDaysAgo, @fiveDaysAgo),
(@apo09, @ped09, 2, 2, 3, 40.00,  0.00, 2.00,  1.20,  36.80,  N'pi_bad01', N'ch_bad01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo10, @ped10, 2, 1, 3, 97.00,  0.00, 4.85,  2.91,  89.24,  N'pi_bad02', N'ch_bad02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo11, @ped11, 2, 1, 3, 30.00,  0.00, 1.50,  0.90,  27.60,  N'pi_bil01', N'ch_bil01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo12, @ped12, 2, 3, 3, 65.00,  0.00, 3.25,  1.95,  59.80,  N'pi_bil02', N'ch_bil02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo13, @ped13, 2, 1, 3, 365.00, 0.00, 18.25, 10.95, 335.80, N'pi_bil03', N'ch_bil03', @fiveDaysAgo, @fiveDaysAgo, NULL, @fiveDaysAgo, @fiveDaysAgo),
(@apo14, @ped14, 3, 1, 3, 25.00,  0.00, 1.25,  0.75,  23.00,  N'pi_arc01', N'ch_arc01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo15, @ped15, 3, 1, 3, 53.00,  0.00, 2.65,  1.59,  48.76,  N'pi_arc02', N'ch_arc02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo16, @ped16, 3, 3, 3, 40.00,  0.00, 2.00,  1.20,  36.80,  N'pi_mus01', N'ch_mus01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo17, @ped17, 3, 1, 3, 100.00, 0.00, 5.00,  3.00,  92.00,  N'pi_mus02', N'ch_mus02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo),
(@apo18, @ped18, 3, 1, 3, 800.00, 0.00, 40.00, 24.00, 736.00, N'pi_mus03', N'ch_mus03', @fiveDaysAgo, @fiveDaysAgo, NULL, @fiveDaysAgo, @fiveDaysAgo),
(@apo19, @ped19, 2, 2, 3, 30.00,  0.00, 1.50,  0.90,  27.60,  N'pi_acd01', N'ch_acd01', @tenDaysAgo, @tenDaysAgo, NULL, @tenDaysAgo, @tenDaysAgo),
(@apo20, @ped20, 2, 1, 3, 68.00,  0.00, 3.40,  2.04,  62.56,  N'pi_acd02', N'ch_acd02', @oneWeekAgo, @oneWeekAgo, NULL, @oneWeekAgo, @oneWeekAgo);

-- ============================================================================
-- SECTION 11: UPDATE CAMPAIGN PLEDGED AMOUNTS
-- ============================================================================

UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 285.00  WHERE Id = @campVetusta;   -- 25 + 60 + 200
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 118.00  WHERE Id = @campRosalia;   -- 35 + 83
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 360.00  WHERE Id = @campTangana;   -- 30 + 70 + 260
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 137.00  WHERE Id = @campBadBunny;  -- 40 + 97
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 460.00  WHERE Id = @campBillie;    -- 30 + 65 + 365
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 78.00   WHERE Id = @campArctic;    -- 25 + 53
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 940.00  WHERE Id = @campMuse;      -- 40 + 100 + 800
UPDATE CampaniaCrowdfunding SET ImportePledgedActual = 98.00   WHERE Id = @campArcade;    -- 30 + 68

-- ============================================================================
-- SECTION 12: CAMPAIGN UPDATES (2-3 per campaign)
-- ============================================================================

INSERT INTO CampaniaCrowdfunding_Update (Id, Campania_Id, Titulo, Contenido, EsPublico, SoloBackers, FechaCreacion, FechaActualizacion) VALUES
-- Vetusta Morla updates
(NEWID(), @campVetusta, N'Arrancamos la campana!', N'Estamos emocionados de lanzar esta campana. Despues de dos anos componiendo, tenemos 14 canciones listas para grabar. Gracias por confiar en nosotros.', 1, 0, DATEADD(DAY, -13, @now), NULL),
(NEWID(), @campVetusta, N'Primer objetivo parcial alcanzado', N'Ya llevamos mas de 200 EUR recaudados. Cada aportacion cuenta. Hemos reservado el estudio para el mes que viene. Os iremos contando el proceso.', 1, 0, DATEADD(DAY, -6, @now), NULL),

-- Rosalia updates
(NEWID(), @campRosalia, N'El concepto de "Abismo"', N'He estado trabajando con un equipo de desarrolladores AR en Barcelona. El videoclip tendra tres narrativas diferentes que el espectador podra explorar con su telefono movil. Es algo que nunca se ha hecho en flamenco.', 1, 0, DATEADD(DAY, -12, @now), NULL),
(NEWID(), @campRosalia, N'Sneak peek del vestuario', N'Exclusivo para backers: os muestro los primeros bocetos del vestuario. El equipo de diseno ha creado piezas que fusionan bata de cola flamenca con estetica cyberpunk.', 0, 1, DATEADD(DAY, -5, @now), NULL),
(NEWID(), @campRosalia, N'Confirmado el director', N'Puedo anunciar que el videoclip sera dirigido por un equipo de artistas visuales de Barcelona especializados en AR y experiencias inmersivas. Mas detalles pronto.', 1, 0, DATEADD(DAY, -2, @now), NULL),

-- C. Tangana updates
(NEWID(), @campTangana, N'Las primeras 3 ciudades confirmadas', N'Madrid, Barcelona y Sevilla son las primeras fechas confirmadas. Salas de 300 personas maximo. Solo guitarra y voz. Va a ser muy especial.', 1, 0, DATEADD(DAY, -11, @now), NULL),
(NEWID(), @campTangana, N'Ensayando versiones acusticas', N'He pasado la ultima semana reencontrando mis canciones con solo una guitarra espanola. "Demasiadas Mujeres" suena completamente diferente asi. Os va a encantar.', 0, 1, DATEADD(DAY, -4, @now), NULL),

-- Bad Bunny updates
(NEWID(), @campBadBunny, N'Director confirmado', N'El documental sera dirigido por un cineasta puertorriqueno que crecio en el mismo barrio que yo en Vega Baja. Esto es personal.', 1, 0, DATEADD(DAY, -10, @now), NULL),
(NEWID(), @campBadBunny, N'Primeras entrevistas grabadas', N'Ya hemos entrevistado a mi mama, mi papa y mis amigos de la infancia. Las historias que cuentan me hacen llorar. Este documental va a ser real.', 0, 1, DATEADD(DAY, -3, @now), NULL),

-- Billie updates
(NEWID(), @campBillie, N'Solar studio setup complete', N'The studio is now 100% solar-powered! We installed 40 panels and a battery system. Recording starts next week. Zero grid electricity.', 1, 0, DATEADD(DAY, -9, @now), NULL),
(NEWID(), @campBillie, N'First track recorded carbon-neutral', N'We just finished tracking the first song. Every watt came from the sun. The sound is warm and organic. This proves green music is possible.', 1, 0, DATEADD(DAY, -2, @now), NULL),

-- Arctic updates
(NEWID(), @campArctic, N'Back in the basement', N'We''re back in Sheffield, back in the basement where we wrote our first songs. It feels like 2002 again. The acoustics down here are terrible and we love it.', 1, 0, DATEADD(DAY, -8, @now), NULL),
(NEWID(), @campArctic, N'Local artist collaboration confirmed', N'Three Sheffield-based artists will create the vinyl sleeve artwork. Each copy will have a slightly different print.', 0, 1, DATEADD(DAY, -3, @now), NULL),

-- Muse updates
(NEWID(), @campMuse, N'LSO rehearsals begin', N'The London Symphony Orchestra has started rehearsing our tracks. Hearing "Knights of Cydonia" with 60 musicians is absolutely breathtaking.', 1, 0, DATEADD(DAY, -7, @now), NULL),
(NEWID(), @campMuse, N'Set list revealed for backers', N'Exclusive: here''s the full 18-song setlist for the Royal Albert Hall show. Some deep cuts you didn''t expect. Check the backer-only section.', 0, 1, DATEADD(DAY, -2, @now), NULL),

-- Arcade Fire updates
(NEWID(), @campArcade, N'All 25 demos available now', N'Backers at the Neighborhood Citizen tier and above: all 25 demos are now available in your dashboard. Start listening and get ready to vote!', 0, 1, DATEADD(DAY, -10, @now), NULL),
(NEWID(), @campArcade, N'Voting opens next week', N'The tracklist vote opens Monday. Each backer gets to pick their 12 favorites from the 25 demos. The people''s album is coming to life.', 1, 0, DATEADD(DAY, -3, @now), NULL);

-- ============================================================================
-- SECTION 13: CAMPAIGN COMMENTS (3-5 per campaign, mix of fan + artist replies)
-- ============================================================================

-- We'll create parent comments first, then replies
-- Vetusta comments
DECLARE @com_v1 UNIQUEIDENTIFIER = NEWID(), @com_v2 UNIQUEIDENTIFIER = NEWID(), @com_v3 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_v1r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_v1, @campVetusta, @userMaria, NULL, N'Llevo esperando este disco desde que salio Cable a Tierra. No puedo esperar!', 0, DATEADD(DAY, -12, @now), NULL, 0),
(@com_v1r, @campVetusta, @userVetusta, @com_v1, N'Gracias Maria! Este disco va a sorprender. Estamos poniendo todo el corazon.', 1, DATEADD(DAY, -11, @now), NULL, 0),
(@com_v2, @campVetusta, @userPablo, NULL, N'El vinilo de edicion coleccionista es una pasada. Ya tengo mi reward.', 0, DATEADD(DAY, -8, @now), NULL, 0),
(@com_v3, @campVetusta, @userCarlos, NULL, N'Desde Mexico apoyando a los mejores del indie espanol!', 0, DATEADD(DAY, -5, @now), NULL, 0);

-- Rosalia comments
DECLARE @com_r1 UNIQUEIDENTIFIER = NEWID(), @com_r2 UNIQUEIDENTIFIER = NEWID(), @com_r3 UNIQUEIDENTIFIER = NEWID(), @com_r4 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_r1r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_r1, @campRosalia, @userAna, NULL, N'La idea del videoclip con AR es absolutamente revolucionaria. ROSALIA siempre innovando!', 0, DATEADD(DAY, -11, @now), NULL, 0),
(@com_r1r, @campRosalia, @userRosalia, @com_r1, N'Gracias! El equipo creativo esta trabajando en algo que va a cambiar como entendemos los videoclips.', 1, DATEADD(DAY, -10, @now), NULL, 0),
(@com_r2, @campRosalia, @userEmma, NULL, N'As a VFX professional I can say this AR concept is incredibly ambitious. Excited to see it!', 0, DATEADD(DAY, -7, @now), NULL, 0),
(@com_r3, @campRosalia, @userPablo, NULL, N'Flamenco meets cyberpunk? Shut up and take my money!', 0, DATEADD(DAY, -4, @now), NULL, 0),
(@com_r4, @campRosalia, @userMaria, NULL, N'El set de art prints con frames del videoclip va a quedar genial enmarcado.', 0, DATEADD(DAY, -2, @now), NULL, 0);

-- Tangana comments
DECLARE @com_t1 UNIQUEIDENTIFIER = NEWID(), @com_t2 UNIQUEIDENTIFIER = NEWID(), @com_t3 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_t2r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_t1, @campTangana, @userMaria, NULL, N'Tangana acustico en sala pequena? Esto va a ser historico.', 0, DATEADD(DAY, -10, @now), NULL, 0),
(@com_t2, @campTangana, @userPablo, NULL, N'Alguna fecha en Valencia? Porfi!', 0, DATEADD(DAY, -7, @now), NULL, 0),
(@com_t2r, @campTangana, @userTangana, @com_t2, N'Valencia esta en los planes. Aun estamos cerrando salas. Paciencia.', 1, DATEADD(DAY, -6, @now), NULL, 0),
(@com_t3, @campTangana, @userCarlos, NULL, N'Me vengo desde Mexico solo para la cena post-concierto. Sin duda.', 0, DATEADD(DAY, -3, @now), NULL, 0);

-- Bad Bunny comments
DECLARE @com_b1 UNIQUEIDENTIFIER = NEWID(), @com_b2 UNIQUEIDENTIFIER = NEWID(), @com_b3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_b1, @campBadBunny, @userCarlos, NULL, N'Este documental va a hacer llorar a toda Latinoamerica. Benito is the GOAT.', 0, DATEADD(DAY, -9, @now), NULL, 0),
(@com_b2, @campBadBunny, @userAna, NULL, N'La premiere VIP en San Juan es un sueno. Ojala alcance un spot!', 0, DATEADD(DAY, -6, @now), NULL, 0),
(@com_b3, @campBadBunny, @userBadBunny, NULL, N'Gracias a todos por el apoyo. Este proyecto es para mi gente de PR y para todos los que creyeron cuando nadie creia. Pa''lante siempre.', 1, DATEADD(DAY, -2, @now), NULL, 0);

-- Billie comments
DECLARE @com_bi1 UNIQUEIDENTIFIER = NEWID(), @com_bi2 UNIQUEIDENTIFIER = NEWID(), @com_bi3 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_bi1r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_bi1, @campBillie, @userEmma, NULL, N'This is exactly what the music industry needs. Proving sustainability and quality can coexist.', 0, DATEADD(DAY, -8, @now), NULL, 0),
(@com_bi1r, @campBillie, @userBillie, @com_bi1, N'Thank you! If we can do it, anyone can. The tech is here, we just need to use it.', 1, DATEADD(DAY, -7, @now), NULL, 0),
(@com_bi2, @campBillie, @userMaria, NULL, N'The seed paper booklet idea is genius! Music AND flowers!', 0, DATEADD(DAY, -5, @now), NULL, 0),
(@com_bi3, @campBillie, @userPablo, NULL, N'Solar-powered studio visit backed! See you there!', 0, DATEADD(DAY, -3, @now), NULL, 0);

-- Arctic comments
DECLARE @com_a1 UNIQUEIDENTIFIER = NEWID(), @com_a2 UNIQUEIDENTIFIER = NEWID(), @com_a3 UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_a1, @campArctic, @userEmma, NULL, N'Back to the basement! This is the Arctic Monkeys energy I''ve been waiting for since Favourite Worst Nightmare.', 0, DATEADD(DAY, -7, @now), NULL, 0),
(@com_a2, @campArctic, @userPablo, NULL, N'The colored vinyl with Sheffield artist prints is such a cool concept.', 0, DATEADD(DAY, -4, @now), NULL, 0),
(@com_a3, @campArctic, @userArctic, NULL, N'We''re having the time of our lives down here. The songs are coming out raw and honest. Just like the early days. Thank you all.', 1, DATEADD(DAY, -1, @now), NULL, 0);

-- Muse comments
DECLARE @com_m1 UNIQUEIDENTIFIER = NEWID(), @com_m2 UNIQUEIDENTIFIER = NEWID(), @com_m3 UNIQUEIDENTIFIER = NEWID(), @com_m4 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_m1r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_m1, @campMuse, @userEmma, NULL, N'Muse with the London Symphony Orchestra at the Royal Albert Hall? This is going to be legendary!', 0, DATEADD(DAY, -6, @now), NULL, 0),
(@com_m1r, @campMuse, @userMuse, @com_m1, N'We''ve been wanting to do this for years. The orchestral arrangements of Hysteria are going to blow minds.', 1, DATEADD(DAY, -5, @now), NULL, 0),
(@com_m2, @campMuse, @userCarlos, NULL, N'I need that orchestral score book. As a musician this is a dream.', 0, DATEADD(DAY, -4, @now), NULL, 0),
(@com_m3, @campMuse, @userAna, NULL, N'Front row VIP backed! This is going to be the concert of a lifetime.', 0, DATEADD(DAY, -3, @now), NULL, 0),
(@com_m4, @campMuse, @userPablo, NULL, N'Will there be a Dolby Atmos mix available? The 3D audio would be incredible for this.', 0, DATEADD(DAY, -1, @now), NULL, 0);

-- Arcade Fire comments
DECLARE @com_af1 UNIQUEIDENTIFIER = NEWID(), @com_af2 UNIQUEIDENTIFIER = NEWID(), @com_af3 UNIQUEIDENTIFIER = NEWID();
DECLARE @com_af1r UNIQUEIDENTIFIER = NEWID();

INSERT INTO CampaniaCrowdfunding_Comentario (Id, Campania_Id, UserId, ComentarioPadre_Id, Contenido, EsRespuestaArtista, FechaCreacion, FechaActualizacion, Borrado) VALUES
(@com_af1, @campArcade, @userAna, NULL, N'Letting fans choose the tracklist is such an Arcade Fire move. Community is everything.', 0, DATEADD(DAY, -9, @now), NULL, 0),
(@com_af1r, @campArcade, @userArcade, @com_af1, N'This band has always been about community. Now we''re taking it to the next level.', 1, DATEADD(DAY, -8, @now), NULL, 0),
(@com_af2, @campArcade, @userMaria, NULL, N'He escuchado algunos demos y son increibles. Va a ser dificil elegir solo 12!', 0, DATEADD(DAY, -5, @now), NULL, 0),
(@com_af3, @campArcade, @userCarlos, NULL, N'The chance to contribute vocals to a bonus track? Take my money!', 0, DATEADD(DAY, -2, @now), NULL, 0);

-- ============================================================================
-- SECTION 14: STRETCH GOALS (1-2 per campaign)
-- ============================================================================

INSERT INTO CampaniaCrowdfunding_StretchGoal (Id, Campania_Id, Titulo, Descripcion, ImporteObjetivo, Moneda_Id, Orden, Alcanzado, FechaAlcanzado, FechaCreacion) VALUES
-- Vetusta: 2 stretch goals
(NEWID(), @campVetusta, N'Orquesta de 50 musicos',         N'Si alcanzamos 60.000 EUR, ampliaremos la orquesta de 30 a 50 musicos para una grabacion aun mas epica.', 60000.00, 1, 1, 0, NULL, @twoWeeksAgo),
(NEWID(), @campVetusta, N'Documental del proceso',          N'Con 75.000 EUR filmaremos un documental de 45 minutos sobre todo el proceso de grabacion.', 75000.00, 1, 2, 0, NULL, @twoWeeksAgo),

-- Rosalia: 1 stretch goal
(NEWID(), @campRosalia, N'Segundo videoclip AR',             N'Si alcanzamos 100.000 EUR, crearemos un segundo videoclip interactivo para otro tema del album.', 100000.00, 1, 1, 0, NULL, @twoWeeksAgo),

-- Tangana: 2 stretch goals
(NEWID(), @campTangana, N'5 ciudades adicionales',           N'Con 45.000 EUR anadiremos 5 ciudades mas a la gira: Bilbao, Malaga, Zaragoza, Valladolid y Santiago.', 45000.00, 1, 1, 0, NULL, @twoWeeksAgo),
(NEWID(), @campTangana, N'Grabacion live album',             N'Si alcanzamos 50.000 EUR grabaremos todas las fechas y lanzaremos un album en vivo.', 50000.00, 1, 2, 0, NULL, @twoWeeksAgo),

-- Bad Bunny: 1 stretch goal
(NEWID(), @campBadBunny, N'Subtitulos en 10 idiomas',       N'Con 80.000 USD ampliaremos los subtitulos de 5 a 10 idiomas incluyendo japones, coreano y arabe.', 80000.00, 2, 1, 0, NULL, @twoWeeksAgo),

-- Billie: 2 stretch goals
(NEWID(), @campBillie, N'Plant 10,000 trees',                N'At 55,000 USD we''ll partner with One Tree Planted to plant 10,000 trees, one for every backer.', 55000.00, 2, 1, 0, NULL, @twoWeeksAgo),
(NEWID(), @campBillie, N'Free album for schools',            N'At 65,000 USD we''ll send free copies of the album + sustainability lesson plans to 500 schools.', 65000.00, 2, 2, 0, NULL, @twoWeeksAgo),

-- Arctic: 1 stretch goal
(NEWID(), @campArctic, N'Basement gig live stream',          N'At 40,000 GBP we''ll live stream an intimate basement gig to all backers worldwide.', 40000.00, 3, 1, 0, NULL, @twoWeeksAgo),

-- Muse: 1 stretch goal
(NEWID(), @campMuse, N'Global cinema broadcast',             N'At 90,000 GBP we''ll broadcast the concert live to cinemas in 20 countries.', 90000.00, 3, 1, 0, NULL, @twoWeeksAgo),

-- Arcade: 2 stretch goals
(NEWID(), @campArcade, N'Bonus disc of B-sides',             N'At 50,000 USD the remaining 13 demos that don''t make the album will be released as a bonus disc.', 50000.00, 2, 1, 0, NULL, @twoWeeksAgo),
(NEWID(), @campArcade, N'Fan remix competition',             N'At 60,000 USD we''ll release stems for 3 tracks and host a global fan remix competition.', 60000.00, 2, 2, 0, NULL, @twoWeeksAgo);

-- ============================================================================
-- SECTION 15: PROYECTOS ARTISTICOS (8 - one per artist, central hub entity)
-- ============================================================================

INSERT INTO ProyectoArtistico (Id, Artista_Id, TipoProyecto_Id, Titulo, DescripcionCorta, DescripcionLarga, UrlPortada, EstadoProyecto_Id, FechaInicio, FechaFinPrevista, FechaFinReal, FechaCreacion, FechaActualizacion) VALUES
-- 1. Vetusta Morla: Album (Crowdfunding ONLY)
(@proy1, @artVetusta, 1, N'Album "Siete Vidas"',
 N'Septimo album de estudio de Vetusta Morla con orquesta sinfonica',
 N'Proyecto de grabacion del septimo album de estudio. 14 canciones originales grabadas en formato analogico con una orquesta de 30 musicos en los estudios de Madrid. El album explorara temas de renovacion, tiempo y memoria colectiva.',
 N'https://images.pexels.com/photos/164938/pexels-photo-164938.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @monthAgo, @in60Days, NULL, @monthAgo, @now),

-- 2. Rosalia: Videoclip (Crowdfunding + Crowdsourcing)
(@proy2, @artRosalia, 3, N'Videoclip "Abismo" AR',
 N'Videoclip interactivo con realidad aumentada para el tema "Abismo"',
 N'Produccion de un videoclip revolucionario que combina flamenco contemporaneo con tecnologia de realidad aumentada. El espectador podra elegir entre tres narrativas diferentes usando su dispositivo movil. Colaboracion con un estudio de AR de Barcelona.',
 N'https://images.pexels.com/photos/3062541/pexels-photo-3062541.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 1, @twoWeeksAgo, @in45Days, NULL, @twoWeeksAgo, @now),

-- 3. Arctic Monkeys: EP (Crowdfunding + Crowdsourcing)
(@proy3, @artArctic, 1, N'EP "Lunar Surface"',
 N'EP experimental de 6 temas grabado en el sotano original de Sheffield',
 N'Vuelta a los origenes: grabacion de un EP de 6 canciones en el sotano de Sheffield donde la banda empezo en 2002. Sonido crudo, sin overdubs, capturando la energia de los primeros dias. Edicion limitada en vinilo con arte de artistas locales de Sheffield.',
 N'https://images.pexels.com/photos/1763075/pexels-photo-1763075.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @twoWeeksAgo, @in45Days, NULL, @twoWeeksAgo, @now),

-- 4. C. Tangana: Gira (Crowdfunding ONLY)
(@proy4, @artTangana, 4, N'Gira Acustica "Sin Cantar ni Afinar"',
 N'Gira acustica por salas pequenas con formato intimo de voz y guitarra',
 N'Gira por 10 ciudades en salas de 300 personas con versiones acusticas de "El Madrileno" y temas nuevos. Sin autotune, sin produccion, solo la verdad de la musica. Formato intimista que conecta directamente con el publico.',
 N'https://images.pexels.com/photos/1407322/pexels-photo-1407322.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @twoWeeksAgo, @in45Days, NULL, @twoWeeksAgo, @now),

-- 5. Bad Bunny: Documental (ALL THREE - Crowdfunding + Crowdsourcing + Crowdpromotion)
(@proy5, @artBadBunny, 5, N'Documental "De Vega Baja al Mundo"',
 N'Documental sobre la historia de Bad Bunny desde Vega Baja hasta los estadios',
 N'Produccion de un documental que cuenta la historia desde Almirante Sur en Vega Baja hasta los estadios mas grandes del mundo. Entrevistas con familia, amigos de infancia y colaboradores. Dirigido por un cineasta puertorriqueno. Subtitulado en 5 idiomas.',
 N'https://images.pexels.com/photos/66134/pexels-photo-66134.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @twoWeeksAgo, @in60Days, NULL, @twoWeeksAgo, @now),

-- 6. Billie Eilish: Album eco (Crowdfunding + Crowdpromotion)
(@proy6, @artBillie, 1, N'Album "Echoes" Eco-Friendly',
 N'Primer album con huella de carbono cero en la historia de la musica',
 N'Produccion de un album de clase mundial siendo completamente carbono neutral. Estudio alimentado por energia solar, vinilo reciclado, distribucion digital-first. Cada apoyo ayuda a compensar la huella de carbono restante y financia tecnologia de grabacion verde innovadora.',
 N'https://images.pexels.com/photos/1105666/pexels-photo-1105666.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @twoWeeksAgo, @in45Days, NULL, @twoWeeksAgo, @now),

-- 7. Muse: Concierto sinfonico (Crowdfunding + Crowdpromotion)
(@proy7, @artMuse, 4, N'Symphonic Experience at Royal Albert Hall',
 N'Concierto unico con la London Symphony Orchestra en el Royal Albert Hall',
 N'Evento unico: clasicos de Muse interpretados con una orquesta de 60 musicos en el Royal Albert Hall. "Hysteria", "Starlight", "Uprising" y mas, arreglados por un equipo de compositores clasicos. El concierto sera grabado en 4K y publicado como album en vivo.',
 N'https://images.pexels.com/photos/210922/pexels-photo-210922.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 1, @twoWeeksAgo, @in60Days, NULL, @twoWeeksAgo, @now),

-- 8. Arcade Fire: Album comunitario (Crowdfunding ONLY)
(@proy8, @artArcade, 1, N'Album "Neighborhoods Vol. 2" Community',
 N'Album comunitario donde los fans eligen canciones y colaboran en produccion',
 N'25 demos grabadas donde los fans deciden cuales 12 forman el album final. Los backers acceden a todos los demos, votan el tracklist, y los de tier superior pueden contribuir vocales o instrumentos a bonus tracks. Musica impulsada por la comunidad.',
 N'https://images.pexels.com/photos/1540406/pexels-photo-1540406.jpeg?auto=compress&cs=tinysrgb&w=800&h=450&fit=crop',
 2, @twoWeeksAgo, @in45Days, NULL, @twoWeeksAgo, @now);

-- ============================================================================
-- SECTION 16: PERFILES PROFESIONALES (3 fans)
-- ============================================================================

INSERT INTO PerfilProfesional (Id, UserId, Titulo, Descripcion, TarifaHora, TarifaProyectoMin, Moneda_Id, UrlPortfolio, FechaCreacion, FechaActualizacion) VALUES
(@perfCarlos, @userCarlos,
 N'Productor Musical',
 N'Productor musical con 10 anos de experiencia en grabacion, mezcla y masterizacion. Especializado en rock alternativo, indie y musica latina. He trabajado con artistas independientes en Mexico y Espana. Estudio propio equipado con Pro Tools HDX.',
 75.00, 2000.00, 1, N'https://carloslopez-music.example.com', @monthAgo, @now),

(@perfAna, @userAna,
 N'Disenadora Grafica Musical',
 N'Disenadora grafica con 5 anos de experiencia en la industria musical. Especializada en portadas de albums, merchandising y branding para artistas. Dominio de Adobe Creative Suite, Figma y herramientas de ilustracion digital.',
 50.00, 800.00, 1, N'https://anamartinez-design.example.com', @monthAgo, @now),

(@perfEmma, @userEmma,
 N'Mixing & Mastering Engineer',
 N'Professional mixing and mastering engineer with 8 years of experience in London studios. Specialized in rock, alternative and electronic music. Credits include work with several UK independent labels. Proficient in Pro Tools, Logic Pro and Ableton Live.',
 85.00, 2500.00, 3, N'https://emmawilson-audio.example.com', @monthAgo, @now);

-- Professional Skills (using TipoSkill_Id: 1=Tecnica, 2=Creativa, 3=Gestion)
INSERT INTO PerfilProfesional_Skill (Id, PerfilProfesional_Id, TipoSkill_Id, Nivel, FechaCreacion) VALUES
-- Carlos: production skills
(NEWID(), @perfCarlos, 1, 5, @monthAgo),  -- Tecnica nivel 5
(NEWID(), @perfCarlos, 2, 4, @monthAgo),  -- Creativa nivel 4

-- Ana: design skills
(NEWID(), @perfAna, 2, 5, @monthAgo),     -- Creativa nivel 5
(NEWID(), @perfAna, 1, 3, @monthAgo),     -- Tecnica nivel 3

-- Emma: audio engineering skills
(NEWID(), @perfEmma, 1, 5, @monthAgo),    -- Tecnica nivel 5
(NEWID(), @perfEmma, 2, 4, @monthAgo);    -- Creativa nivel 4

-- ============================================================================
-- SECTION 17: NECESIDADES CROWDSOURCING (5)
-- ============================================================================

-- EstadoNecesidad: 1=ABIERTA, 2=EN_PROCESO, 3=CERRADA, 4=CANCELADA
-- TipoNecesidad: 1=CONTRATACION, 2=COLABORACION, 3=VOLUNTARIADO
-- ModalidadTrabajo: 1=REMOTO, 2=PRESENCIAL, 3=HIBRIDO

INSERT INTO NecesidadCrowdsourcing (Id, ProyectoArtistico_Id, Artista_Id, Titulo, Descripcion, TipoNecesidad_Id, EstadoNecesidad_Id, ModalidadTrabajo_Id, PresupuestoMin, PresupuestoMax, Moneda_Id, UbicacionCiudad, UbicacionPais, FechaLimitePropuestas, FechaInicioPrevista, FechaCreacion, FechaActualizacion, MotivoCierre) VALUES
-- Vetusta needs: mixing engineer (EN_PROCESO - has agreement)
(@nec1, @proy1, @artVetusta,
 N'Ingeniero de mezcla para album "Siete Vidas"',
 N'Buscamos un ingeniero de mezcla experimentado para las 14 canciones del nuevo album. El proyecto incluye pistas de orquesta sinfonica de 30 musicos, por lo que se requiere experiencia con grandes formaciones. Trabajo en estudio en Madrid con posibilidad de mezcla remota.',
 1, 2, 3, 2000.00, 5000.00, 1, N'Madrid', N'Espana', @in30Days, @twoWeeksAgo, @monthAgo, @now, NULL),

-- Vetusta needs: album cover designer (ABIERTA)
(@nec2, @proy1, @artVetusta,
 N'Diseno de portada y arte grafico para album',
 N'Necesitamos un disenador grafico para crear la portada del album, booklet interior (16 paginas) y arte para las versiones digitales. El concepto visual debe reflejar los temas de renovacion y memoria colectiva del disco.',
 1, 1, 1, 500.00, 1500.00, 1, NULL, NULL, @in30Days, @in30Days, @monthAgo, @now, NULL),

-- Rosalia needs: AR developer (ABIERTA)
(@nec3, @proy2, @artRosalia,
 N'Desarrollador AR para videoclip interactivo "Abismo"',
 N'Buscamos un equipo o desarrollador freelance con experiencia en realidad aumentada (ARKit/ARCore) para implementar las tres narrativas interactivas del videoclip. Debe poder trabajar presencialmente en Barcelona durante la fase de integracion.',
 1, 1, 2, 3000.00, 8000.00, 1, N'Barcelona', N'Espana', @in30Days, @in30Days, @twoWeeksAgo, @now, NULL),

-- Arctic needs: session musician (CERRADA - completed agreement)
(@nec4, @proy3, @artArctic,
 N'Session musician - keyboards for EP recording',
 N'We need a keyboard player for 2 recording sessions in our Sheffield basement studio. Must be comfortable with vintage analog synths (Moog, Juno-106). Raw, unpolished style preferred.',
 1, 3, 2, 400.00, 800.00, 3, N'Sheffield', N'Reino Unido', @oneWeekAgo, @twoWeeksAgo, @monthAgo, @oneWeekAgo, N'Acuerdo completado satisfactoriamente'),

-- Rosalia needs: photographer (CANCELADA)
(@nec5, @proy2, @artRosalia,
 N'Fotografo para sesion promocional "Abismo"',
 N'Buscamos un fotografo para la sesion promocional del single y videoclip "Abismo". Estilo editorial con influencias de flamenco contemporaneo. 1 dia de sesion en estudio en Barcelona.',
 1, 4, 2, 500.00, 1200.00, 1, N'Barcelona', N'Espana', @oneWeekAgo, @twoWeeksAgo, @monthAgo, @oneWeekAgo, N'Se resolvio internamente con el equipo habitual'),

-- Bad Bunny needs: cinematographer (ABIERTA)
(@nec6, @proy5, @artBadBunny,
 N'Director de fotografia para documental "De Vega Baja al Mundo"',
 N'Buscamos un director de fotografia con experiencia en documentales musicales para capturar la esencia de Puerto Rico y la historia de Bad Bunny. El proyecto requiere rodaje en Vega Baja, San Juan y varias ciudades de Estados Unidos. Se valorara experiencia con documentales latinos.',
 1, 1, 3, 3000.00, 8000.00, 2, N'San Juan', N'Puerto Rico', @in30Days, @in30Days, @twoWeeksAgo, @now, NULL),

-- Bad Bunny needs: subtitle translator (ABIERTA)
(@nec7, @proy5, @artBadBunny,
 N'Traductor para subtitulado del documental en 5 idiomas',
 N'Necesitamos un equipo o profesional de traduccion para subtitular el documental completo (estimado 90 minutos) en ingles, portugues, frances, aleman y japones. Se requiere experiencia con contenido coloquial puertorriqueno y jerga musical.',
 2, 1, 1, 1500.00, 4000.00, 2, NULL, NULL, @in30Days, @in30Days, @twoWeeksAgo, @now, NULL);

-- ============================================================================
-- SECTION 18: PROPUESTAS CROWDSOURCING (6)
-- ============================================================================

-- EstadoPropuesta: 1=PENDIENTE, 2=ACEPTADA, 3=RECHAZADA, 4=RETIRADA

INSERT INTO PropuestaCrowdsourcing (Id, Necesidad_Id, UserId, PerfilProfesional_Id, MensajePropuesta, PrecioPropuesto, Moneda_Id, DiasEstimados, EstadoPropuesta_Id, FechaCreacion, FechaActualizacion, MotivoRechazo, Acuerdo_Id_Ref) VALUES
-- Nec1 (Vetusta mixing): Emma accepted, Carlos rejected
(@prop1, @nec1, @userEmma, @perfEmma,
 N'I have 8 years of experience mixing rock and orchestral recordings. I''ve worked with the London Philharmonic on indie rock crossover projects. I can work remotely from London and travel to Madrid for key sessions.',
 3500.00, 1, 30, 2, @twoWeeksAgo, @oneWeekAgo, NULL, @acuerdo1),

(@prop2, @nec1, @userCarlos, @perfCarlos,
 N'Tengo amplia experiencia mezclando rock alternativo y he trabajado con formaciones grandes en Mexico. Puedo adaptar mi calendario para este proyecto.',
 3000.00, 1, 25, 3, @twoWeeksAgo, @oneWeekAgo, N'Se requiere experiencia especifica con orquestas sinfonicas completas', NULL),

-- Nec2 (Vetusta album cover): Ana pending, plus another pending
(@prop3, @nec2, @userAna, @perfAna,
 N'Soy disenadora grafica especializada en la industria musical. He creado portadas para sellos independientes en Buenos Aires y Madrid. Mi estilo combina ilustracion digital con fotografia, perfecto para el concepto de renovacion y memoria.',
 1000.00, 1, 15, 1, @oneWeekAgo, NULL, NULL, NULL),

(@prop4, @nec2, @userCarlos, @perfCarlos,
 N'Ademas de produccion musical, tengo experiencia en direccion de arte para proyectos discograficos. Puedo aportar una vision integral sonido-imagen.',
 800.00, 1, 12, 1, @fiveDaysAgo, NULL, NULL, NULL),

-- Nec3 (Rosalia AR): one pending proposal
(@prop5, @nec3, @userEmma, @perfEmma,
 N'While my primary expertise is audio, I have connections with AR development studios in London and have collaborated on interactive music projects before. I can coordinate the technical audio-visual integration.',
 5000.00, 1, 45, 1, @oneWeekAgo, NULL, NULL, NULL),

-- Nec4 (Arctic keyboards): Carlos accepted (agreement completed)
(@prop6, @nec4, @userCarlos, @perfCarlos,
 N'I play vintage synths and have a collection of Moog and Juno keyboards. I can bring my own gear to Sheffield. Raw and unpolished is my specialty.',
 600.00, 3, 3, 2, @twoWeeksAgo, @oneWeekAgo, NULL, @acuerdo2);

-- ============================================================================
-- SECTION 19: ACUERDOS CROWDSOURCING (2)
-- ============================================================================

-- EstadoAcuerdo_Id: 1=Activo, 2=Completado, 3=Cancelado (MaestraEstadoAcuerdo identity column)

INSERT INTO AcuerdoCrowdsourcing (Id, Necesidad_Id, Propuesta_Id, Artista_Id, UserIdProveedor, PerfilProfesional_Id, TituloInterno, Descripcion, EstadoAcuerdo_Id, Moneda_Id, ImporteTotalPactado, ImporteAnticipo, PorcentajeAnticipo, FechaInicio, FechaFinPrevista, FechaFinReal, FechaCreacion, FechaActualizacion, CanceladoPor, MaestraEstadoAcuerdoId, MotivoCancelacion) VALUES
-- Agreement 1: Emma mixing for Vetusta (ACTIVE)
(@acuerdo1, @nec1, @prop1, @artVetusta, @userEmma, @perfEmma,
 N'Mezcla album Siete Vidas',
 N'Contrato de mezcla para las 14 canciones del album "Siete Vidas" de Vetusta Morla. Incluye mezcla estereo y Dolby Atmos. Trabajo hibrido: sesiones clave presenciales en Madrid, mezcla remota desde Londres.',
 1, 1, 3500.00, 1050.00, 30.00, @oneWeekAgo, @in45Days, NULL, @oneWeekAgo, @now, NULL, 1, NULL),

-- Agreement 2: Carlos keyboards for Arctic (COMPLETED)
(@acuerdo2, @nec4, @prop6, @artArctic, @userCarlos, @perfCarlos,
 N'Session keyboards EP Lunar Surface',
 N'Grabacion de teclados (Moog Sub 37 y Juno-106) para 4 de las 6 canciones del EP "Lunar Surface". Dos sesiones de grabacion en el sotano de Sheffield.',
 2, 3, 600.00, 200.00, 33.33, @twoWeeksAgo, @oneWeekAgo, @threeDAgo, @twoWeeksAgo, @threeDAgo, NULL, 2, NULL);

-- ============================================================================
-- SECTION 20: MILESTONES (3)
-- ============================================================================

INSERT INTO AcuerdoCrowdsourcing_Milestone (Id, Acuerdo_Id, Titulo, Descripcion, Orden, ImporteParcial, PorcentajeParcial, FechaLimite, FechaCompletado, FechaCreacion) VALUES
-- Agreement 1 milestones (Emma - Vetusta mixing)
(@mile1, @acuerdo1, N'Mezcla primeras 5 canciones',
 N'Mezcla estereo de las 5 primeras canciones del album, incluyendo las que tienen orquesta completa',
 1, 1500.00, 42.86, @in30Days, NULL, @oneWeekAgo),

(@mile2, @acuerdo1, N'Mezcla canciones 6-14 + Atmos',
 N'Mezcla estereo de las 9 canciones restantes + mezcla Dolby Atmos de todo el album',
 2, 2000.00, 57.14, @in45Days, NULL, @oneWeekAgo),

-- Agreement 2 milestone (Carlos - Arctic keyboards) - completed
(@mile3, @acuerdo2, N'Grabacion de teclados completa',
 N'Grabacion de Moog y Juno-106 para las 4 canciones acordadas en dos sesiones de estudio',
 1, 600.00, 100.00, @oneWeekAgo, @threeDAgo, @twoWeeksAgo);

-- ============================================================================
-- SECTION 21: ENTREGABLES (2)
-- ============================================================================

-- EstadoEntregable_Id: 1=Entregado, 2=Aprobado, 3=Rechazado (MaestraEstadoEntregable identity)

INSERT INTO AcuerdoCrowdsourcing_Entregable (Id, Acuerdo_Id, Milestone_Id, Titulo, Descripcion, UrlRecurso, EstadoEntregable_Id, FechaEntrega, FechaAprobacion, ComentarioAprobacion, FechaCreacion, ComentarioRechazo, FechaActualizacion, MaestraEstadoEntregableId) VALUES
-- Carlos's keyboard recordings (APPROVED)
(@entre1, @acuerdo2, @mile3, N'Grabaciones de teclados - sesiones 1 y 2',
 N'Archivos WAV 24bit/96kHz de todas las pistas de Moog Sub 37 y Juno-106 para las 4 canciones. Incluye tomas alternativas.',
 N'https://drive.example.com/arctic-keyboards-sessions',
 2, @fiveDaysAgo, @threeDAgo, N'Perfect raw sound, exactly what we wanted. The Juno parts on track 3 are brilliant.',
 @fiveDaysAgo, NULL, @threeDAgo, 2),

-- Emma's first mix batch (ENTREGADO - pending review)
(@entre2, @acuerdo1, @mile1, N'Mezcla preliminar - cancion 1 "Renacer"',
 N'Primera mezcla estereo de la cancion "Renacer" para revision y feedback antes de continuar con el resto del album.',
 N'https://drive.example.com/vetusta-mix-renacer-v1',
 1, @threeDAgo, NULL, NULL,
 @threeDAgo, NULL, NULL, 1);

-- ============================================================================
-- SECTION 22: CONVERSACIONES Y MENSAJES
-- ============================================================================

INSERT INTO ConversacionCrowdsourcing (Id, Necesidad_Id, Acuerdo_Id, UserIdCreador, UserIdDestinatario, Asunto, FechaCreacion, FechaUltimoMensaje) VALUES
-- Conv 1: Vetusta <-> Emma about mixing agreement
(@conv1, @nec1, @acuerdo1, @userVetusta, @userEmma, N'Mezcla album Siete Vidas - Coordinacion', @oneWeekAgo, @threeDAgo),

-- Conv 2: Arctic <-> Carlos about keyboard session
(@conv2, @nec4, @acuerdo2, @userArctic, @userCarlos, N'Keyboard sessions - Sheffield logistics', @twoWeeksAgo, @threeDAgo),

-- Conv 3: Vetusta <-> Ana about album cover proposal
(@conv3, @nec2, NULL, @userVetusta, @userAna, N'Propuesta diseno portada album', @oneWeekAgo, @fiveDaysAgo),

-- Conv 4: Rosalia <-> Emma about AR integration
(@conv4, @nec3, NULL, @userRosalia, @userEmma, N'AR videoclip - audio integration', @oneWeekAgo, @fiveDaysAgo);

-- MENSAJES (12 total)
INSERT INTO MensajeCrowdsourcing (Id, Conversacion_Id, UserIdRemitente, Contenido, UrlAdjunto, Leido, FechaLeido, FechaCreacion) VALUES
-- Conv 1: Vetusta <-> Emma (4 messages)
(@msg01, @conv1, @userVetusta, N'Hola Emma! Encantados de trabajar contigo. Te enviamos los stems de la primera cancion "Renacer" para que puedas empezar. Son 48 pistas incluyendo la seccion de cuerdas.', NULL, 1, @oneWeekAgo, @oneWeekAgo),
(@msg02, @conv1, @userEmma, N'Thank you! I''ve downloaded the stems. The orchestral recording quality is excellent. I''ll have a preliminary mix ready within 5 days. Quick question: do you want the strings upfront or sitting behind the guitars?', NULL, 1, DATEADD(DAY, -6, @now), DATEADD(DAY, -6, @now)),
(@msg03, @conv1, @userVetusta, N'Queremos que las cuerdas tengan protagonismo pero sin tapar las guitarras. Un equilibrio donde ambos respiren. Te confiamos la decision artistica en la mezcla preliminar.', NULL, 1, @fiveDaysAgo, @fiveDaysAgo),
(@msg04, @conv1, @userEmma, N'Perfect, that''s exactly the approach I was thinking. I''ve uploaded the first mix of "Renacer" as a deliverable. Let me know your thoughts - I tried to create that breathing space between strings and guitars.', N'https://drive.example.com/vetusta-mix-renacer-v1', 1, @threeDAgo, @threeDAgo),

-- Conv 2: Arctic <-> Carlos (3 messages)
(@msg05, @conv2, @userArctic, N'Hey Carlos! So excited to have you on board. Can you bring your Moog Sub 37 and Juno-106? Our basement has limited space but we can set up in the corner near the amp wall.', NULL, 1, @twoWeeksAgo, @twoWeeksAgo),
(@msg06, @conv2, @userCarlos, N'Absolutely! Both synths travel well. I also have a Korg MS-20 if you want something more aggressive for any tracks. What''s the vibe - more spacey or more punchy?', NULL, 1, DATEADD(DAY, -12, @now), DATEADD(DAY, -12, @now)),
(@msg07, @conv2, @userArctic, N'Let''s go spacey for tracks 1 and 3, punchy for tracks 4 and 6. Bring the MS-20 too - we might want to experiment. Sessions are Tuesday and Thursday next week, 10am start.', NULL, 1, DATEADD(DAY, -11, @now), DATEADD(DAY, -11, @now)),

-- Conv 3: Vetusta <-> Ana (3 messages)
(@msg08, @conv3, @userVetusta, N'Hola Ana! Hemos visto tu portfolio y nos encanta tu estilo. El concepto del album gira en torno a la idea de "renovacion despues de la tormenta". Buscamos algo visual que transmita esa sensacion.', NULL, 1, @oneWeekAgo, @oneWeekAgo),
(@msg09, @conv3, @userAna, N'Gracias por contactar! Me encanta el concepto. Estoy pensando en una propuesta que combine fotografia de paisajes post-tormenta con ilustracion digital superpuesta. Puedo preparar 3 bocetos para la semana que viene?', NULL, 1, DATEADD(DAY, -6, @now), DATEADD(DAY, -6, @now)),
(@msg10, @conv3, @userVetusta, N'Perfecto! Nos encanta la idea de combinar foto real con ilustracion. Preparanos esos 3 bocetos y los revisamos en equipo. Sin prisa, queremos que sea algo especial.', NULL, 1, @fiveDaysAgo, @fiveDaysAgo),

-- Conv 4: Rosalia <-> Emma (2 messages)
(@msg11, @conv4, @userRosalia, N'Emma! He visto que tienes experiencia en proyectos interactivos. Estamos buscando a alguien que pueda coordinar la integracion del audio con la tecnologia AR del videoclip. El audio espacial es clave.', NULL, 1, @oneWeekAgo, @oneWeekAgo),
(@msg12, @conv4, @userEmma, N'Yes! I''ve worked on spatial audio for interactive installations. For AR, we''d need to create multiple audio stems that respond to user position and narrative choice. I can connect you with the best AR studios in London too. Let me put together a detailed proposal.', NULL, 0, NULL, @fiveDaysAgo);

-- ============================================================================
-- SECTION 23: VALORACIONES (2)
-- ============================================================================

-- TipoValoracion: 1=ARTISTA_A_PROFESIONAL, 2=PROFESIONAL_A_ARTISTA

INSERT INTO ValoracionCrowdsourcing (Id, Acuerdo_Id, UserIdAutor, UserIdValorado, TipoValoracion_Id, Puntuacion, Comentario, FechaCreacion) VALUES
-- Arctic rates Carlos (artista -> profesional)
(@val1, @acuerdo2, @userArctic, @userCarlos, 1, 5,
 N'Carlos brought amazing energy and incredible synth sounds to our sessions. The Moog parts on track 3 elevated the entire song. Professional, punctual, and creative. Highly recommended!',
 @threeDAgo),

-- Carlos rates Arctic (profesional -> artista)
(@val2, @acuerdo2, @userCarlos, @userArctic, 2, 5,
 N'Increible experiencia trabajando con Arctic Monkeys. Muy claros con lo que buscaban pero abiertos a experimentar. El estudio en el sotano tiene una energia unica. Volveria a colaborar sin dudarlo.',
 @threeDAgo);

-- ============================================================================
-- SECTION 24: ARTISTA_FAN (some fans follow some artists)
-- ============================================================================

INSERT INTO Artista_Fan (Id, Artista_Id, FanProfile_Id, EsSuperFan, FechaInicio, FechaFin) VALUES
(NEWID(), @artVetusta,  @fanMaria,  1, @monthAgo, NULL),
(NEWID(), @artVetusta,  @fanPablo,  1, @monthAgo, NULL),
(NEWID(), @artRosalia,  @fanAna,    0, @monthAgo, NULL),
(NEWID(), @artRosalia,  @fanMaria,  0, @twoWeeksAgo, NULL),
(NEWID(), @artTangana,  @fanMaria,  0, @monthAgo, NULL),
(NEWID(), @artTangana,  @fanPablo,  1, @monthAgo, NULL),
(NEWID(), @artBadBunny, @fanCarlos, 1, @monthAgo, NULL),
(NEWID(), @artBadBunny, @fanAna,    0, @twoWeeksAgo, NULL),
(NEWID(), @artBillie,   @fanEmma,   1, @monthAgo, NULL),
(NEWID(), @artBillie,   @fanMaria,  0, @twoWeeksAgo, NULL),
(NEWID(), @artArctic,   @fanEmma,   1, @monthAgo, NULL),
(NEWID(), @artArctic,   @fanPablo,  0, @monthAgo, NULL),
(NEWID(), @artMuse,     @fanEmma,   1, @monthAgo, NULL),
(NEWID(), @artMuse,     @fanCarlos, 0, @twoWeeksAgo, NULL),
(NEWID(), @artMuse,     @fanAna,    0, @twoWeeksAgo, NULL),
(NEWID(), @artArcade,   @fanAna,    1, @monthAgo, NULL),
(NEWID(), @artArcade,   @fanMaria,  0, @twoWeeksAgo, NULL);

-- ============================================================================
-- SECTION 25: ARTISTA_MIEMBRO (owner members)
-- ============================================================================

-- RolMiembro_Id: 1=PROPIETARIO
INSERT INTO Artista_Miembro (Id, Artista_Id, UserId, RolMiembro_Id, EsAdmin, FechaCreacion, FechaBaja) VALUES
(NEWID(), @artVetusta,  @userVetusta,  1, 1, @monthAgo, NULL),
(NEWID(), @artRosalia,  @userRosalia,  1, 1, @monthAgo, NULL),
(NEWID(), @artTangana,  @userTangana,  1, 1, @monthAgo, NULL),
(NEWID(), @artBadBunny, @userBadBunny, 1, 1, @monthAgo, NULL),
(NEWID(), @artBillie,   @userBillie,   1, 1, @monthAgo, NULL),
(NEWID(), @artArctic,   @userArctic,   1, 1, @monthAgo, NULL),
(NEWID(), @artMuse,     @userMuse,     1, 1, @monthAgo, NULL),
(NEWID(), @artArcade,   @userArcade,   1, 1, @monthAgo, NULL);

-- ============================================================================
-- SECTION 26: CROWDPROMOTION - PROMOTORES
-- ============================================================================

-- TipoPromotor_Id: 1=INDIVIDUAL, 2=AGENCIA, 3=MEDIO_COMUNICACION
INSERT INTO Promotor (Id, TipoPromotor_Id, UserId, FanProfile_Id, NombrePublico, EmailContacto, UrlSitioWeb, UrlInstagram, UrlTikTok, UrlTwitter, UrlYouTube, SeguidoresTotales, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@promCarlos, 1, @userCarlos, @fanCarlos,
 N'Carlos Promotor Musical', N'carlos.lopez@weplay-test.com',
 NULL, N'https://instagram.com/carlospromotor', N'https://tiktok.com/@carlospromotor', NULL, NULL,
 5000, 1, @twoWeeksAgo, NULL),
(@promAna, 1, @userAna, @fanAna,
 N'Ana Design Studio', N'ana.martinez@weplay-test.com',
 N'https://anadesignstudio.com', N'https://instagram.com/anadesign', NULL, N'https://twitter.com/anadesign', NULL,
 12000, 1, @twoWeeksAgo, NULL),
(@promEmma, 3, @userEmma, @fanEmma,
 N'Emma Music Blog', N'emma.wilson@weplay-test.com',
 N'https://emmamusicblog.com', N'https://instagram.com/emmamusicblog', NULL, N'https://twitter.com/emmamusicblog', N'https://youtube.com/@emmamusicblog',
 25000, 1, @monthAgo, NULL);

-- ============================================================================
-- SECTION 27: CROWDPROMOTION - PROMOPROGRAMAS
-- ============================================================================

-- TipoPromo_Id: 1=REDES_SOCIALES, 5=INFLUENCER
INSERT INTO PromoPrograma (Id, Artista_Id, ProyectoArtistico_Id, CampaniaCrowdfunding_Id, TipoPromo_Id, Titulo, Descripcion, Moneda_Id, UrlLanding, CodigoTrackingBase, ImporteComisionPorcentaje, ImporteComisionFija, PresupuestoTotal, ComisionPorConversion, ComisionPorClick, FechaInicio, FechaFin, EsActivo, FechaCreacion, FechaActualizacion) VALUES
(@progVetusta, @artVetusta, @proy1, @campVetusta, 1,
 N'Promocion Nuevo Album Vetusta',
 N'Programa de promocion en redes sociales para la campana de crowdfunding del nuevo album de Vetusta Morla',
 1, N'https://weplay.com/campania/vetusta-morla', N'VM2026',
 5.00, NULL, 2000.00, NULL, 0.10,
 @twoWeeksAgo, @in60Days, 1, @twoWeeksAgo, NULL),
(@progMuse, @artMuse, @proy7, @campMuse, 5,
 N'Muse Symphonic Global Promo',
 N'Programa de promocion via influencers para la experiencia sinfonica de Muse',
 1, N'https://weplay.com/campania/muse', N'MUSE2026',
 8.00, NULL, 5000.00, NULL, 0.15,
 @twoWeeksAgo, @in60Days, 1, @twoWeeksAgo, NULL),
(@progBadBunny, @artBadBunny, @proy5, @campBadBunny, 1,
 N'Bad Bunny Docu Promo Latino',
 N'Programa de promocion en redes sociales para el documental de Bad Bunny',
 1, N'https://weplay.com/campania/bad-bunny', N'BB2026',
 6.00, NULL, 3000.00, NULL, 0.12,
 @oneWeekAgo, @in45Days, 1, @oneWeekAgo, NULL),
(@progBillie, @artBillie, @proy6, @campBillie, 1,
 N'Billie Eilish Eco Album Promo',
 N'Programa de promocion en redes sociales para el album eco-friendly de Billie Eilish',
 2, N'https://weplay.com/campania/billie-eilish', N'BE2026',
 7.00, NULL, 4000.00, NULL, 0.12,
 @twoWeeksAgo, @in45Days, 1, @twoWeeksAgo, NULL);

-- ============================================================================
-- SECTION 28: CROWDPROMOTION - PROMOPROGRAMAPROMOTOR (enrollments)
-- ============================================================================

INSERT INTO PromoProgramaPromotor (Id, Programa_Id, Promotor_Id, CodigoReferido, UrlReferido, TotalClicks, TotalConversiones, TotalComisionesGeneradas, EsActivo, EsAprobado, EsBloqueado, FechaInscripcion, FechaBaja) VALUES
(@ppp1, @progVetusta,  @promCarlos, N'CARLOS-VM',
 N'https://weplay.com/campania/vetusta-morla?ref=CARLOS-VM',
 45, 3, 15.00, 1, 1, 0, @twoWeeksAgo, NULL),
(@ppp2, @progBadBunny, @promCarlos, N'CARLOS-BB',
 N'https://weplay.com/campania/bad-bunny?ref=CARLOS-BB',
 120, 8, 38.40, 1, 1, 0, @oneWeekAgo, NULL),
(@ppp3, @progVetusta,  @promAna,    N'ANA-VM',
 N'https://weplay.com/campania/vetusta-morla?ref=ANA-VM',
 30, 2, 10.00, 1, 1, 0, @twoWeeksAgo, NULL),
(@ppp4, @progMuse,     @promAna,    N'ANA-MUSE',
 N'https://weplay.com/campania/muse?ref=ANA-MUSE',
 85, 5, 42.75, 1, 1, 0, @tenDaysAgo, NULL),
(@ppp5, @progMuse,     @promEmma,   N'EMMA-MUSE',
 N'https://weplay.com/campania/muse?ref=EMMA-MUSE',
 200, 12, 105.00, 1, 1, 0, @twoWeeksAgo, NULL),
(@ppp6, @progBillie,   @promAna,    N'ANA-BILLIE',
 N'https://weplay.com/campania/billie-eilish?ref=ANA-BILLIE',
 60, 4, 24.00, 1, 1, 0, @twoWeeksAgo, NULL),
(@ppp7, @progBillie,   @promEmma,   N'EMMA-BILLIE',
 N'https://weplay.com/campania/billie-eilish?ref=EMMA-BILLIE',
 150, 9, 75.60, 1, 1, 0, @twoWeeksAgo, NULL);

-- ============================================================================
-- SECTION 29: CROWDPROMOTION - PROMOTAREAS (2 per program)
-- ============================================================================

-- TipoEventoPromo_Id: 1=CLICK, 3=CONVERSION, 4=COMPARTIR
INSERT INTO PromoTarea (Id, Programa_Id, Titulo, Descripcion, InstruccionesUrl, TipoReward_Id, ImporteRecompensa, Moneda_Id, PuntosRecompensa, TipoEventoPromo_Id, EsRepetible, MaxRepeticiones, FechaInicio, FechaFin, Orden, EsActivo, FechaCreacion) VALUES
-- Vetusta tasks
(@ptarea1, @progVetusta,
 N'Compartir en Instagram', N'Publica una historia o post en Instagram sobre la campana de Vetusta Morla',
 N'https://weplay.com/instrucciones/compartir-instagram', NULL, 2.00, 1, 10, 4, 1, 5,
 @twoWeeksAgo, @in60Days, 1, 1, @twoWeeksAgo),
(@ptarea2, @progVetusta,
 N'Generar clicks al landing', N'Comparte tu enlace personalizado para generar trafico al landing de la campana',
 NULL, NULL, 0.10, 1, 1, 1, 1, NULL,
 @twoWeeksAgo, @in60Days, 2, 1, @twoWeeksAgo),
-- Muse tasks
(@ptarea3, @progMuse,
 N'Post en blog/redes', N'Escribe un post en tu blog o redes sociales sobre la experiencia sinfonica de Muse',
 N'https://weplay.com/instrucciones/post-blog', NULL, 5.00, 1, 25, 4, 1, 3,
 @twoWeeksAgo, @in60Days, 1, 1, @twoWeeksAgo),
(@ptarea4, @progMuse,
 N'Conversion backing', N'Consigue que un nuevo backer apoye la campana de Muse usando tu enlace',
 NULL, NULL, 10.00, 1, 50, 3, 1, NULL,
 @twoWeeksAgo, @in60Days, 2, 1, @twoWeeksAgo),
-- Bad Bunny tasks
(@ptarea5, @progBadBunny,
 N'Historia Instagram', N'Publica una historia en Instagram sobre el documental de Bad Bunny',
 N'https://weplay.com/instrucciones/historia-ig', NULL, 3.00, 1, 15, 4, 1, 10,
 @oneWeekAgo, @in45Days, 1, 1, @oneWeekAgo),
(@ptarea6, @progBadBunny,
 N'Click al documental', N'Comparte tu enlace personalizado para generar trafico al landing del documental',
 NULL, NULL, 0.12, 1, 1, 1, 1, NULL,
 @oneWeekAgo, @in45Days, 2, 1, @oneWeekAgo),
-- Billie Eilish tasks
(@ptarea7, @progBillie,
 N'Share Eco Album on Social Media', N'Post about Billie Eilish eco-friendly album campaign on your social media channels',
 N'https://weplay.com/instrucciones/share-eco-album', NULL, 3.00, 2, 15, 4, 1, 5,
 @twoWeeksAgo, @in45Days, 1, 1, @twoWeeksAgo),
(@ptarea8, @progBillie,
 N'Drive clicks to campaign', N'Share your personalized link to drive traffic to the Billie Eilish campaign landing page',
 NULL, NULL, 0.12, 2, 1, 1, 1, NULL,
 @twoWeeksAgo, @in45Days, 2, 1, @twoWeeksAgo);

-- ============================================================================
-- SECTION 30: CROWDPROMOTION - PROMOTAREAPROMOTOR (task completions)
-- ============================================================================

-- EstadoTarea_Id: 1=PENDIENTE, 2=EN_PROGRESO, 3=COMPLETADA, 4=VALIDADA, 5=RECHAZADA
INSERT INTO PromoTareaPromotor (Id, Tarea_Id, ProgramaPromotor_Id, EstadoTarea_Id, UrlPruebaCompletado, ComentarioPromotor, ComentarioValidacion, FechaCompletado, FechaValidado, FechaCreacion) VALUES
-- 3 VALIDADA
(@ptp1, @ptarea1, @ppp1, 4,
 N'https://instagram.com/p/carlos-vetusta-story1', N'Historia publicada con enlace a la campana', N'Verificado, excelente alcance',
 @tenDaysAgo, @oneWeekAgo, @twoWeeksAgo),
(@ptp2, @ptarea3, @ppp4, 4,
 N'https://anadesign.com/blog/muse-sinfonica', N'Post completo en mi blog con fotos y enlace', N'Gran calidad de contenido',
 @oneWeekAgo, @fiveDaysAgo, @tenDaysAgo),
(@ptp3, @ptarea3, @ppp5, 4,
 N'https://emmamusicblog.com/muse-symphonic-review', N'Review detallada en el blog', N'Excelente cobertura profesional',
 @oneWeekAgo, @fiveDaysAgo, @tenDaysAgo),
-- 2 COMPLETADA
(@ptp4, @ptarea5, @ppp2, 3,
 N'https://instagram.com/stories/carlos-bb-docu', N'Historia publicada sobre el documental', NULL,
 @threeDAgo, NULL, @fiveDaysAgo),
(@ptp5, @ptarea4, @ppp5, 3,
 N'https://emmamusicblog.com/muse-backing-guide', N'Guia para backers publicada', NULL,
 @threeDAgo, NULL, @fiveDaysAgo),
-- 2 PENDIENTE
(@ptp6, @ptarea2, @ppp1, 1,
 NULL, NULL, NULL,
 NULL, NULL, @oneWeekAgo),
(@ptp7, @ptarea6, @ppp2, 1,
 NULL, NULL, NULL,
 NULL, NULL, @threeDAgo),
-- 1 RECHAZADA
(@ptp8, @ptarea1, @ppp3, 5,
 N'https://instagram.com/p/ana-vetusta-post', N'Post publicado en feed', N'El post no cumple con las directrices de marca',
 @oneWeekAgo, @fiveDaysAgo, @tenDaysAgo);

-- ============================================================================
-- SECTION 31: CROWDPROMOTION - PROMOTORWALLET
-- ============================================================================

-- Moneda_Id: 1=EUR
-- NOTE: RowVersion is auto-generated, do NOT insert it
INSERT INTO PromotorWallet (Id, Promotor_Id, Moneda_Id, SaldoDisponible, SaldoPendiente, TotalGanado, TotalRetirado, FechaCreacion, FechaActualizacion) VALUES
(@walCarlos, @promCarlos, 1, 25.50,  8.00, 33.50,  0.00, @twoWeeksAgo, @threeDAgo),
(@walAna,    @promAna,    1, 42.00, 15.00, 57.00,  0.00, @twoWeeksAgo, @fiveDaysAgo),
(@walEmma,   @promEmma,   1, 95.00, 20.00, 115.00, 0.00, @monthAgo,    @threeDAgo);

-- ============================================================================
-- SECTION 32: CROWDPROMOTION - PROMOTORWALLETTRANSACCION
-- ============================================================================

-- EstadoTransaccion_Id: 2=COMPLETADA, EsCredito=1
INSERT INTO PromotorWalletTransaccion (Id, Wallet_Id, TipoReward_Id, EstadoTransaccion_Id, CampaniaPayout_Id, Importe, Concepto, ReferenciaExterna, FechaCreacion, FechaProcesado, EsCredito, PromoEvento_Id, Descripcion) VALUES
(@wtx1, @walCarlos, NULL, 2, NULL, 2.00,
 N'Tarea completada: Compartir en Instagram', NULL, @tenDaysAgo, @oneWeekAgo, 1, NULL,
 N'Recompensa por compartir campana Vetusta Morla en Instagram'),
(@wtx2, @walCarlos, NULL, 2, NULL, 14.40,
 N'Comision por clicks: Bad Bunny', NULL, @fiveDaysAgo, @threeDAgo, 1, NULL,
 N'Comision acumulada por 120 clicks en campana Bad Bunny'),
(@wtx3, @walAna, NULL, 2, NULL, 5.00,
 N'Tarea completada: Post en blog/redes', NULL, @oneWeekAgo, @fiveDaysAgo, 1, NULL,
 N'Recompensa por post en blog sobre Muse Sinfonica'),
(@wtx4, @walAna, NULL, 2, NULL, 12.75,
 N'Comision por clicks: Muse', NULL, @fiveDaysAgo, @threeDAgo, 1, NULL,
 N'Comision acumulada por 85 clicks en campana Muse'),
(@wtx5, @walEmma, NULL, 2, NULL, 5.00,
 N'Tarea completada: Post en blog/redes', NULL, @oneWeekAgo, @fiveDaysAgo, 1, NULL,
 N'Recompensa por review en blog sobre Muse Symphonic'),
(@wtx6, @walEmma, NULL, 2, NULL, 30.00,
 N'Comision por clicks: Muse', NULL, @fiveDaysAgo, @threeDAgo, 1, NULL,
 N'Comision acumulada por 200 clicks en campana Muse');

-- ============================================================================
-- SECTION 33: CROWDPROMOTION - PROMOEVENTOS (tracking events)
-- ============================================================================

-- TipoEvento_Id: 1=CLICK, 2=IMPRESION, 3=CONVERSION, 4=COMPARTIR
INSERT INTO PromoEvento (Id, Programa_Id, Promotor_Id, CampaniaCrowdfunding_Id, PedidoCrowdfunding_Id, AportacionCrowdfunding_Id, TipoEvento_Id, Moneda_Id, ImporteAsociado, CodigoReferido, IpOrigen, UserAgentOrigen, PromoPrograma_Promotor_Id, UserIdAfectado, UrlOrigen, UrlReferer, UtmSource, UtmMedium, UtmCampaign, FechaCreacion) VALUES
-- Clicks
(@pevt01, @progVetusta, @promCarlos, @campVetusta, NULL, NULL, 1, NULL, NULL,
 N'CARLOS-VM', N'192.168.1.10', N'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0)',
 @ppp1, NULL, N'https://instagram.com/stories/carlospromotor', N'https://instagram.com',
 N'instagram', N'social', N'vetusta-album', @tenDaysAgo),
(@pevt02, @progMuse, @promEmma, @campMuse, NULL, NULL, 1, NULL, NULL,
 N'EMMA-MUSE', N'10.0.0.55', N'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)',
 @ppp5, NULL, N'https://emmamusicblog.com/muse-review', N'https://emmamusicblog.com',
 N'blog', N'referral', N'muse-symphonic', @oneWeekAgo),
(@pevt03, @progBadBunny, @promCarlos, @campBadBunny, NULL, NULL, 1, NULL, NULL,
 N'CARLOS-BB', N'192.168.1.10', N'Mozilla/5.0 (Android 14)',
 @ppp2, NULL, N'https://tiktok.com/@carlospromotor', N'https://tiktok.com',
 N'tiktok', N'social', N'badbunny-docu', @fiveDaysAgo),
-- Shares
(@pevt04, @progVetusta, @promCarlos, @campVetusta, NULL, NULL, 4, NULL, NULL,
 N'CARLOS-VM', N'192.168.1.10', N'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0)',
 @ppp1, NULL, N'https://instagram.com/p/carlos-vetusta-story1', NULL,
 N'instagram', N'social', N'vetusta-album', @tenDaysAgo),
(@pevt05, @progMuse, @promAna, @campMuse, NULL, NULL, 4, NULL, NULL,
 N'ANA-MUSE', N'172.16.0.22', N'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
 @ppp4, NULL, N'https://anadesign.com/blog/muse-sinfonica', NULL,
 N'blog', N'referral', N'muse-symphonic', @oneWeekAgo),
(@pevt06, @progMuse, @promEmma, @campMuse, NULL, NULL, 4, NULL, NULL,
 N'EMMA-MUSE', N'10.0.0.55', N'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)',
 @ppp5, NULL, N'https://emmamusicblog.com/muse-symphonic-review', NULL,
 N'blog', N'referral', N'muse-symphonic', @oneWeekAgo),
-- Conversions
(@pevt07, @progVetusta, @promCarlos, @campVetusta, @ped01, @apo01, 3, 1, 25.00,
 N'CARLOS-VM', N'203.0.113.15', N'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
 @ppp1, @userMaria, N'https://weplay.com/campania/vetusta-morla?ref=CARLOS-VM', N'https://instagram.com',
 N'instagram', N'social', N'vetusta-album', @oneWeekAgo),
(@pevt08, @progMuse, @promEmma, @campMuse, @ped06, @apo06, 3, 1, 50.00,
 N'EMMA-MUSE', N'198.51.100.42', N'Mozilla/5.0 (Linux; Android 14)',
 @ppp5, @userPablo, N'https://weplay.com/campania/muse?ref=EMMA-MUSE', N'https://emmamusicblog.com',
 N'blog', N'referral', N'muse-symphonic', @fiveDaysAgo),
-- More clicks
(@pevt09, @progMuse, @promAna, @campMuse, NULL, NULL, 1, NULL, NULL,
 N'ANA-MUSE', N'172.16.0.22', N'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
 @ppp4, NULL, N'https://twitter.com/anadesign/status/123456', N'https://twitter.com',
 N'twitter', N'social', N'muse-symphonic', @fiveDaysAgo),
(@pevt10, @progBadBunny, @promCarlos, @campBadBunny, NULL, NULL, 1, NULL, NULL,
 N'CARLOS-BB', N'192.168.1.10', N'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0)',
 @ppp2, NULL, N'https://instagram.com/stories/carlospromotor', N'https://instagram.com',
 N'instagram', N'social', N'badbunny-docu', @threeDAgo);

PRINT '--- All data inserted successfully ---'
PRINT ''
PRINT '=== WePlay Rises Seed Script - COMPLETE ==='
PRINT ''
PRINT '=== SUMMARY ==='
PRINT 'Users created: 13 (8 artists + 5 fans)'
PRINT 'Artists: 8 (Vetusta Morla, Rosalia, C. Tangana, Bad Bunny, Billie Eilish, Arctic Monkeys, Muse, Arcade Fire)'
PRINT 'Fans: 5 (Maria, Carlos, Ana, Pablo, Emma)'
PRINT 'Campaigns: 8 (all active)'
PRINT 'Rewards: 32 (4 per campaign)'
PRINT 'Backings: 20 (Pedido + Linea + Aportacion)'
PRINT 'Updates: 18'
PRINT 'Comments: ~40 (with replies)'
PRINT 'Stretch Goals: 13'
PRINT 'Proyectos Artisticos: 3'
PRINT 'Necesidades: 5'
PRINT 'Perfiles Profesionales: 3'
PRINT 'Propuestas: 6'
PRINT 'Acuerdos: 2'
PRINT 'Milestones: 3'
PRINT 'Entregables: 2'
PRINT 'Conversaciones: 4'
PRINT 'Mensajes: 12'
PRINT 'Valoraciones: 2'
PRINT '--- Crowdpromotion ---'
PRINT 'Promotores: 3 (Carlos, Ana, Emma)'
PRINT 'PromoProgramas: 3 (Vetusta, Muse, Bad Bunny)'
PRINT 'PromoProgramaPromotor: 5 (enrollments)'
PRINT 'PromoTareas: 6 (2 per program)'
PRINT 'PromoTareaPromotor: 8 (task completions)'
PRINT 'PromotorWallets: 3'
PRINT 'PromotorWalletTransacciones: 6'
PRINT 'PromoEventos: 10 (tracking events)'
PRINT ''
PRINT '=== LOGIN CREDENTIALS ==='
PRINT 'Artist accounts: vetusta@weplay-test.com / WePlay2026! (also has Admin role)'
PRINT 'Artist accounts: rosalia@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: tangana@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: badbunny@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: billie@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: arctic@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: muse@weplay-test.com / WePlay2026!'
PRINT 'Artist accounts: arcade@weplay-test.com / WePlay2026!'
PRINT 'Fan accounts: maria.garcia@weplay-test.com / Test123!'
PRINT 'Fan accounts: carlos.lopez@weplay-test.com / Test123!'
PRINT 'Fan accounts: ana.martinez@weplay-test.com / Test123!'
PRINT 'Fan accounts: pablo.ruiz@weplay-test.com / Test123!'
PRINT 'Fan accounts: emma.wilson@weplay-test.com / Test123!'
