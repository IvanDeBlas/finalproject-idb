# Endpoints Implementation Checklist para cs-valoraciones

Este documento es una guía para desarrolladores que necesitan implementar los endpoints testados por la colección Postman.

---

## Endpoint 1: POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

### Propósito
Crear una nueva valoración sobre un acuerdo completado. Solo el autor (artista o profesional que participó en el acuerdo) puede crear la valoración.

### URL
```
POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones
```

### Autenticación
- Bearer JWT token (requerido)
- El `UserId` del token se extrae de `ClaimTypes.NameIdentifier`

### Path Parameters
```
acuerdoId: Guid (requerido) - ID del acuerdo completado
```

### Request Body
```json
{
  "puntuacion": 5,
  "comentario": "Excelente trabajo, muy profesional y puntual."
}
```

**Validaciones de entrada:**
- `puntuacion` (int): Requerido, entre 1 y 5 inclusive
  - Error 1001 si falta
  - Error 1009 si está fuera de rango
- `comentario` (string): Opcional, máximo 1000 caracteres
  - Error 1002 si supera 1000 chars

### Response 201 Created
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "puntuacion": 5,
    "comentario": "Excelente trabajo, muy profesional y puntual.",
    "fechaCreacion": "2026-03-16T10:00:00Z"
  },
  "messages": [
    {
      "message": "Valoracion enviada. Gracias por tu feedback.",
      "errorCode": "0001",
      "httpStatusCode": 201
    }
  ]
}
```

### Error Responses

#### 400 Bad Request - Validaciones
```json
{
  "data": null,
  "messages": [
    {
      "message": "La puntuacion debe ser entre 1 y 5",
      "errorCode": "1009",
      "httpStatusCode": 400
    }
  ]
}
```

**ErrorCodes posibles (400):**
- `1001` - La puntuacion es obligatoria
- `1009` - La puntuacion debe ser entre 1 y 5
- `1002` - El comentario no puede superar los 1000 caracteres
- `4014` - Solo se puede valorar acuerdos completados (BusinessRule_InvalidState)
- `4017` - Ya has dejado una valoracion para este acuerdo (BusinessRule_DuplicateAction)

#### 401 Unauthorized
```json
{
  "data": null,
  "messages": [
    {
      "message": "Token no valido o expirado",
      "errorCode": "3001",
      "httpStatusCode": 401
    }
  ]
}
```

#### 403 Forbidden
```json
{
  "data": null,
  "messages": [
    {
      "message": "No eres participante de este acuerdo",
      "errorCode": "3002",
      "httpStatusCode": 403
    }
  ]
}
```

#### 404 Not Found
```json
{
  "data": null,
  "messages": [
    {
      "message": "Acuerdo no encontrado",
      "errorCode": "2011",
      "httpStatusCode": 404
    }
  ]
}
```

#### 500 Internal Server Error
```json
{
  "data": null,
  "messages": [
    {
      "message": "Error inesperado al crear la valoracion",
      "errorCode": "5000",
      "httpStatusCode": 500
    }
  ]
}
```

**Nota:** Si ocurre `DbUpdateException` por violación del constraint único, debe capturarse y convertirse a 400 con errorCode `4017`.

### Business Logic
1. Validar que el usuario autenticado es participante del acuerdo
   - Comparar UserId del token con `AcuerdoCrowdsourcing.UserIdProveedor` (profesional)
   - O resolver Artista por UserId y comparar con `AcuerdoCrowdsourcing.ArtistaId`
   - Error 403 si no coincide
2. Validar que acuerdo está en estado Completado (EstadoAcuerdoId == 2)
   - Error 4014 si está en otro estado
3. Verificar que no existe valoración previa del mismo usuario para este acuerdo
   - Consultar `ValoracionCrowdsourcing` con `(AcuerdoId, UserIdAutor)` único
   - Error 4017 si existe
4. Determinar automáticamente `UserIdValorado`:
   - Si autor es el profesional (UserIdProveedor), valorado es el artista
   - Si autor es el artista, valorado es el profesional (UserIdProveedor)
5. Persistir `ValoracionCrowdsourcing` con `FechaCreacion = DateTime.UtcNow`
6. Retornar 201 con datos de la valoración creada

### Implementation Pattern (CQRS)
```csharp
// CreateValoracionCommand.cs (Command + Handler en mismo archivo)
public class CreateValoracionCommand : IRequest<ServiceResponse<ValoracionCreatedResultDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = null!;
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}

