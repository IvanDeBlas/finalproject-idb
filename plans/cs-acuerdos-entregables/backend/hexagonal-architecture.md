# Arquitectura Hexagonal: Acuerdos, Milestones y Entregables

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Bounded Context:** CrowdsourcingContext

---

## 1. Resumen Ejecutivo

Esta arquitectura define las capas Domain e Infrastructure para la feature "Acuerdos, Milestones y Entregables". El sistema formaliza el ciclo de vida completo de un acuerdo de trabajo entre artista y profesional: desde la aceptacion transaccional de una propuesta (con 6 efectos secundarios atomicos), la organizacion del trabajo en milestones con importes parciales, la entrega y revision de entregables con URLs externas, hasta el cierre del acuerdo por completado o cancelacion. La operacion mas critica es `AceptarPropuesta`, que crea el acuerdo, rechaza propuestas competidoras, avanza el estado de la necesidad y genera una conversacion vinculada, todo en una sola transaccion de base de datos.

**Capacidades principales:**
- Crear acuerdo de forma transaccional al aceptar propuesta (6 efectos en 1 transaccion)
- Rechazar propuesta individual de forma independiente
- CRUD de milestones con validacion de suma de importes
- Subir, aprobar y rechazar entregables con URLs externas
- Completar o cancelar acuerdo con transicion de estado de la necesidad vinculada
- Caching con `IRequestCacheService` para compartir datos entre Validator y Handler

---

## 2. Estado Actual del Codigo

### 2.1 Entidades que ya existen (INCOMPLETAS - requieren modificacion)

Las siguientes entidades existen en el repositorio pero les faltan campos definidos en feature-spec.md y contracts.md:

| Entidad | Archivo | Estado |
|---------|---------|--------|
| `AcuerdoCrowdsourcing` | `Domain/Model/AcuerdoCrowdsourcing.cs` | Existe. Faltan: `MotivoCancelacion`, `CanceladoPor`. Tiene campos extra no requeridos en MVP: `Descripcion`, `ImporteAnticipo`, `PorcentajeAnticipo`. |
| `AcuerdoCrowdsourcingMilestone` | `Domain/Model/AcuerdoCrowdsourcingMilestone.cs` | Existe. Faltan: coleccion `Entregables`. Tiene `PorcentajeParcial` como columna (debe ser calculado, no persistido). |
| `AcuerdoCrowdsourcingEntregable` | `Domain/Model/AcuerdoCrowdsourcingEntregable.cs` | Existe. Faltan: `ComentarioRechazo`, `FechaActualizacion`. Tiene `FechaEntrega` que el spec no menciona. |
| `PropuestaCrowdsourcing` | `Domain/Model/PropuestaCrowdsourcing.cs` | Existe. Falta: campo `AcuerdoId` nullable (FK hacia el acuerdo creado). |

### 2.2 Configuraciones EF que ya existen (INCOMPLETAS)

El `CrowdsourcingContext` ya tiene inline configurations para AcuerdoCrowdsourcing, AcuerdoCrowdsourcingMilestone y AcuerdoCrowdsourcingEntregable, pero les faltan las columnas nuevas y algunas relaciones.

### 2.3 Repositorios e Interfaces que NO existen (a crear)

- `IAcuerdoCrowdsourcingRepository` - NO existe
- `IAcuerdoCrowdsourcingMilestoneRepository` - NO existe
- `IAcuerdoCrowdsourcingEntregableRepository` - NO existe
- `IConversacionCrowdsourcingRepository` - NO existe (necesario para crear conversacion al aceptar propuesta)
- Implementaciones de todos los anteriores - NO existen

### 2.4 Services e Interfaces que NO existen (a crear)

- `IAcuerdoCrowdsourcingService` / `AcuerdoCrowdsourcingService` - NO existen
- `IAcuerdoCrowdsourcingMilestoneService` / `AcuerdoCrowdsourcingMilestoneService` - NO existen
- `IAcuerdoCrowdsourcingEntregableService` / `AcuerdoCrowdsourcingEntregableService` - NO existen

### 2.5 DbSets ya registrados en CrowdsourcingContext

Los DbSets `Acuerdos`, `Milestones` y `Entregables` ya existen en el contexto. Solo se necesita agregar columnas nuevas via migracion.

---

## 3. Domain Layer

### 3.1 Entidades (Modificaciones a existentes)

#### AcuerdoCrowdsourcing (MODIFICACION)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/AcuerdoCrowdsourcing.cs`

**Cambios requeridos:**
- Agregar campo `MotivoCancelacion` (string?, nullable, min 20 max 1000 al cancelar)
- Agregar campo `CanceladoPor` (string?, nullable, UserId de quien cancelo)

| Propiedad | Tipo | Nullable | Descripcion | Estado |
|-----------|------|----------|-------------|--------|
| Id | AcuerdoCrowdsourcingId | No | PK (Strongly Typed ID) | Existe |
| NecesidadId | NecesidadCrowdsourcingId | No | FK a NecesidadCrowdsourcing | Existe |
| PropuestaId | PropuestaCrowdsourcingId? | Si | FK a PropuestaCrowdsourcing (nullable por SetNull) | Existe |
| ArtistaId | ArtistaId | No | FK a Artista (modulo UserAccess) | Existe |
| UserIdProveedor | string | No | UserId del profesional (Identity User, max 450) | Existe |
| PerfilProfesionalId | PerfilProfesionalId? | Si | FK a PerfilProfesional (modulo UserAccess) | Existe |
| TituloInterno | string? | Si | Titulo interno del acuerdo (max 200) | Existe |
| EstadoAcuerdoId | int | No | FK a MaestraEstadoAcuerdo (1=Activo, 2=Completado, 3=Cancelado) | Existe |
| MonedaId | int? | Si | FK a MaestraMoneda | Existe |
| ImporteTotalPactado | decimal | No | Importe total pactado (del precio propuesto) | Existe |
| FechaInicio | DateTime? | Si | Fecha inicio del trabajo | Existe |
| FechaFinPrevista | DateTime? | Si | Fecha fin prevista | Existe |
| FechaFinReal | DateTime? | Si | Fecha real de fin (completado o cancelado) | Existe |
| **MotivoCancelacion** | **string?** | **Si** | **Motivo obligatorio al cancelar (min 20, max 1000)** | **NUEVO** |
| **CanceladoPor** | **string?** | **Si** | **UserId de quien ejecuto la cancelacion** | **NUEVO** |
| FechaCreacion | DateTime | No | Timestamp de creacion UTC | Existe |
| FechaActualizacion | DateTime? | Si | Ultima actualizacion UTC | Existe |

**Navegaciones:**
- `Necesidad` -> `NecesidadCrowdsourcing` (N:1)
- `Propuesta` -> `PropuestaCrowdsourcing?` (N:1, nullable)
- `Milestones` -> `ICollection<AcuerdoCrowdsourcingMilestone>` (1:N) - Existe
- `Entregables` -> `ICollection<AcuerdoCrowdsourcingEntregable>` (1:N) - Existe
- `Conversaciones` -> `ICollection<ConversacionCrowdsourcing>` (1:N) - Existe
- `Valoraciones` -> `ICollection<ValoracionCrowdsourcing>` (1:N) - Existe

