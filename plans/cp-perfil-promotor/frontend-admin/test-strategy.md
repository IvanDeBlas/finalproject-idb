# Estrategia de Testing: Perfil de Promotor (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/admin
**Cobertura Objetivo:** 80%

---

## 1. Resumen

| Tipo | Cantidad | Archivos |
|------|----------|----------|
| Unit Tests (componentes) | 26 | 3 archivos |
| Integration Tests (hooks) | 16 | 3 archivos |
| Unit Tests (service) | 10 | 1 archivo |
| **Total** | **52** | **7 archivos** |

**Cobertura objetivo por modulo:**

| Modulo | Lineas | Funciones | Branches |
|--------|--------|-----------|----------|
| `PromotorProfileCard.tsx` | 85% | 90% | 80% |
| `PromotorProfilePage.tsx` | 80% | 85% | 80% |
| `PromotorDeactivateDialog.tsx` | 90% | 100% | 85% |
| `use-promotor.ts` | 90% | 100% | 85% |
| `promotor.service.ts` | 95% | 100% | 90% |

---

## 2. Estructura de Tests

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── crowdpromotion/
│           └── promotor/
│               └── components/
│                   └── __tests__/
│                       ├── PromotorProfileCard.test.tsx
│                       ├── PromotorProfilePage.test.tsx
│                       └── PromotorDeactivateDialog.test.tsx
├── hooks/
│   └── __tests__/
│       └── use-promotor.test.ts
└── services/
    └── __tests__/
        └── promotor.service.test.ts
```

**Ubicacion del mock central:**

```
src/admin/src/__mocks__/
└── promotor.mock.ts          (nuevo - datos de prueba)
```

---

## 3. Contexto del Proyecto: Patrones Existentes

Antes de describir los tests, es importante notar que el proyecto admin ya tiene patrones bien establecidos que DEBEN seguirse:

### 3.1 Test Utils

El proyecto expone `@/test-utils` que envuelve `render` de Testing Library con `QueryClientProvider`. Todos los tests de componentes deben importar `render` y `screen` desde `@/test-utils`, NO desde `@testing-library/react` directamente.

```typescript
// CORRECTO - patron del proyecto
import { render, screen } from "@/test-utils"

// INCORRECTO
import { render, screen } from "@testing-library/react"
```

### 3.2 Wrapper para hooks

Para tests de hooks, el proyecto usa `createElement` en lugar de JSX en los wrappers, con una funcion `createWrapper()` local en cada test:

```typescript
function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}
```

### 3.3 Mocking de servicios

Los services se mockean con `vi.mock` al nivel del modulo, mockeando el objeto instanciado:

```typescript
vi.mock("@/services/promotor.service", () => ({
    promotorService: {
        getMe: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}))
```

### 3.4 Mocking de Next.js router

El dashboard usa `useRouter` de `next/navigation`. Se mockea asi:

```typescript
vi.mock("next/navigation", () => ({
    useRouter: () => ({
        push: vi.fn(),
        replace: vi.fn(),
        back: vi.fn(),
    }),
    usePathname: () => "/crowdpromotion/promotor",
}))
```

### 3.5 Mocking de constantes compartidas

Cuando un componente usa constantes de `@shared/constants`, mockear solo lo necesario:

```typescript
vi.mock("@shared/constants", () => ({
    TIPO_PROMOTOR_LABELS: {
        1: "Fan Embajador",
        2: "Influencer",
        3: "Medio / Blog",
        4: "Profesional Marketing",
    },
    QUERY_KEYS: {
        crowdpromotion: {
            promotor: { me: ["crowdpromotion", "promotor", "me"] },
        },
    },
}))
```

### 3.6 Mocking de sonner (toast)

```typescript
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))
```

---

## 4. Mocks y Fixtures

### 4.1 Archivo de mock data

**Archivo:** `src/admin/src/__mocks__/promotor.mock.ts`

**Contenido planificado:**

```typescript
import type { Promotor, PromotorUpdatedResult, PromotorDesactivadoResult } from "@shared/types"

