# Plan de Contratos Shared: cp-wallet-comisiones

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Basado en:** docs/user-stories/cp-wallet-comisiones/contracts.md

---

## 1. Resumen

- Total de types nuevos: 6 interfaces + 1 union type
- Total de schemas Zod nuevos: 2 schemas + 2 types inferidos
- Constantes nuevas: 7 bloques (QUERY_KEYS wallet, API_ROUTES wallet, APP_ROUTES wallet, ESTADO_WALLET_TRANSACCION, LABELS, BADGES, MIN_RETIRO_WALLET, WALLET_DEFAULT_PAGE_SIZE, VALIDATION wallet)
- Utilidades nuevas: 2 (WALLET_ERROR_MESSAGES record, getWalletErrorMessage funcion)
- Archivos a modificar: 4 (todos son modificaciones de archivos existentes, ningun archivo nuevo)

**Patron de integracion:** Todos los nuevos simbolos se agregan **al final** de sus respectivos archivos, siguiendo el patron de seccion establecido con comentarios `// ========== ... ==========`.

---

## 2. Types (`src/shared/types/crowdpromotion.ts`)

**Accion:** Modificar - agregar al final del archivo.

### 2.1 DTOs de Response

| Tipo | Propiedades clave | Descripcion |
|------|-------------------|-------------|
| `PromotorWallet` | `walletId`, `monedaId`, `monedaNombre`, `saldoDisponible`, `saldoPendiente`, `totalGanado`, `totalRetirado`, `minimoRetiro` | Resumen del wallet del promotor autenticado. Mapeado desde `PromotorWalletDto` C# |
| `WalletTransaccionItem` | `id`, `esCredito`, `importe`, `descripcion`, `concepto`, `estadoTransaccionId`, `estadoTransaccionNombre`, `tipoRewardId`, `tipoRewardNombre`, `promoEventoId`, `fechaCreacion`, `fechaProcesado` | Un item de transaccion en el historial. `esCredito=true` indica ingreso, `false` indica retiro. `importe` es siempre positivo |
| `WalletTransaccionesPagedResponse` | `items`, `totalCount`, `page`, `pageSize`, `totalPages` | Respuesta paginada del endpoint GET /transacciones. Sigue el mismo patron que `MisProgramasResponse`, `TareasPendientesResponse`, etc. |
| `SolicitarCobroResponse` | `transaccionId`, `importe`, `monedaNombre`, `estadoTransaccionNombre`, `saldoRestante`, `fechaCreacion` | Respuesta 201 del POST /cobro. `estadoTransaccionNombre` sera siempre "Pendiente" en MVP |

### 2.2 DTOs de Request

| Tipo | Propiedades | Reglas |
|------|-------------|--------|
| `SolicitarCobroRequest` | `importe: number`, `descripcion?: string` | `importe` > 0 y <= saldoDisponible (validacion de negocio en service). `descripcion` opcional max 500 chars |
| `WalletTransaccionesFilters` | `esCredito?: boolean`, `estadoTransaccionId?: number`, `fechaDesde?: string`, `fechaHasta?: string`, `page?: number`, `pageSize?: number` | Todos opcionales. Mapeados a query params en el servicio. Sigue el patron de `ExplorarProgramasFilters` e `InscripcionesFilters` existentes |

### 2.3 Union Types

No se definen union types nuevos. El estado de transaccion se modela como `number` (ID de maestras) con constantes separadas en `constants/index.ts`, siguiendo el patron existente de `ESTADO_NECESIDAD`, `ESTADO_PROPUESTA`, etc.

### 2.4 Codigo exacto a agregar

