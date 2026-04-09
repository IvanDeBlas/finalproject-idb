# Contratos API: cp-tracking-metricas

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-tracking-metricas (US-CP-05)
**Basado en:** `docs/user-stories/cp-tracking-metricas/contracts.md`
**Depende de:** US-CP-03 (cp-inscripcion-programa)

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Auth | Actor | Descripcion |
|--------|------|------|------|-------|-------------|
| POST | `/api/crowdpromotion/tracking/evento` | Command | Publica (sin token) | Fan / Visitante anonimo | Registrar evento de tracking (Click, PageView, Signup, Share) |
| POST | `/api/crowdpromotion/tracking/conversion` | Command | Interno (sistema) | Modulo Crowdfunding | Registrar conversion (Backing) y acreditar comision en wallet |
| GET | `/api/crowdpromotion/programas/{programaId}/metricas` | Query | Bearer JWT | Artista propietario | Dashboard de metricas del programa con KPIs, ranking y serie temporal |
| GET | `/api/crowdpromotion/promotor/metricas` | Query | Bearer JWT | Promotor autenticado | Dashboard de metricas individuales del promotor |

---

## 2. Request DTOs (Commands y Queries)

### 2.1 RegistrarEventoCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Commands/RegistrarEventoCommand.cs`

**Descripcion:** Command para registrar un evento publico de tracking. Generado por el frontend al detectar parametros UTM o codigo referido en la URL. No requiere autenticacion. El `UserIdAfectado` se resuelve en el Handler desde el JWT si existe; nunca viene en el body.

**Implementa:** `IRequest<ServiceResponse<RegistrarEventoResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `CodigoReferido` | `string?` | No | Codigo referido del promotor. Max 50 caracteres. Null si el fan llego sin parametro `ref`. |
| `TipoEventoPromoId` | `int` | Si | 1=Click, 2=PageView, 3=Signup, 5=Share. El valor 4 (Backing) es invalido en este endpoint. |
| `CampaniaCrowdfundingId` | `Guid?` | No | FK a CampaniaCrowdfunding. Presente en eventos PageView. |
| `UrlOrigen` | `string?` | No | URL completa desde donde llego el click. Max 2048 caracteres. |
| `UrlReferer` | `string?` | No | HTTP Referer header del request del fan. Max 2048 caracteres. |
| `UtmSource` | `string?` | No | UTM source. Siempre "weplay" cuando lo envia el frontend. Max 100 caracteres. |
| `UtmMedium` | `string?` | No | UTM medium. Siempre "referral". Max 100 caracteres. |
| `UtmCampaign` | `string?` | No | UTM campaign. Coincide con CodigoTrackingBase del programa. Max 100 caracteres. |
| `IpOrigen` | `string?` | No | Inyectado por el Controller desde `HttpContext.Connection.RemoteIpAddress`. No viene en el body. Usado para rate limiting. |
| `UserIdAfectado` | `string?` | No | Inyectado por el Controller desde el token JWT opcional. No viene en el body. Null si el request es anonimo. |

**Notas de implementacion:**
- `IpOrigen` y `UserIdAfectado` se asignan en el Controller, nunca vienen del body del cliente.
- El Controller usara `[AllowAnonymous]` para que el endpoint sea publico. Si el usuario tiene JWT valido, se extrae `UserId` y se asigna al command.

---

### 2.2 RegistrarConversionCommand

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Commands/RegistrarConversionCommand.cs`

**Descripcion:** Command para registrar una conversion (backing referido) y calcular/acreditar la comision del promotor. Llamado internamente por el handler de backing del modulo Crowdfunding. En MVP se invoca como llamada directa al Service (no HTTP entre modulos). Operacion atomica: PromoEvento + PromotorWalletTransaccion + actualizacion de PromotorWallet en una sola transaccion.

**Implementa:** `IRequest<ServiceResponse<RegistrarConversionResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `CodigoReferido` | `string` | Si | Codigo referido del promotor. Max 50 caracteres. Obligatorio (backing siempre tiene referido en sesion). |
| `CampaniaCrowdfundingId` | `Guid` | Si | FK a CampaniaCrowdfunding de la aportacion. |
| `AportacionCrowdfundingId` | `Guid` | Si | FK a AportacionCrowdfunding para trazabilidad completa de la conversion. |
| `ValorMonetario` | `decimal` | Si | Importe del backing. Debe ser mayor que 0. |
| `MonedaId` | `int` | Si | FK a MaestraMoneda. Debe ser mayor que 0. |
| `UserIdAfectado` | `string` | Si | UserId del fan que realizó el backing. |

---

### 2.3 GetProgramaMetricasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Queries/GetProgramaMetricasQuery.cs`

**Descripcion:** Query para obtener el dashboard de metricas del programa. El Handler verifica que el ArtistaId del token sea propietario del programa. Los datos se calculan con GROUP BY sobre PromoEvento. Sin cache de query en MVP (lectura directa de BD).

**Implementa:** `IRequest<ServiceResponse<ProgramaMetricasResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `ProgramaId` | `Guid` | Si | ID del programa de promocion (path param). |
| `UserId` | `string` | Si | Inyectado por el Controller desde el claim `sub` del JWT. |
| `FechaDesde` | `DateOnly?` | No | Fecha inicio del periodo. Sin valor: 90 dias atras o inicio del programa (el menor). |
| `FechaHasta` | `DateOnly?` | No | Fecha fin del periodo. Sin valor: hoy (UTC). |

**Validacion de negocio en Handler (no en Validator):**
- Resolver `ArtistaId` buscando `Artista` por `UserId`. Si no existe: retornar 404 con `NotFound_Artista` (2016).
- Verificar que `PromoPrograma.ArtistaId == artista.Id`. Si no coincide: retornar 403 con `BusinessRule_NoEsPropietarioPrograma` (4026).
- Si `FechaDesde > FechaHasta`: retornar 400 con `Validation_FechaRangoInvalido` (1036).

---

### 2.4 GetPromotorMetricasQuery

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Queries/GetPromotorMetricasQuery.cs`

**Descripcion:** Query para obtener el dashboard de metricas individuales del promotor autenticado. El Handler resuelve `PromotorId` desde `UserId`. Si no se proporciona `ProgramaId`, devuelve metricas agregadas de todos los programas del promotor.

**Implementa:** `IRequest<ServiceResponse<PromotorMetricasResponseDto>>`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| `UserId` | `string` | Si | Inyectado por el Controller desde el claim `sub` del JWT. |
| `ProgramaId` | `Guid?` | No | Filtrar por programa especifico. Sin valor: todos los programas del promotor. |
| `FechaDesde` | `DateOnly?` | No | Fecha inicio del periodo. Sin valor: 90 dias atras. |
| `FechaHasta` | `DateOnly?` | No | Fecha fin del periodo. Sin valor: hoy (UTC). |

**Validacion de negocio en Handler (no en Validator):**
- Resolver `PromotorId` buscando `Promotor` por `UserId`. Si no existe: retornar 404 con `NotFound_Promotor` (2015).
- Si `FechaDesde > FechaHasta`: retornar 400 con `Validation_FechaRangoInvalido` (1036).

---

## 3. Response DTOs

