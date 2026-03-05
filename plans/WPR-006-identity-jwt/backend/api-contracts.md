# Contratos API: Identity JWT Authentication

**Fecha:** 2026-02-12
**Modulo:** UserAccess
**Feature:** WPR-006-identity-jwt

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion | Auth Required |
|--------|------|------|-------------|---------------|
| POST | /api/auth/register | Command | Registrar usuario (YA EXISTE) | No |
| POST | /api/auth/login | Command | Login con email/password | No |
| GET | /api/auth/me | Query | Obtener info usuario autenticado | Si |

## 2. Request DTOs

### 2.1 LoginCommand (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/LoginCommand.cs`

| Propiedad | Tipo | Requerido | Validacion |
|-----------|------|-----------|------------|
| Email | string | Si | NotEmpty, EmailAddress |
| Password | string | Si | NotEmpty |

**Implementa:** `IRequest<ServiceResponse<LoginResponseDto>>`

**Notas:**
- No requiere validacion de minima longitud de password (solo validar que no este vacio)
- ASP.NET Core Identity valida las credenciales internamente

### 2.2 GetCurrentUserQuery (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Queries/GetCurrentUserQuery.cs`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| UserId | string | Si | Extraido del claim ClaimTypes.NameIdentifier |

**Implementa:** `IRequest<ServiceResponse<UserInfoDto>>`

**Notas:**
- UserId se extrae automaticamente del JWT token en el Handler
- Controller pasa HttpContext.User al query

### 2.3 RegisterCommand (YA EXISTE)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Modificaciones necesarias:**
- Agregar propiedad `Role` (opcional, default "Fan")
- Agregar logica para asignar rol en Handler

## 3. Response DTOs

### 3.1 LoginResponseDto (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/LoginResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | IdentityUser.Id |
| Email | string | Email del usuario |
| Token | string | JWT token con claims (NameIdentifier, Email, Roles) |
| Roles | List\<string\> | Roles asignados al usuario |

### 3.2 UserInfoDto (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/UserInfoDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| UserId | string | IdentityUser.Id |
| Email | string | Email del usuario |
| Roles | List\<string\> | Roles asignados al usuario |
| EmailConfirmed | bool | Si el email esta confirmado |

### 3.3 RegisterResponseDto (YA EXISTE - MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Dtos/RegisterResponseDto.cs`

**Agregar:**
- `Roles` (List\<string\>) - Roles asignados al usuario

## 4. Constants

### 4.1 UserRoles (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/UserRoles.cs`

```csharp
public static class UserRoles
{
    public const string Artista = "Artista";
    public const string Fan = "Fan";
    public const string Admin = "Admin";

    public static readonly string[] All = { Artista, Fan, Admin };

    public static bool IsValid(string role) => All.Contains(role);
}
```

**Notas:**
- Usar estas constantes SIEMPRE en lugar de strings literales
- Roles disponibles para MVP: Artista, Fan, Admin
- Fan es el rol por defecto en registro

### 4.2 ServiceResponseMessageType (AGREGAR NUEVOS)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`

**Agregar en seccion AUTH (3000-3999):**
```csharp
public const string Auth_InvalidCredentials = "3006";
public const string Auth_UserNotFound = "3007";
public const string Auth_AccountLocked = "3008";
```

## 5. Validadores

### 5.1 LoginCommandValidator (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/LoginCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Email | NotEmpty | El email es obligatorio | Validation_Required |
| Email | EmailAddress | El formato del email no es valido | Validation_InvalidEmail |
| Password | NotEmpty | La contrasena es obligatoria | Validation_Required |

**Notas:**
- NO validar longitud minima de password en login
- La validacion de credenciales se hace en el Handler con SignInManager

### 5.2 RegisterCommandValidator (YA EXISTE - MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Validators/RegisterCommandValidator.cs`

**Agregar validacion para Role (opcional):**
```csharp
RuleFor(x => x.Role)
    .Must(role => string.IsNullOrEmpty(role) || UserRoles.IsValid(role))
    .WithMessage("Rol invalido. Roles permitidos: Artista, Fan")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);
```

## 6. AutoMapper Mappings

### 6.1 AuthProfile (YA EXISTE - MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Mapping/AuthProfile.cs`

**Agregar mappings:**

| Source | Destination | Notas |
|--------|-------------|-------|
| IdentityUser | LoginResponseDto | Incluir roles desde UserManager |
| IdentityUser | UserInfoDto | Incluir roles y EmailConfirmed |
| IdentityUser | RegisterResponseDto | Actualizar para incluir roles |

