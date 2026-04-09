# Plan Frontend: Templates y Guia para Artistas Noveles (Admin Dashboard)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** src/admin (Next.js 14 App Router)
**Prioridad:** LOW (Mostly read-only from seed, CRUD for future use)

---

## 1. Resumen

- **Screens:** 3 (Lista, Crear/Editar, Detalle)
- **Componentes:** 8 componentes principales + 6 componentes compartidos
- **Hooks:** 8 custom hooks (queries + mutations)
- **Services:** 3 services (templates, necesidades, maestras)
- **Forms:** 2 formularios con React Hook Form + Zod

Este plan define la arquitectura frontend para la gestion de templates de crowdsourcing en el dashboard admin. Sigue el patron de Next.js 14 App Router y las convenciones establecidas en el proyecto.

---

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── crowdsourcing/
│           └── templates/
│               ├── page.tsx                      # Lista de templates
│               ├── nuevo/
│               │   └── page.tsx                  # Crear template
│               ├── [id]/
│               │   ├── page.tsx                  # Detalle template
│               │   └── editar/
│               │       └── page.tsx              # Editar template
│               └── components/
│                   ├── list/
│                   │   ├── TemplateTable.tsx     # Tabla principal
│                   │   ├── TemplateRow.tsx       # Fila de tabla
│                   │   └── TemplateFilters.tsx   # Search + status filter
│                   ├── detail/
│                   │   ├── TemplateHeader.tsx    # Header con nombre e info
│                   │   ├── NecesidadesSection.tsx # Lista necesidades por fase
│                   │   └── ResumenCard.tsx       # Resumen financiero
│                   ├── form/
│                   │   ├── TemplateForm.tsx      # Form principal
│                   │   ├── GeneralDataSection.tsx # Nombre, icono, orden
│                   │   ├── NecesidadesSection.tsx # Lista dinamica de necesidades
│                   │   └── NecesidadFormItem.tsx  # Form para 1 necesidad
│                   └── shared/
│                       ├── PriorityBadge.tsx     # Badge prioridad (Alta/Media/Baja)
│                       ├── IconPreview.tsx       # Preview de icono seleccionado
│                       ├── EmptyState.tsx        # Estado vacio (reutilizado)
│                       └── DeleteConfirmDialog.tsx # Dialogo confirmacion
│
├── hooks/
│   ├── use-templates.ts                          # Queries de templates
│   ├── use-templates-mutations.ts                # Mutations CRUD
│   ├── use-roles-profesionales.ts                # Query maestras roles
│   └── use-categorias-rol.ts                     # Query maestras categorias
│
├── services/
│   ├── template.service.ts                       # API calls templates
│   ├── necesidad.service.ts                      # API calls necesidades template
│   └── maestras.service.ts                       # API calls maestras (roles, categorias)
│
└── lib/
    └── validations/
        └── template.schema.ts                    # Schemas Zod para forms (si no se reutilizan de shared)
```

---

## 3. Componentes

### 3.1 Pages (App Router)

#### 3.1.1 `/dashboard/crowdsourcing/templates/page.tsx` (Lista)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/page.tsx`

**Responsabilidad:** Pagina principal de listado de templates con filtros y acciones CRUD.

**Estado Local:**
- `searchQuery` (string) - Termino de busqueda
- `statusFilter` ("Todos" | "Activos" | "Inactivos") - Filtro de estado

**Dependencias:**
- Hooks: `useTemplates`, `useDeleteTemplate`, `useToggleTemplateStatus`
- Componentes: `TemplateTable`, `TemplateFilters`, `EmptyState`, `Button`, `Card`
- shadcn/ui: `Skeleton`, `Alert`

**Props:** Ninguna (page component)

**Data Flow:**
```
useTemplates() -> TemplateTable -> TemplateRow
                                      ├─> Edit (router.push)
                                      ├─> View (router.push)
                                      └─> Toggle Status (mutation)
```

---

#### 3.1.2 `/dashboard/crowdsourcing/templates/nuevo/page.tsx` (Crear)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/nuevo/page.tsx`

**Responsabilidad:** Pagina de creacion de template con form completo.

**Estado Local:** Manejado por `TemplateForm` component

**Dependencias:**
- Hooks: `useCreateTemplate`, `useRolesProfesionales`
- Componentes: `TemplateForm`, `Card`

**Props:** Ninguna (page component)

---

#### 3.1.3 `/dashboard/crowdsourcing/templates/[id]/page.tsx` (Detalle)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/[id]/page.tsx`

**Responsabilidad:** Pagina de detalle read-only de un template.

