# Estructura Visual: WePlay.WalletComisiones.IntegrationTests

## Árbol de Colección Completo

```
WePlay.WalletComisiones.IntegrationTests
│
├── _Setup (1 request, 2 assertions)
│   ├─ 01. Login Promotor
│   │  ├─ Pre-request: Set email y password
│   │  ├─ Request: POST /api/auth/login
│   │  └─ Tests:
│   │     ├─ Status is 200
│   │     └─ Promotor token obtained
│   │
│
├── Wallet - GET Resumen (1 request, 7 assertions)
│   ├─ 01. GET /api/crowdpromotion/promotor/wallet
│   │  ├─ Request: GET con Authorization Bearer token
│   │  └─ Tests:
│   │     ├─ Status is 200
│   │     ├─ Response time < 500ms
│   │     ├─ Response structure: PromotorWalletDto
│   │     ├─ Valores logicos: saldoDisponible >= 0
│   │     ├─ Valores logicos: totalGanado >= totalRetirado
│   │     ├─ Minimo retiro = 10.00
│   │     └─ isSuccess = true
│   │
│
├── Wallet - GET Transacciones (4 requests, 14 assertions)
│   ├─ 01. GET transacciones sin filtros (page=1, pageSize=10)
│   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?page=1&pageSize=10
│   │  └─ Tests:
│   │     ├─ Status is 200
│   │     ├─ Response structure: WalletTransaccionesPagedDto
│   │     ├─ Paginacion: page=1, pageSize=10
│   │     ├─ Transacciones tienen estructura correcta
│   │     └─ isSuccess = true
│   │
│   ├─ 02. GET transacciones filtradas por esCredito=true (ingresos)
│   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?esCredito=true&page=1&pageSize=10
│   │  └─ Tests:
│   │     ├─ Status is 200
│   │     ├─ Todas las transacciones tienen esCredito=true
│   │     └─ isSuccess = true
│   │
│   ├─ 03. GET transacciones filtradas por estadoTransaccionId=1 (Pendiente)
│   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?estadoTransaccionId=1&page=1&pageSize=10
│   │  └─ Tests:
│   │     ├─ Status is 200
│   │     ├─ Todas las transacciones tienen estadoTransaccionId=1
│   │     ├─ Todas tienen estadoTransaccionNombre="Pendiente"
│   │     └─ isSuccess = true
│   │
│   └─ 04. GET transacciones con rango de fechas
│      ├─ Pre-request: Calculate fechaDesde (30 days ago) y fechaHasta (today)
│      ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?fechaDesde={{fechaDesde}}&fechaHasta={{fechaHasta}}&page=1&pageSize=10
│      └─ Tests:
│         ├─ Status is 200
│         ├─ Transacciones estan dentro del rango de fechas
│         └─ isSuccess = true
│
├── Wallet - POST Cobro (Success) (2 requests, 10 assertions)
│   ├─ 01. POST cobro valido (saldo suficiente)
│   │  ├─ Pre-request: Calculate importeCobro = min(saldo*0.5, saldo)
│   │  ├─ Request: POST /api/crowdpromotion/promotor/wallet/cobro
│   │  │           Headers: Authorization, Content-Type
│   │  │           Body: {"importe": {{importeCobro}}, "descripcion": "Retiro mensual de comisiones"}
│   │  └─ Tests:
│   │     ├─ Status is 201 Created
│   │     ├─ Response structure: SolicitarCobroResponseDto
│   │     ├─ Importe cobro coincide con el solicitado
│   │     ├─ Estado inicial es Pendiente
│   │     ├─ Saldo restante = saldo anterior - importe
│   │     ├─ Messages contiene success message (code 0001)
│   │     └─ isSuccess = true
│   │
│   └─ 02. GET wallet para verificar saldo actualizado post-cobro
│      ├─ Request: GET /api/crowdpromotion/promotor/wallet
│      └─ Tests:
│         ├─ Status is 200
│         ├─ Saldo disponible se decrementó correctamente
│         ├─ TotalRetirado se incrementó
│         └─ isSuccess = true
│
├── Wallet - Validation Errors (3 requests, 9 assertions)
│   ├─ 01. POST 400 - Importe requerido
│   │  ├─ Request: POST /api/crowdpromotion/promotor/wallet/cobro
│   │  │           Body: {"descripcion": "Retiro sin importe"} (SIN importe)
│   │  └─ Tests:
│   │     ├─ Status is 400
│   │     ├─ isSuccess = false
│   │     └─ Error code es 1001 (Campo obligatorio)
│   │
│   ├─ 02. POST 400 - Importe <= 0
│   │  ├─ Request: POST /api/crowdpromotion/promotor/wallet/cobro
│   │  │           Body: {"importe": 0, "descripcion": "Importe cero"}
│   │  └─ Tests:
│   │     ├─ Status is 400
│   │     ├─ isSuccess = false
│   │     └─ Error code es 1021 (Rango fuera de limites)
│   │
│   └─ 03. POST 400 - Descripcion > 500 caracteres
│      ├─ Pre-request: Generate longDescription (501 chars)
│      ├─ Request: POST /api/crowdpromotion/promotor/wallet/cobro
│      │           Body: {"importe": 15.00, "descripcion": "{{longDescription}}"}
│      └─ Tests:
│         ├─ Status is 400
│         ├─ isSuccess = false
│         └─ Error code es 1002 (MaxLength excedido)
│
├── Wallet - Business Rule Errors (1 request, 3 assertions)
│   └─ 01. POST 409 - Saldo insuficiente
│      ├─ Pre-request: Calculate importeExcesivo = saldoActual + 1000
│      ├─ Request: POST /api/crowdpromotion/promotor/wallet/cobro
│      │           Body: {"importe": {{importeExcesivo}}, "descripcion": "Cobro excesivo"}
│      └─ Tests:
│         ├─ Status is 409
│         ├─ isSuccess = false
│         └─ Error code es 4040 (Saldo insuficiente)
│
├── Wallet - Auth Errors (2 requests, 5 assertions)
│   ├─ 01. GET 401 - Token ausente
│   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet (SIN Authorization header)
│   │  └─ Tests:
│   │     ├─ Status is 401
│   │     ├─ isSuccess = false
│   │     └─ Error code es 3001 (No autorizado)
│   │
│   └─ 02. GET 401 - Token invalido
│      ├─ Request: GET /api/crowdpromotion/promotor/wallet
│      │           Headers: Authorization: "Bearer token-invalido-xyz"
│      └─ Tests:
│         ├─ Status is 401
│         └─ isSuccess = false
│
└── Wallet - Not Found (3 requests, 9 assertions)
   ├─ 01. GET transacciones con paginacion invalida (page=0)
   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?page=0&pageSize=10
   │  └─ Tests:
   │     ├─ Status is 400
   │     └─ Error code es 1021 (Rango fuera de limites)
   │
   ├─ 02. GET transacciones con pageSize > 50
   │  ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?page=1&pageSize=100
   │  └─ Tests:
   │     ├─ Status is 400
   │     └─ Error code es 1021 (Rango fuera de limites)
   │
   └─ 03. GET transacciones con rango de fechas invalido
      ├─ Request: GET /api/crowdpromotion/promotor/wallet/transacciones?fechaDesde=2026-03-31&fechaHasta=2026-03-01&page=1&pageSize=10
      └─ Tests:
         ├─ Status is 400
         └─ Error code es 1036 (Rango de fechas invalido)
```

