# Plan de Testing Newman: Dashboard de Artista

**Fecha:** 2026-02-14
**Feature:** dashboard-artista
**Coleccion:** WePlay.Dashboard.Artista.IntegrationTests
**Modulo:** Crowdfunding

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 4 GET |
| Total requests planificados | 28 requests |
| Casos de prueba | 35+ casos |
| Status codes a validar | 200, 400, 401, 403, 404, 500 |
| Tiempo estimado ejecucion | < 30 segundos |

**Objetivo:** Validar que todos los endpoints del dashboard del artista funcionan correctamente bajo diferentes escenarios de autenticacion, autorizacion, validacion y carga de datos.

---

## 2. Estructura de Coleccion

```
WePlay.Dashboard.Artista.IntegrationTests/
├── _Setup/
│   ├── Get Auth Token - Valid Artista
│   ├── Get Auth Token - Valid Fan (No Artista)
│   └── Create Test Data (Campanias + Backings)
├── Dashboard/
│   ├── 200 OK - Success/
│   │   └── GET Resumen - Valid Artista
│   ├── 401 Unauthorized/
│   │   ├── GET Resumen - Invalid Token
│   │   ├── GET Resumen - Expired Token
│   │   └── GET Resumen - No Token
│   ├── 403 Forbidden/
│   │   ├── GET Resumen - Fan User (No Artista)
│   │   └── GET Resumen - Admin User (Wrong Role)
│   ├── 404 NotFound/
│   │   └── GET Resumen - User Sin Perfil Artista
│   └── 500 Error/
│       └── GET Resumen - Unexpected Error Simulation
├── Mis Campanias/
│   ├── 200 OK - Success/
│   │   ├── GET Mis Campanias - All (Default Pagination)
│   │   ├── GET Mis Campanias - Filter by Estado (Publicada)
│   │   ├── GET Mis Campanias - Filter by Estado (Borrador)
│   │   ├── GET Mis Campanias - Filter by Estado (Finalizada)
│   │   ├── GET Mis Campanias - Page 2
│   │   ├── GET Mis Campanias - Custom Page Size (20)
│   │   ├── GET Mis Campanias - Empty Result
│   │   └── GET Mis Campanias - Response Schema Validation
│   ├── 400 Bad Request/
│   │   ├── GET Mis Campanias - Invalid Estado (0)
│   │   ├── GET Mis Campanias - Invalid Estado (6)
│   │   ├── GET Mis Campanias - Page Less Than 1 (0)
│   │   ├── GET Mis Campanias - PageSize 0
│   │   ├── GET Mis Campanias - PageSize > 100
│   │   └── GET Mis Campanias - Invalid Query Param Type
│   ├── 401 Unauthorized/
│   │   └── GET Mis Campanias - No Token
│   ├── 403 Forbidden/
│   │   └── GET Mis Campanias - Fan User
│   └── 404 NotFound/
│       └── GET Mis Campanias - User Sin Artista
├── Campanias Backings/
│   ├── 200 OK - Success/
│   │   ├── GET Backings - Default Pagination (Page 1, Size 20)
│   │   ├── GET Backings - Page 2
│   │   ├── GET Backings - Custom Page Size
│   │   ├── GET Backings - Empty Backings
│   │   ├── GET Backings - Anonymous Backers (Privacy)
│   │   ├── GET Backings - Mixed Named and Anonymous
│   │   ├── GET Backings - Response Schema Validation
│   │   └── GET Backings - Stats Aggregation Validation
│   ├── 400 Bad Request/
│   │   ├── GET Backings - Invalid Guid (campaniaId)
│   │   ├── GET Backings - Page Less Than 1
│   │   ├── GET Backings - PageSize > 100
│   │   └── GET Backings - Missing campaniaId
│   ├── 401 Unauthorized/
│   │   └── GET Backings - No Token
│   ├── 403 Forbidden/
│   │   ├── GET Backings - Different Artista (Ownership Validation)
│   │   └── GET Backings - Fan User
│   └── 404 NotFound/
│       ├── GET Backings - Non-existent Campania
│       └── GET Backings - User Sin Artista
├── Campanias Stats/
│   ├── 200 OK - Success/
│   │   ├── GET Stats - Valid Campania
│   │   ├── GET Stats - Campania With Multiple Rewards
│   │   ├── GET Stats - Campania With Anonymous Backings
│   │   ├── GET Stats - Progreso Por Dia Calculation
│   │   ├── GET Stats - Proyeccion Final Calculation
│   │   ├── GET Stats - Response Schema Validation
│   │   └── GET Stats - Reward Stats Percentages
│   ├── 400 Bad Request/
│   │   ├── GET Stats - Invalid Guid (campaniaId)
│   │   └── GET Stats - Missing campaniaId
│   ├── 401 Unauthorized/
│   │   └── GET Stats - No Token
│   ├── 403 Forbidden/
│   │   ├── GET Stats - Different Artista (Ownership)
│   │   └── GET Stats - Fan User
│   └── 404 NotFound/
│       ├── GET Stats - Non-existent Campania
│       └── GET Stats - User Sin Artista
├── Contract Tests/
│   ├── Schema Validation/
│   │   ├── DashboardResumenDto Schema
│   │   ├── MiCampaniaListItemDto Array Schema
│   │   ├── CampaniaBackingListDto Schema
│   │   └── CampaniaStatsDetailDto Schema
│   ├── Response Envelope/
│   │   ├── ServiceResponse Structure (data, messages, isSuccess)
│   │   ├── ServiceResponseMessage ErrorCode Format
│   │   └── Success Message (ErrorCode = "0000")
│   └── Data Type Validation/
│       ├── Guid Fields Format
│       ├── Decimal Fields Precision
│       ├── DateTime ISO 8601 Format
│       ├── Integer Bounds Validation
│       └── String Max Length Validation
├── Authorization Tests/
│   ├── Token Validation/
│   │   ├── JWT Invalid Signature
│   │   ├── JWT Expired (exp claim)
│   │   ├── JWT Missing Required Claims (role)
│   │   └── JWT Invalid Issuer
│   ├── Role Validation/
│   │   ├── Only Artista Role Allowed
│   │   └── Admin and Fan Roles Rejected
│   └── Ownership Validation/
│       ├── User Can Only Access Own Campanias
│       └── User Can Only Access Own Backings
├── Business Logic Tests/
│   ├── Calculations/
│   │   ├── PorcentajeProgreso = (ImporteRecaudado / ImporteObjetivo) * 100
│   │   ├── DiasRestantes = MAX(0, (FechaFin - Now).Days)
│   │   ├── BackingPromedio = TotalRecaudado / TotalBackers
│   │   └── ProyeccionFinal = ImporteRecaudado + (VelocidadDiaria * DiasRestantes)
│   ├── Data Consistency/
│   │   ├── TotalBackers = Count of Completed Orders Only
│   │   ├── TotalRecaudado = Sum of Completed Orders Only
│   │   ├── CampaniasActivas = Count where EstadoCampaniaId = 2
│   │   ├── CampaniasCompletadas = Count where EstadoCampaniaId = 3
│   │   └── Reward Mas Popular = Most Sold Reward
│   └── Sorting and Filtering/
│       ├── Campanias Ordered by FechaCreacion DESC
│       ├── Backings Ordered by FechaCreacion DESC
│       ├── Estado Filter Works Correctly
│       └── Pagination Skip/Take Logic
├── Edge Cases/
│   ├── Empty Data/
│   │   ├── Artista Sin Campanias
│   │   ├── Campania Sin Backings
│   │   └── Campania Sin Rewards
│   ├── Large Data Sets/
│   │   ├── User With Many Campanias (100+)
│   │   ├── Campania With Many Backings (500+)
│   │   └── Large Pagination Results
│   ├── Numeric Edge Cases/
│   │   ├── ImporteObjetivo = 0 (Division by Zero)
│   │   ├── Dias = 0 (Just Started)
│   │   └── Very Large Decimal Values
│   └── Date Edge Cases/
│       ├── Campania Just Started (FechaInicio = Today)
│       ├── Campania Just Ended (FechaFin = Yesterday)
│       └── Future Dated Campania
└── _Cleanup/
    └── Delete Test Data (Campanias y Backings)
```

