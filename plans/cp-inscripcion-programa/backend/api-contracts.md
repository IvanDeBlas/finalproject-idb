# Contratos API: cp-inscripcion-programa

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-inscripcion-programa (US-CP-03)
**Depende de:** US-CP-01 (cp-perfil-promotor), US-CP-02 (cp-programas-promocion)

---

## 0. Cambios de Modelo Previos Requeridos

Antes de implementar cualquier endpoint de esta feature, la entidad `PromoProgramaPromotor` debe ser extendida con dos campos nuevos. Los registros existentes no se rompen porque los defaults son `false`.

### 0.1 Entidad PromoProgramaPromotor - Campos a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoProgramaPromotor.cs`

| Campo | Tipo | Default | Descripcion |
|-------|------|---------|-------------|
| `EsAprobado` | `bool` | `false` | El artista ha aprobado la inscripcion. Al aprobar se genera CodigoReferido. |
| `EsBloqueado` | `bool` | `false` | El artista ha bloqueado al promotor. Impide re-solicitud futura. |

**Maquina de estados derivada del modelo:**

| EsAprobado | EsBloqueado | FechaBaja | Estado visible |
|------------|-------------|-----------|----------------|
| false | false | null | Pendiente |
| true | false | null | Aprobado |
| false | true | null | Bloqueado |
| false | false | valor | DadoDeBaja |

**Nota de mapeo campo existente:** `UrlReferido` (en entidad) se expone como `urlTrackingPersonalizada` en los DTOs. No se renombra la columna en BD.

**Nota de mapeo campo existente:** `FechaInscripcion` (en entidad) se expone como `fechaAlta` en los DTOs para consistencia con la US.

**Migracion EF Core requerida:** Agregar columnas `EsAprobado BIT NOT NULL DEFAULT 0` y `EsBloqueado BIT NOT NULL DEFAULT 0` a la tabla `PromoProgramaPromotor`.

### 0.2 ServiceResponseMessageType - Constantes a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Las constantes nuevas para esta feature deben agregarse al archivo existente:

| Constante | Valor | Descripcion |
|-----------|-------|-------------|
| `NotFound_Inscripcion` | `"2020"` | No existe PromoProgramaPromotor con el id proporcionado |
| `BusinessRule_InscripcionAlreadyExists` | `"4021"` | El promotor ya tiene inscripcion en este programa |
| `BusinessRule_InscripcionBloqueada` | `"4022"` | El promotor esta bloqueado en este programa |
| `BusinessRule_PromotorInactivo` | `"4023"` | El perfil de promotor esta desactivado |
| `BusinessRule_ProgramaInactivo` | `"4024"` | El programa no esta activo |
| `BusinessRule_InscripcionEstadoInvalido` | `"4025"` | La inscripcion no esta en el estado requerido para la operacion |
| `BusinessRule_NoEsPropietarioPrograma` | `"4026"` | El artista del token no coincide con el propietario del programa |

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Actor | Descripcion |
|--------|------|------|-------|-------------|
| GET | `/api/crowdpromotion/programas/explorar` | Query | Promotor | Catalogo publico paginado de programas activos con estado personal |
| POST | `/api/crowdpromotion/programas/{programaId}/inscripcion` | Command | Promotor activo | Solicitar inscripcion en un programa |
| PATCH | `/api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar` | Command | Artista propietario | Aprobar solicitud, genera CodigoReferido y UrlTracking |
| PATCH | `/api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/rechazar` | Command | Artista propietario | Rechazar solicitud (elimina fisicamente el registro) |
| PATCH | `/api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/bloquear` | Command | Artista propietario | Bloquear promotor en el programa |
| PATCH | `/api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja` | Command | Artista propietario | Dar de baja a promotor aprobado |
| GET | `/api/crowdpromotion/programas/{programaId}/inscripciones` | Query | Artista propietario | Lista paginada de inscripciones del programa con filtro por estado |
| GET | `/api/crowdpromotion/promotor/mis-programas` | Query | Promotor | Lista de inscripciones del promotor autenticado con tareas para aprobadas |

---

## 2. Request DTOs (Commands y Queries)

### 2.1 ExplorarProgramasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/ExplorarProgramasQuery.cs`

**Implementa:** `IRequest<ServiceResponse<ExplorarProgramasResultDto>>`

Este es un GET con query params. No tiene body. Los parametros se reciben por la URL y se construye el query en el controller.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver PromotorId y calcular `miEstado` |
| `ArtistaNombre` | `string?` | No | Query param `artistaNombre` | Filtrar por nombre artistico (contains, case-insensitive) |
| `TipoPromoId` | `int?` | No | Query param `tipoPromoId` | Filtrar por tipo de programa |
| `Page` | `int` | No | Query param `page` | Default 1, min 1 |
| `PageSize` | `int` | No | Query param `pageSize` | Default 10, min 1, max 50 |

**Nota:** No tiene validator porque no hay body. La validacion de rango de Page/PageSize se hace con `Math.Max`/`Math.Min` en el controller antes de construir el query.

---

### 2.2 SolicitarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/SolicitarInscripcionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionCreadaDto>>`

No tiene body de request. Toda la informacion viene de la ruta y el token.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa al que se solicita inscripcion |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver PromotorId del promotor autenticado |

---

### 2.3 AprobarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/AprobarInscripcionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionAprobadaDto>>`

No tiene body. Toda la informacion viene de la ruta y el token.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa (para verificar ownership) |
| `InscripcionId` | `Guid` | Si | Path param `inscripcionId` | Id del PromoProgramaPromotor a aprobar |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

---

### 2.4 RechazarInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/RechazarInscripcionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionRechazadaDto>>`

No tiene body. Toda la informacion viene de la ruta y el token.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa (para verificar ownership) |
| `InscripcionId` | `Guid` | Si | Path param `inscripcionId` | Id del PromoProgramaPromotor a rechazar (se eliminara fisicamente) |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

---

### 2.5 BloquearInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/BloquearInscripcionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionBloqueadaDto>>`

No tiene body. Toda la informacion viene de la ruta y el token.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa (para verificar ownership) |
| `InscripcionId` | `Guid` | Si | Path param `inscripcionId` | Id del PromoProgramaPromotor a bloquear |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

