# Arquitectura Hexagonal: Gestionar Necesidades de Crowdsourcing

**Fecha:** 2026-02-16
**Modulo:** Crowdsourcing
**Feature:** cs-gestionar-necesidades (US-CS-02)

---

## 1. Resumen Ejecutivo

Esta feature implementa el CRUD completo de necesidades de crowdsourcing para artistas, permitiendo publicar nuevas necesidades (manual o desde templates), listar con filtros y paginacion, editar necesidades abiertas, y cerrar necesidades con auto-rechazo de propuestas pendientes. Es la capacidad central del modulo de crowdsourcing desde la perspectiva del artista.

---

## 2. Domain Layer

### 2.1 Entidades

#### NecesidadCrowdsourcing (EXISTENTE - Requiere modificacion menor)

**Estado:** YA EXISTE (creada en US-CS-01)
**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/NecesidadCrowdsourcing.cs`

**Campos actuales:**
| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | NecesidadCrowdsourcingId | No | PK (Strongly Typed ID) |
| ProyectoArtisticoId | ProyectoArtisticoId | No | FK a ProyectoArtistico (UserAccess) |
| ArtistaId | ArtistaId | No | FK a Artista (UserAccess) |
| Titulo | string | No | Titulo de la necesidad (5-200 chars) |
| Descripcion | string | Si | Descripcion detallada (max 4000 chars) |
| TipoNecesidadId | int | No | FK a MaestraTipoNecesidad |
| EstadoNecesidadId | int | No | FK a MaestraEstadoNecesidad (1=Abierta, 2=EnProgreso, 3=Cerrada, 4=Cancelada) |
| ModalidadTrabajoId | int | No | FK a MaestraModalidadTrabajo (1=Presencial, 2=Remoto, 3=Hibrido) |
| PresupuestoMin | decimal? | Si | Presupuesto minimo |
| PresupuestoMax | decimal? | Si | Presupuesto maximo (debe ser >= min) |
| MonedaId | int? | Si | FK a MaestraMoneda (requerido si hay presupuesto) |
| UbicacionCiudad | string? | Si | Ciudad (requerido si modalidad Presencial/Hibrido) |
| UbicacionPais | string? | Si | Pais (requerido si modalidad Presencial/Hibrido) |
| FechaLimitePropuestas | DateTime? | Si | Fecha limite para recibir propuestas (> hoy+1) |
| FechaInicioPrevista | DateTime? | Si | Fecha de inicio estimada (>= hoy) |
| FechaCreacion | DateTime | No | Timestamp de creacion |
| FechaActualizacion | DateTime? | Si | Timestamp de ultima actualizacion |

**MODIFICACION REQUERIDA:** Agregar campo `MotivoCierre`

```csharp
public string? MotivoCierre { get; set; }  // max 500 chars - opcional al cerrar necesidad
```

**Navegaciones:**
- `Propuestas` -> `ICollection<PropuestaCrowdsourcing>` (1:N)
- `Acuerdos` -> `ICollection<AcuerdoCrowdsourcing>` (1:N)
- `Conversaciones` -> `ICollection<ConversacionCrowdsourcing>` (1:N)

**Nota:** La entidad NO tiene navegacion directa a `Artista` ni `ProyectoArtistico` porque estan en otro modulo (UserAccess). Se relacionan por strongly typed IDs.

---

#### PropuestaCrowdsourcing (EXISTENTE - Sera impactada por cierre)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/PropuestaCrowdsourcing.cs`

**Campos impactados al cerrar necesidad:**
| Propiedad | Tipo | Cambio en Cierre |
|-----------|------|------------------|
| EstadoPropuestaId | int | Cambiar a 3 (Rechazada) si estado actual = 1 (Pendiente) |
| FechaActualizacion | DateTime? | Actualizar con timestamp del cierre |

**Nota:** PropuestaCrowdsourcing NO tiene campo `MotivoRechazo` actualmente. Si se requiere en futuro, agregar campo opcional `string? MotivoRechazo` (max 500 chars).

---

### 2.2 Constants (NUEVO - ServiceResponseMessageType)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**Estado:** MODIFICAR archivo existente - Agregar nuevas constantes