### 3.1 RegistrarEventoResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarEventoResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `EventoId` | `Guid` | ID del PromoEvento creado. Presente siempre que el evento se inserte en BD, independientemente de si el CodigoReferido es valido. |
| `Registrado` | `bool` | Siempre `true` cuando se devuelve 201. Incluido para claridad en el contrato. |

**Wrapped en:** `ServiceResponse<RegistrarEventoResponseDto>`

---

### 3.2 RegistrarConversionResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarConversionResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `EventoId` | `Guid` | ID del PromoEvento tipo Backing creado. |
| `ComisionCalculada` | `decimal` | Importe de la comision calculada. Cero si el programa estaba inactivo, el promotor fue dado de baja, o el codigo referido no existe. |
| `MonedaNombre` | `string?` | Nombre de la moneda de la comision. Null si `ComisionAcreditada = false`. |
| `WalletTransaccionId` | `Guid?` | ID de la PromotorWalletTransaccion creada. Null si no se genero comision. |
| `ComisionAcreditada` | `bool` | `true` solo cuando se creo `PromotorWalletTransaccion` exitosamente y se actualizo el saldo. |

**Wrapped en:** `ServiceResponse<RegistrarConversionResponseDto>`

---

### 3.3 ProgramaMetricasKpisDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaMetricasKpisDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `TotalClicks` | `int` | COUNT(PromoEvento WHERE TipoEventoPromoId=1 AND ProgramaId=X AND FechaEvento BETWEEN fechaDesde AND fechaHasta) |
| `TotalPageViews` | `int` | COUNT con TipoEventoPromoId=2 |
| `TotalSignups` | `int` | COUNT con TipoEventoPromoId=3 |
| `TotalConversiones` | `int` | COUNT con TipoEventoPromoId=4 |
| `ValorTotalGenerado` | `decimal` | SUM(ValorMonetario) WHERE TipoEventoPromoId=4 en el mismo filtro. Cero si sin conversiones. |
| `MonedaNombre` | `string?` | Resuelto desde `PromoPrograma.MonedaId`. Null si no hay conversiones. |
| `TasaConversion` | `decimal` | `TotalConversiones / TotalClicks * 100`. Cero si `TotalClicks = 0`. Redondeado a 2 decimales. |
| `ComisionesTotales` | `decimal` | SUM de `PromotorWalletTransaccion.Importe` vinculadas a conversiones del programa en el periodo. |

---

### 3.4 RankingPromotorItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RankingPromotorItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PromotorId` | `Guid` | ID del promotor. |
| `PromotorNombre` | `string` | `Promotor.NombrePublico`. |
| `TipoPromotorNombre` | `string?` | Resuelto desde `MaestraTipoPromotor`. Null si no se puede resolver. |
| `Clicks` | `int` | COUNT de eventos tipo Click de este promotor en el periodo. |
| `PageViews` | `int` | COUNT de eventos tipo PageView. |
| `Signups` | `int` | COUNT de eventos tipo Signup. |
| `Conversiones` | `int` | COUNT de eventos tipo Backing. Usado para ordenar el ranking DESC. |
| `ValorGenerado` | `decimal` | SUM(ValorMonetario) de eventos tipo Backing de este promotor. |
| `ComisionAcumulada` | `decimal` | SUM de `PromotorWalletTransaccion.Importe` de este promotor en el programa y periodo. |

**Nota:** Solo incluye promotores con al menos 1 evento en el periodo filtrado.

---

### 3.5 EventosPorDiaItemDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/EventosPorDiaItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Fecha` | `string` | Fecha en formato `YYYY-MM-DD`. Calculada con `CAST(FechaEvento AS DATE)` en la query GROUP BY. |
| `Clicks` | `int` | COUNT de eventos tipo Click en ese dia. |
| `PageViews` | `int` | COUNT de eventos tipo PageView. |
| `Signups` | `int` | COUNT de eventos tipo Signup. |
| `Conversiones` | `int` | COUNT de eventos tipo Backing. |

**Nota:** Solo dias con al menos 1 evento. No se incluyen dias intermedios con cero actividad.

---

### 3.6 ProgramaMetricasResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaMetricasResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `ProgramaId` | `Guid` | ID del programa consultado. |
| `ProgramaTitulo` | `string` | Titulo del programa. |
| `FechaDesde` | `string` | Fecha inicio del periodo aplicado en formato `YYYY-MM-DD`. |
| `FechaHasta` | `string` | Fecha fin del periodo aplicado en formato `YYYY-MM-DD`. |
| `Kpis` | `ProgramaMetricasKpisDto` | KPIs agregados del programa en el periodo. |
| `RankingPromotores` | `IReadOnlyList<RankingPromotorItemDto>` | Ranking ordenado por `Conversiones DESC`. |
| `EventosPorDia` | `IReadOnlyList<EventosPorDiaItemDto>` | Serie temporal. Solo dias con actividad. |

**Wrapped en:** `ServiceResponse<ProgramaMetricasResponseDto>`

---

### 3.7 PromotorMetricasKpisDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorMetricasKpisDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `MisClicks` | `int` | COUNT de PromoEvento donde `PromoProgramaPromotorId` es del promotor y `TipoEventoPromoId=1`. |
| `MisPageViews` | `int` | COUNT con TipoEventoPromoId=2. |
| `MisSignups` | `int` | COUNT con TipoEventoPromoId=3. |
| `MisConversiones` | `int` | COUNT con TipoEventoPromoId=4. |
| `MiValorGenerado` | `decimal` | SUM(ValorMonetario) de conversiones propias en el periodo. |
| `MiComisionAcumulada` | `decimal` | SUM de `PromotorWalletTransaccion.Importe` del promotor en el periodo. |
| `MonedaNombre` | `string?` | Moneda de la comision. Null si sin conversiones. |
| `MiTasaConversion` | `decimal` | `MisConversiones / MisClicks * 100`. Cero si `MisClicks = 0`. Redondeado a 2 decimales. |

---

### 3.8 EventoRecienteDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/EventoRecienteDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del PromoEvento. |
| `TipoEventoId` | `int` | Siempre 4 (Backing) en esta lista, pero se incluye para consistencia con el tipo del evento. |
| `TipoEventoNombre` | `string` | Resuelto desde `Maestra_TipoEventoPromo`. Siempre "Backing" en esta lista. |
| `ValorMonetario` | `decimal` | Importe del backing asociado. |
| `ComisionGenerada` | `decimal?` | Comision acreditada para este evento especifico. Null si no se genero comision (programa inactivo o promotor dado de baja al momento del evento). |
| `MonedaNombre` | `string?` | Nombre de la moneda. Resuelto desde `MaestraMoneda`. |
| `FechaEvento` | `DateTime` | Timestamp del evento en UTC. Serializa como ISO 8601 (`2026-03-20T14:30:00Z`). |

**Nota:** Solo se incluyen eventos tipo Backing (TipoEventoPromoId=4) porque son los que tienen `ValorMonetario`. Maximo 20 items, ordenados por `FechaEvento DESC`.

---

