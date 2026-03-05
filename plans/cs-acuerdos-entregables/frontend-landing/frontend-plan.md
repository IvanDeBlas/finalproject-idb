# Plan Frontend: Acuerdos, Milestones y Entregables (Landing)

**Fecha:** 2026-02-18
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Target:** src/web (Vite + React 18, puerto 3000)

---

## 1. Resumen

- Screens: 1 nueva pagina (`AcuerdoDetallePage`)
- Dialogs de accion: 7 dialogs modales
- Componentes de presentacion: 11 componentes
- Hooks: 11 hooks (1 query + 10 mutations)
- Services (api classes): 3 clases (`acuerdo.api.ts`, `milestone.api.ts`, `entregable.api.ts`)
- Rutas nuevas: 1 ruta protegida en `DashboardLayout`

Esta feature NO requiere nueva pagina de listado de acuerdos (out of scope en US-CS-04). El acceso al acuerdo se hace desde el detalle de la necesidad (US-CS-03) y desde mis propuestas. La pagina de detalle del acuerdo es el unico entry point nuevo.

---

## 2. Estructura de Carpetas

```
src/web/src/features/crowdsourcing/
│
├── domain/
│   └── types.ts                         # Agregar re-exports de types de acuerdos
│
├── application/
│   ├── hooks/
│   │   ├── useAcuerdo.ts                # Query hook - GET detalle acuerdo
│   │   ├── useAceptarPropuesta.ts       # Mutation hook - POST aceptar propuesta
│   │   ├── useRechazarPropuesta.ts      # Mutation hook - PATCH rechazar propuesta
│   │   ├── useCreateMilestone.ts        # Mutation hook - POST crear milestone
│   │   ├── useUpdateMilestone.ts        # Mutation hook - PUT editar milestone
│   │   ├── useDeleteMilestone.ts        # Mutation hook - DELETE eliminar milestone
│   │   ├── useCreateEntregable.ts       # Mutation hook - POST subir entregable
│   │   ├── useAprobarEntregable.ts      # Mutation hook - PATCH aprobar entregable
│   │   ├── useRechazarEntregable.ts     # Mutation hook - PATCH rechazar entregable
│   │   ├── useCompletarAcuerdo.ts       # Mutation hook - PATCH completar acuerdo
│   │   └── useCancelarAcuerdo.ts        # Mutation hook - PATCH cancelar acuerdo
│   └── index.ts                         # Agregar exports de hooks nuevos
│
├── infrastructure/
│   ├── api/
│   │   ├── acuerdo.api.ts               # API class para acuerdos
│   │   ├── milestone.api.ts             # API class para milestones
│   │   └── entregable.api.ts            # API class para entregables
│   └── index.ts                         # Agregar exports de api classes nuevas
│
├── presentation/
│   ├── components/
│   │   ├── AcuerdoCabecera.tsx          # Cabecera del acuerdo (estado, partes, importe)
│   │   ├── MilestonesSection.tsx        # Seccion de milestones con barra de progreso
│   │   ├── MilestoneCard.tsx            # Card de milestone con entregables anidados
│   │   ├── EntregableItem.tsx           # Item de entregable con acciones condicionales
│   │   ├── EntregablesSinMilestone.tsx  # Seccion de entregables sin milestone asignado
│   │   ├── ImporteAsignadoBar.tsx       # Barra de progreso de importe asignado
│   │   ├── AcuerdoTimeline.tsx          # Lista de eventos del timeline
│   │   ├── ConversacionLink.tsx         # Boton/link al chat del acuerdo
│   │   ├── EstadoAcuerdoBadge.tsx       # Badge de estado del acuerdo
│   │   ├── EstadoEntregableBadge.tsx    # Badge de estado del entregable
│   │   ├── MilestoneFormDialog.tsx      # Dialog para crear/editar milestone (RHF+Zod)
│   │   ├── SubirEntregableDialog.tsx    # Dialog para subir entregable (profesional)
│   │   ├── AprobarEntregableDialog.tsx  # Dialog con comentario opcional (artista)
│   │   ├── RechazarEntregableDialog.tsx # Dialog con comentario obligatorio (artista)
│   │   ├── CompletarAcuerdoDialog.tsx   # Dialog resumen + confirmacion (artista)
│   │   ├── CancelarAcuerdoDialog.tsx    # Dialog advertencia + motivo (ambos roles)
│   │   ├── AceptarPropuestaDialog.tsx   # Dialog formulario de aceptacion (artista)
│   │   └── RechazarPropuestaDialog.tsx  # Dialog motivo de rechazo (artista)
│   ├── pages/
│   │   └── AcuerdoDetallePage.tsx       # Pagina principal del acuerdo
│   ├── components/
│   │   └── index.ts                     # Agregar exports de componentes nuevos
│   └── pages/
│       └── index.ts                     # Agregar export de AcuerdoDetallePage
│
└── __mocks__/
    └── acuerdo.mock.ts                  # Datos mock para tests
```

**Nota sobre AceptarPropuestaDialog y RechazarPropuestaDialog:** Estos dos dialogs se consumen desde la pagina de detalle de necesidad (`NecesidadDetallePage.tsx`), no desde `AcuerdoDetallePage`. Se colocan en la carpeta `components/` del mismo feature para mantener cohesion. La redireccion post-aceptacion lleva al usuario a `AcuerdoDetallePage`.

---

## 3. Componentes

### 3.1 AcuerdoDetallePage

**Archivo:** `src/web/src/features/crowdsourcing/presentation/pages/AcuerdoDetallePage.tsx`

**Props:** Ninguna (lee `id` de `useParams`)

**Estado Local:**
| Estado | Tipo | Descripcion |
|--------|------|-------------|
| `milestoneDialogOpen` | `boolean` | Controla apertura del dialog de crear milestone |
| `milestoneToEdit` | `Milestone \| null` | Milestone seleccionado para editar (null = modo crear) |
| `entregableDialogOpen` | `boolean` | Controla apertura del dialog de subir entregable |
| `entregableToAprobar` | `Entregable \| null` | Entregable seleccionado para aprobar |
| `entregableToRechazar` | `Entregable \| null` | Entregable seleccionado para rechazar |
| `completarDialogOpen` | `boolean` | Controla apertura del dialog de completar acuerdo |
| `cancelarDialogOpen` | `boolean` | Controla apertura del dialog de cancelar acuerdo |

**Responsabilidad:**
Pagina contenedora de todos los componentes del acuerdo. Lee el `id` de `useParams`, usa `useAcuerdo(id)` para obtener los datos, y pasa estado de dialogs a los componentes hijos. Maneja el renderizado condicional de secciones de accion segun `acuerdo.miRol` y `acuerdo.estadoAcuerdoId`. Muestra skeleton durante carga, mensaje de acceso denegado en 403 y error generico en 404.

**Layout:**
```
[Breadcrumb: Volver a la necesidad]
[AcuerdoCabecera]                     <- cabecera completa
[grid: main content + sticky sidebar]
  main:
    [MilestonesSection]               <- con boton "+ Agregar milestone" si Artista+Activo
    [EntregablesSinMilestone]         <- entregables sin milestone asignado
    [AcuerdoTimeline]
  sidebar (sticky):
    [ConversacionLink]
    [Acciones del acuerdo]            <- Completar / Cancelar segun rol y estado
```

**Dependencias:**
- Hooks: `useAcuerdo`
- Componentes: todos los listados en la estructura

---

### 3.2 AcuerdoCabecera

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AcuerdoCabecera.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdo` | `Acuerdo` | Si | Datos completos del acuerdo |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Card`, `CardContent`, `Badge`, `Separator`
- Componentes propios: `EstadoAcuerdoBadge`, `ImporteAsignadoBar`
- Iconos: `Calendar`, `DollarSign`, `Users`

