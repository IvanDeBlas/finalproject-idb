# Arquitectura Hexagonal: cp-tareas-promocion

**Fecha:** 2026-03-01
**Modulo:** Crowdpromotion
**Feature:** cp-tareas-promocion (US-CP-04)
**Depende de:** cp-inscripcion-programa (US-CP-03)

---

## 1. Resumen Ejecutivo

Esta feature implementa el ciclo central de trabajo de Crowdpromotion: el promotor aprobado en un programa ejecuta tareas de difusion (compartir en redes, publicar videos, escribir resenas), envia una URL de prueba para validacion, y el artista propietario revisa, valida o rechaza esos completados. Al validar, el sistema acredita automaticamente la recompensa en la wallet del promotor dentro de una sola transaccion de base de datos.

La capa de dominio requiere **dos nuevos repository interfaces** (`IPromoTareaRepository` e `IPromoTareaPromotorRepository`) ya que las entidades `PromoTarea` y `PromoTareaPromotor` existen en el modelo pero no tienen repositorios dedicados. La capa de infraestructura agrega **un nuevo service** (`IPromoTareaService`) y extiende `IPromotorWalletService` con el metodo de acreditacion transaccional. No se requieren cambios de esquema de base de datos porque se elige la Estrategia A: los campos `VecesCompletada`, `FechaPrimeraCompletada` y `FechaUltimaCompletada` son calculados via agregacion SQL en la capa de consulta, sin columnas nuevas en `PromoTareaPromotor`.

---

## 2. Analisis del Dominio Existente

### 2.1 Entidades ya existentes (sin cambios de esquema)

Las siguientes entidades ya existen en `WePlayRises.Crowdpromotion.Domain/Model/` y sus tablas ya estan configuradas en `CrowdpromotionContext`. **No requieren modificaciones estructurales** para esta feature.

| Entidad | Tabla | Estado |
|---------|-------|--------|
| `PromoTarea` | `PromoTarea` | Existe. Sin cambios de columnas. |
| `PromoTareaPromotor` | `PromoTareaPromotor` | Existe. Sin cambios de columnas. |
| `PromoProgramaPromotor` | `PromoProgramaPromotor` | Existe. Sin cambios. |
| `PromoPrograma` | `PromoPrograma` | Existe. Sin cambios. |
| `Promotor` | `Promotor` | Existe. Sin cambios. |
| `PromotorWallet` | `PromotorWallet` | Existe. Sin cambios. |
| `PromotorWalletTransaccion` | `PromotorWalletTransaccion` | Existe. Sin cambios. |

### 2.2 Decision de diseno: Estrategia A para campos calculados

Los campos `VecesCompletada`, `FechaPrimeraCompletada` y `FechaUltimaCompletada` mencionados en la US **no se agregan como columnas a `PromoTareaPromotor`**. Se calculan en la capa de consulta mediante agregacion:

| Campo en DTO | Calculo en query |
|---|---|
| `VecesCompletada` | `COUNT(*)` de `PromoTareaPromotor` por `TareaId + ProgramaPromotorId` donde `EstadoTareaId IN (2, 3, 4)` |
| `FechaPrimeraCompletada` | `MIN(FechaCompletado)` del mismo agrupamiento |
| `FechaUltimaCompletada` | `MAX(FechaCompletado)` del mismo agrupamiento |

**Consecuencia:** No se requiere migracion de EF Core para la entidad `PromoTareaPromotor`. El unico cambio de esquema considerado es ninguno.

### 2.3 Maquina de estados de PromoTareaPromotor.EstadoTareaId

Seed en `Maestra_EstadoTareaPromo`:

| Id | Nombre | Significado |
|----|--------|-------------|
| 1 | Pendiente | Estado conceptual; no se crea registro en BD |
| 2 | Completada | Promotor envio URL, pendiente de validacion |
| 3 | Validada | Artista aprobo, recompensa acreditada |
| 4 | Rechazada | Artista rechazo, promotor puede re-enviar |

**Transiciones validas:**

```
[Sin registro]  --POST completar--> Completada (2)  [nuevo registro]
Completada (2)  --PATCH validar-->  Validada (3)
Completada (2)  --PATCH rechazar--> Rechazada (4)
Rechazada (4)   --POST completar--> Completada (2)  [actualiza registro existente: URL, FechaCompletado, limpia ComentarioValidacion/FechaValidado]
Validada (3)    --POST completar--> Completada (2)  [nuevo registro, solo si EsRepetible = true]
```

---

## 3. Domain Layer

### 3.1 Entidades (sin cambios - solo documentacion de campos relevantes)

#### PromoTareaPromotor

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoTareaPromotor.cs`

**Estado:** EXISTE - sin modificaciones

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `Id` | `Guid` | No | PK |
| `TareaId` | `Guid` | No | FK a `PromoTarea` |
| `ProgramaPromotorId` | `Guid` | No | FK a `PromoProgramaPromotor` |
| `EstadoTareaId` | `int` | No | FK a `Maestra_EstadoTareaPromo` (1=Pendiente, 2=Completada, 3=Validada, 4=Rechazada) |
| `UrlPruebaCompletado` | `string?` | Si | URL enviada por el promotor como prueba. Max 500 chars. |
| `ComentarioPromotor` | `string?` | Si | Comentario del promotor al completar |
| `ComentarioValidacion` | `string?` | Si | Comentario del artista al validar/rechazar |
| `FechaCompletado` | `DateTime?` | Si | Timestamp de cuando el promotor envio la prueba |
| `FechaValidado` | `DateTime?` | Si | Timestamp de cuando el artista valido o rechazo |
| `FechaCreacion` | `DateTime` | No | Timestamp de creacion del registro |

**Navegaciones existentes:**
- `Tarea` -> `PromoTarea` (N:1)
- `ProgramaPromotor` -> `PromoProgramaPromotor` (N:1)

**Nota critica sobre la URL:** La columna actual tiene `HasMaxLength(500)` en la configuracion de EF. El contrato define max 2048 caracteres para la URL. Ver seccion 3.4 para la configuracion requerida.

#### PromoTarea

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoTarea.cs`