---

### 2.6 DarDeBajaInscripcionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Commands/DarDeBajaInscripcionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionDadaDeBajaDto>>`

No tiene body. Toda la informacion viene de la ruta y el token.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa (para verificar ownership) |
| `InscripcionId` | `Guid` | Si | Path param `inscripcionId` | Id del PromoProgramaPromotor a dar de baja |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |

---

### 2.7 GetInscripcionesProgramaQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/GetInscripcionesProgramaQuery.cs`

**Implementa:** `IRequest<ServiceResponse<InscripcionListResultDto>>`

GET con query params. No tiene body.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Path param `programaId` | Id del programa cuyas inscripciones se consultan |
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver ArtistaId y verificar ownership |
| `Estado` | `string?` | No | Query param `estado` | Filtrar: `Pendiente`, `Aprobado`, `Bloqueado`, `DadoDeBaja`. Sin valor: todos |
| `Page` | `int` | No | Query param `page` | Default 1 |
| `PageSize` | `int` | No | Query param `pageSize` | Default 20, max 50 |

---

### 2.8 GetMisInscripcionesQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Queries/GetMisInscripcionesQuery.cs`

**Implementa:** `IRequest<ServiceResponse<MisInscripcionesResultDto>>`

GET con query params. No tiene body.

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| `UserId` | `string` | Si | JWT claim `sub` | Para resolver PromotorId del promotor autenticado |
| `Page` | `int` | No | Query param `page` | Default 1 |
| `PageSize` | `int` | No | Query param `pageSize` | Default 10, max 50 |

---

## 3. Response DTOs

### 3.1 DTOs de Response - Promotor

#### 3.1.1 ProgramaExplorarItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaExplorarItemDto.cs`

Item individual del catalogo publico de programas.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Identificador del PromoPrograma |
| `Titulo` | `string` | Titulo del programa |
| `ArtistaNombre` | `string` | Nombre artistico, resuelto via join `PromoPrograma.ArtistaId -> Artista.NombreArtistico` |
| `TipoPromoId` | `int` | ID del tipo de programa |
| `TipoPromoNombre` | `string` | Nombre del tipo (ej: "Referral", "Afiliado") |
| `ImporteComisionPorcentaje` | `decimal?` | Porcentaje de comision. Null si usa comision fija |
| `ImporteComisionFija` | `decimal?` | Comision fija. Null si usa porcentaje |
| `MonedaNombre` | `string?` | Nombre de la moneda (ej: "EUR") |
| `NumeroTareas` | `int` | COUNT de `PromoTarea` con `EsActivo == true` para este programa |
| `CampaniaTitulo` | `string?` | Titulo de la campana vinculada. Null si no hay campana |
| `FechaInicio` | `DateTime?` | Fecha de inicio del programa |
| `FechaFin` | `DateTime?` | Fecha de fin del programa |
| `MiEstado` | `string?` | Estado de inscripcion del promotor autenticado. Null si no inscrito. Valores: `"Pendiente"`, `"Aprobado"`, `"Bloqueado"`, `"DadoDeBaja"` |

#### 3.1.2 ExplorarProgramasResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ExplorarProgramasResultDto.cs`

Wrapper paginado para el catalogo.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Items` | `List<ProgramaExplorarItemDto>` | Lista de programas de la pagina actual |
| `TotalCount` | `int` | Total de programas activos (para calcular paginas) |
| `Page` | `int` | Pagina actual |
| `PageSize` | `int` | Elementos por pagina |
| `TotalPages` | `int` | Total de paginas calculado: `(int)Math.Ceiling((double)TotalCount / PageSize)` |

#### 3.1.3 InscripcionCreadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionCreadaDto.cs`

Resultado de solicitar inscripcion en un programa (HTTP 201).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor creado |
| `ProgramaId` | `Guid` | Id del programa en que se inscribio |
| `ProgramaTitulo` | `string` | Titulo del programa |
| `EsAprobado` | `bool` | Siempre `false` al crear (pendiente de aprobacion) |
| `EsBloqueado` | `bool` | Siempre `false` al crear |
| `FechaAlta` | `DateTime` | Fecha de creacion del registro. Mapeado desde `PromoProgramaPromotor.FechaInscripcion` |

#### 3.1.4 MiInscripcionDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MiInscripcionDto.cs`

Item del listado de inscripciones del promotor autenticado.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor |
| `ProgramaId` | `Guid` | Id del PromoPrograma |
| `ProgramaTitulo` | `string` | Titulo del programa |
| `ArtistaNombre` | `string` | Nombre artistico del artista propietario del programa |
| `TipoPromoNombre` | `string` | Nombre del tipo de programa |
| `ImporteComisionPorcentaje` | `decimal?` | Comision porcentaje del programa |
| `ImporteComisionFija` | `decimal?` | Comision fija del programa |
| `MonedaNombre` | `string?` | Moneda del programa |
| `EsAprobado` | `bool` | Estado de aprobacion |
| `EsBloqueado` | `bool` | Estado de bloqueo |
| `CodigoReferido` | `string?` | Solo con valor cuando `EsAprobado == true` y `FechaBaja == null`. Null en cualquier otro estado |
| `UrlTrackingPersonalizada` | `string?` | Solo con valor cuando `EsAprobado == true` y `FechaBaja == null`. Mapeado desde `UrlReferido`. Null en cualquier otro estado |
| `FechaAlta` | `DateTime` | Fecha de inscripcion. Mapeado desde `PromoProgramaPromotor.FechaInscripcion` |
| `FechaBaja` | `DateTime?` | Fecha de baja si fue dado de baja |
| `Estado` | `string` | Calculado: `EsBloqueado=true` -> `"Bloqueado"` / `FechaBaja!=null` -> `"DadoDeBaja"` / `EsAprobado=true` -> `"Aprobado"` / default -> `"Pendiente"` |
| `Tareas` | `List<TareaResumenDto>` | Tareas activas del programa. Solo para `EsAprobado == true`. Array vacio en cualquier otro estado |

#### 3.1.5 TareaResumenDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareaResumenDto.cs`

