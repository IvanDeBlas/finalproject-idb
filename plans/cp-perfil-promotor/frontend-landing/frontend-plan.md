# Plan Frontend: Perfil de Promotor (Landing)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

| Elemento | Cantidad |
|----------|----------|
| Screens (Pages) | 3 |
| Componentes de presentacion | 9 |
| Hooks | 5 |
| Services | 1 |
| Archivos de dominio / tipos | 1 |

### Screens

| Ruta | Componente | Layout | Auth |
|------|------------|--------|------|
| `/promotor/registro` | `PromotorRegistroPage` | `DashboardLayout` | Bearer JWT; si ya tiene perfil -> redirige a `/promotor/dashboard` |
| `/promotor/dashboard` | `PromotorDashboardPage` | `DashboardLayout` | Bearer JWT; requiere perfil de promotor |
| `/promotor/perfil` | `PromotorPerfilPage` | `DashboardLayout` | Bearer JWT; requiere perfil de promotor |

**Nota de layout:** Las tres rutas usan `DashboardLayout` (sidebar + main). La pagina `/promotor/registro` es la excepcion semantica: aunque reutiliza el layout con sidebar, tiene una presentacion centrada dentro del main (patron formulario, no de dashboard). Se acepta este enfoque porque el usuario ya esta autenticado y el header/nav estandar del `DashboardLayout` existente aplica correctamente.

---

## 2. Estructura de Carpetas

```
src/web/src/
├── app/
│   └── router.tsx                          MODIFICAR - agregar 3 rutas de promotor
│
└── features/
    └── promotor/
        ├── domain/
        │   └── index.ts                    Re-export de @shared/types/crowdpromotion
        │
        ├── application/
        │   ├── hooks/
        │   │   ├── usePromotor.ts          Query: GET /me
        │   │   ├── useCreatePromotor.ts    Mutation: POST /promotor
        │   │   ├── useUpdatePromotor.ts    Mutation: PUT /me
        │   │   └── useDesactivarPromotor.ts Mutation: PATCH /me/desactivar
        │   └── index.ts                    Barrel exports de hooks
        │
        ├── infrastructure/
        │   └── promotor.service.ts         API calls raw
        │
        └── presentation/
            ├── components/
            │   ├── PromotorForm.tsx         Formulario compartido (registro + edicion)
            │   ├── PromotorKpiCard.tsx      Card de KPI individual del dashboard
            │   ├── PromotorPerfilCard.tsx   Card de resumen del perfil en dashboard
            │   ├── PromotorSocialLinks.tsx  Fila de iconos de redes sociales (lectura)
            │   ├── PromotorSocialFields.tsx Campos de URL de redes en formulario
            │   ├── PromotorDeactivateDialog.tsx AlertDialog de confirmacion de desactivacion
            │   └── index.ts                Barrel exports de componentes
            │
            └── pages/
                ├── PromotorRegistroPage.tsx
                ├── PromotorDashboardPage.tsx
                ├── PromotorPerfilPage.tsx
                └── index.ts
```

---

## 3. Componentes

### 3.1 PromotorRegistroPage

**Archivo:** `src/web/src/features/promotor/presentation/pages/PromotorRegistroPage.tsx`

**Responsabilidad:**
Pagina de registro del promotor. Actua como guard: consulta `usePromotor()` antes de renderizar el formulario. Si el backend devuelve datos (200), redirige a `/promotor/dashboard`. Si devuelve 404 (`errorCode: 2015`), muestra el formulario de alta. Mientras la query carga, muestra un spinner centrado.

**Props:** Ninguna (es una page, se monta via router).

**Estado Local:**
- Ninguno. Todo el estado vive en el hook de mutacion y en React Hook Form via `PromotorForm`.

**Guard de ruta (logica interna):**
```
usePromotor() - retry: false
  isLoading  -> <PageLoader /> centrado
  data       -> <Navigate to="/promotor/dashboard" replace />
  error 404  -> render formulario
  error 401  -> el interceptor de apiClient redirige a /auth/login automaticamente
```

**Dependencias:**
- Hooks: `usePromotor`, `useCreatePromotor`
- Componentes: `PromotorForm`
- Componentes UI: `Card`, `CardContent` (shadcn)
- Iconos: `Megaphone` (lucide-react)
- Router: `Navigate` (react-router-dom)
- Shared: `APP_ROUTES.landing.promotor`

**Layout visual:**
```
[PublicLayout header]
  <main className="flex flex-col items-center py-12 px-4">
    [Icono Megaphone gradient]
    <h1>Conviertete en Promotor</h1>
    <p>Difunde la musica que amas y gana comisiones</p>
    <Card max-w-lg>
      <PromotorForm mode="create" onSubmit={handleCreate} isSubmitting />
    </Card>
  </main>
```

---

### 3.2 PromotorDashboardPage

**Archivo:** `src/web/src/features/promotor/presentation/pages/PromotorDashboardPage.tsx`

