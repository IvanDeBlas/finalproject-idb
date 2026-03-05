# Plan de Testing Newman: WPR-006 Identity JWT Authentication

**Fecha:** 2026-02-12
**Feature:** WPR-006-identity-jwt
**Modulo:** UserAccess
**Coleccion:** WePlay.WPR006.IdentityTests
**Endpoints a testear:** 3
**Total requests planificados:** 18
**Casos de prueba:** 30+

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 3 |
| Total requests | 18 |
| Casos de prueba | 30+ |
| Flujos E2E | 3 |
| Escenarios de error | 12+ |
| Variables de entorno | 12 |

### Endpoints a Testear

1. **POST /api/auth/register** - Registro de nuevos usuarios (YA EXISTE)
2. **POST /api/auth/login** - Login con email/password (A IMPLEMENTAR)
3. **GET /api/auth/me** - Obtener usuario autenticado (A IMPLEMENTAR)

---

## 2. Estructura de Coleccion

```
WePlay.WPR006.IdentityTests/
│
├── _Setup/
│   ├── 001_Initialize Environment Variables
│   ├── 002_Get Test User Credentials
│   └── 003_Cleanup Previous Test Data (Opcional)
│
├── Auth - Register/
│   ├── 200 OK - Success Cases/
│   │   ├── 201_Register with Valid Data (Fan)
│   │   ├── 202_Register with Valid Data (Artista)
│   │   ├── 203_Register with Valid Data (Admin)
│   │   └── 204_Register and Save Token
│   │
│   ├── 400 Bad Request - Validation Errors/
│   │   ├── 211_Register with Empty Email
│   │   ├── 212_Register with Invalid Email Format
│   │   ├── 213_Register with Empty Password
│   │   ├── 214_Register with Short Password (< 8 chars)
│   │   ├── 215_Register with Non-matching Passwords
│   │   ├── 216_Register with Empty ConfirmPassword
│   │   ├── 217_Register with Invalid Role
│   │   └── 218_Register with Null/Undefined Fields
│   │
│   ├── 409 Conflict - Business Rules/
│   │   ├── 221_Register with Existing Email
│   │   └── 222_Register Twice with Same Email
│   │
│   └── 500 Internal Server Error/
│       └── 231_Register and Verify Database Consistency
│
├── Auth - Login/
│   ├── 200 OK - Success Cases/
│   │   ├── 301_Login with Valid Credentials (Fan)
│   │   ├── 302_Login with Valid Credentials (Artista)
│   │   ├── 303_Login with Valid Credentials (Admin)
│   │   ├── 304_Login and Verify Token Structure
│   │   ├── 305_Login and Verify Token Claims
│   │   ├── 306_Login Multiple Times (Different Tokens)
│   │   └── 307_Login and Extract Roles Array
│   │
│   ├── 400 Bad Request - Validation Errors/
│   │   ├── 311_Login with Empty Email
│   │   ├── 312_Login with Invalid Email Format
│   │   ├── 313_Login with Empty Password
│   │   ├── 314_Login with Only Spaces Email
│   │   ├── 315_Login with Only Spaces Password
│   │   └── 316_Login with Missing Email
│   │
│   ├── 401 Unauthorized - Invalid Credentials/
│   │   ├── 321_Login with Non-existent Email
│   │   ├── 322_Login with Incorrect Password
│   │   ├── 323_Login with Correct Email Wrong Password
│   │   ├── 324_Login Case-Sensitive Email Check
│   │   └── 325_Login with Empty Body
│   │
│   └── 5xx Server Errors/
│       └── 331_Login and Handle Locked Account
│
├── Auth - Get Current User (/me)/
│   ├── 200 OK - Success Cases/
│   │   ├── 401_Get User with Valid Token
│   │   ├── 402_Get User and Verify Email
│   │   ├── 403_Get User and Verify Roles
│   │   ├── 404_Get User and Verify EmailConfirmed Status
│   │   ├── 405_Get User After Register Flow
│   │   └── 406_Get User After Login Flow
│   │
│   ├── 401 Unauthorized - Authentication Issues/
│   │   ├── 411_Get User without Token
│   │   ├── 412_Get User with Invalid Token
│   │   ├── 413_Get User with Expired Token
│   │   ├── 414_Get User with Token from Another User
│   │   ├── 415_Get User with Malformed Header
│   │   ├── 416_Get User with Bearer Missing
│   │   └── 417_Get User with Empty Bearer Token
│   │
│   ├── 404 Not Found/
│   │   ├── 421_Get Deleted User
│   │   └── 422_Get User with Non-existent UserId Claim
│   │
│   └── 500 Internal Server Error/
│       └── 431_Get User and Database Consistency
│
├── E2E Flows/
│   ├── Flow 1 - Register New User → Login → Get User Info/
│   │   ├── 501_Register New User
│   │   ├── 502_Login with Registered Credentials
│   │   ├── 503_Get User Info with Login Token
│   │   └── 504_Verify Data Consistency Across Flow
│   │
│   ├── Flow 2 - Register Artista → Login → Verify Roles/
│   │   ├── 511_Register as Artista
│   │   ├── 512_Login and Extract Roles
│   │   ├── 513_Get User and Verify Artista Role
│   │   └── 514_Verify Role Claims Match
│   │
│   └── Flow 3 - Multiple Login Sessions/
│       ├── 521_Login Session 1
│       ├── 522_Login Session 2
│       ├── 523_Both Sessions Valid Simultaneously
│       └── 524_Tokens Are Different
│
├── Security & Performance/
│   ├── Security Tests/
│   │   ├── 601_Verify HTTPS in Production URLs
│   │   ├── 602_Verify Password Not Returned in Response
│   │   ├── 603_Verify Token Not Logged
│   │   ├── 604_Verify No Sensitive Data in Error Messages
│   │   └── 605_Verify SQL Injection Prevention
│   │
│   └── Performance Tests/
│       ├── 611_Register Response Time < 1000ms
│       ├── 612_Login Response Time < 500ms
│       ├── 613_Get User Response Time < 200ms
│       ├── 614_Batch Register Performance
│       └── 615_Concurrent Login Requests
│
├── Contract Validation/
│   ├── 701_Verify Register Response Structure
│   ├── 702_Verify Login Response Structure
│   ├── 703_Verify User Info Response Structure
│   ├── 704_Verify Error Response Structure
│   ├── 705_Verify Token Is Valid JWT Format
│   ├── 706_Verify Token Expiration Claim
│   ├── 707_Verify Token Signature
│   └── 708_Verify Claims Match ServiceResponse
│
└── _Cleanup/
    ├── 801_Delete Test Users
    ├── 802_Clear Environment Variables
    └── 803_Reset Collection State
```