### 3.9 PromotorMetricasResponseDto

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorMetricasResponseDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PromotorId` | `Guid` | ID del promotor autenticado. |
| `PromotorNombre` | `string` | `Promotor.NombrePublico`. |
| `ProgramaId` | `Guid?` | ID del programa filtrado. Null si la consulta no filtra por programa especifico. |
| `ProgramaTitulo` | `string?` | Titulo del programa filtrado. Null si sin filtro de programa. |
| `FechaDesde` | `string` | Fecha inicio del periodo aplicado en formato `YYYY-MM-DD`. |
| `FechaHasta` | `string` | Fecha fin del periodo aplicado en formato `YYYY-MM-DD`. |
| `Kpis` | `PromotorMetricasKpisDto` | KPIs individuales del promotor. |
| `EventosRecientes` | `IReadOnlyList<EventoRecienteDto>` | Ultimos 20 eventos tipo Backing. Ordenados por `FechaEvento DESC`. |

**Wrapped en:** `ServiceResponse<PromotorMetricasResponseDto>`

---

## 4. Validadores FluentValidation

### 4.1 RegistrarEventoCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/RegistrarEventoCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `TipoEventoPromoId` | `NotEmpty()` / `GreaterThan(0)` | "El tipo de evento es obligatorio" | `Validation_Required` ("1001") |
| `TipoEventoPromoId` | `InclusiveBetween(1, 5)` | "Tipo de evento invalido. Valores validos: 1 (Click), 2 (PageView), 3 (Signup), 4 (Backing), 5 (Share)" | `Validation_TipoEventoInvalido` ("1033") |
| `TipoEventoPromoId` | `NotEqual(4)` | "El tipo de evento Backing no se acepta en este endpoint. Use el endpoint de conversion" | `Validation_BackingNoPermitido` ("1034") |
| `CodigoReferido` | `MaximumLength(50)` cuando `!string.IsNullOrEmpty` | "El codigo referido no puede superar los 50 caracteres" | `Validation_MaxLength` ("1002") |
| `UrlOrigen` | `MaximumLength(2048)` cuando `!string.IsNullOrEmpty` | "La URL de origen no puede superar los 2048 caracteres" | `Validation_MaxLength` ("1002") |
| `UrlReferer` | `MaximumLength(2048)` cuando `!string.IsNullOrEmpty` | "La URL del referer no puede superar los 2048 caracteres" | `Validation_MaxLength` ("1002") |
| `UtmSource` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty` | "El utm_source no puede superar los 100 caracteres" | `Validation_MaxLength` ("1002") |
| `UtmMedium` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty` | "El utm_medium no puede superar los 100 caracteres" | `Validation_MaxLength` ("1002") |
| `UtmCampaign` | `MaximumLength(100)` cuando `!string.IsNullOrEmpty` | "El utm_campaign no puede superar los 100 caracteres" | `Validation_MaxLength` ("1002") |

**Notas de validacion:**
- `IpOrigen` y `UserIdAfectado` son inyectados por el Controller y no se validan (pueden ser null legitimos).
- `CampaniaCrowdfundingId` es un `Guid?` nativo; si se proporciona un GUID con formato invalido, el model binding lo rechazara antes del Validator.
- La validacion de rate limiting (429) NO se implementa en el Validator, sino en el Handler consultando `IMemoryCache`.

---

### 4.2 RegistrarConversionCommandValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/RegistrarConversionCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `CodigoReferido` | `NotEmpty()` | "El codigo referido es obligatorio" | `Validation_Required` ("1001") |
| `CodigoReferido` | `MaximumLength(50)` | "El codigo referido no puede superar los 50 caracteres" | `Validation_MaxLength` ("1002") |
| `CampaniaCrowdfundingId` | `NotEmpty()` | "La campana es obligatoria" | `Validation_Required` ("1001") |
| `AportacionCrowdfundingId` | `NotEmpty()` | "La aportacion es obligatoria" | `Validation_Required` ("1001") |
| `ValorMonetario` | `GreaterThan(0)` | "El valor monetario debe ser mayor que cero" | `Validation_ValorMonetarioInvalido` ("1035") |
| `MonedaId` | `GreaterThan(0)` | "La moneda es obligatoria" | `Validation_Required` ("1001") |
| `UserIdAfectado` | `NotEmpty()` | "El usuario que realizo el backing es obligatorio" | `Validation_Required` ("1001") |

---

### 4.3 GetProgramaMetricasQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/GetProgramaMetricasQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `ProgramaId` | `NotEmpty()` | "El identificador del programa es obligatorio" | `Validation_Required` ("1001") |
| `UserId` | `NotEmpty()` | "El usuario no pudo ser identificado" | `Validation_Required` ("1001") |
| `FechaDesde` y `FechaHasta` | `.Must((q, f) => f == null \|\| q.FechaHasta == null \|\| f <= q.FechaHasta)` cuando ambas presentes | "La fecha de inicio no puede ser posterior a la fecha fin" | `Validation_FechaRangoInvalido` ("1036") |

**Nota:** La verificacion de propiedad del programa (403) y la existencia del Artista (404) se realizan en el Handler, no en el Validator, porque requieren consultas a la BD.

---

### 4.4 GetPromotorMetricasQueryValidator

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Tracking/Validators/GetPromotorMetricasQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode (Constante) |
|-------|-------|---------|----------------------|
| `UserId` | `NotEmpty()` | "El usuario no pudo ser identificado" | `Validation_Required` ("1001") |
| `FechaDesde` y `FechaHasta` | `.Must((q, f) => f == null \|\| q.FechaHasta == null \|\| f <= q.FechaHasta)` cuando ambas presentes | "La fecha de inicio no puede ser posterior a la fecha fin" | `Validation_FechaRangoInvalido` ("1036") |

---

## 5. Nuevas Constantes en ServiceResponseMessageType

**Archivo a modificar:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Se agregan las siguientes constantes al final de cada seccion correspondiente:

### 5.1 Nuevas constantes de Validacion (1000-1999)

```csharp
// Tracking - US-CP-05
public const string Validation_TipoEventoInvalido = "1033";
public const string Validation_BackingNoPermitido = "1034";
public const string Validation_ValorMonetarioInvalido = "1035";
public const string Validation_FechaRangoInvalido = "1036";
```

### 5.2 Nueva constante de Negocio (4000-4999)

```csharp
// Tracking Rate Limiting - US-CP-05
public const string BusinessRule_RateLimitExcedido = "4032";
```

### 5.3 Nuevas constantes de Not Found (2000-2999) - ya existentes, verificar

Los codigos `NotFound_Promotor` ("2015") y `NotFound_Artista` ("2016") y `NotFound_PromoPrograma` ("2019") ya existen en el archivo y se reutilizan en los Handlers de metricas.

---

## 6. AutoMapper Profiles

### 6.1 PromoEventoProfile

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoEventoProfile.cs`

**Descripcion:** Profile para los mappings de tracking. Los mappings de Response DTOs (ProgramaMetricasResponseDto, PromotorMetricasResponseDto) NO usan AutoMapper porque se calculan mediante queries agregadas (GROUP BY) directamente en el Service. Solo se define el mapping de Command -> Entity para la creacion del PromoEvento.

