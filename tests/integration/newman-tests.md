# Plan de Testing Newman: Campanias API

**Fecha:** 2026-02-12
**Feature:** crear-campania (WPR-007)
**Coleccion:** WePlay.Campanias.IntegrationTests
**Base URL:** http://localhost:5000/api
**Auth Type:** Bearer JWT

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 6 |
| Total requests | 28 |
| Casos de prueba | 52 |
| Test suites | 6 |
| Environments | 3 (dev, staging, prod) |

**Cobertura:**
- POST /api/campanias (CREATE)
- GET /api/campanias/{id} (READ detail)
- PUT /api/campanias/{id} (UPDATE)
- POST /api/campanias/{id}/publicar (PUBLISH)
- GET /api/campanias (LIST public)
- GET /api/campanias/mis-campanias (LIST user's)

---

## 2. Estructura de Coleccion

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
│   │   ├── POST Create Minimal (only required)
│   │   ├── POST Create with URLs
│   │   └── POST Create Save ID to Environment
│   │
│   ├── 200 OK - GET Detail/
│   │   ├── GET Existing Campaign
│   │   └── GET Campaign Save Data to Environment
│   │
│   ├── 200 OK - UPDATE/
│   │   ├── PUT Update Single Field
│   │   ├── PUT Update Multiple Fields
│   │   ├── PUT Verify Changes Persisted
│   │   └── PUT Change All Fields
│   │
│   ├── 200 OK - PUBLISH/
│   │   ├── POST Publish Valid Draft
│   │   ├── POST Verify State Changed to PUBLICADA
│   │   └── POST Verify FechaPublicacion Set
│   │
│   ├── 200 OK - LIST PUBLIC/
│   │   ├── GET All Campaigns Paginated
│   │   ├── GET Filter by SearchTerm
│   │   ├── GET Filter by ArtistaId
│   │   ├── GET Filter by EstadoCampaniaId
│   │   └── GET Pagination Parameters
│   │
│   ├── 200 OK - LIST MY CAMPAIGNS/
│   │   ├── GET My Campaigns Authenticated
│   │   ├── GET Filter My Campaigns by State
│   │   ├── GET My Campaigns Pagination
│   │   └── GET My Campaigns Includes Drafts
│   │
│   ├── 400 BAD REQUEST/
│   │   ├── POST Create Empty Titulo
│   │   ├── POST Create Titulo Too Long
│   │   ├── POST Create Invalid ImporteObjetivo
│   │   ├── POST Create ImporteMinimo > ImporteObjetivo
│   │   ├── POST Create Invalid URL Format
│   │   ├── POST Create FechaFin < FechaInicio
│   │   ├── PUT Update Titulo Too Long
│   │   ├── PUT Update Invalid ImporteObjetivo
│   │   ├── POST Publish Missing Titulo
│   │   ├── POST Publish Missing ImporteObjetivo
│   │   └── POST Publish FechaFin < 7 days
│   │
│   ├── 401 UNAUTHORIZED/
│   │   ├── POST Create Without Token
│   │   ├── POST Create with Expired Token
│   │   ├── PUT Update Without Token
│   │   ├── POST Publish Without Token
│   │   └── GET My Campaigns Without Token
│   │
│   ├── 403 FORBIDDEN/
│   │   ├── PUT Update Campaign Not Owner
│   │   ├── POST Publish Campaign Not Owner
│   │   └── PUT Update with Different User Token
│   │
│   ├── 404 NOT FOUND/
│   │   ├── GET Non-existent Campaign
│   │   ├── PUT Non-existent Campaign
│   │   └── POST Publish Non-existent Campaign
│   │
│   ├── 409 CONFLICT/
│   │   ├── PUT Update Published Campaign
│   │   ├── POST Publish Already Published Campaign
│   │   ├── PUT Update cancelled Campaign
│   │   └── POST Publish non-draft Campaign
│   │
│   └── E2E Happy Path/
│       ├── Create Draft Campaign
│       ├── Update Draft Campaign
│       ├── Verify Updates with GET
│       ├── Get My Campaigns List
│       ├── Publish Campaign
│       ├── Verify State is PUBLICADA
│       ├── Try Edit Published (expect 409)
│       ├── Try Publish Again (expect 409)
│       └── Get from Public List
│
└── _Cleanup/
    ├── Delete Test Campaign (if cascade delete exists)
    ├── Delete Test Artista Profile
    └── Delete Test User Account
```

---

## 3. Setup & Teardown

### 3.1 Setup - Register and Authenticate

**Request 1: POST Register New Test User**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "test-{{$timestamp}}@weplay.com",
    "password": "TestPassword123!",
    "confirmPassword": "TestPassword123!",
    "firstName": "Test",
    "lastName": "User"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Has user ID', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.environment.set('testUserId', json.data.id);
});

pm.test('Response time < 1000ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});
```

**Request 2: POST Login & Get JWT Token**
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
    "email": "{{testUserEmail}}",
    "password": "TestPassword123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Token obtained and saved', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.token).to.exist;
    pm.environment.set('accessToken', json.data.token);
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});
```

**Request 3: POST Create Artista Profile**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "nombreArtistico": "Test Artist {{$timestamp}}",
    "descripcion": "Test artist for integration testing"
}
```

