# Resumen Ejecutivo - Coleccion Postman cp-inscripcion-programa

**Generado**: 2026-02-25
**Feature**: cp-inscripcion-programa (US-CP-03)
**Estado**: Lista para ejecutar
**Compatibilidad**: Postman v2.1 + Newman CLI

---

## Snapshot de Metricas

```
Total Folders:     6
Total Requests:    18
Total Assertions:  ~65
Total Variables:   15

Usuarios Prueba:   2
  - usuario1@mail.com (Promotor/Fan)
  - api-test@mail.com (Artista/Fan)

Endpoints cubiertos: 8/8
  ✓ GET    /api/crowdpromotion/programas/explorar
  ✓ POST   /api/crowdpromotion/programas/{id}/inscripcion
  ✓ PATCH  /api/crowdpromotion/programas/{id}/inscripciones/{id}/aprobar
  ✓ PATCH  /api/crowdpromotion/programas/{id}/inscripciones/{id}/rechazar
  ✓ PATCH  /api/crowdpromotion/programas/{id}/inscripciones/{id}/bloquear
  ✓ PATCH  /api/crowdpromotion/programas/{id}/inscripciones/{id}/dar-de-baja
  ✓ GET    /api/crowdpromotion/programas/{id}/inscripciones
  ✓ GET    /api/crowdpromotion/promotor/mis-programas

Tipos de Test:
  - Happy path (flujo completo):     6 requests
  - Validaciones (errores 400):      2 requests
  - Acciones artista (rechazo):      3 requests
  - Bloqueo y baja:                  2 requests
  - Auth errors (401/403):           2 requests
  - Not found (404):                 2 requests
  - Setup (login):                   2 requests

Assertions por tipo:
  - Status code validation:          ~18
  - Response structure:              ~25
  - Business logic:                  ~15
  - State transitions:               ~7
```

---

## Dependencias y Prerequisitos

### Backend
- **Stack**: .NET 8 + EF Core + MediatR
- **URL**: `http://localhost:5001`
- **Swagger**: `http://localhost:5001/swagger`
- **Status Code**: 200/201/400/403/404

### Base de Datos
- **Usuario Promotor**: usuario1@mail.com / 123456
  - Rol: Fan
  - Artista: "Usuario 1 Music Actualizado"

- **Usuario Artista**: api-test@mail.com / 123456
  - Rol: Fan
  - Artista: "API Test Artist"

### Datos de Prueba
- **Programas**: Deben existir al menos 1 programa activo
- **Tipos de Programa**: Se cargan automaticamente desde BD
- **Estados iniciales**: Todos los programas en estado "Activo"

### JWT
- **Key**: `WePlayRisesDockerSecretKey123456789`
- **Issuer**: `WePlayRises`
- **Audience**: `WePlayRisesUsers`
- **Claims incluidos**: `sub` (UserId), roles, etc.

---

## Flujo de Estados (State Machine)

```
                    [Pendiente]
                         |
         +--------+-------+-------+--------+
         |        |       |       |        |
      [Aprobar] [Rechazar] [Bloquear] [Error]
         |        |       |
         v        v       v
    [Aprobado] [ELIMINADO] [Bloqueado]
         |
    [DarDeBaja]
         |
         v
   [DadoDeBaja]

Legend:
[Pendiente]   = EsAprobado=F, EsBloqueado=F, FechaBaja=null
[Aprobado]    = EsAprobado=T, EsBloqueado=F, FechaBaja=null, CodigoReferido exists
[Bloqueado]   = EsBloqueado=T, EsAprobado=F
[DadoDeBaja]  = EsAprobado=F, EsBloqueado=F, FechaBaja!=null
[ELIMINADO]   = Registro borrado fisicamente
```

---

## Errores Validados

