# Plan Frontend: Templates y Guia para Artistas Noveles (Landing)

**Fecha:** 2026-02-15
**Feature:** cs-templates-guia (US-CS-01)
**Target:** Landing (src/web)
**Basado en:**
- `docs/user-stories/cs-templates-guia/feature-spec.md`
- `docs/user-stories/cs-templates-guia/contracts.md`
- `docs/user-stories/cs-templates-guia/ui-ux.md`
- `plans/cs-templates-guia/shared/contracts-plan.md`

---

## 1. Resumen

- **Screens:** 3 (Wizard de 3 pasos)
- **Componentes:** 13 componentes principales + 4 componentes shared
- **Hooks:** 5 hooks personalizados (3 query, 1 mutation, 1 state management)
- **Services:** 1 API service class
- **Ruta base:** `/crowdsourcing/nuevo-proyecto`

### Arquitectura
Sigue el patron **feature-based con capas hexagonales** existente en `src/web/src/features/`:
- **Domain:** Types re-exportados de shared
- **Application:** Hooks (TanStack Query + state management)
- **Infrastructure:** API service con mappers
- **Presentation:** Componentes UI + Pages

---

## 2. Estructura de Carpetas

```
src/web/src/features/crowdsourcing/
├── domain/
│   ├── types.ts                         # Re-export de @shared/types/crowdsourcing
│   └── index.ts
├── application/
│   ├── hooks/
│   │   ├── useTemplates.ts              # Query hook - GET /templates
│   │   ├── useTemplateDetail.ts         # Query hook - GET /templates/{id}
│   │   ├── useRolesProfesionales.ts     # Query hook - GET /maestras/roles
│   │   ├── useGenerarNecesidades.ts     # Mutation hook - POST /templates/{id}/generar
│   │   └── useWizardState.ts            # Local state management (wizard steps)
│   ├── schemas.ts                       # Re-export de @shared/schemas/crowdsourcing
│   └── index.ts
├── infrastructure/
│   ├── api/
│   │   └── crowdsourcing.api.ts         # API service class (campania.api.ts pattern)
│   └── index.ts
├── presentation/
│   ├── components/
│   │   ├── WizardStepper.tsx            # Progress indicator (Step X of 3)
│   │   ├── TemplateCard.tsx             # Card de template para galeria
│   │   ├── TemplateGallery.tsx          # Grid de templates (Paso 1)
│   │   ├── NecesidadItem.tsx            # Item de necesidad con checkbox + budget
│   │   ├── NecesidadList.tsx            # Lista agrupada por fases
│   │   ├── BudgetSummary.tsx            # Resumen sticky con totales
│   │   ├── RolProfesionalTooltip.tsx    # Tooltip con descripcion del rol
│   │   ├── ConfirmationSummary.tsx      # Resumen final (Paso 3)
│   │   ├── ProyectoSelector.tsx         # Selector de proyecto artistico
│   │   ├── TemplateCardSkeleton.tsx     # Loading skeleton para galeria
│   │   ├── NecesidadListSkeleton.tsx    # Loading skeleton para paso 2
│   │   └── index.ts
│   ├── pages/
│   │   ├── NuevoProyectoPage.tsx        # Page container (maneja routing steps)
│   │   └── index.ts
│   └── index.ts
├── __tests__/
│   ├── components/
│   │   ├── TemplateCard.test.tsx
│   │   ├── NecesidadItem.test.tsx
│   │   └── BudgetSummary.test.tsx
│   └── hooks/
│       ├── useTemplates.test.ts
│       └── useWizardState.test.ts
└── index.ts
```

---

## 3. Componentes

### 3.1 WizardStepper

**Archivo:** `presentation/components/WizardStepper.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| currentStep | 1 \| 2 \| 3 | Si | Paso actual del wizard |
| stepLabels | string[] | No | Labels personalizados (default: ["Seleccionar", "Personalizar", "Confirmar"]) |

**Estado Local:** Ninguno (stateless)

**Dependencias:**
- shadcn/ui: `<Progress>`
- Lucide icons: `Check`, `Circle`

**Responsabilidad:**
- Muestra barra de progreso horizontal con 3 pasos
- Pasos completados con check icon
- Paso actual destacado con primary color
- Pasos futuros con estado muted

**Renderizado:**
```tsx
<div className="max-w-4xl mx-auto mb-8">
  <div className="flex items-center justify-between">
    {[1, 2, 3].map((step) => (
      <StepCircle key={step} completed={step < currentStep} active={step === currentStep} />
    ))}
  </div>
  <Progress value={(currentStep / 3) * 100} className="mt-4" />
</div>
```

---

### 3.2 TemplateCard

**Archivo:** `presentation/components/TemplateCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| template | PlantillaProyectoList | Si | Datos del template |
| onSelect | (id: string) => void | Si | Callback al seleccionar |
| className | string | No | Classes adicionales |

**Estado Local:**
- `isHovered: boolean` (para animaciones)

**Dependencias:**
- shadcn/ui: `<Card>`, `<CardHeader>`, `<CardContent>`, `<Button>`
- Lucide icons: Dinamico segun `template.icono` ("music", "video", "route", etc.)
- Utils: `formatCurrency` de `@/lib/utils`

**Responsabilidad:**
- Renderiza card individual de template con icono, titulo, stats
- Hover effect con scale y glow shadow
- Click llama a `onSelect(template.id)`

**Renderizado:**
```tsx
<Card
  onClick={() => onSelect(template.id)}
  className="bg-[#0f1729] border-[#334155] hover:bg-[#1e2a42] hover:border-primary transition-all duration-200 cursor-pointer"
>
  <CardHeader>
    <Icon className="w-16 h-16 text-primary mb-4" />
    <h3 className="text-xl font-semibold text-white">{template.nombre}</h3>
  </CardHeader>
  <CardContent>
    <p className="text-sm text-[#94a3b8] mb-4">{template.descripcion}</p>
    <div className="flex flex-col gap-1 text-sm text-[#94a3b8]">
      <span>{template.cantidadNecesidades} necesidades</span>
      <span className="font-medium text-primary">
        {formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}
      </span>
    </div>
    <Button className="w-full mt-4">Seleccionar</Button>
  </CardContent>
</Card>
```

---

### 3.3 TemplateGallery

