# Ejemplo de Ejecución: Colección Postman cs-valoraciones

Este documento muestra cómo se vería la ejecución de la colección Postman con Newman.

---

## Comando de Ejecución

```bash
$ cd C:\Repos\WePlay_Rises
$ newman run plans/cs-valoraciones/backend/postman-collection.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export reports/valoraciones-test-report.html
```

---

## Salida CLI Esperada

```
┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                                                                    ┃
┃          WePlay.Valoraciones.IntegrationTests                     ┃
┃                                                                    ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛


→ _Setup
  ↳ 01. Login Artista
    POST http://localhost:5001/api/auth/login [200 OK, 120ms]
    ✓ Status is 200
    ✓ Artista token obtained

  ↳ 02. Register Profesional
    POST http://localhost:5001/api/auth/register [200 OK, 150ms]
    ✓ Status is 200
    ✓ Profesional registered and token obtained

  ↳ 03. Get Artista Profile ID
    GET http://localhost:5001/api/artistas [200 OK, 80ms]
    ✓ Status is 200
    ✓ Has artista data

  ↳ 04. Create Necesidad (Crowdsourcing)
    POST http://localhost:5001/api/crowdsourcing/necesidades [201 Created, 200ms]
    ✓ Status is 201
    ✓ Necesidad created with ID

  ↳ 05. Create Propuesta (Profesional)
    POST http://localhost:5001/api/crowdsourcing/propuestas [201 Created, 180ms]
    ✓ Status is 201
    ✓ Propuesta created with ID

  ↳ 06. Accept Propuesta (Artista)
    POST http://localhost:5001/api/crowdsourcing/propuestas/.../aceptar [200 OK, 160ms]
    ✓ Status is 200

  ↳ 07. Get Acuerdo by Propuesta
    GET http://localhost:5001/api/crowdsourcing/necesidades/... [200 OK, 90ms]
    ✓ Status is 200
    ✓ Acuerdo data obtained

  ↳ 08. Complete Acuerdo (Artista)
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../completar [200 OK, 150ms]
    ✓ Status is 200
    ✓ Acuerdo completed

→ Valoraciones - Happy Path
  ↳ 01. POST Create Valoracion (Artista valora Profesional)
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [201 Created, 140ms]
    ✓ Status is 201
    ✓ Response time < 500ms
    ✓ ServiceResponse success with valoracionId
    ✓ Success message in response

  ↳ 02. GET Valoraciones del Usuario Valorado
    GET http://localhost:5001/api/crowdsourcing/usuarios/.../valoraciones?page=1&pageSize=10
    [200 OK, 120ms]
    ✓ Status is 200
    ✓ Response has resumen structure
    ✓ Response has valoraciones list
    ✓ Created valoracion appears in list

  ↳ 03. POST Valoracion Profesional valora Artista
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [201 Created, 130ms]
    ✓ Status is 201
    ✓ Profesional valoracion created

→ Valoraciones - Validation Errors
  ↳ 01. POST 400 - Puntuacion Missing
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [400 Bad Request, 80ms]
    ✓ Status is 400
    ✓ Has validation error for puntuacion

  ↳ 02. POST 400 - Puntuacion Zero
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [400 Bad Request, 75ms]
    ✓ Status is 400
    ✓ Has validation error for puntuacion range

  ↳ 03. POST 400 - Puntuacion Greater Than 5
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [400 Bad Request, 80ms]
    ✓ Status is 400
    ✓ Has validation error for puntuacion range

  ↳ 04. POST 400 - Comentario Exceeds Max Length
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [400 Bad Request, 85ms]
    ✓ Status is 400
    ✓ Has validation error for comentario length

→ Valoraciones - Business Rules
  ↳ 01. POST 400 - Duplicate Valoracion Same Acuerdo
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [400 Bad Request, 90ms]
    ✓ Status is 400
    ✓ Has BusinessRule_DuplicateAction error

→ Valoraciones - Auth Errors
  ↳ 01. POST 401 - Missing Token
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [401 Unauthorized, 50ms]
    ✓ Status is 401

  ↳ 02. GET 401 - Missing Token
    GET http://localhost:5001/api/crowdsourcing/usuarios/.../valoraciones [401 Unauthorized, 45ms]
    ✓ Status is 401

→ Valoraciones - Not Found
  ↳ 01. POST 404 - Acuerdo Not Found
    POST http://localhost:5001/api/crowdsourcing/acuerdos/00000000-0000-0000-0000-000000000000/valoraciones
    [404 Not Found, 60ms]
    ✓ Status is 404
    ✓ Has NotFound_Acuerdo error

  ↳ 02. GET 404 - User Not Found
    GET http://localhost:5001/api/crowdsourcing/usuarios/nonexistent-user-id/valoraciones [404 Not Found, 55ms]
    ✓ Status is 404
    ✓ Has NotFound_Entity error

→ Valoraciones - Pagination
  ↳ 01. GET Valoraciones Page 1 with PageSize 2
    GET http://localhost:5001/api/crowdsourcing/usuarios/.../valoraciones?page=1&pageSize=2
    [200 OK, 110ms]
    ✓ Status is 200
    ✓ Pagination working correctly

  ↳ 02. GET Valoraciones with Invalid Page
    GET http://localhost:5001/api/crowdsourcing/usuarios/.../valoraciones?page=0&pageSize=10
    [400 Bad Request, 70ms]
    ✓ Status is 400 for invalid page
    ✓ Has validation error


┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                          Test Results                              ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

    Passed:        22
    Failed:        0
    Skipped:       0
    Assertions:    65

┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                      Response Time Summary                         ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

    Min:    45ms
    Max:    200ms
    Avg:    115ms
    Total:  2.5s

┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
┃                  Execution Summary                                  ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

    Collection : WePlay.Valoraciones.IntegrationTests
    Environment: (none)
    Iterations : 1
    Duration   : 2m 45s (estimated)
    Requests   : 22
    Folders    : 7

    Reporting  : htmlextra
    Report     : reports/valoraciones-test-report.html

✓ HTML Report generated successfully
```

