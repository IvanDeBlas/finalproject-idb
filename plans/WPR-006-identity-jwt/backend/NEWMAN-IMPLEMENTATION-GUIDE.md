# Guia de Implementacion: Newman Tests WPR-006

**Fecha:** 2026-02-12
**Status:** Plan listo para implementacion

---

## Resumen Rapido

Se ha creado un plan completo de testing con Postman/Newman para los 3 endpoints de autenticacion de WPR-006:
- POST /api/auth/register (YA EXISTE)
- POST /api/auth/login (A IMPLEMENTAR)
- GET /api/auth/me (A IMPLEMENTAR)

**Total:** 18+ requests, 30+ casos de prueba, 3 flujos E2E

---

## Pasos para Implementar

### 1. Crear Estructura de Carpetas

```bash
mkdir -p tests/newman/{collections,environments,scripts}
```

### 2. Descargar Postman Workspace

**Opcion A: Importar JSON manualmente**
1. Abrir Postman
2. Click en "Import"
3. Crear nueva coleccion "WePlay.WPR006.IdentityTests"
4. Seguir la estructura detallada en `newman-tests.md` secciones 2 y 5

**Opcion B: Usar plantilla JSON**
Exportar coleccion desde Postman como JSON en:
```
tests/newman/collections/WePlay.WPR006.IdentityTests.json
```

### 3. Crear Archivos de Entorno

**Archivo:** `tests/newman/environments/development.json`

Copiar variables desde seccion 3.1 del plan.

**Archivo:** `tests/newman/environments/ci.json`

Copiar variables desde seccion 3.2 del plan.

### 4. Crear Scripts de Ejecucion

**Archivo:** `tests/newman/scripts/run-local.sh`

```bash
#!/bin/bash
set -e

echo "Running Newman Identity Tests (Development)"
echo "============================================"

# Asegurar que API esta corriendo en http://localhost:5261
echo "Checking API health..."
curl -f http://localhost:5261/healthz 2>/dev/null || {
    echo "WARNING: API healthz endpoint not found, but continuing..."
}

echo ""
echo "Executing tests..."
echo ""

newman run tests/newman/collections/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export tests/reports/identity-tests-$(date +%Y%m%d-%H%M%S).html \
    --timeout-request 10000

echo ""
echo "Tests completed!"
echo "Report: tests/reports/identity-tests-*.html"
```

**Archivo:** `tests/newman/scripts/run-ci.sh`

```bash
#!/bin/bash
set -e

echo "Running Newman Identity Tests (CI/CD)"

mkdir -p tests/reports

newman run tests/newman/collections/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/ci.json \
    --reporters cli,junit,htmlextra \
    --reporter-junit-export tests/reports/identity-tests.xml \
    --reporter-htmlextra-export tests/reports/identity-tests.html \
    --timeout-request 15000 \
    --bail

exit $?
```

Hacer ejecutables:
```bash
chmod +x tests/newman/scripts/run-*.sh
```

### 5. Instalar Dependencias

```bash
# Newman
npm install -g newman

# Reporters
npm install -g newman-reporter-htmlextra
npm install -g newman-reporter-junitxml
```

O para proyecto local:
```bash
npm install --save-dev newman newman-reporter-htmlextra newman-reporter-junitxml
```

---

## Estructura de Requests por Folder

La coleccion debe tener esta estructura EXACTA para que los tests funcionen:

```
WePlay.WPR006.IdentityTests/
│
├── _Setup/ (Pre-request scripts para inicializar)
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
│   │   ├── 214_Register with Short Password
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
│   │   ├── 306_Login Multiple Times
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
│   ├── Flow 1 - Register → Login → Get User Info/
│   │   ├── 501_Register New User
│   │   ├── 502_Login with Registered Credentials
│   │   ├── 503_Get User Info with Login Token
│   │   └── 504_Verify Data Consistency
│   │
│   ├── Flow 2 - Register Artista → Verify Roles/
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
│   │   ├── 602_Verify Password Not Returned
│   │   ├── 603_Verify Token Not Logged
│   │   ├── 604_Verify No Sensitive Data in Errors
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
└── _Cleanup/ (Limpiar datos de test)
    ├── 801_Delete Test Users
    ├── 802_Clear Environment Variables
    └── 803_Reset Collection State
```

---

## Uso Rapido

### Ejecucion Local

```bash
# Prerequisito: API corriendo en localhost:5261
dotnet run --project src/api/WebApi/WebApi.csproj

# En otra terminal:
bash tests/newman/scripts/run-local.sh
```

### Ejecucion Especifica

```bash
# Solo tests de Register (Success)
newman run tests/newman/collections/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Register" \
    --folder "200 OK - Success Cases"

# Solo tests de Login (Errors)
newman run tests/newman/collections/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "Auth - Login" \
    --folder "401 Unauthorized - Invalid Credentials"

# Solo flujos E2E
newman run tests/newman/collections/WePlay.WPR006.IdentityTests.json \
    -e tests/newman/environments/development.json \
    --folder "E2E Flows"
```

### Ver Reportes

Después de ejecutar los tests, abrir:
```
tests/reports/identity-tests-YYYYMMDD-HHMMSS.html
```

---

## Elementos Clave a Revisar en el Plan

### 1. Estructura de Respuestas (Seccion 5)

Cada request detallado incluye:
- **Pre-Request Script:** Inicializar variables
- **Request Body:** JSON con estructura esperada
- **Test Script:** Validaciones completas

