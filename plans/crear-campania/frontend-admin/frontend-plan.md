# Plan Frontend: Crear Campania (Admin Dashboard)

**Fecha:** 2026-02-12
**Feature:** crear-campania (US-02)
**Target:** Admin Dashboard (Next.js 14)
**Basado en:** feature-spec.md, contracts.md, ui-ux.md, shared/contracts-plan.md

---

## 1. Resumen

- **Screens:** 4 (Wizard 4 pasos, Vista Previa, Mis Campanias, Editar)
- **Componentes:** 18 (Wizard stepper, steps forms, list cards, preview layout, modals)
- **Hooks:** 8 (queries, mutations, wizard state management)
- **Services:** 1 (campaniaService - ya existe, requiere actualizacion menor)
- **Pages:** 4 Next.js pages

**Arquitectura:** Next.js App Router con feature-based components dentro de `(dashboard)/campanias/`

---

## 2. Estructura de Carpetas

```
src/admin/src/
├── app/
│   └── (dashboard)/
│       └── campanias/
│           ├── page.tsx                          # Lista "Mis Campanias"
│           ├── nueva/
│           │   └── page.tsx                      # Wizard crear campania (client component)
│           ├── [id]/
│           │   ├── page.tsx                      # Vista previa (con banner borrador)
│           │   └── editar/
│           │       └── page.tsx                  # Wizard editar (pre-completado)
│           └── components/
│               ├── wizard/
│               │   ├── WizardStepper.tsx         # Stepper horizontal (1/4, 2/4, etc)
│               │   ├── BasicInfoStep.tsx         # Paso 1: Titulo, meta, fecha, imagen
│               │   ├── StoryStep.tsx             # Paso 2: Subtitulo, descripcion rica
│               │   ├── RewardsStep.tsx           # Paso 3: Lista rewards (opcional)
│               │   ├── ReviewStep.tsx            # Paso 4: Revision final
│               │   └── WizardContainer.tsx       # State manager del wizard
│               ├── list/
│               │   ├── CampaniaListCard.tsx      # Card en lista (imagen, titulo, progress, badges)
│               │   ├── CampaniaStatusBadge.tsx   # Badge de estado (Borrador/Publicada/etc)
│               │   └── CampaniaActions.tsx       # Botones Editar/Ver/Publicar/Menu
│               ├── preview/
│               │   ├── DraftBanner.tsx           # Banner warning "Campania en borrador"
│               │   ├── PreviewLayout.tsx         # Layout de preview (similar a landing)
│               │   └── PublishConfirmModal.tsx   # Modal confirmar publicacion
│               └── shared/
│                   ├── RichTextEditor.tsx        # TipTap editor para descripcion
│                   ├── ImageUploadField.tsx      # Upload con preview
│                   └── EmptyState.tsx            # Empty state para lista vacia
│
├── hooks/
│   ├── use-campanias.ts                          # ACTUALIZAR - agregar nuevos hooks
│   └── use-wizard-state.ts                      # CREAR - State manager wizard
│
├── services/
│   └── campania.service.ts                       # ACTUALIZAR - metodos ya existen
│
└── lib/
    └── api-client.ts                             # Axios client con auth (ya existe)
```

---

## 3. Componentes

### 3.1 WizardStepper

**Archivo:** `app/(dashboard)/campanias/components/wizard/WizardStepper.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| currentStep | number | Si | Paso actual (1-4) |
| totalSteps | number | Si | Total de pasos (4) |
| completedSteps | number[] | Si | Array de pasos completados |
| onStepClick | (step: number) => void | No | Callback al hacer click en un paso completado |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: ninguno (custom styling con Tailwind)

**Responsabilidad:**
Renderiza stepper horizontal visual que muestra progreso del wizard. Paso activo tiene gradient pink-purple, pasos completados tienen check icon, pasos futuros son gray. Conectores entre pasos se llenan con gradient al avanzar.

**Estructura:**
```tsx
"use client"

interface WizardStepperProps {
  currentStep: number
  totalSteps: number
  completedSteps: number[]
  onStepClick?: (step: number) => void
}

export function WizardStepper({ currentStep, totalSteps, completedSteps, onStepClick }: WizardStepperProps) {
  const steps = [
    { number: 1, label: "Informacion Basica" },
    { number: 2, label: "Historia" },
    { number: 3, label: "Recompensas" },
    { number: 4, label: "Revision" },
  ]

  return (
    <div className="flex items-center justify-between max-w-2xl mx-auto mb-10" role="progressbar" aria-valuenow={currentStep} aria-valuemin={1} aria-valuemax={totalSteps} aria-label="Progreso de creacion de campania">
      {steps.map((step, index) => {
        const isActive = step.number === currentStep
        const isCompleted = completedSteps.includes(step.number)
        const isClickable = isCompleted && onStepClick

        return (
          <Fragment key={step.number}>
            <div className="flex flex-col items-center">
              <button
                type="button"
                onClick={() => isClickable && onStepClick(step.number)}
                disabled={!isClickable}
                className={cn(
                  "w-12 h-12 rounded-full flex items-center justify-center font-bold transition-all",
                  isActive && "bg-gradient-to-r from-pink-500 to-purple-600 text-white shadow-lg",
                  isCompleted && !isActive && "bg-gradient-to-r from-pink-500 to-purple-600 text-white",
                  !isActive && !isCompleted && "bg-muted text-muted-foreground",
                  isClickable && "cursor-pointer hover:scale-105"
                )}
                aria-current={isActive ? "step" : undefined}
              >
                {isCompleted ? <Check className="w-6 h-6" /> : step.number}
              </button>
              <span className={cn(
                "text-sm mt-2 text-center",
                isActive ? "text-foreground font-medium" : "text-muted-foreground"
              )}>
                {step.label}
              </span>
            </div>

            {index < steps.length - 1 && (
              <div className="flex-1 h-0.5 bg-muted mx-2">
                {(isCompleted || (completedSteps.includes(step.number) && completedSteps.includes(step.number + 1))) && (
                  <div className="h-full bg-gradient-to-r from-pink-500 to-purple-600 transition-all duration-400" />
                )}
              </div>
            )}
          </Fragment>
        )
      })}
    </div>
  )
}
```

---

### 3.2 BasicInfoStep

**Archivo:** `app/(dashboard)/campanias/components/wizard/BasicInfoStep.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| defaultValues | Partial\<BasicInfoFormData\> | No | Valores iniciales para editar |
| onNext | (data: BasicInfoFormData) => void | Si | Callback al avanzar con datos validados |
| onBack | () => void | No | Callback al retroceder (no aplica en paso 1) |
| isSubmitting | boolean | No | Estado de loading |

**Estado Local:**
- React Hook Form state (titulo, importeObjetivo, monedaId, tipoFinanciacionId, fechaFin, imagenPrincipalUrl, videoPrincipalUrl)

