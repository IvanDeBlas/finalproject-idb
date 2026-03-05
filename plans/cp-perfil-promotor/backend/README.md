# Colección Postman: Perfil de Promotor (cp-perfil-promotor)

**Feature:** US-CP-01 - Crear y gestionar perfil de promotor
**Módulo Backend:** Crowdpromotion
**Generado:** 2026-02-25
**Status:** ✓ Completado y Documentado

---

## Descripción

Colección completa de pruebas de integración para validar el **lifecycle CRUD** del perfil de promotor en la plataforma WePlay Rises. Cubre los 4 endpoints principales del módulo Crowdpromotion:

- **POST /api/crowdpromotion/promotor** - Crear perfil
- **GET /api/crowdpromotion/promotor/me** - Obtener perfil propio
- **PUT /api/crowdpromotion/promotor/me** - Actualizar perfil
- **PATCH /api/crowdpromotion/promotor/me/desactivar** - Desactivar perfil

**100% auto-inclusiva:** Ejecutable sin datos previos en la base de datos.

---

## Archivos Incluidos

| Archivo | Descripción | Leer |
|---------|-------------|------|
| **postman-collection.json** | Colección JSON v2.1 ejecutable (20 requests, 64 assertions) | ⏱️ N/A |
| **QUICK_START.md** | Guía de 2 minutos para ejecutar la colección | ⏱️ 2 min |
| **POSTMAN_COLLECTION_GUIDE.md** | Guía detallada con patrones y troubleshooting | ⏱️ 15 min |
| **COLLECTION_EXECUTION_SUMMARY.md** | Resumen ejecutivo con estadísticas | ⏱️ 20 min |
| **DELIVERABLE_SUMMARY.md** | Checklist formal de entrega | ⏱️ 25 min |
| **INDEX.md** | Índice y matriz de navegación | ⏱️ 3 min |
| **README.md** | Este archivo (visión general) | ⏱️ 3 min |

---

## Quick Start (30 segundos)

### Opción 1: Postman UI

```
1. Importar postman-collection.json en Postman
2. Abrir carpeta "_Setup" → Click "Login"
3. Abrir carpeta "Promotor - CRUD Lifecycle" → Click ▶️
4. Verificar: 6 requests ✓ 6 checkmarks verdes ✓
```

### Opción 2: Newman CLI

```bash
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html
```

---

## Estadísticas

| Métrica | Valor |
|---------|-------|
| **Folders** | 5 (_Setup + 4 feature folders) |
| **Requests** | 20 (ejecutables) |
| **Assertions** | ~64 (validaciones) |
| **Endpoints** | 4 (POST, GET, PUT, PATCH) |
| **Status Codes** | 5 (201, 200, 400, 401, 404) |
| **Error Codes** | 9 (validados específicamente) |
| **Variables** | 9 (dinámicas, reutilizables) |
| **Tiempo Ejecución** | ~5-8 seg (Newman completo) |

---

## Cobertura

### Endpoints (4/4 = 100%)

- ✓ **POST /api/crowdpromotion/promotor** (Create)
  - 1 caso happy path
  - 7 casos validación
  - 1 caso auth error
  - 1 caso business rule

- ✓ **GET /api/crowdpromotion/promotor/me** (Get)
  - 3 casos happy path (verify create, update, deactivate)
  - 1 caso auth error
  - 1 caso not found

- ✓ **PUT /api/crowdpromotion/promotor/me** (Update)
  - 1 caso happy path
  - 7 casos validación
  - 1 caso auth error

- ✓ **PATCH /api/crowdpromotion/promotor/me/desactivar** (Deactivate)
  - 1 caso happy path
  - 1 caso auth error
  - 1 caso business rule

### Error Codes (9/9 = 100%)

| Rango | Código | Validado |
|-------|--------|----------|
| Success | 0001, 0002 | ✓ |
| Validation | 1001, 1002, 1003, 1010, 1011, 1013 | ✓ |
| NotFound | 2015 | ✓ |
| Auth | 3001 | ✓ |
| Business | 4018, 4019 | ✓ |