**Nota:** Los campos `Descripcion`, `ImporteAnticipo`, `PorcentajeAnticipo` ya existen en la entidad. No se deben eliminar para no romper la migracion existente. El plan de esta feature no los usa pero tampoco los elimina.

---

#### AcuerdoCrowdsourcingMilestone (MODIFICACION)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/AcuerdoCrowdsourcingMilestone.cs`

**Cambios requeridos:**
- Agregar coleccion de navegacion `Entregables`
- El campo `PorcentajeParcial` ya existe como columna; en la implementacion se calculara en memoria (no es necesario persistirlo, pero la columna ya existe en BD y no se elimina)

| Propiedad | Tipo | Nullable | Descripcion | Estado |
|-----------|------|----------|-------------|--------|
| Id | Guid | No | PK | Existe |
| AcuerdoId | AcuerdoCrowdsourcingId | No | FK a AcuerdoCrowdsourcing | Existe |
| Titulo | string | No | Titulo del milestone (min 3, max 200) | Existe |
| Descripcion | string? | Si | Descripcion (max 1000) | Existe |
| Orden | int | No | Orden secuencial dentro del acuerdo | Existe |
| ImporteParcial | decimal | No | Importe parcial asignado a este milestone | Existe |
| PorcentajeParcial | decimal? | Si | Porcentaje calculado (columna existente, se calcula en handler) | Existe |
| FechaLimite | DateTime? | Si | Fecha limite del milestone | Existe |
| FechaCompletado | DateTime? | Si | Null = pendiente; valor = completado (marca de completado) | Existe |
| FechaCreacion | DateTime | No | Timestamp de creacion UTC | Existe |

**Navegaciones:**
- `Acuerdo` -> `AcuerdoCrowdsourcing` (N:1) - Existe
- **`Entregables`** -> **`ICollection<AcuerdoCrowdsourcingEntregable>`** (1:N) - **NUEVO** (falta en la entidad actual)

---

#### AcuerdoCrowdsourcingEntregable (MODIFICACION)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/AcuerdoCrowdsourcingEntregable.cs`

**Cambios requeridos:**
- Agregar campo `ComentarioRechazo` (string?, max 500, obligatorio al rechazar)
- Agregar campo `FechaActualizacion` (DateTime?, nullable)

| Propiedad | Tipo | Nullable | Descripcion | Estado |
|-----------|------|----------|-------------|--------|
| Id | Guid | No | PK | Existe |
| AcuerdoId | AcuerdoCrowdsourcingId | No | FK a AcuerdoCrowdsourcing | Existe |
| MilestoneId | Guid? | Si | FK a AcuerdoCrowdsourcingMilestone (agrupacion opcional) | Existe |
| Titulo | string | No | Titulo del entregable (min 3, max 200) | Existe |
| Descripcion | string? | Si | Descripcion (max 1000) | Existe |
| UrlRecurso | string? | Si | URL externa valida (Dropbox, Drive, WeTransfer, max 500) | Existe |
| EstadoEntregableId | int | No | FK a MaestraEstadoEntregable (1=Entregado, 2=Aprobado, 3=Rechazado) | Existe |
| ComentarioAprobacion | string? | Si | Comentario opcional al aprobar (max 500) | Existe |
| **ComentarioRechazo** | **string?** | **Si** | **Comentario obligatorio al rechazar (min 10, max 500)** | **NUEVO** |
| FechaAprobacion | DateTime? | Si | Timestamp de aprobacion UTC | Existe |
| **FechaActualizacion** | **DateTime?** | **Si** | **Ultima actualizacion UTC (para timeline)** | **NUEVO** |
| FechaCreacion | DateTime | No | Timestamp de creacion UTC | Existe |

**Nota:** El campo `FechaEntrega` ya existe en la entidad actual. No se elimina para no romper migracion existente, pero esta feature no lo usa activamente (el estado inicial `Entregado` ya representa la entrega).

**Navegaciones:**
- `Acuerdo` -> `AcuerdoCrowdsourcing` (N:1) - Existe
- `Milestone` -> `AcuerdoCrowdsourcingMilestone?` (N:1, nullable) - Existe

---

#### PropuestaCrowdsourcing (MODIFICACION MENOR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/PropuestaCrowdsourcing.cs`

**Cambios requeridos:**
- Agregar campo `AcuerdoId` (Guid?, nullable) - navegacion hacia el acuerdo creado al aceptar

| Propiedad | Tipo | Nullable | Descripcion | Estado |
|-----------|------|----------|-------------|--------|
| Id | PropuestaCrowdsourcingId | No | PK | Existe |
| NecesidadId | NecesidadCrowdsourcingId | No | FK a NecesidadCrowdsourcing | Existe |
| UserId | string | No | UserId del profesional proponente | Existe |
| PerfilProfesionalId | PerfilProfesionalId? | Si | FK a PerfilProfesional | Existe |
| MensajePropuesta | string? | Si | Mensaje de la propuesta (nvarchar(max)) | Existe |
| PrecioPropuesto | decimal | No | Precio propuesto (decimal(18,2)) | Existe |
| MonedaId | int? | Si | FK a MaestraMoneda | Existe |
| DiasEstimados | int? | Si | Dias estimados para el trabajo | Existe |
| EstadoPropuestaId | int | No | FK a MaestraEstadoPropuesta | Existe |
| MotivoRechazo | string? | Si | Motivo de rechazo (max 500; profesional NO lo ve) | Existe |
| **AcuerdoId** | **Guid?** | **Si** | **ID del acuerdo creado al aceptar (FK nullable)** | **NUEVO** |
| FechaCreacion | DateTime | No | Timestamp de creacion UTC | Existe |
| FechaActualizacion | DateTime? | Si | Ultima actualizacion UTC | Existe |

**Navegaciones:**
- `Necesidad` -> `NecesidadCrowdsourcing` (N:1) - Existe
- `Acuerdos` -> `ICollection<AcuerdoCrowdsourcing>` (1:N via AcuerdoCrowdsourcing.PropuestaId) - Existe

**Nota sobre AcuerdoId en PropuestaCrowdsourcing:** La relacion existente `PropuestaCrowdsourcing.Acuerdos` (coleccion) ya permite navegar de propuesta a acuerdos. El campo `AcuerdoId` (singular, nullable) es una referencia directa conveniente al acuerdo principal creado al aceptar esta propuesta. En la configuracion EF se mapea como una simple columna nullable; NO como una relacion con FK constraint adicional (para evitar circular references). Solo se persiste el GUID.

---

### 3.2 Maestras Nuevas

Las siguientes maestras son requeridas por la feature. Se crean como entidades simples de maestra (solo para seeding; NO tienen repositorio propio ya que son tablas de solo lectura consultadas via FK).

#### MaestraEstadoAcuerdo (NUEVA)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MaestraEstadoAcuerdo.cs`

| Id | Nombre | Descripcion |
|----|--------|-------------|
| 1 | Activo | Acuerdo en curso |
| 2 | Completado | Acuerdo finalizado exitosamente |
| 3 | Cancelado | Acuerdo cancelado por cualquiera de las partes |

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | int | No | PK (valor del estado) |
| Nombre | string | No | Nombre del estado (max 50) |
| Descripcion | string? | Si | Descripcion adicional (max 200) |

**Navegacion:** `Acuerdos` -> `ICollection<AcuerdoCrowdsourcing>` (1:N, opcional para navegacion inversa)

