# Arquitectura Hexagonal: cp-inscripcion-programa

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-inscripcion-programa (US-CP-03)

---

## 1. Resumen Ejecutivo

Esta feature implementa el ciclo completo de inscripcion de promotores en programas de crowdpromotion. El flujo cubre desde la solicitud del promotor (creacion del registro `PromoProgramaPromotor` en estado pendiente) hasta la gestion del artista (aprobar, rechazar, bloquear, dar de baja). La aprobacion genera automaticamente un `CodigoReferido` unico con patron `{CodigoTrackingBase}-{ShortId}` y una `UrlTrackingPersonalizada` con parametros UTM. Requiere agregar dos campos nuevos (`EsAprobado`, `EsBloqueado`) a la entidad existente `PromoProgramaPromotor` y crear una nueva interfaz de repositorio dedicada a las inscripciones.

---

## 2. Cambios al Modelo de Dominio (Entidad Existente)

### 2.1 Modificacion: PromoProgramaPromotor

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoProgramaPromotor.cs`

La entidad existente debe recibir dos campos nuevos. No se modifica ninguna propiedad existente.

**Campos a AGREGAR:**

| Propiedad | Tipo | Nullable | Default | Descripcion |
|-----------|------|----------|---------|-------------|
| `EsAprobado` | `bool` | No | `false` | El artista ha aprobado la inscripcion. Al aprobar: true + CodigoReferido generado |
| `EsBloqueado` | `bool` | No | `false` | El artista ha bloqueado al promotor en este programa. Impide re-solicitar inscripcion |

**Campos existentes relevantes (sin cambio):**

| Propiedad | Tipo | Uso en esta feature |
|-----------|------|---------------------|
| `Id` | `Guid` | PK de la inscripcion |
| `ProgramaId` | `PromoProgramaId` | FK al programa |
| `PromotorId` | `PromotorId` | FK al promotor |
| `CodigoReferido` | `string?` | Se genera al aprobar con patron {CodigoTrackingBase}-{ShortId} |
| `UrlReferido` | `string?` | Almacena la UrlTrackingPersonalizada generada al aprobar. Se expone en DTOs como `UrlTrackingPersonalizada` |
| `EsActivo` | `bool` | Campo pre-existente (no usar para la maquina de estados de inscripcion; usar EsAprobado/EsBloqueado/FechaBaja) |
| `FechaInscripcion` | `DateTime` | Fecha de alta de la inscripcion. Se expone en DTOs como `FechaAlta` |
| `FechaBaja` | `DateTime?` | Se establece al dar de baja (junto con EsAprobado=false) |

**Maquina de estados derivada de los campos:**

| EsAprobado | EsBloqueado | FechaBaja | Estado visible | Descripcion |
|------------|-------------|-----------|----------------|-------------|
| `false` | `false` | `null` | `Pendiente` | Solicitud enviada, esperando decision del artista |
| `true` | `false` | `null` | `Aprobado` | Promotor activo con CodigoReferido funcional |
| `false` | `true` | `null` | `Bloqueado` | Bloqueado por el artista, no puede re-solicitar |
| `false` | `false` | `DateTime` | `DadoDeBaja` | Baja historica, CodigoReferido desactivado |
| (eliminado) | - | - | `Rechazado` | DELETE fisico; el promotor puede re-solicitar |

**Logica de calculo del estado (prioridad en orden):**

```
1. Si EsBloqueado == true  -> "Bloqueado"
2. Si FechaBaja != null    -> "DadoDeBaja"
3. Si EsAprobado == true   -> "Aprobado"
4. Ninguna de las anteriores -> "Pendiente"
```

---

## 3. Domain Layer

### 3.1 Constantes: ServiceResponseMessageType

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Accion:** MODIFICAR el archivo existente. Agregar las constantes que faltan para esta feature.

**Constantes a AGREGAR al archivo existente:**

```
// NotFound (2000-2999) - AGREGAR:
NotFound_Inscripcion = "2020"       // La inscripcion no existe (PromoProgramaPromotor no encontrado)

