# Estrategia de Testing: Definir Recompensas (Landing)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/web (Landing publica)
**Cobertura Objetivo:** 80%

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Component Tests | 2 | 85% |
| Hook Tests | 1 | 90% |
| Schema Tests | 3 | 95% |
| Total | 6 | 85%+ |

**Nota:** El alcance de Landing es SOLO vista publica de recompensas (componentes de solo lectura). No incluye formularios CRUD ni drag & drop (eso es Admin).

## 2. Estructura de Tests

```
src/web/
├── src/
│   ├── features/
│   │   └── rewards/
│   │       ├── __tests__/
│   │       │   ├── components/
│   │       │   │   ├── RewardPublicCard.test.tsx
│   │       │   │   └── CampaniaRewardsSection.test.tsx
│   │       │   └── hooks/
│   │       │       └── useRewardsByCampania.test.ts
│   │       ├── components/
│   │       │   ├── RewardPublicCard.tsx
│   │       │   └── CampaniaRewardsSection.tsx
│   │       └── hooks/
│   │           └── useRewardsByCampania.ts
│   └── shared/
│       └── schemas/
│           └── __tests__/
│               └── reward.schema.test.ts
└── vitest.setup.ts
```

**Nota sobre ubicacion:** Tests en carpetas `__tests__/` dentro de features, organizados por tipo (components/hooks).

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/web/src/features/rewards/__mocks__/reward.mock.ts`

```typescript
import { Reward } from '@shared/types/reward';

export const mockRewardDigital: Reward = {
  id: '123e4567-e89b-12d3-a456-426614174000',
  campaniaId: '550e8400-e29b-41d4-a716-446655440000',
  tipoRewardId: 1, // Digital
  nombre: 'Descarga Digital',
  descripcion: 'Acceso anticipado al album completo en formato digital FLAC + MP3',
  importeMinimo: 10.00,
  monedaId: 1,
  esAddOn: false,
  cantidadMaxima: null, // Ilimitado
  cantidadPorBacker: 1,
  incluyeEnvioFisico: false,
  tiempoEntregaEstimado: 'Inmediato tras finalizar campania',
  orden: 1,
  esActivo: true,
  fechaCreacion: '2026-02-10T10:00:00Z',
  fechaActualizacion: null,
};

export const mockRewardFisico: Reward = {
  id: '223e4567-e89b-12d3-a456-426614174001',
  campaniaId: '550e8400-e29b-41d4-a716-446655440000',
  tipoRewardId: 2, // Fisico
  nombre: 'CD Fisico Firmado',
  descripcion: 'CD fisico del album con firma del artista + booklet dedicado',
  importeMinimo: 25.00,
  monedaId: 1,
  esAddOn: false,
  cantidadMaxima: 200,
  cantidadPorBacker: 1,
  incluyeEnvioFisico: true,
  tiempoEntregaEstimado: 'Marzo 2026',
  orden: 2,
  esActivo: true,
  fechaCreacion: '2026-02-10T10:00:00Z',
  fechaActualizacion: null,
};

export const mockRewardLimitedStock: Reward = {
  id: '323e4567-e89b-12d3-a456-426614174002',
  campaniaId: '550e8400-e29b-41d4-a716-446655440000',
  tipoRewardId: 2,
  nombre: 'Vinilo Edicion Limitada',
  descripcion: 'Vinilo en color especial + poster exclusivo + todo lo anterior',
  importeMinimo: 50.00,
  monedaId: 1,
  esAddOn: false,
  cantidadMaxima: 100,
  cantidadPorBacker: 1,
  incluyeEnvioFisico: true,
  tiempoEntregaEstimado: 'Marzo 2026',
  orden: 3,
  esActivo: true,
  fechaCreacion: '2026-02-10T10:00:00Z',
  fechaActualizacion: null,
};