#### MaestraEstadoEntregable (NUEVA)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MaestraEstadoEntregable.cs`

| Id | Nombre | Descripcion |
|----|--------|-------------|
| 1 | Entregado | Entregable subido por el profesional, pendiente de revision |
| 2 | Aprobado | Aprobado por el artista |
| 3 | Rechazado | Rechazado por el artista, requiere nueva version |

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | int | No | PK (valor del estado) |
| Nombre | string | No | Nombre del estado (max 50) |
| Descripcion | string? | Si | Descripcion adicional (max 200) |

**Navegacion:** `Entregables` -> `ICollection<AcuerdoCrowdsourcingEntregable>` (1:N, opcional)

---

### 3.3 Constants (ErrorCodes) - MODIFICACION

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

El archivo ya existe. Se deben AGREGAR las siguientes constantes nuevas:

#### Constantes a agregar:

```
// Validation (1000-1999) - NUEVAS
Validation_InvalidUrl = "1013"        // URL invalida en UrlRecurso del entregable

// NotFound (2000-2999) - NUEVAS
NotFound_Acuerdo    = "2011"          // Acuerdo no encontrado
NotFound_Milestone  = "2012"          // Milestone no encontrado
NotFound_Entregable = "2013"          // Entregable no encontrado

// Business Rules (4000-4999) - NUEVAS
BusinessRule_PropuestaNotAcceptable      = "4007"   // Propuesta no esta en estado Pendiente
BusinessRule_AcuerdoAlreadyExists        = "4008"   // Ya existe acuerdo activo para la necesidad
BusinessRule_MilestoneImporteExceeded    = "4009"   // Suma milestones > ImporteTotalPactado
BusinessRule_AcuerdoNotActive            = "4010"   // Acuerdo no esta en estado Activo
BusinessRule_MilestoneCompleted          = "4011"   // Milestone ya completado, no editable
BusinessRule_MilestoneHasEntregables     = "4012"   // Milestone tiene entregables, no eliminable
BusinessRule_EntregableNotReviewable     = "4013"   // Entregable no esta en estado Entregado
BusinessRule_InvalidState                = "4014"   // Estado de transicion invalido (generico)
```

#### Constantes existentes que se reutilizan:

| Constante | Valor | Uso en esta feature |
|-----------|-------|---------------------|
| `Validation_Required` | "1001" | Titulo, ImporteParcial, Comentario rechazo, Motivo cancelacion |
| `Validation_MaxLength` | "1002" | Todos los campos con limite maximo |
| `Validation_MinLength` | "1011" | Titulo min 3, Comentario rechazo min 10, Motivo cancelacion min 20 |
| `Validation_InvalidDate` | "1012" | FechaFinPrevista < FechaInicio, FechaLimite < FechaInicio |
| `Validation_ForeignKeyNotFound` | "1010" | MilestoneId no pertenece al acuerdo |
| `Auth_Forbidden` | "3002" | Usuario no es participante del acuerdo |
| `NotFound_Propuesta` | "2010" | Propuesta no encontrada al aceptar/rechazar |
| `Internal_UnexpectedError` | "5000" | Catch en todos los handlers |
| `Created` | "0001" | Respuesta exitosa de creacion (acuerdo, milestone, entregable) |
| `Updated` | "0002" | Respuesta exitosa de actualizacion |
| `Deleted` | "0003" | Respuesta exitosa de eliminacion |

---

### 3.4 Constants de Estado (Nuevas)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/EstadoAcuerdoConstants.cs`

**Nuevo archivo** siguiendo el patron de `EstadoPropuestaConstants.cs`:

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `Activo` | 1 | Acuerdo activo en curso |
| `Completado` | 2 | Acuerdo completado exitosamente |
| `Cancelado` | 3 | Acuerdo cancelado |

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/EstadoEntregableConstants.cs`

**Nuevo archivo:**

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `Entregado` | 1 | Estado inicial al subir entregable |
| `Aprobado` | 2 | Aprobado por el artista |
| `Rechazado` | 3 | Rechazado por el artista |

---

### 3.5 Repository Interfaces (Nuevas - ubicacion en Application)

> Nota: Siguiendo el patron del proyecto, las interfaces de repositorio se ubican en la capa Application bajo `Interfaces/Repositories/`, no en Domain directamente.

#### IAcuerdoCrowdsourcingRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IAcuerdoCrowdsourcingRepository.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcing?` | `AcuerdoCrowdsourcingId id, CancellationToken ct` | Obtener acuerdo por ID (sin includes) |
| `GetByIdWithDetailsAsync` | `AcuerdoCrowdsourcing?` | `AcuerdoCrowdsourcingId id, CancellationToken ct` | Obtener con Milestones, Entregables, Necesidad (para GET detalle) |
| `AddAsync` | `AcuerdoCrowdsourcingId` | `AcuerdoCrowdsourcing entity, CancellationToken ct` | Crear acuerdo (SaveChanges dentro) |
| `UpdateAsync` | `Task` | `AcuerdoCrowdsourcing entity, CancellationToken ct` | Actualizar acuerdo (SaveChanges dentro) |
| `ExisteAcuerdoActivoParaNecesidadAsync` | `bool` | `NecesidadCrowdsourcingId necesidadId, CancellationToken ct` | Verifica si hay acuerdo con EstadoAcuerdoId == 1 para esa necesidad |

**Notas de implementacion del repositorio:**
- `GetByIdWithDetailsAsync` usa `.Include(m => m.Milestones).ThenInclude(m => m.Entregables)` y `.Include(a => a.Necesidad)`. Usar `AsNoTracking()`.
- `ExisteAcuerdoActivoParaNecesidadAsync` usa `.AnyAsync()` para performance.

---

#### IAcuerdoCrowdsourcingMilestoneRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IAcuerdoCrowdsourcingMilestoneRepository.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcingMilestone?` | `Guid id, CancellationToken ct` | Obtener milestone por ID |
| `GetByAcuerdoIdAsync` | `IReadOnlyList<AcuerdoCrowdsourcingMilestone>` | `AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct` | Listar todos los milestones del acuerdo |
| `GetSumaImportesByAcuerdoIdAsync` | `decimal` | `AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct` | SUM(ImporteParcial) de todos los milestones del acuerdo |
| `GetSumaImportesExcluyendoAsync` | `decimal` | `AcuerdoCrowdsourcingId acuerdoId, Guid excludeMilestoneId, CancellationToken ct` | SUM(ImporteParcial) excluyendo un milestone (para UPDATE) |
| `GetMaxOrdenAsync` | `int` | `AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct` | MAX(Orden) de milestones del acuerdo (para asignar siguiente orden) |
| `TieneEntregablesAsync` | `bool` | `Guid milestoneId, CancellationToken ct` | Verifica si el milestone tiene entregables asociados |
| `AddAsync` | `Guid` | `AcuerdoCrowdsourcingMilestone entity, CancellationToken ct` | Crear milestone (SaveChanges dentro) |
| `UpdateAsync` | `Task` | `AcuerdoCrowdsourcingMilestone entity, CancellationToken ct` | Actualizar milestone (SaveChanges dentro) |
| `DeleteAsync` | `Task` | `Guid id, CancellationToken ct` | Eliminar milestone fisicamente (SaveChanges dentro) |

---

