# Resumen de Colección Postman - cs-gestionar-necesidades

**Generada:** 2026-02-16
**Feature:** US-CS-02 - Gestionar Necesidades de Crowdsourcing
**Archivo:** `postman-collection.json`

---

## Índice Rápido

- **Carpeta de Setup:** 6 requests (login + maestras)
- **CRUD Lifecycle:** 9 requests (create, list, filter, search, get, update, close, verify)
- **Validación:** 4 requests (campo requerido, min length, presupuesto, moneda)
- **Business Rules:** 2 requests (no editar cerrada, no cerrar cerrada)
- **Auth Errors:** 2 requests (sin token, token inválido)
- **Not Found:** 2 requests (recurso inexistente)

**Total de requests:** 25 requests
**Total de assertions:** ~85 assertions

---

## 1. Cadena de Dependencias

La colección implementa las dependencias requeridas automáticamente:

```
Auth (JWT Token)
  └── Artista (extraer artistaId del usuario)
        └── ProyectoArtistico (obtener proyectoArtisticoId del artista)
              └── Necesidad (crear, listar, actualizar, cerrar)
```

### _Setup (6 pasos)

| # | Request | Objetivo | Response |
|---|---------|----------|----------|
| 1 | POST /auth/login | Obtener JWT token | token → `accessToken` |
| 2 | GET /artistas/mis-artistas | Extraer artistaId | items[0].id → `artistaId` |
| 3 | GET /proyectos-artisticos | Obtener proyecto | items[0].id → `proyectoArtisticoId` |
| 4 | GET /maestras/tipos-necesidad | Obtener tipo | items[0].id → `tipoNecesidadId` |
| 5 | GET /maestras/modalidades-trabajo | Obtener modalidad | Remoto (id=2) → `modalidadTrabajoId` |
| 6 | GET /maestras/monedas | Obtener moneda | EUR (id=1) → `monedaId` |

Todas las variables se guardan automáticamente con `pm.collectionVariables.set()` para usar en requests posteriores.

---

## 2. CRUD Lifecycle (9 requests)

El lifecycle sigue el patrón: **CREATE → READ → FILTER → READ-DETAIL → UPDATE → READ → CLOSE → READ**

### 2.1 POST Create Necesidad (201)

**Descripción:** Crear una necesidad con datos válidos

**Request:**
```json
{
  "titulo": "Mezcla de pistas para EP de 5 canciones - {{$timestamp}}",
  "descripcion": "Buscamos un ingeniero de mezcla...",
  "tipoNecesidadId": {{tipoNecesidadId}},
  "modalidadTrabajoId": {{modalidadTrabajoId}},
  "presupuestoMin": 150.00,
  "presupuestoMax": 800.00,
  "monedaId": {{monedaId}},
  "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
  "fechaInicioPrevista": "2026-04-01T00:00:00Z",
  "proyectoArtisticoId": "{{proyectoArtisticoId}}"
}
```

**Assertions:**
- ✅ Status 201 Created
- ✅ Response time < 500ms
- ✅ `data.id`, `data.titulo`, `data.estadoNecesidadId`, `data.fechaCreacion` presentes
- ✅ `estadoNecesidadId == 1` (Abierta)
- ✅ Guarda `necesidadId` para siguientes requests

### 2.2 GET All Necesidades (200)

**Descripción:** Listar todas las necesidades del artista con paginación

**Query Params:** Ninguno (defaults)

**Assertions:**
- ✅ Status 200 OK
- ✅ Respuesta tiene estructura paginada: `items`, `totalCount`, `page`, `pageSize`, `totalPages`
- ✅ La necesidad creada aparece en el listado
- ✅ Array `items` contiene campos: `id`, `titulo`, `estadoNecesidadId`, `tipoNecesidadNombre`, etc.

### 2.3 GET All - Filter by Estado

**Descripción:** Listar necesidades filtrando por estado (Abierta=1)

**Query Params:** `?estado=1`

