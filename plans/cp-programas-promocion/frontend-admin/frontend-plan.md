# Plan Frontend: PromoPrograma (Admin)

**Fecha:** 2026-02-25
**Feature:** cp-programas-promocion (US-CP-02)
**Target:** src/admin (Next.js 14 App Router)

---

## 1. Resumen

| Elemento | Cantidad |
|----------|----------|
| Pages (Next.js) | 4 |
| Componentes | 27 |
| Hooks (query) | 3 |
| Hooks (mutation) | 3 |
| Hooks (estado wizard) | 1 |
| Services | 1 |
| Archivos totales a crear | 39 |

**Pantallas:**
1. `/dashboard/crowdpromotion/programas` - Listado Mis Programas
2. `/dashboard/crowdpromotion/programas/nuevo` - Wizard Crear Programa
3. `/dashboard/crowdpromotion/programas/[id]` - Detalle Programa
4. `/dashboard/crowdpromotion/programas/[id]/editar` - Editar Programa

---

## 2. Estructura de Carpetas

```
src/admin/src/app/(dashboard)/crowdpromotion/
├── programas/
│   ├── page.tsx                                    # Listado Mis Programas
│   ├── nuevo/
│   │   └── page.tsx                                # Wizard Crear Programa
│   └── [id]/
│       ├── page.tsx                                # Detalle Programa
│       ├── editar/
│       │   └── page.tsx                            # Editar Programa
│       └── components/
│           ├── PromoProgramaDetailClient.tsx        # Client shell del detalle
│           ├── PromoProgramaHeader.tsx              # Cabecera con acciones
│           ├── PromoProgramaKpiCards.tsx            # 4 KPI cards
│           ├── PromoProgramaInfoTab.tsx             # Tab Info General
│           ├── PromoProgramaTareasTab.tsx           # Tab Tareas
│           ├── PromoProgramaPromotoresTab.tsx       # Tab Promotores
│           ├── PromoProgramaResumenTab.tsx          # Tab Resumen
│           ├── DesactivarPromoProgramaDialog.tsx    # AlertDialog desactivar
│           └── __tests__/
│               ├── PromoProgramaHeader.test.tsx
│               └── DesactivarPromoProgramaDialog.test.tsx
│
└── components/                                     # Compartidos entre pantallas
    ├── list/
    │   ├── PromoProgramaListClient.tsx             # Client shell del listado
    │   ├── PromoProgramaCard.tsx                   # Card del listado
    │   ├── PromoProgramaStatusBadge.tsx            # Badge activo/inactivo
    │   ├── PromoProgramaFilters.tsx                # Filtros y busqueda
    │   ├── PromoProgramaEmptyState.tsx             # Empty state
    │   └── __tests__/
    │       ├── PromoProgramaCard.test.tsx
    │       └── PromoProgramaStatusBadge.test.tsx
    └── wizard/
        ├── PromoProgramaWizardContainer.tsx        # Orquestador del wizard
        ├── PromoProgramaWizardStepper.tsx          # Stepper personalizado 4 pasos
        ├── PromoProgramaWizardHeader.tsx           # Header con X Cancelar
        ├── PromoProgramaWizardFooter.tsx           # Footer con navegacion
        ├── PromoProgramaAbandonDialog.tsx          # AlertDialog abandono
        ├── steps/
        │   ├── DatosBasicosStep.tsx                # Paso 1
        │   ├── ComisionesStep.tsx                  # Paso 2
        │   ├── TareasStep.tsx                      # Paso 3
        │   └── RevisarPublicarStep.tsx             # Paso 4
        ├── tareas/
        │   ├── PromoTareaCard.tsx                  # Card de tarea en lista
        │   ├── PromoTareaForm.tsx                  # Formulario inline de tarea
        │   └── EliminarTareaDialog.tsx             # AlertDialog eliminar tarea
        └── __tests__/
            ├── PromoProgramaWizardStepper.test.tsx
            └── PromoTareaForm.test.tsx

src/admin/src/hooks/
├── use-promo-programas.ts                          # Query hooks (getAll, getById)
└── use-promo-programas-mutations.ts                # Mutation hooks (create, update, desactivar)

src/admin/src/hooks/__tests__/
├── use-promo-programas.test.ts
├── use-promo-programas-mutations.test.ts
└── use-promo-programa-wizard-state.test.ts

src/admin/src/services/
└── promo-programa.service.ts                       # API calls

src/admin/src/services/__tests__/
└── promo-programa.service.test.ts
```

---

## 3. Pages (Next.js Server Components)

### 3.1 Listado - `page.tsx`

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/page.tsx`

**Tipo:** Server Component

**Responsabilidad:** Exponer metadata, renderizar el client shell `PromoProgramaListClient`.

```typescript
export const metadata: Metadata = {
    title: "Mis Programas de Promocion | WePlay Rises",
    description: "Gestiona tus programas de promocion y sus promotores",
}

export default function MisProgramasPage() {
    return <PromoProgramaListClient />
}
```

---

### 3.2 Wizard Nuevo - `nuevo/page.tsx`

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/page.tsx`

**Tipo:** Server Component

**Responsabilidad:** Metadata + renderizar `PromoProgramaWizardContainer` en modo `"create"`.

---

### 3.3 Detalle - `[id]/page.tsx`

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/page.tsx`

**Tipo:** Server Component

**Parametros:** `params: { id: string }`

**Responsabilidad:** Metadata dinamica + renderizar `PromoProgramaDetailClient` con `programaId`.

---

### 3.4 Editar - `[id]/editar/page.tsx`

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/editar/page.tsx`

**Tipo:** Server Component

**Parametros:** `params: { id: string }`

**Responsabilidad:** Metadata + renderizar `PromoProgramaWizardContainer` en modo `"edit"` con `programaId`.

---

## 4. Componentes

