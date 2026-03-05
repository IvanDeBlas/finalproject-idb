# Estrategia de Testing: Hacer Backing (Landing)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/web (Landing - Vite + React)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen

| Tipo | Cantidad | Cobertura Estimada |
|------|----------|-------------------|
| Component Tests | 8 componentes | 75% |
| Hook Tests | 3 hooks | 90% |
| Schema Tests | 1 schema + 3 validators | 95% |
| Integration Tests | 2 flujos | 60% |
| **Total** | **14 test suites** | **80%+** |

### Filosofia de Testing

```
        /\
       /  \      E2E (manual, post-MVP)
      /----\
     /      \    Integration (flujos criticos)
    /--------\
   /          \  Unit (componentes + hooks)
  /____________\ Schema validators (mocks)
```

**Prioridad:**
1. Integration tests para flujo completo de backing
2. Unit tests para validaciones y edge cases
3. Component tests para renderizado y UI states

---

## 2. Estructura de Tests

```
src/web/src/features/hacer-backing/
├── __tests__/
│   ├── components/
│   │   ├── CampaniaCard.test.tsx
│   │   ├── CampaniaDetailPage.test.tsx
│   │   ├── BackingModal.test.tsx
│   │   ├── RewardPublicCard.test.tsx
│   │   ├── CampaniaProgressBar.test.tsx
│   │   ├── AmountInput.test.tsx
│   │   ├── BackingConfirmationPage.test.tsx
│   │   └── BackingsRecentesList.test.tsx
│   ├── hooks/
│   │   ├── useCampanias.test.ts
│   │   ├── useCampaniaDetail.test.ts
│   │   └── useCreateBacking.test.ts
│   ├── schemas/
│   │   └── backing.schema.test.ts
│   └── integration/
│       ├── backing-flow.test.tsx
│       └── anonymous-backing.test.tsx
├── __mocks__/
│   ├── campania.mock.ts
│   ├── backing.mock.ts
│   └── handlers.ts (MSW)
└── test-utils.tsx
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/campania.mock.ts`

```typescript
import type { CampaniaDetail, RewardPublic, BackingPublicDto } from '@shared/types';

export const mockCampaniaActive: CampaniaDetail = {
    id: '550e8400-e29b-41d4-a716-446655440000',
    artistaId: '660e8400-e29b-41d4-a716-446655440001',
    titulo: 'Mi Album Debut',
    subtitulo: 'Rock alternativo desde Madrid',
    descripcionCorta: 'Un viaje musical de 10 canciones originales',
    videoPrincipalUrl: 'https://youtube.com/watch?v=test',
    imagenPrincipalUrl: 'https://example.com/image.jpg',
    importeObjetivo: 5000,
    importeMinimo: 1,
    importePledgedActual: 2340,
    porcentajeProgreso: 46.8,
    monedaId: 1,
    monedaSimbolo: 'EUR',
    estadoCampaniaId: 2, // PUBLICADA
    estadoCampaniaNombre: 'Publicada',
    permiteAportacionesAnonimas: true,
    permitePropinas: true,
    fechaInicio: '2026-02-01T00:00:00Z',
    fechaFin: '2026-03-31T23:59:59Z',
    diasRestantes: 46,
    artistaNombre: 'Juan Perez Music',
    artistaImagenUrl: 'https://example.com/artist.jpg',
    rewards: [
        mockRewardDigital,
        mockRewardCD,
        mockRewardVinilo
    ],
    backingsRecientes: [
        mockBackingRecent1,
        mockBackingRecent2
    ],
    totalBackers: 234,
    fechaCreacion: '2026-01-15T12:00:00Z'
};

export const mockCampaniaFinalizada: CampaniaDetail = {
    ...mockCampaniaActive,
    estadoCampaniaId: 3, // FINALIZADA
    estadoCampaniaNombre: 'Finalizada',
    diasRestantes: 0
};

export const mockCampaniaNoAnonimos: CampaniaDetail = {
    ...mockCampaniaActive,
    permiteAportacionesAnonimas: false
};

export const mockRewardDigital: RewardPublic = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    nombre: 'Descarga Digital',
    descripcion: 'Album completo en MP3 y FLAC',
    importeMinimo: 10,
    cantidadMaxima: undefined, // ilimitado
    cantidadVendida: 45,
    disponible: true,
    incluyeEnvioFisico: false,
    tiempoEntregaEstimado: 'Inmediato tras finalizar campania',
    orden: 1,
    esActivo: true
};

export const mockRewardCD: RewardPublic = {
    id: '223e4567-e89b-12d3-a456-426614174001',
    nombre: 'CD Fisico Firmado',
    descripcion: 'CD fisico con firma del artista',
    importeMinimo: 25,
    cantidadMaxima: 100,
    cantidadVendida: 78,
    disponible: true, // 78 < 100
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: '60 dias tras finalizar campania',
    orden: 2,
    esActivo: true
};

export const mockRewardVinilo: RewardPublic = {
    id: '323e4567-e89b-12d3-a456-426614174002',
    nombre: 'Vinilo Edicion Limitada',
    descripcion: 'Vinilo en color especial + poster',
    importeMinimo: 50,
    cantidadMaxima: 50,
    cantidadVendida: 50,
    disponible: false, // AGOTADO
    incluyeEnvioFisico: true,
    tiempoEntregaEstimado: '90 dias tras finalizar campania',
    orden: 3,
    esActivo: true
};

export const mockBackingRecent1: BackingPublicDto = {
    id: '700e8400-e29b-41d4-a716-446655440001',
    nombreBacker: 'Maria Lopez',
    monto: 25,
    rewardNombre: 'CD Fisico Firmado',
    mensaje: 'Mucha suerte con el proyecto!',
    fechaCreacion: '2026-02-13T10:30:00Z'
};

export const mockBackingRecent2: BackingPublicDto = {
    id: '800e8400-e29b-41d4-a716-446655440002',
    nombreBacker: 'Anonimo',
    monto: 50,
    rewardNombre: undefined,
    mensaje: undefined,
    fechaCreacion: '2026-02-13T09:15:00Z'
};

export const mockCampaniasList = [
    mockCampaniaActive,
    { ...mockCampaniaActive, id: '550e8400-e29b-41d4-a716-446655440001', titulo: 'Segundo Album' }
];
```

**Archivo:** `__mocks__/backing.mock.ts`

