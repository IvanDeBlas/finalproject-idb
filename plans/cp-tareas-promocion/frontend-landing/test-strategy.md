# Estrategia de Testing: cp-tareas-promocion (Landing)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** `src/web` (Landing publica - vista Promotor)
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Archivos |
|------|----------|---------|
| Unit Tests | 18 | 2 componentes puros, 1 servicio |
| Integration Tests | 27 | 1 pagina, 1 dialogo con form, 2 hooks |
| Total | 45 | 7 archivos de test |

**Cobertura esperada por archivo:**

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| `MisTareasPage.tsx` | 85% | 90% | 80% |
| `TareaCard.tsx` | 90% | 100% | 85% |
| `CompletarTareaDialog.tsx` | 85% | 90% | 80% |
| `useMisTareas.ts` | 95% | 100% | 90% |
| `useCompletarTarea.ts` | 90% | 100% | 85% |
| `tareas.service.ts` | 95% | 100% | 90% |

**Meta Global:** 80%+ en todas las metricas

---

## 2. Estructura de Archivos de Test

```
src/web/src/features/crowdpromotion/
├── __mocks__/
│   └── tareas.mock.ts                              NUEVO
├── __tests__/
│   ├── pages/
│   │   └── MisTareasPage.test.tsx                  NUEVO
│   ├── components/
│   │   ├── TareaCard.test.tsx                      NUEVO
│   │   └── CompletarTareaDialog.test.tsx            NUEVO
│   ├── hooks/
│   │   ├── useMisTareas.test.ts                    NUEVO
│   │   └── useCompletarTarea.test.ts               NUEVO
│   └── services/
│       └── tareas.service.test.ts                  NUEVO
```

**Ubicacion real (paths absolutos):**

```
src/web/src/features/crowdpromotion/__mocks__/tareas.mock.ts
src/web/src/features/crowdpromotion/__tests__/pages/MisTareasPage.test.tsx
src/web/src/features/crowdpromotion/__tests__/components/TareaCard.test.tsx
src/web/src/features/crowdpromotion/__tests__/components/CompletarTareaDialog.test.tsx
src/web/src/features/crowdpromotion/__tests__/hooks/useMisTareas.test.ts
src/web/src/features/crowdpromotion/__tests__/hooks/useCompletarTarea.test.ts
src/web/src/features/crowdpromotion/__tests__/services/tareas.service.test.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Archivo de Mock Data

**Archivo:** `src/web/src/features/crowdpromotion/__mocks__/tareas.mock.ts`

**Proposito:** Centralizar todos los datos de prueba para la feature de tareas. Sigue el patron establecido en `inscripcion.mock.ts`.

**Imports requeridos:**
```typescript
import type {
    MisTareasItem,
    MiEstadoTarea,
    MisTareasResponse,
    CompletarTareaResponse,
} from '@shared/types/crowdpromotion'
```

**Fixtures a definir:**

```
mockMiEstado_SinCompletar
  - undefined (el promotor nunca ha completado esta tarea)
  - Usado para: TareaCard con boton activo, primera ejecucion

mockMiEstado_Completada
  - estadoTareaId: 2
  - estadoTareaNombre: 'Completada'
  - vecesCompletada: 1
  - fechaUltimaCompletada: '2026-03-01T10:00:00Z'
  - tareaPromotorId: 'ttp-001'
  - Usado para: tarea no repetible bloqueada, tarea repetible aun con cupo

mockMiEstado_Validada
  - estadoTareaId: 3
  - estadoTareaNombre: 'Validada'
  - vecesCompletada: 1
  - tareaPromotorId: 'ttp-002'
  - Usado para: tarea no repetible bloqueada (estado final)

mockMiEstado_Rechazada
  - estadoTareaId: 4
  - estadoTareaNombre: 'Rechazada'
  - vecesCompletada: 0
  - comentarioValidacion: 'La URL no muestra el contenido requerido'
  - tareaPromotorId: 'ttp-003'
  - Usado para: mostrar motivo de rechazo, boton de re-envio disponible

mockTarea_Pendiente
  - tareaId: 'tarea-001'
  - nombre: 'Comparte en Instagram Stories'
  - descripcion: 'Sube una story mencionando la campana de lanzamiento'
  - instruccionesUrl: 'https://weplay.com/instrucciones/instagram'
  - tipoEventoPromoNombre: 'Share'
  - tipoRewardNombre: 'Monetaria'
  - importeRecompensa: 5.00
  - monedaNombre: 'EUR'
  - puntosRecompensa: undefined
  - esRepetible: true
  - maxRepeticiones: 10
  - orden: 1
  - miEstado: undefined  (promotor nunca ha completado)

mockTarea_Completada_NoRepetible
  - tareaId: 'tarea-002'
  - nombre: 'Escribe una resena en Spotify'
  - esRepetible: false
  - maxRepeticiones: undefined
  - miEstado: mockMiEstado_Completada
  - Usado para: boton deshabilitado en tarea no repetible

