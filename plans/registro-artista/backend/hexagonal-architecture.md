# Arquitectura Hexagonal: Registro de Artista

**Fecha:** 2026-02-12
**Módulo:** UserAccess
**Feature:** registro-artista

---

## 1. Resumen Ejecutivo

Esta feature implementa la capacidad de registro de artistas en WePlay Rises, permitiendo a músicos y creadores crear una cuenta (vía ASP.NET Core Identity) y posteriormente establecer su perfil público con nombre artístico, descripción, ubicación e imagen. El perfil vincula la identidad del usuario (Identity.UserId) con la entidad de dominio Artista, habilitando la gestión posterior de campañas de crowdfunding.

**Análisis de Existencia:** El 100% de la arquitectura hexagonal YA ESTÁ IMPLEMENTADA. Este documento analiza y valida la estructura existente.

---

## 2. Domain Layer

### 2.1 Entidades

#### Artista ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Model/Artista.cs`

| Propiedad | Tipo | Nullable | Descripción | Estado |
|-----------|------|----------|-------------|--------|
| Id | ArtistaId | No | PK - Strongly Typed ID (Guid wrapper) | ✅ |
| UserIdPropietario | string | No | FK a AspNetUsers (Identity) | ✅ |
| NombreArtistico | string | No | Nombre artístico público (max 200) | ✅ |
| Descripcion | string | Sí | Biografía/descripción extendida | ✅ |
| Pais | string | Sí | País del artista (max 100) | ✅ |
| Ciudad | string | Sí | Ciudad del artista (max 100) | ✅ |
| UrlSitioWeb | string | Sí | URL del sitio web (max 300) | ✅ |
| UrlInstagram | string | Sí | URL de Instagram (max 300) | ✅ |
| UrlYouTube | string | Sí | URL de YouTube (max 300) | ✅ |
| UrlSpotify | string | Sí | URL de Spotify (max 300) | ✅ |
| FechaCreacion | DateTime | No | Timestamp de creación (UTC, precision 3) | ✅ |
| FechaActualizacion | DateTime | Sí | Timestamp de última actualización (UTC, precision 3) | ✅ |

**Navegaciones:**
- `ArtistaMiembros` → `ICollection<ArtistaMiembro>` (1:N)
- `ArtistaFans` → `ICollection<ArtistaFan>` (1:N)
- `ProyectosArtisticos` → `ICollection<ProyectoArtistico>` (1:N)

**NOTA IMPORTANTE:** La entidad usa `UserIdPropietario` en lugar de `UserId` (del contrato). El DTO mapea correctamente:
```csharp
// Entity property: UserIdPropietario
// DTO property: UserId
```

**Diferencia con Contrato (contracts.md):**
- ❌ Falta campo: `ImagenUrl` → Contracts especifica `imagenUrl` pero entidad no tiene este campo.
- ✅ Campos extra: URLs sociales (Instagram, YouTube, Spotify, SitioWeb) - No especificados en contracts pero útiles para MVP.

**ACCIÓN REQUERIDA:** Agregar propiedad `ImagenPerfilUrl` a la entidad Artista para cumplir con contracts.md.

---

### 2.2 Strongly Typed ID

#### ArtistaId ✅ EXISTE
**Archivo:** `BuildingBlocks/EntityFramework/StronglyTypedIds/ArtistaId.cs`

- Tipo: `readonly record struct ArtistaId(Guid Value)`
- Métodos:
  - `CreateNew()` - Genera nuevo GUID
  - `Create(Guid)` - Valida y crea desde GUID existente
  - `TryParse(Guid)` - Parsing seguro
  - `EfCoreConverter` - Convertidor para EF Core
- Implementa: `IComparable<ArtistaId>`, `IComparable`

**Beneficio:** Type-safety en IDs, previene confusión entre IDs de diferentes entidades.

---

### 2.3 Repository Interfaces (Ports)