```typescript
import type { BackingDto, CreateBackingRequest } from '@shared/types';

export const mockCreateBackingRequest: CreateBackingRequest = {
    rewardId: '223e4567-e89b-12d3-a456-426614174001',
    monto: 25,
    mensaje: 'Mucha suerte!',
    esAnonimo: false
};

export const mockBackingResponse: BackingDto = {
    id: '900e8400-e29b-41d4-a716-446655440003',
    campaniaId: '550e8400-e29b-41d4-a716-446655440000',
    userId: 'user-123',
    rewardId: '223e4567-e89b-12d3-a456-426614174001',
    monto: 25,
    mensaje: 'Mucha suerte!',
    esAnonimo: false,
    fechaCreacion: new Date().toISOString(),
    campaniaTitulo: 'Mi Album Debut',
    userName: 'Juan Perez',
    rewardNombre: 'CD Fisico Firmado',
    monedaSimbolo: 'EUR',
    estadoPedido: 'Completado'
};

export const mockBackingSinReward: BackingDto = {
    ...mockBackingResponse,
    rewardId: undefined,
    rewardNombre: undefined,
    monto: 15
};

export const mockBackingAnonimo: BackingDto = {
    ...mockBackingResponse,
    esAnonimo: true,
    userName: 'Anonimo',
    userId: undefined
};
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import { mockCampaniasList, mockCampaniaActive, mockBackingResponse } from './campania.mock';

const API_BASE = 'http://localhost:5001/api';

export const campaniaHandlers = [
    // GET /api/campanias - Listar campanias
    http.get(`${API_BASE}/campanias`, ({ request }) => {
        const url = new URL(request.url);
        const page = parseInt(url.searchParams.get('pageNumber') || '1');
        const pageSize = parseInt(url.searchParams.get('pageSize') || '10');

        return HttpResponse.json({
            data: {
                items: mockCampaniasList,
                totalCount: mockCampaniasList.length,
                page,
                pageSize,
                totalPages: Math.ceil(mockCampaniasList.length / pageSize)
            },
            messages: [{ message: 'Campanias encontradas', errorCode: '0000' }]
        });
    }),

    // GET /api/campanias/:id - Detalle campania
    http.get(`${API_BASE}/campanias/:id`, ({ params }) => {
        const { id } = params;

        if (id === mockCampaniaActive.id) {
            return HttpResponse.json({
                data: mockCampaniaActive,
                messages: [{ message: 'Campania encontrada', errorCode: '0000' }]
            });
        }

        return HttpResponse.json(
            {
                data: null,
                messages: [{ message: 'Campania no encontrada', errorCode: '2003' }]
            },
            { status: 404 }
        );
    }),

    // POST /api/campanias/:id/backings - Crear backing
    http.post(`${API_BASE}/campanias/:id/backings`, async ({ request, params }) => {
        const body = await request.json();
        const { id } = params;

        // Simular validaciones
        if (body.monto < 1) {
            return HttpResponse.json(
                {
                    data: null,
                    messages: [{ message: 'El monto debe ser al menos 1 EUR', errorCode: '1011' }]
                },
                { status: 400 }
            );
        }

        // Simular creacion exitosa
        return HttpResponse.json(
            {
                data: mockBackingResponse,
                messages: [{
                    message: 'Aporte realizado con exito. Gracias por tu apoyo!',
                    errorCode: '0001'
                }]
            },
            { status: 201 }
        );
    }),

    // GET /api/campanias/:id/backings - Listar backings de campania
    http.get(`${API_BASE}/campanias/:id/backings`, () => {
        return HttpResponse.json({
            data: {
                items: mockCampaniaActive.backingsRecientes,
                totalCount: mockCampaniaActive.totalBackers,
                page: 1,
                pageSize: 20,
                totalPages: 12
            },
            messages: [{ message: 'Aportes encontrados', errorCode: '0000' }]
        });
    })
];

// Handlers de error para testing
export const campaniaErrorHandlers = [
    http.get(`${API_BASE}/campanias/:id`, () => {
        return HttpResponse.json(
            {
                data: null,
                messages: [{ message: 'Error interno', errorCode: '5000' }]
            },
            { status: 500 }
        );
    }),

    http.post(`${API_BASE}/campanias/:id/backings`, () => {
        return HttpResponse.json(
            {
                data: null,
                messages: [{
                    message: 'Ha ocurrido un error al procesar tu aporte',
                    errorCode: '5000'
                }]
            },
            { status: 500 }
        );
    })
];
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import React from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';
import { vi } from 'vitest';

// Mock useAuth hook
vi.mock('@/hooks/useAuth', () => ({
    useAuth: vi.fn(() => ({
        isAuthenticated: false,
        user: null,
        login: vi.fn(),
        logout: vi.fn()
    }))
}));

export const createTestQueryClient = () =>
    new QueryClient({
        defaultOptions: {
            queries: {
                retry: false,
                gcTime: 0,
                staleTime: 0,
            },
            mutations: {
                retry: false,
            },
        },
    });

interface AllProvidersProps {
    children: React.ReactNode;
    queryClient?: QueryClient;
}

function AllProviders({ children, queryClient }: AllProvidersProps) {
    const client = queryClient || createTestQueryClient();

    return (
        <QueryClientProvider client={client}>
            <BrowserRouter>
                {children}
            </BrowserRouter>
        </QueryClientProvider>
    );
}

export function renderWithProviders(
    ui: React.ReactElement,
    options?: RenderOptions & { queryClient?: QueryClient }
) {
    const { queryClient, ...renderOptions } = options || {};

    return render(ui, {
        wrapper: (props) => <AllProviders {...props} queryClient={queryClient} />,
        ...renderOptions
    });
}

// Helper para crear wrapper de hooks
export function createWrapper(queryClient?: QueryClient) {
    const client = queryClient || createTestQueryClient();

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return (
            <QueryClientProvider client={client}>
                <BrowserRouter>
                    {children}
                </BrowserRouter>
            </QueryClientProvider>
        );
    };
}

// Mock userEvent
export { userEvent } from '@testing-library/user-event';
```

---

## 4. Tests por Modulo

### 4.1 Components

#### CampaniaCard.test.tsx

