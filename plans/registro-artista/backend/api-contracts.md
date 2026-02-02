# Contratos API: Registro de Artista

**Fecha:** 2026-01-26
**Modulo:** UserAccess
**Feature:** registro-artista

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion | Auth |
|--------|------|------|-------------|------|
| POST | /api/auth/register | Command | Registrar nuevo usuario con email/password | No |
| POST | /api/artistas | Command | Crear perfil de artista vinculado a UserId | Si (JWT) |
| GET | /api/artistas/{id} | Query | Obtener perfil publico de artista por ID | No |
| GET | /api/artistas/by-user/{userId} | Query | Obtener perfil de artista por UserId | Si (JWT) |

---

## 2. Request DTOs

### 2.1 RegisterCommand

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Estructura:**

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Email | string | Si | NotEmpty, EmailAddress |
| Password | string | Si | NotEmpty, MinLength(8) |
| ConfirmPassword | string | Si | NotEmpty, Equal(Password) |

**Implementa:** `IRequest<ServiceResponse<RegisterResponseDto>>`

**Notas:**
- Command + Handler en el mismo archivo
- UserId se genera automaticamente por ASP.NET Core Identity
- Token JWT se genera tras registro exitoso

**Codigo de referencia:**
```csharp
public class RegisterCommand : IRequest<ServiceResponse<RegisterResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ServiceResponse<RegisterResponseDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly ILogger<RegisterCommandHandler> _logger;

    // Constructor y Handle method
}
```

---

### 2.2 CreateArtistaCommand

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Artista/Commands/CreateArtistaCommand.cs`

**Estructura:**

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| NombreArtistico | string | Si | NotEmpty, MaxLength(200) |
| Descripcion | string? | No | MaxLength(2000) |
| Pais | string? | No | MaxLength(100) |
| Ciudad | string? | No | MaxLength(100) |
| ImagenUrl | string? | No | URL valida (si no vacio) |
| UserId | string | Si (auto) | Extraido del claim JWT "sub" |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Notas:**
- UserId NO viene en request body, se extrae del token JWT
- Handler valida que el UserId no tenga ya un perfil Artista (unicidad)
- ImagenUrl acepta vacio o URL valida (http/https)

**Codigo de referencia:**
```csharp
public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }

    // Propiedad interna, NO del request body
    public string? UserId { get; set; }
}

public class CreateArtistaCommandHandler : IRequestHandler<CreateArtistaCommand, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateArtistaCommand> _validator;
    private readonly ILogger<CreateArtistaCommandHandler> _logger;

    // Constructor y Handle method
}
```

---

### 2.3 GetArtistaByIdQuery

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Artista/Queries/GetArtistaByIdQuery.cs`

**Estructura:**

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Id | Guid | Si | NotEmpty |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Notas:**
- Query publica, no requiere autenticacion
- Retorna perfil completo del artista
- Retorna 404 si no existe

**Codigo de referencia:**
```csharp
public class GetArtistaByIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public Guid Id { get; set; }
}

public class GetArtistaByIdQueryHandler : IRequestHandler<GetArtistaByIdQuery, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetArtistaByIdQueryHandler> _logger;

    // Constructor y Handle method
}
```

---

### 2.4 GetArtistaByUserIdQuery

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Artista/Queries/GetArtistaByUserIdQuery.cs`

**Estructura:**

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| UserId | Guid | Si | NotEmpty |
| RequestingUserId | string | Si (auto) | Extraido del claim JWT "sub" |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Notas:**
- Requiere autenticacion JWT
- Handler valida que RequestingUserId == UserId (solo puede ver su propio perfil)
- Retorna 401 si no autorizado, 404 si no existe perfil

**Codigo de referencia:**
```csharp
public class GetArtistaByUserIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public Guid UserId { get; set; }

    // Propiedad interna para validacion
    public string? RequestingUserId { get; set; }
}