// Promotor activo con todos los campos
export const mockPromotor: Promotor = {
    id: "promotor-123",
    nombrePublico: "DJ Marketing Pro",
    tipoPromotorId: 2,
    tipoPromotorNombre: "Influencer",
    emailContacto: "contacto@djmarketing.com",
    urlSitioWeb: "https://djmarketing.com",
    urlInstagram: "https://instagram.com/djmarketing",
    urlTikTok: "https://tiktok.com/@djmarketing",
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: "2026-02-25T10:00:00Z",
    totalProgramasActivos: 3,
    totalComisionesGanadas: 150.50,
    monedaComisiones: "EUR",
}

// Promotor con minimo de campos (sin URLs ni email)
export const mockPromotorMinimo: Promotor = {
    id: "promotor-456",
    nombrePublico: "Fan Embajador Test",
    tipoPromotorId: 1,
    tipoPromotorNombre: "Fan Embajador",
    emailContacto: null,
    urlSitioWeb: null,
    urlInstagram: null,
    urlTikTok: null,
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: "2026-02-25T10:00:00Z",
    totalProgramasActivos: 0,
    totalComisionesGanadas: 0,
    monedaComisiones: "EUR",
}

// Promotor desactivado
export const mockPromotorInactivo: Promotor = {
    ...mockPromotor,
    esActivo: false,
    totalProgramasActivos: 0,
}

// Resultado de actualizacion
export const mockPromotorUpdatedResult: PromotorUpdatedResult = {
    id: "promotor-123",
    nombrePublico: "DJ Marketing Pro (Updated)",
    fechaActualizacion: "2026-02-25T11:00:00Z",
}

// Resultado de desactivacion con programas afectados
export const mockDesactivadoConProgramas: PromotorDesactivadoResult = {
    id: "promotor-123",
    esActivo: false,
    programasDadosDeBaja: 2,
}

// Resultado de desactivacion sin programas afectados
export const mockDesactivadoSinProgramas: PromotorDesactivadoResult = {
    id: "promotor-123",
    esActivo: false,
    programasDadosDeBaja: 0,
}
```

---

## 5. Tests por Modulo

### 5.1 PromotorProfileCard.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/promotor/components/__tests__/PromotorProfileCard.test.tsx`

**Descripcion del componente:** Widget de solo lectura que se muestra en el dashboard cuando el usuario tiene perfil de promotor. Muestra nombre, tipo, estado activo/inactivo, estadisticas (programas activos, comisiones ganadas) y un link a la pagina de edicion.

**Dependencias a mockear:**
- `@shared/constants` para `TIPO_PROMOTOR_LABELS`

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders nombre publico | Unit | Muestra `promotor.nombrePublico` en el DOM |
| 2 | renders tipo de promotor | Unit | Muestra `tipoPromotorNombre` (ej: "Influencer") |
| 3 | renders badge activo | Unit | Muestra badge/indicador de estado activo cuando `esActivo=true` |
| 4 | renders badge inactivo | Unit | Muestra badge/indicador de estado inactivo cuando `esActivo=false` |
| 5 | renders totalProgramasActivos | Unit | Muestra el numero de programas activos |
| 6 | renders totalComisionesGanadas formateado | Unit | Muestra comisiones formateadas con moneda EUR |
| 7 | renders link a edicion de perfil | Unit | Tiene link/boton que apunta a la ruta de edicion del perfil |
| 8 | renders null/placeholder cuando no hay datos | Unit | No rompe cuando `promotor` es `undefined` o `null` (estado loading) |
| 9 | renders skeleton en estado loading | Unit | Muestra skeletons cuando `isLoading=true` (patron de MisCampaniasCard) |

**Casos Detallados:**

