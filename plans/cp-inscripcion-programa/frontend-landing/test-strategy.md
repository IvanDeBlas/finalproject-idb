# Estrategia de Testing: cp-inscripcion-programa (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-inscripcion-programa (US-CP-03)
**Target:** src/web
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 28 | ~45% |
| Integration Tests | 22 | ~45% |
| Total | 50 | 80%+ |

Los tests de integracion cubren los flujos criticos del promotor: explorar programas, solicitar inscripcion y consultar mis programas. Los unit tests cubren los componentes de presentacion puros, el service y los mappers.

---

## 2. Estructura de Tests

```
src/web/src/features/crowdpromotion/
├── __mocks__/
│   └── inscripcion.mock.ts
├── __tests__/
│   ├── components/
│   │   ├── ProgramaCard.test.tsx
│   │   ├── InscripcionEstadoBadge.test.tsx
│   │   ├── SolicitarInscripcionButton.test.tsx
│   │   ├── MisProgramasListItem.test.tsx
│   │   └── DetalleInscripcionPanel.test.tsx
│   ├── hooks/
│   │   ├── useExplorarProgramas.test.ts
│   │   ├── useSolicitarInscripcion.test.ts
│   │   └── useMisProgramas.test.ts
│   ├── services/
│   │   └── inscripcion.service.test.ts
│   └── pages/
│       ├── ExplorarProgramasPage.test.tsx
│       └── MisProgramasPage.test.tsx
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/inscripcion.mock.ts`

Debe exportar los siguientes objetos mock que replican exactamente los DTOs del contrato:

**`mockProgramaExplorarItem`** - Programa sin inscripcion del promotor (`miEstado: null`):
```typescript
{
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    titulo: 'Promociona mi nuevo album',
    artistaNombre: 'Luna Nova',
    tipoPromoId: 1,
    tipoPromoNombre: 'Referral',
    importeComisionPorcentaje: 10.00,
    importeComisionFija: null,
    monedaNombre: 'EUR',
    numeroTareas: 3,
    campaniaTitulo: 'Mi Album Debut',
    fechaInicio: '2026-03-01',
    fechaFin: '2026-06-01',
    miEstado: null,
}
```

**`mockProgramaPendiente`** - Programa con `miEstado: 'Pendiente'`

**`mockProgramaAprobado`** - Programa con `miEstado: 'Aprobado'`

**`mockProgramaBloqueado`** - Programa con `miEstado: 'Bloqueado'`

**`mockProgramasDadoDeBaja`** - Programa con `miEstado: 'DadoDeBaja'`

**`mockProgramasList`** - Array de 5 items que combina los estados anteriores

**`mockPaginatedProgramas`** - Respuesta paginada del endpoint explorar:
```typescript
{
    items: mockProgramasList,
    totalCount: 5,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}
```

**`mockInscripcionCreada`** - Respuesta del POST de solicitud (201 Created):
```typescript
{
    id: 'b2c3d4e5-f6a7-8901-bc23-de45fa678901',
    programaId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    programaTitulo: 'Promociona mi nuevo album',
    esAprobado: false,
    esBloqueado: false,
    fechaAlta: '2026-03-05T10:00:00Z',
}
```

**`mockMiPrograma_Aprobado`** - Item de mis-programas con estado Aprobado, con `codigoReferido` y `urlTrackingPersonalizada` poblados y array `tareas` con al menos 1 tarea:
```typescript
{
    id: 'b2c3d4e5-f6a7-8901-bc23-de45fa678901',
    programaId: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    programaTitulo: 'Promociona mi nuevo album',
    artistaNombre: 'Luna Nova',
    tipoPromoNombre: 'Referral',
    importeComisionPorcentaje: 10.00,
    importeComisionFija: null,
    monedaNombre: 'EUR',
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: 'album-2026-x7k9m',
    urlTrackingPersonalizada: 'https://weplay.com/campanias/mi-album?utm_source=weplay&utm_medium=referral&utm_campaign=album-2026&ref=album-2026-x7k9m',
    fechaAlta: '2026-03-05T10:00:00Z',
    fechaBaja: null,
    estado: 'Aprobado',
    tareas: [
        {
            id: 'a1b2c3d4-e5f6-7890-ab12-cd34ef567890',
            titulo: 'Comparte en Instagram Stories',
            descripcion: 'Sube una story mencionando la campana...',
            tipoEventoPromoNombre: 'Share',
            importeRecompensa: 5.00,
            monedaNombre: 'EUR',
            esRepetible: true,
            maxRepeticiones: 10,
            orden: 1,
        }
    ],
}
```

