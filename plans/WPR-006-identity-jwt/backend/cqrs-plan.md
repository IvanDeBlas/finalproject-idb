# Plan CQRS: Identity JWT Authentication (WPR-006)

**Fecha:** 2026-02-12
**Modulo:** UserAccess
**Feature:** WPR-006-identity-jwt

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Request | Response | Estado |
|-----------|------|---------|----------|--------|
| Registrar usuario | Command | RegisterCommand | ServiceResponse\<RegisterResponseDto\> | ✅ YA EXISTE |
| Login con email/password | Command | LoginCommand | ServiceResponse\<LoginResponseDto\> | ❌ CREAR |
| Obtener usuario actual | Query | GetCurrentUserQuery | ServiceResponse\<UserInfoDto\> | ❌ CREAR |

**Nota:** RegisterCommand ya existe y funciona. Este plan se enfoca en completar Login y GetCurrentUser.

---

## 2. Commands

### 2.1 LoginCommand (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/LoginCommand.cs`

**Contiene:** Command + Handler (mismo archivo)

#### Command
| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| Email | string | Si | Email del usuario |
| Password | string | Si | Contrasena |

**Implementa:** `IRequest<ServiceResponse<LoginResponseDto>>`

#### Handler
**Dependencias a inyectar:**
```csharp
private readonly UserManager<IdentityUser> _userManager;
private readonly SignInManager<IdentityUser> _signInManager;
private readonly IValidator<LoginCommand> _validator;
private readonly IJwtTokenGenerator _jwtGenerator;
private readonly IMapper _mapper;
private readonly ILogger<LoginCommandHandler> _logger;
```

