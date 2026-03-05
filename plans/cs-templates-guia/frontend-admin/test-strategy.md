# Estrategia de Testing: Templates de Crowdsourcing (Admin)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** src/admin (Next.js 14)
**Cobertura Objetivo:** 80%+

---

## 1. Resumen

| Tipo | Cantidad | Cobertura |
|------|----------|-----------|
| Unit Tests (Components) | 12 | 85% |
| Unit Tests (Hooks) | 8 | 90% |
| Integration Tests | 4 | 75% |
| **Total** | **24** | **80%+** |

**Scope:** Admin dashboard CRUD para templates de proyecto (Pantallas 4 y 5 del UI/UX).

**No incluye:** Wizard de generacion de necesidades (eso esta en Landing/Web, no en Admin).

---

## 2. Estructura de Tests

```
src/admin/src/features/crowdsourcing/
├── __tests__/
│   ├── components/
│   │   ├── TemplateTable.test.tsx
│   │   ├── TemplateFilters.test.tsx
│   │   ├── TemplateForm.test.tsx
│   │   ├── NeedFormItem.test.tsx
│   │   ├── TemplateStatusDialog.test.tsx
│   │   └── TemplateBadges.test.tsx
│   ├── hooks/
│   │   ├── useTemplates.test.ts
│   │   ├── useTemplateDetail.test.ts
│   │   ├── useCreateTemplate.test.ts
│   │   ├── useUpdateTemplate.test.ts
│   │   ├── useToggleTemplateStatus.test.ts
│   │   └── useRolesProfesionales.test.ts
│   ├── integration/
│   │   ├── template-crud-flow.test.tsx
│   │   └── template-filtering.test.tsx
│   └── __mocks__/
│       ├── templates.mock.ts
│       ├── roles.mock.ts
│       └── handlers.ts
├── components/
│   ├── TemplateTable.tsx
│   ├── TemplateFilters.tsx
│   ├── TemplateForm.tsx
│   ├── NeedFormItem.tsx
│   └── ...
├── hooks/
│   ├── useTemplates.ts
│   ├── useCreateTemplate.ts
│   └── ...
└── services/
    └── templates.service.ts
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `__mocks__/templates.mock.ts`

```typescript
import type { PlantillaProyectoList, PlantillaProyecto, PlantillaProyectoNecesidad, RolProfesional } from '@/shared/types';

// Mock de rol profesional
export const mockRolProductor: RolProfesional = {
    id: 1,
    nombre: 'Productor Musical',
    descripcion: 'Profesional que guia el proceso creativo y tecnico de la grabacion',
    categoriaRolId: 1,
    modalidadCobro: 'Por proyecto',
};

export const mockRolIngeniero: RolProfesional = {
    id: 2,
    nombre: 'Ingeniero de Grabacion',
    descripcion: 'Tecnico especializado en captura de audio de alta calidad',
    categoriaRolId: 2,
    modalidadCobro: 'Por dia',
};

// Mock de necesidad de plantilla
export const mockNecesidadProductor: PlantillaProyectoNecesidad = {
    id: '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d',
    fase: 'Preproduccion',
    titulo: 'Productor Musical',
    descripcion: 'Desarrollo de arreglos, estructura y direccion musical del proyecto',
    rolProfesional: mockRolProductor,
    precioMinOrientativo: 500,
    precioMaxOrientativo: 1500,
    moneda: 1,
    prioridad: 'Alta',
    orden: 1,
};

export const mockNecesidadIngeniero: PlantillaProyectoNecesidad = {
    id: '2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e',
    fase: 'Grabacion',
    titulo: 'Ingeniero de Grabacion',
    descripcion: 'Grabacion profesional de todos los instrumentos y voces',
    rolProfesional: mockRolIngeniero,
    precioMinOrientativo: 800,
    precioMaxOrientativo: 2000,
    moneda: 1,
    prioridad: 'Alta',
    orden: 2,
};

// Mock de plantilla completa
export const mockTemplateEP: PlantillaProyecto = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    nombre: 'Produccion de EP',
    descripcion: 'Plantilla completa para producir un EP de 4-6 canciones con calidad profesional',
    icono: 'music',
    orden: 1,
    necesidades: [mockNecesidadProductor, mockNecesidadIngeniero],
    resumen: {
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidadesAlta: 4,
        cantidadNecesidadesMedia: 2,
        cantidadNecesidadesBaja: 2,
    },
};

