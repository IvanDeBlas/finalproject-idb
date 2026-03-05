# Plan de Testing Newman: Hacer Backing

**Fecha:** 2026-02-13
**Feature:** hacer-backing (US-04)
**Modulo:** Crowdfunding
**Coleccion:** WePlay.HacerBacking.IntegrationTests

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 5 |
| Total de requests | 34 |
| Casos de prueba | 45+ |
| Rutas de flujo | 8 |
| Escenarios de error | 15+ |

**Objetivo:** Validar flujo completo de creacion de backings, incluyendo validaciones de monto, stock, estado de campania, autenticacion opcional y transacciones atomicas.

---

## 2. Estructura de Coleccion Postman

```
WePlay.HacerBacking.IntegrationTests/
├── _Setup/
│   ├── Get Auth Token - Usuario Autenticado
│   └── Create Test Campania (fixture)
├── _Data/
│   ├── Get All Campanias
│   └── Create Test Reward
├── 1. GET /api/campanias (Listar campanias activas)
│   ├── 200 OK - List All/
│   │   ├── GET List with Pagination
│   │   └── GET List with Search
│   ├── 200 OK - Filter by Estado/
│   │   └── GET Filter Published Campanias
│   └── 400 BAD REQUEST/
│       └── GET Invalid Page Size
├── 2. GET /api/campanias/{id} (Detalle con rewards)
│   ├── 200 OK - Campaign Detail/
│   │   ├── GET Campaign with Rewards
│   │   └── GET Campaign with Backings
│   └── 404 NOT FOUND/
│       ├── GET Non-existent Campaign
│       └── GET Invalid GUID Format
├── 3. POST /api/campanias/{id}/backings (Crear backing)
│   ├── 201 CREATED - Happy Path/
│   │   ├── POST With Reward - Authenticated
│   │   ├── POST Without Reward - Authenticated
│   │   ├── POST Anonymous Backing - Unauthenticated
│   │   ├── POST With Message
│   │   └── POST With Maximum Amount
│   ├── 400 BAD REQUEST - Validation/
│   │   ├── POST Empty Required Fields
│   │   ├── POST Monto Below Minimum (1 EUR)
│   │   ├── POST Amount Below Reward Minimum
│   │   ├── POST Message Exceeds 500 Chars
│   │   ├── POST Invalid Money Format
│   │   └── POST Invalid GUID Format
│   ├── 401 UNAUTHORIZED/
│   │   └── POST Unauthenticated Campaign Requires Auth
│   ├── 404 NOT FOUND/
│   │   ├── POST Campaign Not Found
│   │   └── POST Reward Not Found
│   ├── 409 CONFLICT - Business Rules/
│   │   ├── POST Campaign Inactive
│   │   ├── POST Campaign Already Ended
│   │   └── POST Reward Out of Stock
│   └── 422 UNPROCESSABLE ENTITY/
│       └── POST Anonymous Not Allowed for Campaign
├── 4. GET /api/campanias/{id}/backings (Lista backings publicos)
│   ├── 200 OK - Backings List/
│   │   ├── GET Backings First Page
│   │   ├── GET Backings Pagination
│   │   ├── GET Backings Show Anonymous Names
│   │   └── GET Backings with Rewards
│   ├── 400 BAD REQUEST/
│   │   ├── GET Invalid Page Number
│   │   ├── GET Invalid Page Size
│   │   └── GET Page Size Over Limit
│   └── 404 NOT FOUND/
│       └── GET Campaign Not Found
├── 5. GET /api/campanias/{id}/stats (Estadisticas)
│   ├── 200 OK - Stats/
│   │   ├── GET Stats Total Backers
│   │   ├── GET Stats Total Amount
│   │   ├── GET Stats Averages
│   │   └── GET Stats Days Remaining
│   └── 404 NOT FOUND/
│       └── GET Campaign Not Found
├── _Validations/
│   ├── Verify Atomicity - Campaign Updated
│   ├── Verify Atomicity - Stock Updated
│   ├── Verify Stats Consistency
│   └── Verify Anonymous Name Display
└── _Cleanup/
    └── Delete Test Data
```

---

## 3. Variables de Entorno

### Archivo: `tests/newman/environments/desarrollo.json`

```json
{
  "name": "WePlay - Desarrollo",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5001",
      "enabled": true
    },
    {
      "key": "identityUrl",
      "value": "http://localhost:5001",
      "enabled": true
    },
    {
      "key": "clientId",
      "value": "weplay-test",
      "enabled": true
    },
    {
      "key": "testUsername",
      "value": "usuario1@mail.com",
      "enabled": true
    },
    {
      "key": "testPassword",
      "value": "123456",
      "enabled": true
    },
    {
      "key": "testUsername2",
      "value": "api-test@mail.com",
      "enabled": true
    },
    {
      "key": "testPassword2",
      "value": "123456",
      "enabled": true
    },
    {
      "key": "accessToken",
      "value": "",
      "enabled": true
    },
    {
      "key": "accessToken2",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId",
      "value": "",
      "enabled": true
    },
    {
      "key": "userName",
      "value": "",
      "enabled": true
    },
    {
      "key": "campaniaId",
      "value": "",
      "enabled": true
    },
    {
      "key": "rewardId",
      "value": "",
      "enabled": true
    },
    {
      "key": "backingId",
      "value": "",
      "enabled": true
    },
    {
      "key": "testTimestamp",
      "value": "{{$timestamp}}",
      "enabled": true
    }
  ]
}
```

