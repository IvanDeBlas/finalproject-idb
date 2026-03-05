-- ============================================================
-- WePlay Rises - Modelo de Datos: Identity + UserAccess + Crowdfunding
-- Actualizado: 2026-02-16
-- Generado desde EF Core Migrations
-- ============================================================
-- Orden de ejecucion:
--   1. Identity (AspNetUsers, AspNetRoles)
--   2. Perfiles (Artista, Fan, Profesional)
--   3. Crowdfunding
-- ============================================================
-- NOTA: Las tablas maestras se crean en el script Core (ver migracion CoreContext)
-- NOTA: Los nombres de tabla Identity usan prefijo AspNet (convencion EF Core Identity)
-- ============================================================

--	DROP TABLE [dbo].[CampaniaCrowdfunding_Payout]
--	DROP TABLE [dbo].[AportacionCrowdfunding]
--	DROP TABLE [dbo].[PedidoCrowdfunding_Linea]
--	DROP TABLE [dbo].[PedidoCrowdfunding]
--	DROP TABLE [dbo].[CampaniaCrowdfunding_Reward]
--	DROP TABLE [dbo].[CampaniaCrowdfunding_StretchGoal]
--	DROP TABLE [dbo].[CampaniaCrowdfunding_Comentario]
--	DROP TABLE [dbo].[CampaniaCrowdfunding_Update]
--	DROP TABLE [dbo].[CampaniaCrowdfunding]
--	DROP TABLE [dbo].[Artista_MembershipPago]
--	DROP TABLE [dbo].[Artista_MembershipSuscripcion]
--	DROP TABLE [dbo].[Artista_MembershipPlan]
--	DROP TABLE [dbo].[Artista_PayoutCuenta]
--	DROP TABLE [dbo].[ProyectoArtistico]
--	DROP TABLE [dbo].[PerfilProfesional_PortfolioItem]
--	DROP TABLE [dbo].[PerfilProfesional_Skill]
--	DROP TABLE [dbo].[PerfilProfesional]
--	DROP TABLE [dbo].[Artista_Fan]
--	DROP TABLE [dbo].[FanProfile]
--	DROP TABLE [dbo].[Artista_Miembro]
--	DROP TABLE [dbo].[Artista]
--	DROP TABLE [dbo].[DireccionPostal]
--	DROP TABLE [dbo].[AspNetUserTokens]
--	DROP TABLE [dbo].[AspNetUserLogins]
--	DROP TABLE [dbo].[AspNetRoleClaims]
--	DROP TABLE [dbo].[AspNetUserClaims]
--	DROP TABLE [dbo].[AspNetUserRoles]
--	DROP TABLE [dbo].[AspNetRoles]
--	DROP TABLE [dbo].[AspNetUsers]
--	DROP TABLE [dbo].[__EFMigrationsHistory]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- PARTE 1: EF MIGRATIONS HISTORY
-- ============================================================

CREATE TABLE [dbo].[__EFMigrationsHistory](
    [MigrationId]       NVARCHAR(150)   NOT NULL,
    [ProductVersion]    NVARCHAR(32)    NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
);
GO

-- ============================================================
-- PARTE 2: IDENTITY (ASP.NET Core Identity - EF Core)
-- ============================================================