mockTarea_Validada_NoRepetible
  - tareaId: 'tarea-003'
  - nombre: 'Publica un video en TikTok'
  - esRepetible: false
  - maxRepeticiones: undefined
  - miEstado: mockMiEstado_Validada
  - Usado para: boton deshabilitado, estado final validado

mockTarea_Rechazada
  - tareaId: 'tarea-004'
  - nombre: 'Menciona el album en tu blog'
  - esRepetible: false
  - maxRepeticiones: undefined
  - miEstado: mockMiEstado_Rechazada
  - Usado para: mostrar motivo de rechazo y boton de re-envio

mockTarea_RepetibleLimiteAlcanzado
  - tareaId: 'tarea-005'
  - nombre: 'Comparte en Twitter/X'
  - esRepetible: true
  - maxRepeticiones: 3
  - miEstado: { estadoTareaId: 2, vecesCompletada: 3 }  (limite alcanzado)
  - Usado para: boton deshabilitado con mensaje de limite

mockTarea_RepetibleConCupo
  - tareaId: 'tarea-006'
  - nombre: 'Comenta en YouTube'
  - esRepetible: true
  - maxRepeticiones: 5
  - miEstado: { estadoTareaId: 2, vecesCompletada: 2 }  (2/5, aun hay cupo)
  - Usado para: boton activo en tarea repetible con cupo disponible

mockTarea_SoloPuntos
  - tareaId: 'tarea-007'
  - importeRecompensa: undefined
  - monedaNombre: undefined
  - puntosRecompensa: 100
  - tipoRewardNombre: 'Puntos'
  - Usado para: mostrar puntos en lugar de importe monetario

mockMisTareasResponse_ConTareas
  - programaId: 'prog-001'
  - programaTitulo: 'Promociona mi nuevo album'
  - items: [mockTarea_Pendiente, mockTarea_Completada_NoRepetible, mockTarea_Rechazada]

mockMisTareasResponse_Vacia
  - programaId: 'prog-002'
  - programaTitulo: 'Programa sin tareas activas'
  - items: []

mockCompletarTareaResponse
  - tareaPromotorId: 'ttp-new-001'
  - estadoTareaId: 2
  - estadoTareaNombre: 'Completada'
  - vecesCompletada: 1
  - fechaUltimaCompletada: '2026-03-01T12:00:00Z'
```

### 3.2 Estrategia de Mocking por Archivo

**Para tests de componentes (TareaCard, CompletarTareaDialog):**
- Sin mocks de servicios ni hooks: componentes puros que reciben props
- `vi.mock('sonner', ...)` para tests que comprueban feedback visual si el componente dispara toasts internamente

**Para tests de pagina (MisTareasPage):**
- `vi.mock('../../infrastructure/tareas.service', ...)` para mockear el servicio
- `vi.mock('@/features/promotor/application', ...)` para mockear `usePromotor`
- `vi.mock('sonner', ...)` para capturar toasts
- `vi.mock('react-router-dom', ...)` solo si se necesita `useParams` o navegacion

**Para tests de hooks:**
- `vi.mock('../../infrastructure/tareas.service', ...)` para aislar la capa de servicio
- `vi.mock('sonner', ...)` para verificar toasts en `useCompletarTarea`
- Wrapper con `QueryClientProvider` de test (retry: false, gcTime: 0)

**Para tests de servicio:**
- `vi.mock('@/lib/api-client', ...)` para interceptar `apiFetch`
- Sin wrapper de React Query (tests de logica pura async)

### 3.3 Test Utilities (patron del proyecto)

El proyecto usa el patron de `createWrapper()` con `QueryClient` de test. No requiere un archivo `test-utils.tsx` separado porque cada archivo de test define su wrapper inline siguiendo el patron ya establecido:

```typescript
// Patron estandar del proyecto (ver useSolicitarInscripcion.test.ts)
const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

// Para tests de pagina con router (ver MisProgramasPage.test.tsx)
function renderPage(programaId = 'prog-001') {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={[`/promotor/mis-programas/prog-001/tareas`]}>
                <MisTareasPage />
            </MemoryRouter>
        </QueryClientProvider>
    )
}
```

---

## 4. Tests por Modulo

### 4.1 MisTareasPage.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/pages/MisTareasPage.test.tsx`
**Tipo predominante:** Integration
**Total de casos:** 10

**Setup de mocks requeridos:**
```
vi.mock('sonner')
vi.mock('@/features/promotor/application', () => ({ usePromotor: vi.fn() }))
vi.mock('../../infrastructure/tareas.service', () => ({ tareasService: { misTareas: vi.fn() } }))
```

**Tabla de casos:**