**Archivo:** `presentation/components/TemplateGallery.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| templates | PlantillaProyectoList[] | Si | Lista de templates |
| onSelectTemplate | (id: string) => void | Si | Callback al seleccionar |
| isLoading | boolean | No | Estado de carga |

**Estado Local:** Ninguno

**Dependencias:**
- Components: `<TemplateCard>`, `<TemplateCardSkeleton>`

**Responsabilidad:**
- Grid responsive de templates (1/2/3 cols)
- Loading skeletons mientras carga
- Empty state si no hay templates

**Renderizado:**
```tsx
<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
  {isLoading ? (
    Array.from({ length: 6 }).map((_, i) => <TemplateCardSkeleton key={i} />)
  ) : templates.length === 0 ? (
    <EmptyState />
  ) : (
    templates.map((template) => (
      <TemplateCard
        key={template.id}
        template={template}
        onSelect={onSelectTemplate}
      />
    ))
  )}
</div>
```

---

### 3.4 NecesidadItem

**Archivo:** `presentation/components/NecesidadItem.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| necesidad | PlantillaProyectoNecesidad | Si | Datos de la necesidad |
| isSelected | boolean | Si | Estado del checkbox |
| presupuestoMin | number \| undefined | No | Presupuesto min personalizado |
| presupuestoMax | number \| undefined | No | Presupuesto max personalizado |
| onToggle | (id: string) => void | Si | Toggle checkbox |
| onBudgetChange | (id: string, min: number, max: number) => void | Si | Cambio de presupuesto |

**Estado Local:**
- `localMin: number` (controlled input)
- `localMax: number` (controlled input)
- `validationError: string | null`

**Dependencias:**
- shadcn/ui: `<Checkbox>`, `<Label>`, `<Input>`, `<Badge>`, `<Tooltip>`
- Components: `<RolProfesionalTooltip>`
- Utils: `formatCurrency`
- Constants: `PRIORIDAD_NECESIDAD_COLORS`

**Responsabilidad:**
- Renderiza item de necesidad con checkbox
- Badge de prioridad con colores (Alta=red, Media=yellow, Baja=blue)
- Inputs de presupuesto (solo visibles si isSelected)
- Validacion en tiempo real (max >= min)
- Tooltip del rol profesional

**Renderizado:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-4 mb-3">
  <div className="flex items-start gap-3">
    <Checkbox
      checked={isSelected}
      onCheckedChange={() => onToggle(necesidad.id)}
    />
    <div className="flex-1">
      <div className="flex items-center gap-2 mb-1">
        <Label className="text-base font-medium text-white">
          {necesidad.titulo}
        </Label>
        <Badge variant={getBadgeVariant(necesidad.prioridad)}>
          {necesidad.prioridad}
        </Badge>
      </div>
      <div className="flex items-center gap-1 text-sm text-[#94a3b8]">
        <span>Rol: {necesidad.rolProfesional.nombre}</span>
        <RolProfesionalTooltip rol={necesidad.rolProfesional} />
      </div>
      {isSelected && (
        <div className="flex gap-2 items-center mt-2">
          <Input
            type="number"
            value={localMin}
            onChange={(e) => handleMinChange(e.target.value)}
            placeholder={necesidad.precioMinOrientativo?.toString()}
          />
          <span>-</span>
          <Input
            type="number"
            value={localMax}
            onChange={(e) => handleMaxChange(e.target.value)}
            placeholder={necesidad.precioMaxOrientativo?.toString()}
          />
          <span className="text-sm text-[#94a3b8]">EUR</span>
        </div>
      )}
      {validationError && (
        <p className="text-sm text-red-500 mt-1">{validationError}</p>
      )}
    </div>
  </div>
</Card>
```

---

### 3.5 NecesidadList

**Archivo:** `presentation/components/NecesidadList.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| necesidades | PlantillaProyectoNecesidad[] | Si | Lista de necesidades |
| selectedNecesidades | Map<string, { min?: number, max?: number }> | Si | Estado de seleccion y presupuestos |
| onToggleNecesidad | (id: string) => void | Si | Toggle necesidad |
| onBudgetChange | (id: string, min: number, max: number) => void | Si | Cambio presupuesto |

**Estado Local:** Ninguno (delegado al parent via hooks)

**Dependencias:**
- Components: `<NecesidadItem>`

**Responsabilidad:**
- Agrupa necesidades por fase
- Renderiza headers de fase
- Delega rendering de items a `<NecesidadItem>`

**Renderizado:**
```tsx
<div className="space-y-8">
  {Object.entries(groupByFase(necesidades)).map(([fase, items]) => (
    <div key={fase}>
      <h2 className="text-xl font-semibold text-white mb-4 uppercase tracking-wide">
        {fase}
      </h2>
      {items.map((necesidad) => (
        <NecesidadItem
          key={necesidad.id}
          necesidad={necesidad}
          isSelected={selectedNecesidades.has(necesidad.id)}
          presupuestoMin={selectedNecesidades.get(necesidad.id)?.min}
          presupuestoMax={selectedNecesidades.get(necesidad.id)?.max}
          onToggle={onToggleNecesidad}
          onBudgetChange={onBudgetChange}
        />
      ))}
    </div>
  ))}
</div>
```

---

### 3.6 BudgetSummary

**Archivo:** `presentation/components/BudgetSummary.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| minTotal | number | Si | Suma total minima |
| maxTotal | number | Si | Suma total maxima |
| selectedCount | number | Si | Cantidad seleccionada |
| totalCount | number | Si | Cantidad total disponible |

**Estado Local:**
- `animatedMin: number` (count-up animation)
- `animatedMax: number` (count-up animation)

**Dependencias:**
- shadcn/ui: `<Card>`, `<Progress>`
- Utils: `formatCurrency`, `useCountUp` (custom hook)

**Responsabilidad:**
- Card sticky (desktop) con resumen presupuestario
- Totales min/max con animacion count-up
- Contador "X de Y seleccionadas"
- Progress bar visual
- Calculo de promedio: `(min + max) / 2`

**Renderizado:**
```tsx
<Card className="bg-gradient-to-r from-purple-900/20 to-pink-900/20 border-primary p-6 sticky top-4">
  <h3 className="text-lg font-semibold text-white mb-4">
    Resumen Presupuestario
  </h3>
  <div className="space-y-3">
    <div>
      <p className="text-sm text-[#94a3b8]">Min total</p>
      <p className="text-2xl font-bold text-white">
        {formatCurrency(animatedMin)}
      </p>
    </div>
    <div>
      <p className="text-sm text-[#94a3b8]">Max total</p>
      <p className="text-2xl font-bold text-white">
        {formatCurrency(animatedMax)}
      </p>
    </div>
    <div>
      <p className="text-sm text-[#94a3b8]">Promedio estimado</p>
      <p className="text-base text-white">
        {formatCurrency((minTotal + maxTotal) / 2)}
      </p>
    </div>
    <div>
      <p className="text-sm text-[#94a3b8] mb-2">
        Necesidades seleccionadas: {selectedCount} de {totalCount}
      </p>
      <Progress value={(selectedCount / totalCount) * 100} />
    </div>
  </div>
