# Plan Frontend: Perfil de Promotor (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-perfil-promotor (US-CP-01)
**Target:** src/admin (Next.js 14 - App Router)

---

## 1. Resumen

- **Screens:** 2 (PromotorPage con tabs Vista + Edicion, no hay registro en admin)
- **Componentes:** 8
- **Hooks:** 3
- **Services:** 1

### Alcance en Admin

El Admin NO implementa el registro de promotor (eso es responsabilidad de la Landing). El Admin:

1. Muestra una nueva seccion en el dashboard si el usuario autenticado tiene perfil de promotor
2. Provee una pagina dedicada `/promotor` con vista del perfil y acceso a la edicion
3. Muestra KPIs de programas activos y comisiones ganadas
4. Permite editar el perfil (nombre publico, email, URLs de redes)
5. Permite desactivar el perfil con dialogo de confirmacion informativo
6. Muestra el tipo de promotor en solo lectura (AC-CP01-9)

### Criterios de Aceptacion relevantes

| ID | Criterio | Notas |
|----|----------|-------|
| AC-CP01-8 | El promotor puede editar su perfil desde admin; tras guardar, FechaActualizacion se actualiza | Formulario de edicion en `/promotor` |
| AC-CP01-9 | El campo Tipo de Promotor no es editable una vez creado el perfil | Campo readonly en el formulario de edicion |
| AC-CP01-11 | Formularios usan schemas Zod de shared | `updatePromotorSchema` de `@shared/schemas` |

---

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── promotor/
│           ├── page.tsx                          # Server Component - metadata
│           └── components/
│               ├── PromotorPageClient.tsx        # Client Component - logica principal
│               ├── PromotorKpiCards.tsx          # KPI cards: programas activos, comisiones
│               ├── PromotorProfileCard.tsx       # Vista de perfil (datos, estado, tipo)
│               ├── PromotorEditForm.tsx          # Formulario de edicion (RHF + Zod)
│               ├── PromotorSocialLinks.tsx       # Seccion de URLs de redes sociales (readonly)
│               ├── PromotorStatusBadge.tsx       # Badge activo/inactivo
│               ├── DesactivarPromotorDialog.tsx  # Dialogo de confirmacion de desactivacion
│               └── PromotorNoEncontrado.tsx      # Estado cuando el usuario no es promotor
│
├── hooks/
│   ├── use-promotor.ts                           # Query GET /api/crowdpromotion/promotor/me
│   ├── use-promotor-mutations.ts                 # Mutations: update + desactivar
│   └── index.ts                                  # (modificar: agregar exports)
│
├── services/
│   ├── promotor.service.ts                       # API calls del modulo promotor
│   └── index.ts                                  # (modificar: agregar export)
│
└── components/
    └── dashboard/
        └── PromotorBanner.tsx                    # Banner en dashboard si usuario es promotor
```

---

## 3. Componentes

### 3.1 `page.tsx` (Server Component)

**Archivo:** `src/admin/src/app/(dashboard)/promotor/page.tsx`

**Tipo:** Server Component (Next.js 14)

**Responsabilidad:**
- Definir metadata de la pagina (`title`, `description`)
- Renderizar el Client Component `PromotorPageClient` sin logica propia
- No hace fetch de datos (la data se carga en el cliente via TanStack Query)

**Metadata:**
```
title: "Mi Perfil de Promotor | WePlay Rises"
description: "Gestiona tu perfil de promotor en WePlay Rises"
```

**No tiene props** (es una page de Next.js).

---

### 3.2 `PromotorPageClient`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorPageClient.tsx`

**Directiva:** `"use client"`

**Props:** Ninguna

**Estado Local:**
- `isEditing: boolean` - Controla si se muestra el formulario de edicion o la vista de perfil
- `showDesactivarDialog: boolean` - Controla visibilidad del dialogo de desactivacion

**Dependencias:**
- Hooks: `usePromotor`
- Componentes: `PromotorKpiCards`, `PromotorProfileCard`, `PromotorEditForm`, `DesactivarPromotorDialog`, `PromotorNoEncontrado`
- shadcn/ui: `Skeleton`, `Button`, `Tabs`, `TabsContent`, `TabsList`, `TabsTrigger`
- lucide-react: `Megaphone`, `Settings`

**Responsabilidad:**
Orquesta toda la pagina de perfil de promotor. Maneja los estados de carga, error y los dos modos de la pagina (vista y edicion). Si el usuario no tiene perfil de promotor (404), renderiza `PromotorNoEncontrado`.

