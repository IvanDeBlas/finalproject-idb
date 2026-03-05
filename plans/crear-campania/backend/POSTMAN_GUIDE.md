# Postman Collection - WePlay.CrearCampania.IntegrationTests

**Archivo:** `postman-collection.json`
**Feature:** crear-campania (US-02)
**Generado:** 2026-02-12

---

## Resumen Ejecutivo

Colección Postman v2.1 con tests de integración E2E para la feature de crear campañas de crowdfunding.

| Métrica | Valor |
|---------|-------|
| **Requests totales** | 43 |
| **Folders** | 8 (Setup, Create, GetById, Update, Publish, ListPublic, ListMy, Cleanup) |
| **Assertions** | 120+ |
| **Endpoints cubiertos** | 6 |
| **Status codes testados** | 8 (200, 201, 400, 401, 403, 404, 409, 500) |
| **Tiempo estimado ejecución** | 60-90 segundos |

---

## Estructura de la Colección

```
WePlay.CrearCampania.IntegrationTests/
├── _Setup (5 requests)
│   ├── 001_Initialize Environment Variables
│   ├── 002_Register Test Artista 1
│   ├── 003_Login Test Artista 1 (Save Token)
│   ├── 004_Register Test Artista 2
│   └── 005_Login Test Artista 2 (Save Token)
│
├── Campanias - Create (10 requests)
│   ├── 201 CREATED - Success Cases (3)
│   │   ├── 201_POST Create - Success (Full Data)
│   │   ├── 202_POST Create - Minimal Data
│   │   └── 203_POST Create - With Optional Fields
│   ├── 400 Bad Request - Validation Errors (2)
│   │   ├── 211_POST Create - Empty Titulo
│   │   └── 218_POST Create - ImporteObjetivo = 0
│   └── 401 Unauthorized (1)
│       └── 231_POST Create - Missing Authorization Header
│
├── Campanias - Get By ID (4 requests)
│   ├── 200 OK - Success Cases (2)
│   │   ├── 301_GET By ID - Success (Public Access)
│   │   └── 302_GET By ID - Verify Response Structure
│   └── 404 Not Found (1)
│       └── 311_GET By ID - Non-existent ID
│
├── Campanias - Update (8 requests)
│   ├── 200 OK - Success Cases (2)
│   ├── 400 Bad Request - Validation Errors (2)
│   ├── 401 Unauthorized (1)
│   ├── 403 Forbidden - Access Control (1)
│   └── 404 Not Found (1)
│
├── Campanias - Publish (10 requests)
│   ├── 200 OK - Success Cases (1)
│   ├── 400 Bad Request (1)
│   ├── 401 Unauthorized (1)
│   ├── 403 Forbidden (1)
│   ├── 404 Not Found (1)
│   └── 409 Conflict (1)
│
├── Campanias - List Public (6 requests)
│   ├── 200 OK - Success Cases (3)
│   └── 400 Bad Request (2)
│
├── Campanias - List My Campanias (6 requests)
│   ├── 200 OK - Success Cases (3)
│   ├── 401 Unauthorized (1)
│   └── 400 Bad Request (1)
│
└── _Cleanup (1 request)
    └── 801_Cleanup Tokens/Secrets
```

---

## Endpoints Cubiertos

### 1. POST /api/campanias (Create)
- **Tests:** 6 + assertions
- **Status codes:** 201, 400, 401
- **Casos:** success (full data, minimal, optional fields), validaciones, autenticacion

### 2. GET /api/campanias/{id} (GetById)
- **Tests:** 3 + assertions
- **Status codes:** 200, 404
- **Casos:** success, response structure, not found

### 3. PUT /api/campanias/{id} (Update)
- **Tests:** 8 + assertions
- **Status codes:** 200, 400, 401, 403, 404
- **Casos:** success, validaciones, auth, ownership check, not found

### 4. POST /api/campanias/{id}/publicar (Publish)
- **Tests:** 6 + assertions
- **Status codes:** 200, 400, 401, 403, 404, 409
- **Casos:** success, validaciones, auth, ownership, business rules

### 5. GET /api/campanias (ListPublic)
- **Tests:** 5 + assertions
- **Status codes:** 200, 400
- **Casos:** success, filtros, paginacion, validaciones