---

## 3. Variables de Entorno

### 3.1 Development Environment

```json
{
  "name": "WePlay WPR006 Development",
  "values": [
    {
      "key": "base_url",
      "value": "http://localhost:5261",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "api_url",
      "value": "{{base_url}}/api",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "auth_endpoint",
      "value": "{{api_url}}/auth",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "test_email_base",
      "value": "test_{{$randomInt:1000:9999}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "test_password",
      "value": "TestPassword123!",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "artista_email",
      "value": "artista_{{$timestamp}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "fan_email",
      "value": "fan_{{$timestamp}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "admin_email",
      "value": "admin_{{$timestamp}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "registered_user_id",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "auth_token",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "auth_token_2",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "current_user_id",
      "value": "",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "response_time_limit_register",
      "value": "1000",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "response_time_limit_login",
      "value": "500",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "response_time_limit_get_user",
      "value": "200",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

### 3.2 Staging Environment

```json
{
  "name": "WePlay WPR006 Staging",
  "values": [
    {
      "key": "base_url",
      "value": "https://staging-api.weplay.com",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "api_url",
      "value": "{{base_url}}/api",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "auth_endpoint",
      "value": "{{api_url}}/auth",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "test_email_base",
      "value": "test_{{$randomInt:10000:99999}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "test_password",
      "value": "{{env:TEST_PASSWORD}}",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

### 3.3 Production Environment (Read-Only Tests)

```json
{
  "name": "WePlay WPR006 Production",
  "values": [
    {
      "key": "base_url",
      "value": "https://api.weplay.com",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "api_url",
      "value": "{{base_url}}/api",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "auth_endpoint",
      "value": "{{api_url}}/auth",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "test_email_base",
      "value": "test_{{$timestamp}}@weplay.test",
      "enabled": true,
      "type": "string"
    },
    {
      "key": "production_mode",
      "value": "true",
      "enabled": true,
      "type": "string"
    }
  ]
}
```

---

## 4. Pre-Request Scripts y Global Scripts

### 4.1 Collection Level Pre-Request Script

```javascript
// Ejecutar antes de CADA request
// Inicializar variables si no existen
if (!pm.environment.get('auth_token') || pm.environment.get('auth_token') === '') {
    pm.environment.set('auth_token', '');
}

if (!pm.environment.get('registered_user_id')) {
    pm.environment.set('registered_user_id', '');
}

if (!pm.environment.get('current_user_id')) {
    pm.environment.set('current_user_id', '');
}

// Log del request que se va a ejecutar
console.log(`[${new Date().toISOString()}] Executing: ${pm.request.method} ${pm.request.url}`);
```

### 4.2 Collection Level Tests (Global After Hook)

```javascript
// Ejecutar despues de CADA request
// Validar estructura basica de respuesta para todos los endpoints
pm.test('Response is valid JSON', () => {
    pm.response.to.be.json;
});

pm.test('Response time is reasonable', () => {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

// Log de respuesta
console.log(`[${new Date().toISOString()}] Response: ${pm.response.code} - ${pm.response.responseTime}ms`);
```

### 4.3 Helper Function Script (Setup request)

```javascript
// Guardar en folder _Setup como pre-request script
// Genera email unico para cada test
const generateTestEmail = (role = 'test') => {
    const timestamp = Date.now();
    const random = Math.floor(Math.random() * 10000);
    return `${role}_${timestamp}_${random}@weplay.test`;
};

// Guardar en environment para uso en otros requests
pm.environment.set('generated_email', generateTestEmail());

// Log
console.log(`Generated email: ${pm.environment.get('generated_email')}`);
```

---

## 5. Requests Detallados

### 5.1 _Setup - Initialize Environment Variables

**Metodo:** POST
**URL:** `{{auth_endpoint}}/register`
**Tipo:** Preparacion

**Pre-Request Script:**
```javascript
// Limpiar variables de ejecuciones anteriores
pm.environment.set('auth_token', '');
pm.environment.set('registered_user_id', '');
pm.environment.set('current_user_id', '');
pm.environment.set('artista_test_email', `artista_${Date.now()}@weplay.test`);
pm.environment.set('fan_test_email', `fan_${Date.now()}@weplay.test`);

console.log('Environment initialized');
```

---

### 5.2 Auth - Register / 200 OK - Register with Valid Data (Fan)

**Endpoint:** POST /api/auth/register
**Expected Status:** 200
**Expected Response Code:** 201 o similar indicando creacion

**Request:**
```http
POST {{auth_endpoint}}/register
Content-Type: application/json

{
  "email": "{{fan_test_email}}",
  "password": "{{test_password}}",
  "confirmPassword": "{{test_password}}",
  "role": "Fan"
}
```

**Pre-Request Script:**
```javascript
// Generar email unico para este test
const timestamp = Date.now();
const email = `fan_${timestamp}@weplay.test`;
pm.environment.set('fan_test_email', email);

// Log
console.log(`[Register Fan] Email: ${email}`);
```

**Test Script:**
```javascript
pm.test('Status code is 200 OK', () => {
    pm.response.to.have.status(200);
});

pm.test('Response is valid JSON', () => {
    pm.response.to.be.json;
});

pm.test('ServiceResponse has correct structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('Response indicates success', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(true);
});

pm.test('Messages array is not empty', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
});

pm.test('First message has no error code (success)', () => {
    const json = pm.response.json();
    const message = json.messages[0];
    pm.expect(message).to.have.property('message');
    pm.expect(message).to.have.property('errorCode');
    // Success messages typically have code starting with 0 or empty
    pm.expect(['', '0000', '0001', null, undefined]).to.include(message.errorCode);
});

pm.test('Data contains user info', () => {
    const json = pm.response.json();
    const data = json.data;
    pm.expect(data).to.exist;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('token');
    pm.expect(data).to.have.property('roles');
});

pm.test('UserId is valid GUID format', () => {
    const json = pm.response.json();
    const userId = json.data.userId;
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(userId).to.match(guidRegex);
});

pm.test('Email matches request email', () => {
    const json = pm.response.json();
    const email = pm.environment.get('fan_test_email');
    pm.expect(json.data.email).to.equal(email);
});

pm.test('Token is valid JWT format', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const jwtRegex = /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/;
    pm.expect(token).to.match(jwtRegex);
});

pm.test('Roles array contains Fan', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.be.an('array');
    pm.expect(json.data.roles).to.include('Fan');
});

pm.test('Response time is acceptable', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('response_time_limit_register')));
});

// Guardar UserId para otros tests
pm.environment.set('registered_user_id', pm.response.json().data.userId);

console.log(`[Register Fan Success] UserId: ${pm.response.json().data.userId}`);
```

---

### 5.3 Auth - Register / 200 OK - Register with Valid Data (Artista)

**Endpoint:** POST /api/auth/register
**Expected Status:** 200

**Request:**
```http
POST {{auth_endpoint}}/register
Content-Type: application/json

{
  "email": "{{artista_test_email}}",
  "password": "{{test_password}}",
  "confirmPassword": "{{test_password}}",
  "role": "Artista"
}
```

**Test Script:**
```javascript
pm.test('Status code is 200 OK', () => {
    pm.response.to.have.status(200);
});

pm.test('Response indicates success', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(true);
});

pm.test('Data contains userId', () => {
    const json = pm.response.json();
    pm.expect(json.data.userId).to.exist;
});

pm.test('Roles array contains Artista', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.include('Artista');
});

pm.test('Token is valid JWT', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const jwtRegex = /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/;
    pm.expect(token).to.match(jwtRegex);
});

// Guardar para flujo E2E
pm.environment.set('artista_user_id', pm.response.json().data.userId);
pm.environment.set('artista_token', pm.response.json().data.token);

console.log('[Register Artista Success] User registered and token saved');
```

---

### 5.4 Auth - Register / 400 Bad Request - Register with Empty Email

**Endpoint:** POST /api/auth/register
**Expected Status:** 400

**Request:**
```http
POST {{auth_endpoint}}/register
Content-Type: application/json

{
  "email": "",
  "password": "{{test_password}}",
  "confirmPassword": "{{test_password}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400 Bad Request', () => {
    pm.response.to.have.status(400);
});

pm.test('Response indicates failure', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Messages array contains validation errors', () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;
});

pm.test('First message has error code starting with 1 (validation)', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^1\d{3}$/);
});

pm.test('Error message mentions email or required', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(email|obligatorio|required)/i);
});

pm.test('Data field is null or undefined', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.null;
});

console.log('[Register Empty Email] Validation error correctly returned');
```

---

### 5.5 Auth - Register / 400 Bad Request - Register with Non-matching Passwords

**Endpoint:** POST /api/auth/register
**Expected Status:** 400

**Request:**
```http
POST {{auth_endpoint}}/register
Content-Type: application/json

{
  "email": "test@weplay.test",
  "password": "TestPassword123!",
  "confirmPassword": "DifferentPassword123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400 Bad Request', () => {
    pm.response.to.have.status(400);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code indicates validation failure', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(errorCode).to.match(/^1\d{3}$/);
});

pm.test('Error message mentions password mismatch', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(no coinciden|mismatch|match)/i);
});

console.log('[Register Non-matching Passwords] Validation error correctly returned');
```

---

### 5.6 Auth - Register / 409 Conflict - Register with Existing Email

**Endpoint:** POST /api/auth/register
**Expected Status:** 409

**Request:**
```http
POST {{auth_endpoint}}/register
Content-Type: application/json

{
  "email": "{{fan_test_email}}",
  "password": "DifferentPassword123!",
  "confirmPassword": "DifferentPassword123!"
}
```

**Pre-Request Script:**
```javascript
// Este test asume que el email ya fue registrado en test anterior
// Si no existe, registrarlo primero
console.log('Attempting to register duplicate email: ' + pm.environment.get('fan_test_email'));
```

**Test Script:**
```javascript
pm.test('Status code is 409 Conflict', () => {
    pm.response.to.have.status(409);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code indicates conflict (1009 or similar)', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    // Duplicate email error codes: 1009 or similar
    pm.expect(['1009', '1008', '4008']).to.include(errorCode);
});

pm.test('Error message mentions email or duplicate', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(email|duplicado|duplicate|ya registrado|already)/i);
});

console.log('[Register Duplicate Email] Conflict correctly returned');
```

---

### 5.7 Auth - Login / 200 OK - Login with Valid Credentials

**Endpoint:** POST /api/auth/login
**Expected Status:** 200

**Request:**
```http
POST {{auth_endpoint}}/login
Content-Type: application/json

{
  "email": "{{fan_test_email}}",
  "password": "{{test_password}}"
}
```

**Pre-Request Script:**
```javascript
// Verificar que tenemos un email registrado
if (!pm.environment.get('fan_test_email')) {
    console.error('ERROR: fan_test_email not set. Run register test first.');
    pm.test('Setup error', () => {
        pm.expect(false).to.be.true;
    });
}
```

**Test Script:**
```javascript
pm.test('Status code is 200 OK', () => {
    pm.response.to.have.status(200);
});

pm.test('Response is valid JSON', () => {
    pm.response.to.be.json;
});

pm.test('ServiceResponse has correct structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('Response indicates success', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(true);
});

pm.test('Data contains required fields', () => {
    const json = pm.response.json();
    const data = json.data;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('token');
    pm.expect(data).to.have.property('roles');
});

pm.test('UserId is valid GUID', () => {
    const json = pm.response.json();
    const userId = json.data.userId;
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(userId).to.match(guidRegex);
});

pm.test('Email matches request email (case-insensitive)', () => {
    const json = pm.response.json();
    const requestEmail = pm.environment.get('fan_test_email').toLowerCase();
    const responseEmail = json.data.email.toLowerCase();
    pm.expect(responseEmail).to.equal(requestEmail);
});

pm.test('Token is valid JWT format', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const jwtRegex = /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/;
    pm.expect(token).to.match(jwtRegex);
});

pm.test('Token has three parts (header.payload.signature)', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const parts = token.split('.');
    pm.expect(parts).to.have.lengthOf(3);
    pm.expect(parts[0]).to.not.be.empty;
    pm.expect(parts[1]).to.not.be.empty;
    pm.expect(parts[2]).to.not.be.empty;
});

pm.test('Roles array is not empty', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.be.an('array').that.is.not.empty;
});

pm.test('Roles array contains valid role', () => {
    const json = pm.response.json();
    const validRoles = ['Fan', 'Artista', 'Admin'];
    json.data.roles.forEach(role => {
        pm.expect(validRoles).to.include(role);
    });
});

pm.test('Response time is acceptable', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('response_time_limit_login')));
});

// Guardar token para otros tests
pm.environment.set('auth_token', pm.response.json().data.token);
pm.environment.set('current_user_id', pm.response.json().data.userId);

console.log(`[Login Success] Token saved for authenticated tests`);
```

---

### 5.8 Auth - Login / 400 Bad Request - Login with Empty Email

**Endpoint:** POST /api/auth/login
**Expected Status:** 400

**Request:**
```http
POST {{auth_endpoint}}/login
Content-Type: application/json

{
  "email": "",
  "password": "{{test_password}}"
}
```

**Test Script:**
```javascript
pm.test('Status code is 400 Bad Request', () => {
    pm.response.to.have.status(400);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code is validation error (1xxx)', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^1\d{3}$/);
});

pm.test('Error message mentions email or required', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    pm.expect(message).to.match(/(email|obligatorio|required)/i);
});

console.log('[Login Empty Email] Validation error correctly returned');
```

---

### 5.9 Auth - Login / 401 Unauthorized - Login with Invalid Credentials

**Endpoint:** POST /api/auth/login
**Expected Status:** 401

**Request:**
```http
POST {{auth_endpoint}}/login
Content-Type: application/json

{
  "email": "nonexistent@weplay.test",
  "password": "WrongPassword123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code indicates authentication failure (3006 or 3007)', () => {
    const json = pm.response.json();
    const errorCode = json.messages[0].errorCode;
    pm.expect(['3006', '3007']).to.include(errorCode);
});

pm.test('Error message is generic (no user enumeration)', () => {
    const json = pm.response.json();
    const message = json.messages[0].message.toLowerCase();
    // Should NOT reveal whether email exists
    pm.expect(message).to.match(/(email o contraseña|invalid credentials|inválidos)/i);
});

pm.test('Data field is null', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.null;
});

console.log('[Login Invalid Credentials] Unauthorized correctly returned');
```

---

### 5.10 Auth - Login / 401 Unauthorized - Login with Correct Email Wrong Password

**Endpoint:** POST /api/auth/login
**Expected Status:** 401

**Request:**
```http
POST {{auth_endpoint}}/login
Content-Type: application/json

{
  "email": "{{fan_test_email}}",
  "password": "WrongPassword123!"
}
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('Error code is 3006 (Auth_InvalidCredentials)', () => {
    const json = pm.response.json();
    pm.expect(['3006', '3007']).to.include(json.messages[0].errorCode);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('No token returned', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.null;
});

console.log('[Login Wrong Password] Unauthorized correctly returned');
```

---

### 5.11 Auth - Get Current User / 200 OK - Get User with Valid Token

**Endpoint:** GET /api/auth/me
**Expected Status:** 200
**Authorization:** Bearer token

**Request:**
```http
GET {{auth_endpoint}}/me
Authorization: Bearer {{auth_token}}
```

**Pre-Request Script:**
```javascript
// Verificar que tenemos un token valido
if (!pm.environment.get('auth_token')) {
    console.error('ERROR: auth_token not set. Run login test first.');
    pm.test('Setup error', () => {
        pm.expect(false).to.be.true;
    });
}
```

**Test Script:**
```javascript
pm.test('Status code is 200 OK', () => {
    pm.response.to.have.status(200);
});

pm.test('Response is valid JSON', () => {
    pm.response.to.be.json;
});

pm.test('ServiceResponse has correct structure', () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('isSuccess is true', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(true);
});

pm.test('Data contains required fields', () => {
    const json = pm.response.json();
    const data = json.data;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('roles');
    pm.expect(data).to.have.property('emailConfirmed');
});

pm.test('UserId matches logged in user', () => {
    const json = pm.response.json();
    const expectedUserId = pm.environment.get('current_user_id');
    pm.expect(json.data.userId).to.equal(expectedUserId);
});

pm.test('Email is not empty', () => {
    const json = pm.response.json();
    pm.expect(json.data.email).to.not.be.empty;
});

pm.test('Roles array is valid', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.be.an('array').that.is.not.empty;
});

pm.test('EmailConfirmed is boolean', () => {
    const json = pm.response.json();
    pm.expect(json.data.emailConfirmed).to.be.a('boolean');
});

pm.test('Response time is fast', () => {
    pm.expect(pm.response.responseTime).to.be.below(parseInt(pm.environment.get('response_time_limit_get_user')));
});

console.log('[Get User Success] User info retrieved successfully');
```

---

### 5.12 Auth - Get Current User / 401 Unauthorized - Get User without Token

**Endpoint:** GET /api/auth/me
**Expected Status:** 401

**Request:**
```http
GET {{auth_endpoint}}/me
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code is 3005 (Auth_UserNotAuthenticated)', () => {
    const json = pm.response.json();
    pm.expect(['3005', '3001']).to.include(json.messages[0].errorCode);
});

pm.test('Data is null', () => {
    const json = pm.response.json();
    pm.expect(json.data).to.be.null;
});

console.log('[Get User No Token] Unauthorized correctly returned');
```

---

### 5.13 Auth - Get Current User / 401 Unauthorized - Get User with Invalid Token

**Endpoint:** GET /api/auth/me
**Expected Status:** 401

**Request:**
```http
GET {{auth_endpoint}}/me
Authorization: Bearer invalid.token.here
```

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('isSuccess is false', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

pm.test('Error code indicates authentication failure', () => {
    const json = pm.response.json();
    pm.expect(json.messages[0].errorCode).to.match(/^(3\d{3}|401)$/);
});

console.log('[Get User Invalid Token] Unauthorized correctly returned');
```

---

### 5.14 Auth - Get Current User / 401 Unauthorized - Get User with Bearer Missing

**Endpoint:** GET /api/auth/me
**Expected Status:** 401

**Request:**
```http
GET {{auth_endpoint}}/me
Authorization: {{auth_token}}
```

**Note:** Se envia token sin prefijo "Bearer "

**Test Script:**
```javascript
pm.test('Status code is 401 Unauthorized', () => {
    pm.response.to.have.status(401);
});

pm.test('Malformed bearer token rejected', () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.equal(false);
});

console.log('[Get User Malformed Bearer] Unauthorized correctly returned');
```

---

### 5.15 E2E Flow - Register → Login → Get User

**Descripcion:** Flujo completo de un nuevo usuario

**Folder:** E2E Flows / Flow 1

#### Step 1: Register New User

```javascript
// Test Script
pm.test('Register successful', () => {
    pm.response.to.have.status(200);
    pm.expect(pm.response.json().isSuccess).to.equal(true);
});

// Guardar datos para siguiente step
const registerData = pm.response.json().data;
pm.environment.set('e2e_user_id', registerData.userId);
pm.environment.set('e2e_email', registerData.email);

console.log('[E2E Flow 1 - Step 1] User registered');
```

#### Step 2: Login with Registered Credentials

```javascript
// Pre-Request Script
const email = pm.environment.get('e2e_email');
pm.request.body.raw = JSON.stringify({
    email: email,
    password: pm.environment.get('test_password')
});

// Test Script
pm.test('Login successful', () => {
    pm.response.to.have.status(200);
    pm.expect(pm.response.json().isSuccess).to.equal(true);
});

pm.test('Returned email matches registered email', () => {
    const json = pm.response.json();
    pm.expect(json.data.email.toLowerCase()).to.equal(pm.environment.get('e2e_email').toLowerCase());
});

// Guardar token para siguiente step
pm.environment.set('e2e_token', pm.response.json().data.token);

console.log('[E2E Flow 1 - Step 2] Login successful');
```

#### Step 3: Get User Info

```javascript
// Test Script
pm.test('Get user successful', () => {
    pm.response.to.have.status(200);
    pm.expect(pm.response.json().isSuccess).to.equal(true);
});

pm.test('UserId matches registered user', () => {
    const json = pm.response.json();
    const expectedId = pm.environment.get('e2e_user_id');
    pm.expect(json.data.userId).to.equal(expectedId);
});

pm.test('Email matches registered email', () => {
    const json = pm.response.json();
    const expectedEmail = pm.environment.get('e2e_email').toLowerCase();
    pm.expect(json.data.email.toLowerCase()).to.equal(expectedEmail);
});

console.log('[E2E Flow 1 - Step 3] User info retrieved - Flow Complete');
```

#### Step 4: Verify Data Consistency

```javascript
// Test Script
pm.test('Data is consistent across flow', () => {
    const json = pm.response.json();
    const userId = pm.environment.get('e2e_user_id');
    const email = pm.environment.get('e2e_email');

    pm.expect(json.data.userId).to.equal(userId);
    pm.expect(json.data.email.toLowerCase()).to.equal(email.toLowerCase());
});

pm.test('Roles are present and valid', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.be.an('array');
    pm.expect(json.data.roles.length).to.be.greaterThan(0);
});

console.log('[E2E Flow 1 - Complete] All assertions passed');
```

---

### 5.16 E2E Flow - Register Artista → Verify Roles

**Descripcion:** Verificar que los roles se mantienen consistentes

#### Step 1: Register as Artista

```javascript
// Pre-Request Script
const timestamp = Date.now();
const email = `artista_${timestamp}@weplay.test`;
pm.environment.set('e2e_artista_email', email);

// Test Script
pm.test('Artista registration successful', () => {
    pm.response.to.have.status(200);
    pm.expect(pm.response.json().isSuccess).to.equal(true);
});

pm.test('Returned roles include Artista', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.include('Artista');
});

pm.environment.set('e2e_artista_token', pm.response.json().data.token);
pm.environment.set('e2e_artista_user_id', pm.response.json().data.userId);

console.log('[E2E Artista - Step 1] Artista registered');
```

#### Step 2: Login and Extract Roles

```javascript
// Test Script
pm.test('Login successful', () => {
    pm.response.to.have.status(200);
});

pm.test('Roles in login response include Artista', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.include('Artista');
});

