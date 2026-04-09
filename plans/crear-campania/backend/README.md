# Postman Collection - WePlay.CrearCampania.IntegrationTests

Colección Postman v2.1 con tests de integración E2E para la feature **crear-campania (US-02)**.

## Archivos Incluidos

```
plans/crear-campania/backend/
├── postman-collection.json          # Coleccion Postman v2.1 (ejecutable)
├── environment-development.json     # Variables de environment (desarrollo)
├── POSTMAN_GUIDE.md                 # Guia completa de uso
├── run-newman.sh                    # Script bash para ejecutar
├── run-newman.ps1                   # Script PowerShell para ejecutar
└── README.md                        # Este archivo
```

## Resumen Ejecutivo

| Métrica | Valor |
|---------|-------|
| **Requests** | 43 |
| **Folders** | 8 |
| **Assertions** | 120+ |
| **Endpoints** | 6 |
| **Status codes** | 8 (200, 201, 400, 401, 403, 404, 409, 500) |
| **Tiempo estimado** | 60-90 segundos |

## Endpoints Cubiertos

1. **POST /api/campanias** - Crear campaña
2. **GET /api/campanias/{id}** - Obtener campaña por ID
3. **PUT /api/campanias/{id}** - Actualizar campaña
4. **POST /api/campanias/{id}/publicar** - Publicar campaña
5. **GET /api/campanias** - Listar campañas públicas
6. **GET /api/campanias/mis-campanias** - Listar mis campañas

## Inicio Rápido

### 1. Importar en Postman Desktop

```bash
# Abrir Postman
# Menú: File > Import
# Seleccionar: postman-collection.json
# Click: Import
```

### 2. Ejecutar con Newman (Bash)

```bash
cd plans/crear-campania/backend/
chmod +x run-newman.sh
./run-newman.sh development
```

### 3. Ejecutar con Newman (PowerShell - Windows)

```powershell
cd plans\crear-campania\backend\
.\run-newman.ps1 -Environment development
```

### 4. Ejecutar con Newman (CLI directo)

```bash
newman run postman-collection.json \
    -e environment-development.json \
    --reporters cli,html \
    --reporter-html-export=test-report.html
```

## Estructura de la Colección

```
_Setup (5 requests)
├── 001_Initialize Environment Variables
├── 002_Register Test Artista 1
├── 003_Login Test Artista 1 (Save Token)
├── 004_Register Test Artista 2
└── 005_Login Test Artista 2 (Save Token)

Campanias - Create (10 requests)
├── 201 CREATED - Success Cases (3)
├── 400 Bad Request - Validation Errors (2)
└── 401 Unauthorized (1)

Campanias - Get By ID (4 requests)
├── 200 OK - Success Cases (2)
└── 404 Not Found (1)

Campanias - Update (8 requests)
├── 200 OK - Success Cases (2)
├── 400 Bad Request (2)
├── 401 Unauthorized (1)
├── 403 Forbidden (1)
└── 404 Not Found (1)

Campanias - Publish (10 requests)
├── 200 OK - Success Cases (1)
├── 400 Bad Request (1)
├── 401 Unauthorized (1)
├── 403 Forbidden (1)
├── 404 Not Found (1)
└── 409 Conflict (1)

Campanias - List Public (6 requests)
├── 200 OK - Success Cases (3)
└── 400 Bad Request (2)

Campanias - List My Campanias (6 requests)
├── 200 OK - Success Cases (3)
├── 401 Unauthorized (1)
└── 400 Bad Request (1)

_Cleanup (1 request)
└── 801_Cleanup Tokens/Secrets
```

## Variables de Entorno

### Base URLs
```
baseUrl = https://localhost:5001
identityUrl = https://localhost:5001
```

### Credenciales (dinámicas)
```
clientId = weplay-test
testArtista1Email = test.artista1+{{$timestamp}}@weplay.test
testArtista1Password = SecurePassword123!
testArtista2Email = test.artista2+{{$timestamp}}@weplay.test
testArtista2Password = SecurePassword456!
```

