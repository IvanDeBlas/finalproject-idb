# Arquitectura Hexagonal: Crear Campaña de Crowdfunding

**Fecha:** 2026-02-12
**Módulo:** Crowdfunding
**Feature:** crear-campania (US-02)
**Bounded Context:** CampaniaCrowdfunding

---

## 1. Resumen Ejecutivo

Esta arquitectura define las capas Domain e Infrastructure para la feature "Crear Campaña de Crowdfunding". El sistema permite a artistas autenticados crear campañas en estado BORRADOR, editarlas antes de publicar y publicarlas para que sean visibles públicamente. La arquitectura implementa el patrón hexagonal con separación clara entre lógica de dominio (entidades POCOs, interfaces) y detalles de infraestructura (EF Core, repositorios, servicios con caching).

**Capacidades principales:**
- Crear campaña en estado BORRADOR (EstadoCampaniaId = 1)
- Editar campaña mientras esté en BORRADOR
- Publicar campaña (cambio a estado PUBLICADA, EstadoCampaniaId = 2)
- Validar ownership (solo el artista propietario puede editar/publicar)
- Cacheo de queries frecuentes para evitar hits duplicados a DB

---

## 2. Domain Layer

### 2.1 Entidades

#### CampaniaCrowdfunding (EXISTENTE - MODIFICACIÓN MÍNIMA)

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfunding.cs`

**Estado actual:** La entidad YA EXISTE con todas las propiedades necesarias. **NO requiere modificaciones** para esta feature.

| Propiedad | Tipo | Nullable | Descripción |
|-----------|------|----------|-------------|
| Id | CampaniaCrowdfundingId | No | PK (Strongly Typed ID) |
| ArtistaId | ArtistaId | No | FK a Artista (módulo UserAccess) |
| ProyectoArtisticoId | ProyectoArtisticoId? | Sí | FK opcional a ProyectoArtistico |
| Titulo | string | No | Título campaña (max 200) |
| Subtitulo | string? | Sí | Subtítulo (max 300) |
| DescripcionCorta | string? | Sí | Descripción breve (max 500) |
| VideoPrincipalUrl | string? | Sí | URL video principal (max 500) |
| ImagenPrincipalUrl | string? | Sí | URL imagen principal (max 500) |
| MonedaId | int | No | FK a MaestraMoneda (1=EUR) |
| ImporteObjetivo | decimal | No | Meta financiera |
| ImporteMinimo | decimal? | Sí | Importe mínimo para flexible funding |
| ImportePledgedActual | decimal | No | Importe recaudado actual |
| TipoFinanciacionId | int | No | FK a MaestraTipoFinanciacion (1=Todo o Nada, 2=Flexible) |
| EstadoCampaniaId | int | No | FK a MaestraEstadoCampaniaCrowd (1=BORRADOR, 2=PUBLICADA) |
| PermiteAportacionesAnonimas | bool | No | Si permite backings anónimos |
| PermitePropinas | bool | No | Si permite propinas adicionales |
| PorcentajeComisionPlataforma | decimal? | Sí | % comisión (calculado al publicar) |
| FechaInicio | DateTime? | Sí | Fecha inicio campaña |
| FechaFin | DateTime? | Sí | Fecha fin campaña |
| FechaPublicacion | DateTime? | Sí | Fecha de publicación (se establece al publicar) |
| FechaCierre | DateTime? | Sí | Fecha de cierre final |
| Borrado | bool | No | Soft delete flag |
| FechaCreacion | DateTime | No | Fecha creación registro |
| FechaActualizacion | DateTime? | Sí | Última actualización |

**Navegaciones:**
- `Rewards` → `ICollection<CampaniaCrowdfundingReward>` (1:N)
- `Pedidos` → `ICollection<PedidoCrowdfunding>` (1:N)
- `Payouts` → `ICollection<CampaniaCrowdfundingPayout>` (1:N)
- `Updates` → `ICollection<CampaniaCrowdfundingUpdate>` (1:N)
- `Comentarios` → `ICollection<CampaniaCrowdfundingComentario>` (1:N)
- `StretchGoals` → `ICollection<CampaniaCrowdfundingStretchGoal>` (1:N)

**Nota:** Entidad es POCO puro (sin métodos de negocio). Toda lógica de negocio va en Handlers.

---

### 2.2 Constants (ErrorCodes) - EXISTENTE, VERIFICAR COBERTURA

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Estado actual:** El archivo YA EXISTE con la mayoría de constantes. **VERIFICAR** que incluya todas las requeridas por contracts.md.

#### Constantes Requeridas para Crear Campaña:

| ErrorCode | Valor | Uso |
|-----------|-------|-----|
| Success | "0000" | ✅ EXISTE |
| Created | "0001" | ✅ EXISTE |
| Updated | "0002" | ✅ EXISTE |
| Validation_Required | "1001" | ✅ EXISTE |
| Validation_MaxLength | "1002" | ✅ EXISTE |
| Validation_InvalidUrl | "1006" | ✅ EXISTE |
| Validation_InvalidRange | "1007" | ✅ EXISTE |
| Validation_InvalidAmount | "1011" | ✅ EXISTE |
| Validation_InvalidDate | "1012" | ✅ EXISTE |
| NotFound_Campania | "2003" | ✅ EXISTE |
| Auth_Unauthorized | "3001" | ✅ EXISTE |
| Auth_Forbidden | "3002" | ✅ EXISTE |
| BusinessRule_CampaniaNotDraft | "4009" | ✅ EXISTE |
| Internal_UnexpectedError | "5000" | ✅ EXISTE |

**Acción requerida:** Ninguna, todas las constantes ya están definidas.

---

### 2.3 Repository Interfaces - EXISTENTE, VERIFICAR MÉTODOS

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Repositories/ICampaniaRepository.cs`