**Dependencias:**
- Hooks: `useTemplate(id)`
- Componentes: `TemplateHeader`, `NecesidadesSection`, `ResumenCard`, `Skeleton`

**Props:**
| Prop | Tipo | Descripcion |
|------|------|-------------|
| params | { id: string } | Route params (Next.js) |

---

#### 3.1.4 `/dashboard/crowdsourcing/templates/[id]/editar/page.tsx` (Editar)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/[id]/editar/page.tsx`

**Responsabilidad:** Pagina de edicion de template existente.

**Dependencias:**
- Hooks: `useTemplate(id)`, `useUpdateTemplate`, `useRolesProfesionales`
- Componentes: `TemplateForm`, `Card`, `Skeleton`

**Props:**
| Prop | Tipo | Descripcion |
|------|------|-------------|
| params | { id: string } | Route params (Next.js) |

---

### 3.2 Componentes de Lista

#### 3.2.1 TemplateTable

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/list/TemplateTable.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| templates | PlantillaProyectoList[] | Si | Lista de templates |
| onEdit | (id: string) => void | Si | Callback editar |
| onView | (id: string) => void | Si | Callback ver detalle |
| onToggleStatus | (id: string) => void | Si | Callback activar/desactivar |
| isLoading | boolean | No | Estado de carga (mutations) |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableCell`
- Componentes: `TemplateRow`, `PriorityBadge`, `Skeleton`

**Responsabilidad:** Tabla principal con headers y renderizado de filas.

---

#### 3.2.2 TemplateRow

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/list/TemplateRow.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| template | PlantillaProyectoList | Si | Datos del template |
| onEdit | (id: string) => void | Si | Callback editar |
| onView | (id: string) => void | Si | Callback ver detalle |
| onToggleStatus | (id: string) => void | Si | Callback activar/desactivar |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `TableRow`, `TableCell`, `Button`, `Badge`, `DropdownMenu`
- Iconos: `MoreVertical`, `Edit`, `Eye`, `ToggleLeft`, `ToggleRight`

**Responsabilidad:** Renderiza una fila de la tabla con acciones dropdown.

---

#### 3.2.3 TemplateFilters

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/list/TemplateFilters.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| searchQuery | string | Si | Valor actual de busqueda |
| onSearchChange | (value: string) => void | Si | Callback cambio busqueda |
| statusFilter | "Todos" \| "Activos" \| "Inactivos" | Si | Filtro de estado |
| onStatusChange | (value: string) => void | Si | Callback cambio status |

**Estado Local:** Ninguno (controlled component)

**Dependencias:**
- shadcn/ui: `Card`, `Input`, `Select`, `SelectTrigger`, `SelectContent`, `SelectItem`
- Iconos: `Search`

**Responsabilidad:** Barra de filtros con search input y selector de estado.

---

### 3.3 Componentes de Detalle

#### 3.3.1 TemplateHeader

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/detail/TemplateHeader.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| template | PlantillaProyecto | Si | Datos completos del template |
| onEdit | () => void | No | Callback para ir a edicion |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Card`, `CardHeader`, `CardTitle`, `Button`, `Badge`
- Componentes: `IconPreview`
- Iconos: `Edit`

**Responsabilidad:** Header con nombre, descripcion, icono y estado del template.

---

#### 3.3.2 NecesidadesSection (Detalle)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/detail/NecesidadesSection.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| necesidades | PlantillaProyectoNecesidad[] | Si | Lista de necesidades del template |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Card`, `Accordion`, `AccordionItem`, `AccordionTrigger`, `AccordionContent`
- Componentes: `PriorityBadge`
- Utils: `formatCurrency`

**Responsabilidad:** Lista de necesidades agrupadas por fase en accordions.

---

#### 3.3.3 ResumenCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/detail/ResumenCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| resumen | PlantillaResumen | Si | Resumen financiero y de prioridades |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `Card`, `CardHeader`, `CardTitle`, `CardContent`
- Utils: `formatCurrency`

**Responsabilidad:** Card con resumen de precios totales y contadores de prioridades.

---

### 3.4 Componentes de Formulario

#### 3.4.1 TemplateForm

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/form/TemplateForm.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| mode | "create" \| "edit" | Si | Modo de operacion |
| initialData | PlantillaProyecto \| undefined | No | Datos iniciales (edit mode) |
| onSubmit | (data: CreateTemplateRequest) => Promise<void> | Si | Callback submit |
| onCancel | () => void | Si | Callback cancelar |

**Estado Local:**
- Form state (React Hook Form)
- `necesidades` (array de necesidades en form)