**Archivo:** `__tests__/components/CampaniaCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with basic data | Unit | Renderiza titulo, artista, objetivo |
| displays progress bar correctly | Unit | Barra con porcentaje correcto |
| shows days remaining badge | Unit | Badge "X dias restantes" |
| shows correct amounts formatted | Unit | Formatea EUR correctamente |
| navigates on click | Unit | Click redirige a detalle |
| shows placeholder image on error | Unit | Imagen placeholder si falla carga |
| applies hover effect | Unit | Hover cambia estilos |

**Casos Detallados:**

```typescript
describe('CampaniaCard', () => {
    const mockCampania = {
        id: '123',
        titulo: 'Mi Album',
        artistaNombre: 'Juan Perez',
        importeObjetivo: 5000,
        importePledgedActual: 2340,
        porcentajeProgreso: 46.8,
        diasRestantes: 46,
        imagenPrincipalUrl: 'https://example.com/image.jpg'
    };

    it('renders with basic data', () => {
        renderWithProviders(<CampaniaCard campania={mockCampania} />);

        expect(screen.getByText('Mi Album')).toBeInTheDocument();
        expect(screen.getByText('Juan Perez')).toBeInTheDocument();
        expect(screen.getByText('5,000 EUR')).toBeInTheDocument(); // objetivo
        expect(screen.getByText('2,340 EUR')).toBeInTheDocument(); // recaudado
    });

    it('displays progress bar correctly', () => {
        renderWithProviders(<CampaniaCard campania={mockCampania} />);

        const progressBar = screen.getByRole('progressbar');
        expect(progressBar).toHaveAttribute('aria-valuenow', '46.8');
        expect(screen.getByText('46.8%')).toBeInTheDocument();
    });

    it('shows days remaining badge', () => {
        renderWithProviders(<CampaniaCard campania={mockCampania} />);

        expect(screen.getByText('46 dias restantes')).toBeInTheDocument();
    });

    it('navigates on click', async () => {
        const user = userEvent.setup();
        renderWithProviders(<CampaniaCard campania={mockCampania} />);

        const card = screen.getByRole('link');
        expect(card).toHaveAttribute('href', `/campanias/${mockCampania.id}`);
    });
});
```

#### CampaniaDetailPage.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| shows loading skeleton initially | Unit | Skeleton mientras carga |
| renders full detail when loaded | Integration | Renderiza hero, tabs, sidebar, rewards |
| displays artist info correctly | Unit | Nombre, imagen artista |
| shows tabs: Descripcion, Recompensas, Apoyos | Unit | 3 tabs presentes |
| renders rewards list | Integration | Lista de rewards ordenada |
| shows backings recientes | Unit | Lista de backings recientes |
| shows "Apoyar" button when active | Unit | Boton visible si estado=2 |
| disables "Apoyar" when finalizada | Unit | Boton deshabilitado si finalizada |
| shows error message on fetch error | Unit | Mensaje error si API falla |
| refetches on button retry click | Unit | Retry ejecuta query nuevamente |

**Casos Detallados:**

```typescript
describe('CampaniaDetailPage', () => {
    it('shows loading skeleton initially', () => {
        vi.mocked(campaniaService.getById).mockReturnValue(
            new Promise(() => {}) // Never resolves
        );

        renderWithProviders(<CampaniaDetailPage />);

        expect(screen.getByTestId('campania-detail-skeleton')).toBeInTheDocument();
    });

    it('renders full detail when loaded', async () => {
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaActive);

        renderWithProviders(<CampaniaDetailPage />);

        await waitFor(() => {
            expect(screen.getByText('Mi Album Debut')).toBeInTheDocument();
        });

        expect(screen.getByText('Juan Perez Music')).toBeInTheDocument();
        expect(screen.getByText('Descripcion')).toBeInTheDocument(); // Tab
        expect(screen.getByText('Recompensas')).toBeInTheDocument(); // Tab
        expect(screen.getByText('Apoyos')).toBeInTheDocument(); // Tab
        expect(screen.getByRole('button', { name: /Apoyar/i })).toBeInTheDocument();
    });

    it('disables "Apoyar" when finalizada', async () => {
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaFinalizada);

        renderWithProviders(<CampaniaDetailPage />);

        await waitFor(() => {
            expect(screen.getByText('Campania finalizada')).toBeInTheDocument();
        });

        const apoyarButton = screen.getByRole('button', { name: /finalizada/i });
        expect(apoyarButton).toBeDisabled();
    });
});
```

#### BackingModal.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with reward selected | Unit | Muestra reward seleccionado |
| pre-fills amount with reward minimum | Unit | Input pre-rellenado con min |
| shows validation error if amount < min | Integration | Error inline si monto < min |
| allows typing custom amount >= min | Integration | User puede cambiar monto |
| shows character counter for mensaje | Unit | Contador "X/500" |
| validates mensaje max 500 chars | Integration | Error si > 500 chars |
| toggles "anonimo" checkbox | Unit | Checkbox funciona |
| shows loading state on submit | Unit | Button disabled + spinner |
| calls mutation on submit | Integration | useMutation ejecutado |
| closes modal on success | Integration | Modal se cierra tras exito |
| shows error toast on mutation error | Integration | Toast error si falla API |
| allows backing sin recompensa | Integration | RewardId undefined permitido |

**Casos Detallados:**

```typescript
describe('BackingModal', () => {
    const mockReward = mockRewardCD;
    const mockCampania = mockCampaniaActive;

    it('pre-fills amount with reward minimum', () => {
        renderWithProviders(
            <BackingModal
                campania={mockCampania}
                reward={mockReward}
                open={true}
                onClose={vi.fn()}
            />
        );

        const amountInput = screen.getByLabelText(/Monto/i);
        expect(amountInput).toHaveValue(mockReward.importeMinimo); // 25
    });

    it('shows validation error if amount < min', async () => {
        const user = userEvent.setup();
        renderWithProviders(
            <BackingModal
                campania={mockCampania}
                reward={mockReward}
                open={true}
                onClose={vi.fn()}
            />
        );

        const amountInput = screen.getByLabelText(/Monto/i);
        await user.clear(amountInput);
        await user.type(amountInput, '10'); // < 25 min

        await waitFor(() => {
            expect(screen.getByText(/El monto debe ser al menos 25 EUR/i)).toBeInTheDocument();
        });

        const submitButton = screen.getByRole('button', { name: /Confirmar apoyo/i });
        expect(submitButton).toBeDisabled();
    });

    it('calls mutation on submit', async () => {
        const user = userEvent.setup();
        const mockMutate = vi.fn();
        vi.mocked(useCreateBacking).mockReturnValue({
            mutate: mockMutate,
            isLoading: false,
            isError: false,
            error: null
        });

        renderWithProviders(
            <BackingModal
                campania={mockCampania}
                reward={mockReward}
                open={true}
                onClose={vi.fn()}
            />
        );

        const mensajeInput = screen.getByLabelText(/Mensaje/i);
        await user.type(mensajeInput, 'Mucha suerte!');

        const submitButton = screen.getByRole('button', { name: /Confirmar apoyo/i });
        await user.click(submitButton);

        await waitFor(() => {
            expect(mockMutate).toHaveBeenCalledWith({
                campaniaId: mockCampania.id,
                rewardId: mockReward.id,
                monto: 25,
                mensaje: 'Mucha suerte!',
                esAnonimo: false
            });
        });
    });

    it('validates mensaje max 500 chars', async () => {
        const user = userEvent.setup();
        renderWithProviders(
            <BackingModal
                campania={mockCampania}
                reward={mockReward}
                open={true}
                onClose={vi.fn()}
            />
        );

        const mensajeInput = screen.getByLabelText(/Mensaje/i);
        const longMessage = 'a'.repeat(501);
        await user.type(mensajeInput, longMessage);

        await waitFor(() => {
            expect(screen.getByText(/no puede superar los 500 caracteres/i)).toBeInTheDocument();
        });
    });
});
```

#### RewardPublicCard.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders reward info correctly | Unit | Nombre, descripcion, precio |
| shows stock disponible badge | Unit | Badge "Quedan X" |
| shows "Agotado" badge if no stock | Unit | Badge "Agotado" si disponible=false |
| disables select button if stock=0 | Unit | Button disabled si agotado |
| applies selected highlight | Unit | Border highlight si selected=true |
| calls onSelect on button click | Unit | onSelect(rewardId) ejecutado |
| shows delivery estimate | Unit | Tiempo entrega mostrado |
| shows "Envio incluido" if fisico | Unit | Badge "Envio incluido" |

**Casos Detallados:**

```typescript
describe('RewardPublicCard', () => {
    it('shows "Agotado" badge if no stock', () => {
        renderWithProviders(
            <RewardPublicCard reward={mockRewardVinilo} onSelect={vi.fn()} />
        );

        expect(screen.getByText('Agotado')).toBeInTheDocument();
    });

    it('disables select button if stock=0', () => {
        renderWithProviders(
            <RewardPublicCard reward={mockRewardVinilo} onSelect={vi.fn()} />
        );

        const selectButton = screen.getByRole('button', { name: /Seleccionar/i });
        expect(selectButton).toBeDisabled();
    });

    it('calls onSelect on button click', async () => {
        const user = userEvent.setup();
        const mockOnSelect = vi.fn();

        renderWithProviders(
            <RewardPublicCard reward={mockRewardDigital} onSelect={mockOnSelect} />
        );

        const selectButton = screen.getByRole('button', { name: /Seleccionar/i });
        await user.click(selectButton);

        expect(mockOnSelect).toHaveBeenCalledWith(mockRewardDigital.id);
    });
});
```

#### CampaniaProgressBar.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| displays percentage correctly | Unit | Barra con % correcto |
| applies green color if >= 100% | Unit | Color green si meta alcanzada |
| applies yellow color if 50-99% | Unit | Color yellow si medio camino |
| applies red color if < 50% | Unit | Color red si poco progreso |
| shows amounts in EUR format | Unit | "2,340 EUR de 5,000 EUR" |
| animates progress on mount | Unit | Animacion smooth al renderizar |

**Casos Detallados:**

```typescript
describe('CampaniaProgressBar', () => {
    it('applies green color if >= 100%', () => {
        renderWithProviders(
            <CampaniaProgressBar
                importeObjetivo={5000}
                importePledgedActual={5500}
                porcentaje={110}
            />
        );

        const progressBar = screen.getByRole('progressbar');
        expect(progressBar).toHaveClass('bg-green-500');
    });

    it('applies yellow color if 50-99%', () => {
        renderWithProviders(
            <CampaniaProgressBar
                importeObjetivo={5000}
                importePledgedActual={3000}
                porcentaje={60}
            />
        );

        const progressBar = screen.getByRole('progressbar');
        expect(progressBar).toHaveClass('bg-yellow-500');
    });

    it('applies red color if < 50%', () => {
        renderWithProviders(
            <CampaniaProgressBar
                importeObjetivo={5000}
                importePledgedActual={1000}
                porcentaje={20}
            />
        );

        const progressBar = screen.getByRole('progressbar');
        expect(progressBar).toHaveClass('bg-red-500');
    });
});
```

#### AmountInput.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders with initial value | Unit | Input con valor inicial |
| shows currency symbol | Unit | "EUR" o simbolo mostrado |
| validates min value | Unit | Error si < min |
| validates max value | Unit | Error si > max |
| allows only numeric input | Unit | No permite letras |
| formats value on blur | Unit | Formatea a 2 decimales |
| calls onChange on value change | Unit | onChange ejecutado |

**Casos Detallados:**

```typescript
describe('AmountInput', () => {
    it('validates min value', async () => {
        const user = userEvent.setup();
        renderWithProviders(
            <AmountInput
                value={0}
                onChange={vi.fn()}
                min={10}
                label="Monto"
            />
        );

        const input = screen.getByLabelText(/Monto/i);
        await user.clear(input);
        await user.type(input, '5'); // < 10 min

        await waitFor(() => {
            expect(screen.getByText(/El monto debe ser al menos 10/i)).toBeInTheDocument();
        });
    });

    it('allows only numeric input', async () => {
        const user = userEvent.setup();
        const mockOnChange = vi.fn();

        renderWithProviders(
            <AmountInput
                value={0}
                onChange={mockOnChange}
                label="Monto"
            />
        );

        const input = screen.getByLabelText(/Monto/i);
        await user.type(input, 'abc123');

        // Solo acepta numeros
        expect(mockOnChange).toHaveBeenCalledWith(123);
        expect(mockOnChange).not.toHaveBeenCalledWith('abc123');
    });
});
```

#### BackingConfirmationPage.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders success message | Unit | Icono success + mensaje |
| displays backing summary | Unit | Campania, reward, monto |
| shows "Volver a campania" button | Unit | Button presente |
| shows "Ver mis apoyos" if authenticated | Unit | Button solo si auth |
| hides "Ver mis apoyos" if anonymous | Unit | Button oculto si anonimo |
| navigates to campania on click | Unit | Navigate ejecutado |

**Casos Detallados:**

```typescript
describe('BackingConfirmationPage', () => {
    const mockBacking = mockBackingResponse;

    it('renders success message', () => {
        renderWithProviders(<BackingConfirmationPage backing={mockBacking} />);

        expect(screen.getByText(/Gracias por tu apoyo/i)).toBeInTheDocument();
    });

    it('shows "Ver mis apoyos" if authenticated', () => {
        vi.mocked(useAuth).mockReturnValue({
            isAuthenticated: true,
            user: { id: 'user-123', name: 'Juan' }
        });

        renderWithProviders(<BackingConfirmationPage backing={mockBacking} />);

        expect(screen.getByRole('button', { name: /Ver mis apoyos/i })).toBeInTheDocument();
    });

    it('hides "Ver mis apoyos" if anonymous', () => {
        vi.mocked(useAuth).mockReturnValue({
            isAuthenticated: false,
            user: null
        });

        renderWithProviders(<BackingConfirmationPage backing={mockBacking} />);

        expect(screen.queryByRole('button', { name: /Ver mis apoyos/i })).not.toBeInTheDocument();
    });
});
```

#### BackingsRecentesList.test.tsx

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders list of backings | Unit | Lista de backings renderizada |
| shows "Anonimo" for anonymous backers | Unit | Nombre "Anonimo" si aplica |
| formats amounts correctly | Unit | "25 EUR" formato |
| shows relative time | Unit | "hace 2 horas" |
| shows empty state if no backings | Unit | "No hay aportes aun" |
| limits to N backings | Unit | Solo muestra ultimos N |

**Casos Detallados:**

```typescript
describe('BackingsRecentesList', () => {
    it('shows "Anonimo" for anonymous backers', () => {
        const backings = [mockBackingRecent1, mockBackingRecent2];
        renderWithProviders(<BackingsRecentesList backings={backings} />);

        expect(screen.getByText('Maria Lopez')).toBeInTheDocument();
        expect(screen.getByText('Anonimo')).toBeInTheDocument();
    });

    it('shows empty state if no backings', () => {
        renderWithProviders(<BackingsRecentesList backings={[]} />);

        expect(screen.getByText(/No hay aportes aun/i)).toBeInTheDocument();
    });
});
```

---

### 4.2 Hooks

#### useCampanias.test.ts

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches campanias successfully | Unit | Query retorna datos |
| handles loading state | Unit | isLoading true inicialmente |
| handles error state | Unit | error cuando API falla |
| accepts query params | Unit | Pasa params a service |
| filters by estadoCampaniaId | Unit | Filtra por estado |
| paginates results | Unit | page y pageSize funcionan |
| does not fetch if enabled=false | Unit | enabled condicional |

**Casos Detallados:**

```typescript
describe('useCampanias', () => {
    it('fetches campanias successfully', async () => {
        vi.mocked(campaniaService.getAll).mockResolvedValue({
            items: mockCampaniasList,
            totalCount: 2,
            page: 1,
            pageSize: 10,
            totalPages: 1
        });

        const { result } = renderHook(
            () => useCampanias({ estadoCampaniaId: 2 }),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(result.current.data?.items).toHaveLength(2);
        expect(campaniaService.getAll).toHaveBeenCalledWith({
            estadoCampaniaId: 2,
            pageNumber: 1,
            pageSize: 10
        });
    });

    it('handles error state', async () => {
        vi.mocked(campaniaService.getAll).mockRejectedValue(
            new Error('API Error')
        );

        const { result } = renderHook(
            () => useCampanias(),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isError).toBe(true));

        expect(result.current.error).not.toBeNull();
        expect(result.current.data).toBeUndefined();
    });

    it('does not fetch if enabled=false', () => {
        vi.mocked(campaniaService.getAll).mockResolvedValue({
            items: [],
            totalCount: 0,
            page: 1,
            pageSize: 10,
            totalPages: 0
        });

        const { result } = renderHook(
            () => useCampanias({}, { enabled: false }),
            { wrapper: createWrapper() }
        );

        expect(result.current.isFetching).toBe(false);
        expect(campaniaService.getAll).not.toHaveBeenCalled();
    });
});
```

#### useCampaniaDetail.test.ts

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches campania detail successfully | Unit | Query retorna detalle completo |
| includes rewards in response | Unit | rewards array presente |
| includes backings recientes | Unit | backingsRecientes array presente |
| handles not found error | Unit | error 404 manejado |
| does not fetch if id is empty | Unit | enabled=false si id vacio |
| refetches on id change | Unit | Nueva query si id cambia |

**Casos Detallados:**

```typescript
describe('useCampaniaDetail', () => {
    const campaniaId = mockCampaniaActive.id;

    it('fetches campania detail successfully', async () => {
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaActive);

        const { result } = renderHook(
            () => useCampaniaDetail(campaniaId),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(result.current.data).toEqual(mockCampaniaActive);
        expect(result.current.data?.rewards).toHaveLength(3);
        expect(result.current.data?.backingsRecientes).toHaveLength(2);
    });

    it('does not fetch if id is empty', () => {
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaActive);

        const { result } = renderHook(
            () => useCampaniaDetail(''),
            { wrapper: createWrapper() }
        );

        expect(result.current.isFetching).toBe(false);
        expect(campaniaService.getById).not.toHaveBeenCalled();
    });

    it('handles not found error', async () => {
        vi.mocked(campaniaService.getById).mockRejectedValue({
            response: { status: 404, data: { errorCode: '2003' } }
        });

        const { result } = renderHook(
            () => useCampaniaDetail('invalid-id'),
            { wrapper: createWrapper() }
        );

        await waitFor(() => expect(result.current.isError).toBe(true));

        expect(result.current.error).not.toBeNull();
    });
});
```

#### useCreateBacking.test.ts

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| creates backing successfully | Unit | Mutation retorna backing |
| invalidates queries on success | Integration | Invalida campanias queries |
| shows success toast on success | Integration | toast.success llamado |
| shows error toast on error | Integration | toast.error llamado |
| handles validation errors | Unit | Errores 400 manejados |
| handles business rule errors | Unit | Errores 409 manejados |
| passes correct data to service | Unit | Service recibe datos correctos |

**Casos Detallados:**

```typescript
describe('useCreateBacking', () => {
    it('creates backing successfully', async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse);

        const queryClient = createTestQueryClient();
        const { result } = renderHook(
            () => useCreateBacking(),
            { wrapper: createWrapper(queryClient) }
        );

        const backingData = mockCreateBackingRequest;
        result.current.mutate({ ...backingData, campaniaId: mockCampaniaActive.id });

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(result.current.data).toEqual(mockBackingResponse);
        expect(backingService.create).toHaveBeenCalledWith(backingData);
    });

    it('invalidates queries on success', async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse);

        const queryClient = createTestQueryClient();
        const invalidateSpy = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(
            () => useCreateBacking(),
            { wrapper: createWrapper(queryClient) }
        );

        result.current.mutate({
            ...mockCreateBackingRequest,
            campaniaId: mockCampaniaActive.id
        });

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ['campanias', mockCampaniaActive.id]
        });
    });

    it('shows error toast on error', async () => {
        const mockToast = vi.fn();
        vi.mocked(toast).mockReturnValue(mockToast);
        vi.mocked(backingService.create).mockRejectedValue({
            response: {
                data: {
                    messages: [{ errorCode: '5000', message: 'Error inesperado' }]
                }
            }
        });

        const { result } = renderHook(
            () => useCreateBacking(),
            { wrapper: createWrapper() }
        );

        result.current.mutate({
            ...mockCreateBackingRequest,
            campaniaId: mockCampaniaActive.id
        });

        await waitFor(() => expect(result.current.isError).toBe(true));

        expect(mockToast).toHaveBeenCalledWith({
            title: 'Error al procesar apoyo',
            description: expect.stringContaining('Error inesperado'),
            variant: 'destructive'
        });
    });
});
```

---

### 4.3 Schemas

#### backing.schema.test.ts

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| validates monto >= 1 | Unit | Error si monto < 1 |
| validates monto <= 100000 | Unit | Error si monto > 100000 |
| validates mensaje max 500 chars | Unit | Error si mensaje > 500 |
| allows rewardId optional | Unit | rewardId puede ser undefined |
| validates rewardId is UUID | Unit | Error si rewardId no es UUID |
| allows esAnonimo default false | Unit | esAnonimo=false por defecto |
| validateBackingAmount returns error | Unit | Error si monto < reward.min |
| validateBackingAmount returns null if ok | Unit | null si monto valido |
| validateRewardAvailability detects agotado | Unit | Error si disponible=false |
| validateRewardAvailability detects inactivo | Unit | Error si esActivo=false |
| validateCampaniaActive detects finalizada | Unit | Error si estado != 2 |

**Casos Detallados:**

```typescript
describe('backing.schema', () => {
    describe('createBackingSchema', () => {
        it('validates monto >= 1', () => {
            const result = createBackingSchema.safeParse({
                monto: 0.5,
                esAnonimo: false
            });

            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toContain('minimo es 1 EUR');
            }
        });

        it('validates monto <= 100000', () => {
            const result = createBackingSchema.safeParse({
                monto: 150000,
                esAnonimo: false
            });

            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toContain('maximo es 100,000 EUR');
            }
        });

        it('validates mensaje max 500 chars', () => {
            const result = createBackingSchema.safeParse({
                monto: 10,
                mensaje: 'a'.repeat(501),
                esAnonimo: false
            });

            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toContain('500 caracteres');
            }
        });

        it('allows rewardId optional', () => {
            const result = createBackingSchema.safeParse({
                monto: 10,
                esAnonimo: false
            });

            expect(result.success).toBe(true);
            if (result.success) {
                expect(result.data.rewardId).toBeUndefined();
            }
        });
    });

    describe('validateBackingAmount', () => {
        it('returns error if monto < reward.min', () => {
            const error = validateBackingAmount(10, mockRewardCD);

            expect(error).not.toBeNull();
            expect(error).toContain('al menos 25 EUR');
        });

        it('returns null if ok', () => {
            const error = validateBackingAmount(30, mockRewardCD);

            expect(error).toBeNull();
        });
    });

    describe('validateRewardAvailability', () => {
        it('detects agotado', () => {
            const error = validateRewardAvailability(mockRewardVinilo);

            expect(error).not.toBeNull();
            expect(error).toContain('agotada');
        });

        it('detects inactivo', () => {
            const rewardInactivo = { ...mockRewardDigital, esActivo: false };
            const error = validateRewardAvailability(rewardInactivo);

            expect(error).not.toBeNull();
            expect(error).toContain('no esta disponible');
        });
    });

    describe('validateCampaniaActive', () => {
        it('detects finalizada', () => {
            const error = validateCampaniaActive(3); // FINALIZADA

            expect(error).not.toBeNull();
            expect(error).toContain('no esta activa');
        });

        it('detects fecha fin pasada', () => {
            const pastDate = new Date(Date.now() - 86400000).toISOString(); // yesterday
            const error = validateCampaniaActive(2, pastDate);

            expect(error).not.toBeNull();
            expect(error).toContain('ha finalizado');
        });
    });
});
```

---

### 4.4 Integration Tests

#### backing-flow.test.tsx

**Descripcion:** Flujo completo de backing autenticado con reward

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| complete backing flow with reward | Integration | Explorar → Detalle → Seleccionar reward → Submit → Confirmacion |
| validates amount before submit | Integration | Error si monto invalido |
| shows success confirmation after backing | Integration | Redirige a confirmacion |

**Casos Detallados:**

```typescript
describe('Backing Flow (Integration)', () => {
    it('complete backing flow with reward', async () => {
        const user = userEvent.setup();

        // Mock authenticated user
        vi.mocked(useAuth).mockReturnValue({
            isAuthenticated: true,
            user: { id: 'user-123', name: 'Juan Perez' }
        });

        // 1. Navigate to campanias list
        renderWithProviders(<CampaniasListPage />);

        await waitFor(() => {
            expect(screen.getByText('Mi Album Debut')).toBeInTheDocument();
        });

        // 2. Click on campania card
        const campaniaCard = screen.getByText('Mi Album Debut').closest('a');
        await user.click(campaniaCard!);

        // 3. Campania detail loaded
        await waitFor(() => {
            expect(screen.getByText('Recompensas')).toBeInTheDocument();
        });

        // 4. Select reward
        const rewardCard = screen.getByText('CD Fisico Firmado').closest('div');
        const selectButton = within(rewardCard!).getByRole('button', { name: /Seleccionar/i });
        await user.click(selectButton);

        // 5. Backing modal opened
        await waitFor(() => {
            expect(screen.getByRole('dialog')).toBeInTheDocument();
        });

        // 6. Fill form
        const mensajeInput = screen.getByLabelText(/Mensaje/i);
        await user.type(mensajeInput, 'Mucha suerte!');

        // 7. Submit
        const confirmButton = screen.getByRole('button', { name: /Confirmar apoyo/i });
        await user.click(confirmButton);

        // 8. Success confirmation
        await waitFor(() => {
            expect(screen.getByText(/Gracias por tu apoyo/i)).toBeInTheDocument();
        });

        expect(screen.getByText('CD Fisico Firmado')).toBeInTheDocument();
        expect(screen.getByText('25 EUR')).toBeInTheDocument();
    });
});
```

#### anonymous-backing.test.tsx

**Descripcion:** Flujo backing anonimo (sin autenticacion)

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| allows anonymous backing if campania permits | Integration | Backing sin auth si permitido |
| blocks anonymous backing if not permitted | Integration | Redirect a login si no permitido |
| backing sin recompensa works | Integration | Backing con rewardId=undefined |

**Casos Detallados:**

```typescript
describe('Anonymous Backing Flow (Integration)', () => {
    it('allows anonymous backing if campania permits', async () => {
        const user = userEvent.setup();

        // Mock unauthenticated user
        vi.mocked(useAuth).mockReturnValue({
            isAuthenticated: false,
            user: null
        });

        // Mock campania que permite anonimos
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaActive);

        renderWithProviders(<CampaniaDetailPage />);

        await waitFor(() => {
            expect(screen.getByText('Mi Album Debut')).toBeInTheDocument();
        });

        // Click "Apoyar sin recompensa"
        const apoyarButton = screen.getByRole('button', { name: /Apoyar sin recompensa/i });
        await user.click(apoyarButton);

        // Modal opened (no redirect a login)
        await waitFor(() => {
            expect(screen.getByRole('dialog')).toBeInTheDocument();
        });

        // Fill amount (no reward selected)
        const amountInput = screen.getByLabelText(/Monto/i);
        await user.clear(amountInput);
        await user.type(amountInput, '15');

        // Check "anonimo"
        const anonimoCheckbox = screen.getByLabelText(/anonimo/i);
        await user.click(anonimoCheckbox);

        // Submit
        const confirmButton = screen.getByRole('button', { name: /Confirmar apoyo/i });
        await user.click(confirmButton);

        // Success
        await waitFor(() => {
            expect(screen.getByText(/Gracias por tu apoyo/i)).toBeInTheDocument();
        });
    });

    it('blocks anonymous backing if not permitted', async () => {
        const user = userEvent.setup();
        const mockNavigate = vi.fn();

        vi.mocked(useAuth).mockReturnValue({
            isAuthenticated: false,
            user: null
        });
        vi.mocked(useNavigate).mockReturnValue(mockNavigate);

        // Mock campania que NO permite anonimos
        vi.mocked(campaniaService.getById).mockResolvedValue(mockCampaniaNoAnonimos);

        renderWithProviders(<CampaniaDetailPage />);

        await waitFor(() => {
            expect(screen.getByText('Mi Album Debut')).toBeInTheDocument();
        });

        // Click "Apoyar"
        const apoyarButton = screen.getByRole('button', { name: /Apoyar/i });
        await user.click(apoyarButton);

        // Redirect to login
        expect(mockNavigate).toHaveBeenCalledWith('/auth/login', {
            state: { returnUrl: `/campanias/${mockCampaniaActive.id}` }
        });
    });
});
```

---

## 5. MSW Setup

### 5.1 Test Setup con MSW

**Archivo:** `vitest.setup.ts` (modificar existing)

```typescript
import "@testing-library/jest-dom/vitest"
import { setupServer } from 'msw/node';
import { campaniaHandlers } from './src/features/hacer-backing/__mocks__/handlers';