**Constructor con ?? throw (CRITICO):**
```csharp
public LoginCommandHandler(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IValidator<LoginCommand> validator,
    IJwtTokenGenerator jwtGenerator,
    IMapper mapper,
    ILogger<LoginCommandHandler> logger)
{
    _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _jwtGenerator = jwtGenerator ?? throw new ArgumentNullException(nameof(jwtGenerator));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo del Handle:**
```csharp
public async Task<ServiceResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken ct)
{
    try
    {
        // 1. Validar con FluentValidation
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return new ServiceResponse<LoginResponseDto>
            {
                Messages = validationResult.GetServiceResponseMessages()
            };
        }

        // 2. Buscar usuario por email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // NO revelar si el usuario existe o no (seguridad)
            return ValidateExtensions.UnauthorizedServiceResponse<LoginResponseDto>(
                "Email o contrasena incorrectos",
                ServiceResponseMessageType.Auth_InvalidCredentials);
        }

        // 3. Verificar credenciales con SignInManager
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed for user {Email}", request.Email);
            return ValidateExtensions.UnauthorizedServiceResponse<LoginResponseDto>(
                "Email o contrasena incorrectos",
                ServiceResponseMessageType.Auth_InvalidCredentials);
        }

        // 4. Obtener roles del usuario
        var userRoles = await _userManager.GetRolesAsync(user);

        // 5. Generar JWT token con roles
        var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);

        // 6. Mapear a LoginResponseDto
        var response = _mapper.Map<LoginResponseDto>(user);
        response.Token = token;
        response.Roles = userRoles.ToList();

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        // 7. Retornar ServiceResponse exitoso
        return new ServiceResponse<LoginResponseDto>
        {
            Data = response,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Inicio de sesion exitoso",
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error logging in user with email {Email}", request.Email);
        return ValidateExtensions.InternalServerErrorServiceResponse<LoginResponseDto>(
            "Error inesperado al iniciar sesion",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

**Patrones importantes:**
- Usar `SignInManager.CheckPasswordSignInAsync` en lugar de `PasswordHasher` (mejor practica)
- NO revelar si el usuario existe o no (siempre retornar "Email o contrasena incorrectos")
- Lockout deshabilitado en MVP (`lockoutOnFailure: false`)
- Logging de intentos fallidos con Warning
- Logging de exito con Information

### 2.2 RegisterCommand (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Estado:** YA EXISTE - Requiere modificaciones menores

**Cambios necesarios:**

1. **Agregar propiedad Role al Command (opcional):**
```csharp
public class RegisterCommand : IRequest<ServiceResponse<RegisterResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string? Role { get; set; }  // NUEVO - Opcional (default null = Fan)
}
```

2. **Modificar Handler para asignar rol despues de CreateAsync:**

**Antes (linea 73-85):**
```csharp
var result = await _userManager.CreateAsync(user, request.Password);
if (!result.Succeeded)
{
    return new ServiceResponse<RegisterResponseDto>
    {
        Messages = result.Errors.Select(e => new ServiceResponseMessage
        {
            Message = e.Description,
            ErrorCode = "AUTH_IDENTITY_ERROR",
            HttpStatusCode = System.Net.HttpStatusCode.BadRequest
        }).ToList()
    };
}
```

**Despues:**
```csharp
var result = await _userManager.CreateAsync(user, request.Password);
if (!result.Succeeded)
{
    return new ServiceResponse<RegisterResponseDto>
    {
        Messages = result.Errors.Select(e => new ServiceResponseMessage
        {
            Message = e.Description,
            ErrorCode = "AUTH_IDENTITY_ERROR",
            HttpStatusCode = System.Net.HttpStatusCode.BadRequest
        }).ToList()
    };
}

// 3.1 Asignar rol por defecto (Fan si no se especifica)
var roleToAssign = string.IsNullOrEmpty(request.Role) ? Roles.Fan : request.Role;
await _userManager.AddToRoleAsync(user, roleToAssign);

_logger.LogInformation("User {UserId} registered with role {Role}", user.Id, roleToAssign);
```

3. **Modificar generacion de token para incluir roles (linea 88):**

**Antes:**
```csharp
var token = _jwtGenerator.GenerateToken(user.Id, user.Email!);
```

**Despues:**
```csharp
// 4.1 Obtener roles del usuario
var userRoles = await _userManager.GetRolesAsync(user);

// 4.2 Generar JWT token con roles
var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);
```

4. **Actualizar mapping para incluir roles en response:**

**Antes (linea 91-92):**
```csharp
var response = _mapper.Map<RegisterResponseDto>(user);
response.Token = token;
```

**Despues:**
```csharp
var response = _mapper.Map<RegisterResponseDto>(user);
response.Token = token;
response.Roles = userRoles.ToList();
```

**Resumen de cambios:**
- +1 propiedad al Command (Role)
- +3 lineas para asignar rol
- +2 lineas para obtener roles antes de generar token
- Modificar 1 linea (llamada a GenerateToken con roles)
- +1 linea para incluir roles en response

---

## 3. Queries

### 3.1 GetCurrentUserQuery (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Queries/GetCurrentUserQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

#### Query
| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | ID del usuario (extraido del claim NameIdentifier) |

**Implementa:** `IRequest<ServiceResponse<UserInfoDto>>`

#### Handler
**Dependencias a inyectar:**
```csharp
private readonly UserManager<IdentityUser> _userManager;
private readonly IMapper _mapper;
private readonly ILogger<GetCurrentUserQueryHandler> _logger;
```

**Constructor con ?? throw (CRITICO):**
```csharp
public GetCurrentUserQueryHandler(
    UserManager<IdentityUser> userManager,
    IMapper mapper,
    ILogger<GetCurrentUserQueryHandler> logger)
{
    _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Flujo del Handle:**
```csharp
public async Task<ServiceResponse<UserInfoDto>> Handle(GetCurrentUserQuery request, CancellationToken ct)
{
    try
    {
        // 1. Buscar usuario por ID
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", request.UserId);
            return ValidateExtensions.NotFoundServiceResponse<UserInfoDto>(
                "Usuario no encontrado",
                ServiceResponseMessageType.NotFound_User);
        }

        // 2. Obtener roles del usuario
        var userRoles = await _userManager.GetRolesAsync(user);

        // 3. Mapear a UserInfoDto
        var userInfo = _mapper.Map<UserInfoDto>(user);
        userInfo.Roles = userRoles.ToList();

        // 4. Retornar ServiceResponse exitoso
        return new ServiceResponse<UserInfoDto>
        {
            Data = userInfo,
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Usuario recuperado exitosamente",
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                }
            }
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user info for UserId {UserId}", request.UserId);
        return ValidateExtensions.InternalServerErrorServiceResponse<UserInfoDto>(
            "Error inesperado al obtener informacion del usuario",
            ServiceResponseMessageType.Internal_UnexpectedError);
    }
}
```

**Patrones importantes:**
- Query simple (solo lectura, no modifica estado)
- No requiere validator (UserId viene del token autenticado)
- Siempre incluir roles en la respuesta

---

## 4. Validators

### 4.1 LoginCommandValidator (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/LoginCommandValidator.cs`

**CRITICO:** Usar `ServiceResponseMessageType` constants, NO strings literales