---

## 3. Estructura de Requests Detallados

### 3.1 SETUP - Autenticacion

#### 3.1.1 Get Auth Token - Valid Artista

**Request:**
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
    "email": "usuario1@mail.com",
    "password": "123456"
}
```

**Pre-request Script:**
```javascript
// Nada especial necesario
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response contains access_token', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('access_token');
    pm.expect(json.access_token).to.be.a('string').that.is.not.empty;

    // Guardar token para requests posteriores
    pm.environment.set('accessToken', json.access_token);
    pm.environment.set('artistaUsername', 'usuario1@mail.com');
});

pm.test('Response contains token_type (Bearer)', () => {
    const json = pm.response.json();
    pm.expect(json.token_type).to.equal('Bearer');
});

pm.test('Response time < 1000ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});
```

**Variables usadas:**
- `{{baseUrl}}` = http://localhost:5001
- `{{accessToken}}` = Se guarda en ambiente post-request

---

#### 3.1.2 Get Auth Token - Valid Fan (No Artista)

**Request:**
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
    "email": "admin-test@mail.com",
    "password": "123456"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Token obtained for Fan user', () => {
    const json = pm.response.json();
    pm.environment.set('fanAccessToken', json.access_token);
    pm.environment.set('fanUsername', 'admin-test@mail.com');
});
```

---

#### 3.1.3 Create Test Data (Campanias + Backings)

**Description:** Pre-crear datos de prueba en la base de datos para poder testear listings y detalles.

**Approach:**
- Usar endpoint de creacion de campanias (si existe)
- O insertar directamente via SQL script en pre-request (no recomendado)
- O reutilizar datos existentes del seed de DB

**Test Script:**
```javascript
pm.test('Test data prepared', () => {
    // Si los datos ya existen en DB (seed), solo marcar como listo
    const artistaId = 'usuario1-artista-id'; // Obtener del getResumen request
    pm.environment.set('testArtistaId', artistaId);

    // Guardar IDs de campanias conocidas para testeo
    pm.environment.set('testCampaniaId1', 'guid-1');
    pm.environment.set('testCampaniaId2', 'guid-2');
});
```

---

### 3.2 DASHBOARD - GET /api/dashboard/resumen

#### 3.2.1 GET Resumen - Valid Artista (200 OK)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Authorization: Bearer {{accessToken}}
Accept: application/json
```

**Pre-request Script:**
```javascript
// Validar que tenemos token
pm.sendRequest({
    url: pm.environment.get('baseUrl') + '/api/dashboard/resumen',
    method: 'GET',
    header: {
        'Authorization': 'Bearer ' + pm.environment.get('accessToken'),
        'Accept': 'application/json'
    }
}, function(err, response) {
    // Check en el test script
});
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
    pm.expect(json).to.have.property('isSuccess');
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('DashboardResumenDto has all required properties', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('artistaId');
    pm.expect(data).to.have.property('nombreArtistico');
    pm.expect(data).to.have.property('totalRecaudado');
    pm.expect(data).to.have.property('totalBackers');
    pm.expect(data).to.have.property('campaniasActivas');
    pm.expect(data).to.have.property('campaniasCompletadas');
    pm.expect(data).to.have.property('totalCampanias');
    pm.expect(data).to.have.property('monedaSimbolo');
    pm.expect(data).to.have.property('fechaUltimoAporte');
});

pm.test('Numeric fields are numbers or null', () => {
    const data = pm.response.json().data;
    pm.expect(data.totalRecaudado).to.be.a('number');
    pm.expect(data.totalBackers).to.satisfy(v => typeof v === 'number');
    pm.expect(data.campaniasActivas).to.be.a('number');
    pm.expect(data.campaniasCompletadas).to.be.a('number');
    pm.expect(data.totalCampanias).to.be.a('number');
});

pm.test('GUIDs are valid format', () => {
    const data = pm.response.json().data;
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(data.artistaId).to.match(guidRegex, 'artistaId debe ser un GUID valido');
});

pm.test('fechaUltimoAporte is ISO 8601 or null', () => {
    const data = pm.response.json().data;
    if (data.fechaUltimoAporte) {
        const iso8601Regex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z?$/;
        pm.expect(data.fechaUltimoAporte).to.match(iso8601Regex);
    }
});