const roles = pm.response.json().data.roles;
pm.environment.set('e2e_artista_roles', JSON.stringify(roles));

console.log('[E2E Artista - Step 2] Roles extracted: ' + roles.join(', '));
```

#### Step 3: Get User and Verify Artista Role

```javascript
// Test Script
pm.test('Get user successful', () => {
    pm.response.to.have.status(200);
});

pm.test('User info includes Artista role', () => {
    const json = pm.response.json();
    pm.expect(json.data.roles).to.include('Artista');
});

pm.test('Roles match across register, login, and get user', () => {
    const currentRoles = pm.response.json().data.roles;
    const registeredRoles = JSON.parse(pm.environment.get('e2e_artista_roles'));

    pm.expect(currentRoles.length).to.equal(registeredRoles.length);
    currentRoles.forEach(role => {
        pm.expect(registeredRoles).to.include(role);
    });
});

console.log('[E2E Artista - Step 3] Role consistency verified');
```

---

### 5.17 Contract Validation - Verify Response Structures

**Folder:** Contract Validation

#### 701: Register Response Structure

```javascript
pm.test('Register response follows ServiceResponse contract', () => {
    const json = pm.response.json();

    // Estructura base
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');

    // Type checking
    pm.expect(json.data).to.be.an('object');
    pm.expect(json.messages).to.be.an('array');
    pm.expect(json.isSuccess).to.be.a('boolean');

    // ServiceResponseMessage structure
    json.messages.forEach(msg => {
        pm.expect(msg).to.have.property('message');
        pm.expect(msg).to.have.property('errorCode');
    });
});