Resumen de una tarea, incluido en MiInscripcionDto para inscripciones aprobadas.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id de la PromoTarea |
| `Titulo` | `string` | Titulo de la tarea |
| `Descripcion` | `string?` | Descripcion de la tarea |
| `TipoEventoPromoNombre` | `string` | Nombre del tipo de evento (ej: "Share") |
| `ImporteRecompensa` | `decimal?` | Importe de recompensa monetaria |
| `MonedaNombre` | `string?` | Moneda de la recompensa |
| `EsRepetible` | `bool` | Si la tarea se puede completar mas de una vez |
| `MaxRepeticiones` | `int?` | Maximo de repeticiones permitidas |
| `Orden` | `int` | Orden de la tarea en el programa |

#### 3.1.6 MisInscripcionesResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisInscripcionesResultDto.cs`

Wrapper paginado para el listado de inscripciones del promotor.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Items` | `List<MiInscripcionDto>` | Lista de inscripciones de la pagina actual |
| `TotalCount` | `int` | Total de inscripciones del promotor |
| `Page` | `int` | Pagina actual |
| `PageSize` | `int` | Elementos por pagina |
| `TotalPages` | `int` | Total de paginas |

---

### 3.2 DTOs de Response - Artista

#### 3.2.1 InscripcionAprobadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionAprobadaDto.cs`

Resultado de aprobar una inscripcion.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor aprobado |
| `PromotorNombre` | `string` | Nombre publico del promotor |
| `EsAprobado` | `bool` | Siempre `true` tras la aprobacion |
| `EsBloqueado` | `bool` | Siempre `false` tras la aprobacion |
| `CodigoReferido` | `string` | Codigo generado: `{CodigoTrackingBase}-{ShortId5chars}` |
| `UrlTrackingPersonalizada` | `string?` | URL generada con UTMs. Null si `PromoPrograma.UrlLanding` es null |

#### 3.2.2 InscripcionRechazadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionRechazadaDto.cs`

Resultado de rechazar (y eliminar fisicamente) una inscripcion.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `InscripcionId` | `Guid` | Id del PromoProgramaPromotor que fue eliminado |
| `PromotorNombre` | `string` | Nombre publico del promotor rechazado |

#### 3.2.3 InscripcionBloqueadaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionBloqueadaDto.cs`

Resultado de bloquear a un promotor.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor bloqueado |
| `PromotorNombre` | `string` | Nombre publico del promotor |
| `EsBloqueado` | `bool` | Siempre `true` tras el bloqueo |
| `EsAprobado` | `bool` | Siempre `false` tras el bloqueo (si estaba aprobado, se revoca) |

#### 3.2.4 InscripcionDadaDeBajaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionDadaDeBajaDto.cs`

Resultado de dar de baja a un promotor aprobado.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor dado de baja |
| `PromotorNombre` | `string` | Nombre publico del promotor |
| `EsAprobado` | `bool` | Siempre `false` tras la baja |
| `FechaBaja` | `DateTime` | Timestamp de la baja (`DateTime.UtcNow`) |

#### 3.2.5 InscripcionListItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionListItemDto.cs`

Item del listado de inscripciones de un programa (vista del artista).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Id del PromoProgramaPromotor |
| `PromotorId` | `Guid` | Id del Promotor |
| `PromotorNombre` | `string` | Nombre publico del promotor |
| `TipoPromotorNombre` | `string` | Tipo de promotor (ej: "Influencer") |
| `PromotorEmailContacto` | `string?` | Email de contacto del promotor |
| `PromotorUrlInstagram` | `string?` | URL de Instagram del promotor |
| `PromotorUrlTikTok` | `string?` | URL de TikTok del promotor |
| `PromotorUrlSitioWeb` | `string?` | URL del sitio web del promotor |
| `EsAprobado` | `bool` | Estado de aprobacion |
| `EsBloqueado` | `bool` | Estado de bloqueo |
| `CodigoReferido` | `string?` | Solo con valor cuando `EsAprobado == true` |
| `FechaAlta` | `DateTime` | Fecha de inscripcion. Mapeado desde `FechaInscripcion` |
| `FechaBaja` | `DateTime?` | Fecha de baja si aplica |
| `Estado` | `string` | Calculado igual que en `MiInscripcionDto` |

#### 3.2.6 InscripcionListResultDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionListResultDto.cs`

