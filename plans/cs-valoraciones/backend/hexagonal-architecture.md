# Arquitectura Hexagonal: cs-valoraciones

**Fecha:** 2026-02-21
**Modulo:** Crowdsourcing
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)

---

## 1. Resumen Ejecutivo

Esta feature introduce el sistema de reputacion bidireccional del modulo Crowdsourcing, permitiendo que artistas y profesionales se valoren mutuamente al completar un acuerdo. La entidad `ValoracionCrowdsourcing` ya existe como POCO en el dominio y su DbSet esta registrado en `CrowdsourcingContext`, pero carece de repository interface, repository implementation, service y configuration EF Core correcta. Esta US completa la infraestructura de persistencia, incluyendo el constraint unico a nivel de base de datos, los indices de rendimiento, y la query de agregacion (AVG + GROUP BY) ejecutada en SQL.

---

## 2. Estado Actual del Codebase

Antes de diseniar los componentes nuevos, es fundamental entender lo que ya existe para no duplicar ni contradecir:

### Ya existe (NO crear de nuevo)

| Artefacto | Ruta | Estado |
|-----------|------|--------|
| `ValoracionCrowdsourcing` (POCO) | `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/ValoracionCrowdsourcing.cs` | Existe. Necesita ajustes menores (ver seccion 3.1) |
| `MaestraTipoValoracion` | `Modules/Core/WePlayRises.Core.Domain/Model/Maestras/MaestraTipoValoracion.cs` | Existe en Core.Domain |
| `DbSet<ValoracionCrowdsourcing> Valoraciones` | `CrowdsourcingContext.cs` linea 25 | Existe |
| Configuracion inline en `OnModelCreating` (basica) | `CrowdsourcingContext.cs` lineas 271-281 | Existe pero INCOMPLETA - falta constraint unico, check constraint, maxlength en Comentario, indice en UserIdValorado |
| Relacion en `AcuerdoCrowdsourcing` | `AcuerdoCrowdsourcing.cs` linea 65 | Existe - `ICollection<ValoracionCrowdsourcing> Valoraciones` |
| Relacion en `AcuerdoCrowdsourcing` config | `CrowdsourcingContext.cs` linea 168-169 | Existe - `HasMany(e => e.Valoraciones).WithOne(e => e.Acuerdo).HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.Cascade)` |
| `ServiceResponseMessageType.cs` (dominio) | `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs` | Existe. Necesita 2 nuevas constantes |
| `EstadoAcuerdoConstants.cs` | `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/EstadoAcuerdoConstants.cs` | Existe - `Completado = 2` |

### NO existe (crear en esta US)

| Artefacto | Accion |
|-----------|--------|
| `IValoracionCrowdsourcingRepository` | Crear |
| `ValoracionCrowdsourcingRepository` | Crear |
| `IValoracionCrowdsourcingService` | Crear |
| `ValoracionCrowdsourcingService` | Crear |
| `ValoracionCrowdsourcingConfiguration` | Crear (mover configuracion inline a clase separada) |
| Migracion `AddValoracionesCrowdsourcing` | Crear |
| 2 constantes en `ServiceResponseMessageType.cs` | Agregar |

### Discrepancia critica detectada

El contrato (`contracts.md`) define `Validation_InvalidRange = "1014"` para la puntuacion fuera de rango. Sin embargo, el `ServiceResponseMessageType.cs` actual ya tiene `Validation_InvalidRange = "1009"`. El codigo `"1014"` del contrato no coincide con lo existente.

**Decision de arquitectura:** Mantener `Validation_InvalidRange = "1009"` (ya existente) y usar ese codigo en el validator. El contrato sera alineado con el valor real del codebase. NO agregar una constante duplicada con codigo "1014".

La unica constante genuinamente nueva es `BusinessRule_DuplicateAction = "4017"` (el contrato propone "4014" pero ese codigo ya esta ocupado por `BusinessRule_InvalidState = "4014"`).

**Constantes a agregar:**

```
// El codigo 4014 ya esta ocupado por BusinessRule_InvalidState
// Asignar 4017 como siguiente libre en el rango Business Rules
public const string BusinessRule_DuplicateValoracion = "4017";

// El codigo 1014 NO existe - se puede agregar si se quiere distinguir
// del InvalidRange generico (1009). Decision: agregar como alias especifico.
public const string Validation_InvalidRange_Puntuacion = "1014";
```

**Alternativa mas conservadora (recomendada):** Usar `Validation_InvalidRange = "1009"` ya existente para la puntuacion, y agregar solo `BusinessRule_DuplicateValoracion = "4017"`. Esto minimiza cambios al codebase existente.

---

## 3. Domain Layer

