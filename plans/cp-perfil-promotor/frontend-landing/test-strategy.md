# Estrategia de Testing: Perfil de Promotor (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/web
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Cobertura Estimada |
|------|----------|--------------------|
| Unit Tests (schemas Zod) | 18 | 95% schemas |
| Unit Tests (components) | 32 | 85% componentes |
| Unit Tests (hooks - queries) | 12 | 90% queries |
| Unit Tests (hooks - mutations) | 16 | 90% mutations |
| Integration Tests (flujos completos) | 8 | 80% flujos criticos |
| **Total** | **86** | **80%+** |

### Distribucion por Archivo de Test

| Archivo de Test | Tests | Tipo |
|-----------------|-------|------|
| `createPromotorSchema.test.ts` | 14 | Unit |
| `updatePromotorSchema.test.ts` | 10 | Unit |
| `PromotorForm.test.tsx` | 22 | Unit + Integration |
| `PromotorRegistroPage.test.tsx` | 12 | Integration |
| `PromotorPerfilPage.test.tsx` | 10 | Integration |
| `PromotorDashboardPage.test.tsx` | 10 | Unit |
| `PromotorDeactivateDialog.test.tsx` | 8 | Unit + Integration |
| `usePromotor.test.ts` | 6 | Unit |
| `useCreatePromotor.test.ts` | 7 | Unit + Integration |
| `useUpdatePromotor.test.ts` | 5 | Unit |
| `useDesactivarPromotor.test.ts` | 6 | Unit + Integration |
| **Total** | **110** | — |

> Nota: el recuento exacto puede crecer al detallar edge cases durante la implementacion. El objetivo de cobertura del 80% prevalece sobre el recuento exacto de tests.

---

## 2. Estructura de Tests

```
src/web/src/features/crowdpromotion/
├── __mocks__/
│   └── promotor.mock.ts                  # Fixtures: Promotor, TipoPromotor, requests
├── __tests__/
│   ├── schemas/
│   │   ├── createPromotorSchema.test.ts
│   │   └── updatePromotorSchema.test.ts
│   ├── components/
│   │   ├── PromotorForm.test.tsx
│   │   ├── PromotorDashboardPage.test.tsx
│   │   └── PromotorDeactivateDialog.test.tsx
│   ├── pages/
│   │   ├── PromotorRegistroPage.test.tsx
│   │   └── PromotorPerfilPage.test.tsx
│   └── hooks/
│       ├── usePromotor.test.ts
│       ├── useCreatePromotor.test.ts
│       ├── useUpdatePromotor.test.ts
│       └── useDesactivarPromotor.test.ts
├── domain/
├── application/
├── infrastructure/
└── presentation/
```

---

## 3. Mocks y Fixtures

### 3.1 Mock Data

**Archivo:** `src/web/src/features/crowdpromotion/__mocks__/promotor.mock.ts`

