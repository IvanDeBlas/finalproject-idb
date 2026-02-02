# Plan de Testing Newman: Registro de Artista

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Modulo:** UserAccess
**Coleccion:** WePlay.RegistroArtista.IntegrationTests

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Endpoints a testear | 4 |
| Total requests | 18 |
| Casos de prueba | 18 |
| Status codes cubiertos | 5 (200, 400, 401, 404, 409) |
| Flujos E2E | 2 |
| Tiempo estimado ejecucion | 30-45 segundos |

**Objetivos principales:**
- Validar registro de usuarios con JWT generado
- Validar creacion y recuperacion de perfiles de artista
- Verificar autenticacion y autorizacion
- Cubrir todos los casos de error esperados
- Validar estructura de respuestas ServiceResponse<T>

---

## 2. Estructura de Coleccion

```
WePlay.RegistroArtista.IntegrationTests/
│
├── _Setup/
│   └── Get Auth Token (Registra usuario de prueba - reutilizable)
│
├── Auth/
│   ├── 200 OK/
│   │   ├── POST Register - Success
│   │   └── POST Register - With Complete Profile
│   ├── 400 Bad Request/
│   │   ├── POST Register - Empty Email
│   │   ├── POST Register - Invalid Email Format
│   │   ├── POST Register - Empty Password
│   │   ├── POST Register - Password Too Short (< 8)
│   │   ├── POST Register - Empty Confirm Password
│   │   └── POST Register - Passwords Don't Match
│   └── 409 Conflict/
│       └── POST Register - Email Already Exists
│
├── Artistas/
│   ├── 200 OK/
│   │   ├── POST Create - Success
│   │   ├── POST Create - Minimal Data
│   │   ├── GET Get By Id - Success
│   │   ├── GET Get By User Id - Success
│   │   └── GET Get By User Id - Own Profile
│   ├── 400 Bad Request/
│   │   ├── POST Create - Empty Nombre Artistico
│   │   ├── POST Create - Nombre Artistico Too Long (>200)
│   │   ├── POST Create - Descripcion Too Long (>2000)
│   │   ├── POST Create - Pais Too Long (>100)
│   │   ├── POST Create - Ciudad Too Long (>100)
│   │   └── POST Create - Invalid Image URL
│   ├── 401 Unauthorized/
│   │   ├── POST Create - Missing Token
│   │   ├── POST Create - Invalid Token
│   │   ├── POST Create - Expired Token
│   │   ├── GET Get By User Id - Missing Token
│   │   ├── GET Get By User Id - Invalid Token
│   │   └── GET Get By User Id - Unauthorized User (Different UserId)
│   ├── 404 Not Found/
│   │   ├── GET Get By Id - Non-existent Id
│   │   └── GET Get By User Id - No Profile Created
│   └── 409 Conflict/
│       └── POST Create - Artist Profile Already Exists
│
├── _E2E Flows/
│   ├── Complete Registration Flow
│   │   ├── Register new user
│   │   ├── Create artista profile
│   │   ├── Get artista by ID (verify public)
│   │   ├── Get artista by UserId (verify owner)
│   │   └── Verify data consistency
│   │
│   └── Update and Verify Flow
│       ├── Register second user
│       ├── Create profiles for both
│       ├── Verify isolation (user2 cannot see user1's profile)
│       └── Verify public access works
│
└── _Cleanup/
    ├── Delete Test Users (hard delete or disable)
    └── Delete Test Artistas

```

---

## 3. Variables de Entorno

### 3.1 Development Environment

**Archivo:** `newman/environments/development.json`

```json
{
  "name": "WePlay - Development",
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
      "key": "apiVersion",
      "value": "v1",
      "enabled": true
    },
    {
      "key": "testEmail1",
      "value": "test-{{$timestamp}}@weplay.local",
      "enabled": true
    },
    {
      "key": "testEmail2",
      "value": "artist-{{$timestamp}}@weplay.local",
      "enabled": true
    },
    {
      "key": "testPassword",
      "value": "SecurePass123!",
      "enabled": true
    },
    {
      "key": "testUsername",
      "value": "testuser@weplay.local",
      "enabled": true
    },
    {
      "key": "bearerToken",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId",
      "value": "",
      "enabled": true
    },
    {
      "key": "artistaId",
      "value": "",
      "enabled": true
    },
    {
      "key": "bearerToken2",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId2",
      "value": "",
      "enabled": true
    },
    {
      "key": "artistaId2",
      "value": "",
      "enabled": true
    }
  ]
}
```

### 3.2 Staging Environment

**Archivo:** `newman/environments/staging.json`