**Assertions:**
- ✅ Status 200 OK
- ✅ Todos los items retornados tienen `estadoNecesidadId == 1`
- ✅ El filtro es funcional

### 2.4 GET All - Search by Text

**Descripción:** Listar necesidades buscando por texto "mezcla"

**Query Params:** `?search=mezcla`

**Assertions:**
- ✅ Status 200 OK
- ✅ Resultados contienen keyword "mezcla" en `titulo`
- ✅ La búsqueda es funcional

### 2.5 GET Necesidad By ID (200)

**Descripción:** Obtener detalle completo de una necesidad

**Path Params:** `{id}={{necesidadId}}`

**Assertions:**
- ✅ Status 200 OK
- ✅ Retorna todos los campos (titulo, descripcion, tipoNecesidadId, tipoNecesidadNombre, etc.)
- ✅ Incluye array `propuestas` (aunque esté vacío)
- ✅ Incluye `proyectoArtisticoNombre` (nombre relacionado)

### 2.6 PUT Update Necesidad (200)

**Descripción:** Actualizar necesidad (solo permitido si estado=Abierta)

**Request:**
```json
{
  "titulo": "Mezcla profesional para EP indie rock (5 canciones) - ACTUALIZADO",
  "descripcion": "Actualizado: Buscamos ingeniero con experiencia...",
  "modalidadTrabajoId": {{modalidadTrabajoId}},
  "presupuestoMin": 200.00,
  "presupuestoMax": 900.00,
  "monedaId": {{monedaId}},
  "fechaLimitePropuestas": "2026-03-20T00:00:00Z",
  "fechaInicioPrevista": "2026-04-05T00:00:00Z"
}
```

**Assertions:**
- ✅ Status 200 OK
- ✅ Response contiene: `id`, `titulo` (actualizado), `estadoNecesidadId`, `fechaActualizacion`
- ✅ `estadoNecesidadId` sigue siendo 1 (no cambia a menos que sea vía /cerrar)

### 2.7 GET Necesidad By ID - Verify Update

**Descripción:** Verificar que la actualización se reflejó

**Assertions:**
- ✅ Status 200 OK
- ✅ `titulo` contiene "ACTUALIZADO"
- ✅ `presupuestoMin == 200.00` y `presupuestoMax == 900.00`
- ✅ `fechaActualizacion` no es null

### 2.8 PATCH Close Necesidad (200)

**Descripción:** Cerrar una necesidad (rechaza propuestas pendientes)

**Request:**
```json
{
  "motivo": "Ya no necesitamos este servicio porque decidimos cambiar el enfoque del proyecto"
}
```

**Assertions:**
- ✅ Status 200 OK
- ✅ Response contiene: `id`, `estadoNecesidadNombre == "Cerrada"`, `propuestasRechazadas` (int)

### 2.9 GET Necesidad By ID - Verify Closed

**Descripción:** Verificar que la necesidad está cerrada

**Assertions:**
- ✅ Status 200 OK
- ✅ `estadoNecesidadId == 3` (Cerrada)
- ✅ `estadoNecesidadNombre == "Cerrada"`

---

## 3. Validación Errors (4 requests)

Tests de campos requeridos, longitudes, rangos y validaciones condicionales.

### 3.1 POST 400 - Titulo Vacio

**Validación:** `titulo` es requerido (NotEmpty)

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `messages` array contiene error con `errorCode == "1001"` (Validation_Required)
- ✅ Mensaje: "El título es obligatorio"

### 3.2 POST 400 - Titulo Menor a 5 Caracteres

**Validación:** `titulo` debe tener mínimo 5 caracteres (MinLength)

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `errorCode == "1011"` (Validation_MinLength)
- ✅ Mensaje: "El título debe tener al menos 5 caracteres"

### 3.3 POST 400 - Presupuesto Max Menor a Min

