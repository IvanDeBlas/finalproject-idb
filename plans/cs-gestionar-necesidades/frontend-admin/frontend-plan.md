# Plan Frontend: Gestionar Necesidades de Crowdsourcing (Admin)

**Fecha:** 2026-02-16
**Feature:** cs-gestionar-necesidades
**Target:** src/admin (Next.js 14 App Router)

---

## 1. Resumen

- **Paginas:** 4 (Listado, Nueva, Editar, Detalle)
- **Componentes:** 12 reutilizables
- **Hooks:** 10 (5 queries, 5 mutations)
- **Services:** 1 (necesidad.service.ts)
- **Dependencias shared:** Types, schemas Zod, constants, utils

**Arquitectura:** App Router de Next.js con patron feature-based y colocation de componentes.

---

## 2. Estructura de Carpetas

```
src/admin/src/app/(dashboard)/crowdsourcing/necesidades/
├── page.tsx                                   # Listado mis necesidades (GET /mis-necesidades)
├── nueva/
│   └── page.tsx                               # Formulario crear (POST /necesidades)
├── [id]/
│   ├── page.tsx                               # Detalle + propuestas (GET /{id})
│   └── editar/
│       └── page.tsx                           # Formulario editar (PUT /{id})
├── components/
│   ├── list/
│   │   ├── NecesidadCard.tsx                  # Card para listado con badges, stats
│   │   ├── NecesidadFilters.tsx               # Filtros estado + search
│   │   └── EmptyStateNecesidades.tsx          # Empty state con ilustracion
│   ├── form/
│   │   ├── NecesidadForm.tsx                  # Formulario crear/editar con RHF + Zod
│   │   ├── UbicacionFields.tsx                # Ciudad/Pais condicionales
│   │   └── PresupuestoFields.tsx              # Min/Max/Moneda
│   ├── detail/
│   │   ├── NecesidadDetailHeader.tsx          # Header con titulo, badge, actions
│   │   ├── NecesidadDetailsCard.tsx           # Card detalles (descripcion, meta info)
│   │   ├── PropuestasSection.tsx              # Seccion propuestas recibidas
│   │   └── PropuestaCard.tsx                  # Card individual de propuesta
│   ├── shared/
│   │   ├── EstadoNecesidadBadge.tsx           # Badge con colores segun estado
│   │   ├── ModalidadBadge.tsx                 # Badge con icono de modalidad
│   │   └── CerrarNecesidadDialog.tsx          # Dialog confirmacion cierre
│   └── __tests__/
│       ├── NecesidadCard.test.tsx
│       ├── NecesidadFilters.test.tsx
│       ├── EmptyStateNecesidades.test.tsx
│       ├── NecesidadForm.test.tsx
│       ├── EstadoNecesidadBadge.test.tsx
│       └── CerrarNecesidadDialog.test.tsx

src/admin/src/hooks/
├── use-necesidades.ts                         # Query mis necesidades paginadas
├── use-necesidad.ts                           # Query by ID
├── use-necesidades-mutations.ts               # Create, Update, Cerrar mutations
├── use-tipos-necesidad.ts                     # Query maestras tipos
├── use-modalidades-trabajo.ts                 # Query maestras modalidades
├── use-monedas.ts                             # Query maestras monedas
└── __tests__/
    ├── use-necesidades.test.ts
    ├── use-necesidad.test.ts
    └── use-necesidades-mutations.test.ts

src/admin/src/services/
├── necesidad.service.ts                       # API calls a endpoints necesidades
└── __tests__/
    └── necesidad.service.test.ts
```

---

## 3. Componentes

### 3.1 NecesidadCard (Lista)

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/NecesidadCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| necesidad | `NecesidadCrowdsourcingList` | Si | DTO de necesidad del listado |
| onClick | `() => void` | No | Callback al hacer click en card |
| onEdit | `() => void` | No | Callback boton editar (solo si estado Abierta) |
| onCerrar | `() => void` | No | Callback boton cerrar (si Abierta o En Progreso) |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- `Card, CardHeader, CardContent, CardFooter` (shadcn/ui)
- `Badge` (shadcn/ui)
- `Button` (shadcn/ui)
- `EstadoNecesidadBadge` (shared component)
- `ModalidadBadge` (shared component)
- `Calendar, Users, MapPin` icons (lucide-react)
- `formatPresupuesto` (shared utils)
- `formatDistanceToNow` (date-fns)

**Responsabilidad:**
Renderiza card compacto de necesidad con:
- Badge de estado (color segun estado)
- Titulo destacado
- Meta info: tipo, modalidad, presupuesto
- Stats: contador propuestas, fecha creacion, fecha limite
- Actions condicionales: [Ver Detalle] [Editar] (solo Abierta) [Cerrar] (Abierta/En Progreso)
- Hover effect con transicion

**UI:**
```tsx
<Card hover>
  <CardHeader>
    <EstadoNecesidadBadge estado={necesidad.estadoNecesidadId} />
    <h3 className="text-xl font-semibold">{necesidad.titulo}</h3>
  </CardHeader>
  <CardContent>
    <div className="flex items-center gap-3">
      <Music className="w-4 h-4" />
      <span>{necesidad.tipoNecesidadNombre}</span>
      <ModalidadBadge modalidad={necesidad.modalidadTrabajoId} />
    </div>
    <div className="text-base font-medium">
      {formatPresupuesto(necesidad.presupuestoMin, necesidad.presupuestoMax, necesidad.monedaId)}
    </div>
    <div className="flex items-center gap-4 text-sm">
      <div className="flex items-center gap-1">
        <Users className="w-4 h-4" />
        <Badge>{necesidad.numeroPropuestas}</Badge>
      </div>
      <div className="flex items-center gap-1">
        <Calendar className="w-4 h-4" />
        <span>{formatDistanceToNow(necesidad.fechaCreacion)} ago</span>
      </div>
      {necesidad.fechaLimitePropuestas && (
        <div className={cn("flex items-center gap-1", isUrgent && "text-red-400")}>
          Limite: {format(necesidad.fechaLimitePropuestas, 'dd MMM yyyy')}
        </div>
      )}
    </div>
  </CardContent>
  <CardFooter className="flex gap-2">
    <Button variant="ghost" size="sm" onClick={onClick}>Ver Detalle</Button>
    {necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA && (
      <Button variant="ghost" size="sm" onClick={onEdit}>Editar</Button>
    )}
    {(necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA ||
      necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO) && (
      <Button variant="ghost" size="sm" className="text-red-400" onClick={onCerrar}>Cerrar</Button>
    )}
  </CardFooter>
</Card>
```

---

### 3.2 NecesidadFilters

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/NecesidadFilters.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| estadoFilter | `number \| 'all'` | Si | Estado seleccionado para filtrar |
| onEstadoChange | `(estado: number \| 'all') => void` | Si | Callback cambio estado |
| searchQuery | `string` | Si | Query de busqueda actual |
| onSearchChange | `(query: string) => void` | Si | Callback cambio search |

**Estado Local:**
- Ninguno (controlled component)

**Dependencias:**
- `Card` (shadcn/ui)
- `Select` (shadcn/ui)
- `Input` (shadcn/ui)
- `Search` icon (lucide-react)
- `ESTADO_NECESIDAD_LABELS` (shared constants)