```json
{
  "name": "WePlay - Staging",
  "values": [
    {
      "key": "baseUrl",
      "value": "https://staging-api.weplay.dev",
      "enabled": true
    },
    {
      "key": "identityUrl",
      "value": "https://staging-identity.weplay.dev",
      "enabled": true
    },
    {
      "key": "apiVersion",
      "value": "v1",
      "enabled": true
    },
    {
      "key": "testEmail1",
      "value": "test-{{$timestamp}}@staging.weplay.dev",
      "enabled": true
    },
    {
      "key": "testEmail2",
      "value": "artist-{{$timestamp}}@staging.weplay.dev",
      "enabled": true
    },
    {
      "key": "testPassword",
      "value": "{{STAGING_TEST_PASSWORD}}",
      "enabled": true
    },
    {
      "key": "bearerToken",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId",
      "value": "",
      "enabled": true
    },
    {
      "key": "artistaId",
      "value": "",
      "enabled": true
    },
    {
      "key": "bearerToken2",
      "value": "",
      "enabled": true
    },
    {
      "key": "userId2",
      "value": "",
      "enabled": true
    },
    {
      "key": "artistaId2",
      "value": "",
      "enabled": true
    }
  ]
}
```

### 3.3 Production Environment (Read-Only)

**Archivo:** `newman/environments/production.json`

```json
{
  "name": "WePlay - Production",
  "values": [
    {
      "key": "baseUrl",
      "value": "https://api.weplay.com",
      "enabled": true
    },
    {
      "key": "identityUrl",
      "value": "https://identity.weplay.com",
      "enabled": true
    },
    {
      "key": "apiVersion",
      "value": "v1",
      "enabled": true
    }
  ]
}
```

---

## 4. Requests Detallados

### 4.1 _Setup - Get Auth Token (Reutilizable)

**Proposito:** Registrar un usuario de prueba y obtener token para tests autenticados.

**Request:**

```
POST {{identityUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "{{testEmail1}}",
    "password": "{{testPassword}}",
    "confirmPassword": "{{testPassword}}"
}
```

**Pre-request Script:**
```javascript
// Generar email unico con timestamp
pm.environment.set('testEmail1', `test-${Date.now()}@weplay.local`);
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Response time < 2000ms", () => {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

pm.test("ServiceResponse structure is valid", () => {
    const json = pm.response.json();
    pm.expect(json).to.have.property('data');
    pm.expect(json).to.have.property('messages');
    pm.expect(json.messages).to.be.an('array');
});

pm.test("Response data contains required fields", () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('userId');
    pm.expect(json.data).to.have.property('email');
    pm.expect(json.data).to.have.property('token');
});

pm.test("Token is valid JWT", () => {
    const json = pm.response.json();
    const token = json.data.token;
    const parts = token.split('.');
    pm.expect(parts).to.have.lengthOf(3);
});

pm.test("Save token and userId to environment", () => {
    const json = pm.response.json();
    pm.environment.set('bearerToken', json.data.token);
    pm.environment.set('userId', json.data.userId);
});

pm.test("isSuccess is true", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});
```

---

### 4.2 Auth - 200 OK - POST Register - Success

**Proposito:** Verificar registro exitoso de usuario con email y password validos.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "newuser-{{$timestamp}}@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

**Pre-request Script:**
```javascript
// Generar email unico
pm.environment.set('currentTestEmail', `newuser-${Date.now()}@weplay.local`);
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Response time < 1500ms", () => {
    pm.expect(pm.response.responseTime).to.be.below(1500);
});

pm.test("isSuccess is true", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test("Data contains userId (valid GUID)", () => {
    const json = pm.response.json();
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(json.data.userId).to.match(guidRegex);
});

pm.test("Data contains email", () => {
    const json = pm.response.json();
    pm.expect(json.data.email).to.equal(pm.environment.get('currentTestEmail'));
});

pm.test("Data contains token", () => {
    const json = pm.response.json();
    pm.expect(json.data.token).to.exist;
    pm.expect(json.data.token).to.not.be.empty;
});

pm.test("Token contains proper claims", () => {
    const json = pm.response.json();
    const token = json.data.token;
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(atob(base64));

    pm.expect(payload).to.have.property('sub'); // UserId
    pm.expect(payload).to.have.property('email');
    pm.expect(payload).to.have.property('exp');
    pm.expect(payload.email).to.equal(pm.environment.get('currentTestEmail'));
});

pm.test("Success message present", () => {
    const json = pm.response.json();
    const message = json.messages[0];
    pm.expect(message.errorCode).to.equal('SUCCESS');
});
```