**Dependencias:**
- Hooks: useForm (react-hook-form), zodResolver
- Schemas: campaniaBasicInfoSchema + campaniaFundingSchema + campaniaDurationSchema + campaniaMediaSchema (merged)
- Componentes UI: Card, Input, Label, Select, DatePicker (shadcn), ImageUploadField (custom)

**Responsabilidad:**
Formulario paso 1 con validacion Zod. Campos: titulo (max 200), importeObjetivo (min 100), monedaId (default 1 EUR), tipoFinanciacionId (select), fechaFin (date picker con minDate +7 days), imagenPrincipalUrl (upload), videoPrincipalUrl (URL input opcional). Valida antes de permitir avanzar.

**Notas:**
- Merge de schemas por step para crear un "BasicInfoStep" completo segun UI/UX spec
- Date picker deshabilita fechas < today + 7 days y > today + 60 days
- Upload de imagen muestra preview

---

### 3.3 StoryStep

**Archivo:** `app/(dashboard)/campanias/components/wizard/StoryStep.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| defaultValues | Partial\<StoryFormData\> | No | Valores iniciales |
| onNext | (data: StoryFormData) => void | Si | Callback con datos validados |
| onBack | () => void | Si | Callback retroceder a paso 1 |
| isSubmitting | boolean | No | Loading state |

**Estado Local:**
- React Hook Form state (subtitulo, descripcionCorta)

**Dependencias:**
- Hooks: useForm
- Schemas: campaniaBasicInfoSchema (subset: subtitulo, descripcionCorta)
- Componentes: RichTextEditor (TipTap), Input, Textarea, CharacterCounter (custom)

**Responsabilidad:**
Paso 2 del wizard. Campos: subtitulo (opcional, max 300 chars), descripcionCorta (rich text, max 500 chars). Rich text editor con toolbar (bold, italic, lists, links). Character counter con color warning a 80%, error a 95%. Valida longitudes antes de avanzar.

**Notas:**
- RichTextEditor retorna HTML sanitizado
- Descripcion min 50 chars para avanzar

---

### 3.4 RewardsStep

**Archivo:** `app/(dashboard)/campanias/components/wizard/RewardsStep.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campaniaId | string | No | ID de campania si existe (modo editar) |
| onNext | () => void | Si | Callback avanzar (sin validacion, rewards opcional) |
| onBack | () => void | Si | Callback retroceder |

**Estado Local:**
- Ninguno (este paso es opcional, solo navega a rewards management)

**Dependencias:**
- Componentes: Card, Button, Alert (tip box)

**Responsabilidad:**
Paso 3 del wizard. Muestra lista de rewards ya creados (si existen) y boton "Agregar recompensa". En MVP simplificado, permite avanzar sin rewards (se muestra advertencia en paso 4). Si hay campaniaId, muestra rewards asociados. Boton "Siguiente" siempre habilitado.

**Notas:**
- CRUD de rewards es feature separada (US-03)
- Este paso es placeholder que permite continuar
- Muestra empty state si no hay rewards

---

### 3.5 ReviewStep

**Archivo:** `app/(dashboard)/campanias/components/wizard/ReviewStep.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| formData | CreateCampaniaFormData | Si | Datos acumulados de todos los pasos |
| onSubmit | () => void | Si | Callback crear campania |
| onBack | () => void | Si | Callback retroceder |
| onEdit | (step: number) => void | Si | Callback para editar paso especifico |
| isSubmitting | boolean | No | Loading state |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Card, Button, Alert (warning box), Badge

**Responsabilidad:**
Paso 4 - Revision final. Muestra preview de todos los datos: imagen de portada, titulo, meta, fecha fin, descripcion (primeras 3 lineas), resumen de rewards. Link "Editar" por seccion para volver al paso correspondiente. Warning box "Tu campania se creara como BORRADOR". Botones: "Guardar como borrador" (outline) y "Crear campania" (gradient). Ambos llaman a onSubmit.

---

### 3.6 WizardContainer

**Archivo:** `app/(dashboard)/campanias/components/wizard/WizardContainer.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| mode | "create" \| "edit" | Si | Modo wizard (crear o editar) |
| campaniaId | string | No | ID si modo editar |
| initialData | Partial\<CreateCampaniaFormData\> | No | Datos iniciales si editar |

**Estado Local:**
- currentStep: number (1-4)
- formData: Partial\<CreateCampaniaFormData\>
- completedSteps: number[]

**Dependencias:**
- Hooks: useWizardState, useCreateCampania, useUpdateCampania
- Componentes: WizardStepper, BasicInfoStep, StoryStep, RewardsStep, ReviewStep

**Responsabilidad:**
Componente contenedor que gestiona estado global del wizard. Controla navegacion entre pasos, acumula datos de cada step, maneja submit final (create o update), invalida queries al completar. Persiste progreso en localStorage para evitar perdida al recargar (opcional).

**Flujo:**
1. Usuario completa BasicInfoStep → onNext guarda data en formData, avanza a step 2
2. Usuario completa StoryStep → onNext acumula data, avanza a step 3
3. Usuario navega RewardsStep (opcional) → onNext avanza a step 4
4. Usuario revisa en ReviewStep → onSubmit crea campania y redirige

**Estructura:**
```tsx
"use client"

export function WizardContainer({ mode, campaniaId, initialData }: WizardContainerProps) {
  const { currentStep, formData, goToStep, updateFormData, completedSteps } = useWizardState(initialData)
  const createMutation = useCreateCampania()
  const updateMutation = useUpdateCampania()
  const router = useRouter()

  const handleStepComplete = (step: number, data: any) => {
    updateFormData(data)
    goToStep(step + 1)
  }

  const handleSubmit = async () => {
    try {
      if (mode === "create") {
        const result = await createMutation.mutateAsync(formData as CreateCampaniaRequest)
        toast.success("Campania creada exitosamente")
        router.push(`/dashboard/campanias/${result.id}`)
      } else {
        await updateMutation.mutateAsync({ id: campaniaId!, data: formData })
        toast.success("Campania actualizada")
        router.push(`/dashboard/campanias/${campaniaId}`)
      }
    } catch (error) {
      toast.error(getCampaniaErrorMessage(error.errorCode))
    }
  }

  return (
    <div className="max-w-4xl mx-auto py-8 px-6">
      <div className="text-center mb-8">
        <h1 className="text-3xl font-bold text-foreground mb-2">
          {mode === "create" ? "Crear Nueva Campania" : "Editar Campania"}
        </h1>
        <p className="text-muted-foreground">
          Completa los pasos para publicar tu proyecto musical
        </p>
      </div>

      <WizardStepper
        currentStep={currentStep}
        totalSteps={4}
        completedSteps={completedSteps}
        onStepClick={goToStep}
      />

      {currentStep === 1 && (
        <BasicInfoStep
          defaultValues={formData}
          onNext={(data) => handleStepComplete(1, data)}
          isSubmitting={false}
        />
      )}

      {currentStep === 2 && (
        <StoryStep
          defaultValues={formData}
          onNext={(data) => handleStepComplete(2, data)}
          onBack={() => goToStep(1)}
          isSubmitting={false}
        />
      )}

      {currentStep === 3 && (
        <RewardsStep
          campaniaId={campaniaId}
          onNext={() => goToStep(4)}
          onBack={() => goToStep(2)}
        />
      )}

      {currentStep === 4 && (
        <ReviewStep
          formData={formData as CreateCampaniaFormData}
          onSubmit={handleSubmit}
          onBack={() => goToStep(3)}
          onEdit={goToStep}
          isSubmitting={createMutation.isPending || updateMutation.isPending}
        />
      )}
    </div>
  )
}
```

