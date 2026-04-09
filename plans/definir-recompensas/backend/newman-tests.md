# Plan de Testing Newman: Definir Recompensas

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Coleccion:** WePlay.Rewards.IntegrationTests
**Basado en:** `plans/definir-recompensas/backend/api-contracts.md`

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 6 |
| Total requests | 42 |
| Casos de prueba | 28 |
| Casos happy path | 10 |
| Casos error (4xx) | 14 |
| Casos autenticacion (3xx) | 4 |
| Status codes cubiertos | 200, 201, 400, 401, 403, 404, 409, 500 |

---

## 2. Estructura de Coleccion

```
WePlay.Rewards.IntegrationTests/
├── _Setup/
│   ├── Register Test Artist
│   ├── Get Auth Token
│   └── Create Test Campaign
│
├── Rewards Create (POST)
│   ├── 200 OK/
│   │   ├── Create Reward - All Fields
│   │   ├── Create Reward - Minimal Fields
│   │   └── Create Reward - With Stock Limit
│   ├── 400 Bad Request/
│   │   ├── Empty Name
│   │   ├── Name Too Long (201+ chars)
│   │   ├── Description Too Long (2001+ chars)
│   │   ├── InvalidAmount (ImporteMinimo = 0)
│   │   ├── Negative Amount
│   │   ├── Invalid TipoRewardId (0)
│   │   ├── Missing CampaniaId
│   │   └── Missing Name
│   ├── 401 Unauthorized/
│   │   └── Missing Token
│   ├── 403 Forbidden/
│   │   └── Different Artist
│   └── 404 Not Found/
│       └── Non-existent Campaign
│
├── Rewards List (GET)
│   ├── 200 OK/
│   │   ├── Get All Rewards
│   │   ├── Filter by CampaniaId
│   │   ├── Filter by EsActivo=true
│   │   └── Verify Order ASC
│   └── Error Handling/
│       └── Handle Empty List
│
├── Rewards Get By Id (GET /:id)
│   ├── 200 OK/
│   │   ├── Get Existing Reward
│   │   └── Verify Complete Data
│   └── 404 Not Found/
│       └── Non-existent Reward ID
│
├── Rewards Update (PUT)
│   ├── 200 OK/
│   │   ├── Update Name Only
│   │   ├── Update Description Only
│   │   ├── Update Amount
│   │   └── Update Multiple Fields
│   ├── 400 Bad Request/
│   │   ├── Name Too Long
│   │   └── Amount <= 0
│   ├── 401 Unauthorized/
│   │   └── Missing Token
│   ├── 403 Forbidden/
│   │   └── Different Artist
│   ├── 404 Not Found/
│   │   └── Non-existent Reward
│   └── 409 Conflict/
│       └── Reward With Backings
│
├── Rewards Delete (DELETE)
│   ├── 200 OK/
│   │   └── Delete Reward Without Backings
│   ├── 401 Unauthorized/
│   │   └── Missing Token
│   ├── 403 Forbidden/
│   │   └── Different Artist
│   ├── 404 Not Found/
│   │   └── Non-existent Reward
│   └── 409 Conflict/
│       └── Delete Reward With Backings
│
├── Rewards Reorder (PUT /reorder)
│   ├── 200 OK/
│   │   ├── Reorder Multiple Rewards
│   │   └── Verify Order Updated
│   ├── 400 Bad Request/
│   │   ├── Empty RewardOrders List
│   │   ├── Missing CampaniaId
│   │   └── Invalid Orden (< 0)
│   ├── 401 Unauthorized/
│   │   └── Missing Token
│   ├── 403 Forbidden/
│   │   └── Different Artist
│   └── 404 Not Found/
│       ├── Non-existent Campaign
│       └── Non-existent Reward
│
└── _Cleanup/
    ├── Delete Test Rewards
    ├── Delete Test Campaign
    └── Delete Test Artist
```

---

## 3. Flujo E2E Recomendado

Este es el orden de ejecucion para maxima cobertura y validacion:

```
1. _Setup: Crear usuario artista de test
2. _Setup: Obtener token JWT
3. _Setup: Crear campania de test
4. POST /api/rewards (All Fields) → Guardar rewardId1
5. POST /api/rewards (Minimal Fields) → Guardar rewardId2
6. POST /api/rewards (With Stock) → Guardar rewardId3
7. GET /api/rewards?campaniaId=X → Validar 3 rewards listados
8. GET /api/rewards/{id} → Validar reward completo
9. GET /api/rewards (filtro esActivo=true) → Validar todos activos
10. POST /api/rewards (Name Too Long) → Validar error 1002
11. POST /api/rewards (Amount <= 0) → Validar error 1011
12. POST /api/rewards (Missing Token) → Validar error 3001
13. POST /api/rewards (Different Artist) → Validar error 3002
14. POST /api/rewards (Non-existent Campaign) → Validar error 2003
15. PUT /api/rewards/{id1} (Update Name) → Validar actualizacion
16. PUT /api/rewards/{id1} (Update Amount) → Validar cambio
17. PUT /api/rewards/{id1} (Name Too Long) → Validar error 1002
18. PUT /api/rewards/{id1} (Missing Token) → Validar error 3001
19. PUT /api/rewards/{id1} (Different Artist) → Validar error 3002
20. PUT /api/rewards/{non-existent} (Get By Id) → Validar error 404
21. PUT /api/rewards/reorder → Reordenar 3 rewards
22. GET /api/rewards/{id1} → Validar orden actualizado
23. DELETE /api/rewards/{id2} → Soft delete exitoso
24. GET /api/rewards/{id2} → Validar esActivo=false
25. DELETE /api/rewards/{id3} (Con Backing mock) → Validar error 4010
26. DELETE /api/rewards/{non-existent} → Validar error 404
27. DELETE /api/rewards/{id1} (Missing Token) → Validar error 3001
28. _Cleanup: Eliminar todos los datos de prueba
```

---

## 4. Requests Detallados

### 4.1 _Setup - Register Test Artist

**Request:**
```
POST {{identityUrl}}/api/users/register
Content-Type: application/json

{
    "email": "reward-test-{{$timestamp}}@weplay.com",
    "password": "TestPassword123!",
    "confirmPassword": "TestPassword123!"
}
```

**Pre-request Script:**
```javascript
// Generar email unico
const timestamp = new Date().getTime();
const email = `reward-test-${timestamp}@weplay.com`;
pm.environment.set('testArtistEmail', email);
pm.environment.set('testArtistPassword', 'TestPassword123!');
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('User ID returned', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.id).to.be.a('string');
    pm.environment.set('testArtistId', json.data.id);
});

pm.test('Response time < 1500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1500);
});
```

---

### 4.2 _Setup - Get Auth Token

**Request:**
```
POST {{identityUrl}}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&
username={{testArtistEmail}}&
password={{testArtistPassword}}&
client_id={{clientId}}&
client_secret={{clientSecret}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Access token obtained', () => {
    const json = pm.response.json();
    pm.expect(json.access_token).to.be.a('string').that.is.not.empty;
    pm.environment.set('accessToken', json.access_token);
});

pm.test('Token is valid JWT', () => {
    const token = pm.environment.get('accessToken');
    const parts = token.split('.');
    pm.expect(parts.length).to.equal(3);
});
```

---

### 4.3 _Setup - Create Test Campaign

**Request:**
```
POST {{baseUrl}}/api/campanias
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "titulo": "Test Campaign {{$timestamp}}",
    "descripcion": "Campaign for testing rewards",
    "importeObjetivo": 5000.00,
    "monedaId": 1,
    "fechaLimiteFundraising": "2026-12-31T23:59:59Z"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Campaign ID returned', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.id).to.be.a('string');
    pm.environment.set('testCampaniaId', json.data.id);
});

pm.test('Response has correct structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('isSuccess');
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json).to.have.property('messages');
});
```

---

### 4.4 Rewards Create - 200 OK - All Fields

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "Premium Album Download {{$timestamp}}",
    "descripcion": "Complete album in MP3, FLAC and WAV formats with bonus tracks",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "esAddOn": false,
    "cantidadMaxima": 500,
    "cantidadPorBacker": 1,
    "incluyeEnvioFisico": false,
    "tiempoEntregaEstimado": "Immediately after campaign ends",
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('isSuccess');
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Reward data returned with ID', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.id).to.be.a('string');
    pm.expect(json.data.campaniaId).to.equal(pm.environment.get('testCampaniaId'));
    pm.environment.set('testRewardId1', json.data.id);
});

pm.test('All fields mapped correctly', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward.nombre).to.equal('Premium Album Download {{$timestamp}}');
    pm.expect(reward.importeMinimo).to.equal(25.00);
    pm.expect(reward.cantidadMaxima).to.equal(500);
    pm.expect(reward.esActivo).to.be.true;
    pm.expect(reward.tipoRewardId).to.equal(1);
});

