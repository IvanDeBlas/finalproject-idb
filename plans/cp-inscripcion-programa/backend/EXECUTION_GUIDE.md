# Guia de Ejecucion - cp-inscripcion-programa Integration Tests

## Modos de Ejecucion

### 1. Modo: Desarrollo (Postman GUI)

**Cuando usar**: Desarrollo local, debugging, exploracion

```bash
# Requisitos
- Postman instalado
- Backend ejecutandose: http://localhost:5001
- BD con datos de prueba

# Pasos
1. File → Import
2. Seleccionar: plans/cp-inscripcion-programa/backend/postman-collection.json
3. En la coleccion, hacer click en "Run"
4. Ejecutar carpetas en orden:
   - _Setup (generar tokens)
   - Inscripcion - Happy Path (flujo principal)
   - Resto de carpetas segun se necesite
```

**Output**: UI interactiva, respuestas visibles, tests en verde/rojo

---

### 2. Modo: CI/CD Automatizado (Newman CLI)

**Cuando usar**: Pipelines, pre-commit hooks, automated testing

#### A. Ejecucion Basica (JSON output)

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters json \
  --reporter-json-export reports/results.json
```

**Output**: JSON machine-readable

```json
{
  "run": {
    "stats": {
      "requests": { "total": 18, "pending": 0, "failed": 0 },
      "tests": { "total": 65, "pending": 0, "failed": 0 }
    },
    "timings": {
      "start": "2026-02-25T10:30:00Z",
      "stop": "2026-02-25T10:30:15Z"
    }
  }
}
```

#### B. Ejecucion con CLI Output

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli
```

**Output**: Terminal output amigable

```
Newman

Inscripcion - Happy Path

→ 01. GET Explorar Programas (Promotor)
  GET http://localhost:5001/api/crowdpromotion/programas/explorar?... [200 OK, 125ms]
  ✓ Status is 200
  ✓ Response contains programs list
  ✓ First program saved as test data

→ 02. POST Solicitar Inscripcion en Programa
  POST http://localhost:5001/api/crowdpromotion/programas/.../inscripcion [201 Created, 82ms]
  ✓ Status is 201 Created
  ✓ Inscripcion created and pendiente

...

║ requests │ 18 │ responses │ 18 │ pending │ 0
║ tests    │ 65 │ passed   │ 65 │ failed  │ 0

run duration: 15 seconds
```

#### C. Ejecucion con Reporte HTML

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/cp-inscripcion-programa.html \
  --reporter-htmlextra-darkTheme
```

**Output**:
- Console output + HTML report
- Abre: `reports/cp-inscripcion-programa.html` en navegador
- Incluye: graficos, timeline, detalles de cada request

#### D. Ejecucion con Condicion de Fallo (--bail)

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/results.html \
  --bail  # Detiene en primer fallo
```

**Util para**: Pre-commit hooks, strict CI/CD

#### E. Ejecucion Solo Happy Path

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path" \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/happy-path.html
```

**Util para**: Smoke tests rapidos

#### F. Ejecucion Solo Validaciones

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "Inscripcion - Validaciones" \
  --reporters cli
```

**Util para**: Tests de validacion separados

---

### 3. Modo: Con Entorno Externo

#### Usar archivo .env

```bash
cat > test.env.json << 'EOF'
{
  "id": "test-env",
  "values": [
    { "key": "baseUrl", "value": "https://api-staging.weplay.com" },
    { "key": "promotorEmail", "value": "test-promotor@staging.com" },
    { "key": "artistaEmail", "value": "test-artista@staging.com" }
  ]
}
EOF

newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --environment test.env.json \
  --reporters cli,htmlextra
```

#### O con variable line-by-line

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --env-var baseUrl=https://api-staging.com \
  --env-var promotorEmail=staging@test.com \
  --reporters cli
