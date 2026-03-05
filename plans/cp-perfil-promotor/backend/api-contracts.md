# Contratos API: Perfil de Promotor

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-perfil-promotor (US-CP-01)

---

## Notas Previas Criticas (Leer Antes de Implementar)

### A. Campos Faltantes en la Entidad Promotor

La entidad `Promotor` actual (`Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/Promotor.cs`) **NO tiene** los campos `EmailContacto` ni `UrlSitioWeb`. Ambos son requeridos por los contratos. Antes de implementar cualquier Command o Query es obligatorio:

1. Agregar en la entidad `Promotor`:
   - `public string? EmailContacto { get; set; }` (nullable, max 200)
   - `public string? UrlSitioWeb { get; set; }` (nullable, max 300)
2. Agregar la configuracion Fluent API en `CrowdpromotionContext.OnModelCreating()`:
   - `entity.Property(e => e.EmailContacto).HasMaxLength(200);`
   - `entity.Property(e => e.UrlSitioWeb).HasMaxLength(300);`
3. Generar y aplicar una nueva migracion EF Core. Ambas columnas son nullable: no hay riesgo de datos existentes.

### B. Discrepancia en MaxLength de NombrePublico

La configuracion EF Core actual define `NombrePublico` con `HasMaxLength(100)`, pero el contrato establece `MaxLength(200)`. La migracion citada en el punto A debe incluir tambien el ajuste de esta columna a `nvarchar(200)`.

### C. Codigos de ServiceResponseMessageType en Crowdpromotion

El modulo Crowdpromotion no tiene aun su propia clase `ServiceResponseMessageType`. Debe crearse en:
`Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Los codigos difieren del modulo UserAccess (que usa 1003 para MinLength, 1005 para InvalidEmail, 1006 para InvalidUrl). El modulo Crowdpromotion define sus propios codigos segun el contrato:

| Constante | Codigo Crowdpromotion | Codigo UserAccess (referencia) |
|-----------|----------------------|-------------------------------|
| `Validation_Required` | `1001` | `1001` (igual) |
| `Validation_MaxLength` | `1002` | `1002` (igual) |
| `Validation_InvalidEmail` | `1003` | `1005` (diferente) |
| `Validation_ForeignKeyNotFound` | `1010` | `1010` (igual) |
| `Validation_MinLength` | `1011` | `1003` (diferente) |
| `Validation_InvalidUrl` | `1013` | `1006` (diferente) |
| `NotFound_Promotor` | `2015` | N/A (nuevo) |
| `BusinessRule_PromotorAlreadyExists` | `4018` | N/A (nuevo) |
| `BusinessRule_PromotorAlreadyInactive` | `4019` | N/A (nuevo) |

### D. Strongly Typed ID en Promotor

La entidad usa `PromotorId` (strongly typed ID), no `Guid`. En los DTOs de respuesta el `Id` se expone como `Guid` mediante `.Value`. El mapping AutoMapper debe usar `.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))`.

### E. PromoProgramaPromotor no tiene campo de estado de programa activo

`TotalProgramasActivos` se calcula como COUNT de `PromoProgramaPromotor` donde `PromotorId == id` Y `EsActivo == true`. El campo `EsActivo` existe en `PromoProgramaPromotor`. La query la ejecuta el Service, no el Handler.

---

## 1. Resumen de Endpoints

| Metodo | Ruta | Tipo | Autorizacion | DTO Request | DTO Response |
|--------|------|------|--------------|-------------|--------------|
| POST | /api/crowdpromotion/promotor | Command | Bearer JWT (cualquier rol) | `CreatePromotorCommand` | `ServiceResponse<PromotorCreatedResultDto>` |
| GET | /api/crowdpromotion/promotor/me | Query | Bearer JWT | `GetPromotorMeQuery` | `ServiceResponse<PromotorDto>` |
| PUT | /api/crowdpromotion/promotor/me | Command | Bearer JWT | `UpdatePromotorCommand` | `ServiceResponse<PromotorUpdatedResultDto>` |
| PATCH | /api/crowdpromotion/promotor/me/desactivar | Command | Bearer JWT | `DesactivarPromotorCommand` | `ServiceResponse<PromotorDesactivadoResultDto>` |

---

## 2. Request DTOs (Commands y Queries)

### 2.1 CreatePromotorCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/CreatePromotorCommand.cs`

**Descripcion:** Crea el perfil de promotor del usuario autenticado. El `UserId` se obtiene del token JWT en el controller y se inyecta en el Command; no proviene del body. Crea tambien la `PromotorWallet` en EUR en la misma transaccion.

**Propiedades:**

| Propiedad | Tipo | Requerido | Validacion | Fuente |
|-----------|------|-----------|------------|--------|
| NombrePublico | string | Si | NotEmpty, MinLength(3), MaxLength(200) | Body |
| TipoPromotorId | int | Si | NotEmpty (> 0), MustAsync (existe en maestra) | Body |
| EmailContacto | string? | No | EmailAddress cuando presente, MaxLength(200) | Body |
| UrlSitioWeb | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlInstagram | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlTikTok | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlYouTube | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlTwitter | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UserId | string? | Si (interno) | NotEmpty | Token JWT (controller) |

**Implementa:** `IRequest<ServiceResponse<PromotorCreatedResultDto>>`

**Estructura esperada:**
```csharp
public class CreatePromotorCommand : IRequest<ServiceResponse<PromotorCreatedResultDto>>
{
    // From body
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }

    // Populated from JWT token by controller - NOT from request body
    public string? UserId { get; set; }
}
```

**Handler en mismo archivo:** `CreatePromotorCommandHandler`

Flujo del handler:
1. Validar con `CreatePromotorCommandValidator` — retornar `ServiceResponse` con errores si falla
2. Verificar `UserId` presente (autorizacion basica)
3. Llamar `IPromotorService.GetByUserIdAsync(userId, ct)` para verificar unicidad — retornar `BusinessRule_PromotorAlreadyExists` (4018) si existe
4. Llamar `IFanProfileService.GetByUserIdAsync(userId, ct)` para resolver `FanProfileId` — asignar `null` si no existe (flujo FA-02, no es error)
5. Validar que `TipoPromotorId` exista via `IPromotorService.TipoPromotorExistsAsync(tipoPromotorId, ct)` (esto tambien lo hace el Validator de forma asincrona, el Handler puede confiar en que ya paso la validacion)
6. Mapear Command a entidad `Promotor` con AutoMapper
7. Llamar `IPromotorService.CreateWithWalletAsync(promotor, ct)` — crea Promotor + PromotorWallet en transaccion
8. Llamar `IPromotorService.GetByUserIdAsync(userId, ct)` para obtener la entidad creada con navegacion `TipoPromotor`
9. Mapear a `PromotorCreatedResultDto`
10. Retornar `ServiceResponse` con codigo `Created` ("0001")

