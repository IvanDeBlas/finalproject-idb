# Arquitectura Hexagonal: cs-explorar-propuestas

**Fecha:** 2026-02-17
**Modulo:** Crowdsourcing
**Feature:** cs-explorar-propuestas (US-CS-03)

---

## 1. Resumen Ejecutivo

Esta feature activa el flujo del profesional en el modulo de Crowdsourcing: explorar necesidades abiertas de forma publica, enviar propuestas con precio y condiciones propias, hacer seguimiento del estado de sus propuestas, y retirarlas si aun estan pendientes. Reutiliza entidades ya existentes (`PropuestaCrowdsourcing`, `NecesidadCrowdsourcing`) que estaban parcialmente definidas pero sin los metodos de persistencia ni configuraciones de BD necesarias para esta funcionalidad.

---

## 2. Estado Actual del Codigo (Diagnostico)

Antes de disenar los cambios, esta es la situacion real del codigo:

### Ya existe (NO crear de nuevo)

| Artefacto | Archivo | Estado |
|-----------|---------|--------|
| `PropuestaCrowdsourcing` (entidad) | `Crowdsourcing.Domain/Model/PropuestaCrowdsourcing.cs` | Existe. Falta campo `MotivoRechazo`. |
| `IPropuestaCrowdsourcingRepository` | `Crowdsourcing.Application/Interfaces/Repositories/IPropuestaCrowdsourcingRepository.cs` | Existe. Solo tiene 2 metodos: `GetPendientesByNecesidadIdAsync` y `UpdateManyAsync`. Ampliar. |
| `PropuestaCrowdsourcingRepository` | `Crowdsourcing.Infra/Repositories/PropuestaCrowdsourcingRepository.cs` | Existe. Solo implementa los 2 metodos. Ampliar. |
| `IPropuestaCrowdsourcingService` | `Crowdsourcing.Application/Interfaces/Services/IPropuestaCrowdsourcingService.cs` | Existe. Solo tiene `RechazarPropuestasPendientesAsync`. Ampliar. |
| `PropuestaCrowdsourcingService` | `Crowdsourcing.Infra/Services/PropuestaCrowdsourcingService.cs` | Existe. Solo implementa `RechazarPropuestasPendientesAsync`. Ampliar. |
| `INecesidadCrowdsourcingRepository` | `Crowdsourcing.Application/Interfaces/Repositories/INecesidadCrowdsourcingRepository.cs` | Existe. Ampliar con query publico. |
| `NecesidadCrowdsourcingRepository` | `Crowdsourcing.Infra/Repositories/NecesidadCrowdsourcingRepository.cs` | Existe. Ampliar. |
| `INecesidadCrowdsourcingService` | `Crowdsourcing.Application/Interfaces/Services/INecesidadCrowdsourcingService.cs` | Existe. Ampliar con metodos publicos. |
| `NecesidadCrowdsourcingService` | `Crowdsourcing.Infra/Services/NecesidadCrowdsourcingService.cs` | Existe. Ampliar. |
| `ServiceResponseMessageType` | `Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs` | Existe. Agregar 5 nuevas constantes. |
| `CrowdsourcingContext` | `Crowdsourcing.Infra/Context/CrowdsourcingContext.cs` | Existe. Agregar configuracion de indice unico en PropuestaCrowdsourcing. |
| `PropuestaCrowdsourcingProfile` | `Crowdsourcing.Application/Mapping/PropuestaCrowdsourcingProfile.cs` | Existe. Solo tiene `PropuestaCrowdsourcingDto`. Ampliar. |
| `DependencyInjection` | `Crowdsourcing.Infra/DependencyInjection.cs` | Existe. NO modificar para entidades ya registradas. |

### Debe crearse (NUEVO)

| Artefacto | Archivo | Motivo |
|-----------|---------|--------|
| `IPerfilProfesionalService` | `UserAccess.Application/Interfaces/Services/IPerfilProfesionalService.cs` | El modulo Crowdsourcing necesita consultar si un usuario tiene PerfilProfesional. No existe ninguna interfaz de servicio para PerfilProfesional. |
| `IPerfilProfesionalRepository` | `UserAccess.Domain/Interfaces/IPerfilProfesionalRepository.cs` | No existe repositorio para PerfilProfesional. |
| `PerfilProfesionalRepository` | `UserAccess.Infra/Repositories/PerfilProfesionalRepository.cs` | Implementacion del repositorio. |
| `PerfilProfesionalService` | `UserAccess.Infra/Services/PerfilProfesionalService.cs` | Implementacion del servicio. |
| Migracion EF Core | Nueva migracion en `Crowdsourcing.Infra/Migrations/` | Agregar: campo `MotivoRechazo` a `PropuestaCrowdsourcing`, indice unico `(NecesidadId, UserId)` en `PropuestaCrowdsourcing`, seed de `MaestraEstadoPropuesta`. |