```typescript
import type {
    Promotor,
    PromotorCreatedResult,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
    TipoPromotor,
    CreatePromotorRequest,
    UpdatePromotorRequest,
} from '@shared/types/crowdpromotion';

// --- Tipos de Promotor (seed data) ---

export const mockTiposPromotor: TipoPromotor[] = [
    { id: 1, nombre: 'Fan Embajador', descripcion: 'Fan que promueve artistas por pasion' },
    { id: 2, nombre: 'Influencer', descripcion: 'Creador de contenido con audiencia' },
    { id: 3, nombre: 'Medio / Blog', descripcion: 'Medio de comunicacion o blog musical' },
    { id: 4, nombre: 'Profesional Marketing', descripcion: 'Profesional del marketing digital' },
];

// --- Promotor base (activo, con programas y comisiones) ---

export const mockPromotor: Promotor = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    nombrePublico: 'DJ Marketing Pro',
    tipoPromotorId: 2,
    tipoPromotorNombre: 'Influencer',
    emailContacto: 'contacto@djmarketing.com',
    urlSitioWeb: 'https://djmarketing.com',
    urlInstagram: 'https://instagram.com/djmarketing',
    urlTikTok: 'https://tiktok.com/@djmarketing',
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: '2026-02-25T10:00:00Z',
    totalProgramasActivos: 3,
    totalComisionesGanadas: 150.50,
    monedaComisiones: 'EUR',
};

// --- Promotor sin redes sociales (solo campos obligatorios) ---

export const mockPromotorMinimo: Promotor = {
    id: '4ab96g75-6828-5673-c4gd-3d074g77bgb7',
    nombrePublico: 'Fan Embajador Test',
    tipoPromotorId: 1,
    tipoPromotorNombre: 'Fan Embajador',
    emailContacto: null,
    urlSitioWeb: null,
    urlInstagram: null,
    urlTikTok: null,
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: '2026-02-25T11:00:00Z',
    totalProgramasActivos: 0,
    totalComisionesGanadas: 0,
    monedaComisiones: 'EUR',
};

// --- Promotor desactivado ---

export const mockPromotorInactivo: Promotor = {
    ...mockPromotor,
    esActivo: false,
    totalProgramasActivos: 0,
};

// --- Resultados de operaciones ---

export const mockPromotorCreatedResult: PromotorCreatedResult = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    nombrePublico: 'DJ Marketing Pro',
    tipoPromotorNombre: 'Influencer',
    esActivo: true,
    fechaCreacion: '2026-02-25T10:00:00Z',
};

export const mockPromotorUpdatedResult: PromotorUpdatedResult = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    nombrePublico: 'DJ Marketing Pro (Updated)',
    fechaActualizacion: '2026-02-25T12:00:00Z',
};

export const mockPromotorDesactivadoResult: PromotorDesactivadoResult = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    esActivo: false,
    programasDadosDeBaja: 2,
};

export const mockPromotorDesactivadoResultSinProgramas: PromotorDesactivadoResult = {
    id: '3fa85f64-5717-4562-b3fc-2c963f66afa6',
    esActivo: false,
    programasDadosDeBaja: 0,
};

// --- Requests ---

export const mockCreatePromotorRequest: CreatePromotorRequest = {
    nombrePublico: 'DJ Marketing Pro',
    tipoPromotorId: 2,
    emailContacto: 'contacto@djmarketing.com',
    urlSitioWeb: 'https://djmarketing.com',
    urlInstagram: 'https://instagram.com/djmarketing',
    urlTikTok: 'https://tiktok.com/@djmarketing',
    urlYouTube: undefined,
    urlTwitter: undefined,
};

export const mockCreatePromotorRequestMinimo: CreatePromotorRequest = {
    nombrePublico: 'Fan Test',
    tipoPromotorId: 1,
};

export const mockUpdatePromotorRequest: UpdatePromotorRequest = {
    nombrePublico: 'DJ Marketing Pro (Updated)',
    emailContacto: 'nuevo@djmarketing.com',
    urlSitioWeb: 'https://djmarketing.com',
    urlInstagram: 'https://instagram.com/djmarketing',
    urlTikTok: undefined,
    urlYouTube: 'https://youtube.com/@djmarketing',
    urlTwitter: undefined,
};

// --- Mock user autenticado (Zustand auth store) ---

export const mockAuthUser = {
    id: 'user-001',
    email: 'usuario1@mail.com',
    nombreCompleto: 'Usuario Test',
};
```

### 3.2 Estrategia de Mocking del Service

El proyecto usa mocks directos de vi.mock sobre el modulo del service (patron establecido en campanias y backings). **No se usa MSW** en el nivel de tests unitarios/integracion - se mockea el service directamente.

**Patron a seguir (consistente con el proyecto):**

```typescript
// En el test de hooks:
vi.mock('../../infrastructure/promotor.service', () => ({
    promotorService: {
        getMe: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}));

import { promotorService } from '../../infrastructure/promotor.service';

// Uso en cada test:
vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor);
```

**Patron para sonner (toast):**

```typescript
vi.mock('sonner', () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}));
```

**Patron para react-router-dom navigate:**

```typescript
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});
```

**Patron para Zustand auth-store:**

```typescript
vi.mock('@/store/auth-store', () => ({
    useAuthStore: vi.fn(),
}));

import { useAuthStore } from '@/store/auth-store';

// En beforeEach o en cada test:
vi.mocked(useAuthStore).mockReturnValue({
    user: mockAuthUser,
    token: 'mock-jwt-token',
    isAuthenticated: true,
    login: vi.fn(),
    logout: vi.fn(),
    setUser: vi.fn(),
});
```

### 3.3 Wrapper de Providers para Tests

**Patron createWrapper (consistente con campanias/__tests__/hooks):**

```typescript
// Definir en cada archivo de test de hooks o inline para integracion

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    });

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client: queryClient },
            React.createElement(MemoryRouter, null, children)
        );
    };
};
```

**renderWithProviders para componentes/paginas:**

```typescript
function renderWithProviders(
    ui: React.ReactElement,
    {
        initialEntries = ['/'],
        queryClient,
    }: { initialEntries?: string[]; queryClient?: QueryClient } = {}
) {
    const client = queryClient ?? new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    });

    return render(
        <QueryClientProvider client={client}>
            <MemoryRouter initialEntries={initialEntries}>
                {ui}
            </MemoryRouter>
        </QueryClientProvider>
    );
}
```

---

## 4. Tests por Modulo

### 4.1 Schemas Zod

