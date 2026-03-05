# Estrategia de Testing: cs-valoraciones (Landing)

**Fecha:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06 - Valoraciones Bidireccionales)
**Target:** `src/web`
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Archivos |
|------|----------|----------|
| Unit Tests | 38 | Components: StarDisplay, RatingBadge, EmptyValoraciones, ValoracionReadOnly, ValoracionListItem |
| Integration Tests | 34 | Components con mutation/query: StarRating, ValoracionForm, RatingHistogram, ValoracionesSection |
| Hook Tests | 13 | useCreateValoracion, useValoracionesUsuario |
| Service Tests | 10 | valoracion.api.test.ts |
| **Total** | **95** | 11 archivos |

**Meta Global:** 80%+ en lineas, funciones y branches en todos los archivos de la feature.

---

## 2. Estructura de Tests

```
src/web/src/features/crowdsourcing/
├── valoraciones/
│   ├── domain/
│   │   └── types.ts                          (interfaces ValoracionDto, ValoracionListItem, etc.)
│   ├── application/
│   │   └── hooks/
│   │       ├── useCreateValoracion.ts
│   │       └── useValoracionesUsuario.ts
│   ├── infrastructure/
│   │   └── api/
│   │       └── valoracion.api.ts
│   └── presentation/
│       └── components/
│           ├── StarRating.tsx
│           ├── StarDisplay.tsx
│           ├── RatingHistogram.tsx
│           ├── ValoracionForm.tsx
│           ├── ValoracionReadOnly.tsx
│           ├── ValoracionesSection.tsx
│           ├── ValoracionListItem.tsx
│           ├── RatingBadge.tsx
│           └── EmptyValoraciones.tsx
├── __tests__/
│   └── components/
│       ├── StarRating.test.tsx
│       ├── StarDisplay.test.tsx
│       ├── RatingHistogram.test.tsx
│       ├── ValoracionForm.test.tsx
│       ├── ValoracionReadOnly.test.tsx
│       ├── ValoracionesSection.test.tsx
│       ├── ValoracionListItem.test.tsx
│       ├── RatingBadge.test.tsx
│       └── EmptyValoraciones.test.tsx
│   └── hooks/
│       ├── useCreateValoracion.test.ts
│       └── useValoracionesUsuario.test.ts
│   └── services/
│       └── valoracion.api.test.ts
└── __mocks__/
    └── valoracion.mock.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/web/src/features/crowdsourcing/__mocks__/valoracion.mock.ts`

#### Fixtures de ValoracionCreatedResult (respuesta del POST)

```typescript
export const mockValoracionCreada: ValoracionCreatedResult = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    puntuacion: 5,
    comentario: "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
    fechaCreacion: "2026-03-16T10:00:00Z",
}

export const mockValoracionCreadaSinComentario: ValoracionCreatedResult = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
    puntuacion: 4,
    comentario: undefined,
    fechaCreacion: "2026-03-16T11:00:00Z",
}
```

#### Fixtures de ValoracionListItem (items en la lista del perfil)

```typescript
export const mockValoracionItem: ValoracionListItem = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    puntuacion: 5,
    comentario: "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
    autorNombre: "Los Rockeros",
    autorImagenUrl: "https://storage.example.com/imagenes/los-rockeros.jpg",
    acuerdoTituloInterno: "Mezcla EP Los Rockeros",
    fechaCreacion: "2026-03-16T10:00:00Z",
}

export const mockValoracionItemSinComentario: ValoracionListItem = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
    puntuacion: 4,
    comentario: undefined,
    autorNombre: "Indie Band",
    autorImagenUrl: null,
    acuerdoTituloInterno: "Mastering Single",
    fechaCreacion: "2026-02-28T16:00:00Z",
}

export const mockValoracionItemSinImagen: ValoracionListItem = {
    ...mockValoracionItem,
    id: "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    autorNombre: "Studio Mix",
    autorImagenUrl: null,
}
```

#### Fixtures de ValoracionResumen

```typescript
export const mockResumenConValoraciones: ValoracionResumen = {
    puntuacionMedia: 4.5,
    totalValoraciones: 12,
    distribucion: {
        5: 7,
        4: 3,
        3: 1,
        2: 1,
        1: 0,
    },
}

export const mockResumenSinValoraciones: ValoracionResumen = {
    puntuacionMedia: null,
    totalValoraciones: 0,
    distribucion: {
        5: 0,
        4: 0,
        3: 0,
        2: 0,
        1: 0,
    },
}

export const mockResumenUnaValoracion: ValoracionResumen = {
    puntuacionMedia: 5.0,
    totalValoraciones: 1,
    distribucion: {
        5: 1,
        4: 0,
        3: 0,
        2: 0,
        1: 0,
    },
}
```

#### Fixtures de ValoracionesUsuario (respuesta GET completa)