// Business Rules (4000-4999) - AGREGAR:
BusinessRule_InscripcionDuplicada = "4021"         // Ya existe inscripcion del promotor en este programa
BusinessRule_PromotorBloqueadoEnPrograma = "4022"  // Promotor bloqueado no puede re-solicitar
BusinessRule_PromotorDesactivado = "4023"          // Perfil de promotor desactivado (EsActivo=false)
BusinessRule_ProgramaNoActivo = "4024"             // Programa no esta activo (EsActivo=false)
BusinessRule_InscripcionNoEnEstadoEsperado = "4025" // Inscripcion no esta en el estado requerido para la accion
BusinessRule_ArtistaNoEsPropietario = "4026"       // ArtistaId del token no coincide con PromoPrograma.ArtistaId
```

**Nota sobre codigos existentes a verificar:** Los codigos `NotFound_Artista = "2016"` y `NotFound_Promotor = "2015"` ya existen en el archivo. `NotFound_PromoPrograma = "2019"` tambien existe. Solo agregar los listados arriba.

### 3.2 Nuevo Repository Interface: IPromoProgramaPromotorRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoProgramaPromotorRepository.cs`

**Accion:** CREAR archivo nuevo.

**Justificacion:** Siguiendo el patron 1:1 repo por entidad EF del proyecto, se crea un repositorio dedicado para `PromoProgramaPromotor`. El repositorio existente `IPromoProgramaRepository` gestiona `PromoPrograma` y no debe mezclarse con operaciones de inscripcion.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `PromoProgramaPromotor?` | `Guid id, CancellationToken ct` | Obtener inscripcion por PK |
| `GetByIdWithPromotorAsync` | `PromoProgramaPromotor?` | `Guid id, CancellationToken ct` | Obtener inscripcion con Include(Promotor) para datos del perfil |
| `GetByPromotorAndProgramaAsync` | `PromoProgramaPromotor?` | `PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct` | Verificar inscripcion existente (unicidad) y obtener estado actual |
| `GetPagedByProgramaAsync` | `(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)` | `PromoProgramaId programaId, string? estadoFiltro, int page, int pageSize, CancellationToken ct` | Lista paginada de inscripciones de un programa (vista artista) |
| `GetPagedByPromotorAsync` | `(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)` | `PromotorId promotorId, int page, int pageSize, CancellationToken ct` | Lista paginada de inscripciones del promotor (vista mis-programas) con Include(Programa, Programa.Tareas) |
| `ExistsByCodigoReferidoAsync` | `bool` | `string codigoReferido, CancellationToken ct` | Verificar unicidad del CodigoReferido antes de persistir |
| `AddAsync` | `Guid` | `PromoProgramaPromotor entity, CancellationToken ct` | Crear nueva inscripcion; retorna Id |
| `UpdateAsync` | `void` | `PromoProgramaPromotor entity, CancellationToken ct` | Actualizar inscripcion (aprobar, bloquear, dar de baja) |
| `DeleteAsync` | `void` | `Guid id, CancellationToken ct` | Eliminar fisicamente la inscripcion (accion rechazar) |

**Notas sobre `GetPagedByProgramaAsync`:**
- El parametro `estadoFiltro` acepta valores `"Pendiente"`, `"Aprobado"`, `"Bloqueado"`, `"DadoDeBaja"` o `null` (todos).
- La traduccion del filtro a condiciones EF se realiza dentro de la implementacion del repositorio.
- Debe incluir `Include(pp => pp.Promotor)` para devolver datos del perfil en la lista.

**Notas sobre `GetPagedByPromotorAsync`:**
- Debe incluir `Include(pp => pp.Programa).ThenInclude(p => p.Tareas.Where(t => t.EsActivo))` para el endpoint `mis-programas`.
- La carga de tareas activas solo es necesaria para inscripciones aprobadas; el filtro se aplica en el Service o en el mapper.

### 3.3 Entidades existentes referenciadas (sin cambio de firma)

Las siguientes entidades ya existen y se usan como dependencias de lectura:

| Entidad | Repositorio existente | Uso en esta feature |
|---------|-----------------------|---------------------|
| `Promotor` | `IPromotorRepository` | Validar EsActivo del promotor al solicitar inscripcion |
| `PromoPrograma` | `IPromoProgramaRepository` | Validar EsActivo del programa, obtener CodigoTrackingBase, UrlLanding, ArtistaId |

---

## 4. Infrastructure Layer

### 4.1 Repository Implementation: PromoProgramaPromotorRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoProgramaPromotorRepository.cs`

**Accion:** CREAR archivo nuevo.

- Implementa: `IPromoProgramaPromotorRepository`
- Inyecta: `CrowdpromotionContext _context`
- Usa: `?? throw new ArgumentNullException(nameof(context))` en constructor

**Detalle de implementaciones:**

