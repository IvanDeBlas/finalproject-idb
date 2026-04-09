# Colección Postman - Gestionar Necesidades de Crowdsourcing

**Feature:** US-CS-02 - cs-gestionar-necesidades
**Módulo:** Crowdsourcing
**Versión:** 1.0.0
**Fecha:** 2026-02-16

---

## Archivos en Esta Carpeta

```
plans/cs-gestionar-necesidades/backend/
├── postman-collection.json          # COLECCIÓN EJECUTABLE (Postman v2.1 JSON)
├── README.md                        # Este archivo (índice)
├── QUICK-START.md                   # Para comenzar en 5 minutos
├── POSTMAN-README.md                # Documentación completa y detallada
└── COLLECTION-SUMMARY.md            # Resumen técnico de cada test
```

---

## ¿Por Dónde Empiezo?

### 1️⃣ **Quiero ejecutar los tests AHORA** (5 minutos)
→ Lee `QUICK-START.md`

**Resumen:** Importa JSON a Postman → Click "Run" en "_Setup" → Click "Run" en "CRUD Lifecycle" → ✓ Listo

### 2️⃣ **Quiero entender qué hace cada test** (15 minutos)
→ Lee `COLLECTION-SUMMARY.md`

**Contiene:** Descripción de cada uno de los 25 requests, assertions, variables, ErrorCodes

### 3️⃣ **Quiero documentación completa** (30 minutos)
→ Lee `POSTMAN-README.md`

**Contiene:** Setup detallado, troubleshooting, CI/CD, ejemplos Newman, reportes

### 4️⃣ **Quiero revisar el JSON directamente**
→ Abre `postman-collection.json` en editor

---

## Características de la Colección

✅ **100% Auto-inclusiva**
- No requiere datos previos en BD
- El _Setup obtiene todo dinámicamente
- Usa usuario de prueba existente (`usuario1@mail.com`)

✅ **CRUD Lifecycle Completo**
- POST Create → 201
- GET All (listado) → 200
- GET All (filtro por estado) → 200
- GET All (búsqueda por texto) → 200
- GET By ID (detalle) → 200
- PUT Update → 200
- GET By ID (verify update) → 200
- PATCH Close → 200
- GET By ID (verify closed) → 200

✅ **Validación Errors (400)**
- Título vacío → 1001
- Título < 5 caracteres → 1011
- Presupuesto Max < Min → 1009
- Moneda requerida → 1001

✅ **Business Rule Errors (400)**
- No editar necesidad cerrada → 4001
- No cerrar necesidad ya cerrada → 4002

✅ **Auth Errors (401)**
- Sin token → 401
- Token inválido → 401

✅ **Not Found (404)**
- Necesidad inexistente → 404

✅ **+85 Assertions Comprehensivas**
- Status codes
- Response structure
- Field existence
- Field values
- Collections state

---

## Estructura de la Colección

```
WePlay.cs-gestionar-necesidades.IntegrationTests
│
├── _Setup (6 requests)
│   └── Login + obtener maestras (tipos, modalidades, monedas)
│
├── Necesidades - CRUD Lifecycle (9 requests)
│   └── Flujo principal: create → list → filter → search → detail → update → close
│
├── Necesidades - Validation Errors (4 requests)
│   └── Tests de validación de campos (400)
│
├── Necesidades - Business Rules (2 requests)
│   └── Tests de estados y reglas (400)
│
├── Necesidades - Auth Errors (2 requests)
│   └── Tests sin token / token inválido (401)
│
└── Necesidades - Not Found (2 requests)
    └── Tests de recurso inexistente (404)

Total: 25 requests | ~85 assertions
```

---

## Endpoints Testeados

| Método | Ruta | Tests | Status |
|--------|------|-------|--------|
| POST | `/api/crowdsourcing/necesidades` | Create, Create (validation errors) | 201, 400 |
| GET | `/api/crowdsourcing/necesidades/mis-necesidades` | List, Filter, Search | 200 |
| GET | `/api/crowdsourcing/necesidades/{id}` | Detail (2x verify), Not found | 200, 404 |
| PUT | `/api/crowdsourcing/necesidades/{id}` | Update, Not editable (400), Not found | 200, 400, 404 |
| PATCH | `/api/crowdsourcing/necesidades/{id}/cerrar` | Close, Not closeable (400) | 200, 400 |

---

## ErrorCodes Testeados

