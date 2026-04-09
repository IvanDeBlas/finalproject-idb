-- ============================================================
-- WePlay Rises - Crowdsourcing
-- Actualizado: 2026-02-16
-- Generado desde EF Core Migrations (CrowdsourcingContext)
-- ============================================================
-- Incluye:
--   - Maestras propias del modulo (CategoriaRol, RolProfesional)
--   - Plantillas de proyecto
--   - NecesidadCrowdsourcing + PropuestaCrowdsourcing
--   - AcuerdoCrowdsourcing + Milestones + Entregables
--   - Conversaciones + Mensajes
--   - Valoraciones
-- ============================================================

--	DROP TABLE [dbo].[MensajeCrowdsourcing]
--	DROP TABLE [dbo].[ConversacionCrowdsourcing]
--	DROP TABLE [dbo].[ValoracionCrowdsourcing]
--	DROP TABLE [dbo].[AcuerdoCrowdsourcing_Entregable]
--	DROP TABLE [dbo].[AcuerdoCrowdsourcing_Milestone]
--	DROP TABLE [dbo].[AcuerdoCrowdsourcing]
--	DROP TABLE [dbo].[PropuestaCrowdsourcing]
--	DROP TABLE [dbo].[NecesidadCrowdsourcing]
--	DROP TABLE [dbo].[PlantillasProyectoNecesidades]
--	DROP TABLE [dbo].[MaestrasRolProfesional]
--	DROP TABLE [dbo].[MaestrasCategoriaRol]
--	DROP TABLE [dbo].[PlantillasProyecto]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- PARTE 5A: MAESTRAS CROWDSOURCING
-- ============================================================

CREATE TABLE [dbo].[MaestrasCategoriaRol](
    [Id]        INT             IDENTITY(1,1) NOT NULL,
    [Nombre]    NVARCHAR(100)   NOT NULL,
    [Icono]     NVARCHAR(50)    NULL,
    [Orden]     INT             NOT NULL,
    CONSTRAINT [PK_MaestrasCategoriaRol] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_MaestrasCategoriaRol_Orden] ON [dbo].[MaestrasCategoriaRol]([Orden] ASC);
GO

CREATE TABLE [dbo].[MaestrasRolProfesional](
    [Id]                INT             IDENTITY(1,1) NOT NULL,
    [Nombre]            NVARCHAR(100)   NOT NULL,
    [Descripcion]       NVARCHAR(500)   NULL,
    [CategoriaRolId]    INT             NOT NULL,
    [ModalidadCobro]    NVARCHAR(100)   NULL,
    [Activo]            BIT             NOT NULL DEFAULT(1),
    CONSTRAINT [PK_MaestrasRolProfesional] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaestrasRolProfesional_MaestrasCategoriaRol_CategoriaRolId] FOREIGN KEY ([CategoriaRolId]) REFERENCES [dbo].[MaestrasCategoriaRol]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_MaestrasRolProfesional_Activo] ON [dbo].[MaestrasRolProfesional]([Activo] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_MaestrasRolProfesional_CategoriaRolId] ON [dbo].[MaestrasRolProfesional]([CategoriaRolId] ASC);
GO

-- ============================================================
-- PARTE 5B: PLANTILLAS DE PROYECTO
-- ============================================================

CREATE TABLE [dbo].[PlantillasProyecto](
    [Id]            UNIQUEIDENTIFIER    NOT NULL,
    [Nombre]        NVARCHAR(200)       NOT NULL,
    [Descripcion]   NVARCHAR(1000)      NULL,
    [Icono]         NVARCHAR(50)        NULL,
    [Orden]         INT                 NOT NULL,
    [Activo]        BIT                 NOT NULL DEFAULT(1),
    [FechaCreacion] DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_PlantillasProyecto] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_PlantillasProyecto_Activo] ON [dbo].[PlantillasProyecto]([Activo] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_PlantillasProyecto_Orden] ON [dbo].[PlantillasProyecto]([Orden] ASC);
