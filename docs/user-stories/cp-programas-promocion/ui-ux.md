# UI/UX: Programas de Promocion

> **Feature:** cp-programas-promocion
> **User Story:** US-CP-02
> **Ultima actualizacion:** 2026-02-25

---

## Mockups de Referencia

No existen mockups dedicados para esta feature. Se aplica el lenguaje visual extraido de los mockups existentes del proyecto.

| Pantalla | Archivo | Proyecto | Uso |
|----------|---------|----------|-----|
| Dashboard Artista | [WPR_5-Dashboard-Artist.png](../../ui-images/WPR_5-Dashboard-Artist.png) | Admin | Patron sidebar + KPI cards + layout general del dashboard |
| Crear Campana (Wizard) | [WPR_6-Create-Campaign.png](../../ui-images/WPR_6-Create-Campaign.png) | Admin | Patron de wizard de 4 pasos: stepper numerado, contenedor de paso, botones Anterior/Siguiente, layout full-page |

**Observaciones extraidas de WPR_6-Create-Campaign.png:**
- Wizard en pantalla completa con header propio (`bg-[#0d0d1a]`, logo izquierda, X derecha)
- Stepper: 4 circulos numerados conectados con lineas horizontales, paso activo con fondo gradient pink/purple, pasos futuros con fondo `#1e1e38` texto muted
- Etiqueta de paso activo en color accent `#a855f7` debajo del circulo
- Contenedor del paso: `bg-[#151525] border border-[#334155] rounded-xl` centrado con `max-w-2xl`
- Barra inferior: fija con `bg-[#0d0d1a] border-t border-[#334155]`, texto "Paso X de 4" centrado, botones en los extremos
- Boton Siguiente: gradient pink/purple, boton Anterior: outline oscuro

---

## Templates de Referencia

| Template | Uso | Ruta |
|----------|-----|------|
| **Dashtail** | Dashboard artista, wizard crear programa, listado, detalle | `references/templates/dashtail/` |
| **Krowd** | No aplica a esta feature (es 100% dashboard admin) | `references/templates/krowd/` |

**Componentes de referencia clave:**
- Layout dashboard: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/app/[lang]/(dashboard)/layout.tsx`
- Dialogo de confirmacion: `dashtail/Dashtail-starter-v1.3.0/dash-tail-starter-kit(TypeScript)/components/delete-confirmation-dialog.tsx`
- Patron wizard: extraido de WPR_6-Create-Campaign.png (stepper + contenedor + barra inferior)

---

## Design Tokens

### Paleta de Colores

Reutilizados integramente de US-CP-01 (cp-perfil-promotor). No se introducen nuevos tokens de color.

```css
:root {
  /* Backgrounds */
  --bg-primary: #0d0d1a;        /* Fondo base (sidebar, header wizard) */
  --bg-secondary: #1a1a2e;      /* Fondo de paginas y contenedores */
  --bg-card: #151525;           /* Fondo de cards, formularios, contenedor de paso */
  --bg-card-hover: #1e1e38;     /* Card hover state */
  --bg-input: #0f0f1f;          /* Fondo de inputs */
  --bg-input-disabled: #1a1a2e; /* Input deshabilitado */
  --bg-sidebar: #0d0d1a;        /* Sidebar del dashboard */

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

  /* Estado */
  --status-active: #10b981;     /* Verde - programa ACTIVO */
  --status-inactive: #64748b;   /* Gris - programa INACTIVO */
  --status-warning: #f59e0b;    /* Amarillo - aviso promotores inscritos */
  --status-error: #ef4444;      /* Rojo - errores de validacion */
  --status-info: #3b82f6;       /* Azul - mensajes informativos */

  /* Bordes */
  --border-default: #334155;
  --border-focus: #a855f7;
  --border-error: #ef4444;
  --border-warning: #f59e0b;

  /* KPI Cards */
  --card-kpi-bg: #151525;
  --card-kpi-icon-bg-green: rgba(16, 185, 129, 0.15);
  --card-kpi-icon-bg-blue: rgba(59, 130, 246, 0.15);
  --card-kpi-icon-bg-purple: rgba(168, 85, 247, 0.15);
  --card-kpi-icon-bg-orange: rgba(245, 158, 11, 0.15);
  --card-kpi-icon-green: #10b981;
  --card-kpi-icon-blue: #3b82f6;
  --card-kpi-icon-purple: #a855f7;
  --card-kpi-icon-orange: #f59e0b;

  /* Stepper (wizard) */
  --stepper-active-bg: linear-gradient(135deg, #ec4899 0%, #a855f7 100%);
  --stepper-done-bg: #1e3a2f;
  --stepper-done-border: #10b981;
  --stepper-done-text: #10b981;
  --stepper-future-bg: #1e1e38;
  --stepper-future-text: #64748b;
  --stepper-connector: #334155;
}
```

### Tipografia

```css
:root {
  --font-sans: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;

  /* Tamanios */
  --text-xs: 0.75rem;    /* 12px - etiquetas auxiliares, hints */
  --text-sm: 0.875rem;   /* 14px - labels de campos, texto secundario */
  --text-base: 1rem;     /* 16px - texto de inputs, contenido */
  --text-lg: 1.125rem;   /* 18px - titulo de seccion dentro del paso */
  --text-xl: 1.25rem;    /* 20px - titulos de card */
  --text-2xl: 1.5rem;    /* 24px - titulo principal de wizard */
  --text-3xl: 1.875rem;  /* 30px - titulos de pagina (listado, detalle) */
  --text-4xl: 2.25rem;   /* 36px - valor de KPI card */

  /* Pesos */
  --font-normal: 400;
  --font-medium: 500;
  --font-semibold: 600;
  --font-bold: 700;

  /* Interlineado */
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
  --radius-sm: 0.375rem;  /* 6px */
  --radius-md: 0.5rem;    /* 8px */
  --radius-lg: 0.75rem;   /* 12px */
  --radius-xl: 1rem;      /* 16px */
  --radius-full: 9999px;

  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.3);
  --shadow-glow: 0 0 20px rgba(168, 85, 247, 0.35);
  --shadow-glow-green: 0 0 12px rgba(16, 185, 129, 0.25);
}
```

---

## Pantalla 1: Wizard - Crear Programa de Promocion

**Mockup:** WPR_6-Create-Campaign.png (patron visual de referencia)
**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/nuevo`
**Template base:** patron de WPR_6 adaptado para CrowdPromotion

**Precondicion:** Usuario autenticado con rol Artista. Si no tiene campanas ni proyectos, la pantalla igualmente es accesible (FA-01: programa sin vincular campana).

**Estado del wizard:** Gestionado completamente en cliente con `useReducer`. No hay llamadas a la API hasta el Paso 4 (publicar). El estado se pierde al navegar fuera sin confirmar (mostrar dialogo de confirmacion de abandono).

### Layout General (todos los pasos)

```
┌──────────────────────────────────────────────────────────────────────┐
│ WIZARD HEADER (h-14, bg-[#0d0d1a], border-b border-[#334155])        │
│ [Logo WePlay]  Crear programa de promocion              [X Cancelar] │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│    STEPPER (mt-8 mb-10)                                              │
│    [1 DATOS]---------[2 COMISIONES]---------[3 TAREAS]---------[4]  │
│    Datos basicos      Comisiones            Definir tareas   Revisar │
│                                                                      │
│    ┌────────────────────────────────────────────────────────────┐    │
│    │                                                            │    │
│    │   CONTENIDO DEL PASO ACTIVO                                │    │
│    │   (max-w-2xl mx-auto, bg-[#151525], border, rounded-xl)    │    │
│    │                                                            │    │
│    └────────────────────────────────────────────────────────────┘    │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│ WIZARD FOOTER (h-16, bg-[#0d0d1a], border-t border-[#334155])        │
│ [<- Anterior]              Paso X de 4              [Siguiente ->]   │
└──────────────────────────────────────────────────────────────────────┘
```

### Componente Stepper

El stepper es un componente personalizado (no en shadcn/ui de serie). Se implementa con `<div>` y Tailwind.

