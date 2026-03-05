# Mejoras del Monolito Modular - WebApi .NET 8

**Fecha creacion:** 2026-02-21
**Estado:** Completado
**Total tareas:** 8
**Completadas:** 8
**Documento fuente:** N/A (generado desde conversacion de analisis)

---

## Objetivo

Aplicar buenas practicas de .NET 8 al monolito modular de WePlay Rises API, priorizando robustez, reduccion de boilerplate y preparacion para produccion. Las mejoras estan ordenadas por impacto/esfuerzo, desde lo mas critico (P0) hasta mejoras progresivas (P2).

---

## Instrucciones de Uso

Para ejecutar la siguiente tarea pendiente:

```
/tasks/run-auto-task tasks/auto-tasks/20260221_mejoras-monolito-modular-api.md
```

Claude Code:
1. Leera este documento
2. Identificara la primera tarea con `[ ]` (pendiente)
3. Ejecutara las acciones
4. Marcara la tarea como `[x]` (completada)
5. Actualizara el contador de completadas

---

## Progreso

```
[1] Infraestructura  ██████████   Registrar HttpExceptionFilter + ProblemDetails
[2] Infraestructura  ██████████   ICurrentUserService + FromServiceResponse helper
[3] Infraestructura  ██████████   Health Checks + Response Compression
[4] Kernel           ██████████   Inicializar ServiceResponse.Messages + fixes
[5] Controllers      ██████████   Refactorizar CampaniasController con nuevos helpers
[6] Controllers      ██████████   Refactorizar controllers restantes (Crowdfunding + UserAccess)
[7] Controllers      ██████████   Refactorizar controllers Crowdsourcing
[8] Seguridad        ██████████   Rate Limiting en endpoints publicos
────────────────────────────────────────
TOTAL             [8/8] ██████████
```

---

## Tarea 1: Registrar HttpExceptionFilter + ProblemDetails en Pipeline

- [x] **Configurar manejo global de excepciones en Program.cs**

### Contexto
El proyecto tiene un `HttpExceptionFilter` en `BuildingBlocks/Kernel/Http/Filters/HttpExceptionFilter.cs` que mapea excepciones custom (ResourceNotFoundException, ValidationException, ForbiddenAccessException, etc.) a HTTP status codes y ServiceResponse. Sin embargo, este filtro **no esta registrado** en el pipeline de `Program.cs`. Ademas, no se usa `ProblemDetails` ni `UseExceptionHandler` como recomienda .NET 8.

### Acciones
1. En `Program.cs`, registrar `HttpExceptionFilter` como filtro global de MVC:
   ```csharp
   builder.Services.AddControllers(options =>
   {
       options.Filters.Add<HttpExceptionFilter>();
   });
   ```
   Reemplaza el `builder.Services.AddControllers();` existente (linea 26).

2. Agregar `ProblemDetails` service:
   ```csharp
   builder.Services.AddProblemDetails();
   ```

3. Agregar al pipeline de middleware (entre `builder.Build()` y `UseSwagger`):
   ```csharp
   app.UseExceptionHandler();
   app.UseStatusCodePages();
   ```

4. Asegurar que el `using` de `HttpExceptionFilter` este en Program.cs:
   ```csharp
   using WePlayRises.BuildingBlocks.Kernel.Http.Filters;
   ```

5. Verificar que `HttpExceptionFilter` inyecta `IOptions<ServiceOptions>` - si `ServiceOptions` no esta registrado en DI, registrarlo o simplificar el constructor del filtro para que no lo requiera.

6. Verificar que el proyecto compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `Program.cs` actualizado con filtro global, ProblemDetails y middleware de excepciones
- Proyecto compila sin errores

### Criterios de Completado
- [x] `HttpExceptionFilter` registrado como filtro global en `AddControllers`
- [x] `AddProblemDetails()` agregado al service container
- [x] `UseExceptionHandler()` y `UseStatusCodePages()` en el pipeline
- [x] Proyecto compila exitosamente con `dotnet build`

---

## Tarea 2: Crear ICurrentUserService + FromServiceResponse Helper

- [x] **Eliminar boilerplate de extraccion de UserId y conversion de ServiceResponse a IActionResult**

### Contexto
En `CampaniasController.cs` (y otros controllers autenticados), hay un bloque de ~10 lineas que se repite en cada action para extraer el `UserId` del JWT token:
```csharp
var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (!Guid.TryParse(userIdClaim, out var artistaId))
{
    return Unauthorized(new ServiceResponse<T> { Messages = ... });
}
```
Este patron aparece 4+ veces solo en CampaniasController. Ademas, la conversion de `ServiceResponse<T>` a `IActionResult` con switch sobre ErrorCode se repite en cada action (~8 veces).

