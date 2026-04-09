# Estrategia de Testing: Explorar Propuestas (Landing)

**Fecha:** 2026-02-17
**Feature:** cs-explorar-propuestas
**Target:** src/web (Landing - Vite + React)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen

| Tipo | Cantidad | Cobertura Estimada |
|------|----------|-------------------|
| Component Tests | 11 componentes | 80% |
| Hook Tests | 5 hooks | 90% |
| Service Tests | 2 servicios | 95% |
| Schema Tests | 1 schema Zod | 95% |
| Integration Tests | 3 flujos | 70% |
| **Total** | **22 test suites** | **80%+** |

### Filosofia de Testing

```
        /\
       /  \      E2E (manual, post-MVP)
      /----\
     /      \    Integration (flujos criticos de propuesta)
    /--------\
   /          \  Unit (componentes + hooks + services)
  /____________\ Schema + Utility validators
```

**Prioridad:**
1. Integration tests para flujo explorar -> detalle -> enviar propuesta
2. Integration test para flujo mis propuestas -> retirar
3. Unit tests para componentes con logica condicional compleja (CTA states, badges)
4. Unit tests para hooks TanStack Query
5. Schema tests para validaciones Zod del formulario de propuesta

---

## 2. Estructura de Tests

```
src/web/src/features/crowdsourcing/
├── __tests__/
│   ├── components/
│   │   ├── NecesidadCard.test.tsx
│   │   ├── PropuestaCard.test.tsx
│   │   ├── EstadoPropuestaBadge.test.tsx
│   │   ├── UrgenciaBadge.test.tsx
│   │   ├── EnviarPropuestaForm.test.tsx
│   │   ├── RetirarPropuestaDialog.test.tsx
│   │   ├── NecesidadFilters.test.tsx
│   │   ├── EmptyStateNecesidades.test.tsx
│   │   └── EmptyStatePropuestas.test.tsx
│   ├── pages/
│   │   ├── ExplorarNecesidadesPage.test.tsx
│   │   ├── NecesidadDetallePage.test.tsx
│   │   └── MisPropuestasPage.test.tsx
│   ├── hooks/
│   │   ├── useNecesidadesPublicas.test.ts
│   │   ├── useNecesidadPublica.test.ts
│   │   ├── useCreatePropuesta.test.ts
│   │   ├── useMisPropuestas.test.ts
│   │   └── useRetirarPropuesta.test.ts
│   ├── services/
│   │   ├── necesidadPublica.service.test.ts
│   │   └── propuesta.service.test.ts
│   ├── schemas/
│   │   └── propuesta.schema.test.ts
│   └── integration/
│       ├── explorar-enviar-propuesta.test.tsx
│       └── mis-propuestas-retirar.test.tsx
├── __mocks__/
│   ├── necesidad.mock.ts
│   ├── propuesta.mock.ts
│   └── handlers.ts
└── test-utils.tsx
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/necesidad.mock.ts`

```typescript
import type { NecesidadPublicaListDto, NecesidadPublicaDetalleDto } from '@shared/types';

export const mockNecesidadUrgente: NecesidadPublicaListDto = {
    id: '550e8400-e29b-41d4-a716-446655440000',
    titulo: 'Mezcla de pistas para EP de 5 canciones',
    descripcion: 'Buscamos un ingeniero de mezcla experimentado para un EP de rock alternativo...',
    tipoNecesidadNombre: 'Post-produccion',
    presupuestoMin: 150.00,
    presupuestoMax: 800.00,
    monedaNombre: 'EUR',
    modalidadTrabajoNombre: 'Remoto',
    modalidadTrabajoIcono: 'wifi',
    ubicacionCiudad: null,
    ubicacionPais: null,
    artistaNombre: 'Los Rockeros',
    fechaRelativa: 'hace 2 dias',
    fechaLimitePropuestas: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(), // +2 dias
    esUrgente: true,
    numeroPropuestas: 3,
};

export const mockNecesidadNormal: NecesidadPublicaListDto = {
    id: '660e8400-e29b-41d4-a716-446655440001',
    titulo: 'Diseno de portada para EP',
    descripcion: 'Necesitamos un disenador grafico para la portada de nuestro primer EP...',
    tipoNecesidadNombre: 'Diseno',
    presupuestoMin: 200.00,
    presupuestoMax: 500.00,
    monedaNombre: 'EUR',
    modalidadTrabajoNombre: 'Presencial',
    modalidadTrabajoIcono: 'map-pin',
    ubicacionCiudad: 'Madrid',
    ubicacionPais: 'Espana',
    artistaNombre: 'Indie Band',
    fechaRelativa: 'hace 5 dias',
    fechaLimitePropuestas: new Date(Date.now() + 20 * 24 * 60 * 60 * 1000).toISOString(), // +20 dias
    esUrgente: false,
    numeroPropuestas: 1,
};

export const mockNecesidadSinPropuestas: NecesidadPublicaListDto = {
    ...mockNecesidadNormal,
    id: '770e8400-e29b-41d4-a716-446655440002',
    titulo: 'Produccion musical para single',
    numeroPropuestas: 0,
};

export const mockNecesidadesList: NecesidadPublicaListDto[] = [
    mockNecesidadUrgente,
    mockNecesidadNormal,
    mockNecesidadSinPropuestas,
];

export const mockNecesidadDetalle: NecesidadPublicaDetalleDto = {
    id: mockNecesidadUrgente.id,
    titulo: 'Mezcla de pistas para EP de 5 canciones',
    descripcion: 'Buscamos un ingeniero de mezcla experimentado para un EP de 5 canciones de rock alternativo. Necesitamos experiencia en...',
    tipoNecesidadNombre: 'Post-produccion',
    presupuestoMin: 150.00,
    presupuestoMax: 800.00,
    monedaNombre: 'EUR',
    modalidadTrabajoNombre: 'Remoto',
    ubicacionCiudad: null,
    ubicacionPais: null,
    artista: {
        id: 'artista-001',
        nombreArtistico: 'Los Rockeros',
        imagenUrl: 'https://example.com/artista.jpg',
    },
    fechaCreacion: '2026-02-15T10:30:00Z',
    fechaLimitePropuestas: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
    fechaInicioPrevista: '2026-04-01',
    numeroPropuestas: 3,
    yaPropuso: false,
    esPropietario: false,
    tienePerfilProfesional: true,
};

export const mockNecesidadDetalleYaPropuso: NecesidadPublicaDetalleDto = {
    ...mockNecesidadDetalle,
    yaPropuso: true,
};

export const mockNecesidadDetallePropietario: NecesidadPublicaDetalleDto = {
    ...mockNecesidadDetalle,
    esPropietario: true,
};

export const mockNecesidadDetalleSinPerfil: NecesidadPublicaDetalleDto = {
    ...mockNecesidadDetalle,
    tienePerfilProfesional: false,
};

export const mockNecesidadesPaginadas = {
    items: mockNecesidadesList,
    totalCount: 3,
    page: 1,
    pageSize: 12,
    totalPages: 1,
};
```

**Archivo:** `__mocks__/propuesta.mock.ts`

