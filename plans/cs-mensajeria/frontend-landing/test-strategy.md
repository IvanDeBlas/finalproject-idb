# Estrategia de Testing: cs-mensajeria (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)
**Target:** src/web (Landing - Vite + React 18)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen

| Tipo | Cantidad | Cobertura Estimada |
|------|----------|--------------------|
| Unit Tests | 47 | 85% |
| Integration Tests | 19 | 78% |
| Total | 66 | 80%+ |

**Alcance:**

- Componentes: `ConversacionList`, `ConversacionRow`, `ChatView`, `MessageBubble`, `MessageInput`, `NavbarMensajesIcon`, `IniciarConversacionDialog`
- Hooks: `useConversaciones`, `useMensajes`, `useCreateConversacion`, `useEnviarMensaje`, `useMarcarLeidos`, `useNoLeidosCount`
- Services: `conversacion.service.ts`, `mensaje.service.ts`
- Schemas Zod: `createConversacionSchema`, `createMensajeSchema`

**Fuera de alcance:**

- Tests E2E del flujo completo (se realizan por separado)
- Componentes de Admin (la mensajeria es exclusiva de Landing)
- Comportamiento de WebSocket/SignalR (no aplica en MVP)
- Tests visuales de CSS/animaciones

---

## 2. Estructura de Tests

```
src/web/src/features/crowdsourcing/mensajeria/
├── __tests__/
│   ├── components/
│   │   ├── ConversacionList.test.tsx
│   │   ├── ConversacionRow.test.tsx
│   │   ├── ChatView.test.tsx
│   │   ├── MessageBubble.test.tsx
│   │   ├── MessageInput.test.tsx
│   │   ├── NavbarMensajesIcon.test.tsx
│   │   └── IniciarConversacionDialog.test.tsx
│   ├── hooks/
│   │   ├── useConversaciones.test.ts
│   │   ├── useMensajes.test.ts
│   │   ├── useCreateConversacion.test.ts
│   │   ├── useEnviarMensaje.test.ts
│   │   ├── useMarcarLeidos.test.ts
│   │   └── useNoLeidosCount.test.ts
│   └── schemas/
│       └── mensajeria.schema.test.ts
└── __mocks__/
    ├── mensajeria.mock.ts
    └── handlers.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/mensajeria.mock.ts`