```typescript
export const mockValoracionesUsuarioConDatos: ValoracionesUsuario = {
    resumen: mockResumenConValoraciones,
    valoraciones: {
        items: [mockValoracionItem, mockValoracionItemSinComentario],
        totalCount: 12,
        page: 1,
        pageSize: 10,
    },
}

export const mockValoracionesUsuarioVacio: ValoracionesUsuario = {
    resumen: mockResumenSinValoraciones,
    valoraciones: {
        items: [],
        totalCount: 0,
        page: 1,
        pageSize: 10,
    },
}

export const mockValoracionesUsuarioPagina2: ValoracionesUsuario = {
    resumen: mockResumenConValoraciones,
    valoraciones: {
        items: [mockValoracionItemSinImagen],
        totalCount: 12,
        page: 2,
        pageSize: 10,
    },
}
```

#### IDs de prueba reutilizables

```typescript
export const MOCK_ACUERDO_ID = "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b"
export const MOCK_USER_ID = "profesional-user-001"
```

### 3.2 Mock del servicio API

El proyecto mockea el modulo de infraestructura directamente con `vi.mock`. No se usa MSW.
El patron establecido en el proyecto es:

```typescript
vi.mock("../../infrastructure", () => ({
    valoracionApi: {
        create: vi.fn(),
        getByUserId: vi.fn(),
    },
}))
```

O bien, para el test del servicio en si, se mockea `@/lib/api-client`:

```typescript
vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))
```

### 3.3 Mock de Toast (sonner)

```typescript
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))
```

### 3.4 QueryClient para Tests de Hooks

Patron establecido en el proyecto (ver `useAcuerdo.test.ts`, `useEnviarMensaje.test.ts`):

```typescript
function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
            mutations: { retry: false },
        },
    })
}

function createWrapper(queryClient?: QueryClient) {
    const client = queryClient ?? createTestQueryClient()
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client }, children)
    }
}
```

---

## 4. Tests por Modulo

---

### 4.1 StarRating.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/StarRating.test.tsx`
**Componente:** `StarRating.tsx`
**Tipo predominante:** Integration (interacciones de usuario)
**Prioridad:** ALTA - componente critico con logica compleja de hover y accesibilidad

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `StarRating` / `renders 5 star buttons` | Unit | AC-CS06-3 |
| 2 | `StarRating` / `renders all stars as empty when value is 0` | Unit | AC-CS06-3 |
| 3 | `StarRating` / `renders N filled stars when value is N` | Unit | AC-CS06-3 |
| 4 | `StarRating` / `calls onChange with correct value when star is clicked` | Integration | AC-CS06-3 |
| 5 | `StarRating` / `calls onChange with 1 when first star is clicked` | Integration | AC-CS06-3 |
| 6 | `StarRating` / `calls onChange with 5 when last star is clicked` | Integration | AC-CS06-3 |
| 7 | `StarRating` / `shows hover state on mouseenter` | Integration | AC-CS06-3 |
| 8 | `StarRating` / `clears hover state on mouseleave returning to selected value` | Integration | AC-CS06-3 |
| 9 | `StarRating` / `does not call onChange when disabled` | Integration | AC-CS06-3 |
| 10 | `StarRating` / `has role radiogroup with aria-label` | Unit | Accesibilidad |
| 11 | `StarRating` / `each star button has descriptive aria-label` | Unit | Accesibilidad |
| 12 | `StarRating` / `selected star has aria-pressed true` | Unit | Accesibilidad |
| 13 | `StarRating` / `unselected stars have aria-pressed false` | Unit | Accesibilidad |
| 14 | `StarRating` / `star buttons are focusable (not disabled)` | Unit | Accesibilidad |

**Detalles de casos criticos:**

**Caso 4 - calls onChange with correct value when star is clicked:**
- Arrange: Render `<StarRating value={0} onChange={mockOnChange} />`, `mockOnChange = vi.fn()`
- Act: `fireEvent.click(screen.getByRole('button', { name: /3 estrellas/i }))`
- Assert: `expect(mockOnChange).toHaveBeenCalledWith(3)`

**Caso 7 - shows hover state on mouseenter:**
- Arrange: Render con `value={0}`, obtener estrella 3
- Act: `fireEvent.mouseEnter(starButton3)`
- Assert: Verificar que las estrellas 1, 2 y 3 tienen la clase de hover activo (ambar)

**Caso 10 - has role radiogroup with aria-label:**
- Arrange: Render componente
- Assert: `expect(screen.getByRole('radiogroup', { name: /puntuacion de 1 a 5 estrellas/i })).toBeInTheDocument()`

**Caso 12 - selected star has aria-pressed true:**
- Arrange: Render con `value={3}`
- Assert: `expect(screen.getByRole('button', { name: /3 estrellas/i })).toHaveAttribute('aria-pressed', 'true')`
- Assert: `expect(screen.getByRole('button', { name: /4 estrellas/i })).toHaveAttribute('aria-pressed', 'false')`

---

### 4.2 StarDisplay.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/StarDisplay.test.tsx`
**Componente:** `StarDisplay.tsx`
**Tipo predominante:** Unit
**Prioridad:** ALTA - reutilizado en multiples componentes

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `StarDisplay` / `renders 5 star icons` | Unit | AC-CS06-3 |
| 2 | `StarDisplay` / `renders filled stars according to value` | Unit | AC-CS06-3 |
| 3 | `StarDisplay` / `renders fractional star for decimal value (4.5)` | Unit | AC-CS06-7 |
| 4 | `StarDisplay` / `renders numeric value when showNumeric is true` | Unit | - |
| 5 | `StarDisplay` / `does not render numeric value when showNumeric is false` | Unit | - |
| 6 | `StarDisplay` / `applies sm size classes` | Unit | - |
| 7 | `StarDisplay` / `applies md size classes` | Unit | - |
| 8 | `StarDisplay` / `applies lg size classes` | Unit | - |
| 9 | `StarDisplay` / `has role img with descriptive aria-label` | Unit | Accesibilidad |
| 10 | `StarDisplay` / `star icons are aria-hidden` | Unit | Accesibilidad |
| 11 | `StarDisplay` / `aria-label includes the numeric value` | Unit | Accesibilidad |