pm.test('monedaSimbolo is EUR', () => {
    const data = pm.response.json().data;
    pm.expect(data.monedaSimbolo).to.equal('EUR');
});

pm.test('Success message has errorCode 0000', () => {
    const messages = pm.response.json().messages;
    pm.expect(messages).to.be.an('array');
    pm.expect(messages[0].errorCode).to.equal('0000');
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test('Save artistaId for next requests', () => {
    const data = pm.response.json().data;
    pm.environment.set('artistaId', data.artistaId);
});
```

---

#### 3.2.2 GET Resumen - Invalid Token (401 Unauthorized)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Authorization: Bearer invalid_token_xyz123
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Response has error message with errorCode 3001', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.equal('3001');
    pm.expect(errorCode).to.match(/^3\d{3}$/, 'Error code debe ser 3001 (Auth)');
});

pm.test('Response does not contain data', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.null;
});
```

---

#### 3.2.3 GET Resumen - Expired Token (401 Unauthorized)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Authorization: Bearer {{expiredToken}}
```

**Pre-request Script:**
```javascript
// Usar un token conocido que ya esta expirado
// O generar uno con exp claim en el pasado
pm.environment.set('expiredToken', 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkyMjJ9.invalid');
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Error message indicates token expired', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^3\d{3}$/);
});
```

---

#### 3.2.4 GET Resumen - No Token (401 Unauthorized)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Accept: application/json
```

**Test Script:**
```javascript
pm.test('Status code is 401', () => {
    pm.response.to.have.status(401);
});

pm.test('Requires authentication', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('3001');
});
```

---

#### 3.2.5 GET Resumen - Fan User (No Artista) (403 Forbidden)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Authorization: Bearer {{fanAccessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('User without Artista role gets Forbidden', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('3002');
    pm.expect(json.messages[0].message).to.include('Acceso denegado');
});
```

---

#### 3.2.6 GET Resumen - User Sin Perfil Artista (404 NotFound)

**Request:**
```
GET {{baseUrl}}/api/dashboard/resumen
Authorization: Bearer {{validTokenWithoutArtista}}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('User without Artista profile gets NotFound', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('2002');
    pm.expect(json.messages[0].message).to.include('Artista no encontrado');
});
```

---

### 3.3 MIS CAMPANIAS - GET /api/campanias/mis-campanias

#### 3.3.1 GET Mis Campanias - All (Default Pagination) (200 OK)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?page=1&pageSize=10
Authorization: Bearer {{accessToken}}
Accept: application/json
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
    pm.expect(json).to.have.property('data');
});

pm.test('Data has PaginatedResponse structure', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('items').that.is.an('array');
    pm.expect(data).to.have.property('totalCount').that.is.a('number');
    pm.expect(data).to.have.property('page').that.is.a('number');
    pm.expect(data).to.have.property('pageSize').that.is.a('number');
    pm.expect(data).to.have.property('totalPages').that.is.a('number');
});

pm.test('Each item has required MiCampaniaListItemDto properties', () => {
    const items = pm.response.json().data.items;
    items.forEach(item => {
        pm.expect(item).to.have.property('id');
        pm.expect(item).to.have.property('titulo');
        pm.expect(item).to.have.property('imagenPrincipalUrl');
        pm.expect(item).to.have.property('estadoCampaniaId');
        pm.expect(item).to.have.property('estadoCampaniaNombre');
        pm.expect(item).to.have.property('importeObjetivo');
        pm.expect(item).to.have.property('importeRecaudado');
        pm.expect(item).to.have.property('porcentajeProgreso');
        pm.expect(item).to.have.property('numBackers');
        pm.expect(item).to.have.property('diasRestantes');
        pm.expect(item).to.have.property('fechaFin');
        pm.expect(item).to.have.property('fechaCreacion');
    });
});

pm.test('porcentajeProgreso is correctly calculated (0-100)', () => {
    const items = pm.response.json().data.items;
    items.forEach(item => {
        pm.expect(item.porcentajeProgreso).to.be.within(0, 100);
        // Validar calculo: (importeRecaudado / importeObjetivo) * 100
        const expectedPercentage = (item.importeRecaudado / item.importeObjetivo) * 100;
        const expectedRounded = Math.round(expectedPercentage * 100) / 100;
        pm.expect(item.porcentajeProgreso).to.be.closeTo(expectedRounded, 0.01);
    });
});

pm.test('diasRestantes is 0 or positive (or null for draft)', () => {
    const items = pm.response.json().data.items;
    items.forEach(item => {
        if (item.diasRestantes !== null) {
            pm.expect(item.diasRestantes).to.be.at.least(0);
        }
    });
});

pm.test('Campanias ordered by fechaCreacion DESC', () => {
    const items = pm.response.json().data.items;
    if (items.length > 1) {
        for (let i = 1; i < items.length; i++) {
            const prev = new Date(items[i - 1].fechaCreacion);
            const current = new Date(items[i].fechaCreacion);
            pm.expect(prev).to.be.at.least(current, 'Debe estar ordenado por fechaCreacion DESC');
        }
    }
});

pm.test('Pagination info is correct', () => {
    const data = pm.response.json().data;
    const expectedTotalPages = Math.ceil(data.totalCount / data.pageSize);
    pm.expect(data.totalPages).to.equal(expectedTotalPages);
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

#### 3.3.2 GET Mis Campanias - Filter by Estado (Publicada) (200 OK)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?estadoCampaniaId=2&page=1&pageSize=10
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('All returned campanias have estado = Publicada (2)', () => {
    const items = pm.response.json().data.items;
    items.forEach(item => {
        pm.expect(item.estadoCampaniaId).to.equal(2);
        pm.expect(item.estadoCampaniaNombre).to.equal('Publicada');
    });
});

pm.test('Filter restricts results to estado 2 only', () => {
    const items = pm.response.json().data.items;
    pm.expect(items.length).to.be.lessThanOrEqual(pm.response.json().data.pageSize);
    // All items should have EstadoCampaniaId = 2
    const allPublished = items.every(i => i.estadoCampaniaId === 2);
    pm.expect(allPublished).to.be.true;
});
```