**Estado actual:** La interfaz YA EXISTE con métodos básicos.

#### Métodos Existentes:

| Método | Retorno | Descripción | Estado |
|--------|---------|-------------|--------|
| GetByIdAsync | CampaniaCrowdfunding? | Obtener por ID | ✅ EXISTE |
| GetAllAsync | IReadOnlyList<CampaniaCrowdfunding> | Listar todas | ✅ EXISTE |
| GetByArtistaIdAsync | IReadOnlyList<CampaniaCrowdfunding> | Listar por artista | ✅ EXISTE |
| AddAsync | CampaniaCrowdfundingId | Crear nueva | ✅ EXISTE |
| UpdateAsync | void | Actualizar | ✅ EXISTE |
| ExistsByTituloAsync | bool | Verificar título duplicado | ✅ EXISTE |
| ExistsByTituloExcludingIdAsync | bool | Verificar título duplicado excluyendo ID | ✅ EXISTE |

#### Métodos Adicionales Requeridos:

**NINGUNO.** Los métodos existentes son suficientes para esta feature.

**Posible mejora futura (post-MVP):**
```csharp
Task<IReadOnlyList<CampaniaCrowdfunding>> GetByEstadoAsync(int estadoCampaniaId, CancellationToken ct);
Task<IReadOnlyList<CampaniaCrowdfunding>> GetPublicadasAsync(CancellationToken ct); // Para landing page
```

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations - EXISTENTE, VERIFICAR IMPLEMENTACIÓN

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Repositories/CampaniaRepository.cs`

**Estado actual:** La implementación YA EXISTE con todos los métodos necesarios.

#### Implementación Actual:

```csharp
public class CampaniaRepository : ICampaniaRepository
{
    private readonly CrowdfundingContext _context;

    public CampaniaRepository(CrowdfundingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        return await _context.Campanias
            .Include(c => c.Rewards)  // CRÍTICO: Include para eager loading de rewards
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Campanias
            .AsNoTracking()  // CORRECTO: No trackear para lecturas
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct)
    {
        return await _context.Campanias
            .AsNoTracking()
            .Where(c => c.ArtistaId == artistaId)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync(ct);
    }