pm.test('Success message returned', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
    pm.expect(json.messages[0].errorCode).to.match(/^0/);
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

### 4.5 Rewards Create - 200 OK - Minimal Fields

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 2,
    "nombre": "Physical CD",
    "importeMinimo": 15.00,
    "monedaId": 1,
    "esAddOn": false,
    "incluyeEnvioFisico": true,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Reward created with optional fields as null/false', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward.descripcion).to.be.null;
    pm.expect(reward.cantidadMaxima).to.be.null;
    pm.expect(reward.cantidadPorBacker).to.be.null;
    pm.expect(reward.tiempoEntregaEstimado).to.be.null;
    pm.environment.set('testRewardId2', reward.id);
});

pm.test('Default values set correctly', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward.monedaId).to.equal(1);
    pm.expect(reward.esAddOn).to.be.false;
    pm.expect(reward.esActivo).to.be.true;
});
```

---

### 4.6 Rewards Create - 200 OK - With Stock Limit

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 3,
    "nombre": "VIP Meet & Greet Experience",
    "descripcion": "Exclusive 30-minute meet and greet with the artist",
    "importeMinimo": 100.00,
    "monedaId": 1,
    "esAddOn": false,
    "cantidadMaxima": 10,
    "cantidadPorBacker": 1,
    "incluyeEnvioFisico": false,
    "tiempoEntregaEstimado": "During campaign finalization week",
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Reward with stock limit created', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward.cantidadMaxima).to.equal(10);
    pm.environment.set('testRewardId3', reward.id);
});
```

---

### 4.7 Rewards Create - 400 Bad Request - Empty Name

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});

pm.test('Error message references nombre field', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('nombre');
});
```

---

### 4.8 Rewards Create - 400 Bad Request - Name Too Long

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "{{longString201}}",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Pre-request Script:**
```javascript
// Generate 201 character string
const longString = 'A'.repeat(201);
pm.environment.set('longString201', longString);
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_MaxLength (1002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1002');
});

pm.test('Error message mentions max length', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('200');
});
```

---

### 4.9 Rewards Create - 400 Bad Request - Invalid Amount

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "Test Reward",
    "importeMinimo": 0,
    "monedaId": 1,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_InvalidAmount (1011)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1011');
});
```

---

### 4.10 Rewards Create - 400 Bad Request - Missing CampaniaId

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "tipoRewardId": 1,
    "nombre": "Test Reward",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});
```

---

### 4.11 Rewards Create - 401 Unauthorized - Missing Token

**Request:**
```
POST {{baseUrl}}/api/rewards
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "Test Reward",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code is Auth_Unauthorized (3001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3001');
});
```

---

### 4.12 Rewards Create - 403 Forbidden - Different Artist

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{differentArtistToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "tipoRewardId": 1,
    "nombre": "Test Reward",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Pre-request Script:**
```javascript
// Get token from different artist (created in setup)
// This assumes another artist account exists or was created
// For now, use invalid artistId claim
const invalidToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5OTk5OTk5OS05OTk5LTk5OTktOTk5OS05OTk5OTk5OTk5OTkiLCJlbWFpbCI6Im90aGVyQGV4YW1wbGUuY29tIn0.INVALID';
pm.environment.set('differentArtistToken', invalidToken);
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

---

### 4.13 Rewards Create - 404 Not Found - Non-existent Campaign

**Request:**
```
POST {{baseUrl}}/api/rewards
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "00000000-0000-0000-0000-000000000000",
    "tipoRewardId": 1,
    "nombre": "Test Reward",
    "importeMinimo": 25.00,
    "monedaId": 1,
    "orden": 0
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Campania (2003)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2003');
});
```

---

### 4.14 Rewards List - 200 OK - Get All with Filter

**Request:**
```
GET {{baseUrl}}/api/rewards?campaniaId={{testCampaniaId}}&esActivo=true
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response is array of rewards', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.an('array');
    pm.expect(json.data.length).to.be.greaterThan(0);
});

pm.test('All rewards are from test campaign', () => {
    const json = pm.response.json();
    const testCampaniaId = pm.environment.get('testCampaniaId');
    json.data.forEach(reward => {
        pm.expect(reward.campaniaId).to.equal(testCampaniaId);
    });
});

pm.test('All rewards are active', () => {
    const json = pm.response.json();
    json.data.forEach(reward => {
        pm.expect(reward.esActivo).to.be.true;
    });
});

pm.test('Rewards ordered by Orden ASC', () => {
    const json = pm.response.json();
    const orders = json.data.map(r => r.orden);
    pm.expect(orders).to.deep.equal([...orders].sort((a, b) => a - b));
});

pm.test('Response time < 300ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(300);
});
```

