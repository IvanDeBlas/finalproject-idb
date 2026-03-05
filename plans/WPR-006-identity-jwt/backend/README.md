# WePlay WPR-006 Identity JWT - Integration Tests

Coleccion Postman ejecutable para testing de autenticacion JWT en WePlay Rises.

## Archivos incluidos

- `postman-collection.json` - Coleccion Postman v2.1 con todos los tests
- `environment-development.json` - Variables de entorno para desarrollo local
- `run-newman.sh` - Script para ejecutar tests con Newman

## Requisitos previos

1. **Node.js 18+** instalado
2. **Newman** instalado globalmente:
   ```bash
   npm install -g newman
   npm install -g newman-reporter-htmlextra  # Para reportes HTML
   ```
3. **API en ejecucion** en `http://localhost:5261`

## Estructura de la coleccion

```
WePlay.WPR006.IdentityTests
├── _Setup                          # Inicializacion de ambiente
├── Auth - Register                 # Tests de registro
│   ├── 200 OK - Success Cases
│   ├── 400 Bad Request - Validation Errors
│   └── 409 Conflict - Business Rules
├── Auth - Login                    # Tests de login
│   ├── 200 OK - Success Cases
│   ├── 400 Bad Request - Validation Errors
│   └── 401 Unauthorized - Invalid Credentials
├── Auth - Get Current User         # Tests de /me endpoint
│   ├── 200 OK - Success Cases
│   └── 401 Unauthorized - Authentication Issues
├── E2E Flows                       # Flujos end-to-end
│   ├── Flow 1 - Register → Login → Get User
│   └── Flow 2 - Register Artista → Verify Roles
├── Contract Validation             # Validacion de contratos
├── Security Tests                  # Tests de seguridad
└── Performance Tests               # Tests de performance
```

## Ejecucion

### Opcion 1: Ejecutar todo con el script

```bash
cd plans/WPR-006-identity-jwt/backend
chmod +x run-newman.sh
./run-newman.sh
```

### Opcion 2: Ejecutar con Newman directamente

```bash
# Todas las pruebas
newman run postman-collection.json \
    -e environment-development.json

# Con reportes HTML
newman run postman-collection.json \
    -e environment-development.json \
    --reporters cli,htmlextra \
    --reporter-htmlextra-export report.html

# Folder especifico
newman run postman-collection.json \
    -e environment-development.json \
    --folder "Auth - Register"

# Con verbose output
newman run postman-collection.json \
    -e environment-development.json \
    --verbose
```

### Opcion 3: Usar desde Postman UI

1. Importar `postman-collection.json` en Postman
2. Importar `environment-development.json` como environment
3. Seleccionar el environment
4. Ejecutar la coleccion desde el Collection Runner

## Variables de Entorno

Las siguientes variables estan disponibles:

| Variable | Valor Default | Descripcion |
|----------|---------------|-------------|
| `base_url` | `http://localhost:5261` | URL base de la API |
| `api_url` | `{{base_url}}/api` | Endpoint de API |
| `auth_endpoint` | `{{api_url}}/auth` | Endpoint de autenticacion |
| `test_password` | `TestPassword123!` | Password para tests |
| `auth_token` | `` | Token JWT (auto-generado) |
| `registered_user_id` | `` | UserId del usuario registrado |
| `current_user_id` | `` | UserId del usuario logeado |

## Pre-Request Scripts

Los scripts pre-request generan automaticamente:
- Emails unicos con timestamp
- Variables de ambiente para tests posteriores
- Logging de ejecucion

## Test Assertions

Cada request incluye assertions para validar:
- Status codes HTTP
- Estructura de respuesta ServiceResponse
- Validacion de datos (UserId, email, roles)
- Validacion de JWT (formato, claims, expiracion)
- Performance (response times)
- Seguridad (no password en respuesta, SQL injection prevention)

## Flujos E2E incluidos

### Flow 1: Register → Login → Get User
Verifica la consistencia de datos a traves de todo el flujo de autenticacion:
1. Registrar nuevo usuario Fan
2. Login con credenciales registradas
3. Obtener info del usuario autenticado
4. Validar que todos los datos coinciden

### Flow 2: Register Artista → Verify Roles
Verifica que los roles se mantienen consistentes:
1. Registrar usuario como Artista
2. Login y extraer roles
3. Get user y verificar que roles coinciden

## Tests Incluidos

Total: 25+ tests principales + 100+ assertions

