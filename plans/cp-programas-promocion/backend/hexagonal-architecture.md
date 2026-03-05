# Arquitectura Hexagonal: PromoPrograma

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-programas-promocion (US-CP-02)

---

## 1. Resumen Ejecutivo

Esta feature extiende el modulo Crowdpromotion con la gestion completa de programas de promocion. Los cambios son **aditivos**: se agregan campos a las entidades existentes `PromoPrograma` y `PromoTarea`, se crea un nuevo repository `IPromoProgramaRepository` con sus implementaciones, y se registra `IPromoProgramaService` que orquesta las operaciones transaccionales (crear programa + tareas atomicamente, desactivar programa con cascada de tareas). No se modifica ninguna entidad ni servicio existente del modulo.

---

## 2. Estado Actual vs Cambios Requeridos

### 2.1 Entidades que requieren campos nuevos

| Entidad | Campos actuales relevantes | Campos a agregar |
|---------|---------------------------|------------------|
| `PromoPrograma` | Id, ArtistaId, TipoPromoId, Titulo, Descripcion, MonedaId, ComisionPorConversion, ComisionPorClick, FechaInicio, FechaFin, EsActivo, FechaCreacion, FechaActualizacion | UrlLanding, CodigoTrackingBase, ImporteComisionPorcentaje, ImporteComisionFija |
| `PromoTarea` | Id, ProgramaId, Titulo, Descripcion, InstruccionesUrl, TipoRewardId, ImporteRecompensa, MonedaId, PuntosRecompensa, Orden, EsActivo, FechaCreacion | TipoEventoPromoId, EsRepetible, MaxRepeticiones, FechaInicio, FechaFin |

### 2.2 Lo que NO existe y se debe crear

| Artefacto | Ubicacion |
|-----------|-----------|
| `IPromoProgramaRepository` | Domain/Interfaces |
| `PromoProgramaRepository` | Infra/Repositories |
| `IPromoProgramaService` | Application/Interfaces/Services |
| `PromoProgramaService` | Infra/Services |
| Constantes nuevas en `ServiceResponseMessageType` | Domain/Constants |
| Configuracion Fluent API para campos nuevos | Context/CrowdpromotionContext.cs |
| Registros DI nuevos | Infra/DependencyInjection.cs |

### 2.3 Lo que NO se toca

- `Promotor`, `PromotorService`, `IPromotorRepository`, `PromotorRepository` - sin cambios
- `CrowdpromotionContext` DbSets - ya existen todos los DbSet necesarios
- `PromoPrograma.ComisionPorConversion` y `ComisionPorClick` - coexisten con los campos nuevos segun notas tecnicas del feature-spec

---

## 3. Domain Layer

### 3.1 Entidad PromoPrograma - Campos a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoPrograma.cs`

**Operacion:** MODIFICAR - agregar las siguientes propiedades despues del campo `MonedaId` existente

| Propiedad | Tipo | Nullable | Descripcion | Restriccion |
|-----------|------|----------|-------------|-------------|
| `UrlLanding` | string | Si | URL de la landing page del programa | max 500, formato URL |
| `CodigoTrackingBase` | string | Si | Codigo base para tracking de referidos | max 50, solo [a-zA-Z0-9-], unique por ArtistaId |
| `ImporteComisionPorcentaje` | decimal | Si | Comision en porcentaje por conversion | precision 5,2; rango 0-100 |
| `ImporteComisionFija` | decimal | Si | Comision fija en euros/moneda por conversion | precision 18,2; >= 0 |

**Posicion sugerida en el archivo** (insertar despues de `MonedaId`, antes de `PresupuestoTotal`):
```
public string? UrlLanding { get; set; }
public string? CodigoTrackingBase { get; set; }
public decimal? ImporteComisionPorcentaje { get; set; }
public decimal? ImporteComisionFija { get; set; }
```

**Campos existentes que NO se modifican ni eliminan:**
- `ComisionPorConversion` y `ComisionPorClick` permanecen en la entidad sin cambios
- `PresupuestoTotal` permanece sin cambios

### 3.2 Entidad PromoTarea - Campos a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoTarea.cs`

**Operacion:** MODIFICAR - agregar las siguientes propiedades

| Propiedad | Tipo | Nullable | Descripcion | Restriccion |
|-----------|------|----------|-------------|-------------|
| `TipoEventoPromoId` | int | No | FK a Maestra_TipoEventoPromo | obligatorio, debe existir en maestra (1-6) |
| `EsRepetible` | bool | No | Indica si la tarea puede completarse varias veces | default true |
| `MaxRepeticiones` | int | Si | Numero maximo de repeticiones permitidas | >= 1 cuando EsRepetible = true |
| `FechaInicio` | DateTime | Si | Fecha de inicio de vigencia de la tarea | sin restriccion adicional |
| `FechaFin` | DateTime | Si | Fecha de fin de vigencia de la tarea | debe ser > FechaInicio cuando ambas presentes |