**Test Script:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

pm.test('Artista created with ID', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.id).to.exist;
    pm.environment.set('artistaId', json.data.id);
});

pm.test('ServiceResponse structure correct', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});
```

---

## 4. Requests Detallados

### 4.1 POST /api/campanias - Create Campaign

#### 4.1.1 201 CREATED - Full Request with All Fields

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "My First Album {{$timestamp}}",
    "subtitulo": "Rock alternativo con influencias indie",
    "descripcionCorta": "Necesitamos tu apoyo para grabar nuestro primer disco. Queremos contar tu historia.",
    "videoPrincipalUrl": "https://youtube.com/watch?v=xyz123",
    "imagenPrincipalUrl": "https://example.com/images/album.jpg",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "importeMinimo": 100.00,
    "tipoFinanciacionId": 1,
    "permiteAportacionesAnonimas": false,
    "permitePropinas": true,
    "fechaInicio": "2026-03-01T00:00:00Z",
    "fechaFin": "2026-04-30T23:59:59Z"
}
```

**Tests:**
```javascript
pm.test('Status code is 201', () => pm.response.to.have.status(201));

pm.test('Response time < 500ms', () =>
    pm.expect(pm.response.responseTime).to.be.below(500)
);

pm.test('ServiceResponse.isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data contains complete CampaniaDto', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('id');
    pm.expect(data).to.have.property('artistaId');
    pm.expect(data).to.have.property('titulo');
    pm.expect(data).to.have.property('estadoCampaniaId');
    pm.expect(data.titulo).to.include('My First Album');
});

pm.test('Estado is BORRADOR (1)', () => {
    const data = pm.response.json().data;
    pm.expect(data.estadoCampaniaId).to.equal(1);
});

pm.test('FechaCreacion is set', () => {
    const data = pm.response.json().data;
    pm.expect(data.fechaCreacion).to.exist;
});

pm.test('ImportePledgedActual is 0', () => {
    const data = pm.response.json().data;
    pm.expect(data.importePledgedActual).to.equal(0);
});

pm.test('Headers contain Location header', () => {
    pm.expect(pm.response.headers.get('Location')).to.exist;
});

// Save campaign ID to environment for subsequent tests
pm.test('Save campaign ID to environment', () => {
    const data = pm.response.json().data;
    pm.environment.set('campaignId', data.id);
});
```

#### 4.1.2 201 CREATED - Minimal Request (Only Required Fields)

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Minimal Campaign {{$timestamp}}",
    "monedaId": 1,
    "importeObjetivo": 1000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 201', () => pm.response.to.have.status(201));

pm.test('Campaign created with defaults', () => {
    const data = pm.response.json().data;
    pm.expect(data.subtitulo).to.be.null;
    pm.expect(data.descripcionCorta).to.be.null;
    pm.expect(data.importeMinimo).to.be.null;
    pm.expect(data.permiteAportacionesAnonimas).to.be.false;
    pm.expect(data.permitePropinas).to.be.false;
});

pm.test('ID is valid GUID', () => {
    const data = pm.response.json().data;
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(data.id).to.match(guidRegex);
});
```

#### 4.1.3 400 BAD REQUEST - Empty Titulo

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Has validation messages', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
});

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});

pm.test('Error message mentions titulo', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message.toLowerCase()).to.include('titulo');
});
```

#### 4.1.4 400 BAD REQUEST - Titulo Too Long (>200 chars)

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "This is a very long title that exceeds the maximum length of two hundred characters allowed by the system for campaign titles because we want to test the validation rules are working correctly and rejecting input that is too long...",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_MaxLength (1002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1002');
});

pm.test('Error message mentions 200 characters', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message.toLowerCase()).to.include('200');
});
```

#### 4.1.5 400 BAD REQUEST - ImporteObjetivo <= 0

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Invalid Amount Campaign",
    "monedaId": 1,
    "importeObjetivo": 0,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_InvalidAmount (1011)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1011');
});
```

#### 4.1.6 400 BAD REQUEST - ImporteMinimo > ImporteObjetivo

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Invalid Range Campaign",
    "monedaId": 1,
    "importeObjetivo": 1000.00,
    "importeMinimo": 2000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_InvalidRange (1007)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1007');
});
```

#### 4.1.7 400 BAD REQUEST - Invalid URL Format

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Invalid URL Campaign",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1,
    "videoPrincipalUrl": "not-a-valid-url",
    "imagenPrincipalUrl": "also-invalid"
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_InvalidUrl (1006)', () => {
    const json = pm.response.json();
    pm.expect(json.messages.some(m => m.errorCode === '1006')).to.be.true;
});
```

#### 4.1.8 400 BAD REQUEST - FechaFin < FechaInicio

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Invalid Dates Campaign",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1,
    "fechaInicio": "2026-04-30T00:00:00Z",
    "fechaFin": "2026-03-01T00:00:00Z"
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_InvalidDate (1012)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1012');
});
```

#### 4.1.9 401 UNAUTHORIZED - No Token

**Request:**
```
POST {{baseUrl}}/api/campanias
Content-Type: application/json