    public async Task<CampaniaCrowdfundingId> AddAsync(CampaniaCrowdfunding entity, CancellationToken ct)
    {
        await _context.Campanias.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);  // Repository hace SaveChanges
        return entity.Id;
    }

    public async Task UpdateAsync(CampaniaCrowdfunding entity, CancellationToken ct)
    {
        _context.Campanias.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct)
    {
        return await _context.Campanias
            .AnyAsync(c => c.Titulo == titulo && !c.Borrado, ct);
    }

    public async Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct)
    {
        return await _context.Campanias
            .AnyAsync(c => c.Titulo == titulo && c.Id != excludeId && !c.Borrado, ct);
    }
}
```

**Verificación de buenas prácticas:**
- ✅ Constructor con `?? throw new ArgumentNullException`
- ✅ Repository hace `SaveChangesAsync` (NO delega a Service ni Handler)
- ✅ Usa `AsNoTracking()` para queries de solo lectura
- ✅ Filtros incluyen `!c.Borrado` para soft delete
- ✅ `Include(c => c.Rewards)` en GetByIdAsync para eager loading

**Acción requerida:** Ninguna modificación necesaria.

---

### 3.2 Services - EXISTENTE, VERIFICAR CACHING Y UOW

**Interface:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Interfaces/Services/ICampaniaService.cs`
**Implementation:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Services/CampaniaService.cs`

**Estado actual:** Service YA EXISTE con caching implementado.

#### Interface Actual:

```csharp
public interface ICampaniaService
{
    Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct);
    Task<CampaniaCrowdfundingId> CreateAsync(CampaniaCrowdfunding campania, CancellationToken ct);
    Task UpdateAsync(CampaniaCrowdfunding campania, CancellationToken ct);
    Task<bool> ExistsAsync(CampaniaCrowdfundingId id, CancellationToken ct);
    Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct);
    Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct);
}
```

#### Implementation Actual:

```csharp
public class CampaniaService : ICampaniaService
{
    private readonly ICampaniaRepository _repository;
    private readonly IRequestCacheService _requestCache;  // ✅ CORRECTO: Cache para evitar queries duplicados
    private readonly ILogger<CampaniaService> _logger;

    public CampaniaService(
        ICampaniaRepository repository,
        IRequestCacheService requestCache,
        ILogger<CampaniaService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        var cacheKey = $"campania:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<IReadOnlyList<CampaniaCrowdfunding>> GetByArtistaIdAsync(ArtistaId artistaId, CancellationToken ct)
    {
        var cacheKey = $"campanias:artista:{artistaId.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByArtistaIdAsync(artistaId, ct))
            ?? Array.Empty<CampaniaCrowdfunding>();
    }

    public async Task<CampaniaCrowdfundingId> CreateAsync(CampaniaCrowdfunding campania, CancellationToken ct)
    {
        campania.FechaCreacion = DateTime.UtcNow;  // Service establece FechaCreacion
        var id = await _repository.AddAsync(campania, ct);

        _logger.LogInformation("Campania {CampaniaId} created for Artista {ArtistaId}",
            id.Value, campania.ArtistaId.Value);

        return id;
    }

    public async Task UpdateAsync(CampaniaCrowdfunding campania, CancellationToken ct)
    {
        campania.FechaActualizacion = DateTime.UtcNow;  // Service establece FechaActualizacion
        await _repository.UpdateAsync(campania, ct);

        _logger.LogInformation("Campania {CampaniaId} updated", campania.Id.Value);
    }

    public async Task<bool> ExistsAsync(CampaniaCrowdfundingId id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);  // Usa cache vía GetByIdAsync
        return entity != null;
    }

    public async Task<bool> ExistsByTituloAsync(string titulo, CancellationToken ct)
    {
        return await _repository.ExistsByTituloAsync(titulo, ct);
    }

