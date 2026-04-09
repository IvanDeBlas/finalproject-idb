-- ============================================================
-- WePlay Rises - Poblacion de Tablas Maestras
-- Actualizado: 2026-02-16
-- Generado desde EF Core Migrations (seed data)
-- Ejecutar despues de crear todas las tablas
-- ============================================================

SET NOCOUNT ON;
GO

/* ============================================================
   Maestra_Moneda (Core module seed)
   ============================================================ */
SET IDENTITY_INSERT [dbo].[Maestra_Moneda] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_Moneda] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_Moneda] ([Id], [Codigo], [Nombre], [Simbolo], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'EUR', N'Euro', N'EUR', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_Moneda] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_Moneda] ([Id], [Codigo], [Nombre], [Simbolo], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'USD', N'US Dollar', N'$', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_Moneda] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_Moneda] ([Id], [Codigo], [Nombre], [Simbolo], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'GBP', N'British Pound', N'GBP', 1, 3, '2026-01-01');
SET IDENTITY_INSERT [dbo].[Maestra_Moneda] OFF;
GO

/* ============================================================
   Maestra_TipoFinanciacion (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoFinanciacion] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_TipoFinanciacion] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'TODO_O_NADA', N'Todo o Nada', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoFinanciacion] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_TipoFinanciacion] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'FLEXIBLE', N'Flexible', 1, 2, '2026-01-01');
GO

/* ============================================================
   Maestra_EstadoCampaniaCrowd (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'BORRADOR', N'Borrador', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'EN_REVISION', N'En Revision', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'ACTIVA', N'Activa', 1, 3, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 4)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (4, N'FINALIZADA_EXITO', N'Finalizada con Exito', 1, 4, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 5)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (5, N'FINALIZADA_FRACASO', N'Finalizada sin Exito', 1, 5, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoCampaniaCrowd] WHERE [Id] = 6)
    INSERT INTO [dbo].[Maestra_EstadoCampaniaCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (6, N'CANCELADA', N'Cancelada', 1, 6, '2026-01-01');
GO

/* ============================================================
   Maestra_EstadoPedidoCrowd (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPedidoCrowd] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_EstadoPedidoCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'PENDIENTE', N'Pendiente', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPedidoCrowd] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_EstadoPedidoCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'CONFIRMADO', N'Confirmado', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPedidoCrowd] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_EstadoPedidoCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'CANCELADO', N'Cancelado', 1, 3, '2026-01-01');
GO

/* ============================================================
   Maestra_EstadoAportacionCrowd (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoAportacionCrowd] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_EstadoAportacionCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'PENDIENTE', N'Pendiente', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoAportacionCrowd] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_EstadoAportacionCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'AUTORIZADA', N'Autorizada', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoAportacionCrowd] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_EstadoAportacionCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'CAPTURADA', N'Capturada', 1, 3, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoAportacionCrowd] WHERE [Id] = 4)
    INSERT INTO [dbo].[Maestra_EstadoAportacionCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (4, N'CANCELADA', N'Cancelada', 1, 4, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoAportacionCrowd] WHERE [Id] = 5)
    INSERT INTO [dbo].[Maestra_EstadoAportacionCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (5, N'REEMBOLSADA', N'Reembolsada', 1, 5, '2026-01-01');
GO

/* ============================================================
   Maestra_EstadoPayoutCrowd (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPayoutCrowd] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_EstadoPayoutCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'PENDIENTE', N'Pendiente', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPayoutCrowd] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_EstadoPayoutCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'EN_PROCESO', N'En Proceso', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPayoutCrowd] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_EstadoPayoutCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'COMPLETADO', N'Completado', 1, 3, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_EstadoPayoutCrowd] WHERE [Id] = 4)
    INSERT INTO [dbo].[Maestra_EstadoPayoutCrowd] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (4, N'FALLIDO', N'Fallido', 1, 4, '2026-01-01');
GO

/* ============================================================
   Maestra_MetodoPago (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_MetodoPago] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_MetodoPago] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'TARJETA', N'Tarjeta de Credito', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_MetodoPago] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_MetodoPago] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'PAYPAL', N'PayPal', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_MetodoPago] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_MetodoPago] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'STRIPE', N'Stripe', 1, 3, '2026-01-01');
GO

/* ============================================================
   Maestra_TipoReward (Core module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoReward] WHERE [Id] = 1)
    INSERT INTO [dbo].[Maestra_TipoReward] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (1, N'FISICO', N'Fisico', 1, 1, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoReward] WHERE [Id] = 2)
    INSERT INTO [dbo].[Maestra_TipoReward] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (2, N'DIGITAL', N'Digital', 1, 2, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoReward] WHERE [Id] = 3)
    INSERT INTO [dbo].[Maestra_TipoReward] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (3, N'EXPERIENCIA', N'Experiencia', 1, 3, '2026-01-01');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Maestra_TipoReward] WHERE [Id] = 4)
    INSERT INTO [dbo].[Maestra_TipoReward] ([Id], [Codigo], [Nombre], [EsActivo], [Orden], [FechaCreacion]) VALUES (4, N'NO_REWARD', N'Sin Recompensa', 1, 4, '2026-01-01');
GO

/* ============================================================
   Tablas Maestras sin seed data en EF Core
   (TipoProyecto, EstadoProyecto, TipoNecesidad, EstadoNecesidad,
    ModalidadTrabajo, EstadoPropuesta, EstadoAcuerdo, EstadoEntregable,
    TipoValoracion, TipoPromotor, TipoPromo, TipoEventoPromo,
    EstadoWalletTransaccion, EstadoTareaPromo, TipoSkill, RolMiembroArtista)
   Nota: Estas tablas se crean en CoreContext pero no tienen seed data
   en las migraciones. Se pueden poblar manualmente si es necesario.
   ============================================================ */


