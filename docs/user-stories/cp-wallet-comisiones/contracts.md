# Contratos: Wallet de Promotor, Comisiones y Cobros

> **Feature:** cp-wallet-comisiones (US-CP-06)
> **Ultima actualizacion:** 2026-03-02
> **Modulo Backend:** Crowdpromotion
> **Depende de:** US-CP-03 (cp-inscripcion-programa), US-CP-05 (cp-tracking-metricas)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Cambios al Modelo de Dominio

La entidad `PromotorWalletTransaccion` requiere tres campos adicionales para soportar esta feature. Estos campos deben agregarse antes de la implementacion.

### PromotorWalletTransaccion - Campos a Agregar

```csharp
// Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWalletTransaccion.cs

/// <summary>
/// Indica si es un credito (ingreso) o debito (salida/retiro).
/// true = credito (comision acreditada), false = debito (cobro/retiro).
/// </summary>
public bool EsCredito { get; set; }

/// <summary>
/// Referencia al PromoEvento que origino este credito.
/// Solo aplica cuando EsCredito = true. Null para retiros.
/// </summary>
public Guid? PromoEventoId { get; set; }

/// <summary>
/// Descripcion legible del origen de la transaccion.
/// Ejemplos: "Comision por backing referido", "Retiro mensual".
/// Max 500 caracteres.
/// </summary>
public string? Descripcion { get; set; }
```

**Requiere migracion EF Core:**
```
Add-Migration AddWalletTransaccionFields -Project WePlayRises.Crowdpromotion.Infra
```

---

## Modelo de Dominio Completo (Referencia)

```
PromotorWallet
  Id                    Guid          PK
  PromotorId            PromotorId    FK a Promotor (strongly-typed ID)
  MonedaId              int           FK a MaestraMoneda
  SaldoDisponible       decimal       Saldo disponible para retiro
  SaldoPendiente        decimal       Saldo en transacciones pendientes
  TotalGanado           decimal       Acumulado historico de creditos
  TotalRetirado         decimal       Acumulado historico de debitos
  FechaCreacion         DateTime
  FechaActualizacion    DateTime?

PromotorWalletTransaccion
  Id                    Guid          PK
  WalletId              Guid          FK a PromotorWallet
  EsCredito             bool          NUEVO: true=ingreso, false=retiro
  TipoRewardId          int?          FK a MaestraTipoRewardPromo (null para retiros)
  EstadoTransaccionId   int           FK a MaestraEstadoWalletTransaccion
  CampaniaPayoutId      Guid?         FK futuro (MVP: null)
  Importe               decimal       Importe absoluto (siempre positivo)
  Concepto              string?       Etiqueta interna corta
  Descripcion           string?       NUEVO: Texto legible max 500 chars
  ReferenciaExterna     string?       Referencia externa de pago
  PromoEventoId         Guid?         NUEVO: FK a PromoEvento (solo creditos)
  FechaCreacion         DateTime
  FechaProcesado        DateTime?
```

### MaestraEstadoWalletTransaccion (Seed Fijo)

| Id | Nombre |
|----|--------|
| 1 | Pendiente |
| 2 | Procesada |
| 3 | Pagada |
| 4 | Cancelada |

### Regla de Negocio: Minimo de Retiro

El minimo de retiro es **10.00** en la moneda del wallet. Este valor es una constante de dominio (`MinRetiro = 10.00m`) definida en el servicio, no en base de datos (MVP).

---

## Endpoints API

### GET /api/crowdpromotion/promotor/wallet

**Descripcion:** Obtiene el resumen del wallet del promotor autenticado. Si el promotor aun no tiene wallet (nunca recibio comision), retorna 404.

**Autorizacion:** Bearer JWT (cualquier usuario con perfil de promotor activo)

**Request Body:** ninguno

**Query Params:** ninguno

**Response 200:**

```json
{
  "data": {
    "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "monedaId": 1,
    "monedaNombre": "EUR",
    "saldoDisponible": 150.50,
    "saldoPendiente": 0.00,
    "totalGanado": 200.00,
    "totalRetirado": 49.50,
    "minimoRetiro": 10.00
  },
  "messages": []
}
```

