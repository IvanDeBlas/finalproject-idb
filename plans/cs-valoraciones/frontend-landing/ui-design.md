# Diseno UI: Valoraciones Bidireccionales (Landing)

**Fecha:** 2026-02-21
**Feature:** cs-valoraciones (US-CS-06)
**Target:** src/web (Landing publica - Vite + React 18 + Tailwind + shadcn/ui)

---

## 1. Resumen

Este documento define la arquitectura de componentes UI para la feature de valoraciones bidireccionales en la landing publica. La feature se integra en dos contextos existentes y expone un componente reutilizable global:

- **Pantalla 1:** Seccion de valoracion en `AcuerdoDetallePage` (formulario o vista read-only segun estado)
- **Pantalla 2:** Seccion de valoraciones en el perfil de un usuario (resumen + histograma + lista paginada)
- **Pantalla 3:** `RatingBadge` global reutilizable en cards de propuestas y listados de profesionales

**Componentes shadcn/ui utilizados:** 14 (Card, Textarea, Label, Button, Avatar, AvatarImage, AvatarFallback, Skeleton, Alert, AlertTitle, AlertDescription, Pagination, PaginationContent, PaginationItem)
**Componentes custom (no existen en shadcn):** 3 (StarRating, StarDisplay, RatingHistogram)
**Composiciones:** 6 (ValoracionForm, ValoracionReadOnly, ValoracionesSection, ValoracionListItem, RatingBadge, EmptyValoraciones)
**Responsive breakpoints:** mobile (< 768px), tablet (768-1024px), desktop (> 1024px)
**Dark theme:** Si (paleta oscura #1a1a2e / #0f1729, estrellas ambar #f59e0b, gradiente pink-purple)

---

## 2. Componentes shadcn/ui: Estado de Instalacion

### Ya instalados en `src/web/src/components/ui/`

| Componente | Archivo | Estado |
|------------|---------|--------|
| Button | `button.tsx` | Instalado |
| Card, CardContent | `card.tsx` | Instalado |
| Textarea | `textarea.tsx` | Instalado |
| Label | `label.tsx` | Instalado |
| Avatar, AvatarImage, AvatarFallback | `avatar.tsx` | Instalado |
| Skeleton | `skeleton.tsx` | Instalado |
| Alert, AlertTitle, AlertDescription | `alert.tsx` | Instalado |
| Pagination, PaginationContent, PaginationItem | `pagination.tsx` | Instalado |
| Badge | `badge.tsx` | Instalado |
| Separator | `separator.tsx` | Instalado |

### No requieren instalacion adicional

Todos los componentes shadcn necesarios ya estan presentes. El componente `Form` de shadcn/ui no se utiliza en esta feature porque el formulario de valoracion usa `react-hook-form` + `Controller` directamente con `Label` e `Input` custom, siguiendo el patron establecido en `EnviarPropuestaForm.tsx`.

---

## 3. Paleta de Colores (Dark Theme)

| Uso | Valor Hex | Tailwind | Ejemplo de aplicacion |
|-----|-----------|----------|-----------------------|
| Background Card | #0f1729 | `bg-[#0f1729]` | Todos los cards de valoracion |
| Background Card Hover | #1e2a42 | `bg-[#1e2a42]` | Skeleton loading, hover states |
| Background Input | #16213e | `bg-[#16213e]` | Textarea del formulario |
| Text Primary | #ffffff | `text-white` | Titulos, puntuacion media |
| Text Secondary | #94a3b8 | `text-[#94a3b8]` | Nombre autor, subtitulos |
| Text Muted | #64748b | `text-[#64748b]` | Fechas, contadores, hints |
| Text Label | #cbd5e1 | `text-[#cbd5e1]` | Labels del formulario |
| Text Comentario | #e2e8f0 | `text-[#e2e8f0]` | Cuerpo de comentarios |
| Border Default | #334155 | `border-[#334155]` | Bordes de cards e inputs |
| Border Hover | #475569 | `border-[#475569]` | Hover en ValoracionListItem |
| Border Focus | #a855f7 | `border-[#a855f7]` | Focus en Textarea |
| Border Success | #10b981/40 | `border-[#10b981]/40` | Card read-only (ya valorado) |
| Border Error | #ef4444 | `border-[#ef4444]` | Textarea con comentario excedido |
| Star Filled | #f59e0b | `text-[#f59e0b]` | Estrellas seleccionadas / llenas |
| Star Filled Hover | #fbbf24 | `text-[#fbbf24]` | Estrellas durante hover en StarRating |
| Star Empty | #334155 | `text-[#334155]` | Estrellas vacias |
| Histogram Bar Fill | #f59e0b -> #fbbf24 | `from-[#f59e0b] to-[#fbbf24]` | Relleno de barras del histograma |
| Histogram Bar BG | #334155 | `bg-[#334155]` | Track vacio de barras |
| Acuerdo Context | #a855f7 | `text-[#a855f7]` | Titulo del acuerdo en ValoracionListItem |
| Success Icon | #10b981 | `text-[#10b981]` | Icono CheckCircle2 en card read-only |
| Error Text | #ef4444 | `text-[#ef4444]` | Errores de validacion, contador excedido |
| Warning Text | #f59e0b | `text-[#f59e0b]` | Contador ambar (901-1000 chars) |
| Primary Gradient | linear-gradient(135deg, #ec4899, #a855f7) | `bg-gradient-to-r from-pink-500 to-purple-600` | Boton "Enviar valoracion", pagina activa |
| Primary Gradient Hover | linear-gradient(135deg, #f472b6, #c084fc) | `hover:from-pink-600 hover:to-purple-700` | Hover en boton CTA |

---

## 4. Design Tokens Especificos de Valoraciones

```css
/* CSS variables a definir en el index.css o globals.css del proyecto */

/* Estrellas */
--star-filled: #f59e0b;
--star-filled-hover: #fbbf24;
--star-empty: #334155;
--star-empty-hover: #475569;
--shadow-star-glow: 0 0 8px rgba(245, 158, 11, 0.5);

/* Histograma */
--histogram-bar-bg: #334155;
--histogram-bar-fill: #f59e0b;
--histogram-bar-fill-gradient: linear-gradient(90deg, #f59e0b 0%, #fbbf24 100%);
```

---

## 5. Componentes Custom (no existen en shadcn/ui)

### 5.1 StarRating

**Descripcion:** Selector interactivo de 1 a 5 estrellas con hover effect, soporte de teclado completo y accesibilidad ARIA como radiogroup. Solo se usa en el formulario de envio.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/StarRating.tsx`

**Props Interface:**
```typescript
interface StarRatingProps {
    value: number;                          // 0 = sin seleccion, 1-5 = seleccionado
    onChange: (value: number) => void;
    size?: 'sm' | 'md' | 'lg';             // default: 'md'
    disabled?: boolean;                     // default: false
    className?: string;
}
```

**Logica interna:**
```typescript
// Estado interno para el hover
const [hoverValue, setHoverValue] = useState<number>(0);

// Para determinar si la estrella i esta activa:
const isActive = (i: number): boolean =>
    hoverValue > 0 ? i <= hoverValue : i <= value;

// Manejo de teclado (flechas izquierda/derecha)
const handleKeyDown = (e: KeyboardEvent, i: number) => {
    if (e.key === 'ArrowRight' && value < 5) onChange(value + 1);
    if (e.key === 'ArrowLeft' && value > 1) onChange(value - 1);
    if (e.key === 'Enter' || e.key === ' ') onChange(i);
};
```

**Variantes de tamano:**

| Variant | Icono Star | Gap entre estrellas | Uso |
|---------|------------|---------------------|-----|
| `sm` | `w-3.5 h-3.5` | `gap-0.5` | Badge en perfil (StarDisplay small) |
| `md` | `w-8 h-8` | `gap-1` | Mobile (formulario, facilita tap) |
| `lg` | `w-10 h-10` | `gap-1` | Desktop (formulario principal) |

**Clases Tailwind por estado de cada estrella:**

| Estado | Clases Tailwind |
|--------|-----------------|
| Activa (hover activo) | `text-[#fbbf24] scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)] transition-transform duration-100` |
| Activa (seleccionada, sin hover) | `text-[#f59e0b] transition-transform duration-100` |
| Inactiva | `text-[#334155] transition-transform duration-100` |
| Click (active state) | `active:scale-125 transition-transform duration-150` |
| Disabled (toda el area) | `opacity-50 cursor-not-allowed` |

**Estructura JSX:**
```tsx
<div
    role="radiogroup"
    aria-label="Puntuacion de 1 a 5 estrellas"
    aria-required="true"
    className={cn("flex items-center", sizeConfig.gap, disabled && "opacity-50 cursor-not-allowed", className)}
    onMouseLeave={() => setHoverValue(0)}
>
    {[1, 2, 3, 4, 5].map((i) => (
        <button
            key={i}
            type="button"
            role="radio"
            aria-label={`${i} estrella${i > 1 ? 's' : ''}`}
            aria-checked={value === i}
            aria-pressed={value === i}
            disabled={disabled}
            className={cn(
                "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729] rounded-sm",
                "disabled:cursor-not-allowed",
                isActive(i) && hoverValue > 0
                    ? "text-[#fbbf24] scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)] transition-transform duration-100"
                    : isActive(i)
                        ? "text-[#f59e0b] transition-transform duration-100"
                        : "text-[#334155] transition-transform duration-100",
                "active:scale-125"
            )}
            onClick={() => !disabled && onChange(i)}
            onMouseEnter={() => !disabled && setHoverValue(i)}
            onKeyDown={(e) => handleKeyDown(e, i)}
        >
            <Star
                className={sizeConfig.icon}
                fill="currentColor"
                aria-hidden="true"
            />
        </button>
    ))}
</div>
```

**Imports necesarios:**
```typescript
import { useState } from 'react';
import { Star } from 'lucide-react';
import { cn } from '@/lib/utils';
```

**Accesibilidad:**
- `role="radiogroup"` en el contenedor con `aria-label="Puntuacion de 1 a 5 estrellas"`
- Cada estrella: `role="radio"`, `aria-label="{n} estrella{s}"`, `aria-checked={value === n}`
- Navegacion con Tab entre estrellas (cada boton es focuseable)
- Flechas izquierda/derecha: decrementar/incrementar valor
- Enter o Space: seleccionar la estrella actualmente en foco
- `focus-visible:ring-2 focus-visible:ring-[#a855f7]` visible en todos los items

---

### 5.2 StarDisplay

**Descripcion:** Visualizacion read-only de una puntuacion numerica. Soporta fracciones de 0.5 mediante diferencia de opacidad en la media estrella. Usado en ValoracionReadOnly, ValoracionListItem, RatingBadge (variante medium) y en la seccion de resumen del perfil.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/StarDisplay.tsx`

**Props Interface:**
```typescript
interface StarDisplayProps {
    value: number;          // 0-5, puede ser decimal (ej: 4.5)
    size?: 'sm' | 'md' | 'lg';   // default: 'md'
    showNumeric?: boolean;  // muestra "{value} / 5" a la derecha, default: false
    className?: string;
}
```

**Logica de renderizado de media estrella (MVP approach):**
```typescript
// Para una puntuacion de 4.5:
// - Estrellas 1 a 4: llenas (fill="currentColor", text-[#f59e0b])
// - Estrella 5: llena con opacity-50 (representa la media estrella)
// - Implementacion: redondeamos al 0.5 mas cercano

const getStarType = (starIndex: number, value: number): 'full' | 'half' | 'empty' => {
    const rounded = Math.round(value * 2) / 2; // redondear al 0.5 mas cercano
    if (starIndex <= Math.floor(rounded)) return 'full';
    if (starIndex === Math.ceil(rounded) && rounded % 1 !== 0) return 'half';
    return 'empty';
};

// Clases por tipo de estrella:
// 'full':  text-[#f59e0b], fill="currentColor"
// 'half':  text-[#f59e0b] opacity-50, fill="currentColor"
// 'empty': text-[#334155], fill="currentColor"
```

**Variantes de tamano:**

| Variant | Icono Star | Gap |
|---------|------------|-----|
| `sm` | `w-3.5 h-3.5` | `gap-0.5` |
| `md` | `w-4 h-4` | `gap-1` |
| `lg` | `w-5 h-5` | `gap-1` |

**Estructura JSX:**
```tsx
<div
    role="img"
    aria-label={`Puntuacion: ${value} de 5 estrellas`}
    className={cn("flex items-center", sizeConfig.gap, className)}
>
    {[1, 2, 3, 4, 5].map((i) => {
        const starType = getStarType(i, value);
        return (
            <Star
                key={i}
                className={cn(
                    sizeConfig.icon,
                    starType === 'empty' ? "text-[#334155]" : "text-[#f59e0b]",
                    starType === 'half' && "opacity-50"
                )}
                fill="currentColor"
                aria-hidden="true"
            />
        );
    })}
    {showNumeric && (
        <span className="text-sm text-[#94a3b8] ml-2" aria-hidden="true">
            {value} / 5
        </span>
    )}
</div>
```

**Imports necesarios:**
```typescript
import { Star } from 'lucide-react';
import { cn } from '@/lib/utils';
```

**Accesibilidad:**
- Contenedor con `role="img"` y `aria-label="Puntuacion: {value} de 5 estrellas"`
- Estrellas individuales con `aria-hidden="true"` (decorativas)
- El texto numerico (si showNumeric=true) tambien es `aria-hidden="true"` porque el aria-label del contenedor ya lo expresa

---

### 5.3 RatingHistogram

**Descripcion:** Barras de distribucion por nivel de estrella (5 a 1), proporcionales al total. Las barras se animan al montar el componente (expand desde la izquierda en 500ms).

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/RatingHistogram.tsx`

**Props Interface:**
```typescript
interface RatingHistogramProps {
    distribucion: {
        5: number;
        4: number;
        3: number;
        2: number;
        1: number;
    };
    total: number;
    className?: string;
}
```

**Logica de porcentaje:**
```typescript
const getPorcentaje = (count: number): number =>
    total > 0 ? Math.round((count / total) * 100) : 0;
```

**Estructura JSX - fila del histograma:**
```tsx
<div className={cn("space-y-2", className)}>
    {([5, 4, 3, 2, 1] as const).map((nivel) => {
        const count = distribucion[nivel];
        const porcentaje = getPorcentaje(count);
        return (
            <div key={nivel} className="flex items-center gap-2">
                {/* Etiqueta del nivel */}
                <span className="text-xs text-[#94a3b8] w-3 text-right flex-shrink-0">
                    {nivel}
                </span>
                {/* Icono estrella */}
                <Star
                    className="w-3 h-3 text-[#f59e0b] flex-shrink-0"
                    fill="currentColor"
                    aria-hidden="true"
                />
                {/* Track de la barra */}
                <div className="flex-1 h-2 bg-[#334155] rounded-full overflow-hidden">
                    {/* Relleno animado */}
                    <div
                        className="h-full bg-gradient-to-r from-[#f59e0b] to-[#fbbf24] rounded-full transition-all duration-500"
                        style={{ width: `${porcentaje}%` }}
                        role="presentation"
                    />
                </div>
                {/* Contador */}
                <span className="text-xs text-[#64748b] w-4 text-right flex-shrink-0">
                    {count}
                </span>
            </div>
        );
    })}
</div>
```

**Animacion de entrada:** Las barras tienen `transition-all duration-500`. Para que la animacion ocurra al montar, el componente arranca con `width: 0%` en un `useEffect` que setea el porcentaje real despues de la primera render.

```typescript
// Patron de animacion en mount:
const [animado, setAnimado] = useState(false);
useEffect(() => {
    const timer = setTimeout(() => setAnimado(true), 50);
    return () => clearTimeout(timer);
}, []);
// En el style: width: animado ? `${porcentaje}%` : '0%'
```

**Accesibilidad:**
- Las barras son decorativas (`role="presentation"`)
- El conteo numerico `{count}` junto a cada barra provee la informacion en texto accesible
- El contenedor del histograma no necesita rol especial; es informacion visual complementada por texto

---

## 6. Composiciones con shadcn/ui

### 6.1 ValoracionForm

**Descripcion:** Card completo con el formulario para dejar una valoracion. Se muestra cuando el acuerdo esta en estado `Completado` y el usuario aun no ha valorado.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/ValoracionForm.tsx`

**Props Interface:**
```typescript
interface ValoracionFormProps {
    acuerdoId: string;
    onSuccess: (valoracion: ValoracionCreatedResult) => void;
}
```

**Componentes shadcn utilizados:**
- `Textarea` de `@/components/ui/textarea`
- `Label` de `@/components/ui/label`
- `Button` de `@/components/ui/button`

**Iconos Lucide:**
- `Star` - icono decorativo en el header del card
- `Loader2` - spinner durante el submit

**Estados del formulario:**

| Estado | Descripcion |
|--------|-------------|
| `idle` | Formulario limpio, boton disabled (puntuacion === 0) |
| `puntuacion-seleccionada` | Estrellas llenas, boton habilitado, texto "{n} de 5 estrellas" visible |
| `comentario-valido` | Contador en gris `#64748b` o ambar `#f59e0b` (> 900 chars) |
| `comentario-excedido` | Contador rojo `#ef4444`, textarea con `border-[#ef4444]`, boton disabled |
| `submitting` | Spinner en boton, todo disabled, card con `opacity-70` |
| `error-puntuacion` | Mensaje "Selecciona una puntuacion" visible, `role="alert"` |

**Layout:**

```
┌──────────────────────────────────────────────────────┐
│  Deja tu valoracion                     [Star icon]  │
│  Tu comentario ayuda a otros artistas/               │
│  profesionales                                       │
│                                                      │
│  Puntuacion *                                        │
│  [☆] [☆] [☆] [☆] [☆]                               │
│  (visible si > 0) "N de 5 estrellas"                 │
│  (error) "Selecciona una puntuacion"                 │
│                                                      │
│  Comentario (opcional)                               │
│  ┌────────────────────────────────────────────────┐  │
│  │                                                │  │
│  └────────────────────────────────────────────────┘  │
│                                         N / 1000     │
│                                                      │
│                              [Enviar valoracion]     │
└──────────────────────────────────────────────────────┘
```

**Composicion JSX:**
```tsx
<div className={cn(
    "bg-[#0f1729] border border-[#334155] rounded-xl p-6 mt-6",
    isSubmitting && "opacity-70"
)}>
    {/* Header */}
    <div className="flex items-center justify-between mb-2">
        <h3 className="text-lg font-semibold text-white">
            Deja tu valoracion
        </h3>
        <Star
            className="w-5 h-5 text-[#f59e0b]"
            fill="currentColor"
            aria-hidden="true"
        />
    </div>

    {/* Mensaje incentivo */}
    <p className="text-sm text-[#94a3b8] mb-5">
        Tu comentario ayuda a otros artistas/profesionales
    </p>

    <form
        onSubmit={handleSubmit(onSubmit)}
        aria-busy={isSubmitting}
    >
        {/* Campo Puntuacion */}
        <div className="mb-4">
            <Label
                className="text-sm font-medium text-[#cbd5e1] mb-2 block"
                id="puntuacion-label"
            >
                Puntuacion
                <span className="text-red-400 ml-1" aria-hidden="true">*</span>
            </Label>
            <StarRating
                value={puntuacion}
                onChange={setPuntuacion}
                size={isMobile ? 'md' : 'lg'}
                disabled={isSubmitting}
                aria-labelledby="puntuacion-label"
            />
            {/* Texto auxiliar visible solo cuando puntuacion > 0 */}
            {puntuacion > 0 && (
                <p className="text-xs text-[#64748b] mt-1 ml-1">
                    {puntuacion} de 5 estrellas
                </p>
            )}
            {/* Error de validacion */}
            {showPuntuacionError && (
                <p
                    role="alert"
                    aria-live="polite"
                    className="text-sm text-[#ef4444] mt-1"
                    id="puntuacion-error"
                >
                    Selecciona una puntuacion
                </p>
            )}
        </div>

        {/* Campo Comentario */}
        <div className="mt-4">
            <Label
                htmlFor="comentario"
                className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
            >
                Comentario
                <span className="text-[#64748b] font-normal ml-1">(opcional)</span>
            </Label>
            <Textarea
                id="comentario"
                placeholder="Comparte tu experiencia con este profesional..."
                disabled={isSubmitting}
                aria-invalid={comentario.length > 1000}
                aria-describedby="comentario-counter"
                className={cn(
                    "bg-[#16213e] text-white placeholder:text-[#64748b] resize-none min-h-[100px] text-sm leading-relaxed",
                    comentario.length > 1000
                        ? "border-[#ef4444] focus:border-[#ef4444]"
                        : "border-[#334155] focus:border-[#a855f7]"
                )}
                value={comentario}
                onChange={(e) => setComentario(e.target.value)}
            />
            {/* Contador de caracteres */}
            <p
                id="comentario-counter"
                className={cn(
                    "text-xs text-right mt-1",
                    comentario.length > 1000
                        ? "text-[#ef4444]"
                        : comentario.length > 900
                            ? "text-[#f59e0b]"
                            : "text-[#64748b]"
                )}
                aria-live="polite"
                aria-atomic="true"
            >
                {comentario.length} / 1000 caracteres
            </p>
        </div>

        {/* Footer con boton submit */}
        <div className="flex justify-end mt-5">
            <Button
                type="submit"
                disabled={isSubmitting || puntuacion === 0 || comentario.length > 1000}
                aria-disabled={isSubmitting || puntuacion === 0 || comentario.length > 1000}
                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-10 px-6 text-sm font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]"
            >
                {isSubmitting ? (
                    <>
                        <Loader2 className="animate-spin w-4 h-4 mr-2" aria-hidden="true" />
                        Enviando...
                    </>
                ) : (
                    "Enviar valoracion"
                )}
            </Button>
        </div>
    </form>
</div>
```

**Imports necesarios:**
```typescript
import { useState } from 'react';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { Star, Loader2 } from 'lucide-react';
import { cn } from '@/lib/utils';
import { StarRating } from './StarRating';
import type { ValoracionCreatedResult } from '@shared/types/crowdsourcing';
```

**Hook vinculado:** `useCreateValoracion(acuerdoId)` - mutation POST que invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` y muestra toast de exito.

**Responsive:**

| Breakpoint | StarRating size | Layout |
|------------|-----------------|--------|
| Mobile (< 768px) | `md` (w-8 h-8) | Stack vertical, full width, estrellas mas grandes para tap |
| Tablet/Desktop (>= 768px) | `lg` (w-10 h-10) | Igual, footer alineado a la derecha |

---

### 6.2 ValoracionReadOnly

**Descripcion:** Card de solo lectura que se muestra cuando el usuario ya ha valorado el acuerdo. Reemplaza al `ValoracionForm` sin recargar la pagina. Tiene borde verde semitransparente para indicar estado completado.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/ValoracionReadOnly.tsx`

**Props Interface:**
```typescript
interface ValoracionReadOnlyProps {
    valoracion: ValoracionCreatedResult;
    className?: string;
}
```

**Componentes shadcn utilizados:**
- Ninguno de shadcn directamente (usa `StarDisplay` custom)

**Iconos Lucide:**
- `CheckCircle2` - icono de completado en el header

**Animacion de entrada:**
Al reemplazar el formulario por esta vista, aplicar: `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` (via tailwindcss-animate, ya instalado en el proyecto).

**Layout:**

```
┌──────────────────────────────────────────────────────┐
│ (borde verde: border-[#10b981]/40)                    │
│  Tu valoracion                      [CheckCircle2]   │
│                                                      │
│  [★][★][★][★][★]  5 / 5                             │
│                                                      │
│  | "Excelente trabajo, muy profesional y puntual.    │
│  |  Las mezclas quedaron increibles."                │
│                                                      │
│  Enviada el 16 mar 2026                              │
└──────────────────────────────────────────────────────┘
```

**Composicion JSX:**
```tsx
<div className={cn(
    "bg-[#0f1729] border border-[#10b981]/40 rounded-xl p-6 mt-6",
    "animate-in fade-in-0 slide-in-from-bottom-2 duration-200",
    className
)}>
    {/* Header */}
    <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold text-white">
            Tu valoracion
        </h3>
        <CheckCircle2
            className="w-5 h-5 text-[#10b981]"
            aria-hidden="true"
        />
    </div>

    {/* Estrellas con numero */}
    <StarDisplay
        value={valoracion.puntuacion}
        size="lg"
        showNumeric
    />

    {/* Comentario (solo si existe y no es vacio) */}
    {valoracion.comentario && (
        <p className="text-sm text-[#e2e8f0] leading-relaxed mt-3 italic border-l-2 border-[#334155] pl-3">
            &ldquo;{valoracion.comentario}&rdquo;
        </p>
    )}

    {/* Fecha de envio */}
    <p className="text-xs text-[#64748b] mt-3">
        Enviada el {formatFecha(valoracion.fechaCreacion)}
    </p>
</div>
```

**Helper de formato de fecha:**
```typescript
function formatFecha(iso: string): string {
    return new Date(iso).toLocaleDateString('es-ES', {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
    });
}
```

**Imports necesarios:**
```typescript
import { CheckCircle2 } from 'lucide-react';
import { cn } from '@/lib/utils';
import { StarDisplay } from './StarDisplay';
import type { ValoracionCreatedResult } from '@shared/types/crowdsourcing';
```

---

### 6.3 ValoracionesSection

**Descripcion:** Seccion completa de valoraciones para la pagina de perfil de un usuario. Gestiona la carga de datos, paginacion, estados loading/error/empty y renderiza el resumen estadistico, el histograma y la lista paginada.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/ValoracionesSection.tsx`

**Props Interface:**
```typescript
interface ValoracionesSectionProps {
    userId: string;
}
```

**Componentes shadcn utilizados:**
- `Skeleton` de `@/components/ui/skeleton`
- `Alert`, `AlertDescription` de `@/components/ui/alert`
- `Button` de `@/components/ui/button`
- `Pagination`, `PaginationContent`, `PaginationItem` de `@/components/ui/pagination`

**Iconos Lucide:**
- `AlertCircle` - icono de error en el Alert
- `Loader2` - spinner de paginacion
- `ChevronLeft`, `ChevronRight` - controles anterior/siguiente

**Hook vinculado:** `useValoracionesUsuario(userId, { page, pageSize })` que usa `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId)`.

**Layout completo:**

```
┌──────────────────────────────────────────────────────┐
│  VALORACIONES                                        │
│                                                      │
│  ┌──────────────────┐  ┌────────────────────────┐   │
│  │  4.5             │  │  DISTRIBUCION           │   │
│  │  [★★★★☆]        │  │  5 [████████████] 7    │   │
│  │  12 valoraciones │  │  4 [████████    ] 3    │   │
│  └──────────────────┘  │  3 [████        ] 1    │   │
│                        │  2 [████        ] 1    │   │
│                        │  1 [            ] 0    │   │
│                        └────────────────────────┘   │
│                                                      │
│  Valoraciones recientes                             │
│  ┌────────────────────────────────────────────────┐  │
│  │ [ValoracionListItem]                           │  │
│  └────────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────────┐  │
│  │ [ValoracionListItem]                           │  │
│  └────────────────────────────────────────────────┘  │
│                                                      │
│              [<]  [1] [2]  [>]                       │
└──────────────────────────────────────────────────────┘
```

**Estados y su JSX:**

**Estado Loading:**
```tsx
<section className="mt-8">
    <h2 className="text-xl font-semibold text-white mb-5">Valoraciones</h2>
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
        <Skeleton className="h-36 rounded-xl bg-[#1e2a42] animate-pulse" />
        <Skeleton className="h-36 rounded-xl bg-[#1e2a42] animate-pulse" />
    </div>
    <div className="space-y-3">
        <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
        <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
        <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
    </div>
</section>
```

**Estado Error:**
```tsx
<Alert className="border-[#ef4444]/50 bg-[#0f1729] text-white">
    <AlertCircle className="w-4 h-4 text-[#ef4444]" aria-hidden="true" />
    <AlertDescription className="flex items-center justify-between">
        <span className="text-[#94a3b8] text-sm">
            No se pudieron cargar las valoraciones.
        </span>
        <Button
            variant="ghost"
            size="sm"
            className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42] ml-4"
            onClick={() => refetch()}
        >
            Reintentar
        </Button>
    </AlertDescription>
</Alert>
```

**Estado Empty:** Ver componente `EmptyValoraciones` (seccion 6.6).

**Estado Default (con datos):**
```tsx
<section className="mt-8" ref={sectionRef}>
    <h2 className="text-xl font-semibold text-white mb-5">Valoraciones</h2>

    {/* Grid resumen + histograma */}
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
        {/* Card de resumen estadistico */}
        <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-5 flex flex-col items-center justify-center">
            <span className="text-5xl font-bold text-white leading-none">
                {resumen.puntuacionMedia?.toFixed(1)}
            </span>
            <StarDisplay
                value={resumen.puntuacionMedia ?? 0}
                size="md"
                className="my-2"
            />
            <p className="text-sm text-[#94a3b8] text-center">
                {resumen.totalValoraciones} valoraciones
            </p>
        </div>

        {/* Card de histograma */}
        <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-5">
            <p className="text-xs text-[#64748b] uppercase tracking-wider mb-3">
                Distribucion
            </p>
            <RatingHistogram
                distribucion={resumen.distribucion}
                total={resumen.totalValoraciones}
            />
        </div>
    </div>

    {/* Lista de valoraciones recientes */}
    <h3 className="text-base font-semibold text-white mb-4">
        Valoraciones recientes
    </h3>
    <div className="space-y-3">
        {isLoadingPage ? (
            <>
                <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
            </>
        ) : (
            valoraciones.items.map((item) => (
                <ValoracionListItem key={item.id} valoracion={item} />
            ))
        )}
    </div>

    {/* Paginacion */}
    {totalPages > 1 && (
        <nav
            aria-label="Paginacion de valoraciones"
            className="flex items-center justify-center gap-1 mt-6"
        >
            {/* Ver especificacion de Paginacion en seccion 7 */}
        </nav>
    )}
</section>
```

**Logica de paginacion (scroll al cambiar pagina):**
```typescript
const sectionRef = useRef<HTMLElement>(null);

const handlePageChange = (newPage: number) => {
    setPage(newPage);
    sectionRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
};
```

---

### 6.4 ValoracionListItem

**Descripcion:** Card individual para un item de la lista de valoraciones recibidas. Muestra puntuacion, autor con avatar, fecha, contexto del acuerdo y comentario.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/ValoracionListItem.tsx`

**Props Interface:**
```typescript
interface ValoracionListItemProps {
    valoracion: ValoracionListItem;
}
```

**Componentes shadcn utilizados:**
- `Avatar`, `AvatarImage`, `AvatarFallback` de `@/components/ui/avatar`

**Layout:**

```
┌──────────────────────────────────────────────────────────┐
│  [★★★★★]  [Av] Los Rockeros         16 mar 2026         │
│  Mezcla EP Los Rockeros                                  │
│  "Excelente trabajo, muy profesional y puntual."         │
└──────────────────────────────────────────────────────────┘
```

**Composicion JSX:**
```tsx
<div className="bg-[#0f1729] border border-[#334155] rounded-xl p-4 hover:border-[#475569] transition-colors duration-150">
    {/* Fila superior: stars + autor + fecha */}
    <div className="flex items-start justify-between gap-2 mb-2">
        {/* Bloque izquierdo: stars + avatar + nombre */}
        <div className="flex items-center gap-2 flex-wrap">
            <StarDisplay
                value={valoracion.puntuacion}
                size="sm"
            />
            <Avatar className="w-6 h-6 flex-shrink-0">
                <AvatarImage
                    src={valoracion.autorImagenUrl ?? undefined}
                    alt={valoracion.autorNombre}
                />
                <AvatarFallback className="bg-[#334155] text-white text-[10px] font-semibold">
                    {getInitials(valoracion.autorNombre)}
                </AvatarFallback>
            </Avatar>
            <span className="text-sm font-medium text-white">
                {valoracion.autorNombre}
            </span>
        </div>

        {/* Fecha */}
        <span className="text-xs text-[#64748b] flex-shrink-0">
            {formatFecha(valoracion.fechaCreacion)}
        </span>
    </div>

    {/* Contexto del acuerdo */}
    <p className="text-xs text-[#a855f7] font-medium mb-2">
        {valoracion.acuerdoTituloInterno}
    </p>

    {/* Comentario (solo si existe y no es vacio) */}
    {valoracion.comentario && (
        <p className="text-sm text-[#e2e8f0] leading-relaxed italic">
            {valoracion.comentario}
        </p>
    )}
</div>
```

**Helper de iniciales:**
```typescript
function getInitials(nombre: string): string {
    return nombre
        .split(' ')
        .slice(0, 2)
        .map((n) => n[0]?.toUpperCase() ?? '')
        .join('');
}
```

**Responsive - Mobile (< 768px):**
La fecha se coloca debajo del nombre del autor en lugar de en la misma fila:
```tsx
{/* En mobile: flex-col en la fila superior, fecha abajo del nombre */}
<div className="flex items-start justify-between gap-2 mb-2 md:flex-row flex-col">
    <div className="flex items-center gap-2 flex-wrap">
        {/* ... stars + avatar + nombre ... */}
    </div>
    {/* Fecha: en mobile ocupa su propia linea bajo el bloque de autor */}
    <span className="text-xs text-[#64748b] md:flex-shrink-0 mt-1 md:mt-0">
        {formatFecha(valoracion.fechaCreacion)}
    </span>
</div>
```

**Imports necesarios:**
```typescript
import { FC } from 'react';
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar';
import { StarDisplay } from './StarDisplay';
import type { ValoracionListItem as ValoracionListItemType } from '@shared/types/crowdsourcing';
```

---

### 6.5 RatingBadge

**Descripcion:** Badge compacto o medio para mostrar la puntuacion media de un usuario en cards de propuestas, listados de profesionales y cabeceras de perfil. No se renderiza si `totalValoraciones === 0`.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/RatingBadge.tsx`

**Props Interface:**
```typescript
interface RatingBadgeProps {
    puntuacionMedia: number | null;
    totalValoraciones: number;
    variant?: 'compact' | 'medium';   // default: 'compact'
    isLoading?: boolean;               // muestra skeleton de 60px si true
    className?: string;
}
```

**Variante `compact` (para cards y listados):**

```
[★] 4.5  (12)
```

```tsx
{/* variant: 'compact' */}
<div className={cn("flex items-center gap-1", className)}>
    <Star
        className="w-3.5 h-3.5 text-[#f59e0b] flex-shrink-0"
        fill="currentColor"
        aria-hidden="true"
    />
    <span className="text-sm font-semibold text-white">
        {puntuacionMedia?.toFixed(1)}
    </span>
    <span className="text-xs text-[#64748b]">
        ({totalValoraciones})
    </span>
</div>
```

**Variante `medium` (para cabecera de perfil):**

```
[★★★★☆] 4.5  (12 valoraciones)
```

```tsx
{/* variant: 'medium' */}
<div className={cn("flex items-center gap-2", className)}>
    <StarDisplay
        value={puntuacionMedia ?? 0}
        size="sm"
    />
    <span className="text-base font-semibold text-white">
        {puntuacionMedia?.toFixed(1)}
    </span>
    <span className="text-sm text-[#94a3b8]">
        ({totalValoraciones} valoraciones)
    </span>
</div>
```

**Estado Loading (skeleton de 60px):**
```tsx
{isLoading && (
    <Skeleton className="h-5 w-[60px] rounded bg-[#1e2a42] animate-pulse" />
)}
```

**Comportamiento condicional:**
```tsx
// No renderizar si no tiene valoraciones
if (!isLoading && totalValoraciones === 0) return null;

// No renderizar si puntuacionMedia es null (y no esta cargando)
if (!isLoading && puntuacionMedia === null) return null;
```

**Formato de puntuacion:**
Siempre con 1 decimal: `puntuacionMedia?.toFixed(1)` → "4.0", "4.5", "5.0"

**Accesibilidad:**
- El contenedor completo no necesita role especial (la informacion es puramente visual complementaria)
- El `Star` icono tiene `aria-hidden="true"` (decorativo)
- Los valores numericos son texto accesible

**Imports necesarios:**
```typescript
import { FC } from 'react';
import { Star } from 'lucide-react';
import { Skeleton } from '@/components/ui/skeleton';
import { cn } from '@/lib/utils';
import { StarDisplay } from './StarDisplay';
```

---

### 6.6 EmptyValoraciones

**Descripcion:** Estado vacio de la seccion de valoraciones en el perfil. Se muestra cuando `totalValoraciones === 0` y los datos ya han cargado.

**Ruta:** `src/web/src/features/crowdsourcing/valoraciones/components/EmptyValoraciones.tsx`

**Props Interface:**
```typescript
// Sin props - estado vacio siempre tiene el mismo mensaje
interface EmptyValoracionesProps {}
```

**Componentes shadcn:** Ninguno (solo JSX nativo con Tailwind)

**Iconos Lucide:**
- `Star` - icono decorativo grande en gris

**Layout:**

```
┌──────────────────────────────────────────────────────┐
│                                                      │
│              [☆]  (grande, gris)                     │
│   Este usuario aun no tiene valoraciones             │
│   Completa un acuerdo para recibir tu primera        │
│   valoracion                                         │
│                                                      │
└──────────────────────────────────────────────────────┘
```

**Composicion JSX:**
```tsx
<div
    role="status"
    className="bg-[#0f1729] border border-[#334155] rounded-xl p-10 flex flex-col items-center"
>
    <Star
        className="w-10 h-10 text-[#334155] mx-auto mb-3"
        aria-hidden="true"
    />
    <p className="text-[#94a3b8] text-center text-sm">
        Este usuario aun no tiene valoraciones
    </p>
    <p className="text-[#64748b] text-center text-xs mt-1">
        Completa un acuerdo para recibir tu primera valoracion
    </p>
</div>
```

**Imports necesarios:**
```typescript
import { FC } from 'react';
import { Star } from 'lucide-react';
```

---

## 7. Controles de Paginacion

**Descripcion:** Los controles de paginacion de la seccion de valoraciones usan el componente `Pagination` de shadcn/ui con estilos customizados para el dark theme.

**Componentes shadcn:**
- `Pagination`, `PaginationContent`, `PaginationItem` de `@/components/ui/pagination`
- `Button` de `@/components/ui/button` (para los items de pagina)

**Iconos Lucide:**
- `ChevronLeft`, `ChevronRight` - botones anterior y siguiente
- `Loader2` - spinner mientras carga una nueva pagina

**Tabla de elementos:**

| Elemento | Componente | Estilos |
|----------|------------|---------|
| Contenedor nav | `<nav>` | `aria-label="Paginacion de valoraciones"` + `flex items-center justify-center gap-1 mt-6` |
| Boton anterior | `<Button variant="ghost">` | `h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30` |
| Pagina activa | `<Button>` | `h-8 w-8 p-0 text-sm bg-gradient-to-r from-pink-500 to-purple-600 text-white` + `aria-current="page"` |
| Pagina inactiva | `<Button variant="ghost">` | `h-8 w-8 p-0 text-sm text-[#94a3b8] hover:bg-[#16213e] hover:text-white` |
| Boton siguiente | `<Button variant="ghost">` | igual que anterior con ChevronRight |

**Comportamiento por breakpoint:**

| Breakpoint | Paginas visibles |
|------------|-----------------|
| Mobile (< 768px) | Solo: [<] [pagina actual] [>] |
| Desktop (>= 768px) | Hasta 5 numeros de pagina |

**JSX de paginacion:**
```tsx
<nav
    aria-label="Paginacion de valoraciones"
    className="flex items-center justify-center gap-1 mt-6"
>
    {/* Boton anterior */}
    <Button
        variant="ghost"
        size="sm"
        className="h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
        onClick={() => handlePageChange(page - 1)}
        disabled={page <= 1 || isLoadingPage}
        aria-label="Pagina anterior"
    >
        <ChevronLeft className="w-4 h-4" aria-hidden="true" />
    </Button>

    {/* Numeros de pagina (desktop: todos; mobile: solo la actual) */}
    {getPageNumbers(page, totalPages, isMobile).map((pageNum) => (
        <Button
            key={pageNum}
            variant={pageNum === page ? 'default' : 'ghost'}
            size="sm"
            className={cn(
                "h-8 w-8 p-0 text-sm focus-visible:ring-2 focus-visible:ring-[#a855f7]",
                pageNum === page
                    ? "bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-600 hover:to-purple-700"
                    : "text-[#94a3b8] hover:bg-[#16213e] hover:text-white"
            )}
            onClick={() => handlePageChange(pageNum)}
            disabled={isLoadingPage}
            aria-label={pageNum === page ? undefined : `Ir a pagina ${pageNum}`}
            aria-current={pageNum === page ? 'page' : undefined}
        >
            {pageNum}
        </Button>
    ))}

    {/* Boton siguiente */}
    <Button
        variant="ghost"
        size="sm"
        className="h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
        onClick={() => handlePageChange(page + 1)}
        disabled={page >= totalPages || isLoadingPage}
        aria-label="Pagina siguiente"
    >
        <ChevronRight className="w-4 h-4" aria-hidden="true" />
    </Button>
</nav>
```

---

## 8. Integracion en Pantallas Existentes

### 8.1 AcuerdoDetallePage - Seccion de Valoracion

**Condicion de visibilidad:** Solo cuando `acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO`

**Logica de renderizado:**
```tsx
{acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO && (
    <>
        {isLoadingValoracion ? (
            <Skeleton className="h-48 rounded-xl w-full bg-[#1e2a42] animate-pulse" />
        ) : yaValoro && valoracionExistente ? (
            <ValoracionReadOnly valoracion={valoracionExistente} />
        ) : (
            <ValoracionForm
                acuerdoId={acuerdo.id}
                onSuccess={(nueva) => {
                    setValoracionExistente(nueva);
                    setYaValoro(true);
                }}
            />
        )}
    </>
)}
```

**Posicion en la pagina:** Despues del bloque de milestones/entregables, antes o despues del `ConversacionLink` (a decidir en implementacion, pero despues del contenido principal del acuerdo y antes del footer).

**Estado de valoracion del acuerdo:** En MVP, el frontend detecta si ya valoro mediante:
1. La respuesta del POST exitoso (optimistic update: setea `yaValoro = true`)
2. La respuesta 400 con code `4014` en el POST (ya existe: mostrar `ValoracionReadOnly`)
3. Opcionalmente si el endpoint de detalle del acuerdo devuelve `miValoracion` en el futuro

El estado `yaValoro` se mantiene en el estado local del componente padre (`AcuerdoDetallePage`), inicializado en `false`. El formulario `ValoracionForm` llama `onSuccess` con la nueva valoracion. Si el POST retorna 400/4014, el hook `useCreateValoracion` muestra un toast de error y el formulario permanece editable.

### 8.2 PerfilPage - Seccion de Valoraciones

**Ruta hipotetica:** `/crowdsourcing/profesionales/:userId` o `/perfil/:userId`

**Integracion:**
```tsx
{/* En la pagina de perfil, dentro del layout de contenido */}
<ValoracionesSection userId={userId} />
```

**Badge en la cabecera del perfil:**
```tsx
{/* En el bloque de datos del usuario en la cabecera */}
<RatingBadge
    puntuacionMedia={resumenValoraciones?.puntuacionMedia ?? null}
    totalValoraciones={resumenValoraciones?.totalValoraciones ?? 0}
    variant="medium"
    isLoading={isLoadingResumen}
/>
```

### 8.3 PropuestaCard / Cards de Listados

**Integracion del badge compacto:**
```tsx
{/* Dentro de PropuestaCard u otros cards donde se liste un profesional */}
<RatingBadge
    puntuacionMedia={profesional.puntuacionMedia}
    totalValoraciones={profesional.totalValoraciones}
    variant="compact"
    isLoading={isLoadingProfesional}
/>
```

---

## 9. Tabla Completa de Componentes

| Componente | Tipo | Ruta | shadcn/ui usado | Lucide icons |
|------------|------|------|-----------------|--------------|
| `StarRating` | Custom | `valoraciones/components/StarRating.tsx` | Ninguno | `Star` |
| `StarDisplay` | Custom | `valoraciones/components/StarDisplay.tsx` | Ninguno | `Star` |
| `RatingHistogram` | Custom | `valoraciones/components/RatingHistogram.tsx` | Ninguno | `Star` |
| `ValoracionForm` | Composicion | `valoraciones/components/ValoracionForm.tsx` | Textarea, Label, Button | `Star`, `Loader2` |
| `ValoracionReadOnly` | Composicion | `valoraciones/components/ValoracionReadOnly.tsx` | Ninguno | `CheckCircle2` |
| `ValoracionesSection` | Composicion | `valoraciones/components/ValoracionesSection.tsx` | Skeleton, Alert, AlertDescription, Button, Pagination, PaginationContent, PaginationItem | `AlertCircle`, `Loader2`, `ChevronLeft`, `ChevronRight` |
| `ValoracionListItem` | Composicion | `valoraciones/components/ValoracionListItem.tsx` | Avatar, AvatarImage, AvatarFallback | Ninguno |
| `RatingBadge` | Composicion | `valoraciones/components/RatingBadge.tsx` | Skeleton | `Star` |
| `EmptyValoraciones` | Composicion | `valoraciones/components/EmptyValoraciones.tsx` | Ninguno | `Star` |

---

## 10. Estructura de Archivos de la Feature

```
src/web/src/features/crowdsourcing/valoraciones/
├── components/
│   ├── StarRating.tsx              # Custom: selector interactivo de estrellas
│   ├── StarDisplay.tsx             # Custom: display read-only de estrellas
│   ├── RatingHistogram.tsx         # Custom: barras de distribucion
│   ├── ValoracionForm.tsx          # Composicion: formulario completo
│   ├── ValoracionReadOnly.tsx      # Composicion: vista post-envio
│   ├── ValoracionesSection.tsx     # Composicion: seccion completa del perfil
│   ├── ValoracionListItem.tsx      # Composicion: item individual de la lista
│   ├── RatingBadge.tsx             # Composicion: badge reutilizable
│   └── EmptyValoraciones.tsx       # Composicion: estado vacio
├── hooks/
│   ├── useCreateValoracion.ts      # Mutation POST
│   └── useValoracionesUsuario.ts   # Query GET con paginacion
└── index.ts                        # Re-exports de componentes publicos
```

---

## 11. Formularios: Estados Completos

### 11.1 ValoracionForm - Estado de Campos

| Campo | Estado | Visual |
|-------|--------|--------|
| StarRating | Sin seleccion | 5 estrellas en gris `#334155` |
| StarRating | Hover sobre estrella N | Estrellas 1 a N en `#fbbf24` + `scale-110` + `drop-shadow-[0_0_8px_rgba(245,158,11,0.5)]` |
| StarRating | N seleccionadas | Estrellas 1 a N en `#f59e0b`, estrellas N+1 a 5 en gris |
| StarRating | Disabled | Todo el grupo con `opacity-50 cursor-not-allowed` |
| Textarea | Default | `border-[#334155] bg-[#16213e] text-white` |
| Textarea | Focus | `border-[#a855f7]` (ring de focus del navegador) |
| Textarea | Comentario > 1000 | `border-[#ef4444]` + `aria-invalid="true"` |
| Textarea | Disabled | `opacity-50 cursor-not-allowed` (heredado de shadcn Textarea) |
| Contador chars | 0-900 | `text-[#64748b]` |
| Contador chars | 901-1000 | `text-[#f59e0b]` |
| Contador chars | > 1000 | `text-[#ef4444]` |
| Boton submit | Sin puntuacion | `disabled opacity-50 cursor-not-allowed` |
| Boton submit | Valido | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700` |
| Boton submit | Submitting | `disabled` + `<Loader2 animate-spin>` + texto "Enviando..." |
| Card completo | Submitting | `opacity-70` |

### 11.2 Validacion en Tiempo Real

| Evento | Accion |
|--------|--------|
| Click en boton sin puntuacion seleccionada | Mostrar `<p role="alert">Selecciona una puntuacion</p>` con `aria-live="polite"` |
| Typing en textarea hasta 901 chars | Contador cambia a `text-[#f59e0b]` |
| Typing en textarea hasta 1001 chars | Contador cambia a `text-[#ef4444]`, textarea borde rojo, boton disabled |
| Typing en textarea de regreso a < 1000 | Volver a estado valido |
| Submit exitoso | Llamar `onSuccess(valoracion)`, el padre reemplaza ValoracionForm por ValoracionReadOnly |
| Submit con error API (no 4014) | Toast "No se pudo enviar la valoracion. Intenta de nuevo." Formulario permanece editable. |
| Submit con error 4014 | Toast "Ya has enviado una valoracion para este acuerdo." Muestra ValoracionReadOnly. |

---

## 12. Animaciones

| Componente / Elemento | Animacion | Duracion | Implementacion |
|----------------------|-----------|----------|----------------|
| StarRating: hover sobre estrella | Scale up + brillo ambar | 100ms | `transition-transform duration-100 scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)]` |
| StarRating: salida de hover | Scale down | 100ms | `transition-transform duration-100 scale-100` |
| StarRating: click | Brief scale up y vuelta | 150ms | `active:scale-125 transition-transform duration-150` |
| RatingHistogram: relleno de barras | Expand desde izquierda | 500ms | `transition-all duration-500`, iniciando en `width: 0%` via `useEffect` con 50ms delay |
| ValoracionListItem: hover | Cambio de borde | 150ms | `transition-colors duration-150` en el contenedor |
| ValoracionReadOnly: aparicion | Fade + slide desde abajo | 200ms | `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` (tailwindcss-animate) |
| Toast de exito | Fade in desde abajo | 300ms | Manejado por sonner / shadcn toast |
| Skeleton: loading | Pulso | 2s | `animate-pulse` (nativo de shadcn Skeleton) |
| Spinner en boton submit | Rotacion continua | 1s | `animate-spin` (clase Tailwind) |

---

## 13. Responsive Design

### Breakpoints Aplicados

| Breakpoint | Rango | Tailwind prefix |
|------------|-------|-----------------|
| Mobile | < 768px | (default, sin prefijo) |
| Tablet / Desktop | >= 768px | `md:` |
| Desktop large | >= 1024px | `lg:` |

### Cambios por Componente

| Componente | Mobile | Tablet/Desktop |
|------------|--------|----------------|
| **StarRating en ValoracionForm** | `size="md"` (w-8 h-8) para facilitar tap | `size="lg"` (w-10 h-10) |
| **ValoracionesSection: grid de resumen** | `grid-cols-1` (apilado vertical) | `md:grid-cols-2` (lado a lado) |
| **ValoracionListItem: fecha** | Debajo del bloque autor (`flex-col`) | Alineada a la derecha del mismo row (`flex-row`) |
| **Paginacion** | Solo mostrar [<] [pagina actual] [>] | Hasta 5 numeros de pagina visibles |
| **ValoracionForm: boton submit** | Full width (`w-full`) | Alineado a la derecha (`justify-end`) |

### Clases Tailwind de Responsividad Principales

```tsx
// Grid de resumen + histograma
className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6"

// Fila superior de ValoracionListItem (fecha debajo en mobile)
className="flex items-start gap-2 mb-2 flex-col md:flex-row md:justify-between"

// Boton submit en mobile full-width
className="flex justify-end mt-5 md:justify-end"
// Button: "w-full md:w-auto"

// StarRating size segun breakpoint (logica JS, no solo CSS)
const isMobile = useMediaQuery('(max-width: 767px)');
<StarRating size={isMobile ? 'md' : 'lg'} ... />
```

---

## 14. Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de texto principal** | `#ffffff` sobre `#0f1729`: ratio > 10:1 (WCAG AAA) |
| **Contraste de texto secundario** | `#94a3b8` sobre `#0f1729`: ratio ~4.6:1 (WCAG AA) |
| **Contraste de texto muted** | `#64748b` sobre `#0f1729`: ratio ~3.9:1 (adecuado para textos no criticos) |
| **StarRating: radiogroup** | `role="radiogroup"`, `aria-label="Puntuacion de 1 a 5 estrellas"`, `aria-required="true"` |
| **StarRating: cada estrella** | `role="radio"`, `aria-label="{n} estrella{s}"`, `aria-checked={value === n}` |
| **StarRating: navegacion teclado** | Tab entre estrellas, flechas izquierda/derecha cambian valor, Enter/Space seleccionan |
| **StarRating: focus ring** | `focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` |
| **StarDisplay: contenedor** | `role="img"`, `aria-label="Puntuacion: {value} de 5 estrellas"` |
| **StarDisplay: estrellas** | `aria-hidden="true"` en cada `<Star>` individual |
| **Label puntuacion** | `<Label id="puntuacion-label">` + `aria-labelledby` en StarRating |
| **Label comentario** | `<Label htmlFor="comentario">` + `<Textarea id="comentario">` |
| **Error puntuacion** | `<p role="alert" aria-live="polite" id="puntuacion-error">` |
| **Error comentario excedido** | `aria-invalid="true"` en Textarea + `aria-describedby="comentario-counter"` |
| **Contador de caracteres** | `aria-live="polite" aria-atomic="true"` para anuncio en tiempo real |
| **Boton submit durante submitting** | Texto cambia a "Enviando...", `aria-busy="true"` en el `<form>` |
| **Avatar autores** | `<AvatarImage alt="{valoracion.autorNombre}" />`, fallback muestra iniciales como texto real |
| **Histograma: barras** | Barras son decorativas (`role="presentation"`), el conteo numerico provee informacion accesible |
| **Paginacion** | `<nav aria-label="Paginacion de valoraciones">`, pagina activa con `aria-current="page"`, botones con `aria-label` descriptivo |
| **EmptyValoraciones** | `role="status"` en el contenedor |
| **Alert de error** | Usa el componente Alert de shadcn que ya tiene `role="alert"` |
| **Toast de exito** | Manejado por sonner con `role="status"` para lectores de pantalla |
| **Icono estrella decorativo** en headers | `aria-hidden="true"` en todos los iconos decorativos |

---

## 15. Componentes shadcn/ui: Imports Exactos

```typescript
// Componentes de UI ya instalados que se usan en esta feature:

// Button
import { Button } from '@/components/ui/button';

// Card (solo se usa como div custom con clases Tailwind, no el componente Card de shadcn)
// La feature sigue el patron de los componentes existentes que usan <div> con clases

// Textarea
import { Textarea } from '@/components/ui/textarea';

// Label
import { Label } from '@/components/ui/label';

// Avatar
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar';

// Skeleton
import { Skeleton } from '@/components/ui/skeleton';

// Alert
import { Alert, AlertDescription } from '@/components/ui/alert';

// Pagination (referencia, pero los items de pagina se implementan con Button)
import {
    Pagination,
    PaginationContent,
    PaginationItem,
} from '@/components/ui/pagination';

// Badge (no se usa directamente en valoraciones, pero esta disponible)
// import { Badge } from '@/components/ui/badge';
```

**Nota sobre el componente Card de shadcn:** Los componentes existentes del proyecto (`MilestoneCard.tsx`, `PropuestaCard.tsx`, `EnviarPropuestaForm.tsx`) usan tanto `<Card>` de shadcn como `<div>` con clases Tailwind directas. Esta feature usa `<div>` con clases Tailwind siguiendo el patron de los cards inline (no de pagina completa), consistente con `ValoracionForm` y `ValoracionReadOnly` que son secciones dentro de una pagina existente.

---

## 16. Notas de Implementacion

### Orden de Implementacion Recomendado

1. `StarDisplay.tsx` - Sin estado, componente mas simple
2. `StarRating.tsx` - Componente custom critico con estado y accesibilidad
3. `RatingHistogram.tsx` - Con animacion de entrada
4. `RatingBadge.tsx` - Usa StarDisplay, sin estado
5. `EmptyValoraciones.tsx` - Sin props
6. `ValoracionListItem.tsx` - Usa StarDisplay y Avatar
7. `ValoracionReadOnly.tsx` - Usa StarDisplay
8. `ValoracionForm.tsx` - Usa StarRating + hooks
9. `ValoracionesSection.tsx` - Composicion mayor que usa todos los anteriores

### Decisiones de Diseno

1. **StarRating custom vs libreria:** Se implementa como componente custom con Lucide `Star` icon para mantener consistencia visual con el resto del proyecto y evitar dependencias externas. El patron de hover con `useState` es simple y suficiente para el MVP.

2. **StarDisplay con media estrella:** Approach de MVP: la media estrella se representa con `opacity-50` en la estrella fraccionaria. No se usa SVG clip-path. Es visualmente claro y consistente.

3. **Card como div custom:** Los cards de valoracion usan `<div>` con clases Tailwind directas en lugar del componente `<Card>` de shadcn. Esto sigue el patron mayoritario de los componentes existentes del proyecto y da mas control sobre el borde verde del estado read-only.

4. **Paginacion con Button en lugar de PaginationLink:** Los items de pagina numerada usan `<Button>` directamente para obtener los estilos exactos del gradiente pink-purple en el item activo, lo que no es posible con `PaginationLink`.

5. **Form sin shadcn Form component:** El formulario usa `react-hook-form` con `Controller` y `Label` directamente, siguiendo el patron de `EnviarPropuestaForm.tsx`. No se importa el componente `Form` de shadcn.

### Integracion con Toast

El proyecto usa `sonner` o el toast de shadcn (verificar en implementacion). El hook `useCreateValoracion` debe mostrar:
- Exito: `toast.success("Valoracion enviada. Gracias por tu feedback.")`
- Error generico: `toast.error("No se pudo enviar la valoracion. Intenta de nuevo.")`
- Error 4014: `toast.error("Ya has enviado una valoracion para este acuerdo.")`

---

## 17. Checklist UI

### Componentes Custom
- [ ] `StarRating`: 5 botones con fill="currentColor" usando Lucide Star
- [ ] `StarRating`: hover effect: estrellas 1-N en `#fbbf24` + `scale-110` + drop-shadow ambar
- [ ] `StarRating`: click fija puntuacion, estrellas se quedan en `#f59e0b`
- [ ] `StarRating`: disabled con `opacity-50 cursor-not-allowed`
- [ ] `StarRating`: `role="radiogroup"`, cada estrella `role="radio"` con aria-labels
- [ ] `StarRating`: navegacion por teclado (Tab, flechas, Enter, Space)
- [ ] `StarRating`: variantes `sm`/`md`/`lg` con tamanos correctos
- [ ] `StarDisplay`: estrellas llenas en `#f59e0b`, vacias en `#334155`
- [ ] `StarDisplay`: media estrella con `opacity-50`
- [ ] `StarDisplay`: `role="img"` con `aria-label` descriptivo
- [ ] `StarDisplay`: variantes `sm`/`md`/`lg`
- [ ] `RatingHistogram`: 5 filas de 5 a 1, barras proporcionales
- [ ] `RatingHistogram`: barras animadas al montar (0% -> porcentaje real en 500ms)
- [ ] `RatingHistogram`: gradiente ambar `from-[#f59e0b] to-[#fbbf24]`

### ValoracionForm
- [ ] Card con `bg-[#0f1729] border border-[#334155] rounded-xl p-6 mt-6`
- [ ] Header: titulo + icono Star decorativo ambar
- [ ] Mensaje incentivo "Tu comentario ayuda a otros artistas/profesionales"
- [ ] Label de puntuacion con asterisco rojo (`text-red-400`)
- [ ] StarRating `size="lg"` en desktop, `size="md"` en mobile
- [ ] Texto "{N} de 5 estrellas" visible solo cuando puntuacion > 0
- [ ] Error "Selecciona una puntuacion" con `role="alert"` al submit sin puntuacion
- [ ] Label de comentario con "(opcional)" en gris
- [ ] Textarea con contador siempre visible
- [ ] Contador en gris -> ambar (>900) -> rojo (>1000)
- [ ] Textarea con borde rojo cuando comentario > 1000
- [ ] Boton disabled cuando puntuacion === 0 o comentario > 1000
- [ ] Spinner `Loader2` en boton durante submit
- [ ] Card con `opacity-70` durante submit
- [ ] Todo el formulario disabled durante submit
- [ ] `aria-busy="true"` en el `<form>` durante submit

### ValoracionReadOnly
- [ ] Card con `border-[#10b981]/40` (borde verde semitransparente)
- [ ] Titulo "Tu valoracion" + icono CheckCircle2 verde
- [ ] StarDisplay `size="lg"` con `showNumeric`
- [ ] Comentario en italica con borde izquierdo `border-l-2 border-[#334155]`
- [ ] Fecha formateada "Enviada el {dia} {mes} {año}"
- [ ] Animacion de entrada `animate-in fade-in-0 slide-in-from-bottom-2 duration-200`

### ValoracionesSection
- [ ] Skeleton de loading (2 cards de resumen + 3 items de lista)
- [ ] Alert de error con boton "Reintentar"
- [ ] EmptyValoraciones cuando totalValoraciones === 0
- [ ] Grid `grid-cols-1 md:grid-cols-2` para resumen + histograma
- [ ] Card de resumen: puntuacion en `text-5xl font-bold`
- [ ] Card histograma con titulo "DISTRIBUCION" en uppercase
- [ ] Titulo "Valoraciones recientes"
- [ ] Lista con `space-y-3`
- [ ] Paginacion con controles accesibles
- [ ] Scroll suave al inicio de la seccion al cambiar pagina

### ValoracionListItem
- [ ] Hover: `border-[#475569] transition-colors duration-150`
- [ ] StarDisplay `size="sm"`
- [ ] Avatar 6x6 con fallback de iniciales
- [ ] Titulo del acuerdo en `text-[#a855f7]`
- [ ] Comentario omitido si es null o vacio
- [ ] Fecha en mobile debajo del autor (no en la misma linea)

### RatingBadge
- [ ] No se renderiza si `totalValoraciones === 0`
- [ ] Puntuacion siempre con 1 decimal (4.0, 4.5)
- [ ] Skeleton de 60px durante loading
- [ ] Variante `compact`: 1 estrella + numero + total entre parentesis
- [ ] Variante `medium`: StarDisplay sm + numero + "valoraciones"

### EmptyValoraciones
- [ ] Icono Star `w-10 h-10 text-[#334155]`
- [ ] Texto principal "Este usuario aun no tiene valoraciones"
- [ ] Texto secundario "Completa un acuerdo para recibir tu primera valoracion"

### Accesibilidad General
- [ ] Todos los iconos decorativos con `aria-hidden="true"`
- [ ] Focus ring visible (`focus-visible:ring-2 focus-visible:ring-[#a855f7]`) en todos los interactivos
- [ ] Errores de validacion con `role="alert"` y `aria-live="polite"`
- [ ] Contadores con `aria-live="polite" aria-atomic="true"`
- [ ] Paginacion con `aria-label="Paginacion de valoraciones"`
- [ ] Pagina activa con `aria-current="page"`
- [ ] Avatar con alt text del nombre del autor
- [ ] StarRating accesible como radiogroup completo