**Responsabilidad:**
Dashboard del promotor. Muestra KPIs (programas activos, comisiones ganadas) y la card de resumen del perfil. Si el usuario no tiene perfil (query retorna error 404), redirige a `/promotor/registro`. Si el perfil esta inactivo, muestra un banner de advertencia amarillo.

**Props:** Ninguna.

**Estado Local:**
- Ninguno. Datos desde `usePromotor()`.

**Guard de ruta (logica interna):**
```
usePromotor() - retry: false
  isLoading  -> skeletons en KPI cards y perfil card
  data       -> render dashboard
  error 404  -> <Navigate to="/promotor/registro" replace />
  error 401  -> interceptor redirige a /auth/login
```

**Dependencias:**
- Hooks: `usePromotor`
- Componentes: `PromotorKpiCard`, `PromotorPerfilCard`
- Componentes UI: `Badge`, `Alert`, `AlertDescription`, `Skeleton` (shadcn)
- Iconos: `Activity`, `Wallet`, `Link` (lucide-react)
- Shared: `formatComisionesGanadas`, `APP_ROUTES.landing.promotor`

**Layout visual:**
```
<DashboardLayout>
  <main className="space-y-6">
    [Banner amarillo - solo si esActivo === false]
    <header> Hola, {nombrePublico} | [Editar perfil btn] </header>
    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
      <PromotorKpiCard icon=Activity label="Programas activos" value={totalProgramasActivos} />
      <PromotorKpiCard icon=Wallet label="Comisiones ganadas" value={formatComisionesGanadas(...)} />
    </div>
    <PromotorPerfilCard promotor={data} />
  </main>
</DashboardLayout>
```

---

### 3.3 PromotorPerfilPage

**Archivo:** `src/web/src/features/promotor/presentation/pages/PromotorPerfilPage.tsx`

**Responsabilidad:**
Pagina de edicion del perfil de promotor. Carga el perfil actual via `usePromotor()`, pre-rellena el formulario con los datos actuales usando `mapPromotorToUpdateForm`. Muestra el campo `tipoPromotorNombre` como input deshabilitado con icono Lock (no editable). Al final de la pagina expone la zona de peligro con el boton de desactivacion.

Maneja el "dirty state": si el usuario hace click en Cancelar con cambios no guardados, abre un `AlertDialog` de confirmacion antes de navegar.

**Props:** Ninguna.

**Estado Local:**
- `isDeactivateDialogOpen: boolean` - controla apertura del `PromotorDeactivateDialog`.

**Guard de ruta (logica interna):**
```
usePromotor()
  isLoading  -> inputs en estado skeleton
  data       -> pre-rellenar form con mapPromotorToUpdateForm(data)
  error 404  -> <Navigate to="/promotor/registro" replace />
```

**Dependencias:**
- Hooks: `usePromotor`, `useUpdatePromotor`, `useDesactivarPromotor`
- Componentes: `PromotorForm`, `PromotorDeactivateDialog`
- Componentes UI: `Card`, `CardContent`, `CardHeader`, `CardTitle`, `Button`, `AlertDialog` (shadcn)
- Iconos: `AlertTriangle`, `Lock` (lucide-react)
- Shared: `mapPromotorToUpdateForm`, `APP_ROUTES.landing.promotor`

---

### 3.4 PromotorForm

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorForm.tsx`

**Responsabilidad:**
Formulario reutilizable para registro y edicion del perfil de promotor. Acepta `mode` para diferenciar comportamientos: en modo `"create"` incluye el selector de `tipoPromotorId`; en modo `"edit"` lo muestra como campo deshabilitado (readonly) con el valor textual del tipo. Usa React Hook Form con `zodResolver` aplicando el schema correspondiente a cada modo.

El componente es controlado por el padre via `defaultValues` y `onSubmit`. No gestiona mutaciones internamente.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `mode` | `'create' \| 'edit'` | Si | Determina schema Zod, campos visibles y etiquetas de boton |
| `defaultValues` | `CreatePromotorFormData \| UpdatePromotorFormData` | No | Valores iniciales del formulario (para edicion pre-rellena campos) |
| `onSubmit` | `(data: CreatePromotorFormData \| UpdatePromotorFormData) => Promise<void>` | Si | Handler del submit; el padre lo conecta con la mutation |
| `isSubmitting` | `boolean` | Si | Deshabilita inputs y muestra spinner en el boton |
| `tipoPromotorNombre` | `string` | No | Solo en `mode="edit"`: texto a mostrar en el campo readonly de tipo |
| `onCancel` | `() => void` | Si | Handler del boton Cancelar |

**Estado Local:**
- `showSocialWarning: boolean` - se activa onBlur del ultimo campo de red social si ninguna URL fue rellenada (FA-03). Solo en `mode="create"`.

**Dependencias:**
- Componentes: `PromotorSocialFields`
- Componentes UI: `Input`, `Label`, `Button`, `Select`, `SelectTrigger`, `SelectContent`, `SelectItem` (shadcn)
- Iconos: `Loader2`, `Lock`, `Info` (lucide-react)
- Shared: `createPromotorSchema`, `updatePromotorSchema`, `CreatePromotorFormData`, `UpdatePromotorFormData`, `TIPO_PROMOTOR_LABELS`, `TIPO_PROMOTOR_DESCRIPTIONS`
- Libs: `useForm` (react-hook-form), `zodResolver` (@hookform/resolvers/zod)

**Notas de implementacion:**
- En modo `"create"`, el selector de tipo usa las constantes `TIPO_PROMOTOR_LABELS` (valores fijos del seed); no se hace llamada a API de maestras en MVP.
- El campo `tipoPromotorId` en el selector debe registrarse con `valueAsNumber: true` o el schema debe usar `z.coerce.number()` para evitar problemas de tipo string del `<select>`.
- Los campos de URL vacios se convierten a `undefined` mediante los mappers antes de enviar al backend (la conversion la hace el padre en `onSubmit`, no el formulario).

---

### 3.5 PromotorSocialFields

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorSocialFields.tsx`

