# Implementacion de Gestion de Secretos e Identity

**Fecha**: 2026-01-27
**Proyecto**: WePlay Rises
**Proposito**: Guia de implementacion para secretos seguros y capa de autenticacion JWT

---

## Resumen

Este documento describe la implementacion de tres componentes criticos:

| Componente | Ambiente | Estado Actual |
|------------|----------|---------------|
| User Secrets | Desarrollo | No implementado |
| Azure Key Vault | Produccion | No implementado |
| Identity + JWT | Todos | Parcialmente implementado |

**Referencia**: [ADR-004: JWT e Identity](../architecture/adrs/ADR-004_jwt-identity.md)

---

## 1. User Secrets para Desarrollo

### 1.1 Problema Actual

La JWT Key esta hardcodeada en `appsettings.Development.json` (archivo versionado):

```json
{
  "Jwt": {
    "Key": "WePlayRises_DevKey_SuperSecret_MinLength32Chars!"
  }
}
```

**Riesgo**: Cualquier persona con acceso al repositorio puede ver la clave.

### 1.2 Solucion: .NET User Secrets

User Secrets almacena configuracion sensible fuera del proyecto, en el perfil del usuario.

**Ubicacion de secretos**:
- Windows: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- Linux/Mac: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

### 1.3 Pasos de Implementacion

#### Paso 1: Inicializar User Secrets

```bash
cd src/api/WebApi
dotnet user-secrets init
```

Esto agrega un `UserSecretsId` al archivo `.csproj`:

```xml
<PropertyGroup>
  <UserSecretsId>weplayrides-dev-secrets</UserSecretsId>
</PropertyGroup>
```

#### Paso 2: Agregar Secretos

```bash
# JWT Key
dotnet user-secrets set "Jwt:Key" "MiClaveSecretaLocalParaDesarrollo_MinLength32!"

# Connection String (opcional, si se quiere separar)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=WePlayRises_Dev;Trusted_Connection=True;"
```

#### Paso 3: Verificar Secretos

```bash
dotnet user-secrets list
```

Output esperado:
```
Jwt:Key = MiClaveSecretaLocalParaDesarrollo_MinLength32!
ConnectionStrings:DefaultConnection = Server=(localdb)\\MSSQLLocalDB;...
```

#### Paso 4: Uso en Program.cs

No requiere cambios. User Secrets se carga automaticamente en Development:

```csharp
var builder = WebApplication.CreateBuilder(args);
// User Secrets se agregan automaticamente cuando Environment = Development
```

### 1.4 Limpiar Archivos Versionados

```bash
# Remover el archivo del repositorio (ya esta en .gitignore)
git rm --cached src/api/WebApi/appsettings.Development.json

# Crear version sin secretos para documentar estructura
```

**Archivo**: `appsettings.Development.json` (sin secretos)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "Jwt": {
    "Issuer": "WePlayRises",
    "Audience": "WePlayRisesUsers",
    "ExpirationMinutes": 120
  }
}
```

### 1.5 Onboarding de Nuevos Desarrolladores

Agregar al README o documentacion de setup:

```markdown
## Configuracion de Secretos Locales

1. Navegar al proyecto WebApi:
   ```bash
   cd src/api/WebApi
   ```

2. Configurar secretos:
   ```bash
   dotnet user-secrets set "Jwt:Key" "TuClaveSecretaLocal_Minimo32Caracteres!"
   ```

3. Verificar:
   ```bash
   dotnet user-secrets list
   ```
```

---

## 2. Azure Key Vault para Produccion

### 2.1 Arquitectura

```
+------------------+        +------------------+        +------------------+
|   Azure App      |  MSI   |   Azure Key      |        |   Secretos       |
|   Service        | -----> |   Vault          | -----> |   - Jwt:Key      |
|   (WebApi)       |        |                  |        |   - ConnString   |
+------------------+        +------------------+        +------------------+

MSI = Managed Service Identity (sin credenciales en codigo)
```

### 2.2 Prerequisitos en Azure

1. **Crear Key Vault**:
   ```bash
   az keyvault create \
     --name weplayrides-kv \
     --resource-group WePlayRises-RG \
     --location westeurope
   ```

2. **Agregar Secretos**:
   ```bash
   az keyvault secret set \
     --vault-name weplayrides-kv \
     --name "Jwt--Key" \
     --value "ProductionSecretKey_MuySegura_MinLength32!"

   az keyvault secret set \
     --vault-name weplayrides-kv \
     --name "ConnectionStrings--DefaultConnection" \
     --value "Server=tcp:weplayrides.database.windows.net;..."
   ```

   > **Nota**: Key Vault usa `--` como separador de jerarquia (equivale a `:` en appsettings)

3. **Habilitar Managed Identity en App Service**:
   ```bash
   az webapp identity assign \
     --name weplayrides-api \
     --resource-group WePlayRises-RG
   ```

4. **Dar acceso al Key Vault**:
   ```bash
   az keyvault set-policy \
     --name weplayrides-kv \
     --object-id <managed-identity-object-id> \
     --secret-permissions get list
   ```

### 2.3 Implementacion en Codigo

#### Paso 1: Instalar Paquetes NuGet

```bash
cd src/api/WebApi
dotnet add package Azure.Identity
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