---

### 4.3 Auth - 400 Bad Request - POST Register - Empty Email

**Proposito:** Verificar validacion cuando email esta vacio.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test("Messages contain validation error for email", () => {
    const json = pm.response.json();
    pm.expect(json.messages).to.be.an('array').that.is.not.empty;

    const emailError = json.messages.find(m =>
        m.errorCode === 'VALIDATION_REQUIRED' &&
        m.message.toLowerCase().includes('email')
    );
    pm.expect(emailError).to.exist;
});

pm.test("Error message is descriptive", () => {
    const json = pm.response.json();
    const error = json.messages[0];
    pm.expect(error.message).to.not.be.empty;
    pm.expect(error.errorCode).to.not.be.empty;
});
```

---

### 4.4 Auth - 400 Bad Request - POST Register - Invalid Email Format

**Proposito:** Validar rechazo de email con formato invalido.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "notanemail",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is AUTH_EMAIL_INVALID", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'AUTH_EMAIL_INVALID');
    pm.expect(error).to.exist;
    pm.expect(error.message).to.include('formato');
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.5 Auth - 400 Bad Request - POST Register - Empty Password

**Proposito:** Validar rechazo de password vacio.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "test@weplay.local",
    "password": "",
    "confirmPassword": ""
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is VALIDATION_REQUIRED for password", () => {
    const json = pm.response.json();
    const error = json.messages.find(m =>
        m.errorCode === 'VALIDATION_REQUIRED' &&
        m.message.toLowerCase().includes('contraseña')
    );
    pm.expect(error).to.exist;
});
```

---

### 4.6 Auth - 400 Bad Request - POST Register - Password Too Short

**Proposito:** Validar rechazo de password menor a 8 caracteres.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "test@weplay.local",
    "password": "Short1",
    "confirmPassword": "Short1"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is AUTH_PASSWORD_MIN_LENGTH", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'AUTH_PASSWORD_MIN_LENGTH');
    pm.expect(error).to.exist;
    pm.expect(error.message).to.include('8');
});
```

---

### 4.7 Auth - 400 Bad Request - POST Register - Passwords Don't Match

**Proposito:** Validar rechazo cuando password y confirmPassword no coinciden.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "test@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "DifferentPass123"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is AUTH_PASSWORD_MISMATCH", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'AUTH_PASSWORD_MISMATCH');
    pm.expect(error).to.exist;
    pm.expect(error.message.toLowerCase()).to.include('coinciden');
});
```

---

### 4.8 Auth - 409 Conflict - POST Register - Email Already Exists

**Proposito:** Validar rechazo cuando email ya esta registrado.

**Request:**
```
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
    "email": "existing@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

**Pre-request Script:**
```javascript
// Primero registrar el email
const emailToTest = `duplicate-${Date.now()}@weplay.local`;
pm.environment.set('duplicateEmail', emailToTest);

// Este test asume que el email ya existe
// En un flujo E2E, se registraria primero
```

**Test Script:**
```javascript
pm.test("Status code is 409", () => {
    pm.response.to.have.status(409);
});

pm.test("Error code is AUTH_EMAIL_EXISTS", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'AUTH_EMAIL_EXISTS');
    pm.expect(error).to.exist;
    pm.expect(error.message.toLowerCase()).to.include('registrado');
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.9 Artistas - 200 OK - POST Create - Success

**Proposito:** Crear perfil de artista con todos los campos.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Los Rockeros {{$timestamp}}",
    "descripcion": "Banda de rock alternativo con 10 años de trayectoria",
    "pais": "España",
    "ciudad": "Madrid",
    "imagenUrl": "https://example.com/artistas/band.jpg"
}
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Response time < 1000ms", () => {
    pm.expect(pm.response.responseTime).to.be.below(1000);
});