### No existe como tabla en BD

El campo `MotivoRechazo` en `PropuestaCrowdsourcing` no existe en la migracion actual (no esta en el snapshot). Tampoco existe el indice de unicidad `(NecesidadId, UserId)`. Se necesita migracion nueva.

La tabla `MaestraEstadoPropuesta` no esta mapeada en `CrowdsourcingContext`. Los estados se usan como enteros hardcodeados. Se propone agregar seed data via `HasData` en la migracion.

---

## 3. Domain Layer

### 3.1 Entidades

#### PropuestaCrowdsourcing (MODIFICAR - agregar campo MotivoRechazo)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/PropuestaCrowdsourcing.cs`

**Situacion actual:** Entidad existe, le falta el campo `MotivoRechazo`.

| Propiedad | Tipo | Nullable | Cambio | Descripcion |
|-----------|------|----------|--------|-------------|
| Id | `PropuestaCrowdsourcingId` | No | Existe | PK (StronglyTypedId) |
| NecesidadId | `NecesidadCrowdsourcingId` | No | Existe | FK a NecesidadCrowdsourcing |
| UserId | `string` | No | Existe | FK a Identity.User (profesional) |
| PerfilProfesionalId | `PerfilProfesionalId?` | Si | Existe | FK a PerfilProfesional (nullable en entidad, requerido en logica de negocio) |
| PrecioPropuesto | `decimal` | No | Existe | Precio propuesto por el profesional |
| MonedaId | `int?` | Si | Existe | FK a MaestraMoneda |
| DiasEstimados | `int?` | Si | Existe | Dias estimados de entrega |
| MensajePropuesta | `string?` | Si | Existe | Mensaje de propuesta (20-2000 chars en logica) |
| EstadoPropuestaId | `int` | No | Existe | FK a MaestraEstadoPropuesta (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) |
| **MotivoRechazo** | `string?` | Si | **AGREGAR** | Motivo de rechazo o cierre automatico |
| FechaCreacion | `DateTime` | No | Existe | Timestamp de creacion |
| FechaActualizacion | `DateTime?` | Si | Existe | Timestamp de ultima actualizacion de estado |
| Necesidad | `NecesidadCrowdsourcing` | No | Existe | Navigation property |
| Acuerdos | `ICollection<AcuerdoCrowdsourcing>` | No | Existe | Navigation property (1:N) |

**Cambio requerido en el archivo:** Agregar la propiedad `MotivoRechazo`:
```
public string? MotivoRechazo { get; set; }
```

#### NecesidadCrowdsourcing (SIN CAMBIOS en entidad)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/NecesidadCrowdsourcing.cs`

La entidad ya tiene todos los campos necesarios para los queries publicos. No requiere modificacion de la clase POCO.

Campos relevantes para esta feature:

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | `NecesidadCrowdsourcingId` | PK |
| ArtistaId | `ArtistaId` | FK al artista (usado para calcular `esPropietario`) |
| Titulo | `string` | Incluido en listado publico |
| Descripcion | `string?` | Truncado a 150 chars en proyeccion |
| TipoNecesidadId | `int` | FK a MaestraTipoNecesidad |
| EstadoNecesidadId | `int` | Filtro: solo `1 = Abierta` en listado publico |
| ModalidadTrabajoId | `int` | Filtro y display |
| PresupuestoMin | `decimal?` | Display y filtro |
| PresupuestoMax | `decimal?` | Display y filtro |
| MonedaId | `int?` | Display |
| UbicacionCiudad | `string?` | Display (solo si Presencial o Hibrido) |
| UbicacionPais | `string?` | Display y filtro |
| FechaLimitePropuestas | `DateTime?` | Filtro de expiradas, calculo de urgencia |
| FechaInicioPrevista | `DateTime?` | Display en detalle |
| FechaCreacion | `DateTime` | Display y ordenamiento |
| Propuestas | `ICollection<PropuestaCrowdsourcing>` | COUNT para `numeroPropuestas` |

---

### 3.2 Repository Interfaces (Ports)

#### IPropuestaCrowdsourcingRepository (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IPropuestaCrowdsourcingRepository.cs`

**Situacion actual:** Solo tiene `GetPendientesByNecesidadIdAsync` y `UpdateManyAsync`.