| # | Caso | Tipo | AAA |
|---|------|------|-----|
| 1 | renders lista de tareas en fetch exitoso | Integration | A: mockear `tareasService.misTareas` con `mockMisTareasResponse_ConTareas`; Ac: renderPage(); As: `waitFor` que `screen.getByText('Comparte en Instagram Stories')` este en documento |
| 2 | renders skeleton durante carga | Integration | A: mockear servicio con `new Promise(() => {})` (nunca resuelve); Ac: renderPage(); As: `screen.getByLabelText('Cargando tareas')` en documento |
| 3 | renders empty state cuando no hay tareas | Integration | A: mockear con `mockMisTareasResponse_Vacia`; Ac: renderPage(); As: `waitFor` que `screen.getByText(/No hay tareas activas/)` en documento |
| 4 | renders error state cuando falla el fetch | Integration | A: mockear con `Promise.reject(new Error('API Error'))`; Ac: renderPage(); As: `waitFor` que mensaje de error y boton "Reintentar" esten en documento |
| 5 | renders nombre del programa en el header | Integration | A: mockear con `mockMisTareasResponse_ConTareas`; Ac: renderPage(); As: `waitFor` que `screen.getByText('Promociona mi nuevo album')` en documento |
| 6 | renders multiples TareaCard por cada item | Integration | A: mockear respuesta con 3 items; Ac: renderPage(); As: `waitFor` que todos los nombres de tareas esten en documento |
| 7 | abre CompletarTareaDialog al click en boton de tarea completable | Integration | A: mockear con tarea pendiente; Ac: renderPage() + `waitFor` + `userEvent.click(boton Completar)`; As: dialog con titulo "Completar tarea" este visible |
| 8 | no abre dialog al click en tarea no completable | Integration | A: mockear con tarea no repetible ya completada; Ac: renderPage() + `waitFor`; As: boton de completar no presente o deshabilitado |
| 9 | cierra el dialog tras completar exitosamente | Integration | A: mockear servicio + mutation exitosa; Ac: abrir dialog + completar form + submit; As: `waitFor` que dialog no este en documento |
| 10 | muestra titulo de pagina "Mis Tareas" | Integration | A: mockear servicio con pending promise; Ac: renderPage(); As: `screen.getByText('Mis Tareas')` en documento |

**Notas de implementacion:**
- `useParams` retornara `programaId` del `MemoryRouter`. Si `MisTareasPage` usa `useParams`, mockear `react-router-dom` con `vi.importActual` para preservar `MemoryRouter` y solo sobrescribir `useParams`.
- Si el componente recibe `programaId` por props en lugar de `useParams`, pasar directamente en `renderPage`.
- Usar `{ timeout: 5000 }` en `waitFor` para casos de fetch async, siguiendo el patron de `MisProgramasPage.test.tsx`.

---

### 4.2 TareaCard.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/components/TareaCard.test.tsx`
**Tipo predominante:** Unit
**Total de casos:** 14

**Setup de mocks:**
```
// Sin mocks de servicios. TareaCard es un componente de presentacion puro.
// Solo mockear si emite eventos hacia hooks.
```

**Tabla de casos:**

| # | Caso | Tipo | AAA |
|---|------|------|-----|
| 1 | renders nombre de la tarea | Unit | A: `render(<TareaCard tarea={mockTarea_Pendiente} onCompletarClick={vi.fn()} />)`; Ac: (impliciito); As: `screen.getByText('Comparte en Instagram Stories')` en documento |
| 2 | renders tipo de evento como badge | Unit | A: render con `mockTarea_Pendiente`; Ac: -; As: `screen.getByText('Share')` en documento |
| 3 | renders importe de recompensa monetaria | Unit | A: render con tarea de recompensa monetaria; Ac: -; As: `screen.getByText(/5\.00 EUR/)` en documento |
| 4 | renders puntos de recompensa cuando no hay importe | Unit | A: render con `mockTarea_SoloPuntos`; Ac: -; As: `screen.getByText(/100 puntos/)` en documento |
| 5 | renders info de repeticiones cuando esRepetible es true | Unit | A: render con `mockTarea_Pendiente` (repetible x10); Ac: -; As: `screen.getByText(/Repetible/)` en documento |
| 6 | renders "No repetible" cuando esRepetible es false | Unit | A: render con tarea no repetible; Ac: -; As: `screen.getByText(/No repetible/)` en documento |
| 7 | renders boton "Completar" cuando miEstado es undefined | Unit | A: render con `mockTarea_Pendiente` (miEstado: undefined); Ac: -; As: boton "Completar" presente y no deshabilitado |
| 8 | deshabilia boton cuando tarea no repetible ya esta Completada | Unit | A: render con `mockTarea_Completada_NoRepetible`; Ac: -; As: boton ausente o deshabilitado; As extra: algun indicador de estado "Completada" visible |
| 9 | deshabilita boton cuando tarea no repetible ya esta Validada | Unit | A: render con `mockTarea_Validada_NoRepetible`; Ac: -; As: boton ausente o deshabilitado |
| 10 | deshabilita boton cuando limite de repeticiones alcanzado | Unit | A: render con `mockTarea_RepetibleLimiteAlcanzado` (3/3); Ac: -; As: boton deshabilitado; As extra: texto indicando limite (ej: "3/3" o "Limite alcanzado") |
| 11 | habilita boton en tarea repetible con cupo disponible | Unit | A: render con `mockTarea_RepetibleConCupo` (2/5); Ac: -; As: boton "Completar" presente y habilitado |
| 12 | muestra motivo de rechazo cuando estadoTareaId es 4 | Unit | A: render con `mockTarea_Rechazada`; Ac: -; As: `screen.getByText('La URL no muestra el contenido requerido')` visible |
| 13 | habilita boton de re-envio cuando tarea esta Rechazada | Unit | A: render con `mockTarea_Rechazada`; Ac: -; As: boton "Re-enviar" o "Completar" presente y habilitado |
| 14 | llama onCompletarClick con tareaId al hacer click en boton | Unit | A: `const handleClick = vi.fn()` + render; Ac: `userEvent.click(screen.getByRole('button', { name: /Completar/i }))`; As: `handleClick` llamado con `mockTarea_Pendiente.tareaId` |

