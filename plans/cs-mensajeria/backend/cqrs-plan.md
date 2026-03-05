# Plan CQRS: cs-mensajeria

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)

---

## Prerrequisitos criticos antes de implementar CQRS

Antes de crear los archivos CQRS, los siguientes cambios en Domain e Infrastructure deben estar completos (ver `hexagonal-architecture.md`):

1. `ConversacionCrowdsourcing.cs` renombrado: `UserIdArtista` -> `UserIdCreador`, `UserIdProveedor` -> `UserIdDestinatario`, `Asunto` de `string?` a `string = null!`
2. `ServiceResponseMessageType.cs` con 3 nuevas constantes: `NotFound_Conversacion = "2014"`, `BusinessRule_ConversacionDuplicada = "4015"`, `BusinessRule_NoRelacionConDestinatario = "4016"`
3. `IConversacionCrowdsourcingService` creada con 6 metodos
4. `IMensajeCrowdsourcingService` creada con 3 metodos
5. Implementaciones `ConversacionCrowdsourcingService` y `MensajeCrowdsourcingService` creadas
6. `DependencyInjection.cs` actualizado con los 3 registros nuevos

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Command/Query | Response |
|-----------|------|---------------|----------|
| Crear conversacion | Command | `CreateConversacionCommand` | `ServiceResponse<CreateConversacionResultDto>` |
| Enviar mensaje | Command | `SendMensajeCommand` | `ServiceResponse<MensajeDto>` |
| Marcar mensajes como leidos | Command | `MarcarLeidosCommand` | `ServiceResponse<MarcarLeidosResponseDto>` |
| Listar conversaciones | Query | `GetConversacionesQuery` | `ServiceResponse<ConversacionListResponseDto>` |
| Obtener mensajes de conversacion | Query | `GetMensajesQuery` | `ServiceResponse<MensajeListResponseDto>` |
| Conteo total no leidos (badge) | Query | `GetNoLeidosCountQuery` | `ServiceResponse<NoLeidosCountResponseDto>` |

---

## 2. Nuevas Constantes en ServiceResponseMessageType

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

Agregar despues de `NotFound_Entregable = "2013"`:
```csharp
public const string NotFound_Conversacion = "2014";
```

Agregar despues de `BusinessRule_InvalidState = "4014"`:
```csharp
public const string BusinessRule_ConversacionDuplicada = "4015";
public const string BusinessRule_NoRelacionConDestinatario = "4016";
```

---

## 3. Commands

### 3.1 CreateConversacionCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/CreateConversacionCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo.

#### Command

```
public class CreateConversacionCommand : IRequest<ServiceResponse<CreateConversacionResultDto>>
```

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `NecesidadId` | `Guid?` | FK a NecesidadCrowdsourcing. Mutuamente excluyente con AcuerdoId. Viene del body. |
| `AcuerdoId` | `Guid?` | FK a AcuerdoCrowdsourcing. Mutuamente excluyente con NecesidadId. Viene del body. |
| `UserIdDestinatario` | `string` | UserId Identity del destinatario. Viene del body. Inicializar con `= null!`. |
| `Asunto` | `string` | Asunto de la conversacion. Min 1, max 200 chars. Viene del body. Inicializar con `= null!`. |
| `UserIdCreador` | `string` | UserId Identity del creador. Asignado por el controller desde el JWT, NO del body. Inicializar con `= null!`. |

**Nota de binding:** El controller expone al cliente solo `NecesidadId`, `AcuerdoId`, `UserIdDestinatario` y `Asunto`. El `UserIdCreador` se asigna en el controller antes de enviar el command via MediatR:
```csharp
command.UserIdCreador = GetUserId();
```

#### Handler

**Clase:** `CreateConversacionCommandHandler : IRequestHandler<CreateConversacionCommand, ServiceResponse<CreateConversacionResultDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IConversacionCrowdsourcingService _conversacionService`
- `IArtistaService _artistaService` (de `WePlayRises.UserAccess.Application.Interfaces.Services`)
- `IPerfilProfesionalService _perfilService` (de `WePlayRises.UserAccess.Application.Interfaces.Services`)
- `IValidator<CreateConversacionCommand> _validator`
- `ILogger<CreateConversacionCommandHandler> _logger`