**Estado:** EXISTE - sin modificaciones

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `Id` | `Guid` | No | PK |
| `ProgramaId` | `PromoProgramaId` | No | FK a `PromoPrograma` |
| `Titulo` | `string` | No | Nombre visible de la tarea |
| `Descripcion` | `string?` | Si | Descripcion extendida |
| `InstruccionesUrl` | `string?` | Si | URL con instrucciones detalladas |
| `TipoRewardId` | `int?` | Si | FK a `MaestraTipoReward` (1=Dinero, 2=Puntos, 3=Descuento) |
| `ImporteRecompensa` | `decimal?` | Si | Importe monetario de la recompensa |
| `MonedaId` | `int?` | Si | FK a `MaestraMoneda` |
| `PuntosRecompensa` | `int?` | Si | Puntos de recompensa |
| `TipoEventoPromoId` | `int` | No | FK a `MaestraTipoEventoPromo` |
| `EsRepetible` | `bool` | No | Si permite multiples completados |
| `MaxRepeticiones` | `int?` | Si | Limite de completados para tareas repetibles |
| `FechaInicio` | `DateTime?` | Si | Inicio de vigencia de la tarea |
| `FechaFin` | `DateTime?` | Si | Fin de vigencia de la tarea |
| `Orden` | `int` | No | Orden de presentacion en la lista |
| `EsActivo` | `bool` | No | Si la tarea esta activa |
| `FechaCreacion` | `DateTime` | No | Timestamp de creacion |

**Navegaciones existentes:**
- `Programa` -> `PromoPrograma` (N:1)
- `TareasPromotor` -> `ICollection<PromoTareaPromotor>` (1:N)

### 3.2 Repository Interfaces (nuevas - ports)

#### IPromoTareaRepository

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoTareaRepository.cs`

**Estado:** NUEVO - no existe actualmente

**Namespace:** `WePlayRises.Crowdpromotion.Domain.Interfaces`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `Task<PromoTarea?>` | `Guid id, CancellationToken ct` | Obtiene una tarea por su Id. Retorna null si no existe. |
| `GetByIdAndProgramaAsync` | `Task<PromoTarea?>` | `Guid tareaId, PromoProgramaId programaId, CancellationToken ct` | Obtiene tarea verificando que pertenezca al programa. Usado para validar el path param tareaId en el endpoint POST completar. |
| `GetTareasByProgramaAsync` | `Task<IReadOnlyList<PromoTarea>>` | `PromoProgramaId programaId, CancellationToken ct` | Lista todas las tareas activas (`EsActivo = true`) de un programa. Usado en GET mis-tareas. |

**Justificacion de metodos:**
- `GetByIdAsync`: Para resolver la tarea en PATCH validar/rechazar (via PromoTareaPromotor.TareaId).
- `GetByIdAndProgramaAsync`: Valida que `tareaId` del path pertenece al programa en POST completar, evitando queries separadas.
- `GetTareasByProgramaAsync`: Carga las tareas activas para el promotor en GET mis-tareas. No hace LEFT JOIN con completados (eso lo hace el service).

#### IPromoTareaPromotorRepository

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoTareaPromotorRepository.cs`

**Estado:** NUEVO - no existe actualmente

**Namespace:** `WePlayRises.Crowdpromotion.Domain.Interfaces`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `Task<PromoTareaPromotor?>` | `Guid id, CancellationToken ct` | Obtiene un completado por su Id. Retorna null si no existe. Para PATCH validar/rechazar. |
| `GetByIdWithTareaAndProgramaAsync` | `Task<PromoTareaPromotor?>` | `Guid id, CancellationToken ct` | Obtiene completado con Include de Tarea y ProgramaPromotor (con Promotor). Para PATCH validar: necesita Tarea.ImporteRecompensa, Tarea.MonedaId, ProgramaPromotor.PromotorId. |
| `GetUltimoByTareaAndProgramaPromotorAsync` | `Task<PromoTareaPromotor?>` | `Guid tareaId, Guid programaPromotorId, CancellationToken ct` | Obtiene el registro mas reciente (`ORDER BY FechaCreacion DESC`) para el par tarea+inscripcion. Usado en POST completar para determinar si hay un registro rechazado que actualizar. |
| `GetCompletadosByTareaAndProgramaPromotorAsync` | `Task<IReadOnlyList<PromoTareaPromotor>>` | `Guid tareaId, Guid programaPromotorId, CancellationToken ct` | Lista todos los registros para el par tarea+inscripcion. Sin AsNoTracking para permitir updates. Usado en la validacion de limites de repeticion. |
| `CountCompletadosActivosAsync` | `Task<int>` | `Guid tareaId, Guid programaPromotorId, CancellationToken ct` | COUNT de registros con `EstadoTareaId IN (2, 3)` para el par tarea+inscripcion. Para validar MaxRepeticiones. |
| `GetPendientesPagedAsync` | `Task<(IReadOnlyList<PromoTareaPromotor> Items, int TotalCount)>` | `PromoProgramaId programaId, int page, int pageSize, CancellationToken ct` | Lista paginada de completados con `EstadoTareaId = 2` para un programa. Incluye Tarea y ProgramaPromotor+Promotor. Para GET tareas-pendientes del artista. |
| `AddAsync` | `Task<Guid>` | `PromoTareaPromotor entity, CancellationToken ct` | Crea un nuevo registro de completado. Retorna el Id generado. |
| `UpdateAsync` | `Task` | `PromoTareaPromotor entity, CancellationToken ct` | Actualiza un registro existente. Usado para actualizar estado, FechaValidado, ComentarioValidacion. |

**Justificacion de metodos complejos:**
- `GetByIdWithTareaAndProgramaAsync`: En PATCH validar necesitamos en un solo query: el completado, su tarea (para importeRecompensa, monedaId, tipoRewardId, titulo) y la inscripcion con el promotorId (para wallet). Evita N+1 queries.
- `GetPendientesPagedAsync`: Query con filtro `EstadoTareaId = 2`, Include de navegaciones para nombre del promotor y nombre de tarea, con paginacion. Segun el contrato, `vecesCompletada` en la respuesta del artista se calcula como COUNT total del par tarea+promotor (todos los estados). El repositorio retorna los registros crudos; el service calcula el COUNT en una sub-consulta o con GROUP BY.
- `CountCompletadosActivosAsync`: Operacion de COUNT eficiente para la regla de negocio de MaxRepeticiones sin cargar entidades completas.

### 3.3 Actualizacion de ServiceResponseMessageType

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Estado:** EXISTE - agregar constantes nuevas para esta feature