| Metodo | Estrategia de implementacion |
|--------|------------------------------|
| `GetByIdAsync` | `_context.ProgramaPromotores.FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByIdWithPromotorAsync` | `_context.ProgramaPromotores.Include(pp => pp.Promotor).FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByPromotorAndProgramaAsync` | `_context.ProgramaPromotores.FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.ProgramaId == programaId, ct)` |
| `GetPagedByProgramaAsync` | Query con `Include(pp => pp.Promotor)`, filtro por `estadoFiltro` traducido a condiciones booleanas, paginacion con `Skip/Take`, `AsNoTracking()` |
| `GetPagedByPromotorAsync` | Query con `Include(pp => pp.Programa).ThenInclude(p => p.Tareas.Where(t => t.EsActivo))`, paginacion, `AsNoTracking()` |
| `ExistsByCodigoReferidoAsync` | `_context.ProgramaPromotores.AnyAsync(x => x.CodigoReferido == codigoReferido, ct)` |
| `AddAsync` | `_context.ProgramaPromotores.AddAsync(entity, ct)` + `SaveChangesAsync(ct)`. Retorna `entity.Id` |
| `UpdateAsync` | `_context.ProgramaPromotores.Update(entity)` + `SaveChangesAsync(ct)` |
| `DeleteAsync` | `FindAsync` -> `Remove` -> `SaveChangesAsync(ct)` |

**Traduccion de `estadoFiltro` a condiciones EF en `GetPagedByProgramaAsync`:**

| Valor de estadoFiltro | Condicion Where |
|-----------------------|-----------------|
| `"Pendiente"` | `!x.EsBloqueado && x.FechaBaja == null && !x.EsAprobado` |
| `"Aprobado"` | `x.EsAprobado && x.FechaBaja == null` |
| `"Bloqueado"` | `x.EsBloqueado` |
| `"DadoDeBaja"` | `x.FechaBaja != null` |
| `null` | Sin filtro adicional |

### 4.2 Nueva Interfaz e Implementacion de Service: IPromoProgramaPromotorService

**Interface:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoProgramaPromotorService.cs`

**Accion:** CREAR archivo nuevo.

**Justificacion:** El service encapsula la logica de persistencia de inscripciones, incluyendo la generacion del ShortId unico para el CodigoReferido con logica de reintento en caso de colision. Esta logica no pertenece al handler (que no debe conocer detalles de generacion), ni al repositorio (que solo hace CRUD).

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `PromoProgramaPromotor?` | `Guid id, CancellationToken ct` | Delega a repo con cache |
| `GetByIdWithPromotorAsync` | `PromoProgramaPromotor?` | `Guid id, CancellationToken ct` | Delega a repo (sin cache; datos del promotor pueden cambiar) |
| `GetByPromotorAndProgramaAsync` | `PromoProgramaPromotor?` | `PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct` | Delega a repo con cache. Usado por validator para verificar duplicados y bloqueo |
| `GetPagedByProgramaAsync` | `(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)` | `PromoProgramaId programaId, string? estadoFiltro, int page, int pageSize, CancellationToken ct` | Lista paginada de inscripciones del programa para el artista |
| `GetPagedByPromotorAsync` | `(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)` | `PromotorId promotorId, int page, int pageSize, CancellationToken ct` | Lista paginada de inscripciones del promotor (mis-programas) |
| `SolicitarInscripcionAsync` | `Guid` | `PromoProgramaPromotor entity, CancellationToken ct` | Persiste la nueva inscripcion en estado Pendiente. Retorna el Id generado |
| `AprobarInscripcionAsync` | `PromoProgramaPromotor` | `Guid inscripcionId, string codigoTrackingBase, string? urlLanding, CancellationToken ct` | Genera CodigoReferido y UrlTrackingPersonalizada, actualiza la entidad y persiste. Retorna la entidad actualizada con los codigos generados |
| `RechazarInscripcionAsync` | `void` | `Guid inscripcionId, CancellationToken ct` | Elimina fisicamente el registro |
| `BloquearInscripcionAsync` | `PromoProgramaPromotor` | `Guid inscripcionId, CancellationToken ct` | Establece EsBloqueado=true y EsAprobado=false. Persiste y retorna entidad |
| `DarDeBajaInscripcionAsync` | `PromoProgramaPromotor` | `Guid inscripcionId, CancellationToken ct` | Establece FechaBaja=UtcNow y EsAprobado=false. Persiste y retorna entidad |

**Implementation:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromoProgramaPromotorService.cs`

**Accion:** CREAR archivo nuevo.

**Dependencias que inyecta el service:**