```
1. renders nombre publico
   - Render <PromotorProfileCard promotor={mockPromotor} />
   - Assert: screen.getByText("DJ Marketing Pro") en documento

2. renders badge activo cuando esActivo=true
   - Render con mockPromotor (esActivo=true)
   - Assert: texto o role que indique estado activo (ej: "Activo")

3. renders badge inactivo cuando esActivo=false
   - Render con mockPromotorInactivo (esActivo=false)
   - Assert: texto o role que indique estado inactivo (ej: "Inactivo")

4. renders totalProgramasActivos=0
   - Render con mockPromotorMinimo (totalProgramasActivos=0)
   - Assert: presencia del valor "0" en el contexto de programas

5. renders skeleton en estado loading
   - Render <PromotorProfileCard isLoading={true} />
   - Assert: container.querySelectorAll('[class*="animate-pulse"]').length > 0
   (mismo patron que MisCampaniasCard.test.tsx)

6. no rompe sin datos
   - Render <PromotorProfileCard />
   - Assert: componente renderiza sin lanzar error (empty/null state)
```

---

### 5.2 PromotorProfilePage.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/promotor/components/__tests__/PromotorProfilePage.test.tsx`

**Descripcion del componente:** Pagina de edicion del perfil de promotor en el dashboard admin. Pre-rellena el formulario con los datos actuales del promotor. El campo `tipoPromotorNombre` se muestra como solo lectura. Al enviar, llama a `useUpdatePromotor`. Incluye seccion de desactivacion con boton que abre `PromotorDeactivateDialog`.

**Dependencias a mockear:**
- `@/hooks/use-promotor` (usePromotor, useUpdatePromotor)
- `sonner` (toast)
- `next/navigation` (useRouter - no es necesario que redirija en admin)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders formulario con datos pre-rellenados | Integration | Los campos muestran los valores del promotor cargado |
| 2 | campo tipoPromotorNombre es solo lectura | Unit | El campo tipo no tiene input editable; muestra texto fijo |
| 3 | renders skeleton cuando isLoading | Unit | Muestra skeleton cuando el hook de query esta en loading |
| 4 | renderiza estado de error cuando el perfil no se carga | Unit | Muestra mensaje de error si el hook retorna `isError=true` |
| 5 | form submission llama updatePromotor con datos correctos | Integration | Al hacer submit del form, la mutacion recibe el payload correcto |
| 6 | muestra toast success tras actualizacion exitosa | Integration | Tras `onSuccess` de la mutacion, se llama `toast.success` |
| 7 | muestra toast error si la actualizacion falla | Integration | Tras error de mutacion, se llama `toast.error` |
| 8 | valida nombrePublico requerido | Integration | Si nombrePublico esta vacio al submit, muestra error de validacion Zod |
| 9 | valida nombrePublico minimo 3 caracteres | Integration | Si nombrePublico tiene 1 o 2 chars, muestra error "al menos 3 caracteres" |
| 10 | valida formato URL de Instagram invalida | Integration | Si urlInstagram no es URL valida, muestra error de formato |
| 11 | acepta URL de Instagram vacia (campo opcional) | Integration | Si urlInstagram queda vacio, el form es valido (no muestra error) |
| 12 | valida formato de emailContacto invalido | Integration | Si emailContacto no tiene formato de email, muestra error |
| 13 | acepta emailContacto vacio (campo opcional) | Integration | Si emailContacto queda vacio, el form es valido |
| 14 | boton submit deshabilitado durante isPending | Unit | Boton "Guardar" tiene atributo `disabled` cuando la mutacion esta pendiente |
| 15 | boton desactivar abre dialogo de confirmacion | Integration | Al click en "Desactivar cuenta", se muestra `PromotorDeactivateDialog` |

**Casos Detallados:**