export const server = setupServer(...campaniaHandlers);

// Start server before all tests
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));

// Reset handlers after each test
afterEach(() => server.resetHandlers());

// Clean up after all tests
afterAll(() => server.close());
```

### 5.2 Override Handlers en Tests

```typescript
// En un test especifico
it('handles API error', async () => {
    server.use(
        http.get(`${API_BASE}/campanias/:id`, () => {
            return HttpResponse.json(
                { messages: [{ errorCode: '5000' }] },
                { status: 500 }
            );
        })
    );

    // Test logic...
});
```

---

## 6. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches | Notas |
|---------|--------|-----------|----------|-------|
| CampaniaCard.tsx | 85% | 90% | 80% | Component simple, alta cobertura |
| CampaniaDetailPage.tsx | 75% | 80% | 70% | Muchos branches (loading, error, empty) |
| BackingModal.tsx | 90% | 95% | 85% | Formulario critico, maxima cobertura |
| RewardPublicCard.tsx | 85% | 90% | 80% | Logica de stock, branches importantes |
| CampaniaProgressBar.tsx | 90% | 100% | 85% | Logica visual, facil testear |
| AmountInput.tsx | 95% | 100% | 90% | Input validado, alta cobertura |
| BackingConfirmationPage.tsx | 80% | 85% | 75% | Pagina simple, cobertura media |
| BackingsRecentesList.tsx | 85% | 90% | 80% | Lista simple, cobertura buena |
| useCampanias.ts | 90% | 100% | 85% | Hook TanStack Query, bien testeado |
| useCampaniaDetail.ts | 90% | 100% | 85% | Hook critico, maxima cobertura |
| useCreateBacking.ts | 95% | 100% | 90% | Mutation critica, maxima cobertura |
| backing.schema.ts | 95% | 100% | 90% | Validaciones, facil testear |

**Meta Global:** 80%+ en todas las metricas

---

## 7. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar tests de feature especifica
npm run test -- src/features/hacer-backing

# Watch mode
npm run test:watch

# UI mode (Vitest UI)
npx vitest --ui
```