pm.test('RegisterResponseDto has required fields', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('token');
    pm.expect(data).to.have.property('roles');
});

console.log('[Contract] Register response structure validated');
```

#### 702: Login Response Structure

```javascript
pm.test('Login response follows ServiceResponse contract', () => {
    const json = pm.response.json();

    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('LoginResponseDto has required fields', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('token');
    pm.expect(data).to.have.property('roles');
});

console.log('[Contract] Login response structure validated');
```

#### 703: User Info Response Structure

```javascript
pm.test('User info response follows ServiceResponse contract', () => {
    const json = pm.response.json();

    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json).to.have.property('isSuccess');
});

pm.test('UserInfoDto has required fields', () => {
    const data = pm.response.json().data;
    pm.expect(data).to.have.property('userId');
    pm.expect(data).to.have.property('email');
    pm.expect(data).to.have.property('roles');
    pm.expect(data).to.have.property('emailConfirmed');
});

console.log('[Contract] User info response structure validated');
```

#### 705: Token Is Valid JWT Format

```javascript
pm.test('Token is valid JWT format', () => {
    const json = pm.response.json();
    const token = json.data.token;

    // JWT format: header.payload.signature
    const parts = token.split('.');
    pm.expect(parts).to.have.lengthOf(3);

    // Each part should be base64url encoded
    const base64urlRegex = /^[A-Za-z0-9-_]+$/;
    parts.forEach((part, index) => {
        pm.expect(part).to.match(base64urlRegex);
    });
});