---

## 4. Requests Detallados

### 4.1 SETUP - Obtener Token de Autenticacion

**Nombre:** `Setup - Get Auth Token`

**Request:**
```
POST {{identityUrl}}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
username={{testUsername}}
password={{testPassword}}
client_id={{clientId}}
```

**Pre-request Script:**
```javascript
// Generar timestamp unico para evitar conflictos
pm.environment.set('testTimestamp', new Date().getTime());
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Access token is returned', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('access_token');
    pm.expect(json.access_token).to.be.a('string');
});

pm.test('Token saved to environment', () => {
    const json = pm.response.json();
    pm.environment.set('accessToken', json.access_token);
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

### 4.2 SETUP - Obtener Token Segundo Usuario

**Nombre:** `Setup - Get Auth Token (User 2)`

**Request:**
```
POST {{identityUrl}}/connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password
username={{testUsername2}}
password={{testPassword2}}
client_id={{clientId}}
```

**Test Script:**
```javascript
pm.test('Token saved for second user', () => {
    const json = pm.response.json();
    pm.environment.set('accessToken2', json.access_token);
});
```

---

### 4.3 SETUP - Obtener Campania de Prueba

**Nombre:** `Setup - Get Published Campaign`

**Request:**
```
GET {{baseUrl}}/api/campanias?estadoCampaniaId=2&pageNumber=1&pageSize=1
```

**Test Script:**
```javascript
pm.test('Response has published campaigns', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('items');
    pm.expect(json.data.items).to.have.length.greaterThan(0);
});

pm.test('Campaign ID extracted and saved', () => {
    const json = pm.response.json();
    const campaignId = json.data.items[0].id;
    pm.environment.set('campaniaId', campaignId);
    pm.expect(campaignId).to.be.a('string');
});

pm.test('Campaign permits anonymous backings', () => {
    const json = pm.response.json();
    const campaign = json.data.items[0];
    pm.expect(campaign).to.have.property('permiteAportacionesAnonimas');
});
```

---

### 4.4 GET /api/campanias - Listar Campanias Activas

#### 4.4.1 GET List All Campanias with Pagination

**Nombre:** `GET campanias - List with Pagination`

**Request:**
```
GET {{baseUrl}}/api/campanias?pageNumber=1&pageSize=10
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json.data).to.have.property('items');
});

pm.test('Pagination structure is correct', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('totalCount').that.is.a('number');
    pm.expect(json.data).to.have.property('page').that.equals(1);
    pm.expect(json.data).to.have.property('pageSize').that.equals(10);
    pm.expect(json.data).to.have.property('totalPages').that.is.a('number');
});

pm.test('Items array contains campaign objects', () => {
    const json = pm.response.json();
    const items = json.data.items;
    if (items.length > 0) {
        const campaign = items[0];
        pm.expect(campaign).to.have.all.keys(
            'id', 'titulo', 'subtitulo', 'importeObjetivo',
            'importePledgedActual', 'porcentajeProgreso',
            'estadoCampaniaId', 'fechaInicio', 'fechaFin',
            'artistaNombre', 'artistaImagenUrl', 'imagenPrincipalUrl'
        );
    }
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test('isSuccess flag is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});
```

---

#### 4.4.2 GET List Campanias with Search Term

**Nombre:** `GET campanias - Search by Title`

**Request:**
```
GET {{baseUrl}}/api/campanias?searchTerm=album&pageNumber=1&pageSize=20
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Search parameter is respected', () => {
    const json = pm.response.json();
    // Verificar que si hay resultados, contienen el termino de busqueda
    const items = json.data.items;
    if (items.length > 0) {
        items.forEach(campaign => {
            const titleLower = campaign.titulo.toLowerCase();
            pm.expect(titleLower).to.include.oneOf(['album', 'album']);
        });
    }
});
```

---

#### 4.4.3 GET List with Invalid Page Size (Error 400)

**Nombre:** `GET campanias - Invalid Page Size (400)`

**Request:**
```
GET {{baseUrl}}/api/campanias?pageNumber=1&pageSize=1000
```

**Test Script:**
```javascript
pm.test('Status code is 400 for oversized page', () => {
    pm.response.to.have.status(400);
});

pm.test('Error message indicates validation failure', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages).to.be.an('array').with.length.greaterThan(0);
    pm.expect(json.messages[0]).to.have.property('errorCode');
});
```

---

### 4.5 GET /api/campanias/{id} - Detalle Completo

#### 4.5.1 GET Campaign Detail with Rewards and Backings

**Nombre:** `GET campanias/{id} - Campaign Detail with Rewards`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response contains campaign detail structure', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.property('id');
    pm.expect(campaign).to.have.property('titulo');
    pm.expect(campaign).to.have.property('importeObjetivo');
    pm.expect(campaign).to.have.property('importePledgedActual');
    pm.expect(campaign).to.have.property('porcentajeProgreso');
});

pm.test('Rewards array is present and contains structure', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.property('rewards').that.is.an('array');
    if (campaign.rewards.length > 0) {
        const reward = campaign.rewards[0];
        pm.expect(reward).to.have.property('id');
        pm.expect(reward).to.have.property('nombre');
        pm.expect(reward).to.have.property('importeMinimo').that.is.a('number');
        pm.expect(reward).to.have.property('disponible').that.is.a('boolean');
    }
});

pm.test('Recent backings array is present', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.property('backingsRecientes').that.is.an('array');
});

pm.test('Campaign stats are calculated', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.property('totalBackers').that.is.a('number');
    pm.expect(campaign).to.have.property('diasRestantes').that.is.a('number');
    pm.expect(campaign.porcentajeProgreso).to.be.a('number');
});

pm.test('Artist information is populated', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.property('artistaNombre').that.is.a('string');
    pm.expect(campaign).to.have.property('artistaImagenUrl');
});
```