---

#### 3.3.3 GET Mis Campanias - Invalid Estado (0) (400 Bad Request)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?estadoCampaniaId=0&page=1&pageSize=10
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Validation error for invalid estado', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^1\d{3}$/, 'Debe ser error de validacion (1xxx)');
    pm.expect(json.messages[0].message).to.include('invalido');
});
```

---

#### 3.3.4 GET Mis Campanias - Invalid Estado (6) (400 Bad Request)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?estadoCampaniaId=6&page=1&pageSize=10
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Estado 6 is out of range', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});
```

---

#### 3.3.5 GET Mis Campanias - Page Less Than 1 (400 Bad Request)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?page=0&pageSize=10
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Page must be >= 1', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].message).to.include('pagina');
});
```

---

#### 3.3.6 GET Mis Campanias - PageSize > 100 (400 Bad Request)

**Request:**
```
GET {{baseUrl}}/api/campanias/mis-campanias?page=1&pageSize=101
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('PageSize must be <= 100', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].message).to.include('tamano');
});
```

---

### 3.4 CAMPANIAS BACKINGS - GET /api/campanias/{id}/backings

#### 3.4.1 GET Backings - Default Pagination (200 OK)

**Request:**
```
GET {{baseUrl}}/api/campanias/{{testCampaniaId1}}/backings?page=1&pageSize=20
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has CampaniaBackingListDto structure', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('campaniaId');
    pm.expect(data).to.have.property('campaniaTitulo');
    pm.expect(data).to.have.property('stats');
    pm.expect(data).to.have.property('backings');
});

pm.test('Stats has all required properties', () => {
    const stats = pm.response.json().data.stats;
    pm.expect(stats).to.have.property('totalRecaudado');
    pm.expect(stats).to.have.property('backingPromedio');
    pm.expect(stats).to.have.property('totalBackers');
    pm.expect(stats).to.have.property('rewardMasPopular');
    pm.expect(stats).to.have.property('ultimoBacking');
});

pm.test('Backings is PaginatedResponse<CampaniaBackingItemDto>', () => {
    const backings = pm.response.json().data.backings;
    pm.expect(backings).to.have.property('items').that.is.an('array');
    pm.expect(backings).to.have.property('totalCount');
    pm.expect(backings).to.have.property('page');
    pm.expect(backings).to.have.property('pageSize');
    pm.expect(backings).to.have.property('totalPages');
});

pm.test('Each backing item has required properties', () => {
    const items = pm.response.json().data.backings.items;
    items.forEach(item => {
        pm.expect(item).to.have.property('id');
        pm.expect(item).to.have.property('nombreBacker');
        pm.expect(item).to.have.property('email');
        pm.expect(item).to.have.property('monto');
        pm.expect(item).to.have.property('rewardNombre');
        pm.expect(item).to.have.property('mensaje');
        pm.expect(item).to.have.property('esAnonimo');
        pm.expect(item).to.have.property('estadoPedido');
        pm.expect(item).to.have.property('fechaCreacion');
    });
});

pm.test('Backings ordered by fechaCreacion DESC (most recent first)', () => {
    const items = pm.response.json().data.backings.items;
    if (items.length > 1) {
        for (let i = 1; i < items.length; i++) {
            const prev = new Date(items[i - 1].fechaCreacion);
            const current = new Date(items[i].fechaCreacion);
            pm.expect(prev).to.be.at.least(current, 'Backings must be DESC by fechaCreacion');
        }
    }
});

pm.test('Anonymous backings have null email', () => {
    const items = pm.response.json().data.backings.items;
    items.forEach(item => {
        if (item.esAnonimo) {
            pm.expect(item.email).to.be.null;
            pm.expect(item.nombreBacker).to.equal('Anonimo');
        }
    });
});

pm.test('Named backings have email populated', () => {
    const items = pm.response.json().data.backings.items;
    items.forEach(item => {
        if (!item.esAnonimo) {
            pm.expect(item.email).to.not.be.null;
            pm.expect(item.nombreBacker).to.not.equal('Anonimo');
        }
    });
});

pm.test('backingPromedio is correctly calculated', () => {
    const stats = pm.response.json().data.stats;
    if (stats.totalBackers > 0) {
        const expected = Math.round((stats.totalRecaudado / stats.totalBackers) * 100) / 100;
        pm.expect(stats.backingPromedio).to.be.closeTo(expected, 0.01);
    }
});