### 3.1 Entidad: ValoracionCrowdsourcing

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/ValoracionCrowdsourcing.cs`

**NOTA CRITICA:** La entidad ya existe. Se deben revisar dos puntos:

1. `TipoValoracionId` es actualmente `int` (NOT NULL). El feature-spec lo define como **nullable** (`int?`). Requiere ajuste en el POCO y en la migracion.
2. `Puntuacion` es actualmente `byte`. El feature-spec lo define como `int`. Ambos son validos; `byte` es mas eficiente para rango 1-5. Mantener `byte` y documentar la decision.

**Estado final del POCO tras ajustes:**

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `Id` | `Guid` | No | PK, generado por la aplicacion |
| `AcuerdoId` | `AcuerdoCrowdsourcingId` | No | FK -> AcuerdoCrowdsourcing (StronglyTypedId) |
| `UserIdAutor` | `string` | No | FK -> Identity.User. El que emite la valoracion. MaxLength 450 |
| `UserIdValorado` | `string` | No | FK -> Identity.User. El que recibe la valoracion. MaxLength 450 |
| `Puntuacion` | `byte` | No | Estrellas 1-5. Tipo byte es suficiente y eficiente |
| `TipoValoracionId` | `int?` | Si | FK -> MaestraTipoValoracion. Nullable (MVP, campo opcional) |
| `Comentario` | `string?` | Si | Texto libre. MaxLength 1000 |
| `FechaCreacion` | `DateTime` | No | UTC. Precision(3) |

**Navigation properties:**

| Property | Tipo | Descripcion |
|----------|------|-------------|
| `Acuerdo` | `AcuerdoCrowdsourcing` | Navigation hacia el acuerdo padre |

**Cambio requerido en el POCO:**

```
// ANTES (actual):
public int TipoValoracionId { get; set; }  // int NOT NULL

// DESPUES (correcto segun feature-spec):
public int? TipoValoracionId { get; set; }  // int? nullable
```

**No se necesita navigation property hacia `MaestraTipoValoracion`** en MVP porque esa entidad vive en `Core.Domain` (modulo diferente) y la separacion de modulos lo desaconseja.

---

### 3.2 Constraint de dominio: UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor

Este constraint se materializa en dos capas:

- **Aplicacion:** El handler verifica la unicidad via `ExisteValoracionAsync` antes de persistir (primera linea de defensa).
- **Base de datos:** Indice unico sobre `(AcuerdoId, UserIdAutor)` en la migracion (segunda linea de defensa, captura condiciones de carrera).

---

### 3.3 ServiceResponseMessageType: Constantes a agregar

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar al final de la seccion Business Rules (4000-4999):**

| Constante | Codigo | Uso |
|-----------|--------|-----|
| `BusinessRule_DuplicateValoracion` | `"4017"` | El usuario ya dejo una valoracion para este acuerdo |

**Constantes existentes a reutilizar en esta feature (sin cambios):**

| Constante | Codigo | Uso en esta feature |
|-----------|--------|---------------------|
| `Validation_Required` | `"1001"` | Puntuacion no enviada |
| `Validation_MaxLength` | `"1002"` | Comentario > 1000 chars |
| `Validation_InvalidRange` | `"1009"` | Puntuacion fuera del rango 1-5 |
| `NotFound_Entity` | `"2000"` | Usuario no encontrado |
| `NotFound_Acuerdo` | `"2011"` | Acuerdo no encontrado |
| `Auth_Unauthorized` | `"3001"` | Token invalido/expirado |
| `Auth_Forbidden` | `"3002"` | Usuario no es participante del acuerdo |
| `BusinessRule_InvalidState` | `"4014"` | Acuerdo no esta en estado Completado |
| `Internal_UnexpectedError` | `"5000"` | Excepcion no controlada |
| `Created` | `"0001"` | Valoracion creada exitosamente |

---

### 3.4 Repository Interface (Port)

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IValoracionCrowdsourcingRepository.cs`

**Convencion del modulo:** Las interfaces de repositorio viven en `Application/Interfaces/Repositories/`, no en `Domain/Interfaces/`. Esto es consistente con todos los repositorios existentes del modulo Crowdsourcing.

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `AddAsync` | `Task<Guid> AddAsync(ValoracionCrowdsourcing entity, CancellationToken ct)` | Persiste la nueva valoracion y retorna el Id generado. Hace `SaveChangesAsync` internamente. |
| `ExisteValoracionAsync` | `Task<bool> ExisteValoracionAsync(AcuerdoCrowdsourcingId acuerdoId, string userIdAutor, CancellationToken ct)` | Verifica si ya existe una valoracion para ese acuerdo y autor. Usado por el handler para prevenir duplicados. |
| `GetResumenByUserIdAsync` | `Task<ValoracionResumenData> GetResumenByUserIdAsync(string userId, CancellationToken ct)` | Ejecuta AVG + COUNT + GROUP BY a nivel SQL. Retorna tipo de datos interno (no DTO). |
| `GetByUserIdPagedAsync` | `Task<(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)> GetByUserIdPagedAsync(string userId, int page, int pageSize, CancellationToken ct)` | Paginacion con Skip/Take, orden FechaCreacion DESC. AsNoTracking. |