### 4.1 PromoProgramaListClient

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaListClient.tsx`

**Directiva:** `"use client"`

**Props:** ninguna (obtiene datos con hooks)

**Estado Local:**
- `filtroEstado: PromoProgramaEstado | 'todos'` - default `'todos'`
- `busqueda: string` - filtro local, default `''`
- `paginaActual: number` - default `1`
- `showDesactivarDialog: boolean` - default `false`
- `programaADesactivar: PromoProgramaListItem | null` - default `null`

**Hooks utilizados:**
- `useMisProgramas({ esActivo, page, pageSize })` - datos paginados
- `useDesactivarPromoPrograma()` - mutation para desactivar

**Flujo de renderizado:**
1. `isLoading` → Grid de 3 skeletons `animate-pulse h-[160px] bg-[#1e1e38]`
2. `isError` → Card error con boton "Reintentar"
3. `data.items.length === 0 && !filtroEstado` → `PromoProgramaEmptyState`
4. `filteredItems.length === 0 && filtroEstado !== 'todos'` → Empty state con "Limpiar filtros"
5. Default → lista de `PromoProgramaCard` + paginacion

**Responsabilidad:** Shell cliente para el listado. Gestiona estado de filtros, paginacion y el dialog de desactivacion. Aplica filtro de busqueda localmente sobre los items devueltos por la API.

---

### 4.2 PromoProgramaCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaCard.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programa` | `PromoProgramaListItem` | Si | Datos del programa a mostrar |
| `onDesactivar` | `(programa: PromoProgramaListItem) => void` | Si | Callback para abrir dialog de desactivacion |

**Componentes UI:**
- `Card` de shadcn/ui para el contenedor
- `Badge` para tipo de programa y estado
- `DropdownMenu` para menu kebab (Ver detalle / Editar / Desactivar)
- Lucide icons: `MoreVertical`, `Users`, `ClipboardList`, `Music`, `Calendar`

**Responsabilidad:** Renderiza una card de programa con titulo, badges de tipo y estado, metricas (promotores, tareas, comision), campana vinculada, fechas y menu de acciones. Al hacer click en el cuerpo de la card navega a la pagina de detalle. Las opciones del menu delegan al padre o navegan directamente.

---

### 4.3 PromoProgramaStatusBadge

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaStatusBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `esActivo` | `boolean` | Si | Estado del programa |

**Componentes UI:** `Badge` de shadcn/ui

**Responsabilidad:** Renderiza badge "ACTIVO" (verde) o "INACTIVO" (gris) segun el estado. Centralizacion del estilo visual de estado.

---

### 4.4 PromoProgramaFilters

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaFilters.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `filtroEstado` | `PromoProgramaEstado \| 'todos'` | Si | Estado actual del filtro |
| `busqueda` | `string` | Si | Texto de busqueda actual |
| `onFiltroEstadoChange` | `(valor: PromoProgramaEstado \| 'todos') => void` | Si | Callback cambio de filtro |
| `onBusquedaChange` | `(valor: string) => void` | Si | Callback cambio de busqueda |

**Componentes UI:** `Select`, `SelectTrigger`, `SelectContent`, `SelectItem`, `Input`

**Responsabilidad:** Barra de filtros con select de estado y campo de busqueda. La busqueda tiene debounce de 300ms implementado internamente con `useState` + `useEffect`.

---

### 4.5 PromoProgramaEmptyState

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaEmptyState.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `variant` | `'no-programas' \| 'no-resultados'` | Si | Tipo de empty state |
| `onLimpiarFiltros` | `() => void` | No | Solo para variant "no-resultados" |

**Componentes UI:** `Button`
**Iconos:** `Megaphone` (no-programas), `SearchX` (no-resultados)

**Responsabilidad:** Dos variantes de empty state. "no-programas" con CTA a crear nuevo programa. "no-resultados" con boton para limpiar filtros activos.

---

### 4.6 PromoProgramaWizardContainer

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardContainer.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `mode` | `'create' \| 'edit'` | Si | Modo del wizard |
| `programaId` | `string` | No | Solo en modo edit |

**Estado Local:**
- `showAbandonDialog: boolean` - controla el AlertDialog de abandono

**Hooks utilizados:**
- `usePromoProgramaWizardState(initialData?)` - estado del wizard con useReducer
- `usePromoPrograma(programaId)` - solo en modo edit para cargar datos iniciales
- `useCreatePromoPrograma()` - mutation crear
- `useUpdatePromoPrograma()` - mutation actualizar
- `router` de next/navigation para redireccion post-submit

**Responsabilidad:** Orquestador principal del wizard. Coordina el estado global del wizard, las transiciones entre pasos, el submit final a la API y la navegacion post-exito. Contiene el layout full-page (header + stepper + contenido del paso activo + footer). En modo edit, espera a que los datos del programa se carguen antes de inicializar el wizard.

**Flujo submit:**
1. Llamar `createPromoPrograma.mutateAsync(payload)` o `updatePromoPrograma.mutateAsync(id, payload)`
2. En success: `toast.success("Programa de promocion creado")` + `router.push(APP_ROUTES.dashboard.crowdpromotion.programas.detalle(result.id))`
3. En error: extraer `errorCode` de `axiosError.response.data.messages[0]` + `toast.error(getPromoProgramaErrorMessage(errorCode))`

---

### 4.7 PromoProgramaWizardStepper

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardStepper.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `pasoActual` | `number` | Si | Numero del paso activo (1-4) |
| `pasosCompletados` | `number[]` | Si | Array de pasos ya completados |
| `onPasoClick` | `(paso: number) => void` | No | Solo pasos completados son clickeables |

**Responsabilidad:** Stepper visual de 4 pasos con estilos propios del wizard (circulo activo gradient pink/purple, completado con fondo verde y checkmark, futuro muted). Lineas conectoras que cambian de color al completar el paso. Labels: "Datos basicos", "Comisiones", "Definir tareas", "Revisar". Componente de presentacion puro.

**Definicion de pasos (constante interna):**
```typescript
const PASOS_PROGRAMA = [
    { numero: 1, label: "Datos basicos" },
    { numero: 2, label: "Comisiones" },
    { numero: 3, label: "Definir tareas" },
    { numero: 4, label: "Revisar" },
]
```

---

### 4.8 PromoProgramaWizardHeader

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardHeader.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `mode` | `'create' \| 'edit'` | Si | Controla el titulo del header |
| `onCancelar` | `() => void` | Si | Callback para el boton X (abre dialog de abandono) |

**Responsabilidad:** Header fijo del wizard (`bg-[#0d0d1a] border-b h-14`). Logo WePlay a la izquierda, titulo centrado ("Crear programa de promocion" o "Editar programa de promocion"), boton X a la derecha. Llama a `onCancelar` que abre el `PromoProgramaAbandonDialog`.

---

### 4.9 PromoProgramaWizardFooter

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardFooter.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `pasoActual` | `number` | Si | Para mostrar "Paso X de 4" y estado del boton Anterior |
| `totalPasos` | `number` | Si | Default 4 |
| `onAnterior` | `() => void` | Si | Callback boton Anterior |
| `onSiguiente` | `() => void` | Si | Callback boton Siguiente / Publicar |
| `isSubmitting` | `boolean` | Si | Deshabilita botones y muestra spinner en paso 4 |
| `mode` | `'create' \| 'edit'` | Si | Cambia label del boton en paso 4 |

**Responsabilidad:** Footer fijo (`bg-[#0d0d1a] border-t h-16`). Texto "Paso X de 4" centrado. Boton Anterior (outline, deshabilitado en paso 1). Boton Siguiente (gradient pink/purple); en paso 4 cambia label a "Publicar programa" (create) o "Guardar cambios" (edit) con icono `Rocket` / `Save`.

---

### 4.10 PromoProgramaAbandonDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaAbandonDialog.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad |
| `onOpenChange` | `(open: boolean) => void` | Si | Controla visibilidad |
| `onConfirmar` | `() => void` | Si | Callback al confirmar salida |

**Componentes UI:** `AlertDialog`, `AlertDialogContent`, `AlertDialogHeader`, `AlertDialogTitle`, `AlertDialogDescription`, `AlertDialogFooter`, `AlertDialogCancel`, `AlertDialogAction`

**Responsabilidad:** Dialog de confirmacion de abandono del wizard. Texto: "Perderas todo el progreso del wizard. ¿Deseas salir?". Acciones: "Seguir editando" (cancelar) y "Salir sin guardar" (confirmar y navegar a listado).

---

### 4.11 DatosBasicosStep (Paso 1)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/DatosBasicosStep.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `defaultValues` | `Partial<CreatePromoProgramaFormData>` | No | Datos pre-rellenados (modo edit) |
| `onNext` | `(data: Partial<CreatePromoProgramaFormData>) => void` | Si | Callback al validar y avanzar |
| `tienePromotores` | `boolean` | No | Muestra banner de aviso si true |

**Estado del formulario:** `useForm<DatosBasicosStepData>` con `zodResolver(promoProgramaStep1Schema)`

**Schema local:** `promoProgramaStep1Schema` (subconjunto de `createPromoProgramaSchema`):
- Campos: `titulo`, `descripcion`, `tipoPromoId`, `campaniaCrowdfundingId?`, `proyectoArtisticoId?`, `urlLanding?`, `codigoTrackingBase?`, `fechaInicio?`, `fechaFin?`
- Refine: `fechaFin > fechaInicio` si ambas presentes

**Datos para selects:**
- `tipoPromoId`: populado con `TIPO_PROMO_LABELS` desde shared/constants (sin llamada API)
- `campaniaCrowdfundingId`: opcional, en MVP mostrar input libre de UUID o select con `useMisCampanias()` si existe el hook
- `proyectoArtisticoId`: opcional, en MVP mostrar input libre de UUID

**Componentes UI:** `Card`, `CardHeader`, `CardTitle`, `CardContent`, `Input`, `Textarea`, `Label`, `Select`, `Separator`

**Responsabilidad:** Formulario del paso 1. Valida con Zod al hacer submit (click Siguiente en el footer activa el submit de este form via `id` de formulario). Si el programa tiene promotores inscritos (`tienePromotores=true`), muestra banner de aviso amber. Muestra hint contextual del tipo de programa seleccionado.

---

### 4.12 ComisionesStep (Paso 2)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/ComisionesStep.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `defaultValues` | `Partial<CreatePromoProgramaFormData>` | No | Datos pre-rellenados |
| `onNext` | `(data: Partial<CreatePromoProgramaFormData>) => void` | Si | Callback al validar y avanzar |
| `tienePromotores` | `boolean` | No | Muestra banner de aviso |

**Estado del formulario:** `useForm<ComisionesStepData>` con `zodResolver(promoProgramaStep2Schema)`

**Schema local:** `promoProgramaStep2Schema`:
- Campos: `monedaId`, `importeComisionPorcentaje?`, `importeComisionFija?`
- Refine: al menos una comision definida

**Vista previa de comision:** Calculada en tiempo real con `watch()`. Ejemplo base de 50 EUR. Si ninguna comision definida: mostrar texto muted "Ingresa al menos una comision para ver la vista previa".

**Componentes UI:** `Card`, `CardContent`, `Input`, `Label`, `Select`, `Separator`

**Responsabilidad:** Formulario del paso 2 con select de moneda, inputs de comision porcentaje y fija, aviso de obligatoriedad y card de vista previa calculada en tiempo real. La moneda se selecciona aqui y aplica a todas las comisiones.

---

### 4.13 TareasStep (Paso 3)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/TareasStep.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tareas` | `CreatePromoTareaItem[]` | Si | Lista actual de tareas del wizard |
| `onTareasChange` | `(tareas: CreatePromoTareaItem[]) => void` | Si | Callback al modificar la lista |
| `onNext` | `() => void` | Si | Avanzar al paso 4 (sin validacion extra, tareas ya guardadas) |
| `tienePromotores` | `boolean` | No | Muestra banner de aviso |

**Estado Local:**
- `editandoTareaIndex: number | null` - indice de tarea en edicion, `null` = formulario cerrado
- `mostrandoFormNuevo: boolean` - true cuando se agrega una nueva tarea
- `tareaAEliminarIndex: number | null` - para el dialog de confirmacion

**Componentes hijos:**
- `PromoTareaCard` - para cada tarea de la lista
- `PromoTareaForm` - formulario inline (expandido/colapsado)
- `EliminarTareaDialog` - dialog de confirmacion de eliminacion

**Responsabilidad:** Gestor de la lista de tareas. Muestra la lista de `PromoTareaCard` con boton "+ Agregar nueva tarea" al final. Al agregar/editar, expande `PromoTareaForm` inline. Solo una tarea puede estar en edicion a la vez. Aviso informativo de que las tareas son opcionales.

---

### 4.14 RevisarPublicarStep (Paso 4)

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/RevisarPublicarStep.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `formData` | `Partial<CreatePromoProgramaFormData>` | Si | Datos acumulados del wizard |
| `onEditarPaso` | `(paso: number) => void` | Si | Navegar a un paso especifico para editar |
| `isSubmitting` | `boolean` | Si | Estado de carga del submit |
| `mode` | `'create' \| 'edit'` | Si | Para adaptar textos |

**Responsabilidad:** Vista de solo lectura con resumen de los 3 pasos anteriores. Tres secciones: "Datos del programa", "Comisiones" y "Tareas (N)". Cada seccion tiene boton "Editar" que llama `onEditarPaso(1|2|3)`. Valores nulos se muestran como "--" en texto muted italic. Aviso informativo de que al confirmar el programa se publicara con estado Activo.

---

### 4.15 PromoTareaCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/PromoTareaCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tarea` | `CreatePromoTareaItem` | Si | Datos de la tarea |
| `index` | `number` | Si | Posicion en el array (para key y callbacks) |
| `onEditar` | `(index: number) => void` | Si | Abrir formulario inline de edicion |
| `onEliminar` | `(index: number) => void` | Si | Abrir dialog de confirmacion |
| `puedeEliminar` | `boolean` | Si | false si tarea tiene completados (modo edit) |
| `eliminandoDeshabilitadoTooltip` | `string` | No | Texto del tooltip cuando puedeEliminar=false |

**Componentes UI:** `Card`, `Badge`, `Button`, `Tooltip`, `TooltipContent`, `TooltipProvider`

**Responsabilidad:** Card compacta de una tarea en la lista del paso 3. Muestra badge tipo evento, nombre, toggle-visual de estado activa, detalles (recompensa, repetible, fechas) y botones Editar / Eliminar. En modo edit, si la tarea tiene completados (`puedeEliminar=false`), el boton Eliminar aparece deshabilitado con tooltip.

---

### 4.16 PromoTareaForm

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/PromoTareaForm.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `defaultValues` | `Partial<CreatePromoTareaItem>` | No | Datos pre-rellenados en edicion |
| `onGuardar` | `(tarea: CreatePromoTareaItem) => void` | Si | Tarea validada lista para agregar al array |
| `onCancelar` | `() => void` | Si | Cerrar formulario sin guardar |
| `titulo` | `string` | No | "Nueva tarea" o "Editar tarea" |

**Estado del formulario:** `useForm<CreatePromoTareaItem>` con `zodResolver(createPromoTareaSchema)` (schema importado de `@shared/schemas`)

**Visibilidad condicional (via `watch()`):**
- `tipoRewardId === 1 || 3` → mostrar campos `importeRecompensa` y `monedaId`
- `tipoRewardId === 2 || 3` → mostrar campo `puntosRecompensa`
- `esRepetible === true` → mostrar campo `maxRepeticiones` como requerido

**Componentes UI:** `Card`, `Input`, `Textarea`, `Label`, `Select`, `Button`, `Separator`
**Componentes especiales:** `Switch` + `Label` para el toggle `esRepetible`

**Responsabilidad:** Formulario inline expandible para crear/editar una tarea. Gestiona la visibilidad condicional de campos de recompensa con `watch()`. Al hacer "Guardar tarea", llama `handleSubmit` de react-hook-form que valida con Zod y llama `onGuardar`. Borde accent `border-[#a855f7]` para destacar que esta en edicion.

---

### 4.17 EliminarTareaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/EliminarTareaDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad |
| `onOpenChange` | `(open: boolean) => void` | Si | Controla visibilidad |
| `onConfirmar` | `() => void` | Si | Eliminar la tarea del array |
| `nombreTarea` | `string` | Si | Nombre de la tarea a eliminar |

**Componentes UI:** `AlertDialog` y variantes

**Responsabilidad:** Dialog de confirmacion pequeno para eliminar una tarea del wizard. Muestra el nombre de la tarea para confirmacion explicita.

---

### 4.18 PromoProgramaDetailClient

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programaId` | `string` | Si | ID del programa a cargar |

**Estado Local:**
- `showDesactivarDialog: boolean`
- `tabActiva: 'info' \| 'tareas' \| 'promotores' \| 'resumen'`

**Hooks utilizados:**
- `usePromoPrograma(programaId)` - datos del detalle

**Flujo de renderizado:**
1. `isLoading` → Skeleton para header, KPI cards y tabs
2. `isError || !data` → Card de error con boton "Reintentar"
3. Default → `PromoProgramaHeader` + banner si inactivo + `PromoProgramaKpiCards` + Tabs con 4 contenidos

**Responsabilidad:** Shell cliente del detalle. Coordina la carga de datos y la estructura de tabs del detalle. Pasa `onDesactivar` al header que abre el `DesactivarPromoProgramaDialog`.

---

### 4.19 PromoProgramaHeader

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaHeader.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programa` | `PromoProgramaDetail` | Si | Datos completos del programa |
| `onDesactivar` | `() => void` | Si | Abre el dialog de desactivacion |

**Componentes UI:** `Badge`, `Button`
**Iconos:** `Pencil`, `PowerOff`, `Power`, `Music`

**Responsabilidad:** Cabecera del detalle con titulo del programa, badges de tipo y estado, nombre de campana vinculada, y botones de accion. Si `esActivo=true`: boton "Desactivar" (rojo). Si `esActivo=false`: boton "Reactivar" (verde, MVP: puede ser deshabilitado con tooltip "Reactivacion no disponible en MVP"). Boton "Editar" siempre visible navega a la ruta de edicion.

---

### 4.20 PromoProgramaKpiCards

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaKpiCards.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `resumen` | `PromoProgramaResumen` | Si | Metricas del programa |

**Componentes UI:** `Card` (patron de `StatsCard` existente)
**Iconos:** `UserCheck`, `Clock`, `Activity`, `TrendingUp`

**Responsabilidad:** Grid de 4 KPI cards: "Promotores aprobados" (verde), "Promotores pendientes" (amarillo), "Total eventos" (azul), "Valor generado" (purple). Sigue el patron visual de `StatsCard` en `src/admin/src/components/dashboard/stats-card.tsx`.

---

### 4.21 PromoProgramaInfoTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaInfoTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programa` | `PromoProgramaDetail` | Si | Datos completos |

**Responsabilidad:** Tab de info general. Grid de 2 columnas (desktop) con 2 cards: "Datos del programa" (titulo, descripcion, tipo, campana, proyecto, periodo) y "Configuracion de comisiones" (moneda, comision %, comision fija, URL landing, codigo tracking). Valores nulos como "--" italic.

---

### 4.22 PromoProgramaTareasTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaTareasTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tareas` | `PromoTareaDetail[]` | Si | Lista de tareas del programa |
| `esActivo` | `boolean` | Si | Para mostrar boton "Agregar tarea" solo si activo |

**Responsabilidad:** Tab de tareas del detalle. Muestra lista de cards de tarea con badges de tipo evento, tipo recompensa, estado activa/inactiva, completados por promotores. Boton "Agregar tarea" solo visible si `esActivo=true`. En MVP, "Agregar tarea" navega a la pagina de edicion del programa. Empty state si no hay tareas.

---

### 4.23 PromoProgramaPromotoresTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaPromotoresTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `promotores` | `PromoProgramaPromotorSummary[]` | Si | Lista de promotores inscritos |

**Componentes UI:** `Table`, `TableHeader`, `TableBody`, `TableRow`, `TableHead`, `TableCell`, `Avatar`, `AvatarFallback`, `Badge`

**Responsabilidad:** Tab de promotores inscritos. Tabla con columnas: Promotor (avatar + nombre), Tipo, Estado (Aprobado/Pendiente/Bloqueado), Fecha de alta. Empty state si no hay promotores.

---

### 4.24 PromoProgramaResumenTab

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaResumenTab.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `resumen` | `PromoProgramaResumen` | Si | Metricas del programa |

**Responsabilidad:** Tab de resumen/metricas con las mismas metricas que las KPI cards pero en formato tabla/lista mas detallada con texto explicativo. Para MVP: version simplificada sin chart. Cada metrica con label, valor y descripcion contextual.

---

### 4.25 DesactivarPromoProgramaDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/DesactivarPromoProgramaDialog.tsx`

**Directiva:** `"use client"`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `open` | `boolean` | Si | Estado de visibilidad |
| `onOpenChange` | `(open: boolean) => void` | Si | Controla visibilidad |
| `programaId` | `string` | Si | ID del programa a desactivar |
| `tituloProgramas` | `string` | Si | Para mostrar el nombre en el dialog |

**Hooks utilizados:** `useDesactivarPromoPrograma()`

**Responsabilidad:** Dialog de confirmacion de desactivacion. Muestra el titulo del programa y avisa que todas sus tareas activas tambien se desactivaran y dejara de aparecer en el catalogo publico. Al confirmar, llama `desactivarMutation.mutate(programaId)` y cierra el dialog en `onSuccess`.

---

### 4.26 PromoProgramaListCard (Skeleton)

No es un componente separado. El skeleton del listado se implementa directamente en `PromoProgramaListClient` como 3 divs `animate-pulse` con la estructura de la card.

---

### 4.27 Nota sobre componente de paginacion

El listado usa paginacion server-side. Si existe un componente `Pagination` en `src/admin/src/components/ui/`, reutilizarlo. Si no existe, crear un componente simple inline en `PromoProgramaListClient` con botones Anterior/Siguiente y numeros de pagina.

---

## 5. Hooks

### 5.1 useMisProgramas

**Archivo:** `src/admin/src/hooks/use-promo-programas.ts`

**Tipo:** Query Hook

**Firma:**
```typescript
interface MisProgramasParams {
    esActivo?: boolean
    page?: number
    pageSize?: number
}

export function useMisProgramas(params: MisProgramasParams = {})
```

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `esActivo` | `boolean \| undefined` | Filtro de estado. undefined = todos |
| `page` | `number` | Pagina actual. Default 1 |
| `pageSize` | `number` | Items por pagina. Default 10 |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `PromoProgramaListResult \| undefined` | Items paginados con totalCount |
| `isLoading` | `boolean` | Primera carga |
| `isFetching` | `boolean` | Cualquier carga (incluye cambios de pagina) |
| `isError` | `boolean` | Error de red o API |
| `refetch` | `function` | Recargar manualmente |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.misFiltrados({ esActivo, page, pageSize })`

**Query Fn:** `promoProgramaService.getMisProgramas(params)`

**Opciones:** `staleTime: 30_000`

---

### 5.2 usePromoPrograma

**Archivo:** `src/admin/src/hooks/use-promo-programas.ts`

**Tipo:** Query Hook

**Firma:**
```typescript
export function usePromoPrograma(id: string)
```

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `id` | `string` | ID del programa (Guid) |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `PromoProgramaDetail \| null \| undefined` | Detalle completo |
| `isLoading` | `boolean` | Estado de carga |
| `isError` | `boolean` | Error |
| `refetch` | `function` | Recargar |

**Query Key:** `QUERY_KEYS.crowdpromotion.programas.byId(id)`

**Query Fn:** `promoProgramaService.getById(id)`

**Opciones:** `enabled: !!id`, `retry: false`, `staleTime: 30_000`

---

### 5.3 useCreatePromoPrograma

**Archivo:** `src/admin/src/hooks/use-promo-programas-mutations.ts`

**Tipo:** Mutation Hook

**Firma:**
```typescript
export function useCreatePromoPrograma()
```

**Retorna:** `UseMutationResult` de TanStack Query

**mutationFn input:** `CreatePromoProgramaRequest`

**mutationFn output:** `PromoProgramaCreatedResult`

**Acciones:**
- `onSuccess`: invalidar `QUERY_KEYS.crowdpromotion.programas.mis` + el key especifico. Toast success gestionado en el componente contenedor (no aqui, para poder redirigir).
- `onError`: `toast.error(getPromoProgramaErrorMessage(errorCode))`

**Nota:** El toast de success y la redireccion se gestionan en `PromoProgramaWizardContainer` via `mutateAsync` + `try/catch`, siguiendo el patron del `WizardContainer` existente de campanias.

---

### 5.4 useUpdatePromoPrograma

**Archivo:** `src/admin/src/hooks/use-promo-programas-mutations.ts`

**Tipo:** Mutation Hook

**Firma:**
```typescript
export function useUpdatePromoPrograma()
```

**mutationFn input:** `{ id: string; data: UpdatePromoProgramaRequest }`

**mutationFn output:** `PromoProgramaUpdatedResult`

**Acciones:**
- `onSuccess`: invalidar `QUERY_KEYS.crowdpromotion.programas.byId(id)` + `QUERY_KEYS.crowdpromotion.programas.mis`

---

### 5.5 useDesactivarPromoPrograma

**Archivo:** `src/admin/src/hooks/use-promo-programas-mutations.ts`

**Tipo:** Mutation Hook

**Firma:**
```typescript
export function useDesactivarPromoPrograma()
```

**mutationFn input:** `string` (programaId)

**mutationFn output:** `PromoProgramaDesactivadoResult`

**Acciones:**
- `onSuccess`: invalidar `QUERY_KEYS.crowdpromotion.programas.byId(id)` + `QUERY_KEYS.crowdpromotion.programas.mis`. `toast.success("Programa desactivado correctamente")`
- `onError`: `toast.error(getPromoProgramaErrorMessage(errorCode))`

---

### 5.6 usePromoProgramaWizardState

**Archivo:** `src/admin/src/hooks/use-promo-programa-wizard-state.ts`

**Tipo:** Custom Hook con useReducer

**Firma:**
```typescript
export function usePromoProgramaWizardState(
    initialData?: Partial<CreatePromoProgramaFormData>
)
```

**State type:**
```typescript
interface PromoProgramaWizardState {
    pasoActual: number                           // 1-4
    formData: Partial<CreatePromoProgramaFormData>
    pasosCompletados: number[]
    isHydrated: boolean
}
```

**Actions type:**
```typescript
type PromoProgramaWizardAction =
    | { type: 'SET_PASO'; paso: number }
    | { type: 'UPDATE_FORM_DATA'; data: Partial<CreatePromoProgramaFormData> }
    | { type: 'UPDATE_TAREAS'; tareas: CreatePromoTareaItem[] }
    | { type: 'MARCAR_PASO_COMPLETADO'; paso: number }
    | { type: 'RESET' }
    | { type: 'HYDRATE' }
```

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `pasoActual` | `number` | Paso activo (1-4) |
| `formData` | `Partial<CreatePromoProgramaFormData>` | Datos acumulados |
| `pasosCompletados` | `number[]` | Pasos con datos validos |
| `isHydrated` | `boolean` | Hidratacion de localStorage completada |
| `irAPaso` | `(paso: number) => void` | Navega a un paso (solo si es accesible) |
| `actualizarFormData` | `(data: Partial<CreatePromoProgramaFormData>) => void` | Merge de datos |
| `actualizarTareas` | `(tareas: CreatePromoTareaItem[]) => void` | Actualizar array de tareas |
| `marcarPasoCompletado` | `(paso: number) => void` | Marcar paso como completo |
| `resetWizard` | `() => void` | Limpiar todo incluido localStorage |

**Persistencia en localStorage:** Solo en modo create (`!initialData`). Key: `"promo-programa-wizard-draft"`. Misma logica que `useWizardState` del campanias wizard: cargar en mount + guardar en cada cambio + limpiar en reset.

**Diferencia con campanias:** Usa `useReducer` en lugar de multiples `useState`. El array `tareas` es parte del `formData` (como `formData.tareas: CreatePromoTareaItem[]`).

---

## 6. Services

### 6.1 promoProgramaService

**Archivo:** `src/admin/src/services/promo-programa.service.ts`

**Patron:** Clase con metodos async, instancia singleton exportada, igual que `promotorService` y `campaniaService`.

**Metodos:**
| Metodo | Input | Output | Endpoint | HTTP |
|--------|-------|--------|----------|------|
| `getMisProgramas(params)` | `{ esActivo?: boolean, page?: number, pageSize?: number }` | `PromoProgramaListResult` | `API_ROUTES.crowdpromotion.programas.mis` + query params | GET |
| `getById(id)` | `string` | `PromoProgramaDetail \| null` | `API_ROUTES.crowdpromotion.programas.byId(id)` | GET |
| `create(data)` | `CreatePromoProgramaRequest` | `PromoProgramaCreatedResult` | `API_ROUTES.crowdpromotion.programas.base` | POST |
| `update(id, data)` | `string, UpdatePromoProgramaRequest` | `PromoProgramaUpdatedResult` | `API_ROUTES.crowdpromotion.programas.byId(id)` | PUT |
| `desactivar(id)` | `string` | `PromoProgramaDesactivadoResult` | `API_ROUTES.crowdpromotion.programas.desactivar(id)` | PATCH |

**Patron de error handling** (igual que `promotorService`):
```typescript
async create(data: CreatePromoProgramaRequest): Promise<PromoProgramaCreatedResult> {
    const response = await apiFetch<ServiceResponse<PromoProgramaCreatedResult>>(
        API_ROUTES.crowdpromotion.programas.base,
        { method: "POST", data }
    )
    if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
        const errorCode = response.messages.find(
            (m) => m.errorCode && !m.errorCode.startsWith("0")
        )?.errorCode
        throw new Error(getPromoProgramaErrorMessage(errorCode ?? "5000"))
    }
    return response.data
}
```

**getMisProgramas**: construir query string con `URLSearchParams` igual que `getBackings` en `campaniaService`.

**getById**: envuelto en try/catch que retorna `null` si 404, igual que `getById` de campanias.

**Imports:**
```typescript
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getPromoProgramaErrorMessage } from "@shared/utils/error-messages"
import type {
    CreatePromoProgramaRequest,
    UpdatePromoProgramaRequest,
    PromoProgramaCreatedResult,
    PromoProgramaListResult,
    PromoProgramaDetail,
    PromoProgramaUpdatedResult,
    PromoProgramaDesactivadoResult,
} from "@shared/types"
import type { ServiceResponse } from "@shared/types"
```

---

## 7. Estado del Wizard (useReducer)

### 7.1 Estado Inicial

```typescript
const initialState: PromoProgramaWizardState = {
    pasoActual: 1,
    formData: {},
    pasosCompletados: [],
    isHydrated: false,
}
```

En modo edit (`initialData` presente):
```typescript
const initialStateEdit: PromoProgramaWizardState = {
    pasoActual: 1,
    formData: initialData,
    pasosCompletados: [1, 2, 3],   // todos los pasos marcados como completados
    isHydrated: true,               // no espera localStorage
}
```

### 7.2 Reducer

```typescript
function promoProgramaWizardReducer(
    state: PromoProgramaWizardState,
    action: PromoProgramaWizardAction
): PromoProgramaWizardState
```

| Action | Comportamiento |
|--------|----------------|
| `SET_PASO` | Solo si el paso es accesible: `paso <= max(pasosCompletados) + 1` o ya completado |
| `UPDATE_FORM_DATA` | Merge `{ ...state.formData, ...action.data }` + marcar `pasoActual` como completado si no estaba |
| `UPDATE_TAREAS` | Actualizar `formData.tareas` + marcar paso 3 como completado |
| `MARCAR_PASO_COMPLETADO` | Agregar paso al array si no existe |
| `RESET` | Volver a `initialState` |
| `HYDRATE` | Marcar `isHydrated: true` |

### 7.3 Flujo del Wizard en Creacion

```
Paso 1: DatosBasicosStep
    submit form local → UPDATE_FORM_DATA(datosBasicos) → SET_PASO(2)