**Notas de implementacion:**
- Todos los tests son unitarios: render directo sin QueryClientProvider.
- Los casos 8, 9, 10 verifican que `puedeCompletarTarea` del shared mapper esta siendo aplicada correctamente.
- Para los casos de estado visible, usar `screen.getByLabelText` o `screen.getByText` segun como se implemente el badge de estado en el componente.
- El caso 12 (motivo de rechazo) aplica directamente al criterio de aceptacion AC-CP04-9.

---

### 4.3 CompletarTareaDialog.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/components/CompletarTareaDialog.test.tsx`
**Tipo predominante:** Integration
**Total de casos:** 13

**Props del componente esperadas:**
```typescript
interface CompletarTareaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    tarea: MisTareasItem
    onSubmit: (data: CompletarTareaFormData) => Promise<void>
    isSubmitting: boolean
}
```

**Setup de mocks:**
```
// Sin mocks de servicios. El dialog recibe onSubmit como prop.
// No requiere QueryClientProvider si no usa hooks propios.
vi.mock('sonner')  // Solo si el componente emite toast internamente
```

**defaultProps para los tests:**
```
{
    open: true,
    onOpenChange: vi.fn(),
    tarea: mockTarea_Pendiente,
    onSubmit: vi.fn().mockResolvedValue(undefined),
    isSubmitting: false,
}
```

**Tabla de casos:**

| # | Caso | Tipo | AAA |
|---|------|------|-----|
| 1 | no renderiza nada cuando open es false | Unit | A: render con `open: false`; As: `screen.queryByText('Completar tarea')` no en documento |
| 2 | renderiza el dialog cuando open es true | Unit | A: render con `open: true`; As: titulo del dialog visible |
| 3 | muestra el nombre de la tarea en el dialog | Unit | A: render; As: `screen.getByText('Comparte en Instagram Stories')` en documento |
| 4 | muestra instrucciones de URL cuando existen | Unit | A: render con tarea que tiene `instruccionesUrl`; As: link o texto con la URL de instrucciones visible |
| 5 | campo URL es requerido: muestra error al submit sin URL | Integration | A: render; Ac: `userEvent.click(boton Enviar)` sin llenar nada; As: `waitFor` que `screen.getByText('La URL de prueba es obligatoria')` en documento |
| 6 | muestra error cuando URL tiene formato invalido | Integration | A: render; Ac: `userEvent.type(input URL, 'no-es-una-url')` + submit; As: `waitFor` que mensaje de formato invalido visible |
| 7 | acepta URL valida sin mostrar error | Integration | A: render; Ac: `userEvent.type(input URL, 'https://instagram.com/stories/test')` + submit; As: `waitFor` que NO haya mensajes de error de URL |
| 8 | campo comentario es opcional | Integration | A: render; Ac: solo llenar URL valida + submit; As: `onSubmit` llamado sin mensaje de error por comentario vacio |
| 9 | contador de caracteres del comentario empieza en 0/500 | Unit | A: render; As: `screen.getByText('0/500')` en documento |
| 10 | actualiza contador al escribir en el comentario | Integration | A: render; Ac: `userEvent.type(textarea comentario, 'Mi comentario')` (13 chars); As: `screen.getByText('13/500')` en documento |
| 11 | llama onSubmit con los datos del formulario al submit valido | Integration | A: render; Ac: llenar URL valida + comentario + click submit; As: `waitFor` que `onSubmit` llamado con `{ urlPruebaCompletado: 'https://...', comentarioPromotor: '...' }` |
| 12 | muestra estado de carga durante isSubmitting | Unit | A: render con `isSubmitting: true`; As: boton submit deshabilitado y con texto de carga (ej: "Enviando...") |
| 13 | llama onOpenChange(false) al hacer click en Cancelar | Integration | A: render con `onOpenChange: vi.fn()`; Ac: `userEvent.click(screen.getByRole('button', { name: /Cancelar/i }))`; As: `onOpenChange` llamado con `false` |