Metodos existentes (NO modificar firma):

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetPendientesByNecesidadIdAsync(NecesidadCrowdsourcingId, CancellationToken)` | `IReadOnlyList<PropuestaCrowdsourcing>` | Ya existe - NO tocar |
| `UpdateManyAsync(IEnumerable<PropuestaCrowdsourcing>, CancellationToken)` | `Task` | Ya existe - NO tocar |

Metodos a AGREGAR:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct)` | `PropuestaCrowdsourcing?` | Obtener propuesta por PK. Usado en RetirarPropuesta. |
| `AddAsync(PropuestaCrowdsourcing entity, CancellationToken ct)` | `PropuestaCrowdsourcingId` | Insertar nueva propuesta y retornar su ID. |
| `UpdateAsync(PropuestaCrowdsourcing entity, CancellationToken ct)` | `Task` | Actualizar una propuesta (cambio de estado al retirar). |
| `ExisteAsync(NecesidadCrowdsourcingId necesidadId, string userId, CancellationToken ct)` | `bool` | Verificar si ya existe propuesta no-Retirada para (NecesidadId, UserId). Usado en validator de unicidad. |
| `GetByUserIdPaginatedAsync(string userId, int? estadoPropuestaId, int page, int pageSize, CancellationToken ct)` | `(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)` | Listado paginado de propuestas del usuario autenticado (Mis Propuestas). Include Necesidad y Acuerdos. |

#### INecesidadCrowdsourcingRepository (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/INecesidadCrowdsourcingRepository.cs`

**Situacion actual:** Tiene metodos para el artista (GetByArtistaIdPaginatedAsync). Agregar queries publicos.

Metodos a AGREGAR:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetPublicasPaginatedAsync(NecesidadesPublicasFiltro filtro, ArtistaId? artistaDelUsuario, CancellationToken ct)` | `(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)` | Listado publico: solo `EstadoNecesidadId == 1` y no expiradas. Excluye necesidades del propio artista. Soporta filtros, ordenamiento y paginacion. Usa `AsNoTracking()`. |
| `GetPublicaByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)` | `NecesidadCrowdsourcing?` | Detalle publico de necesidad. Incluye navigation `Artista` (via ArtistaId, consulta separada) para obtener nombre e imagen. Solo retorna si `EstadoNecesidadId == 1` y no expirada. |

**Nota sobre `NecesidadesPublicasFiltro`:** Es un record/clase de parametros que encapsula los filtros del listado publico. Se define en la capa Application (no en Domain) como parte del contrato del repositorio.

```
record NecesidadesPublicasFiltro(
    int[]? TiposNecesidadIds,
    int? ModalidadTrabajoId,
    decimal? PresupuestoMin,
    decimal? PresupuestoMax,
    string? Pais,
    string? Search,
    string OrderBy,    // "recientes" | "mayor-presupuesto" | "fecha-limite"
    int Page,
    int PageSize
)
```

**Ubicacion:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/NecesidadesPublicasFiltro.cs`

#### IPerfilProfesionalRepository (CREAR NUEVO en UserAccess)

**Archivo:** `src/api/Modules/UserAccess/WePlayRises.UserAccess.Domain/Interfaces/IPerfilProfesionalRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByUserIdAsync(string userId, CancellationToken ct)` | `PerfilProfesional?` | Obtener perfil profesional por UserId de Identity. |
| `ExistsByUserIdAsync(string userId, CancellationToken ct)` | `bool` | Verificar si el usuario tiene PerfilProfesional. Usado en validators y queries. |

---

### 3.3 Service Interfaces

#### IPropuestaCrowdsourcingService (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IPropuestaCrowdsourcingService.cs`

**Situacion actual:** Solo tiene `RechazarPropuestasPendientesAsync`.

Metodo existente (NO modificar):

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `RechazarPropuestasPendientesAsync(NecesidadCrowdsourcingId, CancellationToken)` | `Task<int>` | Ya existe - NO tocar |

Metodos a AGREGAR:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync(PropuestaCrowdsourcingId id, CancellationToken ct)` | `PropuestaCrowdsourcing?` | Obtener propuesta por ID con cache request-scoped. |
| `ExistePropuestaAsync(NecesidadCrowdsourcingId necesidadId, string userId, CancellationToken ct)` | `bool` | Verificar unicidad. Cache request-scoped. Usado en validator y en handler. |
| `CreateAsync(PropuestaCrowdsourcing entity, CancellationToken ct)` | `PropuestaCrowdsourcingId` | Crear propuesta (estado Pendiente). Hace SaveChanges. |
| `RetirarAsync(PropuestaCrowdsourcingId id, CancellationToken ct)` | `bool` | Cambiar estado a Retirada (4) y actualizar FechaActualizacion. |
| `GetByUserIdPaginatedAsync(string userId, int? estadoPropuestaId, int page, int pageSize, CancellationToken ct)` | `(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)` | Listado paginado "Mis Propuestas". |

#### INecesidadCrowdsourcingService (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/INecesidadCrowdsourcingService.cs`