public class GetArtistaByUserIdQueryHandler : IRequestHandler<GetArtistaByUserIdQuery, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetArtistaByUserIdQueryHandler> _logger;

    // Constructor y Handle method
}
```

---

## 3. Response DTOs

### 3.1 RegisterResponseDto

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/RegisterResponseDto.cs`

**Estructura:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario creado (Identity GUID) |
| Email | string | Email del usuario |
| Token | string | Token JWT para autenticacion |

**Wrapped en:** `ServiceResponse<RegisterResponseDto>`

**Codigo de referencia:**
```csharp
namespace WePlayRises.UserAccess.Application.Dtos;

public class RegisterResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
```

---

### 3.2 ArtistaDto

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/ArtistaDto.cs`

**Estructura:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del artista |
| UserId | string | ID del usuario propietario (Identity) |
| NombreArtistico | string | Nombre artistico publico |
| Descripcion | string? | Biografia o descripcion |
| Pais | string? | Pais de origen |
| Ciudad | string? | Ciudad de residencia |
| ImagenUrl | string? | URL de imagen de perfil |
| FechaCreacion | DateTime | Fecha de creacion del perfil |
| FechaActualizacion | DateTime? | Fecha de ultima actualizacion |

**Wrapped en:** `ServiceResponse<ArtistaDto>`

**Notas:**
- Mapeo directo desde entidad `Artista`
- UserId es `string` porque viene de ASP.NET Core Identity
- Id es `Guid` (ArtistaId strongly-typed en Domain, pero Guid en DTO)

**Codigo de referencia:**
```csharp
namespace WePlayRises.UserAccess.Application.Dtos;

public class ArtistaDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

---

### 3.3 ArtistaListDto

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/ArtistaListDto.cs`

**Estructura:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del artista |
| NombreArtistico | string | Nombre artistico publico |
| ImagenUrl | string? | URL de imagen de perfil |
| Ciudad | string? | Ciudad de residencia |
| Pais | string? | Pais de origen |

**Wrapped en:** `ServiceResponse<List<ArtistaListDto>>`

**Notas:**
- DTO simplificado para listados (uso futuro)
- No incluye campos como Descripcion, FechaCreacion para optimizar payload
- Se usara en endpoints GET /api/artistas (futuro)

**Codigo de referencia:**
```csharp
namespace WePlayRises.UserAccess.Application.Dtos;

public class ArtistaListDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
}
```

---

## 4. Validadores

### 4.1 RegisterCommandValidator

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/RegisterCommandValidator.cs`

**Reglas de validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Email | NotEmpty | El email es obligatorio | VALIDATION_REQUIRED |
| Email | EmailAddress | El formato del email no es valido | AUTH_EMAIL_INVALID |
| Password | NotEmpty | La contraseña es obligatoria | VALIDATION_REQUIRED |
| Password | MinimumLength(8) | La contraseña debe tener al menos 8 caracteres | AUTH_PASSWORD_MIN_LENGTH |
| ConfirmPassword | NotEmpty | Confirme su contraseña | VALIDATION_REQUIRED |
| ConfirmPassword | Equal(Password) | Las contraseñas no coinciden | AUTH_PASSWORD_MISMATCH |

**Validaciones adicionales (en Handler):**
- Email unico: Verificar que email no existe en `UserManager<IdentityUser>`
- ErrorCode: `AUTH_EMAIL_EXISTS`
- Mensaje: "Este email ya esta registrado"

**Codigo de referencia:**
```csharp
using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;

namespace WePlayRises.UserAccess.Application.Features.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .WithErrorCode("VALIDATION_REQUIRED")
            .EmailAddress()
            .WithMessage("El formato del email no es valido")
            .WithErrorCode("AUTH_EMAIL_INVALID");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria")
            .WithErrorCode("VALIDATION_REQUIRED")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres")
            .WithErrorCode("AUTH_PASSWORD_MIN_LENGTH");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirme su contraseña")
            .WithErrorCode("VALIDATION_REQUIRED")
            .Equal(x => x.Password)
            .WithMessage("Las contraseñas no coinciden")
            .WithErrorCode("AUTH_PASSWORD_MISMATCH");
    }
}
```