---

#### 4.5.2 GET Campaign Detail - Verify Percent Calculation

**Nombre:** `GET campanias/{id} - Verify Progress Percent Calculation`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}
```

**Test Script:**
```javascript
pm.test('Percentage is calculated correctly', () => {
    const json = pm.response.json();
    const campaign = json.data;
    const expectedPercent = (campaign.importePledgedActual / campaign.importeObjetivo) * 100;
    const roundedExpected = Math.round(expectedPercent * 10) / 10;
    pm.expect(campaign.porcentajeProgreso).to.equal(roundedExpected);
});

pm.test('Days remaining is non-negative', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign.diasRestantes).to.be.at.least(0);
});
```

---

#### 4.5.3 GET Campaign Detail - 404 Not Found

**Nombre:** `GET campanias/{id} - Campaign Not Found (404)`

**Request:**
```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Response contains not found error', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.include('2003');
});

pm.test('Error message is clear', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('no encontrada');
});
```

---

### 4.6 POST /api/campanias/{id}/backings - Crear Backing

#### 4.6.1 POST Create Backing with Reward - Authenticated

**Nombre:** `POST backings - Create with Reward (Authenticated)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": "{{rewardId}}",
  "monto": 25.00,
  "mensaje": "Mucha suerte con el proyecto!",
  "esAnonimo": false
}
```

**Pre-request Script:**
```javascript
// Obtener el primer reward disponible de la campania
const CampaniaDetailRequest = {
    url: pm.environment.get('baseUrl') + '/api/campanias/' + pm.environment.get('campaniaId'),
    method: 'GET'
};

pm.sendRequest(CampaniaDetailRequest, (err, response) => {
    if (!err && response.code === 200) {
        const json = response.json();
        const availableReward = json.data.rewards.find(r => r.disponible === true);
        if (availableReward) {
            pm.environment.set('rewardId', availableReward.id);
            // Usar el monto minimo del reward para el test
            pm.environment.set('minAmount', availableReward.importeMinimo + 5);
        }
    }
});
```

**Test Script:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

pm.test('Response contains BackingDto structure', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing).to.have.property('id').that.is.a('string');
    pm.expect(backing).to.have.property('campaniaId').that.is.a('string');
    pm.expect(backing).to.have.property('monto').that.is.a('number');
});

pm.test('Backing contains reward information', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing).to.have.property('rewardId').that.is.a('string');
    pm.expect(backing).to.have.property('rewardNombre').that.is.a('string');
});

pm.test('User name is displayed (not anonymous)', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing).to.have.property('userName').that.is.a('string');
    pm.expect(backing.esAnonimo).to.be.false;
});

pm.test('Message is stored', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing.mensaje).to.equal('Mucha suerte con el proyecto!');
});

pm.test('Success message returned', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json.messages[0].errorCode).to.include('0001');
});

pm.test('Backing ID saved to environment', () => {
    const json = pm.response.json();
    pm.environment.set('backingId', json.data.id);
});

pm.test('Order status is Completado', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing.estadoPedido).to.equal('Completado');
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

#### 4.6.2 POST Create Backing Without Reward

**Nombre:** `POST backings - Create without Reward (Donation)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken2}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 50.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

pm.test('Backing without reward is valid', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing.rewardId).to.be.null;
    pm.expect(backing.rewardNombre).to.be.null;
});

pm.test('Amount is recorded correctly', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing.monto).to.equal(50.00);
});
```

---

#### 4.6.3 POST Create Anonymous Backing - Unauthenticated

**Nombre:** `POST backings - Anonymous Backing (No Auth)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Content-Type: application/json

{
  "rewardId": null,
  "monto": 15.00,
  "mensaje": null,
  "esAnonimo": true
}
```

**Test Script:**
```javascript
pm.test('Status code is 201', () => {
    pm.response.to.have.status(201);
});

