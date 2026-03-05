# Arquitectura Hexagonal: cp-tracking-metricas

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-tracking-metricas (US-CP-05)

---

## 1. Resumen Ejecutivo

Esta feature implementa el motor de tracking y medicion del modulo Crowdpromotion. Registra eventos de navegacion (Click, PageView, Signup, Backing) con trazabilidad completa de promotor, programa, UTMs y URL de origen, calcula comisiones al detectar conversiones y las acredita en la wallet del promotor de forma transaccional. Expone dashboards de metricas agregadas para artistas (KPIs globales, ranking, serie temporal) y metricas individuales para promotores.

---

## 2. Analisis de Entidades Existentes vs Requeridas

### 2.1 Estado Actual de PromoEvento

La entidad `PromoEvento` ya existe en:
`src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoEvento.cs`

**Campos existentes:**

| Campo | Tipo | Notas |
|-------|------|-------|
| Id | Guid | PK |
| ProgramaId | PromoProgramaId? | FK a PromoPrograma |
| PromotorId | PromotorId? | FK a Promotor (DIRECTO al Promotor, NO al join) |
| CampaniaCrowdfundingId | CampaniaCrowdfundingId? | OK |
| PedidoCrowdfundingId | PedidoCrowdfundingId? | Presente pero no necesario para esta feature |
| AportacionCrowdfundingId | AportacionCrowdfundingId? | OK |
| TipoEventoId | int | Renombrar concepto a TipoEventoPromoId en contratos |
| MonedaId | int? | OK |
| ImporteAsociado | decimal? | Equivalente a ValorMonetario del contrato |
| CodigoReferido | string? | OK |
| IpOrigen | string? | OK para rate limiting |
| UserAgentOrigen | string? | OK |
| FechaCreacion | DateTime | Equivalente a FechaEvento del contrato |

**Campos FALTANTES** que el contrato requiere:

| Campo | Tipo | Motivo |
|-------|------|--------|
| PromoProgramaPromotorId | Guid? | FK a PromoProgramaPromotor (el join table); contracts usa `PromoPrograma_Promotor_Id` |
| UserIdAfectado | string? | UserId del fan que realizo la accion |
| UrlOrigen | string? | URL completa desde donde llego el click |
| UrlReferer | string? | HTTP Referer header |
| UtmSource | string? | UTM source |
| UtmMedium | string? | UTM medium |
| UtmCampaign | string? | UTM campaign |

### 2.2 Decision de Diseno: Actualizacion de PromoEvento

La entidad existente debe ser **actualizada** (no reemplazada) para agregar los 7 campos faltantes. La logica existente en `PromoProgramaService.GetResumenAsync` usa `TipoEventoId` (nombre de campo actual) - cualquier cambio de nombre de campo debe ser compatible.

**Mapeo de nombres contrato -> entidad:**

| Contrato (contracts.md) | Entidad C# | Columna DB |
|-------------------------|------------|-----------|
| `PromoPrograma_Id` | `ProgramaId` | `Programa_Id` |
| `PromoPrograma_Promotor_Id` | `PromoProgramaPromotorId` (NUEVO) | `PromoPrograma_Promotor_Id` |
| `TipoEventoPromo_Id` | `TipoEventoId` | `TipoEvento_Id` |
| `ValorMonetario` | `ImporteAsociado` | `ImporteAsociado` |
| `FechaEvento` | `FechaCreacion` | `FechaCreacion` |
| `UserIdAfectado` | `UserIdAfectado` (NUEVO) | `UserIdAfectado` |
| `UrlOrigen` | `UrlOrigen` (NUEVO) | `UrlOrigen` |
| `UrlReferer` | `UrlReferer` (NUEVO) | `UrlReferer` |
| `UtmSource` | `UtmSource` (NUEVO) | `UtmSource` |
| `UtmMedium` | `UtmMedium` (NUEVO) | `UtmMedium` |
| `UtmCampaign` | `UtmCampaign` (NUEVO) | `UtmCampaign` |

---

## 3. Domain Layer