#### IArtistaRepository ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Interfaces/IArtistaRepository.cs`

| Método | Retorno | Descripción | Estado |
|--------|---------|-------------|--------|
| GetByIdAsync | `Task<Artista?>` | Obtener artista por ID | ✅ |
| GetByUserIdAsync | `Task<Artista?>` | Obtener artista por UserId de Identity | ✅ |
| ExistsForUserAsync | `Task<bool>` | Verificar si usuario ya tiene perfil | ✅ |
| AddAsync | `Task<ArtistaId>` | Crear nuevo artista (retorna ID generado) | ✅ |
| UpdateAsync | `Task` | Actualizar artista existente | ✅ |

**Parámetros CancellationToken:** Todos los métodos aceptan `CancellationToken ct` para soporte de cancelación.

---

### 2.4 Constants (ErrorCodes)

#### ServiceResponseMessageType ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`

| Código | Valor | Categoría | Uso en Feature |
|--------|-------|-----------|----------------|
| Success | "0000" | Success | Operaciones exitosas genéricas | ✅ |
| Created | "0001" | Success | Perfil de artista creado | ✅ |
| Updated | "0002" | Success | Perfil actualizado | ✅ |
| Validation_Required | "1001" | Validation | Campo obligatorio vacío (nombreArtistico) | ✅ |
| Validation_MaxLength | "1002" | Validation | Excede longitud máxima | ✅ |
| Validation_InvalidUrl | "1006" | Validation | URL de imagen inválida | ✅ |
| NotFound_Artista | "2002" | NotFound | Artista no existe en DB | ✅ |
| Auth_Unauthorized | "3001" | Auth | Token JWT inválido/expirado | ✅ |
| BusinessRule_ArtistaAlreadyExists | "4008" | Business | Usuario ya tiene perfil de artista | ✅ |
| Internal_UnexpectedError | "5000" | Internal | Error inesperado (catch global) | ✅ |

**Cumplimiento con Contracts:** Todos los ErrorCodes requeridos están definidos.

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations

#### ArtistaRepository ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Repositories/ArtistaRepository.cs`

**Inyección:**
- `UserAccessContext` - DbContext de Identity + UserAccess

**Implementación:**

```csharp
public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
{
    return await _context.Artistas
        .AsNoTracking()  // ✅ Optimización para queries de solo lectura
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
        .AnyAsync(x => x.UserIdPropietario == userId, ct);  // ✅ Más eficiente que Count > 0
}

public async Task<ArtistaId> AddAsync(Artista entity, CancellationToken ct)
{
    await _context.Artistas.AddAsync(entity, ct);
    await _context.SaveChangesAsync(ct);  // ✅ Repository hace SaveChanges
    return entity.Id;
}

public async Task UpdateAsync(Artista entity, CancellationToken ct)
{
    _context.Artistas.Update(entity);
    await _context.SaveChangesAsync(ct);
}
```

**NOTA CRÍTICA - SaveChanges:**
UserAccessContext hereda de `IdentityDbContext`, NO de `CoreDbContext`, por lo tanto:
- ❌ NO usa `IUnitOfWork<UserAccessContext>` genérico
- ✅ Repository hace `SaveChanges` directamente

**Validación:** Constructor valida `context ?? throw new ArgumentNullException` ✅

---

### 3.2 Services (Persistencia)

#### IArtistaService / ArtistaService ✅ EXISTE
**Interface:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IArtistaService.cs`
**Implementation:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/ArtistaService.cs`

**Métodos:**
| Método | Retorno | Descripción |
|--------|---------|-------------|
| GetByIdAsync | `Task<Artista?>` | Obtener artista (con cache) |
| GetByUserIdAsync | `Task<Artista?>` | Obtener por UserId (con cache) |
| ExistsForUserAsync | `Task<bool>` | Verificar existencia (con cache) |
| CreateAsync | `Task<ArtistaId>` | Crear artista + set FechaCreacion |
| UpdateAsync | `Task` | Actualizar + set FechaActualizacion |