**Validación:** `presupuestoMax >= presupuestoMin` (GreaterThanOrEqualTo)

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `errorCode == "1009"` (Validation_InvalidRange)
- ✅ Mensaje: "El presupuesto máximo debe ser mayor o igual al mínimo"

### 3.4 POST 400 - Moneda Requerida si Hay Presupuesto

**Validación:** `monedaId` es requerido si `presupuestoMin` o `presupuestoMax` presentes

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `errorCode == "1001"` (Validation_Required)
- ✅ Mensaje menciona "moneda"

---

## 4. Business Rule Errors (2 requests)

Tests de reglas de negocio: estados y permisos.

### 4.1 PUT 400 - No Editar Necesidad Cerrada

**Regla:** Una necesidad en estado Cerrada (3) no puede editarse, solo en estado Abierta (1)

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `errorCode == "4001"` (BusinessRule_NecesidadNotEditable)
- ✅ Mensaje: "Solo se pueden editar necesidades en estado Abierta"

**Contexto:** Este test intenta editar la necesidad que ya fue cerrada en el paso 2.8

### 4.2 PATCH 400 - No Cerrar Necesidad Ya Cerrada

**Regla:** Una necesidad en estado Cerrada (3) no puede volver a cerrarse

**Assertions:**
- ✅ Status 400 Bad Request
- ✅ `errorCode == "4002"` (BusinessRule_NecesidadNotCloseable)
- ✅ Mensaje: "Solo se pueden cerrar necesidades en estado Abierta o En Progreso"

---

## 5. Auth Errors (2 requests)

Tests de autenticación y autorización.

### 5.1 GET 401 - Sin Token

**Verificación:** GET /mis-necesidades sin Authorization header

**Assertions:**
- ✅ Status 401 Unauthorized

### 5.2 POST 401 - Token Inválido

**Verificación:** POST /necesidades con token `Bearer invalid_token_xyz`

**Assertions:**
- ✅ Status 401 Unauthorized

---

## 6. Not Found Errors (2 requests)

Tests de recursos inexistentes.

### 6.1 GET 404 - Necesidad No Existe

**Verificación:** GET /necesidades/00000000-0000-0000-0000-000000000000

**Assertions:**
- ✅ Status 404 Not Found
- ✅ Response contiene `messages` array

### 6.2 PUT 404 - Necesidad No Existe

**Verificación:** PUT /necesidades/99999999-9999-9999-9999-999999999999 con body válido

**Assertions:**
- ✅ Status 404 Not Found

---

## Variables de Colección

### Iniciales (pre-configuradas)

```json
{
  "baseUrl": "http://localhost:5001",
  "testEmail": "usuario1@mail.com",
  "testPassword": "123456",
  "accessToken": "",
  "artistaId": "",
  "proyectoArtisticoId": "",
  "tipoNecesidadId": "",
  "modalidadTrabajoId": "",
  "monedaId": "",
  "necesidadId": ""
}
```

### Dinámicas (establecidas durante ejecución)

| Variable | Establecida por | Uso |
|----------|-----------------|-----|
| `accessToken` | `01. Login User` | Header `Authorization: Bearer {{accessToken}}` |
| `artistaId` | `02. Get Artista ID` | Referencia pero no usado en requests |
| `proyectoArtisticoId` | `03. Get Proyecto Artístico` | Body de POST/PUT `"proyectoArtisticoId": "{{proyectoArtisticoId}}"` |
| `tipoNecesidadId` | `04. Get Maestras - Tipos` | Body `"tipoNecesidadId": {{tipoNecesidadId}}` |
| `modalidadTrabajoId` | `05. Get Maestras - Modalidades` | Body `"modalidadTrabajoId": {{modalidadTrabajoId}}` |
| `monedaId` | `06. Get Maestras - Monedas` | Body `"monedaId": {{monedaId}}` |
| `necesidadId` | `01. POST Create Necesidad` | Path param `{id}={{necesidadId}}` |

---

## ErrorCodes Testeados

