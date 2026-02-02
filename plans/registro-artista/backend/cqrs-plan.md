# Plan CQRS: Registro de Artista

**Fecha:** 2026-01-26
**Modulo:** UserAccess
**Feature:** registro-artista

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response |
|-----------|------|---------|----------|
| Registrar usuario | Command | RegisterCommand | ServiceResponse&lt;RegisterResponseDto&gt; |
| Crear perfil artista | Command | CreateArtistaCommand | ServiceResponse&lt;ArtistaDto&gt; |
| Obtener artista por ID | Query | GetArtistaByIdQuery | ServiceResponse&lt;ArtistaDto&gt; |
| Obtener artista por UserId | Query | GetArtistaByUserIdQuery | ServiceResponse&lt;ArtistaDto&gt; |

---

## 2. Commands

### 2.1 RegisterCommand

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Email | string | Email del usuario (formato email) |
| Password | string | Contraseña (minimo 8 caracteres) |
| ConfirmPassword | string | Confirmacion de contraseña |

**Implementa:** `IRequest<ServiceResponse<RegisterResponseDto>>`

**Request Body JSON:**
```json
{
  "email": "artista@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

#### Handler

**Clase:** `RegisterCommandHandler`

**Dependencias:**
- `UserManager<IdentityUser>` - Para crear usuario en Identity
- `IValidator<RegisterCommand>` - Para validacion con FluentValidation
- `IJwtTokenGenerator` - Para generar token JWT
- `IMapper` - Para mapear IdentityUser -> RegisterResponseDto
- `ILogger<RegisterCommandHandler>` - Para logging

**Flujo Detallado:**

1. **Validacion con FluentValidation**
   ```csharp
   var validationResult = await _validator.ValidateAsync(request, ct);
   if (!validationResult.IsValid)
   {
       return new ServiceResponse<RegisterResponseDto>
       {
           Messages = validationResult.GetServiceResponseMessages()
       };
   }
   ```

2. **Verificar email duplicado (logica de negocio en Handler)**
   ```csharp
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

3. **Crear usuario con UserManager**
   ```csharp
   var user = new IdentityUser
   {
       UserName = request.Email,
       Email = request.Email
   };

   var result = await _userManager.CreateAsync(user, request.Password);
   ```

4. **Manejar errores de Identity**
   ```csharp
   if (!result.Succeeded)
   {
       return new ServiceResponse<RegisterResponseDto>
       {
           Messages = result.Errors.Select(e => new ServiceResponseMessage
           {
               Message = e.Description,
               ErrorCode = "AUTH_IDENTITY_ERROR"
           }).ToList()
       };
   }
   ```

5. **Generar token JWT**
   ```csharp
   var token = _jwtGenerator.GenerateToken(user.Id, user.Email!);
   ```

6. **Mapear y retornar respuesta**
   ```csharp
   var response = _mapper.Map<RegisterResponseDto>(user);
   response.Token = token;

   return new ServiceResponse<RegisterResponseDto>
   {
       Data = response,
       Messages = new()
       {
           new()
           {
               Message = "Usuario registrado exitosamente",
               ErrorCode = "SUCCESS"
           }
       }
   };
   ```

7. **Try-catch con logging**
   ```csharp
   try
   {
       // Flujo completo
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error registering user with email {Email}", request.Email);
       return new ServiceResponse<RegisterResponseDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Error inesperado al crear cuenta",
                   ErrorCode = "ERROR_UNEXPECTED"
               }
           }
       };
   }
   ```

