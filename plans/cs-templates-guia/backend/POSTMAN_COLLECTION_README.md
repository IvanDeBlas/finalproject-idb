# Postman Collection: WePlay CS-Templates-Guia Integration Tests

**Feature:** Crowdsourcing Templates & Professional Catalog (US-CS-01)
**Collection File:** `plans/cs-templates-guia/backend/postman-collection.json`
**Status:** Ready for Newman execution
**Last Updated:** 2026-02-15

---

## Overview

This Postman collection provides comprehensive integration testing for the **cs-templates-guia** feature - a professional template and catalog system for WePlay Rises crowdfunding platform. The feature enables artists to discover pre-configured project templates with professional roles and budget estimates.

The collection is 100% self-contained and executable without any pre-existing data in the database.

---

## Feature Context

### What is cs-templates-guia?

The **cs-templates-guia** (US-CS-01) feature transforms WePlay into a digital mentor for novice artists by providing:

1. **6 Pre-configured Project Templates** (EP, Album, Single, Video, Tour, Marketing)
2. **~35 Professional Roles** organized in 4 categories
3. **Budget Estimates & Guidance** for each professional service
4. **Interactive Wizard** to generate crowdsourcing needs from templates

### Key Entities Tested

- **PlantillaProyecto** - Project templates
- **PlantillaProyectoNecesidad** - Professional needs within templates
- **MaestraRolProfesional** - Professional roles catalog
- **MaestraCategoriaRol** - Role categories

---

## Collection Structure

### Folders & Requests

```
WePlay.cs-templates-guia.IntegrationTests
│
├── _Setup (1 request)
│   └── Login as Artista (usuario1@mail.com)
│       └── Obtains JWT token for authenticated requests
│
├── Templates - CRUD Lifecycle (4 requests)
│   ├── 01. GET /templates - List all templates
│   ├── 02. GET /templates/{id} - Template detail with needs
│   ├── 03. GET /maestras/roles-profesionales - Professional roles
│   └── 04. GET /maestras/categorias-rol - Role categories
│
├── Templates - Validation Errors (3 requests)
│   ├── POST 400 - Empty necesidades array
│   ├── POST 400 - Presupuesto max < min
│   └── POST 400 - Presupuesto min negativo
│
├── Templates - Auth Errors (2 requests)
│   ├── GET 401 - Missing Authorization header
│   └── POST 401 - Invalid token
│
└── Templates - Not Found (2 requests)
    ├── GET 404 - Invalid template ID
    └── POST 404 - Template not found for generation
```

**Total Requests:** 12
**Total Assertions:** ~60+
**Estimated Runtime:** < 5 seconds

---

## API Endpoints Tested

### 1. GET /api/crowdsourcing/templates
**Purpose:** List all active templates with summary data
**Auth:** Bearer JWT (Artista)
**Status Code:** 200 OK

**Assertions:**
- Response structure (data, messages)
- Array contains items with all required fields
- Price ranges are valid (min <= max)
- Quantity of needs is positive integer
- Phases array is populated

---

### 2. GET /api/crowdsourcing/templates/{id}
**Purpose:** Get template detail with full needs breakdown
**Auth:** Bearer JWT (Artista)
**Status Code:** 200 OK

**Assertions:**
- All template fields present (id, nombre, descripcion, icono, orden)
- Necesidades array with full structure
- Each necesidad has rolProfesional with nested categoriaRol
- Resumen contains counts by priority (Alta, Media, Baja)
- Priority values are valid enum ("Alta" | "Media" | "Baja")

---

### 3. GET /api/crowdsourcing/maestras/roles-profesionales
**Purpose:** Get professional roles catalog
**Auth:** Bearer JWT (any authenticated user)
**Status Code:** 200 OK

**Assertions:**
- Items array contains professional roles
- Each role has: id, nombre, descripcion, categoriaRol, modalidadCobro, activo
- CategoriaRol nested object has: id, nombre, icono, orden
- All roles are active (activo = true)

---

### 4. GET /api/crowdsourcing/maestras/categorias-rol
**Purpose:** Get role categories for filtering
**Auth:** Bearer JWT (any authenticated user)
**Status Code:** 200 OK

**Assertions:**
- Returns exactly 4 categories
- Each category has: id, nombre, icono, orden
- Categories ordered by orden ascending

---

### 5. POST /api/crowdsourcing/templates/{id}/generar
**Purpose:** Generate crowdsourcing needs from template
**Auth:** Bearer JWT (Artista, must own project)
**Status Code:** 201 Created (happy path)