```typescript
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

## 3. Schemas Zod (`src/shared/schemas/crowdpromotion.schema.ts`)

**Accion:** Modificar - agregar al final del archivo.

### 3.1 Schemas de Validacion

| Schema | Campos validados | Reglas clave |
|--------|-----------------|--------------|
| `solicitarCobroSchema` | `importe`, `descripcion` | `importe`: number requerido, `.positive()`. `descripcion`: string `.max(500)`, opcional |
| `walletTransaccionesFiltersSchema` | `esCredito`, `estadoTransaccionId`, `fechaDesde`, `fechaHasta`, `page`, `pageSize` | `estadoTransaccionId`: int entre 1-4. `page`: int >= 1. `pageSize`: int entre 1-50. `.refine()` para validar que `fechaDesde <= fechaHasta` - reutiliza la misma logica que `filtroFechasSchema` existente |

### 3.2 Patron de reutilizacion

El `walletTransaccionesFiltersSchema` debe reutilizar las variables internas `fechasRefinement` y `fechasRefinementConfig` ya definidas en el archivo para `filtroFechasSchema` y `filtroMetricasPromotorSchema`. Esto garantiza consistencia en el mensaje de error de fechas en todo el proyecto.

**Nota de implementacion:** Las variables `fechasRefinement` y `fechasRefinementConfig` ya son locales al archivo (no exportadas). El nuevo schema simplemente las referencia desde el mismo archivo.

### 3.3 Types Inferidos

| Type | Schema origen |
|------|---------------|
| `SolicitarCobroFormData` | `z.infer<typeof solicitarCobroSchema>` |
| `WalletTransaccionesFiltersFormData` | `z.infer<typeof walletTransaccionesFiltersSchema>` |

### 3.4 Codigo exacto a agregar

```typescript
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
    .refine(fechasRefinement, fechasRefinementConfig);

export type WalletTransaccionesFiltersFormData = z.infer<typeof walletTransaccionesFiltersSchema>;
```

---

## 4. Constantes (`src/shared/constants/index.ts`)

**Accion:** Modificar - cuatro puntos de insercion distintos dentro del archivo.

### 4.1 QUERY_KEYS - wallet (dentro de `crowdpromotion: { ... }`)

Insertar despues del bloque `metricas: { ... }` dentro del objeto `crowdpromotion` de `QUERY_KEYS`.

**Referencia de integracion:** La firma de `walletTransacciones` sigue el mismo patron que `programas.explorar(filters?)` - acepta filtros opcionales tipados y los embebe en la key para invalidacion granular.

```typescript
wallet: {
    resumen: ['crowdpromotion', 'wallet', 'resumen'] as const,
    transacciones: (filters?: WalletTransaccionesFilters) =>
        ['crowdpromotion', 'wallet', 'transacciones', filters] as const,
},
```

**Nota de import:** Agregar `WalletTransaccionesFilters` al import de types al inicio del archivo junto a `ExplorarProgramasFilters` e `InscripcionesFilters`.

### 4.2 API_ROUTES - promotorWallet (dentro de `crowdpromotion: { ... }`)

Insertar despues de `promotorMetricas` dentro del objeto `crowdpromotion` de `API_ROUTES`.

```typescript
promotorWallet: {
    resumen: '/api/crowdpromotion/promotor/wallet',
    transacciones: '/api/crowdpromotion/promotor/wallet/transacciones',
    cobro: '/api/crowdpromotion/promotor/wallet/cobro',
},
```

### 4.3 APP_ROUTES - landing.promotor (dentro de `landing.promotor: { ... }`)

Insertar dentro del objeto `promotor` en `APP_ROUTES.landing`.

```typescript
wallet: '/promotor/wallet',
```

### 4.4 VALIDATION - campos de wallet (dentro del objeto `VALIDATION`)

Insertar al final del bloque `VALIDATION`, despues de `METRICAS_EVENTOS_RECIENTES_MAX`.

```typescript
// Wallet y Cobros (US-CP-06)
WALLET_DESCRIPCION_COBRO_MAX: 500,
WALLET_TRANSACCIONES_PAGE_SIZE_MAX: 50,
```

### 4.5 Constantes nuevas de dominio (nivel raiz del archivo)

Estas constantes son independientes y se agregan al final del archivo, antes del `export * from './tracking'`.

```typescript
// ========== Crowdpromotion - Estado de Transaccion de Wallet (US-CP-06) ==========

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

**Alineacion de badges con el design system:**
- `1` Pendiente -> `'secondary'` (gris): igual que `INSCRIPCION_ESTADO_BADGE_VARIANT.Pendiente`
- `2` Procesada -> `'default'` (azul): igual que `INSCRIPCION_ESTADO_BADGE_VARIANT.Aprobado`
- `3` Pagada -> `'success'` (verde): igual que `ESTADO_TAREA_PROMO_BADGES[3]` (Validada)
- `4` Cancelada -> `'destructive'` (rojo): igual que `INSCRIPCION_ESTADO_BADGE_VARIANT.Bloqueado`

---

## 5. Utilidades (`src/shared/utils/error-messages.ts`)

**Accion:** Modificar - dos puntos de insercion.

### 5.1 Error Messages