---

### 4.2 CreateArtistaCommandValidator

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Artista/Validators/CreateArtistaCommandValidator.cs`

**Reglas de validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| NombreArtistico | NotEmpty | El nombre artistico es obligatorio | VALIDATION_REQUIRED |
| NombreArtistico | MaximumLength(200) | El nombre artistico no puede superar los 200 caracteres | ARTISTA_NOMBRE_MAX_LENGTH |
| Descripcion | MaximumLength(2000) | La descripcion no puede superar los 2000 caracteres | ARTISTA_DESC_MAX_LENGTH |
| Pais | MaximumLength(100) | El pais no puede superar los 100 caracteres | ARTISTA_PAIS_MAX_LENGTH |
| Ciudad | MaximumLength(100) | La ciudad no puede superar los 100 caracteres | ARTISTA_CIUDAD_MAX_LENGTH |
| ImagenUrl | Must(BeValidUrl) | La URL de la imagen no es valida | ARTISTA_IMAGEN_URL_INVALIDA |
| UserId | NotEmpty | El UserId es obligatorio | VALIDATION_REQUIRED |

**Validaciones adicionales (en Handler):**
- UserId unico: Verificar que UserId no tiene ya un perfil Artista
- ErrorCode: `ARTISTA_ALREADY_EXISTS`
- Mensaje: "Este usuario ya tiene un perfil de artista"

**Metodo helper para URL:**
```csharp
private bool BeValidUrl(string? url)
{
    if (string.IsNullOrWhiteSpace(url))
        return true; // Permitir vacio

    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}
```

**Codigo de referencia:**
```csharp
using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Artista.Commands;

namespace WePlayRises.UserAccess.Application.Features.Artista.Validators;

public class CreateArtistaCommandValidator : AbstractValidator<CreateArtistaCommand>
{
    public CreateArtistaCommandValidator()
    {
        RuleFor(x => x.NombreArtistico)
            .NotEmpty()
            .WithMessage("El nombre artistico es obligatorio")
            .WithErrorCode("VALIDATION_REQUIRED")
            .MaximumLength(200)
            .WithMessage("El nombre artistico no puede superar los 200 caracteres")
            .WithErrorCode("ARTISTA_NOMBRE_MAX_LENGTH");

        RuleFor(x => x.Descripcion)
            .MaximumLength(2000)
            .WithMessage("La descripcion no puede superar los 2000 caracteres")
            .WithErrorCode("ARTISTA_DESC_MAX_LENGTH")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));

        RuleFor(x => x.Pais)
            .MaximumLength(100)
            .WithMessage("El pais no puede superar los 100 caracteres")
            .WithErrorCode("ARTISTA_PAIS_MAX_LENGTH")
            .When(x => !string.IsNullOrEmpty(x.Pais));

        RuleFor(x => x.Ciudad)
            .MaximumLength(100)
            .WithMessage("La ciudad no puede superar los 100 caracteres")
            .WithErrorCode("ARTISTA_CIUDAD_MAX_LENGTH")
            .When(x => !string.IsNullOrEmpty(x.Ciudad));

        RuleFor(x => x.ImagenUrl)
            .Must(BeValidUrl)
            .WithMessage("La URL de la imagen no es valida. Debe comenzar con http:// o https://")
            .WithErrorCode("ARTISTA_IMAGEN_URL_INVALIDA")
            .When(x => !string.IsNullOrEmpty(x.ImagenUrl));

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode("VALIDATION_REQUIRED");
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
```

---

## 5. AutoMapper Mappings

### 5.1 ArtistaProfile

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Mapping/ArtistaProfile.cs`

**Mappings definidos:**

| Source | Destination | Notas |
|--------|-------------|-------|
| CreateArtistaCommand | Artista | Command -> Entity para crear |
| Artista | ArtistaDto | Entity -> DTO para response completo |
| Artista | ArtistaListDto | Entity -> DTO para listados (simplificado) |

