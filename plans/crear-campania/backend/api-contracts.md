# Contratos API: Crear Campaña

**Fecha:** 2026-02-12
**Modulo:** Crowdfunding
**Feature:** crear-campania (US-02)

---

## 1. Endpoints Overview

| Metodo | Ruta | Tipo | Auth | Descripcion |
|--------|------|------|------|-------------|
| POST | /api/campanias | Command | ✅ | Crear campaña en BORRADOR |
| GET | /api/campanias/{id} | Query | ❌ | Obtener detalle de campaña |
| PUT | /api/campanias/{id} | Command | ✅ | Actualizar campaña en BORRADOR |
| POST | /api/campanias/{id}/publicar | Command | ✅ | Publicar campaña (BORRADOR → PUBLICADA) |
| GET | /api/campanias | Query | ❌ | Listar campañas públicas con filtros |
| GET | /api/campanias/mis-campanias | Query | ✅ | Listar campañas del artista autenticado |

---

## 2. Request DTOs (Commands & Queries)

### 2.1 CreateCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/CreateCampaniaCommand.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Propiedad | Tipo | Requerido | Default | Validacion | Notas |
|-----------|------|-----------|---------|------------|-------|
| ArtistaId | Guid | Si | - | NotEmpty | Extraído del token JWT (sub claim) en Controller |
| ProyectoArtisticoId | Guid? | No | null | - | Opcional para MVP |
| Titulo | string | Si | - | NotEmpty, MaxLength(200) | |
| Subtitulo | string? | No | null | MaxLength(300) | |
| DescripcionCorta | string? | No | null | MaxLength(500) | |
| VideoPrincipalUrl | string? | No | null | URL válida, MaxLength(500) | |
| ImagenPrincipalUrl | string? | No | null | URL válida, MaxLength(500) | |
| MonedaId | int | Si | 1 | GreaterThan(0) | 1 = EUR |
| ImporteObjetivo | decimal | Si | - | GreaterThan(0) | |
| ImporteMinimo | decimal? | No | null | GreaterThan(0), LessThanOrEqualTo(ImporteObjetivo) | |
| TipoFinanciacionId | int | Si | - | GreaterThan(0) | 1 = Todo o Nada, 2 = Flexible |
| PermiteAportacionesAnonimas | bool | No | false | - | |
| PermitePropinas | bool | No | false | - | |
| FechaInicio | DateTime? | No | null | LessThan(FechaFin) si ambos presentes | |
| FechaFin | DateTime? | No | null | GreaterThan(FechaInicio) si ambos presentes | |

**Implementa:** `IRequest<ServiceResponse<CampaniaDto>>`

**Cambios necesarios:**
- El Command actual retorna `ServiceResponse<CampaniaDto>` ✅ (correcto)
- NO incluir `ArtistaId` en request body del frontend (extraer de token en Controller)

---

### 2.2 UpdateCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/UpdateCampaniaCommand.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Propiedad | Tipo | Requerido | Validacion | Notas |
|-----------|------|-----------|------------|-------|
| Id | Guid | Si | NotEmpty | Debe coincidir con URL param |
| ArtistaId | Guid | Si (interno) | NotEmpty | Extraído del token, NO del body |
| Titulo | string? | No | MaxLength(200) si presente | Todos los campos son opcionales |
| Subtitulo | string? | No | MaxLength(300) si presente | |
| DescripcionCorta | string? | No | MaxLength(500) si presente | |
| VideoPrincipalUrl | string? | No | URL válida si presente | |
| ImagenPrincipalUrl | string? | No | URL válida si presente | |
| ImporteObjetivo | decimal? | No | GreaterThan(0) si presente | |
| ImporteMinimo | decimal? | No | GreaterThan(0), LessThanOrEqualTo(ImporteObjetivo) si presente | |
| TipoFinanciacionId | int? | No | GreaterThan(0) si presente | |
| PermiteAportacionesAnonimas | bool? | No | - | |
| PermitePropinas | bool? | No | - | |
| FechaInicio | DateTime? | No | - | |
| FechaFin | DateTime? | No | GreaterThan(FechaInicio) si ambos presentes | |

**Implementa:** `IRequest<ServiceResponse<bool>>`

**Validaciones de negocio:**
- Solo permitir edición si `EstadoCampaniaId == 1` (BORRADOR)
- Validar ownership: `ArtistaId` del token == `ArtistaId` de la campaña
- Si estado != BORRADOR → retornar error `BusinessRule_CampaniaNotDraft` (4009)
- Si no es propietario → retornar error `Auth_Forbidden` (3002)

---

### 2.3 PublishCampaniaCommand

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Commands/PublishCampaniaCommand.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

| Propiedad | Tipo | Requerido | Validacion | Notas |
|-----------|------|-----------|------------|-------|
| Id | Guid | Si | NotEmpty | ID de la campaña a publicar |
| ArtistaId | Guid | Si (interno) | NotEmpty | Extraído del token, NO del body |

**Implementa:** `IRequest<ServiceResponse<PublishCampaniaResponse>>`

**Validaciones de negocio:**
- Solo permitir si `EstadoCampaniaId == 1` (BORRADOR)
- Validar ownership: `ArtistaId` del token == `ArtistaId` de la campaña
- Validar campos requeridos para publicación:
  - Titulo: NotEmpty
  - ImporteObjetivo: GreaterThan(0)
  - MonedaId: GreaterThan(0)
  - TipoFinanciacionId: GreaterThan(0)
  - FechaFin: NotNull AND GreaterThan(DateTime.UtcNow.AddDays(7))
- Si falta algún campo → retornar error `Validation_Required` (1001)
- Si FechaFin < UtcNow + 7 días → retornar error `Validation_InvalidDate` (1012)
- Si no es propietario → retornar error `Auth_Forbidden` (3002)
- Si estado != BORRADOR → retornar error `BusinessRule_CampaniaNotDraft` (4009)

**Lógica del Handler:**
1. Validar comando
2. Obtener campaña por Id
3. Validar ownership
4. Validar estado == BORRADOR
5. Validar campos requeridos completos
6. Actualizar:
   - EstadoCampaniaId = 2 (PUBLICADA)
   - FechaPublicacion = DateTime.UtcNow
   - Si FechaInicio es null → FechaInicio = FechaPublicacion
7. Guardar cambios
8. Retornar PublishCampaniaResponse

---

### 2.4 GetCampaniaByIdQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaByIdQuery.cs`