**Response Exitoso:**
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
      "errorCode": "SUCCESS"
    }
  ]
}
```

---

### 2.2 CreateArtistaCommand

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Commands/CreateArtistaCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| NombreArtistico | string | Nombre artistico (maximo 200 caracteres) |
| Descripcion | string? | Biografia opcional (maximo 2000 caracteres) |
| Pais | string? | Pais de origen (maximo 100 caracteres) |
| Ciudad | string? | Ciudad de residencia (maximo 100 caracteres) |
| ImagenUrl | string? | URL de imagen de perfil (formato URL valido) |
| UserId | string? | ID del usuario (extraido del token JWT, NO del body) |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**Request Body JSON:**
```json
{
  "nombreArtistico": "Los Rockeros",
  "descripcion": "Banda de rock alternativo de Madrid",
  "pais": "España",
  "ciudad": "Madrid",
  "imagenUrl": "https://example.com/artista.jpg"
}
```

**CRITICO:** `UserId` NO viene en el request body. Se asigna en el Controller tras extraerlo del token JWT:
```csharp
// En Controller
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
command.UserId = userId;
```

#### Handler

**Clase:** `CreateArtistaCommandHandler`

**Dependencias:**
- `IArtistaService` - Para crear artista (NO inyectar DbContext)
- `IMapper` - Para mappings Command -> Entity, Entity -> DTO
- `IValidator<CreateArtistaCommand>` - Para validacion
- `ILogger<CreateArtistaCommandHandler>` - Para logging

**Flujo Detallado:**

1. **Validacion con FluentValidation**
   ```csharp
   var validationResult = await _validator.ValidateAsync(request, ct);
   if (!validationResult.IsValid)
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = validationResult.GetServiceResponseMessages()
       };
   }
   ```

2. **Validacion de UserId presente**
   ```csharp
   if (string.IsNullOrEmpty(request.UserId))
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "UserId es requerido (debe estar autenticado)",
                   ErrorCode = "AUTH_UNAUTHORIZED"
               }
           }
       };
   }
   ```

3. **Verificar si usuario ya tiene perfil (logica de negocio en Handler)**
   ```csharp
   var existingArtista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
   if (existingArtista != null)
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Este usuario ya tiene un perfil de artista",
                   ErrorCode = "ARTISTA_ALREADY_EXISTS"
               }
           }
       };
   }
   ```

4. **Mapear Command a Entidad**
   ```csharp
   var entity = _mapper.Map<Artista>(request);
   ```

5. **Aplicar logica de negocio (en Handler, NO en Service)**
   ```csharp
   // FechaCreacion se asigna en Service.CreateAsync()
   // entity.FechaCreacion = DateTime.UtcNow; <- NO aqui, en Service
   ```

6. **Persistir via Service**
   ```csharp
   var artistaId = await _artistaService.CreateAsync(entity, ct);
   ```

7. **Obtener entidad creada y mapear a DTO**
   ```csharp
   var createdArtista = await _artistaService.GetByIdAsync(
       new ArtistaId(artistaId), ct);

   var dto = _mapper.Map<ArtistaDto>(createdArtista);
   ```

8. **Retornar respuesta exitosa**
   ```csharp
   return new ServiceResponse<ArtistaDto>
   {
       Data = dto,
       Messages = new()
       {
           new()
           {
               Message = "Perfil de artista creado exitosamente",
               ErrorCode = "SUCCESS"
           }
       }
   };
   ```

9. **Try-catch con logging**
   ```csharp
   try
   {
       // Flujo completo
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error creating Artista for user {UserId}", request.UserId);
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Error inesperado al crear perfil",
                   ErrorCode = "ERROR_UNEXPECTED"
               }
           }
       };
   }
   ```

**Response Exitoso:**
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
      "errorCode": "SUCCESS"
    }
  ]
}
```

---

## 3. Queries

### 3.1 GetArtistaByIdQuery

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Queries/GetArtistaByIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID del artista |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**URL:** `GET /api/artistas/{id}`

#### Handler

**Clase:** `GetArtistaByIdQueryHandler`

**Dependencias:**
- `IArtistaService` - Para obtener artista
- `IMapper` - Para mapear Entity -> DTO
- `ILogger<GetArtistaByIdQueryHandler>` - Para logging

**Flujo Detallado:**

1. **Llamar service con cache (RequestCache en Service)**
   ```csharp
   var entity = await _artistaService.GetByIdAsync(
       new ArtistaId(request.Id), ct);
   ```