**Transformaciones especiales:**
- `Artista.Id` (ArtistaId strongly-typed) -> `ArtistaDto.Id` (Guid): Mapeo automatico
- `Artista.UserIdPropietario` -> `ArtistaDto.UserId`: Renombrar propiedad
- `FechaCreacion`, `FechaActualizacion`: Mapeo directo

**Codigo de referencia:**
```csharp
using AutoMapper;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artista.Commands;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Mapping;

public class ArtistaProfile : Profile
{
    public ArtistaProfile()
    {
        // Command -> Entity
        CreateMap<CreateArtistaCommand, Artista>()
            .ForMember(dest => dest.UserIdPropietario, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Se genera en el Service
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se asigna en el Service
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());

        // Entity -> DTO (completo)
        CreateMap<Artista, ArtistaDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value)) // ArtistaId -> Guid
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserIdPropietario));

        // Entity -> DTO (listado simplificado)
        CreateMap<Artista, ArtistaListDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value));
    }
}
```

**Notas importantes:**
- `Artista.Id` es de tipo `ArtistaId` (strongly-typed ID), pero el DTO usa `Guid`
- El mapeo `ArtistaId -> Guid` se hace con `.Value` property
- `UserIdPropietario` en entidad se mapea a `UserId` en DTO para consistencia con contracts
- Campos como `UrlSitioWeb`, `UrlInstagram`, etc. NO estan en el DTO inicial (no son requeridos para MVP)

---

### 5.2 AuthProfile

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Mapping/AuthProfile.cs`

**Mappings definidos:**

| Source | Destination | Notas |
|--------|-------------|-------|
| IdentityUser | RegisterResponseDto | Parcial: Solo para UserId y Email, Token se asigna manualmente |

**Codigo de referencia:**
```csharp
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using WePlayRises.UserAccess.Application.Dtos;

namespace WePlayRises.UserAccess.Application.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // IdentityUser -> RegisterResponseDto
        // Nota: Token se asigna manualmente en el Handler
        CreateMap<IdentityUser, RegisterResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token, opt => opt.Ignore()); // Se asigna manualmente
    }
}
```

---

## 6. Controller Actions

### 6.1 AuthController

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

**Estructura:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;

namespace WePlayRises.UserAccess.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="command">Datos de registro (email, password, confirmPassword)</param>
    /// <returns>Informacion del usuario registrado y token JWT</returns>
    /// <response code="200">Usuario registrado exitosamente</response>
    /// <response code="400">Datos de validacion incorrectos</response>
    /// <response code="409">Email ya existe en el sistema</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            // Determinar status code segun el error
            var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
            return errorCode switch
            {
                "AUTH_EMAIL_EXISTS" => Conflict(result),
                "VALIDATION_REQUIRED" or "AUTH_EMAIL_INVALID" or "AUTH_PASSWORD_MIN_LENGTH" or "AUTH_PASSWORD_MISMATCH" => BadRequest(result),
                _ => StatusCode(500, result)
            };
        }

        return Ok(result);
    }
}
```

**Decoradores Swagger/OpenAPI:**
- `[AllowAnonymous]`: Endpoint publico
- `[ProducesResponseType]`: Documentar responses posibles
- XML Comments: `<summary>`, `<param>`, `<returns>`, `<response>`

---