---

### 3.7 CampaniaListCard

**Archivo:** `app/(dashboard)/campanias/components/list/CampaniaListCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campania | CampaniaListItem | Si | Datos de campania para mostrar |
| onEdit | (id: string) => void | Si | Callback editar |
| onView | (id: string) => void | Si | Callback ver detalle |
| onPublish | (id: string) => void | Si | Callback publicar |
| onDelete | (id: string) => void | Si | Callback eliminar |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Card, Button, DropdownMenu, Progress
- Componentes custom: CampaniaStatusBadge, CampaniaActions

**Responsabilidad:**
Card de campania en lista "Mis Campanias". Layout horizontal: imagen (w-24 h-24), info (titulo, status badge, progress bar mini, backers count), acciones (Editar/Ver/Menu). Hover effect cambia border a primary. Progress bar usa gradient. Click en card abre detalle.

**Estructura:**
```tsx
<Card className="hover:border-primary transition cursor-pointer" onClick={() => onView(campania.id)}>
  <CardContent className="flex items-center gap-6 p-6">
    <img src={campania.imagenPrincipalUrl || "/placeholder.jpg"} className="w-24 h-24 object-cover rounded-lg" />
    <div className="flex-1">
      <h3 className="text-lg font-bold text-foreground mb-2">{campania.titulo}</h3>
      <div className="flex items-center gap-3 mb-2">
        <CampaniaStatusBadge estadoId={campania.estadoCampaniaId} />
        <span className="text-sm text-muted-foreground">
          {formatCurrencyWithSymbol(campania.importePledgedActual, campania.monedaId)} de {formatCurrencyWithSymbol(campania.importeObjetivo, campania.monedaId)}
        </span>
      </div>
      <Progress value={calcularPorcentaje(campania)} className="h-2" />
    </div>
    <CampaniaActions
      campaniaId={campania.id}
      estado={campania.estadoCampaniaId}
      onEdit={onEdit}
      onView={onView}
      onPublish={onPublish}
      onDelete={onDelete}
    />
  </CardContent>
</Card>
```

---

### 3.8 CampaniaStatusBadge

**Archivo:** `app/(dashboard)/campanias/components/list/CampaniaStatusBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| estadoId | number | Si | ID del estado (1=Borrador, 2=Publicada, etc) |
| className | string | No | Clases adicionales |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Badge
- Constants: CAMPANIA_ESTADOS, CAMPANIA_ESTADOS_LABELS, CAMPANIA_ESTADO_COLORS

**Responsabilidad:**
Badge visual que muestra estado de campania con color semantico. Borrador=yellow, Publicada=green, Finalizada=blue, Cancelada=red. Usa constants de shared para labels y colors.

**Estructura:**
```tsx
"use client"

interface CampaniaStatusBadgeProps {
  estadoId: number
  className?: string
}

export function CampaniaStatusBadge({ estadoId, className }: CampaniaStatusBadgeProps) {
  const label = CAMPANIA_ESTADOS_LABELS[estadoId] || "Desconocido"
  const colorMap = {
    1: "bg-yellow-500/20 text-yellow-400 border-yellow-500/50", // Borrador
    2: "bg-green-500/20 text-green-400 border-green-500/50",    // Publicada
    3: "bg-blue-500/20 text-blue-400 border-blue-500/50",       // Finalizada
    4: "bg-red-500/20 text-red-400 border-red-500/50",          // Cancelada
  }

  return (
    <Badge variant="outline" className={cn(colorMap[estadoId], className)} role="status" aria-label={`Estado de campania: ${label}`}>
      {label}
    </Badge>
  )
}
```

---

### 3.9 CampaniaActions

**Archivo:** `app/(dashboard)/campanias/components/list/CampaniaActions.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campaniaId | string | Si | ID de campania |
| estado | number | Si | Estado actual |
| onEdit | (id: string) => void | Si | Callback editar |
| onView | (id: string) => void | Si | Callback ver |
| onPublish | (id: string) => void | Si | Callback publicar |
| onDelete | (id: string) => void | Si | Callback eliminar |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Button, DropdownMenu
- Constants: CAMPANIA_ESTADOS

**Responsabilidad:**
Botones de accion para cada campania. Muestra "Editar" solo si estado=Borrador, "Publicar" solo si estado=Borrador, "Ver" siempre. Dropdown menu con opciones adicionales (Eliminar, Archivar).

**Estructura:**
```tsx
<div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
  {estado === CAMPANIA_ESTADOS.BORRADOR && (
    <>
      <Button variant="outline" size="sm" onClick={() => onEdit(campaniaId)}>
        Editar
      </Button>
      <Button variant="outline" size="sm" onClick={() => onPublish(campaniaId)} className="border-green-500/50 text-green-400">
        Publicar
      </Button>
    </>
  )}
  <Button variant="outline" size="sm" onClick={() => onView(campaniaId)}>
    Ver
  </Button>
  <DropdownMenu>
    <DropdownMenuTrigger asChild>
      <Button variant="ghost" size="icon">
        <MoreVertical className="h-4 w-4" />
      </Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent align="end">
      <DropdownMenuItem onClick={() => onDelete(campaniaId)}>
        <Trash2 className="mr-2 h-4 w-4" />
        Eliminar
      </DropdownMenuItem>
    </DropdownMenuContent>
  </DropdownMenu>
</div>
```

---

### 3.10 DraftBanner

**Archivo:** `app/(dashboard)/campanias/components/preview/DraftBanner.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| onClose | () => void | No | Callback cerrar banner (opcional) |

**Estado Local:**
- isVisible: boolean (gestiona visibilidad local)

**Dependencias:**
- Componentes UI: Alert, Button

**Responsabilidad:**
Banner warning que aparece en vista previa de campania en estado BORRADOR. Background warning/10, texto warning, border warning. Boton X para cerrar que persiste en localStorage. Texto: "VISTA PREVIA - Campania en borrador (No visible publicamente)".

---

### 3.11 PreviewLayout

