# Estrategia de Testing: cp-tracking-metricas (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/web (Vite + React Landing)
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 38 | 85% |
| Integration Tests | 16 | 78% |
| Total | 54 | 80%+ |

**Tiempo estimado ejecucion:** < 55 segundos

**Herramientas:**
- Vitest (test runner)
- @testing-library/react (component testing)
- @testing-library/user-event (user interactions)
- @testing-library/react (renderHook para hooks)
- @tanstack/react-query (query testing utilities)

**Nota sobre mocking:** El proyecto no usa MSW para tests de hooks/services. Sigue el patron `vi.mock("@/lib/api-client")` para service tests y `vi.mock("../infrastructure/tracking.service")` para tests de hooks y pages. Este patron ya esta establecido en `crowdpromotion` y es el que se debe seguir aqui.

---

## 2. Estructura de Tests

```
src/web/src/features/crowdpromotion/
├── tracking/
│   ├── domain/
│   │   └── index.ts
│   ├── infrastructure/
│   │   └── tracking.service.ts
│   └── application/
│       └── hooks/
│           ├── useTrackingInterceptor.ts
│           ├── useMetricasPromotor.ts
│           └── useMisProgramasPromotor.ts  (selector de programa)
├── metricas/
│   └── presentation/
│       ├── pages/
│       │   └── PromotorMetricasPage.tsx
│       └── components/
│           ├── KpiCard.tsx
│           ├── EnlaceReferido.tsx
│           ├── EventosRecientesList.tsx
│           └── EventoBadge.tsx
├── __mocks__/
│   └── tracking.mock.ts          NUEVO
└── __tests__/
    ├── hooks/
    │   ├── useTrackingInterceptor.test.ts   NUEVO
    │   ├── useMetricasPromotor.test.ts      NUEVO
    │   └── useMisProgramasPromotor.test.ts  NUEVO
    ├── components/
    │   ├── KpiCard.test.tsx                 NUEVO
    │   ├── EnlaceReferido.test.tsx          NUEVO
    │   ├── EventosRecientesList.test.tsx    NUEVO
    │   └── EventoBadge.test.tsx             NUEVO
    ├── pages/
    │   └── PromotorMetricasPage.test.tsx    NUEVO
    └── services/
        └── tracking.service.test.ts         NUEVO
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/tracking.mock.ts`

```typescript
import type {
    PromotorMetricasKpis,
    EventoReciente,
    PromotorMetricasResponse,
    TipoEventoPromo,
} from '@shared/types/crowdpromotion'
import type { MiProgramaInscripcionItem } from '../domain'

// ========== KPIs del Promotor ==========

export const mockPromotorMetricasKpis: PromotorMetricasKpis = {
    misClicks: 450,
    misPageViews: 320,
    misSignups: 18,
    misConversiones: 5,
    miValorGenerado: 500.0,
    miComisionAcumulada: 50.0,
    monedaNombre: 'EUR',
    miTasaConversion: 1.11,
}

export const mockPromotorMetricasKpis_SinActividad: PromotorMetricasKpis = {
    misClicks: 0,
    misPageViews: 0,
    misSignups: 0,
    misConversiones: 0,
    miValorGenerado: 0,
    miComisionAcumulada: 0,
    monedaNombre: null,
    miTasaConversion: 0,
}

// ========== Eventos Recientes ==========

export const mockEventoReciente_Backing: EventoReciente = {
    id: 'evt-001',
    tipoEventoId: 4 as TipoEventoPromo,
    tipoEventoNombre: 'Backing',
    valorMonetario: 100.0,
    comisionGenerada: 10.0,
    monedaNombre: 'EUR',
    fechaEvento: '2026-03-20T14:30:00Z',
}

export const mockEventoReciente_Click: EventoReciente = {
    id: 'evt-002',
    tipoEventoId: 1 as TipoEventoPromo,
    tipoEventoNombre: 'Click',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-20T12:15:00Z',
}

export const mockEventoReciente_PageView: EventoReciente = {
    id: 'evt-003',
    tipoEventoId: 2 as TipoEventoPromo,
    tipoEventoNombre: 'PageView',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-19T18:42:00Z',
}

export const mockEventoReciente_Signup: EventoReciente = {
    id: 'evt-004',
    tipoEventoId: 3 as TipoEventoPromo,
    tipoEventoNombre: 'Signup',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-18T09:00:00Z',
}

export const mockEventoReciente_Share: EventoReciente = {
    id: 'evt-005',
    tipoEventoId: 5 as TipoEventoPromo,
    tipoEventoNombre: 'Share',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-17T11:00:00Z',
}

export const mockEventosRecientes: EventoReciente[] = [
    mockEventoReciente_Backing,
    mockEventoReciente_Click,
    mockEventoReciente_PageView,
]

// ========== Response Completa del Promotor ==========

export const mockPromotorMetricasResponse: PromotorMetricasResponse = {
    promotorId: 'promotor-001',
    promotorNombre: 'DJ Mark',
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    fechaDesde: '2026-01-01',
    fechaHasta: '2026-03-31',
    kpis: mockPromotorMetricasKpis,
    eventosRecientes: mockEventosRecientes,
}

export const mockPromotorMetricasResponse_SinEventos: PromotorMetricasResponse = {
    promotorId: 'promotor-001',
    promotorNombre: 'DJ Mark',
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    fechaDesde: '2026-01-01',
    fechaHasta: '2026-03-31',
    kpis: mockPromotorMetricasKpis_SinActividad,
    eventosRecientes: [],
}

// ========== Programas para el selector ==========

export const mockMiProgramaParaSelector = {
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    codigoReferido: 'album-2026-x7k9m',
    urlTrackingPersonalizada:
        'https://weplay.com/campanias/xxx?ref=album-2026-x7k9m&utm_source=weplay&utm_medium=referral&utm_campaign=album-2026',
    estado: 'Aprobado',
}

export const mockMiProgramaSegundo = {
    programaId: 'prog-002',
    programaTitulo: 'Difunde mi EP de verano',
    codigoReferido: 'ep-verano-2026',
    urlTrackingPersonalizada:
        'https://weplay.com/campanias/yyy?ref=ep-verano-2026&utm_source=weplay&utm_medium=referral&utm_campaign=ep-verano',
    estado: 'Aprobado',
}

export const mockListaProgramasPromotor = [
    mockMiProgramaParaSelector,
    mockMiProgramaSegundo,
]

// ========== Respuesta del servicio de tracking (POST) ==========

export const mockRegistrarEventoResponse = {
    eventoId: 'b2c3d4e5-f6a7-8901-bc23-de45fa678901',
    registrado: true,
}
```

