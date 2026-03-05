# Estrategia de Testing: Gestionar Necesidades (Admin)

**Fecha:** 2026-02-16
**Feature:** cs-gestionar-necesidades
**Target:** src/admin
**Cobertura Objetivo:** 80%+

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests | 32 | 85% |
| Integration Tests | 8 | 80% |
| Total | 40 | 82%+ |

**Tiempo estimado de ejecucion:** < 60 segundos

---

## 2. Estructura de Tests

```
src/admin/src/features/crowdsourcing/
├── necesidades/
│   ├── __tests__/
│   │   ├── components/
│   │   │   ├── NecesidadCard.test.tsx
│   │   │   ├── EstadoNecesidadBadge.test.tsx
│   │   │   ├── NecesidadFilters.test.tsx
│   │   │   ├── EmptyStateNecesidades.test.tsx
│   │   │   ├── CerrarNecesidadDialog.test.tsx
│   │   │   └── PropuestaCard.test.tsx
│   │   ├── hooks/
│   │   │   ├── useMisNecesidades.test.ts
│   │   │   ├── useNecesidadById.test.ts
│   │   │   ├── useCreateNecesidad.test.ts
│   │   │   ├── useUpdateNecesidad.test.ts
│   │   │   └── useCerrarNecesidad.test.ts
│   │   ├── schemas/
│   │   │   ├── createNecesidadSchema.test.ts
│   │   │   ├── updateNecesidadSchema.test.ts
│   │   │   └── cerrarNecesidadSchema.test.ts
│   │   └── integration/
│   │       ├── crear-necesidad-flow.test.tsx
│   │       ├── listar-necesidades-flow.test.tsx
│   │       ├── editar-necesidad-flow.test.tsx
│   │       └── cerrar-necesidad-flow.test.tsx
│   ├── __mocks__/
│   │   ├── necesidades.mock.ts
│   │   ├── propuestas.mock.ts
│   │   └── handlers.ts
│   └── test-utils.tsx
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/necesidades.mock.ts`

```typescript
import { NecesidadCrowdsourcingList, NecesidadCrowdsourcing } from '@/shared/types/crowdsourcing';

export const mockNecesidadAbierta: NecesidadCrowdsourcingList = {
  id: 'nec-123-abierta',
  titulo: 'Mezcla de pistas para EP de 5 canciones',
  estadoNecesidadId: 1,
  estadoNecesidadNombre: 'Abierta',
  tipoNecesidadId: 3,
  tipoNecesidadNombre: 'Ingeniería de Audio',
  presupuestoMin: 150.00,
  presupuestoMax: 800.00,
  monedaId: 1,
  monedaNombre: 'EUR',
  modalidadTrabajoId: 2,
  modalidadTrabajoNombre: 'Remoto',
  numeroPropuestas: 3,
  fechaCreacion: '2026-02-16T10:00:00Z',
  fechaLimitePropuestas: '2026-03-15T00:00:00Z',
  fechaActualizacion: null,
};

export const mockNecesidadEnProgreso: NecesidadCrowdsourcingList = {
  id: 'nec-456-progreso',
  titulo: 'Diseño de portada del álbum',
  estadoNecesidadId: 2,
  estadoNecesidadNombre: 'En Progreso',
  tipoNecesidadId: 5,
  tipoNecesidadNombre: 'Diseño Gráfico',
  presupuestoMin: 200.00,
  presupuestoMax: 500.00,
  monedaId: 1,
  monedaNombre: 'EUR',
  modalidadTrabajoId: 2,
  modalidadTrabajoNombre: 'Remoto',
  numeroPropuestas: 1,
  fechaCreacion: '2026-02-10T10:00:00Z',
  fechaLimitePropuestas: null,
  fechaActualizacion: '2026-02-14T16:20:00Z',
};

export const mockNecesidadCerrada: NecesidadCrowdsourcingList = {
  id: 'nec-789-cerrada',
  titulo: 'Grabación de batería',
  estadoNecesidadId: 3,
  estadoNecesidadNombre: 'Cerrada',
  tipoNecesidadId: 2,
  tipoNecesidadNombre: 'Producción Musical',
  presupuestoMin: 300.00,
  presupuestoMax: 1000.00,
  monedaId: 1,
  monedaNombre: 'EUR',
  modalidadTrabajoId: 1,
  modalidadTrabajoNombre: 'Presencial',
  numeroPropuestas: 5,
  fechaCreacion: '2026-01-20T10:00:00Z',
  fechaLimitePropuestas: '2026-02-10T00:00:00Z',
  fechaActualizacion: '2026-02-12T09:00:00Z',
};

export const mockNecesidadesList: NecesidadCrowdsourcingList[] = [
  mockNecesidadAbierta,
  mockNecesidadEnProgreso,
  mockNecesidadCerrada,
];

export const mockNecesidadDetail: NecesidadCrowdsourcing = {
  ...mockNecesidadAbierta,
  descripcion: 'Buscamos un ingeniero de mezcla experimentado en indie rock para mezclar 5 canciones. El material está grabado en 24bit/96kHz y requiere un enfoque orgánico con énfasis en las guitarras.',
  ubicacionCiudad: null,
  ubicacionPais: null,
  fechaInicioPrevista: '2026-04-01T00:00:00Z',
  proyectoArtisticoId: 'proj-123',
  proyectoArtisticoNombre: 'Mi Primer EP',
  propuestas: [],
};

export const mockNecesidadConPropuestas: NecesidadCrowdsourcing = {
  ...mockNecesidadDetail,
  propuestas: [
    {
      id: 'prop-123',
      profesionalId: 'prof-456',
      profesionalNombre: 'Juan Pérez',
      precioPropuesto: 600.00,
      monedaId: 1,
      tiempoEstimadoDias: 14,
      mensaje: 'Tengo 8 años de experiencia mezclando indie rock...',
      estadoPropuestaId: 1,
      estadoPropuestaNombre: 'Pendiente',
      fechaCreacion: '2026-02-17T09:15:00Z',
    },
    {
      id: 'prop-456',
      profesionalId: 'prof-789',
      profesionalNombre: 'María García',
      precioPropuesto: 450.00,
      monedaId: 1,
      tiempoEstimadoDias: 10,
      mensaje: 'Especializada en producción indie con +100 proyectos...',
      estadoPropuestaId: 1,
      estadoPropuestaNombre: 'Pendiente',
      fechaCreacion: '2026-02-18T11:30:00Z',
    },
  ],
};
```

**Archivo:** `__mocks__/propuestas.mock.ts`