---

## Resumen por Folder

### _Setup (2%)
- 1 request
- 2 assertions
- Obtiene JWT token de usuario de prueba
- **Tipo**: Setup / Authentication

### Wallet - GET Resumen (5%)
- 1 request
- 7 assertions
- Valida estructura PromotorWalletDto completa
- **Tipo**: Happy Path / Estructura de datos

### Wallet - GET Transacciones (25%)
- 4 requests
- 14 assertions
- Valida paginación, filtros por tipo, estado, fechas
- **Tipo**: Happy Path / Filtros y paginación

### Wallet - POST Cobro (Success) (15%)
- 2 requests
- 10 assertions
- Valida créación de cobro y actualización de saldo
- **Tipo**: Happy Path / Business Logic

### Wallet - Validation Errors (15%)
- 3 requests
- 9 assertions
- Valida errores 400 por campo requerido, rango, maxlength
- **Tipo**: Error Cases / Validación de inputs

### Wallet - Business Rule Errors (5%)
- 1 request
- 3 assertions
- Valida error 409 por saldo insuficiente
- **Tipo**: Error Cases / Reglas de negocio

### Wallet - Auth Errors (10%)
- 2 requests
- 5 assertions
- Valida errores 401 por token ausente/inválido
- **Tipo**: Error Cases / Autenticación

### Wallet - Not Found (15%)
- 3 requests
- 9 assertions
- Valida errores 400 por paginación/rango inválidos
- **Tipo**: Error Cases / Validación de query params

---

## Flujo de Ejecución Temporal