CREATE TABLE [dbo].[AspNetUsers](
    [Id]                        NVARCHAR(450)       NOT NULL,
    [UserName]                  NVARCHAR(256)       NULL,
    [NormalizedUserName]        NVARCHAR(256)       NULL,
    [Email]                     NVARCHAR(256)       NULL,
    [NormalizedEmail]           NVARCHAR(256)       NULL,
    [EmailConfirmed]            BIT                 NOT NULL,
    [PasswordHash]              NVARCHAR(MAX)       NULL,
    [SecurityStamp]             NVARCHAR(MAX)       NULL,
    [ConcurrencyStamp]          NVARCHAR(MAX)       NULL,
    [PhoneNumber]               NVARCHAR(MAX)       NULL,
    [PhoneNumberConfirmed]      BIT                 NOT NULL,
    [TwoFactorEnabled]          BIT                 NOT NULL,
    [LockoutEnd]                DATETIMEOFFSET(7)   NULL,
    [LockoutEnabled]            BIT                 NOT NULL,
    [AccessFailedCount]         INT                 NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[AspNetRoles](
    [Id]                NVARCHAR(450)   NOT NULL,
    [Name]              NVARCHAR(256)   NULL,
    [NormalizedName]    NVARCHAR(256)   NULL,
    [ConcurrencyStamp]  NVARCHAR(MAX)   NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[AspNetUserRoles](
    [UserId]    NVARCHAR(450)   NOT NULL,
    [RoleId]    NVARCHAR(450)   NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims](
    [Id]            INT             IDENTITY(1,1) NOT NULL,
    [UserId]        NVARCHAR(450)   NOT NULL,
    [ClaimType]     NVARCHAR(MAX)   NULL,
    [ClaimValue]    NVARCHAR(MAX)   NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetRoleClaims](
    [Id]            INT             IDENTITY(1,1) NOT NULL,
    [RoleId]        NVARCHAR(450)   NOT NULL,
    [ClaimType]     NVARCHAR(MAX)   NULL,
    [ClaimValue]    NVARCHAR(MAX)   NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserLogins](
    [LoginProvider]         NVARCHAR(450)   NOT NULL,
    [ProviderKey]           NVARCHAR(450)   NOT NULL,
    [ProviderDisplayName]   NVARCHAR(MAX)   NULL,
    [UserId]                NVARCHAR(450)   NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserTokens](
    [UserId]        NVARCHAR(450)   NOT NULL,
    [LoginProvider] NVARCHAR(450)   NOT NULL,
    [Name]          NVARCHAR(450)   NOT NULL,
    [Value]         NVARCHAR(MAX)   NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- Indices Identity
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);
GO
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]([UserId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]([RoleId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]([UserId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);
GO


-- ============================================================
-- PARTE 3: PERFILES (Artista, Fan, Profesional)
-- Modulo: UserAccess
-- ============================================================

CREATE TABLE [dbo].[DireccionPostal](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                NVARCHAR(450)       NULL,
    [NombreDestinatario]    NVARCHAR(200)       NOT NULL,
    [Linea1]                NVARCHAR(200)       NOT NULL,
    [Linea2]                NVARCHAR(200)       NULL,
    [Ciudad]                NVARCHAR(100)       NOT NULL,
    [Provincia]             NVARCHAR(100)       NULL,
    [CodigoPostal]          NVARCHAR(20)        NOT NULL,
    [Pais]                  NVARCHAR(100)       NULL,
    [Telefono]              NVARCHAR(50)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_DireccionPostal] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[Artista](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [UserIdPropietario]     NVARCHAR(450)       NOT NULL,
    [NombreArtistico]       NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [Pais]                  NVARCHAR(100)       NULL,
    [Ciudad]                NVARCHAR(100)       NULL,
    [UrlSitioWeb]           NVARCHAR(300)       NULL,
    [UrlInstagram]          NVARCHAR(300)       NULL,
    [UrlYouTube]            NVARCHAR(300)       NULL,
    [UrlSpotify]            NVARCHAR(300)       NULL,
    [ImagenPerfilUrl]       NVARCHAR(500)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Artista_UserIdPropietario] ON [dbo].[Artista]([UserIdPropietario] ASC);
GO

CREATE TABLE [dbo].[Artista_Miembro](
    [Id]                UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]        UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            NVARCHAR(450)       NOT NULL,
    [RolMiembro_Id]     INT                 NOT NULL,
    [EsAdmin]           BIT                 NOT NULL,
    [FechaCreacion]     DATETIME2(3)        NOT NULL,
    [FechaBaja]         DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_Miembro] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Artista_Miembro_Artista_Artista_Id] FOREIGN KEY ([Artista_Id]) REFERENCES [dbo].[Artista]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Artista_Miembro_Artista_Id] ON [dbo].[Artista_Miembro]([Artista_Id] ASC);
GO

CREATE TABLE [dbo].[FanProfile](
    [Id]                UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            NVARCHAR(450)       NOT NULL,
    [Apodo]             NVARCHAR(100)       NULL,
    [Pais]              NVARCHAR(100)       NULL,
    [Ciudad]            NVARCHAR(100)       NULL,
    [FechaCreacion]     DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_FanProfile] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_FanProfile_UserId] ON [dbo].[FanProfile]([UserId] ASC);
GO

