# Colección Postman: Perfil de Promotor (cp-perfil-promotor)

**Feature:** US-CP-01 - Crear y gestionar perfil de promotor
**Colección:** `postman-collection.json`
**Última actualización:** 2026-02-25

---

## Descripción

Colección de pruebas de integración para validar el lifecycle completo de gestión del perfil de promotor en el módulo Crowdpromotion. Prueba los 4 endpoints principales:

1. **POST /api/crowdpromotion/promotor** - Crear perfil
2. **GET /api/crowdpromotion/promotor/me** - Obtener perfil propio
3. **PUT /api/crowdpromotion/promotor/me** - Actualizar perfil
4. **PATCH /api/crowdpromotion/promotor/me/desactivar** - Desactivar perfil

---

## Estructura de la Colección

### Folders Principales

| Folder | Descripción | Requests |
|--------|-------------|----------|
| **_Setup** | Autenticación inicial | Login con usuario1@mail.com |
| **Promotor - CRUD Lifecycle** | Flujo completo de operaciones | 6 pasos (Create → GET → Update → GET → Deactivate → GET) |
| **Promotor - Validation Errors** | Validaciones por campo | 7 casos de validación (required, min/max, format) |
| **Promotor - Auth Errors** | Errores de autenticación | 3 casos sin token o token inválido |
| **Promotor - Not Found & Business Rules** | Errores de negocio | 3 casos (404 no existe, 400 ya existe, 400 ya desactivado) |

**Total:** 19 requests con ~65 assertions

---

## Setup: Variables de Colección

La colección utiliza **Collection Variables** (no Environment Variables) para máxima portabilidad.

### Variables Base

| Variable | Valor Inicial | Descripción |
|----------|---------------|-------------|
| `baseUrl` | `http://localhost:5001` | URL del backend |
| `testEmail` | `usuario1@mail.com` | Email de usuario de prueba (existe en seed) |
| `testPassword` | `123456` | Password de usuario de prueba |
| `accessToken` | (vacío) | JWT obtenido en _Setup → Login |
| `promotorId` | (vacío) | ID del promotor creado en lifecycle |
| `promotorName` | (vacío) | Nombre único generado con timestamp |

### Variables Adicionales

| Variable | Valor Inicial | Descripción |
|----------|---------------|-------------|
| `newUserEmail` | (vacío) | Email único para usuario sin promotor |
| `newUserPassword` | `TestPassword123!` | Password de prueba |
| `newUserToken` | (vacío) | JWT del nuevo usuario |

---

## Instrucciones de Ejecución

### Opción 1: Ejecutar en Postman UI

1. Importa `postman-collection.json` en Postman
2. Asegúrate de que el backend corre en `http://localhost:5001`
3. En la colección, abre **_Setup**
4. Haz clic en "Send" en el request **Login**
   - Obtiene el JWT y lo guarda en `accessToken`
5. En la carpeta **Promotor - CRUD Lifecycle**, ejecuta los requests en orden:
   - 01. POST Create → genera `promotorId`
   - 02. GET Me (verify creation)
   - 03. PUT Update
   - 04. GET Me (verify update)
   - 05. PATCH Deactivate
   - 06. GET Me (verify deactivation)

### Opción 2: Ejecutar con Newman (CLI)

#### Requisitos

```bash
npm install -g newman
npm install -g newman-reporter-htmlextra  # Para reporte HTML
```

#### Ejecutar colección completa

```bash
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html
```

#### Ejecutar carpeta específica (ej. solo CRUD Lifecycle)

```bash
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --folder "Promotor - CRUD Lifecycle" \
  --reporters cli
```

#### Con variables custom

```bash
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --env environment.json \
  --reporters cli,htmlextra
```

Archivo `environment.json`:
```json
{
  "id": "cp-perfil-promotor-env",
  "values": [
    { "key": "baseUrl", "value": "http://localhost:5001", "enabled": true },
    { "key": "testEmail", "value": "usuario1@mail.com", "enabled": true },
    { "key": "testPassword", "value": "123456", "enabled": true }
  ]
}
```

---

## Flujo de Ejecución: CRUD Lifecycle

### 01. POST Create Promotor

**Endpoint:** `POST /api/crowdpromotion/promotor`

**Genera:**
- `promotorId` (extraído de `response.data.id`)
- `promotorName` (generado con timestamp en pre-request)

**Body:**
```json
{
  "nombrePublico": "DJ Marketing Pro {timestamp}",
  "tipoPromotorId": 2,
  "emailContacto": "contacto@djmarketing.com",
  "urlSitioWeb": "https://djmarketing.com",
  "urlInstagram": "https://instagram.com/djmarketing",
  "urlTikTok": "https://tiktok.com/@djmarketing",
  "urlYouTube": null,
  "urlTwitter": null
}
```