#### IAcuerdoCrowdsourcingEntregableRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IAcuerdoCrowdsourcingEntregableRepository.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcingEntregable?` | `Guid id, CancellationToken ct` | Obtener entregable por ID |
| `GetByIdWithAcuerdoAsync` | `AcuerdoCrowdsourcingEntregable?` | `Guid id, CancellationToken ct` | Obtener entregable con su Acuerdo incluido (para validar participantes) |
| `TodosAprobadosEnMilestoneAsync` | `bool` | `Guid milestoneId, CancellationToken ct` | Verifica si todos los entregables del milestone estan en estado Aprobado |
| `AddAsync` | `Guid` | `AcuerdoCrowdsourcingEntregable entity, CancellationToken ct` | Crear entregable (SaveChanges dentro) |
| `UpdateAsync` | `Task` | `AcuerdoCrowdsourcingEntregable entity, CancellationToken ct` | Actualizar entregable (SaveChanges dentro) |

---

#### IConversacionCrowdsourcingRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IConversacionCrowdsourcingRepository.cs`

Esta interface ya deberia existir o es necesaria para la operacion transaccional de `AceptarPropuesta`. Si no existe, crearla con lo minimo necesario:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `AddAsync` | `Guid` | `ConversacionCrowdsourcing entity, CancellationToken ct` | Crear conversacion vinculada al acuerdo (SaveChanges dentro) |

---

### 3.6 Service Interfaces (Nuevas - ubicacion en Application)

> Nota: Las interfaces de servicio se ubican en `Application/Interfaces/Services/` siguiendo el patron del proyecto.

#### IAcuerdoCrowdsourcingService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcing?` | `AcuerdoCrowdsourcingId id, CancellationToken ct` | Obtener acuerdo simple (con cache) |
| `GetByIdWithDetailsAsync` | `AcuerdoCrowdsourcing?` | `AcuerdoCrowdsourcingId id, CancellationToken ct` | Obtener acuerdo con includes para detalle |
| `ExisteAcuerdoActivoParaNecesidadAsync` | `bool` | `NecesidadCrowdsourcingId necesidadId, CancellationToken ct` | Verificar si existe acuerdo activo (con cache; usado en validator) |
| `AceptarPropuestaAsync` | `AcuerdoCrowdsourcingId` | `AcuerdoCrowdsourcing acuerdo, PropuestaCrowdsourcing propuesta, NecesidadCrowdsourcing necesidad, List<PropuestaCrowdsourcing> propuestasPendientes, CancellationToken ct` | Operacion transaccional: crear acuerdo + actualizar propuestas + actualizar necesidad + crear conversacion |
| `CompletarAsync` | `bool` | `AcuerdoCrowdsourcingId id, CancellationToken ct` | Completar acuerdo (transaccional con necesidad) |
| `CancelarAsync` | `bool` | `AcuerdoCrowdsourcingId id, string motivo, string canceladoPorUserId, CancellationToken ct` | Cancelar acuerdo (transaccional con necesidad) |

**Nota sobre AceptarPropuestaAsync:** El service recibe los objetos ya cargados por el handler/validator para evitar queries duplicados. La transaccion de base de datos se abre dentro del service.

---

#### IAcuerdoCrowdsourcingMilestoneService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingMilestoneService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcingMilestone?` | `Guid id, CancellationToken ct` | Obtener milestone por ID (con cache) |
| `GetSumaImportesAsync` | `decimal` | `AcuerdoCrowdsourcingId acuerdoId, CancellationToken ct` | Suma de importes parciales del acuerdo (con cache) |
| `GetSumaImportesExcluyendoAsync` | `decimal` | `AcuerdoCrowdsourcingId acuerdoId, Guid excludeId, CancellationToken ct` | Suma excluyendo milestone (para UPDATE, con cache) |
| `CreateAsync` | `Guid` | `AcuerdoCrowdsourcingMilestone entity, CancellationToken ct` | Crear milestone (asigna Orden, FechaCreacion, llama repository) |
| `UpdateAsync` | `bool` | `AcuerdoCrowdsourcingMilestone entity, CancellationToken ct` | Actualizar milestone |
| `DeleteAsync` | `bool` | `Guid id, CancellationToken ct` | Eliminar milestone (valida sin entregables) |

---

#### IAcuerdoCrowdsourcingEntregableService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingEntregableService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `AcuerdoCrowdsourcingEntregable?` | `Guid id, CancellationToken ct` | Obtener entregable (con cache) |
| `GetByIdWithAcuerdoAsync` | `AcuerdoCrowdsourcingEntregable?` | `Guid id, CancellationToken ct` | Obtener con Acuerdo incluido (para autorizacion) |
| `CreateAsync` | `Guid` | `AcuerdoCrowdsourcingEntregable entity, CancellationToken ct` | Crear entregable en estado Entregado |
| `AprobarAsync` | `bool` | `Guid id, string? comentario, CancellationToken ct` | Cambiar estado a Aprobado, registrar FechaAprobacion y comentario |
| `RechazarAsync` | `bool` | `Guid id, string comentario, CancellationToken ct` | Cambiar estado a Rechazado, registrar ComentarioRechazo |
| `TodosAprobadosEnMilestoneAsync` | `bool` | `Guid milestoneId, CancellationToken ct` | Verificar si todos los entregables del milestone estan aprobados |

---

## 4. Infrastructure Layer

### 4.1 Repository Implementations

#### AcuerdoCrowdsourcingRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/AcuerdoCrowdsourcingRepository.cs`

- Implementa: `IAcuerdoCrowdsourcingRepository`
- Inyecta: `CrowdsourcingContext _context`
- Patron de constructor: `_context = context ?? throw new ArgumentNullException(nameof(context))`

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | `_context.Acuerdos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByIdWithDetailsAsync` | `_context.Acuerdos.AsNoTracking().Include(a => a.Milestones).ThenInclude(m => m.Entregables).Include(a => a.Necesidad).FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `AddAsync` | `_context.Acuerdos.AddAsync(entity, ct)` + `SaveChangesAsync(ct)`. Retorna `entity.Id` |
| `UpdateAsync` | `_context.Acuerdos.Update(entity)` + `SaveChangesAsync(ct)` |
| `ExisteAcuerdoActivoParaNecesidadAsync` | `_context.Acuerdos.AnyAsync(x => x.NecesidadId == necesidadId && x.EstadoAcuerdoId == EstadoAcuerdoConstants.Activo, ct)` |

**IMPORTANTE:** El repository hace `SaveChangesAsync` en cada operacion de escritura individual. Para operaciones transaccionales, el service abre una transaccion de `_context.Database.BeginTransactionAsync()` y delega al repository como parte de esa transaccion (el SaveChanges dentro de la transaccion es valido ya que el contexto EF es el mismo).

---

#### AcuerdoCrowdsourcingMilestoneRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/AcuerdoCrowdsourcingMilestoneRepository.cs`

