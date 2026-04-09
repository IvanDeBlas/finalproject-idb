# Plan CQRS: cp-tareas-promocion

**Fecha:** 2026-03-01
**Modulo:** Crowdpromotion
**Feature:** cp-tareas-promocion (US-CP-04)
**Depende de:** cp-inscripcion-programa (US-CP-03)

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response | Actor |
|-----------|------|---------|----------|-------|
| Listar tareas activas con estado personal | Query | `GetMisTareasQuery` | `ServiceResponse<MisTareasResponseDto>` | Promotor |
| Listar completados pendientes de validacion | Query | `GetTareasPendientesQuery` | `ServiceResponse<TareasPendientesResponseDto>` | Artista |
| Enviar completado de tarea con URL de prueba | Command | `CompletarTareaCommand` | `ServiceResponse<CompletarTareaResponseDto>` | Promotor |
| Validar completado y acreditar recompensa | Command | `ValidarTareaCommand` | `ServiceResponse<ValidarTareaResponseDto>` | Artista |
| Rechazar completado con motivo obligatorio | Command | `RechazarTareaCommand` | `ServiceResponse<RechazarTareaResponseDto>` | Artista |

---

## 2. Dependencias de Servicios

Los cinco Handlers inyectan `IPromoTareaService` (nuevo). Los dos Handlers del artista tambien inyectan `IPromotorWalletService` (existente, sin cambios de interface gracias a la decision de MVP de implementar la logica de wallet dentro de `PromoTareaService.ValidarTareaAsync`).

**CRITICO - Decision de arquitectura:** segun el plan de hexagonal-architecture, la logica transaccional de validacion (actualizar PromoTareaPromotor + crear PromotorWalletTransaccion + actualizar PromotorWallet) se implementa **dentro de `IPromoTareaService.ValidarTareaAsync`** usando el contexto directamente. El `ValidarTareaCommandHandler` delega toda la operacion transaccional al service. Esto simplifica el Handler y centraliza la atomicidad en la capa de infraestructura.

---

## 3. Queries

### 3.1 GetMisTareasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Queries/GetMisTareasQuery.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries`

**Contiene:** Query + Handler en el mismo archivo

**Implementa:** `IRequest<ServiceResponse<MisTareasResponseDto>>`

#### Query - Propiedades

| Propiedad | Tipo C# | Origen | Descripcion |
|-----------|---------|--------|-------------|
| `ProgramaId` | `Guid` | Route param `{programaId:guid}` | Identificador del programa |
| `UserId` | `string` | JWT claim `sub` via `ICurrentUserService` | Resuelto en el controlador |

```csharp
public class GetMisTareasQuery : IRequest<ServiceResponse<MisTareasResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
}
```

