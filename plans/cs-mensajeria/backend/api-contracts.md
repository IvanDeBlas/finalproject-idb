# Contratos API: cs-mensajeria (US-CS-05 - Mensajeria entre Partes)

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-mensajeria

---

## Contexto de Entidades Existentes

Antes de disenar los contratos, se verifico el estado actual del modulo Crowdsourcing:

**Entidades ya existentes (creadas por US-CS-04):**
- `ConversacionCrowdsourcing` - Existe en `Domain/Model/`. Usa `UserIdArtista` y `UserIdProveedor` (NO `UserIdCreador`/`UserIdDestinatario` como definen los contratos).
- `MensajeCrowdsourcing` - Existe en `Domain/Model/`. Tiene todos los campos requeridos.

**Discrepancia critica de nomenclatura:**
El modelo de dominio existente usa `UserIdArtista` / `UserIdProveedor` mientras que los contratos de la feature definen `UserIdCreador` / `UserIdDestinatario`. El plan asume que la implementacion usara los nombres del dominio existente (`UserIdArtista` / `UserIdProveedor`) y el mapping lo resolvera. El implementador debe decidir si renombrar la entidad o adaptar el mapping.

**Para este plan se asume:**
- `UserIdArtista` = quien crea la conversacion (equivalente a `UserIdCreador` del contrato)
- `UserIdProveedor` = destinatario (equivalente a `UserIdDestinatario` del contrato)
- La verificacion de participante se hace: `UserId == UserIdArtista OR UserId == UserIdProveedor`

**Nuevas constantes requeridas en `ServiceResponseMessageType.cs`:**
```csharp
// Agregar al final del archivo existente:
public const string NotFound_Conversacion = "2014";
public const string BusinessRule_ConversacionDuplicada = "4015";
public const string BusinessRule_NoRelacionConDestinatario = "4016";
```

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion | Auth |
|--------|------|------|-------------|------|
| POST | /api/crowdsourcing/conversaciones | Command | Crear conversacion nueva | Bearer JWT |
| GET | /api/crowdsourcing/conversaciones | Query | Listar conversaciones del usuario | Bearer JWT |
| GET | /api/crowdsourcing/conversaciones/no-leidos | Query | Conteo total de mensajes no leidos | Bearer JWT |
| GET | /api/crowdsourcing/conversaciones/{id}/mensajes | Query | Obtener mensajes paginados | Bearer JWT + Participante |
| POST | /api/crowdsourcing/conversaciones/{id}/mensajes | Command | Enviar mensaje | Bearer JWT + Participante |
| PATCH | /api/crowdsourcing/conversaciones/{id}/marcar-leidos | Command | Marcar mensajes como leidos | Bearer JWT + Participante |

**Nota sobre el orden de rutas:** El endpoint `GET /no-leidos` DEBE registrarse ANTES de `GET /{id}/mensajes` en el controller para evitar que el router de ASP.NET Core intente interpretar "no-leidos" como un `{id}` de tipo Guid (lo rechazara por constraint de tipo, pero el orden correcto previene ambiguedades). En ASP.NET Core con atributos de ruta explicitos esto no es un problema, pero se documenta por claridad.

---

## 2. Request DTOs (Commands y Queries)

### 2.1 CreateConversacionCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/CreateConversacionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<CreateConversacionResultDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `NecesidadId` | `Guid?` | Condicional | FK a NecesidadCrowdsourcing. Mutuamente excluyente con AcuerdoId. |
| `AcuerdoId` | `Guid?` | Condicional | FK a AcuerdoCrowdsourcing. Mutuamente excluyente con NecesidadId. |
| `UserIdDestinatario` | `string` | Si | UserId del destinatario (Identity User string). Viene del body del request. |
| `Asunto` | `string` | Si | Asunto de la conversacion. Min 1, max 200 chars. |
| `UserIdCreador` | `string` | Si | UserId del creador. Se obtiene del token JWT en el controller, NO del body. |

**Estructura de clase:**
```csharp
public class CreateConversacionCommand : IRequest<ServiceResponse<CreateConversacionResultDto>>
{
    public Guid? NecesidadId { get; set; }
    public Guid? AcuerdoId { get; set; }
    public string UserIdDestinatario { get; set; } = null!;
    public string Asunto { get; set; } = null!;

    // Inyectado por el controller desde el token JWT, no expuesto en el body
    public string UserIdCreador { get; set; } = null!;
}
```

**Nota:** El body del request solo expone `NecesidadId`, `AcuerdoId`, `UserIdDestinatario` y `Asunto`. El controller extrae `UserIdCreador` del claim `ClaimTypes.NameIdentifier` del token y lo asigna al command antes de enviarlo via MediatR.

---

### 2.2 GetConversacionesQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetConversacionesQuery.cs`

**Implementa:** `IRequest<ServiceResponse<ConversacionListResponseDto>>`

| Propiedad | Tipo | Requerido | Default | Descripcion |
|-----------|------|-----------|---------|-------------|
| `UserId` | `string` | Si | - | Extraido del token JWT en el controller. |
| `Contexto` | `string?` | No | `"todas"` | Filtro: `"todas"`, `"necesidades"`, `"acuerdos"`. |
| `Page` | `int` | No | `1` | Numero de pagina (1-based). |
| `PageSize` | `int` | No | `20` | Elementos por pagina. Max 50. |

**Estructura de clase:**
```csharp
public class GetConversacionesQuery : IRequest<ServiceResponse<ConversacionListResponseDto>>
{
    public string UserId { get; set; } = null!;
    public string Contexto { get; set; } = "todas";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

---

### 2.3 GetNoLeidosCountQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetNoLeidosCountQuery.cs`

