# Contratos API: cp-tareas-promocion

**Fecha:** 2026-03-01
**Modulo:** Crowdpromotion
**Feature:** cp-tareas-promocion (US-CP-04)
**Basado en:** `docs/user-stories/cp-tareas-promocion/contracts.md`
**Depende de:** cp-inscripcion-programa (US-CP-03)

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Actor | Descripcion |
|--------|------|------|-------|-------------|
| GET | `/api/crowdpromotion/programas/{programaId}/mis-tareas` | Query | Promotor | Lista tareas activas del programa con estado personal del promotor |
| POST | `/api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar` | Command | Promotor | Envia completado de tarea con URL de prueba |
| GET | `/api/crowdpromotion/programas/{programaId}/tareas-pendientes` | Query | Artista | Lista paginada de completados pendientes de validacion |
| PATCH | `/api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar` | Command | Artista | Valida un completado y acredita recompensa |
| PATCH | `/api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar` | Command | Artista | Rechaza un completado con motivo obligatorio |

**Nota de controlador:** Todos los endpoints van en un nuevo `PromoTareaController`. La ruta base del controlador es `api/crowdpromotion/programas` para alinear con `PromoProgramaController` y `InscripcionController` existentes.

---

## 2. Nuevas Constantes en ServiceResponseMessageType

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Se agregan las siguientes constantes al archivo existente:

| Constante | Valor | Categoria | Uso |
|-----------|-------|-----------|-----|
| `NotFound_PromoTarea` | `"2021"` | NotFound (2000-2999) | No existe `PromoTarea` con el `tareaId` proporcionado |
| `NotFound_TareaPromotor` | `"2022"` | NotFound (2000-2999) | No existe `PromoTareaPromotor` con el `tareaPromotorId` proporcionado |
| `BusinessRule_TareaNoRepetible` | `"4027"` | Business Rules (4000-4999) | Tarea con `EsRepetible = false` ya tiene completado activo (estado 2 o 3) |
| `BusinessRule_MaxRepeticionesAlcanzado` | `"4028"` | Business Rules (4000-4999) | Tarea repetible con `vecesCompletada >= MaxRepeticiones` |
| `BusinessRule_TareaInactiva` | `"4029"` | Business Rules (4000-4999) | `PromoTarea.EsActivo = false` |
| `BusinessRule_TareaFechaFinPasada` | `"4030"` | Business Rules (4000-4999) | `PromoTarea.FechaFin != null && FechaFin < DateTime.UtcNow` |
| `BusinessRule_CompletadoEstadoInvalido` | `"4031"` | Business Rules (4000-4999) | `PromoTareaPromotor.EstadoTareaId != 2` al intentar validar o rechazar |

**Bloque C# a agregar al final de la seccion correspondiente:**

```csharp
// En la seccion NotFound (2000-2999), despues de NotFound_Inscripcion:
public const string NotFound_PromoTarea = "2021";
public const string NotFound_TareaPromotor = "2022";

// En la seccion Business Rules (4000-4999), despues de BusinessRule_NoEsPropietarioPrograma:
public const string BusinessRule_TareaNoRepetible = "4027";
public const string BusinessRule_MaxRepeticionesAlcanzado = "4028";
public const string BusinessRule_TareaInactiva = "4029";
public const string BusinessRule_TareaFechaFinPasada = "4030";
public const string BusinessRule_CompletadoEstadoInvalido = "4031";
```

---

## 3. Request DTOs (Commands y Queries)

### 3.1 GetMisTareasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Queries/GetMisTareasQuery.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries`

**Implementa:** `IRequest<ServiceResponse<MisTareasResponseDto>>`

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param | Identificador del programa |
| `UserId` | `string` | Si | JWT claim `sub` | Resuelto en el controlador via `ICurrentUserService` |

**Estructura C#:**
```csharp
public class GetMisTareasQuery : IRequest<ServiceResponse<MisTareasResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
}
```

**Notas:**
- No hay request body. Todos los datos vienen del path param y del token JWT.
- El Handler resuelve `PromotorId` y `ProgramaPromotorId` desde `UserId` via `IPromoTareaService`.
- No tiene validator propio: el `ProgramaId` se valida como existente en la logica del Handler.

---

### 3.2 CompletarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/CompletarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Implementa:** `IRequest<ServiceResponse<CompletarTareaResponseDto>>`

**Campos del Command (la suma de request body + path params + JWT):**

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param | Identificador del programa |
| `TareaId` | `Guid` | Si | Route param | Identificador de la tarea a completar |
| `UrlPruebaCompletado` | `string` | Si | Request body | URL valida. Max 2048 chars. |
| `ComentarioPromotor` | `string?` | No | Request body | Opcional. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` | Resuelto en el controlador |

**DTO de request body (`CompletarTareaDto`) - usado en el controlador como `[FromBody]`:**

| Campo | Tipo C# | Obligatorio | Constraints |
|-------|---------|-------------|-------------|
| `UrlPruebaCompletado` | `string` | Si | URL valida, max 2048 |
| `ComentarioPromotor` | `string?` | No | Max 500 chars |

**Estructura C# del Command:**
```csharp
public class CompletarTareaCommand : IRequest<ServiceResponse<CompletarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaId { get; set; }
    public string UrlPruebaCompletado { get; set; } = null!;
    public string? ComentarioPromotor { get; set; }
    public string UserId { get; set; } = null!;
}
```

---

### 3.3 GetTareasPendientesQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Queries/GetTareasPendientesQuery.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries`

**Implementa:** `IRequest<ServiceResponse<TareasPendientesResponseDto>>`

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param | Identificador del programa |
| `Page` | `int` | No | Query param | Default: 1. Min: 1. |
| `PageSize` | `int` | No | Query param | Default: 10. Max: 50. |
| `UserId` | `string` | Si | JWT claim `sub` | Resuelto en el controlador |

**Estructura C#:**
```csharp
public class GetTareasPendientesQuery : IRequest<ServiceResponse<TareasPendientesResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string UserId { get; set; } = null!;
}
```

---

### 3.4 ValidarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/ValidarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Implementa:** `IRequest<ServiceResponse<ValidarTareaResponseDto>>`