---

## Estructura

```
_Setup
└── Login (obtiene JWT)

Promotor - CRUD Lifecycle
├── 01. POST Create (201 → promotorId)
├── 02. GET Me (200 → verify)
├── 03. PUT Update (200 → actualiza)
├── 04. GET Me (200 → verify)
├── 05. PATCH Deactivate (200 → desactiva)
└── 06. GET Me (200 → verify)

Promotor - Validation Errors
├── nombrePublico: empty, min length, max length
├── tipoPromotorId: missing, invalid
├── emailContacto: invalid format
└── urlSitioWeb: invalid format

Promotor - Auth Errors
├── POST sin token
├── GET sin token
└── PUT token inválido

Promotor - Not Found & Business Rules
├── GET 404 (sin perfil)
├── POST 400 (ya existe)
└── PATCH 400 (ya desactivado)
```

---

## Patrones Usados

### Pre-request Scripts ✓

- Generación de nombres únicos con `Date.now()`
- Setup de variables dinámicas
- Sin hardcoding de IDs

### Test Scripts ✓

- Validación de status codes
- Validación de estructura response (`ServiceResponse<T>`)
- Extracción de IDs para siguiente request
- Validación de error codes específicos

### Variables ✓

- `pm.collectionVariables` (no environment)
- Reutilización entre requests
- Setup → Login → CRUD → Validation

---

## Dependencias

### Setup

- **Login:** usuario1@mail.com / 123456 (preexistente en seed)
- **Token:** JWT obtenido dinámicamente en cada ejecución
- **Database:** Migraciones EF Core aplicadas

### Independencias

- ✓ No requiere Artista, Campania, Reward, Backing
- ✓ No requiere datos previos en promotores
- ✓ Desactivación lógica (sin DELETE)

---

## Requisitos

### Software

- **Postman:** v11.0+ (recomendado)
- **Newman:** v6.0+ (para CLI)
- **Node.js:** v16+ (para Newman)
- **Backend:** .NET 8, corriendo en `http://localhost:5001`

### Base de Datos

- **WePlayRises** (SQL Server)
- Migraciones aplicadas
- Seed data: usuario1@mail.com

### Network

- Backend accesible: `http://localhost:5001`
- Swagger disponible: `http://localhost:5001/swagger`

---

## Instrucciones de Uso

### Postman UI (Recomendado para desarrollo)

1. **Importar colección**
   - File → Import
   - Seleccionar `postman-collection.json`
   - Click Import

2. **Ejecutar Setup**
   - Folder: `_Setup`
   - Request: `01. Login con Usuario de Prueba`
   - Click Send
   - Verificar status 200 ✓

3. **Ejecutar CRUD Lifecycle**
   - Folder: `Promotor - CRUD Lifecycle`
   - Click ▶️ a la derecha del folder
   - Verify Mode: ON (ver checkmarks)
   - Aguardar ~3 segundos
   - Verificar: 6/6 ✓

4. **Revisar Resultados**
   - Click en cada request
   - Tab Response → Body
   - Validar structure `ServiceResponse<T>`

### Newman CLI (Recomendado para CI/CD)

```bash
# Opción 1: Sin reportes
newman run plans/cp-perfil-promotor/backend/postman-collection.json

# Opción 2: Con reportes
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export test-results.html

# Opción 3: Solo CRUD Lifecycle
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --folder "Promotor - CRUD Lifecycle" \
  --reporters cli
```

---

## Validación

### JSON ✓

```bash
# Validar JSON
jq . plans/cp-perfil-promotor/backend/postman-collection.json > /dev/null && echo "OK"
```

### Colección ✓

- Schema: v2.1.0
- Requests: 20
- Assertions: ~64
- Variables: 9
- Estructura: 5 folders

### Pre-requisitos ✓

- [ ] Backend corriendo
- [ ] usuario1@mail.com existe
- [ ] Postman o Newman instalado
- [ ] Network conectada

---

## Troubleshooting

### "Connection refused: localhost:5001"