**Responsabilidad:**
Renderiza filtros en una fila horizontal:
- Select de estado (Todos, Abierta, En Progreso, Cerrada, Cancelada)
- Input de busqueda con icono search y debounce (300ms en componente padre)

**UI:**
```tsx
<Card className="p-4">
  <div className="flex flex-col sm:flex-row gap-4">
    <Select value={estadoFilter} onValueChange={onEstadoChange}>
      <SelectTrigger className="w-full sm:w-48">
        <SelectValue placeholder="Estado" />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="all">Todos</SelectItem>
        <SelectItem value={ESTADO_NECESIDAD.ABIERTA}>Abierta</SelectItem>
        <SelectItem value={ESTADO_NECESIDAD.EN_PROGRESO}>En Progreso</SelectItem>
        <SelectItem value={ESTADO_NECESIDAD.CERRADA}>Cerrada</SelectItem>
        <SelectItem value={ESTADO_NECESIDAD.CANCELADA}>Cancelada</SelectItem>
      </SelectContent>
    </Select>
    <div className="relative flex-1">
      <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
      <Input
        placeholder="Buscar por titulo o descripcion..."
        value={searchQuery}
        onChange={(e) => onSearchChange(e.target.value)}
        className="pl-10"
      />
    </div>
  </div>
</Card>
```

---

### 3.3 EmptyStateNecesidades

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/EmptyStateNecesidades.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| hasFilters | `boolean` | Si | Si hay filtros activos |
| onClearFilters | `() => void` | No | Callback limpiar filtros |
| onNuevaNecesidad | `() => void` | No | Callback crear necesidad |
| onUsarPlantilla | `() => void` | No | Callback usar template |

**Estado Local:**
- Ninguno

**Dependencias:**
- `Inbox, SearchX` icons (lucide-react)
- `Button` (shadcn/ui)

**Responsabilidad:**
Muestra empty state segun contexto:
- **Sin filtros:** Ilustracion + mensaje "No tienes necesidades publicadas" + CTAs [Publicar necesidad] [Usar plantilla]
- **Con filtros:** Icono search + mensaje "No se encontraron necesidades" + CTA [Limpiar filtros]

**UI:**
```tsx
<div className="flex flex-col items-center justify-center min-h-[400px] text-center p-8">
  {hasFilters ? (
    <>
      <SearchX className="w-20 h-20 text-muted-foreground mb-4" />
      <h3 className="text-xl font-semibold mb-2">No se encontraron necesidades</h3>
      <p className="text-muted-foreground mb-6">Intenta ajustar los filtros</p>
      <Button variant="outline" onClick={onClearFilters}>Limpiar filtros</Button>
    </>
  ) : (
    <>
      <Inbox className="w-20 h-20 text-muted-foreground mb-4" />
      <h3 className="text-xl font-semibold mb-2">No tienes necesidades publicadas</h3>
      <p className="text-muted-foreground mb-6 max-w-md">
        Publica lo que necesitas y recibe propuestas de profesionales cualificados
      </p>
      <div className="flex gap-3">
        <Button onClick={onNuevaNecesidad}>Publicar necesidad</Button>
        <Button variant="outline" onClick={onUsarPlantilla}>Usar plantilla</Button>
      </div>
    </>
  )}
</div>
```

---

### 3.4 NecesidadForm

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/form/NecesidadForm.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| mode | `'create' \| 'edit'` | Si | Modo del formulario |
| defaultValues | `CreateNecesidadFormData \| UpdateNecesidadFormData` | No | Valores iniciales para editar |
| necesidadId | `string` | No | ID necesidad (solo edit mode) |
| numeroPropuestas | `number` | No | Contador propuestas (para banner en edit) |
| onSubmit | `(data: CreateNecesidadFormData \| UpdateNecesidadFormData) => Promise<void>` | Si | Callback submit |
| onCancel | `() => void` | Si | Callback cancelar |

**Estado Local:**
- Form state (react-hook-form)
- `watchModalidad` - Watch modalidad para mostrar/ocultar ubicacion

**Dependencias:**
- `useForm, Controller` (react-hook-form)
- `zodResolver` (@hookform/resolvers/zod)
- `createNecesidadSchema` o `updateNecesidadSchema` (shared schemas)
- `Card, Label, Input, Textarea, Select, Button, Alert` (shadcn/ui)
- `DatePicker` (custom component)
- `useTiposNecesidad, useModalidadesTrabajo, useMonedas` (hooks)
- `UbicacionFields, PresupuestoFields` (subcomponents)

**Responsabilidad:**
Formulario completo de crear/editar necesidad con:
- **Seccion Info Basica:** Titulo, Descripcion, Tipo Necesidad (disabled en edit), Modalidad
- **Seccion Presupuesto:** Min, Max, Moneda
- **Seccion Ubicacion y Fechas:** Ciudad/Pais (condicional), Fecha Limite, Fecha Inicio, Proyecto Artistico
- Banner warning si edit mode con propuestas > 0
- Validacion en tiempo real con Zod
- Submit handling con loading state

**UI:**
```tsx
<form onSubmit={handleSubmit(onSubmit)}>
  {mode === 'edit' && numeroPropuestas > 0 && (
    <Alert variant="warning" className="mb-6">
      <AlertCircle className="h-5 w-5" />
      <AlertTitle>Necesidad con propuestas</AlertTitle>
      <AlertDescription>
        Esta necesidad ya tiene {numeroPropuestas} propuestas. Los cambios seran visibles para los profesionales.
      </AlertDescription>
    </Alert>
  )}

  <Card className="p-6 mb-6">
    <h2 className="text-lg font-semibold mb-4">INFORMACION BASICA</h2>

    <div className="space-y-4">
      <div>
        <Label htmlFor="titulo">Titulo *</Label>
        <Input id="titulo" {...register('titulo')} />
        {errors.titulo && <p className="text-sm text-red-400">{errors.titulo.message}</p>}
      </div>

      <div>
        <Label htmlFor="descripcion">Descripcion</Label>
        <Textarea id="descripcion" {...register('descripcion')} rows={4} />
        <span className="text-xs text-muted-foreground">
          {watch('descripcion')?.length || 0} / 4000 caracteres
        </span>
        {errors.descripcion && <p className="text-sm text-red-400">{errors.descripcion.message}</p>}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <Label htmlFor="tipoNecesidadId">Tipo de necesidad *</Label>
          <Controller
            name="tipoNecesidadId"
            control={control}
            render={({ field }) => (
              <Select
                value={field.value?.toString()}
                onValueChange={(val) => field.onChange(parseInt(val))}
                disabled={mode === 'edit'}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecciona tipo" />
                </SelectTrigger>
                <SelectContent>
                  {tiposNecesidad?.map(tipo => (
                    <SelectItem key={tipo.id} value={tipo.id.toString()}>
                      {tipo.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {mode === 'edit' && (
            <p className="text-xs text-muted-foreground mt-1">
              No se puede cambiar el tipo una vez publicada
            </p>
          )}
        </div>

        <div>
          <Label htmlFor="modalidadTrabajoId">Modalidad *</Label>
          <Controller
            name="modalidadTrabajoId"
            control={control}
            render={({ field }) => (
              <Select value={field.value?.toString()} onValueChange={(val) => field.onChange(parseInt(val))}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecciona modalidad" />
                </SelectTrigger>
                <SelectContent>
                  {modalidades?.map(mod => (
                    <SelectItem key={mod.id} value={mod.id.toString()}>
                      {mod.nombre}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
        </div>
      </div>
    </div>
  </Card>

  <Card className="p-6 mb-6">
    <h2 className="text-lg font-semibold mb-4">PRESUPUESTO</h2>
    <PresupuestoFields control={control} errors={errors} monedas={monedas} />
  </Card>

  <Card className="p-6 mb-6">
    <h2 className="text-lg font-semibold mb-4">UBICACION Y FECHAS</h2>

    {(watchModalidad === MODALIDAD_TRABAJO.PRESENCIAL || watchModalidad === MODALIDAD_TRABAJO.HIBRIDO) && (
      <UbicacionFields control={control} errors={errors} />
    )}

    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <Label>Fecha limite propuestas</Label>
        <Controller
          name="fechaLimitePropuestas"
          control={control}
          render={({ field }) => (
            <DatePicker
              value={field.value ? new Date(field.value) : undefined}
              onChange={(date) => field.onChange(date?.toISOString())}
              minDate={new Date()}
            />
          )}
        />
      </div>
      <div>
        <Label>Fecha inicio prevista</Label>
        <Controller
          name="fechaInicioPrevista"
          control={control}
          render={({ field }) => (
            <DatePicker
              value={field.value ? new Date(field.value) : undefined}
              onChange={(date) => field.onChange(date?.toISOString())}
              minDate={new Date()}
            />
          )}
        />
      </div>
    </div>

    <div>
      <Label>Proyecto artistico *</Label>
      <Controller
        name="proyectoArtisticoId"
        control={control}
        render={({ field }) => (
          <Select value={field.value} onValueChange={field.onChange}>
            <SelectTrigger>
              <SelectValue placeholder="Selecciona proyecto" />
            </SelectTrigger>
            <SelectContent>
              {proyectos?.map(proyecto => (
                <SelectItem key={proyecto.id} value={proyecto.id}>
                  {proyecto.nombre}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        )}
      />
    </div>
  </Card>

  <div className="flex justify-between">
    <Button type="button" variant="outline" onClick={onCancel}>Cancelar</Button>
    <Button type="submit" disabled={isSubmitting}>
      {isSubmitting && <Loader2 className="animate-spin mr-2" />}
      {mode === 'create' ? 'Publicar Necesidad' : 'Guardar Cambios'}
    </Button>
  </div>
</form>
```