{
    "titulo": "No Auth Campaign",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));

pm.test('Response indicates authentication required', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages.some(m => m.errorCode.startsWith('3'))).to.be.true;
});
```

#### 4.1.10 401 UNAUTHORIZED - Invalid/Expired Token

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer invalid_token_here
Content-Type: application/json

{
    "titulo": "Invalid Token Campaign",
    "monedaId": 1,
    "importeObjetivo": 5000.00,
    "tipoFinanciacionId": 1
}
```

**Tests:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));
```

---

### 4.2 GET /api/campanias/{id} - Get Campaign Detail

#### 4.2.1 200 OK - Get Existing Campaign

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaignId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data contains campaign details', () => {
    const data = pm.response.json().data;
    pm.expect(data.id).to.equal(pm.environment.get('campaignId'));
    pm.expect(data.titulo).to.exist;
    pm.expect(data.estadoCampaniaId).to.exist;
});

pm.test('Response time < 300ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(300);
});

pm.test('Campaign status is BORRADOR (1)', () => {
    const data = pm.response.json().data;
    pm.expect(data.estadoCampaniaId).to.equal(1);
});
```

#### 4.2.2 404 NOT FOUND - Non-existent Campaign

**Request:**
```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000
```

**Tests:**
```javascript
pm.test('Status code is 404', () => pm.response.to.have.status(404));

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code is NotFound_Campania (2003)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2003');
});
```

---

### 4.3 PUT /api/campanias/{id} - Update Campaign

#### 4.3.1 200 OK - Update Single Field

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{campaignId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{campaignId}}",
    "titulo": "Updated Title {{$timestamp}}"
}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Response data is true (update successful)', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.true;
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### 4.3.2 200 OK - Verify Updates Persisted (GET After Update)

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaignId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('Titulo was updated', () => {
    const data = pm.response.json().data;
    pm.expect(data.titulo).to.include('Updated Title');
});
```

#### 4.3.3 200 OK - Update Multiple Fields

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{campaignId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{campaignId}}",
    "titulo": "New Title {{$timestamp}}",
    "subtitulo": "New Subtitle for updated campaign",
    "importeObjetivo": 7500.00
}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('Update response indicates success', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});
```

#### 4.3.4 400 BAD REQUEST - Titulo Too Long

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{campaignId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{campaignId}}",
    "titulo": "This is an extremely long title that exceeds the maximum allowed length of 200 characters which should trigger a validation error in the system to ensure that titles remain concise and user-friendly for display in UI components..."
}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_MaxLength (1002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1002');
});
```

#### 4.3.5 401 UNAUTHORIZED - Update Without Token

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{campaignId}}
Content-Type: application/json

{
    "id": "{{campaignId}}",
    "titulo": "No Auth Update"
}
```

**Tests:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));
```

#### 4.3.6 403 FORBIDDEN - Update Campaign You Don't Own

**Pre-request Script:** (Setup second user/token first)
```javascript
// This assumes second user token is in {{secondUserToken}}
```

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{campaignId}}
Authorization: Bearer {{secondUserToken}}
Content-Type: application/json

{
    "id": "{{campaignId}}",
    "titulo": "Unauthorized Update"
}
```

**Tests:**
```javascript
pm.test('Status code is 403', () => pm.response.to.have.status(403));

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

#### 4.3.7 404 NOT FOUND - Update Non-existent Campaign

**Request:**
```
PUT {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "00000000-0000-0000-0000-000000000000",
    "titulo": "Update Nonexistent"
}
```

**Tests:**
```javascript
pm.test('Status code is 404', () => pm.response.to.have.status(404));

pm.test('Error code is NotFound_Campania (2003)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2003');
});
```

#### 4.3.8 409 CONFLICT - Update Published Campaign

**Pre-request Script:**
```javascript
// First publish the campaign in this test suite
```

**Request:**
```
PUT {{baseUrl}}/api/campanias/{{publishedCampaignId}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{publishedCampaignId}}",
    "titulo": "Update Published Campaign"
}
```

**Tests:**
```javascript
pm.test('Status code is 409', () => pm.response.to.have.status(409));

pm.test('Error code is BusinessRule_CampaniaNotDraft (4009)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('4009');
});

pm.test('Error message mentions draft state', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message.toLowerCase()).to.include('borrador');
});
```

---

### 4.4 POST /api/campanias/{id}/publicar - Publish Campaign

#### 4.4.1 200 OK - Publish Valid Draft

**Request:**
```
POST {{baseUrl}}/api/campanias/{{draftCampaignId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Response contains PublishCampaniaResponse', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('id');
    pm.expect(data).to.have.property('estadoCampaniaId');
    pm.expect(data).to.have.property('fechaPublicacion');
    pm.expect(data).to.have.property('message');
});

pm.test('Estado changed to PUBLICADA (2)', () => {
    const data = pm.response.json().data;
    pm.expect(data.estadoCampaniaId).to.equal(2);
});