Paso 2: ComisionesStep
    submit form local → UPDATE_FORM_DATA(comisiones) → SET_PASO(3)

Paso 3: TareasStep
    guardar/editar/eliminar tarea → UPDATE_TAREAS(nuevaLista)
    click Siguiente → (sin validacion extra) → SET_PASO(4)

Paso 4: RevisarPublicarStep
    click Publicar/Guardar → WizardContainer llama API → success → reset + redirect
```

### 7.4 Mapeado formData → Request

En `PromoProgramaWizardContainer.handleSubmit`:

```typescript
const payload: CreatePromoProgramaRequest = {
    titulo: formData.titulo!,
    tipoPromoId: formData.tipoPromoId!,
    monedaId: formData.monedaId!,
    descripcion: formData.descripcion || undefined,
    campaniaCrowdfundingId: formData.campaniaCrowdfundingId || undefined,
    proyectoArtisticoId: formData.proyectoArtisticoId || undefined,
    urlLanding: formData.urlLanding || undefined,
    codigoTrackingBase: formData.codigoTrackingBase || undefined,
    importeComisionPorcentaje: formData.importeComisionPorcentaje,
    importeComisionFija: formData.importeComisionFija,
    fechaInicio: formData.fechaInicio || undefined,
    fechaFin: formData.fechaFin || undefined,
    tareas: formData.tareas?.map((t) => ({
        titulo: t.titulo,
        tipoEventoPromoId: t.tipoEventoPromoId,
        tipoRewardId: t.tipoRewardId,
        esRepetible: t.esRepetible,
        descripcion: t.descripcion || undefined,
        importeRecompensa: t.importeRecompensa,
        monedaId: t.monedaId,
        puntosRecompensa: t.puntosRecompensa,
        urlInstrucciones: t.urlInstrucciones || undefined,
        maxRepeticiones: t.maxRepeticiones,
        fechaInicio: t.fechaInicio || undefined,
        fechaFin: t.fechaFin || undefined,
    })) || [],
}
```

Los strings vacios se convierten a `undefined` antes de enviar al backend.

---

## 8. Flujo de Datos

### 8.1 Flujo Wizard Crear Programa

```
NuevoProgramaPage (Server)
    └── PromoProgramaWizardContainer (Client)
            ├── usePromoProgramaWizardState()      → estado local
            ├── useCreatePromoPrograma()            → mutation
            ├── PromoProgramaWizardHeader          → X → AbandonDialog
            ├── PromoProgramaWizardStepper         → indicador visual
            ├── [paso activo]
            │   ├── DatosBasicosStep               → UPDATE_FORM_DATA
            │   ├── ComisionesStep                 → UPDATE_FORM_DATA
            │   ├── TareasStep                     → UPDATE_TAREAS
            │   │   ├── PromoTareaCard             → editar/eliminar
            │   │   ├── PromoTareaForm             → guardar tarea
            │   │   └── EliminarTareaDialog        → confirmar eliminacion
            │   └── RevisarPublicarStep            → submit final
            └── PromoProgramaWizardFooter          → anterior/siguiente/publicar