| Dependencia | Tipo | Uso |
|-------------|------|-----|
| `IPromoProgramaPromotorRepository` | `_repository` | Operaciones CRUD de inscripciones |
| `IRequestCacheService` | `_requestCache` | Cachear GetById y GetByPromotorAndPrograma dentro del mismo request |
| `ILogger<PromoProgramaPromotorService>` | `_logger` | Logging de operaciones de persistencia |

**CRITICO:** Todas las dependencias usan `?? throw new ArgumentNullException(nameof(...))` en el constructor.

**Logica de generacion de CodigoReferido en `AprobarInscripcionAsync`:**

El metodo debe:
1. Recibir `codigoTrackingBase` (obtenido del `PromoPrograma` por el handler).
2. Generar un `ShortId` de 5 caracteres alfanumericos en minusculas usando caracteres del conjunto `[a-z0-9]`.
3. Construir el candidato: `{codigoTrackingBase}-{shortId}`.
4. Verificar unicidad via `_repository.ExistsByCodigoReferidoAsync(candidato, ct)`.
5. Si hay colision, reintentar con un nuevo ShortId (maximo 5 reintentos; si todos fallan, lanzar `InvalidOperationException`).
6. Si `codigoTrackingBase` es null o vacio, usar los primeros 8 caracteres del `inscripcionId` como base alternativa (formato: `{inscripcionId.ToString().Substring(0,8)}-{shortId}`).
7. Construir `UrlTrackingPersonalizada`: si `urlLanding != null`, construir `{urlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={codigoTrackingBase}&ref={codigoReferido}`. Si `urlLanding` es null, la URL es null.
8. Asignar `entity.CodigoReferido = codigoReferido` y `entity.UrlReferido = urlTracking`.
9. Asignar `entity.EsAprobado = true`.
10. Persistir via `_repository.UpdateAsync(entity, ct)`.

**Logica del ShortId helper (clase interna o metodo privado estatico):**

```
private static string GenerarShortId(int length = 5)
{
    // Caracteres permitidos: a-z, 0-9 (excluye O, I, l para evitar confusion visual - opcional)
    const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    // Usar Random.Shared o Guid para aleatoriedad
    // Retornar string de `length` caracteres
}
```

**Nota sobre transacciones:** A diferencia de `PromoProgramaService` que usa `_context.Database.BeginTransactionAsync`, el service de inscripciones NO necesita transacciones explicitas porque cada operacion actua sobre una sola entidad. Las operaciones complejas (aprobar = generar codigo + update) se realizan en memoria antes del `UpdateAsync` y la atomicidad esta garantizada por el SaveChanges unico del repositorio.

### 4.3 Modificacion: PromoProgramaService (service existente)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromoProgramaService.cs`

**Accion:** MODIFICAR. Agregar metodo para el endpoint `GET /explorar`.

**Metodo a agregar en `IPromoProgramaService`:**

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetProgramasActivosPagedAsync` | `(IReadOnlyList<PromoPrograma> Items, int TotalCount)` | Lista paginada de programas activos para el catalogo publico de explorar. Filtros: `artistaNombre` (contains), `tipoPromoId`, paginacion |

**Firma completa:**

```
Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetProgramasActivosPagedAsync(
    string? artistaNombre,
    int? tipoPromoId,
    int page,
    int pageSize,
    CancellationToken ct);
```

**Notas sobre la implementacion de este metodo:**
- Debe incluir `Include(p => p.Tareas.Where(t => t.EsActivo))` para calcular `NumeroTareas` en el handler.
- El join con `Artista` para `artistaNombre` se realiza via `SqlQueryRaw` o mediante un metodo de proyeccion en el repositorio (ver seccion 4.4).
- Solo devuelve programas con `EsActivo == true`.
- El filtro por `artistaNombre` es un contains case-insensitive.

**Razon para no crear un repositorio nuevo de exploracion:** El query de exploracion sigue siendo sobre la entidad `PromoPrograma`. Se extiende el repositorio/service existente con el nuevo metodo en lugar de crear uno nuevo.

### 4.4 Modificacion: IPromoProgramaRepository (repositorio existente)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoProgramaRepository.cs`

**Accion:** MODIFICAR. Agregar un metodo para soportar el catalogo publico.