GO

CREATE TABLE [dbo].[PlantillasProyectoNecesidades](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [PlantillaProyectoId]   UNIQUEIDENTIFIER    NOT NULL,
    [Fase]                  NVARCHAR(100)       NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(1000)      NULL,
    [RolProfesionalId]      INT                 NOT NULL,
    [PrecioMinOrientativo]  DECIMAL(18,2)       NULL,
    [PrecioMaxOrientativo]  DECIMAL(18,2)       NULL,
    [MonedaId]              INT                 NOT NULL DEFAULT(1),
    [Prioridad]             NVARCHAR(20)        NOT NULL DEFAULT(N'Media'),
    [Orden]                 INT                 NOT NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_PlantillasProyectoNecesidades] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PlantillasProyectoNecesidades_MaestrasRolProfesional_RolProfesionalId] FOREIGN KEY ([RolProfesionalId]) REFERENCES [dbo].[MaestrasRolProfesional]([Id]),
    CONSTRAINT [FK_PlantillasProyectoNecesidades_PlantillasProyecto_PlantillaProyectoId] FOREIGN KEY ([PlantillaProyectoId]) REFERENCES [dbo].[PlantillasProyecto]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PlantillasProyectoNecesidades_PlantillaProyectoId] ON [dbo].[PlantillasProyectoNecesidades]([PlantillaProyectoId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_PlantillasProyectoNecesidades_Prioridad] ON [dbo].[PlantillasProyectoNecesidades]([Prioridad] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_PlantillasProyectoNecesidades_RolProfesionalId] ON [dbo].[PlantillasProyectoNecesidades]([RolProfesionalId] ASC);
GO

-- ============================================================
-- PARTE 5C: CROWDSOURCING - TABLAS PRINCIPALES
-- ============================================================

CREATE TABLE [dbo].[NecesidadCrowdsourcing](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [ProyectoArtistico_Id]      UNIQUEIDENTIFIER    NOT NULL,
    [Artista_Id]                UNIQUEIDENTIFIER    NOT NULL,
    [Titulo]                    NVARCHAR(200)       NOT NULL,
    [Descripcion]               NVARCHAR(MAX)       NULL,
    [TipoNecesidad_Id]          INT                 NOT NULL,
    [EstadoNecesidad_Id]        INT                 NOT NULL,
    [ModalidadTrabajo_Id]       INT                 NOT NULL,
    [PresupuestoMin]            DECIMAL(18,2)       NULL,
    [PresupuestoMax]            DECIMAL(18,2)       NULL,
    [Moneda_Id]                 INT                 NULL,
    [UbicacionCiudad]           NVARCHAR(100)       NULL,
    [UbicacionPais]             NVARCHAR(100)       NULL,
    [FechaLimitePropuestas]     DATETIME2(3)        NULL,
    [FechaInicioPrevista]       DATETIME2(3)        NULL,
    [MotivoCierre]              NVARCHAR(500)       NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_NecesidadCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_NecesidadCrowdsourcing_Artista_Estado] ON [dbo].[NecesidadCrowdsourcing]([Artista_Id] ASC, [EstadoNecesidad_Id] ASC);
GO

CREATE TABLE [dbo].[PropuestaCrowdsourcing](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [Necesidad_Id]              UNIQUEIDENTIFIER    NOT NULL,
    [UserId]                    NVARCHAR(450)       NOT NULL,
    [PerfilProfesional_Id]      UNIQUEIDENTIFIER    NULL,
    [MensajePropuesta]          NVARCHAR(MAX)       NULL,
    [PrecioPropuesto]           DECIMAL(18,2)       NOT NULL,
    [Moneda_Id]                 INT                 NULL,
    [DiasEstimados]             INT                 NULL,
    [EstadoPropuesta_Id]        INT                 NOT NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_PropuestaCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PropuestaCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id] FOREIGN KEY ([Necesidad_Id]) REFERENCES [dbo].[NecesidadCrowdsourcing]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_PropuestaCrowdsourcing_Necesidad_Id] ON [dbo].[PropuestaCrowdsourcing]([Necesidad_Id] ASC);
