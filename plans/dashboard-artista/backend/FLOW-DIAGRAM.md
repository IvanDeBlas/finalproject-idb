# Flow Diagram - Dashboard Artista Tests

Visualización del flujo de ejecución de tests.

---

## Diagrama de Ejecución Principal

```
┌─────────────────────────────────────────────────────────────────┐
│         Newman Test Execution Flow - Dashboard Artista           │
└─────────────────────────────────────────────────────────────────┘

                              INICIO
                                │
                                ▼
                    ┌───────────────────────┐
                    │  _Setup               │
                    │ (Autenticacion)       │
                    └───────────────────────┘
                                │
                    POST /api/auth/login
                                │
                    ┌─────────────────────────┐
                    │ Response 200 OK         │
                    │ {                       │
                    │   accessToken: "jwt..." │
                    │ }                       │
                    └─────────────────────────┘
                                │
                    authToken = "jwt..."
                                │
                                ▼
                    ┌───────────────────────────┐
                    │  Dashboard - Resumen      │
                    │ (Metricas generales)      │
                    └───────────────────────────┘
                                │
                ┌───────────────┴───────────────┐
                ▼                               ▼
    ┌─────────────────────┐       ┌──────────────────┐
    │ GET /api/dashboard/ │       │  GET (sin auth)  │
    │    resumen          │       │  (401 test)      │
    │ Status: 200 ✅      │       │  Status: 401 ✅  │
    └─────────────────────┘       └──────────────────┘
                │
                │ 6 assertions:
                │ - Status 200
                │ - Response time < 500ms
                │ - totalRecaudado (number)
                │ - totalBackers (number)
                │ - campaniasActivas (number)
                │ - moneda = "EUR"
                │
                ▼
        ┌──────────────────────┐
        │ Mis Campanias        │
        │ (Listado de campanias│
        └──────────────────────┘
                │
        ┌───────┴────────┬──────────┐
        ▼                ▼          ▼
    ┌────────┐       ┌────────┐ ┌──────┐
    │ GET P1 │       │ Filter │ │ 401  │
    │ 200 ✅ │       │ 200 ✅ │ │ ✅   │
    └────────┘       └────────┘ └──────┘
        │
        │ Assertions:
        │ - items[] array
        │ - totalCount number
        │ - porcentaje 0-100
        │ - Guarda campaniaId
        │
        ▼
        ┌──────────────────────────────────┐
        │ Backings por Campania            │
        │ (Listado de backings)            │
        └──────────────────────────────────┘
                │
        ┌───────┼────────┬──────────┐
        ▼       ▼        ▼          ▼
    ┌────┐  ┌────┐   ┌────┐    ┌────┐
    │ P1 │  │ P2 │   │404 │    │401 │
    │200│  │200│   │ ✅ │    │ ✅ │
    └────┘  └────┘   └────┘    └────┘
        │
        │ Assertions:
        │ - items[] array
        │ - stats object
        │ - backingPromedio number
        │ - nombreBacker/emailBacker
        │ - fechaRelativa string
        │
        ▼
        ┌──────────────────────────┐
        │ Stats de Campania        │
        │ (Estadisticas detalladas)│
        └──────────────────────────┘
                │
        ┌───────┼────────┬──────────┐
        ▼       ▼        ▼
    ┌────┐  ┌────┐   ┌────┐
    │200│  │404 │   │401 │
    │ ✅│  │ ✅ │   │ ✅ │
    └────┘  └────┘   └────┘
        │
        │ Assertions:
        │ - importeObjetivo number
        │ - porcentaje = cálculo
        │ - rewardStats[] array
        │ - progressoPorDia[] array
        │ - diasRestantes number
        │
        ▼
    ┌─────────────────────┐
    │ FIN - TODOS PASS ✅ │
    └─────────────────────┘
```

---

## Flujo de Variables