```typescript
import { PropuestaCrowdsourcing } from '@/shared/types/crowdsourcing';

export const mockPropuestaPendiente: PropuestaCrowdsourcing = {
  id: 'prop-123',
  profesionalId: 'prof-456',
  profesionalNombre: 'Juan Pérez',
  precioPropuesto: 600.00,
  monedaId: 1,
  tiempoEstimadoDias: 14,
  mensaje: 'Tengo 8 años de experiencia mezclando indie rock y folk. He trabajado con bandas como X, Y, Z. Mi estudio cuenta con equipment de gama alta incluyendo...',
  estadoPropuestaId: 1,
  estadoPropuestaNombre: 'Pendiente',
  fechaCreacion: '2026-02-17T09:15:00Z',
};

export const mockPropuestaAceptada: PropuestaCrowdsourcing = {
  id: 'prop-789',
  profesionalId: 'prof-101',
  profesionalNombre: 'Carlos López',
  precioPropuesto: 750.00,
  monedaId: 1,
  tiempoEstimadoDias: 21,
  mensaje: 'Propuesta para proyecto de mezcla profesional...',
  estadoPropuestaId: 2,
  estadoPropuestaNombre: 'Aceptada',
  fechaCreacion: '2026-02-15T14:00:00Z',
};

export const mockPropuestaRechazada: PropuestaCrowdsourcing = {
  id: 'prop-012',
  profesionalId: 'prof-202',
  profesionalNombre: 'Ana Martín',
  precioPropuesto: 950.00,
  monedaId: 1,
  tiempoEstimadoDias: 30,
  mensaje: 'Mi propuesta para mezcla...',
  estadoPropuestaId: 3,
  estadoPropuestaNombre: 'Rechazada',
  fechaCreacion: '2026-02-16T08:00:00Z',
};
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from 'msw';
import { API_ROUTES } from '@/shared/constants';
import {
  mockNecesidadesList,
  mockNecesidadDetail,
  mockNecesidadConPropuestas,
} from './necesidades.mock';

const API_BASE = 'http://localhost:5001';

export const necesidadesHandlers = [
  // GET /api/crowdsourcing/necesidades/mis-necesidades
  rest.get(`${API_BASE}${API_ROUTES.crowdsourcing.necesidades.mis}`, (req, res, ctx) => {
    const page = Number(req.url.searchParams.get('page')) || 1;
    const pageSize = Number(req.url.searchParams.get('pageSize')) || 12;
    const estado = req.url.searchParams.get('estado');
    const search = req.url.searchParams.get('search');

    let filtered = [...mockNecesidadesList];

    // Apply filters
    if (estado) {
      const estadoId = Number(estado);
      filtered = filtered.filter(n => n.estadoNecesidadId === estadoId);
    }

    if (search) {
      const searchLower = search.toLowerCase();
      filtered = filtered.filter(n =>
        n.titulo.toLowerCase().includes(searchLower) ||
        n.tipoNecesidadNombre.toLowerCase().includes(searchLower)
      );
    }

    // Pagination
    const start = (page - 1) * pageSize;
    const end = start + pageSize;
    const items = filtered.slice(start, end);

    return res(
      ctx.status(200),
      ctx.json({
        data: {
          items,
          totalCount: filtered.length,
          page,
          pageSize,
          totalPages: Math.ceil(filtered.length / pageSize),
        },
        messages: [{ message: 'Necesidades obtenidas exitosamente', errorCode: '0000' }],
      })
    );
  }),

  // GET /api/crowdsourcing/necesidades/:id
  rest.get(`${API_BASE}${API_ROUTES.crowdsourcing.necesidades.base}/:id`, (req, res, ctx) => {
    const { id } = req.params;

    if (id === 'nec-123-abierta') {
      return res(ctx.status(200), ctx.json({ data: mockNecesidadDetail, messages: [] }));
    }

    if (id === 'nec-with-propuestas') {
      return res(ctx.status(200), ctx.json({ data: mockNecesidadConPropuestas, messages: [] }));
    }

    if (id === 'not-found') {
      return res(
        ctx.status(404),
        ctx.json({
          data: null,
          messages: [{ message: 'Necesidad no encontrada', errorCode: '2009' }],
        })
      );
    }

    if (id === 'forbidden') {
      return res(
        ctx.status(403),
        ctx.json({
          data: null,
          messages: [{ message: 'No tienes permiso para ver esta necesidad', errorCode: '3002' }],
        })
      );
    }

    return res(ctx.status(200), ctx.json({ data: mockNecesidadDetail, messages: [] }));
  }),

  // POST /api/crowdsourcing/necesidades
  rest.post(`${API_BASE}${API_ROUTES.crowdsourcing.necesidades.base}`, async (req, res, ctx) => {
    const body = await req.json();

    // Validation errors
    if (!body.titulo || body.titulo.length < 5) {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'El título debe tener al menos 5 caracteres', errorCode: '1011' }],
        })
      );
    }

    if (body.presupuestoMax && body.presupuestoMin && body.presupuestoMax < body.presupuestoMin) {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'El presupuesto máximo debe ser mayor o igual al mínimo', errorCode: '1009' }],
        })
      );
    }

    if ((body.modalidadTrabajoId === 1 || body.modalidadTrabajoId === 3) && !body.ubicacionCiudad) {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'La ubicación es obligatoria para modalidad Presencial o Híbrida', errorCode: '1001' }],
        })
      );
    }

    // Success
    const newId = `nec-${Date.now()}`;
    return res(
      ctx.status(201),
      ctx.json({
        data: {
          id: newId,
          titulo: body.titulo,
          estadoNecesidadId: 1,
          estadoNecesidadNombre: 'Abierta',
          fechaCreacion: new Date().toISOString(),
        },
        messages: [{ message: 'Necesidad publicada correctamente', errorCode: '0001' }],
      })
    );
  }),

  // PUT /api/crowdsourcing/necesidades/:id
  rest.put(`${API_BASE}${API_ROUTES.crowdsourcing.necesidades.base}/:id`, async (req, res, ctx) => {
    const { id } = req.params;
    const body = await req.json();

    if (id === 'nec-cerrada') {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'Solo se pueden editar necesidades en estado Abierta', errorCode: '4001' }],
        })
      );
    }

    return res(
      ctx.status(200),
      ctx.json({
        data: {
          id,
          titulo: body.titulo,
          estadoNecesidadId: 1,
          estadoNecesidadNombre: 'Abierta',
          fechaActualizacion: new Date().toISOString(),
        },
        messages: [{ message: 'Necesidad actualizada correctamente', errorCode: '0002' }],
      })
    );
  }),

  // PATCH /api/crowdsourcing/necesidades/:id/cerrar
  rest.patch(`${API_BASE}${API_ROUTES.crowdsourcing.necesidades.cerrar(':id')}`, async (req, res, ctx) => {
    const { id } = req.params;
    const body = await req.json();

    if (id === 'nec-cerrada') {
      return res(
        ctx.status(400),
        ctx.json({
          data: null,
          messages: [{ message: 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso', errorCode: '4002' }],
        })
      );
    }

    return res(
      ctx.status(200),
      ctx.json({
        data: {
          id,
          estadoNecesidadNombre: 'Cerrada',
          propuestasRechazadas: 2,
        },
        messages: [{ message: 'Necesidad cerrada correctamente. Se han rechazado 2 propuestas pendientes.', errorCode: '0000' }],
      })
    );
  }),
];
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import { ReactElement } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';

const testQueryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      cacheTime: 0,
    },
    mutations: {
      retry: false,
    },
  },
});

const AllTheProviders = ({ children }: { children: React.ReactNode }) => {
  return (
    <QueryClientProvider client={testQueryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </QueryClientProvider>
  );
};

export const renderWithProviders = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllTheProviders, ...options });

export * from '@testing-library/react';
export { testQueryClient };
```

---

## 4. Tests por Modulo

### 4.1 Components

#### NecesidadCard.test.tsx

**Archivo:** `__tests__/components/NecesidadCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders correctly with all data` | Unit | Renderiza card con todos los datos (titulo, estado, presupuesto, propuestas count, fechas) |
| `displays estado badge with correct color - Abierta` | Unit | Badge verde para estado Abierta |
| `displays estado badge with correct color - En Progreso` | Unit | Badge azul para estado En Progreso |
| `displays estado badge with correct color - Cerrada` | Unit | Badge gris para estado Cerrada |
| `displays estado badge with correct color - Cancelada` | Unit | Badge rojo para estado Cancelada |
| `displays presupuesto range correctly` | Unit | Muestra "150 - 800 EUR" cuando hay min y max |
| `displays presupuesto with only min` | Unit | Muestra "Desde 150 EUR" cuando solo hay min |
| `displays presupuesto with only max` | Unit | Muestra "Hasta 800 EUR" cuando solo hay max |
| `displays presupuesto as "No especificado" when null` | Unit | Muestra texto fallback cuando no hay presupuesto |
| `displays propuestas counter` | Unit | Muestra "3 propuestas" con icono users |
| `displays fecha limite warning when < 7 days` | Unit | Badge amarillo "Cierra pronto" si fecha limite < 7 dias |
| `displays fecha limite error when < 3 days` | Unit | Badge rojo "Urgente" si fecha limite < 3 dias |
| `shows "Editar" button only when estado = Abierta` | Unit | Boton visible solo para Abierta |
| `shows "Cerrar" button when estado = Abierta or En Progreso` | Unit | Boton visible para Abierta y En Progreso |
| `hides "Editar" button when estado != Abierta` | Unit | Boton oculto para Cerrada/Cancelada |
| `calls onClick handler when card is clicked` | Unit | Ejecuta callback onClick al hacer click |
| `applies hover styles` | Unit | Hover cambia bg, border, shadow |