### 3.1 Entidad Actualizada: PromoEvento

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoEvento.cs`

**Accion:** MODIFICAR (agregar 7 campos nuevos, mantener todos los existentes)

| Propiedad | Tipo | Nullable | Constraint | Descripcion |
|-----------|------|----------|------------|-------------|
| Id | Guid | No | PK | Identificador unico del evento |
| ProgramaId | PromoProgramaId? | Si | FK a PromoPrograma (SetNull on delete) | Programa al que pertenece el evento; null si codigo invalido |
| PromotorId | PromotorId? | Si | FK a Promotor (SetNull) | Referencia directa al Promotor (existente) |
| **PromoProgramaPromotorId** | Guid? | Si | FK a PromoProgramaPromotor (NoAction) | **NUEVO** - FK al registro de inscripcion; null si codigo invalido o anonimo |
| CampaniaCrowdfundingId | CampaniaCrowdfundingId? | Si | FK referencia cruzada | Campana asociada al evento (PageView, Backing) |
| PedidoCrowdfundingId | PedidoCrowdfundingId? | Si | FK referencia cruzada | Pedido asociado (existente, sin uso en esta feature) |
| AportacionCrowdfundingId | AportacionCrowdfundingId? | Si | FK referencia cruzada | Aportacion asociada (solo tipo Backing) |
| TipoEventoId | int | No | FK a Maestra_TipoEventoPromo | 1=Click, 2=PageView, 3=Signup, 4=Backing, 5=Share |
| MonedaId | int? | Si | FK a MaestraMoneda | Moneda de ValorMonetario; solo tipo Backing |
| ImporteAsociado | decimal? | Si | decimal(18,2) | Valor monetario del backing (ValorMonetario en contratos) |
| CodigoReferido | string? | Si | max 50 | Codigo referido recibido en la URL |
| IpOrigen | string? | Si | max 45 | IP del visitante (para rate limiting) |
| UserAgentOrigen | string? | Si | max 500 | User-Agent del navegador |
| **UserIdAfectado** | string? | Si | max 450 | **NUEVO** - UserId del usuario que realizo la accion (fan) |
| **UrlOrigen** | string? | Si | max 2048 | **NUEVO** - URL completa desde donde llego el fan |
| **UrlReferer** | string? | Si | max 2048 | **NUEVO** - HTTP Referer header |
| **UtmSource** | string? | Si | max 100 | **NUEVO** - UTM source (siempre "weplay") |
| **UtmMedium** | string? | Si | max 100 | **NUEVO** - UTM medium ("referral") |
| **UtmCampaign** | string? | Si | max 100 | **NUEVO** - UTM campaign (CodigoTrackingBase del programa) |
| FechaCreacion | DateTime | No | HasPrecision(3) | Timestamp del evento en UTC (FechaEvento en contratos) |

**Navegaciones:**
- `Programa` -> `PromoPrograma?` (N:1, existente)
- `Promotor` -> `Promotor?` (N:1, existente)

**Nota critica:** `PromoProgramaPromotorId` es `Guid?` (no un strongly-typed ID) porque `PromoProgramaPromotor` usa `Guid` como PK sin strongly-typed ID wrapper (ver codigo existente). La relacion de navegacion con `PromoProgramaPromotor` NO se agrega como propiedad de navegacion en la entidad para evitar carga lazy no deseada en el repository de tracking; la FK es suficiente para trazabilidad.

---

### 3.2 Entidades Existentes Sin Cambios

Las siguientes entidades ya existen y no requieren modificacion para esta feature:

| Entidad | Archivo | Uso en feature |
|---------|---------|----------------|
| `PromoPrograma` | `Domain/Model/PromoPrograma.cs` | Leer EsActivo, ImporteComisionPorcentaje, ImporteComisionFija, MonedaId, ArtistaId, Titulo |
| `PromoProgramaPromotor` | `Domain/Model/PromoProgramaPromotor.cs` | Resolver CodigoReferido -> (ProgramaId, PromotorId), verificar EsAprobado, EsBloqueado, FechaBaja |
| `Promotor` | `Domain/Model/Promotor.cs` | Leer NombrePublico, TipoPromotorId para ranking |
| `PromotorWallet` | `Domain/Model/PromotorWallet.cs` | Incrementar SaldoPendiente al acreditar comision |
| `PromotorWalletTransaccion` | `Domain/Model/PromotorWalletTransaccion.cs` | Crear transaccion de credito al registrar conversion |

---

### 3.3 Constants: ServiceResponseMessageType

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Accion:** MODIFICAR - Agregar los siguientes codigos nuevos a la clase existente

**Codigos nuevos a agregar:**

| Constante | Valor | Seccion | Descripcion |
|-----------|-------|---------|-------------|
| `Validation_TipoEventoInvalido` | `"1033"` | Validation (1000-1999) | tipoEventoPromoId fuera del rango 1-5 |
| `Validation_BackingNoPermitido` | `"1034"` | Validation (1000-1999) | tipoEventoPromoId = 4 no permitido en endpoint publico |
| `Validation_ValorMonetarioInvalido` | `"1035"` | Validation (1000-1999) | valorMonetario <= 0 en endpoint de conversion |
| `Validation_FechaDesdePostFechaHasta` | `"1036"` | Validation (1000-1999) | fechaDesde > fechaHasta en filtros de dashboard |
| `BusinessRule_RateLimitExcedido` | `"4032"` | Business Rules (4000-4999) | Rate limit de clicks excedido para IP + codigoReferido |

**Codigos existentes reutilizados por esta feature:**

| Constante | Valor | Uso en esta feature |
|-----------|-------|---------------------|
| `Validation_Required` | `"1001"` | tipoEventoPromoId obligatorio, codigoReferido obligatorio en conversion |
| `Validation_MaxLength` | `"1002"` | codigoReferido > 50 chars |
| `NotFound_Promotor` | `"2015"` | Promotor no encontrado al obtener metricas promotor |
| `NotFound_Artista` | `"2016"` | Artista no encontrado al obtener metricas programa |
| `NotFound_PromoPrograma` | `"2019"` | Programa no encontrado al obtener metricas |
| `Auth_Unauthorized` | `"3001"` | Token JWT invalido o expirado |
| `Auth_Forbidden` | `"3002"` | No es propietario del programa |
| `BusinessRule_NoEsPropietarioPrograma` | `"4026"` | ArtistaId del token != PromoPrograma.ArtistaId |
| `Internal_UnexpectedError` | `"5000"` | Excepcion no controlada |

---

### 3.4 Repository Interface: IPromoEventoRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoEventoRepository.cs`

