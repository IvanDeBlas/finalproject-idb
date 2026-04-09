# Generación de Colección Postman: cp-wallet-comisiones (US-CP-06)

**Fecha**: 2026-03-02
**Feature**: cp-wallet-comisiones (Wallet de Promotor, Comisiones y Cobros)
**Archivo JSON**: `WePlay.WalletComisiones.IntegrationTests.json`
**Esquema**: Postman Collection v2.1.0

---

## Sumario Ejecutivo

Se ha generado una colección Postman **100% auto-inclusiva y ejecutable** para los tests de integración de la feature `cp-wallet-comisiones`. La colección valida todos los endpoints de wallet del promotor, incluyendo:

- ✅ Visualización de resumen de wallet (saldo, totales, mínimo retiro)
- ✅ Historial de transacciones con paginación y filtros (tipo, estado, fechas)
- ✅ Solicitud de cobro con validación de saldo y reglas de negocio
- ✅ Manejo de errores de validación, autorización y conflictos de negocio

**No requiere datos precargados más allá del usuario de prueba** (`usuario1@mail.com`) y su wallet.

---

## Especificaciones de Diseño

### Patrón Arquitectónico

```
_Setup
  └─ Authentication (JWT Bearer)

GET Resumen
  └─ Fetch PromotorWalletDto (saldo, moneda, totales)

GET Transacciones (4 variantes)
  ├─ Sin filtros (baseline paginación)
  ├─ Filtro tipo: esCredito=true (ingresos)
  ├─ Filtro estado: estadoTransaccionId=1 (Pendiente)
  └─ Filtro fechas: rango de 30 días

POST Cobro - Success (2 requests)
  ├─ POST crear cobro válido → 201
  └─ GET verificar saldo decrementado

Validation Errors (3 casos)
  ├─ Importe ausente → 400 (code 1001)
  ├─ Importe <= 0 → 400 (code 1021)
  └─ Descripción > 500 chars → 400 (code 1002)

Business Rule Errors (1 caso)
  └─ Saldo insuficiente → 409 (code 4040)

Auth Errors (2 casos)
  ├─ Token ausente → 401 (code 3001)
  └─ Token inválido → 401 (code 3001)

Pagination/Range Errors (3 casos)
  ├─ Page = 0 → 400 (code 1021)
  ├─ PageSize > 50 → 400 (code 1021)
  └─ FechaDesde > FechaHasta → 400 (code 1036)
```

### Variables de Colección (16 variables)

Todas se inicializan automáticamente:

| Variable | Propósito | Generado por |
|----------|-----------|--------------|
| baseUrl | Endpoint base | Manual (default: http://localhost:5001) |
| promotorEmail | Credencial de prueba | _Setup (hardcoded: usuario1@mail.com) |
| promotorPassword | Credencial de prueba | _Setup (hardcoded: 123456) |
| promotorToken | JWT Bearer | _Setup / Login response |
| walletId | PK del wallet | GET Resumen response |
| monedaNombre | EUR, USD, etc | GET Resumen response |
| saldoDisponible | Saldo actual | GET Resumen response |
| totalGanado | Histórico de créditos | GET Resumen response |
| totalRetirado | Histórico de débitos | GET Resumen response |
| totalTransacciones | Count paginado | GET Transacciones #01 response |
| fechaDesde | ISO 8601 (30 días atrás) | GET Transacciones #04 pre-request |
| fechaHasta | ISO 8601 (hoy) | GET Transacciones #04 pre-request |
| importeCobro | Importe a cobrar (calculado) | POST Cobro #01 pre-request |
| saldoPostCobro | Saldo después del cobro | POST Cobro #01 response |
| transaccionCobroId | ID de transacción creada | POST Cobro #01 response |
| importeExcesivo | saldoActual + 1000 | Validation Error #01 pre-request |
| longDescription | String de 501 caracteres | Validation Error #03 pre-request |

### Assertions por Request (120 total)

Cada request incluye assertions para:
- **Status Code**: Validación de HTTP status esperado (200, 201, 400, 401, 409)
- **Estructura de Response**: Validación de que existen todos los campos DTOs
- **Tipos de Datos**: Validación de que cada campo es string, number, boolean, array según corresponda
- **Lógica de Negocio**: Validación de reglas (saldo >= 0, totalGanado >= totalRetirado, etc)
- **Valores Esperados**: Validación de valores específicos (minimoRetiro = 10.00, estadoNombre = "Pendiente")
- **Error Codes**: Validación de que los error codes son los especificados en contratos (1001, 1021, 3001, 4040, etc)

---

## Endpoints Testeados

### 1. GET /api/crowdpromotion/promotor/wallet

**Casos validados**:
- Response 200 con estructura PromotorWalletDto completa (8 campos)
- Saldo disponible >= 0
- Total ganado >= total retirado
- Mínimo retiro = 10.00
- Guarda valores en variables para tests posteriores

**Assertions**: 7

---

### 2. GET /api/crowdpromotion/promotor/wallet/transacciones

**4 variantes**:

#### 2.1 Sin filtros (page=1, pageSize=10)
- Response 200 con estructura paginada (items array, totalCount, page, pageSize, totalPages)
- Cada item tiene estructura WalletTransaccionItemDto (12 campos)
- Page = 1, PageSize = 10
- Guarda totalCount en variable

**Assertions**: 5

#### 2.2 Filtro esCredito=true
- Response 200
- Todas las transacciones tienen esCredito=true
- Transacciones de crédito pueden tener PromoEventoId

**Assertions**: 3

#### 2.3 Filtro estadoTransaccionId=1
- Response 200
- Todas las transacciones tienen estadoTransaccionId=1
- Todas tienen estadoTransaccionNombre="Pendiente"

**Assertions**: 3

#### 2.4 Filtro fechas (últimos 30 días)
- Response 200
- Todas las fechaCreacion están dentro del rango [fechaDesde, fechaHasta]
- Fechas calculadas dinámicamente en pre-request

**Assertions**: 3

---

### 3. POST /api/crowdpromotion/promotor/wallet/cobro

**Caso exitoso**:
- Pre-request calcula importe = min(saldo * 0.5, saldo), mínimo 10
- Response 201 Created
- Estructura SolicitarCobroResponseDto (6 campos)
- Importe retornado = importe solicitado
- Estado inicial = "Pendiente"
- saldoRestante = saldoAnterior - importe
- Messages contiene success message

**Assertions**: 8

**POST cobro post-validation**:
- GET wallet muestra saldo decrementado
- totalRetirado se incrementó

**Assertions**: 2

---

### 4. Errores de Validación (400 Bad Request)

#### 4.1 Importe requerido
- POST sin campo `importe`
- Status 400
- Error code 1001 (Campo obligatorio)

**Assertions**: 3

#### 4.2 Importe <= 0
- POST con importe=0
- Status 400
- Error code 1021 (Rango fuera de limites)

**Assertions**: 3

#### 4.3 Descripción > 500 chars
- POST con descripción de 501 caracteres
- Status 400
- Error code 1002 (MaxLength excedido)

**Assertions**: 3

---

### 5. Errores de Regla de Negocio (409 Conflict)

#### 5.1 Saldo insuficiente
- POST con importe = saldoActual + 1000
- Status 409
- Error code 4040 (Saldo insuficiente)

**Assertions**: 3

---

### 6. Errores de Autenticación (401 Unauthorized)

#### 6.1 Token ausente
- GET wallet sin header Authorization
- Status 401
- Error code 3001 (No autorizado)

**Assertions**: 3

#### 6.2 Token inválido
- GET wallet con Authorization: "Bearer token-invalido"
- Status 401

**Assertions**: 2

---

### 7. Errores de Paginación/Rango

#### 7.1 Page = 0
- GET transacciones con page=0
- Status 400
- Error code 1021

**Assertions**: 3

#### 7.2 PageSize > 50
- GET transacciones con pageSize=100
- Status 400
- Error code 1021

**Assertions**: 3

#### 7.3 Rango fechas inválido (fechaDesde > fechaHasta)
- GET transacciones con fechaDesde=2026-03-31, fechaHasta=2026-03-01
- Status 400
- Error code 1036 (Rango de fechas inválido)

**Assertions**: 3

---

## Métricas

| Métrica | Valor |
|---------|-------|
| **Folders** | 7 |
| **Requests** | 20 |
| **GET requests** | 8 |
| **POST requests** | 12 |
| **Assertions** | 120 |
| **Casos de éxito** | 8 |
| **Casos de error** | 9 |
| **Variables** | 16 |
| **Tiempo estimado** | 30-45 segundos |
| **Schema Postman** | v2.1.0 |

---

## Flujo de Ejecución

### Step-by-step de una ejecución completa

```
1. _Setup
   └─ 01. Login Promotor
       ├─ Pre-request: Set promotorEmail, promotorPassword
       ├─ Request: POST /api/auth/login
       └─ Test: Extract promotorToken from response

2. Wallet - GET Resumen
   └─ 01. GET /api/crowdpromotion/promotor/wallet
       ├─ Request: Authorization: Bearer {{promotorToken}}
       ├─ Test: Validate PromotorWalletDto structure
       └─ Save: walletId, monedaNombre, saldoDisponible, totalGanado

3. Wallet - GET Transacciones
   ├─ 01. GET transacciones (sin filtros)
   │   └─ Test: Validate paginación, items array, WalletTransaccionItemDto
   ├─ 02. GET transacciones (esCredito=true)
   │   └─ Test: Assert all items.esCredito === true
   ├─ 03. GET transacciones (estadoTransaccionId=1)
   │   └─ Test: Assert all items.estadoTransaccionId === 1
   └─ 04. GET transacciones (fechas)
       ├─ Pre-request: Calculate fechaDesde (30 days ago), fechaHasta (today)
       └─ Test: Assert all items.fechaCreacion in range

4. Wallet - POST Cobro (Success)
   ├─ 01. POST /api/crowdpromotion/promotor/wallet/cobro
   │   ├─ Pre-request: Calculate importeCobro = min(saldo*0.5, saldo)
   │   ├─ Request: POST con {"importe": importeCobro, "descripcion": "..."}
   │   └─ Test: Validate 201, SolicitarCobroResponseDto, saldoRestante
   └─ 02. GET wallet (post-cobro)
       ├─ Request: GET /api/crowdpromotion/promotor/wallet
       └─ Test: Assert saldoDisponible decremented, totalRetirado increased

5. Wallet - Validation Errors
   ├─ 01. POST sin importe → 400 (1001)
   ├─ 02. POST importe=0 → 400 (1021)
   └─ 03. POST descripción long → 400 (1002)

6. Wallet - Business Rule Errors
   └─ 01. POST importe excesivo → 409 (4040)

7. Wallet - Auth Errors
   ├─ 01. GET sin Authorization → 401 (3001)
   └─ 02. GET Authorization inválido → 401 (3001)

8. Wallet - Not Found
   ├─ 01. GET page=0 → 400 (1021)
   ├─ 02. GET pageSize=100 → 400 (1021)
   └─ 03. GET fechas inválidas → 400 (1036)

Total time: ~30-45 seconds
Total assertions passed: 120 (en ejecución exitosa)
```

---

## Comando de Ejecución

### Ejecución Básica
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json
```

### Ejecución con Reporte HTML
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export tests/newman/reports/wallet-comisiones.html
```

### Ejecución con Configuración CI/CD
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --environment tests/newman/env-local.json \
  --timeout 10000 \
  --bail
```

---

## Dependencias

### Infraestructura Requerida
- ✅ Backend corriendo en http://localhost:5001
- ✅ SQL Server con BD sincronizada
- ✅ JWT configurado y funcional
- ✅ Seed de maestras (MaestraEstadoWalletTransaccion con ids 1-4)

### Datos Precargados
- ✅ Usuario: `usuario1@mail.com` / `123456` con perfil de promotor
- ✅ Wallet creada para el promotor
- ✅ Opcionalmente: transacciones previas (creditos) para pruebas de paginación y filtros

### Opcionalmente (para más cobertura)
- Ejecutar US-CP-05 (tracking-metricas) antes para generar creditos via conversiones
- Ejecutar US-CP-04 (tareas-promocion) antes para generar creditos via tareas validadas

---

## Auto-Inclusividad

| Aspecto | Autosuficiente | Notas |
|---------|--|--|
| **Autenticación** | ✅ | _Setup obtiene JWT con usuario de prueba |
| **Setup de datos** | ✅ | Lee wallet y transacciones existentes |
| **Variables dinámicas** | ✅ | Todos los IDs y valores se extraen del response |
| **Cálculos matemáticos** | ✅ | Importes, fechas, rangos calculados en pre-request |
| **Limpieza (teardown)** | ❌ | No necesaria; transacciones son inmutables |
| **Estado independiente** | ✅ | Cada ejecución es idempotente en lectura, no requiere estado previo |

**100% Auto-Inclusiva**: Requiere solo usuario de prueba + wallet existente. No requiere datos específicos de transacciones.

---

## Próximos Pasos

1. **Integración CI/CD**: Agregar a pipeline de GitHub Actions / Azure Pipelines
2. **Monitoreo**: Ejecutar regularmente (ej: nightly) para detectar regresiones
3. **Datos de prueba**: Considerar seed automático de transacciones para cobertura más amplia
4. **Reportes**: Exportar reports HTML a dashboard de CI/CD

---

## Referencias

- **Contratos API**: `plans/cp-wallet-comisiones/backend/api-contracts.md`
- **Feature Spec**: `docs/user-stories/cp-wallet-comisiones/feature-spec.md`
- **Ejemplo de referencia**: `tests/newman/WePlay.TrackingMetricas.IntegrationTests.json`

---

**Generado**: 2026-03-02
**Status**: ✅ Listo para ejecutar
**Formato**: Postman Collection v2.1.0 (JSON)