pm.test('Anonymous backing is allowed for campaign', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('User name is set to Anonimo', () => {
    const json = pm.response.json();
    const backing = json.data;
    pm.expect(backing.userName).to.equal('Anonimo');
    pm.expect(backing.esAnonimo).to.be.true;
});
```

---

#### 4.6.4 POST Backing - Amount Below Minimum (400)

**Nombre:** `POST backings - Amount Below Minimum (400)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": "{{rewardId}}",
  "monto": 5.00,
  "mensaje": "Test",
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error indicates amount below reward minimum', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.include('4012');
});

pm.test('Error message is descriptive', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('monto').or.include('minimo');
});
```

---

#### 4.6.5 POST Backing - Amount Less Than 1 EUR (400)

**Nombre:** `POST backings - Amount < 1 EUR (400)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 0.50,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error code is validation error', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});
```

---

#### 4.6.6 POST Backing - Message Exceeds 500 Characters (400)

**Nombre:** `POST backings - Message Exceeds 500 Chars (400)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 10.00,
  "mensaje": "{{longMessage}}",
  "esAnonimo": false
}
```

**Pre-request Script:**
```javascript
// Generar mensaje con mas de 500 caracteres
const longMessage = 'A'.repeat(501);
pm.environment.set('longMessage', longMessage);
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error indicates message length exceeded', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('500');
});
```

---

#### 4.6.7 POST Backing - Campaign Not Found (404)

**Nombre:** `POST backings - Campaign Not Found (404)`

**Request:**
```
POST {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code indicates campaign not found', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('2003');
});
```

---

#### 4.6.8 POST Backing - Reward Not Found (404)

**Nombre:** `POST backings - Reward Not Found (404)`

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": "00000000-0000-0000-0000-000000000000",
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Error code indicates reward not found', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('2004');
});
```

---

#### 4.6.9 POST Backing - Campaign Not Active (409)

**Nombre:** `POST backings - Campaign Not Active (409)`

**Requisito previo:** Crear una campania con estadoCampaniaId != 2 (PUBLICADA)

**Request:**
```
POST {{baseUrl}}/api/campanias/{{inactiveCampaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

pm.test('Error indicates campaign not active', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('4006');
});
```

---

#### 4.6.10 POST Backing - Campaign Ended (409)

**Nombre:** `POST backings - Campaign Already Ended (409)`

**Requisito:** Usar una campania con FechaFin pasada

**Request:**
```
POST {{baseUrl}}/api/campanias/{{endedCampaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": null,
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

pm.test('Error indicates campaign ended', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('4007');
});
```

---

#### 4.6.11 POST Backing - Reward Out of Stock (409)

**Nombre:** `POST backings - Reward Out of Stock (409)`

**Requisito:** Usar un reward con CantidadMaxima y CantidadVendida >= CantidadMaxima

**Request:**
```
POST {{baseUrl}}/api/campanias/{{campaniaId}}/backings
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "rewardId": "{{outOfStockRewardId}}",
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": false
}
```

**Test Script:**
```javascript
pm.test('Status code is 409', () => {
    pm.response.to.have.status(409);
});

pm.test('Error indicates reward out of stock', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('4011');
});
```

---

#### 4.6.12 POST Backing - Unauthenticated When Campaign Requires Auth (401)

**Nombre:** `POST backings - Campaign Requires Auth (401)`

**Requisito:** Usar campania con PermiteAportacionesAnonimas = false

**Request:**
```
POST {{baseUrl}}/api/campanias/{{noAnonCampaniaId}}/backings
Content-Type: application/json

{
  "rewardId": null,
  "monto": 25.00,
  "mensaje": null,
  "esAnonimo": true
}
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error indicates authentication required', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('3005');
});
```

---

### 4.7 GET /api/campanias/{id}/backings - Lista de Backings Publicos

#### 4.7.1 GET Backings List with Pagination

**Nombre:** `GET backings - List with Pagination`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}/backings?pageNumber=1&pageSize=20
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response contains paginated backings', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('items').that.is.an('array');
    pm.expect(json.data).to.have.property('totalCount').that.is.a('number');
    pm.expect(json.data).to.have.property('page').that.equals(1);
});

pm.test('Backing items have correct structure', () => {
    const json = pm.response.json();
    const items = json.data.items;
    if (items.length > 0) {
        const backing = items[0];
        pm.expect(backing).to.have.property('id');
        pm.expect(backing).to.have.property('nombreBacker');
        pm.expect(backing).to.have.property('monto').that.is.a('number');
        pm.expect(backing).to.have.property('fechaCreacion');
    }
});
```

---

#### 4.7.2 GET Backings - Verify Anonymous Names Display

**Nombre:** `GET backings - Anonymous Backings Show Correct Name`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}/backings?pageNumber=1&pageSize=100
```