```csharp
using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
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
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

**Notas importantes:**
- NO validar longitud minima de password en Login (solo en Register)
- La validacion de credenciales se hace en el Handler con SignInManager
- Siempre `.WithMessage()` Y `.WithErrorCode()` juntos

| Campo | Regla | Mensaje | ErrorCode (Constant) |
|-------|-------|---------|----------------------|
| Email | NotEmpty | El email es obligatorio | `ServiceResponseMessageType.Validation_Required` |
| Email | EmailAddress | El formato del email no es valido | `ServiceResponseMessageType.Validation_InvalidEmail` |
| Password | NotEmpty | La contrasena es obligatoria | `ServiceResponseMessageType.Validation_Required` |

### 4.2 RegisterCommandValidator (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/RegisterCommandValidator.cs`

**Estado:** YA EXISTE - Requiere agregar validacion para Role

**Agregar al final del constructor:**
```csharp
RuleFor(x => x.Role)
    .Must(role => string.IsNullOrEmpty(role) || Roles.IsValid(role))
    .WithMessage("Rol invalido. Roles permitidos: Artista, Fan, Admin")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat)
    .When(x => !string.IsNullOrEmpty(x.Role));  // Solo validar si Role no es null/empty
```

**Nota:** Requiere agregar metodo helper a la clase Roles:
```csharp
// En Roles.cs
public static bool IsValid(string role) => new[] { Artista, Fan, Admin }.Contains(role);
```

---

## 5. DTOs

### 5.1 LoginResponseDto (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/LoginResponseDto.cs`

```csharp
namespace WePlayRises.UserAccess.Application.Dtos;

public class LoginResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
```

**Nota:** Estructura identica a RegisterResponseDto pero con nombre semantico para Login.

### 5.2 UserInfoDto (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/UserInfoDto.cs`

```csharp
namespace WePlayRises.UserAccess.Application.Dtos;

public class UserInfoDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; }
}
```

**Diferencias con LoginResponseDto:**
- NO incluye Token (ya lo tiene el usuario autenticado)
- Incluye EmailConfirmed (para mostrar estado de verificacion)

### 5.3 RegisterResponseDto (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/RegisterResponseDto.cs`

**Antes:**
```csharp
public class RegisterResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
```

**Despues:**
```csharp
public class RegisterResponseDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public List<string> Roles { get; set; } = new();  // NUEVO
}
```

---

## 6. AutoMapper Profiles

### 6.1 AuthProfile (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Mapping/AuthProfile.cs`

**Estado:** Verificar si existe. Si no, crear.

**Agregar mappings:**
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
        CreateMap<IdentityUser, RegisterResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token, opt => opt.Ignore())  // Token se asigna en Handler
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles se asignan en Handler

        // IdentityUser -> LoginResponseDto
        CreateMap<IdentityUser, LoginResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // IdentityUser -> UserInfoDto
        CreateMap<IdentityUser, UserInfoDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());
    }
}
```

**Notas:**
- Roles se obtienen via `UserManager.GetRolesAsync()` en Handlers, NO en AutoMapper
- Token se genera via `IJwtTokenGenerator` en Handlers, NO en AutoMapper

---

## 7. Services/Interfaces

### 7.1 IJwtTokenGenerator (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IJwtTokenGenerator.cs`

**Antes:**
```csharp
public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email);
}
```

**Despues:**
```csharp
public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IList<string> roles);
}
```

**Cambios:**
- Agregar parametro `roles` de tipo `IList<string>`
- Los roles se incluiran como claims `ClaimTypes.Role` en el token

### 7.2 JwtTokenGenerator (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/JwtTokenGenerator.cs`

**Modificar metodo GenerateToken:**

**Antes (linea 19-46):**
```csharp
public string GenerateToken(string userId, string email)
{
    var jwtKey = _configuration["Jwt:Key"] ?? "WePlayRisesDefaultSecretKey123456789";
    var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WePlayRises";
    var jwtAudience = _configuration["Jwt:Audience"] ?? "WePlayRisesClient";
    var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 24;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Email, email),
        new(JwtRegisteredClaimNames.Sub, userId),
        new(JwtRegisteredClaimNames.Email, email),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(expirationHours),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Despues:**
```csharp
public string GenerateToken(string userId, string email, IList<string> roles)
{
    var jwtKey = _configuration["Jwt:Key"] ?? "WePlayRisesDefaultSecretKey123456789";
    var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WePlayRises";
    var jwtAudience = _configuration["Jwt:Audience"] ?? "WePlayRisesClient";
    var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 24;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Email, email),
        new(JwtRegisteredClaimNames.Sub, userId),
        new(JwtRegisteredClaimNames.Email, email),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    // NUEVO: Agregar roles como claims
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(expirationHours),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Cambios:**
- Firma del metodo incluye parametro `roles`
- Agregar foreach para incluir cada rol como `ClaimTypes.Role` claim