**Responsabilidad:**
Muestra la cabecera del acuerdo con: titulo interno, `EstadoAcuerdoBadge`, nombres de las partes (artista y profesional), importe total pactado con moneda, fechas (inicio, fin prevista, fin real si existe), y el indicador `ImporteAsignadoBar` con el porcentaje asignado a milestones. Tambien muestra el link a la necesidad de origen.

---

### 3.3 EstadoAcuerdoBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EstadoAcuerdoBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoId` | `number` | Si | ID numerico del estado (1=Activo, 2=Completado, 3=Cancelado) |
| `estadoNombre` | `string` | Si | Texto del estado para mostrar |
| `className` | `string` | No | Clases adicionales de Tailwind |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Badge`
- Constantes: `ESTADO_ACUERDO` desde `@shared/constants`

**Responsabilidad:**
Renderiza un `Badge` de shadcn/ui con el color correcto segun el estado. Activo=blue (`bg-blue-900/30 text-blue-300 border-blue-700`), Completado=green, Cancelado=gray. Centraliza la logica de colores de badge para el estado del acuerdo.

---

### 3.4 EstadoEntregableBadge

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EstadoEntregableBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `estadoId` | `number` | Si | ID numerico del estado (1=Entregado, 2=Aprobado, 3=Rechazado) |
| `estadoNombre` | `string` | Si | Texto del estado para mostrar |
| `className` | `string` | No | Clases adicionales de Tailwind |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Badge`
- Constantes: `ESTADO_ENTREGABLE` desde `@shared/constants`

**Responsabilidad:**
Renderiza un `Badge` con el color correcto segun el estado del entregable. Entregado=amber/yellow (`bg-amber-900/30 text-amber-300`), Aprobado=green, Rechazado=red.

---

### 3.5 ImporteAsignadoBar

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ImporteAsignadoBar.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `importeAsignado` | `number` | Si | Suma de importes de milestones existentes |
| `importeTotal` | `number` | Si | Importe total pactado del acuerdo |
| `porcentaje` | `number` | Si | Porcentaje ya calculado por el backend |
| `monedaNombre` | `string` | Si | Nombre de la moneda (e.g. "EUR") |
| `importeNuevo` | `number` | No | Importe adicional en preview (para uso en formulario de milestone) |
| `className` | `string` | No | Clases adicionales |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Progress` (shadcn) o barra personalizada con Tailwind

**Responsabilidad:**
Renderiza la barra de progreso visual con el texto "Asignado: X de Y EUR (Z%)". Cuando se le pasa `importeNuevo`, muestra un preview del nuevo porcentaje calculado en tiempo real (sin llamadas al backend). Si `importeAsignado + importeNuevo > importeTotal`, aplica estilos de error (barra roja, texto rojo). Se usa tanto en la seccion de milestones como dentro del `MilestoneFormDialog`.

---

### 3.6 MilestonesSection

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MilestonesSection.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `milestones` | `Milestone[]` | Si | Lista de milestones del acuerdo |
| `importeTotal` | `number` | Si | Importe total pactado (para calculo de progreso) |
| `importeAsignado` | `number` | Si | Importe ya asignado a milestones |
| `porcentajeAsignado` | `number` | Si | Porcentaje asignado calculado por backend |
| `monedaNombre` | `string` | Si | Moneda del acuerdo |
| `miRol` | `RolAcuerdo` | Si | Rol del usuario autenticado |
| `estadoAcuerdoId` | `number` | Si | Estado del acuerdo para controlar acciones |
| `onAgregarMilestone` | `() => void` | Si | Callback al hacer click en "+ Agregar milestone" |
| `onEditarMilestone` | `(milestone: Milestone) => void` | Si | Callback al editar |
| `onEliminarMilestone` | `(milestoneId: string) => void` | Si | Callback al eliminar |
| `onAprobarEntregable` | `(entregable: Entregable) => void` | Si | Sube hacia la pagina |
| `onRechazarEntregable` | `(entregable: Entregable) => void` | Si | Sube hacia la pagina |
| `onSubirEntregable` | `(milestoneId: string) => void` | Si | Callback al subir entregable en milestone |

**Estado Local:** Ninguno (estado de dialogs centralizado en la pagina)

**Dependencias:**
- Componentes propios: `MilestoneCard`, `ImporteAsignadoBar`
- Componentes UI: `Card`, `CardHeader`, `CardTitle`, `Button`
- Iconos: `Plus`, `Target`

**Responsabilidad:**
Renderiza la seccion completa de milestones. Muestra la barra de progreso global `ImporteAsignadoBar` al inicio. Si no hay milestones, muestra empty state. Lista los `MilestoneCard` ordenados por `orden`. Si `miRol == 'Artista'` y `estadoAcuerdoId == ESTADO_ACUERDO.ACTIVO`, muestra el boton "+ Agregar milestone".

---

### 3.7 MilestoneCard

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MilestoneCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `milestone` | `Milestone` | Si | Datos del milestone con entregables anidados |
| `importeTotal` | `number` | Si | Importe total del acuerdo (para porcentaje) |
| `monedaNombre` | `string` | Si | Moneda del acuerdo |
| `miRol` | `RolAcuerdo` | Si | Rol del usuario |
| `estadoAcuerdoId` | `number` | Si | Estado del acuerdo |
| `onEditar` | `(milestone: Milestone) => void` | No | Solo visible si Artista, Activo, no completado |
| `onEliminar` | `(milestoneId: string) => void` | No | Solo visible si Artista, Activo, sin entregables, no completado |
| `onAprobarEntregable` | `(entregable: Entregable) => void` | Si | Sube el evento |
| `onRechazarEntregable` | `(entregable: Entregable) => void` | Si | Sube el evento |
| `onSubirEntregable` | `(milestoneId: string) => void` | Si | Sube el evento |
| `todosAprobadosSugerencia` | `boolean` | No | Si true, muestra sugerencia de completar milestone |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes propios: `EntregableItem`
- Componentes UI: `Card`, `CardContent`, `Badge`, `Button`, `Separator`
- Iconos: `CheckCircle2`, `Clock`, `Pencil`, `Trash2`

**Responsabilidad:**
Renderiza un card por milestone con: titulo, descripcion (colapsable si larga), importe parcial y porcentaje, fecha limite (con color segun urgencia), badge de estado (completado/pendiente). Lista los `EntregableItem` anidados. Muestra botones de editar/eliminar solo cuando corresponde segun rol y estado. Si `todosAprobadosSugerencia == true`, muestra una alerta suave sugiriendo marcar el milestone como completado.

---

### 3.8 EntregableItem

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EntregableItem.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `entregable` | `Entregable` | Si | Datos del entregable |
| `miRol` | `RolAcuerdo` | Si | Rol del usuario autenticado |
| `estadoAcuerdoId` | `number` | Si | Estado del acuerdo para controlar acciones |
| `onAprobar` | `(entregable: Entregable) => void` | No | Solo visible si Artista+Activo+estado=Entregado |
| `onRechazar` | `(entregable: Entregable) => void` | No | Solo visible si Artista+Activo+estado=Entregado |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes propios: `EstadoEntregableBadge`
- Componentes UI: `Button`, `Separator`
- Iconos: `ExternalLink`, `CheckCircle`, `XCircle`, `Calendar`

**Responsabilidad:**
Renderiza un item de entregable con: titulo, descripcion (opcional), link externo al `urlRecurso` (si existe, abre en nueva tab), `EstadoEntregableBadge`, fecha de creacion, fecha de aprobacion (si existe), comentario de aprobacion/rechazo (si existe). Botones de "Aprobar" y "Rechazar" visibles solo si `miRol == 'Artista'` y `estadoAcuerdoId == ESTADO_ACUERDO.ACTIVO` y `estadoEntregableId == ESTADO_ENTREGABLE.ENTREGADO`.

---

### 3.9 EntregablesSinMilestone

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/EntregablesSinMilestone.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `entregables` | `Entregable[]` | Si | Lista de entregables sin milestone asignado |
| `miRol` | `RolAcuerdo` | Si | Rol del usuario |
| `estadoAcuerdoId` | `number` | Si | Estado del acuerdo |
| `onAprobarEntregable` | `(entregable: Entregable) => void` | Si | Callback al aprobar |
| `onRechazarEntregable` | `(entregable: Entregable) => void` | Si | Callback al rechazar |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes propios: `EntregableItem`
- Componentes UI: `Card`, `CardHeader`, `CardTitle`

**Responsabilidad:**
Renderiza la seccion de entregables que no tienen milestone asociado. Solo se renderiza si `entregables.length > 0`. Agrupa estos entregables al final del contenido principal, despues de la seccion de milestones.

---

### 3.10 AcuerdoTimeline

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AcuerdoTimeline.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `timeline` | `AcuerdoTimelineEvento[]` | Si | Lista de hasta 20 eventos del acuerdo |
| `className` | `string` | No | Clases adicionales |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Card`, `CardHeader`, `CardTitle`
- Iconos: `Clock`