```

### 8.2 Flujo Listado Programas

```
MisProgramasPage (Server)
    └── PromoProgramaListClient (Client)
            ├── useMisProgramas({ esActivo, page }) → GET /api/programas/mis-programas
            ├── PromoProgramaFilters                → estado local de filtros
            ├── PromoProgramaCard (x N)             → menu kebab
            │   └── onDesactivar → PromoProgramaListClient.setShowDesactivarDialog
            └── DesactivarPromoProgramaDialog       → useDesactivarPromoPrograma
```

### 8.3 Flujo Detalle Programa

```
DetalleProgramaPage (Server)
    └── PromoProgramaDetailClient (Client)
            ├── usePromoPrograma(programaId)        → GET /api/programas/{id}
            ├── PromoProgramaHeader                 → botones Editar/Desactivar
            ├── PromoProgramaKpiCards               → resumen.totalPromotoresAprobados, etc.
            ├── Tabs
            │   ├── PromoProgramaInfoTab            → datos del programa
            │   ├── PromoProgramaTareasTab          → programa.tareas[]
            │   ├── PromoProgramaPromotoresTab      → programa.promotores[]
            │   └── PromoProgramaResumenTab         → programa.resumen
            └── DesactivarPromoProgramaDialog       → useDesactivarPromoPrograma