---

### 2.2 GetPromotorMeQuery

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Queries/GetPromotorMeQuery.cs`

**Descripcion:** Obtiene el perfil completo del promotor autenticado, incluyendo campos calculados (`TotalProgramasActivos`, `TotalComisionesGanadas`). El `UserId` se obtiene del token JWT.

**Propiedades:**

| Propiedad | Tipo | Requerido | Fuente |
|-----------|------|-----------|--------|
| UserId | string | Si | Token JWT (controller) |

**Implementa:** `IRequest<ServiceResponse<PromotorDto>>`

**Estructura esperada:**
```csharp
public class GetPromotorMeQuery : IRequest<ServiceResponse<PromotorDto>>
{
    // Populated from JWT token by controller
    public string UserId { get; set; } = null!;
}
```

**Handler en mismo archivo:** `GetPromotorMeQueryHandler`

Flujo del handler:
1. Llamar `IPromotorService.GetByUserIdAsync(userId, ct)` — retornar `NotFound_Promotor` (2015) si `null`
2. Mapear entidad a `PromotorDto` con AutoMapper
3. Completar campos calculados que no se mapean automaticamente:
   - `TotalProgramasActivos`: via `IPromotorService.GetProgramasActivosCountAsync(promotorId, ct)`
   - `TotalComisionesGanadas` y `MonedaComisiones`: via `IPromotorService.GetWalletEurAsync(promotorId, ct)`
4. Retornar `ServiceResponse` con `Data = promotorDto`

**Nota sobre campos calculados:** El Service es responsable de las queries de calculo. El Handler recibe los valores numericos y los asigna al DTO. No usar `AsNoTracking` directamente en el Handler (eso es responsabilidad del Repository).

---

### 2.3 UpdatePromotorCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/UpdatePromotorCommand.cs`

**Descripcion:** Actualiza los campos editables del perfil del promotor autenticado. `TipoPromotorId` no es editable. El `UserId` se obtiene del token JWT.

**Propiedades:**

| Propiedad | Tipo | Requerido | Validacion | Fuente |
|-----------|------|-----------|------------|--------|
| NombrePublico | string | Si | NotEmpty, MinLength(3), MaxLength(200) | Body |
| EmailContacto | string? | No | EmailAddress cuando presente, MaxLength(200) | Body |
| UrlSitioWeb | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlInstagram | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlTikTok | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlYouTube | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UrlTwitter | string? | No | Must(BeValidUrl) cuando presente, MaxLength(300) | Body |
| UserId | string? | Si (interno) | NotEmpty | Token JWT (controller) |

**Implementa:** `IRequest<ServiceResponse<PromotorUpdatedResultDto>>`

**Estructura esperada:**
```csharp
public class UpdatePromotorCommand : IRequest<ServiceResponse<PromotorUpdatedResultDto>>
{
    // From body
    public string NombrePublico { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }

    // Populated from JWT token by controller - NOT from request body
    public string? UserId { get; set; }
}
```

**Handler en mismo archivo:** `UpdatePromotorCommandHandler`

Flujo del handler:
1. Validar con `UpdatePromotorCommandValidator` — retornar `ServiceResponse` con errores si falla
2. Llamar `IPromotorService.GetByUserIdAsync(userId, ct)` — retornar `NotFound_Promotor` (2015) si `null`
3. Actualizar campos en la entidad recuperada (no usar AutoMapper para update parcial — asignar propiedades directamente)
4. Establecer `FechaActualizacion = DateTime.UtcNow` en la entidad
5. Llamar `IPromotorService.UpdateAsync(promotor, ct)`
6. Retornar `ServiceResponse` con `PromotorUpdatedResultDto` y codigo `Updated` ("0002")

---

### 2.4 DesactivarPromotorCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Commands/DesactivarPromotorCommand.cs`

**Descripcion:** Desactiva logicamente el perfil del promotor (`EsActivo = false`) y da de baja automatica todos los `PromoProgramaPromotor` activos. El `UserId` se obtiene del token JWT. No tiene request body.

**Propiedades:**

| Propiedad | Tipo | Requerido | Fuente |
|-----------|------|-----------|--------|
| UserId | string | Si (interno) | Token JWT (controller) |

**Implementa:** `IRequest<ServiceResponse<PromotorDesactivadoResultDto>>`

**Estructura esperada:**
```csharp
public class DesactivarPromotorCommand : IRequest<ServiceResponse<PromotorDesactivadoResultDto>>
{
    // Populated from JWT token by controller
    public string? UserId { get; set; }
}
```

**Handler en mismo archivo:** `DesactivarPromotorCommandHandler`

Flujo del handler:
1. Llamar `IPromotorService.GetByUserIdAsync(userId, ct)` — retornar `NotFound_Promotor` (2015) si `null`
2. Verificar `promotor.EsActivo == true` — retornar `BusinessRule_PromotorAlreadyInactive` (4019) si ya esta desactivado
3. Llamar `IPromotorService.DesactivarWithProgramasAsync(promotorId, ct)` — retorna el numero de programas desactivados
4. Retornar `ServiceResponse` con `PromotorDesactivadoResultDto` y codigo `Updated` ("0002")

**Nota sobre la transaccion:** `DesactivarWithProgramasAsync` en el Service ejecuta en una unica transaccion la desactivacion del Promotor y la desactivacion de todos sus `PromoProgramaPromotor` activos. El Service retorna el `int programasDadosDeBaja` al Handler.

---

## 3. Response DTOs

### 3.1 PromotorCreatedResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorCreatedResultDto.cs`

**Descripcion:** Respuesta minima al crear el perfil. No incluye todos los campos del perfil completo; solo los necesarios para confirmar la creacion y redirigir al dashboard.

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico del Promotor (desde `PromotorId.Value`) |
| NombrePublico | string | Nombre publico del promotor |
| TipoPromotorNombre | string | Nombre del tipo de promotor (resuelto desde navegacion `MaestraTipoPromotor`) |
| EsActivo | bool | Siempre `true` en la creacion |
| FechaCreacion | DateTime | Timestamp UTC de creacion |