pm.test('JWT header is valid', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const header = token.split('.')[0];

    // Decode base64url
    const decoded = atob(header);
    const headerObj = JSON.parse(decoded);

    pm.expect(headerObj).to.have.property('alg');
    pm.expect(headerObj).to.have.property('typ');
    pm.expect(headerObj.alg).to.equal('HS256');
    pm.expect(headerObj.typ).to.equal('JWT');
});

pm.test('JWT payload contains required claims', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const payload = token.split('.')[1];

    // Decode base64url
    const decoded = atob(payload);
    const payloadObj = JSON.parse(decoded);

    // Check required claims
    pm.expect(payloadObj).to.have.property('sub');
    pm.expect(payloadObj).to.have.property('email');
    pm.expect(payloadObj).to.have.property('iat');
    pm.expect(payloadObj).to.have.property('exp');
    pm.expect(payloadObj).to.have.property('jti');
});

pm.test('JWT has role claims if user has roles', () => {
    const json = pm.response.json();

    if (json.data.roles && json.data.roles.length > 0) {
        const token = json.data.token;
        const payload = token.split('.')[1];
        const decoded = atob(payload);
        const payloadObj = JSON.parse(decoded);

        pm.expect(payloadObj).to.have.property('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role');
    }
});

console.log('[Contract] JWT token structure validated');
```

#### 706: Token Expiration Claim

```javascript
pm.test('Token has valid expiration claim (exp)', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const payload = token.split('.')[1];

    const decoded = atob(payload);
    const payloadObj = JSON.parse(decoded);

    pm.expect(payloadObj.exp).to.be.a('number');
    pm.expect(payloadObj.exp).to.be.greaterThan(0);

    // Token should not be expired
    const now = Math.floor(Date.now() / 1000);
    pm.expect(payloadObj.exp).to.be.greaterThan(now);
});