**Errores:**

| HTTP | ErrorCode | Mensaje Backend | Causa |
|------|-----------|-----------------|-------|
| 401 | 3001 | No autorizado | Token ausente o invalido |
| 403 | 3002 | Forbidden | El userId del token no tiene perfil de promotor |
| 404 | 2015 | Promotor no encontrado | El usuario no tiene perfil de promotor |
| 404 | 2030 | Wallet no encontrado | El promotor aun no tiene wallet asignado |
| 500 | 5000 | Error inesperado | Error interno |

---

### GET /api/crowdpromotion/promotor/wallet/transacciones

**Descripcion:** Lista paginada de transacciones del wallet del promotor autenticado, con filtros opcionales.

**Autorizacion:** Bearer JWT (cualquier usuario con perfil de promotor activo)

**Request Body:** ninguno

**Query Params:**

| Parametro | Tipo | Requerido | Default | Descripcion |
|-----------|------|-----------|---------|-------------|
| esCredito | bool | No | null (todas) | Filtrar por tipo: true=ingresos, false=retiros |
| estadoTransaccionId | int | No | null (todos) | Filtrar por estado (1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada) |
| fechaDesde | string (ISO 8601) | No | null | Filtro desde fecha (inclusive) |
| fechaHasta | string (ISO 8601) | No | null | Filtro hasta fecha (inclusive) |
| page | int | No | 1 | Numero de pagina (minimo 1) |
| pageSize | int | No | 10 | Registros por pagina (minimo 1, maximo 50) |

**Response 200:**

```json
{
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "esCredito": true,
        "importe": 10.00,
        "descripcion": "Comision por backing referido",
        "concepto": null,
        "estadoTransaccionId": 2,
        "estadoTransaccionNombre": "Procesada",
        "tipoRewardId": 1,
        "tipoRewardNombre": "Dinero",
        "promoEventoId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        "fechaCreacion": "2026-03-20T14:30:00Z",
        "fechaProcesado": "2026-03-21T10:00:00Z"
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 10,
    "totalPages": 2
  },
  "messages": []
}
```

**Errores:**

| HTTP | ErrorCode | Mensaje Backend | Causa |
|------|-----------|-----------------|-------|
| 400 | 1021 | Rango fuera de limites | page < 1 o pageSize fuera de [1, 50] |
| 400 | 1036 | Rango de fechas invalido | fechaDesde posterior a fechaHasta |
| 401 | 3001 | No autorizado | Token ausente o invalido |
| 404 | 2015 | Promotor no encontrado | El usuario no tiene perfil de promotor |
| 404 | 2030 | Wallet no encontrado | El promotor aun no tiene wallet |
| 500 | 5000 | Error inesperado | Error interno |

---

### POST /api/crowdpromotion/promotor/wallet/cobro

**Descripcion:** Registra una solicitud de cobro/retiro desde el wallet del promotor. Descuenta el importe del saldo disponible y crea una transaccion en estado Pendiente (EstadoTransaccionId = 1). El procesamiento real del pago queda fuera del MVP.

**Autorizacion:** Bearer JWT (cualquier usuario con perfil de promotor activo)

**Request Body:**

```json
{
  "importe": 50.00,
  "descripcion": "Retiro mensual"
}
```

**Constraints del Request:**

| Campo | Tipo | Requerido | Reglas |
|-------|------|-----------|--------|
| importe | decimal | Si | > 0, <= SaldoDisponible del wallet |
| descripcion | string | No | max 500 caracteres |

**Response 201:**

```json
{
  "data": {
    "transaccionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "importe": 50.00,
    "monedaNombre": "EUR",
    "estadoTransaccionNombre": "Pendiente",
    "saldoRestante": 100.50,
    "fechaCreacion": "2026-03-20T15:00:00Z"
  },
  "messages": [
    { "message": "Solicitud de cobro registrada", "errorCode": "0001" }
  ]
}
```

**Errores:**