**Notas de implementacion:**
- Los casos 5-11 son integration porque implican la interaccion del usuario con el form y la validacion Zod de `completarTareaSchema`.
- Para los casos de validacion (5, 6), asegurarse de hacer `await user.click(submitButton)` seguido de `waitFor` para esperar la re-renderizacion con los errores.
- El caso 11 debe verificar que `onSubmit` reciba exactamente `CompletarTareaFormData` (sin campos extra del formulario interno).
- Para el caso 10, el contador puede estar implementado como `{comentario.length}/500` en el DOM; buscar con `screen.getByText(/13\/500/)`.

---

### 4.4 useMisTareas.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/useMisTareas.test.ts`
**Tipo predominante:** Integration
**Total de casos:** 5

**Setup de mocks:**
```
vi.mock('../../infrastructure/tareas.service', () => ({
    tareasService: { misTareas: vi.fn() }
}))
```

**Tabla de casos:**

| # | Caso | Tipo | AAA |
|---|------|------|-----|
| 1 | retorna datos correctamente en fetch exitoso | Integration | A: `vi.mocked(tareasService.misTareas).mockResolvedValue(mockMisTareasResponse_ConTareas)`; Ac: `renderHook(() => useMisTareas('prog-001'), { wrapper })`; As: `waitFor(() => expect(result.current.isSuccess).toBe(true))` + `result.current.data` igual a mock |
| 2 | isLoading es true inicialmente | Integration | A: mockear con promise que no resuelve; Ac: renderHook; As: `expect(result.current.isLoading).toBe(true)` inmediatamente |
| 3 | isError es true cuando el servicio falla | Integration | A: `tareasService.misTareas.mockRejectedValue(new Error('API Error'))`; Ac: renderHook; As: `waitFor(() => expect(result.current.isError).toBe(true))` |
| 4 | llama al servicio con el programaId correcto | Integration | A: mockear con respuesta exitosa; Ac: renderHook con `programaId = 'prog-especifico'`; As: `expect(tareasService.misTareas).toHaveBeenCalledWith('prog-especifico')` |
| 5 | usa la query key correcta con el programaId | Integration | A: mockear con respuesta exitosa; Ac: renderHook; As: `waitFor` isSuccess + verificar que el queryClient tiene la key `['crowdpromotion', 'tareas', 'prog-001', 'mis']` en cache (via `queryClient.getQueryData`) |

**Notas de implementacion:**
- El wrapper usa el patron `createWrapper()` con `QueryClient` de test: `retry: false, gcTime: 0, staleTime: 0`.
- Para el caso 5, acceder al `queryClient` de test desde fuera del wrapper para usar `queryClient.getQueryData(QUERY_KEYS.crowdpromotion.tareas.mis('prog-001'))`.
- Este patron esta documentado en el proyecto con `invalidateSpy = vi.spyOn(queryClient, 'invalidateQueries')`.

---

### 4.5 useCompletarTarea.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/useCompletarTarea.test.ts`
**Tipo predominante:** Integration
**Total de casos:** 8

**Setup de mocks:**
```
vi.mock('sonner', () => ({ toast: { success: vi.fn(), error: vi.fn() } }))
vi.mock('../../infrastructure/tareas.service', () => ({
    tareasService: { completarTarea: vi.fn() }
}))
```

**Tabla de casos:**