**Tipo auxiliar `ValoracionResumenData` (record de dominio):**

Este tipo transporta los datos crudos de la query de agregacion desde el repositorio hacia el service. No es un DTO de aplicacion; es un contenedor de datos calculados en SQL.

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/ValoracionResumenData.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PuntuacionMedia` | `decimal?` | AVG(Puntuacion). Null si totalValoraciones == 0 |
| `TotalValoraciones` | `int` | COUNT(*) |
| `Distribucion` | `Dictionary<int, int>` | Histograma: clave = estrella (1-5), valor = conteo. Siempre 5 entradas |

---

### 3.5 Service Interface (Port)

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IValoracionCrowdsourcingService.cs`

**Convencion del modulo:** Las interfaces de servicio viven en `Application/Interfaces/Services/`.

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `CreateAsync` | `Task<Guid> CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct)` | Persiste la valoracion y retorna el Id. Delega a repository. |
| `ExisteValoracionAsync` | `Task<bool> ExisteValoracionAsync(AcuerdoCrowdsourcingId acuerdoId, string userIdAutor, CancellationToken ct)` | Verificacion con cache de request para evitar query duplicado entre validator y handler. |
| `GetResumenByUserIdAsync` | `Task<ValoracionResumenData> GetResumenByUserIdAsync(string userId, CancellationToken ct)` | Delegacion directa al repository. Sin cache (datos mutables). |
| `GetByUserIdPagedAsync` | `Task<(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)> GetByUserIdPagedAsync(string userId, int page, int pageSize, CancellationToken ct)` | Delegacion directa al repository. Sin cache. |

---

## 4. Infrastructure Layer

### 4.1 EF Core Configuration: ValoracionCrowdsourcingConfiguration

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/ValoracionCrowdsourcingConfiguration.cs`

**Motivacion:** La configuracion actual de `ValoracionCrowdsourcing` esta inline en `CrowdsourcingContext.OnModelCreating` (lineas 271-281) y esta incompleta. Se debe:
1. Crear la clase de configuracion separada siguiendo el patron de `PlantillaProyectoConfiguration`.
2. Eliminar la configuracion inline de `OnModelCreating`.
3. Agregar `modelBuilder.ApplyConfiguration(new ValoracionCrowdsourcingConfiguration())` en `OnModelCreating`.

**Especificacion completa de la configuracion:**

```
Tabla: "ValoracionCrowdsourcing"

PK:
  - HasKey(e => e.Id)
  - Property(e => e.Id).ValueGeneratedNever()  // La app genera el Guid

Propiedades:
  - AcuerdoId: HasColumnName("Acuerdo_Id")    // Consistente con otras FK del modulo
  - UserIdAutor: HasMaxLength(450).IsRequired()
  - UserIdValorado: HasMaxLength(450).IsRequired()
  - Puntuacion: IsRequired()
    HasColumnType("tinyint")                   // byte mapea a tinyint en SQL Server
    HasAnnotation o HasCheckConstraint:
      "CK_ValoracionCrowdsourcing_Puntuacion_Rango"
      sql: "Puntuacion >= 1 AND Puntuacion <= 5"
  - TipoValoracionId: HasColumnName("TipoValoracion_Id").IsRequired(false)
  - Comentario: HasMaxLength(1000).IsRequired(false)
  - FechaCreacion: IsRequired().HasPrecision(3)

FK y relaciones:
  - HasOne(e => e.Acuerdo)
    .WithMany(e => e.Valoraciones)
    .HasForeignKey(e => e.AcuerdoId)
    .OnDelete(DeleteBehavior.Cascade)
    NOTA: Esta relacion YA esta configurada en el bloque de AcuerdoCrowdsourcing
    en OnModelCreating. Al mover a Configuration class, verificar que no quede duplicada.
    Opcion A: Configurarla solo desde ValoracionCrowdsourcingConfiguration
    Opcion B: Mantenerla en el bloque de AcuerdoCrowdsourcing (lado "uno")
    DECISION: Configurarla desde el bloque de AcuerdoCrowdsourcing (ya existe ahi).
    En ValoracionCrowdsourcingConfiguration NO repetir HasOne/WithMany.

Constraint unico (segunda linea de defensa):
  - HasIndex(e => new { e.AcuerdoId, e.UserIdAutor })
    .IsUnique()
    .HasDatabaseName("UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor")