**Posicion sugerida en el archivo** (insertar despues de `ProgramaId`, antes de `Titulo`; o despues de `PuntosRecompensa`):
```
public int TipoEventoPromoId { get; set; }
public bool EsRepetible { get; set; }
public int? MaxRepeticiones { get; set; }
public DateTime? FechaInicio { get; set; }
public DateTime? FechaFin { get; set; }
```

**Nota sobre `TipoEventoPromoId`:** El campo es int (no strongly-typed ID) siguiendo el patron de los campos de maestra existentes (`TipoRewardId`, `MonedaId`). No hay navegacion hacia la tabla de maestras por consistencia con el resto del modulo.

### 3.3 Repository Interface: IPromoProgramaRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoProgramaRepository.cs`

**Operacion:** CREAR nuevo archivo

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `GetByIdAsync` | `Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct)` | Obtener por ID sin includes |
| `GetByIdWithTareasAsync` | `Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct)` | Obtener por ID con Include de Tareas activas |
| `GetByIdWithFullDetailAsync` | `Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct)` | Obtener con Include de Tareas + Promotores (para GET detalle) |
| `GetByArtistaIdPagedAsync` | `Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetByArtistaIdPagedAsync(ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct)` | Listado paginado por artista con filtro opcional de estado |
| `AddAsync` | `Task<PromoProgramaId> AddAsync(PromoPrograma entity, CancellationToken ct)` | Persistir nuevo programa; SaveChanges en el repository |
| `UpdateAsync` | `Task UpdateAsync(PromoPrograma entity, CancellationToken ct)` | Actualizar programa; SaveChanges en el repository |
| `ExistsByCodigoTrackingAndArtistaAsync` | `Task<bool> ExistsByCodigoTrackingAndArtistaAsync(string codigoTracking, ArtistaId artistaId, CancellationToken ct)` | Verificar unicidad de CodigoTrackingBase por artista |
| `ExistsByCodigoTrackingAndArtistaExcludingIdAsync` | `Task<bool> ExistsByCodigoTrackingAndArtistaExcludingIdAsync(string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct)` | Mismo que anterior pero excluyendo el propio ID (para edicion) |

**Nota sobre SaveChanges:** Siguiendo el patron del proyecto confirmado en `PromotorRepository`, el repository hace `SaveChangesAsync` en cada operacion de escritura (`AddAsync`, `UpdateAsync`). Las operaciones multi-entidad transaccionales (crear programa + tareas, desactivar con cascada) usan `Database.BeginTransactionAsync` en el service, igual que `PromotorService.CreateWithWalletAsync`.

**Nota sobre `GetByArtistaIdPagedAsync`:** Retorna una tupla `(Items, TotalCount)` para permitir paginacion sin una segunda query. El repository realiza el `COUNT` y la proyeccion en una sola consulta EF Core usando `Skip/Take`.

### 3.4 ServiceResponseMessageType - Constantes a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Operacion:** MODIFICAR - agregar las constantes siguientes en sus bloques correspondientes

**Bloque Validation (1000-1999) - agregar:**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `Validation_ComisionRequerida` | `"1020"` | Al menos una comision (porcentaje o fija) es obligatoria |
| `Validation_OutOfRange` | `"1021"` | Valor fuera del rango permitido (comision % fuera de 0-100, o comision fija < 0) |
| `Validation_FechaFinAnterior` | `"1022"` | FechaFin debe ser posterior a FechaInicio |
| `Validation_CodigoTrackingInvalidFormat` | `"1025"` | CodigoTrackingBase contiene caracteres no permitidos |
| `Validation_CodigoTrackingDuplicado` | `"1024"` | CodigoTrackingBase ya existe para este artista |
| `Validation_EsRepetibleSinMaxRepeticiones` | `"1026"` | Tarea con EsRepetible=true sin MaxRepeticiones definido |
| `Validation_ImporteRecompensaRequerido` | `"1027"` | ImporteRecompensa requerido para recompensas monetarias (TipoReward 1 o 3) |
| `Validation_PuntosRecompensaRequeridos` | `"1028"` | PuntosRecompensa requeridos para recompensas de puntos (TipoReward 2 o 3) |

