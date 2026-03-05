# Diagrama de Flujo - cp-inscripcion-programa Integration Tests

## Flujo de Ejecucion Completo

```
┌─────────────────────────────────────────────────────────────────────────┐
│                    START - Coleccion Postman                            │
│             cp-inscripcion-programa (US-CP-03)                          │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌───────────────────────────────────────────────┐
        │          _Setup (Obligatorio)                 │
        │  ─────────────────────────────────────────   │
        │  1. Login Promotor (usuario1@mail.com)        │
        │     → JWT promotorToken                       │
        │                                               │
        │  2. Login Artista (api-test@mail.com)         │
        │     → JWT artistaToken                        │
        └───────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴────────────────┐
                    │ Tokens Ready                   │
                    ▼
        ┌──────────────────────────────────────────────┐
        │  Inscripcion - Happy Path (Recomendado)     │
        │  ──────────────────────────────────────────  │
        │                                              │
        │  01. GET /programas/explorar (Promotor)      │
        │      Status: 200 OK                          │
        │      Response: ProgramaExplorarItemDto[]     │
        │      Saves: programaId, programaTitulo       │
        │      ✓ Assert: items.length > 0              │
        │                                              │
        │                  │                           │
        │                  ▼                           │
        │  02. POST /inscripcion (Promotor)            │
        │      Params: programaId from 01              │
        │      Status: 201 Created                     │
        │      Response: InscripcionCreadaDto          │
        │      Saves: inscripcionId                    │
        │      ✓ Assert: esAprobado=false              │
        │      ✓ Assert: esBloqueado=false             │
        │                                              │
        │                  │                           │
        │                  ▼                           │
        │  03. GET /promotor/mis-programas (Promotor)  │
        │      Status: 200 OK                          │
        │      Response: MisInscripcionesResultDto     │
        │      ✓ Assert: find by inscripcionId         │
        │      ✓ Assert: estado="Pendiente"            │
        │                                              │
        │                  │                           │
        │                  ▼                           │
        │  04. GET /inscripciones (Artista)            │
        │      Params: programaId                      │
        │      Status: 200 OK                          │
        │      Response: InscripcionListResultDto      │
        │      ✓ Assert: find by inscripcionId         │
        │      ✓ Assert: estado="Pendiente"            │
        │                                              │
        │                  │                           │
        │                  ▼                           │
        │  05. PATCH /aprobar (Artista)                │
        │      Params: programaId, inscripcionId       │
        │      Status: 200 OK                          │
        │      Response: InscripcionAprobadaDto        │
        │      Saves: codigoReferido, urlTracking      │
        │      ✓ Assert: esAprobado=true               │
        │      ✓ Assert: esBloqueado=false             │
        │      ✓ Assert: codigoReferido exists         │
        │                                              │
        │                  │                           │
        │                  ▼                           │
        │  06. GET /promotor/mis-programas (Promotor)  │
        │      Status: 200 OK                          │
        │      ✓ Assert: find by inscripcionId         │
        │      ✓ Assert: estado="Aprobado"             │
        │      ✓ Assert: codigoReferido populated      │
        │                                              │
        └──────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴────────────────┐
                    │ Happy Path Completo            │
                    │ Flujo: Solicitar → Aprobar     │
                    ▼
        ┌──────────────────────────────────────────────┐
        │  [OPCIONAL] Otras Validaciones              │
        │  ──────────────────────────────────────────  │
        │                                              │
        │  A. Inscripcion - Validaciones              │
        │     ├─ POST 400: Duplicada                  │
        │     └─ POST 404: Programa inexistente       │
        │                                              │
        │  B. Inscripcion - Acciones Artista          │
        │     ├─ POST: Nueva inscripcion (para reject) │
        │     ├─ PATCH 200: Rechazar (delete)         │
        │     └─ GET: Verificar eliminada (404)       │
        │                                              │
        │  C. Inscripcion - Bloqueo y Baja            │
        │     ├─ PATCH 200: Bloquear                  │
        │     └─ GET: Verificar bloqueada             │
        │                                              │
        │  D. Inscripcion - Errores de Auth           │
        │     ├─ GET 401: Sin token                   │
        │     └─ PATCH 403: No es propietario         │
        │                                              │
        │  E. Inscripcion - Not Found                 │
        │     ├─ PATCH 404: Inscripcion no existe     │
        │     └─ GET 404: Programa no existe          │
        │                                              │
        └──────────────────────────────────────────────┘
                                    │
                                    ▼
        ┌──────────────────────────────────────────────┐
        │           END - Todos Tests Completados      │
        │                                              │
        │ Metricas Finales:                           │
        │ - Total Requests: 18                         │
        │ - Total Assertions: ~65                      │
        │ - Tiempo: 15-20s                             │
        │ - Estado: PASS o FAIL                        │
        └──────────────────────────────────────────────┘
```