**Nota sobre IMapper:** Este handler NO usa AutoMapper para el resultado. El `CreateConversacionResultDto` se construye manualmente en el handler porque varios campos calculados (`NombreDestinatario`, `ContextoTipo`, `ContextoTitulo`) no provienen directamente del mapping. El `ConversacionCrowdsourcing` entity si puede mapearse desde el command para la persistencia, pero dado que los campos de identidad se asignan manualmente, la entidad tambien se construye manualmente en este handler.

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar request con FluentValidation
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Verificar relacion previa entre los usuarios en el contexto indicado
    //    (propuesta enviada o acuerdo activo vinculando a UserIdCreador y UserIdDestinatario)
    var tieneRelacion = await _conversacionService.TieneRelacionAsync(
        request.UserIdCreador, request.UserIdDestinatario,
        request.NecesidadId, request.AcuerdoId, ct);
    if (!tieneRelacion)
        return ServiceResponse con BusinessRule_NoRelacionConDestinatario ("4016")
        // El controller mapeara a HTTP 403

    // 3. Verificar unicidad: no existe ya conversacion para este contexto entre estas partes
    var existe = await _conversacionService.ExisteConversacionParaContextoAsync(
        request.UserIdCreador, request.UserIdDestinatario,
        request.NecesidadId, request.AcuerdoId, ct);
    if (existe)
        return ServiceResponse con BusinessRule_ConversacionDuplicada ("4015")
        // El controller mapeara a HTTP 400

    // 4. Construir entidad (sin AutoMapper por los campos de identidad)
    var entity = new ConversacionCrowdsourcing
    {
        Id = Guid.NewGuid(),
        UserIdCreador = request.UserIdCreador,
        UserIdDestinatario = request.UserIdDestinatario,
        Asunto = request.Asunto,
        FechaCreacion = DateTime.UtcNow,
        FechaUltimoMensaje = null
    };
    // Asignar contexto con StronglyTypedIds si aplica
    if (request.NecesidadId.HasValue)
        entity.NecesidadId = new NecesidadCrowdsourcingId(request.NecesidadId.Value);
    if (request.AcuerdoId.HasValue)
        entity.AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId.Value);

    // 5. Persistir via service (hace SaveChangesAsync internamente)
    var id = await _conversacionService.CreateAsync(entity, ct);

    // 6. Resolver NombreDestinatario
    //    Intentar como artista primero; si null, como perfil profesional
    string nombreDestinatario;
    var artista = await _artistaService.GetByUserIdAsync(request.UserIdDestinatario, ct);
    if (artista != null)
        nombreDestinatario = artista.NombreArtistico;
    else
    {
        var perfil = await _perfilService.GetByUserIdAsync(request.UserIdDestinatario, ct);
        nombreDestinatario = perfil?.NombreCompleto ?? string.Empty;
    }

    // 7. Resolver ContextoTipo y ContextoTitulo
    string contextoTipo;
    string contextoTitulo;
    if (request.NecesidadId.HasValue)
    {
        contextoTipo = "necesidad";
        contextoTitulo = entity.Necesidad?.Titulo ?? string.Empty;
        // Si Necesidad no cargada por navigation, hacer query directa via service
    }
    else
    {
        contextoTipo = "acuerdo";
        contextoTitulo = entity.Acuerdo?.Descripcion ?? string.Empty;
        // Si Acuerdo no cargado por navigation, hacer query directa via service
    }

    // 8. Construir resultado y retornar ServiceResponse exitoso
    var resultDto = new CreateConversacionResultDto
    {
        Id = id,
        Asunto = entity.Asunto,
        NombreDestinatario = nombreDestinatario,
        ContextoTipo = contextoTipo,
        ContextoTitulo = contextoTitulo,
        FechaCreacion = entity.FechaCreacion
    };

    return new ServiceResponse<CreateConversacionResultDto>
    {
        Data = resultDto,
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "Conversacion creada correctamente",
                ErrorCode = ServiceResponseMessageType.Created
            }
        }
    };
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
    when (dbEx.InnerException?.Message.Contains("UX_ConversacionCrowdsourcing") == true)
{
    // Segundo mecanismo de defensa: constraint de unicidad SQL violado por race condition
    _logger.LogWarning(dbEx, "Constraint unicidad violado al crear conversacion (race condition)");
    return new ServiceResponse<CreateConversacionResultDto>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "Ya existe una conversacion para este contexto entre las mismas partes",
                ErrorCode = ServiceResponseMessageType.BusinessRule_ConversacionDuplicada
            }
        }
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error creating conversacion. UserIdCreador={UserIdCreador}, UserIdDestinatario={UserIdDestinatario}",
        request.UserIdCreador, request.UserIdDestinatario);
    return ValidateExtensions.InternalServerErrorServiceResponse<CreateConversacionResultDto>(
        "Error inesperado al crear la conversacion",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Imports necesarios:**
```csharp
using AutoMapper;  // Omitir si no se usa AutoMapper en este handler
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
```

---

### 3.2 SendMensajeCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/SendMensajeCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo.

#### Command

```
public class SendMensajeCommand : IRequest<ServiceResponse<MensajeDto>>
```

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `ConversacionId` | `Guid` | ID de la conversacion. Asignado por el controller desde el path param. |
| `UserIdRemitente` | `string` | UserId Identity del remitente. Asignado por el controller desde el JWT. Inicializar con `= null!`. |
| `Contenido` | `string` | Texto del mensaje. Min 1, max 5000 chars. Viene del body. Inicializar con `= null!`. |
| `UrlAdjunto` | `string?` | URL de adjunto externo. Opcional. Si presente, debe ser URL valida. Viene del body. |

**Nota de binding:** El controller extrae del body solo `Contenido` y `UrlAdjunto`. Asigna `ConversacionId` desde `[FromRoute]` y `UserIdRemitente` desde el JWT antes de enviar via MediatR.

#### Handler

**Clase:** `SendMensajeCommandHandler : IRequestHandler<SendMensajeCommand, ServiceResponse<MensajeDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IConversacionCrowdsourcingService _conversacionService`
- `IMensajeCrowdsourcingService _mensajeService`
- `IArtistaService _artistaService`
- `IPerfilProfesionalService _perfilService`
- `IValidator<SendMensajeCommand> _validator`
- `ILogger<SendMensajeCommandHandler> _logger`

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar campos con FluentValidation (formato, longitud, URL valida)
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Verificar que la conversacion existe
    var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
    if (conversacion == null)
        return ServiceResponse con NotFound_Conversacion ("2014")
        // El controller mapeara a HTTP 404

    // 3. Verificar que el remitente es participante de la conversacion
    bool esParticipante = conversacion.UserIdCreador == request.UserIdRemitente
                          || conversacion.UserIdDestinatario == request.UserIdRemitente;
    if (!esParticipante)
        return ServiceResponse con Auth_Forbidden ("3002")
        // El controller mapeara a HTTP 403

    // 4. Construir entidad MensajeCrowdsourcing
    var entity = new MensajeCrowdsourcing
    {
        Id = Guid.NewGuid(),
        ConversacionId = request.ConversacionId,
        UserIdRemitente = request.UserIdRemitente,
        Contenido = request.Contenido,
        UrlAdjunto = request.UrlAdjunto,
        Leido = false,
        FechaLeido = null,
        FechaCreacion = DateTime.UtcNow
    };

    // 5. Persistir via service (CreateAsync tambien actualiza FechaUltimoMensaje de la conversacion)
    var mensajeCreado = await _mensajeService.SendAsync(entity, request.ConversacionId, ct);

    // 6. Resolver RemitenteNombre del usuario autenticado
    string remitenteNombre;
    var artista = await _artistaService.GetByUserIdAsync(request.UserIdRemitente, ct);
    if (artista != null)
        remitenteNombre = artista.NombreArtistico;
    else
    {
        var perfil = await _perfilService.GetByUserIdAsync(request.UserIdRemitente, ct);
        remitenteNombre = perfil?.NombreCompleto ?? string.Empty;
    }

    // 7. Construir MensajeDto
    //    EsPropio = true siempre (el remitente es el usuario actual que envio el mensaje)
    var dto = new MensajeDto
    {
        Id = mensajeCreado.Id,
        Contenido = mensajeCreado.Contenido,
        UrlAdjunto = mensajeCreado.UrlAdjunto,
        RemitenteNombre = remitenteNombre,
        EsPropio = true,
        Leido = false,
        FechaCreacion = mensajeCreado.FechaCreacion
    };

    // 8. Retornar ServiceResponse exitoso (HTTP 201)
    return new ServiceResponse<MensajeDto>
    {
        Data = dto,
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "Mensaje enviado correctamente",
                ErrorCode = ServiceResponseMessageType.Created
            }
        }
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error sending mensaje in conversacion {ConversacionId} by user {UserIdRemitente}",
        request.ConversacionId, request.UserIdRemitente);
    return ValidateExtensions.InternalServerErrorServiceResponse<MensajeDto>(
        "Error inesperado al enviar el mensaje",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Imports necesarios:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;
```

---

### 3.3 MarcarLeidosCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/MarcarLeidosCommand.cs`

**Contiene:** Command + Handler en el MISMO archivo.

#### Command

```
public class MarcarLeidosCommand : IRequest<ServiceResponse<MarcarLeidosResponseDto>>
```

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `ConversacionId` | `Guid` | ID de la conversacion. Asignado por el controller desde el path param. |
| `UserId` | `string` | UserId Identity del solicitante. Asignado por el controller desde el JWT. Inicializar con `= null!`. |

**Nota de binding:** Este command no tiene body. El controller construye el command completo con path param y JWT.

#### Handler

**Clase:** `MarcarLeidosCommandHandler : IRequestHandler<MarcarLeidosCommand, ServiceResponse<MarcarLeidosResponseDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IConversacionCrowdsourcingService _conversacionService`
- `IMensajeCrowdsourcingService _mensajeService`
- `IValidator<MarcarLeidosCommand> _validator`
- `ILogger<MarcarLeidosCommandHandler> _logger`

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar campos basicos (ConversacionId y UserId not empty)
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Verificar que la conversacion existe
    var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
    if (conversacion == null)
        return ServiceResponse con NotFound_Conversacion ("2014")
        // El controller mapeara a HTTP 404

    // 3. Verificar que el usuario es participante
    bool esParticipante = conversacion.UserIdCreador == request.UserId
                          || conversacion.UserIdDestinatario == request.UserId;
    if (!esParticipante)
        return ServiceResponse con Auth_Forbidden ("3002")
        // El controller mapeara a HTTP 403

    // 4. Ejecutar update batch: marcar como leidos los mensajes del otro participante
    //    (UserIdRemitente != request.UserId AND Leido == false)
    var mensajesMarcados = await _mensajeService.MarcarLeidosAsync(
        request.ConversacionId, request.UserId, ct);

    // 5. Retornar resultado (0 mensajes marcados NO es error)
    return new ServiceResponse<MarcarLeidosResponseDto>
    {
        Data = new MarcarLeidosResponseDto { MensajesMarcados = mensajesMarcados }
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error marking mensajes as read in conversacion {ConversacionId} by user {UserId}",
        request.ConversacionId, request.UserId);
    return ValidateExtensions.InternalServerErrorServiceResponse<MarcarLeidosResponseDto>(
        "Error inesperado al marcar mensajes como leidos",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Imports necesarios:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

## 4. Queries

### 4.1 GetConversacionesQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetConversacionesQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo.

#### Query

```
public class GetConversacionesQuery : IRequest<ServiceResponse<ConversacionListResponseDto>>
```

| Propiedad | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| `UserId` | `string` | - | Extraido del JWT en el controller. Inicializar con `= null!`. |
| `Contexto` | `string` | `"todas"` | Filtro por tipo: `"todas"`, `"necesidades"`, `"acuerdos"`. |
| `Page` | `int` | `1` | Numero de pagina (1-based). |
| `PageSize` | `int` | `20` | Elementos por pagina. Max 50. |

#### Handler

**Clase:** `GetConversacionesQueryHandler : IRequestHandler<GetConversacionesQuery, ServiceResponse<ConversacionListResponseDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IConversacionCrowdsourcingService _conversacionService`
- `IArtistaService _artistaService`
- `IPerfilProfesionalService _perfilService`
- `IValidator<GetConversacionesQuery> _validator`
- `ILogger<GetConversacionesQueryHandler> _logger`

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar parametros (UserId, Page, PageSize, Contexto)
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Obtener listado paginado con conteos desde service
    //    El service delega al repository que calcula en SQL:
    //    - MensajesNoLeidos por conversacion (COUNT filtrado por UserIdRemitente != UserId AND Leido == false)
    //    - TotalNoLeidos global (suma de todos los no leidos del user en TODAS sus conversaciones, sin paginar)
    //    - TotalCount de conversaciones (antes de paginar)
    //    - Preview del ultimo mensaje (truncado a 80 chars en SQL con LEFT(..., 80))
    //    - Ordena: FechaUltimoMensaje DESC NULLS LAST, luego FechaCreacion DESC
    var pageSize = Math.Min(request.PageSize, 50);
    var (items, totalCount, totalNoLeidos) = await _conversacionService.GetConversacionesByUserIdAsync(
        request.UserId, request.Contexto, request.Page, pageSize, ct);

    // 3. Para cada conversacion, resolver NombreOtraParte e ImagenOtraParte
    //    Usar un diccionario en memoria como cache manual para evitar N+1
    //    Si el mismo userId aparece en multiples conversaciones, se consulta solo una vez
    var perfilCache = new Dictionary<string, (string Nombre, string? Imagen)>(StringComparer.OrdinalIgnoreCase);

    var dtos = new List<ConversacionListItemDto>();
    foreach (var conv in items)
    {
        // Determinar el userId de la otra parte
        var userIdOtraParte = conv.UserIdCreador == request.UserId
            ? conv.UserIdDestinatario
            : conv.UserIdCreador;

        // Resolver perfil desde cache o consulta
        if (!perfilCache.TryGetValue(userIdOtraParte, out var perfilOtraParte))
        {
            string nombre;
            string? imagen = null;
            var artista = await _artistaService.GetByUserIdAsync(userIdOtraParte, ct);
            if (artista != null)
            {
                nombre = artista.NombreArtistico;
                imagen = artista.ImagenUrl; // usar el campo correcto del modelo Artista
            }
            else
            {
                var perfil = await _perfilService.GetByUserIdAsync(userIdOtraParte, ct);
                nombre = perfil?.NombreCompleto ?? string.Empty;
                imagen = null; // PerfilProfesional no tiene imagen en MVP
            }
            perfilOtraParte = (nombre, imagen);
            perfilCache[userIdOtraParte] = perfilOtraParte;
        }

        // Resolver ContextoTipo y ContextoTitulo
        string contextoTipo = conv.NecesidadId.HasValue ? "necesidad" : "acuerdo";
        string contextoTitulo = conv.NecesidadId.HasValue
            ? conv.Necesidad?.Titulo ?? string.Empty
            : conv.Acuerdo?.Descripcion ?? string.Empty;

        // El repository ya trunca UltimoMensaje a 80 chars en SQL
        // El campo MensajesNoLeidos ya viene calculado desde el repository
        // NOTA: La proyeccion con MensajesNoLeidos y UltimoMensaje requiere que el repository
        //       retorne un tipo que contenga estos campos calculados, no solo la entidad pura.
        //       Ver nota de implementacion en seccion 9.

        var dto = new ConversacionListItemDto
        {
            Id = conv.Id,
            Asunto = conv.Asunto,
            NombreOtraParte = perfilOtraParte.Nombre,
            ImagenOtraParte = perfilOtraParte.Imagen,
            ContextoTipo = contextoTipo,
            ContextoTitulo = contextoTitulo,
            UltimoMensaje = conv.UltimoMensajePreview,  // campo calculado en repository
            FechaUltimoMensaje = conv.FechaUltimoMensaje,
            MensajesNoLeidos = conv.MensajesNoLeidos  // campo calculado en repository
        };
        dtos.Add(dto);
    }

    // 4. Construir response paginada
    var response = new ConversacionListResponseDto
    {
        Items = dtos,
        TotalCount = totalCount,
        TotalNoLeidos = totalNoLeidos,
        Page = request.Page,
        PageSize = pageSize
    };

    return new ServiceResponse<ConversacionListResponseDto> { Data = response };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting conversaciones for user {UserId}", request.UserId);
    return ValidateExtensions.InternalServerErrorServiceResponse<ConversacionListResponseDto>(
        "Error inesperado al obtener las conversaciones",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Nota de implementacion critica:** La query de listado es compleja porque necesita datos calculados (`MensajesNoLeidos`, `UltimoMensajePreview`) que no forman parte del POCO `ConversacionCrowdsourcing`. Hay dos enfoques:

- **Enfoque A (recomendado):** El repository proyecta a un tipo anonimo o a un DTO especifico del repositorio que incluye los campos calculados. El service retorna `List<ConversacionConResumen>` (una clase simple en Application que extiende los datos de la entidad con los campos calculados).

- **Enfoque B (alternativa):** El repository carga las navigation properties `Necesidad` y `Acuerdo`, y el handler hace el calculo de `MensajesNoLeidos` en memoria. Esto genera N+1 queries y NO se recomienda.

**Decision:** Usar Enfoque A. Ver seccion 8 para el DTO intermedio `ConversacionResumenItem`.

**Imports necesarios:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;
```

---

### 4.2 GetMensajesQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetMensajesQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo.

#### Query

```
public class GetMensajesQuery : IRequest<ServiceResponse<MensajeListResponseDto>>
```

| Propiedad | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| `ConversacionId` | `Guid` | - | ID de la conversacion. Asignado por el controller desde el path param. |
| `UserId` | `string` | - | Extraido del JWT en el controller. Usado para validar participacion y calcular EsPropio. Inicializar con `= null!`. |
| `Page` | `int` | `1` | Numero de pagina (1-based). |
| `PageSize` | `int` | `50` | Elementos por pagina. Max 100. |

#### Handler

**Clase:** `GetMensajesQueryHandler : IRequestHandler<GetMensajesQuery, ServiceResponse<MensajeListResponseDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IConversacionCrowdsourcingService _conversacionService`
- `IMensajeCrowdsourcingService _mensajeService`
- `IArtistaService _artistaService`
- `IPerfilProfesionalService _perfilService`
- `IValidator<GetMensajesQuery> _validator`
- `ILogger<GetMensajesQueryHandler> _logger`

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar parametros basicos
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Verificar que la conversacion existe
    var conversacion = await _conversacionService.GetByIdAsync(request.ConversacionId, ct);
    if (conversacion == null)
        return ServiceResponse con NotFound_Conversacion ("2014")
        // El controller mapeara a HTTP 404

    // 3. Verificar que el usuario es participante
    bool esParticipante = conversacion.UserIdCreador == request.UserId
                          || conversacion.UserIdDestinatario == request.UserId;
    if (!esParticipante)
        return ServiceResponse con Auth_Forbidden ("3002")
        // El controller mapeara a HTTP 403

    // 4. Obtener mensajes paginados (orden FechaCreacion ASC - mas antiguos primero)
    var pageSize = Math.Min(request.PageSize, 100);
    var (mensajes, totalCount) = await _mensajeService.GetByConversacionIdPaginatedAsync(
        request.ConversacionId, request.Page, pageSize, ct);

    // 5. Resolver nombres de remitentes con cache manual (evitar N+1)
    var nombreCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    var dtos = new List<MensajeDto>();
    foreach (var mensaje in mensajes)
    {
        if (!nombreCache.TryGetValue(mensaje.UserIdRemitente, out var remitenteNombre))
        {
            var artista = await _artistaService.GetByUserIdAsync(mensaje.UserIdRemitente, ct);
            if (artista != null)
                remitenteNombre = artista.NombreArtistico;
            else
            {
                var perfil = await _perfilService.GetByUserIdAsync(mensaje.UserIdRemitente, ct);
                remitenteNombre = perfil?.NombreCompleto ?? string.Empty;
            }
            nombreCache[mensaje.UserIdRemitente] = remitenteNombre;
        }

        dtos.Add(new MensajeDto
        {
            Id = mensaje.Id,
            Contenido = mensaje.Contenido,
            UrlAdjunto = mensaje.UrlAdjunto,
            RemitenteNombre = remitenteNombre,
            EsPropio = mensaje.UserIdRemitente == request.UserId,  // calcular en handler
            Leido = mensaje.Leido,
            FechaCreacion = mensaje.FechaCreacion
        });
    }

    // 6. Retornar response paginada
    return new ServiceResponse<MensajeListResponseDto>
    {
        Data = new MensajeListResponseDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = pageSize
        }
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting mensajes for conversacion {ConversacionId} by user {UserId}",
        request.ConversacionId, request.UserId);
    return ValidateExtensions.InternalServerErrorServiceResponse<MensajeListResponseDto>(
        "Error inesperado al obtener los mensajes",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Imports necesarios:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;
```

---

### 4.3 GetNoLeidosCountQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetNoLeidosCountQuery.cs`

**Contiene:** Query + Handler en el MISMO archivo.

#### Query

```
public class GetNoLeidosCountQuery : IRequest<ServiceResponse<NoLeidosCountResponseDto>>
```

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `UserId` | `string` | Extraido del JWT en el controller. Inicializar con `= null!`. |

#### Handler

**Clase:** `GetNoLeidosCountQueryHandler : IRequestHandler<GetNoLeidosCountQuery, ServiceResponse<NoLeidosCountResponseDto>>`

**Dependencias (todas con `?? throw new ArgumentNullException`):**
- `IMensajeCrowdsourcingService _mensajeService`
- `IValidator<GetNoLeidosCountQuery> _validator`
- `ILogger<GetNoLeidosCountQueryHandler> _logger`

**Flujo del metodo `Handle`:**

```
try
{
    // 1. Validar UserId not empty
    var validationResult = await _validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return ServiceResponse con validationResult.GetServiceResponseMessages()

    // 2. Obtener total de mensajes no leidos del usuario en TODAS sus conversaciones
    //    Query SQL directa via service/repository (< 100ms objetivo)
    //    COUNT de mensajes donde (conversacion.UserIdCreador == userId OR conversacion.UserIdDestinatario == userId)
    //    AND mensaje.UserIdRemitente != userId AND mensaje.Leido == false
    var totalNoLeidos = await _mensajeService.GetTotalNoLeidosAsync(request.UserId, ct);

    // 3. Retornar resultado
    return new ServiceResponse<NoLeidosCountResponseDto>
    {
        Data = new NoLeidosCountResponseDto { TotalNoLeidos = totalNoLeidos }
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error getting no leidos count for user {UserId}", request.UserId);
    return ValidateExtensions.InternalServerErrorServiceResponse<NoLeidosCountResponseDto>(
        "Error inesperado al obtener el conteo de no leidos",
        ServiceResponseMessageType.Internal_UnexpectedError);
}
```

**Imports necesarios:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

## 5. Validators

### 5.1 CreateConversacionCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/CreateConversacionCommandValidator.cs`

**Clase:** `CreateConversacionCommandValidator : AbstractValidator<CreateConversacionCommand>`

**Dependencias (en constructor con `?? throw new ArgumentNullException`):**
- Ninguna dependencia de servicio. Este validator solo hace validaciones de formato/campo sincrono.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `UserIdCreador` | `.NotEmpty()` | "Error de autenticacion: usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |
| `UserIdDestinatario` | `.NotEmpty()` | "El destinatario es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Asunto` | `.NotEmpty()` | "El asunto de la conversacion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Asunto` | `.MaximumLength(200)` | "El asunto no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `NecesidadId + AcuerdoId` | Regla custom nivel objeto: exactamente uno de los dos debe tener valor | "Debe especificar exactamente un contexto: una necesidad o un acuerdo, pero no ambos ni ninguno" | `ServiceResponseMessageType.Validation_Required` |

**Regla custom de exclusividad mutua:**
```csharp
RuleFor(x => x)
    .Must(x => (x.NecesidadId.HasValue && !x.AcuerdoId.HasValue)
               || (!x.NecesidadId.HasValue && x.AcuerdoId.HasValue))
    .WithMessage("Debe especificar exactamente un contexto: una necesidad o un acuerdo, pero no ambos ni ninguno")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);
```

**Validaciones de negocio que NO van en el Validator sino en el Handler:**
- Verificar relacion previa entre los usuarios (`TieneRelacionAsync`) -> retorna `BusinessRule_NoRelacionConDestinatario`
- Verificar unicidad de la conversacion (`ExisteConversacionParaContextoAsync`) -> retorna `BusinessRule_ConversacionDuplicada`
- Captura de `DbUpdateException` por constraint de unicidad SQL -> retorna `BusinessRule_ConversacionDuplicada`

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

### 5.2 SendMensajeCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/SendMensajeCommandValidator.cs`

**Clase:** `SendMensajeCommandValidator : AbstractValidator<SendMensajeCommand>`

**Dependencias:** Ninguna dependencia de servicio. Solo validaciones de formato sincrono.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `UserIdRemitente` | `.NotEmpty()` | "Usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |
| `Contenido` | `.NotEmpty()` | "El contenido del mensaje es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Contenido` | `.MaximumLength(5000)` | "El mensaje no puede superar los 5000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `UrlAdjunto` | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlAdjunto))` | "La URL adjunta no es valida. Debe ser una URL completa (ej: https://...)" | `ServiceResponseMessageType.Validation_InvalidUrl` |
| `UrlAdjunto` | `.MaximumLength(2048).When(x => !string.IsNullOrEmpty(x.UrlAdjunto))` | "La URL no puede superar los 2048 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

**Metodo helper privado `BeAValidUrl`:**
```csharp
private static bool BeAValidUrl(string? url)
{
    return Uri.TryCreate(url, UriKind.Absolute, out var result)
           && (result.Scheme == Uri.UriSchemeHttps || result.Scheme == Uri.UriSchemeHttp);
}
```

**Validaciones de negocio que NO van en el Validator sino en el Handler:**
- Verificar que la conversacion existe (`GetByIdAsync`) -> retorna `NotFound_Conversacion`
- Verificar que el remitente es participante -> retorna `Auth_Forbidden`

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

### 5.3 MarcarLeidosCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/MarcarLeidosCommandValidator.cs`