**Detalles de casos criticos:**

**Caso 9 - has role img with descriptive aria-label:**
- Arrange: Render `<StarDisplay value={4.5} />`
- Assert: `expect(screen.getByRole('img')).toHaveAttribute('aria-label', 'Puntuacion: 4.5 de 5 estrellas')`

**Caso 10 - star icons are aria-hidden:**
- Arrange: Render `<StarDisplay value={3} />`
- Assert: Todos los iconos SVG individuales tienen `aria-hidden="true"`

---

### 4.3 RatingHistogram.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/RatingHistogram.test.tsx`
**Componente:** `RatingHistogram.tsx`
**Tipo predominante:** Unit
**Prioridad:** MEDIA

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `RatingHistogram` / `renders 5 rows (one per star level)` | Unit | AC-CS06-7 |
| 2 | `RatingHistogram` / `renders star level labels 5 to 1` | Unit | AC-CS06-7 |
| 3 | `RatingHistogram` / `renders count for each level` | Unit | AC-CS06-7 |
| 4 | `RatingHistogram` / `renders 0 for star level with no ratings` | Unit | AC-CS06-7 |
| 5 | `RatingHistogram` / `bar width is proportional to count relative to total` | Unit | AC-CS06-7 |
| 6 | `RatingHistogram` / `all bars have 0% width when total is 0` | Unit | AC-CS06-9 |
| 7 | `RatingHistogram` / `renders correctly with mockResumenConValoraciones` | Unit | AC-CS06-7 |
| 8 | `RatingHistogram` / `renders correctly with mockResumenSinValoraciones` | Unit | AC-CS06-9 |

**Detalles de casos criticos:**

**Caso 5 - bar width is proportional:**
- Arrange: Render con `distribucion={{ 5: 7, 4: 3, 3: 1, 2: 1, 1: 0 }}`, `total={12}`
- Assert: La barra del nivel 5 tiene `width: ~58%` (7/12 * 100)
- Assert: La barra del nivel 1 tiene `width: 0%`

**Caso 6 - all bars have 0% width when total is 0:**
- Arrange: Render con `distribucion={{ 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 }}`, `total={0}`
- Assert: Todas las barras de relleno tienen `style.width === '0%'`

---

### 4.4 ValoracionForm.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/ValoracionForm.test.tsx`
**Componente:** `ValoracionForm.tsx`
**Tipo predominante:** Integration
**Prioridad:** MAXIMA - flujo critico del formulario con mutation

**Nota de setup:** Requiere `createWrapper()` con QueryClientProvider porque internamente usa `useCreateValoracion`.

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `ValoracionForm` / `renders the form card with title "Deja tu valoracion"` | Unit | - |
| 2 | `ValoracionForm` / `renders incentive message text` | Unit | AC-CS06-4 |
| 3 | `ValoracionForm` / `renders StarRating component` | Unit | AC-CS06-3 |
| 4 | `ValoracionForm` / `renders optional comment textarea` | Unit | AC-CS06-4 |
| 5 | `ValoracionForm` / `renders character counter starting at 0 / 1000` | Unit | FA-06 |
| 6 | `ValoracionForm` / `submit button is disabled when no star is selected` | Unit | AC-CS06-3 |
| 7 | `ValoracionForm` / `submit button is enabled after selecting a star` | Integration | AC-CS06-3 |
| 8 | `ValoracionForm` / `shows validation error when submitting without rating` | Integration | AC-CS06-3 |
| 9 | `ValoracionForm` / `updates character counter as user types in textarea` | Integration | FA-06 |
| 10 | `ValoracionForm` / `counter turns amber color when comment exceeds 900 chars` | Integration | FA-06 |
| 11 | `ValoracionForm` / `counter turns red when comment exceeds 1000 chars` | Integration | FA-06 |
| 12 | `ValoracionForm` / `submit button is disabled when comment exceeds 1000 chars` | Integration | FA-06 |
| 13 | `ValoracionForm` / `calls onSuccess with result when mutation succeeds` | Integration | AC-CS06-5 |
| 14 | `ValoracionForm` / `shows success toast with exact text on submission` | Integration | AC-CS06-5 |
| 15 | `ValoracionForm` / `submits form without comment (comentario is optional)` | Integration | AC-CS06-4 |
| 16 | `ValoracionForm` / `submit button shows spinner text "Enviando..." during submission` | Integration | UX |
| 17 | `ValoracionForm` / `all inputs are disabled during submission` | Integration | UX |
| 18 | `ValoracionForm` / `shows error toast when mutation fails` | Integration | UX |
| 19 | `ValoracionForm` / `form remains editable after API error` | Integration | UX |