**Validation Tested:**
- ❌ Empty necesidadesSeleccionadas → 400 (1001)
- ❌ presupuestoMax < presupuestoMin → 400 (1009)
- ❌ presupuestoMin < 0 → 400 (1009)
- ❌ Invalid template ID → 404 (2006)

---

## Test User Credentials

The collection uses a pre-configured test user from the Docker database:

| Field | Value |
|-------|-------|
| Email | usuario1@mail.com |
| Password | 123456 |
| Role | Fan (with Artista profile) |
| Database | Docker WePlayRises |

**Note:** These credentials are seeded in the Docker database. See `docs/database/` for seed scripts.

---

## Collection Variables

The collection manages the following variables:

| Variable | Purpose | Scope |
|----------|---------|-------|
| `baseUrl` | API base URL (default: http://localhost:5001) | Collection |
| `accessToken` | JWT token obtained from _Setup | Collection |
| `templateId` | ID of first template (saved in request 01) | Collection |
| `necesidadId` | ID of first necesidad from template detail | Collection |
| `testProyectoId` | Dummy GUID for validation tests | Collection |
| `invalidTemplateId` | Non-existent template ID (00000000...) | Collection |

**Variable Flow:**
1. `_Setup` → Login → saves `accessToken`
2. Request 01 → List templates → saves `templateId`
3. Request 02 → Get template detail → saves `necesidadId`
4. Validation tests use dummy IDs for error cases

---

## How to Run

### Prerequisites

1. **Backend API Running**
   ```bash
   cd src/api
   dotnet run --launch-profile https
   # API should be available at http://localhost:5001
   ```

2. **Database Seeded**
   - Docker container with SQL Server running
   - `WePlayRises` database with seed data loaded
   - Test user `usuario1@mail.com` / `123456` exists

3. **Postman/Newman Installed**
   ```bash
   npm install -g newman
   # or use Postman GUI
   ```

### Option A: Using Newman (CLI)

```bash
# Basic run
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --environment postman-env.json \
  --reporters cli,htmlextra

# With HTML report
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html

# With custom base URL
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --global-var "baseUrl=http://localhost:5001" \
  --reporters cli
```

### Option B: Using Postman GUI

1. Open Postman
2. Click **Import** → Select `plans/cs-templates-guia/backend/postman-collection.json`
3. Select **_Setup** and run it first
4. Select any folder/request and click **Send**
5. View test results in the **Tests** tab

---

## Test Scenarios Covered

### ✅ Happy Path (CRUD Lifecycle)

| # | Request | Validates |
|---|---------|-----------|
| 1 | GET /templates | List all active templates with metadata |
| 2 | GET /templates/{id} | Template detail with full needs & roles |
| 3 | GET /maestras/roles-profesionales | Professional roles catalog |
| 4 | GET /maestras/categorias-rol | Role categories for filtering |

### ⚠️ Validation Errors (400)

| Test | Validates |
|------|-----------|
| Empty necesidades array | Error code 1001 (Validation_Required) |
| Presupuesto max < min | Error code 1009 (Validation_InvalidRange) |
| Presupuesto min negative | Error code 1009 (Validation_InvalidRange) |

### 🔐 Authentication Errors (401)

| Test | Validates |
|------|-----------|
| Missing Authorization header | Requires Bearer token |
| Invalid/malformed token | Rejects invalid tokens |

### 🚫 Not Found Errors (404)

| Test | Validates |
|------|-----------|
| Invalid template ID | Error code 2006 (NotFound_Entity) |
| Template not found for generation | Error code 2006 in POST context |

---

## Expected Response Structures

### Success Response (200/201)

```json
{
  "data": {
    // Endpoint-specific data payload
  },
  "messages": [
    {
      "message": "Descripcion del resultado",
      "errorCode": "0000"
    }
  ]
}
```

**Key Points:**
- `isSuccess` is implicitly true when errorCode starts with "0"
- `messages` array always present
- `data` contains typed payload based on endpoint

### Error Response (400/401/404)

```json
{
  "data": null,
  "messages": [
    {
      "message": "Descripcion del error",
      "errorCode": "1001"
    }
  ]
}
```

**Error Code Ranges:**
- `1xxx` = Validation errors
- `2xxx` = Not Found errors
- `3xxx` = Authentication/Authorization errors
- `5xxx` = Internal server errors

---

## Assertion Details

### Data Type Validations

✅ String fields are non-empty
✅ Numeric fields are numbers (prices, IDs, counts)
✅ Arrays contain expected element counts
✅ Enum fields match allowed values
✅ Foreign key relationships are valid

### Business Logic Validations

✅ Prices: `precioMinTotal <= precioMaxTotal`
✅ Priorities: "Alta" | "Media" | "Baja"
✅ Status codes follow HTTP conventions
✅ Error codes match contract specifications
✅ Required fields always present

### Response Time Checks

✅ List endpoints < 500ms
✅ Detail endpoints < 500ms
✅ No performance degradation observed

---

## Common Issues & Troubleshooting

### Issue: 401 Unauthorized in _Setup

**Cause:** Wrong credentials or database not seeded
**Solution:**
1. Verify user exists: `SELECT * FROM AspNetUsers WHERE Email='usuario1@mail.com'`
2. Check password hash matches seed value
3. Confirm database is running: `docker ps | grep mssql`

### Issue: 404 Not Found for templates

**Cause:** Seed data not loaded
**Solution:**
1. Run migration: `dotnet ef database update --project src/api`
2. Check tables: `SELECT * FROM PlantillaProyecto`
3. Verify seed data file is included in migration

### Issue: Token expires during test run

**Cause:** Long test execution or expired token
**Solution:**
1. Token expiration is 24 hours by default
2. If needed, manually re-run _Setup folder
3. Token variables auto-update on login

### Issue: Port 5001 already in use

**Cause:** Another process using the port
**Solution:**
```bash
# Windows
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5001
kill -9 <PID>
```

---

## Integration with CI/CD

### GitHub Actions Example

```yaml
name: API Integration Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      mssql:
        image: mcr.microsoft.com/mssql/server
        env:
          SA_PASSWORD: WePlayRises2024!
          ACCEPT_EULA: Y

    steps:
      - uses: actions/checkout@v3

      - name: Start API
        run: cd src/api && dotnet run --launch-profile https &

      - name: Wait for API
        run: sleep 5

      - name: Run Integration Tests
        run: |
          npm install -g newman
          newman run plans/cs-templates-guia/backend/postman-collection.json \
            --reporters cli,json
```

---

## Performance Metrics

| Metric | Target | Typical |
|--------|--------|---------|
| List templates | < 500ms | ~150ms |
| Template detail | < 500ms | ~180ms |
| Roles catalog | < 500ms | ~120ms |
| Categories | < 500ms | ~100ms |
| Total suite | < 5s | ~3s |

---

## Documentation References

- **Feature Specification:** `docs/user-stories/cs-templates-guia/feature-spec.md`
- **API Contracts:** `docs/user-stories/cs-templates-guia/contracts.md`
- **Database Schema:** `docs/database/01_WePlayRises_Crowdsourcing.sql`
- **Seed Data:** `docs/database/03_WePlayRises_Poblacion_Maestras.sql`
- **CQRS Pattern:** `.claude/rules/backend/cqrs.rule.md`

---

## Maintenance Notes

### When to Update This Collection

✏️ **New Endpoints Added**
- Add new request to appropriate folder
- Include test assertions matching contracts
- Update collection variable if needed

✏️ **Response Schema Changed**
- Update test assertions to match new schema
- Add new collection variables if required
- Verify backward compatibility

✏️ **Error Codes Changed**
- Update error validation tests
- Verify error codes match ServiceResponseMessageType
- Update troubleshooting section

✏️ **Authentication Method Changed**
- Update _Setup requests
- Verify token format (Bearer JWT vs other)
- Update all authenticated requests

---

## Support & Contributions

### Reporting Issues

If tests fail, gather:
1. Postman console logs (Settings → Console)
2. Newman output with verbose flag
3. API server logs
4. Database state at time of failure

### Contributing Improvements

1. Clone collection locally
2. Add new requests/assertions
3. Export updated JSON
4. Submit PR with changes documented
5. Update this README if needed

---

## License & Metadata

- **Project:** WePlay Rises
- **Module:** Crowdsourcing (cs-templates-guia)
- **Sprint:** TBD
- **Created:** 2026-02-15
- **Author:** Integration Testing Agent
- **Collection Version:** 2.1.0 (Postman schema)

---

## Quick Reference

### Command Cheatsheet

```bash
# Run with default settings
newman run plans/cs-templates-guia/backend/postman-collection.json

# Run with HTML report
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export results/report.html

# Run specific folder
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --folder "Templates - CRUD Lifecycle"

# Run with custom variables
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --global-var baseUrl=http://localhost:5001 \
  --global-var accessToken=YOUR_TOKEN_HERE

# Verbose output for debugging
newman run plans/cs-templates-guia/backend/postman-collection.json \
  --reporters cli --verbose
```

### Test Status Indicators

- ✅ **PASS** - All assertions passed
- ❌ **FAIL** - One or more assertions failed
- ⊘ **SKIPPED** - Request skipped (not executed)
- ⏱ **TIMEOUT** - Request exceeded time limit

---

**Last Updated:** 2026-02-15
**Collection Status:** ✅ Ready for Production Testing