</Card>
```

---

### 3.7 RolProfesionalTooltip

**Archivo:** `presentation/components/RolProfesionalTooltip.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| rol | RolProfesional | Si | Datos del rol |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `<Tooltip>`, `<TooltipTrigger>`, `<TooltipContent>`
- Lucide icons: `Info`

**Responsabilidad:**
- Icono de info clicable/hoverable
- Tooltip con descripcion del rol
- Accesible con teclado (Enter/Esc)

**Renderizado:**
```tsx
<Tooltip>
  <TooltipTrigger asChild>
    <button
      className="inline-flex items-center justify-center w-4 h-4 rounded-full hover:bg-primary/10"
      aria-label={`Informacion sobre ${rol.nombre}`}
    >
      <Info className="w-3 h-3 text-[#94a3b8] hover:text-primary" />
    </button>
  </TooltipTrigger>
  <TooltipContent className="max-w-xs">
    <p className="text-sm">{rol.descripcion}</p>
    {rol.modalidadCobro && (
      <p className="text-xs text-[#94a3b8] mt-2">
        Modalidad: {rol.modalidadCobro}
      </p>
    )}
  </TooltipContent>
</Tooltip>
```

---

### 3.8 ConfirmationSummary

**Archivo:** `presentation/components/ConfirmationSummary.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| template | PlantillaProyecto | Si | Template seleccionado |
| selectedNecesidades | PlantillaProyectoNecesidad[] | Si | Necesidades seleccionadas |
| presupuestos | Map<string, { min?: number, max?: number }> | Si | Presupuestos personalizados |
| totalMin | number | Si | Total minimo calculado |
| totalMax | number | Si | Total maximo calculado |

**Estado Local:** Ninguno

**Dependencias:**
- shadcn/ui: `<Card>`, `<Badge>`
- Utils: `formatCurrency`, `groupByFase`

**Responsabilidad:**
- Renderiza resumen final del wizard
- Box destacado con totales de presupuesto
- Lista de necesidades agrupadas por fase
- Badges de prioridad por necesidad

**Renderizado:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-8">
  <h2 className="text-2xl font-semibold text-white mb-1">
    {template.nombre}
  </h2>
  <p className="text-base text-[#94a3b8] mb-6">
    Necesidades seleccionadas: {selectedNecesidades.length} de {template.necesidades.length}
  </p>

  <div className="bg-gradient-to-r from-purple-900/30 to-pink-900/30 border border-primary rounded-lg p-6 mb-6">
    <h3 className="text-lg font-semibold text-white mb-3">
      Resumen Presupuestario
    </h3>
    <div className="space-y-2">
      <div className="flex justify-between">
        <span className="text-white">Min total:</span>
        <span className="text-xl font-bold text-white">{formatCurrency(totalMin)}</span>
      </div>
      <div className="flex justify-between">
        <span className="text-white">Max total:</span>
        <span className="text-xl font-bold text-white">{formatCurrency(totalMax)}</span>
      </div>
      <div className="flex justify-between border-t border-primary/30 pt-2">
        <span className="text-[#94a3b8]">Rango promedio:</span>
        <span className="text-base text-[#94a3b8]">
          {formatCurrency((totalMin + totalMax) / 2)}
        </span>
      </div>
    </div>
  </div>

  <h3 className="text-lg font-semibold text-white mb-4">
    Necesidades Seleccionadas
  </h3>
  {Object.entries(groupByFase(selectedNecesidades)).map(([fase, items]) => (
    <div key={fase} className="mb-4">
      <h4 className="text-sm font-semibold text-[#94a3b8] uppercase tracking-wide mb-2">
        {fase}
      </h4>
      <ul className="space-y-2">
        {items.map((nec) => (
          <li key={nec.id} className="flex items-center gap-2">
            <span className="text-primary">•</span>
            <span className="text-white">{nec.titulo}</span>
            <span className="text-[#94a3b8]">
              ({formatCurrency(presupuestos.get(nec.id)?.min || nec.precioMinOrientativo)} -
               {formatCurrency(presupuestos.get(nec.id)?.max || nec.precioMaxOrientativo)})
            </span>
            <Badge variant={getBadgeVariant(nec.prioridad)} size="sm">
              {nec.prioridad}
            </Badge>
          </li>
        ))}
      </ul>
    </div>
  ))}
</Card>
```

---

### 3.9 ProyectoSelector

**Archivo:** `presentation/components/ProyectoSelector.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| value | string \| undefined | Si | ID del proyecto seleccionado |
| onChange | (id: string) => void | Si | Callback al cambiar seleccion |
| proyectos | ProyectoArtistico[] | Si | Lista de proyectos del artista |

**Estado Local:** Ninguno (controlled component)

**Dependencias:**
- shadcn/ui: `<Select>`, `<SelectTrigger>`, `<SelectContent>`, `<SelectItem>`
- Types: `ProyectoArtistico` (importado de feature existente o shared)

**Responsabilidad:**
- Select dropdown con proyectos del artista
- Opcion "Crear nuevo proyecto" al final
- Label y descripcion

**Renderizado:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-4">
  <Label className="text-sm font-medium text-[#cbd5e1] mb-2">
    Proyecto Artistico (opcional)
  </Label>
  <p className="text-xs text-[#94a3b8] mb-3">
    Vincula estas necesidades a un proyecto existente o crea uno nuevo
  </p>
  <Select value={value} onValueChange={onChange}>
    <SelectTrigger>
      <SelectValue placeholder="Selecciona un proyecto..." />
    </SelectTrigger>
    <SelectContent>
      {proyectos.map((proyecto) => (
        <SelectItem key={proyecto.id} value={proyecto.id}>
          {proyecto.nombre}
        </SelectItem>
      ))}
      <SelectItem value="__create_new__">
        + Crear nuevo proyecto
      </SelectItem>
    </SelectContent>
  </Select>
</Card>
```