export const mockRewardSoldOut: Reward = {
  id: '423e4567-e89b-12d3-a456-426614174003',
  campaniaId: '550e8400-e29b-41d4-a716-446655440000',
  tipoRewardId: 3, // Experiencia
  nombre: 'Paquete VIP',
  descripcion: 'Meet & greet privado + vinilo + mercancia exclusiva',
  importeMinimo: 100.00,
  monedaId: 1,
  esAddOn: false,
  cantidadMaxima: 50,
  cantidadPorBacker: 1,
  incluyeEnvioFisico: false,
  tiempoEntregaEstimado: 'Abril 2026',
  orden: 4,
  esActivo: true,
  fechaCreacion: '2026-02-10T10:00:00Z',
  fechaActualizacion: null,
};

export const mockRewardsList: Reward[] = [
  mockRewardDigital,
  mockRewardFisico,
  mockRewardLimitedStock,
  mockRewardSoldOut,
];

// Helper para calcular stock disponible (simular logica de backend)
export const getStockDisponible = (reward: Reward, cantidadVendida: number): number | null => {
  if (!reward.cantidadMaxima) return null;
  return Math.max(0, reward.cantidadMaxima - cantidadVendida);
};

// Mock de rewards con stock calculado
export const mockRewardsWithStock = [
  { ...mockRewardDigital, cantidadVendida: 234, cantidadDisponible: null }, // Ilimitado
  { ...mockRewardFisico, cantidadVendida: 111, cantidadDisponible: 89 }, // 200 - 111 = 89
  { ...mockRewardLimitedStock, cantidadVendida: 55, cantidadDisponible: 45 }, // 100 - 55 = 45 (< 50%)
  { ...mockRewardSoldOut, cantidadVendida: 50, cantidadDisponible: 0 }, // Agotado
];
```

### 3.2 MSW Handlers

**Archivo:** `src/web/src/mocks/handlers/reward.handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import { mockRewardsList } from '@/features/rewards/__mocks__/reward.mock';

export const rewardHandlers = [
  // GET /api/rewards?campaniaId={id}
  http.get('/api/rewards', ({ request }) => {
    const url = new URL(request.url);
    const campaniaId = url.searchParams.get('campaniaId');
    const esActivo = url.searchParams.get('esActivo');

    let rewards = [...mockRewardsList];

    // Filtrar por campaniaId si se proporciona
    if (campaniaId) {
      rewards = rewards.filter(r => r.campaniaId === campaniaId);
    }

    // Filtrar por esActivo si se proporciona
    if (esActivo !== null) {
      const isActive = esActivo === 'true';
      rewards = rewards.filter(r => r.esActivo === isActive);
    }

    // Ordenar por campo 'orden' ascendente (simular backend)
    rewards.sort((a, b) => a.orden - b.orden);

    return HttpResponse.json({
      data: rewards,
      messages: [
        {
          message: 'Rewards encontrados',
          errorCode: '0000',
        },
      ],
    });
  }),

  // GET /api/rewards/{id}
  http.get('/api/rewards/:id', ({ params }) => {
    const { id } = params;
    const reward = mockRewardsList.find(r => r.id === id);

    if (!reward) {
      return HttpResponse.json(
        {
          data: null,
          messages: [
            {
              message: 'Reward no encontrado',
              errorCode: '2004',
            },
          ],
        },
        { status: 404 }
      );
    }

    return HttpResponse.json({
      data: reward,
      messages: [
        {
          message: 'Reward encontrado',
          errorCode: '0000',
        },
      ],
    });
  }),

  // Error handler (opcional para tests de error)
  http.get('/api/rewards-error', () => {
    return HttpResponse.json(
      {
        data: null,
        messages: [
          {
            message: 'Error inesperado',
            errorCode: '5000',
          },
        ],
      },
      { status: 500 }
    );
  }),
];
```

### 3.3 Test Utilities

**Archivo:** `src/web/src/test-utils.tsx`

```typescript
import { ReactElement, ReactNode } from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';