```
┌──────────────────────────────────────────────────────────────────────────┐
│                                                                          │
│  ┌────┐  ───────────  ┌────┐  ───────────  ┌────┐  ───────────  ┌────┐  │
│  │ 1  │               │ 2  │               │ 3  │               │ 4  │  │
│  └────┘               └────┘               └────┘               └────┘  │
│  Datos basicos        Comisiones           Definir tareas       Revisar  │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

| Estado del circulo | Estilos |
|--------------------|---------|
| Paso activo (actual) | `w-10 h-10 rounded-full bg-gradient-to-r from-pink-500 to-purple-600 text-white font-bold flex items-center justify-center` |
| Paso completado | `w-10 h-10 rounded-full bg-[#1e3a2f] border-2 border-[#10b981] text-[#10b981] flex items-center justify-center` (muestra checkmark `<Check w-4 h-4>` en lugar del numero) |
| Paso futuro | `w-10 h-10 rounded-full bg-[#1e1e38] text-[#64748b] flex items-center justify-center` |
| Etiqueta paso activo | `text-xs font-medium text-[#a855f7] mt-2 text-center` |
| Etiqueta paso completado | `text-xs text-[#10b981] mt-2 text-center` |
| Etiqueta paso futuro | `text-xs text-[#64748b] mt-2 text-center` |
| Linea conectora pendiente | `flex-1 h-px bg-[#334155] mx-2 mt-5` |
| Linea conectora completada | `flex-1 h-px bg-[#10b981] mx-2 mt-5` |

---

### Paso 1: Datos Basicos

```
┌──────────────────────────────────────────────────────────────────┐
│  Datos basicos del programa                                      │
│  Configura la informacion principal de tu programa               │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Titulo *                                                        │
│  [Ej: Promociona mi nuevo album...                           ]   │
│                                                                  │
│  Descripcion                                                     │
│  [Describe que deben hacer los promotores y que ofreces...   ]   │
│  [                                                           ]   │
│  [                                                     0/4000]   │
│                                                                  │
│  Tipo de programa *                                              │
│  [Referral                                                  v]   │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Campana de crowdfunding     │  │ Proyecto artistico        │   │
│  │ [Mi Album Debut          v] │  │ [Tour 2026            v]  │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│  [i] Vincula al menos uno de los dos (opcional)                  │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ URL de landing              │  │ Codigo de tracking *     │   │
│  │ [https://...            ]   │  │ [album-2026          ]   │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│  Hint: Solo letras, numeros y guiones. Debe ser unico.           │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Fecha inicio                │  │ Fecha fin                │   │
│  │ [2026-03-01          cal]   │  │ [2026-06-01        cal]  │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

**Especificaciones Paso 1**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo de paso | `<h2>` | `text-xl font-semibold text-white mb-1` |
| Subtitulo de paso | `<p>` | `text-sm text-[#94a3b8] mb-6` |
| Separador | `<Separator>` | `bg-[#334155] mb-6` |
| Label requerido | `<Label>` con asterisco | `text-sm font-medium text-[#cbd5e1] mb-1.5 block after:content-['*'] after:text-red-500 after:ml-1` |
| Label opcional | `<Label>` | `text-sm font-medium text-[#cbd5e1] mb-1.5 block` |
| Input titulo | `<Input>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-11` |
| Textarea descripcion | `<Textarea>` | `bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] min-h-[100px] resize-none` |
| Contador descripcion | `<span>` | `text-xs text-[#64748b] text-right block mt-1` |
| Select tipo programa | `<Select>` + `<SelectTrigger>` + `<SelectContent>` + `<SelectItem>` | Trigger: `bg-[#0f0f1f] border-[#334155] text-white h-11` / Content: `bg-[#151525] border-[#334155]` / Item hover: `bg-[#1e1e38] text-white` |
| Grid 2 columnas | `<div>` | `grid grid-cols-2 gap-4` |
| Select campana | `<Select>` (opcional) | Igual que Select tipo programa. Primer item: `value="" label="Sin campana vinculada"` |
| Select proyecto | `<Select>` (opcional) | Igual que Select tipo programa. Primer item: `value="" label="Sin proyecto vinculado"` |
| Aviso vinculacion opcional | `<div>` con icono Info | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-1 mb-4` |
| Input URL landing | `<Input type="url">` | Igual que input titulo. Placeholder: `https://weplay.com/campanias/mi-album` |
| Input codigo tracking | `<Input>` | Igual que input titulo. Placeholder: `ej: album-2026` |
| Hint codigo tracking | `<p>` | `text-xs text-[#64748b] mt-1` |
| Input fecha inicio | `<Input type="date">` | `bg-[#0f0f1f] border-[#334155] text-white h-11 [color-scheme:dark]` |
| Input fecha fin | `<Input type="date">` | Igual que fecha inicio |
| Mensaje error campo | `<p role="alert">` | `text-xs text-red-400 mt-1 flex items-center gap-1` (icono `<AlertCircle w-3 h-3>` + texto) |

**Opciones Select Tipo de Programa**

| Valor (id) | Etiqueta visible | Descripcion en hint |
|------------|-----------------|---------------------|
| `1` | Referral | Comision por cada nuevo backer referido |
| `2` | Afiliado | Comision por ventas generadas |
| `3` | Influencer | Tareas de contenido con recompensa |
| `4` | Mixto | Combinacion de referral + tareas de contenido |

Al seleccionar un tipo, mostrar un hint contextual debajo del selector: `text-xs text-[#64748b] mt-1 italic`.

---

### Paso 2: Configurar Comisiones

```
┌──────────────────────────────────────────────────────────────────┐
│  Configurar comisiones                                           │
│  Define cuanto ganar cada promotor por sus conversiones          │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Moneda *                                                        │
│  [EUR - Euro                                                v]   │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Comision por porcentaje     │  │ Comision fija por        │   │
│  │                             │  │ conversion               │   │
│  │  [ 10.00              ] %   │  │  [ 5.00               ]  │   │
│  │  De cada backing referido   │  │  EUR por cada conversion │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│                                                                  │
│  [!] Al menos una de las dos comisiones es obligatoria           │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐    │
│  │ Vista previa de comision                                 │    │
│  │                                                          │    │
│  │  Un backing de 50 EUR generaria:                         │    │
│  │  Comision porcentaje: 5.00 EUR (10%)                     │    │
│  │  Comision fija:       5.00 EUR                           │    │
│  │  Total promotor:     10.00 EUR                           │    │
│  └──────────────────────────────────────────────────────────┘    │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

**Especificaciones Paso 2**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo de paso | `<h2>` | `text-xl font-semibold text-white mb-1` |
| Subtitulo de paso | `<p>` | `text-sm text-[#94a3b8] mb-6` |
| Select moneda | `<Select>` | Igual que selects del paso 1. Opciones cargadas desde API maestras |
| Grid 2 columnas | `<div>` | `grid grid-cols-2 gap-4` |
| Input comision porcentaje | `<div>` con `<Input>` y sufijo | `flex items-center gap-2` / Input: `bg-[#0f0f1f] border-[#334155] text-white h-11 w-full` / Sufijo `%`: `text-[#94a3b8] font-medium text-sm` |
| Input comision fija | `<div>` con `<Input>` | Similar a porcentaje pero sin sufijo visible (la moneda se muestra como hint debajo) |
| Hint de campo | `<p>` | `text-xs text-[#64748b] mt-1` |
| Aviso obligatoriedad | `<div>` con icono AlertTriangle | `flex items-start gap-2 p-3 rounded-lg bg-amber-950/40 border border-amber-800/50 text-sm text-amber-300 mt-2` |
| Card vista previa | `<Card>` | `bg-[#0f0f1f] border-[#334155] p-4 mt-4` |
| Titulo vista previa | `<p>` | `text-sm font-medium text-[#94a3b8] mb-3` |
| Fila de calculo | `<div>` | `flex justify-between text-sm mb-1` / label: `text-[#94a3b8]` / valor: `text-white font-medium` |
| Fila total | `<div>` | `flex justify-between text-sm font-semibold border-t border-[#334155] mt-2 pt-2` / label: `text-[#cbd5e1]` / valor: `text-[#10b981]` |

La card de vista previa se actualiza en tiempo real conforme el usuario escribe. Si ninguna comision esta definida, mostrar: "Ingresa al menos una comision para ver la vista previa" en texto muted.

---

### Paso 3: Definir Tareas

Este es el paso mas complejo. Permite agregar, editar y eliminar tareas inline sin navegacion adicional.

```
┌──────────────────────────────────────────────────────────────────┐
│  Definir tareas                                                  │
│  Especifica que deben hacer los promotores para ganar recompensas│
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │ [Share]  Comparte en Instagram Stories          [  ] Activa│  │
│  │ Dinero: 5.00 EUR  |  Repetible x10  |  Mar-Jun 2026        │  │
│  │                              [Editar v]  [Eliminar (trash)] │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │ [Post]   Publica un TikTok sobre la campana     [  ] Activa│  │
│  │ Dinero: 10.00 EUR  |  Repetible x5  |  Mar-Jun 2026        │  │
│  │                              [Editar v]  [Eliminar (trash)] │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  [+ Agregar nueva tarea]                                         │
│                                                                  │
│  [i] Puedes crear el programa sin tareas y agregar las despues   │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

**Formulario de Tarea (inline, expandido al agregar o editar)**

```
┌──────────────────────────────────────────────────────────────────┐
│  Nueva tarea                                           [X cerrar]│
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Nombre de la tarea *                                            │
│  [Ej: Comparte en Instagram Stories                          ]   │
│                                                                  │
│  Descripcion                                                     │
│  [Instrucciones detalladas para el promotor...               ]   │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Tipo de evento *            │  │ Tipo de recompensa *     │   │
│  │ [Share                  v]  │  │ [Dinero              v]  │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│                                                                  │
│  --- Configuracion de recompensa (segun tipo seleccionado) ---   │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Importe recompensa          │  │ Moneda                   │   │
│  │ [  5.00                 ]   │  │ [EUR                 v]  │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│                                                                  │
│  Puntos de recompensa (si tipo Mixto o Puntos)                   │
│  [  100                     ]                                    │
│                                                                  │
│  URL de instrucciones                                            │
│  [https://docs.example.com/instrucciones                     ]   │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐     │
│  │ [toggle] Es repetible         [toggle ON]               │     │
│  │                                                         │     │
│  │  Maximo de repeticiones *  (visible si toggle ON)       │     │
│  │  [  10                  ]                               │     │
│  └─────────────────────────────────────────────────────────┘     │
│                                                                  │
│  ┌─────────────────────────────┐  ┌──────────────────────────┐   │
│  │ Fecha inicio                │  │ Fecha fin                │   │
│  │ [2026-03-01          cal]   │  │ [2026-06-01        cal]  │   │
│  └─────────────────────────────┘  └──────────────────────────┘   │
│                                                                  │
│  [Cancelar]                              [Guardar tarea  ->]     │
└──────────────────────────────────────────────────────────────────┘
```

**Especificaciones Paso 3 - Lista de tareas**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor de lista | `<div>` | `space-y-3` |
| Card de tarea | `<Card>` | `bg-[#0f0f1f] border-[#334155] p-4 hover:border-[#a855f7] transition-colors` |
| Badge tipo evento | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs font-medium` |
| Nombre de tarea | `<p>` | `text-sm font-semibold text-white ml-2` |
| Toggle activa | `<Switch>` | `data-[state=checked]:bg-[#10b981]` |
| Label toggle | `<span>` | `text-xs text-[#94a3b8] ml-2` |
| Detalles de tarea | `<p>` | `text-xs text-[#64748b] mt-1` |
| Contenedor acciones | `<div>` | `flex justify-end gap-2 mt-3` |
| Boton Editar | `<Button variant="outline" size="sm">` | `border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 px-3 text-xs` |
| Boton Eliminar | `<Button variant="ghost" size="sm">` | `text-red-400 hover:text-red-300 hover:bg-red-950/30 h-8 w-8 p-0` con icono `<Trash2 w-3.5 h-3.5>` |
| Boton agregar tarea | `<Button variant="outline">` | `w-full border-dashed border-[#334155] text-[#a855f7] hover:border-[#a855f7] hover:bg-[#a855f7]/5 h-11 mt-2` con icono `<Plus w-4 h-4 mr-2>` |
| Aviso sin tareas opcional | `<div>` con icono Info | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-2` |

**Especificaciones Paso 3 - Formulario inline de tarea**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor formulario | `<Card>` | `bg-[#151525] border border-[#a855f7] rounded-xl p-5 mt-3` (borde accent para destacar que esta en edicion) |
| Header formulario | `<div>` | `flex justify-between items-center mb-4` |
| Titulo formulario | `<h3>` | `text-base font-semibold text-white` |
| Boton cerrar | `<Button variant="ghost" size="sm">` | `h-8 w-8 p-0 text-[#94a3b8] hover:text-white` con icono `<X w-4 h-4>` |
| Input nombre tarea | `<Input>` | Igual que inputs de paso 1 |
| Textarea descripcion | `<Textarea>` | Igual que textarea de paso 1, `min-h-[80px]` |
| Select tipo evento | `<Select>` | Igual que selects de paso 1 |
| Select tipo recompensa | `<Select>` | Igual que selects de paso 1 |
| Seccion recompensa | `<div>` | `bg-[#0f0f1f] rounded-lg p-4 mt-3 space-y-3` (contenedor visual para agrupar campos de recompensa) |
| Input importe | `<Input type="number">` | `bg-[#0f0f1f] border-[#334155] text-white h-11` |
| Select moneda tarea | `<Select>` | Visible solo si importe > 0 o tipo Dinero/Mixto |
| Input puntos | `<Input type="number">` | Visible solo si tipo Puntos o Mixto |
| Input URL instrucciones | `<Input type="url">` | Igual que input URL del paso 1 |
| Toggle es repetible | `<Switch>` + `<Label>` | `flex items-center gap-3` / Switch: `data-[state=checked]:bg-[#a855f7]` |
| Input max repeticiones | `<Input type="number" min="1">` | Visible y requerido solo si toggle activo. `bg-[#0f0f1f] border-[#334155] text-white h-11` |
| Grid fechas | `<div>` | `grid grid-cols-2 gap-4` |
| Input fecha inicio tarea | `<Input type="date">` | Igual que fechas del paso 1 |
| Input fecha fin tarea | `<Input type="date">` | Igual que fechas del paso 1 |
| Boton Cancelar | `<Button variant="ghost">` | `text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]` |
| Boton Guardar tarea | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold` |

**Visibilidad condicional de campos de recompensa (Paso 3)**

| Tipo de recompensa | Campos visibles |
|--------------------|----------------|
| Dinero | Importe recompensa (requerido) + Moneda (requerida) |
| Puntos | Puntos de recompensa (requerido) |
| Mixto | Importe recompensa (requerido) + Moneda (requerida) + Puntos de recompensa (requerido) |

---

### Paso 4: Revisar y Publicar

```
┌──────────────────────────────────────────────────────────────────┐
│  Revisar y publicar                                              │
│  Revisa todos los datos antes de crear el programa               │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Datos del programa                                              │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │ Titulo:      Promociona mi nuevo album                     │  │
│  │ Tipo:        Referral                     [Editar]         │  │
│  │ Campana:     Mi Album Debut                                │  │
│  │ URL landing: https://weplay.com/campanias/mi-album         │  │
│  │ Tracking:    album-2026                                    │  │
│  │ Periodo:     01 Mar 2026 - 01 Jun 2026                     │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  Comisiones                                                      │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │ Moneda:             EUR                   [Editar]         │  │
│  │ Por porcentaje:     10%                                    │  │
│  │ Fija por conv.:     --                                     │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  Tareas (2)                                                      │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │ 1. Comparte en Instagram Stories         [Editar]          │  │
│  │    Share | Dinero: 5.00 EUR | Repetible x10                │  │
│  │                                                            │  │
│  │ 2. Publica un TikTok                                       │  │
│  │    Post | Dinero: 10.00 EUR | Repetible x5                 │  │
│  └────────────────────────────────────────────────────────────┘  │
│                                                                  │
│  [i] Al confirmar, el programa se publicara con estado Activo.   │
│      Los promotores podran inscribirse inmediatamente.           │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

El boton del footer en el paso 4 cambia de "Siguiente" a "Publicar programa".

**Especificaciones Paso 4**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo de paso | `<h2>` | `text-xl font-semibold text-white mb-1` |
| Subtitulo de paso | `<p>` | `text-sm text-[#94a3b8] mb-6` |
| Seccion de resumen | `<div>` | `space-y-4` |
| Titulo de seccion | `<h3>` | `text-sm font-semibold text-[#cbd5e1] uppercase tracking-wider mb-2` |
| Card de seccion | `<Card>` | `bg-[#0f0f1f] border-[#334155] p-4` |
| Fila de dato | `<div>` | `flex justify-between items-start py-1.5 border-b border-[#1e1e38] last:border-0` |
| Label de dato | `<span>` | `text-sm text-[#64748b]` |
| Valor de dato | `<span>` | `text-sm text-white font-medium text-right max-w-[60%]` |
| Valor nulo | `<span>` | `text-sm text-[#64748b] italic` con "--" |
| Boton Editar seccion | `<Button variant="ghost" size="sm">` | `text-[#a855f7] hover:text-purple-400 hover:bg-[#a855f7]/10 h-6 px-2 text-xs` con icono `<Pencil w-3 h-3 mr-1>` |
| Item de tarea en resumen | `<div>` | `py-2 border-b border-[#1e1e38] last:border-0` |
| Nombre tarea resumen | `<p>` | `text-sm text-white font-medium` |
| Detalles tarea resumen | `<p>` | `text-xs text-[#64748b] mt-0.5` |
| Aviso publicacion | `<div>` con icono Info | `flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 text-sm text-blue-300 mt-4` |
| Boton Publicar (footer) | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-11 px-8` con icono `<Rocket w-4 h-4 mr-2>` |
| Spinner publicar | `<Loader2>` | `w-4 h-4 animate-spin mr-2` (visible durante el submit) |

### Estados de UI del Wizard (todos los pasos)

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Paso 1 activo, campos vacios con placeholders, boton Anterior deshabilitado (gris opaco) en paso 1 |
| **Datos parciales** | Al avanzar de paso sin completar campos requeridos, mostrar errores de validacion debajo de cada campo. El wizard NO avanza |
| **Cargando maestras** | Selects con skeleton animado: `bg-[#1e1e38] animate-pulse rounded h-11 w-full`. Los selects se habilitan cuando los datos estan disponibles |
| **Error maestras** | Select deshabilitado, texto "No se pudo cargar". Badge de error inline |
| **Paso X completado** | Circulo del stepper cambia a icono checkmark verde |
| **Abandono** | Al hacer click en X del header o navegar fuera, mostrar `<AlertDialog>` de confirmacion: "Perderas todo el progreso. ¿Deseas salir?" con acciones "Seguir editando" y "Salir sin guardar" |
| **Submitting (paso 4)** | Boton Publicar muestra spinner + "Publicando...", todos los botones del footer deshabilitados, paso 4 con opacidad 70% |
| **Error API** | Toast destructivo en esquina superior derecha. El wizard permanece en paso 4 con datos intactos para reintento |
| **Success** | Toast verde "Programa de promocion creado correctamente", redireccion a `/dashboard/crowdpromotion/programas/{id}` tras 1.5s |

### Validacion en Tiempo Real del Wizard

| Paso | Campo | Validacion | Mensaje de error |
|------|-------|------------|------------------|
| 1 | Titulo | No vacio | "El titulo es obligatorio" |
| 1 | Titulo | Min 5 caracteres | "El titulo debe tener al menos 5 caracteres" |
| 1 | Titulo | Max 200 caracteres | "Maximo 200 caracteres" |
| 1 | Tipo de programa | Seleccion obligatoria | "Selecciona un tipo de programa" |
| 1 | URL landing | Formato URL (si se proporciona) | "Ingresa una URL valida (ej: https://...)" |
| 1 | Codigo tracking | Solo alfanumerico y guiones | "Solo letras, numeros y guiones" |
| 1 | Fecha fin | Mayor que fecha inicio | "La fecha fin debe ser posterior a la fecha inicio" |
| 2 | Moneda | Seleccion obligatoria | "La moneda es obligatoria" |
| 2 | Comision global | Al menos una definida | "Define al menos una comision (porcentaje o fija)" |
| 2 | Comision porcentaje | 0-100 si se proporciona | "El porcentaje debe estar entre 0 y 100" |
| 2 | Comision fija | >= 0 si se proporciona | "El importe debe ser positivo" |
| 3 | Nombre tarea | No vacio + min 3 caracteres | "El nombre de la tarea es obligatorio" |
| 3 | Tipo evento | Seleccion obligatoria | "Selecciona un tipo de evento" |
| 3 | Tipo recompensa | Seleccion obligatoria | "Selecciona un tipo de recompensa" |
| 3 | Importe recompensa | Requerido si tipo Dinero/Mixto | "El importe es obligatorio para este tipo de recompensa" |
| 3 | Moneda tarea | Requerida si importe > 0 | "Selecciona la moneda para el importe" |
| 3 | Puntos | Requerido si tipo Puntos/Mixto | "Los puntos son obligatorios para este tipo de recompensa" |
| 3 | Max repeticiones | >= 1 si es repetible | "Ingresa el maximo de repeticiones (minimo 1)" |

### Zod Schema (Shared)

```typescript
// src/shared/schemas/programa-promocion.schema.ts

const urlOptionalSchema = z
  .string()
  .max(500, "Maximo 500 caracteres")
  .refine(
    (val) => val === "" || z.string().url().safeParse(val).success,
    "Ingresa una URL valida"
  )
  .optional()
  .or(z.literal(""));

export const promoProgramaTareaSchema = z.object({
  nombre: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(200, "Maximo 200 caracteres"),
  descripcion: z.string().max(4000, "Maximo 4000 caracteres").optional(),
  tipoEventoPromoId: z
    .number({ required_error: "Selecciona un tipo de evento" })
    .int()
    .positive(),
  tipoRewardId: z
    .number({ required_error: "Selecciona un tipo de recompensa" })
    .int()
    .positive(),
  importeReward: z.number().min(0).optional(),
  monedaId: z.number().int().positive().optional(),
  puntosReward: z.number().int().min(0).optional(),
  urlInstrucciones: urlOptionalSchema,
  esRepetible: z.boolean().default(true),
  maxRepeticiones: z.number().int().min(1).optional(),
  fechaInicio: z.string().optional(),
  fechaFin: z.string().optional(),
}).refine(
  (data) => {
    if (data.esRepetible && !data.maxRepeticiones) return false;
    return true;
  },
  { message: "Las tareas repetibles requieren max repeticiones", path: ["maxRepeticiones"] }
);

export const promoProgramaStep1Schema = z.object({
  titulo: z
    .string()
    .min(5, "El titulo debe tener al menos 5 caracteres")
    .max(200, "Maximo 200 caracteres"),
  descripcion: z.string().max(4000, "Maximo 4000 caracteres").optional(),
  tipoPromoId: z
    .number({ required_error: "Selecciona un tipo de programa" })
    .int()
    .positive(),
  campaniaCrowdfundingId: z.string().uuid().optional().or(z.literal("")),
  proyectoArtisticoId: z.string().uuid().optional().or(z.literal("")),
  urlLanding: urlOptionalSchema,
  codigoTrackingBase: z
    .string()
    .max(50, "Maximo 50 caracteres")
    .regex(/^[a-zA-Z0-9-]*$/, "Solo letras, numeros y guiones")
    .optional()
    .or(z.literal("")),
  fechaInicio: z.string().optional(),
  fechaFin: z.string().optional(),
});

export const promoProgramaStep2Schema = z.object({
  monedaId: z
    .number({ required_error: "La moneda es obligatoria" })
    .int()
    .positive(),
  importeComisionPorcentaje: z.number().min(0).max(100).optional(),
  importeComisionFija: z.number().min(0).optional(),
}).refine(
  (data) => data.importeComisionPorcentaje != null || data.importeComisionFija != null,
  { message: "Define al menos una comision (porcentaje o fija)", path: ["importeComisionPorcentaje"] }
);

export const createPromoProgramaSchema = promoProgramaStep1Schema
  .merge(promoProgramaStep2Schema)
  .extend({ tareas: z.array(promoProgramaTareaSchema).default([]) });

export type CreatePromoProgramaFormData = z.infer<typeof createPromoProgramaSchema>;
export type PromoTareaFormData = z.infer<typeof promoProgramaTareaSchema>;
```

### Interacciones del Wizard

| Accion | Comportamiento |
|--------|----------------|
| Click boton Siguiente | Validar solo los campos del paso actual con Zod. Si valido: avanzar al siguiente paso con scroll to top. Si invalido: mostrar errores inline |
| Click boton Anterior | Retroceder sin validar (datos del paso actual se conservan) |
| Click icono X | Mostrar `<AlertDialog>` de confirmacion de abandono |
| Click "Editar" en resumen (paso 4) | Retroceder directamente al paso correspondiente |
| Click boton Publicar (paso 4) | POST a `/api/crowdpromotion/programas`, mostrar loading state, en success redirect |
| Click "Guardar tarea" en formulario inline | Validar tarea con Zod, agregar/actualizar en estado local del wizard, colapsar formulario |
| Click "Cancelar" en formulario de tarea | Descartar cambios del formulario inline, colapsar sin guardar |
| Click "Editar" en card de tarea | Expandir formulario inline pre-rellenado con datos de esa tarea |
| Click "Eliminar" en card de tarea | Mostrar `<AlertDialog>` pequeño: "¿Eliminar esta tarea?" con acciones "Cancelar" y "Eliminar". En confirmacion: remover del array local |
| Toggle "Es repetible" OFF | Ocultar campo "Max repeticiones" y limpiar su valor |
| Toggle "Es repetible" ON | Mostrar campo "Max repeticiones" como requerido |
| Cambio en tipo recompensa | Mostrar/ocultar campos Importe, Moneda, Puntos segun la tabla de visibilidad condicional |

---

## Pantalla 2: Listado Mis Programas

**Mockup:** Patron visual de WPR_5-Dashboard-Artist.png (cards en grid + filtros)
**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas`
**Template base:** Dashtail dashboard layout

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR (w-64, bg-[#0d0d1a])  MAIN CONTENT                          │
│ ┌──────────────────────┐  ┌──────────────────────────────────────┐  │
│ │ [Logo WePlay]        │  │ TOPBAR: breadcrumb                   │  │
│ │                      │  │ CrowdPromotion / Mis Programas       │  │
│ │ [Avatar] Alex Rivera │  ├──────────────────────────────────────┤  │
│ │ Artista              │  │                                      │  │
│ │ ─────────────────── │  │  Mis programas de promocion          │  │
│ │ Dashboard            │  │                    [+ Nuevo programa]│  │
│ │ Mis Campanias        │  │                                      │  │
│ │ CrowdPromotion >     │  │  ┌──────────────────────────────┐    │  │
│ │  > Mis Programas     │  │  │ Filtros: [Todos v]  [Buscar] │    │  │
│ │  > Estadisticas      │  │  └──────────────────────────────┘    │  │
│ │ Configuracion        │  │                                      │  │
│ │ [Logout]             │  │  ┌────────────────────────────────┐  │  │
│ └──────────────────────┘  │  │ Card programa 1                │  │  │
│                           │  └────────────────────────────────┘  │  │
│                           │  ┌────────────────────────────────┐  │  │
│                           │  │ Card programa 2                │  │  │
│                           │  └────────────────────────────────┘  │  │
│                           │                                      │  │
│                           │  [Paginacion: < 1 2 3 >]             │  │
│                           └──────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

**Detalle de Card de Programa**

```
┌────────────────────────────────────────────────────────────────────┐
│ ┌─────────────────────────────────────────────────────────┐  [...]  │
│ │ Promociona mi nuevo album                               │        │
│ │ [Referral badge] [ACTIVO badge]                         │        │
│ └─────────────────────────────────────────────────────────┘        │
│                                                                    │
│  Mi Album Debut                      10% comision                  │
│  5 promotores  |  3 tareas           01 Mar 2026 - 01 Jun 2026     │
└────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 2

**Cabecera de pagina**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo de pagina | `<h1>` | `text-3xl font-bold text-white` |
| Subtitulo | `<p>` | `text-sm text-[#94a3b8] mt-1` (ej: "Gestiona tus programas de promocion y sus promotores") |
| Boton nuevo programa | `<Button>` | `bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-5` con icono `<Plus w-4 h-4 mr-2>` |
| Contenedor cabecera | `<div>` | `flex items-start justify-between mb-6` |

**Barra de filtros**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor filtros | `<div>` | `flex items-center gap-3 mb-6` |
| Filtro estado | `<Select>` | `w-[160px] bg-[#0f0f1f] border-[#334155] text-white h-10` |
| Opciones filtro | `<SelectItem>` | "Todos los estados", "Solo activos", "Solo inactivos" |
| Input busqueda | `<div>` con `<Input>` | `relative flex-1 max-w-xs` / Input: `pl-9 bg-[#0f0f1f] border-[#334155] text-white h-10 placeholder:text-[#64748b]` / Icono `<Search w-4 h-4>`: `absolute left-3 top-3 text-[#64748b]` |

**Card de programa**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Card contenedor | `<Card>` | `bg-[#151525] border-[#334155] hover:border-[#a855f7]/50 transition-colors cursor-pointer p-5` |
| Cabecera card | `<div>` | `flex justify-between items-start mb-3` |
| Titulo programa | `<h3>` | `text-base font-semibold text-white` |
| Menu kebab | `<DropdownMenu>` + `<Button variant="ghost" size="sm">` | `h-8 w-8 p-0 text-[#64748b] hover:text-white` con icono `<MoreVertical w-4 h-4>` |
| Opciones menu | `<DropdownMenuItem>` | "Ver detalle", "Editar", "Desactivar" (destructivo: `text-red-400`) |
| Fila de badges | `<div>` | `flex items-center gap-2 mt-1 mb-3` |
| Badge tipo programa | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155] text-xs` |
| Badge ACTIVO | `<Badge>` | `bg-green-950/50 text-green-400 border border-green-800/50 text-xs` |
| Badge INACTIVO | `<Badge>` | `bg-slate-800 text-slate-400 border border-slate-700 text-xs` |
| Nombre campana | `<p>` | `text-sm text-[#94a3b8] mb-3` con icono `<Music w-3 h-3 mr-1.5 inline>` |
| Grid metricas | `<div>` | `grid grid-cols-2 gap-3` |
| Metrica individual | `<div>` | `flex items-center gap-1.5` |
| Icono metrica | Lucide icon | `w-3.5 h-3.5 text-[#64748b]` |
| Valor metrica | `<span>` | `text-sm text-white font-medium` |
| Label metrica | `<span>` | `text-xs text-[#64748b]` |
| Comision | `<span>` | `text-sm font-semibold text-[#a855f7]` |
| Fechas | `<p>` | `text-xs text-[#64748b] mt-3` con icono `<Calendar w-3 h-3 mr-1 inline>` |

**Metricas de la card:**
- Icono `<Users w-3.5 h-3.5>` + valor promotores + "promotores"
- Icono `<ClipboardList w-3.5 h-3.5>` + valor tareas + "tareas"
- Comision: si porcentaje: "X% comision" en accent; si fija: "X EUR/conv" en accent; si ambas: "X% + X EUR"

### Estados de UI Pantalla 2

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | Grid de 3 cards skeleton con `animate-pulse`. Cada skeleton: `bg-[#1e1e38] rounded-xl h-[160px]` |
| **Default con programas** | Grid de cards con datos reales. Paginacion visible si totalCount > pageSize |
| **Sin programas (empty state)** | Centrado en main: icono `<Megaphone w-12 h-12>` con fondo gradient, titulo "No tienes programas de promocion aun", subtitulo "Crea tu primer programa y empieza a reclutar promotores", boton "Crear primer programa" con gradient |
| **Sin resultados (filtro)** | Icono `<SearchX>` + texto "No se encontraron programas con los filtros seleccionados" + boton "Limpiar filtros" |
| **Error de carga** | Card de error con icono `<AlertCircle>`, texto descriptivo y boton "Reintentar" |
| **Card hover** | Border cambia a `#a855f7/50`. Transicion 200ms |
| **Desactivando** | Item del menu "Desactivar" muestra spinner mientras procesa. La card no se desmonta hasta confirmar respuesta de la API |

### Interacciones Pantalla 2

| Accion | Comportamiento |
|--------|----------------|
| Click en card | Navegar a `/dashboard/crowdpromotion/programas/{id}` |
| Click "Ver detalle" en menu | Navegar a `/dashboard/crowdpromotion/programas/{id}` |
| Click "Editar" en menu | Navegar a `/dashboard/crowdpromotion/programas/{id}/editar` |
| Click "Desactivar" en menu | Abrir `<AlertDialog>` de confirmacion de desactivacion |
| Cambio en filtro estado | Refetch de la lista con nuevo parametro `esActivo` |
| Typing en busqueda | Filtro local sobre los items cargados (debounce 300ms). No dispara nueva llamada API |
| Click boton paginacion | Fetch de la pagina correspondiente. Scroll to top |

---

## Pantalla 3: Detalle del Programa

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/{id}`

### Layout

```
┌─────────────────────────────────────────────────────────────────────┐
│ SIDEBAR        MAIN CONTENT                                         │
│                ┌──────────────────────────────────────────────────┐ │
│                │ TOPBAR: breadcrumb                               │ │
│                │ CrowdPromotion / Mis Programas / {titulo}        │ │
│                ├──────────────────────────────────────────────────┤ │
│                │                                                  │ │
│                │  {titulo}                  [Editar] [Desactivar] │ │
│                │  [Referral] [ACTIVO]  Mi Album Debut             │ │
│                │                                                  │ │
│                │  ┌──────────┐ ┌──────────┐ ┌────────┐ ┌───────┐ │ │
│                │  │Promotores│ │Pendientes│ │Eventos │ │Valor  │ │ │
│                │  │aprobados │ │          │ │totales │ │generado│ │ │
│                │  │   [ 5 ]  │ │  [ 2 ]  │ │[ 150 ] │ │1.200€ │ │ │
│                │  └──────────┘ └──────────┘ └────────┘ └───────┘ │ │
│                │                                                  │ │
│                │  [Info General] [Tareas] [Promotores] [Resumen]  │ │
│                │  ──────────────────────────────────────────────  │ │
│                │  CONTENIDO DE TAB ACTIVO                         │ │
│                │                                                  │ │
│                └──────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘
```

### Especificaciones Pantalla 3

**Cabecera del programa**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Titulo | `<h1>` | `text-2xl font-bold text-white` |
| Fila de badges | `<div>` | `flex items-center gap-2 mt-2` |
| Badge tipo | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border border-[#334155]` |
| Badge estado | `<Badge>` | Activo: `bg-green-950/50 text-green-400 border-green-800/50` / Inactivo: `bg-slate-800 text-slate-400 border-slate-700` |
| Campana vinculada | `<p>` | `text-sm text-[#94a3b8] mt-1` con icono `<Music w-3.5 h-3.5 inline mr-1.5>` |
| Contenedor acciones | `<div>` | `flex items-start gap-3` |
| Boton Editar | `<Button variant="outline">` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white` con icono `<Pencil w-4 h-4 mr-2>` |
| Boton Desactivar | `<Button variant="outline">` | `border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300` con icono `<PowerOff w-4 h-4 mr-2>` (visible solo si EsActivo = true) |
| Boton Reactivar | `<Button variant="outline">` | `border-green-800/50 text-green-400 hover:bg-green-950/30 hover:text-green-300` con icono `<Power w-4 h-4 mr-2>` (visible solo si EsActivo = false) |

**KPI Cards del programa**

| KPI | Icono | Color icono | Valor |
|-----|-------|-------------|-------|
| Promotores aprobados | `<UserCheck>` | `#10b981` (verde) | Numero entero |
| Promotores pendientes | `<UserClock>` (o `<Clock>`) | `#f59e0b` (amarillo) | Numero entero |
| Total eventos | `<Activity>` | `#3b82f6` (azul) | Numero entero |
| Valor generado | `<TrendingUp>` | `#a855f7` (purple) | `€{valor}` con 2 decimales |

Estructura de cada KPI card: igual que en WPR_5 (`bg-[#151525] border-[#334155] p-5`, icono en contenedor `rounded-lg w-10 h-10`, valor `text-3xl font-bold text-white`, label `text-sm text-[#94a3b8] mt-1`).

**Tabs de contenido**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Contenedor tabs | `<Tabs defaultValue="info">` | `mt-6` |
| Lista de tabs | `<TabsList>` | `bg-[#0f0f1f] border border-[#334155] p-1 h-10` |
| Tab item | `<TabsTrigger>` | `text-[#94a3b8] data-[state=active]:bg-[#1e1e38] data-[state=active]:text-white rounded-md text-sm` |
| Contenido de tab | `<TabsContent>` | `mt-4` |

**Tab Info General**

Muestra todos los campos del programa en formato lista de lectura, en 2 columnas en desktop.

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Grid de columnas | `<div>` | `grid grid-cols-1 lg:grid-cols-2 gap-4` |
| Card de datos | `<Card>` | `bg-[#151525] border-[#334155] p-5` |
| Titulo card | `<h3>` | `text-sm font-semibold text-[#cbd5e1] uppercase tracking-wider mb-4` |
| Fila de dato | `<div>` | `flex justify-between items-start py-2 border-b border-[#1e1e38] last:border-0` |
| Label dato | `<span>` | `text-sm text-[#64748b]` |
| Valor dato | `<span>` | `text-sm text-white` |
| Valor URL | `<a>` | `text-sm text-[#a855f7] hover:underline truncate max-w-[200px] inline-block` |
| Valor nulo | `<span>` | `text-sm text-[#64748b] italic` con "--" |

**Tab Tareas**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Cabecera tab tareas | `<div>` | `flex justify-between items-center mb-4` |
| Titulo | `<h3>` | `text-base font-semibold text-white` |
| Boton agregar tarea | `<Button size="sm">` | `bg-gradient-to-r from-pink-500 to-purple-600 text-white text-xs h-8 px-3` (solo si programa activo) |
| Lista de tareas | `<div>` | `space-y-3` |
| Card tarea | `<Card>` | `bg-[#0f0f1f] border-[#334155] p-4` |
| Cabecera tarea | `<div>` | `flex justify-between items-start` |
| Nombre tarea | `<h4>` | `text-sm font-semibold text-white` |
| Acciones tarea | `<div>` | `flex gap-2` |
| Boton editar tarea | `<Button variant="ghost" size="sm">` | `text-[#94a3b8] hover:text-white h-7 px-2 text-xs` |
| Toggle activa tarea | `<Switch>` | `data-[state=checked]:bg-[#10b981] scale-75` |
| Badges tarea | `<div>` | `flex gap-2 mt-2 flex-wrap` |
| Badge tipo evento | `<Badge>` | `bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs` |
| Badge tipo recompensa | `<Badge>` | `bg-purple-950/50 text-purple-400 border-purple-800/50 text-xs` |
| Badge repetible | `<Badge>` | `bg-blue-950/50 text-blue-400 border-blue-800/50 text-xs` con icono `<Repeat w-2.5 h-2.5 mr-1>` |
| Completados por promotores | `<p>` | `text-xs text-[#64748b] mt-2` con icono `<CheckCircle w-3 h-3 mr-1 inline text-green-500>` |

**Tab Promotores Inscritos**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Tabla de promotores | `<Table>` | `bg-[#151525] border-[#334155] rounded-lg overflow-hidden` |
| Header de tabla | `<TableHeader>` | `bg-[#0f0f1f]` |
| Header cell | `<TableHead>` | `text-xs font-semibold text-[#64748b] uppercase tracking-wider` |
| Fila datos | `<TableRow>` | `border-[#1e1e38] hover:bg-[#1e1e38] transition-colors` |
| Avatar en tabla | `<Avatar>` | `w-7 h-7` con `<AvatarFallback className="text-xs bg-[#1e1e38] text-[#94a3b8]">` |
| Nombre promotor | `<span>` | `text-sm text-white font-medium ml-2` |
| Badge aprobado | `<Badge>` | `bg-green-950/50 text-green-400 border-green-800/50 text-xs` |
| Badge pendiente | `<Badge>` | `bg-amber-950/50 text-amber-400 border-amber-800/50 text-xs` |
| Badge bloqueado | `<Badge>` | `bg-red-950/50 text-red-400 border-red-800/50 text-xs` |
| Fecha alta | `<span>` | `text-xs text-[#64748b]` |

**Tab Resumen / Metricas**

Muestra las mismas metricas que las KPI cards pero en formato mas detallado. Para el MVP, replicar los valores de KPI con contexto adicional (texto explicativo). Si se implementa chart: linea de eventos a lo largo del tiempo con el mismo estilo que WPR_5-Dashboard-Artist.png (linea purple sobre fondo oscuro, ejes en gris muted).

### Estados de UI Pantalla 3

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading** | Skeleton para titulo, badges, KPI cards y tabs |
| **Default** | Todos los datos cargados. Primera tab (Info General) activa por defecto |
| **Programa inactivo** | Banner amarillo en la parte superior del main area: "Este programa esta desactivado. Los promotores no pueden inscribirse." Boton "Reactivar" en cabecera en lugar de "Desactivar". |
| **Sin promotores** | Tab Promotores muestra empty state: "Aun no hay promotores inscritos en este programa" |
| **Sin tareas** | Tab Tareas muestra empty state con boton "Agregar primera tarea" |
| **Error de carga** | Reemplazar contenido principal con card de error + "Reintentar" |

---

## Pantalla 4: Editar Programa

**Proyecto:** Admin (Next.js 14)
**Ruta:** `/dashboard/crowdpromotion/programas/{id}/editar`

### Layout

Mismo layout que el wizard (pantalla completa con header, stepper y footer), pero el titulo del header cambia a "Editar programa de promocion" y los datos vienen pre-rellenados desde la API.

**Diferencias con el wizard de creacion:**

1. El boton del footer en el ultimo paso dice "Guardar cambios" en lugar de "Publicar programa".
2. Si el programa tiene promotores inscritos, mostrar banner de aviso en la parte superior del contenedor de cada paso.
3. En el Paso 3, las tareas que ya tienen completados no pueden eliminarse: el boton "Eliminar" aparece deshabilitado con tooltip "Esta tarea tiene completados registrados y no puede eliminarse".
4. La carga inicial hace GET al endpoint de detalle para pre-rellenar el estado del wizard.

```
┌──────────────────────────────────────────────────────────────────────┐
│ [Logo]  Editar programa de promocion                    [X Cancelar] │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  [!] Este programa tiene 5 promotores inscritos. Los cambios en las  │
│      comisiones se aplicaran a nuevas inscripciones unicamente.      │
│                                                                      │
│   STEPPER (igual que en creacion)                                    │
│                                                                      │
│   CONTENIDO DEL PASO (pre-rellenado con datos existentes)            │
│                                                                      │
├──────────────────────────────────────────────────────────────────────┤
│ [<- Anterior]              Paso X de 4        [Guardar cambios ->]   │
└──────────────────────────────────────────────────────────────────────┘
```

**Especificaciones adicionales Pantalla 4**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Banner aviso promotores | `<div>` con icono `<AlertTriangle>` | `flex items-start gap-3 p-4 rounded-lg bg-amber-950/40 border border-amber-800/50 text-sm text-amber-300 mb-6 mx-8` |
| Texto banner | `<p>` | `text-sm text-amber-300` |
| Boton Eliminar tarea deshabilitado | `<Button variant="ghost" size="sm" disabled>` | `text-[#64748b] cursor-not-allowed opacity-50` / Envuelto en `<TooltipProvider>` + `<Tooltip>` + `<TooltipContent>`: "Esta tarea tiene completados registrados y no puede eliminarse" |
| Boton Guardar cambios | `<Button>` | Igual que "Publicar programa" pero con texto "Guardar cambios" e icono `<Save w-4 h-4 mr-2>` |

### Estados de UI Pantalla 4

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Loading inicial** | Skeleton en todos los campos mientras se carga el GET del programa |
| **Con promotores inscritos** | Banner amarillo visible en todos los pasos |
| **Sin promotores** | Sin banner. Todos los campos completamente editables |
| **Tarea con completados** | Boton Eliminar deshabilitado con tooltip. Toggle de tarea activa/inactiva si es posible |
| **Guardando** | Boton "Guardar cambios" con spinner + "Guardando...", footer deshabilitado |
| **Success** | Toast verde "Programa actualizado correctamente", redirect a `/dashboard/crowdpromotion/programas/{id}` |
| **Error API** | Toast destructivo. Datos intactos para reintento |

---

## Pantalla 5: Dialogo de Desactivacion

**Componente:** `<AlertDialog>` de shadcn/ui
**Contexto:** Se abre desde la pantalla de listado (menu kebab) o desde la cabecera del detalle del programa.

```
┌─────────────────────────────────────────────────────────┐
│  Desactivar programa de promocion                       │
│                                                         │
│  [!] Esta accion desactivara el programa y todas        │
│      sus tareas activas.                                │
│                                                         │
│  Impacto:                                               │
│  - X tareas activas se desactivaran                     │
│  - El programa dejara de aparecer a nuevos promotores   │
│  - Los promotores actuales mantienen sus registros      │
│                                                         │
│  Los eventos historicos seguiran siendo rastreables.    │
│                                                         │
│                [Cancelar]  [Desactivar programa]        │
└─────────────────────────────────────────────────────────┘
```

**Especificaciones Pantalla 5**

| Elemento | Componente | Estilos / Props |
|----------|------------|-----------------|
| Dialogo contenedor | `<AlertDialog>` | `open={isOpen} onOpenChange={setIsOpen}` |
| Overlay | `<AlertDialogOverlay>` | `bg-black/60 backdrop-blur-sm` |
| Contenido | `<AlertDialogContent>` | `bg-[#151525] border border-[#334155] max-w-md` |
| Titulo | `<AlertDialogTitle>` | `text-lg font-semibold text-white` |
| Icono de alerta | `<AlertTriangle>` lucide | `w-5 h-5 text-amber-400 mr-2 inline-block` |
| Descripcion | `<AlertDialogDescription>` | `text-sm text-[#94a3b8] mt-2` |
| Lista de impacto | `<ul>` | `mt-3 space-y-1.5` |
| Item de impacto | `<li>` | `flex items-start gap-2 text-sm text-[#94a3b8]` con icono `<AlertCircle w-3.5 h-3.5 text-amber-400 mt-0.5 flex-shrink-0>` |
| Texto historico | `<p>` | `text-xs text-[#64748b] mt-3 italic` |
| Contenedor botones | `<AlertDialogFooter>` | `flex gap-3 mt-6` |
| Boton Cancelar | `<AlertDialogCancel>` | `border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white flex-1` |
| Boton Desactivar | `<AlertDialogAction>` | `bg-red-600 hover:bg-red-700 text-white font-semibold flex-1` con icono `<PowerOff w-4 h-4 mr-2>` |
| Spinner boton | `<Loader2>` | `w-4 h-4 animate-spin mr-2` (visible durante PATCH) |

El numero de tareas activas ("X tareas activas") se obtiene del estado local disponible al momento de abrir el dialogo (data del GET de detalle o del GET de listado).

### Estados de UI Pantalla 5

| Estado | Comportamiento Visual |
|--------|----------------------|
| **Default** | Dialogo visible con datos de impacto. Ambos botones habilitados |
| **Confirmando** | Boton "Desactivar programa" muestra spinner + "Desactivando...", boton Cancelar deshabilitado |
| **Success** | Dialogo se cierra, toast verde "Programa desactivado correctamente". En listado: la card actualiza badge a INACTIVO. En detalle: banner aparece + boton cambia a "Reactivar" |
| **Error** | Toast destructivo. Dialogo se cierra. Datos intactos |

---

## Responsive Breakpoints

Todas las pantallas son parte del Admin (Next.js 14), que es primarily desktop. No obstante, se aplica responsive para uso movil ocasional.

| Breakpoint | Ancho | Cambios de layout |
|------------|-------|-------------------|
| **Mobile** | < 768px | Sidebar colapsa a drawer (hamburger menu). Wizard toma ancho completo. Grid de KPI cards pasa a 2 columnas. Grid de metricas en cards de programa: 1 columna. Tabs en detalle: scroll horizontal |
| **Tablet** | 768px - 1024px | Sidebar visible reducida (solo iconos, w-16). Wizard `max-w-xl`. Grid de KPI: 2 columnas (2+2). Listado de programas: 1 columna |
| **Desktop** | > 1024px | Layout completo. Sidebar w-64. Wizard `max-w-2xl`. Grid de KPI: 4 columnas. Listado: 1 columna de cards full-width (o 2 columnas en pantallas muy anchas) |

**Comportamiento del sidebar en mobile:**
- Boton hamburger `<Menu w-5 h-5>` en topbar a la izquierda
- Sidebar se abre como sheet desde la izquierda: `<Sheet>` de shadcn con contenido identico al sidebar desktop
- Overlay con `bg-black/40`

**Wizard en mobile:**
- Footer sticky: botones ocupan ancho completo en columna vertical (`flex-col gap-2`)
- Campos del formulario: todo en 1 columna (grids de 2 col se convierten en 1 col)
- Stepper: se reduce a mostrar solo el paso actual y las flechas (`Paso X de 4`)
- Formulario inline de tarea: panel que ocupa ancho completo con scroll

---

## Animaciones

| Elemento | Animacion | Duracion | Implementacion |
|----------|-----------|----------|----------------|
| Avance de paso en wizard | Fade in + slide desde derecha | 200ms | `transition-all duration-200 ease-in-out` |
| Retroceso de paso en wizard | Fade in + slide desde izquierda | 200ms | `transition-all duration-200 ease-in-out` |
| Apertura formulario de tarea inline | Expand height + fade in | 250ms | `animate-accordion-down` (shadcn accordion) o Framer Motion `AnimatePresence` |
| Cierre formulario de tarea inline | Collapse height + fade out | 200ms | `animate-accordion-up` o Framer Motion |
| Card de programa hover | Border color transition | 200ms | `transition-colors duration-200` |
| Badge de estado | Sin animacion | - | Estatico |
| Toast (success/error) | Slide in desde abajo-derecha | 300ms | Nativo de shadcn `<Toaster>` |
| Dialogo desactivacion | Fade in + scale up ligero | 200ms | Nativo de shadcn `<AlertDialog>` |
| Stepper cambio de paso | Color/fondo del circulo con transition | 300ms | `transition-all duration-300` |
| Boton hover (gradient) | Brillo ligero | 150ms | `transition-opacity duration-150` |
| Skeleton loading | Pulse | Loop | `animate-pulse` |
| Tab cambio | Fade in | 150ms | `data-[state=active]:animate-in data-[state=active]:fade-in-50` |

---

## Accesibilidad

| Requisito | Implementacion |
|-----------|----------------|
| Contraste de texto | Minimo 4.5:1 para texto normal (blanco `#ffffff` sobre `#151525` = 14:1). Texto muted `#94a3b8` sobre `#151525` = 4.6:1 |
| Contraste de badges | Texto verde `#10b981` sobre `#151525`: 5.1:1. Texto rojo `#ef4444` sobre `#151525`: 4.9:1 |
| Focus visible | Todos los elementos interactivos: `focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#151525]` |
| Labels de formulario | `<Label htmlFor={id}>` siempre vinculado a su `<Input id={id}>`. Campos requeridos con `aria-required="true"` |
| Errores de validacion | `<p role="alert" aria-live="polite">` debajo del campo. Input con `aria-invalid="true"` y `aria-describedby={errorId}` cuando hay error |
| Stepper accesible | `<nav aria-label="Progreso del wizard">` con lista `<ol>`. Paso actual con `aria-current="step"` |
| Dialogo desactivacion | Focus atrapado dentro del `<AlertDialog>`. Escape lo cierra. Primer foco al boton Cancelar |
| Menu kebab | `<DropdownMenu>` con `aria-haspopup="true"` y `aria-expanded`. Items con roles correctos |
| Imagenes decorativas | `aria-hidden="true"` en iconos decorativos. Iconos funcionales con `aria-label` |
| Toggle es repetible | `<Switch>` con `aria-label="La tarea es repetible"` |
| Tabs | `<Tabs>` de shadcn implementa `role="tablist"`, `role="tab"`, `role="tabpanel"` correctamente |
| Skeleton | `aria-busy="true"` en el contenedor mientras carga. `aria-label="Cargando..."` |
| Mensajes de toast | `role="status"` para success, `role="alert"` para errores |
| Tabla de promotores | `<Table>` con `<caption>` descriptivo. Headers con `scope="col"` |
| Boton desactivar tarea (disabled) | `disabled` + `aria-disabled="true"` + `<TooltipContent>` descriptivo |

---

## Checklist UI/UX

### Pantalla 1 - Wizard Crear Programa
- [ ] Header del wizard con logo y boton X
- [ ] Stepper de 4 pasos con estados visual: activo, completado, futuro
- [ ] Footer fijo con botones Anterior / Siguiente / Publicar
- [ ] Paso 1: Datos Basicos - todos los campos implementados
- [ ] Paso 1: Selects con datos de maestras cargados desde API
- [ ] Paso 1: Validacion inline en Zod antes de avanzar
- [ ] Paso 2: Comisiones con vista previa en tiempo real
- [ ] Paso 2: Validacion de al menos una comision
- [ ] Paso 3: Lista de tareas con add/edit/remove inline
- [ ] Paso 3: Formulario inline con visibilidad condicional de campos de recompensa
- [ ] Paso 3: Toggle es repetible con campo max repeticiones condicional
- [ ] Paso 4: Resumen completo con botones "Editar" por seccion
- [ ] Paso 4: Boton "Publicar programa" con loading state
- [ ] AlertDialog de confirmacion al abandonar el wizard
- [ ] Toast success con redirect a detalle
- [ ] Toast error con datos intactos

### Pantalla 2 - Listado Mis Programas
- [ ] Cabecera con titulo y boton "Nuevo programa"
- [ ] Filtro por estado (Todos / Solo activos / Solo inactivos)
- [ ] Busqueda por titulo (filtro local)
- [ ] Cards con toda la informacion: titulo, badges, campana, metricas, comision, fechas
- [ ] Menu kebab con acciones: Ver detalle, Editar, Desactivar
- [ ] Empty state cuando no hay programas
- [ ] Empty state cuando no hay resultados con filtro
- [ ] Loading skeleton con animate-pulse
- [ ] Paginacion funcional
- [ ] Error state con boton Reintentar

### Pantalla 3 - Detalle del Programa
- [ ] Cabecera con titulo, badges, campana, botones Editar y Desactivar/Reactivar
- [ ] 4 KPI cards: promotores aprobados, pendientes, eventos totales, valor generado
- [ ] Tabs: Info General, Tareas, Promotores, Resumen
- [ ] Tab Info General: todos los campos en 2 columnas
- [ ] Tab Tareas: lista con badges y toggle activa
- [ ] Tab Promotores: tabla con avatar, nombre, estado, fecha
- [ ] Tab Resumen: metricas detalladas
- [ ] Banner de programa inactivo cuando EsActivo = false
- [ ] Loading skeleton
- [ ] Empty states en tabs

### Pantalla 4 - Editar Programa
- [ ] Mismo wizard con datos pre-rellenados
- [ ] Banner de aviso si hay promotores inscritos
- [ ] Boton Eliminar deshabilitado con tooltip para tareas con completados
- [ ] Boton "Guardar cambios" en lugar de "Publicar"
- [ ] Loading inicial mientras carga datos del GET
- [ ] Toast success con redirect a detalle

### Pantalla 5 - Dialogo Desactivacion
- [ ] AlertDialog con titulo, descripcion e impacto
- [ ] Numero de tareas activas mostrado dinamicamente
- [ ] Boton Desactivar con estilo destructivo (rojo)
- [ ] Loading state en boton durante PATCH
- [ ] Toast success + actualizacion del badge en la vista anterior
- [ ] Focus trap y cierre con Escape

### General
- [ ] Responsive: mobile con sidebar como drawer
- [ ] Responsive: wizard en 1 columna en mobile
- [ ] Todos los estados de UI (loading, error, success, empty) implementados
- [ ] Tokens de diseño aplicados consistentemente (no colores hardcodeados fuera de tokens)
- [ ] Focus ring visible en todos los elementos interactivos
- [ ] Labels vinculados a inputs con htmlFor/id
- [ ] Errores de validacion con role="alert"
- [ ] Skeleton con aria-busy durante carga
- [ ] Transiciones y animaciones aplicadas