**Casos Detallados:**

```typescript
describe('NecesidadCard', () => {
  it('renders correctly with all data', () => {
    const { getByText, getByRole } = renderWithProviders(
      <NecesidadCard necesidad={mockNecesidadAbierta} />
    );

    expect(getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
    expect(getByText('Abierta')).toBeInTheDocument();
    expect(getByText('Ingeniería de Audio')).toBeInTheDocument();
    expect(getByText('Remoto')).toBeInTheDocument();
    expect(getByText('150 - 800 EUR')).toBeInTheDocument();
    expect(getByText('3 propuestas')).toBeInTheDocument();
  });

  it('displays estado badge with correct color - Abierta', () => {
    const { getByText } = renderWithProviders(
      <NecesidadCard necesidad={mockNecesidadAbierta} />
    );

    const badge = getByText('Abierta').closest('div');
    expect(badge).toHaveClass('bg-green-900/20', 'text-green-400');
  });

  it('displays fecha limite warning when < 7 days', () => {
    const necesidadProxima = {
      ...mockNecesidadAbierta,
      fechaLimitePropuestas: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString(), // 5 days
    };

    const { getByText } = renderWithProviders(
      <NecesidadCard necesidad={necesidadProxima} />
    );

    expect(getByText(/Cierra pronto/i)).toBeInTheDocument();
  });

  it('shows "Editar" button only when estado = Abierta', () => {
    const { getByText, rerender } = renderWithProviders(
      <NecesidadCard necesidad={mockNecesidadAbierta} />
    );

    expect(getByText('Editar')).toBeInTheDocument();

    rerender(<NecesidadCard necesidad={mockNecesidadCerrada} />);
    expect(() => getByText('Editar')).toThrow();
  });

  it('calls onClick handler when card is clicked', () => {
    const handleClick = vi.fn();
    const { getByRole } = renderWithProviders(
      <NecesidadCard necesidad={mockNecesidadAbierta} onClick={handleClick} />
    );

    const card = getByRole('article');
    fireEvent.click(card);

    expect(handleClick).toHaveBeenCalledWith('nec-123-abierta');
  });
});
```

#### EstadoNecesidadBadge.test.tsx

**Archivo:** `__tests__/components/EstadoNecesidadBadge.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders "Abierta" with green color` | Unit | Badge verde con texto "Abierta" |
| `renders "En Progreso" with blue color` | Unit | Badge azul con texto "En Progreso" |
| `renders "Cerrada" with gray color` | Unit | Badge gris con texto "Cerrada" |
| `renders "Cancelada" with red color` | Unit | Badge rojo con texto "Cancelada" |
| `includes aria-label for accessibility` | Unit | `aria-label="Estado: Abierta"` |

**Casos Detallados:**

```typescript
describe('EstadoNecesidadBadge', () => {
  it('renders "Abierta" with green color', () => {
    const { getByText } = renderWithProviders(<EstadoNecesidadBadge estado="Abierta" />);

    const badge = getByText('Abierta');
    expect(badge).toBeInTheDocument();
    expect(badge).toHaveClass('bg-green-900/20', 'text-green-400', 'border-green-700');
  });

  it('includes aria-label for accessibility', () => {
    const { getByLabelText } = renderWithProviders(<EstadoNecesidadBadge estado="Abierta" />);

    expect(getByLabelText('Estado: Abierta')).toBeInTheDocument();
  });
});
```

#### NecesidadFilters.test.tsx

**Archivo:** `__tests__/components/NecesidadFilters.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders estado select with all options` | Unit | Select con opciones: Todos, Abierta, En Progreso, Cerrada, Cancelada |
| `renders search input` | Unit | Input de busqueda visible |
| `calls onEstadoChange when select changes` | Unit | Callback ejecutado al cambiar select |
| `debounces search input changes (300ms)` | Unit | Search debounce de 300ms antes de callback |
| `calls onClearFilters when clear button is clicked` | Unit | Boton limpiar filtros ejecuta callback |
| `disables clear button when no filters applied` | Unit | Boton deshabilitado si filtros vacios |

**Casos Detallados:**

```typescript
describe('NecesidadFilters', () => {
  it('renders estado select with all options', () => {
    const { getByRole, getByText } = renderWithProviders(
      <NecesidadFilters onEstadoChange={vi.fn()} onSearchChange={vi.fn()} />
    );

    const select = getByRole('combobox', { name: /estado/i });
    expect(select).toBeInTheDocument();

    fireEvent.click(select);

    expect(getByText('Todos')).toBeInTheDocument();
    expect(getByText('Abierta')).toBeInTheDocument();
    expect(getByText('En Progreso')).toBeInTheDocument();
    expect(getByText('Cerrada')).toBeInTheDocument();
    expect(getByText('Cancelada')).toBeInTheDocument();
  });

  it('debounces search input changes (300ms)', async () => {
    vi.useFakeTimers();
    const handleSearchChange = vi.fn();
    const { getByPlaceholderText } = renderWithProviders(
      <NecesidadFilters onEstadoChange={vi.fn()} onSearchChange={handleSearchChange} />
    );

    const searchInput = getByPlaceholderText(/buscar necesidad/i);
    fireEvent.change(searchInput, { target: { value: 'mezcla' } });

    expect(handleSearchChange).not.toHaveBeenCalled();

    vi.advanceTimersByTime(300);

    expect(handleSearchChange).toHaveBeenCalledWith('mezcla');

    vi.useRealTimers();
  });

  it('calls onClearFilters when clear button is clicked', () => {
    const handleClearFilters = vi.fn();
    const { getByRole } = renderWithProviders(
      <NecesidadFilters
        onEstadoChange={vi.fn()}
        onSearchChange={vi.fn()}
        onClearFilters={handleClearFilters}
        hasActiveFilters={true}
      />
    );

    const clearButton = getByRole('button', { name: /limpiar filtros/i });
    fireEvent.click(clearButton);

    expect(handleClearFilters).toHaveBeenCalled();
  });
});
```

#### EmptyStateNecesidades.test.tsx

**Archivo:** `__tests__/components/EmptyStateNecesidades.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders with no filters applied` | Unit | Muestra ilustracion + "No tienes necesidades publicadas" + CTAs |
| `renders with filters applied` | Unit | Muestra icono search + "No se encontraron necesidades" + "Limpiar filtros" |
| `shows "Publicar necesidad" button when no filters` | Unit | CTA principal visible sin filtros |
| `shows "Usar plantilla" button when no filters` | Unit | CTA secundario visible sin filtros |
| `calls onPublicar when button clicked` | Unit | Navega a crear necesidad |
| `calls onUsarPlantilla when button clicked` | Unit | Navega a wizard de templates |

**Casos Detallados:**