// Mock de lista de plantillas
export const mockTemplateList: PlantillaProyectoList[] = [
    {
        id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
        nombre: 'Produccion de EP',
        descripcion: 'Plantilla completa para producir un EP de 4-6 canciones',
        icono: 'music',
        orden: 1,
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidades: 8,
        fases: ['Preproduccion', 'Grabacion', 'Mezcla y Master', 'Promocion'],
    },
    {
        id: '7cb95f12-9301-4872-a1de-3f984e22bbc1',
        nombre: 'Produccion de Album Completo',
        descripcion: 'Plantilla para producir un album de 10-12 canciones',
        icono: 'disc',
        orden: 2,
        precioMinTotal: 8000,
        precioMaxTotal: 25000,
        moneda: 1,
        cantidadNecesidades: 12,
        fases: ['Preproduccion', 'Grabacion', 'Mezcla y Master', 'Diseno y Produccion', 'Promocion'],
    },
];

// Mock de template inactivo (para filtrado)
export const mockTemplateInactivo: PlantillaProyectoList = {
    id: '9ab12c45-7821-4963-c2ef-1d873f55aac3',
    nombre: 'Template Inactivo',
    descripcion: 'Template desactivado para testing',
    icono: 'x',
    orden: 99,
    precioMinTotal: 1000,
    precioMaxTotal: 3000,
    moneda: 1,
    cantidadNecesidades: 3,
    fases: ['Test'],
};
```

**Archivo:** `__mocks__/roles.mock.ts`

```typescript
import type { RolProfesionalConCategoria, CategoriaRol } from '@/shared/types';

export const mockCategoriaProduccion: CategoriaRol = {
    id: 1,
    nombre: 'Produccion Musical',
    icono: '🎛️',
    orden: 1,
};

export const mockCategoriaIngenieria: CategoriaRol = {
    id: 2,
    nombre: 'Ingenieria de Audio',
    icono: '🎚️',
    orden: 2,
};

export const mockRolesProfesionales: RolProfesionalConCategoria[] = [
    {
        id: 1,
        nombre: 'Productor Musical',
        descripcion: 'Profesional que guia el proceso creativo y tecnico',
        categoriaRol: mockCategoriaProduccion,
        modalidadCobro: 'Por proyecto',
        activo: true,
    },
    {
        id: 2,
        nombre: 'Ingeniero de Grabacion',
        descripcion: 'Tecnico especializado en captura de audio',
        categoriaRol: mockCategoriaIngenieria,
        modalidadCobro: 'Por dia',
        activo: true,
    },
    {
        id: 3,
        nombre: 'Ingeniero de Mezcla',
        descripcion: 'Especialista en balance y procesamiento de audio',
        categoriaRol: mockCategoriaIngenieria,
        modalidadCobro: 'Por cancion',
        activo: true,
    },
];
```

### 3.2 MSW Handlers

**Archivo:** `__mocks__/handlers.ts`

```typescript
import { rest } from 'msw';
import { mockTemplateList, mockTemplateEP, mockTemplateInactivo } from './templates.mock';
import { mockRolesProfesionales } from './roles.mock';