**Nota sobre codigos 1022 y 1023 vs feature-spec:** Los contracts.md definen el codigo `1023` para "La fecha fin debe ser posterior a la fecha inicio" y `1022` para otro error. Sin embargo, el contracts.md de la API muestra `1023` para formato de fechas. Segun el contracts-plan.md, el codigo `1022` es "La fecha fin debe ser posterior" y `1023` es "formato general". En el backend, se usa `"1022"` para la validacion de fechas (fechaFin anterior a fechaInicio) y se omite `1023` ya que el formato alfanumerico del tracking usa `1025`.

**Bloque NotFound (2000-2999) - agregar:**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `NotFound_Artista` | `"2016"` | No existe perfil Artista para el UserId del token |
| `NotFound_CampaniaCrowdfunding` | `"2017"` | CampaniaCrowdfundingId indicada no existe |
| `NotFound_ProyectoArtistico` | `"2018"` | ProyectoArtisticoId indicado no existe |
| `NotFound_PromoPrograma` | `"2019"` | PromoPrograma no encontrado por ID |

**Bloque Business Rules (4000-4999) - agregar:**

| Constante | Valor | Uso |
|-----------|-------|-----|
| `BusinessRule_PromoProgramaAlreadyInactive` | `"4020"` | Intento de desactivar programa ya inactivo |
| `BusinessRule_PromoTareaConCompletados` | `"4021"` | No se puede desactivar tarea con registros de completados |

**Estado completo del archivo tras los cambios:**

```
// Success (0000-0999)
Success = "0000"
Created = "0001"
Updated = "0002"
Deleted = "0003"

// Validation (1000-1999)
Validation_Required = "1001"
Validation_MaxLength = "1002"
Validation_InvalidEmail = "1003"
Validation_ForeignKeyNotFound = "1010"
Validation_MinLength = "1011"
Validation_InvalidUrl = "1013"
Validation_ComisionRequerida = "1020"        [NUEVO]
Validation_OutOfRange = "1021"               [NUEVO]
Validation_FechaFinAnterior = "1022"         [NUEVO]
Validation_CodigoTrackingDuplicado = "1024"  [NUEVO]
Validation_CodigoTrackingInvalidFormat = "1025" [NUEVO]
Validation_EsRepetibleSinMaxRepeticiones = "1026" [NUEVO]
Validation_ImporteRecompensaRequerido = "1027" [NUEVO]
Validation_PuntosRecompensaRequeridos = "1028"  [NUEVO]

// NotFound (2000-2999)
NotFound_Entity = "2000"
NotFound_Promotor = "2015"
NotFound_Artista = "2016"              [NUEVO]
NotFound_CampaniaCrowdfunding = "2017" [NUEVO]
NotFound_ProyectoArtistico = "2018"    [NUEVO]
NotFound_PromoPrograma = "2019"        [NUEVO]

// Auth (3000-3999)
Auth_Unauthorized = "3001"
Auth_Forbidden = "3002"
Auth_InvalidToken = "3004"

// Business Rules (4000-4999)
BusinessRule_PromotorAlreadyExists = "4018"
BusinessRule_PromotorAlreadyInactive = "4019"
BusinessRule_PromoProgramaAlreadyInactive = "4020"  [NUEVO]
BusinessRule_PromoTareaConCompletados = "4021"       [NUEVO]

// Internal (5000-5999)
Internal_UnexpectedError = "5000"
```

---

## 4. Infrastructure Layer

### 4.1 Repository Implementation: PromoProgramaRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoProgramaRepository.cs`

**Operacion:** CREAR nuevo archivo

- Implementa: `IPromoProgramaRepository`
- Inyecta: `CrowdpromotionContext`
- Constructor: `CrowdpromotionContext context` con `?? throw`

| Metodo | Comportamiento |
|--------|---------------|
| `GetByIdAsync` | `_context.Programas.FirstOrDefaultAsync(x => x.Id == id, ct)` - sin includes, con tracking para update |
| `GetByIdWithTareasAsync` | `Include(p => p.Tareas).Where(t => t.EsActivo)` - tareas activas para edicion |
| `GetByIdWithFullDetailAsync` | `Include(p => p.Tareas).Include(p => p.Promotores)` - para GET detalle completo |
| `GetByArtistaIdPagedAsync` | Query con `Where(x => x.ArtistaId == artistaId)`, filtro opcional `esActivo`, `OrderByDescending(x => x.FechaCreacion)`, `Skip((page-1)*pageSize).Take(pageSize)`. Ejecutar `CountAsync()` y `ToListAsync()` por separado o via `GroupBy` para TotalCount |
| `AddAsync` | `_context.Programas.AddAsync(entity, ct)` + `_context.SaveChangesAsync(ct)` + return `entity.Id` |
| `UpdateAsync` | `_context.Programas.Update(entity)` + `_context.SaveChangesAsync(ct)` |
| `ExistsByCodigoTrackingAndArtistaAsync` | `AnyAsync(x => x.CodigoTrackingBase == codigoTracking && x.ArtistaId == artistaId, ct)` |
| `ExistsByCodigoTrackingAndArtistaExcludingIdAsync` | `AnyAsync(x => x.CodigoTrackingBase == codigoTracking && x.ArtistaId == artistaId && x.Id != excludeId, ct)` |