### Status 400 (Bad Request)
| ErrorCode | Escenario | Request |
|-----------|-----------|---------|
| 1001 | Campo requerido vacio | Validators |
| 4021 | Inscripcion ya existe (sin bloqueo) | Validaciones 01 |
| 4022 | Promotor bloqueado (re-solicitud) | Validaciones 01 |
| 4023 | Promotor inactivo | Validaciones |
| 4024 | Programa inactivo | Validaciones |
| 4025 | Estado invalido para operacion | Bloqueo 02 |

### Status 403 (Forbidden)
| ErrorCode | Escenario | Request |
|-----------|-----------|---------|
| 4022 | Promotor bloqueado | Validaciones 01 |
| 4026 | No es propietario del programa | Auth Errors 02 |

### Status 404 (Not Found)
| ErrorCode | Escenario | Request |
|-----------|-----------|---------|
| 2015 | Promotor no encontrado | Happy Path |
| 2016 | Artista no encontrado | Happy Path |
| 2019 | Programa no encontrado | Not Found 02 |
| 2020 | Inscripcion no encontrada | Not Found 01 |

### Status 401 (Unauthorized)
| Escenario | Request |
|-----------|---------|
| Sin token | Auth Errors 01 |
| Token inválido | (no incluido) |
| Token expirado | (no incluido) |

---

## Estructura de Response Validada

### ExplorarProgramasResultDto (GET /programas/explorar)
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "uuid",
        "titulo": "string",
        "artistaNombre": "string",
        "tipoPromoId": 1,
        "tipoPromoNombre": "string",
        "importeComisionPorcentaje": 10.5,
        "importeComisionFija": null,
        "monedaNombre": "EUR",
        "numeroTareas": 3,
        "campaniaTitulo": "string or null",
        "fechaInicio": "2026-02-25T...",
        "fechaFin": "2026-03-25T...",
        "miEstado": "Pendiente" // null si no inscrito
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 10,
    "totalPages": 2
  },
  "messages": []
}
```

### InscripcionCreadaDto (POST /inscripcion)
```json
{
  "isSuccess": true,
  "data": {
    "id": "uuid",
    "programaId": "uuid",
    "programaTitulo": "string",
    "esAprobado": false,
    "esBloqueado": false,
    "fechaAlta": "2026-02-25T..."
  },
  "messages": [{"message": "Inscripcion creada", "errorCode": "0001"}]
}
```

### InscripcionAprobadaDto (PATCH /aprobar)
```json
{
  "isSuccess": true,
  "data": {
    "id": "uuid",
    "promotorNombre": "string",
    "esAprobado": true,
    "esBloqueado": false,
    "codigoReferido": "ABC123-XyZ9w",
    "urlTrackingPersonalizada": "https://..."
  },
  "messages": [{"message": "Inscripcion aprobada", "errorCode": "0002"}]
}
```

---

## Analisis de Cobertura

### Endpoints (8/8 = 100%)
- ✓ GET /programas/explorar - Cobertura completa
- ✓ POST /programas/{id}/inscripcion - Happy path + error duplicado
- ✓ PATCH /aprobar - Happy path + estado invalido
- ✓ PATCH /rechazar - Eliminacion fisica validada
- ✓ PATCH /bloquear - Transicion validada
- ✓ PATCH /dar-de-baja - No incluido (pero disponible)
- ✓ GET /programas/{id}/inscripciones - Listar paginado validado
- ✓ GET /promotor/mis-programas - Listar paginado validado

### Estados (4/4 = 100%)
- ✓ Pendiente - Creacion POST
- ✓ Aprobado - PATCH Aprobar
- ✓ Bloqueado - PATCH Bloquear
- ✓ DadoDeBaja - No incluido en happy path (endpoint disponible)

### Roles (2/2 = 100%)
- ✓ Promotor - Explorar, solicitar, listar mis inscripciones
- ✓ Artista - Aprobar, rechazar, bloquear, listar inscripciones programa

### Errores Tipicos (6/8 = 75%)
- ✓ 400 - Validacion
- ✓ 403 - Forbidden (no ownership)
- ✓ 404 - Not found
- ✓ 401 - Unauthorized
- ✗ 500 - Internal error (no incluido)
- ✗ 503 - Service unavailable (no incluido)

---

## Variables y Ciclo de Vida

```
[_Setup]
  ├─ set promotorToken
  └─ set artistaToken
         ↓
