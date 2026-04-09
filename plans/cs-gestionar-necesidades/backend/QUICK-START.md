# Quick Start - Colección Postman cs-gestionar-necesidades

**¿Quieres ejecutar los tests de inmediato?** Lee esta página. (5 minutos)

---

## Paso 1: Verificar Backend Ejecutándose

```bash
# En terminal, verifica que el backend está corriendo
curl http://localhost:5001/swagger

# Si ves Swagger UI → ✓ Backend OK
# Si no → ejecuta:
cd src/api
dotnet run --project WebApi
```

---

## Paso 2: Abrir Postman

1. Abre **Postman** (descargar desde https://www.postman.com/downloads/ si no lo tienes)
2. Click en **File** → **Import**
3. Selecciona el archivo:
   ```
   plans/cs-gestionar-necesidades/backend/postman-collection.json
   ```
4. Click **Import** (la colección aparecerá en el sidebar izquierdo)

---

## Paso 3: Ejecutar _Setup

Es obligatorio hacer el setup primero para obtener el token JWT y las maestras.

1. En el sidebar, expande carpeta **`_Setup`**
2. Verás 6 requests:
   ```
   ├── 01. Login User
   ├── 02. Get Artista ID
   ├── 03. Get Proyecto Artístico
   ├── 04. Get Maestras - TiposNecesidad
   ├── 05. Get Maestras - ModalidadesTrabajo
   └── 06. Get Maestras - Monedas
   ```
3. Click derecho en carpeta **`_Setup`** → **Run folder**
4. Se abre modal de ejecución, click **Run _Setup**
5. Espera a que los 6 requests completen (deberían pasar todos)
6. **IMPORTANTE:** No cierres el modal aún. Necesitas que las variables se guarden en memoria.

### ✓ _Setup Exitoso

Deberías ver:

```
✓ 01. Login User
    ✓ Login successful - Status 200
    ✓ Response has token

✓ 02. Get Artista ID
    ✓ Get artista ID - Status 200
    ✓ Extract artistaId from response

✓ 03. Get Proyecto Artístico
    ✓ Get proyecto artístico - Status 200
    ✓ Extract proyectoArtisticoId from response

✓ 04. Get Maestras - TiposNecesidad
    ✓ Get TiposNecesidad - Status 200
    ✓ Extract tipoNecesidadId

✓ 05. Get Maestras - ModalidadesTrabajo
    ✓ Get ModalidadesTrabajo - Status 200
    ✓ Extract modalidadTrabajoId (Remoto)

✓ 06. Get Maestras - Monedas
    ✓ Get Monedas - Status 200
    ✓ Extract monedaId (EUR)
```

---

## Paso 4: Ejecutar CRUD Lifecycle

Ahora veremos el flujo completo: crear → listar → actualizar → cerrar.

1. En el sidebar, expande **`Necesidades - CRUD Lifecycle`**
2. Verás 9 requests en orden
3. Click derecho en carpeta → **Run folder**
4. Click **Run Necesidades - CRUD Lifecycle**
5. Observa los 9 pasos ejecutándose secuencialmente

### ✓ CRUD Exitoso

```
✓ 01. POST Create Necesidad (201)
    ✓ Status is 201 Created
    ✓ Response time < 500ms
    ✓ ServiceResponse structure is correct
    ✓ Response data contains required fields

✓ 02. GET All Necesidades (200)
    ✓ Status is 200 OK
    ✓ Response has paginated data structure
    ✓ Created necesidad appears in list

✓ 03. GET All Necesidades - Filter by Estado Abierta
    ✓ Status is 200 OK
    ✓ All items have estado = Abierta (1)

✓ 04. GET All Necesidades - Search by Text
    ✓ Status is 200 OK
    ✓ Search results contain mezcla keyword

✓ 05. GET Necesidad By ID (200)
    ✓ Status is 200 OK
    ✓ Response contains complete necesidad detail

✓ 06. PUT Update Necesidad (200)
    ✓ Status is 200 OK
    ✓ Update response contains updated data

✓ 07. GET Necesidad By ID - Verify Update
    ✓ Status is 200 OK
    ✓ Updated data reflects changes

✓ 08. PATCH Close Necesidad (200)
    ✓ Status is 200 OK
    ✓ Close response contains correct data

✓ 09. GET Necesidad By ID - Verify Closed
    ✓ Status is 200 OK
    ✓ Necesidad is now closed (estado = 3)
```

---

## Paso 5 (Opcional): Ejecutar Validaciones

Para verificar que la validación de errores funciona:

1. Click en **`Necesidades - Validation Errors`**
2. Click derecho → **Run folder**
3. Deberías ver 4 tests que retornan status 400 (esperado, validación funciona)

```
✓ POST 400 - Titulo Vacio
    ✓ Status is 400 Bad Request
    ✓ Response has validation errors

✓ POST 400 - Titulo Menor a 5 Caracteres
    ✓ Status is 400 Bad Request
    ✓ Error code is 1011 (MinLength)

✓ POST 400 - Presupuesto Max Menor a Min
    ✓ Status is 400 Bad Request
    ✓ Error code is 1009 (InvalidRange)

✓ POST 400 - Moneda Requerida si Hay Presupuesto
    ✓ Status is 400 Bad Request
    ✓ Error code is 1001 (Required)
```

---

## Paso 6 (Opcional): Ejecutar Todo con Newman (CLI)

Si quieres ejecutar todo de una vez desde la terminal:

```bash
# Instalar Newman (solo primera vez)
npm install -g newman
npm install -g newman-reporter-htmlextra

# Ejecutar colección completa
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/test-report.html

# Esperar ~15 segundos...
# Salida esperada:
# ✓ 25 tests ejecutados
# ✓ ~85 assertions pasadas
# → Reporte HTML en: reports/test-report.html
```

---

## Qué Hace Cada Carpeta

| Carpeta | Propósito | Esperar |
|---------|-----------|---------|
| `_Setup` | **OBLIGATORIO** - Obtener token y maestras | ✓ Todos los tests pasan (6/6) |
| `Necesidades - CRUD Lifecycle` | **OBLIGATORIO** - Flujo principal (crear, listar, actualizar, cerrar) | ✓ Todos los tests pasan (9/9) |
| `Necesidades - Validation Errors` | Opcional - Verificar validaciones devuelven 400 | ✓ 4/4 tests con status 400 |
| `Necesidades - Business Rules` | Opcional - Verificar reglas de negocio | ✓ 2/2 tests con status 400 |
| `Necesidades - Auth Errors` | Opcional - Verificar sin token/token inválido | ✓ 2/2 tests con status 401 |
| `Necesidades - Not Found` | Opcional - Verificar recurso inexistente | ✓ 2/2 tests con status 404 |

---

## ¿Qué Significa Cada Status?

### Status 200 OK ✓
- Request exitosa, operación completada
- Ejemplo: GET, PUT con datos válidos

### Status 201 Created ✓
- Nuevo recurso creado
- Ejemplo: POST /necesidades

### Status 400 Bad Request ✓ (en tests de validación)
- Datos inválidos
- Esperado en tests de validación, presupuesto invertido, etc.
- Verifica que retorna `errorCode` correcto (1001, 1009, 1011, 4001, etc.)

### Status 401 Unauthorized ✓ (en tests de auth)
- Token ausente o inválido
- Esperado sin token o token inválido

### Status 404 Not Found ✓ (en tests de not found)
- Recurso no existe
- Esperado al buscar por ID que no existe

---

## Troubleshooting (Problemas Comunes)

### ❌ "Necesidad no encontrada (404)" en paso 5+

**Causa:** El paso 1 (POST Create) no guardó el `necesidadId`

**Solución:**
1. Vuelve a ejecutar `_Setup`
2. Ejecuta `Necesidades - CRUD Lifecycle` nuevamente
3. Si persiste, verifica que el usuario `usuario1@mail.com` existe en la BD

### ❌ "Token no válido (401)" en CRUD

**Causa:** El token JWT expiró

**Solución:**
- Re-ejecuta `_Setup` para obtener un nuevo token
- Los tokens tienen 24 horas de validez, pero Postman puede refrescarse

### ❌ "El proyecto artístico no existe" en POST Create

**Causa:** El usuario no tiene proyectos asociados

**Solución:**
1. Crea un ProyectoArtistico para `usuario1@mail.com` vía Swagger/API
2. O cambia el email en `_Setup` → `01. Login User` a otro usuario que sí tenga proyectos

### ❌ "El tipo de necesidad no existe"

**Causa:** Las maestras (tipos, modalidades, monedas) no están pobladas en la BD

**Solución:**
- Asegúrate de que existen en la BD:
  - `TiposNecesidad` (al menos 1 registro)
  - `ModalidadesTrabajo` (Presencial=1, Remoto=2, Híbrido=3)
  - `Monedas` (EUR=1, USD=2, GBP=3)
- Verifica vía Swagger en: `/api/crowdsourcing/maestras/*`

### ❌ "BadRequest - Validation_InvalidDate"

**Causa:** Las fechas en el futuro relativo actual no son suficientemente futuro

**Solución:**
- La colección usa fechas hardcodeadas (2026-03-15, etc.)
- Si estamos muy cerca de 2026, actualiza las fechas en:
  - `01. POST Create Necesidad` → Body
  - `06. PUT Update Necesidad` → Body

---

## Comandos Útiles

### Ver si el backend está corriendo
```bash
curl -s http://localhost:5001/swagger | grep -q "swagger" && echo "✓ Backend OK" || echo "✗ Backend NO OK"
```

### Ejecutar solo un request
```bash
newman run postman-collection.json \
  --folder "_Setup" \
  --item "01. Login User" \
  --reporters cli
```

### Ejecutar con salida JSON (para CI/CD)
```bash
newman run postman-collection.json \
  --reporters json \
  --reporter-json-export report.json
```

### Ver variables de colección después de setup
Abre Postman → View → Show Postman Console → busca "collectionVariables"

---

## Resumen en 60 segundos

1. ✓ Backend ejecutándose en `http://localhost:5001`
2. ✓ Import `postman-collection.json` a Postman
3. ✓ Ejecuta carpeta `_Setup` (6 requests, ~5 segundos)
4. ✓ Ejecuta carpeta `Necesidades - CRUD Lifecycle` (9 requests, ~10 segundos)
5. ✓ Todos los tests pasan (✓ verde) = colección funciona correctamente

**Total: ~15 segundos para verificar que la feature está operativa.**

---

## Próximos Pasos

- **Después de verificar:** Revisa `COLLECTION-SUMMARY.md` para detalles de cada test
- **Para CI/CD:** Usa `newman run` con reporters JSON
- **Para debugging:** Abre Postman Console (View → Show Postman Console)
- **Para modificar:** Edita variables en Postman UI o en la colección JSON

---

**¿Preguntas?** Revisa `POSTMAN-README.md` para documentación completa.