---

### 3.10 TemplateCardSkeleton

**Archivo:** `presentation/components/TemplateCardSkeleton.tsx`

**Props:** Ninguno

**Responsabilidad:**
- Loading skeleton con misma estructura que `<TemplateCard>`
- Animacion de pulse

**Renderizado:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] animate-pulse">
  <CardHeader>
    <Skeleton className="w-16 h-16 rounded-full mb-4" />
    <Skeleton className="h-6 w-3/4" />
  </CardHeader>
  <CardContent>
    <Skeleton className="h-4 w-full mb-2" />
    <Skeleton className="h-4 w-2/3 mb-4" />
    <Skeleton className="h-4 w-1/2 mb-1" />
    <Skeleton className="h-4 w-2/3" />
    <Skeleton className="h-10 w-full mt-4" />
  </CardContent>
</Card>
```

---

### 3.11 NecesidadListSkeleton

**Archivo:** `presentation/components/NecesidadListSkeleton.tsx`

**Props:** Ninguno

**Responsabilidad:**
- Loading skeleton para lista de necesidades
- Muestra 3 grupos de fase con 2-3 items cada uno

---

### 3.12 NuevoProyectoPage

**Archivo:** `presentation/pages/NuevoProyectoPage.tsx`

**Props:** Ninguno (usa routing params)

**Estado Local:** Via `useWizardState()` hook

**Dependencias:**
- Hooks: `useTemplates`, `useTemplateDetail`, `useGenerarNecesidades`, `useWizardState`
- Components: Todos los anteriores
- Router: `useSearchParams`, `useNavigate` (react-router-dom)

**Responsabilidad:**
- Container de la feature completa
- Maneja routing entre steps via query param `?step=1|2|3`
- Orquesta estado del wizard
- Navegacion entre pasos
- Submit final

**Estructura:**
```tsx
function NuevoProyectoPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  const currentStep = parseInt(searchParams.get('step') || '1') as 1 | 2 | 3;
  const templateId = searchParams.get('template');

  const { data: templates, isLoading: loadingTemplates } = useTemplates();
  const { data: template, isLoading: loadingTemplate } = useTemplateDetail(templateId || '');
  const { mutate: generarNecesidades, isPending } = useGenerarNecesidades();

  const {
    selectedNecesidades,
    presupuestos,
    proyectoId,
    toggleNecesidad,
    updatePresupuesto,
    setProyectoId,
    getTotals,
  } = useWizardState(template);

  const handleSelectTemplate = (id: string) => {
    setSearchParams({ step: '2', template: id });
  };

  const handleNext = () => {
    setSearchParams({ step: String(currentStep + 1), template: templateId || '' });
  };

  const handleBack = () => {
    setSearchParams({ step: String(currentStep - 1), template: templateId || '' });
  };

  const handleSubmit = () => {
    if (!templateId || !proyectoId) return;

    const payload: GenerarNecesidadesRequest = {
      proyectoArtisticoId: proyectoId,
      necesidadesSeleccionadas: Array.from(selectedNecesidades).map((id) => ({
        plantillaNecesidadId: id,
        presupuestoMin: presupuestos.get(id)?.min,
        presupuestoMax: presupuestos.get(id)?.max,
        monedaId: 1, // EUR
      })),
    };

    generarNecesidades(
      { templateId, data: payload },
      {
        onSuccess: (result) => {
          toast.success(`${result.necesidadesCreadas} necesidades publicadas correctamente`);
          navigate('/crowdsourcing/mis-necesidades');
        },
        onError: (error) => {
          toast.error('Error al publicar necesidades. Intenta de nuevo.');
        },
      }
    );
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <WizardStepper currentStep={currentStep} />

      {currentStep === 1 && (
        <>
          <h1 className="text-4xl font-bold text-white mb-2">
            Selecciona tu tipo de proyecto
          </h1>
          <p className="text-lg text-[#94a3b8] mb-8">
            Elige la plantilla que mejor se adapte a tus objetivos
          </p>
          <TemplateGallery
            templates={templates || []}
            isLoading={loadingTemplates}
            onSelectTemplate={handleSelectTemplate}
          />
        </>
      )}

      {currentStep === 2 && template && (
        <>
          <h1 className="text-3xl font-bold text-white mb-2">
            {template.nombre}
          </h1>
          <p className="text-lg text-[#94a3b8] mb-8">
            Personaliza las necesidades de tu proyecto
          </p>
          <div className="grid lg:grid-cols-3 gap-6">
            <div className="lg:col-span-2">
              <NecesidadList
                necesidades={template.necesidades}
                selectedNecesidades={presupuestos}
                onToggleNecesidad={toggleNecesidad}
                onBudgetChange={updatePresupuesto}
              />
            </div>
            <div className="lg:col-span-1">
              <BudgetSummary {...getTotals()} />
            </div>
          </div>
          <div className="flex justify-between mt-8">
            <Button variant="outline" onClick={handleBack}>
              &lt; Atras
            </Button>
            <Button
              onClick={handleNext}
              disabled={selectedNecesidades.size === 0}
            >
              Siguiente &gt;
            </Button>
          </div>
        </>
      )}

      {currentStep === 3 && template && (
        <>
          <h1 className="text-3xl font-bold text-white mb-2">
            Resumen de tu proyecto
          </h1>
          <p className="text-lg text-[#94a3b8] mb-8">
            Revisa los detalles antes de publicar
          </p>
          <ConfirmationSummary
            template={template}
            selectedNecesidades={Array.from(selectedNecesidades).map(id =>
              template.necesidades.find(n => n.id === id)!
            )}
            presupuestos={presupuestos}
            {...getTotals()}
          />
          <ProyectoSelector
            value={proyectoId}
            onChange={setProyectoId}
            proyectos={[]} // TODO: fetch from artista context
          />
          <div className="flex justify-between mt-8">
            <Button variant="outline" onClick={handleBack}>
              &lt; Atras
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={isPending || !proyectoId}
            >
              {isPending ? 'Publicando...' : 'Confirmar y publicar'}
            </Button>
          </div>
        </>
      )}
    </div>
  );
}
```

---

## 4. Hooks

### 4.1 useTemplates

**Archivo:** `application/hooks/useTemplates.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:** Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | PlantillaProyectoList[] \| undefined | Lista de templates |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |
| refetch | () => void | Re-fetch manual |

**Query Key:** `QUERY_KEYS.crowdsourcing.templates.all`