// ValoracionesController.cs
[HttpPost("acuerdos/{acuerdoId}/valoraciones")]
public async Task<IActionResult> CreateValoracion(
    [FromRoute] Guid acuerdoId,
    [FromBody] CreateValoracionCommand command)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    command.AcuerdoId = acuerdoId;
    command.UserId = userId;
    var response = await _mediator.Send(command);

    if (response.HasErrors)
    {
        // Discriminar por ErrorCode y retornar status correcto
        return response.Messages.Any(m => m.ErrorCode == "2011")
            ? NotFound(response)
            : response.Messages.Any(m => m.ErrorCode == "3002")
            ? StatusCode(403, response)
            : BadRequest(response);
    }
    return StatusCode(201, response);
}
```

### Testeo en Colección Postman
- **Setup requerido:** _Setup (8 pasos)
- **Request:** `Valoraciones - Happy Path → 01. POST Create Valoracion`
- **Duplicado:** `Valoraciones - Business Rules → 01. POST Duplicate`
- **Validaciones:** `Valoraciones - Validation Errors → [01-04]`

---

## Endpoint 2: GET /api/crowdsourcing/usuarios/{userId}/valoraciones

### Propósito
Obtener el resumen estadístico y listado paginado de valoraciones recibidas por un usuario específico. Cualquier usuario autenticado puede consultar valoraciones de cualquier otro usuario.

### URL
```
GET /api/crowdsourcing/usuarios/{userId}/valoraciones?page=1&pageSize=10
```

### Autenticación
- Bearer JWT token (requerido)
- No hay restricción por rol: cualquier usuario autenticado puede consultar

### Path Parameters
```
userId: string (requerido) - Identity UserId del usuario consultado
```

### Query Parameters
```
page: int (opcional, default: 1) - Número de página (1-based)
pageSize: int (opcional, default: 10) - Resultados por página (max 50)
```

### Response 200 OK
```json
{
  "data": {
    "resumen": {
      "puntuacionMedia": 4.5,
      "totalValoraciones": 12,
      "distribucion": {
        "5": 7,
        "4": 3,
        "3": 1,
        "2": 1,
        "1": 0
      }
    },
    "valoraciones": {
      "items": [
        {
          "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
          "puntuacion": 5,
          "comentario": "Excelente trabajo, muy profesional y puntual.",
          "autorNombre": "Los Rockeros",
          "autorImagenUrl": "https://storage.example.com/imagenes/los-rockeros.jpg",
          "acuerdoTituloInterno": "Mezcla EP Los Rockeros",
          "fechaCreacion": "2026-03-16T10:00:00Z"
        }
      ],
      "totalCount": 12,
      "page": 1,
      "pageSize": 10,
      "totalPages": 2
    }
  },
  "messages": []
}
```

### Empty Result (Sin valoraciones)
```json
{
  "data": {
    "resumen": {
      "puntuacionMedia": null,
      "totalValoraciones": 0,
      "distribucion": {
        "5": 0,
        "4": 0,
        "3": 0,
        "2": 0,
        "1": 0
      }
    },
    "valoraciones": {
      "items": [],
      "totalCount": 0,
      "page": 1,
      "pageSize": 10,
      "totalPages": 0
    }
  },
  "messages": []
}
```

### Error Responses

#### 400 Bad Request - Validación de parámetros
```json
{
  "data": null,
  "messages": [
    {
      "message": "El tamano de pagina debe ser entre 1 y 50",
      "errorCode": "1009",
      "httpStatusCode": 400
    }
  ]
}
```

**ErrorCodes posibles (400):**
- `1001` - El identificador de usuario es obligatorio
- `1009` - El numero de pagina debe ser mayor o igual a 1
- `1009` - El tamano de pagina debe ser entre 1 y 50

#### 401 Unauthorized
```json
{
  "data": null,
  "messages": [
    {
      "message": "Token no valido o expirado",
      "errorCode": "3001",
      "httpStatusCode": 401
    }
  ]
}
```

#### 404 Not Found
```json
{
  "data": null,
  "messages": [
    {
      "message": "Usuario no encontrado",
      "errorCode": "2000",
      "httpStatusCode": 404
    }
  ]
}
```

#### 500 Internal Server Error
```json
{
  "data": null,
  "messages": [
    {
      "message": "Error inesperado al obtener las valoraciones",
      "errorCode": "5000",
      "httpStatusCode": 500
    }
  ]
}
```

### Business Logic
1. Validar que el userId existe en Identity
   - Error 404 (2000) si no existe
2. Consultar todas las `ValoracionCrowdsourcing` donde `UserIdValorado == userId`
3. Calcular resumen:
   - `PuntuacionMedia = AVG(Puntuacion)` redondeado a 1 decimal, `null` si count=0
   - `TotalValoraciones = COUNT(*)`
   - `Distribucion = GROUP BY Puntuacion COUNT(*)` garantizando 5 claves (1-5)
4. Ejecutar paginación: `SKIP((page-1)*pageSize) TAKE(pageSize)`
5. Ordenar por `FechaCreacion DESC` (más recientes primero)
6. Proyectar campos calculados:
   - `AutorNombre`: `NombreArtistico` si autor es artista, nombre del `PerfilProfesional` si es profesional, o `UserName` de Identity si ninguno
   - `AutorImagenUrl`: URL del avatar del autor, `null` si no tiene
   - `AcuerdoTituloInterno`: Título interno del acuerdo al que pertenece la valoración
7. Retornar 200 con resumen + listado paginado

### Campos Calculados (No almacenados)
- **PuntuacionMedia:** Calculado en SQL con `AVG(Puntuacion)` en la query
- **Distribucion:** Calculado en SQL con `GROUP BY Puntuacion COUNT(*)`
- **AutorNombre:** Proyectado en la query uniendo con `Artista` o `PerfilProfesional`
- **AutorImagenUrl:** Proyectado desde perfil del autor
- **AcuerdoTituloInterno:** Proyectado desde entidad `AcuerdoCrowdsourcing`

Ninguno de estos campos se pre-calcula ni se almacena. Se proyectan en cada query.

### Implementation Pattern (CQRS)
```csharp
// GetValoracionesByUserQuery.cs (Query + Handler en mismo archivo)
public class GetValoracionesByUserQuery : IRequest<ServiceResponse<ValoracionesUsuarioDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// ValoracionesController.cs
[HttpGet("usuarios/{userId}/valoraciones")]
public async Task<IActionResult> GetValoracionesByUser(
    [FromRoute] string userId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var query = new GetValoracionesByUserQuery
    {
        UserId = userId,
        Page = page,
        PageSize = pageSize
    };
    var response = await _mediator.Send(query);

    if (response.HasErrors)
    {
        return response.Messages.Any(m => m.ErrorCode == "2000")
            ? NotFound(response)
            : StatusCode(500, response);
    }
    return Ok(response);
}
```

### Testeo en Colección Postman
- **Setup requerido:** _Setup + Happy Path (para tener valoraciones)
- **Request exitoso:** `Valoraciones - Happy Path → 02. GET Valoraciones`
- **Paginación:** `Valoraciones - Pagination → [01-02]`
- **Usuario no existe:** `Valoraciones - Not Found → 02. GET 404 - User Not Found`

---

## Resumen de Implementación

### Archivos a Crear/Modificar

**Crear:**
```
Modules/Crowdsourcing/Crowdsourcing.Application/
├── Dtos/
│   ├── ValoracionCreatedResultDto.cs
│   ├── ValoracionResumenDto.cs
│   ├── ValoracionListItemDto.cs
│   └── ValoracionesUsuarioDto.cs
├── Features/Valoraciones/
│   ├── Commands/CreateValoracionCommand.cs (+ Handler)
│   ├── Queries/GetValoracionesByUserQuery.cs (+ Handler)
│   └── Validators/
│       ├── CreateValoracionCommandValidator.cs
│       └── GetValoracionesByUserQueryValidator.cs
├── Interfaces/Services/
│   └── IValoracionCrowdsourcingService.cs
└── Mapping/
    └── ValoracionProfile.cs