| HTTP | ErrorCode | Mensaje Backend | Causa |
|------|-----------|-----------------|-------|
| 400 | 1001 | Campo obligatorio | importe ausente |
| 400 | 1021 | Rango fuera de limites | importe <= 0 |
| 400 | 1002 | MaxLength excedido | descripcion > 500 caracteres |
| 401 | 3001 | No autorizado | Token ausente o invalido |
| 404 | 2015 | Promotor no encontrado | El usuario no tiene perfil de promotor |
| 404 | 2030 | Wallet no encontrado | El promotor no tiene wallet |
| 409 | 4040 | Saldo insuficiente | importe > SaldoDisponible |
| 409 | 4041 | Saldo bajo minimo de retiro | SaldoDisponible < 10.00 (MinRetiro) |
| 409 | 4042 | Cobro concurrente en proceso | Ya existe una solicitud pendiente de este promotor |
| 500 | 5000 | Error inesperado | Error interno |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol Requerido | Notas |
|----------|------|---------------|-------|
| `GET /api/crowdpromotion/promotor/wallet` | Bearer JWT | ninguno (cualquier rol) | PromotorId se extrae del token via perfil de promotor |
| `GET /api/crowdpromotion/promotor/wallet/transacciones` | Bearer JWT | ninguno (cualquier rol) | Mismo PromotorId del token |
| `POST /api/crowdpromotion/promotor/wallet/cobro` | Bearer JWT | ninguno (cualquier rol) | Mismo PromotorId del token |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string, GUID format)",
  "email": "usuario@ejemplo.com",
  "exp": 1234567890
}
```

El `PromotorId` no viaja en el JWT. El backend lo resuelve buscando el perfil de promotor asociado al `sub` (userId) del token.

### Proteccion de Rutas Frontend

| Ruta | Auth | Redirect si no autenticado |
|------|------|---------------------------|
| `/promotor/wallet` | Requerida | `/auth/login` |
| `/promotor/wallet/transacciones` | Requerida | `/auth/login` |

---

## DTOs / Types

### Backend (C#)

#### PromotorWalletDto.cs

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromotorWalletDto.cs
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class PromotorWalletDto
{
    /// <summary>ID del wallet</summary>
    public Guid WalletId { get; set; }

    /// <summary>ID de la moneda (FK a MaestraMoneda)</summary>
    public int MonedaId { get; set; }

    /// <summary>Nombre de la moneda (ej: "EUR")</summary>
    public string MonedaNombre { get; set; } = null!;

    /// <summary>Saldo disponible para retiro</summary>
    public decimal SaldoDisponible { get; set; }

    /// <summary>Saldo en transacciones pendientes</summary>
    public decimal SaldoPendiente { get; set; }

    /// <summary>Acumulado historico de creditos</summary>
    public decimal TotalGanado { get; set; }

    /// <summary>Acumulado historico de debitos/retiros</summary>
    public decimal TotalRetirado { get; set; }

    /// <summary>Importe minimo para solicitar un cobro. Constante de dominio = 10.00</summary>
    public decimal MinimoRetiro { get; set; }
}
```

#### WalletTransaccionItemDto.cs

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/WalletTransaccionItemDto.cs
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionItemDto
{
    public Guid Id { get; set; }

    /// <summary>true=credito/ingreso, false=debito/retiro</summary>
    public bool EsCredito { get; set; }

    /// <summary>Importe absoluto (siempre positivo)</summary>
    public decimal Importe { get; set; }

    /// <summary>Texto legible del origen. Max 500 chars. Nullable.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Etiqueta interna corta. Nullable.</summary>
    public string? Concepto { get; set; }

    public int EstadoTransaccionId { get; set; }

    /// <summary>Nombre del estado (ej: "Procesada")</summary>
    public string EstadoTransaccionNombre { get; set; } = null!;

    /// <summary>Tipo de reward que origino el credito. Null para retiros.</summary>
    public int? TipoRewardId { get; set; }

    /// <summary>Nombre del tipo de reward. Null si no aplica.</summary>
    public string? TipoRewardNombre { get; set; }

    /// <summary>ID del PromoEvento que origino el credito. Null para retiros.</summary>
    public Guid? PromoEventoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaProcesado { get; set; }
}
```

#### WalletTransaccionesPagedDto.cs

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/WalletTransaccionesPagedDto.cs
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class WalletTransaccionesPagedDto
{
    public List<WalletTransaccionItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
```