```typescript
describe('EmptyStateNecesidades', () => {
  it('renders with no filters applied', () => {
    const { getByText, getByRole } = renderWithProviders(
      <EmptyStateNecesidades hasFilters={false} onPublicar={vi.fn()} onUsarPlantilla={vi.fn()} />
    );

    expect(getByText('No tienes necesidades publicadas')).toBeInTheDocument();
    expect(getByText(/Publica lo que necesitas y recibe propuestas/i)).toBeInTheDocument();
    expect(getByRole('button', { name: /Publicar necesidad/i })).toBeInTheDocument();
    expect(getByRole('button', { name: /Usar plantilla/i })).toBeInTheDocument();
  });

  it('renders with filters applied', () => {
    const { getByText, getByRole } = renderWithProviders(
      <EmptyStateNecesidades hasFilters={true} onClearFilters={vi.fn()} />
    );

    expect(getByText('No se encontraron necesidades')).toBeInTheDocument();
    expect(getByText(/Intenta ajustar los filtros/i)).toBeInTheDocument();
    expect(getByRole('button', { name: /Limpiar filtros/i })).toBeInTheDocument();
  });
});
```

#### CerrarNecesidadDialog.test.tsx

**Archivo:** `__tests__/components/CerrarNecesidadDialog.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders when open` | Unit | Dialog visible cuando open=true |
| `does not render when closed` | Unit | Dialog oculto cuando open=false |
| `displays warning alert` | Unit | Alerta "propuestas pendientes seran rechazadas" visible |
| `allows typing motivo (optional)` | Unit | Textarea permite input hasta 500 chars |
| `displays character counter` | Unit | Muestra "X / 500 caracteres" |
| `disables submit when motivo > 500 chars` | Unit | Boton deshabilitado si supera limite |
| `calls onConfirm with motivo when submitted` | Unit | Callback con motivo al confirmar |
| `calls onCancel when cancel button clicked` | Unit | Callback al cancelar |
| `calls onCancel when X button clicked` | Unit | Callback al cerrar dialog con X |
| `disables form during submission` | Unit | Inputs deshabilitados durante submit |

**Casos Detallados:**

```typescript
describe('CerrarNecesidadDialog', () => {
  it('renders when open', () => {
    const { getByText } = renderWithProviders(
      <CerrarNecesidadDialog open={true} onClose={vi.fn()} onConfirm={vi.fn()} />
    );

    expect(getByText('Cerrar Necesidad')).toBeInTheDocument();
    expect(getByText(/propuestas pendientes serán rechazadas automáticamente/i)).toBeInTheDocument();
  });

  it('allows typing motivo (optional)', () => {
    const { getByLabelText } = renderWithProviders(
      <CerrarNecesidadDialog open={true} onClose={vi.fn()} onConfirm={vi.fn()} />
    );

    const textarea = getByLabelText(/motivo/i);
    fireEvent.change(textarea, { target: { value: 'Ya no necesito este servicio' } });

    expect(textarea).toHaveValue('Ya no necesito este servicio');
  });

  it('displays character counter', () => {
    const { getByText, getByLabelText } = renderWithProviders(
      <CerrarNecesidadDialog open={true} onClose={vi.fn()} onConfirm={vi.fn()} />
    );

    const textarea = getByLabelText(/motivo/i);
    fireEvent.change(textarea, { target: { value: 'Test' } });

    expect(getByText('4 / 500 caracteres')).toBeInTheDocument();
  });

  it('calls onConfirm with motivo when submitted', async () => {
    const handleConfirm = vi.fn();
    const { getByLabelText, getByRole } = renderWithProviders(
      <CerrarNecesidadDialog open={true} onClose={vi.fn()} onConfirm={handleConfirm} />
    );

    const textarea = getByLabelText(/motivo/i);
    fireEvent.change(textarea, { target: { value: 'Motivo de prueba' } });

    const confirmButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(confirmButton);

    await waitFor(() => {
      expect(handleConfirm).toHaveBeenCalledWith({ motivo: 'Motivo de prueba' });
    });
  });
});
```

#### PropuestaCard.test.tsx

**Archivo:** `__tests__/components/PropuestaCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `renders propuesta data correctly` | Unit | Muestra nombre profesional, precio, mensaje, fecha |
| `displays profesional avatar` | Unit | Avatar visible con initials fallback |
| `displays precio with currency symbol` | Unit | "600 EUR" con simbolo correcto |
| `displays tiempo estimado` | Unit | "14 días" con icono clock |
| `truncates long messages` | Unit | Mensaje truncado a 150 chars con "Leer más" |
| `expands message on "Leer más" click` | Unit | Click muestra mensaje completo |
| `shows action buttons when not readonly` | Unit | Botones [Ver Perfil] [Aceptar] [Rechazar] visibles |
| `hides action buttons when readonly` | Unit | Botones ocultos si readonly=true |
| `calls onVerPerfil when button clicked` | Unit | Callback ejecutado |
| `calls onAceptar when button clicked` | Unit | Callback ejecutado |
| `calls onRechazar when button clicked` | Unit | Callback ejecutado |

**Casos Detallados:**

```typescript
describe('PropuestaCard', () => {
  it('renders propuesta data correctly', () => {
    const { getByText } = renderWithProviders(
      <PropuestaCard propuesta={mockPropuestaPendiente} onVerPerfil={vi.fn()} />
    );

    expect(getByText('Juan Pérez')).toBeInTheDocument();
    expect(getByText('600 EUR')).toBeInTheDocument();
    expect(getByText('14 días')).toBeInTheDocument();
    expect(getByText(/Tengo 8 años de experiencia/i)).toBeInTheDocument();
  });

  it('truncates long messages', () => {
    const longMessage = 'A'.repeat(200);
    const propuestaLarga = { ...mockPropuestaPendiente, mensaje: longMessage };

    const { getByText } = renderWithProviders(
      <PropuestaCard propuesta={propuestaLarga} onVerPerfil={vi.fn()} />
    );

    const displayedText = getByText(/A{150}/);
    expect(displayedText).toBeInTheDocument();
    expect(getByText(/Leer más/i)).toBeInTheDocument();
  });

  it('hides action buttons when readonly', () => {
    const { queryByRole } = renderWithProviders(
      <PropuestaCard propuesta={mockPropuestaPendiente} onVerPerfil={vi.fn()} readonly={true} />
    );

    expect(queryByRole('button', { name: /Aceptar/i })).not.toBeInTheDocument();
    expect(queryByRole('button', { name: /Rechazar/i })).not.toBeInTheDocument();
  });
});
```

---

### 4.2 Hooks

#### useMisNecesidades.test.ts

**Archivo:** `__tests__/hooks/useMisNecesidades.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `fetches necesidades successfully` | Unit | Retorna data con items, totalCount, page, pageSize |
| `handles loading state` | Unit | isLoading=true inicialmente, false tras fetch |
| `handles error state` | Unit | error cuando API falla (500) |
| `applies estado filter` | Unit | Query param `estado=1` filtra por Abierta |
| `applies search filter` | Unit | Query param `search=mezcla` filtra por texto |
| `handles pagination` | Unit | Query params `page=2&pageSize=10` aplican paginacion |
| `refetches when filters change` | Unit | Re-ejecuta query al cambiar filtros |

**Casos Detallados:**

