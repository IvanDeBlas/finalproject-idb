# Contratos API: Registro de Artista

**Fecha:** 2026-02-12
**Modulo:** UserAccess
**Feature:** registro-artista

---

## 1. Resumen de Endpoints

| Metodo | Ruta | Tipo | Autorizacion | Status Implementacion |
|--------|------|------|--------------|----------------------|
| POST | /api/auth/register | Command | Publico | **IMPLEMENTADO** |
| POST | /api/artistas | Command | Bearer JWT | **IMPLEMENTADO** |
| GET | /api/artistas/{id} | Query | Publico | **IMPLEMENTADO** |
| GET | /api/artistas/by-user/{userId} | Query | Bearer JWT | **IMPLEMENTADO** |
| PUT | /api/artistas/{id} | Command | Bearer JWT | **PENDIENTE** |

---

## 2. Request DTOs (Commands/Queries)

### 2.1 RegisterCommand (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Propiedades:**

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| Email | string | Si | Email del usuario (formato valido) |
| Password | string | Si | Contrasena (minimo 8 caracteres) |
| ConfirmPassword | string | Si | Confirmacion de contrasena (debe coincidir) |
| Role | string? | No | Rol del usuario (Fan por defecto) |

**Implementa:** `IRequest<ServiceResponse<RegisterResponseDto>>`

**Codigo Existente:**
```csharp
public class RegisterCommand : IRequest<ServiceResponse<RegisterResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string? Role { get; set; }
}
```

**Handler:** `RegisterCommandHandler` (mismo archivo)
- Valida con `RegisterCommandValidator`
- Verifica email duplicado
- Crea usuario en Identity
- Asigna rol (Fan por defecto)
- Genera JWT token
- Retorna `RegisterResponseDto` con token

---

### 2.2 CreateArtistaCommand (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Commands/CreateArtistaCommand.cs`

**Propiedades:**

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| NombreArtistico | string | Si | Nombre artistico (max 200 caracteres) |
| Descripcion | string? | No | Biografia del artista (max 2000 caracteres) |
| Pais | string? | No | Pais de origen (max 100 caracteres) |
| Ciudad | string? | No | Ciudad de residencia (max 100 caracteres) |
| ImagenUrl | string? | No | URL de imagen de perfil (formato URL valido) |
| UserId | string? | Si | UserId del token JWT (poblado por controller) |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Codigo Existente:**
```csharp
public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? Pais { get; set; }
    public string? Ciudad { get; set; }
    public string? ImagenUrl { get; set; }
    public string? UserId { get; set; }  // Populated from JWT token
}
```

**Handler:** `CreateArtistaCommandHandler` (mismo archivo)
- Valida con `CreateArtistaCommandValidator`
- Verifica que UserId este presente (autenticacion)
- Verifica que usuario no tenga perfil artista existente
- Mapea Command a Entity con AutoMapper
- Crea entidad via `IArtistaService`
- Recupera entidad creada y mapea a DTO
- Retorna `ArtistaDto`

---

### 2.3 GetArtistaByIdQuery (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Queries/GetArtistaByIdQuery.cs`

**Propiedades:**

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| Id | Guid | Si | Identificador unico del artista |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Codigo Existente:**
```csharp
public class GetArtistaByIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public Guid Id { get; set; }
}
```

**Handler:** `GetArtistaByIdQueryHandler` (mismo archivo)
- Recupera entidad via `IArtistaService.GetByIdAsync()`
- Retorna 404 si no existe
- Mapea Entity a DTO con AutoMapper
- Retorna `ArtistaDto`

---

### 2.4 GetArtistaByUserIdQuery (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Queries/GetArtistaByUserIdQuery.cs`

**Propiedades:**

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| UserId | string | Si | UserId del artista a buscar |
| RequestingUserId | string? | Si | UserId del token JWT (para validacion) |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Codigo Existente:**
```csharp
public class GetArtistaByUserIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public string UserId { get; set; } = null!;
    public string? RequestingUserId { get; set; }  // Populated from JWT token
}
```