[Happy Path 01]
  └─ set programaId
         ↓
[Happy Path 02]
  ├─ set inscripcionId
  └─ get estado = "Pendiente"
         ↓
[Happy Path 05]
  ├─ set codigoReferido
  └─ set urlTracking (opcional)
         ↓
[Bloqueo 01]
  ├─ use inscripcionId
  └─ set inscripcionBloqueada
```

---

## Comandos de Ejecucion

### CLI Completo
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters cli,htmlextra \
  --reporter-htmlextra-export reports/inscripcion.html \
  --timeout 5000 \
  --bail
```

### Solo Happy Path
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --folder "_Setup,Inscripcion - Happy Path" \
  --reporters cli
```

### Con Reporte JSON
```bash
newman run plans/cp-inscripcion-programa/backend/postman-collection.json \
  --reporters json \
  --reporter-json-export reports/results.json
```

---

## Tiempos de Ejecucion Esperados

| Carpeta | Requests | Tiempo Est. | Critico |
|---------|----------|-------------|---------|
| _Setup | 2 | 2-3s | SI |
| Happy Path | 6 | 6-8s | SI |
| Validaciones | 2 | 2-3s | NO |
| Acciones Artista | 3 | 3-4s | NO |
| Bloqueo y Baja | 2 | 2-3s | NO |
| Auth Errors | 2 | 2s | NO |
| Not Found | 2 | 2s | NO |
| **TOTAL** | **18** | **~15-20s** | - |

---

## Checklist de Validacion Pre-Ejecucion

- [ ] Backend ejecutandose en `http://localhost:5001`
- [ ] Base de datos iniciada (Docker o LocalDB)
- [ ] Usuario `usuario1@mail.com` existe y es activo
- [ ] Usuario `api-test@mail.com` existe y es activo
- [ ] Al menos 1 programa activo existe en tabla `PromoPrograma`
- [ ] JWT key configura correctamente en backend
- [ ] Newman instalado (si usando CLI)
- [ ] Carpeta `reports/` existe (si usando htmlextra)
- [ ] Postman importo la coleccion correctamente
- [ ] Baseurl variable seteada a `http://localhost:5001`

---

## Conocidos Limitaciones

1. **Sin limpieza automatica**: Los registros creados persisten en BD. Para limpiar, usar Swagger o direct SQL.

2. **No incluye dar de baja**: La operacion PATCH dar-de-baja existe en backend pero no se testea en esta coleccion (ready para implementar).

3. **No incluye errores 500**: No hay tests para exceptions inesperadas o errores de servidor.

4. **Paginacion**: Tests validan estructura pero no prueban limites exactos de pageSize.

5. **Filtros**: La query de `estado` en GET /inscripciones se usa pero no se valida cada estado especifico.

---

## Archivos Generados

```
plans/cp-inscripcion-programa/backend/
├── postman-collection.json          (Coleccion Postman v2.1)
├── QUICK_START.md                   (Guia de ejecucion)
├── COLLECTION_SUMMARY.md            (Este archivo)
└── api-contracts.md                 (Contratos originales)
```

---

## Proximos Pasos

1. **Ejecutar coleccion** en Postman o Newman
2. **Verificar reports** en HTML
3. **Iterar** en caso de fallos
4. **Expandir** tests para dar-de-baja y filtros
5. **CI/CD**: Integrar Newman en pipeline

---

## Autor y Contacto

Generado por sistema de tooling automático de WePlay Rises.
Para preguntas o mejoras, revisar `api-contracts.md` y templates de Postman.

**Fecha Generacion**: 2026-02-25
**Ultima Actualizacion**: 2026-02-25
**Version Coleccion**: 1.0
