# Plan Frontend: Hacer Backing (Landing)

**Fecha:** 2026-02-13
**Feature:** hacer-backing
**Target:** src/web (Landing - Vite + React)

## 1. Resumen

- Screens: 3 (CampaniasListPage, CampaniaDetailPage, BackingConfirmationPage)
- Componentes: 10 (CampaniaCard existe, 9 nuevos)
- Hooks: 4 (useCampanias existe, 3 nuevos)
- Services: 2 (campaniaService existe parcial, backingService existe básico)

## 2. Estructura de Carpetas

```
src/web/src/features/
├── campanias/
│   ├── domain/
│   │   └── index.ts                         # MODIFICAR - agregar CampaniaDetail type
│   ├── application/
│   │   ├── useCampanias.ts                  # YA EXISTE - mantener
│   │   ├── hooks/
│   │   │   ├── useCampaniaDetail.ts         # CREAR - query detalle completo
│   │   │   ├── useCampaniaStats.ts          # CREAR - query estadísticas (opcional)
│   │   │   └── index.ts                     # CREAR
│   │   ├── utils.ts                         # YA EXISTE - mantener
│   │   └── index.ts                         # MODIFICAR - export nuevos hooks
│   ├── infrastructure/
│   │   ├── campania.api.ts                  # YA EXISTE - revisar si tiene getById completo
│   │   ├── campania.service.ts              # MODIFICAR - agregar getDetail, getStats
│   │   └── index.ts                         # MANTENER
│   ├── presentation/
│   │   ├── components/
│   │   │   ├── CampaniaCard.tsx             # YA EXISTE - mantener
│   │   │   ├── CampaniaProgress.tsx         # YA EXISTE - reutilizar
│   │   │   ├── RewardPublicCard.tsx         # YA EXISTE - reutilizar
│   │   │   ├── CampaniaFilters.tsx          # CREAR - search + filtros
│   │   │   ├── BackingsRecentesList.tsx     # CREAR - lista backings recientes
│   │   │   ├── BackingRecenteCard.tsx       # CREAR - card individual de backing
│   │   │   └── index.ts                     # MODIFICAR - export nuevos
│   │   └── pages/
│   │       ├── CampaniasPage.tsx            # YA EXISTE - revisar si tiene filtros
│   │       ├── CampaniaDetailPage.tsx       # CREAR/MODIFICAR - detalle completo con sidebar rewards
│   │       └── index.ts                     # MODIFICAR
│   └── index.ts                             # MANTENER
│
└── backings/
    ├── domain/
    │   ├── types.ts                         # YA EXISTE - revisar contra shared/types/backing.ts
    │   └── index.ts                         # MANTENER
    ├── application/
    │   ├── schemas.ts                       # YA EXISTE - alinear con shared/schemas
    │   ├── hooks/
    │   │   ├── useCreateBacking.ts          # CREAR - mutation crear backing
    │   │   ├── useBackingsByCampania.ts     # CREAR - query backings de campaña
    │   │   └── index.ts                     # CREAR
    │   ├── useBackings.ts                   # YA EXISTE - mantener
    │   └── index.ts                         # MODIFICAR - export nuevos hooks
    ├── infrastructure/
    │   ├── backing.service.ts               # YA EXISTE - revisar y completar
    │   └── index.ts                         # MANTENER
    ├── presentation/
    │   ├── components/
    │   │   ├── BackingModal.tsx             # CREAR - modal formulario backing
    │   │   ├── BackingForm.tsx              # CREAR - formulario interno del modal
    │   │   ├── AmountInput.tsx              # CREAR - input monto con validación
    │   │   └── index.ts                     # CREAR
    │   ├── pages/
    │   │   ├── BackingConfirmationPage.tsx  # CREAR - confirmación post-backing
    │   │   └── index.ts                     # CREAR
    │   └── index.ts                         # CREAR
    └── index.ts                             # MODIFICAR

src/web/src/components/ui/
├── dialog.tsx                               # CREAR - shadcn Dialog (para BackingModal)
├── checkbox.tsx                             # CREAR - shadcn Checkbox (para anónimo)
└── select.tsx                               # CREAR - shadcn Select (para filtros)
```

## 3. Componentes

### 3.1 CampaniaCard

**Archivo:** `features/campanias/presentation/components/CampaniaCard.tsx`

**Estado:** ✅ YA EXISTE - Mantener sin cambios

**Responsabilidad:** Renderiza card de campaña en grid de listado

**Props existentes:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| campania | CampaniaListItem | Sí | Datos de campaña |
| variant | "default" \| "compact" | No | Variante de visualización |
| className | string | No | Clases CSS adicionales |

**Dependencias:**
- Hooks: ninguno (solo navigate)
- Componentes UI: Card, Badge, Users icon
- Utils: calcularDiasRestantes, calcularPorcentaje, formatCurrency

**Notas:**
- Ya renderiza imagen, título, progress bar, monto recaudado, backers count
- Ya navega a `/campanias/${id}` al hacer click
- Reutilizar sin modificaciones

### 3.2 CampaniaFilters

**Archivo:** `features/campanias/presentation/components/CampaniaFilters.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| onSearchChange | (search: string) => void | Sí | Callback al cambiar búsqueda |
| onEstadoChange | (estadoId?: number) => void | Sí | Callback al cambiar filtro estado |
| onSortChange | (sort: string) => void | Sí | Callback al cambiar ordenamiento |
| searchValue | string | No | Valor actual de búsqueda |
| estadoValue | number \| undefined | No | Estado seleccionado |
| sortValue | string | No | Ordenamiento actual |
| resultsCount | number | No | Cantidad de resultados |
| className | string | No | Clases CSS |

**Estado Local:**
- `debouncedSearch` (string) - debounce 500ms del search input

**Dependencias:**
- Hooks: `useDebounce` (ya existe en `src/web/src/hooks/useDebounce.ts`)
- Componentes UI: Input, Select, Badge
- Icons: Search, Filter, SortAsc

**Responsabilidad:**
Renderiza barra de filtros con search, dropdown estado, dropdown ordenamiento, y contador de resultados.

**Layout:**
```tsx
<div className="flex flex-col md:flex-row gap-4 mb-6">
  {/* Search bar */}
  <div className="relative flex-1">
    <Search icon />
    <Input placeholder="Buscar..." />
  </div>

  {/* Filters */}
  <div className="flex gap-2">
    <Select placeholder="Estado">
      <Option>Todas</Option>
      <Option>Activas</Option>
      <Option>Próximas a finalizar</Option>
    </Select>

    <Select placeholder="Ordenar">
      <Option>Más recientes</Option>
      <Option>Más populares</Option>
      <Option>Finalizan pronto</Option>
    </Select>
  </div>

  {/* Results count */}
  <div className="text-sm text-muted">
    {resultsCount} campañas
  </div>
