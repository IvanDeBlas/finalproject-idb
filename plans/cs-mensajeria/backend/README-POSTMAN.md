# Colección Postman: WePlay.CsMensajeria.IntegrationTests

Colección de tests de integración ejecutables para la feature **cs-mensajeria** (US-CS-05 - Mensajeria entre Partes).

## Ubicación

```
plans/cs-mensajeria/backend/postman-collection.json
```

## Descripción

La colección implementa tests de integración completos para los 6 endpoints de mensajería:

1. **POST /api/crowdsourcing/conversaciones** - Crear conversación
2. **GET /api/crowdsourcing/conversaciones** - Listar conversaciones (con filtros y paginación)
3. **GET /api/crowdsourcing/conversaciones/no-leidos** - Conteo de no leidos
4. **GET /api/crowdsourcing/conversaciones/{id}/mensajes** - Obtener mensajes de una conversación
5. **POST /api/crowdsourcing/conversaciones/{id}/mensajes** - Enviar mensaje
6. **PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos** - Marcar mensajes como leidos

## Estructura

La colección está organizada en **8 folders** principales:

### 1. **_Setup** (5 requests)
Prepara el ambiente autenticando dos usuarios y creando las dependencias necesarias:
- Register Artista User (genera email único)
- Register Fan/Proveedor User (genera email único)
- Extract Artista UserId from JWT Token
- Create Necesidad (contexto de la conversación)
- Create Propuesta (establece relación entre usuarios)

**Variables auto-pobladas**: `artistaToken`, `fanToken`, `artistaUserId`, `fanUserId`, `necesidadId`

### 2. **Conversaciones - CRUD Lifecycle** (4 requests)
Flujo principal de CRUD:
- 01. POST Create Conversacion (establece `conversacionId`)
- 02. GET All Conversaciones (verifica aparición en listado)
- 03. GET Conversacion By Id / Mensajes (verifica acceso)
- 04. GET No Leidos Count (endpoint ligero)

**Assertions**: Validación de estructura de respuesta, presencia de campos obligatorios, códigos de éxito

### 3. **Mensajes - CRUD Lifecycle** (5 requests)
Flujo de mensajes:
- 01. POST Send Mensaje (sin adjunto)
- 02. POST Send Mensaje with URL (con adjunto válido)
- 03. GET Mensajes from Conversacion (lista paginada)
- 04. PATCH Marcar Leidos (marca mensajes como leidos)
- 05. Verify Total No Leidos Decreased (verifica conteo actualizado)

**Assertions**: Validación de `esPropio`, `leido`, presencia de URL, conteo de mensajes

### 4. **Conversaciones - Validation Errors** (6 requests)
Casos de error 400 (validación):
- POST 400 - Asunto Empty (1001)
- POST 400 - Asunto Too Long (1002)
- POST 400 - Missing Destinatario (1001)
- POST 400 - Contenido Mensaje Empty (1001)
- POST 400 - Contenido Mensaje Too Long (1002)
- POST 400 - Invalid URL Adjunto (1013)

**Assertions**: Validación de error codes específicos (1001, 1002, 1013)

### 5. **Conversaciones - Auth Errors** (3 requests)
Casos de error 401/403 (autorización):
- GET 401 - Missing Token
- POST 401 - Invalid Token
- GET 403 - Not Participant (3002)

**Assertions**: Validación de status codes y error codes de autenticación

### 6. **Conversaciones - Not Found** (3 requests)
Casos de error 404:
- GET 404 - Conversacion Not Found (2014)
- POST 404 - Send Mensaje to Non-existent Conversacion (2014)
- PATCH 404 - Marcar Leidos Non-existent Conversacion (2014)

**Assertions**: Validación de error code 2014 y status 404

### 7. **Conversaciones - Business Rules** (1 request)
Casos de error 400 (reglas de negocio):
- POST 400 - Duplicate Conversacion (4015)

**Assertions**: Validación de error code 4015 (conversación duplicada para el mismo contexto)

### 8. **Paginación Tests** (2 requests)
Validación de límites de paginación:
- GET Conversaciones with Page Size Limit (max 50)
- GET Mensajes with Page Size Limit (max 100)

**Assertions**: Validación de respeto de limites de `pageSize`

### 9. **Filtros Tests** (1 request)
Validación de filtrado por contexto:
- GET Conversaciones Filtered by Contexto=necesidades

**Assertions**: Validación de filtro `contexto` (solo retorna conversaciones de tipo "necesidad")

## Datos y Variables

### Variables de Colección