**Nota de eficiencia en GetByArtistaIdPagedAsync:** Para evitar dos round-trips a la base de datos, el repository puede ejecutar el count total y la pagina actual en una sola consulta usando `var query = _context.Programas.Where(...)`. Luego `totalCount = await query.CountAsync(ct)` y `items = await query.Skip(...).Take(...).ToListAsync(ct)`. El conteo de promotores y tareas activas (`numeroPromotores`, `numeroTareas`) se calculan en el **service** via projections del objeto cargado, o via queries adicionales especificas. Ver seccion 4.2 para el detalle del service.

### 4.2 Service Interface: IPromoProgramaService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoProgramaService.cs`

**Operacion:** CREAR nuevo archivo

**Nota de ubicacion:** Las interfaces de service siguen el patron del modulo: estan en `Application/Interfaces/Services/` (no en Domain), igual que `IPromotorService`.

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `GetByIdAsync` | `Task<PromoPrograma?> GetByIdAsync(PromoProgramaId id, CancellationToken ct)` | Obtener programa sin tareas (para validaciones) |
| `GetByIdWithTareasAsync` | `Task<PromoPrograma?> GetByIdWithTareasAsync(PromoProgramaId id, CancellationToken ct)` | Obtener programa con tareas activas |
| `GetByIdWithFullDetailAsync` | `Task<PromoPrograma?> GetByIdWithFullDetailAsync(PromoProgramaId id, CancellationToken ct)` | Obtener detalle completo para endpoint GET /{id} |
| `GetMisProgramasAsync` | `Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetMisProgramasAsync(ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct)` | Listado paginado de programas del artista |
| `ExisteCodigoTrackingAsync` | `Task<bool> ExisteCodigoTrackingAsync(string codigoTracking, ArtistaId artistaId, CancellationToken ct)` | Verificar unicidad para creacion (validator) |
| `ExisteCodigoTrackingParaEdicionAsync` | `Task<bool> ExisteCodigoTrackingParaEdicionAsync(string codigoTracking, ArtistaId artistaId, PromoProgramaId excludeId, CancellationToken ct)` | Verificar unicidad excluyendo el propio programa (validator de update) |
| `CreateWithTareasAsync` | `Task<PromoProgramaId> CreateWithTareasAsync(PromoPrograma programa, IReadOnlyList<PromoTarea> tareas, CancellationToken ct)` | Crear programa y tareas en una transaccion unica |
| `UpdateAsync` | `Task UpdateAsync(PromoPrograma programa, CancellationToken ct)` | Actualizar solo los campos del programa (sin tareas) |
| `UpdateWithTareasAsync` | `Task UpdateWithTareasAsync(PromoPrograma programa, IReadOnlyList<PromoTarea> tareasNuevas, IReadOnlyList<PromoTarea> tareasModificadas, CancellationToken ct)` | Actualizar programa y sus tareas en una transaccion |
| `DesactivarWithTareasAsync` | `Task<int> DesactivarWithTareasAsync(PromoProgramaId id, CancellationToken ct)` | Desactivar programa y todas sus tareas activas; retorna count de tareas desactivadas |
| `TipoPromoExistsAsync` | `Task<bool> TipoPromoExistsAsync(int tipoPromoId, CancellationToken ct)` | Validar existencia de tipo de programa en maestra (seed data) |
| `TipoEventoPromoExistsAsync` | `Task<bool> TipoEventoPromoExistsAsync(int tipoEventoPromoId, CancellationToken ct)` | Validar existencia de tipo de evento en maestra (seed data) |

**Notas sobre los metodos:**

- `CreateWithTareasAsync` recibe las entidades ya construidas por el handler via AutoMapper. Internamente inicia una transaccion, hace `AddAsync` del programa, luego `AddRangeAsync` de las tareas via context directo (no hay repositorio separado para PromoTarea), y hace commit. Si falla cualquier paso, hace rollback.

- `DesactivarWithTareasAsync` carga el programa con `GetByIdWithTareasAsync`, establece `EsActivo = false` en el programa y en cada tarea activa, y persiste todo en una transaccion.