**Accion:** CREAR (nuevo archivo)

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `AddAsync` | `Guid` | `PromoEvento entity, CancellationToken ct` | Insertar nuevo evento; NO llama SaveChanges (usado en transacciones de Service). Retorna el Id generado |
| `GetMetricasProgramaAsync` | `PromoEventoMetricasRaw` | `PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | Query GROUP BY para KPIs del artista: totalClicks, totalPageViews, totalSignups, totalConversiones, valorTotalGenerado |
| `GetRankingPromotoresAsync` | `IReadOnlyList<PromoEventoRankingRaw>` | `PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | GROUP BY PromoProgramaPromotorId con contadores por tipo de evento; solo promotores con >= 1 evento; ordenado por conversiones DESC |
| `GetEventosPorDiaAsync` | `IReadOnlyList<PromoEventoPorDiaRaw>` | `PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | GROUP BY CAST(FechaCreacion as DATE) con contadores; solo dias con >= 1 evento |
| `GetMetricasPromotorAsync` | `PromoEventoMetricasPromotorRaw` | `PromotorId promotorId, Guid? programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | KPIs individuales del promotor: misClicks, misConversiones, miValorGenerado |
| `GetEventosRecientesByPromotorAsync` | `IReadOnlyList<PromoEvento>` | `PromotorId promotorId, Guid? programaId, int maxItems, CancellationToken ct` | Ultimos N eventos tipo Backing (TipoEventoId = 4) del promotor; AsNoTracking; ORDER BY FechaCreacion DESC |

**Raw result types** (structs/records para proyeccion de queries agregadas, en el mismo archivo de interface o en un archivo de types del Domain):

```
PromoEventoMetricasRaw:
  TotalClicks        int
  TotalPageViews     int
  TotalSignups       int
  TotalConversiones  int
  ValorTotalGenerado decimal

PromoEventoRankingRaw:
  PromoProgramaPromotorId   Guid
  Clicks                    int
  PageViews                 int
  Signups                   int
  Conversiones              int
  ValorGenerado             decimal

PromoEventoPorDiaRaw:
  Fecha              DateOnly
  Clicks             int
  PageViews          int
  Signups            int
  Conversiones       int

PromoEventoMetricasPromotorRaw:
  MisClicks           int
  MisPageViews        int
  MisSignups          int
  MisConversiones     int
  MiValorGenerado     decimal
```

**Notas de implementacion del repository:**
- `GetMetricasProgramaAsync` y los otros queries de lectura usan `AsNoTracking()`
- El `AddAsync` NO llama `SaveChanges` porque es coordinado por `PromoEventoService` dentro de una transaccion de BD
- Los indices existentes en `PromoEvento` cubren `CodigoReferido` y `FechaCreacion`. La migracion debe agregar indices compuestos para las queries de dashboard (ver Seccion 6)

---

### 3.5 Application Interface: IPromoEventoService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoEventoService.cs`

**Accion:** CREAR (nuevo archivo)

Esta interface vive en la capa **Application** (como el resto de interfaces de servicios en el proyecto) siguiendo el patron establecido.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `ResolverPromotorPorCodigoReferidoAsync` | `PromoProgramaPromotor?` | `string codigoReferido, CancellationToken ct` | Busca PromoProgramaPromotor donde CodigoReferido == codigoReferido AND EsAprobado == true AND EsBloqueado == false AND FechaBaja == null. Retorna null si no existe o codigo invalido |
| `RegistrarEventoAsync` | `Guid` | `PromoEvento evento, CancellationToken ct` | Inserta el PromoEvento; NO calcula comision (para Click, PageView, Signup, Share). Retorna el Id del evento creado |
| `RegistrarConversionAsync` | `RegistrarConversionResult` | `PromoEvento evento, PromoPrograma? programa, PromoProgramaPromotor? inscripcion, CancellationToken ct` | Operacion atomica: inserta PromoEvento tipo Backing + calcula comision + crea PromotorWalletTransaccion + actualiza SaldoPendiente en PromotorWallet. Si programa inactivo o promotor dado de baja: solo inserta evento sin comision |
| `GetMetricasProgramaAsync` | `MetricasProgramaData` | `PromoProgramaId programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | Orquesta las 3 queries del repository (kpis + ranking + porDia). Enriquece el ranking con datos del Promotor (NombrePublico, TipoPromotorNombre). Calcula tasaConversion y comisionesTotales desde WalletTransacciones |
| `GetMetricasPromotorAsync` | `MetricasPromotorData` | `PromotorId promotorId, Guid? programaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct` | Obtiene KPIs individuales y eventos recientes del promotor. Enriquece eventos con monedaNombre. Calcula miTasaConversion |

**Return types** (structs/records para retorno de datos del Service, definidos en el mismo archivo o en Application/Dtos/):

```
RegistrarConversionResult:
  EventoId              Guid
  ComisionCalculada     decimal
  WalletTransaccionId   Guid?
  ComisionAcreditada    bool