**Notas:**
- Los roles se obtienen via `UserManager.GetRolesAsync(user)` en el Handler
- El mapping debe recibir los roles como parametro adicional

## 7. JWT Token Updates

### 7.1 IJwtTokenGenerator (MODIFICAR INTERFAZ)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Interfaces/Services/IJwtTokenGenerator.cs`

**Firma actualizada:**
```csharp
string GenerateToken(string userId, string email, IList<string> roles);
```

**Cambios:**
- Agregar parametro `roles` (IList<string>)
- Backward compatibility: Crear sobrecarga sin roles que pase lista vacia

### 7.2 JwtTokenGenerator (MODIFICAR IMPLEMENTACION)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Services/JwtTokenGenerator.cs`

**Actualizar metodo GenerateToken:**
- Agregar claims de roles: `new Claim(ClaimTypes.Role, role)` por cada rol
- Mantener claims existentes (NameIdentifier, Email, Sub, Jti)

**Claims finales:**
```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, userId),
    new(ClaimTypes.Email, email),
    new(JwtRegisteredClaimNames.Sub, userId),
    new(JwtRegisteredClaimNames.Email, email),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

// Agregar claim por cada rol
foreach (var role in roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}
```

## 8. Handlers Logic

### 8.1 LoginCommandHandler (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/LoginCommand.cs`

**Dependencias a inyectar:**
```csharp
private readonly UserManager<IdentityUser> _userManager;
private readonly SignInManager<IdentityUser> _signInManager;
private readonly IValidator<LoginCommand> _validator;
private readonly IJwtTokenGenerator _jwtGenerator;
private readonly IMapper _mapper;
private readonly ILogger<LoginCommandHandler> _logger;
```

**Flujo del Handler:**
1. Validar con FluentValidation
2. Buscar usuario por email: `_userManager.FindByEmailAsync(request.Email)`
3. Si no existe: retornar error `Auth_UserNotFound` (3007)
4. Verificar credenciales: `_signInManager.CheckPasswordSignInAsync(user, request.Password, false)`
5. Si fallan credenciales: retornar error `Auth_InvalidCredentials` (3006)
6. Si cuenta bloqueada: retornar error `Auth_AccountLocked` (3008)
7. Obtener roles: `await _userManager.GetRolesAsync(user)`
8. Generar JWT token con roles: `_jwtGenerator.GenerateToken(user.Id, user.Email, roles)`
9. Mapear a LoginResponseDto y retornar ServiceResponse exitoso

**Try-Catch:**
- Capturar excepciones y retornar `Internal_UnexpectedError` (5000)
- Loggear con `_logger.LogError`

### 8.2 GetCurrentUserQueryHandler (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Queries/GetCurrentUserQuery.cs`

**Dependencias a inyectar:**
```csharp
private readonly UserManager<IdentityUser> _userManager;
private readonly IMapper _mapper;
private readonly ILogger<GetCurrentUserQueryHandler> _logger;
```

**Flujo del Handler:**
1. Buscar usuario por ID: `await _userManager.FindByIdAsync(request.UserId)`
2. Si no existe: retornar error `NotFound_User` (2001)
3. Obtener roles: `await _userManager.GetRolesAsync(user)`
4. Mapear a UserInfoDto y retornar ServiceResponse exitoso

**Try-Catch:**
- Capturar excepciones y retornar `Internal_UnexpectedError` (5000)

### 8.3 RegisterCommandHandler (YA EXISTE - MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Auth/Commands/RegisterCommand.cs`

**Modificaciones:**
1. Agregar propiedad `Role` al Command (opcional, default null)
2. Despues de `_userManager.CreateAsync`, asignar rol:
   ```csharp
   var roleToAssign = string.IsNullOrEmpty(request.Role) ? UserRoles.Fan : request.Role;
   await _userManager.AddToRoleAsync(user, roleToAssign);
   ```
3. Obtener roles: `await _userManager.GetRolesAsync(user)`
4. Actualizar llamada a `_jwtGenerator.GenerateToken` para incluir roles
5. Actualizar mapping para incluir roles en RegisterResponseDto

**Notas:**
- Los roles deben existir en ASP.NET Core Identity (seed data requerido)
- Si el rol no existe, Identity lanza excepcion (capturar y retornar error)

## 9. Controller Actions

### 9.1 POST /api/auth/login (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

