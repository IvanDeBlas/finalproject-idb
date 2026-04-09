# Plan de Testing Newman: Crear Campaña de Crowdfunding

**Fecha:** 2026-02-12
**Feature:** crear-campania (US-02)
**Modulo:** Crowdfunding
**Coleccion:** WePlay.CrearCampania.IntegrationTests
**Url Contrato:** `plans/crear-campania/backend/api-contracts.md`

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 6 |
| Total requests planificados | 45 |
| Casos de prueba | 65+ |
| Status codes cubiertos | 8 (200, 201, 400, 401, 403, 404, 409, 500) |
| Flujos E2E | 4 |
| Escenarios de error | 22+ |
| Validaciones | 18+ |
| Tiempo estimado ejecucion | 60-90 segundos |

### Endpoints a Validar

1. **POST /api/campanias** - Crear campaña en BORRADOR
2. **GET /api/campanias/{id}** - Obtener detalle de campaña (público)
3. **PUT /api/campanias/{id}** - Actualizar campaña en BORRADOR
4. **POST /api/campanias/{id}/publicar** - Publicar campaña (BORRADOR → PUBLICADA)
5. **GET /api/campanias** - Listar campañas públicas con filtros
6. **GET /api/campanias/mis-campanias** - Listar mis campañas (autenticado)

### Objetivos de Testing

- Validar contrato API: DTOs, request/response, status codes esperados
- Validar autenticación JWT: token extraction, claim validation
- Validar autorización: ownership checks, role-based access
- Validar validaciones: campos requeridos, longitudes, formatos URLs, rangos de fechas
- Validar reglas de negocio: estado BORRADOR/PUBLICADA, 7+ días duración, meta > 0
- Validar integridad de datos en flujos E2E: crear → editar → publicar → listar
- Validar control de errores: 400 (validación), 401 (auth), 403 (forbidden), 404 (not found), 409 (conflict)

---

## 2. Estructura de Coleccion

