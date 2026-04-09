# Plan CQRS: cp-perfil-promotor

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-perfil-promotor (US-CP-01)
**Bounded Context:** Promotor

---

## Notas Criticas de Contexto

### Strongly Typed ID
La entidad `Promotor` usa `PromotorId` (strongly typed ID). En los DTOs de response el campo `Id` se expone como `Guid` via `.Value`. El profile AutoMapper debe usar `.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))`.

### Patron de respuesta HTTP del proyecto
El `BaseLoggerController` infiere el `HttpStatusCode` a partir del prefijo del `ErrorCode`:
- `0xxx` -> 200 OK
- `1xxx` -> 400 Bad Request
- `2xxx` -> 404 Not Found
- `3xxx` -> 401 Unauthorized
- `4xxx` -> 409 Conflict
- `5xxx` -> 500 Internal Server Error

Por tanto, los `ServiceResponseMessage` NO necesitan `HttpStatusCode` explicitico cuando se usa `ServiceResponseMessageType` constants correctamente. El controller llama `FromServiceResponse(result)` y el codigo HTTP se deriva automaticamente.

### Obtencion del UserId desde JWT
El controller usa `ICurrentUserService` (ya usado en `CampaniasController`) para obtener el `UserId` del token JWT. El UserId se inyecta en el Command/Query como propiedad `string? UserId` o `string UserId`. No proviene del body.

### Campos calculados en PromotorDto
`TotalProgramasActivos`, `TotalComisionesGanadas` y `MonedaComisiones` no existen en la entidad `Promotor`. El Handler los asigna directamente al DTO tras llamadas al Service. AutoMapper ignora estos campos con `.ForMember(..., opt => opt.Ignore())`.

### Validacion de TipoPromotorId (MVP)
Los valores validos de `MaestraTipoPromotor` son fijos y conocidos: `{1, 2, 3, 4}`. El Validator usa validacion sincrona `Must(id => id >= 1 && id <= 4)` sin consulta a BD, segun la recomendacion del plan hexagonal para MVP.

### Metodo DesactivarAsync en el Service
El plan hexagonal define `IPromotorService.DesactivarWithProgramasAsync(promotorId, ct)` que retorna `int` (cantidad de programas dados de baja). El Handler recibe este valor y lo asigna al DTO.

### ICurrentUserService
Ya existe en el proyecto (`WePlayRises.BuildingBlocks.Kernel.Services.ICurrentUserService`) con propiedad `UserId` y es usado en `CampaniasController`. El nuevo controller inyecta el mismo servicio.

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Archivo | Request | Response |
|-----------|------|---------|---------|----------|
| Crear perfil de promotor | Command | `Commands/CreatePromotorCommand.cs` | `CreatePromotorCommand` | `ServiceResponse<PromotorCreatedResultDto>` |
| Obtener mi perfil | Query | `Queries/GetPromotorMeQuery.cs` | `GetPromotorMeQuery` | `ServiceResponse<PromotorDto>` |
| Actualizar mi perfil | Command | `Commands/UpdatePromotorCommand.cs` | `UpdatePromotorCommand` | `ServiceResponse<PromotorUpdatedResultDto>` |
| Desactivar mi perfil | Command | `Commands/DesactivarPromotorCommand.cs` | `DesactivarPromotorCommand` | `ServiceResponse<PromotorDesactivadoResultDto>` |

---

## 2. Commands

### 2.1 CreatePromotorCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/CreatePromotorCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<PromotorCreatedResultDto>>`

| Propiedad | Tipo | Requerido | Fuente | Descripcion |
|-----------|------|-----------|--------|-------------|
| NombrePublico | `string` | Si | Body | Nombre publico del promotor. `= null!` |
| TipoPromotorId | `int` | Si | Body | FK a MaestraTipoPromotor (1-4) |
| EmailContacto | `string?` | No | Body | Email de contacto opcional |
| UrlSitioWeb | `string?` | No | Body | URL del sitio web opcional |
| UrlInstagram | `string?` | No | Body | URL Instagram opcional |
| UrlTikTok | `string?` | No | Body | URL TikTok opcional |
| UrlYouTube | `string?` | No | Body | URL YouTube opcional |
| UrlTwitter | `string?` | No | Body | URL Twitter/X opcional |
| UserId | `string?` | Si (interno) | Token JWT via controller | Claim `sub` del JWT. NO proviene del body |

#### Handler: CreatePromotorCommandHandler

**Implementa:** `IRequestHandler<CreatePromotorCommand, ServiceResponse<PromotorCreatedResultDto>>`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_promotorService` | `IPromotorService` | Verificar unicidad, crear, obtener entidad creada |
| `_mapper` | `IMapper` | Mapear Command -> Entidad, Entidad -> DTO |
| `_validator` | `IValidator<CreatePromotorCommand>` | Validacion FluentValidation |
| `_logger` | `ILogger<CreatePromotorCommandHandler>` | Logging de exito y errores |

**Usings requeridos:**
```
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
```

**Flujo del Handle (paso a paso):**

```
1. VALIDACION
   - Llamar _validator.ValidateAsync(request, ct)
   - Si !IsValid -> LogWarning con errores
   - Retornar ServiceResponse<PromotorCreatedResultDto> { Messages = validationResult.GetServiceResponseMessages() }

2. VERIFICAR USERID
   - Si string.IsNullOrEmpty(request.UserId):
     Retornar BadRequestServiceResponse("Token invalido", ServiceResponseMessageType.Auth_Unauthorized)