### 2. Variables de Entorno (Seccion 3)

Variables criticas:
- `base_url` - URL base de la API
- `auth_endpoint` - URL de /api/auth
- `test_password` - Password para tests
- `auth_token` - Token JWT obtenido en login
- `current_user_id` - UserId para /me
- `registered_user_id` - UserId del usuario registrado

### 3. Pre-request Scripts (Seccion 4)

Incluyen:
- Inicializacion de variables
- Generacion de emails unicos
- Validacion de setup

### 4. Assertions por Status Code

**200 OK:**
```javascript
pm.test('Status code is 200', () => pm.response.to.have.status(200));
pm.test('isSuccess is true', () => pm.expect(pm.response.json().isSuccess).to.be.true);
pm.test('Data contains required fields', () => { ... });
```

**400 BAD REQUEST:**
```javascript
pm.test('Status code is 400', () => pm.response.to.have.status(400));
pm.test('isSuccess is false', () => pm.expect(pm.response.json().isSuccess).to.be.false);
pm.test('Error code starts with 1', () => { ... });
```

**401 UNAUTHORIZED:**
```javascript
pm.test('Status code is 401', () => pm.response.to.have.status(401));
pm.test('Error code is 3006 or 3007', () => { ... });
```

---

## Pipeline CI/CD (Seccion 8)

El plan incluye ejemplos para:
- **Azure DevOps** (YAML pipeline)
- **GitHub Actions** (workflow)
- **GitLab CI** (config file)

Elegir segun tu setup y copiar desde seccion 8.

---

## Validaciones JWT Token (Seccion 5.16)

El plan incluye validacion completa de JWT:

```javascript
// 1. Formato basico
const jwtRegex = /^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$/;
pm.expect(token).to.match(jwtRegex);

// 2. Decodificar header
const header = token.split('.')[0];
const decodedHeader = atob(header);
const headerObj = JSON.parse(decodedHeader);
pm.expect(headerObj.alg).to.equal('HS256');

// 3. Decodificar payload
const payload = token.split('.')[1];
const decodedPayload = atob(payload);
const payloadObj = JSON.parse(decodedPayload);
pm.expect(payloadObj).to.have.property('sub');
pm.expect(payloadObj).to.have.property('email');

// 4. Verificar expiration
const now = Math.floor(Date.now() / 1000);
pm.expect(payloadObj.exp).to.be.greaterThan(now);
```

---

## Test Data y Cleanup

### Generar Emails Unicos

```javascript
// En Pre-Request Scripts
const email = `test_${Date.now()}_${Math.random().toString(36).substr(2, 9)}@weplay.test`;
pm.environment.set('test_email', email);
```

### Cleanup de Test Data (OPCIONAL)

```javascript
// Folder _Cleanup
// DELETE /api/auth/users/:id (endpoint para limpiar)
```

---

## Metricas Esperadas

| Metrica | Objetivo |
|---------|----------|
| Success Rate | > 99% |
| Register Response Time | < 1000ms |
| Login Response Time | < 500ms |
| Get User Response Time | < 200ms |
| Test Execution Time (Total) | < 30 segundos |
| Code Coverage | > 95% |

---

## Troubleshooting Rapido

### Error: "Cannot find module 'newman'"
```bash
npm install -g newman
```

### Error: "auth_token is undefined"
- Verificar que login test se ejecuto antes de /me tests
- Verificar que la variable se guarda correctamente
- Usar `--folder "E2E Flows"` para ejecutar en orden correcto

### Error: "Email already exists"
- Los emails generados deben ser unicos
- Usar timestamps en lugar de valores fijos
- Ejecutar cleanup entre test runs

### Error: "Invalid JWT format"
- Verificar que la respuesta contiene un token
- Verificar que es valido Base64URL encoded
- Revisar logs de la API para detalles

---

## Proximos Pasos

### Inmediatos (Hoy)
1. [ ] Crear estructura de carpetas `tests/newman/`
2. [ ] Crear archivo `environments/development.json` con variables
3. [ ] Crear coleccion en Postman con estructura detallada

### Esta Semana
4. [ ] Importar todos los requests desde seccion 5 del plan
5. [ ] Agregar test scripts desde seccion 5
6. [ ] Ejecutar localmente y validar que funciona
7. [ ] Crear scripts bash para ejecucion

### Proximo Sprint
8. [ ] Integrar con Azure DevOps/GitHub Actions
9. [ ] Configurar reportes HTML y JUnit XML
10. [ ] Configurar alertas para test failures
11. [ ] Agregar a pre-commit hooks

---

## Referencias

- **Plan completo:** `plans/WPR-006-identity-jwt/backend/newman-tests.md`
- **API Contracts:** `plans/WPR-006-identity-jwt/backend/api-contracts.md`
- **Documentacion Newman:** https://learning.postman.com/docs/running-collections/using-newman-cli/
- **JWT Claims:** Seccion 14 en api-contracts.md

---

## Contacto y Soporte

Para dudas sobre:
- **Test cases:** Revisar seccion 5 (Requests Detallados)
- **Variables:** Revisar seccion 3 (Variables de Entorno)
- **CI/CD:** Revisar seccion 8 (Ejecucion en CI/CD)
- **JWT:** Revisar seccion 5.16 (Contract Validation)

---

**Documento de Implementacion - WPR-006 Newman Tests**
**Creado: 2026-02-12**
**Status: Listo para Implementacion**