```csharp
// Validation (1000-1999)
public const string Validation_MinLength = "1011";      // NUEVO - titulo min 5 chars
public const string Validation_InvalidDate = "1012";    // NUEVO - fechas invalidas

// NotFound (2000-2999)
public const string NotFound_Necesidad = "2009";        // NUEVO - necesidad no encontrada

// Business Rules (4000-4999)
public const string BusinessRule_NecesidadNotEditable = "4001";   // NUEVO - solo Abierta es editable
public const string BusinessRule_NecesidadNotCloseable = "4002";  // NUEVO - solo Abierta/EnProgreso cerrables
```

**Template:** Usar `.claude/templates/api/Domain/Constants/ServiceResponseMessageType.template.cs` como referencia

---

### 2.3 Repository Interfaces

#### INecesidadCrowdsourcingRepository (MODIFICAR - Expandir)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/INecesidadCrowdsourcingRepository.cs`

**Estado actual:** Solo tiene `AddAsync` y `AddManyAsync` (creados en US-CS-01)

**Metodos a AGREGAR:**

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | `Task<NecesidadCrowdsourcing?>` | Obtener necesidad por ID con Include de Propuestas |
| GetByIdWithDetailsAsync | `Task<NecesidadCrowdsourcing?>` | Obtener con Include de Propuestas + maestras |
| GetByArtistaIdPaginatedAsync | `Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)>` | Listar necesidades del artista con filtros y paginacion |
| UpdateAsync | `Task` | Actualizar necesidad (sin SaveChanges - lo hace Service) |
| DeleteAsync | `Task` | Soft delete o hard delete (futuro) |

**Importante:** Repository NO hace SaveChanges en Update/Delete. El Service usara UnitOfWork para transacciones.

---

#### IPropuestaCrowdsourcingRepository (NUEVO)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IPropuestaCrowdsourcingRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetPendientesByNecesidadIdAsync | `Task<IReadOnlyList<PropuestaCrowdsourcing>>` | Obtener propuestas pendientes de una necesidad |
| UpdateManyAsync | `Task` | Actualizar multiples propuestas (sin SaveChanges) |

**Uso:** Al cerrar necesidad, obtener propuestas pendientes y cambiarlas a Rechazada.

---

### 2.4 Service Interfaces

#### INecesidadCrowdsourcingService (MODIFICAR - Expandir)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/INecesidadCrowdsourcingService.cs`

**Estado actual:** Solo tiene `CreateManyAsync` (US-CS-01)

**Metodos a AGREGAR:**

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | `Task<NecesidadCrowdsourcing?>` | Obtener necesidad por ID (con cache) |
| GetByIdWithDetailsAsync | `Task<NecesidadCrowdsourcing?>` | Obtener con Include de propuestas y maestras |
| GetByArtistaIdPaginatedAsync | `Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)>` | Listar con filtros (estado, search) y paginacion |
| CreateAsync | `Task<NecesidadCrowdsourcingId>` | Crear una necesidad (usa UoW para commit) |
| UpdateAsync | `Task<bool>` | Actualizar necesidad (valida estado=Abierta, usa UoW) |
| CerrarAsync | `Task<int>` | Cerrar necesidad + auto-rechazar propuestas (transaccion atomica). Retorna num propuestas rechazadas |

**IMPORTANTE - Service DEBE tener:**
- `IUnitOfWork<CrowdsourcingContext>` para transacciones
- `IRequestCacheService` para cache (evita queries duplicados Validator/Handler)
- `ILogger<NecesidadCrowdsourcingService>` para logging
- `?? throw new ArgumentNullException` en TODAS las dependencias del constructor

**Service retorna ENTIDADES, NO DTOs**. El Handler hace mapping con AutoMapper.

---

#### IPropuestaCrowdsourcingService (NUEVO)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IPropuestaCrowdsourcingService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| RechazarPropuestasPendientesAsync | `Task<int>` | Rechazar todas las propuestas pendientes de una necesidad (retorna count) |

**Uso:** Llamado por `NecesidadCrowdsourcingService.CerrarAsync` en transaccion.

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations

#### NecesidadCrowdsourcingRepository (MODIFICAR - Expandir)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/NecesidadCrowdsourcingRepository.cs`

**Estado actual:** Tiene `AddAsync` y `AddManyAsync`

**Metodos a AGREGAR:**