- `TipoPromoExistsAsync` y `TipoEventoPromoExistsAsync` pueden validar contra un `HashSet<int>` estatico (igual que `ValidTipoPromotorIds` en `PromotorService`) o via query al contexto. Se recomienda el HashSet estatico para evitar queries a tablas de maestras.

### 4.3 Service Implementation: PromoProgramaService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromoProgramaService.cs`

**Operacion:** CREAR nuevo archivo

**Dependencias inyectadas:**

| Dependencia | Tipo | Proposito |
|-------------|------|-----------|
| `_repository` | `IPromoProgramaRepository` | Operaciones CRUD sobre PromoPrograma |
| `_requestCache` | `IRequestCacheService` | Cache per-request para evitar queries duplicados |
| `_logger` | `ILogger<PromoProgramaService>` | Logging de operaciones |
| `_context` | `CrowdpromotionContext` | Acceso directo para transacciones y PromoTarea (sin repo separado) |

**Patron de constructor:** Todos los parametros con `?? throw new ArgumentNullException(nameof(...))`, siguiendo el patron de `PromotorService`.

**Conjuntos estaticos para validacion de maestras:**

```
private static readonly HashSet<int> ValidTipoPromoIds = new() { 1, 2, 3, 4 };
private static readonly HashSet<int> ValidTipoEventoPromoIds = new() { 1, 2, 3, 4, 5, 6 };
```

**Comportamiento por metodo:**

| Metodo | Comportamiento |
|--------|---------------|
| `GetByIdAsync` | `_requestCache.GetOrAddAsync("promo-programa:{id}", () => _repository.GetByIdAsync(id, ct))` |
| `GetByIdWithTareasAsync` | `_requestCache.GetOrAddAsync("promo-programa:tareas:{id}", () => _repository.GetByIdWithTareasAsync(id, ct))` |
| `GetByIdWithFullDetailAsync` | Sin cache (query compleja con promotores; no se repite en el mismo request) - llama directamente a `_repository.GetByIdWithFullDetailAsync` |
| `GetMisProgramasAsync` | Sin cache de request (resultados variables por filtros) - delega a `_repository.GetByArtistaIdPagedAsync` |
| `ExisteCodigoTrackingAsync` | `_requestCache.GetOrAddAsync("promo-programa:codigo:{codigoTracking}:{artistaId}", () => _repository.ExistsByCodigoTrackingAndArtistaAsync(...))` |
| `ExisteCodigoTrackingParaEdicionAsync` | Delega directamente a `_repository.ExistsByCodigoTrackingAndArtistaExcludingIdAsync` (sin cache, llamada puntual en edicion) |
| `CreateWithTareasAsync` | `await using var transaction = await _context.Database.BeginTransactionAsync(ct)` -> `_repository.AddAsync(programa, ct)` -> asignar `ProgramaId` a cada tarea -> `_context.Tareas.AddRangeAsync(tareas, ct)` -> `_context.SaveChangesAsync(ct)` -> `transaction.CommitAsync(ct)`. Rollback en catch. Log INFO con ID creado y count de tareas. |
| `UpdateAsync` | `programa.FechaActualizacion = DateTime.UtcNow` -> `_repository.UpdateAsync(programa, ct)`. Log INFO. |
| `UpdateWithTareasAsync` | Transaccion: update programa + update tareas modificadas + add tareas nuevas. `FechaActualizacion = DateTime.UtcNow` en programa. |
| `DesactivarWithTareasAsync` | Transaccion: cargar programa con tareas via `GetByIdWithTareasAsync` -> `programa.EsActivo = false` -> `programa.FechaActualizacion = DateTime.UtcNow` -> `foreach tarea where EsActivo: tarea.EsActivo = false` -> `_repository.UpdateAsync(programa, ct)` -> `_context.SaveChangesAsync(ct)` (para tareas) -> commit. Retorna count de tareas desactivadas. |
| `TipoPromoExistsAsync` | `Task.FromResult(ValidTipoPromoIds.Contains(tipoPromoId))` |
| `TipoEventoPromoExistsAsync` | `Task.FromResult(ValidTipoEventoPromoIds.Contains(tipoEventoPromoId))` |

**Nota sobre PromoTarea sin repositorio propio:** La entidad `PromoTarea` se gestiona directamente a traves de `_context.Tareas` dentro del service, en operaciones transaccionales. No se crea `IPromoTareaRepository` porque las tareas siempre se manipulan en el contexto de su programa padre y nunca de forma independiente.

### 4.4 Configuracion Fluent API - Campos Nuevos

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

**Operacion:** MODIFICAR - agregar configuraciones dentro del bloque `modelBuilder.Entity<PromoPrograma>` y `modelBuilder.Entity<PromoTarea>` existentes