Constantes existentes en el archivo que se reutilizan:
- `Validation_Required = "1001"` - URL de prueba obligatoria
- `Validation_MaxLength = "1002"` - Comentario excede longitud maxima
- `Validation_InvalidUrl = "1013"` - URL de prueba no tiene formato valido
- `NotFound_Promotor = "2015"` - No existe perfil de promotor
- `NotFound_Artista = "2016"` - No existe perfil de artista
- `NotFound_PromoPrograma = "2019"` - Programa no encontrado
- `Auth_Unauthorized = "3001"` - Token invalido
- `BusinessRule_ProgramaInactivo = "4024"` - Programa no activo
- `BusinessRule_NoEsPropietarioPrograma = "4026"` - Artista no es propietario
- `Internal_UnexpectedError = "5000"` - Error inesperado

**Constantes NUEVAS a agregar:**

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `NotFound_PromoTarea` | `"2021"` | La tarea no existe o no pertenece a este programa |
| `NotFound_PromoTareaPromotor` | `"2022"` | El completado no existe o no pertenece a este programa |
| `BusinessRule_TareaNoRepetible` | `"4027"` | Tarea no repetible ya completada o validada |
| `BusinessRule_MaxRepeticionesAlcanzado` | `"4028"` | Limite de repeticiones alcanzado |
| `BusinessRule_TareaInactiva` | `"4029"` | Tarea no esta activa |
| `BusinessRule_TareaFueraFecha` | `"4030"` | Tarea fuera de fechas de vigencia |
| `BusinessRule_CompletadoEstadoInvalido` | `"4031"` | Completado no esta en estado Completada (id=2) |

**Seccion del archivo a agregar (despues de `NotFound_Inscripcion = "2020"`):**

```
// NotFound (2000-2999) - continuacion
public const string NotFound_PromoTarea = "2021";
public const string NotFound_PromoTareaPromotor = "2022";

// Business Rules (4000-4999) - continuacion (despues de BusinessRule_NoEsPropietarioPrograma)
public const string BusinessRule_TareaNoRepetible = "4027";
public const string BusinessRule_MaxRepeticionesAlcanzado = "4028";
public const string BusinessRule_TareaInactiva = "4029";
public const string BusinessRule_TareaFueraFecha = "4030";
public const string BusinessRule_CompletadoEstadoInvalido = "4031";
```

---

## 4. Infrastructure Layer

### 4.1 Repository Implementations

#### PromoTareaRepository

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoTareaRepository.cs`

**Estado:** NUEVO

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Repositories`

**Implementa:** `IPromoTareaRepository`

**Inyecta:** `CrowdpromotionContext`

| Metodo | Implementacion EF Core |
|--------|------------------------|
| `GetByIdAsync` | `_context.Tareas.FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByIdAndProgramaAsync` | `_context.Tareas.FirstOrDefaultAsync(x => x.Id == tareaId && x.ProgramaId == programaId, ct)` |
| `GetTareasByProgramaAsync` | `_context.Tareas.AsNoTracking().Where(x => x.ProgramaId == programaId && x.EsActivo).OrderBy(x => x.Orden).ToListAsync(ct)` |

**Notas:**
- `GetTareasByProgramaAsync` usa `AsNoTracking` porque es query de solo lectura.
- El filtro `EsActivo = true` se aplica directamente en la query; no cargar tareas inactivas.
- Ordenar por `Orden` para presentacion correcta al promotor.
- El repository NO hace SaveChanges; PromoTarea es de lectura en esta feature (las tareas las crea/edita el artista en la feature cp-programas-promocion).

#### PromoTareaPromotorRepository

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoTareaPromotorRepository.cs`

**Estado:** NUEVO

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Repositories`

**Implementa:** `IPromoTareaPromotorRepository`

**Inyecta:** `CrowdpromotionContext`

| Metodo | Implementacion EF Core |
|--------|------------------------|
| `GetByIdAsync` | `_context.TareaPromotores.FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByIdWithTareaAndProgramaAsync` | `_context.TareaPromotores.Include(x => x.Tarea).Include(x => x.ProgramaPromotor).ThenInclude(pp => pp.Promotor).FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetUltimoByTareaAndProgramaPromotorAsync` | `_context.TareaPromotores.Where(x => x.TareaId == tareaId && x.ProgramaPromotorId == programaPromotorId).OrderByDescending(x => x.FechaCreacion).FirstOrDefaultAsync(ct)` |
| `GetCompletadosByTareaAndProgramaPromotorAsync` | `_context.TareaPromotores.Where(x => x.TareaId == tareaId && x.ProgramaPromotorId == programaPromotorId).ToListAsync(ct)` (sin AsNoTracking, permite updates) |
| `CountCompletadosActivosAsync` | `_context.TareaPromotores.CountAsync(x => x.TareaId == tareaId && x.ProgramaPromotorId == programaPromotorId && (x.EstadoTareaId == 2 || x.EstadoTareaId == 3), ct)` |
| `GetPendientesPagedAsync` | Query paginada con filtro `EstadoTareaId == 2`, Include de Tarea y ProgramaPromotor+Promotor, AsNoTracking, OrderByDescending(FechaCreacion) |
| `AddAsync` | `await _context.TareaPromotores.AddAsync(entity, ct); await _context.SaveChangesAsync(ct); return entity.Id;` |
| `UpdateAsync` | `_context.TareaPromotores.Update(entity); await _context.SaveChangesAsync(ct);` |

**Detalle de `GetPendientesPagedAsync`:**

La query del artista para tareas pendientes requiere:
- Filtro: `EstadoTareaId == 2` (solo Completada)
- Filtro por programa: JOIN a traves de `ProgramaPromotor.ProgramaId == programaId`
- Include: `Tarea` (para `TareaId` y `Titulo`), `ProgramaPromotor` + `.ThenInclude(pp => pp.Promotor)` (para `PromotorId`, `NombrePublico`)
- La query filtra por `x.ProgramaPromotor.ProgramaId == programaId` en el WHERE
- `vecesCompletada` en el DTO del artista requiere un subquery COUNT; el repositorio retorna los `PromoTareaPromotor` crudos con las navegaciones cargadas. El COUNT de `vecesCompletada` es calculado por el service via una segunda query por cada item (o con GroupBy si se optimiza)

**Nota sobre `vecesCompletada` en GET tareas-pendientes:**
El repositorio retorna los registros con sus navegaciones. El service `PromoTareaService` calcula `VecesCompletada` para cada item usando una query separada de COUNT o incluye una proyeccion con GroupBy. Para MVP, el patron aceptable es: cargar los pendientes paginados + una query COUNT agrupada para todos los pares (TareaId, ProgramaPromotorId) de esa pagina. Esto se documenta como responsabilidad del service, no del repository.

