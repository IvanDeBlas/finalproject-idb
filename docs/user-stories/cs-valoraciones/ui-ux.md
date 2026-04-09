# UI/UX: Valoraciones Bidireccionales

> **Feature:** cs-valoraciones
> **Ultima actualizacion:** 2026-02-21

---

## Mockups de Referencia

| Pantalla | Archivo | Proyecto |
|----------|---------|----------|
| Artist Profile (referencia layout de perfil con resenas, avatares, estrellas) | [WPR_8-Artist-Profile.png](../../ui-images/WPR_8-Artist-Profile.png) | Landing |
| Detail Campaign (referencia layout de detalle con sidebar y secciones) | [WPR_3-Detail-Campaign.png](../../ui-images/WPR_3-Detail-Campaign.png) | Landing |

**Nota:** No hay mockups especificos para esta feature. El diseno sigue los wireframes textuales definidos en US-CS-06 y los patrones visuales del perfil de artista existente (estrellas de color amarillo/ambar sobre fondo oscuro, cards de resenas con avatar + nombre, layout de seccion con histograma). La pantalla WPR_8 muestra el patron de "Resenas de Fans" que es la base visual directa para la seccion de valoraciones.

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Formulario de valoracion, seccion de valoraciones en perfil, badge en cards de propuestas | `references/templates/krowd/` |

**Nota:** Esta feature pertenece exclusivamente a la Landing publica (`src/web`, Vite + React, puerto 3000), NO al dashboard admin. Se integra dentro de paginas existentes: AcuerdoDetallePage y el perfil de usuario.

---

## Design Tokens

### Paleta de Colores

```css
:root {
  /* Background - Dark theme */
  --bg-primary: #1a1a2e;
  --bg-secondary: #16213e;
  --bg-card: #0f1729;
  --bg-card-hover: #1e2a42;
  --bg-overlay: rgba(0, 0, 0, 0.6);

  /* Primary - Gradient pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);

  /* Text */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;

  /* Status generales */
  --status-success: #10b981;
  --status-error: #ef4444;
  --status-warning: #f59e0b;
  --status-info: #3b82f6;

  /* Valoraciones - Estrellas */
  --star-filled: #f59e0b;           /* Amber/yellow para estrella llena */
  --star-filled-hover: #fbbf24;     /* Mas brillante en hover */
  --star-empty: #334155;            /* Gris oscuro para estrella vacia */
  --star-empty-hover: #475569;      /* Ligeramente mas claro en hover */

  /* Valoraciones - Histograma */
  --histogram-bar-bg: #334155;      /* Fondo del track del histograma */
  --histogram-bar-fill: #f59e0b;    /* Relleno de la barra (ambar) */
  --histogram-bar-fill-gradient: linear-gradient(90deg, #f59e0b 0%, #fbbf24 100%);

  /* Borders */
  --border-primary: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;

  /* Input */
  --input-bg: #16213e;
  --input-bg-disabled: #1e293b;
  --input-border: #334155;
  --input-placeholder: #64748b;

  /* Destructive */
  --destructive: #dc2626;
  --destructive-hover: #b91c1c;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Font Sizes */
  --text-xs: 0.75rem;      /* 12px */
  --text-sm: 0.875rem;     /* 14px */
  --text-base: 1rem;       /* 16px */
  --text-lg: 1.125rem;     /* 18px */
  --text-xl: 1.25rem;      /* 20px */
  --text-2xl: 1.5rem;      /* 24px */
  --text-3xl: 1.875rem;    /* 30px - puntuacion media destacada */

  /* Font Weights */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Line Heights */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
  --leading-relaxed: 1.75;
}
```

### Espaciado

```css
:root {
  --space-1: 0.25rem;   /* 4px */
  --space-2: 0.5rem;    /* 8px */
  --space-3: 0.75rem;   /* 12px */
  --space-4: 1rem;      /* 16px */
  --space-5: 1.25rem;   /* 20px */
  --space-6: 1.5rem;    /* 24px */
  --space-8: 2rem;      /* 32px */
  --space-10: 2.5rem;   /* 40px */
  --space-12: 3rem;     /* 48px */
  --space-16: 4rem;     /* 64px */
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;   /* 6px */
  --radius-md: 0.5rem;     /* 8px */
  --radius-lg: 0.75rem;    /* 12px */
  --radius-xl: 1rem;       /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.3);
  --shadow-star-glow: 0 0 8px rgba(245, 158, 11, 0.5);   /* Brillo ambar al seleccionar estrella */
  --shadow-card-hover: 0 8px 24px rgba(0, 0, 0, 0.4);
}
```

---