```typescript
import type { PropuestaListDto, CreatePropuestaRequest, PropuestaCreatedDto } from '@shared/types';

export const mockPropuestaPendiente: PropuestaListDto = {
    id: '880e8400-e29b-41d4-a716-446655440003',
    necesidadId: '550e8400-e29b-41d4-a716-446655440000',
    necesidadTitulo: 'Mezcla de pistas para EP de 5 canciones',
    artistaNombre: 'Los Rockeros',
    precioPropuesto: 450.00,
    monedaNombre: 'EUR',
    estadoPropuestaId: 1,
    estadoPropuestaNombre: 'Pendiente',
    fechaCreacion: '2026-02-20T14:00:00Z',
    fechaActualizacion: undefined,
    acuerdoId: undefined,
};

export const mockPropuestaAceptada: PropuestaListDto = {
    id: '990e8400-e29b-41d4-a716-446655440004',
    necesidadId: '660e8400-e29b-41d4-a716-446655440001',
    necesidadTitulo: 'Diseno de portada para EP',
    artistaNombre: 'Indie Band',
    precioPropuesto: 300.00,
    monedaNombre: 'EUR',
    estadoPropuestaId: 2,
    estadoPropuestaNombre: 'Aceptada',
    fechaCreacion: '2026-02-15T10:00:00Z',
    fechaActualizacion: '2026-02-18T09:30:00Z',
    acuerdoId: 'acuerdo-001',
};

export const mockPropuestaRechazada: PropuestaListDto = {
    ...mockPropuestaPendiente,
    id: 'aaa0e8400-e29b-41d4-a716-446655440005',
    estadoPropuestaId: 3,
    estadoPropuestaNombre: 'Rechazada',
    fechaActualizacion: '2026-02-19T11:00:00Z',
    acuerdoId: undefined,
};

export const mockPropuestaRetirada: PropuestaListDto = {
    ...mockPropuestaPendiente,
    id: 'bbb0e8400-e29b-41d4-a716-446655440006',
    estadoPropuestaId: 4,
    estadoPropuestaNombre: 'Retirada',
    fechaActualizacion: '2026-02-21T16:00:00Z',
    acuerdoId: undefined,
};

export const mockPropuestasList: PropuestaListDto[] = [
    mockPropuestaPendiente,
    mockPropuestaAceptada,
    mockPropuestaRechazada,
    mockPropuestaRetirada,
];

export const mockCreatePropuestaRequest: CreatePropuestaRequest = {
    precioPropuesto: 450.00,
    monedaId: 1,
    diasEstimados: 14,
    mensajePropuesta: 'Soy ingeniero de mezcla con 10 anos de experiencia en rock alternativo. He trabajado con bandas como...',
};

export const mockPropuestaCreatedResponse: PropuestaCreatedDto = {
    id: mockPropuestaPendiente.id,
    necesidadTitulo: mockPropuestaPendiente.necesidadTitulo,
    precioPropuesto: 450.00,
    estadoPropuestaNombre: 'Pendiente',
    fechaCreacion: '2026-02-20T14:00:00Z',
};

export const mockPropuestasPaginadas = {
    items: mockPropuestasList,
    totalCount: 4,
    page: 1,
    pageSize: 10,
    totalPages: 1,
};
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import {
    mockNecesidadesPaginadas,
    mockNecesidadDetalle,
    mockNecesidadDetalleYaPropuso,
} from './necesidad.mock';
import {
    mockPropuestasPaginadas,
    mockPropuestaCreatedResponse,
    mockPropuestaRetirada,
} from './propuesta.mock';

const API_BASE = 'http://localhost:5001/api';

export const necesidadHandlers = [
    // GET /api/crowdsourcing/necesidades - Listado paginado con filtros
    http.get(`${API_BASE}/crowdsourcing/necesidades`, ({ request }) => {
        const url = new URL(request.url);
        const search = url.searchParams.get('search') || '';

        // Simular empty state para busqueda sin resultados
        if (search === 'no-resultados') {
            return HttpResponse.json({
                data: { items: [], totalCount: 0, page: 1, pageSize: 12, totalPages: 0 },
                messages: [],
            });
        }

        return HttpResponse.json({
            data: mockNecesidadesPaginadas,
            messages: [],
        });
    }),

    // GET /api/crowdsourcing/necesidades/:id - Detalle con campos calculados
    http.get(`${API_BASE}/crowdsourcing/necesidades/:id`, ({ params }) => {
        const { id } = params;

        if (id === 'not-found') {
            return HttpResponse.json(
                { data: null, messages: [{ message: 'Necesidad no encontrada', errorCode: '2000' }] },
                { status: 404 }
            );
        }

        return HttpResponse.json({
            data: mockNecesidadDetalle,
            messages: [],
        });
    }),
];

export const propuestaHandlers = [
    // POST /api/crowdsourcing/necesidades/:necesidadId/propuestas - Crear propuesta
    http.post(`${API_BASE}/crowdsourcing/necesidades/:necesidadId/propuestas`, async ({ request }) => {
        const body = await request.json() as Record<string, unknown>;

        if (body.precioPropuesto <= 0) {
            return HttpResponse.json(
                { data: null, messages: [{ message: 'El precio propuesto debe ser mayor a 0', errorCode: '1001' }] },
                { status: 400 }
            );
        }

        if (typeof body.mensajePropuesta === 'string' && body.mensajePropuesta.length < 20) {
            return HttpResponse.json(
                { data: null, messages: [{ message: 'El mensaje debe tener al menos 20 caracteres', errorCode: '1003' }] },
                { status: 400 }
            );
        }

        return HttpResponse.json(
            { data: mockPropuestaCreatedResponse, messages: [{ message: 'Propuesta enviada correctamente. El artista sera notificado.', errorCode: '0001' }] },
            { status: 201 }
        );
    }),

    // GET /api/crowdsourcing/propuestas/mis-propuestas - Mis propuestas paginadas
    http.get(`${API_BASE}/crowdsourcing/propuestas/mis-propuestas`, ({ request }) => {
        const url = new URL(request.url);
        const estado = url.searchParams.get('estado');

        if (estado) {
            const estadoId = parseInt(estado);
            const filtered = mockPropuestasPaginadas.items.filter(p => p.estadoPropuestaId === estadoId);
            return HttpResponse.json({
                data: { items: filtered, totalCount: filtered.length, page: 1, pageSize: 10, totalPages: 1 },
                messages: [],
            });
        }

        return HttpResponse.json({
            data: mockPropuestasPaginadas,
            messages: [],
        });
    }),

    // PATCH /api/crowdsourcing/propuestas/:id/retirar - Retirar propuesta
    http.patch(`${API_BASE}/crowdsourcing/propuestas/:id/retirar`, ({ params }) => {
        const { id } = params;

        if (id === 'non-pending') {
            return HttpResponse.json(
                { data: null, messages: [{ message: 'Solo se pueden retirar propuestas en estado Pendiente', errorCode: '4000' }] },
                { status: 400 }
            );
        }

        return HttpResponse.json({
            data: mockPropuestaRetirada,
            messages: [{ message: 'Propuesta retirada', errorCode: '0002' }],
        });
    }),
];

// Handlers de error para override en tests especificos
export const errorHandlers = {
    necesidadesError: http.get(`${API_BASE}/crowdsourcing/necesidades`, () => {
        return HttpResponse.json(
            { data: null, messages: [{ message: 'Error interno', errorCode: '5000' }] },
            { status: 500 }
        );
    }),
    propuestaYaExiste: http.post(`${API_BASE}/crowdsourcing/necesidades/:necesidadId/propuestas`, () => {
        return HttpResponse.json(
            { data: null, messages: [{ message: 'Ya tienes una propuesta para esta necesidad', errorCode: '1008' }] },
            { status: 400 }
        );
    }),
    propuestaForbidden: http.post(`${API_BASE}/crowdsourcing/necesidades/:necesidadId/propuestas`, () => {
        return HttpResponse.json(
            { data: null, messages: [{ message: 'No puedes enviar propuesta a tu propia necesidad', errorCode: '3002' }] },
            { status: 403 }
        );
    }),
};

export const allHandlers = [...necesidadHandlers, ...propuestaHandlers];
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import React from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, MemoryRouter } from 'react-router-dom';
import { vi } from 'vitest';

// Mock global del store de autenticacion
vi.mock('@/store/auth-store', () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
        user: { id: 'user-profesional-001', email: 'pro@test.com', name: 'Profesional Test' },
        token: 'mock-jwt-token',
    })),
}));

export const createTestQueryClient = () =>
    new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    });

interface AllProvidersProps {
    children: React.ReactNode;
    queryClient?: QueryClient;
    initialEntries?: string[];
}

function AllProviders({ children, queryClient, initialEntries = ['/'] }: AllProvidersProps) {
    const client = queryClient || createTestQueryClient();
    return (
        <QueryClientProvider client={client}>
            <MemoryRouter initialEntries={initialEntries}>
                {children}
            </MemoryRouter>
        </QueryClientProvider>
    );
}

export function renderWithProviders(
    ui: React.ReactElement,
    options?: RenderOptions & { queryClient?: QueryClient; initialEntries?: string[] }
) {
    const { queryClient, initialEntries, ...renderOptions } = options || {};
    return render(ui, {
        wrapper: (props) => (
            <AllProviders {...props} queryClient={queryClient} initialEntries={initialEntries} />
        ),
        ...renderOptions,
    });
}

export function createWrapper(queryClient?: QueryClient, initialEntries?: string[]) {
    const client = queryClient || createTestQueryClient();
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return (
            <QueryClientProvider client={client}>
                <MemoryRouter initialEntries={initialEntries || ['/']}>
                    {children}
                </MemoryRouter>
            </QueryClientProvider>
        );
    };
}

export { userEvent } from '@testing-library/user-event';
```

---

## 4. Tests por Modulo

### 4.1 Components

#### NecesidadCard.test.tsx

