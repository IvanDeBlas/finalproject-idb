# Resumen de Ejecución: Colección Postman cp-perfil-promotor

**Archivo:** `postman-collection.json`
**Feature:** US-CP-01 - Perfil de Promotor
**Módulo Backend:** Crowdpromotion
**Fecha Generación:** 2026-02-25

---

## Quick Start (30 segundos)

### Opción 1: Postman UI (Recomendado para desarrollo)

1. **Importa** `postman-collection.json` en Postman
2. **Abre carpeta** `_Setup`
3. **Envía** request `01. Login con Usuario de Prueba`
4. **Abre carpeta** `Promotor - CRUD Lifecycle`
5. **Ejecuta en orden** los 6 requests (POST → GET → PUT → GET → PATCH → GET)

### Opción 2: Newman CLI (Recomendado para CI/CD)

```bash
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html
```

---

## Cobertura de Endpoints

### ✓ POST /api/crowdpromotion/promotor

**En CRUD Lifecycle:**
- Paso 01: Crear perfil nuevo
- Crea `Promotor` + `PromotorWallet` (EUR)
- Genera `promotorId` para pasos siguientes

**En Validation Errors (7 tests):**
- nombrePublico: empty, < 3 chars, > 200 chars
- tipoPromotorId: missing, invalid (999)
- emailContacto: invalid format
- urlSitioWeb: invalid format

**En Auth Errors:**
- Sin header Authorization

**En Business Rules:**
- Promotor ya existe para este usuario (error 4018)

---

### ✓ GET /api/crowdpromotion/promotor/me

**En CRUD Lifecycle (3 pasos):**
- Paso 02: Verify creación (obtiene campos completos)
- Paso 04: Verify actualización (valida cambios persisten)
- Paso 06: Verify desactivación (valida esActivo = false)

**En Auth Errors:**
- Sin header Authorization

**En Not Found:**
- Usuario sin perfil de promotor existente (error 2015)

---

### ✓ PUT /api/crowdpromotion/promotor/me

**En CRUD Lifecycle:**
- Paso 03: Actualizar campos (nombrePublico, email, URLs)
- Campos no editables: tipoPromotorId

**En Validation Errors:**
- Mismas reglas que POST (nombrePublico, emails, URLs)

**En Auth Errors:**
- Con token inválido

---

### ✓ PATCH /api/crowdpromotion/promotor/me/desactivar

**En CRUD Lifecycle:**
- Paso 05: Desactivar perfil
- Desactivación lógica (EsActivo = false)
- Desactiva programas asociados (programasDadosDeBaja)

**En Business Rules:**
- Perfil ya desactivado (error 4019)

---

## Estructura de Validaciones

### Error Codes Cubiertos

| Rango | Categoría | Códigos Testeados |
|-------|-----------|------------------|
| 0000-0999 | Success | 0001 (Created), 0002 (Updated) |
| 1000-1999 | Validation | 1001 (Required), 1002 (MaxLength), 1003 (InvalidEmail), 1010 (ForeignKey), 1011 (MinLength), 1013 (InvalidUrl) |
| 2000-2999 | NotFound | 2015 (NotFound_Promotor) |
| 3000-3999 | Auth | 3001 (Unauthorized) |
| 4000-4999 | Business | 4018 (PromotorAlreadyExists), 4019 (PromotorAlreadyInactive) |
| 5000-5999 | Internal | 5000 (UnexpectedError) |

---

## Flujo de Datos

### Setup Phase

```
Request: POST /api/auth/login
├── Email: usuario1@mail.com (preexistente en seed)
├── Password: 123456
└── Response: accessToken
    └── Guardado en: pm.collectionVariables('accessToken')
```

### CRUD Lifecycle Phase

```
Request 01: POST /api/crowdpromotion/promotor
├── Auth: Bearer {{accessToken}}
├── Body: nombrePublico, tipoPromotorId=2, email, URLs
└── Response: promotorId
    └── Guardado en: pm.collectionVariables('promotorId')

Request 02: GET /api/crowdpromotion/promotor/me
├── Auth: Bearer {{accessToken}}
├── Valida: Perfil creado con datos correctos
└── Calcs verificados: totalProgramasActivos, totalComisionesGanadas

Request 03: PUT /api/crowdpromotion/promotor/me
├── Auth: Bearer {{accessToken}}
├── Body: nombrePublico (Updated), email (nuevo), URLs
└── Response: fechaActualizacion (verifica que se grabó)

Request 04: GET /api/crowdpromotion/promotor/me
├── Auth: Bearer {{accessToken}}
├── Valida: Cambios persistieron
└── nombrePublico = "DJ Marketing Pro (Updated)"

Request 05: PATCH /api/crowdpromotion/promotor/me/desactivar
├── Auth: Bearer {{accessToken}}
├── Body: Vacío
└── Response: esActivo = false, programasDadosDeBaja = 0

Request 06: GET /api/crowdpromotion/promotor/me
├── Auth: Bearer {{accessToken}}
├── Valida: Desactivación persiste
└── esActivo = false (confirmado)
```