**Estructura esperada:**
```csharp
public class PromotorCreatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

**Wrapped en:** `ServiceResponse<PromotorCreatedResultDto>`

**Ejemplo Response 201:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombrePublico": "DJ Marketing Pro",
    "tipoPromotorNombre": "Influencer",
    "esActivo": true,
    "fechaCreacion": "2026-02-25T10:00:00Z"
  },
  "messages": [
    { "message": "Perfil de promotor creado", "errorCode": "0001" }
  ]
}
```

---

### 3.2 PromotorDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDto.cs`

**Descripcion:** Perfil completo del promotor, incluyendo campos calculados de estadisticas. Se usa en `GET /api/crowdpromotion/promotor/me`.

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico (desde `PromotorId.Value`) |
| NombrePublico | string | Nombre publico del promotor |
| TipoPromotorId | int | ID del tipo de promotor |
| TipoPromotorNombre | string | Nombre del tipo (resuelto desde navegacion) |
| EmailContacto | string? | Email de contacto publico |
| UrlSitioWeb | string? | URL del sitio web |
| UrlInstagram | string? | URL de Instagram |
| UrlTikTok | string? | URL de TikTok |
| UrlYouTube | string? | URL de YouTube |
| UrlTwitter | string? | URL de Twitter/X |
| EsActivo | bool | Estado del perfil |
| FechaCreacion | DateTime | Timestamp UTC de creacion |
| TotalProgramasActivos | int | COUNT de PromoProgramaPromotor activos. Calculado en Service. |
| TotalComisionesGanadas | decimal | TotalGanado de PromotorWallet con MonedaId == 1 (EUR). Calculado en Service. |
| MonedaComisiones | string | Codigo ISO de moneda. Siempre "EUR" en MVP. |

**Estructura esperada:**
```csharp
public class PromotorDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    public string TipoPromotorNombre { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaCreacion { get; set; }
    // Campos calculados - asignados manualmente en el Handler, no por AutoMapper
    public int TotalProgramasActivos { get; set; }
    public decimal TotalComisionesGanadas { get; set; }
    public string MonedaComisiones { get; set; } = "EUR";
}
```

**Wrapped en:** `ServiceResponse<PromotorDto>`

**Nota sobre campos calculados:** Los campos `TotalProgramasActivos`, `TotalComisionesGanadas` y `MonedaComisiones` NO se mapean desde la entidad `Promotor` (no existen en ella). El Handler los asigna directamente al DTO tras las llamadas al Service. AutoMapper mapea el resto de campos desde `Promotor` a `PromotorDto`.

---

### 3.3 PromotorUpdatedResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorUpdatedResultDto.cs`

**Descripcion:** Confirmacion minima de la actualizacion del perfil. Solo expone los campos modificados y la fecha de actualizacion.

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico (desde `PromotorId.Value`) |
| NombrePublico | string | Nombre publico actualizado |
| FechaActualizacion | DateTime | Timestamp UTC de la actualizacion |

**Estructura esperada:**
```csharp
public class PromotorUpdatedResultDto
{
    public Guid Id { get; set; }
    public string NombrePublico { get; set; } = null!;
    public DateTime FechaActualizacion { get; set; }
}
```

**Wrapped en:** `ServiceResponse<PromotorUpdatedResultDto>`

**Ejemplo Response 200:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nombrePublico": "DJ Marketing Pro (Updated)",
    "fechaActualizacion": "2026-02-25T09:00:00Z"
  },
  "messages": [
    { "message": "Perfil actualizado", "errorCode": "0002" }
  ]
}
```

---

### 3.4 PromotorDesactivadoResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorDesactivadoResultDto.cs`

**Descripcion:** Resultado de la desactivacion. Incluye el numero de programas dados de baja como efecto cascada.

**Propiedades:**

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador unico (desde `PromotorId.Value`) |
| EsActivo | bool | Siempre `false` tras la desactivacion |
| ProgramasDadosDeBaja | int | Numero de PromoProgramaPromotor desactivados. Puede ser 0. |

**Estructura esperada:**
```csharp
public class PromotorDesactivadoResultDto
{
    public Guid Id { get; set; }
    public bool EsActivo { get; set; }
    public int ProgramasDadosDeBaja { get; set; }
}
```

**Wrapped en:** `ServiceResponse<PromotorDesactivadoResultDto>`

**Ejemplo Response 200:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "esActivo": false,
    "programasDadosDeBaja": 2
  },
  "messages": [
    { "message": "Perfil desactivado", "errorCode": "0002" }
  ]
}
```

---

## 4. Constantes de ServiceResponseMessageType

**Archivo a Crear:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Descripcion:** Clase de constantes de codigos de error especifica del modulo Crowdpromotion. Cada modulo define la suya propia para mantener independencia. Los codigos continuan la secuencia del modulo Crowdsourcing (que llega hasta 2014 y 4017).

```csharp
namespace WePlayRises.Crowdpromotion.Domain.Constants;

public static class ServiceResponseMessageType
{
    // Success (0000-0999)
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // Validation (1000-1999)
    public const string Validation_Required           = "1001";
    public const string Validation_MaxLength          = "1002";
    public const string Validation_InvalidEmail       = "1003";
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_MinLength          = "1011";
    public const string Validation_InvalidUrl         = "1013";

    // NotFound (2000-2999)
    public const string NotFound_Entity   = "2000";
    public const string NotFound_Promotor = "2015";

    // Auth (3000-3999)
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden    = "3002";
    public const string Auth_InvalidToken = "3004";

    // Business Rules (4000-4999)
    public const string BusinessRule_PromotorAlreadyExists   = "4018";
    public const string BusinessRule_PromotorAlreadyInactive = "4019";

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
}
```

**Tabla de codigos nuevos para esta feature:**

| Constante | Codigo | Descripcion |
|-----------|--------|-------------|
| `NotFound_Promotor` | `2015` | No existe `Promotor` con el `UserId` del token |
| `BusinessRule_PromotorAlreadyExists` | `4018` | Ya existe un `Promotor` con el mismo `UserId` |
| `BusinessRule_PromotorAlreadyInactive` | `4019` | El promotor ya tiene `EsActivo = false` |
| `Validation_InvalidEmail` | `1003` | Email con formato invalido (diferente al UserAccess que usa 1005) |
| `Validation_MinLength` | `1011` | Longitud minima no alcanzada (diferente al UserAccess que usa 1003) |
| `Validation_InvalidUrl` | `1013` | URL con formato invalido (diferente al UserAccess que usa 1006) |

---

## 5. Validadores (FluentValidation)

### 5.1 CreatePromotorCommandValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Validators/CreatePromotorCommandValidator.cs`

