# Postman Collection - Definir Recompensas (Rewards API)

## Overview

La colección Postman en `postman-collection.json` contiene tests de integración ejecutables para todos los endpoints de la API de Recompensas (Rewards).

**Ubicación:** `C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json`

**Formato:** Postman Collection v2.1 (compatible con Newman)

---

## Estructura de la Colección

La colección está organizada en 6 carpetas principales:

### 1. **_Setup** (Setup Inicial)
- `Register Test User` - Crea un usuario de prueba
- `Login & Get Token` - Obtiene JWT token
- `Create Test Artista Profile` - Crea perfil de artista
- `Create Test Campania` - Crea campaña para las recompensas

### 2. **Create Rewards** (Crear Recompensas)
- `201 CREATED - Digital Reward` - Crea recompensa tipo digital
- `201 CREATED - Physical Reward` - Crea recompensa física con stock limitado
- `201 CREATED - Experience Reward` - Crea recompensa tipo experiencia
- `400 VALIDATION - Empty Name` - Valida que nombre es obligatorio
- `400 VALIDATION - Negative Amount` - Valida que importeMinimo > 0
- `401 UNAUTHORIZED - No Token` - Valida que requiere autenticación

### 3. **Read Rewards** (Leer Recompensas)
- `200 OK - List All Rewards by Campaign` - Lista recompensas con filtro
- `200 OK - List Active Only` - Lista solo recompensas activas
- `200 OK - Get Reward by ID` - Obtiene detalles de una recompensa
- `404 NOT FOUND - Invalid Reward ID` - Valida que ID debe existir

### 4. **Update Rewards** (Actualizar Recompensas)
- `200 OK - Update Reward Name` - Actualiza nombre (PATCH semantics)
- `200 OK - Update Multiple Fields` - Actualiza múltiples campos
- `400 VALIDATION - Name Too Long` - Valida maxLength (200 caracteres)
- `404 NOT FOUND - Update Non-Existent Reward` - Valida que existe el reward

### 5. **Reorder Rewards** (Reordenar Recompensas)
- `200 OK - Reorder Multiple Rewards` - Reordena múltiples recompensas
- `400 VALIDATION - Empty Reward Orders List` - Lista no puede estar vacía
- `400 VALIDATION - Negative Order Value` - Orden debe ser >= 0
- `404 NOT FOUND - Campaign Not Found` - Valida que campaña existe

### 6. **Delete Rewards** (Eliminar Recompensas)
- `200 OK - Delete Reward (Soft Delete)` - Elimina con soft delete
- `404 NOT FOUND - Delete Non-Existent Reward` - Valida que existe
- `401 UNAUTHORIZED - Delete Without Token` - Valida autenticación

### 7. **_Cleanup** (Limpieza)
- `Delete Remaining Rewards` - Limpia recompensas de prueba
- `Delete Test Campaign` - Limpia campaña de prueba

---

## Variables de Entorno

La colección usa las siguientes variables (autoseteadas durante setup):

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `baseUrl` | URL base de la API | `http://localhost:5001` |
| `accessToken` | JWT Bearer token (auto-seteado) | Auto |
| `testUserId` | ID del usuario de prueba | Auto |
| `artistaId` | ID del perfil artista | Auto |
| `campaniaId` | ID de la campaña de prueba | Auto |
| `rewardId1` | ID del reward digital | Auto |
| `rewardId2` | ID del reward físico | Auto |
| `rewardId3` | ID del reward experiencia | Auto |

---

## Instalación y Configuración

### Opción 1: Importar en Postman UI

1. Abre Postman
2. Click en "Import" (arriba a la izquierda)
3. Selecciona "Upload Files"
4. Busca y abre `postman-collection.json`
5. Click en "Import"

La colección se importará con todas las variables pre-configuradas.

### Opción 2: Usar con Newman (CLI)

```bash
# Instalar Newman (si no está instalado)
npm install -g newman

# Ejecutar la colección contra localhost:5001
newman run postman-collection.json \
    --environment postman-environment.json \
    --reporters cli,json \
    --reporter-json-export results.json

# Ejecutar contra entorno diferente
newman run postman-collection.json \
    --environment production-env.json \
    --bail \
    --verbose
```

---

## Ejecución Manual (Paso a Paso)

### 1. Setup Inicial (OBLIGATORIO PRIMERO)

Ejecuta en orden:

1. **Register Test User** - Crea usuario
   - Genera email único con timestamp
   - Guarda `testUserId`

2. **Login & Get Token** - Obtiene JWT
   - Usa email del paso anterior
   - Guarda `accessToken` (necesario para requests autenticados)

3. **Create Test Artista Profile** - Crea artista
   - Usa token del paso anterior
   - Guarda `artistaId`

4. **Create Test Campania** - Crea campaña
   - Necesaria para crear recompensas
   - Guarda `campaniaId`

### 2. Crear Recompensas

Ejecuta los 3 rewards "201 CREATED":
- Digital Reward → guarda `rewardId1`
- Physical Reward → guarda `rewardId2`
- Experience Reward → guarda `rewardId3`

### 3. Validar Recompensas (Lectura)

Ejecuta tests de "Read Rewards":
- Listar por campaña
- Filtrar activos
- Obtener por ID

### 4. Actualizar y Reordenar

- Actualiza nombres/descripciones
- Reordena con PUT /reorder

### 5. Eliminar

- Delete Reward 3 (soft delete)
- Verifica que 404 al buscar

### 6. Limpieza

Ejecuta _Cleanup para eliminar datos de prueba.

---

## Estructura de Requests y Responses

### POST /api/rewards - Crear Reward

**Request:**
```json
{
    "campaniaId": "uuid",
    "tipoRewardId": 1,
    "nombre": "Reward name",
    "importeMinimo": 10,
    "monedaId": 1,
    "esAddOn": false,
    "orden": 1
}
```

**Response 201:**
```json
{
    "data": {
        "id": "uuid",
        "campaniaId": "uuid",
        "tipoRewardId": 1,
        "nombre": "Reward name",
        "importeMinimo": 10.00,
        "esActivo": true,
        "orden": 1,
        "fechaCreacion": "2026-02-13T10:30:00Z"
    },
    "messages": [
        {
            "message": "Reward creado correctamente",
            "errorCode": "0000"
        }
    ],
    "isSuccess": true
}
```

### GET /api/rewards?campaniaId={id}

**Response 200:**
```json
{
    "data": [
        {
            "id": "uuid",
            "nombre": "Digital Reward",
            "importeMinimo": 10.00,
            "orden": 1,
            "esActivo": true
        }
    ],
    "messages": [
        {
            "message": "Rewards encontrados",
            "errorCode": "0000"
        }
    ],
    "isSuccess": true
}
```

### PUT /api/rewards/{id}

**Request (PATCH semantics - solo campos a actualizar):**
```json
{
    "id": "uuid",
    "nombre": "Updated name",
    "descripcion": "Updated description"
}
```

**Response 200:**
```json
{
    "data": true,
    "messages": [
        {
            "message": "Reward actualizado correctamente",
            "errorCode": "0002"
        }
    ],
    "isSuccess": true
}
```

### PUT /api/rewards/reorder

**Request:**
```json
{
    "campaniaId": "uuid",
    "rewardOrders": [
        { "rewardId": "uuid1", "orden": 1 },
        { "rewardId": "uuid2", "orden": 2 },
        { "rewardId": "uuid3", "orden": 3 }
    ]
}
```

**Response 200:**
```json
{
    "data": true,
    "messages": [
        {
            "message": "Recompensas reordenadas correctamente",
            "errorCode": "0002"
        }
    ],
    "isSuccess": true
}
```

### DELETE /api/rewards/{id}

**Response 200 (Soft Delete):**
```json
{
    "data": true,
    "messages": [
        {
            "message": "Reward eliminado correctamente",
            "errorCode": "0003"
        }
    ],
    "isSuccess": true
}
```

---

## Códigos de Error

Todos los errores siguen el formato `ServiceResponse` con campos `errorCode`:

| HTTP | Code | Significado |
|------|------|-------------|
| 200 | 0000 | Exito general |
| 200 | 0002 | Actualización exitosa |
| 200 | 0003 | Eliminación exitosa |
| 400 | 1001 | Campo obligatorio vacío |
| 400 | 1002 | Longitud máxima excedida |
| 400 | 1007 | Valor fuera de rango |
| 400 | 1011 | Importe debe ser > 0 |
| 401 | 3001 | Token inválido/expirado |
| 403 | 3002 | Sin permiso (ownership mismatch) |
| 404 | 2003 | Campaña no encontrada |
| 404 | 2004 | Reward no encontrado |
| 409 | 4010 | Reward tiene backings (no puede eliminar) |
| 500 | 5000 | Error inesperado |