Indices de rendimiento:
  - HasIndex(e => e.UserIdValorado)
    .HasDatabaseName("IX_ValoracionCrowdsourcing_UserIdValorado")
    // Para las queries de GET /usuarios/{userId}/valoraciones

  - HasIndex(e => new { e.UserIdValorado, e.FechaCreacion })
    .HasDatabaseName("IX_ValoracionCrowdsourcing_UserIdValorado_FechaCreacion")
    // Cubre el ORDER BY FechaCreacion DESC de la query paginada
```

**Nota sobre CHECK constraint en EF Core 8:**

En EF Core 8, `HasCheckConstraint` es el metodo correcto para CHECK constraints:

```
builder.HasCheckConstraint(
    "CK_ValoracionCrowdsourcing_Puntuacion_Rango",
    "Puntuacion >= 1 AND Puntuacion <= 5");
```

Este constraint se incluye en la migracion generada automaticamente.

---

### 4.2 Repository Implementation

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/ValoracionCrowdsourcingRepository.cs`

**Implementa:** `IValoracionCrowdsourcingRepository`
**Inyecta:** `CrowdsourcingContext` (NO DbContext generico, sino el contexto del modulo)

**Especificacion por metodo:**

#### `AddAsync`

- Llama a `_context.Valoraciones.AddAsync(entity, ct)`
- Llama a `_context.SaveChangesAsync(ct)`
- Retorna `entity.Id`
- El repository hace SaveChanges (no el service, no el handler)
- NOTA: Si el constraint unico viola, EF lanza `DbUpdateException`. El service o handler captura y convierte en `BusinessRule_DuplicateValoracion`.

#### `ExisteValoracionAsync`

- Query: `_context.Valoraciones.AsNoTracking().AnyAsync(v => v.AcuerdoId == acuerdoId && v.UserIdAutor == userIdAutor, ct)`
- Retorna `bool`
- Usa `AsNoTracking()` porque es una query de solo lectura

#### `GetResumenByUserIdAsync`

**Critico:** Esta query se ejecuta a nivel SQL, NO en memoria.

Estrategia de implementacion con LINQ (traducible a SQL por EF Core):

```
Paso 1 - Query base:
  var query = _context.Valoraciones
      .AsNoTracking()
      .Where(v => v.UserIdValorado == userId);

Paso 2 - Calcular total y media en una sola query:
  var stats = await query
      .GroupBy(_ => 1)                           // Agrupa todo en un grupo
      .Select(g => new {
          Total = g.Count(),
          Media = (decimal?)g.Average(v => (decimal)v.Puntuacion)
      })
      .FirstOrDefaultAsync(ct);

  // Si no hay valoraciones: stats es null -> PuntuacionMedia = null, Total = 0

Paso 3 - Calcular histograma (GROUP BY Puntuacion):
  var distribucionRaw = await query
      .GroupBy(v => v.Puntuacion)
      .Select(g => new { Puntuacion = (int)g.Key, Conteo = g.Count() })
      .ToListAsync(ct);

Paso 4 - Construir distribucion completa (siempre 5 entradas):
  var distribucion = Enumerable.Range(1, 5)
      .ToDictionary(
          estrella => estrella,
          estrella => distribucionRaw.FirstOrDefault(d => d.Puntuacion == estrella)?.Conteo ?? 0);

Paso 5 - Construir ValoracionResumenData:
  return new ValoracionResumenData
  {
      PuntuacionMedia = stats?.Media.HasValue == true
          ? Math.Round(stats.Media.Value, 1, MidpointRounding.AwayFromZero)
          : null,
      TotalValoraciones = stats?.Total ?? 0,
      Distribucion = distribucion
  };
```

**Nota de performance:** EF Core traduce los `GroupBy` anteriores a SQL `GROUP BY`. Se generan 2 queries SQL (stats + histograma), lo cual es eficiente para el volumen esperado en MVP. Si escala, se puede consolidar en una sola query con SQL raw o una vista.

#### `GetByUserIdPagedAsync`

```
Query:
  var query = _context.Valoraciones
      .AsNoTracking()
      .Where(v => v.UserIdValorado == userId);

  var totalCount = await query.CountAsync(ct);

  var items = await query
      .OrderByDescending(v => v.FechaCreacion)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync(ct);

  return (items, totalCount);
```

**Nota:** El repository retorna las entidades `ValoracionCrowdsourcing` con las propiedades basicas. El handler (o service) se encarga de enriquecer con `AutorNombre` y `AutorImagenUrl` proyectando desde otros servicios (Artista, PerfilProfesional). Esta responsabilidad de enriquecimiento se resuelve en la capa Application, no en el repository.

---

### 4.3 Service Implementation