**Responsabilidad:**
Agrupa los cuatro campos de URL de redes sociales (Instagram, TikTok, YouTube, Twitter/X) con sus iconos identificadores. Recibe el objeto `register` y `errors` de React Hook Form desde el padre para conectar los inputs sin necesitar un `useFormContext`. Es un componente puramente de presentacion de campos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `register` | `UseFormRegister<CreatePromotorFormData \| UpdatePromotorFormData>` | Si | Funcion register de RHF para bindear los inputs |
| `errors` | `FieldErrors<CreatePromotorFormData \| UpdatePromotorFormData>` | Si | Errores de validacion de RHF para mostrar mensajes |
| `disabled` | `boolean` | No | Deshabilita todos los inputs (estado submitting) |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes UI: `Input`, `Label` (shadcn)
- Iconos: `AlertCircle` (lucide-react) + SVG iconos de redes (inline o lucide equivalentes)

---

### 3.6 PromotorKpiCard

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorKpiCard.tsx`

**Responsabilidad:**
Card de KPI individual para el dashboard. Muestra un icono, un valor numerico o texto formateado, y una etiqueta. Soporta estado de carga mostrando un skeleton animado cuando `isLoading` es `true`.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `icon` | `LucideIcon` | Si | Icono de lucide-react a renderizar en el header de la card |
| `iconColorClass` | `string` | Si | Clase Tailwind del color del icono (ej: `text-[#10b981]`) |
| `iconBgClass` | `string` | Si | Clase Tailwind del fondo del icono (ej: `bg-[rgba(16,185,129,0.15)]`) |
| `label` | `string` | Si | Etiqueta descriptiva del KPI |
| `value` | `string \| number` | Si | Valor a mostrar |
| `isLoading` | `boolean` | No | Muestra skeleton cuando es `true` |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes UI: `Card`, `CardContent`, `CardHeader`, `Skeleton` (shadcn)

---

### 3.7 PromotorPerfilCard

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorPerfilCard.tsx`

**Responsabilidad:**
Card de resumen del perfil en el dashboard. Muestra avatar con iniciales del nombre publico, badge de estado activo/inactivo, tipo de promotor, email, sitio web y los iconos de redes sociales con links. Incluye botones "Editar perfil" y "Ver programas". Cuando `isLoading` es `true` muestra skeletons para todos los campos.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `promotor` | `Promotor` | No | Datos del perfil; si es `undefined` se muestra skeleton |
| `isLoading` | `boolean` | No | Estado de carga |

**Estado Local:** Ninguno.

**Dependencias:**
- Componentes: `PromotorSocialLinks`
- Componentes UI: `Card`, `CardContent`, `CardHeader`, `CardTitle`, `Avatar`, `AvatarFallback`, `Badge`, `Separator`, `Button`, `Skeleton` (shadcn)
- Iconos: `Mail`, `Globe` (lucide-react)
- Router: `Link` (react-router-dom)
- Shared: `APP_ROUTES.landing.promotor`, `TIPO_PROMOTOR_LABELS`, `getPromotorEstado`

---

### 3.8 PromotorSocialLinks

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorSocialLinks.tsx`

**Responsabilidad:**
Renderiza la fila de iconos de redes sociales clicables en modo lectura (dashboard / perfil card). Solo renderiza los iconos de las redes que tienen URL configurada. Si ninguna red tiene URL, muestra el texto muted "Sin redes sociales configuradas".

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `urlInstagram` | `string \| null` | No | URL de Instagram |
| `urlTikTok` | `string \| null` | No | URL de TikTok |
| `urlYouTube` | `string \| null` | No | URL de YouTube |
| `urlTwitter` | `string \| null` | No | URL de Twitter/X |

**Estado Local:** Ninguno.

**Dependencias:**
- Iconos: SVG de plataformas o lucide equivalentes

---

### 3.9 PromotorDeactivateDialog

**Archivo:** `src/web/src/features/promotor/presentation/components/PromotorDeactivateDialog.tsx`

**Responsabilidad:**
`AlertDialog` de confirmacion para la desactivacion del perfil. Diferencia dos casos visuales: si `totalProgramasActivos > 0` muestra el numero de programas afectados y un icono de advertencia (Caso A); si es 0 muestra el mensaje simple (Caso B). Gestiona el estado de carga del boton de confirmacion mientras la mutation esta en progreso.

**Props:**

| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Controla la visibilidad del dialogo |
| `onOpenChange` | `(open: boolean) => void` | Si | Callback de cierre del dialogo |
| `onConfirm` | `() => Promise<void>` | Si | Handler de confirmacion; llama a `useDesactivarPromotor().mutateAsync()` desde el padre |
| `isConfirming` | `boolean` | Si | Deshabilita el boton cancelar y muestra spinner en el boton de confirmacion |
| `totalProgramasActivos` | `number` | Si | Numero de programas que seran afectados (define Caso A vs B) |

**Estado Local:** Ninguno (el estado de apertura lo controla el padre).

**Dependencias:**
- Componentes UI: `AlertDialog`, `AlertDialogAction`, `AlertDialogCancel`, `AlertDialogContent`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogHeader`, `AlertDialogTitle`, `Separator` (shadcn)
- Iconos: `AlertTriangle`, `Loader2` (lucide-react)

---

## 4. Hooks

### 4.1 usePromotor

**Archivo:** `src/web/src/features/promotor/application/hooks/usePromotor.ts`

**Tipo:** Query Hook

**Descripcion:** Obtiene el perfil completo del promotor autenticado. Usado tanto como guard de ruta (en `PromotorRegistroPage` y `PromotorDashboardPage`) como fuente de datos para `PromotorPerfilPage`. Configura `retry: false` para que el error 404 no se reintente; el backend retorna 404 con errorCode `2015` cuando el usuario no tiene perfil.

**Parametros:** Ninguno.

**Retorna:**

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `Promotor \| undefined` | Perfil completo del promotor |
| `isLoading` | `boolean` | `true` mientras la query se ejecuta por primera vez |
| `isError` | `boolean` | `true` si la query falla |
| `error` | `Error \| null` | Objeto de error si la query falla |
| `refetch` | `function` | Para recargar manualmente |

**Query Key:** `QUERY_KEYS.crowdpromotion.promotor.me`

**Configuracion especial:**
- `retry: false` - para no reintentar en 404
- `staleTime: 2 * 60 * 1000` - 2 minutos (el perfil cambia poco frecuentemente)

**Endpoint:** `GET /api/crowdpromotion/promotor/me`

---

### 4.2 useCreatePromotor

**Archivo:** `src/web/src/features/promotor/application/hooks/useCreatePromotor.ts`

**Tipo:** Mutation Hook

**Descripcion:** Crea el perfil de promotor. En `onSuccess` invalida la query del perfil, muestra toast de exito y navega a `/promotor/dashboard`. En `onError` muestra toast de error con el mensaje del backend (usando `getPromotorErrorMessage`). Si el error es `4018` (ya tiene perfil), navega al dashboard en lugar de mostrar error.

**Parametros de `mutate`:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreatePromotorRequest` | Payload del formulario ya mapeado (sin strings vacios) |

**Retorna:** Resultado estandar de `useMutation` de TanStack Query.

**Acciones en `onSuccess`:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })`
- `toast.success('Perfil de promotor creado correctamente')`
- `navigate(APP_ROUTES.landing.promotor.dashboard)`

**Acciones en `onError`:**
- `toast.error(getPromotorErrorMessage(errorCode))` donde `errorCode` se extrae de `error.response.data.messages[0].errorCode`
- Si `errorCode === '4018'`: `navigate(APP_ROUTES.landing.promotor.dashboard)` (guard backup)

**Endpoint:** `POST /api/crowdpromotion/promotor`

---

### 4.3 useUpdatePromotor

**Archivo:** `src/web/src/features/promotor/application/hooks/useUpdatePromotor.ts`

**Tipo:** Mutation Hook

**Descripcion:** Actualiza el perfil del promotor. En `onSuccess` invalida la query del perfil y muestra toast de exito. El formulario permanece visible con los datos actualizados (no navega). En `onError` muestra toast de error.

**Parametros de `mutate`:**

| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `UpdatePromotorRequest` | Payload del formulario ya mapeado |

**Retorna:** Resultado estandar de `useMutation`.

**Acciones en `onSuccess`:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })`
- `toast.success('Perfil actualizado correctamente')`