#### PromoPrograma - Configuraciones a agregar

Dentro del bloque `entity =>` de `PromoPrograma`, agregar despues de la configuracion de `ComisionPorClick`:

| Propiedad | Configuracion Fluent API |
|-----------|--------------------------|
| `UrlLanding` | `entity.Property(e => e.UrlLanding).HasMaxLength(500)` |
| `CodigoTrackingBase` | `entity.Property(e => e.CodigoTrackingBase).HasMaxLength(50)` |
| `ImporteComisionPorcentaje` | `entity.Property(e => e.ImporteComisionPorcentaje).HasColumnType("decimal(5, 2)")` |
| `ImporteComisionFija` | `entity.Property(e => e.ImporteComisionFija).HasColumnType("decimal(18, 2)")` |

**Indice unico para CodigoTrackingBase por artista** (agregar despues del indice existente `IX_PromoPrograma_Artista_Activo`):

```
entity.HasIndex(e => new { e.ArtistaId, e.CodigoTrackingBase })
    .IsUnique()
    .HasFilter("[CodigoTrackingBase] IS NOT NULL")
    .HasDatabaseName("IX_PromoPrograma_Artista_CodigoTracking");
```

**Nota sobre el indice filtrado:** Se usa `HasFilter` con la condicion `IS NOT NULL` para que el indice unico solo aplique a los programas que tienen codigo de tracking definido. Esto permite que multiples programas del mismo artista tengan `CodigoTrackingBase = NULL` sin violar la unicidad.

#### PromoTarea - Configuraciones a agregar

Dentro del bloque `entity =>` de `PromoTarea`, agregar despues de la configuracion de `ProgramaId`:

| Propiedad | Configuracion Fluent API |
|-----------|--------------------------|
| `TipoEventoPromoId` | `entity.Property(e => e.TipoEventoPromoId).HasColumnName("TipoEventoPromo_Id").IsRequired()` |
| `EsRepetible` | `entity.Property(e => e.EsRepetible).HasDefaultValue(true)` |
| `MaxRepeticiones` | `entity.Property(e => e.MaxRepeticiones)` (sin configuracion especial; es int? nullable) |
| `FechaInicio` | `entity.Property(e => e.FechaInicio).HasPrecision(3)` |
| `FechaFin` | `entity.Property(e => e.FechaFin).HasPrecision(3)` |

**Nota sobre `TipoEventoPromoId`:** Se nombra la columna con el patron `TipoEventoPromo_Id` consistente con el naming de FKs de maestras en el modulo (`TipoReward_Id`, `Moneda_Id`, `TipoPromo_Id`).

**Nota sobre `EsRepetible` con `HasDefaultValue`:** El valor `true` se configura como default a nivel de base de datos. En C# la propiedad se inicializa en la entidad con el valor del campo (sin inicializador explicito; el handler asigna el valor desde el command).

### 4.5 DependencyInjection.cs - Registros a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

**Operacion:** MODIFICAR - agregar los registros nuevos dentro de `AddCrowdpromotionServices`

```
// Repositories (agregar)
services.AddScoped<IPromoProgramaRepository, PromoProgramaRepository>();

// Services (agregar)
services.AddScoped<IPromoProgramaService, PromoProgramaService>();
```

**Using statements a agregar:**
```
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;  // ya existe
// Agregar:
// IPromoProgramaService se agrega en el mismo namespace Application.Interfaces.Services
// IPromoProgramaRepository se agrega en Domain.Interfaces (namespace ya importado via IPromotorRepository)
```

---

## 5. Migracion EF Core

### 5.1 Nombre de la migracion

```
AddPromoProgramaAndPromoTareaCamposUS_CP_02
```

### 5.2 Comando

```bash
dotnet ef migrations add AddPromoProgramaAndPromoTareaCamposUS_CP_02 --project src/api/WebApi --context CrowdpromotionContext
dotnet ef database update --project src/api/WebApi --context CrowdpromotionContext
```

**Nota:** El comando requiere `--context CrowdpromotionContext` ya que el proyecto tiene multiples DbContext. Si hay un startup project separado del migrations project, usar `--startup-project src/api/WebApi`.

### 5.3 Cambios esperados en la migracion generada

**Tabla `PromoPrograma` - columnas ADD:**

| Columna | Tipo SQL | Nullable | Default |
|---------|----------|----------|---------|
| `UrlLanding` | `nvarchar(500)` | YES | NULL |
| `CodigoTrackingBase` | `nvarchar(50)` | YES | NULL |
| `ImporteComisionPorcentaje` | `decimal(5,2)` | YES | NULL |
| `ImporteComisionFija` | `decimal(18,2)` | YES | NULL |

