# Estrategia de Testing: cp-inscripcion-programa (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/admin
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura Estimada |
|------|----------|--------------------|
| Unit Tests | 24 | ~85% logica pura |
| Integration Tests | 22 | ~80% flujos UI |
| Total | 46 | 80%+ |

Los tests de integracion cubren los cuatro flujos criticos del artista: aprobar, rechazar, bloquear y dar de baja. Los tests unitarios cubren los componentes de presentacion, el service y los hooks de query.

---

## 2. Estructura de Tests

```
src/admin/src/
├── __mocks__/
│   └── inscripciones.mock.ts                       <- Fixtures de InscripcionListItem, InscripcionEstado
│
├── app/(dashboard)/crowdpromotion/
│   └── programas/[id]/
│       └── components/
│           ├── inscripciones/                      <- Nuevos componentes a crear
│           │   ├── InscripcionesPendientesTab.tsx
│           │   ├── PromotoresAprobadosTab.tsx
│           │   ├── AprobarInscripcionButton.tsx
│           │   ├── RechazarInscripcionDialog.tsx
│           │   ├── BloquearInscripcionDialog.tsx
│           │   ├── DarDeBajaInscripcionDialog.tsx
│           │   └── InscripcionEstadoBadge.tsx
│           └── __tests__/
│               ├── inscripciones/
│               │   ├── InscripcionesPendientesTab.test.tsx
│               │   ├── PromotoresAprobadosTab.test.tsx
│               │   ├── AprobarInscripcionButton.test.tsx
│               │   ├── RechazarInscripcionDialog.test.tsx
│               │   ├── BloquearInscripcionDialog.test.tsx
│               │   ├── DarDeBajaInscripcionDialog.test.tsx
│               │   └── InscripcionEstadoBadge.test.tsx
│
├── hooks/
│   ├── use-inscripciones.ts                        <- useInscripciones (query paginada)
│   ├── use-inscripciones-mutations.ts              <- useAprobarInscripcion, useRechazarInscripcion, etc.
│   └── __tests__/
│       ├── use-inscripciones.test.ts
│       └── use-inscripciones-mutations.test.ts
│
└── services/
    ├── inscripciones.service.ts                    <- Calls a /api/crowdpromotion/programas/{id}/inscripciones
    └── __tests__/
        └── inscripciones.service.test.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/inscripciones.mock.ts`

Importa desde `@shared/types`: `InscripcionListItem`, `InscripcionEstado`, `InscripcionAprobada`, `InscripcionRechazada`, `InscripcionBloqueada`, `InscripcionDadaDeBaja`, `InscripcionesListResponse`.