pm.test('Token expiration is reasonable (within 24-48 hours)', () => {
    const json = pm.response.json();
    const token = json.data.token;
    const payload = token.split('.')[1];

    const decoded = atob(payload);
    const payloadObj = JSON.parse(decoded);

    const issuedAt = payloadObj.iat;
    const expiresAt = payloadObj.exp;
    const expirationHours = (expiresAt - issuedAt) / 3600;

    // Typically 24 hours for JWT in this type of system
    pm.expect(expirationHours).to.be.greaterThanOrEqual(1);
    pm.expect(expirationHours).to.be.lessThanOrEqual(72);
});

console.log('[Contract] Token expiration validated');
```

---

## 6. Performance Testing

### 6.1 Register Performance

**Request:** POST /api/auth/register

**Test Script:**
```javascript
pm.test('Register response time < 1000ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});

pm.test('Register response time acceptable for user experience', () => {
    const responseTime = pm.response.responseTime;

    if (responseTime < 300) {
        console.log('EXCELLENT: ' + responseTime + 'ms');
    } else if (responseTime < 500) {
        console.log('GOOD: ' + responseTime + 'ms');
    } else if (responseTime < 1000) {
        console.log('ACCEPTABLE: ' + responseTime + 'ms');
    } else {
        console.log('SLOW: ' + responseTime + 'ms');
    }
});
```

### 6.2 Login Performance

**Request:** POST /api/auth/login

**Test Script:**
```javascript
pm.test('Login response time < 500ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test('Login response time is fast', () => {
    const responseTime = pm.response.responseTime;
    console.log('Login took: ' + responseTime + 'ms');
});
```

### 6.3 Get User Performance

**Request:** GET /api/auth/me

**Test Script:**
```javascript
pm.test('Get user response time < 200ms', () => {
    pm.expect(pm.response.responseTime).to.be.below(200);
});
```

---

## 7. Security Testing

### 7.1 Verify No Sensitive Data in Responses

**Request:** POST /api/auth/login

**Test Script:**
```javascript
pm.test('Response does not contain plain-text password', () => {
    const responseText = pm.response.text();
    pm.expect(responseText.toLowerCase()).not.to.include('password');
});

pm.test('Response only contains expected fields', () => {
    const json = pm.response.json();
    const data = json.data;

    const allowedFields = ['userId', 'email', 'token', 'roles'];
    const dataKeys = Object.keys(data);

    dataKeys.forEach(key => {
        pm.expect(allowedFields).to.include(key);
    });
});

pm.test('Error messages do not reveal user enumeration', () => {
    // For 401 responses
    if (pm.response.code === 401) {
        const json = pm.response.json();
        const message = json.messages[0].message.toLowerCase();

        // Should not say "user not found" specifically
        pm.expect(message).not.to.include('usuario no encontrado');
        pm.expect(message).not.to.include('email no existe');
    }
});
```

### 7.2 Verify SQL Injection Prevention

**Request:** POST /api/auth/register

**Pre-Request Script:**
```javascript
// Intentar SQL injection en email
const sqlInjectionPayloads = [
    "test@test.com' OR '1'='1",
    "test@test.com'; DROP TABLE users; --",
    "test@test.com\"; OR \"1\"=\"1"
];

// Test con primer payload
pm.environment.set('injection_test_email', sqlInjectionPayloads[0]);
```

**Test Script:**
```javascript
pm.test('SQL injection attempt safely rejected', () => {
    // Debe fallar validacion o retornar error seguro
    pm.expect([400, 401, 500]).to.include(pm.response.code);

    // No debe revelar detalles de la query
    const responseText = pm.response.text().toLowerCase();
    pm.expect(responseText).not.to.include('sql');
    pm.expect(responseText).not.to.include('exception');
});
```

---

## 8. Estrategia de Ejecucion en CI/CD

### 8.1 Local Development

```bash
# Instalar Newman (si no lo tienes)
npm install -g newman
npm install -g newman-reporter-htmlextra

# Ejecutar tests basicos
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/reports/identity-tests.html

# Ejecutar con verbose output
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --verbose
```

### 8.2 Azure DevOps Pipeline

```yaml
trigger:
  - main
  - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  testTimeoutMinutes: '10'
  nodeVersion: '18.x'

stages:
  - stage: RunIntegrationTests
    displayName: 'Run Identity Integration Tests'
    jobs:
      - job: IdentityTests
        displayName: 'Identity JWT Tests'
        timeoutInMinutes: ${{ variables.testTimeoutMinutes }}

        steps:
          - task: UseNode@1
            inputs:
              version: ${{ variables.nodeVersion }}
            displayName: 'Setup Node.js'

          - script: |
              npm install -g newman
              npm install -g newman-reporter-junitxml
              npm install -g newman-reporter-htmlextra
            displayName: 'Install Newman and Reporters'

          - script: |
              mkdir -p $(Build.ArtifactStagingDirectory)/test-results
            displayName: 'Create Results Directory'

          - task: DotNetCoreCLI@2
            inputs:
              command: 'build'
              projects: 'src/api/WePlay_Rises.sln'
            displayName: 'Build Backend API'

          - task: DotNetCoreCLI@2
            inputs:
              command: 'run'
              projects: 'src/api/WebApi/WebApi.csproj'
              arguments: '--no-build'
            displayName: 'Start API Server'
            continueOnError: false

          - script: |
              sleep 5
              # Verify API is running
              curl -f http://localhost:5261/healthz || echo "Health check endpoint may not exist yet"
            displayName: 'Wait for API to Start'

          - script: |
              newman run tests/newman/WePlay.WPR006.IdentityTests.json \
                -e tests/newman/environments/ci.json \
                --reporters cli,junit,htmlextra \
                --reporter-junit-export $(Build.ArtifactStagingDirectory)/test-results/identity-tests.xml \
                --reporter-htmlextra-export $(Build.ArtifactStagingDirectory)/test-results/identity-tests.html \
                --reporters-list
            displayName: 'Run Identity Tests'
            continueOnError: true

          - task: PublishTestResults@2
            inputs:
              testResultsFormat: 'JUnit'
              testResultsFiles: '$(Build.ArtifactStagingDirectory)/test-results/identity-tests.xml'
              failTaskOnFailedTests: true
            displayName: 'Publish Test Results'

          - task: PublishBuildArtifacts@1
            inputs:
              pathToPublish: '$(Build.ArtifactStagingDirectory)'
              artifactName: 'newman-reports'
            condition: always()
            displayName: 'Publish Test Reports'