| Patron | Descripcion |
|--------|-------------|
| `WALLET_ERROR_MESSAGES` | Record con claves numericas (alineadas con backend) y claves semanticas en mayusculas. Sigue exactamente el patron de `PROMOTOR_ERROR_MESSAGES`, `INSCRIPCION_ERROR_MESSAGES`, etc. |
| `getWalletErrorMessage` | Funcion wrapper que busca primero en `WALLET_ERROR_MESSAGES` y hace fallback a `getErrorMessage`. Firma identica a todas las demas funciones `get*ErrorMessage` del archivo |

### 5.2 Punto 1 - Agregar entradas a ERROR_CODE_MESSAGES global

Insertar en el objeto `ERROR_CODE_MESSAGES` dentro del bloque `// Not Found errors (2000-2999)` y `// Business Rule errors (4000-4999)`.

```typescript
// En bloque "Not Found errors (2000-2999)":
"2030": "No tienes un wallet asociado. Completa tu primera actividad para activarlo.",

// En bloque "Business Rule errors (4000-4999)":
"4040": "Saldo insuficiente para realizar el cobro solicitado.",
"4041": "El saldo disponible es inferior al minimo de retiro.",
"4042": "Ya existe una solicitud de cobro pendiente. Espera a que sea procesada.",
```

**Criterio de insercion:** `"2030"` va despues de `"2022"` (ultimo codigo 2xxx existente). Los codigos `"4040"`, `"4041"`, `"4042"` van despues de `"4032"` (ultimo codigo 4xxx existente).

### 5.3 Punto 2 - Agregar WALLET_ERROR_MESSAGES y getWalletErrorMessage

Insertar al final del archivo, despues de `getTareaPromocionErrorMessage`.

```typescript
// ========== Crowdpromotion - Wallet y Cobros Error Messages (US-CP-06) ==========

export const WALLET_ERROR_MESSAGES: Record<string, string> = {
    // Codigos numericos (override del ERROR_CODE_MESSAGES global para contexto wallet)
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

    // Claves semanticas para uso interno en hooks y componentes
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

### 5.4 Mappers (`src/shared/utils/mappers.ts`)

**Accion:** Modificar - agregar mappers de wallet al final del archivo.

No hay mappers de formulario complejos como en promotor (el formulario de cobro es simple con solo dos campos). Sin embargo, se planifican dos utilidades de mapeo de presentacion:

```typescript
// ========== Crowdpromotion - Wallet Mappers (US-CP-06) ==========

/**
 * Devuelve el variant del Badge de shadcn/ui para el estado de transaccion.
 * Valores: 'secondary' (Pendiente), 'default' (Procesada), 'success' (Pagada), 'destructive' (Cancelada)
 */
export function mapEstadoWalletTransaccionToBadge(estadoTransaccionId: number): string {
    return ESTADO_WALLET_TRANSACCION_BADGES[estadoTransaccionId] ?? 'secondary';
}

/**
 * Devuelve el icono/direccion visual para una transaccion de wallet.
 * 'credit' para ingresos (esCredito=true), 'debit' para retiros (esCredito=false).
 * Los componentes usan esto para aplicar color verde/rojo y signo +/-.
 */
export function mapTransaccionTipoToDisplayProps(esCredito: boolean): {
    tipo: 'credit' | 'debit';
    signo: '+' | '-';
    colorClass: string;
} {
    return esCredito
        ? { tipo: 'credit', signo: '+', colorClass: 'text-green-500' }
        : { tipo: 'debit', signo: '-', colorClass: 'text-red-500' };
}
```

**Imports necesarios en mappers.ts:** Agregar `ESTADO_WALLET_TRANSACCION_BADGES` al import desde `'../constants'`.

### 5.5 Format (`src/shared/utils/format.ts`)

**Accion:** Modificar - agregar formateadores de wallet al final del archivo.

```typescript
// ========== Crowdpromotion - Wallet Formatters (US-CP-06) ==========

/**
 * Formatea el saldo del wallet con la moneda del wallet.
 * Usa 2 decimales obligatorios (critico para importes monetarios de retiro).
 * @param importe - Importe a formatear
 * @param monedaNombre - Codigo ISO de la moneda (ej: 'EUR')
 * @returns String formateado (ej: '150,50 €')
 */
export function formatWalletImporte(
    importe: number,
    monedaNombre: string = 'EUR'
): string {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: monedaNombre,
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(importe);
}

/**
 * Formatea el importe de una transaccion con signo segun si es credito o debito.
 * @param importe - Importe absoluto (siempre positivo)
 * @param esCredito - true=ingreso (positivo), false=retiro (negativo)
 * @param monedaNombre - Codigo ISO de la moneda (ej: 'EUR')
 * @returns String formateado con signo (ej: '+10,00 €' o '-50,00 €')
 */