---

## Estadísticas

| Métrica | Valor |
|---------|-------|
| **Total Folders** | 5 (_Setup + 4 feature folders) |
| **Total Requests** | 20 |
| **Total Assertions** | ~64 |
| **Endpoints Cubiertos** | 4 (POST, GET, PUT, PATCH) |
| **HTTP Status Codes Validados** | 201, 200, 400, 401, 404 |
| **Error Codes Validados** | 9 códigos específicos |
| **Tiempo Ejecución Esperado** | ~3-5 segundos (CRUD) + ~5-7 segundos (errores) |

---

## Casos Cubiertos por Folder

### _Setup (1 request, 1 assertion)

```
✓ Login exitoso → obtiene JWT
```

### Promotor - CRUD Lifecycle (6 requests, ~20 assertions)

```
✓ Crear perfil nuevo (201)
✓ Obtener perfil creado (200)
✓ Actualizar perfil (200)
✓ Verificar actualización (200)
✓ Desactivar perfil (200)
✓ Verificar desactivación (200)
```

### Promotor - Validation Errors (7 requests, ~14 assertions)

```
✓ nombrePublico vacío (400-1001)
✓ nombrePublico < 3 caracteres (400-1011)
✓ nombrePublico > 200 caracteres (400-1002)
✓ tipoPromotorId faltante (400-1001)
✓ tipoPromotorId inválido (400-1010)
✓ emailContacto formato inválido (400-1003)
✓ urlSitioWeb formato inválido (400-1013)
```

### Promotor - Auth Errors (3 requests, 3 assertions)

```
✓ POST sin token (401)
✓ GET sin token (401)
✓ PUT con token inválido (401)
```

### Promotor - Not Found & Business Rules (3 requests, ~6 assertions)

```
✓ GET sin perfil existente (404-2015)
✓ POST con perfil ya existente (400-4018)
✓ PATCH ya desactivado (400-4019)
```

---

## Validaciones por Request

### Validaciones de Status Code

Todos los requests validan `pm.response.to.have.status(XXX)`:
- 201: POST Create
- 200: GET Me (x3), PUT Update, PATCH Deactivate
- 400: Todos los errores de validación y negocio
- 401: Todos los errores de autenticación
- 404: Perfil no encontrado

### Validaciones de Estructura Response

Todos validan la estructura `ServiceResponse<T>`:

```javascript
// Success responses (2xx)
json.data.id → guardado en variable
json.data.nombrePublico → validado
json.data.esActivo → validado

// Error responses (4xx)
json.messages[] → array de errores
json.messages[0].errorCode → validado contra código específico
```

### Validaciones de Campos

Por endpoint:

**POST Create:**
- id (Guid generado)
- nombrePublico (string, 3-200 chars)
- tipoPromotorNombre (resolved from maestro)
- esActivo (true)
- fechaCreacion (DateTime ISO)

**GET Me:**
- Todos los campos de POST
- + tipoPromotorId (number)
- + emailContacto, urlSitioWeb, urlInstagram, urlTikTok, urlYouTube, urlTwitter (optional URLs)
- + totalProgramasActivos (calculated, >= 0)
- + totalComisionesGanadas (calculated, >= 0)
- + monedaComisiones ("EUR")

**PUT Update:**
- id (same as created)
- nombrePublico (updated value)
- fechaActualizacion (DateTime ISO, new)

**PATCH Deactivate:**
- id (same as created)
- esActivo (false)
- programasDadosDeBaja (number, >= 0)

---

## Pre-requisitos

### Ambiente

- Backend corriendo: `http://localhost:5001`
- Swagger disponible: `http://localhost:5001/swagger`
- Base de datos actualizada con migraciones

### Data de Prueba

Usuario preexistente (en seed):
- **Email:** usuario1@mail.com
- **Password:** 123456
- **Rol:** Fan (cualquier rol autenticado puede crear promotor)
- **Tiene Promotor:** No (limpio para este test)

### Postman/Newman

```bash
npm install -g newman
npm install -g newman-reporter-htmlextra
```

---

## Ejecución Paso a Paso

### Ejecutar Solo Setup

```bash
newman run postman-collection.json \
  --folder "_Setup" \
  --reporters cli
```

### Ejecutar Solo CRUD

```bash
newman run postman-collection.json \
  --folder "Promotor - CRUD Lifecycle" \
  --reporters cli
```

### Ejecutar Todo (Default)

```bash
newman run postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export report.html
```

### Con variables custom

```bash
newman run postman-collection.json \
  --env custom-environment.json \
  --reporters cli
```

---