#### createPromotorSchema.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/schemas/createPromotorSchema.test.ts`

**Referencia del schema:** `src/shared/schemas/crowdpromotion.schema.ts` - `createPromotorSchema`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | validates valid full request | Unit | Todos los campos validos pasan la validacion |
| 2 | validates minimal request | Unit | Solo nombrePublico + tipoPromotorId validos |
| 3 | requires nombrePublico | Unit | `safeParse` con nombrePublico vacio retorna error con mensaje 'El nombre publico es obligatorio' |
| 4 | rejects nombrePublico shorter than 3 chars | Unit | "AB" retorna error con mensaje 'El nombre debe tener al menos 3 caracteres' |
| 5 | rejects nombrePublico longer than 200 chars | Unit | String de 201 chars retorna error con mensaje 'Maximo 200 caracteres' |
| 6 | requires tipoPromotorId | Unit | `tipoPromotorId: 0` retorna error 'El tipo de promotor es obligatorio' |
| 7 | rejects tipoPromotorId below 1 | Unit | `tipoPromotorId: 0` falla con min(1) |
| 8 | accepts valid emailContacto | Unit | Email bien formado no genera error |
| 9 | rejects invalid emailContacto format | Unit | 'no-es-email' retorna error de formato |
| 10 | allows empty string for emailContacto | Unit | `''` pasa (`.or(z.literal(''))`) |
| 11 | allows undefined emailContacto | Unit | Campo omitido pasa validacion |
| 12 | rejects invalid URL in urlInstagram | Unit | 'not-a-url' retorna error de formato URL |
| 13 | allows empty string for URL fields | Unit | `''` pasa en todos los campos URL |
| 14 | rejects URL longer than 300 chars | Unit | URL de 301 chars retorna error de longitud |

**Notas de implementacion:**
- Importar desde `@shared/schemas/crowdpromotion.schema` (alias `@shared` configurado en vite.config.ts).
- Para cada campo URL, verificar al menos `urlInstagram` como campo representativo; no es necesario repetir la misma suite para cada red social (excepto si hay logica diferente por campo).
- La combinacion `.url().optional().or(z.literal(''))` debe testearse explicitamente: URL valida pasa, URL invalida falla, string vacio pasa, undefined pasa.

#### updatePromotorSchema.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/schemas/updatePromotorSchema.test.ts`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | validates valid update request | Unit | Todos los campos editables validos pasan |
| 2 | requires nombrePublico | Unit | Campo vacio retorna error |
| 3 | rejects nombrePublico shorter than 3 chars | Unit | Igual que createPromotorSchema |
| 4 | rejects nombrePublico longer than 200 chars | Unit | Igual que createPromotorSchema |
| 5 | does NOT include tipoPromotorId field | Unit | Schema no tiene tipoPromotorId; si se incluye en el parse, no debe fallar (campo extra ignorado) |
| 6 | accepts valid emailContacto | Unit | Email valido pasa |
| 7 | rejects invalid emailContacto | Unit | Formato invalido retorna error |
| 8 | rejects invalid URL field | Unit | URL invalida en urlSitioWeb retorna error |
| 9 | allows all URL fields empty | Unit | Todos los campos URL omitidos - validacion exitosa |
| 10 | rejects emailContacto over 200 chars | Unit | String de 201 chars falla con max(200) |

---

### 4.2 Componentes

#### PromotorForm.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/components/PromotorForm.test.tsx`

**Descripcion del componente:** Formulario compartido que se usa tanto en registro (modo `create`) como en edicion (modo `edit`). En modo `edit`, `tipoPromotorId` se muestra como campo read-only.

**Props del componente:**
```typescript
interface PromotorFormProps {
    mode: 'create' | 'edit';
    defaultValues?: Partial<CreatePromotorFormData | UpdatePromotorFormData>;
    tipoPromotorNombre?: string;  // Solo en modo edit - valor read-only
    onSubmit: (data: CreatePromotorFormData | UpdatePromotorFormData) => void;
    isSubmitting?: boolean;
}
```