</div>
```

### 3.3 BackingsRecentesList

**Archivo:** `features/campanias/presentation/components/BackingsRecentesList.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| backings | BackingPublicDto[] | Sí | Lista de backings recientes |
| isLoading | boolean | No | Estado de carga |
| emptyMessage | string | No | Mensaje cuando vacío |
| className | string | No | Clases CSS |

**Estado Local:**
Ninguno (stateless)

**Dependencias:**
- Componentes: BackingRecenteCard
- Componentes UI: Skeleton

**Responsabilidad:**
Renderiza lista de backings recientes usando BackingRecenteCard.

**Layout:**
```tsx
<div className="space-y-3">
  <h3 className="text-lg font-bold text-white mb-4">
    Apoyos Recientes
  </h3>

  {isLoading ? (
    <SkeletonList count={5} />
  ) : backings.length === 0 ? (
    <EmptyState message={emptyMessage} />
  ) : (
    backings.map(backing => (
      <BackingRecenteCard key={backing.id} backing={backing} />
    ))
  )}
</div>
```

### 3.4 BackingRecenteCard

**Archivo:** `features/campanias/presentation/components/BackingRecenteCard.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| backing | BackingPublicDto | Sí | Datos del backing |
| className | string | No | Clases CSS |

**Estado Local:**
Ninguno

**Dependencias:**
- Utils: `formatCurrency` (ya existe), `formatRelativeTime` (crear)
- Icons: User, EyeOff (para anónimo)

**Responsabilidad:**
Renderiza card individual de backing con nombre (o "Anónimo"), monto, reward, fecha relativa.

**Layout:**
```tsx
<div className="flex items-start gap-3 p-3 bg-[#1a1a2e] rounded-lg">
  <Avatar className="w-10 h-10">
    {esAnonimo ? <EyeOff /> : <User />}
  </Avatar>

  <div className="flex-1 min-w-0">
    <p className="text-sm font-semibold text-white">
      {nombreBacker}
    </p>
    <p className="text-xs text-muted">
      Aportó {formatCurrency(monto)} {rewardNombre && `· ${rewardNombre}`}
    </p>
    {mensaje && (
      <p className="text-xs text-muted italic mt-1 line-clamp-2">
        "{mensaje}"
      </p>
    )}
  </div>

  <span className="text-xs text-muted whitespace-nowrap">
    {formatRelativeTime(fechaCreacion)}
  </span>
</div>
```

### 3.5 BackingModal

**Archivo:** `features/backings/presentation/components/BackingModal.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| open | boolean | Sí | Estado del modal |
| onOpenChange | (open: boolean) => void | Sí | Callback cambio estado |
| campaniaId | string | Sí | ID de campaña |
| campaniaTitulo | string | Sí | Título campaña |
| reward | RewardPublic \| null | No | Reward pre-seleccionado |
| onSuccess | () => void | No | Callback después de backing exitoso |

**Estado Local:**
- `selectedReward` (RewardPublic | null) - Reward actualmente seleccionado
- `showRewardSelector` (boolean) - Mostrar modal selector de rewards

**Dependencias:**
- Hooks: `useCreateBacking`
- Componentes: BackingForm, RewardSelectionModal
- Componentes UI: Dialog, DialogContent, DialogHeader, DialogTitle

**Responsabilidad:**
Modal contenedor que maneja apertura/cierre, contiene BackingForm, maneja selección de reward.

**Layout:**
```tsx
<Dialog open={open} onOpenChange={onOpenChange}>
  <DialogContent className="max-w-2xl">
    <DialogHeader>
      <DialogTitle>Apoya a {campaniaTitulo}</DialogTitle>
    </DialogHeader>

    <BackingForm
      campaniaId={campaniaId}
      reward={selectedReward}
      onChangeReward={() => setShowRewardSelector(true)}
      onSuccess={() => {
        onSuccess?.()
        onOpenChange(false)
      }}
    />
  </DialogContent>

  {/* Modal interno para cambiar reward */}
  <RewardSelectionModal
    open={showRewardSelector}
    onOpenChange={setShowRewardSelector}
    campaniaId={campaniaId}
    onSelect={(reward) => {
      setSelectedReward(reward)
      setShowRewardSelector(false)
    }}
  />
</Dialog>
```

### 3.6 BackingForm

**Archivo:** `features/backings/presentation/components/BackingForm.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| campaniaId | string | Sí | ID de campaña |
| reward | RewardPublic \| null | No | Reward seleccionado |
| onChangeReward | () => void | No | Callback para cambiar reward |
| onSuccess | () => void | No | Callback post-submit exitoso |

**Estado Local:**
- `form` (react-hook-form) - Estado del formulario
- `isSubmitting` (boolean) - Estado de envío

**Dependencias:**
- Hooks: `useCreateBacking`, `useForm` (react-hook-form)
- Schemas: `createBackingSchema` (de shared/schemas)
- Componentes: AmountInput
- Componentes UI: Button, Textarea, Checkbox, Label, Card

**Responsabilidad:**
Formulario de backing con validación, maneja submit, muestra errores.

**Layout:**
```tsx
<form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
  {/* Selected Reward Card */}
  <Card className="bg-[#1a1a2e] p-4">
    <div className="flex justify-between">
      <div>
        <h4>{reward?.nombre || "Sin recompensa"}</h4>
        <p className="text-sm text-muted">
          Mínimo {formatCurrency(reward?.importeMinimo || 5)}
        </p>
      </div>
      <Button variant="link" onClick={onChangeReward}>
        Cambiar
      </Button>
    </div>
  </Card>

  {/* Amount Input */}
  <AmountInput
    value={form.watch("monto")}
    onChange={(val) => form.setValue("monto", val)}
    minAmount={reward?.importeMinimo || 5}
    error={form.formState.errors.monto?.message}
  />

  {/* Quick Amount Buttons */}
  <div className="flex gap-2">
    <Button type="button" variant="outline" size="sm" onClick={() => addAmount(5)}>
      +€5
    </Button>
    <Button type="button" variant="outline" size="sm" onClick={() => addAmount(10)}>
      +€10
    </Button>
    <Button type="button" variant="outline" size="sm" onClick={() => addAmount(25)}>
      +€25
    </Button>
  </div>

  {/* Message */}
  <div>
    <Label htmlFor="mensaje">Mensaje para el artista (opcional)</Label>
    <Textarea
      id="mensaje"
      {...form.register("mensaje")}
      placeholder="Escribe un mensaje de apoyo..."
      maxLength={500}
    />
    <p className="text-xs text-muted mt-1">
      {form.watch("mensaje")?.length || 0}/500
    </p>
  </div>

  {/* Anonymous Checkbox */}
  <div className="flex items-center gap-2">
    <Checkbox
      id="esAnonimo"
      {...form.register("esAnonimo")}
    />
    <Label htmlFor="esAnonimo">
      <EyeOff className="inline w-4 h-4 mr-1" />
      Hacer anónimo mi apoyo
    </Label>
  </div>

  {/* Summary */}
  <Card className="bg-[#1a1a2e] p-4">
    <h4 className="font-bold mb-3">Resumen</h4>
    <div className="space-y-2 text-sm">
      <div className="flex justify-between">
        <span>Aportación:</span>
        <span className="font-semibold">
          {formatCurrency(form.watch("monto"))}
        </span>
      </div>
      {reward && (
        <div className="flex justify-between">
          <span>Reward:</span>
          <span className="text-muted">{reward.nombre}</span>
        </div>
      )}
      <div className="flex justify-between text-lg font-bold border-t pt-2">
        <span>Total:</span>
        <span className="text-primary">
          {formatCurrency(form.watch("monto"))}
        </span>
      </div>
    </div>
  </Card>

  {/* Submit */}
  <div className="flex gap-3">
    <Button type="button" variant="outline" onClick={onClose}>
      Cancelar
    </Button>
    <Button
      type="submit"
      className="flex-1 bg-gradient-to-r from-pink-500 to-purple-600"
      disabled={isSubmitting}
    >
      {isSubmitting ? "Procesando..." : "Confirmar Apoyo"}
    </Button>
  </div>