## Output Esperado (Newman CLI)

```
┌─────────────────────────────────────────────────┐
│                                                 │
│   Newman Integration Test Run                   │
│   Feature: cp-perfil-promotor (US-CP-01)        │
│                                                 │
├─────────────────────────────────────────────────┤

 ●  _Setup
     ✓  01. Login con Usuario de Prueba

 ●  Promotor - CRUD Lifecycle
     ✓  01. POST Create Promotor
     ✓  02. GET Me - Verify Creation
     ✓  03. PUT Update Promotor
     ✓  04. GET Me - Verify Update
     ✓  05. PATCH Desactivar Promotor
     ✓  06. GET Me - Verify Deactivation

 ●  Promotor - Validation Errors
     ✓  POST 400 - nombrePublico Empty
     ✓  POST 400 - nombrePublico Min Length (< 3)
     ✓  POST 400 - nombrePublico Max Length (> 200)
     ✓  POST 400 - tipoPromotorId Missing
     ✓  POST 400 - tipoPromotorId Invalid (> 4)
     ✓  POST 400 - emailContacto Invalid Format
     ✓  POST 400 - urlSitioWeb Invalid Format

 ●  Promotor - Auth Errors
     ✓  POST 401 - Missing Auth Token
     ✓  GET 401 - Missing Auth Token
     ✓  PUT 401 - Invalid Token

 ●  Promotor - Not Found & Business Rules
     ✓  GET 404 - Promotor Not Found
     ✓  POST 400 - Promotor Already Exists
     ✓  PATCH 400 - Already Inactive

┌─────────────────────────────────────────────────┐
│          SUMMARY                                │
├─────────────────────────────────────────────────┤
│ Tests:    20 passed, 20 total                   │
│ Assertions: 64 passed, 64 total                 │
│ Requests: 20 completed, 0 failed                │
│ Duration: 4.523s                                │
└─────────────────────────────────────────────────┘

✓ All tests passed
```

---

## Troubleshooting

### Test falla: "Token no valido o expirado"

1. Verifica que **_Setup → Login** se ejecutó primero
2. Revisa que `accessToken` tiene valor (no vacío)
3. Token JWT expira en 24h, regenera si es muy antiguo

### Test falla: "Ya tienes un perfil de promotor creado"

**Esperado** en test de Business Rules. Si falla antes:
- usuario1@mail.com ya tiene un promotor creado
- Opción: Usa otro usuario o limpia BD

### Newman: "Cannot find module 'newman'"

```bash
npm install -g newman newman-reporter-htmlextra
```

### Requests retornan 404 en GET /me

Asegúrate de que:
1. POST Create retornó 201 (perfil fue creado)
2. `promotorId` se guardó correctamente
3. El mismo `accessToken` se usa en GET (mismo usuario)

---

## Notas de Implementación

### Variables Dinámicas

- `promotorName` se genera con `Date.now()` en cada ejecución → garantiza unicidad
- `accessToken` se obtiene dinámicamente en _Setup → no hardcoded
- `promotorId` se extrae de response y se reutiliza en requests siguientes

### Patrones CQRS

- **Commands:** POST (Create), PUT (Update), PATCH (Deactivate)
- **Queries:** GET (GetMyProfile)
- **Response:** Todos retornan `ServiceResponse<T>` con `data` y `messages[]`
- **Validación:** Retorna 400 con `messages[].errorCode` específico

### Diseño sin _Teardown

El endpoint **PATCH Deactivate** hace desactivación lógica (`EsActivo = false`), no DELETE físico. Por lo tanto:
- No hay _Teardown necesario
- El registro permanece en BD
- No afecta ejecuciones futuras

---

## Siguientes Pasos (Post-implementación)

1. **Implementar endpoints** según `api-contracts.md`
2. **Ejecutar colección** contra backend funcionando
3. **Revisar errores** de validación devueltos
4. **Ajustar** si algún error code difiere
5. **Generar reporte HTML** con `--reporter-htmlextra`
6. **Archivar reporte** con cada release

---

## Referencias Rápidas

| Recurso | Ubicación |
|---------|-----------|
| Colección JSON | `plans/cp-perfil-promotor/backend/postman-collection.json` |
| Contratos API | `docs/user-stories/cp-perfil-promotor/contracts.md` |
| Plan Detallado | `plans/cp-perfil-promotor/backend/api-contracts.md` |
| Guía Postman | `plans/cp-perfil-promotor/backend/POSTMAN_COLLECTION_GUIDE.md` |
| CQRS Rules | `.claude/rules/backend/cqrs.rule.md` |
| Feature Spec | `docs/user-stories/cp-perfil-promotor/feature-spec.md` |

---

**Generado:** 2026-02-25 | **Proyecto:** WePlay Rises | **Feature:** cp-perfil-promotor (US-CP-01)