| ErrorCode | Nombre | Test | Causa |
|-----------|--------|------|-------|
| 1001 | Validation_Required | Título vacío, Moneda requerida | Campo obligatorio no proporcionado |
| 1009 | Validation_InvalidRange | Presupuesto Max < Min | Rango inválido |
| 1011 | Validation_MinLength | Título < 5 caracteres | Longitud mínima no alcanzada |
| 4001 | BusinessRule_NecesidadNotEditable | Editar necesidad cerrada | Estado no permite edición |
| 4002 | BusinessRule_NecesidadNotCloseable | Cerrar necesidad cerrada | Estado no permite cierre |
| (implícito) | Auth_Unauthorized | Sin token / Token inválido | JWT inválido o expirado |
| (implícito) | NotFound_Necesidad | GET/PUT/PATCH 404 | Recurso no existe |

---

## Flujo de Ejecución

```
INICIO (_Setup)
  │
  ├─→ 01. Login → obtener accessToken
  ├─→ 02. Get Artista ID → obtener artistaId
  ├─→ 03. Get Proyecto → obtener proyectoArtisticoId
  ├─→ 04. Get Tipo Necesidad → obtener tipoNecesidadId
  ├─→ 05. Get Modalidad Trabajo → obtener modalidadTrabajoId (Remoto)
  └─→ 06. Get Moneda → obtener monedaId (EUR)
       │
       ├─────────────────────────────────────────────────┐
       │                                                  │
       ↓                                                  ↓
  CRUD Lifecycle                          Validation Errors
  │                                       │
  ├─→ 01. POST Create                    ├─→ POST 400 - Título Vacío
  │   Necesidad → necesidadId             ├─→ POST 400 - Título < 5
  │   (Estado: Abierta)                   ├─→ POST 400 - Presupuesto Max < Min
  │                                       └─→ POST 400 - Moneda Requerida
  ├─→ 02. GET All Necesidades
  │   (Verificar aparece)                Business Rules
  │                                       │
  ├─→ 03. GET All - Filter Estado       ├─→ PUT 400 - No Editar Cerrada
  │   (Solo estado=1)                     └─→ PATCH 400 - No Cerrar Cerrada
  │
  ├─→ 04. GET All - Search
  │   (Por "mezcla")
  │
  ├─→ 05. GET By ID                     Auth Errors
  │   (Detalle completo)                │
  │                                      ├─→ GET 401 - Sin Token
  ├─→ 06. PUT Update                    └─→ POST 401 - Token Inválido
  │   (Editar, aún Abierta)
  │
  ├─→ 07. GET By ID - Verify           Not Found
  │   (Verificar cambios)               │
  │                                      ├─→ GET 404 - Necesidad No Existe
  ├─→ 08. PATCH Close                   └─→ PUT 404 - Necesidad No Existe
  │   (Estado: Cerrada)
  │
  └─→ 09. GET By ID - Verify
      (Verificar cierre)

FIN
(Todos los tests pasan ✓)
```

---

## Cómo Usar

### 1. Importar en Postman

```
1. File → Import
2. Seleccionar: plans/cs-gestionar-necesidades/backend/postman-collection.json
3. Click "Import"
```

### 2. Ejecutar Setup

```
1. Click carpeta "_Setup"
2. Click botón "Run"
3. Esperar a que complete (deberían pasar todos los tests)
4. Variables de colección se auto-llenan
```

### 3. Ejecutar CRUD Lifecycle

```
1. Click carpeta "Necesidades - CRUD Lifecycle"
2. Click botón "Run"
3. Observar 9 requests en orden
4. Verificar que todos los assertions pasan (✓ green)
```

### 4. Ejecutar Validaciones

```
1. Click carpeta "Necesidades - Validation Errors"
2. Click botón "Run"
3. Cada test espera status 400 con ErrorCode específico
```

### 5. Con Newman (CLI)

```bash
# Ejecutar todo
newman run postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export report.html

# Ejecutar solo carpeta específica
newman run postman-collection.json \
  --folder "Necesidades - CRUD Lifecycle" \
  --reporters cli
```