pm.test('FechaPublicacion is set', () => {
    const data = pm.response.json().data;
    pm.expect(data.fechaPublicacion).to.exist;
    pm.environment.set('publishedCampaignId', data.id);
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### 4.4.2 200 OK - Verify State Changed via GET

**Request:**
```
GET {{baseUrl}}/api/campanias/{{publishedCampaignId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('Estado is PUBLICADA (2)', () => {
    const data = pm.response.json().data;
    pm.expect(data.estadoCampaniaId).to.equal(2);
});

pm.test('FechaPublicacion is set', () => {
    const data = pm.response.json().data;
    pm.expect(data.fechaPublicacion).to.exist;
});
```

#### 4.4.3 400 BAD REQUEST - Publish with Missing Titulo

**Pre-request Script:**
```javascript
// Create a campaign with no titulo first
```

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaignNoTituloId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});

pm.test('Error mentions titulo', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message.toLowerCase()).to.include('titulo');
});
```

#### 4.4.4 400 BAD REQUEST - Publish with FechaFin < 7 days

**Pre-request Script:**
```javascript
// Create a campaign with FechaFin < 7 days from now
```

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaignShortDateId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));

pm.test('Error code is Validation_InvalidDate (1012)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1012');
});

pm.test('Error mentions 7 days', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message.toLowerCase()).to.include('7');
});
```

#### 4.4.5 401 UNAUTHORIZED - Publish Without Token

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaignId}}/publicar
```

**Tests:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));
```

#### 4.4.6 403 FORBIDDEN - Publish Campaign Not Owner

**Request:**
```
POST {{baseUrl}}/api/campanias/{{otherUserCampaignId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 403', () => pm.response.to.have.status(403));

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

#### 4.4.7 404 NOT FOUND - Publish Non-existent Campaign

**Request:**
```
POST {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 404', () => pm.response.to.have.status(404));

pm.test('Error code is NotFound_Campania (2003)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2003');
});
```

#### 4.4.8 409 CONFLICT - Publish Already Published Campaign

**Pre-request Script:**
```javascript
// This campaign was already published in 4.4.1
```

**Request:**
```
POST {{baseUrl}}/api/campanias/{{publishedCampaignId}}/publicar
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 409', () => pm.response.to.have.status(409));

pm.test('Error code is BusinessRule_CampaniaNotDraft (4009)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('4009');
});
```

---

### 4.5 GET /api/campanias - List Public Campaigns

#### 4.5.1 200 OK - Get All Campaigns (Default Pagination)

**Request:**
```
GET {{baseUrl}}/api/campanias
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data is array', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.an('array');
});