pm.test("isSuccess is true", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test("Data contains all required fields", () => {
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

pm.test("Data.id is valid GUID", () => {
    const json = pm.response.json();
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    pm.expect(json.data.id).to.match(guidRegex);
});

pm.test("Data.userId matches token", () => {
    const json = pm.response.json();
    const token = pm.environment.get('bearerToken');
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(atob(base64));

    pm.expect(json.data.userId).to.equal(payload.sub);
});

pm.test("Save artistaId to environment", () => {
    const json = pm.response.json();
    pm.environment.set('artistaId', json.data.id);
});

pm.test("fechaCreacion is ISO datetime", () => {
    const json = pm.response.json();
    pm.expect(new Date(json.data.fechaCreacion)).to.not.throw();
});
```

---

### 4.10 Artistas - 200 OK - POST Create - Minimal Data

**Proposito:** Crear perfil con solo campos obligatorios.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Solo Artista {{$timestamp}}"
}
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Optional fields are null/empty", () => {
    const json = pm.response.json();
    // Estos campos pueden ser null o string vacio
    pm.expect(json.data.descripcion).to.satisfy(val =>
        val === null || val === "" || val === undefined
    );
    pm.expect(json.data.pais).to.satisfy(val =>
        val === null || val === "" || val === undefined
    );
    pm.expect(json.data.ciudad).to.satisfy(val =>
        val === null || val === "" || val === undefined
    );
});

pm.test("nombreArtistico is required and set", () => {
    const json = pm.response.json();
    pm.expect(json.data.nombreArtistico).to.not.be.empty;
});
```

---

### 4.11 Artistas - 200 OK - GET Get By Id - Success

**Proposito:** Obtener perfil publico de artista por ID.

**Request:**
```
GET {{baseUrl}}/api/artistas/{{artistaId}}
Content-Type: application/json
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Response time < 500ms", () => {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

pm.test("isSuccess is true", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test("Data contains artista with matching ID", () => {
    const json = pm.response.json();
    const artistaId = pm.environment.get('artistaId');
    pm.expect(json.data.id).to.equal(artistaId);
});

pm.test("Data is accessible without authentication", () => {
    // Este test valida que el endpoint GET por ID no requiere token
    pm.expect(pm.request.headers.get('Authorization')).to.be.undefined;
});
```

---

### 4.12 Artistas - 200 OK - GET Get By User Id - Success

**Proposito:** Obtener perfil de artista propio del usuario autenticado.

**Request:**
```
GET {{baseUrl}}/api/artistas/by-user/{{userId}}
Authorization: Bearer {{bearerToken}}
Content-Type: application/json
```

**Test Script:**
```javascript
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("isSuccess is true", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.true;
});

pm.test("Data.userId matches request parameter", () => {
    const json = pm.response.json();
    const userId = pm.environment.get('userId');
    pm.expect(json.data.userId).to.equal(userId);
});

pm.test("Data contains all artista fields", () => {
    const json = pm.response.json();
    pm.expect(json.data).to.have.property('id');
    pm.expect(json.data).to.have.property('nombreArtistico');
});
```

---

### 4.13 Artistas - 400 Bad Request - POST Create - Empty Nombre Artistico

**Proposito:** Validar rechazo cuando nombreArtistico esta vacio.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": ""
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is VALIDATION_REQUIRED", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'VALIDATION_REQUIRED');
    pm.expect(error).to.exist;
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.14 Artistas - 400 Bad Request - POST Create - Nombre Artistico Too Long

**Proposito:** Validar rechazo cuando nombreArtistico excede 200 caracteres.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "A very long artist name that exceeds the maximum allowed length of two hundred characters for the artistic name field in the system and should trigger a validation error when submitted to the API endpoint"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is ARTISTA_NOMBRE_MAX_LENGTH", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'ARTISTA_NOMBRE_MAX_LENGTH');
    pm.expect(error).to.exist;
    pm.expect(error.message).to.include('200');
});
```

---

### 4.15 Artistas - 400 Bad Request - POST Create - Invalid Image URL

**Proposito:** Validar rechazo de URL de imagen invalida.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Valid Artist",
    "imagenUrl": "not-a-valid-url"
}
```

**Test Script:**
```javascript
pm.test("Status code is 400", () => {
    pm.response.to.have.status(400);
});

pm.test("Error code is ARTISTA_IMAGEN_URL_INVALIDA", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'ARTISTA_IMAGEN_URL_INVALIDA');
    pm.expect(error).to.exist;
});
```

---

### 4.16 Artistas - 401 Unauthorized - POST Create - Missing Token

**Proposito:** Validar rechazo cuando falta Authorization header.

**Request:**
```
POST {{baseUrl}}/api/artistas
Content-Type: application/json

{
    "nombreArtistico": "Artist Without Token"
}
```

**Test Script:**
```javascript
pm.test("Status code is 401", () => {
    pm.response.to.have.status(401);
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});

pm.test("Authorization header is not sent", () => {
    pm.expect(pm.request.headers.get('Authorization')).to.be.undefined;
});
```

---

### 4.17 Artistas - 401 Unauthorized - GET Get By User Id - Different UserId

**Proposito:** Validar que usuario no puede acceder al perfil de otro usuario.

**Request:**
```
GET {{baseUrl}}/api/artistas/by-user/{{userId2}}
Authorization: Bearer {{bearerToken}}
Content-Type: application/json
```

**Pre-request Script:**
```javascript
// Asegurar que userId2 es diferente a userId
const userId = pm.environment.get('userId');
const userId2 = pm.environment.get('userId2');

if (userId === userId2) {
    pm.expect.fail("userId2 debe ser diferente a userId para este test");
}
```

**Test Script:**
```javascript
pm.test("Status code is 401", () => {
    pm.response.to.have.status(401);
});

pm.test("Error code is AUTH_UNAUTHORIZED", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'AUTH_UNAUTHORIZED');
    pm.expect(error).to.exist;
    pm.expect(error.message.toLowerCase()).to.include('permiso');
});
```

---

### 4.18 Artistas - 404 Not Found - GET Get By Id - Non-existent

**Proposito:** Validar respuesta cuando artista no existe.

**Request:**
```
GET {{baseUrl}}/api/artistas/00000000-0000-0000-0000-000000000000
Content-Type: application/json
```

**Test Script:**
```javascript
pm.test("Status code is 404", () => {
    pm.response.to.have.status(404);
});

pm.test("Error code is ARTISTA_NOT_FOUND", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'ARTISTA_NOT_FOUND');
    pm.expect(error).to.exist;
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

### 4.19 Artistas - 409 Conflict - POST Create - Artist Profile Already Exists

**Proposito:** Validar rechazo cuando usuario ya tiene perfil de artista.

**Request:**
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken}}
Content-Type: application/json

{
    "nombreArtistico": "Duplicate Artist"
}
```

**Pre-request Script:**
```javascript
// Este request se ejecuta despues de que se haya creado un artista
// El mismo token/userId ya tiene un perfil, lo que debe generar conflict
```

**Test Script:**
```javascript
pm.test("Status code is 409", () => {
    pm.response.to.have.status(409);
});

pm.test("Error code is ARTISTA_ALREADY_EXISTS", () => {
    const json = pm.response.json();
    const error = json.messages.find(m => m.errorCode === 'ARTISTA_ALREADY_EXISTS');
    pm.expect(error).to.exist;
});

pm.test("isSuccess is false", () => {
    const json = pm.response.json();
    pm.expect(json.isSuccess).to.be.false;
});
```

---

## 5. Flujos E2E (End-to-End)

### 5.1 E2E Flow: Complete Registration and Profile Creation

**Objetivo:** Validar el flujo completo de registro e creacion de perfil.

**Secuencia de Requests:**

#### Paso 1: Register User
```
POST {{baseUrl}}/api/auth/register

{
    "email": "e2e-complete-{{$timestamp}}@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

**Validaciones:**
- Status 200
- Token generado
- UserId capturado
- Guardar token en bearerToken_E2E
- Guardar userId en userId_E2E

#### Paso 2: Create Artista Profile
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{bearerToken_E2E}}

{
    "nombreArtistico": "E2E Artist {{$timestamp}}",
    "descripcion": "Integration test artist",
    "pais": "Spain",
    "ciudad": "Barcelona",
    "imagenUrl": "https://example.com/e2e-artist.jpg"
}
```

**Validaciones:**
- Status 200
- artistaId capturado
- userId en respuesta coincide con token
- Guardar artistaId en artistaId_E2E

#### Paso 3: Get Artista by ID (Public Access)
```
GET {{baseUrl}}/api/artistas/{{artistaId_E2E}}
```

**Validaciones:**
- Status 200
- Datos coinciden con los creados
- Sin necesidad de autenticacion
- nombreArtistico, pais, ciudad presentes

#### Paso 4: Get Artista by UserId (Authenticated)
```
GET {{baseUrl}}/api/artistas/by-user/{{userId_E2E}}
Authorization: Bearer {{bearerToken_E2E}}
```

**Validaciones:**
- Status 200
- Datos coinciden con creados
- userId en respuesta coincide con parametro
- artistaId coincide con paso 3

#### Paso 5: Verify Data Consistency
```javascript
pm.test("Complete E2E flow validates data consistency", () => {
    // Validar que los datos se mantengan consistentes
    // entre los cuatro requests
    pm.expect(data1.id).to.equal(data2.id);
    pm.expect(data1.userId).to.equal(data2.userId);
    pm.expect(data1.nombreArtistico).to.equal(data2.nombreArtistico);
});
```

---

### 5.2 E2E Flow: Multi-User Isolation

**Objetivo:** Validar que usuarios no pueden ver perfiles de otros.

**Secuencia:**

#### Paso 1: Register User 1
```
POST {{baseUrl}}/api/auth/register

{
    "email": "user1-{{$timestamp}}@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

Guardar como: token1, userId1

#### Paso 2: Create Artista for User 1
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{token1}}

{
    "nombreArtistico": "Artist One"
}
```

Guardar como: artistaId1

#### Paso 3: Register User 2
```
POST {{baseUrl}}/api/auth/register

{
    "email": "user2-{{$timestamp}}@weplay.local",
    "password": "SecurePass123",
    "confirmPassword": "SecurePass123"
}
```

Guardar como: token2, userId2

#### Paso 4: Create Artista for User 2
```
POST {{baseUrl}}/api/artistas
Authorization: Bearer {{token2}}

{
    "nombreArtistico": "Artist Two"
}
```

Guardar como: artistaId2

#### Paso 5: User 1 Can Access Own Profile
```
GET {{baseUrl}}/api/artistas/by-user/{{userId1}}
Authorization: Bearer {{token1}}
```

**Validacion:** Status 200

#### Paso 6: User 1 Cannot Access User 2 Profile
```
GET {{baseUrl}}/api/artistas/by-user/{{userId2}}
Authorization: Bearer {{token1}}
```

**Validacion:** Status 401

#### Paso 7: Public Access Works for Both
```
GET {{baseUrl}}/api/artistas/{{artistaId1}}
GET {{baseUrl}}/api/artistas/{{artistaId2}}
```

**Validacion:** Ambos Status 200 sin autenticacion

---

## 6. Collection-Level Scripts

### 6.1 Pre-collection Script

```javascript
// Inicializar variables necesarias
pm.collectionVariables.set('testRunId', pm.globals.get('testRunId') || Date.now().toString());

// Validar que el ambiente este configurado correctamente
const baseUrl = pm.environment.get('baseUrl');
if (!baseUrl) {
    throw new Error('baseUrl no configurada en el ambiente');
}

console.log(`[${new Date().toISOString()}] Iniciando tests de registro-artista en ${baseUrl}`);
```

### 6.2 Post-collection Script

```javascript
// Resumen de ejecucion
const results = pm.collectionVariables.get('testResults');
console.log(`
=== Resumen de Ejecucion ===
Tests ejecutados: {{count}}
Exitosos: {{passed}}
Fallidos: {{failed}}
Duracion total: {{duration}}ms
`);
```

---

## 7. Ejecucion

### 7.1 Ejecucion Local - Modo Desarrollo

```bash
# Ejecucion simple
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/newman/reports/development-report.html

# Ejecucion con variables de entorno
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --global-var "baseUrl=https://localhost:5001" \
    --reporters cli,htmlextra

# Ejecucion solo de una carpeta (ej. Auth)
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth" \
    --reporters cli
```

### 7.2 Ejecucion Local - Modo Staging

```bash
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/staging.json \
    --reporters cli,htmlextra,junit \
    --reporter-htmlextra-export tests/newman/reports/staging-report.html \
    --reporter-junit-export tests/newman/reports/staging-results.xml
```

### 7.3 Ejecucion Continua - Pre-push (Local)

```bash
#!/bin/bash
# Script: scripts/test-api-before-push.sh

set -e

echo "Running API integration tests before push..."

newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli \
    --insecure

echo "All tests passed!"
```

### 7.4 Ejecucion en CI/CD - Azure Pipelines

**Archivo:** `azure-pipelines.yml` (fragmento)

```yaml
stages:
  - stage: IntegrationTests
    displayName: 'API Integration Tests'

    jobs:
    - job: RunNewman
      displayName: 'Run Newman Tests'

      steps:
      - task: NodeTool@0
        inputs:
          versionSpec: '18.x'
        displayName: 'Install Node.js'

      - task: Npm@1
        inputs:
          command: 'install'
          workingDir: 'tests/newman'
        displayName: 'Install Newman'

      - task: Bash@3
        inputs:
          targetType: 'filePath'
          filePath: 'scripts/run-newman-tests.sh'
          arguments: 'staging'
        displayName: 'Run Newman Collection'
        env:
          TEST_PASSWORD: $(StagingTestPassword)

      - task: PublishTestResults@2
        inputs:
          testResultsFormat: 'JUnit'
          testResultsFiles: '**/newman/reports/staging-results.xml'
          mergeTestResults: true
          failTaskOnFailedTests: true
        displayName: 'Publish Test Results'
        condition: always()

      - task: PublishBuildArtifacts@1
        inputs:
          pathToPublish: '$(Build.ArtifactStagingDirectory)/newman/reports'
          artifactName: 'newman-reports'
        displayName: 'Publish Newman Reports'
        condition: always()
```

### 7.5 Ejecucion en CI/CD - GitHub Actions

**Archivo:** `.github/workflows/api-tests.yml`

```yaml
name: API Integration Tests

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

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
      run: npm install -g newman newman-reporter-htmlextra

    - name: Wait for API
      run: |
        timeout 300 bash -c 'until curl -f http://localhost:5001/health; do sleep 1; done'
      continue-on-error: true

    - name: Run Newman Tests
      run: |
        newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
          -e tests/newman/environments/staging.json \
          --reporters cli,htmlextra,json \
          --reporter-json-export ./newman-results.json \
          --reporter-htmlextra-export ./newman-report.html

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: newman-results
        path: |
          ./newman-results.json
          ./newman-report.html

    - name: Test Report
      if: always()
      run: cat newman-results.json | jq '.run.stats'
```

---

## 8. Monitoreo y Reportes

### 8.1 Tipos de Reportes Generados

#### 8.1.1 CLI Report
```
newman run collection.json -e environment.json --reporters cli

┌─────────────────────────┬───────┬──────┐
│                         │ Tests │ Fail │
├─────────────────────────┼───────┼──────┤
│ Register - Success      │   1   │  0   │
│ Register - Validation   │  10   │  0   │
│ Create Artista          │   8   │  0   │
│ Get Artista             │   5   │  0   │
├─────────────────────────┼───────┼──────┤
│ Total                   │  24   │  0   │
└─────────────────────────┴───────┴──────┘
```

#### 8.1.2 HTML Report
Reportes visuales con:
- Resumen de ejecucion
- Detalles por request
- Respuesta JSON/XML
- Tiempos de respuesta
- Graphicos de performance

#### 8.1.3 JUnit Report (para CI/CD)
```xml
<?xml version="1.0" encoding="UTF-8"?>
<testsuites>
    <testsuite name="WePlay.RegistroArtista.IntegrationTests" tests="18" failures="0">
        <testcase name="Register - Success" time="1.234">
            <properties>
                <property name="status" value="PASS"/>
                <property name="responseTime" value="1234ms"/>
            </properties>
        </testcase>
    </testsuite>
</testsuites>
```

### 8.2 Metricas de Calidad

| Metrica | Target | Umbral Critico |
|---------|--------|----------------|
| Tasa de exito | 100% | < 95% |
| Tiempo promedio respuesta | < 500ms | > 2000ms |
| P95 latencia | < 1000ms | > 3000ms |
| Cobertura de endpoints | 100% | < 80% |
| Cobertura de error codes | 100% | < 90% |

---

## 9. Mantenimiento de Coleccion

### 9.1 Versionado

```
tests/newman/
├── collections/
│   ├── WePlay.RegistroArtista.IntegrationTests.v1.json
│   ├── WePlay.RegistroArtista.IntegrationTests.v2.json
│   └── WePlay.RegistroArtista.IntegrationTests.latest.json (link a v2)
│
└── CHANGELOG.md
    - v2.0: Agregar tests para imagen URL validation
    - v1.0: Release inicial
```

### 9.2 Sincronizacion con Cambios de API

Cuando la API cambie:

1. Actualizar los contracts en `api-contracts.md`
2. Actualizar requests/responses en la coleccion
3. Agregar nuevos tests si hay nuevos errores
4. Ejecutar regression: `newman run ... --folder "_E2E Flows"`
5. Actualizar CHANGELOG con version incremental

### 9.3 Test Data Management

**Politica de limpieza:**
- Tests generan datos con timestamp unico
- Cleanup folder elimina datos creados
- En staging: ejecutar limpieza al final del run
- En production: NO ejecutar tests (read-only)

**Script de cleanup:**
```bash
# Eliminar datos de prueba mas antiguos a 24h
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/staging.json \
    --folder "_Cleanup" \
    --reporters cli
```

---

## 10. Casos de Uso y Escenarios

### 10.1 Validacion de Contratos

**Como:** Verificar que API cumple el contrato especificado

```bash
# Ejecutar tests especificos
newman run collection.json -e environment.json \
    --folder "Auth/200 OK" \
    --folder "Artistas/200 OK" \
    --bail
```

### 10.2 Testing de Autenticacion

**Como:** Validar seguridad y JWT

```bash
# Ejecutar solo tests de 401/409
newman run collection.json -e environment.json \
    --folder "Auth/409 Conflict" \
    --folder "Artistas/401 Unauthorized" \
    --reporters cli
```

### 10.3 Testing de Validacion

**Como:** Verificar validaciones de campos

```bash
# Ejecutar solo tests de 400 Bad Request
newman run collection.json -e environment.json \
    --folder "Auth/400 Bad Request" \
    --folder "Artistas/400 Bad Request"
```

### 10.4 Testing de Flujos E2E

**Como:** Verificar flujos completos del usuario

```bash
# Ejecutar solo E2E flows
newman run collection.json -e environment.json \
    --folder "_E2E Flows" \
    --reporters cli,htmlextra
```

---

## 11. Troubleshooting

### 11.1 Problemas Comunes

| Problema | Causa | Solucion |
|----------|-------|----------|
| 401 Unauthorized en todos los tests | Token expirado | Ejecutar _Setup para obtener nuevo token |
| 500 Internal Server Error | API no disponible | Verificar que API esta corriendo en puerto correcto |
| Request timeout | API lenta | Aumentar timeout en coleccion (post-delay: 500ms) |
| Email conflict en tests | Datos de prueba no limpiados | Ejecutar _Cleanup folder |
| Variables no se guardan | Scope incorrecto | Usar `pm.environment.set()` en lugar de `pm.collectionVariables` |

### 11.2 Logs Utiles

```bash
# Modo verbose
newman run collection.json -e environment.json --verbose

# Salida JSON completa
newman run collection.json -e environment.json \
    --reporters json \
    --reporter-json-export ./full-output.json

# Debug de requests/responses
newman run collection.json -e environment.json \
    --reporters json | jq '.run.executions[] | {name, request, response}'
```

---

## 12. Integracion con CI/CD

### 12.1 Pre-Push Hook

**Archivo:** `.githooks/pre-push`

```bash
#!/bin/bash
echo "Running API tests before push..."
newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli \
    --bail

if [ $? -ne 0 ]; then
    echo "Tests failed! Push cancelled."
    exit 1
fi
```

### 12.2 Pull Request Checks

Los tests se ejecutan automaticamente en cada PR y deben pasar antes de mergear.

### 12.3 Deployment Gate

En staging/production, los tests se ejecutan como prerequisito para deployment.

---

## 13. Checklist de Implementacion

- [ ] **Coleccion creada:**
  - [ ] JSON exportado desde Postman
  - [ ] Estructura de carpetas configurada
  - [ ] Requests y tests implementados

- [ ] **Environments configurados:**
  - [ ] development.json creado
  - [ ] staging.json creado
  - [ ] production.json creado (read-only)

- [ ] **Requests completados:**
  - [ ] 18 requests creados
  - [ ] Pre-request scripts implementados
  - [ ] Test scripts con assertions completos

- [ ] **E2E Flows:**
  - [ ] Complete Registration flow definido
  - [ ] Multi-user isolation flow definido
  - [ ] Scripts de validacion de consistencia

- [ ] **Ejecucion local:**
  - [ ] Tests pasan en development
  - [ ] Reportes HTML generados
  - [ ] Variables se guardan correctamente

- [ ] **CI/CD integrado:**
  - [ ] Azure Pipelines configurado (si aplica)
  - [ ] GitHub Actions configurado (si aplica)
  - [ ] Pre-push hook instalado
  - [ ] Reportes publicados

- [ ] **Documentacion:**
  - [ ] README.md creado en tests/newman/
  - [ ] Guia de troubleshooting
  - [ ] Ejemplos de uso

---

## 14. Siguiente Paso Sugerido

Una vez implementada la coleccion Newman:

1. **Exportar JSON desde Postman:**
   - Crear coleccion en Postman UI
   - Implementar 18 requests
   - Exportar como JSON a: `tests/newman/WePlay.RegistroArtista.IntegrationTests.json`

2. **Validar ejecucion local:**
   - `newman run tests/newman/WePlay.RegistroArtista.IntegrationTests.json -e tests/newman/environments/development.json`
   - Todos los 18 tests deben pasar

3. **Configurar CI/CD:**
   - Agregar steps en azure-pipelines.yml
   - Crear workflow en .github/workflows/

4. **Integracion pre-push:**
   - Instalar git hook: `chmod +x .githooks/pre-push && git config core.hooksPath .githooks`

---

## 15. Referencias

- **Postman Learning Center:** https://learning.postman.com/
- **Newman CLI:** https://github.com/postmanlabs/newman
- **ServiceResponse Pattern:** Revisar `BuildingBlocks/Kernel/Http/Response/ServiceResponse.cs`
- **JWT Claims:** https://tools.ietf.org/html/rfc7519
- **Error Codes Catalog:** Ver seccion 10 de `api-contracts.md`

---

**Autor:** Claude Code Agent
**Fecha de Creacion:** 2026-01-26
**Ultima Actualizacion:** 2026-01-26
**Version:** 1.0