**`mockMiPrograma_Pendiente`** - Item de mis-programas con `estado: 'Pendiente'`, `codigoReferido: null`, `urlTrackingPersonalizada: null`, `tareas: []`

**`mockMiPrograma_Bloqueado`** - Item con `estado: 'Bloqueado'`

**`mockMiPrograma_DadoDeBaja`** - Item con `estado: 'DadoDeBaja'`, `fechaBaja` con valor

**`mockMisProgramasList`** - Array con los 4 items anteriores

**`mockPaginatedMisProgramas`** - Respuesta paginada de mis-programas

### 3.2 Mock del Service

Los tests usan `vi.mock` sobre el service, siguiendo el patron establecido en el proyecto (ver `campanias/__tests__/components/CampaniaRewardsSection.test.tsx`):

```typescript
vi.mock('../../infrastructure/inscripcion.service', () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        getMisProgramas: vi.fn(),
    },
}))
```

No se usa MSW. El proyecto no tiene MSW configurado; el patron existente es mockear el service directamente con `vi.mock` y `vi.fn()`.

### 3.3 Test Utilities

**Patron `createWrapper`** - Reutilizar el patron de `useRewardsByCampania.test.ts`:

```typescript
const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}
```

**`renderWithProviders`** - Para tests de componentes que necesitan QueryClient:

```typescript
function renderWithProviders(ui: React.ReactElement) {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, gcTime: 0, staleTime: 0 } },
    })
    return render(
        <QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>
    )
}
```

**Mock del auth store** - Para simular usuario autenticado en tests de paginas:

```typescript
vi.mock('@/store/auth-store', () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
        user: { id: 'user-test-001', email: 'test@test.com', nombreCompleto: 'Test User' },
        token: 'mock-jwt-token',
    })),
}))
```

**Mock de `navigator.clipboard`** - Para el test de copiar al portapapeles:

```typescript
Object.assign(navigator, {
    clipboard: { writeText: vi.fn().mockResolvedValue(undefined) },
})
```

---

## 4. Tests por Modulo

### 4.1 Services

#### inscripcion.service.test.ts

**Archivo:** `__tests__/services/inscripcion.service.test.ts`

El service encapsula las llamadas al `apiClient` (axios). Se mockea `@/lib/api-client` para aislar los tests de la red.

**Setup:**
```typescript
vi.mock('@/lib/api-client', () => ({
    apiFetch: vi.fn(),
}))
import { apiFetch } from '@/lib/api-client'
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `explorarProgramas - returns paginated list` | Unit | Llama a `GET /crowdpromotion/programas/explorar` y devuelve `PaginatedResponse<ProgramaExplorarItemDto>` |
| 2 | `explorarProgramas - sends artistaNombre filter as query param` | Unit | Verifica que el filtro `artistaNombre` se envia como query string |
| 3 | `explorarProgramas - sends tipoPromoId filter as query param` | Unit | Verifica que `tipoPromoId` se envia cuando se proporciona |
| 4 | `explorarProgramas - sends pagination params` | Unit | Verifica `page` y `pageSize` en la URL |
| 5 | `explorarProgramas - omits undefined filters` | Unit | No agrega params vacios a la query string |
| 6 | `solicitarInscripcion - returns inscripcion creada on 201` | Unit | Llama a `POST /crowdpromotion/programas/{id}/inscripcion` con body vacio y retorna el DTO |
| 7 | `solicitarInscripcion - throws on 400 (ya inscrito)` | Unit | Propaga el error cuando la API retorna 400 con errorCode 4021 |
| 8 | `solicitarInscripcion - throws on 403 (bloqueado)` | Unit | Propaga el error cuando la API retorna 403 con errorCode 4022 |
| 9 | `getMisProgramas - returns paginated inscripciones` | Unit | Llama a `GET /crowdpromotion/promotor/mis-programas` y devuelve la lista |
| 10 | `getMisProgramas - codigoReferido and url only on aprobado` | Unit | Verifica que `codigoReferido` y `urlTrackingPersonalizada` llegan correctamente para inscripciones aprobadas |

**Casos detallados:**

```markdown
1. explorarProgramas - returns paginated list
   - Arrange: apiFetch resuelve con { data: mockPaginatedProgramas, messages: [] }
   - Act: inscripcionService.explorarProgramas()
   - Assert: retorna el objeto paginado con items correctos
   - Assert: apiFetch fue llamado con URL que incluye /crowdpromotion/programas/explorar

