# WePlay Rises - Campanias Integration Tests (Newman)

Colección Postman ejecutable para testing de integración de la API de Campanias.

## Quick Start

### 1. Instalar Newman y herramientas

```bash
npm install -g newman newman-reporter-htmlextra newman-reporter-junitxml
```

### 2. Ejecutar contra Development

```bash
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export test-results/development-report.html
```

### 3. Ver reporte HTML

El reporte HTML se genera en: `test-results/development-report.html`

## Estructura de la Colección

```
WePlay.Campanias.IntegrationTests/
├── _Setup/
│   ├── Register Test User
│   ├── Login & Get Token
│   └── Create Test Artista Profile
│
├── Campanias/
│   ├── 201 CREATED/
│   │   ├── POST Create with All Fields
│   │   └── POST Create Minimal (Required Only)
│   │
│   ├── 200 OK - GET Detail/
│   │   └── GET Existing Campaign
│   │
│   ├── 200 OK - UPDATE/
│   │   ├── PUT Update Single Field
│   │   ├── GET Verify Updates Persisted
│   │   └── PUT Update Multiple Fields
│   │
│   ├── 200 OK - PUBLISH/
│   │   ├── POST Publish Valid Draft
│   │   └── GET Verify State Changed via GET
│   │
│   ├── 200 OK - LIST PUBLIC/
│   │   ├── GET All Campaigns (Default Pagination)
│   │   └── GET Pagination with pageNumber and pageSize
│   │
│   ├── 200 OK - LIST MY CAMPAIGNS/
│   │   ├── GET My Campaigns Authenticated
│   │   ├── GET My Campaigns Filter by State
│   │   └── GET My Campaigns Pagination
│   │
│   ├── 400 BAD REQUEST/
│   │   ├── POST Create Empty Titulo
│   │   ├── POST Create Titulo Too Long
│   │   ├── POST Create Invalid ImporteObjetivo
│   │   ├── POST Create ImporteMinimo > ImporteObjetivo
│   │   ├── POST Create Invalid URL Format
│   │   ├── POST Create FechaFin < FechaInicio
│   │   └── PUT Update Titulo Too Long
│   │
│   ├── 401 UNAUTHORIZED/
│   │   ├── POST Create Without Token
│   │   ├── POST Create with Invalid Token
│   │   ├── PUT Update Without Token
│   │   ├── POST Publish Without Token
│   │   └── GET My Campaigns Without Token
│   │
│   ├── 403 FORBIDDEN/
│   │   ├── PUT Update Campaign Not Owner
│   │   └── POST Publish Campaign Not Owner
│   │
│   ├── 404 NOT FOUND/
│   │   ├── GET Non-existent Campaign
│   │   ├── PUT Non-existent Campaign
│   │   └── POST Publish Non-existent Campaign
│   │
│   ├── 409 CONFLICT/
│   │   ├── PUT Update Published Campaign
│   │   └── POST Publish Already Published Campaign
│   │
│   └── E2E Happy Path/
│       ├── 1. Create Draft Campaign
│       ├── 2. Update Draft Campaign
│       ├── 3. Verify Updates with GET
│       ├── 4. Get My Campaigns List
│       ├── 5. Publish Campaign
│       ├── 6. Verify State is PUBLICADA
│       ├── 7. Try Edit Published (Expect 409)
│       ├── 8. Try Publish Again (Expect 409)
│       └── 9. Get from Public List
```

## Comandos Útiles

### Ejecutar solo un folder específico

```bash
# Solo tests de CREATE
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --folder "Campanias/201 CREATED"

# Solo E2E Happy Path
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --folder "Campanias/E2E Happy Path"

# Solo validaciones
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --folder "Campanias/400 BAD REQUEST"
```

### Reportes diferentes

```bash
# Reporte JSON para CI/CD
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    -r json \
    --reporter-json-file results.json

# Reporte JUnit para Azure DevOps / Jenkins
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    -r junitxml \
    --reporter-junitxml-export results.xml

# Con delay entre requests
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --delay-request 500
```

### Dry-run (validar colección sin ejecutar)

```bash
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    --dry-run
```

### Guardar variables finales en environment

```bash
newman run WePlay.Campanias.IntegrationTests.postman_collection.json \
    -e environments/development.postman_environment.json \
    --export-environment final-environment.json
```

## Requisitos

### API Backend

La API debe estar ejecutándose en `http://localhost:5000`

**Endpoints requeridos:**
- `POST /api/auth/register` - Registro de usuarios
- `POST /api/auth/login` - Login y obtener JWT
- `POST /api/artistas` - Crear perfil de artista
- `POST /api/campanias` - Crear campaña
- `GET /api/campanias/{id}` - Obtener campaña por ID
- `PUT /api/campanias/{id}` - Actualizar campaña
- `POST /api/campanias/{id}/publicar` - Publicar campaña
- `GET /api/campanias` - Listar campanias públicas
- `GET /api/campanias/mis-campanias` - Listar mis campanias