| # | Caso | Tipo | AAA |
|---|------|------|-----|
| 1 | mutation ejecuta exitosamente | Integration | A: `tareasService.completarTarea.mockResolvedValue(mockCompletarTareaResponse)` + `renderHook(() => useCompletarTarea('prog-001'), { wrapper })`; Ac: `act(() => result.current.mutate({ tareaId: 'tarea-001', data: { urlPruebaCompletado: 'https://...' } }))`; As: `waitFor(() => expect(result.current.isSuccess).toBe(true))` |
| 2 | llama al servicio con programaId, tareaId y datos correctos | Integration | A: mockear exito; Ac: mutate con datos especificos; As: `expect(tareasService.completarTarea).toHaveBeenCalledWith('prog-001', 'tarea-001', { urlPruebaCompletado: 'https://instagram.com/p/test', comentarioPromotor: undefined })` |
| 3 | muestra toast de exito tras completar | Integration | A: mockear exito; Ac: mutate + `waitFor isSuccess`; As: `expect(toast.success).toHaveBeenCalledWith(expect.stringContaining('enviada para validacion'))` |
| 4 | invalida la query mis-tareas del programa tras exito | Integration | A: mockear exito + `queryClient` propio + `invalidateSpy`; Ac: mutate + `waitFor isSuccess`; As: `expect(invalidateSpy).toHaveBeenCalledWith(expect.objectContaining({ queryKey: ['crowdpromotion', 'tareas', 'prog-001', 'mis'] }))` |
| 5 | isError true cuando el servicio retorna error de negocio (4027) | Integration | A: mockear rechazo con error `{ errorCode: '4027' }`; Ac: mutate; As: `waitFor(() => expect(result.current.isError).toBe(true))` |
| 6 | muestra toast de error especifico para 4027 (no repetible) | Integration | A: mockear error 4027; Ac: mutate + `waitFor isError`; As: `expect(toast.error).toHaveBeenCalledWith(expect.stringContaining('no se puede volver a completar'))` o mensaje del `TAREA_PROMOCION_ERROR_MESSAGES['4027']` |
| 7 | muestra toast de error especifico para 4028 (limite alcanzado) | Integration | A: mockear error 4028; Ac: mutate + `waitFor isError`; As: `expect(toast.error).toHaveBeenCalledWith(expect.stringContaining('maximo de veces'))` |
| 8 | isPending es true durante la mutacion | Integration | A: mockear con promise que no resuelve inmediatamente; Ac: act + mutate sin await; As: `waitFor(() => expect(result.current.isPending).toBe(true))` |

**Notas de implementacion:**
- El patron de error con `errorCode` extendido en el `Error` sigue el patron del proyecto: `const error = new Error('mensaje'); (error as Error & { errorCode: string }).errorCode = '4027'`.
- Para el caso 4, crear el `queryClient` fuera del wrapper para poder hacer `vi.spyOn(queryClient, 'invalidateQueries')`. Patron documentado en `useSolicitarInscripcion.test.ts`.
- La firma del hook puede ser `useCompletarTarea(programaId: string)` o recibir `programaId` en `mutate`. Ajustar los tests segun la implementacion del frontend-plan.

---

### 4.6 tareas.service.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/services/tareas.service.test.ts`
**Tipo predominante:** Unit
**Total de casos:** 10

**Setup de mocks:**
```
vi.mock('@/lib/api-client', () => ({ apiFetch: vi.fn() }))
```

**Patron de respuesta API del proyecto:**
```typescript
// apiFetch retorna ServiceResponse<T> donde:
// - data: T (el payload)
// - messages: Array<{ message: string, errorCode: string }>
// errorCode que empieza con '0' = exito; cualquier otro = error
```

**Tabla de casos:**

| # | Metodo | Caso | Tipo | AAA |
|---|--------|------|------|-----|
| 1 | `misTareas` | retorna `MisTareasResponse` en fetch exitoso | Unit | A: `apiFetch.mockResolvedValue({ data: mockMisTareasResponse_ConTareas, messages: [] })`; Ac: `await tareasService.misTareas('prog-001')`; As: resultado igual a `mockMisTareasResponse_ConTareas` |
| 2 | `misTareas` | llama a la URL correcta con el programaId | Unit | A: mockear respuesta exitosa; Ac: `await tareasService.misTareas('prog-xyz')`; As: `expect(apiFetch).toHaveBeenCalledWith(expect.stringContaining('/crowdpromotion/programas/prog-xyz/mis-tareas'))` |
| 3 | `misTareas` | lanza error cuando messages contiene errorCode no-0xxx | Unit | A: `apiFetch.mockResolvedValue({ data: null, messages: [{ message: 'Programa no encontrado', errorCode: '2019' }] })`; Ac: `await tareasService.misTareas('prog-001')`; As: `rejects.toThrow('Programa no encontrado')` |
| 4 | `completarTarea` | retorna `CompletarTareaResponse` en POST exitoso | Unit | A: `apiFetch.mockResolvedValue({ data: mockCompletarTareaResponse, messages: [{ message: 'Tarea enviada', errorCode: '0001' }] })`; Ac: `await tareasService.completarTarea('prog-001', 'tarea-001', { urlPruebaCompletado: 'https://test.com' })`; As: resultado igual a `mockCompletarTareaResponse` |
| 5 | `completarTarea` | llama endpoint correcto con POST y body | Unit | A: mockear respuesta exitosa; Ac: `await tareasService.completarTarea('prog-001', 'tarea-001', body)`; As: `expect(apiFetch).toHaveBeenCalledWith(expect.stringContaining('/programas/prog-001/tareas/tarea-001/completar'), expect.objectContaining({ method: 'POST' }))` |
| 6 | `completarTarea` | incluye comentarioPromotor en el body cuando se provee | Unit | A: mockear exito; Ac: llamar con `{ urlPruebaCompletado: 'https://test.com', comentarioPromotor: 'Mi comentario' }`; As: `expect(apiFetch).toHaveBeenCalledWith(expect.any(String), expect.objectContaining({ body: expect.stringContaining('comentarioPromotor') }))` |
| 7 | `completarTarea` | lanza error 4027 (tarea no repetible) cuando backend retorna ese codigo | Unit | A: `apiFetch.mockResolvedValue({ data: null, messages: [{ message: '...', errorCode: '4027' }] })`; Ac: llamar; As: `rejects.toThrow()` + error con `errorCode === '4027'` |
| 8 | `completarTarea` | lanza error 4028 (limite alcanzado) cuando backend retorna ese codigo | Unit | A: similar con `errorCode: '4028'`; As: `rejects.toThrow()` + error con `errorCode === '4028'` |
| 9 | `completarTarea` | lanza error generico cuando apiFetch falla en red | Unit | A: `apiFetch.mockRejectedValue(new Error('Network error'))`; Ac: llamar; As: `rejects.toThrow('Network error')` |
| 10 | `misTareas` | lanza error 403 cuando promotor no aprobado | Unit | A: `apiFetch.mockResolvedValue({ data: null, messages: [{ message: 'No tienes permiso...', errorCode: '4026' }] })`; As: `rejects.toThrow()` + error con `errorCode === '4026'` |

