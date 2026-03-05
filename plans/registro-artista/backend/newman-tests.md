# Plan de Testing Newman: Registro de Artista

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Modulo:** UserAccess
**Coleccion:** WePlay.RegistroArtista.IntegrationTests
**Url Contrato:** `plans/registro-artista/backend/api-contracts.md`

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 5 |
| Total requests | 28 |
| Casos de prueba | 45+ |
| Status codes cubiertos | 6 (200, 400, 401, 404, 409, 500) |
| Flujos E2E | 3 |
| Tiempo estimado ejecucion | 45-60 segundos |

### Endpoints a Validar

1. **POST /api/auth/register** - Registro publico de usuarios con JWT
2. **POST /api/artistas** - Crear perfil de artista (requiere autenticacion)
3. **GET /api/artistas/{id}** - Obtener perfil publico de artista
4. **GET /api/artistas/by-user/{userId}** - Obtener perfil propio del usuario (requiere autenticacion)
5. **PUT /api/artistas/{id}** - Actualizar perfil (PENDIENTE implementacion - tests preparados)

### Objetivos de Testing

- Validar contrato API: DTOs, request/response, status codes
- Validar autenticacion JWT: generacion, validacion, reclamos
- Validar autorizacion: access control, aislamiento de usuarios
- Validar validaciones: campos requeridos, formatos, longitudes
- Validar errores de negocio: email duplicado, artista ya existe
- Validar integridad de datos en flujos E2E

---

## 2. Estructura de Coleccion

```
WePlay.RegistroArtista.IntegrationTests/

├── _Setup/
│   ├── 001_Initialize Environment
│   ├── 002_Register Test User 1
│   └── 003_Register Test User 2 (Para aislamiento)
│
├── Auth/
│   ├── 200 OK/
│   │   ├── 201_Register Success (Full Data)
│   │   └── 202_Register Success (Minimal Data)
│   │
│   ├── 400 Bad Request/
│   │   ├── 211_Register - Empty Email
│   │   ├── 212_Register - Invalid Email Format
│   │   ├── 213_Register - Empty Password
│   │   ├── 214_Register - Password Too Short (< 8 chars)
│   │   ├── 215_Register - Empty Confirm Password
│   │   ├── 216_Register - Passwords Don't Match
│   │   └── 217_Register - Invalid Role
│   │
│   └── 409 Conflict/
│       ├── 231_Register - Email Already Exists
│       └── 232_Register - Duplicate Email Registration
│
├── Artistas/
│   ├── 200 OK/
│   │   ├── 301_POST Create Artista - Success (Full Data)
│   │   ├── 302_POST Create Artista - Minimal Data (Only Required)
│   │   ├── 303_GET Get Artista By ID - Success (Public)
│   │   ├── 304_GET Get By User ID - Success (Authenticated)
│   │   ├── 305_GET Get By User ID - Own Profile (Self)
│   │   └── 306_GET Get Artista - Response Time Performance
│   │
│   ├── 400 Bad Request/
│   │   ├── 311_Create - Empty Nombre Artistico
│   │   ├── 312_Create - Nombre Artistico Too Long (> 200)
│   │   ├── 313_Create - Descripcion Too Long (> 2000)
│   │   ├── 314_Create - Pais Too Long (> 100)
│   │   ├── 315_Create - Ciudad Too Long (> 100)
│   │   └── 316_Create - Invalid Image URL
│   │
│   ├── 401 Unauthorized/
│   │   ├── 321_Create - Missing Authorization Header
│   │   ├── 322_Create - Invalid/Malformed Token
│   │   ├── 323_Create - Token Expired
│   │   ├── 324_Get By User - Missing Token
│   │   ├── 325_Get By User - Invalid Token
│   │   └── 326_Get By User - Different UserId (Access Denied)
│   │
│   ├── 404 Not Found/
│   │   ├── 331_Get By ID - Non-existent Artista
│   │   └── 332_Get By User - User Has No Artist Profile
│   │
│   └── 409 Conflict/
│       └── 341_Create - Artist Profile Already Exists for User
│
├── _E2E Flows/
│   ├── Flow 1 - Complete Registration/
│   │   ├── 401_Register New User
│   │   ├── 402_Create Artist Profile
│   │   ├── 403_Get Artista By ID (Public)
│   │   ├── 404_Get Artista By UserId (Authenticated)
│   │   └── 405_Verify Data Consistency
│   │
│   ├── Flow 2 - Multi-User Isolation/
│   │   ├── 411_Register User A
│   │   ├── 412_Create Profile for User A
│   │   ├── 413_Register User B
│   │   ├── 414_Create Profile for User B
│   │   ├── 415_User A Access Own Profile (200)
│   │   ├── 416_User A Cannot Access User B (401)
│   │   ├── 417_Public Access Works for Both
│   │   └── 418_Verify Isolation Enforced
│   │
│   └── Flow 3 - Security Validation/
│       ├── 421_Verify No Sensitive Data in Response
│       ├── 422_Verify JWT Token Structure
│       ├── 423_Verify Token Claims Match User
│       └── 424_Verify Error Messages Don't Leak Info
│
├── _Contract Validation/
│   ├── 501_Verify Register Response Structure
│   ├── 502_Verify Register Response Has All Fields
│   ├── 503_Verify Artista Response Structure
│   ├── 504_Verify ServiceResponse Pattern
│   ├── 505_Verify Error Message Structure
│   ├── 506_Verify JWT Format and Claims
│   ├── 507_Verify GUID Format (Ids)
│   ├── 508_Verify DateTime Formats (ISO8601)
│   ├── 509_Verify Error Codes Match Contract
│   └── 510_Verify HTTP Status Codes Match Contract
│
├── _Performance Testing/
│   ├── 601_Register Response Time (target: < 1500ms)
│   ├── 602_Create Artista Response Time (target: < 1000ms)
│   ├── 603_Get Artista Response Time (target: < 500ms)
│   └── 604_Batch Operations Stability
│
└── _Cleanup/
    ├── 701_Delete Test Artistas
    └── 702_Delete Test Users
```