**Handler:** `GetArtistaByUserIdQueryHandler` (mismo archivo)
- Valida que RequestingUserId coincida con UserId (autorizacion)
- Recupera entidad via `IArtistaService.GetByUserIdAsync()`
- Retorna 404 si no existe
- Mapea Entity a DTO con AutoMapper
- Retorna `ArtistaDto`

---

## 3. Response DTOs

### 3.1 RegisterResponseDto (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/RegisterResponseDto.cs`

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | Identificador unico del usuario (GUID) |
| Email | string | Email del usuario |
| Token | string | JWT token de autenticacion |
| Roles | List\<string\> | Lista de roles asignados al usuario |

**Codigo Existente:**
```csharp
public class RegisterResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
```

**Wrapped en:** `ServiceResponse<RegisterResponseDto>`

**Ejemplo Response 200 OK:**
```json
{
  "data": {
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "email": "banda@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "roles": ["Fan"]
  },
  "messages": [
    {
      "message": "Usuario registrado exitosamente",
      "errorCode": "",
      "httpStatusCode": 200
    }
  ]
}
```

---

### 3.2 ArtistaDto (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/ArtistaDto.cs`

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del artista |
| UserId | string | UserId del propietario del perfil |
| NombreArtistico | string | Nombre artistico |
| Descripcion | string? | Biografia del artista |
| Pais | string? | Pais de origen |
| Ciudad | string? | Ciudad de residencia |
| ImagenUrl | string? | URL de imagen de perfil |
| FechaCreacion | DateTime | Fecha de creacion del perfil |
| FechaActualizacion | DateTime? | Fecha de ultima actualizacion |

**Codigo Existente:**
```csharp
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

**Wrapped en:** `ServiceResponse<ArtistaDto>`

**Ejemplo Response 200 OK:**
```json
{
  "data": {
    "id": "b2c3d4e5-f6g7-8901-bcde-f12345678901",
    "userId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "nombreArtistico": "Los Rockeros",
    "descripcion": "Banda de rock alternativo de Madrid",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artista.jpg",
    "fechaCreacion": "2026-02-12T10:30:00Z",
    "fechaActualizacion": null
  },
  "messages": [
    {
      "message": "Perfil de artista creado exitosamente",
      "errorCode": "",
      "httpStatusCode": 200
    }
  ]
}
```

---

### 3.3 ArtistaListDto (IMPLEMENTADO - Para uso futuro)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/ArtistaListDto.cs`

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del artista |
| NombreArtistico | string | Nombre artistico |
| ImagenUrl | string? | URL de imagen de perfil |
| Ciudad | string? | Ciudad de residencia |
| Pais | string? | Pais de origen |

**Uso:** DTO simplificado para listados de artistas (pendiente implementacion de endpoint GetAll)

---

## 4. Validadores (FluentValidation)

### 4.1 RegisterCommandValidator (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Auth/Validators/RegisterCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Email | NotEmpty | El email es obligatorio | `ServiceResponseMessageType.Validation_Required` (1001) |
| Email | EmailAddress | El formato del email no es valido | `ServiceResponseMessageType.Validation_InvalidEmail` (1005) |
| Password | NotEmpty | La contrasena es obligatoria | `ServiceResponseMessageType.Validation_Required` (1001) |
| Password | MinimumLength(8) | La contrasena debe tener al menos 8 caracteres | `ServiceResponseMessageType.Validation_MinLength` (1003) |
| ConfirmPassword | NotEmpty | Confirme su contrasena | `ServiceResponseMessageType.Validation_Required` (1001) |
| ConfirmPassword | Equal(Password) | Las contrasenas no coinciden | `ServiceResponseMessageType.Validation_InvalidFormat` (1004) |
| Role | Must(IsValid) | Rol invalido. Roles permitidos: Artista, Fan, Admin | `ServiceResponseMessageType.Validation_InvalidFormat` (1004) |

**Codigo Existente:**
```csharp
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .EmailAddress()
            .WithMessage("El formato del email no es valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contrasena es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(8)
            .WithMessage("La contrasena debe tener al menos 8 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirme su contrasena")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .Equal(x => x.Password)
            .WithMessage("Las contrasenas no coinciden")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);

        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrEmpty(role) || Roles.IsValid(role))
            .WithMessage("Rol invalido. Roles permitidos: Artista, Fan, Admin")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat)
            .When(x => !string.IsNullOrEmpty(x.Role));
    }
}
```