**Implementacion:**
```typescript
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/lib/constants';
import { crowdsourcingApi } from '@/features/crowdsourcing/infrastructure';
import type { PlantillaProyectoList } from '../domain';

export function useTemplates() {
  return useQuery({
    queryKey: QUERY_KEYS.crowdsourcing.templates.all,
    queryFn: () => crowdsourcingApi.getTemplates(),
    staleTime: 5 * 60 * 1000, // 5 min - datos maestros cambian poco
  });
}
```

---

### 4.2 useTemplateDetail

**Archivo:** `application/hooks/useTemplateDetail.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del template |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | PlantillaProyecto \| undefined | Detalle del template |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Query Key:** `QUERY_KEYS.crowdsourcing.templates.byId(id)`

**Implementacion:**
```typescript
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/lib/constants';
import { crowdsourcingApi } from '@/features/crowdsourcing/infrastructure';
import type { PlantillaProyecto } from '../domain';

export function useTemplateDetail(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.crowdsourcing.templates.byId(id),
    queryFn: () => crowdsourcingApi.getTemplateById(id),
    enabled: !!id, // Solo ejecutar si hay ID
    staleTime: 5 * 60 * 1000,
  });
}
```

---

### 4.3 useRolesProfesionales

**Archivo:** `application/hooks/useRolesProfesionales.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:** Ninguno

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | RolProfesionalConCategoria[] \| undefined | Catalogo de roles |
| isLoading | boolean | Estado de carga |

**Query Key:** `QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales`

**Implementacion:**
```typescript
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/lib/constants';
import { crowdsourcingApi } from '@/features/crowdsourcing/infrastructure';

export function useRolesProfesionales() {
  return useQuery({
    queryKey: QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales,
    queryFn: () => crowdsourcingApi.getRolesProfesionales(),
    staleTime: 30 * 60 * 1000, // 30 min - datos maestros muy estables
  });
}
```

---

### 4.4 useGenerarNecesidades

**Archivo:** `application/hooks/useGenerarNecesidades.ts`

**Tipo:** Mutation Hook (TanStack Query)

**Parametros:** Ninguno (usa mutate con payload)

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| mutate | (vars: { templateId: string, data: GenerarNecesidadesRequest }) => void | Ejecutar mutacion |
| isPending | boolean | Estado de carga |
| isSuccess | boolean | Exito de la operacion |
| error | Error \| null | Error si hay |

**Acciones:**
- `onSuccess`: Invalidar queries, toast success, redirect
- `onError`: Toast error

**Implementacion:**
```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/lib/constants';
import { crowdsourcingApi } from '@/features/crowdsourcing/infrastructure';
import type { GenerarNecesidadesRequest, GenerarNecesidadesResult } from '../domain';

export function useGenerarNecesidades() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ templateId, data }: { templateId: string; data: GenerarNecesidadesRequest }) =>
      crowdsourcingApi.generarNecesidades(templateId, data),
    onSuccess: (result: GenerarNecesidadesResult) => {
      // Invalidar queries relacionadas (mis necesidades, etc.)
      queryClient.invalidateQueries({ queryKey: ['necesidades-crowdsourcing'] });
    },
  });
}
```

---

### 4.5 useWizardState

**Archivo:** `application/hooks/useWizardState.ts`

**Tipo:** State Management Hook (useState + useCallback)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| template | PlantillaProyecto \| undefined | Template actual |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| selectedNecesidades | Set<string> | IDs de necesidades seleccionadas |
| presupuestos | Map<string, { min?: number, max?: number }> | Presupuestos personalizados |
| proyectoId | string \| undefined | Proyecto artistico seleccionado |
| toggleNecesidad | (id: string) => void | Toggle seleccion de necesidad |
| updatePresupuesto | (id: string, min: number, max: number) => void | Actualizar presupuesto |
| setProyectoId | (id: string) => void | Seleccionar proyecto |
| getTotals | () => { minTotal, maxTotal, selectedCount, totalCount } | Calcular totales |
| reset | () => void | Reset estado |

**Estado Interno:**
- Inicializa con necesidades "Alta" pre-seleccionadas
- Guarda en localStorage para persistir entre refrescos (opcional)

**Implementacion:**
```typescript
import { useState, useCallback, useMemo, useEffect } from 'react';
import type { PlantillaProyecto } from '../domain';

interface BudgetData {
  min?: number;
  max?: number;
}

export function useWizardState(template: PlantillaProyecto | undefined) {
  const [selectedNecesidades, setSelectedNecesidades] = useState<Set<string>>(new Set());
  const [presupuestos, setPresupuestos] = useState<Map<string, BudgetData>>(new Map());
  const [proyectoId, setProyectoId] = useState<string | undefined>();

  // Auto-select "Alta" priority needs when template loads
  useEffect(() => {
    if (!template) return;

    const altaPriorityIds = template.necesidades
      .filter((n) => n.prioridad === 'Alta')
      .map((n) => n.id);

    setSelectedNecesidades(new Set(altaPriorityIds));

    // Initialize budgets with orientative prices
    const initialBudgets = new Map<string, BudgetData>();
    altaPriorityIds.forEach((id) => {
      const nec = template.necesidades.find((n) => n.id === id);
      if (nec) {
        initialBudgets.set(id, {
          min: nec.precioMinOrientativo,
          max: nec.precioMaxOrientativo,
        });
      }
    });
    setPresupuestos(initialBudgets);
  }, [template]);

  const toggleNecesidad = useCallback((id: string) => {
    setSelectedNecesidades((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
        // Remove budget when unchecked
        setPresupuestos((p) => {
          const newMap = new Map(p);
          newMap.delete(id);
          return newMap;
        });
      } else {
        next.add(id);
        // Initialize budget with orientative prices
        const nec = template?.necesidades.find((n) => n.id === id);
        if (nec) {
          setPresupuestos((p) => new Map(p).set(id, {
            min: nec.precioMinOrientativo,
            max: nec.precioMaxOrientativo,
          }));
        }
      }
      return next;
    });
  }, [template]);

  const updatePresupuesto = useCallback((id: string, min: number, max: number) => {
    setPresupuestos((prev) => new Map(prev).set(id, { min, max }));
  }, []);

  const getTotals = useCallback(() => {
    let minTotal = 0;
    let maxTotal = 0;

    selectedNecesidades.forEach((id) => {
      const budget = presupuestos.get(id);
      minTotal += budget?.min || 0;
      maxTotal += budget?.max || 0;
    });

    return {
      minTotal,
      maxTotal,
      selectedCount: selectedNecesidades.size,
      totalCount: template?.necesidades.length || 0,
    };
  }, [selectedNecesidades, presupuestos, template]);

  const reset = useCallback(() => {
    setSelectedNecesidades(new Set());
    setPresupuestos(new Map());
    setProyectoId(undefined);
  }, []);

  return {
    selectedNecesidades,
    presupuestos,
    proyectoId,
    toggleNecesidad,
    updatePresupuesto,
    setProyectoId,
    getTotals,
    reset,
  };
}
```