---

### 3.5 EstadoNecesidadBadge

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/shared/EstadoNecesidadBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| estadoId | `number` | Si | ID del estado (1=Abierta, 2=En Progreso, 3=Cerrada, 4=Cancelada) |

**Estado Local:**
- Ninguno

**Dependencias:**
- `Badge` (shadcn/ui)
- `ESTADO_NECESIDAD_LABELS, ESTADO_NECESIDAD_BADGES` (shared constants)
- `cn` (utils)

**Responsabilidad:**
Renderiza badge de estado con color semantico:
- Abierta: verde (bg-green-900/20 text-green-400 border-green-700)
- En Progreso: azul (bg-blue-900/20 text-blue-400 border-blue-700)
- Cerrada: gris (bg-gray-900/20 text-gray-400 border-gray-700)
- Cancelada: rojo (bg-red-900/20 text-red-400 border-red-700)

**UI:**
```tsx
export function EstadoNecesidadBadge({ estadoId }: { estadoId: number }) {
  const label = ESTADO_NECESIDAD_LABELS[estadoId] || 'Desconocido'
  const color = ESTADO_NECESIDAD_BADGES[estadoId] || 'gray'

  return (
    <Badge
      variant="outline"
      className={cn(
        "font-medium",
        color === 'green' && "bg-green-900/20 text-green-400 border-green-700",
        color === 'blue' && "bg-blue-900/20 text-blue-400 border-blue-700",
        color === 'gray' && "bg-gray-900/20 text-gray-400 border-gray-700",
        color === 'red' && "bg-red-900/20 text-red-400 border-red-700"
      )}
      aria-label={`Estado: ${label}`}
    >
      {label}
    </Badge>
  )
}
```

---

### 3.6 CerrarNecesidadDialog

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/shared/CerrarNecesidadDialog.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| open | `boolean` | Si | Estado abierto del dialog |
| onOpenChange | `(open: boolean) => void` | Si | Callback cambio estado |
| onConfirm | `(motivo?: string) => void` | Si | Callback confirmar cierre |
| isPending | `boolean` | Si | Estado loading de mutation |

**Estado Local:**
- `motivo` (string) - Estado local del textarea

**Dependencias:**
- `Dialog, DialogContent, DialogHeader, DialogFooter, DialogTitle` (shadcn/ui)
- `Alert, AlertTitle` (shadcn/ui)
- `Textarea, Button, Label` (shadcn/ui)
- `AlertCircle, Loader2` icons (lucide-react)

**Responsabilidad:**
Dialog de confirmacion para cerrar necesidad con:
- Warning alert sobre auto-rechazo de propuestas pendientes
- Textarea opcional para motivo de cierre (max 500 chars)
- Character counter
- Botones [Cancelar] [Cerrar Necesidad] (destructive)
- Focus trap y escape key handling

**UI:**
```tsx
<Dialog open={open} onOpenChange={onOpenChange}>
  <DialogContent className="max-w-md">
    <DialogHeader>
      <DialogTitle>Cerrar Necesidad</DialogTitle>
    </DialogHeader>

    <Alert variant="warning" className="mb-4">
      <AlertCircle className="h-5 w-5" />
      <AlertTitle>
        Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente.
      </AlertTitle>
    </Alert>

    <div className="space-y-2">
      <Label htmlFor="motivo">Motivo del cierre (opcional)</Label>
      <Textarea
        id="motivo"
        value={motivo}
        onChange={(e) => setMotivo(e.target.value)}
        maxLength={500}
        rows={4}
        placeholder="Ej: Ya encontre un profesional por otra via"
        disabled={isPending}
      />
      <span className="text-xs text-muted-foreground">{motivo.length} / 500 caracteres</span>
    </div>

    <DialogFooter className="flex gap-3">
      <Button variant="outline" onClick={() => onOpenChange(false)} disabled={isPending}>
        Cancelar
      </Button>
      <Button
        variant="destructive"
        onClick={() => onConfirm(motivo)}
        disabled={isPending}
      >
        {isPending && <Loader2 className="animate-spin mr-2" />}
        Cerrar Necesidad
      </Button>
    </DialogFooter>
  </DialogContent>
</Dialog>
```

---

### 3.7 PropuestaCard

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/detail/PropuestaCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| propuesta | `PropuestaCrowdsourcing` | Si | DTO de propuesta |
| readonly | `boolean` | No | Si es solo lectura (estado != Abierta) |
| onVerPerfil | `() => void` | No | Callback ver perfil profesional |
| onAceptar | `() => void` | No | Callback aceptar propuesta |
| onRechazar | `() => void` | No | Callback rechazar propuesta |

**Estado Local:**
- `expanded` (boolean) - Si mensaje expandido completo

**Dependencias:**
- `Card, Badge, Avatar, Button` (shadcn/ui)
- `Star, Clock, Calendar` icons (lucide-react)
- `formatPresupuesto` (shared utils)
- `formatDistanceToNow` (date-fns)

**Responsabilidad:**
Card de propuesta recibida con:
- Avatar + nombre profesional + rol
- Rating (si disponible)
- Mensaje de propuesta (truncado a 150 chars con "Leer mas")
- Precio propuesto + moneda + dias estimados
- Fecha de envio
- Actions: [Ver Perfil] [Aceptar] [Rechazar] (solo si no readonly)