### 4.2 Service Interface

#### IPromoTareaService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoTareaService.cs`

**Estado:** NUEVO

**Namespace:** `WePlayRises.Crowdpromotion.Application.Interfaces.Services`

Siguiendo el patron del modulo (interfaces en Application, implementaciones en Infra):

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetPromotorByUserIdAsync` | `Task<Promotor?>` | `string userId, CancellationToken ct` | Resuelve el Promotor desde el UserId del token. Cache-first. Mismo patron que InscripcionService. |
| `GetArtistaIdByUserIdAsync` | `Task<ArtistaId?>` | `string userId, CancellationToken ct` | Resuelve el ArtistaId desde el UserId del token. Cache-first via SQL cross-module. |
| `GetProgramaByIdAsync` | `Task<PromoPrograma?>` | `PromoProgramaId programaId, CancellationToken ct` | Carga el PromoPrograma para verificar EsActivo y ArtistaId. Cache-first. |
| `GetInscripcionByPromotorYProgramaAsync` | `Task<PromoProgramaPromotor?>` | `PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct` | Carga la inscripcion del promotor para verificar EsAprobado y EsBloqueado. Cache-first. |
| `GetTareasByProgramaAsync` | `Task<IReadOnlyList<PromoTarea>>` | `PromoProgramaId programaId, CancellationToken ct` | Lista tareas activas del programa. Usado en GET mis-tareas. |
| `GetTareaByIdAndProgramaAsync` | `Task<PromoTarea?>` | `Guid tareaId, PromoProgramaId programaId, CancellationToken ct` | Obtiene tarea verificando pertenencia al programa. Cache con clave compuesta. |
| `GetMisTareasAsync` | `Task<MisTareasResponseDto>` | `PromoProgramaId programaId, Guid programaPromotorId, CancellationToken ct` | Construye el DTO completo de GET mis-tareas con campos calculados (VecesCompletada, FechaPrimeraCompletada, FechaUltimaCompletada). Orquesta tareas + completados + maestras. |
| `CompletarTareaAsync` | `Task<CompletarTareaResponseDto>` | `Guid tareaId, Guid programaPromotorId, string urlPrueba, string? comentario, CancellationToken ct` | Ejecuta la logica de completado: crear nuevo registro o actualizar el rechazado mas reciente. Retorna el DTO de respuesta. |
| `GetTareasPendientesAsync` | `Task<(IReadOnlyList<TareaPendienteItemDto> Items, int TotalCount)>` | `PromoProgramaId programaId, int page, int pageSize, CancellationToken ct` | Lista paginada de completados pendientes de validacion con campos calculados. |
| `ValidarTareaAsync` | `Task<ValidarTareaResponseDto>` | `Guid tareaPromotorId, PromoProgramaId programaId, string? comentario, CancellationToken ct` | Valida un completado y acredita la recompensa transaccionalmente. Retorna DTO de respuesta. |
| `RechazarTareaAsync` | `Task<RechazarTareaResponseDto>` | `Guid tareaPromotorId, PromoProgramaId programaId, string comentario, CancellationToken ct` | Rechaza un completado guardando el comentario obligatorio. Retorna DTO de respuesta. |
| `GetCompletadoByIdWithFullDetailAsync` | `Task<PromoTareaPromotor?>` | `Guid tareaPromotorId, CancellationToken ct` | Carga el completado con todas sus navegaciones. Usado en validar/rechazar. |

**Nota sobre la separacion de responsabilidades:**
- Los metodos `GetPromotorByUserIdAsync`, `GetArtistaIdByUserIdAsync`, `GetProgramaByIdAsync`, `GetInscripcionByPromotorYProgramaAsync` duplican funcionalidad existente en `InscripcionService` y `PromoProgramaService`. Para MVP se acepta esta duplicacion con cache compartida (misma clave de cache por request). En una refactorizacion futura se podria extraer a un servicio de identidad compartido.
- Los metodos `CompletarTareaAsync`, `ValidarTareaAsync`, `RechazarTareaAsync` orquestan la logica de persistencia y retornan DTOs directamente porque la informacion para construir el DTO (conteos, nombres de maestras) requiere acceder al contexto en el mismo scope transaccional.

### 4.3 Service Implementation

#### PromoTareaService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromoTareaService.cs`

**Estado:** NUEVO

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Services`

**Implementa:** `IPromoTareaService`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_tareaRepository` | `IPromoTareaRepository` | Queries sobre PromoTarea |
| `_tareaPromotorRepository` | `IPromoTareaPromotorRepository` | CRUD sobre PromoTareaPromotor |
| `_promotorRepository` | `IPromotorRepository` | Resolver promotor por UserId |
| `_programaPromotorRepository` | `IPromoProgramaPromotorRepository` | Verificar inscripcion del promotor |
| `_programaRepository` | `IPromoProgramaRepository` | Cargar PromoPrograma |
| `_walletRepository` | `IPromotorWalletRepository` | Buscar/crear wallet para acreditacion |
| `_requestCache` | `IRequestCacheService` | Cache por request para evitar queries duplicados |
| `_logger` | `ILogger<PromoTareaService>` | Logging de operaciones |
| `_context` | `CrowdpromotionContext` | Transacciones explicitas en ValidarTareaAsync + queries cross-entity |

**Implementacion de metodos clave:**

**`GetMisTareasAsync`:**
1. Cargar `PromoPrograma` por `programaId` (cache)
2. Cargar lista de `PromoTarea` activas via `_tareaRepository.GetTareasByProgramaAsync`
3. Cargar todos los `PromoTareaPromotor` para `programaPromotorId` en una sola query (no N+1)
4. Para cada tarea, filtrar sus completados desde la coleccion cargada en memoria
5. Calcular `VecesCompletada = completados.Count(x => x.EstadoTareaId != 1)`, `FechaPrimeraCompletada = completados.Min(x => x.FechaCompletado)`, `FechaUltimaCompletada = completados.Max(x => x.FechaCompletado)`
6. Seleccionar el registro mas reciente por `FechaCreacion DESC` para `tareaPromotorId` y `estadoTareaId` del `miEstado`
7. Resolver nombres de maestras via tablas maestras en memoria (cache por request o hardcoded para MVP)
8. Construir y retornar `MisTareasResponseDto`