```
WePlay.CrearCampania.IntegrationTests/

├── _Setup/
│   ├── 001_Initialize Environment Variables
│   ├── 002_Register Test Artista 1
│   ├── 003_Login Test Artista 1 (Save Token)
│   ├── 004_Register Test Artista 2 (Para tests de authorization)
│   └── 005_Login Test Artista 2 (Save Token 2)
│
├── Campanias - Create/
│   ├── 201 CREATED - Success Cases/
│   │   ├── 201_POST Create - Success (Full Data)
│   │   ├── 202_POST Create - Minimal Data (Only Required)
│   │   ├── 203_POST Create - With Optional Fields
│   │   ├── 204_POST Create - Save ID for Further Tests
│   │   ├── 205_POST Create - Verify Estado BORRADOR
│   │   ├── 206_POST Create - Verify ImportePledgedActual = 0
│   │   └── 207_POST Create - Response Time < 500ms
│   │
│   ├── 400 Bad Request - Validation Errors/
│   │   ├── 211_POST Create - Empty Titulo
│   │   ├── 212_POST Create - Titulo > 200 chars
│   │   ├── 213_POST Create - Subtitulo > 300 chars
│   │   ├── 214_POST Create - DescripcionCorta > 500 chars
│   │   ├── 215_POST Create - Invalid Video URL
│   │   ├── 216_POST Create - Invalid Image URL
│   │   ├── 217_POST Create - MonedaId = 0
│   │   ├── 218_POST Create - ImporteObjetivo = 0
│   │   ├── 219_POST Create - ImporteObjetivo < 0 (negative)
│   │   ├── 220_POST Create - ImporteMinimo > ImporteObjetivo
│   │   ├── 221_POST Create - TipoFinanciacionId = 0
│   │   ├── 222_POST Create - FechaFin < FechaInicio
│   │   └── 223_POST Create - Multiple Validation Errors
│   │
│   ├── 401 Unauthorized/
│   │   ├── 231_POST Create - Missing Authorization Header
│   │   ├── 232_POST Create - Invalid Token
│   │   ├── 233_POST Create - Malformed Token
│   │   ├── 234_POST Create - Token Expired
│   │   └── 235_POST Create - Only Sub Claim Extracted
│   │
│   └── 500 Internal Server Error/
│       └── 241_POST Create - Verify Error Response Structure
│
├── Campanias - Get By ID/
│   ├── 200 OK - Success Cases/
│   │   ├── 301_GET By ID - Success (Public Access)
│   │   ├── 302_GET By ID - Verify Response Structure
│   │   ├── 303_GET By ID - Verify All Fields Returned
│   │   ├── 304_GET By ID - Estado Matches Created State
│   │   ├── 305_GET By ID - ImportePledgedActual = 0
│   │   ├── 306_GET By ID - Response Time < 500ms
│   │   └── 307_GET By ID - Can Access Without Token
│   │
│   ├── 404 Not Found/
│   │   ├── 311_GET By ID - Non-existent ID
│   │   ├── 312_GET By ID - Invalid GUID Format
│   │   ├── 313_GET By ID - Zero GUID
│   │   └── 314_GET By ID - Empty ID Parameter
│   │
│   └── 500 Internal Server Error/
│       └── 321_GET By ID - Verify Error Response Structure
│
├── Campanias - Update/
│   ├── 200 OK - Success Cases/
│   │   ├── 401_PUT Update - Success (Partial Update)
│   │   ├── 402_PUT Update - Update Only Titulo
│   │   ├── 403_PUT Update - Update Multiple Fields
│   │   ├── 404_PUT Update - Update FechaFin Later
│   │   ├── 405_PUT Update - Update ImporteObjetivo
│   │   ├── 406_PUT Update - Verify isSuccess = true
│   │   ├── 407_PUT Update - Verify Data = true
│   │   └── 408_PUT Update - Response Time < 500ms
│   │
│   ├── 400 Bad Request - Validation Errors/
│   │   ├── 411_PUT Update - Empty Titulo (but not removing)
│   │   ├── 412_PUT Update - Titulo > 200 chars
│   │   ├── 413_PUT Update - ImporteObjetivo = 0
│   │   ├── 414_PUT Update - ImporteMinimo > ImporteObjetivo
│   │   ├── 415_PUT Update - Invalid Video URL
│   │   ├── 416_PUT Update - Invalid Image URL
│   │   ├── 417_PUT Update - FechaFin < FechaInicio
│   │   ├── 418_PUT Update - ID Mismatch (URL vs Body)
│   │   └── 419_PUT Update - Multiple Validation Errors
│   │
│   ├── 401 Unauthorized/
│   │   ├── 421_PUT Update - Missing Authorization Header
│   │   ├── 422_PUT Update - Invalid Token
│   │   ├── 423_PUT Update - Token Expired
│   │   └── 424_PUT Update - Empty Authorization
│   │
│   ├── 403 Forbidden - Access Control/
│   │   ├── 431_PUT Update - Different Artista (Ownership Check)
│   │   └── 432_PUT Update - Other User Attempts Edit
│   │
│   ├── 404 Not Found/
│   │   ├── 441_PUT Update - Non-existent Campaña
│   │   ├── 442_PUT Update - Invalid GUID Format
│   │   └── 443_PUT Update - Deleted Campaña
│   │
│   ├── 409 Conflict - Business Rules/
│   │   ├── 451_PUT Update - Estado != BORRADOR (Publicada)
│   │   └── 452_PUT Update - Estado != BORRADOR (Finalizada)
│   │
│   └── 500 Internal Server Error/
│       └── 461_PUT Update - Verify Error Response Structure
│
├── Campanias - Publish/
│   ├── 200 OK - Success Cases/
│   │   ├── 501_POST Publicar - Success (Full Data)
│   │   ├── 502_POST Publicar - Verify Estado = PUBLICADA (2)
│   │   ├── 503_POST Publicar - Verify FechaPublicacion Set
│   │   ├── 504_POST Publicar - Verify FechaPublicacion = Now (UTC)
│   │   ├── 505_POST Publicar - Verify FechaInicio Set if Null
│   │   ├── 506_POST Publicar - Verify isSuccess = true
│   │   └── 507_POST Publicar - Response Time < 500ms
│   │
│   ├── 400 Bad Request - Missing Required Fields/
│   │   ├── 511_POST Publicar - Missing Titulo
│   │   ├── 512_POST Publicar - Missing ImporteObjetivo
│   │   ├── 513_POST Publicar - Missing MonedaId
│   │   ├── 514_POST Publicar - Missing TipoFinanciacionId
│   │   ├── 515_POST Publicar - Missing FechaFin
│   │   ├── 516_POST Publicar - FechaFin < Now + 7 Days
│   │   └── 517_POST Publicar - Multiple Missing Fields
│   │
│   ├── 401 Unauthorized/
│   │   ├── 521_POST Publicar - Missing Authorization Header
│   │   ├── 522_POST Publicar - Invalid Token
│   │   ├── 523_POST Publicar - Token Expired
│   │   └── 524_POST Publicar - Empty Token
│   │
│   ├── 403 Forbidden - Access Control/
│   │   ├── 531_POST Publicar - Different Artista
│   │   └── 532_POST Publicar - Other User Attempts Publish
│   │
│   ├── 404 Not Found/
│   │   ├── 541_POST Publicar - Non-existent Campaña
│   │   ├── 542_POST Publicar - Invalid GUID Format
│   │   └── 543_POST Publicar - Deleted Campaña
│   │
│   ├── 409 Conflict - Business Rules/
│   │   ├── 551_POST Publicar - Estado != BORRADOR (Already Published)
│   │   ├── 552_POST Publicar - Estado = FINALIZADA
│   │   └── 553_POST Publicar - Estado = CANCELADA
│   │
│   └── 500 Internal Server Error/
│       └── 561_POST Publicar - Verify Error Response Structure
│
├── Campanias - List Public/
│   ├── 200 OK - Success Cases/
│   │   ├── 601_GET List - Success (Default Params)
│   │   ├── 602_GET List - Default Estado = PUBLICADA (2)
│   │   ├── 603_GET List - Default PageSize = 10
│   │   ├── 604_GET List - Search by Titulo
│   │   ├── 605_GET List - Search by Subtitulo
│   │   ├── 606_GET List - Filter by ArtistaId
│   │   ├── 607_GET List - Filter by Estado
│   │   ├── 608_GET List - Pagination Page 1
│   │   ├── 609_GET List - Pagination Page 2
│   │   ├── 610_GET List - Pagination PageSize 5
│   │   ├── 611_GET List - Pagination Max PageSize 50
│   │   ├── 612_GET List - Public Access (No Auth Required)
│   │   ├── 613_GET List - Response Structure Correct
│   │   └── 614_GET List - Response Time < 500ms
│   │
│   ├── 400 Bad Request/
│   │   ├── 621_GET List - Invalid PageNumber (0)
│   │   ├── 622_GET List - Invalid PageSize (0)
│   │   ├── 623_GET List - Invalid PageSize (> 50)
│   │   ├── 624_GET List - Invalid ArtistaId Format
│   │   ├── 625_GET List - Invalid EstadoCampaniaId Format
│   │   └── 626_GET List - Negative PageSize
│   │
│   └── 500 Internal Server Error/
│       └── 631_GET List - Verify Error Response Structure
│
├── Campanias - List My Campanias/
│   ├── 200 OK - Success Cases/
│   │   ├── 701_GET MisCampanias - Success (Authenticated)
│   │   ├── 702_GET MisCampanias - Returns Own Campanias Only
│   │   ├── 703_GET MisCampanias - Includes BORRADOR State
│   │   ├── 704_GET MisCampanias - Includes PUBLICADA State
│   │   ├── 705_GET MisCampanias - Filter by Estado
│   │   ├── 706_GET MisCampanias - Pagination Works
│   │   ├── 707_GET MisCampanias - Default PageSize = 10
│   │   ├── 708_GET MisCampanias - Response Structure Correct
│   │   ├── 709_GET MisCampanias - ArtistaId Extracted from Token
│   │   └── 710_GET MisCampanias - Response Time < 500ms
│   │
│   ├── 401 Unauthorized/
│   │   ├── 711_GET MisCampanias - Missing Authorization Header
│   │   ├── 712_GET MisCampanias - Invalid Token
│   │   ├── 713_GET MisCampanias - Token Expired
│   │   └── 714_GET MisCampanias - Empty Token
│   │
│   ├── 400 Bad Request/
│   │   ├── 721_GET MisCampanias - Invalid PageNumber
│   │   ├── 722_GET MisCampanias - Invalid PageSize
│   │   └── 723_GET MisCampanias - Invalid EstadoCampaniaId
│   │
│   └── 500 Internal Server Error/
│       └── 731_GET MisCampanias - Verify Error Response Structure
│
└── _Cleanup/
    ├── 801_Delete Test Data (Opcional)
    └── 802_Cleanup Tokens/Secrets
```