pm.test('Response time < 300ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(300);
});
```

#### 4.5.2 200 OK - Pagination with pageNumber and pageSize

**Request:**
```
GET {{baseUrl}}/api/campanias?pageNumber=1&pageSize=5
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('Returns maximum 5 items', () => {
    const json = pm.response.json();
    pm.expect(json.data.length).to.be.at.most(5);
});
```

#### 4.5.3 200 OK - Filter by SearchTerm

**Request:**
```
GET {{baseUrl}}/api/campanias?searchTerm={{searchTerm}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('All results match search term', () => {
    const data = pm.response.json().data;
    if (data.length > 0) {
        data.forEach(camp => {
            const haystack = (camp.titulo + camp.subtitulo + camp.descripcionCorta).toLowerCase();
            const needle = pm.environment.get('searchTerm').toLowerCase();
            pm.expect(haystack).to.include(needle);
        });
    }
});
```

#### 4.5.4 200 OK - Filter by ArtistaId

**Request:**
```
GET {{baseUrl}}/api/campanias?artistaId={{artistaId}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('All results belong to specified artista', () => {
    const data = pm.response.json().data;
    const expectedArtistaId = pm.environment.get('artistaId');
    data.forEach(camp => {
        pm.expect(camp.artistaId).to.equal(expectedArtistaId);
    });
});
```

#### 4.5.5 200 OK - Default Filter Only Published (estadoCampaniaId=2)

**Request:**
```
GET {{baseUrl}}/api/campanias
```

**Tests:**
```javascript
pm.test('All campaigns are published (status 2)', () => {
    const data = pm.response.json().data;
    data.forEach(camp => {
        pm.expect(camp.estadoCampaniaId).to.equal(2);
    });
});
```

---

### 4.6 GET /api/campanias/mis-campanias - List My Campaigns

#### 4.6.1 200 OK - Get My Campaigns Authenticated

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data is array', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.an('array');
});

pm.test('Response time < 300ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(300);
});
```

#### 4.6.2 200 OK - Includes Draft Campaigns

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('May include BORRADOR (1) campaigns', () => {
    const data = pm.response.json().data;
    // This is informational - we created drafts, so they should be here
    const hasDrafts = data.some(c => c.estadoCampaniaId === 1);
    pm.expect(hasDrafts || data.length === 0).to.be.true;
});
```

#### 4.6.3 200 OK - Filter by EstadoCampaniaId

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?estadoCampaniaId=1
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('All results are BORRADOR (1)', () => {
    const data = pm.response.json().data;
    data.forEach(camp => {
        pm.expect(camp.estadoCampaniaId).to.equal(1);
    });
});
```

#### 4.6.4 200 OK - Pagination

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?pageNumber=1&pageSize=5
Authorization: Bearer {{accessToken}}
```

**Tests:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));

pm.test('Returns maximum 5 items', () => {
    const data = pm.response.json().data;
    pm.expect(data.length).to.be.at.most(5);
});
```

#### 4.6.5 401 UNAUTHORIZED - Without Token

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias
```

**Tests:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));

pm.test('Requires authentication', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

## 5. Variables de Entorno

### 5.1 Development Environment

**File:** `environments/development.json`

```json
{
    "name": "WePlay Rises - Development",
    "values": [
        {
            "key": "baseUrl",
            "value": "http://localhost:5000/api",
            "enabled": true
        },
        {
            "key": "identityUrl",
            "value": "http://localhost:5000/api",
            "enabled": true
        },
        {
            "key": "clientId",
            "value": "weplay-test",
            "enabled": true
        },
        {
            "key": "testUsername",
            "value": "test@weplay.com",
            "enabled": true
        },
        {
            "key": "testPassword",
            "value": "{{SECRET_TEST_PASSWORD}}",
            "enabled": true
        },
        {
            "key": "accessToken",
            "value": "",
            "enabled": true
        },
        {
            "key": "campaignId",
            "value": "",
            "enabled": true
        },
        {
            "key": "publishedCampaignId",
            "value": "",
            "enabled": true
        },
        {
            "key": "artistaId",
            "value": "",
            "enabled": true
        },
        {
            "key": "testUserId",
            "value": "",
            "enabled": true
        },
        {
            "key": "testUserEmail",
            "value": "",
            "enabled": true
        },
        {
            "key": "secondUserToken",
            "value": "",
            "enabled": true
        }
    ]
}
```

### 5.2 Staging Environment

**File:** `environments/staging.json`

```json
{
    "name": "WePlay Rises - Staging",
    "values": [
        {
            "key": "baseUrl",
            "value": "https://api-staging.weplay.com/api",
            "enabled": true
        },
        {
            "key": "identityUrl",
            "value": "https://api-staging.weplay.com/api",
            "enabled": true
        },
        {
            "key": "clientId",
            "value": "weplay-test-staging",
            "enabled": true
        },
        {
            "key": "testPassword",
            "value": "{{SECRET_TEST_PASSWORD_STAGING}}",
            "enabled": true
        },
        {
            "key": "accessToken",
            "value": "",
            "enabled": true
        }
    ]
}
```

### 5.3 Production Environment (Read-Only Validation)

**File:** `environments/production.json`

```json
{
    "name": "WePlay Rises - Production (Read-Only)",
    "values": [
        {
            "key": "baseUrl",
            "value": "https://api.weplay.com/api",
            "enabled": true
        },
        {
            "key": "identityUrl",
            "value": "https://api.weplay.com/api",
            "enabled": true
        }
    ]
}
```

---

## 6. Ejecucion

### 6.1 Ejecucion Local (Manual)

```bash
# Instalar Newman si no esta instalado
npm install -g newman newman-reporter-htmlextra

# Ejecutar coleccion contra Development
newman run WePlay.Campanias.IntegrationTests.json \
    -e environments/development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests-results/development-report.html

# Ejecutar solo tests de CREATE
newman run WePlay.Campanias.IntegrationTests.json \
    -e environments/development.json \
    --folder "Campanias/201 CREATED" \
    --reporters cli,htmlextra

# Ejecutar E2E Happy Path
newman run WePlay.Campanias.IntegrationTests.json \
    -e environments/development.json \
    --folder "Campanias/E2E Happy Path" \
    --reporters cli
```

### 6.2 CI/CD Pipeline (Azure DevOps)

**File:** `.github/workflows/integration-tests.yml` (GitHub Actions)

```yaml
name: Integration Tests - Campanias API

on:
  push:
    branches: [master, develop]
  pull_request:
    branches: [master, develop]

jobs:
  integration-tests:
    runs-on: ubuntu-latest

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2019-latest
        env:
          SA_PASSWORD: TestPassword123!
          ACCEPT_EULA: Y
        options: >-
          --health-cmd "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P TestPassword123! -Q 'select 1'"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 1433:1433

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install Newman
        run: npm install -g newman newman-reporter-junitxml

      - name: Build Backend
        run: dotnet build src/api/WePlayRises.sln

      - name: Start API Server
        run: |
          dotnet run --project src/api/WebApi/WePlayRises.WebApi.csproj &
          sleep 10
        env:
          ASPNETCORE_ENVIRONMENT: Testing
          ASPNETCORE_URLS: http://localhost:5000

      - name: Run Integration Tests
        run: |
          newman run tests/integration/WePlay.Campanias.IntegrationTests.json \
            -e tests/integration/environments/development.json \
            -r junitxml \
            --reporter-junitxml-export test-results/campanias-results.xml

      - name: Publish Test Results
        if: always()
        uses: dorny/test-reporter@v1
        with:
          name: Integration Tests Report
          path: 'test-results/campanias-results.xml'
          reporter: 'java-junit'

      - name: Generate HTML Report
        if: always()
        run: |
          newman run tests/integration/WePlay.Campanias.IntegrationTests.json \
            -e tests/integration/environments/development.json \
            -r htmlextra \
            --reporter-htmlextra-export test-results/campanias-report.html

      - name: Upload Artifacts
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: integration-test-results
          path: test-results/
```

**File:** `azure-pipelines.yml` (Azure DevOps)

```yaml
trigger:
  - master
  - develop

pr:
  - master
  - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0.x'
      packageType: 'sdk'

  - task: UseNode@1
    inputs:
      version: '18.x'

  - task: Npm@1
    inputs:
      command: 'custom'
      customCommand: 'install -g newman newman-reporter-htmlextra'

  - task: DotNetCoreCLI@2
    inputs:
      command: 'build'
      arguments: '--configuration $(buildConfiguration)'

  - task: DotNetCoreCLI@2
    inputs:
      command: 'run'
      arguments: '--project src/api/WebApi/WePlayRises.WebApi.csproj --configuration $(buildConfiguration)'
    continueOnError: true
    displayName: 'Start API Server'

  - task: Bash@3
    inputs:
      targetType: 'inline'
      script: 'sleep 10'

  - task: Newman@0
    inputs:
      collection: 'tests/integration/WePlay.Campanias.IntegrationTests.json'
      environment: 'tests/integration/environments/development.json'
      reporters: 'cli,json,htmlextra'
      reporterJsonFile: '$(System.DefaultWorkingDirectory)/test-results/newman-results.json'
      reporterHtmlFile: '$(System.DefaultWorkingDirectory)/test-results/newman-report.html'
      continueOnError: false
      delayRequest: 100
    displayName: 'Run Newman Tests'

  - task: PublishTestResults@2
    condition: always()
    inputs:
      testResultsFormat: 'NUnit'
      testResultsFiles: 'test-results/*.xml'
      searchFolder: '$(System.DefaultWorkingDirectory)'
      publishRunAttachments: true
    displayName: 'Publish Test Results'

  - task: PublishBuildArtifacts@1
    condition: always()
    inputs:
      PathtoPublish: 'test-results'
      ArtifactName: 'newman-reports'
    displayName: 'Publish Newman Reports'
```

---

## 7. Casos de Prueba

### Happy Path - Complete E2E Flow

1. Setup: Register user + Login + Create artista profile
2. Create campaign with all fields (minimum 7 days validity)
3. Get campaign by ID (verify response structure)
4. Update campaign title
5. Get updated campaign (verify changes persisted)
6. Get my campaigns list (verify campaign appears)
7. Publish campaign (BORRADOR → PUBLICADA)
8. Get campaign again (verify estado changed)
9. Try to update published campaign (expect 409)
10. Try to publish again (expect 409)
11. Get campaign from public list (verify it appears)
12. Cleanup: Delete test data

### Validation Errors - Negative Cases

**Create Campaign:**
- Empty titulo -> 400
- Titulo > 200 chars -> 400
- ImporteObjetivo <= 0 -> 400
- ImporteMinimo > ImporteObjetivo -> 400
- Invalid URL format -> 400
- FechaFin < FechaInicio -> 400
- MonedaId <= 0 -> 400
- TipoFinanciacionId <= 0 -> 400

**Update Campaign:**
- Titulo > 200 chars -> 400
- ImporteObjetivo <= 0 -> 400
- ImporteMinimo > ImporteObjetivo -> 400
- Invalid URL format -> 400
- FechaFin < FechaInicio -> 400

**Publish Campaign:**
- Missing titulo -> 400
- Missing importeObjetivo -> 400
- Missing monedaId -> 400
- Missing tipoFinanciacionId -> 400
- FechaFin < 7 days from now -> 400

### Authorization Errors

- Create without token -> 401
- Create with invalid token -> 401
- Create with expired token -> 401
- Update without token -> 401
- Update other user's campaign -> 403
- Publish without token -> 401
- Publish other user's campaign -> 403
- Get my campaigns without token -> 401

### Not Found Errors

- GET non-existent campaign -> 404
- UPDATE non-existent campaign -> 404
- DELETE non-existent campaign -> 404
- PUBLISH non-existent campaign -> 404

### Business Rule Conflicts

- Update published campaign -> 409
- Publish already published campaign -> 409
- Update cancelled campaign -> 409
- Publish already finalizada campaign -> 409

---

## 8. Assertions Genericas por Status Code

### 200 OK
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));
pm.test('Response time < 500ms', () => pm.expect(pm.response.responseTime).to.be.below(500));
pm.test('isSuccess is true', () => pm.expect(pm.response.json().isSuccess).to.be.true);
pm.test('Has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});
```

### 201 CREATED
```javascript
pm.test('Status code is 201', () => pm.response.to.have.status(201));
pm.test('Location header present', () => pm.expect(pm.response.headers.get('Location')).to.exist);
pm.test('Has ID in data', () => pm.expect(pm.response.json().data.id).to.exist);
pm.test('Save ID for subsequent tests', () => {
    pm.environment.set('resourceId', pm.response.json().data.id);
});
```

### 400 BAD REQUEST
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Has error messages', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
});
pm.test('Error code starts with 1 (validation)', () => {
    const code = pm.response.json().messages[0].errorCode;
    pm.expect(code.charAt(0)).to.equal('1');
});
```

### 401 UNAUTHORIZED
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Error code starts with 3 (auth)', () => {
    const code = pm.response.json().messages[0].errorCode;
    pm.expect(code.charAt(0)).to.equal('3');
});
```