**Campos del Command:**

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param | Identificador del programa |
| `TareaPromotorId` | `Guid` | Si | Route param | Id del `PromoTareaPromotor` |
| `ComentarioValidacion` | `string?` | No | Request body | Opcional. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` | Resuelto en el controlador |

**DTO de request body (`ValidarTareaDto`) - usado como `[FromBody]`:**

| Campo | Tipo C# | Obligatorio | Constraints |
|-------|---------|-------------|-------------|
| `ComentarioValidacion` | `string?` | No | Max 500 chars |

**Estructura C# del Command:**
```csharp
public class ValidarTareaCommand : IRequest<ServiceResponse<ValidarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string? ComentarioValidacion { get; set; }
    public string UserId { get; set; } = null!;
}
```

---

### 3.5 RechazarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/RechazarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Implementa:** `IRequest<ServiceResponse<RechazarTareaResponseDto>>`

**Campos del Command:**

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param | Identificador del programa |
| `TareaPromotorId` | `Guid` | Si | Route param | Id del `PromoTareaPromotor` |
| `ComentarioValidacion` | `string` | Si | Request body | Obligatorio. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` | Resuelto en el controlador |

**DTO de request body (`RechazarTareaDto`) - usado como `[FromBody]`:**

| Campo | Tipo C# | Obligatorio | Constraints |
|-------|---------|-------------|-------------|
| `ComentarioValidacion` | `string` | Si | NotEmpty, Max 500 chars |

**Estructura C# del Command:**
```csharp
public class RechazarTareaCommand : IRequest<ServiceResponse<RechazarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string ComentarioValidacion { get; set; } = null!;
    public string UserId { get; set; } = null!;
}
```

---

## 4. Response DTOs

### 4.1 MiEstadoTareaDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MiEstadoTareaDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** Anidado dentro de `MisTareasItemDto.MiEstado`. No se usa directamente en `ServiceResponse<T>`.

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaPromotorId` | `Guid` | No | Id del `PromoTareaPromotor` mas reciente por `FechaCreacion DESC` |
| `EstadoTareaId` | `int` | No | FK a `Maestra_EstadoTareaPromo`. 1=Pendiente, 2=Completada, 3=Validada, 4=Rechazada |
| `EstadoTareaNombre` | `string` | No | Nombre del estado resuelto desde la maestra |
| `VecesCompletada` | `int` | No | COUNT de todos los registros `PromoTareaPromotor` para el par `TareaId + ProgramaPromotorId` |
| `FechaPrimeraCompletada` | `DateTime?` | Si | MIN(`FechaCompletado`) calculado en query. Null si nunca completado |
| `FechaUltimaCompletada` | `DateTime?` | Si | MAX(`FechaCompletado`) calculado en query. Null si nunca completado |
| `UrlPruebaCompletado` | `string?` | Si | URL del registro mas reciente |
| `ComentarioValidacion` | `string?` | Si | Motivo del ultimo rechazo o comentario de validacion |

---

### 4.2 MisTareasItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisTareasItemDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** Contenido en `MisTareasResponseDto.Items`. No se usa directamente en `ServiceResponse<T>`.

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaId` | `Guid` | No | Id de la `PromoTarea` |
| `Nombre` | `string` | No | `PromoTarea.Titulo` |
| `Descripcion` | `string?` | Si | `PromoTarea.Descripcion` |
| `InstruccionesUrl` | `string?` | Si | `PromoTarea.UrlInstrucciones` |
| `TipoEventoPromoNombre` | `string` | No | Resuelto via `MaestraTipoEventoPromo.Nombre` |
| `TipoRewardNombre` | `string?` | Si | Resuelto via `MaestraTipoReward.Nombre`. Null si no hay recompensa configurada |
| `ImporteRecompensa` | `decimal?` | Si | `PromoTarea.ImporteRecompensa`. Null si recompensa es en puntos |
| `MonedaNombre` | `string?` | Si | Resuelto via `MaestraMoneda.Nombre`. Null si no hay importe monetario |
| `PuntosRecompensa` | `int?` | Si | `PromoTarea.PuntosRecompensa`. Null si recompensa es monetaria |
| `EsRepetible` | `bool` | No | `PromoTarea.EsRepetible` |
| `MaxRepeticiones` | `int?` | Si | `PromoTarea.MaxRepeticiones`. Null si no es repetible |
| `Orden` | `int` | No | `PromoTarea.Orden` |
| `MiEstado` | `MiEstadoTareaDto?` | Si | Null si el promotor nunca ha completado esta tarea |

---

### 4.3 MisTareasResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisTareasResponseDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** `ServiceResponse<MisTareasResponseDto>`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `ProgramaId` | `Guid` | No | Id del programa de promocion |
| `ProgramaTitulo` | `string` | No | `PromoPrograma.Titulo` |
| `Items` | `IReadOnlyList<MisTareasItemDto>` | No | Lista de tareas activas con estado personal. Nunca null, puede ser lista vacia |

---

### 4.4 CompletarTareaResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CompletarTareaResponseDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** `ServiceResponse<CompletarTareaResponseDto>`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaPromotorId` | `Guid` | No | Id del `PromoTareaPromotor` creado o actualizado |
| `EstadoTareaId` | `int` | No | Siempre 2 (Completada) en respuesta exitosa |
| `EstadoTareaNombre` | `string` | No | Siempre "Completada" en respuesta exitosa |
| `VecesCompletada` | `int` | No | COUNT total de registros para el par `TareaId + ProgramaPromotorId` tras el completado |
| `FechaUltimaCompletada` | `DateTime?` | Si | Fecha del completado recien enviado |

---

### 4.5 TareaPendienteItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareaPendienteItemDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** Contenido en `TareasPendientesResponseDto.Items`. No se usa directamente en `ServiceResponse<T>`.

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaPromotorId` | `Guid` | No | Id del `PromoTareaPromotor` con `EstadoTareaId = 2` |
| `TareaId` | `Guid` | No | Id de la `PromoTarea` |
| `TareaNombre` | `string` | No | `PromoTarea.Titulo` |
| `PromotorId` | `Guid` | No | `Promotor.Id` resuelto via `PromoProgramaPromotor.PromotorId` |
| `PromotorNombre` | `string` | No | `Promotor.NombrePublico` |
| `PromotorTipoNombre` | `string?` | Si | Resuelto via `MaestraTipoPromotor.Nombre`. Null si promotor no tiene tipo |
| `UrlPruebaCompletado` | `string?` | Si | `PromoTareaPromotor.UrlPruebaCompletado` |
| `ComentarioPromotor` | `string?` | Si | `PromoTareaPromotor.ComentarioPromotor` |
| `VecesCompletada` | `int` | No | COUNT total de registros para el par `TareaId + ProgramaPromotorId` |
| `FechaUltimaCompletada` | `DateTime?` | Si | `FechaCompletado` del registro actual con `EstadoTareaId = 2` |

---

### 4.6 TareasPendientesResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareasPendientesResponseDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** `ServiceResponse<TareasPendientesResponseDto>`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `Items` | `IReadOnlyList<TareaPendienteItemDto>` | No | Lista paginada de completados. Nunca null |
| `TotalCount` | `int` | No | Total de registros sin paginar |
| `Page` | `int` | No | Pagina actual |
| `PageSize` | `int` | No | Elementos por pagina |
| `TotalPages` | `int` | No | Total de paginas calculado |

---

### 4.7 ValidarTareaResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ValidarTareaResponseDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** `ServiceResponse<ValidarTareaResponseDto>`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaPromotorId` | `Guid` | No | Id del `PromoTareaPromotor` validado |
| `EstadoTareaId` | `int` | No | Siempre 3 (Validada) en respuesta exitosa |
| `EstadoTareaNombre` | `string` | No | Siempre "Validada" en respuesta exitosa |
| `RecompensaAcreditada` | `decimal?` | Si | `PromoTarea.ImporteRecompensa`. Null si la tarea no tiene recompensa monetaria |
| `MonedaNombre` | `string?` | Si | Resuelto via `MaestraMoneda`. Null si no hay recompensa monetaria |
| `PuntosAcreditados` | `int?` | Si | `PromoTarea.PuntosRecompensa`. Null si no hay recompensa en puntos |