CREATE TABLE [dbo].[Artista_Fan](
    [Id]                UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]        UNIQUEIDENTIFIER    NOT NULL,
    [FanProfile_Id]     UNIQUEIDENTIFIER    NOT NULL,
    [EsSuperFan]        BIT                 NOT NULL,
    [FechaInicio]       DATETIME2(3)        NOT NULL,
    [FechaFin]          DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_Fan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Artista_Fan_Artista_Artista_Id] FOREIGN KEY ([Artista_Id]) REFERENCES [dbo].[Artista]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Artista_Fan_FanProfile_FanProfile_Id] FOREIGN KEY ([FanProfile_Id]) REFERENCES [dbo].[FanProfile]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Artista_Fan_Artista_Id] ON [dbo].[Artista_Fan]([Artista_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_Artista_Fan_FanProfile_Id] ON [dbo].[Artista_Fan]([FanProfile_Id] ASC);
GO

CREATE TABLE [dbo].[PerfilProfesional](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                NVARCHAR(450)       NOT NULL,
    [Titulo]                NVARCHAR(200)       NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [TarifaHora]            DECIMAL(18,2)       NULL,
    [TarifaProyectoMin]     DECIMAL(18,2)       NULL,
    [Moneda_Id]             INT                 NULL,
    [UrlPortfolio]          NVARCHAR(300)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_PerfilProfesional] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[PerfilProfesional_Skill](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [PerfilProfesional_Id]  UNIQUEIDENTIFIER    NOT NULL,
    [TipoSkill_Id]          INT                 NOT NULL,
    [Nivel]                 TINYINT             NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_PerfilProfesional_Skill] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PerfilProfesional_Skill_PerfilProfesional_PerfilProfesional_Id] FOREIGN KEY ([PerfilProfesional_Id]) REFERENCES [dbo].[PerfilProfesional]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PerfilProfesional_Skill_PerfilProfesional_Id] ON [dbo].[PerfilProfesional_Skill]([PerfilProfesional_Id] ASC);
GO

CREATE TABLE [dbo].[PerfilProfesional_PortfolioItem](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [PerfilProfesional_Id]  UNIQUEIDENTIFIER    NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [UrlRecurso]            NVARCHAR(500)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_PerfilProfesional_PortfolioItem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PerfilProfesional_PortfolioItem_PerfilProfesional_PerfilProfesional_Id] FOREIGN KEY ([PerfilProfesional_Id]) REFERENCES [dbo].[PerfilProfesional]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PerfilProfesional_PortfolioItem_PerfilProfesional_Id] ON [dbo].[PerfilProfesional_PortfolioItem]([PerfilProfesional_Id] ASC);
GO

CREATE TABLE [dbo].[ProyectoArtistico](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [TipoProyecto_Id]       INT                 NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [DescripcionCorta]      NVARCHAR(500)       NULL,
    [DescripcionLarga]      NVARCHAR(MAX)       NULL,
    [UrlPortada]            NVARCHAR(300)       NULL,
    [EstadoProyecto_Id]     INT                 NOT NULL,
    [FechaInicio]           DATETIME2(3)        NULL,
    [FechaFinPrevista]      DATETIME2(3)        NULL,
    [FechaFinReal]          DATETIME2(3)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_ProyectoArtistico] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProyectoArtistico_Artista_Artista_Id] FOREIGN KEY ([Artista_Id]) REFERENCES [dbo].[Artista]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_ProyectoArtistico_Artista_Id] ON [dbo].[ProyectoArtistico]([Artista_Id] ASC);
GO


-- ============================================================
-- PARTE 4: CROWDFUNDING
-- Modulo: Crowdfunding
-- ============================================================

CREATE TABLE [dbo].[Artista_PayoutCuenta](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [NombreCuenta]          NVARCHAR(200)       NOT NULL,
    [TipoCuenta]            NVARCHAR(50)        NOT NULL,
    [ProveedorPayout]       NVARCHAR(50)        NULL,
    [IdentificadorExterno]  NVARCHAR(200)       NULL,
    [NombreTitular]         NVARCHAR(200)       NULL,
    [IBAN]                  NVARCHAR(34)        NULL,
    [Moneda_Id]             INT                 NULL,
    [EsPorDefecto]          BIT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaBaja]             DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_PayoutCuenta] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_Artista_PayoutCuenta_Artista_Id] ON [dbo].[Artista_PayoutCuenta]([Artista_Id] ASC, [EsPorDefecto] DESC);
GO

CREATE TABLE [dbo].[Artista_MembershipPlan](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [NombrePlan]            NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [ImporteMensual]        DECIMAL(18,2)       NOT NULL,
    [Moneda_Id]             INT                 NOT NULL,
    [Nivel]                 INT                 NOT NULL,
    [EsActivo]              BIT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_MembershipPlan] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Artista_MembershipPlan_Artista_NombrePlan] ON [dbo].[Artista_MembershipPlan]([Artista_Id] ASC, [NombrePlan] ASC);