**Dependencias inyectadas:** `IPromotorService` (para validacion asincrona de `TipoPromotorId`)

**Nota sobre inyeccion en Validator:** Segun la regla CQRS del proyecto, el Validator puede inyectar el Service (no el DbContext ni el RequestCacheService directamente). El Service encapsula el acceso al cache.

**Reglas de validacion:**

| Campo | Regla | Mensaje | Constante ErrorCode |
|-------|-------|---------|---------------------|
| NombrePublico | NotEmpty | El nombre publico es obligatorio | `Validation_Required` (1001) |
| NombrePublico | MinimumLength(3) | El nombre debe tener al menos 3 caracteres | `Validation_MinLength` (1011) |
| NombrePublico | MaximumLength(200) | El nombre publico no puede superar los 200 caracteres | `Validation_MaxLength` (1002) |
| TipoPromotorId | GreaterThan(0) | El tipo de promotor es obligatorio | `Validation_Required` (1001) |
| TipoPromotorId | MustAsync(ExistInMaestra) | El tipo de promotor no existe | `Validation_ForeignKeyNotFound` (1010) |
| EmailContacto | EmailAddress (cuando presente) | El email de contacto no tiene formato valido | `Validation_InvalidEmail` (1003) |
| EmailContacto | MaximumLength(200) (cuando presente) | El email de contacto no puede superar los 200 caracteres | `Validation_MaxLength` (1002) |
| UrlSitioWeb | Must(BeValidUrl) (cuando presente) | La URL del sitio web no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlSitioWeb | MaximumLength(300) (cuando presente) | La URL del sitio web no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlInstagram | Must(BeValidUrl) (cuando presente) | La URL de Instagram no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlInstagram | MaximumLength(300) (cuando presente) | La URL de Instagram no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlTikTok | Must(BeValidUrl) (cuando presente) | La URL de TikTok no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlTikTok | MaximumLength(300) (cuando presente) | La URL de TikTok no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlYouTube | Must(BeValidUrl) (cuando presente) | La URL de YouTube no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlYouTube | MaximumLength(300) (cuando presente) | La URL de YouTube no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlTwitter | Must(BeValidUrl) (cuando presente) | La URL de Twitter/X no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlTwitter | MaximumLength(300) (cuando presente) | La URL de Twitter/X no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UserId | NotEmpty | El UserId es obligatorio | `Validation_Required` (1001) |

**Pseudocodigo del validator:**
```csharp
public class CreatePromotorCommandValidator : AbstractValidator<CreatePromotorCommand>
{
    private readonly IPromotorService _promotorService;

    public CreatePromotorCommandValidator(IPromotorService promotorService)
    {
        _promotorService = promotorService
            ?? throw new ArgumentNullException(nameof(promotorService));

        RuleFor(x => x.NombrePublico)
            .NotEmpty()
            .WithMessage("El nombre publico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(3)
            .WithMessage("El nombre debe tener al menos 3 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El nombre publico no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.TipoPromotorId)
            .GreaterThan(0)
            .WithMessage("El tipo de promotor es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TipoPromotorId)
            .MustAsync(async (id, ct) => await _promotorService.TipoPromotorExistsAsync(id, ct))
            .WithMessage("El tipo de promotor no existe")
            .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound)
            .When(x => x.TipoPromotorId > 0);

        RuleFor(x => x.EmailContacto)
            .EmailAddress()
            .WithMessage("El email de contacto no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail)
            .MaximumLength(200)
            .WithMessage("El email de contacto no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.EmailContacto));

        // Repetir patron para UrlSitioWeb, UrlInstagram, UrlTikTok, UrlYouTube, UrlTwitter:
        // .Must(BeValidUrl).WithMessage(...).WithErrorCode(Validation_InvalidUrl)
        // .MaximumLength(300).WithMessage(...).WithErrorCode(Validation_MaxLength)
        // .When(x => !string.IsNullOrEmpty(x.UrlXxx))

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
```

**Caching:** El metodo `IPromotorService.TipoPromotorExistsAsync` usa `IRequestCacheService` internamente (en el Service, no en el Validator). Si el Handler tambien consulta la maestra, obtiene el resultado del cache sin ir a BD de nuevo.

---

### 5.2 UpdatePromotorCommandValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Promotor/Validators/UpdatePromotorCommandValidator.cs`

**Dependencias inyectadas:** Ninguna (sin validaciones asincronas; `TipoPromotorId` no se edita en update).

**Reglas de validacion:**

| Campo | Regla | Mensaje | Constante ErrorCode |
|-------|-------|---------|---------------------|
| NombrePublico | NotEmpty | El nombre publico es obligatorio | `Validation_Required` (1001) |
| NombrePublico | MinimumLength(3) | El nombre debe tener al menos 3 caracteres | `Validation_MinLength` (1011) |
| NombrePublico | MaximumLength(200) | El nombre publico no puede superar los 200 caracteres | `Validation_MaxLength` (1002) |
| EmailContacto | EmailAddress (cuando presente) | El email de contacto no tiene formato valido | `Validation_InvalidEmail` (1003) |
| EmailContacto | MaximumLength(200) (cuando presente) | El email de contacto no puede superar los 200 caracteres | `Validation_MaxLength` (1002) |
| UrlSitioWeb | Must(BeValidUrl) (cuando presente) | La URL del sitio web no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlSitioWeb | MaximumLength(300) (cuando presente) | La URL del sitio web no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlInstagram | Must(BeValidUrl) (cuando presente) | La URL de Instagram no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlInstagram | MaximumLength(300) (cuando presente) | La URL de Instagram no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlTikTok | Must(BeValidUrl) (cuando presente) | La URL de TikTok no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlTikTok | MaximumLength(300) (cuando presente) | La URL de TikTok no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlYouTube | Must(BeValidUrl) (cuando presente) | La URL de YouTube no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlYouTube | MaximumLength(300) (cuando presente) | La URL de YouTube no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UrlTwitter | Must(BeValidUrl) (cuando presente) | La URL de Twitter/X no tiene formato valido | `Validation_InvalidUrl` (1013) |
| UrlTwitter | MaximumLength(300) (cuando presente) | La URL de Twitter/X no puede superar los 300 caracteres | `Validation_MaxLength` (1002) |
| UserId | NotEmpty | El UserId es obligatorio | `Validation_Required` (1001) |

