# Contratos API: cs-explorar-propuestas

**Fecha:** 2026-02-17
**Modulo:** Crowdsourcing
**Feature:** cs-explorar-propuestas (US-CS-03)

---

## 1. Endpoints

| Metodo | Ruta | Tipo CQRS | Controller | Descripcion |
|--------|------|-----------|------------|-------------|
| GET | /api/crowdsourcing/necesidades | Query | NecesidadesCrowdsourcingController | Listado publico paginado con filtros |
| GET | /api/crowdsourcing/necesidades/{id} | Query | NecesidadesCrowdsourcingController | Detalle publico con campos calculados |
| POST | /api/crowdsourcing/necesidades/{necesidadId}/propuestas | Command | NecesidadesCrowdsourcingController | Enviar propuesta a una necesidad |
| GET | /api/crowdsourcing/propuestas/mis-propuestas | Query | PropuestasCrowdsourcingController | Mis propuestas paginadas |
| PATCH | /api/crowdsourcing/propuestas/{id}/retirar | Command | PropuestasCrowdsourcingController | Retirar propuesta propia |

**Nota sobre GET /api/crowdsourcing/necesidades/{id}:** Este endpoint ya existe en `NecesidadesCrowdsourcingController` para la vista del artista (US-CS-02). Para US-CS-03, el mismo endpoint detecta si el usuario es el artista propietario y delega a la query correcta: `GetNecesidadByIdQuery` (artista propietario) o `GetNecesidadPublicaByIdQuery` (cualquier otro usuario autenticado). El controller resuelve el `ArtistaId` del usuario y compara con el `ArtistaId` de la necesidad para determinar la query a invocar.

---

## 2. Nuevas Constantes en ServiceResponseMessageType

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

Agregar al archivo existente las siguientes constantes:

```csharp
// NotFound (2000-2999) - nuevas
public const string NotFound_Propuesta = "2010";

// Business Rules (4000-4999) - nuevas
public const string BusinessRule_AlreadyProposed   = "4003";
public const string BusinessRule_CannotProposeSelf = "4004";
public const string BusinessRule_PropuestaNotRetirable    = "4005";
public const string BusinessRule_NoProfessionalProfile    = "4006";
```

---

## 3. Request DTOs (Commands / Queries)

### 3.1 GetNecesidadesPublicasQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetNecesidadesPublicasQuery.cs`

Contiene Query + Handler en el mismo archivo (REGLA 2).

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| UserId | string | Si | JWT claim sub | Para excluir necesidades del artista autenticado |
| TipoNecesidadId | int? | No | Query param | Filtro por tipo |
| ModalidadTrabajoId | int? | No | Query param `modalidad` | Filtro por modalidad |
| PresupuestoMin | decimal? | No | Query param | Filtro por presupuesto minimo |
| PresupuestoMax | decimal? | No | Query param | Filtro por presupuesto maximo |
| Pais | string? | No | Query param | Filtro por pais |
| Search | string? | No | Query param | Busqueda en titulo y descripcion |
| OrderBy | string? | No | Query param (default: `recientes`) | Orden: `recientes`, `mayor-presupuesto`, `fecha-limite` |
| Page | int | No | Query param (default: 1) | Pagina actual |
| PageSize | int | No | Query param (default: 12, max: 50) | Items por pagina |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>>`

**No requiere Validator** (solo query de lectura con parametros opcionales; la paginacion usa defaults en el controller).

---

### 3.2 GetNecesidadPublicaByIdQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetNecesidadPublicaByIdQuery.cs`

Contiene Query + Handler en el mismo archivo (REGLA 2).

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| Id | Guid | Si | Route param | ID de la necesidad |
| UserId | string | Si | JWT claim sub | Para calcular yaPropuso, esPropietario, tienePerfilProfesional |

**Implementa:** `IRequest<ServiceResponse<NecesidadPublicaDto>>`

**No requiere Validator** (query de lectura; si la necesidad no existe el handler retorna NotFound directamente).

---

### 3.3 CreatePropuestaCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Commands/CreatePropuestaCommand.cs`

Contiene Command + Handler en el mismo archivo (REGLA 2).

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| NecesidadId | Guid | Si | Route param `necesidadId` | Necesidad a la que se propone |
| UserId | string | Si | JWT claim sub | Asignado automaticamente desde token |
| PrecioPropuesto | decimal | Si | Request body | Debe ser > 0 |
| MonedaId | int | Si | Request body | FK a MaestraMoneda |
| DiasEstimados | int? | No | Request body | Opcional; si presente, debe ser > 0 y <= 365 |
| MensajePropuesta | string | Si | Request body | Min 20 chars, max 2000 chars |

**Implementa:** `IRequest<ServiceResponse<PropuestaCreatedResultDto>>`

**Validator requerido:** `CreatePropuestaCommandValidator` (ver seccion 5.1).

**Nota sobre PerfilProfesionalId:** No se incluye en el Command. El Handler delega al `IPropuestaCrowdsourcingService.CreateAsync` que resuelve el `PerfilProfesionalId` consultando la BD por `UserId`. Si no existe PerfilProfesional, el servicio retorna error con `BusinessRule_NoProfessionalProfile`.

---

### 3.4 GetMisPropuestasQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Queries/GetMisPropuestasQuery.cs`

Contiene Query + Handler en el mismo archivo (REGLA 2).

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| UserId | string | Si | JWT claim sub | Filtra propuestas del usuario autenticado |
| EstadoPropuestaId | int? | No | Query param `estado` | Filtro por estado (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) |
| Page | int | No | Query param (default: 1) | Pagina actual |
| PageSize | int | No | Query param (default: 10, max: 50) | Items por pagina |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<MiPropuestaListDto>>>`

**No requiere Validator** (query de lectura; siempre filtra por UserId del token).

---

### 3.5 RetirarPropuestaCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Commands/RetirarPropuestaCommand.cs`

Contiene Command + Handler en el mismo archivo (REGLA 2).

| Propiedad | Tipo | Requerido | Origen | Descripcion |
|-----------|------|-----------|--------|-------------|
| Id | Guid | Si | Route param | ID de la propuesta a retirar |
| UserId | string | Si | JWT claim sub | Para validar ownership en el handler |

**Implementa:** `IRequest<ServiceResponse<RetirarPropuestaResultDto>>`

**Validator requerido:** `RetirarPropuestaCommandValidator` (ver seccion 5.2).

**Nota:** No tiene Request Body. El controller solo toma `id` de la ruta y `UserId` del token JWT.

---

## 4. Response DTOs

### 4.1 NecesidadPublicaListDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadPublicaListDto.cs`