- Implementa: `IAcuerdoCrowdsourcingMilestoneRepository`
- Inyecta: `CrowdsourcingContext _context`

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | `_context.Milestones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByAcuerdoIdAsync` | `_context.Milestones.AsNoTracking().Where(x => x.AcuerdoId == acuerdoId).OrderBy(x => x.Orden).ToListAsync(ct)` |
| `GetSumaImportesByAcuerdoIdAsync` | `_context.Milestones.Where(x => x.AcuerdoId == acuerdoId).SumAsync(x => x.ImporteParcial, ct)` (retorna 0 si no hay) |
| `GetSumaImportesExcluyendoAsync` | `_context.Milestones.Where(x => x.AcuerdoId == acuerdoId && x.Id != excludeMilestoneId).SumAsync(x => x.ImporteParcial, ct)` |
| `GetMaxOrdenAsync` | `_context.Milestones.Where(x => x.AcuerdoId == acuerdoId).MaxAsync(x => (int?)x.Orden, ct) ?? 0` |
| `TieneEntregablesAsync` | `_context.Entregables.AnyAsync(x => x.MilestoneId == milestoneId, ct)` |
| `AddAsync` | `_context.Milestones.AddAsync(entity, ct)` + `SaveChangesAsync(ct)`. Retorna `entity.Id` |
| `UpdateAsync` | `_context.Milestones.Update(entity)` + `SaveChangesAsync(ct)` |
| `DeleteAsync` | Busca por ID con `FindAsync`, elimina con `Remove`, hace `SaveChangesAsync(ct)` |

---

#### AcuerdoCrowdsourcingEntregableRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/AcuerdoCrowdsourcingEntregableRepository.cs`

- Implementa: `IAcuerdoCrowdsourcingEntregableRepository`
- Inyecta: `CrowdsourcingContext _context`

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | `_context.Entregables.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByIdWithAcuerdoAsync` | `_context.Entregables.AsNoTracking().Include(e => e.Acuerdo).FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `TodosAprobadosEnMilestoneAsync` | `!await _context.Entregables.AnyAsync(x => x.MilestoneId == milestoneId && x.EstadoEntregableId != EstadoEntregableConstants.Aprobado, ct)` |
| `AddAsync` | `_context.Entregables.AddAsync(entity, ct)` + `SaveChangesAsync(ct)`. Retorna `entity.Id` |
| `UpdateAsync` | `_context.Entregables.Update(entity)` + `SaveChangesAsync(ct)` |

---

#### ConversacionCrowdsourcingRepository

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/ConversacionCrowdsourcingRepository.cs`

- Implementa: `IConversacionCrowdsourcingRepository`
- Inyecta: `CrowdsourcingContext _context`

| Metodo | Implementacion |
|--------|---------------|
| `AddAsync` | `_context.Conversaciones.AddAsync(entity, ct)` + `SaveChangesAsync(ct)`. Retorna `entity.Id` |

---

### 4.2 Service Implementations

#### AcuerdoCrowdsourcingService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/AcuerdoCrowdsourcingService.cs`

**Implementa:** `IAcuerdoCrowdsourcingService`

**Constructor - Dependencias:**

```
IAcuerdoCrowdsourcingRepository _repository
IPropuestaCrowdsourcingRepository _propuestaRepository
INecesidadCrowdsourcingRepository _necesidadRepository
IConversacionCrowdsourcingRepository _conversacionRepository
IRequestCacheService _requestCache
CrowdsourcingContext _context              <- Para transacciones con BeginTransactionAsync
ILogger<AcuerdoCrowdsourcingService> _logger
```

TODAS las dependencias con `?? throw new ArgumentNullException(nameof(...))`.

**Implementacion por metodo:**

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | Cache key `"acuerdo:{id.Value}"`. Llama `_repository.GetByIdAsync`. |
| `GetByIdWithDetailsAsync` | Sin cache (query compleja). Llama `_repository.GetByIdWithDetailsAsync`. |
| `ExisteAcuerdoActivoParaNecesidadAsync` | Cache key `"acuerdo:activo:{necesidadId.Value}"`. Llama `_repository.ExisteAcuerdoActivoParaNecesidadAsync`. |
| `AceptarPropuestaAsync` | Abre `_context.Database.BeginTransactionAsync()`. En orden: (1) `_repository.AddAsync(acuerdo)`, (2) actualizar propuesta aceptada + `_propuestaRepository.UpdateAsync`, (3) actualizar propuestas pendientes a Rechazada + `_propuestaRepository.UpdateManyAsync`, (4) actualizar necesidad a `En Progreso` + `_necesidadRepository.UpdateAsync`, (5) crear `ConversacionCrowdsourcing` + `_conversacionRepository.AddAsync`. `CommitAsync`. En `catch`: `RollbackAsync` + `throw`. |
| `CompletarAsync` | Abre transaccion. Carga acuerdo. Actualiza `EstadoAcuerdoId = Completado`, `FechaFinReal = UtcNow`, `FechaActualizacion = UtcNow`. `_repository.UpdateAsync`. Carga necesidad. Actualiza `EstadoNecesidadId = Cerrada` (valor = 3). `_necesidadRepository.UpdateAsync`. Commit. |
| `CancelarAsync` | Abre transaccion. Carga acuerdo. Actualiza `EstadoAcuerdoId = Cancelado`, `FechaFinReal = UtcNow`, `MotivoCancelacion = motivo`, `CanceladoPor = canceladoPorUserId`, `FechaActualizacion = UtcNow`. `_repository.UpdateAsync`. Carga necesidad. Actualiza `EstadoNecesidadId = Abierta` (valor = 1). `_necesidadRepository.UpdateAsync`. Commit. |

**Constantes de EstadoNecesidad requeridas:**
- Abierta = 1
- En Progreso = 2 (necesidad de US-CS-04; verificar si ya existe en Constants)
- Cerrada = 3

**Nota sobre EstadoNecesidad:** Verificar si existe `EstadoNecesidadConstants.cs` en Domain. Si no existe, crear archivo `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/EstadoNecesidadConstants.cs` con los valores enteros correspondientes a las maestras de BD existentes.

---

#### AcuerdoCrowdsourcingMilestoneService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/AcuerdoCrowdsourcingMilestoneService.cs`

**Implementa:** `IAcuerdoCrowdsourcingMilestoneService`

**Constructor - Dependencias:**

```
IAcuerdoCrowdsourcingMilestoneRepository _repository
IRequestCacheService _requestCache
ILogger<AcuerdoCrowdsourcingMilestoneService> _logger
```

TODAS las dependencias con `?? throw new ArgumentNullException(nameof(...))`.

**Implementacion por metodo:**

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | Cache key `"milestone:{id}"`. Llama `_repository.GetByIdAsync`. |
| `GetSumaImportesAsync` | Cache key `"milestone:suma:{acuerdoId.Value}"`. Llama `_repository.GetSumaImportesByAcuerdoIdAsync`. |
| `GetSumaImportesExcluyendoAsync` | Sin cache (query con exclusion especifica). Llama `_repository.GetSumaImportesExcluyendoAsync`. |
| `CreateAsync` | Asigna `entity.FechaCreacion = DateTime.UtcNow`. Obtiene `maxOrden = await _repository.GetMaxOrdenAsync(entity.AcuerdoId, ct)`. Asigna `entity.Orden = maxOrden + 1`. Llama `_repository.AddAsync(entity, ct)`. Log de creacion. |
| `UpdateAsync` | Llama `_repository.UpdateAsync`. Log de actualizacion. Retorna `true`. |
| `DeleteAsync` | Llama `_repository.DeleteAsync`. Log de eliminacion. Retorna `true`. |

---

#### AcuerdoCrowdsourcingEntregableService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/AcuerdoCrowdsourcingEntregableService.cs`

