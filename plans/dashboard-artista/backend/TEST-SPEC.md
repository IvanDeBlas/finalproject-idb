# Especificación de Tests - Dashboard Artista

Documento detallado de los tests de integración del Dashboard de Artista.

## Cobertura de Endpoints

### 1. Authentication
- **Endpoint**: POST /api/auth/login
- **Autenticacion**: No requiere
- **Proposito**: Obtener JWT token para requests posteriores
- **Tests**:
  - [x] 200 OK con email/password validos
  - [x] Almacena token en variable authToken
  - [x] Token es formato JWT valido (3 partes separadas por .)

### 2. Dashboard Resumen
- **Endpoint**: GET /api/dashboard/resumen
- **Autenticacion**: Bearer Token (requerido)
- **Proposito**: Obtener resumen general de metricas del artista
- **Tests**:
  - [x] 200 OK con token valido
  - [x] Respuesta < 500ms
  - [x] Contiene campos: totalRecaudado, totalBackers, campaniasActivas, campaniasCompletadas, moneda
  - [x] Tipos correctos: numbers para importes y conteos, string para moneda
  - [x] Moneda es EUR
  - [x] 401 Unauthorized sin token

**Validaciones de Negocio**:
- totalRecaudado >= 0
- totalBackers >= 0
- campaniasActivas >= 0
- campaniasCompletadas >= 0
- moneda = "EUR"

### 3. Mis Campanias
- **Endpoint**: GET /api/campanias/mis-campanias?estadoCampaniaId=X&page=Y&pageSize=Z
- **Autenticacion**: Bearer Token (requerido)
- **Proposito**: Listar campanias del artista con paginacion y filtros
- **Tests**:
  - [x] 200 OK - Pagina 1 sin filtros
  - [x] 200 OK - Con filtro estado=EnCurso (estadoCampaniaId=2)
  - [x] Respuesta < 500ms
  - [x] Contiene array items y totalCount
  - [x] Items tienen campos: id, titulo, estado, estadoNombre, importeObjetivo, importeRecaudado, porcentaje, numBackers, diasRestantes, fechaFin, fechaCreacion
  - [x] Porcentaje entre 0-100
  - [x] Se guarda campaniaId de primer item para requests posteriores
  - [x] 401 Unauthorized sin token

**Validaciones de Negocio**:
- porcentaje = (importeRecaudado / importeObjetivo) * 100, redondeado
- porcentaje está entre 0 y 100
- numBackers >= 0
- diasRestantes >= 0 (o negativo si finalizada)
- importeRecaudado <= importeObjetivo * 2 (máximo que se puede recaudar)

**Query Parameters**:
- `page`: número de página (default 1)
- `pageSize`: items por página (default 10)
- `estadoCampaniaId`: ID del estado para filtrar (opcional)

### 4. Backings por Campania
- **Endpoint**: GET /api/campanias/{campaniaId}/backings?page=Y&pageSize=Z
- **Autenticacion**: Bearer Token (requerido)
- **Proposito**: Listar backings de una campania con paginacion
- **Tests**:
  - [x] 200 OK - Pagina 1 (20 items)
  - [x] 200 OK - Pagina 2 (verifica paginacion)
  - [x] Respuesta < 500ms
  - [x] Items contienen campos: id, nombreBacker, emailBacker, rewardNombre, monto, mensaje, esAnonimo, fechaCreacion, fechaRelativa
  - [x] Stats contiene: backingPromedio, rewardMasPopular, ultimoBacking
  - [x] totalRecaudado es suma de montos
  - [x] 404 Not Found con campaniaId invalido (GUID nulo)
  - [x] 401 Unauthorized sin token

**Validaciones de Negocio**:
- nombreBacker: "Anonimo" si esAnonimo=true, caso contrario nombre real
- emailBacker: visible solo para artista (owner de campania)
- monto > 0
- backingPromedio = totalRecaudado / totalCount
- rewardMasPopular: reward con mayor cantidad de backings
- fechaRelativa: formato "hace X horas/dias"

**Query Parameters**:
- `page`: número de página (default 1)
- `pageSize`: items por página (default 20)