</form>
```

### 3.7 AmountInput

**Archivo:** `features/backings/presentation/components/AmountInput.tsx`

**Estado:** 🆕 CREAR

**Props:**
| Prop | Tipo | Requerido | Descripción |
|------|------|-----------|-------------|
| value | number | Sí | Valor actual |
| onChange | (value: number) => void | Sí | Callback cambio valor |
| minAmount | number | No | Monto mínimo (default 5) |
| maxAmount | number | No | Monto máximo (default 10000) |
| error | string | No | Mensaje de error |
| hint | string | No | Texto de ayuda |
| className | string | No | Clases CSS |

**Estado Local:**
Ninguno (controlled component)

**Dependencias:**
- Componentes UI: Input, Label
- Icons: Euro

**Responsabilidad:**
Input de monto con símbolo €, validación inline, formato currency.

**Layout:**
```tsx
<div className={className}>
  <Label htmlFor="monto">Monto a Aportar</Label>
  <div className="relative">
    <span className="absolute left-4 top-1/2 -translate-y-1/2 text-2xl text-muted">
      €
    </span>
    <Input
      id="monto"
      type="number"
      step="0.01"
      value={value}
      onChange={(e) => onChange(Number(e.target.value))}
      className="text-2xl font-bold text-center pl-12 pr-4 py-4"
      min={minAmount}
      max={maxAmount}
      aria-invalid={!!error}
      aria-describedby={error ? "monto-error" : "monto-hint"}
    />
  </div>
  {error && (
    <p id="monto-error" className="text-xs text-red-500 mt-1">
      {error}
    </p>
  )}
  {hint && !error && (
    <p id="monto-hint" className="text-xs text-muted mt-1 italic">
      {hint}
    </p>
  )}
</div>
```

### 3.8 CampaniaDetailPage

**Archivo:** `features/campanias/presentation/pages/CampaniaDetailPage.tsx`

**Estado:** 🔄 CREAR/MODIFICAR (si existe, reemplazar con versión completa)

**Props:**
Ninguno (usa `useParams()` para obtener id)

**Estado Local:**
- `showBackingModal` (boolean) - Estado del modal de backing
- `selectedReward` (RewardPublic | null) - Reward seleccionado para backing

**Dependencias:**
- Hooks: `useCampaniaDetail`, `useRewardsByCampania`, `useBackingsByCampania`
- Componentes: CampaniaHero, CampaniaTabs, CampaniaProgress, CampaniaStats, RewardPublicCard, BackingsRecentesList, BackingModal
- Router: useParams, useNavigate

**Responsabilidad:**
Página de detalle completa de campaña con grid layout (main content + sidebar sticky).

**Layout:**
```tsx
<div className="min-h-screen bg-[#0a0a0f] pb-16">
  <div className="max-w-7xl mx-auto px-4 py-8">
    {isLoading && <CampaniaDetailSkeleton />}

    {error && <ErrorState />}

    {data && (
      <div className="grid lg:grid-cols-[1fr_380px] gap-8">
        {/* Main Content */}
        <div className="space-y-8">
          <CampaniaHero
            imagenUrl={data.imagenPrincipalUrl}
            videoUrl={data.videoPrincipalUrl}
          />

          <div className="mb-6">
            <h1 className="text-3xl md:text-4xl font-bold mb-3">
              {data.titulo}
            </h1>
            <div className="flex items-center gap-3">
              <Avatar />
              <Link to={`/artistas/${data.artistaId}`}>
                {data.artistaNombre}
              </Link>
              <Badge>✓ Verificado</Badge>
            </div>
          </div>

          <CampaniaProgress
            importeObjetivo={data.importeObjetivo}
            importePledgedActual={data.importePledgedActual}
            porcentajeProgreso={data.porcentajeProgreso}
          />

          <CampaniaStats
            totalBackers={data.totalBackers}
            diasRestantes={data.diasRestantes}
          />

          <CampaniaTabs
            descripcion={data.descripcionCorta}
            backingsRecientes={data.backingsRecientes}
          />
        </div>

        {/* Sidebar Sticky */}
        <div className="sticky top-24 space-y-6 h-fit">
          <Button
            className="w-full bg-gradient-to-r from-pink-500 to-purple-600"
            size="lg"
            onClick={() => setShowBackingModal(true)}
          >
            Apoyar esta campaña
          </Button>

          <p className="text-center text-sm text-muted">
            Desde €{data.rewards[0]?.importeMinimo || 5}
          </p>

          <div>
            <h3 className="text-lg font-bold mb-4">Recompensas</h3>
            <div className="space-y-3">
              {data.rewards.map(reward => (
                <RewardPublicCard
                  key={reward.id}
                  reward={reward}
                  onSelect={(rewardId) => handleSelectReward(rewardId)}
                />
              ))}
            </div>
          </div>

          <div className="border-t border-muted pt-4 space-y-2 text-xs text-muted">
            <div className="flex items-center gap-2">
              <Lock className="w-4 h-4" />
              <span>Pago seguro</span>
            </div>
            <div className="flex items-center gap-2">
              <Truck className="w-4 h-4" />
              <span>Entrega: {estimatedDelivery}</span>
            </div>
          </div>
        </div>
      </div>
    )}
  </div>

  {/* Backing Modal */}
  <BackingModal
    open={showBackingModal}
    onOpenChange={setShowBackingModal}
    campaniaId={id}
    campaniaTitulo={data?.titulo || ""}
    reward={selectedReward}
    onSuccess={() => navigate(`/campanias/${id}/confirmacion`)}
  />