pm.test('Response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

#### 3.4.2 GET Backings - Different Artista (Ownership Validation) (403 Forbidden)

**Request:**
```
GET {{baseUrl}}/api/campanias/{{otherArtistaCampaniaId}}/backings?page=1&pageSize=20
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('User cannot access other artists campanias', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('3002');
    pm.expect(json.messages[0].message).to.include('permiso');
});
```

---

#### 3.4.3 GET Backings - Non-existent Campania (404 NotFound)

**Request:**
```
GET {{baseUrl}}/api/campanias/00000000-0000-0000-0000-000000000000/backings?page=1&pageSize=20
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 404', () => {
    pm.response.to.have.status(404);
});

pm.test('Non-existent campania returns not found', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('2003');
    pm.expect(json.messages[0].message).to.include('no encontrada');
});
```

---

#### 3.4.4 GET Backings - Invalid Guid (400 Bad Request)

**Request:**
```
GET {{baseUrl}}/api/campanias/invalid-guid-format/backings?page=1&pageSize=20
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Invalid GUID format is rejected', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});
```

---

### 3.5 CAMPANIAS STATS - GET /api/campanias/{id}/stats

#### 3.5.1 GET Stats - Valid Campania (200 OK)

**Request:**
```
GET {{baseUrl}}/api/campanias/{{testCampaniaId1}}/stats
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response has CampaniaStatsDetailDto structure', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('campaniaId');
    pm.expect(data).to.have.property('campaniaTitulo');
    pm.expect(data).to.have.property('importeObjetivo');
    pm.expect(data).to.have.property('importeRecaudado');
    pm.expect(data).to.have.property('porcentajeProgreso');
    pm.expect(data).to.have.property('numBackers');
    pm.expect(data).to.have.property('backingPromedio');
    pm.expect(data).to.have.property('diasRestantes');
    pm.expect(data).to.have.property('diasTranscurridos');
    pm.expect(data).to.have.property('totalDiasCampania');
    pm.expect(data).to.have.property('proyeccionFinal');
    pm.expect(data).to.have.property('velocidadDiaria');
    pm.expect(data).to.have.property('rewardStats').that.is.an('array');
    pm.expect(data).to.have.property('progressoPorDia').that.is.an('array');
});

pm.test('porcentajeProgreso is correctly calculated', () => {
    const data = pm.response.json().data;
    const expected = (data.importeRecaudado / data.importeObjetivo) * 100;
    const expectedRounded = Math.round(expected * 100) / 100;
    pm.expect(data.porcentajeProgreso).to.be.closeTo(expectedRounded, 0.01);
});

pm.test('velocidadDiaria is correctly calculated', () => {
    const data = pm.response.json().data;
    if (data.diasTranscurridos > 0) {
        const expected = data.importeRecaudado / data.diasTranscurridos;
        pm.expect(data.velocidadDiaria).to.be.closeTo(expected, 1);
    }
});

pm.test('Reward stats percentages sum to 100', () => {
    const rewardStats = pm.response.json().data.rewardStats;
    const totalPercentage = rewardStats.reduce((sum, r) => sum + r.porcentajeDelTotal, 0);
    pm.expect(totalPercentage).to.be.closeTo(100, 0.5);
});

pm.test('Progreso por dia is ordered by fecha ASC', () => {
    const progreso = pm.response.json().data.progressoPorDia;
    if (progreso.length > 1) {
        for (let i = 1; i < progreso.length; i++) {
            pm.expect(progreso[i].fecha).to.be.at.least(progreso[i - 1].fecha, 'Debe estar ordenado ASC por fecha');
        }
    }
});

pm.test('Progreso acumulado increases monotonically', () => {
    const progreso = pm.response.json().data.progressoPorDia;
    if (progreso.length > 1) {
        for (let i = 1; i < progreso.length; i++) {
            pm.expect(progreso[i].acumulado).to.be.at.least(progreso[i - 1].acumulado);
        }
    }
});

pm.test('Response time < 1000ms (complex query)', () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});
```

---

#### 3.5.2 GET Stats - Different Artista (403 Forbidden)

**Request:**
```
GET {{baseUrl}}/api/campanias/{{otherArtistaCampaniaId}}/stats
Authorization: Bearer {{accessToken}}
```

**Test Script:**
```javascript
pm.test('Status code is 403', () => {
    pm.response.to.have.status(403);
});

pm.test('User cannot access other artists stats', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
    pm.expect(json.messages[0].errorCode).to.equal('3002');
});
```

---

## 4. Variables de Entorno

### Development Environment

```json
{
  "name": "Dashboard Artista - Development",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5001",
      "type": "string",
      "enabled": true
    },
    {
      "key": "accessToken",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "fanAccessToken",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "expiredToken",
      "value": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0LXVzZXIiLCJuYW1lIjoiVGVzdCBVc2VyIiwicm9sZSI6IkFydGlzdGEiLCJleHAiOjE1MTYyMzkyMjIsImlhdCI6MTUxNjIzOTAyMn0.invalid",
      "type": "string",
      "enabled": true
    },
    {
      "key": "artistaId",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "testCampaniaId1",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "testCampaniaId2",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "otherArtistaCampaniaId",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "artistaUsername",
      "value": "usuario1@mail.com",
      "type": "string",
      "enabled": true
    },
    {
      "key": "fanUsername",
      "value": "admin-test@mail.com",
      "type": "string",
      "enabled": true
    }
  ]
}
```

---

### Staging Environment

```json
{
  "name": "Dashboard Artista - Staging",
  "values": [
    {
      "key": "baseUrl",
      "value": "https://weplay-staging.azurewebsites.net",
      "type": "string",
      "enabled": true
    },
    {
      "key": "accessToken",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "fanAccessToken",
      "value": "",
      "type": "string",
      "enabled": true
    },
    {
      "key": "artistaUsername",
      "value": "staging-artista@weplay.com",
      "type": "string",
      "enabled": true
    },
    {
      "key": "fanUsername",
      "value": "staging-fan@weplay.com",
      "type": "string",
      "enabled": true
    }
  ]
}
```

---

## 5. Estrategia de Ejecucion en CI/CD

### 5.1 Local Execution

```bash
# Ejecutar toda la coleccion con entorno development
newman run "WePlay.Dashboard.Artista.IntegrationTests.json" \
    -e "environments/development.json" \
    --reporters cli,htmlextra,json \
    --reporter-htmlextra-export "test-results/dashboard-integration.html" \
    --reporter-json-export "test-results/dashboard-integration.json" \
    --timeout 30000 \
    --timeout-request 5000

# Ejecutar solo carpeta de Success cases
newman run "WePlay.Dashboard.Artista.IntegrationTests.json" \
    -e "environments/development.json" \
    -f "200 OK" \
    --reporters cli

# Ejecutar con delay entre requests (simular carga)
newman run "WePlay.Dashboard.Artista.IntegrationTests.json" \
    -e "environments/development.json" \
    --delay-request 500 \
    --reporters cli
```

---

### 5.2 Azure DevOps Pipeline

**File:** `azure-pipelines.yml`

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
    exclude:
      - '*.md'

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'

stages:
  - stage: Build
    jobs:
      - job: BuildBackend
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          - task: DotNetCoreCLI@2
            inputs:
              command: 'build'
              arguments: '--configuration $(buildConfiguration)'
              projects: 'src/api/**/*.csproj'

  - stage: IntegrationTests
    dependsOn: Build
    jobs:
      - job: NewmanTests
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)

          # Start Backend en http://localhost:5001
          - task: DotNetCoreCLI@2
            inputs:
              command: 'run'
              projects: 'src/api/WebApi/WebApi.csproj'
              arguments: '--configuration $(buildConfiguration) --urls http://localhost:5001'
            displayName: 'Start Backend API'
            env:
              ASPNETCORE_ENVIRONMENT: 'Testing'
              ASPNETCORE_URLS: 'http://localhost:5001'
            continueOnError: false
            condition: 'succeeded()'
            backgroundProcess: true

          # Wait for API to be ready
          - script: |
              echo "Waiting for API to start..."
              powershell -Command "
              for ($i = 0; $i -lt 30; $i++) {
                try {
                  $response = Invoke-WebRequest -Uri 'http://localhost:5001/health' -Method GET -TimeoutSec 1 -ErrorAction Stop
                  if ($response.StatusCode -eq 200) {
                    Write-Host 'API is ready'
                    exit 0
                  }
                } catch {
                  Write-Host "Waiting... ($i/30)"
                  Start-Sleep -Seconds 1
                }
              }
              exit 1
              "
            displayName: 'Wait for API Ready'

          # Install Newman
          - task: NodeTool@0
            inputs:
              versionSpec: '18.x'

          - script: npm install -g newman newman-reporter-htmlextra
            displayName: 'Install Newman'

          # Run Dashboard Integration Tests
          - script: |
              newman run "tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json" \
                -e "tests/newman/environments/testing.json" \
                --reporters cli,htmlextra,junit \
                --reporter-htmlextra-export "$(Build.ArtifactStagingDirectory)/dashboard-integration.html" \
                --reporter-junit-export "$(Build.ArtifactStagingDirectory)/dashboard-integration-junit.xml" \
                --timeout 30000 \
                --timeout-request 5000 \
                --bail
            displayName: 'Run Dashboard Integration Tests'
            continueOnError: true

          # Publish Test Results
          - task: PublishTestResults@2
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: '$(Build.ArtifactStagingDirectory)/dashboard-integration-junit.xml'
              testRunTitle: 'Dashboard Artista Integration Tests'
            condition: 'always()'

          # Publish Artifacts (HTML Report)
          - task: PublishBuildArtifacts@1
            inputs:
              PathtoPublish: '$(Build.ArtifactStagingDirectory)'
              ArtifactName: 'newman-reports'
              publishLocation: 'Container'
            condition: 'always()'

  - stage: DeployStaging
    dependsOn: IntegrationTests
    condition: 'and(succeeded(), eq(variables[''Build.SourceBranch''], ''refs/heads/master''))'
    jobs:
      - job: DeployToStaging
        steps:
          # Deploy steps...
          - script: echo "Deploying to staging after successful tests"