**Estado actual:** ✅ Existe

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Id | Guid | Si | NotEmpty |

**Implementa:** `IRequest<ServiceResponse<CampaniaDto>>`

**Notas:**
- Endpoint público (no requiere autenticación)
- Si no existe → retornar error `NotFound_Campania` (2003)

---

### 2.5 GetAllCampaniasQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetAllCampaniasQuery.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Propiedad | Tipo | Requerido | Default | Validacion | Notas |
|-----------|------|-----------|---------|------------|-------|
| SearchTerm | string? | No | null | - | Buscar en Titulo, Subtitulo, DescripcionCorta |
| ArtistaId | Guid? | No | null | - | Filtrar por artista |
| EstadoCampaniaId | int? | No | 2 (PUBLICADA) | - | Por defecto solo campañas publicadas para público |
| PageNumber | int? | No | 1 | GreaterThan(0) | |
| PageSize | int? | No | 10 | GreaterThan(0), LessThanOrEqualTo(50) | Máximo 50 |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>`

**Notas:**
- Endpoint público
- Por defecto filtrar solo `EstadoCampaniaId = 2` (PUBLICADA) si no se especifica
- Implementar paginación con skip/take

---

### 2.6 GetMisCampaniasQuery

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

| Propiedad | Tipo | Requerido | Default | Validacion | Notas |
|-----------|------|-----------|---------|------------|-------|
| ArtistaId | Guid | Si (interno) | - | NotEmpty | Extraído del token, NO del query string |
| EstadoCampaniaId | int? | No | null | - | Filtrar por estado (opcional) |
| PageNumber | int? | No | 1 | GreaterThan(0) | |
| PageSize | int? | No | 10 | GreaterThan(0), LessThanOrEqualTo(50) | |

**Implementa:** `IRequest<ServiceResponse<IEnumerable<CampaniaListDto>>>`

**Notas:**
- Requiere autenticación
- Retorna TODAS las campañas del artista (incluidos BORRADORES)
- Si `EstadoCampaniaId` se especifica, filtrar por ese estado

---

## 3. Response DTOs

### 3.1 CampaniaDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaDto.cs`

**Estado actual:** ✅ Existe (completo)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador único |
| ArtistaId | Guid | Artista propietario |
| ProyectoArtisticoId | Guid? | Proyecto asociado (opcional) |
| Titulo | string | Título de la campaña |
| Subtitulo | string? | Subtítulo (opcional) |
| DescripcionCorta | string? | Descripción breve (opcional) |
| VideoPrincipalUrl | string? | URL del video principal |
| ImagenPrincipalUrl | string? | URL de la imagen principal |
| MonedaId | int | Moneda (1 = EUR, 2 = USD) |
| ImporteObjetivo | decimal | Meta financiera |
| ImporteMinimo | decimal? | Meta mínima (opcional) |
| ImportePledgedActual | decimal | Recaudación actual |
| TipoFinanciacionId | int | Tipo (1 = Todo o Nada, 2 = Flexible) |
| EstadoCampaniaId | int | Estado (1 = BORRADOR, 2 = PUBLICADA, 3 = FINALIZADA, 4 = CANCELADA) |
| PermiteAportacionesAnonimas | bool | Permitir aportes anónimos |
| PermitePropinas | bool | Permitir propinas |
| PorcentajeComisionPlataforma | decimal? | Comisión de plataforma |
| FechaInicio | DateTime? | Fecha de inicio de la campaña |
| FechaFin | DateTime? | Fecha de fin de la campaña |
| FechaPublicacion | DateTime? | Fecha en que fue publicada |
| FechaCierre | DateTime? | Fecha de cierre definitivo |
| FechaCreacion | DateTime | Fecha de creación del registro |
| FechaActualizacion | DateTime? | Última actualización |

**Wrapped en:** `ServiceResponse<CampaniaDto>`

---

### 3.2 CampaniaListDto

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaListDto.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador único |
| ArtistaId | Guid | Artista propietario |
| Titulo | string | Título de la campaña |
| Subtitulo | string? | Subtítulo |
| DescripcionCorta | string? | Descripción breve |
| ImagenPrincipalUrl | string? | URL de la imagen |
| ImporteObjetivo | decimal | Meta financiera |
| ImportePledgedActual | decimal | Recaudación actual |
| EstadoCampaniaId | int | Estado |
| FechaInicio | DateTime? | Fecha de inicio |
| FechaFin | DateTime? | Fecha de fin |
| FechaCreacion | DateTime | Fecha de creación |

**Wrapped en:** `ServiceResponse<IEnumerable<CampaniaListDto>>`

**Notas:**
- DTO simplificado para listados
- No incluye todos los detalles (optimización)

---

### 3.3 PublishCampaniaResponse

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PublishCampaniaResponse.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la campaña publicada |
| EstadoCampaniaId | int | Estado nuevo (2 = PUBLICADA) |
| FechaPublicacion | DateTime | Fecha en que fue publicada |
| Message | string | Mensaje de confirmación |

**Wrapped en:** `ServiceResponse<PublishCampaniaResponse>`

