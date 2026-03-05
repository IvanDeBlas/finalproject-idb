# Estrategia de Testing: Crear Campania (Admin Dashboard)

**Fecha:** 2026-02-12
**Feature:** crear-campania
**Target:** src/admin (Next.js 14)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen Ejecutivo

Esta estrategia de testing cubre la feature "Crear Campania" del dashboard de artistas, implementando tests unitarios y de integracion para garantizar la calidad del wizard multi-step, validaciones con Zod, y flujo completo de creacion/edicion/publicacion de campanias.

| Tipo | Cantidad | Prioridad | Cobertura Esperada |
|------|----------|-----------|-------------------|
| Unit Tests | 28 | Alta | 85% |
| Integration Tests | 12 | Alta | 80% |
| **Total** | **40** | - | **82%** |

**Tiempo estimado de ejecucion:** < 30 segundos

---

## 2. Stack de Testing

| Herramienta | Version | Uso |
|-------------|---------|-----|
| Vitest | ^2.0.0 | Test runner (debe instalarse) |
| @testing-library/react | ^16.0.0 | Testing components |
| @testing-library/user-event | ^14.5.0 | User interactions |
| @testing-library/react-hooks | ^8.0.0 | Testing hooks |
| MSW (Mock Service Worker) | ^2.0.0 | API mocking |
| @tanstack/react-query | ^5.56.0 | Query testing utilities (ya instalado) |
| vitest-mock-extended | ^2.0.0 | Mock helpers |

**Dependencias a instalar:**
```bash
npm install -D vitest @vitest/ui @testing-library/react @testing-library/user-event @testing-library/jest-dom msw vitest-mock-extended happy-dom
```

---

## 3. Estructura de Tests

```
src/admin/src/
├── __tests__/
│   ├── components/
│   │   ├── wizard/
│   │   │   ├── CampaniaWizard.test.tsx
│   │   │   ├── WizardStepper.test.tsx
│   │   │   ├── BasicInfoStep.test.tsx
│   │   │   ├── FundingStep.test.tsx
│   │   │   ├── DurationStep.test.tsx
│   │   │   ├── MediaStep.test.tsx
│   │   │   └── ReviewStep.test.tsx
│   │   ├── campanias/
│   │   │   ├── CampaniaListCard.test.tsx
│   │   │   ├── DraftBanner.test.tsx
│   │   │   └── PublishConfirmModal.test.tsx
│   │   └── shared/
│   │       └── StatusBadge.test.tsx
│   ├── hooks/
│   │   ├── useCreateCampania.test.ts
│   │   ├── useUpdateCampania.test.ts
│   │   ├── usePublishCampania.test.ts
│   │   ├── useMisCampanias.test.ts
│   │   └── useWizardNavigation.test.ts
│   ├── services/
│   │   └── campania.service.test.ts
│   ├── integration/
│   │   ├── create-campania-flow.test.tsx
│   │   ├── edit-campania-flow.test.tsx
│   │   └── publish-campania-flow.test.tsx
│   └── utils/
│       └── validation-helpers.test.ts
├── __mocks__/
│   ├── campania.mock.ts
│   ├── handlers.ts
│   └── test-utils.tsx
└── vitest.config.ts
```

---

## 4. Mocks y Fixtures

### 4.1 Mock Data

**Archivo:** `src/admin/src/__mocks__/campania.mock.ts`