**Responsabilidad:**
Renderiza la lista cronologica de eventos del acuerdo (acuerdo creado, milestone agregado, entregable subido, etc.). Muestra fecha formateada y actor para cada evento. Si `timeline.length == 0`, muestra empty state.

---

### 3.11 ConversacionLink

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/ConversacionLink.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `conversacionId` | `string \| undefined` | Si | ID de la conversacion vinculada |
| `className` | `string` | No | Clases adicionales |

**Estado Local:** Ninguno

**Dependencias:**
- Componentes UI: `Button`
- Iconos: `MessageCircle`

**Responsabilidad:**
Renderiza un boton/link para ir al chat de la conversacion vinculada al acuerdo. Si `conversacionId` es undefined, muestra el boton deshabilitado con tooltip explicativo. El link navega a la ruta del modulo de conversaciones (a definir en feature futura; por ahora placeholder link).

---

### 3.12 MilestoneFormDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/MilestoneFormDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdoId` | `string` | Si | ID del acuerdo al que pertenece el milestone |
| `importeTotal` | `number` | Si | Importe total pactado del acuerdo |
| `importeYaAsignado` | `number` | Si | Suma de importes de milestones existentes |
| `monedaNombre` | `string` | Si | Nombre de la moneda |
| `fechaInicioAcuerdo` | `string` | Si | Fecha de inicio del acuerdo (para validacion de fechaLimite) |
| `milestoneToEdit` | `Milestone \| null` | No | Si se pasa, el dialog esta en modo edicion |
| `isOpen` | `boolean` | Si | Controla visibilidad del dialog |
| `onClose` | `() => void` | Si | Callback al cerrar el dialog |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form

**Dependencias:**
- Hooks: `useCreateMilestone`, `useUpdateMilestone`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogFooter`, `Input`, `Textarea`, `Label`, `Button`
- Componentes propios: `ImporteAsignadoBar`
- Schemas: `createMilestoneSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `Loader2`

**Responsabilidad:**
Dialog reutilizable para crear y editar milestones. En modo crear: campos vacios, usa `useCreateMilestone`. En modo editar: campos pre-rellenados con `milestoneToEdit`, usa `useUpdateMilestone`. Muestra `ImporteAsignadoBar` con preview en tiempo real usando `watch('importeParcial')`. Si el importe nuevo supera el total, muestra error inmediato en el campo y deshabilita el submit. El campo `fechaLimite` es opcional pero si se ingresa, se valida que sea >= `fechaInicioAcuerdo` (validacion en componente con contexto, no en schema puro). Al submit exitoso, llama a `onClose()`.

---

### 3.13 SubirEntregableDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/SubirEntregableDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdoId` | `string` | Si | ID del acuerdo |
| `milestones` | `Milestone[]` | Si | Lista de milestones para el select opcional |
| `preselectedMilestoneId` | `string` | No | Si se abre desde un MilestoneCard, pre-selecciona |
| `isOpen` | `boolean` | Si | Controla visibilidad del dialog |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form

**Dependencias:**
- Hooks: `useCreateEntregable`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Input`, `Textarea`, `Label`, `Button`, `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`
- Schemas: `createEntregableSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `Upload`, `Loader2`, `Info`

**Responsabilidad:**
Dialog para que el profesional suba un entregable. Campos: titulo (required), descripcion (optional), URL del recurso (optional, validada como URL), select de milestone (optional, lista los milestones del acuerdo + opcion "Sin milestone"). Muestra nota informativa sobre URLs aceptadas (Drive, Dropbox, WeTransfer). Al submit exitoso, llama a `onClose()`.

---

### 3.14 AprobarEntregableDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AprobarEntregableDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `entregable` | `Entregable \| null` | Si | Entregable a aprobar (null = cerrado) |
| `acuerdoId` | `string` | Si | ID del acuerdo (para invalidar query) |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form (solo campo comentario)

**Dependencias:**
- Hooks: `useAprobarEntregable`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Textarea`, `Label`, `Button`
- Schemas: `aprobarEntregableSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `CheckCircle`, `Loader2`

**Responsabilidad:**
Dialog de confirmacion con campo de comentario opcional (max 500 chars). Muestra el titulo del entregable en la descripcion. Al confirmar, llama a `useAprobarEntregable`. Si la respuesta incluye `todosAprobadosEnMilestone == true`, muestra un toast adicional sugiriendo marcar el milestone como completado. Al exito, llama a `onClose()`.

---

### 3.15 RechazarEntregableDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RechazarEntregableDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `entregable` | `Entregable \| null` | Si | Entregable a rechazar |
| `acuerdoId` | `string` | Si | ID del acuerdo |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form

**Dependencias:**
- Hooks: `useRechazarEntregable`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Textarea`, `Label`, `Button`
- Schemas: `rechazarEntregableSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `XCircle`, `Loader2`

**Responsabilidad:**
Dialog con campo de comentario obligatorio (min 10, max 500 chars). Muestra contador de caracteres para dar feedback al usuario. Boton "Rechazar" deshabilitado hasta que el comentario sea valido. Al exito, llama a `onClose()`.

---

### 3.16 CompletarAcuerdoDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/CompletarAcuerdoDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdo` | `Acuerdo` | Si | Datos del acuerdo para mostrar resumen |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:** Ninguno