#### Handler - Dependencias

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_promoTareaService` | `IPromoTareaService` | Resolver promotor, verificar inscripcion, obtener tareas con estado |
| `_logger` | `ILogger<GetMisTareasQueryHandler>` | Logging de errores |

**Constructor:** Todas las dependencias con `?? throw new ArgumentNullException`

**No tiene IMapper:** El DTO `MisTareasResponseDto` es construido directamente por el service `GetTareasActivasConEstadoAsync` porque requiere agregaciones SQL (VecesCompletada, FechaPrimeraCompletada, FechaUltimaCompletada) que no se pueden mapear con AutoMapper.

**No tiene IValidator:** Las Queries de lectura sin body no tienen validator propio (patron establecido en `GetMisInscripcionesQuery`). La validacion de existencia de entidades se hace en el Handler.

#### Handler - Flujo completo

```
public async Task<ServiceResponse<MisTareasResponseDto>> Handle(GetMisTareasQuery request, CancellationToken ct)
{
    try
    {
        // 1. Resolver Promotor desde UserId (JWT claim)
        var promotor = await _promoTareaService.GetPromotorByUserIdAsync(request.UserId, ct);
        if (promotor == null)
            -> return ValidateExtensions.NotFoundServiceResponse<MisTareasResponseDto>(
                   "No tienes un perfil de promotor",
                   ServiceResponseMessageType.NotFound_Promotor);

        // 2. Verificar que el programa existe
        var programaId = new PromoProgramaId(request.ProgramaId);
        var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, ct);
        if (programa == null)
            -> return ValidateExtensions.NotFoundServiceResponse<MisTareasResponseDto>(
                   "El programa de promocion no existe",
                   ServiceResponseMessageType.NotFound_PromoPrograma);

        // 3. Verificar que el promotor tiene inscripcion aprobada y no esta bloqueado
        var inscripcion = await _promoTareaService.GetInscripcionAprobadaAsync(promotor.Id, programaId, ct);
        if (inscripcion == null || !inscripcion.EsAprobado || inscripcion.EsBloqueado)
            -> return ValidateExtensions.ForbiddenServiceResponse<MisTareasResponseDto>(
                   "No tienes acceso a este programa de promocion",
                   ServiceResponseMessageType.Auth_Forbidden);

        // 4. Obtener tareas activas con estado personal del promotor
        //    El service realiza 2 queries (tareas activas + completados del promotor en esa inscripcion)
        //    y hace la agrupacion en memoria (LINQ to objects) para calcular campos agregados
        var dto = await _promoTareaService.GetTareasActivasConEstadoAsync(programaId, inscripcion.Id, ct);

        // 5. Retornar respuesta exitosa
        return new ServiceResponse<MisTareasResponseDto> { Data = dto };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting mis-tareas for UserId {UserId} ProgramaId {ProgramaId}",
            request.UserId, request.ProgramaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<MisTareasResponseDto>(
            "Error inesperado al obtener las tareas",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

#### Handler - Imports requeridos

```csharp
using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
```

---

### 3.2 GetTareasPendientesQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Queries/GetTareasPendientesQuery.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries`

**Contiene:** Query + Handler en el mismo archivo

**Implementa:** `IRequest<ServiceResponse<TareasPendientesResponseDto>>`

#### Query - Propiedades

| Propiedad | Tipo C# | Origen | Descripcion |
|-----------|---------|--------|-------------|
| `ProgramaId` | `Guid` | Route param `{programaId:guid}` | Identificador del programa |
| `Page` | `int` | Query param, default 1 | Numero de pagina. Min 1. Normalizado en el controlador. |
| `PageSize` | `int` | Query param, default 10 | Elementos por pagina. Max 50. Normalizado en el controlador. |
| `UserId` | `string` | JWT claim `sub` via `ICurrentUserService` | Resuelto en el controlador |

```csharp
public class GetTareasPendientesQuery : IRequest<ServiceResponse<TareasPendientesResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string UserId { get; set; } = null!;
}
```

#### Handler - Dependencias

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_promoTareaService` | `IPromoTareaService` | Resolver artista, verificar propiedad del programa, obtener pendientes paginados |
| `_logger` | `ILogger<GetTareasPendientesQueryHandler>` | Logging de errores |

**Constructor:** Todas las dependencias con `?? throw new ArgumentNullException`

**No tiene IMapper, no tiene IValidator:** Mismo patron que `GetMisTareasQuery`. Las Queries de solo lectura sin body no tienen validator. El DTO paginado se construye en el Handler.

#### Handler - Flujo completo

```
public async Task<ServiceResponse<TareasPendientesResponseDto>> Handle(GetTareasPendientesQuery request, CancellationToken ct)
{
    try
    {
        // 1. Resolver ArtistaId desde UserId (JWT claim)
        //    CRITICO: El artista es del modulo UserAccess. IPromoTareaService.GetArtistaIdByUserIdAsync
        //    hace una query cross-module (WePlayRises DB via SQL) para resolver el ArtistaId
        var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, ct);
        if (artistaId == null)
            -> return ValidateExtensions.NotFoundServiceResponse<TareasPendientesResponseDto>(
                   "No tienes un perfil de artista",
                   ServiceResponseMessageType.NotFound_Artista);

        // 2. Verificar que el programa existe
        var programaId = new PromoProgramaId(request.ProgramaId);
        var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, ct);
        if (programa == null)
            -> return ValidateExtensions.NotFoundServiceResponse<TareasPendientesResponseDto>(
                   "El programa de promocion no existe",
                   ServiceResponseMessageType.NotFound_PromoPrograma);

        // 3. Verificar que el artista es propietario del programa
        if (programa.ArtistaId != artistaId)
            -> return ValidateExtensions.ForbiddenServiceResponse<TareasPendientesResponseDto>(
                   "No eres el propietario de este programa",
                   ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

        // 4. Obtener completados pendientes paginados (EstadoTareaId == 2)
        //    El service realiza la query paginada con includes + COUNT de vecesCompletada por pares
        var (items, totalCount) = await _promoTareaService.GetTareasPendientesAsync(
            programaId, request.Page, request.PageSize, ct);

        // 5. Calcular TotalPages
        var totalPages = totalCount > 0
            ? (int)Math.Ceiling((double)totalCount / request.PageSize)
            : 0;

        // 6. Construir DTO de respuesta
        var dto = new TareasPendientesResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };

        return new ServiceResponse<TareasPendientesResponseDto> { Data = dto };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting tareas-pendientes for UserId {UserId} ProgramaId {ProgramaId}",
            request.UserId, request.ProgramaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<TareasPendientesResponseDto>(
            "Error inesperado al obtener las tareas pendientes",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

#### Handler - Imports requeridos

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
```

---

## 4. Commands

### 4.1 CompletarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/CompletarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Contiene:** Command + Handler en el mismo archivo

**Implementa:** `IRequest<ServiceResponse<CompletarTareaResponseDto>>`

#### Command - Propiedades

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param `{programaId:guid}` | Identificador del programa |
| `TareaId` | `Guid` | Si | Route param `{tareaId:guid}` | Identificador de la tarea a completar |
| `UrlPruebaCompletado` | `string` | Si | Request body (`CompletarTareaDto`) | URL valida. Max 2048 chars. |
| `ComentarioPromotor` | `string?` | No | Request body (`CompletarTareaDto`) | Comentario opcional. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` via `ICurrentUserService` | Resuelto en el controlador |

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

#### Handler - Dependencias

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_promoTareaService` | `IPromoTareaService` | Resolver promotor, verificar inscripcion, verificar tarea, ejecutar completado |
| `_mapper` | `IMapper` | Mapear `PromoTareaPromotor` -> `CompletarTareaResponseDto` |
| `_validator` | `IValidator<CompletarTareaCommand>` | Validacion de formato antes de logica de negocio |
| `_logger` | `ILogger<CompletarTareaCommandHandler>` | Logging de errores |

**Constructor:** Todas las dependencias con `?? throw new ArgumentNullException`

#### Handler - Flujo completo

```
public async Task<ServiceResponse<CompletarTareaResponseDto>> Handle(CompletarTareaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validacion de formato (FluentValidation)
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CompletarTarea: {Errors}",
                string.Join(", ", validationResult.Errors));
            return new ServiceResponse<CompletarTareaResponseDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Resolver Promotor desde UserId
        var promotor = await _promoTareaService.GetPromotorByUserIdAsync(request.UserId, ct);
        if (promotor == null)
            -> return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                   "No tienes un perfil de promotor",
                   ServiceResponseMessageType.NotFound_Promotor);

        // 3. Verificar que el programa existe
        var programaId = new PromoProgramaId(request.ProgramaId);
        var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, ct);
        if (programa == null)
            -> return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                   "El programa de promocion no existe",
                   ServiceResponseMessageType.NotFound_PromoPrograma);

        // 4. LOGICA DE NEGOCIO: Verificar que el programa esta activo (RN-03)
        if (!programa.EsActivo)
            -> return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                   "El programa de promocion no esta activo",
                   ServiceResponseMessageType.BusinessRule_ProgramaInactivo);

        // 5. Verificar inscripcion aprobada y no bloqueada (RN-01)
        var inscripcion = await _promoTareaService.GetInscripcionAprobadaAsync(promotor.Id, programaId, ct);
        if (inscripcion == null || !inscripcion.EsAprobado || inscripcion.EsBloqueado)
            -> return ValidateExtensions.ForbiddenServiceResponse<CompletarTareaResponseDto>(
                   "No estas aprobado o tienes acceso a este programa",
                   ServiceResponseMessageType.Auth_Forbidden);

        // 6. Verificar que la tarea existe y pertenece al programa
        var tarea = await _promoTareaService.GetTareaByIdAsync(request.TareaId, ct);
        if (tarea == null)
            -> return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                   "La tarea no existe o no pertenece a este programa",
                   ServiceResponseMessageType.NotFound_PromoTarea);

        // 7. LOGICA DE NEGOCIO: Verificar que la tarea esta activa (RN-02, FA-04)
        if (!tarea.EsActivo)
            -> return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                   "Esta tarea ya no esta disponible",
                   ServiceResponseMessageType.BusinessRule_TareaInactiva);

        // 8. LOGICA DE NEGOCIO: Verificar fechas de vigencia (RN-02, FA-03)
        if (tarea.FechaFin.HasValue && tarea.FechaFin.Value < DateTime.UtcNow)
            -> return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                   "El plazo para completar esta tarea ha finalizado",
                   ServiceResponseMessageType.BusinessRule_TareaFueraFecha);

        // 9. LOGICA DE NEGOCIO: Verificar limites de repeticion
        //    ContarCompletadosActivosAsync cuenta EstadoTareaId IN (2, 3) para el par tarea+inscripcion
        var completadosActivos = await _promoTareaService.ContarCompletadosActivosAsync(
            request.TareaId, inscripcion.Id, ct);

        // 10. RN-04: Tarea no repetible - no puede tener completados activos
        if (!tarea.EsRepetible && completadosActivos >= 1)
            -> return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                   "Ya completaste esta tarea. No se puede volver a completar porque no es repetible",
                   ServiceResponseMessageType.BusinessRule_TareaNoRepetible);

        // 11. RN-05: Tarea repetible - verificar limite MaxRepeticiones
        if (tarea.EsRepetible && tarea.MaxRepeticiones.HasValue
            && completadosActivos >= tarea.MaxRepeticiones.Value)
            -> return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                   "Has alcanzado el numero maximo de veces que puedes completar esta tarea",
                   ServiceResponseMessageType.BusinessRule_MaxRepeticionesAlcanzado);

        // 12. Detectar si el ultimo registro esta Rechazado para hacer update en lugar de insert (RN-06, FA-05)
        var ultimoRegistro = await _promoTareaService.GetUltimoCompletadoAsync(
            request.TareaId, inscripcion.Id, ct);
        var esActualizacion = ultimoRegistro != null && ultimoRegistro.EstadoTareaId == 4;

        // 13. Construir registro de PromoTareaPromotor
        PromoTareaPromotor registro;
        if (esActualizacion)
        {
            // Actualizar el registro rechazado con nueva URL y limpiar datos de rechazo
            registro = ultimoRegistro!;
            registro.UrlPruebaCompletado = request.UrlPruebaCompletado;
            registro.ComentarioPromotor = request.ComentarioPromotor;
            registro.EstadoTareaId = 2; // Completada
            registro.FechaCompletado = DateTime.UtcNow;
            registro.ComentarioValidacion = null;  // Limpiar rechazo previo
            registro.FechaValidado = null;
        }
        else
        {
            // Crear nuevo registro de completado
            registro = new PromoTareaPromotor
            {
                Id = Guid.NewGuid(),
                TareaId = request.TareaId,
                ProgramaPromotorId = inscripcion.Id,
                EstadoTareaId = 2, // Completada
                UrlPruebaCompletado = request.UrlPruebaCompletado,
                ComentarioPromotor = request.ComentarioPromotor,
                FechaCompletado = DateTime.UtcNow,
                FechaCreacion = DateTime.UtcNow
            };
        }

        // 14. Persistir via service (Add o Update segun esActualizacion)
        var (registroGuardado, vecesCompletada) = await _promoTareaService.CompletarTareaAsync(
            registro, esActualizacion, ct);

        // 15. Mapear entidad a DTO base y completar campos calculados manualmente
        var dto = _mapper.Map<CompletarTareaResponseDto>(registroGuardado);
        dto.VecesCompletada = vecesCompletada;
        dto.EstadoTareaNombre = "Completada";

        _logger.LogInformation(
            "Tarea {TareaId} completada por Promotor {PromotorId} en Programa {ProgramaId}. VecesCompletada: {Veces}",
            request.TareaId, promotor.Id, request.ProgramaId, vecesCompletada);

        // 16. Retornar respuesta exitosa
        return new ServiceResponse<CompletarTareaResponseDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new() { Message = "Tarea enviada para validacion",
                        ErrorCode = ServiceResponseMessageType.Updated }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error completing Tarea {TareaId} for UserId {UserId} in Programa {ProgramaId}",
            request.TareaId, request.UserId, request.ProgramaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<CompletarTareaResponseDto>(
            "Error inesperado al completar la tarea",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

#### Handler - Imports requeridos

```csharp
using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;
```

---

### 4.2 ValidarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/ValidarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Contiene:** Command + Handler en el mismo archivo

**Implementa:** `IRequest<ServiceResponse<ValidarTareaResponseDto>>`

#### Command - Propiedades

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param `{programaId:guid}` | Identificador del programa |
| `TareaPromotorId` | `Guid` | Si | Route param `{tareaPromotorId:guid}` | Id del `PromoTareaPromotor` a validar |
| `ComentarioValidacion` | `string?` | No | Request body (`ValidarTareaDto`) | Comentario opcional del artista. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` via `ICurrentUserService` | Resuelto en el controlador |

```csharp
public class ValidarTareaCommand : IRequest<ServiceResponse<ValidarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string? ComentarioValidacion { get; set; }
    public string UserId { get; set; } = null!;
}
```

#### Handler - Dependencias

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_promoTareaService` | `IPromoTareaService` | Resolver artista, verificar propiedad, obtener completado, ejecutar validacion transaccional |
| `_mapper` | `IMapper` | Mapear `PromoTareaPromotor` -> `ValidarTareaResponseDto` |
| `_validator` | `IValidator<ValidarTareaCommand>` | Validacion de formato (comentario max 500) |
| `_logger` | `ILogger<ValidarTareaCommandHandler>` | Logging de errores |