**Dependencias:**
- React Hook Form: `useForm`, `FormProvider`
- Zod: `zodResolver`
- shadcn/ui: `Form`, `Button`, `Card`
- Componentes: `GeneralDataSection`, `NecesidadesSection` (form version)

**Responsabilidad:** Form principal que maneja validacion y submit de template completo.

**Validaciones:**
- Nombre: requerido, max 200 caracteres
- Icono: requerido
- Necesidades: minimo 1 necesidad
- Cada necesidad: titulo, rol, prioridad requeridos; precio max >= precio min

---

#### 3.4.2 GeneralDataSection

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/form/GeneralDataSection.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| control | Control<TemplateFormData> | Si | React Hook Form control |

**Estado Local:** Ninguno (controlled by parent form)

**Dependencias:**
- React Hook Form: `Controller`, `useFormContext`
- shadcn/ui: `Card`, `Input`, `Textarea`, `Select`, `Checkbox`, `Label`
- Componentes: `IconPreview`

**Responsabilidad:** Seccion del form para datos generales (nombre, descripcion, icono, orden, activo).

---

#### 3.4.3 NecesidadesSection (Form)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/form/NecesidadesSection.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| control | Control<TemplateFormData> | Si | React Hook Form control |
| rolesProfesionales | RolProfesionalConCategoria[] | Si | Catalogo de roles |

**Estado Local:**
- `necesidades` array (useFieldArray)

**Dependencias:**
- React Hook Form: `useFieldArray`, `useFormContext`
- shadcn/ui: `Card`, `Button`, `Alert`
- Componentes: `NecesidadFormItem`, `DeleteConfirmDialog`
- Iconos: `Plus`, `AlertCircle`

**Responsabilidad:** Lista dinamica de necesidades con botones agregar/eliminar.

---

#### 3.4.4 NecesidadFormItem

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/form/NecesidadFormItem.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| index | number | Si | Indice en el array de necesidades |
| control | Control<TemplateFormData> | Si | React Hook Form control |
| rolesProfesionales | RolProfesionalConCategoria[] | Si | Catalogo de roles |
| onDelete | () => void | Si | Callback eliminar necesidad |

**Estado Local:** Ninguno

**Dependencias:**
- React Hook Form: `Controller`
- shadcn/ui: `Card`, `Input`, `Select`, `Button`
- Componentes: `PriorityBadge`
- Iconos: `Trash2`

**Responsabilidad:** Form para una necesidad individual (fase, titulo, rol, precio, prioridad).

---

### 3.5 Componentes Compartidos

#### 3.5.1 PriorityBadge

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/shared/PriorityBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| prioridad | PrioridadNecesidad | Si | "Alta" \| "Media" \| "Baja" |
| variant | "default" \| "outline" | No | Variant de Badge |

**Responsabilidad:** Badge con color dinamico segun prioridad (rojo/amarillo/azul).

---

#### 3.5.2 IconPreview

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/shared/IconPreview.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| icon | string | Si | Nombre del icono (ej: "music", "video") |
| size | "sm" \| "md" \| "lg" | No | Tamano del icono |

**Responsabilidad:** Preview del icono de Lucide seleccionado.

---

#### 3.5.3 EmptyState

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/shared/EmptyState.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| icon | React.ReactNode | Si | Icono a mostrar |
| title | string | Si | Titulo del estado vacio |
| description | string | Si | Descripcion |
| actionLabel | string | No | Label del boton accion |
| onAction | () => void | No | Callback del boton |

**Responsabilidad:** Estado vacio reutilizable (ya existe en proyecto, reutilizar).

---

#### 3.5.4 DeleteConfirmDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/templates/components/shared/DeleteConfirmDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| open | boolean | Si | Estado open del dialogo |
| onOpenChange | (open: boolean) => void | Si | Callback cambio estado |
| onConfirm | () => void | Si | Callback confirmacion |
| title | string | No | Titulo custom |
| description | string | No | Descripcion custom |

**Responsabilidad:** Dialogo de confirmacion reutilizable para acciones destructivas.

---

## 4. Hooks

### 4.1 use-templates.ts

**Archivo:** `src/admin/src/hooks/use-templates.ts`

**Exports:**

#### 4.1.1 `useTemplates()`

**Tipo:** Query Hook

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | PlantillaProyectoList[] \| undefined | Lista de templates |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Query Key:** `QUERY_KEYS.crowdsourcing.templates.all`

**Query Fn:** `templateService.getAll()`

**Configuracion:**
```typescript
{
    staleTime: 5 * 60 * 1000, // 5 min - datos maestros cambian poco
}
```

