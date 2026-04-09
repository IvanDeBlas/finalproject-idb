# Contratos: Tracking de Eventos, Conversiones y Dashboard de Metricas

> **Feature:** cp-tracking-metricas (US-CP-05)
> **Ultima actualizacion:** 2026-03-02
> **Modulo Backend:** Crowdpromotion
> **Depende de:** US-CP-03 (cp-inscripcion-programa)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Analisis del Modelo de Dominio

### PromoEvento - Entidad Principal de Tracking

La entidad `PromoEvento` registra cada evento de tracking generado por la actividad de un promotor. Cada registro es inmutable una vez creado (no hay actualizaciones, solo nuevos eventos).

```
PromoEvento
  Id                          Guid
  PromoPrograma_Id            Guid?           FK a PromoPrograma (null si codigo invalido)
  PromoPrograma_Promotor_Id   Guid?           FK a PromoProgramaPromotor (null si codigo invalido)
  TipoEventoPromo_Id          int             FK a Maestra_TipoEventoPromo
  CampaniaCrowdfunding_Id     Guid?           FK a CampaniaCrowdfunding (si aplica)
  AportacionCrowdfunding_Id   Guid?           FK a AportacionCrowdfunding (solo tipo Backing)
  UserIdAfectado              string?         UserId del usuario que realizo la accion
  UrlOrigen                   string?         URL desde donde llego el click
  UrlReferer                  string?         HTTP Referer header
  UtmSource                   string?         UTM source (siempre "weplay")
  UtmMedium                   string?         UTM medium ("referral")
  UtmCampaign                 string?         UTM campaign (CodigoTrackingBase del programa)
  ValorMonetario              decimal?        Importe del backing (solo tipo Backing)
  Moneda_Id                   int?            FK a MaestraMoneda (solo tipo Backing)
  FechaEvento                 DateTime        Timestamp del evento (UTC)
```

### Maestra_TipoEventoPromo - Tipos de Evento

```
1 = Click      Fan hace click en URL del promotor
2 = PageView   Fan visita la pagina de la campana
3 = Signup     Fan se registra en la plataforma
4 = Backing    Fan hace un backing/aportacion (conversion)
5 = Share      Promotor comparte via boton de la app
```

### Resolucion de Promotor por CodigoReferido

Al recibir un evento con `codigoReferido`:
1. Buscar `PromoProgramaPromotor` donde `CodigoReferido == codigoReferido` y `EsAprobado == true` y `EsBloqueado == false` y `FechaBaja == null`
2. Si se encuentra: vincular el evento a `PromoPrograma_Id` y `PromoPrograma_Promotor_Id`
3. Si no se encuentra (codigo invalido, promotor inactivo, etc.): registrar el evento con `PromoPrograma_Id = null` y `PromoPrograma_Promotor_Id = null` (flujo alternativo FA-01)

### PromotorWallet y PromotorWalletTransaccion - Comisiones

Al registrar un evento tipo Backing con promotor valido y programa activo:
- El sistema calcula la comision segun el tipo del programa (porcentaje, fija, o el maximo de ambas)
- Crea un `PromotorWalletTransaccion` de tipo Credito con estado Pendiente
- Actualiza `PromotorWallet.SaldoActual += comision`

### Calculo de Comision

| Tipo programa | Formula |
|---------------|---------|
| Solo porcentaje | `ValorMonetario * ImporteComisionPorcentaje / 100` |
| Solo fija | `ImporteComisionFija` |
| Ambas | `MAX(ValorMonetario * ImporteComisionPorcentaje / 100, ImporteComisionFija)` |

Si el programa esta inactivo o el promotor fue dado de baja: registrar el evento pero NO calcular ni acreditar comision.

### Rate Limiting para Eventos tipo Click

Para prevenir duplicados (FA-04):
- Clave: `IP + codigoReferido` (o solo IP si no hay codigo)
- TTL: 5 minutos
- Implementacion: cache en memoria (IMemoryCache) o Redis
- Si la clave existe en cache: devolver HTTP 429 con error `4032`
- Si no existe: registrar evento y guardar clave en cache con TTL de 5 minutos

---

## Endpoints API

### POST /api/crowdpromotion/tracking/evento

**Descripcion:** Registra un evento de tracking generado por la actividad de un visitante o fan. Llamado por el frontend al detectar el parametro `ref` en la URL o al navegar a la pagina de una campana. El endpoint es publico (permite requests anonimos). Si el `codigoReferido` no existe o esta inactivo, registra el evento sin vinculo a promotor. Para eventos tipo Click aplica rate limiting de 5 minutos por IP + codigo referido.

**Autorizacion:** Publica (sin token requerido). El `UserIdAfectado` se puede incluir si el usuario esta autenticado.

**Request Body:**
```json
{
  "codigoReferido": "album-2026-x7k9m",
  "tipoEventoPromoId": 1,
  "campaniaCrowdfundingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "urlOrigen": "https://instagram.com/stories/xxx",
  "urlReferer": "https://instagram.com",
  "utmSource": "weplay",
  "utmMedium": "referral",
  "utmCampaign": "album-2026"
}
```

| Campo | Tipo | Obligatorio | Constraints |
|-------|------|-------------|-------------|
| `codigoReferido` | string | No | max 50 caracteres |
| `tipoEventoPromoId` | int | Si | 1=Click, 2=PageView, 3=Signup, 4=Backing, 5=Share |
| `campaniaCrowdfundingId` | string (Guid) | No | Guid valido si se proporciona |
| `urlOrigen` | string | No | max 2048 caracteres |
| `urlReferer` | string | No | max 2048 caracteres |
| `utmSource` | string | No | max 100 caracteres |
| `utmMedium` | string | No | max 100 caracteres |
| `utmCampaign` | string | No | max 100 caracteres |

**Response 201 Created:**
```json
{
  "data": {
    "eventoId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "registrado": true
  },
  "messages": [
    { "message": "Evento registrado", "errorCode": "0001" }
  ]
}
```