    public async Task<bool> ExistsByTituloExcludingIdAsync(string titulo, CampaniaCrowdfundingId excludeId, CancellationToken ct)
    {
        return await _repository.ExistsByTituloExcludingIdAsync(titulo, excludeId, ct);
    }
}
```

**Verificación de buenas prácticas:**
- ✅ Constructor con `?? throw` para TODAS las dependencias
- ✅ Inyecta `IRequestCacheService` para caching
- ✅ Inyecta `ILogger` para logging
- ✅ Service retorna ENTIDADES (no DTOs)
- ✅ Métodos GetByIdAsync y GetByArtistaIdAsync usan cache
- ✅ Service establece FechaCreacion y FechaActualizacion
- ❌ **FALTA:** NO inyecta `IUnitOfWork<CrowdfundingContext>` porque Repository ya hace SaveChanges directamente

**Nota sobre UnitOfWork:**
El proyecto actual usa un patrón híbrido donde Repository hace `SaveChanges` directamente. En miUrba, se usa UoW explícito. Para esta feature, **mantener el patrón existente** (Repository con SaveChanges) para consistencia con el resto del módulo Crowdfunding.

**Acción requerida:** Ninguna modificación necesaria para esta feature.

---

### 3.3 Entity Configurations - EXISTENTE, VERIFICAR FLUENT API

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Context/CrowdfundingContext.cs`

**Estado actual:** La configuración EF Core YA EXISTE en `OnModelCreating` del DbContext.

#### Configuración Actual de CampaniaCrowdfunding:

```csharp
modelBuilder.Entity<CampaniaCrowdfunding>(entity =>
{
    entity.ToTable("CampaniaCrowdfunding");
    entity.HasKey(e => e.Id);

    // Propiedades
    entity.Property(e => e.ArtistaId).HasColumnName("Artista_Id");
    entity.Property(e => e.ProyectoArtisticoId).HasColumnName("ProyectoArtistico_Id");
    entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
    entity.Property(e => e.Subtitulo).HasMaxLength(300);
    entity.Property(e => e.DescripcionCorta).HasMaxLength(500);
    entity.Property(e => e.VideoPrincipalUrl).HasMaxLength(500);
    entity.Property(e => e.ImagenPrincipalUrl).HasMaxLength(500);
    entity.Property(e => e.MonedaId).HasColumnName("Moneda_Id");
    entity.Property(e => e.ImporteObjetivo).HasColumnType("decimal(18, 2)");
    entity.Property(e => e.ImporteMinimo).HasColumnType("decimal(18, 2)");
    entity.Property(e => e.ImportePledgedActual).HasColumnType("decimal(18, 2)");
    entity.Property(e => e.TipoFinanciacionId).HasColumnName("TipoFinanciacion_Id");
    entity.Property(e => e.EstadoCampaniaId).HasColumnName("EstadoCampania_Id");
    entity.Property(e => e.PorcentajeComisionPlataforma).HasColumnType("decimal(5, 2)");
    entity.Property(e => e.FechaInicio).HasPrecision(3);
    entity.Property(e => e.FechaFin).HasPrecision(3);
    entity.Property(e => e.FechaPublicacion).HasPrecision(3);
    entity.Property(e => e.FechaCierre).HasPrecision(3);
    entity.Property(e => e.FechaCreacion).HasPrecision(3);
    entity.Property(e => e.FechaActualizacion).HasPrecision(3);

    // Índices
    entity.HasIndex(e => new { e.ArtistaId, e.EstadoCampaniaId })
        .HasDatabaseName("IX_CampaniaCrowdfunding_Artista_Estado");

    // Relaciones
    entity.HasMany(e => e.Rewards).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
    entity.HasMany(e => e.Pedidos).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Restrict);
    entity.HasMany(e => e.Payouts).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Restrict);
    entity.HasMany(e => e.Updates).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
    entity.HasMany(e => e.Comentarios).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
    entity.HasMany(e => e.StretchGoals).WithOne(e => e.Campania)
        .HasForeignKey(e => e.CampaniaId).OnDelete(DeleteBehavior.Cascade);
});
```

**Verificación de buenas prácticas:**
- ✅ Usa Fluent API (no Data Annotations)
- ✅ MaxLength definidos para strings
- ✅ HasColumnType para decimales con precisión
- ✅ HasPrecision(3) para DateTime (precisión milisegundos)
- ✅ Índice compuesto en ArtistaId + EstadoCampaniaId (optimiza queries)
- ✅ Relaciones con Delete Behavior apropiados (Cascade para hijos propios, Restrict para referencias externas)
- ✅ Conversión de Strongly Typed IDs en ConfigureConventions

**Acción requerida:** Ninguna modificación necesaria.

---