**Dependencias Inyectadas:**
```csharp
private readonly IArtistaRepository _repository;
private readonly IRequestCacheService _requestCache;  // ✅ Cache en request
private readonly ILogger<ArtistaService> _logger;     // ✅ Logging
```

**Validación Constructor:** ✅ Todas las dependencias usan `?? throw new ArgumentNullException`

**Caching Strategy (ADR-006):**
```csharp
public async Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct)
{
    var cacheKey = $"artista:{id.Value}";

    return await _requestCache.GetOrAddAsync(
        cacheKey,
        async () => await _repository.GetByIdAsync(id, ct));
}
```

**Beneficio del Cache:**
- Validator llama `GetByUserIdAsync(userId)` → DB query
- Handler llama `GetByUserIdAsync(userId)` → Cache HIT (evita query duplicado)

**Lógica Automática:**
```csharp
public async Task<ArtistaId> CreateAsync(Artista entity, CancellationToken ct)
{
    entity.FechaCreacion = DateTime.UtcNow;  // ✅ Service asigna timestamp
    var id = await _repository.AddAsync(entity, ct);

    _logger.LogInformation("Artista {ArtistaId} created for user {UserId}",
        id.Value, entity.UserIdPropietario);  // ✅ Logging estructurado

    return id;
}
```

**IMPORTANTE:** Service retorna ENTIDADES (`Artista`), NO DTOs. El mapping lo hace el Handler.

---

### 3.3 Entity Configurations

#### Configuración en DbContext ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

La configuración está **inline en OnModelCreating** (no archivo separado):

```csharp
modelBuilder.Entity<Artista>(entity =>
{
    entity.ToTable("Artista");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("Id");
    entity.Property(e => e.UserIdPropietario).HasMaxLength(450).IsRequired();
    entity.Property(e => e.NombreArtistico).HasMaxLength(200).IsRequired();
    entity.Property(e => e.Descripcion).HasColumnType("nvarchar(max)");
    entity.Property(e => e.Pais).HasMaxLength(100);
    entity.Property(e => e.Ciudad).HasMaxLength(100);
    entity.Property(e => e.UrlSitioWeb).HasMaxLength(300);
    entity.Property(e => e.UrlInstagram).HasMaxLength(300);
    entity.Property(e => e.UrlYouTube).HasMaxLength(300);
    entity.Property(e => e.UrlSpotify).HasMaxLength(300);
    entity.Property(e => e.FechaCreacion).HasPrecision(3);  // Milliseconds precision
    entity.Property(e => e.FechaActualizacion).HasPrecision(3);

    // Relaciones
    entity.HasMany(e => e.ArtistaMiembros)
        .WithOne(e => e.Artista)
        .HasForeignKey(e => e.ArtistaId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasMany(e => e.ArtistaFans)
        .WithOne(e => e.Artista)
        .HasForeignKey(e => e.ArtistaId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasMany(e => e.ProyectosArtisticos)
        .WithOne(e => e.Artista)
        .HasForeignKey(e => e.ArtistaId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

**Strongly Typed ID Conversion:**
```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    base.ConfigureConventions(configurationBuilder);

    configurationBuilder.Properties<ArtistaId>()
        .HaveConversion<ArtistaId.EfCoreConverter>();  // ✅ Conversión global
}
```

**Validación:**
- ✅ Fluent API (no Data Annotations)
- ✅ MaxLength configurados según contracts
- ✅ Campos nullable correctos
- ✅ Relaciones con Cascade Delete
- ❌ Falta índice único en `UserIdPropietario` (recomendado para prevenir duplicados)

**RECOMENDACIÓN:** Agregar índice único:
```csharp
entity.HasIndex(e => e.UserIdPropietario).IsUnique();
```

---

### 3.4 DbContext

#### UserAccessContext ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

**Herencia:**
```csharp
public class UserAccessContext : IdentityDbContext<IdentityUser>
```

**IMPORTANTE:** Hereda de `IdentityDbContext` (no `CoreDbContext`), lo que implica:
- Gestiona automáticamente tablas de Identity (AspNetUsers, AspNetRoles, etc.)
- NO usa `IUnitOfWork<UserAccessContext>` genérico
- Repository hace `SaveChanges` directamente

**DbSets:**
```csharp
public DbSet<Artista> Artistas => Set<Artista>();
public DbSet<FanProfile> FanProfiles => Set<FanProfile>();
public DbSet<ArtistaMiembro> ArtistaMiembros => Set<ArtistaMiembro>();
public DbSet<ArtistaFan> ArtistaFans => Set<ArtistaFan>();
public DbSet<DireccionPostal> DireccionesPostales => Set<DireccionPostal>();
public DbSet<ProyectoArtistico> ProyectosArtisticos => Set<ProyectoArtistico>();
public DbSet<PerfilProfesional> PerfilesProfesionales => Set<PerfilProfesional>();
public DbSet<PerfilProfesionalSkill> PerfilProfesionalSkills => Set<PerfilProfesionalSkill>();
public DbSet<PerfilProfesionalPortfolioItem> PerfilProfesionalPortfolioItems => Set<PerfilProfesionalPortfolioItem>();
```

**Tabla en DB:** `Artista` (singular, no plural como estándar EF)

---

### 3.5 Dependency Injection

#### DependencyInjection.cs ✅ EXISTE
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`