2. **Validar existencia**
   ```csharp
   if (entity == null)
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Artista no encontrado",
                   ErrorCode = "ARTISTA_NOT_FOUND"
               }
           }
       };
   }
   ```

3. **Mapear Entity a DTO**
   ```csharp
   var dto = _mapper.Map<ArtistaDto>(entity);
   ```

4. **Retornar respuesta**
   ```csharp
   return new ServiceResponse<ArtistaDto>
   {
       Data = dto,
       Messages = new()
       {
           new()
           {
               Message = "Artista encontrado",
               ErrorCode = "SUCCESS"
           }
       }
   };
   ```

5. **Try-catch con logging**
   ```csharp
   try
   {
       // Flujo completo
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error getting Artista by ID {ArtistaId}", request.Id);
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Error inesperado",
                   ErrorCode = "ERROR_UNEXPECTED"
               }
           }
       };
   }
   ```

**NO hay validacion previa** (query simple, solo requiere validar que el ID exista).

---

### 3.2 GetArtistaByUserIdQuery

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Queries/GetArtistaByUserIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario de Identity |
| RequestingUserId | string? | UserId del token JWT (para validacion) |

**Implementa:** `IRequest<ServiceResponse<ArtistaDto>>`

**URL:** `GET /api/artistas/by-user/{userId}`

**CRITICO:** `RequestingUserId` se asigna en Controller tras extraer del token:
```csharp
// En Controller
var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
query.RequestingUserId = requestingUserId;
```

#### Handler

**Clase:** `GetArtistaByUserIdQueryHandler`

**Dependencias:**
- `IArtistaService` - Para obtener artista
- `IMapper` - Para mapear Entity -> DTO
- `ILogger<GetArtistaByUserIdQueryHandler>` - Para logging

**Flujo Detallado:**

1. **Validacion de autorizacion (logica en Handler)**
   ```csharp
   if (string.IsNullOrEmpty(request.RequestingUserId) ||
       request.RequestingUserId != request.UserId)
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "No tienes permisos para ver este perfil",
                   ErrorCode = "AUTH_UNAUTHORIZED"
               }
           }
       };
   }
   ```

2. **Llamar service con cache**
   ```csharp
   var entity = await _artistaService.GetByUserIdAsync(request.UserId, ct);
   ```

3. **Validar existencia**
   ```csharp
   if (entity == null)
   {
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Artista no encontrado para este usuario",
                   ErrorCode = "ARTISTA_NOT_FOUND"
               }
           }
       };
   }
   ```

4. **Mapear y retornar**
   ```csharp
   var dto = _mapper.Map<ArtistaDto>(entity);

   return new ServiceResponse<ArtistaDto>
   {
       Data = dto,
       Messages = new()
       {
           new()
           {
               Message = "Artista encontrado",
               ErrorCode = "SUCCESS"
           }
       }
   };
   ```

5. **Try-catch con logging**
   ```csharp
   try
   {
       // Flujo completo
   }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Error getting Artista by UserId {UserId}", request.UserId);
       return new ServiceResponse<ArtistaDto>
       {
           Messages = new()
           {
               new()
               {
                   Message = "Error inesperado",
                   ErrorCode = "ERROR_UNEXPECTED"
               }
           }
       };
   }
   ```

---

## 4. Validators

### 4.1 RegisterCommandValidator

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Auth/Validators/RegisterCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Email | NotEmpty | El email es obligatorio | VALIDATION_REQUIRED |
| Email | EmailAddress | El formato del email no es valido | AUTH_EMAIL_INVALID |
| Password | NotEmpty | La contraseña es obligatoria | VALIDATION_REQUIRED |
| Password | MinimumLength(8) | La contraseña debe tener al menos 8 caracteres | AUTH_PASSWORD_MIN_LENGTH |
| ConfirmPassword | NotEmpty | Confirme su contraseña | VALIDATION_REQUIRED |
| ConfirmPassword | Equal(x => x.Password) | Las contraseñas no coinciden | AUTH_PASSWORD_MISMATCH |

**Implementacion:**

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

