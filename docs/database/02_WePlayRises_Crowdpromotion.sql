-- ============================================================
-- WePlay Rises - Crowdpromotion
-- ============================================================

--	DROP TABLE [dbo].[Promotor_WalletTransaccion]
--	DROP TABLE [dbo].[PromoEvento]
--	DROP TABLE [dbo].[PromoTarea_Promotor]
--	DROP TABLE [dbo].[PromoTarea]
--	DROP TABLE [dbo].[PromoPrograma_Promotor]
--	DROP TABLE [dbo].[PromoPrograma]
--	DROP TABLE [dbo].[Promotor_Wallet]
--	DROP TABLE [dbo].[Promotor]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- PARTE 6: CROWDPROMOTION
-- ============================================================

CREATE TABLE [dbo].[Promotor](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [TipoPromotor_Id]       INT                 NOT NULL,
    [UserId]                NVARCHAR(450)       NULL,
    [FanProfile_Id]         UNIQUEIDENTIFIER    NULL,
    [NombrePublico]         NVARCHAR(200)       NOT NULL,
    [EmailContacto]         NVARCHAR(200)       NULL,
    [UrlSitioWeb]           NVARCHAR(300)       NULL,
    [UrlInstagram]          NVARCHAR(300)       NULL,
    [UrlTikTok]             NVARCHAR(300)       NULL,
    [UrlYouTube]            NVARCHAR(300)       NULL,
    [UrlTwitter]            NVARCHAR(300)       NULL,
    [EsActivo]              BIT                 NOT NULL CONSTRAINT [DF_Promotor_EsActivo] DEFAULT(1),
    [FechaCreacion]         DATETIME2(3)        NOT NULL CONSTRAINT [DF_Promotor_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_Promotor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Promotor_dbo.Maestra_TipoPromotor] FOREIGN KEY ([TipoPromotor_Id]) REFERENCES [dbo].[Maestra_TipoPromotor]([Id]),
    CONSTRAINT [FK_Promotor_dbo.Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    CONSTRAINT [FK_Promotor_dbo.FanProfile] FOREIGN KEY ([FanProfile_Id]) REFERENCES [dbo].[FanProfile]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Promotor_Tipo_Activo] ON [dbo].[Promotor]([TipoPromotor_Id] ASC, [EsActivo] DESC);
GO

CREATE TABLE [dbo].[Promotor_Wallet](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Promotor_Id]           UNIQUEIDENTIFIER    NOT NULL,
    [Moneda_Id]             INT                 NOT NULL,
    [SaldoActual]           DECIMAL(18,2)       NOT NULL CONSTRAINT [DF_Promotor_Wallet_SaldoActual] DEFAULT(0),
    [FechaCreacion]         DATETIME2(3)        NOT NULL CONSTRAINT [DF_Promotor_Wallet_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_Promotor_Wallet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Promotor_Wallet_dbo.Promotor] FOREIGN KEY ([Promotor_Id]) REFERENCES [dbo].[Promotor]([Id]),
    CONSTRAINT [FK_Promotor_Wallet_dbo.Maestra_Moneda] FOREIGN KEY ([Moneda_Id]) REFERENCES [dbo].[Maestra_Moneda]([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Promotor_Wallet_Promotor_Moneda] ON [dbo].[Promotor_Wallet]([Promotor_Id] ASC, [Moneda_Id] ASC);
GO

CREATE TABLE [dbo].[PromoPrograma](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Artista_Id]                UNIQUEIDENTIFIER    NOT NULL,
    [ProyectoArtistico_Id]      UNIQUEIDENTIFIER    NULL,
    [CampaniaCrowdfunding_Id]   UNIQUEIDENTIFIER    NULL,
    [TipoPromo_Id]              INT                 NOT NULL,
    [Titulo]                    NVARCHAR(200)       NOT NULL,
    [Descripcion]               NVARCHAR(MAX)       NULL,
    [UrlLanding]                NVARCHAR(500)       NULL,
    [CodigoTrackingBase]        NVARCHAR(50)        NULL,
    [Moneda_Id]                 INT                 NULL,
    [ImporteComisionPorcentaje] DECIMAL(5,2)        NULL,
    [ImporteComisionFija]       DECIMAL(18,2)       NULL,
    [EsActivo]                  BIT                 NOT NULL CONSTRAINT [DF_PromoPrograma_EsActivo] DEFAULT(1),
    [FechaInicio]               DATETIME2(3)        NULL,
    [FechaFin]                  DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL CONSTRAINT [DF_PromoPrograma_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_PromoPrograma] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromoPrograma_dbo.Artista] FOREIGN KEY ([Artista_Id]) REFERENCES [dbo].[Artista]([Id]),
    CONSTRAINT [FK_PromoPrograma_dbo.ProyectoArtistico] FOREIGN KEY ([ProyectoArtistico_Id]) REFERENCES [dbo].[ProyectoArtistico]([Id]),
    CONSTRAINT [FK_PromoPrograma_dbo.CampaniaCrowdfunding] FOREIGN KEY ([CampaniaCrowdfunding_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]),
    CONSTRAINT [FK_PromoPrograma_dbo.Maestra_TipoPromo] FOREIGN KEY ([TipoPromo_Id]) REFERENCES [dbo].[Maestra_TipoPromo]([Id]),
    CONSTRAINT [FK_PromoPrograma_dbo.Maestra_Moneda] FOREIGN KEY ([Moneda_Id]) REFERENCES [dbo].[Maestra_Moneda]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_PromoPrograma_Artista_Activo] ON [dbo].[PromoPrograma]([Artista_Id] ASC, [EsActivo] DESC);