**Archivo nuevo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/ValoracionCrowdsourcingService.cs`

**Implementa:** `IValoracionCrowdsourcingService`

**Constructor - dependencias requeridas:**

| Dependencia | Tipo | Descripcion |
|-------------|------|-------------|
| `_repository` | `IValoracionCrowdsourcingRepository` | Acceso a persistencia |
| `_requestCache` | `IRequestCacheService` | Cache de request para `ExisteValoracionAsync` |
| `_logger` | `ILogger<ValoracionCrowdsourcingService>` | Logging de operaciones |

**NOTA:** Este servicio NO necesita `CrowdsourcingContext` directamente. Toda interaccion con DB pasa por el repository. NO inyectar el contexto.

**NOTA:** NO se inyecta `IUnitOfWork` porque el proyecto Crowdsourcing usa el patron de `SaveChangesAsync` directo en el repository (ver `NecesidadCrowdsourcingRepository.AddAsync`, `MensajeCrowdsourcingRepository.AddAsync`). La transaccion se maneja en el repository para operaciones complejas via `_context.Database.BeginTransactionAsync`. Para `AddAsync` simple, el SaveChanges del repository es suficiente.

**Especificacion por metodo:**

#### `CreateAsync`

- Delega a `_repository.AddAsync(entity, ct)`
- Loguea `LogInformation` con el Id creado
- Retorna el `Guid` del Id

#### `ExisteValoracionAsync`

- Usa cache con clave: `$"valoracion:existe:{acuerdoId.Value}:{userIdAutor}"`
- Delega a `_requestCache.GetOrAddAsync(cacheKey, async () => await _repository.ExisteValoracionAsync(acuerdoId, userIdAutor, ct))`
- **Beneficio del cache:** El validator llama a este metodo en su validacion cruzada, y el handler lo llama de nuevo para la verificacion de negocio. Con el cache, solo se ejecuta una query SQL por request.

#### `GetResumenByUserIdAsync`

- Delega directamente a `_repository.GetResumenByUserIdAsync(userId, ct)`
- Sin cache (los datos cambian al crear nuevas valoraciones)
- Retorna `ValoracionResumenData`

#### `GetByUserIdPagedAsync`

- Delega directamente a `_repository.GetByUserIdPagedAsync(userId, page, pageSize, ct)`
- Sin cache
- Retorna `(IReadOnlyList<ValoracionCrowdsourcing> Items, int TotalCount)`

---

### 4.4 Cambios en CrowdsourcingContext

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

**Cambio 1 - Eliminar configuracion inline de `ValoracionCrowdsourcing`:**

Eliminar el bloque de las lineas 271-281:
```
// ELIMINAR este bloque:
modelBuilder.Entity<ValoracionCrowdsourcing>(entity =>
{
    entity.ToTable("ValoracionCrowdsourcing");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id");
    entity.Property(e => e.UserIdAutor).HasMaxLength(450).IsRequired();
    entity.Property(e => e.UserIdValorado).HasMaxLength(450).IsRequired();
    entity.Property(e => e.TipoValoracionId).HasColumnName("TipoValoracion_Id");
    entity.Property(e => e.Comentario).HasColumnType("nvarchar(max)");
    entity.Property(e => e.FechaCreacion).HasPrecision(3);
});
```

**Cambio 2 - Agregar `ApplyConfiguration`:**

En la seccion de `// Templates & Maestras (external configurations)`, agregar:
```csharp
modelBuilder.ApplyConfiguration(new ValoracionCrowdsourcingConfiguration());
```

**Cambio 3 - Mantener el DbSet (ya existe):**

```csharp
public DbSet<ValoracionCrowdsourcing> Valoraciones => Set<ValoracionCrowdsourcing>();  // Ya existe - NO tocar
```

**Cambio 4 - La relacion en AcuerdoCrowdsourcing (ya existe, NO tocar):**

```csharp
// Ya configurada en el bloque de AcuerdoCrowdsourcing:
entity.HasMany(e => e.Valoraciones).WithOne(e => e.Acuerdo)
    .HasForeignKey(e => e.AcuerdoId).OnDelete(DeleteBehavior.Cascade);
```

La `ValoracionCrowdsourcingConfiguration` NO debe repetir esta relacion para evitar conflicto de configuracion duplicada.

---

### 4.5 Migracion: AddValoracionesCrowdsourcing

**Nombre de la migracion:** `AddValoracionesCrowdsourcing`

**Comando para generar:**

```bash
dotnet ef migrations add AddValoracionesCrowdsourcing \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi \
  --context CrowdsourcingContext
```

**Cambios que debe incluir la migracion:**