Metodos a AGREGAR (conservar todos los existentes):

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetPublicasPaginatedAsync(NecesidadesPublicasFiltro filtro, ArtistaId? artistaDelUsuario, CancellationToken ct)` | `(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)` | Delega al repositorio. Sin cache (es un listado con filtros variables). |
| `GetPublicaByIdAsync(NecesidadCrowdsourcingId id, CancellationToken ct)` | `NecesidadCrowdsourcing?` | Detalle publico. Cache request-scoped con clave `necesidad:publica:{id}`. |

#### IPerfilProfesionalService (CREAR NUEVO en UserAccess)

**Archivo:** `src/api/Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IPerfilProfesionalService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByUserIdAsync(string userId, CancellationToken ct)` | `PerfilProfesional?` | Obtener PerfilProfesional por UserId. Cache request-scoped. Usado en handler de CreatePropuesta para obtener PerfilProfesionalId. |
| `ExistsByUserIdAsync(string userId, CancellationToken ct)` | `bool` | Verificar si tiene perfil. Cache request-scoped. Usado en validators. |

---

### 3.4 Constants (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**Situacion actual:** El archivo existe con constantes hasta `4002`.

Constantes a AGREGAR (agregar al final de cada seccion):

```csharp
// NotFound (2000-2999)
// AGREGAR:
public const string NotFound_Propuesta = "2010";

// Business Rules (4000-4999)
// AGREGAR:
public const string BusinessRule_AlreadyProposed = "4003";       // Ya tienes una propuesta para esta necesidad
public const string BusinessRule_CannotProposeSelf = "4004";     // No puedes proponer a tu propia necesidad
public const string BusinessRule_PropuestaNotRetirable = "4005"; // Solo se pueden retirar propuestas en estado Pendiente
public const string BusinessRule_NoProfessionalProfile = "4006"; // Debes crear un perfil profesional
```

---

## 4. Infrastructure Layer

### 4.1 Repository Implementations

#### PropuestaCrowdsourcingRepository (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/PropuestaCrowdsourcingRepository.cs`

**Situacion actual:** Solo implementa `GetPendientesByNecesidadIdAsync` y `UpdateManyAsync`. Inyecta `CrowdsourcingContext`.

Metodos existentes: NO modificar.

Metodos a IMPLEMENTAR (nuevos, usando EF Core):

| Metodo | Patron EF | Notas |
|--------|-----------|-------|
| `GetByIdAsync` | `_context.Propuestas.FirstOrDefaultAsync(p => p.Id == id, ct)` | Sin AsNoTracking (puede ser modificada luego) |
| `AddAsync` | `_context.Propuestas.AddAsync(entity, ct)` + `SaveChangesAsync` | Retorna `entity.Id` |
| `UpdateAsync` | `_context.Propuestas.Update(entity)` + `SaveChangesAsync` | Para cambio de estado al retirar |
| `ExisteAsync` | `_context.Propuestas.AnyAsync(p => p.NecesidadId == necesidadId && p.UserId == userId && p.EstadoPropuestaId != 4, ct)` | Estado 4 = Retirada; `EstadoPropuestaId != 4` excluye retiradas |
| `GetByUserIdPaginatedAsync` | `_context.Propuestas.AsNoTracking().Include(p => p.Necesidad).Include(p => p.Acuerdos).Where(p => p.UserId == userId)...` | Include Necesidad (para titulo), Include Acuerdos (para acuerdoId). Filtro opcional por estado. Skip/Take para paginacion. OrderBy FechaCreacion DESC. |

#### NecesidadCrowdsourcingRepository (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/NecesidadCrowdsourcingRepository.cs`

**Situacion actual:** Implementa metodos para el artista. Inyecta `CrowdsourcingContext`.

Metodos a IMPLEMENTAR (nuevos):

| Metodo | Patron EF | Notas |
|--------|-----------|-------|
| `GetPublicasPaginatedAsync` | Query con multiples filtros sobre `_context.Necesidades.AsNoTracking()` | Filtros: `EstadoNecesidadId == 1`, `FechaLimitePropuestas IS NULL OR FechaLimitePropuestas > DateTime.UtcNow`, excluir si `ArtistaId == artistaDelUsuario` (si el usuario es artista). Soportar ordenamiento (recientes/presupuesto/limite). Calcular `EsUrgente` fuera del query (en la proyeccion del handler). |
| `GetPublicaByIdAsync` | `_context.Necesidades.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && n.EstadoNecesidadId == 1 && ..., ct)` | Filtrar estado Abierta y no expirada. NO incluye artista ni propuestas (se resuelven por separado para evitar cross-context). |

**Nota critica sobre cross-module:** `NecesidadCrowdsourcing.ArtistaId` es un `ArtistaId` (StronglyTypedId de UserAccess). La tabla `Artista` esta en el contexto `UserAccessContext`, no en `CrowdsourcingContext`. Los datos del artista (`NombreArtistico`, `ImagenPerfilUrl`) se obtienen en el Handler llamando a `IArtistaService` del modulo UserAccess, NO mediante Join en el repositorio de Crowdsourcing.

#### PerfilProfesionalRepository (CREAR NUEVO en UserAccess)

**Archivo:** `src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra/Repositories/PerfilProfesionalRepository.cs`

- Implementa: `IPerfilProfesionalRepository`
- Inyecta: `UserAccessContext`
- Constructor: `?? throw new ArgumentNullException`