**Archivo:** `__tests__/components/NecesidadCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders titulo, tipo y artista | Unit | Datos basicos visibles |
| trunca descripcion a 150 caracteres | Unit | Descripcion larga truncada con "..." |
| muestra badge urgente si esUrgente=true | Unit | Badge "URGENTE" visible con pulse |
| oculta badge urgente si esUrgente=false | Unit | Badge no visible |
| muestra rango de presupuesto | Unit | "150 - 800 EUR" visible |
| muestra ubicacion si modalidad es Presencial | Unit | Ciudad y pais visibles |
| oculta ubicacion si modalidad es Remoto | Unit | Sin ciudad/pais |
| muestra numero de propuestas | Unit | "3 propuesta(s)" con icono Users |
| muestra "Sin propuestas aun" si numeroPropuestas=0 | Unit | Texto especifico |
| muestra badge de modalidad con icono correcto | Unit | Wifi para Remoto, MapPin para Presencial |
| fecha limite con color rojo si < 3 dias | Unit | Clase CSS de color rojo |
| fecha limite con color amber si 3-7 dias | Unit | Clase CSS amber |
| llama onClick al hacer click en la card | Unit | Handler ejecutado |
| es accesible con role=article y aria-label | Unit | ARIA correcto |
| responde a Enter y Space (teclado) | Unit | Navegacion por teclado |

**Casos Detallados:**

```typescript
describe('NecesidadCard', () => {
    it('trunca descripcion a 150 caracteres', () => {
        const necesidad = { ...mockNecesidadUrgente, descripcion: 'a'.repeat(200) };
        renderWithProviders(<NecesidadCard necesidad={necesidad} onClick={vi.fn()} />);
        const descripcion = screen.getByText(/a{100,150}\.{3}/); // truncada con "..."
        expect(descripcion).toBeInTheDocument();
    });

    it('muestra badge urgente si esUrgente=true', () => {
        renderWithProviders(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={vi.fn()} />);
        expect(screen.getByText('URGENTE')).toBeInTheDocument();
    });

    it('oculta badge urgente si esUrgente=false', () => {
        renderWithProviders(<NecesidadCard necesidad={mockNecesidadNormal} onClick={vi.fn()} />);
        expect(screen.queryByText('URGENTE')).not.toBeInTheDocument();
    });

    it('muestra "Sin propuestas aun" si numeroPropuestas=0', () => {
        renderWithProviders(<NecesidadCard necesidad={mockNecesidadSinPropuestas} onClick={vi.fn()} />);
        expect(screen.getByText(/Sin propuestas aun/i)).toBeInTheDocument();
    });

    it('llama onClick al hacer click en la card', async () => {
        const user = userEvent.setup();
        const handleClick = vi.fn();
        renderWithProviders(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={handleClick} />);
        await user.click(screen.getByRole('article'));
        expect(handleClick).toHaveBeenCalledOnce();
    });

    it('responde a Enter para navegacion por teclado', async () => {
        const user = userEvent.setup();
        const handleClick = vi.fn();
        renderWithProviders(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={handleClick} />);
        const card = screen.getByRole('article');
        card.focus();
        await user.keyboard('{Enter}');
        expect(handleClick).toHaveBeenCalledOnce();
    });
});
```

---

#### PropuestaCard.test.tsx

**Archivo:** `__tests__/components/PropuestaCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders titulo necesidad y artista | Unit | Datos basicos visibles |
| muestra precio y moneda | Unit | "450 EUR" visible |
| muestra fecha de envio formateada | Unit | Fecha legible |
| muestra fecha de respuesta si existe | Unit | fechaActualizacion visible |
| oculta fecha de respuesta si no existe | Unit | No visible si es undefined |
| muestra boton "Retirar" si estado=Pendiente | Unit | Boton visible |
| muestra boton "Ver acuerdo" si estado=Aceptada | Unit | Boton con link visible |
| no muestra acciones si estado=Rechazada | Unit | Sin botones de accion |
| no muestra acciones si estado=Retirada | Unit | Sin botones de accion |
| llama onRetirar con propuestaId al hacer click | Unit | Handler ejecutado con ID correcto |
| titulo necesidad es clickeable (link a detalle) | Unit | Link con href correcto |

**Casos Detallados:**

```typescript
describe('PropuestaCard', () => {
    it('muestra boton "Retirar" solo si estado=Pendiente', () => {
        renderWithProviders(<PropuestaCard propuesta={mockPropuestaPendiente} onRetirar={vi.fn()} />);
        expect(screen.getByRole('button', { name: /Retirar propuesta/i })).toBeInTheDocument();
    });

    it('muestra boton "Ver acuerdo" si estado=Aceptada', () => {
        renderWithProviders(<PropuestaCard propuesta={mockPropuestaAceptada} onRetirar={vi.fn()} />);
        expect(screen.getByRole('link', { name: /Ver acuerdo/i })).toBeInTheDocument();
    });

    it('no muestra acciones si estado=Rechazada', () => {
        renderWithProviders(<PropuestaCard propuesta={mockPropuestaRechazada} onRetirar={vi.fn()} />);
        expect(screen.queryByRole('button', { name: /Retirar/i })).not.toBeInTheDocument();
        expect(screen.queryByRole('link', { name: /Ver acuerdo/i })).not.toBeInTheDocument();
    });

    it('llama onRetirar con ID correcto al hacer click en Retirar', async () => {
        const user = userEvent.setup();
        const handleRetirar = vi.fn();
        renderWithProviders(<PropuestaCard propuesta={mockPropuestaPendiente} onRetirar={handleRetirar} />);
        await user.click(screen.getByRole('button', { name: /Retirar propuesta/i }));
        expect(handleRetirar).toHaveBeenCalledWith(mockPropuestaPendiente.id);
    });
});
```

---

#### EstadoPropuestaBadge.test.tsx

**Archivo:** `__tests__/components/EstadoPropuestaBadge.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza "Pendiente" con clase amber | Unit | Color y texto correctos |
| renderiza "Aceptada" con clase green | Unit | Color y texto correctos |
| renderiza "Rechazada" con clase red | Unit | Color y texto correctos |
| renderiza "Retirada" con clase gray | Unit | Color y texto correctos |
| tiene aria-label descriptivo | Unit | Accesibilidad correcta |

**Casos Detallados:**

```typescript
describe('EstadoPropuestaBadge', () => {
    it.each([
        ['Pendiente', 'amber'],
        ['Aceptada', 'green'],
        ['Rechazada', 'red'],
        ['Retirada', 'gray'],
    ])('renderiza "%s" con color %s', (estado, colorClass) => {
        renderWithProviders(<EstadoPropuestaBadge estado={estado as EstadoPropuesta} />);
        const badge = screen.getByText(estado);
        expect(badge).toBeInTheDocument();
        expect(badge.closest('[class*="badge"]') || badge).toHaveClass(expect.stringContaining(colorClass));
    });

    it('tiene aria-label descriptivo', () => {
        renderWithProviders(<EstadoPropuestaBadge estado="Pendiente" />);
        expect(screen.getByLabelText(/Estado: Pendiente/i)).toBeInTheDocument();
    });
});
```

---

#### UrgenciaBadge.test.tsx

**Archivo:** `__tests__/components/UrgenciaBadge.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza badge con icono Clock | Unit | Icono y texto "URGENTE" |
| tiene aria-label descriptivo | Unit | Accesibilidad correcta |
| aplica clase de animacion pulse | Unit | Clase pulse CSS presente |

---

#### EnviarPropuestaForm.test.tsx

**Archivo:** `__tests__/components/EnviarPropuestaForm.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza todos los campos del formulario | Unit | Precio, moneda, dias, mensaje visibles |
| muestra titulo y presupuesto de referencia | Unit | "Para: {titulo}" y "150 - 800 EUR" |
| muestra nota "dentro del rango" si precio valido | Unit | Nota verde con precio entre min y max |
| muestra nota "por encima" si precio > max | Unit | Nota amber con precio > 800 |
| muestra nota "por debajo" si precio < min | Unit | Nota azul con precio < 150 |
| oculta nota si precio es 0 o vacio | Unit | Sin nota informativa |
| actualiza contador de caracteres en tiempo real | Unit | "X / 2000 caracteres" |
| muestra error si precio <= 0 tras blur | Unit | Mensaje de error visible |
| muestra error si mensaje < 20 caracteres | Unit | Mensaje de error visible |
| muestra error si dias estimados > 365 | Unit | Mensaje de error visible |
| deshabilita submit si hay errores de validacion | Unit | Boton disabled |
| muestra spinner en submit mientras carga | Unit | Loader2 + "Enviando..." |
| llama onSubmit con datos correctos | Integration | Datos del formulario correctos |
| cierra dialog al hacer click en Cancelar | Unit | onClose ejecutado |
| cierra dialog al presionar Escape | Unit | onClose ejecutado |
| resetea formulario al cerrar | Unit | Valores limpios al reabrir |

**Casos Detallados:**

```typescript
describe('EnviarPropuestaForm', () => {
    const mockNecesidad = mockNecesidadDetalle;
    const defaultProps = {
        necesidad: mockNecesidad,
        open: true,
        onClose: vi.fn(),
        onSuccess: vi.fn(),
    };

    it('muestra nota "dentro del rango" si precio entre 150 y 800', async () => {
        const user = userEvent.setup();
        renderWithProviders(<EnviarPropuestaForm {...defaultProps} />);
        const precioInput = screen.getByLabelText(/Precio propuesto/i);
        await user.clear(precioInput);
        await user.type(precioInput, '450');
        await waitFor(() => {
            expect(screen.getByText(/Tu precio esta dentro del rango/i)).toBeInTheDocument();
        });
    });

    it('muestra nota "por encima" si precio > 800', async () => {
        const user = userEvent.setup();
        renderWithProviders(<EnviarPropuestaForm {...defaultProps} />);
        const precioInput = screen.getByLabelText(/Precio propuesto/i);
        await user.clear(precioInput);
        await user.type(precioInput, '1200');
        await waitFor(() => {
            expect(screen.getByText(/por encima del presupuesto/i)).toBeInTheDocument();
        });
    });

    it('actualiza contador de caracteres en tiempo real', async () => {
        const user = userEvent.setup();
        renderWithProviders(<EnviarPropuestaForm {...defaultProps} />);
        const mensajeInput = screen.getByLabelText(/Mensaje de propuesta/i);
        await user.type(mensajeInput, 'Hola mundo');
        await waitFor(() => {
            expect(screen.getByText(/10 \/ 2000 caracteres/i)).toBeInTheDocument();
        });
    });

    it('llama onSubmit con datos correctos tras rellenar formulario valido', async () => {
        const user = userEvent.setup();
        const handleSuccess = vi.fn();
        renderWithProviders(<EnviarPropuestaForm {...defaultProps} onSuccess={handleSuccess} />);

        await user.type(screen.getByLabelText(/Precio propuesto/i), '450');
        // Select moneda - EUR
        await user.type(screen.getByLabelText(/Dias estimados/i), '14');
        await user.type(
            screen.getByLabelText(/Mensaje de propuesta/i),
            'Soy ingeniero con 10 anos de experiencia en rock. Tengo equipos profesionales.'
        );
        await user.click(screen.getByRole('button', { name: /Enviar propuesta/i }));

        await waitFor(() => {
            expect(handleSuccess).toHaveBeenCalledOnce();
        });
    });

    it('deshabilita submit si mensaje tiene menos de 20 caracteres', async () => {
        const user = userEvent.setup();
        renderWithProviders(<EnviarPropuestaForm {...defaultProps} />);
        const mensajeInput = screen.getByLabelText(/Mensaje de propuesta/i);
        await user.type(mensajeInput, 'Muy corto');
        await user.tab(); // Trigger blur validation
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Enviar propuesta/i })).toBeDisabled();
        });
    });
});
```

---

#### RetirarPropuestaDialog.test.tsx

**Archivo:** `__tests__/components/RetirarPropuestaDialog.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza titulo y warning de irreversibilidad | Unit | Titulo + alerta roja visibles |
| muestra el titulo de la necesidad entre comillas | Unit | Nombre de necesidad en dialog |
| cierra sin accion al hacer click en Cancelar | Unit | onClose ejecutado, onConfirm no |
| cierra sin accion al presionar Escape | Unit | onClose ejecutado |
| llama onConfirm al confirmar retirada | Unit | onConfirm ejecutado |
| deshabilita botones durante submit (isPending=true) | Unit | Ambos botones disabled |
| muestra spinner en boton confirmar durante submit | Unit | Loader2 + "Retirando..." |