```

---

### 4. Modo: Con Timeout Personalizado

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --timeout 10000 \  # 10 segundos por request
  --reporters cli,htmlextra
```

---

### 5. Modo: Multiples Ejecuciones (Load Testing Basico)

```bash
for i in {1..5}; do
  echo "Run $i"
  newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
    --folder "_Setup,Inscripcion - Happy Path" \
    --reporters json \
    --reporter-json-export "reports/run-$i.json"
done
```

---

## Integracion en CI/CD

### GitHub Actions

```yaml
name: Integration Tests - cp-inscripcion-programa

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: WePlayRises2024!
          ACCEPT_EULA: Y
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P WePlayRises2024! -Q 'SELECT 1' || exit 1"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'

      - name: Start Backend
        run: |
          cd src/api
          dotnet run --project WebApi &
          sleep 10  # Wait for backend to start

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Run Integration Tests
        run: |
          newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export reports/results.html \
            --bail

      - name: Upload Reports
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-reports
          path: reports/
```

### GitLab CI

```yaml
integration_tests:
  image: mcr.microsoft.com/dotnet:8.0

  services:
    - name: mcr.microsoft.com/mssql/server:2022-latest
      alias: mssql
      variables:
        SA_PASSWORD: WePlayRises2024!
        ACCEPT_EULA: Y

  before_script:
    - apt-get update && apt-get install -y nodejs npm
    - npm install -g newman newman-reporter-htmlextra
    - dotnet restore src/api

  script:
    - dotnet run --project src/api/WebApi &
    - sleep 10
    - newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
        --reporters cli,htmlextra \
        --reporter-htmlextra-export reports/results.html \
        --bail

  artifacts:
    when: always
    paths:
      - reports/
    reports:
      junit: reports/junit.xml
```

### Azure Pipelines

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0'

  - task: UseNode@1
    inputs:
      version: '18.x'

  - script: npm install -g newman newman-reporter-htmlextra
    displayName: 'Install Newman'

  - script: |
      cd src/api
      dotnet run --project WebApi &
      sleep 10
    displayName: 'Start Backend'

  - script: |
      newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
        --reporters cli,htmlextra \
        --reporter-htmlextra-export $(System.DefaultWorkingDirectory)/reports/results.html \
        --bail
    displayName: 'Run Integration Tests'

  - task: PublishBuildArtifacts@1
    condition: always()
    inputs:
      pathToPublish: 'reports/'
      artifactName: 'test-reports'
```

---

## Validacion de Salida

### Exit Codes

```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json
echo "Exit code: $?"

# 0 = Todos los tests pasaron
# 1 = Algun test fallo
# 2 = Error en ejecucion (ej: archivo no encontrado)
```

### Parsing JSON Output

```bash
# Extraer numero de tests fallados
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters json \
  --reporter-json-export results.json

jq '.run.stats.tests.failed' results.json
# Output: 0 (si todos pasan)
```

### Script de Validacion Bash

```bash
#!/bin/bash

echo "Running cp-inscripcion-programa integration tests..."

newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters json \
  --reporter-json-export results.json

# Check results
FAILED=$(jq '.run.stats.tests.failed' results.json)
PASSED=$(jq '.run.stats.tests.passed' results.json)
TOTAL=$(jq '.run.stats.tests.total' results.json)

echo "Results: $PASSED/$TOTAL passed, $FAILED failed"

if [ $FAILED -gt 0 ]; then
  echo "❌ FAILED"
  exit 1
else
  echo "✓ SUCCESS"
  exit 0
fi
```

---

## Troubleshooting en CI/CD

### Error: "connection refused"
**Causa**: Backend no esta corriendo
**Solucion**: Agregar delay antes de ejecutar tests o wait-for-it script

```bash
# Esperar a que backend este ready
while ! curl -f http://localhost:5001/swagger > /dev/null 2>&1; do
  echo "Esperando backend..."
  sleep 2
done