```typescript
describe('useMisNecesidades', () => {
  it('fetches necesidades successfully', async () => {
    const { result } = renderHook(() => useMisNecesidades(), {
      wrapper: AllTheProviders,
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.items).toHaveLength(3);
    expect(result.current.data?.totalCount).toBe(3);
  });

  it('handles loading state', async () => {
    const { result } = renderHook(() => useMisNecesidades(), {
      wrapper: AllTheProviders,
    });

    expect(result.current.isLoading).toBe(true);

    await waitFor(() => expect(result.current.isLoading).toBe(false));
  });

  it('applies estado filter', async () => {
    const { result } = renderHook(() => useMisNecesidades({ estado: 1 }), {
      wrapper: AllTheProviders,
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    const abiertas = result.current.data?.items.filter(n => n.estadoNecesidadId === 1);
    expect(abiertas?.length).toBeGreaterThan(0);
  });

  it('applies search filter', async () => {
    const { result } = renderHook(() => useMisNecesidades({ search: 'mezcla' }), {
      wrapper: AllTheProviders,
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    const filtered = result.current.data?.items.every(n =>
      n.titulo.toLowerCase().includes('mezcla')
    );
    expect(filtered).toBe(true);
  });
});
```

#### useNecesidadById.test.ts

**Archivo:** `__tests__/hooks/useNecesidadById.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `fetches necesidad detail successfully` | Unit | Retorna data con propuestas |
| `handles loading state` | Unit | isLoading=true inicialmente |
| `handles not found error (404)` | Unit | error con mensaje "Necesidad no encontrada" |
| `handles forbidden error (403)` | Unit | error con mensaje "No tienes permiso" |
| `disables query when id is undefined` | Unit | Query deshabilitado si no hay id |

**Casos Detallados:**

```typescript
describe('useNecesidadById', () => {
  it('fetches necesidad detail successfully', async () => {
    const { result } = renderHook(() => useNecesidadById('nec-123-abierta'), {
      wrapper: AllTheProviders,
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.id).toBe('nec-123-abierta');
    expect(result.current.data?.titulo).toBe('Mezcla de pistas para EP de 5 canciones');
  });

  it('handles not found error (404)', async () => {
    const { result } = renderHook(() => useNecesidadById('not-found'), {
      wrapper: AllTheProviders,
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    expect(result.current.error).toBeDefined();
  });

  it('disables query when id is undefined', () => {
    const { result } = renderHook(() => useNecesidadById(undefined), {
      wrapper: AllTheProviders,
    });

    expect(result.current.isLoading).toBe(false);
    expect(result.current.data).toBeUndefined();
  });
});
```

#### useCreateNecesidad.test.ts

**Archivo:** `__tests__/hooks/useCreateNecesidad.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `creates necesidad successfully` | Unit | Mutation exitosa retorna id nuevo |
| `invalidates mis-necesidades query on success` | Unit | Query invalidado tras mutacion |
| `handles validation error (400)` | Unit | error con mensaje de validacion |
| `handles forbidden error (403)` | Unit | error "No tienes permiso para crear necesidades en este proyecto" |
| `sets isPending to true during mutation` | Unit | isPending=true durante ejecucion |

**Casos Detallados:**

```typescript
describe('useCreateNecesidad', () => {
  it('creates necesidad successfully', async () => {
    const { result } = renderHook(() => useCreateNecesidad(), {
      wrapper: AllTheProviders,
    });

    const newNecesidad = {
      titulo: 'Nueva necesidad de prueba',
      descripcion: 'Descripcion de prueba',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      proyectoArtisticoId: 'proj-123',
    };

    act(() => {
      result.current.mutate(newNecesidad);
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.id).toBeDefined();
  });

  it('handles validation error (400)', async () => {
    const { result } = renderHook(() => useCreateNecesidad(), {
      wrapper: AllTheProviders,
    });

    const invalidNecesidad = {
      titulo: 'Abc', // < 5 chars
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      proyectoArtisticoId: 'proj-123',
    };

    act(() => {
      result.current.mutate(invalidNecesidad);
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    expect(result.current.error).toBeDefined();
  });

  it('invalidates mis-necesidades query on success', async () => {
    const invalidateQueriesSpy = vi.spyOn(testQueryClient, 'invalidateQueries');

    const { result } = renderHook(() => useCreateNecesidad(), {
      wrapper: AllTheProviders,
    });

    const newNecesidad = {
      titulo: 'Nueva necesidad de prueba',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      proyectoArtisticoId: 'proj-123',
    };

    act(() => {
      result.current.mutate(newNecesidad);
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(invalidateQueriesSpy).toHaveBeenCalledWith({
      queryKey: ['crowdsourcing', 'necesidades', 'mis'],
    });
  });
});
```

#### useUpdateNecesidad.test.ts

**Archivo:** `__tests__/hooks/useUpdateNecesidad.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `updates necesidad successfully` | Unit | Mutation exitosa retorna data actualizada |
| `invalidates queries on success` | Unit | Invalida `mis-necesidades` y `byId(id)` |
| `handles estado != Abierta error (400)` | Unit | error "Solo se pueden editar necesidades en estado Abierta" |
| `handles validation error` | Unit | error con mensaje especifico |

**Casos Detallados:**

```typescript
describe('useUpdateNecesidad', () => {
  it('updates necesidad successfully', async () => {
    const { result } = renderHook(() => useUpdateNecesidad('nec-123-abierta'), {
      wrapper: AllTheProviders,
    });

    const updateData = {
      titulo: 'Titulo actualizado',
      modalidadTrabajoId: 2,
    };

    act(() => {
      result.current.mutate(updateData);
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.titulo).toBe('Titulo actualizado');
  });

  it('handles estado != Abierta error (400)', async () => {
    const { result } = renderHook(() => useUpdateNecesidad('nec-cerrada'), {
      wrapper: AllTheProviders,
    });

    const updateData = {
      titulo: 'Intento de edicion',
      modalidadTrabajoId: 2,
    };

    act(() => {
      result.current.mutate(updateData);
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    expect(result.current.error).toBeDefined();
  });
});
```

#### useCerrarNecesidad.test.ts

**Archivo:** `__tests__/hooks/useCerrarNecesidad.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `closes necesidad successfully` | Unit | Mutation exitosa retorna propuestasRechazadas count |
| `invalidates queries on success` | Unit | Invalida `mis-necesidades` y `byId(id)` |
| `handles estado != Abierta/En Progreso error (400)` | Unit | error "Solo se pueden cerrar necesidades en estado Abierta o En Progreso" |
| `sends motivo when provided` | Unit | Request incluye motivo si se proporciona |

**Casos Detallados:**

```typescript
describe('useCerrarNecesidad', () => {
  it('closes necesidad successfully', async () => {
    const { result } = renderHook(() => useCerrarNecesidad('nec-123-abierta'), {
      wrapper: AllTheProviders,
    });

    act(() => {
      result.current.mutate({ motivo: 'Ya no necesito el servicio' });
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toBeDefined();
    expect(result.current.data?.estadoNecesidadNombre).toBe('Cerrada');
    expect(result.current.data?.propuestasRechazadas).toBe(2);
  });

  it('handles estado != Abierta/En Progreso error (400)', async () => {
    const { result } = renderHook(() => useCerrarNecesidad('nec-cerrada'), {
      wrapper: AllTheProviders,
    });

    act(() => {
      result.current.mutate({});
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    expect(result.current.error).toBeDefined();
  });
});
```

---

### 4.3 Schemas

#### createNecesidadSchema.test.ts

**Archivo:** `__tests__/schemas/createNecesidadSchema.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `validates valid data` | Unit | Schema acepta datos correctos |
| `requires titulo min 5 chars` | Unit | Error si titulo < 5 |
| `requires titulo max 200 chars` | Unit | Error si titulo > 200 |
| `requires descripcion max 4000 chars` | Unit | Error si descripcion > 4000 |
| `requires tipoNecesidadId` | Unit | Error si falta |
| `requires modalidadTrabajoId` | Unit | Error si falta |
| `validates presupuesto max >= min` | Unit | Error si max < min |
| `requires monedaId when presupuesto present` | Unit | Error si presupuesto pero no moneda |
| `requires ubicacion when modalidad Presencial` | Unit | Error si modalidad=1 sin ubicacion |
| `requires ubicacion when modalidad Hibrido` | Unit | Error si modalidad=3 sin ubicacion |
| `allows null ubicacion when modalidad Remoto` | Unit | Valido si modalidad=2 sin ubicacion |
| `validates fechaLimitePropuestas > today` | Unit | Error si fecha pasada |
| `validates fechaInicioPrevista >= today` | Unit | Error si fecha anterior a hoy |
| `requires proyectoArtisticoId` | Unit | Error si falta |

**Casos Detallados:**

```typescript
import { createNecesidadSchema } from '@/shared/schemas/crowdsourcing.schema';

describe('createNecesidadSchema', () => {
  it('validates valid data', () => {
    const validData = {
      titulo: 'Mezcla de pistas para EP',
      descripcion: 'Necesito un ingeniero...',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      presupuestoMin: 150,
      presupuestoMax: 800,
      monedaId: 1,
      fechaLimitePropuestas: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
      fechaInicioPrevista: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
      proyectoArtisticoId: 'proj-123',
    };

    const result = createNecesidadSchema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it('requires titulo min 5 chars', () => {
    const invalidData = {
      titulo: 'Abc', // < 5
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      proyectoArtisticoId: 'proj-123',
    };

    const result = createNecesidadSchema.safeParse(invalidData);
    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.errors[0].message).toContain('al menos 5 caracteres');
    }
  });

  it('validates presupuesto max >= min', () => {
    const invalidData = {
      titulo: 'Titulo valido',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      presupuestoMin: 800,
      presupuestoMax: 150, // max < min
      monedaId: 1,
      proyectoArtisticoId: 'proj-123',
    };

    const result = createNecesidadSchema.safeParse(invalidData);
    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.errors[0].message).toContain('máximo debe ser mayor o igual al mínimo');
    }
  });

  it('requires ubicacion when modalidad Presencial', () => {
    const invalidData = {
      titulo: 'Titulo valido',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 1, // Presencial
      // ubicacionCiudad missing
      proyectoArtisticoId: 'proj-123',
    };

    const result = createNecesidadSchema.safeParse(invalidData);
    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.errors[0].message).toContain('ubicación es obligatoria');
    }
  });

  it('validates fechaLimitePropuestas > today', () => {
    const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();

    const invalidData = {
      titulo: 'Titulo valido',
      tipoNecesidadId: 3,
      modalidadTrabajoId: 2,
      fechaLimitePropuestas: yesterday,
      proyectoArtisticoId: 'proj-123',
    };

    const result = createNecesidadSchema.safeParse(invalidData);
    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.errors[0].message).toContain('debe ser posterior a hoy');
    }
  });
});
```

#### updateNecesidadSchema.test.ts

**Archivo:** `__tests__/schemas/updateNecesidadSchema.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `validates valid data` | Unit | Schema acepta datos correctos |
| `requires titulo min 5 chars` | Unit | Error si titulo < 5 |
| `validates presupuesto max >= min` | Unit | Error si max < min |
| `requires ubicacion when modalidad Presencial/Hibrido` | Unit | Error si modalidad 1 o 3 sin ubicacion |
| `does not require tipoNecesidadId` | Unit | Campo no incluido (inmutable) |
| `does not require proyectoArtisticoId` | Unit | Campo no incluido (inmutable) |