## Pantalla 1: Formulario de Valoracion en Detalle de Acuerdo

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/acuerdos/:id` (seccion dentro de la pagina existente)
**Template base:** Krowd - card con formulario inline
**Condicion de visibilidad:** Solo cuando `acuerdo.estado === 'Completado'`

### Layout Desktop

```
┌────────────────────────────────────────────────────────────┐
│  NAVBAR                                                    │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [< Volver]  Mezcla EP Los Rockeros    [COMPLETADO]        │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  ... (contenido existente del acuerdo: milestones,   │  │
│  │       entregables, conversacion, etc.)               │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Deja tu valoracion                          [Star]  │  │
│  │  Tu comentario ayuda a otros artistas/               │  │
│  │  profesionales                                       │  │
│  │                                                      │  │
│  │  Puntuacion *                                        │  │
│  │  [☆] [☆] [☆] [☆] [☆]   (hover: estrellas se        │  │
│  │                          llenan de ambar)            │  │
│  │                                                      │  │
│  │  Comentario (opcional)                               │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │                                              │    │  │
│  │  │                                              │    │  │
│  │  │                                              │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  │  0 / 1000 caracteres                                 │  │
│  │                                                      │  │
│  │                         [Enviar valoracion]          │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

**Estado: Valoracion ya enviada (read-only)**