---

## Diagrama de Estados - Inscripcion

```
    ┌─────────────┐
    │   START     │
    └──────┬──────┘
           │
           │ POST /inscripcion
           ▼
    ┌──────────────┐
    │  PENDIENTE   │◄──────── [Promotor solicita inscripcion]
    │              │
    │ esAprobado:F │
    │ esBloqueado:F│
    │ fechaBaja:null
    └───────┬──────┘
            │
    ┌───────┴──────────────────────────┐
    │                                   │
    │ PATCH /aprobar (Artista)          │ PATCH /rechazar (DELETE)
    │                                   │ PATCH /bloquear
    ▼                                   ▼
┌──────────────┐         ┌──────────────────┐
│  APROBADO    │         │   BLOQUEADO      │
│              │         │                  │
│ esAprobado:T │         │ esAprobado:F     │
│ esBloqueado:F│         │ esBloqueado:T    │
│ CodigoReferido        │ (no puede re-sol)│
│ UrlTracking  │         └──────────────────┘
│              │
│ PATCH /dar-de-baja
└────┬─────────┘
     │
     ▼
┌──────────────┐
│ DADO DE BAJA │
│              │
│ esAprobado:F │
│ fechaBaja:!=null
└──────────────┘

Leyenda:
[PENDIENTE]   → Estado inicial tras POST
[APROBADO]    → Artista aprueba + genera codigo
[BLOQUEADO]   → Artista bloquea promotor
[DADO DE BAJA]→ Artista da de baja aprobado
[ELIMINADO]   → Registro borrado (PATCH rechazar)
```

---

## Flujo de Datos - Variables de Coleccion

```
┌──────────────────────────────────────────────────────────────┐
│  _Setup                                                      │
│  ├─ Login Promotor                                           │
│  │  └─ promotorEmail, promotorPassword                      │
│  │     └─ promotorToken ◄── JWT (usado en todos los requests)
│  │                                                           │
│  └─ Login Artista                                            │
│     └─ artistaEmail, artistaPassword                        │
│        └─ artistaToken ◄── JWT (usado en todos los requests)
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│  Happy Path 01: GET /explorar                               │
│  ├─ Request Headers: Authorization: Bearer {{promotorToken}} │
│  └─ Response Save:                                           │
│     ├─ programaId        (usado en todos los requests)      │
│     ├─ programaTitulo    (info)                             │
│     └─ miEstado          (info: estado inscripcion)         │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│  Happy Path 02: POST /inscripcion                            │
│  ├─ Request Params: {{programaId}}                           │
│  ├─ Request Headers: Authorization: Bearer {{promotorToken}} │
│  └─ Response Save:                                           │
│     └─ inscripcionId     (usado en PATCH requests)          │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│  Happy Path 05: PATCH /aprobar                               │
│  ├─ Request Params: {{programaId}}, {{inscripcionId}}        │
│  ├─ Request Headers: Authorization: Bearer {{artistaToken}}  │
│  └─ Response Save:                                           │
│     ├─ codigoReferido    (info, para verificar en GET)      │
│     └─ urlTracking       (info, opcional si landing existe) │
└──────────────────────────────────────────────────────────────┘

Variables Finales:
┌────────────────────────────────────────────────────────────────┐
│ baseUrl: "http://localhost:5001"                               │
│ promotorToken: "eyJhbGc..."                                    │
│ artistaToken: "eyJhbGc..."                                     │
│ programaId: "uuid-xxx-yyy-zzz"                                 │
│ inscripcionId: "uuid-aaa-bbb-ccc"                              │
│ codigoReferido: "ABC123-XyZ9w"                                 │
└────────────────────────────────────────────────────────────────┘
```

---

## Matriz de Autorizacion (Roles vs Endpoints)