### Acciones
1. **Crear `ICurrentUserService`** en `BuildingBlocks/Kernel/Services/`:
   ```csharp
   // ICurrentUserService.cs
   public interface ICurrentUserService
   {
       Guid? UserId { get; }
       string? UserName { get; }
       bool IsAuthenticated { get; }
   }
   ```

2. **Crear implementacion `CurrentUserService`** en `WebApi/Services/`:
   ```csharp
   // CurrentUserService.cs
   public class CurrentUserService : ICurrentUserService
   {
       private readonly IHttpContextAccessor _httpContextAccessor;

       public CurrentUserService(IHttpContextAccessor httpContextAccessor)
       {
           _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
       }

       public Guid? UserId
       {
           get
           {
               var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
               return Guid.TryParse(claim, out var id) ? id : null;
           }
       }

       public string? UserName => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

       public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
   }
   ```

3. **Registrar en Program.cs**:
   ```csharp
   builder.Services.AddHttpContextAccessor();
   builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
   ```

4. **Agregar metodo `FromServiceResponse<T>`** en `BaseLoggerController.cs`:
   ```csharp
   protected IActionResult FromServiceResponse<T>(ServiceResponse<T> response, HttpStatusCode successCode = HttpStatusCode.OK)
   {
       if (response.IsSuccess)
       {
           return successCode == HttpStatusCode.Created
               ? StatusCode((int)HttpStatusCode.Created, response)
               : Ok(response);
       }

       var mostSevere = response.Messages
           .Select(m => m.HttpStatusCode)
           .OrderByDescending(c => (int)c)
           .FirstOrDefault();

       return StatusCode((int)mostSevere, response);
   }
   ```
   Nota: Este metodo reutiliza la logica que ya existe en `ProcessServiceResponse` pero la expone como `protected` para uso directo en actions.

5. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `ICurrentUserService.cs` en BuildingBlocks/Kernel/Services/
- `CurrentUserService.cs` en WebApi/Services/
- `BaseLoggerController.cs` actualizado con `FromServiceResponse<T>`
- `Program.cs` con registro de ICurrentUserService
- Proyecto compila sin errores

### Criterios de Completado
- [x] Interface `ICurrentUserService` creada con `UserId`, `UserName`, `IsAuthenticated`
- [x] Implementacion `CurrentUserService` usando `IHttpContextAccessor`
- [x] Registrado en DI como Scoped
- [x] `FromServiceResponse<T>` agregado a `BaseLoggerController`
- [x] Proyecto compila exitosamente

---

## Tarea 3: Agregar Health Checks + Response Compression

- [x] **Configurar health checks para SQL Server y compresion de respuestas**

### Contexto
El proyecto no tiene health checks configurados, necesarios para despliegues en Docker/Azure. Tampoco tiene compresion de respuestas, lo cual impacta el rendimiento en listas grandes de campanias/rewards.

### Acciones
1. **Agregar paquete NuGet** de health checks para EF Core (ya incluido en ASP.NET Core):
   ```bash
   cd src/api/WebApi
   dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
   ```

2. **Configurar Health Checks** en `Program.cs` (despues de registrar DbContexts):
   ```csharp
   builder.Services.AddHealthChecks()
       .AddDbContextCheck<UserAccessContext>("useraccess-db")
       .AddDbContextCheck<CrowdfundingContext>("crowdfunding-db")
       .AddDbContextCheck<CoreContext>("core-db")
       .AddDbContextCheck<CrowdsourcingContext>("crowdsourcing-db");
   ```

3. **Mapear endpoint** en el pipeline (despues de `MapControllers`):
   ```csharp
   app.MapHealthChecks("/health");
   ```

4. **Agregar Response Compression** en `Program.cs`:
   ```csharp
   builder.Services.AddResponseCompression(options =>
   {
       options.EnableForHttps = true;
   });
   ```

5. **Agregar al pipeline** (antes de `UseHttpsRedirection`):
   ```csharp
   app.UseResponseCompression();
   ```

6. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `Program.cs` con health checks para los 4 DbContexts
- Endpoint `/health` funcional
- Response compression habilitada
- `.csproj` actualizado si se agrego paquete NuGet

### Criterios de Completado
- [x] Health checks registrados para los 4 DbContexts
- [x] Endpoint `/health` mapeado en el pipeline
- [x] Response compression configurada y en pipeline
- [x] Proyecto compila exitosamente