```
1. renders formulario con datos pre-rellenados
   - Mock usePromotor para retornar mockPromotor
   - Render <PromotorProfilePage />
   - Assert: screen.getByDisplayValue("DJ Marketing Pro") existe
   - Assert: screen.getByDisplayValue("contacto@djmarketing.com") existe
   - Assert: screen.getByDisplayValue("https://instagram.com/djmarketing") existe

2. campo tipoPromotorNombre es solo lectura
   - Mock usePromotor para retornar mockPromotor (tipoPromotorNombre="Influencer")
   - Assert: texto "Influencer" visible en pantalla
   - Assert: NO hay input con name="tipoPromotorId" editable o con valor

3. form submission llama updatePromotor con datos correctos
   - Mock useUpdatePromotor.mutateAsync para resolver exitosamente
   - Render pagina con datos pre-rellenados
   - Modificar campo nombrePublico con userEvent.type
   - Click en boton submit
   - Assert: mutateAsync llamado con objeto que contiene nombrePublico actualizado
   - Assert: campos URL vacios convertidos a undefined en el payload

4. valida nombrePublico requerido
   - Render con formulario vacio (mock usePromotor retorna null)
   - Limpiar campo nombrePublico
   - Click submit
   - Assert: screen.getByText("El nombre publico es obligatorio")

5. valida formato URL de Instagram invalida
   - Render con datos cargados
   - Limpiar campo urlInstagram y escribir "no-es-url"
   - Click submit
   - Assert: mensaje de error de formato URL visible

6. boton desactivar abre dialogo de confirmacion
   - Render pagina con mockPromotor activo
   - Click en boton "Desactivar cuenta de promotor"
   - Assert: dialogo de confirmacion visible (PromotorDeactivateDialog)
```

---

### 5.3 PromotorDeactivateDialog.test.tsx

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/promotor/components/__tests__/PromotorDeactivateDialog.test.tsx`

**Descripcion del componente:** Dialogo de confirmacion de desactivacion del perfil de promotor. Recibe `open`, `onOpenChange`, `onConfirm`, `isPending` y `totalProgramasActivos` como props. Si `totalProgramasActivos > 0` muestra aviso de programas afectados con el numero. Si `totalProgramasActivos == 0` muestra dialogo simple sin aviso. El boton de confirmacion llama `onConfirm`. El boton cancelar llama `onOpenChange(false)`.

Sigue el mismo patron que `CerrarNecesidadDialog.test.tsx` del proyecto.

**Dependencias a mockear:** Ninguna (componente puro de UI).

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | renders cuando open=true | Unit | Muestra el heading del dialogo |
| 2 | no renderiza cuando open=false | Unit | El contenido del dialogo no esta en el DOM |
| 3 | muestra aviso de programas afectados cuando totalProgramasActivos > 0 | Unit | Texto con numero de programas afectados visible |
| 4 | NO muestra aviso de programas cuando totalProgramasActivos = 0 | Unit | El texto de programas afectados no aparece |
| 5 | llama onConfirm al hacer click en confirmar | Unit | El handler se ejecuta al click en boton de confirmacion |
| 6 | llama onOpenChange(false) al hacer click en cancelar | Unit | El handler cierra el dialogo |
| 7 | deshabilita botones cuando isPending=true | Unit | Ambos botones tienen `disabled` durante la operacion |
| 8 | muestra texto de carga cuando isPending=true | Unit | Boton muestra "Desactivando..." o similar |
| 9 | el texto de confirmacion menciona la accion irreversible | Unit | Hay texto de advertencia sobre la desactivacion |

**Casos Detallados:**

```
1. renders cuando open=true
   - Render <PromotorDeactivateDialog open={true} onOpenChange={vi.fn()}
     onConfirm={vi.fn()} isPending={false} totalProgramasActivos={0} />
   - Assert: screen.getByRole("heading", { name: /Desactivar/i }) existe

2. no renderiza cuando open=false
   - Render con open={false}
   - Assert: screen.queryByRole("heading", { name: /Desactivar/i }) es null

3. muestra aviso cuando totalProgramasActivos=3
   - Render con totalProgramasActivos={3}
   - Assert: screen.getByText(/3/) + texto sobre programas afectados visible

4. no muestra aviso cuando totalProgramasActivos=0
   - Render con totalProgramasActivos={0}
   - Assert: NO hay texto sobre "programas afectados" o "baja automatica"

