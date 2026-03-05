# Diseno UI: cp-tracking-metricas (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/web (Vite + React 18, puerto 3000)
**Ruta de pantalla:** `/promotor/metricas`

---

## 1. Resumen

- Componentes shadcn utilizados: Card, CardHeader, CardContent, Badge, Button, Input, Select, SelectTrigger, SelectContent, SelectItem, SelectValue, Skeleton, Alert, AlertTitle, AlertDescription
- Composiciones custom: 7 (PromotorMetricasPage, ProgramaSelector, KpiCard, EnlaceReferido, EventosRecientesList, EventoBadge, skeletons por seccion)
- Responsive breakpoints: mobile (< 640px), tablet (sm, >= 640px), desktop (lg, > 1024px)
- Contenido centrado: `max-w-4xl mx-auto px-4`
- Tema: oscuro (dark theme). Todos los fondos y colores son literales Tailwind o hex del design system.

---

## 2. Paleta de Colores (Design Tokens de la Feature)

| Uso | Valor CSS / Clase Tailwind | Descripcion |
|-----|---------------------------|-------------|
| Fondo de pagina | `bg-[#1a1a2e]` | Fondo secundario de paginas |
| Fondo de cards | `bg-[#151525]` | Cards KPI, enlace, eventos |
| Fondo card hover | `bg-[#1e1e38]` | Hover sobre filas y cards |
| Fondo inputs | `bg-[#0f0f1f]` | Input URL readonly |
| Borde default | `border-[#334155]` | Todos los bordes de cards e inputs |
| Borde focus | `focus:border-[#a855f7]` | Focus en Select e Input |
| Texto primario | `text-white` | Valores de KPI, titulos |
| Texto secundario | `text-[#94a3b8]` | Labels de KPI, subtitulos |
| Texto muted | `text-[#64748b]` | Fechas, notas, sufijos |
| Color primary (link, accent) | `text-[#a855f7]` | Boton "Ver todos", links |
| Color KPI clicks | `text-[#3b82f6]` | Valor e icono de clicks |
| Color KPI conversiones | `text-[#a855f7]` | Valor e icono de conversiones |
| Color KPI comision | `text-[#10b981]` | Valor e icono de comision |
| Color valor monetario | `text-[#f59e0b]` | Comision en EUR |
| Focus ring | `focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]` | Todos los elementos interactivos |

### Colores por tipo de evento (badges)

| Tipo | Fondo clase | Texto clase | Borde clase | Label UI |
|------|------------|-------------|-------------|----------|
| Click (1) | `bg-blue-950/50` | `text-[#3b82f6]` | `border border-blue-800/50` | "Click" |
| PageView (2) | `bg-slate-900/50` | `text-[#94a3b8]` | `border border-slate-700/50` | "Vista" |
| Signup (3) | `bg-green-950/50` | `text-[#10b981]` | `border border-green-800/50` | "Registro" |
| Backing (4) | `bg-purple-950/50` | `text-[#a855f7]` | `border border-purple-800/50` | "Backing" |
| Share (5) | `bg-amber-950/50` | `text-[#f59e0b]` | `border border-amber-800/50` | "Share" |

---

## 3. Layout General de la Pantalla