### 403 FORBIDDEN
```javascript
pm.test('Status code is 403', () => pm.response.to.have.status(403));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Error code is Auth_Forbidden (3002)', () => {
    pm.expect(pm.response.json().messages[0].errorCode).to.equal('3002');
});
```

### 404 NOT FOUND
```javascript
pm.test('Status code is 404', () => pm.response.to.have.status(404));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Error code is NotFound_Campania (2003)', () => {
    pm.expect(pm.response.json().messages[0].errorCode).to.equal('2003');
});
```

### 409 CONFLICT
```javascript
pm.test('Status code is 409', () => pm.response.to.have.status(409));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Error code is BusinessRule (4xxx)', () => {
    const code = pm.response.json().messages[0].errorCode;
    pm.expect(code.charAt(0)).to.equal('4');
});
```

---

## 9. Checklist de Implementacion

### Coleccion de Requests
- [ ] _Setup folder with 3 requests (Register, Login, Create Artista)
- [ ] All 28 requests implemented with correct HTTP methods
- [ ] All endpoints have proper Authorization headers set
- [ ] Request bodies match API contracts exactly
- [ ] All status code responses documented

### Tests y Assertions
- [ ] Setup requests save tokens to environment variables
- [ ] 200 OK responses validate isSuccess=true
- [ ] 201 Created responses save IDs to environment
- [ ] 400 Bad Request responses check errorCode
- [ ] 401 Unauthorized responses check auth errors
- [ ] 403 Forbidden responses check ownership
- [ ] 404 Not Found responses check NotFound_Campania
- [ ] 409 Conflict responses check BusinessRule_CampaniaNotDraft
- [ ] Response time assertions < 500ms for all requests
- [ ] ServiceResponse structure validated in every request
- [ ] Error messages contain helpful context