5. llama onConfirm al confirmar
   - const onConfirm = vi.fn()
   - user.click(screen.getByRole("button", { name: /Confirmar|Desactivar/i }))
   - Assert: onConfirm llamado una vez

6. llama onOpenChange(false) al cancelar
   - const onOpenChange = vi.fn()
   - user.click(screen.getByRole("button", { name: /Cancelar/i }))
   - Assert: onOpenChange llamado con false

7. deshabilita botones cuando isPending=true
   - Render con isPending={true}
   - Assert: screen.getByRole("button", { name: /Cancelar/i }).disabled = true
   - Assert: boton de confirmacion tiene disabled

8. muestra texto de carga durante isPending
   - Render con isPending={true}
   - Assert: screen.getByText(/Desactivando/i) o texto similar en boton
```

---

### 5.4 use-promotor.test.ts

**Archivo:** `src/admin/src/hooks/__tests__/use-promotor.test.ts`

**Descripcion:** Tests de los tres hooks: `usePromotor` (query GET /me), `useUpdatePromotor` (mutation PUT /me), `useDesactivarPromotor` (mutation PATCH /me/desactivar).

Sigue el patron de `use-rewards.test.ts` y `use-dashboard-stats.test.ts`.

**Dependencias a mockear:**
- `@/services/promotor.service` (promotorService)

#### usePromotor

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | retorna datos del promotor en success | Integration | `result.current.data` iguala `mockPromotor` |
| 2 | retorna null cuando API retorna 404 | Integration | El service devuelve `null`; `isSuccess=true` con `data=null` |
| 3 | maneja estado loading | Integration | `isLoading=true` mientras la promesa esta pendiente |
| 4 | maneja estado error | Integration | `isError=true` cuando el service lanza excepcion |
| 5 | no hace retry en errores (retry:false) | Integration | El hook no reintenta la query fallida automaticamente |

#### useUpdatePromotor

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 6 | llama al service con datos correctos en mutate | Integration | `promotorService.update` recibe el payload del formulario |
| 7 | invalida query me tras exito | Integration | Tras `onSuccess`, se invalida `QUERY_KEYS.crowdpromotion.promotor.me` |
| 8 | maneja error de actualizacion | Integration | `isError=true` cuando el service falla |
| 9 | payload excluye strings vacios (transforma '' a undefined) | Integration | El hook o service no envia campos de URL con valor '' |

#### useDesactivarPromotor

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 10 | llama al service de desactivacion al mutar | Integration | `promotorService.desactivar` se invoca al llamar `mutate()` |
| 11 | invalida query me tras desactivacion exitosa | Integration | Tras `onSuccess`, se invalida `QUERY_KEYS.crowdpromotion.promotor.me` |
| 12 | retorna programasDadosDeBaja en data | Integration | `result.current.data.programasDadosDeBaja` es el numero retornado por el service |
| 13 | maneja error de desactivacion | Integration | `isError=true` cuando el service falla |

**Setup de referencia para los hooks:**

```typescript
vi.mock("@/services/promotor.service", () => ({
    promotorService: {
        getMe: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}))

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}
```

**Casos Detallados:**

```
1. usePromotor - retorna datos en success
   - vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
   - renderHook(() => usePromotor(), { wrapper: createWrapper() })
   - await waitFor(() => expect(result.current.isSuccess).toBe(true))
   - Assert: result.current.data igual a mockPromotor

2. usePromotor - retorna null en 404
   - vi.mocked(promotorService.getMe).mockResolvedValue(null)
   - await waitFor(() => expect(result.current.isSuccess).toBe(true))
   - Assert: result.current.data es null (no undefined)

3. useUpdatePromotor - llama service con payload correcto
   - vi.mocked(promotorService.update).mockResolvedValue(mockPromotorUpdatedResult)
   - act(() => result.current.mutate(updatePayload))
   - await waitFor(() => expect(result.current.isSuccess).toBe(true))
   - Assert: promotorService.update llamado con updatePayload