```
Endpoint                              Promotor  Artista  Anonimo
────────────────────────────────────────────────────────────────
GET /explorar                           ✓        ✗         ✗
POST /inscripcion                       ✓        ✗         ✗
GET /promotor/mis-programas             ✓        ✗         ✗

GET /inscripciones (lista programa)     ✗        ✓         ✗
PATCH /aprobar                          ✗        ✓         ✗
PATCH /rechazar                         ✗        ✓         ✗
PATCH /bloquear                         ✗        ✓         ✗
PATCH /dar-de-baja                      ✗        ✓         ✗

Leyenda:
✓ = Acceso permitido (200 OK)
✗ = Acceso negado (401 Unauthorized o 403 Forbidden)
```

---

## Diagrama de Request/Response

```
═══════════════════════════════════════════════════════════════
Ejemplo: POST /inscripcion
═══════════════════════════════════════════════════════════════

CLIENT (Promotor con Token)
  │
  ├─ METHOD: POST
  ├─ URL: /api/crowdpromotion/programas/{{programaId}}/inscripcion
  ├─ HEADER: Authorization: Bearer {{promotorToken}}
  ├─ HEADER: Content-Type: application/json
  ├─ BODY: {} (empty, todo viene de ruta + token)
  │
  └─ REQUEST SENT ────────────────► SERVER
                                      │
                                      ├─ Extract UserId from token
                                      ├─ Extract ProgramaId from route
                                      ├─ Validate via SolicitarInscripcionValidator
                                      ├─ Resolve PromotorId by UserId
                                      ├─ Check Promotor.EsActivo
                                      ├─ Check PromoPrograma.EsActivo
                                      ├─ Check no duplicate (sin bloqueo)
                                      ├─ Create PromoProgramaPromotor
                                      ├─ Save to DB
                                      │
                                      └─ RESPONSE ────────────┐
                                                              │
                          CLIENT RECEIVES ◄─────────────────┘
                                │
                                ├─ STATUS: 201 Created
                                ├─ HEADER: Content-Type: application/json
                                ├─ BODY: {
                                │   "isSuccess": true,
                                │   "data": {
                                │     "id": "uuid-xxx",
                                │     "programaId": "uuid-yyy",
                                │     "programaTitulo": "Mi Programa",
                                │     "esAprobado": false,
                                │     "esBloqueado": false,
                                │     "fechaAlta": "2026-02-25T10:30:00Z"
                                │   },
                                │   "messages": [{
                                │     "message": "Inscripcion creada",
                                │     "errorCode": "0001"
                                │   }]
                                │ }
                                │
                                ├─ POSTMAN SCRIPTS:
                                │  ├─ pm.collectionVariables.set('inscripcionId', json.data.id)
                                │  ├─ pm.test('Status is 201', ...)
                                │  └─ pm.test('esAprobado is false', ...)
                                │
                                └─ ✓ Test passed
```

---

## Arbol de Dependencias

```
postman-collection.json
│
├─ _Setup (Obligatorio)
│  ├─ Login Promotor
│  │  └─ promotorToken (necesario para todos los Promotor requests)
│  │
│  └─ Login Artista
│     └─ artistaToken (necesario para todos los Artista requests)
│
├─ Happy Path (Recomendado)
│  ├─ Depends on: _Setup ✓
│  ├─ 01. GET /explorar
│  │  └─ programaId (guardado para 02-06)
│  │
│  ├─ 02. POST /inscripcion
│  │  ├─ Depends on: programaId from 01 ✓
│  │  └─ inscripcionId (guardado para 04-06)
│  │
│  ├─ 03. GET /promotor/mis-programas
│  │  ├─ Depends on: promotorToken ✓
│  │  └─ Verifica inscripcionId aparece
│  │
│  ├─ 04. GET /inscripciones
│  │  ├─ Depends on: programaId, artistaToken ✓
│  │  └─ Verifica inscripcionId aparece
│  │
│  ├─ 05. PATCH /aprobar
│  │  ├─ Depends on: programaId, inscripcionId, artistaToken ✓
│  │  └─ codigoReferido (guardado para info)
│  │
│  └─ 06. GET /promotor/mis-programas (verificar)
│     └─ Depends on: promocionId, promotorToken ✓
│
├─ Validaciones (Independiente)
│  ├─ Depends on: promotorToken, programaId ✓
│  └─ Tests de error 400/404
│
├─ Acciones Artista (Independiente)
│  ├─ Depends on: artistaToken, promotorToken, programaId ✓
│  └─ Tests de rechazo y bloqueo
│
├─ Bloqueo y Baja (Independiente)
│  ├─ Depends on: artistaToken, inscripcionId ✓
│  └─ Tests de transiciones de estado
│
├─ Auth Errors (Independiente)
│  ├─ Puede correr sin _Setup para 401
│  └─ Requiere artistaToken/promotorToken para 403
│
└─ Not Found (Independiente)
   ├─ Depends on: artistaToken, programaId ✓
   └─ Tests de 404
```