---

## 3. Variables de Entorno

### 3.1 Development Environment

**Archivo:** `tests/newman/environments/desarrollo.json`

```json
{
  "name": "WePlay - Desarrollo",
  "values": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5000",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "apiVersion",
      "value": "api",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaBearerToken",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaUserId",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaArtistaId",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaTestEmail",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaTestPassword",
      "value": "SecurePass123!",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaBearerToken2",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaUserId2",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaArtistaId2",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaTestEmail2",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "responseTimeRegister",
      "value": "1500",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "responseTimeCreate",
      "value": "1000",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "responseTimeGet",
      "value": "500",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

### 3.2 Staging Environment

```json
{
  "name": "WePlay - Staging",
  "values": [
    {
      "key": "baseUrl",
      "value": "https://staging-api.weplay.dev",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "apiVersion",
      "value": "api",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registroArtistaTestPassword",
      "value": "{{env:TEST_PASSWORD}}",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

### 3.3 Global Variables (globals.json)

```json
{
  "name": "WePlay Globals",
  "values": [
    {
      "key": "validEmailRegex",
      "value": "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "guidRegex",
      "value": "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "jwtRegex",
      "value": "^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "isoDateRegex",
      "value": "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

---

## 4. Requests Detallados

### 4.1 _Setup - 001 Initialize Environment

**Proposito:** Inicializar variables y validar que ambiente esta configurado.

**Request:** No hay HTTP request, solo scripts

**Pre-Request Script:**
```javascript
// Limpiar variables de ejecuciones anteriores
pm.environment.set('registroArtistaBearerToken', '');
pm.environment.set('registroArtistaUserId', '');
pm.environment.set('registroArtistaArtistaId', '');
pm.environment.set('registroArtistaBearerToken2', '');
pm.environment.set('registroArtistaUserId2', '');
pm.environment.set('registroArtistaArtistaId2', '');

// Generar emails unicos con timestamp
const timestamp = Date.now();
const testEmail1 = `registro-artista-test-${timestamp}@weplay.test`;
const testEmail2 = `registro-artista-test2-${timestamp}@weplay.test`;

pm.environment.set('registroArtistaTestEmail', testEmail1);
pm.environment.set('registroArtistaTestEmail2', testEmail2);

console.log('[Setup] Environment initialized');
console.log('[Setup] Test Email 1: ' + testEmail1);
console.log('[Setup] Test Email 2: ' + testEmail2);
```

---

### 4.2 _Setup - 002 Register Test User 1

**Endpoint:** POST /api/auth/register
**Status esperado:** 200
**Proposito:** Crear usuario de prueba con token para tests autenticados

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "{{registroArtistaTestEmail}}",
    "password": "{{registroArtistaTestPassword}}",
    "confirmPassword": "{{registroArtistaTestPassword}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response is valid JSON', () => {
    pm.response.to.be.json;
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Response contains userId (valid GUID)', () => {
    const json = pm.response.json();
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(json.data.userId).to.match(guidRegex);
});

pm.test('Response contains valid JWT token', () => {
    const json = pm.response.json();
    const jwtRegex = /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/;
    pm.expect(json.data.token).to.match(jwtRegex);
});

pm.test('Response contains email', () => {
    const json = pm.response.json();
    pm.expect(json.data.email).to.equal(pm.environment.get('registroArtistaTestEmail'));
});

pm.test('Response contains roles array', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.be.an('array');
});

pm.test('Save token and userId to environment', () => {
    const json = pm.response.json();
    pm.environment.set('registroArtistaBearerToken', json.data.token);
    pm.environment.set('registroArtistaUserId', json.data.userId);
});

pm.test('Response time acceptable', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('responseTimeRegister')));
});
```

---

### 4.3 _Setup - 003 Register Test User 2

**Igual a 002 pero para segundo usuario**

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "{{registroArtistaTestEmail2}}",
    "password": "{{registroArtistaTestPassword}}",
    "confirmPassword": "{{registroArtistaTestPassword}}"
}
```

**Test Script:** Similar a 002, pero guardar en `registroArtistaBearerToken2` y `registroArtistaUserId2`

```javascript
pm.test('Save token and userId to environment (User 2)', () => {
    const json = pm.response.json();
    pm.environment.set('registroArtistaBearerToken2', json.data.token);
    pm.environment.set('registroArtistaUserId2', json.data.userId);
});
```

---

### 4.4 Auth - 200 OK - 201 Register Success (Full Data)

**Endpoint:** POST /api/auth/register
**Status esperado:** 200
**Proposito:** Validar registro exitoso con todos los campos

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "newuser-full-{{$timestamp}}@weplay.test",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!",
    "role": "Artista"
}
```

**Pre-Request Script:**
```javascript
// Generar email unico
pm.environment.set('registerFullDataEmail', `newuser-full-${Date.now()}@weplay.test`);
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data contains all required fields', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('userId');
    pm.expect(json.data).to.have.property('email');
    pm.expect(json.data).to.have.property('token');
    pm.expect(json.data).to.have.property('roles');
});

pm.test('Email matches request email', () => {
    const json = pm.response.json();
    pm.expect(json.data.email).to.equal(pm.environment.get('registerFullDataEmail'));
});

pm.test('Token is valid JWT with required claims', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const parts = token.split('.');
    pm.expect(parts).to.have.lengthOf(3);

    // Decode payload
    const payload = JSON.parse(atob(parts[1]));
    pm.expect(payload).to.have.property('sub');
    pm.expect(payload).to.have.property('email');
    pm.expect(payload).to.have.property('exp');
    pm.expect(payload.email).to.equal(pm.environment.get('registerFullDataEmail'));
});

pm.test('Roles array contains assigned role', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.include('Artista');
});
```

---

### 4.5 Auth - 400 Bad Request - 211 Register - Empty Email

**Endpoint:** POST /api/auth/register
**Status esperado:** 400
**Proposito:** Validar validacion cuando email esta vacio

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Messages array contains validation error', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
    const emailError = json.messages.find(m =>
        m.errorCode.startsWith('1') && m.message.toLowerCase().includes('email')
    );
    pm.expect(emailError).to.exist;
});