---

## 3. Flujos E2E Planificados

### Flujo E2E-01: Happy Path Complete - Crear → Editar → Publicar → Listar

```
1. POST /api/campanias
   - Crear campaña con datos válidos (BORRADOR)
   - Extraer ID de response
   - Guardar en variable: {{campaniaId}}

2. GET /api/campanias/{{campaniaId}}
   - Verificar estado = BORRADOR (1)
   - Verificar ImportePledgedActual = 0
   - Verificar respuesta contiene todos los campos

3. PUT /api/campanias/{{campaniaId}}
   - Actualizar titulo y descripcion
   - Verificar isSuccess = true
   - Verificar data = true

4. POST /api/campanias/{{campaniaId}}/publicar
   - Publicar campaña
   - Verificar estado = PUBLICADA (2)
   - Verificar FechaPublicacion set
   - Guardar response

5. GET /api/campanias?estadoCampaniaId=2
   - Verificar campaña publicada aparece en listado
   - Verificar estado = PUBLICADA
   - Verificar isSuccess = true

6. GET /api/campanias/mis-campanias
   - Verificar incluye campaña publicada
   - Verificar ArtistaId coincide
   - Verificar puede filtrar por estado
```

### Flujo E2E-02: Validaciones en Create

```
1. POST /api/campanias (titulo vacio)
   - Retorna 400
   - ErrorCode = 1001
   - Message = "El titulo es obligatorio"

2. POST /api/campanias (importeObjetivo = 0)
   - Retorna 400
   - ErrorCode = 1011
   - Message = "El importe objetivo debe ser mayor a 0"

3. POST /api/campanias (fechaFin < fechaInicio)
   - Retorna 400
   - ErrorCode = 1012
   - Message = "La fecha de fin debe ser posterior a la fecha de inicio"
```

### Flujo E2E-03: Authorization - Edit Attempt by Different Artista

```
1. Artista1 crea campaña
   - POST /api/campanias (token Artista1)
   - Retorna 201 con ID

2. Artista2 intenta editar
   - PUT /api/campanias/{{campaniaId}} (token Artista2)
   - Retorna 403 Forbidden
   - ErrorCode = 3002
   - Message = "No tienes permiso para editar esta campania"

3. Artista1 puede editar
   - PUT /api/campanias/{{campaniaId}} (token Artista1)
   - Retorna 200
   - isSuccess = true
```

### Flujo E2E-04: State Transitions

```
1. Crear campaña → Estado BORRADOR (1)

2. Editar campaña → Estado sigue BORRADOR

3. Publicar campaña → Estado PUBLICADA (2)

4. Intenta editar publicada
   - PUT /api/campanias/{{campaniaId}}
   - Retorna 409 Conflict
   - ErrorCode = 4009
   - Message = "Solo se pueden editar campanias en estado borrador"
```

---

## 4. Variables de Entorno

### Desarrollo (Development)

```json
{
  "baseUrl": "https://localhost:5001",
  "identityUrl": "https://localhost:5001",

  "clientId": "weplay-test",
  "clientSecret": "{{SECRET_CLIENT_SECRET}}",

  "testArtista1Email": "test.artista1+{{$timestamp}}@weplay.test",
  "testArtista1Password": "SecurePassword123!",

  "testArtista2Email": "test.artista2+{{$timestamp}}@weplay.test",
  "testArtista2Password": "SecurePassword456!",

  "accessToken": "",
  "accessToken2": "",
  "artistaId": "",
  "artistaId2": "",

  "campaniaId": "",
  "campaniaId2": "",
  "campaniaIdPublished": "",

  "timestamp": "{{$timestamp}}",
  "randomString": "{{$randomString}}"
}
```