```typescript
import type {
    ConversacionListItem,
    ConversacionListResponse,
    Mensaje,
    MensajeListResponse,
    CreateConversacionResult,
    MarcarLeidosResponse,
    NoLeidosCountResponse,
} from '@shared/types/crowdsourcing';

// --- Conversaciones ---

export const mockConversacionConNoLeidos: ConversacionListItem = {
    id: 'f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c',
    asunto: 'Consulta sobre la mezcla de pistas',
    nombreOtraParte: 'Studio Mix Pro',
    imagenOtraParte: 'https://storage.weplay.com/avatars/studio-mix-pro.jpg',
    contextoTipo: 'necesidad',
    contextoTitulo: 'Mezcla de pistas para EP',
    ultimoMensaje: 'Perfecto, te envio los stems manana por la manana...',
    fechaUltimoMensaje: '2026-03-02T15:30:00Z',
    mensajesNoLeidos: 2,
};

export const mockConversacionSinNoLeidos: ConversacionListItem = {
    id: 'a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6e',
    asunto: 'Portada del album',
    nombreOtraParte: 'Diseno Grafico Pro',
    imagenOtraParte: null,
    contextoTipo: 'necesidad',
    contextoTitulo: 'Portada del album debut',
    ultimoMensaje: 'He preparado 3 bocetos para que elijas...',
    fechaUltimoMensaje: '2026-03-02T10:00:00Z',
    mensajesNoLeidos: 0,
};

export const mockConversacionSobreAcuerdo: ConversacionListItem = {
    id: 'b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7f',
    asunto: 'Coordinacion sesion de grabacion',
    nombreOtraParte: 'Fotografo Madrid',
    imagenOtraParte: null,
    contextoTipo: 'acuerdo',
    contextoTitulo: 'Sesion de fotos promo',
    ultimoMensaje: 'Perfecto, confirmamos el sabado',
    fechaUltimoMensaje: '2026-03-01T20:00:00Z',
    mensajesNoLeidos: 0,
};

export const mockConversacionSinMensajes: ConversacionListItem = {
    id: 'c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e80',
    asunto: 'Primer contacto',
    nombreOtraParte: 'Productor Nuevo',
    imagenOtraParte: null,
    contextoTipo: 'necesidad',
    contextoTitulo: 'Produccion musical EP',
    ultimoMensaje: null,
    fechaUltimoMensaje: null,
    mensajesNoLeidos: 0,
};

export const mockConversacionListResponse: ConversacionListResponse = {
    items: [
        mockConversacionConNoLeidos,
        mockConversacionSinNoLeidos,
        mockConversacionSobreAcuerdo,
    ],
    totalCount: 3,
    totalNoLeidos: 2,
    page: 1,
    pageSize: 20,
};

export const mockConversacionListResponseEmpty: ConversacionListResponse = {
    items: [],
    totalCount: 0,
    totalNoLeidos: 0,
    page: 1,
    pageSize: 20,
};

export const mockCreateConversacionResult: CreateConversacionResult = {
    id: 'f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c',
    asunto: 'Consulta sobre la mezcla de pistas',
    nombreDestinatario: 'Studio Mix Pro',
    contextoTipo: 'necesidad',
    contextoTitulo: 'Mezcla de pistas para EP',
    fechaCreacion: '2026-03-01T10:00:00Z',
};

// --- Mensajes ---

export const mockMensajeAjeno: Mensaje = {
    id: 'a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d',
    contenido: 'Hola, me interesa tu propuesta. Podrias contarme mas sobre tu experiencia?',
    urlAdjunto: null,
    remitenteNombre: 'Los Rockeros',
    esPropio: false,
    leido: true,
    fechaCreacion: '2026-03-01T10:05:00Z',
};

export const mockMensajePropio: Mensaje = {
    id: 'b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e',
    contenido: 'Claro! He trabajado con bandas como...',
    urlAdjunto: 'https://drive.google.com/portfolio',
    remitenteNombre: 'Studio Mix Pro',
    esPropio: true,
    leido: true,
    fechaCreacion: '2026-03-01T10:15:00Z',
};

export const mockMensajePropioSinAdjunto: Mensaje = {
    id: 'c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f',
    contenido: 'Perfecto, te envio los stems manana',
    urlAdjunto: null,
    remitenteNombre: 'Studio Mix Pro',
    esPropio: true,
    leido: false,
    fechaCreacion: '2026-03-02T15:30:00Z',
};

export const mockMensajeListResponse: MensajeListResponse = {
    items: [mockMensajeAjeno, mockMensajePropio, mockMensajePropioSinAdjunto],
    totalCount: 3,
    page: 1,
    pageSize: 50,
};

export const mockMensajeListResponseEmpty: MensajeListResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 50,
};

export const mockMensajeListResponsePaginado: MensajeListResponse = {
    items: [mockMensajeAjeno, mockMensajePropio, mockMensajePropioSinAdjunto],
    totalCount: 75,
    page: 1,
    pageSize: 50,
};

export const mockMensajeNuevo: Mensaje = {
    id: 'd4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a',
    contenido: 'Nuevo mensaje enviado correctamente',
    urlAdjunto: null,
    remitenteNombre: 'Studio Mix Pro',
    esPropio: true,
    leido: false,
    fechaCreacion: '2026-03-02T16:00:00Z',
};

export const mockMarcarLeidosResponse: MarcarLeidosResponse = {
    mensajesMarcados: 2,
};

export const mockMarcarLeidosResponseCero: MarcarLeidosResponse = {
    mensajesMarcados: 0,
};

export const mockNoLeidosCountResponse: NoLeidosCountResponse = {
    totalNoLeidos: 5,
};

export const mockNoLeidosCountCero: NoLeidosCountResponse = {
    totalNoLeidos: 0,
};

// --- Factories ---

export const makeConversacion = (
    overrides: Partial<ConversacionListItem> = {}
): ConversacionListItem => ({
    ...mockConversacionConNoLeidos,
    id: `conv-${Math.random().toString(36).substring(2, 9)}`,
    ...overrides,
});

export const makeMensaje = (overrides: Partial<Mensaje> = {}): Mensaje => ({
    ...mockMensajeAjeno,
    id: `msg-${Math.random().toString(36).substring(2, 9)}`,
    fechaCreacion: new Date().toISOString(),
    ...overrides,
});

export const makeConversacionList = (count: number): ConversacionListItem[] =>
    Array.from({ length: count }, (_, i) =>
        makeConversacion({
            id: `conv-test-${i}`,
            nombreOtraParte: `Profesional ${i + 1}`,
            mensajesNoLeidos: i === 0 ? 2 : 0,
        })
    );
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { http, HttpResponse } from 'msw';
import {
    mockConversacionListResponse,
    mockConversacionListResponseEmpty,
    mockMensajeListResponse,
    mockMensajeNuevo,
    mockMarcarLeidosResponse,
    mockNoLeidosCountResponse,
    mockCreateConversacionResult,
} from './mensajeria.mock';

const CONVERSACION_ID = 'f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c';

export const mensajeriaHandlers = [
    // GET /api/crowdsourcing/conversaciones
    http.get('/api/crowdsourcing/conversaciones', ({ request }) => {
        const url = new URL(request.url);
        const contexto = url.searchParams.get('contexto') ?? 'todas';

        if (contexto === 'acuerdos') {
            return HttpResponse.json({
                data: {
                    ...mockConversacionListResponse,
                    items: mockConversacionListResponse.items.filter(
                        (c) => c.contextoTipo === 'acuerdo'
                    ),
                },
                messages: [],
            });
        }

        return HttpResponse.json({
            data: mockConversacionListResponse,
            messages: [],
        });
    }),

    // POST /api/crowdsourcing/conversaciones
    http.post('/api/crowdsourcing/conversaciones', async ({ request }) => {
        const body = await request.json() as Record<string, unknown>;

        if (!body.asunto || (body.asunto as string).trim() === '') {
            return HttpResponse.json(
                {
                    data: null,
                    messages: [{ message: 'El asunto es obligatorio', errorCode: '1001' }],
                },
                { status: 400 }
            );
        }

        return HttpResponse.json(
            {
                data: mockCreateConversacionResult,
                messages: [{ message: 'Conversacion creada', errorCode: '0001' }],
            },
            { status: 201 }
        );
    }),

    // GET /api/crowdsourcing/conversaciones/:id/mensajes
    http.get('/api/crowdsourcing/conversaciones/:id/mensajes', ({ params }) => {
        if (params.id === 'forbidden-id') {
            return HttpResponse.json(
                {
                    data: null,
                    messages: [{ message: 'No tienes acceso', errorCode: '3002' }],
                },
                { status: 403 }
            );
        }

        return HttpResponse.json({
            data: mockMensajeListResponse,
            messages: [],
        });
    }),

    // POST /api/crowdsourcing/conversaciones/:id/mensajes
    http.post('/api/crowdsourcing/conversaciones/:id/mensajes', async ({ request }) => {
        const body = await request.json() as Record<string, unknown>;

        if (!body.contenido || (body.contenido as string).trim() === '') {
            return HttpResponse.json(
                {
                    data: null,
                    messages: [{ message: 'El contenido del mensaje es obligatorio', errorCode: '1001' }],
                },
                { status: 400 }
            );
        }

        return HttpResponse.json(
            {
                data: { ...mockMensajeNuevo, contenido: body.contenido as string },
                messages: [{ message: 'Mensaje enviado', errorCode: '0001' }],
            },
            { status: 201 }
        );
    }),

    // PATCH /api/crowdsourcing/conversaciones/:id/marcar-leidos
    http.patch('/api/crowdsourcing/conversaciones/:id/marcar-leidos', () => {
        return HttpResponse.json({
            data: mockMarcarLeidosResponse,
            messages: [],
        });
    }),

    // GET /api/crowdsourcing/conversaciones/no-leidos
    http.get('/api/crowdsourcing/conversaciones/no-leidos', () => {
        return HttpResponse.json({
            data: mockNoLeidosCountResponse,
            messages: [],
        });
    }),
];

// Handlers de error para sobreescribir en tests especificos
export const errorHandlers = {
    conversacionDuplicada: http.post(
        '/api/crowdsourcing/conversaciones',
        () =>
            HttpResponse.json(
                {
                    data: null,
                    messages: [
                        {
                            message: 'Ya existe una conversacion para este contexto',
                            errorCode: '4015',
                        },
                    ],
                },
                { status: 400 }
            )
    ),

    sinRelacion: http.post(
        '/api/crowdsourcing/conversaciones',
        () =>
            HttpResponse.json(
                {
                    data: null,
                    messages: [
                        {
                            message: 'No tienes relacion con este destinatario',
                            errorCode: '3002',
                        },
                    ],
                },
                { status: 403 }
            )
    ),

    mensajesError: http.get(
        '/api/crowdsourcing/conversaciones/:id/mensajes',
        () => HttpResponse.error()
    ),

    noLeidosError: http.get(
        '/api/crowdsourcing/conversaciones/no-leidos',
        () => HttpResponse.error()
    ),
};
```

### 3.3 Test Utilities

**Archivo:** `src/web/src/test-utils/mensajeria.tsx`

```typescript
import React from 'react';
import { render, RenderOptions } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter, MemoryRouterProps } from 'react-router-dom';

// QueryClient con configuracion para tests (sin reintentos, sin cache)
export const createTestQueryClient = () =>
    new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    });

interface TestProviderProps {
    queryClient?: QueryClient;
    routerProps?: MemoryRouterProps;
    children: React.ReactNode;
}

// Provider compuesto para tests de mensajeria
export const MensajeriaTestProvider: React.FC<TestProviderProps> = ({
    children,
    queryClient,
    routerProps = {},
}) => {
    const client = queryClient ?? createTestQueryClient();
    return (
        <QueryClientProvider client={client}>
            <MemoryRouter {...routerProps}>{children}</MemoryRouter>
        </QueryClientProvider>
    );
};

// Helper para render con providers
export const renderWithMensajeriaProviders = (
    ui: React.ReactElement,
    options?: Omit<RenderOptions, 'wrapper'> & {
        queryClient?: QueryClient;
        routerProps?: MemoryRouterProps;
    }
) => {
    const { queryClient, routerProps, ...renderOptions } = options ?? {};
    return render(ui, {
        wrapper: ({ children }) => (
            <MensajeriaTestProvider queryClient={queryClient} routerProps={routerProps}>
                {children}
            </MensajeriaTestProvider>
        ),
        ...renderOptions,
    });
};

// Wrapper para renderHook
export const createHookWrapper = (queryClient?: QueryClient) => {
    const client = queryClient ?? createTestQueryClient();
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client },
            React.createElement(MemoryRouter, null, children)
        );
    };
};
```

### 3.4 Configuracion MSW

El proyecto ya cuenta con `vitest.setup.ts`. Los tests de servicio que requieran MSW deben configurar `server` con `beforeAll / afterEach / afterAll`:

```typescript
import { setupServer } from 'msw/node';
import { mensajeriaHandlers } from '../../__mocks__/handlers';

const server = setupServer(...mensajeriaHandlers);

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

---

## 4. Tests por Modulo

### 4.1 Schemas Zod

#### mensajeria.schema.test.ts

**Archivo:** `__tests__/schemas/mensajeria.schema.test.ts`

**Tipo:** Unit - Funciones puras de validacion

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | `createConversacionSchema` acepta asunto valido con destinatario | Unit | FA-07 |
| 2 | `createConversacionSchema` rechaza asunto vacio | Unit | FA-07 |
| 3 | `createConversacionSchema` rechaza asunto con solo espacios | Unit | FA-07 |
| 4 | `createConversacionSchema` rechaza asunto mayor a 200 caracteres | Unit | AC-CS05-1 |
| 5 | `createConversacionSchema` rechaza sin `userIdDestinatario` | Unit | - |
| 6 | `createConversacionSchema` acepta `necesidadId` UUID valido | Unit | - |
| 7 | `createConversacionSchema` rechaza `necesidadId` no UUID | Unit | - |
| 8 | `createConversacionSchema` acepta sin `necesidadId` ni `acuerdoId` | Unit | - |
| 9 | `createMensajeSchema` acepta contenido de 1 caracter | Unit | FA-06 |
| 10 | `createMensajeSchema` rechaza contenido vacio | Unit | FA-06 |
| 11 | `createMensajeSchema` rechaza contenido mayor a 5000 caracteres | Unit | AC-CS05-11 |
| 12 | `createMensajeSchema` acepta sin `urlAdjunto` | Unit | AC-CS05-10 |
| 13 | `createMensajeSchema` acepta `urlAdjunto` vacia como string | Unit | AC-CS05-10 |
| 14 | `createMensajeSchema` acepta `urlAdjunto` con URL valida https:// | Unit | AC-CS05-10 |
| 15 | `createMensajeSchema` rechaza `urlAdjunto` sin protocolo | Unit | FA-04 |
| 16 | `createMensajeSchema` rechaza `urlAdjunto` con texto libre | Unit | FA-04 |

**Casos detallados:**

```markdown
1. schema acepta datos minimos validos
   - Input: { asunto: 'Consulta', userIdDestinatario: 'user-abc' }
   - Assert: schema.safeParse().success === true

4. schema rechaza asunto demasiado largo
   - Input: { asunto: 'A'.repeat(201), userIdDestinatario: 'user-abc' }
   - Assert: success === false, error.issues[0].path[0] === 'asunto'
   - Assert: mensaje de error contiene 'El asunto no puede superar'

14. schema acepta URL adjunta valida
   - Input: { contenido: 'Mensaje', urlAdjunto: 'https://drive.google.com/file' }
   - Assert: safeParse().success === true

15. schema rechaza URL sin protocolo
   - Input: { contenido: 'Mensaje', urlAdjunto: 'drive.google.com/file' }
   - Assert: success === false, error.issues[0].path[0] === 'urlAdjunto'
```

---

### 4.2 Components

#### ConversacionList.test.tsx

**Archivo:** `__tests__/components/ConversacionList.test.tsx`

**Tipo:** Integration - Componente que usa `useConversaciones` con filtros y paginacion

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Muestra skeleton durante carga inicial | Unit | - |
| 2 | Muestra lista de conversaciones al cargar con exito | Integration | AC-CS05-7 |
| 3 | Muestra badge total de no leidos en el encabezado | Integration | AC-CS05-7 |
| 4 | Oculta badge de no leidos cuando `totalNoLeidos === 0` | Integration | AC-CS05-9 |
| 5 | Muestra empty state cuando no hay conversaciones | Integration | FA-03 |
| 6 | Muestra alert de error cuando la API falla | Integration | - |
| 7 | Renderiza tab "Todas" activo por defecto | Unit | AC-CS05-8 |
| 8 | Filtra por "Necesidades" al cambiar de tab | Integration | AC-CS05-8 |
| 9 | Filtra por "Acuerdos" al cambiar de tab | Integration | AC-CS05-8 |
| 10 | Muestra boton "Cargar mas" si hay mas paginas | Integration | AC-CS05-12 |
| 11 | Oculta boton "Cargar mas" cuando no hay mas paginas | Integration | AC-CS05-12 |
| 12 | Navega a la vista de chat al hacer click en una fila | Integration | - |
| 13 | Resetea a pagina 1 al cambiar el filtro de contexto | Integration | AC-CS05-8 |

**Casos detallados:**

```markdown
1. Muestra skeleton durante carga inicial
   - Mock: `conversacionService.getConversaciones` que retorna Promise pendiente
   - Render: <ConversacionList filtro="todas" onFiltroChange={vi.fn()} />
   - Assert: screen.getAllByTestId('conversacion-skeleton') tiene length > 0
   - Assert: ninguna fila de conversacion visible

2. Muestra lista de conversaciones al cargar con exito
   - Mock: servicio retorna mockConversacionListResponse
   - Await: waitFor(() => isSuccess)
   - Assert: screen.getByText('Studio Mix Pro') visible
   - Assert: screen.getByText('Diseno Grafico Pro') visible
   - Assert: las 3 conversaciones del mock visibles

5. Empty state cuando no hay conversaciones
   - Mock: servicio retorna mockConversacionListResponseEmpty
   - Await: waitFor(() => isSuccess)
   - Assert: screen.getByText('No tienes conversaciones activas') visible
   - Assert: screen.getByRole('img', { name: /mensajes/i }) o icono presente

8. Cambia filtro a Necesidades
   - Render con filtro="todas"
   - userEvent.click(screen.getByRole('tab', { name: 'Necesidades' }))
   - Assert: onFiltroChange fue llamado con 'necesidades'
   - Assert: la query se re-ejecuta con contexto=necesidades

12. Navegar al chat al hacer click
   - Mock navigate
   - userEvent.click(fila de conversacion)
   - Assert: navigate('/crowdsourcing/mensajes/f2a3b4c5-...')
```

---

#### ConversacionRow.test.tsx

**Archivo:** `__tests__/components/ConversacionRow.test.tsx`

**Tipo:** Unit - Componente de presentacion pura

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Renderiza nombre de la otra parte | Unit | AC-CS05-7 |
| 2 | Renderiza el asunto de la conversacion | Unit | AC-CS05-7 |
| 3 | Renderiza titulo del contexto | Unit | AC-CS05-7 |
| 4 | Muestra preview del ultimo mensaje cuando existe | Unit | AC-CS05-7 |
| 5 | Oculta preview cuando `ultimoMensaje` es null | Unit | - |
| 6 | Muestra badge numerico cuando `mensajesNoLeidos > 0` | Unit | AC-CS05-7 |
| 7 | Oculta badge cuando `mensajesNoLeidos === 0` | Unit | AC-CS05-7 |
| 8 | Fondo diferenciado cuando hay mensajes no leidos | Unit | AC-CS05-7 |
| 9 | Muestra avatar con imagen cuando `imagenOtraParte` existe | Unit | - |
| 10 | Muestra iniciales como fallback cuando no hay imagen | Unit | - |
| 11 | Muestra timestamp formateado cuando hay `fechaUltimoMensaje` | Unit | - |
| 12 | Llama a `onClick` al hacer click en la fila | Unit | - |
| 13 | Badge de no leidos tiene `aria-label` accesible | Unit | Accesibilidad |
| 14 | Badge muestra "99+" cuando `mensajesNoLeidos > 99` | Unit | - |

**Casos detallados:**

```markdown
6. Badge numerico visible con mensajes no leidos
   - Render: <ConversacionRow conversacion={mockConversacionConNoLeidos} onClick={vi.fn()} />
   - Assert: screen.getByLabelText('2 mensajes no leidos') visible
   - Assert: badge contiene el numero '2'