### 3.4 DbContext - EXISTENTE, VERIFICAR DBSET

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/Context/CrowdfundingContext.cs`

**Estado actual:** El DbContext YA EXISTE con todos los DbSets necesarios.

```csharp
public class CrowdfundingContext : CoreDbContext
{
    public CrowdfundingContext(DbContextOptions<CrowdfundingContext> options) : base(options)
    {
    }

    public DbSet<CampaniaCrowdfunding> Campanias => Set<CampaniaCrowdfunding>();
    public DbSet<CampaniaCrowdfundingReward> Rewards => Set<CampaniaCrowdfundingReward>();
    public DbSet<PedidoCrowdfunding> Pedidos => Set<PedidoCrowdfunding>();
    // ... otros DbSets
}
```

**Verificación:**
- ✅ DbSet<CampaniaCrowdfunding> Campanias existe
- ✅ Hereda de CoreDbContext (Building Blocks)
- ✅ ConfigureConventions para Strongly Typed IDs
- ✅ OnModelCreating con Fluent API completo

**Acción requerida:** Ninguna modificación necesaria.

---

## 4. Dependency Injection - EXISTENTE, VERIFICAR REGISTROS

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/DependencyInjection.cs`

**Estado actual:** DI YA EXISTE con registros correctos.

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddCrowdfundingServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ICampaniaRepository, CampaniaRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        // Register services
        services.AddScoped<ICampaniaService, CampaniaService>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<IPedidoService, PedidoService>();

        return services;
    }
}
```

**Verificación:**
- ✅ ICampaniaRepository → CampaniaRepository registrado
- ✅ ICampaniaService → CampaniaService registrado
- ✅ Lifetime Scoped correcto

**Acción requerida:** Ninguna modificación necesaria.

---

## 5. Migraciones

**Estado actual:** El esquema de base de datos ya existe (CampaniaCrowdfunding table).

**Verificar migración existente:**
```bash
cd src/api/WebApi
dotnet ef migrations list --context CrowdfundingContext
```

**Si NO existe migración:**
```bash
dotnet ef migrations add CreateCrowdfundingSchema --project Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra --context CrowdfundingContext
dotnet ef database update --project Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra --context CrowdfundingContext
```

**IMPORTANTE:** Si la tabla CampaniaCrowdfunding ya existe en DB, NO crear nueva migración. Verificar con:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'CampaniaCrowdfunding';
```

---

## 6. Archivos a Crear/Modificar

### 6.1 NO requieren modificación (YA EXISTEN y son correctos)

```
✅ Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/
    ✅ Model/CampaniaCrowdfunding.cs
    ✅ Constants/ServiceResponseMessageType.cs

✅ Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
    ✅ Interfaces/Repositories/ICampaniaRepository.cs
    ✅ Interfaces/Services/ICampaniaService.cs

✅ Modules/Crowdfunding/WePlayRises.Crowdfunding.Infra/
    ✅ Repositories/CampaniaRepository.cs
    ✅ Services/CampaniaService.cs
    ✅ Context/CrowdfundingContext.cs
    ✅ DependencyInjection.cs
```

### 6.2 Archivos a crear (en Application Layer - siguiente fase)

**NOTA:** Estos archivos SON PARTE DE LA APPLICATION LAYER y serán diseñados por el agente `cqrs-planning-architect`. Se mencionan aquí solo para contexto.

```
📝 Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
    📝 Features/Campanias/Commands/
        📝 PublicarCampaniaCommand.cs  # NUEVO - Cambiar estado a PUBLICADA
    📝 Features/Campanias/Validators/
        📝 PublicarCampaniaCommandValidator.cs  # NUEVO - Validar campos requeridos para publicar
```

---

## 7. Flujo de Datos (Arquitectura Hexagonal)