MetricasProgramaData:
  ProgramaId            PromoProgramaId
  ProgramaTitulo        string
  FechaDesde            DateTime
  FechaHasta            DateTime
  Kpis                  MetricasProgramaKpisData
  RankingPromotores     IReadOnlyList<RankingPromotorData>
  EventosPorDia         IReadOnlyList<EventoPorDiaData>

MetricasProgramaKpisData:
  TotalClicks           int
  TotalPageViews        int
  TotalSignups          int
  TotalConversiones     int
  ValorTotalGenerado    decimal
  MonedaNombre          string?
  TasaConversion        decimal
  ComisionesTotales     decimal

RankingPromotorData:
  PromotorId            PromotorId
  PromotorNombre        string
  TipoPromotorNombre    string?
  Clicks                int
  PageViews             int
  Signups               int
  Conversiones          int
  ValorGenerado         decimal
  ComisionAcumulada     decimal

EventoPorDiaData:
  Fecha                 DateOnly
  Clicks                int
  PageViews             int
  Signups               int
  Conversiones          int

MetricasPromotorData:
  PromotorId            PromotorId
  PromotorNombre        string
  ProgramaId            PromoProgramaId?
  ProgramaTitulo        string?
  FechaDesde            DateTime
  FechaHasta            DateTime
  Kpis                  MetricasPromotorKpisData
  EventosRecientes      IReadOnlyList<EventoRecienteData>

MetricasPromotorKpisData:
  MisClicks             int
  MisPageViews          int
  MisSignups            int
  MisConversiones       int
  MiValorGenerado       decimal
  MiComisionAcumulada   decimal
  MonedaNombre          string?
  MiTasaConversion      decimal

EventoRecienteData:
  Id                    Guid
  TipoEventoId          int
  TipoEventoNombre      string
  ValorMonetario        decimal
  ComisionGenerada      decimal?
  MonedaNombre          string?
  FechaEvento           DateTime
```

---

### 3.6 Application Interface: ITrackingRateLimitService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/ITrackingRateLimitService.cs`

**Accion:** CREAR (nuevo archivo)

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `IsRateLimitedAsync` | `bool` | `string ipOrigen, string? codigoReferido, CancellationToken ct` | Verifica si la combinacion IP + codigoReferido ya registro un Click en los ultimos 5 minutos. La clave de cache es `ratelimit:click:{ipOrigen}:{codigoReferido ?? "anon"}`. Retorna true si existe en IMemoryCache |
| `RegisterClickAsync` | `Task` | `string ipOrigen, string? codigoReferido, CancellationToken ct` | Agrega la clave al IMemoryCache con TTL de 5 minutos (TimeSpan.FromMinutes(5)). Solo llamar despues de crear el PromoEvento exitosamente |

**Nota de diseno:** Esta interface vive en Application porque es una abstraccion de infraestructura (cache). La implementacion en Infra usa `IMemoryCache`. Esto permite mockear el rate limiting en tests sin tocar la cache real.

---

## 4. Infrastructure Layer

### 4.1 Repository Implementation: PromoEventoRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoEventoRepository.cs`

**Accion:** CREAR (nuevo archivo)

**Implementa:** `IPromoEventoRepository`
**Inyecta:** `CrowdpromotionContext context`

| Metodo | Descripcion de Implementacion |
|--------|-------------------------------|
| `AddAsync` | `await _context.Eventos.AddAsync(entity, ct)`. NO llama SaveChanges. El Service gestiona la transaccion. Retorna `entity.Id` |
| `GetMetricasProgramaAsync` | Query LINQ con `GroupBy` sobre `_context.Eventos.AsNoTracking()` filtrado por `ProgramaId == programaId AND FechaCreacion >= fechaDesde AND FechaCreacion <= fechaHasta`. Proyecta contadores por TipoEventoId usando `.Count(e => e.TipoEventoId == 1)` etc. Retorna `PromoEventoMetricasRaw` |
| `GetRankingPromotoresAsync` | Query LINQ con `Where(e => e.ProgramaId == programaId AND e.PromoProgramaPromotorId != null AND FechaCreacion BETWEEN ...).GroupBy(e => e.PromoProgramaPromotorId).Select(g => new PromoEventoRankingRaw { PromoProgramaPromotorId = g.Key!.Value, Clicks = g.Count(e => e.TipoEventoId == 1), ... }).OrderByDescending(r => r.Conversiones).ToListAsync(ct)` |
| `GetEventosPorDiaAsync` | Query LINQ con `GroupBy(e => e.FechaCreacion.Date)` proyectando contadores por tipo de evento por dia; `OrderBy(g => g.Key)` |
| `GetMetricasPromotorAsync` | Query filtrado por `PromotorId == promotorId` y opcionalmente `ProgramaId == programaId` con rango de fechas; GROUP implícito con `Count` y `Sum` sobre todos los registros del promotor |
| `GetEventosRecientesByPromotorAsync` | `_context.Eventos.AsNoTracking().Where(e => e.PromotorId == promotorId AND e.TipoEventoId == 4 AND (programaId == null OR e.ProgramaId == programaId)).OrderByDescending(e => e.FechaCreacion).Take(maxItems).ToListAsync(ct)` |