**Ejemplo de token decodificado (jwt.io):**
```json
{
  "nameid": "abc123-def456",
  "email": "artista@weplay.com",
  "sub": "abc123-def456",
  "jti": "unique-guid",
  "role": "Fan",
  "exp": 1707789600,
  "iss": "WePlayRises",
  "aud": "WePlayRisesClient"
}
```

---

## 8. Constants

### 8.1 Roles (NUEVO)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/Roles.cs`

```csharp
namespace WePlayRises.UserAccess.Domain.Constants;

/// <summary>
/// Constantes de roles del sistema.
/// Usar SIEMPRE estas constantes al asignar roles en Identity.
/// </summary>
public static class Roles
{
    public const string Artista = "Artista";
    public const string Fan = "Fan";
    public const string Admin = "Admin";

    /// <summary>
    /// Todos los roles validos del sistema.
    /// </summary>
    public static readonly string[] All = { Artista, Fan, Admin };

    /// <summary>
    /// Valida si un rol es valido.
    /// </summary>
    public static bool IsValid(string role) => All.Contains(role);
}
```

**Uso en el codigo:**
```csharp
// CORRECTO - Usar constantes
await _userManager.AddToRoleAsync(user, Roles.Fan);

// INCORRECTO - String literal
await _userManager.AddToRoleAsync(user, "Fan");  // NO HACER
```

### 8.2 ServiceResponseMessageType (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar despues de la linea 48 (Auth_UserNotAuthenticated):**

```csharp
// Seccion AUTHENTICATION/AUTHORIZATION ERRORS (3000-3999)
public const string Auth_Unauthorized = "3001";
public const string Auth_Forbidden = "3002";
public const string Auth_TokenExpired = "3003";
public const string Auth_InvalidToken = "3004";
public const string Auth_UserNotAuthenticated = "3005";
public const string Auth_InvalidCredentials = "3006";  // NUEVO
public const string Auth_UserNotFound = "3007";        // NUEVO
public const string Auth_AccountLocked = "3008";       // NUEVO
```

**Uso:**
| Constante | Codigo | Uso |
|-----------|--------|-----|
| `Auth_InvalidCredentials` | 3006 | Login con email/password incorrectos |
| `Auth_UserNotFound` | 3007 | Usuario no existe (solo para logs, NO retornar al cliente) |
| `Auth_AccountLocked` | 3008 | Cuenta bloqueada por intentos fallidos (futuro MVP+) |

---

## 9. Controllers

### 9.1 AuthController (MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

**Agregar endpoint POST /login:**