GO

CREATE TABLE [dbo].[AcuerdoCrowdsourcing](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL,
    [Necesidad_Id]              UNIQUEIDENTIFIER    NOT NULL,
    [Propuesta_Id]              UNIQUEIDENTIFIER    NULL,
    [Artista_Id]                UNIQUEIDENTIFIER    NOT NULL,
    [UserIdProveedor]           NVARCHAR(450)       NOT NULL,
    [PerfilProfesional_Id]      UNIQUEIDENTIFIER    NULL,
    [TituloInterno]             NVARCHAR(200)       NULL,
    [Descripcion]               NVARCHAR(MAX)       NULL,
    [EstadoAcuerdo_Id]          INT                 NOT NULL,
    [Moneda_Id]                 INT                 NULL,
    [ImporteTotalPactado]       DECIMAL(18,2)       NOT NULL,
    [ImporteAnticipo]           DECIMAL(18,2)       NULL,
    [PorcentajeAnticipo]        DECIMAL(5,2)        NULL,
    [FechaInicio]               DATETIME2(3)        NULL,
    [FechaFinPrevista]          DATETIME2(3)        NULL,
    [FechaFinReal]              DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL,
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_AcuerdoCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AcuerdoCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id] FOREIGN KEY ([Necesidad_Id]) REFERENCES [dbo].[NecesidadCrowdsourcing]([Id]),
    CONSTRAINT [FK_AcuerdoCrowdsourcing_PropuestaCrowdsourcing_Propuesta_Id] FOREIGN KEY ([Propuesta_Id]) REFERENCES [dbo].[PropuestaCrowdsourcing]([Id]) ON DELETE SET NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Artista_Estado] ON [dbo].[AcuerdoCrowdsourcing]([Artista_Id] ASC, [EstadoAcuerdo_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Necesidad] ON [dbo].[AcuerdoCrowdsourcing]([Necesidad_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Propuesta_Id] ON [dbo].[AcuerdoCrowdsourcing]([Propuesta_Id] ASC);
GO

CREATE TABLE [dbo].[AcuerdoCrowdsourcing_Milestone](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Acuerdo_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [Orden]                 INT                 NOT NULL,
    [ImporteParcial]        DECIMAL(18,2)       NOT NULL,
    [PorcentajeParcial]     DECIMAL(5,2)        NULL,
    [FechaLimite]           DATETIME2(3)        NULL,
    [FechaCompletado]       DATETIME2(3)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_AcuerdoCrowdsourcing_Milestone] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AcuerdoCrowdsourcing_Milestone_AcuerdoCrowdsourcing_Acuerdo_Id] FOREIGN KEY ([Acuerdo_Id]) REFERENCES [dbo].[AcuerdoCrowdsourcing]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Milestone_Acuerdo_Id] ON [dbo].[AcuerdoCrowdsourcing_Milestone]([Acuerdo_Id] ASC);
GO