---

## 5. Services

### 5.1 crowdsourcingApi

**Archivo:** `infrastructure/api/crowdsourcing.api.ts`

**Patron:** Singleton class (sigue patron de `campaniaApi`)

**Metodos:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getTemplates | - | PlantillaProyectoList[] | GET /api/crowdsourcing/templates |
| getTemplateById | id: string | PlantillaProyecto | GET /api/crowdsourcing/templates/{id} |
| generarNecesidades | templateId: string, data: GenerarNecesidadesRequest | GenerarNecesidadesResult | POST /api/crowdsourcing/templates/{id}/generar |
| getRolesProfesionales | - | RolProfesionalConCategoria[] | GET /api/crowdsourcing/maestras/roles-profesionales |
| getCategoriasRol | - | CategoriaRol[] | GET /api/crowdsourcing/maestras/categorias-rol |

**Implementacion:**
```typescript
import { apiFetch } from '@/lib/api-client';
import type {
  PlantillaProyectoList,
  PlantillaProyecto,
  GenerarNecesidadesRequest,
  GenerarNecesidadesResult,
  RolProfesionalConCategoria,
  CategoriaRol,
} from '@/features/crowdsourcing/domain';

// ServiceResponse wrapper from backend
interface ServiceResponse<T> {
  data: T;
  messages: Array<{ message: string; errorCode: string }>;
  isSuccess?: boolean;
}

class CrowdsourcingApiService {
  private readonly baseUrl = '/api/crowdsourcing';

  async getTemplates(): Promise<PlantillaProyectoList[]> {
    const response = await apiFetch<ServiceResponse<{ items: PlantillaProyectoList[] }>>(
      `${this.baseUrl}/templates`
    );
    return response.data.items || [];
  }

  async getTemplateById(id: string): Promise<PlantillaProyecto> {
    const response = await apiFetch<ServiceResponse<PlantillaProyecto>>(
      `${this.baseUrl}/templates/${id}`
    );
    if (!response.data) {
      throw new Error('Template not found');
    }
    return response.data;
  }

  async generarNecesidades(
    templateId: string,
    data: GenerarNecesidadesRequest
  ): Promise<GenerarNecesidadesResult> {
    const response = await apiFetch<ServiceResponse<GenerarNecesidadesResult>>(
      `${this.baseUrl}/templates/${templateId}/generar`,
      {
        method: 'POST',
        data,
      }
    );
    if (!response.data) {
      throw new Error('Failed to generate necesidades');
    }
    return response.data;
  }

  async getRolesProfesionales(): Promise<RolProfesionalConCategoria[]> {
    const response = await apiFetch<ServiceResponse<{ items: RolProfesionalConCategoria[] }>>(
      `${this.baseUrl}/maestras/roles-profesionales`
    );
    return response.data.items || [];
  }

  async getCategoriasRol(): Promise<CategoriaRol[]> {
    const response = await apiFetch<ServiceResponse<{ items: CategoriaRol[] }>>(
      `${this.baseUrl}/maestras/categorias-rol`
    );
    return response.data.items || [];
  }
}

export const crowdsourcingApi = new CrowdsourcingApiService();
```

---

## 6. Flujo de Datos

```
User Action (Landing)
    ↓
Component (presentation/pages/NuevoProyectoPage.tsx)
    ↓
Hook (application/hooks/useTemplates, useGenerarNecesidades)
    ↓
Service (infrastructure/api/crowdsourcing.api.ts)
    ↓
API Client (lib/api-client.ts)
    ↓
Backend (POST /api/crowdsourcing/templates/{id}/generar)
    ↓
ServiceResponse<GenerarNecesidadesResult>
    ↓
TanStack Query cache update
    ↓
Component re-render with updated data
    ↓
Toast notification + redirect
```

### Flujo Detallado del Wizard

**Paso 1 - Seleccion de Template:**
1. Page monta `<TemplateGallery>`
2. `useTemplates()` ejecuta query a `/templates`
3. Backend retorna lista de 6 templates
4. Renderiza grid con `<TemplateCard>` por cada template
5. Usuario click en card → `handleSelectTemplate(id)`
6. Navega a `?step=2&template={id}`

**Paso 2 - Personalizacion:**
1. Page detecta `step=2` y `template={id}`
2. `useTemplateDetail(id)` ejecuta query a `/templates/{id}`
3. Backend retorna template completo con necesidades
4. `useWizardState` inicializa con necesidades "Alta" pre-seleccionadas
5. Renderiza `<NecesidadList>` + `<BudgetSummary>` sticky
6. Usuario toggle checkboxes → `toggleNecesidad(id)` → actualiza state
7. Usuario modifica presupuestos → `updatePresupuesto(id, min, max)` → actualiza state
8. `<BudgetSummary>` recalcula totales en tiempo real
9. Click "Siguiente" → valida al menos 1 seleccionada → navega a `?step=3`

**Paso 3 - Confirmacion:**
1. Page detecta `step=3`
2. Renderiza `<ConfirmationSummary>` con datos de state
3. Renderiza `<ProyectoSelector>` para vincular a proyecto
4. Usuario selecciona proyecto → `setProyectoId(id)`
5. Click "Confirmar y publicar" → `handleSubmit()`
6. `useGenerarNecesidades().mutate()` ejecuta POST a `/templates/{id}/generar`
7. Backend crea N `NecesidadCrowdsourcing` en DB
8. `onSuccess`: toast verde + redirect a `/crowdsourcing/mis-necesidades`
9. `onError`: toast rojo + boton vuelve a enabled

---

## 7. Dependencias de Shared

**Importar de `@shared/`:**