/* ============================================================
   MaestrasCategoriaRol (Crowdsourcing module seed)
   ============================================================ */
SET IDENTITY_INSERT [dbo].[MaestrasCategoriaRol] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 1)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (1, N'Produccion Musical', N'music', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 2)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (2, N'Ingenieria de Audio', N'sliders', 2);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 3)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (3, N'Diseno y Creatividad', N'palette', 3);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 4)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (4, N'Marketing y Promocion', N'megaphone', 4);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 5)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (5, N'Gestion y Legal', N'briefcase', 5);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasCategoriaRol] WHERE [Id] = 6)
    INSERT INTO [dbo].[MaestrasCategoriaRol] ([Id], [Nombre], [Icono], [Orden]) VALUES (6, N'Produccion de Eventos / Live', N'mic', 6);
SET IDENTITY_INSERT [dbo].[MaestrasCategoriaRol] OFF;
GO

/* ============================================================
   MaestrasRolProfesional (Crowdsourcing module seed)
   ============================================================ */
SET IDENTITY_INSERT [dbo].[MaestrasRolProfesional] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 1)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (1, N'Productor musical', N'Dirige la vision sonora del proyecto completo', 1, N'Por proyecto o por cancion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 2)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (2, N'Arreglista / Compositor', N'Crea arreglos instrumentales y vocales', 1, N'Por cancion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 3)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (3, N'Beatmaker', N'Crea bases ritmicas y beats', 1, N'Por beat', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 4)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (4, N'Director musical', N'Coordina ensayos y direccion artistica en vivo', 1, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 5)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (5, N'Musico de sesion', N'Interpreta instrumentos para grabaciones', 1, N'Por sesion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 6)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (6, N'Vocal coach', N'Prepara y entrena vocalmente al artista', 1, N'Por sesion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 7)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (7, N'Ingeniero de grabacion', N'Captura audio de alta calidad en estudio', 2, N'Por dia', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 8)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (8, N'Ingeniero de mezcla', N'Equilibra y procesa todas las pistas grabadas', 2, N'Por cancion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 9)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (9, N'Ingeniero de mastering', N'Optimiza el audio final para distribucion', 2, N'Por cancion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 10)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (10, N'Tecnico de sonido (live)', N'Gestiona el sonido en eventos en vivo', 2, N'Por evento', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 11)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (11, N'Disenador sonoro', N'Crea efectos de sonido y ambientes', 2, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 12)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (12, N'Disenador grafico', N'Crea portadas, logos y material visual', 3, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 13)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (13, N'Fotografo', N'Sesiones fotograficas profesionales', 3, N'Por sesion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 14)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (14, N'Director de videoclip', N'Dirige la produccion audiovisual del videoclip', 3, N'Por video', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 15)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (15, N'Editor de video', N'Edita y post-produce material audiovisual', 3, N'Por video', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 16)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (16, N'Animador / Motion graphics', N'Crea animaciones y graficos en movimiento', 3, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 17)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (17, N'Director de arte', N'Define la estetica visual del proyecto', 3, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 18)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (18, N'Estilista / Vestuarista', N'Disena la imagen y vestuario del artista', 3, N'Por sesion', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 19)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (19, N'Community manager', N'Gestiona redes sociales y comunidad online', 4, N'Por mes', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 20)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (20, N'Publicista musical', N'Gestiona prensa y relaciones publicas', 4, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 21)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (21, N'Especialista en ads', N'Campanas de publicidad digital (Spotify, Meta, etc.)', 4, N'Por mes', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 22)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (22, N'Creador de contenido', N'Produce contenido para redes sociales y plataformas', 4, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 23)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (23, N'Experto en distribucion digital', N'Gestiona la distribucion en plataformas de streaming', 4, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 24)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (24, N'Manager artistico', N'Gestiona la carrera y negocios del artista', 5, N'Por mes', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 25)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (25, N'Abogado musical', N'Asesoria legal en contratos y derechos', 5, N'Por hora', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 26)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (26, N'Especialista en derechos de autor', N'Registro y gestion de derechos de propiedad intelectual', 5, N'Por proyecto', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 27)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (27, N'Promotor de eventos', N'Organiza y promueve conciertos y eventos', 6, N'Por evento', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 28)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (28, N'Stage manager', N'Coordina la logistica del escenario', 6, N'Por evento', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 29)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (29, N'Tecnico de iluminacion', N'Disena y opera la iluminacion de eventos', 6, N'Por evento', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[MaestrasRolProfesional] WHERE [Id] = 30)
    INSERT INTO [dbo].[MaestrasRolProfesional] ([Id], [Nombre], [Descripcion], [CategoriaRolId], [ModalidadCobro], [Activo]) VALUES (30, N'Roadie / Tour manager', N'Gestiona la logistica de giras', 6, N'Por dia', 1);
SET IDENTITY_INSERT [dbo].[MaestrasRolProfesional] OFF;
GO

/* ============================================================
   PlantillasProyecto (Crowdsourcing module seed)
   ============================================================ */
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111111', N'Grabar un Album / EP', N'Todas las fases para grabar tu primer disco: desde pre-produccion hasta el master final', N'music', 1, 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '22222222-2222-2222-2222-222222222222')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('22222222-2222-2222-2222-222222222222', N'Produccion de Videoclip', N'Todo lo necesario para producir un videoclip profesional', N'video', 2, 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '33333333-3333-3333-3333-333333333333')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('33333333-3333-3333-3333-333333333333', N'Organizar Gira / Tour', N'Planifica y ejecuta una gira de conciertos', N'map', 3, 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '44444444-4444-4444-4444-444444444444')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('44444444-4444-4444-4444-444444444444', N'Campana de Marketing', N'Estrategia completa de promocion y marketing musical', N'megaphone', 4, 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '55555555-5555-5555-5555-555555555555')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('55555555-5555-5555-5555-555555555555', N'Lanzamiento de Single', N'Todo lo necesario para lanzar un single con impacto', N'disc', 5, 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyecto] WHERE [Id] = '66666666-6666-6666-6666-666666666666')
    INSERT INTO [dbo].[PlantillasProyecto] ([Id], [Nombre], [Descripcion], [Icono], [Orden], [Activo], [FechaCreacion]) VALUES ('66666666-6666-6666-6666-666666666666', N'Presencia Online', N'Construye tu identidad digital y presencia en redes', N'globe', 6, 1, '2026-02-15');
GO

/* ============================================================
   PlantillasProyectoNecesidades (Crowdsourcing module seed)
   Nota: Hay 60+ registros. Se incluyen los de las 6 plantillas.
   ============================================================ */

-- Plantilla: Grabar un Album / EP
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111101')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111101', '11111111-1111-1111-1111-111111111111', N'Pre-produccion', N'Composicion y arreglos musicales', N'Crear arreglos instrumentales y vocales', 2, 200, 1500, 1, N'Alta', 1, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111102')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111102', '11111111-1111-1111-1111-111111111111', N'Pre-produccion', N'Produccion musical', N'Definir la vision sonora del proyecto', 1, 500, 3000, 1, N'Alta', 2, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111103')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111103', '11111111-1111-1111-1111-111111111111', N'Pre-produccion', N'Coaching vocal', N'Preparacion vocal para grabacion', 6, 100, 500, 1, N'Media', 3, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111104')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111104', '11111111-1111-1111-1111-111111111111', N'Grabacion', N'Ingeniero de grabacion', N'Captura de audio en estudio', 7, 300, 1500, 1, N'Alta', 4, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111105')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111105', '11111111-1111-1111-1111-111111111111', N'Grabacion', N'Musicos de sesion', N'Instrumentistas para grabacion', 5, 200, 1000, 1, N'Media', 5, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111106')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111106', '11111111-1111-1111-1111-111111111111', N'Mezcla y Master', N'Mezcla de pistas', N'Equilibrar y procesar todas las pistas', 8, 300, 2000, 1, N'Alta', 6, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111107')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111107', '11111111-1111-1111-1111-111111111111', N'Mezcla y Master', N'Mastering', N'Optimizar audio final para distribucion', 9, 200, 1000, 1, N'Alta', 7, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111108')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111108', '11111111-1111-1111-1111-111111111111', N'Diseno y Produccion', N'Diseno de portada', N'Crear portada del album', 12, 150, 800, 1, N'Media', 8, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111109')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111109', '11111111-1111-1111-1111-111111111111', N'Diseno y Produccion', N'Sesion fotografica', N'Fotos promocionales del artista', 13, 200, 1000, 1, N'Media', 9, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111110')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111110', '11111111-1111-1111-1111-111111111111', N'Promocion', N'Distribucion digital', N'Subir a plataformas de streaming', 23, 50, 300, 1, N'Alta', 10, '2026-02-15');
IF NOT EXISTS (SELECT 1 FROM [dbo].[PlantillasProyectoNecesidades] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
    INSERT INTO [dbo].[PlantillasProyectoNecesidades] ([Id], [PlantillaProyectoId], [Fase], [Titulo], [Descripcion], [RolProfesionalId], [PrecioMinOrientativo], [PrecioMaxOrientativo], [MonedaId], [Prioridad], [Orden], [FechaCreacion]) VALUES ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', N'Promocion', N'Publicidad y prensa', N'Campana de prensa y publicity', 20, 200, 1500, 1, N'Media', 11, '2026-02-15');
GO

-- Nota: Las plantillas de Videoclip, Gira, Marketing, Single y Presencia Online
-- tienen ~50 registros adicionales en PlantillasProyectoNecesidades.
-- Se omiten por brevedad. Consultar la migracion InitialCrowdsourcing.cs para el listado completo.

SET NOCOUNT OFF;
GO