---

### 4.2 CreateArtistaCommandValidator (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Validators/CreateArtistaCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| NombreArtistico | NotEmpty | El nombre artistico es obligatorio | `ServiceResponseMessageType.Validation_Required` (1001) |
| NombreArtistico | MaximumLength(200) | El nombre artistico no puede superar los 200 caracteres | `ServiceResponseMessageType.Validation_MaxLength` (1002) |
| Descripcion | MaximumLength(2000) | La descripcion no puede superar los 2000 caracteres | `ServiceResponseMessageType.Validation_MaxLength` (1002) |
| Pais | MaximumLength(100) | El pais no puede superar los 100 caracteres | `ServiceResponseMessageType.Validation_MaxLength` (1002) |
| Ciudad | MaximumLength(100) | La ciudad no puede superar los 100 caracteres | `ServiceResponseMessageType.Validation_MaxLength` (1002) |
| ImagenUrl | Must(BeValidUrl) | La URL de la imagen no es valida. Debe comenzar con http:// o https:// | `ServiceResponseMessageType.Validation_InvalidUrl` (1006) |
| UserId | NotEmpty | El UserId es obligatorio | `ServiceResponseMessageType.Validation_Required` (1001) |

**Codigo Existente:**
```csharp
public class CreateArtistaCommandValidator : AbstractValidator<CreateArtistaCommand>
{
    public CreateArtistaCommandValidator()
    {
        RuleFor(x => x.NombreArtistico)
            .NotEmpty()
            .WithMessage("El nombre artistico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(200)
            .WithMessage("El nombre artistico no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(2000)
            .WithMessage("La descripcion no puede superar los 2000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Descripcion));

        RuleFor(x => x.Pais)
            .MaximumLength(100)
            .WithMessage("El pais no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Pais));

        RuleFor(x => x.Ciudad)
            .MaximumLength(100)
            .WithMessage("La ciudad no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Ciudad));

        RuleFor(x => x.ImagenUrl)
            .Must(BeValidUrl)
            .WithMessage("La URL de la imagen no es valida. Debe comenzar con http:// o https://")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.ImagenUrl));

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    private static bool BeValidUrl(string? url)
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

### 5.1 ArtistaProfile (IMPLEMENTADO)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Mapping/ArtistaProfile.cs`

**Mappings Configurados:**

| Source | Destination | Notas |
|--------|-------------|-------|
| CreateArtistaCommand | Artista | Mapea `UserId` a `UserIdPropietario`, ignora campos auto-generados (Id, fechas, URLs redes sociales, colecciones) |
| Artista | ArtistaDto | Mapea `Id.Value` (ArtistaId) a `Id` (Guid), `UserIdPropietario` a `UserId`, `UrlSitioWeb` a `ImagenUrl` (MVP) |
| Artista | ArtistaListDto | DTO simplificado para listados (solo campos basicos) |

**Codigo Existente:**
```csharp
public class ArtistaProfile : Profile
{
    public ArtistaProfile()
    {
        // Command -> Entity
        CreateMap<CreateArtistaCommand, Artista>()
            .ForMember(dest => dest.UserIdPropietario,
                       opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Id,
                       opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion,
                       opt => opt.Ignore())
            .ForMember(dest => dest.FechaActualizacion,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlSitioWeb,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlInstagram,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlYouTube,
                       opt => opt.Ignore())
            .ForMember(dest => dest.UrlSpotify,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaMiembros,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ArtistaFans,
                       opt => opt.Ignore())
            .ForMember(dest => dest.ProyectosArtisticos,
                       opt => opt.Ignore());

        // Entity -> DTO (full)
        CreateMap<Artista, ArtistaDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.UserIdPropietario))
            .ForMember(dest => dest.ImagenUrl,
                       opt => opt.MapFrom(src => src.UrlSitioWeb));

        // Entity -> DTO (list simplified)
        CreateMap<Artista, ArtistaListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.ImagenUrl,
                       opt => opt.MapFrom(src => src.UrlSitioWeb));
    }
}
```

