# Arquitectura Hexagonal: Registro de Artista

**Fecha:** 2026-01-26
**Modulo:** UserAccess
**Feature:** registro-artista

---

## 1. Resumen Ejecutivo

Esta feature implementa las capas Domain e Infrastructure para permitir el registro de artistas en la plataforma. La entidad Artista ya existe pero requiere repository interfaces, implementations y services que permitan crear, consultar y verificar perfiles de artista vinculados a usuarios de ASP.NET Core Identity. El service incluye caching request-scoped para optimizar validaciones y operaciones CQRS.

---

## 2. Domain Layer

### 2.1 Entidades

#### Artista
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Model/Artista.cs`

**Estado:** Ya existe. No requiere cambios.

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | ArtistaId | No | Primary Key (Strongly Typed ID) |
| UserIdPropietario | string | No | FK a AspNetUsers (Identity) |
| NombreArtistico | string | No | Nombre publico del artista |
| Descripcion | string | Si | Biografia/descripcion del artista |
| Pais | string | Si | Pais de origen |
| Ciudad | string | Si | Ciudad de residencia |
| UrlSitioWeb | string | Si | URL del sitio web oficial |
| UrlInstagram | string | Si | URL perfil Instagram |
| UrlYouTube | string | Si | URL canal YouTube |
| UrlSpotify | string | Si | URL perfil Spotify |
| FechaCreacion | DateTime | No | Timestamp de creacion |
| FechaActualizacion | DateTime | Si | Timestamp de ultima actualizacion |

**Navegaciones:**
- `ArtistaMiembros` -> `ICollection<ArtistaMiembro>` (1:N)
- `ArtistaFans` -> `ICollection<ArtistaFan>` (1:N)
- `ProyectosArtisticos` -> `ICollection<ProyectoArtistico>` (1:N)

**Notas:**
- La entidad usa `ArtistaId` (Strongly Typed ID) segun ADR-005
- `UserIdPropietario` se mapea a `userId` del DTO (field name diferente por legacy)
- Para MVP solo se usan campos basicos: `NombreArtistico`, `Descripcion`, `Pais`, `Ciudad`
- Campo `imagenUrl` del contrato NO existe en entidad actual. Para MVP, usar campo URL existente o agregar si es critico.

### 2.2 Repository Interfaces

#### IArtistaRepository
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Interfaces/IArtistaRepository.cs`

**Estado:** Crear nuevo archivo.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| GetByIdAsync | Task<Artista?> | ArtistaId id, CancellationToken ct | Obtiene artista por ID, incluye navegaciones basicas |
| GetByUserIdAsync | Task<Artista?> | string userId, CancellationToken ct | Obtiene artista por UserId de Identity |
| ExistsForUserAsync | Task<bool> | string userId, CancellationToken ct | Verifica si usuario ya tiene perfil de artista |
| AddAsync | Task<ArtistaId> | Artista entity, CancellationToken ct | Crea nuevo artista, retorna ID generado |
| UpdateAsync | Task | Artista entity, CancellationToken ct | Actualiza artista existente |

**Consideraciones:**
- `GetByIdAsync` debe usar `.Include()` si se necesitan navegaciones para DTOs
- `GetByUserIdAsync` es critico para validar duplicados (un usuario = un artista)
- `ExistsForUserAsync` optimiza validacion sin cargar entidad completa
- Repository hace `SaveChangesAsync` en cada operacion de escritura (AddAsync, UpdateAsync)

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations

#### ArtistaRepository
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Repositories/ArtistaRepository.cs`

**Implementa:** `IArtistaRepository`

**Dependencias:**
- `UserAccessContext` (DbContext del modulo)

**Metodos Clave:**

```csharp
public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
{
    return await _context.Artistas
        .AsNoTracking()  // Solo lectura
        .FirstOrDefaultAsync(x => x.Id == id, ct);
}

public async Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct)
{
    return await _context.Artistas
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserIdPropietario == userId, ct);
}

public async Task<bool> ExistsForUserAsync(string userId, CancellationToken ct)
{
    return await _context.Artistas
        .AnyAsync(x => x.UserIdPropietario == userId, ct);
}

public async Task<ArtistaId> AddAsync(Artista entity, CancellationToken ct)
{
    await _context.Artistas.AddAsync(entity, ct);
    await _context.SaveChangesAsync(ct);  // Repository hace SaveChanges
    return entity.Id;
}

public async Task UpdateAsync(Artista entity, CancellationToken ct)
{
    _context.Artistas.Update(entity);
    await _context.SaveChangesAsync(ct);
}
```

**CRITICO:**
- Queries de lectura usan `AsNoTracking()` para performance
- Repository hace `SaveChangesAsync`, NO el Handler
- `ExistsForUserAsync` usa `AnyAsync` (mas eficiente que cargar entidad)

### 3.2 Services

#### IArtistaService
**Interface:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Interfaces/IArtistaService.cs`