**Nota:** `BeValidUrl` es el mismo metodo privado estaico que en `CreatePromotorCommandValidator`. Se puede extraer a una clase de helpers estatica compartida dentro del modulo, como `UrlValidationHelper`, pero eso es decision de implementacion.

---

## 6. AutoMapper Profiles

### 6.1 PromotorProfile

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromotorProfile.cs`

**Mappings requeridos:**

| Source | Destination | Tipo | Notas |
|--------|-------------|------|-------|
| `CreatePromotorCommand` | `Promotor` | Command → Entity | Ignorar Id, FechaCreacion, FechaActualizacion, Wallets, Programas |
| `Promotor` | `PromotorCreatedResultDto` | Entity → DTO | Id desde `Id.Value`; TipoPromotorNombre desde navegacion |
| `Promotor` | `PromotorDto` | Entity → DTO | Id desde `Id.Value`; TipoPromotorNombre desde navegacion; campos calculados ignorados |
| `Promotor` | `PromotorUpdatedResultDto` | Entity → DTO | Id desde `Id.Value`; solo campos Id, NombrePublico, FechaActualizacion |
| `Promotor` | `PromotorDesactivadoResultDto` | Entity → DTO | Id desde `Id.Value`; ProgramasDadosDeBaja se asigna manualmente en Handler |

**Pseudocodigo del profile:**
```csharp
public class PromotorProfile : Profile
{
    public PromotorProfile()
    {
        // Command -> Entity (para Create)
        CreateMap<CreatePromotorCommand, Promotor>()
            .ForMember(dest => dest.Id,
                       opt => opt.Ignore()) // Generado en Service
            .ForMember(dest => dest.UserId,
                       opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FechaCreacion,
                       opt => opt.Ignore()) // Asignado en Service
            .ForMember(dest => dest.FechaActualizacion,
                       opt => opt.Ignore())
            .ForMember(dest => dest.EsActivo,
                       opt => opt.Ignore()) // Asignado en Service como true
            .ForMember(dest => dest.FanProfileId,
                       opt => opt.Ignore()) // Resuelto en Handler via IFanProfileService
            .ForMember(dest => dest.SeguidoresTotales,
                       opt => opt.Ignore()) // No se carga en creacion
            .ForMember(dest => dest.Programas,
                       opt => opt.Ignore())
            .ForMember(dest => dest.Wallets,
                       opt => opt.Ignore());

        // Entity -> DTO (respuesta POST - creacion)
        CreateMap<Promotor, PromotorCreatedResultDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromotorNombre,
                       opt => opt.MapFrom(src => src.TipoPromotor != null
                           ? src.TipoPromotor.Nombre
                           : string.Empty)); // Requiere navegacion cargada

        // Entity -> DTO (respuesta GET - perfil completo)
        CreateMap<Promotor, PromotorDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.TipoPromotorNombre,
                       opt => opt.MapFrom(src => src.TipoPromotor != null
                           ? src.TipoPromotor.Nombre
                           : string.Empty))
            // Campos calculados - NO se mapean desde entidad; el Handler los asigna
            .ForMember(dest => dest.TotalProgramasActivos,
                       opt => opt.Ignore())
            .ForMember(dest => dest.TotalComisionesGanadas,
                       opt => opt.Ignore())
            .ForMember(dest => dest.MonedaComisiones,
                       opt => opt.Ignore());

        // Entity -> DTO (respuesta PUT - actualizacion)
        CreateMap<Promotor, PromotorUpdatedResultDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value));

        // Entity -> DTO (respuesta PATCH - desactivacion)
        CreateMap<Promotor, PromotorDesactivadoResultDto>()
            .ForMember(dest => dest.Id,
                       opt => opt.MapFrom(src => src.Id.Value))
            // ProgramasDadosDeBaja se asigna manualmente en Handler
            .ForMember(dest => dest.ProgramasDadosDeBaja,
                       opt => opt.Ignore());
    }
}
```

**Nota sobre TipoPromotor:** El mapping de `TipoPromotorNombre` requiere que la entidad `Promotor` tenga una propiedad de navegacion `public virtual MaestraTipoPromotor? TipoPromotor { get; set; }`. Verificar que esta navegacion existe en el modelo de dominio y que el Repository la incluye con `.Include()` al consultar. Si no existe la navegacion, el Service puede resolver el nombre por separado y asignarlo al DTO manualmente.

---

## 7. Interfaces de Service

### 7.1 IPromotorService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorService.cs`

**Metodos requeridos para esta feature:**

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByUserIdAsync(string userId, CancellationToken ct)` | `Task<Promotor?>` | Busca promotor por UserId. Usa cache. Retorna null si no existe. |
| `TipoPromotorExistsAsync(int tipoPromotorId, CancellationToken ct)` | `Task<bool>` | Verifica si el TipoPromotorId existe en la maestra. Usa cache. |
| `CreateWithWalletAsync(Promotor promotor, CancellationToken ct)` | `Task<PromotorId>` | Crea Promotor + PromotorWallet EUR en transaccion. Retorna el PromotorId generado. |
| `UpdateAsync(Promotor promotor, CancellationToken ct)` | `Task` | Persiste cambios en el Promotor. |
| `DesactivarWithProgramasAsync(PromotorId promotorId, CancellationToken ct)` | `Task<int>` | Desactiva Promotor y todos sus PromoProgramaPromotor activos en transaccion. Retorna count de programas desactivados. |
| `GetProgramasActivosCountAsync(PromotorId promotorId, CancellationToken ct)` | `Task<int>` | COUNT de PromoProgramaPromotor activos del promotor. |
| `GetWalletEurAsync(PromotorId promotorId, CancellationToken ct)` | `Task<PromotorWallet?>` | Obtiene la PromotorWallet con MonedaId == 1. Retorna null si no existe aun. |

---

## 8. Controller

### 8.1 PromotorController

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromotorController.cs`

**Ruta base:** `[Route("api/crowdpromotion/[controller]")]`

**Nota:** El segmento `promotor` de la ruta proviene del nombre del controller (`PromotorController` → `promotor`). La ruta completa sera `/api/crowdpromotion/promotor`.

**Dependencias inyectadas:**
- `IMediator _mediator`
- `ICurrentUserService _currentUser`
- `ILogger<PromotorController> _logger`

**Patron de extraccion de UserId (igual que ArtistasController):**
```csharp
var userId = _currentUser.UserId;
if (userId == null)
{
    return Unauthorized(new ServiceResponse<T> { Messages = [...Auth_InvalidToken...] });
}
command.UserId = userId.Value.ToString();
```