```csharp
public async Task<NecesidadCrowdsourcing?> GetByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
{
    return await _context.Necesidades
        .AsNoTracking()  // CRITICO: Solo lectura
        .FirstOrDefaultAsync(n => n.Id == id, ct);
}

public async Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(NecesidadCrowdsourcingId id, CancellationToken ct)
{
    return await _context.Necesidades
        .AsNoTracking()
        .Include(n => n.Propuestas)  // Incluir propuestas
        .FirstOrDefaultAsync(n => n.Id == id, ct);
}

public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
    ArtistaId artistaId,
    int? estadoNecesidadId,
    string? search,
    int page,
    int pageSize,
    CancellationToken ct)
{
    var query = _context.Necesidades
        .AsNoTracking()
        .Where(n => n.ArtistaId == artistaId);

    // Filtro por estado (opcional)
    if (estadoNecesidadId.HasValue)
    {
        query = query.Where(n => n.EstadoNecesidadId == estadoNecesidadId.Value);
    }

    // Filtro por texto (titulo y descripcion)
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(n =>
            n.Titulo.Contains(search) ||
            (n.Descripcion != null && n.Descripcion.Contains(search)));
    }

    var totalCount = await query.CountAsync(ct);

    var items = await query
        .OrderByDescending(n => n.FechaCreacion)  // Mas recientes primero
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    return (items, totalCount);
}

public async Task UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
{
    _context.Necesidades.Update(entity);
    // NO hacer SaveChanges - lo hace el Service via UoW
}
```

**IMPORTANTE:**
- **AsNoTracking()** para queries de solo lectura (mejor performance)
- Repository NO hace SaveChanges (UoW lo hace desde Service)
- Queries con proyecciones optimizadas

---

#### PropuestaCrowdsourcingRepository (NUEVO)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/PropuestaCrowdsourcingRepository.cs`

```csharp
public class PropuestaCrowdsourcingRepository : IPropuestaCrowdsourcingRepository
{
    private readonly CrowdsourcingContext _context;

    public PropuestaCrowdsourcingRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<PropuestaCrowdsourcing>> GetPendientesByNecesidadIdAsync(
        NecesidadCrowdsourcingId necesidadId,
        CancellationToken ct)
    {
        return await _context.Propuestas
            .Where(p => p.NecesidadId == necesidadId && p.EstadoPropuestaId == 1)  // 1 = Pendiente
            .ToListAsync(ct);
    }

    public async Task UpdateManyAsync(IEnumerable<PropuestaCrowdsourcing> propuestas, CancellationToken ct)
    {
        _context.Propuestas.UpdateRange(propuestas);
        // NO hacer SaveChanges - lo hace Service via UoW
    }
}
```

---

### 3.2 Services

#### NecesidadCrowdsourcingService (MODIFICAR - Expandir significativamente)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/NecesidadCrowdsourcingService.cs`

**Estado actual:** Solo tiene `CreateManyAsync`

**Estructura completa del Service:**