export const templateHandlers = [
    // GET /api/crowdsourcing/templates
    rest.get('/api/crowdsourcing/templates', (req, res, ctx) => {
        const status = req.url.searchParams.get('status');

        let templates = [...mockTemplateList];

        // Filtrar por status
        if (status === 'active') {
            templates = templates; // Todos activos por defecto en mock
        } else if (status === 'inactive') {
            templates = [mockTemplateInactivo];
        }

        return res(
            ctx.status(200),
            ctx.json({
                data: { items: templates },
                messages: [{ message: 'Plantillas obtenidas exitosamente', errorCode: '0000' }],
            })
        );
    }),

    // GET /api/crowdsourcing/templates/:id
    rest.get('/api/crowdsourcing/templates/:id', (req, res, ctx) => {
        const { id } = req.params;

        if (id === mockTemplateEP.id) {
            return res(
                ctx.status(200),
                ctx.json({
                    data: mockTemplateEP,
                    messages: [{ message: 'Plantilla obtenida exitosamente', errorCode: '0000' }],
                })
            );
        }

        // Not found
        return res(
            ctx.status(404),
            ctx.json({
                messages: [{ message: 'Plantilla no encontrada', errorCode: '2006' }],
            })
        );
    }),

    // POST /api/crowdsourcing/templates
    rest.post('/api/crowdsourcing/templates', async (req, res, ctx) => {
        const body = await req.json();

        // Validacion basica
        if (!body.nombre) {
            return res(
                ctx.status(400),
                ctx.json({
                    messages: [{ message: 'El nombre es obligatorio', errorCode: '1001' }],
                })
            );
        }

        const newId = '123e4567-e89b-12d3-a456-426614174000';

        return res(
            ctx.status(201),
            ctx.json({
                data: newId,
                messages: [{ message: 'Template creado exitosamente', errorCode: '0001' }],
            })
        );
    }),

    // PUT /api/crowdsourcing/templates/:id
    rest.put('/api/crowdsourcing/templates/:id', async (req, res, ctx) => {
        const { id } = req.params;
        const body = await req.json();

        if (!body.nombre) {
            return res(
                ctx.status(400),
                ctx.json({
                    messages: [{ message: 'El nombre es obligatorio', errorCode: '1001' }],
                })
            );
        }

        return res(
            ctx.status(200),
            ctx.json({
                data: id,
                messages: [{ message: 'Template actualizado exitosamente', errorCode: '0002' }],
            })
        );
    }),

    // PATCH /api/crowdsourcing/templates/:id/toggle-status
    rest.patch('/api/crowdsourcing/templates/:id/toggle-status', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json({
                data: { activo: false },
                messages: [{ message: 'Estado del template actualizado', errorCode: '0002' }],
            })
        );
    }),

    // GET /api/crowdsourcing/maestras/roles-profesionales
    rest.get('/api/crowdsourcing/maestras/roles-profesionales', (req, res, ctx) => {
        return res(
            ctx.status(200),
            ctx.json({
                data: { items: mockRolesProfesionales },
                messages: [{ message: 'Roles obtenidos exitosamente', errorCode: '0000' }],
            })
        );
    }),
];
```

### 3.3 Test Utilities

**Archivo:** `test-utils.tsx`

```typescript
import { render } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactElement } from 'react';