**Flujo:**
1. Llama a `usePromotor()` para obtener datos
2. Si `isLoading` → muestra skeletons del layout completo
3. Si `!data` (404 / sin perfil) → renderiza `PromotorNoEncontrado`
4. Si `data` → renderiza header de pagina + `PromotorKpiCards` + seccion con tabs (Vista | Edicion)
5. Tab "Vista" → `PromotorProfileCard`
6. Tab "Edicion" → `PromotorEditForm`
7. Boton "Desactivar cuenta" abre `DesactivarPromotorDialog`

**Estructura de Tabs:**
- Tab "Perfil" (`value="perfil"`) → `PromotorProfileCard`
- Tab "Editar" (`value="editar"`) → `PromotorEditForm`

---

### 3.3 `PromotorKpiCards`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorKpiCards.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `totalProgramasActivos` | `number` | Si | Numero de programas activos del promotor |
| `totalComisionesGanadas` | `number` | Si | Total ganado en EUR |
| `monedaComisiones` | `string` | Si | Codigo ISO de moneda (siempre "EUR" en MVP) |
| `esActivo` | `boolean` | Si | Estado del promotor para deshabilitar KPI si inactivo |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes: `StatsCard` (componente existente en `@/components/dashboard/stats-card`)
- Utiles: `formatComisionesGanadas` de `@shared/utils`
- lucide-react: `Activity`, `Wallet`, `TrendingUp`

**Responsabilidad:**
Muestra 2 tarjetas KPI en un grid de 2 columnas:
1. "Programas Activos" - valor `totalProgramasActivos` con icono `Activity`
2. "Comisiones Ganadas" - valor formateado con `formatComisionesGanadas(totalComisionesGanadas, monedaComisiones)` con icono `Wallet`

Reutiliza el componente `StatsCard` existente para mantener consistencia visual con el resto del dashboard.

---

### 3.4 `PromotorProfileCard`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorProfileCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `promotor` | `Promotor` | Si | Datos completos del perfil del promotor |
| `onEditClick` | `() => void` | Si | Callback para cambiar al modo edicion |
| `onDesactivarClick` | `() => void` | Si | Callback para abrir dialogo de desactivacion |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes hijos: `PromotorStatusBadge`, `PromotorSocialLinks`
- shadcn/ui: `Card`, `CardContent`, `CardHeader`, `CardTitle`, `CardDescription`, `Button`, `Separator`, `Badge`
- lucide-react: `User`, `Mail`, `Globe`, `Edit`, `PowerOff`
- Constantes: `TIPO_PROMOTOR_LABELS` de `@shared/constants`

**Responsabilidad:**
Vista de solo lectura del perfil del promotor. Organiza la informacion en secciones:

1. **Header de card**: Nombre publico + `PromotorStatusBadge` (activo/inactivo) + boton "Editar perfil"
2. **Seccion datos basicos**:
   - Tipo de Promotor: `TIPO_PROMOTOR_LABELS[promotor.tipoPromotorId]` con `Badge` en readonly (no editable, AC-CP01-9)
   - Email de contacto: con icono `Mail`, texto `"No indicado"` si es null
   - Sitio web: con icono `Globe`, texto `"No indicado"` si es null; si tiene valor, es un link externo
3. **Seccion redes sociales**: `PromotorSocialLinks`
4. **Seccion peligro** (al final, separada por `<Separator />`): Boton "Desactivar cuenta de promotor" con variante destructiva. Se oculta si `!promotor.esActivo`.
5. **Footer**: Fecha de creacion del perfil en formato `dd/MM/yyyy`

---

### 3.5 `PromotorEditForm`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorEditForm.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `promotor` | `Promotor` | Si | Datos actuales para pre-rellenar el formulario |
| `onSuccess` | `() => void` | Si | Callback ejecutado tras guardar exitosamente |
| `onCancel` | `() => void` | Si | Callback para volver a la vista de perfil sin guardar |

**Estado Local:**
- El formulario es controlado por React Hook Form (no se necesita `useState` adicional)

**Dependencias:**
- Hooks: `useUpdatePromotor` de `use-promotor-mutations`
- Utiles: `mapPromotorToUpdateForm`, `mapUpdateFormToRequest` de `@shared/utils`
- Schemas: `updatePromotorSchema`, `UpdatePromotorFormData` de `@shared/schemas`
- shadcn/ui: `Card`, `CardContent`, `CardHeader`, `CardTitle`, `CardDescription`, `Button`, `Input`, `Label`, `Separator`
- RHF: `useForm`, `zodResolver`
- lucide-react: `Instagram`, `Globe`, `Youtube`, `Twitter`, `Save`, `X`, `Loader2`