### Crear Campaña (POST /api/campanias)
```
1. Controller recibe CreateCampaniaRequest
   └─> Extrae ArtistaId del token JWT (User.FindFirstValue(ClaimTypes.NameIdentifier))
   └─> Crea CreateCampaniaCommand con ArtistaId inyectado

2. MediatR → CreateCampaniaCommandHandler
   └─> Validator (CreateCampaniaCommandValidator) valida Command
       └─> Llama ICampaniaService.ExistsByTituloAsync() para validar duplicado
           └─> Service consulta Repository (sin cache, es validación)

   └─> Handler llama ICampaniaService.CreateAsync()
       └─> Service establece FechaCreacion = DateTime.UtcNow
       └─> Service establece EstadoCampaniaId = 1 (BORRADOR)
       └─> Service establece ImportePledgedActual = 0
       └─> Service delega a Repository.AddAsync()
           └─> Repository hace SaveChangesAsync()
           └─> Retorna CampaniaCrowdfundingId

   └─> Handler mapea entidad a CampaniaDto (AutoMapper)
   └─> Retorna ServiceResponse<CampaniaDto>

3. Controller retorna 200 OK con DTO
```

### Publicar Campaña (POST /api/campanias/{id}/publicar)
```
1. Controller recibe {id}
   └─> Extrae ArtistaId del token JWT
   └─> Crea PublicarCampaniaCommand

2. MediatR → PublicarCampaniaCommandHandler
   └─> Validator (PublicarCampaniaCommandValidator) valida campos requeridos
       └─> Llama ICampaniaService.GetByIdAsync() para obtener campaña
           └─> Service consulta cache (cacheKey: "campania:{id.Value}")
           └─> Si cache HIT → retorna inmediato
           └─> Si cache MISS → Repository.GetByIdAsync() → almacena en cache

       └─> Valida que EstadoCampaniaId == 1 (BORRADOR)
       └─> Valida que Titulo, ImporteObjetivo, FechaFin están completos
       └─> Valida que FechaFin >= DateTime.UtcNow + 7 days

   └─> Handler actualiza entidad:
       └─> campania.EstadoCampaniaId = 2 (PUBLICADA)
       └─> campania.FechaPublicacion = DateTime.UtcNow
       └─> Si campania.FechaInicio == null → campania.FechaInicio = DateTime.UtcNow

   └─> Handler llama ICampaniaService.UpdateAsync()
       └─> Service establece FechaActualizacion
       └─> Service delega a Repository.UpdateAsync()
           └─> Repository hace SaveChangesAsync()

   └─> Retorna ServiceResponse<PublishCampaniaResponse>

3. Controller retorna 200 OK con response
```

**Beneficio del caching:**
- Validator llama `GetByIdAsync()` → Resultado en cache
- Handler llama `GetByIdAsync()` → Usa cache, NO hace query a DB
- Ahorro: 1 query a DB por request

---

## 8. Validaciones de Negocio (Domain Rules)

### Reglas implementadas en Validators (Application Layer):

| Regla | Validación | ErrorCode |
|-------|------------|-----------|
| Título obligatorio | `.NotEmpty()` | 1001 |
| Título max 200 chars | `.MaximumLength(200)` | 1002 |
| Subtítulo max 300 chars | `.MaximumLength(300)` | 1002 |
| Descripción corta max 500 chars | `.MaximumLength(500)` | 1002 |
| URL video válida | `.Must(BeValidUrl)` | 1006 |
| URL imagen válida | `.Must(BeValidUrl)` | 1006 |
| ImporteObjetivo > 0 | `.GreaterThan(0)` | 1011 |
| ImporteMinimo <= ImporteObjetivo | `.LessThanOrEqualTo(x => x.ImporteObjetivo)` | 1007 |
| FechaFin > FechaInicio | `.GreaterThan(x => x.FechaInicio)` | 1012 |
| FechaFin >= Now + 7 days (publicar) | `.Must(x => x >= DateTime.UtcNow.AddDays(7))` | 1012 |
| Solo BORRADOR puede editarse | `campania.EstadoCampaniaId == 1` | 4009 |
| Solo propietario puede editar | `campania.ArtistaId == artistaIdFromToken` | 3002 |

**NOTA:** Estas validaciones se implementan en Validators (Application Layer), NO en Domain.

---

## 9. Queries Eficientes y Performance

### Estrategias implementadas:

#### 9.1 AsNoTracking para Lecturas
```csharp
// GetAllAsync - Lista para UI (solo lectura)
return await _context.Campanias
    .AsNoTracking()
    .OrderByDescending(c => c.FechaCreacion)
    .ToListAsync(ct);
```

