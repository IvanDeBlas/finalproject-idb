# Colección Postman - cs-gestionar-necesidades

**Archivo:** `postman-collection.json`

**Descripción:** Colección de tests de integración 100% auto-inclusiva para la feature de Gestionar Necesidades de Crowdsourcing (US-CS-02).

---

## Características

- ✅ **100% Auto-inclusivo:** No requiere datos pre-existentes en la BD
- ✅ **CRUD Lifecycle completo:** POST → GET ALL → GET ID → PUT → GET ID → PATCH → GET ID
- ✅ **Validación errors:** Tests para 400 Bad Request con ErrorCodes específicos
- ✅ **Business rule errors:** Tests para estados (no editable si cerrada, etc)
- ✅ **Auth errors:** Tests para 401 Unauthorized y tokens inválidos
- ✅ **Not Found errors:** Tests para 404 cuando recurso no existe
- ✅ **Assertions comprehensivas:** +80 assertions distribuidas
- ✅ **Estructura de carpetas:** Organización por tipo de test (CRUD, Validation, Auth, NotFound)

---

## Requisitos Previos

1. **Backend ejecutándose:** `http://localhost:5001`
   ```bash
   cd src/api
   dotnet run --project WebApi
   ```

2. **Base de datos con datos de prueba:**
   - Usuario test: `usuario1@mail.com` / `123456`
   - El usuario debe tener un Artista asociado
   - El artista debe tener al menos un ProyectoArtistico

3. **Maestras pobladas:**
   - TiposNecesidad
   - ModalidadesTrabajo (Presencial=1, Remoto=2, Híbrido=3)
   - Monedas (EUR, USD, GBP)
   - EstadosNecesidad (Abierta=1, EnProgreso=2, Cerrada=3, Cancelada=4)

4. **Newman instalado** (opcional, para CLI):
   ```bash
   npm install -g newman
   npm install -g newman-reporter-htmlextra
   ```

---

## Estructura de la Colección

```
WePlay.cs-gestionar-necesidades.IntegrationTests
│
├── _Setup                          # Inicialización: Login + Obtener maestras
│   ├── 01. Login User              # Obtener JWT token
│   ├── 02. Get Artista ID          # Extraer artistaId
│   ├── 03. Get Proyecto Artístico  # Obtener proyectoArtisticoId
│   ├── 04. Get Maestras - Tipos    # tipoNecesidadId
│   ├── 05. Get Maestras - Modalidad# modalidadTrabajoId
│   └── 06. Get Maestras - Monedas  # monedaId
│
├── Necesidades - CRUD Lifecycle    # Tests funcionales del flujo principal
│   ├── 01. POST Create (201)       # Crear necesidad → guardar ID
│   ├── 02. GET All (200)           # Listar, verificar aparece la creada
│   ├── 03. GET All - Filter Estado # Filtro por estado=1 (Abierta)
│   ├── 04. GET All - Search        # Búsqueda por texto "mezcla"
│   ├── 05. GET By ID (200)         # Obtener detalle completo
│   ├── 06. PUT Update (200)        # Editar necesidad
│   ├── 07. GET By ID - Verify      # Verificar actualización
│   ├── 08. PATCH Close (200)       # Cerrar necesidad
│   └── 09. GET By ID - Verify      # Verificar cierre (estado=3)
│
├── Necesidades - Validation Errors # Tests de validación (400)
│   ├── POST 400 - Titulo Vacio     # 1001: Validation_Required
│   ├── POST 400 - Titulo < 5       # 1011: Validation_MinLength
│   ├── POST 400 - Presupuesto Max < Min # 1009: Validation_InvalidRange
│   └── POST 400 - Falta Moneda     # 1001: Validation_Required (condicional)
│
├── Necesidades - Business Rules    # Tests de reglas de negocio (400)
│   ├── PUT 400 - No Editar Cerrada # 4001: BusinessRule_NecesidadNotEditable
│   └── PATCH 400 - No Cerrar Cerrada # 4002: BusinessRule_NecesidadNotCloseable
│
├── Necesidades - Auth Errors       # Tests de autenticación (401)
│   ├── GET 401 - Sin Token         # Sin Authorization header
│   └── POST 401 - Token Inválido   # Token JWT inválido
│
└── Necesidades - Not Found         # Tests de no encontrado (404)
    ├── GET 404 - Necesidad No Existe
    └── PUT 404 - Necesidad No Existe
```