```csharp
public class NecesidadCrowdsourcingService : INecesidadCrowdsourcingService
{
    private readonly INecesidadCrowdsourcingRepository _repository;
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly IUnitOfWork<CrowdsourcingContext> _uow;         // CRITICO: Para transacciones
    private readonly IRequestCacheService _requestCache;             // CRITICO: Para cache
    private readonly ILogger<NecesidadCrowdsourcingService> _logger;

    public NecesidadCrowdsourcingService(
        INecesidadCrowdsourcingRepository repository,
        IPropuestaCrowdsourcingService propuestaService,
        IUnitOfWork<CrowdsourcingContext> uow,
        IRequestCacheService requestCache,
        ILogger<NecesidadCrowdsourcingService> logger)
    {
        // CRITICO: ?? throw en TODAS las dependencias
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // NUEVO - Con RequestCache para evitar queries duplicados
    public async Task<NecesidadCrowdsourcing?> GetByIdAsync(
        NecesidadCrowdsourcingId id,
        CancellationToken ct)
    {
        return await _requestCache.GetOrAddAsync(
            $"necesidad:{id.Value}",
            async () => await _repository.GetByIdAsync(id, ct));
    }

    // NUEVO
    public async Task<NecesidadCrowdsourcing?> GetByIdWithDetailsAsync(
        NecesidadCrowdsourcingId id,
        CancellationToken ct)
    {
        return await _repository.GetByIdWithDetailsAsync(id, ct);
    }

    // NUEVO
    public async Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)> GetByArtistaIdPaginatedAsync(
        ArtistaId artistaId,
        int? estadoNecesidadId,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        return await _repository.GetByArtistaIdPaginatedAsync(
            artistaId,
            estadoNecesidadId,
            search,
            page,
            pageSize,
            ct);
    }

    // NUEVO - Crea una sola necesidad con UoW
    public async Task<NecesidadCrowdsourcingId> CreateAsync(
        NecesidadCrowdsourcing entity,
        CancellationToken ct)
    {
        var id = await _repository.AddAsync(entity, ct);
        await _uow.CommitAsync(ct);  // Service hace commit via UoW

        _logger.LogInformation(
            "Created NecesidadCrowdsourcing {Id} for Artista {ArtistaId}",
            id.Value,
            entity.ArtistaId.Value);

        return id;
    }

    // NUEVO - Actualiza necesidad (requiere estado = Abierta, validado en Handler/Validator)
    public async Task<bool> UpdateAsync(
        NecesidadCrowdsourcing entity,
        CancellationToken ct)
    {
        await _repository.UpdateAsync(entity, ct);
        await _uow.CommitAsync(ct);

        _logger.LogInformation(
            "Updated NecesidadCrowdsourcing {Id}",
            entity.Id.Value);

        return true;
    }

    // NUEVO - Cierra necesidad + auto-rechaza propuestas pendientes (TRANSACCION ATOMICA)
    public async Task<int> CerrarAsync(
        NecesidadCrowdsourcingId necesidadId,
        string? motivo,
        CancellationToken ct)
    {
        var necesidad = await _repository.GetByIdAsync(necesidadId, ct);
        if (necesidad == null)
        {
            _logger.LogWarning("Attempted to close non-existent necesidad {Id}", necesidadId.Value);
            return 0;
        }

        // 1. Actualizar estado de necesidad
        necesidad.EstadoNecesidadId = 3;  // 3 = Cerrada
        necesidad.MotivoCierre = motivo;
        necesidad.FechaActualizacion = DateTime.UtcNow;

        await _repository.UpdateAsync(necesidad, ct);

        // 2. Rechazar propuestas pendientes
        var propuestasRechazadas = await _propuestaService.RechazarPropuestasPendientesAsync(
            necesidadId,
            ct);

        // 3. Commit transaccion atomica (todo o nada)
        await _uow.CommitAsync(ct);

        _logger.LogInformation(
            "Closed NecesidadCrowdsourcing {Id} with {Count} proposals rejected",
            necesidadId.Value,
            propuestasRechazadas);

        return propuestasRechazadas;
    }

    // EXISTENTE - Mantener para US-CS-01
    public async Task<List<NecesidadCrowdsourcingId>> CreateManyAsync(
        List<NecesidadCrowdsourcing> entities,
        CancellationToken ct)
    {
        var ids = await _repository.AddManyAsync(entities, ct);

        _logger.LogInformation(
            "Created {Count} NecesidadCrowdsourcing entities for ProyectoArtistico {ProyectoId}",
            ids.Count,
            entities.FirstOrDefault()?.ProyectoArtisticoId.Value);

        return ids;
    }
}
```

**BENEFICIO de RequestCache:**
- Validator llama `GetByIdAsync(id)` para validar estado = Abierta
- Handler llama `GetByIdAsync(id)` para obtener entidad
- Con cache: 1 query DB (cache HIT en Handler)
- Sin cache: 2 queries DB (duplicado)

---

#### PropuestaCrowdsourcingService (NUEVO)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/PropuestaCrowdsourcingService.cs`

```csharp
public class PropuestaCrowdsourcingService : IPropuestaCrowdsourcingService
{
    private readonly IPropuestaCrowdsourcingRepository _repository;
    private readonly ILogger<PropuestaCrowdsourcingService> _logger;

    public PropuestaCrowdsourcingService(
        IPropuestaCrowdsourcingRepository repository,
        ILogger<PropuestaCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> RechazarPropuestasPendientesAsync(
        NecesidadCrowdsourcingId necesidadId,
        CancellationToken ct)
    {
        var propuestas = await _repository.GetPendientesByNecesidadIdAsync(necesidadId, ct);

        if (!propuestas.Any())
        {
            return 0;
        }

        foreach (var propuesta in propuestas)
        {
            propuesta.EstadoPropuestaId = 3;  // 3 = Rechazada
            propuesta.FechaActualizacion = DateTime.UtcNow;
            // Futuro: agregar propuesta.MotivoRechazo = "Necesidad cerrada por el artista";
        }

        await _repository.UpdateManyAsync(propuestas, ct);

        _logger.LogInformation(
            "Rejected {Count} pending proposals for necesidad {NecesidadId}",
            propuestas.Count,
            necesidadId.Value);

        return propuestas.Count;
    }
}
```