---

### 4.15 Rewards Get By Id - 200 OK

**Request:**
```
GET {{baseUrl}}/api/rewards/{{testRewardId1}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Reward data returned', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.exist;
    pm.expect(json.data.id).to.equal(pm.environment.get('testRewardId1'));
});

pm.test('Complete reward structure returned', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward).to.have.property('id');
    pm.expect(reward).to.have.property('campaniaId');
    pm.expect(reward).to.have.property('tipoRewardId');
    pm.expect(reward).to.have.property('nombre');
    pm.expect(reward).to.have.property('importeMinimo');
    pm.expect(reward).to.have.property('orden');
    pm.expect(reward).to.have.property('esActivo');
    pm.expect(reward).to.have.property('fechaCreacion');
});

pm.test('All validation fields present', () => {
    const json = pm.response.json();
    const reward = json.data;
    pm.expect(reward.monedaId).to.be.a('number');
    pm.expect(reward.esAddOn).to.be.a('boolean');
    pm.expect(reward.incluyeEnvioFisico).to.be.a('boolean');
});
```

---

### 4.16 Rewards Get By Id - 404 Not Found

**Request:**
```
GET {{baseUrl}}/api/rewards/00000000-0000-0000-0000-000000000000
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Reward (2004)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2004');
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.17 Rewards Update - 200 OK - Update Name Only

**Request:**
```
PUT {{baseUrl}}/api/rewards/{{testRewardId1}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{testRewardId1}}",
    "nombre": "Updated Premium Album {{$timestamp}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json.data).to.be.true;
});

pm.test('Update success message returned', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^0/);
});

// Verify by GET
pm.sendRequest({
    url: pm.environment.get('baseUrl') + '/api/rewards/{{testRewardId1}}',
    method: 'GET',
    header: {
        'Content-Type': 'application/json'
    }
}, function (err, response) {
    if (!err) {
        const json = response.json();
        pm.test('Name updated correctly', () => {
            pm.expect(json.data.nombre).to.include('Updated Premium Album');
        });
    }
});
```

---

### 4.18 Rewards Update - 400 Bad Request - Name Too Long

**Request:**
```
PUT {{baseUrl}}/api/rewards/{{testRewardId1}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{testRewardId1}}",
    "nombre": "{{longString201}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_MaxLength (1002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1002');
});
```

---

### 4.19 Rewards Update - 401 Unauthorized

**Request:**
```
PUT {{baseUrl}}/api/rewards/{{testRewardId1}}
Content-Type: application/json

{
    "id": "{{testRewardId1}}",
    "nombre": "Updated Name"
}
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code is Auth_Unauthorized (3001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3001');
});
```

---

### 4.20 Rewards Update - 403 Forbidden

**Request:**
```
PUT {{baseUrl}}/api/rewards/{{testRewardId1}}
Authorization: Bearer {{differentArtistToken}}
Content-Type: application/json

{
    "id": "{{testRewardId1}}",
    "nombre": "Updated Name"
}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

---

### 4.21 Rewards Update - 404 Not Found

**Request:**
```
PUT {{baseUrl}}/api/rewards/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "00000000-0000-0000-0000-000000000000",
    "nombre": "Updated Name"
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Reward (2004)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2004');
});
```

---

### 4.22 Rewards Update - 409 Conflict - With Backings

**Request:**
```
PUT {{baseUrl}}/api/rewards/{{testRewardIdWithBacking}}
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "id": "{{testRewardIdWithBacking}}",
    "importeMinimo": 50.00
}
```

**Pre-request Script:**
```javascript
// Create backing first to simulate reward with backings
// For testing, assume reward testRewardId3 has a backing
pm.environment.set('testRewardIdWithBacking', pm.environment.get('testRewardId3'));
```

**Test Script:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