**UI:**
```tsx
<Card className="p-4">
  <div className="flex items-center gap-3 mb-3">
    <Avatar>
      <AvatarFallback>{propuesta.profesionalNombre[0]}</AvatarFallback>
    </Avatar>
    <div className="flex-1">
      <div className="font-semibold">{propuesta.profesionalNombre}</div>
      <div className="text-sm text-muted-foreground">
        {/* Rol profesional + Rating */}
        <div className="flex items-center gap-2">
          <span>Ingeniero de Audio</span>
          <div className="flex items-center gap-1">
            <Star className="w-3 h-3 fill-yellow-400 text-yellow-400" />
            <span>4.8 (12)</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <p className="text-sm text-muted-foreground mb-3">
    {expanded ? propuesta.mensaje : `${propuesta.mensaje.slice(0, 150)}...`}
    {propuesta.mensaje.length > 150 && (
      <Button variant="link" size="sm" onClick={() => setExpanded(!expanded)}>
        {expanded ? 'Leer menos' : 'Leer mas'}
      </Button>
    )}
  </p>

  <div className="flex items-center gap-4 text-sm text-muted-foreground mb-3">
    <Badge variant="secondary" className="font-semibold">
      {formatPresupuesto(propuesta.precioPropuesto, undefined, propuesta.monedaId)}
    </Badge>
    {propuesta.tiempoEstimadoDias && (
      <div className="flex items-center gap-1">
        <Clock className="w-4 h-4" />
        <span>{propuesta.tiempoEstimadoDias} dias</span>
      </div>
    )}
    <div className="flex items-center gap-1">
      <Calendar className="w-4 h-4" />
      <span>{formatDistanceToNow(propuesta.fechaCreacion)} ago</span>
    </div>
  </div>

  {!readonly && (
    <div className="flex gap-2 pt-3 border-t">
      <Button variant="ghost" size="sm" onClick={onVerPerfil}>Ver Perfil</Button>
      <Button variant="default" size="sm" onClick={onAceptar}>Aceptar</Button>
      <Button variant="ghost" size="sm" className="text-red-400" onClick={onRechazar}>
        Rechazar
      </Button>
    </div>
  )}
</Card>
```

---

### Componentes adicionales (breve descripcion)

**3.8 NecesidadDetailHeader**
- Props: `necesidad`, `onEdit`, `onCerrar`
- Renderiza header con titulo, badge estado, actions menu

**3.9 NecesidadDetailsCard**
- Props: `necesidad`
- Card con descripcion + grid de info (tipo, modalidad, presupuesto, ubicacion, fechas)

**3.10 PropuestasSection**
- Props: `propuestas`, `readonly`, callbacks
- Seccion de propuestas recibidas con header count + lista de PropuestaCard

**3.11 UbicacionFields**
- Props: `control`, `errors` (form context)
- Subcomponente campos ciudad/pais para NecesidadForm

**3.12 PresupuestoFields**
- Props: `control`, `errors`, `monedas` (form context)
- Subcomponente campos min/max/moneda para NecesidadForm

---

## 4. Hooks

### 4.1 useMisNecesidades

**Archivo:** `src/admin/src/hooks/use-necesidades.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| page | `number` | Numero de pagina (default 1) |
| pageSize | `number` | Items por pagina (default 12) |
| estado | `number \| undefined` | Filtro por estado |
| search | `string \| undefined` | Busqueda texto libre |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `PaginatedResponse<NecesidadCrowdsourcingList>` | Datos paginados |
| isLoading | `boolean` | Estado de carga inicial |
| error | `Error \| null` | Error si hay |
| refetch | `() => void` | Refetch manual |

**Query Key:** `QUERY_KEYS.crowdsourcing.necesidades.mis`

**Implementacion:**
```typescript
import { useQuery } from "@tanstack/react-query"
import { necesidadService } from "@/services/necesidad.service"
import { QUERY_KEYS } from "@shared/constants"
import type { PaginatedResponse, NecesidadCrowdsourcingList } from "@shared/types"

interface UseMisNecesidadesParams {
    page?: number
    pageSize?: number
    estado?: number
    search?: string
}

export function useMisNecesidades({
    page = 1,
    pageSize = 12,
    estado,
    search,
}: UseMisNecesidadesParams = {}) {
    return useQuery<PaginatedResponse<NecesidadCrowdsourcingList>>({
        queryKey: [...QUERY_KEYS.crowdsourcing.necesidades.mis, { page, pageSize, estado, search }],
        queryFn: () => necesidadService.getMisNecesidades({ page, pageSize, estado, search }),
        staleTime: 2 * 60 * 1000, // 2 minutos
        keepPreviousData: true, // Para paginacion smooth
    })
}
```

---

### 4.2 useNecesidad

**Archivo:** `src/admin/src/hooks/use-necesidades.ts` (mismo archivo)

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | `string` | ID de necesidad |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `NecesidadCrowdsourcing \| null` | Necesidad completa con propuestas |
| isLoading | `boolean` | Estado de carga |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.crowdsourcing.necesidades.byId(id)`

**Implementacion:**
```typescript
export function useNecesidad(id: string) {
    return useQuery<NecesidadCrowdsourcing | null>({
        queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id),
        queryFn: () => necesidadService.getById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000, // 5 minutos
    })
}
```

---

### 4.3 useCreateNecesidad

**Archivo:** `src/admin/src/hooks/use-necesidades-mutations.ts`

**Tipo:** Mutation Hook

**Parametros:**
- Ninguno (se pasa data en mutate)

**Acciones:**
- `mutate(data: CreateNecesidadRequest)` - Ejecutar mutacion
- `onSuccess` - Invalidar queries de listado, toast success, callback opcional
- `onError` - Toast error con mensaje del backend

**Implementacion:**
```typescript
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { necesidadService } from "@/services/necesidad.service"
import { QUERY_KEYS, APP_ROUTES } from "@shared/constants"
import type { CreateNecesidadRequest, NecesidadCreateResult } from "@shared/types"

export function useCreateNecesidad() {
    const queryClient = useQueryClient()
    const router = useRouter()

    return useMutation<NecesidadCreateResult, Error, CreateNecesidadRequest>({
        mutationFn: (data) => necesidadService.create(data),
        onSuccess: (result) => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis })
            toast.success("Necesidad publicada correctamente")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)
        },
        onError: (error) => {
            toast.error(error.message || "Error al publicar la necesidad")
        },
    })
}
```

---

### 4.4 useUpdateNecesidad

**Archivo:** `src/admin/src/hooks/use-necesidades-mutations.ts` (mismo archivo)

**Tipo:** Mutation Hook

**Parametros:**
- `id` (string) - ID de necesidad a actualizar

**Acciones:**
- `mutate(data: UpdateNecesidadRequest)` - Ejecutar mutacion
- `onSuccess` - Invalidar queries (listado + detalle), toast success, redirect a detalle
- `onError` - Toast error con mensaje del backend

**Implementacion:**
```typescript
export function useUpdateNecesidad(id: string) {
    const queryClient = useQueryClient()
    const router = useRouter()

    return useMutation<NecesidadUpdateResult, Error, UpdateNecesidadRequest>({
        mutationFn: (data) => necesidadService.update(id, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis })
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id) })
            toast.success("Necesidad actualizada correctamente")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(id))
        },
        onError: (error) => {
            toast.error(error.message || "Error al actualizar la necesidad")
        },
    })
}
```