**Dependencias:**
- Hooks: `useCompletarAcuerdo`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Button`, `Separator`
- Iconos: `CheckCircle2`, `AlertTriangle`, `Loader2`

**Responsabilidad:**
Dialog de confirmacion con resumen del acuerdo: milestones completados vs total, entregables aprobados vs total. Si hay entregables en estado `Entregado` (pendientes de revision), muestra aviso en amarillo (no bloqueante). Boton "Completar acuerdo" siempre habilitado pero el aviso informa al artista. Al exito, la query del acuerdo se invalida y el componente padre lee el nuevo estado.

---

### 3.17 CancelarAcuerdoDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/CancelarAcuerdoDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `acuerdoId` | `string` | Si | ID del acuerdo a cancelar |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form (solo campo motivo)

**Dependencias:**
- Hooks: `useCancelarAcuerdo`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Textarea`, `Label`, `Button`
- Schemas: `cancelarAcuerdoSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `AlertTriangle`, `Loader2`

**Responsabilidad:**
Dialog destructivo con advertencia explicita: "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas." en un bloque de alerta rojo. Campo de motivo obligatorio (min 20, max 1000 chars) con contador de caracteres. Boton "Cancelar acuerdo" en variante destructiva, deshabilitado hasta que el formulario sea valido.

---

### 3.18 AceptarPropuestaDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/AceptarPropuestaDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `propuesta` | `PropuestaCrowdsourcing \| null` | Si | Propuesta a aceptar (null = cerrado) |
| `necesidadTitulo` | `string` | Si | Titulo de la necesidad (default para tituloInterno) |
| `diasEstimados` | `number \| undefined` | No | Para calcular fechaFinPrevista default |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Nota sobre tipos:** El tipo exacto de `propuesta` depende del tipo definido en US-CS-03 para la propuesta. Usar el tipo de la lista de propuestas de la necesidad.

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form

**Dependencias:**
- Hooks: `useAceptarPropuesta`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Input`, `Label`, `Button`
- Schemas: `aceptarPropuestaSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `Info`, `Loader2`, `DollarSign`

**Responsabilidad:**
Dialog que se abre desde la vista de propuestas de una necesidad. Muestra resumen de la propuesta (profesional, precio, mensaje). Campos editables: `tituloInterno` (default: `necesidadTitulo`), `fechaInicio` (default: hoy), `fechaFinPrevista` (default: hoy + `diasEstimados`). Muestra nota informativa de que las demas propuestas seran rechazadas automaticamente. Al submit exitoso, redirige a `/crowdsourcing/acuerdos/{acuerdoId}` usando `useNavigate`.

---

### 3.19 RechazarPropuestaDialog

**Archivo:** `src/web/src/features/crowdsourcing/presentation/components/RechazarPropuestaDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `propuestaId` | `string \| null` | Si | ID de la propuesta a rechazar |
| `isOpen` | `boolean` | Si | Controla visibilidad |
| `onClose` | `() => void` | Si | Callback al cerrar |

**Estado Local:**
- Control del formulario con `useForm` de React Hook Form (campo motivo opcional)

**Dependencias:**
- Hooks: `useRechazarPropuesta`
- Componentes UI: `Dialog`, `DialogContent`, `DialogHeader`, `DialogTitle`, `DialogDescription`, `DialogFooter`, `Textarea`, `Label`, `Button`
- Schemas: `rechazarPropuestaSchema` desde `@shared/schemas/crowdsourcing.schema`
- Iconos: `XCircle`, `Loader2`

**Responsabilidad:**
Dialog para rechazar una propuesta individual. Campo de motivo opcional (max 500 chars). Nota visible: "El profesional no podra ver el motivo de rechazo". Al exito, llama a `onClose()`.

---

## 4. Hooks

### 4.1 useAcuerdo

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useAcuerdo.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `id` | `string` | ID del acuerdo a cargar |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `Acuerdo \| undefined` | Detalle completo del acuerdo |
| `isLoading` | `boolean` | True durante la primera carga |
| `error` | `Error \| null` | Error si el fetch falla (403, 404, etc.) |
| `refetch` | `() => void` | Para refrescar manualmente |

**Query Key:** `QUERY_KEYS.crowdsourcing.acuerdos.byId(id)`

**Configuracion:**
- `staleTime`: 30_000 ms (30 segundos)
- `enabled`: `!!id`
- `retry`: false (para no reintentar en 403/404)

**Notas:** Este hook es el unico de lectura para la feature. Todos los hooks de mutacion invalidan esta query key al tener exito.

---

### 4.2 useAceptarPropuesta

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useAceptarPropuesta.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `propuestaId` | `string` | ID de la propuesta a aceptar |
| `onSuccess` | `(acuerdoId: string) => void` | Callback post-exito para redirigir |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `AceptarPropuestaRequest` | Datos del formulario de aceptacion |

**Acciones:**
- `mutate(data)` - Llama a `acuerdoApi.aceptarPropuesta(propuestaId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.propuestas.mis`, llama a `onSuccess(result.acuerdoId)` para que el componente padre navegue
- `onError` - Toast de error con mensaje del error code

**Nota:** El toast de exito "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." se muestra desde el componente padre usando el mensaje del response, o se muestra directamente en el `onSuccess` del hook con el texto exacto del contrato.

---

### 4.3 useRechazarPropuesta

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useRechazarPropuesta.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `params` | `{ propuestaId: string; data: RechazarPropuestaRequest }` | ID y datos |

