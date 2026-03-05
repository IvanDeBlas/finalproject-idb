using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Tests.Helpers;

public static class CrowdsourcingTestData
{
    public static PlantillaProyecto CreatePlantillaActiva(
        Guid? id = null,
        string nombre = "Produccion de EP",
        int necesidades = 3)
    {
        var plantillaId = new PlantillaProyectoId(id ?? Guid.NewGuid());
        var plantilla = new PlantillaProyecto
        {
            Id = plantillaId,
            Nombre = nombre,
            Descripcion = "Plantilla de test",
            Icono = "🎵",
            Orden = 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        for (var i = 0; i < necesidades; i++)
        {
            plantilla.Necesidades.Add(CreatePlantillaNecesidad(plantillaId, orden: i + 1));
        }

        return plantilla;
    }

    public static PlantillaProyecto CreatePlantillaInactiva(Guid? id = null)
    {
        var plantilla = CreatePlantillaActiva(id);
        plantilla.Activo = false;
        return plantilla;
    }

    public static PlantillaProyectoNecesidad CreatePlantillaNecesidad(
        PlantillaProyectoId plantillaId,
        Guid? necesidadId = null,
        string fase = "Grabacion",
        string titulo = "Mezcla de pistas",
        decimal? precioMin = 500m,
        decimal? precioMax = 1500m,
        string prioridad = "Alta",
        int orden = 1)
    {
        return new PlantillaProyectoNecesidad
        {
            Id = new PlantillaProyectoNecesidadId(necesidadId ?? Guid.NewGuid()),
            PlantillaProyectoId = plantillaId,
            Fase = fase,
            Titulo = titulo,
            Descripcion = "Necesidad de test",
            RolProfesionalId = 1,
            PrecioMinOrientativo = precioMin,
            PrecioMaxOrientativo = precioMax,
            MonedaId = 1,
            Prioridad = prioridad,
            Orden = orden,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static MaestraRolProfesional CreateRolProfesional(
        int id = 1,
        string nombre = "Productor Musical",
        int categoriaId = 1,
        bool activo = true)
    {
        return new MaestraRolProfesional
        {
            Id = id,
            Nombre = nombre,
            Descripcion = "Rol de test",
            CategoriaRolId = categoriaId,
            ModalidadCobro = "Por proyecto",
            Activo = activo,
            CategoriaRol = CreateCategoriaRol(categoriaId)
        };
    }

    public static MaestraCategoriaRol CreateCategoriaRol(
        int id = 1,
        string nombre = "Produccion Musical",
        int orden = 1)
    {
        return new MaestraCategoriaRol
        {
            Id = id,
            Nombre = nombre,
            Icono = "🎤",
            Orden = orden
        };
    }

    public static List<MaestraRolProfesional> CreateRolesProfesionales(int count = 3)
    {
        var roles = new List<MaestraRolProfesional>();
        for (var i = 1; i <= count; i++)
        {
            roles.Add(CreateRolProfesional(id: i, nombre: $"Rol {i}", categoriaId: (i % 2) + 1));
        }
        return roles;
    }

    public static List<MaestraCategoriaRol> CreateCategorias(int count = 3)
    {
        var categorias = new List<MaestraCategoriaRol>();
        for (var i = 1; i <= count; i++)
        {
            categorias.Add(CreateCategoriaRol(id: i, nombre: $"Categoria {i}", orden: i));
        }
        return categorias;
    }

    public static NecesidadCrowdsourcing CreateNecesidad(
        Guid? id = null,
        Guid? artistaId = null,
        Guid? proyectoId = null,
        string titulo = "Mezcla profesional de 5 canciones",
        int estadoNecesidadId = 1,
        int tipoNecesidadId = 1,
        int modalidadTrabajoId = 2,
        int propuestas = 0)
    {
        var necesidad = new NecesidadCrowdsourcing
        {
            Id = new NecesidadCrowdsourcingId(id ?? Guid.NewGuid()),
            ArtistaId = new ArtistaId(artistaId ?? Guid.NewGuid()),
            ProyectoArtisticoId = new ProyectoArtisticoId(proyectoId ?? Guid.NewGuid()),
            Titulo = titulo,
            Descripcion = "Descripcion de test",
            TipoNecesidadId = tipoNecesidadId,
            EstadoNecesidadId = estadoNecesidadId,
            ModalidadTrabajoId = modalidadTrabajoId,
            PresupuestoMin = 100m,
            PresupuestoMax = 500m,
            MonedaId = 1,
            FechaCreacion = DateTime.UtcNow.AddDays(-5),
            FechaLimitePropuestas = DateTime.UtcNow.AddDays(30)
        };

        for (var i = 0; i < propuestas; i++)
        {
            necesidad.Propuestas.Add(CreatePropuesta(necesidad.Id));
        }

        return necesidad;
    }

    public static NecesidadCrowdsourcing CreateNecesidadAbierta(Guid? artistaId = null)
    {
        return CreateNecesidad(artistaId: artistaId, estadoNecesidadId: 1);
    }

    public static NecesidadCrowdsourcing CreateNecesidadCerrada(Guid? artistaId = null)
    {
        return CreateNecesidad(artistaId: artistaId, estadoNecesidadId: 3);
    }

    public static PropuestaCrowdsourcing CreatePropuesta(
        NecesidadCrowdsourcingId necesidadId,
        int estadoPropuestaId = 1)
    {
        return new PropuestaCrowdsourcing
        {
            Id = new PropuestaCrowdsourcingId(Guid.NewGuid()),
            NecesidadId = necesidadId,
            UserId = Guid.NewGuid().ToString(),
            MensajePropuesta = "Propuesta de test",
            PrecioPropuesto = 300m,
            MonedaId = 1,
            DiasEstimados = 10,
            EstadoPropuestaId = estadoPropuestaId,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static PropuestaCrowdsourcing CreatePropuestaPendiente(
        NecesidadCrowdsourcingId? necesidadId = null,
        string? userId = null,
        Guid? perfilProfesionalId = null)
    {
        var nId = necesidadId ?? new NecesidadCrowdsourcingId(Guid.NewGuid());
        return new PropuestaCrowdsourcing
        {
            Id = new PropuestaCrowdsourcingId(Guid.NewGuid()),
            NecesidadId = nId,
            UserId = userId ?? Guid.NewGuid().ToString(),
            PerfilProfesionalId = new PerfilProfesionalId(perfilProfesionalId ?? Guid.NewGuid()),
            MensajePropuesta = "Propuesta pendiente de test",
            PrecioPropuesto = 500m,
            MonedaId = 1,
            DiasEstimados = 15,
            EstadoPropuestaId = EstadoPropuestaConstants.Pendiente,
            FechaCreacion = DateTime.UtcNow
        };
    }

    public static AcuerdoCrowdsourcing CreateAcuerdoActivo(
        Guid? artistaId = null,
        string? userIdProveedor = null,
        decimal importeTotal = 1000m)
    {
        return new AcuerdoCrowdsourcing
        {
            Id = AcuerdoCrowdsourcingId.CreateNew(),
            NecesidadId = new NecesidadCrowdsourcingId(Guid.NewGuid()),
            ArtistaId = new ArtistaId(artistaId ?? Guid.NewGuid()),
            UserIdProveedor = userIdProveedor ?? Guid.NewGuid().ToString(),
            EstadoAcuerdoId = EstadoAcuerdoConstants.Activo,
            ImporteTotalPactado = importeTotal,
            MonedaId = 1,
            TituloInterno = "Acuerdo de test",
            FechaInicio = DateTime.UtcNow,
            FechaCreacion = DateTime.UtcNow,
            Milestones = new List<AcuerdoCrowdsourcingMilestone>()
        };
    }

    public static AcuerdoCrowdsourcing CreateAcuerdoCompletado(Guid? artistaId = null)
    {
        var acuerdo = CreateAcuerdoActivo(artistaId);
        acuerdo.EstadoAcuerdoId = EstadoAcuerdoConstants.Completado;
        acuerdo.FechaFinReal = DateTime.UtcNow;
        return acuerdo;
    }

    public static AcuerdoCrowdsourcingMilestone CreateMilestone(
        AcuerdoCrowdsourcingId? acuerdoId = null,
        decimal importeParcial = 300m,
        bool completado = false)
    {
        return new AcuerdoCrowdsourcingMilestone
        {
            Id = Guid.NewGuid(),
            AcuerdoId = acuerdoId ?? AcuerdoCrowdsourcingId.CreateNew(),
            Titulo = "Milestone de test",
            Descripcion = "Descripcion de milestone",
            Orden = 1,
            ImporteParcial = importeParcial,
            FechaCompletado = completado ? DateTime.UtcNow : null,
            FechaCreacion = DateTime.UtcNow,
            Entregables = new List<AcuerdoCrowdsourcingEntregable>()
        };
    }

    public static AcuerdoCrowdsourcingEntregable CreateEntregable(
        AcuerdoCrowdsourcingId? acuerdoId = null,
        Guid? milestoneId = null,
        int estadoEntregableId = EstadoEntregableConstants.Entregado)
    {
        var aId = acuerdoId ?? AcuerdoCrowdsourcingId.CreateNew();
        return new AcuerdoCrowdsourcingEntregable
        {
            Id = Guid.NewGuid(),
            AcuerdoId = aId,
            MilestoneId = milestoneId,
            Titulo = "Entregable de test",
            Descripcion = "Descripcion de entregable",
            UrlRecurso = "https://drive.google.com/file/test",
            EstadoEntregableId = estadoEntregableId,
            FechaCreacion = DateTime.UtcNow,
            Acuerdo = CreateAcuerdoActivo()
        };
    }

    public static Artista CreateArtista(Guid? id = null, string? userId = null)
    {
        return new Artista
        {
            Id = new ArtistaId(id ?? Guid.NewGuid()),
            UserIdPropietario = userId ?? Guid.NewGuid().ToString(),
            NombreArtistico = "Artista Test",
            Descripcion = "Artista de test"
        };
    }
}