```typescript
import { Campania, CampaniaListItem, EstadoCampania, TipoFinanciacion } from '@/shared/types/campania';

export const mockArtista = {
  id: '550e8400-e29b-41d4-a716-446655440000',
  nombreArtistico: 'Test Artist',
  email: 'artist@test.com',
};

export const mockCampaniaBorrador: Campania = {
  id: '123e4567-e89b-12d3-a456-426614174000',
  artistaId: mockArtista.id,
  proyectoArtisticoId: undefined,
  titulo: 'Mi Primer Album',
  subtitulo: 'Rock alternativo con influencias indie',
  descripcionCorta: 'Necesitamos tu apoyo para grabar nuestro primer album de estudio.',
  videoPrincipalUrl: 'https://youtube.com/watch?v=xyz123',
  imagenPrincipalUrl: 'https://example.com/image.jpg',
  monedaId: 1,
  importeObjetivo: 5000.00,
  importeMinimo: 100.00,
  importePledgedActual: 0.00,
  tipoFinanciacionId: TipoFinanciacion.TodoONada,
  estadoCampaniaId: EstadoCampania.Borrador,
  permiteAportacionesAnonimas: false,
  permitePropinas: true,
  porcentajeComisionPlataforma: null,
  fechaInicio: '2026-03-01T00:00:00Z',
  fechaFin: '2026-04-30T23:59:59Z',
  fechaPublicacion: null,
  fechaCierre: null,
  fechaCreacion: '2026-02-12T10:30:00Z',
  fechaActualizacion: null,
};

export const mockCampaniaPublicada: Campania = {
  ...mockCampaniaBorrador,
  id: '789e4567-e89b-12d3-a456-426614174111',
  estadoCampaniaId: EstadoCampania.Publicada,
  fechaPublicacion: '2026-02-12T14:00:00Z',
  importePledgedActual: 1250.50,
};

export const mockCampaniaList: CampaniaListItem[] = [
  {
    id: mockCampaniaBorrador.id,
    artistaId: mockCampaniaBorrador.artistaId,
    titulo: mockCampaniaBorrador.titulo,
    subtitulo: mockCampaniaBorrador.subtitulo,
    descripcionCorta: mockCampaniaBorrador.descripcionCorta,
    imagenPrincipalUrl: mockCampaniaBorrador.imagenPrincipalUrl,
    importeObjetivo: mockCampaniaBorrador.importeObjetivo,
    importePledgedActual: mockCampaniaBorrador.importePledgedActual,
    estadoCampaniaId: mockCampaniaBorrador.estadoCampaniaId,
    fechaInicio: mockCampaniaBorrador.fechaInicio,
    fechaFin: mockCampaniaBorrador.fechaFin,
    fechaCreacion: mockCampaniaBorrador.fechaCreacion,
  },
  {
    id: mockCampaniaPublicada.id,
    artistaId: mockCampaniaPublicada.artistaId,
    titulo: mockCampaniaPublicada.titulo,
    subtitulo: mockCampaniaPublicada.subtitulo,
    descripcionCorta: mockCampaniaPublicada.descripcionCorta,
    imagenPrincipalUrl: mockCampaniaPublicada.imagenPrincipalUrl,
    importeObjetivo: mockCampaniaPublicada.importeObjetivo,
    importePledgedActual: mockCampaniaPublicada.importePledgedActual,
    estadoCampaniaId: mockCampaniaPublicada.estadoCampaniaId,
    fechaInicio: mockCampaniaPublicada.fechaInicio,
    fechaFin: mockCampaniaPublicada.fechaFin,
    fechaCreacion: mockCampaniaPublicada.fechaCreacion,
  },
];

export const mockCreateCampaniaRequest = {
  titulo: mockCampaniaBorrador.titulo,
  subtitulo: mockCampaniaBorrador.subtitulo,
  descripcionCorta: mockCampaniaBorrador.descripcionCorta,
  videoPrincipalUrl: mockCampaniaBorrador.videoPrincipalUrl,
  imagenPrincipalUrl: mockCampaniaBorrador.imagenPrincipalUrl,
  monedaId: mockCampaniaBorrador.monedaId,
  importeObjetivo: mockCampaniaBorrador.importeObjetivo,
  importeMinimo: mockCampaniaBorrador.importeMinimo,
  tipoFinanciacionId: mockCampaniaBorrador.tipoFinanciacionId,
  permiteAportacionesAnonimas: mockCampaniaBorrador.permiteAportacionesAnonimas,
  permitePropinas: mockCampaniaBorrador.permitePropinas,
  fechaInicio: mockCampaniaBorrador.fechaInicio,
  fechaFin: mockCampaniaBorrador.fechaFin,
};

export const mockValidationErrors = {
  tituloVacio: {
    errorCode: '1001',
    message: 'El titulo es obligatorio',
  },
  tituloMuyLargo: {
    errorCode: '1002',
    message: 'El titulo no puede superar los 200 caracteres',
  },
  importeInvalido: {
    errorCode: '1011',
    message: 'El importe objetivo debe ser mayor a 0',
  },
  fechaInvalida: {
    errorCode: '1012',
    message: 'La fecha de fin debe ser posterior a la fecha de inicio',
  },
};
```

---

### 4.2 MSW Handlers

**Archivo:** `src/admin/src/__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import { setupServer } from 'msw/node';
import { mockCampaniaBorrador, mockCampaniaList, mockCampaniaPublicada } from './campania.mock';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export const campaniaHandlers = [
  // GET /api/campanias/mis-campanias
  http.get(`${API_BASE_URL}/api/campanias/mis-campanias`, () => {
    return HttpResponse.json({
      data: mockCampaniaList,
      messages: [{ message: 'Campanias encontradas', errorCode: '0000' }],
    });
  }),

  // GET /api/campanias/:id
  http.get(`${API_BASE_URL}/api/campanias/:id`, ({ params }) => {
    const { id } = params;
    const campania = id === mockCampaniaBorrador.id ? mockCampaniaBorrador : mockCampaniaPublicada;

    return HttpResponse.json({
      data: campania,
      messages: [{ message: 'Campania encontrada', errorCode: '0000' }],
    });
  }),

  // POST /api/campanias
  http.post(`${API_BASE_URL}/api/campanias`, async ({ request }) => {
    const body = await request.json();

    return HttpResponse.json({
      data: { ...mockCampaniaBorrador, ...body, id: '123e4567-e89b-12d3-a456-426614174000' },
      messages: [{ message: 'Campania creada correctamente', errorCode: '0001' }],
    }, { status: 201 });
  }),

  // PUT /api/campanias/:id
  http.put(`${API_BASE_URL}/api/campanias/:id`, async ({ request }) => {
    const body = await request.json();

    return HttpResponse.json({
      data: true,
      messages: [{ message: 'Campania actualizada correctamente', errorCode: '0002' }],
    });
  }),

  // POST /api/campanias/:id/publicar
  http.post(`${API_BASE_URL}/api/campanias/:id/publicar`, ({ params }) => {
    const { id } = params;

    return HttpResponse.json({
      data: {
        id,
        estadoCampaniaId: 2,
        fechaPublicacion: new Date().toISOString(),
        message: 'Campania publicada exitosamente',
      },
      messages: [{ message: 'Campania publicada exitosamente', errorCode: '0000' }],
    });
  }),
];

// Error handlers (para tests de error)
export const campaniaErrorHandlers = [
  // Error 401 - Unauthorized
  http.post(`${API_BASE_URL}/api/campanias`, () => {
    return HttpResponse.json({
      data: null,
      messages: [{ message: 'Token no valido o expirado', errorCode: '3001' }],
    }, { status: 401 });
  }),

  // Error 404 - Not Found
  http.get(`${API_BASE_URL}/api/campanias/:id`, () => {
    return HttpResponse.json({
      data: null,
      messages: [{ message: 'Campania no encontrada', errorCode: '2003' }],
    }, { status: 404 });
  }),

  // Error 409 - Solo borradores editables
  http.put(`${API_BASE_URL}/api/campanias/:id`, () => {
    return HttpResponse.json({
      data: false,
      messages: [{ message: 'Solo se pueden editar campanias en estado borrador', errorCode: '4009' }],
    }, { status: 409 });
  }),

  // Error 400 - Validation error
  http.post(`${API_BASE_URL}/api/campanias`, () => {
    return HttpResponse.json({
      data: null,
      messages: [
        { message: 'El titulo es obligatorio', errorCode: '1001' },
        { message: 'El importe objetivo debe ser mayor a 0', errorCode: '1011' },
      ],
    }, { status: 400 });
  }),
];

export const server = setupServer(...campaniaHandlers);
```