**Implementa:** `IRequest<ServiceResponse<NoLeidosCountResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `UserId` | `string` | Si | Extraido del token JWT en el controller. |

**Estructura de clase:**
```csharp
public class GetNoLeidosCountQuery : IRequest<ServiceResponse<NoLeidosCountResponseDto>>
{
    public string UserId { get; set; } = null!;
}
```

---

### 2.4 GetMensajesQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Queries/GetMensajesQuery.cs`

**Implementa:** `IRequest<ServiceResponse<MensajeListResponseDto>>`

| Propiedad | Tipo | Requerido | Default | Descripcion |
|-----------|------|-----------|---------|-------------|
| `ConversacionId` | `Guid` | Si | - | ID de la conversacion (path param). |
| `UserId` | `string` | Si | - | Extraido del token JWT. Usado para validar participacion y calcular `EsPropio`. |
| `Page` | `int` | No | `1` | Numero de pagina (1-based). |
| `PageSize` | `int` | No | `50` | Elementos por pagina. Max 100. |

**Estructura de clase:**
```csharp
public class GetMensajesQuery : IRequest<ServiceResponse<MensajeListResponseDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
```

---

### 2.5 SendMensajeCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/SendMensajeCommand.cs`

**Implementa:** `IRequest<ServiceResponse<MensajeDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `ConversacionId` | `Guid` | Si | ID de la conversacion (path param). |
| `UserIdRemitente` | `string` | Si | Extraido del token JWT en el controller. |
| `Contenido` | `string` | Si | Texto del mensaje. Min 1, max 5000 chars. |
| `UrlAdjunto` | `string?` | No | URL valida si se proporciona. Max 2048 chars. |

**Estructura de clase:**
```csharp
public class SendMensajeCommand : IRequest<ServiceResponse<MensajeDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserIdRemitente { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public string? UrlAdjunto { get; set; }
}
```

**Nota:** El body del request solo expone `Contenido` y `UrlAdjunto`. El controller asigna `ConversacionId` desde el path param y `UserIdRemitente` desde el token JWT.

---

### 2.6 MarcarLeidosCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Commands/MarcarLeidosCommand.cs`

**Implementa:** `IRequest<ServiceResponse<MarcarLeidosResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `ConversacionId` | `Guid` | Si | ID de la conversacion (path param). |
| `UserId` | `string` | Si | Extraido del token JWT. Solo se marcan leidos los mensajes cuyo remitente NO sea este UserId. |

**Estructura de clase:**
```csharp
public class MarcarLeidosCommand : IRequest<ServiceResponse<MarcarLeidosResponseDto>>
{
    public Guid ConversacionId { get; set; }
    public string UserId { get; set; } = null!;
}
```

**Nota:** Este command no tiene body de request. El controller construye el command con los datos del path param y del token JWT.

---

## 3. Response DTOs

### 3.1 CreateConversacionResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CreateConversacionResultDto.cs`

**Wrapped en:** `ServiceResponse<CreateConversacionResultDto>` (HTTP 201)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID de la conversacion creada. |
| `Asunto` | `string` | Asunto de la conversacion. |
| `NombreDestinatario` | `string` | Nombre de perfil del destinatario (artista o profesional). |
| `ContextoTipo` | `string` | `"necesidad"` o `"acuerdo"`. |
| `ContextoTitulo` | `string` | Titulo de la necesidad o del acuerdo vinculado. |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion. |

---

### 3.2 ConversacionListResponseDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ConversacionListResponseDto.cs`

**Wrapped en:** `ServiceResponse<ConversacionListResponseDto>` (HTTP 200)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Items` | `List<ConversacionListItemDto>` | Lista paginada de conversaciones. |
| `TotalCount` | `int` | Total de conversaciones del usuario (antes de paginar). |
| `TotalNoLeidos` | `int` | Suma global de mensajes no leidos en TODAS las conversaciones (no paginado). |
| `Page` | `int` | Pagina actual. |
| `PageSize` | `int` | Elementos por pagina. |

---

### 3.3 ConversacionListItemDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ConversacionListItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID de la conversacion. |
| `Asunto` | `string` | Asunto de la conversacion. |
| `NombreOtraParte` | `string` | Nombre del otro participante (artista o profesional). |
| `ImagenOtraParte` | `string?` | URL de avatar del otro participante. Null si no tiene foto. |
| `ContextoTipo` | `string` | `"necesidad"` o `"acuerdo"`. |
| `ContextoTitulo` | `string` | Titulo de la necesidad o del acuerdo. |
| `UltimoMensaje` | `string?` | Preview del ultimo mensaje truncado a 80 chars. Null si no hay mensajes. |
| `FechaUltimoMensaje` | `DateTime?` | Fecha del ultimo mensaje. Null si no hay mensajes. |
| `MensajesNoLeidos` | `int` | COUNT de mensajes no leidos del otro participante en esta conversacion. |

---

### 3.4 MensajeDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MensajeDto.cs`

**Wrapped en:** `ServiceResponse<MensajeDto>` para POST (HTTP 201), o como item de `MensajeListResponseDto` para GET.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del mensaje. |
| `Contenido` | `string` | Texto del mensaje. |
| `UrlAdjunto` | `string?` | URL del adjunto. Null si no tiene. |
| `RemitenteNombre` | `string` | Nombre de perfil del remitente. |
| `EsPropio` | `bool` | `true` si `UserIdRemitente == UserId del token`. Calculado en handler. |
| `Leido` | `bool` | Estado de lectura del mensaje. |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion. |