Usado como item en `PaginatedResponse<NecesidadPublicaListDto>` para el listado publico.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la necesidad |
| Titulo | string | Titulo de la necesidad |
| Descripcion | string? | Descripcion truncada a 150 caracteres (truncado en proyeccion de la query) |
| TipoNecesidadId | int | FK a MaestraTipoNecesidad |
| TipoNecesidadNombre | string | Nombre del tipo (Ingenieria de Audio, Diseno, etc.) |
| PresupuestoMin | decimal? | Presupuesto minimo; null si no se especifico |
| PresupuestoMax | decimal? | Presupuesto maximo; null si no se especifico |
| MonedaId | int? | FK a MaestraMoneda; null si no se especifico presupuesto |
| MonedaNombre | string? | Nombre de la moneda (EUR, USD); null si no aplica |
| ModalidadTrabajoId | int | FK a MaestraModalidadTrabajo |
| ModalidadTrabajoNombre | string | Nombre de la modalidad (Remoto, Presencial, Hibrido) |
| UbicacionCiudad | string? | Ciudad; null si modalidad es Remoto |
| UbicacionPais | string? | Pais; null si modalidad es Remoto |
| ArtistaNombre | string | Nombre artistico del propietario de la necesidad |
| FechaCreacion | DateTime | Fecha de publicacion de la necesidad (UTC) |
| FechaLimitePropuestas | DateTime? | Fecha limite para recibir propuestas; null si no se especifico |
| EsUrgente | bool | Calculado en backend: FechaLimitePropuestas existe y es menor a 3 dias desde DateTime.UtcNow |
| NumeroPropuestas | int | COUNT de propuestas activas (no Retiradas) en esta necesidad |

**Retornado por:** `GetNecesidadesPublicasQuery`

---

### 4.2 NecesidadPublicaDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/NecesidadPublicaDto.cs`

Detalle completo de necesidad con campos calculados para el profesional autenticado.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la necesidad |
| Titulo | string | Titulo completo de la necesidad |
| Descripcion | string? | Descripcion completa (sin truncar) |
| TipoNecesidadId | int | FK a MaestraTipoNecesidad |
| TipoNecesidadNombre | string | Nombre del tipo |
| EstadoNecesidadId | int | FK a MaestraEstadoNecesidad (siempre 1=Abierta en este endpoint) |
| EstadoNecesidadNombre | string | Nombre del estado (siempre "Abierta" en este endpoint) |
| ModalidadTrabajoId | int | FK a MaestraModalidadTrabajo |
| ModalidadTrabajoNombre | string | Nombre de la modalidad |
| PresupuestoMin | decimal? | Presupuesto minimo |
| PresupuestoMax | decimal? | Presupuesto maximo |
| MonedaId | int? | FK a MaestraMoneda |
| MonedaNombre | string? | Nombre de la moneda |
| UbicacionCiudad | string? | Ciudad; null si modalidad es Remoto |
| UbicacionPais | string? | Pais; null si modalidad es Remoto |
| FechaCreacion | DateTime | Fecha de publicacion (UTC) |
| FechaLimitePropuestas | DateTime? | Fecha limite para propuestas |
| FechaInicioPrevista | DateTime? | Fecha prevista de inicio del trabajo |
| NumeroPropuestas | int | COUNT de propuestas activas (no Retiradas) |
| Artista | ArtistaPublicoDto | Datos publicos del artista propietario |
| YaPropuso | bool | **Calculado:** true si UserId autenticado tiene propuesta no-Retirada para esta necesidad |
| EsPropietario | bool | **Calculado:** true si UserId autenticado corresponde al artista propietario |
| TienePerfilProfesional | bool | **Calculado:** true si existe PerfilProfesional para el UserId autenticado |

**Retornado por:** `GetNecesidadPublicaByIdQuery`

---

### 4.3 ArtistaPublicoDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/ArtistaPublicoDto.cs`

Sub-DTO embebido en `NecesidadPublicaDto`. Datos publicos del artista sin informacion sensible.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID del artista |
| NombreArtistico | string | Nombre artistico publico |
| ImagenUrl | string? | URL de imagen de perfil del artista; null si no tiene imagen |

---

### 4.4 PropuestaCreatedResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/PropuestaCreatedResultDto.cs`

Respuesta minima al crear una propuesta exitosamente (POST 201).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la propuesta recien creada |
| NecesidadTitulo | string | Titulo de la necesidad a la que se propuso |
| PrecioPropuesto | decimal | Precio enviado en la propuesta |
| EstadoPropuestaNombre | string | Siempre "Pendiente" al crear |
| FechaCreacion | DateTime | Timestamp UTC de creacion |

**Retornado por:** `CreatePropuestaCommand`

---

### 4.5 MiPropuestaListDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MiPropuestaListDto.cs`

Item del listado paginado de propuestas del profesional.

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la propuesta |
| NecesidadTitulo | string | Titulo de la necesidad a la que se propuso |
| ArtistaNombre | string | Nombre artistico del artista propietario de la necesidad |
| PrecioPropuesto | decimal | Precio ofertado |
| MonedaId | int | FK a MaestraMoneda |
| MonedaNombre | string | Nombre de la moneda (EUR, USD) |
| EstadoPropuestaId | int | FK a MaestraEstadoPropuesta (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada) |
| EstadoPropuestaNombre | string | Nombre del estado |
| FechaCreacion | DateTime | Timestamp UTC de envio de la propuesta |
| FechaActualizacion | DateTime? | Timestamp UTC del ultimo cambio de estado; null si no ha cambiado |
| AcuerdoId | Guid? | ID del acuerdo generado; null si no fue aceptada aun |

**Retornado por:** `GetMisPropuestasQuery`

---

### 4.6 RetirarPropuestaResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/RetirarPropuestaResultDto.cs`

Respuesta minima tras retirar una propuesta (PATCH 200).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la propuesta retirada |
| EstadoPropuestaNombre | string | Siempre "Retirada" tras la operacion exitosa |

**Retornado por:** `RetirarPropuestaCommand`

---

## 5. Validadores FluentValidation

### 5.1 CreatePropuestaCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Validators/CreatePropuestaCommandValidator.cs`

**Dependencias inyectadas:**
- `IPropuestaCrowdsourcingService propuestaService` - Para verificar unicidad via cache
- `INecesidadCrowdsourcingService necesidadService` - Para verificar necesidad abierta y ownership via cache
- `IPerfilProfesionalService perfilService` - Para verificar que el usuario tiene perfil profesional