### 3.2 Test Utilities

El proyecto ya tiene el patron `createWrapper` con `QueryClientProvider` en cada archivo de tests de hooks (sin un helper centralizado). Se sigue el mismo patron:

```typescript
// Patron a usar en cada archivo de test de hooks
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
```

Para tests de pages que necesitan routing:

```typescript
function renderPage(programaId?: string) {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    const initialPath = programaId
        ? `/promotor/metricas?programaId=${programaId}`
        : '/promotor/metricas'

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={[initialPath]}>
                <PromotorMetricasPage />
            </MemoryRouter>
        </QueryClientProvider>
    )
}
```

---

## 4. Tests por Modulo

### 4.1 Hook: useTrackingInterceptor

**Archivo:** `__tests__/hooks/useTrackingInterceptor.test.ts`

**Descripcion:** Hook critico que se ejecuta al montar la app. Lee parametros de URL, persiste en sessionStorage/cookie y llama al endpoint de tracking. No tiene interfaz visual. Los errores son silenciosos.

**Mocks necesarios:**
- `vi.mock("../tracking/infrastructure/tracking.service")` para `trackingService.registrarEvento`
- `window.location` via `Object.defineProperty` o `vi.stubGlobal`
- `sessionStorage` via mock del objeto global
- `document.cookie` para verificar escritura de cookie

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| no llama al servicio si no hay param ref ni cookie | Unit | Si la URL no tiene `?ref=` y sessionStorage esta vacio, el hook no llama al API |
| detecta param ref y llama al servicio con tipo Click | Unit | URL con `?ref=album-2026-x7k9m` genera llamada a `trackingService.registrarEvento` con `tipoEventoPromoId: 1` |
| persiste ref en sessionStorage al detectarlo en URL | Unit | Tras el mount, `sessionStorage.getItem('wp_ref')` devuelve el codigo referido |
| persiste utm_source en sessionStorage | Unit | `sessionStorage.getItem('wp_utm_source')` devuelve 'weplay' |
| persiste utm_medium en sessionStorage | Unit | `sessionStorage.getItem('wp_utm_medium')` devuelve 'referral' |
| persiste utm_campaign en sessionStorage | Unit | `sessionStorage.getItem('wp_utm_campaign')` devuelve el valor de la URL |
| escribe cookie wp_ref al detectar ref | Unit | `document.cookie` incluye `wp_ref=album-2026-x7k9m` tras el mount |
| envia urlOrigen en el payload de tracking | Unit | El payload incluye `urlOrigen` con `window.location.href` |
| envia urlReferer en el payload de tracking | Unit | El payload incluye `urlReferer` con `document.referrer` |
| maneja 429 silenciosamente sin reintentar | Unit | Si el servicio rechaza con error code 4032, no se vuelve a llamar |
| maneja error 400 y limpia sessionStorage | Unit | Si el servicio rechaza con error code 1xxx, se limpia `sessionStorage` |
| maneja error generico silenciosamente | Unit | Si el servicio lanza un error de red, el hook no propaga el error |
| no llama al servicio si ref ya fue procesado en esta sesion y no hay nuevo param | Unit | Si sessionStorage tiene `wp_ref` pero la URL no tiene `?ref=`, no llama al tracking (atribucion via sesion, no reregistra Click) |
| llama al servicio solo una vez por mount | Unit | El useEffect se ejecuta solo al montar (dependencias vacias) |

**Casos Detallados:**

