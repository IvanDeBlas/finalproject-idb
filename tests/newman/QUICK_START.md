# Quick Start: WePlay.WalletComisiones.IntegrationTests

## Instalación de Newman (una vez)

```bash
npm install -g newman newman-reporter-htmlextra
```

## Ejecución Rápida

### Opción 1: Ejecución Básica (solo CLI)
```bash
cd /path/to/WePlay_Rises
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json
```

### Opción 2: Ejecución con Reporte HTML (recomendado)
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export tests/newman/reports/wallet-comisiones-$(date +%Y%m%d-%H%M%S).html
```

### Opción 3: Ejecución con Variables Personalizadas
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --environment tests/newman/env-local.json
```

### Opción 4: Ejecución Verbose (debugging)
```bash
newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
  --verbose
```

## Precondiciones Checklist

Antes de ejecutar, verificar:

- [ ] Backend corriendo en `http://localhost:5001`
- [ ] SQL Server conectado y BD sincronizada
- [ ] Usuario `usuario1@mail.com` registrado con perfil de promotor
- [ ] Wallet del promotor creada
- [ ] JWT funcional y configurado

## Estructura de Carpetas

```
tests/newman/
├── WePlay.WalletComisiones.IntegrationTests.json  ← Colección
├── QUICK_START.md                                  ← Este archivo
├── WALLET_COMISIONES_README.md                     ← Documentación completa
├── GENERATION_SUMMARY.md                           ← Generación de colección
├── reports/                                        ← HTML reports (generados)
│   └── wallet-comisiones-*.html
└── env-local.json                                  ← (opcional) Variables personalizadas
```

## Ejemplo: env-local.json

```json
{
  "id": "wallet-env-local",
  "name": "Wallet Comisiones - Local",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5001",
      "enabled": true
    },
    {
      "key": "promotorEmail",
      "value": "usuario1@mail.com",
      "enabled": true
    },
    {
      "key": "promotorPassword",
      "value": "123456",
      "enabled": true
    }
  ]
}
```

## Resultado Esperado

```
┌─────────────────────────────────────────────────────┐
│                  NEWMAN TEST RUN                   │
├─────────────────────────────────────────────────────┤
│ Collection: WePlay.WalletComisiones.IntegrationTests
│ Iteration: 1/1
│ Environment: (none)
│ Delay: 0ms
│ Timeout: 0ms
└─────────────────────────────────────────────────────┘

→ _Setup
  → 01. Login Promotor
    ✓ Status is 200
    ✓ Promotor token obtained

→ Wallet - GET Resumen
  → 01. GET /api/crowdpromotion/promotor/wallet
    ✓ Status is 200
    ✓ Response time < 500ms
    ✓ Response structure: PromotorWalletDto
    ✓ Valores logicos: saldoDisponible >= 0
    ✓ Valores logicos: totalGanado >= totalRetirado
    ✓ Minimo retiro = 10.00
    ✓ isSuccess = true

→ Wallet - GET Transacciones
  → 01. GET transacciones sin filtros (page=1, pageSize=10)
    ✓ Status is 200
    ✓ Response structure: WalletTransaccionesPagedDto
    ✓ Paginacion: page=1, pageSize=10
    ✓ Transacciones tienen estructura correcta
    ✓ isSuccess = true

  → 02. GET transacciones filtradas por esCredito=true
    ✓ Status is 200
    ✓ Todas las transacciones tienen esCredito=true
    ✓ isSuccess = true

  → 03. GET transacciones filtradas por estadoTransaccionId=1
    ✓ Status is 200
    ✓ Todas las transacciones tienen estadoTransaccionId=1
    ✓ isSuccess = true

  → 04. GET transacciones con rango de fechas
    ✓ Status is 200
    ✓ Transacciones estan dentro del rango de fechas
    ✓ isSuccess = true

→ Wallet - POST Cobro (Success)
  → 01. POST cobro valido (saldo suficiente)
    ✓ Status is 201 Created
    ✓ Response structure: SolicitarCobroResponseDto
    ✓ Importe cobro coincide con el solicitado
    ✓ Estado inicial es Pendiente
    ✓ Saldo restante = saldo anterior - importe
    ✓ Messages contiene success message (code 0001)
    ✓ isSuccess = true

  → 02. GET wallet para verificar saldo actualizado post-cobro
    ✓ Status is 200
    ✓ Saldo disponible se decrementó correctamente
    ✓ TotalRetirado se incrementó
    ✓ isSuccess = true

[... más resultados ...]

┌─────────────────────────────────────────────────────┐
│ SUMMARY                                             │
├─────────────────────────────────────────────────────┤
│ Requests: 20                                        │
│ Passed: 120 ✓                                       │
│ Failed: 0                                           │
│ Skipped: 0                                          │
│ Redirects: 0                                        │
│ Time: 35s                                           │
└─────────────────────────────────────────────────────┘
```

## Troubleshooting Rápido

### Error: ECONNREFUSED - Backend no responde
```bash
# Verificar que backend corre
curl http://localhost:5001/swagger
# Si no responde, iniciar backend en Visual Studio o Docker
```

### Error: 404 Wallet no encontrado
```
Solución: El promotor no tiene wallet.
Ejecutar antes: US-CP-01 (perfil-promotor) o US-CP-05 (tracking-metricas)
```

### Error: 401 Unauthorized
```
Solución: Token expirado o credenciales inválidas.
Verificar: usuario1@mail.com existe y está registrado como promotor
```

### Error: 409 Saldo insuficiente
```
Solución: El promotor no tiene saldo para cobro.
Ejecutar antes: US-CP-05 (tracking-metricas) para generar créditos
```

## Variables por Defecto

| Variable | Valor |
|----------|-------|
| baseUrl | http://localhost:5001 |
| promotorEmail | usuario1@mail.com |
| promotorPassword | 123456 |

Para cambiar: crear `env-local.json` y pasar `--environment tests/newman/env-local.json`

## CI/CD Integration Examples

### GitHub Actions
```yaml
name: Integration Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2019-latest
        env:
          SA_PASSWORD: TestPassword123!
          ACCEPT_EULA: Y
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: 18

      - run: npm install -g newman newman-reporter-htmlextra
      - run: |
          newman run tests/newman/WePlay.WalletComisiones.IntegrationTests.json \
            --environment tests/newman/env-ci.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export reports/wallet-comisiones.html

      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: newman-reports
          path: reports/
```

### Azure Pipelines
```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: NodeTool@0
    inputs:
      versionSpec: '18.x'

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
          --reporters cli,htmlextra \
          --reporter-htmlextra-export $(Build.ArtifactStagingDirectory)/wallet-comisiones.html

  - task: PublishBuildArtifacts@1
    inputs:
      pathToPublish: '$(Build.ArtifactStagingDirectory)'
      artifactName: 'newman-reports'
```

## Métricas Clave

| Métrica | Valor |
|---------|-------|
| Requests | 20 |
| Assertions | 120 |
| Casos exitosos | 8 |
| Casos de error | 9 |
| Tiempo promedio | 30-45 segundos |

## Documentación Completa

Para documentación detallada, ver:
- `WALLET_COMISIONES_README.md` - Guía completa
- `GENERATION_SUMMARY.md` - Especificaciones técnicas
- `plans/cp-wallet-comisiones/backend/api-contracts.md` - Contratos API

---

**Última actualización**: 2026-03-02
**Status**: ✅ Listo para ejecución