**Assertions:**
- ✓ Status 201 Created
- ✓ Response time < 500ms
- ✓ `data.id` existe y se guarda en `promotorId`
- ✓ `data.nombrePublico` coincide con el enviado
- ✓ `data.tipoPromotorNombre` = "Influencer"
- ✓ `data.esActivo` = true
- ✓ `data.fechaCreacion` existe

---

### 02. GET Me - Verify Creation

**Endpoint:** `GET /api/crowdpromotion/promotor/me`

**Assertions:**
- ✓ Status 200
- ✓ `data.id` = `promotorId` (mismo que POST)
- ✓ `data.nombrePublico` = nombre enviado
- ✓ `data.tipoPromotorId` = 2
- ✓ `data.tipoPromotorNombre` existe
- ✓ `data.emailContacto` = "contacto@djmarketing.com"
- ✓ `data.urlSitioWeb` = "https://djmarketing.com"
- ✓ `data.esActivo` = true
- ✓ `data.totalProgramasActivos` es número
- ✓ `data.totalComisionesGanadas` es número
- ✓ `data.monedaComisiones` = "EUR"

---

### 03. PUT Update Promotor

**Endpoint:** `PUT /api/crowdpromotion/promotor/me`

**Body:**
```json
{
  "nombrePublico": "DJ Marketing Pro (Updated)",
  "emailContacto": "nuevo@djmarketing.com",
  "urlSitioWeb": "https://djmarketing.com",
  "urlInstagram": "https://instagram.com/djmarketing",
  "urlTikTok": "https://tiktok.com/@djmarketing",
  "urlYouTube": "https://youtube.com/@djmarketing",
  "urlTwitter": null
}
```

**Assertions:**
- ✓ Status 200
- ✓ `data.id` = `promotorId` (mismo promotor)
- ✓ `data.nombrePublico` = "DJ Marketing Pro (Updated)"
- ✓ `data.fechaActualizacion` existe

---

### 04. GET Me - Verify Update

**Endpoint:** `GET /api/crowdpromotion/promotor/me`

**Assertions:**
- ✓ Status 200
- ✓ `data.nombrePublico` = "DJ Marketing Pro (Updated)"
- ✓ `data.emailContacto` = "nuevo@djmarketing.com"
- ✓ `data.urlYouTube` = "https://youtube.com/@djmarketing"
- ✓ `data.esActivo` = true (aún activo)

---

### 05. PATCH Deactivate

**Endpoint:** `PATCH /api/crowdpromotion/promotor/me/desactivar`

**Body:** Vacío (sin body)

**Assertions:**
- ✓ Status 200
- ✓ `data.id` = `promotorId`
- ✓ `data.esActivo` = false
- ✓ `data.programasDadosDeBaja` es número (0 en este caso)

---

### 06. GET Me - Verify Deactivation

**Endpoint:** `GET /api/crowdpromotion/promotor/me`

**Assertions:**
- ✓ Status 200
- ✓ `data.esActivo` = false (confirmado desactivado)

---

## Casos de Error

### Validation Errors (7 tests)

Todos retornan **HTTP 400** con error codes específicos:

| Test | Campo | Validación | Error Code |
|------|-------|-----------|-----------|
| nombrePublico Empty | nombrePublico | Requerido | 1001 |
| nombrePublico MinLength | nombrePublico | Min 3 chars | 1011 |
| nombrePublico MaxLength | nombrePublico | Max 200 chars | 1002 |
| tipoPromotorId Missing | tipoPromotorId | Requerido | 1001 |
| tipoPromotorId Invalid | tipoPromotorId | Debe existir | 1010 |
| emailContacto Invalid | emailContacto | Formato email | 1003 |
| urlSitioWeb Invalid | urlSitioWeb | Formato URL | 1013 |

---

### Auth Errors (3 tests)

Todos retornan **HTTP 401**:

| Test | Descripción |
|------|-------------|
| POST 401 - Missing Auth | Sin header Authorization |
| GET 401 - Missing Auth | Sin header Authorization |
| PUT 401 - Invalid Token | Token malformado |

---

### Not Found & Business Rules (3 tests)

| Test | Endpoint | Status | Error Code | Causa |
|------|----------|--------|-----------|-------|
| GET 404 Promotor Not Found | GET /me | 404 | 2015 | Usuario sin perfil de promotor |
| POST 400 Already Exists | POST / | 400 | 4018 | Usuario ya tiene perfil |
| PATCH 400 Already Inactive | PATCH /desactivar | 400 | 4019 | Perfil ya desactivado |