GO

CREATE TABLE [dbo].[Artista_MembershipSuscripcion](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [MembershipPlan_Id]     UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                NVARCHAR(450)       NOT NULL,
    [FechaInicio]           DATETIME2(3)        NOT NULL,
    [FechaFin]              DATETIME2(3)        NULL,
    [ProximoCargo]          DATETIME2(3)        NULL,
    [EsActiva]              BIT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_MembershipSuscripcion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Artista_MembershipSuscripcion_Artista_MembershipPlan_MembershipPlan_Id] FOREIGN KEY ([MembershipPlan_Id]) REFERENCES [dbo].[Artista_MembershipPlan]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Artista_MembershipSuscripcion_Artista_User] ON [dbo].[Artista_MembershipSuscripcion]([Artista_Id] ASC, [UserId] ASC, [EsActiva] DESC);
GO
CREATE NONCLUSTERED INDEX [IX_Artista_MembershipSuscripcion_MembershipPlan_Id] ON [dbo].[Artista_MembershipSuscripcion]([MembershipPlan_Id] ASC);
GO

CREATE TABLE [dbo].[Artista_MembershipPago](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [MembershipSuscripcion_Id]  UNIQUEIDENTIFIER    NOT NULL,
    [Moneda_Id]                 INT                 NOT NULL,
    [MetodoPago_Id]             INT                 NOT NULL,
    [EstadoAportacion_Id]       INT                 NOT NULL,
    [ImporteTotal]              DECIMAL(18,2)       NOT NULL,
    [ImporteImpuestos]          DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPlataforma] DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPasarela]   DECIMAL(18,2)       NOT NULL,
    [ImporteNetoArtista]        DECIMAL(18,2)       NOT NULL,
    [CodigoOperacionPasarela]   NVARCHAR(100)       NULL,
    [CodigoOperacionProveedor]  NVARCHAR(100)       NULL,
    [FechaAutorizacion]         DATETIME2(3)        NULL,
    [FechaCaptura]              DATETIME2(3)        NULL,
    [FechaCancelacion]          DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_Artista_MembershipPago] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Artista_MembershipPago_Artista_MembershipSuscripcion_MembershipSuscripcion_Id] FOREIGN KEY ([MembershipSuscripcion_Id]) REFERENCES [dbo].[Artista_MembershipSuscripcion]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_Artista_MembershipPago_Suscripcion] ON [dbo].[Artista_MembershipPago]([MembershipSuscripcion_Id] ASC, [EstadoAportacion_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding](
    [Id]                            UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [ProyectoArtistico_Id]          UNIQUEIDENTIFIER    NULL,
    [Titulo]                        NVARCHAR(200)       NOT NULL,
    [Subtitulo]                     NVARCHAR(300)       NULL,
    [DescripcionCorta]              NVARCHAR(500)       NULL,
    [VideoPrincipalUrl]             NVARCHAR(500)       NULL,
    [ImagenPrincipalUrl]            NVARCHAR(500)       NULL,
    [Moneda_Id]                     INT                 NOT NULL,
    [ImporteObjetivo]               DECIMAL(18,2)       NOT NULL,
    [ImporteMinimo]                 DECIMAL(18,2)       NULL,
    [ImportePledgedActual]          DECIMAL(18,2)       NOT NULL,
    [TipoFinanciacion_Id]           INT                 NOT NULL,
    [EstadoCampania_Id]             INT                 NOT NULL,
    [PermiteAportacionesAnonimas]   BIT                 NOT NULL,
    [PermitePropinas]               BIT                 NOT NULL,
    [PorcentajeComisionPlataforma]  DECIMAL(5,2)        NULL,
    [FechaInicio]                   DATETIME2(3)        NULL,
    [FechaFin]                      DATETIME2(3)        NULL,
    [FechaPublicacion]              DATETIME2(3)        NULL,
    [FechaCierre]                   DATETIME2(3)        NULL,
    [Borrado]                       BIT                 NOT NULL,
    [FechaCreacion]                 DATETIME2(3)        NOT NULL,
    [FechaActualizacion]            DATETIME2(3)        NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Artista_Estado] ON [dbo].[CampaniaCrowdfunding]([Artista_Id] ASC, [EstadoCampania_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding_Update](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]           UNIQUEIDENTIFIER    NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Contenido]             NVARCHAR(MAX)       NOT NULL,
    [EsPublico]             BIT                 NOT NULL,
    [SoloBackers]           BIT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding_Update] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampaniaCrowdfunding_Update_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Update_Campania_Id] ON [dbo].[CampaniaCrowdfunding_Update]([Campania_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding_Comentario](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]           UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                NVARCHAR(450)       NOT NULL,
    [ComentarioPadre_Id]    UNIQUEIDENTIFIER    NULL,
    [Contenido]             NVARCHAR(MAX)       NOT NULL,
    [EsRespuestaArtista]    BIT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaActualizacion]    DATETIME2(3)        NULL,
    [Borrado]               BIT                 NOT NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding_Comentario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampaniaCrowdfunding_Comentario_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CampaniaCrowdfunding_Comentario_CampaniaCrowdfunding_Comentario_ComentarioPadre_Id] FOREIGN KEY ([ComentarioPadre_Id]) REFERENCES [dbo].[CampaniaCrowdfunding_Comentario]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Comentario_Campania_Id] ON [dbo].[CampaniaCrowdfunding_Comentario]([Campania_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Comentario_ComentarioPadre_Id] ON [dbo].[CampaniaCrowdfunding_Comentario]([ComentarioPadre_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding_StretchGoal](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]           UNIQUEIDENTIFIER    NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [ImporteObjetivo]       DECIMAL(18,2)       NOT NULL,
    [Moneda_Id]             INT                 NOT NULL,
    [Orden]                 INT                 NOT NULL,
    [Alcanzado]             BIT                 NOT NULL,
    [FechaAlcanzado]        DATETIME2(3)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding_StretchGoal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampaniaCrowdfunding_StretchGoal_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_StretchGoal_Campania_Id] ON [dbo].[CampaniaCrowdfunding_StretchGoal]([Campania_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding_Reward](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]               UNIQUEIDENTIFIER    NOT NULL,
    [TipoReward_Id]             INT                 NOT NULL,
    [Nombre]                    NVARCHAR(200)       NOT NULL,
    [Descripcion]               NVARCHAR(MAX)       NULL,
    [ImporteMinimo]             DECIMAL(18,2)       NOT NULL,
    [Moneda_Id]                 INT                 NOT NULL,
    [EsAddOn]                   BIT                 NOT NULL,
    [CantidadMaxima]            INT                 NULL,
    [CantidadPorBacker]         INT                 NULL,
    [IncluyeEnvioFisico]        BIT                 NOT NULL,
    [TiempoEntregaEstimado]     NVARCHAR(200)       NULL,
    [Orden]                     INT                 NOT NULL,
    [EsActivo]                  BIT                 NOT NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding_Reward] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampaniaCrowdfunding_Reward_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Reward_Campania_Orden] ON [dbo].[CampaniaCrowdfunding_Reward]([Campania_Id] ASC, [Orden] ASC);