---

### 4.5 useCerrarNecesidad

**Archivo:** `src/admin/src/hooks/use-necesidades-mutations.ts` (mismo archivo)

**Tipo:** Mutation Hook

**Parametros:**
- `id` (string) - ID de necesidad a cerrar

**Acciones:**
- `mutate(data: CerrarNecesidadRequest)` - Ejecutar mutacion (motivo opcional)
- `onSuccess` - Invalidar queries, toast success con contador propuestas rechazadas
- `onError` - Toast error

**Implementacion:**
```typescript
export function useCerrarNecesidad(id: string) {
    const queryClient = useQueryClient()

    return useMutation<CerrarNecesidadResult, Error, CerrarNecesidadRequest>({
        mutationFn: (data) => necesidadService.cerrar(id, data),
        onSuccess: (result) => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.mis })
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id) })

            const message = result.propuestasRechazadas > 0
                ? `Necesidad cerrada. Se han rechazado ${result.propuestasRechazadas} propuestas pendientes.`
                : "Necesidad cerrada correctamente"

            toast.success(message)
        },
        onError: (error) => {
            toast.error(error.message || "Error al cerrar la necesidad")
        },
    })
}
```

---

### 4.6 useTiposNecesidad

**Archivo:** `src/admin/src/hooks/use-tipos-necesidad.ts`

**Tipo:** Query Hook (Maestras)

**Parametros:**
- Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `MaestraTipoNecesidad[]` | Lista de tipos |
| isLoading | `boolean` | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.tiposNecesidad`

**Implementacion:**
```typescript
import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useTiposNecesidad() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.tiposNecesidad,
        queryFn: () => maestrasService.getTiposNecesidad(),
        staleTime: Infinity, // Maestras no cambian
    })
}
```

---

### 4.7 useModalidadesTrabajo

**Archivo:** `src/admin/src/hooks/use-modalidades-trabajo.ts`

**Tipo:** Query Hook (Maestras)

**Parametros:**
- Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `MaestraModalidadTrabajo[]` | Lista de modalidades |
| isLoading | `boolean` | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.modalidadesTrabajo`

**Implementacion:**
Similar a useTiposNecesidad.

---

### 4.8 useMonedas

**Archivo:** `src/admin/src/hooks/use-monedas.ts`

**Tipo:** Query Hook (Maestras)

**Parametros:**
- Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `MaestraMoneda[]` | Lista de monedas |
| isLoading | `boolean` | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.monedas`

**Implementacion:**
Similar a useTiposNecesidad.

---

### Hooks adicionales (breve descripcion)

**4.9 useProyectosArtista**
- Query para obtener proyectos del artista autenticado
- Usado en select de NecesidadForm
- (Ya existe hook similar en `use-artista.ts`, reutilizar o extender)

**4.10 useDebounce**
- Custom hook para debounce de search input (300ms)
- (Utility hook, puede estar en `hooks/use-debounce.ts`)

---

## 5. Services

### 5.1 necesidad.service.ts

**Archivo:** `src/admin/src/services/necesidad.service.ts`

**Metodos:**

| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| `getMisNecesidades` | `{ page, pageSize, estado?, search? }` | `PaginatedResponse<NecesidadCrowdsourcingList>` | GET `/api/crowdsourcing/necesidades/mis-necesidades` |
| `getById` | `id: string` | `NecesidadCrowdsourcing \| null` | GET `/api/crowdsourcing/necesidades/{id}` |
| `create` | `data: CreateNecesidadRequest` | `NecesidadCreateResult` | POST `/api/crowdsourcing/necesidades` |
| `update` | `id: string, data: UpdateNecesidadRequest` | `NecesidadUpdateResult` | PUT `/api/crowdsourcing/necesidades/{id}` |
| `cerrar` | `id: string, data: CerrarNecesidadRequest` | `CerrarNecesidadResult` | PATCH `/api/crowdsourcing/necesidades/{id}/cerrar` |

**Implementacion:**
```typescript
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    NecesidadCrowdsourcingList,
    NecesidadCrowdsourcing,
    CreateNecesidadRequest,
    UpdateNecesidadRequest,
    CerrarNecesidadRequest,
    NecesidadCreateResult,
    NecesidadUpdateResult,
    CerrarNecesidadResult,
    ServiceResponse,
    PaginatedResponse,
} from "@shared/types"

interface GetMisNecesidadesParams {
    page?: number
    pageSize?: number
    estado?: number
    search?: string
}

class NecesidadService {
    private readonly baseUrl = API_ROUTES.crowdsourcing.necesidades.base

    async getMisNecesidades(params: GetMisNecesidadesParams = {}): Promise<PaginatedResponse<NecesidadCrowdsourcingList>> {
        const queryParams = new URLSearchParams()
        if (params.page) queryParams.append("page", params.page.toString())
        if (params.pageSize) queryParams.append("pageSize", params.pageSize.toString())
        if (params.estado) queryParams.append("estado", params.estado.toString())
        if (params.search) queryParams.append("search", params.search)

        const url = `${API_ROUTES.crowdsourcing.necesidades.mis}?${queryParams.toString()}`
        const response = await apiFetch<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingList>>>(url)
        return response.data
    }

    async getById(id: string): Promise<NecesidadCrowdsourcing | null> {
        try {
            const response = await apiFetch<ServiceResponse<NecesidadCrowdsourcing>>(
                API_ROUTES.crowdsourcing.necesidades.byId(id)
            )
            return response.data
        } catch {
            return null
        }
    }

    async create(data: CreateNecesidadRequest): Promise<NecesidadCreateResult> {
        const response = await apiFetch<ServiceResponse<NecesidadCreateResult>>(this.baseUrl, {
            method: "POST",
            data,
        })
        return response.data
    }

    async update(id: string, data: UpdateNecesidadRequest): Promise<NecesidadUpdateResult> {
        const response = await apiFetch<ServiceResponse<NecesidadUpdateResult>>(
            API_ROUTES.crowdsourcing.necesidades.byId(id),
            {
                method: "PUT",
                data,
            }
        )
        return response.data
    }

    async cerrar(id: string, data: CerrarNecesidadRequest): Promise<CerrarNecesidadResult> {
        const response = await apiFetch<ServiceResponse<CerrarNecesidadResult>>(
            API_ROUTES.crowdsourcing.necesidades.cerrar(id),
            {
                method: "PATCH",
                data,
            }
        )
        return response.data
    }
}

export const necesidadService = new NecesidadService()
```

**Notas:**
- Usa `apiFetch` wrapper (ya existe en proyecto)
- Retorna directamente `response.data` (data unwrapping)
- Manejo de errores delegado a `apiFetch` (lanza excepciones que captura react-query)
- `getById` tiene try-catch para retornar null si 404 (evita error state en UI)

---

## 6. Paginas (Next.js App Router)

### 6.1 Listado Mis Necesidades

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/page.tsx`

**Ruta:** `/dashboard/crowdsourcing/necesidades`

**Layout:**
- Header con titulo, subtitulo, botones [+ Nueva Necesidad] [Usar Plantilla]
- Filtros (estado + search con debounce)
- Grid de NecesidadCard (responsive: 1/2/3 cols)
- Empty state si no hay necesidades
- Paginacion