7. useUpdatePromotor - invalida query me tras exito
   - Spy en queryClient.invalidateQueries
   - Ejecutar mutacion exitosa
   - Assert: invalidateQueries llamado con queryKey que incluye "crowdpromotion"

10. useDesactivarPromotor - llama service de desactivacion
    - vi.mocked(promotorService.desactivar).mockResolvedValue(mockDesactivadoConProgramas)
    - act(() => result.current.mutate())
    - await waitFor(() => expect(result.current.isSuccess).toBe(true))
    - Assert: promotorService.desactivar llamado una vez
```

---

### 5.5 promotor.service.test.ts

**Archivo:** `src/admin/src/services/__tests__/promotor.service.test.ts`

**Descripcion:** Tests del servicio que encapsula las llamadas al API de promotor. Sigue el patron de `necesidad.service.test.ts` y `campania-backings.service.test.ts`.

**Dependencias a mockear:**
- `@/lib/api-client` (apiFetch)

| # | Test Case | Tipo | Descripcion |
|---|-----------|------|-------------|
| 1 | getMe - retorna perfil completo del promotor | Unit | `apiFetch` llamado con URL correcta; retorna `response.data` |
| 2 | getMe - retorna null cuando API lanza error | Unit | Si `apiFetch` lanza excepcion, el service retorna `null` (no propaga) |
| 3 | getMe - llama a la URL /api/crowdpromotion/promotor/me | Unit | La URL del endpoint es correcta |
| 4 | update - llama a PUT con URL y body correctos | Unit | `apiFetch` recibe `method: "PUT"` y la URL `/api/crowdpromotion/promotor/me` |
| 5 | update - retorna PromotorUpdatedResult en success | Unit | `response.data` se retorna correctamente |
| 6 | update - propaga error cuando API falla | Unit | Si `apiFetch` lanza, el service deja que se propague (sin silenciar) |
| 7 | desactivar - llama a PATCH con URL correcta | Unit | `apiFetch` recibe `method: "PATCH"` y la URL `/me/desactivar` |
| 8 | desactivar - retorna PromotorDesactivadoResult en success | Unit | `response.data` contiene `programasDadosDeBaja` |
| 9 | desactivar - propaga error cuando API falla | Unit | Si `apiFetch` lanza, el error no se silencia |
| 10 | update - no envia campos de URL como strings vacios | Unit | Si el payload tiene URL="" el service lo envia como undefined/no incluido |

**Setup de referencia:**

```typescript
vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

describe("PromotorService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })
    // ...
})
```

**Casos Detallados:**

```
1. getMe - retorna perfil en success
   - vi.mocked(apiFetch).mockResolvedValue({ data: mockPromotor, messages: [] })
   - const result = await promotorService.getMe()
   - Assert: result igual a mockPromotor
   - Assert: apiFetch llamado con string que contiene "/api/crowdpromotion/promotor/me"

2. getMe - retorna null en error
   - vi.mocked(apiFetch).mockRejectedValue(new Error("Not found"))
   - const result = await promotorService.getMe()
   - Assert: result es null (no lanza excepcion al caller)

4. update - llama PUT con URL y body correctos
   - vi.mocked(apiFetch).mockResolvedValue({ data: mockPromotorUpdatedResult, messages: [] })
   - await promotorService.update({ nombrePublico: "Nuevo Nombre" })
   - Assert: apiFetch llamado con url que contiene "/api/crowdpromotion/promotor/me"
   - Assert: apiFetch llamado con { method: "PUT", data: { nombrePublico: "Nuevo Nombre" } }

7. desactivar - llama PATCH con URL correcta
   - vi.mocked(apiFetch).mockResolvedValue({ data: mockDesactivadoConProgramas, messages: [] })
   - await promotorService.desactivar()
   - Assert: apiFetch llamado con url que contiene "desactivar"
   - Assert: apiFetch llamado con { method: "PATCH" }