**NO incluye validacion de email duplicado** - se hace en Handler porque requiere acceso a `UserManager<IdentityUser>`.

---

### 4.2 CreateArtistaCommandValidator

**Archivo:** `Modules/UserAccess/UserAccess.Application/Features/Artistas/Validators/CreateArtistaCommandValidator.cs`

**Reglas de Validacion:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| NombreArtistico | NotEmpty | El nombre artistico es obligatorio | VALIDATION_REQUIRED |
| NombreArtistico | MaximumLength(200) | El nombre artistico no puede superar los 200 caracteres | ARTISTA_NOMBRE_MAX_LENGTH |
| Descripcion | MaximumLength(2000) | La descripcion no puede superar los 2000 caracteres | ARTISTA_DESC_MAX_LENGTH |
| Pais | MaximumLength(100) | El pais no puede superar los 100 caracteres | ARTISTA_PAIS_MAX_LENGTH |
| Ciudad | MaximumLength(100) | La ciudad no puede superar los 100 caracteres | ARTISTA_CIUDAD_MAX_LENGTH |
| ImagenUrl | Must(BeValidUrl) | La URL de la imagen no es valida. Debe comenzar con http:// o https:// | ARTISTA_IMAGEN_URL_INVALIDA |
| UserId | NotEmpty | El UserId es obligatorio | VALIDATION_REQUIRED |

**Implementacion:**

```csharp
using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;

namespace WePlayRises.UserAccess.Application.Features.Artistas.Validators;

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
            return true; // Permitir vacio/null

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
```

**NO incluye validacion de UserId duplicado** - se hace en Handler via `IArtistaService.GetByUserIdAsync()` para aprovechar caching.

---

## 5. DTOs

### 5.1 RegisterResponseDto

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/RegisterResponseDto.cs`

**Estructura:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario creado (Identity GUID) |
| Email | string | Email del usuario |
| Token | string | Token JWT para autenticacion |

**Codigo:**

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

### 5.2 ArtistaDto

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/ArtistaDto.cs`

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

**Codigo:**

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

### 5.3 ArtistaListDto

**Archivo:** `Modules/UserAccess/UserAccess.Application/Dtos/ArtistaListDto.cs`

**Estructura:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del artista |
| NombreArtistico | string | Nombre artistico publico |
| ImagenUrl | string? | URL de imagen de perfil |
| Ciudad | string? | Ciudad de residencia |
| Pais | string? | Pais de origen |

**Codigo:**

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

**Nota:** DTO simplificado para listados futuros. NO se usa en esta feature, pero se define para consistencia con contratos.

---

## 6. AutoMapper Profiles

### 6.1 ArtistaProfile

**Archivo:** `Modules/UserAccess/UserAccess.Application/Mapping/ArtistaProfile.cs`

**Mappings Definidos:**

| Source | Destination | Transformaciones |
|--------|-------------|------------------|
| CreateArtistaCommand | Artista | UserIdPropietario &lt;- UserId |
| Artista | ArtistaDto | Id.Value -> Id, UserIdPropietario -> UserId |
| Artista | ArtistaListDto | Id.Value -> Id |

**Codigo:**

```csharp
using AutoMapper;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Features.Artistas.Commands;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Mapping;

public class ArtistaProfile : Profile
{
    public ArtistaProfile()
    {
        // Command -> Entity
        CreateMap<CreateArtistaCommand, Artista>()
            .ForMember(dest => dest.UserIdPropietario,
                       opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Id,
                       opt => opt.Ignore()) // Se genera en Domain
            .ForMember(dest => dest.FechaCreacion,
                       opt => opt.Ignore()) // Se asigna en Service
            .ForMember(dest => dest.FechaActualizacion,
                       opt => opt.Ignore());

        // Entity -> DTO (completo)
        CreateMap<Artista, ArtistaDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value)) // ArtistaId -> Guid
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.UserIdPropietario));

        // Entity -> DTO (listado simplificado)
        CreateMap<Artista, ArtistaListDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value));
    }
}
```