**Notas de implementacion:**
- Los tests de servicio son puros (sin React ni QueryClient). Solo `describe` + `beforeEach(() => vi.clearAllMocks())`.
- Para verificar el `errorCode` en el error lanzado, usar el patron ya establecido en `inscripcion.service.test.ts`:
  ```typescript
  // La funcion extractError del servicio adjunta errorCode al Error:
  await expect(tareasService.completarTarea(...)).rejects.toThrow('mensaje')
  // Para verificar el errorCode:
  try { await tareasService.completarTarea(...) } catch (e: unknown) {
      expect((e as { errorCode?: string }).errorCode).toBe('4027')
  }
  ```
- El caso 6 sobre el body depende de como `apiFetch` recibe el body. Si usa `JSON.stringify`, el assert debe buscar en el string; si usa objeto, buscar en `body` directamente.

---

## 5. Flujos Criticos Cubiertos por Integration Tests

Los siguientes flujos del AC (criterios de aceptacion) quedan cubiertos por la combinacion de tests:

| AC | Criterio | Test que lo cubre |
|----|----------|-------------------|
| AC-CP04-1 | Promotor ve listado de tareas con nombre, tipo, recompensa y estado | `TareaCard` casos 1-6 + `MisTareasPage` caso 1 |
| AC-CP04-2 | URL requerida con formato valido; comentario opcional max 500 | `CompletarTareaDialog` casos 5-8 |
| AC-CP04-5 | Tarea no repetible ya completada: boton deshabilitado | `TareaCard` casos 8-9 |
| AC-CP04-6 | Limite de repeticiones alcanzado: boton deshabilitado | `TareaCard` caso 10 |
| AC-CP04-9 | Motivo de rechazo visible para el promotor | `TareaCard` caso 12 |
| AC-CP04-10 | Re-envio posible cuando estado es Rechazada | `TareaCard` caso 13 |
| AC-CP04-14 | Tipos TypeScript de shared usados correctamente | Impliciito via imports en mocks |

**Flujo critico end-to-end cubierto por `MisTareasPage` casos 1 + 7 combinados:**
```
Promotor ve lista de tareas -> hace click en "Completar" -> se abre dialog ->
introduce URL valida -> submit -> dialog se cierra -> lista refrescada
```

---

## 6. Casos Edge y Errores de Negocio

Los siguientes edge cases tienen cobertura explicita:

| Edge Case | Test |
|-----------|------|
| Tarea repetible sin limite (`maxRepeticiones: undefined`) | `TareaCard` caso 7 (miEstado undefined: puede completar siempre) |
| Tarea solo con puntos (sin importe monetario) | `TareaCard` caso 4 |
| Respuesta API con lista vacia de tareas | `MisTareasPage` caso 3 |
| Error de red en fetch de tareas | `MisTareasPage` caso 4 |
| Error 4027 (no repetible ya completada) | `useCompletarTarea` caso 6 + `tareas.service` caso 7 |
| Error 4028 (limite de repeticiones) | `useCompletarTarea` caso 7 + `tareas.service` caso 8 |
| Error 4026 (promotor no aprobado) | `tareas.service` caso 10 |
| Dialog con `open: false` no renderiza contenido | `CompletarTareaDialog` caso 1 |
| Submit con URL invalida (no formato URL) | `CompletarTareaDialog` caso 6 |
| isSubmitting bloquea submit y cancelar | `CompletarTareaDialog` caso 12 |

---

## 7. Queries de Testing Library a Usar por Componente