6. solicitarInscripcion - returns inscripcion creada on 201
   - Arrange: apiFetch resuelve con { data: mockInscripcionCreada, messages: [{ message: 'Solicitud enviada al artista', errorCode: '0001' }] }
   - Act: inscripcionService.solicitarInscripcion('3fa85f64-...')
   - Assert: retorna mockInscripcionCreada
   - Assert: apiFetch fue llamado con method: 'POST' y URL /crowdpromotion/programas/3fa85f64-.../inscripcion

7. solicitarInscripcion - throws on 400 (ya inscrito)
   - Arrange: apiFetch rechaza con AxiosError { response.status: 400, response.data.messages[0].errorCode: '4021' }
   - Act: inscripcionService.solicitarInscripcion(programaId)
   - Assert: el error se propaga (expect(promise).rejects.toThrow())
```

---

### 4.2 Hooks

#### useExplorarProgramas.test.ts

**Archivo:** `__tests__/hooks/useExplorarProgramas.test.ts`

Usa `renderHook` con el wrapper `createWrapper`. Mockea `inscripcionService` con `vi.mock`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `returns paginated programs on success` | Unit | `isSuccess: true` y `data.items` con la lista mockeada |
| 2 | `isLoading true initially` | Unit | Estado de carga inicial antes de resolver |
| 3 | `isError true when service fails` | Unit | `isError: true` cuando el service rechaza |
| 4 | `passes artistaNombre filter to service` | Unit | Verifica que el filtro se pasa correctamente al service |
| 5 | `passes tipoPromoId filter to service` | Unit | Verifica que el filtro numerico se pasa al service |
| 6 | `updates when filters change` | Integration | Al cambiar los filtros el hook refetch con nuevos params |
| 7 | `does not fetch when hook is disabled` | Unit | `enabled: false` -> `isFetching: false` |

**Casos detallados:**

```markdown
4. passes artistaNombre filter to service
   - Arrange: mockear inscripcionService.explorarProgramas
   - Act: renderHook(() => useExplorarProgramas({ artistaNombre: 'Luna Nova' }), { wrapper })
   - Assert: await waitFor(() => expect(isSuccess).toBe(true))
   - Assert: inscripcionService.explorarProgramas fue llamado con { artistaNombre: 'Luna Nova' }

6. updates when filters change
   - Arrange: renderHook con filtros iniciales vacios
   - Act: rerender hook con nuevo filtro artistaNombre
   - Assert: service llamado 2 veces, segunda llamada con el filtro nuevo
```

---

#### useSolicitarInscripcion.test.ts

**Archivo:** `__tests__/hooks/useSolicitarInscripcion.test.ts`

Usa `renderHook` + `act`. El hook es una mutation que invalida las queries de explorar-programas y mis-programas tras el exito.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `mutation executes successfully` | Unit | `isSuccess: true` tras llamar a `mutate(programaId)` |
| 2 | `calls service with programaId` | Unit | Service llamado con el ID correcto |
| 3 | `invalidates explorar-programas query on success` | Integration | `queryClient.invalidateQueries` con key de explorar-programas |
| 4 | `invalidates mis-programas query on success` | Integration | `queryClient.invalidateQueries` con key de mis-programas |
| 5 | `isError true when service returns 400` | Unit | `isError: true` cuando el service rechaza con error de negocio |
| 6 | `isError true when service returns 403 (bloqueado)` | Unit | `isError: true` con error 403 |
| 7 | `isPending true during mutation` | Unit | Estado pendiente mientras la mutation no resuelve |

**Casos detallados:**

```markdown
3. invalidates explorar-programas query on success
   - Arrange: spy sobre queryClient.invalidateQueries
   - Act: renderHook, llamar mutate, await waitFor(isSuccess)
   - Assert: invalidateQueries llamado con queryKey que contiene QUERY_KEYS.EXPLORAR_PROGRAMAS

5. isError true when service returns 400
   - Arrange: inscripcionService.solicitarInscripcion rechaza con error '4021 - Ya estas inscrito'
   - Act: mutate(programaId)
   - Assert: await waitFor(() => expect(isError).toBe(true))
   - Assert: data es undefined