**IMPORTANTE:**
- `Artista.Id` es de tipo `ArtistaId` (strongly-typed), se mapea a `Guid` con `.Value`
- `UserIdPropietario` en entidad se mapea a `UserId` en DTO
- Campos `FechaCreacion` y `FechaActualizacion` se ignoran en Command -> Entity (los asigna el Service)

---

### 6.2 AuthProfile

**Archivo:** `Modules/UserAccess/UserAccess.Application/Mapping/AuthProfile.cs`

**Mappings Definidos:**

| Source | Destination | Transformaciones |
|--------|-------------|------------------|
| IdentityUser | RegisterResponseDto | Id -> UserId, Token se asigna manualmente |

**Codigo:**

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
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email,
                       opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token,
                       opt => opt.Ignore()); // Se asigna manualmente en Handler
    }
}
```

---

## 7. Interfaces de Servicios

### 7.1 IJwtTokenGenerator

**Archivo:** `Modules/UserAccess/UserAccess.Application/Interfaces/Services/IJwtTokenGenerator.cs`

**Estructura:**

```csharp
namespace WePlayRises.UserAccess.Application.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email);
}
```

**Implementacion:** Se implementa en `UserAccess.Infra/Services/JwtTokenGenerator.cs`

**Claims JWT Generados:**
- `sub` (ClaimTypes.NameIdentifier): UserId
- `email` (ClaimTypes.Email): Email del usuario
- `exp`: Expiracion (24 horas por defecto)
- `iat`: Issued at (timestamp)

**Configuracion JWT:**
- Algoritmo: HS256
- Issuer: WePlayRises
- Audience: WePlayRisesClient

---

### 7.2 IArtistaService

**Archivo:** `Modules/UserAccess/UserAccess.Application/Interfaces/Services/IArtistaService.cs`

**Estructura:**

```csharp
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Interfaces.Services;

public interface IArtistaService
{
    Task<Artista?> GetByIdAsync(ArtistaId id, CancellationToken ct);
    Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct);
    Task<bool> ExistsForUserAsync(string userId, CancellationToken ct);
    Task<Guid> CreateAsync(Artista entity, CancellationToken ct);
    Task UpdateAsync(Artista entity, CancellationToken ct);
}
```

**CRITICO:**
- Metodos retornan **Entidades** (Artista), NO DTOs
- Service implementa caching con `IRequestCacheService` (ver hexagonal-architecture.md)
- Service NO es inyectado desde Application layer (se define interface, implementacion en Infra)

---

## 8. Archivos a Crear

```
Modules/UserAccess/UserAccess.Application/
├── Features/
│   ├── Auth/
│   │   ├── Commands/
│   │   │   └── RegisterCommand.cs                    # Command + Handler (MISMO archivo)
│   │   └── Validators/
│   │       └── RegisterCommandValidator.cs
│   │
│   └── Artistas/
│       ├── Commands/
│       │   └── CreateArtistaCommand.cs               # Command + Handler (MISMO archivo)
│       ├── Queries/
│       │   ├── GetArtistaByIdQuery.cs                # Query + Handler (MISMO archivo)
│       │   └── GetArtistaByUserIdQuery.cs            # Query + Handler (MISMO archivo)
│       └── Validators/
│           └── CreateArtistaCommandValidator.cs
│
├── Dtos/
│   ├── RegisterResponseDto.cs
│   ├── ArtistaDto.cs
│   └── ArtistaListDto.cs
│
├── Mapping/
│   ├── ArtistaProfile.cs
│   └── AuthProfile.cs
│
└── Interfaces/
    └── Services/
        ├── IJwtTokenGenerator.cs
        └── IArtistaService.cs