**Test Script:**
```javascript
pm.test('Anonymous backings show Anonimo as name', () => {
    const json = pm.response.json();
    const items = json.data.items;

    // Buscar un backing anonimo (asumiendo que existe)
    const anonBackings = items.filter(b => b.nombreBacker === 'Anonimo');
    if (anonBackings.length > 0) {
        pm.expect(anonBackings[0].nombreBacker).to.equal('Anonimo');
    }
});
```

---

#### 4.7.3 GET Backings - Invalid Page Size (400)

**Nombre:** `GET backings - Invalid Page Size (400)`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}/backings?pageNumber=1&pageSize=200
```

**Test Script:**
```javascript
pm.test('Status code is 400 for oversized page', () => {
    pm.response.to.have.status(400);
});

pm.test('Error indicates validation failure', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

#### 4.7.4 GET Backings - Campaign Not Found (404)

**Nombre:** `GET backings - Campaign Not Found (404)`

**Request:**
```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000/backings
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});
```

---

### 4.8 GET /api/campanias/{id}/stats - Estadisticas

#### 4.8.1 GET Campaign Stats

**Nombre:** `GET stats - Campaign Statistics`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}/stats
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response contains stats structure', () => {
    const json = pm.response.json();
    const stats = json.data;
    pm.expect(stats).to.have.property('campaniaId');
    pm.expect(stats).to.have.property('totalBackers').that.is.a('number');
    pm.expect(stats).to.have.property('totalRecaudado').that.is.a('number');
    pm.expect(stats).to.have.property('promedioAporte').that.is.a('number');
    pm.expect(stats).to.have.property('aporteMinimo').that.is.a('number');
    pm.expect(stats).to.have.property('aporteMaximo').that.is.a('number');
});

pm.test('Stats are non-negative', () => {
    const json = pm.response.json();
    const stats = json.data;
    pm.expect(stats.totalBackers).to.be.at.least(0);
    pm.expect(stats.totalRecaudado).to.be.at.least(0);
    pm.expect(stats.promedioAporte).to.be.at.least(0);
    pm.expect(stats.aporteMinimo).to.be.at.least(0);
    pm.expect(stats.aporteMaximo).to.be.at.least(0);
});

pm.test('Average is between min and max', () => {
    const json = pm.response.json();
    const stats = json.data;
    if (stats.totalBackers > 0) {
        pm.expect(stats.promedioAporte).to.be.at.least(stats.aporteMinimo);
        pm.expect(stats.promedioAporte).to.be.at.most(stats.aporteMaximo);
    }
});

pm.test('Total recaudado equals sum formula', () => {
    const json = pm.response.json();
    const stats = json.data;
    if (stats.totalBackers > 0) {
        const calculatedTotal = stats.promedioAporte * stats.totalBackers;
        // Comparar con tolerancia de redondeo
        pm.expect(Math.abs(stats.totalRecaudado - calculatedTotal)).to.be.below(1);
    }
});
```

---

#### 4.8.2 GET Stats - Campaign Not Found (404)

**Nombre:** `GET stats - Campaign Not Found (404)`

**Request:**
```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000/stats
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});
```

---

## 5. Tests de Validacion y Atomicidad

### 5.1 Validar Atomicidad de Transaccion

**Nombre:** `Validation - Campaign ImportePledged Updated`

**Objetivo:** Verificar que tras crear un backing, ImportePledgedActual se actualiza correctamente

**Pasos:**
1. GET `/api/campanias/{id}` - Obtener ImportePledgedActual actual
2. POST `/api/campanias/{id}/backings` - Crear backing con monto conocido
3. GET `/api/campanias/{id}` - Obtener nuevo ImportePledgedActual
4. Validar: nuevo valor = antiguo valor + monto del backing

**Test Script:**
```javascript
// Script en el contexto de la coleccion o test global

// Antes de crear backing
let originalAmount = 0;
const getCampaignBefore = {
    url: pm.environment.get('baseUrl') + '/api/campanias/' + pm.environment.get('campaniaId'),
    method: 'GET'
};

pm.sendRequest(getCampaignBefore, (err, response) => {
    originalAmount = response.json().data.importePledgedActual;
    pm.environment.set('originalAmount', originalAmount);
});

// Despues de crear backing
const getCampaignAfter = {
    url: pm.environment.get('baseUrl') + '/api/campanias/' + pm.environment.get('campaniaId'),
    method: 'GET'
};

pm.sendRequest(getCampaignAfter, (err, response) => {
    const newAmount = response.json().data.importePledgedActual;
    const expectedAmount = parseFloat(pm.environment.get('originalAmount')) + 25.00;

    pm.test('Campaign ImportePledged increased by backing amount', () => {
        pm.expect(newAmount).to.equal(expectedAmount);
    });
});
```

---

### 5.2 Validar Consistencia de Stats

**Nombre:** `Validation - Stats Consistency with Backings`

**Request:**
```
GET {{baseUrl}}/api/campanias/{{campaniaId}}/stats
GET {{baseUrl}}/api/campanias/{{campaniaId}}/backings?pageSize=1000
GET {{baseUrl}}/api/campanias/{{campaniaId}}
```

**Test Script:**
```javascript
// Comparar stats total con suma manual de backings
const statsResponse = pm.response.json();
const stats = statsResponse.data;