---

### 4.3 Test Utilities

**Archivo:** `src/admin/src/__mocks__/test-utils.tsx`

```typescript
import React, { ReactElement } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from '@/components/ui/sonner';

// Create test QueryClient with custom defaults
export const createTestQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
        gcTime: 0,
      },
      mutations: {
        retry: false,
      },
    },
    logger: {
      log: console.log,
      warn: console.warn,
      error: () => {}, // Silence errors in tests
    },
  });

interface AllProvidersProps {
  children: React.ReactNode;
}

export const AllProviders = ({ children }: AllProvidersProps) => {
  const testQueryClient = createTestQueryClient();

  return (
    <QueryClientProvider client={testQueryClient}>
      {children}
      <Toaster />
    </QueryClientProvider>
  );
};

// Custom render with providers
export const renderWithProviders = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => {
  return render(ui, { wrapper: AllProviders, ...options });
};

// Helper para esperar actualizaciones de React Query
export const waitForQuerySuccess = async (queryClient: QueryClient, queryKey: string[]) => {
  await new Promise((resolve) => {
    const unsubscribe = queryClient.getQueryCache().subscribe((event) => {
      if (event?.type === 'updated' && event.query.queryKey[0] === queryKey[0]) {
        if (event.query.state.status === 'success') {
          unsubscribe();
          resolve(true);
        }
      }
    });
  });
};

// Mock localStorage
export const mockLocalStorage = () => {
  let store: Record<string, string> = {};

  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value.toString();
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
};

// Mock router
export const mockRouter = {
  push: vi.fn(),
  replace: vi.fn(),
  back: vi.fn(),
  pathname: '/dashboard/campanias/nueva',
  query: {},
};

// Re-export everything from testing-library
export * from '@testing-library/react';
```

---

### 4.4 Vitest Configuration

**Archivo:** `src/admin/vitest.config.ts`

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'happy-dom',
    setupFiles: ['./src/__tests__/setup.ts'],
    globals: true,
    css: false,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      include: ['src/**/*.{ts,tsx}'],
      exclude: [
        'src/**/*.test.{ts,tsx}',
        'src/**/__tests__/**',
        'src/**/__mocks__/**',
        'src/types/**',
        'src/**/*.d.ts',
        'node_modules/**',
      ],
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 75,
        statements: 80,
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      '@/shared': path.resolve(__dirname, '../shared'),
    },
  },
});
```

**Archivo:** `src/admin/src/__tests__/setup.ts`

```typescript
import { expect, afterEach, beforeAll, afterAll, vi } from 'vitest';
import { cleanup } from '@testing-library/react';
import '@testing-library/jest-dom';
import { server } from './__mocks__/handlers';

// Setup MSW
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => {
  server.resetHandlers();
  cleanup();
});
afterAll(() => server.close());

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
    back: vi.fn(),
    pathname: '/dashboard/campanias/nueva',
  }),
  usePathname: () => '/dashboard/campanias/nueva',
  useSearchParams: () => new URLSearchParams(),
}));

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// Mock IntersectionObserver
global.IntersectionObserver = class IntersectionObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
  takeRecords() {
    return [];
  }
};
```

---

## 5. Tests por Modulo

### 5.1 Components - Wizard Steps

#### BasicInfoStep.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/BasicInfoStep.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders all form fields | Unit | Renderiza titulo, subtitulo, descripcionCorta | Alta |
| displays validation errors | Unit | Muestra errores de Zod en campos invalidos | Alta |
| titulo field is required | Unit | Error cuando titulo esta vacio | Alta |
| titulo max 200 characters | Unit | Error cuando titulo > 200 chars | Alta |
| subtitulo max 300 characters | Unit | Error cuando subtitulo > 300 chars | Media |
| descripcionCorta max 500 characters | Unit | Error cuando descripcion > 500 chars | Media |
| calls onNext with valid data | Integration | Llama onNext con datos validos | Alta |
| blocks advance with invalid data | Integration | No permite avanzar si validacion falla | Alta |
| loads initial data correctly | Unit | Carga datos previos en edicion | Media |
| character counter updates | Unit | Contador de caracteres se actualiza en tiempo real | Baja |

**Setup basico:**
```typescript
import { renderWithProviders, screen, fireEvent, waitFor } from '@/__mocks__/test-utils';
import { BasicInfoStep } from '@/components/wizard/BasicInfoStep';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { campaniaBasicInfoSchema } from '@/shared/schemas/campania.schema';