```

---

## 6. Casos Edge y de Error Criticos

Los siguientes casos son transversales a varios modulos y cubren flujos de negocio importantes:

### 6.1 Desactivacion con programas activos

Flujo: Usuario con `totalProgramasActivos=3` hace click en desactivar.

- **PromotorProfilePage**: Boton desactivar abre dialog pasando `totalProgramasActivos=3`
- **PromotorDeactivateDialog**: Muestra aviso "3 programas se veran afectados"
- **useDesactivarPromotor**: Llama al service y retorna `programasDadosDeBaja: 2`

### 6.2 Perfil inactivo ya desactivado (error 4019)

Flujo: API retorna `{ errorCode: "4019" }` al intentar desactivar.

- **useDesactivarPromotor**: `isError=true`; el componente padre captura el error
- **PromotorProfilePage**: Muestra `toast.error` con mensaje correspondiente al error 4019

### 6.3 Promotor sin redes sociales

Flujo: `mockPromotorMinimo` sin URLs ni email.

- **PromotorProfileCard**: No debe romper; muestra placeholder o texto vacío para URLs
- **PromotorProfilePage**: Pre-rellena el formulario con campos de URL vacios (no null), usando `mapPromotorToUpdateForm` del shared

### 6.4 Formulario con URL invalida

Flujo: Usuario escribe "hola" en campo `urlInstagram`.

- **PromotorProfilePage**: Schema Zod bloquea el submit
- Assert: mensaje "La URL de Instagram no tiene formato valido" visible

### 6.5 Error de red al guardar

Flujo: `promotorService.update` lanza `new Error("Network error")`.

- **PromotorProfilePage**: Captura el error en el handler `handleSubmit`
- Assert: `toast.error` llamado con mensaje de error generico

---

## 7. Alineamiento con Patrones del Proyecto

### Lo que ya existe y DEBE replicarse

| Patron | Referencia Existente | Aplicacion en Promotor |
|--------|---------------------|------------------------|
| `render` desde `@/test-utils` | Todos los tests de componentes | `PromotorProfileCard`, `PromotorProfilePage`, `PromotorDeactivateDialog` |
| `createWrapper()` con `createElement` | `use-rewards.test.ts`, `use-dashboard-stats.test.ts` | `use-promotor.test.ts` |
| `vi.mock("@/services/...")` | `use-rewards.test.ts` | `use-promotor.test.ts` |
| `vi.mock("@/lib/api-client")` | `necesidad.service.test.ts` | `promotor.service.test.ts` |
| Mock data en `__mocks__/` | `dashboard.mock.ts` | `promotor.mock.ts` (nuevo) |
| `vi.clearAllMocks()` en `beforeEach` | Todos los tests con mocks | Todos los tests de promotor |
| `userEvent.setup()` para interacciones | `PublishConfirmModal.test.tsx`, `TemplateForm.test.tsx` | `PromotorProfilePage.test.tsx` |
| `fireEvent` para cambios de input simples | `CerrarNecesidadDialog.test.tsx` | `PromotorProfilePage.test.tsx` |
| Verificar `.disabled` en botones | `PublishConfirmModal.test.tsx`, `CerrarNecesidadDialog.test.tsx` | `PromotorDeactivateDialog.test.tsx` |

### Diferencia especifica del modulo Promotor vs otros modulos

El perfil de promotor en Admin es de **IMPACTO MEDIO** segun la feature spec: no tiene registro desde admin, solo edicion y desactivacion. Esto significa:

- No hay tests de formulario de **creacion** en admin (creacion es en Landing)
- Si hay tests de formulario de **edicion** (updatePromotorSchema)
- Si hay tests del dialogo de **desactivacion** (flujo critico con cascada en programas)
- El `PromotorProfileCard` es un widget informativo, no un formulario

### Next.js 14 App Router - Consideraciones

El dashboard admin usa Next.js 14 con App Router. Los componentes de pagina (`page.tsx`) usan `"use client"`. No se usa `next-page-tester` ni helpers de Next.js en los tests existentes; se testean como componentes React normales con mocks de `next/navigation`.

**Pattern de mock de router ya usado en el proyecto:**

```typescript
// Verificar patron en el codebase existente antes de implementar
// Los tests existentes como TemplateForm.test.tsx no mockean router
// porque TemplateForm no usa router directamente.
// PromotorProfilePage si puede usar router para redireccion post-submit,
// pero en Admin es menos critico (la edicion no redirige).
```

---

## 8. Cobertura por Archivo

| Archivo | Tests | Lineas | Funciones | Branches |
|---------|-------|--------|-----------|----------|
| `PromotorProfileCard.tsx` | 9 | 85% | 90% | 80% |
| `PromotorProfilePage.tsx` | 15 | 82% | 88% | 80% |
| `PromotorDeactivateDialog.tsx` | 8 | 90% | 100% | 88% |
| `use-promotor.ts` | 13 | 90% | 100% | 85% |
| `promotor.service.ts` | 10 | 95% | 100% | 90% |
| `promotor.mock.ts` | - | N/A | N/A | N/A |

**Meta Global:** 80% en todas las metricas

---

## 9. Comandos de Ejecucion

```bash
# Ejecutar todos los tests del admin
cd src/admin && npm run test