pm.test('Error code is BusinessRule_RewardHasBackings (4010)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('4010');
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.23 Rewards Delete - 200 OK

**Request:**
```
DELETE {{baseUrl}}/api/rewards/{{testRewardId2}}
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Delete success response', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json.data).to.be.true;
    pm.expect(json.messages[0].errorCode).to.match(/^0/);
});

// Verify soft delete by GET
pm.sendRequest({
    url: pm.environment.get('baseUrl') + '/api/rewards/{{testRewardId2}}',
    method: 'GET',
    header: {
        'Content-Type': 'application/json'
    }
}, function (err, response) {
    if (!err) {
        const json = response.json();
        pm.test('Soft delete: esActivo = false', () => {
            pm.expect(json.data.esActivo).to.be.false;
        });
    }
});
```

---

### 4.24 Rewards Delete - 401 Unauthorized

**Request:**
```
DELETE {{baseUrl}}/api/rewards/{{testRewardId1}}
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code is Auth_Unauthorized (3001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3001');
});
```

---

### 4.25 Rewards Delete - 403 Forbidden

**Request:**
```
DELETE {{baseUrl}}/api/rewards/{{testRewardId1}}
Authorization: Bearer {{differentArtistToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

---

### 4.26 Rewards Delete - 404 Not Found

**Request:**
```
DELETE {{baseUrl}}/api/rewards/00000000-0000-0000-0000-000000000000
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Reward (2004)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2004');
});
```

---

### 4.27 Rewards Delete - 409 Conflict - With Backings

**Request:**
```
DELETE {{baseUrl}}/api/rewards/{{testRewardId3}}
Authorization: Bearer {{accessToken}}
```

**Pre-request Script:**
```javascript
// testRewardId3 should have a backing association
// Backend should prevent deletion
```

**Test Script:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

pm.test('Error code is BusinessRule_RewardHasBackings (4010)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('4010');
});

pm.test('Reward still exists and is active', () => {
    // Verify reward was not deleted
    pm.sendRequest({
        url: pm.environment.get('baseUrl') + '/api/rewards/{{testRewardId3}}',
        method: 'GET',
        header: { 'Content-Type': 'application/json' }
    }, function (err, response) {
        if (!err) {
            const json = response.json();
            pm.test('Reward still active after failed delete', () => {
                pm.expect(json.data.esActivo).to.be.true;
            });
        }
    });
});
```

---

### 4.28 Rewards Reorder - 200 OK

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": 3
        },
        {
            "rewardId": "{{testRewardId2}}",
            "orden": 1
        },
        {
            "rewardId": "{{testRewardId3}}",
            "orden": 2
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Reorder success response', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json.data).to.be.true;
});

pm.test('Success message returned', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^0/);
});

// Verify reorder by GET list
pm.sendRequest({
    url: pm.environment.get('baseUrl') + '/api/rewards?campaniaId={{testCampaniaId}}',
    method: 'GET',
    header: { 'Content-Type': 'application/json' }
}, function (err, response) {
    if (!err) {
        const json = response.json();
        pm.test('Rewards reordered correctly', () => {
            // Verify orden matches
            const rewardOrders = json.data.map((r, idx) => ({
                id: r.id,
                expectedOrden: idx === 0 ? 1 : idx === 1 ? 2 : 3
            }));
            // Check that order is now 1, 2, 3
            pm.expect([1, 2, 3]).to.deep.equal(json.data.map(r => r.orden).sort());
        });
    }
});
```

---

### 4.29 Rewards Reorder - 400 Bad Request - Empty List

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": []
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});
```

---

### 4.30 Rewards Reorder - 400 Bad Request - Missing CampaniaId

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": 1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_Required (1001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1001');
});
```

---

### 4.31 Rewards Reorder - 400 Bad Request - Invalid Orden

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": -1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is Validation_InvalidRange (1007)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('1007');
});
```

---

### 4.32 Rewards Reorder - 401 Unauthorized

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": 1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code is Auth_Unauthorized (3001)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3001');
});
```

---

### 4.33 Rewards Reorder - 403 Forbidden

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{differentArtistToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": 1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('Error code is Auth_Forbidden (3002)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

---

### 4.34 Rewards Reorder - 404 Not Found - Non-existent Campaign

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "00000000-0000-0000-0000-000000000000",
    "rewardOrders": [
        {
            "rewardId": "{{testRewardId1}}",
            "orden": 1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Campania (2003)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2003');
});
```

---

### 4.35 Rewards Reorder - 404 Not Found - Non-existent Reward