describe('BasicInfoStep', () => {
  const mockOnNext = vi.fn();
  const mockOnPrev = vi.fn();

  const renderStep = (initialData = {}) => {
    return renderWithProviders(
      <BasicInfoStep
        data={initialData}
        onNext={mockOnNext}
        onPrev={mockOnPrev}
      />
    );
  };

  it('renders all form fields', () => {
    renderStep();

    expect(screen.getByLabelText(/titulo/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/subtitulo/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/descripcion corta/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /siguiente/i })).toBeInTheDocument();
  });

  it('displays validation errors for empty titulo', async () => {
    renderStep();

    const submitButton = screen.getByRole('button', { name: /siguiente/i });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/el titulo es obligatorio/i)).toBeInTheDocument();
    });
    expect(mockOnNext).not.toHaveBeenCalled();
  });

  it('calls onNext with valid data', async () => {
    renderStep();

    fireEvent.change(screen.getByLabelText(/titulo/i), {
      target: { value: 'Mi Primer Album' },
    });

    fireEvent.change(screen.getByLabelText(/subtitulo/i), {
      target: { value: 'Rock alternativo' },
    });

    fireEvent.click(screen.getByRole('button', { name: /siguiente/i }));

    await waitFor(() => {
      expect(mockOnNext).toHaveBeenCalledWith({
        titulo: 'Mi Primer Album',
        subtitulo: 'Rock alternativo',
        descripcionCorta: '',
      });
    });
  });
});
```

---

#### FundingStep.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/FundingStep.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders funding fields | Unit | Renderiza importeObjetivo, importeMinimo, tipo | Alta |
| importeObjetivo required | Unit | Error si objetivo vacio | Alta |
| importeObjetivo must be positive | Unit | Error si objetivo <= 0 | Alta |
| importeMinimo <= importeObjetivo | Unit | Error si minimo > objetivo | Alta |
| moneda default is EUR | Unit | Moneda por defecto es 1 (EUR) | Media |
| tipo financiacion required | Unit | Error si no selecciona tipo | Alta |
| toggles switches correctly | Unit | Switches de propinas y anonimas funcionan | Baja |
| calls onNext with valid funding data | Integration | Avanza con datos validos | Alta |

---

#### DurationStep.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/DurationStep.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders date pickers | Unit | Renderiza fecha inicio y fecha fin | Alta |
| fechaFin must be after fechaInicio | Unit | Error si fin <= inicio | Alta |
| fechaFin minimum 7 days from now | Unit | Error si fin < hoy + 7 dias | Alta |
| fechaFin maximum 60 days from now | Unit | Validacion opcional max 60 dias | Media |
| fechaInicio is optional | Unit | Permite crear sin fecha inicio | Media |
| date picker opens on click | Unit | DatePicker se abre al hacer click | Baja |
| calls onNext with valid dates | Integration | Avanza con fechas validas | Alta |

---

#### MediaStep.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/MediaStep.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders media URL fields | Unit | Renderiza imagenPrincipalUrl, videoPrincipalUrl | Alta |
| validates image URL format | Unit | Error si URL de imagen invalida | Alta |
| validates video URL format | Unit | Error si URL de video invalida | Alta |
| all media fields are optional | Unit | Permite crear sin media | Media |
| calls onFinish with complete data | Integration | Finaliza wizard con datos completos | Alta |
| shows loading state on submit | Unit | Muestra spinner durante creacion | Media |

---

#### ReviewStep.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/ReviewStep.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| displays all collected data | Unit | Muestra preview de todos los datos | Alta |
| renders imagen principal preview | Unit | Muestra imagen si existe URL | Media |
| renders video embed preview | Unit | Muestra embed de video si existe | Media |
| calculates funding percentage | Unit | Calcula % recaudado correctamente | Baja |
| creates draft button works | Integration | Boton "Guardar borrador" funciona | Alta |
| publish button works | Integration | Boton "Publicar" funciona | Alta |
| shows validation errors before publish | Unit | Valida campos requeridos antes de publicar | Alta |
| can edit from review | Unit | Permite volver a editar steps | Media |

---

### 5.2 Components - Wizard Core

#### CampaniaWizard.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/CampaniaWizard.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders first step on mount | Integration | Muestra BasicInfoStep inicialmente | Alta |
| navigates to next step | Integration | Avanza al siguiente step | Alta |
| navigates to previous step | Integration | Retrocede al step anterior | Alta |
| maintains data between steps | Integration | Persiste datos al cambiar de step | Alta |
| disables next if validation fails | Integration | Deshabilita "Siguiente" si validacion falla | Alta |
| persists data to localStorage | Integration | Guarda progreso en localStorage | Media |
| loads data from localStorage | Integration | Recupera progreso guardado | Media |
| clears localStorage on success | Integration | Limpia localStorage al completar | Media |

---

#### WizardStepper.test.tsx

**Archivo:** `src/admin/src/__tests__/components/wizard/WizardStepper.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders all steps | Unit | Muestra indicadores de 4 steps | Alta |
| highlights current step | Unit | Marca step activo visualmente | Alta |
| marks completed steps | Unit | Marca steps completados con checkmark | Alta |
| prevents jumping to future steps | Unit | No permite saltar a steps no completados | Media |
| allows jumping to previous steps | Unit | Permite volver a steps anteriores | Media |
| shows step labels | Unit | Muestra nombres de steps | Baja |