---

### 3.5 MensajeListResponseDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MensajeListResponseDto.cs`

**Wrapped en:** `ServiceResponse<MensajeListResponseDto>` (HTTP 200)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Items` | `List<MensajeDto>` | Lista paginada de mensajes. Orden: `FechaCreacion ASC` (cronologico). |
| `TotalCount` | `int` | Total de mensajes en la conversacion (antes de paginar). |
| `Page` | `int` | Pagina actual. |
| `PageSize` | `int` | Elementos por pagina. |

**Nota sobre orden:** Los mensajes se retornan en orden cronologico ascendente (mas antiguos primero). El frontend invierte el display si necesita mostrar los recientes al fondo con scroll-to-bottom.

---

### 3.6 MarcarLeidosResponseDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MarcarLeidosResponseDto.cs`

**Wrapped en:** `ServiceResponse<MarcarLeidosResponseDto>` (HTTP 200)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `MensajesMarcados` | `int` | Numero de mensajes que pasaron de `Leido=false` a `Leido=true`. 0 si no habia no leidos (no es error). |

---

### 3.7 NoLeidosCountResponseDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NoLeidosCountResponseDto.cs`

**Wrapped en:** `ServiceResponse<NoLeidosCountResponseDto>` (HTTP 200)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `TotalNoLeidos` | `int` | Total de mensajes no leidos del usuario en todas sus conversaciones. |

---

## 4. Validadores (FluentValidation)

### 4.1 CreateConversacionCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/CreateConversacionCommandValidator.cs`

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constant |
|-------|------------------------|---------|-------------------|
| `Asunto` | `.NotEmpty()` | "El asunto de la conversacion es obligatorio" | `Validation_Required` |
| `Asunto` | `.MaximumLength(200)` | "El asunto no puede superar los 200 caracteres" | `Validation_MaxLength` |
| `UserIdDestinatario` | `.NotEmpty()` | "El destinatario es obligatorio" | `Validation_Required` |
| `UserIdCreador` | `.NotEmpty()` | "Error de autenticacion: usuario no identificado" | `Validation_Required` |
| `NecesidadId / AcuerdoId` | Regla custom: exactamente uno de los dos debe tener valor | "Debe especificar una necesidad o un acuerdo como contexto, pero no ambos" | `Validation_Required` |

**Regla custom para exclusividad mutua:**
```csharp
RuleFor(x => x)
    .Must(x => (x.NecesidadId.HasValue && !x.AcuerdoId.HasValue)
               || (!x.NecesidadId.HasValue && x.AcuerdoId.HasValue))
    .WithMessage("Debe especificar exactamente un contexto: necesidad o acuerdo")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required);
```

**Validaciones de negocio (en el Handler, NO en el Validator):**
- Verificar que exista relacion entre `UserIdCreador` y `UserIdDestinatario` (propuesta o acuerdo que los vincule). Si no, retornar `BusinessRule_NoRelacionConDestinatario` ("4016").
- Verificar que no exista ya una conversacion para el mismo contexto (mismo `UserIdArtista` + `UserIdProveedor` + mismo `NecesidadId` o `AcuerdoId`). Si ya existe, retornar `BusinessRule_ConversacionDuplicada` ("4015"). El handler tambien debe capturar `DbUpdateException` por el indice unico filtrado en SQL Server como segundo mecanismo de defensa.

---

### 4.2 GetConversacionesQueryValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetConversacionesQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `Validation_Required` |
| `Page` | `.GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `Validation_InvalidRange` |
| `PageSize` | `.InclusiveBetween(1, 50)` | "El tamano de pagina debe estar entre 1 y 50" | `Validation_InvalidRange` |
| `Contexto` | `.Must(v => v == "todas" || v == "necesidades" || v == "acuerdos").When(x => !string.IsNullOrEmpty(x.Contexto))` | "Valor de contexto invalido. Use: todas, necesidades, acuerdos" | `Validation_Required` |

---

### 4.3 GetMensajesQueryValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetMensajesQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `Validation_Required` |
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `Validation_Required` |
| `Page` | `.GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `Validation_InvalidRange` |
| `PageSize` | `.InclusiveBetween(1, 100)` | "El tamano de pagina debe estar entre 1 y 100" | `Validation_InvalidRange` |

**Nota:** La validacion de participante (que el `UserId` sea artista o proveedor de la conversacion) se realiza en el Handler, no en el Validator, para evitar una query adicional a DB en el Validator. Retornar `Auth_Forbidden` ("3002") si no es participante.

---