export function formatTransaccionImporte(
    importe: number,
    esCredito: boolean,
    monedaNombre: string = 'EUR'
): string {
    const formatted = formatWalletImporte(importe, monedaNombre);
    return esCredito ? `+${formatted}` : `-${formatted}`;
}
```

**Justificacion:** `formatWalletImporte` usa `minimumFractionDigits: 2` a diferencia de `formatCurrency` que usa `minimumFractionDigits: 0`. Los importes de wallet son monetarios precisos (ej: 150,50 EUR) y deben mostrar siempre dos decimales.

---

## 6. Archivos a Modificar

```
src/shared/
├── types/
│   └── crowdpromotion.ts          MODIFICAR - agregar 6 interfaces al final
├── schemas/
│   └── crowdpromotion.schema.ts   MODIFICAR - agregar 2 schemas + 2 types al final
├── constants/
│   └── index.ts                   MODIFICAR - 4 puntos de insercion:
│                                    1. QUERY_KEYS.crowdpromotion.wallet
│                                    2. API_ROUTES.crowdpromotion.promotorWallet
│                                    3. APP_ROUTES.landing.promotor.wallet
│                                    4. VALIDATION wallet fields
│                                    5. ESTADO_WALLET_TRANSACCION (al final, antes de re-export)
├── utils/
│   ├── error-messages.ts          MODIFICAR - 2 puntos de insercion:
│   │                                1. Entradas en ERROR_CODE_MESSAGES (2030, 4040-4042)
│   │                                2. WALLET_ERROR_MESSAGES + getWalletErrorMessage (al final)
│   ├── mappers.ts                 MODIFICAR - agregar 2 mappers al final
│   └── format.ts                  MODIFICAR - agregar 2 formatters al final
```

**Total: 6 archivos modificados, 0 archivos creados nuevos.**

---

## 7. Dependencias entre Archivos

El orden de implementacion importa para evitar errores de tipo en las fases de backend/frontend:

```
Paso 1: src/shared/types/crowdpromotion.ts
        (base: no depende de nada en shared)
        Agrega: PromotorWallet, WalletTransaccionItem, WalletTransaccionesPagedResponse,
                SolicitarCobroRequest, SolicitarCobroResponse, WalletTransaccionesFilters

Paso 2: src/shared/constants/index.ts
        (depende de: WalletTransaccionesFilters del Paso 1 para tipar QUERY_KEYS.wallet.transacciones)
        Agrega: QUERY_KEYS.wallet, API_ROUTES.promotorWallet, APP_ROUTES.promotor.wallet,
                VALIDATION campos, ESTADO_WALLET_TRANSACCION, LABELS, BADGES, MIN_RETIRO_WALLET,
                WALLET_DEFAULT_PAGE_SIZE

Paso 3: src/shared/schemas/crowdpromotion.schema.ts
        (depende de: z ya importado, fechasRefinement/fechasRefinementConfig del mismo archivo)
        Agrega: solicitarCobroSchema, walletTransaccionesFiltersSchema, types inferidos

Paso 4: src/shared/utils/error-messages.ts
        (depende de: getErrorMessage ya definido en el mismo archivo)
        Agrega: entradas en ERROR_CODE_MESSAGES, WALLET_ERROR_MESSAGES, getWalletErrorMessage

Paso 5: src/shared/utils/mappers.ts
        (depende de: ESTADO_WALLET_TRANSACCION_BADGES del Paso 2)
        Agrega: mapEstadoWalletTransaccionToBadge, mapTransaccionTipoToDisplayProps

Paso 6: src/shared/utils/format.ts
        (depende de: Paso 1 para documentacion de tipos, no depende de otros archivos shared)
        Agrega: formatWalletImporte, formatTransaccionImporte