---

## 8. CI/CD Integration

**Archivo:** `.github/workflows/frontend-tests.yml`

```yaml
name: Frontend Tests

on:
  push:
    branches: [master]
    paths:
      - 'src/web/**'
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

      - run: npm run test -- --coverage

      - name: Upload Coverage
        uses: codecov/codecov-action@v4
        with:
          files: ./src/web/coverage/coverage-final.json
          flags: frontend-landing
          fail_ci_if_error: true
```

---

## 9. Criterios de Aceptacion a Validar

| AC-ID | Criterio | Test Que Lo Valida |
|-------|----------|-------------------|
| AC-04-2 | Monto >= ImporteMinimo del reward | `backing.schema.test.ts` - validateBackingAmount |
| AC-04-5 | Reward con stock 0 no seleccionable | `RewardPublicCard.test.tsx` - disables select button if stock=0 |
| AC-04-6 | Backers anonimos muestran "Anonimo" | `BackingsRecentesList.test.tsx` - shows "Anonimo" for anonymous backers |
| AC-04-7 | "Apoyar sin recompensa" permite monto > 0 | `backing-flow.test.tsx` - backing sin recompensa works |

---

## 10. Edge Cases y Errores

### 10.1 Validaciones

| Caso | Test | Archivo |
|------|------|---------|
| Monto negativo | validates monto >= 1 | backing.schema.test.ts |
| Monto excesivo | validates monto <= 100000 | backing.schema.test.ts |
| Mensaje muy largo | validates mensaje max 500 chars | backing.schema.test.ts |
| RewardId invalido (no UUID) | validates rewardId is UUID | backing.schema.test.ts |
| Monto < reward.importeMinimo | shows validation error if amount < min | BackingModal.test.tsx |