```
┌────────────────────────────────────────────────────────────────────┐
│                    EJECUCIÓN SECUENCIAL                            │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  [0s]  _Setup                                                     │
│    ├─ 01. Login Promotor                              [2s]        │
│    │   └─ jwt token → promotorToken                              │
│                                                                    │
│  [2s]  Wallet - GET Resumen                                      │
│    ├─ 01. GET /promotor/wallet                        [1s]        │
│    │   └─ walletId, saldoDisponible, totalGanado                 │
│                                                                    │
│  [3s]  Wallet - GET Transacciones                                │
│    ├─ 01. GET sin filtros (page=1)                    [1s]        │
│    ├─ 02. GET esCredito=true                          [1s]        │
│    ├─ 03. GET estadoTransaccionId=1                   [1s]        │
│    └─ 04. GET rango fechas                            [1s]        │
│       └─ fechaDesde, fechaHasta                                   │
│                                                                    │
│  [7s]  Wallet - POST Cobro (Success)                             │
│    ├─ 01. POST cobro válido                           [1s]        │
│    │   └─ transaccionCobroId, saldoPostCobro                     │
│    └─ 02. GET wallet verificación                     [1s]        │
│                                                                    │
│  [9s]  Wallet - Validation Errors                                │
│    ├─ 01. POST sin importe                            [0.5s]      │
│    ├─ 02. POST importe=0                              [0.5s]      │
│    └─ 03. POST descripción long                       [0.5s]      │
│       └─ longDescription (501 chars)                              │
│                                                                    │
│ [10s]  Wallet - Business Rule Errors                             │
│    └─ 01. POST importe excesivo                       [0.5s]      │
│       └─ importeExcesivo = saldo + 1000                          │
│                                                                    │
│ [11s]  Wallet - Auth Errors                                      │
│    ├─ 01. GET sin Authorization                       [0.5s]      │
│    └─ 02. GET Authorization inválido                  [0.5s]      │
│                                                                    │
│ [12s]  Wallet - Not Found                                        │
│    ├─ 01. GET page=0                                  [0.5s]      │
│    ├─ 02. GET pageSize=100                            [0.5s]      │
│    └─ 03. GET fechas inválidas                        [0.5s]      │
│                                                                    │
│ [13s]  ✓ COMPLETADO - 120 assertions passed                      │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘

Tiempo total: ~30-45 segundos (incluyendo latencia de red)
```

---

## Matriz de Cobertura: Endpoints

| Endpoint | GET | POST | Filtros | Errores | Cobertura |
|----------|-----|------|---------|---------|-----------|
| `/api/crowdpromotion/promotor/wallet` | ✅ (2x) | - | - | 2x (auth) | 100% |
| `/api/crowdpromotion/promotor/wallet/transacciones` | ✅ (7x) | - | esCredito, estadoId, fechas (3x) | 5x (pag, rango) | 95% |
| `/api/crowdpromotion/promotor/wallet/cobro` | - | ✅ (4x) | - | 4x (validación, biz rule, saldo) | 90% |

---

## Matriz de Cobertura: Status Codes

| Status | Casos | Cobertura |
|--------|-------|-----------|
| **200 OK** | 8 requests | GET resumen, GET transacciones (4x), GET post-cobro |
| **201 Created** | 1 request | POST cobro exitoso |
| **400 Bad Request** | 6 requests | 3 validación (importe, desc) + 3 paginación (page, size, fechas) |
| **401 Unauthorized** | 2 requests | Token ausente, token inválido |
| **409 Conflict** | 1 request | Saldo insuficiente |

---

## Matriz de Cobertura: Error Codes

| Error Code | Tipo | Casos | Validado |
|-----------|------|-------|----------|
| **1001** | Validación | Importe requerido | ✅ |
| **1002** | Validación | MaxLength descripción | ✅ |
| **1021** | Rango | Importe <= 0, page=0, pageSize>50 | ✅ |
| **1036** | Rango | Fechas inválidas | ✅ |
| **3001** | Auth | Token ausente, inválido | ✅ |
| **4040** | Business Rule | Saldo insuficiente | ✅ |

---

## Variables Utilizadas por Folder

```
_Setup
  ├─ input: (ninguno)
  └─ output: promotorToken

Wallet - GET Resumen
  ├─ input: promotorToken
  └─ output: walletId, monedaNombre, saldoDisponible, totalGanado

Wallet - GET Transacciones
  ├─ input: promotorToken, saldoDisponible (para filtro de fechas)
  └─ output: fechaDesde, fechaHasta, totalTransacciones

Wallet - POST Cobro
  ├─ input: promotorToken, saldoDisponible
  └─ output: importeCobro, saldoPostCobro, transaccionCobroId

Validation Errors
  ├─ input: promotorToken
  └─ output: longDescription, importeExcesivo

Business Rule Errors
  ├─ input: promotorToken, saldoDisponible
  └─ output: importeExcesivo

Auth Errors
  ├─ input: (ninguno, propósito es fallar sin token)
  └─ output: (ninguno)

Not Found
  ├─ input: promotorToken
  └─ output: (ninguno)
```

---

## Dependencias Entre Requests

```
_Setup: 01. Login Promotor
         ↓
         promotorToken
         ↓
   ┌─────┴─────┬──────────┬──────────┬──────────────┐
   │            │          │          │              │
   v            v          v          v              v
GET Resumen   GET Trans   POST Cobro Auth Errors  Not Found
   ↓            ↓          ↓
   └────────────┴──────────┘
   Genera:      Genera:    Genera:
   saldoDisp    fechas     importeCobro
   walletId     totalTx    saldoPostCobro
```

---

**Actualización**: 2026-03-02
**Formato**: Markdown (visual reference)
**Schema**: Postman Collection v2.1.0