</div>
```

### 3.9 BackingConfirmationPage

**Archivo:** `features/backings/presentation/pages/BackingConfirmationPage.tsx`

**Estado:** 🆕 CREAR

**Props:**
Ninguno (usa `useParams()` y `useSearchParams()`)

**Estado Local:**
Ninguno (datos vienen de query params o server)

**Dependencias:**
- Hooks: `useParams`, `useSearchParams`, `useCampania`
- Componentes UI: Card, Button, Badge
- Icons: CheckCircle, HeartHandshake, Mail

**Responsabilidad:**
Página de confirmación post-backing con resumen y next steps.

**Layout:**
```tsx
<div className="min-h-screen bg-[#0a0a0f] flex items-center justify-center py-8 px-4">
  <Card className="max-w-2xl mx-auto bg-[#0f1729] p-8 text-center">
    {/* Success Icon */}
    <div className="w-24 h-24 mx-auto mb-6 rounded-full bg-green-500/20 flex items-center justify-center">
      <HeartHandshake className="w-12 h-12 text-green-400" />
    </div>

    {/* Title */}
    <h1 className="text-3xl font-bold text-white mb-4">
      ¡Gracias por tu apoyo!
    </h1>
    <p className="text-lg text-white/80 mb-2">
      Tu contribución ha sido confirmada con éxito.
    </p>
    <p className="text-muted mb-8">
      Has ayudado a {artistaNombre} a alcanzar su sueño musical.
    </p>

    {/* Summary Card */}
    <Card className="bg-[#1a1a2e] p-6 mb-6 text-left">
      <h3 className="text-xl font-bold mb-4 text-center">
        Resumen de tu Apoyo
      </h3>
      <div className="space-y-2 text-sm">
        <div className="flex justify-between border-b border-muted pb-2">
          <span className="text-muted">Campaña:</span>
          <span className="text-white font-semibold">{campaniaTitulo}</span>
        </div>
        <div className="flex justify-between border-b border-muted pb-2">
          <span className="text-muted">Artista:</span>
          <span className="text-white">{artistaNombre}</span>
        </div>
        <div className="flex justify-between border-b border-muted pb-2">
          <span className="text-muted">Monto:</span>
          <span className="text-primary font-bold text-lg">
            {formatCurrency(monto)}
          </span>
        </div>
        {reward && (
          <div className="flex justify-between border-b border-muted pb-2">
            <span className="text-muted">Reward:</span>
            <span className="text-white">{rewardNombre}</span>
          </div>
        )}
        <div className="flex justify-between border-b border-muted pb-2">
          <span className="text-muted">Fecha:</span>
          <span className="text-white">{formatDate(fechaCreacion)}</span>
        </div>
        <div className="flex justify-between pb-2">
          <span className="text-muted">ID Backing:</span>
          <span className="text-white font-mono text-xs">{backingId}</span>
        </div>
      </div>

      {/* Email Notice */}
      <div className="flex items-start gap-2 bg-[#1e293b] p-3 rounded-lg mt-4">
        <Mail className="w-5 h-5 text-primary mt-0.5 flex-shrink-0" />
        <p className="text-xs text-white/80">
          Te hemos enviado un email de confirmación a <strong>{userEmail}</strong>
        </p>
      </div>
    </Card>

    {/* Next Steps */}
    <div className="text-left mb-8">
      <h4 className="text-lg font-bold mb-3">¿Qué sigue?</h4>
      <ul className="space-y-2 text-sm text-muted">
        <li className="flex items-start gap-2">
          <CheckCircle className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
          <span>El artista procesará tu recompensa</span>
        </li>
        <li className="flex items-start gap-2">
          <CheckCircle className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
          <span>Te notificaremos sobre actualizaciones de la campaña</span>
        </li>
        {reward && (
          <li className="flex items-start gap-2">
            <CheckCircle className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
            <span>Recibirás tu recompensa en: {estimatedDelivery}</span>
          </li>
        )}
      </ul>
    </div>

    {/* Actions */}
    <div className="flex gap-3 justify-center">
      <Button
        variant="outline"
        onClick={() => navigate(`/campanias/${campaniaId}`)}
      >
        Ver Campaña
      </Button>
      <Button
        className="bg-gradient-to-r from-pink-500 to-purple-600"
        onClick={() => navigate("/campanias")}
      >
        Explorar Más Campañas
      </Button>
    </div>
  </Card>
</div>
```

### 3.10 CampaniasPage (Modificar)

**Archivo:** `features/campanias/presentation/pages/CampaniasPage.tsx`

**Estado:** 🔄 MODIFICAR (agregar filtros)

**Cambios necesarios:**
- Agregar `CampaniaFilters` component
- Agregar estado para filtros (search, estado, sort)
- Pasar filtros a `useCampanias` hook
- Agregar paginación (opcional para MVP)

**Ejemplo:**
```tsx
export default function CampaniasPage() {
  const [search, setSearch] = useState("")
  const [estadoId, setEstadoId] = useState<number | undefined>(2) // Default: PUBLICADA
  const [sort, setSort] = useState("recent")

  const { data, isLoading, error, refetch } = useCampanias({
    searchTerm: search,
    estadoCampaniaId: estadoId,
    // sort mapping
  })

  return (
    <div className="min-h-screen bg-[#0a0a0f]">
      <div className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold">Campañas</h1>
          <p className="text-muted">
            Explora campañas activas y apoya a tus artistas favoritos
          </p>
        </div>

        {/* Filters */}
        <CampaniaFilters
          onSearchChange={setSearch}
          onEstadoChange={setEstadoId}
          onSortChange={setSort}
          searchValue={search}
          estadoValue={estadoId}
          sortValue={sort}
          resultsCount={data?.length || 0}
        />

        {/* List */}
        <CampaniaList
          campanias={data || []}
          isLoading={isLoading}
          error={error}
          onRetry={refetch}
        />
      </div>
    </div>
  )
}
```

## 4. Hooks

### 4.1 useCampaniaDetail

**Archivo:** `features/campanias/application/hooks/useCampaniaDetail.ts`

**Tipo:** Query Hook

**Parámetros:**
| Param | Tipo | Descripción |
|-------|------|-------------|
| id | string | ID de campaña |

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | CampaniaDetail \| undefined | Datos completos de campaña |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |
| refetch | () => void | Función para refrescar |

**Query Key:** `[QUERY_KEYS.CAMPANIA, id, "detail"]`

**Implementación:**
```typescript
import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { campaniaService } from "../../infrastructure/campania.service"