### Ambientes
- [ ] Development environment configured
- [ ] Staging environment configured
- [ ] Production environment configured (read-only)
- [ ] All variables properly set
- [ ] Sensitive values use {{SECRET_*}} placeholders

### CI/CD
- [ ] GitHub Actions workflow created
- [ ] Azure DevOps pipeline created
- [ ] Newman installed in CI environment
- [ ] Test results published
- [ ] HTML reports generated
- [ ] Failures block merge requests

### Documentacion
- [ ] README with setup instructions
- [ ] Error code reference document
- [ ] Test data management strategy
- [ ] Known issues and limitations documented

---

## 10. Error Code Reference

| Code | Nombre | HTTP Status | Descripcion |
|------|--------|-------------|-------------|
| 0000 | Success | 200 | Operacion exitosa |
| 0001 | Created | 201 | Recurso creado |
| 0002 | Updated | 200 | Recurso actualizado |
| 1001 | Validation_Required | 400 | Campo obligatorio faltante |
| 1002 | Validation_MaxLength | 400 | Excede longitud maxima |
| 1006 | Validation_InvalidUrl | 400 | URL con formato invalido |
| 1007 | Validation_InvalidRange | 400 | Rango invalido (min > max) |
| 1011 | Validation_InvalidAmount | 400 | Importe invalido (<= 0) |
| 1012 | Validation_InvalidDate | 400 | Fecha invalida o fuera de rango |
| 2003 | NotFound_Campania | 404 | Campania no encontrada |
| 3001 | Auth_Unauthorized | 401 | Token no valido/expirado |
| 3002 | Auth_Forbidden | 403 | No tienes permiso |
| 3004 | Auth_InvalidToken | 401 | Token con formato invalido |
| 4009 | BusinessRule_CampaniaNotDraft | 409 | Campania debe estar en BORRADOR |
| 5000 | Internal_UnexpectedError | 500 | Error inesperado |

---

## 11. Ejecucion en Detalle

### Flujo de Ejecucion Esperado

1. **Setup Phase** (Request 1-3)
   - Registra nuevo usuario de test
   - Autentica y obtiene JWT
   - Crea perfil de artista
   - Guarda tokens y IDs en ambiente

2. **Happy Path Phase** (Requests 4-11)
   - Crea campaña borrador completa
   - Actualiza campos de campaña
   - Verifica cambios con GET
   - Obtiene lista "mis campanias"
   - Publica campaña
   - Verifica estado cambio
   - Intenta editar (debe fallar 409)
   - Intenta publicar de nuevo (debe fallar 409)