| Metodo | Patron EF | Notas |
|--------|-----------|-------|
| `GetByUserIdAsync` | `_context.PerfilesProfesionales.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, ct)` | Sin tracking (solo lectura) |
| `ExistsByUserIdAsync` | `_context.PerfilesProfesionales.AnyAsync(p => p.UserId == userId, ct)` | Mas eficiente que GetByUserId cuando solo se necesita existencia |

---

### 4.2 Services (para persistencia)

#### PropuestaCrowdsourcingService (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/PropuestaCrowdsourcingService.cs`

**Situacion actual:** Solo tiene `RechazarPropuestasPendientesAsync`. Inyecta `IPropuestaCrowdsourcingRepository` y `ILogger`.

**Cambio en constructor:** Agregar `IRequestCacheService` como dependencia (con `?? throw`).

```
Dependencias del constructor actualizadas:
- IPropuestaCrowdsourcingRepository _repository   (ya existe)
- IRequestCacheService _requestCache              (AGREGAR)
- ILogger<PropuestaCrowdsourcingService> _logger  (ya existe)
```

Metodo existente (NO modificar): `RechazarPropuestasPendientesAsync`.

Metodos a IMPLEMENTAR (nuevos):

| Metodo | Patron | Cache |
|--------|--------|-------|
| `GetByIdAsync` | Delega a `_repository.GetByIdAsync` | `_requestCache.GetOrAddAsync($"propuesta:{id.Value}", ...)` |
| `ExistePropuestaAsync` | Delega a `_repository.ExisteAsync` | `_requestCache.GetOrAddAsync($"propuesta:existe:{necesidadId.Value}:{userId}", ...)` |
| `CreateAsync` | Llama `_repository.AddAsync`. Loguea creacion. | Sin cache (escritura) |
| `RetirarAsync` | Obtiene entidad, cambia estado a 4, actualiza `FechaActualizacion = DateTime.UtcNow`, llama `_repository.UpdateAsync`. Retorna true. | Sin cache (escritura) |
| `GetByUserIdPaginatedAsync` | Delega directamente a `_repository.GetByUserIdPaginatedAsync` | Sin cache (listado variable) |

**IMPORTANTE:** `RetirarAsync` en el Service NO valida ownership ni estado. Esas validaciones van en el Validator y el Handler (capa Application). El Service solo ejecuta la actualizacion.

#### NecesidadCrowdsourcingService (AMPLIAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/NecesidadCrowdsourcingService.cs`

**Situacion actual:** Inyecta `INecesidadCrowdsourcingRepository`, `IPropuestaCrowdsourcingService`, `IRequestCacheService`, `CrowdsourcingContext` y `ILogger`. Todos los metodos existentes se conservan.

Metodos a IMPLEMENTAR (nuevos):

| Metodo | Patron | Cache |
|--------|--------|-------|
| `GetPublicasPaginatedAsync` | Delega a `_repository.GetPublicasPaginatedAsync` | Sin cache (query con filtros variables por request) |
| `GetPublicaByIdAsync` | Delega a `_repository.GetPublicaByIdAsync` con cache | `_requestCache.GetOrAddAsync($"necesidad:publica:{id.Value}", ...)` |

**Nota:** `GetPublicaByIdAsync` usa una clave de cache diferente a `GetByIdAsync` existente (`necesidad:{id.Value}`) para evitar colisiones, ya que el query publico incluye filtro de estado y expiracion mientras que el `GetByIdAsync` existente no.

#### PerfilProfesionalService (CREAR NUEVO en UserAccess)

**Archivo:** `src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/PerfilProfesionalService.cs`

- Implementa: `IPerfilProfesionalService`
- Inyecta:
  - `IPerfilProfesionalRepository _repository` (`?? throw`)
  - `IRequestCacheService _requestCache` (`?? throw`)
  - `ILogger<PerfilProfesionalService> _logger` (`?? throw`)

| Metodo | Patron | Cache |
|--------|--------|-------|
| `GetByUserIdAsync` | Delega a `_repository.GetByUserIdAsync` | `_requestCache.GetOrAddAsync($"perfilprofesional:user:{userId}", ...)` |
| `ExistsByUserIdAsync` | Delega a `_repository.ExistsByUserIdAsync` | `_requestCache.GetOrAddAsync($"perfilprofesional:exists:user:{userId}", ...)` |

---

### 4.3 Configuraciones EF Core

#### CrowdsourcingContext - PropuestaCrowdsourcing (MODIFICAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

**Situacion actual:** La entidad `PropuestaCrowdsourcing` esta configurada en `OnModelCreating` pero le faltan:
1. La propiedad `MotivoRechazo`
2. El indice unico `(NecesidadId, UserId)`

Cambios a aplicar dentro del bloque de configuracion de `PropuestaCrowdsourcing` en `OnModelCreating`:

```csharp
// AGREGAR estas lineas dentro del bloque entity => { ... } de PropuestaCrowdsourcing:

entity.Property(e => e.MotivoRechazo).HasMaxLength(500);

entity.HasIndex(e => new { e.NecesidadId, e.UserId })
    .IsUnique()
    .HasDatabaseName("UQ_PropuestaCrowdsourcing_Necesidad_User");
```

**Nota sobre el indice unico:** El indice de unicidad en `(NecesidadId, UserId)` es la ultima linea de defensa contra propuestas duplicadas. Se aplica a nivel de BD. La validacion en el Validator (capa Application) es la primera linea de defensa para dar un mensaje de error amigable.

#### CrowdsourcingContext - Seed MaestraEstadoPropuesta

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

**Situacion actual:** Los estados de propuesta se usan como enteros directos (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) sin tabla maestra mapeada en EF. La feature-spec indica que deben estar en BD.

**Decision de diseno:** NO agregar una nueva tabla `MaestraEstadoPropuesta` como entidad EF con DbSet. Dado que son valores fijos de referencia (catalogo), se documentan como constantes en codigo y se pueden agregar como comentario en la migracion. Si en el futuro se requiere una tabla maestra real, se puede agregar como una entidad separada. Para el MVP, los enteros hardcodeados con constantes de codigo son suficientes.

**Constantes de estados (en Domain, NO en BD):**

Agregar en `ServiceResponseMessageType.cs` o en un archivo separado `EstadoPropuestaConstants.cs`:

```
Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/EstadoPropuestaConstants.cs
```

```csharp
// Estados de MaestraEstadoPropuesta
public static class EstadoPropuestaConstants
{
    public const int Pendiente = 1;
    public const int Aceptada = 2;
    public const int Rechazada = 3;
    public const int Retirada = 4;
}
```

Esto evita los "magic numbers" en el codigo (actualmente existen en `PropuestaCrowdsourcingService` y `PropuestaCrowdsourcingRepository`).

---

### 4.4 Migracion EF Core

**Nombre sugerido:** `AddPropuestaMotivoRechazoAndUniqueIndex`

**Proyecto:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra`

```bash
dotnet ef migrations add AddPropuestaMotivoRechazoAndUniqueIndex \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi
```

**Cambios que genera la migracion:**

| Tabla | Operacion | Detalle |
|-------|-----------|---------|
| `PropuestaCrowdsourcing` | `AddColumn` | `MotivoRechazo nvarchar(500) NULL` |
| `PropuestaCrowdsourcing` | `CreateIndex` | `UQ_PropuestaCrowdsourcing_Necesidad_User` UNIQUE en `(Necesidad_Id, UserId)` |

**Aplicar migracion:**

```bash
dotnet ef database update \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi
```

---

### 4.5 Dependency Injection

#### UserAccess.Infra/DependencyInjection.cs (MODIFICAR)

**Archivo:** `src/api/Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`

Agregar los nuevos registros:

```csharp
// AGREGAR en el metodo AddUserAccessServices:
services.AddScoped<IPerfilProfesionalRepository, PerfilProfesionalRepository>();
services.AddScoped<IPerfilProfesionalService, PerfilProfesionalService>();
```

#### Crowdsourcing.Infra/DependencyInjection.cs (SIN CAMBIOS)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

Los repositorios y servicios de Crowdsourcing ya estan registrados:
- `IPropuestaCrowdsourcingRepository` -> `PropuestaCrowdsourcingRepository` (ya registrado)
- `IPropuestaCrowdsourcingService` -> `PropuestaCrowdsourcingService` (ya registrado)
- `INecesidadCrowdsourcingRepository` -> `NecesidadCrowdsourcingRepository` (ya registrado)
- `INecesidadCrowdsourcingService` -> `NecesidadCrowdsourcingService` (ya registrado)

No se necesitan registros nuevos en este archivo para esta feature.

---

## 5. Dependencias Cross-Module

Esta feature introduce una dependencia de **Crowdsourcing** hacia **UserAccess**:

```
Crowdsourcing.Application (Handlers, Validators)
  -> IPerfilProfesionalService  (definida en UserAccess.Application)
  -> IArtistaService            (definida en UserAccess.Application, ya usada en otros handlers)