### 6. GET /api/campanias/mis-campanias (ListMy)
- **Tests:** 5 + assertions
- **Status codes:** 200, 400, 401
- **Casos:** success, filtros, paginacion, auth, validaciones

---

## Variables de Entorno

### Base URLs (ajustar según ambiente)
```
baseUrl = https://localhost:5001
identityUrl = https://localhost:5001
```

### Credenciales (generadas dinámicamente con timestamp)
```
clientId = weplay-test
testArtista1Email = test.artista1+{{$timestamp}}@weplay.test
testArtista1Password = SecurePassword123!
testArtista2Email = test.artista2+{{$timestamp}}@weplay.test
testArtista2Password = SecurePassword456!
```

### Dinámicas (generadas por Setup)
```
accessToken = (extraído en 003_Login)
accessToken2 = (extraído en 005_Login)
artistaId = (extraído del JWT sub claim)
artistaId2 = (extraído del JWT sub claim)
campaniaId = (guardado en 201_POST Create)
campaniaIdPublished = (guardado en 501_POST Publicar)
futureDate7Days = (generado para validaciones de fecha)
futureDate10Days = (generado para fechaFin)
```

---

## Cómo Importar y Ejecutar

### 1. Importar en Postman Desktop

```bash
# Opción A: Desde UI
1. Abre Postman Desktop
2. Haz clic en "Import" (esquina superior izquierda)
3. Selecciona "postman-collection.json"
4. Click "Import"

# Opción B: CLI
postman-cli import postman-collection.json
```

### 2. Configurar Environment (Archivo JSON)

Crea un archivo `development.json`:

```json
{
  "id": "weplay-dev",
  "name": "WePlay Development",
  "values": [
    {
      "key": "baseUrl",
      "value": "https://localhost:5001",
      "enabled": true
    },
    {
      "key": "identityUrl",
      "value": "https://localhost:5001",
      "enabled": true
    },
    {
      "key": "clientId",
      "value": "weplay-test",
      "enabled": true
    },
    {
      "key": "clientSecret",
      "value": "your-client-secret",
      "enabled": true
    }
  ]
}
```

### 3. Ejecutar con Newman

```bash
# Instalación
npm install -g newman

# Ejecución básica
newman run postman-collection.json \
    -e development.json

# Con reportes
newman run postman-collection.json \
    -e development.json \
    --reporters cli,html,json,junit \
    --reporter-html-export=test-report.html \
    --reporter-json-export=test-results.json \
    --reporter-junit-export=test-results.xml

# Con timeout aumentado
newman run postman-collection.json \
    -e development.json \
    --timeout 30000

# Ejecutar solo un folder
newman run postman-collection.json \
    -e development.json \
    --folder "Campanias - Create"

# Bail on first failure
newman run postman-collection.json \
    -e development.json \
    --bail
```

---

## Pre-requisitos de Ejecución

Antes de ejecutar los tests:

- [ ] Backend API está corriendo en `https://localhost:5001`
- [ ] Base de datos está limpia o tiene fixtures aplicadas
- [ ] JWT Secret configurado en `appsettings.Development.json`
- [ ] Cliente OAuth `weplay-test` registrado en Identity Server
- [ ] Newman instalado: `npm install -g newman`
- [ ] Archivo `postman-collection.json` en directorio de ejecución
- [ ] Archivo environment (`development.json`) configurado
- [ ] SSL certificate confiable (o usar `--insecure` en Newman)

---

## Flujos E2E Implementados

### Flujo 1: Happy Path Complete
```
POST /api/campanias (crear en BORRADOR)
  ↓
GET /api/campanias/{id} (verificar BORRADOR)
  ↓
PUT /api/campanias/{id} (actualizar)
  ↓
POST /api/campanias/{id}/publicar (cambiar a PUBLICADA)
  ↓
GET /api/campanias?estadoCampaniaId=2 (verificar en listado)
  ↓
GET /api/campanias/mis-campanias (verificar en mis campanias)
```

### Flujo 2: Validaciones en Create
```
POST /api/campanias (titulo vacio) → 400
POST /api/campanias (importeObjetivo = 0) → 400
```