```
┌──────────────────────────────────────────────────────────────┐
│ Variable Flow - Cómo se pasan datos entre requests           │
└──────────────────────────────────────────────────────────────┘

INICIO
  │
  │ authToken = ""
  │ campaniaId = ""
  │
  ▼
┌─────────────────────┐
│ Login               │
│ POST /api/auth/login│
└─────────────────────┘
  │
  │ Response:
  │ { accessToken: "eyJ..." }
  │
  │ Script:
  │ pm.environment.set('authToken', json.accessToken)
  │
  ▼
  authToken = "eyJ0eXAiOiJKV1QiLCJhbGc..."
  │
  │ (compartido a todos los GET requests)
  │
  ├─────────────────────────────────────────────┐
  │                                             │
  ▼                                             ▼
┌──────────────────┐                   ┌──────────────────┐
│ GET /dashboard/  │                   │ GET /campanias/  │
│ resumen          │                   │ mis-campanias    │
│ (usa authToken)  │                   │ (usa authToken)  │
└──────────────────┘                   └──────────────────┘
                                          │
                                          │ Response:
                                          │ { items: [{id: "GUID1", ...}] }
                                          │
                                          │ Script:
                                          │ pm.environment.set('campaniaId',
                                          │   json.items[0].id)
                                          │
                                          ▼
                                    campaniaId = "GUID1"
                                          │
                        ┌─────────────────┴──────────────┐
                        │                               │
                        ▼                               ▼
                ┌─────────────────┐        ┌──────────────────┐
                │ GET /campanias/ │        │ GET /campanias/  │
                │ {id}/backings   │        │ {id}/stats       │
                │ (usa ambos)     │        │ (usa ambos)      │
                └─────────────────┘        └──────────────────┘
                        │                               │
                        └─────────────────┬─────────────┘
                                          │
                                          ▼
                                      FIN - Todas variables
                                      fueron utilizadas
```

---

## Árbol de Decisión por Status Code

```
                        Request
                          │
                          ▼
                    ┌──────────────┐
                    │ Ejecutar GET │
                    └──────────────┘
                          │
                          ▼
                    ┌────────────────┐
                    │ ¿Status = 200? │
                    └────────────────┘
                      /          \
                    SI            NO
                    │              │
                    ▼              ▼
            ┌─────────────┐    ┌──────────────┐
            │ Validar     │    │ ¿Status=401? │
            │ - Fields    │    └──────────────┘
            │ - Types     │      /        \
            │ - Business  │    SI         NO
            │ - Perf      │    │           │
            │ PASS ✅     │    ▼           ▼
            └─────────────┘ ┌──────┐   ┌──────────┐
                           │PASS ✅│ │¿404 o 5xx?│
                           └──────┘ └──────────┘
                                      /       \
                                    SI        NO
                                    │          │
                                    ▼          ▼
                               ┌────────┐  ┌────────┐
                               │PASS ✅ │  │FAIL ❌ │
                               └────────┘  └────────┘
```

---

## Timeline de Ejecución

```
Timeline - Orden de Requests (13 principales)
═══════════════════════════════════════════════════════════════

Tiempo    Request                              Duración   Status
─────────────────────────────────────────────────────────────────
0ms      [_Setup] Login                       ~200ms      200 ✅
200ms    [Dashboard] Resumen (200)            ~150ms      200 ✅
350ms    [Dashboard] Resumen (401)            ~100ms      401 ✅
450ms    [Mis Campanias] P1                   ~250ms      200 ✅
700ms    [Mis Campanias] Filter               ~200ms      200 ✅
900ms    [Mis Campanias] 401                  ~100ms      401 ✅
1000ms   [Backings] P1                        ~300ms      200 ✅
1300ms   [Backings] P2                        ~250ms      200 ✅
1550ms   [Backings] 404                       ~100ms      404 ✅
1650ms   [Backings] 401                       ~100ms      401 ✅
1750ms   [Stats] 200                          ~350ms      200 ✅
2100ms   [Stats] 404                          ~100ms      404 ✅
2200ms   [Stats] 401                          ~100ms      401 ✅
─────────────────────────────────────────────────────────────────
2300ms   TOTAL TIME: ~2.3 segundos (local)

En ambiente real:
- Desarrollo local:  2-3 segundos
- Staging:           3-5 segundos
- Produccion:        4-6 segundos
```