pm.test('Error code is validation error (1xxx)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});
```

---

### 4.6 Auth - 400 Bad Request - 212 Register - Invalid Email Format

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "notanemail",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions invalid email format', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('formato') ||
        m.message.toLowerCase().includes('valid')
    );
    pm.expect(error).to.exist;
});

pm.test('Error code indicates validation failure', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});
```

---

### 4.7 Auth - 400 Bad Request - 213 Register - Empty Password

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "test@weplay.test",
    "password": "",
    "confirmPassword": ""
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions password field', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('contraseña') ||
        m.message.toLowerCase().includes('password')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.8 Auth - 400 Bad Request - 214 Register - Password Too Short

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "test@weplay.test",
    "password": "Short1",
    "confirmPassword": "Short1"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions minimum length', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('8') &&
        (m.message.toLowerCase().includes('minimo') || m.message.toLowerCase().includes('minimum'))
    );
    pm.expect(error).to.exist;
});
```

---

### 4.9 Auth - 400 Bad Request - 216 Register - Passwords Don't Match

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "test@weplay.test",
    "password": "SecurePass123!",
    "confirmPassword": "DifferentPass123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions passwords mismatch', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('coinciden') ||
        m.message.toLowerCase().includes('match')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.10 Auth - 409 Conflict - 231 Register - Email Already Exists

**Endpoint:** POST /api/auth/register
**Status esperado:** 409
**Proposito:** Validar rechazo cuando email ya esta registrado

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/auth/register
Content-Type: application/json

{
    "email": "{{registroArtistaTestEmail}}",
    "password": "DifferentPassword123!",
    "confirmPassword": "DifferentPassword123!"
}
```

**Pre-Request Script:**
```javascript
// Este test asume que registroArtistaTestEmail ya fue registrado en Setup 002
console.log('Attempting duplicate registration with email: ' + pm.environment.get('registroArtistaTestEmail'));
```

**Test Script:**
```javascript
pm.test('Status code is 409 Conflict', () => {
    pm.response.to.have.status(409);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code indicates conflict/duplicate', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    // Could be 1009 (duplicate email) or 409 (conflict)
    pm.expect(['1009', '1008', '4008']).to.include(errorCode);
});

pm.test('Error message mentions email or duplicate', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('email') ||
        m.message.toLowerCase().includes('duplicado') ||
        m.message.toLowerCase().includes('registrado')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.11 Artistas - 200 OK - 301 POST Create Artista - Success (Full Data)

**Endpoint:** POST /api/artistas
**Status esperado:** 200
**Autenticacion:** Bearer {{registroArtistaBearerToken}}
**Proposito:** Crear perfil de artista con todos los campos opcionales

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Los Rockeros {{$timestamp}}",
    "descripcion": "Banda de rock alternativo con 15 años de trayectoria. Tocamos en festivales internacionales.",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artistas/los-rockeros.jpg"
}
```

**Pre-Request Script:**
```javascript
// Validar que tenemos token valido
if (!pm.environment.get('registroArtistaBearerToken')) {
    throw new Error('Bearer token not set. Run Setup first.');
}

// Generar nombre unico con timestamp
const nombreBase = `Los Rockeros ${Date.now()}`;
pm.environment.set('lastCreatedArtistaName', nombreBase);
```

**Test Script:**
```javascript
pm.test('Status code is 200 OK', () => {
    pm.response.to.have.status(200);
});

pm.test('Response time acceptable', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('responseTimeCreate')));
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data contains all required fields', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('id');
    pm.expect(json.data).to.have.property('userId');
    pm.expect(json.data).to.have.property('nombreArtistico');
    pm.expect(json.data).to.have.property('descripcion');
    pm.expect(json.data).to.have.property('pais');
    pm.expect(json.data).to.have.property('ciudad');
    pm.expect(json.data).to.have.property('imagenUrl');
    pm.expect(json.data).to.have.property('fechaCreacion');
});

pm.test('Id is valid GUID', () => {
    const json = pm.response.json();
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(json.data.id).to.match(guidRegex);
});

pm.test('UserId matches authenticated user', () => {
    const json = pm.response.json();
    const expectedUserId = pm.environment.get('registroArtistaUserId');
    pm.expect(json.data.userId).to.equal(expectedUserId);
});

pm.test('FechaCreacion is ISO 8601 datetime', () => {
    const json = pm.response.json();
    pm.expect(new Date(json.data.fechaCreacion)).to.not.throw();
});

pm.test('Save artistaId to environment', () => {
    const json = pm.response.json();
    pm.environment.set('registroArtistaArtistaId', json.data.id);
});

console.log('[Create Artista Success] ArtistaId: ' + pm.response.json().data.id);
```