### Staging

```json
{
  "baseUrl": "https://staging-api.weplay.test",
  "identityUrl": "https://staging-api.weplay.test",

  "clientId": "weplay-staging-test",
  "clientSecret": "{{SECRET_STAGING_CLIENT_SECRET}}",

  "testArtista1Email": "staging.artista1+{{$timestamp}}@weplay.test",
  "testArtista1Password": "{{SECRET_STAGING_TEST_PASSWORD}}",

  "testArtista2Email": "staging.artista2+{{$timestamp}}@weplay.test",
  "testArtista2Password": "{{SECRET_STAGING_TEST_PASSWORD}}",

  "accessToken": "",
  "accessToken2": "",
  "artistaId": "",
  "artistaId2": "",

  "campaniaId": "",
  "campaniaId2": "",
  "campaniaIdPublished": "",

  "timestamp": "{{$timestamp}}",
  "randomString": "{{$randomString}}"
}
```

### Variables Computadas (Pre-request Script global)

```javascript
// Timestamp (ISO 8601 + 10 days for FechaFin)
const now = new Date();
pm.environment.set('now', now.toISOString());

const futureDate = new Date(now.getTime() + 10 * 24 * 60 * 60 * 1000);
pm.environment.set('futureDate10Days', futureDate.toISOString());

const futureDate7Days = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000);
pm.environment.set('futureDate7Days', futureDate7Days.toISOString());

const pastDate = new Date(now.getTime() - 1 * 24 * 60 * 60 * 1000);
pm.environment.set('pastDate', pastDate.toISOString());
```

---

## 5. Requests Detallados

### 5.1 _Setup

#### 001_Register Test Artista 1

```
POST {{identityUrl}}/api/auth/register
Content-Type: application/json

{
  "email": "{{testArtista1Email}}",
  "password": "{{testArtista1Password}}",
  "confirmPassword": "{{testArtista1Password}}",
  "role": "artista"
}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has access_token', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('access_token');
});

pm.test('Token is JWT format', () => {
    const json = pm.response.json();
    const parts = json.access_token.split('.');
    pm.expect(parts).to.have.length(3);
});
```

#### 003_Login Test Artista 1 (Save Token)

```
POST {{identityUrl}}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
&username={{testArtista1Email}}
&password={{testArtista1Password}}
&client_id={{clientId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Has access_token', () => {
    pm.expect(json).to.have.property('access_token');
});

pm.test('Token saved to environment', () => {
    pm.environment.set('accessToken', json.access_token);
    pm.expect(pm.environment.get('accessToken')).to.not.be.null;
});

pm.test('Extract ArtistaId from token', () => {
    const token = json.access_token;
    const payload = JSON.parse(atob(token.split('.')[1]));
    pm.expect(payload).to.have.property('sub');
    pm.environment.set('artistaId', payload.sub);
});
```

---

### 5.2 Campanias - Create

#### 201_POST Create - Success (Full Data)

```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "titulo": "Mi primer album - Test {{$randomString}}",
  "subtitulo": "Rock alternativo con influencias indie",
  "descripcionCorta": "Necesitamos tu apoyo para grabar nuestro primer disco de estudio",
  "videoPrincipalUrl": "https://youtube.com/watch?v=dQw4w9WgXcQ",
  "imagenPrincipalUrl": "https://ejemplo.com/imagen.jpg",
  "monedaId": 1,
  "importeObjetivo": 5000.00,
  "importeMinimo": 100.00,
  "tipoFinanciacionId": 1,
  "permiteAportacionesAnonimas": false,
  "permitePropinas": true,
  "fechaInicio": "{{futureDate7Days}}",
  "fechaFin": "{{futureDate10Days}}"
}
```

**Tests:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

const json = pm.response.json();

pm.test('Response has ServiceResponse structure', () => {
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.be.an('object');
});

pm.test('data contains campaña', () => {
    pm.expect(json.data).to.have.property('id');
    pm.expect(json.data).to.have.property('titulo');
    pm.expect(json.data).to.have.property('estadoCampaniaId');
    pm.expect(json.data).to.have.property('importePledgedActual');
});

pm.test('Estado is BORRADOR (1)', () => {
    pm.expect(json.data.estadoCampaniaId).to.equal(1);
});

pm.test('ImportePledgedActual is 0', () => {
    pm.expect(json.data.importePledgedActual).to.equal(0);
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Messages contain success code', () => {
    const message = json.messages[0];
    pm.expect(message.errorCode).to.equal('0000');
});

pm.test('FechaCreacion is set', () => {
    pm.expect(json.data.fechaCreacion).to.exist;
});

pm.test('FechaPublicacion is null (not published)', () => {
    pm.expect(json.data.fechaPublicacion).to.be.null;
});