### 4.4 SendMensajeCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/SendMensajeCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `Validation_Required` |
| `UserIdRemitente` | `.NotEmpty()` | "Usuario no identificado" | `Validation_Required` |
| `Contenido` | `.NotEmpty()` | "El contenido del mensaje es obligatorio" | `Validation_Required` |
| `Contenido` | `.MaximumLength(5000)` | "El mensaje no puede superar los 5000 caracteres" | `Validation_MaxLength` |
| `UrlAdjunto` | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlAdjunto))` | "La URL adjunta no es valida. Debe ser una URL completa (ej: https://...)" | `Validation_InvalidUrl` |

**Metodo helper `BeAValidUrl`:**
```csharp
private static bool BeAValidUrl(string? url)
{
    return Uri.TryCreate(url, UriKind.Absolute, out var result)
           && (result.Scheme == Uri.UriSchemeHttps || result.Scheme == Uri.UriSchemeHttp);
}
```

**Nota:** La validacion de que `UrlAdjunto` no supere 2048 chars (limite de la columna) se puede agregar:
`.MaximumLength(2048).When(x => !string.IsNullOrEmpty(x.UrlAdjunto)).WithMessage("La URL no puede superar los 2048 caracteres").WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)`

**Validaciones de negocio (en el Handler):**
- Verificar que la conversacion existe. Si no, retornar `NotFound_Conversacion` ("2014").
- Verificar que `UserIdRemitente` sea participante de la conversacion (`UserIdArtista` o `UserIdProveedor`). Si no, retornar `Auth_Forbidden` ("3002").

---

### 4.5 MarcarLeidosCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/MarcarLeidosCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `ConversacionId` | `.NotEmpty()` | "El ID de la conversacion es obligatorio" | `Validation_Required` |
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `Validation_Required` |

**Validaciones de negocio (en el Handler):**
- Verificar que la conversacion existe. Si no, retornar `NotFound_Conversacion` ("2014").
- Verificar que `UserId` sea participante. Si no, retornar `Auth_Forbidden` ("3002").

---

### 4.6 GetNoLeidosCountQueryValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Conversaciones/Validators/GetNoLeidosCountQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `UserId` | `.NotEmpty()` | "Usuario no identificado" | `Validation_Required` |

---

## 5. AutoMapper Profiles

### 5.1 ConversacionCrowdsourcingProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/ConversacionCrowdsourcingProfile.cs`

| Source | Destination | Notas de Mapping |
|--------|-------------|-----------------|
| `CreateConversacionCommand` | `ConversacionCrowdsourcing` | Ignorar: `Id`, `UserIdArtista` (viene del command.UserIdCreador pero con nombre distinto), `UserIdProveedor`, `FechaCreacion`, `FechaUltimoMensaje`, `Necesidad`, `Acuerdo`, `Mensajes`. El handler asigna manualmente los campos de identidad y fechas. |
| `ConversacionCrowdsourcing` | `CreateConversacionResultDto` | `NombreDestinatario` -> Ignorar (resuelto por el handler via servicio de perfil). `ContextoTipo` -> Ignorar (calculado: si `NecesidadId != null` -> "necesidad", sino "acuerdo"). `ContextoTitulo` -> Ignorar (resuelto por el handler). |
| `ConversacionCrowdsourcing` | `ConversacionListItemDto` | `NombreOtraParte` -> Ignorar (resuelto por el handler). `ImagenOtraParte` -> Ignorar. `ContextoTipo` -> Ignorar (calculado). `ContextoTitulo` -> Ignorar. `UltimoMensaje` -> Ignorar (calculado por query SQL). `FechaUltimoMensaje` -> MapFrom src. `MensajesNoLeidos` -> Ignorar (calculado por query SQL). |

**Detalle de mapeos especiales:**
- `ConversacionCrowdsourcing.Id` es `Guid` (no strongly-typed ID), por lo que el mapping es directo.
- `NecesidadId` en la entidad es `NecesidadCrowdsourcingId?` (strongly-typed ID). En el command es `Guid?`. El handler asigna manualmente: `entity.NecesidadId = request.NecesidadId.HasValue ? new NecesidadCrowdsourcingId(request.NecesidadId.Value) : null`.
- Misma logica para `AcuerdoId`.

---

### 5.2 MensajeCrowdsourcingProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/MensajeCrowdsourcingProfile.cs`

| Source | Destination | Notas de Mapping |
|--------|-------------|-----------------|
| `SendMensajeCommand` | `MensajeCrowdsourcing` | Ignorar: `Id`, `ConversacionId` (asignado manualmente), `Leido` (default false), `FechaLeido` (null), `FechaCreacion`, `Conversacion`. Mapear: `Contenido`, `UrlAdjunto`, `UserIdRemitente`. |
| `MensajeCrowdsourcing` | `MensajeDto` | `RemitenteNombre` -> Ignorar (resuelto por handler via servicio de perfil). `EsPropio` -> Ignorar (calculado en handler: `src.UserIdRemitente == currentUserId`). |

---

## 6. Servicios de Aplicacion (Interfaces)

### 6.1 IConversacionCrowdsourcingService

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IConversacionCrowdsourcingService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `CreateAsync` | `Task<Guid>` | `ConversacionCrowdsourcing entity, CancellationToken ct` | Crea la conversacion y retorna el Guid del nuevo Id. |
| `GetByIdAsync` | `Task<ConversacionCrowdsourcing?>` | `Guid id, CancellationToken ct` | Obtiene la conversacion por ID. Null si no existe. Usa cache. |
| `GetListByUserIdAsync` | `Task<(List<ConversacionCrowdsourcing> items, int totalCount, int totalNoLeidos)>` | `string userId, string contexto, int page, int pageSize, CancellationToken ct` | Listado paginado con conteo. `totalNoLeidos` es la suma global. |
| `ExisteConversacionParaContextoAsync` | `Task<bool>` | `string userIdArtista, string userIdProveedor, Guid? necesidadId, Guid? acuerdoId, CancellationToken ct` | Verifica si ya existe una conversacion para ese contexto entre esas partes. |
| `TieneRelacionAsync` | `Task<bool>` | `string userId1, string userId2, Guid? necesidadId, Guid? acuerdoId, CancellationToken ct` | Verifica que exista propuesta o acuerdo que vincule a los dos usuarios en ese contexto. |
| `EsParticipanteAsync` | `Task<bool>` | `Guid conversacionId, string userId, CancellationToken ct` | Retorna true si el userId es `UserIdArtista` o `UserIdProveedor` de la conversacion. Usa cache. |

---