---

## Árbol de Carpetas y Dependencias

```
WePlay.DashboardArtista.IntegrationTests
│
├── _Setup (REQUERIDO - va primero)
│   └── Login
│       └── Genera: authToken ✅
│
├── Dashboard - Resumen (Depende de: authToken)
│   ├── GET Resumen (200)
│   │   └── Valida: autenticacion funciona
│   │
│   └── GET Resumen (401)
│       └── Valida: requerimiento de token
│
├── Mis Campanias (Depende de: authToken)
│   ├── GET P1 (200)
│   │   └── Genera: campaniaId ✅
│   │
│   ├── GET Filter (200)
│   │   └── Valida: filtros funcionan
│   │
│   └── GET (401)
│       └── Valida: requerimiento de token
│
├── Backings (Depende de: authToken, campaniaId)
│   ├── GET P1 (200)
│   │   └── Valida: estructura de backings
│   │
│   ├── GET P2 (200)
│   │   └── Valida: paginacion
│   │
│   ├── GET (404)
│   │   └── Valida: error handling
│   │
│   └── GET (401)
│       └── Valida: requerimiento de token
│
└── Stats (Depende de: authToken, campaniaId)
    ├── GET (200)
    │   └── Valida: estadisticas y calculos
    │
    ├── GET (404)
    │   └── Valida: error handling
    │
    └── GET (401)
        └── Valida: requerimiento de token

LEYENDA:
✅ = Genera variable usada por otros
[x] = Depende de variable anterior
```

---

## Matriz de Dependencias

```
Request                  Requiere          Genera         Status
───────────────────────────────────────────────────────────────
Login                    (ninguno)         authToken      200
GET Resumen              authToken         (ninguno)      200, 401
GET Mis Campanias P1     authToken         campaniaId     200, 401
GET Mis Campanias Filter authToken         (ninguno)      200, 401
GET Backings P1          authToken+id      (ninguno)      200, 404, 401
GET Backings P2          authToken+id      (ninguno)      200, 404, 401
GET Stats                authToken+id      (ninguno)      200, 404, 401
```

---

## Flujo de Assertions

```
REQUEST
  │
  ├─ Test 1: Status Code
  │  ├─ pm.response.to.have.status(200/401/404)
  │  └─ PASS ✅ o FAIL ❌
  │
  ├─ Test 2: Response Time
  │  ├─ pm.expect(responseTime).to.be.below(500)
  │  └─ PASS ✅ o FAIL ❌
  │
  ├─ Test 3: Structure
  │  ├─ pm.expect(json).to.have.property('field')
  │  ├─ pm.expect(field).to.be.a('type')
  │  └─ PASS ✅ o FAIL ❌
  │
  ├─ Test 4: Values
  │  ├─ pm.expect(value).to.equal('expected')
  │  ├─ pm.expect(value).to.be.at.least(0)
  │  ├─ pm.expect(value).to.be.below(100)
  │  └─ PASS ✅ o FAIL ❌
  │
  ├─ Test 5: Business Logic
  │  ├─ pm.expect(calc).to.equal(expected)
  │  ├─ pm.environment.set('variable', value)
  │  └─ PASS ✅ o FAIL ❌
  │
  └─ RESULTADO FINAL
     - Todos PASS → SIGUIENTE REQUEST
     - Alguno FAIL → REPORTE ERROR (--bail on)
```

---

## Modelo de Datos JSON