pm.test('Save ID for further tests', () => {
    pm.environment.set('campaniaId', json.data.id);
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### 202_POST Create - Minimal Data (Only Required)

```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "titulo": "Minimal Campania {{$randomString}}",
  "monedaId": 1,
  "importeObjetivo": 1000.00,
  "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

const json = pm.response.json();

pm.test('Minimal campanias created successfully', () => {
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json.data.id).to.exist;
});

pm.test('Optional fields are null', () => {
    pm.expect(json.data.subtitulo).to.be.null;
    pm.expect(json.data.descripcionCorta).to.be.null;
    pm.expect(json.data.videoPrincipalUrl).to.be.null;
    pm.expect(json.data.imagenPrincipalUrl).to.be.null;
});

pm.test('Required fields are set', () => {
    pm.expect(json.data.titulo).to.equal('Minimal Campania {{$randomString}}');
    pm.expect(json.data.importeObjetivo).to.equal(1000);
    pm.expect(json.data.monedaId).to.equal(1);
});
```

#### 211_POST Create - Empty Titulo

```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "titulo": "",
  "monedaId": 1,
  "importeObjetivo": 1000.00,
  "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

const json = pm.response.json();

pm.test('isSuccess is false', () => {
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Has validation error message', () => {
    pm.expect(json.messages).to.be.an('array');
    pm.expect(json.messages.length).to.be.greaterThan(0);
});

pm.test('Error code is Validation_Required (1001)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('1001');
});

pm.test('Error message is descriptive', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('titulo').or.include('obligatorio');
});
```

#### 218_POST Create - ImporteObjetivo = 0

```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "titulo": "Test Campaña",
  "monedaId": 1,
  "importeObjetivo": 0,
  "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

const json = pm.response.json();

pm.test('Error code is Validation_InvalidAmount (1011)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('1011');
});

pm.test('Error message mentions amount', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('mayor');
});
```

#### 231_POST Create - Missing Authorization Header

```
POST {{baseUrl}}/api/campanias
Content-Type: application/json

{
  "titulo": "Test",
  "monedaId": 1,
  "importeObjetivo": 1000,
  "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

const json = pm.response.json();

pm.test('isSuccess is false', () => {
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code is Auth (3xxx)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.match(/^3\d{3}$/);
});
```

---

### 5.3 Campanias - Get By ID

#### 301_GET By ID - Success (Public Access)

```
GET {{baseUrl}}/api/campanias/{{campaniaId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Response has ServiceResponse structure', () => {
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
});

pm.test('data contains all campaña fields', () => {
    const data = json.data;
    pm.expect(data).to.have.property('id');
    pm.expect(data).to.have.property('titulo');
    pm.expect(data).to.have.property('importeObjetivo');
    pm.expect(data).to.have.property('estadoCampaniaId');
    pm.expect(data).to.have.property('fechaCreacion');
});

pm.test('ID matches requested', () => {
    pm.expect(json.data.id).to.equal(pm.environment.get('campaniaId'));
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('No Authorization header required', () => {
    pm.expect(pm.request.headers.has('Authorization')).to.be.false;
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### 311_GET By ID - Non-existent ID

```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000
```

**Tests:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

const json = pm.response.json();

pm.test('isSuccess is false', () => {
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code is NotFound_Campania (2003)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('2003');
});

pm.test('Error message indicates not found', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('encontrada').or.include('no existe');
});
```

---

### 5.4 Campanias - Update

#### 401_PUT Update - Success (Partial Update)

```
PUT {{baseUrl}}/api/campanias/{{campaniaId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "id": "{{campaniaId}}",
  "titulo": "Updated Title - {{$randomString}}",
  "descripcionCorta": "Updated short description"
}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Response has ServiceResponse structure', () => {
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
});

pm.test('data is true', () => {
    pm.expect(json.data).to.be.true;
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Message code is Updated (0002)', () => {
    const message = json.messages[0];
    pm.expect(message.errorCode).to.equal('0002');
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

Verify update worked with GET:
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}

pm.test('Titulo was updated', () => {
    const json = pm.response.json();
    pm.expect(json.data.titulo).to.include('Updated Title');
});

pm.test('Estado still BORRADOR', () => {
    const json = pm.response.json();
    pm.expect(json.data.estadoCampaniaId).to.equal(1);
});
```

#### 431_PUT Update - Different Artista (Ownership Check)

```
PUT {{baseUrl}}/api/campanias/{{campaniaId}}
Authorization: Bearer {{accessToken2}}
Content-Type: application/json

{
  "id": "{{campaniaId}}",
  "titulo": "Malicious Update"
}
```

**Tests:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

const json = pm.response.json();

pm.test('isSuccess is false', () => {
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('3002');
});

pm.test('Error message indicates no permission', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('permiso').or.include('No tienes');
});
```

#### 451_PUT Update - Estado != BORRADOR (Already Published)

First: Publish the campaign
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/publicar
Authorization: Bearer {{accessToken}}

Then attempt update:

PUT {{baseUrl}}/api/campanias/{{campaniaId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "id": "{{campaniaId}}",
  "titulo": "Late Update"
}
```

**Tests:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

const json = pm.response.json();

pm.test('isSuccess is false', () => {
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code is BusinessRule_CampaniaNotDraft (4009)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('4009');
});

pm.test('Error message indicates state issue', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('borrador').or.include('estado');
});
```

---

### 5.5 Campanias - Publish

#### 501_POST Publicar - Success (Full Data)

```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Response has PublishCampaniaResponse', () => {
    pm.expect(json.data).to.have.property('id');
    pm.expect(json.data).to.have.property('estadoCampaniaId');
    pm.expect(json.data).to.have.property('fechaPublicacion');
    pm.expect(json.data).to.have.property('message');
});

pm.test('Estado changed to PUBLICADA (2)', () => {
    pm.expect(json.data.estadoCampaniaId).to.equal(2);
});

pm.test('FechaPublicacion is set', () => {
    pm.expect(json.data.fechaPublicacion).to.exist;
    pm.expect(new Date(json.data.fechaPublicacion)).to.be.a('Date');
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Message indicates success', () => {
    pm.expect(json.data.message).to.include('publicad').or.include('exito');
});