### 6.2 IMensajeCrowdsourcingService

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IMensajeCrowdsourcingService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `CreateAsync` | `Task<Guid>` | `MensajeCrowdsourcing entity, CancellationToken ct` | Crea el mensaje Y actualiza `FechaUltimoMensaje` de la conversacion en la misma transaccion. |
| `GetByConversacionAsync` | `Task<(List<MensajeCrowdsourcing> items, int totalCount)>` | `Guid conversacionId, int page, int pageSize, CancellationToken ct` | Mensajes paginados en orden `FechaCreacion ASC`. |
| `MarcarLeidosAsync` | `Task<int>` | `Guid conversacionId, string userIdSolicitante, CancellationToken ct` | UPDATE en batch: `Leido=true, FechaLeido=UtcNow` para mensajes donde `UserIdRemitente != userIdSolicitante AND Leido=false`. Retorna count de filas afectadas. |
| `GetTotalNoLeidosAsync` | `Task<int>` | `string userId, CancellationToken ct` | COUNT total de mensajes no leidos del usuario en todas sus conversaciones. Candidato a cache de request. |

---

## 7. Logica de los Handlers (Flujo Detallado)

### 7.1 CreateConversacionCommandHandler

**Flujo:**
1. Validar con `CreateConversacionCommandValidator`. Si falla, retornar `ServiceResponse` con errores.
2. Verificar relacion via `IConversacionCrowdsourcingService.TieneRelacionAsync`. Si no existe relacion, retornar `BusinessRule_NoRelacionConDestinatario` (403 en controller).
3. Verificar duplicado via `IConversacionCrowdsourcingService.ExisteConversacionParaContextoAsync`. Si ya existe, retornar `BusinessRule_ConversacionDuplicada` (400 en controller).
4. Crear entidad `ConversacionCrowdsourcing`. Asignar `UserIdArtista = request.UserIdCreador`, `UserIdProveedor = request.UserIdDestinatario`, `FechaCreacion = DateTime.UtcNow`, `FechaUltimoMensaje = null`.
5. Persistir via `IConversacionCrowdsourcingService.CreateAsync`.
6. Resolver `NombreDestinatario`, `ContextoTipo` y `ContextoTitulo` (via servicio o consulta directa).
7. Retornar `ServiceResponse<CreateConversacionResultDto>` con `Messages` conteniendo `{ Message = "Conversacion creada", ErrorCode = ServiceResponseMessageType.Created }` y `HttpStatusCode = HttpStatusCode.Created`.
8. En `catch(DbUpdateException)` verificar codigo de constraint de unicidad -> retornar `BusinessRule_ConversacionDuplicada`.
9. En `catch(Exception)` loguear y retornar `Internal_UnexpectedError`.

---

### 7.2 GetConversacionesQueryHandler

**Flujo:**
1. Validar con `GetConversacionesQueryValidator`. Si falla, retornar errores.
2. Llamar `IConversacionCrowdsourcingService.GetListByUserIdAsync`. El servicio aplica: filtro por contexto, ordenacion (`FechaUltimoMensaje DESC NULLS LAST, FechaCreacion DESC`), paginacion, calculo de `MensajesNoLeidos` por conversacion y `TotalNoLeidos` global.
3. Para cada item del listado, resolver `NombreOtraParte` e `ImagenOtraParte` consultando el perfil del otro participante via `IRequestCacheService` (evita N+1 queries si el mismo usuario aparece en varias conversaciones).
4. Truncar `UltimoMensaje` a 80 chars si supera ese limite.
5. Retornar `ServiceResponse<ConversacionListResponseDto>` con lista paginada.

---

### 7.3 GetMensajesQueryHandler

**Flujo:**
1. Validar con `GetMensajesQueryValidator`.
2. Verificar que la conversacion existe via `IConversacionCrowdsourcingService.GetByIdAsync`. Si null, retornar `NotFound_Conversacion` (404 en controller).
3. Verificar que `request.UserId` es participante. Si no, retornar `Auth_Forbidden` (403 en controller).
4. Llamar `IMensajeCrowdsourcingService.GetByConversacionAsync` con paginacion. Orden: `FechaCreacion ASC`.
5. Para cada mensaje, resolver `RemitenteNombre` via `IRequestCacheService` (evita N+1).
6. Calcular `EsPropio = mensaje.UserIdRemitente == request.UserId` por cada item.
7. Retornar `ServiceResponse<MensajeListResponseDto>`.

---

### 7.4 SendMensajeCommandHandler

**Flujo:**
1. Validar con `SendMensajeCommandValidator`.
2. Verificar que la conversacion existe. Si null, retornar `NotFound_Conversacion` (404).
3. Verificar que `UserIdRemitente` es participante. Si no, retornar `Auth_Forbidden` (403).
4. Crear entidad `MensajeCrowdsourcing`: `Leido = false`, `FechaLeido = null`, `FechaCreacion = DateTime.UtcNow`.
5. Persistir via `IMensajeCrowdsourcingService.CreateAsync` (incluye `UPDATE ConversacionCrowdsourcing SET FechaUltimoMensaje = UtcNow`).
6. Resolver `RemitenteNombre` del usuario autenticado.
7. Construir `MensajeDto` con `EsPropio = true` (el remitente siempre es el propio usuario).
8. Retornar `ServiceResponse<MensajeDto>` con `HttpStatusCode.Created` y `{ Message = "Mensaje enviado", ErrorCode = ServiceResponseMessageType.Created }`.

---

### 7.5 MarcarLeidosCommandHandler

