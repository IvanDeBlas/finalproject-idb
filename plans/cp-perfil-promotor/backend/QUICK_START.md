# Quick Start: Colección Postman cp-perfil-promotor

**Tiempo de lectura:** 2 minutos
**Requisitos:** Postman o Newman instalado

---

## 30 Segundos: Ejecutar en Postman

1. **Abre** `postman-collection.json` en Postman
2. **Click** en carpeta `_Setup`
3. **Send** el request `01. Login con Usuario de Prueba`
4. **Abre** carpeta `Promotor - CRUD Lifecycle`
5. **Click** el ▶️ (play) a la derecha de la carpeta
6. **Verifica:** Todos los requests pasan (checkmarks verdes)

**Resultado esperado:** 6 requests, 6 pasos, ~3 segundos

---

## 1 Minuto: Ejecutar con Newman

```bash
# Opción 1: Instalar newman (si no lo tienes)
npm install -g newman newman-reporter-htmlextra

# Opción 2: Ejecutar colección
newman run plans/cp-perfil-promotor/backend/postman-collection.json

# Opción 3: Con reporte HTML
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html
```

**Resultado esperado:** 20 requests ✓, 64 assertions ✓, 0 fallos, ~5-8 segundos

---

## Pre-requisitos (Verificar antes)

- [ ] Backend corriendo en `http://localhost:5001`
- [ ] Postman instalado O Newman instalado
- [ ] Archivo `postman-collection.json` accesible
- [ ] Usuario `usuario1@mail.com` existe en BD (en seed)

---

## Estructura de Ejecución

```
_Setup
└── 01. Login
    └── Obtiene JWT en accessToken

Promotor - CRUD Lifecycle
├── 01. POST Create Promotor
│   └── Crea promotor, guarda ID
├── 02. GET Me - Verify Creation
│   └── Valida creación
├── 03. PUT Update Promotor
│   └── Actualiza campos
├── 04. GET Me - Verify Update
│   └── Valida actualización
├── 05. PATCH Deactivate
│   └── Desactiva perfil
└── 06. GET Me - Verify Deactivation
    └── Valida desactivación

Promotor - Validation Errors
├── 7 tests de validación
└── Todos esperan 400 Bad Request

Promotor - Auth Errors
├── 3 tests sin autenticación
└── Todos esperan 401 Unauthorized

Promotor - Not Found & Business Rules
├── GET 404 sin perfil
├── POST 400 duplicado
└── PATCH 400 ya desactivado
```

---

## Variables Key

| Variable | Auto-Generada | Valor |
|----------|---------------|-------|
| `baseUrl` | No | `http://localhost:5001` |
| `testEmail` | No | `usuario1@mail.com` |
| `testPassword` | No | `123456` |
| `accessToken` | **Sí** ← _Setup | JWT (24h válido) |
| `promotorId` | **Sí** ← CRUD 01 | UUID del promotor |
| `promotorName` | **Sí** ← CRUD 01 | "DJ Marketing Pro {timestamp}" |

---

## Modo Debugging

### Si un request falla:

**Paso 1:** Click en request → Tab **Response**

**Paso 2:** Mira el JSON:
```json
{
  "data": { ... },
  "messages": [
    { "message": "...", "errorCode": "4018" }
  ]
}
```

**Paso 3:** Busca el error:
- `4018` = Promotor ya existe para este usuario
- `1001` = Campo requerido faltante
- `401` = Token inválido
- `404` = Perfil no encontrado

**Paso 4:** Relée `POSTMAN_COLLECTION_GUIDE.md` sección Troubleshooting

---

## Ejecución por Carpeta

### Solo Setup
```bash
newman run postman-collection.json --folder "_Setup"
```

### Solo CRUD
```bash
newman run postman-collection.json --folder "Promotor - CRUD Lifecycle"
```

### Solo Validation Errors
```bash
newman run postman-collection.json --folder "Promotor - Validation Errors"
```

### Todo
```bash
newman run postman-collection.json
```

---

## Casos Especiales

### ¿Error "Promotor Already Exists" (4018)?

**Causa:** usuario1@mail.com ya tiene un promotor creado

**Soluciones:**
1. Espera 24h (limpieza automática) - No recomendado ❌
2. Usa otro usuario de prueba - Recomendado ✓
3. Limpia BD y vuelve a ejecutar - Para dev local ✓