```typescript
// Inscripcion en estado Pendiente
export const mockInscripcionPendiente: InscripcionListItem = {
    id: "inscripcion-1",
    promotorId: "promotor-1",
    promotorNombre: "DJ Marketing Pro",
    tipoPromotorNombre: "Influencer",
    promotorEmailContacto: "contacto@djmarketing.com",
    promotorUrlInstagram: "https://instagram.com/djmarketing",
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: "https://djmarketing.com",
    esAprobado: false,
    esBloqueado: false,
    codigoReferido: null,
    fechaAlta: "2026-03-05T14:00:00Z",
    fechaBaja: null,
    estado: "Pendiente",
}

// Inscripcion en estado Aprobado (con codigo referido)
export const mockInscripcionAprobada: InscripcionListItem = {
    id: "inscripcion-2",
    promotorId: "promotor-2",
    promotorNombre: "MusicBlog.es",
    tipoPromotorNombre: "Medio / Blog",
    promotorEmailContacto: "info@musicblog.es",
    promotorUrlInstagram: null,
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: "https://musicblog.es",
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: "album-2026-m3k2n",
    fechaAlta: "2026-03-06T09:00:00Z",
    fechaBaja: null,
    estado: "Aprobado",
}

// Inscripcion en estado Bloqueado
export const mockInscripcionBloqueada: InscripcionListItem = {
    id: "inscripcion-3",
    promotorId: "promotor-3",
    promotorNombre: "Spammer123",
    tipoPromotorNombre: "Fan Embajador",
    promotorEmailContacto: null,
    promotorUrlInstagram: null,
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: null,
    esAprobado: false,
    esBloqueado: true,
    codigoReferido: null,
    fechaAlta: "2026-03-07T10:00:00Z",
    fechaBaja: null,
    estado: "Bloqueado",
}

// Inscripcion en estado DadoDeBaja
export const mockInscripcionDadaDeBaja: InscripcionListItem = {
    id: "inscripcion-4",
    promotorId: "promotor-4",
    promotorNombre: "Ex Promotor",
    tipoPromotorNombre: "Influencer",
    promotorEmailContacto: "expromotor@example.com",
    promotorUrlInstagram: null,
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: null,
    esAprobado: false,
    esBloqueado: false,
    codigoReferido: null,
    fechaAlta: "2026-02-01T10:00:00Z",
    fechaBaja: "2026-04-01T12:00:00Z",
    estado: "DadoDeBaja",
}

// Respuesta paginada con mezcla de estados
export const mockInscripcionesListResponse: InscripcionesListResponse = {
    items: [mockInscripcionPendiente, mockInscripcionAprobada],
    totalCount: 2,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

// Solo pendientes
export const mockInscripcionesPendientesResponse: InscripcionesListResponse = {
    items: [mockInscripcionPendiente],
    totalCount: 1,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

// Solo aprobadas
export const mockInscripcionesAprobadasResponse: InscripcionesListResponse = {
    items: [mockInscripcionAprobada],
    totalCount: 1,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

// Lista vacia
export const mockInscripcionesVaciaResponse: InscripcionesListResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
}

// Resultados de mutaciones
export const mockAprobadaResult: InscripcionAprobada = {
    id: "inscripcion-1",
    promotorNombre: "DJ Marketing Pro",
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: "album-2026-x7k9m",
    urlTrackingPersonalizada: "https://weplay.com/campanias/mi-album?utm_source=weplay&utm_medium=referral&utm_campaign=album-2026&ref=album-2026-x7k9m",
}

export const mockRechazadaResult: InscripcionRechazada = {
    inscripcionId: "inscripcion-1",
    promotorNombre: "Spammer123",
}

export const mockBloqueadaResult: InscripcionBloqueada = {
    id: "inscripcion-1",
    promotorNombre: "Spammer123",
    esBloqueado: true,
    esAprobado: false,
}

export const mockDadaDeBajaResult: InscripcionDadaDeBaja = {
    id: "inscripcion-2",
    promotorNombre: "MusicBlog.es",
    esAprobado: false,
    fechaBaja: "2026-04-01T12:00:00Z",
}
```

### 3.2 Mocking del Service

Los tests de componentes y hooks mockean `inscripcionesService` directamente con `vi.mock`. El patron es identico al empleado en `promo-programa.service.test.ts` del proyecto.

```typescript
vi.mock("@/services/inscripciones.service", () => ({
    inscripcionesService: {
        getInscripciones: vi.fn(),
        aprobar: vi.fn(),
        rechazar: vi.fn(),
        bloquear: vi.fn(),
        darDeBaja: vi.fn(),
    },
}))
```

### 3.3 Mocking de Sonner (Toast)

Todos los tests de mutaciones y dialogs destructivos deben mockear `sonner`:

```typescript
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))
```

### 3.4 Mocking de Next.js Router

Para cualquier componente que use `useRouter` de Next.js:

```typescript
vi.mock("next/navigation", () => ({
    useRouter: () => ({ push: vi.fn(), refresh: vi.fn() }),
    useParams: () => ({ id: "programa-1" }),
}))
```

### 3.5 Test Utilities

El proyecto ya dispone de `src/admin/src/test-utils.tsx` con `renderWithProviders` (alias `render`). Se reutiliza sin modificaciones. Para hooks se usa `createWrapper()` local como en `use-promo-programas.test.ts`.

---

## 4. Tests por Modulo

### 4.1 Service

#### inscripciones.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/inscripciones.service.test.ts`

El service encapsula todas las llamadas HTTP a los endpoints de gestion. Mockea `apiFetch` directamente.

**Setup:**
```typescript
vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: { interceptors: { request: { use: vi.fn() }, response: { use: vi.fn() } } },
}))
```