---

### 3.3 Entity Configurations

#### NecesidadCrowdsourcingConfiguration (VERIFICAR - Probablemente ya existe)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/NecesidadCrowdsourcingConfiguration.cs`

**Verificar configuracion existente y agregar si falta:**

```csharp
public class NecesidadCrowdsourcingConfiguration : IEntityTypeConfiguration<NecesidadCrowdsourcing>
{
    public void Configure(EntityTypeBuilder<NecesidadCrowdsourcing> builder)
    {
        builder.ToTable("Necesidades", "crowdsourcing");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(
            v => v.Value,
            v => new NecesidadCrowdsourcingId(v));

        builder.Property(x => x.ProyectoArtisticoId).HasConversion(
            v => v.Value,
            v => new ProyectoArtisticoId(v));

        builder.Property(x => x.ArtistaId).HasConversion(
            v => v.Value,
            v => new ArtistaId(v));

        builder.Property(x => x.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(4000);

        builder.Property(x => x.MotivoCierre)  // NUEVO
            .HasMaxLength(500);

        builder.Property(x => x.UbicacionCiudad)
            .HasMaxLength(100);

        builder.Property(x => x.UbicacionPais)
            .HasMaxLength(100);

        builder.Property(x => x.PresupuestoMin)
            .HasPrecision(18, 2);

        builder.Property(x => x.PresupuestoMax)
            .HasPrecision(18, 2);

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        // Indices para queries optimizadas
        builder.HasIndex(x => x.ArtistaId);
        builder.HasIndex(x => x.EstadoNecesidadId);
        builder.HasIndex(x => x.FechaCreacion);
        builder.HasIndex(x => new { x.ArtistaId, x.EstadoNecesidadId });  // Compuesto para listado filtrado

        // Relaciones
        builder.HasMany(x => x.Propuestas)
            .WithOne(p => p.Necesidad)
            .HasForeignKey(p => p.NecesidadId)
            .OnDelete(DeleteBehavior.Restrict);  // No eliminar necesidad si tiene propuestas
    }
}
```

**IMPORTANTE:** Indices compuestos para queries de listado paginado con filtros.

---

### 3.4 DbContext

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

**Modificacion:** Verificar que existan DbSet para Necesidades y Propuestas

```csharp
public DbSet<NecesidadCrowdsourcing> Necesidades => Set<NecesidadCrowdsourcing>();
public DbSet<PropuestaCrowdsourcing> Propuestas => Set<PropuestaCrowdsourcing>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new NecesidadCrowdsourcingConfiguration());
    modelBuilder.ApplyConfiguration(new PropuestaCrowdsourcingConfiguration());
    // ... otras configuraciones
}
```

---

## 4. Dependency Injection

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