---

## Tarea 4: Inicializar ServiceResponse.Messages + Mejoras Kernel

- [x] **Corregir inicializacion de Messages y mejorar robustez del Kernel**

### Contexto
En `BuildingBlocks/Kernel/Http/Response/ServiceResponse.cs`, la propiedad `Messages` no tiene inicializador, lo que permite `null`. La logica de `IsSuccess` maneja esto con `Messages == null`, pero es fragil - cualquier consumidor que haga `response.Messages.Add(...)` sin verificar null obtendra `NullReferenceException`.

### Acciones
1. **Actualizar `ServiceResponse.cs`** en `BuildingBlocks/Kernel/Http/Response/`:
   - En `ServiceResponse` (linea 19): cambiar `public List<ServiceResponseMessage> Messages { get; set; }` a `public List<ServiceResponseMessage> Messages { get; set; } = new();`
   - En `ServiceResponse<T>` (linea 42): mismo cambio: `public List<ServiceResponseMessage> Messages { get; set; } = new();`

2. **Actualizar `IServiceResponse`** (linea 10): Verificar que la interface no necesita cambio (solo define la propiedad, el inicializador va en la implementacion).

3. **Revisar `IsSuccess`**: Con Messages inicializado a `new()`, la condicion `Messages == null` ya no aplica. Simplificar:
   ```csharp
   public bool IsSuccess => Messages.Count == 0 || !Messages.Any(m =>
       !string.IsNullOrEmpty(m.ErrorCode) ||
       (m.HttpStatusCode != HttpStatusCode.OK &&
        m.HttpStatusCode != HttpStatusCode.Created));
   ```

4. **Buscar y verificar** que no haya codigo que dependa de `Messages == null` como estado valido:
   ```bash
   # Buscar usos de Messages == null o Messages = null
   grep -r "Messages == null\|Messages = null\|\.Messages\s*=" src/api/ --include="*.cs"
   ```

5. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `ServiceResponse.cs` actualizado con `Messages` inicializado
- `IsSuccess` simplificado
- Sin regresiones en compilacion

### Criterios de Completado
- [x] `Messages` inicializado con `= new()` en ambas clases ServiceResponse
- [x] `IsSuccess` no depende de `Messages == null`
- [x] No hay regresiones en otros archivos que usan ServiceResponse
- [x] Proyecto compila exitosamente

---

## Tarea 5: Refactorizar CampaniasController con Nuevos Helpers

- [x] **Aplicar ICurrentUserService y FromServiceResponse en CampaniasController**

### Contexto
`CampaniasController` en `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/CampaniasController.cs` es el controller mas grande (479 lineas, 8 actions). Tiene el patron repetido de extraccion de UserId (4 veces) y conversion manual de ServiceResponse a IActionResult (8 veces). Es el candidato ideal para validar el refactoring antes de aplicarlo a los demas controllers.

### Acciones
1. **Cambiar herencia** de `ControllerBase` a `BaseLoggerController` e inyectar `ICurrentUserService`:
   ```csharp
   public class CampaniasController : BaseLoggerController
   {
       private readonly IMediator _mediator;
       private readonly ICurrentUserService _currentUser;

       public CampaniasController(
           IMediator mediator,
           ICurrentUserService currentUser,
           ILogger<CampaniasController> logger) : base(logger)
       {
           _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
           _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
       }
   }
   ```

2. **Refactorizar cada action autenticada** reemplazando el bloque de parseo de token por:
   ```csharp
   var artistaId = _currentUser.UserId;
   if (artistaId == null)
       return Unauthorized(new ServiceResponse<T> { Messages = ... });
   ```

3. **Reemplazar los switch de error** por `FromServiceResponse`:
   ```csharp
   // Antes (8+ lineas):
   if (!result.IsSuccess)
   {
       var errorCode = result.Messages.FirstOrDefault()?.ErrorCode;
       return errorCode switch { ... };
   }
   return Ok(result);

   // Despues (1 linea):
   return FromServiceResponse(result);
   ```
   Para el caso de `Create` que retorna 201:
   ```csharp
   return FromServiceResponse(result, HttpStatusCode.Created);
   ```

4. **Mover `CreateBackingRequest`** (lineas 484-490) a su propio archivo o al namespace de DTOs de Application.

5. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `CampaniasController.cs` refactorizado (~200 lineas reducidas a ~120)
- `CreateBackingRequest` movido a archivo separado si corresponde
- Proyecto compila sin errores

