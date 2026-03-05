# WePlay.WalletComisiones.IntegrationTests

Colección Postman Newman ejecutable para tests de integración de la feature **cp-wallet-comisiones (US-CP-06)**: Wallet de Promotor, Comisiones y Cobros.

## Descripción

Esta colección valida:
1. **GET /api/crowdpromotion/promotor/wallet** - Resumen de wallet (saldo, totales, minimo retiro)
2. **GET /api/crowdpromotion/promotor/wallet/transacciones** - Historial paginado con filtros
3. **POST /api/crowdpromotion/promotor/wallet/cobro** - Solicitud de cobro/retiro

## Precondiciones

- Backend corriendo en `http://localhost:5001`
- Base de datos sincronizada con migraciones (EF Core)
- Usuario de prueba existente: `usuario1@mail.com` / `123456`
- El usuario debe tener perfil de promotor con wallet creada

## Estructura de la Colección

```
_Setup
  └─ 01. Login Promotor (obtiene JWT token)

Wallet - GET Resumen
  └─ 01. GET /api/crowdpromotion/promotor/wallet (PromotorWalletDto)

Wallet - GET Transacciones
  ├─ 01. GET transacciones sin filtros (page=1, pageSize=10)
  ├─ 02. GET transacciones filtradas por esCredito=true (ingresos)
  ├─ 03. GET transacciones filtradas por estadoTransaccionId=1 (Pendiente)
  └─ 04. GET transacciones con rango de fechas

Wallet - POST Cobro (Success)
  ├─ 01. POST cobro válido (saldo suficiente) → 201 Created
  └─ 02. GET wallet para verificar saldo actualizado post-cobro

Wallet - Validation Errors
  ├─ 01. POST 400 - Importe requerido
  ├─ 02. POST 400 - Importe <= 0
  └─ 03. POST 400 - Descripcion > 500 caracteres

Wallet - Business Rule Errors
  └─ 01. POST 409 - Saldo insuficiente

Wallet - Auth Errors
  ├─ 01. GET 401 - Token ausente
  └─ 02. GET 401 - Token invalido

Wallet - Not Found
  ├─ 01. GET transacciones con paginacion invalida (page=0)
  ├─ 02. GET transacciones con pageSize > 50
  └─ 03. GET transacciones con rango de fechas invalido
```

## Ejecución

### Con Newman (CLI)

```bash
# Ejecución básica
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json

# Con reporte HTML
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export tests/newman/reports/wallet-comisiones.html

# Con variables de entorno personalizadas
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --environment tests/newman/env-local.json

# Especificar timeout
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --timeout 10000
```

### Dentro de Postman (GUI)

1. Importar el archivo `WePlay.WalletComisiones.IntegrationTests.json`
2. Seleccionar la colección en el sidebar
3. Click en "Run" (botón azul)
4. Configurar:
   - Environment: seleccionar o crear con `baseUrl: http://localhost:5001`
   - Iterations: 1
   - Delay: 100ms (entre requests)
5. Click en "Run WePlay.WalletComisiones.IntegrationTests"

## Variables de Colección

Todas las variables se configuran automáticamente en el _Setup o durante la ejecución:

| Variable | Descripción | Set by |
|----------|-------------|--------|
| `baseUrl` | URL base del API (default: http://localhost:5001) | Manual |
| `promotorEmail` | Email de usuario de prueba | _Setup |
| `promotorPassword` | Password de usuario de prueba | _Setup |
| `promotorToken` | JWT Bearer token | _Setup / Login |
| `walletId` | ID del wallet del promotor | GET Resumen |
| `monedaNombre` | Nombre de la moneda (ej: EUR) | GET Resumen |
| `saldoDisponible` | Saldo actual disponible para retiro | GET Resumen |
| `totalGanado` | Total histórico de ganancias | GET Resumen |
| `totalRetirado` | Total histórico de retiros | GET Resumen |
| `totalTransacciones` | Cantidad total de transacciones | GET Transacciones |
| `fechaDesde` | Fecha de inicio de filtro (ISO 8601) | GET Transacciones #04 |
| `fechaHasta` | Fecha de fin de filtro (ISO 8601) | GET Transacciones #04 |
| `importeCobro` | Importe a cobrar en POST cobro | POST Cobro #01 |
| `saldoPostCobro` | Saldo después del cobro | POST Cobro #01 |
| `transaccionCobroId` | ID de transacción creada por POST cobro | POST Cobro #01 |
| `importeExcesivo` | Importe que excede el saldo (para error 409) | Validation Errors #01 |
| `longDescription` | Descripción de 501+ caracteres (para error 400) | Validation Errors #03 |

## Métricas

- **Folders**: 7 (_Setup, Resumen, Transacciones, POST Success, Validation, Business Rules, Auth, Not Found)
- **Requests totales**: 20
- **GET requests**: 8
- **POST requests**: 12
- **Assertions**: ~120
- **Tiempo estimado**: 30-45 segundos

## Casos de Éxito Validados

✅ GET /promotor/wallet retorna 200 con estructura PromotorWalletDto completa
✅ GET /promotor/wallet/transacciones sin filtros retorna paginación correcta
✅ Filtro por esCredito=true retorna solo créditos
✅ Filtro por estadoTransaccionId=1 retorna solo Pendientes
✅ Filtro por rango de fechas respeta boundaries
✅ POST /promotor/wallet/cobro válido retorna 201 y decrementa saldo
✅ GET /promotor/wallet post-cobro refleja nuevo saldo
✅ Minimo retiro = 10.00

## Casos de Error Validados

❌ 400: Importe requerido (error code 1001)
❌ 400: Importe <= 0 (error code 1021)
❌ 400: Descripción > 500 chars (error code 1002)
❌ 400: Page = 0 (error code 1021)
❌ 400: PageSize > 50 (error code 1021)
❌ 400: Rango fechas inválido (error code 1036)
❌ 401: Token ausente (error code 3001)
❌ 401: Token inválido (error code 3001)
❌ 409: Saldo insuficiente (error code 4040)

## Dependencias de Datos

### Setup autosuficiente

- **Autenticación**: La colección crea el JWT en _Setup con usuario de prueba `usuario1@mail.com`
- **Wallet**: Se asume que el promotor ya tiene wallet creada (creada en US-CP-01, US-CP-03, US-CP-04 o US-CP-05)
- **Transacciones**: La colección lee transacciones existentes; no requiere crear datos previos

### Asunciones

1. El usuario `usuario1@mail.com` existe en la BD y tiene perfil de promotor activo
2. La wallet del promotor existe con al menos un saldo de 15 EUR (para cobro exitoso de 10-15 EUR)
3. Las tablas maestras de estado de transacción están pobladas (1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada)

## Notas

- La colección NO limpia datos después de ejecutarse (las transacciones son inmutables)
- Cada ejecución crea un nuevo cobro (request de POST cobro), incrementando el historial de transacciones
- El importe de cobro se calcula dinámicamente como `min(saldo * 0.5, saldo)` pero mínimo 10 EUR
- Los timestamps de transacciones usan la hora del servidor (UTC)
- La colección es 100% auto-inclusiva: no requiere datos precargados más allá del usuario de prueba y su wallet

## Troubleshooting

### 404 - Wallet no encontrado
**Causa**: El usuario `usuario1@mail.com` no tiene wallet o el perfil de promotor no existe.
**Solución**: Ejecutar US-CP-01 (cp-perfil-promotor) o verificar que el usuario está registrado como promotor.

### 401 - Token expirado
**Causa**: El JWT obtenido en _Setup ha expirado (lifetime muy corto en test).
**Solución**: Aumentar el lifetime del JWT en appsettings o re-ejecutar _Setup.

### 409 - Saldo insuficiente
**Causa**: El saldo disponible es menor que el importe de prueba (10 EUR).
**Solución**: Registrar conversiones o tareas validadas (US-CP-05, US-CP-04) para acumular saldo.

### Paginación vacía
**Causa**: El promotor no tiene transacciones registradas.
**Solución**: Normal en primera ejecución; ejecutar US-CP-05 (tracking-metricas) para generar créditos.

## CI/CD Integration

### GitHub Actions

```yaml
- name: Run Wallet Comisiones Integration Tests
  run: |
    npm install -g newman newman-reporter-htmlextra
    newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
      --environment tests/newman/env-ci.json \
      --reporters cli,htmlextra \
      --reporter-htmlextra-export reports/wallet-comisiones.html
```

### Azure Pipelines

```yaml
- task: Npm@1
  inputs:
    command: 'custom'
    customCommand: 'install -g newman newman-reporter-htmlextra'

- task: Bash@3
  inputs:
    targetType: 'inline'
    script: |
      newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
        --environment tests/newman/env-ci.json \
        --reporters cli,htmlextra
```

---

**Última actualización**: 2026-03-02
**Feature**: cp-wallet-comisiones (US-CP-06)
**Status**: Ejecutable en MVP