CREATE TABLE [dbo].[AcuerdoCrowdsourcing_Entregable](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Acuerdo_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [Milestone_Id]          UNIQUEIDENTIFIER    NULL,
    [Titulo]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [UrlRecurso]            NVARCHAR(500)       NULL,
    [EstadoEntregable_Id]   INT                 NOT NULL,
    [FechaEntrega]          DATETIME2(3)        NULL,
    [FechaAprobacion]       DATETIME2(3)        NULL,
    [ComentarioAprobacion]  NVARCHAR(MAX)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_AcuerdoCrowdsourcing_Entregable] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AcuerdoCrowdsourcing_Entregable_AcuerdoCrowdsourcing_Acuerdo_Id] FOREIGN KEY ([Acuerdo_Id]) REFERENCES [dbo].[AcuerdoCrowdsourcing]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AcuerdoCrowdsourcing_Entregable_AcuerdoCrowdsourcing_Milestone_Milestone_Id] FOREIGN KEY ([Milestone_Id]) REFERENCES [dbo].[AcuerdoCrowdsourcing_Milestone]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Entregable_Acuerdo_Id] ON [dbo].[AcuerdoCrowdsourcing_Entregable]([Acuerdo_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_AcuerdoCrowdsourcing_Entregable_Milestone_Id] ON [dbo].[AcuerdoCrowdsourcing_Entregable]([Milestone_Id] ASC);
GO

CREATE TABLE [dbo].[ValoracionCrowdsourcing](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Acuerdo_Id]            UNIQUEIDENTIFIER    NOT NULL,
    [UserIdAutor]           NVARCHAR(450)       NOT NULL,
    [UserIdValorado]        NVARCHAR(450)       NOT NULL,
    [TipoValoracion_Id]     INT                 NOT NULL,
    [Puntuacion]            TINYINT             NOT NULL,
    [Comentario]            NVARCHAR(MAX)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_ValoracionCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ValoracionCrowdsourcing_AcuerdoCrowdsourcing_Acuerdo_Id] FOREIGN KEY ([Acuerdo_Id]) REFERENCES [dbo].[AcuerdoCrowdsourcing]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_ValoracionCrowdsourcing_Acuerdo_Id] ON [dbo].[ValoracionCrowdsourcing]([Acuerdo_Id] ASC);
GO

CREATE TABLE [dbo].[ConversacionCrowdsourcing](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Necesidad_Id]          UNIQUEIDENTIFIER    NULL,
    [Acuerdo_Id]            UNIQUEIDENTIFIER    NULL,
    [UserIdArtista]         NVARCHAR(450)       NOT NULL,
    [UserIdProveedor]       NVARCHAR(450)       NOT NULL,
    [Asunto]                NVARCHAR(200)       NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    [FechaUltimoMensaje]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_ConversacionCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConversacionCrowdsourcing_AcuerdoCrowdsourcing_Acuerdo_Id] FOREIGN KEY ([Acuerdo_Id]) REFERENCES [dbo].[AcuerdoCrowdsourcing]([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_ConversacionCrowdsourcing_NecesidadCrowdsourcing_Necesidad_Id] FOREIGN KEY ([Necesidad_Id]) REFERENCES [dbo].[NecesidadCrowdsourcing]([Id]) ON DELETE SET NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_ConversacionCrowdsourcing_Necesidad_Id] ON [dbo].[ConversacionCrowdsourcing]([Necesidad_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_ConversacionCrowdsourcing_Acuerdo_Id] ON [dbo].[ConversacionCrowdsourcing]([Acuerdo_Id] ASC);
GO

CREATE TABLE [dbo].[MensajeCrowdsourcing](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL,
    [Conversacion_Id]       UNIQUEIDENTIFIER    NOT NULL,
    [UserIdRemitente]       NVARCHAR(450)       NOT NULL,
    [Contenido]             NVARCHAR(MAX)       NOT NULL,
    [UrlAdjunto]            NVARCHAR(500)       NULL,
    [Leido]                 BIT                 NOT NULL,
    [FechaLeido]            DATETIME2(3)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL,
    CONSTRAINT [PK_MensajeCrowdsourcing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MensajeCrowdsourcing_ConversacionCrowdsourcing_Conversacion_Id] FOREIGN KEY ([Conversacion_Id]) REFERENCES [dbo].[ConversacionCrowdsourcing]([Id]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_MensajeCrowdsourcing_Conversacion_Id] ON [dbo].[MensajeCrowdsourcing]([Conversacion_Id] ASC);
GO