**Clase:** `MarcarLeidosCommandValidator : AbstractValidator<MarcarLeidosCommand>`

**Dependencias:** Ninguna dependencia de servicio.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |

**Validaciones de negocio que van en el Handler:**
- Verificar que la conversacion existe -> retorna `NotFound_Conversacion`
- Verificar que el usuario es participante -> retorna `Auth_Forbidden`

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

### 5.4 GetConversacionesQueryValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetConversacionesQueryValidator.cs`

**Clase:** `GetConversacionesQueryValidator : AbstractValidator<GetConversacionesQuery>`

**Dependencias:** Ninguna dependencia de servicio.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |
| `Page` | `.GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `ServiceResponseMessageType.Validation_InvalidRange` |
| `PageSize` | `.InclusiveBetween(1, 50)` | "El tamano de pagina debe estar entre 1 y 50" | `ServiceResponseMessageType.Validation_InvalidRange` |
| `Contexto` | `.Must(v => v == "todas" \|\| v == "necesidades" \|\| v == "acuerdos")` (with `.When`) | "El valor de contexto no es valido. Use: todas, necesidades, acuerdos" | `ServiceResponseMessageType.Validation_Required` |

**Nota sobre Contexto:** La regla de `Contexto` solo aplica cuando `Contexto` no es null ni empty. En la query, `Contexto` tiene default `"todas"`, por lo que siempre tendra valor. La regla previene valores arbitrarios enviados por el cliente:
```csharp
RuleFor(x => x.Contexto)
    .Must(v => v == "todas" || v == "necesidades" || v == "acuerdos")
    .When(x => !string.IsNullOrEmpty(x.Contexto))
    .WithMessage("El valor de contexto no es valido. Use: todas, necesidades, acuerdos")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);