```csharp
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
            "3006" or "3007" => Unauthorized(result), // InvalidCredentials, UserNotFound
            "3008" => StatusCode(423, result), // AccountLocked
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**OpenAPI Documentation:**
- Summary: "Authenticate user with email and password"
- Request Body: LoginCommand
- Response 200: ServiceResponse\<LoginResponseDto\> con JWT token
- Response 400: Errores de validacion
- Response 401: Credenciales invalidas o usuario no encontrado
- Response 423: Cuenta bloqueada
- Response 500: Error interno

### 9.2 GET /api/auth/me (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

```csharp
[HttpGet("me")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<UserInfoDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetCurrentUser()
{
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
            "2001" => NotFound(result), // NotFound_User
            _ => StatusCode(500, result)
        };
    }

    return Ok(result);
}
```

**OpenAPI Documentation:**
- Summary: "Get current authenticated user information"
- Authorization: Bearer token required
- Response 200: ServiceResponse\<UserInfoDto\>
- Response 401: Usuario no autenticado
- Response 404: Usuario no encontrado
- Response 500: Error interno

### 9.3 POST /api/auth/register (YA EXISTE - MODIFICAR)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`

**Cambios:**
- Actualizar ProducesResponseType para incluir roles en RegisterResponseDto
- Documentacion ya existe, no requiere cambios mayores

## 10. Identity Roles Seeding

### 10.1 Role Initialization (NUEVO)
**Archivo:** `Modules/UserAccess/WePlayRises.UserAccess.Infra/Extensions/IdentityServiceExtensions.cs`

**Crear metodo de extension:**
```csharp
public static async Task SeedRolesAsync(this IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in UserRoles.All)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
```

**Llamar desde Program.cs o DependencyInjection:**
- Ejecutar al iniciar la aplicacion
- Asegurar que los roles existan antes de asignarlos a usuarios

## 11. Archivos a Crear/Modificar

```
Modules/UserAccess/
├── WePlayRises.UserAccess.Domain/
│   └── Constants/
│       ├── ServiceResponseMessageType.cs    (MODIFICAR - agregar Auth_InvalidCredentials, etc.)
│       └── UserRoles.cs                     (NUEVO)
│
├── WePlayRises.UserAccess.Application/
│   ├── Features/Auth/
│   │   ├── Commands/
│   │   │   ├── LoginCommand.cs             (NUEVO - Command + Handler)
│   │   │   └── RegisterCommand.cs          (MODIFICAR - agregar Role)
│   │   ├── Queries/
│   │   │   └── GetCurrentUserQuery.cs      (NUEVO - Query + Handler)
│   │   └── Validators/
│   │       ├── LoginCommandValidator.cs    (NUEVO)
│   │       └── RegisterCommandValidator.cs (MODIFICAR - validar Role)
│   │
│   ├── Dtos/
│   │   ├── LoginResponseDto.cs             (NUEVO)
│   │   ├── UserInfoDto.cs                  (NUEVO)
│   │   └── RegisterResponseDto.cs          (MODIFICAR - agregar Roles)
│   │
│   ├── Mapping/
│   │   └── AuthProfile.cs                  (MODIFICAR - agregar mappings)
│   │
│   └── Interfaces/Services/
│       └── IJwtTokenGenerator.cs           (MODIFICAR - agregar parametro roles)
│
├── WePlayRises.UserAccess.Infra/
│   ├── Services/
│   │   └── JwtTokenGenerator.cs            (MODIFICAR - incluir role claims)
│   │
│   └── Extensions/
│       └── IdentityServiceExtensions.cs    (NUEVO - seed roles)
│
└── WePlayRises.UserAccess.WebApi/
    └── Controllers/
        └── AuthController.cs               (MODIFICAR - agregar login y me endpoints)
```

## 12. OpenAPI/Swagger Documentation

### Summary de Endpoints

#### POST /api/auth/register
- **Summary:** Register a new user in the system
- **Description:** Creates a new user account with email/password and assigns default role (Fan)
- **Request Body:** RegisterCommand
  - Email (string, required)
  - Password (string, required, min 8 chars)
  - ConfirmPassword (string, required)
  - Role (string, optional) - Default: "Fan"
- **Responses:**
  - 200: ServiceResponse\<RegisterResponseDto\> - Usuario registrado exitosamente
  - 400: ServiceResponse con errores de validacion
  - 409: Email ya registrado
  - 500: Error interno
- **Authentication:** No requerida

#### POST /api/auth/login
- **Summary:** Authenticate user with email and password
- **Description:** Validates user credentials and returns JWT token with role claims
- **Request Body:** LoginCommand
  - Email (string, required)
  - Password (string, required)
- **Responses:**
  - 200: ServiceResponse\<LoginResponseDto\> - Login exitoso con JWT token
  - 400: Errores de validacion
  - 401: Credenciales invalidas o usuario no encontrado
  - 423: Cuenta bloqueada
  - 500: Error interno
- **Authentication:** No requerida