### 10.2 Estados de Campania

| Caso | Test | Archivo |
|------|------|---------|
| Campania finalizada | disables "Apoyar" when finalizada | CampaniaDetailPage.test.tsx |
| Campania sin rewards | shows empty state when no rewards | CampaniaDetailPage.test.tsx |
| Campania no permite anonimos | blocks anonymous backing if not permitted | anonymous-backing.test.tsx |

### 10.3 Errores de API

| Caso | Test | Archivo |
|------|------|---------|
| 404 Campania no encontrada | handles not found error | useCampaniaDetail.test.ts |
| 500 Error interno | shows error toast on error | useCreateBacking.test.ts |
| 400 Validacion fallida | handles validation errors | useCreateBacking.test.ts |
| 409 Reward agotado | handles business rule errors | useCreateBacking.test.ts |

---

## 11. Mocking Strategies

### 11.1 Service Mocks

```typescript
// Mock service completo
vi.mock('@/services/campania.service', () => ({
    campaniaService: {
        getAll: vi.fn(),
        getById: vi.fn()
    }
}));

// Import despues del mock
import { campaniaService } from '@/services/campania.service';
```

### 11.2 Hook Mocks

```typescript
// Mock useAuth hook
vi.mock('@/hooks/useAuth', () => ({
    useAuth: vi.fn(() => ({
        isAuthenticated: false,
        user: null
    }))
}));
```