**Campos del formulario:**

| Campo | Tipo Input | Editable | Regla |
|-------|-----------|----------|-------|
| Nombre publico | `Input text` | Si | min 3, max 200 |
| Email de contacto | `Input email` | Si | formato email, max 200, opcional |
| Sitio web | `Input url` | Si | formato URL, max 300, opcional |
| URL Instagram | `Input url` | Si | formato URL, max 300, opcional |
| URL TikTok | `Input url` | Si | formato URL, max 300, opcional |
| URL YouTube | `Input url` | Si | formato URL, max 300, opcional |
| URL Twitter/X | `Input url` | Si | formato URL, max 300, opcional |
| Tipo de Promotor | `Input disabled` | **No** | readonly, valor de `TIPO_PROMOTOR_LABELS[promotor.tipoPromotorId]` (AC-CP01-9) |

**Flujo de envio:**
1. `useForm` inicializado con `mapPromotorToUpdateForm(promotor)` como `defaultValues`
2. `handleSubmit` llama a `updatePromotor.mutate(mapUpdateFormToRequest(formData))`
3. `onSuccess` del mutation: toast de exito + llama al prop `onSuccess()`
4. `onError` del mutation: toast de error con mensaje del backend

**Botones:**
- "Cancelar" → llama a `onCancel()`, variante `outline`
- "Guardar cambios" → submit, variante default con gradiente pink/purple, muestra `Loader2` si `isPending`

**Responsabilidad:**
Formulario de edicion del perfil del promotor con validacion Zod. El campo tipo de promotor se muestra pero no es editable (input con `disabled`). Convierte datos del formulario al formato de la API usando mappers de shared antes de enviar.

---

### 3.6 `PromotorSocialLinks`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorSocialLinks.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `urlInstagram` | `string \| null` | Si | URL de Instagram o null |
| `urlTikTok` | `string \| null` | Si | URL de TikTok o null |
| `urlYouTube` | `string \| null` | Si | URL de YouTube o null |
| `urlTwitter` | `string \| null` | Si | URL de Twitter/X o null |
| `urlSitioWeb` | `string \| null` | Si | URL del sitio web o null |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Badge`
- lucide-react: `Instagram`, `Globe`, `Youtube`, `Twitter`, `ExternalLink`

**Responsabilidad:**
Muestra las URLs de redes sociales del promotor en formato readonly como lista de enlaces. Cada red con URL muestra: icono de red + nombre + link externo con icono `ExternalLink`. Si no hay ninguna red configurada, muestra texto `"Sin redes sociales configuradas"` en `text-muted-foreground`.

Diseño: lista vertical con `gap-2`. Cada item: `flex items-center gap-2 text-sm`. Los links abren en `target="_blank" rel="noopener noreferrer"`.

---

### 3.7 `PromotorStatusBadge`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorStatusBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `esActivo` | `boolean` | Si | Estado del promotor |
| `className` | `string` | No | Clases adicionales opcionales |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Badge`
- Utiles: `getPromotorEstado` de `@shared/utils`

**Responsabilidad:**
Badge visual que indica si el promotor esta activo o inactivo.
- `esActivo = true` → `Badge` con clase `bg-green-500/15 text-green-400 border-green-500/30` y texto "Activo"
- `esActivo = false` → `Badge` con clase `bg-slate-500/15 text-slate-400 border-slate-500/30` y texto "Inactivo"

Sigue el patron de `CampaniaStatusBadge` existente en el proyecto.

---

### 3.8 `DesactivarPromotorDialog`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/DesactivarPromotorDialog.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Controla visibilidad del dialog |
| `onOpenChange` | `(open: boolean) => void` | Si | Handler de cambio de visibilidad |
| `totalProgramasActivos` | `number` | Si | Numero de programas que se veran afectados |

**Estado Local:** Ninguno (la mutacion se maneja via hook)