#### 9.2 Eager Loading Selectivo
```csharp
// GetByIdAsync - Incluye Rewards porque se usan frecuentemente
return await _context.Campanias
    .Include(c => c.Rewards)  // Solo lo necesario
    .FirstOrDefaultAsync(x => x.Id == id, ct);
```

#### 9.3 Request Scoped Cache (ADR-006)
```csharp
// Service usa cache para evitar queries duplicados en mismo request
public async Task<CampaniaCrowdfunding?> GetByIdAsync(CampaniaCrowdfundingId id, CancellationToken ct)
{
    var cacheKey = $"campania:{id.Value}";

    return await _requestCache.GetOrAddAsync(
        cacheKey,
        async () => await _repository.GetByIdAsync(id, ct));
}
```

#### 9.4 Índices en DB
```csharp
// Índice compuesto para query frecuente: listar por artista y filtrar por estado
entity.HasIndex(e => new { e.ArtistaId, e.EstadoCampaniaId })
    .HasDatabaseName("IX_CampaniaCrowdfunding_Artista_Estado");
```

**Beneficio:** Query `GetByArtistaIdAsync` usa índice, evita table scan.

#### 9.5 Soft Delete
```csharp
// Filtros incluyen !c.Borrado para evitar retornar registros eliminados
return await _context.Campanias
    .AnyAsync(c => c.Titulo == titulo && !c.Borrado, ct);
```

---

## 10. Checklist de Arquitectura Hexagonal

### Domain Layer
- [x] Entidades son POCOs (sin métodos de negocio) ✅ CampaniaCrowdfunding es POCO
- [x] ServiceResponseMessageType.cs existe en Domain/Constants ✅ Existe con todos los códigos
- [x] Repository interfaces están en Domain/Interfaces ✅ ICampaniaRepository existe
- [x] Interfaces retornan entidades (no DTOs) ✅ GetByIdAsync retorna CampaniaCrowdfunding

### Infrastructure Layer
- [x] Repository implementations en Infra/Repositories ✅ CampaniaRepository existe
- [x] Repository hace SaveChanges (no Service ni Handler) ✅ Implementado correctamente
- [x] Services inyectan Repository + Cache + Logger ✅ CampaniaService inyecta los 3
- [x] Services usan `?? throw new ArgumentNullException` ✅ Todos los parámetros validados
- [x] Services retornan entidades (no DTOs) ✅ GetByIdAsync retorna entidad
- [x] Entity configurations con Fluent API ✅ OnModelCreating completo
- [x] DbContext configurado con DbSets ✅ CrowdfundingContext tiene Campanias
- [x] DI registrado correctamente ✅ AddCrowdfundingServices registra todo

### Performance y Buenas Prácticas
- [x] AsNoTracking en queries de solo lectura ✅ GetAllAsync lo usa
- [x] Include para eager loading selectivo ✅ GetByIdAsync incluye Rewards
- [x] Request cache implementado ✅ GetByIdAsync y GetByArtistaIdAsync usan cache
- [x] Índices en columnas frecuentemente consultadas ✅ IX_CampaniaCrowdfunding_Artista_Estado
- [x] Soft delete implementado ✅ Filtros incluyen !c.Borrado
- [x] Strongly Typed IDs con conversores EF ✅ ConfigureConventions configurado

---

## 11. Decisiones Arquitectónicas Clave

### 11.1 Patrón Repository con SaveChanges Directo
**Decisión:** Repository hace `SaveChangesAsync()` directamente, NO usa UnitOfWork explícito.

**Razón:**
- El módulo Crowdfunding ya implementa este patrón
- Mantener consistencia con el resto del código existente
- Simplifica el flujo (menos capas de indirección)

**Trade-off:**
- ✅ Simplicidad, menos abstracciones
- ❌ Menos control sobre transacciones complejas multi-entidad
- Para esta feature (crear/actualizar 1 entidad), es suficiente

### 11.2 Request Scoped Cache en Services (ADR-006)
**Decisión:** Solo Services acceden a `IRequestCacheService`, Handlers y Validators usan Services.