**Estado:** Crear nuevo archivo.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| GetByIdAsync | Task<Artista?> | ArtistaId id, CancellationToken ct | Obtiene artista con caching request-scoped |
| GetByUserIdAsync | Task<Artista?> | string userId, CancellationToken ct | Obtiene artista con caching request-scoped |
| ExistsForUserAsync | Task<bool> | string userId, CancellationToken ct | Verifica existencia, usa caching |
| CreateAsync | Task<ArtistaId> | Artista entity, CancellationToken ct | Crea artista, invalida cache si aplica |
| UpdateAsync | Task | Artista entity, CancellationToken ct | Actualiza artista, invalida cache si aplica |

#### ArtistaService
**Implementation:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/ArtistaService.cs`

**Estado:** Crear nuevo archivo.

**Dependencias:**
- `IArtistaRepository` - Acceso a datos
- `IRequestCacheService` - Caching request-scoped (ADR-006)
- `ILogger<ArtistaService>` - Logging

**Ejemplo Implementacion:**

```csharp
public class ArtistaService : IArtistaService
{
    private readonly IArtistaRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<ArtistaService> _logger;

    public ArtistaService(
        IArtistaRepository repository,
        IRequestCacheService requestCache,
        ILogger<ArtistaService> logger)
    {
        _repository = repository;
        _requestCache = requestCache;
        _logger = logger;
    }

    public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
    {
        var cacheKey = $"artista:{id}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"artista:user:{userId}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByUserIdAsync(userId, ct));
    }

    public async Task<bool> ExistsForUserAsync(string userId, CancellationToken ct)
    {
        var cacheKey = $"artista:exists:user:{userId}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.ExistsForUserAsync(userId, ct));
    }

    public async Task<ArtistaId> CreateAsync(Artista entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation("Artista {ArtistaId} creado para usuario {UserId}",
            id, entity.UserIdPropietario);

        return id;
    }

    public async Task UpdateAsync(Artista entity, CancellationToken ct)
    {
        entity.FechaActualizacion = DateTime.UtcNow;
        await _repository.UpdateAsync(entity, ct);

        _logger.LogInformation("Artista {ArtistaId} actualizado", entity.Id);
    }
}
```

**IMPORTANTE:**
- Service retorna ENTIDADES (Artista), NO DTOs
- Usa `IRequestCacheService` para evitar queries duplicados entre Validator y Handler
- Cache keys consistentes: `artista:{id}`, `artista:user:{userId}`, `artista:exists:user:{userId}`
- Logging de operaciones criticas (Create, Update)
- NO inyecta DbContext directamente

**Beneficio de Caching (ADR-006):**
```
Flujo sin cache:
  Validator.ExistsForUserAsync() -> DB Query
  Handler.GetByUserIdAsync()     -> DB Query (DUPLICADO)

Flujo con cache:
  Validator.ExistsForUserAsync() -> DB Query -> Cache
  Handler.GetByUserIdAsync()     -> Cache HIT (0 queries)
```

### 3.3 Entity Configurations

#### ArtistaConfiguration
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

**Estado:** Ya existe configuracion inline en `OnModelCreating`. No requiere cambios.

**Configuracion Actual:**

| Propiedad | Configuracion EF Core |
|-----------|----------------------|
| Table | "Artista" |
| Id | HasKey, ArtistaId (Strongly Typed) |
| UserIdPropietario | MaxLength(450), IsRequired |
| NombreArtistico | MaxLength(200), IsRequired |
| Descripcion | nvarchar(max) |
| Pais | MaxLength(100) |
| Ciudad | MaxLength(100) |
| UrlSitioWeb | MaxLength(300) |
| UrlInstagram | MaxLength(300) |
| UrlYouTube | MaxLength(300) |
| UrlSpotify | MaxLength(300) |
| FechaCreacion | HasPrecision(3) |
| FechaActualizacion | HasPrecision(3) |

**Navegaciones:**
- `ArtistaMiembros` (1:N, CASCADE)
- `ArtistaFans` (1:N, CASCADE)
- `ProyectosArtisticos` (1:N, CASCADE)

**Nota:** No existe constraint UNIQUE en `UserIdPropietario`. Debe agregarse para garantizar 1 usuario = 1 artista.

**Modificacion Requerida:**

```csharp
// Agregar dentro de la configuracion de Artista en OnModelCreating
entity.HasIndex(e => e.UserIdPropietario).IsUnique();
```

### 3.4 DbContext

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

**Estado:** Ya existe. No requiere cambios en DbSets (Artistas ya esta registrado).

**Cambio Requerido:**
- Agregar indice UNIQUE a `UserIdPropietario` (ver seccion 3.3)

**DbSet Existente:**
```csharp
public DbSet<Artista> Artistas => Set<Artista>();
```

**Strongly Typed ID Converter:**
Ya configurado en `ConfigureConventions`:
```csharp
configurationBuilder.Properties<ArtistaId>()
    .HaveConversion<ArtistaId.EfCoreConverter>();