| Source | Destination | Tipo | Notas |
|--------|-------------|------|-------|
| `RegistrarEventoCommand` | `PromoEvento` | Command -> Entity | `TipoEventoId` mapea desde `TipoEventoPromoId`. `FechaCreacion` se asigna en el Service. `ProgramaId` y `PromotorId` se resuelven en el Service via CodigoReferido. |
| `RegistrarConversionCommand` | `PromoEvento` | Command -> Entity | `TipoEventoId` fijo = 4. `ImporteAsociado` mapea desde `ValorMonetario`. `FechaCreacion` en el Service. |

**Mappings que NO usan AutoMapper (calculados en Service):**
- `ProgramaMetricasKpisDto` - Construido desde resultados de query GROUP BY en `IPromoEventoService.GetProgramaMetricasAsync()`
- `RankingPromotorItemDto` - Construido desde JOIN con GROUP BY
- `EventosPorDiaItemDto` - Construido desde GROUP BY fecha
- `PromotorMetricasKpisDto` - Construido desde query filtrada por PromotorId
- `EventoRecienteDto` - Construido desde query de PromoEvento tipo Backing con JOIN a PromotorWalletTransaccion

**Nota sobre el modelo de dominio:** La entidad `PromoEvento` ya existe en el dominio con los campos `TipoEventoId`, `ImporteAsociado`, `CodigoReferido`, `IpOrigen`, etc. El mapping debe ignorar los campos de navegacion (`Programa`, `Promotor`) ya que se asignan via FK en el Service.

#### Detalle de mappings

**RegistrarEventoCommand -> PromoEvento:**

| Source | Destination | Notas |
|--------|-------------|-------|
| `TipoEventoPromoId` | `TipoEventoId` | Mapeo directo |
| `CampaniaCrowdfundingId` | `CampaniaCrowdfundingId` | Mapeo directo, nullable |
| `UrlOrigen` | `IpOrigen` | NO - UrlOrigen no existe en la entidad actual. Ver nota. |
| `CodigoReferido` | `CodigoReferido` | Mapeo directo |
| `IpOrigen` | `IpOrigen` | Mapeo directo |
| `UserIdAfectado` | Ignorado en PromoEvento | La entidad no tiene `UserIdAfectado` actualmente |
| `Ignore` | `Id` | Asignado en Service |
| `Ignore` | `ProgramaId` | Resuelto en Service via CodigoReferido |
| `Ignore` | `PromotorId` | Resuelto en Service via CodigoReferido |
| `Ignore` | `FechaCreacion` | Asignado en Service como `DateTime.UtcNow` |
| `Ignore` | `MonedaId` | Null para eventos no-Backing |
| `Ignore` | `ImporteAsociado` | Null para eventos no-Backing |
| `Ignore` | `Programa` | Navigation property |
| `Ignore` | `Promotor` | Navigation property |

**Nota critica:** La entidad `PromoEvento` actual en el dominio no tiene campos como `UrlOrigen`, `UrlReferer`, `UtmSource`, `UtmMedium`, `UtmCampaign`, `UserIdAfectado`. Estos campos estan definidos en el contracts.md como parte del modelo conceptual pero la entidad actual del dominio tiene `IpOrigen` y `UserAgentOrigen`. El implementador debera verificar si la entidad debe extenderse con estos campos o adaptarse a los campos existentes.

**RegistrarConversionCommand -> PromoEvento:**

| Source | Destination | Notas |
|--------|-------------|-------|
| `ValorMonetario` | `ImporteAsociado` | Mapeo directo |
| `MonedaId` | `MonedaId` | Mapeo directo |
| `CampaniaCrowdfundingId` | `CampaniaCrowdfundingId` | Mapeo directo |
| `AportacionCrowdfundingId` | `AportacionCrowdfundingId` | Mapeo directo |
| `CodigoReferido` | `CodigoReferido` | Mapeo directo |
| `Ignore` | `TipoEventoId` | Fijado en 4 (Backing) en el Service |
| `Ignore` | `Id` | Asignado en Service |
| `Ignore` | `ProgramaId` | Resuelto en Service via CodigoReferido |
| `Ignore` | `PromotorId` | Resuelto en Service via CodigoReferido |
| `Ignore` | `FechaCreacion` | Asignado en Service |
| `Ignore` | `Programa` | Navigation property |
| `Ignore` | `Promotor` | Navigation property |

---

## 7. Interfaces de Servicio

### 7.1 IPromoEventoService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoEventoService.cs`

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `RegistrarEventoAsync` | `Task<PromoEvento> RegistrarEventoAsync(PromoEvento evento, CancellationToken ct)` | Resuelve CodigoReferido a ProgramaId+PromotorId, crea PromoEvento. Retorna la entidad creada. |
| `RegistrarConversionAsync` | `Task<(PromoEvento evento, decimal comision, Guid? walletTransaccionId, bool comisionAcreditada)> RegistrarConversionAsync(PromoEvento evento, CancellationToken ct)` | Crea PromoEvento tipo Backing + calcula comision + crea PromotorWalletTransaccion en transaccion atomica. |
| `GetProgramaMetricasAsync` | `Task<ProgramaMetricasResponseDto> GetProgramaMetricasAsync(Guid programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)` | Calcula KPIs, ranking y serie temporal via queries GROUP BY. Retorna DTO directamente (excepcion justificada por naturaleza de query agregada). |
| `GetPromotorMetricasAsync` | `Task<PromotorMetricasResponseDto> GetPromotorMetricasAsync(Guid promotorId, Guid? programaId, DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken ct)` | Calcula KPIs individuales y eventos recientes. Retorna DTO directamente. |

**Nota sobre la excepcion a la Regla 4:** Los metodos `GetProgramaMetricasAsync` y `GetPromotorMetricasAsync` retornan DTOs directamente desde el Service en lugar de entidades. Esto es una excepcion justificada porque:
1. Los datos son calculados (agregaciones GROUP BY), no entidades del dominio.
2. Retornar entidades intermedias forzaria una segunda iteracion sobre colecciones ya calculadas sin beneficio.
3. Los Handlers de las Queries siguen siendo los responsables del `ServiceResponse<T>`.

---

### 7.2 IRateLimitingService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IRateLimitingService.cs`

| Metodo | Firma | Descripcion |
|--------|-------|-------------|
| `IsClickRateLimitedAsync` | `Task<bool> IsClickRateLimitedAsync(string ip, string? codigoReferido, CancellationToken ct)` | Verifica si IP + codigoReferido ya registro un Click en los ultimos 5 minutos. Retorna `true` si el rate limit ha sido excedido. |
| `RegisterClickAsync` | `Task RegisterClickAsync(string ip, string? codigoReferido, CancellationToken ct)` | Registra la clave IP + codigoReferido en cache con TTL de 5 minutos. Llamado despues de insertar el PromoEvento exitosamente. |

**Implementacion:** Usar `IMemoryCache` de ASP.NET Core. Clave: `$"click_rl:{ip}:{codigoReferido ?? "anonimo"}"`. TTL: `TimeSpan.FromMinutes(5)`.

---

## 8. Controller Actions