```
Request                Response Structure
─────────────────────────────────────────────────────────────
Login                  {
                         accessToken: string,
                         tokenType: string,
                         expiresIn: number
                       }

GET /resumen           {
                         totalRecaudado: number,
                         totalBackers: number,
                         campaniasActivas: number,
                         campaniasCompletadas: number,
                         moneda: string
                       }

GET /mis-campanias     {
                         items: [{
                           id: string,
                           titulo: string,
                           estado: string,
                           importeObjetivo: number,
                           importeRecaudado: number,
                           porcentaje: number,
                           numBackers: number,
                           diasRestantes: number
                         }],
                         totalCount: number
                       }

GET /backings          {
                         items: [{
                           id: string,
                           nombreBacker: string,
                           emailBacker: string,
                           monto: number,
                           esAnonimo: boolean,
                           fechaRelativa: string
                         }],
                         totalCount: number,
                         totalRecaudado: number,
                         stats: {
                           backingPromedio: number,
                           rewardMasPopular: string,
                           ultimoBacking: string
                         }
                       }

GET /stats             {
                         importeObjetivo: number,
                         importeRecaudado: number,
                         porcentaje: number,
                         numBackers: number,
                         backingPromedio: number,
                         diasRestantes: number,
                         diasTranscurridos: number,
                         proyeccionFinal: number,
                         rewardStats: [{
                           rewardNombre: string,
                           cantidad: number,
                           total: number,
                           porcentaje: number
                         }],
                         progressoPorDia: [{
                           fecha: string,
                           total: number
                         }]
                       }
```

---

## Estado Global Durante Ejecución

```
ESTADO 1: Post-Login
  ┌──────────────────────────┐
  │ Variables:               │
  │ - authToken: "eyJ..."    │
  │ - campaniaId: ""         │
  │ - baseUrl: "http://..."  │
  │ - testEmail: "user@..."  │
  │ - testPassword: "pass"   │
  └──────────────────────────┘
            │
            ▼ (Ejecutan Mis Campanias)
ESTADO 2: Post-Mis Campanias
  ┌──────────────────────────┐
  │ Variables:               │
  │ - authToken: "eyJ..."    │
  │ - campaniaId: "GUID-123" │ ← NUEVO
  │ - baseUrl: "http://..."  │
  │ - testEmail: "user@..."  │
  │ - testPassword: "pass"   │
  └──────────────────────────┘
            │
            ▼ (Ejecutan Backings/Stats)
ESTADO 3: Final
  ┌──────────────────────────┐
  │ Variables:               │
  │ - authToken: "eyJ..."    │
  │ - campaniaId: "GUID-123" │
  │ - baseUrl: "http://..."  │
  │ - testEmail: "user@..."  │
  │ - testPassword: "pass"   │
  │ - Todas acciones exitosas│
  └──────────────────────────┘
```

---

## Ciclo de Error

```
Si algún test FAIL:

Request
  │
  ▼
Status: 200 ✅

Fields: ✅
Types: ✅
Values: ❌ (ejemplo: porcentaje fuera de rango)
  │
  ▼
Test FAIL:
  "Expected 150 to be below 100"
  │
  ▼
¿--bail on? (default: YES)
  │
  ├─ SÍ: DETENER ejecucion
  │      Mostrar error en console
  │      Exit code: 1 (fallo)
  │
  └─ NO: Continuar con siguiente request
         Mostrar error pero sigue
         Exit code: 1 (fallo) al final
```

---

## Resumen Visual

```
ENTRADA                      PROCESAMIENTO                   SALIDA
═══════════════════════════════════════════════════════════════════════

postman-collection.json  →  Newman executes:             →  Console output
(18 requests)               - Login
                            - 5 Endpoints
environment.json         →  - 5 Folders
(variables)                - 45+ Assertions
                                                        →  Exit code:
                           Valida:                         - 0 (pass)
                           - Status codes                  - 1 (fail)
                           - Response times
                           - JSON structure             →  Reports:
                           - Business logic               - JSON
                           - Data types                   - HTML
                           - Calculations                 - JUnit XML
```

---

**Diagrama actualizado**: 2026-02-14
**Requests totales**: 18
**Assertions totales**: 45+
**Ejecución paralela**: No soportada (dependencias)