**Notas:**
- `eventoId` se devuelve siempre que el evento sea insertado en BD, independientemente de si el codigo referido es valido.
- Si el tipo de evento es Backing (4), este endpoint NO debe usarse desde el frontend. Usar el endpoint interno `/tracking/conversion`.
- El campo `userIdAfectado` NO se envia en el body. Se resuelve desde el token JWT si existe; si el request es anonimo queda `null`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El tipo de evento es obligatorio | `tipoEventoPromoId` no enviado o cero |
| 400 | 1033 | Tipo de evento invalido | `tipoEventoPromoId` no esta entre los valores 1-5 |
| 400 | 1034 | El tipo de evento Backing no se acepta en este endpoint | `tipoEventoPromoId == 4` (usar /tracking/conversion) |
| 400 | 1002 | El codigo referido no puede superar los 50 caracteres | `codigoReferido.Length > 50` |
| 429 | 4032 | Demasiados clicks en poco tiempo. Intenta de nuevo en unos minutos | Rate limit excedido (IP + codigoReferido, ventana 5 min) |
| 500 | 5000 | Error inesperado al registrar el evento | Excepcion no controlada |

**Nota sobre el error 429:** Solo aplica para eventos tipo Click (`tipoEventoPromoId == 1`). El rate limiting no aplica para PageView, Signup, Share.

---

### POST /api/crowdpromotion/tracking/conversion

**Descripcion:** Registra una conversion (backing referido) y calcula la comision del promotor. Llamado internamente por el handler de backing del modulo Crowdfunding al procesar un aportacion exitosa que tiene un codigo referido asociado en la sesion del usuario. Este endpoint esta protegido para uso interno del sistema.

**Autorizacion:** Bearer JWT - Solo llamado internamente entre modulos (el handler de backing pasa su propio token de sistema, o se implementa como llamada directa al Service del modulo Crowdpromotion en lugar de HTTP).

**Nota de implementacion:** En MVP, en lugar de HTTP entre modulos, el handler de backing inyectara directamente `IPromoEventoService` del modulo Crowdpromotion. Este contrato documenta el payload y response esperados como si fuera HTTP para claridad.

**Request Body:**
```json
{
  "codigoReferido": "album-2026-x7k9m",
  "campaniaCrowdfundingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "aportacionCrowdfundingId": "7d89e123-4567-8901-b234-c567d890e123",
  "valorMonetario": 50.00,
  "monedaId": 1,
  "userIdAfectado": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890"
}
```

| Campo | Tipo | Obligatorio | Constraints |
|-------|------|-------------|-------------|
| `codigoReferido` | string | Si | max 50 caracteres |
| `campaniaCrowdfundingId` | string (Guid) | Si | Guid valido |
| `aportacionCrowdfundingId` | string (Guid) | Si | Guid valido |
| `valorMonetario` | decimal | Si | mayor que 0 |
| `monedaId` | int | Si | mayor que 0 |
| `userIdAfectado` | string | Si | UserId del fan que hizo el backing |

**Response 201 Created:**
```json
{
  "data": {
    "eventoId": "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
    "comisionCalculada": 5.00,
    "monedaNombre": "EUR",
    "walletTransaccionId": "d4e5f6a7-b8c9-0123-de45-fa6789012345",
    "comisionAcreditada": true
  },
  "messages": [
    { "message": "Conversion registrada y comision acreditada", "errorCode": "0001" }
  ]
}
```

**Notas sobre `comisionCalculada` y `comisionAcreditada`:**
- Si el programa esta inactivo: `comisionCalculada = 0`, `comisionAcreditada = false`, `walletTransaccionId = null`
- Si el promotor fue dado de baja: idem sin acreditacion
- Si el codigo referido no existe: evento se registra sin promotor, `comisionCalculada = 0`, `comisionAcreditada = false`
- Solo cuando `comisionAcreditada = true` se crea el `PromotorWalletTransaccion`

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El codigo referido es obligatorio | `codigoReferido` vacio o null |
| 400 | 1002 | El codigo referido no puede superar los 50 caracteres | `codigoReferido.Length > 50` |
| 400 | 1001 | La aportacion es obligatoria | `aportacionCrowdfundingId` vacio |
| 400 | 1035 | El valor monetario debe ser mayor que cero | `valorMonetario <= 0` |
| 500 | 5000 | Error inesperado al registrar la conversion | Excepcion no controlada |

---

### GET /api/crowdpromotion/programas/{programaId}/metricas

**Descripcion:** Dashboard de metricas del programa para el artista propietario. Devuelve KPIs agregados, ranking de promotores ordenado por conversiones descendente y serie temporal de eventos por dia. Soporta filtrado por rango de fechas. El `ArtistaId` se resuelve desde el token JWT y se verifica contra `PromoPrograma.ArtistaId`. Los datos se calculan mediante queries agregadas (GROUP BY) sobre `PromoEvento`.

**Autorizacion:** Bearer JWT - Artista propietario del programa (`PromoPrograma.ArtistaId == ArtistaId del token`)