**Tabla `PromoPrograma` - indices ADD:**

| Nombre | Columnas | Tipo |
|--------|----------|------|
| `IX_PromoPrograma_Artista_CodigoTracking` | `Artista_Id`, `CodigoTrackingBase` | UNIQUE FILTERED (WHERE CodigoTrackingBase IS NOT NULL) |

**Tabla `PromoTarea` - columnas ADD:**

| Columna | Tipo SQL | Nullable | Default |
|---------|----------|----------|---------|
| `TipoEventoPromo_Id` | `int` | NO | (obligatorio; migracion asignara valor default 1 para rows existentes) |
| `EsRepetible` | `bit` | NO | `1` (true) |
| `MaxRepeticiones` | `int` | YES | NULL |
| `FechaInicio` | `datetime2(3)` | YES | NULL |
| `FechaFin` | `datetime2(3)` | YES | NULL |

**IMPORTANTE sobre `TipoEventoPromo_Id` NOT NULL en tabla con datos existentes:** Si existen filas en `PromoTarea`, EF Core generara un error al aplicar la migracion porque la nueva columna es NOT NULL sin default. La migracion debe incluir un valor por defecto para la columna al agregarla. Opciones:

- **Opcion A (recomendada):** Editar manualmente el script de migracion generado para agregar la columna como NOT NULL con `defaultValue: 1` (Click = tipo de evento mas generico).
- **Opcion B:** Configurar en Fluent API `entity.Property(e => e.TipoEventoPromoId).HasDefaultValue(1)` solo para la migracion, y remover el default despues.

Se recomienda la Opcion A: en el archivo `_Migration.cs` generado, cambiar el `AddColumn` para incluir `defaultValue: 1` o hacerlo nullable temporalmente, migrar, y dejar la restriccion NOT NULL a nivel de la entidad.

### 5.4 Verificacion post-migracion

```sql
-- Verificar columnas nuevas en PromoPrograma
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PromoPrograma'
  AND COLUMN_NAME IN ('UrlLanding', 'CodigoTrackingBase', 'ImporteComisionPorcentaje', 'ImporteComisionFija');

-- Verificar columnas nuevas en PromoTarea
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'PromoTarea'
  AND COLUMN_NAME IN ('TipoEventoPromo_Id', 'EsRepetible', 'MaxRepeticiones', 'FechaInicio', 'FechaFin');

-- Verificar indice unico con filtro
SELECT i.name, i.is_unique, i.filter_definition
FROM sys.indexes i
WHERE i.name = 'IX_PromoPrograma_Artista_CodigoTracking';
```

---

## 6. Resumen de Archivos a Crear y Modificar

### 6.1 Archivos a MODIFICAR

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Model/
│   │   ├── PromoPrograma.cs          <- MODIFICAR: agregar 4 propiedades
│   │   └── PromoTarea.cs             <- MODIFICAR: agregar 5 propiedades
│   └── Constants/
│       └── ServiceResponseMessageType.cs  <- MODIFICAR: agregar 12 constantes
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Context/
    │   └── CrowdpromotionContext.cs   <- MODIFICAR: 6 configs PromoPrograma + 5 configs PromoTarea + 1 indice
    └── DependencyInjection.cs         <- MODIFICAR: 2 registros nuevos
```

### 6.2 Archivos a CREAR

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   └── Interfaces/
│       └── IPromoProgramaRepository.cs   <- CREAR: 8 metodos
│
├── WePlayRises.Crowdpromotion.Application/
│   └── Interfaces/
│       └── Services/
│           └── IPromoProgramaService.cs  <- CREAR: 12 metodos
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Repositories/
    │   └── PromoProgramaRepository.cs   <- CREAR: implementa IPromoProgramaRepository
    └── Services/
        └── PromoProgramaService.cs      <- CREAR: implementa IPromoProgramaService
```

---

## 7. Decisiones Arquitectonicas

### 7.1 No se crea IPromoTareaRepository

Las `PromoTarea` se gestionan siempre en el contexto de su `PromoPrograma` padre. El `PromoProgramaService` accede directamente a `_context.Tareas` para operaciones de escritura en las tareas (dentro de transacciones que ya usan el contexto). Esto es consistente con el patron del modulo: `PromotorService` accede a `_context.ProgramaPromotores.UpdateRange(...)` directamente sin un repositorio separado para `PromoProgramaPromotor`.

### 7.2 Transacciones con Database.BeginTransactionAsync