**Acciones:**
- `mutate({ propuestaId, data })` - Llama a `acuerdoApi.rechazarPropuesta(propuestaId, data)`
- `onSuccess` - Invalida query de propuestas de la necesidad correspondiente, toast "Propuesta rechazada", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.4 useCreateMilestone

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCreateMilestone.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo al que pertenece el milestone |
| `onSuccess` | `() => void` | Callback post-exito (para cerrar dialog) |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreateMilestoneRequest` | Datos del formulario |

**Acciones:**
- `mutate(data)` - Llama a `milestoneApi.create(acuerdoId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Milestone creado", llama a `onSuccess()`
- `onError` - Toast con mensaje de error (especialmente el error 4009 de suma excedida)

---

### 4.5 useUpdateMilestone

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useUpdateMilestone.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo |
| `milestoneId` | `string` | ID del milestone a editar |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreateMilestoneRequest` | Datos actualizados (mismo schema que crear) |

**Acciones:**
- `mutate(data)` - Llama a `milestoneApi.update(acuerdoId, milestoneId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Milestone actualizado", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.6 useDeleteMilestone

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useDeleteMilestone.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `milestoneId` | `string` | ID del milestone a eliminar |

**Acciones:**
- `mutate(milestoneId)` - Llama a `milestoneApi.delete(acuerdoId, milestoneId)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Milestone eliminado"
- `onError` - Toast de error (especialmente error 4012 de milestone con entregables)

**Nota:** La confirmacion de eliminacion se hace con un dialog de confirmacion inline en `MilestoneCard` (simple `window.confirm` o un pequeno popover de confirmacion - no un dialog separado complejo dada la simplicidad de la accion).

---

### 4.7 useCreateEntregable

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCreateEntregable.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CreateEntregableRequest` | Datos del formulario |

**Acciones:**
- `mutate(data)` - Llama a `entregableApi.create(acuerdoId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Entregable subido correctamente. El artista sera notificado.", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.8 useAprobarEntregable

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useAprobarEntregable.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo (para invalidar query) |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `params` | `{ entregableId: string; data: AprobarEntregableRequest }` | ID y comentario |

**Acciones:**
- `mutate({ entregableId, data })` - Llama a `entregableApi.aprobar(entregableId, data)`
- `onSuccess(result)` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Entregable aprobado". Si `result.todosAprobadosEnMilestone == true`, muestra un toast adicional: "Todos los entregables de este milestone fueron aprobados. Puedes marcarlo como completado.", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.9 useRechazarEntregable

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useRechazarEntregable.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `params` | `{ entregableId: string; data: RechazarEntregableRequest }` | ID y comentario obligatorio |

**Acciones:**
- `mutate({ entregableId, data })` - Llama a `entregableApi.rechazar(entregableId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Entregable rechazado. El profesional sera notificado.", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.10 useCompletarAcuerdo

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCompletarAcuerdo.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo a completar |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:** Ninguno (PATCH sin body)

**Acciones:**
- `mutate()` - Llama a `acuerdoApi.completar(acuerdoId)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Acuerdo completado. Puedes dejar una valoracion al profesional.", llama a `onSuccess()`
- `onError` - Toast de error

---

### 4.11 useCancelarAcuerdo

**Archivo:** `src/web/src/features/crowdsourcing/application/hooks/useCancelarAcuerdo.ts`

**Tipo:** Mutation Hook

**Parametros del hook:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `acuerdoId` | `string` | ID del acuerdo a cancelar |
| `onSuccess` | `() => void` | Callback post-exito |

**Parametros de `mutate`:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `CancelarAcuerdoRequest` | Motivo de cancelacion (min 20, max 1000) |

**Acciones:**
- `mutate(data)` - Llama a `acuerdoApi.cancelar(acuerdoId, data)`
- `onSuccess` - Invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`, toast "Acuerdo cancelado.", llama a `onSuccess()`
- `onError` - Toast de error

---

## 5. Services (API Classes)

### 5.1 acuerdo.api.ts

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/acuerdo.api.ts`

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getById` | `id: string` | `Promise<Acuerdo>` | `GET API_ROUTES.crowdsourcing.acuerdos.byId(id)` |
| `completar` | `id: string` | `Promise<CompletarAcuerdoResult>` | `PATCH API_ROUTES.crowdsourcing.acuerdos.completar(id)` |
| `cancelar` | `id: string, data: CancelarAcuerdoRequest` | `Promise<CancelarAcuerdoResult>` | `PATCH API_ROUTES.crowdsourcing.acuerdos.cancelar(id)` |
| `aceptarPropuesta` | `propuestaId: string, data: AceptarPropuestaRequest` | `Promise<AceptarPropuestaResult>` | `POST API_ROUTES.crowdsourcing.propuestas.aceptar(propuestaId)` |
| `rechazarPropuesta` | `propuestaId: string, data: RechazarPropuestaRequest` | `Promise<RechazarPropuestaResult>` | `PATCH API_ROUTES.crowdsourcing.propuestas.rechazar(propuestaId)` |

**Patron de implementacion:**
```
class AcuerdoApiService {
    - Usa apiFetch<ServiceResponse<T>> de @/lib/api-client
    - Si !response.data, extrae errorCode de messages[0] y lanza Error(errorCode)
    - Retorna response.data directamente en el happy path
}
export const acuerdoApi = new AcuerdoApiService()
```

**Nota sobre aceptar y rechazar propuesta:** Estos metodos se agrupan en `acuerdo.api.ts` porque son parte del flujo de creacion de acuerdo, aunque los endpoints usan la ruta `/propuestas/`. Esta decision mantiene la cohesion: todo lo relativo a la creacion y gestion del acuerdo esta en el mismo servicio.

---

### 5.2 milestone.api.ts

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/milestone.api.ts`

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `create` | `acuerdoId: string, data: CreateMilestoneRequest` | `Promise<MilestoneCreatedResult>` | `POST API_ROUTES.crowdsourcing.acuerdos.milestones(acuerdoId)` |
| `update` | `acuerdoId: string, milestoneId: string, data: CreateMilestoneRequest` | `Promise<MilestoneCreatedResult>` | `PUT API_ROUTES.crowdsourcing.acuerdos.milestoneById(acuerdoId, milestoneId)` |
| `delete` | `acuerdoId: string, milestoneId: string` | `Promise<void>` | `DELETE API_ROUTES.crowdsourcing.acuerdos.milestoneById(acuerdoId, milestoneId)` |

**Patron de implementacion:** Identico a `acuerdo.api.ts`. Para el DELETE (204 No Content), no intenta parsear `response.data` sino solo verificar que no hay error en `messages`.

---

### 5.3 entregable.api.ts

**Archivo:** `src/web/src/features/crowdsourcing/infrastructure/api/entregable.api.ts`

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `create` | `acuerdoId: string, data: CreateEntregableRequest` | `Promise<EntregableCreatedResult>` | `POST API_ROUTES.crowdsourcing.acuerdos.entregables(acuerdoId)` |
| `aprobar` | `entregableId: string, data: AprobarEntregableRequest` | `Promise<AprobarEntregableResult>` | `PATCH API_ROUTES.crowdsourcing.entregables.aprobar(entregableId)` |
| `rechazar` | `entregableId: string, data: RechazarEntregableRequest` | `Promise<RechazarEntregableResult>` | `PATCH API_ROUTES.crowdsourcing.entregables.rechazar(entregableId)` |

---

## 6. Rutas

### Ruta nueva a agregar en `src/web/src/app/router.tsx`

**Ruta:** `/crowdsourcing/acuerdos/:id`
**Layout:** `DashboardLayout` (requiere autenticacion, ya que solo participantes pueden acceder)
**Componente:** `AcuerdoDetallePage` (lazy loaded)

**Cambios en `router.tsx`:**
1. Agregar el import lazy: `const AcuerdoDetallePage = lazy(() => import("@/features/crowdsourcing/presentation/pages/AcuerdoDetallePage"))`
2. Agregar dentro del bloque `<Route element={<DashboardLayout />}>`:
   ```
   <Route path="/crowdsourcing/acuerdos/:id" element={<AcuerdoDetallePage />} />
   ```

**Justificacion de DashboardLayout:** La ruta del acuerdo es privada (solo participantes). `DashboardLayout` ya maneja la autenticacion y el redirect a login. El backend retorna 403 si el usuario no es participante, la pagina debe manejar ese caso con un mensaje de "Sin acceso".

**Constante de ruta:** Agregar `CROWDSOURCING_ACUERDO_DETAIL: '/crowdsourcing/acuerdos/:id'` a `src/web/src/lib/constants.ts` (archivo donde estan definidas las rutas locales del router). La funcion helper `APP_ROUTES.landing.crowdsourcing.acuerdoDetail(id)` ya esta definida en `@shared/constants`.

---

## 7. Estado Local de la Pagina de Detalle

La pagina `AcuerdoDetallePage` es el hub de estado de dialogs para toda la feature. Se usa `useState` para cada dialog, con un patron consistente:

```
Estado                      | Tipo              | Dialog que controla
----------------------------|-------------------|-----------------------------
milestoneDialogOpen         | boolean           | MilestoneFormDialog (crear)
milestoneToEdit             | Milestone | null  | MilestoneFormDialog (editar)
entregableDialogOpen        | boolean           | SubirEntregableDialog
entregableDialogMilestoneId | string | null     | Milestone preseleccionado al abrir SubirEntregableDialog
entregableToAprobar         | Entregable | null | AprobarEntregableDialog
entregableToRechazar        | Entregable | null | RechazarEntregableDialog
completarDialogOpen         | boolean           | CompletarAcuerdoDialog
cancelarDialogOpen          | boolean           | CancelarAcuerdoDialog
```

**Logica de apertura/cierre:**
- Abrir para editar: `setMilestoneToEdit(milestone); setMilestoneDialogOpen(true)`
- Abrir para crear: `setMilestoneToEdit(null); setMilestoneDialogOpen(true)`
- Cerrar: callback `onClose` que resetea el estado a `false`/`null`
- El estado `milestoneToEdit` determina si `MilestoneFormDialog` esta en modo crear o editar

**Logica de acciones segun rol y estado:**
```
Artista + Activo:
  - Puede abrir MilestoneFormDialog (crear)
  - Puede abrir MilestoneFormDialog (editar) en milestones no completados
  - Puede eliminar milestones sin entregables
  - Puede abrir AprobarEntregableDialog en entregables "Entregado"
  - Puede abrir RechazarEntregableDialog en entregables "Entregado"
  - Puede abrir CompletarAcuerdoDialog
  - Puede abrir CancelarAcuerdoDialog

Profesional + Activo:
  - Puede abrir SubirEntregableDialog
  - Puede abrir CancelarAcuerdoDialog

Cualquier rol + Completado o Cancelado:
  - Vista de solo lectura (ningun dialog de accion disponible)
  - Ver timeline, milestones, entregables como historial
```

---

## 8. Flujo de Datos

### 8.1 Flujo: Aceptar Propuesta y Navegar al Acuerdo

```
NecesidadDetallePage
    ↓ Estado: propuestaToAceptar, aceptarDialogOpen
AceptarPropuestaDialog (isOpen=true, propuesta=propuestaToAceptar)
    ↓ Usuario completa formulario (tituloInterno, fechaInicio, fechaFinPrevista)
    ↓ Submit → useAceptarPropuesta.mutate(data)
        ↓ acuerdoApi.aceptarPropuesta(propuestaId, data)
            ↓ POST /api/crowdsourcing/propuestas/{id}/aceptar
            ↓ Response 201: { data: { acuerdoId, ... } }
        ↓ onSuccess:
            - Invalida QUERY_KEYS.crowdsourcing.propuestas.mis
            - Toast: "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."
            - Llama a onSuccess(result.acuerdoId)
    ↓ Componente padre: navigate(APP_ROUTES.landing.crowdsourcing.acuerdoDetail(acuerdoId))
AcuerdoDetallePage carga con el nuevo acuerdoId
```

### 8.2 Flujo: Ver Detalle del Acuerdo

```
AcuerdoDetallePage
    ↓ useParams() → id
    ↓ useAcuerdo(id)
        ↓ acuerdoApi.getById(id)
            ↓ GET /api/crowdsourcing/acuerdos/{id}
            ↓ Response 200: { data: Acuerdo }
    ↓ Si error 403: muestra "No tienes acceso a este acuerdo"
    ↓ Si error 404: muestra "Acuerdo no encontrado"
    ↓ Si cargando: muestra skeletons
    ↓ Si datos: renderiza AcuerdoCabecera + MilestonesSection + EntregablesSinMilestone + AcuerdoTimeline + sidebar
```

### 8.3 Flujo: Crear Milestone

```
MilestonesSection
    ↓ Click "+ Agregar milestone"
    ↓ Callback onAgregarMilestone() → pagina: setMilestoneToEdit(null); setMilestoneDialogOpen(true)
MilestoneFormDialog (isOpen=true, milestoneToEdit=null → modo crear)
    ↓ Usuario completa formulario
    ↓ watch('importeParcial') → ImporteAsignadoBar actualiza preview en tiempo real
    ↓ Si importeNuevo + importeAsignado > importeTotal → error visible, submit deshabilitado
    ↓ Submit → useCreateMilestone.mutate(data)
        ↓ milestoneApi.create(acuerdoId, data)
            ↓ POST /api/crowdsourcing/acuerdos/{id}/milestones
            ↓ Response 201: { data: MilestoneCreatedResult }
        ↓ onSuccess:
            - Invalida QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)
            - Toast: "Milestone creado"
            - Llama a onSuccess() → pagina cierra el dialog
    ↓ useAcuerdo refetch automatico por invalidacion
    ↓ MilestonesSection se actualiza con el nuevo milestone
```

### 8.4 Flujo: Subir Entregable

```
MilestoneCard o EntregablesSinMilestone
    ↓ Click "Subir entregable"
    ↓ Callback onSubirEntregable(milestoneId) → pagina: setEntregableDialogMilestoneId(milestoneId); setEntregableDialogOpen(true)
SubirEntregableDialog (isOpen=true, preselectedMilestoneId=milestoneId)
    ↓ Usuario completa formulario (titulo, descripcion, url, milestone select)
    ↓ Submit → useCreateEntregable.mutate(data)
        ↓ entregableApi.create(acuerdoId, data)
            ↓ POST /api/crowdsourcing/acuerdos/{id}/entregables
            ↓ Response 201: { data: EntregableCreatedResult }
        ↓ onSuccess:
            - Invalida QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)
            - Toast: "Entregable subido correctamente. El artista sera notificado."
            - Llama a onSuccess() → pagina cierra el dialog
    ↓ Entregable aparece en el milestone correspondiente (o en EntregablesSinMilestone)
```

### 8.5 Flujo: Aprobar Entregable

```
EntregableItem (estadoEntregableId == ESTADO_ENTREGABLE.ENTREGADO, miRol == 'Artista')
    ↓ Click "Aprobar"
    ↓ Callback onAprobar(entregable) → pagina: setEntregableToAprobar(entregable)
AprobarEntregableDialog (isOpen=true, entregable!=null)
    ↓ Usuario ingresa comentario opcional
    ↓ Click "Confirmar aprobacion" → useAprobarEntregable.mutate({ entregableId, data })
        ↓ entregableApi.aprobar(entregableId, data)
            ↓ PATCH /api/crowdsourcing/entregables/{id}/aprobar
            ↓ Response 200: { data: AprobarEntregableResult }
        ↓ onSuccess(result):
            - Invalida QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)
            - Toast: "Entregable aprobado"
            - Si result.todosAprobadosEnMilestone == true:
                Toast adicional: "Todos los entregables del milestone fueron aprobados."
            - Llama a onSuccess() → pagina cierra el dialog