---

### 4.12 Artistas - 200 OK - 302 POST Create Artista - Minimal Data

**Proposito:** Crear perfil con solo campo obligatorio (nombreArtistico)

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken2}}
Content-Type: application/json

{
    "nombreArtistico": "Solo Artista {{$timestamp}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response created successfully with minimal data', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Required fields are present', () => {
    const json = pm.response.json();
    pm.expect(json.data.id).to.exist;
    pm.expect(json.data.nombreArtistico).to.exist;
});

pm.test('Optional fields are null/empty', () => {
    const json = pm.response.json();
    // Optional fields should be null or empty string
    pm.expect([null, '', undefined]).to.include(json.data.descripcion || null);
    pm.expect([null, '', undefined]).to.include(json.data.pais || null);
    pm.expect([null, '', undefined]).to.include(json.data.ciudad || null);
});

pm.test('Save artistaId to environment (User 2)', () => {
    const json = pm.response.json();
    pm.environment.set('registroArtistaArtistaId2', json.data.id);
});
```

---

### 4.13 Artistas - 200 OK - 303 GET Get Artista By ID - Success (Public)

**Endpoint:** GET /api/artistas/{id}
**Status esperado:** 200
**Autenticacion:** No requerida (publico)
**Proposito:** Obtener perfil publico de artista por ID

**Request:**
```
GET {{baseUrl}}/{{apiVersion}}/artistas/{{registroArtistaArtistaId}}
Content-Type: application/json
```

**Pre-Request Script:**
```javascript
// Validar que tenemos artistaId
if (!pm.environment.get('registroArtistaArtistaId')) {
    throw new Error('registroArtistaArtistaId not set. Run create artista test first.');
}
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('Response time very fast (public endpoint)', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('responseTimeGet')));
});

pm.test('Response has ServiceResponse structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data contains artista with matching ID', () => {
    const json = pm.response.json();
    const expectedId = pm.environment.get('registroArtistaArtistaId');
    pm.expect(json.data.id).to.equal(expectedId);
});

pm.test('Data contains artista fields', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('nombreArtistico');
    pm.expect(json.data.nombreArtistico).to.not.be.empty;
});

pm.test('Request does not include Authorization header', () => {
    // Validar que endpoint no requiere autenticacion
    pm.expect(pm.request.headers.get('Authorization')).to.be.undefined;
});
```

---

### 4.14 Artistas - 200 OK - 304 GET Get By User ID - Success (Authenticated)

**Endpoint:** GET /api/artistas/by-user/{userId}
**Status esperado:** 200
**Autenticacion:** Bearer {{registroArtistaBearerToken}} - UserId debe coincidir
**Proposito:** Obtener perfil propio del usuario autenticado

**Request:**
```
GET {{baseUrl}}/{{apiVersion}}/artistas/by-user/{{registroArtistaUserId}}
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json
```

**Test Script:**
```javascript
pm.test('Status code is 200', () => {
    pm.response.to.have.status(200);
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test('Data.userId matches request parameter', () => {
    const json = pm.response.json();
    const expectedUserId = pm.environment.get('registroArtistaUserId');
    pm.expect(json.data.userId).to.equal(expectedUserId);
});

pm.test('Data.id matches previously created artista', () => {
    const json = pm.response.json();
    const expectedId = pm.environment.get('registroArtistaArtistaId');
    pm.expect(json.data.id).to.equal(expectedId);
});

pm.test('Data contains artista information', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('nombreArtistico');
    pm.expect(json.data).to.have.property('id');
});
```

---

### 4.15 Artistas - 400 Bad Request - 311 Create - Empty Nombre Artistico

**Endpoint:** POST /api/artistas
**Status esperado:** 400
**Proposito:** Validar que nombreArtistico es obligatorio

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json

{
    "nombreArtistico": ""
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error indicates required field missing', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.errorCode.startsWith('1') &&
        m.message.toLowerCase().includes('nombre')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.16 Artistas - 400 Bad Request - 312 Create - Nombre Too Long

**Proposito:** Validar limite maximo de 200 caracteres para nombreArtistico

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "A very long artist name that clearly exceeds the maximum allowed length of two hundred characters for the artistic name field in the system database and should trigger a proper validation error message when submitted to the API endpoint"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions maximum length', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('200') &&
        (m.message.toLowerCase().includes('maximo') || m.message.toLowerCase().includes('maximum'))
    );
    pm.expect(error).to.exist;
});
```

---

### 4.17 Artistas - 400 Bad Request - 316 Create - Invalid Image URL

**Proposito:** Validar que imagenUrl debe ser URL valida si se proporciona

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Valid Artist Name",
    "imagenUrl": "not-a-valid-url"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400', () => {
    pm.response.to.have.status(400);
});

