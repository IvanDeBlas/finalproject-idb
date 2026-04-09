# Contratos API: cs-valoraciones (US-CS-06)

**Fecha:** 2026-02-21
**Modulo:** Crowdsourcing
**Feature:** cs-valoraciones - Valoraciones Bidireccionales

---

## Hallazgos del Analisis de Codigo Existente

Antes de los contratos, se registran los hallazgos criticos del codigo actual que impactan el diseno:

### ServiceResponseMessageType.cs (Crowdsourcing) - Estado Real

| Constante | Codigo Real | Nota |
|-----------|------------|------|
| `Validation_InvalidRange` | `"1009"` | **YA EXISTE** - El contracts.md proponia "1014" incorrectamente |
| `BusinessRule_InvalidState` | `"4014"` | **YA EXISTE** - Aplica para "acuerdo no esta Completado" |
| `BusinessRule_DuplicateAction` | NO EXISTE | Debe agregarse como `"4017"` (siguiente libre) |

### PaginatedResponse ya existe en el modulo

El tipo `PaginatedResponse<T>` existe en:
`WePlayRises.Crowdsourcing.Application/Dtos/PaginatedResponse.cs`

Incluye `TotalPages` calculado. El plan usa este tipo existente con el nombre correcto del proyecto.

### ValoracionCrowdsourcing - Tipo de Puntuacion

En la entidad existente `ValoracionCrowdsourcing`, el campo `Puntuacion` es de tipo `byte` (no `int`).
El mapping `CreateValoracionCommand.Puntuacion (int)` -> `ValoracionCrowdsourcing.Puntuacion (byte)` requiere conversion explicita.

### Patron de Controller observado en AcuerdosCrowdsourcingController

- `GetUserId()` via `User.FindFirstValue(ClaimTypes.NameIdentifier)`
- Discriminacion de errores por `response.Messages.Any(m => m.ErrorCode == ...)`
- `ValidateExtensions.InternalServerErrorServiceResponse<T>()` para errores internos

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Auth | Descripcion |
|--------|------|------|------|-------------|
| POST | `/api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones` | Command | Bearer JWT (participante) | Crear valoracion sobre acuerdo completado |
| GET | `/api/crowdsourcing/usuarios/{userId}/valoraciones` | Query | Bearer JWT (cualquier rol) | Obtener valoraciones recibidas por usuario |

---

## 2. Request DTOs (Commands y Queries)

### 2.1 CreateValoracionCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Commands/CreateValoracionCommand.cs`

**Implementa:** `IRequest<ServiceResponse<ValoracionCreatedResultDto>>`

**Nota de diseno:** El Command recibe del controller los campos del body mas los campos de contexto extraidos por el controller (AcuerdoId del path, UserId del JWT). El `UserIdValorado` NO se acepta del cliente; se determina en el Handler.

| Propiedad | Tipo | Origen | Requerido | Descripcion |
|-----------|------|--------|-----------|-------------|
| `AcuerdoId` | `Guid` | Path param | Si | ID del acuerdo. Asignado por el controller desde `[FromRoute]` |
| `UserId` | `string` | JWT claim | Si | Identity UserId del autor. Extraido del token por el controller |
| `Puntuacion` | `int` | Body | Si | Puntuacion de 1 a 5 estrellas |
| `Comentario` | `string?` | Body | No | Comentario opcional. Max 1000 chars |

```csharp
public class CreateValoracionCommand : IRequest<ServiceResponse<ValoracionCreatedResultDto>>
{
    // Del path param - asignado por el controller
    public Guid AcuerdoId { get; set; }

    // Del JWT claim - asignado por el controller
    public string UserId { get; set; } = null!;

    // Del request body
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}
```

**Request Body (lo que el cliente envia):**
```json
{
  "puntuacion": 5,
  "comentario": "Excelente trabajo, muy profesional y puntual."
}
```

---

### 2.2 GetValoracionesByUserQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Queries/GetValoracionesByUserQuery.cs`

**Implementa:** `IRequest<ServiceResponse<ValoracionesUsuarioDto>>`

| Propiedad | Tipo | Origen | Requerido | Default | Descripcion |
|-----------|------|--------|-----------|---------|-------------|
| `UserId` | `string` | Path param | Si | - | Identity UserId del usuario consultado |
| `Page` | `int` | Query param | No | 1 | Numero de pagina (1-based) |
| `PageSize` | `int` | Query param | No | 10 | Items por pagina. Max 50 |