// QueryClient para tests (sin retry, sin cache persistente)
const createTestQueryClient = () =>
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
  children: ReactNode;
}

const AllProviders = ({ children }: AllProvidersProps) => {
  const queryClient = createTestQueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>{children}</BrowserRouter>
    </QueryClientProvider>
  );
};

const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllProviders, ...options });

export * from '@testing-library/react';
export { customRender as render };
```

### 3.4 Vitest Setup

**Archivo:** `src/web/vitest.setup.ts`

```typescript
import { afterAll, afterEach, beforeAll } from 'vitest';
import { setupServer } from 'msw/node';
import { rewardHandlers } from '@/mocks/handlers/reward.handlers';
import '@testing-library/jest-dom/vitest';

// Setup MSW server con handlers
export const server = setupServer(...rewardHandlers);

// Start server antes de todos los tests
beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));

// Reset handlers despues de cada test
afterEach(() => server.resetHandlers());

// Cleanup server despues de todos los tests
afterAll(() => server.close());
```

## 4. Tests por Modulo

### 4.1 Components

#### RewardPublicCard.test.tsx

**Archivo:** `src/web/src/features/rewards/__tests__/components/RewardPublicCard.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders reward data correctly | Unit | Renderiza titulo, precio, descripcion |
| shows unlimited stock badge | Unit | Badge "Ilimitado" cuando cantidadMaxima es null |
| shows available stock badge | Unit | Badge con stock disponible cuando hay stock |
| shows "Pocas unidades" badge | Unit | Badge warning cuando stock < 50% de cantidadMaxima |
| shows "AGOTADO" badge | Unit | Badge gris cuando cantidadDisponible = 0 |
| shows "Mas popular" badge | Unit | Badge rosa cuando es la recompensa mas popular |
| select button enabled when stock available | Unit | Boton "Seleccionar" habilitado si cantidadDisponible > 0 o null |
| select button disabled when sold out | Unit | Boton disabled y texto "Agotado" cuando cantidadDisponible = 0 |
| card opacity reduced when sold out | Unit | Card con opacity-60 cuando agotado |
| click select navigates correctly | Integration | Click en "Seleccionar" navega a /campanias/{id}/backing?reward={rewardId} |
| displays shipping badge | Unit | Badge de envio fisico cuando incluyeEnvioFisico = true |

**Casos Detallados:**