**`CompletarTareaAsync`:**
1. Cargar `PromoTareaPromotor` mas reciente via `_tareaPromotorRepository.GetUltimoByTareaAndProgramaPromotorAsync`
2. Si el ultimo registro tiene `EstadoTareaId == 4` (Rechazada): actualizar ese registro (URL, Comentario, FechaCompletado = now, EstadoTareaId = 2, limpiar ComentarioValidacion y FechaValidado)
3. Si no existe registro previo O el mas reciente es Validado (y la tarea es repetible): crear nuevo `PromoTareaPromotor` con `EstadoTareaId = 2`, `FechaCompletado = now`, `FechaCreacion = now`
4. Calcular `VecesCompletada` post-operacion con COUNT
5. Construir y retornar `CompletarTareaResponseDto`

**`ValidarTareaAsync` - flujo transaccional critico:**

```
Usar _context directamente para transaccion explicita:

1. await using var tx = await _context.Database.BeginTransactionAsync(ct);
try {
    // a. Cargar completado con Tarea y ProgramaPromotor.Promotor
    // b. Verificar EstadoTareaId == 2
    // c. Actualizar PromoTareaPromotor: EstadoTareaId = 3, FechaValidado = now, ComentarioValidacion
    // d. Si Tarea.ImporteRecompensa != null:
    //    - Buscar PromotorWallet por (PromotorId, Tarea.MonedaId)
    //    - Si no existe: crear nueva PromotorWallet con SaldoDisponible=0, SaldoPendiente=0, TotalGanado=0
    //    - Crear PromotorWalletTransaccion: TipoRewardId=Tarea.TipoRewardId, Importe=Tarea.ImporteRecompensa,
    //      EstadoTransaccionId=1, Concepto="Recompensa tarea: {Tarea.Titulo}",
    //      ReferenciaExterna=tareaPromotorId.ToString()
    //    - Actualizar PromotorWallet.SaldoPendiente += importe, TotalGanado += importe, FechaActualizacion = now
    // e. SaveChangesAsync (dentro de la transaccion)
    await tx.CommitAsync(ct);
    return ValidarTareaResponseDto { ... }
} catch {
    await tx.RollbackAsync(ct);
    throw;
}
```

**Razon para usar `CrowdpromotionContext` directamente en `ValidarTareaAsync`:** La acreditacion de la recompensa es una operacion que involucra multiples entidades (`PromoTareaPromotor`, `PromotorWalletTransaccion`, `PromotorWallet`) y debe ser atomica (RNF-01). El patron del proyecto en casos transaccionales multi-entidad (ver `PromoProgramaService.CreateWithTareasAsync`) es inyectar el `CrowdpromotionContext` directamente en el service y usar transacciones explicitas. Esto es coherente con el patron existente en `InscripcionService` y `PromoProgramaService`.

**`RechazarTareaAsync`:**
1. Cargar `PromoTareaPromotor` por Id
2. Verificar `EstadoTareaId == 2`
3. Actualizar: `EstadoTareaId = 4`, `FechaValidado = now`, `ComentarioValidacion = comentario`
4. `_tareaPromotorRepository.UpdateAsync(entity, ct)` (sin transaccion explicita, operacion simple)
5. Construir y retornar `RechazarTareaResponseDto`

### 4.4 Extension de IPromotorWalletService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

**Estado:** EXISTE - agregar un metodo

El metodo existente `GetWalletEurByPromotorIdAsync` es especifico para EUR. La acreditacion de recompensa necesita obtener la wallet por monedaId variable (el de la tarea).

**Metodo a agregar a la interface:**

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetOrCreateWalletAsync` | `Task<PromotorWallet>` | `PromotorId promotorId, int monedaId, CancellationToken ct` | Busca la wallet del promotor para la moneda indicada. Si no existe, la crea con saldos en cero. Retorna siempre una wallet valida. Implementa FA-07 (wallet autocreacion). |

**Nota:** Este metodo NO debe hacer commit (el commit lo hace la transaccion del `ValidarTareaAsync` en `PromoTareaService`). El repositorio `IPromotorWalletRepository` ya tiene `AddAsync` que hace `SaveChanges`. Para la transaccion atomica de validacion, `PromoTareaService` accede al contexto directamente; `PromotorWalletService.GetOrCreateWalletAsync` se usa solo para el caso de lectura simple fuera del flujo transaccional.

**Alternativa para MVP:** Dado que la logica de buscar-o-crear la wallet es parte del flujo transaccional de validacion, puede implementarse directamente en `PromoTareaService.ValidarTareaAsync` sin necesidad de agregar el metodo a `IPromotorWalletService`. Esta es la opcion recomendada para simplificar. `IPromotorWalletService` queda sin cambios.

**Decision final:** La logica de buscar-o-crear wallet se implementa **dentro de `PromoTareaService.ValidarTareaAsync`** usando `_walletRepository` directamente. `IPromotorWalletService` no cambia. Esto mantiene la atomicidad sin necesidad de coordinar dos servicios.

### 4.5 DTOs de la capa Application

**Nota:** Los DTOs son parte de la capa Application (no Domain ni Infra), pero se documentan aqui porque son el contrato de retorno de los services. Ver `contracts.md` para las definiciones completas.

**Archivos NUEVOS a crear en `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/`:**

| Archivo | Descripcion |
|---------|-------------|
| `MiEstadoTareaDto.cs` | Estado del promotor para una tarea (con campos calculados). Ver contracts.md seccion DTOs. |
| `MisTareasItemDto.cs` | Item del listado de tareas del promotor. |
| `MisTareasResponseDto.cs` | Wrapper del response GET mis-tareas. |
| `CompletarTareaDto.cs` | Request body del POST completar. |
| `CompletarTareaResponseDto.cs` | Response del POST completar. |
| `TareaPendienteItemDto.cs` | Item del listado de tareas pendientes del artista. |
| `TareasPendientesResponseDto.cs` | Wrapper paginado del GET tareas-pendientes. |
| `ValidarTareaDto.cs` | Request body del PATCH validar. |
| `ValidarTareaResponseDto.cs` | Response del PATCH validar. |
| `RechazarTareaDto.cs` | Request body del PATCH rechazar. |
| `RechazarTareaResponseDto.cs` | Response del PATCH rechazar. |

### 4.6 Configuracion EF Core - Cambio Requerido

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

**Estado:** EXISTE - una modificacion menor en la configuracion de `PromoTareaPromotor`

El contrato define que `UrlPruebaCompletado` tiene max **2048 caracteres**, pero la configuracion actual tiene `HasMaxLength(500)`:

```csharp
// ACTUAL (linea en PromoTareaPromotor entity config):
entity.Property(e => e.UrlPruebaCompletado).HasMaxLength(500);

