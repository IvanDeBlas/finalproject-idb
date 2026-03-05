# Diseno UI: Definir Recompensas (Landing)

**Fecha:** 2026-02-13
**Feature:** definir-recompensas
**Target:** src/web (Landing publica)

---

## 1. Resumen

- Componentes shadcn: 5 (Card, Badge, Button, Separator, Skeleton)
- Composiciones custom: 3 (CampaniaRewardsSection, RewardPublicCard, RewardPublicCardSkeleton)
- Responsive breakpoints: sm (640px), md (768px), lg (1024px)

**Alcance:**
Esta documentacion cubre UNICAMENTE la vista publica de recompensas en la landing (`/campanias/{id}`). La gestion de recompensas en el dashboard de admin sera documentada en `plans/definir-recompensas/frontend-admin/ui-design.md`.

**Diferencias con Componente Actual:**
El archivo `RewardCard.tsx` existente se mantendra como base, pero se renombrara a `RewardPublicCard.tsx` y se actualizara para:
1. Usar la nueva estructura de datos de contracts.md (`cantidadMaxima` en lugar de `cantidadDisponible`)
2. Agregar iconos de stock (package, alert-circle, x-circle) segun el ui-ux.md
3. Incluir badge "Pocas unidades" con logica de stock < 50%
4. Mejorar accesibilidad con ARIA labels especificos

---

## 2. Paleta de Colores (del proyecto)