### 6.2 ArtistasController

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/ArtistasController.cs`

**Estructura:**

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artista.Commands;
using WePlayRises.UserAccess.Application.Features.Artista.Queries;

namespace WePlayRises.UserAccess.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistasController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArtistasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo perfil de artista vinculado al usuario autenticado.
    /// </summary>
    /// <param name="command">Datos del perfil (nombreArtistico, descripcion, etc.)</param>
    /// <returns>Perfil de artista creado</returns>
    /// <response code="200">Perfil creado exitosamente</response>
    /// <response code="400">Datos de validacion incorrectos</response>
    /// <response code="401">Token JWT invalido o expirado</response>
    /// <response code="409">Usuario ya tiene un perfil de artista</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateArtistaCommand command)
    {
        // Extraer UserId del token JWT
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Token invalido o expirado", ErrorCode = "AUTH_UNAUTHORIZED" }
                }
            });
        }

        command.UserId = userId;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
            return errorCode switch
            {
                "ARTISTA_ALREADY_EXISTS" => Conflict(result),
                "VALIDATION_REQUIRED" or "ARTISTA_NOMBRE_MAX_LENGTH" or "ARTISTA_DESC_MAX_LENGTH" or "ARTISTA_IMAGEN_URL_INVALIDA" => BadRequest(result),
                _ => StatusCode(500, result)
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene el perfil publico de un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista</param>
    /// <returns>Perfil del artista</returns>
    /// <response code="200">Artista encontrado</response>
    /// <response code="404">Artista no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetArtistaByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
            return errorCode switch
            {
                "ARTISTA_NOT_FOUND" => NotFound(result),
                _ => StatusCode(500, result)
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene el perfil de artista del usuario autenticado por su UserId.
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Perfil del artista</returns>
    /// <response code="200">Artista encontrado</response>
    /// <response code="401">Token invalido o no autorizado</response>
    /// <response code="404">Artista no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("by-user/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        // Validar que el usuario solo pueda ver su propio perfil
        var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(requestingUserId) || requestingUserId != userId.ToString())
        {
            return Unauthorized(new ServiceResponse<ArtistaDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "No tienes permisos para ver este perfil", ErrorCode = "AUTH_UNAUTHORIZED" }
                }
            });
        }

        var query = new GetArtistaByUserIdQuery
        {
            UserId = userId,
            RequestingUserId = requestingUserId
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
            return errorCode switch
            {
                "ARTISTA_NOT_FOUND" => NotFound(result),
                _ => StatusCode(500, result)
            };
        }

        return Ok(result);
    }
}
```

**Decoradores:**
- `[Authorize]`: Requiere JWT token
- `[AllowAnonymous]`: Endpoint publico
- `[ProducesResponseType]`: Documentar tipos de response

**Extraccion de UserId del JWT:**
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
```

---

## 7. OpenAPI Documentation

### 7.1 POST /api/auth/register

**Summary:** Registra un nuevo usuario en el sistema

**Description:** Crea una cuenta de usuario utilizando ASP.NET Core Identity y retorna un token JWT para autenticacion inmediata.

**Request Body:**
```json
{
  "email": "artista@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

**Responses:**

**200 OK - Usuario registrado exitosamente**
```json
{
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "artista@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "messages": [
    {
      "message": "Usuario registrado exitosamente",
      "errorCode": "SUCCESS",
      "httpStatusCode": 200
    }
  ]
}
```

**400 Bad Request - Errores de validacion**
```json
{
  "data": null,
  "messages": [
    {
      "message": "El email es obligatorio",
      "errorCode": "VALIDATION_REQUIRED",
      "httpStatusCode": 400
    },
    {
      "message": "Las contraseñas no coinciden",
      "errorCode": "AUTH_PASSWORD_MISMATCH",
      "httpStatusCode": 400
    }
  ]
}
```

**409 Conflict - Email duplicado**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Este email ya esta registrado",
      "errorCode": "AUTH_EMAIL_EXISTS",
      "httpStatusCode": 409
    }
  ]
}
```

**500 Internal Server Error**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Error inesperado al crear cuenta",
      "errorCode": "ERROR_UNEXPECTED",
      "httpStatusCode": 500
    }
  ]
}
```

**Authentication:** None (publico)

---

### 7.2 POST /api/artistas

**Summary:** Crea un nuevo perfil de artista vinculado al usuario autenticado

**Description:** Registra el perfil artistico con nombre, descripcion y ubicacion. El UserId se extrae automaticamente del token JWT.

**Request Body:**
```json
{
  "nombreArtistico": "Los Rockeros",
  "descripcion": "Banda de rock alternativo de Madrid",
  "pais": "España",
  "ciudad": "Madrid",
  "imagenUrl": "https://example.com/artista.jpg"
}
```