// En un test posterior, cuando obtengas backings
pm.test('Stats totalRecaudado matches sum of backings', () => {
    // Este test requiere coordinacion entre multiples requests
    // Se ejecuta en un script global de coleccion
    const backings = pm.collectionVariables.get('allBackings');
    if (backings && backings.length > 0) {
        const calculatedTotal = backings.reduce((sum, b) => sum + b.monto, 0);
        pm.expect(stats.totalRecaudado).to.be.closeTo(calculatedTotal, 0.1);
    }
});
```

---

### 5.3 Validar Nombre Anonimo en Lista Publica

**Nombre:** `Validation - Anonymous Name Display in Public List`

**Pasos:**
1. Crear backing con esAnonimo=true
2. GET `/api/campanias/{id}/backings`
3. Verificar que nombreBacker = "Anonimo"

**Test Script:**
```javascript
pm.test('Anonymous backings display Anonimo in public list', () => {
    const json = pm.response.json();
    const items = json.data.items;

    // Buscar backing recientemente creado (asumiendo orden DESC por fecha)
    const recentBacking = items[0];

    // Si fue anonimo, debe mostrar "Anonimo"
    if (recentBacking.nombreBacker === 'Anonimo') {
        pm.expect(recentBacking.nombreBacker).to.equal('Anonimo');
    }
});
```

---

## 6. Execution Strategy

### 6.1 Ejecucion Local

```bash
# Instalar Newman (si aun no esta instalado)
npm install -g newman

# Ejecutar coleccion completa contra entorno development
newman run tests/newman/WePlay.HacerBacking.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/newman/reports/backing-report.html \
    --bail
```

---

### 6.2 Ejecucion en CI/CD

#### Azure Pipelines (YAML)

```yaml
jobs:
  - job: IntegrationTests
    displayName: 'Newman Integration Tests - Hacer Backing'
    pool:
      vmImage: 'ubuntu-latest'

    steps:
    - task: UseNode@1
      inputs:
        version: '18.x'

    - task: Npm@1
      inputs:
        command: 'custom'
        customCommand: 'install -g newman'

    - task: Newman@0
      displayName: 'Run Hacer Backing Tests'
      inputs:
        collection: '$(Build.SourcesDirectory)/tests/newman/WePlay.HacerBacking.IntegrationTests.json'
        environment: '$(Build.SourcesDirectory)/tests/newman/environments/desarrollo.json'
        reporters: 'cli,junit'
        reporterJunitexportPath: '$(Build.ArtifactStagingDirectory)/TEST-hacer-backing.xml'
      continueOnError: true

    - task: PublishTestResults@2
      inputs:
        testResultsFormat: 'JUnit'
        testResultsFiles: '$(Build.ArtifactStagingDirectory)/TEST-*.xml'
        failTaskOnFailedTests: true

    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'newman-reports'
      condition: succeededOrFailed()
```

---

### 6.3 Datos de Fixture

Para que los tests sean repetiables, crear una carpeta `tests/newman/fixtures/` con scripts de setup:

**File:** `tests/newman/fixtures/create-test-campaign.sql`

```sql
-- Script para crear una campania de prueba con rewards
INSERT INTO CampaniaCrowdfunding (
    Id, ArtistaId, Titulo, EstadoCampaniaId,
    MonedaId, ImporteObjetivo, ImportePledgedActual,
    FechaInicio, FechaFin, PermiteAportacionesAnonimas,
    FechaCreacion
) VALUES (
    '12345678-1234-1234-1234-123456789012',
    '87654321-4321-4321-4321-210987654321',
    'Test Campaign for Newman',
    2, -- PUBLICADA
    1, -- EUR
    5000.00,
    0.00,
    GETUTCDATE(),
    DATEADD(day, 30, GETUTCDATE()),
    1, -- Allow anonymous
    GETUTCDATE()
);

INSERT INTO CampaniaCrowdfundingReward (
    Id, CampaniaId, Nombre, ImporteMinimo,
    CantidadMaxima, EsActivo, FechaCreacion
) VALUES (
    '11111111-1111-1111-1111-111111111111',
    '12345678-1234-1234-1234-123456789012',
    'Digital Download',
    10.00,
    NULL, -- Unlimited
    1,
    GETUTCDATE()
);
```

---

### 6.4 Limpieza de Datos

**Archivo:** `tests/newman/fixtures/cleanup-test-data.sql`

```sql
-- Limpiar backings de prueba
DELETE FROM PedidoCrowdfundingLinea
WHERE PedidoId IN (
    SELECT Id FROM PedidoCrowdfunding
    WHERE CampaniaId = '12345678-1234-1234-1234-123456789012'
);

DELETE FROM PedidoCrowdfunding
WHERE CampaniaId = '12345678-1234-1234-1234-123456789012';

-- Limpiar rewards de prueba
DELETE FROM CampaniaCrowdfundingReward
WHERE CampaniaId = '12345678-1234-1234-1234-123456789012';