```csharp
/// <summary>
/// Authenticate user with email and password.
/// </summary>
/// <param name="command">Login credentials (email, password)</param>
/// <returns>User information and JWT token</returns>
/// <response code="200">Login successful</response>
/// <response code="400">Validation errors</response>
/// <response code="401">Invalid credentials</response>
/// <response code="500">Internal server error</response>
[HttpPost("login")]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<LoginResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Login([FromBody] LoginCommand command)
{
    var result = await _mediator.Send(command);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            "3006" or "3007" => Unauthorized(result),  // InvalidCredentials, UserNotFound
            "3008" => StatusCode(423, result),         // AccountLocked (HTTP 423 Locked)
            "1001" or "1005" => BadRequest(result),    // Validation errors
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Agregar endpoint GET /me:**

```csharp
/// <summary>
/// Get current authenticated user information.
/// </summary>
/// <returns>User information and roles</returns>
/// <response code="200">User information retrieved successfully</response>
/// <response code="401">User not authenticated</response>
/// <response code="404">User not found</response>
/// <response code="500">Internal server error</response>
[HttpGet("me")]
[Authorize]  // CRITICO - Requiere autenticacion
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetCurrentUser()
{
    // Extraer UserId del claim NameIdentifier
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
    {
        return Unauthorized(new ServiceResponse<UserInfoDto>
        {
            Messages = new List<ServiceResponseMessage>
            {
                new()
                {
                    Message = "Usuario no autenticado",
                    ErrorCode = ServiceResponseMessageType.Auth_UserNotAuthenticated,
                    HttpStatusCode = HttpStatusCode.Unauthorized
                }
            }
        });
    }

    var query = new GetCurrentUserQuery { UserId = userId };
    var result = await _mediator.Send(query);

    if (!result.IsSuccess)
    {
        var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
        return errorCode switch
        {
            "2001" => NotFound(result),  // NotFound_User
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**Imports necesarios:**
```csharp
using System.Security.Claims;
using WePlayRises.UserAccess.Application.Features.Auth.Queries;
using WePlayRises.UserAccess.Application.Dtos;
```

---

## 10. Infrastructure - Seed Roles

### 10.1 Program.cs (MODIFICAR)

**Archivo:** `src/api/WebApi/Program.cs`

**Agregar despues de `var app = builder.Build();` y antes de configurar middleware:**

```csharp
// Seed roles - Ejecuta solo una vez al iniciar app
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { Roles.Artista, Roles.Fan, Roles.Admin };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            app.Logger.LogInformation("Role {Role} created", role);
        }
    }
}
```

**Imports necesarios:**
```csharp
using Microsoft.AspNetCore.Identity;
using WePlayRises.UserAccess.Domain.Constants;
```

**Proposito:**
- Crear roles Artista, Fan, Admin en la DB al iniciar la aplicacion
- Solo crea roles si no existen (idempotente)
- Loggea cada rol creado para debugging

---

## 11. Archivos a Crear/Modificar

### 11.1 NUEVOS (7 archivos)

```
Modules/UserAccess/WePlayRises.UserAccess.Domain/
└── Constants/
    └── Roles.cs                                                     # NUEVO

Modules/UserAccess/WePlayRises.UserAccess.Application/
├── Features/Auth/
│   ├── Commands/
│   │   └── LoginCommand.cs                                         # NUEVO (Command + Handler)
│   ├── Queries/
│   │   └── GetCurrentUserQuery.cs                                  # NUEVO (Query + Handler)
│   └── Validators/
│       └── LoginCommandValidator.cs                                # NUEVO
│
└── Dtos/
    ├── LoginResponseDto.cs                                         # NUEVO
    └── UserInfoDto.cs                                              # NUEVO
```

### 11.2 MODIFICAR (8 archivos)

```
Modules/UserAccess/WePlayRises.UserAccess.Domain/
└── Constants/
    └── ServiceResponseMessageType.cs                               # +3 constantes Auth

Modules/UserAccess/WePlayRises.UserAccess.Application/
├── Features/Auth/
│   ├── Commands/
│   │   └── RegisterCommand.cs                                      # +8 lineas (rol + roles en token)
│   └── Validators/
│       └── RegisterCommandValidator.cs                             # +4 lineas (validar Role)
│
├── Dtos/
│   └── RegisterResponseDto.cs                                      # +1 propiedad (Roles)
│
├── Interfaces/Services/
│   └── IJwtTokenGenerator.cs                                       # Firma metodo (+ roles param)
│
└── Mapping/
    └── AuthProfile.cs                                              # +3 mappings (verificar si existe, crear si no)

Modules/UserAccess/WePlayRises.UserAccess.Infra/
└── Services/
    └── JwtTokenGenerator.cs                                        # +5 lineas (foreach roles)

Modules/UserAccess/WePlayRises.UserAccess.WebApi/
└── Controllers/
    └── AuthController.cs                                           # +2 metodos (Login, GetCurrentUser)
```

### 11.3 WebApi (1 archivo)

```
src/api/WebApi/
└── Program.cs                                                      # +12 lineas (seed roles)
```

---

## 12. Orden de Implementacion

**IMPORTANTE:** Implementar en este orden para evitar errores de compilacion.

### Fase 1: Domain Layer (5 min)
1. ✅ Crear `Roles.cs`
2. ✅ Modificar `ServiceResponseMessageType.cs` (agregar 3 constantes Auth)

### Fase 2: Application Layer - DTOs (3 min)
3. ✅ Crear `LoginResponseDto.cs`
4. ✅ Crear `UserInfoDto.cs`
5. ✅ Modificar `RegisterResponseDto.cs` (agregar propiedad Roles)

### Fase 3: Application Layer - Interfaces (2 min)
6. ✅ Modificar `IJwtTokenGenerator.cs` (agregar parametro roles)

### Fase 4: Infrastructure Layer - Services (5 min)
7. ✅ Modificar `JwtTokenGenerator.cs` (incluir roles en claims)

### Fase 5: Application Layer - Commands/Queries (20 min)
8. ✅ Crear `LoginCommand.cs` (Command + Handler)
9. ✅ Crear `GetCurrentUserQuery.cs` (Query + Handler)
10. ✅ Modificar `RegisterCommand.cs` (asignar rol + incluir roles en token)

### Fase 6: Application Layer - Validators (5 min)
11. ✅ Crear `LoginCommandValidator.cs`
12. ✅ Modificar `RegisterCommandValidator.cs` (validar Role)

### Fase 7: Application Layer - Mapping (3 min)
13. ✅ Verificar si existe `AuthProfile.cs`, crear o modificar con 3 mappings

### Fase 8: WebApi Layer (10 min)
14. ✅ Modificar `AuthController.cs` (agregar Login + GetCurrentUser endpoints)
15. ✅ Modificar `Program.cs` (seed roles)

### Fase 9: Testing Manual (15 min)
16. ✅ Ejecutar migracion Identity (si no existe)
17. ✅ Probar POST /api/auth/register
18. ✅ Probar POST /api/auth/login
19. ✅ Probar GET /api/auth/me con Bearer token
20. ✅ Decodificar token en jwt.io para verificar roles

**Tiempo total estimado:** ~68 minutos (~1.1 horas)

---

## 13. Dependency Injection

### 13.1 DependencyInjection.cs (NO MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`

**Ya registra automaticamente:**
- Validators con `AddValidatorsFromAssembly` (LoginCommandValidator se registra automatico)
- Handlers con `AddMediatR` (LoginCommandHandler se registra automatico)
- `IJwtTokenGenerator` / `JwtTokenGenerator` (ya registrado)
- `UserManager<IdentityUser>` (registrado por Identity)
- `SignInManager<IdentityUser>` (registrado por Identity)
- `RoleManager<IdentityRole>` (registrado por Identity)

**No requiere cambios** - Todos los nuevos validators y handlers se registran automaticamente via reflexion.

---

## 14. Testing Manual - Swagger

### 14.1 Escenario 1: Register + Login

```bash
cd C:\Repos\WePlay_Rises\src\api
dotnet run --project WebApi
```

**Navegar a:** `https://localhost:5001/swagger`

#### Step 1: Register
**POST /api/auth/register**
```json
{
  "email": "artista@test.com",
  "password": "Test1234",
  "confirmPassword": "Test1234"
}
```

**Response esperado (200 OK):**
```json
{
  "data": {
    "userId": "guid-user-id",
    "email": "artista@test.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "roles": ["Fan"]
  },
  "messages": [
    {
      "message": "Usuario registrado exitosamente",
      "errorCode": null,
      "httpStatusCode": 200
    }
  ]
}
```

#### Step 2: Login
**POST /api/auth/login**
```json
{
  "email": "artista@test.com",
  "password": "Test1234"
}
```

**Response esperado (200 OK):**
```json
{
  "data": {
    "userId": "guid-user-id",
    "email": "artista@test.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "roles": ["Fan"]
  },
  "messages": [
    {
      "message": "Inicio de sesion exitoso",
      "errorCode": null,
      "httpStatusCode": 200
    }
  ]
}
```

#### Step 3: Login con credenciales incorrectas
**POST /api/auth/login**
```json
{
  "email": "artista@test.com",
  "password": "WrongPassword"
}
```

**Response esperado (401 Unauthorized):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Email o contrasena incorrectos",
      "errorCode": "3006",
      "httpStatusCode": 401
    }
  ]
}
```

### 14.2 Escenario 2: Get Current User

#### Step 1: Obtener token
Ejecutar POST /api/auth/login y copiar el token del response.

#### Step 2: GET /me sin token
**GET /api/auth/me** (sin header Authorization)

**Response esperado (401 Unauthorized):**
```json
{
  "data": null,
  "messages": [
    {
      "message": "Usuario no autenticado",
      "errorCode": "3005",
      "httpStatusCode": 401
    }
  ]
}
```

#### Step 3: GET /me con token
**GET /api/auth/me**
**Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response esperado (200 OK):**
```json
{
  "data": {
    "userId": "guid-user-id",
    "email": "artista@test.com",
    "roles": ["Fan"],
    "emailConfirmed": false
  },
  "messages": [
    {
      "message": "Usuario recuperado exitosamente",
      "errorCode": null,
      "httpStatusCode": 200
    }
  ]
}
```

### 14.3 Escenario 3: Verificar Roles en Token

1. Copiar token del response de Login
2. Navegar a [jwt.io](https://jwt.io)
3. Pegar token en el campo "Encoded"
4. Verificar payload decodificado:

```json
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "guid-user-id",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "artista@test.com",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Fan",
  "sub": "guid-user-id",
  "email": "artista@test.com",
  "jti": "unique-guid",
  "exp": 1707789600,
  "iss": "WePlayRises",
  "aud": "WePlayRisesClient"
}
```

**Verificar:**
- ✅ Claim `role` contiene "Fan"
- ✅ Claim `nameidentifier` contiene UserId
- ✅ Claim `email` contiene email del usuario
- ✅ Token expira en 24 horas (verificar `exp` timestamp)

---

## 15. Patrones Arquitectonicos Clave

### 15.1 Logica de Negocio en Handlers
```
Handler contiene:
- Validacion con FluentValidation
- Logica de negocio (asignar roles, verificar credenciales)
- Orquestacion de llamadas a services (UserManager, IJwtTokenGenerator)
- Mapping de entidades a DTOs