---

#### 4.1.2 `useTemplate(id: string)`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del template |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | PlantillaProyecto \| undefined | Datos completos del template |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Query Key:** `QUERY_KEYS.crowdsourcing.templates.byId(id)`

**Query Fn:** `templateService.getById(id)`

**Configuracion:**
```typescript
{
    enabled: !!id,
    staleTime: 5 * 60 * 1000,
}
```

---

### 4.2 use-templates-mutations.ts

**Archivo:** `src/admin/src/hooks/use-templates-mutations.ts`

**Exports:**

#### 4.2.1 `useCreateTemplate()`

**Tipo:** Mutation Hook

**Parametros Mutation:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| data | CreateTemplateRequest | Datos del nuevo template |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| mutate | (data: CreateTemplateRequest) => void | Ejecutar mutacion |
| mutateAsync | (data: CreateTemplateRequest) => Promise<string> | Version async |
| isPending | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Acciones:**
- `onSuccess` - Invalidar `QUERY_KEYS.crowdsourcing.templates.all`, toast success, redirect
- `onError` - Toast error

---

#### 4.2.2 `useUpdateTemplate()`

**Tipo:** Mutation Hook

**Parametros Mutation:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del template |
| data | UpdateTemplateRequest | Datos actualizados |

**Retorna:** Similar a `useCreateTemplate`

**Acciones:**
- `onSuccess` - Invalidar queries (all + byId), toast success, redirect
- `onError` - Toast error

---

#### 4.2.3 `useDeleteTemplate()`

**Tipo:** Mutation Hook

**Parametros Mutation:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del template a eliminar |

**Retorna:** Similar estructura

**Acciones:**
- `onSuccess` - Invalidar `QUERY_KEYS.crowdsourcing.templates.all`, toast success
- `onError` - Toast error

---

#### 4.2.4 `useToggleTemplateStatus()`

**Tipo:** Mutation Hook

**Parametros Mutation:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del template |

**Retorna:** Similar estructura

**Acciones:**
- `onSuccess` - Invalidar queries, toast "Template [activado/desactivado]"
- `onError` - Toast error

---

### 4.3 use-roles-profesionales.ts

**Archivo:** `src/admin/src/hooks/use-roles-profesionales.ts`

**Export:** `useRolesProfesionales()`

**Tipo:** Query Hook

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | RolProfesionalConCategoria[] \| undefined | Catalogo de roles |
| isLoading | boolean | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales`

**Query Fn:** `maestrasService.getRolesProfesionales()`

**Configuracion:**
```typescript
{
    staleTime: 10 * 60 * 1000, // 10 min - datos maestros
}
```

---

### 4.4 use-categorias-rol.ts

**Archivo:** `src/admin/src/hooks/use-categorias-rol.ts`

**Export:** `useCategoriasRol()`

**Tipo:** Query Hook

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | CategoriaRol[] \| undefined | Categorias de roles |
| isLoading | boolean | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.categoriasRol`

**Query Fn:** `maestrasService.getCategoriasRol()`

**Configuracion:**
```typescript
{
    staleTime: 10 * 60 * 1000,
}
```

---

## 5. Services

### 5.1 template.service.ts

**Archivo:** `src/admin/src/services/template.service.ts`

**Clase:** `TemplateService`

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getAll()` | - | Promise<PlantillaProyectoList[]> | `/api/crowdsourcing/templates` | GET |
| `getById(id)` | id: string | Promise<PlantillaProyecto \| null> | `/api/crowdsourcing/templates/${id}` | GET |
| `create(data)` | data: CreateTemplateRequest | Promise<string> | `/api/crowdsourcing/templates` | POST |
| `update(id, data)` | id: string, data: UpdateTemplateRequest | Promise<void> | `/api/crowdsourcing/templates/${id}` | PUT |
| `delete(id)` | id: string | Promise<void> | `/api/crowdsourcing/templates/${id}` | DELETE |
| `toggleStatus(id)` | id: string | Promise<void> | `/api/crowdsourcing/templates/${id}/toggle-status` | PATCH |

**Manejo de Errores:**
- Usa `apiFetch` existente que maneja `ServiceResponse<T>` wrapper
- Retorna `response.data` en success
- Throws error en caso de fallo (catch en mutation hooks)

**Patron de Implementacion:**
```typescript
import { apiFetch } from "@/lib/api-client";
import type {
    PlantillaProyectoList,
    PlantillaProyecto,
    ServiceResponse
} from "@shared/types";

class TemplateService {
    private readonly baseUrl = "/api/crowdsourcing/templates";