```

### 8.4 Flujo Editar Programa

```
EditarProgramaPage (Server)
    └── PromoProgramaWizardContainer (mode="edit", programaId)
            ├── usePromoPrograma(programaId)        → carga datos iniciales
            │   └── mapear PromoProgramaDetail → initialData (Partial<CreatePromoProgramaFormData>)
            ├── usePromoProgramaWizardState(initialData)
            ├── useUpdatePromoPrograma()            → mutation update
            └── [mismos pasos del wizard, pre-rellenados]
                └── TareasStep con puedeEliminar=false si tarea.completadosPorPromotores > 0
```

---

## 9. Dependencias de Shared

**Importar de `@shared/types`:**
- `CreatePromoTareaItem`
- `UpdatePromoTareaItem`
- `CreatePromoProgramaRequest`
- `UpdatePromoProgramaRequest`
- `PromoProgramaCreatedResult`
- `PromoProgramaListItem`
- `PromoProgramaListResult`
- `PromoTareaDetail`
- `PromoProgramaPromotorSummary`
- `PromoProgramaResumen`
- `PromoProgramaDetail`
- `PromoProgramaUpdatedResult`
- `PromoProgramaDesactivadoResult`
- `PromoProgramaEstado`
- `ServiceResponse` (ya existe)

**Importar de `@shared/schemas`:**
- `createPromoProgramaSchema` (validacion form wizard)
- `updatePromoProgramaSchema` (validacion form edicion)
- `CreatePromoProgramaFormData` (type inferido)
- `UpdatePromoProgramaFormData` (type inferido)

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdpromotion.programas` (mis, byId, misFiltrados)
- `API_ROUTES.crowdpromotion.programas` (base, mis, byId, desactivar)
- `APP_ROUTES.dashboard.crowdpromotion.programas` (list, nuevo, detalle, editar)
- `TIPO_PROMO`, `TIPO_PROMO_LABELS`, `TIPO_PROMO_DESCRIPTIONS`
- `TIPO_EVENTO_PROMO`, `TIPO_EVENTO_PROMO_LABELS`
- `TIPO_REWARD_PROMO`, `TIPO_REWARD_PROMO_LABELS`
- `VALIDATION` (limites PROGRAMA_TITULO_MAX, etc.)