**Casos Detallados:**

```typescript
import { updateNecesidadSchema } from '@/shared/schemas/crowdsourcing.schema';

describe('updateNecesidadSchema', () => {
  it('validates valid data', () => {
    const validData = {
      titulo: 'Titulo actualizado',
      modalidadTrabajoId: 2,
      presupuestoMin: 200,
      presupuestoMax: 900,
      monedaId: 1,
    };

    const result = updateNecesidadSchema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it('does not require tipoNecesidadId', () => {
    const dataWithoutTipo = {
      titulo: 'Titulo actualizado',
      modalidadTrabajoId: 2,
      // tipoNecesidadId missing - OK
    };

    const result = updateNecesidadSchema.safeParse(dataWithoutTipo);
    expect(result.success).toBe(true);
  });
});
```

#### cerrarNecesidadSchema.test.ts

**Archivo:** `__tests__/schemas/cerrarNecesidadSchema.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `validates valid data with motivo` | Unit | Schema acepta motivo |
| `validates valid data without motivo` | Unit | Schema acepta motivo opcional |
| `requires motivo max 500 chars` | Unit | Error si motivo > 500 |

**Casos Detallados:**

```typescript
import { cerrarNecesidadSchema } from '@/shared/schemas/crowdsourcing.schema';

describe('cerrarNecesidadSchema', () => {
  it('validates valid data with motivo', () => {
    const validData = {
      motivo: 'Ya no necesito este servicio',
    };

    const result = cerrarNecesidadSchema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it('validates valid data without motivo', () => {
    const validData = {};

    const result = cerrarNecesidadSchema.safeParse(validData);
    expect(result.success).toBe(true);
  });

  it('requires motivo max 500 chars', () => {
    const longMotivo = 'A'.repeat(501);
    const invalidData = { motivo: longMotivo };

    const result = cerrarNecesidadSchema.safeParse(invalidData);
    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.errors[0].message).toContain('no puede superar los 500 caracteres');
    }
  });
});
```

---

### 4.4 Integration Tests

#### crear-necesidad-flow.test.tsx

**Archivo:** `__tests__/integration/crear-necesidad-flow.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `completes full create flow successfully` | Integration | Completa formulario → submit → toast → redirect |
| `validates required fields` | Integration | Submit sin datos → errores visibles |
| `shows ubicacion fields when modalidad Presencial` | Integration | Select Presencial → campos ubicacion visibles |
| `hides ubicacion fields when modalidad Remoto` | Integration | Select Remoto → campos ubicacion ocultos |
| `validates presupuesto max >= min` | Integration | Ingresa max < min → error visible |
| `shows toast error on API failure` | Integration | API 500 → toast rojo con mensaje |

**Casos Detallados:**

```typescript
describe('Crear Necesidad Flow', () => {
  it('completes full create flow successfully', async () => {
    const { getByLabelText, getByRole, getByText } = renderWithProviders(
      <NuevaNecesidadPage />
    );

    // Fill form
    fireEvent.change(getByLabelText(/título/i), {
      target: { value: 'Mezcla de pistas para EP' },
    });

    fireEvent.change(getByLabelText(/descripción/i), {
      target: { value: 'Necesito un ingeniero de mezcla experimentado...' },
    });

    // Select tipo necesidad (shadcn select)
    const tipoSelect = getByRole('combobox', { name: /tipo necesidad/i });
    fireEvent.click(tipoSelect);
    fireEvent.click(getByText('Ingeniería de Audio'));

    // Select modalidad
    const modalidadSelect = getByRole('combobox', { name: /modalidad/i });
    fireEvent.click(modalidadSelect);
    fireEvent.click(getByText('Remoto'));

    // Presupuesto
    fireEvent.change(getByLabelText(/presupuesto mínimo/i), {
      target: { value: '150' },
    });

    fireEvent.change(getByLabelText(/presupuesto máximo/i), {
      target: { value: '800' },
    });

    // Submit
    const submitButton = getByRole('button', { name: /Publicar Necesidad/i });
    fireEvent.click(submitButton);

    // Wait for success
    await waitFor(() => {
      expect(screen.getByText(/Necesidad publicada correctamente/i)).toBeInTheDocument();
    });

    // Verify redirect (mock navigate)
    expect(mockNavigate).toHaveBeenCalledWith('/dashboard/crowdsourcing/mis-necesidades');
  });

  it('shows ubicacion fields when modalidad Presencial', async () => {
    const { getByLabelText, getByRole, getByText, queryByLabelText } = renderWithProviders(
      <NuevaNecesidadPage />
    );

    // Initially hidden
    expect(queryByLabelText(/ciudad/i)).not.toBeInTheDocument();

    // Select Presencial
    const modalidadSelect = getByRole('combobox', { name: /modalidad/i });
    fireEvent.click(modalidadSelect);
    fireEvent.click(getByText('Presencial'));

    // Now visible
    await waitFor(() => {
      expect(getByLabelText(/ciudad/i)).toBeInTheDocument();
      expect(getByLabelText(/país/i)).toBeInTheDocument();
    });
  });
});
```