---

### 4.8 RechazarTareaResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RechazarTareaResponseDto.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Dtos`

**Wrapping:** `ServiceResponse<RechazarTareaResponseDto>`

| Propiedad | Tipo C# | Nullable | Descripcion |
|-----------|---------|----------|-------------|
| `TareaPromotorId` | `Guid` | No | Id del `PromoTareaPromotor` rechazado |
| `EstadoTareaId` | `int` | No | Siempre 4 (Rechazada) en respuesta exitosa |
| `EstadoTareaNombre` | `string` | No | Siempre "Rechazada" en respuesta exitosa |

---

### 4.9 DTOs de Request Body (usados en controlador como `[FromBody]`)

Estos DTOs se usan en el controlador para deserializar el body y luego mapear a los Commands.

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CompletarTareaDto.cs`

```csharp
public class CompletarTareaDto
{
    /// <summary>URL valida. Max 2048 caracteres. Obligatoria.</summary>
    public string UrlPruebaCompletado { get; set; } = null!;
    /// <summary>Opcional. Max 500 caracteres.</summary>
    public string? ComentarioPromotor { get; set; }
}
```

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ValidarTareaDto.cs`

```csharp
public class ValidarTareaDto
{
    /// <summary>Opcional para validacion. Max 500 caracteres.</summary>
    public string? ComentarioValidacion { get; set; }
}
```

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RechazarTareaDto.cs`

```csharp
public class RechazarTareaDto
{
    /// <summary>Obligatorio para rechazo. Max 500 caracteres.</summary>
    public string ComentarioValidacion { get; set; } = null!;
}
```

---

## 5. Validadores FluentValidation

### 5.1 CompletarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/CompletarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `CompletarTareaCommand`

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constante | Condicion |
|-------|----------------------|---------|---------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `Validation_Required` | Siempre |
| `ProgramaId` | `.NotEmpty()` | "El ProgramaId es obligatorio" | `Validation_Required` | Siempre |
| `TareaId` | `.NotEmpty()` | "El TareaId es obligatorio" | `Validation_Required` | Siempre |
| `UrlPruebaCompletado` | `.NotEmpty()` | "La URL de prueba es obligatoria" | `Validation_Required` | Siempre |
| `UrlPruebaCompletado` | `.MaximumLength(2048)` | "La URL de prueba no puede superar los 2048 caracteres" | `Validation_MaxLength` | Cuando no vacia |
| `UrlPruebaCompletado` | `.Must(BeValidUrl)` | "La URL de prueba no tiene formato valido" | `Validation_InvalidUrl` | Cuando no vacia |
| `ComentarioPromotor` | `.MaximumLength(500)` | "El comentario no puede superar los 500 caracteres" | `Validation_MaxLength` | Cuando no null |

**Metodo privado compartido:**
```csharp
protected static bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url)) return true;
    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Notas:**
- El validator NO inyecta ningun servicio. Las validaciones de negocio (programa activo, tarea activa, limites de repeticion, aprobacion del promotor) se hacen en el Handler.
- El patron `BeValidUrl` es identico al de `CreatePromoProgramaCommandValidator`. Copiar el metodo estatico protegido.
- Las reglas de `UserId`, `ProgramaId`, `TareaId` son identicas al patron de `AprobarInscripcionCommandValidator`.

---

### 5.2 ValidarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/ValidarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `ValidarTareaCommand`

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constante | Condicion |
|-------|----------------------|---------|---------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `Validation_Required` | Siempre |
| `ProgramaId` | `.NotEmpty()` | "El ProgramaId es obligatorio" | `Validation_Required` | Siempre |
| `TareaPromotorId` | `.NotEmpty()` | "El TareaPromotorId es obligatorio" | `Validation_Required` | Siempre |
| `ComentarioValidacion` | `.MaximumLength(500)` | "El comentario no puede superar los 500 caracteres" | `Validation_MaxLength` | Cuando no null |

**Notas:**
- `ComentarioValidacion` es opcional al validar (a diferencia del rechazo). Por eso no hay regla `NotEmpty`.
- Las verificaciones de estado del completado (`EstadoTareaId == 2`) y propiedad del artista se hacen en el Handler.

---

### 5.3 RechazarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/RechazarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `RechazarTareaCommand`

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constante | Condicion |
|-------|----------------------|---------|---------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `Validation_Required` | Siempre |
| `ProgramaId` | `.NotEmpty()` | "El ProgramaId es obligatorio" | `Validation_Required` | Siempre |
| `TareaPromotorId` | `.NotEmpty()` | "El TareaPromotorId es obligatorio" | `Validation_Required` | Siempre |
| `ComentarioValidacion` | `.NotEmpty()` | "El motivo de rechazo es obligatorio" | `Validation_Required` | Siempre |
| `ComentarioValidacion` | `.MaximumLength(500)` | "El motivo de rechazo no puede superar los 500 caracteres" | `Validation_MaxLength` | Cuando no vacio |

**Notas:**
- `ComentarioValidacion` es OBLIGATORIO al rechazar (RN-10). La regla `NotEmpty` lo garantiza a nivel de validator antes de llegar al Handler.
- Este es el unico validator de los tres Commands que requiere un servicio. No requiere servicios: todas las validaciones son puramente sintacticas.

---

### 5.4 No se necesita GetMisTareasQueryValidator ni GetTareasPendientesQueryValidator

Las Queries solo tienen `ProgramaId` (Guid del route), `UserId` (del JWT) y parametros de paginacion opcionales. La validacion de existencia de entidades se hace en el Handler. Siguiendo el patron del proyecto (ver `GetMisInscripcionesQuery`, `GetInscripcionesProgramaQuery`), las Queries de lectura sin body no tienen validator propio.

---

## 6. AutoMapper Mappings

