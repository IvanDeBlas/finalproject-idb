# UI/UX: Wallet de Promotor, Comisiones y Cobros

> **Feature:** cp-wallet-comisiones
> **User Story:** US-CP-06
> **Ultima actualizacion:** 2026-03-02

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto y los diagramas ASCII de US-CP-06.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron de KPI cards con valor numerico grande, sidebar oscuro, cards de listas con filas hover |
| Landing principal | [WPR_1-Landing.png](../../ui-images/WPR_1-Landing.png) | Landing | Header publico, paleta de colores dark blue/negro, gradiente pink/purple en botones primarios |

**Observaciones de los mockups aplicables a esta feature:**

Extraido de WPR_5-Dashboard-Artist.png:
- Fondo del dashboard: `#0d0d1a` sidebar, `#1a1a2e` contenido principal
- KPI cards sobre fondo `#151525`, borde `#334155`, padding interno `p-5`
- Valor de KPI destacado: `text-3xl font-bold text-white`, etiqueta inferior: `text-sm text-[#94a3b8]`
- Iconos de KPI en contenedor `w-10 h-10 rounded-lg` con fondo de color tematico segun la metrica
- Filas de lista con `hover:bg-[#1e1e38]` y `transition-colors`
- Boton primario "Nueva campana": gradiente pink-to-purple con texto blanco, `rounded-lg`
- Links de seccion ("Ver todas"): `text-[#a855f7]` sin subrayado, hover con opacidad

Extraido de WPR_1-Landing.png:
- Header publico con fondo `#0d0d1a`, border-bottom sutil
- Botones de accion primaria con `bg-gradient-to-r from-pink-500 to-purple-600`
- Tipografia de numeros destacados usa peso bold, color verde o blanco segun contexto
- El patron de la pagina de wallet del promotor se construye dentro del `DashboardLayout` de la Landing (sidebar + main content), no en PublicLayout

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Krowd** | Paginas del promotor en la Landing (con DashboardLayout) | `references/templates/krowd/` |
| **Dashtail** | Patron de cards, dialogs y formularios con inputs dark-theme | `references/templates/dashtail/` |