**Importar de `@shared/utils/error-messages`:**
- `getPromoProgramaErrorMessage`
- `PROMO_PROGRAMA_ERROR_MESSAGES`

---

## 10. Archivos a Crear

### Pages y Client Shells

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/page.tsx` | Server Page | Listado mis programas |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/nuevo/page.tsx` | Server Page | Wizard crear programa |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/page.tsx` | Server Page | Detalle programa |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/editar/page.tsx` | Server Page | Editar programa |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaListClient.tsx` | Client Component | Shell del listado |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaDetailClient.tsx` | Client Component | Shell del detalle |

### Componentes - Listado

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaCard.tsx` | Client Component | Card del listado con menu |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaStatusBadge.tsx` | Server Component | Badge activo/inactivo |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaFilters.tsx` | Client Component | Filtros y busqueda |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/PromoProgramaEmptyState.tsx` | Server Component | Empty states |

### Componentes - Wizard

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardContainer.tsx` | Client Component | Orquestador del wizard |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardStepper.tsx` | Client Component | Stepper 4 pasos |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardHeader.tsx` | Client Component | Header wizard con X |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaWizardFooter.tsx` | Client Component | Footer con navegacion |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/PromoProgramaAbandonDialog.tsx` | Client Component | Dialog abandono wizard |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/DatosBasicosStep.tsx` | Client Component | Formulario paso 1 |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/ComisionesStep.tsx` | Client Component | Formulario paso 2 |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/TareasStep.tsx` | Client Component | Gestor lista tareas paso 3 |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/steps/RevisarPublicarStep.tsx` | Client Component | Resumen paso 4 |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/PromoTareaCard.tsx` | Client Component | Card de tarea en lista |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/PromoTareaForm.tsx` | Client Component | Formulario inline de tarea |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/tareas/EliminarTareaDialog.tsx` | Client Component | Dialog eliminar tarea |