| Test Case | Suite | Descripcion |
|-----------|-------|-------------|
| returns list on success | getInscripciones | Retorna `InscripcionesListResponse` correctamente |
| calls correct URL with programaId | getInscripciones | Usa `API_ROUTES.crowdpromotion.programas.inscripciones(programaId)` |
| appends estado filter when provided | getInscripciones | URL contiene `estado=Pendiente` cuando se filtra |
| appends page and pageSize when provided | getInscripciones | URL contiene `page=2&pageSize=10` |
| returns empty list when no results | getInscripciones | `items` es array vacio, `totalCount` es 0 |
| throws on error response | getInscripciones | Propaga error cuando `messages[0].errorCode` es 4026 |
| sends PATCH to correct URL | aprobar | Usa `.../inscripciones/{id}/aprobar` con `method: PATCH` |
| returns InscripcionAprobada on success | aprobar | Datos de `mockAprobadaResult` correctos |
| throws on error response (4025) | aprobar | Propaga error cuando inscripcion no esta pendiente |
| propagates network error | aprobar | Relanza `Error("Network error")` |
| sends PATCH to correct URL | rechazar | Usa `.../inscripciones/{id}/rechazar` |
| returns InscripcionRechazada on success | rechazar | Contiene `inscripcionId` y `promotorNombre` |
| throws on error response | rechazar | Propaga error en respuesta con `errorCode` de error |
| sends PATCH to correct URL | bloquear | Usa `.../inscripciones/{id}/bloquear` |
| returns InscripcionBloqueada on success | bloquear | `esBloqueado: true` en resultado |
| throws if already blocked (4025) | bloquear | Error cuando ya estaba bloqueado |
| sends PATCH to correct URL | darDeBaja | Usa `.../inscripciones/{id}/dar-de-baja` |
| returns InscripcionDadaDeBaja on success | darDeBaja | `esAprobado: false` y `fechaBaja` presente |
| throws if not in aprobado state (4025) | darDeBaja | Error cuando inscripcion no estaba aprobada |

**Total service:** 18 tests unitarios

---

### 4.2 Hooks de Query

#### use-inscripciones.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-inscripciones.test.ts`

Testea `useInscripciones(programaId, filters?)` - hook de React Query que llama a `inscripcionesService.getInscripciones`.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| returns paginated data on success | Unit | `items.length` y `totalCount` correctos |
| passes programaId to service | Unit | Service llamado con el `programaId` correcto |
| passes estado filter to service | Unit | Con `filters.estado = "Pendiente"`, service recibe parametro |
| passes page and pageSize to service | Unit | Parametros de paginacion propagados |
| handles loading state | Unit | `isLoading: true` antes de resolver |
| handles error state | Unit | `isError: true` cuando service rechaza |
| does not fetch when programaId is empty | Unit | `fetchStatus: "idle"`, service no llamado |
| does not retry on error | Unit | `getInscripciones` llamado solo una vez |

**Total hook query:** 8 tests unitarios

---

### 4.3 Hooks de Mutation

#### use-inscripciones-mutations.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-inscripciones-mutations.test.ts`

Testea cuatro hooks de mutacion: `useAprobarInscripcion`, `useRechazarInscripcion`, `useBloquearInscripcion`, `useDarDeBajaInscripcion`. Cada uno invalida la query `inscripciones` del programa al completarse con exito.

**Setup adicional:** Mock de `sonner` para verificar toasts.