**Flujo:**
1. Validar con `MarcarLeidosCommandValidator`.
2. Verificar que la conversacion existe. Si null, retornar `NotFound_Conversacion` (404).
3. Verificar que `UserId` es participante. Si no, retornar `Auth_Forbidden` (403).
4. Llamar `IMensajeCrowdsourcingService.MarcarLeidosAsync`. Retorna count de filas afectadas.
5. Retornar `ServiceResponse<MarcarLeidosResponseDto>` con `{ MensajesMarcados = count }`. Si count es 0, respuesta exitosa igualmente (no es error).

---

### 7.6 GetNoLeidosCountQueryHandler

**Flujo:**
1. Validar con `GetNoLeidosCountQueryValidator`.
2. Llamar `IMensajeCrowdsourcingService.GetTotalNoLeidosAsync`. Candidato a usar `IRequestCacheService` si el listado de conversaciones y este endpoint se invocan en el mismo request.
3. Retornar `ServiceResponse<NoLeidosCountResponseDto>`.

---

## 8. Documentacion OpenAPI/Swagger

### POST /api/crowdsourcing/conversaciones

- **Summary:** Crear nueva conversacion entre dos partes con relacion previa
- **Description:** Crea una conversacion vinculada a una necesidad o a un acuerdo. Si ya existe una conversacion para el mismo contexto entre las mismas partes, retorna error 400 con codigo 4015. El UserId del creador se extrae del token JWT.
- **Request Body:** `CreateConversacionRequest` (body JSON; `UserIdCreador` no se incluye en el body)
  ```json
  {
    "necesidadId": "guid | null",
    "acuerdoId": "guid | null",
    "userIdDestinatario": "string",
    "asunto": "string (max 200)"
  }
  ```