### Tokens (generados por Setup)
```
accessToken = (extraído en login)
accessToken2 = (extraído en login)
artistaId = (extraído del JWT)
artistaId2 = (extraído del JWT)
campaniaId = (guardado en POST Create)
campaniaIdPublished = (guardado en POST Publish)
```

## Flujos E2E

### Happy Path: Crear → Editar → Publicar → Listar
```
1. POST /api/campanias (crear en BORRADOR)
   └─ Estado: 1 (BORRADOR)

2. GET /api/campanias/{id} (verificar)
   └─ Confirmar estado

3. PUT /api/campanias/{id} (actualizar)
   └─ Modificar titulo/descripcion

4. POST /api/campanias/{id}/publicar (publicar)
   └─ Estado cambia a: 2 (PUBLICADA)

5. GET /api/campanias?estadoCampaniaId=2 (listar públicas)
   └─ Verificar en listado

6. GET /api/campanias/mis-campanias (mis campanias)
   └─ Verificar acceso propietario
```

### Validaciones en Create
```
POST /api/campanias (titulo vacio) → 400 (ErrorCode: 1001)
POST /api/campanias (importeObjetivo = 0) → 400 (ErrorCode: 1011)
```

### Autorización - Edit by Different Artist
```
Artista1 crea campaña → 201
Artista2 intenta editar → 403 Forbidden (ErrorCode: 3002)
```

### State Transitions
```
Crear (BORRADOR: 1)
  ↓
Editar (sigue BORRADOR)
  ↓
Publicar (PUBLICADA: 2)
  ↓
Intenta editar → 409 Conflict (ErrorCode: 4009)
```

## Requisitos Previos

- [ ] Backend API corriendo en `https://localhost:5001`
- [ ] Base de datos limpia o con fixtures
- [ ] JWT Secret en `appsettings.Development.json`
- [ ] Cliente OAuth `weplay-test` registrado
- [ ] Node.js 16+ instalado
- [ ] Newman instalado: `npm install -g newman`
- [ ] Certificado SSL de confianza (o usar `--insecure`)

## Instalación de Newman

```bash
# Global install
npm install -g newman

# Verificar instalacion
newman --version
```

## Comandos Newman

```bash
# Basico
newman run postman-collection.json -e environment-development.json

# Con reportes completos
newman run postman-collection.json \
    -e environment-development.json \
    --reporters cli,html,json,junit,htmlextra \
    --reporter-html-export=test-report.html \
    --reporter-json-export=test-results.json \
    --reporter-junit-export=test-results.xml

# Solo un folder
newman run postman-collection.json \
    -e environment-development.json \
    --folder "Campanias - Create"

# Con timeout
newman run postman-collection.json \
    -e environment-development.json \
    --timeout 30000

# Bail en primer fallo
newman run postman-collection.json \
    -e environment-development.json \
    --bail

# Verbose logging
newman run postman-collection.json \
    -e environment-development.json \
    --verbose
```

## Scripts Disponibles

### Bash (Linux/macOS)
```bash
chmod +x run-newman.sh

# Ejecucion basica
./run-newman.sh

# Specificar ambiente
./run-newman.sh staging

# Solo un folder
./run-newman.sh development --folder "Campanias - Create"

# Con opciones
./run-newman.sh development --bail --verbose
```

### PowerShell (Windows)
```powershell
# Ejecucion basica
.\run-newman.ps1

# Specificar ambiente
.\run-newman.ps1 -Environment staging

# Solo un folder
.\run-newman.ps1 -Folder "Campanias - Create"

# Con opciones
.\run-newman.ps1 -Environment development -Bail -Verbose

# Ver ayuda
.\run-newman.ps1 -ShowHelp
```

## Interpretar Resultados