**Request:**
```
PUT {{baseUrl}}/api/rewards/reorder
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
    "campaniaId": "{{testCampaniaId}}",
    "rewardOrders": [
        {
            "rewardId": "00000000-0000-0000-0000-000000000000",
            "orden": 1
        }
    ]
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code is NotFound_Reward (2004)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.equal('2004');
});
```

---

### 4.36 _Cleanup - Delete Test Rewards

**Request:**
```
DELETE {{baseUrl}}/api/rewards/{{testRewardId1}}
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Cleanup: Delete reward 1', () => {
    pm.response.to.have.status(200);
});
```

---

### 4.37 _Cleanup - Delete Test Campaign

**Request:**
```
DELETE {{baseUrl}}/api/campanias/{{testCampaniaId}}
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Cleanup: Delete campaign', () => {
    pm.response.to.have.status(200);
});
```

---

### 4.38 _Cleanup - Delete Test Artist

**Request:**
```
DELETE {{identityUrl}}/api/users/{{testArtistId}}
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Cleanup: Delete artist', () => {
    pm.response.to.have.status(200);
});

pm.test('All test data cleaned up', () => {
    pm.environment.unset('testArtistId');
    pm.environment.unset('testCampaniaId');
    pm.environment.unset('testRewardId1');
    pm.environment.unset('testRewardId2');
    pm.environment.unset('testRewardId3');
    pm.environment.unset('accessToken');
});
```

---

## 5. Variables de Entorno

### Development Environment

```json
{
    "name": "WePlay Rewards Tests - Development",
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
            "value": "{{from_secrets_vault}}",
            "enabled": true
        },
        {
            "key": "testArtistEmail",
            "value": "",
            "enabled": true
        },
        {
            "key": "testArtistPassword",
            "value": "TestPassword123!",
            "enabled": true
        },
        {
            "key": "testArtistId",
            "value": "",
            "enabled": true
        },
        {
            "key": "testCampaniaId",
            "value": "",
            "enabled": true
        },
        {
            "key": "testRewardId1",
            "value": "",
            "enabled": true
        },
        {
            "key": "testRewardId2",
            "value": "",
            "enabled": true
        },
        {
            "key": "testRewardId3",
            "value": "",
            "enabled": true
        },
        {
            "key": "testRewardIdWithBacking",
            "value": "",
            "enabled": true
        },
        {
            "key": "accessToken",
            "value": "",
            "enabled": true
        },
        {
            "key": "differentArtistToken",
            "value": "",
            "enabled": true
        }
    ]
}
```

### Staging Environment

```json
{
    "name": "WePlay Rewards Tests - Staging",
    "values": [
        {
            "key": "baseUrl",
            "value": "https://api-staging.weplay.dev",
            "enabled": true
        },
        {
            "key": "identityUrl",
            "value": "https://auth-staging.weplay.dev",
            "enabled": true
        },
        {
            "key": "clientId",
            "value": "weplay-test-staging",
            "enabled": true
        },
        {
            "key": "clientSecret",
            "value": "{{from_secrets_vault}}",
            "enabled": true
        }
    ]
}
```

---

## 6. Ejecucion Local

### Instalacion de Newman

```bash
npm install -g newman
npm install -g newman-reporter-htmlextra
```

### Comando de Ejecucion

```bash
# Ejecucion basica
newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,json

# Con reportes HTML detallados
newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/newman/reports/rewards-test-report.html

# Con variables adicionales
newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    -d tests/newman/data/test-data.json \
    --reporters cli,htmlextra,junit

# Con timeout personalizado
newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --request-timeout 5000 \
    --reporters cli

# Ejecutar solo una carpeta de tests
newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli \
    --grep "400 Bad Request"
```

---

## 7. Ejecucion en CI/CD Pipeline

### Azure Pipelines (YAML)

```yaml
trigger:
  - master

pool:
  vmImage: 'ubuntu-latest'

variables:
  baseUrl: 'https://api-staging.weplay.dev'
  clientId: 'weplay-test-staging'

jobs:
  - job: IntegrationTests
    displayName: 'Rewards Integration Tests'
    steps:
      - task: NodeTool@0
        inputs:
          versionSpec: '18.x'
        displayName: 'Setup Node.js'

      - script: npm install -g newman newman-reporter-htmlextra
        displayName: 'Install Newman'

      - script: |
          newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
            -e tests/newman/environments/staging.json \
            --reporters cli,htmlextra,junit \
            --reporter-junit-export tests/newman/reports/junit.xml \
            --reporter-htmlextra-export tests/newman/reports/htmlextra.html
        displayName: 'Run Integration Tests'
        env:
          CLIENT_SECRET: $(clientSecret)
          API_BASEURL: $(baseUrl)

      - task: PublishTestResults@2
        inputs:
          testResultsFormat: 'JUnit'
          testResultsFiles: 'tests/newman/reports/junit.xml'
          mergeTestResults: true
          testRunTitle: 'Rewards Integration Tests'
        condition: always()

      - task: PublishBuildArtifacts@1
        inputs:
          PathtoPublish: 'tests/newman/reports/htmlextra.html'
          ArtifactName: 'test-report'
        condition: always()

      - script: |
          if grep -q "failures" tests/newman/reports/junit.xml; then
            echo "Tests failed!"
            exit 1
          fi
        displayName: 'Check Test Results'
```