10. Iniciales como fallback de avatar
   - Render: <ConversacionRow conversacion={mockConversacionSinNoLeidos} onClick={vi.fn()} />
   - (imagenOtraParte es null, nombreOtraParte = 'Diseno Grafico Pro')
   - Assert: screen.getByText('D') visible (primera inicial)
   - Assert: no hay elemento <img> con el avatar

13. Badge accesible
   - Assert: badge tiene atributo aria-label='2 mensajes no leidos'
   - (Segun spec: aria-label="{n} mensajes no leidos")

14. Badge muestra 99+
   - Render: <ConversacionRow conversacion={makeConversacion({ mensajesNoLeidos: 150 })} ... />
   - Assert: screen.getByText('99+') visible
```

---

#### MessageBubble.test.tsx

**Archivo:** `__tests__/components/MessageBubble.test.tsx`

**Tipo:** Unit - Componente de presentacion con logica de alineacion

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Renderiza contenido del mensaje | Unit | AC-CS05-5 |
| 2 | Mensaje propio alineado a la derecha (`flex-row-reverse`) | Unit | AC-CS05-5 |
| 3 | Mensaje ajeno alineado a la izquierda | Unit | AC-CS05-5 |
| 4 | Burbuja propia tiene clase de gradiente rosa-purpura | Unit | AC-CS05-5 |
| 5 | Burbuja ajena tiene fondo neutro | Unit | AC-CS05-5 |
| 6 | Muestra nombre del remitente para mensajes ajenos | Unit | AC-CS05-5 |
| 7 | Oculta nombre del remitente para mensajes propios | Unit | AC-CS05-5 |
| 8 | Renderiza enlace de `urlAdjunto` cuando existe | Unit | AC-CS05-10 |
| 9 | Enlace adjunto se abre en tab nueva (`target="_blank"`) | Unit | - |
| 10 | Enlace adjunto tiene `rel="noopener noreferrer"` | Unit | - |
| 11 | No renderiza seccion de adjunto cuando `urlAdjunto` es null | Unit | AC-CS05-10 |
| 12 | Muestra timestamp formateado | Unit | - |
| 13 | Renderiza avatar con alt text accesible | Unit | Accesibilidad |

**Casos detallados:**

```markdown
2. Mensaje propio alineado a la derecha
   - Render: <MessageBubble mensaje={mockMensajePropio} />
   - Assert: el contenedor del grupo de mensaje tiene clase 'flex-row-reverse'

8. Enlace adjunto renderizado correctamente
   - Render: <MessageBubble mensaje={mockMensajePropio} />
   - (urlAdjunto = 'https://drive.google.com/portfolio')
   - Assert: screen.getByRole('link', { name: /drive.google.com/ }) visible
   - Assert: href='https://drive.google.com/portfolio'

11. Sin adjunto no se muestra la seccion
   - Render: <MessageBubble mensaje={mockMensajeAjeno} />
   - (urlAdjunto = null)
   - Assert: screen.queryByRole('link') === null
```

---

#### MessageInput.test.tsx

**Archivo:** `__tests__/components/MessageInput.test.tsx`

**Tipo:** Integration - Formulario con validacion y mutation

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Renderiza textarea y boton Enviar | Unit | - |
| 2 | Boton Enviar deshabilitado cuando textarea esta vacio | Unit | FA-06 |
| 3 | Boton Enviar habilitado cuando hay contenido | Unit | FA-06 |
| 4 | Muestra error de validacion cuando contenido supera 5000 chars | Unit | AC-CS05-11 |
| 5 | Muestra contador de caracteres cuando supera 4000 | Unit | - |
| 6 | Toggle muestra campo de URL al hacer click en "Adjuntar URL" | Unit | - |
| 7 | Toggle oculta campo de URL al hacer click en "Quitar adjunto" | Unit | - |
| 8 | Muestra error de URL invalida en tiempo real | Unit | FA-04 |
| 9 | Enter envia el mensaje (sin Shift) | Integration | - |
| 10 | Shift+Enter inserta salto de linea sin enviar | Unit | - |
| 11 | Llama a mutation con contenido correcto al hacer click en Enviar | Integration | - |
| 12 | Limpia textarea tras envio exitoso | Integration | - |
| 13 | Muestra spinner y deshabilita input durante envio | Integration | - |
| 14 | Muestra toast de error si el envio falla | Integration | - |
| 15 | Boton Enviar tiene aria-label accesible | Unit | Accesibilidad |

**Casos detallados:**

```markdown
2. Boton deshabilitado con textarea vacio
   - Render: <MessageInput conversacionId="conv-123" onMensajeEnviado={vi.fn()} />
   - Assert: screen.getByRole('button', { name: /enviar/i }) tiene atributo disabled

9. Enter sin Shift envia el mensaje
   - userEvent.type(textarea, 'Hola mundo')
   - userEvent.keyboard('{Enter}')
   - Assert: mutation.mutate llamada con { contenido: 'Hola mundo', urlAdjunto: undefined }

11. Llama a mutation con datos correctos
   - Mock: vi.fn().mockResolvedValue(mockMensajeNuevo)
   - userEvent.type(textarea, 'Mensaje de prueba')
   - userEvent.click(screen.getByRole('button', { name: /enviar/i }))
   - waitFor(() => expect(mutationFn).toHaveBeenCalledWith({
       contenido: 'Mensaje de prueba', urlAdjunto: undefined
     }))

12. Limpia textarea tras envio exitoso
   - Despues del envio exitoso
   - Assert: textarea.value === ''
```

---

#### NavbarMensajesIcon.test.tsx

**Archivo:** `__tests__/components/NavbarMensajesIcon.test.tsx`

**Tipo:** Unit - Componente de presentacion con logica de badge

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Renderiza el icono de mensajeria | Unit | AC-CS05-9 |
| 2 | Muestra badge cuando `totalNoLeidos > 0` | Unit | AC-CS05-9 |
| 3 | Oculta badge cuando `totalNoLeidos === 0` | Unit | AC-CS05-9 |
| 4 | Muestra el numero exacto en el badge (1-99) | Unit | AC-CS05-9 |
| 5 | Muestra "99+" cuando `totalNoLeidos > 99` | Unit | AC-CS05-9 |
| 6 | Badge tiene `aria-label` correcto con el conteo | Unit | Accesibilidad |
| 7 | Enlace apunta a `/crowdsourcing/mensajes` | Unit | - |
| 8 | No se renderiza cuando el usuario no esta autenticado | Unit | - |

**Casos detallados:**

```markdown
3. Badge oculto con cero no leidos
   - Render: <NavbarMensajesIcon totalNoLeidos={0} />
   - Assert: badge no visible en el DOM (hidden o no existe)

5. Badge muestra 99+
   - Render: <NavbarMensajesIcon totalNoLeidos={150} />
   - Assert: screen.getByText('99+') visible

6. Badge accesible
   - Render: <NavbarMensajesIcon totalNoLeidos={3} />
   - Assert: elemento con aria-label='3 mensajes no leidos' presente