```markdown
1. **no llama al servicio si no hay param ref ni cookie**
   - Setup: URL = '/campanias/123', sessionStorage vacio
   - Mount hook con renderHook
   - Assert: trackingService.registrarEvento no fue llamado

2. **detecta param ref y llama al servicio con tipo Click**
   - Setup: URL = '/campanias/123?ref=album-2026-x7k9m&utm_source=weplay'
   - Mount hook con renderHook
   - Assert: trackingService.registrarEvento fue llamado con:
     - tipoEventoPromoId: 1
     - codigoReferido: 'album-2026-x7k9m'
     - utmSource: 'weplay'

3. **persiste ref en sessionStorage al detectarlo en URL**
   - Setup: URL con ?ref=album-2026-x7k9m
   - Mount hook
   - Assert: sessionStorage.setItem fue llamado con ('wp_ref', 'album-2026-x7k9m')

4. **maneja 429 silenciosamente sin reintentar**
   - Setup: URL con ?ref=test, trackingService.registrarEvento rechaza con { errorCode: '4032' }
   - Mount hook
   - Assert: registrarEvento fue llamado exactamente 1 vez
   - Assert: no hay unhandled error, el hook termina silenciosamente
```

---

### 4.2 Hook: useMetricasPromotor

**Archivo:** `__tests__/hooks/useMetricasPromotor.test.ts`

**Descripcion:** Hook useQuery que obtiene las metricas del promotor desde el API. Acepta `programaId` como parametro. Usa `QUERY_KEYS.crowdpromotion.metricas.promotor`.

**Mocks necesarios:**
- `vi.mock("../tracking/infrastructure/tracking.service")` para `trackingService.getMetricasPromotor`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna datos correctamente en exito | Unit | `isSuccess=true` y `data` igual a `mockPromotorMetricasResponse` |
| isLoading es true inicialmente | Unit | Estado inicial del hook es loading |
| isError es true cuando el servicio falla | Unit | Cuando el servicio rechaza, `isError=true` |
| llama al servicio con el programaId correcto | Unit | El servicio recibe el `programaId` pasado al hook |
| no realiza fetch cuando programaId es undefined | Unit | `fetchStatus === 'idle'` cuando no hay programaId |
| no realiza fetch cuando programaId es string vacio | Unit | `fetchStatus === 'idle'` cuando programaId es '' |
| retorna kpis con valores cero cuando sin actividad | Unit | `data.kpis.misClicks === 0` cuando el promotor no tiene actividad |
| retorna eventosRecientes como array vacio cuando sin eventos | Unit | `data.eventosRecientes.length === 0` en respuesta sin eventos |

---

### 4.3 Hook: useMisProgramasPromotor

**Archivo:** `__tests__/hooks/useMisProgramasPromotor.test.ts`

**Descripcion:** Hook useQuery que obtiene los programas activos del promotor para poblar el selector de programa en `PromotorMetricasPage`. Reutiliza o adapta la logica de `useMisProgramas` filtrando por estado Aprobado.

**Mocks necesarios:**
- `vi.mock("../../infrastructure/inscripcion.service")` para `inscripcionService.misProgramas`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna lista de programas aprobados | Unit | Data contiene los programas con estado Aprobado |
| isLoading es true inicialmente | Unit | Estado inicial es loading |
| isError es true cuando el servicio falla | Unit | Error propagado correctamente |
| retorna lista vacia cuando no hay programas aprobados | Unit | `data` es array vacio si no hay programas |
| cada item tiene codigoReferido y urlTrackingPersonalizada | Unit | Verifica estructura de los items para el selector |

---

### 4.4 Componente: KpiCard

**Archivo:** `__tests__/components/KpiCard.test.tsx`

**Descripcion:** Componente presentacional que muestra un KPI individual con icono, valor y label. Soporta variantes de color para el icono y para el valor (numerico vs monetario).

**Props esperadas:**
```typescript
interface KpiCardProps {
    label: string
    value: number | string
    icon: ReactNode
    iconBgClass: string      // Clase CSS para el fondo del icono
    valueColorClass?: string // Clase CSS para el color del valor (default: text-white)
}
```

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza el label correctamente | Unit | El texto del label aparece en el DOM |
| renderiza el valor numerico | Unit | El valor `450` se muestra como texto |
| renderiza el valor monetario con sufijo EUR | Unit | `50 EUR` con sufijo correctamente separado |
| aplica la clase de color al contenedor del icono | Unit | El container del icono tiene la clase `iconBgClass` |
| aplica valueColorClass cuando se proporciona | Unit | El valor tiene la clase de color especificada |
| usa text-white por defecto para el valor | Unit | Sin `valueColorClass`, el valor tiene clase `text-white` |
| renderiza con valor cero sin errores | Unit | `value=0` se renderiza como "0" sin crash |

---

### 4.5 Componente: EnlaceReferido

**Archivo:** `__tests__/components/EnlaceReferido.test.tsx`

**Descripcion:** Componente que muestra el enlace referido del promotor con un boton "Copiar". Al hacer click, copia la URL al portapapeles y cambia el boton a "Copiado!" durante 2000ms. Si la Clipboard API falla, muestra un toast de error.