Modules/Crowdsourcing/Crowdsourcing.Infra/
├── Services/
│   └── ValoracionCrowdsourcingService.cs
├── Repositories/
│   └── ValoracionCrowdsourcingRepository.cs
├── Data/Configurations/
│   └── ValoracionCrowdsourcingConfiguration.cs
└── Migrations/
    └── AddValoracionesCrowdsourcing.cs

Modules/Crowdsourcing/Crowdsourcing.WebApi/
└── Controllers/
    └── ValoracionesController.cs
```

**Modificar:**
```
Modules/Crowdsourcing/Crowdsourcing.Domain/
└── Constants/ServiceResponseMessageType.cs
    (Agregar: BusinessRule_DuplicateAction = "4017")
```

### Constantes Requeridas
```csharp
// Ya existen
Validation_Required = "1001"
Validation_MaxLength = "1002"
Validation_InvalidRange = "1009"
NotFound_Entity = "2000"
NotFound_Acuerdo = "2011"
Auth_Unauthorized = "3001"
Auth_Forbidden = "3002"
BusinessRule_InvalidState = "4014"
Internal_UnexpectedError = "5000"

// Agregar
BusinessRule_DuplicateAction = "4017"
```

### Inyecciones de Dependencia
```csharp
// En DependencyInjection.cs del módulo Crowdsourcing
services.AddScoped<IValoracionCrowdsourcingRepository, ValoracionCrowdsourcingRepository>();
services.AddScoped<IValoracionCrowdsourcingService, ValoracionCrowdsourcingService>();
```

### Constraint Unique en BD
```sql
ALTER TABLE ValoracionesCrowdsourcing
ADD CONSTRAINT UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor
UNIQUE (AcuerdoId, UserIdAutor);
```

O en Fluent API:
```csharp
builder.HasIndex(x => new { x.AcuerdoId, x.UserIdAutor })
    .IsUnique();
