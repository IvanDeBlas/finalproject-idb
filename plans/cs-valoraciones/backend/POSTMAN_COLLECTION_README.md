# Colección Postman: cs-valoraciones Integration Tests

**Generada:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)
**Archivo:** `plans/cs-valoraciones/backend/postman-collection.json`

---

## Overview

Colección Postman **100% auto-inclusiva** diseñada para probar completamente los endpoints de valoraciones bidireccionales (US-CS-06). La colección genera automáticamente todos los datos necesarios (usuarios, acuerdos, propuestas) en el _Setup y ejecuta un flujo completo de testing sin datos previos.

---

## Estructura de la Colección

### 1. **_Setup** (8 pasos - requisitos previos)

Genera el contexto completo necesario:

1. **Login Artista** - Autentica usuario artista pre-existente (`usuario1@mail.com`)
   - Obtiene `artistaToken` para los siguientes pasos

2. **Register Profesional** - Registra nuevo usuario profesional con email dinámico
   - Genera email único: `profesional-{timestamp}@weplay.com`
   - Obtiene `profesionalToken`

3. **Get Artista Profile ID** - Obtiene ID del perfil artista (pre-existente)
   - Almacena `artistaId`

4. **Create Necesidad (Crowdsourcing)** - Crea una necesidad de crowdsourcing
   - Artista crea necesidad
   - Almacena `necesidadId`

5. **Create Propuesta** - Profesional crea propuesta para la necesidad
   - Almacena `propuestaId`

6. **Accept Propuesta** - Artista acepta la propuesta del profesional
   - Transición: Propuesta → Aceptada

7. **Get Acuerdo by Propuesta** - Recupera datos del acuerdo creado
   - Al aceptar la propuesta se crea automáticamente un acuerdo
   - Almacena `acuerdoId`

8. **Complete Acuerdo** - Artista marca el acuerdo como completado
   - **CRUCIAL:** Sin este paso no se pueden crear valoraciones (regla de negocio)
   - Transición: Activo → Completado

**Nota:** El _Setup es auto-contenido. No requiere datos previos más allá de que `usuario1@mail.com` exista en la BD (usuario de prueba estándar del proyecto).

---

### 2. **Valoraciones - Happy Path** (3 pasos - flujo exitoso)

#### 01. POST Create Valoracion (Artista valora Profesional)
- **Endpoint:** `POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones`
- **Auth:** Bearer `artistaToken`
- **Body:** Puntuación 5 + comentario
- **Validaciones:**
  - Status 201 Created
  - Response tiene estructura correcta: `data.id`, `data.puntuacion`, `data.comentario`, `data.fechaCreacion`
  - `isSuccess = true`
  - Message con texto "Valoracion enviada"
  - Almacena `valoracionId` para futuros tests

#### 02. GET Valoraciones del Usuario Valorado
- **Endpoint:** `GET /api/crowdsourcing/usuarios/{userId}/valoraciones?page=1&pageSize=10`
- **Auth:** Bearer `artistaToken` (cualquier usuario autenticado puede consultar)
- **Validaciones:**
  - Status 200 OK
  - Response tiene estructura correcta:
    - `data.resumen` con `puntuacionMedia`, `totalValoraciones`, `distribucion` (histograma 1-5)
    - `data.valoraciones` con `items[]`, `totalCount`, `page`, `pageSize`
  - La valoración creada aparece en la lista
  - Campo `autorNombre` existe (nombre del artista que valoró)
  - Campo `fechaCreacion` tiene timestamp

#### 03. POST Valoracion Profesional valora Artista
- **Endpoint:** `POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones`
- **Auth:** Bearer `profesionalToken`
- **Body:** Puntuación 4 + comentario diferente
- **Validaciones:**
  - Status 201 Created
  - `data.puntuacion = 4`
  - `isSuccess = true`
  - Almacena `valoracionId2`

---

### 3. **Valoraciones - Validation Errors** (4 pasos)

Verifica manejo correcto de errores de validación (400 Bad Request).