3. VERIFICAR UNICIDAD
   - var existente = await _promotorService.GetByUserIdAsync(request.UserId, ct)
   - Si existente != null:
     LogWarning("Promotor already exists for UserId {UserId}", request.UserId)
     Retornar ServiceResponse con Messages:
       { Message = "Ya tienes un perfil de promotor creado",
         ErrorCode = ServiceResponseMessageType.BusinessRule_PromotorAlreadyExists }

4. RESOLVER FANPROFILEID
   - Llamar _promotorService.GetFanProfileIdByUserIdAsync(request.UserId, ct)
   - Resultado puede ser null (flujo FA-02, no es error bloqueante)
   - fanProfileId = FanProfileId? (nullable)

5. MAPEAR COMMAND -> ENTIDAD
   - var promotor = _mapper.Map<Promotor>(request)
   - promotor.Id = PromotorId.CreateNew()
   - promotor.UserId = request.UserId
   - promotor.FanProfileId = fanProfileId  (asignado manualmente, no AutoMapper)
   - promotor.EsActivo = true
   - promotor.FechaCreacion = DateTime.UtcNow

6. CREAR VIA SERVICE (ATOMICO: Promotor + PromotorWallet en transaccion)
   - var promotorId = await _promotorService.CreateWithWalletAsync(promotor, ct)

7. OBTENER ENTIDAD CREADA CON NAVEGACIONES
   - var creado = await _promotorService.GetByIdAsync(promotorId, ct)

8. MAPEAR ENTIDAD -> DTO
   - var dto = _mapper.Map<PromotorCreatedResultDto>(creado)

9. RETORNAR EXITO
   - LogInformation("Promotor created for UserId {UserId} with PromotorId {PromotorId}")
   - Retornar ServiceResponse<PromotorCreatedResultDto>:
     { Data = dto,
       Messages = [{ Message = "Perfil de promotor creado",
                     ErrorCode = ServiceResponseMessageType.Created }] }