export function useCampaniaDetail(id: string) {
  return useQuery({
    queryKey: [QUERY_KEYS.CAMPANIA, id, "detail"],
    queryFn: () => campaniaService.getDetail(id),
    enabled: !!id,
    staleTime: 2 * 60 * 1000, // 2 min (más corto que useCampanias para stats actualizadas)
  })
}
```

### 4.2 useCampaniaStats (Opcional)

**Archivo:** `features/campanias/application/hooks/useCampaniaStats.ts`

**Tipo:** Query Hook

**Parámetros:**
| Param | Tipo | Descripción |
|-------|------|-------------|
| id | string | ID de campaña |

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | CampaniaStats \| undefined | Estadísticas agregadas |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Query Key:** `[QUERY_KEYS.CAMPANIA, id, "stats"]`

**Implementación:**
```typescript
import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { campaniaService } from "../../infrastructure/campania.service"

export function useCampaniaStats(id: string) {
  return useQuery({
    queryKey: [QUERY_KEYS.CAMPANIA, id, "stats"],
    queryFn: () => campaniaService.getStats(id),
    enabled: !!id,
    staleTime: 1 * 60 * 1000, // 1 min
  })
}
```

**Nota MVP:** Este hook es opcional. Si `useCampaniaDetail` ya retorna las stats necesarias en `CampaniaDetail`, no es necesario crear un endpoint separado.

### 4.3 useCreateBacking

**Archivo:** `features/backings/application/hooks/useCreateBacking.ts`

**Tipo:** Mutation Hook

**Parámetros:**
Ninguno (usa `mutate(data)` al ejecutar)

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| mutate | (data: CreateBackingRequest) => void | Función para ejecutar mutación |
| mutateAsync | (data: CreateBackingRequest) => Promise<BackingDto> | Versión async |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |
| data | BackingDto \| undefined | Resultado si exitoso |

**Acciones:**
- `mutate(data)` - Ejecutar mutación
- `onSuccess` - Invalidar queries de campaña, toast success, redirect
- `onError` - Toast error con mensaje contextual

**Implementación:**
```typescript
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { QUERY_KEYS } from "@/lib/constants"
import { backingService } from "../../infrastructure/backing.service"
import type { CreateBackingRequest } from "@shared/types/backing"
import { toast } from "sonner" // o el sistema de toast que uses

export function useCreateBacking() {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (data: CreateBackingRequest & { campaniaId: string }) =>
      backingService.create(data),

    onSuccess: (result, variables) => {
      // Invalidar queries de campaña para refrescar stats
      queryClient.invalidateQueries({
        queryKey: [QUERY_KEYS.CAMPANIA, variables.campaniaId],
      })

      // Invalidar lista de backings de campaña
      queryClient.invalidateQueries({
        queryKey: [QUERY_KEYS.BACKINGS, "campania", variables.campaniaId],
      })

      // Toast success
      toast.success("¡Apoyo confirmado!", {
        description: `Has apoyado con €${variables.monto}`,
      })

      // Redirect a confirmación
      navigate(`/campanias/${variables.campaniaId}/confirmacion?backingId=${result.id}`)
    },

    onError: (error: any) => {
      // Extraer mensaje de error del backend (ServiceResponse)
      const message =
        error.response?.data?.messages?.[0]?.message ||
        "Error al procesar tu apoyo. Intenta nuevamente."

      toast.error("Error al procesar apoyo", {
        description: message,
      })
    },
  })
}
```

### 4.4 useBackingsByCampania

**Archivo:** `features/backings/application/hooks/useBackingsByCampania.ts`

**Tipo:** Query Hook

**Parámetros:**
| Param | Tipo | Descripción |
|-------|------|-------------|
| campaniaId | string | ID de campaña |
| options | { pageNumber?: number, pageSize?: number } | Paginación opcional |

**Retorna:**
| Campo | Tipo | Descripción |
|-------|------|-------------|
| data | BackingPublicDto[] \| undefined | Lista de backings |
| isLoading | boolean | Estado de carga |
| error | Error \| null | Error si hay |

**Query Key:** `[QUERY_KEYS.BACKINGS, "campania", campaniaId, options]`

**Implementación:**
```typescript
import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { backingService } from "../../infrastructure/backing.service"

export function useBackingsByCampania(
  campaniaId: string,
  options?: { pageNumber?: number; pageSize?: number }
) {
  return useQuery({
    queryKey: [QUERY_KEYS.BACKINGS, "campania", campaniaId, options],
    queryFn: () => backingService.getByCampaniaId(campaniaId, options),
    enabled: !!campaniaId,
    staleTime: 1 * 60 * 1000, // 1 min
  })
}
```

**Nota:** Si `useCampaniaDetail` ya retorna los backings recientes en `CampaniaDetail.backingsRecientes`, este hook puede ser opcional o usado solo para paginación.

## 5. Services

### 5.1 campaniaService (Modificar)

**Archivo:** `features/campanias/infrastructure/campania.service.ts`

**Estado:** 🔄 MODIFICAR - Agregar métodos nuevos

**Métodos Nuevos:**

| Método | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getDetail | id: string | CampaniaDetail | GET /api/campanias/{id} (extendido) |
| getStats | id: string | CampaniaStats | GET /api/campanias/{id}/stats |

**Implementación:**
```typescript
// AGREGAR a campania.service.ts existente

import type { CampaniaDetail, CampaniaStats } from "@shared/types/campania"