**Props esperadas:**
```typescript
interface EnlaceReferidoProps {
    url: string
}
```

**Mocks necesarios:**
- `navigator.clipboard.writeText` via `Object.defineProperty`
- `vi.mock("sonner")` para el toast de error en fallback

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza el input con la URL correcta | Unit | El input readonly muestra la URL del enlace |
| muestra el boton "Copiar" en estado inicial | Unit | El boton contiene el texto "Copiar" |
| click en Copiar llama a navigator.clipboard.writeText con la URL | Integration | Verifica que se copia la URL correcta |
| boton cambia a "Copiado!" despues del click | Integration | Tras el click exitoso, el texto del boton es "Copiado!" |
| boton vuelve a "Copiar" despues de 2000ms | Integration | Usando vi.useFakeTimers, verificar que tras 2000ms vuelve a "Copiar" |
| boton tiene aria-label dinamico "Enlace copiado" cuando copiado | Unit | `aria-label` cambia tras el click |
| muestra toast de error si Clipboard API falla | Integration | Si `writeText` rechaza, se llama a `toast.error` |
| el input tiene atributo readOnly | Unit | El input no es editable por el usuario |
| el input tiene aria-label="Tu enlace de promocion" | Unit | Accesibilidad correcta |

**Casos Detallados:**

```markdown
1. **click en Copiar llama a navigator.clipboard.writeText**
   - Setup: navigator.clipboard.writeText = vi.fn().mockResolvedValue(undefined)
   - Render: <EnlaceReferido url="https://weplay.com/campanias/xxx?ref=album-2026-x7k9m" />
   - Act: await user.click(screen.getByRole('button', { name: /Copiar/i }))
   - Assert: navigator.clipboard.writeText toHaveBeenCalledWith('https://weplay.com/...')

2. **boton vuelve a Copiar despues de 2000ms**
   - Setup: vi.useFakeTimers(), navigator.clipboard.writeText resuelve OK
   - Render el componente
   - Click en Copiar
   - Assert: texto es "Copiado!" inmediatamente
   - Act: vi.advanceTimersByTime(2000)
   - Assert: texto vuelve a "Copiar"
   - Teardown: vi.useRealTimers()

3. **muestra toast de error si Clipboard API falla**
   - Setup: navigator.clipboard.writeText = vi.fn().mockRejectedValue(new Error('denied'))
   - Click en Copiar
   - Assert: toast.error fue llamado con mensaje de error
```

---

### 4.6 Componente: EventoBadge

**Archivo:** `__tests__/components/EventoBadge.test.tsx`

**Descripcion:** Componente presentacional que renderiza un badge con el tipo de evento. Aplica estilos de color especificos por tipo de evento segun el design system.

**Props esperadas:**
```typescript
interface EventoBadgeProps {
    tipoEventoId: TipoEventoPromo   // 1 | 2 | 3 | 4 | 5
}
```

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza "Click" para tipoEventoId=1 | Unit | El texto del badge es "Click" |
| renderiza "Vista" para tipoEventoId=2 | Unit | El texto del badge es "Vista" (label UI de PageView) |
| renderiza "Registro" para tipoEventoId=3 | Unit | El texto del badge es "Registro" (label UI de Signup) |
| renderiza "Backing" para tipoEventoId=4 | Unit | El texto del badge es "Backing" |
| renderiza "Share" para tipoEventoId=5 | Unit | El texto del badge es "Share" |
| aplica estilos azules para Click (tipo 1) | Unit | El badge tiene clase con 'blue' en su className |
| aplica estilos slate/gris para PageView (tipo 2) | Unit | El badge tiene clase con 'slate' en su className |
| aplica estilos verdes para Signup (tipo 3) | Unit | El badge tiene clase con 'green' en su className |
| aplica estilos purpura para Backing (tipo 4) | Unit | El badge tiene clase con 'purple' en su className |
| aplica estilos ambar para Share (tipo 5) | Unit | El badge tiene clase con 'amber' en su className |

**Patron de test para colores (consistente con InscripcionEstadoBadge.test.tsx existente):**

```markdown
- Usar container.querySelector("[class*='blue']") para verificar que existe un elemento
  con clase que contiene 'blue'
- Alternativa preferida si el componente usa aria-label: verificar aria-label='Tipo: Click'
```

---

### 4.7 Componente: EventosRecientesList

**Archivo:** `__tests__/components/EventosRecientesList.test.tsx`

**Descripcion:** Lista de eventos recientes del promotor. Cada fila muestra el badge de tipo, la descripcion del evento, el valor monetario y la comision si es un Backing, y la fecha formateada.