---

### 5.3 Components - Campanias Management

#### CampaniaListCard.test.tsx

**Archivo:** `src/admin/src/__tests__/components/campanias/CampaniaListCard.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders campania data | Unit | Muestra titulo, imagen, meta, recaudado | Alta |
| displays status badge | Unit | Muestra badge segun estado (Borrador/Publicada) | Alta |
| calculates progress percentage | Unit | Calcula % recaudado vs meta | Media |
| shows draft indicator | Unit | Muestra indicador visual de borrador | Alta |
| shows edit button for draft | Unit | Muestra boton editar solo si borrador | Alta |
| hides edit button for published | Unit | Oculta editar si publicada | Alta |
| calls onEdit when clicked | Unit | Llama callback onEdit con ID | Media |
| formats currency correctly | Unit | Formatea montos con simbolo EUR | Baja |
| shows fallback image | Unit | Muestra placeholder si no hay imagen | Baja |

---

#### DraftBanner.test.tsx

**Archivo:** `src/admin/src/__tests__/components/campanias/DraftBanner.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders banner with message | Unit | Muestra mensaje "Campania en borrador" | Alta |
| shows publish button | Unit | Muestra boton "Publicar Ahora" | Alta |
| shows close button | Unit | Muestra boton cerrar | Media |
| calls onPublish when clicked | Unit | Llama callback onPublish | Alta |
| calls onClose when clicked | Unit | Llama callback onClose | Media |
| hides after close | Unit | Se oculta al hacer click en cerrar | Media |

---

#### PublishConfirmModal.test.tsx

**Archivo:** `src/admin/src/__tests__/components/campanias/PublishConfirmModal.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| renders modal when open | Unit | Muestra modal cuando open=true | Alta |
| displays confirmation message | Unit | Muestra mensaje de confirmacion | Alta |
| shows confirm and cancel buttons | Unit | Muestra botones Publicar y Cancelar | Alta |
| calls onConfirm when confirm clicked | Unit | Llama callback onConfirm | Alta |
| calls onCancel when cancel clicked | Unit | Llama callback onCancel | Alta |
| shows loading state | Unit | Muestra loading al confirmar | Media |
| disables buttons during loading | Unit | Deshabilita botones durante loading | Media |
| closes on backdrop click | Unit | Cierra al hacer click fuera | Baja |

---

### 5.4 Hooks - Mutations y Queries

#### useCreateCampania.test.ts

**Archivo:** `src/admin/src/__tests__/hooks/useCreateCampania.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| returns mutation functions | Unit | Expone mutate, mutateAsync, isLoading | Alta |
| creates campania successfully | Integration | POST correcto retorna ID | Alta |
| handles validation errors | Integration | Maneja errores 400 correctamente | Alta |
| handles 401 unauthorized | Integration | Maneja error de autenticacion | Alta |
| invalidates queries on success | Integration | Invalida cache de mis-campanias | Alta |
| shows success toast | Integration | Muestra toast de exito | Media |
| shows error toast | Integration | Muestra toast de error | Media |
| resets form on success | Integration | Limpia formulario tras exito | Media |

**Setup:**
```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { useCreateCampania } from '@/hooks/useCreateCampania';
import { AllProviders } from '@/__mocks__/test-utils';
import { server } from '@/__mocks__/handlers';
import { mockCreateCampaniaRequest } from '@/__mocks__/campania.mock';

describe('useCreateCampania', () => {
  it('creates campania successfully', async () => {
    const { result } = renderHook(() => useCreateCampania(), {
      wrapper: AllProviders,
    });

    result.current.mutate(mockCreateCampaniaRequest);

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.data.id).toBeTruthy();
  });

  it('handles validation errors', async () => {
    server.use(campaniaErrorHandlers[3]); // 400 validation error

    const { result } = renderHook(() => useCreateCampania(), {
      wrapper: AllProviders,
    });

    result.current.mutate(mockCreateCampaniaRequest);

    await waitFor(() => {
      expect(result.current.isError).toBe(true);
    });

    expect(result.current.error).toBeDefined();
  });
});
```

---

#### useUpdateCampania.test.ts