3. **Validation Phase** (Requests 12-31)
   - Prueba cada validacion individual
   - Verifica codigos de error correctos
   - Comprueba mensajes de error

4. **Authorization Phase** (Requests 32-39)
   - Sin token -> 401
   - Con token expirado -> 401
   - Token de otro usuario -> 403

5. **Not Found Phase** (Requests 40-42)
   - GET/PUT/POST con IDs inexistentes

6. **Conflict Phase** (Requests 43-45)
   - Editar campaña publicada -> 409
   - Publicar campaña ya publicada -> 409

7. **Cleanup Phase** (Request X)
   - Limpia datos de test
   - Opcional si la BD soporta cascade delete

### Duracion Estimada

- Setup: 2-3 segundos
- Happy Path: 3-4 segundos
- Validation: 8-10 segundos
- Authorization: 4-5 segundos
- Not Found: 2-3 segundos
- Conflict: 2-3 segundos
- **Total: 20-30 segundos para suite completa**

---

## 12. Manejo de Datos de Test

### Estrategia de Limpieza

**Opcion 1: Cascade Delete (Recomendado)**
- Crear usuarios/campanias con dato unico: `{{$timestamp}}`
- DB elimina automaticamente en cascade
- No requiere cleanup manual

**Opcion 2: Cleanup Requests**
```javascript
// POST DELETE request en _Cleanup folder
DELETE {{baseUrl}}/api/campanias/{{campaignId}}
Authorization: Bearer {{accessToken}}

// Pre-request script
// Obtener lista de campañas de test y eliminarlas
```

**Opcion 3: Clean Database Script**
```sql
-- Script para ejecutar antes de suite
DELETE FROM Backings WHERE CampaniaId IN (
    SELECT Id FROM Campanias WHERE Titulo LIKE '%{{$timestamp}}%'
)
DELETE FROM Campanias WHERE Titulo LIKE '%{{$timestamp}}%'
DELETE FROM Artistas WHERE NombreArtistico LIKE '%{{$timestamp}}%'
```

### Aislamiento de Tests

- Cada test crea su propia campaña con timestamp unico
- No reutiliza datos entre ejecuciones
- Parallelizable (no hay conflictos de datos)

---

## 13. Troubleshooting

### Problema: Token Invalido

**Causa:** Token expirado o malformado
**Solucion:**
```javascript
// En _Setup → Login, verificar que token se guarda correctamente
pm.environment.set('accessToken', json.data.token);
```

### Problema: 404 on valid ID

**Causa:** ID no guardado correctamente
**Solucion:**
```javascript
// Verificar que test de CREATE guarda ID:
pm.environment.set('campaignId', pm.response.json().data.id);
```

### Problema: 401 en todos los requests

**Causa:** {{accessToken}} vacio
**Solucion:**
1. Ejecutar setup primero: `--folder "_Setup"`
2. O configurar manualmente en environment

### Problema: Assertion falsa en GET despues UPDATE

**Causa:** Cambio no persistio
**Solucion:**
1. Verificar que PUT retorna 200 OK
2. Agregar delay entre PUT y GET: `"delayRequest": 100`

---

## 14. Siguiente Paso Sugerido

### Inmediato
1. Exportar esta coleccion como archivo JSON desde Postman
2. Configurar environments segun template
3. Ejecutar contra development local

### Corto Plazo (1-2 dias)
1. Implementar todos los requests
2. Agregar todos los test scripts
3. Validar contra API real

### Mediano Plazo (1 semana)
1. Integrar en CI/CD pipeline
2. Generar reportes HTML automáticos
3. Documentar fallos comunes

### Largo Plazo (2+ semanas)
1. Extender a otros endpoints (Backings, Rewards)
2. Pruebas de carga/performance
3. Security testing (SQL injection, XSS, etc.)

---

## 15. Referencias

### Archivos Relacionados
- API Contracts: `plans/crear-campania/backend/api-contracts.md`
- CQRS Rules: `.claude/rules/backend/cqrs.rule.md`
- Postman Guide: `plans/crear-campania/backend/POSTMAN_GUIDE.md`

### Documentacion Externa
- Newman CLI: https://learning.postman.com/docs/running-collections/using-newman-cli/command-line-integration-with-newman/
- Postman Tests: https://learning.postman.com/docs/writing-scripts/test-scripts/
- ServiceResponse Pattern: Industry standard response wrapper

### Comandos Utiles
```bash
# Validar coleccion sin ejecutar
newman run collection.json --dry-run

# Ejecutar y guardar variables finales
newman run collection.json -e env.json --export-environment final-env.json

# Ejecutar con delay entre requests
newman run collection.json --delay-request 500

# Solo requests de una carpeta especifica
newman run collection.json --folder "201 CREATED"

# Ejecutar en paralelo (limitado por rate limiting del API)
newman run collection.json -n 2 --reporter-json-file results.json

# Generar reporte JUnit para CI/CD
newman run collection.json -r junitxml --reporter-junitxml-export results.xml
```

---

**Plan de Testing Completado**
**Fecha de Creacion:** 2026-02-12
**Version:** 1.0
**Estado:** Ready for Implementation