| Variable | Descripción | Poblada por |
|----------|-------------|------------|
| `baseUrl` | URL base del API | Manual (default: http://localhost:5001) |
| `testPassword` | Contraseña de prueba | Manual (default: TestPassword123!) |
| `artistaEmail` | Email del usuario artista | Setup (generado con timestamp) |
| `artistaToken` | JWT token del artista | Setup (extraído de respuesta) |
| `artistaUserId` | UserId del artista | Setup (extraído del token JWT) |
| `fanEmail` | Email del usuario fan/proveedor | Setup (generado con timestamp) |
| `fanToken` | JWT token del fan | Setup (extraído de respuesta) |
| `fanUserId` | UserId del fan | Setup (extraído de respuesta) |
| `necesidadId` | ID de la necesidad creada | Setup (Crear Necesidad) |
| `propuestaId` | ID de la propuesta | Setup (Crear Propuesta) |
| `conversacionId` | ID de la conversación creada | Lifecycle 01 (POST Create) |
| `mensajeId` | ID del primer mensaje | Lifecycle 01 (POST Send Mensaje) |
| `mensajeId2` | ID del segundo mensaje | Lifecycle 02 (POST Send Mensaje with URL) |
| `randomToken` | Token inválido para tests de auth | Manual (token JWT falso) |

### Generación de Datos Únicos

El _Setup genera emails únicos con timestamp para cada ejecución:

```javascript
const email = 'artista-' + Date.now() + '@weplay.com';
pm.collectionVariables.set('artistaEmail', email);
```

Esto permite ejecutar la colección múltiples veces sin conflictos de usuario duplicado.

## Precondiciones

### Backend

- Backend API ejecutándose en `http://localhost:5001`
- Swagger disponible en `http://localhost:5001/swagger`
- Base de datos SqlServer/LocalDB con migraciones aplicadas
- JWT configurado con:
  - **Key**: WePlayRisesDockerSecretKey123456789 (o la configurada)
  - **Issuer**: WePlayRises
  - **Audience**: WePlayRisesUsers/WePlayRisesClient

### Endpoints Requeridos

- `POST /api/auth/register` - Autenticación
- `POST /api/crowdsourcing/necesidades` - Crear contexto
- `POST /api/crowdsourcing/propuestas` - Crear relación entre usuarios
- `POST /api/crowdsourcing/conversaciones` - Crear conversación
- `GET /api/crowdsourcing/conversaciones` - Listar conversaciones
- `GET /api/crowdsourcing/conversaciones/no-leidos` - Conteo de no leidos
- `GET /api/crowdsourcing/conversaciones/{id}/mensajes` - Obtener mensajes
- `POST /api/crowdsourcing/conversaciones/{id}/mensajes` - Enviar mensaje
- `PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos` - Marcar leidos

## Instrucciones de Ejecución

### Opción 1: Postman UI (Interactivo)

1. Abrir Postman
2. File → Import → Seleccionar `/plans/cs-mensajeria/backend/postman-collection.json`
3. Verificar que `baseUrl` esté configurado a `http://localhost:5001`
4. Click derecho en la colección → "Run collection"
5. Seleccionar los folders a ejecutar
6. Hacer click en "Run WePlay.CsMensajeria.IntegrationTests"

### Opción 2: Newman CLI (Recomendado para CI/CD)

```bash
# Instalar Newman si no lo tienes
npm install -g newman

# Ejecutar colección completa
newman run plans/cs-mensajeria/backend/postman-collection.json \
  --environment <env-file> \
  --reporters cli,json,html \
  --reporter-json-export results.json \
  --reporter-html-export results.html

# Ejecutar solo un folder
newman run plans/cs-mensajeria/backend/postman-collection.json \
  --folder "Conversaciones - CRUD Lifecycle" \
  --reporters cli

# Ejecutar con variables personalizadas
newman run plans/cs-mensajeria/backend/postman-collection.json \
  --var baseUrl=http://localhost:5001 \
  --var testPassword=TestPassword123! \
  --reporters cli

# Ejecutar con delays entre requests
newman run plans/cs-mensajeria/backend/postman-collection.json \
  --delay-request 100 \
  --reporters cli
```

### Opción 3: Script Bash (One-liner)

```bash
cd /c/Repos/WePlay_Rises

newman run plans/cs-mensajeria/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export plans/cs-mensajeria/backend/test-results.html
```

## Resultados Esperados

### Ejecución Exitosa

```
✓ WePlay.CsMensajeria.IntegrationTests

    _Setup
    ✓ 01. Register Artista User
    ✓ 02. Register Fan/Proveedor User
    ✓ 03. Extract Artista UserId from Token
    ✓ 04. Create Necesidad
    ✓ 05. Create Propuesta

    Conversaciones - CRUD Lifecycle
    ✓ 01. POST Create Conversacion
    ✓ 02. GET All Conversaciones
    ✓ 03. GET Conversacion By Id
    ✓ 04. GET No Leidos Count

    Mensajes - CRUD Lifecycle
    ✓ 01. POST Send Mensaje
    ✓ 02. POST Send Mensaje with URL
    ✓ 03. GET Mensajes from Conversacion
    ✓ 04. PATCH Marcar Leidos
    ✓ 05. Verify Total No Leidos Decreased

    Conversaciones - Validation Errors
    ✓ POST 400 - Asunto Empty
    ✓ POST 400 - Asunto Too Long
    ✓ POST 400 - Missing Destinatario
    ✓ POST 400 - Contenido Mensaje Empty
    ✓ POST 400 - Contenido Mensaje Too Long
    ✓ POST 400 - Invalid URL Adjunto

    Conversaciones - Auth Errors
    ✓ GET 401 - Missing Token
    ✓ POST 401 - Invalid Token
    ✓ GET 403 - Not Participant

    Conversaciones - Not Found
    ✓ GET 404 - Conversacion Not Found
    ✓ POST 404 - Send Mensaje to Non-existent Conversacion
    ✓ PATCH 404 - Marcar Leidos Non-existent Conversacion

    Conversaciones - Business Rules
    ✓ POST 400 - Duplicate Conversacion

    Paginación Tests
    ✓ GET Conversaciones with Page Size Limit
    ✓ GET Mensajes with Page Size Limit

    Filtros Tests
    ✓ GET Conversaciones Filtered by Contexto=necesidades

    40 tests passed (30s)
    0 failed
```

## Depuración

### Habilitar Logs Detallados

En Newman:
```bash
newman run plans/cs-mensajeria/backend/postman-collection.json \
  --reporters cli \
  -v
```

En Postman UI:
- View → Show DevTools (Ctrl+Alt+I)
- Pestaña "Console" para ver logs
- Pestaña "Network" para ver requests/responses

### Inspeccionar Variables

En la ejecución, Postman mostrará el valor de las variables en cada paso. Para inspeccionar valores intermedios:

```javascript
// En pre-request o test scripts
console.log('Current conversacionId:', pm.collectionVariables.get('conversacionId'));
console.log('Artista Token:', pm.collectionVariables.get('artistaToken'));
```

### Errores Comunes

| Error | Causa | Solución |
|-------|-------|----------|
| `401 Unauthorized` | Token JWT inválido o expirado | Verificar que _Setup se ejecutó correctamente; revisar JWT key en backend |
| `404 Not Found` en setup | Endpoints de necesidades/propuestas no existen | Verificar que módulo Crowdsourcing está deployed |
| `400 Bad Request` en conversación | Email duplicado | Esperar a que el timestamp cambie; ejecutar otra vez |
| Timeout | Backend lento o no responde | Aumentar timeout en Newman: `--timeout-request 10000` |
| CORS error | Frontend llamando desde otro puerto | Verificar CORS en backend; no aplica para API directo |

## Métricas Esperadas

| Métrica | Valor | Notas |
|---------|-------|-------|
| Total Requests | 40 | 5 setup + 4 lifecycle + 5 mensajes + 6 validación + 3 auth + 3 not found + 1 business rules + 2 paginación + 1 filtros |
| Total Assertions | ~70+ | Mínimo 2 assertions por request |
| Tiempo Total | 20-40s | Depende de latencia del backend |
| Success Rate | 100% | Todos los tests deben pasar |
| Coverage de Endpoints | 6/6 | Todos los endpoints testeados |
| Coverage de Errores | 14 | Validación (6) + Auth (3) + NotFound (3) + BusinessRules (1) + Paginación (2) + Filtros (1) |

## Integración con CI/CD

### GitHub Actions

```yaml
name: API Integration Tests - Mensajeria

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    services:
      sqlserver:
        image: mcr.microsoft.com/mssql/server
        env:
          SA_PASSWORD: WePlayRises2024!
          ACCEPT_EULA: Y

    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'

      - name: Install Newman
        run: npm install -g newman newman-reporter-htmlextra

      - name: Wait for Backend
        run: npx wait-on http://localhost:5001 --timeout 30000

      - name: Run Postman Collection
        run: |
          newman run plans/cs-mensajeria/backend/postman-collection.json \
            --reporters cli,htmlextra \
            --reporter-htmlextra-export test-results.html

      - name: Upload Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: test-results.html
```

### Dockerfile

```dockerfile
FROM node:18-alpine
WORKDIR /app
RUN npm install -g newman newman-reporter-htmlextra
COPY plans/cs-mensajeria/backend/postman-collection.json .
CMD ["newman", "run", "postman-collection.json", "--reporters", "cli,htmlextra"]
```

## Limitaciones y TODOs

1. **Sin WebSocket**: El MVP usa polling (10s). Los tests no validan real-time updates.
2. **Sin notificaciones**: Tests no validan push notifications o emails.
3. **Sin archivos adjuntos**: URLs adjuntas solo se validan como formato; no se descarga contenido.
4. **Usuarios de prueba**: Se crea nuevo usuario por ejecución; para CI/CD considerar reutilizar usuarios.
5. **Cleanup**: No hay _Teardown automático. Las conversaciones y mensajes quedan en BD para análisis posterior.

## Referencias

- Especificación de contratos: `/docs/user-stories/cs-mensajeria/contracts.md`
- Feature spec: `/docs/user-stories/cs-mensajeria/feature-spec.md`
- Plan de implementación: `/plans/cs-mensajeria/backend/api-contracts.md`
- Postman Collection Schema: https://schema.getpostman.com/json/collection/v2.1.0/collection.json

## Contacto

Para issues o mejoras en la colección Postman, contactar al equipo backend.

---

**Última actualización**: 2026-02-18
**Colección versión**: 1.0
**Schema Postman**: v2.1.0