```

---

## Validación Pre-implementación

Antes de empezar a implementar, verificar que existen:

- [ ] Tabla `AcuerdoCrowdsourcing` con campo `EstadoAcuerdoId`
- [ ] Estados de acuerdo (Activo=1, Completado=2, Cancelado=?, etc.)
- [ ] Tabla `ValoracionCrowdsourcing` (o necesita crearse)
- [ ] Tabla `Artista` con relación a `AspNetUsers`
- [ ] Tabla `PerfilProfesional` con relación a `AspNetUsers`
- [ ] DbContext actualizado con `DbSet<ValoracionCrowdsourcing>`
- [ ] AutoMapper configurado en el módulo
- [ ] FluentValidation configurado
- [ ] MediatR configurado
- [ ] Logging configurado (ILogger<T>)

---

## Orden de Implementación Sugerido

1. **DTOs** - Definir estructura de respuestas
2. **Commands/Queries** - Definir requests
3. **Validators** - Validar inputs
4. **AutoMapper Profile** - Mapping entidades ↔ DTOs
5. **Service Interface** - Interfaz de servicios
6. **Service Implementation** - Lógica de negocio
7. **Repository** - Acceso a datos
8. **Handlers** - Orquestar validación + servicio + mapping
9. **Controller** - Exponer endpoints
10. **Migrations** - Crear tabla en BD
11. **Tests** - Ejecutar colección Postman

---

## Checklist Final

- [ ] Ambos endpoints retornan `ServiceResponse<T>` con estructura correcta
- [ ] ErrorCodes coinciden con `ServiceResponseMessageType`
- [ ] Validaciones de entrada con FluentValidation
- [ ] Reglas de negocio en Handler + Service
- [ ] Constraint único en BD previene duplicados
- [ ] Usuario autenticado extraído del JWT correctamente
- [ ] Campos calculados proyectados en query (no pre-calculados)
- [ ] Paginación con SKIP/TAKE en SQL
- [ ] Ordenamiento por FechaCreacion DESC
- [ ] Swagger documentation con `[ProducesResponseType]`
- [ ] Logging en todos los Handlers
- [ ] Race condition de duplicado maneja `DbUpdateException`
- [ ] Todos los tests Postman pasan

---

**Última actualización:** 2026-02-21
**Referencia:** api-contracts.md, CQRS.rule.md