UserManager/SignInManager contiene:
- SOLO operaciones de Identity (crear usuario, verificar password, asignar roles)
- NO logica de negocio custom
```

### 15.2 ServiceResponse Siempre con Constants

```csharp
// ✅ CORRECTO - Usar ServiceResponseMessageType constants
return new ServiceResponse<LoginResponseDto>
{
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Email o contrasena incorrectos",
            ErrorCode = ServiceResponseMessageType.Auth_InvalidCredentials  // Constant
        }
    }
};

// ❌ INCORRECTO - String literal
return new ServiceResponse<LoginResponseDto>
{
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "Email o contrasena incorrectos",
            ErrorCode = "AUTH_INVALID_CREDENTIALS"  // NO USAR strings literales
        }
    }
};
```

### 15.3 Constructor con ?? throw para TODAS las Dependencias

```csharp
// ✅ CORRECTO - Validar TODAS las dependencias
public LoginCommandHandler(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IValidator<LoginCommand> validator,
    IJwtTokenGenerator jwtGenerator,
    IMapper mapper,
    ILogger<LoginCommandHandler> logger)
{
    _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    _jwtGenerator = jwtGenerator ?? throw new ArgumentNullException(nameof(jwtGenerator));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}

// ❌ INCORRECTO - Asignacion directa sin validacion
public LoginCommandHandler(
    UserManager<IdentityUser> userManager,
    IValidator<LoginCommand> validator)
{
    _userManager = userManager;  // NUNCA - falta ?? throw
    _validator = validator;      // NUNCA - falta ?? throw
}
```

### 15.4 Validators con .WithMessage() Y .WithErrorCode()

```csharp
// ✅ CORRECTO - Ambos presentes, usar constants
RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage("El email es obligatorio")
    .WithErrorCode(ServiceResponseMessageType.Validation_Required)
    .EmailAddress()
    .WithMessage("El formato del email no es valido")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);