**Metodo a agregar:**

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetActivosPagedAsync` | `(IReadOnlyList<PromoPrograma> Items, int TotalCount)` | `string? artistaNombre, int? tipoPromoId, int page, int pageSize, CancellationToken ct` | Query de programas activos con filtros para el catalogo de explorar |

**Nota sobre join con Artista:** El campo `ArtistaNombre` del DTO viene de la tabla `Artista` (modulo UserAccess). Para evitar dependencia cruzada de contextos, el repositorio puede:
- Opcion A (recomendada para MVP): Usar `SqlQueryRaw` similar al patron ya establecido en `PromoProgramaService.GetArtistaIdByUserIdAsync`. El handler resuelve el nombre del artista a partir del `ArtistaId` via el mismo patron de cross-context query.
- Opcion B: Incluir un campo desnormalizado o resolver el nombre en el handler con una segunda consulta (menos eficiente pero mas simple).

**Decision arquitectonica:** Seguir Opcion A. El `GetActivosPagedAsync` del repositorio devuelve `PromoPrograma` con sus `Tareas`. El handler, al mapear al DTO, llama a `IPromoProgramaService.GetArtistaIdByUserIdAsync` adaptado (o un nuevo metodo `GetArtistaNombreByIdAsync`) para resolver el nombre del artista. La implementacion concreta del cross-context query ya esta demostrada en el codebase actual.

### 4.5 Modificacion: CrowdpromotionContext

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

**Accion:** MODIFICAR. Agregar configuracion EF para los dos campos nuevos de `PromoProgramaPromotor`.

**Cambios en `OnModelCreating` - bloque de `PromoProgramaPromotor`:**

Dentro del bloque existente `modelBuilder.Entity<PromoProgramaPromotor>(entity => { ... })`, agregar:

| Configuracion | Detalle |
|---------------|---------|
| `entity.Property(e => e.EsAprobado).HasDefaultValue(false)` | Campo bool con default false en BD |
| `entity.Property(e => e.EsBloqueado).HasDefaultValue(false)` | Campo bool con default false en BD |

**Columnas resultantes en tabla `PromoProgramaPromotor`:**

| Columna | Tipo SQL | Nullable | Default | Nota |
|---------|----------|----------|---------|------|
| `EsAprobado` | `BIT` | NOT NULL | `0` | Nueva columna |
| `EsBloqueado` | `BIT` | NOT NULL | `0` | Nueva columna |

**Importante:** No se agrega un nuevo `DbSet`. La tabla `PromoProgramaPromotor` ya esta mapeada como `public DbSet<PromoProgramaPromotor> ProgramaPromotores => Set<PromoProgramaPromotor>();`. Solo se extiende la configuracion de la entidad existente.

### 4.6 Modificacion: DependencyInjection

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

**Accion:** MODIFICAR. Registrar el nuevo repositorio y service.

**Registros a AGREGAR:**

```csharp
// Repositories
services.AddScoped<IPromoProgramaPromotorRepository, PromoProgramaPromotorRepository>();

// Services
services.AddScoped<IPromoProgramaPromotorService, PromoProgramaPromotorService>();
```

---

## 5. Migracion EF Core

**Nombre sugerido:** `AddInscripcionEstadoFields`

**Descripcion:** Agrega las columnas `EsAprobado` y `EsBloqueado` a la tabla `PromoProgramaPromotor`.

**Comando:**

```bash
dotnet ef migrations add AddInscripcionEstadoFields --project src/api/WebApi
dotnet ef database update --project src/api/WebApi
```

**SQL generado esperado (Up):**

```sql
ALTER TABLE [PromoProgramaPromotor]
    ADD [EsAprobado] BIT NOT NULL DEFAULT 0,
        [EsBloqueado] BIT NOT NULL DEFAULT 0;
```

**Impacto en datos existentes:** Los registros existentes en `PromoProgramaPromotor` recibiran `EsAprobado = 0` y `EsBloqueado = 0` por el DEFAULT. Esto es correcto: todos los registros pre-existentes quedan en estado `Pendiente`, lo cual es coherente dado que el campo `EsActivo` existente sera revisado en la logica de negocio futura. No hay roturas de datos.

---

## 6. Resumen de Archivos

### 6.1 Archivos a CREAR

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   └── Interfaces/
│       └── IPromoProgramaPromotorRepository.cs          (NUEVO)
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Repositories/
    │   └── PromoProgramaPromotorRepository.cs           (NUEVO)
    └── Services/
        └── PromoProgramaPromotorService.cs              (NUEVO)

src/api/Modules/Crowdpromotion/
└── WePlayRises.Crowdpromotion.Application/
    └── Interfaces/
        └── Services/
            └── IPromoProgramaPromotorService.cs         (NUEVO)
```