### Criterios de Completado
- [x] CampaniasController hereda de BaseLoggerController
- [x] ICurrentUserService inyectado y usado en actions autenticadas
- [x] FromServiceResponse usado en todas las actions
- [x] Eliminado todo boilerplate de parseo de token repetido
- [x] Proyecto compila exitosamente

---

## Tarea 6: Refactorizar Controllers Restantes (Crowdfunding + UserAccess)

- [x] **Aplicar el mismo patron de refactoring a PedidosController, RewardsController, DashboardController, AuthController y ArtistasController**

### Contexto
Tras validar el patron en CampaniasController (Tarea 5), aplicar el mismo refactoring a los controllers de los modulos Crowdfunding y UserAccess. Los controllers son:
- `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/PedidosController.cs`
- `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/RewardsController.cs`
- `Modules/Crowdfunding/WePlayRises.Crowdfunding.WebApi/Controllers/DashboardController.cs`
- `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/AuthController.cs`
- `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/ArtistasController.cs`

### Acciones
1. Para cada controller:
   a. Cambiar herencia a `BaseLoggerController` si no la tiene
   b. Inyectar `ICurrentUserService` donde se extraiga UserId del token
   c. Reemplazar bloques de parseo de token por `_currentUser.UserId`
   d. Reemplazar switches de error por `FromServiceResponse(result)`
   e. Mover Request DTOs sueltos a archivos/namespace apropiados

2. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

3. Si AuthController tiene logica especial de login/registro que no usa ServiceResponse, dejar esas actions sin cambio.

### Entregables
- 5 controllers refactorizados con patron consistente
- Proyecto compila sin errores

### Criterios de Completado
- [x] Los 5 controllers usan BaseLoggerController + ICurrentUserService + FromServiceResponse
- [x] No queda boilerplate de parseo de token repetido
- [x] Proyecto compila exitosamente
- [x] Request DTOs no estan al final de archivos de controller

---

## Tarea 7: Refactorizar Controllers Crowdsourcing

- [x] **Aplicar refactoring a los 8 controllers del modulo Crowdsourcing**

### Contexto
El modulo Crowdsourcing tiene 7 controllers que necesitan el mismo refactoring. Son:
- `NecesidadesCrowdsourcingController.cs`
- `PropuestasCrowdsourcingController.cs`
- `AcuerdosCrowdsourcingController.cs`
- `EntregablesCrowdsourcingController.cs`
- `ConversacionesCrowdsourcingController.cs`
- `CrowdsourcingTemplatesController.cs`
- `MaestrasCrowdsourcingController.cs`

Todos en `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/`.

### Acciones
1. Para cada controller:
   a. Cambiar herencia a `BaseLoggerController`
   b. Inyectar `ICurrentUserService` donde aplique
   c. Reemplazar parseo de token por `_currentUser.UserId`
   d. Reemplazar switches de error por `FromServiceResponse(result)`
   e. Mover Request DTOs si estan inline

2. Nota: `MaestrasCrowdsourcingController` probablemente solo tiene queries publicas - verificar si necesita ICurrentUserService.

3. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- 7 controllers refactorizados
- Patron consistente en todo el modulo Crowdsourcing
- Proyecto compila sin errores

### Criterios de Completado
- [x] Los 8 controllers usan BaseLoggerController + FromServiceResponse
- [x] ICurrentUserService inyectado solo donde se necesita autenticacion
- [x] Proyecto compila exitosamente
- [x] No queda boilerplate repetido

---

## Tarea 8: Configurar Rate Limiting en Endpoints Publicos

- [x] **Proteger endpoints AllowAnonymous con rate limiting built-in de .NET 8**

### Contexto
Los endpoints con `[AllowAnonymous]` como `GET /api/campanias`, `GET /api/campanias/{id}`, `POST /api/campanias/{id}/backings` estan expuestos a abuso sin ningun limite de peticiones. .NET 8 incluye rate limiting built-in que no requiere paquetes externos.

### Acciones
1. **Configurar Rate Limiter** en `Program.cs`:
   ```csharp
   using System.Threading.RateLimiting;

   builder.Services.AddRateLimiter(options =>
   {
       options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

       // Limite global por IP
       options.AddPolicy("public-api", httpContext =>
           RateLimitPartition.GetFixedWindowLimiter(
               partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
               factory: _ => new FixedWindowRateLimiterOptions
               {
                   PermitLimit = 60,
                   Window = TimeSpan.FromMinutes(1),
                   QueueLimit = 0
               }));

       // Limite mas estricto para mutaciones anonimas (backings)
       options.AddPolicy("public-mutation", httpContext =>
           RateLimitPartition.GetFixedWindowLimiter(
               partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
               factory: _ => new FixedWindowRateLimiterOptions
               {
                   PermitLimit = 10,
                   Window = TimeSpan.FromMinutes(1),
                   QueueLimit = 0
               }));
   });
   ```