| Test Case | Suite | Descripcion |
|-----------|-------|-------------|
| calls aprobar with programaId and inscripcionId | useAprobarInscripcion | Service recibe ambos IDs |
| returns InscripcionAprobada on success | useAprobarInscripcion | `data.codigoReferido` presente |
| shows success toast on success | useAprobarInscripcion | `toast.success` llamado con mensaje "Promotor aprobado" |
| shows error toast on failure | useAprobarInscripcion | `toast.error` llamado cuando service rechaza |
| invalidates inscripciones query on success | useAprobarInscripcion | Query key `['crowdpromotion', 'programas', programaId, 'inscripciones']` invalidada |
| handles error from service | useAprobarInscripcion | `isError: true` en estado del hook |
| calls rechazar with programaId and inscripcionId | useRechazarInscripcion | Service recibe ambos IDs |
| returns InscripcionRechazada on success | useRechazarInscripcion | Contiene `inscripcionId` |
| shows success toast on success | useRechazarInscripcion | `toast.success` con "Solicitud rechazada" |
| shows error toast on failure | useRechazarInscripcion | `toast.error` cuando falla |
| invalidates inscripciones query on success | useRechazarInscripcion | Query key invalidada |
| calls bloquear with programaId and inscripcionId | useBloquearInscripcion | Service recibe ambos IDs |
| returns InscripcionBloqueada with esBloqueado true | useBloquearInscripcion | `data.esBloqueado: true` |
| shows success toast on success | useBloquearInscripcion | `toast.success` con "Promotor bloqueado" |
| shows error toast on failure | useBloquearInscripcion | `toast.error` cuando falla |
| invalidates inscripciones query on success | useBloquearInscripcion | Query key invalidada |
| calls darDeBaja with programaId and inscripcionId | useDarDeBajaInscripcion | Service recibe ambos IDs |
| returns InscripcionDadaDeBaja with fechaBaja | useDarDeBajaInscripcion | `data.fechaBaja` presente |
| shows success toast on success | useDarDeBajaInscripcion | `toast.success` con "Promotor dado de baja" |
| shows error toast on failure | useDarDeBajaInscripcion | `toast.error` cuando falla |
| invalidates inscripciones query on success | useDarDeBajaInscripcion | Query key invalidada |
| handles error from service | useDarDeBajaInscripcion | `isError: true` |

**Total hooks mutation:** 22 tests (mayoritariamente unitarios con verificacion de efectos secundarios)

---

### 4.4 Componentes de Presentacion

#### InscripcionEstadoBadge.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/InscripcionEstadoBadge.test.tsx`

Componente que recibe `estado: InscripcionEstado` y renderiza un badge con el color y texto correspondiente.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders "Pendiente" badge with yellow color | Unit | Badge amarillo para estado Pendiente |
| renders "Aprobado" badge with green color | Unit | Badge verde para estado Aprobado |
| renders "Bloqueado" badge with red color | Unit | Badge rojo para estado Bloqueado |
| renders "DadoDeBaja" badge with gray color | Unit | Badge gris para estado DadoDeBaja |

**Total:** 4 tests unitarios

---

#### InscripcionesPendientesTab.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/InscripcionesPendientesTab.test.tsx`

Tabla de solicitudes pendientes. Recibe `programaId` y usa `useInscripciones(programaId, { estado: "Pendiente" })` internamente. Muestra nombre, tipo, redes sociales y botones Aprobar / Rechazar / Bloquear por fila.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders promotor name in table row | Integration | `promotorNombre` visible en la tabla |
| renders tipo promotor in row | Integration | `tipoPromotorNombre` visible |
| renders email contact when present | Integration | Email de contacto visible |
| renders Instagram link when present | Integration | Link a Instagram como enlace |
| renders empty state when no pending inscripciones | Integration | Mensaje "No hay solicitudes pendientes" |
| renders loading skeleton | Integration | Skeletons visibles durante `isLoading` |
| renders error state on fetch failure | Integration | Mensaje de error visible |
| renders Aprobar button for each row | Integration | Boton Aprobar visible por fila pendiente |
| renders Rechazar button for each row | Integration | Boton Rechazar visible por fila pendiente |
| renders Bloquear button for each row | Integration | Boton Bloquear visible por fila pendiente |

**Total:** 10 tests de integracion

---

#### PromotoresAprobadosTab.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/PromotoresAprobadosTab.test.tsx`

Tabla de promotores aprobados. Usa `useInscripciones(programaId, { estado: "Aprobado" })`. Muestra nombre, tipo, codigo referido y boton Dar de Baja.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders promotor name in row | Integration | Nombre del promotor visible |
| renders codigoReferido when present | Integration | Codigo referido en la fila |
| renders "Dar de baja" button per row | Integration | Boton de baja visible por fila |
| renders empty state when no approved promotores | Integration | Mensaje "No hay promotores aprobados" |
| renders loading skeleton | Integration | Skeletons visibles durante `isLoading` |

**Total:** 5 tests de integracion

---

### 4.5 Dialogs de Acciones Destructivas