**Casos Detallados:**

```typescript
describe('RetirarPropuestaDialog', () => {
    const defaultProps = {
        open: true,
        necesidadTitulo: 'Mezcla de pistas para EP de 5 canciones',
        onClose: vi.fn(),
        onConfirm: vi.fn(),
        isPending: false,
    };

    it('muestra el titulo de la necesidad', () => {
        renderWithProviders(<RetirarPropuestaDialog {...defaultProps} />);
        expect(screen.getByText(/"Mezcla de pistas para EP de 5 canciones"/i)).toBeInTheDocument();
    });

    it('cierra sin ejecutar onConfirm al cancelar', async () => {
        const user = userEvent.setup();
        const handleClose = vi.fn();
        const handleConfirm = vi.fn();
        renderWithProviders(
            <RetirarPropuestaDialog {...defaultProps} onClose={handleClose} onConfirm={handleConfirm} />
        );
        await user.click(screen.getByRole('button', { name: /Cancelar/i }));
        expect(handleClose).toHaveBeenCalledOnce();
        expect(handleConfirm).not.toHaveBeenCalled();
    });

    it('deshabilita ambos botones durante submit', () => {
        renderWithProviders(<RetirarPropuestaDialog {...defaultProps} isPending={true} />);
        expect(screen.getByRole('button', { name: /Retirando/i })).toBeDisabled();
        expect(screen.getByRole('button', { name: /Cancelar/i })).toBeDisabled();
    });
});
```

---

#### NecesidadFilters.test.tsx

**Archivo:** `__tests__/components/NecesidadFilters.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza chips de tipo de necesidad | Unit | Chips de tipo visibles |
| toggle de chip cambia aria-pressed | Unit | Estado seleccionado correcto |
| click en "Limpiar filtros" resetea todos los filtros | Unit | onClear ejecutado |
| input de busqueda llama onChange con debounce | Unit | Callback tras debounce 300ms |
| select de modalidad llama onChange inmediatamente | Unit | Callback inmediato |
| campo ciudad se muestra solo si hay pais seleccionado | Unit | Input ciudad oculto/visible |
| input de presupuesto respeta debounce de 500ms | Unit | Callback tras 500ms |

**Casos Detallados:**

```typescript
describe('NecesidadFilters', () => {
    it('toggle de chip actualiza aria-pressed', async () => {
        const user = userEvent.setup();
        renderWithProviders(<NecesidadFilters onChange={vi.fn()} onClear={vi.fn()} filtros={{}} />);
        const chipProduccion = screen.getByRole('button', { name: /Filtrar por tipo: Produccion/i });
        expect(chipProduccion).toHaveAttribute('aria-pressed', 'false');
        await user.click(chipProduccion);
        expect(chipProduccion).toHaveAttribute('aria-pressed', 'true');
    });

    it('campo ciudad aparece solo con pais seleccionado', async () => {
        const user = userEvent.setup();
        renderWithProviders(<NecesidadFilters onChange={vi.fn()} onClear={vi.fn()} filtros={{}} />);
        expect(screen.queryByLabelText(/Ciudad/i)).not.toBeInTheDocument();
        // Seleccionar un pais en el select
        // Verificar que aparece el input de ciudad
    });
});
```

---

#### EmptyStateNecesidades.test.tsx

**Archivo:** `__tests__/components/EmptyStateNecesidades.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza mensaje "sin filtros" cuando no hay filtros activos | Unit | Icono Inbox + mensaje correcto |
| renderiza mensaje "con filtros" y boton Limpiar cuando hay filtros | Unit | Icono SearchX + boton visible |
| llama onClearFilters al hacer click en Limpiar | Unit | Handler ejecutado |
| no muestra boton Limpiar si hayFiltrosActivos=false | Unit | Boton ausente |

---

#### EmptyStatePropuestas.test.tsx

**Archivo:** `__tests__/components/EmptyStatePropuestas.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renderiza mensaje cuando no hay propuestas en total | Unit | Icono FileText + CTA a explorar |
| renderiza mensaje con estado filtrado | Unit | "No tienes propuestas Pendientes" |
| muestra link "Explorar necesidades" si sin ninguna propuesta | Unit | Link presente |

---

### 4.2 Pages

#### ExplorarNecesidadesPage.test.tsx

**Archivo:** `__tests__/pages/ExplorarNecesidadesPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| muestra 6 skeletons durante loading inicial | Unit | 6 NecesidadCardSkeleton visibles |
| renderiza listado de necesidades al cargar | Integration | Cards con datos visibles |
| muestra contador de resultados | Unit | "3 necesidades encontradas" |
| muestra empty state si no hay resultados con filtros | Integration | EmptyStateNecesidades visible |
| muestra empty state si listado vacio sin filtros | Integration | Mensaje "Sin necesidades abiertas" |
| muestra error state si API falla | Integration | Icono error + boton Reintentar |
| navega al detalle al hacer click en card | Integration | Navegacion a /crowdsourcing/necesidades/:id |
| paginacion muestra paginas disponibles | Unit | Componente Pagination visible |

**Casos Detallados:**

```typescript
describe('ExplorarNecesidadesPage', () => {
    it('muestra 6 skeletons durante loading', () => {
        vi.mocked(necesidadPublicaService.getAll).mockReturnValue(new Promise(() => {}));
        renderWithProviders(<ExplorarNecesidadesPage />);
        // Se renderizan 6 esqueletos mientras se carga
        const skeletons = screen.getAllByRole('status', { name: /Cargando necesidades/i });
        expect(skeletons).toHaveLength(6);
    });

    it('renderiza listado tras carga exitosa', async () => {
        vi.mocked(necesidadPublicaService.getAll).mockResolvedValue(mockNecesidadesPaginadas);
        renderWithProviders(<ExplorarNecesidadesPage />);
        await waitFor(() => {
            expect(screen.getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
        });
        expect(screen.getByText('Diseno de portada para EP')).toBeInTheDocument();
    });

    it('muestra error state con boton Reintentar si API falla', async () => {
        vi.mocked(necesidadPublicaService.getAll).mockRejectedValue(new Error('Network error'));
        renderWithProviders(<ExplorarNecesidadesPage />);
        await waitFor(() => {
            expect(screen.getByText(/Error al cargar necesidades/i)).toBeInTheDocument();
        });
        expect(screen.getByRole('button', { name: /Reintentar/i })).toBeInTheDocument();
    });
});
```

---

#### NecesidadDetallePage.test.tsx