GO

CREATE TABLE [dbo].[PedidoCrowdfunding](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]               UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                    NVARCHAR(450)       NULL,
    [FanProfile_Id]             UNIQUEIDENTIFIER    NULL,
    [EstadoPedido_Id]           INT                 NOT NULL,
    [Moneda_Id]                 INT                 NOT NULL,
    [ImporteSubtotal]           DECIMAL(18,2)       NOT NULL,
    [ImportePropina]            DECIMAL(18,2)       NOT NULL,
    [ImporteEnvio]              DECIMAL(18,2)       NOT NULL,
    [ImporteImpuestos]          DECIMAL(18,2)       NOT NULL,
    [ImporteTotal]              DECIMAL(18,2)       NOT NULL,
    [PermitirMostrarNombre]     BIT                 NOT NULL,
    [ComentarioBacker]          NVARCHAR(500)       NULL,
    [DireccionEnvio_Id]         UNIQUEIDENTIFIER    NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_PedidoCrowdfunding] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PedidoCrowdfunding_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_PedidoCrowdfunding_Campania_Estado] ON [dbo].[PedidoCrowdfunding]([Campania_Id] ASC, [EstadoPedido_Id] ASC);
GO

CREATE TABLE [dbo].[PedidoCrowdfunding_Linea](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [PedidoCrowdfunding_Id]     UNIQUEIDENTIFIER    NOT NULL,
    [Reward_Id]                 UNIQUEIDENTIFIER    NOT NULL,
    [Cantidad]                  INT                 NOT NULL,
    [PrecioUnitario]            DECIMAL(18,2)       NOT NULL,
    [ImporteLinea]              DECIMAL(18,2)       NOT NULL,
    [EsRewardPrincipal]         BIT                 NOT NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_PedidoCrowdfunding_Linea] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PedidoCrowdfunding_Linea_CampaniaCrowdfunding_Reward_Reward_Id] FOREIGN KEY ([Reward_Id]) REFERENCES [dbo].[CampaniaCrowdfunding_Reward]([Id]),
    CONSTRAINT [FK_PedidoCrowdfunding_Linea_PedidoCrowdfunding_PedidoCrowdfunding_Id] FOREIGN KEY ([PedidoCrowdfunding_Id]) REFERENCES [dbo].[PedidoCrowdfunding]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PedidoCrowdfunding_Linea_Pedido] ON [dbo].[PedidoCrowdfunding_Linea]([PedidoCrowdfunding_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_PedidoCrowdfunding_Linea_Reward_Id] ON [dbo].[PedidoCrowdfunding_Linea]([Reward_Id] ASC);
GO

CREATE TABLE [dbo].[AportacionCrowdfunding](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [PedidoCrowdfunding_Id]     UNIQUEIDENTIFIER    NOT NULL,
    [Moneda_Id]                 INT                 NOT NULL,
    [MetodoPago_Id]             INT                 NOT NULL,
    [EstadoAportacion_Id]       INT                 NOT NULL,
    [ImporteTotal]              DECIMAL(18,2)       NOT NULL,
    [ImporteImpuestos]          DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPlataforma] DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPasarela]   DECIMAL(18,2)       NOT NULL,
    [ImporteNetoArtista]        DECIMAL(18,2)       NOT NULL,
    [CodigoOperacionPasarela]   NVARCHAR(100)       NULL,
    [CodigoOperacionProveedor]  NVARCHAR(100)       NULL,
    [FechaAutorizacion]         DATETIME2(3)        NULL,
    [FechaCaptura]              DATETIME2(3)        NULL,
    [FechaCancelacion]          DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_AportacionCrowdfunding] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AportacionCrowdfunding_PedidoCrowdfunding_PedidoCrowdfunding_Id] FOREIGN KEY ([PedidoCrowdfunding_Id]) REFERENCES [dbo].[PedidoCrowdfunding]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AportacionCrowdfunding_Pedido_Estado] ON [dbo].[AportacionCrowdfunding]([PedidoCrowdfunding_Id] ASC, [EstadoAportacion_Id] ASC);