### Componentes - Detalle

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaHeader.tsx` | Client Component | Cabecera con acciones |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaKpiCards.tsx` | Server Component | Grid KPI cards |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaInfoTab.tsx` | Server Component | Tab info general |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaTareasTab.tsx` | Client Component | Tab tareas |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaPromotoresTab.tsx` | Server Component | Tab promotores |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/PromoProgramaResumenTab.tsx` | Server Component | Tab resumen |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/DesactivarPromoProgramaDialog.tsx` | Client Component | Dialog desactivar |

### Hooks y Services

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/services/promo-programa.service.ts` | Service | API calls para programas |
| `src/admin/src/hooks/use-promo-programas.ts` | Query Hooks | useMisProgramas + usePromoPrograma |
| `src/admin/src/hooks/use-promo-programas-mutations.ts` | Mutation Hooks | create + update + desactivar |
| `src/admin/src/hooks/use-promo-programa-wizard-state.ts` | Custom Hook | useReducer estado wizard |

### Tests

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/admin/src/services/__tests__/promo-programa.service.test.ts` | Test | Service unit tests |
| `src/admin/src/hooks/__tests__/use-promo-programas.test.ts` | Test | Query hooks tests |
| `src/admin/src/hooks/__tests__/use-promo-programas-mutations.test.ts` | Test | Mutation hooks tests |
| `src/admin/src/hooks/__tests__/use-promo-programa-wizard-state.test.ts` | Test | Reducer tests |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/__tests__/PromoProgramaCard.test.tsx` | Test | Card unit test |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/list/__tests__/PromoProgramaStatusBadge.test.tsx` | Test | Badge unit test |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/__tests__/PromoProgramaWizardStepper.test.tsx` | Test | Stepper unit test |
| `src/admin/src/app/(dashboard)/crowdpromotion/components/wizard/__tests__/PromoTareaForm.test.tsx` | Test | Formulario tarea test |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/PromoProgramaHeader.test.tsx` | Test | Header test |
| `src/admin/src/app/(dashboard)/crowdpromotion/programas/[id]/components/__tests__/DesactivarPromoProgramaDialog.test.tsx` | Test | Dialog test |

---

## 11. Consideraciones de Implementacion

### 11.1 Schemas por Paso vs Schema Global

Los pasos del wizard validan contra schemas parciales del schema completo:

| Paso | Schema de validacion |
|------|---------------------|
| Paso 1 | `promoProgramaStep1Schema` (subset de `createPromoProgramaSchema`) |
| Paso 2 | `promoProgramaStep2Schema` (subset con el refine de comision) |
| Paso 3 | Cada tarea individual valida con `createPromoTareaSchema` al guardarla |
| Paso 4 | No hay validacion adicional; los datos ya fueron validados por pasos |

Los schemas por paso se definen localmente en cada step component (igual que `basicInfoStepSchema` en el wizard de campanias), no se exportan desde shared.

### 11.2 Modo Edit - Mapeo PromoProgramaDetail a initialData

Al cargar el detalle para edicion, los campos del `PromoProgramaDetail` deben mapearse a `Partial<CreatePromoProgramaFormData>`:

```typescript
// En PromoProgramaWizardContainer, modo edit:
const initialData: Partial<CreatePromoProgramaFormData> = {
    titulo: detail.titulo,
    descripcion: detail.descripcion ?? '',
    tipoPromoId: detail.tipoPromoId,
    campaniaCrowdfundingId: detail.campaniaCrowdfundingId ?? '',
    proyectoArtisticoId: detail.proyectoArtisticoId ?? '',
    urlLanding: detail.urlLanding ?? '',
    codigoTrackingBase: detail.codigoTrackingBase ?? '',
    monedaId: detail.monedaId,
    importeComisionPorcentaje: detail.importeComisionPorcentaje ?? undefined,
    importeComisionFija: detail.importeComisionFija ?? undefined,
    fechaInicio: detail.fechaInicio ?? '',
    fechaFin: detail.fechaFin ?? '',
    tareas: detail.tareas.map((t) => ({
        titulo: t.titulo,
        descripcion: t.descripcion ?? '',
        tipoEventoPromoId: t.tipoEventoPromoId,
        tipoRewardId: t.tipoRewardId,
        importeRecompensa: t.importeRecompensa ?? undefined,
        monedaId: t.monedaId ?? undefined,
        puntosRecompensa: t.puntosRecompensa ?? undefined,
        urlInstrucciones: t.urlInstrucciones ?? '',
        esRepetible: t.esRepetible,
        maxRepeticiones: t.maxRepeticiones ?? undefined,
        fechaInicio: t.fechaInicio ?? '',
        fechaFin: t.fechaFin ?? '',
    })),
}
```

En modo edit, el `PromoProgramaWizardContainer` debe esperar a que `usePromoPrograma` termine antes de renderizar los pasos. Mostrar un skeleton mientras `isLoading`.

### 11.3 Selects de Maestras (MVP: constantes)

Segun el `contracts-plan.md` (nota 9), los selects de `tipoPromoId`, `tipoEventoPromoId` y `tipoRewardId` usan las constantes de dominio en MVP sin llamadas a API.

```typescript
// DatosBasicosStep: Select de tipo de programa
Object.entries(TIPO_PROMO_LABELS).map(([id, label]) => (
    <SelectItem key={id} value={id}>{label}</SelectItem>
))
```

Para `campaniaCrowdfundingId` y `proyectoArtisticoId`, en MVP se implementan como inputs de texto (UUID libre) con el hint "UUID de la campana". Esto evita la dependencia de endpoints adicionales no definidos en esta feature.

### 11.4 Wizard - Comunicacion entre Footer y Steps

Los steps exponen sus formularios con un `id` de formulario que el footer puede triggear via boton de tipo `submit`:

```typescript
// En DatosBasicosStep:
<form id="paso-1-form" onSubmit={handleSubmit(onNext)}>