**Implementa:** `IAcuerdoCrowdsourcingEntregableService`

**Constructor - Dependencias:**

```
IAcuerdoCrowdsourcingEntregableRepository _repository
IRequestCacheService _requestCache
ILogger<AcuerdoCrowdsourcingEntregableService> _logger
```

TODAS las dependencias con `?? throw new ArgumentNullException(nameof(...))`.

**Implementacion por metodo:**

| Metodo | Implementacion |
|--------|---------------|
| `GetByIdAsync` | Cache key `"entregable:{id}"`. Llama `_repository.GetByIdAsync`. |
| `GetByIdWithAcuerdoAsync` | Sin cache. Llama `_repository.GetByIdWithAcuerdoAsync`. |
| `CreateAsync` | Asigna `entity.FechaCreacion = DateTime.UtcNow`. `entity.EstadoEntregableId = EstadoEntregableConstants.Entregado`. Llama `_repository.AddAsync`. Log. |
| `AprobarAsync` | Carga entregable (sin cache para asegurar estado actual). Actualiza `EstadoEntregableId = Aprobado`, `FechaAprobacion = UtcNow`, `ComentarioAprobacion = comentario`, `FechaActualizacion = UtcNow`. `_repository.UpdateAsync`. Log. Retorna `true`. |
| `RechazarAsync` | Carga entregable. Actualiza `EstadoEntregableId = Rechazado`, `ComentarioRechazo = comentario`, `FechaActualizacion = UtcNow`. `_repository.UpdateAsync`. Log. Retorna `true`. |
| `TodosAprobadosEnMilestoneAsync` | Cache key `"entregable:todos-aprobados:{milestoneId}"`. Llama `_repository.TodosAprobadosEnMilestoneAsync`. Si `milestoneId == null`, retorna `false`. |

---

### 4.3 Entity Configurations EF Core (Modificaciones al CrowdsourcingContext)

> Nota: El proyecto usa inline configurations en `OnModelCreating` del `CrowdsourcingContext`. No usa archivos `IEntityTypeConfiguration<T>` separados para las entidades principales. Se mantiene ese patron.

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

#### Modificaciones a AcuerdoCrowdsourcing (inline en OnModelCreating)

Agregar dentro del bloque existente `modelBuilder.Entity<AcuerdoCrowdsourcing>`:

```
// Campos nuevos a agregar:
entity.Property(e => e.MotivoCancelacion).HasMaxLength(1000);
entity.Property(e => e.CanceladoPor).HasMaxLength(450);
```

#### Modificaciones a AcuerdoCrowdsourcingMilestone (inline en OnModelCreating)

Agregar dentro del bloque existente `modelBuilder.Entity<AcuerdoCrowdsourcingMilestone>`:

```
// Relacion con Entregables (nueva - coleccion de navegacion):
entity.HasMany(e => e.Entregables)
    .WithOne(e => e.Milestone)
    .HasForeignKey(e => e.MilestoneId)
    .OnDelete(DeleteBehavior.NoAction);
```

**Nota:** La relacion `AcuerdoCrowdsourcingEntregable.Milestone` ya tiene configurado `.OnDelete(DeleteBehavior.NoAction)`. Mover esa configuracion al lado del Milestone para mayor coherencia.

#### Modificaciones a AcuerdoCrowdsourcingEntregable (inline en OnModelCreating)

Agregar dentro del bloque existente `modelBuilder.Entity<AcuerdoCrowdsourcingEntregable>`:

```
// Campos nuevos a agregar:
entity.Property(e => e.ComentarioRechazo).HasMaxLength(500);
entity.Property(e => e.FechaActualizacion).HasPrecision(3);
```

#### Modificaciones a PropuestaCrowdsourcing (inline en OnModelCreating)

Agregar dentro del bloque existente `modelBuilder.Entity<PropuestaCrowdsourcing>`:

```
// Campo nuevo AcuerdoId (referencia directa al acuerdo creado, sin FK constraint):
entity.Property(e => e.AcuerdoId).HasColumnName("Acuerdo_Id_Ref");  // Nombre de columna alternativo para evitar confusion con FK de navegacion
```

**Nota sobre PropuestaCrowdsourcing.AcuerdoId:** Este campo es un simple Guid nullable que referencia convenientemente al acuerdo. No se configura como FK con constraint EF porque la relacion principal ya esta modelada via `AcuerdoCrowdsourcing.PropuestaId`. Usar `HasColumnName("Acuerdo_Id_Ref")` para distinguirlo de otras columnas FK del patron. En el handler de `AceptarPropuesta`, se asigna despues de crear el acuerdo.

#### Nuevas Maestras en OnModelCreating

Agregar configuraciones para `MaestraEstadoAcuerdo` y `MaestraEstadoEntregable`:

**MaestraEstadoAcuerdo:**

```
modelBuilder.Entity<MaestraEstadoAcuerdo>(entity =>
{
    entity.ToTable("MaestraEstadoAcuerdo");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Descripcion).HasMaxLength(200);

    // Seed data
    entity.HasData(
        new MaestraEstadoAcuerdo { Id = 1, Nombre = "Activo",     Descripcion = "Acuerdo en curso" },
        new MaestraEstadoAcuerdo { Id = 2, Nombre = "Completado", Descripcion = "Acuerdo finalizado exitosamente" },
        new MaestraEstadoAcuerdo { Id = 3, Nombre = "Cancelado",  Descripcion = "Acuerdo cancelado" }
    );
});
```

**MaestraEstadoEntregable:**

```
modelBuilder.Entity<MaestraEstadoEntregable>(entity =>
{
    entity.ToTable("MaestraEstadoEntregable");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Descripcion).HasMaxLength(200);

    // Seed data
    entity.HasData(
        new MaestraEstadoEntregable { Id = 1, Nombre = "Entregado", Descripcion = "Subido por el profesional, pendiente revision" },
        new MaestraEstadoEntregable { Id = 2, Nombre = "Aprobado",  Descripcion = "Aprobado por el artista" },
        new MaestraEstadoEntregable { Id = 3, Nombre = "Rechazado", Descripcion = "Rechazado, requiere nueva version" }
    );
});
```

#### Nuevos DbSets en CrowdsourcingContext

Agregar en la region `#region DbSets`:

```csharp
public DbSet<MaestraEstadoAcuerdo> MaestrasEstadoAcuerdo => Set<MaestraEstadoAcuerdo>();
public DbSet<MaestraEstadoEntregable> MaestrasEstadoEntregable => Set<MaestraEstadoEntregable>();
```

#### StronglyTypedIds - Verificar

Los siguientes StronglyTypedIds ya existen y se reutilizan:
- `AcuerdoCrowdsourcingId` (ya existe en BuildingBlocks)
- `NecesidadCrowdsourcingId` (ya existe)
- `PropuestaCrowdsourcingId` (ya existe)
- `ArtistaId` (ya existe)
- `PerfilProfesionalId` (ya existe)

Los tipos Guid simples se usan para Milestone y Entregable (sin StronglyTypedId propio - patron del codigo existente).

---