#### AprobarInscripcionButton.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/AprobarInscripcionButton.test.tsx`

Boton de aprobacion directo (sin dialogo de confirmacion). Recibe `programaId`, `inscripcionId`, `promotorNombre`. Al clicar llama directamente a `useAprobarInscripcion`.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders "Aprobar" button | Unit | Texto "Aprobar" visible |
| calls aprobar mutation when clicked | Integration | Mutation invocada con `programaId` e `inscripcionId` correctos |
| disables button while mutation is pending | Integration | Boton deshabilitado o con spinner durante `isPending` |
| shows success toast after approval | Integration | `toast.success` mostrado al resolverse |
| shows error toast when mutation fails | Integration | `toast.error` mostrado si la mutation rechaza |

**Total:** 5 tests de integracion

---

#### RechazarInscripcionDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/RechazarInscripcionDialog.test.tsx`

Dialog de confirmacion para rechazar una solicitud pendiente. Accion destructiva: elimina fisicamente el registro.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders dialog title "Rechazar solicitud" | Unit | Titulo correcto |
| shows promotor name in description | Unit | Nombre del promotor visible en el mensaje de confirmacion |
| renders Cancelar and Confirmar buttons | Unit | Ambos botones presentes |
| does not render when open is false | Unit | Dialog no visible cuando `open={false}` |
| calls onOpenChange(false) when Cancelar clicked | Integration | Cierra el dialog sin ejecutar mutation |
| calls rechazar mutation when confirmed | Integration | Mutation llamada con IDs correctos al confirmar |
| closes dialog after successful mutation | Integration | `onOpenChange(false)` llamado tras exito |
| disables Confirmar button while mutation pending | Integration | Boton deshabilitado durante `isPending` |

**Total:** 8 tests (4 unitarios + 4 integracion)

---

#### BloquearInscripcionDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/BloquearInscripcionDialog.test.tsx`

Dialog de confirmacion para bloquear un promotor. Funciona tanto desde pendiente como desde aprobado. El dialog debe comunicar claramente que el bloqueo es permanente (el promotor no puede re-solicitar).

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders dialog title "Bloquear promotor" | Unit | Titulo correcto |
| shows promotor name in description | Unit | Nombre del promotor en el cuerpo del dialog |
| shows warning about permanent block | Unit | Texto sobre no poder re-solicitar visible |
| renders Cancelar and Bloquear buttons | Unit | Ambos botones presentes |
| does not render when open is false | Unit | Dialog no visible cuando `open={false}` |
| calls onOpenChange(false) when Cancelar clicked | Integration | Cierra sin ejecutar mutation |
| calls bloquear mutation when confirmed | Integration | Mutation llamada con IDs correctos |
| closes dialog after successful mutation | Integration | `onOpenChange(false)` llamado tras exito |
| disables Bloquear button while mutation pending | Integration | Boton deshabilitado durante `isPending` |

**Total:** 9 tests (5 unitarios + 4 integracion)

---

#### DarDeBajaInscripcionDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/inscripciones/DarDeBajaInscripcionDialog.test.tsx`

Dialog de confirmacion para dar de baja a un promotor aprobado. El registro persiste (soft delete: `FechaBaja = now`, `EsAprobado = false`). El codigo referido queda desactivado.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders dialog title "Dar de baja" | Unit | Titulo correcto |
| shows promotor name in description | Unit | Nombre visible en cuerpo del dialog |
| shows info about codigo referido deactivation | Unit | Texto sobre desactivacion del codigo visible |
| renders Cancelar and Confirmar buttons | Unit | Ambos botones presentes |
| does not render when open is false | Unit | Dialog no visible cuando `open={false}` |
| calls onOpenChange(false) when Cancelar clicked | Integration | Cierra sin ejecutar mutation |
| calls darDeBaja mutation when confirmed | Integration | Mutation llamada con IDs correctos |
| closes dialog after successful mutation | Integration | `onOpenChange(false)` llamado tras exito |
| disables Confirmar button while mutation pending | Integration | Boton deshabilitado durante `isPending` |

**Total:** 9 tests (5 unitarios + 4 integracion)

---

## 5. Flujos de Integracion Criticos