**Dependencias:**
- Hooks: `useDesactivarPromotor` de `use-promotor-mutations`
- shadcn/ui: `AlertDialog`, `AlertDialogAction`, `AlertDialogCancel`, `AlertDialogContent`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogHeader`, `AlertDialogTitle`, `Alert`, `AlertDescription`
- lucide-react: `AlertTriangle`, `Loader2`

**Responsabilidad:**
Dialogo de confirmacion para desactivar el perfil de promotor. Muestra un mensaje diferenciado segun si hay programas activos afectados:

- Si `totalProgramasActivos > 0`:
  - Titulo: "Desactivar perfil de promotor"
  - Descripcion principal: "Esta accion desactivara tu perfil de promotor."
  - `Alert` con variante warning dentro del dialog: "Se daran de baja automaticamente {N} programa(s) de promocion activo(s)."
  - Boton confirmar en rojo: "Si, desactivar"

- Si `totalProgramasActivos === 0`:
  - Titulo: "Desactivar perfil de promotor"
  - Descripcion: "Esta accion desactivara tu perfil de promotor. Podras contactar con soporte para reactivarlo."
  - Boton confirmar en rojo: "Si, desactivar"

Al confirmar, llama a `desactivarPromotor.mutate()`. El boton de confirmar muestra `Loader2` si `isPending`. El boton cancelar se deshabilita durante `isPending`.

Sigue el patron de `DeleteConfirmDialog` existente en `crowdsourcing/templates`.

---

### 3.9 `PromotorNoEncontrado`

**Archivo:** `src/admin/src/app/(dashboard)/promotor/components/PromotorNoEncontrado.tsx`

**Props:** Ninguna

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Card`, `CardContent`, `Button`
- lucide-react: `Megaphone`

**Responsabilidad:**
Estado vacio que se muestra cuando el usuario autenticado no tiene perfil de promotor. Muestra icono + mensaje explicativo + boton que redirige a la Landing en `/promotor/registro` (link externo al admin).

Texto: "No tienes un perfil de promotor. Registrate en la plataforma para acceder a las funcionalidades de Crowdpromotion."

Boton: "Ir a registro de promotor" → `href` hacia el URL de la landing en `/promotor/registro` (usa variable de entorno `NEXT_PUBLIC_LANDING_URL`).

---

### 3.10 `PromotorBanner`

**Archivo:** `src/admin/src/components/dashboard/PromotorBanner.tsx`

**Directiva:** `"use client"`

**Props:** Ninguna

**Estado Local:**
- `dismissed: boolean` - Persiste en localStorage con key `"promotor-banner-dismissed"`

**Dependencias:**
- Hooks: `usePromotor`
- shadcn/ui: `Card`, `CardContent`, `Button`
- Next.js: `Link`
- lucide-react: `Megaphone`, `X`, `ExternalLink`

**Responsabilidad:**
Banner en el dashboard principal que aparece si el usuario tiene perfil de promotor activo. Informa al usuario que tiene perfil de promotor y ofrece acceso rapido a `/promotor`.

**Logica de visibilidad:**
- Si `isLoading` → `null` (no renderiza nada)
- Si `!data` (sin perfil de promotor, error 404) → `null` (no renderiza nada; el banner no invita a registrarse, eso es responsabilidad de la Landing)
- Si `data && data.esActivo && !dismissed` → renderiza el banner
- Si `data && !data.esActivo` → `null` (perfil inactivo no muestra banner)

**Contenido del banner:**
- Icono `Megaphone` con gradiente pink/purple
- Texto: "Eres promotor activo" + descripcion breve con numero de programas activos
- Link `"Ver mi perfil de promotor"` → `/promotor`
- Boton X para descartar (persistido en localStorage)

Sigue el patron de `CompleteProfileBanner` existente.

---

## 4. Hooks

### 4.1 `usePromotor`

**Archivo:** `src/admin/src/hooks/use-promotor.ts`

**Tipo:** Query Hook

**Parametros:** Ninguno (usa el JWT del usuario autenticado)

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `Promotor \| undefined` | Perfil completo del promotor |
| `isLoading` | `boolean` | Estado de carga inicial |
| `isFetching` | `boolean` | Estado de re-fetch en background |
| `error` | `Error \| null` | Error si hay (incluyendo 404) |
| `isError` | `boolean` | True si hubo error |
| `refetch` | `function` | Funcion para re-fetch manual |

**Query Key:** `QUERY_KEYS.crowdpromotion.promotor.me`

**Configuracion especial:**
- `retry: false` - No reintentar en 404 (usuario sin perfil de promotor es un estado valido, no un error transitorio)
- `staleTime: 30_000` - Datos frescos por 30 segundos para evitar re-fetch innecesario al navegar entre tabs de la pagina

**Comportamiento en error:**
- `404` con `errorCode: "2015"` → `data` sera `undefined`; los componentes consumidores muestran `PromotorNoEncontrado`
- `401` → el interceptor de `apiClient` redirige a `/login` automaticamente

---

### 4.2 `useUpdatePromotor`

**Archivo:** `src/admin/src/hooks/use-promotor-mutations.ts`