**Path Params:** `programaId` (Guid) - Identificador del programa de promocion

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `fechaDesde` | string (date) | No | Fecha inicio del periodo. Formato: `YYYY-MM-DD`. Sin valor: inicio del programa o 90 dias atras |
| `fechaHasta` | string (date) | No | Fecha fin del periodo. Formato: `YYYY-MM-DD`. Sin valor: hoy |

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "programaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "programaTitulo": "Promociona mi nuevo album",
    "fechaDesde": "2026-03-01",
    "fechaHasta": "2026-06-01",
    "kpis": {
      "totalClicks": 1250,
      "totalPageViews": 890,
      "totalSignups": 45,
      "totalConversiones": 12,
      "valorTotalGenerado": 1200.00,
      "monedaNombre": "EUR",
      "tasaConversion": 0.96,
      "comisionesTotales": 120.00
    },
    "rankingPromotores": [
      {
        "promotorId": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
        "promotorNombre": "DJ Marketing Pro",
        "tipoPromotorNombre": "Influencer",
        "clicks": 450,
        "pageViews": 300,
        "signups": 15,
        "conversiones": 5,
        "valorGenerado": 500.00,
        "comisionAcumulada": 50.00
      },
      {
        "promotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
        "promotorNombre": "MusicBlog.es",
        "tipoPromotorNombre": "Medio / Blog",
        "clicks": 300,
        "pageViews": 200,
        "signups": 10,
        "conversiones": 4,
        "valorGenerado": 400.00,
        "comisionAcumulada": 40.00
      }
    ],
    "eventosPorDia": [
      {
        "fecha": "2026-03-15",
        "clicks": 45,
        "pageViews": 30,
        "signups": 2,
        "conversiones": 1
      },
      {
        "fecha": "2026-03-16",
        "clicks": 60,
        "pageViews": 42,
        "signups": 3,
        "conversiones": 0
      }
    ]
  },
  "messages": []
}
```

**Notas sobre los calculos:**
- `totalClicks`: COUNT de PromoEvento donde `TipoEventoPromoId = 1` y `PromoPrograma_Id = programaId` y `FechaEvento BETWEEN fechaDesde AND fechaHasta`
- `totalPageViews`: idem con `TipoEventoPromoId = 2`
- `totalSignups`: idem con `TipoEventoPromoId = 3`
- `totalConversiones`: idem con `TipoEventoPromoId = 4`
- `valorTotalGenerado`: SUM(`ValorMonetario`) donde `TipoEventoPromoId = 4` en el mismo filtro
- `tasaConversion`: `totalConversiones / totalClicks * 100`. Devuelve `0` si `totalClicks = 0`
- `comisionesTotales`: SUM de importes de `PromotorWalletTransaccion` vinculadas a conversiones del programa en el periodo
- `monedaNombre`: Resuelto desde la moneda del programa (`PromoPrograma.MonedaId`). Null si no hay conversiones
- `rankingPromotores`: Solo promotores con al menos 1 evento en el periodo, ordenados por `conversiones DESC`
- `eventosPorDia`: Un item por dia que tenga al menos 1 evento en el periodo. Sin ceros intermedios

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1036 | La fecha de inicio no puede ser posterior a la fecha fin | `fechaDesde > fechaHasta` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para ver las metricas de este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 500 | 5000 | Error inesperado al obtener las metricas | Excepcion no controlada |

---

### GET /api/crowdpromotion/promotor/metricas

**Descripcion:** Dashboard de metricas individuales del promotor autenticado. Devuelve KPIs propios y el historial de eventos recientes (solo tipo Backing, que tienen valor monetario). El `PromotorId` se resuelve desde el token JWT via `UserId`. Permite filtrar por programa y rango de fechas. Si no se proporciona `programaId`, devuelve metricas agregadas de todos los programas del promotor.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `programaId` | string (Guid) | No | Filtrar por programa especifico. Sin valor: todos los programas |
| `fechaDesde` | string (date) | No | Fecha inicio del periodo. Formato: `YYYY-MM-DD`. Sin valor: 90 dias atras |
| `fechaHasta` | string (date) | No | Fecha fin del periodo. Formato: `YYYY-MM-DD`. Sin valor: hoy |

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "promotorId": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    "promotorNombre": "DJ Marketing Pro",
    "programaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "programaTitulo": "Promociona mi nuevo album",
    "fechaDesde": "2026-03-01",
    "fechaHasta": "2026-06-01",
    "kpis": {
      "misClicks": 450,
      "misPageViews": 300,
      "misSignups": 15,
      "misConversiones": 5,
      "miValorGenerado": 500.00,
      "miComisionAcumulada": 50.00,
      "monedaNombre": "EUR",
      "miTasaConversion": 1.11
    },
    "eventosRecientes": [
      {
        "id": "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
        "tipoEventoId": 4,
        "tipoEventoNombre": "Backing",
        "valorMonetario": 100.00,
        "comisionGenerada": 10.00,
        "monedaNombre": "EUR",
        "fechaEvento": "2026-03-20T14:30:00Z"
      },
      {
        "id": "d4e5f6a7-b8c9-0123-de45-fa6789012345",
        "tipoEventoId": 4,
        "tipoEventoNombre": "Backing",
        "valorMonetario": 50.00,
        "comisionGenerada": 5.00,
        "monedaNombre": "EUR",
        "fechaEvento": "2026-03-18T09:15:00Z"
      }
    ]
  },
  "messages": []
}
```