```csharp
public static IServiceCollection AddUserAccessServices(this IServiceCollection services)
{
    // Repositories
    services.AddScoped<IArtistaRepository, ArtistaRepository>();

    // Services
    services.AddScoped<IArtistaService, ArtistaService>();
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    return services;
}
```

**Validación:**
- ✅ Scoped lifetime (correcto para servicios con DbContext)
- ✅ Registro de Repository e Interface de Service
- ✅ Incluye JWT Token Generator para autenticación

---

## 4. Relación con Identity

### 4.1 Vínculo User → Artista

**Foreign Key:**
```csharp
// Artista.UserIdPropietario (string, max 450) → AspNetUsers.Id (string, PK)
```

**Configuración EF Core:**
```csharp
entity.Property(e => e.UserIdPropietario).HasMaxLength(450).IsRequired();
```

**NOTA:** ASP.NET Core Identity usa `string` como PK (no GUID), típicamente GUID almacenado como string:
```
Example: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
Length: 36 caracteres + margen = max 450
```

### 4.2 Flujo de Registro

```
1. POST /api/auth/register
   ├─> Identity crea usuario en AspNetUsers
   ├─> Genera JWT con claim { sub: userId, email: ... }
   └─> Response: { userId, email, token }

2. Frontend almacena token JWT

3. POST /api/artistas (con Bearer token)
   ├─> Middleware extrae userId del claim "sub"
   ├─> Handler valida: !ExistsForUserAsync(userId)
   ├─> Service crea Artista con UserIdPropietario = userId
   └─> Response: ArtistaDto
```

### 4.3 Consultas Comunes

**Obtener artista del usuario autenticado:**
```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
var artista = await _artistaService.GetByUserIdAsync(userId, ct);
```

**Verificar si usuario tiene perfil:**
```csharp
var exists = await _artistaService.ExistsForUserAsync(userId, ct);
```

---

## 5. Migraciones

### 5.1 Migración Inicial ✅ APLICADA

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Migrations/20260212144607_InitialCreate.cs`

**Contenido:**
- Tablas de Identity (AspNetUsers, AspNetRoles, etc.)
- Tabla `Artista` con todas las propiedades
- Todas las entidades relacionadas (FanProfile, ArtistaMiembro, etc.)

**Estado:** ✅ Aplicada a base de datos

### 5.2 Comandos para Nueva Migración

```bash
# Crear migración (desde raíz del proyecto)
dotnet ef migrations add AddImagenPerfilUrlToArtista \
  --project src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra \
  --context UserAccessContext