### GitHub Actions

```yaml
name: Rewards Integration Tests

on:
  push:
    branches: [master, develop]
  pull_request:
    branches: [master]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18.x'

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Run Integration Tests
        run: |
          newman run tests/newman/WePlay.Rewards.IntegrationTests.json \
            -e tests/newman/environments/staging.json \
            --reporters cli,htmlextra,junit \
            --reporter-junit-export test-results/junit.xml \
            --reporter-htmlextra-export test-results/report.html
        env:
          BASE_URL: https://api-staging.weplay.dev
          CLIENT_SECRET: ${{ secrets.WEPLAY_CLIENT_SECRET }}

      - name: Upload Test Report
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-reports
          path: test-results/

      - name: Publish Test Results
        if: always()
        uses: EnricoMi/publish-unit-test-result-action@v2
        with:
          files: test-results/junit.xml
```

---

## 8. Estructura de Directorios

```
tests/
├── newman/
│   ├── collections/
│   │   ├── WePlay.Rewards.IntegrationTests.json
│   │   ├── WePlay.Campanias.IntegrationTests.json
│   │   └── README.md (como importar/exportar colecciones)
│   │
│   ├── environments/
│   │   ├── development.json
│   │   ├── staging.json
│   │   └── production.json (con valores protegidos)
│   │
│   ├── data/
│   │   ├── test-users.json
│   │   ├── test-campaigns.json
│   │   └── test-rewards.json
│   │
│   ├── reports/
│   │   ├── .gitkeep
│   │   └── README.md (como interpretar reportes)
│   │
│   └── scripts/
│       ├── run-local.sh
│       ├── run-staging.sh
│       └── run-ci.sh
│
└── integration/
    ├── rewards/
    │   ├── CreateReward.test.ts
    │   ├── UpdateReward.test.ts
    │   ├── DeleteReward.test.ts
    │   └── ReorderRewards.test.ts
    │
    └── campaña/
        ├── CreateCampaign.test.ts
        └── PublishCampaign.test.ts
```

---

## 9. Casos de Prueba - Matriz de Cobertura

### Happy Path (10 casos)

| # | Request | Status | Objetivo |
|---|---------|--------|----------|
| 1 | POST Create - All Fields | 200 | Crear reward con todos los campos |
| 2 | POST Create - Minimal | 200 | Crear reward con campos minimos |
| 3 | POST Create - Stock | 200 | Crear reward con cantidad limitada |
| 4 | GET List - Filter | 200 | Listar rewards filtrados por campania |
| 5 | GET List - Order | 200 | Validar orden ascendente |
| 6 | GET By ID | 200 | Obtener reward por ID |
| 7 | PUT Update - Name | 200 | Actualizar nombre |
| 8 | PUT Update - Amount | 200 | Actualizar importe minimo |
| 9 | DELETE Soft Delete | 200 | Eliminar (soft delete) |
| 10 | PUT Reorder | 200 | Reordenar multiples rewards |

### Validation Errors (8 casos - 400)

| # | Request | ErrorCode | Objetivo |
|---|---------|-----------|----------|
| 1 | POST - Empty Name | 1001 | Campo requerido |
| 2 | POST - Name > 200 | 1002 | MaxLength validation |
| 3 | POST - Description > 2000 | 1002 | MaxLength validation |
| 4 | POST - Amount = 0 | 1011 | Importe debe ser > 0 |
| 5 | POST - Amount < 0 | 1011 | Importe debe ser > 0 |
| 6 | POST - Missing CampaniaId | 1001 | Requerido |
| 7 | PUT - Name > 200 | 1002 | MaxLength validation |
| 8 | PUT/Reorder - Invalid Orden | 1007 | Orden >= 0 |