**Detalles de casos criticos:**

**Setup comun para tests de ValoracionForm:**
```typescript
vi.mock("../../infrastructure", () => ({
    valoracionApi: { create: vi.fn() },
}))
vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))
// Render con wrapper (QueryClientProvider) o usar renderWithProviders
```

**Caso 2 - renders incentive message text:**
- Arrange: Render `<ValoracionForm acuerdoId={MOCK_ACUERDO_ID} onSuccess={vi.fn()} />`
- Assert: `expect(screen.getByText("Tu comentario ayuda a otros artistas/profesionales")).toBeInTheDocument()`

**Caso 14 - shows success toast with exact text on submission:**
- Arrange: Mock `valoracionApi.create` resuelve con `mockValoracionCreada` (donde el mensaje de la API es `"Valoracion enviada. Gracias por tu feedback."`)
- Act: Seleccionar estrella 5, click en "Enviar valoracion"
- Assert: `expect(toast.success).toHaveBeenCalledWith("Valoracion enviada. Gracias por tu feedback.")`
- Nota: El toast usa `messages[0].message` de la respuesta del backend, NO un texto hardcodeado

**Caso 15 - submits form without comment:**
- Arrange: Mock `valoracionApi.create` resuelve con `mockValoracionCreadaSinComentario`
- Act: Seleccionar estrella 4, NO escribir comentario, click en "Enviar valoracion"
- Assert: mutation llamada con `{ puntuacion: 4 }` (sin campo comentario)
- Assert: `expect(result.current.isSuccess).toBe(true)`

**Caso 13 - calls onSuccess with result when mutation succeeds:**
- Arrange: Mock API exitoso, `onSuccess = vi.fn()`
- Act: Submit formulario valido
- Assert: `expect(onSuccess).toHaveBeenCalledWith(mockValoracionCreada)`

---

### 4.5 ValoracionReadOnly.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/ValoracionReadOnly.test.tsx`
**Componente:** `ValoracionReadOnly.tsx`
**Tipo predominante:** Unit
**Prioridad:** ALTA - estado post-envio (AC-CS06-5)

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `ValoracionReadOnly` / `renders card with title "Tu valoracion"` | Unit | AC-CS06-5 |
| 2 | `ValoracionReadOnly` / `renders StarDisplay with correct puntuacion` | Unit | AC-CS06-5 |
| 3 | `ValoracionReadOnly` / `renders comment text when comentario exists` | Unit | AC-CS06-5 |
| 4 | `ValoracionReadOnly` / `does not render comment section when comentario is null` | Unit | AC-CS06-5 |
| 5 | `ValoracionReadOnly` / `renders formatted creation date` | Unit | AC-CS06-5 |
| 6 | `ValoracionReadOnly` / `does not render any input or button (read-only)` | Unit | AC-CS06-5 |
| 7 | `ValoracionReadOnly` / `renders check icon indicating completion` | Unit | UX |
| 8 | `ValoracionReadOnly` / `has green border styling indicating completed state` | Unit | UX |

**Detalles de casos criticos:**

**Caso 4 - does not render comment section when comentario is null:**
- Arrange: Render `<ValoracionReadOnly valoracion={{ ...mockValoracionCreada, comentario: undefined }} />`
- Assert: `expect(screen.queryByRole('blockquote')).not.toBeInTheDocument()` (o el elemento que contenga el comentario)

**Caso 6 - does not render any input or button (read-only):**
- Arrange: Render con `mockValoracionCreada`
- Assert: `expect(screen.queryByRole('button')).not.toBeInTheDocument()`
- Assert: `expect(screen.queryByRole('textbox')).not.toBeInTheDocument()`

---

### 4.6 ValoracionesSection.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/ValoracionesSection.test.tsx`
**Componente:** `ValoracionesSection.tsx`
**Tipo predominante:** Integration
**Prioridad:** MAXIMA - seccion completa con query paginada

**Nota de setup:** Requiere `createWrapper()` con QueryClientProvider. Se mockea `valoracionApi`.

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `ValoracionesSection` / `renders loading skeletons while fetching` | Integration | UX |
| 2 | `ValoracionesSection` / `renders section title "Valoraciones"` | Integration | - |
| 3 | `ValoracionesSection` / `renders resumen with puntuacionMedia when data loads` | Integration | AC-CS06-7 |
| 4 | `ValoracionesSection` / `renders totalValoraciones count` | Integration | AC-CS06-7 |
| 5 | `ValoracionesSection` / `renders RatingHistogram component` | Integration | AC-CS06-7 |
| 6 | `ValoracionesSection` / `renders list of ValoracionListItem` | Integration | AC-CS06-8 |
| 7 | `ValoracionesSection` / `renders EmptyValoraciones when no ratings exist` | Integration | AC-CS06-9 |
| 8 | `ValoracionesSection` / `renders empty state message "Este usuario aun no tiene valoraciones"` | Integration | AC-CS06-9 |
| 9 | `ValoracionesSection` / `renders error alert when API fails` | Integration | UX |
| 10 | `ValoracionesSection` / `renders pagination controls when totalCount > pageSize` | Integration | AC-CS06-8 |
| 11 | `ValoracionesSection` / `previous button is disabled on first page` | Integration | AC-CS06-8 |
| 12 | `ValoracionesSection` / `next button is disabled on last page` | Integration | AC-CS06-8 |
| 13 | `ValoracionesSection` / `clicking next page button fetches page 2` | Integration | AC-CS06-8 |
| 14 | `ValoracionesSection` / `pagination controls are not rendered when totalCount <= pageSize` | Integration | AC-CS06-8 |