**Responses:**

**200 OK - Perfil creado exitosamente**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Perfil de artista creado exitosamente",
      "errorCode": "SUCCESS",
      "httpStatusCode": 200
    }
  ]
}
```

**400 Bad Request - Errores de validacion**
```json
{
  "data": null,
  "messages": [
    {
      "message": "El nombre artistico es obligatorio",
      "errorCode": "VALIDATION_REQUIRED",
      "httpStatusCode": 400
    }
  ]
}
```

**401 Unauthorized - Token invalido**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Token invalido o expirado",
      "errorCode": "AUTH_UNAUTHORIZED",
      "httpStatusCode": 401
    }
  ]
}
```

**409 Conflict - Usuario ya tiene perfil**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Este usuario ya tiene un perfil de artista",
      "errorCode": "ARTISTA_ALREADY_EXISTS",
      "httpStatusCode": 409
    }
  ]
}
```

**Authentication:** Bearer JWT (claim: `sub` = UserId)

---

### 7.3 GET /api/artistas/{id}

**Summary:** Obtiene el perfil publico de un artista por su ID

**Description:** Endpoint publico para visualizar informacion de artistas en la landing page.

**Parameters:**
- `id` (path, required): ID del artista (GUID)

**Responses:**

**200 OK - Artista encontrado**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Artista encontrado",
      "errorCode": "SUCCESS",
      "httpStatusCode": 200
    }
  ]
}
```

**404 Not Found - Artista no existe**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Artista no encontrado",
      "errorCode": "ARTISTA_NOT_FOUND",
      "httpStatusCode": 404
    }
  ]
}
```

**Authentication:** None (publico)

---

### 7.4 GET /api/artistas/by-user/{userId}

**Summary:** Obtiene el perfil de artista del usuario autenticado por su UserId

**Description:** Endpoint protegido para que el usuario obtenga su propio perfil de artista. Se valida que el UserId del token coincida con el parametro.

**Parameters:**
- `userId` (path, required): ID del usuario (GUID)

**Responses:**

**200 OK - Artista encontrado**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-01-26T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Artista encontrado",
      "errorCode": "SUCCESS",
      "httpStatusCode": 200
    }
  ]
}
```

**401 Unauthorized - Token invalido o no autorizado**
```json
{
  "data": null,
  "messages": [
    {
      "message": "No tienes permisos para ver este perfil",
      "errorCode": "AUTH_UNAUTHORIZED",
      "httpStatusCode": 401
    }
  ]
}
```

**404 Not Found - Usuario no tiene perfil de artista**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Artista no encontrado para este usuario",
      "errorCode": "ARTISTA_NOT_FOUND",
      "httpStatusCode": 404
    }
  ]
}
```

**Authentication:** Bearer JWT (claim: `sub` debe coincidir con `userId`)

---

## 8. Archivos a Crear

```
Modules/UserAccess/
├── WePlayRises.UserAccess.Application/
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   └── RegisterCommand.cs           (Command + Handler)
│   │   │   └── Validators/
│   │   │       └── RegisterCommandValidator.cs
│   │   └── Artista/
│   │       ├── Commands/
│   │       │   └── CreateArtistaCommand.cs      (Command + Handler)
│   │       ├── Queries/
│   │       │   ├── GetArtistaByIdQuery.cs       (Query + Handler)
│   │       │   └── GetArtistaByUserIdQuery.cs   (Query + Handler)
│   │       └── Validators/
│   │           └── CreateArtistaCommandValidator.cs
│   ├── Dtos/
│   │   ├── RegisterResponseDto.cs
│   │   ├── ArtistaDto.cs
│   │   └── ArtistaListDto.cs
│   ├── Mapping/
│   │   ├── ArtistaProfile.cs
│   │   └── AuthProfile.cs
│   └── Interfaces/
│       └── Services/
│           └── IJwtTokenGenerator.cs            (Si no existe)
│
├── WePlayRises.UserAccess.WebApi/
│   └── Controllers/
│       ├── AuthController.cs
│       └── ArtistasController.cs
│
└── WePlayRises.UserAccess.Infra/
    └── Services/
        └── JwtTokenGenerator.cs                 (Implementacion)