```

---

#### useMisProgramas.test.ts

**Archivo:** `__tests__/hooks/useMisProgramas.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `returns inscripciones list on success` | Unit | Datos correctos con todos los estados |
| 2 | `isLoading true initially` | Unit | Estado de carga inicial |
| 3 | `isError true when service fails` | Unit | Error propagado correctamente |
| 4 | `aprobado item has codigoReferido and urlTracking` | Unit | Verifica que los campos sensibles llegan en items aprobados |
| 5 | `pendiente item has null codigoReferido` | Unit | `codigoReferido: null` para pendientes |
| 6 | `returns empty items when promotor has no inscripciones` | Unit | Array vacio, `totalCount: 0` |

---

### 4.3 Components (Unit)

#### InscripcionEstadoBadge.test.tsx

**Archivo:** `__tests__/components/InscripcionEstadoBadge.test.tsx`

Componente puramente presentacional. No requiere providers.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders Pendiente badge` | Unit | Muestra texto "Pendiente" con color amarillo/warning |
| 2 | `renders Aprobado badge` | Unit | Muestra texto "Aprobado" con color verde/success |
| 3 | `renders Bloqueado badge` | Unit | Muestra texto "Bloqueado" con color rojo/destructive |
| 4 | `renders DadoDeBaja badge` | Unit | Muestra texto "Dado de baja" con color gris/muted |
| 5 | `renders null when miEstado is null` | Unit | No renderiza nada cuando el promotor no tiene inscripcion |

**Casos detallados:**

```markdown
1. renders Pendiente badge
   - Render: <InscripcionEstadoBadge estado="Pendiente" />
   - Assert: screen.getByText('Pendiente') en documento
   - Assert: badge tiene clase o variant que indica estado de espera

5. renders null when miEstado is null
   - Render: <InscripcionEstadoBadge estado={null} />
   - Assert: container.firstChild es null o no hay texto visible
```

---

#### ProgramaCard.test.tsx

**Archivo:** `__tests__/components/ProgramaCard.test.tsx`

Componente de presentacion para el catalogo. Muestra datos del programa y el badge de estado.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders programa titulo and artista` | Unit | Muestra titulo y nombre del artista |
| 2 | `renders tipo de programa` | Unit | Muestra el tipoPromoNombre |
| 3 | `renders comision porcentaje when present` | Unit | Muestra "10%" cuando importeComisionPorcentaje tiene valor |
| 4 | `renders comision fija when present` | Unit | Muestra "5 EUR" cuando importeComisionFija tiene valor |
| 5 | `renders numero de tareas` | Unit | Muestra el count de tareas |
| 6 | `renders campaniaTitulo when present` | Unit | Muestra el titulo de la campana vinculada |
| 7 | `does not render campania section when null` | Unit | Oculta la seccion de campana cuando `campaniaTitulo` es null |
| 8 | `renders fechas de vigencia` | Unit | Muestra fechaInicio y fechaFin formateadas |
| 9 | `renders InscripcionEstadoBadge when miEstado is not null` | Unit | Delega el badge al subcomponente |
| 10 | `calls onVerDetalle when card is clicked` | Unit | El callback se invoca con el `id` del programa |

**Casos detallados:**

```markdown
3. renders comision porcentaje when present
   - Render: <ProgramaCard programa={mockProgramaExplorarItem} /> (tiene importeComisionPorcentaje: 10)
   - Assert: screen.getByText(/10%/i) en documento

10. calls onVerDetalle when card is clicked
   - Arrange: const handleDetalle = vi.fn()
   - Render: <ProgramaCard programa={mockProgramaExplorarItem} onVerDetalle={handleDetalle} />
   - Act: userEvent.click(screen.getByRole('article')) o el elemento clickeable
   - Assert: handleDetalle llamado con '3fa85f64-5717-4562-b3fc-2c963f66afa6'
```

---

#### SolicitarInscripcionButton.test.tsx

**Archivo:** `__tests__/components/SolicitarInscripcionButton.test.tsx`