| Code | Nombre | Escenario |
|------|--------|-----------|
| 1001 | Validation_Required | Campo requerido (titulo, moneda) |
| 1009 | Validation_InvalidRange | Presupuesto Max < Min |
| 1011 | Validation_MinLength | Título < 5 caracteres |
| 4001 | BusinessRule_NecesidadNotEditable | Editar necesidad cerrada |
| 4002 | BusinessRule_NecesidadNotCloseable | Cerrar necesidad ya cerrada |

---

## Variables de Colección

Se establecen automáticamente en _Setup:

| Variable | Valor Inicial | Establecida por | Uso |
|----------|---------------|-----------------|-----|
| `baseUrl` | `http://localhost:5001` | Manual | Base de todas las URLs |
| `testEmail` | `usuario1@mail.com` | Manual | Credenciales test |
| `testPassword` | `123456` | Manual | Credenciales test |
| `accessToken` | (vacío) | Login | Header `Authorization: Bearer {{accessToken}}` |
| `artistaId` | (vacío) | Get Artista ID | Extrae pero no usado aquí |
| `proyectoArtisticoId` | (vacío) | Get Proyecto | Body de POST/PUT |
| `tipoNecesidadId` | (vacío) | Get Tipos | Body de POST |
| `modalidadTrabajoId` | (vacío) | Get Modalidades | Body de POST/PUT |
| `monedaId` | (vacío) | Get Monedas | Body de POST/PUT |
| `necesidadId` | (vacío) | POST Create | Path param `{id}` |

---

## Requisitos Previos

1. **Backend ejecutándose**
   ```bash
   cd src/api
   dotnet run --project WebApi
   ```
   - Disponible en `http://localhost:5001`
   - Swagger en `http://localhost:5001/swagger`

2. **Usuario de prueba existente**
   - Email: `usuario1@mail.com`
   - Password: `123456`
   - Rol: Artista
   - Con Artista y ProyectoArtistico asociados

3. **Maestras pobladas**
   - TiposNecesidad (al menos 1)
   - ModalidadesTrabajo (Presencial=1, Remoto=2, Híbrido=3)
   - Monedas (EUR=1, USD=2, GBP=3)
   - EstadoNecesidad (Abierta=1, EnProgreso=2, Cerrada=3, Cancelada=4)

4. **Postman instalado** (opcional, para ejecución en UI)
   - Descargar desde https://www.postman.com/downloads/
   - O usar Newman CLI (ver POSTMAN-README.md)

---

## Cómo Ejecutar

### Opción 1: Postman UI (Recomendado para Desarrollo)

```bash
1. Abrir Postman
2. File → Import → Seleccionar postman-collection.json
3. Click carpeta "_Setup" → Run
4. Click carpeta "Necesidades - CRUD Lifecycle" → Run
5. Esperar resultado (verde = ✓, rojo = ✗)
```

**Ventajas:**
- Interfaz visual
- Debugging fácil
- Ver requests/responses en detalle

### Opción 2: Newman CLI (Recomendado para CI/CD)

```bash
# Instalar (primera vez)
npm install -g newman
npm install -g newman-reporter-htmlextra

# Ejecutar todo
newman run plans/cs-gestionar-necesidades/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/test-report.html

# Ejecutar solo carpeta específica
newman run postman-collection.json \
  --folder "Necesidades - CRUD Lifecycle" \
  --reporters cli
```

**Ventajas:**
- Automatización
- Reportes HTML
- Integración CI/CD
- Salida JSON para parsing

---

## Resultados Esperados

### Setup (6/6 requests ✓)
```
✓ 01. Login User
✓ 02. Get Artista ID
✓ 03. Get Proyecto Artístico
✓ 04. Get Maestras - TiposNecesidad
✓ 05. Get Maestras - ModalidadesTrabajo
✓ 06. Get Maestras - Monedas
```

### CRUD Lifecycle (9/9 requests ✓)
```
✓ 01. POST Create Necesidad (201)
✓ 02. GET All Necesidades (200)
✓ 03. GET All - Filter by Estado (200)
✓ 04. GET All - Search by Text (200)
✓ 05. GET Necesidad By ID (200)
✓ 06. PUT Update Necesidad (200)
✓ 07. GET - Verify Update (200)
✓ 08. PATCH Close Necesidad (200)
✓ 09. GET - Verify Closed (200)
```

### Validaciones (4/4 requests ✓)
```
✓ POST 400 - Titulo Vacio
✓ POST 400 - Titulo Menor a 5
✓ POST 400 - Presupuesto Max < Min
✓ POST 400 - Moneda Requerida
```

---

## Estructura de Carpetas (Backend)

Después de implementar los endpoints, la estructura será:

```
Modules/Crowdsourcing/
├── Crowdsourcing.Domain/
│   └── Constants/ServiceResponseMessageType.cs (nuevas constantes)
│
├── Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── CreateNecesidadRequest.cs
│   │   ├── UpdateNecesidadRequest.cs
│   │   ├── CerrarNecesidadRequest.cs
│   │   ├── NecesidadCrowdsourcingListDto.cs
│   │   ├── NecesidadCrowdsourcingDto.cs
│   │   └── PropuestaCrowdsourcingDto.cs
│   │
│   ├── Features/Necesidades/
│   │   ├── Commands/
│   │   │   ├── CreateNecesidadCommand.cs
│   │   │   ├── UpdateNecesidadCommand.cs
│   │   │   └── CerrarNecesidadCommand.cs
│   │   │
│   │   ├── Queries/
│   │   │   ├── GetMisNecesidadesQuery.cs
│   │   │   └── GetNecesidadByIdQuery.cs
│   │   │
│   │   └── Validators/
│   │       ├── CreateNecesidadCommandValidator.cs
│   │       ├── UpdateNecesidadCommandValidator.cs
│   │       └── CerrarNecesidadCommandValidator.cs
│   │
│   └── Mapping/
│       ├── NecesidadCrowdsourcingProfile.cs
│       └── PropuestaCrowdsourcingProfile.cs
│
└── Crowdsourcing.WebApi/
    └── Controllers/NecesidadesCrowdsourcingController.cs
```

---

## Documentación Relacionada

| Archivo | Propósito | Audiencia |
|---------|-----------|-----------|
| `postman-collection.json` | Colección ejecutable | Developers, QA, CI/CD |
| `QUICK-START.md` | Guía de 5 minutos | Developers (primera vez) |
| `COLLECTION-SUMMARY.md` | Detalles técnicos | QA, Code Reviewers |
| `POSTMAN-README.md` | Documentación completa | Developers, DevOps |
| `api-contracts.md` | Especificación de API | Developers |
| `hexagonal-architecture.md` | Diseño de la feature | Architects |

---

## Troubleshooting

### Problem: "Token not found" en CRUD
**Solution:** Re-ejecuta _Setup primero

### Problem: "Necesidad no encontrada"
**Solution:** Verifica que `usuario1@mail.com` existe en BD

### Problem: "Backend not responding"
**Solution:** Ejecuta `dotnet run --project WebApi` en src/api

Más ayuda en `POSTMAN-README.md` → Troubleshooting

---

## Notas Importantes

1. **Datos no se limpian:** Las necesidades creadas permanecen en la BD
   - Normal para testeo múltiple
   - Usa DELETE endpoint si necesitas limpiar

2. **Timestamps dinámicos:** El título incluye `{{$timestamp}}`
   - Evita duplicados al ejecutar múltiples veces

3. **Fechas hardcodeadas:** Usa futuro relativo (2026-03-15, etc.)
   - Válidas hasta 2026-03-15
   - Actualiza si necesitas extender

4. **No hay limpieza automática:** Importa manualmente o usa:
   ```bash
   # Hacer rollback de la BD
   git checkout -- (database backup)
   ```

---

## Métricas de Cobertura

| Métrica | Valor |
|---------|-------|
| **Endpoints únicos** | 7 |
| **Métodos HTTP** | POST, GET, PUT, PATCH |
| **Status codes** | 200, 201, 400, 401, 404 |
| **ErrorCodes cubiertos** | 1001, 1009, 1011, 4001, 4002 |
| **Requests** | 25 |
| **Assertions** | ~85 |
| **Folders** | 6 |
| **Tiempo ejecución** | ~15 segundos |

---

## Checklist para QA

- [ ] _Setup completa sin errores (6/6)
- [ ] CRUD Lifecycle completa sin errores (9/9)
- [ ] Validaciones retornan 400 (4/4)
- [ ] Business rules retornan 400 (2/2)
- [ ] Auth errors retornan 401 (2/2)
- [ ] Not found retorna 404 (2/2)
- [ ] Todas las assertions pasan (85/85)
- [ ] Variables de colección se establecen correctamente
- [ ] Datos persisten en BD después de tests
- [ ] Timestamp en título es único (evita duplicados)

---

## Siguiente Paso

Lee `QUICK-START.md` para comenzar a ejecutar en 5 minutos.

---

**Mantenida por:** WePlay Development Team
**Última actualización:** 2026-02-16
**Versión de Postman:** v2.1.0
**Schema de API:** OpenAPI 3.0