**Nota sobre caching (ADR-006):** Las llamadas a `necesidadService.GetByIdAsync` y `perfilService.GetPerfilByUserIdAsync` en el validator se resuelven con `IRequestCacheService` dentro de los services. Cuando el Handler llame a los mismos metodos, el cache devolvera los datos sin ir a BD nuevamente.

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| PrecioPropuesto | `.GreaterThan(0)` | "El precio propuesto debe ser mayor a 0" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| MonedaId | `.NotEmpty()` | "La moneda es obligatoria" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| MonedaId | `.GreaterThan(0)` | "La moneda no es valida" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| DiasEstimados | `.GreaterThan(0).When(x => x.DiasEstimados.HasValue)` | "Los dias estimados deben ser mayor a 0" | `ServiceResponseMessageType.Validation_InvalidRange` ("1009") |
| DiasEstimados | `.LessThanOrEqualTo(365).When(x => x.DiasEstimados.HasValue)` | "El tiempo estimado no puede superar los 365 dias" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| MensajePropuesta | `.NotEmpty()` | "El mensaje de propuesta es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| MensajePropuesta | `.MinimumLength(20)` | "El mensaje debe tener al menos 20 caracteres" | `ServiceResponseMessageType.Validation_MinLength` ("1011") |
| MensajePropuesta | `.MaximumLength(2000)` | "El mensaje no puede superar los 2000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` ("1002") |
| NecesidadId (MustAsync) | Necesidad existe, estado Abierta (EstadoNecesidadId == 1) y no expirada | "La necesidad no existe, no esta abierta o ha expirado" | `ServiceResponseMessageType.NotFound_Necesidad` ("2009") |
| command (MustAsync - PerfilProfesional) | `perfilService.ExistePerfilProfesionalAsync(command.UserId, ct)` retorna true | "Debes crear un perfil profesional para enviar propuestas" | `ServiceResponseMessageType.BusinessRule_NoProfessionalProfile` ("4006") |
| command (MustAsync - Ownership) | Artista propietario de la necesidad != artista del UserId del command | "No puedes enviar propuesta a tu propia necesidad" | `ServiceResponseMessageType.BusinessRule_CannotProposeSelf` ("4004") |
| command (MustAsync - Unicidad) | No existe propuesta activa (no Retirada) con mismo NecesidadId y UserId | "Ya tienes una propuesta enviada para esta necesidad" | `ServiceResponseMessageType.BusinessRule_AlreadyProposed` ("4003") |

**Orden de validacion recomendado:** Primero las validaciones de campo simples (precio, moneda, dias, mensaje), luego las tres validaciones asincronas en el orden: necesidad existente, perfil profesional, ownership, unicidad.

```
Pseudo-estructura del validator:
1. RuleFor(PrecioPropuesto): GreaterThan(0)
2. RuleFor(MonedaId): NotEmpty + GreaterThan(0)
3. RuleFor(DiasEstimados): When HasValue -> GreaterThan(0) + LessThanOrEqualTo(365)
4. RuleFor(MensajePropuesta): NotEmpty + MinLength(20) + MaxLength(2000)
5. RuleFor(NecesidadId): MustAsync -> necesidad existe y esta Abierta y no expirada
6. RuleFor(x => x): MustAsync -> usuario tiene PerfilProfesional activo
7. RuleFor(x => x): MustAsync -> usuario NO es el artista propietario
8. RuleFor(x => x): MustAsync -> usuario NO tiene propuesta previa no-Retirada
```

---

### 5.2 RetirarPropuestaCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Propuestas/Validators/RetirarPropuestaCommandValidator.cs`

**Dependencias inyectadas:**
- `IPropuestaCrowdsourcingService propuestaService` - Para verificar existencia, ownership y estado

**Nota:** El validator verifica existencia y estado. La verificacion de ownership (UserId del token == UserId de la propuesta) se realiza en el Handler directamente despues de obtener la entidad del service, siguiendo el patron del proyecto (ver `GetNecesidadByIdQueryHandler`). Esto permite retornar 403 en lugar de 400 para el caso de ownership.

| Campo | Regla FluentValidation | Mensaje | ErrorCode |
|-------|------------------------|---------|-----------|
| Id | `.NotEmpty()` | "El ID de la propuesta es obligatorio" | `ServiceResponseMessageType.Validation_Required` ("1001") |
| command (MustAsync) | Propuesta con ese Id existe en BD | "La propuesta no fue encontrada" | `ServiceResponseMessageType.NotFound_Propuesta` ("2010") |
| command (MustAsync) | Propuesta encontrada tiene EstadoPropuestaId == 1 (Pendiente) | "Solo se pueden retirar propuestas en estado Pendiente" | `ServiceResponseMessageType.BusinessRule_PropuestaNotRetirable` ("4005") |

**Alternativa de implementacion:** Se pueden combinar las dos validaciones asincronas en un solo `MustAsync` que carga la propuesta una vez via cache y verifica tanto existencia como estado, retornando el mensaje apropiado segun cual falle.

---

## 6. AutoMapper Profiles

### 6.1 NecesidadCrowdsourcingProfile (ampliacion)

**Archivo existente:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/NecesidadCrowdsourcingProfile.cs`

Agregar los siguientes mappings al profile existente:

| Source | Destination | Notas de Mapping |
|--------|-------------|------------------|
| `NecesidadCrowdsourcing` | `NecesidadPublicaListDto` | `Id.Value -> Id`; `Ignore` todos los campos calculados (TipoNecesidadNombre, ModalidadTrabajoNombre, MonedaNombre, ArtistaNombre, EsUrgente, NumeroPropuestas) porque se rellenan manualmente en el Handler o via proyeccion SQL |
| `NecesidadCrowdsourcing` | `NecesidadPublicaDto` | `Id.Value -> Id`; `Ignore` TipoNecesidadNombre, EstadoNecesidadNombre, ModalidadTrabajoNombre, MonedaNombre, Artista, YaPropuso, EsPropietario, TienePerfilProfesional (campos calculados) |

**Nota:** Los campos calculados `YaPropuso`, `EsPropietario`, `TienePerfilProfesional` y los nombres de maestras NO se mapean via AutoMapper. El Handler los asigna directamente al DTO despues de llamar al service. Esto se alinea con el patron existente en el proyecto (ver `NecesidadCrowdsourcingProfile` donde `EstadoNecesidadNombre` ya se marca como `Ignore`).

---

### 6.2 PropuestaCrowdsourcingProfile (ampliacion)

**Archivo existente:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/PropuestaCrowdsourcingProfile.cs`

Agregar los siguientes mappings al profile existente (actualmente solo tiene `PropuestaCrowdsourcing -> PropuestaCrowdsourcingDto`):