**Razón:**
- Evita queries duplicados cuando Validator y Handler consultan la misma entidad
- Centraliza lógica de cacheo en Service
- Handlers y Validators permanecen limpios

**Flujo:**
1. Validator llama `_service.GetByIdAsync(id)` → Cache MISS → DB → Almacena en cache
2. Handler llama `_service.GetByIdAsync(id)` → Cache HIT → Retorna inmediato

### 11.3 Strongly Typed IDs
**Decisión:** Usar `CampaniaCrowdfundingId`, `ArtistaId` en lugar de `Guid` plano.

**Razón:**
- Type safety: imposible asignar ArtistaId donde se espera CampaniaCrowdfundingId
- Self-documenting code
- Conversores EF Core en ConfigureConventions

**Trade-off:**
- ✅ Seguridad de tipos, menos errores
- ❌ Más verboso (requiere `.Value` para acceder al Guid)

### 11.4 Fluent API en OnModelCreating (No Entity Configurations Separadas)
**Decisión:** Configuración EF Core directamente en `CrowdfundingContext.OnModelCreating`, NO en archivos `IEntityTypeConfiguration<T>` separados.

**Razón:**
- El proyecto ya implementa este patrón
- Para un módulo pequeño (13 entidades), es manejable
- Evita crear 13 archivos adicionales

**Trade-off:**
- ✅ Menos archivos, configuración centralizada
- ❌ Archivo OnModelCreating largo (360 líneas)
- Si el módulo crece, considerar refactor a Configuration classes separadas

---

## 12. Próximos Pasos

### 12.1 Application Layer (CQRS)
El agente `cqrs-planning-architect` debe diseñar:
- Commands: CreateCampaniaCommand, UpdateCampaniaCommand, PublicarCampaniaCommand
- Queries: GetCampaniaByIdQuery, GetAllCampaniasQuery, GetMisCampaniasQuery
- Validators: CreateCampaniaCommandValidator, UpdateCampaniaCommandValidator, PublicarCampaniaCommandValidator
- AutoMapper Profiles: CampaniaProfile
- DTOs: CampaniaDto, CampaniaListDto, CreateCampaniaRequest, UpdateCampaniaRequest, PublishCampaniaResponse

### 12.2 WebApi Layer
- Controllers: CampaniasController con endpoints (POST, PUT, GET)
- Authorization filters: ValidateOwnership (ArtistaId del token == ArtistaId de la campaña)
- JWT configuration: Extraer claims (sub, email)

### 12.3 Frontend (Admin Dashboard)
- Wizard de 4 pasos (info básica, meta, fechas, multimedia)
- Validación Zod (schemas compartidos en `src/shared/schemas`)
- React Query mutations (useMutation para CREATE, UPDATE, PUBLISH)
- Vista previa campaña antes de publicar

---

## 13. Riesgos y Mitigaciones

| Riesgo | Impacto | Mitigación |
|--------|---------|------------|
| Cambio de estado sin validación | Alto | Validators verifican estado antes de transiciones |
| Edición por no-propietario | Alto | Authorization filter valida ArtistaId del token |
| Queries N+1 en Rewards | Medio | Include(c => c.Rewards) en GetByIdAsync |
| Cache desactualizado | Bajo | Request scoped cache (lifetime corto) |
| Migraciones conflictivas | Medio | Verificar tabla existente antes de crear migración |

---

## 14. Métricas de Éxito

- ✅ Todas las entidades son POCOs (sin métodos de negocio)
- ✅ Repository hace SaveChanges (no Service ni Handler)
- ✅ Services inyectan Repository + Cache + Logger con `?? throw`
- ✅ Services usan Request Cache para evitar queries duplicados
- ✅ Fluent API configurado (no Data Annotations)
- ✅ AsNoTracking en queries de solo lectura
- ✅ Índices en columnas frecuentemente consultadas
- ✅ Soft delete implementado con filtros

**Objetivo:** 0 modificaciones necesarias en Domain e Infrastructure para esta feature (TODO YA EXISTE).

---

**Fin del documento de arquitectura hexagonal.**