```

### 8.6 Flujo: Completar Acuerdo

```
Sidebar de AcuerdoDetallePage (miRol == 'Artista', estadoAcuerdoId == ACTIVO)
    ↓ Click "Completar acuerdo"
    ↓ setCompletarDialogOpen(true)
CompletarAcuerdoDialog (isOpen=true, acuerdo=acuerdo)
    ↓ Calcula: milestones completados, entregables con estado=Entregado (pendientes)
    ↓ Si hay entregables pendientes: muestra aviso amarillo (no bloqueante)
    ↓ Click "Completar acuerdo" → useCompletarAcuerdo.mutate()
        ↓ acuerdoApi.completar(acuerdoId)
            ↓ PATCH /api/crowdsourcing/acuerdos/{id}/completar
            ↓ Response 200: { data: CompletarAcuerdoResult }
        ↓ onSuccess:
            - Invalida QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)
            - Toast: "Acuerdo completado. Puedes dejar una valoracion al profesional."
            - Llama a onSuccess() → pagina cierra el dialog
    ↓ useAcuerdo refetch → AcuerdoDetallePage se actualiza con estado=Completado
    ↓ Acciones condicionales desaparecen (estado no Activo)
```

---

## 9. Dependencias de Shared

**Importar de `@shared/types/crowdsourcing`:**
- `Acuerdo`, `AcuerdoArtista`, `AcuerdoProfesional`, `AcuerdoNecesidad`, `AcuerdoTimelineEvento`
- `Milestone`, `Entregable`
- `AceptarPropuestaRequest`, `AceptarPropuestaResult`
- `RechazarPropuestaRequest`, `RechazarPropuestaResult`
- `CreateMilestoneRequest`, `MilestoneCreatedResult`
- `CreateEntregableRequest`, `EntregableCreatedResult`
- `AprobarEntregableRequest`, `AprobarEntregableResult`
- `RechazarEntregableRequest`, `RechazarEntregableResult`
- `CancelarAcuerdoRequest`, `CancelarAcuerdoResult`
- `CompletarAcuerdoResult`
- `EstadoAcuerdo`, `EstadoEntregable`, `RolAcuerdo`

**Importar de `@shared/schemas/crowdsourcing.schema`:**
- `aceptarPropuestaSchema`, `AceptarPropuestaFormData`
- `rechazarPropuestaSchema`, `RechazarPropuestaFormData`
- `createMilestoneSchema`, `CreateMilestoneFormData`
- `createEntregableSchema`, `CreateEntregableFormData`
- `aprobarEntregableSchema`, `AprobarEntregableFormData`
- `rechazarEntregableSchema`, `RechazarEntregableFormData`
- `cancelarAcuerdoSchema`, `CancelarAcuerdoFormData`

**Importar de `@shared/constants`:**
- `QUERY_KEYS.crowdsourcing.acuerdos` (nuevo, definido en contracts-plan)
- `API_ROUTES.crowdsourcing.acuerdos` (nuevo, definido en contracts-plan)
- `API_ROUTES.crowdsourcing.propuestas.aceptar` (nuevo)
- `API_ROUTES.crowdsourcing.propuestas.rechazar` (nuevo)
- `API_ROUTES.crowdsourcing.entregables` (nuevo)
- `APP_ROUTES.landing.crowdsourcing.acuerdoDetail` (nuevo)
- `ESTADO_ACUERDO` (nuevo)
- `ESTADO_ENTREGABLE` (nuevo)

**Agregar en `domain/types.ts` de la feature:**
```typescript
export type {
    Acuerdo,
    AcuerdoArtista,
    AcuerdoProfesional,
    AcuerdoNecesidad,
    AcuerdoTimelineEvento,
    Milestone,
    Entregable,
    AceptarPropuestaRequest,
    AceptarPropuestaResult,
    RechazarPropuestaRequest,
    RechazarPropuestaResult,
    CreateMilestoneRequest,
    MilestoneCreatedResult,
    CreateEntregableRequest,
    EntregableCreatedResult,
    AprobarEntregableRequest,
    AprobarEntregableResult,
    RechazarEntregableRequest,
    RechazarEntregableResult,
    CancelarAcuerdoRequest,
    CancelarAcuerdoResult,
    CompletarAcuerdoResult,
    EstadoAcuerdo,
    EstadoEntregable,
    RolAcuerdo,
} from "@shared/types/crowdsourcing"
```

---

## 10. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdsourcing/infrastructure/api/acuerdo.api.ts` | API Class | Metodos: getById, completar, cancelar, aceptarPropuesta, rechazarPropuesta |
| `src/web/src/features/crowdsourcing/infrastructure/api/milestone.api.ts` | API Class | Metodos: create, update, delete |
| `src/web/src/features/crowdsourcing/infrastructure/api/entregable.api.ts` | API Class | Metodos: create, aprobar, rechazar |
| `src/web/src/features/crowdsourcing/application/hooks/useAcuerdo.ts` | Query Hook | GET detalle acuerdo con staleTime 30s |
| `src/web/src/features/crowdsourcing/application/hooks/useAceptarPropuesta.ts` | Mutation Hook | POST aceptar propuesta, navega al acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useRechazarPropuesta.ts` | Mutation Hook | PATCH rechazar propuesta individual |
| `src/web/src/features/crowdsourcing/application/hooks/useCreateMilestone.ts` | Mutation Hook | POST crear milestone, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useUpdateMilestone.ts` | Mutation Hook | PUT editar milestone, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useDeleteMilestone.ts` | Mutation Hook | DELETE eliminar milestone, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useCreateEntregable.ts` | Mutation Hook | POST subir entregable, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useAprobarEntregable.ts` | Mutation Hook | PATCH aprobar entregable, toast extra si todos aprobados |
| `src/web/src/features/crowdsourcing/application/hooks/useRechazarEntregable.ts` | Mutation Hook | PATCH rechazar entregable, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useCompletarAcuerdo.ts` | Mutation Hook | PATCH completar acuerdo, invalida acuerdo |
| `src/web/src/features/crowdsourcing/application/hooks/useCancelarAcuerdo.ts` | Mutation Hook | PATCH cancelar acuerdo, invalida acuerdo |
| `src/web/src/features/crowdsourcing/presentation/pages/AcuerdoDetallePage.tsx` | Page Component | Hub principal, maneja estado de todos los dialogs |
| `src/web/src/features/crowdsourcing/presentation/components/AcuerdoCabecera.tsx` | Component | Cabecera: titulo, estado, partes, importe, fechas |
| `src/web/src/features/crowdsourcing/presentation/components/EstadoAcuerdoBadge.tsx` | Component | Badge coloreado por estado del acuerdo |
| `src/web/src/features/crowdsourcing/presentation/components/EstadoEntregableBadge.tsx` | Component | Badge coloreado por estado del entregable |
| `src/web/src/features/crowdsourcing/presentation/components/ImporteAsignadoBar.tsx` | Component | Barra de progreso de importe con preview en tiempo real |
| `src/web/src/features/crowdsourcing/presentation/components/MilestonesSection.tsx` | Component | Seccion de milestones con progreso global y boton agregar |
| `src/web/src/features/crowdsourcing/presentation/components/MilestoneCard.tsx` | Component | Card de milestone con entregables anidados |
| `src/web/src/features/crowdsourcing/presentation/components/EntregableItem.tsx` | Component | Item de entregable con acciones condicionales por rol |
| `src/web/src/features/crowdsourcing/presentation/components/EntregablesSinMilestone.tsx` | Component | Seccion para entregables sin milestone asignado |
| `src/web/src/features/crowdsourcing/presentation/components/AcuerdoTimeline.tsx` | Component | Lista cronologica de eventos del acuerdo |
| `src/web/src/features/crowdsourcing/presentation/components/ConversacionLink.tsx` | Component | Boton/link al chat vinculado al acuerdo |
| `src/web/src/features/crowdsourcing/presentation/components/MilestoneFormDialog.tsx` | Form Dialog | RHF+Zod para crear/editar milestone con preview de importe |
| `src/web/src/features/crowdsourcing/presentation/components/SubirEntregableDialog.tsx` | Form Dialog | RHF+Zod para subir entregable (profesional) |
| `src/web/src/features/crowdsourcing/presentation/components/AprobarEntregableDialog.tsx` | Dialog | Confirmacion con comentario opcional para aprobar |
| `src/web/src/features/crowdsourcing/presentation/components/RechazarEntregableDialog.tsx` | Form Dialog | RHF+Zod con comentario obligatorio para rechazar |
| `src/web/src/features/crowdsourcing/presentation/components/CompletarAcuerdoDialog.tsx` | Dialog | Resumen + aviso de entregables pendientes + confirmacion |
| `src/web/src/features/crowdsourcing/presentation/components/CancelarAcuerdoDialog.tsx` | Form Dialog | RHF+Zod con advertencia + motivo obligatorio |
| `src/web/src/features/crowdsourcing/presentation/components/AceptarPropuestaDialog.tsx` | Form Dialog | RHF+Zod formulario de aceptacion con defaults de propuesta |
| `src/web/src/features/crowdsourcing/presentation/components/RechazarPropuestaDialog.tsx` | Form Dialog | RHF+Zod con motivo opcional para rechazar propuesta |
| `src/web/src/features/crowdsourcing/__mocks__/acuerdo.mock.ts` | Mock | Datos de prueba para tests de la feature |