**Acciones en `onError`:**
- `toast.error(getPromotorErrorMessage(errorCode))`

**Endpoint:** `PUT /api/crowdpromotion/promotor/me`

---

### 4.4 useDesactivarPromotor

**Archivo:** `src/web/src/features/promotor/application/hooks/useDesactivarPromotor.ts`

**Tipo:** Mutation Hook

**Descripcion:** Desactiva el perfil del promotor. En `onSuccess` invalida la query del perfil, muestra toast de exito y redirige a la pagina principal de la landing. En `onError` muestra toast de error y cierra el dialogo sin navegar.

**Parametros de `mutate`:** Ninguno (body vacio, el endpoint no requiere body).

**Retorna:** Resultado estandar de `useMutation`.

**Acciones en `onSuccess`:**
- `queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })`
- `toast.success('Perfil de promotor desactivado')`
- `navigate(APP_ROUTES.landing.home)` (pagina principal)

**Acciones en `onError`:**
- Si `errorCode === '4019'`: `toast.info('Tu perfil de promotor ya estaba desactivado')` + invalidar query
- En otros casos: `toast.error(getPromotorErrorMessage(errorCode))`

**Endpoint:** `PATCH /api/crowdpromotion/promotor/me/desactivar`

---

### 4.5 usePromotorGuard (helper interno, no hook standalone)