### 5. Stats de Campania
- **Endpoint**: GET /api/campanias/{campaniaId}/stats
- **Autenticacion**: Bearer Token (requerido)
- **Proposito**: Obtener estadisticas detalladas y proyecciones de campania
- **Tests**:
  - [x] 200 OK con campaniaId valido
  - [x] Respuesta < 500ms
  - [x] Contiene: importeObjetivo, importeRecaudado, porcentaje, numBackers, backingPromedio, diasRestantes, diasTranscurridos, proyeccionFinal
  - [x] rewardStats es array con estructura correcta
  - [x] progressoPorDia es array con estructura correcta
  - [x] Porcentaje calculado correctamente: (importeRecaudado / importeObjetivo) * 100
  - [x] 404 Not Found con campaniaId invalido
  - [x] 401 Unauthorized sin token

**Validaciones de Negocio**:
- porcentaje = (importeRecaudado / importeObjetivo) * 100
- backingPromedio = importeRecaudado / numBackers (si numBackers > 0)
- diasRestantes = (fechaFin - hoy).Days, >= 0
- diasTranscurridos = (hoy - fechaInicio).Days
- proyeccionFinal = (importeRecaudado / diasTranscurridos) * (fechaFin - fechaInicio).Days
- rewardStats[].porcentaje = (rewardStats[].cantidad / numBackers) * 100
- rewardStats[].total = suma de montos de ese reward

## Matriz de Tests

| # | Folder | Request | Method | Endpoint | Status | Assertions |
|---|--------|---------|--------|----------|--------|-----------|
| 1 | _Setup | Login | POST | /api/auth/login | 200 | 3 |
| 2 | Dashboard | GET Resumen | GET | /api/dashboard/resumen | 200 | 6 |
| 3 | Dashboard | GET Resumen (401) | GET | /api/dashboard/resumen | 401 | 1 |
| 4 | Mis Campanias | GET Mis Campanias P1 | GET | /api/campanias/mis-campanias | 200 | 6 |
| 5 | Mis Campanias | GET Mis Campanias Filter | GET | /api/campanias/mis-campanias | 200 | 2 |
| 6 | Mis Campanias | GET Mis Campanias (401) | GET | /api/campanias/mis-campanias | 401 | 1 |
| 7 | Backings | GET Backings P1 | GET | /api/campanias/{id}/backings | 200 | 6 |
| 8 | Backings | GET Backings P2 | GET | /api/campanias/{id}/backings | 200 | 2 |
| 9 | Backings | GET Backings (404) | GET | /api/campanias/{id}/backings | 404 | 1 |
| 10 | Backings | GET Backings (401) | GET | /api/campanias/{id}/backings | 401 | 1 |
| 11 | Stats | GET Stats | GET | /api/campanias/{id}/stats | 200 | 8 |
| 12 | Stats | GET Stats (404) | GET | /api/campanias/{id}/stats | 404 | 1 |
| 13 | Stats | GET Stats (401) | GET | /api/campanias/{id}/stats | 401 | 1 |

**Total**: 13 requests, 45+ assertions

## Orden de Ejecución

Los tests deben ejecutarse en este orden para que funcione correctamente el flujo:

1. **_Setup > Login** (obtiene authToken)
2. **Dashboard > GET Resumen** (valida acceso autenticado)
3. **Mis Campanias > GET Mis Campanias P1** (obtiene campaniaId)
4. **Mis Campanias > GET Mis Campanias Filter** (valida filtros)
5. **Backings > GET Backings P1** (usa campaniaId)
6. **Backings > GET Backings P2** (paginacion)
7. **Stats > GET Stats** (metricas detalladas)

Los tests de 401 y 404 pueden ejecutarse en cualquier momento.

## Variables de Coleccion

| Variable | Tipo | Scope | Modificable | Default | Descripcion |
|----------|------|-------|------------|---------|-------------|
| baseUrl | string | Coleccion | Sí | http://localhost:5001 | URL base de la API |
| authToken | string | Coleccion | Automatico | (vacio) | JWT token obtenido en Login |
| campaniaId | string | Coleccion | Automatico | (vacio) | GUID de campania obtenido en Mis Campanias |
| testEmail | string | Coleccion | Sí | usuario1@mail.com | Email usuario de test |
| testPassword | string | Coleccion | Sí | 123456 | Password usuario de test |

## Assertions Detalladas