```

---

## 4. Dependency Injection

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`

**Estado:** Modificar archivo existente.

**Cambios a Aplicar:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using WePlayRises.UserAccess.Domain.Interfaces;
using WePlayRises.UserAccess.Infra.Interfaces;
using WePlayRises.UserAccess.Infra.Repositories;
using WePlayRises.UserAccess.Infra.Services;

namespace WePlayRises.UserAccess.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddUserAccessServices(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IArtistaRepository, ArtistaRepository>();

        // Register services
        services.AddScoped<IArtistaService, ArtistaService>();

        return services;
    }
}
```

**CRITICO:**
- Repositories y Services son **Scoped** (por request HTTP)
- `IRequestCacheService` ya esta registrado en BuildingBlocks (Scoped)
- NO registrar `UserAccessContext` aqui (se registra en WebApi/Startup)

---

## 5. Migraciones

### 5.1 Crear Migracion

**Comando:**
```bash
cd src/api
dotnet ef migrations add AddUniqueIndexToArtistaUserId --project Modules/UserAccess/WePlayRises.UserAccess.Infra --startup-project WebApi
```

**Descripcion:** Agrega indice UNIQUE a `Artista.UserIdPropietario` para prevenir duplicados a nivel de BD.

**Archivo Generado:**
`Modules/UserAccess/WePlayRises.UserAccess.Infra/Migrations/{timestamp}_AddUniqueIndexToArtistaUserId.cs`

**Contenido Esperado:**
```csharp
migrationBuilder.CreateIndex(
    name: "IX_Artista_UserIdPropietario",
    table: "Artista",
    column: "UserIdPropietario",
    unique: true);
```

### 5.2 Aplicar Migracion

**Comando:**
```bash
dotnet ef database update --project Modules/UserAccess/WePlayRises.UserAccess.Infra --startup-project WebApi
```

**Nota:** Si ya existen artistas duplicados en BD, la migracion fallara. Limpiar duplicados antes de aplicar.

---

## 6. Archivos a Crear

```
Modules/UserAccess/
├── WePlayRises.UserAccess.Domain/
│   └── Interfaces/
│       └── IArtistaRepository.cs                       # CREAR
│
└── WePlayRises.UserAccess.Infra/
    ├── Repositories/
    │   └── ArtistaRepository.cs                        # CREAR
    │
    ├── Services/
    │   └── ArtistaService.cs                           # CREAR
    │
    ├── Interfaces/
    │   └── IArtistaService.cs                          # CREAR
    │
    ├── Context/
    │   └── UserAccessContext.cs                        # MODIFICAR (agregar unique index)
    │
    ├── Migrations/
    │   └── {timestamp}_AddUniqueIndexToArtistaUserId.cs  # GENERAR via EF CLI
    │
    └── DependencyInjection.cs                          # MODIFICAR (registrar repo + service)
```

**Total:** 4 archivos nuevos, 2 modificaciones.

---

## 7. Flujo de Datos - Registro de Artista

### Escenario: Crear Perfil de Artista

```
1. POST /api/artistas (con JWT token)
   |
   v
2. Controller extrae UserId del token JWT
   |
   v
3. MediatR envia CreateArtistaCommand
   |
   v
4. CreateArtistaValidator.ValidateAsync()
   |-> ArtistaService.ExistsForUserAsync(userId)
       |-> RequestCache.GetOrAddAsync("artista:exists:user:{userId}")
           |-> Cache MISS
           |-> ArtistaRepository.ExistsForUserAsync(userId)
               |-> DB: SELECT COUNT(*) FROM Artista WHERE UserIdPropietario = @userId
           |-> Cache: almacena resultado (false)
       |-> Retorna: false (OK, usuario NO tiene artista)
   |
   v
5. CreateArtistaCommandHandler.Handle()
   |-> Mapper: Command -> Artista entity
   |-> ArtistaService.CreateAsync(entity)
       |-> Set FechaCreacion = UtcNow
       |-> ArtistaRepository.AddAsync(entity)
           |-> DbContext.Artistas.AddAsync(entity)
           |-> DbContext.SaveChangesAsync()  // Repository hace SaveChanges
           |-> Retorna: ArtistaId generado
       |-> Log: "Artista {id} creado para usuario {userId}"
   |
   v
6. Handler retorna ServiceResponse<Guid> con ID del artista
   |
   v
