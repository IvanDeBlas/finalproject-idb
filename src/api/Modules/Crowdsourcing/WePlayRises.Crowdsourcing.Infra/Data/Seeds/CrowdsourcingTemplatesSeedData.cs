using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Seeds;

public static class CrowdsourcingTemplatesSeedData
{
    private static readonly DateTime SeedDate = new(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedCategorias(modelBuilder);
        SeedRoles(modelBuilder);
        SeedTemplates(modelBuilder);
        SeedNecesidades(modelBuilder);
    }

    private static void SeedCategorias(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaestraCategoriaRol>().HasData(
            new MaestraCategoriaRol { Id = 1, Nombre = "Produccion Musical", Icono = "music", Orden = 1 },
            new MaestraCategoriaRol { Id = 2, Nombre = "Ingenieria de Audio", Icono = "sliders", Orden = 2 },
            new MaestraCategoriaRol { Id = 3, Nombre = "Diseno y Creatividad", Icono = "palette", Orden = 3 },
            new MaestraCategoriaRol { Id = 4, Nombre = "Marketing y Promocion", Icono = "megaphone", Orden = 4 },
            new MaestraCategoriaRol { Id = 5, Nombre = "Gestion y Legal", Icono = "briefcase", Orden = 5 },
            new MaestraCategoriaRol { Id = 6, Nombre = "Produccion de Eventos / Live", Icono = "mic", Orden = 6 }
        );
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaestraRolProfesional>().HasData(
            // Categoria 1: Produccion Musical
            new MaestraRolProfesional { Id = 1, Nombre = "Productor musical", Descripcion = "Dirige la vision sonora del proyecto completo", CategoriaRolId = 1, ModalidadCobro = "Por proyecto o por cancion", Activo = true },
            new MaestraRolProfesional { Id = 2, Nombre = "Arreglista / Compositor", Descripcion = "Crea arreglos instrumentales y vocales", CategoriaRolId = 1, ModalidadCobro = "Por cancion", Activo = true },
            new MaestraRolProfesional { Id = 3, Nombre = "Beatmaker", Descripcion = "Crea bases ritmicas y beats", CategoriaRolId = 1, ModalidadCobro = "Por beat", Activo = true },
            new MaestraRolProfesional { Id = 4, Nombre = "Director musical", Descripcion = "Coordina ensayos y direccion artistica en vivo", CategoriaRolId = 1, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 5, Nombre = "Musico de sesion", Descripcion = "Interpreta instrumentos para grabaciones", CategoriaRolId = 1, ModalidadCobro = "Por sesion", Activo = true },
            new MaestraRolProfesional { Id = 6, Nombre = "Vocal coach", Descripcion = "Prepara y entrena vocalmente al artista", CategoriaRolId = 1, ModalidadCobro = "Por sesion", Activo = true },

            // Categoria 2: Ingenieria de Audio
            new MaestraRolProfesional { Id = 7, Nombre = "Ingeniero de grabacion", Descripcion = "Captura audio de alta calidad en estudio", CategoriaRolId = 2, ModalidadCobro = "Por dia", Activo = true },
            new MaestraRolProfesional { Id = 8, Nombre = "Ingeniero de mezcla", Descripcion = "Equilibra y procesa todas las pistas grabadas", CategoriaRolId = 2, ModalidadCobro = "Por cancion", Activo = true },
            new MaestraRolProfesional { Id = 9, Nombre = "Ingeniero de mastering", Descripcion = "Optimiza el audio final para distribucion", CategoriaRolId = 2, ModalidadCobro = "Por cancion", Activo = true },
            new MaestraRolProfesional { Id = 10, Nombre = "Tecnico de sonido (live)", Descripcion = "Gestiona el sonido en eventos en vivo", CategoriaRolId = 2, ModalidadCobro = "Por evento", Activo = true },
            new MaestraRolProfesional { Id = 11, Nombre = "Disenador sonoro", Descripcion = "Crea efectos de sonido y ambientes", CategoriaRolId = 2, ModalidadCobro = "Por proyecto", Activo = true },

            // Categoria 3: Diseno y Creatividad
            new MaestraRolProfesional { Id = 12, Nombre = "Disenador grafico", Descripcion = "Crea portadas, logos y material visual", CategoriaRolId = 3, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 13, Nombre = "Fotografo", Descripcion = "Sesiones fotograficas profesionales", CategoriaRolId = 3, ModalidadCobro = "Por sesion", Activo = true },
            new MaestraRolProfesional { Id = 14, Nombre = "Director de videoclip", Descripcion = "Dirige la produccion audiovisual del videoclip", CategoriaRolId = 3, ModalidadCobro = "Por video", Activo = true },
            new MaestraRolProfesional { Id = 15, Nombre = "Editor de video", Descripcion = "Edita y post-produce material audiovisual", CategoriaRolId = 3, ModalidadCobro = "Por video", Activo = true },
            new MaestraRolProfesional { Id = 16, Nombre = "Animador / Motion graphics", Descripcion = "Crea animaciones y graficos en movimiento", CategoriaRolId = 3, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 17, Nombre = "Director de arte", Descripcion = "Define la estetica visual del proyecto", CategoriaRolId = 3, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 18, Nombre = "Estilista / Vestuarista", Descripcion = "Disena la imagen y vestuario del artista", CategoriaRolId = 3, ModalidadCobro = "Por sesion", Activo = true },

            // Categoria 4: Marketing y Promocion
            new MaestraRolProfesional { Id = 19, Nombre = "Community manager", Descripcion = "Gestiona redes sociales y comunidad online", CategoriaRolId = 4, ModalidadCobro = "Por mes", Activo = true },
            new MaestraRolProfesional { Id = 20, Nombre = "Publicista musical", Descripcion = "Gestiona prensa y relaciones publicas", CategoriaRolId = 4, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 21, Nombre = "Especialista en ads", Descripcion = "Campanas de publicidad digital (Spotify, Meta, etc.)", CategoriaRolId = 4, ModalidadCobro = "Por mes", Activo = true },
            new MaestraRolProfesional { Id = 22, Nombre = "Creador de contenido", Descripcion = "Produce contenido para redes sociales y plataformas", CategoriaRolId = 4, ModalidadCobro = "Por proyecto", Activo = true },
            new MaestraRolProfesional { Id = 23, Nombre = "Experto en distribucion digital", Descripcion = "Gestiona la distribucion en plataformas de streaming", CategoriaRolId = 4, ModalidadCobro = "Por proyecto", Activo = true },

            // Categoria 5: Gestion y Legal
            new MaestraRolProfesional { Id = 24, Nombre = "Manager artistico", Descripcion = "Gestiona la carrera y negocios del artista", CategoriaRolId = 5, ModalidadCobro = "Por mes", Activo = true },
            new MaestraRolProfesional { Id = 25, Nombre = "Abogado musical", Descripcion = "Asesoria legal en contratos y derechos", CategoriaRolId = 5, ModalidadCobro = "Por hora", Activo = true },
            new MaestraRolProfesional { Id = 26, Nombre = "Especialista en derechos de autor", Descripcion = "Registro y gestion de derechos de propiedad intelectual", CategoriaRolId = 5, ModalidadCobro = "Por proyecto", Activo = true },

            // Categoria 6: Produccion de Eventos / Live
            new MaestraRolProfesional { Id = 27, Nombre = "Promotor de eventos", Descripcion = "Organiza y promueve conciertos y eventos", CategoriaRolId = 6, ModalidadCobro = "Por evento", Activo = true },
            new MaestraRolProfesional { Id = 28, Nombre = "Stage manager", Descripcion = "Coordina la logistica del escenario", CategoriaRolId = 6, ModalidadCobro = "Por evento", Activo = true },
            new MaestraRolProfesional { Id = 29, Nombre = "Tecnico de iluminacion", Descripcion = "Disena y opera la iluminacion de eventos", CategoriaRolId = 6, ModalidadCobro = "Por evento", Activo = true },
            new MaestraRolProfesional { Id = 30, Nombre = "Roadie / Tour manager", Descripcion = "Gestiona la logistica de giras", CategoriaRolId = 6, ModalidadCobro = "Por dia", Activo = true }
        );
    }