**Archivo:** `__tests__/pages/NecesidadDetallePage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| muestra skeletons durante loading | Unit | 3 skeletons visibles |
| renderiza detalle completo al cargar | Integration | Titulo, artista, descripcion, detalles |
| muestra boton "Enviar propuesta" si puede proponer | Unit | CTA habilitado |
| muestra estado "Ya enviaste" si yaPropuso=true | Unit | Bloque verde, sin boton |
| muestra estado "Esta es tu necesidad" si esPropietario=true | Unit | Bloque info, sin boton |
| muestra CTA "Crear perfil" si tienePerfilProfesional=false | Unit | Link a crear perfil |
| muestra badge URGENTE si fecha limite < 3 dias | Unit | Badge visible |
| abre modal de propuesta al hacer click en boton | Integration | Dialog visible tras click |
| muestra error 404 con link volver al listado | Unit | Mensaje no encontrado + link |
| back link navega al listado | Unit | Link "/crowdsourcing/necesidades" |

**Casos Detallados:**

```typescript
describe('NecesidadDetallePage', () => {
    it('muestra boton "Enviar propuesta" habilitado si puede proponer', async () => {
        vi.mocked(necesidadPublicaService.getById).mockResolvedValue(mockNecesidadDetalle);
        renderWithProviders(
            <NecesidadDetallePage />,
            { initialEntries: [`/crowdsourcing/necesidades/${mockNecesidadDetalle.id}`] }
        );
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Enviar propuesta/i })).toBeInTheDocument();
            expect(screen.getByRole('button', { name: /Enviar propuesta/i })).not.toBeDisabled();
        });
    });

    it('muestra bloque verde "Ya enviaste una propuesta" si yaPropuso=true', async () => {
        vi.mocked(necesidadPublicaService.getById).mockResolvedValue(mockNecesidadDetalleYaPropuso);
        renderWithProviders(<NecesidadDetallePage />);
        await waitFor(() => {
            expect(screen.getByText(/Ya enviaste una propuesta/i)).toBeInTheDocument();
        });
        expect(screen.queryByRole('button', { name: /Enviar propuesta/i })).not.toBeInTheDocument();
    });

    it('muestra CTA para crear perfil si tienePerfilProfesional=false', async () => {
        vi.mocked(necesidadPublicaService.getById).mockResolvedValue(mockNecesidadDetalleSinPerfil);
        renderWithProviders(<NecesidadDetallePage />);
        await waitFor(() => {
            expect(screen.getByRole('link', { name: /Crear perfil profesional/i })).toBeInTheDocument();
        });
        expect(screen.queryByRole('button', { name: /Enviar propuesta/i })).not.toBeInTheDocument();
    });

    it('muestra "Esta es tu necesidad" si esPropietario=true', async () => {
        vi.mocked(necesidadPublicaService.getById).mockResolvedValue(mockNecesidadDetallePropietario);
        renderWithProviders(<NecesidadDetallePage />);
        await waitFor(() => {
            expect(screen.getByText(/Esta es tu necesidad/i)).toBeInTheDocument();
        });
    });

    it('abre dialog de propuesta al hacer click en boton', async () => {
        const user = userEvent.setup();
        vi.mocked(necesidadPublicaService.getById).mockResolvedValue(mockNecesidadDetalle);
        renderWithProviders(<NecesidadDetallePage />);
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Enviar propuesta/i })).toBeInTheDocument();
        });
        await user.click(screen.getByRole('button', { name: /Enviar propuesta/i }));
        expect(screen.getByRole('dialog')).toBeInTheDocument();
    });
});
```

---

#### MisPropuestasPage.test.tsx

**Archivo:** `__tests__/pages/MisPropuestasPage.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| muestra 4 skeletons durante loading | Unit | 4 PropuestaCardSkeleton visibles |
| renderiza lista de propuestas al cargar | Integration | 4 cards con datos |
| muestra contador de resultados | Unit | "4 propuestas" |
| muestra chips de filtro de estado | Unit | Todos/Pendiente/Aceptada/Rechazada/Retirada |
| filtrar por estado actualiza lista | Integration | Solo propuestas del estado seleccionado |
| empty state si no hay propuestas totales | Unit | Mensaje + CTA explorar |
| empty state si filtro no tiene resultados | Unit | Mensaje con nombre del estado |
| abre dialog de retirar al hacer click en boton Retirar | Integration | Dialog visible |
| muestra error state si API falla | Integration | Mensaje error |

**Casos Detallados:**

```typescript
describe('MisPropuestasPage', () => {
    it('renderiza lista de propuestas al cargar', async () => {
        vi.mocked(propuestaService.getMisPropuestas).mockResolvedValue(mockPropuestasPaginadas);
        renderWithProviders(<MisPropuestasPage />);
        await waitFor(() => {
            expect(screen.getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
        });
        // Verifica badges de estado
        expect(screen.getByText('Pendiente')).toBeInTheDocument();
        expect(screen.getByText('Aceptada')).toBeInTheDocument();
        expect(screen.getByText('Rechazada')).toBeInTheDocument();
        expect(screen.getByText('Retirada')).toBeInTheDocument();
    });

    it('filtra por estado al hacer click en chip', async () => {
        const user = userEvent.setup();
        vi.mocked(propuestaService.getMisPropuestas).mockResolvedValue(mockPropuestasPaginadas);
        renderWithProviders(<MisPropuestasPage />);
        await waitFor(() => { expect(screen.getByText('Pendiente')).toBeInTheDocument(); });

        vi.mocked(propuestaService.getMisPropuestas).mockResolvedValue({
            items: [mockPropuestaPendiente],
            totalCount: 1,
            page: 1,
            pageSize: 10,
            totalPages: 1,
        });

        await user.click(screen.getByRole('button', { name: /Filtrar por estado: Pendiente/i }));

        await waitFor(() => {
            expect(screen.queryByText('Aceptada')).not.toBeInTheDocument();
        });
    });
});
```

---

### 4.3 Hooks

#### useNecesidadesPublicas.test.ts

**Archivo:** `__tests__/hooks/useNecesidadesPublicas.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna datos paginados con exito | Unit | Data correcta en isSuccess |
| isLoading=true durante la peticion | Unit | Estado de carga correcto |
| isError=true si API falla | Unit | Error cuando servicio falla |
| llama al servicio con filtros correctos | Unit | Params pasados al service |
| cambia queryKey al cambiar filtros | Unit | Nueva peticion con nuevos params |
| usa keepPreviousData durante refetch con filtros | Unit | Data anterior durante carga |

**Casos Detallados:**

```typescript
describe('useNecesidadesPublicas', () => {
    it('retorna datos paginados con exito', async () => {
        vi.mocked(necesidadPublicaService.getAll).mockResolvedValue(mockNecesidadesPaginadas);
        const { result } = renderHook(
            () => useNecesidadesPublicas({}),
            { wrapper: createWrapper() }
        );
        await waitFor(() => expect(result.current.isSuccess).toBe(true));
        expect(result.current.data?.items).toHaveLength(3);
        expect(result.current.data?.totalCount).toBe(3);
    });

    it('pasa filtros correctos al servicio', async () => {
        vi.mocked(necesidadPublicaService.getAll).mockResolvedValue(mockNecesidadesPaginadas);
        const filtros = { search: 'mezcla', modalidad: 1, page: 2 };
        const { result } = renderHook(
            () => useNecesidadesPublicas(filtros),
            { wrapper: createWrapper() }
        );
        await waitFor(() => expect(result.current.isSuccess).toBe(true));
        expect(necesidadPublicaService.getAll).toHaveBeenCalledWith(filtros);
    });

    it('isError=true y data=undefined si servicio falla', async () => {
        vi.mocked(necesidadPublicaService.getAll).mockRejectedValue(new Error('Network error'));
        const { result } = renderHook(
            () => useNecesidadesPublicas({}),
            { wrapper: createWrapper() }
        );
        await waitFor(() => expect(result.current.isError).toBe(true));
        expect(result.current.data).toBeUndefined();
    });
});
```

---

#### useNecesidadPublica.test.ts

**Archivo:** `__tests__/hooks/useNecesidadPublica.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna detalle completo con campos calculados | Unit | yaPropuso, esPropietario, tienePerfilProfesional |
| no ejecuta query si id es string vacio | Unit | enabled=false, sin llamada al service |
| retorna 404 como isError | Unit | Error manejado correctamente |
| cambia query al cambiar id | Unit | Nueva peticion con nuevo id |

---

#### useCreatePropuesta.test.ts

**Archivo:** `__tests__/hooks/useCreatePropuesta.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| crea propuesta con exito y retorna created response | Unit | Data correcta en onSuccess |
| invalida queries de necesidades tras exito | Integration | invalidateQueries llamado |
| invalida query de detalle de la necesidad especifica | Integration | queryKey con necesidadId |
| invalida queries de mis-propuestas tras exito | Integration | invalidateQueries llamado |
| muestra toast de exito tras crear | Unit | Toast "Propuesta enviada correctamente..." |
| muestra toast de error si API retorna 400 | Unit | Toast con mensaje de error |
| muestra toast de error si API retorna 403 | Unit | Toast "No puedes enviar propuesta..." |
| pasa datos correctos al servicio | Unit | Payload correcto al service |
| isPending=true durante la mutacion | Unit | Estado pendiente correcto |

**Casos Detallados:**

```typescript
describe('useCreatePropuesta', () => {
    const necesidadId = mockNecesidadDetalle.id;

    it('invalida queries correctas tras exito', async () => {
        vi.mocked(propuestaService.create).mockResolvedValue(mockPropuestaCreatedResponse);
        const queryClient = createTestQueryClient();
        const invalidateSpy = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(
            () => useCreatePropuesta(necesidadId),
            { wrapper: createWrapper(queryClient) }
        );

        result.current.mutate(mockCreatePropuestaRequest);

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(invalidateSpy).toHaveBeenCalledWith({ queryKey: ['necesidades'] });
        expect(invalidateSpy).toHaveBeenCalledWith({ queryKey: ['necesidad-detalle', necesidadId] });
        expect(invalidateSpy).toHaveBeenCalledWith({ queryKey: ['mis-propuestas'] });
    });

    it('muestra toast de exito con el mensaje correcto', async () => {
        const toastSuccessMock = vi.fn();
        vi.mocked(propuestaService.create).mockResolvedValue(mockPropuestaCreatedResponse);

        // Arrange toast mock
        vi.mock('sonner', () => ({ toast: { success: toastSuccessMock, error: vi.fn() } }));

        const { result } = renderHook(
            () => useCreatePropuesta(necesidadId),
            { wrapper: createWrapper() }
        );

        result.current.mutate(mockCreatePropuestaRequest);

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(toastSuccessMock).toHaveBeenCalledWith(
            expect.stringContaining('Propuesta enviada correctamente')
        );
    });
});
```

---

#### useMisPropuestas.test.ts

**Archivo:** `__tests__/hooks/useMisPropuestas.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retorna lista paginada de propuestas | Unit | Items correctos |
| filtra por estado cuando se especifica | Unit | Service llamado con parametro estado |
| sin filtro retorna todas las propuestas | Unit | Service llamado sin estado |
| isLoading=true durante la peticion | Unit | Estado loading correcto |
| isError=true si API falla | Unit | Error manejado |

---

#### useRetirarPropuesta.test.ts