```typescript
import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@/test-utils';
import { RewardPublicCard } from '@/features/rewards/components/RewardPublicCard';
import { mockRewardDigital, mockRewardSoldOut, mockRewardLimitedStock } from '@/features/rewards/__mocks__/reward.mock';
import userEvent from '@testing-library/user-event';

describe('RewardPublicCard', () => {
  it('renders reward data correctly', () => {
    render(
      <RewardPublicCard
        reward={mockRewardDigital}
        cantidadDisponible={null}
        cantidadVendida={234}
      />
    );

    expect(screen.getByText('Descarga Digital')).toBeInTheDocument();
    expect(screen.getByText('€ 10.00')).toBeInTheDocument();
    expect(screen.getByText(/Acceso anticipado al album/i)).toBeInTheDocument();
  });

  it('shows unlimited stock badge when cantidadMaxima is null', () => {
    render(
      <RewardPublicCard
        reward={mockRewardDigital}
        cantidadDisponible={null}
        cantidadVendida={234}
      />
    );

    expect(screen.getByText(/Ilimitadas disponibles/i)).toBeInTheDocument();
    expect(screen.getByLabelText('Infinity icon')).toBeInTheDocument();
  });

  it('shows available stock badge when stock > 0', () => {
    render(
      <RewardPublicCard
        reward={mockRewardFisico}
        cantidadDisponible={89}
        cantidadVendida={111}
      />
    );

    expect(screen.getByText('89 de 200 disponibles')).toBeInTheDocument();
  });

  it('shows "Pocas unidades" badge when stock < 50%', () => {
    render(
      <RewardPublicCard
        reward={mockRewardLimitedStock}
        cantidadDisponible={45}
        cantidadVendida={55}
      />
    );

    expect(screen.getByText('Pocas unidades')).toBeInTheDocument();
    expect(screen.getByText('45 de 100 disponibles')).toBeInTheDocument();
  });

  it('shows "AGOTADO" badge when cantidadDisponible = 0', () => {
    render(
      <RewardPublicCard
        reward={mockRewardSoldOut}
        cantidadDisponible={0}
        cantidadVendida={50}
      />
    );

    expect(screen.getByText('AGOTADO')).toBeInTheDocument();
    expect(screen.getByText('Agotado')).toBeInTheDocument(); // Badge
  });

  it('select button enabled when stock available', () => {
    render(
      <RewardPublicCard
        reward={mockRewardDigital}
        cantidadDisponible={null}
        cantidadVendida={234}
      />
    );

    const button = screen.getByRole('button', { name: /Seleccionar/i });
    expect(button).toBeEnabled();
  });

  it('select button disabled when sold out', () => {
    render(
      <RewardPublicCard
        reward={mockRewardSoldOut}
        cantidadDisponible={0}
        cantidadVendida={50}
      />
    );

    const button = screen.getByRole('button', { name: /Agotado/i });
    expect(button).toBeDisabled();
  });

  it('card opacity reduced when sold out', () => {
    const { container } = render(
      <RewardPublicCard
        reward={mockRewardSoldOut}
        cantidadDisponible={0}
        cantidadVendida={50}
      />
    );

    const card = container.querySelector('[data-testid="reward-card"]');
    expect(card).toHaveClass('opacity-60');
  });

  it('click select navigates correctly', async () => {
    const mockNavigate = vi.fn();
    vi.mock('react-router-dom', () => ({
      ...vi.importActual('react-router-dom'),
      useNavigate: () => mockNavigate,
    }));

    const user = userEvent.setup();

    render(
      <RewardPublicCard
        reward={mockRewardDigital}
        cantidadDisponible={null}
        cantidadVendida={234}
      />
    );

    const button = screen.getByRole('button', { name: /Seleccionar/i });
    await user.click(button);

    expect(mockNavigate).toHaveBeenCalledWith(
      `/campanias/${mockRewardDigital.campaniaId}/backing?reward=${mockRewardDigital.id}`
    );
  });

  it('displays shipping badge when incluyeEnvioFisico = true', () => {
    render(
      <RewardPublicCard
        reward={mockRewardFisico}
        cantidadDisponible={89}
        cantidadVendida={111}
      />
    );

    expect(screen.getByLabelText('Incluye envio fisico')).toBeInTheDocument();
  });
});
```

#### CampaniaRewardsSection.test.tsx

**Archivo:** `src/web/src/features/rewards/__tests__/components/CampaniaRewardsSection.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders list of rewards | Integration | Renderiza todas las rewards de una campania |
| rewards ordered by precio ascendente | Unit | Lista ordenada por importeMinimo (campo 'orden') |
| shows empty state when no rewards | Unit | Mensaje "Esta campania no tiene recompensas" |
| shows skeleton loaders while loading | Unit | 3 skeleton cards durante isLoading |
| shows error message on fetch error | Integration | Mensaje de error cuando API falla |
| filters out inactive rewards | Integration | Solo muestra rewards con esActivo = true |
| calculates stock disponible correctly | Unit | cantidadDisponible = cantidadMaxima - cantidadVendida |

**Casos Detallados:**

```typescript
import { describe, it, expect } from 'vitest';
import { render, screen, waitFor } from '@/test-utils';
import { CampaniaRewardsSection } from '@/features/rewards/components/CampaniaRewardsSection';
import { server } from '@/vitest.setup';
import { http, HttpResponse } from 'msw';

