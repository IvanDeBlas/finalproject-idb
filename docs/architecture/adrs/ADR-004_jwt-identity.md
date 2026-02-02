# ADR-004: Autenticacion con JWT y ASP.NET Core Identity

## Metadata
- **Estado**: Aceptada
- **Fecha**: 2026-01-21
- **Relacionado con**: ADR-003
- **Especifico de**: WePlay Rises

---

## Contexto

WePlay Rises necesita autenticacion y autorizacion para:
- Artistas que crean y gestionan campanas
- Fans que apoyan campanas y ven su historial
- Endpoints protegidos en la API
- Frontend Next.js que consume la API

Requisitos:
- Registro y login de usuarios
- Roles diferenciados (Artista, Fan)
- Tokens para autenticacion de API
- Compatible con Next.js frontend

---

## Decision

Usar **ASP.NET Core Identity** para gestion de usuarios y **JWT (JSON Web Tokens)** para autenticacion de API.

**Arquitectura:**

```
+------------------+        +------------------+        +------------------+
|   Next.js        |  JWT   |    WebApi        |        |   SQL Server     |
|   Frontend       | -----> |    .NET 8        | -----> |   Identity       |
|                  |        |                  |        |   Tables         |
+------------------+        +------------------+        +------------------+

Flujo:
1. Usuario hace login con email/password
2. API valida credenciales con Identity
3. API genera JWT con claims (UserId, Role)
4. Frontend almacena JWT y lo envia en headers
5. API valida JWT en cada request
```

---

## Justificacion

**Por que ASP.NET Core Identity:**
- Framework maduro y probado
- Incluye hash de passwords, lockout, confirmacion email
- Integracion nativa con EF Core
- Roles y claims out-of-the-box

**Por que JWT:**
- Stateless (no requiere sesiones en servidor)
- Estandar ampliamente adoptado
- Facil de consumir desde Next.js
- Permite incluir claims (roles, userId)

---

## Alternativas Consideradas

### Alternativa 1: Cookies de sesion
- **Descripcion**: Sesiones tradicionales server-side
- **Razon para no elegirla**: Problemas con CORS, no ideal para SPA

### Alternativa 2: OAuth2 con proveedor externo (Auth0, Firebase)
- **Descripcion**: Delegar autenticacion a servicio externo
- **Razon para no elegirla**: Costo adicional, dependencia externa para MVP

### Alternativa 3: Identity Server
- **Descripcion**: OpenID Connect completo
- **Razon para no elegirla**: Complejidad excesiva para MVP

### Alternativa 4: Simple API Keys
- **Descripcion**: API keys estaticas por usuario
- **Razon para no elegirla**: Sin soporte de roles, poco seguro

---

## Consecuencias

### Positivas
- Autenticacion robusta sin dependencias externas
- Roles integrados (Artista, Fan)
- Claims personalizados en JWT
- Facil integracion con Next.js

### Negativas
- Mas codigo que usar Auth0/Firebase
- Responsabilidad de seguridad propia
- Refresh tokens requieren implementacion adicional

### Riesgos
- **JWT robado** -> Mitigacion: Expiracion corta (1h), refresh tokens
- **Almacenamiento inseguro en frontend** -> Mitigacion: HttpOnly cookies o localStorage con cuidado

---

## Notas de Implementacion

**Modelo de Usuario:**

```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    public string? NombreCompleto { get; set; }
    public DateTime FechaRegistro { get; set; }
}
```

**Roles definidos:**

```csharp
public static class Roles
{
    public const string Artista = "Artista";
    public const string Fan = "Fan";
    public const string Admin = "Admin";
}
```

**Configuracion JWT en appsettings.json:**

```json
{
  "Jwt": {
    "Key": "SuperSecretKeyMinimo32Caracteres!",
    "Issuer": "WePlayRises",
    "Audience": "WePlayRisesApp",
    "ExpirationMinutes": 60
  }
}
```

**Registro de Identity y JWT:**

```csharp
// Program.cs
services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(options =>
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
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
    };
});
```

**Generacion de JWT:**

```csharp
public class TokenService : ITokenService
{
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.NombreCompleto ?? user.Email!)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

**Proteger endpoints:**

```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CampaniasController : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Artista)]
    public async Task<IActionResult> Create(CreateCampaniaCommand command)
    {
        // Solo artistas pueden crear campanas
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        // Cualquiera puede ver campanas
    }
}
```

**Uso desde Next.js:**

```typescript
// Guardar token despues de login
localStorage.setItem('token', response.token);

// Enviar token en requests
const response = await fetch('/api/campanias', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
```

---

## Referencias

- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt)
- [OWASP JWT Security](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