### 8.1 TrackingController

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/TrackingController.cs`

**Base Route:** `[Route("api/crowdpromotion/tracking")]`

#### POST /tracking/evento

```
[HttpPost("evento")]
[AllowAnonymous]
[ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ServiceResponse<RegistrarEventoResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> RegistrarEvento(
    [FromBody] RegistrarEventoDto request,
    CancellationToken ct)
```

**Logica del Controller:**
1. Construir `RegistrarEventoCommand` desde el body
2. Asignar `command.IpOrigen = HttpContext.Connection.RemoteIpAddress?.ToString()`
3. Si el usuario tiene JWT valido: `command.UserIdAfectado = _currentUser.UserId?.ToString()`
4. El Controller usa `[AllowAnonymous]` pero el `ICurrentUserService` puede retornar null sin error
5. Enviar via MediatR, retornar 201 si `result.IsSuccess`

**Nota sobre el return 429:** El Handler retornara un `ServiceResponse` con `ErrorCode = BusinessRule_RateLimitExcedido`. El Controller interpreta este codigo y retorna `StatusCode(429, result)` en lugar del 400 estandar. Requiere logica adicional en `FromServiceResponse` o manejo manual.

#### POST /tracking/conversion

```
[HttpPost("conversion")]
[Authorize]  // Uso interno - el handler de Crowdfunding pasa autenticacion de sistema
[ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<RegistrarConversionResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> RegistrarConversion(
    [FromBody] RegistrarConversionDto request,
    CancellationToken ct)
```

**Nota MVP:** En MVP, el modulo Crowdfunding NO llamara este endpoint por HTTP. El handler de backing inyectara `IPromoEventoService` directamente del modulo Crowdpromotion. Este endpoint se documenta para referencia del contrato pero puede no ser invocado via HTTP.

---

### 8.2 PromoProgramaController (extension)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromoProgramaController.cs` (existente, agregar action)

#### GET /programas/{programaId}/metricas

```
[HttpGet("{programaId:guid}/metricas")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMetricas(
    [FromRoute] Guid programaId,
    [FromQuery] string? fechaDesde,
    [FromQuery] string? fechaHasta,
    CancellationToken ct)
```

**Logica del Controller:**
1. Extraer `UserId` del token. Si null: `return Unauthorized()`
2. Parsear `fechaDesde` y `fechaHasta` de string `YYYY-MM-DD` a `DateOnly?`. Si el formato es invalido: retornar 400.
3. Construir `GetProgramaMetricasQuery` y enviar via MediatR.

---

### 8.3 PromotorController (extension)

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromotorController.cs` (existente, agregar action)

#### GET /promotor/metricas

```
[HttpGet("metricas")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ServiceResponse<PromotorMetricasResponseDto>), StatusCodes.Status500InternalServerError)]
public async Task<IActionResult> GetMetricas(
    [FromQuery] Guid? programaId,
    [FromQuery] string? fechaDesde,
    [FromQuery] string? fechaHasta,
    CancellationToken ct)
```

**Nota de ruta:** El `PromotorController` tiene base route `api/crowdpromotion/promotor`. El action `GetMetricas` queda en `api/crowdpromotion/promotor/metricas`. Verificar que no haya conflicto con el action `GetMe` (si existe) del mismo controller.

---

## 9. Flujo Completo por Endpoint

### 9.1 POST /tracking/evento - Flujo del Handler

```
1. Validar command (FluentValidation)
   -> Error de validacion: return ServiceResponse con errores (400)

2. Verificar rate limiting (solo si TipoEventoPromoId == 1 / Click):
   -> _rateLimitingService.IsClickRateLimitedAsync(ip, codigoReferido)
   -> Si true: return ServiceResponse con BusinessRule_RateLimitExcedido (429)

3. Resolver CodigoReferido (via _promoEventoService o directamente en Service):
   -> Buscar PromoProgramaPromotor donde CodigoReferido == X AND EsAprobado == true AND EsBloqueado == false AND FechaBaja == null
   -> Si encontrado: asignar ProgramaId y PromotorId al evento
   -> Si no encontrado: dejar ProgramaId y PromotorId como null (FA-01)

4. Crear PromoEvento via _promoEventoService.RegistrarEventoAsync(entity, ct)

5. Si TipoEventoPromoId == 1 (Click):
   -> _rateLimitingService.RegisterClickAsync(ip, codigoReferido, ct)

6. Mapear PromoEvento -> RegistrarEventoResponseDto
7. return ServiceResponse<RegistrarEventoResponseDto> con Created (0001)
```

---

### 9.2 POST /tracking/conversion - Flujo del Handler

```
1. Validar command (FluentValidation)

2. Mapear command -> PromoEvento (tipo Backing = 4)

3. _promoEventoService.RegistrarConversionAsync(entity, ct)
   -> Dentro del Service (operacion atomica en una transaccion):
      a. Resolver CodigoReferido -> PromoProgramaPromotor
      b. Crear PromoEvento con ProgramaId, PromotorId, ImporteAsociado
      c. Si promotor valido Y programa activo:
         - Calcular comision segun tipo del programa
         - Crear PromotorWalletTransaccion (Credito, Pendiente)
         - Actualizar PromotorWallet.SaldoPendiente += comision
         - Si wallet no existe: crearla automaticamente (FA-08)
      d. Commit transaccion
      e. Retornar (evento, comision, walletTransaccionId, comisionAcreditada)

4. Construir RegistrarConversionResponseDto con los valores retornados
5. return ServiceResponse<RegistrarConversionResponseDto> con Created (0001)
```

---

### 9.3 GET /programas/{id}/metricas - Flujo del Handler

```
1. Validar query (FluentValidation)

2. Resolver ArtistaId:
   -> _promoEventoService.GetArtistaByUserIdAsync(userId) o via IArtistaService del modulo UserAccess
   -> Si null: return 404 con NotFound_Artista (2016)

3. Verificar programa:
   -> _promoEventoService.GetProgramaByIdAsync(programaId)
   -> Si null: return 404 con NotFound_PromoPrograma (2019)
   -> Si programa.ArtistaId != artista.Id: return 403 con BusinessRule_NoEsPropietarioPrograma (4026)

4. Calcular defaults de fechas si no se proporcionan:
   -> fechaDesde = Min(programa.FechaInicio, hoy - 90 dias) o hoy - 90 dias si FechaInicio es null
   -> fechaHasta = DateOnly.FromDateTime(DateTime.UtcNow)

5. Obtener metricas:
   -> var metricas = await _promoEventoService.GetProgramaMetricasAsync(programaId, fechaDesde, fechaHasta, ct)

6. return ServiceResponse<ProgramaMetricasResponseDto> con Success (0000)
```

---

### 9.4 GET /promotor/metricas - Flujo del Handler

```
1. Validar query (FluentValidation)

2. Resolver PromotorId:
   -> _promoEventoService.GetPromotorByUserIdAsync(userId)
   -> Si null: return 404 con NotFound_Promotor (2015)

3. Calcular defaults de fechas si no se proporcionan:
   -> fechaDesde = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90))
   -> fechaHasta = DateOnly.FromDateTime(DateTime.UtcNow)

4. Obtener metricas:
   -> var metricas = await _promoEventoService.GetPromotorMetricasAsync(promotor.Id, programaId, fechaDesde, fechaHasta, ct)