GO

CREATE TABLE [dbo].[CampaniaCrowdfunding_Payout](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [Campania_Id]               UNIQUEIDENTIFIER    NOT NULL,
    [ArtistaPayoutCuenta_Id]    UNIQUEIDENTIFIER    NOT NULL,
    [Moneda_Id]                 INT                 NOT NULL,
    [ImporteBruto]              DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPlataforma] DECIMAL(18,2)       NOT NULL,
    [ImporteComisionPasarela]   DECIMAL(18,2)       NOT NULL,
    [ImporteImpuestosRetenidos] DECIMAL(18,2)       NOT NULL,
    [ImporteNetoArtista]        DECIMAL(18,2)       NOT NULL,
    [EstadoPayout_Id]           INT                 NOT NULL,
    [FechaProgramada]           DATETIME2(3)        NULL,
    [FechaEjecucion]            DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    [NotasInternas]             NVARCHAR(500)       NULL,
    CONSTRAINT [PK_CampaniaCrowdfunding_Payout] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CampaniaCrowdfunding_Payout_Artista_PayoutCuenta_ArtistaPayoutCuenta_Id] FOREIGN KEY ([ArtistaPayoutCuenta_Id]) REFERENCES [dbo].[Artista_PayoutCuenta]([Id]),
    CONSTRAINT [FK_CampaniaCrowdfunding_Payout_CampaniaCrowdfunding_Campania_Id] FOREIGN KEY ([Campania_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Payout_ArtistaPayoutCuenta_Id] ON [dbo].[CampaniaCrowdfunding_Payout]([ArtistaPayoutCuenta_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_CampaniaCrowdfunding_Payout_Campania_Estado] ON [dbo].[CampaniaCrowdfunding_Payout]([Campania_Id] ASC, [EstadoPayout_Id] ASC);
GO