**Notas:**
- Si `programaId` es null (sin filtro): `programaId` y `programaTitulo` en la respuesta seran `null`, y las metricas son la suma de todos los programas del promotor
- `misClicks`: COUNT de PromoEvento donde `PromoPrograma_Promotor_Id` corresponde al promotor, `TipoEventoPromoId = 1`, y `FechaEvento` en el periodo
- `miTasaConversion`: `misConversiones / misClicks * 100`. Devuelve `0` si `misClicks = 0`
- `miComisionAcumulada`: SUM de `PromotorWalletTransaccion.Importe` vinculadas al promotor en el periodo
- `eventosRecientes`: Solo eventos tipo Backing (tienen `ValorMonetario`). Los ultimos 20 del periodo filtrado, ordenados por `FechaEvento DESC`
- `comisionGenerada`: Calculado a posteriori desde `PromotorWalletTransaccion` vinculada al evento. Puede ser `null` si no se genero comision (programa inactivo, promotor dado de baja en ese momento)

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1036 | La fecha de inicio no puede ser posterior a la fecha fin | `fechaDesde > fechaHasta` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | 5000 | Error inesperado al obtener tus metricas | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Actor | Verificacion |
|----------|------|-------|--------------|
| `POST /api/crowdpromotion/tracking/evento` | Publica | Anonimo / Fan | Sin verificacion. UserIdAfectado desde token opcional |
| `POST /api/crowdpromotion/tracking/conversion` | Interno | Sistema | Llamada interna entre modulos, no expuesto al frontend |
| `GET /api/crowdpromotion/programas/{id}/metricas` | Bearer JWT | Artista propietario | `PromoPrograma.ArtistaId == ArtistaId del token` |
| `GET /api/crowdpromotion/promotor/metricas` | Bearer JWT | Promotor | `PromotorId` resuelto por `UserId` del token |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "usuario@example.com",
  "role": "Fan",
  "exp": 1740873600,
  "iat": 1740787200
}
```

**Nota:** Ni el `PromotorId` ni el `ArtistaId` se incluyen en el token. El backend resuelve cada entidad consultando por `UserId` (claim `sub`) al inicio de cada handler. Usar `IRequestCacheService` para cachear estas resoluciones dentro del mismo request.

**Resolucion de identidad en backend:**
- **Artista (metricas del programa):** `UserId` del claim `sub` -> buscar `Artista` por `UserId` -> obtener `ArtistaId` -> verificar `PromoPrograma.ArtistaId == artista.Id`. Si no coincide: HTTP 403, ErrorCode `4026`.
- **Promotor (metricas del promotor):** `UserId` del claim `sub` -> buscar `Promotor` por `UserId` -> obtener `PromotorId` -> filtrar `PromoEvento` por inscripciones del promotor.

### Proteccion de Rutas Frontend

| Ruta | App | Auth | Actor | Redirect si no auth |
|------|-----|------|-------|---------------------|
| `/admin/programas/:id/metricas` | Admin | Bearer JWT | Artista | `/auth/login` |
| `/promotor/metricas` | Landing | Bearer JWT | Promotor | `/auth/login` |

**Nota:** El tracking de eventos (`POST /tracking/evento`) se llama desde el frontend sin autenticacion. No requiere ruta protegida; el frontend lo invoca como side-effect al detectar el parametro `ref` en la URL.

---

## DTOs / Types

### Backend (C#)

#### RegistrarEventoDto (Request)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarEventoDto.cs

/// <summary>
/// Payload para registrar un evento de tracking (endpoint publico).
/// </summary>
public class RegistrarEventoDto
{
    /// <summary>Codigo referido del promotor. Opcional. Max 50 caracteres.</summary>
    public string? CodigoReferido { get; set; }

    /// <summary>Tipo de evento. Obligatorio. 1=Click, 2=PageView, 3=Signup, 4=Backing, 5=Share.</summary>
    public int TipoEventoPromoId { get; set; }

    /// <summary>FK campana. Opcional.</summary>
    public Guid? CampaniaCrowdfundingId { get; set; }

    /// <summary>URL desde donde llego el click. Max 2048 caracteres.</summary>
    public string? UrlOrigen { get; set; }

    /// <summary>HTTP Referer. Max 2048 caracteres.</summary>
    public string? UrlReferer { get; set; }

    /// <summary>UTM source. Max 100 caracteres.</summary>
    public string? UtmSource { get; set; }

    /// <summary>UTM medium. Max 100 caracteres.</summary>
    public string? UtmMedium { get; set; }

    /// <summary>UTM campaign. Max 100 caracteres.</summary>
    public string? UtmCampaign { get; set; }
}
```

#### RegistrarEventoResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarEventoResponseDto.cs

public class RegistrarEventoResponseDto
{
    public Guid EventoId { get; set; }

    public bool Registrado { get; set; }
}
```

#### RegistrarConversionDto (Request)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarConversionDto.cs

/// <summary>
/// Payload para registrar una conversion (backing referido). Uso interno entre modulos.
/// </summary>
public class RegistrarConversionDto
{
    /// <summary>Codigo referido del promotor. Obligatorio.</summary>
    public string CodigoReferido { get; set; } = null!;

    /// <summary>FK campana crowdfunding. Obligatorio.</summary>
    public Guid CampaniaCrowdfundingId { get; set; }

    /// <summary>FK aportacion crowdfunding. Obligatorio.</summary>
    public Guid AportacionCrowdfundingId { get; set; }

    /// <summary>Importe del backing. Obligatorio. Mayor que 0.</summary>
    public decimal ValorMonetario { get; set; }

    /// <summary>FK moneda. Obligatorio.</summary>
    public int MonedaId { get; set; }

    /// <summary>UserId del fan que hizo el backing.</summary>
    public string UserIdAfectado { get; set; } = null!;
}
```

#### RegistrarConversionResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RegistrarConversionResponseDto.cs

public class RegistrarConversionResponseDto
{
    public Guid EventoId { get; set; }

    /// <summary>
    /// Comision calculada y acreditada al promotor.
    /// Cero si el programa estaba inactivo, el promotor fue dado de baja, o el codigo es invalido.
    /// </summary>
    public decimal ComisionCalculada { get; set; }

    /// <summary>Null si no se acredito comision.</summary>
    public string? MonedaNombre { get; set; }

    /// <summary>
    /// Id de la transaccion de wallet creada.
    /// Null si no se genero comision (ComisionAcreditada = false).
    /// </summary>
    public Guid? WalletTransaccionId { get; set; }

    /// <summary>True solo cuando se creo PromotorWalletTransaccion exitosamente.</summary>
    public bool ComisionAcreditada { get; set; }
}
```

#### ProgramaMetricasKpisDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaMetricasKpisDto.cs

/// <summary>
/// KPIs agregados del programa (vista artista). Todos los contadores son COUNT/SUM de PromoEvento.
/// </summary>
public class ProgramaMetricasKpisDto
{
    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 1</summary>
    public int TotalClicks { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 2</summary>
    public int TotalPageViews { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 3</summary>
    public int TotalSignups { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 4</summary>
    public int TotalConversiones { get; set; }

    /// <summary>SUM(ValorMonetario) WHERE TipoEventoPromoId = 4</summary>
    public decimal ValorTotalGenerado { get; set; }

    /// <summary>Resuelto desde PromoPrograma.MonedaId. Null si no hay conversiones con moneda.</summary>
    public string? MonedaNombre { get; set; }

    /// <summary>TotalConversiones / TotalClicks * 100. Cero si TotalClicks = 0. Redondeado a 2 decimales.</summary>
    public decimal TasaConversion { get; set; }

    /// <summary>SUM de PromotorWalletTransaccion vinculadas a conversiones del programa en el periodo.</summary>
    public decimal ComisionesTotales { get; set; }
}
```

#### RankingPromotorItemDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RankingPromotorItemDto.cs

/// <summary>
/// Item del ranking de promotores en el dashboard del artista.
/// Calculado via GROUP BY PromoProgramaPromotor_Id sobre PromoEvento.
/// </summary>
public class RankingPromotorItemDto
{
    public Guid PromotorId { get; set; }

    /// <summary>Promotor.NombrePublico</summary>
    public string PromotorNombre { get; set; } = null!;

    /// <summary>Resuelto via MaestraTipoPromotor</summary>
    public string? TipoPromotorNombre { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 1 para este promotor en el periodo</summary>
    public int Clicks { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 2 para este promotor</summary>
    public int PageViews { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 3 para este promotor</summary>
    public int Signups { get; set; }

    /// <summary>COUNT PromoEvento WHERE TipoEventoPromoId = 4 para este promotor</summary>
    public int Conversiones { get; set; }

    /// <summary>SUM(ValorMonetario) WHERE TipoEventoPromoId = 4 para este promotor</summary>
    public decimal ValorGenerado { get; set; }

    /// <summary>SUM de PromotorWalletTransaccion vinculadas a este promotor en el programa y periodo</summary>
    public decimal ComisionAcumulada { get; set; }
}
```

#### EventosPorDiaItemDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/EventosPorDiaItemDto.cs

/// <summary>
/// Serie temporal de eventos agrupados por dia (grafico del dashboard artista).
/// Solo dias con al menos 1 evento; no incluye ceros intermedios.
/// </summary>
public class EventosPorDiaItemDto
{
    /// <summary>Fecha en formato YYYY-MM-DD</summary>
    public string Fecha { get; set; } = null!;

    public int Clicks { get; set; }

    public int PageViews { get; set; }

    public int Signups { get; set; }

    public int Conversiones { get; set; }
}
```

#### ProgramaMetricasResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaMetricasResponseDto.cs

public class ProgramaMetricasResponseDto
{
    public Guid ProgramaId { get; set; }

    public string ProgramaTitulo { get; set; } = null!;

    /// <summary>Fecha inicio del periodo aplicado. Formato YYYY-MM-DD.</summary>
    public string FechaDesde { get; set; } = null!;

    /// <summary>Fecha fin del periodo aplicado. Formato YYYY-MM-DD.</summary>
    public string FechaHasta { get; set; } = null!;

    public ProgramaMetricasKpisDto Kpis { get; set; } = null!;

    /// <summary>Ordenado por Conversiones DESC. Solo promotores con al menos 1 evento en el periodo.</summary>
    public IReadOnlyList<RankingPromotorItemDto> RankingPromotores { get; set; } = new List<RankingPromotorItemDto>();

    /// <summary>Serie temporal de eventos. Solo dias con actividad.</summary>
    public IReadOnlyList<EventosPorDiaItemDto> EventosPorDia { get; set; } = new List<EventosPorDiaItemDto>();
}
```

#### PromotorMetricasKpisDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorMetricasKpisDto.cs

/// <summary>
/// KPIs del promotor autenticado. Filtrados por su PromotorId.
/// </summary>
public class PromotorMetricasKpisDto
{
    public int MisClicks { get; set; }

    public int MisPageViews { get; set; }

    public int MisSignups { get; set; }

    public int MisConversiones { get; set; }

    /// <summary>SUM(ValorMonetario) de mis conversiones (backings) en el periodo</summary>
    public decimal MiValorGenerado { get; set; }

    /// <summary>SUM de PromotorWalletTransaccion del promotor en el periodo</summary>
    public decimal MiComisionAcumulada { get; set; }

    /// <summary>Resuelto desde la moneda del programa o la moneda mas frecuente si multiples programas. Null si sin conversiones.</summary>
    public string? MonedaNombre { get; set; }

    /// <summary>MisConversiones / MisClicks * 100. Cero si MisClicks = 0. Redondeado a 2 decimales.</summary>
    public decimal MiTasaConversion { get; set; }
}
```

#### EventoRecienteDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/EventoRecienteDto.cs

/// <summary>
/// Evento reciente tipo Backing en el historial del promotor.
/// Solo se incluyen eventos tipo Backing (TipoEventoPromoId = 4) porque tienen ValorMonetario.
/// </summary>
public class EventoRecienteDto
{
    public Guid Id { get; set; }

    public int TipoEventoId { get; set; }

    /// <summary>Resuelto desde Maestra_TipoEventoPromo</summary>
    public string TipoEventoNombre { get; set; } = null!;

    public decimal ValorMonetario { get; set; }

    /// <summary>
    /// Comision calculada para este backing.
    /// Null si no se genero comision (programa inactivo, promotor dado de baja en ese momento).
    /// </summary>
    public decimal? ComisionGenerada { get; set; }

    /// <summary>Resuelto desde MaestraMoneda</summary>
    public string? MonedaNombre { get; set; }

    /// <summary>ISO 8601 UTC</summary>
    public DateTime FechaEvento { get; set; }
}
```

#### PromotorMetricasResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorMetricasResponseDto.cs

public class PromotorMetricasResponseDto
{
    public Guid PromotorId { get; set; }

    public string PromotorNombre { get; set; } = null!;

    /// <summary>Null si la consulta no filtra por programa especifico.</summary>
    public Guid? ProgramaId { get; set; }

    /// <summary>Null si la consulta no filtra por programa especifico.</summary>
    public string? ProgramaTitulo { get; set; }

    public string FechaDesde { get; set; } = null!;

    public string FechaHasta { get; set; } = null!;

    public PromotorMetricasKpisDto Kpis { get; set; } = null!;

    /// <summary>
    /// Ultimos 20 eventos tipo Backing del promotor en el periodo, ordenados por FechaEvento DESC.
    /// </summary>
    public IReadOnlyList<EventoRecienteDto> EventosRecientes { get; set; } = new List<EventoRecienteDto>();
}
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/cp-tracking-metricas.ts

// -------------------------
// Tipos de dominio - Eventos
// -------------------------

export type TipoEventoPromo = 1 | 2 | 3 | 4 | 5;
// 1 = Click, 2 = PageView, 3 = Signup, 4 = Backing, 5 = Share

export const TIPO_EVENTO_PROMO = {
  Click: 1,
  PageView: 2,
  Signup: 3,
  Backing: 4,
  Share: 5,
} as const;

// -------------------------
// POST /tracking/evento - Request
// -------------------------

export interface RegistrarEventoRequest {
  codigoReferido?: string;
  tipoEventoPromoId: TipoEventoPromo;
  campaniaCrowdfundingId?: string;
  urlOrigen?: string;
  urlReferer?: string;
  utmSource?: string;
  utmMedium?: string;
  utmCampaign?: string;
}

export interface RegistrarEventoResponse {
  eventoId: string;
  registrado: boolean;
}

// -------------------------
// GET /programas/{id}/metricas - Response (Artista)
// -------------------------

export interface ProgramaMetricasKpis {
  totalClicks: number;
  totalPageViews: number;
  totalSignups: number;
  totalConversiones: number;
  valorTotalGenerado: number;
  /** Null si no hay conversiones con moneda */
  monedaNombre: string | null;
  /** TotalConversiones / TotalClicks * 100. 0 si sin clicks. */
  tasaConversion: number;
  comisionesTotales: number;
}

export interface RankingPromotorItem {
  promotorId: string;
  promotorNombre: string;
  tipoPromotorNombre: string | null;
  clicks: number;
  pageViews: number;
  signups: number;
  conversiones: number;
  valorGenerado: number;
  comisionAcumulada: number;
}

export interface EventosPorDiaItem {
  /** Formato YYYY-MM-DD */
  fecha: string;
  clicks: number;
  pageViews: number;
  signups: number;
  conversiones: number;
}

export interface ProgramaMetricasResponse {
  programaId: string;
  programaTitulo: string;
  /** Formato YYYY-MM-DD */
  fechaDesde: string;
  /** Formato YYYY-MM-DD */
  fechaHasta: string;
  kpis: ProgramaMetricasKpis;
  /** Ordenado por conversiones DESC */
  rankingPromotores: RankingPromotorItem[];
  /** Solo dias con actividad */
  eventosPorDia: EventosPorDiaItem[];
}

// -------------------------
// GET /promotor/metricas - Response (Promotor)
// -------------------------

export interface PromotorMetricasKpis {
  misClicks: number;
  misPageViews: number;
  misSignups: number;
  misConversiones: number;
  miValorGenerado: number;
  miComisionAcumulada: number;
  monedaNombre: string | null;
  /** MisConversiones / MisClicks * 100. 0 si sin clicks. */
  miTasaConversion: number;
}

export interface EventoReciente {
  id: string;
  tipoEventoId: TipoEventoPromo;
  tipoEventoNombre: string;
  valorMonetario: number;
  /** Null si no se genero comision */
  comisionGenerada: number | null;
  monedaNombre: string | null;
  /** ISO 8601 UTC string */
  fechaEvento: string;
}

export interface PromotorMetricasResponse {
  promotorId: string;
  promotorNombre: string;
  /** Null si la consulta no filtra por programa */
  programaId: string | null;
  /** Null si la consulta no filtra por programa */
  programaTitulo: string | null;
  fechaDesde: string;
  fechaHasta: string;
  kpis: PromotorMetricasKpis;
  /** Ultimos 20 eventos tipo Backing del periodo, FechaEvento DESC */
  eventosRecientes: EventoReciente[];
}
```

---

## Validaciones Compartidas

### Tabla de Reglas

| Campo | Regla | FluentValidation | Zod |
|-------|-------|-----------------|-----|
| `tipoEventoPromoId` | requerido | `.GreaterThan(0).WithErrorCode("1001")` | `.min(1, 'El tipo de evento es obligatorio')` |
| `tipoEventoPromoId` | valores validos (1-5) | `.Must(x => x >= 1 && x <= 5).WithErrorCode("1033")` | `.refine(v => [1,2,3,4,5].includes(v), 'Tipo de evento invalido')` |
| `codigoReferido` | max 50 caracteres | `.MaximumLength(50).WithErrorCode("1002")` | `.max(50, 'Maximo 50 caracteres').optional()` |
| `urlOrigen` | max 2048 caracteres | `.MaximumLength(2048).WithErrorCode("1002")` | `.max(2048, 'Maximo 2048 caracteres').optional()` |
| `urlReferer` | max 2048 caracteres | `.MaximumLength(2048).WithErrorCode("1002")` | `.max(2048, 'Maximo 2048 caracteres').optional()` |
| `utmSource` | max 100 caracteres | `.MaximumLength(100).WithErrorCode("1002")` | `.max(100, 'Maximo 100 caracteres').optional()` |
| `utmMedium` | max 100 caracteres | `.MaximumLength(100).WithErrorCode("1002")` | `.max(100, 'Maximo 100 caracteres').optional()` |
| `utmCampaign` | max 100 caracteres | `.MaximumLength(100).WithErrorCode("1002")` | `.max(100, 'Maximo 100 caracteres').optional()` |
| `fechaDesde` / `fechaHasta` | fechaDesde <= fechaHasta | `.Must((q, f) => f == null || q.FechaHasta == null || f <= q.FechaHasta).WithErrorCode("1036")` | `.refine(d => !d.fechaDesde || !d.fechaHasta || d.fechaDesde <= d.fechaHasta, ...)` |
| `valorMonetario` (conversion) | mayor que 0 | `.GreaterThan(0).WithErrorCode("1035")` | `.positive('El valor debe ser mayor que cero')` |

### FluentValidation (C#)

```csharp
// RegistrarEventoValidator.cs
using WePlayRises.Crowdpromotion.Domain.Constants;

public class RegistrarEventoValidator : AbstractValidator<RegistrarEventoCommand>
{
    public RegistrarEventoValidator()
    {
        RuleFor(x => x.TipoEventoPromoId)
            .GreaterThan(0)
            .WithMessage("El tipo de evento es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .Must(id => id >= 1 && id <= 5)
            .WithMessage("Tipo de evento invalido")
            .WithErrorCode(ServiceResponseMessageType.Validation_TipoEventoInvalido)
            .When(x => x.TipoEventoPromoId > 0);

        RuleFor(x => x.TipoEventoPromoId)
            .Must(id => id != 4)
            .WithMessage("El tipo de evento Backing no se acepta en este endpoint. Usar /tracking/conversion")
            .WithErrorCode(ServiceResponseMessageType.Validation_BackingNoPermitido)
            .When(x => x.TipoEventoPromoId >= 1 && x.TipoEventoPromoId <= 5);

        RuleFor(x => x.CodigoReferido)
            .MaximumLength(50)
            .WithMessage("El codigo referido no puede superar los 50 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.CodigoReferido));

        RuleFor(x => x.UrlOrigen)
            .MaximumLength(2048)
            .WithMessage("La URL de origen no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlOrigen));

        RuleFor(x => x.UrlReferer)
            .MaximumLength(2048)
            .WithMessage("La URL de referer no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlReferer));

        RuleFor(x => x.UtmSource)
            .MaximumLength(100)
            .WithMessage("El UTM source no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmSource));

        RuleFor(x => x.UtmMedium)
            .MaximumLength(100)
            .WithMessage("El UTM medium no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmMedium));

        RuleFor(x => x.UtmCampaign)
            .MaximumLength(100)
            .WithMessage("El UTM campaign no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmCampaign));
    }
}

// GetProgramaMetricasValidator.cs
public class GetProgramaMetricasValidator : AbstractValidator<GetProgramaMetricasQuery>
{
    public GetProgramaMetricasValidator()
    {
        RuleFor(x => x)
            .Must(q => q.FechaDesde == null || q.FechaHasta == null || q.FechaDesde <= q.FechaHasta)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha fin")
            .WithErrorCode(ServiceResponseMessageType.Validation_DateFinBeforeInicio);
    }
}

// GetPromotorMetricasValidator.cs
public class GetPromotorMetricasValidator : AbstractValidator<GetPromotorMetricasQuery>
{
    public GetPromotorMetricasValidator()
    {
        RuleFor(x => x)
            .Must(q => q.FechaDesde == null || q.FechaHasta == null || q.FechaDesde <= q.FechaHasta)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha fin")
            .WithErrorCode(ServiceResponseMessageType.Validation_DateFinBeforeInicio);
    }
}
```

### Zod Schemas (TypeScript)

```typescript
// Ruta: src/shared/schemas/cp-tracking-metricas.schema.ts
import { z } from 'zod';

// ---- Schema de filtro de fechas (reutilizable en ambos dashboards) ----

export const filtroFechasSchema = z
  .object({
    fechaDesde: z.string().optional(),
    fechaHasta: z.string().optional(),
  })
  .refine(
    (data) => {
      if (!data.fechaDesde || !data.fechaHasta) return true;
      return data.fechaDesde <= data.fechaHasta;
    },
    {
      message: 'La fecha de inicio no puede ser posterior a la fecha fin',
      path: ['fechaDesde'],
    }
  );

export type FiltroFechasFormData = z.infer<typeof filtroFechasSchema>;

// ---- Schema del filtro del dashboard del promotor ----

export const filtroMetricasPromotorSchema = filtroFechasSchema.extend({
  programaId: z.string().uuid('ID de programa invalido').optional(),
});

export type FiltroMetricasPromotorFormData = z.infer<typeof filtroMetricasPromotorSchema>;
```

**Nota:** El tracking de eventos (`RegistrarEventoRequest`) no requiere schema Zod porque se construye programaticamente en el frontend al detectar parametros UTM en la URL; no es un formulario de usuario.

---

## Constantes Nuevas en ServiceResponseMessageType

Los siguientes codigos deben agregarse a `ServiceResponseMessageType.cs` en el modulo Crowdpromotion:

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs

// Validation (1000-1999) - AGREGAR:

/// <summary>TipoEventoPromoId fuera del rango 1-5</summary>
public const string Validation_TipoEventoInvalido = "1033";

/// <summary>TipoEventoPromoId == 4 (Backing) enviado al endpoint publico de eventos</summary>
public const string Validation_BackingNoPermitido = "1034";

/// <summary>valorMonetario menor o igual a cero en una conversion</summary>
public const string Validation_ValorMonetarioInvalido = "1035";

/// <summary>fechaDesde posterior a fechaHasta en filtros de metricas</summary>
// NOTA: Validation_DateFinBeforeInicio = "1022" ya existe. Reutilizar.

// Business Rules (4000-4999) - AGREGAR:

/// <summary>Rate limit excedido: mismo IP + codigoReferido, ventana de 5 minutos, solo para tipo Click</summary>
public const string BusinessRule_RateLimitExcedido = "4032";
```

---

## Constantes Compartidas

```typescript
// src/shared/constants/query-keys.ts
// AGREGAR al objeto QUERY_KEYS existente:
export const QUERY_KEYS = {
  // ... existentes ...
  cpProgramaMetricas: (programaId: string, fechaDesde?: string, fechaHasta?: string) =>
    ['cp-programa-metricas', programaId, fechaDesde, fechaHasta] as const,
  cpPromotorMetricas: (programaId?: string, fechaDesde?: string, fechaHasta?: string) =>
    ['cp-promotor-metricas', programaId, fechaDesde, fechaHasta] as const,
};
```

```typescript
// src/shared/constants/api-routes.ts
// AGREGAR al objeto API_ROUTES existente:
export const API_ROUTES = {
  // ... existentes ...
  crowdpromotion: {
    // ... existentes ...
    tracking: {
      evento: '/api/crowdpromotion/tracking/evento',
    },
    programaMetricas: (programaId: string) =>
      `/api/crowdpromotion/programas/${programaId}/metricas`,
    promotorMetricas: '/api/crowdpromotion/promotor/metricas',
  },
};
```

```typescript
// src/shared/constants/tracking.ts
// Constantes de parametros UTM y tracking para el frontend

export const TRACKING_PARAMS = {
  ref: 'ref',
  utmSource: 'utm_source',
  utmMedium: 'utm_medium',
  utmCampaign: 'utm_campaign',
} as const;

export const TRACKING_STORAGE_KEY = 'wp_tracking_ref';
// Clave para persistir el codigoReferido en sessionStorage durante la sesion del usuario

export const UTM_SOURCE_WEPLAY = 'weplay';
export const UTM_MEDIUM_REFERRAL = 'referral';

export const TIPO_EVENTO_PROMO = {
  Click: 1,
  PageView: 2,
  Signup: 3,
  Backing: 4,
  Share: 5,
} as const;
```

---

## Mapeo de Errores a UI

```typescript
// src/shared/utils/error-messages.ts
// AGREGAR al objeto ERROR_MESSAGES existente:
export const ERROR_MESSAGES: Record<string, string> = {
  // ... existentes ...

  // Tracking de metricas
  '1033': 'Tipo de evento no reconocido.',
  '1034': 'Este tipo de evento no puede registrarse por esta via.',
  '1035': 'El valor de la aportacion debe ser mayor que cero.',
  '4032': 'Demasiadas peticiones en poco tiempo. Espera unos minutos e intenta de nuevo.',
};
```

---

## Reglas de Negocio Documentadas

### RN-01: Tracking anonimo vs vinculado

El endpoint `POST /tracking/evento` siempre devuelve `201 Created` si el evento se inserta, independientemente de si el `codigoReferido` es valido. Un codigo invalido no es un error; simplemente el evento se registra sin vinculacion a promotor. Esto permite rastrear trafico organico con UTMs sin codigo referido (FA-06).

### RN-02: Rate limiting solo para tipo Click

El rate limiting de 5 minutos aplica exclusivamente para eventos `TipoEventoPromoId = 1` (Click). Los demas tipos (PageView, Signup, Share) no tienen rate limiting. Esto previene que un promotor infle artificialmente sus clicks (FA-04).

Clave de cache: `tracking:ratelimit:{ip}:{codigoReferido}` o `tracking:ratelimit:{ip}:anonimo` si no hay codigo referido.

### RN-03: Tipo Backing prohibido en endpoint publico

El tipo `Backing` (4) nunca debe enviarse al endpoint publico `/tracking/evento`. Las conversiones se registran exclusivamente via el servicio interno al procesar backings en el modulo Crowdfunding. Si el frontend envia `TipoEventoPromoId = 4`, devolver HTTP 400 con error `1034`.

### RN-04: Comision solo para programa activo y promotor vigente

Al registrar una conversion, la comision solo se calcula y acredita si:
- `PromoPrograma.EsActivo == true` en el momento del backing
- `PromoProgramaPromotor.EsAprobado == true` y `EsBloqueado == false` y `FechaBaja == null`

Si alguna condicion falla: el evento se registra igualmente, `ComisionCalculada = 0`, `ComisionAcreditada = false` (FA-02 y FA-03).

### RN-05: Persistencia del codigo referido en sesion

El frontend debe persistir el `codigoReferido` detectado en la URL en `sessionStorage` con la clave `wp_tracking_ref`. Al navegar a cualquier pagina dentro de la misma sesion, si `sessionStorage` tiene el codigo, incluirlo en los eventos subsiguientes (PageView, Signup). Al cerrar el tab, el codigo se pierde (comportamiento nativo de sessionStorage).

Esto implementa el AC-CP05-2: atribucion de eventos posteriores al mismo promotor sin que el codigo aparezca en todas las URLs.

### RN-06: Calculo de tasa de conversion

`tasaConversion = (totalConversiones / totalClicks) * 100`

Si `totalClicks == 0`, devolver `0` (no dividir por cero). El valor se redondea a 2 decimales en el backend antes de serializar. En el frontend, mostrar como porcentaje: `{tasaConversion.toFixed(2)}%`.

### RN-07: Ownership del artista en metricas del programa

Para `GET /programas/{programaId}/metricas`, el handler debe:
1. Resolver `ArtistaId` desde `UserId` del token.
2. Cargar `PromoPrograma` por `programaId`.
3. Verificar `PromoPrograma.ArtistaId == artista.Id`. Si no coincide: HTTP 403, ErrorCode `4026`.

### RN-08: eventosRecientes limita a 20 registros

El endpoint `GET /promotor/metricas` devuelve como maximo 20 eventos recientes tipo Backing. No hay paginacion en este campo; es un historial de actividad reciente. Si se requiere mas historico, implementar un endpoint dedicado (fuera del MVP).

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores completos
- [x] Autorizacion por endpoint (publico, artista, promotor, interno)
- [x] Claims JWT documentados
- [x] DTOs C# completos con comentarios de constraints
- [x] Types TypeScript equivalentes a los DTOs C#
- [x] Schemas Zod con las mismas reglas de validacion
- [x] Constantes compartidas (query keys, API routes, tracking params)
- [x] Mapeo de errores a mensajes de UI
- [x] Nuevos ServiceResponseMessageType documentados
- [x] Reglas de negocio criticas documentadas (rate limiting, comisiones, persistencia de sesion)