describe('CampaniaRewardsSection', () => {
  const campaniaId = '550e8400-e29b-41d4-a716-446655440000';

  it('renders list of rewards', async () => {
    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    await waitFor(() => {
      expect(screen.getByText('Descarga Digital')).toBeInTheDocument();
    });

    expect(screen.getByText('CD Fisico Firmado')).toBeInTheDocument();
    expect(screen.getByText('Vinilo Edicion Limitada')).toBeInTheDocument();
    expect(screen.getByText('Paquete VIP')).toBeInTheDocument();
  });

  it('rewards ordered by orden ascendente', async () => {
    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    await waitFor(() => {
      expect(screen.getByText('Descarga Digital')).toBeInTheDocument();
    });

    const rewards = screen.getAllByTestId('reward-card');
    expect(rewards).toHaveLength(4);

    // Verificar orden por precio
    expect(rewards[0]).toHaveTextContent('€ 10.00');
    expect(rewards[1]).toHaveTextContent('€ 25.00');
    expect(rewards[2]).toHaveTextContent('€ 50.00');
    expect(rewards[3]).toHaveTextContent('€ 100.00');
  });

  it('shows empty state when no rewards', async () => {
    server.use(
      http.get('/api/rewards', () => {
        return HttpResponse.json({
          data: [],
          messages: [{ message: 'No rewards', errorCode: '0000' }],
        });
      })
    );

    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    await waitFor(() => {
      expect(screen.getByText(/Esta campania no tiene recompensas/i)).toBeInTheDocument();
    });

    expect(screen.getByText(/Puedes hacer una contribucion libre/i)).toBeInTheDocument();
  });

  it('shows skeleton loaders while loading', () => {
    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    const skeletons = screen.getAllByTestId('reward-skeleton');
    expect(skeletons).toHaveLength(3);
  });

  it('shows error message on fetch error', async () => {
    server.use(
      http.get('/api/rewards', () => {
        return HttpResponse.json(
          {
            data: null,
            messages: [{ message: 'Error inesperado', errorCode: '5000' }],
          },
          { status: 500 }
        );
      })
    );

    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    await waitFor(() => {
      expect(screen.getByText(/Error al cargar recompensas/i)).toBeInTheDocument();
    });
  });

  it('filters out inactive rewards', async () => {
    const rewardsWithInactive = [
      { ...mockRewardDigital, esActivo: true },
      { ...mockRewardFisico, esActivo: false }, // Inactiva
    ];

    server.use(
      http.get('/api/rewards', () => {
        return HttpResponse.json({
          data: rewardsWithInactive.filter(r => r.esActivo),
          messages: [{ message: 'Success', errorCode: '0000' }],
        });
      })
    );

    render(<CampaniaRewardsSection campaniaId={campaniaId} />);

    await waitFor(() => {
      expect(screen.getByText('Descarga Digital')).toBeInTheDocument();
    });

    expect(screen.queryByText('CD Fisico Firmado')).not.toBeInTheDocument();
  });
});
```

### 4.2 Hooks

#### useRewardsByCampania.test.ts

**Archivo:** `src/web/src/features/rewards/__tests__/hooks/useRewardsByCampania.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches rewards by campaniaId | Integration | Hook retorna rewards correctamente |
| returns empty array when no rewards | Integration | Retorna [] cuando data es vacia |
| handles loading state | Unit | isLoading true inicialmente, false al completar |
| handles error state | Integration | error no null cuando API falla |
| filters by esActivo | Integration | Solo retorna rewards con esActivo = true |
| caches query with correct key | Unit | Usa QUERY_KEYS.rewards.byCampania(id) |

**Casos Detallados:**