```
┌─────────────────────────────────────────────────────────────────────┐
│ HEADER (sticky, h-16, bg-[#0d0d1a], border-b border-[#334155])      │
│ [Logo WePlay Rises]   Explorar  Campanias  [CrowdPromotion]  [User] │
├─────────────────────────────────────────────────────────────────────┤
│ bg-[#1a1a2e] min-h-screen                                           │
│                                                                     │
│  max-w-4xl mx-auto px-4 pt-8                                        │
│                                                                     │
│  CABECERA DE PAGINA                                                 │
│  Mis metricas de promocion                                          │
│  Programa: [Select dropdown v]                                      │
│                                                                     │
│  KPI CARDS (grid-cols-1 sm:grid-cols-3 gap-4)                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐              │
│  │ Clicks       │  │ Conversiones │  │ Comision     │              │
│  │    450       │  │      5       │  │  50 EUR      │              │
│  └──────────────┘  └──────────────┘  └──────────────┘              │
│                                                                     │
│  Tasa de conversion: 1.11%   (conversiones / clicks)               │
│                                                                     │
│  ENLACE REFERIDO                                                    │
│  ┌─────────────────────────────────────────────┐ [Copiar]          │
│  │ https://weplay.com/campanias/xxx?ref=abc    │                   │
│  └─────────────────────────────────────────────┘                   │
│                                                                     │
│  EVENTOS RECIENTES                           [Ver todos ->]         │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ [Backing] 100 EUR -> 10 EUR comision            20 mar 14:30 │  │
│  ├──────────────────────────────────────────────────────────────┤  │
│  │ [Click]   Click desde instagram.com            20 mar 12:15 │  │
│  ├──────────────────────────────────────────────────────────────┤  │
│  │ [Vista]   Visita a pagina de campana           19 mar 18:42 │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

**Clases del contenedor raiz de pagina:**
```
bg-[#1a1a2e] min-h-screen
```

**Clases del contenedor de contenido:**
```
max-w-4xl mx-auto px-4 pt-8 pb-10
```

---

## 4. Componentes Detallados

### 4.1 PromotorMetricasPage

**Descripcion:** Pagina principal. Orquesta todos los componentes. Gestiona el estado de programa seleccionado, el refetch al cambiar programa y los estados globales de la pagina (loading, error, empty).

**Props:**
```
ninguna (es una pagina, accede a query params via useSearchParams / useLocation)
```

**Estructura de composicion:**
```
<PublicLayout>                             {/* Header existente de la landing */}
  <div bg-[#1a1a2e] min-h-screen>
    <div max-w-4xl mx-auto px-4 pt-8 pb-10>

      {/* Cabecera */}
      <div flex flex-col sm:flex-row sm:items-center gap-3 mb-8>
        <h1 text-2xl font-bold text-white>Mis metricas de promocion</h1>
        <ProgramaSelector
          programas={programas}
          selectedId={selectedProgramaId}
          onChange={handleProgramaChange}
        />
      </div>

      {/* Estado: sin programas */}
      {!hasProgramas && <EmptyStateSinProgramas />}

      {/* Estado: con programa seleccionado */}
      {hasProgramas && (
        <>
          {isLoading ? <KpiCardsSkeleton /> : (
            <>
              <KpiCardsGrid kpis={metricas.kpis} />
              <TasaConversionInline tasa={metricas.kpis.miTasaConversion} />
            </>
          )}

          {isLoading ? <EnlaceReferidoSkeleton /> : (
            <EnlaceReferido url={enlaceReferido} />
          )}

          {isLoading ? <EventosRecientesSkeleton /> : (
            <EventosRecientesList
              eventos={metricas.eventosRecientes}
              programaId={selectedProgramaId}
            />
          )}

          {isError && <ErrorState onRetry={refetch} />}
        </>
      )}

    </div>
  </div>
</PublicLayout>
```

**Estados manejados:**
| Estado | Condicion | Renderizado |
|--------|-----------|-------------|
| Loading inicial | `isProgramasLoading` o `isMetricasLoading` | Skeletons por seccion |
| Sin programas | `programas.length === 0` | `EmptyStateSinProgramas` |
| Con datos | datos cargados correctamente | UI completa |
| Error | `isError === true` | `ErrorState` con boton reintentar |
| Cambio de programa | `isFetching && !isLoading` | Skeletons reemplazando KPI y eventos |

**Accesibilidad:**
- Contenedor de metricas con `aria-busy="true"` mientras `isLoading`
- `role="main"` en el contenedor principal del contenido
- Al cambiar programa: `aria-live="polite"` para anunciar nueva carga

---

### 4.2 ProgramaSelector

**Descripcion:** Dropdown para seleccionar el programa de promocion activo. Pobla sus opciones desde el endpoint `GET /api/crowdpromotion/promotor/mis-programas`.

**Props:**
```typescript
interface ProgramaSelectorProps {
    programas: Array<{ programaId: string; nombrePrograma: string }>;
    selectedId: string | undefined;
    onChange: (programaId: string) => void;
    isLoading?: boolean;
}
```

**Componentes shadcn:**
- `Select`, `SelectTrigger`, `SelectValue`, `SelectContent`, `SelectItem`

**Composicion:**
```
<div flex items-center gap-3>
  <label
    htmlFor="programa-select"
    className="text-sm text-[#94a3b8] shrink-0"
  >
    Programa:
  </label>
  <Select
    value={selectedId}
    onValueChange={onChange}
    aria-label="Selecciona un programa de promocion"
  >
    <SelectTrigger
      id="programa-select"
      className="w-64 bg-[#0f0f1f] border-[#334155] text-white text-sm
                 focus:border-[#a855f7] focus:ring-[#a855f7]"
    >
      <SelectValue placeholder="Selecciona un programa" />
    </SelectTrigger>
    <SelectContent>
      {programas.map(p => (
        <SelectItem key={p.programaId} value={p.programaId} className="text-sm text-white">
          {p.nombrePrograma}
        </SelectItem>
      ))}
    </SelectContent>
  </Select>
</div>
```

**Customizaciones sobre el Select existente:**
- Ancho fijo: `w-64`
- Fondo del trigger: `bg-[#0f0f1f]` (mas oscuro que el default `#1a1a2e`)
- En mobile: `w-full` en lugar de `w-64`

**Estados:**
| Estado | Visual |
|--------|--------|
| Default | Border `#334155`, texto `text-white` |
| Focus | Border `#a855f7`, ring `ring-[#a855f7]` |
| Loading programas | Skeleton rectangular `w-64 h-10 animate-pulse bg-[#1e1e38] rounded-md` |
| Sin programas | No se muestra el selector |

**Responsive:**
- Mobile (< sm): `w-full` en el SelectTrigger
- Tablet/Desktop: `w-64`

**Accesibilidad:**
- `<label htmlFor="programa-select">` vinculado al trigger
- `aria-label="Selecciona un programa de promocion"` en el `<Select>`

---

### 4.3 KpiCard

**Descripcion:** Card reusable para un KPI individual. Muestra icono + valor + label. Tiene variantes de color por tipo de metrica.

**Props:**
```typescript
type KpiCardVariant = 'clicks' | 'conversiones' | 'comision';

interface KpiCardProps {
    variant: KpiCardVariant;
    value: number;
    label: string;
    moneda?: string | null;     // Solo para variant="comision"
    className?: string;
}
```

**Componentes shadcn:**
- `Card`

**Composicion:**
```
<Card
  className="bg-[#151525] border-[#334155] p-5
             transition-colors duration-150 hover:border-[#a855f7]/30"
>
  {/* Icono */}
  <div className={cn(
    "w-10 h-10 rounded-lg flex items-center justify-center mb-3",
    iconContainerClass   // variable por variante
  )}>
    {/* Icono especifico por variante (Lucide) */}
    <IconComponent className={cn("w-5 h-5", iconColorClass)} />
  </div>

  {/* Valor */}
  {variant === 'comision' ? (
    <p className="text-3xl font-bold text-[#f59e0b] tabular-nums">
      {value}
      <span className="text-lg text-[#64748b] ml-1">{moneda ?? 'EUR'}</span>
    </p>
  ) : (
    <p className={cn("text-3xl font-bold tabular-nums", valueColorClass)}>
      {value.toLocaleString()}
    </p>
  )}

  {/* Label */}
  <p className="text-sm text-[#94a3b8] mt-1">{label}</p>
</Card>
```

**Variantes de color por tipo:**

| Variant | Icono Lucide | Container class | Icon color class | Value color class |
|---------|-------------|-----------------|------------------|-------------------|
| `clicks` | `MousePointerClick` | `bg-blue-950/50` | `text-[#3b82f6]` | `text-white` |
| `conversiones` | `ShoppingCart` | `bg-purple-950/50` | `text-[#a855f7]` | `text-white` |
| `comision` | `Wallet` | `bg-green-950/50` | `text-[#10b981]` | `text-[#f59e0b]` (valor monetario) |

**Grid contenedor (en PromotorMetricasPage):**
```
<div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6" aria-busy={isLoading}>
  <KpiCard variant="clicks" value={kpis.misClicks} label="Mis clicks" />
  <KpiCard variant="conversiones" value={kpis.misConversiones} label="Conversiones" />
  <KpiCard variant="comision" value={kpis.miComisionAcumulada} label="Comision" moneda={kpis.monedaNombre} />
</div>
```

**Animacion de entrada:** `transition-opacity duration-300 animate-in fade-in` al reemplazar skeleton.

**Responsive:**
- Mobile: 1 columna (`grid-cols-1`), cards apiladas
- Tablet/Desktop (>= sm): 3 columnas (`grid-cols-3`)

---

### 4.4 TasaConversionInline

**Descripcion:** Elemento informativo inline debajo de las KPI cards. No es una card independiente, es un bloque de texto con icono.

**Props:**
```typescript
interface TasaConversionInlineProps {
    tasa: number;  // valor numerico, ej: 1.11 -> se muestra "1.11%"
}
```

**Composicion (sin componentes shadcn, solo elementos HTML + Lucide):**
```
<div className="flex items-center gap-2 mb-6 text-sm">
  <TrendingUp className="w-4 h-4 text-[#94a3b8] shrink-0" />
  <span className="text-[#94a3b8]">Tasa de conversion:</span>
  <span className="font-semibold text-white">{formatTasaConversion(tasa)}</span>
  <span className="text-xs text-[#64748b]">(conversiones / clicks)</span>
</div>
```

**Nota:** Usa la funcion `formatTasaConversion` de `src/shared/utils/format.ts`.

---

### 4.5 EnlaceReferido

**Descripcion:** Seccion que muestra la URL de promocion del promotor con boton para copiar al portapapeles. El feedback de "copiado" ocurre en el propio boton, sin toast (salvo fallo de Clipboard API).

**Props:**
```typescript
interface EnlaceReferidoProps {
    url: string;  // URL completa con ?ref=codigoReferido
}
```

**Componentes shadcn:**
- `Card`, `Input`, `Button`

**Estado interno del componente:**
```typescript
type CopyState = 'idle' | 'copying' | 'copied' | 'error';
// 'idle'    -> boton "Copiar" normal
// 'copying' -> boton deshabilitado 300ms
// 'copied'  -> boton "Copiado!" verde 2000ms
// 'error'   -> toast de error (via sonner), boton vuelve a idle
```

**Composicion:**
```
<Card className="bg-[#151525] border-[#334155] p-5 mb-6">
  <p className="text-sm font-medium text-[#94a3b8] mb-3">
    Mi enlace de promocion
  </p>
  <div className="flex items-center gap-2">
    <Input
      readOnly
      value={url}
      aria-label="Tu enlace de promocion"
      aria-readonly="true"
      className="bg-[#0f0f1f] border-[#334155] text-[#94a3b8] text-sm
                 flex-1 font-mono cursor-default focus-visible:ring-[#a855f7]
                 focus-visible:ring-offset-[#1a1a2e] truncate sm:truncate-none"
    />

    {/* Estado: idle / copying */}
    {copyState !== 'copied' && (
      <Button
        variant="outline"
        size="sm"
        disabled={copyState === 'copying'}
        onClick={handleCopy}
        aria-label="Copiar enlace de promocion"
        className="shrink-0 border-[#334155] text-[#94a3b8]
                   hover:border-[#a855f7] hover:text-[#a855f7]
                   focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]
                   disabled:opacity-50"
      >
        <Copy className="w-4 h-4 mr-1.5" />
        Copiar
      </Button>
    )}

    {/* Estado: copied */}
    {copyState === 'copied' && (
      <Button
        variant="outline"
        size="sm"
        disabled
        aria-label="Enlace copiado"
        className="shrink-0 border-green-800/50 text-[#10b981]"
      >
        <Check className="w-4 h-4 mr-1.5" />
        Copiado!
      </Button>
    )}
  </div>
</Card>
```

**Logica de copia (solo descripcion, no implementacion):**
1. Click en "Copiar" -> `copyState = 'copying'` (boton disabled)
2. Llamar `navigator.clipboard.writeText(url)`
3. En exito (300ms despues del click): `copyState = 'copied'`
4. Despues de 2000ms: `copyState = 'idle'`
5. En error de Clipboard API: mostrar toast de error (sonner), `copyState = 'idle'`

**Estados:**
| Estado | Visual del boton |
|--------|-----------------|
| `idle` | `border-[#334155] text-[#94a3b8]` con icono `<Copy>` |
| `copying` | Mismo estilo + `disabled opacity-50` |
| `copied` | `border-green-800/50 text-[#10b981]` con icono `<Check>` |
| `error` | Toast sonner destructivo; boton vuelve a idle |

**Accesibilidad:**
- `aria-label` dinamico en el boton: "Copiar enlace de promocion" / "Enlace copiado"
- `aria-readonly="true"` en el Input
- `aria-label="Tu enlace de promocion"` en el Input

**Responsive:**
- Mobile: Input con `truncate` para no desbordar
- Tablet/Desktop: Input visible sin truncado (`sm:truncate-none`)

---

### 4.6 EventosRecientesList

**Descripcion:** Card contenedor con la lista de los ultimos eventos del promotor. Incluye cabecera con boton "Ver todos" y la lista de filas de eventos. Muestra empty state si no hay eventos.

**Props:**
```typescript
interface EventosRecientesListProps {
    eventos: EventoReciente[];
    programaId: string | undefined;
}
```

**Componentes shadcn:**
- `Card`, `Button`, `Badge` (via EventoBadge)

**Composicion:**
```
<Card className="bg-[#151525] border-[#334155]">

  {/* Cabecera de la seccion */}
  <div className="flex items-center justify-between p-5 border-b border-[#334155]">
    <h3 className="text-base font-semibold text-white">Eventos recientes</h3>
    <Button
      variant="ghost"
      size="sm"
      asChild
      className="text-[#a855f7] hover:text-[#c084fc] text-xs
                 focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
    >
      <Link to={`/promotor/eventos?programaId=${programaId}`}>
        Ver todos
        <ChevronRight className="w-3.5 h-3.5 ml-0.5" />
      </Link>
    </Button>
  </div>

  {/* Lista de eventos o empty state */}
  {eventos.length === 0 ? (
    <EmptyStateEventos />
  ) : (
    <div className="divide-y divide-[#1e1e38]">
      {eventos.map(evento => (
        <EventoRow key={evento.id} evento={evento} />
      ))}
    </div>
  )}

</Card>
```

**EventoRow (sub-componente interno):**
```
<div
  className="flex items-center gap-3 px-5 py-3.5
             hover:bg-[#1e1e38] transition-colors duration-150"
>
  <EventoBadge tipoEventoId={evento.tipoEventoId} />

  <div className="flex-1 min-w-0">
    <p className="text-sm text-white truncate">
      {evento.tipoEventoNombre}
    </p>
    {/* Detalle adicional solo para tipo Backing (4) */}
    {evento.tipoEventoId === 4 && evento.valorMonetario > 0 && (
      <p className="text-xs text-[#94a3b8]">
        {evento.valorMonetario} {evento.monedaNombre ?? 'EUR'}
        {evento.comisionGenerada != null && (
          <> - {evento.comisionGenerada} {evento.monedaNombre ?? 'EUR'} comision</>
        )}
      </p>
    )}
  </div>

  <span className="text-xs text-[#64748b] shrink-0">
    {formatFechaEvento(evento.fechaEvento)}
  </span>
</div>
```

**Formato de fecha:** `fechaEvento` es ISO 8601 UTC. Se formatea como "20 mar 2026, 14:30" usando `Intl.DateTimeFormat` o `date-fns`.

**Accesibilidad:**
- El boton "Ver todos" tiene texto descriptivo visible
- Filas de evento sin elemento interactivo individual (solo hover visual)
- `role="list"` en el contenedor de filas, `role="listitem"` en cada fila (opcional si se usa `<ul>/<li>`)

---

### 4.7 EventoBadge

**Descripcion:** Badge colorado que representa el tipo de evento. Usa las clases Tailwind del design system. Internamente usa la funcion `mapTipoEventoPromoToBadgeClass` de shared/utils/mappers.ts.

**Props:**
```typescript
interface EventoBadgeProps {
    tipoEventoId: TipoEventoPromo;  // 1 | 2 | 3 | 4 | 5
    className?: string;
}
```

**Componentes shadcn:**
- `Badge` (con `className` override total, sin usar variantes predefinidas)

**Composicion:**
```
<Badge
  className={cn(
    mapTipoEventoPromoToBadgeClass(tipoEventoId),
    "shrink-0",
    className
  )}
>
  {TIPO_EVENTO_PROMO_LABELS[tipoEventoId] ?? 'Evento'}
</Badge>
```

**Clases por variante (desde mapTipoEventoPromoToBadgeClass):**

| tipoEventoId | Label | Clases Tailwind completas |
|-------------|-------|---------------------------|
| 1 (Click) | "Click" | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs` |
| 2 (PageView) | "Vista" | `bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs` |
| 3 (Signup) | "Registro" | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs` |
| 4 (Backing) | "Backing" | `bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs` |
| 5 (Share) | "Share" | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs` |
| desconocido | "Evento" | `bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs` |

**Nota sobre Badge shadcn:** El componente `Badge` en este proyecto acepta `className` libre. Se ignoran las variantes predefinidas (`default`, `secondary`, etc.) y se aplican las clases del design system directamente, lo que es el patron correcto para este caso de uso de multiples colores semanticos.

---

## 5. Estados de UI

### 5.1 Loading State (Skeletons)

**Componentes shadcn:** `Skeleton`

**KpiCardsSkeleton:**
```
<div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6" aria-busy="true">
  {[0, 1, 2].map(i => (
    <div key={i} className="bg-[#151525] border border-[#334155] rounded-lg p-5">
      <Skeleton className="w-10 h-10 rounded-lg mb-3 bg-[#1e1e38]" />
      <Skeleton className="w-20 h-8 mb-2 bg-[#1e1e38]" />
      <Skeleton className="w-24 h-4 bg-[#1e1e38]" />
    </div>
  ))}
</div>
```

**EnlaceReferidoSkeleton:**
```
<div className="bg-[#151525] border border-[#334155] rounded-lg p-5 mb-6">
  <Skeleton className="w-32 h-4 mb-3 bg-[#1e1e38]" />
  <div className="flex gap-2">
    <Skeleton className="flex-1 h-10 bg-[#1e1e38] rounded-md" />
    <Skeleton className="w-20 h-10 bg-[#1e1e38] rounded-md" />
  </div>
</div>
```

**EventosRecientesSkeleton:**
```
<div className="bg-[#151525] border border-[#334155] rounded-lg">
  <div className="flex items-center justify-between p-5 border-b border-[#334155]">
    <Skeleton className="w-36 h-5 bg-[#1e1e38]" />
    <Skeleton className="w-20 h-6 bg-[#1e1e38] rounded-md" />
  </div>
  <div className="divide-y divide-[#1e1e38]">
    {[0, 1, 2].map(i => (
      <div key={i} className="flex items-center gap-3 px-5 py-3.5">
        <Skeleton className="w-14 h-5 rounded-full bg-[#1e1e38]" />
        <Skeleton className="flex-1 h-4 bg-[#1e1e38]" />
        <Skeleton className="w-24 h-4 bg-[#1e1e38] shrink-0" />
      </div>
    ))}
  </div>
</div>
```

**ProgramaSelectorSkeleton (mientras carga lista de programas):**
```
<Skeleton className="w-64 h-10 rounded-md bg-[#1e1e38]" />
```

### 5.2 Empty State: Sin Programas

Mostrado cuando el promotor no tiene ningun programa activo.

**Componentes shadcn:** `Button`

```
<div className="flex flex-col items-center justify-center py-20 text-center">
  <div className="w-16 h-16 rounded-full flex items-center justify-center mb-4"
       style={{ background: 'linear-gradient(135deg, #ec4899 0%, #a855f7 100%)' }}>
    <BarChart2 className="w-8 h-8 text-white" />
  </div>
  <h2 className="text-xl font-semibold text-white mb-2">Sin programas activos</h2>
  <p className="text-sm text-[#94a3b8] mb-6 max-w-xs">
    Inscribete en un programa de promocion para ver tus metricas
  </p>
  <Button
    asChild
    className="bg-gradient-to-r from-pink-500 to-purple-600 text-white
               hover:from-pink-400 hover:to-purple-500
               focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
  >
    <Link to="/promotor/mis-programas">Ver programas disponibles</Link>
  </Button>
</div>
```

### 5.3 Empty State: Sin Eventos en el Programa

Mostrado dentro de `EventosRecientesList` cuando `eventos.length === 0`.

```
<div className="flex flex-col items-center justify-center py-10 text-center px-5">
  <Activity className="w-8 h-8 text-[#64748b] mb-3" />
  <p className="text-sm text-[#94a3b8]">
    Aun no hay eventos registrados.
  </p>
  <p className="text-sm text-[#64748b]">
    Comparte tu enlace de promocion para empezar.
  </p>
</div>
```

### 5.4 Error State

Mostrado cuando falla el fetch de metricas.

**Componentes shadcn:** `Button`

```
<div
  className="flex flex-col items-center justify-center py-16 text-center"
  role="alert"
>
  <AlertCircle className="w-10 h-10 text-red-400 mb-4" />
  <h2 className="text-lg font-semibold text-white mb-2">
    No se pudieron cargar las metricas
  </h2>
  <p className="text-sm text-[#94a3b8] mb-6">
    Ocurrio un error al obtener tus datos. Intenta de nuevo.
  </p>
  <Button
    variant="outline"
    size="sm"
    onClick={onRetry}
    className="border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-[#a855f7]
               focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
  >
    Reintentar
  </Button>
</div>
```

**Accesibilidad:** `role="alert"` para anuncio automatico a lectores de pantalla.

---

## 6. Toast Messages

Usa `sonner` (ya configurado en el proyecto).

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Copia de enlace exitosa (via Clipboard API) | (sin toast - feedback en boton) | N/A |
| Fallo de Clipboard API (navegador sin soporte) | `toast.error(...)` | "No se pudo copiar. Copia el enlace manualmente." |

**Nota:** El feedback principal de copia exitosa es el cambio de estado del boton a "Copiado!" durante 2000ms. El toast solo aparece en el caso de error como fallback accesible.

---

## 7. Componente Invisible: useTrackingInterceptor

**Tipo:** Hook React sin renderizado visual
**Ubicacion planificada:** `src/web/src/features/tracking/hooks/useTrackingInterceptor.ts`
**Montaje:** En `App.tsx` con `useTrackingInterceptor()` (una sola llamada)

No tiene interfaz UI. Ejecuta logica en background:
- Lee `?ref=`, `?utm_source=`, `?utm_medium=`, `?utm_campaign=` de la URL
- Persiste en `sessionStorage` y cookie `wp_ref` (30 min)
- Llama `POST /api/crowdpromotion/tracking/evento` con `tipoEventoPromoId = 1` (Click)
- Ignora errores silenciosamente (sin UI de error)
- No bloquea el render de la aplicacion

No requiere diseno de UI. Se documenta aqui para completitud del plan de la feature.

---

## 8. Estructura de Archivos del Componente

```
src/web/src/features/tracking/
  hooks/
    useTrackingInterceptor.ts        Hook invisible (sin UI)
    usePromotorMetricas.ts           useQuery para GET /promotor/metricas
    useMisProgramas.ts               useQuery para GET /promotor/mis-programas
  presentation/
    pages/
      PromotorMetricasPage.tsx       Pagina principal (componente 4.1)
    components/
      ProgramaSelector.tsx           Selector de programa (componente 4.2)
      KpiCard.tsx                    Card de KPI (componente 4.3)
      TasaConversionInline.tsx       Texto de tasa (componente 4.4)
      EnlaceReferido.tsx             Seccion de enlace (componente 4.5)
      EventosRecientesList.tsx       Lista de eventos (componente 4.6)
      EventoBadge.tsx                Badge por tipo (componente 4.7)
      skeletons/
        KpiCardsSkeleton.tsx         Skeleton de KPI cards
        EnlaceReferidoSkeleton.tsx   Skeleton de enlace
        EventosRecientesSkeleton.tsx Skeleton de eventos
      empty-states/
        EmptyStateSinProgramas.tsx   Empty state sin programas
        EmptyStateEventos.tsx        Empty state sin eventos
      ErrorState.tsx                 Estado de error con reintentar
```

---

## 9. Responsive Design

| Breakpoint | Ancho | Cambios de layout |
|------------|-------|-------------------|
| Mobile | < 640px (< sm) | KPI cards en 1 columna (`grid-cols-1`). Selector de programa `w-full`. Cabecera apilada verticalmente (`flex-col`). Input de enlace con `truncate`. Titulo `text-xl` |
| Tablet | >= 640px (sm) | KPI cards en 3 columnas (`grid-cols-3`). Cabecera en fila (`flex-row`). Selector `w-64`. Input sin truncar |
| Desktop | > 1024px (lg) | Igual que tablet. Contenido centrado con `max-w-4xl mx-auto` |

**Clases clave responsive:**
```
Cabecera: "flex flex-col sm:flex-row sm:items-center gap-3 mb-8"
KPI grid: "grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6"
Selector: "w-full sm:w-64"
Titulo:   "text-xl sm:text-2xl font-bold text-white"
```

---

## 10. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Labels en Select | `<label htmlFor="programa-select">` vinculado a `<SelectTrigger id="programa-select">` |
| aria-label en Select | `aria-label="Selecciona un programa de promocion"` |
| Input readonly URL | `aria-label="Tu enlace de promocion"`, `aria-readonly="true"` |
| Boton copiar dinamico | `aria-label` cambia: "Copiar enlace de promocion" -> "Enlace copiado" |
| Focus ring | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en todos los elementos interactivos |
| Contraste texto primario | Blanco `#ffffff` sobre `#151525`: ratio > 7:1 (AAA) |
| Contraste texto secundario | `#94a3b8` sobre `#151525`: ratio ~4.6:1 (AA) |
| Contraste badges | Colores de evento cumplen minimo 3:1 sobre fondos oscuros semi-transparentes |
| Estado de carga | `aria-busy="true"` en contenedor de metricas durante fetch |
| Estado de error | `role="alert"` en contenedor del error state |
| Lista de eventos | `role="list"` + `role="listitem"` en estructura de filas |
| Skeletons | Contenedores skeleton con `aria-busy="true"` y `aria-label="Cargando datos"` |

---

## 11. Animaciones

| Elemento | Animacion | Duracion | Clase Tailwind |
|----------|-----------|----------|----------------|
| Skeleton -> contenido | Fade in | 200ms | `animate-in fade-in duration-200` |
| KPI card al cargar | Fade + slide up | 300ms | `animate-in fade-in slide-in-from-bottom-2 duration-300` |
| Boton "Copiar" -> "Copiado!" | Cambio de texto/icono | 150ms | Cambio de estado React |
| Hover sobre card | Transicion de borde | 150ms | `transition-colors duration-150` |
| Hover sobre fila de evento | Transicion de fondo | 150ms | `hover:bg-[#1e1e38] transition-colors duration-150` |

---

## 12. Componentes shadcn/ui Utilizados

| Componente | Uso | Archivo fuente |
|------------|-----|----------------|
| `Card` | KpiCard, EnlaceReferido, EventosRecientesList | `components/ui/card.tsx` |
| `Badge` | EventoBadge (5 variantes de color custom) | `components/ui/badge.tsx` |
| `Button` | Copiar, Ver todos, Reintentar, Ver programas | `components/ui/button.tsx` |
| `Input` | URL readonly del enlace referido | `components/ui/input.tsx` |
| `Select` + `SelectTrigger` + `SelectContent` + `SelectItem` + `SelectValue` | ProgramaSelector | `components/ui/select.tsx` |
| `Skeleton` | 3 variantes de skeleton por seccion | `components/ui/skeleton.tsx` |

**Componentes shadcn NO utilizados en esta pantalla:** Table, Dialog, Sheet, Tabs, Form, Textarea, Checkbox, RadioGroup, Switch, Progress, Alert, Toast (se usa sonner directamente), Separator, Popover, Tooltip, Avatar, Pagination, ScrollArea.

---

## 13. Iconos Lucide Utilizados

| Icono | Uso | Tamano |
|-------|-----|--------|
| `MousePointerClick` | KPI card clicks | `w-5 h-5` |
| `ShoppingCart` | KPI card conversiones | `w-5 h-5` |
| `Wallet` | KPI card comision | `w-5 h-5` |
| `TrendingUp` | Tasa de conversion inline | `w-4 h-4` |
| `Copy` | Boton copiar (estado idle) | `w-4 h-4` |
| `Check` | Boton copiar (estado copiado) | `w-4 h-4` |
| `ChevronRight` | Boton "Ver todos" | `w-3.5 h-3.5` |
| `BarChart2` | Empty state sin programas | `w-8 h-8` |
| `Activity` | Empty state sin eventos | `w-8 h-8` |
| `AlertCircle` | Error state | `w-10 h-10` |

---

## 14. Checklist

- [ ] `PromotorMetricasPage` con layout `max-w-4xl mx-auto px-4`
- [ ] Selector de programa (`<Select>`) con label vinculado
- [ ] 3 `KpiCard` con variantes: clicks (azul), conversiones (purpura), comision (verde/ambar)
- [ ] `TasaConversionInline` con `formatTasaConversion`
- [ ] `EnlaceReferido` con boton copiar y estados idle/copied/error
- [ ] Feedback "Copiado!" en boton (sin toast) durante 2000ms
- [ ] `EventosRecientesList` con lista de `EventoRow` y boton "Ver todos"
- [ ] `EventoBadge` con 5 variantes de color (Click, Vista, Registro, Backing, Share)
- [ ] Detalle monetario solo en filas de tipo Backing (4)
- [ ] Fecha formateada "DD mmm YYYY, HH:MM" en filas de eventos
- [ ] `KpiCardsSkeleton` (3 cards) con `animate-pulse`
- [ ] `EnlaceReferidoSkeleton` con `animate-pulse`
- [ ] `EventosRecientesSkeleton` (3 filas) con `animate-pulse`
- [ ] `EmptyStateSinProgramas` con icono gradiente y boton a `/promotor/mis-programas`
- [ ] `EmptyStateEventos` dentro de la card de eventos
- [ ] `ErrorState` con `role="alert"` y boton "Reintentar"
- [ ] Responsive: mobile 1 columna KPI, tablet/desktop 3 columnas
- [ ] `aria-busy="true"` en contenedor de metricas durante carga
- [ ] Focus rings `ring-[#a855f7]` en todos los elementos interactivos
- [ ] Labels vinculados a todos los inputs y selects
- [ ] `useTrackingInterceptor` montado en `App.tsx` (sin UI)
- [ ] Toast sonner solo en fallo de Clipboard API