**Archivo:** `app/(dashboard)/campanias/components/preview/PreviewLayout.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| campania | Campania | Si | Datos completos de campania |
| onEdit | () => void | Si | Callback editar |
| onPublish | () => void | Si | Callback publicar |
| onDelete | () => void | Si | Callback eliminar |
| showDraftBanner | boolean | Si | Mostrar banner borrador |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes: DraftBanner, Button, Card
- Layout similar a CampaniaDetailPage de landing (reutilizar estructura)

**Responsabilidad:**
Layout de preview de campania. Si showDraftBanner=true, muestra DraftBanner arriba. Action bar con botones Editar/Publicar/Eliminar. Contenido igual a pagina publica de detalle: hero image, titulo, meta, progress bar, descripcion, tabs (Historia/Actualizaciones/Comentarios). Sidebar con rewards.

---

### 3.12 PublishConfirmModal

**Archivo:** `app/(dashboard)/campanias/components/preview/PublishConfirmModal.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| isOpen | boolean | Si | Estado modal |
| onClose | () => void | Si | Callback cerrar |
| onConfirm | () => void | Si | Callback confirmar publicacion |
| hasRewards | boolean | Si | Si campania tiene rewards |
| isPublishing | boolean | No | Loading state |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Dialog, Button, Alert

**Responsabilidad:**
Modal de confirmacion antes de publicar campania. Si hasRewards=false, muestra advertencia "No has creado recompensas. ¿Continuar sin recompensas?" con opciones "Agregar recompensas" (redirect a rewards) o "Publicar sin recompensas". Si hasRewards=true, confirmacion simple "¿Publicar campania ahora? Una vez publicada, no podras editarla.".

---

### 3.13 RichTextEditor