| Source | Destination | Notas de Mapping |
|--------|-------------|------------------|
| `CreatePropuestaCommand` | `PropuestaCrowdsourcing` | `Ignore` Id, NecesidadId, UserId, PerfilProfesionalId, EstadoPropuestaId, FechaCreacion, FechaActualizacion, Necesidad, Acuerdos (se asignan en el Handler) |
| `PropuestaCrowdsourcing` | `PropuestaCreatedResultDto` | `Id.Value -> Id`; `Ignore` NecesidadTitulo, EstadoPropuestaNombre (se asignan en el Handler desde la entidad Necesidad cargada y el nombre de estado) |
| `PropuestaCrowdsourcing` | `MiPropuestaListDto` | `Id.Value -> Id`; `Ignore` NecesidadTitulo, ArtistaNombre, MonedaNombre, EstadoPropuestaNombre, AcuerdoId (se asignan en el Handler o via proyeccion con Include) |
| `PropuestaCrowdsourcing` | `RetirarPropuestaResultDto` | `Id.Value -> Id`; `Ignore` EstadoPropuestaNombre (se asigna en Handler con valor "Retirada") |

---

## 7. Controllers

### 7.1 NecesidadesCrowdsourcingController (ampliacion)

**Archivo existente:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/NecesidadesCrowdsourcingController.cs`

Agregar los siguientes metodos al controller existente.

#### GET /api/crowdsourcing/necesidades (nuevo metodo publico)

```
[HttpGet]
[ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetPublicas(
    [FromQuery] int? tipoNecesidadId,
    [FromQuery] int? modalidad,
    [FromQuery] decimal? presupuestoMin,
    [FromQuery] decimal? presupuestoMax,
    [FromQuery] string? pais,
    [FromQuery] string? search,
    [FromQuery] string? orderBy,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 12)
```

- Obtiene `userId` del claim `ClaimTypes.NameIdentifier` (string, no Guid)
- Si `userId` es null o vacio, retorna `Unauthorized()`
- Construye `GetNecesidadesPublicasQuery` con todos los parametros
- Si `response.HasErrors`: retorna `StatusCode(500, response)` (este endpoint no tiene errores de negocio, solo 401 y 500)
- Si exitoso: retorna `Ok(response)`

**Nota de conflicto de ruta:** El controller existente ya tiene `[HttpGet("mis-necesidades")]`. El nuevo `[HttpGet]` sin ruta especifica responde a `GET /api/crowdsourcing/necesidades` (raiz), que es diferente. No hay conflicto de rutas.

#### GET /api/crowdsourcing/necesidades/{id} (modificar metodo existente)

El metodo `GetById` existente valida ownership y retorna `NecesidadCrowdsourcingDto`. Para US-CS-03, este metodo debe ampliarse para detectar si el usuario es el artista propietario:

```
Logica de despacho en GetById:
1. Obtener userId (string) del JWT
2. Intentar resolver artistaId del usuario (puede ser null si el usuario no es artista)
3. Cargar necesidad con GetPublicaByIdAsync(id, userId) -> NecesidadPublicaDto
   - Si necesidad == null: return NotFound(response con NotFound_Necesidad)
   - Si necesidad.EsPropietario == true:
       -> Delegar a GetNecesidadByIdQuery (vista artista, logica US-CS-02 existente)
       -> Retorna NecesidadCrowdsourcingDto con propuestas completas
   - Si necesidad.EsPropietario == false:
       -> Retorna NecesidadPublicaDto con campos calculados
```

**Alternativa mas simple:** Mantener los dos endpoints separados y no modificar el existente. Crear un nuevo route distinto para la vista publica, como `GET /api/crowdsourcing/necesidades/{id}/publica`. Sin embargo, esto rompe el contrato definido en contracts.md que especifica la misma ruta. **Decision recomendada:** Mantener la logica de despacho en el controller, usando el campo `EsPropietario` de la query publica para determinar si devolver la vista artista o la vista profesional.

**Atributos del metodo modificado:**
```
[HttpGet("{id}")]
[ProducesResponseType(typeof(ServiceResponse<NecesidadCrowdsourcingDto>), StatusCodes.Status200OK)]      // vista artista
[ProducesResponseType(typeof(ServiceResponse<NecesidadPublicaDto>), StatusCodes.Status200OK)]             // vista profesional
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

#### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas (nuevo metodo)

```
[HttpPost("{necesidadId}/propuestas")]
[ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> CreatePropuesta(
    [FromRoute] Guid necesidadId,
    [FromBody] CreatePropuestaRequestDto request)
```

Logica del metodo:
- Obtener `userId` del claim `ClaimTypes.NameIdentifier`; si null -> `Unauthorized()`
- Construir `CreatePropuestaCommand` con necesidadId del route, userId del token y campos del body
- Enviar via MediatR
- Si `response.HasErrors`:
  - ErrorCode empieza con "20" -> `NotFound(response)`
  - ErrorCode == `Auth_Forbidden` o `BusinessRule_CannotProposeSelf` o `BusinessRule_NoProfessionalProfile` -> `StatusCode(403, response)`
  - Otros -> `BadRequest(response)`
- Si exitoso: `Created($"/api/crowdsourcing/propuestas/{response.Data?.Id}", response)` con HTTP 201

**Nota:** `CreatePropuestaRequestDto` es el Request Body DTO (ver seccion 4 de contracts.md). Es distinto del Command: el controller construye el Command a partir del DTO mas los parametros del route y el claim del token.

---

### 7.2 PropuestasCrowdsourcingController (nuevo)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/PropuestasCrowdsourcingController.cs`

```csharp
[ApiController]
[Route("api/crowdsourcing/propuestas")]
[Authorize]
public class PropuestasCrowdsourcingController : ControllerBase
```

Constructor: inyecta `IMediator _mediator` con `?? throw new ArgumentNullException`.

Helper privado:
```
private string? GetUserId() =>
    User.FindFirstValue(ClaimTypes.NameIdentifier);
```

#### GET /api/crowdsourcing/propuestas/mis-propuestas

```
[HttpGet("mis-propuestas")]
[ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<MiPropuestaListDto>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMisPropuestas(
    [FromQuery] int? estado,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
```

- Obtiene `userId`; si null -> `Unauthorized()`
- Construye `GetMisPropuestasQuery` con userId, estado, page, pageSize
- Si `response.HasErrors`: retorna `StatusCode(500, response)`
- Si exitoso: retorna `Ok(response)`

#### PATCH /api/crowdsourcing/propuestas/{id}/retirar

```
[HttpPatch("{id}/retirar")]
[ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<RetirarPropuestaResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Retirar([FromRoute] Guid id)
```

- Obtiene `userId`; si null -> `Unauthorized()`
- Construye `RetirarPropuestaCommand` con id del route y userId del token
- Si `response.HasErrors`:
  - ErrorCode == `NotFound_Propuesta` ("2010") -> `NotFound(response)`
  - ErrorCode == `Auth_Forbidden` ("3002") -> `StatusCode(403, response)`
  - Otros (BusinessRule_PropuestaNotRetirable) -> `BadRequest(response)`
- Si exitoso: retorna `Ok(response)`

---

## 8. Interfaces de Service a Ampliar / Crear

### 8.1 INecesidadCrowdsourcingService (ampliacion)

**Archivo existente:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/INecesidadCrowdsourcingService.cs`

Agregar los siguientes metodos a la interfaz existente:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetPublicasPaginatedAsync` | `Task<(IReadOnlyList<NecesidadCrowdsourcing> Items, int TotalCount)>` | `string userId, int? tipoNecesidadId, int? modalidadTrabajoId, decimal? presupuestoMin, decimal? presupuestoMax, string? pais, string? search, string? orderBy, int page, int pageSize, CancellationToken ct` | Listado publico paginado: solo Abiertas, no expiradas, excluye necesidades del artista del userId |
| `GetPublicaByIdAsync` | `Task<NecesidadCrowdsourcing?>` | `NecesidadCrowdsourcingId id, CancellationToken ct` | Detalle publico: solo Abiertas, no expiradas. Sin filtro de ownership. |

**Nota:** Los campos calculados `YaPropuso`, `EsPropietario`, `TienePerfilProfesional` NO los calcula el `INecesidadCrowdsourcingService`. Los calcula el Handler de `GetNecesidadPublicaByIdQuery` llamando a metodos de `IPropuestaCrowdsourcingService` y `IPerfilProfesionalService` por separado. Esto mantiene la separacion de responsabilidades.

---

### 8.2 IPropuestaCrowdsourcingService (ampliacion)

**Archivo existente:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IPropuestaCrowdsourcingService.cs`

Actualmente solo tiene `RechazarPropuestasPendientesAsync`. Agregar:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `Task<PropuestaCrowdsourcing?>` | `PropuestaCrowdsourcingId id, CancellationToken ct` | Obtener propuesta por ID. Usado en validator de RetirarPropuesta y handler. |
| `ExistePropuestaActivaAsync` | `Task<bool>` | `NecesidadCrowdsourcingId necesidadId, string userId, CancellationToken ct` | Verifica si existe propuesta no-Retirada del userId para la necesidad. Para validacion de unicidad. |
| `CreateAsync` | `Task<PropuestaCrowdsourcingId>` | `PropuestaCrowdsourcing entity, CancellationToken ct` | Crea la propuesta. El service resuelve PerfilProfesionalId por UserId antes de guardar. |
| `RetirarAsync` | `Task<bool>` | `PropuestaCrowdsourcingId id, CancellationToken ct` | Cambia estado a Retirada (4) y actualiza FechaActualizacion. Retorna false si no existe. |
| `GetByUserIdPaginatedAsync` | `Task<(IReadOnlyList<PropuestaCrowdsourcing> Items, int TotalCount)>` | `string userId, int? estadoPropuestaId, int page, int pageSize, CancellationToken ct` | Listado paginado de propuestas del usuario, con Include de Necesidad y Artista para nombres. |

---

### 8.3 IPerfilProfesionalService (nueva dependencia)

**Este servicio pertenece al modulo UserAccess.** El modulo Crowdsourcing necesita consultar si un usuario tiene PerfilProfesional. Dos opciones de implementacion:

**Opcion A (Recomendada para MVP):** Cross-module query via `IPerfilProfesionalService` que se registra en el contenedor desde UserAccess.Infra y se consume desde Crowdsourcing.Application. Requiere definir la interfaz en `BuildingBlocks` o en `Crowdsourcing.Application.Interfaces`.

**Opcion B:** Duplicar la logica en `IPropuestaCrowdsourcingService.CreateAsync` que consulta la tabla PerfilProfesional directamente desde CrowdsourcingContext (si PerfilProfesional esta en el mismo DbContext).

Para el plan de contratos, se asume Opcion A:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `ExistePerfilProfesionalAsync` | `Task<bool>` | `string userId, CancellationToken ct` | Verifica si el userId tiene PerfilProfesional activo. Usado en CreatePropuestaValidator. |
| `GetArtistaByUserIdAsync` | `Task<Artista?>` | `string userId, CancellationToken ct` | Obtiene el artista vinculado al userId. Usado en CreatePropuestaValidator para verificar ownership. Retorna null si el usuario no es artista. |

---

## 9. Flujo de Datos por Endpoint

### GET /api/crowdsourcing/necesidades

```
Controller.GetPublicas(query params)
  -> userId del JWT claim
  -> GetNecesidadesPublicasQuery { UserId, filtros, paginacion }
  -> GetNecesidadesPublicasQueryHandler.Handle()
     -> INecesidadCrowdsourcingService.GetPublicasPaginatedAsync(userId, filtros..., page, pageSize, ct)
        [Service] -> Repository.GetPublicasPaginatedAsync() con AsNoTracking():
                     - WHERE EstadoNecesidadId = 1
                     - AND (FechaLimitePropuestas IS NULL OR FechaLimitePropuestas > UtcNow)
                     - AND ArtistaId NOT IN (artistas del userId) -- excluir las propias
                     - Aplicar filtros opcionales
                     - SELECT con proyeccion (truncar Descripcion a 150 chars)
                     - Calcular EsUrgente en proyeccion
                     - COUNT propuestas no-Retiradas
                     - Ordenar segun OrderBy
                     - Paginar
     -> _mapper.Map<List<NecesidadPublicaListDto>>(entities)
     -> Asignar nombres de maestras (TipoNecesidadNombre, ModalidadTrabajoNombre, MonedaNombre, ArtistaNombre)
        desde las maestras cargadas en el mismo Include o via IRequestCacheService
     -> ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>
  -> Controller: Ok(response)
```

### GET /api/crowdsourcing/necesidades/{id}

```
Controller.GetById(id)
  -> userId del JWT claim
  -> GetNecesidadPublicaByIdQuery { Id, UserId }
  -> GetNecesidadPublicaByIdQueryHandler.Handle()
     -> INecesidadCrowdsourcingService.GetPublicaByIdAsync(necesidadId, ct)
        [Service] -> Repository: WHERE Id = id AND EstadoNecesidadId = 1 AND no expirada, AsNoTracking
        -> Si null: retornar ServiceResponse con NotFound_Necesidad ("2009")
     -> IPropuestaCrowdsourcingService.ExistePropuestaActivaAsync(necesidadId, userId, ct) -> yaPropuso
     -> Calcular esPropietario: necesidad.ArtistaId corresponde al userId del token
        (via IPerfilProfesionalService.GetArtistaByUserIdAsync o similar)
     -> IPerfilProfesionalService.ExistePerfilProfesionalAsync(userId, ct) -> tienePerfilProfesional
     -> _mapper.Map<NecesidadPublicaDto>(entity)
     -> dto.YaPropuso = yaPropuso
     -> dto.EsPropietario = esPropietario
     -> dto.TienePerfilProfesional = tienePerfilProfesional
     -> Asignar nombres de maestras e info del Artista
     -> ServiceResponse<NecesidadPublicaDto>
  -> Controller (despacho segun EsPropietario):
     -> Si EsPropietario: invocar adicionalmente GetNecesidadByIdQuery para vista artista
     -> Si no EsPropietario: retornar Ok(response) con NecesidadPublicaDto
```

### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas

```
Controller.CreatePropuesta(necesidadId, body)
  -> userId del JWT claim
  -> CreatePropuestaCommand { NecesidadId, UserId, PrecioPropuesto, MonedaId, DiasEstimados, MensajePropuesta }
  -> CreatePropuestaCommandHandler.Handle()
     -> _validator.ValidateAsync(request, ct)
        [Validator] -> Validaciones de campo (precio, moneda, dias, mensaje)
        [Validator] -> MustAsync: necesidad existe y esta Abierta (via INecesidadCrowdsourcingService, cacheada)
        [Validator] -> MustAsync: usuario tiene PerfilProfesional (via IPerfilProfesionalService, cacheada)
        [Validator] -> MustAsync: usuario NO es artista propietario (via IPerfilProfesionalService, cacheada)
        [Validator] -> MustAsync: no existe propuesta previa no-Retirada (via IPropuestaCrowdsourcingService, cacheada)
        -> Si errores: ServiceResponse con lista de errores de validacion
     -> Si valido:
        -> entity = _mapper.Map<PropuestaCrowdsourcing>(request)
        -> entity.Id = PropuestaCrowdsourcingId.CreateNew()
        -> entity.NecesidadId = new NecesidadCrowdsourcingId(request.NecesidadId)
        -> entity.UserId = request.UserId
        -> entity.EstadoPropuestaId = 1 (Pendiente)
        -> entity.FechaCreacion = DateTime.UtcNow
        -> id = await _service.CreateAsync(entity, ct)
           [Service] -> Resuelve PerfilProfesionalId por UserId (del cache)
                     -> Asigna entity.PerfilProfesionalId
                     -> Repository.AddAsync(entity) -> SaveChanges
        -> Cargar NecesidadTitulo para el DTO (desde cache o load separado)
        -> resultDto = new PropuestaCreatedResultDto { Id, NecesidadTitulo, PrecioPropuesto, "Pendiente", FechaCreacion }
        -> ServiceResponse con Created ("0001") + dto
  -> Controller: 201 Created
```

### GET /api/crowdsourcing/propuestas/mis-propuestas

```
Controller.GetMisPropuestas(estado, page, pageSize)
  -> userId del JWT claim
  -> GetMisPropuestasQuery { UserId, EstadoPropuestaId, Page, PageSize }
  -> GetMisPropuestasQueryHandler.Handle()
     -> IPropuestaCrowdsourcingService.GetByUserIdPaginatedAsync(userId, estadoPropuestaId, page, pageSize, ct)
        [Service] -> Repository: WHERE UserId = userId
                     - AND (EstadoPropuestaId = estado si presente)
                     - Include Necesidad (para Titulo)
                     - Include Necesidad.Artista (para NombreArtistico)
                     - Include Acuerdos (para AcuerdoId, si existe)
                     - AsNoTracking
                     - ORDER BY FechaCreacion DESC
                     - Paginar
     -> _mapper.Map<List<MiPropuestaListDto>>(entities)
     -> Asignar NecesidadTitulo, ArtistaNombre, MonedaNombre, EstadoPropuestaNombre, AcuerdoId
     -> ServiceResponse<PaginatedResponse<MiPropuestaListDto>>
  -> Controller: Ok(response)
```

### PATCH /api/crowdsourcing/propuestas/{id}/retirar

```
Controller.Retirar(id)
  -> userId del JWT claim
  -> RetirarPropuestaCommand { Id, UserId }
  -> RetirarPropuestaCommandHandler.Handle()
     -> _validator.ValidateAsync(request, ct)
        [Validator] -> Id.NotEmpty()
        [Validator] -> MustAsync: propuesta existe (via IPropuestaCrowdsourcingService, cacheada)
        [Validator] -> MustAsync: propuesta.EstadoPropuestaId == 1 (Pendiente)
        -> Si errores: ServiceResponse con errores (404 para NotFound, 400 para estado invalido)
     -> Si valido:
        -> propuesta = await _service.GetByIdAsync(propuestaId, ct) [del cache]
        -> Si propuesta.UserId != request.UserId:
           -> _logger.LogWarning("Unauthorized attempt to retirar propuesta...")
           -> return ServiceResponse con Auth_Forbidden ("3002")
        -> await _service.RetirarAsync(propuestaId, ct)
           [Service] -> Repository.Update: EstadoPropuestaId = 4, FechaActualizacion = UtcNow
        -> resultDto = new RetirarPropuestaResultDto { Id = request.Id, EstadoPropuestaNombre = "Retirada" }
        -> ServiceResponse con Updated ("0002") + dto
  -> Controller: Ok(response) o error segun ErrorCode
```

---

## 10. OpenAPI / Swagger Documentation

### GET /api/crowdsourcing/necesidades

- **Summary:** Explorar necesidades abiertas (listado publico paginado)
- **Description:** Retorna necesidades en estado Abierta y no expiradas. Excluye las necesidades del propio artista autenticado. Soporta filtros por tipo, modalidad, presupuesto, pais, texto y ordenamiento.
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `tipoNecesidadId` (query, int, optional): Filtrar por tipo de necesidad
  - `modalidad` (query, int, optional): ModalidadTrabajoId (1=Presencial, 2=Remoto, 3=Hibrido)
  - `presupuestoMin` (query, decimal, optional): Precio minimo del rango
  - `presupuestoMax` (query, decimal, optional): Precio maximo del rango
  - `pais` (query, string, optional): Filtrar por pais de ubicacion
  - `search` (query, string, optional): Busqueda en titulo y descripcion
  - `orderBy` (query, string, optional): Orden - recientes (default), mayor-presupuesto, fecha-limite
  - `page` (query, int, optional, default 1): Numero de pagina
  - `pageSize` (query, int, optional, default 12, max 50): Items por pagina
- **Responses:**
  - `200 OK`: `ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>` con items y paginacion
  - `401 Unauthorized`: Token JWT invalido o expirado
  - `500 Internal Server Error`: Error inesperado
- **Security:** Bearer JWT

### GET /api/crowdsourcing/necesidades/{id}

- **Summary:** Detalle de necesidad con campos calculados para el usuario autenticado
- **Description:** Retorna el detalle completo de una necesidad Abierta y no expirada. Incluye yaPropuso, esPropietario y tienePerfilProfesional calculados segun el usuario autenticado. Si el usuario es el artista propietario, retorna la vista del artista con propuestas recibidas (NecesidadCrowdsourcingDto).
- **Tags:** Crowdsourcing - Necesidades
- **Parameters:**
  - `id` (path, Guid, required): ID de la necesidad
- **Responses:**
  - `200 OK (profesional)`: `ServiceResponse<NecesidadPublicaDto>` con campos calculados
  - `200 OK (artista propietario)`: `ServiceResponse<NecesidadCrowdsourcingDto>` con propuestas recibidas
  - `401 Unauthorized`: Token JWT invalido o expirado
  - `404 Not Found`: ErrorCode 2009 - Necesidad no existe, no esta Abierta o ha expirado
  - `500 Internal Server Error`: Error inesperado
- **Security:** Bearer JWT

### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas

- **Summary:** Enviar propuesta a una necesidad abierta
- **Description:** Crea una propuesta para la necesidad indicada. El UserId se obtiene del JWT. Requiere que el usuario tenga PerfilProfesional activo. Maximo una propuesta activa por necesidad por usuario. El artista propietario no puede enviar propuesta a su propia necesidad.
- **Tags:** Crowdsourcing - Propuestas
- **Parameters:**
  - `necesidadId` (path, Guid, required): ID de la necesidad a la que se propone
- **Request Body (application/json):** `CreatePropuestaRequestDto`
  - `precioPropuesto` (decimal, required): Precio ofertado. Debe ser > 0
  - `monedaId` (int, required): ID de maestra de moneda
  - `diasEstimados` (int, optional): Dias estimados de entrega. Si se envia, debe ser > 0 y <= 365
  - `mensajePropuesta` (string, required): Mensaje de la propuesta. Min 20, max 2000 caracteres
- **Responses:**
  - `201 Created`: `ServiceResponse<PropuestaCreatedResultDto>` con datos de la propuesta creada
  - `400 Bad Request`: Errores de validacion (precio, moneda, mensaje, propuesta duplicada)
  - `401 Unauthorized`: Token invalido
  - `403 Forbidden`: ErrorCode 4006 (sin PerfilProfesional) o 4004 (es el artista propietario)
  - `404 Not Found`: ErrorCode 2009 - Necesidad no existe, no Abierta o expirada
  - `500 Internal Server Error`: Error inesperado
- **Security:** Bearer JWT

### GET /api/crowdsourcing/propuestas/mis-propuestas

- **Summary:** Listar propuestas enviadas por el usuario autenticado
- **Description:** Retorna las propuestas enviadas por el profesional autenticado. Filtrable por estado. Incluye acuerdoId para propuestas aceptadas.
- **Tags:** Crowdsourcing - Propuestas
- **Parameters:**
  - `estado` (query, int, optional): EstadoPropuestaId (1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Retirada)
  - `page` (query, int, optional, default 1): Numero de pagina
  - `pageSize` (query, int, optional, default 10, max 50): Items por pagina
- **Responses:**
  - `200 OK`: `ServiceResponse<PaginatedResponse<MiPropuestaListDto>>` con items y paginacion
  - `401 Unauthorized`: Token invalido
  - `500 Internal Server Error`: Error inesperado
- **Security:** Bearer JWT

### PATCH /api/crowdsourcing/propuestas/{id}/retirar

- **Summary:** Retirar una propuesta propia en estado Pendiente
- **Description:** Cambia el estado de la propuesta a Retirada. Solo aplicable a propuestas propias en estado Pendiente. Accion irreversible. Sin request body.
- **Tags:** Crowdsourcing - Propuestas
- **Parameters:**
  - `id` (path, Guid, required): ID de la propuesta a retirar
- **Request Body:** Ninguno
- **Responses:**
  - `200 OK`: `ServiceResponse<RetirarPropuestaResultDto>` con estado final "Retirada"
  - `400 Bad Request`: ErrorCode 4005 - Propuesta no esta en estado Pendiente
  - `401 Unauthorized`: Token invalido
  - `403 Forbidden`: ErrorCode 3002 - El usuario no es el propietario de la propuesta
  - `404 Not Found`: ErrorCode 2010 - Propuesta no encontrada
  - `500 Internal Server Error`: Error inesperado
- **Security:** Bearer JWT

---

## 11. Mapa de ErrorCodes HTTP

| HTTP Status | ErrorCode | Constante | Descripcion | Endpoint(s) afectados |
|-------------|-----------|-----------|-------------|----------------------|
| 200 | 0000 | `Success` | Operacion exitosa | GET listado, GET mis-propuestas |
| 201 | 0001 | `Created` | Propuesta creada | POST propuesta |
| 200 | 0002 | `Updated` | Propuesta retirada | PATCH retirar |
| 400 | 1001 | `Validation_Required` | Campo requerido faltante | POST propuesta |
| 400 | 1002 | `Validation_MaxLength` | Excede longitud maxima | POST propuesta |
| 400 | 1009 | `Validation_InvalidRange` | Valor fuera de rango | POST propuesta |
| 400 | 1011 | `Validation_MinLength` | Por debajo de longitud minima | POST propuesta |
| 404 | 2009 | `NotFound_Necesidad` | Necesidad no encontrada, no Abierta o expirada | GET detalle, POST propuesta |
| 404 | 2010 | `NotFound_Propuesta` | Propuesta no encontrada | PATCH retirar |
| 401 | 3001 | `Auth_Unauthorized` | Token invalido/expirado | Todos |
| 403 | 3002 | `Auth_Forbidden` | No es propietario de la propuesta | PATCH retirar |
| 400 | 4003 | `BusinessRule_AlreadyProposed` | Ya tiene propuesta activa para esa necesidad | POST propuesta |
| 403 | 4004 | `BusinessRule_CannotProposeSelf` | Artista propietario intenta proponerse | POST propuesta |
| 400 | 4005 | `BusinessRule_PropuestaNotRetirable` | Propuesta no esta en estado Pendiente | PATCH retirar |
| 403 | 4006 | `BusinessRule_NoProfessionalProfile` | Usuario sin PerfilProfesional | POST propuesta |
| 500 | 5000 | `Internal_UnexpectedError` | Excepcion no controlada | Todos |

---

## 12. Archivos a Crear

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs   MODIFICAR - agregar 5 constantes nuevas
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── NecesidadPublicaListDto.cs      CREAR
│   │   ├── NecesidadPublicaDto.cs          CREAR
│   │   ├── ArtistaPublicoDto.cs            CREAR
│   │   ├── PropuestaCreatedResultDto.cs    CREAR
│   │   ├── MiPropuestaListDto.cs           CREAR
│   │   └── RetirarPropuestaResultDto.cs    CREAR
│   │
│   ├── Features/Propuestas/
│   │   ├── Commands/
│   │   │   ├── CreatePropuestaCommand.cs   CREAR (Command + Handler en mismo archivo)
│   │   │   └── RetirarPropuestaCommand.cs  CREAR (Command + Handler en mismo archivo)
│   │   ├── Queries/
│   │   │   ├── GetNecesidadesPublicasQuery.cs      CREAR (Query + Handler en mismo archivo)
│   │   │   ├── GetNecesidadPublicaByIdQuery.cs     CREAR (Query + Handler en mismo archivo)
│   │   │   └── GetMisPropuestasQuery.cs            CREAR (Query + Handler en mismo archivo)
│   │   └── Validators/
│   │       ├── CreatePropuestaCommandValidator.cs  CREAR
│   │       └── RetirarPropuestaCommandValidator.cs CREAR
│   │
│   ├── Interfaces/Services/
│   │   ├── INecesidadCrowdsourcingService.cs  MODIFICAR - agregar 2 metodos nuevos
│   │   └── IPropuestaCrowdsourcingService.cs  MODIFICAR - agregar 5 metodos nuevos
│   │
│   └── Mapping/
│       ├── NecesidadCrowdsourcingProfile.cs    MODIFICAR - agregar 2 mappings nuevos
│       └── PropuestaCrowdsourcingProfile.cs    MODIFICAR - agregar 4 mappings nuevos
│
├── WePlayRises.Crowdsourcing.WebApi/
│   └── Controllers/
│       ├── NecesidadesCrowdsourcingController.cs  MODIFICAR - agregar GetPublicas, CreatePropuesta y ajustar GetById
│       └── PropuestasCrowdsourcingController.cs    CREAR
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Repositories/
    │   └── PropuestaCrowdsourcingRepository.cs  MODIFICAR - implementar nuevos metodos del repositorio
    └── Services/
        ├── NecesidadCrowdsourcingService.cs     MODIFICAR - implementar GetPublicasPaginatedAsync y GetPublicaByIdAsync
        └── PropuestaCrowdsourcingService.cs     MODIFICAR - implementar los 5 nuevos metodos
```

---

## 13. Checklist de Contratos

### Commands / Queries
- [ ] Commands implementan `IRequest<ServiceResponse<T>>`
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command/Query en el MISMO archivo (Regla 2)
- [ ] Handlers NO inyectan DbContext - usan Services (Regla 3)
- [ ] Constructores con `?? throw new ArgumentNullException` para todas las dependencias (Regla 6)
- [ ] Todos los Handlers tienen `ILogger<T>` inyectado y usado en el bloque catch (Regla 6)
- [ ] Validacion retorna `ServiceResponse` con mensajes, NO throw exception (Regla 7)
- [ ] Services retornan Entidades, NO DTOs (Regla 4)
- [ ] Handler hace el mapping Entity -> DTO con AutoMapper

### Validators
- [ ] `CreatePropuestaCommandValidator` con `.WithMessage()` Y `.WithErrorCode()` en cada regla (Regla 5)
- [ ] `RetirarPropuestaCommandValidator` con `.WithMessage()` Y `.WithErrorCode()` en cada regla (Regla 5)
- [ ] ErrorCodes usan constantes de `ServiceResponseMessageType`, NO strings literales (Regla 5)
- [ ] Las 5 nuevas constantes estan definidas en `ServiceResponseMessageType.cs`
- [ ] Validaciones asincronas usan `MustAsync`

### AutoMapper
- [ ] `NecesidadCrowdsourcingProfile` ampliado con mappings a `NecesidadPublicaListDto` y `NecesidadPublicaDto`
- [ ] `PropuestaCrowdsourcingProfile` ampliado con 4 mappings nuevos
- [ ] Campos calculados y nombres de maestras marcados como `Ignore()` en mappings (se asignan en Handler)
- [ ] Strongly-typed IDs mapeados correctamente: `Id.Value -> Id`

### DTOs
- [ ] 6 DTOs de response nuevos creados
- [ ] `NecesidadPublicaDto` incluye propiedades `YaPropuso`, `EsPropietario`, `TienePerfilProfesional`
- [ ] `MiPropuestaListDto` incluye `AcuerdoId` nullable para US-CS-04
- [ ] No se usa `any` ni tipos sin definir

### Controllers
- [ ] `NecesidadesCrowdsourcingController` tiene metodo `GetPublicas` para el listado publico
- [ ] `NecesidadesCrowdsourcingController` tiene metodo `CreatePropuesta` con ruta `{necesidadId}/propuestas`
- [ ] `PropuestasCrowdsourcingController` nuevo con `[Authorize]` a nivel de clase
- [ ] Controller extrae `UserId` (string) del JWT claim, NO ArtistaId (Guid) para estos endpoints
- [ ] Logica de despacho HTTP correcta: 201 para Create, 403 para Forbidden, 404 para NotFound
- [ ] `[ProducesResponseType]` documentados en cada action

### Servicios e Interfaces
- [ ] `INecesidadCrowdsourcingService` ampliado con `GetPublicasPaginatedAsync` y `GetPublicaByIdAsync`
- [ ] `IPropuestaCrowdsourcingService` ampliado con 5 metodos nuevos
- [ ] Interfaz `IPerfilProfesionalService` disponible con `ExistePerfilProfesionalAsync` y `GetArtistaByUserIdAsync`
- [ ] Caching via `IRequestCacheService` planificado para evitar queries duplicados en Validator y Handler

### Seguridad
- [ ] Ownership de propuesta verificado en Handler de `RetirarPropuestaCommand` (no solo en Validator)
- [ ] UserId siempre del JWT, nunca del request body
- [ ] Validaciones de negocio criticas en backend (no solo frontend)