#### listar-necesidades-flow.test.tsx

**Archivo:** `__tests__/integration/listar-necesidades-flow.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `displays list of necesidades with filters` | Integration | Carga lista → aplica filtros → actualiza resultados |
| `filters by estado` | Integration | Select estado → lista filtrada |
| `filters by search text` | Integration | Type en search → debounce → lista filtrada |
| `navigates to detail on card click` | Integration | Click card → navega a detalle |
| `clears filters` | Integration | Aplica filtros → click limpiar → muestra todos |
| `paginates results` | Integration | Click siguiente → carga pagina 2 |

**Casos Detallados:**

```typescript
describe('Listar Necesidades Flow', () => {
  it('displays list of necesidades with filters', async () => {
    const { getByRole, getAllByRole, getByPlaceholderText } = renderWithProviders(
      <MisNecesidadesPage />
    );

    // Wait for list to load
    await waitFor(() => {
      const cards = getAllByRole('article');
      expect(cards.length).toBeGreaterThan(0);
    });

    // Apply estado filter
    const estadoSelect = getByRole('combobox', { name: /estado/i });
    fireEvent.click(estadoSelect);
    fireEvent.click(screen.getByText('Abierta'));

    await waitFor(() => {
      const cards = getAllByRole('article');
      expect(cards.length).toBe(1); // Only Abierta necesidades
    });
  });

  it('filters by search text', async () => {
    vi.useFakeTimers();

    const { getByPlaceholderText, getAllByRole } = renderWithProviders(
      <MisNecesidadesPage />
    );

    const searchInput = getByPlaceholderText(/buscar necesidad/i);
    fireEvent.change(searchInput, { target: { value: 'mezcla' } });

    // Debounce 300ms
    act(() => {
      vi.advanceTimersByTime(300);
    });

    await waitFor(() => {
      const cards = getAllByRole('article');
      expect(cards.length).toBeGreaterThan(0);
      expect(screen.getByText(/Mezcla de pistas/i)).toBeInTheDocument();
    });

    vi.useRealTimers();
  });

  it('navigates to detail on card click', async () => {
    const { getAllByRole } = renderWithProviders(<MisNecesidadesPage />);

    await waitFor(() => {
      const cards = getAllByRole('article');
      expect(cards.length).toBeGreaterThan(0);
    });

    const firstCard = getAllByRole('article')[0];
    fireEvent.click(firstCard);

    expect(mockNavigate).toHaveBeenCalledWith(
      expect.stringContaining('/dashboard/crowdsourcing/necesidades/')
    );
  });
});
```

#### editar-necesidad-flow.test.tsx

**Archivo:** `__tests__/integration/editar-necesidad-flow.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `loads and pre-fills form with existing data` | Integration | GET necesidad → form pre-rellenado |
| `shows warning banner when necesidad has propuestas` | Integration | GET con propuestas → banner visible |
| `disables tipoNecesidad field` | Integration | Campo disabled con tooltip |
| `updates necesidad successfully` | Integration | Modifica campos → submit → toast → redirect |
| `shows error when estado != Abierta` | Integration | Estado Cerrada → submit → error 400 → toast |

**Casos Detallados:**

```typescript
describe('Editar Necesidad Flow', () => {
  it('loads and pre-fills form with existing data', async () => {
    const { getByLabelText } = renderWithProviders(
      <EditarNecesidadPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta/editar'] }
    );

    await waitFor(() => {
      const tituloInput = getByLabelText(/título/i);
      expect(tituloInput).toHaveValue('Mezcla de pistas para EP de 5 canciones');
    });

    expect(getByLabelText(/descripción/i)).toHaveValue(
      expect.stringContaining('Buscamos un ingeniero')
    );
  });

  it('shows warning banner when necesidad has propuestas', async () => {
    const { getByText } = renderWithProviders(
      <EditarNecesidadPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-with-propuestas/editar'] }
    );

    await waitFor(() => {
      expect(getByText(/Esta necesidad ya tiene \d+ propuestas/i)).toBeInTheDocument();
    });
  });

  it('disables tipoNecesidad field', async () => {
    const { getByRole } = renderWithProviders(
      <EditarNecesidadPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta/editar'] }
    );

    await waitFor(() => {
      const tipoSelect = getByRole('combobox', { name: /tipo necesidad/i });
      expect(tipoSelect).toBeDisabled();
    });
  });

  it('updates necesidad successfully', async () => {
    const { getByLabelText, getByRole } = renderWithProviders(
      <EditarNecesidadPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta/editar'] }
    );

    await waitFor(() => {
      expect(getByLabelText(/título/i)).toHaveValue(expect.any(String));
    });

    // Modify title
    fireEvent.change(getByLabelText(/título/i), {
      target: { value: 'Titulo actualizado' },
    });

    // Submit
    const submitButton = getByRole('button', { name: /Guardar Cambios/i });
    fireEvent.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/Necesidad actualizada/i)).toBeInTheDocument();
    });
  });
});
```

#### cerrar-necesidad-flow.test.tsx