export const campaniaService = {
  // ... métodos existentes (getAll, getById, create, update, publicar)

  /**
   * Obtiene detalle completo de campaña con rewards y backings
   */
  async getDetail(id: string): Promise<CampaniaDetail> {
    const response = await apiFetch<ServiceResponse<CampaniaDetail>>(
      `/campanias/${id}`
    )

    if (!response.data) {
      throw new Error("Campaña no encontrada")
    }

    return response.data
  },

  /**
   * Obtiene estadísticas agregadas de campaña (OPCIONAL)
   */
  async getStats(id: string): Promise<CampaniaStats> {
    const response = await apiFetch<ServiceResponse<CampaniaStats>>(
      `/campanias/${id}/stats`
    )

    if (!response.data) {
      throw new Error("No se pudieron cargar las estadísticas")
    }

    return response.data
  },
}
```

**Nota:** Si el endpoint `GET /api/campanias/{id}` ya retorna `CampaniaDetail` completo (con rewards y backingsRecientes), entonces `getDetail` es igual a `getById` y podemos reutilizarlo. Solo agregar `getStats` si el backend lo implementa por separado.

### 5.2 backingService (Modificar)

**Archivo:** `features/backings/infrastructure/backing.service.ts`

**Estado:** 🔄 MODIFICAR - Alinear con contracts y agregar métodos

**Métodos a Revisar/Agregar:**

| Método | Input | Output | Endpoint |
|--------|-------|--------|----------|
| create | data: CreateBackingRequest & { campaniaId } | BackingDto | POST /api/campanias/{campaniaId}/backings |
| getByCampaniaId | campaniaId: string, options?: PaginationParams | BackingPublicDto[] | GET /api/campanias/{campaniaId}/backings |
| getMyBackings | - | BackingDto[] | GET /api/backings/me |

**Implementación Actualizada:**
```typescript
import { apiFetch } from "@/lib/api-client"
import type {
  BackingDto,
  BackingPublicDto,
  CreateBackingRequest,
} from "@shared/types/backing"

interface ServiceResponse<T> {
  data: T
  messages: Array<{ message: string; errorCode: string }>
}

interface PaginationParams {
  pageNumber?: number
  pageSize?: number
}

class BackingService {
  /**
   * Crea un nuevo backing
   */
  async create(
    data: CreateBackingRequest & { campaniaId: string }
  ): Promise<BackingDto> {
    const { campaniaId, ...backingData } = data

    const response = await apiFetch<ServiceResponse<BackingDto>>(
      `/campanias/${campaniaId}/backings`,
      {
        method: "POST",
        data: backingData,
      }
    )

    if (!response.data) {
      throw new Error(
        response.messages?.[0]?.message || "Error al crear backing"
      )
    }

    return response.data
  }

  /**
   * Obtiene backings públicos de una campaña (con paginación)
   */
  async getByCampaniaId(
    campaniaId: string,
    options?: PaginationParams
  ): Promise<BackingPublicDto[]> {
    const params = new URLSearchParams()
    if (options?.pageNumber) params.append("pageNumber", String(options.pageNumber))
    if (options?.pageSize) params.append("pageSize", String(options.pageSize))

    const queryString = params.toString() ? `?${params.toString()}` : ""

    const response = await apiFetch<ServiceResponse<BackingPublicDto[]>>(
      `/campanias/${campaniaId}/backings${queryString}`
    )

    return response.data || []
  }

  /**
   * Obtiene backings del usuario autenticado
   */
  async getMyBackings(): Promise<BackingDto[]> {
    const response = await apiFetch<ServiceResponse<BackingDto[]>>(
      "/backings/me"
    )

    return response.data || []
  }
}

export const backingService = new BackingService()
```

**Cambios respecto al existente:**
- Alinear tipos con `@shared/types/backing` (usar `CreateBackingRequest`, `BackingPublicDto`)
- Quitar `userId` del create (se obtiene del JWT en backend)
- Agregar soporte de paginación en `getByCampaniaId`
- Mejorar manejo de errores (extraer mensaje de ServiceResponse)

## 6. Flujo de Datos

```
User Action (Click "Apoyar" en CampaniaDetailPage)
    ↓
Component (BackingModal abierto)
    ↓
Component (BackingForm - user llena monto, mensaje, anónimo)
    ↓
Hook (useCreateBacking.mutate({ campaniaId, rewardId, monto, mensaje, esAnonimo }))
    ↓
Service (backingService.create → POST /api/campanias/{id}/backings)
    ↓
Backend (CreateBackingCommand handler)
    ↓
Response (BackingDto con id, fecha, etc.)
    ↓
Hook onSuccess (invalidate queries, toast, navigate to confirmation)
    ↓
Component (BackingConfirmationPage renderiza resumen)
```

**Flujo de Datos para Listado de Campañas:**
```
User navega a /campanias
    ↓
Component (CampaniasPage monta)
    ↓
Hook (useCampanias({ estadoCampaniaId: 2 }))
    ↓
Service (campaniaService.getAll → GET /api/campanias?estadoCampaniaId=2)
    ↓
Backend (GetAllCampaniasQuery handler)
    ↓
Response (CampaniaListItem[])
    ↓
Component (CampaniaList renderiza CampaniaCard[])
```

**Flujo de Datos para Detalle de Campaña:**
```
User navega a /campanias/{id}
    ↓
Component (CampaniaDetailPage monta)
    ↓
Hook (useCampaniaDetail(id))
    ↓
Service (campaniaService.getDetail → GET /api/campanias/{id})
    ↓
Backend (GetCampaniaDetailQuery handler)
    ↓
Response (CampaniaDetail con rewards y backingsRecientes)
    ↓
Component (renderiza hero, tabs, sidebar con RewardPublicCard[])
    ↓
User hace click en "Seleccionar" en RewardCard
    ↓