Este es el componente con mayor logica condicional. Critico cubrir todos los estados.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `shows solicitar button when miEstado is null` | Unit | Boton "Solicitar inscripcion" visible y habilitado |
| 2 | `button is disabled while mutation is pending` | Unit | Boton deshabilitado durante `isPending: true` |
| 3 | `hides button when miEstado is Pendiente` | Unit | No muestra boton; muestra badge Pendiente |
| 4 | `hides button when miEstado is Aprobado` | Unit | No muestra boton; muestra badge Aprobado |
| 5 | `shows blocked message when miEstado is Bloqueado` | Unit | Mensaje "No puedes inscribirte en este programa" |
| 6 | `hides button when miEstado is DadoDeBaja` | Unit | No muestra boton para inscripciones dadas de baja |
| 7 | `calls onSolicitar when button clicked` | Unit | El callback se invoca al hacer click |
| 8 | `shows loading spinner while pending` | Unit | Indicador visual de carga durante la mutation |

**Casos detallados:**

```markdown
1. shows solicitar button when miEstado is null
   - Render: <SolicitarInscripcionButton miEstado={null} onSolicitar={vi.fn()} isPending={false} />
   - Assert: screen.getByRole('button', { name: /Solicitar inscripcion/i }) visible y habilitado

5. shows blocked message when miEstado is Bloqueado
   - Render: <SolicitarInscripcionButton miEstado="Bloqueado" onSolicitar={vi.fn()} isPending={false} />
   - Assert: screen.getByText(/No puedes inscribirte en este programa/i)
   - Assert: screen.queryByRole('button', { name: /Solicitar/i }) no en documento
```

---

#### MisProgramasListItem.test.tsx

**Archivo:** `__tests__/components/MisProgramasListItem.test.tsx`

Card para el listado de inscripciones del promotor.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders programa titulo and artista` | Unit | Muestra los datos del programa |
| 2 | `renders estado badge correcto` | Unit | Badge con el estado actual de la inscripcion |
| 3 | `renders comision info` | Unit | Muestra el tipo y valor de comision |
| 4 | `shows ver detalle link when aprobado` | Unit | Enlace o boton para ver detalle visible en aprobados |
| 5 | `does not show ver detalle when pendiente` | Unit | Sin enlace de detalle para pendientes |
| 6 | `calls onVerDetalle with inscripcion id` | Unit | Callback con el ID de la inscripcion |

---

#### DetalleInscripcionPanel.test.tsx

**Archivo:** `__tests__/components/DetalleInscripcionPanel.test.tsx`

Panel de detalle para inscripciones aprobadas con codigo referido, URL de tracking y boton copiar.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders codigoReferido` | Unit | Muestra el codigo referido textualmente |
| 2 | `renders urlTrackingPersonalizada` | Unit | Muestra la URL de tracking |
| 3 | `copies codigoReferido to clipboard on button click` | Unit | `navigator.clipboard.writeText` llamado con el codigo |
| 4 | `copies urlTracking to clipboard on button click` | Unit | `navigator.clipboard.writeText` llamado con la URL |
| 5 | `shows copy success feedback after copy` | Unit | Feedback visual tras copiar (ej: icono cambia, texto "Copiado") |
| 6 | `renders tareas list when tareas is not empty` | Unit | Lista de tareas del programa visibles |
| 7 | `renders empty state when tareas is empty` | Unit | Mensaje cuando no hay tareas asociadas |
| 8 | `shows tarea details: titulo, recompensa, repetibilidad` | Unit | Detalles de cada tarea en la lista |

**Casos detallados:**

```markdown
3. copies codigoReferido to clipboard on button click
   - Arrange: Object.assign(navigator, { clipboard: { writeText: vi.fn().mockResolvedValue(undefined) } })
   - Render: <DetalleInscripcionPanel inscripcion={mockMiPrograma_Aprobado} />
   - Act: userEvent.click(screen.getByRole('button', { name: /copiar codigo/i }))
   - Assert: navigator.clipboard.writeText fue llamado con 'album-2026-x7k9m'

5. shows copy success feedback after copy
   - Arrange: clipboard mock configurado
   - Act: click en boton copiar
   - Assert: await screen.findByText(/Copiado/i) o icono de check visible
```

---

### 4.4 Pages (Integration)

Los tests de paginas son los de mayor valor ya que verifican flujos completos del promotor. Se mockea el service, se usan providers reales de React Query.

#### ExplorarProgramasPage.test.tsx

**Archivo:** `__tests__/pages/ExplorarProgramasPage.test.tsx`

Verifica el flujo completo de la pagina `/crowdpromotion/explorar`.