**Archivo:** `__tests__/hooks/useRetirarPropuesta.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| retira propuesta con exito | Unit | Respuesta con estado=Retirada |
| invalida queries de mis-propuestas tras exito | Integration | invalidateQueries llamado |
| muestra toast "Propuesta retirada" tras exito | Unit | Toast de exito correcto |
| muestra toast de error si API retorna 400 | Unit | Toast con mensaje de error |
| pasa propuestaId correcto al servicio | Unit | ID correcto al service |

---

### 4.4 Services

#### necesidadPublica.service.test.ts

**Archivo:** `__tests__/services/necesidadPublica.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| getAll - retorna lista paginada con filtros | Unit | Respuesta paginada correcta |
| getAll - construye query params correctamente | Unit | URL con params esperados |
| getAll - retorna array vacio si no hay resultados | Unit | Items vacio |
| getById - retorna detalle con campos calculados | Unit | Objeto con yaPropuso, esPropietario |
| getById - lanza error 404 si no existe | Unit | Axios error propagado |
| getById - lanza error de red | Unit | Error generico propagado |

**Casos Detallados:**

```typescript
describe('necesidadPublica.service', () => {
    beforeAll(() => server.listen());
    afterEach(() => server.resetHandlers());
    afterAll(() => server.close());

    describe('getAll', () => {
        it('retorna lista paginada', async () => {
            const result = await necesidadPublicaService.getAll({});
            expect(result.items).toHaveLength(3);
            expect(result.totalCount).toBe(3);
            expect(result.page).toBe(1);
        });

        it('retorna array vacio para busqueda sin resultados', async () => {
            const result = await necesidadPublicaService.getAll({ search: 'no-resultados' });
            expect(result.items).toHaveLength(0);
            expect(result.totalCount).toBe(0);
        });
    });

    describe('getById', () => {
        it('retorna detalle con campos calculados', async () => {
            const result = await necesidadPublicaService.getById(mockNecesidadDetalle.id);
            expect(result.yaPropuso).toBe(false);
            expect(result.esPropietario).toBe(false);
            expect(result.tienePerfilProfesional).toBe(true);
        });

        it('lanza error si necesidad no encontrada', async () => {
            await expect(necesidadPublicaService.getById('not-found'))
                .rejects.toThrow();
        });
    });
});
```

---

#### propuesta.service.test.ts

**Archivo:** `__tests__/services/propuesta.service.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| create - crea propuesta y retorna id + datos | Unit | Respuesta 201 con datos correctos |
| create - lanza error 400 si validacion falla | Unit | Error propagado con mensaje |
| create - lanza error 403 si no tiene perfilProfesional | Unit | Error de autorizacion propagado |
| getMisPropuestas - retorna lista paginada | Unit | Datos correctos |
| getMisPropuestas - acepta filtro de estado | Unit | Filtro aplicado en request |
| retirar - retira propuesta y retorna estado actualizado | Unit | Estado=Retirada |
| retirar - lanza error 400 si propuesta no esta Pendiente | Unit | Error propagado |

---

### 4.5 Schemas

#### propuesta.schema.test.ts

**Archivo:** `__tests__/schemas/propuesta.schema.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| valida precioPropuesto requerido | Unit | Error si falta precio |
| valida precioPropuesto > 0 | Unit | Error si precio <= 0 |
| valida monedaId requerido | Unit | Error si falta moneda |
| diasEstimados opcional - permite undefined | Unit | Sin error si no se pasa |
| diasEstimados - error si <= 0 | Unit | Error si dias = 0 |
| diasEstimados - error si > 365 | Unit | Error si dias = 366 |
| mensajePropuesta requerido | Unit | Error si falta mensaje |
| mensajePropuesta - error si < 20 caracteres | Unit | Error si muy corto |
| mensajePropuesta - error si > 2000 caracteres | Unit | Error si muy largo |
| acepta datos validos sin errores | Unit | safeParse exitoso |

**Casos Detallados:**

```typescript
describe('createPropuestaSchema', () => {
    it('acepta datos validos', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 450,
            monedaId: 1,
            diasEstimados: 14,
            mensajePropuesta: 'Soy ingeniero con amplia experiencia en rock alternativo y produccion musical.',
        });
        expect(result.success).toBe(true);
    });

    it('error si precioPropuesto <= 0', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 0,
            monedaId: 1,
            mensajePropuesta: 'Mensaje de al menos veinte caracteres valido',
        });
        expect(result.success).toBe(false);
        if (!result.success) {
            expect(result.error.issues[0].message).toContain('mayor a 0');
        }
    });

    it('error si mensajePropuesta < 20 caracteres', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 100,
            monedaId: 1,
            mensajePropuesta: 'Muy corto',
        });
        expect(result.success).toBe(false);
        if (!result.success) {
            expect(result.error.issues[0].message).toContain('20 caracteres');
        }
    });

    it('error si mensajePropuesta > 2000 caracteres', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 100,
            monedaId: 1,
            mensajePropuesta: 'a'.repeat(2001),
        });
        expect(result.success).toBe(false);
    });

    it('diasEstimados es opcional - permite undefined', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 100,
            monedaId: 1,
            mensajePropuesta: 'Mensaje valido con mas de veinte caracteres',
        });
        expect(result.success).toBe(true);
        if (result.success) {
            expect(result.data.diasEstimados).toBeUndefined();
        }
    });

    it('error si diasEstimados > 365', () => {
        const result = createPropuestaSchema.safeParse({
            precioPropuesto: 100,
            monedaId: 1,
            diasEstimados: 366,
            mensajePropuesta: 'Mensaje valido con mas de veinte caracteres',
        });
        expect(result.success).toBe(false);
    });
});
```

---

### 4.6 Integration Tests

#### explorar-enviar-propuesta.test.tsx

**Archivo:** `__tests__/integration/explorar-enviar-propuesta.test.tsx`

**Descripcion:** Flujo completo desde explorar necesidades hasta enviar propuesta exitosamente.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| flujo completo: listado -> detalle -> enviar propuesta -> toast exito | Integration | Happy path completo |
| intento de enviar propuesta ya existente muestra toast de error | Integration | FA-02: error ya propuso |
| detalle visible pero sin boton si no tiene perfilProfesional | Integration | FA-01: CTA crear perfil |
| filtros actualizan el listado en tiempo real | Integration | Debounce + actualizacion UI |

**Casos Detallados:**

```typescript
describe('Flujo Explorar y Enviar Propuesta (Integration)', () => {
    beforeAll(() => server.listen());
    afterEach(() => server.resetHandlers());
    afterAll(() => server.close());

    it('flujo completo: explorar -> ver detalle -> enviar propuesta exitosa', async () => {
        const user = userEvent.setup();

        // 1. Renderizar pagina de listado
        renderWithProviders(
            <Routes>
                <Route path="/crowdsourcing/necesidades" element={<ExplorarNecesidadesPage />} />
                <Route path="/crowdsourcing/necesidades/:id" element={<NecesidadDetallePage />} />
                <Route path="/crowdsourcing/mis-propuestas" element={<MisPropuestasPage />} />
            </Routes>,
            { initialEntries: ['/crowdsourcing/necesidades'] }
        );

        // 2. Esperar que cargue el listado
        await waitFor(() => {
            expect(screen.getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
        });

        // 3. Click en card de necesidad
        await user.click(screen.getByText('Mezcla de pistas para EP de 5 canciones'));

        // 4. Esperar que cargue el detalle
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Enviar propuesta/i })).toBeInTheDocument();
        });

        // 5. Click en boton "Enviar propuesta" -> abre modal
        await user.click(screen.getByRole('button', { name: /Enviar propuesta/i }));
        expect(screen.getByRole('dialog')).toBeInTheDocument();

        // 6. Rellenar formulario
        await user.type(screen.getByLabelText(/Precio propuesto/i), '450');
        await user.type(screen.getByLabelText(/Dias estimados/i), '14');
        await user.type(
            screen.getByLabelText(/Mensaje de propuesta/i),
            'Soy ingeniero con 10 anos de experiencia. Conozco el genero y tengo equipos de alta calidad.'
        );

        // 7. Submit del formulario
        await user.click(screen.getByRole('button', { name: /Enviar propuesta/i }));

        // 8. Toast de exito + modal cerrado + redireccion a mis-propuestas
        await waitFor(() => {
            expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
        });
        expect(screen.getByText(/Propuesta enviada correctamente/i)).toBeInTheDocument();
    });

    it('muestra CTA crear perfil en lugar de boton si sin perfilProfesional', async () => {
        server.use(
            http.get('*/crowdsourcing/necesidades/:id', () => {
                return HttpResponse.json({ data: mockNecesidadDetalleSinPerfil, messages: [] });
            })
        );

        renderWithProviders(
            <NecesidadDetallePage />,
            { initialEntries: [`/crowdsourcing/necesidades/${mockNecesidadDetalle.id}`] }
        );

        await waitFor(() => {
            expect(screen.getByRole('link', { name: /Crear perfil profesional/i })).toBeInTheDocument();
        });
        expect(screen.queryByRole('button', { name: /Enviar propuesta/i })).not.toBeInTheDocument();
    });
});
```

---

#### mis-propuestas-retirar.test.tsx

**Archivo:** `__tests__/integration/mis-propuestas-retirar.test.tsx`

**Descripcion:** Flujo de ver mis propuestas y retirar una propuesta pendiente.

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| flujo completo: ver mis propuestas -> retirar -> confirmar -> badge actualizado | Integration | Happy path retirada |
| cancelar en dialog no cambia estado | Integration | Propuesta permanece Pendiente |
| error al retirar muestra toast de error | Integration | FA-07: error retirada fallida |

**Casos Detallados:**