#### GET /api/auth/me
- **Summary:** Get current authenticated user information
- **Description:** Returns user info and assigned roles from JWT token
- **Parameters:** None (UserId extraido del token)
- **Responses:**
  - 200: ServiceResponse\<UserInfoDto\> - Info del usuario
  - 401: Usuario no autenticado o token invalido
  - 404: Usuario no encontrado
  - 500: Error interno
- **Authentication:** Bearer token requerido

### Security Schemes

```yaml
securitySchemes:
  Bearer:
    type: http
    scheme: bearer
    bearerFormat: JWT
    description: JWT Authorization header using Bearer scheme
```

### Example Responses

#### POST /api/auth/login - Success (200)
```json
{
  "data": {
    "userId": "abc123-def456",
    "email": "artista@weplay.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "roles": ["Artista"]
  },
  "messages": [
    {
      "message": "Login exitoso",
      "errorCode": null,
      "httpStatusCode": 200
    }
  ]
}
```

#### POST /api/auth/login - Invalid Credentials (401)
```json
{
  "data": null,
  "messages": [
    {
      "message": "Email o contrasena invalidos",
      "errorCode": "3006",
      "httpStatusCode": 401
    }
  ]
}
```

#### GET /api/auth/me - Success (200)
```json
{
  "data": {
    "userId": "abc123-def456",
    "email": "artista@weplay.com",
    "roles": ["Artista"],
    "emailConfirmed": true
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

## 13. Validation Error Codes Reference

| ErrorCode | Constante | Mensaje Ejemplo | HTTP Status |
|-----------|-----------|-----------------|-------------|
| 1001 | Validation_Required | "El email es obligatorio" | 400 |
| 1005 | Validation_InvalidEmail | "El formato del email no es valido" | 400 |
| 1003 | Validation_MinLength | "La contrasena debe tener al menos 8 caracteres" | 400 |
| 1004 | Validation_InvalidFormat | "Las contrasenas no coinciden" | 400 |
| 1009 | Validation_DuplicateEmail | "Este email ya esta registrado" | 409 |
| 2001 | NotFound_User | "Usuario no encontrado" | 404 |
| 3005 | Auth_UserNotAuthenticated | "Usuario no autenticado" | 401 |
| 3006 | Auth_InvalidCredentials | "Email o contrasena invalidos" | 401 |
| 3007 | Auth_UserNotFound | "Usuario no encontrado" | 401 |
| 3008 | Auth_AccountLocked | "Cuenta bloqueada por multiples intentos fallidos" | 423 |
| 5000 | Internal_UnexpectedError | "Error inesperado al procesar la solicitud" | 500 |

## 14. JWT Token Claims

### Claims Structure
```csharp
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "abc123-def456",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "artista@weplay.com",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role": "Artista",
  "sub": "abc123-def456",
  "email": "artista@weplay.com",
  "jti": "unique-token-id",
  "exp": 1707789600,
  "iss": "WePlayRises",
  "aud": "WePlayRisesClient"
}
```

### Notas sobre Claims
- **NameIdentifier:** Se usa para extraer UserId en controllers con `User.FindFirstValue(ClaimTypes.NameIdentifier)`
- **Role:** Multiples claims si el usuario tiene varios roles (uno por rol)
- **Email:** Duplicado en claim standard y JWT registered claim
- **Sub:** Subject (identico a NameIdentifier para compatibilidad)
- **Jti:** JWT ID unico por token (para revocacion futura)

## 15. Authorization Examples

### Controller Level Authorization
```csharp
[Authorize] // Requiere cualquier usuario autenticado
public class MyController : ControllerBase
```

### Action Level Authorization
```csharp
[Authorize(Roles = "Artista")] // Solo rol Artista
public async Task<IActionResult> CreateCampania()