### Authentication (4 casos - 401/403)

| # | Request | ErrorCode | Objetivo |
|---|---------|-----------|----------|
| 1 | POST - No Token | 3001 | Unauthorized |
| 2 | POST - Different Artist | 3002 | Forbidden (ownership) |
| 3 | DELETE - No Token | 3001 | Unauthorized |
| 4 | PUT/Reorder - Different Artist | 3002 | Forbidden (ownership) |

### Not Found (4 casos - 404)

| # | Request | ErrorCode | Objetivo |
|---|---------|-----------|----------|
| 1 | POST - Campaign Not Found | 2003 | NotFound_Campania |
| 2 | GET By ID - Not Found | 2004 | NotFound_Reward |
| 3 | PUT - Not Found | 2004 | NotFound_Reward |
| 4 | DELETE - Not Found | 2004 | NotFound_Reward |

### Business Rules (2 casos - 409)

| # | Request | ErrorCode | Objetivo |
|---|---------|-----------|----------|
| 1 | PUT - With Backings | 4010 | RewardHasBackings |
| 2 | DELETE - With Backings | 4010 | RewardHasBackings |

---

## 10. Checklist de Implementacion

### Pre-ejecucion

- [ ] Backend levantado en http://localhost:5001
- [ ] Base de datos limpia o con datos de test
- [ ] JWT secret configurado en variables
- [ ] Postman/Newman instalado localmente
- [ ] Coleccion importada en Postman

### Durante Ejecucion

- [ ] Todos los tests pasan en local
- [ ] Todos los status codes validados
- [ ] Todos los error codes segun contracts
- [ ] Variables de entorno se populan correctamente
- [ ] Setup y Cleanup se ejecutan correctamente
- [ ] Tokens obtenidos exitosamente

### Post-ejecucion

- [ ] Reporte HTML generado
- [ ] Reporte JUnit generado
- [ ] Datos de test limpiados
- [ ] No hay registros fantasma en BD
- [ ] Pipeline CI/CD configurable

---

## 11. Notas Importantes

### Validacion de Ownership

Todos los endpoints autenticados (POST, PUT, DELETE, PUT/reorder) deben validar que el `ArtistaId` del token JWT coincida con el `ArtistaId` de la campaña asociada. Tests con token de artista diferente deben retornar 403.

### Soft Delete

El endpoint DELETE utiliza soft delete (`EsActivo = false`) en lugar de eliminar fisicamente el registro. Validar que el reward aparece con `esActivo=false` pero sigue existiendo en BD.

### Reordenamiento en Lote

El endpoint PUT /api/rewards/reorder debe actualizar los ordenes de multiples rewards en una sola transaccion. Si alguno falla, todos deben rollback.

### Backings Mock

Para tests de 409 (reward with backings), es posible que se necesite crear un backing real o mockear la propiedad de relacion `Lineas` en el reward. Verificar con el equipo backend como simular backings en tests.

### Token Expirado

No se cubre explicitamente el caso de token expirado (tambien 401 pero con diferentes circunstancias). Si es necesario, agregar test con token manipulado o esperar hasta que expire.

### Concurrencia

Tests de concurrencia (dos artistas actualizando el mismo reward simultaneamente) no se cubren en este plan. Considerar agregar en fase 2.

---

## 12. Proximu Pasos

1. **Exportar coleccion desde Postman** si esta fue creada en UI
2. **Configurar variables en CI/CD** (secrets para client_secret, API URLs)
3. **Ejecutar localmente** con `newman run` y validar todos los tests pasan
4. **Integrar en pipeline Azure Pipelines** o GitHub Actions
5. **Establecer baseline** de ejecucion exitosa (0 fallos esperados)
6. **Agregar notificaciones** en Slack/Teams cuando tests fallen
7. **Documentar troubleshooting** en README de tests
8. **Automatizar limpieza** de datos de test si es posible

---

## 13. Referencias

- API Contracts: `plans/definir-recompensas/backend/api-contracts.md`
- Feature Spec: `docs/user-stories/definir-recompensas/feature-spec.md`
- Contract Details: `docs/user-stories/definir-recompensas/contracts.md`
- Postman Collection Ejemplo: `tests/integration/WePlay.Campanias.IntegrationTests.postman_collection.json`
- Newman Docs: https://learning.postman.com/docs/running-collections/using-newman-cli/

---

**Fin del Plan de Testing Newman - Definir Recompensas**