// ❌ INCORRECTO - Solo WithMessage
RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage("El email es obligatorio");  // Falta WithErrorCode

// ❌ INCORRECTO - String literal en ErrorCode
RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage("El email es obligatorio")
    .WithErrorCode("VALIDATION_REQUIRED");  // NO usar strings literales
```

---

## 16. Checklist de Code Review

Antes de marcar WPR-006 como completo, verificar:

### 16.1 Codigo
- [ ] LoginCommand + LoginCommandHandler en MISMO archivo
- [ ] GetCurrentUserQuery + GetCurrentUserQueryHandler en MISMO archivo
- [ ] Todos los Handlers retornan `ServiceResponse<T>`
- [ ] Constructores con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Validators usan `ServiceResponseMessageType.X` constants (NO strings literales)
- [ ] Handlers usan `ServiceResponseMessageType.X` en respuestas de error
- [ ] Try-catch con logging en todos los Handlers
- [ ] Roles se asignan usando `Roles.X` constants (NO strings literales)
- [ ] JwtTokenGenerator incluye roles como claims `ClaimTypes.Role`
- [ ] AuthProfile tiene mappings para LoginResponseDto y UserInfoDto

### 16.2 Arquitectura
- [ ] Handler NUNCA inyecta DbContext
- [ ] Handlers inyectan Services (UserManager, SignInManager, IJwtTokenGenerator)
- [ ] Logica de negocio en Handlers, NO en Services
- [ ] Services retornan entidades/IdentityResult, NO DTOs
- [ ] Validators en carpeta `Validators/` separada
- [ ] Handler + Command/Query en MISMO archivo

### 16.3 Funcionalidad
- [ ] POST /api/auth/register asigna rol "Fan" por defecto
- [ ] POST /api/auth/login retorna token con roles en claims
- [ ] GET /api/auth/me requiere autenticacion (atributo `[Authorize]`)
- [ ] Roles se crean automaticamente en startup (Program.cs seed)
- [ ] Token decodificado en jwt.io contiene claim `role`

### 16.4 Testing
- [ ] Register exitoso con validacion de campos
- [ ] Login exitoso con credenciales validas
- [ ] Login fallido con credenciales invalidas (401)
- [ ] Login fallido con usuario no existente (401 - mismo mensaje que password incorrecto)
- [ ] GET /me sin token retorna 401
- [ ] GET /me con token valido retorna 200 con UserInfoDto
- [ ] Token incluye roles en claims (verificar en jwt.io)

---

## 17. Proximos Pasos

Una vez completado WPR-006, el siguiente paso es:

**WPR-010: CRUD Artista** - Crear endpoint para que un Fan cree su perfil de Artista.

Flujo esperado:
1. Usuario se registra con rol "Fan"
2. Usuario crea perfil Artista (POST /api/artistas)
3. Handler de CreateArtistaCommand:
   - Crea entidad Artista en DB
   - Asigna rol "Artista" al usuario actual: `await _userManager.AddToRoleAsync(user, Roles.Artista)`
4. Usuario ahora tiene roles ["Fan", "Artista"]
5. Endpoints de Campania requieren rol "Artista" con `[Authorize(Roles = "Artista")]`

---

## 18. Referencias

- **CQRS Rule:** `.claude/rules/backend/cqrs.rule.md` - Reglas CRITICAS de CQRS
- **API Contracts:** `plans/WPR-006-identity-jwt/backend/api-contracts.md` - Contratos de endpoints
- **Hexagonal Architecture:** `plans/WPR-006-identity-jwt/backend/hexagonal-architecture.md` - Arquitectura de capas
- **EF Core Rule:** `.claude/rules/backend/ef-core.rule.md` - Para migraciones Identity
- **Templates:** `.claude/templates/api/` - Templates de Command/Query/Validator

---

## 19. Notas Finales

### 19.1 Decisiones Arquitectonicas

**¿Por que reutilizar RegisterResponseDto para Login?**
- En MVP, ambos retornan `{ Token, UserId, Email, Roles }`
- Evita duplicacion de codigo
- Si en futuro necesitan datos distintos, se refactoriza

**¿Por que SignInManager en lugar de PasswordHasher?**
- `SignInManager.CheckPasswordSignInAsync` maneja lockout, two-factor, etc.
- Mas consistente con flujo Identity estandar
- Mejor practica segun docs de Microsoft

**¿Por que Seed de roles en Program.cs?**
- Garantiza que roles existan en cualquier ambiente (dev, test, prod)
- Idempotente (no duplica roles si ya existen)
- Mas simple que migracion con seed data para MVP

### 19.2 Seguridad

**Mensaje generico para login fallido:**
- NO revelar si el usuario existe: "Email o contrasena incorrectos"
- Evita enumeration attacks
- Mismo mensaje para "usuario no existe" y "password incorrecto"

**Lockout deshabilitado en MVP:**
- `lockoutOnFailure: false` en `CheckPasswordSignInAsync`
- En produccion, habilitar lockout despues de N intentos fallidos

**EmailConfirmed = false por defecto:**
- MVP no implementa confirmacion de email (requiere SMTP)
- En produccion, enviar email de confirmacion y validar antes de login

### 19.3 Deuda Tecnica (MVP+)

- [ ] Implementar refresh token (no solo access token)
- [ ] Confirmacion de email con envio de correos
- [ ] Lockout de cuentas despues de intentos fallidos
- [ ] Politicas de password mas estrictas (ASP.NET Identity PasswordOptions)
- [ ] Two-factor authentication
- [ ] OAuth providers (Google, Facebook)
- [ ] Auditoria de logins (tabla LoginHistory)
- [ ] Rate limiting para endpoint /login

---

**Fin del Plan CQRS - WPR-006**

**Archivo creado:** `C:\Repos\WePlay_Rises\plans\WPR-006-identity-jwt\backend\cqrs-plan.md`

**Puntos clave:**
- LoginCommand + Handler con SignInManager para verificar credenciales
- GetCurrentUserQuery para endpoint GET /me (requiere autenticacion)
- Modificar RegisterCommand para asignar rol "Fan" por defecto
- Actualizar JwtTokenGenerator para incluir roles en claims del token
- Seed de roles (Artista, Fan, Admin) en Program.cs
- SIEMPRE usar `ServiceResponseMessageType.X` constants (NO strings literales)
- SIEMPRE `?? throw new ArgumentNullException` en constructores

**Siguiente paso:** Implementar codigo siguiendo este plan (usar agente `api-feature-architect` o implementar manualmente).