**Mocks necesarios:** Ninguno - el componente es presentacional puro, sin dependencias de servicios o queries.

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| **Modo Create - Render** | | | |
| 1 | renders all fields in create mode | Unit | Nombre publico, selector tipo promotor, email, URLs de redes - todos visibles |
| 2 | shows tipoPromotorId select in create mode | Unit | El selector de tipo promotor es interactuable (no read-only) |
| 3 | populates tipo promotor options from TIPO_PROMOTOR_LABELS | Unit | Los 4 tipos de promotor aparecen como opciones en el select |
| 4 | shows optional badge on non-required fields | Unit | Email y URLs muestran indicacion de opcionales |
| 5 | shows aviso informativo when no social URL is filled (FA-03) | Unit | Aviso de recomendacion visible cuando todos los campos URL estan vacios |
| **Modo Edit - Render** | | | |
| 6 | renders tipo promotor as read-only text in edit mode | Unit | En modo `edit`, el tipo promotor se muestra como texto, no como select |
| 7 | pre-fills form with defaultValues in edit mode | Unit | Los campos se rellenan con los valores recibidos por props |
| 8 | does NOT show tipoPromotorId select in edit mode | Unit | El selector no existe en el DOM en modo edit |
| **Validaciones visuales** | | | |
| 9 | shows required error when nombrePublico is empty on submit | Integration | Click en submit sin nombre - error visible en campo |
| 10 | shows min-length error when nombrePublico has 2 chars | Integration | Escribe 'AB', submit - error 'al menos 3 caracteres' |
| 11 | shows URL format error when Instagram URL is invalid | Integration | Escribe 'no-es-url' en Instagram, tab fuera del campo - error de formato |
| 12 | shows email format error when emailContacto is invalid | Integration | Escribe 'email-invalido', submit - error de email |
| 13 | clears field error when user corrects the input | Integration | Error presente, usuario corrige el campo - error desaparece |
| **Interaccion** | | | |
| 14 | hides social URL aviso when user fills at least one URL | Integration | Usuario rellena Instagram - aviso FA-03 desaparece |
| 15 | calls onSubmit with correct data on valid create | Integration | Rellena campos validos, click submit, verifica llamada onSubmit con datos correctos |
| 16 | calls onSubmit with correct data on valid edit | Integration | Pre-rellena con defaultValues, modifica nombre, click submit, verifica datos |
| 17 | converts empty URL strings to undefined before onSubmit | Integration | Deja URL vacia, verifica que onSubmit no recibe la clave o recibe undefined |
| **Estados del boton** | | | |
| 18 | shows 'Crear perfil' submit text in create mode | Unit | Texto del boton correcto segun modo |
| 19 | shows 'Guardar cambios' submit text in edit mode | Unit | Texto del boton correcto segun modo |
| 20 | shows loading state when isSubmitting is true | Unit | Boton muestra 'Guardando...' y esta deshabilitado |
| 21 | disables submit button when isSubmitting is true | Unit | `disabled` atributo presente |
| **Accesibilidad** | | | |
| 22 | all inputs have associated labels | Unit | `htmlFor` + `id` correctos para cada campo |

**Setup tipico para tests de interaccion (userEvent):**
```typescript
const user = userEvent.setup();
render(<PromotorForm mode="create" onSubmit={vi.fn()} />);
const input = screen.getByLabelText('Nombre publico');
await user.type(input, 'Mi nombre');
```

---

#### PromotorDashboardPage.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/components/PromotorDashboardPage.test.tsx`

**Descripcion del componente:** Dashboard que muestra KPIs del promotor: programas activos, comisiones ganadas y estado de la cuenta. Consume `usePromotor` internamente.

**Mocks necesarios:**
- `../../infrastructure/promotor.service` (via mock del hook `usePromotor`)
- `@/store/auth-store` para datos del usuario

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | shows loading skeleton while data is fetching | Unit | Estado loading - skeletons visibles, datos no |
| 2 | shows promotor nombre in welcome message | Unit | `data: mockPromotor` - nombrePublico visible en saludo |
| 3 | shows totalProgramasActivos KPI | Unit | Numero de programas activos en la tarjeta KPI correspondiente |
| 4 | shows totalComisionesGanadas KPI formatted as EUR | Unit | '150,50 EUR' o formato locale equivalente visible |
| 5 | shows 'Perfil activo' badge when esActivo is true | Unit | Badge verde/positivo visible |
| 6 | shows 'Perfil inactivo' badge when esActivo is false | Unit | Badge rojo/negativo visible cuando promotor inactivo |
| 7 | shows link/button to edit profile (/promotor/perfil) | Unit | Link o boton de edicion apuntando a la ruta correcta |
| 8 | shows 'Desactivar cuenta' button | Unit | Boton para iniciar flujo de desactivacion |
| 9 | shows error state when query fails | Unit | Mensaje de error visible cuando `isError: true` |
| 10 | shows zero state for KPIs when promotor has no activity | Unit | Con `mockPromotorMinimo`: programas=0, comisiones=0 EUR - renderiza sin errores |

---

#### PromotorDeactivateDialog.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/components/PromotorDeactivateDialog.test.tsx`

**Descripcion del componente:** Dialog de confirmacion antes de desactivar el perfil. Muestra el numero de programas activos que se veran afectados (si hay).