#### Paso 2: Modificar Program.cs

```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURACION DE SECRETOS POR AMBIENTE
// ============================================
if (builder.Environment.IsProduction())
{
    // Produccion: Azure Key Vault con Managed Identity
    var keyVaultName = builder.Configuration["KeyVault:Name"]
        ?? throw new InvalidOperationException("KeyVault:Name not configured");

    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential());
}
// Desarrollo: User Secrets (cargados automaticamente)

// Resto de la configuracion...
```

#### Paso 3: Configuracion en appsettings.json

```json
{
  "KeyVault": {
    "Name": "weplayrides-kv"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 2.4 Estructura de Secretos en Key Vault

| Nombre en Key Vault | Equivale a appsettings | Valor |
|---------------------|------------------------|-------|
| `Jwt--Key` | `Jwt:Key` | Clave JWT produccion |
| `Jwt--Issuer` | `Jwt:Issuer` | WePlayRises |
| `Jwt--Audience` | `Jwt:Audience` | WePlayRisesUsers |
| `ConnectionStrings--DefaultConnection` | `ConnectionStrings:DefaultConnection` | Azure SQL connection string |

### 2.5 Fallback para Desarrollo Local con Azure

Si se quiere probar Key Vault en desarrollo:

```csharp
if (builder.Environment.IsProduction() ||
    builder.Configuration.GetValue<bool>("UseKeyVault"))
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

    // DefaultAzureCredential intenta en orden:
    // 1. Environment variables
    // 2. Managed Identity
    // 3. Visual Studio credentials
    // 4. Azure CLI credentials
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential());
}
```

---

## 3. Implementacion de Identity y JWT

### 3.1 Arquitectura (segun ADR-004)

```
+------------------+        +------------------+        +------------------+
|   Frontend       |  JWT   |    WebApi        |        |   SQL Server     |
|   (Next.js/Vite) | -----> |    .NET 8        | -----> |   Identity       |
|                  |        |                  |        |   Tables         |
+------------------+        +------------------+        +------------------+

Flujo:
1. Usuario hace login con email/password
2. API valida credenciales con Identity
3. API genera JWT con claims (UserId, Role)
4. Frontend almacena JWT y lo envia en headers
5. API valida JWT en cada request
```

### 3.2 Modelo de Usuario Extendido

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Domain/Model/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace WePlayRises.UserAccess.Domain.Model;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? NombreCompleto { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Navegacion a Artista (si el usuario es artista)
    public Artista? Artista { get; set; }
}
```

### 3.3 Roles del Sistema

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Domain/Constants/Roles.cs`

```csharp
namespace WePlayRises.UserAccess.Domain.Constants;

public static class Roles
{
    public const string Artista = "Artista";
    public const string Fan = "Fan";
    public const string Admin = "Admin";
}
```

### 3.4 Configuracion de Identity en DbContext

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Infra/Context/UserAccessContext.cs`

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Infra.Context;

public class UserAccessContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public UserAccessContext(DbContextOptions<UserAccessContext> options)
        : base(options)
    {
    }

    public DbSet<Artista> Artistas => Set<Artista>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Renombrar tablas de Identity (opcional)
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        // Configuracion de Artista
        builder.Entity<Artista>(entity =>
        {
            entity.HasOne(a => a.User)
                  .WithOne(u => u.Artista)
                  .HasForeignKey<Artista>(a => a.UserId);
        });
    }
}
```

### 3.5 Servicio de Generacion de Tokens

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Application/Services/TokenService.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Services;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WePlayRises";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "WePlayRisesUsers";
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.NombreCompleto ?? user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Agregar roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 3.6 Command de Login

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Application/Features/Auth/Commands/LoginCommand.cs`

```csharp
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Core.Models;
using WePlayRises.UserAccess.Application.Services;
using WePlayRises.UserAccess.Domain.Constants;
using WePlayRises.UserAccess.Domain.Model;

namespace WePlayRises.UserAccess.Application.Features.Auth.Commands;

// ============================================
// COMMAND
// ============================================
public class LoginCommand : IRequest<ServiceResponse<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? NombreCompleto { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

// ============================================
// VALIDATOR
// ============================================
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

// ============================================
// HANDLER
// ============================================
public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResponse<LoginResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginCommand> _validator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IValidator<LoginCommand> validator,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validacion
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<LoginResponse>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Buscar usuario
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
                return new ServiceResponse<LoginResponse>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Credenciales invalidas",
                            ErrorCode = ServiceResponseMessageType.Auth_InvalidCredentials
                        }
                    }
                };
            }

            // 3. Verificar password
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} is locked out", request.Email);
                    return new ServiceResponse<LoginResponse>
                    {
                        Messages = new List<ServiceResponseMessage>
                        {
                            new()
                            {
                                Message = "Cuenta bloqueada temporalmente",
                                ErrorCode = ServiceResponseMessageType.Auth_AccountLocked
                            }
                        }
                    };
                }

                _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                return new ServiceResponse<LoginResponse>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Credenciales invalidas",
                            ErrorCode = ServiceResponseMessageType.Auth_InvalidCredentials
                        }
                    }
                };
            }

            // 4. Obtener roles
            var roles = await _userManager.GetRolesAsync(user);

            // 5. Generar token
            var token = _tokenService.GenerateToken(user, roles);

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            // 6. Retornar respuesta exitosa
            return new ServiceResponse<LoginResponse>
            {
                Data = new LoginResponse
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(60),
                    Email = user.Email!,
                    NombreCompleto = user.NombreCompleto,
                    Roles = roles
                },
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Login exitoso",
                        ErrorCode = ServiceResponseMessageType.Success
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return new ServiceResponse<LoginResponse>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Error inesperado durante el login",
                        ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError
                    }
                }
            };
        }
    }
}
```