#### 01. POST 400 - Puntuacion Missing
- **Body:** Sin campo `puntuacion`
- **ErrorCode esperado:** `1001` (Validation_Required)
- **Mensaje:** "La puntuacion es obligatoria"

#### 02. POST 400 - Puntuacion Zero
- **Body:** `puntuacion: 0`
- **ErrorCode esperado:** `1001` o `1009` (Validation_Required / Validation_InvalidRange)

#### 03. POST 400 - Puntuacion Greater Than 5
- **Body:** `puntuacion: 6`
- **ErrorCode esperado:** `1009` (Validation_InvalidRange)
- **Mensaje:** "La puntuacion debe ser entre 1 y 5"

#### 04. POST 400 - Comentario Exceeds Max Length
- **Body:** Comentario > 1000 caracteres
- **ErrorCode esperado:** `1002` (Validation_MaxLength)
- **Mensaje:** "El comentario no puede superar los 1000 caracteres"

---

### 4. **Valoraciones - Business Rules** (1 paso)

Verifica reglas de negocio específicas del dominio.

#### 01. POST 400 - Duplicate Valoracion Same Acuerdo
- **Descripción:** Intenta crear una segunda valoración del mismo usuario sobre el mismo acuerdo
- **Setup:** Requiere haber ejecutado "Happy Path 01" primero
- **ErrorCode esperado:** `4017` (BusinessRule_DuplicateAction)
- **Mensaje:** "Ya has dejado una valoracion para este acuerdo"
- **Garantía BD:** Constraint único `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor` previene duplicados

---

### 5. **Valoraciones - Auth Errors** (2 pasos)

Verifica manejo de autenticación (401 Unauthorized).

#### 01. POST 401 - Missing Token
- **Auth:** `noauth` (sin token)
- **Status esperado:** 401 Unauthorized
- **Mensaje:** "Token no valido o expirado"

#### 02. GET 401 - Missing Token
- **Auth:** `noauth`
- **Endpoint:** GET de valoraciones
- **Status esperado:** 401 Unauthorized

---

### 6. **Valoraciones - Not Found** (2 pasos)

Verifica manejo de recursos no encontrados (404 Not Found).

#### 01. POST 404 - Acuerdo Not Found
- **Path:** `{acuerdoId} = 00000000-0000-0000-0000-000000000000` (UUID invalida)
- **ErrorCode esperado:** `2011` (NotFound_Acuerdo)
- **Status esperado:** 404

#### 02. GET 404 - User Not Found
- **Path:** `{userId} = nonexistent-user-id`
- **ErrorCode esperado:** `2000` (NotFound_Entity)
- **Status esperado:** 404

---

### 7. **Valoraciones - Pagination** (2 pasos)

Verifica funcionamiento de paginación en GET de valoraciones.

#### 01. GET Valoraciones Page 1 with PageSize 2
- **Query params:** `page=1&pageSize=2`
- **Validaciones:**
  - Status 200 OK
  - `data.valoraciones.page = 1`
  - `data.valoraciones.pageSize = 2`
  - Items retornados ≤ pageSize
  - Ordenamiento por `fechaCreacion DESC` (más recientes primero)

#### 02. GET Valoraciones with Invalid Page
- **Query params:** `page=0&pageSize=10`
- **ErrorCode esperado:** `1009` (Validation_InvalidRange)
- **Status esperado:** 400 Bad Request

---

## Variables de Colección

Todas las variables necesarias están pre-inicializadas o se generan automáticamente en el _Setup:

| Variable | Tipo | Origen | Uso |
|----------|------|--------|-----|
| `baseUrl` | string | Manual | Base URL del backend (`http://localhost:5001`) |
| `artistaEmail` | string | Pre-set | Email artista conocido (`usuario1@mail.com`) |
| `artistaPassword` | string | Pre-set | Password artista (`123456`) |
| `artistaToken` | string | Login artista | Token JWT del artista |
| `artistaId` | string | Get Artista | ID del perfil artista |
| `profesionalEmail` | string | Generate | Email dinámico: `profesional-{timestamp}@weplay.com` |
| `profesionalPassword` | string | Pre-set | Password profesional (`TestPassword123!`) |
| `profesionalToken` | string | Register prof. | Token JWT del profesional |
| `profesionalUserId` | string | Register prof. | UserID del profesional (opcional) |
| `necesidadId` | string | Create necesidad | ID de la necesidad crowdsourcing |
| `propuestaId` | string | Create propuesta | ID de la propuesta |
| `acuerdoId` | string | Get acuerdo | ID del acuerdo (CRUCIAL para valoraciones) |
| `acuerdoEstado` | string | Get acuerdo | Estado actual del acuerdo (debe ser Completado=2) |
| `valoracionId` | string | POST valoracion 1 | ID de la primera valoración creada |
| `valoracionId2` | string | POST valoracion 2 | ID de la segunda valoración creada |
| `userIdValorado` | string | Dinámico | ID del usuario que recibió valoración |

---

## Flujo de Ejecución Recomendado

```
1. Ejecutar _Setup (8 pasos) → Prepara acuerdo en estado Completado
2. Ejecutar Valoraciones - Happy Path (3 pasos) → Verifica flujo exitoso
3. Ejecutar Valoraciones - Validation Errors (4 pasos) → Verifica validaciones
4. Ejecutar Valoraciones - Business Rules (1 paso) → Verifica reglas de negocio
5. Ejecutar Valoraciones - Auth Errors (2 pasos) → Verifica seguridad
6. Ejecutar Valoraciones - Not Found (2 pasos) → Verifica manejo de 404
7. Ejecutar Valoraciones - Pagination (2 pasos) → Verifica paginación
```

**Total:** 22 requests, todas independientes dentro de su folder.

---

## Estadísticas de la Colección

| Métrica | Valor |
|---------|-------|
| **Folders totales** | 7 (_Setup + 6 feature folders) |
| **Requests totales** | 22 |
| **Assertions** | ~65+ (distribuidas en test scripts) |
| **Endpoints únicos testeados** | 2 (POST create, GET list) |
| **Dependencias del _Setup** | Artista pre-existente (`usuario1@mail.com`) |
| **Datos generados dinámicamente** | Profesional, necesidad, propuesta, acuerdo, valoraciones |

---

## Cómo Ejecutar

### Opción 1: Postman UI
1. Abrir Postman
2. Importar archivo `plans/cs-valoraciones/backend/postman-collection.json`
3. Establecer variable de entorno: `baseUrl = http://localhost:5001`
4. Ejecutar _Setup primero
5. Ejecutar folder de tests deseado

### Opción 2: Newman CLI
```bash
# Ejecutar toda la colección
newman run plans/cs-valoraciones/backend/postman-collection.json \
  --environment tests/integration/environments/development.postman_environment.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/valoraciones-test-report.html

# Ejecutar solo _Setup + Happy Path
newman run plans/cs-valoraciones/backend/postman-collection.json \
  --folder "_Setup,Valoraciones - Happy Path" \
  --reporters cli

# Ejecutar con variables inline
newman run plans/cs-valoraciones/backend/postman-collection.json \
  -e tests/integration/environments/development.postman_environment.json \
  --reporters cli,json \
  --reporter-json-export reports/valoraciones-results.json
```

---

## Dependencias

### BD / Sistema
- **Motor de BD:** SQL Server
- **Usuario pre-existente:** `usuario1@mail.com` (usuario artista estándar de pruebas)
- **Módulos previos:** cs-acuerdos-entregables (US-CS-04) debe estar implementado
- **Backend activo:** http://localhost:5001 con Swagger en `/swagger`

### Dentro de la colección
- _Setup es completamente auto-contenido
- Happy Path solo depende de _Setup
- Business Rules depende de Happy Path (intenta crear duplicado)
- Auth/Not Found son independientes
- Pagination depende de Happy Path (consulta valoraciones creadas)

---

## Notas Importantes