**Modificacion:** Agregar registros de nuevos repositorios y services

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddCrowdsourcingServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<INecesidadCrowdsourcingRepository, NecesidadCrowdsourcingRepository>();
        services.AddScoped<IPropuestaCrowdsourcingRepository, PropuestaCrowdsourcingRepository>();  // NUEVO
        services.AddScoped<IPlantillaProyectoRepository, PlantillaProyectoRepository>();
        // ... otros repositories

        // Services
        services.AddScoped<INecesidadCrowdsourcingService, NecesidadCrowdsourcingService>();
        services.AddScoped<IPropuestaCrowdsourcingService, PropuestaCrowdsourcingService>();  // NUEVO
        services.AddScoped<IPlantillaProyectoService, PlantillaProyectoService>();
        // ... otros services

        return services;
    }
}
```

**Nota:** UnitOfWork y RequestCacheService ya estan registrados en BuildingBlocks, no es necesario registrarlos aqui.

---

## 5. Migraciones

### 5.1 Modificacion de NecesidadCrowdsourcing

**Comando:**
```bash
cd src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra
dotnet ef migrations add AddMotivoCierreToNecesidad --context CrowdsourcingContext
```

**Contenido esperado de la migracion:**
```csharp
public partial class AddMotivoCierreToNecesidad : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "MotivoCierre",
            schema: "crowdsourcing",
            table: "Necesidades",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MotivoCierre",
            schema: "crowdsourcing",
            table: "Necesidades");
    }
}
```

**Aplicar migracion:**
```bash
dotnet ef database update --context CrowdsourcingContext
```

---

## 6. Archivos a Crear/Modificar

### MODIFICAR (existentes)

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   ├── Model/
│   │   └── NecesidadCrowdsourcing.cs                    # MODIFICAR - Agregar MotivoCierre
│   └── Constants/
│       └── ServiceResponseMessageType.cs                # MODIFICAR - Agregar 1011, 1012, 2009, 4001, 4002
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   └── INecesidadCrowdsourcingRepository.cs     # MODIFICAR - Agregar GetById, GetPaginated, Update
│   │   └── Services/
│   │       └── INecesidadCrowdsourcingService.cs        # MODIFICAR - Agregar GetById, GetPaginated, Create, Update, Cerrar
│
├── WePlayRises.Crowdsourcing.Infra/
│   ├── Repositories/
│   │   └── NecesidadCrowdsourcingRepository.cs          # MODIFICAR - Agregar metodos
│   ├── Services/
│   │   └── NecesidadCrowdsourcingService.cs             # MODIFICAR - Agregar metodos + UoW + RequestCache
│   ├── Data/Configurations/
│   │   └── NecesidadCrowdsourcingConfiguration.cs       # VERIFICAR - Agregar config MotivoCierre si falta
│   └── DependencyInjection.cs                           # MODIFICAR - Agregar registros
```

### CREAR (nuevos)

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Application/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   └── IPropuestaCrowdsourcingRepository.cs     # NUEVO
│   │   └── Services/
│   │       └── IPropuestaCrowdsourcingService.cs        # NUEVO
│
├── WePlayRises.Crowdsourcing.Infra/
│   ├── Repositories/
│   │   └── PropuestaCrowdsourcingRepository.cs          # NUEVO
│   └── Services/
│       └── PropuestaCrowdsourcingService.cs             # NUEVO
```

---

## 7. Queries Optimizadas (Performance)

### 7.1 Listado Paginado

**Query con proyeccion (para DTOs en Handler):**
```csharp
// En Handler (NO en Repository - Repository retorna entidades)
var query = necesidades.Select(n => new NecesidadCrowdsourcingListDto
{
    Id = n.Id.Value,
    Titulo = n.Titulo,
    EstadoNecesidadId = n.EstadoNecesidadId,
    // ... proyectar solo campos necesarios
    NumeroPropuestas = _context.Propuestas.Count(p => p.NecesidadId == n.Id)  // COUNT eficiente
});
```

**Beneficio:** No cargar entidad completa + navegaciones, solo campos necesarios.

### 7.2 Detalle con Propuestas

**Include optimizado:**
```csharp
return await _context.Necesidades
    .Include(n => n.Propuestas.OrderByDescending(p => p.FechaCreacion))  // Solo propuestas ordenadas
    .FirstOrDefaultAsync(n => n.Id == id, ct);
```

### 7.3 Indices Compuestos

**Optimizar queries con filtros:**
```csharp
// Indice para: WHERE ArtistaId = X AND EstadoNecesidadId = Y ORDER BY FechaCreacion DESC
builder.HasIndex(x => new { x.ArtistaId, x.EstadoNecesidadId, x.FechaCreacion });
```

---

## 8. Validaciones de Negocio

### 8.1 Validar Ownership de Proyecto

**En Validator de Create:**
```csharp
RuleFor(x => x.ProyectoArtisticoId)
    .MustAsync(async (command, proyectoId, ct) =>
    {
        var proyecto = await _proyectoService.GetByIdAsync(proyectoId, ct);
        return proyecto != null && proyecto.ArtistaId == command.ArtistaIdFromJwt;
    })
    .WithMessage("El proyecto no existe o no te pertenece")
    .WithErrorCode(ServiceResponseMessageType.Auth_Forbidden);
```

**IMPORTANTE:** `IProyectoArtisticoService` debe usar RequestCache para evitar query duplicado en Handler.

### 8.2 Validar Estado Abierta para Editar

**En Validator de Update:**
```csharp
RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        var necesidad = await _necesidadService.GetByIdAsync(command.Id, ct);
        return necesidad != null && necesidad.EstadoNecesidadId == 1;  // 1 = Abierta
    })
    .WithMessage("Solo se pueden editar necesidades en estado Abierta")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