**Componentes de referencia clave en el codebase existente:**
- KPI card pattern: `src/web/src/features/crowdpromotion/metricas/presentation/components/MetricasKpiCard.tsx`
- Dialog con formulario: `src/web/src/features/crowdpromotion/tareas/presentation/components/CompletarTareaDialog.tsx`
- Pagina con layout dashboard: `src/web/src/features/crowdpromotion/metricas/presentation/pages/PromotorMetricasPage.tsx`
- Layout: `src/web/src/components/layout/DashboardLayout.tsx` + `Sidebar.tsx`
- Dialog component dark-theme: `src/web/src/components/ui/dialog.tsx` (bg-[#0f1729], border-[#334155])

---

## Design Tokens

### Paleta de Colores

Reutilizados de US-CP-01 a US-CP-05. Se agregan tokens especificos para estados de transacciones y la card de saldo.

```css
:root {
  /* Backgrounds */
  --bg-primary: #0d0d1a;        /* Fondo base (sidebar, header) */
  --bg-secondary: #1a1a2e;      /* Fondo de paginas y contenedores */
  --bg-card: #151525;           /* Fondo de cards */
  --bg-card-hover: #1e1e38;     /* Card hover state */
  --bg-input: #0f0f1f;          /* Fondo de inputs */
  --bg-dialog: #0f1729;         /* Fondo del Dialog (valor actual del componente) */

  /* Colores primarios - Gradiente pink/purple */
  --primary-color: #a855f7;
  --primary-gradient: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --primary-gradient-hover: linear-gradient(135deg, #f472b6 0%, #c084fc 100%);
  --primary-active: #9333ea;

  /* Texto */
  --text-primary: #ffffff;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --text-label: #cbd5e1;
  --text-link: #a855f7;

  /* Wallet - Credito (verde) */
  --wallet-credito: #10b981;          /* Texto importe credito */
  --wallet-credito-bg: rgba(16, 185, 129, 0.1);   /* Fondo icono credito */
  --wallet-credito-border: rgba(16, 185, 129, 0.3);

  /* Wallet - Debito (rojo) */
  --wallet-debito: #ef4444;           /* Texto importe debito */
  --wallet-debito-bg: rgba(239, 68, 68, 0.1);     /* Fondo icono debito */
  --wallet-debito-border: rgba(239, 68, 68, 0.3);

  /* Wallet - Saldo disponible (amarillo/dorado) */
  --wallet-saldo: #f59e0b;            /* Valor del saldo destacado */

  /* Estados de transaccion */
  --estado-pendiente-bg: rgba(245, 158, 11, 0.1);
  --estado-pendiente-text: #f59e0b;
  --estado-pendiente-border: rgba(245, 158, 11, 0.3);

  --estado-procesada-bg: rgba(16, 185, 129, 0.1);
  --estado-procesada-text: #10b981;
  --estado-procesada-border: rgba(16, 185, 129, 0.3);

  --estado-pagada-bg: rgba(59, 130, 246, 0.1);
  --estado-pagada-text: #3b82f6;
  --estado-pagada-border: rgba(59, 130, 246, 0.3);

  --estado-cancelada-bg: rgba(100, 116, 139, 0.1);
  --estado-cancelada-text: #64748b;
  --estado-cancelada-border: rgba(100, 116, 139, 0.3);

  /* Estado general */
  --status-success: #10b981;
  --status-warning: #f59e0b;
  --status-error: #ef4444;
  --status-info: #3b82f6;

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - fechas, hints, etiquetas auxiliares */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario, items de lista */
  --text-base: 1rem;     /* 16px - texto de inputs, concepto de transaccion */
  --text-lg: 1.125rem;   /* 18px - subtitulos de seccion */
  --text-xl: 1.25rem;    /* 20px - titulos de card */
  --text-2xl: 1.5rem;    /* 24px - titulo de pagina */
  --text-3xl: 1.875rem;  /* 30px - valor KPI de saldo secundario */
  --text-4xl: 2.25rem;   /* 36px - saldo disponible hero */

  /* Pesos */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Interlineado */
  --leading-tight: 1.25;
  --leading-normal: 1.5;
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
}
```

### Bordes y Sombras

```css
:root {
  --radius-sm: 0.375rem;  /* 6px */
  --radius-md: 0.5rem;    /* 8px */
  --radius-lg: 0.75rem;   /* 12px */
  --radius-xl: 1rem;      /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.35);
  --shadow-glow-green: 0 0 16px rgba(16, 185, 129, 0.2);  /* Saldo disponible */
}
```

---

## Pantalla 1: Mi Wallet

**Proyecto:** Landing (Vite + React 18)
**Ruta:** `/promotor/wallet`
**Layout:** `DashboardLayout` (sidebar + main, require autenticacion)
**Template base:** Patron de PromotorMetricasPage con adaptaciones para wallet

**Precondicion:** Usuario autenticado con perfil de promotor. Si no tiene perfil de promotor, redirigir a `/promotor/registro`.

**API calls al montar:**
- `GET /api/crowdpromotion/promotor/wallet` - datos de saldo
- `GET /api/crowdpromotion/promotor/wallet/transacciones?page=1&pageSize=10` - historial inicial

### Layout

```
┌──────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a], border-r border-[#334155], fixed)   │
│ [Logo WePlay Rises]                                              │
│ ─────                                                            │
│ Mis programas   /promotor/mis-programas                          │
│ Mis tareas      /promotor/tareas                                 │
│ Metricas        /promotor/metricas                               │
│ Mi Wallet  [•]  /promotor/wallet        (activo, resaltado)      │
│ ─────                                                            │
│ [Avatar usuario] Nombre promotor                                 │
├──────────────────────────────────────────────────────────────────┤
│ MAIN CONTENT (ml-64 sm:ml-0, bg-[#1a1a2e], min-h-screen)        │
│                                                                  │
│  CABECERA (max-w-3xl mx-auto px-4 sm:px-6, pt-8 pb-4)           │
│  Mi Wallet                                                       │
│                                                                  │
│  SALDO CARD (max-w-3xl mx-auto px-4 sm:px-6, mb-6)              │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │  SALDO DISPONIBLE                                        │    │
│  │                                                          │    │
│  │            150.50 EUR                                    │    │
│  │                                                          │    │
│  │  ┌───────────────────┐  ┌───────────────────┐           │    │
│  │  │  Total Ganado     │  │  Total Retirado   │           │    │
│  │  │  200.00 EUR       │  │   49.50 EUR       │           │    │
│  │  └───────────────────┘  └───────────────────┘           │    │
│  │                                                          │    │
│  │  [Solicitar cobro]       Minimo: 10.00 EUR               │    │
│  └──────────────────────────────────────────────────────────┘    │
│                                                                  │
│  FILTROS (max-w-3xl mx-auto px-4 sm:px-6, mb-4)                  │
│  Historial de transacciones                                      │
│  [Tipo v]  [Estado v]  [Desde____]  [Hasta____]  [Limpiar]      │
│                                                                  │
│  LISTA TRANSACCIONES (max-w-3xl mx-auto px-4 sm:px-6)           │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │ [ArrowUp]  + 10.00 EUR   Comision por backing referido   │    │
│  │            Mi Album Debut             [PROCESADA]        │    │
│  │            20 mar 2026                                   │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │ [ArrowUp]  + 5.00 EUR    Recompensa: Comparte en IG      │    │
│  │                                        [PENDIENTE]       │    │
│  │            19 mar 2026                                   │    │
│  ├──────────────────────────────────────────────────────────┤    │
│  │ [ArrowDown] - 49.50 EUR  Solicitud de cobro              │    │
│  │                                          [PAGADA]        │    │
│  │            15 mar 2026                                   │    │
│  └──────────────────────────────────────────────────────────┘    │
│                                                                  │
│  PAGINACION (max-w-3xl mx-auto px-4 sm:px-6, py-4)              │
│  < [1] [2] [3] >                                                 │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Componente WalletSaldoCard

La card de saldo es el elemento visual mas prominente de la pantalla. Ocupa el ancho completo del contenedor y destaca el saldo disponible como el dato principal.

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedora | `<Card>` | `bg-[#151525] border-[#334155] p-6 mb-6 relative overflow-hidden` |
| Efecto de fondo | `<div>` | `absolute inset-0 bg-gradient-to-br from-green-950/20 via-transparent to-transparent pointer-events-none` |
| Label "Saldo disponible" | `<p>` | `text-sm font-medium text-[#94a3b8] mb-1 uppercase tracking-wide` |
| Valor principal | `<p>` | `text-4xl font-bold text-[#f59e0b] tabular-nums mb-1` |
| Sufijo moneda en valor | `<span>` | `text-2xl text-[#64748b] ml-2` / "EUR" |
| Grid de stats secundarias | `<div>` | `grid grid-cols-2 gap-4 my-5 pt-4 border-t border-[#334155]` |
| Stat "Total Ganado" label | `<p>` | `text-xs text-[#64748b] mb-1` / "Total Ganado" |
| Stat "Total Ganado" valor | `<p>` | `text-lg font-semibold text-[#10b981] tabular-nums` |
| Stat "Total Ganado" moneda | `<span>` | `text-sm text-[#64748b] ml-1` / "EUR" |
| Stat "Total Retirado" label | `<p>` | `text-xs text-[#64748b] mb-1` / "Total Retirado" |
| Stat "Total Retirado" valor | `<p>` | `text-lg font-semibold text-[#94a3b8] tabular-nums` |
| Stat "Total Retirado" moneda | `<span>` | `text-sm text-[#64748b] ml-1` / "EUR" |
| Fila accion | `<div>` | `flex items-center justify-between flex-wrap gap-3 mt-4` |
| Boton "Solicitar cobro" (habilitado) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-400 hover:to-purple-500 transition-all duration-150 font-medium` |
| Boton "Solicitar cobro" (deshabilitado) | `<Button disabled>` | `opacity-50 cursor-not-allowed bg-[#1e1e38] text-[#64748b] border border-[#334155]` - no aplicar el gradiente |
| Texto minimo de retiro | `<p>` | `text-xs text-[#64748b]` / "Minimo: 10.00 EUR" |
| Aviso saldo insuficiente | `<div>` (visible solo si saldo < minimo) | `flex items-center gap-1.5 text-xs text-[#f59e0b]` con icono `<Info w-3.5 h-3.5>` y texto "Necesitas al menos 10.00 EUR para solicitar un cobro" |

**Condicion del boton "Solicitar cobro":**
- Habilitado: `saldoDisponible >= minimoRetiro` (10.00 EUR por defecto)
- Deshabilitado: `saldoDisponible < minimoRetiro` o `wallet.data === undefined` (loading)
- El boton deshabilitado NO tiene interaccion; no debe mostrar tooltip para no confundir

**Skeleton de WalletSaldoCard (estado loading):**

```
bg-[#151525] border-[#334155] rounded-lg p-6 mb-6
  Skeleton w-32 h-4 mb-3 bg-[#1e1e38]            <- label
  Skeleton w-48 h-10 mb-4 bg-[#1e1e38]           <- valor principal
  border-t border-[#334155] pt-4
  grid grid-cols-2 gap-4
    Skeleton w-20 h-4 mb-1 bg-[#1e1e38]          <- label Total Ganado
    Skeleton w-28 h-6 bg-[#1e1e38]               <- valor Total Ganado
    Skeleton w-20 h-4 mb-1 bg-[#1e1e38]          <- label Total Retirado
    Skeleton w-28 h-6 bg-[#1e1e38]               <- valor Total Retirado
  mt-4
  Skeleton w-36 h-9 rounded-lg bg-[#1e1e38]      <- boton
```

Todos los skeletons tienen `animate-pulse`.

### Componente FiltrosHistorial

Barra de filtros sobre la lista de transacciones. Los filtros son inline y siempre visibles (no colapsables).

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Cabecera seccion | `<h2>` | `text-base font-semibold text-white mb-4` / "Historial de transacciones" |
| Contenedor filtros | `<div>` | `flex flex-wrap items-center gap-3 mb-4` |
| Select "Tipo" | `<Select>` | `w-36 bg-[#0f0f1f] border-[#334155] text-sm text-white focus:border-[#a855f7]` |
| Opciones de Tipo | `<SelectItem>` | "Todos", "Creditos", "Debitos" |
| Select "Estado" | `<Select>` | `w-40 bg-[#0f0f1f] border-[#334155] text-sm text-white focus:border-[#a855f7]` |
| Opciones de Estado | `<SelectItem>` | "Todos los estados", "Pendiente", "Procesada", "Pagada", "Cancelada" |
| Input fecha desde | `<Input type="date">` | `w-36 bg-[#0f0f1f] border-[#334155] text-white text-sm [color-scheme:dark]` |
| Input fecha hasta | `<Input type="date">` | Mismos estilos que fecha desde |
| Boton "Limpiar filtros" | `<Button variant="ghost" size="sm">` | `text-[#64748b] hover:text-[#94a3b8] text-xs` con icono `<X w-3.5 h-3.5 mr-1>` - solo visible cuando hay algun filtro activo |

**Comportamiento de los filtros:**
- Los filtros se aplican inmediatamente al cambiar cualquier `<Select>` (no hay boton "Aplicar" para tipo/estado)
- Las fechas se aplican al perder el foco en el input (onBlur) o al cambiar valor, con debounce de 500ms
- Al aplicar cualquier filtro, la pagina de paginacion vuelve a la 1
- El boton "Limpiar filtros" aparece cuando al menos uno de los selectores no esta en su valor por defecto ("Todos") o hay alguna fecha ingresada
- Al limpiar, todos los filtros vuelven a sus valores por defecto y la paginacion a la pagina 1

### Componente TransaccionItem

Cada fila de transaccion en el historial. Debe comunicar claramente si es credito o debito y su estado actual.

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor fila | `<div>` | `flex items-start gap-3 px-5 py-4 hover:bg-[#1e1e38] transition-colors duration-150 cursor-default` |
| Icono credito | `<div>` | `w-8 h-8 rounded-full flex items-center justify-center shrink-0 bg-green-950/50 border border-green-800/30 mt-0.5` con `<ArrowUp w-4 h-4 text-[#10b981]>` |
| Icono debito | `<div>` | `w-8 h-8 rounded-full flex items-center justify-center shrink-0 bg-red-950/50 border border-red-800/30 mt-0.5` con `<ArrowDown w-4 h-4 text-[#ef4444]>` |
| Columna central (flex-1) | `<div>` | `flex-1 min-w-0` |
| Texto concepto | `<p>` | `text-sm font-medium text-white truncate` |
| Texto contexto (tipo reward o programa) | `<p>` | `text-xs text-[#64748b] mt-0.5 truncate` - solo si hay informacion disponible (tipoRewardNombre o referencia al programa) |
| Fecha | `<p>` | `text-xs text-[#64748b] mt-1` / formato "20 mar 2026" |
| Columna derecha | `<div>` | `flex flex-col items-end gap-1.5 shrink-0` |
| Importe credito | `<p>` | `text-sm font-semibold text-[#10b981] tabular-nums` / "+ 10.00 EUR" |
| Importe debito | `<p>` | `text-sm font-semibold text-[#ef4444] tabular-nums` / "- 49.50 EUR" |
| Badge de estado | `<Badge>` | Ver tabla de badges de estado mas abajo |

**Badges por estado de transaccion:**

| Estado | ID | Componente | Estilos |
|--------|----|------------|---------|
| Pendiente | 1 | `<Badge>` | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs font-medium` |
| Procesada | 2 | `<Badge>` | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs font-medium` |
| Pagada | 3 | `<Badge>` | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs font-medium` |
| Cancelada | 4 | `<Badge>` | `bg-slate-900/50 text-[#64748b] border border-slate-700/50 text-xs font-medium` |

**Formato del importe:**
- Credito: prefijo `+` en verde `text-[#10b981]`
- Debito: prefijo `-` en rojo `text-[#ef4444]`
- Siempre 2 decimales: `importe.toFixed(2)`
- Sufijo de moneda: `" EUR"` al final del importe (o el `monedaNombre` del DTO si en el futuro hay multidivisa)
- Numeros con separador de miles usando `toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 })`

**Formato de fecha:**
- Usar `new Date(fechaCreacion).toLocaleDateString('es-ES', { day: 'numeric', month: 'short', year: 'numeric' })`
- Resultado esperado: "20 mar 2026"

### Componente PaginacionWallet

Paginacion simple en la parte inferior de la lista.

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor | `<div>` | `flex items-center justify-between py-4 border-t border-[#334155]` |
| Texto informativo | `<p>` | `text-xs text-[#64748b]` / "Mostrando 1-10 de 25 transacciones" |
| Controles de pagina | `<Pagination>` (shadcn) | Con `<PaginationPrevious>`, `<PaginationItem>`, `<PaginationNext>` |
| Boton pagina activa | `<PaginationLink isActive>` | `bg-[#a855f7] text-white border-[#a855f7]` |
| Boton pagina inactiva | `<PaginationLink>` | `bg-transparent text-[#94a3b8] border-[#334155] hover:bg-[#1e1e38] hover:text-white` |
| Boton anterior/siguiente | `<PaginationPrevious>` / `<PaginationNext>` | Mismos estilos que pagina inactiva; deshabilitado con `opacity-40 pointer-events-none` en los extremos |

**Reglas de paginacion:**
- Mostrar maximo 5 numeros de pagina a la vez; si hay mas, mostrar ellipsis (`<PaginationEllipsis>`)
- Default: `pageSize = 10` transacciones por pagina
- Al cambiar filtros, siempre volver a pagina 1
- El componente `<Pagination>` de shadcn esta disponible en `src/web/src/components/ui/pagination.tsx`

### Componente WalletEmptyState

Estado vacio cuando el promotor no tiene ninguna transaccion en la wallet.

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor centrado | `<div>` | `flex flex-col items-center justify-center py-20 text-center` |
| Icono contenedor | `<div>` | `w-16 h-16 rounded-full flex items-center justify-center mb-4 bg-[#151525] border border-[#334155]` |
| Icono wallet | `<Wallet w-7 h-7>` | `text-[#64748b]` |
| Titulo | `<h3>` | `text-base font-semibold text-white mb-2` / "Aun no tienes transacciones" |
| Subtitulo | `<p>` | `text-sm text-[#64748b] max-w-sm` / "Completa tareas de promocion o genera backings referidos para ganar comisiones." |
| Boton CTA | `<Button>` | `mt-6 bg-gradient-to-r from-pink-500 to-purple-600 text-white` / "Explorar programas" |
| Accion del boton CTA | Navegar a | `/crowdpromotion/explorar` |

**Variante de empty state para filtros sin resultados** (cuando hay transacciones pero los filtros activos no arrojan resultados):

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor centrado | `<div>` | `flex flex-col items-center justify-center py-12 text-center` |
| Icono | `<SearchX w-8 h-8>` | `text-[#64748b] mb-3` |
| Texto | `<p>` | `text-sm text-[#64748b]` / "No hay transacciones que coincidan con los filtros seleccionados." |
| Boton limpiar filtros | `<Button variant="ghost" size="sm">` | `text-[#a855f7] hover:text-[#c084fc] mt-2` / "Limpiar filtros" |

### Componente TransaccionesListSkeleton

Esqueleto de carga para la lista completa de transacciones.

```
Card bg-[#151525] border-[#334155] rounded-lg
  divide-y divide-[#334155]
  [Repetir 5 veces]:
    flex items-start gap-3 px-5 py-4
      Skeleton w-8 h-8 rounded-full bg-[#1e1e38] shrink-0
      flex-1
        Skeleton w-3/4 h-4 mb-2 bg-[#1e1e38]
        Skeleton w-1/2 h-3 bg-[#1e1e38]
      flex flex-col items-end gap-2
        Skeleton w-20 h-4 bg-[#1e1e38]
        Skeleton w-16 h-5 rounded-full bg-[#1e1e38]
```

Todos los skeletons tienen `animate-pulse`.

### Estados de UI de la Pantalla 1

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | `WalletSaldoCard` en skeleton (sin datos). `FiltrosHistorial` con selects visibles pero deshabilitados (o visibles aunque no funcionales). `TransaccionesListSkeleton` con 5 filas. El titulo de pagina se muestra siempre. `aria-busy="true"` en el contenedor principal mientras carga |
| **Default (con datos)** | `WalletSaldoCard` con valores reales. Filtros activos. Lista de transacciones ordenadas por fecha descendente. Paginacion visible si hay mas de 10 transacciones |
| **Empty state (sin transacciones)** | `WalletSaldoCard` con saldo 0.00, Total Ganado 0.00, Total Retirado 0.00. Boton "Solicitar cobro" deshabilitado. Ocultar barra de filtros. Mostrar `WalletEmptyState` en lugar de la lista |
| **Sin resultados con filtros** | `WalletSaldoCard` con datos reales. Filtros visibles con sus valores activos. Mostrar empty state de filtros sin resultados |
| **Error de carga (wallet)** | Card centrada con `<AlertCircle w-8 h-8 text-red-400>`, "No se pudo cargar tu wallet", boton "Reintentar" `<Button variant="outline" size="sm">`. Usar `role="alert"` |
| **Error de carga (transacciones)** | `WalletSaldoCard` se muestra normalmente con los datos de saldo. Dentro del area de lista: card de error con icono y "No se pudieron cargar las transacciones", boton "Reintentar". Los datos del saldo no se pierden |
| **Saldo insuficiente** | Boton deshabilitado. Aviso inline en amarillo bajo el boton con `<Info w-3.5 h-3.5>` y texto descriptivo del minimo requerido |
| **Post solicitud de cobro** | Actualizar `WalletSaldoCard` con el nuevo saldo (sin recargar pagina completa). Agregar la nueva transaccion de debito al inicio de la lista (invalidar y refetch de la query de transacciones). Mostrar toast de exito |

### Validacion en Tiempo Real de Filtros

| Campo | Validacion | Comportamiento |
|-------|-----------|----------------|
| Fecha desde | No puede ser mayor que fecha hasta | Si fechaDesde > fechaHasta, mostrar borde rojo en el input con error y no aplicar el filtro de fechas |
| Fecha hasta | No puede ser mayor que la fecha de hoy | Limitar `max` del input a la fecha actual |
| Combinacion de fechas | Rango de hasta 365 dias | Aviso informativo no bloqueante si el rango supera 1 anio |

### Interacciones de la Pantalla 1

| Accion | Comportamiento |
|--------|----------------|
| Click en "Solicitar cobro" | Abrir `SolicitarCobroDialog` con el saldo actual pre-cargado como informacion |
| Click en boton deshabilitado "Solicitar cobro" | Sin respuesta visual ni tooltip (el aviso de saldo insuficiente ya es visible) |
| Cambiar Select "Tipo" | Refetch inmediato de transacciones con `esCredito` filtrado; volver a pagina 1 |
| Cambiar Select "Estado" | Refetch inmediato de transacciones con `estadoId` filtrado; volver a pagina 1 |
| Cambiar fecha desde/hasta | Debounce 500ms, luego refetch si el rango es valido; volver a pagina 1 |
| Click en paginacion | Refetch con el nuevo numero de pagina, mantener filtros activos |
| Click "Limpiar filtros" | Resetear todos los selects y fechas a valor por defecto; refetch sin filtros; volver a pagina 1 |
| Hover sobre fila de transaccion | `bg-[#1e1e38]` con `transition-colors duration-150` |
| Scroll vertical | La pagina hace scroll completo. El sidebar permanece fijo |

---

## Pantalla 2: Dialog Solicitar Cobro

**Proyecto:** Landing (Vite + React 18)
**Tipo:** Modal `<Dialog>` de shadcn/ui, montado dentro de la Pantalla 1
**Trigger:** Click en boton "Solicitar cobro" en `WalletSaldoCard`

El dialog se abre desde `PromotorWalletPage` y recibe como props el saldo actual y el minimo de retiro. No hace fetch propio; los datos de saldo ya estan en el componente padre.

### Layout del Dialog

```
+--------------------------------------------------+
|  Solicitar cobro                          [X]    |
+--------------------------------------------------+
|                                                  |
|  Tu saldo disponible                             |
|  ┌──────────────────────────────────────┐        |
|  │  150.50 EUR                          │        |
|  └──────────────────────────────────────┘        |
|  Minimo de retiro: 10.00 EUR                     |
|                                                  |
|  Importe a retirar *                             |
|  [50.00________________] EUR                     |
|  Hasta 150.50 EUR disponibles                   |
|                                                  |
|  Descripcion (opcional)                          |
|  [Retiro mensual___________________________]     |
|  [___________________________________________]   |
|  0 / 500 caracteres                              |
|                                                  |
|  [Cancelar]              [Solicitar cobro]       |
+--------------------------------------------------+
```

### Especificaciones del Dialog

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialog raiz | `<Dialog open={open} onOpenChange={onOpenChange}>` | Controlado por el padre |
| Content | `<DialogContent>` | `max-w-md` - usa los estilos por defecto del componente: `bg-[#0f1729] border-[#334155] rounded-lg` |
| Titulo | `<DialogTitle>` | `text-lg font-semibold text-white` / "Solicitar cobro" |
| Subtitulo | `<DialogDescription>` | No se usa; la informacion se muestra con elementos internos |
| Seccion informativa de saldo | `<div>` | `bg-[#151525] border border-[#334155] rounded-lg p-4 mb-5` |
| Label "Tu saldo disponible" | `<p>` | `text-xs text-[#64748b] mb-1` |
| Valor saldo disponible | `<p>` | `text-2xl font-bold text-[#f59e0b] tabular-nums` / "150.50 EUR" |
| Texto minimo | `<p>` | `text-xs text-[#64748b] mt-2` / "Minimo de retiro: 10.00 EUR" |
| Separador | `<Separator>` | `bg-[#334155] my-4` |
| Label campo importe | `<Label htmlFor="importe">` | `text-sm font-medium text-[#cbd5e1]` / "Importe a retirar" con `<span className="text-red-400 ml-1">*</span>` |
| Input importe | `<Input id="importe" type="number">` | `bg-[#0f0f1f] border-[#334155] text-white focus:border-[#a855f7] pr-14` - el sufijo "EUR" se posiciona absolutamente a la derecha del input |
| Sufijo "EUR" del input | `<span>` | `absolute right-3 top-1/2 -translate-y-1/2 text-sm text-[#64748b] pointer-events-none` |
| Hint bajo el input | `<p>` | `text-xs text-[#64748b] mt-1` / "Hasta {saldoDisponible.toFixed(2)} EUR disponibles" |
| Error del input importe | `<p role="alert">` | `text-xs text-[#ef4444] mt-1 flex items-center gap-1` con `<AlertCircle w-3 h-3>` |
| Label campo descripcion | `<Label htmlFor="descripcion">` | `text-sm font-medium text-[#cbd5e1] mt-4 block` / "Descripcion (opcional)" |
| Textarea descripcion | `<Textarea id="descripcion">` | `bg-[#0f0f1f] border-[#334155] text-white resize-none h-20 focus:border-[#a855f7] placeholder:text-[#64748b]` |
| Counter descripcion | `<p>` | `text-xs text-[#64748b] text-right mt-1` / "{n} / 500 caracteres" |
| Footer | `<DialogFooter>` | `flex items-center justify-end gap-3 mt-5` |
| Boton "Cancelar" | `<Button variant="outline">` | `border-[#334155] text-[#94a3b8] hover:bg-[#1e1e38] hover:text-white` |
| Boton "Solicitar cobro" (normal) | `<Button type="submit">` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-400 hover:to-purple-500` |
| Boton "Solicitar cobro" (loading) | `<Button type="submit" disabled>` | Mismos estilos con `<Loader2 w-4 h-4 animate-spin mr-2>` y texto "Procesando..." |
| Boton "Solicitar cobro" (form invalido) | `<Button type="submit" disabled>` | `opacity-50 cursor-not-allowed` - aplicado automaticamente por React Hook Form cuando el form no es valido |

### Schema de Validacion Zod (solicitarCobroSchema)

```typescript
// src/shared/schemas/crowdpromotion.schema.ts

export const solicitarCobroSchema = z.object({
    importe: z
        .number({
            required_error: 'El importe es obligatorio',
            invalid_type_error: 'El importe debe ser un numero',
        })
        .positive('El importe debe ser mayor a 0')
        .max(999999, 'El importe es demasiado alto'),
    descripcion: z
        .string()
        .max(500, 'Maximo 500 caracteres')
        .optional()
        .or(z.literal('')),
})

// Refinement adicional aplicado en el componente (no en el schema global porque depende del saldo):
//   .refine(data => data.importe <= saldoDisponible, {
//     message: `El importe no puede superar tu saldo disponible (${saldoDisponible.toFixed(2)} EUR)`,
//     path: ['importe']
//   })
```

### Estados del Dialog

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default (abierto)** | Saldo visible, campo importe vacio y con foco, descripcion vacia, boton "Solicitar cobro" deshabilitado hasta que el importe sea valido |
| **Importe valido** | Boton "Solicitar cobro" habilitado con gradiente. Sin mensajes de error |
| **Importe invalido** | Mensaje de error en rojo bajo el input. Boton deshabilitado con `opacity-50` |
| **Error: importe > saldo** | Mensaje `"El importe no puede superar tu saldo disponible (X EUR)"` en rojo bajo el input |
| **Error: importe <= 0** | Mensaje `"El importe debe ser mayor a 0"` |
| **Loading (enviando)** | Boton muestra spinner + "Procesando...". Ambos botones deshabilitados. Inputs `disabled`. Overlay no se cierra al hacer click fuera |
| **Error de API** | Mostrar `<Alert variant="destructive">` dentro del dialog con el mensaje de error del backend. El dialog permanece abierto para que el promotor pueda corregir |
| **Exito** | Cerrar el dialog. Mostrar toast de exito. Actualizar datos de wallet en el padre |

### Validacion en Tiempo Real del Dialog

| Campo | Regla | Mensaje de Error |
|-------|-------|-----------------|
| Importe | Obligatorio | "El importe es obligatorio" |
| Importe | > 0 | "El importe debe ser mayor a 0" |
| Importe | <= saldoDisponible | "El importe no puede superar tu saldo disponible ({saldo} EUR)" |
| Importe | Numero valido (no texto) | "El importe debe ser un numero valido" |
| Descripcion | max 500 caracteres | "Maximo 500 caracteres" |

La validacion del importe contra el saldo disponible se aplica como refinement en el nivel del componente usando `setError` de React Hook Form o como superRefine pasando el saldo como parametro externo.

### Interacciones del Dialog

| Accion | Comportamiento |
|--------|----------------|
| Abrir dialog | El campo importe recibe foco automatico (`autoFocus`) |
| Click fuera del dialog (durante submit) | No cierra el dialog mientras `isSubmitting === true` |
| Click fuera del dialog (en reposo) | Cierra el dialog. Reset del formulario |
| Tecla Escape | Cierra el dialog si no esta submitting |
| Click "Cancelar" | Cierra el dialog. Reset del formulario |
| Submit exitoso | Cerrar dialog. Toast success. Refetch wallet y transacciones en el padre |
| Submit con error de API | Mantener dialog abierto. Mostrar error inline |
| Escribir en descripcion | Actualizar contador de caracteres en tiempo real |

### Toast Messages

| Evento | Tipo | Mensaje |
|--------|------|---------|
| Solicitud creada con exito | Success (verde) | "Solicitud de cobro registrada. Procesaremos tu pago en breve." |
| Error de API inesperado | Destructive (rojo) | "No se pudo procesar tu solicitud. Intenta de nuevo." |
| Error 409 - Conflicto de concurrencia | Destructive (rojo) | "Hubo un conflicto al procesar tu solicitud. Recarga la pagina e intenta de nuevo." |
| Saldo insuficiente (error del backend) | Destructive (rojo) | "Saldo insuficiente para realizar el cobro." |

---

## Pantalla 3: Empty State (Sin Transacciones)

Esta pantalla no es una ruta diferente, es un estado de la Pantalla 1 cuando el historial no tiene ningun elemento y no hay filtros activos.

**Condicion de activacion:** `wallet.data.saldoDisponible === 0 && transacciones.total === 0 && !hayFiltrosActivos`

### Layout del Empty State

```
┌──────────────────────────────────────────────────────┐
│ SIDEBAR (idem)                                       │
├──────────────────────────────────────────────────────┤
│ MAIN CONTENT                                         │
│                                                      │
│  Mi Wallet                                           │
│                                                      │
│  SALDO CARD (saldo 0)                                │
│  ┌──────────────────────────────────────────┐        │
│  │  SALDO DISPONIBLE                        │        │
│  │     0.00 EUR                             │        │
│  │                                          │        │
│  │  Ganado: 0.00     Retirado: 0.00        │        │
│  │                                          │        │
│  │  [Solicitar cobro] (disabled)            │        │
│  │  Necesitas al menos 10.00 EUR            │        │
│  └──────────────────────────────────────────┘        │
│                                                      │
│  EMPTY STATE                                         │
│                                                      │
│            [icono wallet]                            │
│       Aun no tienes transacciones                    │
│  Completa tareas de promocion o genera               │
│  backings referidos para ganar comisiones.           │
│                                                      │
│            [Explorar programas]                      │
│                                                      │
└──────────────────────────────────────────────────────┘
```

La barra de filtros se oculta completamente cuando se muestra el empty state principal (no tiene sentido filtrar una lista vacia).

---

## Jerarquia de Componentes

```
PromotorWalletPage                         <- Pagina principal, ruta /promotor/wallet
├── WalletSaldoCard                        <- Card de saldo disponible y stats
│   └── SolicitarCobroDialog               <- Modal de solicitud de cobro
│       └── [Form con React Hook Form + Zod]
│
├── FiltrosHistorial                       <- Barra de filtros (Select + date inputs)
│
├── [Loading] TransaccionesListSkeleton    <- Skeleton mientras carga
├── [Error]   WalletErrorState            <- Card de error con boton reintentar
├── [Empty]   WalletEmptyState            <- Empty state sin transacciones
├── [Empty filtered] WalletEmptyFiltered  <- Empty state con filtros sin resultados
│
└── [Default] TransaccionesList           <- Lista paginada de transacciones
    ├── TransaccionItem (x n)             <- Fila individual de transaccion
    │   ├── [Icon] ArrowUp / ArrowDown
    │   ├── [Text] Concepto + contexto + fecha
    │   └── [Right] Importe + Badge estado
    └── PaginacionWallet                  <- Controles de paginacion
```

**Hooks necesarios:**
- `useWallet()` - query para `GET /api/crowdpromotion/promotor/wallet`
- `useWalletTransacciones(filtros, pagina)` - query para `GET /api/crowdpromotion/promotor/wallet/transacciones`
- `useSolicitarCobro()` - mutation para `POST /api/crowdpromotion/promotor/wallet/cobro`

**Ubicacion de archivos sugerida:**
```
src/web/src/features/crowdpromotion/wallet/
  domain/
    index.ts                        <- tipos re-exportados de @shared/types
  application/
    hooks/
      useWallet.ts
      useWalletTransacciones.ts
      useSolicitarCobro.ts
  infrastructure/
    wallet.service.ts
  presentation/
    pages/
      PromotorWalletPage.tsx
    components/
      WalletSaldoCard.tsx
      FiltrosHistorial.tsx
      TransaccionItem.tsx
      TransaccionEstadoBadge.tsx
      TransaccionesList.tsx
      TransaccionesListSkeleton.tsx
      WalletEmptyState.tsx
      PaginacionWallet.tsx
      SolicitarCobroDialog.tsx
      index.ts
```

---

## Navegacion y Routing

### Ruta nueva a agregar en router.tsx

```tsx
// En la seccion de rutas protegidas (DashboardLayout):
const PromotorWalletPage = lazy(() => import("@/features/crowdpromotion/wallet/presentation/pages/PromotorWalletPage"))

// Dentro de <Route element={<DashboardLayout />}>:
<Route path="/promotor/wallet" element={<PromotorWalletPage />} />
```

### Accesos a la Wallet

| Origen | Destino | Tipo |
|--------|---------|------|
| Item de sidebar "Mi Wallet" | `/promotor/wallet` | Link de navegacion |
| Completar tarea con reward monetario (success toast) | Link en el toast a `/promotor/wallet` | Opcional - link "Ver tu wallet" en el toast |
| Pagina de metricas (KPI comision) | `/promotor/wallet` | Link opcional en la card de comision acumulada |
| CTA en empty state de transacciones | `/crowdpromotion/explorar` | Navegacion a explorar programas |

### Item del Sidebar

El sidebar actual (`src/web/src/components/layout/Sidebar.tsx`) debe incluir un nuevo item:

| Propiedad | Valor |
|-----------|-------|
| Icono | `<Wallet w-4 h-4>` de lucide-react |
| Texto | "Mi Wallet" |
| Ruta | `/promotor/wallet` |
| Posicion | Despues de "Metricas" en el grupo de navegacion del promotor |
| Badge de saldo | Opcional: badge pequeno con el saldo disponible (ej. "150 EUR") si hay datos cargados. No obligatorio para MVP |

---

## Responsive Breakpoints

| Breakpoint | Width | Cambios en la UI |
|------------|-------|-----------------|
| Mobile | < 640px | Sidebar colapsado (hamburger). `WalletSaldoCard`: grid de stats en columna unica (`grid-cols-1`). Valor principal con `text-3xl` en lugar de `text-4xl`. Filtros en dos filas (tipo/estado en la primera, fechas en la segunda). `TransaccionItem`: descripcion truncada, sin texto de contexto secundario. `SolicitarCobroDialog`: ocupa `max-w-full` con bordes al ras en mobile (usa `sm:rounded-lg` del componente) |
| Tablet | 640-1024px | Sidebar fijo visible (`w-48`). Stats de wallet en 2 columnas. Filtros en fila unica. Dialog centrado con `max-w-md`. Transacciones con descripcion completa |
| Desktop | > 1024px | Layout completo. Sidebar `w-64`. Contenido centrado con `max-w-3xl mx-auto`. Dialog `max-w-md`. Todas las columnas visibles |

### Adaptaciones especificas mobile de WalletSaldoCard

```
Mobile (< 640px):
  p-4 (menos padding)
  text-3xl para el saldo (en vez de text-4xl)
  grid-cols-1 para stats (en vez de grid-cols-2)
  Boton "Solicitar cobro" a full width: w-full
  Texto de minimo debajo del boton, centrado
```

### Adaptaciones especificas mobile de FiltrosHistorial

```
Mobile (< 640px):
  flex-col (en vez de flex-wrap)
  Select "Tipo" y Select "Estado" cada uno a full width (w-full)
  Inputs de fecha en fila de 2 columnas (grid grid-cols-2 gap-2)
  Boton "Limpiar filtros" a full width
```

### Adaptaciones especificas mobile de TransaccionItem

```
Mobile (< 640px):
  py-3 (menos padding vertical)
  Texto de contexto secundario (tipoRewardNombre) ocultado con hidden sm:block
  Fecha movida debajo del concepto (no a la derecha del importe)
  Importe y badge en columna vertical (en vez de alineados en la misma fila)
```

---

## Animaciones

| Elemento | Animacion | Duracion | Trigger |
|----------|-----------|----------|---------|
| WalletSaldoCard al cargar datos | Fade in `opacity-0 -> opacity-100` + slide up `translateY(8px) -> 0` | 300ms ease-out | Cuando los datos del wallet resuelven (reemplazan skeleton) |
| TransaccionItem al cargar lista | Fade in escalonado con `animation-delay` por item (0ms, 50ms, 100ms...) | 200ms ease-out cada item | Cuando la lista de transacciones resuelve |
| Dialog open | Zoom in desde 95% + fade in (comportamiento por defecto de shadcn Dialog: `data-[state=open]:zoom-in-95 data-[state=open]:fade-in-0`) | 200ms | Al abrir el dialog |
| Dialog close | Zoom out a 95% + fade out | 200ms | Al cerrar el dialog |
| Hover sobre TransaccionItem | `background-color` transition | 150ms | Hover del mouse |
| Badge de estado en TransaccionItem | Sin animacion especial. El color es estatico | - | - |
| Boton "Solicitar cobro" (gradient hover) | Transition en el gradiente (aclarar tonos) `transition-all duration-150` | 150ms | Hover |
| Skeleton | Pulso de opacidad | 1.5s infinite | Mientras el dato no ha cargado |
| Nueva transaccion al top de la lista (post-submit) | Refetch completo de la query; la lista se re-renderiza. Sin animacion de insercion en MVP | - | Post-submit exitoso |
| Contador de caracteres en textarea | Cambia de `text-[#64748b]` a `text-[#f59e0b]` cuando supera 400 caracteres y a `text-[#ef4444]` al llegar a 500 | Instantaneo | Al escribir |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1 para texto normal. Saldo disponible (`#f59e0b` sobre `#151525`) cumple AA. Badges de estado con texto coloreado sobre fondo oscuro cumplen minimo 3:1 |
| Focus ring | `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#1a1a2e]` en todos los elementos interactivos (botones, selects, inputs, paginacion) |
| Labels de formulario | Todos los `<Input>` y `<Select>` del dialog tienen `<Label htmlFor>` asociado. Los selects de filtros tienen `aria-label` (ej: `aria-label="Filtrar por tipo de transaccion"`) |
| Dialog accesible | `<Dialog>` de shadcn usa Radix UI que gestiona focus-trap, `aria-modal="true"`, retorno de foco al trigger al cerrar. `<DialogTitle>` visible siempre |
| Estados de carga | `aria-busy="true"` en el contenedor principal de la pagina durante la carga inicial. Los skeletons son decorativos con `aria-hidden="true"` |
| Mensajes de error | `role="alert"` en los contenedores de error (tanto en la pagina como en el dialog) para anuncio inmediato a lectores de pantalla |
| Empty state | El boton CTA del empty state tiene texto descriptivo "Explorar programas de promocion" (texto completo en el `aria-label` si el texto del boton es mas corto) |
| Boton deshabilitado | `disabled` semantico en el boton de solicitar cobro. El aviso de saldo insuficiente esta asociado al boton via `aria-describedby` para que lectores de pantalla lo anuncien al enfocar el boton |
| Lista de transacciones | El elemento contenedor de la lista tiene `role="list"` o es un `<ul>`; cada `TransaccionItem` es un `<li>`. Las fechas usan `<time dateTime="2026-03-20">` |
| Badges de estado | Los badges tienen texto visible (no solo color); no dependen solo del color para transmitir el estado |
| Importe con signo | El signo +/- delante del importe se anuncia textualmente. No usar solo el color para indicar credito/debito; el signo y el icono de flecha son esenciales |
| Paginacion | Los botones de pagina tienen `aria-label` descriptivos: "Ir a la pagina 2", "Pagina anterior", "Pagina siguiente". La pagina activa tiene `aria-current="page"` |

---

## Futuro: Vista Admin (Fuera de Scope MVP)

El modulo Admin (`src/admin`) no implementa ninguna vista de wallet en el MVP. La gestion manual de transacciones (marcar Procesada/Pagada/Cancelada) se hace directamente en la base de datos en MVP.

En una iteracion futura, el Admin podria incluir:

| Pantalla futura | Ruta | Descripcion |
|-----------------|------|-------------|
| Lista de wallets | `/admin/promotores/wallets` | Ver todos los promotores con su saldo actual, filtrar por estado, exportar |
| Detalle de transacciones | `/admin/promotores/{id}/wallet` | Ver historial completo de un promotor y cambiar estado de transacciones |
| Procesador de cobros | `/admin/cobros/pendientes` | Lista de transacciones de debito en estado Pendiente para procesar masivamente |

Estos no son parte de US-CP-06 y no se definen en este documento.

---

## Checklist UI/UX

### Pantalla 1: Mi Wallet (/promotor/wallet)
- [ ] Ruta `/promotor/wallet` registrada en `router.tsx` dentro de `DashboardLayout`
- [ ] Item "Mi Wallet" agregado al sidebar con icono `<Wallet>`
- [ ] `WalletSaldoCard` con saldo disponible en `text-4xl font-bold text-[#f59e0b]`
- [ ] Stats secundarias (Total Ganado en verde, Total Retirado en gris) en grid de 2 columnas
- [ ] Boton "Solicitar cobro" habilitado solo si `saldo >= minimoRetiro`
- [ ] Aviso de saldo insuficiente visible cuando boton deshabilitado
- [ ] `FiltrosHistorial` con Select tipo, Select estado, inputs de fecha
- [ ] Boton "Limpiar filtros" visible solo cuando hay filtros activos
- [ ] `TransaccionItem` con icono flecha coloreado (verde=credito, rojo=debito)
- [ ] Importe con signo + o - y color correcto
- [ ] Badge de estado con color correcto (amarillo=Pendiente, verde=Procesada, azul=Pagada, gris=Cancelada)
- [ ] Fecha formateada en espanol "20 mar 2026"
- [ ] `WalletEmptyState` cuando no hay transacciones con CTA a `/crowdpromotion/explorar`
- [ ] Empty state de filtros sin resultados con boton "Limpiar filtros"
- [ ] `WalletSaldoCard` skeleton con `animate-pulse` durante carga
- [ ] `TransaccionesListSkeleton` con 5 filas durante carga
- [ ] Estado error con icono y boton reintentar
- [ ] `PaginacionWallet` con shadcn Pagination, texto informativo "Mostrando X-Y de Z"
- [ ] Paginacion con pagina activa resaltada en purple
- [ ] Post-submit: refetch de wallet y transacciones, toast de exito
- [ ] Responsive: mobile con grid-cols-1 en stats, boton full-width, filtros en columna

### Pantalla 2: Dialog Solicitar Cobro
- [ ] `SolicitarCobroDialog` abre al click en "Solicitar cobro"
- [ ] Saldo disponible visible en el dialog en `text-2xl text-[#f59e0b]`
- [ ] Minimo de retiro visible como texto informativo
- [ ] Campo importe con tipo `number`, `min="0.01"`, `step="0.01"`
- [ ] Sufijo "EUR" posicionado absolutamente en el input
- [ ] Hint "Hasta X EUR disponibles" bajo el input
- [ ] Validacion Zod: importe obligatorio, positivo, no exceder saldo
- [ ] Contador de caracteres en textarea de descripcion
- [ ] Boton submit deshabilitado cuando form invalido
- [ ] Boton submit con spinner `<Loader2 animate-spin>` durante submit
- [ ] Ambos botones deshabilitados durante el submit
- [ ] Error de API mostrado como `<Alert variant="destructive">` dentro del dialog
- [ ] Dialog cierra en exito, toast de confirmacion
- [ ] Dialog NO cierra al hacer click fuera si `isSubmitting`
- [ ] Focus automatico en el campo importe al abrir
- [ ] Reset del form al cerrar el dialog

### Accesibilidad
- [ ] Focus ring visible en todos los elementos interactivos
- [ ] `aria-busy="true"` durante carga inicial
- [ ] `role="alert"` en mensajes de error
- [ ] `disabled` semantico en boton con saldo insuficiente
- [ ] `aria-describedby` en boton deshabilitado apuntando al aviso de saldo
- [ ] Importe muestra signo textual (+ o -) no solo color
- [ ] Badges de estado con texto visible
- [ ] `<time dateTime>` en fechas de transacciones
- [ ] Dialog con `<DialogTitle>` visible
- [ ] Labels asociados a todos los inputs del dialog
