# Arquitectura Hexagonal: Identity JWT (WPR-006)

**Fecha:** 2026-02-12
**Módulo:** UserAccess
**Feature:** Identity mínimo con JWT

---

## 1. Resumen Ejecutivo

Completar el sistema de autenticación JWT para WePlay Rises. La mayoría del código ya está implementado (RegisterCommand, JwtTokenGenerator, UserAccessContext con Identity). Este plan se enfoca en:

1. **Agregar LoginCommand** - Permitir login de usuarios existentes
2. **Agregar constantes de roles** - Artista, Fan, Admin
3. **Modificar JwtTokenGenerator** - Incluir roles en claims del token
4. **Agregar error codes** - Para errores de login (credenciales inválidas)
5. **Primera migración Identity** - Crear tablas AspNetUsers, AspNetRoles, etc.

**Estado actual:** ✅ 70% completado (Register funciona, falta Login y roles)

---

## 2. Domain Layer

### 2.1 Constantes Nuevas

#### ServiceResponseMessageType (MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar al rango AUTH (3000-3999):**

| Constante | Código | Mensaje Típico |
|-----------|--------|----------------|
| `Auth_InvalidCredentials` | 3006 | Email o contraseña incorrectos |
| `Auth_UserNotFound` | 3007 | Usuario no encontrado |
| `Auth_EmailNotConfirmed` | 3008 | Email no confirmado (futuro) |

**Cambios requeridos:**
```csharp
// Agregar después de la línea 48 (Auth_UserNotAuthenticated)
public const string Auth_InvalidCredentials = "3006";
public const string Auth_UserNotFound = "3007";
public const string Auth_EmailNotConfirmed = "3008";  // Para MVP+
```

#### Roles (NUEVO)
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
}
```

**Decisión arquitectónica:**
- Roles son strings, NO enums (Identity usa strings internamente)
- Por defecto, usuarios registrados reciben rol "Fan"
- Artistas deben crear perfil Artista (WPR-010) para obtener rol "Artista"
- Admin solo se asigna manualmente en DB (no hay UI de registro Admin en MVP)

### 2.2 Entidades

**No se requieren cambios.** Identity usa tablas AspNetUsers, AspNetRoles (generadas por EF Core).

La entidad `Artista` ya existe y tiene `UserIdPropietario` que referencia a `AspNetUsers.Id`.

---

## 3. Infrastructure Layer

### 3.1 Services (MODIFICAR)

#### JwtTokenGenerator (MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/JwtTokenGenerator.cs`

**Cambios requeridos:**

1. **Actualizar interfaz para aceptar roles:**

```csharp
// IJwtTokenGenerator.cs
public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, IList<string> roles);
}
```

2. **Modificar método para incluir roles en claims:**

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

**Impacto:** RegisterCommand debe actualizarse para pasar roles al generar token (ver 4.1.1).

### 3.2 Context

**UserAccessContext (NO MODIFICAR)** - Ya está configurado como `IdentityDbContext<IdentityUser>`.

---

## 4. Application Layer

### 4.1 Commands

#### 4.1.1 RegisterCommand (MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Cambios requeridos:**

1. **Asignar rol "Fan" por defecto:**

```csharp
// Después de crear usuario (línea 73), agregar:
// 3.1 Assign default role
await _userManager.AddToRoleAsync(user, Roles.Fan);
```

2. **Obtener roles del usuario antes de generar token:**

```csharp
// Línea 88 (antes de generar token):
var userRoles = await _userManager.GetRolesAsync(user);
var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);
```

**Total de cambios:** 2 líneas agregadas, 1 línea modificada.