**Nota:** El mapping de `UrlSitioWeb` a `ImagenUrl` es temporal para MVP. En implementaciones futuras, se usara un campo dedicado `ImagenPerfilUrl` en la entidad.

---

### 5.2 AuthProfile (IMPLEMENTADO - Inferido)

**Archivo:** `Modules/UserAccess/UserAccess.Application/Mapping/AuthProfile.cs` (inferido del codigo)

**Mappings Configurados:**

| Source | Destination | Notas |
|--------|-------------|-------|
| IdentityUser | RegisterResponseDto | Mapea `Id` a `UserId`, `Email`, token y roles se asignan manualmente en handler |

**Codigo Inferido:**
```csharp
public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<IdentityUser, RegisterResponseDto>()
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email,
                       opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token,
                       opt => opt.Ignore())  // Set manually in handler
            .ForMember(dest => dest.Roles,
                       opt => opt.Ignore()); // Set manually in handler
    }
}
```

---

## 6. OpenAPI/Swagger Documentation

### 6.1 POST /api/auth/register

**Controller:** `AuthController`
**Action:** `Register(RegisterCommand command)`

**Swagger Attributes (Existentes):**
```csharp
/// <summary>
/// Registers a new user in the system.
/// </summary>
/// <param name="command">Registration data (email, password, confirmPassword)</param>
/// <returns>User information and JWT token</returns>
/// <response code="200">User registered successfully</response>
/// <response code="400">Validation errors</response>
/// <response code="409">Email already exists</response>
/// <response code="500">Internal server error</response>
[HttpPost("register")]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status500InternalServerError)]
```

**Responses:**

| Status | Tipo | ErrorCode | Descripcion |
|--------|------|-----------|-------------|
| 200 | Success | - | Usuario registrado exitosamente |
| 400 | Error | `1001`, `1003`, `1004`, `1005` | Errores de validacion (email vacio, password corta, passwords no coinciden, email invalido) |
| 409 | Error | `1009` | Email ya registrado (`Validation_DuplicateEmail`) |
| 500 | Error | `5000` | Error inesperado (`Internal_UnexpectedError`) |

---

### 6.2 POST /api/artistas

**Controller:** `ArtistasController`
**Action:** `Create(CreateArtistaCommand command)`

**Swagger Attributes (Existentes):**
```csharp
/// <summary>
/// Creates a new artist profile linked to the authenticated user.
/// </summary>
/// <param name="command">Profile data (nombreArtistico, descripcion, etc.)</param>
/// <returns>Created artist profile</returns>
/// <response code="200">Profile created successfully</response>
/// <response code="400">Validation errors</response>
/// <response code="401">Invalid or expired JWT token</response>
/// <response code="409">User already has an artist profile</response>
/// <response code="500">Internal server error</response>
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
```

**Responses:**

| Status | Tipo | ErrorCode | Descripcion |
|--------|------|-----------|-------------|
| 200 | Success | - | Perfil de artista creado exitosamente |
| 400 | Error | `1001`, `1002`, `1006` | Errores de validacion (nombre vacio, longitud excedida, URL invalida) |
| 401 | Error | `3001` | Token no valido o expirado (`Auth_Unauthorized`) |
| 409 | Error | `4008` | Usuario ya tiene perfil de artista (`BusinessRule_ArtistaAlreadyExists`) |
| 500 | Error | `5000` | Error inesperado (`Internal_UnexpectedError`) |

---

### 6.3 GET /api/artistas/{id}

**Controller:** `ArtistasController`
**Action:** `GetById(Guid id)`

**Swagger Attributes (Existentes):**
```csharp
/// <summary>
/// Gets the public profile of an artist by their ID.
/// </summary>
/// <param name="id">Artist ID</param>
/// <returns>Artist profile</returns>
/// <response code="200">Artist found</response>
/// <response code="404">Artist not found</response>
/// <response code="500">Internal server error</response>
[HttpGet("{id}")]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
```

**Responses:**

| Status | Tipo | ErrorCode | Descripcion |
|--------|------|-----------|-------------|
| 200 | Success | - | Artista encontrado |
| 404 | Error | `2002` | Artista no encontrado (`NotFound_Artista`) |
| 500 | Error | `5000` | Error inesperado (`Internal_UnexpectedError`) |