### Types (`@shared/types/crowdsourcing`)
- `PlantillaProyectoList`
- `PlantillaProyecto`
- `PlantillaProyectoNecesidad`
- `PlantillaResumen`
- `RolProfesional`
- `RolProfesionalConCategoria`
- `CategoriaRol`
- `GenerarNecesidadesRequest`
- `NecesidadSeleccionada`
- `GenerarNecesidadesResult`
- `PrioridadNecesidad`

### Schemas (`@shared/schemas/crowdsourcing`)
- `necesidadSeleccionadaSchema`
- `generarNecesidadesSchema`
- `NecesidadSeleccionadaFormData`
- `GenerarNecesidadesFormData`

### Constants (`@shared/constants`)
- `QUERY_KEYS.crowdsourcing.templates.all`
- `QUERY_KEYS.crowdsourcing.templates.byId(id)`
- `QUERY_KEYS.crowdsourcing.maestras.rolesProfesionales`
- `QUERY_KEYS.crowdsourcing.maestras.categoriasRol`
- `API_ROUTES.crowdsourcing.templates.base`
- `API_ROUTES.crowdsourcing.templates.byId(id)`
- `API_ROUTES.crowdsourcing.templates.generar(id)`
- `PRIORIDAD_NECESIDAD`
- `PRIORIDAD_NECESIDAD_COLORS`
- `MODALIDAD_COBRO`
- `FASES_PROYECTO`

### Utils (`@shared/utils/error-messages`)
- `ERROR_MESSAGES.TEMPLATE_NOT_FOUND`
- `ERROR_MESSAGES.PROYECTO_ARTISTICO_NOT_FOUND`
- `ERROR_MESSAGES.VALIDATION_PRESUPUESTO_MIN_NEGATIVO`
- `ERROR_MESSAGES.VALIDATION_PRESUPUESTO_MAX_MENOR_MIN`
- `ERROR_MESSAGES.VALIDATION_NECESIDADES_REQUERIDAS`

**IMPORTANTE:** NO duplicar types ni constants. SIEMPRE importar de shared.

---

## 8. Archivos a Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `domain/types.ts` | Re-export | Re-export de @shared/types/crowdsourcing |
| `domain/index.ts` | Index | Barrel export del domain |
| `application/hooks/useTemplates.ts` | Hook | Query hook para templates |
| `application/hooks/useTemplateDetail.ts` | Hook | Query hook para detalle |
| `application/hooks/useRolesProfesionales.ts` | Hook | Query hook para roles |
| `application/hooks/useGenerarNecesidades.ts` | Hook | Mutation hook para generar |
| `application/hooks/useWizardState.ts` | Hook | State management del wizard |
| `application/schemas.ts` | Re-export | Re-export de @shared/schemas/crowdsourcing |
| `application/index.ts` | Index | Barrel export de hooks |
| `infrastructure/api/crowdsourcing.api.ts` | Service | API service class |
| `infrastructure/index.ts` | Index | Barrel export de infrastructure |
| `presentation/components/WizardStepper.tsx` | Component | Progress indicator |
| `presentation/components/TemplateCard.tsx` | Component | Card de template |
| `presentation/components/TemplateGallery.tsx` | Component | Grid de templates |
| `presentation/components/NecesidadItem.tsx` | Component | Item de necesidad |
| `presentation/components/NecesidadList.tsx` | Component | Lista de necesidades |
| `presentation/components/BudgetSummary.tsx` | Component | Resumen presupuestario |
| `presentation/components/RolProfesionalTooltip.tsx` | Component | Tooltip de rol |
| `presentation/components/ConfirmationSummary.tsx` | Component | Resumen final |
| `presentation/components/ProyectoSelector.tsx` | Component | Selector de proyecto |
| `presentation/components/TemplateCardSkeleton.tsx` | Component | Loading skeleton |
| `presentation/components/NecesidadListSkeleton.tsx` | Component | Loading skeleton |
| `presentation/components/index.ts` | Index | Barrel export de componentes |
| `presentation/pages/NuevoProyectoPage.tsx` | Page | Container principal |
| `presentation/pages/index.ts` | Index | Barrel export de pages |
| `presentation/index.ts` | Index | Barrel export de presentation |
| `index.ts` | Index | Barrel export de feature |
| `__tests__/components/TemplateCard.test.tsx` | Test | Unit test para TemplateCard |
| `__tests__/components/NecesidadItem.test.tsx` | Test | Unit test para NecesidadItem |
| `__tests__/components/BudgetSummary.test.tsx` | Test | Unit test para BudgetSummary |
| `__tests__/hooks/useTemplates.test.ts` | Test | Unit test para useTemplates |
| `__tests__/hooks/useWizardState.test.ts` | Test | Unit test para useWizardState |

**Total:** 32 archivos (13 componentes, 5 hooks, 1 service, 5 tests, 8 index files)

---

## 9. Modificaciones a Archivos Existentes

### 9.1 `src/web/src/lib/constants.ts`

Agregar constantes de crowdsourcing (si no estan en shared):

```typescript
export const QUERY_KEYS = {
  // ... existing keys

  // Crowdsourcing Templates
  crowdsourcing: {
    templates: {
      all: ['crowdsourcing', 'templates'] as const,
      byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
    },
    maestras: {
      rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
      categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
    },
  },
} as const;
```

**NOTA:** Si estas constantes ya estan en `@shared/constants`, NO duplicar. Importar de shared.

### 9.2 `src/web/src/app/routes.tsx` (o donde se definen rutas)

Agregar ruta del wizard:

```tsx
import { NuevoProyectoPage } from '@/features/crowdsourcing';

// En router config
{
  path: '/crowdsourcing/nuevo-proyecto',
  element: <NuevoProyectoPage />,
  // Requiere autenticacion
  // Requiere rol Artista
}
```

### 9.3 `src/web/src/components/layout/Header.tsx` (o Navigation)

Agregar link al wizard en navegacion del artista:

```tsx
<Link to="/crowdsourcing/nuevo-proyecto">
  Crear Necesidades
</Link>
```

---

## 10. Checklist de Implementacion

### Domain Layer
- [ ] `domain/types.ts` - Re-export de @shared/types/crowdsourcing
- [ ] `domain/index.ts` - Barrel export

### Application Layer
- [ ] `application/hooks/useTemplates.ts` - Query hook templates
- [ ] `application/hooks/useTemplateDetail.ts` - Query hook detalle
- [ ] `application/hooks/useRolesProfesionales.ts` - Query hook roles
- [ ] `application/hooks/useGenerarNecesidades.ts` - Mutation hook generar
- [ ] `application/hooks/useWizardState.ts` - State management
- [ ] `application/schemas.ts` - Re-export de @shared/schemas
- [ ] `application/index.ts` - Barrel export