**Nota de constructor:**
```
public PromoEventoRepository(CrowdpromotionContext context)
{
    _context = context ?? throw new ArgumentNullException(nameof(context));
}
```

---

### 4.2 Service Implementation: PromoEventoService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromoEventoService.cs`

**Accion:** CREAR (nuevo archivo)

**Implementa:** `IPromoEventoService`

**Dependencias inyectadas:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_eventoRepository` | `IPromoEventoRepository` | Acceso a datos de PromoEvento |
| `_programaPromotorRepository` | `IPromoProgramaPromotorRepository` | Resolver codigoReferido -> PromoProgramaPromotor |
| `_walletRepository` | `IPromotorWalletRepository` | Leer y actualizar PromotorWallet |
| `_requestCache` | `IRequestCacheService` | Cache por request (evitar queries duplicados dentro del mismo request) |
| `_context` | `CrowdpromotionContext` | Transacciones de BD (BeginTransactionAsync) |
| `_logger` | `ILogger<PromoEventoService>` | Logging |

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

**Detalle de implementacion por metodo:**

#### `ResolverPromotorPorCodigoReferidoAsync`
- Clave de cache: `"tracking:ref:{codigoReferido}"`
- Delega a `_programaPromotorRepository` con un metodo nuevo `GetByCodigoReferidoActivoAsync` (ver Seccion 4.5)
- Si retorna null: es flujo FA-01 (codigo invalido); el caller crea el evento sin vinculo a promotor
- Cache con `IRequestCacheService` para que el Validator y el Handler compartan el mismo resultado sin doble query

#### `RegistrarEventoAsync`
- Solo llama `await _eventoRepository.AddAsync(evento, ct)` y `await _context.SaveChangesAsync(ct)`
- Sin transaccion explicita (operacion simple de un solo insert)
- Log: `LogInformation("PromoEvento {EventoId} tipo {TipoEventoId} registrado", evento.Id, evento.TipoEventoId)`

#### `RegistrarConversionAsync`
- Operacion ATOMICA con `await using var transaction = await _context.Database.BeginTransactionAsync(ct)`
- Paso 1: Insertar PromoEvento (tipo Backing con todos los campos de trazabilidad)
- Paso 2: Si `programa != null && programa.EsActivo && inscripcion != null && inscripcion.EsAprobado && !inscripcion.EsBloqueado && !inscripcion.FechaBaja.HasValue`:
  - Calcular comision:
    - Solo porcentaje: `valorMonetario * programa.ImporteComisionPorcentaje / 100`
    - Solo fija: `programa.ImporteComisionFija`
    - Ambas (ambas tienen valor): `MAX(porcentaje, fija)`
    - Si ninguna tiene valor: comision = 0
  - Buscar PromotorWallet del promotor con MonedaId del programa (o MonedaId = 1 como fallback)
  - Si wallet no existe: crear nueva PromotorWallet (flujo FA-08)
  - Crear PromotorWalletTransaccion con EstadoTransaccionId = 1 (Pendiente), Importe = comision, Concepto = "Comision backing referido"
  - Actualizar `wallet.SaldoPendiente += comision` y `wallet.TotalGanado += comision`
  - Retornar `RegistrarConversionResult { ComisionAcreditada = true }`
- Paso 3: Si no aplica comision: retornar `RegistrarConversionResult { ComisionCalculada = 0, ComisionAcreditada = false }`
- Llamar `await _context.SaveChangesAsync(ct)` antes del commit
- `await transaction.CommitAsync(ct)` al final; `RollbackAsync` en el catch
- Log en exito y en error

#### `GetMetricasProgramaAsync`
- Sin transaccion (operacion de solo lectura)
- Llamar en paralelo (o secuencial si la BD no soporta multiple readers en la misma conexion):
  1. `_eventoRepository.GetMetricasProgramaAsync(programaId, fechaDesde, fechaHasta, ct)` -> kpis raw
  2. `_eventoRepository.GetRankingPromotoresAsync(programaId, fechaDesde, fechaHasta, ct)` -> ranking raw
  3. `_eventoRepository.GetEventosPorDiaAsync(programaId, fechaDesde, fechaHasta, ct)` -> serie temporal raw
  4. `_context.WalletTransacciones.AsNoTracking().Where(wt => ...)` para calcular comisionesTotales del periodo (join via PromoEvento para filtrar al programa y periodo)
- Para enriquecer el ranking con NombrePublico y TipoPromotorNombre: obtener los `PromoProgramaPromotorId` del ranking y cargar las inscripciones con `Include(ppp => ppp.Promotor)` en batch
- Calcular `tasaConversion = totalClicks > 0 ? (decimal)totalConversiones / totalClicks * 100 : 0`
- Retornar `MetricasProgramaData` completo

#### `GetMetricasPromotorAsync`
- Obtener metricas raw del repository con filtro por promotorId (y programaId opcional)
- Cargar eventos recientes (solo Backing, max 20) del repository
- Calcular `miTasaConversion = misClicks > 0 ? (decimal)misConversiones / misClicks * 100 : 0`
- Para `miComisionAcumulada`: SUM de WalletTransacciones del promotor en el periodo (query adicional)
- Si `programaId` tiene valor, cargar el titulo del programa via `IRequestCacheService` con `_context.Programas`
- Retornar `MetricasPromotorData` completo

---

### 4.3 Service Implementation: TrackingRateLimitService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/TrackingRateLimitService.cs`