**Nota:** La logica de guard no se extrae como hook independiente. Cada page implementa el guard directamente usando `usePromotor()` con una lectura del estado `isLoading / data / error`. Esto evita sobre-abstraer un comportamiento que difiere ligeramente entre pages (registro vs dashboard vs perfil).

---

## 5. Services

### 5.1 promotorService

**Archivo:** `src/web/src/features/promotor/infrastructure/promotor.service.ts`

**Descripcion:** Servicio que encapsula todas las llamadas HTTP al modulo Crowdpromotion. Sigue el mismo patron de `campaniaApi` y `artistaService` del proyecto: clase instanciada como singleton, usando `apiFetch` de `@/lib/api-client`, wrapeando `ServiceResponse<T>`. Lanza `Error` con el mensaje del backend si `isSuccess` es falso, propagando el `errorCode` en el mensaje para que los hooks puedan inspeccionarlo.

**Patron de error handling:**
Cada metodo extrae el `errorCode` del primer mensaje de error y lo adjunta al objeto `Error` como propiedad `errorCode` (o incluido en el mensaje). Esto permite a los hooks diferenciar el tipo de error para mostrar mensajes contextualizados.

```
Error.message = messages[0]?.message || 'Error desconocido'
Error.cause   = { errorCode: messages[0]?.errorCode }
```

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getMe()` | - | `Promise<Promotor>` | `/api/crowdpromotion/promotor/me` | GET |
| `create(data)` | `CreatePromotorRequest` | `Promise<PromotorCreatedResult>` | `/api/crowdpromotion/promotor` | POST |
| `update(data)` | `UpdatePromotorRequest` | `Promise<PromotorUpdatedResult>` | `/api/crowdpromotion/promotor/me` | PUT |
| `desactivar()` | - | `Promise<PromotorDesactivadoResult>` | `/api/crowdpromotion/promotor/me/desactivar` | PATCH |

**Imports:**
- `apiFetch` desde `@/lib/api-client`
- `ServiceResponse` desde `@shared/types/api`
- `Promotor`, `PromotorCreatedResult`, `PromotorUpdatedResult`, `PromotorDesactivadoResult`, `CreatePromotorRequest`, `UpdatePromotorRequest` desde `@shared/types/crowdpromotion`
- `API_ROUTES` desde `@shared/constants`

---

## 6. Flujo de Datos

### Flujo de Registro

```
Usuario accede a /promotor/registro
    |
    v
PromotorRegistroPage
    |-- usePromotor() [retry: false]
    |       |
    |       |-- isLoading: true  --> <PageLoader />
    |       |-- data existe     --> <Navigate to="/promotor/dashboard" />
    |       |-- error 404       --> render formulario
    |
    v
PromotorForm [mode="create"]
    |-- React Hook Form + createPromotorSchema (Zod)
    |-- useCreatePromotor().mutate(mapCreateFormToRequest(formData))
    |
    v
promotorService.create(data)
    |-- POST /api/crowdpromotion/promotor
    |
    v
Backend (ServiceResponse<PromotorCreatedResult>)
    |
    |-- onSuccess:
    |       queryClient.invalidateQueries(promotor.me)
    |       toast.success(...)
    |       navigate("/promotor/dashboard")
    |
    |-- onError:
            toast.error(getPromotorErrorMessage(errorCode))
```

### Flujo de Edicion

```
Usuario accede a /promotor/perfil
    |
    v
PromotorPerfilPage
    |-- usePromotor() [con staleTime]
    |       |
    |       |-- data -> mapPromotorToUpdateForm(data) -> defaultValues del form
    |       |-- error 404 -> <Navigate to="/promotor/registro" />
    |
    v
PromotorForm [mode="edit", defaultValues=..., tipoPromotorNombre=...]
    |-- React Hook Form + updatePromotorSchema (Zod)
    |-- useUpdatePromotor().mutate(mapUpdateFormToRequest(formData))
    |
    v
promotorService.update(data)
    |-- PUT /api/crowdpromotion/promotor/me
    |
    v
Backend (ServiceResponse<PromotorUpdatedResult>)
    |
    |-- onSuccess:
    |       queryClient.invalidateQueries(promotor.me)
    |       toast.success("Perfil actualizado correctamente")
    |       [formulario permanece, dirty state se resetea]
    |
    |-- onError:
            toast.error(...)
```

### Flujo de Desactivacion

```
Click "Desactivar cuenta" en PromotorPerfilPage
    |
    v
setIsDeactivateDialogOpen(true)
    |
    v