**Archivos a modificar:**
| Archivo | Cambio |
|---------|--------|
| `src/web/src/features/crowdsourcing/domain/types.ts` | Agregar re-exports de types de acuerdos |
| `src/web/src/features/crowdsourcing/infrastructure/index.ts` | Exportar `acuerdoApi`, `milestoneApi`, `entregableApi` |
| `src/web/src/features/crowdsourcing/application/index.ts` | Exportar todos los hooks nuevos |
| `src/web/src/features/crowdsourcing/presentation/components/index.ts` | Exportar todos los componentes nuevos |
| `src/web/src/features/crowdsourcing/presentation/pages/index.ts` | Exportar `AcuerdoDetallePage` |
| `src/web/src/app/router.tsx` | Agregar ruta `/crowdsourcing/acuerdos/:id` en DashboardLayout |
| `src/web/src/lib/constants.ts` | Agregar `CROWDSOURCING_ACUERDO_DETAIL` a `ROUTES` |
| `src/web/src/features/crowdsourcing/presentation/pages/NecesidadDetallePage.tsx` | Agregar estado y uso de `AceptarPropuestaDialog` y `RechazarPropuestaDialog` |

---

## 11. Orden de Implementacion Recomendado

El orden esta diseñado para que cada paso sea verificable antes de avanzar al siguiente.

### Fase 1: Infraestructura base (no visible, sin UI)

1. **`acuerdo.api.ts`** - Implementar `getById` primero. Verificar con Swagger que el endpoint funciona.
2. **`useAcuerdo.ts`** - Hook de query. Permite cargar datos antes de tener UI.
3. **`AcuerdoDetallePage.tsx`** (esqueleto) - Solo leer params, mostrar JSON crudo del acuerdo. Verificar que la ruta y el DashboardLayout funcionan.
4. Actualizar **`router.tsx`** y **`lib/constants.ts`** con la nueva ruta.

### Fase 2: Vista de detalle (read-only, solo presentacion)