```typescript
describe('Flujo Mis Propuestas y Retirar (Integration)', () => {
    beforeAll(() => server.listen());
    afterEach(() => server.resetHandlers());
    afterAll(() => server.close());

    it('flujo completo: mis propuestas -> click retirar -> confirmar -> badge Retirada', async () => {
        const user = userEvent.setup();

        renderWithProviders(
            <MisPropuestasPage />,
            { initialEntries: ['/crowdsourcing/mis-propuestas'] }
        );

        // 1. Esperar carga de propuestas
        await waitFor(() => {
            expect(screen.getByText('Mezcla de pistas para EP de 5 canciones')).toBeInTheDocument();
        });

        // 2. Click en "Retirar propuesta" de la propuesta Pendiente
        await user.click(screen.getByRole('button', { name: /Retirar propuesta/i }));

        // 3. Dialog de confirmacion aparece
        expect(screen.getByRole('dialog')).toBeInTheDocument();
        expect(screen.getByText(/Esta accion no se puede deshacer/i)).toBeInTheDocument();

        // 4. Confirmar retirada
        await user.click(screen.getByRole('button', { name: /Confirmar retirada/i }));

        // 5. Dialog cerrado + toast + badge actualizado
        await waitFor(() => {
            expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
        });
        expect(screen.getByText(/Propuesta retirada/i)).toBeInTheDocument();
    });

    it('cancelar en dialog no cambia la propuesta', async () => {
        const user = userEvent.setup();
        renderWithProviders(<MisPropuestasPage />);
        await waitFor(() => {
            expect(screen.getByRole('button', { name: /Retirar propuesta/i })).toBeInTheDocument();
        });

        await user.click(screen.getByRole('button', { name: /Retirar propuesta/i }));
        expect(screen.getByRole('dialog')).toBeInTheDocument();

        await user.click(screen.getByRole('button', { name: /Cancelar/i }));

        await waitFor(() => {
            expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
        });

        // La propuesta sigue siendo Pendiente
        expect(screen.getByRole('button', { name: /Retirar propuesta/i })).toBeInTheDocument();
    });
});
```

---

## 5. MSW Setup

### 5.1 Modificar vitest.setup.ts

```typescript
import '@testing-library/jest-dom/vitest';
import { setupServer } from 'msw/node';
import { allHandlers } from './src/features/crowdsourcing/__mocks__/handlers';

// Polyfill ResizeObserver for Radix UI components in jsdom
if (typeof globalThis.ResizeObserver === 'undefined') {
    globalThis.ResizeObserver = class ResizeObserver {
        observe() {}
        unobserve() {}
        disconnect() {}
    } as unknown as typeof ResizeObserver;
}

export const server = setupServer(...allHandlers);

beforeAll(() => server.listen({ onUnhandledRequest: 'warn' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

### 5.2 Override Handlers en Tests

```typescript
// Para testear error state especifico
it('maneja error de propuesta ya existente', async () => {
    server.use(errorHandlers.propuestaYaExiste);
    // Test logic...
});

// Para testear propietario de la necesidad
it('muestra "Esta es tu necesidad" si esPropietario', async () => {
    server.use(
        http.get('*/crowdsourcing/necesidades/:id', () => {
            return HttpResponse.json({ data: mockNecesidadDetallePropietario, messages: [] });
        })
    );
    // Test logic...
});
```

---

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Notas |
|---------|--------|-----------|----------|-------|
| NecesidadCard.tsx | 85% | 90% | 80% | Logica de urgencia y color de fecha |
| PropuestaCard.tsx | 90% | 95% | 85% | 4 estados de propuesta |
| EstadoPropuestaBadge.tsx | 95% | 100% | 90% | Solo logica de colores |
| UrgenciaBadge.tsx | 90% | 100% | 85% | Componente simple |
| EnviarPropuestaForm.tsx | 85% | 90% | 80% | Logica de rango de precio compleja |
| RetirarPropuestaDialog.tsx | 90% | 95% | 85% | Logica de confirmacion |
| NecesidadFilters.tsx | 80% | 85% | 75% | Filtros con debounce |
| EmptyStateNecesidades.tsx | 90% | 100% | 85% | Estados con/sin filtros |
| EmptyStatePropuestas.tsx | 90% | 100% | 85% | Estados con/sin filtro |
| ExplorarNecesidadesPage.tsx | 75% | 80% | 70% | Muchos branches (loading, error, empty) |
| NecesidadDetallePage.tsx | 80% | 85% | 75% | 5 CTA states distintos |
| MisPropuestasPage.tsx | 80% | 85% | 75% | Filtros + dialogs |
| useNecesidadesPublicas.ts | 90% | 100% | 85% | Hook con keepPreviousData |
| useNecesidadPublica.ts | 90% | 100% | 85% | Campos calculados del backend |
| useCreatePropuesta.ts | 90% | 100% | 85% | Invalidaciones multiples |
| useMisPropuestas.ts | 90% | 100% | 85% | Hook con filtro opcional |
| useRetirarPropuesta.ts | 90% | 100% | 85% | Mutation con invalidacion |
| necesidadPublica.service.ts | 95% | 100% | 90% | Servicios GET |
| propuesta.service.ts | 95% | 100% | 90% | POST, GET, PATCH |
| propuesta.schema.ts | 95% | 100% | 90% | Validaciones Zod |

**Meta Global:** 80%+ en todas las metricas

---

## 7. Mocking Strategy

### 7.1 Service Mocks (preferido para unit tests)

```typescript
// Mockear el service directamente - NO mockear axios
vi.mock('@/features/crowdsourcing/services/necesidadPublica.service', () => ({
    necesidadPublicaService: {
        getAll: vi.fn(),
        getById: vi.fn(),
    },
}));

vi.mock('@/features/crowdsourcing/services/propuesta.service', () => ({
    propuestaService: {
        create: vi.fn(),
        getMisPropuestas: vi.fn(),
        retirar: vi.fn(),
    },
}));
```

### 7.2 MSW Handlers (para integration tests)

Para los tests de integracion se usa MSW a nivel de red para validar el flujo completo incluyendo
el comportamiento real del service (no mockeado). Esto valida que la integracion service -> hook -> component
funciona correctamente end-to-end en el frontend.

### 7.3 Auth Store Mock

```typescript
// En test-utils.tsx - mock global para todos los tests
vi.mock('@/store/auth-store', () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
        user: { id: 'user-profesional-001', email: 'pro@test.com' },
        token: 'mock-jwt-token',
    })),
}));

// Override en tests que requieren usuario no autenticado
vi.mocked(useAuthStore).mockReturnValue({
    isAuthenticated: false,
    user: null,
    token: null,
});
```

### 7.4 Router Mocks

```typescript
// Mock useNavigate en tests unitarios
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
        useParams: () => ({ id: mockNecesidadDetalle.id }),
        useSearchParams: () => [new URLSearchParams(), vi.fn()],
    };
});
```

### 7.5 Sonner Toast Mock

```typescript
// Mock de sonner para verificar toasts
vi.mock('sonner', () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        warning: vi.fn(),
    },
}));
```

---

## 8. Criterios de Aceptacion Cubiertos

| AC-ID | Criterio | Test Que Lo Valida |
|-------|----------|-------------------|
| AC-CS03-3 | Badge URGENTE si < 3 dias | `NecesidadCard.test.tsx` - muestra badge urgente si esUrgente=true |
| AC-CS03-5 | yaPropuso=true desactiva boton | `NecesidadDetallePage.test.tsx` - muestra estado "Ya enviaste" |
| AC-CS03-6 | esPropietario=true desactiva boton | `NecesidadDetallePage.test.tsx` - muestra "Esta es tu necesidad" |
| AC-CS03-7 | Toast exito al enviar propuesta | `useCreatePropuesta.test.ts` - muestra toast exito tras crear |
| AC-CS03-8 | CTA crear perfil si sin PerfilProfesional | `NecesidadDetallePage.test.tsx` - muestra CTA crear perfil |
| AC-CS03-9 | Badges de estado con colores semanticos | `EstadoPropuestaBadge.test.tsx` - verifica colores por estado |
| AC-CS03-10 | Boton "Retirar" solo en Pendiente | `PropuestaCard.test.tsx` - muestra boton Retirar solo si Pendiente |
| AC-CS03-11 | Dialog confirmacion antes de retirar | `mis-propuestas-retirar.test.tsx` - flujo completo retirar |
| AC-CS03-12 | Empty state con filtros activos | `ExplorarNecesidadesPage.test.tsx` - empty state con filtros |

---

## 9. Edge Cases y Errores

### 9.1 Estados de Necesidad (Campos Calculados)

| Caso | Test | Archivo |
|------|------|---------|
| yaPropuso=true | muestra "Ya enviaste" | NecesidadDetallePage.test.tsx |
| esPropietario=true | muestra "Esta es tu necesidad" | NecesidadDetallePage.test.tsx |
| tienePerfilProfesional=false | CTA crear perfil | NecesidadDetallePage.test.tsx |
| Todos false, perfil presente | Boton habilitado | NecesidadDetallePage.test.tsx |

### 9.2 Validaciones del Formulario

| Caso | Test | Archivo |
|------|------|---------|
| Precio = 0 | Error "debe ser mayor a 0" | propuesta.schema.test.ts |
| Precio negativo | Error validacion | propuesta.schema.test.ts |
| Mensaje < 20 chars | Error "al menos 20 caracteres" | propuesta.schema.test.ts |
| Mensaje > 2000 chars | Error "maximo 2000" | propuesta.schema.test.ts |
| Dias = 0 | Error validacion | propuesta.schema.test.ts |
| Dias = 366 | Error "max 365 dias" | propuesta.schema.test.ts |
| Precio fuera de rango | Nota informativa (no bloquea) | EnviarPropuestaForm.test.tsx |

### 9.3 Errores de API

| Caso | Test | Archivo |
|------|------|---------|
| 400 precio invalido | Toast error validacion | useCreatePropuesta.test.ts |
| 400 mensaje corto | Toast error mensaje | useCreatePropuesta.test.ts |
| 400 ya existe propuesta | Toast "Ya tienes una propuesta" | useCreatePropuesta.test.ts |
| 403 sin perfilProfesional | Toast error autorizacion | useCreatePropuesta.test.ts |
| 403 propietario | Toast "No puedes enviar..." | useCreatePropuesta.test.ts |
| 404 necesidad no encontrada | Error 404 en detalle | NecesidadDetallePage.test.tsx |
| 500 error interno | Toast error generico | useCreatePropuesta.test.ts |
| 400 retirar no-pendiente | Toast error retirada | useRetirarPropuesta.test.ts |

### 9.4 UI States

| Caso | Test | Archivo |
|------|------|---------|
| Loading inicial - 6 skeletons | muestra skeletons loading | ExplorarNecesidadesPage.test.tsx |
| Loading detalle - 3 card skeletons | muestra skeletons loading | NecesidadDetallePage.test.tsx |
| Loading mis propuestas - 4 skeletons | muestra skeletons loading | MisPropuestasPage.test.tsx |
| Empty sin filtros | Icono Inbox + mensaje sin oportunidades | ExplorarNecesidadesPage.test.tsx |
| Empty con filtros | Icono SearchX + boton limpiar | ExplorarNecesidadesPage.test.tsx |
| Error de red | Icono error + boton Reintentar | ExplorarNecesidadesPage.test.tsx |
| Form submitting | Spinner + "Enviando..." + inputs disabled | EnviarPropuestaForm.test.tsx |
| Dialog retirar submitting | Spinner + "Retirando..." + disabled | RetirarPropuestaDialog.test.tsx |

---

## 10. Orden de Implementacion

El orden recomendado para implementar los tests, de mayor a menor prioridad de negocio y
menor a mayor complejidad de setup:

```
Fase 1 - Schemas y Mocks (sin dependencias)
  1. propuesta.schema.test.ts              (logica pura, sin mocks de componentes)
  2. __mocks__/necesidad.mock.ts           (datos de prueba)
  3. __mocks__/propuesta.mock.ts           (datos de prueba)
  4. __mocks__/handlers.ts                 (MSW handlers)
  5. test-utils.tsx                        (utilidades compartidas)