**Props esperadas:**
```typescript
interface EventosRecientesListProps {
    eventos: EventoReciente[]
    isLoading: boolean
}
```

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza lista de eventos correctamente | Unit | Se muestran todos los eventos del array |
| muestra badge de tipo por cada evento | Unit | Cada evento tiene su badge correspondiente |
| muestra valor monetario y comision para eventos Backing | Unit | "100 EUR - 10 EUR comision" visible para tipo 4 |
| no muestra valor monetario para eventos no-Backing | Unit | Click y PageView no muestran importe |
| muestra fecha formateada para cada evento | Unit | La fecha '2026-03-20T14:30:00Z' se muestra legible |
| muestra empty state cuando el array esta vacio | Unit | Mensaje "Aun no hay eventos registrados" cuando eventos=[] |
| el empty state tiene el icono de Activity | Unit | Icono en el estado vacio |
| muestra skeletons cuando isLoading es true | Unit | Skeleton rows durante carga |
| renderiza el enlace "Ver todos" | Unit | Boton/link "Ver todos" presente en la cabecera |

**Casos Detallados:**

```markdown
1. **muestra valor monetario y comision para eventos Backing**
   - Render con [mockEventoReciente_Backing] (tipoEventoId=4, valorMonetario=100, comisionGenerada=10)
   - Assert: screen.getByText(/100.*EUR/) visible
   - Assert: screen.getByText(/10.*EUR.*comision/) visible

2. **no muestra valor monetario para eventos no-Backing**
   - Render con [mockEventoReciente_Click] (tipoEventoId=1)
   - Assert: screen.queryByText(/EUR/) not toBeInTheDocument

3. **muestra empty state cuando el array esta vacio**
   - Render con eventos=[], isLoading=false
   - Assert: screen.getByText(/Aun no hay eventos registrados/) toBeInTheDocument
```

---

### 4.8 Page: PromotorMetricasPage

**Archivo:** `__tests__/pages/PromotorMetricasPage.test.tsx`

**Descripcion:** Pagina completa del dashboard del promotor. Integra el selector de programa, las 3 KPI cards, la tasa de conversion inline, la seccion de enlace referido y la lista de eventos recientes. Cada cambio en el selector provoca un refetch.

**Mocks necesarios:**
- `vi.mock("sonner")` para toast
- `vi.mock("../tracking/infrastructure/tracking.service")` para `trackingService.getMetricasPromotor`
- `vi.mock("../../infrastructure/inscripcion.service")` para `inscripcionService.misProgramas` (selector de programa)
- `Object.defineProperty(navigator, 'clipboard', ...)` para el boton copiar

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza el titulo de la pagina | Unit | "Mis metricas de promocion" visible al cargar |
| muestra skeletons durante la carga inicial | Unit | 3 KPI cards skeleton con animate-pulse durante loading |
| renderiza KPI cards con datos del promotor tras carga | Integration | Clicks=450, Conversiones=5, Comision=50 EUR visibles |
| muestra tasa de conversion correctamente | Integration | "1.11%" visible en la pagina |
| muestra el selector de programa con opciones cargadas | Integration | El Select muestra el titulo del programa activo |
| renderiza la seccion EnlaceReferido con la URL correcta | Integration | El input de enlace referido tiene la URL del mock |
| renderiza la lista EventosRecientesList con eventos | Integration | Los 3 eventos del mock estan en el DOM |
| muestra empty state cuando el promotor no tiene programas | Integration | Icono BarChart2 y boton "Ver programas disponibles" |
| muestra empty state de eventos cuando eventosRecientes esta vacio | Integration | Mensaje de sin eventos en la seccion de eventos |
| muestra estado de error con boton Reintentar | Integration | Card de error cuando el servicio falla |
| el boton Reintentar hace refetch de las metricas | Integration | Tras click en Reintentar, el servicio es llamado de nuevo |
| cambiar programa en el selector provoca nuevo fetch | Integration | Al seleccionar otro programa, se llama al servicio con nuevo programaId |
| el selector actualiza el query param programaId en la URL | Integration | URL cambia a `?programaId=prog-002` tras la seleccion |
| click en Copiar enlace funciona correctamente | Integration | navigator.clipboard.writeText llamado con la URL del enlace |

**Casos Detallados:**

```markdown
1. **renderiza KPI cards con datos del promotor tras carga**
   - Mock: inscripcionService.misProgramas -> lista con 1 programa Aprobado
   - Mock: trackingService.getMetricasPromotor -> mockPromotorMetricasResponse
   - Render la pagina
   - waitFor: screen.getByText('450') visible (misClicks)
   - Assert: screen.getByText('5') visible (misConversiones)
   - Assert: screen.getByText(/50/) y screen.getByText('EUR') visibles (comision)

2. **muestra empty state cuando el promotor no tiene programas**
   - Mock: inscripcionService.misProgramas -> { items: [], totalCount: 0 }
   - Render la pagina
   - waitFor: screen.getByText(/Sin programas activos/) visible
   - Assert: screen.getByRole('button', { name: /Ver programas disponibles/i }) visible

3. **cambiar programa en el selector provoca nuevo fetch**
   - Mock: 2 programas en la lista, datos de metricas por programa
   - Render pagina con programaId=prog-001
   - waitFor: datos del programa 1 visibles
   - Act: user.selectOptions o user.click en el Select con prog-002
   - Assert: trackingService.getMetricasPromotor fue llamado con 'prog-002'
```

---

### 4.9 Service: tracking.service

