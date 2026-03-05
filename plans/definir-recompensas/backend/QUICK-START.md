# Quick Start - Rewards API Testing con Postman

## En 5 minutos

### Paso 1: Descargar la Colección

Archivos necesarios:
- `postman-collection.json` - Colección con 23 requests
- `postman-environment.json` - Variables pre-configuradas

### Paso 2: Iniciar Backend

```bash
cd C:\Repos\WePlay_Rises\src\api\WebApi
dotnet run --launch-profile https
```

Verificar en: http://localhost:5001/swagger

### Paso 3: Abrir Postman e Importar

1. Abre Postman
2. Click "Import" → "Upload Files"
3. Selecciona `postman-collection.json`
4. Importa también `postman-environment.json` (File → Import)
5. Selecciona "WePlay - Definir Recompensas - Development" en dropdown de entornos (arriba a la derecha)

### Paso 4: Ejecutar Tests

#### Opción A: Interfaz Gráfica (UI)

1. En el panel izquierdo, abre "_Setup"
2. Click en "Register Test User" → "Send"
3. Click en "Login & Get Token" → "Send"
4. Click en "Create Test Artista Profile" → "Send"
5. Click en "Create Test Campania" → "Send"
6. Abre "Create Rewards" y ejecuta los 3 requests "201 CREATED"
7. Explora "Read Rewards", "Update Rewards", etc.

#### Opción B: Runner (Ejecución Automática)

1. Click en "Run" (arriba a la derecha)
2. Selecciona "WePlay.DefinirRecompensas.IntegrationTests"
3. Click "Run" (ejecuta toda la colección)

#### Opción C: Newman (Terminal)

```bash
newman run "C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json"
```

---

## Estructura Rápida

```
_Setup                      ← Ejecutar PRIMERO (en orden)
├── Register Test User
├── Login & Get Token
├── Create Test Artista Profile
└── Create Test Campania

Create Rewards              ← Crear 3 tipos de recompensas
├── 201 CREATED - Digital
├── 201 CREATED - Physical
├── 201 CREATED - Experience
├── 400 VALIDATION - Empty Name
├── 400 VALIDATION - Negative Amount
└── 401 UNAUTHORIZED - No Token

Read Rewards                ← Leer/Listar recompensas
├── 200 OK - List All Rewards
├── 200 OK - List Active Only
├── 200 OK - Get by ID
└── 404 NOT FOUND - Invalid ID

Update Rewards              ← Actualizar recompensas
├── 200 OK - Update Name
├── 200 OK - Multiple Fields
├── 400 VALIDATION - Name Too Long
└── 404 NOT FOUND - Non-existent

Reorder Rewards             ← Drag & drop (reordenar)
├── 200 OK - Reorder Multiple
├── 400 VALIDATION - Empty List
├── 400 VALIDATION - Negative Order
└── 404 NOT FOUND - Campaign Not Found

Delete Rewards              ← Soft delete (desactivar)
├── 200 OK - Delete Reward
├── 404 NOT FOUND - Non-existent
└── 401 UNAUTHORIZED - No Token

_Cleanup                    ← Limpieza (ejecutar al final)
├── Delete Remaining Rewards
└── Delete Test Campaign
```

---

## Variables Automáticas

Después de ejecutar Setup, estas variables se auto-rellenan:

| Variable | Creada por | Usada en |
|----------|-----------|---------|
| `accessToken` | Login & Get Token | Todos POST/PUT/DELETE |
| `artistaId` | Create Artista | Create Campaign |
| `campaniaId` | Create Campaign | Todos Rewards |
| `rewardId1` | Create Digital | Update/Delete |
| `rewardId2` | Create Physical | Reorder/Update |
| `rewardId3` | Create Experience | Delete |

---

## Test Cases por Carpeta

### Create Rewards (6 requests)
- ✅ POST /api/rewards con todos los campos
- ✅ Diferentes tipoRewardId (1=Digital, 2=Físico, 3=Experiencia)
- ✅ Validación de campo obligatorio
- ✅ Validación de rango (importeMinimo > 0)
- ✅ Autenticación requerida

### Read Rewards (4 requests)
- ✅ GET /api/rewards (listar por campaña)
- ✅ Filtro esActivo=true
- ✅ GET /api/rewards/{id} (detalles)
- ✅ 404 con ID inválido

### Update Rewards (4 requests)
- ✅ PUT con un campo (PATCH semantics)
- ✅ PUT con múltiples campos
- ✅ Validación maxLength
- ✅ 404 con reward inexistente

### Reorder Rewards (4 requests)
- ✅ PUT /api/rewards/reorder con múltiples rewards
- ✅ Validación lista vacía
- ✅ Validación orden >= 0
- ✅ 404 campaña inexistente

### Delete Rewards (3 requests)
- ✅ DELETE con soft delete
- ✅ 404 reward inexistente
- ✅ 401 sin token

---

## Verificar que Funciona

Después de ejecutar un request, verifica en el panel derecho:

```
✓ Status: 200 OK o 201 CREATED
✓ Body: contiene "data", "messages", "isSuccess"
✓ Tests: verás checkmarks verdes (✓)
✓ Variables: se actualizan en la pestaña "Variables" (arriba)
```

---

## Errores Comunes

### "Campania no encontrada" (404 - 2003)
→ No ejecutaste Create Test Campania en Setup

### "Token no valido" (401 - 3001)
→ Login no completó. Re-ejecuta "Login & Get Token"

### "Reward no encontrado" (404 - 2004)
→ El reward fue eliminado. Vuelve a crear con "Create Rewards"

### Variables vacías
→ En Postman: File → Import → postman-environment.json

---

## Profundizar

Para más detalles, lee:
- **POSTMAN-SETUP.md** - Documentación completa
- **api-contracts.md** - Especificación técnica de endpoints
- **hexagonal-architecture.md** - Arquitectura backend

---

## Comando Rápido (Terminal)

```bash
# Ejecutar TODO (setup + tests + cleanup)
newman run "C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json" \
    --environment "C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-environment.json" \
    --reporters cli,json \
    --reporter-json-export results.json

# Solo setup
newman run "C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json" \
    --folder "_Setup"

# Solo Create Rewards
newman run "C:\Repos\WePlay_Rises\plans\definir-recompensas\backend\postman-collection.json" \
    --folder "Create Rewards"
```

---

**¡Listo! Ahora puedes testear la API de Recompensas.**