## 5. Modificaciones al IPropuestaCrowdsourcingService

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IPropuestaCrowdsourcingService.cs`

Agregar metodos necesarios para la operacion de aceptar propuesta:

| Metodo nuevo | Retorno | Descripcion |
|-------------|---------|-------------|
| `AceptarAsync` | `Task` | Actualiza propuesta a estado Aceptada y asigna AcuerdoId |

**Nota:** El metodo `RechazarPropuestasPendientesAsync` ya existe y se reutiliza en la logica transaccional de `AceptarPropuestaAsync`. El handler de `RechazarPropuesta` (individual) usara directamente el service existente con una actualizacion de estado.

---

## 6. Migraciones Necesarias

### Migracion 1: AddCamposAcuerdoEntregables

**Comando:**
```bash
dotnet ef migrations add AddCamposAcuerdoEntregables \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi \
  --context CrowdsourcingContext
```

**Cambios incluidos:**
- Tabla `AcuerdoCrowdsourcing`: agregar columnas `MotivoCancelacion` (nvarchar(1000), nullable) y `CanceladoPor` (nvarchar(450), nullable)
- Tabla `AcuerdoCrowdsourcing_Entregable`: agregar columnas `ComentarioRechazo` (nvarchar(500), nullable) y `FechaActualizacion` (datetime2(3), nullable)
- Tabla `PropuestaCrowdsourcing`: agregar columna `Acuerdo_Id_Ref` (uniqueidentifier, nullable)

### Migracion 2: AddMaestrasEstadoAcuerdoEntregable

**Comando:**
```bash
dotnet ef migrations add AddMaestrasEstadoAcuerdoEntregable \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi \
  --context CrowdsourcingContext
```

**Cambios incluidos:**
- Crear tabla `MaestraEstadoAcuerdo` (Id, Nombre, Descripcion)
- Crear tabla `MaestraEstadoEntregable` (Id, Nombre, Descripcion)
- Seed data con los 3 estados de cada tabla

**Nota sobre orden de migraciones:** Aplicar `AddMaestrasEstadoAcuerdoEntregable` antes o despues de `AddCamposAcuerdoEntregables` es indiferente (son tablas independientes sin FK entre si en esta fase).

### Aplicar migraciones:

```bash
dotnet ef database update \
  --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
  --startup-project src/api/WebApi \
  --context CrowdsourcingContext
```

---

## 7. Registro DI

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

Agregar dentro del metodo `AddCrowdsourcingServices`:

```csharp
// Nuevos Repositories (cs-acuerdos-entregables)
services.AddScoped<IAcuerdoCrowdsourcingRepository, AcuerdoCrowdsourcingRepository>();
services.AddScoped<IAcuerdoCrowdsourcingMilestoneRepository, AcuerdoCrowdsourcingMilestoneRepository>();
services.AddScoped<IAcuerdoCrowdsourcingEntregableRepository, AcuerdoCrowdsourcingEntregableRepository>();
services.AddScoped<IConversacionCrowdsourcingRepository, ConversacionCrowdsourcingRepository>();

// Nuevos Services (cs-acuerdos-entregables)
services.AddScoped<IAcuerdoCrowdsourcingService, AcuerdoCrowdsourcingService>();
services.AddScoped<IAcuerdoCrowdsourcingMilestoneService, AcuerdoCrowdsourcingMilestoneService>();
services.AddScoped<IAcuerdoCrowdsourcingEntregableService, AcuerdoCrowdsourcingEntregableService>();
```

---

## 8. Flujo de Datos por Operacion

### Flujo: AceptarPropuesta (Transaccional)

```
Controller (POST /propuestas/{id}/aceptar)
  -> AceptarPropuestaCommand (MediatR)
  -> AceptarPropuestaCommandHandler
       |-> AceptarPropuestaValidator
       |     |-> _propuestaService.GetByIdAsync(id)    [cache: "propuesta:{id}"]
       |     |-> _acuerdoService.ExisteAcuerdoActivoParaNecesidadAsync  [cache]
       |
       |-> _mapper.Map<AcuerdoCrowdsourcing>(command)
       |-> _acuerdoService.AceptarPropuestaAsync(acuerdo, propuesta, necesidad, propuestasPendientes)
             |-> BEGIN TRANSACTION
             |-> _repository.AddAsync(acuerdo)               -> SaveChanges
             |-> _propuestaRepository.UpdateAsync(aceptada)  -> SaveChanges
             |-> _propuestaRepository.UpdateManyAsync(rechazadas) -> SaveChanges
             |-> _necesidadRepository.UpdateAsync(enProgreso)-> SaveChanges
             |-> _conversacionRepository.AddAsync(conv)      -> SaveChanges
             |-> COMMIT
```

### Flujo: CreateMilestone (Simple)

```
Controller (POST /acuerdos/{id}/milestones)
  -> CreateMilestoneCommand (MediatR)
  -> CreateMilestoneCommandHandler
       |-> CreateMilestoneValidator
       |     |-> _acuerdoService.GetByIdAsync(acuerdoId)    [cache]
       |     |-> _milestoneService.GetSumaImportesAsync(acuerdoId) [cache: "milestone:suma:{id}"]
       |
       |-> _milestoneService.CreateAsync(entity)
             |-> GetMaxOrdenAsync -> Asigna Orden
             |-> _repository.AddAsync -> SaveChanges
```

### Flujo: AprobarEntregable

```
Controller (PATCH /entregables/{id}/aprobar)
  -> AprobarEntregableCommand (MediatR)
  -> AprobarEntregableCommandHandler
       |-> AprobarEntregableValidator
       |     |-> _entregableService.GetByIdWithAcuerdoAsync(id)  [sin cache]
       |
       |-> _entregableService.AprobarAsync(id, comentario)
       |     |-> Update estado + FechaAprobacion + ComentarioAprobacion
       |     |-> _repository.UpdateAsync -> SaveChanges
       |
       |-> _entregableService.TodosAprobadosEnMilestoneAsync(milestoneId)
             |-> [cache: "entregable:todos-aprobados:{milestoneId}"]
             |-> Retorna bool para AprobarEntregableResultDto.TodosAprobadosEnMilestone
```

---

## 9. Resumen de Archivos a Crear / Modificar

### Archivos a CREAR (nuevos)

```
src/api/Modules/Crowdsourcing/
|
+-- WePlayRises.Crowdsourcing.Domain/
|   +-- Model/
|   |   +-- MaestraEstadoAcuerdo.cs                   [NUEVO - entidad maestra]
|   |   +-- MaestraEstadoEntregable.cs                [NUEVO - entidad maestra]
|   +-- Constants/
|       +-- EstadoAcuerdoConstants.cs                 [NUEVO - int constants 1,2,3]
|       +-- EstadoEntregableConstants.cs              [NUEVO - int constants 1,2,3]
|
+-- WePlayRises.Crowdsourcing.Application/
|   +-- Interfaces/
|       +-- Repositories/
|       |   +-- IAcuerdoCrowdsourcingRepository.cs              [NUEVO]
|       |   +-- IAcuerdoCrowdsourcingMilestoneRepository.cs     [NUEVO]
|       |   +-- IAcuerdoCrowdsourcingEntregableRepository.cs    [NUEVO]
|       |   +-- IConversacionCrowdsourcingRepository.cs         [NUEVO]
|       +-- Services/
|           +-- IAcuerdoCrowdsourcingService.cs                 [NUEVO]
|           +-- IAcuerdoCrowdsourcingMilestoneService.cs        [NUEVO]
|           +-- IAcuerdoCrowdsourcingEntregableService.cs       [NUEVO]
|
+-- WePlayRises.Crowdsourcing.Infra/
    +-- Repositories/
    |   +-- AcuerdoCrowdsourcingRepository.cs                   [NUEVO]
    |   +-- AcuerdoCrowdsourcingMilestoneRepository.cs          [NUEVO]
    |   +-- AcuerdoCrowdsourcingEntregableRepository.cs         [NUEVO]
    |   +-- ConversacionCrowdsourcingRepository.cs              [NUEVO]
    +-- Services/
        +-- AcuerdoCrowdsourcingService.cs                      [NUEVO]
        +-- AcuerdoCrowdsourcingMilestoneService.cs             [NUEVO]
        +-- AcuerdoCrowdsourcingEntregableService.cs            [NUEVO]