```

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

### 5.5 GetMensajesQueryValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetMensajesQueryValidator.cs`

**Clase:** `GetMensajesQueryValidator : AbstractValidator<GetMensajesQuery>`

**Dependencias:** Ninguna dependencia de servicio.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |
| `Page` | `.GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `ServiceResponseMessageType.Validation_InvalidRange` |
| `PageSize` | `.InclusiveBetween(1, 100)` | "El tamano de pagina debe estar entre 1 y 100" | `ServiceResponseMessageType.Validation_InvalidRange` |

**Validaciones de negocio que van en el Handler (no en este Validator):**
- Verificar que la conversacion existe -> retorna `NotFound_Conversacion`
- Verificar que el userId es participante -> retorna `Auth_Forbidden`

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

### 5.6 GetNoLeidosCountQueryValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetNoLeidosCountQueryValidator.cs`

**Clase:** `GetNoLeidosCountQueryValidator : AbstractValidator<GetNoLeidosCountQuery>`

**Dependencias:** Ninguna dependencia de servicio.

**Reglas de validacion:**

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `ServiceResponseMessageType.Validation_Required` |

**Imports:**
```csharp
using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

## 6. DTOs de Aplicacion

Todos los DTOs se crean en `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/`.

### 6.1 CreateConversacionResultDto

**Archivo:** `Dtos/CreateConversacionResultDto.cs`

```
namespace WePlayRises.Crowdsourcing.Application.Dtos;