**Archivo:** `app/(dashboard)/campanias/components/shared/RichTextEditor.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| value | string | No | HTML content inicial |
| onChange | (html: string) => void | Si | Callback con HTML actualizado |
| placeholder | string | No | Placeholder text |
| maxLength | number | No | Max caracteres permitidos |
| className | string | No | Clases adicionales |

**Estado Local:**
- TipTap editor instance
- characterCount: number

**Dependencias:**
- TipTap: useEditor, EditorContent, StarterKit extensions
- Componentes UI: Button (toolbar)

**Responsabilidad:**
Editor de texto rico con toolbar. Extensiones: bold, italic, underline, bulletList, orderedList, link. Toolbar con botones para cada formato. Character counter en footer. Estilos dark theme. Sanitiza HTML output.

**Notas:**
- Instalar `@tiptap/react`, `@tiptap/starter-kit`, `@tiptap/extension-link`
- Usar DOMPurify para sanitizar HTML (instalar `dompurify` y `@types/dompurify`)

---

### 3.14 ImageUploadField

**Archivo:** `app/(dashboard)/campanias/components/shared/ImageUploadField.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| value | string | No | URL de imagen actual |
| onChange | (url: string) => void | Si | Callback con nueva URL |
| label | string | No | Label del campo |
| accept | string | No | Tipos de archivo aceptados (default: image/*) |
| maxSize | number | No | Tamano max en MB (default: 5) |

**Estado Local:**
- isUploading: boolean
- previewUrl: string | null
- dragActive: boolean

**Dependencias:**
- Componentes UI: Label, Button
- Icons: CloudUpload, X

**Responsabilidad:**
Campo de upload de imagen con drag & drop. Valida formato (PNG, JPG) y tamano (max 5MB). Muestra preview de imagen. Area dashed border que cambia a primary al hover/drag. Boton X para remover imagen. En MVP simplificado, onChange retorna URL (el upload a storage es scope aparte, por ahora acepta URL directa).

**Notas:**
- En MVP, usuario pega URL directa
- Futuro: integrar con upload service (Azure Blob Storage)

---

### 3.15 EmptyState

**Archivo:** `app/(dashboard)/campanias/components/shared/EmptyState.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| icon | ReactNode | Si | Icono a mostrar |
| title | string | Si | Titulo del empty state |
| description | string | Si | Descripcion |
| actionLabel | string | No | Label boton CTA |
| onAction | () => void | No | Callback CTA |

**Estado Local:**
- Ninguno (stateless)

**Dependencias:**
- Componentes UI: Button

**Responsabilidad:**
Componente generico para estados vacios. Centra verticalmente icono grande (gray), titulo, descripcion y boton CTA opcional. Usado en lista de campanias vacia, rewards vacio, etc.

---

## 4. Hooks

### 4.1 useWizardState

**Archivo:** `hooks/use-wizard-state.ts`

**Tipo:** Custom State Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| initialData | Partial\<CreateCampaniaFormData\> | Datos iniciales (modo editar) |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| currentStep | number | Paso actual (1-4) |
| formData | Partial\<CreateCampaniaFormData\> | Datos acumulados |
| completedSteps | number[] | Array de pasos completados |
| goToStep | (step: number) => void | Navegar a paso especifico |
| updateFormData | (data: any) => void | Actualizar formData |
| resetWizard | () => void | Resetear wizard |

**Responsabilidad:**
Hook personalizado que gestiona estado del wizard. Persiste formData en localStorage para evitar perdida al recargar. Trackea pasos completados. Valida que solo se pueda navegar a pasos ya completados.

**Estructura:**
```tsx
export function useWizardState(initialData?: Partial<CreateCampaniaFormData>) {
  const [currentStep, setCurrentStep] = useState(1)
  const [formData, setFormData] = useState<Partial<CreateCampaniaFormData>>(initialData || {})
  const [completedSteps, setCompletedSteps] = useState<number[]>([])

  useEffect(() => {
    // Cargar desde localStorage si existe
    const saved = localStorage.getItem("wizard-draft")
    if (saved && !initialData) {
      try {
        const parsed = JSON.parse(saved)
        setFormData(parsed.formData)
        setCurrentStep(parsed.currentStep)
        setCompletedSteps(parsed.completedSteps)
      } catch (e) {
        console.error("Failed to parse wizard draft", e)
      }
    }
  }, [])

  useEffect(() => {
    // Guardar en localStorage al cambiar
    localStorage.setItem("wizard-draft", JSON.stringify({
      formData,
      currentStep,
      completedSteps,
    }))
  }, [formData, currentStep, completedSteps])

  const goToStep = (step: number) => {
    if (step <= completedSteps.length + 1 || completedSteps.includes(step)) {
      setCurrentStep(step)
    }
  }

  const updateFormData = (data: any) => {
    setFormData((prev) => ({ ...prev, ...data }))
    if (!completedSteps.includes(currentStep)) {
      setCompletedSteps((prev) => [...prev, currentStep])
    }
  }

  const resetWizard = () => {
    setCurrentStep(1)
    setFormData({})
    setCompletedSteps([])
    localStorage.removeItem("wizard-draft")
  }

  return {
    currentStep,
    formData,
    completedSteps,
    goToStep,
    updateFormData,
    resetWizard,
  }
}
```

---

### 4.2 useCreateCampania (ACTUALIZAR)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Mutation Hook

**Parametros:**
- Ninguno (mutationFn recibe CreateCampaniaRequest)

**Retorna:**
- useMutation result (mutate, mutateAsync, isPending, error, data)

**Accion:**
Mantener implementacion existente, validar que usa CreateCampaniaRequest de shared (no CreateCampaniaDto deprecated).

---

### 4.3 useUpdateCampania (ACTUALIZAR)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Mutation Hook

**Accion:**
Mantener implementacion existente, validar que usa UpdateCampaniaRequest de shared.

---

### 4.4 usePublishCampania (YA EXISTE)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Mutation Hook

**Accion:**
Mantener implementacion existente. Este hook llama a `campaniaService.publicar(id)`.

---

### 4.5 useMisCampanias (YA EXISTE)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Query Hook

**Query Key:** `QUERY_KEYS.campanias.misCampanias` (de shared)

**Accion:**
Actualizar queryKey para usar constant de shared actualizado. Retorna CampaniaListItem[] en lugar de Campania[].

---

### 4.6 useCampania (YA EXISTE)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Query Hook

**Accion:**
Mantener implementacion existente. Usado para cargar campania en modo editar y preview.

---

### 4.7 useDeleteCampania (YA EXISTE)

**Archivo:** `hooks/use-campanias.ts`

**Tipo:** Mutation Hook

**Accion:**
Mantener implementacion existente. Invalida queries de lista al completar.

---

## 5. Services

### 5.1 campaniaService (ACTUALIZAR)

**Archivo:** `services/campania.service.ts`

**Metodos Existentes:** Mantener todos

**Cambios Necesarios:**
1. Actualizar tipos importados de shared (usar Campania, CreateCampaniaRequest, UpdateCampaniaRequest en lugar de deprecated)
2. Validar que endpoint `publicar(id)` usa ruta correcta: `/api/campanias/${id}/publicar`
3. Validar que `getMisCampanias()` usa endpoint correcto: `/api/campanias/mis-campanias`

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getAll | - | Campania[] | GET /api/campanias |
| getById | id: string | Campania \| null | GET /api/campanias/{id} |
| getMisCampanias | - | CampaniaListItem[] | GET /api/campanias/mis-campanias |
| create | data: CreateCampaniaRequest | Campania | POST /api/campanias |
| update | id: string, data: UpdateCampaniaRequest | Campania | PUT /api/campanias/{id} |
| publicar | id: string | PublishCampaniaResponse | POST /api/campanias/{id}/publicar |
| delete | id: string | void | DELETE /api/campanias/{id} |

**Estructura Actual (mantener, solo actualizar types):**
```typescript
import { apiFetch } from "@/lib/api-client"
import type {
  Campania,
  CampaniaListItem,
  CreateCampaniaRequest,
  UpdateCampaniaRequest,
  PublishCampaniaResponse,
  ServiceResponse
} from "@shared/types"

class CampaniaService {
  private readonly baseUrl = "/campanias"

  async getMisCampanias(): Promise<CampaniaListItem[]> {
    const response = await apiFetch<ServiceResponse<CampaniaListItem[]>>(`${this.baseUrl}/mis-campanias`)
    return response.data
  }

  // ... resto de metodos
}

export const campaniaService = new CampaniaService()
```

---

## 6. Pages (Next.js App Router)

### 6.1 `/dashboard/campanias/page.tsx` (Mis Campanias)

**Responsabilidad:**
Lista de campanias del artista autenticado. Usa useMisCampanias() para fetch. Renderiza CampaniaListCard por cada item. Boton "Nueva Campania" gradient top-right. EmptyState si no hay campanias. Loading skeletons. Error boundary.

**Estructura:**
```tsx
"use client"

export default function MisCampaniasPage() {
  const { data: campanias, isLoading } = useMisCampanias()
  const publishMutation = usePublishCampania()
  const deleteMutation = useDeleteCampania()
  const router = useRouter()

  const handlePublish = async (id: string) => {
    if (confirm("¿Publicar campania ahora?")) {
      await publishMutation.mutateAsync(id)
      toast.success("Campania publicada")
    }
  }

  const handleDelete = async (id: string) => {
    if (confirm("¿Eliminar campania?")) {
      await deleteMutation.mutateAsync(id)
      toast.success("Campania eliminada")
    }
  }

  if (isLoading) return <div>Loading skeletons...</div>

  if (!campanias?.length) {
    return (
      <EmptyState
        icon={<FolderOpen className="w-16 h-16" />}
        title="No tienes campanias aun"
        description="Crea tu primera campania para empezar a recaudar fondos"
        actionLabel="Crear tu primera campania"
        onAction={() => router.push("/dashboard/campanias/nueva")}
      />
    )
  }

  return (
    <div className="p-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold">Mis Campanias</h1>
        <Button onClick={() => router.push("/dashboard/campanias/nueva")} className="bg-gradient-to-r from-pink-500 to-purple-600">
          + Nueva campania
        </Button>
      </div>

      <div className="space-y-4">
        {campanias.map((campania) => (
          <CampaniaListCard
            key={campania.id}
            campania={campania}
            onEdit={(id) => router.push(`/dashboard/campanias/${id}/editar`)}
            onView={(id) => router.push(`/dashboard/campanias/${id}`)}
            onPublish={handlePublish}
            onDelete={handleDelete}
          />
        ))}
      </div>
    </div>
  )
}
```

---

### 6.2 `/dashboard/campanias/nueva/page.tsx` (Wizard Crear)

**Responsabilidad:**
Renderiza WizardContainer en modo "create". Redirige a preview al completar wizard.

**Estructura:**
```tsx
"use client"

export default function NuevaCampaniaPage() {
  return <WizardContainer mode="create" />
}
```

---

### 6.3 `/dashboard/campanias/[id]/page.tsx` (Vista Previa)

**Responsabilidad:**
Vista previa de campania. Si estado=BORRADOR, muestra DraftBanner y action bar (Editar/Publicar/Eliminar). Si estado=PUBLICADA, renderiza igual que landing publica pero con analytics extras. Usa useCampania(id) para fetch.

**Estructura:**
```tsx
"use client"

export default function CampaniaPreviewPage({ params }: { params: { id: string } }) {
  const { data: campania, isLoading } = useCampania(params.id)
  const publishMutation = usePublishCampania()
  const deleteMutation = useDeleteCampania()
  const router = useRouter()
  const [showPublishModal, setShowPublishModal] = useState(false)

  if (isLoading) return <div>Loading...</div>
  if (!campania) return <div>Campania no encontrada</div>

  const isBorrador = campania.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR

  const handlePublish = async () => {
    await publishMutation.mutateAsync(params.id)
    toast.success("Campania publicada exitosamente")
    setShowPublishModal(false)
    router.push("/dashboard/campanias")
  }

  return (
    <div>
      <PreviewLayout
        campania={campania}
        showDraftBanner={isBorrador}
        onEdit={() => router.push(`/dashboard/campanias/${params.id}/editar`)}
        onPublish={() => setShowPublishModal(true)}
        onDelete={async () => {
          if (confirm("¿Eliminar campania?")) {
            await deleteMutation.mutateAsync(params.id)
            router.push("/dashboard/campanias")
          }
        }}
      />

      <PublishConfirmModal
        isOpen={showPublishModal}
        onClose={() => setShowPublishModal(false)}
        onConfirm={handlePublish}
        hasRewards={false} // TODO: verificar si tiene rewards
        isPublishing={publishMutation.isPending}
      />
    </div>
  )
}
```

---

### 6.4 `/dashboard/campanias/[id]/editar/page.tsx` (Wizard Editar)

**Responsabilidad:**
Renderiza WizardContainer en modo "edit" con datos pre-cargados. Solo permite editar si estado=BORRADOR (backend valida tambien). Usa useCampania(id) para cargar initialData.

**Estructura:**
```tsx
"use client"

export default function EditarCampaniaPage({ params }: { params: { id: string } }) {
  const { data: campania, isLoading } = useCampania(params.id)

  if (isLoading) return <div>Loading...</div>
  if (!campania) return <div>Campania no encontrada</div>

  if (campania.estadoCampaniaId !== CAMPANIA_ESTADOS.BORRADOR) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>No se puede editar</AlertTitle>
        <AlertDescription>
          Solo se pueden editar campanias en estado borrador
        </AlertDescription>
      </Alert>
    )
  }

  return (
    <WizardContainer
      mode="edit"
      campaniaId={params.id}
      initialData={campania}
    />
  )
}
```

---

## 7. Flujo de Datos

### Crear Campania (Happy Path)

```
1. Usuario: Click "Nueva campania"
   └─> Navigate to /dashboard/campanias/nueva

2. WizardContainer: Render BasicInfoStep (step 1)
   └─> Usuario: Completa titulo, meta, fecha, imagen
       └─> Validacion Zod en tiempo real
       └─> Click "Siguiente"
           └─> useWizardState.updateFormData(step1Data)
           └─> goToStep(2)

3. WizardContainer: Render StoryStep (step 2)
   └─> Usuario: Escribe subtitulo, descripcion rica
       └─> RichTextEditor: onChange(html)
       └─> Click "Siguiente"
           └─> updateFormData(step2Data)
           └─> goToStep(3)

4. WizardContainer: Render RewardsStep (step 3)
   └─> Usuario: Click "Siguiente" (rewards opcional)
       └─> goToStep(4)

5. WizardContainer: Render ReviewStep (step 4)
   └─> Usuario: Revisa datos, click "Crear campania"
       └─> WizardContainer.handleSubmit()
           └─> useCreateCampania.mutateAsync(formData)
               └─> campaniaService.create(data)
                   └─> POST /api/campanias
                       └─> Backend: Crea campania en BORRADOR
                       └─> Response: { id: "...", estadoCampaniaId: 1, ... }
               └─> onSuccess: invalidate QUERY_KEYS.campanias.all + misCampanias
               └─> toast.success("Campania creada")
               └─> router.push(`/dashboard/campanias/${id}`)

6. CampaniaPreviewPage: Render preview con DraftBanner
   └─> Usuario: Click "Publicar Ahora"
       └─> PublishConfirmModal: Mostrar confirmacion
           └─> Usuario: Click "Confirmar"
               └─> usePublishCampania.mutateAsync(id)
                   └─> campaniaService.publicar(id)
                       └─> POST /api/campanias/{id}/publicar
                           └─> Backend: Cambia estado a PUBLICADA (2), establece fechaPublicacion
                   └─> onSuccess: invalidate queries
                   └─> toast.success("Campania publicada")
                   └─> router.push("/dashboard/campanias")

7. MisCampaniasPage: Lista actualizada con campania publicada
```

### Editar Campania (Borrador)

```
1. Usuario: Click "Editar" en CampaniaListCard
   └─> Navigate to /dashboard/campanias/{id}/editar

2. EditarCampaniaPage: useCampania(id) fetch data
   └─> Valida: estadoCampaniaId === BORRADOR
       └─> Si PUBLICADA: Muestra error "No se puede editar"
       └─> Si BORRADOR: Render WizardContainer(mode="edit", initialData)

3. WizardContainer: Pre-completa steps con initialData
   └─> Usuario: Modifica campos, avanza por steps
       └─> Click "Guardar cambios" en ReviewStep
           └─> useUpdateCampania.mutateAsync({ id, data })
               └─> campaniaService.update(id, data)
                   └─> PUT /api/campanias/{id}
                       └─> Backend: Actualiza campania (solo si BORRADOR)
               └─> onSuccess: invalidate queries
               └─> toast.success("Campania actualizada")
               └─> router.push(`/dashboard/campanias/${id}`)
```

### Eliminar Campania

```
1. Usuario: Click menu "..." en CampaniaListCard
   └─> Select "Eliminar"
       └─> Confirm dialog: "¿Eliminar campania?"
           └─> Usuario: Click "Eliminar"
               └─> useDeleteCampania.mutateAsync(id)
                   └─> campaniaService.delete(id)
                       └─> DELETE /api/campanias/{id}
                           └─> Backend: Soft delete o hard delete
                   └─> onSuccess: invalidate queries
                   └─> toast.success("Campania eliminada")
                   └─> Lista actualizada (item removido)
```

---

## 8. Dependencias de Shared

**Importar de `@shared/`:**

### Types
- `Campania`
- `CampaniaListItem`
- `CreateCampaniaRequest`
- `UpdateCampaniaRequest`
- `PublishCampaniaResponse`
- `EstadoCampania` (enum)
- `TipoFinanciacion` (enum)
- `ServiceResponse<T>`

### Schemas
- `campaniaBasicInfoSchema`
- `campaniaFundingSchema`
- `campaniaDurationSchema`
- `campaniaMediaSchema`
- `createCampaniaSchema`
- `updateCampaniaSchema`
- `publishCampaniaSchema`
- `CreateCampaniaFormData` (inferred type)
- `UpdateCampaniaFormData` (inferred type)

### Constants
- `QUERY_KEYS.campanias.*`
- `API_ROUTES.campanias.*`
- `APP_ROUTES.dashboard.campanias.*`
- `CAMPANIA_ESTADOS`
- `CAMPANIA_ESTADOS_LABELS`
- `CAMPANIA_ESTADO_COLORS`
- `TIPO_FINANCIACION`
- `TIPO_FINANCIACION_LABELS`
- `MONEDAS`
- `MONEDA_SYMBOLS`
- `MONEDA_CODES`

### Utils
- `getErrorMessage(errorCode: string)`
- `getCampaniaErrorMessage(errorCode: string)`
- `formatCurrencyWithSymbol(amount: number, monedaId: number)`
- `getCurrencySymbol(monedaId: number)`

---

## 9. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `app/(dashboard)/campanias/components/wizard/WizardStepper.tsx` | Component | Stepper horizontal visual |
| `app/(dashboard)/campanias/components/wizard/BasicInfoStep.tsx` | Component | Formulario paso 1 |
| `app/(dashboard)/campanias/components/wizard/StoryStep.tsx` | Component | Formulario paso 2 |
| `app/(dashboard)/campanias/components/wizard/RewardsStep.tsx` | Component | Placeholder paso 3 |
| `app/(dashboard)/campanias/components/wizard/ReviewStep.tsx` | Component | Revision paso 4 |
| `app/(dashboard)/campanias/components/wizard/WizardContainer.tsx` | Component | Contenedor wizard |
| `app/(dashboard)/campanias/components/list/CampaniaListCard.tsx` | Component | Card lista |
| `app/(dashboard)/campanias/components/list/CampaniaStatusBadge.tsx` | Component | Badge estado |
| `app/(dashboard)/campanias/components/list/CampaniaActions.tsx` | Component | Botones accion |
| `app/(dashboard)/campanias/components/preview/DraftBanner.tsx` | Component | Banner warning |
| `app/(dashboard)/campanias/components/preview/PreviewLayout.tsx` | Component | Layout preview |
| `app/(dashboard)/campanias/components/preview/PublishConfirmModal.tsx` | Component | Modal confirmar |
| `app/(dashboard)/campanias/components/shared/RichTextEditor.tsx` | Component | Editor TipTap |
| `app/(dashboard)/campanias/components/shared/ImageUploadField.tsx` | Component | Upload imagen |
| `app/(dashboard)/campanias/components/shared/EmptyState.tsx` | Component | Empty state |
| `hooks/use-wizard-state.ts` | Hook | State manager wizard |
| `app/(dashboard)/campanias/page.tsx` | Page | Lista mis campanias |
| `app/(dashboard)/campanias/nueva/page.tsx` | Page | Wizard crear |
| `app/(dashboard)/campanias/[id]/page.tsx` | Page | Preview |
| `app/(dashboard)/campanias/[id]/editar/page.tsx` | Page | Wizard editar |

**Total:** 20 archivos nuevos

---

## 10. Archivos a Actualizar

| Archivo | Cambios |
|---------|---------|
| `hooks/use-campanias.ts` | Actualizar types importados (usar Campania, CreateCampaniaRequest, UpdateCampaniaRequest) |
| `services/campania.service.ts` | Actualizar types importados de shared |
| `app/(dashboard)/campanias/page.tsx` | REEMPLAZAR contenido existente (si existe) con nueva implementacion |
| `app/(dashboard)/campanias/nueva/page.tsx` | REEMPLAZAR con WizardContainer |
| `components/campanias/campania-form.tsx` | DEPRECAR (reemplazado por wizard steps) |

---

## 11. Dependencias NPM a Instalar

```bash
# En src/admin/
npm install @tiptap/react @tiptap/starter-kit @tiptap/extension-link
npm install dompurify @types/dompurify
npm install date-fns
```

**Dependencias ya instaladas (verificar):**
- `@tanstack/react-query`
- `react-hook-form`
- `@hookform/resolvers`
- `zod`
- `sonner` (toast notifications)
- `lucide-react` (icons)

---

## 12. State Management del Wizard

### Arquitectura

El wizard usa un **state manager local** (`useWizardState`) en lugar de Redux/Zustand para simplificar. Estado se persiste en localStorage para evitar perdida al recargar.

### Estado Global del Wizard

```typescript
interface WizardState {
  currentStep: number              // 1-4
  formData: Partial<CreateCampaniaFormData>
  completedSteps: number[]         // [1, 2] si completaste pasos 1 y 2
}
```

### Persistencia en localStorage

```typescript
// Key: "wizard-draft"
// Value: JSON.stringify(WizardState)

// Al cargar wizard:
useEffect(() => {
  const saved = localStorage.getItem("wizard-draft")
  if (saved) {
    const { formData, currentStep, completedSteps } = JSON.parse(saved)
    restoreState(formData, currentStep, completedSteps)
  }
}, [])

// Al cambiar estado:
useEffect(() => {
  localStorage.setItem("wizard-draft", JSON.stringify(wizardState))
}, [wizardState])

// Al completar wizard:
onSuccess(() => {
  localStorage.removeItem("wizard-draft")
  router.push(...)
})
```

### Navegacion entre Pasos

```typescript
// Permitir navegar solo a:
// 1. Paso actual + 1
// 2. Pasos ya completados

function canNavigateTo(targetStep: number): boolean {
  return targetStep === currentStep + 1 || completedSteps.includes(targetStep)
}

// Desde stepper:
<WizardStepper onStepClick={(step) => canNavigateTo(step) && goToStep(step)} />
```

---

## 13. Protected Routes y Autorizacion

### Validacion en Pages

Todas las pages de campanias requieren autenticacion:

```tsx
// En layout de (dashboard):
export default function DashboardLayout({ children }) {
  const { user, isLoading } = useAuth()

  if (isLoading) return <div>Loading...</div>

  if (!user) {
    redirect("/login")
  }

  return <div>{children}</div>
}
```

### Ownership Validation

Backend valida que ArtistaId del token == ArtistaId de la campania para PUT/DELETE/POST publicar. Frontend no necesita validacion extra (confiar en backend 403).

---

## 14. Loading y Error States

### Skeletons para Loading

```tsx
// En MisCampaniasPage:
if (isLoading) {
  return (
    <div className="space-y-4">
      {[1, 2, 3].map((i) => (
        <Card key={i}>
          <CardContent className="flex items-center gap-6 p-6">
            <Skeleton className="w-24 h-24 rounded-lg" />
            <div className="flex-1 space-y-2">
              <Skeleton className="h-6 w-48" />
              <Skeleton className="h-4 w-32" />
              <Skeleton className="h-2 w-full" />
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
```

### Error Handling

```tsx
// En mutations:
try {
  await mutation.mutateAsync(data)
  toast.success("Operacion exitosa")
} catch (error) {
  if (error.response?.data?.messages) {
    const errorCode = error.response.data.messages[0].errorCode
    toast.error(getCampaniaErrorMessage(errorCode))
  } else {
    toast.error("Error inesperado")
  }
}
```

### Error Boundary

```tsx
// En layout de campanias:
<ErrorBoundary fallback={<ErrorPage />}>
  {children}
</ErrorBoundary>
```

---

## 15. Responsive Behavior

### Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| Mobile | < 640px | Wizard: stepper vertical o texto "Paso X de 4", grid → stack, botones full width |
| Tablet | 640-1024px | Wizard: stepper horizontal compacto, grid 2 cols mantiene |
| Desktop | > 1024px | Layout completo, max-w-4xl para wizard, max-w-7xl para lista |

### Mobile Optimizations

```tsx
// WizardStepper mobile:
<div className="md:flex hidden">
  {/* Stepper horizontal completo */}