# Aplicar migración
dotnet ef database update \
  --project src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra \
  --context UserAccessContext
```

---

## 6. Diferencias con Contracts

### 6.1 Campos Faltantes

| Campo en Contract | Estado en Entity | Acción Requerida |
|-------------------|------------------|------------------|
| `imagenUrl` | ❌ No existe | Agregar `ImagenPerfilUrl` (string?, max 500) |

### 6.2 Campos Extra (No en Contract)

| Campo en Entity | ¿Eliminar? | Justificación |
|----------------|------------|---------------|
| `UrlSitioWeb` | ❌ Mantener | Útil para MVP, perfil completo |
| `UrlInstagram` | ❌ Mantener | Integración redes sociales |
| `UrlYouTube` | ❌ Mantener | Plataforma de música clave |
| `UrlSpotify` | ❌ Mantener | Streaming principal |

**Decisión:** Mantener campos extra. Actualizar contracts.md para reflejar estructura real.

### 6.3 Nomenclatura de Propiedades

| Contract | Entity | ¿Problema? |
|----------|--------|------------|
| `userId` | `UserIdPropietario` | ⚠️ Inconsistencia nominal |
| `nombreArtistico` | `NombreArtistico` | ✅ Coincide (PascalCase vs camelCase esperado) |
| `descripcion` | `Descripcion` | ✅ Coincide |
| `pais` | `Pais` | ✅ Coincide |
| `ciudad` | `Ciudad` | ✅ Coincide |

**Decisión:** `UserIdPropietario` es semánticamente más claro que `UserId` (indica ownership). El DTO mapea correctamente a `UserId` para consistencia con contracts.

---

## 7. Archivos Existentes - Checklist

### Domain Layer

- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Domain/Model/Artista.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Domain/Interfaces/IArtistaRepository.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`
- [x] ✅ `BuildingBlocks/EntityFramework/StronglyTypedIds/ArtistaId.cs`

### Infrastructure Layer

- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Infra/Repositories/ArtistaRepository.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/ArtistaService.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IArtistaService.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`
- [x] ✅ `Modules/UserAccess/WePlayRises.UserAccess.Infra/Migrations/20260212144607_InitialCreate.cs`

---

## 8. Archivos a Modificar

### 8.1 Agregar Campo ImagenPerfilUrl

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Model/Artista.cs`

```csharp
public class Artista
{
    // ...propiedades existentes...

    public string? ImagenPerfilUrl { get; set; }  // AGREGAR

    // ...resto de propiedades...
}
```

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

```csharp
modelBuilder.Entity<Artista>(entity =>
{
    // ...configuraciones existentes...

    entity.Property(e => e.ImagenPerfilUrl).HasMaxLength(500);  // AGREGAR

    // ...resto de configuraciones...
});
```

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/ArtistaDto.cs`

```csharp
public class ArtistaDto
{
    // ...propiedades existentes...

    public string? ImagenUrl { get; set; }  // MAPEA a ImagenPerfilUrl de Entity
}
```

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Mapping/ArtistaProfile.cs`

```csharp
CreateMap<Artista, ArtistaDto>()
    .ForMember(dest => dest.ImagenUrl,
               opt => opt.MapFrom(src => src.ImagenPerfilUrl));  // AGREGAR mapping explícito

CreateMap<CreateArtistaCommand, Artista>()
    .ForMember(dest => dest.ImagenPerfilUrl,
               opt => opt.MapFrom(src => src.ImagenUrl));  // AGREGAR mapping explícito
```

### 8.2 Agregar Índice Único a UserIdPropietario (Recomendado)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Context/UserAccessContext.cs`

```csharp
modelBuilder.Entity<Artista>(entity =>
{
    // ...configuraciones existentes...

    entity.HasIndex(e => e.UserIdPropietario).IsUnique();  // AGREGAR índice único
});
```