**Setup base:**
- Mock de `inscripcionService` con `vi.mock`
- Mock de `useAuthStore` para simular promotor autenticado
- `renderWithProviders` con QueryClient
- Mock de toast (si se usa sonner o similar): `vi.mock('sonner', () => ({ toast: { success: vi.fn(), error: vi.fn() } }))`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders paginated list of programas` | Integration | Muestra todos los programas del mock paginado |
| 2 | `shows skeleton while loading` | Integration | Skeletons visibles antes de que el service resuelva |
| 3 | `shows error state when service fails` | Integration | Mensaje de error y opcion de reintentar |
| 4 | `shows empty state when no programas` | Integration | Mensaje cuando la lista retorna 0 items |
| 5 | `filters by artistaNombre on input change` | Integration | El input de filtro dispara el refetch con el nuevo filtro |
| 6 | `filters by tipoPromoId on select change` | Integration | El select dispara refetch con el tipo seleccionado |
| 7 | `solicitar inscripcion shows success toast` | Integration | Flujo completo: click -> mutation -> toast "Solicitud enviada al artista" |
| 8 | `solicitar inscripcion shows error toast on 400` | Integration | Toast de error cuando ya esta inscrito |
| 9 | `solicitar inscripcion shows error toast on 403` | Integration | Toast con mensaje de bloqueado cuando retorna 403 |
| 10 | `after successful inscripcion programa shows Pendiente badge` | Integration | El badge del programa pasa a "Pendiente" tras solicitar |
| 11 | `programa con miEstado Aprobado does not show solicitar button` | Integration | AC-CP03-1: estado personal visible, sin boton |
| 12 | `programa con miEstado Bloqueado shows blocked message` | Integration | AC-CP03-6: mensaje de bloqueo visible |

**Casos detallados:**

```markdown
5. filters by artistaNombre on input change
   - Arrange: inscripcionService.explorarProgramas resuelve con lista completa inicialmente
   - Act: await screen.findByPlaceholderText(/artista/i)
   - Act: userEvent.type(input, 'Luna Nova')
   - Assert: await waitFor(() => inscripcionService.explorarProgramas fue llamado con { artistaNombre: 'Luna Nova' })

7. solicitar inscripcion shows success toast
   - Arrange: inscripcionService.explorarProgramas resuelve con mockProgramasList (incluye programa sin inscripcion)
   - Arrange: inscripcionService.solicitarInscripcion resuelve con mockInscripcionCreada
   - Act: await screen.findByText('Promociona mi nuevo album')
   - Act: userEvent.click(screen.getByRole('button', { name: /Solicitar inscripcion/i }))
   - Assert: await waitFor(() => toast.success fue llamado con 'Solicitud enviada al artista')
   - Assert: inscripcionService.solicitarInscripcion llamado con '3fa85f64-...'

8. solicitar inscripcion shows error toast on 400
   - Arrange: inscripcionService.solicitarInscripcion rechaza con error { response.data.messages[0].message: 'Ya estas inscrito en este programa' }
   - Act: click en boton solicitar
   - Assert: await waitFor(() => toast.error llamado)
```

---

#### MisProgramasPage.test.tsx

**Archivo:** `__tests__/pages/MisProgramasPage.test.tsx`

Verifica el flujo completo de `/promotor/mis-programas`.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `renders lista de inscripciones con todos los estados` | Integration | Listado completo con badges correctos por estado |
| 2 | `shows skeleton while loading` | Integration | Skeletons durante carga |
| 3 | `shows empty state with link to explorar` | Integration | AC-CP03-15: si no hay inscripciones, empty state con enlace |
| 4 | `shows error state when service fails` | Integration | Mensaje de error |
| 5 | `inscripcion aprobada shows detalle with codigoReferido` | Integration | AC-CP03-14: codigo y URL visibles solo en aprobados |
| 6 | `inscripcion pendiente does not show codigoReferido` | Integration | `codigoReferido` no visible en estado Pendiente |
| 7 | `inscripcion aprobada allows copy codigoReferido` | Integration | Click en copiar -> clipboard.writeText con el codigo |
| 8 | `inscripcion aprobada allows copy urlTracking` | Integration | Click en copiar URL -> clipboard.writeText con la URL |
| 9 | `shows tareas for inscripcion aprobada` | Integration | Lista de tareas visible para aprobados |
| 10 | `inscripcion bloqueada shows Bloqueado badge` | Integration | Badge correcto para bloqueados |
| 11 | `inscripcion DadoDeBaja shows DadoDeBaja badge` | Integration | AC-CP03-15: badge "Baja" para dados de baja |

**Casos detallados:**

```markdown
3. shows empty state with link to explorar
   - Arrange: inscripcionService.getMisProgramas resuelve con { items: [], totalCount: 0, ... }
   - Render: <MisProgramasPage /> with providers
   - Assert: await screen.findByText(/no tienes inscripciones/i) o similar
   - Assert: screen.getByRole('link', { name: /explorar programas/i }) apunta a /crowdpromotion/explorar