### Response Format

Todos los endpoints deben retornar `ServiceResponse<T>`:

```json
{
    "data": {},
    "messages": [
        {
            "message": "string",
            "errorCode": "string"
        }
    ],
    "isSuccess": true,
    "hasErrors": false
}
```

## Variables de Entorno

### Automáticamente configuradas durante la ejecución:

- `baseUrl` - URL base de la API
- `accessToken` - JWT obtenido en login (setup)
- `testUserId` - ID del usuario de test
- `testUserEmail` - Email del usuario de test
- `artistaId` - ID del artista de test
- `campaignId` - ID de la primera campaña creada
- `publishedCampaignId` - ID de la campaña publicada
- `e2eCampaignId` - ID de campaña para E2E test
- `secondUserToken` - Token de segundo usuario (para tests de 403)

## Assertions Incluidos

### Por Status Code

**200 OK:**
- Status code is 200
- Response time < 500ms
- isSuccess is true
- ServiceResponse structure validated

**201 CREATED:**
- Status code is 201
- Location header present
- Has ID in data
- ID saved to environment

**400 BAD REQUEST:**
- Status code is 400
- isSuccess is false
- Has error messages
- ErrorCode is 1xxx (validation)

**401 UNAUTHORIZED:**
- Status code is 401
- isSuccess is false
- ErrorCode is 3xxx (auth)

**403 FORBIDDEN:**
- Status code is 403
- ErrorCode is 3002 (Forbidden)

**404 NOT FOUND:**
- Status code is 404
- ErrorCode is 2003 (NotFound_Campania)

**409 CONFLICT:**
- Status code is 409
- ErrorCode is 4009 (BusinessRule_CampaniaNotDraft)

## Error Codes de Referencia

| Código | Descripción | HTTP |
|--------|-------------|------|
| 1001   | Validation_Required | 400 |
| 1002   | Validation_MaxLength | 400 |
| 1006   | Validation_InvalidUrl | 400 |
| 1007   | Validation_InvalidRange | 400 |
| 1011   | Validation_InvalidAmount | 400 |
| 1012   | Validation_InvalidDate | 400 |
| 2003   | NotFound_Campania | 404 |
| 3001   | Auth_Unauthorized | 401 |
| 3002   | Auth_Forbidden | 403 |
| 4009   | BusinessRule_CampaniaNotDraft | 409 |

## Metrics

**Total Requests:** 48
- Setup: 3
- 201 CREATED: 2
- 200 OK - GET: 1
- 200 OK - UPDATE: 3
- 200 OK - PUBLISH: 2
- 200 OK - LIST PUBLIC: 2
- 200 OK - LIST MY CAMPAIGNS: 3
- 400 BAD REQUEST: 8
- 401 UNAUTHORIZED: 5
- 403 FORBIDDEN: 2
- 404 NOT FOUND: 3
- 409 CONFLICT: 2
- E2E Happy Path: 9

**Total Test Assertions:** ~150+

**Expected Execution Time:** 30-40 segundos

## Troubleshooting

### Error: "Cannot read property 'id' of null"

**Causa:** Setup no se ejecutó correctamente
**Solución:** Asegúrate de que los requests de _Setup se ejecuten primero y que retornen 200/201

### Error: "Invalid token"

**Causa:** Token expirado o no guardado correctamente
**Solución:** Verifica que el setup de login guarde el token en {{accessToken}}

### Error: "Campaign not found (404)"

**Causa:** El ID no se guardó en la variable de ambiente
**Solución:** Verifica que la respuesta de POST campanias tenga un `data.id` válido

### Error: "401 on GET mi-campanias"

**Causa:** Falta el token en el header Authorization
**Solución:** Asegúrate de incluir: `Authorization: Bearer {{accessToken}}`

## CI/CD Integration

### GitHub Actions

```yaml
- name: Run Newman Tests
  run: |
    newman run tests/integration/WePlay.Campanias.IntegrationTests.postman_collection.json \
      -e tests/integration/environments/development.postman_environment.json \
      -r junitxml,htmlextra \
      --reporter-junitxml-export test-results/results.xml \
      --reporter-htmlextra-export test-results/report.html
```

### Azure DevOps

```yaml
- task: Newman@0
  inputs:
    collection: 'tests/integration/WePlay.Campanias.IntegrationTests.postman_collection.json'
    environment: 'tests/integration/environments/development.postman_environment.json'
    reporters: 'cli,junitxml,htmlextra'
    reporterJsonFile: '$(System.DefaultWorkingDirectory)/test-results/newman-results.json'
    reporterHtmlFile: '$(System.DefaultWorkingDirectory)/test-results/newman-report.html'
```

## Documentación Relacionada

- **Plan de Tests:** `tests/integration/newman-tests.md`
- **API Contracts:** `plans/crear-campania/backend/api-contracts.md`
- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`

---

**Creado:** 2026-02-12
**Versión:** 1.0
**Estado:** Ready for Execution