#### Action: POST / (Crear Perfil)

```csharp
/// <summary>
/// Creates the promoter profile for the authenticated user.
/// One user can only have one promoter profile. UserId is extracted from JWT token.
/// Also creates a PromotorWallet in EUR automatically.
/// </summary>
/// <param name="command">Promoter profile data</param>
/// <returns>Created promoter profile (minimal)</returns>
/// <response code="201">Profile created successfully</response>
/// <response code="400">Validation errors or duplicate promoter profile</response>
/// <response code="401">Invalid or expired JWT token</response>
/// <response code="500">Internal server error</response>
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorCreatedResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Create([FromBody] CreatePromotorCommand command)
```

**Nota HTTP 201 vs 400 para duplicado:** La regla de negocio `PromotorAlreadyExists` (4018) se retorna como 400, no como 409. Esto es consistente con como `BusinessRule_ArtistaAlreadyExists` se maneja en el modulo UserAccess (retorna 400 a traves de `FromServiceResponse`). Si se desea diferenciar con 409, se puede ajustar el comportamiento de `FromServiceResponse` en el Building Block.

#### Action: GET /me (Obtener Perfil Propio)

```csharp
/// <summary>
/// Returns the full promoter profile of the authenticated user, including stats.
/// </summary>
/// <returns>Full promoter profile with totalProgramasActivos and totalComisionesGanadas</returns>
/// <response code="200">Profile found</response>
/// <response code="401">Invalid or expired JWT token</response>
/// <response code="404">No promoter profile exists for this user</response>
/// <response code="500">Internal server error</response>
[HttpGet("me")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMyProfile()
```

#### Action: PUT /me (Actualizar Perfil)

```csharp
/// <summary>
/// Updates the editable fields of the authenticated user's promoter profile.
/// TipoPromotorId is NOT editable after creation.
/// </summary>
/// <param name="command">Fields to update</param>
/// <returns>Confirmation with updated name and timestamp</returns>
/// <response code="200">Profile updated successfully</response>
/// <response code="400">Validation errors</response>
/// <response code="401">Invalid or expired JWT token</response>
/// <response code="404">No promoter profile exists for this user</response>
/// <response code="500">Internal server error</response>
[HttpPut("me")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorUpdatedResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePromotorCommand command)
```

#### Action: PATCH /me/desactivar (Desactivar Perfil)

```csharp
/// <summary>
/// Logically deactivates the authenticated user's promoter profile.
/// Sets EsActivo = false and removes the promoter from all active programs.
/// The record is NOT deleted from the database.
/// </summary>
/// <returns>Deactivation result with count of programs removed</returns>
/// <response code="200">Profile deactivated successfully</response>
/// <response code="400">Profile is already inactive</response>
/// <response code="401">Invalid or expired JWT token</response>
/// <response code="404">No promoter profile exists for this user</response>
/// <response code="500">Internal server error</response>
[HttpPatch("me/desactivar")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorDesactivadoResultDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> Desactivar()
```

**Nota:** Este action no recibe `[FromBody]`. Crea el `DesactivarPromotorCommand` directamente en el controller e inyecta el `UserId` del token.

---

## 9. Tabla de Errores por Endpoint

### 9.1 POST /api/crowdpromotion/promotor

| HTTP | Constante | Codigo | Mensaje | Causa |
|------|-----------|--------|---------|-------|
| 400 | `Validation_Required` | 1001 | El nombre publico es obligatorio | `NombrePublico` vacio |
| 400 | `Validation_MinLength` | 1011 | El nombre debe tener al menos 3 caracteres | `NombrePublico` < 3 chars |
| 400 | `Validation_MaxLength` | 1002 | El nombre publico no puede superar los 200 caracteres | `NombrePublico` > 200 chars |
| 400 | `Validation_Required` | 1001 | El tipo de promotor es obligatorio | `TipoPromotorId` == 0 |
| 400 | `Validation_ForeignKeyNotFound` | 1010 | El tipo de promotor no existe | `TipoPromotorId` no existe en maestra |
| 400 | `Validation_InvalidEmail` | 1003 | El email de contacto no tiene formato valido | `EmailContacto` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | El email de contacto no puede superar los 200 caracteres | `EmailContacto` > 200 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL del sitio web no tiene formato valido | `UrlSitioWeb` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL del sitio web no puede superar los 300 caracteres | `UrlSitioWeb` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de Instagram no tiene formato valido | `UrlInstagram` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de Instagram no puede superar los 300 caracteres | `UrlInstagram` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de TikTok no tiene formato valido | `UrlTikTok` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de TikTok no puede superar los 300 caracteres | `UrlTikTok` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de YouTube no tiene formato valido | `UrlYouTube` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de YouTube no puede superar los 300 caracteres | `UrlYouTube` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de Twitter/X no tiene formato valido | `UrlTwitter` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de Twitter/X no puede superar los 300 caracteres | `UrlTwitter` > 300 chars |
| 400 | `BusinessRule_PromotorAlreadyExists` | 4018 | Ya tienes un perfil de promotor creado | Ya existe `Promotor` con mismo `UserId` |
| 401 | `Auth_InvalidToken` | 3004 | Token no valido o expirado | Token JWT invalido o expirado |
| 500 | `Internal_UnexpectedError` | 5000 | Error inesperado al crear el perfil de promotor | Excepcion no controlada |

### 9.2 GET /api/crowdpromotion/promotor/me

| HTTP | Constante | Codigo | Mensaje | Causa |
|------|-----------|--------|---------|-------|
| 401 | `Auth_InvalidToken` | 3004 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | `NotFound_Promotor` | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | `Internal_UnexpectedError` | 5000 | Error inesperado al obtener el perfil de promotor | Excepcion no controlada |

### 9.3 PUT /api/crowdpromotion/promotor/me