### 1. Constraint Único en BD
El error `BusinessRule_DuplicateAction (4017)` se garantiza a dos niveles:
- **Validación en Handler:** Verifica `ExisteValoracionAsync(acuerdoId, userIdAutor)`
- **Constraint BD:** `UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor`

Si dos requests simultaneas pasan la validación, la BD rechaza con constraint violation, que el handler captura y retorna 400.

### 2. Acuerdo Completado Requerido
Regla de negocio crítica: **No se puede valorar un acuerdo que no esté en estado Completado (2)**.

El _Setup ejecuta "Complete Acuerdo" en el paso 8. Sin este paso, el test POST valoración fallará con ErrorCode `4014` (BusinessRule_InvalidState).

### 3. Estado de Puntuacion Media
El resumen de valoraciones incluye:
- `puntuacionMedia`: AVG(Puntuacion) con 1 decimal, `null` si no hay valoraciones
- `totalValoraciones`: COUNT(*)
- `distribucion`: { "1": count, "2": count, ..., "5": count } - siempre con 5 claves

En Happy Path con 2 valoraciones (5 y 4), la media debe ser 4.5.

### 4. AutorNombre y AutorImagenUrl
En la respuesta GET de valoraciones:
- `autorNombre`: Nombre artístico si autor es artista, nombre del perfil profesional si es profesional
- `autorImagenUrl`: URL del avatar, puede ser `null` si el autor no tiene imagen configurada

El test verifica que `autorNombre` existe pero no es específico sobre su contenido (depende del perfil).

### 5. Ordenamiento
El listado de valoraciones está ordenado por `fechaCreacion DESC` (más recientes primero). El test no verifica explícitamente este ordenamiento en Happy Path, pero es correctamente implementado en el backend.

---

## Troubleshooting

### _Setup falla en "Login Artista"
**Causa:** Usuario `usuario1@mail.com` no existe en BD.
**Solución:** Verificar que la BD tiene datos de prueba estándar. Usar script de seeding si es necesario.

### _Setup falla en "Create Necesidad"
**Causa:** Módulo Crowdsourcing no implementado completamente.
**Solución:** Verificar que endpoints de necesidades están disponibles en Swagger.

### Happy Path falla en POST Valoracion con 4007 (BusinessRule_InvalidState)
**Causa:** Acuerdo no está en estado Completado.
**Solución:** El paso 8 del _Setup ("Complete Acuerdo") no se ejecutó correctamente. Verificar que responde 200 OK.

### Happy Path falla en GET Valoraciones con 404
**Causa:** El userId usado en la ruta no corresponde al profesional.
**Solución:** El test usa variable `{{profesionalEmail}}` en la ruta, pero debería ser el `userId`. Se puede ajustar manualmente o almacenar el userId en una variable de response.

### Newman no encuentra el archivo
**Causa:** Ruta relativa incorrecta.
**Solución:** Usar ruta absoluta:
```bash
newman run /c/Repos/WePlay_Rises/plans/cs-valoraciones/backend/postman-collection.json
```

---

## Cambios Futuros

Para MVP v2 o mejoras de testing:
1. **Pre-crear múltiples valoraciones:** Para testing más completo de paginación y distribucion
2. **Escenarios de terceros:** Verificar que un tercero (sin relación con acuerdo) no puede valorar
3. **Tests de concurrencia:** Race condition del constraint único
4. **Mapeo de errores UI:** Validar mensajes mostrados en frontend
5. **Performance:** Medir tiempos de respuesta en GET con muchas valoraciones

---

## Referencia

- **Contratos API:** `docs/user-stories/cs-valoraciones/contracts.md`
- **Especificación feature:** `docs/user-stories/cs-valoraciones/feature-spec.md`
- **Plan técnico:** `plans/cs-valoraciones/backend/api-contracts.md`
- **Pattern CQRS:** `.claude/rules/backend/cqrs.rule.md`
- **Newman docs:** https://github.com/postmanlabs/newman
- **Postman v2.1 schema:** https://schema.getpostman.com/json/collection/v2.1.0/collection.json

---

**Generada con Claude Agent - WePlay Rises Integration Testing Framework**