**State:**
- `searchQuery` (string)
- `estadoFilter` (number | 'all')
- `page` (number)
- `cerrarDialogId` (string | null) - ID necesidad a cerrar

**Hooks usados:**
- `useMisNecesidades({ page, pageSize: 12, estado: estadoFilter !== 'all' ? estadoFilter : undefined, search: debouncedSearch })`
- `useCerrarNecesidad(cerrarDialogId!)`
- `useDebounce(searchQuery, 300)`
- `useRouter` (Next.js)

**Implementacion:**
```tsx
"use client"

import { useState, useMemo } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useMisNecesidades, useDebounce } from "@/hooks"
import { useCerrarNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadCard } from "./components/list/NecesidadCard"
import { NecesidadFilters } from "./components/list/NecesidadFilters"
import { EmptyStateNecesidades } from "./components/list/EmptyStateNecesidades"
import { CerrarNecesidadDialog } from "./components/shared/CerrarNecesidadDialog"
import { APP_ROUTES } from "@shared/constants"
import { Skeleton } from "@/components/ui/skeleton"

export default function MisNecesidadesPage() {
    const router = useRouter()
    const [searchQuery, setSearchQuery] = useState("")
    const [estadoFilter, setEstadoFilter] = useState<number | "all">("all")
    const [page, setPage] = useState(1)
    const [cerrarDialogId, setCerrarDialogId] = useState<string | null>(null)

    const debouncedSearch = useDebounce(searchQuery, 300)

    const { data, isLoading } = useMisNecesidades({
        page,
        pageSize: 12,
        estado: estadoFilter !== "all" ? estadoFilter : undefined,
        search: debouncedSearch || undefined,
    })

    const cerrarMutation = useCerrarNecesidad(cerrarDialogId!)

    const hasFilters = searchQuery !== "" || estadoFilter !== "all"

    const handleCerrarConfirm = (motivo?: string) => {
        cerrarMutation.mutate({ motivo }, {
            onSettled: () => setCerrarDialogId(null),
        })
    }

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div>
                    <h1 className="text-3xl font-bold">Mis Necesidades</h1>
                    <p className="text-muted-foreground mt-1">
                        Gestiona tus solicitudes de servicios profesionales
                    </p>
                </div>
                <div className="flex gap-3">
                    <Button onClick={() => router.push(APP_ROUTES.dashboard.crowdsourcing.nuevaNecesidad)}>
                        <Plus className="h-4 w-4 mr-2" />
                        Nueva Necesidad
                    </Button>
                    <Button
                        variant="outline"
                        onClick={() => router.push(APP_ROUTES.dashboard.crowdsourcing.templates)}
                    >
                        Usar Plantilla
                    </Button>
                </div>
            </div>

            <NecesidadFilters
                estadoFilter={estadoFilter}
                onEstadoChange={setEstadoFilter}
                searchQuery={searchQuery}
                onSearchChange={setSearchQuery}
            />

            {isLoading ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {[...Array(6)].map((_, i) => (
                        <Skeleton key={i} className="h-64" />
                    ))}
                </div>
            ) : data?.items.length === 0 ? (
                <EmptyStateNecesidades
                    hasFilters={hasFilters}
                    onClearFilters={() => {
                        setSearchQuery("")
                        setEstadoFilter("all")
                    }}
                    onNuevaNecesidad={() => router.push(APP_ROUTES.dashboard.crowdsourcing.nuevaNecesidad)}
                    onUsarPlantilla={() => router.push(APP_ROUTES.dashboard.crowdsourcing.templates)}
                />
            ) : (
                <>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {data?.items.map((necesidad) => (
                            <NecesidadCard
                                key={necesidad.id}
                                necesidad={necesidad}
                                onClick={() => router.push(APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(necesidad.id))}
                                onEdit={() => router.push(APP_ROUTES.dashboard.crowdsourcing.editarNecesidad(necesidad.id))}
                                onCerrar={() => setCerrarDialogId(necesidad.id)}
                            />
                        ))}
                    </div>

                    {/* TODO: Pagination component */}
                </>
            )}

            <CerrarNecesidadDialog
                open={!!cerrarDialogId}
                onOpenChange={(open) => !open && setCerrarDialogId(null)}
                onConfirm={handleCerrarConfirm}
                isPending={cerrarMutation.isPending}
            />
        </div>
    )
}
```

---

### 6.2 Nueva Necesidad

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/nueva/page.tsx`

**Ruta:** `/dashboard/crowdsourcing/necesidades/nueva`

**Layout:**
- Header con link "< Volver a Mis Necesidades"
- Titulo "Publicar Nueva Necesidad"
- NecesidadForm en modo create

**State:**
- Ninguno (state en form component)

**Hooks usados:**
- `useCreateNecesidad()`
- `useRouter`

**Implementacion:**
```tsx
"use client"

import { useRouter } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useCreateNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadForm } from "../components/form/NecesidadForm"
import { APP_ROUTES } from "@shared/constants"
import type { CreateNecesidadFormData } from "@shared/schemas"

export default function NuevaNecesidadPage() {
    const router = useRouter()
    const createMutation = useCreateNecesidad()

    const handleSubmit = async (data: CreateNecesidadFormData) => {
        await createMutation.mutateAsync(data)
        // Router push handled in mutation onSuccess
    }

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() => router.back()}
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Volver a Mis Necesidades
            </Button>

            <div>
                <h1 className="text-3xl font-bold">Publicar Nueva Necesidad</h1>
                <p className="text-muted-foreground mt-1">
                    Completa los detalles de tu necesidad para recibir propuestas
                </p>
            </div>

            <NecesidadForm
                mode="create"
                onSubmit={handleSubmit}
                onCancel={() => router.back()}
            />
        </div>
    )
}
```

---

### 6.3 Editar Necesidad

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/[id]/editar/page.tsx`

**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}/editar`

**Layout:**
- Header con link "< Volver"
- Titulo "Editar Necesidad"
- NecesidadForm en modo edit con datos pre-rellenados
- Banner warning si tiene propuestas

**State:**
- Ninguno (state en form component)

**Hooks usados:**
- `useNecesidad(id)` - Para fetch datos
- `useUpdateNecesidad(id)`
- `useRouter`

**Implementacion:**
```tsx
"use client"

import { useRouter } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useNecesidad } from "@/hooks/use-necesidades"
import { useUpdateNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadForm } from "../../components/form/NecesidadForm"
import { APP_ROUTES, ESTADO_NECESIDAD } from "@shared/constants"
import type { UpdateNecesidadFormData } from "@shared/schemas"
import { Skeleton } from "@/components/ui/skeleton"
import { toast } from "sonner"
import { useEffect } from "react"

interface EditarNecesidadPageProps {
    params: {
        id: string
    }
}