CATCH (Exception ex):
   - _logger.LogError(ex, "Error creating Promotor for UserId {UserId}", request.UserId)
   - Retornar ValidateExtensions.InternalServerErrorServiceResponse<PromotorCreatedResultDto>(
       "Error inesperado al crear el perfil de promotor",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

**Nota sobre paso 4 (FanProfileId):** El `IPromotorService` debe exponer un metodo `GetFanProfileIdByUserIdAsync(string userId, CancellationToken ct)` que internamente consulte el modulo UserAccess. Si este metodo no es viable cross-modulo en MVP, simplificar a `FanProfileId = null` directamente (la entidad lo acepta como nullable y el constraint de BD no lo requiere).

---

### 2.2 UpdatePromotorCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/UpdatePromotorCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<PromotorUpdatedResultDto>>`

| Propiedad | Tipo | Requerido | Fuente | Descripcion |
|-----------|------|-----------|--------|-------------|
| NombrePublico | `string` | Si | Body | `= null!`. Editable. |
| EmailContacto | `string?` | No | Body | Editable |
| UrlSitioWeb | `string?` | No | Body | Editable |
| UrlInstagram | `string?` | No | Body | Editable |
| UrlTikTok | `string?` | No | Body | Editable |
| UrlYouTube | `string?` | No | Body | Editable |
| UrlTwitter | `string?` | No | Body | Editable |
| UserId | `string?` | Si (interno) | Token JWT via controller | NO en body. NO incluir TipoPromotorId |

**CRITICO:** `TipoPromotorId` no existe en este Command. No es editable post-creacion.

#### Handler: UpdatePromotorCommandHandler

**Implementa:** `IRequestHandler<UpdatePromotorCommand, ServiceResponse<PromotorUpdatedResultDto>>`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_promotorService` | `IPromotorService` | Buscar promotor por UserId, actualizar |
| `_mapper` | `IMapper` | Mapear entidad a PromotorUpdatedResultDto |
| `_validator` | `IValidator<UpdatePromotorCommand>` | Validacion FluentValidation |
| `_logger` | `ILogger<UpdatePromotorCommandHandler>` | Logging |

**Flujo del Handle (paso a paso):**

```
1. VALIDACION
   - Llamar _validator.ValidateAsync(request, ct)
   - Si !IsValid -> LogWarning + retornar ServiceResponse con errores

2. VERIFICAR USERID
   - Si string.IsNullOrEmpty(request.UserId):
     Retornar BadRequestServiceResponse("Token invalido", ServiceResponseMessageType.Auth_Unauthorized)

3. BUSCAR PROMOTOR
   - var promotor = await _promotorService.GetByUserIdAsync(request.UserId, ct)
   - Si promotor == null:
     Retornar NotFoundServiceResponse<PromotorUpdatedResultDto>(
       "No tienes un perfil de promotor",
       ServiceResponseMessageType.NotFound_Promotor)

4. ACTUALIZAR CAMPOS EN LA ENTIDAD (NO AutoMapper para update parcial)
   - promotor.NombrePublico = request.NombrePublico
   - promotor.EmailContacto = request.EmailContacto
   - promotor.UrlSitioWeb = request.UrlSitioWeb
   - promotor.UrlInstagram = request.UrlInstagram
   - promotor.UrlTikTok = request.UrlTikTok
   - promotor.UrlYouTube = request.UrlYouTube
   - promotor.UrlTwitter = request.UrlTwitter
   - promotor.FechaActualizacion = DateTime.UtcNow  (logica de negocio en Handler)

5. PERSISTIR VIA SERVICE
   - await _promotorService.UpdateAsync(promotor, ct)

6. MAPEAR -> DTO
   - var dto = _mapper.Map<PromotorUpdatedResultDto>(promotor)

7. RETORNAR EXITO
   - LogInformation("Promotor updated for UserId {UserId}", request.UserId)
   - Retornar ServiceResponse<PromotorUpdatedResultDto>:
     { Data = dto,
       Messages = [{ Message = "Perfil actualizado",
                     ErrorCode = ServiceResponseMessageType.Updated }] }

CATCH (Exception ex):
   - _logger.LogError(ex, "Error updating Promotor for UserId {UserId}", request.UserId)
   - Retornar ValidateExtensions.InternalServerErrorServiceResponse<PromotorUpdatedResultDto>(
       "Error inesperado al actualizar el perfil de promotor",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

---

### 2.3 DesactivarPromotorCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/DesactivarPromotorCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands`

#### Command

**Implementa:** `IRequest<ServiceResponse<PromotorDesactivadoResultDto>>`

| Propiedad | Tipo | Requerido | Fuente | Descripcion |
|-----------|------|-----------|--------|-------------|
| UserId | `string?` | Si (interno) | Token JWT via controller | Claim `sub`. NO hay body en este endpoint. |

#### Handler: DesactivarPromotorCommandHandler

**Implementa:** `IRequestHandler<DesactivarPromotorCommand, ServiceResponse<PromotorDesactivadoResultDto>>`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_promotorService` | `IPromotorService` | Buscar promotor, desactivar con cascade |
| `_logger` | `ILogger<DesactivarPromotorCommandHandler>` | Logging |

**Nota:** No se inyecta `IMapper` ni `IValidator` en este handler porque no hay body que mapear ni validar (el command solo tiene `UserId`). La verificacion del estado activo es logica de negocio del Handler.

**Flujo del Handle (paso a paso):**

```
1. VERIFICAR USERID
   - Si string.IsNullOrEmpty(request.UserId):
     Retornar BadRequestServiceResponse("Token invalido", ServiceResponseMessageType.Auth_Unauthorized)

2. BUSCAR PROMOTOR
   - var promotor = await _promotorService.GetByUserIdAsync(request.UserId, ct)
   - Si promotor == null:
     Retornar NotFoundServiceResponse<PromotorDesactivadoResultDto>(
       "No tienes un perfil de promotor",
       ServiceResponseMessageType.NotFound_Promotor)

3. VERIFICAR ESTADO ACTIVO (logica de negocio en Handler)
   - Si !promotor.EsActivo:
     LogWarning("Promotor {PromotorId} already inactive", promotor.Id.Value)
     Retornar ServiceResponse<PromotorDesactivadoResultDto>:
       { Messages = [{ Message = "El perfil de promotor ya esta desactivado",
                       ErrorCode = ServiceResponseMessageType.BusinessRule_PromotorAlreadyInactive }] }

4. DESACTIVAR VIA SERVICE (ATOMICO: Promotor + PromoProgramaPromotor en transaccion)
   - int programasDadosDeBaja = await _promotorService.DesactivarWithProgramasAsync(promotor.Id, ct)

5. CONSTRUIR DTO DE RESULTADO (manual, sin AutoMapper)
   - var dto = new PromotorDesactivadoResultDto
     {
         Id = promotor.Id.Value,
         EsActivo = false,
         ProgramasDadosDeBaja = programasDadosDeBaja
     }

6. RETORNAR EXITO
   - LogInformation("Promotor {PromotorId} deactivated. Programs deactivated: {Count}",
       promotor.Id.Value, programasDadosDeBaja)
   - Retornar ServiceResponse<PromotorDesactivadoResultDto>:
     { Data = dto,
       Messages = [{ Message = "Perfil desactivado",
                     ErrorCode = ServiceResponseMessageType.Updated }] }

CATCH (Exception ex):
   - _logger.LogError(ex, "Error deactivating Promotor for UserId {UserId}", request.UserId)
   - Retornar ValidateExtensions.InternalServerErrorServiceResponse<PromotorDesactivadoResultDto>(
       "Error inesperado al desactivar el perfil de promotor",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

---

## 3. Queries

### 3.1 GetPromotorMeQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Queries/GetPromotorMeQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries`

#### Query

**Implementa:** `IRequest<ServiceResponse<PromotorDto>>`

| Propiedad | Tipo | Fuente | Descripcion |
|-----------|------|--------|-------------|
| UserId | `string` | Token JWT via controller | `= null!`. Claim `sub` del JWT |

#### Handler: GetPromotorMeQueryHandler

**Implementa:** `IRequestHandler<GetPromotorMeQuery, ServiceResponse<PromotorDto>>`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_promotorService` | `IPromotorService` | Obtener promotor por UserId con wallet, calcular estadisticas |
| `_mapper` | `IMapper` | Mapear Promotor -> PromotorDto (campos base) |
| `_logger` | `ILogger<GetPromotorMeQueryHandler>` | Logging |

**Flujo del Handle (paso a paso):**

```
1. BUSCAR PROMOTOR (con wallet cargada via Include)
   - var promotor = await _promotorService.GetByUserIdAsync(request.UserId, ct)
   - Si promotor == null:
     Retornar NotFoundServiceResponse<PromotorDto>(
       "No tienes un perfil de promotor",
       ServiceResponseMessageType.NotFound_Promotor)

2. MAPEAR ENTIDAD -> DTO (campos base via AutoMapper)
   - var dto = _mapper.Map<PromotorDto>(promotor)
   - (AutoMapper mapea: Id, NombrePublico, TipoPromotorId, TipoPromotorNombre, EmailContacto, todas las URLs, EsActivo, FechaCreacion)
   - (AutoMapper ignora: TotalProgramasActivos, TotalComisionesGanadas, MonedaComisiones)

3. CALCULAR TotalProgramasActivos
   - int count = await _promotorService.GetProgramasActivosCountAsync(promotor.Id, ct)
   - dto.TotalProgramasActivos = count

4. CALCULAR TotalComisionesGanadas y MonedaComisiones
   - var walletEur = await _promotorService.GetWalletEurAsync(promotor.Id, ct)
   - dto.TotalComisionesGanadas = walletEur?.TotalGanado ?? 0m
   - dto.MonedaComisiones = "EUR"  (constante en MVP)

5. RETORNAR EXITO
   - Retornar ServiceResponse<PromotorDto> { Data = dto }
   (sin Messages = lista vacia segun contrato GET)

CATCH (Exception ex):
   - _logger.LogError(ex, "Error getting Promotor for UserId {UserId}", request.UserId)
   - Retornar ValidateExtensions.InternalServerErrorServiceResponse<PromotorDto>(
       "Error inesperado al obtener el perfil de promotor",
       ServiceResponseMessageType.Internal_UnexpectedError)
```

**Nota sobre IPromotorService para GetPromotorMeQuery:** El plan hexagonal define `GetByUserIdAsync` (usa cache request-scoped) y metodos separados `GetProgramasActivosCountAsync` y `GetWalletEurAsync`. El Handler orquesta estas tres llamadas. El Service implementa el caching para evitar queries duplicados.

---

## 4. Validators

### 4.1 CreatePromotorCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Validators/CreatePromotorCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Validators`

**Extiende:** `AbstractValidator<CreatePromotorCommand>`

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

El Validator para CreatePromotorCommand NO inyecta servicios. La validacion de unicidad de `UserId` (BusinessRule_PromotorAlreadyExists) se realiza en el Handler, no en el Validator, porque es una regla de negocio y no una regla de formato/estructura. La validacion de `TipoPromotorId` usa un conjunto de valores validos hardcodeado para MVP.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constant) |
|-------|----------------------|---------|----------------------|
| NombrePublico | `.NotEmpty()` | "El nombre publico es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| NombrePublico | `.MinimumLength(3)` | "El nombre debe tener al menos 3 caracteres" | `ServiceResponseMessageType.Validation_MinLength` |
| NombrePublico | `.MaximumLength(200)` | "El nombre publico no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| TipoPromotorId | `.GreaterThan(0)` | "El tipo de promotor es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| TipoPromotorId | `.Must(id => id >= 1 && id <= 4)` cuando GreaterThan(0) | "El tipo de promotor no existe" | `ServiceResponseMessageType.Validation_ForeignKeyNotFound` |
| EmailContacto | `.EmailAddress()` con `.When(x => !string.IsNullOrEmpty(x.EmailContacto))` | "El email de contacto no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidEmail` |
| EmailContacto | `.MaximumLength(200)` con `.When(x => !string.IsNullOrEmpty(x.EmailContacto))` | "El email de contacto no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlSitioWeb | `.Must(BeValidUrl)` con `.When(x => !string.IsNullOrEmpty(x.UrlSitioWeb))` | "La URL del sitio web no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlSitioWeb | `.MaximumLength(300)` con `.When(x => !string.IsNullOrEmpty(x.UrlSitioWeb))` | "La URL del sitio web no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlInstagram | `.Must(BeValidUrl)` con `.When(x => !string.IsNullOrEmpty(x.UrlInstagram))` | "La URL de Instagram no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlInstagram | `.MaximumLength(300)` con `.When(x => !string.IsNullOrEmpty(x.UrlInstagram))` | "La URL de Instagram no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlTikTok | `.Must(BeValidUrl)` con `.When(x => !string.IsNullOrEmpty(x.UrlTikTok))` | "La URL de TikTok no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlTikTok | `.MaximumLength(300)` con `.When(x => !string.IsNullOrEmpty(x.UrlTikTok))` | "La URL de TikTok no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlYouTube | `.Must(BeValidUrl)` con `.When(x => !string.IsNullOrEmpty(x.UrlYouTube))` | "La URL de YouTube no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlYouTube | `.MaximumLength(300)` con `.When(x => !string.IsNullOrEmpty(x.UrlYouTube))` | "La URL de YouTube no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlTwitter | `.Must(BeValidUrl)` con `.When(x => !string.IsNullOrEmpty(x.UrlTwitter))` | "La URL de Twitter/X no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlTwitter | `.MaximumLength(300)` con `.When(x => !string.IsNullOrEmpty(x.UrlTwitter))` | "La URL de Twitter/X no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

**Metodo privado auxiliar (igual que `CreateCampaniaCommandValidator`):**
```
private static bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url)) return true;
    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

---

### 4.2 UpdatePromotorCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Validators/UpdatePromotorCommandValidator.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Features.Promotor.Validators`

**Extiende:** `AbstractValidator<UpdatePromotorCommand>`

**Sin dependencias inyectadas.** Constructor sin parametros.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode (Constant) |
|-------|----------------------|---------|----------------------|
| NombrePublico | `.NotEmpty()` | "El nombre publico es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| NombrePublico | `.MinimumLength(3)` | "El nombre debe tener al menos 3 caracteres" | `ServiceResponseMessageType.Validation_MinLength` |
| NombrePublico | `.MaximumLength(200)` | "El nombre publico no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| EmailContacto | `.EmailAddress()` con `.When(x => !string.IsNullOrEmpty(x.EmailContacto))` | "El email de contacto no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidEmail` |
| EmailContacto | `.MaximumLength(200)` con `.When(x => !string.IsNullOrEmpty(x.EmailContacto))` | "El email de contacto no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlSitioWeb | `.Must(BeValidUrl)` con `.When(...)` | "La URL del sitio web no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlSitioWeb | `.MaximumLength(300)` con `.When(...)` | "La URL del sitio web no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlInstagram | `.Must(BeValidUrl)` con `.When(...)` | "La URL de Instagram no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlInstagram | `.MaximumLength(300)` con `.When(...)` | "La URL de Instagram no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlTikTok | `.Must(BeValidUrl)` con `.When(...)` | "La URL de TikTok no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlTikTok | `.MaximumLength(300)` con `.When(...)` | "La URL de TikTok no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlYouTube | `.Must(BeValidUrl)` con `.When(...)` | "La URL de YouTube no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlYouTube | `.MaximumLength(300)` con `.When(...)` | "La URL de YouTube no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| UrlTwitter | `.Must(BeValidUrl)` con `.When(...)` | "La URL de Twitter/X no tiene formato valido" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| UrlTwitter | `.MaximumLength(300)` con `.When(...)` | "La URL de Twitter/X no puede superar los 300 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

**Mismo metodo privado `BeValidUrl` que el CreateValidator.**

---

## 5. AutoMapper Profile

### 5.1 PromotorProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromotorProfile.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Mapping`

**Extiende:** `Profile`

**Usings requeridos:**
```
using AutoMapper;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Domain.Model;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
```

**Mappings a definir:**

#### A) `Promotor` -> `PromotorCreatedResultDto`
```
CreateMap<Promotor, PromotorCreatedResultDto>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.TipoPromotorNombre,
               opt => opt.Ignore());  // Resuelto manualmente en Handler o via navegacion
```
**Nota:** `TipoPromotorNombre` requiere la navegacion a `MaestraTipoPromotor`. Si la navegacion no esta cargada via Include, se puede resolver en el Handler antes de mapear, asignando el valor al DTO directamente.

#### B) `Promotor` -> `PromotorDto`
```
CreateMap<Promotor, PromotorDto>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.TipoPromotorNombre,
               opt => opt.Ignore())         // Resuelto desde navegacion si esta cargada
    .ForMember(dest => dest.TotalProgramasActivos,
               opt => opt.Ignore())         // Calculado en Handler via Service
    .ForMember(dest => dest.TotalComisionesGanadas,
               opt => opt.Ignore())         // Calculado en Handler via Service
    .ForMember(dest => dest.MonedaComisiones,
               opt => opt.Ignore());        // Constante "EUR" asignada en Handler
```

#### C) `Promotor` -> `PromotorUpdatedResultDto`
```
CreateMap<Promotor, PromotorUpdatedResultDto>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.FechaActualizacion,
               opt => opt.MapFrom(src => src.FechaActualizacion ?? DateTime.UtcNow));
```

#### D) `CreatePromotorCommand` -> `Promotor`
```
CreateMap<CreatePromotorCommand, Promotor>()
    .ForMember(dest => dest.Id,
               opt => opt.Ignore())          // Generado en Handler: PromotorId.CreateNew()
    .ForMember(dest => dest.UserId,
               opt => opt.Ignore())          // Asignado manualmente en Handler
    .ForMember(dest => dest.FanProfileId,
               opt => opt.Ignore())          // Resuelto en Handler
    .ForMember(dest => dest.EsActivo,
               opt => opt.Ignore())          // Establecido en Handler: true
    .ForMember(dest => dest.FechaCreacion,
               opt => opt.Ignore())          // Establecido en Handler: DateTime.UtcNow
    .ForMember(dest => dest.FechaActualizacion,
               opt => opt.Ignore())          // null en creacion
    .ForMember(dest => dest.SeguidoresTotales,
               opt => opt.Ignore())          // campo legacy, no en esta US
    .ForMember(dest => dest.Programas,
               opt => opt.Ignore())          // navegacion
    .ForMember(dest => dest.Wallets,
               opt => opt.Ignore());         // navegacion
```

**Nota sobre `TipoPromotorNombre` en `PromotorCreatedResultDto`:** La entidad `Promotor` no tiene una propiedad de navegacion cargada a `MaestraTipoPromotor` en el codebase actual. Hay dos opciones para el Handler:
- Opcion A (recomendada): Despues de llamar `CreateWithWalletAsync`, llamar a `GetByIdAsync` que incluya la navegacion a `MaestraTipoPromotor`, y dejar que AutoMapper resuelva `TipoPromotorNombre` desde esa navegacion via perfil extendido.
- Opcion B (simple): Asignar `dto.TipoPromotorNombre` manualmente en el Handler usando un diccionario de constantes con los valores del seed: `{1: "Fan Embajador", 2: "Influencer", 3: "Medio / Blog", 4: "Profesional Marketing"}`.

Para MVP, usar **Opcion B** dado que los valores son fijos. El Handler declara un diccionario `private static readonly Dictionary<int, string> _tipoPromotorNombres` con los valores seed.

---

## 6. Controller

### 6.1 PromotorController

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromotorController.cs`

**Namespace:** `WePlayRises.Crowdpromotion.WebApi.Controllers`

**Extiende:** `BaseLoggerController`

**Route:** `[Route("api/crowdpromotion/[controller]")]` -> `/api/crowdpromotion/promotor`

**Usings requeridos:**
```
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;
```

**Dependencias del constructor (con `?? throw`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_mediator` | `IMediator` | Despachar Commands y Queries |
| `_currentUser` | `ICurrentUserService` | Obtener UserId del JWT |
| `_logger` | `ILogger<PromotorController>` | Logging (pasado a base) |

#### Acciones del Controller

##### POST /api/crowdpromotion/promotor

```
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Create([FromBody] CreatePromotorRequestDto request, CancellationToken cancellationToken)
```

**Logica del action:**
1. Obtener `userId` de `_currentUser.UserId` (string del claim `sub`)
2. Si null: retornar `Unauthorized` con `ServiceResponseMessage` y `ErrorCode = Auth_Unauthorized`
3. Construir `CreatePromotorCommand` desde `request` + `UserId = userId`
4. `var result = await _mediator.Send(command, cancellationToken)`
5. Si `result.IsSuccess`: retornar `CreatedAtAction(nameof(GetMe), null, result)` (HTTP 201)
6. Si no: retornar `FromServiceResponse(result)`

**Nota sobre `ICurrentUserService.UserId`:** En `CampaniasController` se usa `_currentUser.UserId` que retorna `Guid?`. Para el modulo Crowdpromotion el `UserId` es `string` (Identity User ID). Verificar el tipo exacto de `ICurrentUserService.UserId` en el BuildingBlock. Si retorna `Guid?`, usar `.ToString()`. Si retorna `string?` directamente, usar tal cual. El Command acepta `string?`.

##### GET /api/crowdpromotion/promotor/me

```
[HttpGet("me")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
```

**Logica del action:**
1. Obtener `userId` de `_currentUser.UserId`
2. Si null: retornar `Unauthorized`
3. Construir `GetPromotorMeQuery { UserId = userId }`
4. `var result = await _mediator.Send(query, cancellationToken)`
5. Retornar `FromServiceResponse(result)`

##### PUT /api/crowdpromotion/promotor/me

```
[HttpPut("me")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Update([FromBody] UpdatePromotorRequestDto request, CancellationToken cancellationToken)
```

**Logica del action:**
1. Obtener `userId` de `_currentUser.UserId`
2. Si null: retornar `Unauthorized`
3. Construir `UpdatePromotorCommand` desde `request` + `UserId = userId`
4. `var result = await _mediator.Send(command, cancellationToken)`
5. Retornar `FromServiceResponse(result)`

##### PATCH /api/crowdpromotion/promotor/me/desactivar

```
[HttpPatch("me/desactivar")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Desactivar(CancellationToken cancellationToken)
```

**Logica del action:**
1. Obtener `userId` de `_currentUser.UserId`
2. Si null: retornar `Unauthorized`
3. Construir `DesactivarPromotorCommand { UserId = userId }`
4. `var result = await _mediator.Send(command, cancellationToken)`
5. Retornar `FromServiceResponse(result)`

---

## 7. DTOs (Application Layer)

Los DTOs ya estan definidos en `api-contracts.md`. Esta seccion resume su ubicacion y contenido.

### 7.1 CreatePromotorRequestDto (Request - usado en Controller)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromotorRequestDto.cs`

Propiedades: `NombrePublico`, `TipoPromotorId`, `EmailContacto?`, `UrlSitioWeb?`, `UrlInstagram?`, `UrlTikTok?`, `UrlYouTube?`, `UrlTwitter?`

### 7.2 UpdatePromotorRequestDto (Request - usado en Controller)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromotorRequestDto.cs`

Propiedades: `NombrePublico`, `EmailContacto?`, `UrlSitioWeb?`, `UrlInstagram?`, `UrlTikTok?`, `UrlYouTube?`, `UrlTwitter?`

### 7.3 PromotorCreatedResultDto (Response)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorCreatedResultDto.cs`

Propiedades: `Id (Guid)`, `NombrePublico`, `TipoPromotorNombre`, `EsActivo`, `FechaCreacion`

### 7.4 PromotorDto (Response - GET /me)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDto.cs`

Propiedades: `Id (Guid)`, `NombrePublico`, `TipoPromotorId`, `TipoPromotorNombre`, `EmailContacto?`, `UrlSitioWeb?`, `UrlInstagram?`, `UrlTikTok?`, `UrlYouTube?`, `UrlTwitter?`, `EsActivo`, `FechaCreacion`, `TotalProgramasActivos`, `TotalComisionesGanadas`, `MonedaComisiones`

### 7.5 PromotorUpdatedResultDto (Response)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorUpdatedResultDto.cs`

Propiedades: `Id (Guid)`, `NombrePublico`, `FechaActualizacion`

### 7.6 PromotorDesactivadoResultDto (Response)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDesactivadoResultDto.cs`

Propiedades: `Id (Guid)`, `EsActivo (= false)`, `ProgramasDadosDeBaja (int)`

---

## 8. Interfaces de Servicio (Referencia para el Plan)

Las interfaces de servicio se definen en `Application/Interfaces/Services/` segun el plan hexagonal. Se listan aqui para referencia del Handler:

### IPromotorService (Application/Interfaces/Services/)

| Metodo | Retorno | Uso en |
|--------|---------|--------|
| `GetByIdAsync(PromotorId id, CancellationToken ct)` | `Promotor?` | CreateHandler paso 7 |
| `GetByUserIdAsync(string userId, CancellationToken ct)` | `Promotor?` | Todos los Handlers. Con cache request-scoped. |
| `ExistsByUserIdAsync(string userId, CancellationToken ct)` | `bool` | CreateHandler paso 3 (alternativa a GetByUserIdAsync) |
| `CreateWithWalletAsync(Promotor entity, CancellationToken ct)` | `PromotorId` | CreateHandler paso 6. Atomico. |
| `UpdateAsync(Promotor entity, CancellationToken ct)` | `void` | UpdateHandler paso 5 |
| `DesactivarWithProgramasAsync(PromotorId id, CancellationToken ct)` | `int` | DesactivarHandler paso 4. Retorna count de programas. |
| `GetProgramasActivosCountAsync(PromotorId id, CancellationToken ct)` | `int` | GetPromotorMeHandler paso 3 |
| `GetWalletEurAsync(PromotorId id, CancellationToken ct)` | `PromotorWallet?` | GetPromotorMeHandler paso 4 |

---

## 9. Patron de Caching (ADR-006) Aplicado

El `IRequestCacheService` se usa en `PromotorService` (Infra), no en los Handlers. Los Handlers siempre llaman al Service, que internamente gestiona el cache.

```
FLUJO CreatePromotorCommand:
  Validator -> (no llama Service, no necesita cache)
  Handler paso 3 -> PromotorService.GetByUserIdAsync(userId)
    └─> RequestCache key: "promotor:userid:{userId}"
        └─> MISS -> PromotorRepository -> DB
        └─> Almacena en cache

FLUJO UpdatePromotorCommand / GetPromotorMeQuery / DesactivarPromotorCommand:
  Handler -> PromotorService.GetByUserIdAsync(userId)
    └─> RequestCache key: "promotor:userid:{userId}"
        └─> HIT si ya fue consultado en el mismo request HTTP
```

**Nota:** El cache es de alcance de request (IRequestCacheService es Scoped). Cada request HTTP comienza con cache vacio.

---

## 10. Estructura de Archivos a Crear

```
src/api/Modules/Crowdpromotion/
|
├── WePlayRises.Crowdpromotion.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs              [CREAR - ver api-contracts.md seccion 4]
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Dtos/
│   │   ├── CreatePromotorRequestDto.cs                [CREAR]
│   │   ├── UpdatePromotorRequestDto.cs                [CREAR]
│   │   ├── PromotorCreatedResultDto.cs                [CREAR]
│   │   ├── PromotorDto.cs                             [CREAR]
│   │   ├── PromotorUpdatedResultDto.cs                [CREAR]
│   │   └── PromotorDesactivadoResultDto.cs            [CREAR]
│   ├── Features/
│   │   └── Promotor/
│   │       ├── Commands/
│   │       │   ├── CreatePromotorCommand.cs           [CREAR - Command + Handler]
│   │       │   ├── UpdatePromotorCommand.cs           [CREAR - Command + Handler]
│   │       │   └── DesactivarPromotorCommand.cs       [CREAR - Command + Handler]
│   │       ├── Queries/
│   │       │   └── GetPromotorMeQuery.cs              [CREAR - Query + Handler]
│   │       └── Validators/
│   │           ├── CreatePromotorCommandValidator.cs  [CREAR]
│   │           └── UpdatePromotorCommandValidator.cs  [CREAR]
│   ├── Interfaces/
│   │   └── Services/
│   │       ├── IPromotorService.cs                    [CREAR - ver hexagonal-architecture.md]
│   │       └── IPromotorWalletService.cs              [CREAR - ver hexagonal-architecture.md]
│   └── Mapping/
│       └── PromotorProfile.cs                         [CREAR]
│
├── WePlayRises.Crowdpromotion.Infra/
│   ├── Repositories/
│   │   ├── PromotorRepository.cs                      [CREAR - ver hexagonal-architecture.md]
│   │   └── PromotorWalletRepository.cs                [CREAR - ver hexagonal-architecture.md]
│   ├── Services/
│   │   ├── PromotorService.cs                         [CREAR - ver hexagonal-architecture.md]
│   │   └── PromotorWalletService.cs                   [CREAR - ver hexagonal-architecture.md]
│   ├── Context/
│   │   └── CrowdpromotionContext.cs                   [MODIFICAR - columnas Promotor]
│   ├── Migrations/
│   │   └── {timestamp}_AddPromotorEmailContactoUrlSitioWeb.cs  [GENERAR]
│   └── DependencyInjection.cs                         [MODIFICAR - registrar repos y services]
│
└── WePlayRises.Crowdpromotion.WebApi/
    └── Controllers/
        └── PromotorController.cs                      [CREAR]
```

---

## 11. Dependencias entre Capas (Usings por archivo)

### Commands (Application layer)
```
WePlayRises.Crowdpromotion.Domain.Constants    // ServiceResponseMessageType
WePlayRises.Crowdpromotion.Application.Dtos   // DTOs de response
WePlayRises.Crowdpromotion.Application.Interfaces.Services  // IPromotorService
WePlayRises.Crowdpromotion.Domain.Model        // Promotor, PromotorWallet
WePlayRises.BuildingBlocks.Kernel.Http.Response          // ServiceResponse<T>, ServiceResponseMessage
WePlayRises.BuildingBlocks.Kernel.Extensions             // ValidateExtensions, GetServiceResponseMessages
WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds  // PromotorId
AutoMapper
FluentValidation
MediatR
Microsoft.Extensions.Logging
```

### Validators (Application layer)
```
WePlayRises.Crowdpromotion.Domain.Constants    // ServiceResponseMessageType
WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands  // Command a validar
FluentValidation
```

### Profile (Application layer)
```
WePlayRises.Crowdpromotion.Application.Dtos
WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands
WePlayRises.Crowdpromotion.Domain.Model
WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds
AutoMapper
```

### Controller (WebApi layer)
```
WePlayRises.Crowdpromotion.Application.Dtos
WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands
WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries
WePlayRises.Crowdpromotion.Domain.Constants
WePlayRises.BuildingBlocks.Kernel.Base.Controllers   // BaseLoggerController
WePlayRises.BuildingBlocks.Kernel.Http.Response
WePlayRises.BuildingBlocks.Kernel.Services           // ICurrentUserService
MediatR
Microsoft.AspNetCore.Authorization
Microsoft.AspNetCore.Http
Microsoft.AspNetCore.Mvc
Microsoft.Extensions.Logging
```

---

## 12. Puntos de Atencion para Implementacion

### A. FanProfileId cross-modulo (CreatePromotorCommand paso 4)
La entidad `Promotor` tiene `FanProfileId?` como FK opcional al modulo UserAccess. El plan hexagonal sugiere que `IPromotorService.CreateWithWalletAsync` recibe la entidad ya construida con `FanProfileId = null` o con el valor resuelto. Para MVP, simplificar: **asignar `FanProfileId = null` directamente en el Handler** (flujo FA-02). Resolver el FanProfileId cross-modulo se puede agregar en una iteracion posterior.

### B. TipoPromotorNombre en PromotorCreatedResultDto
La entidad `Promotor` no tiene navegacion directa a `MaestraTipoPromotor`. Para MVP: en el Handler, despues de construir el DTO via AutoMapper, **asignar `dto.TipoPromotorNombre` usando el diccionario de constantes** con los valores seed:
```
private static readonly IReadOnlyDictionary<int, string> TipoPromotorNombres = new Dictionary<int, string>
{
    { 1, "Fan Embajador" },
    { 2, "Influencer" },
    { 3, "Medio / Blog" },
    { 4, "Profesional Marketing" }
};
```
Esta logica de negocio reside en el Handler, no en el Service ni en AutoMapper.

### C. ICurrentUserService.UserId tipo
En `CampaniasController` se usa `_currentUser.UserId` que retorna `Guid?`. Sin embargo, `Promotor.UserId` es `string`. El controller debe convertir: `userId = _currentUser.UserId?.ToString()`. Verificar el tipo real de `ICurrentUserService` en el BuildingBlock y ajustar en consecuencia.

### D. Orden de reglas en Validators para campos opcionales
El patron del proyecto (`CreateCampaniaCommandValidator`) aplica `.When(x => !string.IsNullOrEmpty(x.Campo))` como ultimo encadenamiento de cada `RuleFor`. En FluentValidation, `.When()` aplica al ultimo `RuleFor`, no solo a la ultima regla. Para aplicar `.When()` a una regla individual (por ejemplo, solo a `MaximumLength` pero no a `Must`), usar la sobrecarga `Must(...).When(..., ApplyConditionTo.CurrentValidator)`.

### E. ServiceResponse sin Messages para GET exitoso
Segun el contrato `GET /api/crowdpromotion/promotor/me`, el response exitoso tiene `"messages": []`. El Handler retorna `new ServiceResponse<PromotorDto> { Data = dto }` sin agregar mensajes. El `BaseLoggerController.FromServiceResponse()` maneja este caso correctamente (sin mensajes = 200 OK).

### F. Codigo HTTP para BusinessRule (4xxx = 409 Conflict)
Segun la logica de `BaseLoggerController.GetEffectiveHttpStatusCode()`, los `ErrorCode` con prefijo `4` se mapean a `HttpStatusCode.Conflict` (409). El contrato define estos errores como HTTP 400, pero el framework del proyecto los procesa como 409. **Esta discrepancia ya existe en el proyecto y se mantiene consistente con el patron existente.** El frontend debe manejar tanto 400 como 409 para errores de regla de negocio.

---

## 13. Checklist

- [ ] `ServiceResponseMessageType.cs` creado en `Domain/Constants/` con codigos 2015, 4018, 4019
- [ ] `CreatePromotorCommand.cs` contiene Command + Handler en mismo archivo
- [ ] `UpdatePromotorCommand.cs` contiene Command + Handler en mismo archivo
- [ ] `DesactivarPromotorCommand.cs` contiene Command + Handler en mismo archivo
- [ ] `GetPromotorMeQuery.cs` contiene Query + Handler en mismo archivo
- [ ] Todos los Commands/Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Todos los Handlers inyectan `IPromotorService` (no `CrowdpromotionContext`)
- [ ] **Constructores de Handlers con `?? throw new ArgumentNullException` para TODAS las dependencias**
- [ ] **Constructores de Validators con `?? throw` si tienen dependencias inyectadas**
- [ ] `CreatePromotorCommandValidator` en carpeta `Validators/` separada
- [ ] `UpdatePromotorCommandValidator` en carpeta `Validators/` separada
- [ ] **Validators usan `ServiceResponseMessageType.X` (NO strings literales)**
- [ ] **Handlers usan `ServiceResponseMessageType.X` en ServiceResponseMessage.ErrorCode**
- [ ] Try-catch con `_logger.LogError` en todos los Handlers
- [ ] Logica de negocio en Handlers (FechaActualizacion, EsActivo, TipoPromotorNombres dict)
- [ ] Services retornan entidades, no DTOs (el Handler hace el mapping con `_mapper`)
- [ ] `PromotorProfile.cs` creado en `Application/Mapping/`
- [ ] Campos calculados en `PromotorDto` asignados manualmente en Handler (no por AutoMapper)
- [ ] `DesactivarPromotorCommand` Handler: NO inyecta IMapper (construye DTO manualmente)
- [ ] `PromotorController` hereda de `BaseLoggerController`
- [ ] `PromotorController` inyecta `ICurrentUserService` para obtener UserId del JWT
- [ ] `PromotorController` usa `FromServiceResponse(result)` (delegado al base controller)
- [ ] `POST /api/crowdpromotion/promotor` retorna HTTP 201 si `result.IsSuccess`
- [ ] DTOs en carpeta `Application/Dtos/`: 2 request + 4 response = 6 archivos
- [ ] `DependencyInjection.cs` registra 2 repositorios + 2 servicios
- [ ] Migracion `AddPromotorEmailContactoUrlSitioWeb` generada y aplicada