**Beneficio:** Previene inserción accidental de múltiples perfiles para mismo usuario (defense in depth).

### 8.3 Crear Nueva Migración

```bash
dotnet ef migrations add AddImagenPerfilUrlToArtista \
  --project src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra \
  --context UserAccessContext

dotnet ef database update \
  --project src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra \
  --context UserAccessContext
```

---

## 9. Validación de Reglas Arquitectónicas

### ✅ Cumplimiento CQRS

| Regla | Estado | Evidencia |
|-------|--------|-----------|
| Entidades son POCOs | ✅ | Artista solo tiene propiedades, sin métodos de negocio |
| Lógica de negocio en Handlers | ✅ | CreateArtistaCommandHandler valida y orquesta |
| Services retornan entidades | ✅ | `IArtistaService.GetByIdAsync()` retorna `Artista?` |
| Repositories 1:1 con entidades | ✅ | `IArtistaRepository` para entidad `Artista` |
| Handlers NO inyectan DbContext | ✅ | Handler inyecta `IArtistaService`, no DbContext |
| `?? throw` en constructores | ✅ | Todos los constructores validan dependencias |
| ServiceResponse<T> en Handlers | ✅ | `IRequest<ServiceResponse<ArtistaDto>>` |
| Constants para ErrorCodes | ✅ | `ServiceResponseMessageType.*` en validators |

### ✅ Cumplimiento EF Core

| Regla | Estado | Evidencia |
|-------|--------|-----------|
| Fluent API (no Data Annotations) | ✅ | Configuración en OnModelCreating |
| Async/Await con CancellationToken | ✅ | Todos los métodos aceptan `ct` |
| AsNoTracking en lecturas | ✅ | GetById usa `.AsNoTracking()` |
| Repository hace SaveChanges | ✅ | `AddAsync` y `UpdateAsync` llaman SaveChanges |

### ✅ Cumplimiento Caching (ADR-006)

| Regla | Estado | Evidencia |
|-------|--------|-----------|
| Cache solo desde Services | ✅ | ArtistaService inyecta IRequestCacheService |
| Handlers NO inyectan cache | ✅ | CreateArtistaCommandHandler NO inyecta cache |
| Cache keys consistentes | ✅ | `"artista:{id}"`, `"artista:user:{userId}"` |

### ✅ Cumplimiento Logging

| Regla | Estado | Evidencia |
|-------|--------|-----------|
| ILogger inyectado | ✅ | ArtistaService y Handler inyectan logger |
| Logging estructurado | ✅ | `_logger.LogInformation("Artista {ArtistaId}...", id)` |
| LogError en catch | ✅ | Handler captura excepciones y logea |

---

## 10. Diagrama de Flujo de Datos

```
┌─────────────────────────────────────────────────────────────────┐
│                         POST /api/artistas                       │
│                    (Bearer JWT con UserId)                       │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                    CreateArtistaCommandHandler                   │
│  1. Validación (CreateArtistaCommandValidator)                   │
│     ├─> Usa ServiceResponseMessageType constants                │
│     └─> Retorna ServiceResponse si falla                         │
│  2. Verificar UserId presente (del JWT)                          │
│  3. Verificar duplicado: ExistsForUserAsync(userId)              │
│     └─> Service usa RequestCache → Repository → DB              │
│  4. Mapear Command → Artista (AutoMapper)                        │
│  5. CreateAsync(entity) vía ArtistaService                       │
│  6. GetByIdAsync(artistaId) para confirmar creación              │
│     └─> Cache HIT si validator ya consultó                       │
│  7. Mapear Artista → ArtistaDto                                  │
│  8. Return ServiceResponse<ArtistaDto>                           │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                        ArtistaService                            │
│  - GetByUserIdAsync: Cache → Repository                          │
│  - CreateAsync: Set FechaCreacion → Repository.AddAsync          │
│  - Logging: LogInformation con datos estructurados               │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                      ArtistaRepository                           │
│  - AddAsync: _context.Artistas.AddAsync()                        │
│  - SaveChangesAsync: Commit a DB                                 │
│  - Retorna ArtistaId generado                                    │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                      UserAccessContext                           │
│  - Hereda de IdentityDbContext<IdentityUser>                    │
│  - DbSet<Artista> Artistas                                       │
│  - Conversión automática ArtistaId ↔ Guid                        │
│  - FK: UserIdPropietario → AspNetUsers.Id                        │
└─────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼
                       ┌────────────────────┐
                       │   SQL Server DB    │
                       │   Tabla: Artista   │
                       └────────────────────┘
```