export default function EditarNecesidadPage({ params }: EditarNecesidadPageProps) {
    const router = useRouter()
    const { data: necesidad, isLoading } = useNecesidad(params.id)
    const updateMutation = useUpdateNecesidad(params.id)

    useEffect(() => {
        // Redirect si estado != Abierta
        if (necesidad && necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA) {
            toast.error("Solo se pueden editar necesidades en estado Abierta")
            router.push(APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(params.id))
        }
    }, [necesidad, params.id, router])

    const handleSubmit = async (data: UpdateNecesidadFormData) => {
        await updateMutation.mutateAsync(data)
        // Router push handled in mutation onSuccess
    }

    if (isLoading) {
        return <Skeleton className="h-screen" />
    }

    if (!necesidad) {
        return <div>Necesidad no encontrada</div>
    }

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() => router.back()}
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Volver
            </Button>

            <div>
                <h1 className="text-3xl font-bold">Editar Necesidad</h1>
                <p className="text-muted-foreground mt-1">
                    Modifica los detalles de tu necesidad
                </p>
            </div>

            <NecesidadForm
                mode="edit"
                necesidadId={params.id}
                defaultValues={{
                    titulo: necesidad.titulo,
                    descripcion: necesidad.descripcion,
                    modalidadTrabajoId: necesidad.modalidadTrabajoId,
                    presupuestoMin: necesidad.presupuestoMin,
                    presupuestoMax: necesidad.presupuestoMax,
                    monedaId: necesidad.monedaId,
                    ubicacionCiudad: necesidad.ubicacionCiudad,
                    ubicacionPais: necesidad.ubicacionPais,
                    fechaLimitePropuestas: necesidad.fechaLimitePropuestas,
                    fechaInicioPrevista: necesidad.fechaInicioPrevista,
                }}
                numeroPropuestas={necesidad.propuestas.length}
                onSubmit={handleSubmit}
                onCancel={() => router.back()}
            />
        </div>
    )
}
```

---

### 6.4 Detalle Necesidad

**Archivo:** `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/[id]/page.tsx`

**Ruta:** `/dashboard/crowdsourcing/necesidades/{id}`

**Layout:**
- Header con link "< Mis Necesidades" + titulo + badge estado + actions
- Card de detalles (descripcion, meta info)
- Seccion propuestas recibidas con cards

**State:**
- `cerrarDialogOpen` (boolean)

**Hooks usados:**
- `useNecesidad(id)`
- `useCerrarNecesidad(id)`
- `useRouter`

**Implementacion:**
```tsx
"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useNecesidad } from "@/hooks/use-necesidades"
import { useCerrarNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadDetailHeader } from "../components/detail/NecesidadDetailHeader"
import { NecesidadDetailsCard } from "../components/detail/NecesidadDetailsCard"
import { PropuestasSection } from "../components/detail/PropuestasSection"
import { CerrarNecesidadDialog } from "../components/shared/CerrarNecesidadDialog"
import { APP_ROUTES, ESTADO_NECESIDAD } from "@shared/constants"
import { Skeleton } from "@/components/ui/skeleton"

interface NecesidadDetailPageProps {
    params: {
        id: string
    }
}

export default function NecesidadDetailPage({ params }: NecesidadDetailPageProps) {
    const router = useRouter()
    const { data: necesidad, isLoading } = useNecesidad(params.id)
    const cerrarMutation = useCerrarNecesidad(params.id)

    const [cerrarDialogOpen, setCerrarDialogOpen] = useState(false)

    const handleCerrarConfirm = (motivo?: string) => {
        cerrarMutation.mutate({ motivo }, {
            onSettled: () => setCerrarDialogOpen(false),
        })
    }

    if (isLoading) {
        return <Skeleton className="h-screen" />
    }

    if (!necesidad) {
        return <div>Necesidad no encontrada</div>
    }

    const readonly = necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() => router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)}
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Mis Necesidades
            </Button>

            <NecesidadDetailHeader
                necesidad={necesidad}
                onEdit={() => router.push(APP_ROUTES.dashboard.crowdsourcing.editarNecesidad(params.id))}
                onCerrar={() => setCerrarDialogOpen(true)}
            />

            <NecesidadDetailsCard necesidad={necesidad} />

            <PropuestasSection
                propuestas={necesidad.propuestas}
                readonly={readonly}
                onVerPerfil={(profesionalId) => {
                    // TODO: Navigate to profesional profile
                }}
                onAceptar={(propuestaId) => {
                    // TODO: Implement aceptar propuesta (US-CS-04)
                }}
                onRechazar={(propuestaId) => {
                    // TODO: Implement rechazar propuesta
                }}
            />

            <CerrarNecesidadDialog
                open={cerrarDialogOpen}
                onOpenChange={setCerrarDialogOpen}
                onConfirm={handleCerrarConfirm}
                isPending={cerrarMutation.isPending}
            />
        </div>
    )
}
```

---

## 7. Flujo de Datos

```
User Action (click, submit)
    ↓
Component (presentation) - NecesidadCard, NecesidadForm, etc.
    ↓
Hook (application) - useCreateNecesidad, useMisNecesidades
    ↓
Service (application) - necesidadService.create()
    ↓
API Client (infrastructure) - apiFetch wrapper
    ↓
Backend API (external) - POST /api/crowdsourcing/necesidades
    ↓
Response
    ↓
Service unwraps data
    ↓
Hook handles success/error (toast, invalidate queries, redirect)
    ↓