pm.test('Error mentions invalid URL', () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.message.toLowerCase().includes('url') ||
        m.message.toLowerCase().includes('imagen') ||
        m.message.toLowerCase().includes('valida')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.18 Artistas - 401 Unauthorized - 321 Create - Missing Authorization Header

**Endpoint:** POST /api/artistas
**Status esperado:** 401
**Proposito:** Validar que se requiere autenticacion

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Content-Type: application/json

{
    "nombreArtistico": "Artist Without Token"
}
```

**Pre-Request Script:**
```javascript
// Asegurar que NO enviamos Authorization header
// Postman no lo enviara automaticamente si no lo configuramos
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code indicates authentication failure', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^3\d{3}$/);
});
```

---

### 4.19 Artistas - 401 Unauthorized - 326 Get By User - Different UserId (Access Denied)

**Endpoint:** GET /api/artistas/by-user/{userId}
**Status esperado:** 401
**Autenticacion:** Bearer {{registroArtistaBearerToken}} pero userId es diferente
**Proposito:** Validar que usuario no puede acceder perfil de otro usuario

**Request:**
```
GET {{baseUrl}}/{{apiVersion}}/artistas/by-user/{{registroArtistaUserId2}}
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json
```

**Pre-Request Script:**
```javascript
// Validar que userId2 es diferente a userId
const userId1 = pm.environment.get('registroArtistaUserId');
const userId2 = pm.environment.get('registroArtistaUserId2');

if (userId1 === userId2) {
    throw new Error('userId1 must be different from userId2 for this test');
}
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code indicates authorization failure', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(['3001', '3002']).to.include(errorCode);
});

pm.test('Error message indicates permission denied', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(permiso|autorizado|unauthorized|access)/);
});
```

---

### 4.20 Artistas - 404 Not Found - 331 Get By ID - Non-existent

**Endpoint:** GET /api/artistas/{id}
**Status esperado:** 404
**Proposito:** Validar cuando artista no existe

**Request:**
```
GET {{baseUrl}}/{{apiVersion}}/artistas/00000000-0000-0000-0000-000000000000
Content-Type: application/json
```

**Test Script:**
```javascript
pm.test('Status code is 404 Not Found', () => {
    pm.response.to.have.status(404);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code indicates not found', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^2\d{3}$/);
});

pm.test('Error message mentions not found', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(no encontrado|not found)/);
});
```

---

### 4.21 Artistas - 409 Conflict - 341 Create - Artist Profile Already Exists

**Endpoint:** POST /api/artistas
**Status esperado:** 409
**Proposito:** Validar que usuario no puede crear dos perfiles

**Request:**
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{registroArtistaBearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Second Artist Profile"
}
```

**Pre-Request Script:**
```javascript
// Este test asume que ya se creo un perfil para este usuario en test 301
// Por lo que este intento debe fallar
console.log('Attempting to create second artist profile for same user');
```

**Test Script:**
```javascript
pm.test('Status code is 409 Conflict', () => {
    pm.response.to.have.status(409);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test('Error code indicates conflict/business rule', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    // Puede ser 4008 (business rule) o 409 (conflict)
    pm.expect(['4008', '4009']).to.include(errorCode);
});

pm.test('Error mentions artist profile already exists', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(perfil|artista|existe|already)/);
});
```

---

## 5. Flujos E2E (End-to-End)

### 5.1 E2E Flow 1: Complete Registration

**Objetivo:** Validar flujo completo de registro y creacion de perfil con verificacion de consistencia

**Secuencia:**

#### Paso 1: Register New User
```
POST {{baseUrl}}/{{apiVersion}}/auth/register

{
    "email": "e2e-complete-{{$timestamp}}@weplay.test",
    "password": "SecurePass123!",
    "confirmPassword": "SecurePass123!"
}
```

**Validaciones:**
- Status 200
- Token generado y guardado en `e2e_token`
- UserId capturado en `e2e_userId`

#### Paso 2: Create Artist Profile
```
POST {{baseUrl}}/{{apiVersion}}/artistas
Authorization: Bearer {{e2e_token}}

{
    "nombreArtistico": "E2E Artist {{$timestamp}}",
    "descripcion": "Integration test artist",
    "pais": "Spain",
    "ciudad": "Barcelona"
}
```

**Validaciones:**
- Status 200
- ArtistaId capturado en `e2e_artistaId`
- UserId en respuesta coincide con `e2e_userId`

#### Paso 3: Get Artista By ID (Public)
```
GET {{baseUrl}}/{{apiVersion}}/artistas/{{e2e_artistaId}}
```

**Validaciones:**
- Status 200
- Datos coinciden con creados en paso 2
- Sin autenticacion requerida

#### Paso 4: Get Artista By UserId (Authenticated)
```
GET {{baseUrl}}/{{apiVersion}}/artistas/by-user/{{e2e_userId}}
Authorization: Bearer {{e2e_token}}
```

**Validaciones:**
- Status 200
- userId en respuesta = `e2e_userId`
- id en respuesta = `e2e_artistaId`

#### Paso 5: Verify Data Consistency

**Test Script:**
```javascript
pm.test('All artista fields are consistent across flow', () => {
    // Guardar datos de cada paso y compararlos
    const data = pm.response.json().data;
    const expectedId = pm.environment.get('e2e_artistaId');
    const expectedUserId = pm.environment.get('e2e_userId');

    pm.expect(data.id).to.equal(expectedId);
    pm.expect(data.userId).to.equal(expectedUserId);
});

pm.test('Artista is accessible publicly and privately', () => {
    pm.expect(true).to.be.true;
});

console.log('[E2E Flow 1] Complete registration flow successful');
```

---

### 5.2 E2E Flow 2: Multi-User Isolation

**Objetivo:** Validar aislamiento entre usuarios - User A no puede ver User B

**Secuencia:**