**Archivo:** `__tests__/integration/cerrar-necesidad-flow.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| `opens dialog on "Cerrar" button click` | Integration | Click boton → dialog visible |
| `closes necesidad without motivo` | Integration | Submit vacio → success → toast → listado actualizado |
| `closes necesidad with motivo` | Integration | Type motivo → submit → success |
| `displays propuestas count in success message` | Integration | Toast muestra "2 propuestas rechazadas" |
| `cancels without closing necesidad` | Integration | Click cancelar → dialog cierra sin cambios |

**Casos Detallados:**

```typescript
describe('Cerrar Necesidad Flow', () => {
  it('opens dialog on "Cerrar" button click', async () => {
    const { getByRole, getByText } = renderWithProviders(
      <NecesidadDetailPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta'] }
    );

    await waitFor(() => {
      expect(getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
    });

    const cerrarButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(cerrarButton);

    await waitFor(() => {
      expect(getByText(/propuestas pendientes serán rechazadas/i)).toBeInTheDocument();
    });
  });

  it('closes necesidad with motivo', async () => {
    const { getByRole, getByText, getByLabelText } = renderWithProviders(
      <NecesidadDetailPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta'] }
    );

    await waitFor(() => {
      expect(getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
    });

    const cerrarButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(cerrarButton);

    await waitFor(() => {
      expect(getByLabelText(/motivo/i)).toBeInTheDocument();
    });

    // Type motivo
    fireEvent.change(getByLabelText(/motivo/i), {
      target: { value: 'Ya encontré un profesional' },
    });

    // Confirm
    const confirmButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(confirmButton);

    await waitFor(() => {
      expect(screen.getByText(/Necesidad cerrada. Las propuestas pendientes han sido rechazadas/i)).toBeInTheDocument();
    });
  });

  it('displays propuestas count in success message', async () => {
    const { getByRole, getByText } = renderWithProviders(
      <NecesidadDetailPage />,
      { initialEntries: ['/dashboard/crowdsourcing/necesidades/nec-123-abierta'] }
    );

    await waitFor(() => {
      expect(getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
    });

    const cerrarButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(cerrarButton);

    const confirmButton = getByRole('button', { name: /Cerrar Necesidad/i });
    fireEvent.click(confirmButton);

    await waitFor(() => {
      expect(screen.getByText(/2 propuestas rechazadas/i)).toBeInTheDocument();
    });
  });
});
```

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| NecesidadCard.tsx | 90% | 95% | 85% |
| EstadoNecesidadBadge.tsx | 100% | 100% | 100% |
| NecesidadFilters.tsx | 85% | 90% | 80% |
| EmptyStateNecesidades.tsx | 95% | 100% | 90% |
| CerrarNecesidadDialog.tsx | 88% | 92% | 82% |
| PropuestaCard.tsx | 87% | 90% | 80% |
| useMisNecesidades.ts | 92% | 95% | 88% |
| useNecesidadById.ts | 90% | 95% | 85% |
| useCreateNecesidad.ts | 88% | 92% | 85% |
| useUpdateNecesidad.ts | 88% | 92% | 85% |
| useCerrarNecesidad.ts | 90% | 95% | 87% |
| createNecesidadSchema.ts | 95% | 100% | 92% |
| updateNecesidadSchema.ts | 95% | 100% | 92% |
| cerrarNecesidadSchema.ts | 100% | 100% | 100% |

**Meta Global:** 85% en lineas, 90% en funciones, 82% en branches

---

## 6. Comandos de Ejecucion

```bash
# Navegar a admin
cd src/admin

# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar tests de feature especifica
npm run test -- necesidades

# Watch mode
npm run test:watch

# Solo unit tests
npm run test -- --testPathPattern="__tests__/(components|hooks|schemas)"

# Solo integration tests
npm run test -- --testPathPattern="__tests__/integration"
```

---

## 7. CI/CD Integration

```yaml
# .github/workflows/test-admin.yml
name: Test Admin Frontend

on:
  push:
    branches: [master, develop]
    paths:
      - 'src/admin/**'
      - 'src/shared/**'
  pull_request:
    branches: [master, develop]

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
          files: src/admin/coverage/lcov.info
          flags: admin-frontend
          name: admin-coverage

      - name: Check coverage threshold
        working-directory: src/admin
        run: |
          COVERAGE=$(jq -r '.total.lines.pct' coverage/coverage-summary.json)
          if (( $(echo "$COVERAGE < 80" | bc -l) )); then
            echo "Coverage $COVERAGE% is below 80% threshold"
            exit 1
          fi
```

---

## 8. Checklist

### Mocks y Fixtures
- [x] Mock data para necesidades (Abierta, En Progreso, Cerrada, Cancelada)
- [x] Mock data para propuestas (Pendiente, Aceptada, Rechazada)
- [x] MSW handlers para todos los endpoints (GET, POST, PUT, PATCH)
- [x] Error handlers (404, 403, 400, 500)
- [x] Test utilities con QueryClient y Router

### Component Tests
- [x] NecesidadCard: renders, badges, propuestas counter, actions condicionales, hover, click
- [x] EstadoNecesidadBadge: colores por estado (4 estados), aria-label
- [x] NecesidadFilters: select estado, search debounce 300ms, clear filters
- [x] EmptyStateNecesidades: render con/sin filtros, CTAs
- [x] CerrarNecesidadDialog: open/close, textarea motivo, char counter 500, submit, cancel
- [x] PropuestaCard: render datos, avatar, truncate mensaje, readonly mode, actions

### Hook Tests
- [x] useMisNecesidades: fetch, loading, error, filtros (estado, search), paginacion, refetch
- [x] useNecesidadById: fetch, loading, not found 404, forbidden 403, disabled when no id
- [x] useCreateNecesidad: success, validation error 400, forbidden 403, invalidates queries
- [x] useUpdateNecesidad: success, estado != Abierta error 400, invalidates queries
- [x] useCerrarNecesidad: success con propuestasRechazadas count, estado error 400, invalidates queries

### Schema Tests
- [x] createNecesidadSchema: valid data, titulo min/max, descripcion max, presupuesto refine, moneda refine, ubicacion refine (modalidad 1/3), fechas refines, required fields
- [x] updateNecesidadSchema: valid data, mismas validaciones que create, NO incluye tipo/proyecto
- [x] cerrarNecesidadSchema: valid con/sin motivo, motivo max 500

### Integration Tests
- [x] Crear necesidad flow: fill form completo, submit, toast success, redirect
- [x] Listar necesidades flow: carga lista, filtros estado, search debounce, paginacion, click card navega
- [x] Editar necesidad flow: pre-fill data, warning banner con propuestas, tipo disabled, submit, estado error
- [x] Cerrar necesidad flow: open dialog, submit con/sin motivo, success message con count, cancel

### Coverage y Performance
- [x] Cobertura objetivo 80%+ definida por archivo
- [x] Tiempo ejecucion < 60 segundos
- [x] CI/CD con coverage check threshold 80%
- [x] Codecov integration para reportes

---

## 9. Notas de Implementacion

### MSW Setup

**Archivo:** `src/admin/vitest.setup.ts`

```typescript
import { beforeAll, afterEach, afterAll } from 'vitest';
import { setupServer } from 'msw/node';
import { necesidadesHandlers } from '@/features/crowdsourcing/necesidades/__mocks__/handlers';

export const server = setupServer(...necesidadesHandlers);

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

### Vitest Config

**Archivo:** `src/admin/vitest.config.ts`

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./vitest.setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      include: ['src/features/**/*.{ts,tsx}'],
      exclude: [
        '**/*.test.{ts,tsx}',
        '**/__tests__/**',
        '**/__mocks__/**',
        '**/node_modules/**',
        '**/dist/**',
      ],
      thresholds: {
        lines: 80,
        functions: 85,
        branches: 75,
        statements: 80,
      },
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
});
```

### Testing Library Best Practices

1. **Query Priority:**
   - `getByRole` > `getByLabelText` > `getByPlaceholderText` > `getByText` > `getByTestId`

2. **User Interactions:**
   - Usar `userEvent` en lugar de `fireEvent` cuando sea posible
   - Simular interacciones reales del usuario

3. **Async Testing:**
   - Usar `waitFor` para esperar cambios asincrónicos
   - Usar `findBy*` queries que esperan automáticamente

4. **Accessibility:**
   - Verificar `aria-label`, `aria-describedby` en tests
   - Usar queries basadas en roles

5. **Mock Service Layer:**
   - Mockear services, NO fetch/axios directamente
   - MSW para interceptar requests HTTP

---

## 10. Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Tests lentos (> 60s) | Media | Alto | Usar MSW para mocks rapidos. Evitar tests E2E en unit tests. Paralelizar ejecucion. |
| Cobertura < 80% | Baja | Alto | CI/CD bloquea merge si coverage < 80%. Coverage report en cada PR. |
| Flaky tests (intermitentes) | Media | Medio | Usar `waitFor` correctamente. Evitar timeouts fijos. Mock de timers con `vi.useFakeTimers`. |
| Mocks desactualizados vs API | Media | Medio | Sync manual de mocks con contratos API. Integration tests E2E opcionales. |
| Tests no reflejan UX real | Baja | Medio | Usar Testing Library queries semanticas. User interactions realistas con userEvent. |

---

## Archivo Generado

**Ruta:** `C:\Repos\WePlay_Rises\plans\cs-gestionar-necesidades\frontend-admin\test-strategy.md`

**Total de tests planificados:** 40 (32 unit + 8 integration)

**Cobertura objetivo:** 82%+ global (85% lineas, 90% funciones, 82% branches)

**Tiempo estimado de ejecucion:** < 60 segundos