**Tipo:** Mutation Hook

**Parametros de mutacion:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `UpdatePromotorRequest` | Datos del formulario ya convertidos via mapper |

**Acciones:**
- `mutate(data)` / `mutateAsync(data)` - Ejecuta PUT `/api/crowdpromotion/promotor/me`
- `onSuccess` - Invalida `QUERY_KEYS.crowdpromotion.promotor.me`; muestra `toast.success("Perfil actualizado correctamente")`
- `onError` - Muestra `toast.error` con mensaje del backend usando `getPromotorErrorMessage`

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `function` | Ejecutar mutacion (fire and forget) |
| `mutateAsync` | `function` | Ejecutar mutacion con await |
| `isPending` | `boolean` | True durante la peticion |
| `isError` | `boolean` | True si la mutacion fallo |

---

### 4.3 `useDesactivarPromotor`

**Archivo:** `src/admin/src/hooks/use-promotor-mutations.ts` (mismo archivo que `useUpdatePromotor`)

**Tipo:** Mutation Hook

**Parametros de mutacion:** Ninguno (PATCH sin body)

**Acciones:**
- `mutate()` - Ejecuta PATCH `/api/crowdpromotion/promotor/me/desactivar`
- `onSuccess` - Invalida `QUERY_KEYS.crowdpromotion.promotor.me`; muestra `toast.success("Perfil de promotor desactivado")`; llama a `onOpenChange(false)` en el dialog para cerrarlo
- `onError` - Muestra `toast.error` con mensaje del backend usando `getPromotorErrorMessage`

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `mutate` | `function` | Ejecutar mutacion |
| `mutateAsync` | `function` | Ejecutar mutacion con await |
| `isPending` | `boolean` | True durante la peticion |

---

## 5. Services

### 5.1 `promotorService`

**Archivo:** `src/admin/src/services/promotor.service.ts`

**Patron:** Clase con metodos async (igual que `ArtistaService` y `CampaniaService`)

**Metodos:**
| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getMe` | - | `Promotor \| null` | `/api/crowdpromotion/promotor/me` | GET |
| `update` | `data: UpdatePromotorRequest` | `PromotorUpdatedResult` | `/api/crowdpromotion/promotor/me` | PUT |
| `desactivar` | - | `PromotorDesactivadoResult` | `/api/crowdpromotion/promotor/me/desactivar` | PATCH |

**Detalle de implementacion por metodo:**

`getMe`:
- Usa `apiFetch<ServiceResponse<Promotor>>(API_ROUTES.crowdpromotion.promotor.me)`
- Si la respuesta es exitosa, retorna `response.data`
- Si lanza error con status 404, retorna `null` (estado valido: usuario sin perfil)
- Cualquier otro error se propaga (el interceptor de axios maneja 401)

`update`:
- Usa `apiFetch<ServiceResponse<PromotorUpdatedResult>>(API_ROUTES.crowdpromotion.promotor.me, { method: "PUT", data })`
- Si `!response.isSuccess`, lanza `new Error(getPromotorErrorMessage(response.messages[0]?.errorCode))`
- Si exitoso, retorna `response.data`

`desactivar`:
- Usa `apiFetch<ServiceResponse<PromotorDesactivadoResult>>(API_ROUTES.crowdpromotion.promotor.desactivar, { method: "PATCH" })`
- Mismo patron de error que `update`
- Si exitoso, retorna `response.data`

**Imports requeridos:**
```
apiFetch              de "@/lib/api-client"
API_ROUTES            de "@shared/constants"
Promotor              de "@shared/types"
UpdatePromotorRequest de "@shared/types"
PromotorUpdatedResult de "@shared/types"
PromotorDesactivadoResult de "@shared/types"
ServiceResponse       de "@shared/types"
getPromotorErrorMessage de "@shared/utils"
```

---

## 6. Flujo de Datos

```
Usuario navega a /promotor
        |
        v
page.tsx (Server Component)
  - Define metadata
  - Renderiza <PromotorPageClient />
        |
        v
PromotorPageClient (Client Component)
  - Llama usePromotor()
        |
        v
usePromotor (TanStack Query)
  - queryKey: QUERY_KEYS.crowdpromotion.promotor.me
  - retry: false, staleTime: 30s
        |
        v
promotorService.getMe()
  - apiFetch -> apiClient (axios + interceptors JWT)
        |
        v
Backend GET /api/crowdpromotion/promotor/me
        |
        v (response)
promotorService.getMe() -> Promotor | null
        |
        v