| HTTP | Constante | Codigo | Mensaje | Causa |
|------|-----------|--------|---------|-------|
| 400 | `Validation_Required` | 1001 | El nombre publico es obligatorio | `NombrePublico` vacio |
| 400 | `Validation_MinLength` | 1011 | El nombre debe tener al menos 3 caracteres | `NombrePublico` < 3 chars |
| 400 | `Validation_MaxLength` | 1002 | El nombre publico no puede superar los 200 caracteres | `NombrePublico` > 200 chars |
| 400 | `Validation_InvalidEmail` | 1003 | El email de contacto no tiene formato valido | `EmailContacto` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | El email de contacto no puede superar los 200 caracteres | `EmailContacto` > 200 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL del sitio web no tiene formato valido | `UrlSitioWeb` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL del sitio web no puede superar los 300 caracteres | `UrlSitioWeb` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de Instagram no tiene formato valido | `UrlInstagram` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de Instagram no puede superar los 300 caracteres | `UrlInstagram` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de TikTok no tiene formato valido | `UrlTikTok` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de TikTok no puede superar los 300 caracteres | `UrlTikTok` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de YouTube no tiene formato valido | `UrlYouTube` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de YouTube no puede superar los 300 caracteres | `UrlYouTube` > 300 chars |
| 400 | `Validation_InvalidUrl` | 1013 | La URL de Twitter/X no tiene formato valido | `UrlTwitter` formato invalido |
| 400 | `Validation_MaxLength` | 1002 | La URL de Twitter/X no puede superar los 300 caracteres | `UrlTwitter` > 300 chars |
| 401 | `Auth_InvalidToken` | 3004 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | `NotFound_Promotor` | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | `Internal_UnexpectedError` | 5000 | Error inesperado al actualizar el perfil de promotor | Excepcion no controlada |

### 9.4 PATCH /api/crowdpromotion/promotor/me/desactivar

| HTTP | Constante | Codigo | Mensaje | Causa |
|------|-----------|--------|---------|-------|
| 400 | `BusinessRule_PromotorAlreadyInactive` | 4019 | El perfil de promotor ya esta desactivado | `EsActivo == false` en la solicitud |
| 401 | `Auth_InvalidToken` | 3004 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | `NotFound_Promotor` | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | `Internal_UnexpectedError` | 5000 | Error inesperado al desactivar el perfil de promotor | Excepcion no controlada |

---

## 10. OpenAPI/Swagger Documentation

### 10.1 POST /api/crowdpromotion/promotor

- **Summary:** Creates the promoter profile for the authenticated user
- **Description:** One user can only have one promoter profile (enforced by unique index on UserId). UserId is extracted exclusively from the JWT token. Also creates a PromotorWallet in EUR automatically in the same transaction.
- **Auth:** Bearer JWT required (`[Authorize]`)
- **Request Body:** `CreatePromotorCommand` (sin `UserId`)
- **Responses:**
  - `201`: `ServiceResponse<PromotorCreatedResultDto>` - Perfil creado
  - `400`: `ServiceResponse<PromotorCreatedResultDto>` - Errores de validacion o promotor duplicado (4018)
  - `401`: `ServiceResponse<PromotorCreatedResultDto>` - Token invalido
  - `500`: `ServiceResponse<PromotorCreatedResultDto>` - Error inesperado

### 10.2 GET /api/crowdpromotion/promotor/me

- **Summary:** Returns the full promoter profile of the authenticated user
- **Description:** Includes calculated stats: totalProgramasActivos (COUNT of active PromoProgramaPromotor) and totalComisionesGanadas (TotalGanado from PromotorWallet with MonedaId=1). Returns 0 if no programs or wallet has no earnings.
- **Auth:** Bearer JWT required (`[Authorize]`)
- **Request Body:** None
- **Responses:**
  - `200`: `ServiceResponse<PromotorDto>` - Perfil completo con estadisticas
  - `401`: `ServiceResponse<PromotorDto>` - Token invalido
  - `404`: `ServiceResponse<PromotorDto>` - No existe perfil de promotor (2015)
  - `500`: `ServiceResponse<PromotorDto>` - Error inesperado

### 10.3 PUT /api/crowdpromotion/promotor/me

- **Summary:** Updates the editable fields of the authenticated user's promoter profile
- **Description:** Only NombrePublico, EmailContacto and all URL fields are editable. TipoPromotorId cannot be changed after creation. Sets FechaActualizacion = DateTime.UtcNow on success.
- **Auth:** Bearer JWT required (`[Authorize]`)
- **Request Body:** `UpdatePromotorCommand` (sin `UserId`, sin `TipoPromotorId`)
- **Responses:**
  - `200`: `ServiceResponse<PromotorUpdatedResultDto>` - Actualizacion confirmada
  - `400`: `ServiceResponse<PromotorUpdatedResultDto>` - Errores de validacion
  - `401`: `ServiceResponse<PromotorUpdatedResultDto>` - Token invalido
  - `404`: `ServiceResponse<PromotorUpdatedResultDto>` - No existe perfil de promotor (2015)
  - `500`: `ServiceResponse<PromotorUpdatedResultDto>` - Error inesperado

### 10.4 PATCH /api/crowdpromotion/promotor/me/desactivar

- **Summary:** Logically deactivates the authenticated user's promoter profile
- **Description:** Sets EsActivo = false. Also deactivates all active PromoProgramaPromotor entries for this promoter (cascade soft-delete). The Promotor record is NOT deleted from the database. Returns count of programs deactivated (can be 0).
- **Auth:** Bearer JWT required (`[Authorize]`)
- **Request Body:** None
- **Responses:**
  - `200`: `ServiceResponse<PromotorDesactivadoResultDto>` - Desactivacion confirmada
  - `400`: `ServiceResponse<PromotorDesactivadoResultDto>` - Perfil ya desactivado (4019)
  - `401`: `ServiceResponse<PromotorDesactivadoResultDto>` - Token invalido
  - `404`: `ServiceResponse<PromotorDesactivadoResultDto>` - No existe perfil de promotor (2015)
  - `500`: `ServiceResponse<PromotorDesactivadoResultDto>` - Error inesperado

---

## 11. Estructura de Archivos a Crear