---

## Variables de Colección

La colección define automáticamente y actualiza estas variables:

| Variable | Descripción | Establecido por |
|----------|-------------|-----------------|
| `baseUrl` | Base URL del backend | Inicialmente `http://localhost:5001` |
| `testEmail` | Email usuario prueba | Inicialmente `usuario1@mail.com` |
| `testPassword` | Password usuario prueba | Inicialmente `123456` |
| `accessToken` | JWT token obtenido en login | `01. Login User` |
| `artistaId` | ID del artista del usuario | `02. Get Artista ID` |
| `proyectoArtisticoId` | ID del proyecto artístico | `03. Get Proyecto Artístico` |
| `tipoNecesidadId` | ID del tipo de necesidad | `04. Get Maestras - Tipos` |
| `modalidadTrabajoId` | ID modalidad trabajo (Remoto=2) | `05. Get Maestras - Modalidad` |
| `monedaId` | ID moneda (EUR=1) | `06. Get Maestras - Monedas` |
| `necesidadId` | ID de la necesidad creada | `01. POST Create Necesidad` |

---

## Cómo Ejecutar

### 1. En Postman UI

```
1. Abre Postman
2. Click "File" → "Import"
3. Selecciona: plans/cs-gestionar-necesidades/backend/postman-collection.json
4. Haz click en la carpeta "_Setup" y click "Run"
5. Click "Run" en el modal para ejecutar todos los _Setup steps
6. Luego, haz click en "Necesidades - CRUD Lifecycle" y click "Run"
7. Visualiza los resultados y assertions en la pestaña "Test Results"
```

### 2. Con Newman (CLI)

#### Ejecutar todo (Setup + CRUD + Validations + Auth + NotFound):
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/cs-necesidades.html
```

#### Ejecutar solo Setup:
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --folder "_Setup" \
  --reporters cli
```

#### Ejecutar solo CRUD Lifecycle:
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --folder "Necesidades - CRUD Lifecycle" \
  --reporters cli
```

#### Ejecutar todos los tests de validación:
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --folder "Necesidades - Validation Errors" \
  --reporters cli
```

#### Con variables de entorno (opcional):
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --environment postman-env.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/cs-necesidades-$(date +%Y%m%d_%H%M%S).html
```

---

## Resultados Esperados

### Tests Exitosos

Todos los tests deben pasar (✓ green):

```
✓ _Setup (6 requests)
  ✓ 01. Login User
  ✓ 02. Get Artista ID
  ✓ 03. Get Proyecto Artístico
  ✓ 04. Get Maestras - TiposNecesidad
  ✓ 05. Get Maestras - ModalidadesTrabajo
  ✓ 06. Get Maestras - Monedas

✓ Necesidades - CRUD Lifecycle (9 requests)
  ✓ 01. POST Create Necesidad (201)
  ✓ 02. GET All Necesidades (200)
  ✓ 03. GET All - Filter by Estado
  ✓ 04. GET All - Search by Text
  ✓ 05. GET Necesidad By ID (200)
  ✓ 06. PUT Update Necesidad (200)
  ✓ 07. GET Necesidad By ID - Verify Update
  ✓ 08. PATCH Close Necesidad (200)
  ✓ 09. GET Necesidad By ID - Verify Closed

✓ Necesidades - Validation Errors (4 requests)
  ✓ POST 400 - Titulo Vacio
  ✓ POST 400 - Titulo Menor a 5 Caracteres
  ✓ POST 400 - Presupuesto Max Menor a Min
  ✓ POST 400 - Moneda Requerida si Hay Presupuesto