    async getAll(): Promise<PlantillaProyectoList[]> {
        const response = await apiFetch<ServiceResponse<PlantillaProyectoList[]>>(
            this.baseUrl
        );
        return response.data;
    }

    async getById(id: string): Promise<PlantillaProyecto | null> {
        try {
            const response = await apiFetch<ServiceResponse<PlantillaProyecto>>(
                `${this.baseUrl}/${id}`
            );
            return response.data;
        } catch {
            return null;
        }
    }

    // ... resto de metodos
}

export const templateService = new TemplateService();
```

---

### 5.2 necesidad.service.ts

**Archivo:** `src/admin/src/services/necesidad.service.ts`

**Clase:** `NecesidadService`

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getByTemplate(templateId)` | templateId: string | Promise<PlantillaProyectoNecesidad[]> | `/api/crowdsourcing/templates/${id}` (necesidades array) | GET |

**Responsabilidad:** Obtener necesidades de un template (usado en detalle).

---

### 5.3 maestras.service.ts

**Archivo:** `src/admin/src/services/maestras.service.ts`

**Clase:** `MaestrasService`

**Metodos:**

| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getRolesProfesionales()` | - | Promise<RolProfesionalConCategoria[]> | `/api/crowdsourcing/maestras/roles-profesionales` | GET |
| `getCategoriasRol()` | - | Promise<CategoriaRol[]> | `/api/crowdsourcing/maestras/categorias-rol` | GET |

**Responsabilidad:** Obtener datos maestros (roles y categorias) para formularios.

---

## 6. Flujo de Datos

### 6.1 Flujo de Lectura (Lista)

```
Page (templates/page.tsx)
    ↓
useTemplates() hook
    ↓
templateService.getAll()
    ↓
apiFetch<ServiceResponse<PlantillaProyectoList[]>>(GET /api/crowdsourcing/templates)
    ↓
Backend API
    ↓
Response: { data: PlantillaProyectoList[], messages: [...] }
    ↓
React Query cache (staleTime: 5min)
    ↓
TemplateTable component
    ↓
TemplateRow[] components (map)
```

---

### 6.2 Flujo de Escritura (Crear)

```
User Action (click "Guardar template")
    ↓
TemplateForm.onSubmit(data)
    ↓
useCreateTemplate().mutate(data)
    ↓
templateService.create(data)
    ↓
apiFetch<ServiceResponse<string>>(POST /api/crowdsourcing/templates, body: data)
    ↓
Backend API (Command Handler)
    ↓
Response: { data: "guid-nuevo-template", messages: [...] }
    ↓
Mutation onSuccess:
    ├─> invalidateQueries(templates.all)
    ├─> toast.success("Template creado")
    └─> router.push("/dashboard/crowdsourcing/templates")
```

---

### 6.3 Flujo de Edicion

```
Page (templates/[id]/editar/page.tsx)
    ↓
useTemplate(id) - fetch inicial
    ↓
TemplateForm (mode="edit", initialData={template})
    ↓
User modifica form
    ↓
TemplateForm.onSubmit(data)
    ↓
useUpdateTemplate().mutate({ id, data })
    ↓
templateService.update(id, data)
    ↓
apiFetch<ServiceResponse<void>>(PUT /api/crowdsourcing/templates/{id}, body: data)
    ↓
Backend API
    ↓
Mutation onSuccess:
    ├─> invalidateQueries(templates.byId(id))
    ├─> invalidateQueries(templates.all)
    ├─> toast.success("Template actualizado")
    └─> router.push("/dashboard/crowdsourcing/templates")
```

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

### 7.1 Types
```typescript
import type {
    PlantillaProyectoList,
    PlantillaProyecto,
    PlantillaProyectoNecesidad,
    PlantillaResumen,
    RolProfesional,
    RolProfesionalConCategoria,
    CategoriaRol,
    PrioridadNecesidad,
    ServiceResponse,
} from "@shared/types";
```

### 7.2 Constants
```typescript
import {
    QUERY_KEYS,
    API_ROUTES,
    APP_ROUTES,
    PRIORIDAD_NECESIDAD,
    PRIORIDAD_NECESIDAD_LABELS,
    PRIORIDAD_NECESIDAD_COLORS,
    MODALIDAD_COBRO,
    FASES_PROYECTO,
} from "@shared/constants";
```

### 7.3 Schemas (Opcional)
```typescript
// Si se necesita validacion adicional no cubierta por shared
import {
    generarNecesidadesSchema, // NO se usa en admin
} from "@shared/schemas";
```

**Nota:** Para CRUD de templates (admin), crear schemas locales si es necesario:
- `src/admin/src/lib/validations/template.schema.ts` con `createTemplateSchema` y `updateTemplateSchema`

---

## 8. Validacion de Formularios

### 8.1 CreateTemplateSchema (Zod)

**Archivo:** `src/admin/src/lib/validations/template.schema.ts`

```typescript
import { z } from "zod";