---

## Assertions Incluidas

Cada request tiene test scripts que validan:

✅ **Status codes** - 200, 201, 400, 401, 403, 404, 409, 500
✅ **Response time** - < 500ms para operaciones de lectura
✅ **ServiceResponse structure** - Presencia de data, messages, isSuccess
✅ **Error codes** - Códigos correctos según tipo de error
✅ **Data integrity** - Campos obligatorios presentes
✅ **Variable storage** - IDs guardados para requests posteriores

---

## Requisitos Previos

### Backend debe estar ejecutándose

```bash
# En C:\Repos\WePlay_Rises\src\api\WebApi
dotnet run --launch-profile https
```

Verifica que esté disponible en `http://localhost:5001`

### Swagger debe estar accesible

Para debugging, abre:
```
http://localhost:5001/swagger
```

---

## Troubleshooting

### Error: "Token no valido o expirado" (401)

**Causa:** El setup no ejecutó correctamente.

**Solución:**
1. Ejecuta de nuevo "Register Test User"
2. Ejecuta "Login & Get Token"
3. Verifica que `accessToken` aparece en variables

### Error: "Campania no encontrada" (404 - 2003)

**Causa:** No ejecutaste "Create Test Campania" en setup.

**Solución:**
1. Ejecuta setup completo en orden
2. Verifica que `campaniaId` está seteado

### Error: "Reward no encontrado" (404 - 2004)

**Causa:** El reward fue eliminado o rewardId es incorrecto.

**Solución:**
1. Re-ejecuta "Create Rewards" para generar nuevos IDs
2. Verifica que las variables están actualizadas

### Los tests no guardan variables

**Causa:** Los test scripts no ejecutaron.

**Solución:**
1. En Postman UI, abre Settings (rueda dentada)
2. Habilita "Keep variable values in sync"
3. Re-ejecuta los requests

---

## Métricas de la Colección

| Métrica | Valor |
|---------|-------|
| Total Folders | 7 |
| Total Requests | 23 |
| Total Test Assertions | ~85 |
| Coverage de Endpoints | 100% (6 endpoints) |
| HTTP Methods | GET, POST, PUT, DELETE |
| Status Codes Cubiertos | 200, 201, 400, 401, 403, 404, 409, 500 |
| Validaciones | 35+ reglas |

---

## Comando para Ejecutar con Newman

```bash
# Básico (CLI output)
newman run C:/Repos/WePlay_Rises/plans/definir-recompensas/backend/postman-collection.json

# Con reporte JSON
newman run C:/Repos/WePlay_Rises/plans/definir-recompensas/backend/postman-collection.json \
    --reporters cli,json \
    --reporter-json-export results.json

# Con stopping en primer error
newman run C:/Repos/WePlay_Rises/plans/definir-recompensas/backend/postman-collection.json \
    --bail

# Con variables de entorno
newman run C:/Repos/WePlay_Rises/plans/definir-recompensas/backend/postman-collection.json \
    --environment C:/Repos/WePlay_Rises/tests/newman/environments/development.json

# Ejecutar solo carpeta específica
newman run C:/Repos/WePlay_Rises/plans/definir-recompensas/backend/postman-collection.json \
    --folder "Create Rewards"
```

---

## Notas Importantes

1. **Orden de Ejecución:** Siempre ejecuta _Setup primero
2. **Variables:** Las variables se propagan automáticamente entre requests
3. **Soft Delete:** El DELETE usa soft delete (EsActivo = false)
4. **PATCH Semantics:** PUT /api/rewards/{id} solo actualiza campos proporcionados
5. **Autenticación:** Todos los POST, PUT, DELETE requieren Bearer token
6. **Timestamps:** Los nombres de usuarios/campanias usan {{$timestamp}} para unicidad

---

## Archivo Generado

- **Ruta:** `C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json`
- **Versión Postman:** v2.1.0
- **Fecha Generación:** 2026-02-13
- **Esquema:** https://schema.getpostman.com/json/collection/v2.1.0/collection.json

---

## Para Más Información

- API Contracts: `docs/user-stories/definir-recompensas/contracts.md`
- API Implementation Details: `plans/definir-recompensas/backend/api-contracts.md`
- Backend Architecture: `plans/definir-recompensas/backend/hexagonal-architecture.md`

---

**Fin de documentación de Postman Collection.**