**Detalles de casos criticos:**

**Setup comun:**
```typescript
vi.mock("../../infrastructure", () => ({
    valoracionApi: { getByUserId: vi.fn() },
}))
// Render: <ValoracionesSection userId={MOCK_USER_ID} /> con wrapper QueryClientProvider
```

**Caso 7 & 8 - EmptyValoraciones cuando no hay datos:**
- Arrange: Mock `valoracionApi.getByUserId` resuelve con `mockValoracionesUsuarioVacio`
- Act: Render `<ValoracionesSection userId={MOCK_USER_ID} />`
- Assert: `expect(screen.getByText("Este usuario aun no tiene valoraciones")).toBeInTheDocument()`
- Assert: El resumen y el histograma no se renderizan

**Caso 13 - clicking next page button fetches page 2:**
- Arrange: Mock resuelve con `mockValoracionesUsuarioConDatos` (12 items, pagina 1)
- Act: Click en boton "Pagina siguiente" o boton "2"
- Assert: `valoracionApi.getByUserId` llamado con `{ userId: MOCK_USER_ID, page: 2, pageSize: 10 }`

---

### 4.7 ValoracionListItem.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/ValoracionListItem.test.tsx`
**Componente:** `ValoracionListItem.tsx`
**Tipo predominante:** Unit
**Prioridad:** MEDIA

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `ValoracionListItem` / `renders autor nombre` | Unit | AC-CS06-8 |
| 2 | `ValoracionListItem` / `renders acuerdo titulo interno` | Unit | AC-CS06-8 |
| 3 | `ValoracionListItem` / `renders StarDisplay with correct puntuacion` | Unit | AC-CS06-8 |
| 4 | `ValoracionListItem` / `renders formatted date` | Unit | AC-CS06-8 |
| 5 | `ValoracionListItem` / `renders comment when comentario exists` | Unit | AC-CS06-8 |
| 6 | `ValoracionListItem` / `does not render comment section when comentario is null` | Unit | AC-CS06-8 |
| 7 | `ValoracionListItem` / `renders avatar with autor image when autorImagenUrl exists` | Unit | Accesibilidad |
| 8 | `ValoracionListItem` / `renders avatar fallback initials when autorImagenUrl is null` | Unit | Accesibilidad |
| 9 | `ValoracionListItem` / `avatar image has correct alt text` | Unit | Accesibilidad |

**Detalles de casos criticos:**

**Caso 8 - renders avatar fallback initials when autorImagenUrl is null:**
- Arrange: Render `<ValoracionListItem valoracion={mockValoracionItemSinImagen} />`
- Assert: Que las iniciales del autor (`"SM"` para `"Studio Mix"`) esten presentes como fallback

**Caso 9 - avatar image has correct alt text:**
- Arrange: Render `<ValoracionListItem valoracion={mockValoracionItem} />`
- Assert: `expect(screen.getByAltText("Los Rockeros")).toBeInTheDocument()`

---

### 4.8 RatingBadge.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/RatingBadge.test.tsx`
**Componente:** `RatingBadge.tsx`
**Tipo predominante:** Unit
**Prioridad:** MEDIA

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `RatingBadge` / `renders puntuacionMedia value` | Unit | AC-CS06-7 |
| 2 | `RatingBadge` / `renders totalValoraciones in parentheses` | Unit | AC-CS06-7 |
| 3 | `RatingBadge` / `renders puntuacion with one decimal (4.0 not 4)` | Unit | UX |
| 4 | `RatingBadge` / `does not render when totalValoraciones is 0` | Unit | UX |
| 5 | `RatingBadge` / `renders compact variant for cards` | Unit | UX |
| 6 | `RatingBadge` / `renders medium variant for profile header` | Unit | UX |
| 7 | `RatingBadge` / `compact variant renders single star icon` | Unit | UX |
| 8 | `RatingBadge` / `medium variant renders StarDisplay` | Unit | UX |

**Detalles de casos criticos:**

**Caso 3 - renders puntuacion with one decimal:**
- Arrange: Render `<RatingBadge puntuacionMedia={4.0} totalValoraciones={5} />`
- Assert: `expect(screen.getByText("4.0")).toBeInTheDocument()` (no "4")

**Caso 4 - does not render when totalValoraciones is 0:**
- Arrange: Render `<RatingBadge puntuacionMedia={0} totalValoraciones={0} />`
- Assert: `expect(container.firstChild).toBeNull()` (o que el componente retorne null)

---