### 6.1 PromoTareaProfile (nuevo)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoTareaProfile.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Mapping`

**Nota:** El archivo `PromoTareaProfile.cs` ya existe en el proyecto. Se deben AGREGAR los mappings nuevos al `Profile` existente, no crear uno nuevo.

**Mappings a agregar:**

| Source | Destination | Tipo | Notas |
|--------|-------------|------|-------|
| `PromoTarea` | `MisTareasItemDto` | Entity -> DTO | `Nombre` = `src.Titulo`; `InstruccionesUrl` = `src.UrlInstrucciones`; `TipoEventoPromoNombre`, `TipoRewardNombre`, `MonedaNombre`, `MiEstado` todos via `.Ignore()` (se resuelven en query manual) |
| `PromoTareaPromotor` | `CompletarTareaResponseDto` | Entity -> DTO | `VecesCompletada` y `FechaUltimaCompletada` via `.Ignore()` (calculados en query); `EstadoTareaNombre` via `.Ignore()` (resuelto desde la maestra) |
| `PromoTareaPromotor` | `ValidarTareaResponseDto` | Entity -> DTO | `EstadoTareaNombre`, `RecompensaAcreditada`, `MonedaNombre`, `PuntosAcreditados` via `.Ignore()` (resueltos en Handler desde la tarea relacionada) |
| `PromoTareaPromotor` | `RechazarTareaResponseDto` | Entity -> DTO | `EstadoTareaNombre` via `.Ignore()` (resuelto desde la maestra en Handler) |

**Detalle de cada mapping:**

```
// PromoTarea -> MisTareasItemDto
ForMember(dest => dest.TareaId, opt => opt.MapFrom(src => src.Id))
ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Titulo))
ForMember(dest => dest.InstruccionesUrl, opt => opt.MapFrom(src => src.UrlInstrucciones))
ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())   // query manual
ForMember(dest => dest.TipoRewardNombre, opt => opt.Ignore())        // query manual
ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())            // query manual
ForMember(dest => dest.MiEstado, opt => opt.Ignore())                // query manual

// PromoTareaPromotor -> CompletarTareaResponseDto
ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
ForMember(dest => dest.EstadoTareaId, opt => opt.MapFrom(src => src.EstadoTareaId))
ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())       // resuelto desde maestra
ForMember(dest => dest.VecesCompletada, opt => opt.Ignore())         // calculado en query
ForMember(dest => dest.FechaUltimaCompletada, opt => opt.MapFrom(src => src.FechaCompletado))

// PromoTareaPromotor -> ValidarTareaResponseDto
ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
ForMember(dest => dest.EstadoTareaId, opt => opt.MapFrom(src => src.EstadoTareaId))
ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())       // resuelto desde maestra
ForMember(dest => dest.RecompensaAcreditada, opt => opt.Ignore())    // desde PromoTarea.ImporteRecompensa
ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())            // desde PromoTarea.MonedaId
ForMember(dest => dest.PuntosAcreditados, opt => opt.Ignore())       // desde PromoTarea.PuntosRecompensa

// PromoTareaPromotor -> RechazarTareaResponseDto
ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
ForMember(dest => dest.EstadoTareaId, opt => opt.MapFrom(src => src.EstadoTareaId))
ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())       // resuelto desde maestra
```

**Nota importante sobre la estrategia de mapping:**

Los DTOs de esta feature (`MisTareasItemDto`, `TareaPendienteItemDto`, `MiEstadoTareaDto`) requieren datos calculados en SQL (COUNTs, MIN/MAX) y datos de multiples entidades resueltos por join. Por este motivo, el mapping de AutoMapper solo cubre los campos directos de la entidad fuente. Los campos calculados y los lookup de maestras se asignan manualmente en el Handler despues de obtener los datos de la query al Service. Este patron es identico al usado en `MiInscripcionDto` (ver `InscripcionProfile.cs`, lineas 44-56 donde la mayoria de campos son `.Ignore()`).

---

## 7. Interface de Servicio: IPromoTareaService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoTareaService.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Interfaces.Services`

Los Handlers de esta feature inyectaran `IPromoTareaService` (nuevo servicio). Las operaciones de wallet se delegan a `IPromotorWalletService` (ya existente, se extendera).

**Metodos requeridos por los Handlers:**

| Metodo | Retorno | Parametros | Usado por |
|--------|---------|------------|-----------|
| `GetPromotorByUserIdAsync` | `Task<Promotor?>` | `string userId, CancellationToken ct` | Todos los Handlers del promotor |
| `GetArtistaIdByUserIdAsync` | `Task<ArtistaId?>` | `string userId, CancellationToken ct` | Handlers del artista |
| `GetProgramaByIdAsync` | `Task<PromoPrograma?>` | `PromoProgramaId id, CancellationToken ct` | Todos los Handlers |
| `GetInscripcionAprobadaAsync` | `Task<PromoProgramaPromotor?>` | `PromotorId promotorId, PromoProgramaId programaId, CancellationToken ct` | `GetMisTareasQuery`, `CompletarTareaCommand` |
| `GetTareasActivasConEstadoAsync` | `Task<MisTareasResponseDto>` | `PromoProgramaId programaId, Guid programaPromotorId, CancellationToken ct` | `GetMisTareasQuery` |
| `GetTareaByIdAsync` | `Task<PromoTarea?>` | `Guid tareaId, CancellationToken ct` | `CompletarTareaCommand` |
| `ContarCompletadosActivosAsync` | `Task<int>` | `Guid tareaId, Guid programaPromotorId, CancellationToken ct` | `CompletarTareaCommand` (verifica limite MaxRepeticiones) |
| `GetUltimoCompletadoAsync` | `Task<PromoTareaPromotor?>` | `Guid tareaId, Guid programaPromotorId, CancellationToken ct` | `CompletarTareaCommand` (detecta si ultimo esta Rechazado) |
| `CompletarTareaAsync` | `Task<(PromoTareaPromotor registro, int vecesCompletada)>` | `PromoTareaPromotor registro, bool esActualizacion, CancellationToken ct` | `CompletarTareaCommand` |
| `GetTareasPendientesAsync` | `Task<(IReadOnlyList<TareaPendienteItemDto> Items, int TotalCount)>` | `PromoProgramaId programaId, int page, int pageSize, CancellationToken ct` | `GetTareasPendientesQuery` |
| `GetTareaPromotorByIdAsync` | `Task<PromoTareaPromotor?>` | `Guid id, CancellationToken ct` | `ValidarTareaCommand`, `RechazarTareaCommand` |
| `ValidarTareaAsync` | `Task` | `PromoTareaPromotor registro, string? comentario, CancellationToken ct` | `ValidarTareaCommand` (actualiza estado, NO la parte de wallet) |
| `RechazarTareaAsync` | `Task` | `PromoTareaPromotor registro, string comentario, CancellationToken ct` | `RechazarTareaCommand` |