GO

CREATE TABLE [dbo].[PromoPrograma_Promotor](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PromoPrograma_Id]          UNIQUEIDENTIFIER    NOT NULL,
    [Promotor_Id]               UNIQUEIDENTIFIER    NOT NULL,
    [CodigoReferido]            NVARCHAR(50)        NULL,
    [UrlTrackingPersonalizada]  NVARCHAR(500)       NULL,
    [EsAprobado]                BIT                 NOT NULL CONSTRAINT [DF_PromoPrograma_Promotor_EsAprobado] DEFAULT(1),
    [EsBloqueado]               BIT                 NOT NULL CONSTRAINT [DF_PromoPrograma_Promotor_EsBloqueado] DEFAULT(0),
    [FechaAlta]                 DATETIME2(3)        NOT NULL CONSTRAINT [DF_PromoPrograma_Promotor_FechaAlta] DEFAULT SYSUTCDATETIME(),
    [FechaBaja]                 DATETIME2(3)        NULL,
    CONSTRAINT [PK_PromoPrograma_Promotor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromoPrograma_Promotor_dbo.PromoPrograma] FOREIGN KEY ([PromoPrograma_Id]) REFERENCES [dbo].[PromoPrograma]([Id]),
    CONSTRAINT [FK_PromoPrograma_Promotor_dbo.Promotor] FOREIGN KEY ([Promotor_Id]) REFERENCES [dbo].[Promotor]([Id])
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PromoPrograma_Promotor_Programa_Promotor] ON [dbo].[PromoPrograma_Promotor]([PromoPrograma_Id] ASC, [Promotor_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_PromoPrograma_Promotor_Promotor_Activo] ON [dbo].[PromoPrograma_Promotor]([Promotor_Id] ASC, [EsAprobado] DESC, [EsBloqueado] ASC);
GO

CREATE TABLE [dbo].[PromoTarea](
    [Id]                    UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PromoPrograma_Id]      UNIQUEIDENTIFIER    NOT NULL,
    [TipoEventoPromo_Id]    INT                 NOT NULL,
    [TipoReward_Id]         INT                 NOT NULL,
    [Nombre]                NVARCHAR(200)       NOT NULL,
    [Descripcion]           NVARCHAR(MAX)       NULL,
    [UrlInstrucciones]      NVARCHAR(500)       NULL,
    [Moneda_Id]             INT                 NULL,
    [ImporteReward]         DECIMAL(18,2)       NULL,
    [PuntosReward]          INT                 NULL,
    [EsRepetible]           BIT                 NOT NULL CONSTRAINT [DF_PromoTarea_EsRepetible] DEFAULT(1),
    [MaxRepeticiones]       INT                 NULL,
    [EsActivo]              BIT                 NOT NULL CONSTRAINT [DF_PromoTarea_EsActivo] DEFAULT(1),
    [FechaInicio]           DATETIME2(3)        NULL,
    [FechaFin]              DATETIME2(3)        NULL,
    [FechaCreacion]         DATETIME2(3)        NOT NULL CONSTRAINT [DF_PromoTarea_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]    DATETIME2(3)        NULL,
    CONSTRAINT [PK_PromoTarea] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromoTarea_dbo.PromoPrograma] FOREIGN KEY ([PromoPrograma_Id]) REFERENCES [dbo].[PromoPrograma]([Id]),
    CONSTRAINT [FK_PromoTarea_dbo.Maestra_TipoEventoPromo] FOREIGN KEY ([TipoEventoPromo_Id]) REFERENCES [dbo].[Maestra_TipoEventoPromo]([Id]),
    CONSTRAINT [FK_PromoTarea_dbo.Maestra_TipoReward] FOREIGN KEY ([TipoReward_Id]) REFERENCES [dbo].[Maestra_TipoReward]([Id]),
    CONSTRAINT [FK_PromoTarea_dbo.Maestra_Moneda] FOREIGN KEY ([Moneda_Id]) REFERENCES [dbo].[Maestra_Moneda]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_PromoTarea_Programa_Activo] ON [dbo].[PromoTarea]([PromoPrograma_Id] ASC, [EsActivo] DESC);