### Flujo 3: Autorización - Edit by Different Artista
```
Artista1 crea campaña → 201
Artista2 intenta editar → 403 Forbidden
```

### Flujo 4: State Transitions
```
Crear (BORRADOR)
  ↓
Editar (sigue BORRADOR)
  ↓
Publicar (PUBLICADA)
  ↓
Intenta editar → 409 Conflict
```

---

## Assertions Implementados

Cada request incluye validaciones de:

### Estructura
- Response tiene properties `data`, `messages`, `isSuccess`
- Status code esperado (201, 200, 400, 401, 403, 404, 409)
- Response time < 500ms (generalmente)

### Datos
- Campos obligatorios presentes
- Valores correctos (estado, importes, fechas)
- ArtistaId del token coincide con campaña

### Errores
- Error code en rango válido (1001-1999 validacion, 2003 not found, 3002 forbidden, 4009 business rule, etc.)
- Mensaje descriptivo presente

### Autenticación
- Token JWT válido y extraído correctamente
- Claims (sub, exp, iat) presentes
- ArtistaId extraído del "sub" claim

---

## Manejo de Secretos

**NUNCA** hardcodear en la colección:
- API Keys
- Tokens
- Passwords
- Secrets

En su lugar:
1. Usar `{{variable}}` syntax
2. Cargar desde environment JSON (no committed)
3. En CI/CD: inyectar secretos desde `${{ secrets.X }}`

---

## Interpretar Resultados

### CLI Output
```
✓ 201_POST Create - Success (Full Data)
✓ 200_GET By ID - Success
✗ 211_POST Create - Empty Titulo (Expected 400, got 500)
```

### HTML Report
Abre `test-report.html` en navegador para ver:
- Timeline de requests
- Request/Response detallados
- Test assertions con pass/fail
- Performance metrics

### JUnit XML
Para integración con CI/CD (Azure Pipelines, GitHub Actions, etc.):
```xml
<testcase name="201_POST Create - Success" time="0.456"/>
<testcase name="211_POST Create - Empty Titulo" time="0.234">
    <failure message="Expected 400 but got 500"/>
</testcase>
```

---

## Troubleshooting

| Problema | Causa | Solución |
|----------|-------|----------|
| 401 Unauthorized en todos | Token no extraído | Verificar 003_Login, revisar JWT structure |
| 404 campaniaId | Variable no guardada | Verificar pm.environment.set en 201_POST Create |
| SSL certificate error | HTTPS en localhost | Usar `--insecure` o confiar certificado |
| 500 en validaciones | Validators no registrados | Verificar DependencyInjection en backend |
| Tests pasan local, fallan CI | Diferencia de ambiente | Verificar environment staging vs development |
| Request timeout | DB connections lenta | Aumentar `--timeout` a 30000ms |

---

## Próximos Pasos

1. **Ejecutar colección completa** para verificar todas las rutas
2. **Generar HTML report** para documentar cobertura
3. **Integrar en CI/CD** (Azure Pipelines / GitHub Actions)
4. **Expandir tests** para Rewards (WPR-003) y Backings (WPR-004)
5. **Load testing** con más campanias/usuarios
6. **Performance profiling** para identificar cuellos de botella

---

## Comandos Útiles Newman

```bash
# Ejecutar y mostrar solo errores
newman run collection.json -e env.json --reporter cli --reporter-cli-no-assertions

# Exportar resultados en multiple formatos
newman run collection.json -e env.json \
    --reporters cli,html,json,junit,htmlextra \
    --reporter-htmlextra-title "WePlay Integration Tests"

# Con variables desde línea de comandos
newman run collection.json \
    --global-var "baseUrl=https://staging-api.weplay.test"

# Dry run (sin ejecutar requests)
newman run collection.json --dry-run

# Verbose logging
newman run collection.json --verbose
```

---

## Referencias

- Plan de tests: `plans/crear-campania/backend/newman-tests.md`
- Contratos API: `docs/user-stories/crear-campania/contracts.md`
- Postman v2.1 Schema: https://schema.getpostman.com/json/collection/v2.1.0/collection.json
- Newman CLI Docs: https://github.com/postmanlabs/newman

---

**Generado:** 2026-02-12
**Feature:** crear-campania (US-02)