El modulo Crowdpromotion usa `_context.Database.BeginTransactionAsync` para transacciones multi-entidad, sin `IUnitOfWork`. Esto es el patron establecido en `PromotorService.CreateWithWalletAsync` y `PromotorService.DesactivarWithProgramasAsync`. El `PromoProgramaService` sigue el mismo patron en `CreateWithTareasAsync` y `DesactivarWithTareasAsync`.

### 7.3 Cache per-request para operaciones de lectura

El validator de `CreatePromoProgramaCommand` llamara a `IPromoProgramaService.ExisteCodigoTrackingAsync` para verificar la unicidad del codigo de tracking. Si el handler tambien necesita verificar la existencia del programa (para edicion), la segunda llamada sera atendida por el cache sin ir a la base de datos. Esta es la implementacion del patron ADR-006 del proyecto.

### 7.4 Campos de comision coexistentes

`ComisionPorConversion` y `ComisionPorClick` (existentes) coexisten con `ImporteComisionFija` e `ImporteComisionPorcentaje` (nuevos). Los campos nuevos tienen semantica claramente definida en la US. Los campos existentes se mantienen sin modificar para no romper compatibilidad. Los handlers CQRS de esta feature solo leen y escriben los campos nuevos.

### 7.5 MonedaId en PromoPrograma pasa a ser obligatorio

El contrato de la API define `monedaId` como campo obligatorio en el request. Sin embargo, en la entidad existente `MonedaId` es `int?`. El handler CQRS debe asignar el valor del command a `MonedaId` en la entidad. La entidad no cambia su tipo (se mantiene `int?` para no romper registros existentes sin moneda). La validacion de obligatoriedad se hace en el validator.

---

## 8. Dependencias entre Artefactos

```
ServiceResponseMessageType.cs
    |
    +-- IPromoProgramaRepository.cs   (usa PromoProgramaId, ArtistaId de StronglyTypedIds)
    |       |
    |       +-- PromoProgramaRepository.cs  (inyecta CrowdpromotionContext)
    |
    +-- IPromoProgramaService.cs      (usa PromoProgramaId, ArtistaId; retorna entidades)
            |
            +-- PromoProgramaService.cs  (inyecta IPromoProgramaRepository + IRequestCacheService +
                                          ILogger + CrowdpromotionContext)
                    |
                    +-- DependencyInjection.cs (registra ambos)

PromoPrograma.cs + PromoTarea.cs (modificados)
    |
    +-- CrowdpromotionContext.cs (configuraciones Fluent API)
    |
    +-- Migracion EF Core (genera SQL)
```

---

## 9. Checklist

- [ ] Entidades son POCOs (sin metodos de negocio)
- [ ] `PromoPrograma.cs`: 4 propiedades nuevas agregadas (UrlLanding, CodigoTrackingBase, ImporteComisionPorcentaje, ImporteComisionFija)
- [ ] `PromoTarea.cs`: 5 propiedades nuevas agregadas (TipoEventoPromoId, EsRepetible, MaxRepeticiones, FechaInicio, FechaFin)
- [ ] `ServiceResponseMessageType.cs`: 12 constantes nuevas (8 validation + 4 notfound/businessrule)
- [ ] `IPromoProgramaRepository.cs` creado en Domain/Interfaces con 8 metodos
- [ ] `PromoProgramaRepository.cs` creado con `?? throw` en constructor, SaveChanges en escrituras
- [ ] `IPromoProgramaService.cs` creado en Application/Interfaces/Services con 12 metodos
- [ ] `PromoProgramaService.cs` creado con todas las dependencias + `?? throw` en constructor
- [ ] `PromoProgramaService` inyecta `CrowdpromotionContext` directamente (patron del modulo)
- [ ] `PromoProgramaService.CreateWithTareasAsync` usa transaccion `BeginTransactionAsync`
- [ ] `PromoProgramaService.DesactivarWithTareasAsync` usa transaccion `BeginTransactionAsync`
- [ ] `PromoProgramaService` usa `IRequestCacheService` para GetById y ExisteCodigo
- [ ] `CrowdpromotionContext.cs` configuraciones Fluent API para 9 campos nuevos
- [ ] Indice unico filtrado `IX_PromoPrograma_Artista_CodigoTracking` configurado con `HasFilter`
- [ ] `DependencyInjection.cs` registra `IPromoProgramaRepository` y `IPromoProgramaService`
- [ ] Migracion EF Core planificada con nombre descriptivo
- [ ] Estrategia para `TipoEventoPromo_Id NOT NULL` en tabla con datos existentes documentada
- [ ] Sin cambios en `PromotorService`, `PromotorRepository`, `IPromotorService`, `IPromotorRepository`
- [ ] Sin nuevo DbSet en CrowdpromotionContext (todos los DbSet existentes son suficientes)