**Accion:** CREAR (nuevo archivo)

**Implementa:** `ITrackingRateLimitService`

**Dependencias inyectadas:**

| Dependencia | Interface | Descripcion |
|-------------|-----------|-------------|
| `_memoryCache` | `IMemoryCache` | Cache en memoria de ASP.NET Core |
| `_logger` | `ILogger<TrackingRateLimitService>` | Logging |

**Constantes internas:**
```
private const int RateLimitWindowMinutes = 5;
private const string CacheKeyPrefix = "ratelimit:click:";
```

**Constructor:** Usar `?? throw new ArgumentNullException` para TODAS las dependencias.

| Metodo | Descripcion de Implementacion |
|--------|-------------------------------|
| `IsRateLimitedAsync` | Construir clave `$"{CacheKeyPrefix}{ipOrigen}:{codigoReferido ?? "anon"}"`. Retornar `_memoryCache.TryGetValue(cacheKey, out _)`. Retorna `true` si la clave existe (rate limited). Retorna `false` si no existe. El metodo es sincrono internamente; firma Task para compatibilidad con la interface |
| `RegisterClickAsync` | Construir misma clave. Llamar `_memoryCache.Set(cacheKey, true, TimeSpan.FromMinutes(RateLimitWindowMinutes))`. Log: `LogDebug("Rate limit registrado para {Ip}:{CodigoReferido}", ipOrigen, codigoReferido)` |

**Nota:** `IMemoryCache` ya esta disponible en el proyecto via `services.AddMemoryCache()` en el startup. No requiere nueva dependencia de NuGet.

**Registro:** `AddSingleton<ITrackingRateLimitService, TrackingRateLimitService>` (Singleton porque IMemoryCache es Singleton y el estado del rate limit es global entre requests)

---

### 4.4 Configuracion EF Core: PromoEventoConfiguration Update

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

**Accion:** MODIFICAR - Actualizar la configuracion del entity `PromoEvento` en `OnModelCreating`

Agregar las siguientes configuraciones dentro del bloque `modelBuilder.Entity<PromoEvento>(entity => { ... })`:

| Propiedad | Configuracion Fluent API |
|-----------|--------------------------|
| `PromoProgramaPromotorId` | `entity.Property(e => e.PromoProgramaPromotorId).HasColumnName("PromoPrograma_Promotor_Id")` |
| `UserIdAfectado` | `entity.Property(e => e.UserIdAfectado).HasMaxLength(450)` |
| `UrlOrigen` | `entity.Property(e => e.UrlOrigen).HasMaxLength(2048)` |
| `UrlReferer` | `entity.Property(e => e.UrlReferer).HasMaxLength(2048)` |
| `UtmSource` | `entity.Property(e => e.UtmSource).HasMaxLength(100)` |
| `UtmMedium` | `entity.Property(e => e.UtmMedium).HasMaxLength(100)` |
| `UtmCampaign` | `entity.Property(e => e.UtmCampaign).HasMaxLength(100)` |

**Indices adicionales a agregar** (para performance de queries de dashboard):

| Indice | Columnas | Nombre | Justificacion |
|--------|----------|--------|---------------|
| Compuesto | `(ProgramaId, FechaCreacion)` | `IX_PromoEvento_Programa_Fecha` | Queries de metricas del artista filtradas por programa y periodo |
| Compuesto | `(PromotorId, TipoEventoId, FechaCreacion)` | `IX_PromoEvento_Promotor_Tipo_Fecha` | Queries de metricas del promotor |
| Columna | `PromoProgramaPromotorId` | `IX_PromoEvento_ProgramaPromotor` | Queries de ranking agrupadas por inscripcion |

**Nota:** Los indices existentes `IX_PromoEvento_CodigoReferido` y `IX_PromoEvento_FechaCreacion` se mantienen.

---

### 4.5 Modificacion: IPromoProgramaPromotorRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromoProgramaPromotorRepository.cs`

**Accion:** MODIFICAR - Agregar un metodo nuevo a la interface existente

| Metodo nuevo | Retorno | Parametros | Descripcion |
|-------------|---------|------------|-------------|
| `GetByCodigoReferidoActivoAsync` | `PromoProgramaPromotor?` | `string codigoReferido, CancellationToken ct` | Busca PromoProgramaPromotor donde `CodigoReferido == codigoReferido AND EsAprobado == true AND EsBloqueado == false AND FechaBaja == null`. Include de Promotor y Programa. Usado por PromoEventoService para resolver el codigo referido al registrar eventos |