**Constructor:** Todas las dependencias con `?? throw new ArgumentNullException`

**CRITICO - Decision de transaccion:** La logica transaccional (actualizar PromoTareaPromotor + crear PromotorWalletTransaccion + actualizar PromotorWallet) se ejecuta completamente dentro de `IPromoTareaService.ValidarTareaAsync`. El Handler solo llama al metodo del service y maneja el resultado. Si el service lanza una excepcion, el catch del Handler la captura y retorna `Internal_UnexpectedError`. La transaccion de BD ya fue revertida por el service antes de lanzar la excepcion.

#### Handler - Flujo completo

```
public async Task<ServiceResponse<ValidarTareaResponseDto>> Handle(ValidarTareaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validacion de formato (FluentValidation) - solo comentario max 500
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for ValidarTarea: {Errors}",
                string.Join(", ", validationResult.Errors));
            return new ServiceResponse<ValidarTareaResponseDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Resolver ArtistaId desde UserId (JWT claim)
        var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, ct);
        if (artistaId == null)
            -> return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                   "No tienes un perfil de artista",
                   ServiceResponseMessageType.NotFound_Artista);

        // 3. Verificar que el programa existe
        var programaId = new PromoProgramaId(request.ProgramaId);
        var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, ct);
        if (programa == null)
            -> return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                   "El programa de promocion no existe",
                   ServiceResponseMessageType.NotFound_PromoPrograma);

        // 4. LOGICA DE NEGOCIO: Verificar propiedad del programa (RN-09)
        if (programa.ArtistaId != artistaId)
            -> return ValidateExtensions.ForbiddenServiceResponse<ValidarTareaResponseDto>(
                   "No eres el propietario de este programa",
                   ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

        // 5. Obtener el completado con sus navegaciones (Tarea + ProgramaPromotor.Promotor)
        var registro = await _promoTareaService.GetTareaPromotorByIdAsync(request.TareaPromotorId, ct);
        if (registro == null)
            -> return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                   "El completado no existe o no pertenece a este programa",
                   ServiceResponseMessageType.NotFound_PromoTareaPromotor);

        // 6. LOGICA DE NEGOCIO: Solo se puede validar si esta en estado Completada (id=2)
        if (registro.EstadoTareaId != 2)
            -> return ValidateExtensions.BadRequestServiceResponse<ValidarTareaResponseDto>(
                   "Este completado no puede ser procesado porque ya fue validado o rechazado",
                   ServiceResponseMessageType.BusinessRule_CompletadoEstadoInvalido);

        // 7. TRANSACCION ATOMICA via service:
        //    a. Actualiza PromoTareaPromotor: EstadoTareaId=3, FechaValidado=now(), ComentarioValidacion
        //    b. Si Tarea.ImporteRecompensa != null:
        //       - Busca o crea PromotorWallet para (PromotorId, MonedaId)
        //       - Crea PromotorWalletTransaccion con importe + ReferenciaExterna = tareaPromotorId
        //       - Actualiza PromotorWallet.SaldoPendiente += importe, TotalGanado += importe
        //    c. Hace commit de la transaccion
        //    Si cualquier paso falla: rollback atomico y re-throw
        var dto = await _promoTareaService.ValidarTareaAsync(
            registro, programaId, request.ComentarioValidacion, ct);

        _logger.LogInformation(
            "TareaPromotor {TareaPromotorId} validada por Artista {UserId} en Programa {ProgramaId}",
            request.TareaPromotorId, request.UserId, request.ProgramaId);

        // 8. Retornar respuesta exitosa con datos de recompensa acreditada
        return new ServiceResponse<ValidarTareaResponseDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new() { Message = "Tarea validada y recompensa acreditada",
                        ErrorCode = ServiceResponseMessageType.Updated }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "Error validating TareaPromotor {TareaPromotorId} for UserId {UserId} in Programa {ProgramaId}",
            request.TareaPromotorId, request.UserId, request.ProgramaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<ValidarTareaResponseDto>(
            "Error inesperado al validar la tarea",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

#### Handler - Nota sobre el retorno de `ValidarTareaAsync`

El metodo `IPromoTareaService.ValidarTareaAsync` retorna `Task<ValidarTareaResponseDto>` directamente (no la entidad). Esto es una excepcion justificada al patron "services retornan entidades": la construccion del DTO requiere datos de la tarea relacionada (ImporteRecompensa, MonedaNombre, PuntosRecompensa) que estan disponibles dentro del scope transaccional del service. Devolver la entidad implicaria cargar la tarea por separado en el Handler, rompiendo la atomicidad del flujo. Este patron es consistente con `GetMisTareasAsync` y `GetTareasPendientesAsync` del mismo service.

#### Handler - Imports requeridos

```csharp
using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
```

---

### 4.3 RechazarTareaCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Commands/RechazarTareaCommand.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands`