</div>
<div className="md:hidden block text-center mb-6">
  <span className="text-sm text-muted-foreground">Paso {currentStep} de {totalSteps}</span>
</div>

// CampaniaListCard mobile:
<Card className="p-4">
  <div className="flex flex-col md:flex-row gap-4">
    <img className="w-full md:w-24 h-48 md:h-24 object-cover rounded-lg" />
    <div className="flex-1">
      {/* Info */}
    </div>
  </div>
</Card>
```

---

## 16. Accesibilidad

### ARIA Labels

- **Wizard Stepper:** `role="progressbar" aria-valuenow={currentStep} aria-valuemin={1} aria-valuemax={4}`
- **Status Badge:** `role="status" aria-label="Estado de campania: Borrador"`
- **Form Fields:** Todos los `<Input>` tienen `<Label htmlFor>` asociado
- **Loading Buttons:** `aria-busy="true"` cuando isSubmitting
- **Modals:** `role="dialog" aria-modal="true"`, focus trap, Esc para cerrar

### Keyboard Navigation

- Tab order logico en wizard
- Enter para submit forms
- Esc para cerrar modals
- Space para toggle switches

### Focus States

```tsx
// Todos los interactivos:
className="focus:ring-2 focus:ring-primary focus:ring-offset-2"
```

---

## 17. Animaciones

| Elemento | Animacion | Duración |
|----------|-----------|----------|
| Wizard step advance | Slide content left + fade | 400ms ease-in-out |
| Stepper connector fill | Width 0 → 100% | 400ms ease |
| Button hover | Scale 1.02 + shadow | 150ms ease |
| Card hover | Border color change | 200ms ease |
| Modal open | Scale 0.95 → 1 + fade | 200ms ease-out |
| Toast notification | Slide in from top | 250ms ease-out |
| Progress bar fill | Width transition | 600ms ease-out |

```tsx
// En WizardContainer:
<motion.div
  key={currentStep}
  initial={{ opacity: 0, x: 20 }}
  animate={{ opacity: 1, x: 0 }}
  exit={{ opacity: 0, x: -20 }}
  transition={{ duration: 0.4 }}