Fase 2 - Services (con MSW)
  6. necesidadPublica.service.test.ts      (servicios HTTP)
  7. propuesta.service.test.ts             (servicios HTTP)

Fase 3 - Hooks (con service mocks)
  8. useNecesidadesPublicas.test.ts        (query basico)
  9. useNecesidadPublica.test.ts           (query con campos calculados)
 10. useCreatePropuesta.test.ts            (mutation critica con invalidaciones)
 11. useMisPropuestas.test.ts              (query con filtro)
 12. useRetirarPropuesta.test.ts           (mutation con invalidacion)

Fase 4 - Componentes simples (unit tests)
 13. EstadoPropuestaBadge.test.tsx         (componente visual puro)
 14. UrgenciaBadge.test.tsx               (componente visual puro)
 15. EmptyStateNecesidades.test.tsx        (componente informativo)
 16. EmptyStatePropuestas.test.tsx         (componente informativo)
 17. NecesidadCard.test.tsx               (card con logica de urgencia y click)
 18. PropuestaCard.test.tsx               (card con acciones condicionales)

Fase 5 - Componentes complejos
 19. RetirarPropuestaDialog.test.tsx       (dialog con confirmacion)
 20. NecesidadFilters.test.tsx            (filtros con debounce)
 21. EnviarPropuestaForm.test.tsx          (formulario con validacion compleja)

Fase 6 - Pages
 22. ExplorarNecesidadesPage.test.tsx      (page con hooks + filtros)
 23. MisPropuestasPage.test.tsx            (page con hooks + dialog)
 24. NecesidadDetallePage.test.tsx         (page con 5 estados de CTA)

Fase 7 - Integration Tests
 25. explorar-enviar-propuesta.test.tsx    (flujo critico end-to-end)
 26. mis-propuestas-retirar.test.tsx       (flujo de retirada)
```

---

## 11. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del proyecto
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar tests de esta feature especifica
npm run test -- src/features/crowdsourcing

# Ejecutar solo los integration tests
npm run test -- src/features/crowdsourcing/__tests__/integration

# Watch mode durante desarrollo
npm run test:watch

# UI mode (Vitest UI - si esta instalado)
npx vitest --ui

# Ejecutar un archivo especifico
npm run test -- src/features/crowdsourcing/__tests__/components/EnviarPropuestaForm.test.tsx
```

---

## 12. CI/CD Integration

```yaml
name: Frontend Tests - cs-explorar-propuestas

on:
  push:
    branches: [master]
    paths:
      - 'src/web/src/features/crowdsourcing/**'
      - 'src/shared/**'
  pull_request:
    branches: [master]

jobs:
  test:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: src/web

    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: src/web/package-lock.json

      - run: npm ci

      - run: npm run test -- --coverage --reporter=verbose

      - name: Upload Coverage
        uses: codecov/codecov-action@v4
        with:
          files: ./src/web/coverage/coverage-final.json
          flags: frontend-landing-crowdsourcing
          fail_ci_if_error: true
```

---

## 13. Testing Best Practices Aplicadas

### Patron AAA

```typescript
it('nombre descriptivo del caso', async () => {
    // Arrange - Preparar datos y mocks
    const mockData = mockNecesidadDetalle;
    vi.mocked(service.getById).mockResolvedValue(mockData);

    // Act - Ejecutar la accion bajo test
    renderWithProviders(<Componente />);
    await waitFor(() => expect(screen.getByText('Titulo')).toBeInTheDocument());

    // Assert - Verificar el resultado
    expect(screen.getByRole('button', { name: /Enviar/i })).not.toBeDisabled();
});
```

### Queries por Prioridad (Testing Library)

```typescript
// PREFERIR - Accesibles y semanticas
screen.getByRole('button', { name: /Enviar propuesta/i })
screen.getByLabelText(/Precio propuesto/i)
screen.getByRole('article')  // NecesidadCard

// ACEPTABLE - Para contenido textual
screen.getByText('Los Rockeros')
screen.getByText(/URGENTE/i)

// EVITAR - Fragiles, se rompen con refactor
screen.getByTestId('necesidad-card')
screen.getByClassName('urgencia-badge')
```

### User Events vs FireEvent

```typescript
// USAR userEvent (mas realista - simula comportamiento real del usuario)
const user = userEvent.setup();
await user.click(button);
await user.type(input, 'texto');
await user.keyboard('{Enter}');
await user.tab(); // trigger blur

// EVITAR fireEvent (menos realista, no simula todos los eventos del navegador)
fireEvent.click(button);
fireEvent.change(input, { target: { value: 'texto' } });
```

---

## 14. Checklist de Implementacion

### Setup Base
- [ ] Mock data creado: `__mocks__/necesidad.mock.ts`
- [ ] Mock data creado: `__mocks__/propuesta.mock.ts`
- [ ] MSW handlers: `__mocks__/handlers.ts`
- [ ] Test utilities: `test-utils.tsx`
- [ ] vitest.setup.ts actualizado con MSW server

### Schema Tests
- [ ] propuesta.schema.test.ts (10 casos)

### Service Tests
- [ ] necesidadPublica.service.test.ts (6 casos)
- [ ] propuesta.service.test.ts (7 casos)

### Hook Tests
- [ ] useNecesidadesPublicas.test.ts (6 casos)
- [ ] useNecesidadPublica.test.ts (4 casos)
- [ ] useCreatePropuesta.test.ts (9 casos)
- [ ] useMisPropuestas.test.ts (5 casos)
- [ ] useRetirarPropuesta.test.ts (5 casos)

### Component Tests
- [ ] NecesidadCard.test.tsx (15 casos)
- [ ] PropuestaCard.test.tsx (11 casos)
- [ ] EstadoPropuestaBadge.test.tsx (5 casos)
- [ ] UrgenciaBadge.test.tsx (3 casos)
- [ ] EnviarPropuestaForm.test.tsx (16 casos)
- [ ] RetirarPropuestaDialog.test.tsx (7 casos)
- [ ] NecesidadFilters.test.tsx (7 casos)
- [ ] EmptyStateNecesidades.test.tsx (4 casos)
- [ ] EmptyStatePropuestas.test.tsx (3 casos)

### Page Tests
- [ ] ExplorarNecesidadesPage.test.tsx (8 casos)
- [ ] NecesidadDetallePage.test.tsx (10 casos)
- [ ] MisPropuestasPage.test.tsx (9 casos)

### Integration Tests
- [ ] explorar-enviar-propuesta.test.tsx (4 casos)
- [ ] mis-propuestas-retirar.test.tsx (3 casos)

### Coverage y CI
- [ ] Cobertura 80%+ en todas las metricas
- [ ] Tests pasan en < 60 segundos
- [ ] CI/CD pipeline configurado

---

**Fin de la estrategia de testing para cs-explorar-propuestas (Landing).**