---

### 6.4 GET /api/artistas/by-user/{userId}

**Controller:** `ArtistasController`
**Action:** `GetByUserId(string userId)`

**Swagger Attributes (Existentes):**
```csharp
/// <summary>
/// Gets the artist profile of the authenticated user by their UserId.
/// </summary>
/// <param name="userId">User ID</param>
/// <returns>Artist profile</returns>
/// <response code="200">Artist found</response>
/// <response code="401">Invalid token or unauthorized</response>
/// <response code="404">Artist not found</response>
/// <response code="500">Internal server error</response>
[HttpGet("by-user/{userId}")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<ArtistaDto>), StatusCodes.Status500InternalServerError)]
```

**Responses:**

| Status | Tipo | ErrorCode | Descripcion |
|--------|------|-----------|-------------|
| 200 | Success | - | Artista encontrado |
| 401 | Error | `3001` | Token no valido o userId no coincide (`Auth_Unauthorized`) |
| 404 | Error | `2002` | Artista no encontrado para este usuario (`NotFound_Artista`) |
| 500 | Error | `5000` | Error inesperado (`Internal_UnexpectedError`) |

---

## 7. Manejo de Errores (ServiceResponse)

### 7.1 Codigos de Error Utilizados

**Ubicacion:** `Modules/UserAccess/UserAccess.Domain/Constants/ServiceResponseMessageType.cs`

| ErrorCode | Valor | Categoria | Uso |
|-----------|-------|-----------|-----|
| `Validation_Required` | 1001 | Validation | Campo obligatorio vacio |
| `Validation_MaxLength` | 1002 | Validation | Longitud maxima excedida |
| `Validation_MinLength` | 1003 | Validation | Longitud minima no alcanzada |
| `Validation_InvalidFormat` | 1004 | Validation | Formato invalido (passwords no coinciden) |
| `Validation_InvalidEmail` | 1005 | Validation | Formato de email invalido |
| `Validation_InvalidUrl` | 1006 | Validation | Formato de URL invalido |
| `Validation_DuplicateEmail` | 1009 | Validation | Email duplicado |
| `NotFound_Artista` | 2002 | NotFound | Artista no encontrado |
| `Auth_Unauthorized` | 3001 | Auth | No autorizado o token invalido |
| `BusinessRule_ArtistaAlreadyExists` | 4008 | BusinessRule | Usuario ya tiene perfil artista |
| `Internal_UnexpectedError` | 5000 | Internal | Error inesperado |

---

### 7.2 Estructura de ServiceResponse

**Todas las responses siguen este patron:**

```csharp
public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public List<ServiceResponseMessage> Messages { get; set; } = new();
    public bool IsSuccess => !Messages.Any(m => m.IsError);
}

public class ServiceResponseMessage
{
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public HttpStatusCode HttpStatusCode { get; set; }
    public bool IsError => !string.IsNullOrEmpty(ErrorCode) && !ErrorCode.StartsWith("0");
}
```

---

### 7.3 Ejemplos de Respuestas de Error

**Validacion (400 Bad Request):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "El nombre artistico es obligatorio",
      "errorCode": "1001",
      "httpStatusCode": 400
    }
  ]
}
```

**Email Duplicado (409 Conflict):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Este email ya esta registrado",
      "errorCode": "1009",
      "httpStatusCode": 409
    }
  ]
}
```

**Artista No Encontrado (404 Not Found):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Artista no encontrado",
      "errorCode": "2002",
      "httpStatusCode": 404
    }
  ]
}
```

**No Autorizado (401 Unauthorized):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Token invalido o expirado",
      "errorCode": "3001",
      "httpStatusCode": 401
    }
  ]
}
```

---

## 8. Autorizacion y JWT

### 8.1 Endpoints Publicos vs Protegidos