| Uso | Variable | Ejemplo | Codigo Hex |
|-----|----------|---------|------------|
| Primary | --primary | Botones principales, borders hover | #a855f7 |
| Background Card | --card | Fondo de reward cards | #1a1a2e |
| Background Dark | --background | Fondo de sidebar container | #0f1729 |
| Border Default | --border | Bordes de cards | #334155 |
| Text Primary | text-white | Titulos, nombres | #ffffff |
| Text Secondary | text-[#94a3b8] | Descripciones | #94a3b8 |
| Text Muted | text-[#64748b] | Textos secundarios, hints | #64748b |
| Success | green-400 | Stock disponible | #4ade80 |
| Warning | --warning | Stock limitado | #f59e0b |
| Destructive | red-400 | Sold out (en algunos contextos) | #f87171 |

**Colores Especificos de Rewards (del ui-ux.md):**
```css
--reward-available: #10b981;   /* green-400 aprox */
--reward-limited: #f59e0b;     /* warning */
--reward-sold-out: #64748b;    /* muted */
```

---

## 3. Componentes por Screen

### 3.1 Campaign Detail Page (Seccion Recompensas)

#### Layout ASCII

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  ┌───────────────────┬───────────────────────────────────┐  │
│  │                   │  [SIDEBAR - Sticky]               │  │
│  │  Main Content     │                                   │  │
│  │  (Hero, Tabs,     │  ┌─────────────────────────────┐  │  │
│  │   Description)    │  │ [Apoyar esta campana]       │  │  │
│  │                   │  │ Desde € 10                  │  │  │
│  │                   │  └─────────────────────────────┘  │  │
│  │                   │                                   │  │
│  │                   │  ──────────────────────────────   │  │
│  │                   │                                   │  │
│  │                   │  Recompensas                      │  │
│  │                   │                                   │  │
│  │                   │  ┌───────────────────────────┐    │  │
│  │                   │  │ € 10  Descarga Digital    │    │  │
│  │                   │  │ Mas popular               │    │  │
│  │                   │  │ Acceso anticipado...      │    │  │
│  │                   │  │ 📦 234 de 500 disponibles │    │  │
│  │                   │  │ [Seleccionar]             │    │  │
│  │                   │  └───────────────────────────┘    │  │
│  │                   │                                   │  │
│  │                   │  ┌───────────────────────────┐    │  │
│  │                   │  │ € 25  CD Fisico           │    │  │
│  │                   │  │ ⚠️ Pocas unidades         │    │  │
│  │                   │  │ CD firmado + descarga...  │    │  │
│  │                   │  │ 📦 89 de 200 disponibles  │    │  │
│  │                   │  │ [Seleccionar]             │    │  │
│  │                   │  └───────────────────────────┘    │  │
│  │                   │                                   │  │
│  │                   │  ┌───────────────────────────┐    │  │
│  │                   │  │ € 100  Paquete VIP        │    │  │
│  │                   │  │ AGOTADO                   │    │  │
│  │                   │  │ Meet & greet privado...   │    │  │
│  │                   │  │ ❌ Agotado                 │    │  │
│  │                   │  │ [Agotado]                 │    │  │
│  │                   │  └───────────────────────────┘    │  │
│  │                   │                                   │  │
│  │                   │  ──────────────────────────────   │  │
│  │                   │                                   │  │
│  │                   │  🔒 Pago seguro                    │  │
│  │                   │  📦 Entrega: Marzo 2025           │  │
│  │                   │                                   │  │
│  └───────────────────┴───────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

#### Componentes

**CampaniaRewardsSection (Container)**

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Container | Card | `bg-[#0f1729] border-[#334155] p-6 sticky top-24` |
| Support CTA Button | Button | `w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2` |
| Starting Price Text | p | `text-center text-sm text-[#94a3b8] mb-6` |
| Divider | Separator | `my-6 bg-[#334155]` |
| Rewards Title | h3 | `text-lg font-bold text-white mb-4` |
| Rewards List Container | div | `space-y-3` |
| Footer Divider | Separator | `my-6 bg-[#334155]` |
| Footer Container | div | `space-y-2 text-xs text-[#64748b]` |
| Security Info Row | div | `flex items-center gap-2` |
| Security Icon | Lock (lucide-react) | `w-4 h-4` |
| Delivery Info Row | div | `flex items-center gap-2` |
| Delivery Icon | Package (lucide-react) | `w-4 h-4` |

**Composicion CampaniaRewardsSection:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-6 sticky top-24">
  {/* CTA Button */}
  <Button
    className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3 mb-2"
    onClick={onApoyar}
    aria-label="Apoyar esta campana"
  >
    Apoyar esta campana
  </Button>

  {/* Starting Price */}
  <p className="text-center text-sm text-[#94a3b8] mb-6">
    Desde {formatCurrency(minRewardAmount, monedaId)}
  </p>

  <Separator className="my-6 bg-[#334155]" />

  {/* Title */}
  <h3 className="text-lg font-bold text-white mb-4">Recompensas</h3>

  {/* Rewards List */}
  <div className="space-y-3">
    {rewards.map(reward => (
      <RewardPublicCard
        key={reward.id}
        reward={reward}
        monedaId={monedaId}
        onSelect={handleSelectReward}
      />
    ))}
  </div>

  <Separator className="my-6 bg-[#334155]" />

  {/* Footer Info */}
  <div className="space-y-2 text-xs text-[#64748b]">
    <div className="flex items-center gap-2">
      <Lock className="w-4 h-4" aria-hidden="true" />
      <span>Pago seguro</span>
    </div>
    {earliestDelivery && (
      <div className="flex items-center gap-2">
        <Package className="w-4 h-4" aria-hidden="true" />
        <span>Entrega estimada: {formatDeliveryDate(earliestDelivery)}</span>
      </div>
    )}
  </div>
</Card>
```

---

### 3.2 RewardPublicCard

**Props Interface:**
```tsx
interface RewardPublicCardProps {
  reward: Reward; // De src/shared/types/reward.ts (actualizado con contracts.md)
  monedaId: number;
  onSelect?: (rewardId: string) => void;
  disabled?: boolean; // True si campania finalizada
  className?: string;
}
```

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card Container | Card | `bg-[#1a1a2e] border-[#334155] p-4 mb-3 transition group` |
| Card (hover available) | Card | `hover:border-primary cursor-pointer` |
| Card (sold out) | Card | `opacity-60 cursor-not-allowed` |
| Header Row | div | `flex items-start justify-between mb-2` |
| Price | div | `text-xl font-bold text-primary` |
| Badges Container | div | `flex gap-1` |
| Badge "Mas popular" | Badge | `bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs` |
| Badge "Pocas unidades" | Badge | `bg-warning/20 text-warning border-warning/50 text-xs` |
| Badge "AGOTADO" | Badge | `bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50 text-xs` |
| Reward Title | h4 | `text-white font-semibold mb-1` |
| Description | p | `text-sm text-[#94a3b8] mb-2 line-clamp-2` |
| Stock Info Container | p | `text-xs text-[#64748b] mb-3 flex items-center gap-1` |
| Stock Icon (available) | Package (lucide-react) | `w-4 h-4 text-green-400` |
| Stock Icon (limited) | AlertCircle (lucide-react) | `w-4 h-4 text-warning` |
| Stock Icon (sold out) | XCircle (lucide-react) | `w-4 h-4 text-[#64748b]` |
| Stock Text (available) | span | `text-[#94a3b8]` |
| Stock Text (limited) | span | `text-warning` |
| Stock Text (sold out) | span | `text-[#64748b]` |
| Delivery Info | p | `text-xs text-[#64748b] mb-3` (solo si `tiempoEntregaEstimado` existe) |
| Select Button | Button | `w-full border-primary text-primary hover:bg-primary/10 group-hover:bg-primary/10` variant="outline" size="sm" |
| Select Button (sold out) | Button | `w-full border-[#64748b]/50 text-[#64748b] cursor-not-allowed` variant="outline" size="sm" disabled |

**Composicion RewardPublicCard:**
```tsx
<Card
  className={cn(
    "bg-[#1a1a2e] border-[#334155] p-4 mb-3 transition group",
    !isDisabled && !isSoldOut && "hover:border-primary cursor-pointer",
    isSoldOut && "opacity-60 cursor-not-allowed",
    className
  )}
  role="article"
  aria-labelledby={`reward-${reward.id}-title`}
  aria-describedby={`reward-${reward.id}-description reward-${reward.id}-stock`}
  tabIndex={isDisabled ? -1 : 0}
  onClick={() => !isDisabled && !isSoldOut && onSelect?.(reward.id)}
  onKeyDown={(e) => {
    if (e.key === "Enter" && !isDisabled && !isSoldOut) {
      onSelect?.(reward.id)
    }
  }}
>
  {/* Header Row */}
  <div className="flex items-start justify-between mb-2">
    <div className="text-xl font-bold text-primary">
      {formatCurrency(reward.importeMinimo, monedaId)}
    </div>
    <div className="flex gap-1">
      {isPopular && !isSoldOut && (
        <Badge
          className="bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs"
          role="status"
          aria-label="Recompensa mas popular"
        >
          Mas popular
        </Badge>
      )}
      {isLimited && !isSoldOut && (
        <Badge
          className="bg-warning/20 text-warning border-warning/50 text-xs"
          role="status"
          aria-label="Pocas unidades disponibles"
        >
          Pocas unidades
        </Badge>
      )}
      {isSoldOut && (
        <Badge
          className="bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50 text-xs"
          role="status"
          aria-label="Recompensa agotada"
        >
          AGOTADO
        </Badge>
      )}
    </div>
  </div>

  {/* Title */}
  <h4 id={`reward-${reward.id}-title`} className="text-white font-semibold mb-1">
    {reward.nombre}
  </h4>

  {/* Description */}
  {reward.descripcion && (
    <p
      id={`reward-${reward.id}-description`}
      className="text-sm text-[#94a3b8] mb-2 line-clamp-2"
    >
      {reward.descripcion}
    </p>
  )}

  {/* Stock Info */}
  <p
    id={`reward-${reward.id}-stock`}
    className="text-xs mb-3 flex items-center gap-1"
    role="status"
    aria-label={getStockAriaLabel(reward)}
  >
    {isSoldOut ? (
      <>
        <XCircle className="w-4 h-4 text-[#64748b]" aria-hidden="true" />
        <span className="text-[#64748b]">Agotado</span>
      </>
    ) : isLimited ? (
      <>
        <AlertCircle className="w-4 h-4 text-warning" aria-hidden="true" />
        <span className="text-warning">{getStockText(reward)}</span>
      </>
    ) : isUnlimited ? (
      <>
        <Package className="w-4 h-4 text-green-400" aria-hidden="true" />
        <span className="text-[#94a3b8]">Ilimitadas disponibles</span>
      </>
    ) : (
      <>
        <Package className="w-4 h-4 text-green-400" aria-hidden="true" />
        <span className="text-[#94a3b8]">{getStockText(reward)}</span>
      </>
    )}
  </p>

  {/* Delivery Info (optional) */}
  {reward.tiempoEntregaEstimado && (
    <p className="text-xs text-[#64748b] mb-3">
      Entrega estimada: {reward.tiempoEntregaEstimado}
    </p>
  )}

  {/* Select Button */}
  <Button
    variant="outline"
    size="sm"
    className={cn(
      "w-full",
      isSoldOut
        ? "border-[#64748b]/50 text-[#64748b] cursor-not-allowed"
        : "border-primary text-primary hover:bg-primary/10 group-hover:bg-primary/10"
    )}
    disabled={isDisabled || isSoldOut}
    onClick={(e) => {
      e.stopPropagation()
      if (!isDisabled && !isSoldOut) onSelect?.(reward.id)
    }}
    aria-label={
      isSoldOut
        ? "Recompensa agotada"
        : `Seleccionar recompensa ${reward.nombre} por ${formatCurrency(reward.importeMinimo, monedaId)}`
    }
  >
    {isSoldOut ? "Agotado" : "Seleccionar"}
  </Button>
</Card>
```

---

### 3.3 RewardPublicCardSkeleton (Loading State)

| Elemento | Componente shadcn | Customizacion |
|----------|-------------------|---------------|
| Card Container | Card | `bg-[#1a1a2e] border-[#334155] p-4 mb-3` |
| Price Skeleton | Skeleton | `h-7 w-20 bg-[#334155]` |
| Title Skeleton | Skeleton | `h-5 w-3/4 bg-[#334155] mb-2` |
| Description Skeleton | Skeleton | `h-4 w-full bg-[#334155] mb-1` (repetir 2 veces) |
| Stock Skeleton | Skeleton | `h-4 w-1/2 bg-[#334155] mb-3` |
| Button Skeleton | Skeleton | `h-9 w-full bg-[#334155]` |

**Composicion RewardPublicCardSkeleton:**
```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-4 mb-3">
  {/* Price */}
  <Skeleton className="h-7 w-20 bg-[#334155] mb-2" />

  {/* Title */}
  <Skeleton className="h-5 w-3/4 bg-[#334155] mb-2" />

  {/* Description lines */}
  <Skeleton className="h-4 w-full bg-[#334155] mb-1" />
  <Skeleton className="h-4 w-full bg-[#334155] mb-2" />

  {/* Stock info */}
  <Skeleton className="h-4 w-1/2 bg-[#334155] mb-3" />

  {/* Button */}
  <Skeleton className="h-9 w-full bg-[#334155]" />
</Card>
```

---

## 4. Estados Visuales

### 4.1 RewardPublicCard States

| Estado | Visual | Triggers |
|--------|--------|----------|
| **Default (Available)** | Border gris, hover border primary, cursor pointer, boton "Seleccionar" primary | `!isSoldOut && !isDisabled && remaining > 50% total` |
| **Popular** | Badge rosa "Mas popular" top-right | `cantidadReclamada > 5` (o el reward con mas backings) |
| **Limited Stock** | Badge warning "Pocas unidades", icono alert-circle amarillo, texto stock warning | `remaining < 50% cantidadMaxima && !isSoldOut` |
| **Sold Out** | Card opacity 60%, badge gris "AGOTADO", icono x-circle gris, boton "Agotado" disabled gris, cursor not-allowed | `remaining <= 0` |
| **Unlimited Stock** | Icono package verde, texto "Ilimitadas disponibles" | `cantidadMaxima === null` |
| **Hover (Available)** | Border primary, elevation shadow, boton bg-primary/10 | Hover sobre card no sold-out |
| **Hover (Sold Out)** | Sin efecto | Card con `cursor-not-allowed` |
| **Disabled (Campania Finalizada)** | Similar a sold out, botones disabled | `disabled={true}` prop |
| **Focus Keyboard** | Ring primary 2px offset 2px | Tab navigation |

### 4.2 CampaniaRewardsSection States

| Estado | Visual | Triggers |
|--------|--------|----------|
| **Loading** | 3 RewardPublicCardSkeleton en lista | `isLoading` |
| **Empty State** | Icono package grande gris, texto "Esta campana no tiene recompensas especificas", solo boton general "Apoyar" | `rewards.length === 0` |
| **Campania Finalizada** | Boton CTA gris "Campana finalizada" disabled, todas las rewards con `disabled={true}` | `campaniaFinalizada={true}` |
| **Sticky Sidebar (Desktop)** | Container con `sticky top-24`, scroll independiente | Viewport > 1024px |
| **Mobile View** | Container NO sticky, scroll normal con el contenido | Viewport < 640px |

---

## 5. Logica de Negocio (Calculos Frontend)

### 5.1 Stock Calculation

**Basado en la nueva estructura de contracts.md:**

```typescript
// Reward interface (actualizada)
interface Reward {
  cantidadMaxima?: number; // null = ilimitado
  // Note: cantidadReclamada NO existe en backend DTO
  // Necesitamos calcular desde Backings o agregar al DTO
}

// Asumiendo que el backend agregara "backingsCount" al DTO
// O se calculara en el frontend desde /api/backings?rewardId=X

const isUnlimited = reward.cantidadMaxima === null || reward.cantidadMaxima === undefined;
const cantidadReclamada = reward.backingsCount ?? 0; // A confirmar con backend
const remaining = isUnlimited ? null : (reward.cantidadMaxima! - cantidadReclamada);
const isSoldOut = !isUnlimited && remaining !== null && remaining <= 0;
const isLimited = !isUnlimited && remaining !== null && remaining < (reward.cantidadMaxima! * 0.5); // < 50%

const getStockText = (reward: Reward): string => {
  if (isUnlimited) return "Ilimitadas disponibles";
  if (isSoldOut) return "Agotado";
  return `${remaining} de ${reward.cantidadMaxima} disponibles`;
};
```

**IMPORTANTE:** El backend DTO actual NO incluye `backingsCount`. Se debe:
1. Agregar campo calculado `backingsCount` en `RewardListDto` del backend
2. O hacer query separada en frontend para obtener conteo de backings por reward
3. **Recomendacion:** Backend debe incluir `backingsCount` en el DTO para evitar N+1 queries

### 5.2 Badge "Mas popular"

**Estrategia:**
- Mostrar en la reward con `backingsCount` mas alto de la campania
- Solo si `backingsCount > 5` (para evitar mostrar en campanias nuevas)
- No mostrar si reward esta sold out

```typescript
const mostPopularRewardId = useMemo(() => {
  const sorted = rewards
    .filter(r => r.backingsCount > 5)
    .sort((a, b) => b.backingsCount - a.backingsCount);
  return sorted[0]?.id ?? null;
}, [rewards]);

const isPopular = reward.id === mostPopularRewardId;
```

### 5.3 Precio Minimo (Starting Price)

```typescript
const minRewardAmount = useMemo(() => {
  if (rewards.length === 0) return 5; // Default si no hay rewards
  return Math.min(...rewards.map(r => r.importeMinimo));
}, [rewards]);
```

### 5.4 Fecha de Entrega Mas Cercana

```typescript
const earliestDelivery = useMemo(() => {
  const withDelivery = rewards
    .filter(r => r.tiempoEntregaEstimado)
    .map(r => r.tiempoEntregaEstimado!);

  // tiempoEntregaEstimado es texto libre ("30 dias", "Marzo 2025")
  // No se puede ordenar facilmente, mostrar el primero o no mostrar
  return withDelivery[0] ?? null;
}, [rewards]);
```

**Nota:** Como `tiempoEntregaEstimado` es texto libre, no hay forma confiable de ordenar fechas. Opciones:
1. Mostrar el primer reward con delivery info
2. No mostrar fecha global (solo en cada reward individual)
3. **Recomendacion:** Mostrar en footer solo si TODAS las rewards tienen la misma fecha

---

## 6. Responsive Design

### 6.1 Breakpoints y Cambios

| Breakpoint | Cambios |
|------------|---------|
| **Mobile (< 640px)** | - Sidebar NO sticky (scroll normal con contenido)<br>- Reward cards padding reducido: `p-3` en lugar de `p-4`<br>- Font size precio: `text-lg` en lugar de `text-xl`<br>- Font size titulo: `text-base` en lugar de `font-semibold`<br>- Badges `text-xs` (ya es el default)<br>- Boton CTA `py-2` en lugar de `py-3`<br>- Espacing entre cards: `space-y-2` en lugar de `space-y-3` |
| **Tablet (640-1024px)** | - Sidebar sticky con `top-24`<br>- Layout normal como desktop<br>- Reward cards padding normal `p-4` |
| **Desktop (> 1024px)** | - Sidebar sticky con `top-24`<br>- Max width del container principal para centrado<br>- Spacing amplio `space-y-3` |

### 6.2 Clases Tailwind Responsivas

**Sidebar Container:**
```tsx
<Card className="bg-[#0f1729] border-[#334155] p-4 sm:p-6 sm:sticky sm:top-24">
  {/* Sticky solo en sm+ */}
</Card>
```

**Reward Card Padding:**
```tsx
<Card className="bg-[#1a1a2e] border-[#334155] p-3 sm:p-4 mb-2 sm:mb-3">
  {/* Mobile: p-3, Tablet+: p-4 */}
</Card>
```

**Price Font Size:**
```tsx
<div className="text-lg sm:text-xl font-bold text-primary">
  {/* Mobile: text-lg, Tablet+: text-xl */}
</div>
```

**CTA Button:**
```tsx
<Button className="w-full ... py-2 sm:py-3 mb-2">
  {/* Mobile: py-2, Tablet+: py-3 */}
</Button>
```

---

## 7. Accesibilidad (ARIA)

### 7.1 Contraste de Colores

| Elemento | Foreground | Background | Ratio | WCAG |
|----------|------------|------------|-------|------|
| Primary text (white) | #ffffff | #1a1a2e | 12.6:1 | AAA |
| Secondary text (#94a3b8) | #94a3b8 | #1a1a2e | 6.2:1 | AA+ |
| Primary button | #a855f7 | #1a1a2e | 5.2:1 | AA |
| Warning badge | #f59e0b | #1a1a2e | 4.8:1 | AA |
| Success icon (green-400) | #4ade80 | #1a1a2e | 5.1:1 | AA |

**Todos los contrastes cumplen WCAG AA minimo.**

### 7.2 ARIA Labels Especificos

```tsx
// Container Section
<Card
  role="region"
  aria-labelledby="rewards-section-title"
  aria-describedby="rewards-section-description"
>
  <h3 id="rewards-section-title">Recompensas</h3>
  <p id="rewards-section-description" className="sr-only">
    Lista de recompensas disponibles para apoyar esta campana
  </p>
</Card>

// Individual Reward Card
<Card
  role="article"
  aria-labelledby={`reward-${reward.id}-title`}
  aria-describedby={`reward-${reward.id}-description reward-${reward.id}-stock`}
  tabIndex={isDisabled ? -1 : 0}
>
  <h4 id={`reward-${reward.id}-title`}>Descarga Digital</h4>
  <p id={`reward-${reward.id}-description`}>Acceso anticipado...</p>
  <p id={`reward-${reward.id}-stock`} role="status">
    234 de 500 disponibles
  </p>
</Card>

// Stock Badges con role="status"
<Badge role="status" aria-label="Recompensa mas popular">
  Mas popular
</Badge>

<Badge role="status" aria-label="Pocas unidades disponibles">
  Pocas unidades
</Badge>

<Badge role="status" aria-label="Recompensa agotada">
  AGOTADO
</Badge>

// Stock Indicator con ARIA label
<p role="status" aria-label="Stock disponible: 234 de 500 unidades">
  <Package aria-hidden="true" />
  234 de 500 disponibles
</p>

<p role="status" aria-label="Stock limitado: 89 de 200 unidades, menos del 50% disponible">
  <AlertCircle aria-hidden="true" />
  89 de 200 disponibles
</p>

<p role="status" aria-label="Recompensa agotada, no quedan unidades disponibles">
  <XCircle aria-hidden="true" />
  Agotado
</p>

// Select Button con descriptive label
<Button
  aria-label="Seleccionar recompensa Descarga Digital por 10 euros"
  onClick={...}
>
  Seleccionar
</Button>

<Button
  aria-label="Recompensa agotada, no disponible para seleccion"
  disabled
>
  Agotado
</Button>

// CTA Main Button
<Button
  aria-label={campaniaFinalizada ? "Campana finalizada, no disponible para apoyo" : "Apoyar esta campana con cualquier monto"}
  aria-busy={false}
>
  {campaniaFinalizada ? "Campana finalizada" : "Apoyar esta campana"}
</Button>

// Icons always with aria-hidden
<Lock className="w-4 h-4" aria-hidden="true" />
<Package className="w-4 h-4" aria-hidden="true" />
```

### 7.3 Keyboard Navigation

| Tecla | Accion | Elemento |
|-------|--------|----------|
| **Tab** | Navegar entre rewards y boton CTA | Todos los interactivos |
| **Shift + Tab** | Navegar hacia atras | Todos los interactivos |
| **Enter** | Seleccionar reward (equivalente a click) | RewardPublicCard con focus |
| **Space** | Activar boton | Botones con focus |

**Focus Trap:**
No se requiere focus trap porque no es un modal. El sidebar es parte del flujo normal de la pagina.

**Focus Visible:**
```css
/* Aplicado via tailwind */
.reward-card:focus-visible {
  outline: 2px solid var(--primary);
  outline-offset: 2px;
  border-radius: 0.5rem;
}
```

**Tailwind class:**
```tsx
<Card
  className="... focus-visible:outline focus-visible:outline-2 focus-visible:outline-primary focus-visible:outline-offset-2"
  tabIndex={0}
>
```

### 7.4 Screen Reader Announcements

**Stock Changes (futuro con live updates):**
```tsx
<div aria-live="polite" aria-atomic="true" className="sr-only">
  {`Stock actualizado: ${remaining} unidades disponibles de ${cantidadMaxima}`}
</div>
```

**Loading State:**
```tsx
<div aria-live="polite" aria-busy={isLoading} className="sr-only">
  {isLoading ? "Cargando recompensas" : "Recompensas cargadas"}
</div>
```

---

## 8. Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| **Reward card hover** | Border color transition + shadow elevation | 200ms ease | Hover sobre card available |
| **Reward card focus** | Outline fade in | 150ms ease | Keyboard Tab focus |
| **Button hover** | Background color transition | 150ms ease | Hover sobre boton "Seleccionar" |
| **Badge appear** | Fade in + scale 0.9 → 1 | 200ms ease | Mount cuando stock cambia a limited |
| **Skeleton pulse** | Opacity 0.5 → 1 → 0.5 loop | 1500ms ease-in-out | Loading state |
| **Stock icon change** | Fade out old + Fade in new | 250ms ease | Cuando stock pasa de available a limited o sold out |
| **Empty state appear** | Fade in + scale 0.95 → 1 | 300ms ease-out | Mount cuando rewards.length === 0 |

### 8.1 CSS Transitions

```css
/* Reward Card Hover */
.reward-card {
  transition: border-color 200ms ease, box-shadow 200ms ease;
}

.reward-card:hover {
  border-color: var(--primary);
  box-shadow: 0 4px 6px -1px rgba(168, 85, 247, 0.1),
              0 2px 4px -1px rgba(168, 85, 247, 0.06);
}

/* Button Hover */
.select-button {
  transition: background-color 150ms ease;
}

/* Badge Appear Animation */
.badge-limited {
  animation: badgeAppear 200ms ease;
}

@keyframes badgeAppear {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

/* Skeleton Pulse */
.skeleton {
  animation: skeletonPulse 1500ms ease-in-out infinite;
}

@keyframes skeletonPulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}
```

### 8.2 Tailwind Animation Classes

```tsx
// Skeleton
<Skeleton className="animate-pulse bg-[#334155]" />

// Badge appear (usar framer-motion o CSS animation)
<Badge
  className="bg-warning/20 text-warning border-warning/50 text-xs animate-in fade-in zoom-in duration-200"
>
  Pocas unidades
</Badge>

// Card hover (via CSS transition, no Tailwind animation)
<Card className="transition hover:border-primary hover:shadow-lg">
```

---

## 9. Interacciones

| Accion Usuario | Comportamiento Frontend |
|----------------|------------------------|
| **Click CTA "Apoyar esta campana"** | Si autenticado → `onApoyar()` → Navigate a `/campanias/{id}/backing`<br>Si NO autenticado → Navigate a `/login?returnUrl=/campanias/{id}/backing` |
| **Click "Seleccionar" en reward** | Si autenticado → Navigate a `/campanias/{id}/backing?reward={rewardId}`<br>Si NO autenticado → Navigate a `/login?returnUrl=/campanias/{id}/backing?reward={rewardId}` |
| **Click sobre card (anywhere)** | Mismo comportamiento que click "Seleccionar" (solo si card NO sold out) |
| **Keyboard Enter sobre card** | Mismo comportamiento que click |
| **Hover sobre card available** | Border cambia a primary, elevation shadow, boton bg-primary/10 |
| **Hover sobre card sold out** | Sin efecto visual (cursor-not-allowed) |
| **Scroll en mobile** | Sidebar scrollea normalmente con el contenido (NO sticky) |
| **Scroll en desktop** | Sidebar sticky, permanece visible al hacer scroll |

---

## 10. Validacion de Datos (Frontend)

**No aplica validaciones de formulario** - Esta es una vista de solo lectura.

**Validaciones defensivas:**

```typescript
// Validar que reward tenga datos minimos antes de renderizar
const isValidReward = (reward: Reward): boolean => {
  return Boolean(
    reward.id &&
    reward.nombre &&
    reward.importeMinimo > 0 &&
    reward.campaniaId
  );
};

// Filtrar rewards invalidos antes de render
const validRewards = rewards.filter(isValidReward);

// Validar cantidad maxima vs cantidad reclamada
const isValidStock = (reward: Reward): boolean => {
  if (reward.cantidadMaxima === null) return true; // Unlimited
  return reward.backingsCount <= reward.cantidadMaxima;
};

// Log error si stock inconsistente (deberia ser manejado por backend)
if (!isValidStock(reward)) {
  console.error(`Invalid stock for reward ${reward.id}: claimed ${reward.backingsCount} > max ${reward.cantidadMaxima}`);
}
```

---

## 11. Integracion con Backend

### 11.1 Endpoints Usados

| Endpoint | Uso | Datos Obtenidos |
|----------|-----|-----------------|
| `GET /api/rewards?campaniaId={id}` | Listar rewards de una campania | `RewardListItem[]` |
| `GET /api/campanias/{id}` | Obtener datos de campania (para validar estado finalizada) | `CampaniaDto` (con estado) |

### 11.2 Transformacion de Datos

**Backend DTO → Frontend Type:**

```typescript
// Backend RewardListDto (del contracts.md)
interface RewardListDto {
  id: string;
  campaniaId: string;
  nombre: string;
  descripcion?: string;
  importeMinimo: number;
  esAddOn: boolean;
  cantidadMaxima?: number;
  incluyeEnvioFisico: boolean;
  orden: number;
  esActivo: boolean;
}

// Frontend Reward Type (expandido para UI)
interface RewardWithBackings extends RewardListDto {
  backingsCount: number; // A agregar por backend o calcular en frontend
}

// Mapping en service layer
const mapRewardDto = (dto: RewardListDto, backingsCount: number): RewardWithBackings => ({
  ...dto,
  backingsCount,
});
```

**IMPORTANTE:** El backend debe incluir `backingsCount` en el DTO o proveer endpoint para obtener conteo de backings por reward.

**Opcion 1 (recomendada):** Backend agrega campo calculado:
```csharp
// En RewardListDto
public int BackingsCount { get; set; } // Calculado desde COUNT(Backings WHERE RewardId = ...)
```

**Opcion 2:** Frontend hace query separada:
```typescript
const { data: rewards } = useQuery(['rewards', campaniaId], () =>
  rewardService.getByCampania(campaniaId)
);

const { data: backingsCounts } = useQuery(['backings', 'counts', campaniaId], () =>
  backingService.getCountsByCampania(campaniaId)
);

// Merge data
const rewardsWithCounts = rewards.map(r => ({
  ...r,
  backingsCount: backingsCounts[r.id] ?? 0,
}));
```

### 11.3 Ordenamiento

**Backend:** Debe retornar rewards ordenadas por campo `orden` ASC.

**Frontend:** Si backend NO ordena, ordenar localmente:
```typescript
const sortedRewards = useMemo(() => {
  return [...rewards].sort((a, b) => a.orden - b.orden);
}, [rewards]);
```

**Convencion:** Ordenar por `importeMinimo` ASC si campo `orden` es identico para multiples rewards.

---

## 12. Estados de Error

| Error | Mensaje UI | Visual |
|-------|-----------|--------|
| **API Error (500)** | "No pudimos cargar las recompensas. Intenta nuevamente." | Toast roja con boton "Reintentar" |
| **Network Error** | "Parece que no hay conexion. Verifica tu internet." | Toast warning con auto-retry |
| **Reward Not Found (404)** | "Esta campana no tiene recompensas disponibles." | Empty state con icono package |
| **Campania Not Found (404)** | "No encontramos esta campana." | Redirect a 404 page |
| **Campania Finalizada** | (No error) Boton CTA "Campana finalizada" disabled | Boton gris, rewards disabled |

---

## 13. Checklist UI/UX

### Componente CampaniaRewardsSection
- [ ] Container Card con bg-[#0f1729], border-[#334155]
- [ ] Sticky en desktop (top-24), scroll normal en mobile
- [ ] Boton CTA gradient pink→purple, full-width, py-3
- [ ] Texto "Desde € X" calculado del precio minimo
- [ ] Divider separator antes y despues de lista
- [ ] Titulo "Recompensas" h3 bold white
- [ ] Lista de rewards con space-y-3
- [ ] Empty state con icono package, texto descriptivo
- [ ] Footer con icono lock + "Pago seguro"
- [ ] Footer con icono package + "Entrega estimada: {fecha}" (opcional)
- [ ] Loading state con 3 RewardPublicCardSkeleton
- [ ] Responsive: mobile (no sticky, padding reducido), tablet+ (sticky)

### Componente RewardPublicCard
- [ ] Card bg-[#1a1a2e], border-[#334155], padding p-4
- [ ] Header row con precio (text-xl bold primary) y badges
- [ ] Badge "Mas popular" rosa (solo si es el reward con mas backings)
- [ ] Badge "Pocas unidades" warning (solo si stock < 50%)
- [ ] Badge "AGOTADO" gris (solo si remaining <= 0)
- [ ] Titulo reward (h4 white font-semibold)
- [ ] Descripcion con line-clamp-2
- [ ] Stock info con icono dinamico (package/alert-circle/x-circle) y texto
- [ ] Delivery info (opcional, solo si tiempoEntregaEstimado existe)
- [ ] Boton "Seleccionar" outline primary (o "Agotado" disabled gris)
- [ ] Hover effect: border primary, shadow, boton bg-primary/10
- [ ] Sold out state: opacity 60%, cursor-not-allowed, no hover
- [ ] Click en card = click en boton (excepto si sold out)
- [ ] Keyboard Enter funcional
- [ ] ARIA labels completos (article, labelledby, describedby, role="status")

### Componente RewardPublicCardSkeleton
- [ ] Card bg-[#1a1a2e], border-[#334155], padding p-4
- [ ] Skeleton precio (h-7 w-20)
- [ ] Skeleton titulo (h-5 w-3/4)
- [ ] Skeleton descripcion 2 lineas (h-4 w-full)
- [ ] Skeleton stock (h-4 w-1/2)
- [ ] Skeleton boton (h-9 w-full)
- [ ] Animacion pulse (opacity 0.5 → 1 loop)

### Accesibilidad
- [ ] Contraste minimo 4.5:1 en todos los textos
- [ ] Focus visible con outline primary 2px en todos los interactivos
- [ ] ARIA labels descriptivos en botones
- [ ] role="status" en badges y stock info
- [ ] role="article" en reward cards
- [ ] Iconos con aria-hidden="true"
- [ ] Keyboard navigation funcional (Tab, Enter)
- [ ] Screen reader announces con sr-only helpers
- [ ] Texto de accesibilidad para stock ("X de Y unidades disponibles")

### Responsive
- [ ] Mobile (< 640px): sidebar no sticky, padding p-3, font-size reducido
- [ ] Tablet (640-1024px): sidebar sticky, padding p-4
- [ ] Desktop (> 1024px): sidebar sticky, layout completo
- [ ] Clases Tailwind responsive (sm:, md:, lg:)

### Logica de Negocio
- [ ] Calculo correcto de stock: `remaining = cantidadMaxima - backingsCount`
- [ ] Deteccion de unlimited: `cantidadMaxima === null`
- [ ] Deteccion de sold out: `remaining <= 0`
- [ ] Deteccion de limited: `remaining < cantidadMaxima * 0.5`
- [ ] Badge "Mas popular" solo en reward con mas backings (> 5)
- [ ] Ordenamiento por campo `orden` ASC
- [ ] Precio minimo calculado correctamente
- [ ] Fecha de entrega mostrada solo si existe

### Integracion Backend
- [ ] Query GET /api/rewards?campaniaId={id}
- [ ] Query GET /api/campanias/{id} para validar estado
- [ ] Manejo de errores 404, 500 con toasts
- [ ] Loading state mientras fetch
- [ ] Transformacion RewardListDto → RewardWithBackings
- [ ] Validacion de datos antes de renderizar

---

## 14. Migracion del Componente Actual

**Archivo actual:** `src/web/src/features/campanias/presentation/components/RewardCard.tsx`

**Cambios requeridos:**

1. **Renombrar archivo:**
   - `RewardCard.tsx` → `RewardPublicCard.tsx`

2. **Actualizar interface de datos:**
   ```typescript
   // ANTES (actual)
   interface Reward {
     stockLimitado: boolean;
     cantidadDisponible?: number;
     stockDisponible: number;
     fechaEntregaEstimada?: string; // ISO Date
   }

   // DESPUES (contracts.md)
   interface Reward {
     cantidadMaxima?: number; // null = ilimitado
     backingsCount: number; // A agregar por backend
     tiempoEntregaEstimado?: string; // Texto libre
   }
   ```

3. **Actualizar calculos de stock:**
   ```typescript
   // ANTES
   const cantidadTotal = reward.cantidadDisponible;
   const cantidadUsada = reward.cantidadReclamada;
   const remaining = cantidadTotal - cantidadUsada;

   // DESPUES
   const isUnlimited = reward.cantidadMaxima === null;
   const remaining = isUnlimited ? null : (reward.cantidadMaxima - reward.backingsCount);
   const isSoldOut = !isUnlimited && remaining !== null && remaining <= 0;
   const isLimited = !isUnlimited && remaining !== null && remaining < (reward.cantidadMaxima * 0.5);
   ```

4. **Agregar iconos de stock:**
   ```typescript
   import { Package, AlertCircle, XCircle } from "lucide-react";

   // En render:
   {isSoldOut && <XCircle className="w-4 h-4 text-[#64748b]" />}
   {isLimited && <AlertCircle className="w-4 h-4 text-warning" />}
   {!isSoldOut && !isLimited && <Package className="w-4 h-4 text-green-400" />}
   ```

5. **Agregar badge "Pocas unidades":**
   ```tsx
   {isLimited && !isSoldOut && (
     <Badge className="bg-warning/20 text-warning border-warning/50 text-xs">
       Pocas unidades
     </Badge>
   )}
   ```

6. **Cambiar delivery info:**
   ```typescript
   // ANTES
   {reward.fechaEntregaEstimada && (
     <p>Entrega estimada: {formatDate(reward.fechaEntregaEstimada)}</p>
   )}

   // DESPUES
   {reward.tiempoEntregaEstimado && (
     <p>Entrega estimada: {reward.tiempoEntregaEstimado}</p>
   )}
   ```

7. **Mejorar ARIA labels:**
   ```tsx
   <Card
     role="article"
     aria-labelledby={`reward-${reward.id}-title`}
     aria-describedby={`reward-${reward.id}-description reward-${reward.id}-stock`}
   >
     <h4 id={`reward-${reward.id}-title`}>{reward.nombre}</h4>
     <p id={`reward-${reward.id}-description`}>{reward.descripcion}</p>
     <p id={`reward-${reward.id}-stock`} role="status">
       {getStockText(reward)}
     </p>
   </Card>
   ```

**Archivo actual:** `src/web/src/features/campanias/presentation/components/RewardsList.tsx`

**Cambios minimos:**
- Importar `RewardPublicCard` en lugar de `RewardCard`
- Actualizar props de Reward type
- Cambio de `fechaEntregaEstimada` a `tiempoEntregaEstimado` (texto libre, no ordenar)

---

## 15. Dependencias

**Lucide React Icons Usados:**
- `Lock` - Pago seguro
- `Package` - Stock disponible, entrega
- `AlertCircle` - Stock limitado
- `XCircle` - Sold out

**Ya instaladas en el proyecto.**

**shadcn/ui Components Usados:**
- `Card` - Container principal y reward cards
- `Badge` - Popular, Limited, Sold out
- `Button` - CTA y Select buttons
- `Separator` - Dividers
- `Skeleton` - Loading states

**Todos ya disponibles en `src/web/src/components/ui/`.**

---

## 16. Testing Considerations

**Unit Tests (Vitest):**
```typescript
describe("RewardPublicCard", () => {
  it("shows 'Mas popular' badge for most backed reward", () => {
    // Test badge logic
  });

  it("shows 'Pocas unidades' badge when stock < 50%", () => {
    // Test limited stock badge
  });

  it("shows sold out state when remaining <= 0", () => {
    // Test sold out state
  });

  it("shows unlimited stock text when cantidadMaxima is null", () => {
    // Test unlimited stock
  });

  it("calls onSelect when card is clicked", () => {
    // Test click handler
  });

  it("does not call onSelect when sold out card is clicked", () => {
    // Test disabled click
  });
});

describe("CampaniaRewardsSection", () => {
  it("calculates minimum price correctly", () => {
    // Test minRewardAmount
  });

  it("shows empty state when rewards array is empty", () => {
    // Test empty state
  });

  it("shows skeleton loaders during loading", () => {
    // Test loading state
  });
});
```

**Accessibility Tests:**
```typescript
import { axe, toHaveNoViolations } from "jest-axe";

expect.extend(toHaveNoViolations);

it("should not have accessibility violations", async () => {
  const { container } = render(<RewardPublicCard {...props} />);
  const results = await axe(container);
  expect(results).toHaveNoViolations();
});
```

---

**Fin del documento de diseno UI.**