[Authorize(Roles = "Artista,Admin")] // Artista O Admin
public async Task<IActionResult> UpdateCampania()
```

### Policy-Based Authorization (Futuro)
```csharp
[Authorize(Policy = "CanManageCampanias")]
public async Task<IActionResult> DeleteCampania()
```

## 16. Testing Considerations

### Unit Tests a Crear
1. **LoginCommandHandlerTests**
   - Login exitoso con credenciales validas
   - Login fallido con credenciales invalidas
   - Login con usuario no existente
   - Login con cuenta bloqueada
   - Validacion de campos vacios

2. **GetCurrentUserQueryHandlerTests**
   - Obtener usuario autenticado exitosamente
   - Usuario no encontrado por ID

3. **RegisterCommandHandlerTests (Actualizar)**
   - Registro con rol por defecto (Fan)
   - Registro con rol Artista
   - Registro con rol invalido

4. **JwtTokenGeneratorTests (Actualizar)**
   - Token incluye claims de roles correctamente
   - Token sin roles genera lista vacia

### Integration Tests
- Login E2E desde controller hasta database
- GET /api/auth/me con token valido
- Rechazar requests sin token en endpoints protegidos

## 17. Checklist de Implementacion

- [ ] Crear UserRoles constants class
- [ ] Actualizar ServiceResponseMessageType con Auth_InvalidCredentials, Auth_UserNotFound, Auth_AccountLocked
- [ ] Crear LoginCommand + LoginCommandHandler en mismo archivo
- [ ] Crear LoginCommandValidator con WithMessage + WithErrorCode
- [ ] Crear GetCurrentUserQuery + GetCurrentUserQueryHandler en mismo archivo
- [ ] Modificar RegisterCommand para incluir Role (opcional)
- [ ] Actualizar RegisterCommandValidator para validar Role
- [ ] Modificar RegisterCommandHandler para asignar rol al usuario
- [ ] Crear LoginResponseDto
- [ ] Crear UserInfoDto
- [ ] Actualizar RegisterResponseDto para incluir Roles
- [ ] Modificar IJwtTokenGenerator para agregar parametro roles
- [ ] Actualizar JwtTokenGenerator para incluir role claims
- [ ] Actualizar AuthProfile con mappings de LoginResponseDto y UserInfoDto
- [ ] Crear IdentityServiceExtensions con SeedRolesAsync
- [ ] Agregar login endpoint en AuthController
- [ ] Agregar me endpoint en AuthController
- [ ] Registrar RoleManager en DependencyInjection
- [ ] Ejecutar SeedRolesAsync al iniciar aplicacion
- [ ] Actualizar Swagger documentation con ejemplos de responses
- [ ] Crear unit tests para LoginCommandHandler
- [ ] Crear unit tests para GetCurrentUserQueryHandler
- [ ] Actualizar unit tests de RegisterCommandHandler
- [ ] Crear integration test para login E2E

## 18. Notas de Implementacion

### ASP.NET Core Identity - Roles
- Los roles se gestionan con `RoleManager<IdentityRole>`
- Asignar roles con `UserManager.AddToRoleAsync(user, roleName)`
- Obtener roles con `UserManager.GetRolesAsync(user)` retorna `IList<string>`
- Validar existencia de rol con `RoleManager.RoleExistsAsync(roleName)`

### SignInManager vs UserManager
- **SignInManager:** Usado para autenticacion (verificar credenciales)
- **UserManager:** Usado para gestion de usuarios (crear, buscar, asignar roles)
- `CheckPasswordSignInAsync` retorna `SignInResult` con flags: Succeeded, IsLockedOut, IsNotAllowed

### JWT Token Expiration
- Configurado en appsettings.json: `Jwt:ExpirationHours`
- Default: 24 horas
- Frontend debe manejar refresh o re-login cuando expire

### Security Best Practices
- NUNCA retornar mensajes que revelen si un email existe ("Usuario no encontrado")
- Usar mensaje generico: "Email o contrasena invalidos"
- Implementar rate limiting para login (futuro)
- Considerar lockout despues de N intentos fallidos (ya incluido en Identity)

### Backward Compatibility
- RegisterCommand actual funciona sin cambios (Role null = Fan asignado)
- JwtTokenGenerator acepta lista vacia de roles (no genera claim de role)
- Endpoints existentes no requieren cambios

## 19. Dependencies ya Registradas

Verificar en `WePlayRises.UserAccess.Infra/DependencyInjection.cs`:
- UserManager\<IdentityUser\>
- SignInManager\<IdentityUser\>
- RoleManager\<IdentityRole\>
- IJwtTokenGenerator / JwtTokenGenerator
- FluentValidation validators (auto-registrados)
- AutoMapper profiles (auto-registrados)

## 20. Frontend Integration Notes

### Login Flow
```typescript
// POST /api/auth/login
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});

const { data, messages } = await response.json();

if (data?.token) {
  // Guardar token en localStorage o cookie
  localStorage.setItem('auth_token', data.token);
  localStorage.setItem('user_roles', JSON.stringify(data.roles));
}
```

### Authenticated Requests
```typescript
// GET /api/auth/me
const token = localStorage.getItem('auth_token');
const response = await fetch('/api/auth/me', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

### Role-Based UI
```typescript
const userRoles = JSON.parse(localStorage.getItem('user_roles') || '[]');
const isArtista = userRoles.includes('Artista');

if (isArtista) {
  // Mostrar dashboard de artista
}
```

---

**Fin del Plan de Contratos API - WPR-006**