```

**Patron ya establecido:** El modulo Crowdsourcing ya referencia `ArtistaId`, `PerfilProfesionalId` de `WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds`. La inyeccion de `IArtistaService` en handlers de Crowdsourcing ya es un patron existente (ver `CreateNecesidadCommand` que recibe `ArtistaId`).

**Para los campos calculados** (`yaPropuso`, `esPropietario`, `tienePerfilProfesional`) en `GetNecesidadPublicaByIdQuery`:

- `yaPropuso`: `IPropuestaCrowdsourcingService.ExistePropuestaAsync(necesidadId, userId)`
- `esPropietario`: comparar `necesidad.ArtistaId` con `IArtistaService.GetByUserIdAsync(userId)?.Id`
- `tienePerfilProfesional`: `IPerfilProfesionalService.ExistsByUserIdAsync(userId)`

Los tres se calculan en el Handler usando los services con cache. El cache request-scoped garantiza que si el Validator ya llamo al mismo service, el Handler no hace un segundo query a BD.

---

## 6. Archivos a Crear / Modificar

### Crear (NUEVOS)

```
src/api/Modules/UserAccess/
├── WePlayRises.UserAccess.Domain/
│   └── Interfaces/
│       └── IPerfilProfesionalRepository.cs          [NUEVO]
│
├── WePlayRises.UserAccess.Application/
│   └── Interfaces/
│       └── Services/
│           └── IPerfilProfesionalService.cs         [NUEVO]
│
└── WePlayRises.UserAccess.Infra/
    ├── Repositories/
    │   └── PerfilProfesionalRepository.cs           [NUEVO]
    └── Services/
        └── PerfilProfesionalService.cs              [NUEVO]

src/api/Modules/Crowdsourcing/
└── WePlayRises.Crowdsourcing.Application/
    └── Interfaces/
        └── Repositories/
            └── NecesidadesPublicasFiltro.cs         [NUEVO]

src/api/Modules/Crowdsourcing/
└── WePlayRises.Crowdsourcing.Domain/
    └── Constants/
        └── EstadoPropuestaConstants.cs              [NUEVO]
```

### Modificar (EXISTENTES)

```
src/api/Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   ├── Model/
│   │   └── PropuestaCrowdsourcing.cs                [MODIFICAR - agregar MotivoRechazo]
│   └── Constants/
│       └── ServiceResponseMessageType.cs            [MODIFICAR - agregar 5 constantes]
│
├── WePlayRises.Crowdsourcing.Application/
│   └── Interfaces/
│       ├── Repositories/
│       │   ├── IPropuestaCrowdsourcingRepository.cs [MODIFICAR - agregar 5 metodos]
│       │   └── INecesidadCrowdsourcingRepository.cs [MODIFICAR - agregar 2 metodos]
│       └── Services/
│           ├── IPropuestaCrowdsourcingService.cs    [MODIFICAR - agregar 5 metodos]
│           └── INecesidadCrowdsourcingService.cs    [MODIFICAR - agregar 2 metodos]
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Context/
    │   └── CrowdsourcingContext.cs                  [MODIFICAR - MotivoRechazo + indice unico]
    ├── Repositories/
    │   ├── PropuestaCrowdsourcingRepository.cs      [MODIFICAR - agregar 5 metodos]
    │   └── NecesidadCrowdsourcingRepository.cs      [MODIFICAR - agregar 2 metodos]
    ├── Services/
    │   ├── PropuestaCrowdsourcingService.cs         [MODIFICAR - agregar IRequestCacheService + 5 metodos]
    │   └── NecesidadCrowdsourcingService.cs         [MODIFICAR - agregar 2 metodos]
    └── Migrations/
        └── {timestamp}_AddPropuestaMotivoRechazoAndUniqueIndex.cs  [NUEVO - autogenerado]

src/api/Modules/UserAccess/
└── WePlayRises.UserAccess.Infra/
    └── DependencyInjection.cs                       [MODIFICAR - registrar PerfilProfesional repo/service]