### Login (3 assertions)
```javascript
1. pm.response.to.have.status(200)
2. pm.expect(json.accessToken).to.exist
3. parts.length == 3 (JWT format)
```

### GET Resumen (6 assertions)
```javascript
1. pm.response.to.have.status(200)
2. pm.expect(pm.response.responseTime).to.be.below(500)
3. json.totalRecaudado existe
4. json.totalBackers es number
5. json.campaniasActivas es number
6. json.moneda == 'EUR'
```

### GET Mis Campanias (6 assertions)
```javascript
1. pm.response.to.have.status(200)
2. pm.expect(pm.response.responseTime).to.be.below(500)
3. json.items es array
4. json.totalCount es number
5. items[0].porcentaje entre 0-100
6. items[0].id guardado en campaniaId
```

### GET Backings (6 assertions)
```javascript
1. pm.response.to.have.status(200)
2. pm.expect(pm.response.responseTime).to.be.below(500)
3. json.items es array
4. json.stats.backingPromedio es number
5. json.totalRecaudado es number
6. backing[0].id existe
```

### GET Stats (8 assertions)
```javascript
1. pm.response.to.have.status(200)
2. pm.expect(pm.response.responseTime).to.be.below(500)
3. json.importeObjetivo es number
4. json.rewardStats es array
5. json.progressoPorDia es array
6. porcentaje == (importeRecaudado / importeObjetivo) * 100
7. importeRecaudado <= importeObjetivo * 2
8. diasRestantes >= 0
```

## Escenarios de Error

### 401 Unauthorized
**Cuando**: No se envía Authorization header o token invalido
**Esperado**:
- Status 401
- Mensaje indicando falta de autenticacion

### 404 Not Found
**Cuando**: Se usa campaniaId que no existe (GUID nulo)
**Esperado**:
- Status 404
- Mensaje de recurso no encontrado

### 400 Bad Request (no probado actualmente)
**Escenarios posibles**:
- Query params invalidos (page < 1, pageSize < 0)
- campaña no pertenece al artista autenticado

## Variables de Entorno

### Development (environment.json)
```json
{
  "baseUrl": "http://localhost:5001",
  "testEmail": "usuario1@mail.com",
  "testPassword": "123456"
}
```

### Staging (environment-staging.json)
```json
{
  "baseUrl": "https://staging-api.weplayurises.com",
  "testEmail": "staging-test@mail.com",
  "testPassword": "StagingTestPassword123"
}
```

## Performance Targets

| Endpoint | Target | Umbrales |
|----------|--------|----------|
| GET /dashboard/resumen | < 200ms | Warning: 300ms, Error: 500ms |
| GET /campanias/mis-campanias | < 300ms | Warning: 400ms, Error: 500ms |
| GET /campanias/{id}/backings | < 300ms | Warning: 400ms, Error: 500ms |
| GET /campanias/{id}/stats | < 400ms | Warning: 500ms, Error: 1000ms |

Todos los tests validan < 500ms.

## Casos Especiales

### Artista sin Campanias
- GET /api/campanias/mis-campanias retorna items: [], totalCount: 0
- Variables campaniaId no se llenan
- Tests de Backings y Stats se saltan o usan GUID nulo

### Campania sin Backings
- GET /api/campanias/{id}/backings retorna items: [], totalCount: 0
- stats.backingPromedio es 0
- stats.ultimoBacking es null

### Backing Anonimo
- nombreBacker = "Anonimo"
- esAnonimo = true
- emailBacker se muestra solo si usuario es owner

## Integracion Continua

### GitHub Actions
```yaml
name: Dashboard Artista Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: 18
      - run: npm install -g newman
      - run: |
          newman run ./plans/dashboard-artista/backend/postman-collection.json \
            --environment ./plans/dashboard-artista/backend/environment.json \
            --reporters cli,json \
            --reporter-json-export results.json
      - name: Upload Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: results.json
```

## Notas Finales

- Los tests son independientes del estado previo de la BD
- Usan usuario de test existente (usuario1@mail.com)
- Las variables se guardan automaticamente entre requests
- El flujo principal es: Login -> Resumen -> Mis Campanias -> Backings -> Stats
- Los tests de error (401, 404) pueden ejecutarse en paralelo