### Infrastructure Layer
- [ ] `infrastructure/api/crowdsourcing.api.ts` - API service
- [ ] `infrastructure/index.ts` - Barrel export

### Presentation Layer - Components
- [ ] `presentation/components/WizardStepper.tsx`
- [ ] `presentation/components/TemplateCard.tsx`
- [ ] `presentation/components/TemplateGallery.tsx`
- [ ] `presentation/components/NecesidadItem.tsx`
- [ ] `presentation/components/NecesidadList.tsx`
- [ ] `presentation/components/BudgetSummary.tsx`
- [ ] `presentation/components/RolProfesionalTooltip.tsx`
- [ ] `presentation/components/ConfirmationSummary.tsx`
- [ ] `presentation/components/ProyectoSelector.tsx`
- [ ] `presentation/components/TemplateCardSkeleton.tsx`
- [ ] `presentation/components/NecesidadListSkeleton.tsx`
- [ ] `presentation/components/index.ts`

### Presentation Layer - Pages
- [ ] `presentation/pages/NuevoProyectoPage.tsx`
- [ ] `presentation/pages/index.ts`
- [ ] `presentation/index.ts`

### Feature Root
- [ ] `index.ts` - Barrel export

### Testing
- [ ] `__tests__/components/TemplateCard.test.tsx`
- [ ] `__tests__/components/NecesidadItem.test.tsx`
- [ ] `__tests__/components/BudgetSummary.test.tsx`
- [ ] `__tests__/hooks/useTemplates.test.ts`
- [ ] `__tests__/hooks/useWizardState.test.ts`

### Integration
- [ ] Modificar `src/web/src/lib/constants.ts` (agregar QUERY_KEYS si no estan en shared)
- [ ] Modificar router para agregar ruta `/crowdsourcing/nuevo-proyecto`
- [ ] Agregar link en navegacion/menu

### UX/UI
- [ ] Componentes usan shadcn/ui (Card, Button, Input, Checkbox, etc.)
- [ ] Design tokens aplicados (colores, tipografia, espaciado)
- [ ] Responsive design (mobile-first)
- [ ] Loading states con skeletons
- [ ] Empty states con mensajes
- [ ] Error states con toast notifications
- [ ] ARIA labels para accesibilidad
- [ ] Focus states visibles
- [ ] Animaciones suaves (150-400ms)

### Validacion
- [ ] Validacion frontend con Zod schemas
- [ ] Validacion en tiempo real (presupuesto max >= min)
- [ ] Al menos 1 necesidad seleccionada para avanzar
- [ ] Proyecto artistico seleccionado para submit
- [ ] Mensajes de error descriptivos

---

## 11. Notas de Implementacion

### Routing con Query Params

Usar query params `?step=1|2|3&template={id}` en lugar de rutas separadas para:
- Mantener estado en URL (refrescable)
- Navegacion con browser back/forward funciona
- Compartir links a pasos especificos

### State Management

- **Global:** TanStack Query para server state (templates, roles)
- **Local:** `useWizardState` para wizard state (seleccion, presupuestos)
- **Opcional:** Persistir wizard state en localStorage para recuperar progreso

### Performance

- Templates y roles son datos maestros → `staleTime: 5-30 min`
- Lazy loading de iconos (dynamic import segun `template.icono`)
- Virtualizacion de lista si > 20 necesidades (react-window)
- Debounce en inputs de presupuesto (300ms)

### Accesibilidad

- Wizard keyboard navigable (Tab, Enter, Esc)
- ARIA labels en checkboxes y tooltips
- Focus states visibles en todos los interactivos
- Tooltips accesibles con teclado (Enter/Esc)
- Screen reader compatible

### Error Handling

- Query errors → Toast notification con mensaje de `ERROR_MESSAGES`
- Mutation errors → Toast + mantener form editable para retry
- Validacion errors → Mensaje inline debajo del input
- Network errors → Retry automatico (TanStack Query)

### Testing Strategy

- **Unit tests:** Componentes con logic (NecesidadItem, BudgetSummary)
- **Hook tests:** useWizardState, useGenerarNecesidades
- **Integration tests:** NuevoProyectoPage con mocked API
- **E2E:** Wizard completo (Paso 1 → 2 → 3 → Submit)

---

## 12. Siguiente Paso Sugerido

**Orden de implementacion recomendado:**

1. **Fase 1 - Foundation (2-3h)**
   - Crear estructura de carpetas
   - `domain/types.ts` (re-export shared)
   - `infrastructure/api/crowdsourcing.api.ts`
   - `application/hooks/useTemplates.ts`
   - Verificar imports de shared funcionan

2. **Fase 2 - Paso 1 del Wizard (3-4h)**
   - `presentation/components/WizardStepper.tsx`
   - `presentation/components/TemplateCard.tsx`
   - `presentation/components/TemplateGallery.tsx`
   - `presentation/components/TemplateCardSkeleton.tsx`
   - `presentation/pages/NuevoProyectoPage.tsx` (solo paso 1)
   - Testing: Renderiza galeria, selecciona template

3. **Fase 3 - Paso 2 del Wizard (4-5h)**
   - `application/hooks/useWizardState.ts`
   - `application/hooks/useTemplateDetail.ts`
   - `presentation/components/NecesidadItem.tsx`
   - `presentation/components/NecesidadList.tsx`
   - `presentation/components/BudgetSummary.tsx`
   - `presentation/components/RolProfesionalTooltip.tsx`
   - Integrar en `NuevoProyectoPage.tsx` paso 2
   - Testing: Toggle, presupuestos, validacion

4. **Fase 4 - Paso 3 del Wizard (2-3h)**
   - `presentation/components/ConfirmationSummary.tsx`
   - `presentation/components/ProyectoSelector.tsx`
   - `application/hooks/useGenerarNecesidades.ts`
   - Integrar en `NuevoProyectoPage.tsx` paso 3
   - Testing: Submit, success, error

5. **Fase 5 - Polish & Testing (2-3h)**
   - Unit tests completos
   - Skeletons y loading states
   - Animaciones y transiciones
   - Responsive design refinement
   - Accesibilidad ARIA
   - E2E tests

**Total estimado:** 13-18 horas (de 30h disponibles)

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (Arquitecto Frontend)
**Estado:** READY FOR IMPLEMENTATION