export const necesidadTemplateSchema = z.object({
    fase: z.string().min(1, "La fase es obligatoria"),
    titulo: z.string().min(1, "El titulo es obligatorio").max(200),
    descripcion: z.string().optional(),
    rolProfesionalId: z.number().positive("Selecciona un rol profesional"),
    precioMinOrientativo: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
    precioMaxOrientativo: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
    prioridad: z.enum(["Alta", "Media", "Baja"], {
        errorMap: () => ({ message: "Selecciona una prioridad" })
    }),
    orden: z.number().int().nonnegative(),
}).refine(
    (data) => {
        if (data.precioMinOrientativo !== undefined && data.precioMaxOrientativo !== undefined) {
            return data.precioMaxOrientativo >= data.precioMinOrientativo;
        }
        return true;
    },
    {
        message: "El precio maximo debe ser mayor o igual al minimo",
        path: ["precioMaxOrientativo"],
    }
);

export const createTemplateSchema = z.object({
    nombre: z.string().min(1, "El nombre es obligatorio").max(200),
    descripcion: z.string().optional(),
    icono: z.string().min(1, "Selecciona un icono"),
    orden: z.number().int().nonnegative("El orden debe ser mayor o igual a 0"),
    activo: z.boolean().default(true),
    necesidades: z.array(necesidadTemplateSchema).min(1, "Debe agregar al menos una necesidad"),
});

export type CreateTemplateFormData = z.infer<typeof createTemplateSchema>;
```

### 8.2 Integracion con React Hook Form

```typescript
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { createTemplateSchema, type CreateTemplateFormData } from "@/lib/validations/template.schema";

export const TemplateForm = ({ mode, initialData, onSubmit, onCancel }) => {
    const form = useForm<CreateTemplateFormData>({
        resolver: zodResolver(createTemplateSchema),
        defaultValues: initialData || {
            nombre: "",
            descripcion: "",
            icono: "",
            orden: 0,
            activo: true,
            necesidades: [],
        },
    });

    const handleSubmit = form.handleSubmit(async (data) => {
        await onSubmit(data);
    });

    // ...
};
```

---

## 9. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `app/(dashboard)/crowdsourcing/templates/page.tsx` | Page | Lista de templates |
| `app/(dashboard)/crowdsourcing/templates/nuevo/page.tsx` | Page | Crear template |
| `app/(dashboard)/crowdsourcing/templates/[id]/page.tsx` | Page | Detalle template |
| `app/(dashboard)/crowdsourcing/templates/[id]/editar/page.tsx` | Page | Editar template |
| `app/(dashboard)/crowdsourcing/templates/components/list/TemplateTable.tsx` | Component | Tabla de templates |
| `app/(dashboard)/crowdsourcing/templates/components/list/TemplateRow.tsx` | Component | Fila de tabla |
| `app/(dashboard)/crowdsourcing/templates/components/list/TemplateFilters.tsx` | Component | Filtros (search + status) |
| `app/(dashboard)/crowdsourcing/templates/components/detail/TemplateHeader.tsx` | Component | Header de detalle |
| `app/(dashboard)/crowdsourcing/templates/components/detail/NecesidadesSection.tsx` | Component | Lista necesidades (detalle) |
| `app/(dashboard)/crowdsourcing/templates/components/detail/ResumenCard.tsx` | Component | Card resumen financiero |
| `app/(dashboard)/crowdsourcing/templates/components/form/TemplateForm.tsx` | Component | Form principal |
| `app/(dashboard)/crowdsourcing/templates/components/form/GeneralDataSection.tsx` | Component | Seccion datos generales |
| `app/(dashboard)/crowdsourcing/templates/components/form/NecesidadesSection.tsx` | Component | Seccion necesidades (form) |
| `app/(dashboard)/crowdsourcing/templates/components/form/NecesidadFormItem.tsx` | Component | Item de necesidad (form) |
| `app/(dashboard)/crowdsourcing/templates/components/shared/PriorityBadge.tsx` | Component | Badge prioridad |
| `app/(dashboard)/crowdsourcing/templates/components/shared/IconPreview.tsx` | Component | Preview icono |
| `app/(dashboard)/crowdsourcing/templates/components/shared/DeleteConfirmDialog.tsx` | Component | Dialogo confirmacion |
| `hooks/use-templates.ts` | Hook | Queries de templates |
| `hooks/use-templates-mutations.ts` | Hook | Mutations CRUD |
| `hooks/use-roles-profesionales.ts` | Hook | Query roles |
| `hooks/use-categorias-rol.ts` | Hook | Query categorias |
| `services/template.service.ts` | Service | API calls templates |
| `services/necesidad.service.ts` | Service | API calls necesidades |
| `services/maestras.service.ts` | Service | API calls maestras |
| `lib/validations/template.schema.ts` | Schema | Validaciones Zod |

**Total:** 24 archivos nuevos

---

## 10. Next.js App Router - Patterns

### 10.1 Loading States

**Archivo:** `app/(dashboard)/crowdsourcing/templates/loading.tsx`

```typescript
import { Skeleton, Card, CardContent } from "@/components/ui";