    private static void SeedTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlantillaProyecto>().HasData(
            new { Id = new PlantillaProyectoId(Guid.Parse("11111111-1111-1111-1111-111111111111")), Nombre = "Grabar un Album / EP", Descripcion = "Todas las fases para grabar tu primer disco: desde pre-produccion hasta el master final", Icono = "music", Orden = 1, Activo = true, FechaCreacion = SeedDate },
            new { Id = new PlantillaProyectoId(Guid.Parse("22222222-2222-2222-2222-222222222222")), Nombre = "Produccion de Videoclip", Descripcion = "Todo lo necesario para producir un videoclip profesional", Icono = "video", Orden = 2, Activo = true, FechaCreacion = SeedDate },
            new { Id = new PlantillaProyectoId(Guid.Parse("33333333-3333-3333-3333-333333333333")), Nombre = "Organizar Gira / Tour", Descripcion = "Planifica y ejecuta una gira de conciertos", Icono = "map", Orden = 3, Activo = true, FechaCreacion = SeedDate },
            new { Id = new PlantillaProyectoId(Guid.Parse("44444444-4444-4444-4444-444444444444")), Nombre = "Campana de Marketing", Descripcion = "Estrategia completa de promocion y marketing musical", Icono = "megaphone", Orden = 4, Activo = true, FechaCreacion = SeedDate },
            new { Id = new PlantillaProyectoId(Guid.Parse("55555555-5555-5555-5555-555555555555")), Nombre = "Lanzamiento de Single", Descripcion = "Todo lo necesario para lanzar un single con impacto", Icono = "disc", Orden = 5, Activo = true, FechaCreacion = SeedDate },
            new { Id = new PlantillaProyectoId(Guid.Parse("66666666-6666-6666-6666-666666666666")), Nombre = "Presencia Online", Descripcion = "Construye tu identidad digital y presencia en redes", Icono = "globe", Orden = 6, Activo = true, FechaCreacion = SeedDate }
        );
    }

    private static void SeedNecesidades(ModelBuilder modelBuilder)
    {
        var t1 = new PlantillaProyectoId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var t2 = new PlantillaProyectoId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        var t3 = new PlantillaProyectoId(Guid.Parse("33333333-3333-3333-3333-333333333333"));
        var t4 = new PlantillaProyectoId(Guid.Parse("44444444-4444-4444-4444-444444444444"));
        var t5 = new PlantillaProyectoId(Guid.Parse("55555555-5555-5555-5555-555555555555"));
        var t6 = new PlantillaProyectoId(Guid.Parse("66666666-6666-6666-6666-666666666666"));

        modelBuilder.Entity<PlantillaProyectoNecesidad>().HasData(
            // Template 1: Grabar un Album / EP (11 necesidades)
            Nec("11111111-1111-1111-1111-111111111101", t1, "Pre-produccion", "Composicion y arreglos musicales", "Crear arreglos instrumentales y vocales", 2, 200, 1500, "Alta", 1),
            Nec("11111111-1111-1111-1111-111111111102", t1, "Pre-produccion", "Produccion musical", "Definir la vision sonora del proyecto", 1, 500, 3000, "Alta", 2),
            Nec("11111111-1111-1111-1111-111111111103", t1, "Pre-produccion", "Coaching vocal", "Preparacion vocal para grabacion", 6, 100, 500, "Media", 3),
            Nec("11111111-1111-1111-1111-111111111104", t1, "Grabacion", "Ingeniero de grabacion", "Captura de audio en estudio", 7, 300, 1500, "Alta", 4),
            Nec("11111111-1111-1111-1111-111111111105", t1, "Grabacion", "Musicos de sesion", "Instrumentistas para grabacion", 5, 200, 1000, "Media", 5),
            Nec("11111111-1111-1111-1111-111111111106", t1, "Mezcla y Master", "Mezcla de pistas", "Equilibrar y procesar todas las pistas", 8, 300, 2000, "Alta", 6),
            Nec("11111111-1111-1111-1111-111111111107", t1, "Mezcla y Master", "Mastering", "Optimizar audio final para distribucion", 9, 200, 1000, "Alta", 7),
            Nec("11111111-1111-1111-1111-111111111108", t1, "Diseno y Produccion", "Diseno de portada", "Crear portada del album", 12, 150, 800, "Media", 8),
            Nec("11111111-1111-1111-1111-111111111109", t1, "Diseno y Produccion", "Sesion fotografica", "Fotos promocionales del artista", 13, 200, 1000, "Media", 9),
            Nec("11111111-1111-1111-1111-111111111110", t1, "Promocion", "Distribucion digital", "Subir a plataformas de streaming", 23, 50, 300, "Alta", 10),
            Nec("11111111-1111-1111-1111-111111111111", t1, "Promocion", "Publicidad y prensa", "Campana de prensa y publicity", 20, 200, 1500, "Media", 11),

            // Template 2: Produccion de Videoclip (11 necesidades)
            Nec("22222222-2222-2222-2222-222222222201", t2, "Pre-produccion", "Direccion de videoclip", "Concepto creativo y storyboard", 14, 500, 3000, "Alta", 1),
            Nec("22222222-2222-2222-2222-222222222202", t2, "Pre-produccion", "Director de arte", "Definir estetica visual del video", 17, 300, 1500, "Alta", 2),
            Nec("22222222-2222-2222-2222-222222222203", t2, "Pre-produccion", "Estilista / Vestuarista", "Imagen y vestuario del artista", 18, 150, 800, "Media", 3),
            Nec("22222222-2222-2222-2222-222222222204", t2, "Grabacion", "Equipo de grabacion video", "Camaras, iluminacion y equipo tecnico", 14, 800, 5000, "Alta", 4),
            Nec("22222222-2222-2222-2222-222222222205", t2, "Grabacion", "Iluminacion", "Diseno y operacion de iluminacion", 29, 200, 1000, "Media", 5),
            Nec("22222222-2222-2222-2222-222222222206", t2, "Post-produccion", "Edicion de video", "Montaje y edicion del videoclip", 15, 300, 2000, "Alta", 6),
            Nec("22222222-2222-2222-2222-222222222207", t2, "Post-produccion", "Animacion / Motion graphics", "Efectos visuales y graficos animados", 16, 300, 2000, "Media", 7),
            Nec("22222222-2222-2222-2222-222222222208", t2, "Post-produccion", "Correccion de color", "Color grading profesional", 15, 150, 800, "Media", 8),
            Nec("22222222-2222-2222-2222-222222222209", t2, "Promocion", "Contenido para redes", "Teasers y clips promocionales", 22, 100, 500, "Media", 9),
            Nec("22222222-2222-2222-2222-222222222210", t2, "Promocion", "Campana de lanzamiento", "Estrategia de lanzamiento del video", 21, 200, 1000, "Alta", 10),
            Nec("22222222-2222-2222-2222-222222222211", t2, "Distribucion", "Distribucion en plataformas", "YouTube, Vevo y redes sociales", 23, 50, 200, "Baja", 11),

            // Template 3: Organizar Gira / Tour (11 necesidades)
            Nec("33333333-3333-3333-3333-333333333301", t3, "Pre-produccion", "Tour manager", "Planificacion y logistica de la gira", 30, 500, 3000, "Alta", 1),
            Nec("33333333-3333-3333-3333-333333333302", t3, "Pre-produccion", "Promotor de eventos", "Conseguir venues y negociar fechas", 27, 300, 2000, "Alta", 2),
            Nec("33333333-3333-3333-3333-333333333303", t3, "Pre-produccion", "Director musical", "Preparacion del show en vivo", 4, 300, 1500, "Alta", 3),
            Nec("33333333-3333-3333-3333-333333333304", t3, "Pre-produccion", "Abogado musical", "Revision de contratos con venues", 25, 200, 1000, "Media", 4),
            Nec("33333333-3333-3333-3333-333333333305", t3, "Produccion", "Tecnico de sonido", "Sonido en vivo para cada show", 10, 200, 1000, "Alta", 5),
            Nec("33333333-3333-3333-3333-333333333306", t3, "Produccion", "Tecnico de iluminacion", "Iluminacion para cada show", 29, 150, 800, "Media", 6),
            Nec("33333333-3333-3333-3333-333333333307", t3, "Produccion", "Stage manager", "Coordinacion de escenario", 28, 150, 800, "Media", 7),
            Nec("33333333-3333-3333-3333-333333333308", t3, "Produccion", "Musicos de sesion", "Banda para el show en vivo", 5, 500, 3000, "Alta", 8),
            Nec("33333333-3333-3333-3333-333333333309", t3, "Promocion", "Community manager", "Promocion en redes de cada fecha", 19, 200, 800, "Media", 9),
            Nec("33333333-3333-3333-3333-333333333310", t3, "Promocion", "Fotografo de gira", "Documentar la gira visualmente", 13, 300, 1500, "Baja", 10),
            Nec("33333333-3333-3333-3333-333333333311", t3, "Promocion", "Creador de contenido", "Contenido backstage y making of", 22, 200, 1000, "Baja", 11),

            // Template 4: Campana de Marketing (9 necesidades)
            Nec("44444444-4444-4444-4444-444444444401", t4, "Estrategia", "Publicista musical", "Plan de comunicacion y prensa", 20, 300, 2000, "Alta", 1),
            Nec("44444444-4444-4444-4444-444444444402", t4, "Estrategia", "Especialista en ads", "Campanas de publicidad digital", 21, 300, 2000, "Alta", 2),
            Nec("44444444-4444-4444-4444-444444444403", t4, "Contenido", "Community manager", "Gestion de redes sociales", 19, 200, 800, "Alta", 3),
            Nec("44444444-4444-4444-4444-444444444404", t4, "Contenido", "Creador de contenido", "Contenido visual y audiovisual", 22, 200, 1000, "Alta", 4),
            Nec("44444444-4444-4444-4444-444444444405", t4, "Contenido", "Fotografo", "Sesion de fotos promocionales", 13, 200, 1000, "Media", 5),
            Nec("44444444-4444-4444-4444-444444444406", t4, "Contenido", "Disenador grafico", "Material grafico para campana", 12, 150, 800, "Media", 6),
            Nec("44444444-4444-4444-4444-444444444407", t4, "Distribucion", "Distribucion digital", "Plataformas de streaming y tiendas", 23, 50, 300, "Alta", 7),
            Nec("44444444-4444-4444-4444-444444444408", t4, "Legal", "Registro de derechos", "Registro de obra en entidades", 26, 100, 500, "Media", 8),
            Nec("44444444-4444-4444-4444-444444444409", t4, "Seguimiento", "Manager artistico", "Coordinacion general de la campana", 24, 300, 1500, "Media", 9),

            // Template 5: Lanzamiento de Single (8 necesidades)
            Nec("55555555-5555-5555-5555-555555555501", t5, "Pre-produccion", "Produccion musical", "Produccion del single", 1, 300, 2000, "Alta", 1),
            Nec("55555555-5555-5555-5555-555555555502", t5, "Grabacion", "Ingeniero de grabacion", "Grabacion del single", 7, 200, 1000, "Alta", 2),
            Nec("55555555-5555-5555-5555-555555555503", t5, "Mezcla y Master", "Mezcla", "Mezcla del single", 8, 200, 1000, "Alta", 3),
            Nec("55555555-5555-5555-5555-555555555504", t5, "Mezcla y Master", "Mastering", "Mastering del single", 9, 100, 500, "Alta", 4),
            Nec("55555555-5555-5555-5555-555555555505", t5, "Diseno", "Diseno de portada", "Portada del single", 12, 100, 500, "Media", 5),
            Nec("55555555-5555-5555-5555-555555555506", t5, "Promocion", "Distribucion digital", "Subida a plataformas", 23, 50, 200, "Alta", 6),
            Nec("55555555-5555-5555-5555-555555555507", t5, "Promocion", "Campana de ads", "Publicidad digital del single", 21, 200, 1000, "Media", 7),
            Nec("55555555-5555-5555-5555-555555555508", t5, "Legal", "Registro de derechos", "Registro del single", 26, 50, 300, "Media", 8),

            // Template 6: Presencia Online (5 necesidades)
            Nec("66666666-6666-6666-6666-666666666601", t6, "Identidad", "Disenador grafico", "Logo, paleta de colores, branding", 12, 200, 1000, "Alta", 1),
            Nec("66666666-6666-6666-6666-666666666602", t6, "Contenido", "Fotografo", "Sesion de fotos para perfiles", 13, 150, 800, "Alta", 2),
            Nec("66666666-6666-6666-6666-666666666603", t6, "Redes", "Community manager", "Configuracion y gestion de redes", 19, 200, 800, "Alta", 3),
            Nec("66666666-6666-6666-6666-666666666604", t6, "Contenido", "Creador de contenido", "Contenido inicial para perfiles", 22, 150, 600, "Media", 4),
            Nec("66666666-6666-6666-6666-666666666605", t6, "Distribucion", "Distribucion digital", "Perfiles en plataformas de streaming", 23, 50, 200, "Media", 5)
        );
    }

    private static object Nec(string id, PlantillaProyectoId plantillaId, string fase, string titulo, string descripcion, int rolId, decimal precioMin, decimal precioMax, string prioridad, int orden)
    {
        return new
        {
            Id = new PlantillaProyectoNecesidadId(Guid.Parse(id)),
            PlantillaProyectoId = plantillaId,
            Fase = fase,
            Titulo = titulo,
            Descripcion = descripcion,
            RolProfesionalId = rolId,
            PrecioMinOrientativo = precioMin,
            PrecioMaxOrientativo = precioMax,
            MonedaId = 1,
            Prioridad = prioridad,
            Orden = orden,
            FechaCreacion = SeedDate
        };
    }
}