```
┌────────────────────────────────────────────────────────────┐
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Tu valoracion                            [Check]   │  │
│  │                                                      │  │
│  │  [★] [★] [★] [★] [★]   5 / 5                       │  │
│  │                                                      │  │
│  │  "Excelente trabajo, muy profesional y puntual.      │  │
│  │   Las mezclas quedaron increibles."                  │  │
│  │                                                      │  │
│  │  Enviada el 16 mar 2026                              │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────────┐
│  NAVBAR                                 │
├─────────────────────────────────────────┤
│  [< Volver]                             │
│  Mezcla EP Los Rockeros  [COMPLETADO]   │
│                                         │
│  ... (contenido existente) ...          │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │  Deja tu valoracion     [Star]   │  │
│  │  Tu comentario ayuda a otros     │  │
│  │  artistas/profesionales          │  │
│  │                                   │  │
│  │  Puntuacion *                     │  │
│  │  [☆] [☆] [☆] [☆] [☆]            │  │
│  │                                   │  │
│  │  Comentario (opcional)            │  │
│  │  [________________________]       │  │
│  │  [________________________]       │  │
│  │  [________________________]       │  │
│  │  0 / 1000                         │  │
│  │                                   │  │
│  │  [Enviar valoracion        ]      │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

### Especificaciones

#### Card del Formulario de Valoracion (estado pendiente)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Card contenedor | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-6 mt-6` |
| Encabezado del card | `<div>` | `flex items-center justify-between mb-2` |
| Titulo "Deja tu valoracion" | `<h3>` | `text-lg font-semibold text-white` |
| Icono de estrella decorativo | `<Star>` | Lucide, `w-5 h-5 text-[#f59e0b]` fill="currentColor" |
| Mensaje incentivo | `<p>` | `text-sm text-[#94a3b8] mb-5` "Tu comentario ayuda a otros artistas/profesionales" |
| Label puntuacion | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-2 block` "Puntuacion" con `<span className="text-red-400 ml-1">*</span>` |
| Componente StarRating | `<StarRating>` | `size="lg" value={puntuacion} onChange={setPuntuacion}` - ver especificacion de componente |
| Texto auxiliar puntuacion | `<p>` | `text-xs text-[#64748b] mt-1 ml-1` "{puntuacion} de 5 estrellas" - visible solo cuando puntuacion > 0 |
| Label comentario | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block mt-4` "Comentario" con `<span className="text-[#64748b] font-normal ml-1">(opcional)</span>` |
| Textarea comentario | `<Textarea>` | `bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] resize-none min-h-[100px] focus:border-[#a855f7] text-sm leading-relaxed` placeholder="Comparte tu experiencia con este profesional..." |
| Contador de caracteres | `<p>` | `text-xs text-[#64748b] text-right mt-1` "{n} / 1000 caracteres" - visible siempre |
| Contador en limite (> 900) | `<p>` | `text-xs text-[#f59e0b] text-right mt-1` - ambar como advertencia |
| Contador excedido (> 1000) | `<p>` | `text-xs text-[#ef4444] text-right mt-1` - rojo, textarea con `border-[#ef4444]` |
| Error de validacion puntuacion | `<p>` | `text-sm text-[#ef4444] mt-1` "Selecciona una puntuacion" con `role="alert"` |
| Footer del card | `<div>` | `flex justify-end mt-5` |
| Boton "Enviar valoracion" (activo) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-10 px-6 text-sm font-medium transition-all` |
| Boton "Enviar valoracion" (disabled) | `<Button disabled>` | `opacity-50 cursor-not-allowed` - cuando puntuacion === 0 o comentario > 1000 chars |
| Boton "Enviar valoracion" (submitting) | `<Button disabled>` | con `<Loader2 className="animate-spin w-4 h-4 mr-2" />` "Enviando..." |

#### Card de Valoracion Enviada (read-only)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Card contenedor | `<div>` | `bg-[#0f1729] border border-[#10b981]/40 rounded-xl p-6 mt-6` - borde verde semitransparente para indicar completado |
| Encabezado del card | `<div>` | `flex items-center justify-between mb-4` |
| Titulo "Tu valoracion" | `<h3>` | `text-lg font-semibold text-white` |
| Icono check | `<CheckCircle2>` | Lucide, `w-5 h-5 text-[#10b981]` |
| Componente StarDisplay | `<StarDisplay>` | `value={puntuacion} size="lg" showNumeric` - ver especificacion de componente |
| Texto numerico de puntuacion | `<span>` | `text-sm text-[#94a3b8] ml-2` "{puntuacion} / 5" - dentro de StarDisplay |
| Comentario (si existe) | `<p>` | `text-sm text-[#e2e8f0] leading-relaxed mt-3 italic border-l-2 border-[#334155] pl-3` entre comillas |
| Fecha de envio | `<p>` | `text-xs text-[#64748b] mt-3` "Enviada el {fecha formateada}" |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Acuerdo no completado** | El card completo no se renderiza. No hay ninguna seccion de valoracion visible. |
| **Loading** | Skeleton: `<Skeleton className="h-48 rounded-xl w-full bg-[#1e2a42]" />` con `animate-pulse` |
| **Pendiente (default)** | Card con formulario completo. Estrellas vacias. Boton disabled hasta seleccionar puntuacion. |
| **Puntuacion seleccionada** | Estrellas llenas hasta la puntuacion seleccionada en ambar `#f59e0b`. Boton habilitado. Texto "{n} de 5 estrellas" visible. |
| **Submitting** | Spinner en boton, todos los inputs disabled (puntuacion no clickeable, textarea disabled), opacidad 70% en el card. |
| **Success** | Toast "Valoracion enviada. Gracias por tu feedback." (2000ms). Card cambia inmediatamente al estado read-only con borde verde. |
| **Error API** | Toast error "No se pudo enviar la valoracion. Intenta de nuevo." El formulario permanece editable. |
| **Ya valorado (read-only)** | Card muestra la valoracion existente con borde verde, icono check, StarDisplay, comentario y fecha. Sin controles de edicion. |

### Validacion en Tiempo Real

| Campo | Validacion | Mensaje de Error |
|-------|-----------|-----------------|
| **Puntuacion** | Obligatorio, debe ser 1-5 | "Selecciona una puntuacion" - visible al intentar submit sin seleccionar |
| **Comentario** | Maximo 1000 caracteres | Contador cambia a rojo `#ef4444` y textarea borde rojo. Boton Submit disabled. |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Hover en estrella N | Las estrellas 1 a N se iluminan en ambar `#f59e0b` con efecto `scale-110` en 100ms. Las estrellas N+1 a 5 permanecen en gris. |
| Salida del hover sin click | Las estrellas vuelven al estado de la puntuacion seleccionada (o todas vacias si no hay seleccion). Transicion 100ms. |
| Click en estrella N | Fija la puntuacion en N. Las estrellas 1 a N quedan llenas con leve `shadow-glow` ambar. Texto "{N} de 5 estrellas" aparece debajo. |
| Escribir en textarea | Actualizar contador en tiempo real. A partir de 901 chars el contador cambia a ambar. A partir de 1001 chars cambia a rojo y el boton submit se deshabilita. |
| Click "Enviar valoracion" (valido) | Validar puntuacion > 0 y comentario <= 1000. POST a API. Mostrar spinner. |
| Click "Enviar valoracion" (sin puntuacion) | Mostrar mensaje de error "Selecciona una puntuacion" debajo del componente StarRating. No llamar API. |
| Respuesta exitosa de API | Toast success. Reemplazar card de formulario por card read-only sin recargar la pagina. Invalidar query key de la valoracion del acuerdo. |

---

## Pantalla 2: Seccion de Valoraciones en Perfil de Usuario

**Proyecto:** Landing (`src/web`)
**Ruta:** `/crowdsourcing/profesionales/:userId` o `/perfil/:userId` (seccion dentro de la pagina de perfil existente)
**Template base:** Krowd - seccion con stats y lista de items
**Condicion:** Visible para cualquier usuario autenticado que visite el perfil. Carga los datos de `GET /api/crowdsourcing/usuarios/{userId}/valoraciones`.

### Layout Desktop

```
┌────────────────────────────────────────────────────────────┐
│  NAVBAR                                                    │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [Avatar grande]  Studio Mix Pro                           │
│                   Ingeniero de mezcla                      │
│                   [★★★★☆] 4.5  (12 valoraciones)          │
│                   [Seguir]  [Mensaje]                      │
│                                                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  VALORACIONES                                        │  │
│  │                                                      │  │
│  │  ┌─────────────────┐  ┌───────────────────────────┐  │  │
│  │  │  4.5            │  │ DISTRIBUCION              │  │  │
│  │  │  [★★★★☆]       │  │ 5 [████████████████] 7   │  │  │
│  │  │  12 valoraciones│  │ 4 [████████        ] 3   │  │  │
│  │  └─────────────────┘  │ 3 [████            ] 1   │  │  │
│  │                       │ 2 [████            ] 1   │  │  │
│  │                       │ 1 [                ] 0   │  │  │
│  │                       └───────────────────────────┘  │  │
│  │                                                      │  │
│  │  VALORACIONES RECIENTES                              │  │
│  │                                                      │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │ [★★★★★]  Los Rockeros  [Av]    16 mar 2026  │    │  │
│  │  │ Mezcla EP Los Rockeros                       │    │  │
│  │  │ "Excelente trabajo, muy profesional y        │    │  │
│  │  │  puntual. Las mezclas quedaron increibles."  │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  │                                                      │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │ [★★★★☆]  Indie Band  [Av]      28 feb 2026  │    │  │
│  │  │ Mastering Single                             │    │  │
│  │  │ "Buen trabajo aunque se paso un poco del     │    │  │
│  │  │  plazo."                                     │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  │                                                      │  │
│  │  [< 1 2 > ]  (paginacion)                            │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

**Estado empty:**

```
┌────────────────────────────────────────────────────────────┐
│  ┌──────────────────────────────────────────────────────┐  │
│  │  VALORACIONES                                        │  │
│  │                                                      │  │
│  │             [☆]                                      │  │
│  │   Este usuario aun no tiene valoraciones             │  │
│  │   Completa un acuerdo para recibir tu primera        │  │
│  │   valoracion                                         │  │
│  │                                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

### Layout Mobile (< 768px)

```
┌─────────────────────────────────────────┐
│  NAVBAR                                 │
├─────────────────────────────────────────┤
│  [Avatar]  Studio Mix Pro               │
│            Ingeniero de mezcla          │
│            [★★★★☆] 4.5 (12)            │
│            [Seguir]  [Mensaje]          │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │  VALORACIONES                     │  │
│  │                                   │  │
│  │  4.5   [★★★★☆]                   │  │
│  │  12 valoraciones                  │  │
│  │                                   │  │
│  │  DISTRIBUCION                     │  │
│  │  5 [████████████] 7               │  │
│  │  4 [████████    ] 3               │  │
│  │  3 [████        ] 1               │  │
│  │  2 [████        ] 1               │  │
│  │  1 [            ] 0               │  │
│  │                                   │  │
│  │  VALORACIONES RECIENTES           │  │
│  │                                   │  │
│  │  ┌─────────────────────────────┐  │  │
│  │  │ [★★★★★] Los Rockeros  [Av] │  │  │
│  │  │ 16 mar 2026                 │  │  │
│  │  │ Mezcla EP Los Rockeros      │  │  │
│  │  │ "Excelente trabajo, muy     │  │  │
│  │  │  profesional y puntual..."  │  │  │
│  │  └─────────────────────────────┘  │  │
│  │                                   │  │
│  │  ┌─────────────────────────────┐  │  │
│  │  │ [★★★★☆] Indie Band    [Av] │  │  │
│  │  │ 28 feb 2026                 │  │  │
│  │  │ Mastering Single            │  │  │
│  │  │ "Buen trabajo aunque se..." │  │  │
│  │  └─────────────────────────────┘  │  │
│  │                                   │  │
│  │  [< 1 2 >]                        │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

### Especificaciones

#### Badge de Puntuacion en Cabecera de Perfil

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de badge en cabecera | `<div>` | `flex items-center gap-2 mt-1` |
| Componente StarDisplay compacto | `<StarDisplay>` | `value={puntuacionMedia} size="sm"` |
| Texto puntuacion media | `<span>` | `text-base font-semibold text-white` "{puntuacionMedia}" |
| Texto total valoraciones | `<span>` | `text-sm text-[#94a3b8]` "({totalValoraciones} valoraciones)" |

#### Seccion de Valoraciones

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de seccion | `<section>` | `mt-8` |
| Titulo de seccion | `<h2>` | `text-xl font-semibold text-white mb-5` "Valoraciones" |
| Grid de resumen + histograma (desktop) | `<div>` | `grid grid-cols-1 md:grid-cols-2 gap-4 mb-6` |

#### Resumen Estadistico (lado izquierdo del grid en desktop)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Card resumen | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-5 flex flex-col items-center justify-center` |
| Numero de puntuacion media | `<span>` | `text-5xl font-bold text-white leading-none` "{puntuacionMedia}" |
| Componente StarDisplay | `<StarDisplay>` | `value={puntuacionMedia} size="md" className="my-2"` - permite fracciones (0.5 estrellas) |
| Total de valoraciones | `<p>` | `text-sm text-[#94a3b8] text-center` "{totalValoraciones} valoraciones" |

#### Histograma de Distribucion (lado derecho del grid en desktop, columna unica en mobile)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Card histograma | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-5` |
| Titulo histograma | `<p>` | `text-xs text-[#64748b] uppercase tracking-wider mb-3` "Distribucion" |
| Componente RatingHistogram | `<RatingHistogram>` | `distribucion={distribucion} total={totalValoraciones}` - ver especificacion de componente |

#### Fila de Histograma (interna a RatingHistogram)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de fila | `<div>` | `flex items-center gap-2 mb-2 last:mb-0` |
| Etiqueta del nivel (5, 4, 3...) | `<span>` | `text-xs text-[#94a3b8] w-3 text-right flex-shrink-0` |
| Icono de estrella pequena | `<Star>` | Lucide, `w-3 h-3 text-[#f59e0b] flex-shrink-0` fill="currentColor" |
| Track de la barra | `<div>` | `flex-1 h-2 bg-[#334155] rounded-full overflow-hidden` |
| Relleno de la barra | `<div>` | `h-full bg-gradient-to-r from-[#f59e0b] to-[#fbbf24] rounded-full transition-all duration-500` style={{ width: `${porcentaje}%` }} |
| Contador de ese nivel | `<span>` | `text-xs text-[#64748b] w-4 text-right flex-shrink-0` "{count}" |

#### Lista de Valoraciones Individuales

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Titulo de subseccion | `<h3>` | `text-base font-semibold text-white mb-4` "Valoraciones recientes" |
| Lista | `<div>` | `space-y-3` |
| Componente ValoracionListItem | `<ValoracionListItem>` | `valoracion={item}` - ver especificacion de componente |

#### ValoracionListItem (card individual)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Card item | `<div>` | `bg-[#0f1729] border border-[#334155] rounded-xl p-4 hover:border-[#475569] transition-colors duration-150` |
| Fila superior (stars + autor + fecha) | `<div>` | `flex items-start justify-between gap-2 mb-2` |
| Bloque izquierdo (stars + autor) | `<div>` | `flex items-center gap-2 flex-wrap` |
| StarDisplay del item | `<StarDisplay>` | `value={valoracion.puntuacion} size="sm"` |
| Avatar del autor | `<Avatar>` | `w-6 h-6 flex-shrink-0` |
| AvatarImage del autor | `<AvatarImage>` | `src={valoracion.autorImagenUrl} alt={valoracion.autorNombre}` |
| AvatarFallback | `<AvatarFallback>` | `bg-[#334155] text-white text-[10px] font-semibold` iniciales del autor |
| Nombre del autor | `<span>` | `text-sm font-medium text-white` "{valoracion.autorNombre}" |
| Fecha de la valoracion | `<span>` | `text-xs text-[#64748b] flex-shrink-0` "{fecha relativa: '16 mar 2026'}" |
| Contexto del acuerdo | `<p>` | `text-xs text-[#a855f7] font-medium mb-2` "{valoracion.acuerdoTituloInterno}" |
| Comentario | `<p>` | `text-sm text-[#e2e8f0] leading-relaxed italic` - omitido si comentario es null o vacio |

#### Controles de Paginacion

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor de paginacion | `<div>` | `flex items-center justify-center gap-1 mt-6` |
| Boton anterior | `<Button variant="ghost">` | `h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30` con `<ChevronLeft className="w-4 h-4" />` - disabled en pagina 1 |
| Boton de pagina numerada (activa) | `<Button>` | `h-8 w-8 p-0 text-sm bg-gradient-to-r from-pink-500 to-purple-600 text-white` |
| Boton de pagina numerada (inactiva) | `<Button variant="ghost">` | `h-8 w-8 p-0 text-sm text-[#94a3b8] hover:bg-[#16213e] hover:text-white` |
| Boton siguiente | `<Button variant="ghost">` | igual que anterior con `<ChevronRight className="w-4 h-4" />` - disabled en ultima pagina |

### Estados de UI

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton del resumen: `<Skeleton className="h-36 rounded-xl" />`. Skeleton del histograma: `<Skeleton className="h-36 rounded-xl" />`. Skeleton de 3 items de valoracion: `<Skeleton className="h-28 rounded-xl" />` con `space-y-3`. Todos con `animate-pulse bg-[#1e2a42]`. |
| **Error API** | `<Alert>` con borde `border-[#ef4444]/50`, icono `<AlertCircle className="w-4 h-4 text-[#ef4444]" />`, texto "No se pudieron cargar las valoraciones." y `<Button variant="ghost">` "Reintentar" |
| **Empty** | Card centrado con `<Star className="w-10 h-10 text-[#334155] mx-auto mb-3" />` + `<p className="text-[#94a3b8] text-center text-sm">"Este usuario aun no tiene valoraciones"</p>` + `<p className="text-[#64748b] text-center text-xs mt-1">"Completa un acuerdo para recibir tu primera valoracion"</p>` |
| **Default** | Resumen con puntuacion media, histograma y lista de items. |
| **Cargando mas paginas** | Spinner `<Loader2 className="animate-spin w-4 h-4 text-[#94a3b8]" />` centrado mientras se carga la nueva pagina. Los controles de paginacion permanecen visibles pero disabled. |

### Interacciones

| Accion | Comportamiento |
|--------|----------------|
| Click en boton de pagina numerada | Fetch de la nueva pagina. Scroll suave al inicio de la seccion de valoraciones. Reemplazar lista de items. |
| Click en boton anterior/siguiente | Misma logica que pagina numerada con pagina actual -1 o +1. |
| Hover sobre ValoracionListItem | Borde cambia de `#334155` a `#475569` en 150ms. |

---

## Pantalla 3: Badge de Puntuacion en Cards y Propuestas

**Proyecto:** Landing (`src/web`)
**Ruta:** Componente global reutilizable en propuestas, listados de profesionales, cabeceras de perfil
**Template base:** Krowd - badge inline en cards

### Layout

**Badge compacto (en card de propuesta o listado):**

```
┌────────────────────────────────────────────────────────────┐
│  Studio Mix Pro                                            │
│  Ingeniero de mezcla profesional con 8 anos de...         │
│                                                            │
│  [★] 4.5  (12)     450 EUR      Pendiente                 │
└────────────────────────────────────────────────────────────┘
```

**Badge en cabecera de perfil:**

```
┌────────────────────────────────────────────────────────────┐
│  [Avatar grande]  Studio Mix Pro                           │
│                   Ingeniero de mezcla                      │
│                   [★★★★☆] 4.5  (12 valoraciones)          │
└────────────────────────────────────────────────────────────┘
```

### Especificaciones

#### RatingBadge (compacto - para cards y listados)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor del badge | `<div>` | `flex items-center gap-1` |
| Icono estrella | `<Star>` | Lucide, `w-3.5 h-3.5 text-[#f59e0b]` fill="currentColor" |
| Puntuacion media | `<span>` | `text-sm font-semibold text-white` "{puntuacionMedia}" |
| Total entre parentesis | `<span>` | `text-xs text-[#64748b]` "({totalValoraciones})" |

#### RatingBadge (medio - para cabecera de perfil)

| Elemento | Componente | Estilos/Props |
|----------|------------|---------------|
| Contenedor del badge | `<div>` | `flex items-center gap-2` |
| StarDisplay de media estrella | `<StarDisplay>` | `value={puntuacionMedia} size="sm"` |
| Puntuacion media | `<span>` | `text-base font-semibold text-white` "{puntuacionMedia}" |
| Total entre parentesis | `<span>` | `text-sm text-[#94a3b8]` "({totalValoraciones} valoraciones)" |

### Estados del Badge

| Condicion | Visualizacion |
|-----------|--------------|
| `totalValoraciones === 0` | Badge no se renderiza. No se muestra nada en el espacio destinado al badge. |
| `totalValoraciones >= 1` | Badge completo con icono + puntuacion + total. |
| `puntuacionMedia` es entero (ej: 4.0) | Mostrar "4.0" siempre con un decimal. |
| Usuario sin datos de valoracion (loading) | Skeleton de 60px de ancho con `animate-pulse`. |

---

## Componentes Personalizados Necesarios

Los siguientes componentes deben crearse en `src/web/src/features/valoraciones/`:

| Componente | Descripcion | Props Interface |
|------------|-------------|-----------------|
| `StarRating` | Rating interactivo con hover, click y teclado (1-5). Solo para el formulario de envio. | `{ value: number; onChange: (value: number) => void; size?: 'sm' \| 'md' \| 'lg'; disabled?: boolean; }` |
| `StarDisplay` | Visualizacion read-only de puntuacion, soporta medias estrellas (fraccion 0.5). | `{ value: number; size?: 'sm' \| 'md' \| 'lg'; showNumeric?: boolean; className?: string; }` |
| `RatingHistogram` | Barras de distribucion por nivel de estrella (5 a 1). | `{ distribucion: Record<'1'\|'2'\|'3'\|'4'\|'5', number>; total: number; }` |
| `ValoracionForm` | Card completo con StarRating + textarea + boton de envio. Estado pendiente o submitting. | `{ acuerdoId: string; onSuccess: (valoracion: ValoracionDto) => void; }` |
| `ValoracionReadOnly` | Card con StarDisplay + comentario + fecha. Estado post-envio o ya-valorado. | `{ valoracion: ValoracionDto; }` |
| `ValoracionesSection` | Seccion completa para el perfil: resumen + histograma + lista paginada. | `{ userId: string; }` |
| `ValoracionListItem` | Card individual de una valoracion en la lista. | `{ valoracion: ValoracionListItemDto; }` |
| `RatingBadge` | Badge compacto para cards y listados. | `{ puntuacionMedia: number; totalValoraciones: number; variant?: 'compact' \| 'medium'; }` |
| `EmptyValoraciones` | Estado vacio de la seccion de valoraciones en perfil. | `{}` |

### Especificacion de StarRating (componente critico)

El componente no existe en shadcn/ui. Se implementa con SVG o Lucide `<Star>` icon:

```tsx
// Logica de hover con estado interno
const [hoverValue, setHoverValue] = useState(0);

// Para cada estrella i (1 a 5):
const isActive = hoverValue > 0 ? i <= hoverValue : i <= value;

// Clases CSS segun estado:
// isActive + hover: text-[#fbbf24] scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)]
// isActive sin hover: text-[#f59e0b]
// inactivo: text-[#334155]

// Accesibilidad: el grupo de estrellas es un radiogroup con aria-label="Puntuacion"
// Cada estrella es un radio button con aria-label="N estrellas"
```

### Especificacion de StarDisplay (componente critico)

Soporta fracciones de 0.5 para representar la media:

```tsx
// Para puntuacion media de 4.5:
// Estrellas 1-4: llenas (text-[#f59e0b] fill="currentColor")
// Estrella 5: mitad llena (usar SVG con clip-path para el 50%)
// Alternativa simple MVP: redondear al 0.5 mas cercano y mostrar
// estrellas enteras llenas + media estrella con opacity-50 en el icono

// Size variants:
// sm: w-3.5 h-3.5, gap-0.5
// md: w-4 h-4, gap-1
// lg: w-5 h-5, gap-1
```

---

## Tipos TypeScript de Referencia

```typescript
// DTO para crear una valoracion
interface CreateValoracionDto {
  puntuacion: number;         // 1-5
  comentario?: string;        // max 1000 chars, opcional
}

// DTO de una valoracion creada (respuesta del POST)
interface ValoracionDto {
  id: string;
  puntuacion: number;
  comentario: string | null;
  fechaCreacion: string;      // ISO 8601
}

// DTO para un item de la lista de valoraciones recibidas
interface ValoracionListItemDto {
  id: string;
  puntuacion: number;
  comentario: string | null;
  autorNombre: string;
  autorImagenUrl: string | null;
  acuerdoTituloInterno: string;
  fechaCreacion: string;      // ISO 8601
}

// DTO para el resumen estadistico
interface ResumenValoracionesDto {
  puntuacionMedia: number;    // con 1 decimal, ej: 4.5
  totalValoraciones: number;
  distribucion: {
    '5': number;
    '4': number;
    '3': number;
    '2': number;
    '1': number;
  };
}

// DTO completo de la respuesta del GET de valoraciones de un usuario
interface ValoracionesUsuarioDto {
  resumen: ResumenValoracionesDto;
  valoraciones: {
    items: ValoracionListItemDto[];
    totalCount: number;
    page: number;
    pageSize: number;
  };
}

// Estado de la valoracion para un acuerdo especifico (acuerdoDetalle)
interface ValoracionAcuerdoState {
  yaValoro: boolean;
  valoracionExistente: ValoracionDto | null;
}
```

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios |
|------------|-------|---------|
| **Mobile** | < 768px | Formulario de valoracion: estrellas de tamano `size="md"` (w-8 h-8) para facilitar el tap. Seccion de perfil: resumen y histograma en columna unica. ValoracionListItem: fecha debajo del nombre del autor (no en la misma linea). Paginacion: solo mostrar boton anterior, pagina actual y boton siguiente (no todos los numeros). |
| **Tablet** | 768px - 1024px | Formulario de valoracion: igual que desktop. Seccion de perfil: grid de 2 columnas para resumen + histograma. Lista de items: igual que desktop. |
| **Desktop** | > 1024px | Formulario de valoracion: estrellas de tamano `size="lg"` (w-10 h-10). Seccion de perfil: grid de 2 columnas. Lista: hasta 5 numeros de pagina visibles. |

---

## Animaciones

| Elemento | Animacion | Duracion | Clases Tailwind |
|----------|-----------|----------|-----------------|
| Hover en estrella (StarRating) | Scale up + brillo ambar | 100ms | `transition-transform duration-100 scale-110` + `drop-shadow-[0_0_8px_rgba(245,158,11,0.5)]` |
| Salida de hover en estrella | Scale down | 100ms | `transition-transform duration-100 scale-100` |
| Click en estrella | Breve scale up y vuelta | 150ms | `active:scale-125 transition-transform duration-150` |
| Relleno de barra de histograma | Expand desde izquierda | 500ms | `transition-all duration-500` (aplicado al montar el componente) |
| Hover en ValoracionListItem | Cambio de borde | 150ms | `transition-colors duration-150` |
| Card formulario -> card read-only | Fade + slide | 200ms | `animate-in fade-in-0 slide-in-from-bottom-2 duration-200` |
| Toast de exito | Fade in desde abajo | 300ms | Manejado por shadcn/ui Toast |
| Skeleton loading | Pulso | 2s | `animate-pulse` (parte de shadcn Skeleton) |
| Spinner en boton submit | Rotacion continua | 1s | `animate-spin` |
| Paginacion: cambio de pagina | Sin animacion de transicion | - | Solo reemplazo de lista (no slide ni fade en MVP) |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| **Contraste de texto** | Texto principal `#ffffff` sobre `#0f1729`: ratio > 10:1. Texto secundario `#94a3b8` sobre `#0f1729`: ratio > 4.5:1. Estrellas ambar `#f59e0b` sobre `#0f1729`: decorativas, no requieren contraste minimo de texto. |
| **Focus ring** | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]` en todos los elementos interactivos. |
| **StarRating con teclado** | El grupo de estrellas tiene `role="radiogroup" aria-label="Puntuacion de 1 a 5 estrellas"`. Cada estrella es un boton con `aria-label="{n} estrella{s}" aria-pressed={value === n}`. Navegacion con Tab entre estrellas. Enter o Space para seleccionar. Flechas izquierda/derecha para decrementar/incrementar. |
| **StarDisplay solo lectura** | El contenedor tiene `aria-label="Puntuacion: {value} de 5 estrellas"` y `role="img"`. Las estrellas individuales tienen `aria-hidden="true"`. |
| **Labels en formulario** | `<Label htmlFor="comentario">` asociado con `<Textarea id="comentario">`. El campo de puntuacion usa `role="radiogroup"`. |
| **ARIA en errores de validacion** | Mensajes de error con `role="alert"` y `aria-live="polite"`. Campo con error tiene `aria-invalid="true"` y `aria-describedby="{id-del-mensaje-de-error}"`. |
| **Contraste en barras de histograma** | Las barras son decorativas. El conteo numerico `{n}` junto a cada barra provee la informacion en texto accesible. |
| **Alt text en avatares** | `<AvatarImage alt="{valoracion.autorNombre}" />`. AvatarFallback muestra iniciales como texto real. |
| **Boton submit accesible** | El boton siempre tiene texto visible "Enviar valoracion". Durante submitting el texto cambia a "Enviando..." con `aria-busy="true"` en el formulario. |
| **Toast de exito** | El toast tiene `role="status"` para lectores de pantalla. |
| **Paginacion** | Controles de paginacion con `aria-label="Paginacion de valoraciones"`. Boton activo con `aria-current="page"`. Botones anterior/siguiente con `aria-label="Pagina anterior"` / `aria-label="Pagina siguiente"`. |

---

## Checklist UI/UX

### Pantalla 1: Formulario de Valoracion en Detalle de Acuerdo
- [ ] Card no se renderiza si el acuerdo no esta en estado Completado
- [ ] Titulo "Deja tu valoracion" con icono de estrella decorativo
- [ ] Mensaje incentivo "Tu comentario ayuda a otros artistas/profesionales"
- [ ] StarRating interactivo con 5 estrellas en tamano `lg`
- [ ] Hover sobre estrella N ilumina estrellas 1 a N en ambar
- [ ] Click fija la puntuacion y muestra "{n} de 5 estrellas"
- [ ] Textarea de comentario con label "(opcional)"
- [ ] Contador de caracteres visible siempre: "{n} / 1000"
- [ ] Contador ambar al llegar a 901 chars
- [ ] Contador rojo y boton disabled al superar 1000 chars
- [ ] Boton "Enviar valoracion" disabled si puntuacion === 0
- [ ] Error "Selecciona una puntuacion" al intentar submit sin puntuacion
- [ ] Estado submitting con spinner y todo el formulario disabled
- [ ] Toast "Valoracion enviada. Gracias por tu feedback." al exito
- [ ] Card cambia a modo read-only sin recargar pagina
- [ ] Estado already-rated muestra ValoracionReadOnly con borde verde
- [ ] Skeleton de carga mientras se consulta el estado de valoracion del acuerdo
- [ ] Navegacion por teclado funcional en el StarRating (Tab, flechas, Enter)
- [ ] Responsive: estrellas de tamano adecuado en mobile para tap

### Pantalla 2: Seccion de Valoraciones en Perfil de Usuario
- [ ] Badge de puntuacion media en la cabecera del perfil (si tiene valoraciones)
- [ ] Seccion "Valoraciones" con titulo visible
- [ ] Card de resumen con puntuacion media en texto grande y StarDisplay
- [ ] Total de valoraciones debajo del resumen
- [ ] Histograma de distribucion con barras de 5 a 1
- [ ] Barra del histograma proporcional al porcentaje del total
- [ ] Animacion de relleno de barras al montar el componente (500ms)
- [ ] Titulo "Valoraciones recientes" sobre la lista
- [ ] Lista de ValoracionListItem con: estrellas + avatar + nombre + fecha + contexto del acuerdo + comentario
- [ ] Estado empty con icono y mensaje "Este usuario aun no tiene valoraciones"
- [ ] Estado loading con skeletons
- [ ] Estado error con boton reintentar
- [ ] Controles de paginacion funcionales
- [ ] Scroll suave al inicio de la seccion al cambiar de pagina
- [ ] Responsive: resumen + histograma apilados verticalmente en mobile
- [ ] Responsive: paginacion simplificada en mobile (prev + numero + next)

### Pantalla 3: Badge en Cards y Propuestas
- [ ] RatingBadge compacto: icono estrella + puntuacion + total entre parentesis
- [ ] Badge no se renderiza si totalValoraciones === 0
- [ ] Puntuacion siempre con 1 decimal (4.0, 4.5)
- [ ] Skeleton de 60px mientras cargan los datos
- [ ] Variante compact para cards de propuesta
- [ ] Variante medium para cabecera de perfil
- [ ] Visible en tarjetas de propuestas y listados de profesionales

### Componentes Personalizados
- [ ] StarRating implementado con soporte de teclado (radiogroup)
- [ ] StarDisplay con soporte de medias estrellas (fraccion 0.5)
- [ ] RatingHistogram con barras animadas
- [ ] ValoracionForm encapsula logica de formulario + mutation
- [ ] ValoracionReadOnly para el estado post-envio
- [ ] ValoracionesSection carga datos y gestiona paginacion
- [ ] ValoracionListItem con todos los datos del item
- [ ] RatingBadge con dos variantes (compact y medium)
- [ ] EmptyValoraciones con mensaje correcto

### Accesibilidad General
- [ ] StarRating accesible como radiogroup con navegacion por teclado
- [ ] StarDisplay con aria-label descriptivo y role="img"
- [ ] Focus ring visible en todos los elementos interactivos
- [ ] role="alert" en mensajes de error de validacion
- [ ] aria-busy="true" en el formulario durante submitting
- [ ] Alt text en todos los avatares de autores de valoraciones
- [ ] Controles de paginacion con aria-labels descriptivos
- [ ] Contraste minimo 4.5:1 verificado para textos