**Props del componente:**
```typescript
interface PromotorDeactivateDialogProps {
    open: boolean;
    programasActivos: number;
    isLoading?: boolean;
    onConfirm: () => void;
    onCancel: () => void;
}
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders nothing when open is false | Unit | Dialog no visible cuando `open: false` |
| 2 | renders dialog when open is true | Unit | Dialog visible con titulo y botones |
| 3 | shows program count when programasActivos > 0 | Unit | 'Se daran de baja 3 programas activos' visible |
| 4 | shows simple confirmation when programasActivos is 0 | Unit | Mensaje simple sin mencionar programas |
| 5 | calls onConfirm when confirm button is clicked | Integration | Click en 'Confirmar' - `onConfirm` llamado 1 vez |
| 6 | calls onCancel when cancel button is clicked | Integration | Click en 'Cancelar' - `onCancel` llamado 1 vez |
| 7 | disables buttons when isLoading is true | Unit | Ambos botones con `disabled` durante la operacion |
| 8 | shows loading indicator in confirm button when isLoading | Unit | Spinner o texto 'Desactivando...' visible |

---

### 4.3 Paginas (Integration Tests)

#### PromotorRegistroPage.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/pages/PromotorRegistroPage.test.tsx`

**Descripcion:** Pagina `/promotor/registro`. Implementa el guard de redireccion: si el usuario ya tiene perfil (query GET /me retorna datos), redirige a `/promotor/dashboard`. Si la query retorna 404/null, muestra el formulario.

**Mocks necesarios:**
- `../../infrastructure/promotor.service` (para controlar estado de `usePromotor` y `useCreatePromotor`)
- `@/store/auth-store` (usuario autenticado)
- `react-router-dom` (`useNavigate`)
- `sonner` (`toast.success`, `toast.error`)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| **Guard de redireccion** | | | |
| 1 | redirects to /promotor/dashboard when promotor profile already exists (FA-01) | Integration | `getMe` retorna `mockPromotor` - se navega a `/promotor/dashboard` |
| 2 | shows registration form when promotor profile does not exist | Integration | `getMe` rechaza con 404 - formulario de registro visible |
| 3 | shows loading state while checking existing profile | Integration | Durante la query pendiente - skeleton o spinner visible, formulario no |
| **Flujo de registro exitoso** | | | |
| 4 | shows toast success and navigates to dashboard on successful create | Integration | Rellena nombre + tipo, submit, `create` resuelve con `mockPromotorCreatedResult` - toast 'Perfil de promotor creado correctamente' + navigate a `/promotor/dashboard` |
| 5 | invalidates promotor query on successful create | Integration | Tras exito de mutation, `queryClient.invalidateQueries` llamado con `QUERY_KEYS.crowdpromotion.promotor.me` |
| **Manejo de errores** | | | |
| 6 | shows toast error and keeps form data on network error (FA-05) | Integration | `create` rechaza con error - toast de error visible, formulario con datos intactos |
| 7 | shows toast error when promotor already exists (errorCode 4018) | Integration | Backend retorna error 4018 - toast con mensaje de duplicado |
| **Acceso sin sesion** | | | |
| 8 | redirects to /auth/login when user is not authenticated | Integration | `isAuthenticated: false` en auth store - navigate a `/auth/login` |
| **Flujo alternativo FA-03** | | | |
| 9 | shows aviso when no social URL is provided before submit | Integration | Usuario no rellena URLs de redes, aviso informativo visible en formulario |
| **Validaciones de formulario en pagina** | | | |
| 10 | does not submit when tipoPromotorId is not selected | Integration | Submit sin seleccionar tipo - error de validacion en campo, `create` no llamado |
| 11 | submits with only required fields | Integration | Solo nombre + tipo - submit valido, `create` llamado con campos opcionales como undefined |
| 12 | preserves form state after failed mutation | Integration | Error de red, los valores del formulario siguen presentes |

---