```

**Total:** 13 archivos nuevos.

**Estructura de carpetas critica:**
- Commands y Queries contienen Handler en el **MISMO archivo**
- Validators en carpeta **separada** (`Validators/`)
- DTOs en carpeta **separada** (`Dtos/`)
- Mapping Profiles en carpeta **separada** (`Mapping/`)

---

## 9. Patrones Importantes

### 9.1 Logica de Negocio en Handler

**Handler contiene:**
- Validacion con FluentValidation
- Logica de negocio (verificar duplicados, calculos, reglas)
- Orquestacion de llamadas a Services
- Transformaciones con AutoMapper
- Construccion de ServiceResponse

**Service contiene:**
- SOLO persistencia (llamadas a Repository)
- Asignacion de timestamps (FechaCreacion, FechaActualizacion)
- Caching con IRequestCacheService
- NO logica de negocio

**Ejemplo:**
```csharp
// CORRECTO - Verificacion de duplicados en Handler
var existingArtista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
if (existingArtista != null)
{
    return new ServiceResponse<ArtistaDto>
    {
        Messages = new() { new() { Message = "...", ErrorCode = "ARTISTA_ALREADY_EXISTS" } }
    };
}

// INCORRECTO - NO verificar duplicados en Service
// El Service solo hace CRUD, no valida reglas de negocio
```

---

### 9.2 ServiceResponse Siempre

**Exito:**
```csharp
return new ServiceResponse<T>
{
    Data = result,
    Messages = new()
    {
        new() { Message = "Operacion exitosa", ErrorCode = "SUCCESS" }
    }
};
```

**Error de Validacion:**
```csharp
return new ServiceResponse<T>
{
    Messages = validationResult.GetServiceResponseMessages()
};
```

**Error de Negocio:**
```csharp
return new ServiceResponse<T>
{
    Messages = new()
    {
        new()
        {
            Message = "Este usuario ya tiene un perfil de artista",
            ErrorCode = "ARTISTA_ALREADY_EXISTS"
        }
    }
};
```

**Error Inesperado:**
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Error creating Artista for user {UserId}", request.UserId);
    return new ServiceResponse<T>
    {
        Messages = new()
        {
            new()
            {
                Message = "Error inesperado",
                ErrorCode = "ERROR_UNEXPECTED"
            }
        }
    };
}
```

---

### 9.3 Flujo de Caching (ADR-006)

**Problema Sin Cache:**
```
Validator.ExistsForUserAsync(userId)
  -> ArtistaService -> Repository -> DB Query 1

Handler.GetByUserIdAsync(userId)
  -> ArtistaService -> Repository -> DB Query 2 (DUPLICADO)
```

**Solucion Con Cache:**
```
Validator.ExistsForUserAsync(userId)
  -> ArtistaService
     -> RequestCache.GetOrAddAsync("artista:exists:user:{userId}")
        -> Cache MISS -> Repository -> DB Query
        -> Cache almacena resultado

Handler.GetByUserIdAsync(userId)
  -> ArtistaService
     -> RequestCache.GetOrAddAsync("artista:user:{userId}")
        -> Cache HIT -> Retorna inmediato (0 queries)
```

**Beneficio:** Evita queries duplicados a DB durante el mismo request HTTP.

**Cache Keys Consistentes:**
- `artista:{id}` - GetByIdAsync
- `artista:user:{userId}` - GetByUserIdAsync
- `artista:exists:user:{userId}` - ExistsForUserAsync

---

### 9.4 Extraccion de UserId del Token

**En Controller:**
```csharp
[HttpPost]
[Authorize]
public async Task<IActionResult> Create([FromBody] CreateArtistaCommand command)
{
    // Extraer UserId del token JWT
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized(new ServiceResponse<ArtistaDto>
        {
            Messages = new() { new() { Message = "Token invalido", ErrorCode = "AUTH_UNAUTHORIZED" } }
        });
    }

    // Asignar al Command (NO viene del request body)
    command.UserId = userId;

    var result = await _mediator.Send(command);
    // ...
}
```

**En Command:**
```csharp
public class CreateArtistaCommand : IRequest<ServiceResponse<ArtistaDto>>
{
    public string NombreArtistico { get; set; } = null!;
    public string? Descripcion { get; set; }
    // ...

    // Propiedad interna, NO del request body JSON
    public string? UserId { get; set; }
}
```

**CRITICO:** El UserId NUNCA viene del request body. Se extrae del token JWT para prevenir suplantacion.