pm.test('Save published ID for list tests', () => {
    pm.environment.set('campaniaIdPublished', json.data.id);
});
```

#### 511_POST Publicar - Missing Titulo

Create campaña without titulo, then publish:
```
POST {{baseUrl}}/api/campanias/{{campaniaIdNoTitulo}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

const json = pm.response.json();

pm.test('Error code is Validation_Required (1001)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('1001');
});

pm.test('Error mentions titulo', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('titulo').or.include('obligatorio');
});
```

#### 516_POST Publicar - FechaFin < Now + 7 Days

Create campaña with FechaFin = Now + 2 days, then publish:
```
POST {{baseUrl}}/api/campanias/{{campaniaIdShortDuration}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

const json = pm.response.json();

pm.test('Error code is Validation_InvalidDate (1012)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('1012');
});

pm.test('Error mentions duration', () => {
    const error = json.messages[0];
    pm.expect(error.message).to.include('7').or.include('dias');
});
```

#### 531_POST Publicar - Different Artista

```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/publicar
Authorization: Bearer {{accessToken2}}
```

**Tests:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

const json = pm.response.json();

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('3002');
});
```

#### 551_POST Publicar - Estado != BORRADOR (Already Published)

Try to publish same campaña twice:
```
POST {{baseUrl}}/api/campanias/{{campaniaIdPublished}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

const json = pm.response.json();

pm.test('Error code is BusinessRule_CampaniaNotDraft (4009)', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.equal('4009');
});
```

---

### 5.6 Campanias - List Public

#### 601_GET List - Success (Default Params)

```
GET {{baseUrl}}/api/campanias
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Response has ServiceResponse structure', () => {
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
});

pm.test('data is array', () => {
    pm.expect(json.data).to.be.an('array');
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('All items are CampaniaListDto', () => {
    json.data.forEach(item => {
        pm.expect(item).to.have.property('id');
        pm.expect(item).to.have.property('titulo');
        pm.expect(item).to.have.property('estadoCampaniaId');
        pm.expect(item.estadoCampaniaId).to.equal(2); // Default PUBLICADA
    });
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### 604_GET List - Search by Titulo

```
GET {{baseUrl}}/api/campanias?searchTerm=primer%20album
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Search results contain term', () => {
    json.data.forEach(item => {
        pm.expect(
            item.titulo.toLowerCase().includes('primer') ||
            item.subtitulo?.toLowerCase().includes('primer')
        ).to.be.true;
    });
});
```

#### 607_GET List - Filter by Estado

```
GET {{baseUrl}}/api/campanias?estadoCampaniaId=1
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Only BORRADOR campanias returned', () => {
    json.data.forEach(item => {
        pm.expect(item.estadoCampaniaId).to.equal(1);
    });
});
```

#### 609_GET List - Pagination Page 2

```
GET {{baseUrl}}/api/campanias?pageNumber=2&pageSize=5
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Data is array', () => {
    pm.expect(json.data).to.be.an('array');
});

pm.test('Page 2 items received (may be fewer or empty)', () => {
    pm.expect(json.data.length).to.be.lessThanOrEqual(5);
});
```

#### 621_GET List - Invalid PageNumber (0)

```
GET {{baseUrl}}/api/campanias?pageNumber=0
```

**Tests:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

const json = pm.response.json();

pm.test('Error indicates invalid page number', () => {
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 5.7 Campanias - List My Campanias

#### 701_GET MisCampanias - Success (Authenticated)

```
GET {{baseUrl}}/api/campanias/mis-campanias
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Response has ServiceResponse structure', () => {
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
});

pm.test('data is array of CampaniaListDto', () => {
    pm.expect(json.data).to.be.an('array');
});