public class CreateConversacionResultDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreDestinatario { get; set; } = null!;
    /// "necesidad" o "acuerdo"
    public string ContextoTipo { get; set; } = null!;
    /// Titulo de la necesidad o del acuerdo vinculado
    public string ContextoTitulo { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

### 6.2 ConversacionListResponseDto

**Archivo:** `Dtos/ConversacionListResponseDto.cs`

```
public class ConversacionListResponseDto
{
    public List<ConversacionListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    /// Suma de mensajes no leidos en TODAS las conversaciones del usuario (no paginado)
    public int TotalNoLeidos { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

### 6.3 ConversacionListItemDto

**Archivo:** `Dtos/ConversacionListItemDto.cs`

```
public class ConversacionListItemDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreOtraParte { get; set; } = null!;
    /// URL de imagen de perfil de la otra parte. Null si no tiene foto de perfil.
    public string? ImagenOtraParte { get; set; }
    /// "necesidad" o "acuerdo"
    public string ContextoTipo { get; set; } = null!;
    public string ContextoTitulo { get; set; } = null!;
    /// Preview del ultimo mensaje truncado a 80 chars. Null si la conversacion no tiene mensajes.
    public string? UltimoMensaje { get; set; }
    /// Null si la conversacion no tiene mensajes aun.
    public DateTime? FechaUltimoMensaje { get; set; }
    /// COUNT de mensajes no leidos del otro participante en esta conversacion.
    public int MensajesNoLeidos { get; set; }
}
```

### 6.4 MensajeDto

**Archivo:** `Dtos/MensajeDto.cs`

```
public class MensajeDto
{
    public Guid Id { get; set; }
    public string Contenido { get; set; } = null!;
    /// URL de adjunto externo. Null si no tiene adjunto.
    public string? UrlAdjunto { get; set; }
    public string RemitenteNombre { get; set; } = null!;
    /// true si UserIdRemitente == UserId del usuario autenticado. Calculado en el Handler.
    public bool EsPropio { get; set; }
    public bool Leido { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

### 6.5 MensajeListResponseDto

**Archivo:** `Dtos/MensajeListResponseDto.cs`

```
public class MensajeListResponseDto
{
    /// Ordenados por FechaCreacion ASC (mas antiguos primero).
    public List<MensajeDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

### 6.6 MarcarLeidosResponseDto

**Archivo:** `Dtos/MarcarLeidosResponseDto.cs`

```
public class MarcarLeidosResponseDto
{
    /// Numero de mensajes que pasaron de Leido=false a Leido=true. 0 si no habia mensajes no leidos.
    public int MensajesMarcados { get; set; }
}
```

### 6.7 NoLeidosCountResponseDto

**Archivo:** `Dtos/NoLeidosCountResponseDto.cs`

```
public class NoLeidosCountResponseDto
{
    /// Total de mensajes no leidos del usuario en todas sus conversaciones.
    public int TotalNoLeidos { get; set; }
}
```

### 6.8 ConversacionResumenItem (DTO intermedio de repositorio)

**Archivo:** `Dtos/ConversacionResumenItem.cs`

Este DTO no se expone al cliente. Es un tipo de proyeccion usado internamente entre el repositorio y el handler del listado, para transportar los campos calculados que no forman parte del POCO `ConversacionCrowdsourcing`.

```
public class ConversacionResumenItem
{
    public Guid Id { get; set; }
    public string UserIdCreador { get; set; } = null!;
    public string UserIdDestinatario { get; set; } = null!;
    public string Asunto { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaUltimoMensaje { get; set; }
    /// FK nullable del contexto
    public Guid? NecesidadId { get; set; }
    public Guid? AcuerdoId { get; set; }
    /// Titulo de la necesidad o acuerdo vinculado. Calculado via JOIN en el repositorio.
    public string ContextoTitulo { get; set; } = string.Empty;
    /// Preview truncado a 80 chars. Null si no hay mensajes. Calculado via subquery en el repositorio.
    public string? UltimoMensajePreview { get; set; }
    /// COUNT de mensajes no leidos del otro participante. Calculado via subquery en el repositorio.
    public int MensajesNoLeidos { get; set; }
}
```

**Uso:** `IConversacionCrowdsourcingService.GetConversacionesByUserIdAsync` retorna `Task<(IReadOnlyList<ConversacionResumenItem> Items, int TotalCount, int TotalNoLeidos)>` en lugar de la entidad pura.

---

## 7. AutoMapper Profiles

### 7.1 ConversacionCrowdsourcingProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/ConversacionCrowdsourcingProfile.cs`

**Clase:** `ConversacionCrowdsourcingProfile : Profile`

**Namespace:** `WePlayRises.Crowdsourcing.Application.Mapping`

**Mappings requeridos:**

```csharp
public ConversacionCrowdsourcingProfile()
{
    // Command -> Entity: Se usa en CreateConversacionCommandHandler
    // NOTA: La mayoria de campos se asignan manualmente en el Handler.
    //       Este mapping puede ser minimo o incluso omitido si el handler construye
    //       la entidad completamente a mano. Si se usa AutoMapper, ignorar todos los
    //       campos que el handler asigna manualmente.
    CreateMap<CreateConversacionCommand, ConversacionCrowdsourcing>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.UserIdCreador, opt => opt.Ignore())        // asignado desde request.UserIdCreador
        .ForMember(dest => dest.UserIdDestinatario, opt => opt.Ignore())   // asignado desde request.UserIdDestinatario
        .ForMember(dest => dest.NecesidadId, opt => opt.Ignore())          // requiere StronglyTypedId conversion
        .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())            // requiere StronglyTypedId conversion
        .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
        .ForMember(dest => dest.FechaUltimoMensaje, opt => opt.Ignore())
        .ForMember(dest => dest.Necesidad, opt => opt.Ignore())
        .ForMember(dest => dest.Acuerdo, opt => opt.Ignore())
        .ForMember(dest => dest.Mensajes, opt => opt.Ignore());
        // Solo Asunto se mapea directamente desde el Command
}
```

**Nota de implementacion:** Dado que el `CreateConversacionCommandHandler` construye la entidad completamente a mano (por los StronglyTypedIds y la logica de identidad), el mapping de Command -> Entity puede ser innecesario. Decidir durante la implementacion si usar AutoMapper o construccion manual. Usar construction manual si hay dudas para mayor claridad.

### 7.2 MensajeCrowdsourcingProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/MensajeCrowdsourcingProfile.cs`

**Clase:** `MensajeCrowdsourcingProfile : Profile`

**Namespace:** `WePlayRises.Crowdsourcing.Application.Mapping`

**Mappings requeridos:**

```csharp
public MensajeCrowdsourcingProfile()
{
    // Entity -> MensajeDto: Los campos calculados (RemitenteNombre, EsPropio) se ignoran.
    //                       El handler los asigna manualmente despues del mapping.
    //                       NOTA: Si el handler construye MensajeDto manualmente, este mapping
    //                       tambien puede omitirse. Preferir construccion manual en el handler
    //                       ya que EsPropio requiere el UserId del request (no disponible en el mapping).
    CreateMap<MensajeCrowdsourcing, MensajeDto>()
        .ForMember(dest => dest.RemitenteNombre, opt => opt.Ignore())   // resuelto en handler
        .ForMember(dest => dest.EsPropio, opt => opt.Ignore());         // calculado en handler
}
```

**Nota de implementacion:** Al igual que el handler de Conversacion, los handlers de SendMensaje y GetMensajes construyen el `MensajeDto` manualmente porque `EsPropio` requiere el `UserId` del request. El mapping de AutoMapper puede usarse como punto de partida y luego el handler sobreescribe los campos calculados:
```csharp
var dto = _mapper.Map<MensajeDto>(mensaje);
dto.RemitenteNombre = remitenteNombre;  // sobreescribir
dto.EsPropio = mensaje.UserIdRemitente == request.UserId;  // sobreescribir
```

---

## 8. Controller: ConversacionesCrowdsourcingController

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/ConversacionesCrowdsourcingController.cs`

**Namespace:** `WePlayRises.Crowdsourcing.WebApi.Controllers`

**Atributos de clase:**
```csharp
[ApiController]
[Route("api/crowdsourcing/conversaciones")]
[Authorize]
```

**Constructor:**
```csharp
public ConversacionesCrowdsourcingController(IMediator mediator)
{
    _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
}
```

**Metodo helper privado (patron del proyecto):**
```csharp
private string? GetUserId()
{
    return User.FindFirstValue(ClaimTypes.NameIdentifier);
}
```

### Actions del Controller

#### POST "" - CreateConversacion

```csharp
[HttpPost("")]
[ProducesResponseType(typeof(ServiceResponse<CreateConversacionResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<CreateConversacionResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> CreateConversacion([FromBody] CreateConversacionCommand command)
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Asignar `command.UserIdCreador = userId`
3. `var response = await _mediator.Send(command)`
4. Si `response.HasErrors`:
   - Si contiene `NotFound_Conversacion` -> `return NotFound(response)`
   - Si contiene `Auth_Forbidden` o `BusinessRule_NoRelacionConDestinatario` -> `return StatusCode(403, response)`
   - Default -> `return BadRequest(response)`
5. Si exito -> `return StatusCode(StatusCodes.Status201Created, response)`

#### GET "" - GetConversaciones

**ORDEN CRITICO:** Este action debe registrarse ANTES de los actions con path params como `{id}/mensajes` para prevenir ambiguedad en el router. Con atributos de ruta explicitos en ASP.NET Core esto no es problema funcional, pero la convencion del contrato lo documenta.

```csharp
[HttpGet("")]
[ProducesResponseType(typeof(ServiceResponse<ConversacionListResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetConversaciones(
    [FromQuery] string contexto = "todas",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Construir `var query = new GetConversacionesQuery { UserId = userId, Contexto = contexto, Page = page, PageSize = pageSize }`
3. `var response = await _mediator.Send(query)`
4. Si `response.HasErrors` -> `return BadRequest(response)` (solo errores de validacion esperados aqui)
5. Si exito -> `return Ok(response)`

#### GET "no-leidos" - GetNoLeidos

**ORDEN CRITICO:** Este action DEBE declararse ANTES del action `GET {id}/mensajes` en el archivo del controller. Aunque ASP.NET Core con atributos de ruta explicita resuelve correctamente la ambiguedad (el constraint `:guid` en `{id}` rechazaria "no-leidos"), declararlo antes es la convencion del contrato y buena practica.

```csharp
[HttpGet("no-leidos")]
[ProducesResponseType(typeof(ServiceResponse<NoLeidosCountResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetNoLeidos()
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Construir `var query = new GetNoLeidosCountQuery { UserId = userId }`
3. `var response = await _mediator.Send(query)`
4. Si `response.HasErrors` -> `return BadRequest(response)`
5. Si exito -> `return Ok(response)`

#### GET "{id}/mensajes" - GetMensajes

```csharp
[HttpGet("{id:guid}/mensajes")]
[ProducesResponseType(typeof(ServiceResponse<MensajeListResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMensajes(
    [FromRoute] Guid id,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Construir `var query = new GetMensajesQuery { ConversacionId = id, UserId = userId, Page = page, PageSize = pageSize }`
3. `var response = await _mediator.Send(query)`
4. Si `response.HasErrors`:
   - Si contiene `NotFound_Conversacion` -> `return NotFound(response)`
   - Si contiene `Auth_Forbidden` -> `return StatusCode(403, response)`
   - Default -> `return StatusCode(500, response)`
5. Si exito -> `return Ok(response)`

#### POST "{id}/mensajes" - SendMensaje

```csharp
[HttpPost("{id:guid}/mensajes")]
[ProducesResponseType(typeof(ServiceResponse<MensajeDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<MensajeDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> SendMensaje(
    [FromRoute] Guid id,
    [FromBody] SendMensajeCommand command)
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Asignar `command.ConversacionId = id` y `command.UserIdRemitente = userId`
3. `var response = await _mediator.Send(command)`
4. Si `response.HasErrors`:
   - Si contiene `NotFound_Conversacion` -> `return NotFound(response)`
   - Si contiene `Auth_Forbidden` -> `return StatusCode(403, response)`
   - Default -> `return BadRequest(response)`
5. Si exito -> `return StatusCode(StatusCodes.Status201Created, response)`

#### PATCH "{id}/marcar-leidos" - MarcarLeidos

```csharp
[HttpPatch("{id:guid}/marcar-leidos")]
[ProducesResponseType(typeof(ServiceResponse<MarcarLeidosResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> MarcarLeidos([FromRoute] Guid id)
```

**Logica del action:**
1. Extraer `userId = GetUserId()`. Si null -> `return Unauthorized()`
2. Construir `var command = new MarcarLeidosCommand { ConversacionId = id, UserId = userId }`
3. `var response = await _mediator.Send(command)`
4. Si `response.HasErrors`:
   - Si contiene `NotFound_Conversacion` -> `return NotFound(response)`
   - Si contiene `Auth_Forbidden` -> `return StatusCode(403, response)`
   - Default -> `return StatusCode(500, response)`
5. Si exito -> `return Ok(response)`

**Imports del controller:**
```csharp
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

## 9. Estructura de Archivos a Crear

```
src/api/Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs               [MODIFICAR] Agregar 3 constantes nuevas
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── CreateConversacionResultDto.cs               [CREAR]
│   │   ├── ConversacionListResponseDto.cs               [CREAR]
│   │   ├── ConversacionListItemDto.cs                   [CREAR]
│   │   ├── MensajeDto.cs                               [CREAR]
│   │   ├── MensajeListResponseDto.cs                   [CREAR]
│   │   ├── MarcarLeidosResponseDto.cs                  [CREAR]
│   │   ├── NoLeidosCountResponseDto.cs                 [CREAR]
│   │   └── ConversacionResumenItem.cs                  [CREAR] DTO intermedio repositorio->handler
│   │
│   ├── Features/Conversaciones/
│   │   ├── Commands/
│   │   │   ├── CreateConversacionCommand.cs            [CREAR] Command + Handler
│   │   │   ├── SendMensajeCommand.cs                   [CREAR] Command + Handler
│   │   │   └── MarcarLeidosCommand.cs                  [CREAR] Command + Handler
│   │   ├── Queries/
│   │   │   ├── GetConversacionesQuery.cs               [CREAR] Query + Handler
│   │   │   ├── GetMensajesQuery.cs                     [CREAR] Query + Handler
│   │   │   └── GetNoLeidosCountQuery.cs                [CREAR] Query + Handler
│   │   └── Validators/
│   │       ├── CreateConversacionCommandValidator.cs   [CREAR]
│   │       ├── SendMensajeCommandValidator.cs          [CREAR]
│   │       ├── MarcarLeidosCommandValidator.cs         [CREAR]
│   │       ├── GetConversacionesQueryValidator.cs      [CREAR]
│   │       ├── GetMensajesQueryValidator.cs            [CREAR]
│   │       └── GetNoLeidosCountQueryValidator.cs       [CREAR]
│   │
│   └── Mapping/
│       ├── ConversacionCrowdsourcingProfile.cs         [CREAR]
│       └── MensajeCrowdsourcingProfile.cs              [CREAR]
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        └── ConversacionesCrowdsourcingController.cs    [CREAR]
```

**Prerequisitos de Infrastructure (de hexagonal-architecture.md):**
```
WePlayRises.Crowdsourcing.Application/Interfaces/Services/
│   ├── IConversacionCrowdsourcingService.cs   [CREAR - prerequisito]
│   └── IMensajeCrowdsourcingService.cs        [CREAR - prerequisito]
│
WePlayRises.Crowdsourcing.Infra/Services/
│   ├── ConversacionCrowdsourcingService.cs    [CREAR - prerequisito]
│   └── MensajeCrowdsourcingService.cs         [CREAR - prerequisito]
```

---

## 10. Registro de DI

**Archivo a modificar:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

**Agregar en la seccion de Repositories (despues de la linea `IConversacionCrowdsourcingRepository`):**
```csharp
services.AddScoped<IMensajeCrowdsourcingRepository, MensajeCrowdsourcingRepository>();
```

**Agregar en la seccion de Services (despues de la linea de `IAcuerdoCrowdsourcingEntregableService`):**
```csharp
services.AddScoped<IConversacionCrowdsourcingService, ConversacionCrowdsourcingService>();
services.AddScoped<IMensajeCrowdsourcingService, MensajeCrowdsourcingService>();
```

**Los Validators se registran automaticamente** via el `AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)` que ya existe en la configuracion de MediatR del modulo. No se requiere registro manual de cada validator.

---

## 11. Consideraciones de Implementacion Especificas

### 11.1 Resolucion de nombres en GetConversacionesQuery (anti N+1)

El handler de `GetConversacionesQuery` itera sobre los items del listado y resuelve el nombre e imagen de la otra parte via `IArtistaService` e `IPerfilProfesionalService`. Para evitar N+1 queries cuando hay multiples conversaciones con el mismo participante, el handler usa un `Dictionary<string, (string, string?)>` local como cache manual del scope del request.

El `IRequestCacheService` de BuildingBlocks tambien puede usarse en lugar del diccionario local para compartir el cache entre el Validator y el Handler si el Validator necesitara acceder a perfiles (en este caso los Validators no acceden a perfiles, por lo que el diccionario local es suficiente).

### 11.2 Captura de DbUpdateException en CreateConversacionCommand

El `CreateConversacionCommandHandler` captura `DbUpdateException` especificamente para el constraint de unicidad de la tabla `ConversacionCrowdsourcing`. Este es el segundo mecanismo de defensa contra condiciones de carrera. El handler usa:

```csharp
catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
    when (dbEx.InnerException?.Message.Contains("UX_ConversacionCrowdsourcing") == true)
```

El string `"UX_ConversacionCrowdsourcing"` debe coincidir con el nombre del indice unico definido en la configuracion EF (`UX_ConversacionCrowdsourcing_Usuarios_Necesidad` o `UX_ConversacionCrowdsourcing_Usuarios_Acuerdo`). Alternativamente, capturar `SqlException` con codigo 2601 (unique index violation) o 2627 (unique constraint violation) para mayor robustez.

### 11.3 Construcion manual vs AutoMapper en Handlers de Mensajeria

Dado que los DTOs de respuesta (`MensajeDto`, `ConversacionListItemDto`, `CreateConversacionResultDto`) tienen campos calculados (`EsPropio`, `MensajesNoLeidos`, `NombreOtraParte`, `ContextoTipo`, `ContextoTitulo`, `RemitenteNombre`) que no forman parte de la entidad y requieren datos del request (como `UserId`) o de servicios externos (perfiles de usuario), la construccion manual del DTO en el handler es la opcion mas clara.

AutoMapper se puede usar para el mapeo de `CreateConversacionCommand -> ConversacionCrowdsourcing` (solo el campo `Asunto` se mapea directamente), pero para los demas casos la construccion manual es preferible.

### 11.4 Campo ImagenUrl de Artista

El handler de `GetConversacionesQuery` y `GetMensajesQuery` necesita resolver `ImagenOtraParte`. El servicio `IArtistaService.GetByUserIdAsync` retorna la entidad `Artista`. Verificar el nombre del campo de imagen en el modelo `Artista` (puede ser `FotoPerfil`, `ImagenUrl`, `AvatarUrl` u otro). Adaptar segun el campo real del modelo.

### 11.5 Orden de actions en el Controller

El controller debe declarar los actions en este orden para claridad (aunque ASP.NET Core con `:guid` constraint resuelve correctamente):

1. `GET ""` - GetConversaciones
2. `GET "no-leidos"` - GetNoLeidos (antes de los que tienen `{id}`)
3. `POST ""` - CreateConversacion
4. `GET "{id:guid}/mensajes"` - GetMensajes
5. `POST "{id:guid}/mensajes"` - SendMensaje
6. `PATCH "{id:guid}/marcar-leidos"` - MarcarLeidos

### 11.6 Flujo de TieneRelacionAsync en CreateConversacionCommand

El metodo `IConversacionCrowdsourcingService.TieneRelacionAsync` debe verificar si existe alguna de estas condiciones:
- Existe una `PropuestaCrowdsourcing` donde `(UserId == userId1 o artista vinculado al userId1)` y el contexto de la necesidad vincula a `userId2`, Y la propuesta esta en estado no retirado
- Existe un `AcuerdoCrowdsourcing` vinculado al `NecesidadId` o `AcuerdoId` indicado que vincule a ambos usuarios

La implementacion exacta depende de como estan modeladas las relaciones en el dominio y se define en el plan de hexagonal-architecture.md. El Handler solo invoca el metodo del service y actua sobre el resultado booleano.

---

## 12. Codigos de Error por Handler

| Handler | Errores Posibles | HTTP en Controller |
|---------|-----------------|-------------------|
| `CreateConversacionCommandHandler` | `Validation_Required (1001)`, `Validation_MaxLength (1002)`, `BusinessRule_NoRelacionConDestinatario (4016)`, `BusinessRule_ConversacionDuplicada (4015)`, `Internal_UnexpectedError (5000)` | 400 / 403 / 400 / 500 |
| `SendMensajeCommandHandler` | `Validation_Required (1001)`, `Validation_MaxLength (1002)`, `Validation_InvalidUrl (1013)`, `NotFound_Conversacion (2014)`, `Auth_Forbidden (3002)`, `Internal_UnexpectedError (5000)` | 400 / 404 / 403 / 500 |
| `MarcarLeidosCommandHandler` | `Validation_Required (1001)`, `NotFound_Conversacion (2014)`, `Auth_Forbidden (3002)`, `Internal_UnexpectedError (5000)` | 400 / 404 / 403 / 500 |
| `GetConversacionesQueryHandler` | `Validation_Required (1001)`, `Validation_InvalidRange (1009)`, `Internal_UnexpectedError (5000)` | 400 / 500 |
| `GetMensajesQueryHandler` | `Validation_Required (1001)`, `Validation_InvalidRange (1009)`, `NotFound_Conversacion (2014)`, `Auth_Forbidden (3002)`, `Internal_UnexpectedError (5000)` | 400 / 404 / 403 / 500 |
| `GetNoLeidosCountQueryHandler` | `Validation_Required (1001)`, `Internal_UnexpectedError (5000)` | 400 / 500 |

---

## 13. Checklist de Verificacion CQRS

- [ ] `ServiceResponseMessageType.cs` tiene 3 nuevas constantes (`NotFound_Conversacion="2014"`, `BusinessRule_ConversacionDuplicada="4015"`, `BusinessRule_NoRelacionConDestinatario="4016"`)
- [ ] Todos los Commands/Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command/Query en el MISMO archivo (regla critica)
- [ ] Constructores de Handlers con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Handlers NO inyectan `CrowdsourcingContext` ni ningun `DbContext`
- [ ] Handlers usan `IConversacionCrowdsourcingService` e `IMensajeCrowdsourcingService`
- [ ] Validators en carpeta `Validators/` separada (NO en el mismo archivo que el Command/Query)
- [ ] Validators usan `ServiceResponseMessageType.X` constants (NO strings literales como "1001")
- [ ] Validators tienen `.WithMessage(...)` Y `.WithErrorCode(ServiceResponseMessageType.X)` en CADA regla
- [ ] `SendMensajeCommandValidator` tiene metodo helper `BeAValidUrl` para validar la URL adjunta
- [ ] `CreateConversacionCommandValidator` tiene regla custom de exclusividad mutua `NecesidadId`/`AcuerdoId`
- [ ] Validaciones de negocio (participante, relacion, unicidad) en Handlers, NO en Validators
- [ ] `CreateConversacionCommandHandler` captura `DbUpdateException` por constraint de unicidad SQL
- [ ] `GetConversacionesQueryHandler` resuelve `NombreOtraParte` e `ImagenOtraParte` con cache manual (dictionary)
- [ ] `GetMensajesQueryHandler` y `SendMensajeCommandHandler` resuelven `RemitenteNombre` con cache manual
- [ ] `EsPropio` calculado en handler como `mensaje.UserIdRemitente == request.UserId` (NO en AutoMapper)
- [ ] Try-catch con `_logger.LogError(ex, ...)` en todos los Handlers
- [ ] Try-catch usa `ValidateExtensions.InternalServerErrorServiceResponse<T>(...)` para errores internos
- [ ] AutoMapper Profiles creados: `ConversacionCrowdsourcingProfile` y `MensajeCrowdsourcingProfile`
- [ ] DTOs de Application creados (7 DTOs de respuesta + `ConversacionResumenItem` intermedio)
- [ ] Controller extrae `UserId` desde `User.FindFirstValue(ClaimTypes.NameIdentifier)` (NO del body)
- [ ] Action `GetNoLeidos` declarado ANTES de `{id:guid}/mensajes` en el controller
- [ ] Action `CreateConversacion` asigna `command.UserIdCreador = userId` antes de `_mediator.Send`
- [ ] Action `SendMensaje` asigna `command.ConversacionId` y `command.UserIdRemitente` antes de `_mediator.Send`
- [ ] Controller mapea `BusinessRule_NoRelacionConDestinatario` a HTTP 403 (igual que `Auth_Forbidden`)
- [ ] Controller mapea `BusinessRule_ConversacionDuplicada` a HTTP 400
- [ ] `ProducesResponseType` en todas las actions del controller para todos los status codes esperados
- [ ] `DependencyInjection.cs` actualizado con `IMensajeCrowdsourcingRepository`, `IConversacionCrowdsourcingService`, `IMensajeCrowdsourcingService`
- [ ] Ningun handler inyecta `IRequestCacheService` directamente (el caching de entidades esta en los Services, no en los Handlers)