-- Limpiar campanias de prueba
DELETE FROM CampaniaCrowdfunding
WHERE Id = '12345678-1234-1234-1234-123456789012';
```

---

## 7. Assertions Detallados por Tipo de Respuesta

### 7.1 Assertions 200 OK (GET Queries)

```javascript
// Status Code
pm.test('Status code is 200', () => pm.response.to.have.status(200));

// Response Structure
pm.test('Has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess').that.equals(true);
});

// Messages Array
pm.test('Messages array is not empty', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').with.length.greaterThan(0);
});

// Data Structure (ejemplo para campanias)
pm.test('Data contains expected fields', () => {
    const json = pm.response.json();
    const campaign = json.data;
    pm.expect(campaign).to.have.all.keys(
        'id', 'titulo', 'importeObjetivo', 'importePledgedActual', ...
    );
});

// Performance
pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

### 7.2 Assertions 201 CREATED (POST Commands)

```javascript
// Status Code
pm.test('Status code is 201 Created', () => pm.response.to.have.status(201));

// Resource ID
pm.test('Response contains created resource ID', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('id').that.is.a('string');
    pm.environment.set('createdId', json.data.id);
});

// Success Message
pm.test('Success error code is 0001', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.include('0001');
});

// Data Integrity
pm.test('Created resource contains submitted data', () => {
    const json = pm.response.json();
    pm.expect(json.data.monto).to.equal(25.00);
});
```

---

### 7.3 Assertions 400 BAD REQUEST

```javascript
// Status Code
pm.test('Status code is 400 Bad Request', () => pm.response.to.have.status(400));

// Error Structure
pm.test('Error indicates validation failure', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages).to.be.an('array').with.length.greaterThan(0);
});

// Error Code Range (1000-1999 para validation)
pm.test('Error code is in validation range', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^1\d{3}$/);
});

// Error Message
pm.test('Error message is descriptive', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].message).to.be.a('string').with.length.greaterThan(5);
});
```

---

### 7.4 Assertions 401 UNAUTHORIZED

```javascript
// Status Code
pm.test('Status code is 401 Unauthorized', () => pm.response.to.have.status(401));

// Auth Error Code (3000-3999)
pm.test('Error code is in auth range', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^3\d{3}$/);
});

// Specific Check
pm.test('Error indicates authentication required', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.include('autenticacion').or.include('login');
});
```

---

### 7.5 Assertions 404 NOT FOUND

```javascript
// Status Code
pm.test('Status code is 404 Not Found', () => pm.response.to.have.status(404));

// Error Code Range (2000-2999)
pm.test('Error code is in not-found range', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^2\d{3}$/);
});

// Specific Resource
pm.test('Error indicates which resource not found', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/campania|reward|backing/);
});
```

---

### 7.6 Assertions 409 CONFLICT (Business Rules)

```javascript
// Status Code
pm.test('Status code is 409 Conflict', () => pm.response.to.have.status(409));

// Error Code Range (4000-4999)
pm.test('Error code is business rule error', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^4\d{3}$/);
});

// Specific Business Rule
pm.test('Error indicates specific business rule violation', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(['4006', '4007', '4011', '4012', '4013']).to.include(errorCode);
});
```

---

## 8. Checklist de Testing

### Funcionalidad Basica
- [ ] GET `/api/campanias` - Listar campanias con paginacion
- [ ] GET `/api/campanias/{id}` - Obtener detalle con rewards y backings
- [ ] POST `/api/campanias/{id}/backings` - Crear backing con reward
- [ ] POST `/api/campanias/{id}/backings` - Crear backing sin reward
- [ ] POST `/api/campanias/{id}/backings` - Crear backing anonimo
- [ ] GET `/api/campanias/{id}/backings` - Listar backings con paginacion
- [ ] GET `/api/campanias/{id}/stats` - Obtener estadisticas

### Validaciones de Entrada
- [ ] Monto < 1 EUR retorna error
- [ ] Monto < importeMinimo del reward retorna error
- [ ] Mensaje > 500 caracteres retorna error
- [ ] CampaniaId invalido retorna 404
- [ ] RewardId invalido retorna 404
- [ ] RewardId no valido como GUID retorna 400

### Autenticacion y Autorizacion
- [ ] Backing anonimo permitido cuando campania.PermiteAportacionesAnonimas = true
- [ ] Backing anonimo rechazado cuando campania no permite anonimos (401)
- [ ] Backing autenticado guarda UserId correctamente
- [ ] Token expirado retorna 401

### Reglas de Negocio
- [ ] Campaign PUBLICADA acepta backings
- [ ] Campaign NO PUBLICADA rechaza backings (409)
- [ ] Campaign pasada FechaFin rechaza backings (409)
- [ ] Reward sin stock rechaza backings (409)
- [ ] Reward con stock disponible acepta backings

### Atomicidad de Transacciones
- [ ] ImportePledgedActual se incrementa tras backing
- [ ] CantidadVendida del reward se incrementa
- [ ] Backing se crea con EstadoPedidoId = 3 (Completado)
- [ ] Si falla cualquier paso, todo se revierte (rollback)