| Endpoint | Autorizacion | Atributo | Extraccion UserId |
|----------|--------------|----------|-------------------|
| POST /api/auth/register | Publico | `[AllowAnonymous]` | N/A |
| POST /api/artistas | Protegido | `[Authorize]` | `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` |
| GET /api/artistas/{id} | Publico | `[AllowAnonymous]` | N/A |
| GET /api/artistas/by-user/{userId} | Protegido | `[Authorize]` | `User.FindFirst(ClaimTypes.NameIdentifier)?.Value` |

---

### 8.2 Claims JWT Requeridos

**Estructura del Token:**
```json
{
  "sub": "userId (GUID)",
  "email": "usuario@example.com",
  "role": ["Fan", "Artista"],
  "exp": 1738000000,
  "iat": 1737913600,
  "iss": "WePlayRises",
  "aud": "WePlayRisesClient"
}
```

**Configuracion:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

---

### 8.3 Extraccion de UserId en Controllers

**Patron Utilizado (ArtistasController):**
```csharp
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

command.UserId = userId;  // Populate command with authenticated user
var result = await _mediator.Send(command);
```

---

## 9. Estructura de Archivos

```
Modules/UserAccess/
├── WePlayRises.UserAccess.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs    [IMPLEMENTADO]
│
├── WePlayRises.UserAccess.Application/
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   └── RegisterCommand.cs       [IMPLEMENTADO] (Command + Handler)
│   │   │   └── Validators/
│   │   │       └── RegisterCommandValidator.cs [IMPLEMENTADO]
│   │   │
│   │   └── Artistas/
│   │       ├── Commands/
│   │       │   └── CreateArtistaCommand.cs  [IMPLEMENTADO] (Command + Handler)
│   │       ├── Queries/
│   │       │   ├── GetArtistaByIdQuery.cs   [IMPLEMENTADO] (Query + Handler)
│   │       │   └── GetArtistaByUserIdQuery.cs [IMPLEMENTADO] (Query + Handler)
│   │       └── Validators/
│   │           └── CreateArtistaCommandValidator.cs [IMPLEMENTADO]
│   │
│   ├── Dtos/
│   │   ├── RegisterResponseDto.cs           [IMPLEMENTADO]
│   │   ├── ArtistaDto.cs                    [IMPLEMENTADO]
│   │   └── ArtistaListDto.cs                [IMPLEMENTADO]
│   │
│   └── Mapping/
│       ├── AuthProfile.cs                   [IMPLEMENTADO]
│       └── ArtistaProfile.cs                [IMPLEMENTADO]
│
└── WePlayRises.UserAccess.WebApi/
    └── Controllers/
        ├── AuthController.cs                [IMPLEMENTADO]
        └── ArtistasController.cs            [IMPLEMENTADO]
```

---

## 10. Checklist de Contratos API

### Implementacion Actual

- [x] RegisterCommand + Handler retorna ServiceResponse<RegisterResponseDto>
- [x] CreateArtistaCommand + Handler retorna ServiceResponse<ArtistaDto>
- [x] GetArtistaByIdQuery + Handler retorna ServiceResponse<ArtistaDto>
- [x] GetArtistaByUserIdQuery + Handler retorna ServiceResponse<ArtistaDto>
- [x] Handlers inyectan Services (no DbContext)
- [x] Validators usan ServiceResponseMessageType constants (no strings literales)
- [x] Validators incluyen WithMessage + WithErrorCode
- [x] AutoMapper profiles registrados (ArtistaProfile, AuthProfile)
- [x] Controllers con documentacion Swagger completa
- [x] Manejo de errores con try-catch y logging
- [x] Constructores con ?? throw new ArgumentNullException
- [x] Validacion retorna ServiceResponse (no throw)

### Pendiente (Fuera del Scope de esta Feature)

- [ ] PUT /api/artistas/{id} - Actualizar perfil artista
- [ ] GET /api/artistas - Listar todos los artistas (paginado)
- [ ] DELETE /api/artistas/{id} - Eliminar perfil artista
- [ ] PATCH /api/artistas/{id}/imagen - Subir imagen de perfil
- [ ] UpdateArtistaCommand + Validator
- [ ] Validacion asincrona de unicidad de NombreArtistico
- [ ] Tests unitarios de Validators
- [ ] Tests de integracion de endpoints

---

## 11. Notas de Implementacion

### 11.1 Puntos Clave de CQRS