| Operacion | Detalle |
|-----------|---------|
| ALTER TABLE `ValoracionCrowdsourcing` | Cambiar `TipoValoracionId` de `int NOT NULL` a `int NULL` |
| ALTER TABLE `ValoracionCrowdsourcing` | Cambiar `Comentario` de `nvarchar(max)` a `nvarchar(1000)` |
| ALTER TABLE `ValoracionCrowdsourcing` | Cambiar `Puntuacion` column type a `tinyint` si no lo es ya |
| ADD CONSTRAINT `CK_ValoracionCrowdsourcing_Puntuacion_Rango` | `CHECK (Puntuacion >= 1 AND Puntuacion <= 5)` |
| CREATE UNIQUE INDEX `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor` | `ON ValoracionCrowdsourcing (Acuerdo_Id, UserIdAutor)` |
| CREATE INDEX `IX_ValoracionCrowdsourcing_UserIdValorado` | `ON ValoracionCrowdsourcing (UserIdValorado)` |
| CREATE INDEX `IX_ValoracionCrowdsourcing_UserIdValorado_FechaCreacion` | `ON ValoracionCrowdsourcing (UserIdValorado, FechaCreacion DESC)` |

**Nota:** La tabla `ValoracionCrowdsourcing` ya existe en la BD (creada en la migracion `AddAcuerdosEntregablesAndMilestones`). Esta nueva migracion es `ALTER`, no `CREATE TABLE`.

**Aplicar migracion:**

```bash
dotnet ef database update --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi \
  --context CrowdsourcingContext
```

---

### 4.6 Dependency Injection

**Archivo existente:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

**Agregar en la seccion `// Repositories`:**

```csharp
services.AddScoped<IValoracionCrowdsourcingRepository, ValoracionCrowdsourcingRepository>();
```

**Agregar en la seccion `// Services`:**

```csharp
services.AddScoped<IValoracionCrowdsourcingService, ValoracionCrowdsourcingService>();
```

---

## 5. Diagrama de Dependencias entre Capas

```
┌─────────────────────────────────────────────────────────────────────┐
│  WebApi Layer                                                       │
│  ValoracionesController                                             │
│  POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones          │
│  GET  /api/crowdsourcing/usuarios/{userId}/valoraciones             │
└──────────────────────────┬──────────────────────────────────────────┘
                           │ MediatR.Send()
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  Application Layer                                                  │
│                                                                     │
│  CreateValoracionCommand + CreateValoracionCommandHandler           │
│  GetValoracionesByUserQuery + GetValoracionesByUserQueryHandler      │
│  CreateValoracionValidator                                          │
│  ValoracionProfile (AutoMapper)                                     │
│                                                                     │
│  Depende de:                                                        │
│  - IValoracionCrowdsourcingService (interface en Application)       │
│  - IAcuerdoCrowdsourcingService (interface en Application)          │
│  - IArtistaService (interface del modulo UserAccess/Crowdsourcing)  │
└────────────────┬────────────────────────────────────────────────────┘
                 │ (ports / interfaces)
                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│  Domain Layer (Ports)                                               │
│  [Application/Interfaces/Repositories/]                             │
│  IValoracionCrowdsourcingRepository                                 │
│  ValoracionResumenData (record de datos)                            │
│                                                                     │
│  [Application/Interfaces/Services/]                                 │
│  IValoracionCrowdsourcingService                                    │
│                                                                     │
│  [Domain/Model/]                                                    │
│  ValoracionCrowdsourcing (POCO)                                     │
│  AcuerdoCrowdsourcing (POCO existente - navigation Valoraciones)    │
│                                                                     │
│  [Domain/Constants/]                                                │
│  ServiceResponseMessageType (BusinessRule_DuplicateValoracion 4017) │
│  EstadoAcuerdoConstants (Completado = 2, ya existe)                 │
└────────────────┬────────────────────────────────────────────────────┘
                 │ (adapters / implementations)
                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│  Infrastructure Layer (Adapters)                                    │
│                                                                     │
│  ValoracionCrowdsourcingService                                     │
│  - IValoracionCrowdsourcingRepository (inyectado)                   │
│  - IRequestCacheService (inyectado, para ExisteValoracion)          │
│  - ILogger<ValoracionCrowdsourcingService> (inyectado)              │
│                                                                     │
│  ValoracionCrowdsourcingRepository                                  │
│  - CrowdsourcingContext (inyectado)                                 │
│  - AddAsync -> SaveChangesAsync                                     │
│  - ExisteValoracionAsync -> AnyAsync (AsNoTracking)                 │
│  - GetResumenByUserIdAsync -> GroupBy -> SQL AVG/COUNT              │
│  - GetByUserIdPagedAsync -> OrderBy/Skip/Take (AsNoTracking)        │
│                                                                     │
│  ValoracionCrowdsourcingConfiguration                               │
│  - ToTable("ValoracionCrowdsourcing")                               │
│  - Constraint unico (AcuerdoId, UserIdAutor)                        │
│  - CHECK Puntuacion 1-5                                             │
│  - Indices de rendimiento                                           │
│                                                                     │
│  CrowdsourcingContext (modificado)                                  │
│  - DbSet<ValoracionCrowdsourcing> Valoraciones (ya existe)          │
│  - ApplyConfiguration(new ValoracionCrowdsourcingConfiguration())   │
└─────────────────────────────────────────────────────────────────────┘

Flujo de cache dentro del request (ADR-006):

  CreateValoracionCommandHandler
       |
       ├─> CreateValoracionValidator.ValidateAsync(request)
       |        |
       |        └─> IValoracionCrowdsourcingService.ExisteValoracionAsync(acuerdoId, userId)
       |                  |
       |                  └─> IRequestCacheService.GetOrAddAsync("valoracion:existe:{acuerdoId}:{userId}")
       |                            |
       |                            └─> [CACHE MISS] IValoracionCrowdsourcingRepository.ExisteValoracionAsync()
       |                                              -> SQL: SELECT 1 WHERE AcuerdoId = ? AND UserIdAutor = ?
       |
       └─> Handler.Handle()
                |
                └─> IValoracionCrowdsourcingService.ExisteValoracionAsync(acuerdoId, userId)
                          |
                          └─> IRequestCacheService.GetOrAddAsync("valoracion:existe:{acuerdoId}:{userId}")
                                        |
                                        └─> [CACHE HIT] Retorna inmediato - SIN query SQL
```