```

### 8.3 Validar Estado Abierta/EnProgreso para Cerrar

**En Validator de Cerrar:**
```csharp
RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        var necesidad = await _necesidadService.GetByIdAsync(command.Id, ct);
        return necesidad != null && (necesidad.EstadoNecesidadId == 1 || necesidad.EstadoNecesidadId == 2);
    })
    .WithMessage("Solo se pueden cerrar necesidades en estado Abierta o En Progreso")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
```

---

## 9. Transacciones Atomicas (UnitOfWork)

### 9.1 Cierre con Auto-Rechazo de Propuestas

**Flujo en NecesidadCrowdsourcingService.CerrarAsync:**

1. **Actualizar necesidad** (estado -> Cerrada, motivo, fecha)
2. **Rechazar propuestas pendientes** (estado -> Rechazada)
3. **Commit transaccion** (via UoW)
4. **Rollback automatico** si falla cualquier paso

**Beneficio:** Todo o nada. Si falla rechazo de propuestas, necesidad NO se cierra.

### 9.2 UnitOfWork Pattern

**Repository NO hace SaveChanges:**
```csharp
// Repository
public async Task UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
{
    _context.Necesidades.Update(entity);
    // NO SaveChanges
}
```

**Service hace Commit via UoW:**
```csharp
// Service
public async Task<bool> UpdateAsync(NecesidadCrowdsourcing entity, CancellationToken ct)
{
    await _repository.UpdateAsync(entity, ct);
    await _uow.CommitAsync(ct);  // Aqui se hace SaveChanges
    return true;
}
```

---

## 10. Caching con RequestCache (ADR-006)

### 10.1 Evitar Queries Duplicados

**Sin cache:**
```
Validator -> GetByIdAsync(id) -> Query DB
Handler   -> GetByIdAsync(id) -> Query DB (DUPLICADO)
```

**Con RequestCache:**
```
Validator -> GetByIdAsync(id) -> Query DB -> Almacena en cache
Handler   -> GetByIdAsync(id) -> Cache HIT (sin DB)
```

### 10.2 Implementacion en Service

```csharp
public async Task<NecesidadCrowdsourcing?> GetByIdAsync(
    NecesidadCrowdsourcingId id,
    CancellationToken ct)
{
    return await _requestCache.GetOrAddAsync(
        $"necesidad:{id.Value}",
        async () => await _repository.GetByIdAsync(id, ct));
}
```

**Scope:** Request-scoped (se limpia al final de cada HTTP request).

---

## 11. Checklist

- [ ] **NecesidadCrowdsourcing.cs** - Agregar campo `MotivoCierre`
- [ ] **ServiceResponseMessageType.cs** - Agregar constantes 1011, 1012, 2009, 4001, 4002
- [ ] **INecesidadCrowdsourcingRepository.cs** - Agregar metodos GetById, GetPaginated, Update
- [ ] **NecesidadCrowdsourcingRepository.cs** - Implementar metodos con AsNoTracking
- [ ] **INecesidadCrowdsourcingService.cs** - Agregar metodos GetById, Create, Update, Cerrar
- [ ] **NecesidadCrowdsourcingService.cs** - Implementar con UoW + RequestCache + Logger
- [ ] **IPropuestaCrowdsourcingRepository.cs** - Crear interface
- [ ] **PropuestaCrowdsourcingRepository.cs** - Crear implementacion
- [ ] **IPropuestaCrowdsourcingService.cs** - Crear interface
- [ ] **PropuestaCrowdsourcingService.cs** - Crear implementacion con rechazo de propuestas
- [ ] **NecesidadCrowdsourcingConfiguration.cs** - Agregar config MotivoCierre + indices
- [ ] **DependencyInjection.cs** - Registrar PropuestaRepository y PropuestaService
- [ ] **Migracion** - `AddMotivoCierreToNecesidad` + aplicar
- [ ] **Services usan `?? throw new ArgumentNullException`** en constructores
- [ ] **Services retornan entidades**, NO DTOs
- [ ] **Repository NO hace SaveChanges** (UoW lo hace)
- [ ] **RequestCache evita queries duplicados** Validator/Handler

---

## 12. Dependencias de Otros Modulos

### 12.1 UserAccess Module

**Necesitado para:**
- Validar que `ProyectoArtisticoId` existe y pertenece al artista
- Obtener `ArtistaId` desde JWT

**Interface necesaria:**
```csharp
// Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IProyectoArtisticoService.cs
public interface IProyectoArtisticoService
{
    Task<ProyectoArtistico?> GetByIdAsync(ProyectoArtisticoId id, CancellationToken ct);
}
```

**IMPORTANTE:** Este service debe usar RequestCache para evitar queries duplicados.

### 12.2 Cross-Module References

**NecesidadCrowdsourcing NO tiene navegacion directa** a `Artista` ni `ProyectoArtistico` porque estan en otro modulo. Se relacionan solo por strongly typed IDs.

**Validacion de ownership:**
- Validator inyecta `IProyectoArtisticoService` (de UserAccess)
- Valida que `proyecto.ArtistaId == artistaIdFromJwt`

---

## 13. Siguiente Paso Sugerido

1. **Implementar Domain Layer:**
   - Modificar `NecesidadCrowdsourcing.cs` (agregar `MotivoCierre`)
   - Modificar `ServiceResponseMessageType.cs` (agregar constantes)

2. **Implementar Repository Interfaces:**
   - Modificar `INecesidadCrowdsourcingRepository.cs`
   - Crear `IPropuestaCrowdsourcingRepository.cs`

3. **Implementar Service Interfaces:**
   - Modificar `INecesidadCrowdsourcingService.cs`
   - Crear `IPropuestaCrowdsourcingService.cs`

4. **Implementar Infrastructure:**
   - Modificar `NecesidadCrowdsourcingRepository.cs`
   - Crear `PropuestaCrowdsourcingRepository.cs`
   - Modificar `NecesidadCrowdsourcingService.cs` (agregar UoW + RequestCache)
   - Crear `PropuestaCrowdsourcingService.cs`

5. **Crear Migracion:**
   - `dotnet ef migrations add AddMotivoCierreToNecesidad`
   - `dotnet ef database update`

6. **Pasar a CQRS Planning:**
   - Crear Commands: `CreateNecesidadCommand`, `UpdateNecesidadCommand`, `CerrarNecesidadCommand`
   - Crear Queries: `GetMisNecesidadesQuery`, `GetNecesidadByIdQuery`
   - Crear Validators con reglas de negocio
   - Crear AutoMapper Profile

---

## 14. Notas Finales

### NUEVO vs MODIFICAR

| Componente | Estado | Accion |
|------------|--------|--------|
| NecesidadCrowdsourcing (entidad) | EXISTENTE | MODIFICAR - Agregar MotivoCierre |
| ServiceResponseMessageType | EXISTENTE | MODIFICAR - Agregar 5 constantes |
| INecesidadCrowdsourcingRepository | EXISTENTE | MODIFICAR - Agregar 4 metodos |
| NecesidadCrowdsourcingRepository | EXISTENTE | MODIFICAR - Implementar 4 metodos |
| INecesidadCrowdsourcingService | EXISTENTE | MODIFICAR - Agregar 5 metodos + UoW + Cache |
| NecesidadCrowdsourcingService | EXISTENTE | MODIFICAR - Implementar 5 metodos |
| IPropuestaCrowdsourcingRepository | NO EXISTE | CREAR - Interface completa |
| PropuestaCrowdsourcingRepository | NO EXISTE | CREAR - Implementacion completa |
| IPropuestaCrowdsourcingService | NO EXISTE | CREAR - Interface completa |
| PropuestaCrowdsourcingService | NO EXISTE | CREAR - Implementacion completa |
| DependencyInjection.cs | EXISTENTE | MODIFICAR - Agregar 2 registros |

### Transacciones Criticas

**CerrarNecesidadAsync:**
- Actualizar necesidad (estado, motivo, fecha)
- Rechazar propuestas pendientes (update batch)
- Commit atomico via UoW

**Si falla cualquier paso:** Rollback automatico (nada persiste).

### Performance Considerations

- **AsNoTracking()** en queries de solo lectura
- **Indices compuestos** para filtros frecuentes (ArtistaId + EstadoNecesidadId)
- **Proyecciones** en lugar de entidades completas (DTOs en Handler)
- **RequestCache** para evitar queries duplicados Validator/Handler
- **Paginacion obligatoria** en listados (nunca cargar todos los registros)

---

**Fin del Plan de Arquitectura Hexagonal**