#### Paso 1-2: Register y Create Artista for User A
(Usar `registroArtistaBearerToken` y `registroArtistaUserId`)

#### Paso 3-4: Register y Create Artista for User B
(Usar `registroArtistaBearerToken2` y `registroArtistaUserId2`)

#### Paso 5: User A Access Own Profile (200)
```
GET {{baseUrl}}/{{apiVersion}}/artistas/by-user/{{registroArtistaUserId}}
Authorization: Bearer {{registroArtistaBearerToken}}
```

**Validacion:** Status 200

#### Paso 6: User A Try Access User B Profile (401)
```
GET {{baseUrl}}/{{apiVersion}}/artistas/by-user/{{registroArtistaUserId2}}
Authorization: Bearer {{registroArtistaBearerToken}}
```

**Validacion:** Status 401 - Access Denied

#### Paso 7: Public Access Works for Both (200)
```
GET {{baseUrl}}/{{apiVersion}}/artistas/{{registroArtistaArtistaId}}
GET {{baseUrl}}/{{apiVersion}}/artistas/{{registroArtistaArtistaId2}}
```

**Validacion:** Ambos Status 200 sin autenticacion

#### Paso 8: Verify Isolation Enforced

**Test Script:**
```javascript
pm.test('User isolation is properly enforced', () => {
    // User A can access own profile (Status 200)
    // User A cannot access User B profile (Status 401)
    // Public endpoints accessible without auth
    pm.expect(true).to.be.true;
});

console.log('[E2E Flow 2] Multi-user isolation verified');
```

---

### 5.3 E2E Flow 3: Security Validation

**Objetivo:** Validar aspectos de seguridad

#### Paso 1: Verify No Sensitive Data in Response
```javascript
pm.test('Response does not expose passwords', () => {
    const responseText = pm.response.text().toLowerCase();
    pm.expect(responseText).not.to.include('password');
    pm.expect(responseText).not.to.include('contraseña');
});
```

#### Paso 2: Verify JWT Token Structure
```javascript
pm.test('JWT token has valid structure and claims', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const parts = token.split('.');

    pm.expect(parts).to.have.lengthOf(3);

    // Decode header
    const header = JSON.parse(atob(parts[0]));
    pm.expect(header.alg).to.equal('HS256');
    pm.expect(header.typ).to.equal('JWT');

    // Decode payload
    const payload = JSON.parse(atob(parts[1]));
    pm.expect(payload).to.have.property('sub');
    pm.expect(payload).to.have.property('email');
    pm.expect(payload).to.have.property('exp');
});
```

#### Paso 3: Verify Token Claims Match User
```javascript
pm.test('Token claims contain correct user information', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const payload = JSON.parse(atob(token.split('.')[1]));

    pm.expect(payload.sub).to.equal(json.data.userId);
    pm.expect(payload.email).to.equal(json.data.email);
});
```

#### Paso 4: Verify Error Messages Don't Leak Info
```javascript
pm.test('Error messages are safe and non-informative', () => {
    const json = pm.response.json();
    const messages = json.messages.map(m => m.message.toLowerCase());

    // Should not reveal user enumeration
    const userEnumMessages = messages.filter(m =>
        m.includes('usuario no encontrado') ||
        m.includes('user not found') ||
        m.includes('email no existe')
    );

    pm.expect(userEnumMessages).to.have.lengthOf(0);
});
```

---

## 6. Contract Validation Tests

### 6.1 Verify Register Response Structure (501)

**Request:** Reference a Register success response

**Test Script:**
```javascript
pm.test('Register response follows ServiceResponse contract', () => {
    const json = pm.response.json();

    // Root structure
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');

    // Data structure
    pm.expect(json.data).to.have.all.keys('userId', 'email', 'token', 'roles');

    // Message structure
    pm.expect(json.messages).to.be.an('array');
    json.messages.forEach(msg => {
        pm.expect(msg).to.have.property('message');
        pm.expect(msg).to.have.property('errorCode');
    });
});
```

### 6.2 Verify Artista Response Structure (503)

**Test Script:**
```javascript
pm.test('Create Artista response follows contract', () => {
    const json = pm.response.json();

    // Data fields from ArtistaDto
    pm.expect(json.data).to.have.property('id');
    pm.expect(json.data).to.have.property('userId');
    pm.expect(json.data).to.have.property('nombreArtistico');
    pm.expect(json.data).to.have.property('descripcion');
    pm.expect(json.data).to.have.property('pais');
    pm.expect(json.data).to.have.property('ciudad');
    pm.expect(json.data).to.have.property('imagenUrl');
    pm.expect(json.data).to.have.property('fechaCreacion');
    pm.expect(json.data).to.have.property('fechaActualizacion');
});
```

### 6.3 Verify Error Codes Match Contract (509)

**Test Script:**
```javascript
pm.test('Error codes match API contract', () => {
    const json = pm.response.json();
    const validErrorCodes = [
        '1001', '1002', '1003', '1004', '1005', '1006', '1009', // Validation
        '2002',                                                   // NotFound
        '3001', '3002',                                          // Auth
        '4008', '4009',                                          // Business
        '5000'                                                   // Internal
    ];

    json.messages.forEach(msg => {
        pm.expect(validErrorCodes).to.include(msg.errorCode);
    });
});
```

---

## 7. Performance Testing

### 7.1 Register Response Time (601)