```typescript
import { describe, it, expect } from 'vitest';
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useRewardsByCampania } from '@/features/rewards/hooks/useRewardsByCampania';
import { server } from '@/vitest.setup';
import { http, HttpResponse } from 'msw';

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });

  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

describe('useRewardsByCampania', () => {
  const campaniaId = '550e8400-e29b-41d4-a716-446655440000';

  it('fetches rewards by campaniaId', async () => {
    const { result } = renderHook(() => useRewardsByCampania(campaniaId), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toHaveLength(4);
    expect(result.current.data?.[0].nombre).toBe('Descarga Digital');
  });

  it('returns empty array when no rewards', async () => {
    server.use(
      http.get('/api/rewards', () => {
        return HttpResponse.json({
          data: [],
          messages: [{ message: 'No rewards', errorCode: '0000' }],
        });
      })
    );

    const { result } = renderHook(() => useRewardsByCampania(campaniaId), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toEqual([]);
  });

  it('handles loading state', async () => {
    const { result } = renderHook(() => useRewardsByCampania(campaniaId), {
      wrapper: createWrapper(),
    });

    expect(result.current.isLoading).toBe(true);

    await waitFor(() => expect(result.current.isLoading).toBe(false));
    expect(result.current.isSuccess).toBe(true);
  });

  it('handles error state', async () => {
    server.use(
      http.get('/api/rewards', () => {
        return HttpResponse.json(
          {
            data: null,
            messages: [{ message: 'Error', errorCode: '5000' }],
          },
          { status: 500 }
        );
      })
    );

    const { result } = renderHook(() => useRewardsByCampania(campaniaId), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isError).toBe(true));

    expect(result.current.error).not.toBeNull();
    expect(result.current.data).toBeUndefined();
  });

  it('filters by esActivo = true', async () => {
    const { result } = renderHook(() => useRewardsByCampania(campaniaId), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    // Todas las rewards del mock tienen esActivo = true
    result.current.data?.forEach(reward => {
      expect(reward.esActivo).toBe(true);
    });
  });

  it('caches query with correct key', async () => {
    const queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false } },
    });

    const wrapper = ({ children }: { children: React.ReactNode }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );

    renderHook(() => useRewardsByCampania(campaniaId), { wrapper });

    await waitFor(() => {
      const cache = queryClient.getQueryCache();
      const queries = cache.findAll({ queryKey: ['rewards', 'campania', campaniaId] });
      expect(queries).toHaveLength(1);
    });
  });
});
```

### 4.3 Schemas (Shared)

#### reward.schema.test.ts

**Archivo:** `src/shared/schemas/__tests__/reward.schema.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| createRewardSchema validates nombre required | Unit | Error cuando nombre vacio |
| createRewardSchema validates nombre maxLength | Unit | Error cuando nombre > 200 caracteres |
| createRewardSchema validates descripcion maxLength | Unit | Error cuando descripcion > 2000 caracteres |
| createRewardSchema validates importeMinimo positive | Unit | Error cuando importeMinimo <= 0 |
| createRewardSchema validates tipoRewardId positive | Unit | Error cuando tipoRewardId <= 0 |
| createRewardSchema validates cantidadMaxima positive | Unit | Error cuando cantidadMaxima <= 0 |
| createRewardSchema accepts valid data | Unit | Schema pasa con datos validos |
| updateRewardSchema all fields optional | Unit | Schema pasa con solo id |
| updateRewardSchema validates when fields provided | Unit | Valida campos cuando se proporcionan |
| reorderRewardsSchema validates rewardOrders array | Unit | Error cuando rewardOrders vacio |
| reorderRewardsSchema validates orden >= 0 | Unit | Error cuando orden < 0 |

**Casos Detallados:**

```typescript
import { describe, it, expect } from 'vitest';
import {
  createRewardSchema,
  updateRewardSchema,
  reorderRewardsSchema,
} from '@shared/schemas/reward.schema';