GO

CREATE TABLE [dbo].[PromoTarea_Promotor](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PromoTarea_Id]             UNIQUEIDENTIFIER    NOT NULL,
    [PromoPrograma_Promotor_Id] UNIQUEIDENTIFIER    NOT NULL,
    [EstadoTarea_Id]            INT                 NOT NULL,
    [VecesCompletada]           INT                 NOT NULL CONSTRAINT [DF_PromoTarea_Promotor_VecesCompletada] DEFAULT(0),
    [FechaPrimeraCompletada]    DATETIME2(3)        NULL,
    [FechaUltimaCompletada]     DATETIME2(3)        NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL CONSTRAINT [DF_PromoTarea_Promotor_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_PromoTarea_Promotor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromoTarea_Promotor_dbo.PromoTarea] FOREIGN KEY ([PromoTarea_Id]) REFERENCES [dbo].[PromoTarea]([Id]),
    CONSTRAINT [FK_PromoTarea_Promotor_dbo.PromoPrograma_Promotor] FOREIGN KEY ([PromoPrograma_Promotor_Id]) REFERENCES [dbo].[PromoPrograma_Promotor]([Id]),
    CONSTRAINT [FK_PromoTarea_Promotor_dbo.Maestra_EstadoTareaPromo] FOREIGN KEY ([EstadoTarea_Id]) REFERENCES [dbo].[Maestra_EstadoTareaPromo]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_PromoTarea_Promotor_Tarea_Estado] ON [dbo].[PromoTarea_Promotor]([PromoTarea_Id] ASC, [EstadoTarea_Id] ASC);
GO