```

---

## 9. Dependencias de Servicios

### 9.1 IArtistaService (Ya existe)

**Ubicacion:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IArtistaService.cs`

**Metodos requeridos:**
```csharp
Task<Artista?> GetByIdAsync(Guid id, CancellationToken ct);
Task<Artista?> GetByUserIdAsync(Guid userId, CancellationToken ct);
Task<Guid> CreateAsync(Artista artista, CancellationToken ct);
```

**Nota:** Ya existe interfaz, solo falta implementacion en Infra layer.

---

### 9.2 IJwtTokenGenerator (Nuevo)

**Ubicacion:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IJwtTokenGenerator.cs`

**Metodos requeridos:**
```csharp
public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email);
}
```

**Implementacion:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/JwtTokenGenerator.cs`

**Claims generados:**
- `sub`: UserId (GUID string)
- `email`: Email del usuario
- `exp`: Expiracion (24 horas por defecto)
- `iat`: Issued at

---

## 10. ErrorCodes Catalog

**Archivo de referencia:** `BuildingBlocks/Kernel/Constants/ValidationErrorCodes.cs` (si no existe, crear)

| ErrorCode | Descripcion | HTTP Status |
|-----------|-------------|-------------|
| SUCCESS | Operacion exitosa | 200/201 |
| VALIDATION_REQUIRED | Campo requerido vacio | 400 |
| AUTH_EMAIL_INVALID | Formato de email invalido | 400 |
| AUTH_EMAIL_EXISTS | Email duplicado en Identity | 409 |
| AUTH_PASSWORD_MIN_LENGTH | Password menor a 8 caracteres | 400 |
| AUTH_PASSWORD_MISMATCH | Passwords no coinciden | 400 |
| AUTH_UNAUTHORIZED | Token invalido o expirado | 401 |
| ARTISTA_NOMBRE_MAX_LENGTH | Nombre artistico excede 200 caracteres | 400 |
| ARTISTA_DESC_MAX_LENGTH | Descripcion excede 2000 caracteres | 400 |
| ARTISTA_PAIS_MAX_LENGTH | Pais excede 100 caracteres | 400 |
| ARTISTA_CIUDAD_MAX_LENGTH | Ciudad excede 100 caracteres | 400 |
| ARTISTA_IMAGEN_URL_INVALIDA | URL de imagen formato invalido | 400 |
| ARTISTA_ALREADY_EXISTS | UserId ya tiene perfil Artista | 409 |
| ARTISTA_NOT_FOUND | Artista no encontrado por ID o UserId | 404 |
| ERROR_UNEXPECTED | Error no controlado | 500 |

---

## 11. Checklist de Implementacion

- [ ] **DTOs creados:**
  - [ ] RegisterResponseDto
  - [ ] ArtistaDto
  - [ ] ArtistaListDto

- [ ] **Commands/Queries creados (con Handlers en mismo archivo):**
  - [ ] RegisterCommand + RegisterCommandHandler
  - [ ] CreateArtistaCommand + CreateArtistaCommandHandler
  - [ ] GetArtistaByIdQuery + GetArtistaByIdQueryHandler
  - [ ] GetArtistaByUserIdQuery + GetArtistaByUserIdQueryHandler

- [ ] **Validators creados (con WithMessage + WithErrorCode):**
  - [ ] RegisterCommandValidator
  - [ ] CreateArtistaCommandValidator

- [ ] **AutoMapper Profiles creados:**
  - [ ] ArtistaProfile (Command -> Entity, Entity -> DTOs)
  - [ ] AuthProfile (IdentityUser -> RegisterResponseDto)