// Query client de test con retry deshabilitado
export const createTestQueryClient = () => new QueryClient({
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

// Wrapper con providers necesarios
export const AllProviders = ({ children }: { children: React.ReactNode }) => {
    const testQueryClient = createTestQueryClient();

    return (
        <QueryClientProvider client={testQueryClient}>
            {children}
        </QueryClientProvider>
    );
};

// Render helper que incluye providers
export const renderWithProviders = (ui: ReactElement) => {
    return render(ui, { wrapper: AllProviders });
};
```

---

## 4. Tests por Modulo

### 4.1 Components

#### 4.1.1 TemplateTable.test.tsx

**Archivo:** `__tests__/components/TemplateTable.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders table with templates | Unit | Renderiza tabla con datos correctos |
| displays template name and stats | Unit | Muestra nombre, necesidades, precio |
| renders status badge correctly | Unit | Badge verde "Activo" o gris "Inactivo" |
| shows empty state when no data | Unit | Muestra "No hay templates disponibles" |
| handles row click navigation | Integration | Click en fila navega a detalle |
| edit button navigates to edit page | Integration | Click "Editar" navega a /templates/:id/editar |
| toggle status opens dialog | Integration | Click "Desactivar" abre dialogo confirmacion |
| displays loading skeleton | Unit | Muestra skeletons mientras isLoading=true |

**Casos Detallados:**

```typescript
describe('TemplateTable', () => {
    it('renders table with templates', () => {
        // Arrange: Mock data con 2 templates
        const { getByText, getAllByRole } = renderWithProviders(
            <TemplateTable templates={mockTemplateList} isLoading={false} />
        );

        // Assert: Headers y datos visibles
        expect(getByText('Nombre')).toBeInTheDocument();
        expect(getByText('Necesidades')).toBeInTheDocument();
        expect(getByText('Produccion de EP')).toBeInTheDocument();
        expect(getAllByRole('row')).toHaveLength(3); // header + 2 data rows
    });

    it('displays template name and stats', () => {
        const { getByText } = renderWithProviders(
            <TemplateTable templates={mockTemplateList} isLoading={false} />
        );

        expect(getByText('Produccion de EP')).toBeInTheDocument();
        expect(getByText('8')).toBeInTheDocument(); // cantidadNecesidades
        expect(getByText('3,700 - 11,400 EUR')).toBeInTheDocument(); // precio formateado
    });

    it('shows empty state when no data', () => {
        const { getByText, queryByRole } = renderWithProviders(
            <TemplateTable templates={[]} isLoading={false} />
        );

        expect(getByText(/No hay templates disponibles/i)).toBeInTheDocument();
        expect(queryByRole('row')).not.toBeInTheDocument();
    });

    it('displays loading skeleton', () => {
        const { container } = renderWithProviders(
            <TemplateTable templates={[]} isLoading={true} />
        );

        // Verificar presencia de skeleton loaders
        const skeletons = container.querySelectorAll('[data-testid="skeleton"]');
        expect(skeletons.length).toBeGreaterThan(0);
    });
});
```

#### 4.1.2 TemplateFilters.test.tsx

**Archivo:** `__tests__/components/TemplateFilters.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders search input and filters | Unit | Inputs visibles correctamente |
| search input triggers onSearchChange | Unit | Callback llamado con valor correcto |
| status select triggers onStatusChange | Unit | Callback con "active"/"inactive"/"all" |
| debounces search input | Unit | No llama callback hasta 300ms despues |
| clear filters button resets state | Integration | Limpia search y status |

**Casos Detallados:**

```typescript
import { vi } from 'vitest';
import { userEvent } from '@testing-library/user-event';

describe('TemplateFilters', () => {
    it('search input triggers onSearchChange', async () => {
        const handleSearchChange = vi.fn();
        const user = userEvent.setup();

        const { getByPlaceholderText } = renderWithProviders(
            <TemplateFilters
                onSearchChange={handleSearchChange}
                onStatusChange={vi.fn()}
            />
        );

        const searchInput = getByPlaceholderText('Buscar templates...');
        await user.type(searchInput, 'EP');

        // Debounce 300ms
        await vi.waitFor(() => {
            expect(handleSearchChange).toHaveBeenCalledWith('EP');
        }, { timeout: 500 });
    });

    it('status select triggers onStatusChange', async () => {
        const handleStatusChange = vi.fn();
        const user = userEvent.setup();

        const { getByRole } = renderWithProviders(
            <TemplateFilters
                onSearchChange={vi.fn()}
                onStatusChange={handleStatusChange}
            />
        );

        const statusSelect = getByRole('combobox', { name: /estado/i });
        await user.click(statusSelect);
        await user.click(getByText('Activos'));

        expect(handleStatusChange).toHaveBeenCalledWith('active');
    });
});
```

#### 4.1.3 TemplateForm.test.tsx

**Archivo:** `__tests__/components/TemplateForm.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders form with all fields | Unit | Inputs de nombre, descripcion, icono, orden |
| validates required fields | Integration | Muestra errores si nombre vacio |
| pre-fills form in edit mode | Unit | Valores iniciales cargados correctamente |
| handles form submission | Integration | onSubmit llamado con datos correctos |
| add need button appends new item | Integration | Agrega nueva necesidad vacia a lista |
| remove need button deletes item | Integration | Elimina necesidad de la lista |
| icon select shows preview | Unit | Icono seleccionado visible |
| active checkbox toggles state | Unit | Checkbox cambia valor activo |

**Casos Detallados:**

```typescript
describe('TemplateForm', () => {
    it('validates required fields', async () => {
        const handleSubmit = vi.fn();
        const user = userEvent.setup();

        const { getByRole, getByText } = renderWithProviders(
            <TemplateForm onSubmit={handleSubmit} />
        );

        const submitButton = getByRole('button', { name: /guardar/i });
        await user.click(submitButton);

        // Errores de validacion visibles
        expect(getByText('El nombre es obligatorio')).toBeInTheDocument();
        expect(handleSubmit).not.toHaveBeenCalled();
    });

    it('handles form submission', async () => {
        const handleSubmit = vi.fn();
        const user = userEvent.setup();

        const { getByLabelText, getByRole } = renderWithProviders(
            <TemplateForm onSubmit={handleSubmit} />
        );

        await user.type(getByLabelText('Nombre'), 'Nuevo Template');
        await user.type(getByLabelText('Descripcion'), 'Descripcion de prueba');

        const submitButton = getByRole('button', { name: /guardar/i });
        await user.click(submitButton);

        expect(handleSubmit).toHaveBeenCalledWith(
            expect.objectContaining({
                nombre: 'Nuevo Template',
                descripcion: 'Descripcion de prueba',
            })
        );
    });
});
```

#### 4.1.4 NeedFormItem.test.tsx

**Archivo:** `__tests__/components/NeedFormItem.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders need form inputs | Unit | Fase, titulo, rol, presupuestos visibles |
| validates budget min < max | Unit | Muestra error si max < min |
| role select populates from maestras | Integration | Opciones de roles cargadas correctamente |
| priority select has correct options | Unit | "Alta", "Media", "Baja" disponibles |
| delete button triggers onRemove | Unit | Callback llamado con ID correcto |
| budget inputs accept only numbers | Unit | Input type="number" valida correctamente |

**Casos Detallados:**

```typescript
describe('NeedFormItem', () => {
    it('validates budget min < max', async () => {
        const user = userEvent.setup();

        const { getByLabelText, getByText } = renderWithProviders(
            <NeedFormItem
                necesidad={{}}
                roles={mockRolesProfesionales}
                onRemove={vi.fn()}
            />
        );

        await user.type(getByLabelText('Presupuesto minimo'), '1000');
        await user.type(getByLabelText('Presupuesto maximo'), '500');
        await user.tab(); // Trigger blur validation

        expect(getByText('El maximo debe ser mayor que el minimo')).toBeInTheDocument();
    });
});
```

#### 4.1.5 TemplateStatusDialog.test.tsx

**Archivo:** `__tests__/components/TemplateStatusDialog.test.tsx`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| renders dialog with confirmation message | Unit | Mensaje "Desactivar template?" visible |
| confirm button triggers onConfirm | Unit | Callback llamado al confirmar |
| cancel button closes dialog | Unit | Dialog cerrado sin llamar onConfirm |
| shows loading state during mutation | Unit | Boton disabled con spinner |

---

### 4.2 Hooks

#### 4.2.1 useTemplates.test.ts

**Archivo:** `__tests__/hooks/useTemplates.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches templates successfully | Unit | Retorna lista de templates |
| handles loading state | Unit | isLoading true inicialmente |
| handles error state | Unit | error cuando API falla |
| uses correct query key | Unit | Query key: ['crowdsourcing', 'templates'] |
| filters templates by status | Integration | Query param ?status=active/inactive |

**Setup:**

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { setupServer } from 'msw/node';
import { templateHandlers } from '../__mocks__/handlers';
import { AllProviders } from '@/test-utils';

const server = setupServer(...templateHandlers);

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

describe('useTemplates', () => {
    it('fetches templates successfully', async () => {
        const { result } = renderHook(() => useTemplates(), {
            wrapper: AllProviders
        });

        await waitFor(() => expect(result.current.isSuccess).toBe(true));

        expect(result.current.data).toHaveLength(2);
        expect(result.current.data?.[0].nombre).toBe('Produccion de EP');
    });

    it('handles loading state', () => {
        const { result } = renderHook(() => useTemplates(), {
            wrapper: AllProviders
        });

        expect(result.current.isLoading).toBe(true);
    });

    it('handles error state', async () => {
        server.use(
            rest.get('/api/crowdsourcing/templates', (req, res, ctx) => {
                return res(ctx.status(500), ctx.json({ error: 'Server error' }));
            })
        );

        const { result } = renderHook(() => useTemplates(), {
            wrapper: AllProviders
        });

        await waitFor(() => expect(result.current.isError).toBe(true));
    });
});
```

#### 4.2.2 useTemplateDetail.test.ts

**Archivo:** `__tests__/hooks/useTemplateDetail.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches template by id | Unit | Retorna template completo con necesidades |
| returns null when not found | Unit | error cuando template no existe (404) |
| uses correct query key with id | Unit | Query key: ['crowdsourcing', 'templates', id] |

#### 4.2.3 useCreateTemplate.test.ts

**Archivo:** `__tests__/hooks/useCreateTemplate.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| creates template successfully | Unit | Retorna nuevo ID |
| invalidates templates query | Integration | Query cache invalidado despues de crear |
| handles validation errors | Unit | Muestra errores de backend (400) |
| shows toast on success | Integration | Toast verde "Template creado" |

**Casos Detallados:**

```typescript
import { renderHook, waitFor } from '@testing-library/react';
import { vi } from 'vitest';

describe('useCreateTemplate', () => {
    it('creates template successfully', async () => {
        const { result } = renderHook(() => useCreateTemplate(), {
            wrapper: AllProviders
        });

        const newTemplate = {
            nombre: 'Nuevo Template',
            descripcion: 'Test',
            icono: 'music',
            orden: 1,
            activo: true,
            necesidades: [],
        };

        result.current.mutate(newTemplate);

        await waitFor(() => expect(result.current.isSuccess).toBe(true));
        expect(result.current.data).toBe('123e4567-e89b-12d3-a456-426614174000');
    });

    it('invalidates templates query', async () => {
        const queryClient = createTestQueryClient();
        const invalidateSpy = vi.spyOn(queryClient, 'invalidateQueries');

        const { result } = renderHook(() => useCreateTemplate(), {
            wrapper: ({ children }) => (
                <QueryClientProvider client={queryClient}>
                    {children}
                </QueryClientProvider>
            )
        });

        result.current.mutate({ nombre: 'Test' });

        await waitFor(() => {
            expect(invalidateSpy).toHaveBeenCalledWith(['crowdsourcing', 'templates']);
        });
    });
});
```

#### 4.2.4 useUpdateTemplate.test.ts

**Archivo:** `__tests__/hooks/useUpdateTemplate.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| updates template successfully | Unit | Retorna ID actualizado |
| invalidates specific template query | Integration | Invalida query del template especifico |
| handles not found error | Unit | Error 404 cuando template no existe |

#### 4.2.5 useToggleTemplateStatus.test.ts

**Archivo:** `__tests__/hooks/useToggleTemplateStatus.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| toggles status successfully | Unit | Retorna nuevo estado |
| invalidates templates list | Integration | Lista de templates se recarga |
| shows confirmation dialog | Integration | Dialog visible antes de toggle |

#### 4.2.6 useRolesProfesionales.test.ts

**Archivo:** `__tests__/hooks/useRolesProfesionales.test.ts`

| Test Case | Tipo | Descripcion |
|-----------|------|-------------|
| fetches roles successfully | Unit | Retorna lista de roles con categoria |
| caches roles for 10 minutes | Unit | staleTime configurado correctamente |
| groups roles by categoria | Unit | Roles agrupados por categoriaRol |

---

### 4.3 Integration Tests

#### 4.3.1 template-crud-flow.test.tsx

**Archivo:** `__tests__/integration/template-crud-flow.test.tsx`

**Descripcion:** Flujo completo de CRUD desde listado hasta edicion.

| Test Case | Descripcion |
|-----------|-------------|
| complete create flow | Navega a /nuevo -> llena form -> submit -> redirect a listado -> nuevo template visible |
| complete edit flow | Click "Editar" en tabla -> form pre-llenado -> modifica campos -> submit -> toast de exito -> cambios reflejados en tabla |
| delete flow with confirmation | Click "Desactivar" -> dialog confirmacion -> confirma -> template marcado como inactivo en tabla |
| validation prevents submission | Intenta guardar sin nombre -> errores visibles -> submit bloqueado |

**Caso Detallado:**

```typescript
describe('Template CRUD Flow', () => {
    it('complete create flow', async () => {
        const user = userEvent.setup();

        const { getByRole, getByText, getByLabelText } = render(
            <TemplatesPage />,
            { wrapper: AllProviders }
        );

        // 1. Click "Nuevo Template"
        await user.click(getByRole('button', { name: /nuevo template/i }));

        // 2. Llenar formulario
        await user.type(getByLabelText('Nombre'), 'Template de Prueba');
        await user.type(getByLabelText('Descripcion'), 'Descripcion de test');
        await user.selectOptions(getByLabelText('Icono'), 'music');

        // 3. Agregar necesidad
        await user.click(getByRole('button', { name: /agregar necesidad/i }));
        await user.type(getByLabelText('Titulo'), 'Necesidad 1');
        await user.selectOptions(getByLabelText('Rol'), '1'); // Productor

        // 4. Submit
        await user.click(getByRole('button', { name: /guardar template/i }));

        // 5. Verificar toast y redirect
        await waitFor(() => {
            expect(getByText('Template creado exitosamente')).toBeInTheDocument();
        });

        // 6. Verificar nuevo template en tabla
        await waitFor(() => {
            expect(getByText('Template de Prueba')).toBeInTheDocument();
        });
    });
});
```

#### 4.3.2 template-filtering.test.tsx

**Archivo:** `__tests__/integration/template-filtering.test.tsx`

**Descripcion:** Flujo de filtrado y busqueda en tabla.

| Test Case | Descripcion |
|-----------|-------------|
| search filters table | Escribe "EP" en search -> solo templates con "EP" visibles |
| status filter shows correct items | Selecciona "Inactivos" -> solo templates inactivos |
| combined filters work together | Busqueda + filtro de status aplicados simultaneamente |
| clear filters resets table | Click "Limpiar filtros" -> todos los templates visibles |

---

## 5. Cobertura por Archivo

| Archivo | Lineas | Funciones | Branches |
|---------|--------|-----------|----------|
| TemplateTable.tsx | 90% | 95% | 85% |
| TemplateFilters.tsx | 85% | 90% | 80% |
| TemplateForm.tsx | 85% | 90% | 85% |
| NeedFormItem.tsx | 90% | 95% | 85% |
| TemplateStatusDialog.tsx | 95% | 100% | 90% |
| useTemplates.ts | 95% | 100% | 90% |
| useTemplateDetail.ts | 95% | 100% | 90% |
| useCreateTemplate.ts | 90% | 100% | 85% |
| useUpdateTemplate.ts | 90% | 100% | 85% |
| useToggleTemplateStatus.ts | 90% | 100% | 85% |
| useRolesProfesionales.ts | 95% | 100% | 90% |
| templates.service.ts | 95% | 100% | 90% |

**Meta Global:** 80%+ en todas las metricas

---

## 6. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del feature
npm run test -- crowdsourcing

# Ejecutar con coverage
npm run test:coverage -- crowdsourcing

# Ejecutar solo tests de componentes
npm run test -- __tests__/components

# Ejecutar solo tests de hooks
npm run test -- __tests__/hooks

# Ejecutar solo integration tests
npm run test -- __tests__/integration

# Watch mode (desarrollo)
npm run test:watch -- crowdsourcing
```

---

## 7. CI/CD Integration

```yaml
- name: Run Admin Tests
  run: |
    cd src/admin
    npm run test:coverage -- crowdsourcing

- name: Check Coverage Threshold
  run: |
    npm run test:coverage -- --coverage.threshold.lines=80

- name: Upload Coverage to Codecov
  uses: codecov/codecov-action@v3
  with:
    files: ./src/admin/coverage/coverage-final.json
    flags: admin-crowdsourcing
```

---

## 8. Dependencias de Testing

### 8.1 Packages Requeridos

| Package | Version | Uso |
|---------|---------|-----|
| vitest | ^1.0.0 | Test runner |
| @testing-library/react | ^14.0.0 | Testing components |
| @testing-library/user-event | ^14.0.0 | User interactions |
| @testing-library/jest-dom | ^6.0.0 | Custom matchers |
| msw | ^2.0.0 | API mocking |
| @tanstack/react-query | ^5.0.0 | Query testing utilities |

### 8.2 Setup Global

**Archivo:** `vitest.config.ts`

```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
    plugins: [react()],
    test: {
        globals: true,
        environment: 'jsdom',
        setupFiles: ['./src/__tests__/setup.ts'],
        coverage: {
            provider: 'v8',
            reporter: ['text', 'json', 'html'],
            exclude: [
                'node_modules/',
                'src/__tests__/',
                '**/*.d.ts',
                '**/*.config.*',
                '**/mockData/*',
            ],
            all: true,
            lines: 80,
            functions: 80,
            branches: 80,
            statements: 80,
        },
    },
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
});
```

**Archivo:** `src/__tests__/setup.ts`

```typescript
import '@testing-library/jest-dom';
import { setupServer } from 'msw/node';
import { templateHandlers } from './features/crowdsourcing/__mocks__/handlers';

// Setup MSW server
export const server = setupServer(...templateHandlers);

beforeAll(() => server.listen({ onUnhandledRequest: 'error' }));
afterEach(() => server.resetHandlers());
afterAll(() => server.close());

// Mock window.matchMedia (for responsive components)
Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: vi.fn().mockImplementation(query => ({
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

// Mock IntersectionObserver (for lazy loading)
global.IntersectionObserver = class IntersectionObserver {
    constructor() {}
    disconnect() {}
    observe() {}
    takeRecords() { return []; }
    unobserve() {}
};
```

---

## 9. Edge Cases y Scenarios Especiales

### 9.1 Error Handling

| Scenario | Test | Comportamiento Esperado |
|----------|------|-------------------------|
| API retorna 500 | Error state en useTemplates | Toast rojo "Error cargando templates" |
| Template not found (404) | useTemplateDetail error | Redirect a listado con toast "Template no encontrado" |
| Validation error (400) | Form submission fails | Errores mostrados debajo de inputs |
| Network timeout | Query retry agotado | Toast "Error de conexion. Intenta de nuevo." |
| Token expirado (401) | API retorna Unauthorized | Redirect a /login |

### 9.2 Boundary Cases

| Caso | Test | Validacion |
|------|------|-----------|
| Template sin necesidades | Renderiza form vacio | Lista de necesidades vacia, puede guardar |
| Presupuesto min = max | Validacion acepta | No error si min == max |
| Nombre con 200 caracteres | Validacion maxLength | Acepta hasta 200, rechaza 201+ |
| 0 templates en DB | Empty state | Muestra mensaje y boton "Crear primero" |
| 100+ templates | Paginacion | Solo muestra 20 por pagina, paginacion funcional |

### 9.3 User Interactions

| Interaccion | Test | Comportamiento |
|-------------|------|----------------|
| Click "Editar" durante loading | Button disabled | No navega hasta que datos carguen |
| Submit form con Enter | Form submission | Se envia igual que click en boton |
| Escape en dialog | Dialog closes | Cierra sin ejecutar accion |
| Doble click en "Guardar" | Prevent double submit | Boton disabled despues del primer click |
| Tab navigation | Focus order correcto | Tab recorre inputs en orden logico |

---

## 10. Checklist de Implementacion

### Estructura
- [ ] Carpeta `__tests__/` creada en feature root
- [ ] Subcarpetas `components/`, `hooks/`, `integration/`, `__mocks__/` creadas
- [ ] Archivo `test-utils.tsx` con helpers compartidos

### Mocks
- [ ] `templates.mock.ts` con 5+ templates de prueba
- [ ] `roles.mock.ts` con roles profesionales y categorias
- [ ] `handlers.ts` con MSW handlers para todos los endpoints
- [ ] Mock data cubre casos: activo, inactivo, sin necesidades, con muchas necesidades

### Component Tests
- [ ] TemplateTable.test.tsx (8 tests)
- [ ] TemplateFilters.test.tsx (5 tests)
- [ ] TemplateForm.test.tsx (8 tests)
- [ ] NeedFormItem.test.tsx (6 tests)
- [ ] TemplateStatusDialog.test.tsx (4 tests)
- [ ] TemplateBadges.test.tsx (3 tests - opcional)

### Hook Tests
- [ ] useTemplates.test.ts (5 tests)
- [ ] useTemplateDetail.test.ts (3 tests)
- [ ] useCreateTemplate.test.ts (4 tests)
- [ ] useUpdateTemplate.test.ts (3 tests)
- [ ] useToggleTemplateStatus.test.ts (3 tests)
- [ ] useRolesProfesionales.test.ts (3 tests)

### Integration Tests
- [ ] template-crud-flow.test.tsx (4 tests)
- [ ] template-filtering.test.tsx (4 tests)

### Config
- [ ] vitest.config.ts configurado con coverage thresholds
- [ ] setup.ts con MSW server y mocks globales
- [ ] package.json scripts: test, test:coverage, test:watch

### Coverage
- [ ] Todas las metricas >= 80%
- [ ] Coverage report generado en CI
- [ ] No warnings de uncovered branches criticos

---

## 11. Proximos Pasos

1. **Implementar mocks:** Crear archivos en `__mocks__/` con datos de prueba
2. **Setup MSW:** Configurar handlers para todos los endpoints
3. **Component tests:** Empezar con TemplateTable (mas critico)
4. **Hook tests:** Testear queries y mutations
5. **Integration tests:** Flujos E2E dentro del feature
6. **Refinar coverage:** Ajustar tests para llegar a 80%+

---

**Documento creado:** 2026-02-15
**Autor:** Claude Code (Testing Strategist)
**Estado:** READY FOR IMPLEMENTATION

**Nota:** Este plan cubre SOLO la parte de Admin (CRUD de templates). El wizard de generacion de necesidades esta en Landing/Web y requiere un plan de testing separado.