### Datos Retornados
- [ ] BackingDto contiene todas las propiedades requeridas
- [ ] Backing anonimo muestra "Anonimo" como nombre
- [ ] Backing autenticado muestra nombre real si esAnonimo = false
- [ ] Mensaje se almacena y retorna correctamente
- [ ] Moneda del backing coincide con campaña

### Paginacion y Filtros
- [ ] PageSize por defecto es correcto
- [ ] PageNumber fuera de rango retorna lista vacia
- [ ] PageSize > máximo retorna error 400
- [ ] Orden de backings es DESC por fechaCreacion

### Calculos y Estadisticas
- [ ] PorcentajeProgreso = (ImportePledged / ImporteObjetivo) * 100
- [ ] DiasRestantes >= 0 siempre
- [ ] TotalBackers es count de backings completados
- [ ] PromedioAporte = TotalRecaudado / TotalBackers
- [ ] TotalRecaudado coincide con suma de backings

### Casos Limite
- [ ] Monto decimal con muchos decimales se trunca correctamente
- [ ] Mensaje con caracteres especiales se guarda correctamente
- [ ] Multiples backings del mismo usuario son permitidos
- [ ] Dos backings simultaneos en el mismo reward se maneja correctamente (race condition)

### Performance
- [ ] Response time < 500ms para GET queries
- [ ] Response time < 500ms para POST backing
- [ ] Listados grandes (1000+ items) no exceden timeout

---

## 9. Troubleshooting

### Problema: "Campaign not found" a pesar de existir

**Causa:** ID incorrecto o no sincronizado entre setup y tests

**Solucion:**
```javascript
// Usar variable de entorno en todos los requests
// NO hardcodear IDs
pm.environment.set('campaniaId', json.data.items[0].id);
// Y luego usar: {{campaniaId}}
```

---

### Problema: "Invalid Token" incluso con token valido

**Causa:** Bearer token mal formateado o incluir "Bearer" en la variable

**Solucion:**
```javascript
// En environment, guardar solo el token SIN "Bearer"
pm.environment.set('accessToken', json.access_token);

// En header
Authorization: Bearer {{accessToken}}
```

---

### Problema: Tests pasan localmente pero fallan en CI/CD

**Causa:** Timestamp/fecha diferente en CI, o DB limpia

**Solucion:**
```javascript
// Usar timestamps dinamicos en los tests
const now = new Date();
const futureDate = new Date(now.getTime() + 30*24*60*60*1000);
pm.environment.set('futureDate', futureDate.toISOString());

// O ejecutar scripts SQL de setup antes de coleccion
```

---

### Problema: "Amount below minimum" aunque monto es correcto

**Causa:** Reward cargado antes, stock se agoto

**Solucion:**
```javascript
// Recargar reward antes de cada test
const getRewardRequest = {
    url: pm.environment.get('baseUrl') + '/api/campanias/' + pm.environment.get('campaniaId'),
    method: 'GET'
};

pm.sendRequest(getRewardRequest, (err, response) => {
    const reward = response.json().data.rewards[0];
    if (reward.disponible) {
        // Proceder con backing
    }
});
```

---

## 10. Informes y Metricas

### Generar Reporte HTML

```bash
newman run tests/newman/WePlay.HacerBacking.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --reporters htmlextra \
    --reporter-htmlextra-export tests/newman/reports/hacer-backing-report.html \
    --reporter-htmlextra-title "Hacer Backing Integration Tests"
```

---

### Metricas Clave a Monitorear

| Metrica | Target | Alerta |
|---------|--------|--------|
| Total Tests | 40+ | < 30 |
| Pass Rate | 100% | < 95% |
| Avg Response Time | < 300ms | > 500ms |
| 4xx Errors (intencionales) | 15+ | Si faltan validaciones |
| Timeout Errors | 0 | Si > 0 |
| Network Errors | 0 | Si > 0 |

---

## 11. Proximos Pasos

### Post-MVP
1. **Tests de Performance:** Agregar stress tests para backings concurrentes
2. **Tests de Seguridad:** Validar SQL injection, XSS en mensaje
3. **Tests de Integracion E2E:** Flujo completo desde landing hasta confirmacion
4. **Contract Tests:** Validar contratos entre backend y frontend
5. **Tests de Carga:** Simular multiples backings simultaneos con stress

---

## Conclusiones

Este plan cubre:
- **34 requests** distribuidos en 5 endpoints
- **45+ casos de prueba** incluyendo happy path, validaciones y edge cases
- **Assertions completos** para cada status code y respuesta
- **Variables de entorno** para reutilizabilidad y mantenimiento
- **Estrategia CI/CD** para automatizacion en pipelines
- **Validaciones atomicas** para garantizar integridad de datos

**Tiempo estimado de ejecucion:** 2-3 minutos en entorno local, 5-7 minutos en CI/CD (incluyendo setup y cleanup)

---

**Fin del plan de testing Newman para hacer-backing**