### ¿Error "Token no válido o expirado"?

**Causa:** JWT expiró (> 24h) o no fue obtenido

**Solución:** Ejecuta _Setup → Login de nuevo

### ¿Requests retornan 404 en GET /me?

**Causa:** El POST no creó el promotor correctamente

**Verificar:**
1. POST retornó 201? ✓
2. `promotorId` se guardó? (ver variables)
3. ¿Mismo token en todos los requests?

---

## Output Esperado (Newman)

```
┌─────────────────────────────────────────┐
│ Newman Integration Test Run              │
│ WePlay.PerfilPromotor.IntegrationTests   │
└─────────────────────────────────────────┘

 ●  _Setup
     ✓  01. Login con Usuario de Prueba

 ●  Promotor - CRUD Lifecycle
     ✓  01. POST Create Promotor
     ✓  02. GET Me - Verify Creation
     ✓  03. PUT Update Promotor
     ✓  04. GET Me - Verify Update
     ✓  05. PATCH Deactivar Promotor
     ✓  06. GET Me - Verify Deactivation

 ●  Promotor - Validation Errors
     ✓  POST 400 - nombrePublico Empty
     ✓  POST 400 - nombrePublico Min Length
     ✓  POST 400 - nombrePublico Max Length
     ✓  POST 400 - tipoPromotorId Missing
     ✓  POST 400 - tipoPromotorId Invalid
     ✓  POST 400 - emailContacto Invalid Format
     ✓  POST 400 - urlSitioWeb Invalid Format

 ●  Promotor - Auth Errors
     ✓  POST 401 - Missing Auth Token
     ✓  GET 401 - Missing Auth Token
     ✓  PUT 401 - Invalid Token

 ●  Promotor - Not Found & Business Rules
     ✓  GET 404 - Promotor Not Found
     ✓  POST 400 - Promotor Already Exists
     ✓  PATCH 400 - Already Inactive

┌─────────────────────────────────────────┐
│ SUMMARY                                  │
├─────────────────────────────────────────┤
│ Tests:    20 passed, 20 total            │
│ Assertions: 64 passed, 64 total          │
│ Duration: 5.234s                         │
└─────────────────────────────────────────┘

✓ All tests passed
```

---

## Endpoints Testeados

```
POST   /api/crowdpromotion/promotor
GET    /api/crowdpromotion/promotor/me
PUT    /api/crowdpromotion/promotor/me
PATCH  /api/crowdpromotion/promotor/me/desactivar
```

---

## Estadísticas Rápidas

| Métrica | Valor |
|---------|-------|
| Requests | 20 |
| Assertions | 64 |
| Folders | 5 |
| Tiempo | ~5-8 seg |
| Endpoints | 4 |
| Error Codes | 9 |

---

## Documentos de Referencia

| Documento | Para Qué |
|-----------|----------|
| **POSTMAN_COLLECTION_GUIDE.md** | Detalles completos, troubleshooting |
| **COLLECTION_EXECUTION_SUMMARY.md** | Estadísticas, cobertura, patrones |
| **DELIVERABLE_SUMMARY.md** | Checklist, validaciones |
| **QUICK_START.md** | Este archivo (30 segundos) |

---

## Comando Copy-Paste

```bash
# Instalar (si no tienes newman)
npm install -g newman newman-reporter-htmlextra

# Ejecutar
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export promotor-test-report.html

# Ver reporte
open promotor-test-report.html  # macOS
start promotor-test-report.html # Windows
xdg-open promotor-test-report.html # Linux
```

---

## ¿Qué Validar?

### Happy Path (CRUD) ✓
- Crear promotor
- Obtener datos completos
- Actualizar campos
- Desactivar

### Errores ✓
- Validación: campos requeridos, formatos
- Autenticación: sin token, token inválido
- Negocio: duplicado, ya desactivado

### Performance ✓
- Response time < 500ms
- Colección completa < 10 segundos

---

## Próximas Acciones

1. **Importa** colección en Postman
2. **Ejecuta** _Setup
3. **Ejecuta** CRUD Lifecycle
4. **Verifica** que pasan todos
5. **Lee** POSTMAN_COLLECTION_GUIDE.md para detalles

---

**Listo en 30 segundos. Adiós.** 👋