```

---

#### IniciarConversacionDialog.test.tsx

**Archivo:** `__tests__/components/IniciarConversacionDialog.test.tsx`

**Tipo:** Integration - Dialog con formulario React Hook Form + Zod y mutation

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Renderiza el dialog cuando `isOpen=true` | Unit | - |
| 2 | No renderiza contenido cuando `isOpen=false` | Unit | - |
| 3 | Muestra nombre del destinatario en la cabecera | Unit | AC-CS05-1 |
| 4 | Muestra titulo y tipo del contexto | Unit | - |
| 5 | Campo asunto tiene foco automatico al abrir | Integration | - |
| 6 | Muestra contador de caracteres del asunto en tiempo real | Integration | AC-CS05-1 |
| 7 | Muestra error si asunto esta vacio al hacer submit | Integration | FA-07 |
| 8 | Muestra error si asunto supera 200 caracteres | Integration | AC-CS05-1 |
| 9 | Submit exitoso: cierra dialog y navega al chat | Integration | AC-CS05-1 |
| 10 | Submit exitoso: muestra toast de exito | Integration | AC-CS05-1 |
| 11 | Error 4015: muestra toast informativo y redirige a conversacion existente | Integration | AC-CS05-3 / FA-01 |
| 12 | Error 403 sin relacion: muestra toast de error, dialog permanece abierto | Integration | FA-02 |
| 13 | Error generico de API: muestra toast de error, dialog permanece abierto | Integration | - |
| 14 | Boton Cancelar cierra el dialog | Integration | - |
| 15 | Tecla Escape cierra el dialog | Integration | - |
| 16 | Inputs deshabilitados durante el envio | Integration | - |
| 17 | Muestra spinner en boton durante el envio | Integration | - |
| 18 | Resetea el campo asunto al cerrar y reabrir | Integration | - |

**Casos detallados:**

```markdown
9. Submit exitoso navega al chat
   - Mock: useCreateConversacion retorna mutationFn que resuelve con mockCreateConversacionResult
   - userEvent.type(screen.getByLabelText(/asunto/i), 'Consulta sobre la propuesta')
   - userEvent.click(screen.getByRole('button', { name: /iniciar conversacion/i }))
   - waitFor(() => {
       expect(onClose).toHaveBeenCalled()
       expect(navigate).toHaveBeenCalledWith('/crowdsourcing/mensajes/f2a3b4c5-...')
       expect(toast.success).toHaveBeenCalledWith('Conversacion iniciada correctamente.')
     })

11. Error 4015 - conversacion duplicada
   - Mock: mutation falla con errorCode '4015' y algun conversacionId existente
   - Hacer submit valido
   - waitFor(() => {
       expect(toast.info o toast.success).toHaveBeenCalledWith(
         expect.stringContaining('Ya existe una conversacion')
       )
       expect(navigate).toHaveBeenCalledWith('/crowdsourcing/mensajes/{id-existente}')
     })
   - Assert: dialog puede haber cerrado (FA-01: redirige)

12. Error sin relacion dialog permanece abierto
   - Mock: mutation falla con errorCode '3002'
   - Hacer submit
   - waitFor(() => expect(toast.error).toHaveBeenCalledWith(
       expect.stringContaining('No puedes iniciar una conversacion')
     ))
   - Assert: dialog sigue visible (screen.getByRole('dialog'))
```

---

#### ChatView.test.tsx

**Archivo:** `__tests__/components/ChatView.test.tsx`

**Tipo:** Integration - Vista completa del chat, orquesta hooks, auto-scroll, marcar leidos

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Muestra skeleton durante carga inicial | Unit | - |
| 2 | Renderiza cabecera con nombre de la otra parte | Integration | - |
| 3 | Renderiza lista de mensajes al cargar con exito | Integration | AC-CS05-5 |
| 4 | Muestra empty state cuando no hay mensajes | Integration | - |
| 5 | Ejecuta `useMarcarLeidos` automaticamente al montar | Integration | AC-CS05-6 |
| 6 | Muestra boton "Cargar mensajes anteriores" cuando hay mas paginas | Integration | AC-CS05-12 |
| 7 | Oculta boton "Cargar mas" cuando es la ultima pagina | Integration | AC-CS05-12 |
| 8 | Muestra alert de error cuando la carga de mensajes falla | Integration | - |
| 9 | Enlace "Volver a mensajes" navega a `/crowdsourcing/mensajes` | Integration | - |
| 10 | Renderiza separadores de fecha entre dias diferentes | Unit | - |

**Casos detallados:**

```markdown
5. useMarcarLeidos se ejecuta al montar
   - Mock: useMarcarLeidos con vi.fn() espionado
   - Render: <ChatView conversacionId="f2a3b4c5-..." />
   - waitFor para que carguen los mensajes
   - Assert: marcarLeidos.mutate fue llamado al montar el componente
   - Este test verifica el criterio AC-CS05-6

9. Volver a mensajes
   - Render: <ChatView conversacionId="..." /> dentro de MemoryRouter
   - Assert: screen.getByRole('link', { name: /volver a mensajes/i })
         .getAttribute('href') === '/crowdsourcing/mensajes'
```

---

### 4.3 Hooks

#### useConversaciones.test.ts

**Archivo:** `__tests__/hooks/useConversaciones.test.ts`

**Tipo:** Integration - Query con filtro, paginacion y polling

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Retorna datos correctamente en estado de exito | Integration | AC-CS05-7 |
| 2 | `isLoading` true en el estado inicial | Unit | - |
| 3 | `isError` true cuando la API falla | Integration | - |
| 4 | Llama al servicio con el filtro por defecto 'todas' | Integration | AC-CS05-8 |
| 5 | Llama al servicio con filtro 'necesidades' cuando se especifica | Integration | AC-CS05-8 |
| 6 | Llama al servicio con filtro 'acuerdos' cuando se especifica | Integration | AC-CS05-8 |
| 7 | Re-ejecuta la query cuando cambia el filtro | Integration | AC-CS05-8 |
| 8 | Llama al servicio con numero de pagina correcto | Integration | AC-CS05-12 |
| 9 | Usa la `QUERY_KEY` correcta segun el filtro | Unit | - |
| 10 | Tiene configurado `refetchInterval` con `MENSAJERIA_POLLING_INTERVAL_MS` | Unit | - |

**Casos detallados:**

```markdown
1. Retorna datos en exito
   - vi.mock servicio
   - vi.mocked(conversacionService.getConversaciones).mockResolvedValue(mockConversacionListResponse)
   - renderHook(() => useConversaciones(), { wrapper })
   - waitFor(() => result.current.isSuccess)
   - Assert: result.current.data.items tiene 3 items
   - Assert: result.current.data.totalNoLeidos === 2

4. Llama con filtro por defecto
   - renderHook(() => useConversaciones())
   - waitFor(() => result.current.isSuccess)
   - Assert: conversacionService.getConversaciones fue llamado con contexto: 'todas'

10. Polling configurado
   - renderHook(() => useConversaciones())
   - Assert: la opcion refetchInterval del useQuery es 10000
   - (Verificar via inspeccion del hook o mockeando useQuery)
```

---

#### useMensajes.test.ts

**Archivo:** `__tests__/hooks/useMensajes.test.ts`

**Tipo:** Integration - Query con paginacion y polling activo

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Retorna mensajes correctamente para el conversacionId dado | Integration | - |
| 2 | `isLoading` true inicialmente | Unit | - |
| 3 | `isError` true cuando la API falla | Integration | - |
| 4 | Llama al servicio con `conversacionId` correcto | Integration | - |
| 5 | Llama al servicio con `page=1` por defecto | Integration | AC-CS05-12 |
| 6 | Llama al servicio con numero de pagina especificado | Integration | AC-CS05-12 |
| 7 | Re-ejecuta la query cuando cambia la pagina | Integration | AC-CS05-12 |
| 8 | Usa la `QUERY_KEY` correcta incluyendo `conversacionId` y pagina | Unit | - |
| 9 | Tiene configurado `refetchInterval` con `MENSAJERIA_POLLING_INTERVAL_MS` | Unit | - |
| 10 | Deshabilitado cuando `conversacionId` esta vacio | Unit | - |

**Casos detallados:**

```markdown
5. Pagina 1 por defecto
   - renderHook(() => useMensajes('conv-123'), { wrapper })
   - waitFor(() => result.current.isSuccess)
   - Assert: mensajeService.getMensajes llamado con ('conv-123', 1)