---

## 6. Consideraciones de Diseno Especificas

### 6.1 Por que el repository retorna entidades y no DTOs proyectados

El metodo `GetByUserIdPagedAsync` retorna `IReadOnlyList<ValoracionCrowdsourcing>` y no objetos proyectados con `AutorNombre`. Esto es correcto porque:

1. Las reglas del proyecto exigen que los repositories retornen entidades (nunca DTOs).
2. El enriquecimiento con `AutorNombre` y `AutorImagenUrl` requiere consultar otros servicios (`IArtistaService`, `IPerfilProfesionalService`), lo cual no pertenece al repository.
3. El handler de `GetValoracionesByUserQuery` es el lugar correcto para orquestar el enriquecimiento: obtiene la pagina de valoraciones del service, luego para cada `UserIdAutor` resuelve el nombre via el servicio correspondiente.

### 6.2 Por que no se usa IUnitOfWork en este servicio

El modulo Crowdsourcing no sigue el patron `IUnitOfWork` en sus servicios. Todos los repositories del modulo llaman `SaveChangesAsync` directamente (ver `NecesidadCrowdsourcingRepository`, `MensajeCrowdsourcingRepository`, `PropuestaCrowdsourcingRepository`). Para operaciones transaccionales complejas se usa `_context.Database.BeginTransactionAsync` directamente en el service (ver `NecesidadCrowdsourcingService.CerrarAsync`). Esta consistencia con el codebase existente prevalece sobre la convension abstracta de los templates.

### 6.3 Relacion con MaestraTipoValoracion

La maestra `MaestraTipoValoracion` existe en `Core.Domain.Model.Maestras`. En MVP el campo `TipoValoracionId` es nullable y no se usa activamente. La navigation property hacia `MaestraTipoValoracion` NO se agrega en esta US porque:

- `MaestraTipoValoracion` vive en `Core.Domain`, un modulo diferente.
- Agregar navigation properties cross-modulo rompe la separacion de modulos.
- Si en el futuro se necesita el nombre del tipo, se puede hacer un join en el query o una consulta separada al servicio de maestras.

### 6.4 Calculo del UserIdValorado (logica de negocio - NO va en el repository ni en el service)

La logica de determinar quien es el `UserIdValorado` (si el autor es el artista, el valorado es el proveedor, y viceversa) es logica de negocio pura. Pertenece al `CreateValoracionCommandHandler`, no al service ni al repository. El service de infraestructura simplemente persiste la entidad ya construida con el `UserIdValorado` calculado.

---

## 7. Archivos a Crear / Modificar

### 7.1 Archivos nuevos

```
src/api/Modules/Crowdsourcing/
│
├── WePlayRises.Crowdsourcing.Application/
│   └── Interfaces/
│       ├── Repositories/
│       │   ├── IValoracionCrowdsourcingRepository.cs       [NUEVO]
│       │   └── ValoracionResumenData.cs                    [NUEVO]
│       └── Services/
│           └── IValoracionCrowdsourcingService.cs          [NUEVO]
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Data/
    │   └── Configurations/
    │       └── ValoracionCrowdsourcingConfiguration.cs     [NUEVO]
    ├── Repositories/
    │   └── ValoracionCrowdsourcingRepository.cs            [NUEVO]
    ├── Services/
    │   └── ValoracionCrowdsourcingService.cs               [NUEVO]
    └── Migrations/
        └── {timestamp}_AddValoracionesCrowdsourcing.cs    [NUEVO - generado por EF CLI]
```