**Ejemplo:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "estadoCampaniaId": 2,
    "fechaPublicacion": "2026-02-12T14:00:00Z",
    "message": "Campaña publicada exitosamente"
  },
  "messages": [
    {
      "message": "Campaña publicada exitosamente",
      "errorCode": "0000"
    }
  ]
}
```

---

## 4. Validadores FluentValidation

### 4.1 CreateCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/CreateCampaniaCommandValidator.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Campo | Regla | Mensaje | ErrorCode | Condición |
|-------|-------|---------|-----------|-----------|
| ArtistaId | NotEmpty | El ArtistaId es obligatorio | Validation_Required | Siempre |
| Titulo | NotEmpty | El titulo es obligatorio | Validation_Required | Siempre |
| Titulo | MaxLength(200) | El titulo no puede superar los 200 caracteres | Validation_MaxLength | Siempre |
| Subtitulo | MaxLength(300) | El subtitulo no puede superar los 300 caracteres | Validation_MaxLength | Si no es null/empty |
| DescripcionCorta | MaxLength(500) | La descripcion corta no puede superar los 500 caracteres | Validation_MaxLength | Si no es null/empty |
| VideoPrincipalUrl | MaxLength(500) | La URL del video no puede superar los 500 caracteres | Validation_MaxLength | Si no es null/empty |
| VideoPrincipalUrl | Must(BeValidUrl) | La URL del video no es valida | Validation_InvalidUrl | Si no es null/empty |
| ImagenPrincipalUrl | MaxLength(500) | La URL de la imagen no puede superar los 500 caracteres | Validation_MaxLength | Si no es null/empty |
| ImagenPrincipalUrl | Must(BeValidUrl) | La URL de la imagen no es valida | Validation_InvalidUrl | Si no es null/empty |
| MonedaId | GreaterThan(0) | La moneda es obligatoria | Validation_Required | Siempre |
| ImporteObjetivo | GreaterThan(0) | El importe objetivo debe ser mayor a 0 | Validation_InvalidAmount | Siempre |
| ImporteMinimo | GreaterThan(0) | El importe minimo debe ser mayor a 0 | Validation_InvalidAmount | Si tiene valor |
| ImporteMinimo | LessThanOrEqualTo(ImporteObjetivo) | El importe minimo no puede ser mayor al importe objetivo | Validation_InvalidRange | Si tiene valor |
| TipoFinanciacionId | GreaterThan(0) | El tipo de financiacion es obligatorio | Validation_Required | Siempre |
| FechaFin | GreaterThan(FechaInicio) | La fecha de fin debe ser posterior a la fecha de inicio | Validation_InvalidDate | Si ambos tienen valor |

**Método auxiliar:**
```csharp
private static bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url))
        return true;

    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Cambios necesarios:**
- ✅ Ya usa `ServiceResponseMessageType` constants (correcto)
- ✅ Ya incluye `.WithMessage()` y `.WithErrorCode()` (correcto)

---

### 4.2 UpdateCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/UpdateCampaniaCommandValidator.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

| Campo | Regla | Mensaje | ErrorCode | Condición |
|-------|-------|---------|-----------|-----------|
| Id | NotEmpty | El Id es obligatorio | Validation_Required | Siempre |
| Titulo | MaxLength(200) | El titulo no puede superar los 200 caracteres | Validation_MaxLength | Si presente |
| Subtitulo | MaxLength(300) | El subtitulo no puede superar los 300 caracteres | Validation_MaxLength | Si presente |
| DescripcionCorta | MaxLength(500) | La descripcion corta no puede superar los 500 caracteres | Validation_MaxLength | Si presente |
| VideoPrincipalUrl | Must(BeValidUrl) | La URL del video no es valida | Validation_InvalidUrl | Si presente |
| ImagenPrincipalUrl | Must(BeValidUrl) | La URL de la imagen no es valida | Validation_InvalidUrl | Si presente |
| ImporteObjetivo | GreaterThan(0) | El importe objetivo debe ser mayor a 0 | Validation_InvalidAmount | Si presente |
| ImporteMinimo | GreaterThan(0) | El importe minimo debe ser mayor a 0 | Validation_InvalidAmount | Si presente |
| ImporteMinimo | LessThanOrEqualTo(ImporteObjetivo) | El importe minimo no puede ser mayor al importe objetivo | Validation_InvalidRange | Si presente |
| TipoFinanciacionId | GreaterThan(0) | El tipo de financiacion debe ser valido | Validation_Required | Si presente |
| FechaFin | GreaterThan(FechaInicio) | La fecha de fin debe ser posterior a la fecha de inicio | Validation_InvalidDate | Si ambos presentes |

**Validaciones de negocio en Handler:**
- Estado debe ser BORRADOR (validar en Handler antes de actualizar)
- Ownership (validar en Handler antes de actualizar)

---

### 4.3 PublishCampaniaCommandValidator

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/PublishCampaniaCommandValidator.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

| Campo | Regla | Mensaje | ErrorCode | Condición |
|-------|-------|---------|-----------|-----------|
| Id | NotEmpty | El Id es obligatorio | Validation_Required | Siempre |
| ArtistaId | NotEmpty | El ArtistaId es obligatorio | Validation_Required | Siempre |

**Validaciones de negocio en Handler:**
- Obtener campaña por Id
- Validar ownership: `campaign.ArtistaId == command.ArtistaId`
- Validar estado == BORRADOR
- Validar campos requeridos para publicación:
  - `string.IsNullOrEmpty(campaign.Titulo)` → error `Validation_Required`
  - `campaign.ImporteObjetivo <= 0` → error `Validation_InvalidAmount`
  - `campaign.MonedaId <= 0` → error `Validation_Required`
  - `campaign.TipoFinanciacionId <= 0` → error `Validation_Required`
  - `campaign.FechaFin == null || campaign.FechaFin < DateTime.UtcNow.AddDays(7)` → error `Validation_InvalidDate`

**Mensajes de error específicos:**
```csharp
if (string.IsNullOrEmpty(campaign.Titulo))
{
    return new ServiceResponse<PublishCampaniaResponse>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "El titulo es obligatorio para publicar",
                ErrorCode = ServiceResponseMessageType.Validation_Required
            }
        }
    };
}

if (campaign.FechaFin == null || campaign.FechaFin < DateTime.UtcNow.AddDays(7))
{
    return new ServiceResponse<PublishCampaniaResponse>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "La campania debe durar minimo 7 dias desde hoy",
                ErrorCode = ServiceResponseMessageType.Validation_InvalidDate
            }
        }
    };
}
```

---

## 5. AutoMapper Mappings

### 5.1 CampaniaProfile

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CampaniaProfile.cs`

**Estado actual:** ✅ Existe (completo)

| Source | Destination | Notas | Estado |
|--------|-------------|-------|--------|
| CampaniaCrowdfunding | CampaniaDto | Mapping completo con StronglyTypedIds | ✅ Existe |
| CampaniaCrowdfunding | CampaniaListDto | DTO simplificado para listados | ✅ Existe |
| CreateCampaniaCommand | CampaniaCrowdfunding | Para crear entidad desde Command | ✅ Existe |
| UpdateCampaniaCommand | CampaniaCrowdfunding | Para actualizar entidad desde Command | ⚠️ Verificar |

**Transformaciones especiales:**
```csharp
// Entity -> DTO (conversión de StronglyTypedIds a Guid)
CreateMap<CampaniaCrowdfunding, CampaniaDto>()
    .ForMember(dest => dest.Id,
               opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.ArtistaId,
               opt => opt.MapFrom(src => src.ArtistaId.Value))
    .ForMember(dest => dest.ProyectoArtisticoId,
               opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue
                   ? src.ProyectoArtisticoId.Value.Value
                   : (Guid?)null));