### 11.3 Router Mocks

```typescript
// Mock useNavigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
        useParams: () => ({ id: '123' })
    };
});
```

---

## 12. Testing Best Practices

### 12.1 Patron AAA (Arrange-Act-Assert)

```typescript
it('renders with basic data', () => {
    // Arrange - Setup
    const mockData = { ... };

    // Act - Execute
    renderWithProviders(<Component data={mockData} />);

    // Assert - Verify
    expect(screen.getByText('Expected')).toBeInTheDocument();
});
```

### 12.2 Query Priorities (Testing Library)

```typescript
// PREFERIR - Queries accesibles
screen.getByRole('button', { name: /Apoyar/i })
screen.getByLabelText(/Monto/i)
screen.getByText('Mi Album')

// EVITAR - Queries fragiles
screen.getByTestId('backing-button')
screen.getByClassName('backing-form')
```

### 12.3 User Events vs FireEvent

```typescript
// PREFERIR - userEvent (mas realista)
const user = userEvent.setup();
await user.click(button);
await user.type(input, 'text');

// EVITAR - fireEvent (menos realista)
fireEvent.click(button);
fireEvent.change(input, { target: { value: 'text' } });
```

### 12.4 Async Queries

```typescript
// CORRECTO - waitFor para async
await waitFor(() => {
    expect(screen.getByText('Loaded')).toBeInTheDocument();
});

// CORRECTO - findBy queries (built-in waitFor)
const element = await screen.findByText('Loaded');

// INCORRECTO - Queries sincronas en async code
expect(screen.getByText('Loaded')).toBeInTheDocument(); // Falla
```