### 7.2 Archivos a modificar

```
src/api/Modules/Crowdsourcing/
│
├── WePlayRises.Crowdsourcing.Domain/
│   ├── Model/
│   │   └── ValoracionCrowdsourcing.cs                     [MODIFICAR - TipoValoracionId int -> int?]
│   └── Constants/
│       └── ServiceResponseMessageType.cs                  [MODIFICAR - agregar BusinessRule_DuplicateValoracion "4017"]
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Context/
    │   └── CrowdsourcingContext.cs                        [MODIFICAR - mover config inline, agregar ApplyConfiguration]
    └── DependencyInjection.cs                             [MODIFICAR - registrar nuevo repo y service]
```

---

## 8. Checklist de Arquitectura

- [ ] `ValoracionCrowdsourcing` es POCO (sin metodos de negocio, solo propiedades)
- [ ] `TipoValoracionId` cambiado de `int` a `int?` en el POCO
- [ ] `IValoracionCrowdsourcingRepository` definida en `Application/Interfaces/Repositories/`
- [ ] `IValoracionCrowdsourcingService` definida en `Application/Interfaces/Services/`
- [ ] `ValoracionCrowdsourcingRepository` inyecta `CrowdsourcingContext` con `?? throw`
- [ ] `ValoracionCrowdsourcingRepository` hace `SaveChangesAsync` en `AddAsync`
- [ ] `ValoracionCrowdsourcingRepository` usa `AsNoTracking` en todas las queries de lectura
- [ ] `GetResumenByUserIdAsync` ejecuta AVG + COUNT + GROUP BY a nivel SQL (no en memoria)
- [ ] `GetByUserIdPagedAsync` usa Skip/Take (no carga todo en memoria)
- [ ] `ValoracionCrowdsourcingService` NO inyecta `CrowdsourcingContext` (solo el repository)
- [ ] `ValoracionCrowdsourcingService` inyecta `IRequestCacheService` para `ExisteValoracionAsync`
- [ ] `ValoracionCrowdsourcingService` inyecta `ILogger<ValoracionCrowdsourcingService>`
- [ ] `ValoracionCrowdsourcingService` usa `?? throw new ArgumentNullException` en constructor para TODAS las dependencias
- [ ] `ValoracionCrowdsourcingService` retorna entidades, NO DTOs
- [ ] `ValoracionCrowdsourcingConfiguration` usa Fluent API (NO data annotations en el POCO)
- [ ] Constraint unico `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor` en la configuration
- [ ] CHECK constraint `CK_ValoracionCrowdsourcing_Puntuacion_Rango` (1-5) en la configuration
- [ ] Indice `IX_ValoracionCrowdsourcing_UserIdValorado` en la configuration
- [ ] Indice `IX_ValoracionCrowdsourcing_UserIdValorado_FechaCreacion` en la configuration
- [ ] Configuracion inline eliminada de `CrowdsourcingContext.OnModelCreating`
- [ ] `modelBuilder.ApplyConfiguration(new ValoracionCrowdsourcingConfiguration())` agregado en `OnModelCreating`
- [ ] `BusinessRule_DuplicateValoracion = "4017"` agregado a `ServiceResponseMessageType.cs`
- [ ] DI registrado en `DependencyInjection.cs` (repo + service)
- [ ] Migracion `AddValoracionesCrowdsourcing` generada con todos los cambios de schema
- [ ] La logica de `UserIdValorado` queda en el Handler, NO en el service/repository
- [ ] NO se agrega navigation property cross-modulo hacia `MaestraTipoValoracion`

---

## 9. Siguiente Paso Sugerido

Una vez validado este plan, el siguiente paso es crear el plan de Application Layer (CQRS) con `cqrs-planning-architect`:

- `CreateValoracionCommand` + `CreateValoracionCommandHandler`
- `GetValoracionesByUserQuery` + `GetValoracionesByUserQueryHandler`
- `CreateValoracionValidator`
- `ValoracionProfile` (AutoMapper)
- `ValoracionesController`

El handler necesitara:
1. `IValoracionCrowdsourcingService` (definido en este plan)
2. `IAcuerdoCrowdsourcingService` (ya existe en el modulo)
3. `IArtistaService` (ya existe en el modulo UserAccess, accesible via DI)
4. `IPerfilProfesionalService` (ya existe en UserAccess, para resolución del nombre del profesional)