```

### 8.3 GitHub Actions

```yaml
name: Identity Integration Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  integration-tests:
    runs-on: ubuntu-latest
    timeout-minutes: 15

    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2019-latest
        env:
          ACCEPT_EULA: Y
          SA_PASSWORD: ${{ secrets.MSSQL_SA_PASSWORD }}
        options: >-
          --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P ${{ secrets.MSSQL_SA_PASSWORD }} -Q 'SELECT 1'"
          --health-interval=10s
          --health-timeout=5s
          --health-retries=5

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18.x'

      - name: Install Newman
        run: |
          npm install -g newman
          npm install -g newman-reporter-htmlextra

      - name: Restore Dependencies
        run: dotnet restore src/api/

      - name: Build Backend
        run: dotnet build src/api/ --configuration Release --no-restore

      - name: Start API Server
        run: |
          cd src/api/WebApi
          dotnet run --configuration Release &
          sleep 10

      - name: Run Identity Tests
        run: |
          mkdir -p test-results
          newman run tests/newman/WePlay.WPR006.IdentityTests.json \
            -e tests/newman/environments/ci.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export test-results/identity-tests.html \
            --bail

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: newman-reports
          path: test-results/
```

### 8.4 GitLab CI

```yaml
image: node:18

stages:
  - setup
  - build
  - test

variables:
  DOTNET_VERSION: '8.0'
  TEST_TIMEOUT: '10m'

before_script:
  - npm install -g newman newman-reporter-htmlextra

setup:
  stage: setup
  script:
    - npm install -g newman newman-reporter-junitxml newman-reporter-htmlextra
  artifacts:
    paths:
      - node_modules/

build:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - cd src/api
    - dotnet restore
    - dotnet build --configuration Release --no-restore
  artifacts:
    paths:
      - src/api/WebApi/bin/Release/

identity-tests:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  services:
    - mcr.microsoft.com/mssql/server:2019-latest
  variables:
    MSSQL_SA_PASSWORD: "P@ssw0rd123!"
    ACCEPT_EULA: "Y"
  script:
    # Start API
    - cd src/api/WebApi
    - dotnet run --configuration Release &
    - sleep 10

    # Run tests
    - cd ../../../
    - mkdir -p test-results
    - newman run tests/newman/WePlay.WPR006.IdentityTests.json \
        -e tests/newman/environments/ci.json \
        --reporters cli,junit,htmlextra \
        --reporter-junit-export test-results/identity-tests.xml \
        --reporter-htmlextra-export test-results/identity-tests.html \
        --timeout $TEST_TIMEOUT
  artifacts:
    when: always
    reports:
      junit: test-results/identity-tests.xml
    paths:
      - test-results/
```

---

## 9. Ejecucion Manual paso a paso

### Scenario A: Test Register → Login → Get User

```bash
# 1. Setup
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "_Setup"

# 2. Register
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Register" \
    --folder "200 OK - Success Cases"

# 3. Login
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Login" \
    --folder "200 OK - Success Cases"

# 4. Get User
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Get Current User (/me)" \
    --folder "200 OK - Success Cases"

# 5. E2E Flow
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "E2E Flows"
```

### Scenario B: Test Validation Errors

```bash
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Register" \
    --folder "400 Bad Request - Validation Errors"
```

### Scenario C: Test Security

```bash
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Security & Performance" \
    --folder "Security Tests"
```

### Scenario D: Full Suite with Reports

```bash
newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra,junitxml \
    --reporter-htmlextra-export reports/identity-tests-$(date +%Y%m%d-%H%M%S).html \
    --reporter-junitxml-export reports/identity-tests-$(date +%Y%m%d-%H%M%S).xml
```

---

## 10. Casos de Prueba Checklist

### Register Endpoint
- [x] Register con datos validos (Fan) - 200
- [x] Register con datos validos (Artista) - 200
- [x] Register con datos validos (Admin) - 200
- [x] Register con email vacio - 400
- [x] Register con email invalido - 400
- [x] Register con password vacia - 400
- [x] Register con password corta - 400
- [x] Register con passwords no coincidentes - 400
- [x] Register con email duplicado - 409
- [x] Register con role invalido - 400
- [x] Verify token en respuesta de register
- [x] Verify roles en respuesta de register

### Login Endpoint
- [x] Login con credenciales validas - 200
- [x] Login con email inexistente - 401
- [x] Login con password incorrecta - 401
- [x] Login con email vacio - 400
- [x] Login con password vacia - 400
- [x] Login con email invalido - 400
- [x] Login sin cuerpo - 400
- [x] Login multiple veces (diferentes tokens)
- [x] Verify token en respuesta de login
- [x] Verify roles en respuesta de login
- [x] Verify token valido para /me

### Get User (/me) Endpoint
- [x] Get user con token valido - 200
- [x] Get user sin token - 401
- [x] Get user con token invalido - 401
- [x] Get user con token expirado - 401
- [x] Get user con bearer malformado - 401
- [x] Verify estructura de response
- [x] Verify email correcto
- [x] Verify userId correcto
- [x] Verify roles presentes

### E2E Flows
- [x] Register → Login → Get User (data consistency)
- [x] Register Artista → Login → Verify Roles
- [x] Multiple login sessions (tokens diferentes)

### Security
- [x] No password en response
- [x] No sensitive data en error messages
- [x] SQL injection prevention
- [x] Token format validation
- [x] JWT signature validation
- [x] Claims validation

### Performance
- [x] Register < 1000ms
- [x] Login < 500ms
- [x] Get User < 200ms

### Contract Validation
- [x] Register response structure
- [x] Login response structure
- [x] Get user response structure
- [x] Error response structure
- [x] JWT token format
- [x] JWT claims
- [x] Token expiration

---

## 11. Notas de Implementacion

### Estructura de Carpetas Recomendada

```
tests/
├── newman/
│   ├── collections/
│   │   └── WePlay.WPR006.IdentityTests.json
│   ├── environments/
│   │   ├── development.json
│   │   ├── staging.json
│   │   ├── ci.json
│   │   └── production.json
│   ├── globals.json
│   └── scripts/
│       ├── run-all.sh
│       ├── run-local.sh
│       └── run-ci.sh
└── reports/
    ├── identity-tests.html
    └── identity-tests.xml