2. **Agregar al pipeline** (despues de `UseCors`, antes de `UseAuthentication`):
   ```csharp
   app.UseRateLimiter();
   ```

3. **Aplicar a endpoints publicos** con atributo `[EnableRateLimiting("public-api")]`:
   - `GET /api/campanias` - policy `public-api`
   - `GET /api/campanias/{id}` - policy `public-api`
   - `POST /api/campanias/{id}/backings` - policy `public-mutation`

4. Agregar using necesario en controllers: `using Microsoft.AspNetCore.RateLimiting;`

5. Verificar que compila: `dotnet build src/api/WebApi/WePlayRises.WebApi.csproj`

### Entregables
- `Program.cs` con rate limiter configurado (2 policies)
- Endpoints publicos protegidos con rate limiting
- Proyecto compila sin errores

### Criterios de Completado
- [x] Rate limiter configurado con policy `public-api` (60/min) y `public-mutation` (10/min)
- [x] `UseRateLimiter()` en el pipeline
- [x] Endpoints AllowAnonymous decorados con `[EnableRateLimiting]`
- [x] Proyecto compila exitosamente

---

## Registro de Ejecucion

| Tarea | Fecha | Duracion | Notas |
|-------|-------|----------|-------|
| 1 | 2026-02-21 | ~5 min | Filtro global + ProblemDetails + middleware. Eliminado IOptions\<ServiceOptions\> no usado del constructor de HttpExceptionFilter. |
| 2 | 2026-02-21 | ~5 min | ICurrentUserService en Kernel/Services, CurrentUserService en WebApi/Services, FromServiceResponse en BaseLoggerController, DI registrado en Program.cs. |
| 3 | 2026-02-21 | ~3 min | NuGet HealthChecks.EntityFrameworkCore, health checks para 4 DbContexts, endpoint /health, response compression con EnableForHttps. |
| 4 | 2026-02-21 | ~3 min | Messages inicializado con = new() en ambas clases ServiceResponse, IsSuccess simplificado sin null check, BaseLoggerController actualizado. |
| 5 | 2026-02-21 | ~5 min | CampaniasController refactorizado: hereda BaseLoggerController, inyecta ICurrentUserService, FromServiceResponse en todas las actions. GetEffectiveHttpStatusCode agregado para derivar HTTP status desde ErrorCode prefix. CreateBackingRequest movido a Application/Dtos. Kernel.csproj referenciado en WebApi.csproj. |
| 6 | 2026-02-21 | ~5 min | 5 controllers refactorizados: PedidosController, RewardsController, DashboardController (removido HandleResult), AuthController, ArtistasController. Todos heredan BaseLoggerController, usan FromServiceResponse. ICurrentUserService inyectado donde hay token parsing (Dashboard, Auth, Artistas). Kernel.csproj agregado a UserAccess.WebApi.csproj. Constructores con ?? throw. |
| 7 | 2026-02-21 | ~5 min | 8 controllers refactorizados (7 originales + ValoracionesController): NecesidadesCrowdsourcing, PropuestasCrowdsourcing, AcuerdosCrowdsourcing, EntregablesCrowdsourcing, ConversacionesCrowdsourcing, CrowdsourcingTemplates, MaestrasCrowdsourcing, Valoraciones. Todos heredan BaseLoggerController, usan FromServiceResponse. ICurrentUserService inyectado en 7 de 8 (MaestrasCrowdsourcing solo tiene queries publicas). Kernel.csproj agregado a Crowdsourcing.WebApi.csproj. Eliminados GetUserId/GetArtistaId privados. DeleteMilestone mantiene NoContent() para 204. |
| 8 | 2026-02-21 | ~3 min | Rate limiting configurado con 2 policies: public-api (60 req/min) y public-mutation (10 req/min). UseRateLimiter() en pipeline despues de UseCors. 5 endpoints AllowAnonymous en CampaniasController decorados: GetAll, GetById, GetBackings, GetStats con public-api; CreateBacking con public-mutation. |

---

*Ultima actualizacion: 2026-02-21 (Tarea 8 - COMPLETADO)*