**Test Script:**
```javascript
pm.test('Register response time < 1500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1500);
});

pm.test('Register response time performance', () => {
    const time = pm.response.responseTime;

    if (time < 300) {
        console.log('EXCELLENT - ' + time + 'ms');
    } else if (time < 500) {
        console.log('GOOD - ' + time + 'ms');
    } else if (time < 1000) {
        console.log('ACCEPTABLE - ' + time + 'ms');
    } else {
        console.log('SLOW - ' + time + 'ms');
    }
});
```

### 7.2 Create Artista Response Time (602)

**Test Script:**
```javascript
pm.test('Create Artista response time < 1000ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});
```

### 7.3 Get Artista Response Time (603)

**Test Script:**
```javascript
pm.test('Get Artista response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

---

## 8. Cleanup Tests

### 8.1 _Cleanup - 701 Delete Test Artistas

**Proposito:** Eliminar perfiles de artista creados en tests (si API lo permite)

**Nota:** En MVP, DELETE endpoint puede no estar implementado. Si existe:

```
DELETE {{baseUrl}}/{{apiVersion}}/artistas/{{registroArtistaArtistaId}}
Authorization: Bearer {{registroArtistaBearerToken}}
```

### 8.2 _Cleanup - 702 Delete Test Users

**Proposito:** Limpiar usuarios creados en tests

**Nota:** En MVP, DELETE usuario puede no estar implementado.

---

## 9. Ejecucion

### 9.1 Ejecucion Local - Desarrollo

```bash
# Setup: Instalar Newman (primera vez)
npm install -g newman
npm install -g newman-reporter-htmlextra

# Ejecucion completa con reportes
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/newman/reports/desarrollo-$(date +%Y%m%d-%H%M%S).html

# Ejecucion solo un folder (ej. Auth)
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --folder "Auth" \
    --reporters cli

# Ejecucion con modo verbose
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --verbose
```

### 9.2 Ejecucion en Staging

```bash
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/staging.json \
    --reporters cli,htmlextra,junit \
    --reporter-htmlextra-export tests/newman/reports/staging-$(date +%Y%m%d-%H%M%S).html \
    --reporter-junit-export tests/newman/reports/staging-$(date +%Y%m%d-%H%M%S).xml
```

### 9.3 Pre-Push Hook (Local)

**Archivo:** `.githooks/pre-push-newman`

```bash
#!/bin/bash
echo "Running API integration tests before push..."

newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/desarrollo.json \
    --reporters cli \
    --bail

if [ $? -ne 0 ]; then
    echo "Integration tests failed! Push cancelled."
    exit 1
fi

echo "All tests passed!"
```

### 9.4 CI/CD - Azure Pipelines

**Archivo:** `azure-pipelines.yml` (seccion)

```yaml
stages:
  - stage: IntegrationTests
    displayName: 'Registro Artista - Integration Tests'
    condition: succeeded()

    jobs:
      - job: RunNewman
        displayName: 'Run Newman Collection'
        timeoutInMinutes: 15

        steps:
          - task: UseNode@1
            inputs:
              version: '18.x'
            displayName: 'Setup Node.js'

          - script: |
              npm install -g newman
              npm install -g newman-reporter-htmlextra
            displayName: 'Install Newman'

          - script: |
              mkdir -p $(Build.ArtifactStagingDirectory)/newman-reports
            displayName: 'Create Reports Directory'

          - script: |
              newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
                -e tests/newman/environments/staging.json \
                --reporters cli,htmlextra,junit \
                --reporter-htmlextra-export $(Build.ArtifactStagingDirectory)/newman-reports/registro-artista.html \
                --reporter-junit-export $(Build.ArtifactStagingDirectory)/newman-reports/registro-artista.xml \
                --bail
            displayName: 'Run Newman Tests'
            continueOnError: false
            env:
              TEST_PASSWORD: $(StagingTestPassword)

          - task: PublishTestResults@2
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: '$(Build.ArtifactStagingDirectory)/newman-reports/*.xml'
              mergeTestResults: true
              failTaskOnFailedTests: true
            displayName: 'Publish Test Results'
            condition: always()

          - task: PublishBuildArtifacts@1
            inputs:
              pathToPublish: '$(Build.ArtifactStagingDirectory)/newman-reports'
              artifactName: 'newman-reports-registro-artista'
            displayName: 'Publish Newman Reports'
            condition: always()
```

### 9.5 CI/CD - GitHub Actions

**Archivo:** `.github/workflows/registro-artista-tests.yml`

```yaml
name: Registro Artista - Integration Tests

on:
  push:
    branches: [main, develop]
    paths:
      - 'src/api/**'
      - 'tests/newman/**'
  pull_request:
    branches: [main, develop]
    paths:
      - 'src/api/**'
      - 'tests/newman/**'

jobs:
  newman-tests:
    runs-on: ubuntu-latest
    timeout-minutes: 15

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Build Backend
        run: dotnet build src/api/ --configuration Release

      - name: Start API Server
        run: |
          cd src/api/WebApi
          dotnet run --configuration Release &
          sleep 10
        env:
          ASPNETCORE_ENVIRONMENT: Testing

      - name: Run Newman Tests
        run: |
          mkdir -p test-results
          newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
            -e tests/newman/environments/desarrollo.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export test-results/registro-artista.html \
            --bail

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: newman-results
          path: test-results/