export default function TemplatesLoading() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <Skeleton className="h-8 w-48" />
                    <Skeleton className="h-4 w-64 mt-2" />
                </div>
                <Skeleton className="h-10 w-40" />
            </div>
            <Card>
                <CardContent className="p-6">
                    <Skeleton className="h-64 w-full" />
                </CardContent>
            </Card>
        </div>
    );
}
```

---

### 10.2 Error Boundaries

**Archivo:** `app/(dashboard)/crowdsourcing/templates/error.tsx`

```typescript
"use client";

import { useEffect } from "react";
import { Button, Alert, AlertDescription } from "@/components/ui";
import { AlertCircle } from "lucide-react";

export default function TemplatesError({
    error,
    reset,
}: {
    error: Error & { digest?: string };
    reset: () => void;
}) {
    useEffect(() => {
        console.error("Templates error:", error);
    }, [error]);

    return (
        <div className="space-y-6">
            <Alert variant="destructive">
                <AlertCircle className="h-4 w-4" />
                <AlertDescription>
                    Error al cargar templates: {error.message}
                </AlertDescription>
            </Alert>
            <Button onClick={reset}>Reintentar</Button>
        </div>
    );
}
```

---

### 10.3 Metadata (SEO)

**En cada page.tsx:**

```typescript
import type { Metadata } from "next";

export const metadata: Metadata = {
    title: "Templates de Proyecto | WePlay Rises",
    description: "Gestiona las plantillas de proyectos musicales para artistas noveles",
};
```

---

## 11. State Management

### 11.1 Server State (React Query)

Toda la data de backend se maneja con React Query:
- `useTemplates()` - cache de 5min
- `useTemplate(id)` - cache de 5min
- `useRolesProfesionales()` - cache de 10min (maestras)
- `useCategoriasRol()` - cache de 10min

**Invalidacion:**
- Al crear: invalidar `templates.all`
- Al actualizar: invalidar `templates.all` + `templates.byId(id)`
- Al eliminar: invalidar `templates.all`
- Al toggle status: invalidar `templates.all` + `templates.byId(id)`

---

### 11.2 Form State (React Hook Form)

Formularios manejados con React Hook Form:
- `TemplateForm` usa `useForm()` con `zodResolver`
- Necesidades usan `useFieldArray()` para lista dinamica
- Validacion on-submit + real-time en inputs criticos (precio max >= min)

---

### 11.3 Local UI State (useState)

Estado local minimo:
- `searchQuery` en page de lista
- `statusFilter` en page de lista
- `open` en dialogs de confirmacion

---

## 12. Integracion con UI/UX Spec

### 12.1 Design Tokens

**Usar variables CSS existentes del proyecto:**

```typescript
// Tailwind classes siguiendo ui-ux.md
const cardClasses = "bg-[#0f1729] border-[#334155]";
const textPrimaryClasses = "text-white";
const textSecondaryClasses = "text-[#94a3b8]";
const buttonPrimaryClasses = "bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700";
```

---

### 12.2 Prioridad Colors

```typescript
import { PRIORIDAD_NECESIDAD_COLORS } from "@shared/constants";

const PriorityBadge = ({ prioridad }: { prioridad: PrioridadNecesidad }) => {
    const colorMap = {
        Alta: "bg-red-500/10 text-red-500 border-red-500/20",
        Media: "bg-yellow-500/10 text-yellow-500 border-yellow-500/20",
        Baja: "bg-blue-500/10 text-blue-500 border-blue-500/20",
    };

    return (
        <Badge variant="outline" className={colorMap[prioridad]}>
            {prioridad}
        </Badge>
    );
};
```

---

### 12.3 Responsive Breakpoints

Seguir spec de ui-ux.md:
- Mobile (< 640px): Tabla se convierte en cards apiladas
- Tablet (640-1024px): Tabla con menos columnas
- Desktop (> 1024px): Tabla completa con todas las columnas

**Implementacion:**

```tsx
// Desktop: Table
<div className="hidden md:block">
    <TemplateTable templates={templates} ... />