5. **`EstadoAcuerdoBadge.tsx`** - Componente simple, verificable aislado.
6. **`EstadoEntregableBadge.tsx`** - Idem.
7. **`ImporteAsignadoBar.tsx`** - Barra de progreso. Verificar con datos mock.
8. **`AcuerdoCabecera.tsx`** - Cabecera completa con todos los datos del acuerdo.
9. **`EntregableItem.tsx`** (solo lectura, sin botones de accion por ahora).
10. **`MilestoneCard.tsx`** (solo lectura, con lista de EntregableItem sin acciones).
11. **`EntregablesSinMilestone.tsx`**.
12. **`AcuerdoTimeline.tsx`**.
13. **`ConversacionLink.tsx`**.
14. **`MilestonesSection.tsx`** (sin boton de agregar aun).
15. Integrar todo en **`AcuerdoDetallePage.tsx`** con el layout completo. Verificar render con acuerdo real.

### Fase 3: Acciones del Artista - Milestones

16. **`milestone.api.ts`** - Los tres metodos.
17. **`useCreateMilestone.ts`**, **`useUpdateMilestone.ts`**, **`useDeleteMilestone.ts`**.
18. **`MilestoneFormDialog.tsx`** - Con preview de importe en tiempo real. Esta es la pieza mas compleja de la fase.
19. Conectar en **`MilestoneCard.tsx`** los botones de editar/eliminar.
20. Conectar en **`MilestonesSection.tsx`** el boton "+ Agregar milestone".
21. Conectar callbacks en **`AcuerdoDetallePage.tsx`** para apertura/cierre del dialog.

### Fase 4: Acciones del Profesional - Entregables

22. **`entregable.api.ts`** - Los tres metodos.
23. **`useCreateEntregable.ts`**.
24. **`SubirEntregableDialog.tsx`**.
25. Conectar en **`AcuerdoDetallePage.tsx`** el boton y estado para subir entregables.
26. Verificar que el entregable aparece agrupado en el milestone correcto despues del refetch.

### Fase 5: Acciones del Artista - Revision de Entregables

27. **`useAprobarEntregable.ts`**, **`useRechazarEntregable.ts`**.
28. **`AprobarEntregableDialog.tsx`** con logica de `todosAprobadosEnMilestone`.
29. **`RechazarEntregableDialog.tsx`** con contador de caracteres.
30. Conectar botones en **`EntregableItem.tsx`** (activar acciones condicionales por rol).

### Fase 6: Ciclo de vida del Acuerdo

31. Completar **`acuerdo.api.ts`** con metodos `completar` y `cancelar`.
32. **`useCompletarAcuerdo.ts`**, **`useCancelarAcuerdo.ts`**.
33. **`CompletarAcuerdoDialog.tsx`** con logica de aviso de entregables pendientes.
34. **`CancelarAcuerdoDialog.tsx`** con el texto de advertencia irreversible.
35. Conectar ambos dialogs en el sidebar de **`AcuerdoDetallePage.tsx`**.

### Fase 7: Flujo desde NecesidadDetallePage (aceptar/rechazar propuesta)

36. Completar **`acuerdo.api.ts`** con `aceptarPropuesta` y `rechazarPropuesta`.
37. **`useAceptarPropuesta.ts`**, **`useRechazarPropuesta.ts`**.
38. **`AceptarPropuestaDialog.tsx`** con defaults calculados de la propuesta.
39. **`RechazarPropuestaDialog.tsx`** con motivo opcional.
40. Modificar **`NecesidadDetallePage.tsx`** para agregar el estado de estos dialogs y los botones de aceptar/rechazar en la seccion de propuestas del artista.

### Fase 8: Tests

41. **`acuerdo.mock.ts`** - Datos mock completos para todos los tests.
42. Tests de componentes: `EstadoAcuerdoBadge`, `EstadoEntregableBadge`, `ImporteAsignadoBar`, `EntregableItem`, `MilestoneCard`.
43. Tests de hooks: `useAcuerdo`, `useCreateMilestone`, `useAprobarEntregable`.
44. Tests de schemas: `createMilestoneSchema`, `rechazarEntregableSchema`, `cancelarAcuerdoSchema`.
45. Tests de la pagina: `AcuerdoDetallePage` con mocks de `useAcuerdo`.

---

## 12. Checklist

### Arquitectura
- [ ] Feature sigue la estructura hexagonal: domain / application / infrastructure / presentation
- [ ] Types importados de `@shared/`, no duplicados en la feature
- [ ] API Classes exportadas desde `infrastructure/index.ts`
- [ ] Hooks exportados desde `application/index.ts`
- [ ] Componentes exportados desde `presentation/components/index.ts`
- [ ] Pagina exportada desde `presentation/pages/index.ts`

### Componentes
- [ ] Todos los componentes usan `FC<Props>` con interface de props explicitamente tipada
- [ ] Componentes de UI usan exclusivamente shadcn/ui (no elementos HTML nativos para botones, dialogs, inputs)
- [ ] Dark theme consistente con colores del design system (`bg-[#0f1729]`, `border-[#334155]`, `text-white`)
- [ ] Renderizado condicional de acciones segun `miRol` y `estadoAcuerdoId`
- [ ] Skeletons para estados de carga en `AcuerdoDetallePage`
- [ ] Manejo de error 403 (acceso denegado) con mensaje explicativo
- [ ] Manejo de error 404 (no encontrado) con link para volver

### Hooks
- [ ] Query hook (`useAcuerdo`) con `enabled: !!id` y `staleTime` configurado
- [ ] Todos los mutation hooks invalidan `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` en `onSuccess`
- [ ] Toast de exito con el texto exacto del contrato en cada mutation
- [ ] Toast de error con mensaje legible en cada mutation (usando error codes de `@shared/utils/error-messages`)
- [ ] `useAprobarEntregable` maneja el caso `todosAprobadosEnMilestone == true` con toast adicional
- [ ] `useAceptarPropuesta` llama al callback con `acuerdoId` para navegacion desde el componente padre

### Formularios
- [ ] Todos los formularios usan `useForm` con `zodResolver`
- [ ] Schemas importados de `@shared/schemas/crowdsourcing.schema`
- [ ] `MilestoneFormDialog` muestra preview de importe en tiempo real con `watch`
- [ ] `RechazarEntregableDialog` muestra contador de caracteres
- [ ] `CancelarAcuerdoDialog` muestra contador de caracteres y advertencia en rojo
- [ ] Formularios se resetean correctamente al cerrar los dialogs
- [ ] Botones de submit deshabilitados mientras `isPending == true`
- [ ] `Loader2` con `animate-spin` en botones durante `isPending`

### Services
- [ ] API Classes usan `apiFetch` de `@/lib/api-client`
- [ ] Errores se lanzean como `Error(errorCode)` para que los hooks los capturen
- [ ] `DELETE` de milestone maneja correctamente la respuesta 204 sin body
- [ ] Todos los endpoints usan `API_ROUTES` de `@shared/constants`, no strings hardcodeados

### Router
- [ ] Ruta `/crowdsourcing/acuerdos/:id` dentro del `DashboardLayout` (autenticada)
- [ ] Pagina cargada con `lazy()` para code splitting
- [ ] Constante `CROWDSOURCING_ACUERDO_DETAIL` agregada a `ROUTES` en `lib/constants.ts`

### UX
- [ ] `ImporteAsignadoBar` se actualiza en tiempo real al modificar el importe en `MilestoneFormDialog` sin llamadas al backend
- [ ] `CompletarAcuerdoDialog` muestra aviso de entregables pendientes antes de confirmar
- [ ] `CancelarAcuerdoDialog` incluye el texto exacto: "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas."
- [ ] URLs externas de entregables se abren en nueva tab (`target="_blank" rel="noopener noreferrer"`)
- [ ] Sidebar de acciones sticky en desktop, CTA al fondo en mobile (consistente con `NecesidadDetallePage`)