### 4.9 EmptyValoraciones.test.tsx

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/components/EmptyValoraciones.test.tsx`
**Componente:** `EmptyValoraciones.tsx`
**Tipo predominante:** Unit
**Prioridad:** MEDIA

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `EmptyValoraciones` / `renders empty state message` | Unit | AC-CS06-9 |
| 2 | `EmptyValoraciones` / `renders exact text "Este usuario aun no tiene valoraciones"` | Unit | AC-CS06-9 |
| 3 | `EmptyValoraciones` / `renders secondary helper text` | Unit | AC-CS06-9 |
| 4 | `EmptyValoraciones` / `renders star icon as visual indicator` | Unit | UX |
| 5 | `EmptyValoraciones` / `has accessible role status` | Unit | Accesibilidad |

**Detalles de casos criticos:**

**Caso 2 - renders exact text:**
- Assert: `expect(screen.getByText("Este usuario aun no tiene valoraciones")).toBeInTheDocument()`

**Caso 5 - has accessible role status:**
- Assert: `expect(screen.getByRole("status")).toBeInTheDocument()`
- Nota: Patron establecido en `EmptyStateNecesidades.test.tsx` del mismo proyecto

---

### 4.10 useCreateValoracion.test.ts

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/hooks/useCreateValoracion.test.ts`
**Hook:** `useCreateValoracion.ts`
**Tipo predominante:** Integration
**Prioridad:** MAXIMA - mutacion critica con toast y efecto secundario

**Setup:**
```typescript
vi.mock("../../infrastructure", () => ({
    valoracionApi: { create: vi.fn() },
}))
vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))
// Usar createWrapper() y createTestQueryClient() segun patron del proyecto
```

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `useCreateValoracion` / `calls valoracionApi.create with acuerdoId and request data` | Integration | AC-CS06-5 |
| 2 | `useCreateValoracion` / `isSuccess is true after successful mutation` | Integration | AC-CS06-5 |
| 3 | `useCreateValoracion` / `returns created valoracion data on success` | Integration | AC-CS06-5 |
| 4 | `useCreateValoracion` / `shows success toast with message from API response` | Integration | AC-CS06-5 |
| 5 | `useCreateValoracion` / `invalidates acuerdo query on success` | Integration | AC-CS06-5 |
| 6 | `useCreateValoracion` / `isError is true when API fails` | Integration | FA-04 |
| 7 | `useCreateValoracion` / `shows error toast when API fails` | Integration | UX |
| 8 | `useCreateValoracion` / `accepts request without comentario` | Integration | AC-CS06-4 |
| 9 | `useCreateValoracion` / `isPending is true during mutation execution` | Unit | UX |
| 10 | `useCreateValoracion` / `calls onSuccess callback when provided` | Integration | UX |

**Detalles de casos criticos:**

**Caso 1 - calls valoracionApi.create with acuerdoId and request data:**
- Arrange: Mock `valoracionApi.create` resuelve con response que incluye `data: mockValoracionCreada, messages: [{ message: "Valoracion enviada. Gracias por tu feedback.", errorCode: "0001" }]`
- Act: `result.current.mutate({ puntuacion: 5, comentario: "Excelente" })`
- Assert: `expect(valoracionApi.create).toHaveBeenCalledWith(MOCK_ACUERDO_ID, { puntuacion: 5, comentario: "Excelente" })`

**Caso 4 - shows success toast with message from API response:**
- Arrange: API resuelve con `messages[0].message = "Valoracion enviada. Gracias por tu feedback."`
- Assert: `expect(toast.success).toHaveBeenCalledWith("Valoracion enviada. Gracias por tu feedback.")`

**Caso 5 - invalidates acuerdo query on success:**
- Arrange: Crear `queryClient` con `invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")`
- Act: Mutation exitosa
- Assert: `invalidateSpy` llamado con queryKey que incluye el `acuerdoId`
- Nota: Patron de verificacion identico a `useEnviarMensaje.test.ts` lineas 66-83

---