// REQUERIDO:
entity.Property(e => e.UrlPruebaCompletado).HasMaxLength(2048);
```

**Impacto:** Este cambio requiere una migracion de EF Core que altere la columna `UrlPruebaCompletado` en la tabla `PromoTareaPromotor` de `nvarchar(500)` a `nvarchar(2048)`.

**DbSets existentes que ya cubren esta feature (sin agregar nuevos):**

```csharp
public DbSet<PromoTarea> Tareas => Set<PromoTarea>();              // Ya existe
public DbSet<PromoTareaPromotor> TareaPromotores => Set<PromoTareaPromotor>(); // Ya existe
public DbSet<PromotorWallet> Wallets => Set<PromotorWallet>();     // Ya existe
public DbSet<PromotorWalletTransaccion> WalletTransacciones => Set<PromotorWalletTransaccion>(); // Ya existe
```

No se agregan nuevos DbSets al contexto.

### 4.7 Actualizacion de DependencyInjection

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

**Estado:** EXISTE - agregar registros

Registros actuales (no modificar):
```csharp
services.AddScoped<IPromotorRepository, PromotorRepository>();
services.AddScoped<IPromotorWalletRepository, PromotorWalletRepository>();
services.AddScoped<IPromoProgramaRepository, PromoProgramaRepository>();
services.AddScoped<IPromoProgramaPromotorRepository, PromoProgramaPromotorRepository>();
services.AddScoped<IPromotorService, PromotorService>();
services.AddScoped<IPromotorWalletService, PromotorWalletService>();
services.AddScoped<IPromoProgramaService, PromoProgramaService>();
services.AddScoped<IInscripcionService, InscripcionService>();
```

**Registros NUEVOS a agregar:**

```csharp
// Repositories - cp-tareas-promocion (US-CP-04)
services.AddScoped<IPromoTareaRepository, PromoTareaRepository>();
services.AddScoped<IPromoTareaPromotorRepository, PromoTareaPromotorRepository>();

// Services - cp-tareas-promocion (US-CP-04)
services.AddScoped<IPromoTareaService, PromoTareaService>();
```

---

## 5. Queries EF Core Criticas

### 5.1 Query GET mis-tareas - Evitar N+1

**Estrategia:** Cargar todos los completados del promotor en la inscripcion de una sola query, luego hacer la agrupacion en memoria (LINQ to objects).

```
// En PromoTareaService.GetMisTareasAsync:

// Query 1: Tareas activas del programa (lista plana)
List<PromoTarea> tareas = await _tareaRepository.GetTareasByProgramaAsync(programaId, ct)
// -> SELECT * FROM PromoTarea WHERE Programa_Id = @programaId AND EsActivo = 1 ORDER BY Orden

// Query 2: Todos los completados del promotor en esta inscripcion
List<PromoTareaPromotor> completados = await _context.TareaPromotores
    .Where(x => x.ProgramaPromotorId == programaPromotorId)
    .AsNoTracking()
    .ToListAsync(ct)
// -> SELECT * FROM PromoTareaPromotor WHERE ProgramaPromotor_Id = @programaPromotorId

// Agrupacion en memoria (LINQ to objects):
Dictionary<Guid, List<PromoTareaPromotor>> completadosPorTarea = completados
    .GroupBy(x => x.TareaId)
    .ToDictionary(g => g.Key, g => g.ToList());

// Para cada tarea:
// - completadosPorTarea.TryGetValue(tarea.Id, out var registros)
// - VecesCompletada = registros?.Count(x => x.EstadoTareaId != 1) ?? 0
// - FechaPrimeraCompletada = registros?.Min(x => x.FechaCompletado)
// - FechaUltimaCompletada = registros?.Max(x => x.FechaCompletado)
// - ultimoRegistro = registros?.OrderByDescending(x => x.FechaCreacion).FirstOrDefault()
// - MiEstado = ultimoRegistro != null ? new MiEstadoTareaDto { ... } : null
```

**Total queries: 2** (independiente del numero de tareas). Cumple RNF-02.

### 5.2 Query GET tareas-pendientes - Paginacion con COUNT de completados

**Estrategia:** Dos queries. La primera pagina los pendientes con includes. La segunda calcula los conteos de vecesCompletada para los pares (TareaId, ProgramaPromotorId) de esa pagina.

```
// Query 1: Pendientes paginados con includes
var pendientes = await _context.TareaPromotores
    .Where(x => x.EstadoTareaId == 2 && x.ProgramaPromotor.ProgramaId == programaId)
    .Include(x => x.Tarea)
    .Include(x => x.ProgramaPromotor)
        .ThenInclude(pp => pp.Promotor)
    .AsNoTracking()
    .OrderByDescending(x => x.FechaCreacion)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync(ct);

// Query 2: COUNT de completados para los pares de esa pagina
var pares = pendientes.Select(x => new { x.TareaId, x.ProgramaPromotorId }).ToList();
var conteos = await _context.TareaPromotores
    .Where(x => pares.Select(p => p.ProgramaPromotorId).Contains(x.ProgramaPromotorId))
    .GroupBy(x => new { x.TareaId, x.ProgramaPromotorId })
    .Select(g => new { g.Key.TareaId, g.Key.ProgramaPromotorId, Count = g.Count() })
    .ToListAsync(ct);
```

**Total queries: 3** (COUNT total + pagina + conteos). Cumple RNF-03.

### 5.3 Query POST completar - Verificar limites de repeticion

```
// Para tareas NO repetibles:
// Verificar si existe algun registro con EstadoTareaId != 4 (no rechazado)
bool yaCompletada = await _context.TareaPromotores
    .AnyAsync(x => x.TareaId == tareaId
                && x.ProgramaPromotorId == programaPromotorId
                && x.EstadoTareaId != 4, ct);
// Si yaCompletada: retornar error 4027

// Para tareas REPETIBLES con MaxRepeticiones:
int completadosActivos = await _context.TareaPromotores
    .CountAsync(x => x.TareaId == tareaId
                  && x.ProgramaPromotorId == programaPromotorId
                  && (x.EstadoTareaId == 2 || x.EstadoTareaId == 3), ct);