usePromotor -> { data, isLoading, error }
        |
        v
PromotorPageClient
  - data=null  -> <PromotorNoEncontrado />
  - isLoading  -> <Skeleton />
  - data!=null -> <PromotorKpiCards /> + <Tabs>
                    Tab "Perfil"  -> <PromotorProfileCard promotor={data} />
                    Tab "Editar" -> <PromotorEditForm promotor={data} />

Flujo de edicion:
PromotorEditForm
  - useForm inicializado con mapPromotorToUpdateForm(promotor)
  - handleSubmit -> mapUpdateFormToRequest(formData)
  - useUpdatePromotor.mutate(request)
        |
        v
promotorService.update(data)
  - apiFetch PUT /api/crowdpromotion/promotor/me
        |
        v (onSuccess)
  - queryClient.invalidateQueries(QUERY_KEYS.crowdpromotion.promotor.me)
  - toast.success("Perfil actualizado correctamente")
  - props.onSuccess() -> PromotorPageClient vuelve a tab "Perfil"

Flujo de desactivacion:
PromotorProfileCard -> onDesactivarClick()
  -> PromotorPageClient: setShowDesactivarDialog(true)
  -> <DesactivarPromotorDialog open={true} totalProgramasActivos={data.totalProgramasActivos} />
  -> Usuario confirma
  -> useDesactivarPromotor.mutate()
        |
        v
promotorService.desactivar()
  - apiFetch PATCH /api/crowdpromotion/promotor/me/desactivar
        |
        v (onSuccess)
  - queryClient.invalidateQueries(QUERY_KEYS.crowdpromotion.promotor.me)
  - toast.success("Perfil de promotor desactivado")
  - dialog se cierra
  - UI re-renderiza con promotor.esActivo=false
```

---

## 7. Integracion con Dashboard Principal

El banner `PromotorBanner` se integra en el dashboard existente (`dashboard/page.tsx`) junto a `CompleteProfileBanner`.

**Orden de banners en dashboard:**
1. `CompleteProfileBanner` (artista) - ya existe
2. `PromotorBanner` (promotor) - nuevo

**Criterio de visibilidad de `PromotorBanner`:**
- Solo aparece si `usePromotor().data?.esActivo === true`
- No aparece si el usuario no tiene perfil (no invitar al registro desde admin)
- Se puede descartar (localStorage key: `"promotor-banner-dismissed"`)

**Navegacion del sidebar:**
Se debe agregar un item al sidebar bajo la seccion "Crowdpromotion" con:
- Label: "Mi perfil promotor"
- Icono: `Megaphone`
- Href: `/promotor`
- Solo visible si el usuario tiene perfil de promotor (`usePromotor().data !== undefined`)

**Nota sobre el sidebar:** El sidebar actual es stateful y su estructura no esta detallada en este plan. El item de navegacion debe agregarse al componente `Sidebar` existente en `@/components/layout/sidebar` siguiendo el patron de los items existentes.

---

## 8. Dependencias de Shared

**Importar de `@shared/types`:**
- `Promotor`
- `UpdatePromotorRequest`
- `PromotorUpdatedResult`
- `PromotorDesactivadoResult`
- `ServiceResponse`

**Importar de `@shared/schemas`:**
- `updatePromotorSchema`
- `UpdatePromotorFormData`

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdpromotion.promotor.me`
- `API_ROUTES.crowdpromotion.promotor.me`
- `API_ROUTES.crowdpromotion.promotor.desactivar`
- `TIPO_PROMOTOR_LABELS`

**Importar de `@shared/utils`:**
- `mapPromotorToUpdateForm` (formulario de edicion: Promotor → UpdatePromotorFormData)
- `mapUpdateFormToRequest` (envio: UpdatePromotorFormData → UpdatePromotorRequest)
- `getPromotorEstado` (para `PromotorStatusBadge`)
- `getPromotorErrorMessage` (para manejo de errores en hooks y service)
- `formatComisionesGanadas` (para `PromotorKpiCards`)

---

## 9. Consideraciones Next.js 14

### Client vs Server Components