### 4.11 useValoracionesUsuario.test.ts

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/hooks/useValoracionesUsuario.test.ts`
**Hook:** `useValoracionesUsuario.ts`
**Tipo predominante:** Integration
**Prioridad:** ALTA - query paginada con parametros

**Setup:**
```typescript
vi.mock("../../infrastructure", () => ({
    valoracionApi: { getByUserId: vi.fn() },
}))
```

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `useValoracionesUsuario` / `fetches valoraciones data successfully` | Integration | AC-CS06-6 |
| 2 | `useValoracionesUsuario` / `isLoading is true initially` | Unit | UX |
| 3 | `useValoracionesUsuario` / `isSuccess is true after data loads` | Integration | AC-CS06-6 |
| 4 | `useValoracionesUsuario` / `data contains resumen and valoraciones list` | Integration | AC-CS06-7 |
| 5 | `useValoracionesUsuario` / `calls API with correct userId` | Integration | AC-CS06-6 |
| 6 | `useValoracionesUsuario` / `calls API with page and pageSize params` | Integration | AC-CS06-8 |
| 7 | `useValoracionesUsuario` / `defaults to page 1 and pageSize 10` | Integration | AC-CS06-8 |
| 8 | `useValoracionesUsuario` / `isError is true when API fails` | Integration | UX |
| 9 | `useValoracionesUsuario` / `query is disabled when userId is undefined` | Unit | UX |
| 10 | `useValoracionesUsuario` / `re-fetches when page param changes` | Integration | AC-CS06-8 |

**Detalles de casos criticos:**

**Caso 6 - calls API with page and pageSize params:**
- Arrange: Render hook con `useValoracionesUsuario(MOCK_USER_ID, { page: 2, pageSize: 5 })`
- Assert: `expect(valoracionApi.getByUserId).toHaveBeenCalledWith(MOCK_USER_ID, { page: 2, pageSize: 5 })`

**Caso 9 - query is disabled when userId is undefined:**
- Arrange: `renderHook(() => useValoracionesUsuario(undefined))`
- Assert: `valoracionApi.getByUserId` NO llamado
- Assert: `result.current.isLoading === false`

**Caso 10 - re-fetches when page param changes:**
- Arrange: Render con `page: 1`, esperar success
- Act: Rerender con `page: 2`
- Assert: `valoracionApi.getByUserId` llamado dos veces, segunda vez con `page: 2`

---

### 4.12 valoracion.api.test.ts

**Archivo de test:** `src/web/src/features/crowdsourcing/__tests__/services/valoracion.api.test.ts`
**Servicio:** `valoracion.api.ts`
**Tipo predominante:** Unit
**Prioridad:** ALTA - contrato con la API

**Setup:**
```typescript
vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))
// Patron identico a crowdsourcing.api.test.ts
```

| # | describe / it | Tipo | Criterio AC |
|---|--------------|------|-------------|
| 1 | `valoracionApi.create` / `calls correct endpoint with POST method` | Unit | AC-CS06-5 |
| 2 | `valoracionApi.create` / `sends puntuacion and comentario in request body` | Unit | AC-CS06-5 |
| 3 | `valoracionApi.create` / `returns created valoracion data on success` | Unit | AC-CS06-5 |
| 4 | `valoracionApi.create` / `throws when API returns error` | Unit | FA-04 |
| 5 | `valoracionApi.getByUserId` / `calls correct endpoint with userId in path` | Unit | AC-CS06-6 |
| 6 | `valoracionApi.getByUserId` / `sends page and pageSize as query params` | Unit | AC-CS06-8 |
| 7 | `valoracionApi.getByUserId` / `returns resumen and paginated valoraciones on success` | Unit | AC-CS06-7 |
| 8 | `valoracionApi.getByUserId` / `throws when API returns 404 (user not found)` | Unit | UX |
| 9 | `valoracionApi.getByUserId` / `uses default page 1 and pageSize 10 when params not provided` | Unit | AC-CS06-8 |
| 10 | `valoracionApi.getByUserId` / `returns empty valoraciones list when user has no ratings` | Unit | AC-CS06-9 |

**Detalles de casos criticos:**

**Caso 1 - calls correct endpoint with POST method:**
- Assert: `expect(apiFetch).toHaveBeenCalledWith("/crowdsourcing/acuerdos/e1f2a3b4.../valoraciones", { method: "POST", data: {...} })`

**Caso 5 - calls correct endpoint with userId in path:**
- Assert: `expect(apiFetch).toHaveBeenCalledWith("/crowdsourcing/usuarios/profesional-user-001/valoraciones?page=1&pageSize=10")`
- Nota: Verificar la forma exacta de construccion de la URL segun la implementacion de `apiFetch`

---

## 5. Tests de Accesibilidad Consolidados

Los siguientes tests de accesibilidad son los mas criticos y deben existir obligatoriamente:

| Componente | Test | Requisito |
|-----------|------|-----------|
| `StarRating` | `has role radiogroup with aria-label` | WCAG 1.3.1 |
| `StarRating` | `each star button has descriptive aria-label` | WCAG 2.4.6 |
| `StarRating` | `selected star has aria-pressed true` | WCAG 4.1.2 |
| `StarRating` | `star buttons are focusable` | WCAG 2.1.1 |
| `StarDisplay` | `has role img with descriptive aria-label` | WCAG 1.1.1 |
| `StarDisplay` | `star icons are aria-hidden` | WCAG 1.1.1 |
| `ValoracionForm` | `shows validation error with role alert` | WCAG 4.1.3 |
| `ValoracionListItem` | `avatar image has correct alt text` | WCAG 1.1.1 |
| `EmptyValoraciones` | `has accessible role status` | WCAG 4.1.3 |

---

## 6. Cobertura Objetivo por Archivo

| Archivo | Lineas | Funciones | Branches | Justificacion |
|---------|--------|-----------|----------|---------------|
| `StarRating.tsx` | 90% | 100% | 85% | Logica critica de hover/click/accesibilidad |
| `StarDisplay.tsx` | 90% | 100% | 85% | Logica de fraccion de estrella |
| `RatingHistogram.tsx` | 85% | 100% | 80% | Calculo de porcentajes de barras |
| `ValoracionForm.tsx` | 85% | 90% | 80% | Formulario con multiples estados |
| `ValoracionReadOnly.tsx` | 90% | 100% | 85% | Casos con/sin comentario |
| `ValoracionesSection.tsx` | 80% | 90% | 80% | Paginacion, loading, error, empty |
| `ValoracionListItem.tsx` | 90% | 100% | 85% | Casos con/sin imagen, con/sin comentario |
| `RatingBadge.tsx` | 90% | 100% | 90% | Condicion de no render con 0 valoraciones |
| `EmptyValoraciones.tsx` | 100% | 100% | 100% | Componente simple sin branches |
| `useCreateValoracion.ts` | 90% | 100% | 85% | Toast, invalidacion, error handling |
| `useValoracionesUsuario.ts` | 90% | 100% | 85% | Paginacion, enabled/disabled |
| `valoracion.api.ts` | 95% | 100% | 90% | Endpoints, errores |

**Meta Global:** 80%+ en todas las metricas para el conjunto de la feature.

---

## 7. Casos Edge y Boundary

Los siguientes casos edge deben cubrirse para garantizar robustez:

| Escenario | Componente/Hook | Test |
|-----------|-----------------|------|
| Comentario de exactamente 1000 caracteres (limite) | `ValoracionForm` | Submit permitido, contador en rojo |
| Comentario de 1001 caracteres (supera limite) | `ValoracionForm` | Submit bloqueado |
| Comentario de 901 caracteres (zona ambar) | `ValoracionForm` | Contador ambar, submit permitido |
| `puntuacionMedia` es entero exacto (ej: 4.0) | `RatingBadge`, `StarDisplay` | Se muestra "4.0" con decimal |
| `autorImagenUrl` es null en item | `ValoracionListItem` | Avatar fallback con iniciales |
| `distribucion` con todos 0 | `RatingHistogram` | Todas las barras al 0% |
| `totalValoraciones` es 0 | `RatingBadge` | Componente no se renderiza |
| API retorna 400 con errorCode 4014 (ya valorado) | `useCreateValoracion` | Error toast contextual |
| userId es undefined en hook | `useValoracionesUsuario` | Query no ejecutada |
| Primera pagina (prev button disabled) | `ValoracionesSection` | Boton deshabilitado |
| Ultima pagina (next button disabled) | `ValoracionesSection` | Boton deshabilitado |

---

## 8. Comandos de Ejecucion

```bash
# Desde la raiz del proyecto src/web
cd src/web

# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar solo los tests de la feature valoraciones
npm run test -- --reporter=verbose src/features/crowdsourcing/__tests__

# Ejecutar un archivo especifico
npm run test -- src/features/crowdsourcing/__tests__/components/StarRating.test.tsx

# Watch mode para desarrollo
npm run test -- --watch

# Filtrar por nombre de test
npm run test -- --reporter=verbose -t "ValoracionForm"
```

---

## 9. Decisiones de Diseno del Plan

### 9.1 Patron de Mocking Establecido

El proyecto NO usa MSW. El patron es `vi.mock` sobre el modulo de infraestructura directamente:
```typescript
vi.mock("../../infrastructure", () => ({ valoracionApi: { create: vi.fn() } }))
```
O sobre `@/lib/api-client` para tests del servicio.

### 9.2 Wrapper de QueryClient

Se usa `createElement` en lugar de JSX en el wrapper, identico a `useAcuerdo.test.ts` y `useEnviarMensaje.test.ts`:
```typescript
return function Wrapper({ children }: { children: React.ReactNode }) {
    return createElement(QueryClientProvider, { client }, children)
}
```

### 9.3 Toast de Exito con Texto Exacto

El toast de exito de `useCreateValoracion` debe usar el `messages[0].message` de la respuesta del backend, NO un string hardcodeado en el frontend. Esto garantiza que si el backend cambia el mensaje, el test lo detecta. El texto esperado es exactamente: `"Valoracion enviada. Gracias por tu feedback."`

### 9.4 Separacion entre ValoracionForm e Integration

`ValoracionForm` se testea como componente de integracion porque internamente instancia `useCreateValoracion`. Los tests mockean `valoracionApi` y verifican el comportamiento visible (toast, llamada al callback `onSuccess`, estado del boton). No se testea directamente el hook dentro del componente.

### 9.5 Tests de Hover en StarRating

Los tests de hover usan `fireEvent.mouseEnter` y `fireEvent.mouseLeave`. Si el hover se implementa con CSS puro (`:hover`) sin estado React, los tests de hover de clase visual pueden ser limitados en jsdom. En ese caso se prioriza verificar el comportamiento de estado con eventos de React.

---

## 10. Checklist Pre-Implementacion

- [ ] Archivo `__mocks__/valoracion.mock.ts` creado con todos los fixtures
- [ ] Mock de `valoracionApi` preparado con los dos metodos (`create`, `getByUserId`)
- [ ] Mock de `sonner` toast configurado
- [ ] `createWrapper()` y `createTestQueryClient()` disponibles (pueden copiarse del patron existente)
- [ ] Tests de `StarRating` incluyen accesibilidad (radiogroup, aria-label, aria-pressed)
- [ ] Tests de `ValoracionForm` verifican toast con texto exacto del backend
- [ ] Tests de `useCreateValoracion` verifican invalidacion de query con spy
- [ ] Tests de `ValoracionesSection` cubren estado empty con mensaje exacto
- [ ] Tests de `RatingBadge` cubren el caso de no render con 0 valoraciones
- [ ] Todos los tests de componentes verifican el caso sin comentario (null/undefined)
- [ ] Cobertura 80%+ en cada archivo verificada con `npm run test -- --coverage`
- [ ] Tests pasan sin errores en modo CI (sin modo watch)