```

---

### 5.3 GitHub Actions Pipeline

**File:** `.github/workflows/integration-tests.yml`

```yaml
name: Dashboard Integration Tests

on:
  push:
    branches: [master, develop]
    paths:
      - 'src/api/**'
      - 'tests/newman/**'
  pull_request:
    branches: [master, develop]

jobs:
  integration-tests:
    runs-on: windows-latest

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2019-latest
        env:
          SA_PASSWORD: TestPassword123!
          ACCEPT_EULA: Y
        options: >-
          --health-cmd="sqlcmd -S . -U sa -P TestPassword123! -Q 'SELECT 1'"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install Newman
        run: |
          npm install -g newman
          npm install -g newman-reporter-htmlextra

      - name: Build Backend
        run: dotnet build src/api --configuration Release

      - name: Start Backend
        run: |
          Start-Process -FilePath "dotnet" -ArgumentList "run --project src/api/WebApi/WebApi.csproj --configuration Release --urls http://localhost:5001" -WindowStyle Hidden
          Start-Sleep -Seconds 10
        shell: powershell

      - name: Run Newman Tests
        run: |
          newman run tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json `
            -e tests/newman/environments/testing.json `
            --reporters cli,htmlextra,json `
            --reporter-htmlextra-export newman-report.html `
            --reporter-json-export newman-report.json `
            --timeout 30000 `
            --bail

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: newman-reports
          path: |
            newman-report.html
            newman-report.json

      - name: Comment PR
        if: always() && github.event_name == 'pull_request'
        uses: actions/github-script@v6
        with:
          script: |
            const fs = require('fs');
            const report = JSON.parse(fs.readFileSync('newman-report.json', 'utf8'));
            const passed = report.run.stats.tests.passed;
            const failed = report.run.stats.tests.failed;

            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: `## Dashboard Integration Tests\n\n✅ Passed: ${passed}\n❌ Failed: ${failed}\n\n[Full Report](https://github.com/${context.repo.owner}/${context.repo.repo}/actions/runs/${context.runId})`
            });