**Archivo:** `__tests__/services/tracking.service.test.ts`

**Descripcion:** Service de infraestructura que encapsula las llamadas al API de tracking y metricas. Sigue el patron del proyecto: mock de `apiFetch` y verificacion de URL y payload.

**Mocks necesarios:**
- `vi.mock("@/lib/api-client")` para `apiFetch`

#### Metodo: registrarEvento

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna RegistrarEventoResponse en exito | Unit | `{ eventoId: '...', registrado: true }` |
| llama al endpoint correcto con POST | Unit | URL contiene `/crowdpromotion/tracking/evento` con method POST |
| envia el payload completo en el body | Unit | `codigoReferido`, `tipoEventoPromoId`, UTMs en el body |
| envia payload minimo con solo tipoEventoPromoId | Unit | Funciona con solo el campo obligatorio |
| no lanza error en 429 (silent handling) | Unit | Si el API devuelve mensajes con errorCode 4032, el service retorna sin lanzar (o lanza y el hook captura) |
| lanza error con errorCode 1033 para tipo invalido | Unit | Error propagado con errorCode '1033' |
| lanza error de red correctamente | Unit | Error de red propagado |

#### Metodo: getMetricasPromotor

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna PromotorMetricasResponse en exito | Unit | Response mapeado correctamente |
| llama al endpoint correcto con programaId en query | Unit | URL contiene `/crowdpromotion/promotor/metricas?programaId=prog-001` |
| llama al endpoint sin programaId cuando es undefined | Unit | URL es `/crowdpromotion/promotor/metricas` sin query param |
| lanza error 401 cuando no autenticado | Unit | Error con errorCode '3001' |
| lanza error 403 cuando no tiene acceso | Unit | Error con errorCode '4026' |
| lanza error generico en 500 | Unit | Error con errorCode '5000' |

**Casos Detallados:**

```markdown
1. **retorna RegistrarEventoResponse en exito**
   - Mock: apiFetch retorna { data: mockRegistrarEventoResponse, messages: [{ message: 'OK', errorCode: '0001' }] }
   - Act: await trackingService.registrarEvento({ tipoEventoPromoId: 1, codigoReferido: 'test-ref' })
   - Assert: result.eventoId toBe 'b2c3d4e5-...'
   - Assert: result.registrado toBe true

2. **llama al endpoint correcto con POST**
   - Mock: apiFetch resuelve con datos validos
   - Act: await trackingService.registrarEvento({ tipoEventoPromoId: 1 })
   - Assert: apiFetch llamado con:
     - URL que contiene '/crowdpromotion/tracking/evento'
     - { method: 'POST' }

3. **no lanza error en 429**
   - Mock: apiFetch retorna { data: null, messages: [{ message: 'Rate limit', errorCode: '4032' }] }
   - El servicio puede lanzar un error especial con errorCode '4032'
   - El hook useTrackingInterceptor lo captura silenciosamente en su try/catch
   - Assert: el error tiene errorCode '4032'

4. **llama al endpoint correcto con programaId en query**
   - Mock: apiFetch resuelve con mockPromotorMetricasResponse
   - Act: await trackingService.getMetricasPromotor('prog-001')
   - Assert: apiFetch llamado con URL que contiene 'programaId=prog-001'
```

---

### 4.10 Utilities: mappers de tracking (shared)

**Archivo:** `src/shared/utils/__tests__/tracking.mappers.test.ts`

**Nota:** Aunque este archivo esta en `src/shared`, lo planificamos aqui porque los mappers son consumidos directamente por los componentes de Landing de esta feature. Son funciones puras, ideales para tests unitarios rapidos.

**Funciones a testear:** `mapTipoEventoPromoToLabel`, `mapTipoEventoPromoToBadgeClass`, `formatTasaConversion`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| mapTipoEventoPromoToLabel: 1 devuelve "Click" | Unit | Label correcto para Click |
| mapTipoEventoPromoToLabel: 2 devuelve "Vista" | Unit | Label UI de PageView |
| mapTipoEventoPromoToLabel: 3 devuelve "Registro" | Unit | Label UI de Signup |
| mapTipoEventoPromoToLabel: 4 devuelve "Backing" | Unit | Label correcto para Backing |
| mapTipoEventoPromoToLabel: 5 devuelve "Share" | Unit | Label correcto para Share |
| mapTipoEventoPromoToLabel: tipo desconocido devuelve "Evento" | Unit | Fallback para tipo fuera de rango |
| mapTipoEventoPromoToBadgeClass: 1 contiene 'blue' | Unit | Badge azul para Click |
| mapTipoEventoPromoToBadgeClass: 2 contiene 'slate' | Unit | Badge gris para PageView |
| mapTipoEventoPromoToBadgeClass: 3 contiene 'green' | Unit | Badge verde para Signup |
| mapTipoEventoPromoToBadgeClass: 4 contiene 'purple' | Unit | Badge purpura para Backing |
| mapTipoEventoPromoToBadgeClass: 5 contiene 'amber' | Unit | Badge ambar para Share |
| mapTipoEventoPromoToBadgeClass: tipo desconocido devuelve clase fallback slate | Unit | Fallback para tipo desconocido |
| formatTasaConversion: 1.11 devuelve "1.11%" | Unit | Formato correcto con 2 decimales |
| formatTasaConversion: 0 devuelve "0.00%" | Unit | Cero formateado correctamente |
| formatTasaConversion: 100 devuelve "100.00%" | Unit | Maximo posible |
| formatTasaConversion: valor falsy devuelve "0.00%" | Unit | Defensive para null/undefined |