// Command -> Entity (conversión de Guid a StronglyTypedIds)
CreateMap<CreateCampaniaCommand, CampaniaCrowdfunding>()
    .ForMember(dest => dest.ArtistaId,
               opt => opt.MapFrom(src => new ArtistaId(src.ArtistaId)))
    .ForMember(dest => dest.ProyectoArtisticoId,
               opt => opt.MapFrom(src => src.ProyectoArtisticoId.HasValue
                   ? new ProyectoArtisticoId(src.ProyectoArtisticoId.Value)
                   : (ProyectoArtisticoId?)null))
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.EstadoCampaniaId, opt => opt.Ignore())
    .ForMember(dest => dest.ImportePledgedActual, opt => opt.Ignore())
    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
    .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore())
    .ForMember(dest => dest.Rewards, opt => opt.Ignore())
    .ForMember(dest => dest.Pedidos, opt => opt.Ignore());
```

**Notas:**
- ✅ Profile actual está completo y sigue reglas CQRS
- ⚠️ Verificar que `UpdateCampaniaCommand` tenga mapping (si no, agregarlo)

---

## 6. Controller Actions

### 6.1 POST /api/campanias

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

```csharp
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Create(
    [FromBody] CreateCampaniaRequest request,
    CancellationToken cancellationToken)
{
    // Extraer ArtistaId del token JWT
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // "sub" claim
    if (!Guid.TryParse(userIdClaim, out var artistaId))
    {
        return Unauthorized(new ServiceResponse<CampaniaDto>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Token invalido",
                    ErrorCode = ServiceResponseMessageType.Auth_InvalidToken
                }
            }
        });
    }

    // Mapear request DTO a Command (sin ArtistaId en body)
    var command = new CreateCampaniaCommand
    {
        ArtistaId = artistaId, // Inyectar desde token
        ProyectoArtisticoId = request.ProyectoArtisticoId,
        Titulo = request.Titulo,
        Subtitulo = request.Subtitulo,
        DescripcionCorta = request.DescripcionCorta,
        VideoPrincipalUrl = request.VideoPrincipalUrl,
        ImagenPrincipalUrl = request.ImagenPrincipalUrl,
        MonedaId = request.MonedaId,
        ImporteObjetivo = request.ImporteObjetivo,
        ImporteMinimo = request.ImporteMinimo,
        TipoFinanciacionId = request.TipoFinanciacionId,
        PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas,
        PermitePropinas = request.PermitePropinas,
        FechaInicio = request.FechaInicio,
        FechaFin = request.FechaFin
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
        var firstError = result.Messages.FirstOrDefault();
        return firstError?.ErrorCode switch
        {
            var code when code?.StartsWith("1") == true => BadRequest(result), // Validation errors
            var code when code?.StartsWith("3") == true => Unauthorized(result), // Auth errors
            _ => StatusCode(500, result)
        };
    }

    return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
}
```

**Request Body (CreateCampaniaRequest DTO):**
```csharp
// NO incluir ArtistaId en body (se extrae del token)
public class CreateCampaniaRequest
{
    public Guid? ProyectoArtisticoId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public int MonedaId { get; set; } = 1; // Default EUR
    public decimal ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int TipoFinanciacionId { get; set; }
    public bool PermiteAportacionesAnonimas { get; set; }
    public bool PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

**Cambios necesarios:**
- Crear `CreateCampaniaRequest` DTO separado (sin `ArtistaId`)
- Extraer `ArtistaId` del token en Controller
- Mapear request DTO a Command
- Retornar `201 Created` con header `Location`

---

### 6.2 GET /api/campanias/{id}

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ✅ Existe (correcto)

```csharp
[HttpGet("{id:guid}")]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<CampaniaDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
{
    var query = new GetCampaniaByIdQuery { Id = id };
    var result = await _mediator.Send(query, cancellationToken);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            ServiceResponseMessageType.NotFound_Campania => NotFound(result),
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Notas:**
- ✅ Endpoint público correcto
- ✅ Manejo de errores correcto

---

### 6.3 PUT /api/campanias/{id}

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

```csharp
[HttpPut("{id:guid}")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateCampaniaRequest request,
    CancellationToken cancellationToken)
{
    // Validar que ID de URL coincide con body
    if (request.Id != id)
    {
        return BadRequest(new ServiceResponse<bool>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "El Id en URL no coincide con el Id en body",
                    ErrorCode = ServiceResponseMessageType.Validation_InvalidFormat
                }
            }
        });
    }

    // Extraer ArtistaId del token
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var artistaId))
    {
        return Unauthorized(new ServiceResponse<bool>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Token invalido",
                    ErrorCode = ServiceResponseMessageType.Auth_InvalidToken
                }
            }
        });
    }

    // Mapear request DTO a Command
    var command = new UpdateCampaniaCommand
    {
        Id = request.Id,
        ArtistaId = artistaId, // Inyectar desde token
        Titulo = request.Titulo,
        Subtitulo = request.Subtitulo,
        DescripcionCorta = request.DescripcionCorta,
        VideoPrincipalUrl = request.VideoPrincipalUrl,
        ImagenPrincipalUrl = request.ImagenPrincipalUrl,
        ImporteObjetivo = request.ImporteObjetivo,
        ImporteMinimo = request.ImporteMinimo,
        TipoFinanciacionId = request.TipoFinanciacionId,
        PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas,
        PermitePropinas = request.PermitePropinas,
        FechaInicio = request.FechaInicio,
        FechaFin = request.FechaFin
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            ServiceResponseMessageType.NotFound_Campania => NotFound(result),
            ServiceResponseMessageType.Auth_Forbidden => Forbid(),
            ServiceResponseMessageType.BusinessRule_CampaniaNotDraft => Conflict(result),
            var code when code?.StartsWith("1") == true => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Request Body (UpdateCampaniaRequest DTO):**
```csharp
public class UpdateCampaniaRequest
{
    public Guid Id { get; set; } // Requerido
    // Todos los demás campos opcionales
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal? ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoFinanciacionId { get; set; }
    public bool? PermiteAportacionesAnonimas { get; set; }
    public bool? PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
```

**Cambios necesarios:**
- Crear `UpdateCampaniaRequest` DTO separado (sin `ArtistaId`)
- Extraer `ArtistaId` del token en Controller
- Agregar manejo de error 409 para estado != BORRADOR

---

### 6.4 POST /api/campanias/{id}/publicar

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

```csharp
[HttpPost("{id:guid}/publicar")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<PublishCampaniaResponse>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Publicar(
    Guid id,
    CancellationToken cancellationToken)
{
    // Extraer ArtistaId del token
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var artistaId))
    {
        return Unauthorized(new ServiceResponse<PublishCampaniaResponse>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Token invalido",
                    ErrorCode = ServiceResponseMessageType.Auth_InvalidToken
                }
            }
        });
    }

    var command = new PublishCampaniaCommand
    {
        Id = id,
        ArtistaId = artistaId
    };

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            ServiceResponseMessageType.NotFound_Campania => NotFound(result),
            ServiceResponseMessageType.Auth_Forbidden => Forbid(),
            ServiceResponseMessageType.BusinessRule_CampaniaNotDraft => Conflict(result),
            var code when code?.StartsWith("1") == true => BadRequest(result),
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Notas:**
- No requiere body (solo ID en URL)
- Extrae `ArtistaId` del token
- Retorna `PublishCampaniaResponse` con datos de la campaña publicada

---

### 6.5 GET /api/campanias

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ✅ Existe (requiere refinamiento)

```csharp
[HttpGet]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetAll(
    [FromQuery] string? searchTerm,
    [FromQuery] Guid? artistaId,
    [FromQuery] int? estadoCampaniaId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    // Si no se especifica estado, filtrar solo PUBLICADAS (2) para público
    var estadoFiltro = estadoCampaniaId ?? 2;

    var query = new GetAllCampaniasQuery
    {
        SearchTerm = searchTerm,
        ArtistaId = artistaId,
        EstadoCampaniaId = estadoFiltro,
        PageNumber = pageNumber,
        PageSize = Math.Min(pageSize, 50) // Max 50
    };

    var result = await _mediator.Send(query, cancellationToken);

    if (!result.IsSuccess)
    {
        return StatusCode(500, result);
    }

    return Ok(result);
}
```

**Query Parameters:**
- `searchTerm` (string, opcional): Buscar en Titulo, Subtitulo, DescripcionCorta
- `artistaId` (Guid, opcional): Filtrar por artista
- `estadoCampaniaId` (int, opcional): Filtrar por estado (default 2 = PUBLICADA)
- `pageNumber` (int, opcional): Número de página (default 1)
- `pageSize` (int, opcional): Tamaño de página (default 10, max 50)

**Cambios necesarios:**
- Establecer default `estadoCampaniaId = 2` si no se especifica
- Limitar `pageSize` a máximo 50

---

### 6.6 GET /api/campanias/mis-campanias

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs`

**Estado actual:** ❌ NO existe (crear nuevo)

```csharp
[HttpGet("mis-campanias")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<IEnumerable<CampaniaListDto>>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMisCampanias(
    [FromQuery] int? estadoCampaniaId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    // Extraer ArtistaId del token
    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdClaim, out var artistaId))
    {
        return Unauthorized(new ServiceResponse<IEnumerable<CampaniaListDto>>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Token invalido",
                    ErrorCode = ServiceResponseMessageType.Auth_InvalidToken
                }
            }
        });
    }

    var query = new GetMisCampaniasQuery
    {
        ArtistaId = artistaId,
        EstadoCampaniaId = estadoCampaniaId,
        PageNumber = pageNumber,
        PageSize = Math.Min(pageSize, 50)
    };

    var result = await _mediator.Send(query, cancellationToken);

    if (!result.IsSuccess)
    {
        return StatusCode(500, result);
    }

    return Ok(result);
}
```

**Query Parameters:**
- `estadoCampaniaId` (int, opcional): Filtrar por estado
- `pageNumber` (int, opcional): Número de página (default 1)
- `pageSize` (int, opcional): Tamaño de página (default 10, max 50)

**Notas:**
- Retorna TODAS las campañas del artista (incluidos BORRADORES)
- Extrae `ArtistaId` del token (no del query string)

---

## 7. Swagger/OpenAPI Documentation

### 7.1 POST /api/campanias

**Summary:** Crear nueva campaña de crowdfunding en estado BORRADOR

**Tags:** Campanias

**Security:** Bearer JWT

**Request Body:**
- Content-Type: `application/json`
- Schema: `CreateCampaniaRequest`

**Responses:**
- **201 Created**
  - Description: Campaña creada exitosamente
  - Content: `ServiceResponse<CampaniaDto>`
  - Headers: `Location: /api/campanias/{id}`
- **400 Bad Request**
  - Description: Errores de validación
  - Content: `ServiceResponse<CampaniaDto>` con errores en `messages`
- **401 Unauthorized**
  - Description: Token no válido o expirado
- **500 Internal Server Error**
  - Description: Error inesperado en el servidor

**Example Request:**
```json
{
  "titulo": "Mi primer album",
  "subtitulo": "Rock alternativo con influencias indie",
  "descripcionCorta": "Necesitamos tu apoyo para grabar nuestro primer disco...",
  "videoPrincipalUrl": "https://youtube.com/watch?v=xyz",
  "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
  "monedaId": 1,
  "importeObjetivo": 5000.00,
  "importeMinimo": 100.00,
  "tipoFinanciacionId": 1,
  "permiteAportacionesAnonimas": false,
  "permitePropinas": true,
  "fechaInicio": "2026-03-01T00:00:00Z",
  "fechaFin": "2026-04-30T23:59:59Z"
}
```

---

### 7.2 GET /api/campanias/{id}

**Summary:** Obtener detalle de una campaña por ID

**Tags:** Campanias

**Security:** None (público)

**Parameters:**
- `id` (path, required, Guid): ID de la campaña

**Responses:**
- **200 OK**
  - Description: Campaña encontrada
  - Content: `ServiceResponse<CampaniaDto>`
- **404 Not Found**
  - Description: Campaña no encontrada
  - Content: `ServiceResponse<CampaniaDto>` con error 2003
- **500 Internal Server Error**

---

### 7.3 PUT /api/campanias/{id}

**Summary:** Actualizar campaña en estado BORRADOR

**Tags:** Campanias

**Security:** Bearer JWT (solo propietario)

**Parameters:**
- `id` (path, required, Guid): ID de la campaña

**Request Body:**
- Content-Type: `application/json`
- Schema: `UpdateCampaniaRequest`

**Responses:**
- **200 OK**
  - Description: Campaña actualizada correctamente
  - Content: `ServiceResponse<bool>`
- **400 Bad Request**
  - Description: Errores de validación
- **401 Unauthorized**
  - Description: Token no válido
- **403 Forbidden**
  - Description: No eres el propietario de la campaña
- **404 Not Found**
  - Description: Campaña no encontrada
- **409 Conflict**
  - Description: Solo se pueden editar campañas en estado BORRADOR

---

### 7.4 POST /api/campanias/{id}/publicar

**Summary:** Publicar campaña (cambiar de BORRADOR a PUBLICADA)

**Tags:** Campanias

**Security:** Bearer JWT (solo propietario)

**Parameters:**
- `id` (path, required, Guid): ID de la campaña a publicar

**Responses:**
- **200 OK**
  - Description: Campaña publicada exitosamente
  - Content: `ServiceResponse<PublishCampaniaResponse>`
- **400 Bad Request**
  - Description: Faltan campos requeridos o fechas inválidas
- **401 Unauthorized**
- **403 Forbidden**
  - Description: No eres el propietario
- **404 Not Found**
- **409 Conflict**
  - Description: Solo se pueden publicar campañas en estado BORRADOR

---

### 7.5 GET /api/campanias

**Summary:** Listar campañas públicas con filtros y paginación

**Tags:** Campanias

**Security:** None (público)

**Parameters:**
- `searchTerm` (query, optional, string): Buscar en título, subtítulo, descripción
- `artistaId` (query, optional, Guid): Filtrar por artista
- `estadoCampaniaId` (query, optional, int): Filtrar por estado (default: 2 = PUBLICADA)
- `pageNumber` (query, optional, int): Número de página (default: 1)
- `pageSize` (query, optional, int): Tamaño de página (default: 10, max: 50)

**Responses:**
- **200 OK**
  - Description: Lista de campañas
  - Content: `ServiceResponse<IEnumerable<CampaniaListDto>>`
- **500 Internal Server Error**

---

### 7.6 GET /api/campanias/mis-campanias

**Summary:** Listar campañas del artista autenticado (incluye BORRADORES)

**Tags:** Campanias

**Security:** Bearer JWT

**Parameters:**
- `estadoCampaniaId` (query, optional, int): Filtrar por estado
- `pageNumber` (query, optional, int): Número de página (default: 1)
- `pageSize` (query, optional, int): Tamaño de página (default: 10, max: 50)

**Responses:**
- **200 OK**
  - Description: Lista de campañas del artista
  - Content: `ServiceResponse<IEnumerable<CampaniaListDto>>`
- **401 Unauthorized**
- **500 Internal Server Error**

---

## 8. Archivos a Crear/Modificar

### 8.1 Crear Nuevos

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/Campanias/
│   ├── Commands/
│   │   └── PublishCampaniaCommand.cs                     # NUEVO - Command + Handler
│   ├── Queries/
│   │   └── GetMisCampaniasQuery.cs                       # NUEVO - Query + Handler
│   └── Validators/
│       └── PublishCampaniaCommandValidator.cs            # NUEVO
└── Dtos/
    ├── CreateCampaniaRequest.cs                          # NUEVO - DTO para request (sin ArtistaId)
    ├── UpdateCampaniaRequest.cs                          # NUEVO - DTO para request (sin ArtistaId)
    └── PublishCampaniaResponse.cs                        # NUEVO - Response específico
```

### 8.2 Modificar Existentes

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/Campanias/
│   ├── Commands/
│   │   ├── CreateCampaniaCommand.cs                      # REFINAMIENTO - Ajustar Handler
│   │   └── UpdateCampaniaCommand.cs                      # REFINAMIENTO - Agregar validación ownership
│   └── Validators/
│       ├── CreateCampaniaCommandValidator.cs             # VERIFICAR - Ya está completo
│       └── UpdateCampaniaCommandValidator.cs             # VERIFICAR - Ya está completo
└── Mapping/
    └── CampaniaProfile.cs                                # VERIFICAR - Agregar mapping UpdateCommand si falta

Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/
└── Controllers/
    └── CampaniasController.cs                            # REFINAMIENTO
        - Modificar POST: Extraer ArtistaId del token, mapear CreateCampaniaRequest → Command
        - Modificar PUT: Extraer ArtistaId del token, mapear UpdateCampaniaRequest → Command
        - Modificar GET: Establecer default estadoCampaniaId = 2
        - Agregar POST /publicar: Nuevo endpoint
        - Agregar GET /mis-campanias: Nuevo endpoint
```

---

## 9. ServiceResponseMessageType Constants

**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`

**Estado actual:** ✅ Existe y está completo

### Constantes Utilizadas en Esta Feature

| Constante | Valor | Categoría | Uso |
|-----------|-------|-----------|-----|
| Success | "0000" | Success | Operación exitosa genérica |
| Created | "0001" | Success | Campaña creada |
| Updated | "0002" | Success | Campaña actualizada |
| Validation_Required | "1001" | Validation | Campo obligatorio faltante |
| Validation_MaxLength | "1002" | Validation | Excede longitud máxima |
| Validation_InvalidUrl | "1006" | Validation | URL con formato inválido |
| Validation_InvalidRange | "1007" | Validation | ImporteMinimo > ImporteObjetivo |
| Validation_InvalidAmount | "1011" | Validation | Importe <= 0 |
| Validation_InvalidDate | "1012" | Validation | FechaFin <= FechaInicio o < 7 días |
| NotFound_Campania | "2003" | NotFound | Campaña no encontrada |
| Auth_Unauthorized | "3001" | Auth | Token inválido/expirado |
| Auth_Forbidden | "3002" | Auth | No eres propietario |
| Auth_InvalidToken | "3004" | Auth | Token con formato inválido |
| BusinessRule_CampaniaNotDraft | "4009" | Business | Solo BORRADOR puede editarse/publicarse |
| Internal_UnexpectedError | "5000" | Internal | Error inesperado |

---

## 10. Business Rules en Handlers

### 10.1 CreateCampaniaCommandHandler

**Reglas:**
1. Validar Command con Validator
2. Mapear Command → Entity
3. Establecer valores automáticos:
   - `Id = CampaniaCrowdfundingId.CreateNew()`
   - `EstadoCampaniaId = 1` (BORRADOR)
   - `ImportePledgedActual = 0`
   - `FechaCreacion = DateTime.UtcNow`
4. Llamar a `ICampaniaService.CreateAsync(entity, ct)`
5. Obtener entidad creada con `GetByIdAsync`
6. Mapear Entity → DTO
7. Retornar `ServiceResponse<CampaniaDto>` con mensaje `Created`

---

### 10.2 UpdateCampaniaCommandHandler

**Reglas:**
1. Validar Command con Validator
2. Obtener campaña existente por `Id`
3. Si no existe → retornar error `NotFound_Campania` (2003)
4. Validar ownership: `campaign.ArtistaId == command.ArtistaId`
   - Si no coincide → retornar error `Auth_Forbidden` (3002)
5. Validar estado: `campaign.EstadoCampaniaId == 1` (BORRADOR)
   - Si no es BORRADOR → retornar error `BusinessRule_CampaniaNotDraft` (4009)
6. Actualizar solo campos presentes en Command (patch parcial)
7. Establecer `FechaActualizacion = DateTime.UtcNow`
8. Llamar a `ICampaniaService.UpdateAsync(entity, ct)`
9. Retornar `ServiceResponse<bool>` con mensaje `Updated`

---

### 10.3 PublishCampaniaCommandHandler

**Reglas:**
1. Validar Command con Validator (Id, ArtistaId)
2. Obtener campaña por `Id`
3. Si no existe → retornar error `NotFound_Campania` (2003)
4. Validar ownership: `campaign.ArtistaId == command.ArtistaId`
   - Si no coincide → retornar error `Auth_Forbidden` (3002)
5. Validar estado: `campaign.EstadoCampaniaId == 1` (BORRADOR)
   - Si no es BORRADOR → retornar error `BusinessRule_CampaniaNotDraft` (4009)
6. Validar campos requeridos para publicación:
   - `string.IsNullOrEmpty(campaign.Titulo)` → error `Validation_Required` (1001)
   - `campaign.ImporteObjetivo <= 0` → error `Validation_InvalidAmount` (1011)
   - `campaign.MonedaId <= 0` → error `Validation_Required` (1001)
   - `campaign.TipoFinanciacionId <= 0` → error `Validation_Required` (1001)
   - `campaign.FechaFin == null || campaign.FechaFin < DateTime.UtcNow.AddDays(7)` → error `Validation_InvalidDate` (1012)
7. Si todas las validaciones pasan:
   - `campaign.EstadoCampaniaId = 2` (PUBLICADA)
   - `campaign.FechaPublicacion = DateTime.UtcNow`
   - Si `campaign.FechaInicio == null` → `campaign.FechaInicio = campaign.FechaPublicacion`
8. Llamar a `ICampaniaService.UpdateAsync(campaign, ct)`
9. Crear y retornar `PublishCampaniaResponse`:
   - `Id = campaign.Id`
   - `EstadoCampaniaId = 2`
   - `FechaPublicacion = campaign.FechaPublicacion`
   - `Message = "Campaña publicada exitosamente"`
10. Retornar `ServiceResponse<PublishCampaniaResponse>` con mensaje `Success`

---

### 10.4 GetCampaniaByIdQueryHandler

**Reglas:**
1. Validar Query (Id NotEmpty)
2. Llamar a `ICampaniaService.GetByIdAsync(id, ct)`
3. Si no existe → retornar error `NotFound_Campania` (2003)
4. Mapear Entity → DTO
5. Retornar `ServiceResponse<CampaniaDto>` con mensaje `Success`

---

### 10.5 GetAllCampaniasQueryHandler

**Reglas:**
1. Validar Query (PageNumber > 0, PageSize entre 1-50)
2. Llamar a `ICampaniaService.GetAllAsync(query, ct)`
3. Aplicar filtros:
   - SearchTerm: buscar en `Titulo LIKE`, `Subtitulo LIKE`, `DescripcionCorta LIKE`
   - ArtistaId: filtrar por `ArtistaId`
   - EstadoCampaniaId: filtrar por `EstadoCampaniaId` (default 2 si no se especifica)
4. Aplicar paginación: `.Skip((PageNumber - 1) * PageSize).Take(PageSize)`
5. Mapear List<Entity> → List<CampaniaListDto>
6. Retornar `ServiceResponse<IEnumerable<CampaniaListDto>>` con mensaje `Success`

---

### 10.6 GetMisCampaniasQueryHandler

**Reglas:**
1. Validar Query (ArtistaId NotEmpty, PageNumber > 0, PageSize entre 1-50)
2. Llamar a `ICampaniaService.GetByArtistaIdAsync(artistaId, query, ct)`
3. Aplicar filtros:
   - ArtistaId: filtrar por `ArtistaId == query.ArtistaId`
   - EstadoCampaniaId: filtrar por `EstadoCampaniaId` si se especifica (NO aplicar default)
4. Aplicar paginación: `.Skip((PageNumber - 1) * PageSize).Take(PageSize)`
5. Mapear List<Entity> → List<CampaniaListDto>
6. Retornar `ServiceResponse<IEnumerable<CampaniaListDto>>` con mensaje `Success`

**Nota:** Este endpoint retorna TODAS las campañas del artista, incluidos BORRADORES.

---

## 11. Checklist de Implementación

### DTOs y Commands

- [ ] Crear `CreateCampaniaRequest` DTO (sin ArtistaId)
- [ ] Crear `UpdateCampaniaRequest` DTO (sin ArtistaId)
- [ ] Crear `PublishCampaniaResponse` DTO
- [ ] Crear `PublishCampaniaCommand` + Handler
- [ ] Crear `GetMisCampaniasQuery` + Handler
- [ ] Verificar que `CreateCampaniaCommand` implementa `IRequest<ServiceResponse<CampaniaDto>>`
- [ ] Verificar que `UpdateCampaniaCommand` implementa `IRequest<ServiceResponse<bool>>`

### Validators

- [ ] Verificar `CreateCampaniaCommandValidator` (ya existe, revisar que esté completo)
- [ ] Verificar `UpdateCampaniaCommandValidator` (ya existe, revisar que esté completo)
- [ ] Crear `PublishCampaniaCommandValidator`
- [ ] Todos los validators usan `ServiceResponseMessageType` constants
- [ ] Todos los validators tienen `.WithMessage()` Y `.WithErrorCode()`

### Handlers

- [ ] `CreateCampaniaCommandHandler`: Handler + Command en MISMO archivo
- [ ] `UpdateCampaniaCommandHandler`: Validar ownership y estado BORRADOR
- [ ] `PublishCampaniaCommandHandler`: Validar campos requeridos y cambiar estado
- [ ] `GetCampaniaByIdQueryHandler`: Retornar error 2003 si no existe
- [ ] `GetAllCampaniasQueryHandler`: Default estadoCampaniaId = 2 (PUBLICADA)
- [ ] `GetMisCampaniasQueryHandler`: Filtrar por ArtistaId del token
- [ ] Todos los Handlers con `?? throw new ArgumentNullException` en constructor
- [ ] Todos los Handlers con try-catch y logging
- [ ] Todos los Handlers retornan `ServiceResponse<T>`

### AutoMapper

- [ ] Verificar `CampaniaProfile` tiene mapping `CreateCampaniaCommand → CampaniaCrowdfunding`
- [ ] Verificar mapping `UpdateCampaniaCommand → CampaniaCrowdfunding` (agregar si falta)
- [ ] Verificar mapping `CampaniaCrowdfunding → CampaniaDto`
- [ ] Verificar mapping `CampaniaCrowdfunding → CampaniaListDto`

### Controller

- [ ] POST /api/campanias: Extraer ArtistaId del token, mapear Request → Command
- [ ] GET /api/campanias/{id}: Verificar que está correcto (ya existe)
- [ ] PUT /api/campanias/{id}: Extraer ArtistaId del token, mapear Request → Command
- [ ] POST /api/campanias/{id}/publicar: Crear nuevo endpoint
- [ ] GET /api/campanias: Establecer default estadoCampaniaId = 2
- [ ] GET /api/campanias/mis-campanias: Crear nuevo endpoint
- [ ] Todos los endpoints con atributos `[Authorize]` o `[AllowAnonymous]` correctos
- [ ] Todos los endpoints con `[ProducesResponseType]` para Swagger

### Swagger Documentation

- [ ] POST /api/campanias: Summary, Request/Response schemas, Auth
- [ ] GET /api/campanias/{id}: Summary, Parameters, Response schemas
- [ ] PUT /api/campanias/{id}: Summary, Request/Response schemas, Auth
- [ ] POST /api/campanias/{id}/publicar: Summary, Response schemas, Auth
- [ ] GET /api/campanias: Summary, Query params, Response schemas
- [ ] GET /api/campanias/mis-campanias: Summary, Query params, Response schemas, Auth

---

## 12. Dependencias y Orden de Implementación

### Fase 1: DTOs y Constants (sin lógica)

1. Crear `CreateCampaniaRequest.cs`
2. Crear `UpdateCampaniaRequest.cs`
3. Crear `PublishCampaniaResponse.cs`
4. Verificar `ServiceResponseMessageType.cs` (ya existe)

### Fase 2: Commands y Queries

1. Refinar `CreateCampaniaCommand.cs` (Handler con lógica completa)
2. Refinar `UpdateCampaniaCommand.cs` (agregar validación ownership y estado)
3. Crear `PublishCampaniaCommand.cs` (Command + Handler completo)
4. Crear `GetMisCampaniasQuery.cs` (Query + Handler completo)

### Fase 3: Validators

1. Verificar `CreateCampaniaCommandValidator.cs`
2. Verificar `UpdateCampaniaCommandValidator.cs`
3. Crear `PublishCampaniaCommandValidator.cs`

### Fase 4: AutoMapper

1. Verificar `CampaniaProfile.cs` (agregar mappings faltantes)

### Fase 5: Controller

1. Refinar `CampaniasController.cs`:
   - POST /api/campanias
   - PUT /api/campanias/{id}
   - POST /api/campanias/{id}/publicar (NUEVO)
   - GET /api/campanias (ajustar default)
   - GET /api/campanias/mis-campanias (NUEVO)

### Fase 6: Testing

1. Unit tests para Validators
2. Unit tests para Handlers
3. Integration tests para Controller endpoints

---

## 13. Notas Técnicas Adicionales

### Extracción de ArtistaId del Token JWT

En Controller:
```csharp
var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // "sub" claim
if (!Guid.TryParse(userIdClaim, out var artistaId))
{
    return Unauthorized("Token invalido");
}
```

### Validación de Ownership

En Handler:
```csharp
var campaign = await _service.GetByIdAsync(command.Id, ct);
if (campaign.ArtistaId.Value != command.ArtistaId)
{
    return new ServiceResponse<bool>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "No tienes permiso para modificar esta campania",
                ErrorCode = ServiceResponseMessageType.Auth_Forbidden
            }
        }
    };
}
```

### Validación de Estado BORRADOR

En Handler:
```csharp
if (campaign.EstadoCampaniaId != 1) // 1 = BORRADOR
{
    return new ServiceResponse<bool>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new()
            {
                Message = "Solo se pueden editar campanias en estado borrador",
                ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
            }
        }
    };
}
```

### Validación de Campos Requeridos para Publicación

En PublishCampaniaCommandHandler:
```csharp
var errors = new List<ServiceResponseMessage>();