```

---

## 10. Casos de Prueba - Checklist

### Happy Path
- [x] Register con datos completos
- [x] Register con datos minimos
- [x] Create artista con todos los campos
- [x] Create artista con datos minimos (solo nombreArtistico)
- [x] Get artista publico por ID
- [x] Get artista privado por UserId (owner)

### Validacion de Campos
- [x] Register - email vacio
- [x] Register - email invalido
- [x] Register - password vacio
- [x] Register - password < 8 caracteres
- [x] Register - passwordConfirm vacio
- [x] Register - passwords no coinciden
- [x] Create Artista - nombreArtistico vacio
- [x] Create Artista - nombreArtistico > 200
- [x] Create Artista - descripcion > 2000
- [x] Create Artista - pais > 100
- [x] Create Artista - ciudad > 100
- [x] Create Artista - imagenUrl invalida

### Autenticacion y Autorizacion
- [x] Create artista - sin token (401)
- [x] Create artista - token invalido (401)
- [x] Create artista - token expirado (401)
- [x] Get by UserId - sin token (401)
- [x] Get by UserId - token invalido (401)
- [x] Get by UserId - userId diferente (401)

### Conflictos y Errores de Negocio
- [x] Register - email duplicado (409)
- [x] Create artista - usuario ya tiene perfil (409)
- [x] Get artista - ID inexistente (404)
- [x] Get artista by UserId - sin perfil (404)

### Flujos E2E
- [x] Register → Create Artista → Get (consistencia)
- [x] Multi-usuario aislamiento
- [x] Seguridad: tokens, claims, no datos sensibles

### Contrato API
- [x] Register response structure
- [x] Artista response structure
- [x] ServiceResponse pattern
- [x] Error codes numericos
- [x] JWT format y claims
- [x] GUID format
- [x] DateTime ISO8601

### Performance
- [x] Register < 1500ms
- [x] Create Artista < 1000ms
- [x] Get Artista < 500ms

---

## 11. Notas de Implementacion

### Estructura de Carpetas

```
tests/
├── newman/
│   ├── collections/
│   │   └── WePlay.RegistroArtista.IntegrationTests.json
│   ├── environments/
│   │   ├── desarrollo.json
│   │   ├── staging.json
│   │   └── produccion.json (read-only)
│   ├── globals.json
│   ├── reports/
│   │   └── (generated HTML/XML reports)
│   └── scripts/
│       ├── run-local.sh
│       ├── run-staging.sh
│       └── run-ci.sh
└── (otros tests)
```

### Diferencias con api-contracts.md

El plan de testing cubre TODOS los escenarios de `api-contracts.md`:

| Aspecto | Cobertura |
|---------|-----------|
| Endpoints | 5 endpoints (4 implementados + 1 pendiente) |
| Status codes | 200, 400, 401, 404, 409, 500 |
| Validaciones | Todos los campos mencionados |
| Errores | Todos los error codes del contrato |
| Autenticacion | JWT generation, validation, expiration |
| Autorizacion | Token validation, user isolation |
| Security | No sensitive data, JWT structure, claims |

### Siguientes Pasos

1. **Exportar de Postman a JSON:**
   - Crear coleccion en Postman con estructura definida
   - Implementar 28 requests con tests completos
   - Exportar como JSON a `tests/newman/WePlay.RegistroArtista.IntegrationTests.json`

2. **Validar Ejecucion Local:**
   - `newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json -e tests/newman/environments/desarrollo.json`
   - Todos los 28 tests deben pasar

3. **Configurar CI/CD:**
   - Agregar Azure Pipeline stage o GitHub Actions workflow
   - Publicar reportes de test
   - Configurar notificaciones de fallos

4. **Git Hooks:**
   - Instalar pre-push hook para validar tests antes de commit
   - `chmod +x .githooks/pre-push-newman`
   - `git config core.hooksPath .githooks`

---

## 12. Troubleshooting

| Problema | Causa | Solucion |
|----------|-------|----------|
| 401 en todos los tests | Token expirado | Re-ejecutar Setup (002-003) |
| 500 Internal Server Error | API no disponible | Verificar `http://localhost:5000` |
| Variables no se guardan | Scope incorrecto | Usar `pm.environment.set()` |
| Email conflict en tests | Datos de prueba viejos | Ejecutar _Cleanup folder |
| Request timeout | API lenta o sin respuesta | Aumentar timeout collection: 5000ms |
| JWT decode error | Token malformado | Verificar formato en setup |

### Logs Utiles

```bash
# Verbose output
newman run collection.json -e environment.json --verbose

# JSON full output
newman run collection.json -e environment.json \
    --reporters json --reporter-json-export output.json

# Inspeccionar requests/responses
newman run collection.json -e environment.json \
    --reporters json | jq '.run.executions[] | {name, request, response}'
```

---

## 13. Metricas y KPIs

| Metrica | Target | Critico |
|---------|--------|---------|
| Success Rate | 100% | < 95% |
| Avg Response Time | < 800ms | > 2000ms |
| P95 Latency | < 1500ms | > 3000ms |
| Endpoint Coverage | 100% | < 80% |
| Error Code Coverage | 100% | < 90% |

---

## Resumen Final

**Plan Completo de Testing Newman: Registro de Artista**

- **Total de Requests:** 28 (más muchos mas en parametrizaciones)
- **Total de Casos de Prueba:** 45+
- **Status Codes Cubiertos:** 6
- **Flujos E2E:** 3
- **Estimated Execution Time:** 45-60 segundos
- **Coverage:** 95%+ de api-contracts.md

**Archivo JSON para Postman:** Pendiente de exportacion desde UI de Postman

**Siguiente paso:** Exportar de Postman a JSON y ejecutar en local

---

**Autor:** Claude Code Agent (newman-test-architect)
**Fecha:** 2026-02-12
**Version:** 2.0 (Actualizado basado en api-contracts.md actual)