Estos flujos representan los casos de uso principales del artista. Son los mas importantes del plan y deben pasar en CI.

### Flujo 1: Aprobar solicitud pendiente

**Componente orquestador:** `InscripcionesPendientesTab` + `AprobarInscripcionButton`

```
Dado: Tab muestra una inscripcion con estado "Pendiente"
Cuando: Artista hace click en "Aprobar"
Entonces:
  - inscripcionesService.aprobar(programaId, inscripcionId) es llamado
  - toast.success("Promotor aprobado") es mostrado
  - La query inscripciones del programa es invalidada
  - La fila desaparece del tab de pendientes en re-fetch
```

**Cubierto por:** `AprobarInscripcionButton.test.tsx` - tests de integracion

### Flujo 2: Rechazar solicitud con confirmacion

**Componente orquestador:** `InscripcionesPendientesTab` + `RechazarInscripcionDialog`

```
Dado: Tab muestra una inscripcion pendiente
Cuando: Artista hace click en "Rechazar"
Entonces: Dialog de confirmacion se abre con nombre del promotor
Cuando: Artista confirma en el dialog
Entonces:
  - inscripcionesService.rechazar(programaId, inscripcionId) es llamado
  - toast.success mostrado
  - Dialog se cierra
  - Query invalidada
Cuando: Artista cancela en el dialog
Entonces:
  - Service NO es llamado
  - Dialog se cierra
```

**Cubierto por:** `RechazarInscripcionDialog.test.tsx` - tests de integracion

### Flujo 3: Bloquear promotor con confirmacion

**Componente orquestador:** `InscripcionesPendientesTab` + `BloquearInscripcionDialog`

```
Dado: Tab muestra una inscripcion pendiente o aprobada
Cuando: Artista hace click en "Bloquear"
Entonces: Dialog se abre con warning de bloqueo permanente
Cuando: Artista confirma
Entonces:
  - inscripcionesService.bloquear(programaId, inscripcionId) es llamado
  - toast.success mostrado
  - Dialog se cierra
  - Query invalidada
```

**Cubierto por:** `BloquearInscripcionDialog.test.tsx` - tests de integracion

### Flujo 4: Dar de baja a promotor aprobado

**Componente orquestador:** `PromotoresAprobadosTab` + `DarDeBajaInscripcionDialog`

```
Dado: Tab de aprobados muestra un promotor con codigoReferido
Cuando: Artista hace click en "Dar de baja"
Entonces: Dialog se abre explicando que el codigo referido quedara desactivado
Cuando: Artista confirma
Entonces:
  - inscripcionesService.darDeBaja(programaId, inscripcionId) es llamado
  - toast.success("Promotor dado de baja") mostrado
  - Dialog se cierra
  - Query invalidada
```

**Cubierto por:** `DarDeBajaInscripcionDialog.test.tsx` - tests de integracion

---

## 6. Cobertura por Archivo

| Archivo | Lineas Objetivo | Funciones | Branches |
|---------|-----------------|-----------|----------|
| `inscripciones.service.ts` | 95% | 100% | 90% |
| `use-inscripciones.ts` | 90% | 100% | 85% |
| `use-inscripciones-mutations.ts` | 90% | 100% | 85% |
| `InscripcionEstadoBadge.tsx` | 100% | 100% | 100% |
| `InscripcionesPendientesTab.tsx` | 85% | 90% | 80% |
| `PromotoresAprobadosTab.tsx` | 85% | 90% | 80% |
| `AprobarInscripcionButton.tsx` | 85% | 90% | 80% |
| `RechazarInscripcionDialog.tsx` | 85% | 90% | 80% |
| `BloquearInscripcionDialog.tsx` | 85% | 90% | 80% |
| `DarDeBajaInscripcionDialog.tsx` | 85% | 90% | 80% |

**Meta Global:** 80% en todas las metricas

---

## 7. Casos Edge y Error Handling

Los siguientes casos deben estar cubiertos en los tests de integracion de dialogs y tabs:

| Caso | Componente | Comportamiento Esperado |
|------|------------|------------------------|
| Artista aprueba inscripcion que ya esta aprobada (error 4025) | AprobarInscripcionButton | `toast.error` con mensaje del servidor |
| Artista bloquea promotor ya bloqueado (error 4025) | BloquearInscripcionDialog | `toast.error` con mensaje del servidor |
| Artista da de baja a inscripcion no aprobada (error 4025) | DarDeBajaInscripcionDialog | `toast.error` con mensaje del servidor |
| Error de red al cargar inscripciones | InscripcionesPendientesTab | Estado de error visible con opcion de reintentar |
| Tab pendientes con lista vacia | InscripcionesPendientesTab | Empty state: "No hay solicitudes pendientes" |
| Tab aprobados con lista vacia | PromotoresAprobadosTab | Empty state: "No hay promotores aprobados" |
| Promotor sin redes sociales | InscripcionesPendientesTab | Columnas de redes muestran guion o estan vacias |
| InscripcionEstadoBadge con todos los estados | InscripcionEstadoBadge | Cada estado renderiza el badge correcto |

---

## 8. Notas de Implementacion para los Tests

### Patron de Mock de Service en Hooks

Seguir exactamente el patron de `use-promo-programas.test.ts`:
- `vi.mock` a nivel de modulo para el service completo
- `createWrapper()` local con `QueryClient(retry: false)`
- `vi.clearAllMocks()` en `beforeEach`

### Patron de Mock de Service en Dialogs

Seguir el patron de `DesactivarPromoProgramaDialog.test.tsx`:
- Mock del service completo al nivel del archivo de test
- Mock de `sonner` para verificar toasts
- Props `open={true}` por defecto en `defaultProps`
- Verificar que el service NO es llamado cuando se cancela

### Query Keys a Invalidar

Los hooks de mutation deben invalidar:
```typescript
['crowdpromotion', 'programas', programaId, 'inscripciones', filters]
```

Esta key debe coincidir con la definida en `QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId)`.

### Estructura del Test de Service

Seguir el patron de `promo-programa.service.test.ts`:
- Una `describe` por metodo del service
- Test de URL correcta (verificar `apiFetch` fue llamado con la URL esperada)
- Test de retorno exitoso con los datos del mock
- Test de error: cuando `messages[0].errorCode` no empieza por `0`, el service debe lanzar un error
- Test de error de red: `apiFetch` rechaza y el service propaga

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del admin
cd src/admin && npm run test

# Ejecutar con coverage
cd src/admin && npm run test:coverage

# Ejecutar solo tests de la feature de inscripciones
cd src/admin && npm run test -- --reporter=verbose inscripciones

# Watch mode durante desarrollo
cd src/admin && npm run test:watch
```

---

## 10. CI/CD Integration

Los tests de esta feature se ejecutan junto con el resto del admin en el pipeline existente:

```yaml
- name: Run Admin Tests
  run: |
    cd src/admin
    npm run test:coverage

- name: Check Coverage Threshold
  run: |
    # Verificar que coverage >= 80%
    cd src/admin
    npm run test:coverage -- --coverage.thresholds.lines=80
```

---

## 11. Checklist de Completitud

- [ ] Fixtures definidos en `src/admin/src/__mocks__/inscripciones.mock.ts`
- [ ] Tests del service: `inscripciones.service.test.ts` (18 tests)
- [ ] Tests del hook de query: `use-inscripciones.test.ts` (8 tests)
- [ ] Tests de hooks de mutation: `use-inscripciones-mutations.test.ts` (22 tests)
- [ ] Tests del badge: `InscripcionEstadoBadge.test.tsx` (4 tests)
- [ ] Tests del tab pendientes: `InscripcionesPendientesTab.test.tsx` (10 tests)
- [ ] Tests del tab aprobados: `PromotoresAprobadosTab.test.tsx` (5 tests)
- [ ] Tests del boton aprobar: `AprobarInscripcionButton.test.tsx` (5 tests)
- [ ] Tests del dialog rechazar: `RechazarInscripcionDialog.test.tsx` (8 tests)
- [ ] Tests del dialog bloquear: `BloquearInscripcionDialog.test.tsx` (9 tests)
- [ ] Tests del dialog dar de baja: `DarDeBajaInscripcionDialog.test.tsx` (9 tests)
- [ ] Cobertura >= 80% verificada en CI
- [ ] Todos los tests pasan en < 60 segundos