# Luego ejecutar tests
newman run ...
```

### Error: "Database connection failed"
**Causa**: BD no esta iniciada
**Solucion**: En CI/CD, asegurar que el servicio DB esta up antes de run

### Error: "401 Unauthorized"
**Causa**: _Setup no se ejecuto o tokens expiraron
**Solucion**: Asegurar que _Setup carpeta se ejecuta primero

### Error: "Empty items array"
**Causa**: No hay programas activos en BD de prueba
**Solucion**: Seed BD con datos de prueba en setup script

```bash
# Script de setup antes de tests
dotnet run --project tools/DbSeeder -- seed-test-data
sleep 2
newman run ...
```

### Reportes no se generan
**Causa**: Carpeta `reports/` no existe
**Solucion**: Crearla previamente

```bash
mkdir -p reports
chmod 777 reports
newman run ... --reporter-htmlextra-export reports/results.html
```

---

## Optimizacion de Performance

### Parallelizacion (si es posible)

```bash
# Ejecutar carpetas en paralelo (NOTA: requiere BD aislada para cada run)
newman run collection.json --folder "_Setup,Inscripcion - Happy Path" &
newman run collection.json --folder "Inscripcion - Validaciones" &
wait
```

### Reduccion de Timeout

```bash
# Si backend es rapido, reducir timeout
newman run collection.json \
  --timeout 3000 \  # 3s en lugar de 5s por defecto
  --reporters cli
```

### Desabilitar SSL Verification (solo desarrollo)

```bash
# Para environments auto-firmados
newman run collection.json \
  --insecure \
  --reporters cli
```

---

## Retencion de Artifacts

### GitHub Actions

```yaml
- uses: actions/upload-artifact@v3
  with:
    name: test-reports
    path: reports/
    retention-days: 30  # Mantener por 30 dias
```

### GitLab CI

```yaml
artifacts:
  paths:
    - reports/
  expire_in: 30 days
```

---

## Alertas y Notificaciones

### Slack Notification

```bash
# Script para enviar resultado a Slack
WEBHOOK_URL="https://hooks.slack.com/services/YOUR/WEBHOOK/URL"

RESULT=$(jq '.run.stats.tests.failed' results.json)

if [ $RESULT -eq 0 ]; then
  curl -X POST $WEBHOOK_URL \
    -H 'Content-Type: application/json' \
    -d '{"text":"✓ cp-inscripcion-programa tests PASSED"}'
else
  curl -X POST $WEBHOOK_URL \
    -H 'Content-Type: application/json' \
    -d "{\"text\":\"❌ cp-inscripcion-programa tests FAILED: $RESULT failed\"}"
fi
```

### Email Notification

```bash
# Via mailx (si disponible)
newman run collection.json --reporters html,json | \
  mail -s "Test Results" team@weplay.com
```

---

## Comandos Rapidos (Copy-Paste)

```bash
# Desarrollo local
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/results.html

# Pre-commit hook
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path" \
  --bail && echo "✓ Tests passed"

# CI/CD strict (falla si hay algun error)
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,json \
  --reporter-json-export results.json \
  --bail

# Load testing (5 ejecuciones)
for i in {1..5}; do newman run plans/cp-inscripcion-programa/backend/postman-collection.json; done

# Solo validaciones
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "Inscripcion - Validaciones"
```

---

## Resumen

| Modo | Comando | Output | Cuando Usar |
|------|---------|--------|-------------|
| Development | `--reporters cli` | Terminal | Local debugging |
| Report | `--reporters htmlextra` | HTML | Analizar resultados |
| CI/CD | `--reporters json --bail` | JSON + exit code | Pipelines |
| Quick | `--folder "_Setup,Happy"` | CLI | Pre-commit |
| Load | Multiples runs | JSON + stats | Performance |

---

**Generado**: 2026-02-25
**Feature**: cp-inscripcion-programa (US-CP-03)
**Version**: 1.0