**Archivo:** `src/admin/src/__tests__/hooks/useUpdateCampania.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| updates campania successfully | Integration | PUT correcto retorna true | Alta |
| validates only draft can be edited | Integration | Error 409 si campania publicada | Alta |
| handles 403 forbidden | Integration | Error si no es propietario | Alta |
| invalidates specific query | Integration | Invalida cache de campania/{id} | Alta |
| shows success toast | Integration | Muestra toast "Actualizado" | Media |

---

#### usePublishCampania.test.ts

**Archivo:** `src/admin/src/__tests__/hooks/usePublishCampania.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| publishes campania successfully | Integration | POST /publicar retorna estado 2 | Alta |
| validates required fields | Integration | Error si faltan campos obligatorios | Alta |
| updates fechaPublicacion | Integration | Retorna fechaPublicacion actualizada | Alta |
| invalidates all queries | Integration | Invalida mis-campanias y campania/{id} | Alta |
| redirects to list on success | Integration | Redirige a /dashboard/campanias | Alta |

---

#### useMisCampanias.test.ts

**Archivo:** `src/admin/src/__tests__/hooks/useMisCampanias.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| fetches campanias list | Integration | GET /mis-campanias retorna lista | Alta |
| handles empty list | Integration | Retorna array vacio si sin campanias | Media |
| filters by estado | Integration | Aplica filtro estadoCampaniaId | Media |
| shows loading state | Unit | isLoading true inicialmente | Media |
| shows error state | Unit | isError true si falla | Media |

---

#### useWizardNavigation.test.ts

**Archivo:** `src/admin/src/__tests__/hooks/useWizardNavigation.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| initializes on step 0 | Unit | currentStep = 0 al inicio | Alta |
| advances to next step | Unit | nextStep incrementa currentStep | Alta |
| goes back to previous step | Unit | prevStep decrementa currentStep | Alta |
| cannot go below step 0 | Unit | prevStep no va a -1 | Media |
| cannot exceed max steps | Unit | nextStep no pasa del ultimo | Media |
| updates completed steps | Unit | Marca steps completados | Media |

---

### 5.5 Services

#### campania.service.test.ts

**Archivo:** `src/admin/src/__tests__/services/campania.service.test.ts`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| getMisCampanias returns list | Unit | GET correcto retorna campanias | Alta |
| getById returns campania | Unit | GET /{id} retorna campania | Alta |
| create sends correct payload | Unit | POST envia request correcto | Alta |
| update sends correct payload | Unit | PUT envia request correcto | Alta |
| publish calls correct endpoint | Unit | POST /publicar envia ID | Alta |
| includes auth token | Unit | Todas las requests incluyen Bearer token | Alta |
| handles 401 error | Unit | Lanza error en unauthorized | Alta |
| handles network error | Unit | Lanza error en fallo de red | Alta |

**Setup:**
```typescript
import { campaniaService } from '@/services/campania.service';
import { server } from '@/__mocks__/handlers';
import { mockCreateCampaniaRequest, mockCampaniaBorrador } from '@/__mocks__/campania.mock';

describe('campania.service', () => {
  beforeAll(() => server.listen());
  afterEach(() => server.resetHandlers());
  afterAll(() => server.close());

  it('getMisCampanias returns list', async () => {
    const result = await campaniaService.getMisCampanias();

    expect(result.data).toHaveLength(2);
    expect(result.data[0]).toHaveProperty('id');
    expect(result.messages[0].errorCode).toBe('0000');
  });

  it('create sends correct payload', async () => {
    const result = await campaniaService.create(mockCreateCampaniaRequest);

    expect(result.data.id).toBeTruthy();
    expect(result.messages[0].errorCode).toBe('0001');
  });

  it('includes auth token in headers', async () => {
    const requestSpy = vi.spyOn(global, 'fetch');

    await campaniaService.getMisCampanias();

    expect(requestSpy).toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: expect.stringContaining('Bearer '),
        }),
      })
    );
  });
});
```

---

### 5.6 Integration Tests

#### create-campania-flow.test.tsx

**Archivo:** `src/admin/src/__tests__/integration/create-campania-flow.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| completes full wizard flow | Integration | Completa todos los 4 steps + crea campania | Alta |
| validates each step before advancing | Integration | Bloquea avance con datos invalidos | Alta |
| persists data between steps | Integration | Mantiene datos al navegar entre steps | Alta |
| creates draft successfully | Integration | Crea campania en estado Borrador | Alta |
| shows success toast | Integration | Muestra toast "Campania creada" | Alta |
| redirects to preview page | Integration | Redirige a /campanias/{id} | Alta |
| handles API errors gracefully | Integration | Muestra errores del backend | Alta |

**Flow completo:**
```typescript
describe('Create Campania Flow', () => {
  it('completes full wizard flow and creates draft', async () => {
    const { user } = renderWithProviders(<CampaniaWizard mode="create" />);

    // Step 1: Basic Info
    await user.type(screen.getByLabelText(/titulo/i), 'Mi Primer Album');
    await user.click(screen.getByRole('button', { name: /siguiente/i }));

    // Step 2: Funding
    await user.type(screen.getByLabelText(/importe objetivo/i), '5000');
    await user.selectOptions(screen.getByLabelText(/tipo financiacion/i), '1');
    await user.click(screen.getByRole('button', { name: /siguiente/i }));

    // Step 3: Duration
    const fechaFin = addDays(new Date(), 30).toISOString();
    await user.type(screen.getByLabelText(/fecha fin/i), fechaFin);
    await user.click(screen.getByRole('button', { name: /siguiente/i }));

    // Step 4: Media (optional)
    await user.click(screen.getByRole('button', { name: /siguiente/i }));

    // Review: Create Draft
    await user.click(screen.getByRole('button', { name: /guardar borrador/i }));

    await waitFor(() => {
      expect(screen.getByText(/campania creada/i)).toBeInTheDocument();
    });

    expect(mockRouter.push).toHaveBeenCalledWith(expect.stringContaining('/campanias/'));
  });
});
```

---

#### edit-campania-flow.test.tsx

**Archivo:** `src/admin/src/__tests__/integration/edit-campania-flow.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| loads existing campania data | Integration | Carga datos en formulario | Alta |
| allows editing draft campania | Integration | Permite editar si estado = Borrador | Alta |
| blocks editing published campania | Integration | Muestra error 409 si estado = Publicada | Alta |
| updates campania successfully | Integration | PUT actualiza datos correctamente | Alta |
| shows updated toast | Integration | Muestra toast "Actualizado" | Media |