**Archivo de implementacion a modificar:**
`src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromoProgramaPromotorRepository.cs`

Agregar la implementacion del nuevo metodo con la query LINQ correspondiente usando `AsNoTracking()`, `Include(ppp => ppp.Promotor)`, `Include(ppp => ppp.Programa)`.

---

### 4.6 Dependency Injection

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

**Accion:** MODIFICAR - Agregar los nuevos registros al metodo `AddCrowdpromotionServices`

**Registros a agregar:**

```
// Repositorios nuevos
services.AddScoped<IPromoEventoRepository, PromoEventoRepository>();

// Servicios nuevos
services.AddScoped<IPromoEventoService, PromoEventoService>();
services.AddSingleton<ITrackingRateLimitService, TrackingRateLimitService>();
```

**Nota sobre Singleton:** `TrackingRateLimitService` se registra como Singleton porque depende de `IMemoryCache` que es Singleton. Un Scoped service que depende de un Singleton es correcto en este sentido (el Singleton puede tener dependencias Singleton).

**Prerequisito:** Verificar que `services.AddMemoryCache()` esta llamado en el startup de la WebApi (normalmente en `Program.cs` o en el modulo de infraestructura).

---

## 5. Migraciones EF Core

**Migracion a crear:** `AddPromoEventoTrackingFields`

**Cambios incluidos:**
- Agregar columnas nuevas a tabla `PromoEvento`:
  - `PromoPrograma_Promotor_Id` (uniqueidentifier, nullable)
  - `UserIdAfectado` (nvarchar(450), nullable)
  - `UrlOrigen` (nvarchar(2048), nullable)
  - `UrlReferer` (nvarchar(2048), nullable)
  - `UtmSource` (nvarchar(100), nullable)
  - `UtmMedium` (nvarchar(100), nullable)
  - `UtmCampaign` (nvarchar(100), nullable)
- Agregar indices:
  - `IX_PromoEvento_Programa_Fecha` en `(Programa_Id, FechaCreacion)`
  - `IX_PromoEvento_Promotor_Tipo_Fecha` en `(Promotor_Id, TipoEvento_Id, FechaCreacion)`
  - `IX_PromoEvento_ProgramaPromotor` en `PromoPrograma_Promotor_Id`
- Sin FK constraint para `PromoPrograma_Promotor_Id` (para evitar dependencias circulares en inserts y mantener trazabilidad aun si la inscripcion es eliminada)

**Comando:**
```bash
dotnet ef migrations add AddPromoEventoTrackingFields --project src/api/WebApi --context CrowdpromotionContext
dotnet ef database update --project src/api/WebApi
```

---

## 6. Archivos a Crear o Modificar

```
src/api/Modules/Crowdpromotion/
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Model/
│   │   └── PromoEvento.cs                          MODIFICAR: agregar 7 campos nuevos
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs           MODIFICAR: agregar 5 nuevas constantes
│   └── Interfaces/
│       ├── IPromoEventoRepository.cs               CREAR: interface con 6 metodos
│       └── IPromoProgramaPromotorRepository.cs     MODIFICAR: agregar GetByCodigoReferidoActivoAsync
│
├── WePlayRises.Crowdpromotion.Application/
│   └── Interfaces/
│       └── Services/
│           ├── IPromoEventoService.cs              CREAR: interface con 5 metodos + return types
│           └── ITrackingRateLimitService.cs        CREAR: interface con 2 metodos
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Repositories/
    │   ├── PromoEventoRepository.cs                CREAR: implementacion con 6 metodos
    │   └── PromoProgramaPromotorRepository.cs      MODIFICAR: agregar GetByCodigoReferidoActivoAsync
    ├── Services/
    │   ├── PromoEventoService.cs                   CREAR: implementacion con 5 metodos
    │   └── TrackingRateLimitService.cs             CREAR: implementacion con IMemoryCache
    ├── Context/
    │   └── CrowdpromotionContext.cs                MODIFICAR: configuracion PromoEvento + indices
    └── DependencyInjection.cs                      MODIFICAR: registrar 2 services + 1 repository
```

---

## 7. Relaciones Entre Capas

```
Handler (Application)
   |
   +--> IPromoEventoService (Application Interface)
   |         |
   |         +--> PromoEventoService (Infra)
   |                   |
   |                   +--> IPromoEventoRepository -> PromoEventoRepository -> CrowdpromotionContext
   |                   +--> IPromoProgramaPromotorRepository -> PromoProgramaPromotorRepository
   |                   +--> IPromotorWalletRepository -> PromotorWalletRepository
   |                   +--> IRequestCacheService (BuildingBlocks)
   |                   +--> CrowdpromotionContext (transacciones directas)
   |                   +--> ILogger<PromoEventoService>
   |
   +--> ITrackingRateLimitService (Application Interface)
             |
             +--> TrackingRateLimitService (Infra)
                       |
                       +--> IMemoryCache (ASP.NET Core)
                       +--> ILogger<TrackingRateLimitService>
```