---

## 13. Checklist de Implementacion

### Setup
- [ ] MSW handlers definidos en `__mocks__/handlers.ts`
- [ ] Mock data completo en `__mocks__/campania.mock.ts` y `backing.mock.ts`
- [ ] Test utilities configurados en `test-utils.tsx`
- [ ] MSW setup en `vitest.setup.ts`

### Component Tests
- [ ] CampaniaCard.test.tsx (7 test cases)
- [ ] CampaniaDetailPage.test.tsx (10 test cases)
- [ ] BackingModal.test.tsx (12 test cases)
- [ ] RewardPublicCard.test.tsx (8 test cases)
- [ ] CampaniaProgressBar.test.tsx (6 test cases)
- [ ] AmountInput.test.tsx (7 test cases)
- [ ] BackingConfirmationPage.test.tsx (6 test cases)
- [ ] BackingsRecentesList.test.tsx (6 test cases)

### Hook Tests
- [ ] useCampanias.test.ts (7 test cases)
- [ ] useCampaniaDetail.test.ts (6 test cases)
- [ ] useCreateBacking.test.ts (7 test cases)

### Schema Tests
- [ ] backing.schema.test.ts (11 test cases)

### Integration Tests
- [ ] backing-flow.test.tsx (3 test cases)
- [ ] anonymous-backing.test.tsx (3 test cases)

### Coverage
- [ ] Cobertura 80%+ en todas las metricas
- [ ] CI/CD pipeline configurado
- [ ] Comandos de ejecucion documentados

---

## 14. Proximos Pasos Post-Tests

Despues de implementar estos tests:

1. **Ejecutar cobertura:** `npm run test -- --coverage`
2. **Revisar reporte:** Abrir `coverage/index.html`
3. **Identificar gaps:** Buscar lineas/branches no cubiertos
4. **Agregar tests faltantes:** Cubrir casos edge detectados
5. **Integrar en CI:** Configurar GitHub Actions workflow
6. **Code review:** Revisar tests con el equipo
7. **Documentar learnings:** Actualizar este documento con insights

---

**Fin de la estrategia de testing para hacer-backing (Landing).**