describe('createRewardSchema', () => {
  it('validates nombre required', () => {
    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: '',
      importeMinimo: 10,
      monedaId: 1,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toBe('El nombre es obligatorio');
    }
  });

  it('validates nombre maxLength 200', () => {
    const longNombre = 'a'.repeat(201);

    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: longNombre,
      importeMinimo: 10,
      monedaId: 1,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('200 caracteres');
    }
  });

  it('validates descripcion maxLength 2000', () => {
    const longDescripcion = 'a'.repeat(2001);

    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: 'Test',
      descripcion: longDescripcion,
      importeMinimo: 10,
      monedaId: 1,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('2000 caracteres');
    }
  });

  it('validates importeMinimo positive', () => {
    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: 'Test',
      importeMinimo: 0,
      monedaId: 1,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('mayor a 0');
    }
  });

  it('validates tipoRewardId positive', () => {
    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 0,
      nombre: 'Test',
      importeMinimo: 10,
      monedaId: 1,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('tipo de recompensa');
    }
  });

  it('validates cantidadMaxima positive when provided', () => {
    const result = createRewardSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: 'Test',
      importeMinimo: 10,
      monedaId: 1,
      cantidadMaxima: -5,
      orden: 0,
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('mayor a 0');
    }
  });

  it('accepts valid data', () => {
    const validData = {
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      tipoRewardId: 1,
      nombre: 'Descarga Digital',
      descripcion: 'Album completo en MP3',
      importeMinimo: 10,
      monedaId: 1,
      esAddOn: false,
      cantidadMaxima: 100,
      cantidadPorBacker: 1,
      incluyeEnvioFisico: false,
      tiempoEntregaEstimado: 'Inmediato',
      orden: 1,
    };

    const result = createRewardSchema.safeParse(validData);

    expect(result.success).toBe(true);
  });
});

describe('updateRewardSchema', () => {
  it('all fields optional except id', () => {
    const result = updateRewardSchema.safeParse({
      id: '550e8400-e29b-41d4-a716-446655440000',
    });

    expect(result.success).toBe(true);
  });

  it('validates fields when provided', () => {
    const result = updateRewardSchema.safeParse({
      id: '550e8400-e29b-41d4-a716-446655440000',
      nombre: '',
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('vacio');
    }
  });
});

describe('reorderRewardsSchema', () => {
  it('validates rewardOrders array not empty', () => {
    const result = reorderRewardsSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      rewardOrders: [],
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('al menos una');
    }
  });

  it('validates orden >= 0', () => {
    const result = reorderRewardsSchema.safeParse({
      campaniaId: '550e8400-e29b-41d4-a716-446655440000',
      rewardOrders: [
        {
          rewardId: '123e4567-e89b-12d3-a456-426614174000',
          orden: -1,
        },
      ],
    });

    expect(result.success).toBe(false);
    if (!result.success) {
      expect(result.error.issues[0].message).toContain('mayor o igual a 0');
    }
  });
});
```

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| RewardPublicCard.tsx | 90% | 95% | 85% |
| CampaniaRewardsSection.tsx | 85% | 90% | 80% |
| useRewardsByCampania.ts | 95% | 100% | 90% |
| reward.schema.ts | 100% | 100% | 95% |

**Meta Global:** 85%+ en todas las metricas

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests
cd src/web
npm run test

# Ejecutar con coverage
npm run test -- --coverage

# Ejecutar tests de feature especifica
npm run test -- --filter=rewards

# Watch mode
npm run test:watch

# UI mode (Vitest UI)
npm run test -- --ui
```

## 7. CI/CD Integration

```yaml
- name: Run Frontend Tests (Landing)
  working-directory: src/web
  run: |
    npm ci
    npm run test -- --coverage --reporter=json --reporter=html

- name: Upload Coverage to Codecov
  uses: codecov/codecov-action@v3
  with:
    files: src/web/coverage/coverage-final.json
    flags: landing-rewards
```

## 8. Dependencias de Testing