PromotorDeactivateDialog [open=true, totalProgramasActivos=N]
    |-- Caso A (N > 0): mensaje con numero de programas afectados
    |-- Caso B (N === 0): mensaje de confirmacion simple
    |
Click "Desactivar cuenta" (confirmacion)
    |
    v
useDesactivarPromotor().mutateAsync()
    |
    v
promotorService.desactivar()
    |-- PATCH /api/crowdpromotion/promotor/me/desactivar
    |
    v
Backend (ServiceResponse<PromotorDesactivadoResult>)
    |
    |-- onSuccess:
    |       queryClient.invalidateQueries(promotor.me)
    |       toast.success("Perfil de promotor desactivado")
    |       navigate("/")
    |
    |-- onError:
            toast.error(...)
            dialog se cierra [onOpenChange(false)]
```

---

## 7. Routing

### Cambios en `src/web/src/app/router.tsx`

Agregar imports lazy para las tres pages nuevas:

```
const PromotorRegistroPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorRegistroPage"))
const PromotorDashboardPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorDashboardPage"))
const PromotorPerfilPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorPerfilPage"))
```

Agregar rutas dentro del bloque `<Route element={<DashboardLayout />}>` (rutas protegidas con auth guard del layout):

```
<Route path="/promotor/registro"   element={<PromotorRegistroPage />} />
<Route path="/promotor/dashboard"  element={<PromotorDashboardPage />} />
<Route path="/promotor/perfil"     element={<PromotorPerfilPage />} />
```

**Nota:** El `DashboardLayout` existente ya gestiona el redirect a `/auth/login` si `!isAuthenticated`, cubriendo el criterio AC-CP01-12. El guard adicional de "ya tiene perfil" lo implementa cada page internamente via `usePromotor()`.

### Sidebar (`src/web/src/components/layout/Sidebar.tsx`)

Agregar items de menu del promotor al array `menuItems` del Sidebar. Los items deben aparecer condicionalmente: solo si el usuario tiene perfil de promotor activo (se puede detectar con una query `usePromotor` en el Sidebar con `enabled: isAuthenticated`).

Items a agregar:

| Label | Icono | Href |
|-------|-------|------|
| "Promotor" (separador de seccion) | - | - |
| "Dashboard Promotor" | `Megaphone` | `/promotor/dashboard` |
| "Mi Perfil Promotor" | `UserCheck` | `/promotor/perfil` |

**Alternativa simplificada para MVP:** Agregar los items siempre visibles para usuarios autenticados sin condicional (el guard de ruta maneja el redirect si no tiene perfil). Esto evita una query adicional en el Sidebar.

---

## 8. Dependencias de Shared

**Importar de `@shared/`:**

### Types
```typescript
import type {
    Promotor,
    PromotorCreatedResult,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
    CreatePromotorRequest,
    UpdatePromotorRequest,
    PromotorEstado,
} from '@shared/types/crowdpromotion'
import type { ServiceResponse } from '@shared/types/api'
```

### Schemas
```typescript
import {
    createPromotorSchema,
    updatePromotorSchema,
    type CreatePromotorFormData,
    type UpdatePromotorFormData,
} from '@shared/schemas/crowdpromotion.schema'
```

### Constants
```typescript
import {
    QUERY_KEYS,           // QUERY_KEYS.crowdpromotion.promotor.me
    API_ROUTES,           // API_ROUTES.crowdpromotion.promotor.*
    APP_ROUTES,           // APP_ROUTES.landing.promotor.*
    TIPO_PROMOTOR_LABELS,
    TIPO_PROMOTOR_DESCRIPTIONS,
} from '@shared/constants'
```

### Utils
```typescript
import {
    mapCreateFormToRequest,
    mapUpdateFormToRequest,
    mapPromotorToUpdateForm,
    getPromotorEstado,
} from '@shared/utils/mappers'

import { formatComisionesGanadas } from '@shared/utils/format'

import {
    getPromotorErrorMessage,
    PROMOTOR_ERROR_MESSAGES,
} from '@shared/utils/error-messages'
```

---

## 9. Componentes shadcn/ui Requeridos

Los siguientes componentes de shadcn deben estar disponibles en `src/web/src/components/ui/`. Verificar si ya estan instalados:

| Componente | Uso | Estado probable |
|------------|-----|-----------------|
| `Card`, `CardContent`, `CardHeader`, `CardTitle` | PromotorForm, PromotorPerfilCard, PromotorKpiCard | Ya existe (`card.tsx`) |
| `Input` | PromotorForm, PromotorSocialFields | Ya existe (`input.tsx`) |
| `Label` | PromotorForm, PromotorSocialFields | Ya existe (`label.tsx`) |
| `Button` | Todas las pages y formularios | Ya existe (referenciado en codigo) |
| `Select`, `SelectTrigger`, `SelectContent`, `SelectItem` | PromotorForm (selector de tipo) | Ya existe (`select.tsx`) |
| `Avatar`, `AvatarFallback` | PromotorPerfilCard | Ya existe (`avatar.tsx`) |
| `Badge` | PromotorPerfilCard | Ya existe (`badge.tsx`) |
| `Separator` | PromotorPerfilCard, PromotorDeactivateDialog | Ya existe (`separator.tsx`) |
| `Skeleton` | PromotorKpiCard, PromotorPerfilCard | Ya existe (`skeleton.tsx`) |
| `AlertDialog`, `AlertDialogAction`, `AlertDialogCancel`, `AlertDialogContent`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogHeader`, `AlertDialogTitle` | PromotorDeactivateDialog | Verificar (no se ve en `src/web/src/components/ui/`) |
| `Alert`, `AlertDescription` | PromotorDashboardPage (banner perfil inactivo) | Ya existe (`alert.tsx`) |