---

## 10. Codigos de Error

| ErrorCode | Descripcion | HTTP Status |
|-----------|-------------|-------------|
| SUCCESS | Operacion exitosa | 200/201 |
| VALIDATION_REQUIRED | Campo requerido vacio | 400 |
| AUTH_EMAIL_INVALID | Formato de email invalido | 400 |
| AUTH_EMAIL_EXISTS | Email duplicado en Identity | 409 |
| AUTH_PASSWORD_MIN_LENGTH | Password menor a 8 caracteres | 400 |
| AUTH_PASSWORD_MISMATCH | Passwords no coinciden | 400 |
| AUTH_UNAUTHORIZED | Token invalido o expirado | 401 |
| AUTH_IDENTITY_ERROR | Error de ASP.NET Core Identity | 400 |
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

### Commands/Queries
- [ ] RegisterCommand + RegisterCommandHandler (mismo archivo)
- [ ] CreateArtistaCommand + CreateArtistaCommandHandler (mismo archivo)
- [ ] GetArtistaByIdQuery + GetArtistaByIdQueryHandler (mismo archivo)
- [ ] GetArtistaByUserIdQuery + GetArtistaByUserIdQueryHandler (mismo archivo)

### Handlers
- [ ] Handler + Command/Query en MISMO archivo
- [ ] Retornan ServiceResponse&lt;T&gt;
- [ ] NO inyectan DbContext (usan Services)
- [ ] Inyectan IValidator, IMapper, IService, ILogger
- [ ] Incluyen try-catch con logging
- [ ] Validacion retorna ServiceResponse (no throw)
- [ ] Logica de negocio en Handler (duplicados, reglas)

### Validators
- [ ] RegisterCommandValidator (en carpeta Validators/)
- [ ] CreateArtistaCommandValidator (en carpeta Validators/)
- [ ] Todas las reglas con .WithMessage() Y .WithErrorCode()
- [ ] NO validaciones async que requieren DB (hacerlas en Handler)

### DTOs
- [ ] RegisterResponseDto
- [ ] ArtistaDto
- [ ] ArtistaListDto

### AutoMapper Profiles
- [ ] ArtistaProfile (Command -> Entity, Entity -> DTOs)
- [ ] AuthProfile (IdentityUser -> RegisterResponseDto)
- [ ] Profiles separados por entidad (NO mega-profile)

### Interfaces de Servicios
- [ ] IJwtTokenGenerator (Application/Interfaces/Services/)
- [ ] IArtistaService (Application/Interfaces/Services/)
- [ ] Servicios retornan entidades, NO DTOs

### Patrones CQRS
- [ ] Commands modifican estado (Create)
- [ ] Queries solo leen (GetById, GetByUserId)
- [ ] ServiceResponse en todos los retornos
- [ ] Caching via IArtistaService (no en Handler/Validator)

---

## 12. Siguiente Paso Sugerido

Una vez aprobado este plan CQRS:

1. **Implementar Application Layer**
   - Crear Commands/Queries con Handlers
   - Crear Validators
   - Crear DTOs
   - Crear AutoMapper Profiles
   - Crear Interfaces de Servicios

2. **Implementar Infrastructure Layer** (si no esta hecho)
   - Crear ArtistaService (implementacion)
   - Crear JwtTokenGenerator (implementacion)
   - Registrar en DependencyInjection

3. **Implementar WebApi Layer**
   - Crear AuthController
   - Crear ArtistasController
   - Configurar JWT Authentication
   - Registrar Validators en DI

4. **Testing**
   - Unit tests para Handlers
   - Unit tests para Validators
   - Integration tests para endpoints

---

## Referencias

- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`
- **API Contracts:** `plans/registro-artista/backend/api-contracts.md`
- **Hexagonal Architecture:** `plans/registro-artista/backend/hexagonal-architecture.md`
- **Feature Spec:** `docs/user-stories/registro-artista/feature-spec.md`
- **Contracts:** `docs/user-stories/registro-artista/contracts.md`