5. return ServiceResponse<PromotorMetricasResponseDto> con Success (0000)
```

---

## 10. OpenAPI / Swagger Documentation

### POST /api/crowdpromotion/tracking/evento

- **Summary:** Registrar evento de tracking (Click, PageView, Signup, Share)
- **Description:** Endpoint publico para registrar eventos generados por la actividad de un visitante a traves del enlace referido de un promotor. No requiere autenticacion. Para eventos tipo Click aplica rate limiting de 1 evento por IP + codigo referido cada 5 minutos. El tipo Backing (4) no esta permitido en este endpoint; usar `/tracking/conversion` para conversiones.
- **Tags:** `Crowdpromotion`, `Tracking`
- **Request Body:** `RegistrarEventoDto` (application/json)
- **Responses:**
  - 201: `ServiceResponse<RegistrarEventoResponseDto>` - Evento registrado correctamente
  - 400: `ServiceResponse<RegistrarEventoResponseDto>` - Errores de validacion (tipo invalido, backing no permitido, longitud excedida)
  - 429: `ServiceResponse<RegistrarEventoResponseDto>` - Rate limit excedido para eventos tipo Click
  - 500: `ServiceResponse<RegistrarEventoResponseDto>` - Error inesperado
- **Auth:** Ninguna (AllowAnonymous). Si se proporciona Bearer token, se usa para resolver UserIdAfectado.

---

### POST /api/crowdpromotion/tracking/conversion

- **Summary:** Registrar conversion (backing referido) con calculo de comision
- **Description:** Uso INTERNO entre modulos. Registra un evento tipo Backing, calcula la comision del promotor y la acredita en su wallet de forma atomica. En MVP, llamado directamente desde el handler de Crowdfunding via inyeccion de IPromoEventoService.
- **Tags:** `Crowdpromotion`, `Tracking`, `Interno`
- **Request Body:** `RegistrarConversionDto` (application/json)
- **Responses:**
  - 201: `ServiceResponse<RegistrarConversionResponseDto>` - Conversion registrada. `comisionAcreditada` puede ser false si el programa estaba inactivo o el promotor dado de baja.
  - 400: `ServiceResponse<RegistrarConversionResponseDto>` - Errores de validacion
  - 500: `ServiceResponse<RegistrarConversionResponseDto>` - Error inesperado
- **Auth:** Bearer JWT (uso interno)

---

### GET /api/crowdpromotion/programas/{programaId}/metricas

- **Summary:** Dashboard de metricas del programa (vista artista)
- **Description:** Devuelve KPIs agregados, ranking de promotores y serie temporal de eventos por dia para el periodo especificado. El artista debe ser el propietario del programa. Si no se proporcionan fechas, el periodo por defecto son los ultimos 90 dias.
- **Tags:** `Crowdpromotion`, `Metricas`, `Artista`
- **Parameters:**
  - `programaId` (path, Guid, required) - Identificador del programa de promocion
  - `fechaDesde` (query, string YYYY-MM-DD, optional) - Inicio del periodo
  - `fechaHasta` (query, string YYYY-MM-DD, optional) - Fin del periodo
- **Responses:**
  - 200: `ServiceResponse<ProgramaMetricasResponseDto>` - Metricas calculadas
  - 400: `ServiceResponse<ProgramaMetricasResponseDto>` - Rango de fechas invalido
  - 401: No autenticado
  - 403: `ServiceResponse<ProgramaMetricasResponseDto>` - No es propietario del programa
  - 404: `ServiceResponse<ProgramaMetricasResponseDto>` - Artista o programa no encontrado
  - 500: `ServiceResponse<ProgramaMetricasResponseDto>` - Error inesperado
- **Auth:** Bearer JWT (Artista propietario del programa)

---

### GET /api/crowdpromotion/promotor/metricas

- **Summary:** Dashboard de metricas individuales del promotor
- **Description:** Devuelve KPIs propios y historial de eventos recientes (tipo Backing, max 20) para el promotor autenticado. Si no se proporciona `programaId`, se agregan metricas de todos los programas del promotor. Periodo por defecto: ultimos 90 dias.
- **Tags:** `Crowdpromotion`, `Metricas`, `Promotor`
- **Parameters:**
  - `programaId` (query, Guid, optional) - Filtrar por programa especifico
  - `fechaDesde` (query, string YYYY-MM-DD, optional) - Inicio del periodo
  - `fechaHasta` (query, string YYYY-MM-DD, optional) - Fin del periodo
- **Responses:**
  - 200: `ServiceResponse<PromotorMetricasResponseDto>` - Metricas calculadas. `programaId` y `programaTitulo` son null si no se filtro por programa.
  - 400: `ServiceResponse<PromotorMetricasResponseDto>` - Rango de fechas invalido
  - 401: No autenticado
  - 404: `ServiceResponse<PromotorMetricasResponseDto>` - Promotor no encontrado
  - 500: `ServiceResponse<PromotorMetricasResponseDto>` - Error inesperado
- **Auth:** Bearer JWT (Usuario con perfil de promotor)

---

## 11. Tabla de Errores Completa

| HTTP | ErrorCode | Constante | Mensaje | Endpoint | Causa |
|------|-----------|-----------|---------|----------|-------|
| 400 | 1001 | `Validation_Required` | "El tipo de evento es obligatorio" | POST /evento | `tipoEventoPromoId` no enviado o cero |
| 400 | 1002 | `Validation_MaxLength` | "El codigo referido no puede superar los 50 caracteres" | POST /evento | `codigoReferido.Length > 50` |
| 400 | 1002 | `Validation_MaxLength` | "La URL de origen no puede superar los 2048 caracteres" | POST /evento | `urlOrigen.Length > 2048` |
| 400 | 1033 | `Validation_TipoEventoInvalido` | "Tipo de evento invalido. Valores validos: 1 (Click), 2 (PageView), 3 (Signup), 4 (Backing), 5 (Share)" | POST /evento | `tipoEventoPromoId` no esta entre 1-5 |
| 400 | 1034 | `Validation_BackingNoPermitido` | "El tipo de evento Backing no se acepta en este endpoint. Use el endpoint de conversion" | POST /evento | `tipoEventoPromoId == 4` |
| 400 | 1001 | `Validation_Required` | "El codigo referido es obligatorio" | POST /conversion | `codigoReferido` vacio |
| 400 | 1035 | `Validation_ValorMonetarioInvalido` | "El valor monetario debe ser mayor que cero" | POST /conversion | `valorMonetario <= 0` |
| 400 | 1036 | `Validation_FechaRangoInvalido` | "La fecha de inicio no puede ser posterior a la fecha fin" | GET /metricas (ambos) | `fechaDesde > fechaHasta` |
| 401 | 3001 | `Auth_Unauthorized` | "Token no valido o expirado" | GET /metricas (ambos) | JWT invalido o expirado |
| 403 | 4026 | `BusinessRule_NoEsPropietarioPrograma` | "No tienes permiso para ver las metricas de este programa" | GET /programas/{id}/metricas | ArtistaId del token != PromoPrograma.ArtistaId |
| 404 | 2016 | `NotFound_Artista` | "No tienes un perfil de artista" | GET /programas/{id}/metricas | No existe Artista con el UserId del token |
| 404 | 2019 | `NotFound_PromoPrograma` | "El programa de promocion no existe" | GET /programas/{id}/metricas | No existe PromoPrograma con el programaId |
| 404 | 2015 | `NotFound_Promotor` | "No tienes un perfil de promotor" | GET /promotor/metricas | No existe Promotor con el UserId del token |
| 429 | 4032 | `BusinessRule_RateLimitExcedido` | "Demasiados clicks en poco tiempo. Intenta de nuevo en unos minutos" | POST /evento | Rate limit excedido para tipo Click (1 por 5 min por IP + codigoReferido) |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado al registrar el evento" | POST /evento | Excepcion no controlada |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado al registrar la conversion" | POST /conversion | Excepcion no controlada |
| 500 | 5000 | `Internal_UnexpectedError` | "Error inesperado al obtener las metricas" | GET /metricas (ambos) | Excepcion no controlada |

---

## 12. Archivos a Crear o Modificar

```
src/api/Modules/Crowdpromotion/
│
├── WePlayRises.Crowdpromotion.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs         MODIFICAR: agregar 5 constantes nuevas (1033, 1034, 1035, 1036, 4032)
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Dtos/
│   │   ├── RegistrarEventoDto.cs                  NUEVO: DTO del body del request publico
│   │   ├── RegistrarEventoResponseDto.cs          NUEVO: Response de POST /evento
│   │   ├── RegistrarConversionDto.cs              NUEVO: DTO del body del request interno
│   │   ├── RegistrarConversionResponseDto.cs      NUEVO: Response de POST /conversion
│   │   ├── ProgramaMetricasKpisDto.cs             NUEVO: KPIs del programa (artista)
│   │   ├── RankingPromotorItemDto.cs              NUEVO: Item del ranking de promotores
│   │   ├── EventosPorDiaItemDto.cs                NUEVO: Item de la serie temporal
│   │   ├── ProgramaMetricasResponseDto.cs         NUEVO: Response completo del artista
│   │   ├── PromotorMetricasKpisDto.cs             NUEVO: KPIs individuales del promotor
│   │   ├── EventoRecienteDto.cs                   NUEVO: Evento reciente en historial del promotor
│   │   └── PromotorMetricasResponseDto.cs         NUEVO: Response completo del promotor
│   │
│   ├── Features/
│   │   └── Tracking/
│   │       ├── Commands/
│   │       │   ├── RegistrarEventoCommand.cs      NUEVO: Command + Handler (mismo archivo)
│   │       │   └── RegistrarConversionCommand.cs  NUEVO: Command + Handler (mismo archivo)
│   │       ├── Queries/
│   │       │   ├── GetProgramaMetricasQuery.cs    NUEVO: Query + Handler (mismo archivo)
│   │       │   └── GetPromotorMetricasQuery.cs    NUEVO: Query + Handler (mismo archivo)
│   │       └── Validators/
│   │           ├── RegistrarEventoCommandValidator.cs      NUEVO
│   │           ├── RegistrarConversionCommandValidator.cs  NUEVO
│   │           ├── GetProgramaMetricasQueryValidator.cs    NUEVO
│   │           └── GetPromotorMetricasQueryValidator.cs    NUEVO
│   │
│   ├── Interfaces/
│   │   └── Services/
│   │       ├── IPromoEventoService.cs             NUEVO: Interfaz del servicio de tracking
│   │       └── IRateLimitingService.cs            NUEVO: Interfaz del rate limiting
│   │
│   └── Mapping/
│       └── PromoEventoProfile.cs                  NUEVO: AutoMapper profile para PromoEvento
│
├── WePlayRises.Crowdpromotion.WebApi/
│   └── Controllers/
│       ├── TrackingController.cs                  NUEVO: POST /evento y POST /conversion
│       ├── PromoProgramaController.cs             MODIFICAR: agregar GET {id}/metricas
│       └── PromotorController.cs                  MODIFICAR: agregar GET metricas
│
└── WePlayRises.Crowdpromotion.Infra/
    └── Services/
        ├── PromoEventoService.cs                  NUEVO: Implementacion de IPromoEventoService
        └── RateLimitingService.cs                 NUEVO: Implementacion con IMemoryCache