7. Controller mapea a DTO y retorna 200 OK
```

**Beneficio del Caching:**
Si hubiera una segunda validacion en el Handler que llame a `GetByUserIdAsync`, usaria el cache.

### Escenario: Usuario Ya Tiene Artista

```
1. POST /api/artistas (usuario con artista existente)
   |
   v
2. Validator.ExistsForUserAsync()
   |-> ArtistaService.ExistsForUserAsync(userId)
       |-> RequestCache -> DB Query
       |-> Resultado: true
   |
   v
3. Validator retorna validacion fallida
   |
   v
4. Handler NO se ejecuta
   |
   v
5. Retorna ServiceResponse con error:
   {
     "messages": [{
       "message": "Este usuario ya tiene un perfil de artista",
       "errorCode": "ARTISTA_ALREADY_EXISTS"
     }]
   }
```

---

## 8. Consideraciones de Seguridad

| Aspecto | Implementacion |
|---------|----------------|
| Validacion UserId | Handler DEBE extraer UserId del JWT token, NO del request body |
| Autorizacion | Usuario solo puede crear perfil para si mismo (validar UserId == token.sub) |
| Duplicados | Indice UNIQUE en BD + validacion en Validator |
| SQL Injection | EF Core usa queries parametrizadas (protegido) |

**CRITICO:** El `UserId` del artista DEBE venir del token JWT, no del request. El Command NO debe tener campo `userId`.

---

## 9. Mapeo DTO vs Entidad

### Diferencias entre Contrato y Entidad

| Campo Contrato (DTO) | Campo Entidad | Notas |
|----------------------|---------------|-------|
| userId | UserIdPropietario | Mapear en AutoMapper Profile |
| imagenUrl | ¿No existe? | Agregar campo o usar UrlSitioWeb como placeholder MVP |

**Accion Requerida:**
- Verificar si se agrega campo `ImagenPerfilUrl` a entidad Artista
- O mapear `imagenUrl` a `UrlSitioWeb` temporalmente para MVP

---

## 10. Testing Considerations

### Unit Tests Recomendados

| Clase | Test | Objetivo |
|-------|------|----------|
| ArtistaRepository | GetByUserIdAsync_WhenExists_ReturnsArtista | Verifica query correcta |
| ArtistaRepository | ExistsForUserAsync_WhenNotExists_ReturnsFalse | Verifica validacion duplicados |
| ArtistaService | GetByIdAsync_UsesCaching_AvoidsDuplicateQueries | Verifica RequestCache funciona |
| ArtistaService | CreateAsync_SetsFechaCreacion | Verifica timestamp automatico |

### Integration Tests

| Test | Objetivo |
|------|----------|
| CreateArtista_WithValidData_Returns200 | E2E crear artista |
| CreateArtista_DuplicateUserId_Returns409 | Constraint UNIQUE funciona |
| GetArtistaById_WhenExists_ReturnsDto | Query publica funciona |

---

## 11. Checklist de Validacion

- [ ] Entidades son POCOs sin metodos de negocio
- [ ] Repository interfaces en Domain layer
- [ ] Repository implementations en Infra layer
- [ ] Repository hace SaveChanges, NO Handler
- [ ] Services usan IRequestCacheService (ADR-006)
- [ ] Services retornan entidades, NO DTOs
- [ ] Services inyectan Repository, NO DbContext
- [ ] Indice UNIQUE en UserIdPropietario
- [ ] Entity Configuration usa Fluent API (no Data Annotations)
- [ ] DI registra Repositories y Services como Scoped
- [ ] Logging en operaciones criticas (Create, Update)
- [ ] Cache keys consistentes (`artista:{id}`, `artista:user:{userId}`)

---

## 12. Siguiente Paso Sugerido

Una vez aprobado este plan:

1. **Implementar Domain Layer**
   - Crear `IArtistaRepository.cs`

2. **Implementar Infrastructure Layer**
   - Crear `ArtistaRepository.cs`
   - Crear `IArtistaService.cs` y `ArtistaService.cs`
   - Modificar `UserAccessContext.cs` (unique index)
   - Modificar `DependencyInjection.cs`

3. **Ejecutar Migracion**
   - `dotnet ef migrations add AddUniqueIndexToArtistaUserId`
   - `dotnet ef database update`

4. **Continuar con Application Layer**
   - Ejecutar agente `cqrs-planning-architect` para disenar Commands/Queries/Validators

---

## Referencias

- **ADR-006:** Caching Strategy (RequestCacheService)
- **ADR-005:** Strongly Typed IDs (ArtistaId)
- **ADR-002:** CQRS con MediatR
- **Contratos:** `docs/user-stories/registro-artista/contracts.md`
- **Spec:** `docs/user-stories/registro-artista/feature-spec.md`