Component (abre BackingModal con reward pre-seleccionado)
```

## 7. Dependencias de Shared

**Importar de `@shared/`:**

### Types
- `@shared/types/backing`: `BackingDto`, `BackingPublicDto`, `CreateBackingRequest`, `CampaniaStats`
- `@shared/types/campania`: `CampaniaDetail`, `RewardPublic`

### Schemas
- `@shared/schemas/backing.schema`: `createBackingSchema`, `validateBackingAmount`, `validateRewardAvailability`, `validateCampaniaActive`

### Constants
- `@shared/constants`: `QUERY_KEYS.backings.*`, `API_ROUTES.backings.*`, `APP_ROUTES.landing.campanias.*`, `ESTADO_PEDIDO`, `CAMPANIA_ESTADOS`

### Utils
- `@shared/utils/error-messages`: `getBackingErrorMessage`, `getBackingContextualError`
- `@shared/utils/format`: `getBackerDisplayName`

## 8. Archivos a Crear/Modificar

| Archivo | Tipo | Descripción |
|---------|------|-------------|
| `features/campanias/domain/index.ts` | MODIFICAR | Re-exportar `CampaniaDetail` de shared |
| `features/campanias/application/hooks/useCampaniaDetail.ts` | CREAR | Query hook para detalle completo |
| `features/campanias/application/hooks/useCampaniaStats.ts` | CREAR | Query hook para stats (opcional) |
| `features/campanias/application/hooks/index.ts` | CREAR | Exportar hooks nuevos |
| `features/campanias/infrastructure/campania.service.ts` | MODIFICAR | Agregar getDetail, getStats |
| `features/campanias/presentation/components/CampaniaFilters.tsx` | CREAR | Barra de filtros y search |
| `features/campanias/presentation/components/BackingsRecentesList.tsx` | CREAR | Lista de backings recientes |
| `features/campanias/presentation/components/BackingRecenteCard.tsx` | CREAR | Card individual de backing |
| `features/campanias/presentation/components/index.ts` | MODIFICAR | Exportar nuevos componentes |
| `features/campanias/presentation/pages/CampaniasPage.tsx` | MODIFICAR | Agregar CampaniaFilters |
| `features/campanias/presentation/pages/CampaniaDetailPage.tsx` | CREAR | Página completa de detalle |
| `features/campanias/presentation/pages/index.ts` | MODIFICAR | Exportar CampaniaDetailPage |
| `features/backings/domain/types.ts` | REVISAR | Alinear con @shared/types/backing |
| `features/backings/application/schemas.ts` | REVISAR | Alinear con @shared/schemas/backing.schema |
| `features/backings/application/hooks/useCreateBacking.ts` | CREAR | Mutation hook crear backing |
| `features/backings/application/hooks/useBackingsByCampania.ts` | CREAR | Query hook backings de campaña |
| `features/backings/application/hooks/index.ts` | CREAR | Exportar hooks |
| `features/backings/infrastructure/backing.service.ts` | MODIFICAR | Alinear con contracts |
| `features/backings/presentation/components/BackingModal.tsx` | CREAR | Modal contenedor |
| `features/backings/presentation/components/BackingForm.tsx` | CREAR | Formulario interno |
| `features/backings/presentation/components/AmountInput.tsx` | CREAR | Input de monto |
| `features/backings/presentation/components/index.ts` | CREAR | Exportar componentes |
| `features/backings/presentation/pages/BackingConfirmationPage.tsx` | CREAR | Página confirmación |
| `features/backings/presentation/pages/index.ts` | CREAR | Exportar página |
| `features/backings/index.ts` | MODIFICAR | Exportar presentation |
| `components/ui/dialog.tsx` | CREAR | shadcn Dialog component |
| `components/ui/checkbox.tsx` | CREAR | shadcn Checkbox component |
| `components/ui/select.tsx` | CREAR | shadcn Select component |
| `lib/constants.ts` | MODIFICAR | Agregar rutas de confirmación |
| `app/router.tsx` | MODIFICAR | Agregar ruta CampaniaDetailPage, BackingConfirmationPage |

## 9. Checklist

### Componentes
- [ ] CampaniaCard ya existe y funciona ✓
- [ ] CampaniaFilters creado con search + filtros
- [ ] BackingsRecentesList creado
- [ ] BackingRecenteCard creado con formatRelativeTime
- [ ] BackingModal creado (Dialog container)
- [ ] BackingForm creado con react-hook-form + Zod
- [ ] AmountInput creado con validación inline
- [ ] CampaniaDetailPage creado con grid layout
- [ ] BackingConfirmationPage creado con success animation
- [ ] CampaniasPage modificado con filtros

### Hooks
- [ ] useCampaniaDetail creado
- [ ] useCampaniaStats creado (opcional)
- [ ] useCreateBacking creado con invalidación y toast
- [ ] useBackingsByCampania creado

### Services
- [ ] campaniaService.getDetail agregado
- [ ] campaniaService.getStats agregado (opcional)
- [ ] backingService.create alineado con contracts
- [ ] backingService.getByCampaniaId con paginación

### Types y Schemas
- [ ] Types importados de @shared/types
- [ ] Schemas importados de @shared/schemas
- [ ] Validaciones runtime usadas (validateBackingAmount, etc.)

### UI Components (shadcn)
- [ ] Dialog component instalado
- [ ] Checkbox component instalado
- [ ] Select component instalado

### Router
- [ ] Ruta `/campanias` ya existe ✓
- [ ] Ruta `/campanias/:id` agregada
- [ ] Ruta `/campanias/:id/confirmacion` agregada

### Constantes
- [ ] ROUTES.CAMPANIA_DETAIL agregado
- [ ] ROUTES.BACKING_CONFIRMATION agregado
- [ ] QUERY_KEYS para backings verificados

### Integraciones
- [ ] Toast system configurado (sonner o react-hot-toast)
- [ ] Auth context verificado (useAuth para obtener userId)
- [ ] API client con interceptors funcionando ✓

### Validaciones
- [ ] Validación Zod en BackingForm
- [ ] Validación runtime de monto vs reward.importeMinimo
- [ ] Validación de stock de reward disponible
- [ ] Validación de campaña activa (estadoCampaniaId === 2)

### Accesibilidad
- [ ] Form labels con htmlFor
- [ ] ARIA attributes en progress bars
- [ ] Focus trap en modal
- [ ] Keyboard navigation (Tab, Enter, Esc)
- [ ] Screen reader announcements (role="status", aria-live)

### Testing (Post-implementación)
- [ ] Unit tests para hooks (useCreateBacking)
- [ ] Unit tests para validaciones (schemas)
- [ ] Component tests para BackingForm
- [ ] E2E test flujo completo (explorar → detalle → backing → confirmación)

## 10. Orden de Implementación Recomendado

1. **Instalar shadcn/ui components faltantes:**
   ```bash
   npx shadcn-ui@latest add dialog
   npx shadcn-ui@latest add checkbox
   npx shadcn-ui@latest add select
   ```

2. **Actualizar constants y router:**
   - Agregar ROUTES.CAMPANIA_DETAIL, ROUTES.BACKING_CONFIRMATION en `lib/constants.ts`
   - Agregar rutas en `app/router.tsx`

3. **Actualizar services:**
   - Modificar `campaniaService` con getDetail
   - Modificar `backingService` con create alineado

4. **Crear hooks de queries:**
   - `useCampaniaDetail`
   - `useBackingsByCampania` (opcional si data viene en detail)

5. **Crear hooks de mutations:**
   - `useCreateBacking`

6. **Crear componentes atómicos:**
   - `AmountInput`
   - `BackingRecenteCard`
   - `CampaniaFilters`

7. **Crear componentes moleculares:**
   - `BackingsRecentesList`
   - `BackingForm`

8. **Crear componentes de página:**
   - `CampaniaDetailPage` (puede reutilizar componentes existentes de campanias)
   - `BackingModal`
   - `BackingConfirmationPage`

9. **Modificar CampaniasPage:**
   - Agregar `CampaniaFilters`

10. **Testing:**
    - Unit tests para hooks
    - Component tests para formularios
    - E2E test del flujo

## 11. Dependencias NPM Adicionales

**Ya instaladas (verificar):**
- `@tanstack/react-query` ✓
- `react-hook-form` ✓
- `zod` ✓
- `@hookform/resolvers` (para zodResolver) ✓
- `lucide-react` (iconos) ✓

**Posiblemente faltantes:**
- `sonner` o `react-hot-toast` (sistema de toasts)
- `date-fns` (para formatRelativeTime en BackingRecenteCard)

**Instalar si faltan:**
```bash
npm install sonner date-fns
```

## 12. Utilidades a Crear

### 12.1 formatRelativeTime

**Archivo:** `features/campanias/application/utils.ts` (agregar a archivo existente)

**Implementación:**
```typescript
import { formatDistanceToNow } from "date-fns"
import { es } from "date-fns/locale"