#### PromotorPerfilPage.test.tsx

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/pages/PromotorPerfilPage.test.tsx`

**Descripcion:** Pagina `/promotor/perfil`. Carga el perfil existente, pre-rellena el formulario y permite editar. Campo `tipoPromotorId` es read-only (muestra nombre del tipo, no selector).

**Mocks necesarios:**
- `../../infrastructure/promotor.service`
- `@/store/auth-store`
- `react-router-dom` (`useNavigate`)
- `sonner` (`toast.success`, `toast.error`)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | shows loading skeleton while fetching profile | Integration | `getMe` pendiente - skeleton visible |
| 2 | pre-fills form with current profile data | Integration | `getMe` resuelve con `mockPromotor` - campos rellenados con sus valores actuales |
| 3 | shows tipoPromotorNombre as read-only text, not select | Integration | 'Influencer' visible como texto, no hay select de tipo promotor en el DOM |
| 4 | shows update button with 'Guardar cambios' text | Integration | Texto correcto del boton en modo edit |
| 5 | calls updatePromotor mutation on submit with correct data | Integration | Modifica nombre, click guardar - `update` llamado con los datos correctos (sin tipoPromotorId) |
| 6 | shows toast success after successful update | Integration | `update` resuelve - toast 'Perfil actualizado correctamente' |
| 7 | shows toast error when update fails | Integration | `update` rechaza - toast de error |
| 8 | invalidates promotor query on successful update | Integration | Tras exito, `invalidateQueries` llamado con key correcto |
| 9 | shows error state when profile cannot be loaded | Integration | `getMe` rechaza - mensaje de error visible, no formulario |
| 10 | shows PromotorDeactivateDialog when deactivate button is clicked | Integration | Click en 'Desactivar cuenta' - dialog visible con `open: true` |

---

### 4.4 Hooks

#### usePromotor.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/usePromotor.test.ts`

**Descripcion del hook:**
```typescript
export function usePromotor() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.promotor.me,
        queryFn: () => promotorService.getMe(),
        retry: false,  // No reintentar en 404
    });
}
```

**Setup:**
```typescript
vi.mock('../../infrastructure/promotor.service', () => ({
    promotorService: { getMe: vi.fn() },
}));

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, gcTime: 0, staleTime: 0 } },
    });
    return ({ children }) => (
        <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
};
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | returns promotor data on success | Unit | `getMe` resuelve con `mockPromotor` - `result.current.data` == `mockPromotor` |
| 2 | handles loading state initially | Unit | Antes de resolver - `isLoading: true`, `data: undefined` |
| 3 | handles error state when API fails | Unit | `getMe` rechaza con Error - `isError: true`, `data: undefined` |
| 4 | handles 404 not found (no profile yet) | Unit | `getMe` rechaza con error 404 - `isError: true`, no reintenta (retry: false) |
| 5 | uses correct query key | Unit | Verificar que `QUERY_KEYS.crowdpromotion.promotor.me` se pasa como queryKey |
| 6 | does NOT retry on failure | Unit | `getMe` rechaza una vez - el hook no vuelve a llamar a `getMe` con retry: false |

---

#### useCreatePromotor.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/useCreatePromotor.test.ts`

**Descripcion del hook:**
```typescript
export function useCreatePromotor() {
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    return useMutation({
        mutationFn: (data: CreatePromotorRequest) => promotorService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me });
            toast.success('Perfil de promotor creado correctamente');
            navigate('/promotor/dashboard');
        },
        onError: (error) => {
            toast.error('Error al crear el perfil', { description: error.message });
        },
    });
}
```

**Mocks necesarios:** `promotorService`, `sonner`, `react-router-dom`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | creates promotor successfully | Unit | `create` resuelve con `mockPromotorCreatedResult` - `isSuccess: true`, `data` correcto |
| 2 | passes correct data to service | Unit | Llama a `create` con exactamente el objeto recibido por `mutate` |
| 3 | shows success toast on success | Unit | `toast.success` llamado con 'Perfil de promotor creado correctamente' |
| 4 | navigates to /promotor/dashboard on success | Unit | `mockNavigate` llamado con '/promotor/dashboard' |
| 5 | invalidates promotor query on success | Integration | `queryClient.invalidateQueries` llamado con `QUERY_KEYS.crowdpromotion.promotor.me` |
| 6 | shows error toast on failure | Unit | `create` rechaza - `toast.error` llamado con descripcion del error |
| 7 | does NOT navigate on failure | Unit | `create` rechaza - `mockNavigate` no llamado |

---

#### useUpdatePromotor.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/useUpdatePromotor.test.ts`