---

## Configuración de Ambiente

### Docker (Recomendado para desarrollo)

```bash
docker compose up -d
```

Backend estará en `http://localhost:5001`

Usuarios de prueba preexistentes:
- **Email:** usuario1@mail.com
- **Password:** 123456
- **Ya tiene:** Perfil de Artista (opcional, no usado en cp-perfil-promotor)

### LocalDB (Si usas base de datos local)

Asegúrate de que:
1. Base de datos `WePlayRises` existe
2. Migraciones aplicadas
3. Seed data ejecutado (usuarios de prueba creados)

Backend corre en `http://localhost:5001` por defecto

---

## Metricas de la Colección

```
Folder: _Setup
└── 1 request
    └── 1 assertion

Folder: Promotor - CRUD Lifecycle
└── 6 requests (numerados 01-06)
    └── ~20 assertions

Folder: Promotor - Validation Errors
└── 7 requests
    └── ~14 assertions (2 cada uno)

Folder: Promotor - Auth Errors
└── 3 requests
    └── 3 assertions (1 cada uno)

Folder: Promotor - Not Found & Business Rules
└── 3 requests
    └── ~6 assertions

TOTAL:
- 20 requests
- ~64 assertions
- 4 folders + 1 setup
```

---

## Patrones Usados

### Pre-request Scripts

```javascript
// Generar email único con timestamp
const email = 'test-' + Date.now() + '@weplay.com';
pm.collectionVariables.set('promotorName', 'DJ Marketing Pro ' + timestamp);
```

### Test Scripts

```javascript
// Guardar valor para siguiente request
pm.collectionVariables.set('promotorId', json.data.id);

// Validar error code específico
const messages = json.messages || [];
const found = messages.some(m => m.errorCode === '4018');
pm.expect(found).to.be.true;

// Validar múltiples campos
pm.expect(json.data.nombrePublico).to.equal(pm.collectionVariables.get('promotorName'));
pm.expect(json.data.esActivo).to.be.true;
```

### Variables de Colección

```
pm.collectionVariables.get('promotorId')
pm.collectionVariables.set('accessToken', token)
```

---

## Troubleshooting

### Error: "Token no valido o expirado"

**Causa:** Token JWT expirado o no fue obtenido en _Setup

**Solución:**
1. Ejecuta nuevamente **_Setup → Login**
2. Verifica que `accessToken` tiene valor

### Error: "Ya tienes un perfil de promotor creado" (4018)

**Causa:** usuario1@mail.com ya tiene un perfil creado

**Solución:**
1. Opción A: Usar otro usuario de prueba
2. Opción B: Eliminar el promotor de la BD y reintentar
3. Opción C: En test de negocio, es el comportamiento esperado

### Error: "El tipo de promotor no existe" (1010)

**Causa:** tipoPromotorId no es 1-4

**Solución:** En los tests, siempre usar valores 1-4:
- 1 = Fan Embajador
- 2 = Influencer
- 3 = Medio / Blog
- 4 = Profesional Marketing

### Colección importada pero variables vacías

**Causa:** Las variables se generan dinámicamente durante ejecución

**Solución:** Ejecuta siempre en orden:
1. _Setup (Login)
2. CRUD Lifecycle (Create, GET, Update, GET, Deactivate, GET)

---

## Notas de Implementación

### Dependencias Creadas en _Setup

- **Login:** Obtiene JWT con `accessToken`
- **Usuario:** usuario1@mail.com (preexistente en seed)

### Dependencias Limpias en Lifecycle

El endpoint **PATCH Deactivate** solo hace desactivación lógica (`EsActivo = false`). El registro no se elimina de BD, por lo que no hay limpieza de _Teardown necesaria.

### Validación de Respuesta

Todas las respuestas siguen el patrón `ServiceResponse<T>`:
```json
{
  "data": { ... },
  "messages": [
    { "message": "...", "errorCode": "0001" }
  ]
}
```

Los tests validan `json.data` (en 2xx) o `json.messages[]` (en errores).

---

## Referencias

- **Contratos API:** `docs/user-stories/cp-perfil-promotor/contracts.md`
- **Plan API:** `plans/cp-perfil-promotor/backend/api-contracts.md`
- **Feature Spec:** `docs/user-stories/cp-perfil-promotor/feature-spec.md`
- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`

---

## Autor & Contacto

**Generado por:** Claude Code Agent
**Fecha:** 2026-02-25
**Proyecto:** WePlay Rises - MVP Crowdfunding Musical