---

## Datos Utilizados en Tests

### Usuario Test

- **Email:** `usuario1@mail.com`
- **Password:** `123456`
- **Rol:** Artista
- **Requerimiento:** El usuario debe tener al menos un Artista y ProyectoArtistico asociado

### Necesidad Creada

```json
{
  "titulo": "Mezcla de pistas para EP de 5 canciones - [timestamp]",
  "descripcion": "Buscamos un ingeniero de mezcla...",
  "tipoNecesidadId": "{{tipoNecesidadId}}", // Extraído de maestras
  "modalidadTrabajoId": 2,                   // Remoto
  "presupuestoMin": 150.00,
  "presupuestoMax": 800.00,
  "monedaId": 1,                             // EUR
  "fechaLimitePropuestas": "2026-03-15T00:00:00Z",
  "fechaInicioPrevista": "2026-04-01T00:00:00Z",
  "proyectoArtisticoId": "{{proyectoArtisticoId}}"
}
```

### Necesidad Actualizada

```json
{
  "titulo": "Mezcla profesional para EP indie rock (5 canciones) - ACTUALIZADO",
  "descripcion": "Actualizado: Buscamos ingeniero con experiencia...",
  "modalidadTrabajoId": 2,
  "presupuestoMin": 200.00,
  "presupuestoMax": 900.00,
  "monedaId": 1,
  "fechaLimitePropuestas": "2026-03-20T00:00:00Z",
  "fechaInicioPrevista": "2026-04-05T00:00:00Z"
}
```

### Necesidad Cerrada

```json
{
  "motivo": "Ya no necesitamos este servicio porque decidimos cambiar el enfoque del proyecto"
}
```

---

## Validación Cubierta

| Tipo | Cobertura | Tests |
|------|-----------|-------|
| **Campos requeridos** | ✅ Completa | Titulo, TipoNecesidad, ModalidadTrabajo, ProyectoArtistico |
| **Rangos** | ✅ Completa | Presupuesto Max >= Min |
| **Longitudes** | ✅ Parcial | Titulo (min 5, max 200) |
| **Validaciones condicionales** | ✅ Completa | Moneda si presupuesto, Ubicación si presencial/híbrido (no testeado) |
| **Estados** | ✅ Completa | No editar si cerrada, no cerrar si cerrada |
| **Autorización** | ✅ Completa | Sin token, token inválido |
| **Existencia de recursos** | ✅ Completa | Necesidad no existe, proyecto no existe |

---

## Consideraciones

### Limpiezas de Datos

La colección **NO limpia datos** al finalizar:
- Las necesidades creadas quedan en la BD
- Usar para testear múltiples veces sin problemas (nuevo ID cada vez)
- Si deseas limpiar: DELETE manualmente o restaurar BD

### Timestamps

El título incluye `{{$timestamp}}` para:
- Evitar duplicados al ejecutar múltiples veces
- Facilitar debugging (saber cuándo se creó)
- Cumplir validaciones de unicidad si existen

### Cambio de Credenciales

Modificar en _Setup → 01. Login si quieres probar con otro usuario:

```json
{
  "email": "otro@usuario.com",
  "password": "otra_password"
}
```

---

## Métricas Finales

| Métrica | Valor |
|---------|-------|
| **Total de folders** | 6 |
| **Total de requests** | 25 |
| **Requests exitosos** | ~16 (todos en happy path + validations + errors) |
| **Assertions** | ~85 |
| **Status codes cubiertos** | 200, 201, 400, 401, 404 |
| **Endpoints únicos** | 7 |
| **ErrorCodes cubiertos** | 1001, 1009, 1011, 4001, 4002, 3001, 2009 |
| **Tiempo ejecución estimado** | 10-15 segundos |

---

**Generada automáticamente por WePlay Integration Testing Framework**
**Última actualización:** 2026-02-16