#### SolicitarCobroRequest.cs

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Commands/SolicitarCobroCommand.cs
// (El record de comando actua como DTO de request en CQRS)
namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;

public record SolicitarCobroCommand(
    Guid PromotorId,
    /// <summary>Importe a retirar. Debe ser > 0 y <= SaldoDisponible.</summary>
    decimal Importe,
    /// <summary>Descripcion opcional del retiro. Max 500 chars.</summary>
    string? Descripcion
) : IRequest<ServiceResponse<SolicitarCobroResponseDto>>;
```

#### SolicitarCobroResponseDto.cs

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/SolicitarCobroResponseDto.cs
namespace WePlayRises.Crowdpromotion.Application.Dtos;

public class SolicitarCobroResponseDto
{
    public Guid TransaccionId { get; set; }

    public decimal Importe { get; set; }

    /// <summary>Nombre de la moneda del wallet (ej: "EUR")</summary>
    public string MonedaNombre { get; set; } = null!;

    /// <summary>Nombre del estado inicial: siempre "Pendiente"</summary>
    public string EstadoTransaccionNombre { get; set; } = null!;

    /// <summary>SaldoDisponible del wallet tras descontar el retiro</summary>
    public decimal SaldoRestante { get; set; }

    public DateTime FechaCreacion { get; set; }
}
```

#### WalletTransaccionesQuery.cs (Query params como record)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/Wallet/Queries/GetWalletTransaccionesQuery.cs
namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;

public record GetWalletTransaccionesQuery(
    Guid PromotorId,
    bool? EsCredito,
    int? EstadoTransaccionId,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int Page,
    int PageSize
) : IRequest<ServiceResponse<WalletTransaccionesPagedDto>>;
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdpromotion.ts (agregar al final)

// ========== Wallet de Promotor - US-CP-06 ==========

// --- DTOs de Response ---

export interface PromotorWallet {
    walletId: string;
    monedaId: number;
    monedaNombre: string;
    saldoDisponible: number;
    saldoPendiente: number;
    totalGanado: number;
    totalRetirado: number;
    minimoRetiro: number;
}

export interface WalletTransaccionItem {
    id: string;
    esCredito: boolean;
    importe: number;
    descripcion: string | null;
    concepto: string | null;
    estadoTransaccionId: number;
    estadoTransaccionNombre: string;
    tipoRewardId: number | null;
    tipoRewardNombre: string | null;
    promoEventoId: string | null;
    fechaCreacion: string;
    fechaProcesado: string | null;
}