5. inscripcion aprobada shows detalle with codigoReferido
   - Arrange: getMisProgramas resuelve con lista que incluye mockMiPrograma_Aprobado
   - Act: click en la inscripcion aprobada para ver detalle (si hay navegacion interna)
   - Assert: screen.getByText('album-2026-x7k9m') visible
   - Assert: screen.getByText(/utm_source=weplay/i) visible (URL de tracking)

6. inscripcion pendiente does not show codigoReferido
   - Arrange: lista incluye mockMiPrograma_Pendiente (codigoReferido: null)
   - Assert: 'album-2026-x7k9m' no en documento para el item pendiente
   - Assert: ningun elemento con texto de URL de tracking para pendiente
```

---

## 5. Cobertura por Archivo

| Archivo | Lineas Obj. | Funciones Obj. | Branches Obj. | Prioridad |
|---------|------------|----------------|---------------|-----------|
| `infrastructure/inscripcion.service.ts` | 90% | 100% | 85% | Alta |
| `application/hooks/useExplorarProgramas.ts` | 90% | 100% | 85% | Alta |
| `application/hooks/useSolicitarInscripcion.ts` | 90% | 100% | 90% | Alta |
| `application/hooks/useMisProgramas.ts` | 90% | 100% | 85% | Alta |
| `presentation/components/SolicitarInscripcionButton.tsx` | 95% | 100% | 95% | Alta (logica condicional compleja) |
| `presentation/components/InscripcionEstadoBadge.tsx` | 95% | 100% | 100% | Alta (todos los estados) |
| `presentation/components/DetalleInscripcionPanel.tsx` | 85% | 90% | 85% | Alta (clipboard) |
| `presentation/components/ProgramaCard.tsx` | 80% | 90% | 80% | Media |
| `presentation/components/MisProgramasListItem.tsx` | 80% | 90% | 80% | Media |
| `presentation/pages/ExplorarProgramasPage.tsx` | 80% | 85% | 80% | Alta (flujos criticos) |
| `presentation/pages/MisProgramasPage.tsx` | 80% | 85% | 80% | Alta (flujos criticos) |

**Meta Global:** 80% en todas las metricas

---

## 6. Criterios de Aceptacion Cubiertos por Tests

| AC | Cubierto por | Tipo |
|----|-------------|------|
| AC-CP03-1 | `ExplorarProgramasPage - renders paginated list` + `ProgramaCard - renders...` | Integration + Unit |
| AC-CP03-2 | `ExplorarProgramasPage - filters by artistaNombre` + `filters by tipoPromoId` | Integration |
| AC-CP03-6 | `SolicitarInscripcionButton - shows blocked message` + `ExplorarProgramasPage - shows blocked message` | Unit + Integration |
| AC-CP03-14 | `MisProgramasPage - inscripcion aprobada shows codigoReferido` + `pendiente does not show` | Integration |
| AC-CP03-15 | `MisProgramasPage - renders lista con todos los estados` + `shows empty state` | Integration |
| RN-04 (bloqueado no puede re-solicitar) | `SolicitarInscripcionButton - shows blocked message` + `solicitar shows error toast on 403` | Unit + Integration |

**Criterios de AC que dependen del Backend y NO se cubren aqui:** AC-CP03-3, AC-CP03-4, AC-CP03-5, AC-CP03-7 a AC-CP03-13.

---

## 7. Flujos de Error a Cubrir

| Escenario de Error | Donde se testea | Comportamiento Esperado |
|-------------------|-----------------|------------------------|
| Service `explorarProgramas` rechaza (500) | `ExplorarProgramasPage` | Mensaje de error visible, sin crash |
| Service `solicitarInscripcion` rechaza con 400 (ya inscrito) | `ExplorarProgramasPage` + `useSolicitarInscripcion` | Toast de error con mensaje descriptivo |
| Service `solicitarInscripcion` rechaza con 403 (bloqueado) | `ExplorarProgramasPage` | Toast con mensaje "No puedes inscribirte en este programa" |
| Service `getMisProgramas` rechaza (500) | `MisProgramasPage` | Mensaje de error visible |
| `navigator.clipboard.writeText` rechaza | `DetalleInscripcionPanel` | No crash; feedback de error opcional |
| Lista de programas vacia (0 items) | `ExplorarProgramasPage` | Empty state con CTA para buscar |
| Lista de inscripciones vacia (0 items) | `MisProgramasPage` | Empty state con enlace a `/crowdpromotion/explorar` |

---

## 8. Flujos Alternativos (FA) a Cubrir

| FA | Cubierto en | Test Case |
|----|-------------|-----------|
| FA-01: promotor ya inscrito, sin boton | `SolicitarInscripcionButton` + `ExplorarProgramasPage` | Tests de estado Pendiente, Aprobado, Bloqueado |
| FA-02: promotor bloqueado | `SolicitarInscripcionButton` | `shows blocked message when miEstado is Bloqueado` |
| FA-03: programas inactivos no aparecen | No aplica en frontend (filtrado server-side) | N/A |
| FA-04: promotor desactivado | `useSolicitarInscripcion` + `ExplorarProgramasPage` | `isError on 400` |

---

## 9. Convenciones del Proyecto a Seguir

Basado en el analisis de los tests existentes en `src/web/src/features/campanias/__tests__/`:

1. **Imports**: `{ describe, it, expect, vi }` desde `vitest`; no usar `test` alias, usar `it`
2. **React**: Importar `React` explicitamente para `React.createElement` en wrappers de hooks
3. **Mock del service**: Siempre con `vi.mock('ruta/relativa/al/service', ...)` ANTES de los imports del modulo testeado; luego importar el service mockeado despues con `import { service } from '...'`
4. **QueryClient en hooks**: Usar `createWrapper()` factory con `retry: false, gcTime: 0` (patron de `useRewardsByCampania.test.ts`)
5. **QueryClient en componentes**: Usar funcion local `renderWithProviders` con `staleTime: 0` adicional (patron de `CampaniaRewardsSection.test.tsx`)
6. **Nombrado de tests**: Descripcion en ingles tecnico o espanol mezclado, consistente con lo existente en el proyecto
7. **No usar `fireEvent` para interacciones de usuario**: Preferir `userEvent` de `@testing-library/user-event` para clicks, tipeo y keypresses que simulen comportamiento real del usuario
8. **`data-testid`**: Usar solo cuando no existe selector de accesibilidad (`role`, `label`, `text`); seguir el patron de `data-testid='reward-card'` ya existente

---

## 10. Comandos de Ejecucion

```bash
# Ejecutar todos los tests de la feature (desde src/web/)
npm run test -- --reporter=verbose src/features/crowdpromotion