- **Responses:**
  - `201 Created`: `ServiceResponse<CreateConversacionResultDto>` - Conversacion creada exitosamente
  - `400 Bad Request`: `ServiceResponse` con errores de validacion (1001, 1002) o `BusinessRule_ConversacionDuplicada` (4015)
  - `401 Unauthorized`: Token JWT invalido o expirado (3001)
  - `403 Forbidden`: Sin relacion con el destinatario (4016)
  - `500 Internal Server Error`: Error inesperado (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

### GET /api/crowdsourcing/conversaciones

- **Summary:** Listar conversaciones del usuario autenticado
- **Description:** Retorna las conversaciones del usuario (como artista o proveedor) ordenadas por `FechaUltimoMensaje` descendente. Soporta filtrado por contexto y paginacion.
- **Query Parameters:**
  - `contexto` (string, opcional, default: "todas") - Valores: `todas`, `necesidades`, `acuerdos`
  - `page` (int, opcional, default: 1)
  - `pageSize` (int, opcional, default: 20, max: 50)
- **Responses:**
  - `200 OK`: `ServiceResponse<ConversacionListResponseDto>`
  - `401 Unauthorized`: (3001)
  - `500 Internal Server Error`: (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

### GET /api/crowdsourcing/conversaciones/no-leidos

- **Summary:** Conteo total de mensajes no leidos del usuario
- **Description:** Endpoint ligero para el badge del navbar. Retorna el total de mensajes no leidos del usuario en todas sus conversaciones. Disenado para polling cada 10 segundos.
- **Responses:**
  - `200 OK`: `ServiceResponse<NoLeidosCountResponseDto>`
  - `401 Unauthorized`: (3001)
  - `500 Internal Server Error`: (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

### GET /api/crowdsourcing/conversaciones/{id}/mensajes

- **Summary:** Obtener mensajes de una conversacion
- **Description:** Retorna los mensajes de la conversacion en orden cronologico ascendente (mas antiguos primero). Solo los dos participantes pueden acceder. El campo `esPropio` se calcula comparando `UserIdRemitente` con el UserId del token.
- **Path Parameters:** `id` (Guid, requerido) - ID de la conversacion
- **Query Parameters:**
  - `page` (int, opcional, default: 1)
  - `pageSize` (int, opcional, default: 50, max: 100)
- **Responses:**
  - `200 OK`: `ServiceResponse<MensajeListResponseDto>`
  - `401 Unauthorized`: (3001)
  - `403 Forbidden`: No es participante de la conversacion (3002)
  - `404 Not Found`: Conversacion no encontrada (2014)
  - `500 Internal Server Error`: (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

### POST /api/crowdsourcing/conversaciones/{id}/mensajes

- **Summary:** Enviar mensaje en una conversacion
- **Description:** Crea un nuevo mensaje en la conversacion. Al crear el mensaje se actualiza `FechaUltimoMensaje` de la conversacion. El mensaje se crea con `Leido = false`. Solo los dos participantes pueden enviar mensajes.
- **Path Parameters:** `id` (Guid, requerido) - ID de la conversacion
- **Request Body:** `CreateMensajeRequest`
  ```json
  {
    "contenido": "string (min 1, max 5000)",
    "urlAdjunto": "string (URL valida) | null"
  }
  ```
- **Responses:**
  - `201 Created`: `ServiceResponse<MensajeDto>` - Mensaje creado con `esPropio = true`
  - `400 Bad Request`: Errores de validacion (1001 contenido vacio, 1002 contenido muy largo, 1013 URL invalida)
  - `401 Unauthorized`: (3001)
  - `403 Forbidden`: No es participante (3002)
  - `404 Not Found`: Conversacion no encontrada (2014)
  - `500 Internal Server Error`: (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

### PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos

- **Summary:** Marcar mensajes no leidos como leidos
- **Description:** Marca como leidos todos los mensajes no leidos del otro participante en la conversacion (mensajes donde `UserIdRemitente != UserId del token AND Leido = false`). Establece `Leido = true` y `FechaLeido = DateTime.UtcNow`. Se invoca automaticamente al abrir una conversacion. Retorna 200 con `mensajesMarcados = 0` si no habia mensajes no leidos.
- **Path Parameters:** `id` (Guid, requerido) - ID de la conversacion
- **Request Body:** Ninguno.
- **Responses:**
  - `200 OK`: `ServiceResponse<MarcarLeidosResponseDto>` con `mensajesMarcados`
  - `401 Unauthorized`: (3001)
  - `403 Forbidden`: No es participante (3002)
  - `404 Not Found`: Conversacion no encontrada (2014)
  - `500 Internal Server Error`: (5000)
- **Auth:** Bearer JWT requerido. `[Authorize]`

---

## 9. Controller: ConversacionesCrowdsourcingController

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/ConversacionesCrowdsourcingController.cs`

**Ruta base:** `[Route("api/crowdsourcing/conversaciones")]`

**Patron de extraccion del UserId en cada action:**
```csharp
var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
```

**Patron de retorno HTTP segun ErrorCode:**
```csharp
if (response.HasErrors)
{
    if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Conversacion))
        return NotFound(response);

    if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden))
        return StatusCode(StatusCodes.Status403Forbidden, response);

    if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_NoRelacionConDestinatario))
        return StatusCode(StatusCodes.Status403Forbidden, response);

    return BadRequest(response);
}
```

**Actions del controller:**

| Action | Metodo HTTP | Ruta | Retorno exitoso |
|--------|-------------|------|-----------------|
| `CreateConversacion` | POST | `""` | `Created(location, response)` con status 201 |
| `GetConversaciones` | GET | `""` | `Ok(response)` con status 200 |
| `GetNoLeidos` | GET | `"no-leidos"` | `Ok(response)` con status 200 |
| `GetMensajes` | GET | `"{id}/mensajes"` | `Ok(response)` con status 200 |
| `SendMensaje` | POST | `"{id}/mensajes"` | `Created(location, response)` con status 201 |
| `MarcarLeidos` | PATCH | `"{id}/marcar-leidos"` | `Ok(response)` con status 200 |

**Importante:** El action `GetNoLeidos` con ruta `"no-leidos"` debe declararse ANTES de `GetMensajes` con ruta `"{id}/mensajes"` para prevenir ambiguedades en el router, aunque con atributos de ruta explicitos en ASP.NET Core esto no causa problema funcional.

---

## 10. Codigos de Error Completos

| HTTP Status | ErrorCode | Constant | Causa |
|-------------|-----------|----------|-------|
| 400 | 1001 | `Validation_Required` | Campo requerido vacio |
| 400 | 1002 | `Validation_MaxLength` | Campo supera longitud maxima |
| 400 | 1009 | `Validation_InvalidRange` | Paginacion fuera de rango |
| 400 | 1013 | `Validation_InvalidUrl` | URL adjunta invalida |
| 400 | 4015 | `BusinessRule_ConversacionDuplicada` | Ya existe conversacion para ese contexto |
| 401 | 3001 | `Auth_Unauthorized` | Token JWT invalido o expirado |
| 403 | 3002 | `Auth_Forbidden` | No es participante de la conversacion |
| 403 | 4016 | `BusinessRule_NoRelacionConDestinatario` | Sin relacion previa con el destinatario |
| 404 | 2014 | `NotFound_Conversacion` | Conversacion no encontrada por ID |
| 500 | 5000 | `Internal_UnexpectedError` | Excepcion no controlada |
| 500 | 5001 | `Internal_DatabaseError` | Error de DB / constraint violation no controlada |

---

## 11. Archivos a Crear

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs        [MODIFICAR] Agregar 3 nuevas constantes (2014, 4015, 4016)
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── CreateConversacionResultDto.cs        [CREAR]
│   │   ├── ConversacionListResponseDto.cs        [CREAR]
│   │   ├── ConversacionListItemDto.cs            [CREAR]
│   │   ├── MensajeDto.cs                        [CREAR]
│   │   ├── MensajeListResponseDto.cs             [CREAR]
│   │   ├── MarcarLeidosResponseDto.cs            [CREAR]
│   │   └── NoLeidosCountResponseDto.cs           [CREAR]
│   │
│   ├── Features/Conversaciones/
│   │   ├── Commands/
│   │   │   ├── CreateConversacionCommand.cs      [CREAR] (Command + Handler juntos)
│   │   │   ├── SendMensajeCommand.cs             [CREAR] (Command + Handler juntos)
│   │   │   └── MarcarLeidosCommand.cs            [CREAR] (Command + Handler juntos)
│   │   ├── Queries/
│   │   │   ├── GetConversacionesQuery.cs         [CREAR] (Query + Handler juntos)
│   │   │   ├── GetMensajesQuery.cs               [CREAR] (Query + Handler juntos)
│   │   │   └── GetNoLeidosCountQuery.cs          [CREAR] (Query + Handler juntos)
│   │   └── Validators/
│   │       ├── CreateConversacionCommandValidator.cs  [CREAR]
│   │       ├── GetConversacionesQueryValidator.cs     [CREAR]
│   │       ├── GetMensajesQueryValidator.cs           [CREAR]
│   │       ├── SendMensajeCommandValidator.cs         [CREAR]
│   │       ├── MarcarLeidosCommandValidator.cs        [CREAR]
│   │       └── GetNoLeidosCountQueryValidator.cs      [CREAR]
│   │
│   ├── Interfaces/Services/
│   │   ├── IConversacionCrowdsourcingService.cs  [CREAR]
│   │   └── IMensajeCrowdsourcingService.cs       [CREAR]
│   │
│   └── Mapping/
│       ├── ConversacionCrowdsourcingProfile.cs   [CREAR]
│       └── MensajeCrowdsourcingProfile.cs        [CREAR]
│
├── WePlayRises.Crowdsourcing.Infra/
│   ├── Services/
│   │   ├── ConversacionCrowdsourcingService.cs   [CREAR]
│   │   └── MensajeCrowdsourcingService.cs        [CREAR]
│   ├── Repositories/
│   │   ├── ConversacionCrowdsourcingRepository.cs [CREAR] (si se usa patron Repository)
│   │   └── MensajeCrowdsourcingRepository.cs     [CREAR] (si se usa patron Repository)
│   ├── Data/Configurations/
│   │   ├── ConversacionCrowdsourcingConfiguration.cs [VERIFICAR/CREAR si no existe]
│   │   └── MensajeCrowdsourcingConfiguration.cs     [VERIFICAR/CREAR si no existe]
│   └── Migrations/
│       └── YYYYMMDD_AddMensajeriaCrowdsourcing.cs    [CREAR si tablas no existen aun]
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        └── ConversacionesCrowdsourcingController.cs  [CREAR]
```