---

#### publish-campania-flow.test.tsx

**Archivo:** `src/admin/src/__tests__/integration/publish-campania-flow.test.tsx`

| Test Case | Tipo | Descripcion | Prioridad |
|-----------|------|-------------|-----------|
| opens confirmation modal | Integration | Muestra modal al hacer click Publicar | Alta |
| validates required fields | Integration | Valida titulo, meta, fechaFin antes de publicar | Alta |
| publishes campania successfully | Integration | POST /publicar cambia estado a 2 | Alta |
| updates UI with new status | Integration | Actualiza badge a "Publicada" | Alta |
| redirects to list page | Integration | Redirige a /dashboard/campanias | Alta |
| shows success toast | Integration | Muestra toast "Publicada exitosamente" | Alta |

---

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Statements |
|---------|--------|-----------|----------|------------|
| components/wizard/BasicInfoStep.tsx | 90% | 95% | 85% | 90% |
| components/wizard/FundingStep.tsx | 88% | 92% | 83% | 88% |
| components/wizard/DurationStep.tsx | 85% | 90% | 80% | 85% |
| components/wizard/MediaStep.tsx | 82% | 88% | 78% | 82% |
| components/wizard/ReviewStep.tsx | 87% | 90% | 82% | 87% |
| components/wizard/CampaniaWizard.tsx | 85% | 88% | 80% | 85% |
| components/wizard/WizardStepper.tsx | 95% | 98% | 92% | 95% |
| components/campanias/CampaniaListCard.tsx | 92% | 95% | 88% | 92% |
| components/campanias/DraftBanner.tsx | 90% | 95% | 85% | 90% |
| components/campanias/PublishConfirmModal.tsx | 88% | 92% | 83% | 88% |
| hooks/useCreateCampania.ts | 95% | 100% | 90% | 95% |
| hooks/useUpdateCampania.ts | 93% | 100% | 88% | 93% |
| hooks/usePublishCampania.ts | 94% | 100% | 90% | 94% |
| hooks/useMisCampanias.ts | 92% | 98% | 87% | 92% |
| hooks/useWizardNavigation.ts | 96% | 100% | 93% | 96% |
| services/campania.service.ts | 97% | 100% | 95% | 97% |

**Meta Global:** 80%+ en todas las metricas

---

## 7. Comandos de Ejecucion

```bash
# Instalar dependencias de testing
npm install -D vitest @vitest/ui @testing-library/react @testing-library/user-event @testing-library/jest-dom msw vitest-mock-extended happy-dom

# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- campanias

# Ejecutar solo integration tests
npm run test -- integration

# Watch mode
npm run test:watch

# UI mode (visual)
npm run test:ui
```

**Agregar scripts a package.json:**
```json
{
  "scripts": {
    "test": "vitest run",
    "test:watch": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest run --coverage"
  }
}
```

---

## 8. CI/CD Integration

**Archivo:** `.github/workflows/admin-tests.yml`

```yaml
name: Admin Tests

on:
  push:
    branches: [master]
    paths:
      - 'src/admin/**'
  pull_request:
    branches: [master]
    paths:
      - 'src/admin/**'

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
          cache-dependency-path: src/admin/package-lock.json

      - name: Install dependencies
        working-directory: src/admin
        run: npm ci

      - name: Run tests with coverage
        working-directory: src/admin
        run: npm run test:coverage

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          files: ./src/admin/coverage/lcov.info
          flags: admin
          name: admin-coverage

      - name: Fail if coverage below threshold
        working-directory: src/admin
        run: |
          COVERAGE=$(cat coverage/coverage-summary.json | jq '.total.lines.pct')
          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "Coverage $COVERAGE% is below 80% threshold"
            exit 1
          fi
```

---

## 9. Estrategias de Mocking Avanzadas

### 9.1 Mock de React Query con Cache

```typescript
// Para tests que requieren cache persistence
import { QueryCache } from '@tanstack/react-query';

export const createTestQueryClientWithCache = () => {
  const queryCache = new QueryCache();

  return new QueryClient({
    queryCache,
    defaultOptions: {
      queries: { retry: false, gcTime: Infinity },
    },
  });
};
```

### 9.2 Mock de Next.js Server Components