### 6.2 Archivos a MODIFICAR

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Model/
│   │   └── PromoProgramaPromotor.cs          (MODIFICAR: agregar EsAprobado, EsBloqueado)
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs     (MODIFICAR: agregar 6 nuevas constantes)
│   └── Interfaces/
│       └── IPromoProgramaRepository.cs       (MODIFICAR: agregar GetActivosPagedAsync)
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Context/
    │   └── CrowdpromotionContext.cs           (MODIFICAR: agregar HasDefaultValue para EsAprobado/EsBloqueado)
    ├── Repositories/
    │   └── PromoProgramaRepository.cs         (MODIFICAR: implementar GetActivosPagedAsync)
    ├── Services/
    │   └── PromoProgramaService.cs            (MODIFICAR: agregar GetProgramasActivosPagedAsync)
    └── DependencyInjection.cs                 (MODIFICAR: registrar nuevo repo y service)

src/api/Migrations/ (o WebApi/Migrations/)
└── {timestamp}_AddInscripcionEstadoFields.cs  (NUEVO - generado por EF)
```

---

## 7. Diagrama de Dependencias

```
Handler (Application Layer)
    |
    +-- IPromoProgramaPromotorService (nueva)
    |       |
    |       +-- IPromoProgramaPromotorRepository (nueva)
    |       |       |
    |       |       +-- CrowdpromotionContext
    |       |
    |       +-- IRequestCacheService (BuildingBlocks)
    |       +-- ILogger<PromoProgramaPromotorService>
    |
    +-- IPromoProgramaService (existente, extendida)
    |       |
    |       +-- IPromoProgramaRepository (existente, extendido)
    |       +-- IRequestCacheService
    |       +-- ILogger<PromoProgramaService>
    |       +-- CrowdpromotionContext (cross-context queries)
    |
    +-- IPromotorService (existente, sin cambios)
            |
            +-- IPromotorRepository
            +-- IRequestCacheService
```

---

## 8. Patrones de Cache por Service

El `IRequestCacheService` se usa para evitar queries duplicados dentro del mismo request HTTP.

| Clave de cache | Service | Invalidacion |
|----------------|---------|--------------|
| `ppp:id:{id}` | `PromoProgramaPromotorService.GetByIdAsync` | Por request (request-scoped cache) |
| `ppp:promotor:{promotorId}:programa:{programaId}` | `GetByPromotorAndProgramaAsync` | Por request. Usado por validator y handler en el mismo request |
| `promo-programa:{id}` | `PromoProgramaService.GetByIdAsync` | Ya existe; reutilizado sin cambio |
| `promotor:userid:{userId}` | `PromotorService.GetByUserIdAsync` | Ya existe; reutilizado sin cambio |

---

## 9. Checklist de Arquitectura

- [ ] Entidad `PromoProgramaPromotor` modificada con `EsAprobado` y `EsBloqueado` como POCOs (solo propiedades, sin metodos de negocio)
- [ ] `ServiceResponseMessageType.cs` actualizado con 6 nuevas constantes (2020, 4021-4026)
- [ ] `IPromoProgramaPromotorRepository` en Domain/Interfaces (puerto)
- [ ] `PromoProgramaPromotorRepository` en Infra/Repositories (adaptador)
- [ ] `IPromoProgramaPromotorService` en Application/Interfaces/Services
- [ ] `PromoProgramaPromotorService` en Infra/Services
- [ ] Service inyecta `IPromoProgramaPromotorRepository` + `IRequestCacheService` + `ILogger`
- [ ] Service usa `?? throw new ArgumentNullException` en constructor para TODAS las dependencias
- [ ] Service retorna entidades (`PromoProgramaPromotor`), NO DTOs
- [ ] Logica de generacion de ShortId con reintento en colision encapsulada en `PromoProgramaPromotorService.AprobarInscripcionAsync`
- [ ] `IPromoProgramaRepository` extendido con `GetActivosPagedAsync`
- [ ] `IPromoProgramaService` extendido con `GetProgramasActivosPagedAsync`
- [ ] `CrowdpromotionContext` actualizado con `HasDefaultValue(false)` para ambas columnas nuevas
- [ ] `DependencyInjection.cs` actualizado con registro del nuevo repo y service
- [ ] Migracion EF Core generada con nombre `AddInscripcionEstadoFields`
- [ ] Repository hace `SaveChangesAsync` (no el Handler ni el Service via UoW)
- [ ] Handlers NO inyectan `CrowdpromotionContext` directamente