```

### Archivos a MODIFICAR (existentes)

```
src/api/Modules/Crowdsourcing/
|
+-- WePlayRises.Crowdsourcing.Domain/
|   +-- Model/
|   |   +-- AcuerdoCrowdsourcing.cs          [MODIFICAR: +MotivoCancelacion, +CanceladoPor]
|   |   +-- AcuerdoCrowdsourcingMilestone.cs [MODIFICAR: +ICollection<Entregables>]
|   |   +-- AcuerdoCrowdsourcingEntregable.cs[MODIFICAR: +ComentarioRechazo, +FechaActualizacion]
|   |   +-- PropuestaCrowdsourcing.cs        [MODIFICAR: +AcuerdoId (Guid?)]
|   +-- Constants/
|       +-- ServiceResponseMessageType.cs   [MODIFICAR: +nuevas constantes 1013,2011-2013,4007-4014]
|
+-- WePlayRises.Crowdsourcing.Application/
|   +-- Interfaces/
|       +-- Services/
|           +-- IPropuestaCrowdsourcingService.cs  [MODIFICAR: +AceptarAsync]
|
+-- WePlayRises.Crowdsourcing.Infra/
    +-- Context/
    |   +-- CrowdsourcingContext.cs          [MODIFICAR: +DbSets maestras, +configs nuevas columnas, +HasMany Entregables en Milestone]
    +-- DependencyInjection.cs               [MODIFICAR: +7 nuevos registros]
```

---

## 10. Checklist de Validacion Arquitectonica

- [ ] Entidades son POCOs (sin metodos de negocio, solo propiedades y navegaciones)
- [ ] `AcuerdoCrowdsourcing` tiene `MotivoCancelacion` y `CanceladoPor`
- [ ] `AcuerdoCrowdsourcingMilestone` tiene coleccion `Entregables`
- [ ] `AcuerdoCrowdsourcingEntregable` tiene `ComentarioRechazo` y `FechaActualizacion`
- [ ] `PropuestaCrowdsourcing` tiene `AcuerdoId` (Guid?, nullable)
- [ ] `MaestraEstadoAcuerdo` y `MaestraEstadoEntregable` creadas con seed data
- [ ] `EstadoAcuerdoConstants.cs` y `EstadoEntregableConstants.cs` creados
- [ ] `ServiceResponseMessageType.cs` actualizado con constantes 1013, 2011-2013, 4007-4014
- [ ] Repository interfaces en `Application/Interfaces/Repositories/` (4 nuevas)
- [ ] Service interfaces en `Application/Interfaces/Services/` (3 nuevas)
- [ ] Repository implementations en `Infra/Repositories/` (4 nuevas)
- [ ] Service implementations en `Infra/Services/` (3 nuevas)
- [ ] Todos los constructores con `?? throw new ArgumentNullException(nameof(...))` en TODAS las dependencias
- [ ] Services usan `IRequestCacheService` con cache keys descriptivas
- [ ] Services NO retornan DTOs (retornan entidades de dominio)
- [ ] `AcuerdoCrowdsourcingService.AceptarPropuestaAsync` usa `BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`
- [ ] `CompletarAsync` y `CancelarAsync` son transaccionales (acuerdo + necesidad en una transaccion)
- [ ] Repositorios hacen `SaveChangesAsync` en cada operacion de escritura individual
- [ ] Handlers NUNCA inyectan `CrowdsourcingContext` directamente (usan services)
- [ ] `CrowdsourcingContext` actualizado con nuevas columnas en OnModelCreating
- [ ] Nuevos DbSets de maestras agregados al context
- [ ] 2 migraciones planificadas y con nombres descriptivos
- [ ] `DependencyInjection.cs` actualizado con 7 nuevos registros (4 repos + 3 services)
- [ ] `IConversacionCrowdsourcingRepository` creada para uso en la operacion transaccional

---

## 11. Consideraciones de Performance y Cache

| Operacion | Cache Strategy | Cache Key |
|-----------|---------------|-----------|
| `GetAcuerdoByIdAsync` (simple) | Request-scoped | `"acuerdo:{id.Value}"` |
| `ExisteAcuerdoActivoParaNecesidadAsync` | Request-scoped | `"acuerdo:activo:{necesidadId.Value}"` |
| `GetMilestoneByIdAsync` | Request-scoped | `"milestone:{id}"` |
| `GetSumaImportesAsync` | Request-scoped | `"milestone:suma:{acuerdoId.Value}"` |
| `GetEntregableByIdAsync` | Request-scoped | `"entregable:{id}"` |
| `TodosAprobadosEnMilestoneAsync` | Request-scoped | `"entregable:todos-aprobados:{milestoneId}"` |
| `GetByIdWithDetailsAsync` (GET detalle completo) | Sin cache | Query compleja unica |
| Operaciones transaccionales (AceptarPropuesta, Completar, Cancelar) | Sin cache | Operaciones de escritura |

**Patron clave:** El `IRequestCacheService` es request-scoped (por solicitud HTTP). Permite que el `Validator` cargue el acuerdo una vez y el `Handler` lo reutilice sin segunda query a BD, evitando el N+1 clasico en el flujo Validator -> Handler.

---

## 12. Dependencias entre Servicios

```
AcuerdoCrowdsourcingService
    +-- IAcuerdoCrowdsourcingRepository
    +-- IPropuestaCrowdsourcingRepository  (para rechazar propuestas al aceptar)
    +-- INecesidadCrowdsourcingRepository  (para cambiar estado de necesidad)
    +-- IConversacionCrowdsourcingRepository (para crear conversacion al aceptar)
    +-- CrowdsourcingContext               (para transacciones explicitas)
    +-- IRequestCacheService
    +-- ILogger<AcuerdoCrowdsourcingService>

AcuerdoCrowdsourcingMilestoneService
    +-- IAcuerdoCrowdsourcingMilestoneRepository
    +-- IRequestCacheService
    +-- ILogger<AcuerdoCrowdsourcingMilestoneService>

AcuerdoCrowdsourcingEntregableService
    +-- IAcuerdoCrowdsourcingEntregableRepository
    +-- IRequestCacheService
    +-- ILogger<AcuerdoCrowdsourcingEntregableService>
```

**Nota sobre inyeccion del CrowdsourcingContext en AcuerdoCrowdsourcingService:** Este es el patron establecido en el proyecto para operaciones transaccionales. El `NecesidadCrowdsourcingService` ya lo hace (ver implementacion actual de `CerrarAsync`). Los Handlers siguen sin inyectar el contexto directamente.