</div>

// Mobile: Cards
<div className="md:hidden space-y-4">
    {templates.map(template => (
        <TemplateCard key={template.id} template={template} ... />
    ))}
</div>
```

---

## 13. Testing Strategy

### 13.1 Component Tests (Vitest + Testing Library)

**Archivos:**
- `__tests__/TemplateTable.test.tsx`
- `__tests__/TemplateForm.test.tsx`
- `__tests__/PriorityBadge.test.tsx`

**Casos clave:**
- TemplateTable: renderiza filas correctamente, maneja empty state
- TemplateForm: validacion de campos requeridos, submit exitoso
- PriorityBadge: colores correctos segun prioridad

---

### 13.2 Hook Tests

**Archivos:**
- `__tests__/use-templates.test.ts`
- `__tests__/use-templates-mutations.test.ts`

**Casos clave:**
- useTemplates: fetch exitoso, manejo de errores
- useCreateTemplate: mutation exitosa, invalidacion de queries

---

### 13.3 Service Tests

**Archivos:**
- `__tests__/template.service.test.ts`

**Casos clave:**
- getAll: retorna lista correctamente
- create: maneja errores de validacion

---

## 14. Checklist de Implementacion

### Pages
- [ ] Lista de templates (`/templates/page.tsx`)
- [ ] Crear template (`/templates/nuevo/page.tsx`)
- [ ] Detalle template (`/templates/[id]/page.tsx`)
- [ ] Editar template (`/templates/[id]/editar/page.tsx`)
- [ ] Loading state (`/templates/loading.tsx`)
- [ ] Error boundary (`/templates/error.tsx`)

### Componentes de Lista
- [ ] TemplateTable
- [ ] TemplateRow
- [ ] TemplateFilters

### Componentes de Detalle
- [ ] TemplateHeader
- [ ] NecesidadesSection (detalle)
- [ ] ResumenCard

### Componentes de Formulario
- [ ] TemplateForm
- [ ] GeneralDataSection
- [ ] NecesidadesSection (form)
- [ ] NecesidadFormItem

### Componentes Compartidos
- [ ] PriorityBadge
- [ ] IconPreview
- [ ] DeleteConfirmDialog
- [ ] EmptyState (reutilizar existente)

### Hooks
- [ ] use-templates.ts (2 queries)
- [ ] use-templates-mutations.ts (4 mutations)
- [ ] use-roles-profesionales.ts
- [ ] use-categorias-rol.ts

### Services
- [ ] template.service.ts
- [ ] necesidad.service.ts
- [ ] maestras.service.ts

### Validaciones
- [ ] template.schema.ts (Zod schemas)

### Tests
- [ ] Component tests (TemplateTable, TemplateForm, PriorityBadge)
- [ ] Hook tests (use-templates, mutations)
- [ ] Service tests (template.service)

### Integracion
- [ ] Types importados de @shared/types
- [ ] Constants importados de @shared/constants
- [ ] Query keys correctos
- [ ] API routes correctos
- [ ] Manejo de errores con toast
- [ ] Loading states con skeletons
- [ ] Responsive design (mobile/tablet/desktop)
- [ ] Accesibilidad (ARIA labels, keyboard navigation)

---

## 15. Siguiente Paso

**Este plan es READY para implementacion.**

### Orden de Implementacion Sugerido:

1. **Shared contracts** (BLOCKING) - Ya completado en `plans/cs-templates-guia/shared/contracts-plan.md`
2. **Services** - Crear los 3 services (template, necesidad, maestras)
3. **Hooks** - Crear hooks de queries y mutations
4. **Componentes Compartidos** - PriorityBadge, IconPreview, etc.
5. **Page de Lista** - Templates list con tabla y filtros
6. **Page de Detalle** - Template detail read-only
7. **Page de Formulario** - Create/Edit con form completo
8. **Tests** - Unit tests de componentes, hooks y services

### Dependencias Externas:

- **Backend API** - Endpoints deben estar implementados (ver `contracts.md`)
- **Shared types** - Deben estar en `src/shared/types/crowdsourcing.ts`
- **Shared constants** - Deben estar en `src/shared/constants/index.ts`

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (Senior Frontend Architect)
**Estado:** READY FOR IMPLEMENTATION