export function formatRelativeTime(dateString: string): string {
  try {
    const date = new Date(dateString)
    return formatDistanceToNow(date, { addSuffix: true, locale: es })
  } catch (error) {
    return "Fecha inválida"
  }
}
```

### 12.2 formatDate

**Archivo:** `features/campanias/application/utils.ts` (agregar)

**Implementación:**
```typescript
import { format } from "date-fns"
import { es } from "date-fns/locale"

export function formatDate(dateString: string): string {
  try {
    const date = new Date(dateString)
    return format(date, "d MMM yyyy, HH:mm", { locale: es })
  } catch (error) {
    return "Fecha inválida"
  }
}
```

## 13. Notas de Implementación

### 13.1 Autenticación Opcional

El flujo de backing debe funcionar para usuarios autenticados y anónimos (si la campaña lo permite):

```typescript
// En BackingModal o BackingForm
const { user } = useAuth()

const handleSubmit = (data: CreateBackingFormData) => {
  if (!user && !campania.permiteAportacionesAnonimas) {
    // Redirect a login con returnUrl
    navigate(`/auth/login?returnUrl=/campanias/${campaniaId}`)
    return
  }

  // Continuar con backing
  createBacking.mutate({
    campaniaId,
    ...data,
  })
}
```

### 13.2 Validación de Monto en Tiempo Real

Usar validación runtime además de Zod:

```typescript
// En BackingForm
const selectedReward = props.reward
const montoActual = form.watch("monto")

const montoError = useMemo(() => {
  if (!montoActual) return null
  return validateBackingAmount(montoActual, selectedReward)
}, [montoActual, selectedReward])

// Mostrar error adicional si hay
{montoError && (
  <p className="text-xs text-red-500 mt-1">{montoError}</p>
)}
```

### 13.3 Invalidación de Queries

Después de crear backing exitoso, invalidar:
1. Query de campaña detail (`[QUERY_KEYS.CAMPANIA, id, "detail"]`)
2. Query de backings de campaña (`[QUERY_KEYS.BACKINGS, "campania", id]`)
3. Query de stats si existe (`[QUERY_KEYS.CAMPANIA, id, "stats"]`)

```typescript
onSuccess: (result, variables) => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.CAMPANIA, variables.campaniaId],
  })
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.BACKINGS, "campania", variables.campaniaId],
  })
}
```

### 13.4 Manejo de Errores del Backend

El backend retorna `ServiceResponse` con `messages[]`. Extraer error code y usar helper de shared:

```typescript
onError: (error: any) => {
  const errorCode =
    error.response?.data?.messages?.[0]?.errorCode || "5000"

  const message = getBackingErrorMessage(errorCode)

  toast.error("Error al procesar apoyo", {
    description: message,
  })
}
```

### 13.5 Progress Bar Compartido

Reutilizar `CampaniaProgress.tsx` existente:

```typescript
import { CampaniaProgress } from "@/features/campanias/presentation/components"

<CampaniaProgress
  importeObjetivo={data.importeObjetivo}
  importePledgedActual={data.importePledgedActual}
  porcentajeProgreso={data.porcentajeProgreso}
/>
```

### 13.6 RewardPublicCard Ya Existe

Reutilizar `RewardPublicCard.tsx` existente en sidebar de `CampaniaDetailPage`:

```typescript
import { RewardPublicCard } from "@/features/campanias/presentation/components"

{data.rewards.map(reward => (
  <RewardPublicCard
    key={reward.id}
    reward={reward}
    monedaId={data.monedaId}
    onSelect={(rewardId) => handleSelectReward(rewardId)}
  />
))}
```

### 13.7 Toast System

Configurar toast provider en `App.tsx` si no existe:

```typescript
import { Toaster } from "sonner"

function App() {
  return (
    <>
      <AppRouter />
      <Toaster position="top-right" />
    </>
  )
}
```

### 13.8 Lazy Loading de Páginas

Usar lazy loading para CampaniaDetailPage y BackingConfirmationPage:

```typescript
// En router.tsx
const CampaniaDetailPage = lazy(() =>
  import("@/features/campanias/presentation/pages/CampaniaDetailPage")
)
const BackingConfirmationPage = lazy(() =>
  import("@/features/backings/presentation/pages/BackingConfirmationPage")
)
```

## 14. Siguiente Paso Sugerido

Después de implementar este plan frontend, el siguiente paso recomendado es:

1. **Implementar el plan de backend** (`plans/hacer-backing/backend/backend-plan.md`) para crear:
   - `CreateBackingCommand` con validaciones y transacción atómica
   - `GetCampaniaDetailQuery` con rewards y backings recientes
   - Endpoints en `CampaniasController`

2. **Testing E2E** del flujo completo:
   - Usuario explora campañas → selecciona campaña → ve rewards → hace backing → ve confirmación
   - Verificar que los datos persisten en DB
   - Verificar invalidación de cache

3. **Implementación de Shared** si no existe:
   - Ejecutar plan `plans/hacer-backing/shared/contracts-plan.md`
   - Asegurar que tipos, schemas, constantes están alineados

---

**Fin del plan frontend para hacer-backing (Landing).**