---

## 5. Patrones de Mock Especificos para esta Feature

### 5.1 Mock de sessionStorage

```typescript
// En beforeEach de useTrackingInterceptor.test.ts
const mockSessionStorage = {
    data: {} as Record<string, string>,
    getItem: vi.fn((key: string) => mockSessionStorage.data[key] ?? null),
    setItem: vi.fn((key: string, value: string) => {
        mockSessionStorage.data[key] = value
    }),
    removeItem: vi.fn((key: string) => {
        delete mockSessionStorage.data[key]
    }),
    clear: vi.fn(() => {
        mockSessionStorage.data = {}
    }),
}

Object.defineProperty(window, 'sessionStorage', {
    value: mockSessionStorage,
    writable: true,
})
```

### 5.2 Mock de window.location.search

```typescript
// Para tests del interceptor que necesitan URL especifica
Object.defineProperty(window, 'location', {
    value: {
        ...window.location,
        search: '?ref=album-2026-x7k9m&utm_source=weplay&utm_medium=referral&utm_campaign=album-2026',
        href: 'https://weplay.com/campanias/xxx?ref=album-2026-x7k9m',
    },
    writable: true,
})

// Restaurar en afterEach
afterEach(() => {
    Object.defineProperty(window, 'location', {
        value: { ...window.location, search: '', href: 'http://localhost/' },
        writable: true,
    })
})
```

### 5.3 Mock de navigator.clipboard

```typescript
// En beforeEach de EnlaceReferido.test.tsx y PromotorMetricasPage.test.tsx
beforeEach(() => {
    Object.defineProperty(navigator, 'clipboard', {
        value: { writeText: vi.fn().mockResolvedValue(undefined) },
        writable: true,
        configurable: true,
    })
})
```

### 5.4 Patron para mocks de servicios (consistente con el proyecto)

```typescript
// CORRECTO - patron del proyecto (sin MSW para service/hook tests)
vi.mock("../tracking/infrastructure/tracking.service", () => ({
    trackingService: {
        registrarEvento: vi.fn(),
        getMetricasPromotor: vi.fn(),
    },
}))

import { trackingService } from "../tracking/infrastructure/tracking.service"

// En cada test:
vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(mockPromotorMetricasResponse)
```

### 5.5 Mock de sonner para tests que verifican toasts

```typescript
vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

import { toast } from "sonner"
// Assert: expect(toast.error).toHaveBeenCalledWith(expect.stringContaining('No se pudo copiar'))
```

---

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| useTrackingInterceptor.ts | 90% | 100% | 85% |
| useMetricasPromotor.ts | 90% | 100% | 80% |
| useMisProgramasPromotor.ts | 85% | 100% | 80% |
| tracking.service.ts | 95% | 100% | 90% |
| PromotorMetricasPage.tsx | 80% | 90% | 75% |
| KpiCard.tsx | 95% | 100% | 90% |
| EnlaceReferido.tsx | 90% | 100% | 85% |
| EventoBadge.tsx | 100% | 100% | 100% |
| EventosRecientesList.tsx | 90% | 100% | 85% |
| mappers.ts (nuevas funciones) | 100% | 100% | 100% |

**Meta Global:** 80% en todas las metricas

---

## 7. Edge Cases Criticos

| Escenario | Componente/Hook | Test |
|-----------|-----------------|------|
| URL sin ningun param de tracking | useTrackingInterceptor | No llama al servicio |
| URL con UTMs pero sin `ref` | useTrackingInterceptor | No llama al servicio (FA-05: sin ref, sin tracking automatico de Click) |
| 429 del endpoint de tracking | useTrackingInterceptor | Error silencioso, no reintenta |
| 400 del endpoint de tracking | useTrackingInterceptor | Limpia sessionStorage, silencioso |
| Promotor sin programas aprobados | PromotorMetricasPage | Empty state correcto |
| Promotor con 0 clicks (division por cero en tasa) | KpiCard / Page | Muestra "0.00%" sin NaN ni Infinity |
| Clipboard API no disponible (HTTP context) | EnlaceReferido | Fallback con toast de error |
| Boton Copiar: timeout de 2s | EnlaceReferido | Vuelve a estado "Copiar" exactamente a 2s |
| Lista de eventos vacia | EventosRecientesList | Empty state con mensaje correcto |
| Tipo de evento desconocido (future-proof) | EventoBadge | Fallback a badge gris |
| programaId undefined en useMetricasPromotor | useMetricasPromotor | Query deshabilitada (enabled=false) |
| Error 401 en getMetricasPromotor | PromotorMetricasPage | Estado de error con Reintentar |
| Cambio rapido de programa (race condition) | PromotorMetricasPage | TanStack Query maneja correctamente con nueva queryKey |