# Ejecutar en modo run (no watch) - para CI
cd src/admin && npm run test:run

# Ejecutar solo tests de la feature promotor
cd src/admin && npm run test -- --reporter=verbose promotor

# Ejecutar con coverage
cd src/admin && npx vitest run --coverage

# Watch mode durante desarrollo
cd src/admin && npm run test
```

---

## 10. Orden de Implementacion de Tests

Respetar este orden minimiza el tiempo de debugging por dependencias:

1. **`promotor.mock.ts`** - Sin dependencias. Base para todos los demas tests.
2. **`promotor.service.test.ts`** - Solo depende del mock de `apiFetch`. Tests puros y rapidos.
3. **`use-promotor.test.ts`** - Depende del mock del service. Valida la logica de negocio de invalidacion de cache.
4. **`PromotorDeactivateDialog.test.tsx`** - Componente puro sin dependencias externas. Tests rapidos.
5. **`PromotorProfileCard.test.tsx`** - Componente de solo lectura con pocos mocks.
6. **`PromotorProfilePage.test.tsx`** - Tests de integracion mas complejos. Depende de que los patrones anteriores esten claros.

---

## 11. Checklist de Implementacion

- [ ] Archivo `src/admin/src/__mocks__/promotor.mock.ts` creado con `mockPromotor`, `mockPromotorMinimo`, `mockPromotorInactivo`, `mockPromotorUpdatedResult`, `mockDesactivadoConProgramas`, `mockDesactivadoSinProgramas`
- [ ] `promotor.service.test.ts` con 10 casos (getMe, update, desactivar + errores)
- [ ] `use-promotor.test.ts` con 13 casos (usePromotor x5, useUpdatePromotor x4, useDesactivarPromotor x4)
- [ ] `PromotorDeactivateDialog.test.tsx` con 8 casos (render, open/close, programas afectados, isPending)
- [ ] `PromotorProfileCard.test.tsx` con 9 casos (datos, estados activo/inactivo, loading, null)
- [ ] `PromotorProfilePage.test.tsx` con 15 casos (pre-relleno, solo-lectura tipo, validacion, submit, errors)
- [ ] Todos los tests importan `render`/`screen` desde `@/test-utils` (no de `@testing-library/react`)
- [ ] Hooks usan `createWrapper()` con `createElement` (patron del proyecto)
- [ ] `vi.clearAllMocks()` en `beforeEach` en todos los archivos con mocks
- [ ] Tests de service mockean `@/lib/api-client`, no el service completo
- [ ] Tests de hooks mockean `@/services/promotor.service`, no `apiFetch`
- [ ] Cobertura 80%+ verificada con `npx vitest run --coverage`
- [ ] Tests pasan en CI (`npm run test:run`)