```

---

## 8. Dependencias de Paquetes

- `zod` - ya instalado (usado en crowdpromotion.schema.ts)
- No se requiere ningun paquete adicional

---

## 9. Alineacion con Backend (contracts.md)

### Campos de respuesta - verificacion de nombres

| Campo backend (C# PascalCase) | Campo frontend (TS camelCase) | Tipo backend | Tipo frontend |
|-------------------------------|-------------------------------|--------------|---------------|
| `WalletId` | `walletId` | `Guid` | `string` |
| `MonedaId` | `monedaId` | `int` | `number` |
| `MonedaNombre` | `monedaNombre` | `string` | `string` |
| `SaldoDisponible` | `saldoDisponible` | `decimal` | `number` |
| `SaldoPendiente` | `saldoPendiente` | `decimal` | `number` |
| `TotalGanado` | `totalGanado` | `decimal` | `number` |
| `TotalRetirado` | `totalRetirado` | `decimal` | `number` |
| `MinimoRetiro` | `minimoRetiro` | `decimal` | `number` |
| `EsCredito` | `esCredito` | `bool` | `boolean` |
| `Importe` | `importe` | `decimal` | `number` |
| `Descripcion` | `descripcion` | `string?` | `string \| null` |
| `Concepto` | `concepto` | `string?` | `string \| null` |
| `EstadoTransaccionId` | `estadoTransaccionId` | `int` | `number` |
| `EstadoTransaccionNombre` | `estadoTransaccionNombre` | `string` | `string` |
| `TipoRewardId` | `tipoRewardId` | `int?` | `number \| null` |
| `TipoRewardNombre` | `tipoRewardNombre` | `string?` | `string \| null` |
| `PromoEventoId` | `promoEventoId` | `Guid?` | `string \| null` |
| `FechaCreacion` | `fechaCreacion` | `DateTime` | `string` (ISO 8601) |
| `FechaProcesado` | `fechaProcesado` | `DateTime?` | `string \| null` |
| `TransaccionId` | `transaccionId` | `Guid` | `string` |
| `SaldoRestante` | `saldoRestante` | `decimal` | `number` |

### Endpoints y constantes API alineadas

| Endpoint backend | Constante API_ROUTES | Metodo |
|-----------------|---------------------|--------|
| `GET /api/crowdpromotion/promotor/wallet` | `API_ROUTES.crowdpromotion.promotorWallet.resumen` | GET |
| `GET /api/crowdpromotion/promotor/wallet/transacciones` | `API_ROUTES.crowdpromotion.promotorWallet.transacciones` | GET |
| `POST /api/crowdpromotion/promotor/wallet/cobro` | `API_ROUTES.crowdpromotion.promotorWallet.cobro` | POST |

### Codigos de error alineados

| Backend ErrorCode | Constante backend | WALLET_ERROR_MESSAGES key | Mensaje |
|------------------|-------------------|--------------------------|---------|
| `1001` | `Validation_Required` | `'1001'` | El importe es obligatorio. |
| `1002` | `Validation_MaxLength` | `'1002'` | La descripcion no puede superar los 500 caracteres. |
| `1021` | `Validation_OutOfRange` | `'1021'` | El importe debe ser mayor que cero. |
| `1036` | `Validation_InvalidDateRange` | `'1036'` | La fecha de inicio no puede ser posterior a la fecha fin. |
| `2015` | `NotFound_Promotor` | `'2015'` | No tienes un perfil de promotor. |
| `2030` | `NotFound_Wallet` (nuevo) | `'2030'` | No tienes un wallet asociado. |
| `4040` | `BusinessRule_SaldoInsuficiente` (nuevo) | `'4040'` | Saldo insuficiente. |
| `4041` | `BusinessRule_SaldoBajoMinimoRetiro` (nuevo) | `'4041'` | Saldo bajo minimo de retiro. |
| `4042` | `BusinessRule_CobroConcurrente` (nuevo) | `'4042'` | Ya tienes una solicitud pendiente. |

---

## 10. Notas de Implementacion

1. **Patron de fechas ISO 8601:** Las fechas en los types se modelan como `string` (no `Date`), igual que en todos los types existentes de crowdpromotion. La conversion a `Date` se hace en los componentes de presentacion segun necesidad usando `formatDate()` de `src/shared/utils/format.ts`.

2. **`importe` es siempre positivo en el backend:** El campo `WalletTransaccionItem.importe` viene siempre positivo del backend. El signo visual (+ o -) se calcula en frontend usando `esCredito`. Por eso se planifica `formatTransaccionImporte(importe, esCredito, monedaNombre)` en `format.ts`.

3. **`minimoRetiro` viene del backend en la respuesta del wallet:** El backend incluye la constante `MinimoRetiro = 10.00` en el DTO del wallet. La constante `MIN_RETIRO_WALLET = 10.00` en frontend es una copia para validaciones client-side (soft check antes de enviar el request). Ambas deben estar sincronizadas.

4. **Validacion de saldo disponible en el schema:** El `solicitarCobroSchema` NO incluye validacion de `importe <= saldoDisponible` porque el schema no tiene acceso al estado del wallet. Esta validacion se hace en el hook (`useSolicitarCobro`) como soft check antes de llamar al API, y el backend retorna `4040` si falla.

5. **`estadoTransaccionId` range 1-4:** El `.max(4, 'Estado invalido')` en `walletTransaccionesFiltersSchema` esta hardcodeado porque la tabla `MaestraEstadoWalletTransaccion` tiene exactamente 4 valores fijos en MVP (seed fijo, no maestra dinamica).

6. **`WalletTransaccionesFilters` en el import de `constants/index.ts`:** El archivo actualmente importa `ExplorarProgramasFilters` e `InscripcionesFilters` desde `'../types/crowdpromotion'` en la linea 2. Hay que agregar `WalletTransaccionesFilters` a ese import para tipar correctamente `QUERY_KEYS.crowdpromotion.wallet.transacciones`.

7. **Convencion `as const` en ESTADO_WALLET_TRANSACCION:** Sigue el mismo patron que `ESTADO_NECESIDAD`, `ESTADO_PROPUESTA`, `ESTADO_ACUERDO` - objeto con `as const` para el enum, `Record<number, string>` para labels y badges (sin `as const` en los Records porque son lookup tables mutables por design).

8. **`getWalletErrorMessage` fallback a `getErrorMessage`:** A diferencia de `BACKING_ERROR_MESSAGES` que hace fallback a `BACKING_ERROR_MESSAGES["5000"]`, la nueva funcion hace fallback a `getErrorMessage(errorCode)` que busca en `ERROR_CODE_MESSAGES`. Esto es mas robusto y sigue el patron de `getPromotorErrorMessage`, `getPromoProgramaErrorMessage`, etc.

---

## 11. Checklist de Implementacion

- [ ] `src/shared/types/crowdpromotion.ts` - 6 interfaces agregadas al final con comentario de seccion
- [ ] `src/shared/constants/index.ts` - Import de `WalletTransaccionesFilters` agregado en linea 2
- [ ] `src/shared/constants/index.ts` - `QUERY_KEYS.crowdpromotion.wallet` agregado
- [ ] `src/shared/constants/index.ts` - `API_ROUTES.crowdpromotion.promotorWallet` agregado
- [ ] `src/shared/constants/index.ts` - `APP_ROUTES.landing.promotor.wallet` agregado
- [ ] `src/shared/constants/index.ts` - `VALIDATION.WALLET_DESCRIPCION_COBRO_MAX` y `WALLET_TRANSACCIONES_PAGE_SIZE_MAX` agregados
- [ ] `src/shared/constants/index.ts` - `ESTADO_WALLET_TRANSACCION`, `LABELS`, `BADGES`, `MIN_RETIRO_WALLET`, `WALLET_DEFAULT_PAGE_SIZE` agregados antes del `export * from './tracking'`
- [ ] `src/shared/schemas/crowdpromotion.schema.ts` - `solicitarCobroSchema` + `SolicitarCobroFormData` agregados
- [ ] `src/shared/schemas/crowdpromotion.schema.ts` - `walletTransaccionesFiltersSchema` + `WalletTransaccionesFiltersFormData` agregados (reutiliza `fechasRefinement`)
- [ ] `src/shared/utils/error-messages.ts` - Codigos `2030`, `4040`, `4041`, `4042` agregados a `ERROR_CODE_MESSAGES`
- [ ] `src/shared/utils/error-messages.ts` - `WALLET_ERROR_MESSAGES` con claves numericas y semanticas agregado
- [ ] `src/shared/utils/error-messages.ts` - `getWalletErrorMessage` agregado
- [ ] `src/shared/utils/mappers.ts` - Import de `ESTADO_WALLET_TRANSACCION_BADGES` agregado
- [ ] `src/shared/utils/mappers.ts` - `mapEstadoWalletTransaccionToBadge` agregado
- [ ] `src/shared/utils/mappers.ts` - `mapTransaccionTipoToDisplayProps` agregado
- [ ] `src/shared/utils/format.ts` - `formatWalletImporte` agregado
- [ ] `src/shared/utils/format.ts` - `formatTransaccionImporte` agregado
- [ ] Schemas Zod con mensajes en espanol y sin tildes en las constantes de texto
- [ ] Constantes de endpoints alineadas con rutas definidas en contracts.md
- [ ] Query keys siguen la jerarquia `['crowdpromotion', 'wallet', ...]`
- [ ] Error messages cubren todos los codigos de la tabla de errores de contracts.md