---

## 8. Flujo de Integracion Critico: Tracking Interceptor

El flujo mas critico de esta feature es la cadena: **URL con ref -> interceptor -> sessionStorage -> API**. Los tests de integracion deben cubrir este flujo end-to-end dentro del scope del frontend.

**Flujo a testear:**

```
1. URL contiene ?ref=CODIGO&utm_source=weplay
   └─> useTrackingInterceptor detecta parametros
       └─> persiste en sessionStorage (wp_ref, wp_utm_source, etc.)
       └─> persiste cookie wp_ref
       └─> llama trackingService.registrarEvento({ tipoEventoPromoId: 1, codigoReferido: 'CODIGO', ... })
           └─> API responde 201 -> flujo completado silenciosamente
           └─> API responde 429 -> silencioso, sessionStorage conservado
           └─> API responde 400 -> limpia sessionStorage
```

Tests de integracion especificos para este flujo:

| Test Case | Descripcion |
|-----------|-------------|
| flujo completo con ref valido | URL con ref -> sessionStorage poblado -> API llamado con payload correcto |
| flujo con 429 conserva sessionStorage | 429 no debe limpiar sessionStorage (el ref sigue atribuido en sesion) |
| flujo con 400 limpia sessionStorage | 400 debe limpiar todas las claves de tracking en sessionStorage |

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests (desde src/web)
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar solo tests de esta feature
npm run test -- crowdpromotion/tracking
npm run test -- crowdpromotion/metricas

# Watch mode durante desarrollo
npm run test:watch

# UI mode
npm run test:ui
```

---

## 10. CI/CD Integration

```yaml
- name: Run Frontend Tests (Landing)
  working-directory: ./src/web
  run: npm run test:coverage

- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    files: ./src/web/coverage/coverage-final.json
    flags: frontend-landing
```

---

## 11. Prioridades de Implementacion

### Fase 1 - Critical Path (2 horas)

Tests del nucleo funcional que generan mas valor y riesgo:

1. `__mocks__/tracking.mock.ts` - Mock data completo
2. `useTrackingInterceptor.test.ts` - Hook critico invisible (14 tests)
3. `tracking.service.test.ts` - Service con todos los endpoints (13 tests)
4. `EventoBadge.test.tsx` - Componente puro, tests rapidos (10 tests)

### Fase 2 - Core UI (2 horas)

Tests de los componentes y hooks de la pagina principal:

5. `useMetricasPromotor.test.ts` - Hook de datos principal (8 tests)
6. `KpiCard.test.tsx` - Componente KPI (7 tests)
7. `EnlaceReferido.test.tsx` - Componente con interaccion critica (9 tests)
8. `EventosRecientesList.test.tsx` - Lista de eventos (9 tests)

### Fase 3 - Integration Page (1.5 horas)

9. `PromotorMetricasPage.test.tsx` - Tests de integracion de la pagina completa (14 tests)
10. `useMisProgramasPromotor.test.ts` - Hook selector de programa (5 tests)

### Fase 4 - Shared Utilities (30 min)

11. `tracking.mappers.test.ts` - Funciones puras en shared (16 tests)

**Total estimado implementacion:** 6 horas

---

## 12. Checklist

- [ ] Mock data definido en `__mocks__/tracking.mock.ts`
- [ ] Tests de `useTrackingInterceptor` completos (14 tests)
- [ ] Mock de `sessionStorage` y `window.location` configurados
- [ ] Tests de `tracking.service` completos (13 tests)
- [ ] Tests de `EventoBadge` completos (10 tests)
- [ ] Tests de `useMetricasPromotor` completos (8 tests)
- [ ] Tests de `KpiCard` completos (7 tests)
- [ ] Tests de `EnlaceReferido` con mock de Clipboard API (9 tests)
- [ ] Tests de `EventosRecientesList` completos (9 tests)
- [ ] Tests de integracion de `PromotorMetricasPage` (14 tests)
- [ ] Tests de `useMisProgramasPromotor` (5 tests)
- [ ] Tests de shared mappers (16 tests)
- [ ] Cobertura 80%+ alcanzada
- [ ] Todos los edge cases cubiertos
- [ ] Tests pasan en CI en < 60 segundos

---

## 13. Deuda Tecnica Conocida

| Item | Impacto | Razon |
|------|---------|-------|
| No hay tests de la cookie `wp_ref` en detalle (TTL, SameSite) | Bajo | jsdom no implementa cookie TTL real; verificar solo la escritura del string |
| Grafico Recharts no testeable sin canvas mock | Bajo | El chart esta en Admin (Next.js), no en Landing; fuera de scope de este plan |
| Timer-based tests de EnlaceReferido con fakeTimers | Medio | Requiere cuidado con `vi.useFakeTimers()` y cleanup en afterEach para no contaminar otros tests |
| Tests de accesibilidad (axe-core) | Bajo | No instalado en el proyecto; agregar post-MVP si se quiere coverage de a11y |