### CLI Output
```
┌─────────────────────────────────────────┐
│ WePlay.CrearCampania.IntegrationTests   │
└─────────────────────────────────────────┘

_Setup
  ✓ 001_Initialize Environment Variables
  ✓ 002_Register Test Artista 1
  ✓ 003_Login Test Artista 1 (Save Token)

Campanias - Create
  201 CREATED - Success Cases
    ✓ 201_POST Create - Success (Full Data)
    ✓ 202_POST Create - Minimal Data
    ✓ 203_POST Create - With Optional Fields

  400 Bad Request - Validation Errors
    ✓ 211_POST Create - Empty Titulo
    ✓ 218_POST Create - ImporteObjetivo = 0

...

┌────────────────────────┐
│     Test Summary       │
├────────────────────────┤
│ Total Requests  │  43  │
│ Passed          │  42  │
│ Failed          │   1  │
│ Skipped         │   0  │
└────────────────────────┘

Total time: 87.234 seconds
```

### HTML Report
- Abre `test-report.html` en navegador
- Timeline de requests
- Request/Response detallados
- Test assertions con pass/fail
- Performance graphs

### JUnit XML
Usado por CI/CD pipelines:
```xml
<testcase name="201_POST Create - Success" time="0.456"/>
<testcase name="211_POST Create - Empty Titulo" time="0.234">
    <failure message="Expected 400 but got 500"/>
</testcase>
```

## Troubleshooting

| Problema | Causa | Solución |
|----------|-------|----------|
| 401 en todos tests | Token no extraído | Verificar 003_Login, JWT válido |
| 404 campaniaId | Variable no guardada | Verificar pm.environment.set |
| SSL certificate error | HTTPS localhost | Usar `--insecure` en Newman |
| 500 validaciones | Validators no registrados | Verificar DependencyInjection |
| Timeout | DB lenta | Aumentar `--timeout 30000` |
| Email duplicate | Reutilizar mismo email | Usar `{{$timestamp}}` en emails |

## Integración CI/CD

### GitHub Actions
```yaml
name: Integration Tests

on: [push, pull_request]

jobs:
  newman-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: npm install -g newman
      - run: |
          newman run postman-collection.json \
              -e environment-development.json \
              --reporters cli,json,junit \
              --reporter-junit-export=test-results.xml
      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: test-results
          path: test-results.xml
```

### Azure Pipelines
```yaml
stages:
  - stage: IntegrationTests
    jobs:
      - job: NewmanTests
        pool:
          vmImage: 'ubuntu-latest'
        steps:
          - task: UseNode@1
            inputs:
              version: '18.x'
          - script: npm install -g newman
          - script: |
              newman run postman-collection.json \
                  -e environment-development.json \
                  --reporters cli,junit \
                  --reporter-junit-export=test-results.xml
          - task: PublishTestResults@2
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: 'test-results.xml'
```

## Referencias

- **Plan de tests:** `plans/crear-campania/backend/newman-tests.md`
- **Contratos API:** `docs/user-stories/crear-campania/contracts.md`
- **Guia completa:** `plans/crear-campania/backend/POSTMAN_GUIDE.md`
- **Postman v2.1 Schema:** https://schema.getpostman.com/json/collection/v2.1.0/collection.json
- **Newman CLI:** https://github.com/postmanlabs/newman

## Notas Importantes

- Los emails usan `{{$timestamp}}` para evitar duplicados
- Tokens JWT se extraen automáticamente en Setup
- ArtistaId se extrae del claim "sub" del token
- Campanias se crean en estado BORRADOR (1)
- Publicadas pasan a estado PUBLICADA (2)
- Las fechas usan dinámicamente: futureDate7Days, futureDate10Days
- Todos los tests tienen assertions para validar contratos
- No hardcodear secrets: usar environment variables

## Soporte

Para reportar issues o sugerencias sobre esta colección:

1. Verificar que el plan `newman-tests.md` esté actualizado
2. Revisar los contratos en `contracts.md`
3. Consultar la guía completa en `POSTMAN_GUIDE.md`
4. Ejecutar con `--verbose` para más detalles

---

**Generado:** 2026-02-12
**Feature:** crear-campania (US-02)
**Status:** Ready for testing