10. Deshabilitado sin ID
   - renderHook(() => useMensajes(''), { wrapper })
   - Assert: result.current.isFetching === false
   - Assert: mensajeService.getMensajes no fue llamado
```

---

#### useCreateConversacion.test.ts

**Archivo:** `__tests__/hooks/useCreateConversacion.test.ts`

**Tipo:** Integration - Mutation con logica de redireccion y manejo de error 4015

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Mutation exitosa retorna `CreateConversacionResult` | Integration | AC-CS05-1 |
| 2 | Navega al chat de la nueva conversacion tras exito | Integration | AC-CS05-1 |
| 3 | Muestra toast de exito tras creacion | Integration | AC-CS05-1 |
| 4 | Invalida queries de conversaciones tras exito | Integration | - |
| 5 | Detecta error 4015 y redirige a conversacion existente | Integration | AC-CS05-3 |
| 6 | Muestra toast informativo para error 4015 | Integration | AC-CS05-3 |
| 7 | Error 403 sin relacion no navega | Integration | FA-02 |
| 8 | Error 403 muestra toast de error | Integration | FA-02 |
| 9 | Error generico muestra toast de error | Integration | - |
| 10 | `isLoading` true durante la mutation | Unit | - |

**Casos detallados:**

```markdown
5. Redireccion en error 4015
   - Mock: servicio rechaza con error que contiene errorCode '4015'
     y en el payload incluye el id de conversacion existente
   - mutation.mutate({ asunto: 'Test', userIdDestinatario: 'user-x', necesidadId: 'nec-y' })
   - waitFor(() => expect(navigate).toHaveBeenCalledWith(
       '/crowdsourcing/mensajes/{id-conversacion-existente}'
     ))
   - Assert: toast.info llamado con mensaje de conversacion existente

   Nota: El backend devuelve errorCode '4015'. El frontend debe interpretar
   este error y redirigir. El ID de la conversacion existente se obtiene
   de la respuesta del backend (a definir como campo adicional en el error).
```

---

#### useEnviarMensaje.test.ts

**Archivo:** `__tests__/hooks/useEnviarMensaje.test.ts`

**Tipo:** Integration - Mutation con invalidacion de queries

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Mutation exitosa retorna el mensaje creado | Integration | - |
| 2 | Invalida query de mensajes del conversacionId tras exito | Integration | - |
| 3 | Invalida query del listado de conversaciones tras exito | Integration | AC-CS05-7 |
| 4 | Invalida query de no-leidos tras exito | Integration | AC-CS05-9 |
| 5 | Llama al servicio con `conversacionId` y datos correctos | Integration | - |
| 6 | `isError` true cuando la API falla | Integration | - |
| 7 | Acepta mensaje sin `urlAdjunto` | Integration | AC-CS05-10 |
| 8 | Acepta mensaje con `urlAdjunto` valida | Integration | AC-CS05-10 |

**Casos detallados:**

```markdown
2. Invalida query de mensajes
   - const queryClient = createTestQueryClient()
   - const invalidateSpy = vi.spyOn(queryClient, 'invalidateQueries')
   - renderHook(() => useEnviarMensaje('conv-123'), { wrapper con queryClient })
   - mutate({ contenido: 'Hola', urlAdjunto: undefined })
   - waitFor(() => result.current.isSuccess)
   - Assert: invalidateSpy llamado con queryKey que incluye 'conv-123' y 'mensajes'

3. Invalida listado de conversaciones
   - Assert: invalidateSpy llamado con queryKey de lista de conversaciones
   (para actualizar el preview del ultimo mensaje)
```

---

#### useMarcarLeidos.test.ts

**Archivo:** `__tests__/hooks/useMarcarLeidos.test.ts`

**Tipo:** Integration - Mutation automatica al abrir chat

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Mutation exitosa retorna `mensajesMarcados` | Integration | AC-CS05-6 |
| 2 | Invalida query de no-leidos tras exito | Integration | AC-CS05-9 |
| 3 | Invalida query del listado de conversaciones tras exito | Integration | AC-CS05-7 |
| 4 | Llama al servicio con el `conversacionId` correcto | Integration | AC-CS05-6 |
| 5 | No genera error cuando `mensajesMarcados === 0` | Integration | AC-CS05-6 |
| 6 | `isError` true cuando la API falla | Integration | - |

**Casos detallados:**

```markdown
1. Retorna mensajesMarcados
   - vi.mocked(conversacionService.marcarLeidos).mockResolvedValue(mockMarcarLeidosResponse)
   - renderHook(() => useMarcarLeidos('conv-123'), { wrapper })
   - result.current.mutate()
   - waitFor(() => result.current.isSuccess)
   - Assert: result.current.data.mensajesMarcados === 2

5. Sin error cuando mensajesMarcados es cero
   - vi.mocked(...).mockResolvedValue(mockMarcarLeidosResponseCero)
   - mutate()
   - waitFor(() => result.current.isSuccess)
   - Assert: result.current.isError === false
```

---

#### useNoLeidosCount.test.ts

**Archivo:** `__tests__/hooks/useNoLeidosCount.test.ts`

**Tipo:** Integration - Query ligera para badge del navbar con polling

| # | Test Case | Tipo | Criterio AC |
|---|-----------|------|-------------|
| 1 | Retorna el conteo total de no leidos | Integration | AC-CS05-9 |
| 2 | Retorna 0 cuando no hay no leidos | Integration | AC-CS05-9 |
| 3 | `isLoading` true inicialmente | Unit | - |
| 4 | `isError` true cuando la API falla | Integration | - |
| 5 | Usa la `QUERY_KEY` correcta (`noLeidos`) | Unit | - |
| 6 | Tiene configurado `refetchInterval` | Unit | AC-CS05-9 |

---

### 4.4 Services

Los tests de services validan la comunicacion con la API real via MSW. Se usa `setupServer` de MSW directamente en estos tests.

#### conversacion.service.test.ts

**Archivo:** `__tests__/services/conversacion.service.test.ts` (opcional, depende de si el servicio tiene logica relevante ademas del fetch)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | `getConversaciones` llama al endpoint correcto con filtro | Unit | GET /api/crowdsourcing/conversaciones?contexto=todas |
| 2 | `getConversaciones` pasa parametros de paginacion | Unit | page y pageSize en query string |
| 3 | `createConversacion` envia POST con body correcto | Unit | Incluye asunto, userIdDestinatario, necesidadId |
| 4 | `marcarLeidos` envia PATCH al endpoint correcto | Unit | PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos |
| 5 | `getNoLeidosCount` llama al endpoint ligero correcto | Unit | GET /api/crowdsourcing/conversaciones/no-leidos |

---

## 5. Tests de Integracion de Flujos Criticos

Estos tests validan flujos completos de usuario que cruzan multiples componentes y hooks.

### Flujo 1: Enviar Mensaje en un Chat (AC-CS05-5, AC-CS05-6)

**Archivo:** `__tests__/integration/flujo-enviar-mensaje.test.tsx`

**Escenario:** Usuario abre un chat, los mensajes se cargan, se ejecuta marcar-leidos, el usuario escribe y envia un mensaje.

```markdown
Setup:
- MSW handlers para GET mensajes (retorna mockMensajeListResponse)
- MSW handler para PATCH marcar-leidos
- MSW handler para POST mensajes