✓ Necesidades - Business Rule Errors (2 requests)
  ✓ PUT 400 - No Editar Necesidad Cerrada
  ✓ PATCH 400 - No Cerrar Necesidad Ya Cerrada

✓ Necesidades - Auth Errors (2 requests)
  ✓ GET 401 - Sin Token
  ✓ POST 401 - Token Inválido

✓ Necesidades - Not Found (2 requests)
  ✓ GET 404 - Necesidad No Existe
  ✓ PUT 404 - Necesidad No Existe
```

---

## Assertions por Categoría

### Setup (6 assertions)
- ✓ Login retorna status 200 + token
- ✓ Extrae artistaId desde respuesta
- ✓ Obtiene proyectoArtisticoId del artista
- ✓ Obtiene maestras (tipos, modalidades, monedas)
- ✓ Propaga variables para siguientes requests

### CRUD Lifecycle (55+ assertions)

**Create (POST):**
- ✓ Status 201 Created
- ✓ Response time < 500ms
- ✓ ServiceResponse structure (data + messages)
- ✓ Response contiene id, titulo, estado, fechaCreacion
- ✓ EstadoNecesidadId = 1 (Abierta)
- ✓ Guarda necesidadId para usar en siguientes requests

**List (GET /mis-necesidades):**
- ✓ Status 200 OK
- ✓ Respuesta tiene estructura paginada (items, totalCount, page, pageSize, totalPages)
- ✓ Necesidad creada aparece en listado
- ✓ Filtro por estado funciona (solo estado=1)
- ✓ Búsqueda por texto funciona ("mezcla")

**Get By ID:**
- ✓ Status 200 OK
- ✓ Respuesta contiene todos los campos (incluyen nombres de maestras)
- ✓ Incluye array de propuestas (aunque esté vacío)
- ✓ ID coincide con el guardado

**Update (PUT):**
- ✓ Status 200 OK
- ✓ Actualiza titulo y presupuesto
- ✓ Mantiene estado = Abierta
- ✓ Establece fechaActualizacion

**Close (PATCH):**
- ✓ Status 200 OK
- ✓ Retorna estado = Cerrada (3)
- ✓ Retorna propuestasRechazadas counter

**Verify After Operations:**
- ✓ Cambios reflejados en siguiente GET

### Validation Errors (4 tests × 5 assertions = 20 assertions)
- ✓ Status 400 Bad Request
- ✓ Response tiene array de mensajes de error
- ✓ ErrorCode correcto (1001, 1011, 1009, etc)
- ✓ Mensaje descriptivo presente
- ✓ JSON válido en respuesta

### Business Rule Errors (2 tests × 3 assertions = 6 assertions)
- ✓ Status 400 Bad Request
- ✓ ErrorCode 4001 (no editable si cerrada)
- ✓ ErrorCode 4002 (no closeable si ya cerrada)

### Auth Errors (2 tests × 1 assertion = 2 assertions)
- ✓ Status 401 Unauthorized (sin token o token inválido)

### Not Found (2 tests × 1 assertion = 2 assertions)
- ✓ Status 404 Not Found para necesidad inexistente

**Total de assertions: ~85 assertions**

---

## Endpoints Testeados

### Necesidades
| Método | Ruta | Test | Expected |
|--------|------|------|----------|
| POST | `/api/crowdsourcing/necesidades` | Create | 201 |
| GET | `/api/crowdsourcing/necesidades/mis-necesidades` | List | 200 |
| GET | `/api/crowdsourcing/necesidades/mis-necesidades?estado=1` | Filter | 200 |
| GET | `/api/crowdsourcing/necesidades/mis-necesidades?search=mezcla` | Search | 200 |
| GET | `/api/crowdsourcing/necesidades/{id}` | Detail | 200 / 404 |
| PUT | `/api/crowdsourcing/necesidades/{id}` | Update | 200 / 400 / 404 |
| PATCH | `/api/crowdsourcing/necesidades/{id}/cerrar` | Close | 200 / 400 / 404 |

### Maestras
| Método | Ruta | Test | Expected |
|--------|------|------|----------|
| GET | `/api/crowdsourcing/maestras/tipos-necesidad` | Setup | 200 |
| GET | `/api/crowdsourcing/maestras/modalidades-trabajo` | Setup | 200 |
| GET | `/api/crowdsourcing/maestras/monedas` | Setup | 200 |

### Auth & Artista
| Método | Ruta | Test | Expected |
|--------|------|------|----------|
| POST | `/api/auth/login` | Setup | 200 |
| GET | `/api/artistas/mis-artistas` | Setup | 200 |
| GET | `/api/proyectos-artisticos` | Setup | 200 |

---

## Troubleshooting

### Error: "Necesidad no encontrada (404)"
**Causa:** El `necesidadId` no se guardó correctamente en _Setup
**Solución:** Verifica que el paso "01. POST Create Necesidad" se ejecutó exitosamente

### Error: "Token no válido (401)"
**Causa:** El token JWT expiró o es inválido
**Solución:** Re-ejecuta el paso "_Setup" para obtener un nuevo token

### Error: "El tipo de necesidad no existe"
**Causa:** El `tipoNecesidadId` no se extrajo correctamente
**Solución:** Verifica que el endpoint `GET /api/crowdsourcing/maestras/tipos-necesidad` retorna data

### Error: "El proyecto artístico no existe"
**Causa:** El usuario `usuario1@mail.com` no tiene un ProyectoArtistico asociado
**Solución:**
1. Crea un ProyectoArtistico para el usuario vía Postman/API
2. O usa otro usuario que tenga proyectos

### Error: "BadRequest - Presupuesto máximo debe ser mayor o igual al mínimo"
**Causa:** La request tiene `presupuestoMax < presupuestoMin`
**Solución:** Normal en tests de validación. Verifica que el test espera status 400

---

## Modificaciones Permitidas

Puedes ajustar estas variables sin afectar los tests:

```javascript
// En _Setup → 01. Login User
const email = 'usuario1@mail.com';  // Cambiar usuario
const password = '123456';            // Cambiar password