| Archivo | Tipo | Razon |
|---------|------|-------|
| `promotor/page.tsx` | Server Component | Solo metadata; sin estado ni interactividad |
| `PromotorPageClient.tsx` | Client Component (`"use client"`) | useState, hooks de TanStack Query |
| `PromotorKpiCards.tsx` | Client Component (`"use client"`) | Puede recibir data de su padre client; no necesita directiva propia si el padre ya es client. Marcar como client si se usa directamente en un server component |
| `PromotorProfileCard.tsx` | Client Component | Callbacks de evento (`onEditClick`, `onDesactivarClick`) |
| `PromotorEditForm.tsx` | Client Component | `useForm`, submit handlers, `useUpdatePromotor` |
| `PromotorSocialLinks.tsx` | Puede ser sin directiva | Solo renderiza datos estaticos recibidos por props |
| `PromotorStatusBadge.tsx` | Puede ser sin directiva | Solo renderiza datos estaticos recibidos por props |
| `DesactivarPromotorDialog.tsx` | Client Component | `useDesactivarPromotor` mutation |
| `PromotorNoEncontrado.tsx` | Puede ser sin directiva | Solo UI estatica con link externo |
| `PromotorBanner.tsx` | Client Component | `useState`, `useEffect`, `usePromotor` |

**Regla practica del proyecto:** El layout `(dashboard)/layout.tsx` ya es `"use client"`. Los componentes hijos heredan el contexto de cliente. Solo los componentes que necesitan hooks o estado explicitamente requieren la directiva.

### Loading States

Seguir el patron existente del proyecto (ver `campanias/page.tsx`):
- `PromotorPageClient` muestra un skeleton layout durante `isLoading`:
  - 2 `Skeleton` de KPI cards (`h-32`)
  - 1 `Skeleton` de card principal (`h-64`)
- NO usar `loading.tsx` de Next.js (el proyecto no lo usa en otras features)

### Error Boundaries

- No se implementan `error.tsx` custom (el proyecto no los usa actualmente)
- Los errores del service se capturan en los hooks via TanStack Query
- El 404 se maneja como estado valido (no es error): `usePromotor` retorna `data=undefined` si el servicio retorna `null`

### Metadata