Component re-renders with updated data
```

**React Query Cache Flow:**
1. Query ejecuta `queryFn` (service call)
2. Datos cacheados con `queryKey`
3. Mutation invalida queries relacionadas
4. React Query refetch automatico de queries activas
5. UI re-render con datos actualizados

---

## 8. Dependencias de Shared

**Importar de `@shared/`:**

**Types:**
- `NecesidadCrowdsourcingList`
- `NecesidadCrowdsourcing`
- `PropuestaCrowdsourcing`
- `CreateNecesidadRequest`
- `UpdateNecesidadRequest`
- `CerrarNecesidadRequest`
- `NecesidadCreateResult`
- `NecesidadUpdateResult`
- `CerrarNecesidadResult`
- `ServiceResponse<T>`
- `PaginatedResponse<T>`

**Schemas:**
- `createNecesidadSchema`
- `updateNecesidadSchema`
- `cerrarNecesidadSchema`
- Tipos inferidos: `CreateNecesidadFormData`, `UpdateNecesidadFormData`, `CerrarNecesidadFormData`

**Constants:**
- `QUERY_KEYS.crowdsourcing.necesidades.*`
- `API_ROUTES.crowdsourcing.necesidades.*`
- `APP_ROUTES.dashboard.crowdsourcing.*`
- `ESTADO_NECESIDAD` (numeric IDs)
- `ESTADO_NECESIDAD_LABELS` (labels)
- `ESTADO_NECESIDAD_BADGES` (colors)
- `MODALIDAD_TRABAJO` (numeric IDs)
- `MODALIDAD_TRABAJO_LABELS`
- `MODALIDAD_TRABAJO_ICONS`
- `MONEDA` (numeric IDs)
- `MONEDA_SIMBOLOS`

**Utils:**
- `formatPresupuesto(min?, max?, monedaId?)`
- `getNecesidadErrorMessage(errorCode)`

---

## 9. Archivos a Crear

### Paginas (4 archivos)

| Archivo | Tipo | Lineas Est. |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/page.tsx` | Page | ~120 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/nueva/page.tsx` | Page | ~50 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/[id]/page.tsx` | Page | ~90 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/[id]/editar/page.tsx` | Page | ~80 |

### Componentes (12 archivos)

| Archivo | Tipo | Lineas Est. |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/NecesidadCard.tsx` | Component | ~100 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/NecesidadFilters.tsx` | Component | ~60 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/EmptyStateNecesidades.tsx` | Component | ~50 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/form/NecesidadForm.tsx` | Component | ~300 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/form/UbicacionFields.tsx` | Component | ~40 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/form/PresupuestoFields.tsx` | Component | ~70 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/detail/NecesidadDetailHeader.tsx` | Component | ~60 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/detail/NecesidadDetailsCard.tsx` | Component | ~80 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/detail/PropuestasSection.tsx` | Component | ~70 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/detail/PropuestaCard.tsx` | Component | ~90 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/shared/EstadoNecesidadBadge.tsx` | Component | ~30 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/shared/CerrarNecesidadDialog.tsx` | Component | ~80 |

### Hooks (7 archivos)

| Archivo | Tipo | Lineas Est. |
|---------|------|-------------|
| `src/admin/src/hooks/use-necesidades.ts` | Hook | ~60 |
| `src/admin/src/hooks/use-necesidades-mutations.ts` | Hook | ~100 |
| `src/admin/src/hooks/use-tipos-necesidad.ts` | Hook | ~15 |
| `src/admin/src/hooks/use-modalidades-trabajo.ts` | Hook | ~15 |
| `src/admin/src/hooks/use-monedas.ts` | Hook | ~15 |
| `src/admin/src/hooks/use-debounce.ts` | Hook | ~20 (si no existe) |
| `src/admin/src/hooks/index.ts` | Barrel | ~5 (modificar) |

### Services (1 archivo)

| Archivo | Tipo | Lineas Est. |
|---------|------|-------------|
| `src/admin/src/services/necesidad.service.ts` | Service | ~80 |

### Tests (6 archivos minimo)

| Archivo | Tipo | Lineas Est. |
|---------|------|-------------|
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/__tests__/NecesidadCard.test.tsx` | Test | ~100 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/list/__tests__/NecesidadFilters.test.tsx` | Test | ~60 |
| `src/admin/src/app/(dashboard)/crowdsourcing/necesidades/components/shared/__tests__/EstadoNecesidadBadge.test.tsx` | Test | ~40 |
| `src/admin/src/hooks/__tests__/use-necesidades.test.ts` | Test | ~80 |
| `src/admin/src/hooks/__tests__/use-necesidades-mutations.test.ts` | Test | ~120 |
| `src/admin/src/services/__tests__/necesidad.service.test.ts` | Test | ~100 |

**Total estimado:** ~2,100 lineas de codigo (sin contar tests)

---

## 10. Checklist

### Pre-implementacion
- [ ] Shared contracts implementados (types, schemas, constants, utils)
- [ ] Backend API endpoints funcionales y testeados
- [ ] Maestras de TipoNecesidad, ModalidadTrabajo, Moneda seed en DB
- [ ] ProyectoArtistico entity y endpoints disponibles

### Componentes
- [ ] Todos los componentes usan shadcn/ui (Card, Badge, Button, Input, Select, Dialog, Alert)
- [ ] EstadoNecesidadBadge con colores correctos segun estado
- [ ] NecesidadCard con hover effect y actions condicionales
- [ ] NecesidadForm con validacion Zod en tiempo real
- [ ] Campos condicionales: ubicacion (modalidad), moneda (presupuesto)
- [ ] CerrarNecesidadDialog con warning alert y textarea motivo
- [ ] PropuestaCard con mensaje expandible y actions
- [ ] EmptyStateNecesidades con dos variantes (sin filtros, con filtros)
- [ ] Todos los componentes con ARIA labels y focus states

### Hooks
- [ ] useMisNecesidades con paginacion y filtros (estado, search)
- [ ] useNecesidad con fetch by ID
- [ ] useCreateNecesidad con toast success, invalidate queries, redirect
- [ ] useUpdateNecesidad con toast, invalidate queries (listado + detalle)
- [ ] useCerrarNecesidad con toast contando propuestas rechazadas
- [ ] useTiposNecesidad, useModalidadesTrabajo, useMonedas con staleTime Infinity
- [ ] useDebounce para search input (300ms)

### Services
- [ ] necesidadService con metodos: getMisNecesidades, getById, create, update, cerrar
- [ ] Query params correctos en getMisNecesidades (page, pageSize, estado, search)
- [ ] Error handling delegado a apiFetch
- [ ] Data unwrapping (retornar response.data directamente)

### Paginas
- [ ] Listado con grid responsive (1/2/3 cols), filtros, paginacion
- [ ] Nueva con NecesidadForm en modo create
- [ ] Editar con NecesidadForm en modo edit, datos pre-rellenados, banner si tiene propuestas
- [ ] Detalle con header, detalles, propuestas, actions condicionales
- [ ] Todas las paginas con loading skeletons
- [ ] Todas las paginas con error handling (404, 403)

### Routing
- [ ] Navegacion correcta entre paginas
- [ ] Back links funcionales
- [ ] Redirect tras success (create -> listado, update -> detalle)
- [ ] Redirect si estado != Abierta en editar

### Validacion
- [ ] React Hook Form + Zod resolver en NecesidadForm
- [ ] Mensajes de error en español, alineados con backend
- [ ] Validacion en tiempo real (onBlur)
- [ ] Refines de Zod funcionando: presupuesto, moneda, ubicacion, fechas
- [ ] Character counters en descripcion y motivo cierre

### UX
- [ ] Toast notifications en todas las acciones (create, update, cerrar)
- [ ] Loading states con spinner en botones
- [ ] Disabled states en forms durante submit
- [ ] Dialog confirmacion al cerrar necesidad
- [ ] Fecha limite con warning si < 7 dias
- [ ] Botones de accion condicionales segun estado
- [ ] Empty states con ilustraciones y CTAs

### Testing
- [ ] Tests unitarios de componentes clave (NecesidadCard, NecesidadFilters, EstadoNecesidadBadge)
- [ ] Tests de hooks (use-necesidades, use-necesidades-mutations)
- [ ] Tests de service (necesidad.service)
- [ ] Cobertura minima 80%

---

## 11. Siguiente Paso Sugerido

**Orden de implementacion:**

1. **Crear archivos base:**
   - Service: `necesidad.service.ts`
   - Hooks: `use-necesidades.ts`, `use-necesidades-mutations.ts`, maestras hooks

2. **Componentes shared:**
   - `EstadoNecesidadBadge.tsx`
   - `ModalidadBadge.tsx` (si no existe)
   - `CerrarNecesidadDialog.tsx`

3. **Componentes lista:**
   - `NecesidadCard.tsx`
   - `NecesidadFilters.tsx`
   - `EmptyStateNecesidades.tsx`

4. **Pagina listado:**
   - `page.tsx` (listado mis necesidades)
   - Probar filtros, paginacion, empty state

5. **Componentes form:**
   - `NecesidadForm.tsx`
   - `UbicacionFields.tsx`
   - `PresupuestoFields.tsx`

6. **Paginas form:**
   - `nueva/page.tsx`
   - `[id]/editar/page.tsx`
   - Probar create y update

7. **Componentes detalle:**
   - `NecesidadDetailHeader.tsx`
   - `NecesidadDetailsCard.tsx`
   - `PropuestaCard.tsx`
   - `PropuestasSection.tsx`

8. **Pagina detalle:**
   - `[id]/page.tsx`
   - Probar detalle con propuestas

9. **Testing:**
   - Tests unitarios de componentes
   - Tests de hooks y service

**Dependencias bloqueantes:**
- Shared contracts deben estar implementados primero
- Backend API debe estar funcional para probar
- Maestras seed en DB para selects

---

**Fin del Plan Frontend Admin - Gestionar Necesidades**