---

## Salida Esperada al Ejecutar Solo Setup + Happy Path

```bash
$ ./run-tests.sh happy
```

```
========================================
WePlay Rises - Valoraciones Integration Tests (US-CS-06)
========================================

ℹ Ejecutando _Setup + Happy Path...
ℹ Colección: plans/cs-valoraciones/backend/postman-collection.json
ℹ Reporte: reports/valoraciones_20260221_143055.html
ℹ Iniciando pruebas...

→ _Setup (8 requests) ... ✓ All passed
→ Valoraciones - Happy Path (3 requests) ... ✓ All passed

✓ Todas las pruebas pasaron correctamente
ℹ Reporte HTML: reports/valoraciones_20260221_143055.html
ℹ Reporte JSON: reports/valoraciones_20260221_143055.json
```

---

## Salida Esperada en Caso de Fallo

### Ejemplo 1: Endpoint no implementado

```
→ Valoraciones - Happy Path
  ↳ 01. POST Create Valoracion
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones
    Error: connect ECONNREFUSED 127.0.0.1:5001
    ✗ Unable to connect

? Failed: Backend not running or endpoint not implemented
```

**Solución:** Verificar que backend está corriendo en http://localhost:5001

### Ejemplo 2: Validator falla (puntuación fuera de rango no validada)

```
→ Valoraciones - Validation Errors
  ↳ 03. POST 400 - Puntuacion Greater Than 5
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones
    [201 Created, 140ms] ← UNEXPECTED, should be 400
    ✗ Status is 400
      Expected: 400, Actual: 201

? Failed: Validation_InvalidRange (1009) not implemented in Validator
```

**Solución:** Implementar validación de rango en `CreateValoracionCommandValidator`

### Ejemplo 3: ServiceResponse structure incorrect

```
→ Valoraciones - Happy Path
  ↳ 01. POST Create Valoracion
    POST http://localhost:5001/api/crowdsourcing/acuerdos/.../valoraciones [201 Created, 140ms]
    ✓ Status is 201
    ✗ ServiceResponse success with valoracionId
      Error: json.data is undefined

? Failed: Response doesn't have expected structure
```

**Solución:** Verificar que handler retorna `ServiceResponse<ValoracionCreatedResultDto>` con estructura correcta

---

## Validación Post-Implementación

Después de implementar endpoints, ejecutar:

```bash
$ newman run plans/cs-valoraciones/backend/postman-collection.json \
    --environment tests/integration/environments/development.postman_environment.json \
    --reporters cli,json \
    --reporter-json-export reports/results.json
```

**Criterio de aceptación:**
```
Passed: 22
Failed: 0
```

Si hay fallos:
1. Revisar output detallado
2. Identificar qué request falló
3. Leer documentación en `ENDPOINTS_IMPLEMENTATION_CHECKLIST.md`
4. Implementar fix
5. Re-ejecutar

---

## Script de Automatización (CI/CD)

Ejemplo de script bash para CI/CD:

```bash
#!/bin/bash

COLLECTION="plans/cs-valoraciones/backend/postman-collection.json"
ENVIRONMENT="tests/integration/environments/development.postman_environment.json"
REPORTS_DIR="reports"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "Starting cs-valoraciones integration tests..."

newman run "$COLLECTION" \
  --environment "$ENVIRONMENT" \
  --reporters cli,json,htmlextra \
  --reporter-json-export "$REPORTS_DIR/results_${TIMESTAMP}.json" \
  --reporter-htmlextra-export "$REPORTS_DIR/report_${TIMESTAMP}.html" \
  --timeout-request 10000

EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
  echo "✓ All tests passed"
  exit 0
else
  echo "✗ Tests failed (exit code: $EXIT_CODE)"
  exit 1
fi
```

---

## Performance Esperado

```
Tier          Duration
Setup:        ~2-3 segundos
Happy Path:   ~0.5-1 segundo
Validations:  ~0.7-1 segundo
Business:     ~0.2 segundos
Auth:         ~0.2 segundos
Not Found:    ~0.2 segundos
Pagination:   ~0.3 segundos
─────────────────────────
Total:        ~4-6 segundos (sin contar tiempo de setup)
             ~45-60 segundos (con setup completo)
```

---

## Logs Útiles para Debugging

### Ver request/response detallado
```bash
newman run postman-collection.json \
  --reporter cli \
  --bail \
  --verbose
```

### Generar reporte JSON para parsing programático
```bash
newman run postman-collection.json \
  --reporter json \
  --reporter-json-export results.json
# Luego procesar results.json con herramienta favorite
```

### Ver solo requests que fallaron
```bash
newman run postman-collection.json \
  --reporter json \
  --reporter-json-export results.json
jq '.run.executions[] | select(.response.status != "OK") | .item.name' results.json
```

---

## Integración en GitHub Actions (Ejemplo)

```yaml
name: cs-valoraciones Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Run Integration Tests
        run: |
          newman run plans/cs-valoraciones/backend/postman-collection.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export test-report.html

      - name: Upload Report
        if: always()
        uses: actions/upload-artifact@v2
        with:
          name: test-reports
          path: test-report.html
```

---

**Nota:** Los tiempos exactos varían según:
- Velocidad de la máquina
- Carga de BD
- Latencia de red (si backend está remoto)
- Tamaño de datos en BD

El ejemplo anterior asume:
- Backend en localhost:5001
- BD local o cercana
- Machine moderadamente potente

---

**Última actualización:** 2026-02-21