if (string.IsNullOrEmpty(campaign.Titulo))
{
    errors.Add(new ServiceResponseMessage
    {
        Message = "El titulo es obligatorio para publicar",
        ErrorCode = ServiceResponseMessageType.Validation_Required
    });
}

if (campaign.ImporteObjetivo <= 0)
{
    errors.Add(new ServiceResponseMessage
    {
        Message = "El importe objetivo es obligatorio",
        ErrorCode = ServiceResponseMessageType.Validation_InvalidAmount
    });
}

if (campaign.FechaFin == null || campaign.FechaFin < DateTime.UtcNow.AddDays(7))
{
    errors.Add(new ServiceResponseMessage
    {
        Message = "La campania debe durar minimo 7 dias desde hoy",
        ErrorCode = ServiceResponseMessageType.Validation_InvalidDate
    });
}

if (errors.Any())
{
    return new ServiceResponse<PublishCampaniaResponse>
    {
        Messages = errors
    };
}
```

---

## 14. Siguiente Paso Sugerido

Después de implementar los contratos API, el siguiente paso es:

1. **Implementar los Commands y Queries faltantes:**
   - `PublishCampaniaCommand` + Handler
   - `GetMisCampaniasQuery` + Handler

2. **Refinar Controller con extracción de ArtistaId del token**

3. **Crear tests unitarios para validators y handlers**

4. **Implementar frontend (Admin Dashboard):**
   - Wizard de creación (4 pasos)
   - Vista previa de campaña
   - Botón "Publicar"
   - Listado "Mis Campañas"

5. **Implementar frontend (Landing):**
   - Listado público de campañas
   - Detalle de campaña

---

**Fin del plan de contratos API**