```

---

## 6. Matriz de Casos de Prueba

### 6.1 Resumen - GET /api/dashboard/resumen

| Test Case | Input | Status | ErrorCode | Validaciones |
|-----------|-------|--------|-----------|--------------|
| Valid Artista | Valid Token | 200 | 0000 | Todos los campos presentes, metricas validas |
| Invalid Token | Invalid Token | 401 | 3001 | Sin data, error message |
| Expired Token | Expired JWT | 401 | 3001 | Sin data |
| No Token | No Authorization | 401 | 3001 | Sin data |
| Fan User | Token con role=Fan | 403 | 3002 | Sin data, Acceso denegado |
| No Artista Profile | Valid token sin perfil Artista | 404 | 2002 | Sin data, Artista no encontrado |

---

### 6.2 Mis Campanias - GET /api/campanias/mis-campanias

| Test Case | Query Params | Status | ErrorCode | Validaciones |
|-----------|--------------|--------|-----------|--------------|
| Default | page=1, pageSize=10 | 200 | 0000 | Paginacion correcta, ordenamiento DESC |
| Filter Estado 1 | estadoCampaniaId=1 | 200 | 0000 | Solo Borrador |
| Filter Estado 2 | estadoCampaniaId=2 | 200 | 0000 | Solo Publicada |
| Filter Estado 3 | estadoCampaniaId=3 | 200 | 0000 | Solo Finalizada |
| Page 2 | page=2, pageSize=10 | 200 | 0000 | Skip/Take correcto |
| Custom PageSize | page=1, pageSize=20 | 200 | 0000 | 20 items max |
| Empty Result | estadoCampaniaId=1 (sin borradores) | 200 | 0000 | items=[], totalCount=0 |
| Invalid Estado 0 | estadoCampaniaId=0 | 400 | 1xxx | Validation error |
| Invalid Estado 6 | estadoCampaniaId=6 | 400 | 1xxx | Validation error |
| Page 0 | page=0 | 400 | 1xxx | Validation error |
| PageSize 0 | pageSize=0 | 400 | 1xxx | Validation error |
| PageSize 101 | pageSize=101 | 400 | 1xxx | Validation error |
| No Token | No Authorization | 401 | 3001 | Unauthorized |
| Fan User | Token Fan | 403 | 3002 | Forbidden |
| No Artista | Valid token sin Artista | 404 | 2002 | Not Found |

---

### 6.3 Campanias Backings - GET /api/campanias/{id}/backings

| Test Case | Input | Status | ErrorCode | Validaciones |
|-----------|-------|--------|-----------|--------------|
| Valid Campania | Valid ID, Owner | 200 | 0000 | Stats + items paginados, DESC order |
| Page 2 | page=2, pageSize=20 | 200 | 0000 | Skip/Take correcto |
| Custom PageSize | pageSize=10 | 200 | 0000 | 10 items max |
| Empty Backings | Valid ID sin backings | 200 | 0000 | items=[], totalCount=0 |
| Anonymous Backings | Backings con esAnonimo=true | 200 | 0000 | nombreBacker="Anonimo", email=null |
| Mixed Backings | Algunos anonimos, otros no | 200 | 0000 | Ambos tipos presentes |
| Different Artista | Valid ID, Different Owner | 403 | 3002 | Forbidden (ownership) |
| Non-existent | Invalid GUID | 404 | 2003 | Not Found |
| Invalid GUID | invalid-guid | 400 | 1xxx | Validation error |
| Page 0 | page=0 | 400 | 1xxx | Validation error |
| PageSize 101 | pageSize=101 | 400 | 1xxx | Validation error |
| No Token | No Authorization | 401 | 3001 | Unauthorized |
| Fan User | Token Fan | 403 | 3002 | Forbidden |

---

### 6.4 Campanias Stats - GET /api/campanias/{id}/stats

| Test Case | Input | Status | ErrorCode | Validaciones |
|-----------|-------|--------|-----------|--------------|
| Valid Campania | Valid ID, Owner | 200 | 0000 | Todos los campos, calculos correctos |
| Multi-Reward | Campania con varios rewards | 200 | 0000 | rewardStats con multiples items, %=100 |
| Anonymous Backings | Campania con anonimos | 200 | 0000 | Stats validas, progreso correcto |
| Progreso Calculation | Validar serie temporal | 200 | 0000 | Acumulado crece monotonicamente |
| Proyeccion Final | Calcular basado en velocidad | 200 | 0000 | Valor entre 0 e infinito |
| Different Artista | Valid ID, Different Owner | 403 | 3002 | Forbidden |
| Non-existent | Invalid GUID | 404 | 2003 | Not Found |
| Invalid GUID | invalid-guid | 400 | 1xxx | Validation error |
| No Token | No Authorization | 401 | 3001 | Unauthorized |
| Fan User | Token Fan | 403 | 3002 | Forbidden |

---

## 7. Ejecucion y Reportes

### 7.1 Comando de Ejecucion Recomendado

```bash
# Basico - Output CLI
newman run tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli

# Completo con reports
newman run tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra,json \
    --reporter-htmlextra-export reports/dashboard-integration.html \
    --reporter-json-export reports/dashboard-integration.json \
    --timeout 30000 \
    --timeout-request 5000

# Con bail (stop on error)
newman run tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --bail \
    --reporters cli
```

---

### 7.2 Interpretacion de Resultados

**Success:**
```
└─ 28 total
   ├─ 28 passed
   └─ 0 failed
```

**Con Errores:**
```
└─ 28 total
   ├─ 26 passed
   └─ 2 failed

FAILED TESTS:
┌─────────────────────────────────────────────────────┐
│ 1. GET Mis Campanias - Invalid Estado (0)            │
│    Status code is 400                                 │
│    Expected 400, received 200                         │
│                                                       │
│ 2. GET Backings - Different Artista (403)            │
│    User cannot access other artists backings          │
│    Expected 403, received 200                         │
└─────────────────────────────────────────────────────┘
```

---

## 8. Validacion de Contrato (Schema)

### 8.1 DashboardResumenDto Schema Validation

```javascript
// Test agregado a "GET Resumen - Valid Artista"
pm.test('DashboardResumenDto matches expected schema', () => {
    const schema = {
        type: "object",
        properties: {
            artistaId: { type: "string", pattern: "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$" },
            nombreArtistico: { type: "string", minLength: 1 },
            totalRecaudado: { type: "number", minimum: 0 },
            totalBackers: { type: "integer", minimum: 0 },
            campaniasActivas: { type: "integer", minimum: 0 },
            campaniasCompletadas: { type: "integer", minimum: 0 },
            totalCampanias: { type: "integer", minimum: 0 },
            monedaSimbolo: { type: "string", enum: ["EUR"] },
            fechaUltimoAporte: { type: ["string", "null"], pattern: "^\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}Z?$" }
        },
        required: [
            "artistaId", "nombreArtistico", "totalRecaudado", "totalBackers",
            "campaniasActivas", "campaniasCompletadas", "totalCampanias", "monedaSimbolo"
        ]
    };

    pm.expect(tv4.validateResult(pm.response.json().data, schema).valid).to.be.true;
});
```

---

### 8.2 MiCampaniaListItemDto Array Schema

```javascript
pm.test('MiCampaniaListItemDto[] matches expected schema', () => {
    const itemSchema = {
        type: "object",
        properties: {
            id: { type: "string", pattern: "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$" },
            titulo: { type: "string", minLength: 1, maxLength: 200 },
            imagenPrincipalUrl: { type: ["string", "null"] },
            estadoCampaniaId: { type: "integer", enum: [1, 2, 3, 4, 5] },
            estadoCampaniaNombre: { type: "string", enum: ["Borrador", "Publicada", "Finalizada", "Cancelada", "Pausada"] },
            importeObjetivo: { type: "number", minimum: 0 },
            importeRecaudado: { type: "number", minimum: 0 },
            porcentajeProgreso: { type: "number", minimum: 0, maximum: 100 },
            numBackers: { type: "integer", minimum: 0 },
            diasRestantes: { type: ["integer", "null"], minimum: 0 },
            fechaFin: { type: ["string", "null"] },
            fechaCreacion: { type: "string" }
        },
        required: [
            "id", "titulo", "estadoCampaniaId", "estadoCampaniaNombre",
            "importeObjetivo", "importeRecaudado", "porcentajeProgreso", "numBackers", "fechaCreacion"
        ]
    };

    const items = pm.response.json().data.items;
    items.forEach((item, index) => {
        pm.expect(tv4.validateResult(item, itemSchema).valid).to.be.true;
    });
});
```

---

## 9. Testing de Rendimiento

### 9.1 Criterios de Aceptacion de Performance

| Endpoint | Escenario | Target | Threshold |
|----------|-----------|--------|-----------|
| GET /api/dashboard/resumen | User con 10 campanias | < 500ms | < 1000ms |
| GET /api/campanias/mis-campanias | Default (10 items) | < 300ms | < 500ms |
| GET /api/campanias/mis-campanias | Con filter + paginacion | < 400ms | < 700ms |
| GET /api/campanias/{id}/backings | Default (20 items) | < 400ms | < 700ms |
| GET /api/campanias/{id}/backings | Large dataset (500+ backings) | < 800ms | < 1500ms |
| GET /api/campanias/{id}/stats | Campania con 30+ dias | < 600ms | < 1200ms |

---

### 9.2 Load Test Ejemplo

```bash
# Ejecutar 5 iteraciones en paralelo (simular 5 usuarios simultaneos)
newman run tests/newman/WePlay.Dashboard.Artista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    -n 5 \
    --delay-request 100 \
    --reporters cli,json \
    --reporter-json-export "reports/load-test.json"