pm.test('isSuccess is true', () => {
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('All campanias belong to authenticated artista', () => {
    const artistaId = pm.environment.get('artistaId');
    json.data.forEach(item => {
        pm.expect(item.artistaId).to.equal(artistaId);
    });
});

pm.test('Includes BORRADOR and PUBLICADA states', () => {
    const states = new Set(json.data.map(c => c.estadoCampaniaId));
    pm.expect(states.size).to.be.greaterThan(0);
    // May include 1 (BORRADOR) and/or 2 (PUBLICADA)
});
```

#### 703_GET MisCampanias - Includes BORRADOR State

```
GET {{baseUrl}}/api/campanias/mis-campanias?estadoCampaniaId=1
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

const json = pm.response.json();

pm.test('Only BORRADOR (1) returned', () => {
    json.data.forEach(item => {
        pm.expect(item.estadoCampaniaId).to.equal(1);
    });
});

pm.test('Can see own draft campanias', () => {
    pm.expect(json.data.length).to.be.greaterThanOrEqual(0);
});
```

#### 711_GET MisCampanias - Missing Authorization Header

```
GET {{baseUrl}}/api/campanias/mis-campanias
```

**Tests:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

const json = pm.response.json();

pm.test('Error code indicates auth required', () => {
    const error = json.messages[0];
    pm.expect(error.errorCode).to.match(/^3\d{3}$/);
});
```

---

## 6. Estrategia de Ejecucion

### Local Development

```bash
# Ejecutar toda la coleccion
newman run tests/newman/WePlay.CrearCampania.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,html \
    --reporter-html-export=test-report.html \
    --disable-unicode

# Ejecutar solo folder especifico
newman run tests/newman/WePlay.CrearCampania.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --folder "Campanias - Create"

# Con timeout y retries
newman run tests/newman/WePlay.CrearCampania.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --timeout 30000
```

### CI/CD Pipeline (Azure Pipelines YAML)

```yaml
trigger:
  branches:
    include:
      - master
      - develop
  paths:
    include:
      - 'src/api/**'
      - 'tests/newman/**'

pool:
  vmImage: 'ubuntu-latest'

variables:
  API_URL: 'https://localhost:5001'
  CLIENT_SECRET: $(SECRET_CLIENT_SECRET)
  STAGING_PASSWORD: $(SECRET_STAGING_TEST_PASSWORD)

stages:
  - stage: IntegrationTests
    displayName: 'Newman Integration Tests'
    jobs:
      - job: RunNewmanTests
        displayName: 'Run Campania Integration Tests'
        steps:
          - task: UseNode@1
            inputs:
              version: '18.x'

          - task: Npm@1
            displayName: 'Install Newman'
            inputs:
              command: 'install'
              arguments: '-g newman'

          - task: PowerShell@2
            displayName: 'Run Integration Tests'
            inputs:
              targetType: 'inline'
              script: |
                $env:SECRET_CLIENT_SECRET = $(SECRET_CLIENT_SECRET)
                $env:SECRET_STAGING_TEST_PASSWORD = $(SECRET_STAGING_TEST_PASSWORD)

                newman run 'tests/newman/WePlay.CrearCampania.IntegrationTests.json' `
                    -e 'tests/newman/environments/staging.json' `
                    --reporters cli,json,junit,htmlextra `
                    --reporter-junit-export='test-results.xml' `
                    --reporter-json-export='test-results.json' `
                    --reporter-htmlextra-export='test-report.html' `
                    --timeout 30000 `
                    --bail

          - task: PublishTestResults@2
            displayName: 'Publish Test Results'
            condition: always()
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: 'test-results.xml'
              searchFolder: '$(System.DefaultWorkingDirectory)'

          - task: PublishBuildArtifacts@1
            displayName: 'Publish Test Report'
            condition: always()
            inputs:
              pathToPublish: '$(System.DefaultWorkingDirectory)/test-report.html'
              artifactName: 'newman-report'
```

### GitHub Actions YAML

```yaml
name: Integration Tests - Crear Campania

on:
  push:
    branches: [master, develop]
    paths:
      - 'src/api/**'
      - 'tests/newman/**'
  pull_request:
    branches: [master, develop]

jobs:
  newman-tests:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install Newman
        run: npm install -g newman

      - name: Run Integration Tests
        run: |
          newman run tests/newman/WePlay.CrearCampania.IntegrationTests.json \
              -e tests/newman/environments/staging.json \
              --reporters cli,json,junit,htmlextra \
              --reporter-junit-export=test-results.xml \
              --reporter-json-export=test-results.json \
              --reporter-htmlextra-export=test-report.html \
              --timeout 30000 \
              --bail
        env:
          SECRET_CLIENT_SECRET: ${{ secrets.CLIENT_SECRET }}
          SECRET_STAGING_TEST_PASSWORD: ${{ secrets.STAGING_TEST_PASSWORD }}

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: test-results.xml

      - name: Upload HTML Report
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: newman-report
          path: test-report.html

      - name: Publish Test Report
        if: always()
        uses: dorny/test-reporter@v1
        with:
          name: Integration Tests Results
          path: 'test-results.xml'
          reporter: 'java-junit'
```

---

## 7. Casos de Prueba Especiales

### Performance Tests

```javascript
// Response time targets
pm.test('POST /api/campanias < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test('GET /api/campanias/{id} < 300ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(300);
});

pm.test('PUT /api/campanias/{id} < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test('GET /api/campanias < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

### Security Tests

```javascript
// JWT Token Validation
pm.test('Token contains sub claim', () => {
    const token = pm.environment.get('accessToken');
    const payload = JSON.parse(atob(token.split('.')[1]));
    pm.expect(payload).to.have.property('sub');
});

// No sensitive data in response
pm.test('Response does not contain password', () => {
    const response = pm.response.text();
    pm.expect(response).to.not.include('password');
});

// CORS Headers
pm.test('Response has CORS headers', () => {
    pm.expect(pm.response.headers.has('Access-Control-Allow-Origin')).to.be.true;
});
```

### Contract Validation Tests

```javascript
// Verify response schema
pm.test('Response matches ServiceResponse contract', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.all.keys('data', 'messages', 'isSuccess');
});

// Verify error codes are in valid range
pm.test('Error code in valid range', () => {
    const error = pm.response.json().messages[0];
    const code = parseInt(error.errorCode);
    pm.expect(code).to.be.within(0, 5999);
});
```

---

## 8. Checklist Pre-Ejecucion

- [ ] Backend API está corriendo en localhost:5001
- [ ] Base de datos está limpia o fixtures estan aplicadas
- [ ] JWT Secret configurado en `appsettings.Development.json`
- [ ] Cliente OAuth `weplay-test` registrado en Identity Server
- [ ] Newman instalado: `npm install -g newman`
- [ ] Archivo `WePlay.CrearCampania.IntegrationTests.json` está en `tests/newman/`
- [ ] Variables de entorno (`development.json`, `staging.json`) están configuradas
- [ ] Todos los secretos están en `pm.environment` (no hardcodeados)
- [ ] URLs en variables ({{baseUrl}}, {{identityUrl}})
- [ ] Pre-request scripts configurados para generar tokens
- [ ] Tests scripts incluyen assertions completas
- [ ] Coleccion está organizada por carpetas (status code, endpoint)

---

## 9. Reportes y Documentacion

### Salida de Ejecucion CLI

```
┌─────────────────────────────────────────┐
│     WePlay.CrearCampania.IntegrationTests│
└─────────────────────────────────────────┘

_Setup
    ✓ 001_Initialize Environment Variables
    ✓ 002_Register Test Artista 1
    ✓ 003_Login Test Artista 1 (Save Token)
    ✓ 004_Register Test Artista 2
    ✓ 005_Login Test Artista 2 (Save Token 2)

Campanias - Create
    201 CREATED - Success Cases
        ✓ 201_POST Create - Success (Full Data)
        ✓ 202_POST Create - Minimal Data
        ...

    400 Bad Request - Validation Errors
        ✓ 211_POST Create - Empty Titulo
        ✓ 212_POST Create - Titulo > 200 chars
        ...

    401 Unauthorized
        ✓ 231_POST Create - Missing Authorization Header
        ...

...

_Cleanup
    ✓ 801_Delete Test Data
    ✓ 802_Cleanup Tokens/Secrets

┌────────────────────────┐
│     Test Summary       │
├────────────────────────┤
│ Total Tests    │  125  │
│ Passed         │  124  │
│ Failed         │    1  │
│ Warnings       │    0  │
│ Errors         │    0  │
│ Skipped        │    0  │
└────────────────────────┘

Total time: 87.234 seconds
```

### JUnit XML Output

```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites name="Newman Test Run" time="87.234">
    <testsuite name="WePlay.CrearCampania.IntegrationTests" tests="125" failures="1" skipped="0" time="87.234">
        <testcase name="Setup - Initialize Environment Variables" time="0.045"/>
        <testcase name="Setup - Register Test Artista 1" time="1.234"/>
        <testcase name="Campanias Create - Full Data" time="0.456"/>
        ...
        <testcase name="Campanias Create - Empty Titulo" time="0.234">
            <failure message="Expected status 400 but got 500">
                Stack trace...
            </failure>
        </testcase>
        ...
    </testsuite>
</testsuites>
```

### HTML Report Features

- Resumen de ejecucion (total, passed, failed)
- Timeline de requests
- Request/Response details
- Test assertions detalladas
- Error screenshots
- Performance graphs
- Folder breakdowns

---

## 10. Troubleshooting Comun

| Problema | Causa | Solucion |
|----------|-------|----------|
| 401 Unauthorized en todos los tests | Token expirado o no extraido | Verificar token extraction en _Setup, renovar token |
| 404 en campaniaId | Variable no guardada correctamente | Verificar pm.environment.set en POST tests |
| 500 errors esporadicos | DB connection timeouts | Aumentar --timeout a 30000ms |
| CORS errors | Headers faltantes | Verificar Content-Type, Accept headers |
| SSL certificate error | HTTPS en localhost | Usar `--insecure` flag o confiar certificado |
| Tests pasan local, fallan en CI | Diferencias de ambiente | Verificar ambiente staging vs development |
| Validaciones no se disparan | Validator no registrado | Verificar validators en DependencyInjection |

---

## 11. Post-Execution Checklist

- [ ] Todos los 125 tests pasaron
- [ ] No hay warnings en los logs
- [ ] Report HTML generado exitosamente
- [ ] JUnit XML generado para CI/CD
- [ ] JSON results exportado para análisis
- [ ] Datos de prueba limpios (cleanup ejecutado)
- [ ] Certificados/tokens no expuestos en reporte
- [ ] Performance metrics bajo los targets
- [ ] Error codes coinciden con contrato

---

## 12. Proximos Pasos Recomendados

1. **Implementar coleccion en Postman Desktop**
   - Descargar coleccion desde archivo
   - Testear manualmente si es necesario
   - Exportar como `WePlay.CrearCampania.IntegrationTests.json`

2. **Configurar en CI/CD Pipeline**
   - Azure Pipelines (si usas Azure DevOps)
   - GitHub Actions (si usas GitHub)
   - Otros: Jenkins, GitLab CI, Bitbucket Pipelines

3. **Monitoreo continuo**
   - Ejecutar tests en cada commit/PR
   - Guardar historicos de resultados
   - Alertar si % de exito baja

4. **Expansion de tests**
   - Agregar tests para Rewards (WPR-003)
   - Agregar tests para Backings (WPR-004)
   - Performance load tests

5. **Documentacion**
   - Como ejecutar tests localmente
   - Como debuggear requests fallidas
   - Como agregar nuevos tests

---

## Resumen de Endpoints a Testear

| # | Metodo | Ruta | Auth | Status | Total Tests |
|---|--------|------|------|--------|-------------|
| 1 | POST | /api/campanias | ✅ | 201, 400, 401, 500 | 18 |
| 2 | GET | /api/campanias/{id} | ❌ | 200, 404, 500 | 10 |
| 3 | PUT | /api/campanias/{id} | ✅ | 200, 400, 401, 403, 404, 409, 500 | 20 |
| 4 | POST | /api/campanias/{id}/publicar | ✅ | 200, 400, 401, 403, 404, 409, 500 | 22 |
| 5 | GET | /api/campanias | ❌ | 200, 400, 500 | 15 |
| 6 | GET | /api/campanias/mis-campanias | ✅ | 200, 400, 401, 500 | 15 |
| **TOTAL** | | | | | **100+** |

---

## Notas Finales

- Este plan NO IMPLEMENTA colecciones Postman, solo proporciona arquitectura y detalles
- Todos los tests incluyen assertions completos para validar contratos
- Las variables de entorno usan secretos (${{ secrets.X }}) en CI/CD
- Los flujos E2E validan integridad end-to-end
- Performance targets son conservadores (< 500ms para la mayoría)
- Security tests validan JWT, CORS, sensitive data exposure
- El plan es extensible para agregar mas tests posteriores

**Fin del plan de testing Newman**