export interface WalletTransaccionesPagedResponse {
    items: WalletTransaccionItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

// --- DTOs de Request ---

export interface SolicitarCobroRequest {
    importe: number;
    descripcion?: string;
}

export interface SolicitarCobroResponse {
    transaccionId: string;
    importe: number;
    monedaNombre: string;
    estadoTransaccionNombre: string;
    saldoRestante: number;
    fechaCreacion: string;
}

// --- Filtros (Query Params) ---

export interface WalletTransaccionesFilters {
    esCredito?: boolean;
    estadoTransaccionId?: number;
    fechaDesde?: string;
    fechaHasta?: string;
    page?: number;
    pageSize?: number;
}
```

---

## Validaciones Compartidas

### SolicitarCobro

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| importe | Requerido | `.NotEmpty().WithErrorCode("1001")` | `.min(0.01, ...)` |
| importe | > 0 | `.GreaterThan(0m).WithErrorCode("1021")` | `.positive('El importe debe ser mayor que cero')` |
| importe | <= SaldoDisponible | Verificacion en service, retorna `4040` | Validacion en cliente antes de enviar (soft check) |
| descripcion | max 500 chars | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres').optional()` |

### GetWalletTransacciones (Query Params)

| Parametro | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-----------|-------|---------------------------|----------------|
| page | >= 1 | `.GreaterThanOrEqualTo(1).WithErrorCode("1021")` | `.int().min(1).optional()` |
| pageSize | 1-50 | `.InclusiveBetween(1, 50).WithErrorCode("1021")` | `.int().min(1).max(50).optional()` |
| fechaDesde | fechaDesde <= fechaHasta | Custom rule, retorna `1036` | Refine de Zod (mismo que filtroFechasSchema) |

---

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdpromotion.schema.ts (agregar al final del archivo)

// ========== Wallet - US-CP-06 ==========

export const solicitarCobroSchema = z.object({
    importe: z
        .number({
            required_error: 'El importe es obligatorio',
            invalid_type_error: 'El importe debe ser un numero',
        })
        .positive('El importe debe ser mayor que cero'),

    descripcion: z
        .string()
        .max(500, 'La descripcion no puede superar los 500 caracteres')
        .optional(),
});

export type SolicitarCobroFormData = z.infer<typeof solicitarCobroSchema>;

export const walletTransaccionesFiltersSchema = z
    .object({
        esCredito: z.boolean().optional(),

        estadoTransaccionId: z
            .number()
            .int()
            .min(1)
            .max(4, 'Estado invalido')
            .optional(),

        fechaDesde: z.string().optional(),

        fechaHasta: z.string().optional(),

        page: z
            .number()
            .int()
            .min(1, 'La pagina debe ser mayor a 0')
            .optional(),

        pageSize: z
            .number()
            .int()
            .min(1, 'El tamano de pagina debe ser mayor a 0')
            .max(50, 'El tamano de pagina no puede superar 50')
            .optional(),
    })
    .refine(
        (data) =>
            !data.fechaDesde || !data.fechaHasta || data.fechaDesde <= data.fechaHasta,
        {
            message: 'La fecha de inicio no puede ser posterior a la fecha fin',
            path: ['fechaDesde'],
        }
    );

export type WalletTransaccionesFiltersFormData = z.infer<typeof walletTransaccionesFiltersSchema>;
```

---

## Nuevas Constantes de ServiceResponseMessageType

Los siguientes rangos estan libres en el archivo de constantes del dominio y deben agregarse:

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs
// Agregar dentro de la clase publica estatica ServiceResponseMessageType

// Wallet - US-CP-06 (NotFound 2030-2039)
public const string NotFound_Wallet = "2030";

// Wallet - US-CP-06 (Business Rules 4040-4049)
public const string BusinessRule_SaldoInsuficiente = "4040";
public const string BusinessRule_SaldoBajoMinimoRetiro = "4041";
public const string BusinessRule_CobroConcurrente = "4042";
```

**Nota sobre rangos:**
- `2030` es el primer libre en el bloque `2000-2999` tras `2022`.
- `4040` es el primer libre en el bloque `4000-4999` tras `4032`.

---

## Nuevas Constantes Compartidas

```typescript
// Ruta: src/shared/constants/index.ts (agregar en las secciones correspondientes)

// ========== QUERY_KEYS: agregar dentro de crowdpromotion: { ... } ==========
wallet: {
    resumen: ['crowdpromotion', 'wallet', 'resumen'] as const,
    transacciones: (filters?: WalletTransaccionesFilters) =>
        ['crowdpromotion', 'wallet', 'transacciones', filters] as const,
},

// ========== API_ROUTES: agregar dentro de crowdpromotion: { ... } ==========
promotorWallet: {
    resumen: '/api/crowdpromotion/promotor/wallet',
    transacciones: '/api/crowdpromotion/promotor/wallet/transacciones',
    cobro: '/api/crowdpromotion/promotor/wallet/cobro',
},

// ========== APP_ROUTES: agregar dentro de landing.promotor: { ... } ==========
// (nuevo) dentro de landing.promotor (ya existente):
wallet: '/promotor/wallet',

// ========== VALIDATION: agregar al final del bloque VALIDATION ==========
// Wallet (US-CP-06)
WALLET_DESCRIPCION_COBRO_MAX: 500,
WALLET_TRANSACCIONES_PAGE_SIZE_MAX: 50,

// ========== ESTADO_WALLET_TRANSACCION (nueva seccion) ==========
export const ESTADO_WALLET_TRANSACCION = {
    PENDIENTE: 1,
    PROCESADA: 2,
    PAGADA: 3,
    CANCELADA: 4,
} as const;

export const ESTADO_WALLET_TRANSACCION_LABELS: Record<number, string> = {
    1: 'Pendiente',
    2: 'Procesada',
    3: 'Pagada',
    4: 'Cancelada',
};

export const ESTADO_WALLET_TRANSACCION_BADGES: Record<number, string> = {
    1: 'secondary',
    2: 'default',
    3: 'success',
    4: 'destructive',
};

export const MIN_RETIRO_WALLET = 10.00;
export const WALLET_DEFAULT_PAGE_SIZE = 10;
```

---

## Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts (agregar al final)

// ========== Crowdpromotion - Wallet y Cobros Error Messages (US-CP-06) ==========

export const WALLET_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes
    '1001': 'El importe es obligatorio.',
    '1002': 'La descripcion no puede superar los 500 caracteres.',
    '1021': 'El importe debe ser mayor que cero.',
    '1036': 'La fecha de inicio no puede ser posterior a la fecha fin.',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '2030': 'No tienes un wallet asociado. Completa tu primera actividad de promocion para activarlo.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente.',
    '3002': 'No tienes permiso para realizar esta accion.',
    '4040': 'Saldo insuficiente. El importe solicitado supera tu saldo disponible.',
    '4041': 'El saldo disponible es inferior al minimo de retiro (10.00 EUR). Acumula mas comisiones antes de solicitar un cobro.',
    '4042': 'Ya tienes una solicitud de cobro en proceso. Espera a que sea procesada antes de crear otra.',
    '5000': 'Ha ocurrido un error inesperado al procesar tu solicitud. Por favor, intenta nuevamente.',