// Si completadosActivos >= tarea.MaxRepeticiones: retornar error 4028
```

### 5.4 Query PATCH validar - Transaccion atomica

```
// Usando transaccion explicita sobre CrowdpromotionContext:

await using var transaction = await _context.Database.BeginTransactionAsync(ct);
try
{
    // 1. Cargar completado con navegaciones necesarias
    var completado = await _context.TareaPromotores
        .Include(x => x.Tarea)
        .Include(x => x.ProgramaPromotor)
        .FirstOrDefaultAsync(x => x.Id == tareaPromotorId, ct);

    // 2. Actualizar estado del completado
    completado.EstadoTareaId = 3;
    completado.FechaValidado = DateTime.UtcNow;
    completado.ComentarioValidacion = comentario;

    // 3. Si hay recompensa monetaria:
    if (completado.Tarea.ImporteRecompensa.HasValue)
    {
        var promotorId = completado.ProgramaPromotor.PromotorId;
        var monedaId = completado.Tarea.MonedaId!.Value;

        // 3a. Buscar o crear wallet
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.PromotorId == promotorId && w.MonedaId == monedaId, ct);

        if (wallet == null)
        {
            wallet = new PromotorWallet
            {
                Id = Guid.NewGuid(),
                PromotorId = promotorId,
                MonedaId = monedaId,
                SaldoDisponible = 0,
                SaldoPendiente = 0,
                TotalGanado = 0,
                TotalRetirado = 0,
                FechaCreacion = DateTime.UtcNow
            };
            await _context.Wallets.AddAsync(wallet, ct);
        }

        // 3b. Crear transaccion de wallet
        var transaccion = new PromotorWalletTransaccion
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            TipoRewardId = completado.Tarea.TipoRewardId,
            EstadoTransaccionId = 1, // Pendiente de liquidacion
            Importe = completado.Tarea.ImporteRecompensa.Value,
            Concepto = $"Recompensa tarea: {completado.Tarea.Titulo}",
            ReferenciaExterna = tareaPromotorId.ToString(),
            FechaCreacion = DateTime.UtcNow
        };
        await _context.WalletTransacciones.AddAsync(transaccion, ct);

        // 3c. Actualizar saldos de la wallet
        wallet.SaldoPendiente += completado.Tarea.ImporteRecompensa.Value;
        wallet.TotalGanado += completado.Tarea.ImporteRecompensa.Value;
        wallet.FechaActualizacion = DateTime.UtcNow;
    }

    await _context.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}
```

---

## 6. Migraciones

### 6.1 Migracion requerida

Solo se requiere una migracion para el cambio en `UrlPruebaCompletado.MaxLength`:

```bash
dotnet ef migrations add ExpandUrlPruebaCompletadoMaxLength \
  --project src/api/WebApi \
  --context CrowdpromotionContext

dotnet ef database update \
  --project src/api/WebApi \
  --context CrowdpromotionContext
```

**Contenido esperado de la migracion:**

```csharp
// Up:
migrationBuilder.AlterColumn<string>(
    name: "UrlPruebaCompletado",
    table: "PromoTareaPromotor",
    type: "nvarchar(2048)",
    maxLength: 2048,
    nullable: true,
    oldClrType: typeof(string),
    oldType: "nvarchar(500)",
    oldMaxLength: 500,
    oldNullable: true);

// Down:
migrationBuilder.AlterColumn<string>(
    name: "UrlPruebaCompletado",
    table: "PromoTareaPromotor",
    type: "nvarchar(500)",
    maxLength: 500,
    nullable: true,
    ...);
```

---

## 7. Archivos a Crear/Modificar

### 7.1 Archivos NUEVOS

```
src/api/Modules/Crowdpromotion/
│
├── WePlayRises.Crowdpromotion.Domain/
│   └── Interfaces/
│       ├── IPromoTareaRepository.cs                    NUEVO
│       └── IPromoTareaPromotorRepository.cs             NUEVO
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Interfaces/
│   │   └── Services/
│   │       └── IPromoTareaService.cs                   NUEVO
│   └── Dtos/
│       ├── MiEstadoTareaDto.cs                         NUEVO
│       ├── MisTareasItemDto.cs                         NUEVO
│       ├── MisTareasResponseDto.cs                     NUEVO
│       ├── CompletarTareaDto.cs                        NUEVO
│       ├── CompletarTareaResponseDto.cs                NUEVO
│       ├── TareaPendienteItemDto.cs                    NUEVO
│       ├── TareasPendientesResponseDto.cs              NUEVO
│       ├── ValidarTareaDto.cs                          NUEVO
│       ├── ValidarTareaResponseDto.cs                  NUEVO
│       ├── RechazarTareaDto.cs                         NUEVO
│       └── RechazarTareaResponseDto.cs                 NUEVO
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Repositories/
    │   ├── PromoTareaRepository.cs                     NUEVO
    │   └── PromoTareaPromotorRepository.cs             NUEVO
    └── Services/
        └── PromoTareaService.cs                        NUEVO