```typescript
// Para tests de components que usan cookies o headers
vi.mock('next/headers', () => ({
  cookies: () => ({
    get: vi.fn().mockReturnValue({ value: 'mock-token' }),
  }),
  headers: () => new Headers(),
}));
```

### 9.3 Mock de File Upload

```typescript
// Para tests de upload de imagenes/videos (si se implementa)
const mockFile = new File(['dummy content'], 'test.jpg', { type: 'image/jpeg' });

const dataTransfer = {
  files: [mockFile],
  items: [{
    kind: 'file',
    type: mockFile.type,
    getAsFile: () => mockFile,
  }],
  types: ['Files'],
};
```

---

## 10. Edge Cases y Escenarios Especiales

| Escenario | Tests Requeridos | Prioridad |
|-----------|------------------|-----------|
| Usuario abandona wizard y vuelve | Verifica recuperacion desde localStorage | Alta |
| Token expira durante creacion | Maneja 401 y redirige a login | Alta |
| Network timeout | Muestra error "Tiempo de espera agotado" | Media |
| Campania ya existe (titulo duplicado) | Muestra error 1008 | Media |
| Fecha fin en el pasado | Bloquea creacion con error | Alta |
| Importe objetivo muy grande (> 1M) | Valida limite maximo razonable | Baja |
| Usuario sin perfil de artista | Bloquea acceso a crear campania | Alta |
| Multi-tab editing | Advierte conflictos de edicion simultanea | Baja |

---

## 11. Performance Testing

### 11.1 Metricas de Performance

```typescript
// Ejemplo de test de performance para wizard
it('renders wizard in less than 500ms', async () => {
  const start = performance.now();

  renderWithProviders(<CampaniaWizard />);

  await waitFor(() => {
    expect(screen.getByText(/informacion basica/i)).toBeInTheDocument();
  });

  const end = performance.now();
  expect(end - start).toBeLessThan(500);
});
```

### 11.2 Memory Leaks

```typescript
// Test para detectar memory leaks en queries
it('cleans up queries on unmount', () => {
  const { unmount } = renderWithProviders(<CampaniaWizard />);

  const queryClient = useQueryClient();
  const initialQueries = queryClient.getQueryCache().getAll().length;

  unmount();

  const finalQueries = queryClient.getQueryCache().getAll().length;
  expect(finalQueries).toBeLessThanOrEqual(initialQueries);
});
```

---

## 12. Checklist de Testing

- [ ] Vitest configurado con happy-dom
- [ ] MSW configurado con handlers de campania
- [ ] Test utilities creados (renderWithProviders, etc)
- [ ] Mocks de campania definidos
- [ ] Tests de BasicInfoStep (10 tests)
- [ ] Tests de FundingStep (8 tests)
- [ ] Tests de DurationStep (7 tests)
- [ ] Tests de MediaStep (6 tests)
- [ ] Tests de ReviewStep (8 tests)
- [ ] Tests de CampaniaWizard (8 tests)
- [ ] Tests de WizardStepper (6 tests)
- [ ] Tests de CampaniaListCard (9 tests)
- [ ] Tests de DraftBanner (6 tests)
- [ ] Tests de PublishConfirmModal (8 tests)
- [ ] Tests de useCreateCampania (8 tests)
- [ ] Tests de useUpdateCampania (5 tests)
- [ ] Tests de usePublishCampania (5 tests)
- [ ] Tests de useMisCampanias (5 tests)
- [ ] Tests de useWizardNavigation (6 tests)
- [ ] Tests de campania.service (8 tests)
- [ ] Integration test: create-campania-flow (7 tests)
- [ ] Integration test: edit-campania-flow (5 tests)
- [ ] Integration test: publish-campania-flow (6 tests)
- [ ] Tests de edge cases criticos
- [ ] Cobertura >= 80% verificada
- [ ] CI/CD workflow configurado
- [ ] Scripts de test en package.json

---

## 13. Notas Tecnicas Adicionales

### 13.1 Testing con React Hook Form

```typescript
// Pattern para testear formularios con RHF
const { result } = renderHook(() => useForm({
  resolver: zodResolver(campaniaBasicInfoSchema),
  defaultValues: { titulo: '', subtitulo: '' },
}));

act(() => {
  result.current.setValue('titulo', 'Test');
});

await waitFor(() => {
  expect(result.current.formState.isValid).toBe(true);
});
```

### 13.2 Testing de Toasts

```typescript
// Verificar que se muestra un toast
import { toast } from 'sonner';

vi.spyOn(toast, 'success');

// ... ejecutar accion ...

expect(toast.success).toHaveBeenCalledWith('Campania creada correctamente');
```

### 13.3 Testing de Fechas

```typescript
// Mock de date-fns para tests consistentes
import { addDays } from 'date-fns';

vi.mock('date-fns', () => ({
  ...vi.importActual('date-fns'),
  addDays: vi.fn((date, days) => new Date('2026-03-15')),
}));
```

---

## 14. Mantenimiento del Plan

Este plan de testing debe actualizarse cuando:
- Se agreguen nuevos componentes al wizard
- Se modifiquen validaciones en schemas Zod
- Se cambien endpoints o contratos de API
- Se agreguen nuevas features a campanias (ej: rewards)

**Responsable:** Lead Frontend
**Frecuencia de revision:** Cada sprint

---

**Fin del Plan de Testing**