- [ ] **Controllers creados:**
  - [ ] AuthController (Register action)
  - [ ] ArtistasController (Create, GetById, GetByUserId actions)

- [ ] **Servicios implementados:**
  - [ ] IJwtTokenGenerator interface + implementacion
  - [ ] ArtistaService implementacion (si no existe)

- [ ] **Swagger documentation:**
  - [ ] XML comments en controllers
  - [ ] ProducesResponseType decorators
  - [ ] Request/Response examples documentados

- [ ] **Handlers siguen patron CQRS:**
  - [ ] Handler + Command/Query en mismo archivo
  - [ ] Retornan ServiceResponse<T>
  - [ ] NO inyectan DbContext (usan Services)
  - [ ] Incluyen try-catch con logging
  - [ ] Validacion retorna ServiceResponse (no throw)

- [ ] **Services registrados en DI:**
  - [ ] IArtistaService en Infra DependencyInjection
  - [ ] IJwtTokenGenerator en Infra DependencyInjection
  - [ ] Validators en Application DependencyInjection
  - [ ] AutoMapper profiles registrados

---

## 12. Notas de Implementacion

### 12.1 Strongly-Typed IDs

La entidad `Artista` usa `ArtistaId` (strongly-typed ID) en Domain, pero el DTO usa `Guid`:

```csharp
// Domain
public class Artista
{
    public ArtistaId Id { get; set; } // Strongly-typed
}

// DTO
public class ArtistaDto
{
    public Guid Id { get; set; } // Simple Guid
}

// Mapping
CreateMap<Artista, ArtistaDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value));
```

### 12.2 UserId en Commands

El `UserId` NO viene en el request body, se extrae del token JWT en el Controller:

```csharp
// Controller
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
command.UserId = userId;
var result = await _mediator.Send(command);

// Command
public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    // ... propiedades del request body

    // Propiedad interna (no del JSON)
    public string? UserId { get; set; }
}
```

### 12.3 Validacion de Unicidad

La validacion de unicidad (email duplicado, UserId duplicado) se hace en el **Handler**, no en el Validator:

```csharp
// Handler
var existingUser = await _userManager.FindByEmailAsync(request.Email);
if (existingUser != null)
{
    return new ServiceResponse<RegisterResponseDto>
    {
        Messages = new()
        {
            new()
            {
                Message = "Este email ya esta registrado",
                ErrorCode = "AUTH_EMAIL_EXISTS"
            }
        }
    };
}
```

**Razon:** Las validaciones que requieren acceso a DB/Services se hacen en Handler, no en Validator.

### 12.4 ImagenUrl Opcional

El campo `ImagenUrl` es opcional y puede ser:
- Vacio/null: Valido
- URL valida (http/https): Valido
- URL invalida: Error de validacion

```csharp
// Validator
RuleFor(x => x.ImagenUrl)
    .Must(BeValidUrl)
    .When(x => !string.IsNullOrEmpty(x.ImagenUrl)); // Solo validar si no vacio
```

### 12.5 JWT Claims

El token JWT debe incluir:
- `sub`: UserId (claim type: `ClaimTypes.NameIdentifier`)
- `email`: Email del usuario (claim type: `ClaimTypes.Email`)
- `exp`: Expiracion (24 horas)

```csharp
// JwtTokenGenerator
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, userId),
    new(ClaimTypes.Email, email)
};
```

---

## 13. Siguiente Paso Sugerido

Una vez completado este plan de contratos API, el siguiente paso es:

**Crear el plan de implementacion del backend:**
- `plans/registro-artista/backend/implementation-plan.md`

Este plan debera incluir:
1. Implementacion de Services (ArtistaService, JwtTokenGenerator)
2. Implementacion de Repositories (ArtistaRepository)
3. Configuracion de DbContext y Entity Framework
4. Configuracion de JWT Authentication en WebApi
5. Registro de dependencias en DI
6. Testing unitario de Handlers y Services

Alternativamente, si el agente de implementacion backend esta listo, puede proceder directamente a implementar los archivos listados en la seccion 8.