**Descripcion del hook:**
```typescript
export function useUpdatePromotor() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: UpdatePromotorRequest) => promotorService.update(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me });
            toast.success('Perfil actualizado correctamente');
        },
        onError: (error) => {
            toast.error('Error al actualizar el perfil', { description: error.message });
        },
    });
}
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | updates promotor successfully | Unit | `update` resuelve con `mockPromotorUpdatedResult` - `isSuccess: true` |
| 2 | passes correct data to service (without tipoPromotorId) | Unit | Request enviado al service no contiene `tipoPromotorId` |
| 3 | shows success toast on success | Unit | `toast.success` llamado con 'Perfil actualizado correctamente' |
| 4 | invalidates promotor query on success | Integration | `queryClient.invalidateQueries` con key correcto |
| 5 | shows error toast on failure | Unit | `update` rechaza - `toast.error` llamado |

---

#### useDesactivarPromotor.test.ts

**Archivo:** `src/web/src/features/crowdpromotion/__tests__/hooks/useDesactivarPromotor.test.ts`

**Descripcion del hook:**
```typescript
export function useDesactivarPromotor() {
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    return useMutation({
        mutationFn: () => promotorService.desactivar(),
        onSuccess: (data) => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me });
            toast.success('Perfil de promotor desactivado');
            navigate('/');
        },
        onError: (error) => {
            toast.error('Error al desactivar el perfil', { description: error.message });
        },
    });
}
```

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | deactivates promotor successfully | Unit | `desactivar` resuelve con `mockPromotorDesactivadoResult` - `isSuccess: true` |
| 2 | shows success toast on deactivation | Unit | `toast.success` con 'Perfil de promotor desactivado' |
| 3 | navigates to home page after deactivation | Unit | `mockNavigate` llamado con '/' |
| 4 | invalidates promotor query on success | Integration | `queryClient.invalidateQueries` con key de promotor |
| 5 | shows error toast when already inactive (errorCode 4019) | Unit | `desactivar` rechaza con error 4019 - `toast.error` llamado |
| 6 | does NOT navigate on failure | Unit | `desactivar` rechaza - `mockNavigate` no llamado |

---

## 5. Flujos de Integracion Criticos

Estos flujos combinan multiples componentes y hooks para verificar escenarios end-to-end dentro del contexto de testing de integracion (sin backend real).

### Flujo A: Registro de Nuevo Promotor (Camino feliz)

**Test en:** `PromotorRegistroPage.test.tsx`

```
1. Render PromotorRegistroPage con MemoryRouter en /promotor/registro
2. Mock useAuthStore: isAuthenticated=true
3. Mock promotorService.getMe: rechaza con error 404 (no existe perfil)
4. Verificar: formulario de registro visible
5. userEvent.selectOptions en tipo promotor -> 'Influencer'
6. userEvent.type en nombre publico -> 'DJ Marketing Pro'
7. userEvent.type en emailContacto -> 'contacto@djmarketing.com'
8. userEvent.type en urlInstagram -> 'https://instagram.com/djmarketing'
9. Mock promotorService.create: resuelve con mockPromotorCreatedResult
10. userEvent.click en boton submit
11. Verificar: toast.success llamado con mensaje correcto
12. Verificar: mockNavigate llamado con '/promotor/dashboard'
13. Verificar: queryClient.invalidateQueries llamado
```

### Flujo B: Redireccion Automatica (FA-01)

**Test en:** `PromotorRegistroPage.test.tsx`

```
1. Render PromotorRegistroPage
2. Mock promotorService.getMe: resuelve con mockPromotor (perfil existente)
3. Verificar: mockNavigate llamado con '/promotor/dashboard'
4. Verificar: formulario de registro NO visible
```

### Flujo C: Edicion de Perfil Existente

**Test en:** `PromotorPerfilPage.test.tsx`

```
1. Render PromotorPerfilPage
2. Mock promotorService.getMe: resuelve con mockPromotor
3. Verificar: campos pre-rellenados con datos del promotor
4. Verificar: tipoPromotorNombre 'Influencer' como texto (no select)
5. userEvent.clear en nombre publico
6. userEvent.type en nombre publico -> 'Nuevo Nombre Publico'
7. Mock promotorService.update: resuelve con mockPromotorUpdatedResult
8. userEvent.click en boton guardar
9. Verificar: update llamado con datos correctos (sin tipoPromotorId)
10. Verificar: toast.success con mensaje de actualizacion
```

### Flujo D: Desactivacion con Programas Activos

**Test en:** `PromotorPerfilPage.test.tsx`

```
1. Render PromotorPerfilPage
2. Mock promotorService.getMe: resuelve con mockPromotor (totalProgramasActivos: 3)
3. userEvent.click en boton 'Desactivar cuenta'
4. Verificar: PromotorDeactivateDialog visible con open=true
5. Verificar: mensaje sobre 3 programas afectados
6. Mock promotorService.desactivar: resuelve con mockPromotorDesactivadoResult
7. userEvent.click en boton confirmar del dialog
8. Verificar: toast.success con mensaje de desactivacion
9. Verificar: navigate llamado con '/'
```

---

## 6. Cobertura por Archivo

| Archivo | Lineas Obj. | Funciones Obj. | Branches Obj. | Notas |
|---------|-------------|----------------|---------------|-------|
| `shared/schemas/crowdpromotion.schema.ts` | 95% | 100% | 95% | Logica pura - alta cobertura alcanzable |
| `application/hooks/usePromotor.ts` | 90% | 100% | 85% | Hook simple de query |
| `application/hooks/useCreatePromotor.ts` | 90% | 100% | 90% | Incluye onSuccess/onError |
| `application/hooks/useUpdatePromotor.ts` | 90% | 100% | 90% | Incluye onSuccess/onError |
| `application/hooks/useDesactivarPromotor.ts` | 90% | 100% | 90% | Incluye onSuccess/onError |
| `presentation/components/PromotorForm.tsx` | 85% | 95% | 80% | Logica de validacion visual |
| `presentation/components/PromotorDeactivateDialog.tsx` | 90% | 100% | 85% | Componente acotado |
| `presentation/pages/PromotorRegistroPage.tsx` | 80% | 90% | 80% | Guards y flujos alternativos |
| `presentation/pages/PromotorPerfilPage.tsx` | 80% | 90% | 80% | Flujos de edicion y desactivacion |
| `presentation/pages/PromotorDashboardPage.tsx` | 80% | 90% | 75% | Display de KPIs |
| `infrastructure/promotor.service.ts` | 75% | 90% | 70% | Service testeado via hooks; no requiere tests directos de metodos |

**Meta Global:** 80% en lineas, funciones y branches.

---

## 7. Casos Edge a Cubrir

### Conversión de string vacio a undefined en campos URL

El schema Zod usa `.or(z.literal(''))` para aceptar strings vacios, pero el service no debe recibir un string vacio como valor de URL (causaria error de validacion en backend). La logica de conversion debe estar en el `onSubmit` del formulario o en el hook.

**Tests a incluir:**
- En `PromotorForm.test.tsx`: verificar que `onSubmit` recibe `undefined` (no `''`) para campos URL vacios.
- En `useCreatePromotor.test.ts`: si la conversion ocurre en el hook, verificar que `promotorService.create` no recibe campos URL con string vacio.

### Estado de promotor inactivo en Dashboard

- Dashboard muestra badge 'Perfil inactivo' cuando `esActivo: false`.
- El boton 'Desactivar cuenta' puede estar oculto o deshabilitado si el perfil ya esta inactivo.

### Concurrencia de queries en PromotorRegistroPage

- El guard de redireccion depende de `usePromotor`. Si la query esta en estado `isLoading`, el formulario no debe mostrarse (evitar flash de contenido).

### Error 4018 - Duplicado

- Si el backend retorna error con errorCode '4018' (promotor ya existe), el frontend debe mostrar el mensaje especifico del error, no un mensaje generico.

---

## 8. Configuración de Testing Existente

El proyecto ya tiene Vitest configurado en `src/web/vite.config.ts`:

```typescript
test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./vitest.setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
}
```

Los tests deben ubicarse en `src/web/src/features/crowdpromotion/__tests__/` para ser recogidos automaticamente por el runner.

**Path aliases disponibles (vite.config.ts):**
- `@` → `src/web/src`
- `@shared` → `src/shared`

Los imports de schemas y tipos deben usar `@shared/schemas/crowdpromotion.schema` y `@shared/types/crowdpromotion`.

---

## 9. Comandos de Ejecucion

```bash
# Desde src/web/