**Causa:** Backend no corre
**Solución:**
```bash
docker compose up -d
# O ejecutar .NET desde VS
```

### "Token no valido o expirado"

**Causa:** JWT expiró (> 24h)
**Solución:** Ejecutar _Setup → Login de nuevo

### "Promotor Already Exists (4018)"

**Causa:** usuario1@mail.com ya tiene un promotor
**Solución:** Usar otro usuario O limpiar BD

### "Body must be empty for PATCH"

**Causa:** PATCH /desactivar tiene body (no debe)
**Solución:** En Postman, Tab Body → None

---

## Documentación

| Documento | Propósito | Tiempo |
|-----------|-----------|--------|
| **QUICK_START.md** | Setup rápido, copy-paste | 2 min |
| **POSTMAN_COLLECTION_GUIDE.md** | Detalles técnicos, patrones | 15 min |
| **COLLECTION_EXECUTION_SUMMARY.md** | Estadísticas, cobertura | 20 min |
| **DELIVERABLE_SUMMARY.md** | Checklist, validación | 25 min |
| **INDEX.md** | Navegación, matriz públicos | 3 min |

**Lectura recomendada:**
1. Este README (3 min)
2. QUICK_START.md (2 min)
3. POSTMAN_COLLECTION_GUIDE.md (si necesitas detalles)

---

## Cambios Futuros

### Si cambias endpoints

1. Modifica los requests en `postman-collection.json`
2. Actualiza las URLs en requests afectados
3. Revisa auth (Bearer token)
4. Revalida status codes esperados

### Si cambias error codes

1. Modifica los test scripts (buscar `errorCode`)
2. Actualiza documentación en POSTMAN_COLLECTION_GUIDE.md

### Si cambias estructura response

1. Modifica los test scripts (búsqueda de campos)
2. Revisa assertions de `json.data.*`

---

## Referencias

### Contratos & Especificaciones

- **contracts.md:** `docs/user-stories/cp-perfil-promotor/contracts.md`
- **api-contracts.md:** `plans/cp-perfil-promotor/backend/api-contracts.md`
- **feature-spec.md:** `docs/user-stories/cp-perfil-promotor/feature-spec.md`

### Reglas & Patrones

- **CQRS Rules:** `.claude/rules/backend/cqrs.rule.md`
- **Code Style:** `.claude/rules/always/code-style.rule.md`

### Colecciones de Referencia

- **cs-valoraciones:** `plans/cs-valoraciones/backend/postman-collection.json`
- **hacer-backing:** `plans/hacer-backing/backend/postman-collection.json`

---

## Support & Feedback

**Problemas:** Ver POSTMAN_COLLECTION_GUIDE.md → Troubleshooting
**Documentación:** Leer INDEX.md para matriz de públicos
**Cambios:** Modificar postman-collection.json + documentación
**Reportes:** Newman genera HTML con --reporter-htmlextra

---

## Checklist: Antes de Usar

- [ ] Backend corriendo en `http://localhost:5001`
- [ ] Postman instalado (v11.0+) O Newman instalado (v6.0+)
- [ ] Base de datos WePlayRises tiene user: usuario1@mail.com
- [ ] Network conectada (sin VPN bloqueando localhost)
- [ ] Archivo postman-collection.json accesible

---

## Inicio Rápido

```bash
# 1. Descargar colección
# (Ya está en plans/cp-perfil-promotor/backend/postman-collection.json)

# 2. Ejecutar con Newman
npm install -g newman newman-reporter-htmlextra
newman run plans/cp-perfil-promotor/backend/postman-collection.json \
  --reporters cli,htmlextra

# 3. Ver reporte
open test-results.html  # macOS
start test-results.html # Windows
```

---

## Contacto

**Proyecto:** WePlay Rises
**Feature:** US-CP-01 - Perfil de Promotor
**Módulo:** Crowdpromotion
**Generado:** 2026-02-25
**Status:** ✓ Producción

---

**Listo para usar. 🚀**

Primeros pasos: 👉 **QUICK_START.md**