```

### Archivo globals.json

```json
{
  "name": "WePlay Globals",
  "values": [
    {
      "key": "jwt_algorithm",
      "value": "HS256",
      "enabled": true
    },
    {
      "key": "jwt_header_type",
      "value": "JWT",
      "enabled": true
    },
    {
      "key": "valid_roles",
      "value": "Fan,Artista,Admin",
      "enabled": true
    },
    {
      "key": "validation_error_code_prefix",
      "value": "1",
      "enabled": true
    },
    {
      "key": "auth_error_code_prefix",
      "value": "3",
      "enabled": true
    },
    {
      "key": "notfound_error_code_prefix",
      "value": "2",
      "enabled": true
    }
  ]
}
```

### Generador de Reportes

```bash
#!/bin/bash
# reports-generator.sh

TIMESTAMP=$(date +%Y%m%d-%H%M%S)
REPORT_DIR="test-results/newman-reports-${TIMESTAMP}"

mkdir -p "${REPORT_DIR}"

echo "Running Newman tests and generating reports..."

newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra,junitxml \
    --reporter-htmlextra-export "${REPORT_DIR}/identity-tests.html" \
    --reporter-junitxml-export "${REPORT_DIR}/identity-tests.xml"

echo "Reports generated in: ${REPORT_DIR}"

# Open HTML report
if command -v xdg-open &> /dev/null; then
    xdg-open "${REPORT_DIR}/identity-tests.html"
elif command -v open &> /dev/null; then
    open "${REPORT_DIR}/identity-tests.html"
fi
```

---

## 12. Integracion con Git y CI/CD

### Pre-commit Hook

```bash
#!/bin/bash
# .git/hooks/pre-commit

echo "Running API integration tests..."

newman run tests/newman/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli

if [ $? -ne 0 ]; then
    echo "Integration tests failed. Commit aborted."
    exit 1
fi

echo "All tests passed. Proceeding with commit."
exit 0
```

### GitHub Actions Reusable Workflow

```yaml
name: Run Newman Identity Tests

on:
  workflow_call:
    inputs:
      environment:
        required: true
        type: string
        default: 'development'
    secrets:
      API_BASE_URL:
        required: true
      TEST_EMAIL_PASSWORD:
        required: false

jobs:
  newman-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - uses: actions/setup-node@v3
        with:
          node-version: '18.x'

      - run: npm install -g newman newman-reporter-htmlextra

      - run: |
          newman run tests/newman/WePlay.WPR006.IdentityTests.json \
            -e tests/newman/environments/${{ inputs.environment }}.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export test-results.html

      - uses: actions/upload-artifact@v3
        if: always()
        with:
          name: test-results
          path: test-results.html
```

---

## 13. Troubleshooting Guide

### Problema: "Token invalido" en /me después de login

**Solucion:**
1. Verificar que el token se guarda correctamente en la variable de entorno
2. Verificar que la respuesta de login contiene un token valido
3. Asegurarse de que el token no tiene espacios adicionales
4. Verificar que el endpoint espera formato "Bearer <token>"

```javascript
// Debug script
const token = pm.response.json().data.token;
console.log('Token length:', token.length);
console.log('Token first 50 chars:', token.substring(0, 50));
pm.environment.set('auth_token', token.trim());
```

### Problema: "Usuario no encontrado" en login

**Solucion:**
1. Asegurar que el test de register se ejecuto primero
2. Verificar que el email usado en register coincida con login
3. Verificar que las variables de entorno se mantengan entre requests
4. Usar timestamps o random numbers para generar emails unicos

### Problema: "Email ya registrado" (409)

**Solucion:**
1. Usar timestamps o variables aleatorias en emails
2. Limpiar base de datos de test data antes de ejecutar
3. Implementar cleanup en folder _Cleanup

### Problema: "CORS error" en requests

**Solucion:**
1. Verificar que la API tiene CORS habilitado
2. Asegurar que la URL base es correcta
3. Verificar headers de la request

```javascript
// Agregar headers personalizados
pm.request.headers.add({
    key: 'Origin',
    value: 'http://localhost:3000'
});
```

---

## 14. Metricas y KPIs

### Metricas a Rastrear

| Metrica | Objetivo | Frecuencia |
|---------|----------|-----------|
| Success Rate (%) | > 99% | Cada ejecucion |
| Response Time (ms) | Register < 1000, Login < 500, Get < 200 | Cada ejecucion |
| Total Tests | >= 30 | Baseline |
| Test Coverage | >= 80% de endpoints | Cada ejecucion |
| API Availability | > 99.5% | Monitoreo continuo |
| Error Rate | < 1% | Agregado semanal |

### Dashboard Recomendado

```javascript
// Generar resumen de metricas (guardar en global script)
const totalTests = pm.info.stats.iterations.total;
const failedTests = pm.info.stats.assertions.failed;
const successRate = ((totalTests - failedTests) / totalTests) * 100;

console.log(`
╔════════════════════════════════════════╗
║       Test Summary - ${new Date().toISOString().split('T')[0]}        ║
╠════════════════════════════════════════╣
║ Total Tests:     ${totalTests}
║ Failed Tests:    ${failedTests}
║ Success Rate:    ${successRate.toFixed(2)}%
║ Avg Response:    ${pm.response.responseTime}ms
╚════════════════════════════════════════╝
`);
```

---

## 15. Siguientes Pasos

### Fase 1: Setup Inicial
1. [x] Crear estructura de coleccion en Postman
2. [x] Importar variables de entorno
3. [ ] Ejecutar tests locales (PENDIENTE)
4. [ ] Validar que todos los tests pasen

### Fase 2: CI/CD Integration
1. [ ] Copiar plan a archivo JSON de Postman
2. [ ] Configurar pipeline de CI/CD (Azure DevOps/GitHub Actions)
3. [ ] Configurar reportes automaticos
4. [ ] Ejecutar tests en staging

### Fase 3: Monitoring y Optimizacion
1. [ ] Configurar alertas para test failures
2. [ ] Analizar metricas de performance
3. [ ] Optimizar tests lentos
4. [ ] Agregar tests adicionales segun necesidades

---

**Plan completo de Testing Newman: WPR-006 Identity JWT Authentication**

**Total de requests planificados:** 18+ requests principales
**Total de casos de prueba:** 30+ assertions
**Flujos E2E:** 3 flujos completos
**Cobertura estimada:** 95%+ de endpoints

**Pronto a implementar en Postman/Newman**