**Contiene:** Command + Handler en el mismo archivo

**Implementa:** `IRequest<ServiceResponse<RechazarTareaResponseDto>>`

#### Command - Propiedades

| Propiedad | Tipo C# | Requerido | Origen | Descripcion |
|-----------|---------|-----------|--------|-------------|
| `ProgramaId` | `Guid` | Si | Route param `{programaId:guid}` | Identificador del programa |
| `TareaPromotorId` | `Guid` | Si | Route param `{tareaPromotorId:guid}` | Id del `PromoTareaPromotor` a rechazar |
| `ComentarioValidacion` | `string` | Si | Request body (`RechazarTareaDto`) | Motivo de rechazo obligatorio. Max 500 chars. |
| `UserId` | `string` | Si | JWT claim `sub` via `ICurrentUserService` | Resuelto en el controlador |

```csharp
public class RechazarTareaCommand : IRequest<ServiceResponse<RechazarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string ComentarioValidacion { get; set; } = null!;
    public string UserId { get; set; } = null!;
}
```

#### Handler - Dependencias

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_promoTareaService` | `IPromoTareaService` | Resolver artista, verificar propiedad, obtener completado, ejecutar rechazo |
| `_mapper` | `IMapper` | Mapear `PromoTareaPromotor` -> `RechazarTareaResponseDto` |
| `_validator` | `IValidator<RechazarTareaCommand>` | Validacion de formato (comentario requerido y max 500) |
| `_logger` | `ILogger<RechazarTareaCommandHandler>` | Logging de errores |

**Constructor:** Todas las dependencias con `?? throw new ArgumentNullException`

#### Handler - Flujo completo

```
public async Task<ServiceResponse<RechazarTareaResponseDto>> Handle(RechazarTareaCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validacion de formato (FluentValidation) - comentario requerido y max 500
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for RechazarTarea: {Errors}",
                string.Join(", ", validationResult.Errors));
            return new ServiceResponse<RechazarTareaResponseDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Resolver ArtistaId desde UserId (JWT claim)
        var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, ct);
        if (artistaId == null)
            -> return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                   "No tienes un perfil de artista",
                   ServiceResponseMessageType.NotFound_Artista);

        // 3. Verificar que el programa existe
        var programaId = new PromoProgramaId(request.ProgramaId);
        var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, ct);
        if (programa == null)
            -> return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                   "El programa de promocion no existe",
                   ServiceResponseMessageType.NotFound_PromoPrograma);

        // 4. LOGICA DE NEGOCIO: Verificar propiedad del programa (RN-09)
        if (programa.ArtistaId != artistaId)
            -> return ValidateExtensions.ForbiddenServiceResponse<RechazarTareaResponseDto>(
                   "No eres el propietario de este programa",
                   ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

        // 5. Obtener el completado
        var registro = await _promoTareaService.GetTareaPromotorByIdAsync(request.TareaPromotorId, ct);
        if (registro == null)
            -> return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                   "El completado no existe o no pertenece a este programa",
                   ServiceResponseMessageType.NotFound_PromoTareaPromotor);

        // 6. LOGICA DE NEGOCIO: Solo se puede rechazar si esta en estado Completada (id=2)
        if (registro.EstadoTareaId != 2)
            -> return ValidateExtensions.BadRequestServiceResponse<RechazarTareaResponseDto>(
                   "Este completado no puede ser procesado porque ya fue validado o rechazado",
                   ServiceResponseMessageType.BusinessRule_CompletadoEstadoInvalido);

        // 7. Ejecutar rechazo via service (sin transaccion explicita: operacion simple)
        //    Service actualiza: EstadoTareaId=4, FechaValidado=now(), ComentarioValidacion
        await _promoTareaService.RechazarTareaAsync(registro, request.ComentarioValidacion, ct);

        // 8. Mapear entidad actualizada a DTO
        var dto = _mapper.Map<RechazarTareaResponseDto>(registro);
        dto.EstadoTareaNombre = "Rechazada";

        _logger.LogInformation(
            "TareaPromotor {TareaPromotorId} rechazada por Artista {UserId} en Programa {ProgramaId}",
            request.TareaPromotorId, request.UserId, request.ProgramaId);

        // 9. Retornar respuesta exitosa
        return new ServiceResponse<RechazarTareaResponseDto>
        {
            Data = dto,
            Messages = new List<ServiceResponseMessage>
            {
                new() { Message = "Tarea rechazada",
                        ErrorCode = ServiceResponseMessageType.Updated }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex,
            "Error rejecting TareaPromotor {TareaPromotorId} for UserId {UserId} in Programa {ProgramaId}",
            request.TareaPromotorId, request.UserId, request.ProgramaId);
        return ValidateExtensions.InternalServerErrorServiceResponse<RechazarTareaResponseDto>(
            "Error inesperado al rechazar la tarea",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

#### Handler - Imports requeridos

```csharp
using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
```

---

## 5. Validators

**Carpeta:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/`

**Nota:** Solo los Commands tienen validators. Las Queries no tienen body y siguen el patron establecido en el proyecto (ver `GetMisInscripcionesQuery`).

**CRITICO:** Todos los validators usan `ServiceResponseMessageType.X` constants, NUNCA strings literales.

---

### 5.1 CompletarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/CompletarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `CompletarTareaCommand`

**Constructor:** Sin dependencias de servicios. Las validaciones de negocio (programa activo, tarea activa, limites de repeticion, aprobacion del promotor) se hacen en el Handler.

| Campo | Regla | Mensaje | ErrorCode (Constante) | Condicion |
|-------|-------|---------|----------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ProgramaId` | `.Must(id => id != Guid.Empty)` | "El ProgramaId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `TareaId` | `.Must(id => id != Guid.Empty)` | "El TareaId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `UrlPruebaCompletado` | `.NotEmpty()` | "La URL de prueba es obligatoria" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `UrlPruebaCompletado` | `.MaximumLength(2048)` | "La URL de prueba no puede superar los 2048 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` | Cuando no vacia |
| `UrlPruebaCompletado` | `.Must(BeValidUrl)` | "La URL de prueba no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` | Cuando no vacia |
| `ComentarioPromotor` | `.MaximumLength(500)` | "El comentario no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` | Cuando no null |

**Metodo privado helper:**
```csharp
protected static bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url)) return true;
    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Nota:** El metodo `BeValidUrl` es identico al de `CreatePromoProgramaCommandValidator`. Copiar el metodo estatico protegido.

**Estructura de clase:**
```csharp
public class CompletarTareaCommandValidator : AbstractValidator<CompletarTareaCommand>
{
    public CompletarTareaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TareaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El TareaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UrlPruebaCompletado)
            .NotEmpty()
            .WithMessage("La URL de prueba es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UrlPruebaCompletado)
            .MaximumLength(2048)
            .WithMessage("La URL de prueba no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlPruebaCompletado));

        RuleFor(x => x.UrlPruebaCompletado)
            .Must(BeValidUrl)
            .WithMessage("La URL de prueba no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.UrlPruebaCompletado));

        RuleFor(x => x.ComentarioPromotor)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.ComentarioPromotor != null);
    }

    protected static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
```

---

### 5.2 ValidarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/ValidarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `ValidarTareaCommand`

**Constructor:** Sin dependencias de servicios. Las verificaciones de estado del completado y propiedad del artista se hacen en el Handler.

| Campo | Regla | Mensaje | ErrorCode (Constante) | Condicion |
|-------|-------|---------|----------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ProgramaId` | `.Must(id => id != Guid.Empty)` | "El ProgramaId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `TareaPromotorId` | `.Must(id => id != Guid.Empty)` | "El TareaPromotorId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ComentarioValidacion` | `.MaximumLength(500)` | "El comentario no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` | Cuando no null |

**Nota:** `ComentarioValidacion` es OPCIONAL en la validacion (distinto a `RechazarTareaCommand`). No hay regla `NotEmpty`. La verificacion de estado `EstadoTareaId == 2` se hace en el Handler, no en el validator.

**Estructura de clase:**
```csharp
public class ValidarTareaCommandValidator : AbstractValidator<ValidarTareaCommand>
{
    public ValidarTareaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TareaPromotorId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El TareaPromotorId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ComentarioValidacion)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.ComentarioValidacion != null);
    }
}
```

---

### 5.3 RechazarTareaCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoTarea/Validators/RechazarTareaCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators`

**Valida:** `RechazarTareaCommand`

**Constructor:** Sin dependencias de servicios. Validaciones puramente sintacticas.

| Campo | Regla | Mensaje | ErrorCode (Constante) | Condicion |
|-------|-------|---------|----------------------|-----------|
| `UserId` | `.NotEmpty()` | "El UserId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ProgramaId` | `.Must(id => id != Guid.Empty)` | "El ProgramaId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `TareaPromotorId` | `.Must(id => id != Guid.Empty)` | "El TareaPromotorId es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ComentarioValidacion` | `.NotEmpty()` | "El motivo de rechazo es obligatorio" | `ServiceResponseMessageType.Validation_Required` | Siempre |
| `ComentarioValidacion` | `.MaximumLength(500)` | "El motivo de rechazo no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` | Cuando no vacio |

**Nota:** A diferencia de `ValidarTareaCommandValidator`, aqui `ComentarioValidacion` es OBLIGATORIO (RN-10). La regla `NotEmpty` lo garantiza antes de llegar al Handler.

**Estructura de clase:**
```csharp
public class RechazarTareaCommandValidator : AbstractValidator<RechazarTareaCommand>
{
    public RechazarTareaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TareaPromotorId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El TareaPromotorId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ComentarioValidacion)
            .NotEmpty()
            .WithMessage("El motivo de rechazo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ComentarioValidacion)
            .MaximumLength(500)
            .WithMessage("El motivo de rechazo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.ComentarioValidacion));
    }
}
```

---

## 6. AutoMapper Profile

### 6.1 PromoTareaProfile (AMPLIAR archivo existente)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoTareaProfile.cs`

**Estado:** EXISTE - agregar mappings nuevos al `Profile` existente. NO crear archivo nuevo.

**Namespace:** `WePlayRises.Crowdpromotion.Application.Mapping`

**Estrategia de mapping:** Los DTOs de esta feature requieren datos calculados en SQL (COUNTs, MIN/MAX) y datos de multiples entidades (maestras). Por este motivo, AutoMapper solo cubre los campos directos de la entidad fuente. Los campos calculados y lookups de maestras se asignan manualmente en el Handler o en el Service despues de obtener los datos. Este patron es identico al de `MiInscripcionDto` en `InscripcionProfile.cs`.

#### Mappings a agregar

**1. PromoTarea -> MisTareasItemDto**

| Destino | Origen | Estrategia |
|---------|--------|------------|
| `TareaId` | `src.Id` | `MapFrom` |
| `Nombre` | `src.Titulo` | `MapFrom` - nombre en DTO != nombre en entidad |
| `Descripcion` | `src.Descripcion` | Auto |
| `InstruccionesUrl` | `src.UrlInstrucciones` | `MapFrom` - nombre en DTO != nombre en entidad |
| `EsRepetible` | `src.EsRepetible` | Auto |
| `MaxRepeticiones` | `src.MaxRepeticiones` | Auto |
| `Orden` | `src.Orden` | Auto |
| `ImporteRecompensa` | `src.ImporteRecompensa` | Auto |
| `PuntosRecompensa` | `src.PuntosRecompensa` | Auto |
| `TipoEventoPromoNombre` | - | `.Ignore()` - resuelto en service via maestra |
| `TipoRewardNombre` | - | `.Ignore()` - resuelto en service via maestra |
| `MonedaNombre` | - | `.Ignore()` - resuelto en service via maestra |
| `MiEstado` | - | `.Ignore()` - construido en service a partir de completados del promotor |

**2. PromoTareaPromotor -> CompletarTareaResponseDto**

| Destino | Origen | Estrategia |
|---------|--------|------------|
| `TareaPromotorId` | `src.Id` | `MapFrom` - nombre en DTO != nombre en entidad |
| `EstadoTareaId` | `src.EstadoTareaId` | Auto |
| `FechaUltimaCompletada` | `src.FechaCompletado` | `MapFrom` - nombre en DTO != nombre en entidad |
| `EstadoTareaNombre` | - | `.Ignore()` - asignado manualmente en Handler ("Completada") |
| `VecesCompletada` | - | `.Ignore()` - calculado post-persistencia en service |

**3. PromoTareaPromotor -> ValidarTareaResponseDto**

| Destino | Origen | Estrategia |
|---------|--------|------------|
| `TareaPromotorId` | `src.Id` | `MapFrom` |
| `EstadoTareaId` | `src.EstadoTareaId` | Auto |
| `EstadoTareaNombre` | - | `.Ignore()` - asignado en Handler ("Validada") |
| `RecompensaAcreditada` | - | `.Ignore()` - desde PromoTarea.ImporteRecompensa via service |
| `MonedaNombre` | - | `.Ignore()` - resuelto via maestra en service |
| `PuntosAcreditados` | - | `.Ignore()` - desde PromoTarea.PuntosRecompensa via service |

**Nota:** `ValidarTareaAsync` retorna `ValidarTareaResponseDto` directamente desde el service (no entidad). El mapper de `ValidarTareaResponseDto` puede usarse como respaldo pero el service construye el DTO completo.

**4. PromoTareaPromotor -> RechazarTareaResponseDto**

| Destino | Origen | Estrategia |
|---------|--------|------------|
| `TareaPromotorId` | `src.Id` | `MapFrom` |
| `EstadoTareaId` | `src.EstadoTareaId` | Auto |
| `EstadoTareaNombre` | - | `.Ignore()` - asignado manualmente en Handler ("Rechazada") |

#### Pseudocodigo de los mappings a agregar en PromoTareaProfile

```csharp
// --- Mappings nuevos para cp-tareas-promocion (US-CP-04) ---

// PromoTarea -> MisTareasItemDto
CreateMap<PromoTarea, MisTareasItemDto>()
    .ForMember(dest => dest.TareaId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Titulo))
    .ForMember(dest => dest.InstruccionesUrl, opt => opt.MapFrom(src => src.UrlInstrucciones))
    .ForMember(dest => dest.TipoEventoPromoNombre, opt => opt.Ignore())
    .ForMember(dest => dest.TipoRewardNombre, opt => opt.Ignore())
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
    .ForMember(dest => dest.MiEstado, opt => opt.Ignore());

// PromoTareaPromotor -> CompletarTareaResponseDto
CreateMap<PromoTareaPromotor, CompletarTareaResponseDto>()
    .ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())
    .ForMember(dest => dest.VecesCompletada, opt => opt.Ignore())
    .ForMember(dest => dest.FechaUltimaCompletada, opt => opt.MapFrom(src => src.FechaCompletado));

// PromoTareaPromotor -> ValidarTareaResponseDto
CreateMap<PromoTareaPromotor, ValidarTareaResponseDto>()
    .ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore())
    .ForMember(dest => dest.RecompensaAcreditada, opt => opt.Ignore())
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())
    .ForMember(dest => dest.PuntosAcreditados, opt => opt.Ignore());

// PromoTareaPromotor -> RechazarTareaResponseDto
CreateMap<PromoTareaPromotor, RechazarTareaResponseDto>()
    .ForMember(dest => dest.TareaPromotorId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.EstadoTareaNombre, opt => opt.Ignore());
```

---

## 7. Controlador

### 7.1 PromoTareaController (nuevo)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromoTareaController.cs`

**Namespace:** `WePlayRises.Crowdpromotion.WebApi.Controllers`

**Ruta base:** `[Route("api/crowdpromotion/programas")]`

**Hereda de:** `BaseLoggerController` (patron del modulo, igual que `InscripcionController`)

**Atributos de clase:**
```csharp
[ApiController]
[Route("api/crowdpromotion/programas")]
[Produces("application/json")]
[Tags("PromoTarea")]
```

#### Dependencias del constructor

| Dependencia | Tipo | Razon |
|-------------|------|-------|
| `_mediator` | `IMediator` | Enviar Commands y Queries |
| `_currentUser` | `ICurrentUserService` | Extraer UserId del JWT claim `sub` |
| `logger` | `ILogger<PromoTareaController>` | Pasado a `BaseLoggerController` |

**Constructor con `?? throw new ArgumentNullException` para todas las dependencias.**

#### Endpoints

**GET `{programaId:guid}/mis-tareas`**
- `[HttpGet("{programaId:guid}/mis-tareas")]`
- `[Authorize]`
- Parametros: `Guid programaId` (route), `CancellationToken ct`
- Logica:
  1. `var userId = _currentUser.UserId; if (userId == null) return Unauthorized();`
  2. Construir `GetMisTareasQuery { ProgramaId = programaId, UserId = userId.Value.ToString() }`
  3. `var result = await _mediator.Send(query, ct);`
  4. `return FromServiceResponse(result);`
- ProducesResponseType: 200, 401, 403, 404, 500 con `ServiceResponse<MisTareasResponseDto>`

**POST `{programaId:guid}/tareas/{tareaId:guid}/completar`**
- `[HttpPost("{programaId:guid}/tareas/{tareaId:guid}/completar")]`
- `[Authorize]`
- Parametros: `Guid programaId` (route), `Guid tareaId` (route), `[FromBody] CompletarTareaDto request`, `CancellationToken ct`
- Logica:
  1. `var userId = _currentUser.UserId; if (userId == null) return Unauthorized();`
  2. Construir `CompletarTareaCommand` mapeando desde `request` + route params + userId
  3. `var result = await _mediator.Send(command, ct);`
  4. `return FromServiceResponse(result);`
- Nota: HTTP 200 (no 201) porque es una accion de dominio que puede crear O actualizar segun si el ultimo registro estaba rechazado
- ProducesResponseType: 200, 400, 401, 403, 404, 500 con `ServiceResponse<CompletarTareaResponseDto>`

**GET `{programaId:guid}/tareas-pendientes`**
- `[HttpGet("{programaId:guid}/tareas-pendientes")]`
- `[Authorize]`
- Parametros: `Guid programaId` (route), `[FromQuery] int page = 1`, `[FromQuery] int pageSize = 10`, `CancellationToken ct`
- Logica:
  1. `var userId = _currentUser.UserId; if (userId == null) return Unauthorized();`
  2. Normalizar paginacion: `page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 50);`
  3. Construir `GetTareasPendientesQuery { ProgramaId = programaId, Page = page, PageSize = pageSize, UserId = userId.Value.ToString() }`
  4. `var result = await _mediator.Send(query, ct);`
  5. `return FromServiceResponse(result);`
- ProducesResponseType: 200, 401, 403, 404, 500 con `ServiceResponse<TareasPendientesResponseDto>`

**PATCH `{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/validar`**
- `[HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/validar")]`
- `[Authorize]`
- Parametros: `Guid programaId` (route), `Guid tareaPromotorId` (route), `[FromBody] ValidarTareaDto request`, `CancellationToken ct`
- Logica:
  1. `var userId = _currentUser.UserId; if (userId == null) return Unauthorized();`
  2. Construir `ValidarTareaCommand { ProgramaId = programaId, TareaPromotorId = tareaPromotorId, ComentarioValidacion = request.ComentarioValidacion, UserId = userId.Value.ToString() }`
  3. `var result = await _mediator.Send(command, ct);`
  4. `return FromServiceResponse(result);`
- ProducesResponseType: 200, 400, 401, 403, 404, 500 con `ServiceResponse<ValidarTareaResponseDto>`

**PATCH `{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/rechazar`**
- `[HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/rechazar")]`
- `[Authorize]`
- Parametros: `Guid programaId` (route), `Guid tareaPromotorId` (route), `[FromBody] RechazarTareaDto request`, `CancellationToken ct`
- Logica:
  1. `var userId = _currentUser.UserId; if (userId == null) return Unauthorized();`
  2. Construir `RechazarTareaCommand { ProgramaId = programaId, TareaPromotorId = tareaPromotorId, ComentarioValidacion = request.ComentarioValidacion, UserId = userId.Value.ToString() }`
  3. `var result = await _mediator.Send(command, ct);`
  4. `return FromServiceResponse(result);`
- ProducesResponseType: 200, 400, 401, 403, 404, 500 con `ServiceResponse<RechazarTareaResponseDto>`

---

## 8. Constantes ServiceResponseMessageType a agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Estado:** EXISTE - agregar las constantes nuevas para esta feature al archivo existente.

### Constantes nuevas (bloque a insertar)

```csharp
// NotFound (2000-2999) - agregar despues de NotFound_Inscripcion = "2020"
public const string NotFound_PromoTarea = "2021";
public const string NotFound_PromoTareaPromotor = "2022";

// Business Rules (4000-4999) - agregar despues de BusinessRule_NoEsPropietarioPrograma = "4026"
public const string BusinessRule_TareaNoRepetible = "4027";
public const string BusinessRule_MaxRepeticionesAlcanzado = "4028";
public const string BusinessRule_TareaInactiva = "4029";
public const string BusinessRule_TareaFueraFecha = "4030";
public const string BusinessRule_CompletadoEstadoInvalido = "4031";
```

### Constantes existentes reutilizadas por los Handlers

| Constante | Valor | Usado en |
|-----------|-------|----------|
| `Validation_Required` | `"1001"` | Todos los validators |
| `Validation_MaxLength` | `"1002"` | Todos los validators |
| `Validation_InvalidUrl` | `"1013"` | `CompletarTareaCommandValidator` |
| `NotFound_Promotor` | `"2015"` | `GetMisTareasQuery`, `CompletarTareaCommand` |
| `NotFound_Artista` | `"2016"` | `GetTareasPendientesQuery`, `ValidarTareaCommand`, `RechazarTareaCommand` |
| `NotFound_PromoPrograma` | `"2019"` | Todos los Handlers |
| `Auth_Forbidden` | `"3002"` | `GetMisTareasQuery`, `CompletarTareaCommand` |
| `BusinessRule_ProgramaInactivo` | `"4024"` | `CompletarTareaCommand` |
| `BusinessRule_NoEsPropietarioPrograma` | `"4026"` | `GetTareasPendientesQuery`, `ValidarTareaCommand`, `RechazarTareaCommand` |
| `Updated` | `"0002"` | Todos los Commands en respuesta exitosa |
| `Internal_UnexpectedError` | `"5000"` | Todos los Handlers en catch |

---

## 9. Tabla de ErrorCodes por Endpoint

| Endpoint | HTTP | ErrorCode | Constante | Causa |
|----------|------|-----------|-----------|-------|
| Todos | 401 | 3001 | `Auth_Unauthorized` | Token invalido o expirado |
| GET mis-tareas | 404 | 2015 | `NotFound_Promotor` | No existe perfil de promotor |
| GET mis-tareas | 404 | 2019 | `NotFound_PromoPrograma` | Programa no encontrado |
| GET mis-tareas | 403 | 3002 | `Auth_Forbidden` | Promotor no aprobado o bloqueado |
| POST completar | 404 | 2015 | `NotFound_Promotor` | No existe perfil de promotor |
| POST completar | 404 | 2019 | `NotFound_PromoPrograma` | Programa no encontrado |
| POST completar | 404 | 2021 | `NotFound_PromoTarea` | Tarea no encontrada en programa |
| POST completar | 400 | 4024 | `BusinessRule_ProgramaInactivo` | Programa no activo |
| POST completar | 403 | 3002 | `Auth_Forbidden` | Promotor no aprobado o bloqueado |
| POST completar | 400 | 4027 | `BusinessRule_TareaNoRepetible` | Tarea no repetible ya tiene completado activo |
| POST completar | 400 | 4028 | `BusinessRule_MaxRepeticionesAlcanzado` | Limite de repeticiones alcanzado |
| POST completar | 400 | 4029 | `BusinessRule_TareaInactiva` | Tarea no esta activa |
| POST completar | 400 | 4030 | `BusinessRule_TareaFueraFecha` | Tarea fuera de vigencia |
| POST completar | 1001 | val. | `Validation_Required` | URL de prueba vacia |
| POST completar | 1002 | val. | `Validation_MaxLength` | URL > 2048 chars o comentario > 500 chars |
| POST completar | 1013 | val. | `Validation_InvalidUrl` | URL con formato invalido |
| GET tareas-pendientes | 404 | 2016 | `NotFound_Artista` | No existe perfil de artista |
| GET tareas-pendientes | 404 | 2019 | `NotFound_PromoPrograma` | Programa no encontrado |
| GET tareas-pendientes | 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | No es propietario del programa |
| PATCH validar | 404 | 2016 | `NotFound_Artista` | No existe perfil de artista |
| PATCH validar | 404 | 2019 | `NotFound_PromoPrograma` | Programa no encontrado |
| PATCH validar | 404 | 2022 | `NotFound_PromoTareaPromotor` | Completado no encontrado |
| PATCH validar | 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | No es propietario del programa |
| PATCH validar | 400 | 4031 | `BusinessRule_CompletadoEstadoInvalido` | Completado no esta en estado Completada |
| PATCH rechazar | 404 | 2016 | `NotFound_Artista` | No existe perfil de artista |
| PATCH rechazar | 404 | 2019 | `NotFound_PromoPrograma` | Programa no encontrado |
| PATCH rechazar | 404 | 2022 | `NotFound_PromoTareaPromotor` | Completado no encontrado |
| PATCH rechazar | 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | No es propietario del programa |
| PATCH rechazar | 400 | 4031 | `BusinessRule_CompletadoEstadoInvalido` | Completado no esta en estado Completada |
| PATCH rechazar | 1001 | val. | `Validation_Required` | Motivo de rechazo vacio |
| PATCH rechazar | 1002 | val. | `Validation_MaxLength` | Motivo > 500 chars |
| Todos | 500 | 5000 | `Internal_UnexpectedError` | Error no controlado |

---

## 10. Archivos a Crear

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs       MODIFICAR: agregar 7 constantes nuevas
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Features/
│   │   └── PromoTarea/
│   │       ├── Commands/
│   │       │   ├── CompletarTareaCommand.cs     NUEVO: Command + Handler (mismo archivo)
│   │       │   ├── ValidarTareaCommand.cs       NUEVO: Command + Handler (mismo archivo)
│   │       │   └── RechazarTareaCommand.cs      NUEVO: Command + Handler (mismo archivo)
│   │       ├── Queries/
│   │       │   ├── GetMisTareasQuery.cs          NUEVO: Query + Handler (mismo archivo)
│   │       │   └── GetTareasPendientesQuery.cs   NUEVO: Query + Handler (mismo archivo)
│   │       └── Validators/
│   │           ├── CompletarTareaCommandValidator.cs   NUEVO
│   │           ├── ValidarTareaCommandValidator.cs     NUEVO
│   │           └── RechazarTareaCommandValidator.cs    NUEVO
│   ├── Dtos/
│   │   ├── MiEstadoTareaDto.cs                 NUEVO
│   │   ├── MisTareasItemDto.cs                 NUEVO
│   │   ├── MisTareasResponseDto.cs             NUEVO
│   │   ├── CompletarTareaDto.cs                NUEVO (request body)
│   │   ├── CompletarTareaResponseDto.cs        NUEVO
│   │   ├── TareaPendienteItemDto.cs            NUEVO
│   │   ├── TareasPendientesResponseDto.cs      NUEVO
│   │   ├── ValidarTareaDto.cs                  NUEVO (request body)
│   │   ├── ValidarTareaResponseDto.cs          NUEVO
│   │   ├── RechazarTareaDto.cs                 NUEVO (request body)
│   │   └── RechazarTareaResponseDto.cs         NUEVO
│   ├── Interfaces/
│   │   └── Services/
│   │       └── IPromoTareaService.cs            NUEVO (definido en hexagonal-architecture.md)
│   └── Mapping/
│       └── PromoTareaProfile.cs                MODIFICAR: agregar 4 mappings nuevos
│
└── WePlayRises.Crowdpromotion.WebApi/
    └── Controllers/
        └── PromoTareaController.cs             NUEVO: 5 endpoints con [Authorize]
```

---

## 11. Patrones Importantes

### 11.1 Handlers que NO usan IMapper (solo los de Queries)

`GetMisTareasQueryHandler` y `GetTareasPendientesQueryHandler` no inyectan `IMapper`. Los DTOs de respuesta son construidos directamente por el `IPromoTareaService` porque requieren datos calculados (VecesCompletada, FechaPrimeraCompletada, FechaUltimaCompletada) y joins con tablas maestras que no se pueden modelar en AutoMapper sin queries adicionales.

### 11.2 Service que retorna DTO (excepcion justificada)

`IPromoTareaService.ValidarTareaAsync` retorna `Task<ValidarTareaResponseDto>` en lugar de una entidad. Esta es una excepcion justificada a la regla "services retornan entidades": la construccion del DTO requiere datos de `PromoTarea` (ImporteRecompensa, MonedaNombre, PuntosRecompensa) que estan disponibles dentro del scope transaccional. Devolver solo la entidad `PromoTareaPromotor` obligaria al Handler a cargar `PromoTarea` por separado fuera de la transaccion.

### 11.3 Transaccion atomica en ValidarTarea

La operacion de validacion implementa RNF-01 (atomicidad). El `IPromoTareaService.ValidarTareaAsync` implementa la transaccion:
- `await using var transaction = await _context.Database.BeginTransactionAsync(ct);`
- Si cualquier paso falla: `await transaction.RollbackAsync(ct); throw;`
- El `ValidarTareaCommandHandler` captura la excepcion re-lanzada en el `catch` y retorna `Internal_UnexpectedError`

### 11.4 Maquina de estados de PromoTareaPromotor

```
[Sin registro]         --POST completar-->  Completada (2)   [NUEVO registro]
Completada (2)         --PATCH validar-->   Validada (3)
Completada (2)         --PATCH rechazar-->  Rechazada (4)
Rechazada (4)          --POST completar-->  Completada (2)   [ACTUALIZA registro existente]
Validada (3) [si EsRepetible] --POST completar--> Completada (2) [NUEVO registro adicional]
```

El Handler de `CompletarTareaCommand` detecta el caso de re-envio: si `GetUltimoCompletadoAsync` retorna un registro con `EstadoTareaId == 4`, se hace update del registro existente (limpiando ComentarioValidacion y FechaValidado) en lugar de crear uno nuevo.

### 11.5 Estrategia A para campos calculados

`VecesCompletada`, `FechaPrimeraCompletada`, `FechaUltimaCompletada` NO son columnas en `PromoTareaPromotor`. Se calculan en el service:
- `GetTareasActivasConEstadoAsync`: 2 queries (tareas activas + todos los completados del promotor) + agrupacion LINQ en memoria
- `GetTareasPendientesAsync`: 3 queries (COUNT total + pendientes paginados + conteos de vecesCompletada por pares)

### 11.6 Validators sin inyeccion de servicios

Los tres validators son puramente sintacticos (formato, longitud, URL). Las validaciones de negocio (programa activo, tarea activa, limites de repeticion, aprobacion del promotor, estado del completado) se realizan en los Handlers. Esto evita queries duplicadas: el Handler carga las entidades necesarias una sola vez con cache-first via `IPromoTareaService`.

---

## 12. Checklist de Implementacion

### Paso 1: Constantes de dominio
- [ ] `ServiceResponseMessageType.cs`: Agregar `NotFound_PromoTarea = "2021"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `NotFound_PromoTareaPromotor = "2022"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaNoRepetible = "4027"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_MaxRepeticionesAlcanzado = "4028"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaInactiva = "4029"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_TareaFueraFecha = "4030"`
- [ ] `ServiceResponseMessageType.cs`: Agregar `BusinessRule_CompletadoEstadoInvalido = "4031"`

### Paso 2: DTOs de Application
- [ ] `MiEstadoTareaDto.cs`: Crear con 8 propiedades (TareaPromotorId, EstadoTareaId, EstadoTareaNombre, VecesCompletada, FechaPrimeraCompletada, FechaUltimaCompletada, UrlPruebaCompletado, ComentarioValidacion)
- [ ] `MisTareasItemDto.cs`: Crear con 13 propiedades
- [ ] `MisTareasResponseDto.cs`: Crear con ProgramaId, ProgramaTitulo, Items
- [ ] `CompletarTareaDto.cs`: Crear con UrlPruebaCompletado + ComentarioPromotor (request body)
- [ ] `CompletarTareaResponseDto.cs`: Crear con TareaPromotorId, EstadoTareaId, EstadoTareaNombre, VecesCompletada, FechaUltimaCompletada
- [ ] `TareaPendienteItemDto.cs`: Crear con 10 propiedades
- [ ] `TareasPendientesResponseDto.cs`: Crear con Items, TotalCount, Page, PageSize, TotalPages
- [ ] `ValidarTareaDto.cs`: Crear con ComentarioValidacion? (request body)
- [ ] `ValidarTareaResponseDto.cs`: Crear con TareaPromotorId, EstadoTareaId, EstadoTareaNombre, RecompensaAcreditada, MonedaNombre, PuntosAcreditados
- [ ] `RechazarTareaDto.cs`: Crear con ComentarioValidacion (request body, obligatorio)
- [ ] `RechazarTareaResponseDto.cs`: Crear con TareaPromotorId, EstadoTareaId, EstadoTareaNombre

### Paso 3: Interface de servicio
- [ ] `IPromoTareaService.cs`: Crear interfaz con 12 metodos definidos en hexagonal-architecture.md

### Paso 4: Validators (PRIMERO que Commands/Queries, para poder inyectarlos)
- [ ] `CompletarTareaCommandValidator.cs`: Crear con 7 reglas + metodo privado `BeValidUrl`
- [ ] `ValidarTareaCommandValidator.cs`: Crear con 4 reglas
- [ ] `RechazarTareaCommandValidator.cs`: Crear con 5 reglas

### Paso 5: Queries
- [ ] `GetMisTareasQuery.cs`: Crear Query + Handler en mismo archivo
  - [ ] Constructor con `?? throw` para las 2 dependencias
  - [ ] Flujo: resolver promotor -> verificar programa -> verificar inscripcion aprobada -> obtener tareas con estado
  - [ ] try-catch con `_logger.LogError` + `Internal_UnexpectedError`
- [ ] `GetTareasPendientesQuery.cs`: Crear Query + Handler en mismo archivo
  - [ ] Constructor con `?? throw` para las 2 dependencias
  - [ ] Flujo: resolver artista -> verificar programa -> verificar propiedad -> obtener pendientes paginados
  - [ ] try-catch con `_logger.LogError` + `Internal_UnexpectedError`

### Paso 6: Commands
- [ ] `CompletarTareaCommand.cs`: Crear Command + Handler en mismo archivo
  - [ ] Constructor con `?? throw` para las 4 dependencias
  - [ ] Flujo completo de 16 pasos con todas las validaciones de negocio
  - [ ] try-catch con `_logger.LogError` + `Internal_UnexpectedError`
- [ ] `ValidarTareaCommand.cs`: Crear Command + Handler en mismo archivo
  - [ ] Constructor con `?? throw` para las 4 dependencias
  - [ ] Flujo: validacion -> resolver artista -> verificar propiedad -> verificar estado -> delegar transaccion al service
  - [ ] try-catch con `_logger.LogError` + `Internal_UnexpectedError`
- [ ] `RechazarTareaCommand.cs`: Crear Command + Handler en mismo archivo
  - [ ] Constructor con `?? throw` para las 4 dependencias
  - [ ] Flujo: validacion -> resolver artista -> verificar propiedad -> verificar estado -> rechazar
  - [ ] try-catch con `_logger.LogError` + `Internal_UnexpectedError`

### Paso 7: AutoMapper Profile
- [ ] `PromoTareaProfile.cs`: Agregar los 4 mappings nuevos al Profile existente
  - [ ] `PromoTarea -> MisTareasItemDto`
  - [ ] `PromoTareaPromotor -> CompletarTareaResponseDto`
  - [ ] `PromoTareaPromotor -> ValidarTareaResponseDto`
  - [ ] `PromoTareaPromotor -> RechazarTareaResponseDto`

### Paso 8: Controlador
- [ ] `PromoTareaController.cs`: Crear con 5 endpoints todos con `[Authorize]`
  - [ ] Constructor con `?? throw` para las 3 dependencias
  - [ ] `GetMisTareas`: GET, resolver userId, construir query, FromServiceResponse
  - [ ] `CompletarTarea`: POST, resolver userId, mapear body + params, FromServiceResponse
  - [ ] `GetTareasPendientes`: GET, resolver userId, normalizar paginacion, FromServiceResponse
  - [ ] `ValidarTarea`: PATCH, resolver userId, construir command, FromServiceResponse
  - [ ] `RechazarTarea`: PATCH, resolver userId, construir command, FromServiceResponse

### Paso 9: Verificacion arquitectural
- [ ] Ningun Handler inyecta `CrowdpromotionContext` directamente
- [ ] Ningun Handler inyecta `IRequestCacheService` directamente (solo el service)
- [ ] Todos los constructores tienen `?? throw new ArgumentNullException` para cada parametro
- [ ] Todos los validators usan `ServiceResponseMessageType.X` (NO strings literales)
- [ ] Todos los Handlers tienen try-catch con `_logger.LogError` y retornan `Internal_UnexpectedError`
- [ ] Todos los Handlers retornan `ServiceResponse<T>` (NUNCA throw exception al cliente)
- [ ] Validators NUNCA inyectan servicios ni DbContext
- [ ] Handler + Command/Query estan en el MISMO archivo .cs
- [ ] Validators estan en carpeta `Validators/` separada