# Ejecutar con coverage
npm run test:coverage -- src/features/crowdpromotion

# Watch mode para desarrollo
npm run test:watch -- src/features/crowdpromotion

# Ejecutar solo un archivo
npm run test -- src/features/crowdpromotion/__tests__/pages/ExplorarProgramasPage.test.tsx
```

---

## 11. Checklist

- [ ] Mocks definidos en `__mocks__/inscripcion.mock.ts` para todos los estados: null, Pendiente, Aprobado, Bloqueado, DadoDeBaja
- [ ] Service mockeado con `vi.mock` en todos los archivos de test que lo necesiten
- [ ] `InscripcionEstadoBadge` cubre los 5 estados del enum InscripcionEstado
- [ ] `SolicitarInscripcionButton` cubre todos los estados condicionales (especialmente Bloqueado = FA-02)
- [ ] `DetalleInscripcionPanel` cubre copiar al clipboard (codigo y URL)
- [ ] `ExplorarProgramasPage` cubre flujo de solicitud exitoso + error 400 + error 403
- [ ] `MisProgramasPage` cubre empty state + codigoReferido solo en aprobados + todos los badges
- [ ] Hooks cubren loading, success y error states
- [ ] Cobertura 80%+ en todas las metricas
- [ ] Tests pasan en menos de 60 segundos