Pasos:
1. Render: <ChatView conversacionId="f2a3b4c5-..." />
2. waitFor: mensajes cargados y visibles
3. Assert: PATCH marcar-leidos fue invocado (AC-CS05-6)
4. Assert: mensaje ajeno aparece a la IZQUIERDA (AC-CS05-5)
5. Assert: mensaje propio aparece a la DERECHA (AC-CS05-5)
6. userEvent.type(textarea, 'Nuevo mensaje de prueba')
7. userEvent.click(boton Enviar)
8. waitFor: POST /mensajes invocado con contenido correcto
9. Assert: textarea limpiado tras envio
```

### Flujo 2: Filtrar Conversaciones y Navegar al Chat

**Archivo:** `__tests__/integration/flujo-filtrar-navegar.test.tsx`

**Escenario:** Usuario en `/crowdsourcing/mensajes` filtra por acuerdos y hace click en una conversacion.

```markdown
Setup:
- MSW handlers para GET conversaciones (retorna lista mixta)
- Mock navigate

Pasos:
1. Render: <ConversacionList filtro="todas" ... /> en MemoryRouter
2. waitFor: lista cargada
3. Assert: conversaciones sobre necesidades y acuerdos visibles
4. userEvent.click(tab 'Acuerdos')
5. waitFor: nueva peticion con contexto=acuerdos
6. Assert: solo conversacion sobre acuerdo visible
7. userEvent.click(fila de conversacion sobre acuerdo)
8. Assert: navigate llamado con '/crowdsourcing/mensajes/{id}'
```

### Flujo 3: Crear Conversacion - Exito (AC-CS05-1, AC-CS05-3)

**Archivo:** `__tests__/integration/flujo-crear-conversacion.test.tsx`

**Escenario:** Usuario abre el dialog, completa el asunto, confirma y es redirigido al chat.

```markdown
Setup:
- MSW handler para POST /api/crowdsourcing/conversaciones (201 Created)
- Mock navigate y toast

Pasos:
1. Render: <IniciarConversacionDialog isOpen={true} ... destinatarioNombre="Studio Mix Pro"
           contextoTipo="necesidad" contextoTitulo="Mezcla de pistas" ... />
2. Assert: dialog visible con "Con: Studio Mix Pro"
3. userEvent.type(input asunto, 'Consulta sobre tu experiencia')
4. Assert: contador muestra '33 / 200 caracteres'
5. userEvent.click(boton 'Iniciar conversacion')
6. waitFor: mutation ejecutada
7. Assert: navigate llamado con la ruta del chat
8. Assert: toast.success llamado
```

### Flujo 4: Crear Conversacion - Conversacion Duplicada (FA-01, AC-CS05-3)

**Escenario:** Backend retorna error 4015, el frontend redirige a la conversacion existente.

```markdown
Setup:
- MSW handler: POST /conversaciones retorna 400 con errorCode '4015'
              y la respuesta incluye el ID de conversacion existente
- Mock navigate y toast

Pasos:
1. Completar formulario con asunto valido
2. userEvent.click('Iniciar conversacion')
3. waitFor: peticion completada
4. Assert: toast.info con mensaje de conversacion existente
5. Assert: navigate a '/crowdsourcing/mensajes/{id-existente}'
6. (No muestra error rojo - es un flujo de redireccion, no de error)
```

### Flujo 5: Badge de Navbar se Actualiza tras Marcar Leidos (AC-CS05-9)

**Escenario:** Al abrir un chat, useMarcarLeidos invalida la query de noLeidos, el badge se actualiza.

```markdown
Setup:
- queryClient compartido con spy en invalidateQueries
- MSW handlers para mensajes y marcar-leidos

Pasos:
1. Render: ambos componentes (NavbarMensajesIcon + ChatView) en el mismo QueryClientProvider
2. waitFor: datos iniciales de no-leidos cargados (badge muestra 2)
3. Render o mount ChatView
4. waitFor: PATCH marcar-leidos ejecutado
5. Assert: invalidateQueries llamado con queryKey de no-leidos
6. (El badge se actualiza via refetch automatico tras invalidacion)
```

---

## 6. Mapeo de Criterios de Aceptacion a Tests

| AC | Criterio | Tests que lo cubren |
|----|----------|---------------------|
| AC-CS05-1 | Artista puede iniciar conversacion, asunto requerido max 200 chars | `IniciarConversacionDialog` #7,#8,#9; `createConversacionSchema` #2,#3,#4; `useCreateConversacion` #1,#2,#3 |
| AC-CS05-3 | Si ya existe conversacion, redirige a la existente | `IniciarConversacionDialog` #11; `useCreateConversacion` #5,#6; Flujo 4 |
| AC-CS05-5 | Mensajes en orden cronologico, propios a la derecha, ajenos a la izquierda | `MessageBubble` #2,#3; Flujo 1 |
| AC-CS05-6 | Al abrir chat, auto-marcar mensajes no leidos del interlocutor | `ChatView` #5; `useMarcarLeidos` #1,#4; Flujo 1 |
| AC-CS05-7 | Listado con badge numerico de no leidos y ordenado por fecha | `ConversacionRow` #6,#7,#8; `ConversacionList` #3,#4 |
| AC-CS05-8 | Filtrar listado por Todas / Necesidades / Acuerdos | `ConversacionList` #7,#8,#9,#13; `useConversaciones` #4,#5,#6 |
| AC-CS05-9 | Navbar con badge total de no leidos, actualizable | `NavbarMensajesIcon` #2,#3,#4,#5; `useNoLeidosCount` #1,#6; Flujo 5 |
| AC-CS05-10 | Mensaje con URL adjunta valida; invalida retorna error | `createMensajeSchema` #14,#15,#16; `MessageBubble` #8,#11; `MessageInput` #8 |
| AC-CS05-11 | Contenido del mensaje: min 1, max 5000 chars | `createMensajeSchema` #9,#10,#11; `MessageInput` #2,#4 |
| AC-CS05-12 | Paginacion en listado y en mensajes (inversa) | `ConversacionList` #10,#11; `useMensajes` #5,#6,#7; `ChatView` #6,#7 |

| FA | Flujo Alternativo | Tests que lo cubren |
|----|-------------------|---------------------|
| FA-01 | Conversacion ya existe, redirige | `IniciarConversacionDialog` #11; `useCreateConversacion` #5,#6; Flujo 4 |
| FA-02 | Sin relacion, accion no disponible, 403 | `IniciarConversacionDialog` #12; `useCreateConversacion` #7,#8 |
| FA-03 | Sin conversaciones activas, empty state | `ConversacionList` #5 |
| FA-04 | URL adjunta invalida, error de validacion | `createMensajeSchema` #15,#16; `MessageInput` #8 |
| FA-06 | Contenido vacio o > 5000 chars | `createMensajeSchema` #10,#11; `MessageInput` #2,#4 |
| FA-07 | Asunto vacio o > 200 chars | `createConversacionSchema` #2,#4; `IniciarConversacionDialog` #7,#8 |

---

## 7. Cobertura por Archivo

| Archivo | Lineas Objetivo | Funciones Objetivo | Branches Objetivo |
|---------|-----------------|--------------------|--------------------|
| `ConversacionList.tsx` | 85% | 90% | 80% |
| `ConversacionRow.tsx` | 90% | 100% | 85% |
| `ChatView.tsx` | 80% | 85% | 75% |
| `MessageBubble.tsx` | 95% | 100% | 90% |
| `MessageInput.tsx` | 85% | 90% | 80% |
| `NavbarMensajesIcon.tsx` | 95% | 100% | 95% |
| `IniciarConversacionDialog.tsx` | 85% | 90% | 80% |
| `useConversaciones.ts` | 90% | 100% | 85% |
| `useMensajes.ts` | 90% | 100% | 85% |
| `useCreateConversacion.ts` | 90% | 100% | 90% |
| `useEnviarMensaje.ts` | 90% | 100% | 85% |
| `useMarcarLeidos.ts` | 95% | 100% | 90% |
| `useNoLeidosCount.ts` | 90% | 100% | 85% |
| `conversacion.service.ts` | 90% | 100% | 85% |
| `mensaje.service.ts` | 90% | 100% | 85% |
| `mensajeria.schema.ts` | 95% | 100% | 95% |

**Meta Global:** 80%+ en todas las metricas

---

## 8. Estrategia de Mocking

### Patron establecido en el proyecto

El proyecto usa `vi.mock` a nivel de modulo para mockear servicios, consistente con los tests existentes en `campanias` y `backings`:

```typescript
// Mockear el servicio de conversaciones
vi.mock('../../infrastructure/conversacion.service', () => ({
    conversacionService: {
        getConversaciones: vi.fn(),
        createConversacion: vi.fn(),
        marcarLeidos: vi.fn(),
        getNoLeidosCount: vi.fn(),
    },
}));