```

---

## 13. Alineamiento Backend-Frontend

| DTO C# | Interface TypeScript | Notas de Serializacion |
|--------|---------------------|------------------------|
| `RegistrarEventoDto` | `RegistrarEventoRequest` | JSON camelCase via `System.Text.Json`. `TipoEventoPromoId` en C# serializa como `tipoEventoPromoId` en JSON. |
| `RegistrarEventoResponseDto` | `RegistrarEventoResponse` | `EventoId: Guid` serializa como string en JSON. |
| `RegistrarConversionDto` | `RegistrarConversionRequest` | `ValorMonetario: decimal` serializa como number en JSON. |
| `RegistrarConversionResponseDto` | `RegistrarConversionResponse` | `WalletTransaccionId: Guid?` serializa como string en JSON o null. TS lo modela como `string \| undefined`. |
| `ProgramaMetricasKpisDto` | `ProgramaMetricasKpis` | `decimal` serializa como number. `MonedaNombre: string?` serializa como `string \| null`. |
| `RankingPromotorItemDto` | `RankingPromotorItem` | `PromotorId: Guid` serializa como string. |
| `EventosPorDiaItemDto` | `EventosPorDiaItem` | `Fecha: string` en formato `YYYY-MM-DD` (formateado en el Service, no en el serializer). |
| `ProgramaMetricasResponseDto` | `ProgramaMetricasResponse` | `IReadOnlyList<T>` serializa como array JSON. |
| `PromotorMetricasKpisDto` | `PromotorMetricasKpis` | Alineamiento completo. |
| `EventoRecienteDto` | `EventoReciente` | `FechaEvento: DateTime` serializa como ISO 8601 UTC string en JSON. |
| `PromotorMetricasResponseDto` | `PromotorMetricasResponse` | `ProgramaId: Guid?` serializa como string o null en JSON. |

---

## 14. Checklist de Implementacion

### Constantes

- [ ] Agregar `Validation_TipoEventoInvalido = "1033"` a `ServiceResponseMessageType.cs`
- [ ] Agregar `Validation_BackingNoPermitido = "1034"` a `ServiceResponseMessageType.cs`
- [ ] Agregar `Validation_ValorMonetarioInvalido = "1035"` a `ServiceResponseMessageType.cs`
- [ ] Agregar `Validation_FechaRangoInvalido = "1036"` a `ServiceResponseMessageType.cs`
- [ ] Agregar `BusinessRule_RateLimitExcedido = "4032"` a `ServiceResponseMessageType.cs`

### DTOs de Request

- [ ] `RegistrarEventoDto.cs` - 8 propiedades (body del cliente, sin IpOrigen ni UserIdAfectado)
- [ ] `RegistrarConversionDto.cs` - 6 propiedades todas obligatorias
- [ ] Verificar que el model binding rechaza correctamente `Guid` con formato invalido (no se necesita validator adicional)

### DTOs de Response

- [ ] `RegistrarEventoResponseDto.cs` - 2 propiedades
- [ ] `RegistrarConversionResponseDto.cs` - 5 propiedades (ComisionCalculada, MonedaNombre, WalletTransaccionId, ComisionAcreditada, EventoId)
- [ ] `ProgramaMetricasKpisDto.cs` - 8 propiedades
- [ ] `RankingPromotorItemDto.cs` - 9 propiedades
- [ ] `EventosPorDiaItemDto.cs` - 5 propiedades (Fecha como string YYYY-MM-DD)
- [ ] `ProgramaMetricasResponseDto.cs` - con IReadOnlyList para RankingPromotores y EventosPorDia
- [ ] `PromotorMetricasKpisDto.cs` - 8 propiedades
- [ ] `EventoRecienteDto.cs` - 7 propiedades (ComisionGenerada nullable)
- [ ] `PromotorMetricasResponseDto.cs` - con IReadOnlyList para EventosRecientes

### Commands y Queries (con sus Handlers en el mismo archivo)

- [ ] `RegistrarEventoCommand.cs` - Command con 9 propiedades + Handler con `IPromoEventoService`, `IRateLimitingService`, `IMapper`, `IValidator`, `ILogger`
- [ ] `RegistrarConversionCommand.cs` - Command con 6 propiedades + Handler con `IPromoEventoService`, `IMapper`, `IValidator`, `ILogger`
- [ ] `GetProgramaMetricasQuery.cs` - Query con 4 propiedades + Handler con `IPromoEventoService`, `IValidator`, `ILogger`
- [ ] `GetPromotorMetricasQuery.cs` - Query con 4 propiedades + Handler con `IPromoEventoService`, `IValidator`, `ILogger`
- [ ] Todos los constructores con `?? throw new ArgumentNullException(nameof(dep))`
- [ ] Todos los Handlers con try-catch y logging

### Validators (FluentValidation)

- [ ] `RegistrarEventoCommandValidator.cs` - 9 reglas con `WithMessage` + `WithErrorCode` usando constantes
- [ ] `RegistrarConversionCommandValidator.cs` - 7 reglas con `WithMessage` + `WithErrorCode`
- [ ] `GetProgramaMetricasQueryValidator.cs` - 3 reglas incluyendo cross-field de fechas
- [ ] `GetPromotorMetricasQueryValidator.cs` - 2 reglas incluyendo cross-field de fechas
- [ ] TODOS los validators usan `ServiceResponseMessageType.X` (no strings literales)

### AutoMapper Profile

- [ ] `PromoEventoProfile.cs` - 2 mappings (Command -> Entity), con `Ignore()` en campos calculados
- [ ] Verificar que `TipoEventoId` se mapea correctamente desde `TipoEventoPromoId`
- [ ] DTOs de metricas NO usan AutoMapper (construidos en Service directamente)

### Interfaces de Servicio

- [ ] `IPromoEventoService.cs` - 4 metodos (RegistrarEvento, RegistrarConversion, GetProgramaMetricas, GetPromotorMetricas)
- [ ] `IRateLimitingService.cs` - 2 metodos (IsClickRateLimited, RegisterClick)

### Controllers

- [ ] `TrackingController.cs` - 2 endpoints ([AllowAnonymous] para /evento, [Authorize] para /conversion)
- [ ] Logica de extraccion de IpOrigen en el Controller (no en el Handler)
- [ ] Manejo del 429 en el Controller (verificar si `FromServiceResponse` de base lo soporta o requiere manejo manual)
- [ ] `PromoProgramaController.cs` - Agregar action `GetMetricas` con parseo de fechas
- [ ] `PromotorController.cs` - Agregar action `GetMetricas`

### Registro DI

- [ ] Registrar `IPromoEventoService` -> `PromoEventoService` como Scoped en `DependencyInjection.cs`
- [ ] Registrar `IRateLimitingService` -> `RateLimitingService` como Singleton en `DependencyInjection.cs`
- [ ] Registrar validators con `services.AddValidatorsFromAssembly(typeof(...).Assembly)`
- [ ] Registrar `PromoEventoProfile` con AutoMapper

### Verificaciones Finales CQRS

- [ ] Handlers en mismo archivo que Command/Query
- [ ] Handlers retornan `ServiceResponse<T>`
- [ ] Handlers inyectan Services (no DbContext)
- [ ] Handlers usan `?? throw new ArgumentNullException` para todas las dependencias
- [ ] Validacion retorna `ServiceResponse` (no throw de ValidationException)
- [ ] `ServiceResponseMessageType.X` en TODOS los `WithErrorCode` y en respuestas de Handler
- [ ] Logger inyectado y usado en try-catch de TODOS los Handlers
- [ ] `PromoEventoProfile` registrado en AutoMapper

---

## 15. Notas de Implementacion Criticas

### Nota 1: Entidad PromoEvento vs Contrato

La entidad `PromoEvento` existente en el dominio tiene campos `IpOrigen` y `UserAgentOrigen`, pero los contratos definen campos `UrlOrigen`, `UrlReferer`, `UtmSource`, `UtmMedium`, `UtmCampaign` y `UserIdAfectado`. El implementador debe decidir antes de codificar si:

**Opcion A (recomendada):** Extender la entidad `PromoEvento` con los campos faltantes y crear una nueva migracion de BD.

**Opcion B (minima):** Usar los campos existentes y mapear de forma aproximada (`IpOrigen` ya existe, ignorar UTMs en MVP si no hay columnas).

La especificacion del contracts.md es la fuente de verdad: se recomienda la Opcion A.

### Nota 2: Rate Limiting 429 en el Controller

El `BaseLoggerController.FromServiceResponse()` existente probablemente mapea errores a 400. Para retornar un 429 correcto cuando el `ServiceResponse` contiene el codigo `4032`, el `TrackingController` debe manejar este caso manualmente:

```csharp
var result = await _mediator.Send(command, ct);
if (result.Messages.Any(m => m.ErrorCode == ServiceResponseMessageType.BusinessRule_RateLimitExcedido))
    return StatusCode(429, result);