### Register Endpoint
- ✓ Registro exitoso (Fan, Artista)
- ✓ Validacion de email (vacio, formato invalido)
- ✓ Validacion de password (vacio, corta, no coinciden)
- ✓ Email duplicado (409 Conflict)
- ✓ Estructura de respuesta
- ✓ Validacion de JWT
- ✓ Roles correctos

### Login Endpoint
- ✓ Login exitoso con credenciales validas
- ✓ Validacion de email (vacio, invalido)
- ✓ Validacion de password (vacio)
- ✓ Credenciales invalidas (401)
- ✓ Email inexistente (401)
- ✓ Password incorrecta (401)
- ✓ Estructura de respuesta
- ✓ JWT valido

### Get User Endpoint (/me)
- ✓ Get user con token valido (200)
- ✓ Sin token (401)
- ✓ Token invalido (401)
- ✓ Bearer malformado (401)
- ✓ Estructura de respuesta
- ✓ Datos consistentes

### Security
- ✓ No password en respuestas
- ✓ SQL injection prevention
- ✓ No user enumeration en errores

### Performance
- ✓ Register < 1000ms
- ✓ Login < 500ms
- ✓ Get User < 200ms

## Reportes

Despues de ejecutar los tests, se generan reportes en:
- **HTML**: `reports/wpr006-identity-tests-TIMESTAMP.html`
- **JUnit XML**: `reports/wpr006-identity-tests-TIMESTAMP.xml`

El reporte HTML incluye:
- Resumen de ejecucion
- Detalles de cada request/test
- Logs de pre-request y test scripts
- Tiempos de respuesta
- Status de assertions

## Troubleshooting

### "Cannot POST /api/auth/register"
La API no esta en ejecucion. Iniciar el backend:
```bash
cd src/api/WebApi
dotnet run
```

### "Email already registered"
Usar timestamps en los emails para evitar colisiones. Los scripts ya lo hacen automaticamente.

### "Token invalido en /me"
1. Verificar que el login se ejecuto exitosamente
2. Verificar que `auth_token` se guardo correctamente
3. Revisar los logs del script de test

### "CORS error"
Verificar que CORS esta habilitado en el backend:
```csharp
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
```

## Comandos Utiles

```bash
# Ejecutar con verbose para debugging
newman run postman-collection.json -e environment-development.json --verbose

# Ejecutar folder especifico
newman run postman-collection.json -e environment-development.json --folder "Auth - Register"

# Ejecutar sin parar en errores
newman run postman-collection.json -e environment-development.json --bail off

# Ejecutar con delay entre requests
newman run postman-collection.json -e environment-development.json --delay 500

# Listar todas las carpetas disponibles
newman run postman-collection.json -e environment-development.json --reporters cli --export summary.json
```

## CI/CD Integration

Para ejecutar en Azure DevOps o GitHub Actions:

```bash
# Instalar Newman
npm install -g newman newman-reporter-junitxml newman-reporter-htmlextra

# Ejecutar tests
newman run postman-collection.json \
    -e environment-development.json \
    --reporters cli,junit,htmlextra \
    --reporter-junit-export test-results.xml \
    --reporter-htmlextra-export test-results.html \
    --bail
```

## Referencia de Error Codes

Los tests validan estos error codes segun el plan:

| Rango | Categoria |
|-------|-----------|
| 0xxx | Success |
| 1xxx | Validation Errors |
| 2xxx | NotFound |
| 3xxx | Authentication Errors |
| 4xxx | Business Rules |
| 5xxx | Internal Errors |

## Documentacion Relacionada

- Plan de tests detallado: `newman-tests.md`
- Arquitectura CQRS: `.claude/rules/backend/cqrs.rule.md`
- Convenciones de codigo: `.claude/rules/always/code-style.rule.md`

## Mantenimiento

### Actualizar coleccion manualmente

1. Exportar desde Postman UI (File → Export Collection)
2. Reemplazar `postman-collection.json`
3. Validar con Newman antes de commitear

### Agregar nuevos tests

1. Crear request en Postman
2. Agregar test scripts con assertions
3. Exportar coleccion
4. Validar con Newman locally

## Metricas de Exito

Todos estos tests deben pasar:

- [ ] Total requests: 25+ ejecutados sin errores
- [ ] Status codes correctos en cada endpoint
- [ ] ServiceResponse estructura valida
- [ ] E2E flows ejecutan sin fallos
- [ ] Performance tests dentro de limites
- [ ] Security tests sin vulnerabilidades
- [ ] JWT tokens validos con claims correctos

---

**Generated:** 2026-02-12
**Collection Version:** 2.1.0
**Schema:** https://schema.getpostman.com/json/collection/v2.1.0/collection.json