### MisTareasPage
- `screen.getByText(nombre)` para texto del programa y nombre de tareas
- `screen.getByLabelText('Cargando tareas')` para el skeleton
- `screen.getByText(/No hay tareas activas/i)` para empty state
- `screen.getByRole('button', { name: /Reintentar/i })` para error state
- `screen.getByRole('button', { name: /Completar/i })` para abrir dialog

### TareaCard
- `screen.getByText(tarea.nombre)` para nombre
- `screen.getByText(tarea.tipoEventoPromoNombre)` para badge de tipo
- `screen.getByText(/5\.00 EUR/)` para recompensa monetaria
- `screen.getByText(/100 puntos/)` para recompensa en puntos
- `screen.getByText(/Repetible/)` o `screen.getByText(/No repetible/)` para repetibilidad
- `screen.getByRole('button', { name: /Completar/i })` para boton de accion
- `screen.queryByRole('button', ...)` para verificar ausencia de boton
- `screen.getByText(comentarioValidacion)` para motivo de rechazo

### CompletarTareaDialog
- `screen.getByRole('dialog')` para verificar apertura
- `screen.getByPlaceholderText(/URL de prueba/i)` para input URL
- `screen.getByPlaceholderText(/Comentario/i)` para textarea
- `screen.getByText(/0\/500/)` para contador de caracteres
- `screen.getByRole('button', { name: /Enviar/i })` para submit
- `screen.getByRole('button', { name: /Cancelar/i })` para cerrar
- `screen.getByRole('button', { name: /Enviando/i })` para loading state
- `screen.getByText('La URL de prueba es obligatoria')` para error de validacion

---

## 8. Estructura de Imports en Cada Archivo de Test

### Patron de imports (basado en el codebase existente)

```typescript
// Vitest + RTL
import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import { renderHook, act } from "@testing-library/react"
import userEvent from "@testing-library/user-event"

// React + Router + Query
import React from "react"
import { MemoryRouter } from "react-router-dom"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

// Mocks del feature
import { mockTarea_Pendiente, mockMisTareasResponse_ConTareas } from "../../__mocks__/tareas.mock"

// El modulo bajo test (importar DESPUES de los vi.mock)
import { NombreComponente } from "../../presentation/components/NombreComponente"
import { useNombreHook } from "../../application/hooks/useNombreHook"
import { tareasService } from "../../infrastructure/tareas.service"
```

**Regla critica:** Los `vi.mock(...)` deben declararse ANTES de los imports del modulo bajo test, siguiendo el patron de todos los tests del proyecto. Vitest eleva (hoists) los `vi.mock` al principio del archivo.

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del proyecto web
cd src/web && npm run test

# Ejecutar con cobertura
cd src/web && npm run test:coverage

# Ejecutar solo los tests de crowdpromotion
cd src/web && npm run test -- --reporter=verbose src/features/crowdpromotion

# Ejecutar solo los tests de tareas de promocion
cd src/web && npm run test -- --reporter=verbose src/features/crowdpromotion/__tests__

# Watch mode para desarrollo
cd src/web && npm run test:watch

# Ejecutar un archivo especifico
cd src/web && npm run test -- src/features/crowdpromotion/__tests__/components/TareaCard.test.tsx
```

---

## 10. CI/CD Integration

Los tests se ejecutan en el mismo pipeline existente. No requiere cambios en la configuracion de CI:

```yaml
- name: Run Landing Tests
  working-directory: src/web
  run: npm run test:coverage

- name: Check Coverage Threshold
  working-directory: src/web
  run: npm run test:coverage -- --coverage.thresholds.lines=80
```

---

## 11. Checklist de Implementacion

- [ ] Crear `src/web/src/features/crowdpromotion/__mocks__/tareas.mock.ts` con todos los fixtures
- [ ] Crear `__tests__/services/tareas.service.test.ts` (10 casos, base para validar la capa de infra)
- [ ] Crear `__tests__/hooks/useMisTareas.test.ts` (5 casos, query hook)
- [ ] Crear `__tests__/hooks/useCompletarTarea.test.ts` (8 casos, mutation hook con invalidacion)
- [ ] Crear `__tests__/components/TareaCard.test.tsx` (14 casos, logica de estado visible)
- [ ] Crear `__tests__/components/CompletarTareaDialog.test.tsx` (13 casos, form + validacion Zod)
- [ ] Crear `__tests__/pages/MisTareasPage.test.tsx` (10 casos, integracion de pagina completa)
- [ ] Verificar que todos los imports de shared types resuelven correctamente via alias `@shared`
- [ ] Verificar que `vi.mock` esta hoisted correctamente en cada archivo
- [ ] Ejecutar `npm run test:coverage` y verificar 80%+ en todos los archivos nuevos
- [ ] Confirmar que los tests pasan en menos de 60 segundos (target del proyecto)