1. **Handler + Command en Mismo Archivo:** Todos los Commands/Queries tienen su Handler en el mismo archivo (.cs).
2. **ServiceResponse<T> Obligatorio:** Todas las responses estan wrapped en `ServiceResponse<T>`.
3. **Services Retornan Entidades:** `IArtistaService` retorna entidades `Artista`, NO DTOs. El mapping a DTO se hace en el Handler.
4. **Validators con Constants:** Todos los validators usan `ServiceResponseMessageType.X` en lugar de strings literales.
5. **Logger SIEMPRE Inyectado:** Todos los handlers incluyen `ILogger<T>` y lo usan en catch blocks.

---

### 11.2 Diferencias con el Contrato Original

| Aspecto | contracts.md | Implementacion Real | Notas |
|---------|--------------|---------------------|-------|
| RegisterResponse | Incluye `roles: []` | Incluye `Roles: List<string>` | Implementacion mas completa |
| CreateArtista UserId | No mencionado explicitamente | `UserId` propiedad en Command | Poblado desde JWT en controller |
| ErrorCodes | Strings literales | Constants numericas | Mas robusto (1001, 2002, etc.) |
| Validacion ImagenUrl | Mencionada | Custom validator `BeValidUrl()` | Implementacion completa |
| AutoMapper | No detallado | Profile completo con `.ForMember()` | Mapea campos especificos |

---

### 11.3 Consideraciones de Seguridad

1. **UserId NUNCA viene del Body:** El `UserId` se extrae del token JWT en el controller, nunca se confia en el valor enviado por el cliente.
2. **Autorizacion en GetByUserId:** El handler valida que `RequestingUserId` (del token) coincida con `UserId` (del route).
3. **Passwords en Transit:** Las passwords se envian en HTTPS (configurado en produccion).
4. **Token Expiration:** JWT tokens expiran en 24 horas. El frontend debe refrescar o solicitar re-login.

---

### 11.4 Optimizaciones Futuras (No Bloqueantes)

1. **Caching:** Implementar `IRequestCacheService` en `ArtistaService` para evitar queries duplicados en Validator + Handler.
2. **Upload de Imagenes:** Reemplazar `ImagenUrl` string por upload real a Azure Blob Storage.
3. **Validacion Asincrona:** Verificar unicidad de `NombreArtistico` en tiempo real durante validacion.
4. **DTOs Separados:** Crear `CreateArtistaRequestDto` separado de `CreateArtistaCommand` para desacoplar API de Application layer.
5. **Paginacion:** Implementar `GetAllArtistasQuery` con paginacion para escalabilidad.

---

## 12. Proximos Pasos

### Para Implementacion de PUT /api/artistas/{id}

1. **Crear UpdateArtistaCommand + Handler:**
   - Archivo: `Features/Artistas/Commands/UpdateArtistaCommand.cs`
   - Validar que `UserId` del token coincida con `UserIdPropietario` de la entidad
   - Retornar `ServiceResponse<ArtistaDto>`

2. **Crear UpdateArtistaCommandValidator:**
   - Archivo: `Features/Artistas/Validators/UpdateArtistaCommandValidator.cs`
   - Mismas reglas que `CreateArtistaCommandValidator` excepto `UserId` (ya validado en handler)

3. **Actualizar ArtistaProfile:**
   - Agregar `CreateMap<UpdateArtistaCommand, Artista>()`

4. **Agregar Endpoint en ArtistasController:**
   - `[HttpPut("{id}")]`
   - `[Authorize]`
   - Extraer `UserId` del token
   - Validar permisos

5. **Agregar Swagger Documentation:**
   - Documentar request/response
   - Status codes: 200, 400, 401, 403, 404, 500

---

## 13. Referencias

- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`
- **Contracts Spec:** `docs/user-stories/registro-artista/contracts.md`
- **Feature Spec:** `docs/user-stories/registro-artista/feature-spec.md`
- **ServiceResponse Definition:** `BuildingBlocks/Kernel/Http/Response/ServiceResponse.cs`
- **ErrorCodes Constants:** `Modules/UserAccess/UserAccess.Domain/Constants/ServiceResponseMessageType.cs`