```

### 7.2 Archivos MODIFICADOS

```
src/api/Modules/Crowdpromotion/
│
├── WePlayRises.Crowdpromotion.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs              MODIFICAR: +7 constantes nuevas
│
├── WePlayRises.Crowdpromotion.Infra/
│   ├── Context/
│   │   └── CrowdpromotionContext.cs                   MODIFICAR: UrlPruebaCompletado MaxLength 500->2048
│   └── DependencyInjection.cs                        MODIFICAR: +3 registros AddScoped
```

### 7.3 Archivos SIN cambios

```
WePlayRises.Crowdpromotion.Domain/Model/PromoTareaPromotor.cs   Sin cambios (Estrategia A)
WePlayRises.Crowdpromotion.Domain/Model/PromoTarea.cs           Sin cambios
WePlayRises.Crowdpromotion.Domain/Model/PromoProgramaPromotor.cs Sin cambios
WePlayRises.Crowdpromotion.Domain/Model/PromotorWallet.cs       Sin cambios
WePlayRises.Crowdpromotion.Domain/Model/PromotorWalletTransaccion.cs Sin cambios
WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs Sin cambios
WePlayRises.Crowdpromotion.Infra/Services/PromotorWalletService.cs Sin cambios
```

---

## 8. Reglas de Negocio y donde se verifican

Esta tabla documenta que capa verifica cada regla de negocio de la feature. Las reglas se verifican en los Handlers (capa Application), usando el `IPromoTareaService` para acceder a los datos necesarios.

| Regla | Codigo | Donde se verifica | Como |
|-------|--------|-------------------|------|
| RN-01: Promotor aprobado y no bloqueado | 4026 | Handler `CompletarTareaCommand` | `service.GetInscripcionByPromotorYProgramaAsync` -> verificar `EsAprobado && !EsBloqueado` |
| RN-01: Promotor aprobado GET mis-tareas | 4026 | Handler `GetMisTareasQuery` | Igual que arriba |
| RN-02: Tarea activa | 4029 | Handler `CompletarTareaCommand` | `tarea.EsActivo == false` |
| RN-02: Tarea dentro de fechas | 4030 | Handler `CompletarTareaCommand` | `tarea.FechaFin != null && tarea.FechaFin < DateTime.UtcNow` |
| RN-03: Programa activo | 4024 | Handler `CompletarTareaCommand` | `programa.EsActivo == false` |
| RN-04: Tarea no repetible no completada | 4027 | Handler `CompletarTareaCommand` | `service.CountCompletadosActivosAsync > 0 && !tarea.EsRepetible` |
| RN-05: Limite MaxRepeticiones | 4028 | Handler `CompletarTareaCommand` | `service.CountCompletadosActivosAsync >= tarea.MaxRepeticiones` |
| RN-07: Acreditacion transaccional | - | `PromoTareaService.ValidarTareaAsync` | Transaccion explicita de BD |
| RN-09: Solo artista propietario valida/rechaza | 4026 | Handlers `ValidarTareaCommand` y `RechazarTareaCommand` | `programa.ArtistaId != artistaIdDelToken` |
| RN-10: Comentario obligatorio al rechazar | 1001 | Validator `RechazarTareaCommandValidator` | FluentValidation `.NotEmpty()` |
| AC-CP04-8: Estado Completada para validar | 4031 | `PromoTareaService.ValidarTareaAsync` | `completado.EstadoTareaId != 2` |
| AC-CP04-8: Estado Completada para rechazar | 4031 | `PromoTareaService.RechazarTareaAsync` | `completado.EstadoTareaId != 2` |
| AC-CP04-13: Wallet autocreacion | - | `PromoTareaService.ValidarTareaAsync` | Si wallet == null, crearla dentro de la transaccion |

---

## 9. Checklist de Arquitectura

- [ ] `IPromoTareaRepository.cs` creado en `Domain/Interfaces/`
- [ ] `IPromoTareaPromotorRepository.cs` creado en `Domain/Interfaces/`
- [ ] `ServiceResponseMessageType.cs` actualizado con 7 nuevas constantes (2021, 2022, 4027, 4028, 4029, 4030, 4031)
- [ ] `IPromoTareaService.cs` creado en `Application/Interfaces/Services/`
- [ ] 11 DTOs creados en `Application/Dtos/`
- [ ] `PromoTareaRepository.cs` creado en `Infra/Repositories/`
- [ ] `PromoTareaPromotorRepository.cs` creado en `Infra/Repositories/`
- [ ] `PromoTareaService.cs` creado en `Infra/Services/`
- [ ] `PromoTareaService` inyecta `CrowdpromotionContext` para transacciones en `ValidarTareaAsync`
- [ ] `PromoTareaService` usa `?? throw new ArgumentNullException` en todas las dependencias del constructor
- [ ] `PromoTareaService` usa `IRequestCacheService` para evitar queries duplicados
- [ ] `CrowdpromotionContext.cs` actualizado: `UrlPruebaCompletado.HasMaxLength(2048)`
- [ ] `DependencyInjection.cs` actualizado con 3 nuevos registros `AddScoped`
- [ ] Migracion EF Core para MaxLength de `UrlPruebaCompletado` (500 -> 2048)
- [ ] Entidades `PromoTareaPromotor` y `PromoTarea` sin modificacion de columnas (Estrategia A)
- [ ] Repositories son POCOs de acceso a datos: sin logica de negocio, sin validaciones de estado
- [ ] Services retornan entidades (metodos de acceso a datos) o DTOs (metodos de orquestacion con campos calculados)
- [ ] `IPromotorWalletService` sin cambios (logica de wallet en `PromoTareaService.ValidarTareaAsync`)

---

## 10. Notas de Implementacion para el siguiente agente (CQRS Planning)

### 10.1 Handlers requeridos

El agente de CQRS planning debera crear los siguientes handlers, todos inyectando `IPromoTareaService`:

| Handler | Tipo | Retorna | Endpoint |
|---------|------|---------|----------|
| `GetMisTareasQueryHandler` | Query | `ServiceResponse<MisTareasResponseDto>` | GET `/programas/{programaId}/mis-tareas` |
| `CompletarTareaCommandHandler` | Command | `ServiceResponse<CompletarTareaResponseDto>` | POST `/programas/{programaId}/tareas/{tareaId}/completar` |
| `GetTareasPendientesQueryHandler` | Query | `ServiceResponse<TareasPendientesResponseDto>` | GET `/programas/{programaId}/tareas-pendientes` |
| `ValidarTareaCommandHandler` | Command | `ServiceResponse<ValidarTareaResponseDto>` | PATCH `/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar` |
| `RechazarTareaCommandHandler` | Command | `ServiceResponse<RechazarTareaResponseDto>` | PATCH `/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar` |

### 10.2 Validators requeridos

| Validator | Para |
|-----------|------|
| `CompletarTareaCommandValidator` | UrlPruebaCompletado requerido + formato URL + max 2048; ComentarioPromotor max 500 |
| `ValidarTareaCommandValidator` | ComentarioValidacion opcional max 500 |
| `RechazarTareaCommandValidator` | ComentarioValidacion requerido + max 500 |

### 10.3 Controller

Un nuevo `TareasPromocionController` en `WePlayRises.Crowdpromotion.WebApi/Controllers/` con los 5 endpoints. Los path params `programaId` son de tipo `Guid` y se usan directamente (el contexto de `CrowdpromotionContext` usa `PromoProgramaId` strongly typed, la conversion es en el handler).

### 10.4 AutoMapper Profiles

El agente CQRS debera crear `PromoTareaProfile.cs` en `Application/Mapping/`. Sin embargo, dado que `IPromoTareaService` retorna DTOs directamente en los metodos de orquestacion (`GetMisTareasAsync`, `CompletarTareaAsync`, etc.), el mapping Command->DTO para los requests puede ser minimo o innecesario para estos casos. Los mappers pueden ser Command->Dto para los requests de entrada.