Wrapper paginado del listado de inscripciones de un programa (vista del artista).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Items` | `List<InscripcionListItemDto>` | Lista de inscripciones de la pagina actual |
| `TotalCount` | `int` | Total de inscripciones con el filtro aplicado |
| `Page` | `int` | Pagina actual |
| `PageSize` | `int` | Elementos por pagina |
| `TotalPages` | `int` | Total de paginas |

---

## 4. Validadores

Todos los Commands de esta feature no tienen body de request (todos los parametros vienen del token o de la ruta). Por este motivo, los validators son ligeros y solo validan la presencia del `UserId` extraido del token. La logica de negocio (promotor activo, programa activo, ownership, estado de inscripcion) se valida en el Handler, no en el Validator, porque requiere acceso a base de datos a traves del Service.

### 4.1 SolicitarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/SolicitarInscripcionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty` | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty` (Guid no vacio) | El identificador del programa es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |

### 4.2 AprobarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/AprobarInscripcionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty` | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty` | El identificador del programa es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `InscripcionId` | `NotEmpty` | El identificador de la inscripcion es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |

### 4.3 RechazarInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/RechazarInscripcionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty` | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty` | El identificador del programa es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `InscripcionId` | `NotEmpty` | El identificador de la inscripcion es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |

### 4.4 BloquearInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/BloquearInscripcionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty` | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty` | El identificador del programa es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `InscripcionId` | `NotEmpty` | El identificador de la inscripcion es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |

### 4.5 DarDeBajaInscripcionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Inscripcion/Validators/DarDeBajaInscripcionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `UserId` | `NotEmpty` | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `ProgramaId` | `NotEmpty` | El identificador del programa es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |
| `InscripcionId` | `NotEmpty` | El identificador de la inscripcion es obligatorio | `ServiceResponseMessageType.Validation_Required` ("1001") |

---

## 5. Logica de Negocio en Handlers

Cada handler debe seguir el flujo estandar del proyecto: Validacion -> Resolucion de identidad -> Verificaciones de negocio -> Operacion -> Respuesta.

### 5.1 SolicitarInscripcionCommandHandler

**Flujo de validaciones en orden:**

1. Validar `SolicitarInscripcionCommandValidator`
2. Resolver `PromotorId` via `IInscripcionService.GetPromotorIdByUserIdAsync(UserId)`. Si null -> `NotFound_Promotor` (2015)
3. Verificar `Promotor.EsActivo == true`. Si false -> `BusinessRule_PromotorInactivo` (4023) HTTP 400
4. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
5. Verificar `PromoPrograma.EsActivo == true`. Si false -> `BusinessRule_ProgramaInactivo` (4024) HTTP 400
6. Verificar no existe `PromoProgramaPromotor` para este `PromotorId` + `ProgramaId`. Si existe y `EsBloqueado == true` -> `BusinessRule_InscripcionBloqueada` (4022) HTTP 403. Si existe sin bloqueo -> `BusinessRule_InscripcionAlreadyExists` (4021) HTTP 400
7. Crear `PromoProgramaPromotor` con: `EsAprobado = false`, `EsBloqueado = false`, `FechaInscripcion = DateTime.UtcNow`, `EsActivo = true`
8. Persistir via `IInscripcionService.CreateAsync(...)`
9. Retornar `ServiceResponse<InscripcionCreadaDto>` con errorCode `ServiceResponseMessageType.Created` ("0001"), HTTP 201

### 5.2 AprobarInscripcionCommandHandler

**Flujo de validaciones en orden:**

1. Validar `AprobarInscripcionCommandValidator`
2. Resolver `ArtistaId` via `IInscripcionService.GetArtistaIdByUserIdAsync(UserId)`. Si null -> `NotFound_Artista` (2016) HTTP 404
3. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
4. Verificar `PromoPrograma.ArtistaId == artista.Id`. Si no coincide -> `BusinessRule_NoEsPropietarioPrograma` (4026) HTTP 403
5. Resolver inscripcion `PromoProgramaPromotor` por `InscripcionId`. Si null -> `NotFound_Inscripcion` (2020) HTTP 404
6. Verificar estado pendiente: `EsAprobado == false && EsBloqueado == false && FechaBaja == null`. Si no -> `BusinessRule_InscripcionEstadoInvalido` (4025) HTTP 400
7. Generar `CodigoReferido`: `{PromoPrograma.CodigoTrackingBase ?? ProgramaId.ToString()[..8]}-{ShortId(5 chars alfanumericos)}`. Reintentar si colision de unicidad (maximo 3 intentos)
8. Generar `UrlTrackingPersonalizada`: si `PromoPrograma.UrlLanding != null` -> `{UrlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={CodigoTrackingBase}&ref={CodigoReferido}`. Si null -> null
9. Actualizar inscripcion: `EsAprobado = true`, `CodigoReferido = generado`, `UrlReferido = urlGenerada`
10. Persistir via `IInscripcionService.AprobarAsync(...)`
11. Retornar `ServiceResponse<InscripcionAprobadaDto>` con errorCode `ServiceResponseMessageType.Updated` ("0002"), HTTP 200

**Nota logica de generacion de ShortId:** Usar `Guid.NewGuid().ToString("N")[..5]` o similar helper de 5 chars alfanumericos. La generacion y verificacion de unicidad debe ocurrir dentro del mismo scope de transaccion del Service para cumplir RNF-02.

### 5.3 RechazarInscripcionCommandHandler

**Flujo de validaciones en orden:**

1. Validar `RechazarInscripcionCommandValidator`
2. Resolver `ArtistaId`. Si null -> `NotFound_Artista` (2016) HTTP 404
3. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
4. Verificar ownership del artista. Si no coincide -> `BusinessRule_NoEsPropietarioPrograma` (4026) HTTP 403
5. Resolver inscripcion por `InscripcionId`. Si null -> `NotFound_Inscripcion` (2020) HTTP 404
6. Verificar estado pendiente: `EsAprobado == false && EsBloqueado == false && FechaBaja == null`. Si no -> `BusinessRule_InscripcionEstadoInvalido` (4025) HTTP 400
7. Guardar `PromotorNombre` antes de eliminar (para el DTO de respuesta)
8. Eliminar fisicamente el registro via `IInscripcionService.RechazarAsync(InscripcionId)` (DELETE fisico, no soft-delete)
9. Retornar `ServiceResponse<InscripcionRechazadaDto>` con errorCode `ServiceResponseMessageType.Deleted` ("0003"), HTTP 200

### 5.4 BloquearInscripcionCommandHandler

**Flujo de validaciones en orden:**

1. Validar `BloquearInscripcionCommandValidator`
2. Resolver `ArtistaId`. Si null -> `NotFound_Artista` (2016) HTTP 404
3. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
4. Verificar ownership del artista. Si no coincide -> `BusinessRule_NoEsPropietarioPrograma` (4026) HTTP 403
5. Resolver inscripcion por `InscripcionId`. Si null -> `NotFound_Inscripcion` (2020) HTTP 404
6. Verificar que no este ya bloqueada: `EsBloqueado == false`. Si ya bloqueada -> `BusinessRule_InscripcionEstadoInvalido` (4025) HTTP 400, mensaje "Este promotor ya esta bloqueado"
7. Actualizar: `EsBloqueado = true`. Si ademas `EsAprobado == true`, tambien establecer `EsAprobado = false` (desactivar codigo)
8. Persistir via `IInscripcionService.BloquearAsync(...)`
9. Retornar `ServiceResponse<InscripcionBloqueadaDto>` con errorCode `ServiceResponseMessageType.Updated` ("0002"), HTTP 200

### 5.5 DarDeBajaInscripcionCommandHandler

**Flujo de validaciones en orden:**

1. Validar `DarDeBajaInscripcionCommandValidator`
2. Resolver `ArtistaId`. Si null -> `NotFound_Artista` (2016) HTTP 404
3. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
4. Verificar ownership del artista. Si no coincide -> `BusinessRule_NoEsPropietarioPrograma` (4026) HTTP 403
5. Resolver inscripcion por `InscripcionId`. Si null -> `NotFound_Inscripcion` (2020) HTTP 404
6. Verificar estado aprobado: `EsAprobado == true && FechaBaja == null`. Si no -> `BusinessRule_InscripcionEstadoInvalido` (4025) HTTP 400, mensaje "Esta inscripcion no esta en estado aprobado"
7. Actualizar: `FechaBaja = DateTime.UtcNow`, `EsAprobado = false`
8. Persistir via `IInscripcionService.DarDeBajaAsync(...)`
9. Retornar `ServiceResponse<InscripcionDadaDeBajaDto>` con errorCode `ServiceResponseMessageType.Updated` ("0002"), HTTP 200

### 5.6 ExplorarProgramasQueryHandler

**Flujo:**

1. Resolver `PromotorId` via `IInscripcionService.GetPromotorIdByUserIdAsync(UserId)`. Si null -> `NotFound_Promotor` (2015) HTTP 404
2. Llamar a `IInscripcionService.GetProgramasActivosAsync(ArtistaNombre, TipoPromoId, Page, PageSize, PromotorId, ct)` que retorna `(IReadOnlyList<PromoPrograma> Items, int TotalCount)`
3. Para cada item, calcular `MiEstado` desde la inscripcion del promotor si existe
4. Mapear a `List<ProgramaExplorarItemDto>` via AutoMapper + campos calculados
5. Retornar `ServiceResponse<ExplorarProgramasResultDto>`, HTTP 200

### 5.7 GetInscripcionesProgramaQueryHandler

**Flujo:**

1. Resolver `ArtistaId`. Si null -> `NotFound_Artista` (2016) HTTP 404
2. Resolver `PromoPrograma` por `ProgramaId`. Si null -> `NotFound_PromoPrograma` (2019) HTTP 404
3. Verificar ownership. Si no coincide -> `BusinessRule_NoEsPropietarioPrograma` (4026) HTTP 403
4. Llamar a `IInscripcionService.GetInscripcionesPorProgramaAsync(ProgramaId, Estado, Page, PageSize, ct)`
5. Para cada inscripcion, calcular campo `Estado` segun prioridad: `EsBloqueado > FechaBaja != null > EsAprobado > default Pendiente`
6. Mapear a `List<InscripcionListItemDto>`
7. Retornar `ServiceResponse<InscripcionListResultDto>`, HTTP 200

### 5.8 GetMisInscripcionesQueryHandler

**Flujo:**

1. Resolver `PromotorId`. Si null -> `NotFound_Promotor` (2015) HTTP 404
2. Llamar a `IInscripcionService.GetMisInscripcionesAsync(PromotorId, Page, PageSize, ct)` que incluye las tareas activas para inscripciones aprobadas
3. Para cada inscripcion: calcular `Estado`, y si `EsAprobado == true && FechaBaja == null` incluir `CodigoReferido` y `UrlTrackingPersonalizada` (UrlReferido), si no -> null
4. Para cada inscripcion: si `EsAprobado == true` incluir `Tareas` activas, si no -> lista vacia
5. Mapear a `List<MiInscripcionDto>`
6. Retornar `ServiceResponse<MisInscripcionesResultDto>`, HTTP 200

---

## 6. Service Interface

El servicio de inscripciones debe ser un servicio nuevo dedicado a esta feature, separado de `IPromoProgramaService` y `IPromotorService`.

### 6.1 IInscripcionService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IInscripcionService.cs`

```
Metodos requeridos:

// Resolucion de identidad (con cache via IRequestCacheService)
Task<PromotorId?> GetPromotorIdByUserIdAsync(string userId, CancellationToken ct)
Task<ArtistaId?> GetArtistaIdByUserIdAsync(string userId, CancellationToken ct)

// Operaciones del promotor
Task<(IReadOnlyList<PromoPrograma> Items, int TotalCount)> GetProgramasActivosAsync(
    string? artistaNombre, int? tipoPromoId, int page, int pageSize, PromotorId promotorId, CancellationToken ct)
Task<PromoProgramaPromotor?> GetByPromotorYProgramaAsync(
    PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct)
Task<Guid> CreateAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetMisInscripcionesAsync(
    PromotorId promotorId, int page, int pageSize, CancellationToken ct)

// Operaciones del artista
Task<PromoProgramaPromotor?> GetByIdAsync(Guid id, CancellationToken ct)
Task AprobarAsync(PromoProgramaPromotor inscripcion, string codigoReferido, string? urlTracking, CancellationToken ct)
Task RechazarAsync(Guid inscripcionId, CancellationToken ct)  // DELETE fisico
Task BloquearAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
Task DarDeBajaAsync(PromoProgramaPromotor inscripcion, CancellationToken ct)
Task<(IReadOnlyList<PromoProgramaPromotor> Items, int TotalCount)> GetInscripcionesPorProgramaAsync(
    PromoProgramaId programaId, string? estado, int page, int pageSize, CancellationToken ct)

// Verificacion de unicidad de CodigoReferido
Task<bool> CodigoReferidoExistsAsync(string codigoReferido, CancellationToken ct)
```

---

## 7. AutoMapper Mappings

### 7.1 InscripcionProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/InscripcionProfile.cs`

| Source | Destination | Notas especiales |
|--------|-------------|------------------|
| `PromoProgramaPromotor` | `InscripcionCreadaDto` | `FechaAlta <- FechaInscripcion`; `ProgramaTitulo` se ignora en mapping (se asigna en handler desde entidad `PromoPrograma`) |
| `PromoProgramaPromotor` | `InscripcionAprobadaDto` | `PromotorNombre` ignorado en mapping (asignado en handler desde `Promotor.NombrePublico`); `UrlTrackingPersonalizada <- UrlReferido` |
| `PromoProgramaPromotor` | `InscripcionRechazadaDto` | `InscripcionId <- Id`; `PromotorNombre` ignorado en mapping (asignado en handler) |
| `PromoProgramaPromotor` | `InscripcionBloqueadaDto` | `PromotorNombre` ignorado en mapping |
| `PromoProgramaPromotor` | `InscripcionDadaDeBajaDto` | `PromotorNombre` ignorado en mapping |
| `PromoProgramaPromotor` | `InscripcionListItemDto` | `FechaAlta <- FechaInscripcion`; `PromotorNombre`, `TipoPromotorNombre`, `PromotorEmail*`, `PromotorUrl*` ignorados (asignados en handler desde `Promotor`); `Estado` ignorado (calculado en handler); `CodigoReferido` ignorado (asignado condicionalmente en handler) |
| `PromoProgramaPromotor` | `MiInscripcionDto` | `FechaAlta <- FechaInscripcion`; `UrlTrackingPersonalizada <- UrlReferido`; `ArtistaNombre`, `TipoPromoNombre`, `MonedaNombre`, `Estado` ignorados (asignados en handler); `CodigoReferido` y `UrlTrackingPersonalizada` ignorados (asignados condicionalmente en handler); `Tareas` ignorada (asignada en handler) |
| `PromoPrograma` | `ProgramaExplorarItemDto` | `Id <- Id.Value`; `ArtistaNombre`, `TipoPromoNombre`, `MonedaNombre`, `NumeroTareas`, `CampaniaTitulo`, `MiEstado` ignorados (asignados en handler) |
| `PromoTarea` | `TareaResumenDto` | `TipoEventoPromoNombre`, `MonedaNombre` ignorados (asignados en handler) |

**Detalle de ForMember necesarios:**

Para `PromoProgramaPromotor -> InscripcionCreadaDto`:
- `.ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))`
- `.ForMember(dest => dest.ProgramaId, opt => opt.MapFrom(src => src.ProgramaId.Value))`
- `.ForMember(dest => dest.ProgramaTitulo, opt => opt.Ignore())` (asignado desde Programa.Titulo en handler)

Para `PromoProgramaPromotor -> InscripcionAprobadaDto`:
- `.ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.MapFrom(src => src.UrlReferido))`
- `.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())`

Para `PromoProgramaPromotor -> InscripcionRechazadaDto`:
- `.ForMember(dest => dest.InscripcionId, opt => opt.MapFrom(src => src.Id))`
- `.ForMember(dest => dest.PromotorNombre, opt => opt.Ignore())`

Para `PromoProgramaPromotor -> MiInscripcionDto`:
- `.ForMember(dest => dest.FechaAlta, opt => opt.MapFrom(src => src.FechaInscripcion))`
- `.ForMember(dest => dest.UrlTrackingPersonalizada, opt => opt.Ignore())` (asignado condicionalmente en handler)
- `.ForMember(dest => dest.CodigoReferido, opt => opt.Ignore())` (asignado condicionalmente en handler)
- `.ForMember(dest => dest.Estado, opt => opt.Ignore())`
- `.ForMember(dest => dest.Tareas, opt => opt.Ignore())`
- Todos los campos de Programa y Artista ignorados

Para `PromoPrograma -> ProgramaExplorarItemDto`:
- `.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))`
- `.ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => src.FechaInicio))`
- `.ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => src.FechaFin))`
- Todos los campos calculados (ArtistaNombre, TipoPromoNombre, etc.) ignorados

---

## 8. Controller

### 8.1 InscripcionController

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/InscripcionController.cs`

**Route base:** `[Route("api/crowdpromotion")]`

El controller tiene dos grupos de rutas:
- Rutas de programa: `/programas/{programaId}/...`
- Ruta de promotor: `/promotor/mis-programas`

**Acciones del controller:**

| Action method | HTTP verb + route | Parametros |
|---------------|-------------------|------------|
| `ExplorarProgramas` | `GET /api/crowdpromotion/programas/explorar` | `[FromQuery] string? artistaNombre, [FromQuery] int? tipoPromoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10` |
| `SolicitarInscripcion` | `POST /api/crowdpromotion/programas/{programaId}/inscripcion` | `[FromRoute] Guid programaId` |
| `AprobarInscripcion` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar` | `[FromRoute] Guid programaId, [FromRoute] Guid inscripcionId` |
| `RechazarInscripcion` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/rechazar` | `[FromRoute] Guid programaId, [FromRoute] Guid inscripcionId` |
| `BloquearInscripcion` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/bloquear` | `[FromRoute] Guid programaId, [FromRoute] Guid inscripcionId` |
| `DarDeBajaInscripcion` | `PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja` | `[FromRoute] Guid programaId, [FromRoute] Guid inscripcionId` |
| `GetInscripciones` | `GET /api/crowdpromotion/programas/{programaId}/inscripciones` | `[FromRoute] Guid programaId, [FromQuery] string? estado, [FromQuery] int page = 1, [FromQuery] int pageSize = 20` |
| `GetMisInscripciones` | `GET /api/crowdpromotion/promotor/mis-programas` | `[FromQuery] int page = 1, [FromQuery] int pageSize = 10` |

**Patron de construccion del Command/Query:** Igual que en `PromoProgramaController`: el controller extrae `UserId` de `_currentUser.UserId`, retorna `Unauthorized()` si es null, construye el Command/Query con los parametros y llama `_mediator.Send(...)`. Para los resultados exitosos de `SolicitarInscripcion`, retornar `Created(...)` con HTTP 201. El resto con `FromServiceResponse(...)`.

---

## 9. OpenAPI / Swagger

### GET /api/crowdpromotion/programas/explorar

- **Summary:** Catalogo publico paginado de programas activos con estado de inscripcion del promotor
- **Auth:** Bearer JWT requerido
- **Query params:** `artistaNombre` (string?), `tipoPromoId` (int?), `page` (int, default 1), `pageSize` (int, default 10, max 50)
- **Responses:**
  - 200: `ServiceResponse<ExplorarProgramasResultDto>` - Listado paginado con `miEstado` calculado
  - 401: No autenticado o token invalido (ErrorCode 3001)
  - 404: No existe perfil de promotor para el usuario del token (ErrorCode 2015)
  - 500: Error interno (ErrorCode 5000)

### POST /api/crowdpromotion/programas/{programaId}/inscripcion

- **Summary:** Solicitar inscripcion en un programa de promocion
- **Auth:** Bearer JWT requerido - Promotor activo
- **Path params:** `programaId` (Guid)
- **Request body:** Ninguno
- **Responses:**
  - 201: `ServiceResponse<InscripcionCreadaDto>` - Inscripcion creada con estado Pendiente (ErrorCode 0001)
  - 400: Promotor inactivo (ErrorCode 4023), programa inactivo (ErrorCode 4024), ya inscrito (ErrorCode 4021)
  - 401: No autenticado (ErrorCode 3001)
  - 403: Promotor bloqueado en este programa (ErrorCode 4022)
  - 404: Perfil de promotor no encontrado (ErrorCode 2015), programa no encontrado (ErrorCode 2019)
  - 500: Error interno (ErrorCode 5000)

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar

- **Summary:** Aprobar solicitud de inscripcion y generar codigo referido y URL de tracking
- **Auth:** Bearer JWT requerido - Artista propietario del programa
- **Path params:** `programaId` (Guid), `inscripcionId` (Guid)
- **Request body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<InscripcionAprobadaDto>` - Inscripcion aprobada con codigos generados (ErrorCode 0002)
  - 400: Inscripcion no en estado pendiente (ErrorCode 4025)
  - 401: No autenticado (ErrorCode 3001)
  - 403: No es propietario del programa (ErrorCode 4026)
  - 404: Perfil de artista no encontrado (ErrorCode 2016), programa no encontrado (ErrorCode 2019), inscripcion no encontrada (ErrorCode 2020)
  - 500: Error interno (ErrorCode 5000)

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/rechazar

- **Summary:** Rechazar solicitud de inscripcion pendiente (elimina fisicamente el registro)
- **Auth:** Bearer JWT requerido - Artista propietario del programa
- **Path params:** `programaId` (Guid), `inscripcionId` (Guid)
- **Request body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<InscripcionRechazadaDto>` - Inscripcion rechazada y eliminada (ErrorCode 0003)
  - 400: Inscripcion no en estado pendiente (ErrorCode 4025)
  - 401: No autenticado (ErrorCode 3001)
  - 403: No es propietario del programa (ErrorCode 4026)
  - 404: Artista no encontrado (ErrorCode 2016), programa no encontrado (ErrorCode 2019), inscripcion no encontrada (ErrorCode 2020)
  - 500: Error interno (ErrorCode 5000)

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/bloquear

- **Summary:** Bloquear promotor en un programa (impide re-solicitud futura)
- **Auth:** Bearer JWT requerido - Artista propietario del programa
- **Path params:** `programaId` (Guid), `inscripcionId` (Guid)
- **Request body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<InscripcionBloqueadaDto>` - Promotor bloqueado (ErrorCode 0002)
  - 400: Promotor ya esta bloqueado (ErrorCode 4025)
  - 401: No autenticado (ErrorCode 3001)
  - 403: No es propietario del programa (ErrorCode 4026)
  - 404: Artista no encontrado (ErrorCode 2016), programa no encontrado (ErrorCode 2019), inscripcion no encontrada (ErrorCode 2020)
  - 500: Error interno (ErrorCode 5000)

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja

- **Summary:** Dar de baja a un promotor aprobado (desactiva codigo referido, mantiene historico)
- **Auth:** Bearer JWT requerido - Artista propietario del programa
- **Path params:** `programaId` (Guid), `inscripcionId` (Guid)
- **Request body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<InscripcionDadaDeBajaDto>` - Promotor dado de baja (ErrorCode 0002)
  - 400: Inscripcion no esta en estado aprobado (ErrorCode 4025)
  - 401: No autenticado (ErrorCode 3001)
  - 403: No es propietario del programa (ErrorCode 4026)
  - 404: Artista no encontrado (ErrorCode 2016), programa no encontrado (ErrorCode 2019), inscripcion no encontrada (ErrorCode 2020)
  - 500: Error interno (ErrorCode 5000)

### GET /api/crowdpromotion/programas/{programaId}/inscripciones

- **Summary:** Listar inscripciones de un programa con filtro por estado (vista del artista)
- **Auth:** Bearer JWT requerido - Artista propietario del programa
- **Path params:** `programaId` (Guid)
- **Query params:** `estado` (string?, valores: Pendiente|Aprobado|Bloqueado|DadoDeBaja), `page` (int, default 1), `pageSize` (int, default 20, max 50)
- **Responses:**
  - 200: `ServiceResponse<InscripcionListResultDto>` - Listado paginado con datos de perfil de promotor
  - 401: No autenticado (ErrorCode 3001)
  - 403: No es propietario del programa (ErrorCode 4026)
  - 404: Artista no encontrado (ErrorCode 2016), programa no encontrado (ErrorCode 2019)
  - 500: Error interno (ErrorCode 5000)

### GET /api/crowdpromotion/promotor/mis-programas

- **Summary:** Listar inscripciones del promotor autenticado con tareas para inscripciones aprobadas
- **Auth:** Bearer JWT requerido - Promotor
- **Query params:** `page` (int, default 1), `pageSize` (int, default 10, max 50)
- **Responses:**
  - 200: `ServiceResponse<MisInscripcionesResultDto>` - Listado con estado, codigos referido (solo aprobadas) y tareas
  - 401: No autenticado (ErrorCode 3001)
  - 404: Perfil de promotor no encontrado (ErrorCode 2015)
  - 500: Error interno (ErrorCode 5000)

---

## 10. Tabla de Errores Consolidada

| HTTP | ErrorCode | Constante | Descripcion |
|------|-----------|-----------|-------------|
| 401 | 3001 | `Auth_Unauthorized` | Token invalido o expirado |
| 403 | 4022 | `BusinessRule_InscripcionBloqueada` | Promotor bloqueado intenta solicitar inscripcion |
| 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | Artista del token no es propietario del programa |
| 400 | 4021 | `BusinessRule_InscripcionAlreadyExists` | Ya existe inscripcion para este promotor + programa |
| 400 | 4023 | `BusinessRule_PromotorInactivo` | Perfil de promotor desactivado |
| 400 | 4024 | `BusinessRule_ProgramaInactivo` | Programa no esta activo |
| 400 | 4025 | `BusinessRule_InscripcionEstadoInvalido` | Estado de inscripcion no valido para la operacion solicitada |
| 400 | 1001 | `Validation_Required` | Campo obligatorio vacio (validador) |
| 404 | 2015 | `NotFound_Promotor` | No existe perfil de promotor para el UserId del token |
| 404 | 2016 | `NotFound_Artista` | No existe perfil de artista para el UserId del token |
| 404 | 2019 | `NotFound_PromoPrograma` | No existe PromoPrograma con el programaId proporcionado |
| 404 | 2020 | `NotFound_Inscripcion` | No existe PromoProgramaPromotor con el inscripcionId proporcionado |
| 500 | 5000 | `Internal_UnexpectedError` | Excepcion no controlada |

---

## 11. Archivos a Crear

```
src/api/Modules/Crowdpromotion/
|
+-- WePlayRises.Crowdpromotion.Domain/
|   +-- Model/
|   |   +-- PromoProgramaPromotor.cs               [MODIFICAR - agregar EsAprobado y EsBloqueado]
|   +-- Constants/
|       +-- ServiceResponseMessageType.cs           [MODIFICAR - agregar 7 constantes nuevas]
|
+-- WePlayRises.Crowdpromotion.Application/
|   +-- Dtos/
|   |   +-- ProgramaExplorarItemDto.cs              [CREAR]
|   |   +-- ExplorarProgramasResultDto.cs           [CREAR]
|   |   +-- InscripcionCreadaDto.cs                 [CREAR]
|   |   +-- InscripcionAprobadaDto.cs               [CREAR]
|   |   +-- InscripcionRechazadaDto.cs              [CREAR]
|   |   +-- InscripcionBloqueadaDto.cs              [CREAR]
|   |   +-- InscripcionDadaDeBajaDto.cs             [CREAR]
|   |   +-- InscripcionListItemDto.cs               [CREAR]
|   |   +-- InscripcionListResultDto.cs             [CREAR]
|   |   +-- MiInscripcionDto.cs                     [CREAR]
|   |   +-- MisInscripcionesResultDto.cs            [CREAR]
|   |   +-- TareaResumenDto.cs                      [CREAR]
|   +-- Features/
|   |   +-- Inscripcion/
|   |       +-- Commands/
|   |       |   +-- SolicitarInscripcionCommand.cs  [CREAR - Command + Handler en mismo archivo]
|   |       |   +-- AprobarInscripcionCommand.cs    [CREAR - Command + Handler en mismo archivo]
|   |       |   +-- RechazarInscripcionCommand.cs   [CREAR - Command + Handler en mismo archivo]
|   |       |   +-- BloquearInscripcionCommand.cs   [CREAR - Command + Handler en mismo archivo]
|   |       |   +-- DarDeBajaInscripcionCommand.cs  [CREAR - Command + Handler en mismo archivo]
|   |       +-- Queries/
|   |       |   +-- ExplorarProgramasQuery.cs       [CREAR - Query + Handler en mismo archivo]
|   |       |   +-- GetInscripcionesProgramaQuery.cs [CREAR - Query + Handler en mismo archivo]
|   |       |   +-- GetMisInscripcionesQuery.cs     [CREAR - Query + Handler en mismo archivo]
|   |       +-- Validators/
|   |           +-- SolicitarInscripcionCommandValidator.cs  [CREAR]
|   |           +-- AprobarInscripcionCommandValidator.cs    [CREAR]
|   |           +-- RechazarInscripcionCommandValidator.cs   [CREAR]
|   |           +-- BloquearInscripcionCommandValidator.cs   [CREAR]
|   |           +-- DarDeBajaInscripcionCommandValidator.cs  [CREAR]
|   +-- Interfaces/
|   |   +-- Services/
|   |       +-- IInscripcionService.cs              [CREAR]
|   +-- Mapping/
|       +-- InscripcionProfile.cs                   [CREAR]
|
+-- WePlayRises.Crowdpromotion.Infra/
|   +-- Repositories/
|   |   +-- InscripcionRepository.cs               [CREAR - implementa IInscripcionRepository]
|   +-- Services/
|       +-- InscripcionService.cs                  [CREAR - implementa IInscripcionService]
|
+-- WePlayRises.Crowdpromotion.WebApi/
    +-- Controllers/
        +-- InscripcionController.cs               [CREAR]
```

**Nota sobre migracion:** Crear migracion EF Core para agregar `EsAprobado` y `EsBloqueado` a la tabla `PromoProgramaPromotor`. Nombre sugerido: `AddEsAprobadoEsBloqueadoToPromoProgramaPromotor`.

---

## 12. Checklist

- [ ] Entidad `PromoProgramaPromotor` actualizada con `EsAprobado` y `EsBloqueado` (default false)
- [ ] Constantes nuevas agregadas en `ServiceResponseMessageType` (7 constantes: 2020, 4021-4026)
- [ ] Migracion EF Core creada y aplicada
- [ ] `IInscripcionService` definido con todos los metodos requeridos
- [ ] Commands implementan `IRequest<ServiceResponse<T>>`
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command/Query en mismo archivo (regla CQRS)
- [ ] Handlers inyectan `IInscripcionService`, NO `DbContext` directamente
- [ ] Constructores con `?? throw new ArgumentNullException` para todas las dependencias
- [ ] Validators con `WithMessage()` Y `WithErrorCode()` usando constantes (no strings literales)
- [ ] Validacion en handlers retorna `ServiceResponse` (no throw)
- [ ] Handlers con try-catch y `_logger.LogError` en el catch
- [ ] Campo `Estado` calculado con prioridad: `EsBloqueado > FechaBaja != null > EsAprobado > Pendiente`
- [ ] `CodigoReferido` y `UrlTrackingPersonalizada` solo se exponen en respuesta cuando `EsAprobado == true && FechaBaja == null`
- [ ] `Tareas` solo incluidas en `MiInscripcionDto` cuando `EsAprobado == true`
- [ ] Rechazo hace DELETE fisico del registro (no soft-delete)
- [ ] Bloqueo de promotor aprobado tambien establece `EsAprobado = false`
- [ ] Generacion de `CodigoReferido` unica dentro de la misma transaccion (con reintento por colision)
- [ ] `InscripcionProfile` registrado en DI de `DependencyInjection.cs`
- [ ] `InscripcionService` e `IInscripcionService` registrados en DI
- [ ] Swagger documentation completa en controller con `[ProducesResponseType]` para todos los status codes