### 3.7 Registro de Servicios

**Ubicacion**: `src/api/Modules/UserAccess/UserAccess.Infra/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using WePlayRises.UserAccess.Application.Services;

namespace WePlayRises.UserAccess.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddUserAccessServices(this IServiceCollection services)
    {
        // Token Service
        services.AddScoped<ITokenService, TokenService>();

        // Otros servicios del modulo...

        return services;
    }
}
```

### 3.8 Configuracion Completa en Program.cs

```csharp
using System.Text;
using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WePlayRises.UserAccess.Domain.Model;
using WePlayRises.UserAccess.Infra.Context;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. CONFIGURACION DE SECRETOS
// ============================================
if (builder.Environment.IsProduction())
{
    var keyVaultName = builder.Configuration["KeyVault:Name"]
        ?? throw new InvalidOperationException("KeyVault:Name not configured");

    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}
// Desarrollo: User Secrets cargados automaticamente

// ============================================
// 2. DATABASE
// ============================================
builder.Services.AddDbContext<UserAccessContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// 3. IDENTITY
// ============================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<UserAccessContext>()
.AddDefaultTokenProviders();

// ============================================
// 4. JWT AUTHENTICATION
// ============================================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WePlayRises";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WePlayRisesUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Sin tolerancia de tiempo
    };
});

// ============================================
// 5. AUTHORIZATION POLICIES (opcional)
// ============================================
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireArtista", policy =>
        policy.RequireRole("Artista"))
    .AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin"));

// Resto de la configuracion...
```

### 3.9 Uso en Controllers

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePlayRises.UserAccess.Domain.Constants;

[ApiController]
[Route("api/[controller]")]
public class CampaniasController : ControllerBase
{
    // Publico - cualquiera puede ver
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        // ...
    }

    // Solo usuarios autenticados
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        // ...
    }

    // Solo artistas pueden crear
    [HttpPost]
    [Authorize(Roles = Roles.Artista)]
    public async Task<IActionResult> Create(CreateCampaniaCommand command)
    {
        // ...
    }

    // Solo el dueno o admin puede eliminar
    [HttpDelete("{id}")]
    [Authorize(Policy = "RequireArtista")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Verificar ownership
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // ...
    }
}
```

---

## 4. Checklist de Implementacion

### Desarrollo Local

- [ ] Ejecutar `dotnet user-secrets init` en WebApi
- [ ] Configurar `Jwt:Key` con `dotnet user-secrets set`
- [ ] Remover secretos de `appsettings.Development.json`
- [ ] Ejecutar `git rm --cached` para archivos con secretos
- [ ] Documentar proceso en README

### Produccion (Azure)

- [ ] Crear Azure Key Vault
- [ ] Agregar secretos: `Jwt--Key`, `ConnectionStrings--DefaultConnection`
- [ ] Habilitar Managed Identity en App Service
- [ ] Configurar politicas de acceso en Key Vault
- [ ] Instalar paquetes: `Azure.Identity`, `Azure.Extensions.AspNetCore.Configuration.Secrets`
- [ ] Modificar Program.cs para cargar Key Vault en produccion
- [ ] Rotar todas las claves expuestas previamente

### Identity + JWT

- [ ] Crear `ApplicationUser` extendiendo `IdentityUser<Guid>`
- [ ] Crear `TokenService` con `ITokenService`
- [ ] Implementar `LoginCommand` con validacion y generacion de token
- [ ] Registrar servicios en `DependencyInjection.cs`
- [ ] Configurar Identity y JWT en `Program.cs`
- [ ] Agregar `[Authorize]` a endpoints protegidos
- [ ] Crear roles iniciales (Artista, Fan, Admin)

---

## 5. Constantes de Autenticacion

Agregar a `ServiceResponseMessageType.cs`:

```csharp
// Auth (3000-3999)
public const string Auth_Unauthorized = "3001";
public const string Auth_Forbidden = "3002";
public const string Auth_InvalidCredentials = "3003";
public const string Auth_AccountLocked = "3004";
public const string Auth_TokenExpired = "3005";
public const string Auth_TokenInvalid = "3006";
```

---

## Referencias

- [ADR-004: JWT e Identity](../architecture/adrs/ADR-004_jwt-identity.md)
- [.NET User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