#### 4.1.2 LoginCommand (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/LoginCommand.cs`

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<ServiceResponse<RegisterResponseDto>>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResponse<RegisterResponseDto>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

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

    public async Task<ServiceResponse<RegisterResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RegisterResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ValidateExtensions.UnauthorizedServiceResponse<RegisterResponseDto>(
                    "Email o contraseña incorrectos",
                    ServiceResponseMessageType.Auth_InvalidCredentials);
            }

            // 3. Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return ValidateExtensions.UnauthorizedServiceResponse<RegisterResponseDto>(
                    "Email o contraseña incorrectos",
                    ServiceResponseMessageType.Auth_InvalidCredentials);
            }

            // 4. Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // 5. Generate JWT token
            var token = _jwtGenerator.GenerateToken(user.Id, user.Email!, userRoles);

            // 6. Map and return response
            var response = _mapper.Map<RegisterResponseDto>(user);
            response.Token = token;

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new ServiceResponse<RegisterResponseDto>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Inicio de sesión exitoso",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user with email {Email}", request.Email);
            return ValidateExtensions.InternalServerErrorServiceResponse<RegisterResponseDto>(
                "Error inesperado al iniciar sesión",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
```

**Decisiones arquitectónicas:**

- Retorna mismo DTO que Register (`RegisterResponseDto`) - En MVP se unifica respuesta auth
- Usa `SignInManager.CheckPasswordSignInAsync` en lugar de `PasswordHasher` (mejor práctica)
- NO devuelve mensaje específico "usuario no existe" vs "password incorrecto" (seguridad)
- Lockout deshabilitado en MVP (`lockoutOnFailure: false`)

### 4.2 Validators

#### 4.2.1 LoginCommandValidator (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/LoginCommandValidator.cs`

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
            .WithMessage("El email no tiene formato válido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

**Nota:** NO valida longitud mínima de password en Login (solo en Register).

### 4.3 DTOs

**RegisterResponseDto (NO MODIFICAR)** - Ya existe y contiene `Token`, `UserId`, `Email`.

Se reutiliza para Login por simplicidad de MVP.

### 4.4 Mapping

**AuthProfile (NO MODIFICAR)** - Ya existe mapping de `IdentityUser` -> `RegisterResponseDto`.

---

## 5. WebApi Layer

### 5.1 Controllers

#### AuthController (MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

**Agregar endpoint POST /login:**

```csharp
[HttpPost("login")]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<RegisterResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
{
    var response = await _mediator.Send(command, ct);
    return response.ToActionResult();
}
```

**Total de cambios:** 1 método agregado.

### 5.2 Program.cs (MODIFICAR - Seed de Roles)

**Archivo:** `src/api/WebApi/Program.cs`

**Agregar después de `app.Build()` (antes de configurar middleware):**

```csharp
// Seed roles (ejecuta solo una vez al iniciar app)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Artista", "Fan", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
```

**Propósito:** Crear roles Artista, Fan, Admin en la DB al iniciar la aplicación.

**Alternativa (mejor práctica):** Migración con seed data (pero esto es más simple para MVP).

---

## 6. Migrations

### 6.1 Crear Primera Migración Identity

**Comandos:**

```bash
cd C:\Repos\WePlay_Rises\src\api

# Crear migración
dotnet ef migrations add AddIdentityTables --project WebApi --context UserAccessContext

# Aplicar migración
dotnet ef database update --project WebApi --context UserAccessContext
```

**Tablas que se crearán:**

| Tabla | Descripción |
|-------|-------------|
| `AspNetUsers` | Usuarios del sistema (IdentityUser) |
| `AspNetRoles` | Roles (Artista, Fan, Admin) |
| `AspNetUserRoles` | Relación N:N entre Users y Roles |
| `AspNetUserClaims` | Claims adicionales por usuario |
| `AspNetUserLogins` | Logins externos (Google, FB - futuro) |
| `AspNetUserTokens` | Tokens de refresh, password reset |
| `AspNetRoleClaims` | Claims por rol (futuro) |

**IMPORTANTE:** La tabla `Artista` ya existe (migración anterior). Esta migración solo agrega tablas Identity.

---

## 7. Dependency Injection

### 7.1 DependencyInjection.cs (NO MODIFICAR)

**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/DependencyInjection.cs`

Ya registra:
- `IJwtTokenGenerator` -> `JwtTokenGenerator`
- Validators con `AddValidatorsFromAssembly`
- MediatR con `AddMediatR`

**No requiere cambios** - Los nuevos validators y handlers se registran automáticamente.

---

## 8. Archivos a Crear/Modificar

### 8.1 Archivos NUEVOS (3)

```
Modules/UserAccess/WePlayRises.UserAccess.Domain/
└── Constants/
    └── Roles.cs                                          # NUEVO

Modules/UserAccess/WePlayRises.UserAccess.Application/
└── Features/Auth/
    ├── Commands/
    │   └── LoginCommand.cs                              # NUEVO
    └── Validators/
        └── LoginCommandValidator.cs                     # NUEVO
```

### 8.2 Archivos a MODIFICAR (5)

```
Modules/UserAccess/WePlayRises.UserAccess.Domain/
└── Constants/
    └── ServiceResponseMessageType.cs                    # +3 constantes Auth

Modules/UserAccess/WePlayRises.UserAccess.Application/
├── Interfaces/Services/
│   └── IJwtTokenGenerator.cs                           # Firma método (+ roles param)
└── Features/Auth/Commands/
    └── RegisterCommand.cs                               # +3 líneas (asignar rol, obtener roles)

Modules/UserAccess/WePlayRises.UserAccess.Infra/
└── Services/
    └── JwtTokenGenerator.cs                             # +5 líneas (foreach roles)

Modules/UserAccess/WePlayRises.UserAccess.WebApi/
└── Controllers/
    └── AuthController.cs                                # +1 método Login
```

### 8.3 WebApi/Program.cs

```
src/api/WebApi/
└── Program.cs                                           # +10 líneas (seed roles)
```

---

## 9. Testing Manual

### 9.1 Swagger Endpoints

Una vez implementados los cambios:

```bash
cd C:\Repos\WePlay_Rises\src\api
dotnet run --project WebApi
```

**Navegar a:** `https://localhost:5001/swagger`

#### Test 1: Register + Login

1. **POST /api/auth/register**
   ```json
   {
     "email": "artista@test.com",
     "password": "Test1234",
     "confirmPassword": "Test1234"
   }
   ```

   **Esperado:** 201 Created con token JWT

2. **POST /api/auth/login**
   ```json
   {
     "email": "artista@test.com",
     "password": "Test1234"
   }
   ```

   **Esperado:** 200 OK con token JWT

3. **POST /api/auth/login** (credenciales incorrectas)
   ```json
   {
     "email": "artista@test.com",
     "password": "WrongPass"
   }
   ```

   **Esperado:** 401 Unauthorized con ErrorCode "3006"

#### Test 2: Token con Roles

1. Copiar token del response de Register o Login
2. Decodificar en [jwt.io](https://jwt.io)
3. Verificar que payload contiene:
   ```json
   {
     "nameid": "user-id",
     "email": "artista@test.com",
     "role": "Fan",
     "jti": "guid"
   }
   ```

#### Test 3: Endpoint Protegido

1. **GET /api/artistas/me** (sin token)
   **Esperado:** 401 Unauthorized

2. **GET /api/artistas/me** (con Bearer token)
   **Esperado:** 200 OK o 404 (si artista no creado aún)

---

## 10. Diagramas

### 10.1 Flujo de Autenticación

```
┌──────────┐
│ Frontend │
└────┬─────┘
     │
     │ POST /api/auth/register
     ▼
┌─────────────────┐
│ AuthController  │
└────┬────────────┘
     │ Send(RegisterCommand)
     ▼
┌──────────────────────────┐
│ RegisterCommandHandler   │
└────┬─────────────────────┘
     │
     ├─► Validator ──► Email format, password rules
     │
     ├─► UserManager.CreateAsync ──► Identity crea usuario en DB
     │
     ├─► UserManager.AddToRoleAsync ──► Asigna rol "Fan"
     │
     ├─► UserManager.GetRolesAsync ──► Obtiene roles ["Fan"]
     │
     ├─► JwtTokenGenerator.GenerateToken(id, email, roles) ──► JWT con claims + role
     │
     └─► ServiceResponse<RegisterResponseDto> ──► { Token, UserId, Email }
```

### 10.2 Flujo de Login

```
┌──────────┐
│ Frontend │
└────┬─────┘
     │
     │ POST /api/auth/login
     ▼
┌─────────────────┐
│ AuthController  │
└────┬────────────┘
     │ Send(LoginCommand)
     ▼
┌──────────────────────────┐
│ LoginCommandHandler      │
└────┬─────────────────────┘
     │
     ├─► Validator ──► Email format, password required
     │
     ├─► UserManager.FindByEmailAsync ──► Busca usuario
     │   └─► Si no existe → 401 InvalidCredentials
     │
     ├─► SignInManager.CheckPasswordSignInAsync ──► Valida password
     │   └─► Si incorrecto → 401 InvalidCredentials
     │
     ├─► UserManager.GetRolesAsync ──► Obtiene roles
     │
     ├─► JwtTokenGenerator.GenerateToken(id, email, roles) ──► JWT
     │
     └─► ServiceResponse<RegisterResponseDto> ──► { Token, UserId, Email }
```

### 10.3 Arquitectura Hexagonal - Capas

```
┌─────────────────────────────────────────────────────────────────┐
│                         WEBAPI LAYER                            │
│  AuthController (POST /register, /login)                        │
│  Program.cs (DI, Identity, JWT config, Seed roles)              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             │ MediatR.Send(Command)
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      APPLICATION LAYER                           │
│  Commands: RegisterCommand, LoginCommand                         │
│  Handlers: RegisterCommandHandler, LoginCommandHandler          │
│  Validators: RegisterCommandValidator, LoginCommandValidator    │
│  DTOs: RegisterResponseDto                                       │
│  Interfaces: IJwtTokenGenerator (Port OUT)                       │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             │ Dependency on
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                       DOMAIN LAYER                               │
│  Constants: ServiceResponseMessageType (error codes)             │
│  Constants: Roles (Artista, Fan, Admin)                          │
│  Model: (no entities - Identity usa AspNetUsers)                 │
└─────────────────────────────────────────────────────────────────┘
                             │
                             │ Implements
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                          │
│  Services: JwtTokenGenerator (implementa IJwtTokenGenerator)     │
│  Context: UserAccessContext (IdentityDbContext<IdentityUser>)   │
│  DependencyInjection.cs                                          │
└─────────────────────────────────────────────────────────────────┘
```

**Puertos (Ports):**
- `IJwtTokenGenerator` - Port OUT (Application → Infra)
- `UserManager<IdentityUser>` - Port OUT (Identity framework, inyectado)
- `SignInManager<IdentityUser>` - Port OUT (Identity framework, inyectado)

**Adaptadores (Adapters):**
- `JwtTokenGenerator` - Adapter (implementa port)
- `UserAccessContext` - Adapter (EF Core + Identity)

---

## 11. Decisiones Arquitectónicas

### 11.1 ¿Por qué reutilizar RegisterResponseDto para Login?

**Decisión:** Unificar respuestas de auth en MVP.

**Alternativas consideradas:**
- Crear `LoginResponseDto` separado (más semántico)
- Usar `AuthResultDto` genérico (más flexible)

**Justificación:** En MVP, ambos endpoints retornan `{ Token, UserId, Email }`. Agregar DTOs separados sería YAGNI (You Aren't Gonna Need It). Si en futuro necesitamos datos distintos, se refactoriza.

### 11.2 ¿Por qué SignInManager en lugar de PasswordHasher?

**Decisión:** Usar `SignInManager.CheckPasswordSignInAsync`.

**Razones:**
- Maneja lockout automáticamente (aunque en MVP está disabled)
- Maneja two-factor (futuro)
- Más consistente con flujo Identity estándar

### 11.3 ¿Por qué Seed de roles en Program.cs?

**Decisión:** Crear roles al iniciar app si no existen.

**Alternativas consideradas:**
- Migración con seed data (mejor práctica)
- Script SQL manual
- Endpoint admin para crear roles

**Justificación:** Para MVP, seed en Program.cs es más simple y garantiza que roles existan en cualquier ambiente (dev, test, prod). En producción, esto se movería a migración.

### 11.4 ¿Por qué NO confirmar email en MVP?

**Decisión:** EmailConfirmed = true por defecto.

**Razones:**
- MVP necesita flujo rápido (registro → crear campaña)
- Confirmación de email requiere servicio SMTP (no configurado)
- Se puede agregar en MVP+ sin breaking changes

### 11.5 ¿Por qué NO refresh token en MVP?

**Decisión:** Solo Access Token con expiración de 24h.

**Razones:**
- Refresh token agrega complejidad (storage, rotación, revocación)
- Para demo/curso, 24h es suficiente
- Frontend puede manejar re-login manual

**Deuda técnica identificada:** Agregar refresh token en producción.

---

## 12. Checklist de Implementación

### 12.1 Domain Layer
- [ ] Agregar constantes `Auth_InvalidCredentials`, `Auth_UserNotFound`, `Auth_EmailNotConfirmed` a `ServiceResponseMessageType.cs`
- [ ] Crear archivo `Roles.cs` con constantes Artista, Fan, Admin

### 12.2 Application Layer
- [ ] Crear `LoginCommand.cs` con Command + Handler
- [ ] Crear `LoginCommandValidator.cs`
- [ ] Modificar `RegisterCommand.cs` para asignar rol "Fan" y obtener roles antes de generar token
- [ ] Actualizar interfaz `IJwtTokenGenerator.cs` para aceptar lista de roles

### 12.3 Infrastructure Layer
- [ ] Modificar `JwtTokenGenerator.cs` para incluir roles en claims del token

### 12.4 WebApi Layer
- [ ] Agregar endpoint `POST /login` en `AuthController.cs`
- [ ] Agregar seed de roles en `Program.cs`

### 12.5 Database
- [ ] Ejecutar migración: `dotnet ef migrations add AddIdentityTables`
- [ ] Aplicar migración: `dotnet ef database update`
- [ ] Verificar que roles se crearon: `SELECT * FROM AspNetRoles`

### 12.6 Testing
- [ ] Probar register → obtener token con rol "Fan"
- [ ] Probar login → obtener mismo token
- [ ] Probar login con credenciales incorrectas → 401
- [ ] Decodificar token en jwt.io → verificar claim "role": "Fan"
- [ ] Probar endpoint protegido sin token → 401
- [ ] Probar endpoint protegido con token → 200 (o 404 si no hay data)

### 12.7 Integration con Frontend
- [ ] Frontend puede llamar `POST /api/auth/register`
- [ ] Frontend puede llamar `POST /api/auth/login`
- [ ] Frontend guarda token en localStorage
- [ ] Frontend incluye `Authorization: Bearer {token}` en requests protegidos

---

## 13. Próximos Pasos

Una vez completado WPR-006:

1. **WPR-010: CRUD Artista** - Endpoint para crear perfil artista
   - Handler asigna rol "Artista" al crear perfil
   - Frontend redirige a crear perfil después de register

2. **Frontend Auth** - Implementar páginas de login/register en Landing

3. **Frontend Admin** - Proteger rutas del dashboard con token

---

## 14. Referencias

- **ADR-006:** Caching Strategy (RequestCacheService) - No aplica a auth (no se cachea)
- **CQRS Rule:** `.claude/rules/backend/cqrs.rule.md` - Seguir para Commands/Handlers
- **EF Core Rule:** `.claude/rules/backend/ef-core.rule.md` - Para migraciones
- **Templates:** `.claude/templates/api/` - Para Command/Validator templates

---

## 15. Notas Finales

### 15.1 Tiempo Estimado

| Tarea | Tiempo |
|-------|--------|
| Agregar constantes Domain | 5 min |
| Crear LoginCommand + Validator | 15 min |
| Modificar JwtTokenGenerator | 10 min |
| Modificar RegisterCommand | 5 min |
| Actualizar AuthController | 5 min |
| Seed roles en Program.cs | 5 min |
| Crear y aplicar migración | 10 min |
| Testing manual en Swagger | 15 min |
| **Total** | **70 min** (~1.2h) |

### 15.2 Riesgos Identificados

| Riesgo | Probabilidad | Mitigación |
|--------|--------------|------------|
| Migración falla (tablas ya existen) | Media | Revisar migrations existentes, usar `-Context UserAccessContext` explícito |
| Token no incluye roles | Baja | Decodificar token en jwt.io para verificar |
| Roles no se crean en DB | Baja | Agregar logs al seed de roles |
| Frontend no envía Bearer token | Media | Documentar formato header en Swagger |

### 15.3 Deuda Técnica (MVP+)

- [ ] Implementar refresh token
- [ ] Confirmación de email con envío de correos
- [ ] Lockout de cuentas después de intentos fallidos
- [ ] Políticas de password más estrictas (ASP.NET Identity PasswordOptions)
- [ ] Two-factor authentication
- [ ] OAuth providers (Google, Facebook)
- [ ] Auditoría de logins (tabla de LoginHistory)

---

*Última actualización: 2026-02-12*
*Plan creado por: hexagonal-planning-architect agent*