---

## 12. Dependencias entre Servicios

El handler de `GetConversacionesQuery` y `GetMensajesQuery` necesita resolver nombres e imagenes de perfil de usuarios (artistas y profesionales). Esto implica una dependencia cruzada con el modulo `UserAccess`:

- `IArtistaService` (de `UserAccess.Application`) para obtener nombre e imagen de artistas.
- `IPerfilProfesionalService` (de `UserAccess.Application`) para obtener nombre e imagen de profesionales.

**Pattern recomendado:** Usar `IRequestCacheService` (BuildingBlocks) para cachear la resolucion de perfil por `UserId` dentro del scope de la request. Si el mismo usuario aparece en multiples conversaciones del listado, la primera resolucion se cachea y las siguientes se sirven desde memoria.

**Alternativa:** Si la dependencia cruzada entre modulos es un problema arquitectonico, el servicio de Infra podria hacer un JOIN directo a las tablas de `Artista` y `PerfilProfesional` via el `DbContext` compartido, evitando la dependencia de interfaz entre modulos.

---

## 13. Notas Criticas de Implementacion

### Sobre la entidad ConversacionCrowdsourcing existente

La entidad en disco usa `UserIdArtista` y `UserIdProveedor` en lugar de `UserIdCreador` y `UserIdDestinatario`. Esto tiene implicaciones:

1. La verificacion de participante es: `conversacion.UserIdArtista == userId || conversacion.UserIdProveedor == userId`.
2. El campo `NombreOtraParte` en el listado se calcula identificando cual de los dos roles tiene el usuario solicitante y resolviendo el nombre del otro.
3. Si el usuario es `UserIdArtista`, la "otra parte" es el `UserIdProveedor`, y viceversa.

### Sobre el indice de unicidad de ConversacionCrowdsourcing

La configuracion EF del contrato define indices filtrados para evitar conversaciones duplicadas. Si la tabla ya existe por US-CS-04, verificar que esos indices existan. Si no existen, agregar una migracion.

### Sobre el UPDATE de FechaUltimoMensaje en SendMensaje

El `IMensajeCrowdsourcingService.CreateAsync` debe actualizar `ConversacionCrowdsourcing.FechaUltimoMensaje = DateTime.UtcNow` en la misma transaccion que crea el mensaje. Esto se implementa en el Repository o usando un `SaveChangesAsync` unico que persiste ambos cambios.

### Sobre el UPDATE en batch de MarcarLeidos

El handler invoca `IMensajeCrowdsourcingService.MarcarLeidosAsync` que debe ejecutar una sola query SQL de tipo `ExecuteUpdateAsync` (EF Core 7+) o `ExecuteSqlRawAsync` para marcar en batch sin cargar entidades en memoria. El uso de `ExecuteUpdateAsync` es el patron preferido en .NET 8 con EF Core 8.

---

## 14. Checklist

- [ ] Nuevas constantes en `ServiceResponseMessageType.cs` agregadas (2014, 4015, 4016)
- [ ] Commands implementan `IRequest<ServiceResponse<T>>`
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command/Query en el MISMO archivo (regla CQRS)
- [ ] Handlers NO inyectan DbContext (usan servicios)
- [ ] Constructores con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Validators usan constantes `ServiceResponseMessageType.X` (no strings literales)
- [ ] Validators tienen `.WithMessage()` Y `.WithErrorCode()` en cada regla
- [ ] AutoMapper profiles separados por entidad (ConversacionProfile, MensajeProfile)
- [ ] Propiedades calculadas en handler (EsPropio, NombreOtraParte, ContextoTipo, ContextoTitulo) ignoradas en AutoMapper
- [ ] `IRequestCacheService` usado para resolver nombres de perfil (evitar N+1)
- [ ] `AsNoTracking()` en todas las queries de lectura
- [ ] Try-catch con `_logger.LogError` en todos los handlers
- [ ] Validacion de participante en handlers para mensajes y marcar-leidos
- [ ] Controller extrae `UserId` del token JWT (no del body)
- [ ] `GetNoLeidos` declarado antes de `{id}/mensajes` en el controller
- [ ] Swagger con `ProducesResponseType` para todos los status codes documentados
- [ ] Servicio `CreateAsync` de mensajes actualiza `FechaUltimoMensaje` en la misma transaccion
- [ ] `MarcarLeidosAsync` usa UPDATE en batch (no carga entidades en memoria)
- [ ] `DbUpdateException` capturada en `CreateConversacionCommandHandler` como segundo mecanismo de defensa contra duplicados