// En Necesidades → 01. POST Create
const titulo = 'Tu título aquí';
const descripcion = 'Tu descripción';
const presupuestoMin = 200;           // Ajustar presupuesto
const presupuestoMax = 1000;
```

---

## Exportar Reportes

### HTML Report con Newman
```bash
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --reporters htmlextra \
  --reporter-htmlextra-export reports/necesidades-$(date +%Y%m%d_%H%M%S).html
```

Abre el reporte HTML en el navegador para ver:
- Timeline de requests
- Assertions pasadas/fallidas
- Response bodies
- Request details
- Environment snapshot

---

## Notas de Implementación

- **Timestamps:** Los títulos incluyen `{{$timestamp}}` para evitar colisiones al ejecutar múltiples veces
- **Dates:** Las fechas son dinámicas (futuro relativo) para que no expiren
- **Cleanup:** La colección NO limpia datos (el test lifecycle los deja en BD)
  - Usa `DELETE` endpoints si existen para cleanup manual
  - O restaura BD desde backup

---

## Próximos Pasos

1. **Integración CI/CD:**
   ```bash
   # En GitHub Actions / GitLab CI
   newman run postman-collection.json --reporters cli,json
   ```

2. **Load Testing:**
   ```bash
   npm install -g artillery
   artillery run load-test.yml
   ```

3. **Integración con Tests E2E:**
   - Usar mismos datos generados por Postman
   - Ejecutar Postman tests antes de E2E
   - Verificar consistencia de datos

---

**Mantenida por:** WePlay Development Team
**Última actualización:** 2026-02-16
**Versión:** 1.0.0