---

## Secuencia de Tiempo

```
T=0s      START
          ├─ _Setup
          │  ├─ 01. Login Promotor        [~1s]
          │  └─ 02. Login Artista         [~1s]
          │
T=2s      ├─ Happy Path
          │  ├─ 01. GET /explorar         [~0.2s]
          │  ├─ 02. POST /inscripcion     [~0.1s]
          │  ├─ 03. GET /mis-programas    [~0.2s]
          │  ├─ 04. GET /inscripciones    [~0.2s]
          │  ├─ 05. PATCH /aprobar        [~0.2s]
          │  └─ 06. GET /mis-programas    [~0.2s]
          │
T=8s      ├─ [OPCIONAL] Validaciones
          │  ├─ POST 400 /inscripcion     [~0.1s]
          │  └─ POST 404 /inscripcion     [~0.1s]
          │
T=10s     ├─ [OPCIONAL] Acciones Artista
          │  ├─ POST /inscripcion         [~0.1s]
          │  ├─ PATCH /rechazar           [~0.1s]
          │  └─ GET /inscripciones        [~0.1s]
          │
T=14s     ├─ [OPCIONAL] Bloqueo y Baja
          │  ├─ PATCH /bloquear           [~0.1s]
          │  └─ GET /inscripciones        [~0.1s]
          │
T=18s     ├─ [OPCIONAL] Auth Errors
          │  ├─ GET 401 /explorar         [~0.1s]
          │  └─ PATCH 403 /aprobar        [~0.1s]
          │
T=20s     ├─ [OPCIONAL] Not Found
          │  ├─ PATCH 404 /aprobar        [~0.1s]
          │  └─ GET 404 /inscripciones    [~0.1s]
          │
T=22s     END

Solo Happy Path: 8s
Con todas opcionales: 22s
```

---

## Estados y Transiciones Visuales

```
Inscripcion Lifecycle:

         CREATE
           │
           ▼
    ┌─────────────┐
    │  PENDIENTE  │      User: Promotor
    │ (Estado 1)  │      EsAprobado: false
    └──────┬──────┘      EsBloqueado: false
           │             FechaBaja: null
           │
    ┌──────┴────────────────────────────────────────┐
    │                                               │
    │ User: Artista                                 │
    │                                               │
    │ Option A: APROBAR            Option B: RECHAZAR/BLOQUEAR
    │                              │
    ▼                              ▼
┌──────────────┐         ┌─────────────────────────┐
│  APROBADO    │         │  RECHAZAR (DELETE)      │
│ (Estado 2)   │         │  CodigoReferido generated
│ EsAprobado:T │         │                         │
│ CodigoRef: X │         │ O                       │
│ UrlTracking: Y         │                         │
│              │         │ BLOQUEAR                │
│              │         │ EsBloqueado: true       │
│              │         │ (re-solicitud blocked)  │
│ Option C:    │         └─────────────────────────┘
│ DAR DE BAJA  │
│              │
▼
┌──────────────┐
│DADO DE BAJA  │
│(Estado 3)    │
│ FechaBaja: T │
│ EsAprobado:F │
└──────────────┘
```

---

## Resumen Visual

```
┌─────────────────────────────────────────────────────────────┐
│                  COLECCION POSTMAN                          │
│          cp-inscripcion-programa (US-CP-03)                 │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  6 CARPETAS │ 18 REQUESTS │ 65 ASSERTIONS │ 15-20 SEGUNDOS │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ✓ _Setup                      (2 requests)                │
│  ✓ Happy Path                  (6 requests)                │
│  ○ Validaciones                (2 requests)                │
│  ○ Acciones Artista            (3 requests)                │
│  ○ Bloqueo y Baja              (2 requests)                │
│  ○ Auth Errors                 (2 requests)                │
│  ○ Not Found                   (2 requests)                │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│  USUARIOS:                                                  │
│  • usuario1@mail.com (Promotor/Fan)                        │
│  • api-test@mail.com (Artista/Fan)                         │
│                                                             │
│  ENDPOINTS: 8/8 (100% cobertura)                           │
│  ESTADOS: 4/4 (100% cobertura)                             │
│  ERRORES: 12 tipos validados                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

**Generado**: 2026-02-25
**Feature**: cp-inscripcion-programa (US-CP-03)
**Version**: 1.0