>
  {renderStep()}
</motion.div>
```

**Nota:** Animaciones con framer-motion son opcionales. Usar CSS transitions por defecto.

---

## 18. Testing Strategy (Out of MVP Scope)

### Unit Tests (Opcional, post-MVP)

- `WizardStepper.test.tsx` - Render correcto segun estado
- `useWizardState.test.ts` - State management logica
- `campaniaService.test.ts` - Mocking API calls

### Integration Tests (Opcional, post-MVP)

- Flujo completo wizard crear campania
- Publicar campania desde preview
- Editar campania borrador

**Herramientas:** Vitest + React Testing Library

---

## 19. Checklist de Implementacion

### Shared (Prerequisito)
- [ ] Types actualizados (Campania, CreateCampaniaRequest, etc)
- [ ] Schemas por step creados (campaniaBasicInfoSchema, etc)
- [ ] Constants actualizados (QUERY_KEYS, API_ROUTES, estados)
- [ ] Utils error-messages.ts creado
- [ ] Utils format.ts actualizado (formatCurrencyWithSymbol)

### Componentes Wizard
- [ ] WizardStepper con stepper visual
- [ ] BasicInfoStep con validacion Zod
- [ ] StoryStep con RichTextEditor (TipTap)
- [ ] RewardsStep placeholder
- [ ] ReviewStep con preview
- [ ] WizardContainer con state management

### Componentes Lista
- [ ] CampaniaListCard con imagen, titulo, progress, badges
- [ ] CampaniaStatusBadge con colors semanticos
- [ ] CampaniaActions con botones condicionales
- [ ] EmptyState generico

### Componentes Preview
- [ ] DraftBanner con warning
- [ ] PreviewLayout reutiliza estructura landing
- [ ] PublishConfirmModal con validacion rewards

### Componentes Shared
- [ ] RichTextEditor con TipTap
- [ ] ImageUploadField con preview
- [ ] EmptyState reutilizable

### Hooks
- [ ] useWizardState con localStorage
- [ ] use-campanias.ts actualizado con types correctos

### Services
- [ ] campaniaService actualizado con types shared

### Pages
- [ ] /dashboard/campanias - Lista con EmptyState
- [ ] /dashboard/campanias/nueva - Wizard crear
- [ ] /dashboard/campanias/[id] - Preview con DraftBanner
- [ ] /dashboard/campanias/[id]/editar - Wizard editar

### Integracion
- [ ] Axios client con auth headers configurado
- [ ] React Query provider configurado
- [ ] Toast notifications (sonner) configurado
- [ ] Protected routes (auth middleware)

### UI/UX
- [ ] Dark theme aplicado consistente
- [ ] Gradient buttons (pink → purple) en CTAs
- [ ] Responsive mobile/tablet/desktop
- [ ] Loading skeletons en todas las listas
- [ ] Error handling con toast notifications
- [ ] Animaciones smooth (transitions CSS)

### Dependencias
- [ ] TipTap instalado (@tiptap/react, starter-kit, extension-link)
- [ ] dompurify instalado
- [ ] date-fns instalado

### Testing (Opcional)
- [ ] Tests unitarios componentes clave
- [ ] Tests integracion wizard completo

---

## 20. Siguiente Paso Recomendado

1. **PASO 1: Implementar Shared Contracts** (Pre-requisito)
   - Completar `plans/crear-campania/shared/contracts-plan.md`
   - Validar types, schemas, constants funcionan

2. **PASO 2: Actualizar Services y Hooks**
   - Actualizar `campaniaService` con types correctos
   - Actualizar `use-campanias.ts` con types correctos
   - Crear `use-wizard-state.ts`

3. **PASO 3: Crear Componentes Shared**
   - RichTextEditor (TipTap)
   - ImageUploadField
   - EmptyState

4. **PASO 4: Implementar Wizard**
   - WizardStepper
   - BasicInfoStep, StoryStep, RewardsStep, ReviewStep
   - WizardContainer

5. **PASO 5: Implementar Lista y Preview**
   - CampaniaListCard, StatusBadge, Actions
   - DraftBanner, PreviewLayout
   - PublishConfirmModal

6. **PASO 6: Conectar Pages**
   - Crear 4 pages de Next.js
   - Conectar navegacion con router

7. **PASO 7: Testing Manual**
   - Flujo completo crear campania
   - Flujo editar campania borrador
   - Flujo publicar campania
   - Validaciones frontend y backend

---

**Fin del Plan Frontend Admin Dashboard**