// En PromoProgramaWizardFooter:
<Button type="submit" form={`paso-${pasoActual}-form`}>
    Siguiente
</Button>
```

El paso 3 (TareasStep) y paso 4 (RevisarPublicarStep) no tienen form principal. El footer del paso 3 llama directamente `onSiguiente` (ya que las tareas se validan individualmente al guardarlas). El footer del paso 4 dispara el submit final del wizard.

### 11.5 Invalidacion de Queries

Despues de crear o actualizar un programa, invalidar ambas queries para que el listado y el detalle se actualicen:

```typescript
// onSuccess de create:
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.programas.mis })

// onSuccess de update:
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.programas.byId(id) })
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.programas.mis })

// onSuccess de desactivar:
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.programas.byId(id) })
queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.programas.mis })
```

### 11.6 Accesibilidad del Stepper

El stepper sigue el patron ARIA del wizard de campanias:
- `role="progressbar"` en el contenedor del stepper
- `aria-current="step"` en el paso activo
- `aria-label` descriptivo en el contenedor

---

## 12. Orden de Implementacion

El orden minimiza dependencias durante el desarrollo:

### Fase 1: Base (prereqs)
1. Verificar que los archivos `@shared` del contracts-plan.md esten implementados
2. `promo-programa.service.ts` - sin dependencias de componentes
3. `use-promo-programas.ts` - depende del service
4. `use-promo-programas-mutations.ts` - depende del service
5. `use-promo-programa-wizard-state.ts` - independiente

### Fase 2: Listado (pantalla mas simple)
6. `PromoProgramaStatusBadge.tsx` - componente puro
7. `PromoProgramaEmptyState.tsx` - componente puro
8. `PromoProgramaFilters.tsx` - estado local solo
9. `PromoProgramaCard.tsx` - depende de Badge y DropdownMenu
10. `PromoProgramaListClient.tsx` - compone los anteriores + hooks
11. `programas/page.tsx` - server shell del listado

### Fase 3: Detalle (consume API existente)
12. `PromoProgramaKpiCards.tsx`
13. `PromoProgramaInfoTab.tsx`
14. `PromoProgramaPromotoresTab.tsx`
15. `PromoProgramaResumenTab.tsx`
16. `PromoProgramaTareasTab.tsx`
17. `DesactivarPromoProgramaDialog.tsx` - depende de mutations hook
18. `PromoProgramaHeader.tsx`
19. `PromoProgramaDetailClient.tsx` - compone todos los del detalle
20. `programas/[id]/page.tsx` - server shell del detalle

### Fase 4: Wizard Crear (el mas complejo)
21. `PromoProgramaWizardStepper.tsx` - componente puro
22. `PromoProgramaWizardHeader.tsx` - componente puro
23. `PromoProgramaWizardFooter.tsx` - componente puro
24. `PromoProgramaAbandonDialog.tsx` - componente puro
25. `EliminarTareaDialog.tsx` - componente puro
26. `PromoTareaForm.tsx` - depende de Zod schema de tarea
27. `PromoTareaCard.tsx` - depende de PromoTareaForm para el tooltip
28. `DatosBasicosStep.tsx` - formulario paso 1
29. `ComisionesStep.tsx` - formulario paso 2
30. `TareasStep.tsx` - compone PromoTareaCard + PromoTareaForm
31. `RevisarPublicarStep.tsx` - vista de solo lectura
32. `PromoProgramaWizardContainer.tsx` - orquestador
33. `programas/nuevo/page.tsx` - server shell del wizard

### Fase 5: Editar (reutiliza wizard)
34. Agregar logica modo `edit` a `PromoProgramaWizardContainer`
35. `programas/[id]/editar/page.tsx` - server shell del editar

### Fase 6: Tests
36. Tests de service
37. Tests de hooks (query y mutations)
38. Tests de reducer del wizard
39. Tests de componentes clave (Card, Badge, Stepper, PromoTareaForm, DesactivarDialog, Header)

---

## 13. Checklist

- [ ] Shared plan implementado (types, schemas, constants, error-messages)
- [ ] **Service**
    - [ ] `promo-programa.service.ts` con los 5 metodos
    - [ ] Patron de error handling igual que `promotorService`
    - [ ] getMisProgramas con query params via URLSearchParams
- [ ] **Hooks**
    - [ ] `useMisProgramas` con filtros y paginacion
    - [ ] `usePromoPrograma` con enabled y retry false
    - [ ] `useCreatePromoPrograma` con invalidacion
    - [ ] `useUpdatePromoPrograma` con invalidacion
    - [ ] `useDesactivarPromoPrograma` con toast y invalidacion
    - [ ] `usePromoProgramaWizardState` con useReducer + localStorage
- [ ] **Listado**
    - [ ] `PromoProgramaStatusBadge` con badge activo/inactivo
    - [ ] `PromoProgramaCard` con menu kebab y metricas
    - [ ] `PromoProgramaFilters` con debounce en busqueda
    - [ ] `PromoProgramaEmptyState` con 2 variantes
    - [ ] `PromoProgramaListClient` con estados loading/error/empty/data
    - [ ] Paginacion server-side funcional
- [ ] **Wizard**
    - [ ] `PromoProgramaWizardStepper` con 4 pasos y estilos propios
    - [ ] `PromoProgramaWizardHeader` con X y dialog de abandono
    - [ ] `PromoProgramaWizardFooter` con boton adaptado al paso y modo
    - [ ] `DatosBasicosStep` con todos los campos del Paso 1
    - [ ] `ComisionesStep` con vista previa en tiempo real
    - [ ] `TareasStep` con lista inline y formulario expandible
    - [ ] `PromoTareaForm` con visibilidad condicional de campos de recompensa
    - [ ] `RevisarPublicarStep` con resumen completo y botones de edicion
    - [ ] Submit final mapea formData → CreatePromoProgramaRequest correctamente
    - [ ] Strings vacios convertidos a undefined antes del POST
    - [ ] Toast success + redirect a detalle en exito
    - [ ] Toast error con mensaje de `getPromoProgramaErrorMessage` en fallo
- [ ] **Detalle**
    - [ ] `PromoProgramaHeader` con botones Editar y Desactivar/Reactivar
    - [ ] `PromoProgramaKpiCards` con 4 cards del `resumen`
    - [ ] 4 tabs con contenido correcto
    - [ ] `DesactivarPromoProgramaDialog` con confirmacion
    - [ ] Banner de "Programa desactivado" si `esActivo=false`
- [ ] **Editar**
    - [ ] Wizard pre-rellenado con datos del detalle
    - [ ] Banner de aviso si programa tiene promotores inscritos
    - [ ] Tareas con `puedeEliminar=false` si `completadosPorPromotores > 0`
    - [ ] Boton footer dice "Guardar cambios" en modo edit
- [ ] **Calidad**
    - [ ] Todos los componentes usan shadcn/ui
    - [ ] Tipos importados de `@shared/types`, no duplicados
    - [ ] Sin uso de `any` en TypeScript
    - [ ] Esquemas Zod desde `@shared/schemas` para validacion final del wizard
    - [ ] Schemas locales por paso para validacion incremental
    - [ ] Tests clave implementados (service, hooks, reducer, components criticos)