```csharp
public class GetValoracionesByUserQuery : IRequest<ServiceResponse<ValoracionesUsuarioDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

---

## 3. Response DTOs

### 3.1 ValoracionCreatedResultDto

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionCreatedResultDto.cs`

Retornado en el `Data` de `ServiceResponse<ValoracionCreatedResultDto>` tras crear exitosamente.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Identificador unico de la valoracion creada |
| `Puntuacion` | `int` | Puntuacion enviada (1-5) |
| `Comentario` | `string?` | Comentario opcional enviado |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion |

```csharp
public class ValoracionCreatedResultDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

**Response 201 completo:**
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
      "httpStatusCode": 201,
      "errorCode": null
    }
  ]
}
```

---

### 3.2 ValoracionResumenDto

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionResumenDto.cs`

Resumen estadistico de todas las valoraciones recibidas por un usuario.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PuntuacionMedia` | `decimal?` | AVG(Puntuacion) con 1 decimal. Null si no hay valoraciones |
| `TotalValoraciones` | `int` | COUNT(*) total de valoraciones recibidas |
| `Distribucion` | `Dictionary<int, int>` | Histograma. Clave = estrella (1-5), valor = conteo. Siempre incluye las 5 claves aunque el valor sea 0 |

```csharp
public class ValoracionResumenDto
{
    /// <summary>
    /// AVG(Puntuacion) con 1 decimal. Null si TotalValoraciones == 0.
    /// </summary>
    public decimal? PuntuacionMedia { get; set; }

    /// <summary>
    /// COUNT(*) total de valoraciones recibidas por el usuario.
    /// </summary>
    public int TotalValoraciones { get; set; }

    /// <summary>
    /// Histograma de distribucion. Claves 1..5 siempre presentes aunque el valor sea 0.
    /// Calculado con GROUP BY Puntuacion COUNT(*) en la query SQL.
    /// </summary>
    public Dictionary<int, int> Distribucion { get; set; } = new();
}
```

**Nota de implementacion del resumen vacio:** Cuando `TotalValoraciones == 0`, retornar:
```json
{
  "puntuacionMedia": null,
  "totalValoraciones": 0,
  "distribucion": { "1": 0, "2": 0, "3": 0, "4": 0, "5": 0 }
}
```

---

### 3.3 ValoracionListItemDto

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionListItemDto.cs`

Item individual del listado paginado de valoraciones.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | Identificador unico de la valoracion |
| `Puntuacion` | `int` | Puntuacion en estrellas (1-5) |
| `Comentario` | `string?` | Comentario opcional. Null si no se envio |
| `AutorNombre` | `string` | Nombre del autor: NombreArtistico si es artista, nombre del perfil profesional si es profesional |
| `AutorImagenUrl` | `string?` | URL de imagen de perfil del autor. Null si no tiene |
| `AcuerdoTituloInterno` | `string` | TituloInterno del acuerdo al que pertenece la valoracion |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion |

```csharp
public class ValoracionListItemDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }

    /// <summary>
    /// NombreArtistico si el autor tiene perfil Artista, Nombre del PerfilProfesional
    /// si el autor es profesional. Se proyecta en el Query Handler, no en AutoMapper.
    /// </summary>
    public string AutorNombre { get; set; } = null!;

    /// <summary>
    /// URL de imagen de perfil. Null si el autor no tiene imagen configurada.
    /// </summary>
    public string? AutorImagenUrl { get; set; }

    /// <summary>
    /// TituloInterno del AcuerdoCrowdsourcing al que pertenece esta valoracion.
    /// </summary>
    public string AcuerdoTituloInterno { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }
}
```

---

### 3.4 ValoracionesUsuarioDto

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ValoracionesUsuarioDto.cs`

DTO raiz que combina resumen estadistico + listado paginado. Es el `Data` de `ServiceResponse<ValoracionesUsuarioDto>`.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Resumen` | `ValoracionResumenDto` | Estadisticas agregadas: media, total, histograma |
| `Valoraciones` | `PaginatedResponse<ValoracionListItemDto>` | Listado paginado ordenado por FechaCreacion DESC |

```csharp
public class ValoracionesUsuarioDto
{
    public ValoracionResumenDto Resumen { get; set; } = null!;
    public PaginatedResponse<ValoracionListItemDto> Valoraciones { get; set; } = null!;
}
```

**Nota:** Usar el `PaginatedResponse<T>` existente en `WePlayRises.Crowdsourcing.Application/Dtos/PaginatedResponse.cs`. NO crear un nuevo `PaginatedResult<T>`.

**Response 200 completo:**
```json
{
  "data": {
    "resumen": {
      "puntuacionMedia": 4.5,
      "totalValoraciones": 12,
      "distribucion": { "5": 7, "4": 3, "3": 1, "2": 1, "1": 0 }
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

---

## 4. Validadores

### 4.1 CreateValoracionCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Validators/CreateValoracionCommandValidator.cs`

**Nota critica:** Usar `ServiceResponseMessageType.Validation_InvalidRange = "1009"` (ya existe en el modulo con ese codigo). El contracts.md proponia "1014" pero ese codigo ya esta ocupado por `BusinessRule_InvalidState`.

| Campo | Regla FluentValidation | Mensaje | ErrorCode Constante | Codigo |
|-------|------------------------|---------|---------------------|--------|
| `Puntuacion` | `NotEqual(0)` / `GreaterThan(0)` | "La puntuacion es obligatoria" | `Validation_Required` | "1001" |
| `Puntuacion` | `InclusiveBetween(1, 5)` | "La puntuacion debe ser entre 1 y 5" | `Validation_InvalidRange` | "1009" |
| `Comentario` | `MaximumLength(1000)` cuando no vacio | "El comentario no puede superar los 1000 caracteres" | `Validation_MaxLength` | "1002" |
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` | "1001" |
| `AcuerdoId` | `NotEmpty()` | "El identificador del acuerdo es obligatorio" | `Validation_Required` | "1001" |

```csharp
public class CreateValoracionCommandValidator : AbstractValidator<CreateValoracionCommand>
{
    public CreateValoracionCommandValidator()
    {
        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El identificador del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .GreaterThan(0)
            .WithMessage("La puntuacion es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .InclusiveBetween(1, 5)
            .WithMessage("La puntuacion debe ser entre 1 y 5")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.Comentario)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comentario))
            .WithMessage("El comentario no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
```

**Nota sobre validaciones de negocio:** Las siguientes validaciones son de regla de negocio y NO van en el Validator. Van en el Handler (via Service, con `IRequestCacheService` para evitar queries duplicados):
- Verificar que el acuerdo existe (`NotFound_Acuerdo` 2011)
- Verificar que el usuario es participante del acuerdo (`Auth_Forbidden` 3002)
- Verificar que el acuerdo esta en estado `Completado` (`BusinessRule_InvalidState` 4014)
- Verificar que el usuario no ha valorado previamente (`BusinessRule_DuplicateAction` 4017 - nueva constante)

---

### 4.2 GetValoracionesByUserQueryValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Validators/GetValoracionesByUserQueryValidator.cs`

Validador de los parametros de paginacion del query.

| Campo | Regla | Mensaje | ErrorCode Constante | Codigo |
|-------|-------|---------|---------------------|--------|
| `UserId` | `NotEmpty()` | "El identificador de usuario es obligatorio" | `Validation_Required` | "1001" |
| `Page` | `GreaterThanOrEqualTo(1)` | "El numero de pagina debe ser mayor o igual a 1" | `Validation_InvalidRange` | "1009" |
| `PageSize` | `InclusiveBetween(1, 50)` | "El tamano de pagina debe ser entre 1 y 50" | `Validation_InvalidRange` | "1009" |

```csharp
public class GetValoracionesByUserQueryValidator : AbstractValidator<GetValoracionesByUserQuery>
{
    public GetValoracionesByUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El numero de pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("El tamano de pagina debe ser entre 1 y 50")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

## 5. AutoMapper Mappings

### 5.1 ValoracionProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/ValoracionProfile.cs`

| Source | Destination | Tipo | Notas |
|--------|-------------|------|-------|
| `CreateValoracionCommand` | `ValoracionCrowdsourcing` | Command -> Entity | Ignorar: Id, UserIdValorado, TipoValoracionId, FechaCreacion, Acuerdo. Conversion int -> byte para Puntuacion |
| `ValoracionCrowdsourcing` | `ValoracionCreatedResultDto` | Entity -> DTO | Mapping directo de Id, Puntuacion (byte -> int), Comentario, FechaCreacion |
| `ValoracionCrowdsourcing` | `ValoracionListItemDto` | Entity -> DTO | AutorNombre, AutorImagenUrl y AcuerdoTituloInterno se ignoran en el profile; se proyectan manualmente en el Query Handler |

```csharp
using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class ValoracionProfile : Profile
{
    public ValoracionProfile()
    {
        // Command -> Entity (para crear valoracion)
        CreateMap<CreateValoracionCommand, ValoracionCrowdsourcing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())         // Se asigna en handler con strongly-typed ID
            .ForMember(dest => dest.UserIdAutor, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserIdValorado, opt => opt.Ignore())    // Calculado en handler
            .ForMember(dest => dest.TipoValoracionId, opt => opt.Ignore())  // Opcional en MVP
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (byte)src.Puntuacion))
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())     // Asignado en handler
            .ForMember(dest => dest.Acuerdo, opt => opt.Ignore());

        // Entity -> ValoracionCreatedResultDto (respuesta del POST)
        CreateMap<ValoracionCrowdsourcing, ValoracionCreatedResultDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion));

        // Entity -> ValoracionListItemDto (proyeccion parcial - campos calculados se asignan en handler)
        CreateMap<ValoracionCrowdsourcing, ValoracionListItemDto>()
            .ForMember(dest => dest.Puntuacion, opt => opt.MapFrom(src => (int)src.Puntuacion))
            .ForMember(dest => dest.AutorNombre, opt => opt.Ignore())           // Resuelto en handler
            .ForMember(dest => dest.AutorImagenUrl, opt => opt.Ignore())        // Resuelto en handler
            .ForMember(dest => dest.AcuerdoTituloInterno, opt => opt.Ignore()); // Resuelto en handler
    }
}
```

**Nota sobre AutorNombre, AutorImagenUrl y AcuerdoTituloInterno:** Estos campos requieren consultar entidades relacionadas (Artista o PerfilProfesional segun el tipo de autor, y AcuerdoCrowdsourcing). Se proyectan en el Query Handler tras el mapping base, NO en AutoMapper, para mantener la claridad y evitar dependencias circulares en el Profile.

---

## 6. Handler: CreateValoracionCommandHandler

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Commands/CreateValoracionCommand.cs`
(Command + Handler en el MISMO archivo, segun REGLA 2 CQRS)

**Dependencias del Handler:**

| Dependencia | Interfaz | Motivo |
|-------------|----------|--------|
| `IValoracionCrowdsourcingService` | Nueva | Crear valoracion, verificar unicidad |
| `IAcuerdoCrowdsourcingService` | Existente | Obtener acuerdo para validar estado y participacion |
| `IArtistaService` | Existente (UserAccess) | Resolver ArtistaId por UserId para determinar participante/valorado |
| `IMapper` | AutoMapper | Command -> Entity, Entity -> DTO |
| `IValidator<CreateValoracionCommand>` | FluentValidation | Validacion de entrada |
| `ILogger<CreateValoracionCommandHandler>` | Microsoft.Extensions.Logging | Logging obligatorio |

**Logica de negocio del Handler (orden de ejecucion):**

```
1. Validar con FluentValidator (formato de entrada)
   └── Si invalido -> retornar ServiceResponse con errores de validacion

2. Obtener acuerdo via IRequestCacheService (clave: "acuerdo:{acuerdoId}")
   └── Si no existe -> retornar NotFoundServiceResponse(NotFound_Acuerdo)

3. Resolver participacion:
   a. Obtener Artista por UserId via IArtistaService.GetByUserIdAsync(command.UserId)
   b. Comparar: es artista si artista != null && acuerdo.ArtistaId == artista.Id
   c. Es proveedor si acuerdo.UserIdProveedor == command.UserId
   └── Si ninguno -> retornar ForbiddenServiceResponse(Auth_Forbidden)

4. Verificar que acuerdo.EstadoAcuerdoId == EstadoAcuerdoConstants.Completado (2)
   └── Si no -> retornar BadRequestServiceResponse(BusinessRule_InvalidState)

5. Verificar unicidad via IValoracionCrowdsourcingService.ExisteValoracionAsync(acuerdoId, userId)
   └── Si existe -> retornar BadRequestServiceResponse(BusinessRule_DuplicateAction)

6. Determinar UserIdValorado:
   - Si el autor es el artista -> UserIdValorado = acuerdo.UserIdProveedor
   - Si el autor es el proveedor -> UserIdValorado = UserId del artista del acuerdo
     (requiere resolver el UserId del artista via IArtistaService)

7. Mapear Command -> ValoracionCrowdsourcing via AutoMapper
   - Asignar Id = Guid.NewGuid()
   - Asignar AcuerdoId = new AcuerdoCrowdsourcingId(command.AcuerdoId)
   - Asignar UserIdValorado calculado
   - Asignar FechaCreacion = DateTime.UtcNow

8. Persistir via IValoracionCrowdsourcingService.CreateAsync(entity, ct)

9. Mapear entity -> ValoracionCreatedResultDto via AutoMapper

10. Retornar ServiceResponse exitoso con Data y message "Valoracion enviada. Gracias por tu feedback."
    con HttpStatusCode = Created
```

---

## 7. Handler: GetValoracionesByUserQueryHandler

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Valoraciones/Queries/GetValoracionesByUserQuery.cs`
(Query + Handler en el MISMO archivo, segun REGLA 2 CQRS)

**Dependencias del Handler:**

| Dependencia | Interfaz | Motivo |
|-------------|----------|--------|
| `IValoracionCrowdsourcingService` | Nueva | Obtener resumen y listado paginado |
| `IValidator<GetValoracionesByUserQuery>` | FluentValidation | Validacion de parametros |
| `ILogger<GetValoracionesByUserQueryHandler>` | Microsoft.Extensions.Logging | Logging obligatorio |

**Nota:** Este handler NO usa AutoMapper directamente para `ValoracionListItemDto` porque requiere proyeccion de campos calculados (AutorNombre, AutorImagenUrl, AcuerdoTituloInterno). El servicio devuelve la proyeccion completa.

**Logica del Handler:**

```
1. Validar con GetValoracionesByUserQueryValidator
   └── Si invalido -> retornar ServiceResponse con errores

2. Verificar que el usuario existe via IValoracionCrowdsourcingService.UsuarioExisteAsync(query.UserId)
   └── Si no existe -> retornar NotFoundServiceResponse(NotFound_Entity)

3. Obtener resumen via IValoracionCrowdsourcingService.GetResumenByUserIdAsync(userId)
   (AVG, COUNT, GROUP BY en SQL - no en memoria)

4. Obtener listado paginado via IValoracionCrowdsourcingService.GetByUserIdPagedAsync(userId, page, pageSize)
   (SKIP/TAKE + ORDER BY FechaCreacion DESC)
   - Proyeccion de AutorNombre se resuelve en el Service/Repository:
     JOIN con Artista ON UserIdAutor = Artista.UserId para nombre y avatar
     Si no hay Artista: JOIN con PerfilProfesional para nombre y avatar

5. Construir ValoracionesUsuarioDto con Resumen + Valoraciones (PaginatedResponse<ValoracionListItemDto>)

6. Retornar ServiceResponse exitoso
```

---

## 8. Nuevas Constantes a Agregar en ServiceResponseMessageType.cs

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

### Estado actual del archivo (ultimas entradas):

```csharp
public const string BusinessRule_EntregableNotReviewable = "4013";
public const string BusinessRule_InvalidState = "4014";          // YA EXISTE - para acuerdo no completado
public const string BusinessRule_ConversacionDuplicada = "4015";
public const string BusinessRule_NoRelacionConDestinatario = "4016";

public const string Internal_UnexpectedError = "5000";
public const string Internal_DatabaseError = "5001";
```

### Constante a agregar:

```csharp
// Business Rules (continuacion)
public const string BusinessRule_DuplicateAction = "4017";  // Ya existe valoracion del mismo autor para este acuerdo
```

### Analisis de uso de constantes en esta feature:

| Situacion | Constante | Codigo | Estado |
|-----------|-----------|--------|--------|
| Puntuacion no enviada | `Validation_Required` | "1001" | Ya existe |
| Comentario > 1000 chars | `Validation_MaxLength` | "1002" | Ya existe |
| Puntuacion fuera de rango | `Validation_InvalidRange` | "1009" | Ya existe |
| Acuerdo no encontrado | `NotFound_Acuerdo` | "2011" | Ya existe |
| Usuario no encontrado | `NotFound_Entity` | "2000" | Ya existe |
| Token invalido/expirado | `Auth_Unauthorized` | "3001" | Ya existe |
| No es participante | `Auth_Forbidden` | "3002" | Ya existe |
| Acuerdo no esta Completado | `BusinessRule_InvalidState` | "4014" | Ya existe |
| Ya existe valoracion | `BusinessRule_DuplicateAction` | "4017" | **NUEVA - Agregar** |
| Error inesperado | `Internal_UnexpectedError` | "5000" | Ya existe |

---

## 9. Nuevas Interfaces de Servicio

### 9.1 IValoracionCrowdsourcingService

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IValoracionCrowdsourcingService.cs`

```csharp
public interface IValoracionCrowdsourcingService
{
    /// <summary>
    /// Crea una nueva valoracion. El servicio hace SaveChanges via repositorio.
    /// </summary>
    Task<Guid> CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct);

    /// <summary>
    /// Verifica si ya existe una valoracion del mismo autor para el mismo acuerdo.
    /// Constraint unico (AcuerdoId + UserIdAutor).
    /// </summary>
    Task<bool> ExisteValoracionAsync(Guid acuerdoId, string userIdAutor, CancellationToken ct);

    /// <summary>
    /// Verifica si el userId existe en Identity (para validar el path param del GET).
    /// </summary>
    Task<bool> UsuarioExisteAsync(string userId, CancellationToken ct);

    /// <summary>
    /// Calcula el resumen estadistico: AVG(Puntuacion), COUNT(*) y GROUP BY Puntuacion.
    /// El calculo se realiza en SQL, no en memoria.
    /// </summary>
    Task<ValoracionResumenDto> GetResumenByUserIdAsync(string userId, CancellationToken ct);

    /// <summary>
    /// Retorna listado paginado de valoraciones recibidas por el usuario,
    /// ordenado por FechaCreacion DESC, con proyeccion de AutorNombre, AutorImagenUrl
    /// y AcuerdoTituloInterno. SKIP/TAKE se aplica en la query SQL.
    /// </summary>
    Task<PaginatedResponse<ValoracionListItemDto>> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct);
}
```

---

## 10. Controller: ValoracionesController

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/ValoracionesController.cs`

**Notas de diseno del controller:**
- El endpoint POST esta bajo ruta `api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones` (nested resource)
- El endpoint GET esta bajo ruta `api/crowdsourcing/usuarios/{userId}/valoraciones`
- Ambas rutas pertenecen a dominios distintos, por lo que el controller usa rutas absolutas en cada action (no ruta base comun en `[Route]`)
- Patron identico a `AcuerdosCrowdsourcingController`: `GetUserId()` via `ClaimTypes.NameIdentifier`

```csharp
[ApiController]
[Route("api/crowdsourcing")]
[Authorize]
public class ValoracionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ValoracionesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    // POST api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones
    [HttpPost("acuerdos/{acuerdoId}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateValoracion(
        [FromRoute] Guid acuerdoId,
        [FromBody] CreateValoracionCommand command)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        command.AcuerdoId = acuerdoId;
        command.UserId = userId;

        var response = await _mediator.Send(command);

        if (response.HasErrors)
        {
            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Acuerdo))
                return NotFound(response);

            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.Auth_Forbidden))
                return StatusCode(StatusCodes.Status403Forbidden, response);

            return BadRequest(response);
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    // GET api/crowdsourcing/usuarios/{userId}/valoraciones
    [HttpGet("usuarios/{userId}/valoraciones")]
    [ProducesResponseType(typeof(ServiceResponse<ValoracionesUsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            if (response.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.NotFound_Entity))
                return NotFound(response);

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        return Ok(response);
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
```

---

## 11. Tabla de Errores por Endpoint

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

| HTTP Status | ErrorCode | Constante | Mensaje | Causa |
|-------------|-----------|-----------|---------|-------|
| 400 | "1001" | `Validation_Required` | "La puntuacion es obligatoria" | `puntuacion` == 0 o no enviada |
| 400 | "1001" | `Validation_Required` | "El identificador del acuerdo es obligatorio" | `acuerdoId` es Guid.Empty |
| 400 | "1009" | `Validation_InvalidRange` | "La puntuacion debe ser entre 1 y 5" | `puntuacion` < 1 o > 5 |
| 400 | "1002" | `Validation_MaxLength` | "El comentario no puede superar los 1000 caracteres" | `comentario`.Length > 1000 |
| 400 | "4014" | `BusinessRule_InvalidState` | "Solo se puede valorar acuerdos completados" | `EstadoAcuerdoId` != `Completado` (2) |
| 400 | "4017" | `BusinessRule_DuplicateAction` | "Ya has dejado una valoracion para este acuerdo" | Ya existe `ValoracionCrowdsourcing` con mismo `(AcuerdoId, UserIdAutor)` |
| 401 | "3001" | `Auth_Unauthorized` | "Token no valido o expirado" | JWT invalido o ausente |
| 403 | "3002" | `Auth_Forbidden` | "No eres participante de este acuerdo" | `UserId` no coincide con artista ni proveedor del acuerdo |
| 404 | "2011" | `NotFound_Acuerdo` | "Acuerdo no encontrado" | `acuerdoId` no existe en DB |
| 500 | "5000" | `Internal_UnexpectedError` | "Error inesperado al crear la valoracion" | Excepcion no controlada (incluyendo DbUpdateException por violacion del constraint unico) |

**Nota sobre 500 y race condition:** Si dos requests simultaneas del mismo usuario pasan la verificacion en memoria, la segunda falla con `DbUpdateException` (violacion del constraint unico `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor`). El handler debe capturar `DbUpdateException` en el catch y retornar un `BadRequestServiceResponse` con `BusinessRule_DuplicateAction` en lugar de `InternalServerError`.

---

### GET /api/crowdsourcing/usuarios/{userId}/valoraciones

| HTTP Status | ErrorCode | Constante | Mensaje | Causa |
|-------------|-----------|-----------|---------|-------|
| 400 | "1001" | `Validation_Required` | "El identificador de usuario es obligatorio" | `userId` es null o vacio |
| 400 | "1009" | `Validation_InvalidRange` | "El numero de pagina debe ser mayor o igual a 1" | `page` < 1 |
| 400 | "1009" | `Validation_InvalidRange` | "El tamano de pagina debe ser entre 1 y 50" | `pageSize` < 1 o > 50 |
| 401 | "3001" | `Auth_Unauthorized` | "Token no valido o expirado" | JWT invalido o ausente |
| 404 | "2000" | `NotFound_Entity` | "Usuario no encontrado" | `userId` no existe en Identity |
| 500 | "5000" | `Internal_UnexpectedError` | "Error inesperado al obtener las valoraciones" | Excepcion no controlada |

---

## 12. Documentacion OpenAPI/Swagger

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

```
Summary: Crear valoracion sobre un acuerdo completado
Description: Crea una valoracion del usuario autenticado hacia la otra parte de un acuerdo
             completado. El UserIdValorado se determina automaticamente en el backend.
             Solo se puede crear una valoracion por participante por acuerdo.
             Las valoraciones son inmutables (no existe PATCH ni DELETE).

Tags: [Valoraciones, Crowdsourcing]

Parameters:
  - name: acuerdoId
    in: path
    required: true
    schema: { type: string, format: uuid }
    description: ID del acuerdo completado sobre el que se valora

Request Body:
  required: true
  content:
    application/json:
      schema:
        type: object
        required: [puntuacion]
        properties:
          puntuacion:
            type: integer
            minimum: 1
            maximum: 5
            example: 5
          comentario:
            type: string
            maxLength: 1000
            nullable: true
            example: "Excelente trabajo, muy profesional y puntual."

Responses:
  201: ServiceResponse<ValoracionCreatedResultDto> - Valoracion creada exitosamente
  400: ServiceResponse con errores de validacion o regla de negocio
  401: No autenticado (JWT invalido o ausente)
  403: No es participante del acuerdo
  404: Acuerdo no encontrado
  500: Error interno del servidor

Security: Bearer JWT requerido
```

---

### GET /api/crowdsourcing/usuarios/{userId}/valoraciones

```
Summary: Obtener valoraciones recibidas por un usuario
Description: Retorna el resumen estadistico (puntuacion media, total e histograma) y
             el listado paginado de valoraciones recibidas por el usuario indicado.
             Ordenadas por FechaCreacion descendente.
             Cualquier usuario autenticado puede consultar valoraciones de cualquier usuario.

Tags: [Valoraciones, Crowdsourcing]

Parameters:
  - name: userId
    in: path
    required: true
    schema: { type: string }
    description: Identity UserId del usuario cuyas valoraciones se consultan

  - name: page
    in: query
    required: false
    schema: { type: integer, minimum: 1, default: 1 }
    description: Numero de pagina (1-based)

  - name: pageSize
    in: query
    required: false
    schema: { type: integer, minimum: 1, maximum: 50, default: 10 }
    description: Resultados por pagina

Responses:
  200: ServiceResponse<ValoracionesUsuarioDto> - Resumen + listado paginado
  401: No autenticado
  404: Usuario no encontrado
  500: Error interno del servidor

Security: Bearer JWT requerido
```

---

## 13. Estructura de Archivos a Crear

```
src/api/Modules/Crowdsourcing/
│
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs          MODIFICAR - Agregar BusinessRule_DuplicateAction = "4017"
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── ValoracionCreatedResultDto.cs           NUEVO
│   │   ├── ValoracionResumenDto.cs                 NUEVO
│   │   ├── ValoracionListItemDto.cs                NUEVO
│   │   └── ValoracionesUsuarioDto.cs               NUEVO
│   │       (PaginatedResponse<T> ya existe - NO crear nuevo)
│   │
│   ├── Features/Valoraciones/
│   │   ├── Commands/
│   │   │   └── CreateValoracionCommand.cs          NUEVO (Command + Handler en mismo archivo)
│   │   ├── Queries/
│   │   │   └── GetValoracionesByUserQuery.cs       NUEVO (Query + Handler en mismo archivo)
│   │   └── Validators/
│   │       ├── CreateValoracionCommandValidator.cs NUEVO
│   │       └── GetValoracionesByUserQueryValidator.cs NUEVO
│   │
│   ├── Interfaces/Services/
│   │   └── IValoracionCrowdsourcingService.cs      NUEVO
│   │
│   └── Mapping/
│       └── ValoracionProfile.cs                    NUEVO
│
├── WePlayRises.Crowdsourcing.Infra/
│   ├── Services/
│   │   └── ValoracionCrowdsourcingService.cs       NUEVO (implementa IValoracionCrowdsourcingService)
│   ├── Repositories/
│   │   └── ValoracionCrowdsourcingRepository.cs   NUEVO
│   ├── Data/
│   │   └── Configurations/
│   │       └── ValoracionCrowdsourcingConfiguration.cs NUEVO (incluye constraint unico)
│   └── Migrations/
│       └── AddValoracionesCrowdsourcing.cs         NUEVO (tabla + constraint UQ_AcuerdoId_UserIdAutor)
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        └── ValoracionesController.cs               NUEVO
```

---

## 14. Checklist de Contratos

- [ ] Commands implementan `IRequest<ServiceResponse<T>>`
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command en mismo archivo (CreateValoracionCommand.cs)
- [ ] Handler + Query en mismo archivo (GetValoracionesByUserQuery.cs)
- [ ] Handler NUNCA inyecta DbContext (usa IValoracionCrowdsourcingService, IAcuerdoCrowdsourcingService, IArtistaService)
- [ ] Constructor con `?? throw new ArgumentNullException` para TODAS las dependencias
- [ ] Validators usan constantes de `ServiceResponseMessageType` (NO strings literales)
- [ ] Validators usan `.WithMessage()` Y `.WithErrorCode()` en cada regla
- [ ] `Validation_InvalidRange = "1009"` (ya existe - NO crear "1014")
- [ ] `BusinessRule_DuplicateAction = "4017"` agregada al ServiceResponseMessageType.cs
- [ ] `BusinessRule_InvalidState = "4014"` ya existe - usar para acuerdo no completado
- [ ] `PaginatedResponse<T>` existente usado en lugar de nuevo tipo
- [ ] ValoracionProfile con conversion byte<->int para Puntuacion
- [ ] Fields calculados (AutorNombre, AutorImagenUrl, AcuerdoTituloInterno) ignorados en Profile, proyectados en Handler/Service
- [ ] Controller extrae UserId del JWT via `ClaimTypes.NameIdentifier`
- [ ] Controller discrimina errores por ErrorCode para retornar 404/403/400/500
- [ ] Race condition de constraint unico manejada capturando DbUpdateException -> retornar 400
- [ ] Try-catch con `_logger.LogError` en ambos Handlers
- [ ] Services retornan entidades o DTOs de proyeccion (NO DbContext en Handler)
- [ ] `IRequestCacheService` usado en CreateValoracionCommandHandler para compartir lectura del acuerdo con el Validator
- [ ] Swagger documentation con ProducesResponseType en todos los status codes posibles