```typescript
// promotor/page.tsx
export const metadata = {
    title: "Mi Perfil de Promotor | WePlay Rises",
    description: "Gestiona tu perfil de promotor en WePlay Rises",
}
```

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/promotor/page.tsx` | Server Component | Entry point de la ruta con metadata |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorPageClient.tsx` | Client Component | Orquestador de la pagina con estado |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorKpiCards.tsx` | Client Component | KPI cards de programas y comisiones |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorProfileCard.tsx` | Client Component | Vista readonly del perfil |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorEditForm.tsx` | Client Component | Formulario RHF + Zod de edicion |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorSocialLinks.tsx` | Component | Lista de redes sociales readonly |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorStatusBadge.tsx` | Component | Badge activo/inactivo |
| `src/admin/src/app/(dashboard)/promotor/components/DesactivarPromotorDialog.tsx` | Client Component | AlertDialog de confirmacion con logica de programas afectados |
| `src/admin/src/app/(dashboard)/promotor/components/PromotorNoEncontrado.tsx` | Component | Estado vacio si usuario no es promotor |
| `src/admin/src/hooks/use-promotor.ts` | Hook | Query hook GET perfil del promotor |
| `src/admin/src/hooks/use-promotor-mutations.ts` | Hook | Mutation hooks update + desactivar |
| `src/admin/src/services/promotor.service.ts` | Service | API calls del modulo promotor |
| `src/admin/src/components/dashboard/PromotorBanner.tsx` | Client Component | Banner en dashboard principal |

## 11. Archivos a Modificar

| Archivo | Modificacion |
|---------|-------------|
| `src/admin/src/hooks/index.ts` | Agregar `export * from "./use-promotor"` y `export * from "./use-promotor-mutations"` |
| `src/admin/src/services/index.ts` | Agregar `export { promotorService } from "./promotor.service"` |
| `src/admin/src/app/(dashboard)/dashboard/page.tsx` | Agregar `<PromotorBanner />` debajo de `<CompleteProfileBanner />` |
| `src/admin/src/components/layout/sidebar.tsx` | Agregar item de navegacion "Mi perfil promotor" con icono `Megaphone` hacia `/promotor` |

---

## 12. Checklist

### Arquitectura
- [ ] Ruta `/promotor` creada en App Router `(dashboard)` group
- [ ] `page.tsx` es Server Component con metadata
- [ ] Cliente separado en `PromotorPageClient.tsx`
- [ ] Patron de carpeta `components/` local a la ruta (igual que campanias)

### Componentes
- [ ] Todos los componentes tipados con interfaces de props explicitas
- [ ] Ningun componente usa `any`
- [ ] `PromotorStatusBadge` sigue patron de `CampaniaStatusBadge`
- [ ] `DesactivarPromotorDialog` sigue patron de `DeleteConfirmDialog`
- [ ] `PromotorBanner` sigue patron de `CompleteProfileBanner`
- [ ] Tipo de promotor renderizado como readonly en `PromotorProfileCard` (AC-CP01-9)
- [ ] Tipo de promotor renderizado como `<Input disabled />` en `PromotorEditForm` (AC-CP01-9)
- [ ] `PromotorNoEncontrado` no invita al registro (admin no tiene esa ruta)
- [ ] Todos los componentes usan shadcn/ui (no HTML nativo para UI)

### Hooks
- [ ] `usePromotor` usa `retry: false` para no reintentar en 404
- [ ] `usePromotor` usa `staleTime: 30_000`
- [ ] `useUpdatePromotor` invalida `QUERY_KEYS.crowdpromotion.promotor.me` en `onSuccess`
- [ ] `useDesactivarPromotor` invalida `QUERY_KEYS.crowdpromotion.promotor.me` en `onSuccess`
- [ ] Los hooks muestran `toast.success` y `toast.error` (sonner, patron del proyecto)
- [ ] Los query keys usan `QUERY_KEYS.crowdpromotion.promotor.me` de `@shared/constants`

### Services
- [ ] `promotorService.getMe()` retorna `null` en 404 (no lanza error)
- [ ] `promotorService.update()` y `promotorService.desactivar()` lanzan error con mensaje del backend si `!isSuccess`
- [ ] El service usa `apiFetch` de `@/lib/api-client` (patron del proyecto)
- [ ] El service usa `API_ROUTES.crowdpromotion.*` de `@shared/constants`

### Formulario
- [ ] `PromotorEditForm` usa `updatePromotorSchema` de `@shared/schemas`
- [ ] `defaultValues` inicializados con `mapPromotorToUpdateForm(promotor)` de `@shared/utils`
- [ ] Payload al backend convertido con `mapUpdateFormToRequest(formData)` de `@shared/utils`
- [ ] Strings vacios (`''`) de URLs convertidos a `undefined` antes del envio (via mapper)
- [ ] Boton de submit deshabilitado durante `isPending`
- [ ] Errores de campo mostrados con `errors.{campo}.message`

### Tipos
- [ ] Types importados de `@shared/types` (no duplicar en admin)
- [ ] Schemas importados de `@shared/schemas` (no duplicar en admin)
- [ ] Constantes importadas de `@shared/constants` (no duplicar en admin)

### Dashboard
- [ ] `PromotorBanner` agregado a `dashboard/page.tsx`
- [ ] Banner solo visible si `data?.esActivo === true`
- [ ] Banner descartable con persistencia en localStorage
- [ ] Item en sidebar hacia `/promotor`

---

## 13. Notas de Implementacion

### Patron de formulario de edicion

El formulario de edicion del promotor es similar al `ArtistaForm` existente pero con un campo adicional de solo lectura (tipo de promotor). El patron es:

```
useForm con defaultValues de mapper
  -> handleSubmit
    -> mapUpdateFormToRequest (convierte '' a undefined)
    -> useUpdatePromotor.mutate()
      -> onSuccess: invalidate query + toast + onSuccess()
      -> onError: toast con mensaje del backend
```

### Manejo del campo `tipoPromotorId` readonly

El tipo de promotor no se incluye en `UpdatePromotorRequest` (el backend lo ignora si se enviara). En el formulario se muestra como un `<Input>` con `disabled` y `value={TIPO_PROMOTOR_LABELS[promotor.tipoPromotorId]}`. NO se registra con `register()` de RHF porque no forma parte del schema de actualizacion.

### Re-render tras desactivacion

Cuando `useDesactivarPromotor` completa con exito, invalida la query de `usePromotor`. TanStack Query re-fetchea automaticamente y el componente se actualiza:
- `PromotorStatusBadge` muestra "Inactivo"
- El boton "Desactivar cuenta" en `PromotorProfileCard` se oculta (condicion `esActivo`)
- `PromotorBanner` en dashboard desaparece

### Ausencia de registro en Admin

El Admin deliberadamente NO tiene flujo de registro de promotor. Si el usuario llega a `/promotor` sin perfil, `PromotorNoEncontrado` le explica que debe ir a la Landing para registrarse. El link apunta a `${process.env.NEXT_PUBLIC_LANDING_URL}/promotor/registro`.

### Variable de entorno requerida

Agregar a `.env.local` del admin:
```
NEXT_PUBLIC_LANDING_URL=http://localhost:3000
```

Para produccion, esta variable apuntara al dominio de la landing publica.