return FromServiceResponse(result);
```

### Nota 3: IPromoEventoService retorna DTOs de metricas

Los metodos `GetProgramaMetricasAsync` y `GetPromotorMetricasAsync` de `IPromoEventoService` retornan DTOs directamente desde el Service, no entidades. Esto es una excepcion deliberada a la Regla 4 del CQRS porque los datos son el resultado de queries agregadas (GROUP BY) sin entidad de dominio equivalente. Esta excepcion debe documentarse en el codigo con un comentario XML.

### Nota 4: Transaccion en RegistrarConversionAsync

El metodo `RegistrarConversionAsync` en `PromoEventoService` debe envolver las tres operaciones (crear PromoEvento, crear PromotorWalletTransaccion, actualizar PromotorWallet) en una transaccion de BD explicita. Si el `IPromotorWalletRepository` no tiene soporte de transaccion, puede ser necesario acceder al DbContext del modulo Crowdpromotion directamente desde el Service o usar `IUnitOfWork` si existe en el proyecto.

### Nota 5: Dependencia Cross-Modulo

El handler `GetProgramaMetricasQuery` necesita resolver el `ArtistaId` del usuario, lo que requiere acceder a la entidad `Artista` del modulo `UserAccess`. Opciones:
- Inyectar `IArtistaService` del modulo UserAccess en el handler de Crowdpromotion (dependencia cross-modulo)
- Crear un endpoint dedicado en UserAccess y llamarlo via HTTP (sobre-ingenieria para MVP)
- Aceptar el `ArtistaId` como parametro adicional del controller (lo extrae directamente del claim si se agrega al token)

La opcion mas pragmatica para MVP: si el `ArtistaId` no esta en el token, inyectar `IArtistaService` del modulo UserAccess en el `PromoEventoService`.