CREATE TABLE [dbo].[PromoEvento](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [PromoPrograma_Id]          UNIQUEIDENTIFIER    NOT NULL,
    [PromoPrograma_Promotor_Id] UNIQUEIDENTIFIER    NOT NULL,
    [TipoEventoPromo_Id]        INT                 NOT NULL,
    [CampaniaCrowdfunding_Id]   UNIQUEIDENTIFIER    NULL,
    [PedidoCrowdfunding_Id]     UNIQUEIDENTIFIER    NULL,
    [AportacionCrowdfunding_Id] UNIQUEIDENTIFIER    NULL,
    [UserIdAfectado]            NVARCHAR(450)       NULL,
    [UrlOrigen]                 NVARCHAR(500)       NULL,
    [UrlReferer]                NVARCHAR(500)       NULL,
    [UtmSource]                 NVARCHAR(100)       NULL,
    [UtmMedium]                 NVARCHAR(100)       NULL,
    [UtmCampaign]               NVARCHAR(100)       NULL,
    [ValorMonetario]            DECIMAL(18,2)       NULL,
    [Moneda_Id]                 INT                 NULL,
    [FechaEvento]               DATETIME2(3)        NOT NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL CONSTRAINT [DF_PromoEvento_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_PromoEvento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromoEvento_dbo.PromoPrograma] FOREIGN KEY ([PromoPrograma_Id]) REFERENCES [dbo].[PromoPrograma]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.PromoPrograma_Promotor] FOREIGN KEY ([PromoPrograma_Promotor_Id]) REFERENCES [dbo].[PromoPrograma_Promotor]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.Maestra_TipoEventoPromo] FOREIGN KEY ([TipoEventoPromo_Id]) REFERENCES [dbo].[Maestra_TipoEventoPromo]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.CampaniaCrowdfunding] FOREIGN KEY ([CampaniaCrowdfunding_Id]) REFERENCES [dbo].[CampaniaCrowdfunding]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.PedidoCrowdfunding] FOREIGN KEY ([PedidoCrowdfunding_Id]) REFERENCES [dbo].[PedidoCrowdfunding]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.AportacionCrowdfunding] FOREIGN KEY ([AportacionCrowdfunding_Id]) REFERENCES [dbo].[AportacionCrowdfunding]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.Users_Afectado] FOREIGN KEY ([UserIdAfectado]) REFERENCES [dbo].[Users]([Id]),
    CONSTRAINT [FK_PromoEvento_dbo.Maestra_Moneda] FOREIGN KEY ([Moneda_Id]) REFERENCES [dbo].[Maestra_Moneda]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_PromoEvento_Programa_Tipo_Fecha] ON [dbo].[PromoEvento]([PromoPrograma_Id] ASC, [TipoEventoPromo_Id] ASC, [FechaEvento] DESC);
GO
CREATE NONCLUSTERED INDEX [IX_PromoEvento_ProgramaPromotor_Fecha] ON [dbo].[PromoEvento]([PromoPrograma_Promotor_Id] ASC, [FechaEvento] DESC);
GO

CREATE TABLE [dbo].[Promotor_WalletTransaccion](
    [Id]                        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Wallet_Id]                 UNIQUEIDENTIFIER    NOT NULL,
    [EstadoWalletTransaccion_Id] INT                NOT NULL,
    [TipoReward_Id]             INT                 NULL,
    [PromoEvento_Id]            UNIQUEIDENTIFIER    NULL,
    [CampaniaPayout_Id]         UNIQUEIDENTIFIER    NULL,
    [EsCredito]                 BIT                 NOT NULL CONSTRAINT [DF_Promotor_WalletTransaccion_EsCredito] DEFAULT(1),
    [Importe]                   DECIMAL(18,2)       NOT NULL,
    [Descripcion]               NVARCHAR(500)       NULL,
    [FechaCreacion]             DATETIME2(3)        NOT NULL CONSTRAINT [DF_Promotor_WalletTransaccion_FechaCreacion] DEFAULT SYSUTCDATETIME(),
    [FechaActualizacion]        DATETIME2(3)        NULL,
    CONSTRAINT [PK_Promotor_WalletTransaccion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Promotor_WalletTransaccion_dbo.Promotor_Wallet] FOREIGN KEY ([Wallet_Id]) REFERENCES [dbo].[Promotor_Wallet]([Id]),
    CONSTRAINT [FK_Promotor_WalletTransaccion_dbo.Maestra_EstadoWalletTransaccion] FOREIGN KEY ([EstadoWalletTransaccion_Id]) REFERENCES [dbo].[Maestra_EstadoWalletTransaccion]([Id]),
    CONSTRAINT [FK_Promotor_WalletTransaccion_dbo.Maestra_TipoReward] FOREIGN KEY ([TipoReward_Id]) REFERENCES [dbo].[Maestra_TipoReward]([Id]),
    CONSTRAINT [FK_Promotor_WalletTransaccion_dbo.PromoEvento] FOREIGN KEY ([PromoEvento_Id]) REFERENCES [dbo].[PromoEvento]([Id]),
    CONSTRAINT [FK_Promotor_WalletTransaccion_dbo.CampaniaCrowdfunding_Payout] FOREIGN KEY ([CampaniaPayout_Id]) REFERENCES [dbo].[CampaniaCrowdfunding_Payout]([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Promotor_WalletTransaccion_Wallet_Fecha] ON [dbo].[Promotor_WalletTransaccion]([Wallet_Id] ASC, [FechaCreacion] DESC);
GO