    // Semantic keys para uso interno en hooks y componentes
    WALLET_NOT_FOUND: 'No tienes un wallet asociado. Completa tu primera actividad de promocion para activarlo.',
    PROMOTOR_NOT_FOUND: 'No tienes un perfil de promotor. Registrate primero.',
    SALDO_INSUFICIENTE: 'Saldo insuficiente. El importe solicitado supera tu saldo disponible.',
    SALDO_BAJO_MINIMO: 'El saldo disponible es inferior al minimo de retiro (10.00 EUR).',
    COBRO_CONCURRENTE: 'Ya tienes una solicitud de cobro en proceso. Espera a que sea procesada.',
    IMPORTE_INVALIDO: 'El importe debe ser mayor que cero.',
    DESCRIPCION_MAX: 'La descripcion no puede superar los 500 caracteres.',
} as const;

export const getWalletErrorMessage = (errorCode: string): string => {
    return WALLET_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
```

**Adicionalmente**, agregar al `ERROR_CODE_MESSAGES` global:

```typescript
// En src/shared/utils/error-messages.ts, dentro de ERROR_CODE_MESSAGES
"2030": "No tienes un wallet asociado. Completa tu primera actividad para activarlo.",
"4040": "Saldo insuficiente para realizar el cobro solicitado.",
"4041": "El saldo disponible es inferior al minimo de retiro.",
"4042": "Ya existe una solicitud de cobro pendiente. Espera a que sea procesada.",
```

---

## Checklist de Contratos

- [x] Cambios al modelo de dominio documentados (EsCredito, PromoEventoId, Descripcion)
- [x] Endpoints con metodo, ruta, descripcion
- [x] Autorizacion documentada por endpoint
- [x] Claims JWT documentados
- [x] Request body con constraints por endpoint
- [x] Response exitosa con ejemplo JSON completo
- [x] Tabla de errores con HTTP, ErrorCode, mensaje y causa
- [x] DTOs C# completos (Request, Response, Paged)
- [x] Types TypeScript equivalentes (interfaz por interfaz)
- [x] Schemas Zod con mismas reglas que FluentValidation
- [x] Nuevas constantes ServiceResponseMessageType (2030, 4040-4042)
- [x] Nuevas constantes QUERY_KEYS, API_ROUTES, APP_ROUTES
- [x] Constante ESTADO_WALLET_TRANSACCION con labels y badges
- [x] Constante MIN_RETIRO_WALLET
- [x] Constante VALIDATION (campos de wallet)
- [x] WALLET_ERROR_MESSAGES con codigos numericos y claves semanticas
- [x] Entradas en ERROR_CODE_MESSAGES global