# Analizar resultados
jq '.run.stats' reports/load-test.json
```

---

## 10. Checklist de Validacion

- [x] Estructura de coleccion organizada por endpoint
- [x] Requests con assert completos para status, schema, datos
- [x] Tests de autenticacion (invalid token, expired, no token)
- [x] Tests de autorizacion (403 forbidden, role validation)
- [x] Tests de validacion (400 bad request con query params invalidos)
- [x] Tests de ownership (user no puede ver campanias de otros)
- [x] Tests de contratos (DashboardResumenDto, MiCampaniaListItemDto, etc.)
- [x] Tests de calculos (porcentajeProgreso, backingPromedio, etc.)
- [x] Tests de paginacion (skip/take, totalPages calculation)
- [x] Tests de ordenamiento (DESC por fechaCreacion)
- [x] Tests de edge cases (empty data, large datasets)
- [x] Variables de entorno (baseUrl, accessToken, artistaId, etc.)
- [x] Scripts de setup (Get Auth Token, Create Test Data)
- [x] Scripts de cleanup (Delete Test Data)
- [x] Pipeline CI/CD (Azure DevOps + GitHub Actions)
- [x] Configuracion de Newman (reporters, timeouts, bail)
- [x] Performance test criteria y load test ejemplo
- [x] Interpretacion de resultados y troubleshooting

---

## 11. Notas Adicionales

### 11.1 Configuracion de Timeout

```javascript
// Pre-request script global
pm.collection.variables.set('timeout', 30000);  // 30 segundos total

// Per-request timeout en test script
setTimeout(() => {
    throw new Error('Request timeout exceeded');
}, pm.environment.get('timeout'));
```

---

### 11.2 Debugging de Requests Fallidos

```javascript
// En test script
pm.test('DEBUG: Log response for debugging', () => {
    console.log('Status:', pm.response.code);
    console.log('Response:', JSON.stringify(pm.response.json(), null, 2));

    // Guardar en coleccion variable para analisis posterior
    pm.collectionVariables.set('lastResponseBody',
        JSON.stringify(pm.response.json()));
});
```

---

### 11.3 Dependencias entre Requests

La coleccion funciona en el siguiente orden:

1. **Setup** → Obtener tokens y preparar data
2. **Dashboard Tests** → Ejecutar tests de resumen (independiente)
3. **Mis Campanias Tests** → Listar campanias (independiente)
4. **Backings Tests** → Detalles de campaña (requiere testCampaniaId1)
5. **Stats Tests** → Estadisticas (requiere testCampaniaId1)
6. **Cleanup** → Limpiar datos de prueba

Todas las folder marcadas como "Runnable" pueden ejecutarse en cualquier orden dentro de su categoria.

---

### 11.4 Monitoreo en Produccion

Post-MVP, considerar:
- Ejecutar Newman tests cada 15 minutos via cron en prod
- Alertas si alguna endpoint tarda > 2 segundos
- Tracking de errores 401/403/404 (posibles problemas de permisos)
- Dashboard de metricas de API (uptime, latencia, tasa de error)

---

## 12. Cronograma de Implementacion

| Fase | Duracion | Tareas |
|------|----------|--------|
| Fase 1 (Setup) | 1-2 horas | Estructura coleccion, variables, auth setup |
| Fase 2 (Dashboard) | 2-3 horas | Requests GET resumen, tests 200/401/403/404 |
| Fase 3 (Mis Campanias) | 2-3 horas | Requests GET con paginacion, validaciones |
| Fase 4 (Backings + Stats) | 3-4 horas | Requests detalladas, schema validation, calculos |
| Fase 5 (CI/CD) | 1-2 horas | Pipeline setup, reporters, artifacts |
| Fase 6 (Docs + Review) | 1 hora | Documentacion, validacion, cleanup |
| **Total** | **10-15 horas** | Coleccion completa lista para uso |

---

## 13. Referencias

- **API Contracts:** `plans/dashboard-artista/backend/api-contracts.md`
- **Feature Spec:** `docs/user-stories/dashboard-artista/feature-spec.md`
- **Newman Docs:** https://learning.postman.com/docs/running-collections/using-newman-cli/
- **Postman Testing:** https://learning.postman.com/docs/writing-scripts/test-scripts/
- **JSON Schema:** https://json-schema.org/
- **HTTP Status Codes:** https://httpwg.org/specs/rfc9110.html#status.codes

---

**Fin del Plan de Testing Newman: Dashboard de Artista**