Todas las dependencias YA ESTAN instaladas en `src/web/package.json`:

| Dependencia | Version | Uso |
|-------------|---------|-----|
| `vitest` | ^4.0.18 | Test runner |
| `@testing-library/react` | ^16.3.2 | Testing components |
| `@testing-library/jest-dom` | ^6.9.1 | Custom matchers |
| `jsdom` | ^28.0.0 | DOM environment |
| `msw` | (instalar) | API mocking |

**Accion requerida:** Instalar MSW para mocking de API.

```bash
cd src/web
npm install -D msw@latest
```

## 9. Checklist

- [ ] Mocks definidos para API rewards (`reward.mock.ts`, `reward.handlers.ts`)
- [ ] MSW configurado en `vitest.setup.ts`
- [ ] Test utilities con QueryClient y Router (`test-utils.tsx`)
- [ ] Tests de RewardPublicCard (11 test cases)
- [ ] Tests de CampaniaRewardsSection (7 test cases)
- [ ] Tests de useRewardsByCampania (6 test cases)
- [ ] Tests de schemas Zod (11 test cases)
- [ ] Cobertura 85%+
- [ ] Tests pasan en CI
- [ ] MSW instalado como dependencia

## 10. Notas de Implementacion

### 10.1 Stock Disponible

El campo `cantidadDisponible` NO existe en el DTO de backend. Se calcula en el frontend:

```typescript
const cantidadDisponible = reward.cantidadMaxima
  ? Math.max(0, reward.cantidadMaxima - cantidadVendida)
  : null;
```

**Mock:** Incluir `cantidadVendida` en mocks para simular este calculo.

### 10.2 Badge "Pocas Unidades"

Se muestra cuando `cantidadDisponible < (cantidadMaxima * 0.5)`:

```typescript
const isLimitedStock = cantidadDisponible !== null &&
  reward.cantidadMaxima !== null &&
  cantidadDisponible < reward.cantidadMaxima * 0.5;
```

### 10.3 Badge "Mas Popular"

Se calcula comparando `cantidadVendida` entre todas las rewards de la campania. La reward con mayor `cantidadVendida` tiene el badge.

**Implementacion en componente padre** (CampaniaRewardsSection):

```typescript
const mostPopularId = rewards.reduce((maxId, r) =>
  r.cantidadVendida > rewards.find(x => x.id === maxId)?.cantidadVendida
    ? r.id
    : maxId,
  rewards[0]?.id
);
```

### 10.4 Ordenamiento

Las rewards se ordenan por el campo `orden` (ascendente) que viene del backend. El backend YA retorna ordenado, pero el frontend puede re-ordenar para asegurar consistencia.

### 10.5 Navegacion al Seleccionar

Al hacer click en "Seleccionar":
- Si autenticado: `/campanias/{campaniaId}/backing?reward={rewardId}`
- Si NO autenticado: `/login?returnUrl=/campanias/{campaniaId}/backing?reward={rewardId}`

**Test:** Mockear `useAuth()` para simular ambos casos.

### 10.6 Empty State

Cuando no hay rewards:
- Mostrar mensaje: "Esta campania no tiene recompensas especificas. Puedes hacer una contribucion libre."
- Boton: "Apoyar esta campania" (sin reward seleccionada)

### 10.7 Error Handling

En caso de error en fetch:
- Mostrar toast error con mensaje generico
- Componente muestra mensaje: "Error al cargar recompensas. Intenta nuevamente."
- Boton "Reintentar" que ejecuta `refetch()`

## 11. Exclusiones de Testing

**NO incluir en tests de Landing (son del Admin):**
- Formularios de creacion/edicion de rewards
- Drag & drop reordering
- Botones de editar/eliminar
- Validaciones de formularios (solo validar schemas Zod)

**Estos tests se haran en el plan de Admin.**

---

**Fin del plan de estrategia de testing frontend para Landing.**