# Ejecutar todos los tests
npm run test

# Ejecutar con coverage
npm run test:coverage

# Ejecutar solo los tests de crowdpromotion
npm run test -- --reporter=verbose src/features/crowdpromotion

# Watch mode para desarrollo
npm run test:watch
```

---

## 10. Checklist de Testing

- [ ] Mock data definida en `__mocks__/promotor.mock.ts` para todos los casos (activo, minimo, inactivo)
- [ ] Tests de schemas Zod: createPromotorSchema (14 tests)
- [ ] Tests de schemas Zod: updatePromotorSchema (10 tests)
- [ ] Tests de PromotorForm: render, validacion, interaccion, modos create/edit
- [ ] Tests de PromotorDashboardPage: KPIs, estados loading/error/success
- [ ] Tests de PromotorDeactivateDialog: render, confirmacion, cancelacion, loading
- [ ] Tests de PromotorRegistroPage: guard redireccion, flujo registro, manejo errores, auth guard
- [ ] Tests de PromotorPerfilPage: pre-llenado, edicion, tipo read-only, desactivacion
- [ ] Tests de usePromotor: query success/error/loading, retry:false
- [ ] Tests de useCreatePromotor: mutation success/error, toast, navigate, invalidate
- [ ] Tests de useUpdatePromotor: mutation success/error, toast, invalidate, sin tipoPromotorId
- [ ] Tests de useDesactivarPromotor: mutation success/error, toast, navigate home, invalidate
- [ ] Conversion string-vacio-a-undefined en campos URL cubierta
- [ ] Auth guard (`isAuthenticated: false` → redirect) cubierto en PromotorRegistroPage
- [ ] Cobertura 80%+ verificada con `npm run test:coverage`
- [ ] Tests pasan en < 60 segundos (objetivo del proyecto)