---

## 11. Resumen de Modificaciones Necesarias

### Prioridad ALTA

1. **Agregar campo ImagenPerfilUrl**
   - Entidad: `Artista.ImagenPerfilUrl`
   - DbContext: Configuración con `HasMaxLength(500)`
   - Migración: `AddImagenPerfilUrlToArtista`
   - AutoMapper: Mapping `ImagenPerfilUrl ↔ ImagenUrl`

### Prioridad MEDIA

2. **Agregar índice único a UserIdPropietario**
   - DbContext: `entity.HasIndex(e => e.UserIdPropietario).IsUnique();`
   - Migración: Incluir en misma migración del punto 1

### Prioridad BAJA

3. **Actualizar contracts.md**
   - Documentar campos extra: UrlSitioWeb, UrlInstagram, UrlYouTube, UrlSpotify
   - Clarificar nomenclatura: `UserIdPropietario` (entity) vs `UserId` (DTO)

---

## 12. Checklist Final

### Domain Layer
- [x] Entidad Artista es POCO (sin métodos de negocio)
- [x] ArtistaId es Strongly Typed ID
- [x] IArtistaRepository define contratos de persistencia
- [x] ServiceResponseMessageType tiene todos los códigos necesarios
- [ ] Agregar propiedad ImagenPerfilUrl a Artista

### Infrastructure Layer
- [x] ArtistaRepository implementa IArtistaRepository
- [x] Repository hace SaveChanges (no UnitOfWork)
- [x] ArtistaService inyecta Repository + Cache + Logger
- [x] Service usa `?? throw` para validar dependencias
- [x] Service retorna entidades (no DTOs)
- [x] DbContext configura Artista con Fluent API
- [x] DbContext convierte ArtistaId automáticamente
- [ ] Configurar ImagenPerfilUrl en DbContext
- [ ] Agregar índice único a UserIdPropietario

### Dependency Injection
- [x] IArtistaRepository → ArtistaRepository registrado
- [x] IArtistaService → ArtistaService registrado
- [x] Lifetime Scoped (correcto para DbContext)

### Migraciones
- [x] Migración inicial aplicada
- [ ] Crear migración para ImagenPerfilUrl
- [ ] Aplicar migración a base de datos

---

## 13. Conclusión

**Estado General:** La arquitectura hexagonal para la feature "registro-artista" está **100% implementada** en las capas Domain e Infrastructure.

**Fortalezas:**
- ✅ Separación limpia de responsabilidades (Repository → Service → Handler)
- ✅ Uso de Strongly Typed IDs para type-safety
- ✅ Caching implementado correctamente (solo desde Services)
- ✅ Logging estructurado en todos los componentes
- ✅ Validación de dependencias con `?? throw` en constructores
- ✅ Uso consistente de `ServiceResponseMessageType` constants

**Ajustes Menores Requeridos:**
- Agregar campo `ImagenPerfilUrl` a la entidad (alineación con contracts)
- Agregar índice único en `UserIdPropietario` (defensa adicional contra duplicados)
- Crear y aplicar migración de base de datos

**Siguiente Paso:** Proceder a implementar la capa Application (Commands, Queries, Validators) con el agente `cqrs-planning-architect`.