**AlertDialog** es el componente mas probable que falte. Instalarlo con:
```bash
npx shadcn@latest add alert-dialog
```

---

## 10. Archivos a Crear y Modificar

### Archivos Nuevos (feature promotor)

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/promotor/domain/index.ts` | TypeScript | Re-export de tipos de `@shared/types/crowdpromotion` |
| `src/web/src/features/promotor/infrastructure/promotor.service.ts` | TypeScript | Clase `PromotorService` con 4 metodos HTTP |
| `src/web/src/features/promotor/application/hooks/usePromotor.ts` | TypeScript | Query hook GET /me |
| `src/web/src/features/promotor/application/hooks/useCreatePromotor.ts` | TypeScript | Mutation hook POST |
| `src/web/src/features/promotor/application/hooks/useUpdatePromotor.ts` | TypeScript | Mutation hook PUT |
| `src/web/src/features/promotor/application/hooks/useDesactivarPromotor.ts` | TypeScript | Mutation hook PATCH |
| `src/web/src/features/promotor/application/index.ts` | TypeScript | Barrel exports de hooks |
| `src/web/src/features/promotor/presentation/components/PromotorForm.tsx` | React TSX | Formulario compartido |
| `src/web/src/features/promotor/presentation/components/PromotorSocialFields.tsx` | React TSX | Campos URL de redes sociales |
| `src/web/src/features/promotor/presentation/components/PromotorKpiCard.tsx` | React TSX | Card de KPI |
| `src/web/src/features/promotor/presentation/components/PromotorPerfilCard.tsx` | React TSX | Card de resumen de perfil |
| `src/web/src/features/promotor/presentation/components/PromotorSocialLinks.tsx` | React TSX | Iconos de redes sociales en lectura |
| `src/web/src/features/promotor/presentation/components/PromotorDeactivateDialog.tsx` | React TSX | AlertDialog de confirmacion |
| `src/web/src/features/promotor/presentation/components/index.ts` | TypeScript | Barrel exports de componentes |
| `src/web/src/features/promotor/presentation/pages/PromotorRegistroPage.tsx` | React TSX | Pagina de registro |
| `src/web/src/features/promotor/presentation/pages/PromotorDashboardPage.tsx` | React TSX | Dashboard del promotor |
| `src/web/src/features/promotor/presentation/pages/PromotorPerfilPage.tsx` | React TSX | Edicion de perfil |
| `src/web/src/features/promotor/presentation/pages/index.ts` | TypeScript | Barrel exports de pages |
| `src/web/src/features/promotor/index.ts` | TypeScript | Barrel principal de la feature |

### Archivos Existentes a Modificar

| Archivo | Cambio |
|---------|--------|
| `src/web/src/app/router.tsx` | Agregar 3 imports lazy + 3 `<Route>` dentro del bloque `DashboardLayout` |
| `src/web/src/components/layout/Sidebar.tsx` | Agregar items de menu del promotor |
| `src/shared/types/crowdpromotion.ts` | CREAR (segun plan de shared) |
| `src/shared/schemas/crowdpromotion.schema.ts` | CREAR (segun plan de shared) |
| `src/shared/constants/index.ts` | Agregar bloques `crowdpromotion` (segun plan de shared) |
| `src/shared/utils/error-messages.ts` | Agregar errores de promotor (segun plan de shared) |
| `src/shared/utils/mappers.ts` | Agregar mappers de promotor (segun plan de shared) |
| `src/shared/utils/format.ts` | Agregar `formatComisionesGanadas` (segun plan de shared) |

---

## 11. Consideraciones de Implementacion

### 11.1 Conversion de strings vacios en URLs

El formulario HTML devuelve `''` para inputs no rellenados. El schema Zod usa `.optional().or(z.literal(''))` para aceptar ambos. Antes de llamar al servicio, el handler `onSubmit` de la page aplica el mapper correspondiente:

```
mapCreateFormToRequest(formData) // '' -> undefined para campos opcionales
mapUpdateFormToRequest(formData) // '' -> undefined para campos opcionales
```

El `PromotorForm` recibe `onSubmit` del padre; la page es quien aplica el mapper y llama a `mutation.mutate()`.

### 11.2 tipoPromotorId en el select

El `<select>` nativo y el componente `<Select>` de shadcn devuelven strings. Para que Zod valide el numero correctamente, el schema debe usar `z.coerce.number()` en lugar de `z.number()` para `tipoPromotorId`. El plan de shared ya documenta esta recomendacion en la nota 10.2.

### 11.3 Dirty state en PromotorPerfilPage

React Hook Form expone `formState.isDirty`. El boton "Guardar cambios" usa `disabled={!isDirty || isSubmitting}`. Si el usuario hace click en Cancelar con `isDirty === true`, la page abre un `AlertDialog` de confirmacion antes de navegar (distinto del `PromotorDeactivateDialog`). Este segundo dialog se implementa directamente en `PromotorPerfilPage` sin extraer componente.

### 11.4 Manejo de errores del servicio

Los errores del backend tienen la forma `ServiceResponse<T>` con `messages[0].errorCode`. El `promotorService` relanza un `Error` enriquecido para que los hooks de mutation puedan inspeccionarlo:

```typescript
const error = new Error(messages[0]?.message || 'Error desconocido')
;(error as Error & { errorCode: string }).errorCode = messages[0]?.errorCode ?? '5000'
throw error
```

En los hooks, `onError(error)` extrae `errorCode` con:
```typescript
const errorCode = (error as Error & { errorCode?: string }).errorCode ?? '5000'
```

### 11.5 Estados de loading progresivos en dashboard

`PromotorDashboardPage` muestra skeletons mientras `isLoading` es `true`. Los componentes `PromotorKpiCard` y `PromotorPerfilCard` aceptan prop `isLoading` y renderizan su propio skeleton internamente, evitando logica condicional en la page.

### 11.6 Redes sociales: aviso FA-03

En `PromotorForm [mode="create"]`, el estado local `showSocialWarning` se activa en el handler `onBlur` del ultimo campo de red social (`urlTwitter`). La condicion es: `!formValues.urlInstagram && !formValues.urlTikTok && !formValues.urlYouTube && !formValues.urlTwitter`. Este aviso es informativo y no bloquea el submit.

---

## 12. Checklist

### Estructura y arquitectura
- [ ] Feature-based en `src/web/src/features/promotor/` con las 4 capas (domain, application, infrastructure, presentation)
- [ ] Barrel exports en cada capa (`index.ts`)
- [ ] Tipos importados de `@shared/types/crowdpromotion` (no duplicados en la feature)

### Componentes
- [ ] Todos los componentes son functional components (`FC<Props>`)
- [ ] Todas las interfaces de Props son explicitas y tipadas
- [ ] Componentes usan shadcn/ui (no HTML nativo para botones, inputs, cards)
- [ ] `AlertDialog` de shadcn instalado si no existe
- [ ] `PromotorForm` funciona en modo `"create"` y `"edit"` sin duplicar logica

### Hooks
- [ ] `usePromotor` usa `retry: false`
- [ ] `useCreatePromotor` y `useUpdatePromotor` aplican mappers antes de llamar al servicio
- [ ] Todos los hooks de mutation invalidan `QUERY_KEYS.crowdpromotion.promotor.me` en `onSuccess`
- [ ] Todos los hooks muestran toast en `onSuccess` y `onError` (via sonner, que ya usa el proyecto)
- [ ] `useDesactivarPromotor` redirige a home en `onSuccess`

### Services
- [ ] `promotorService` usa `apiFetch` de `@/lib/api-client` (no axios directo)
- [ ] `promotorService` usa constantes `API_ROUTES.crowdpromotion.promotor.*` (no strings hardcodeados)
- [ ] Errores propagados con `errorCode` accesible en los hooks

### Routing y guards
- [ ] 3 rutas agregadas en `router.tsx` con lazy loading
- [ ] Las 3 rutas estan dentro del bloque `DashboardLayout` (auth guard existente)
- [ ] `PromotorRegistroPage` redirige a dashboard si ya tiene perfil (guard interno)
- [ ] `PromotorDashboardPage` redirige a registro si no tiene perfil (guard interno)
- [ ] `PromotorPerfilPage` redirige a registro si no tiene perfil (guard interno)

### Formularios
- [ ] `PromotorForm` usa React Hook Form con `zodResolver`
- [ ] Schema de creacion incluye `tipoPromotorId` con `z.coerce.number()`
- [ ] Schema de edicion NO incluye `tipoPromotorId`
- [ ] Conversion de `'' -> undefined` aplicada antes de enviar al backend
- [ ] Dirty state detectado para deshabilitar "Guardar" y confirmar cancelacion en edicion

### UX
- [ ] Estados de loading con skeletons en KPI cards y perfil card
- [ ] Toast de exito/error en todas las acciones de escritura
- [ ] Banner de advertencia cuando `esActivo === false` en dashboard
- [ ] `PromotorDeactivateDialog` diferencia Caso A (con programas) y Caso B (sin programas)
- [ ] Aviso informativo FA-03 cuando no se rellena ninguna red social (solo en registro)
- [ ] Campo tipo promotor claramente deshabilitado con icono Lock en modo edicion

### TypeScript
- [ ] Sin uso de `any`
- [ ] Sin `console.log` en codigo productivo
- [ ] Path aliases usados (`@/` para web, `@shared/` para shared)