// Mockear el servicio de mensajes
vi.mock('../../infrastructure/mensaje.service', () => ({
    mensajeService: {
        getMensajes: vi.fn(),
        enviarMensaje: vi.fn(),
    },
}));
```

### Mocking de react-router-dom

```typescript
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
        useParams: () => ({ conversacionId: 'f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c' }),
    };
});
```

### Mocking de sonner (toast)

```typescript
vi.mock('sonner', () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}));
```

### Mocking de polling con vi.useFakeTimers

Para tests que necesiten verificar comportamiento de polling:

```typescript
beforeEach(() => {
    vi.useFakeTimers();
});

afterEach(() => {
    vi.useRealTimers();
});

// Avanzar tiempo para disparar el refetch por polling
vi.advanceTimersByTime(10000); // MENSAJERIA_POLLING_INTERVAL_MS
```

### Cuando usar MSW vs vi.mock

| Escenario | Herramienta | Razon |
|-----------|-------------|-------|
| Tests de services directamente | MSW | Verifica el contrato HTTP real |
| Tests de hooks (`useQuery`, `useMutation`) | `vi.mock` del servicio | Mas rapido, aislado, sin red |
| Tests de componentes con hooks | `vi.mock` del servicio o hook | Control total del estado |
| Tests de integracion de flujo | MSW | Simula comportamiento real end-to-end |

---

## 9. Consideraciones Especiales

### Polling

El polling (`refetchInterval: 10000`) se configura en `useConversaciones`, `useMensajes` y `useNoLeidosCount`. En los tests:

- **Tests unitarios de hooks:** Verificar que la opcion `refetchInterval` tiene el valor correcto (`MENSAJERIA_POLLING_INTERVAL_MS`). No es necesario simular el tiempo real.
- **Tests de integracion:** Usar `vi.useFakeTimers()` solo si se necesita verificar que el refetch ocurre despues de un intervalo. En la mayoria de los tests de hooks esto no es necesario.

### Error 4015 - Conversacion Duplicada

Este es el caso mas critico de la feature. `useCreateConversacion` debe:
1. Detectar que el error tiene `errorCode === '4015'`
2. Extraer el ID de la conversacion existente de la respuesta del backend
3. Navegar a `/crowdsourcing/mensajes/{id-existente}`
4. Mostrar un toast informativo (no de error)

El mock de este escenario debe construirse con precision para reflejar la estructura real del error del backend.

### Marcar Leidos Automatico

`ChatView` debe invocar `useMarcarLeidos.mutate()` en un `useEffect` con array de dependencias vacio (al montar). El test debe verificar que esta llamada ocurre sin interaccion del usuario.

### Paginacion Inversa de Mensajes

Los mensajes se reciben del backend ya en orden cronologico ascendente (backend ordena por `FechaCreacion ASC`). El test de `ChatView` con paginacion debe verificar que el boton "Cargar mensajes anteriores" aparece cuando `totalCount > pageSize * page` y que al hacer click se solicita la pagina 2.

### ResizeObserver

El `vitest.setup.ts` ya incluye el polyfill de `ResizeObserver` necesario para componentes de Radix UI (Dialog, Tabs). El `ScrollArea` de shadcn/ui tambien puede requerir este polyfill.

---

## 10. Comandos de Ejecucion

```bash
# Ejecutar todos los tests de la feature de mensajeria
npm run test -- --reporter=verbose src/web/src/features/crowdsourcing/mensajeria

# Ejecutar con cobertura
npm run test:coverage -- src/web/src/features/crowdsourcing/mensajeria

# Watch mode durante desarrollo
npm run test -- --watch src/web/src/features/crowdsourcing/mensajeria

# Ejecutar solo los tests de schemas (los mas rapidos)
npm run test -- --reporter=verbose src/web/src/features/crowdsourcing/mensajeria/__tests__/schemas

# Ejecutar todos los tests del proyecto web
cd src/web && npm run test

# Con cobertura completa del proyecto
cd src/web && npm run test:coverage
```

> Nota: Los comandos exactos dependen de la configuracion de `package.json` del proyecto. El runner es Vitest segun `vite.config.ts`.

---

## 11. CI/CD Integration

```yaml
- name: Run Frontend Tests (Mensajeria)
  working-directory: src/web
  run: npm run test:coverage

- name: Check Coverage Threshold
  run: |
    # La cobertura debe ser >= 80% para esta feature
    echo "Coverage check para cs-mensajeria"

- name: Upload Coverage Report
  uses: codecov/codecov-action@v3
  with:
    files: src/web/coverage/lcov.info
    flags: frontend-landing-mensajeria
```

---

## 12. Checklist

### Mocks y Fixtures
- [ ] `mensajeria.mock.ts` con todos los tipos de conversacion y mensaje
- [ ] `handlers.ts` MSW con los 5 endpoints de la feature
- [ ] `handlers.ts` incluye `errorHandlers` para casos de error especificos
- [ ] Test utilities (`renderWithMensajeriaProviders`, `createHookWrapper`)

### Tests de Schemas
- [ ] `createConversacionSchema`: validaciones de asunto y destinatario
- [ ] `createMensajeSchema`: validaciones de contenido y URL adjunta

### Tests de Componentes
- [ ] `ConversacionList`: loading, exito, empty, error, filtros, paginacion
- [ ] `ConversacionRow`: todos los estados de badge, avatar, preview
- [ ] `MessageBubble`: alineacion propio/ajeno, adjunto, accesibilidad
- [ ] `MessageInput`: validacion, toggle URL, enter-enviar, estados de mutation
- [ ] `NavbarMensajesIcon`: badge condicional, 99+, aria-label
- [ ] `IniciarConversacionDialog`: flujo completo incluyendo error 4015

### Tests de Hooks
- [ ] `useConversaciones`: filtros, paginacion, polling configurado
- [ ] `useMensajes`: paginacion, polling, deshabilitado sin ID
- [ ] `useCreateConversacion`: exito, error 4015, error 403, error generico
- [ ] `useEnviarMensaje`: exito, invalidaciones de queries
- [ ] `useMarcarLeidos`: exito, respuesta con cero marcados, invalidaciones
- [ ] `useNoLeidosCount`: retorno correcto, polling configurado

### Tests de Integracion
- [ ] Flujo enviar mensaje (AC-CS05-5, AC-CS05-6)
- [ ] Flujo filtrar y navegar (AC-CS05-8)
- [ ] Flujo crear conversacion exitosa (AC-CS05-1)
- [ ] Flujo conversacion duplicada/redireccion (AC-CS05-3, FA-01)
- [ ] Flujo badge actualizado tras marcar leidos (AC-CS05-9)

### Cobertura
- [ ] Cobertura 80%+ en todos los archivos de la feature
- [ ] Tests pasan en modo CI (sin dependencias de red real)
- [ ] Tiempo de ejecucion < 60 segundos