---

## 8. Decisiones de Diseno

### 8.1 PromoEventoRepository.AddAsync no llama SaveChanges
El `AddAsync` del repository de PromoEvento NO llama `SaveChangesAsync` porque `RegistrarConversionAsync` necesita coordinar tres operaciones (insert evento + insert walletTransaccion + update wallet) en una sola transaccion de BD. El Service controla el SaveChanges dentro del scope de la transaccion.

Para `RegistrarEventoAsync` (eventos simples, sin transaccion multietapa), el Service llama `_context.SaveChangesAsync` directamente despues del AddAsync del repository. Este patron es consistente con `PromotorService.CreateWithWalletAsync` ya existente.

### 8.2 TrackingRateLimitService como Singleton
Se registra como Singleton (no Scoped) porque el estado del rate limiting debe persistir entre requests HTTP. Si fuera Scoped, cada nuevo request HTTP obtendria una instancia nueva del service y el cache de rate limiting se perderia. La dependencia en `IMemoryCache` (que es Singleton) refuerza este patron.

### 8.3 PromoProgramaPromotorId como Guid? sin FK constraint en BD
La columna `PromoPrograma_Promotor_Id` en `PromoEvento` se almacena como `uniqueidentifier nullable` pero **sin FK constraint de BD**. Razon: PromoEvento es inmutable (nunca se actualiza ni elimina) y necesita mantener la trazabilidad incluso si la inscripcion `PromoProgramaPromotor` es eliminada o modificada. La integridad referencial se garantiza a nivel de aplicacion en el Service.

### 8.4 Queries de Dashboard sin IRequestCacheService
Los endpoints de dashboard (GET metricas artista, GET metricas promotor) NO usan `IRequestCacheService` para los resultados de las queries agregadas, en linea con RNF-07: "Los dashboards deben mostrar datos actualizados; no se usa cache de consulta en MVP". El `IRequestCacheService` SI se usa dentro del Service para los sub-lookups de entidades maestras (promotor, programa, moneda) dentro del mismo request.

### 8.5 ComisionesTotales desde WalletTransacciones, no de PromoEvento
El calculo de `comisionesTotales` para el dashboard del artista se obtiene sumando los importes de `PromotorWalletTransaccion` vinculadas al programa en el periodo, no calculando desde `PromoEvento.ImporteAsociado`. Esto garantiza que el importe reflejado es el efectivamente acreditado, no el valor del backing (que puede ser mayor).

La vinculacion se realiza via `PromoEvento` como tabla de join: `WalletTransaccion.ReferenciaExterna = evento.Id.ToString()` o alternativamente mediante una query que correlaciona `PromoProgramaPromotorId` del evento con el `WalletId` del promotor. El implementador debe definir el mecanismo exacto al implementar el Service. La opcion recomendada es guardar el `PromoEventoId` como `ReferenciaExterna` en `PromotorWalletTransaccion` al crear la transaccion de comision.

---

## 9. Checklist

- [ ] `PromoEvento.cs` actualizado con 7 nuevos campos (PromoProgramaPromotorId, UserIdAfectado, UrlOrigen, UrlReferer, UtmSource, UtmMedium, UtmCampaign)
- [ ] `ServiceResponseMessageType.cs` actualizado con 5 nuevas constantes (1033, 1034, 1035, 1036, 4032)
- [ ] `IPromoEventoRepository.cs` creado con 6 metodos y los 5 raw result types definidos
- [ ] `IPromoProgramaPromotorRepository.cs` actualizado con `GetByCodigoReferidoActivoAsync`
- [ ] `IPromoEventoService.cs` creado en Application/Interfaces/Services con 5 metodos y los return types definidos
- [ ] `ITrackingRateLimitService.cs` creado en Application/Interfaces/Services con 2 metodos
- [ ] `PromoEventoRepository.cs` creado con queries AsNoTracking para lecturas y AddAsync sin SaveChanges
- [ ] `PromoProgramaPromotorRepository.cs` actualizado con la implementacion de `GetByCodigoReferidoActivoAsync`
- [ ] `PromoEventoService.cs` creado con transaccion atomica en `RegistrarConversionAsync`; inyecta `?? throw` en constructor
- [ ] `TrackingRateLimitService.cs` creado usando `IMemoryCache` con TTL de 5 minutos
- [ ] `CrowdpromotionContext.cs` actualizado con configuracion de 7 campos nuevos y 3 indices compuestos
- [ ] `DependencyInjection.cs` actualizado con `IPromoEventoRepository`, `IPromoEventoService` (Scoped) y `ITrackingRateLimitService` (Singleton)
- [ ] Migracion `AddPromoEventoTrackingFields` creada y aplicada
- [ ] Verificar que `services.AddMemoryCache()` esta en el startup de WebApi
- [ ] Entidades son POCOs sin logica de negocio
- [ ] Services retornan entidades y data objects, NO DTOs
- [ ] Handlers en Application layer inyectan Services (no DbContext, no Repository)