```

---

## 7. Notas de Implementacion Criticas

### 7.1 StronglyTypedIds

El proyecto usa `StronglyTypedId` wrapping pattern. Las claves foraneas entre modulos (ej. `PropuestaCrowdsourcingId`, `NecesidadCrowdsourcingId`, `ArtistaId`, `PerfilProfesionalId`) son wrappers sobre `Guid`, no `Guid` directos. Al implementar los metodos, usar los tipos correctos:

- `PropuestaCrowdsourcingId id` (no `Guid id`)
- Acceder al valor con `.Value` cuando se necesita el `Guid` raw

### 7.2 SaveChanges en Repository vs Service

El patron del proyecto es inconsistente (algunos repositorios hacen SaveChanges, el servicio de NecesidadCrowdsourcing usa transacciones directas con `_context`). Para esta feature, mantener el patron existente en Crowdsourcing:

- **`PropuestaCrowdsourcingRepository.AddAsync`**: hace `SaveChangesAsync` (igual que `NecesidadCrowdsourcingRepository.AddAsync`)
- **`PropuestaCrowdsourcingRepository.UpdateAsync`**: hace `SaveChangesAsync` (igual que `NecesidadCrowdsourcingRepository.UpdateAsync`)
- **`PropuestaCrowdsourcingService.CreateAsync` y `RetirarAsync`**: delegan al repositorio (que ya hace SaveChanges). No necesitan IUnitOfWork separado.

### 7.3 Cache Request-Scoped

El patron ya esta establecido en `NecesidadCrowdsourcingService` y `ArtistaService`. Las claves de cache deben ser consistentes:

| Cache Key | Metodo | Proposito |
|-----------|--------|-----------|
| `necesidad:{id.Value}` | `GetByIdAsync` (existente) | Para artista propietario |
| `necesidad:publica:{id.Value}` | `GetPublicaByIdAsync` (nuevo) | Para profesionales (incluye filtro estado+expiracion) |
| `propuesta:{id.Value}` | `GetByIdAsync` (nuevo) | Para RetirarPropuesta |
| `propuesta:existe:{necesidadId.Value}:{userId}` | `ExistePropuestaAsync` (nuevo) | Para validator de unicidad |
| `perfilprofesional:user:{userId}` | `GetByUserIdAsync` (nuevo) | Para obtener PerfilProfesionalId en handler |
| `perfilprofesional:exists:user:{userId}` | `ExistsByUserIdAsync` (nuevo) | Para validator y GetNecesidadPublicaByIdQuery |

### 7.4 Query de Campos Calculados

En `GetNecesidadPublicaByIdQuery` (Handler en Application, NO aqui), los tres campos calculados se resuelven asi:

1. Cargar necesidad: `INecesidadCrowdsourcingService.GetPublicaByIdAsync(id)` -> resultado en cache
2. Cargar artista: `IArtistaService.GetByUserIdAsync(userId)` -> para comparar `ArtistaId`
3. `esPropietario = necesidad.ArtistaId == artista?.Id`
4. `yaPropuso = await IPropuestaCrowdsourcingService.ExistePropuestaAsync(id, userId)` -> en cache
5. `tienePerfilProfesional = await IPerfilProfesionalService.ExistsByUserIdAsync(userId)` -> en cache
6. Para datos del artista en el DTO (nombre, imagen): `IArtistaService.GetByIdAsync(necesidad.ArtistaId)` -> el `IArtistaService` ya tiene este metodo

**Esta logica pertenece al Handler (Application layer), no al Service ni al Repository.**

### 7.5 Artista de la Necesidad en el DTO Publico

El `NecesidadPublicaDto` requiere `artista: { id, nombreArtistico, imagenUrl }`. Dado que `ArtistaId` en `NecesidadCrowdsourcing` apunta a la tabla `Artista` en `UserAccessContext`, el handler debe:

1. Llamar a `IArtistaService.GetByIdAsync(necesidad.ArtistaId)` (ya inyectable)
2. Mapear a `ArtistaPublicoDto` en el mismo handler

Para el listado publico (`GetNecesidadesPublicasQuery`), solo se necesita `artistaNombre`. Opciones:
- Cargar todos los ArtistaIds del resultado y resolver nombres en batch via `IArtistaService`
- O incluir `NombreArtistico` como denormalizacion en la proyeccion del query de Crowdsourcing (requiere raw SQL o subquery)

**Recomendacion para MVP:** Resolver por batch con `IArtistaService.GetByIdAsync` para cada item del listado. Con paginacion de 12 items y cache request-scoped, son como maximo 12 queries a BD por request (o menos si hay artistas repetidos). Aceptable para MVP.

---

## 8. Checklist de Verificacion

- [ ] `PropuestaCrowdsourcing.cs` tiene campo `MotivoRechazo` agregado
- [ ] `ServiceResponseMessageType.cs` tiene las 5 nuevas constantes (2010, 4003, 4004, 4005, 4006)
- [ ] `EstadoPropuestaConstants.cs` creado con los 4 valores
- [ ] `IPropuestaCrowdsourcingRepository` tiene los 5 nuevos metodos
- [ ] `IPropuestaCrowdsourcingService` tiene los 5 nuevos metodos
- [ ] `INecesidadCrowdsourcingRepository` tiene los 2 nuevos metodos
- [ ] `INecesidadCrowdsourcingService` tiene los 2 nuevos metodos
- [ ] `NecesidadesPublicasFiltro` record definido en Application
- [ ] `IPerfilProfesionalRepository` creado en UserAccess.Domain
- [ ] `IPerfilProfesionalService` creado en UserAccess.Application
- [ ] `PerfilProfesionalRepository` implementado con `AsNoTracking`
- [ ] `PerfilProfesionalService` implementado con `IRequestCacheService`
- [ ] `PropuestaCrowdsourcingService` actualizado con `IRequestCacheService` en constructor
- [ ] `CrowdsourcingContext` tiene configuracion de `MotivoRechazo` y el indice unico
- [ ] `UserAccess.Infra/DependencyInjection.cs` registra el nuevo repo y servicio
- [ ] **Migracion EF Core generada y validada** con los 2 cambios (columna + indice)
- [ ] Entidades son POCOs (sin metodos de negocio)
- [ ] Services retornan entidades, NO DTOs
- [ ] Constructores con `?? throw new ArgumentNullException` en TODAS las dependencias
- [ ] Cache request-scoped aplicado en todos los metodos de lectura de services