```
Modules/Crowdpromotion/
│
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs          [NUEVO]
│   └── Model/
│       └── Promotor.cs                            [MODIFICAR - agregar EmailContacto, UrlSitioWeb]
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Features/
│   │   └── Promotor/
│   │       ├── Commands/
│   │       │   ├── CreatePromotorCommand.cs        [NUEVO] (Command + Handler)
│   │       │   ├── UpdatePromotorCommand.cs        [NUEVO] (Command + Handler)
│   │       │   └── DesactivarPromotorCommand.cs    [NUEVO] (Command + Handler)
│   │       ├── Queries/
│   │       │   └── GetPromotorMeQuery.cs           [NUEVO] (Query + Handler)
│   │       └── Validators/
│   │           ├── CreatePromotorCommandValidator.cs  [NUEVO]
│   │           └── UpdatePromotorCommandValidator.cs  [NUEVO]
│   ├── Dtos/
│   │   ├── PromotorCreatedResultDto.cs             [NUEVO]
│   │   ├── PromotorDto.cs                          [NUEVO]
│   │   ├── PromotorUpdatedResultDto.cs             [NUEVO]
│   │   └── PromotorDesactivadoResultDto.cs         [NUEVO]
│   ├── Interfaces/
│   │   └── Services/
│   │       └── IPromotorService.cs                 [NUEVO]
│   └── Mapping/
│       └── PromotorProfile.cs                      [NUEVO]
│
├── WePlayRises.Crowdpromotion.Infra/
│   ├── Context/
│   │   └── CrowdpromotionContext.cs               [MODIFICAR - agregar EmailContacto, UrlSitioWeb; corregir MaxLength NombrePublico a 200]
│   ├── Repositories/
│   │   └── PromotorRepository.cs                  [NUEVO]
│   ├── Services/
│   │   └── PromotorService.cs                     [NUEVO]
│   ├── Migrations/
│   │   └── {timestamp}_AddEmailContactoAndUrlSitioWebToPromotor.cs  [NUEVO - migracion EF Core]
│   └── DependencyInjection.cs                     [MODIFICAR - registrar servicios]
│
└── WePlayRises.Crowdpromotion.WebApi/
    └── Controllers/
        └── PromotorController.cs                  [NUEVO]
```

---

## 12. Checklist de Contratos API

### Previo a la implementacion

- [ ] Agregar `EmailContacto` y `UrlSitioWeb` a entidad `Promotor`
- [ ] Actualizar configuracion EF Core en `CrowdpromotionContext` para ambos campos nuevos
- [ ] Corregir `MaxLength` de `NombrePublico` de 100 a 200 en `CrowdpromotionContext`
- [ ] Crear y aplicar migracion EF Core
- [ ] Verificar que `Promotor` tiene propiedad de navegacion `MaestraTipoPromotor` (para resolver `TipoPromotorNombre`)
- [ ] Crear `ServiceResponseMessageType.cs` en `Crowdpromotion.Domain/Constants/`

### Commands y Queries

- [ ] `CreatePromotorCommand` implementa `IRequest<ServiceResponse<PromotorCreatedResultDto>>`
- [ ] `GetPromotorMeQuery` implementa `IRequest<ServiceResponse<PromotorDto>>`
- [ ] `UpdatePromotorCommand` implementa `IRequest<ServiceResponse<PromotorUpdatedResultDto>>`
- [ ] `DesactivarPromotorCommand` implementa `IRequest<ServiceResponse<PromotorDesactivadoResultDto>>`
- [ ] Handler en el **mismo archivo** que su Command/Query (REGLA CQRS 1)
- [ ] Todos los Handlers retornan `ServiceResponse<T>` (REGLA CQRS 2)

### Handlers

- [ ] Handlers inyectan `IPromotorService`, nunca `CrowdpromotionContext` (REGLA CQRS 3)
- [ ] Handlers inyectan `IMapper` para los mappings Entity → DTO
- [ ] Handlers inyectan `IValidator<T>` para validacion
- [ ] Handlers inyectan `ILogger<T>` y lo usan en catch (REGLA CQRS 9)
- [ ] Constructores con `?? throw new ArgumentNullException` para todas las dependencias (REGLA CQRS 6)
- [ ] Validacion retorna `ServiceResponse` con mensajes, NO throw (REGLA CQRS 7)
- [ ] Respuestas exitosas usan `ServiceResponseMessageType.Created` o `ServiceResponseMessageType.Updated`
- [ ] Respuestas de error usan constantes de `ServiceResponseMessageType` (no strings literales)

### Validators

- [ ] `CreatePromotorCommandValidator` valida todos los campos del contrato
- [ ] `UpdatePromotorCommandValidator` valida todos los campos del contrato
- [ ] Todos los `.WithMessage()` tienen tambien `.WithErrorCode()` con constante (REGLA CQRS 5)
- [ ] ErrorCodes usan `ServiceResponseMessageType.X` del modulo **Crowdpromotion** (no UserAccess)
- [ ] Validaciones opcionales usan `.When(x => !string.IsNullOrEmpty(x.Campo))`
- [ ] `TipoPromotorId` se valida de forma asincrona con `MustAsync` usando `IPromotorService`

### AutoMapper

- [ ] `PromotorProfile` registrado en `DependencyInjection`
- [ ] `Id` mapeado desde `PromotorId.Value` (strongly typed ID → Guid)
- [ ] Campos calculados (`TotalProgramasActivos`, `TotalComisionesGanadas`, `MonedaComisiones`, `ProgramasDadosDeBaja`) con `.Ignore()` en el profile (el Handler los asigna manualmente)
- [ ] `TipoPromotorNombre` mapeado desde navegacion `TipoPromotor.Nombre`

### Controller

- [ ] `PromotorController` hereda de `BaseLoggerController`
- [ ] Ruta base `[Route("api/crowdpromotion/[controller]")]`
- [ ] Todos los actions con `[Authorize]`
- [ ] `UserId` extraido de `ICurrentUserService`, nunca del body
- [ ] Swagger comments con `<summary>`, `<param>`, `<returns>`, `<response>` en cada action
- [ ] `[ProducesResponseType]` para todos los status codes documentados
- [ ] Action `Desactivar` sin `[FromBody]` (no tiene body)

### DependencyInjection

- [ ] `IPromotorService` → `PromotorService` registrado como Scoped
- [ ] `IPromotorRepository` → `PromotorRepository` registrado como Scoped
- [ ] `PromotorProfile` de AutoMapper incluido en el scan de perfiles del modulo
- [ ] `CreatePromotorCommandValidator` registrado (via `AddValidatorsFromAssembly` si se usa)

---

## 13. Referencias

- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`
- **Contracts Spec:** `docs/user-stories/cp-perfil-promotor/contracts.md`
- **Feature Spec:** `docs/user-stories/cp-perfil-promotor/feature-spec.md`
- **Entidad Promotor:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/Promotor.cs`
- **Entidad PromotorWallet:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWallet.cs`
- **Entidad PromoProgramaPromotor:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoProgramaPromotor.cs`
- **Contexto EF Core:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`
- **Patron Command existente:** `Modules/UserAccess/WePlayRises.UserAccess.Application/Features/Artistas/Commands/CreateArtistaCommand.cs`
- **Patron Controller existente:** `Modules/UserAccess/WePlayRises.UserAccess.WebApi/Controllers/ArtistasController.cs`
- **ServiceResponseMessageType referencia:** `Modules/UserAccess/WePlayRises.UserAccess.Domain/Constants/ServiceResponseMessageType.cs`