**Nota sobre `IPromotorWalletService`:** El Handler de `ValidarTareaCommand` inyectara tanto `IPromoTareaService` como `IPromotorWalletService`. La logica transaccional de acreditacion (crear `PromotorWalletTransaccion`, actualizar `PromotorWallet.SaldoPendiente`) se coordina en el Handler con una transaccion explicita de BD. El `IPromotorWalletService` existente requiere agregar metodos:
- `GetOrCreateWalletAsync(PromotorId promotorId, int monedaId, CancellationToken ct) -> Task<PromotorWallet>`
- `AcreditarRecompensaAsync(PromotorWallet wallet, PromoTarea tarea, Guid tareaPromotorId, CancellationToken ct) -> Task`

---

## 8. Estructura del Controlador: PromoTareaController

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromoTareaController.cs`

**Namespace:** `WePlayRises.Crowdpromotion.WebApi.Controllers`

**Ruta base:** `[Route("api/crowdpromotion/programas")]`

**Hereda de:** `BaseLoggerController`

**Dependencias inyectadas:**
- `IMediator _mediator`
- `ICurrentUserService _currentUser`
- `ILogger<PromoTareaController> logger` (via base)

### 8.1 GET mis-tareas

```
GET /api/crowdpromotion/programas/{programaId}/mis-tareas
[HttpGet("{programaId:guid}/mis-tareas")]
[Authorize]
```

**Logica del controlador:**
1. Obtener `userId` desde `_currentUser.UserId`. Si null: retornar `Unauthorized()`.
2. Construir `GetMisTareasQuery { ProgramaId = programaId, UserId = userId.Value.ToString() }`.
3. Enviar via `_mediator.Send(query, ct)`.
4. Retornar `FromServiceResponse(result)`.

**ProducesResponseType:**
- 200: `ServiceResponse<MisTareasResponseDto>`
- 401: `ServiceResponse<MisTareasResponseDto>`
- 403: `ServiceResponse<MisTareasResponseDto>`
- 404: `ServiceResponse<MisTareasResponseDto>`
- 500: `ServiceResponse<MisTareasResponseDto>`

---

### 8.2 POST completar

```
POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar
[HttpPost("{programaId:guid}/tareas/{tareaId:guid}/completar")]
[Authorize]
```

**Logica del controlador:**
1. Obtener `userId`. Si null: `Unauthorized()`.
2. Construir `CompletarTareaCommand` mapeando desde `[FromBody] CompletarTareaDto request` + route params + userId.
3. Enviar via mediator.
4. Si `result.IsSuccess`: retornar `Ok(result)` (HTTP 200, no 201 porque es una accion de negocio, no creacion de recurso puro).
5. Si no: `FromServiceResponse(result)`.

**Nota sobre codigo HTTP:** El contracts.md define response como `200 OK` (no 201). Esto es correcto porque `completar` es una accion de dominio que puede crear o actualizar un registro existente (en caso de re-envio tras rechazo).

**ProducesResponseType:**
- 200: `ServiceResponse<CompletarTareaResponseDto>`
- 400: `ServiceResponse<CompletarTareaResponseDto>`
- 401: `ServiceResponse<CompletarTareaResponseDto>`
- 403: `ServiceResponse<CompletarTareaResponseDto>`
- 404: `ServiceResponse<CompletarTareaResponseDto>`
- 500: `ServiceResponse<CompletarTareaResponseDto>`

---

### 8.3 GET tareas-pendientes

```
GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes
[HttpGet("{programaId:guid}/tareas-pendientes")]
[Authorize]
```

**Logica del controlador:**
1. Obtener `userId`. Si null: `Unauthorized()`.
2. Normalizar paginacion: `page = Math.Max(1, page)`, `pageSize = Math.Clamp(pageSize, 1, 50)`.
3. Construir `GetTareasPendientesQuery`.
4. `FromServiceResponse(await _mediator.Send(query, ct))`.

**ProducesResponseType:**
- 200: `ServiceResponse<TareasPendientesResponseDto>`
- 401: `ServiceResponse<TareasPendientesResponseDto>`
- 403: `ServiceResponse<TareasPendientesResponseDto>`
- 404: `ServiceResponse<TareasPendientesResponseDto>`
- 500: `ServiceResponse<TareasPendientesResponseDto>`

---

### 8.4 PATCH validar

```
PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar
[HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/validar")]
[Authorize]
```

**Logica del controlador:**
1. Obtener `userId`. Si null: `Unauthorized()`.
2. Construir `ValidarTareaCommand` desde `[FromBody] ValidarTareaDto request` + route params + userId.
3. `FromServiceResponse(await _mediator.Send(command, ct))`.

**ProducesResponseType:**
- 200: `ServiceResponse<ValidarTareaResponseDto>`
- 400: `ServiceResponse<ValidarTareaResponseDto>`
- 401: `ServiceResponse<ValidarTareaResponseDto>`
- 403: `ServiceResponse<ValidarTareaResponseDto>`
- 404: `ServiceResponse<ValidarTareaResponseDto>`
- 500: `ServiceResponse<ValidarTareaResponseDto>`

---

### 8.5 PATCH rechazar

```
PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar
[HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/rechazar")]
[Authorize]
```

**Logica del controlador:**
1. Obtener `userId`. Si null: `Unauthorized()`.
2. Construir `RechazarTareaCommand` desde `[FromBody] RechazarTareaDto request` + route params + userId.
3. `FromServiceResponse(await _mediator.Send(command, ct))`.

**ProducesResponseType:**
- 200: `ServiceResponse<RechazarTareaResponseDto>`
- 400: `ServiceResponse<RechazarTareaResponseDto>`
- 401: `ServiceResponse<RechazarTareaResponseDto>`
- 403: `ServiceResponse<RechazarTareaResponseDto>`
- 404: `ServiceResponse<RechazarTareaResponseDto>`
- 500: `ServiceResponse<RechazarTareaResponseDto>`

---

## 9. Logica de Negocio en Handlers (flujo esperado)

### 9.1 GetMisTareasQueryHandler

```
try
1. Validar no nulos (UserId, ProgramaId)
2. GetPromotorByUserIdAsync(UserId) -> 404 NotFound_Promotor si null
3. GetProgramaByIdAsync(ProgramaId) -> 404 NotFound_PromoPrograma si null
4. GetInscripcionAprobadaAsync(PromotorId, ProgramaId) -> 403 Auth_Forbidden si null o !EsAprobado o EsBloqueado
5. GetTareasActivasConEstadoAsync(ProgramaId, programaPromotor.Id) -> devuelve MisTareasResponseDto
6. Retornar ServiceResponse<MisTareasResponseDto> { Data = dto }
catch -> logger.LogError + Internal_UnexpectedError
```

### 9.2 CompletarTareaCommandHandler

```
try
1. FluentValidation -> retornar ServiceResponse con errores si invalido
2. GetPromotorByUserIdAsync(UserId) -> 404 NotFound_Promotor si null
3. GetProgramaByIdAsync(ProgramaId) -> 404 NotFound_PromoPrograma si null
4. Si !programa.EsActivo -> 400 BusinessRule_ProgramaInactivo
5. GetInscripcionAprobadaAsync(PromotorId, ProgramaId) -> 403 Auth_Forbidden si no aprobado o bloqueado
6. GetTareaByIdAsync(TareaId) -> 404 NotFound_PromoTarea si null
7. Si !tarea.EsActivo -> 400 BusinessRule_TareaInactiva
8. Si tarea.FechaFin != null && tarea.FechaFin < now() -> 400 BusinessRule_TareaFechaFinPasada
9. ContarCompletadosActivosAsync(TareaId, inscripcion.Id) -> int completadosActivos
10. Si !tarea.EsRepetible && completadosActivos >= 1 -> 400 BusinessRule_TareaNoRepetible
11. Si tarea.EsRepetible && tarea.MaxRepeticiones != null && completadosActivos >= tarea.MaxRepeticiones -> 400 BusinessRule_MaxRepeticionesAlcanzado
12. GetUltimoCompletadoAsync(TareaId, inscripcion.Id) -> PromoTareaPromotor? ultimo
13. Si ultimo != null && ultimo.EstadoTareaId == 4 (Rechazada): actualizar registro existente (esActualizacion = true)
    Sino: crear nuevo registro (esActualizacion = false)
14. CompletarTareaAsync(registro, esActualizacion) -> (PromoTareaPromotor registroGuardado, int vecesCompletada)
15. Mapear a CompletarTareaResponseDto + asignar VecesCompletada y EstadoTareaNombre
16. Retornar ServiceResponse { Data = dto, Messages = [{ "Tarea enviada para validacion", Updated }] }
catch -> LogError + Internal_UnexpectedError
```

### 9.3 GetTareasPendientesQueryHandler

```
try
1. GetArtistaIdByUserIdAsync(UserId) -> 404 NotFound_Artista si null
2. GetProgramaByIdAsync(ProgramaId) -> 404 NotFound_PromoPrograma si null
3. Si programa.ArtistaId != artistaId -> 403 BusinessRule_NoEsPropietarioPrograma
4. GetTareasPendientesAsync(ProgramaId, Page, PageSize) -> (items, totalCount)
5. Calcular TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize)
6. Construir TareasPendientesResponseDto { Items, TotalCount, Page, PageSize, TotalPages }
7. Retornar ServiceResponse<TareasPendientesResponseDto> { Data = dto }
catch -> LogError + Internal_UnexpectedError
```

### 9.4 ValidarTareaCommandHandler

```
try
1. FluentValidation -> retornar ServiceResponse con errores si invalido
2. GetArtistaIdByUserIdAsync(UserId) -> 404 NotFound_Artista si null
3. GetProgramaByIdAsync(ProgramaId) -> 404 NotFound_PromoPrograma si null
4. Si programa.ArtistaId != artistaId -> 403 BusinessRule_NoEsPropietarioPrograma
5. GetTareaPromotorByIdAsync(TareaPromotorId) -> 404 NotFound_TareaPromotor si null
6. Si registro.EstadoTareaId != 2 -> 400 BusinessRule_CompletadoEstadoInvalido
7. Obtener PromoTarea relacionada (via registro.TareaId) para obtener ImporteRecompensa, MonedaId, PuntosRecompensa
8. [TRANSACCION EXPLICITA]
   a. ValidarTareaAsync(registro, ComentarioValidacion)  // actualiza EstadoTareaId=3, FechaValidado=now()
   b. Si tarea.ImporteRecompensa != null:
      - GetOrCreateWalletAsync(PromotorId, tarea.MonedaId)
      - AcreditarRecompensaAsync(wallet, tarea, TareaPromotorId)  // crea PromotorWalletTransaccion + actualiza SaldoPendiente
9. Mapear a ValidarTareaResponseDto + asignar RecompensaAcreditada, MonedaNombre, PuntosAcreditados desde la tarea
10. Retornar ServiceResponse { Data = dto, Messages = [{ "Tarea validada y recompensa acreditada", Updated }] }
catch -> LogError + Internal_UnexpectedError
```

**CRITICO:** El paso 8 es una transaccion de BD. Si `AcreditarRecompensaAsync` falla, el estado de `PromoTareaPromotor` no debe quedar en Validada. Implementar con `IDbContextTransaction` en la capa de Infrastructure o coordinado via Unit of Work.

### 9.5 RechazarTareaCommandHandler

```
try
1. FluentValidation -> retornar ServiceResponse con errores si invalido
2. GetArtistaIdByUserIdAsync(UserId) -> 404 NotFound_Artista si null
3. GetProgramaByIdAsync(ProgramaId) -> 404 NotFound_PromoPrograma si null
4. Si programa.ArtistaId != artistaId -> 403 BusinessRule_NoEsPropietarioPrograma
5. GetTareaPromotorByIdAsync(TareaPromotorId) -> 404 NotFound_TareaPromotor si null
6. Si registro.EstadoTareaId != 2 -> 400 BusinessRule_CompletadoEstadoInvalido
7. RechazarTareaAsync(registro, ComentarioValidacion)  // EstadoTareaId=4, FechaValidado=now(), ComentarioValidacion
8. Mapear a RechazarTareaResponseDto + asignar EstadoTareaNombre = "Rechazada"
9. Retornar ServiceResponse { Data = dto, Messages = [{ "Tarea rechazada", Updated }] }
catch -> LogError + Internal_UnexpectedError
```

---

## 10. OpenAPI / Swagger Documentation

### GET /api/crowdpromotion/programas/{programaId}/mis-tareas

- **Summary:** Listar tareas activas del programa con estado personal del promotor
- **Description:** Retorna las tareas con `EsActivo = true` del programa indicado, junto con el estado de completado del promotor autenticado. Los campos `vecesCompletada`, `fechaPrimeraCompletada` y `fechaUltimaCompletada` son calculados en la query agregando registros `PromoTareaPromotor`.
- **Parameters:**
  - `programaId` (path, Guid, required)
- **Request Body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<MisTareasResponseDto>` - Lista de tareas con estado personal
  - 401: No autenticado
  - 403: Promotor no aprobado en el programa o bloqueado (ErrorCode: 4026)
  - 404: Promotor no existe (2015) o programa no existe (2019)
  - 500: Error inesperado (5000)
- **Auth:** Bearer JWT requerido. Actor: Promotor aprobado en el programa.
- **Tag:** `PromoTarea`

---

### POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar

- **Summary:** Marcar una tarea como completada aportando URL de prueba
- **Description:** Crea un nuevo registro `PromoTareaPromotor` en estado Completada (2), o actualiza el registro mas reciente si este estaba en estado Rechazada (4). Para tareas repetibles, permite multiples completados hasta el limite `MaxRepeticiones`.
- **Parameters:**
  - `programaId` (path, Guid, required)
  - `tareaId` (path, Guid, required)
- **Request Body:** `CompletarTareaDto` (application/json)
  - `urlPruebaCompletado` (string, required, max 2048, URL valida)
  - `comentarioPromotor` (string, optional, max 500)
- **Responses:**
  - 200: `ServiceResponse<CompletarTareaResponseDto>` - Completado registrado exitosamente
  - 400: Errores de validacion (1001, 1002, 1013) o reglas de negocio (4024, 4027, 4028, 4029, 4030)
  - 401: No autenticado
  - 403: Promotor no aprobado o bloqueado (4026)
  - 404: Promotor (2015), programa (2019) o tarea (2021) no existe
  - 500: Error inesperado (5000)
- **Auth:** Bearer JWT requerido. Actor: Promotor aprobado en el programa.
- **Tag:** `PromoTarea`

---

### GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes

- **Summary:** Listar completados pendientes de validacion para un programa (vista artista)
- **Description:** Retorna los registros `PromoTareaPromotor` con `EstadoTareaId = 2` del programa indicado, paginados. Solo accesible por el artista propietario del programa. Incluye nombre del promotor, URL de prueba y conteo de completados totales.
- **Parameters:**
  - `programaId` (path, Guid, required)
  - `page` (query, int, optional, default: 1)
  - `pageSize` (query, int, optional, default: 10, max: 50)
- **Request Body:** Ninguno
- **Responses:**
  - 200: `ServiceResponse<TareasPendientesResponseDto>` - Lista paginada de completados
  - 401: No autenticado
  - 403: No es propietario del programa (4026)
  - 404: Artista (2016) o programa (2019) no existe
  - 500: Error inesperado (5000)
- **Auth:** Bearer JWT requerido. Actor: Artista propietario del programa.
- **Tag:** `PromoTarea`

---

### PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar

- **Summary:** Validar un completado de tarea y acreditar la recompensa al promotor
- **Description:** Cambia el estado del completado a Validada (3), registra `FechaValidado` y acredita la recompensa en la wallet del promotor de forma transaccional. Si la tarea tiene `ImporteRecompensa`, crea un `PromotorWalletTransaccion` y actualiza `SaldoPendiente` y `TotalGanado` en `PromotorWallet`. Si la wallet no existe para la moneda, la crea automaticamente.
- **Parameters:**
  - `programaId` (path, Guid, required)
  - `tareaPromotorId` (path, Guid, required)
- **Request Body:** `ValidarTareaDto` (application/json)
  - `comentarioValidacion` (string, optional, max 500)
- **Responses:**
  - 200: `ServiceResponse<ValidarTareaResponseDto>` - Completado validado y recompensa acreditada
  - 400: Comentario demasiado largo (1002) o completado no esta en estado Completada (4031)
  - 401: No autenticado
  - 403: No es propietario del programa (4026)
  - 404: Artista (2016), programa (2019) o completado (2022) no existe
  - 500: Error inesperado (5000) - incluye fallo en la transaccion de acreditacion
- **Auth:** Bearer JWT requerido. Actor: Artista propietario del programa.
- **Tag:** `PromoTarea`

---

### PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar

- **Summary:** Rechazar un completado de tarea con motivo obligatorio
- **Description:** Cambia el estado del completado a Rechazada (4), registra `FechaValidado` y guarda el motivo de rechazo. El promotor podra re-enviar la tarea con una nueva URL de prueba; el registro rechazado queda como historico.
- **Parameters:**
  - `programaId` (path, Guid, required)
  - `tareaPromotorId` (path, Guid, required)
- **Request Body:** `RechazarTareaDto` (application/json)
  - `comentarioValidacion` (string, required, min 1, max 500)
- **Responses:**
  - 200: `ServiceResponse<RechazarTareaResponseDto>` - Completado rechazado
  - 400: Motivo vacio (1001), motivo demasiado largo (1002) o completado no esta en estado Completada (4031)
  - 401: No autenticado
  - 403: No es propietario del programa (4026)
  - 404: Artista (2016), programa (2019) o completado (2022) no existe
  - 500: Error inesperado (5000)
- **Auth:** Bearer JWT requerido. Actor: Artista propietario del programa.
- **Tag:** `PromoTarea`

---

## 11. Tabla de ErrorCodes por Endpoint

| Endpoint | HTTP | ErrorCode | Constante | Causa |
|----------|------|-----------|-----------|-------|
| Todos | 401 | 3001 | `Auth_Unauthorized` | Token invalido o expirado |
| Todos (promotor) | 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | Promotor no aprobado, bloqueado, o no es promotor del programa |
| Todos (artista) | 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | ArtistaId del token != PromoPrograma.ArtistaId |
| GET mis-tareas, POST completar | 404 | 2015 | `NotFound_Promotor` | No existe `Promotor` con el UserId del token |
| GET tareas-pendientes, PATCH validar, PATCH rechazar | 404 | 2016 | `NotFound_Artista` | No existe `Artista` con el UserId del token |
| Todos | 404 | 2019 | `NotFound_PromoPrograma` | No existe `PromoPrograma` con el programaId |
| POST completar | 404 | 2021 | `NotFound_PromoTarea` | No existe `PromoTarea` con el tareaId |
| PATCH validar, PATCH rechazar | 404 | 2022 | `NotFound_TareaPromotor` | No existe `PromoTareaPromotor` con el tareaPromotorId |
| POST completar | 400 | 1001 | `Validation_Required` | UrlPruebaCompletado vacio |
| POST completar | 400 | 1013 | `Validation_InvalidUrl` | UrlPruebaCompletado no es URL valida |
| POST completar, PATCH validar, PATCH rechazar | 400 | 1002 | `Validation_MaxLength` | Comentario supera 500 chars |
| PATCH rechazar | 400 | 1001 | `Validation_Required` | ComentarioValidacion vacio en rechazo |
| POST completar | 400 | 4024 | `BusinessRule_ProgramaInactivo` | PromoPrograma.EsActivo = false |
| POST completar | 400 | 4027 | `BusinessRule_TareaNoRepetible` | Tarea no repetible ya tiene completado activo |
| POST completar | 400 | 4028 | `BusinessRule_MaxRepeticionesAlcanzado` | vecesCompletada >= MaxRepeticiones |
| POST completar | 400 | 4029 | `BusinessRule_TareaInactiva` | PromoTarea.EsActivo = false |
| POST completar | 400 | 4030 | `BusinessRule_TareaFechaFinPasada` | PromoTarea.FechaFin < DateTime.UtcNow |
| PATCH validar, PATCH rechazar | 400 | 4031 | `BusinessRule_CompletadoEstadoInvalido` | EstadoTareaId != 2 al intentar validar o rechazar |
| Todos | 500 | 5000 | `Internal_UnexpectedError` | Excepcion no controlada |

---

## 12. Archivos a Crear

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs     MODIFICAR: agregar 7 nuevas constantes
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Dtos/
│   │   ├── MiEstadoTareaDto.cs               CREAR
│   │   ├── MisTareasItemDto.cs               CREAR
│   │   ├── MisTareasResponseDto.cs           CREAR
│   │   ├── CompletarTareaDto.cs              CREAR (request body)
│   │   ├── CompletarTareaResponseDto.cs      CREAR
│   │   ├── TareaPendienteItemDto.cs          CREAR
│   │   ├── TareasPendientesResponseDto.cs    CREAR
│   │   ├── ValidarTareaDto.cs                CREAR (request body)
│   │   ├── ValidarTareaResponseDto.cs        CREAR
│   │   ├── RechazarTareaDto.cs               CREAR (request body)
│   │   └── RechazarTareaResponseDto.cs       CREAR
│   │
│   ├── Features/PromoTarea/
│   │   ├── Commands/
│   │   │   ├── CompletarTareaCommand.cs      CREAR (Command + Handler en mismo archivo)
│   │   │   ├── ValidarTareaCommand.cs        CREAR (Command + Handler en mismo archivo)
│   │   │   └── RechazarTareaCommand.cs       CREAR (Command + Handler en mismo archivo)
│   │   ├── Queries/
│   │   │   ├── GetMisTareasQuery.cs          CREAR (Query + Handler en mismo archivo)
│   │   │   └── GetTareasPendientesQuery.cs   CREAR (Query + Handler en mismo archivo)
│   │   └── Validators/
│   │       ├── CompletarTareaCommandValidator.cs  CREAR
│   │       ├── ValidarTareaCommandValidator.cs    CREAR
│   │       └── RechazarTareaCommandValidator.cs   CREAR
│   │
│   ├── Interfaces/Services/
│   │   └── IPromoTareaService.cs             CREAR (nuevo servicio para esta feature)
│   │
│   └── Mapping/
│       └── PromoTareaProfile.cs              MODIFICAR: agregar 4 nuevos CreateMap
│
└── WePlayRises.Crowdpromotion.WebApi/
    └── Controllers/
        └── PromoTareaController.cs           CREAR (5 endpoints)
```

**Total: 16 archivos nuevos + 3 archivos modificados = 19 cambios de archivos**

---

## 13. Checklist

- [ ] `ServiceResponseMessageType.cs`: Agregar `NotFound_PromoTarea` ("2021")
- [ ] `ServiceResponseMessageType.cs`: Agregar `NotFound_TareaPromotor` ("2022")
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaNoRepetible` ("4027")
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_MaxRepeticionesAlcanzado` ("4028")
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaInactiva` ("4029")
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaFechaFinPasada` ("4030")
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_CompletadoEstadoInvalido` ("4031")
- [ ] `MiEstadoTareaDto.cs`: Crear con 8 propiedades
- [ ] `MisTareasItemDto.cs`: Crear con 13 propiedades incluyendo `MiEstado`
- [ ] `MisTareasResponseDto.cs`: Crear con 3 propiedades
- [ ] `CompletarTareaDto.cs`: Crear (request body)
- [ ] `CompletarTareaResponseDto.cs`: Crear con 5 propiedades
- [ ] `TareaPendienteItemDto.cs`: Crear con 10 propiedades
- [ ] `TareasPendientesResponseDto.cs`: Crear con 5 propiedades de paginacion
- [ ] `ValidarTareaDto.cs`: Crear (request body, campo opcional)
- [ ] `ValidarTareaResponseDto.cs`: Crear con 6 propiedades
- [ ] `RechazarTareaDto.cs`: Crear (request body, campo obligatorio)
- [ ] `RechazarTareaResponseDto.cs`: Crear con 3 propiedades
- [ ] `GetMisTareasQuery.cs`: Query implementa `IRequest<ServiceResponse<MisTareasResponseDto>>` + Handler en mismo archivo
- [ ] `CompletarTareaCommand.cs`: Command implementa `IRequest<ServiceResponse<CompletarTareaResponseDto>>` + Handler en mismo archivo
- [ ] `GetTareasPendientesQuery.cs`: Query implementa `IRequest<ServiceResponse<TareasPendientesResponseDto>>` + Handler en mismo archivo
- [ ] `ValidarTareaCommand.cs`: Command implementa `IRequest<ServiceResponse<ValidarTareaResponseDto>>` + Handler en mismo archivo
- [ ] `RechazarTareaCommand.cs`: Command implementa `IRequest<ServiceResponse<RechazarTareaResponseDto>>` + Handler en mismo archivo
- [ ] `CompletarTareaCommandValidator.cs`: Validator con `.WithMessage()` Y `.WithErrorCode()` usando constantes
- [ ] `ValidarTareaCommandValidator.cs`: Validator con `.WithMessage()` Y `.WithErrorCode()` usando constantes
- [ ] `RechazarTareaCommandValidator.cs`: Validator con `.WithMessage()` Y `.WithErrorCode()` usando constantes
- [ ] `IPromoTareaService.cs`: Interface con 12 metodos requeridos por los Handlers
- [ ] `PromoTareaProfile.cs` (MODIFICAR): Agregar 4 nuevos `CreateMap` con campos ignorados para resoluciones manuales
- [ ] `PromoTareaController.cs`: 5 endpoints con `[Authorize]`, `ICurrentUserService`, `FromServiceResponse`
- [ ] Todos los Handlers tienen constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Todos los Handlers tienen try-catch con `_logger.LogError` y `Internal_UnexpectedError`
- [ ] Todos los Handlers retornan `ServiceResponse<T>` (nunca throw para flujos de negocio)
- [ ] Validators NO inyectan servicios (validaciones de negocio en Handlers)
- [ ] Handler de `ValidarTareaCommand` implementa la acreditacion transaccional en una sola transaccion de BD
- [ ] `GetMisTareasQueryHandler` NO tiene validator propio registrado (validacion implicita en logica)
- [ ] `GetTareasPendientesQueryHandler` NO tiene validator propio registrado
