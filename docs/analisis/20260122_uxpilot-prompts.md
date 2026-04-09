# UXPilot Prompts - WePlay Rises

> Prompts para generar diseños UI con [UXPilot.ai](https://uxpilot.ai/es)
>
> **Stack:** React 19 + TypeScript + Tailwind CSS + shadcn/ui

---

## Prompt Master - Sistema de Diseño

```markdown
# WePlay Rises - Sistema de Diseño UI

## Descripción del Proyecto
Plataforma de crowdfunding musical donde artistas pueden crear campañas para financiar
sus proyectos musicales y los fans pueden apoyarlos a cambio de recompensas exclusivas.

## Stack Tecnológico
- **Framework:** React 19 con TypeScript
- **UI Library:** shadcn/ui (componentes basados en Radix UI)
- **Styling:** Tailwind CSS
- **Iconos:** Lucide Icons

## Identidad Visual
- **Estilo:** Moderno, profesional, con toques creativos/musicales
- **Paleta principal:**
  - Primary: Violeta/Púrpura (#7C3AED - violet-600)
  - Secondary: Rosa (#EC4899 - pink-500)
  - Accent: Cyan (#06B6D4 - cyan-500)
  - Neutrals: Slate grays
- **Modo:** Dark mode como principal, soporte para light mode
- **Tipografía:** Inter para UI, opcionalmente una display font para headings

## Componentes shadcn/ui a Usar
- Button, Card, Input, Textarea, Select
- Dialog, Sheet, Tabs, Accordion
- Avatar, Badge, Progress
- Form (con React Hook Form + Zod)
- Table, DataTable
- Toast, Alert
- Navigation Menu, Dropdown Menu

## Patrones de Diseño
- Cards con hover effects sutiles y sombras
- Gradientes para CTAs principales (violet → pink)
- Bordes redondeados (rounded-lg/xl)
- Espaciado generoso (Tailwind spacing scale)
- Iconografía consistente con Lucide
- Feedback visual claro (loading states, success/error)

## Entidades Principales
1. **Artista:** Creador de campañas (nombre artístico, bio, imagen)
2. **Campaña:** Proyecto a financiar (título, descripción, meta, fecha fin, estado)
3. **Reward:** Recompensas por nivel de aporte (nombre, descripción, monto mínimo, stock)
4. **Backing:** Aporte de un fan (monto, reward seleccionado)

## Flujos Principales
1. Fan descubre campaña → Ve detalles → Selecciona reward → Hace backing
2. Artista se registra → Crea perfil → Crea campaña → Añade rewards → Publica
3. Artista ve dashboard → Métricas → Lista de backers
```

---

## Pantallas

### 1. Landing Page

```markdown
## Pantalla: Landing Page - WePlay Rises

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode principal

### Descripción
Página de inicio de plataforma de crowdfunding musical. Debe transmitir energía
creativa y confianza para artistas y fans.

### Secciones

1. **Hero Section**
   - Headline impactante: "Financia la música que amas"
   - Subheadline: breve descripción del concepto
   - 2 CTAs: "Explorar Campañas" (primary gradient button) y "Soy Artista" (outline)
   - Imagen/ilustración de artista con fans o elemento musical abstracto
   - Stats: "X artistas financiados", "X€ recaudados"

2. **Campañas Destacadas**
   - Grid de 3-4 cards de campañas
   - Cada card: imagen, título, artista, barra de progreso, % completado, días restantes
   - Badge de categoría (Rock, Pop, Jazz, etc.)
   - Botón "Ver todas"

3. **Cómo Funciona**
   - 3 pasos con iconos (Para Artistas / Para Fans)
   - Iconos de Lucide: Music, Heart, Gift
   - Diseño limpio con iconos en círculos con gradiente

4. **Testimonios/Social Proof**
   - 2-3 quotes de artistas exitosos
   - Avatar, nombre, monto recaudado

5. **CTA Final**
   - Banner con gradiente violet→pink
   - "¿Tienes un proyecto musical?" + botón registro

6. **Footer**
   - Links: Sobre nosotros, FAQ, Contacto, Legal
   - Redes sociales
   - Copyright

### Estilo Visual
- Fondo oscuro (slate-900/950)
- Acentos de color en CTAs y elementos destacados
- Subtle grid o noise texture en background
- Cards con glass morphism effect sutil
```

---

### 2. Explorar Campañas

```markdown
## Pantalla: Explorar Campañas

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Página de listado de todas las campañas activas con filtros y búsqueda.

### Layout
- Header con navegación (logo, links, botones auth)
- Barra de búsqueda prominente
- Filtros laterales o dropdown
- Grid de cards de campañas

### Componentes

1. **Search Bar**
   - Input grande con icono Search
   - Placeholder: "Buscar artistas, géneros, proyectos..."
   - shadcn Input component

2. **Filtros** (shadcn Select/DropdownMenu)
   - Género musical: Rock, Pop, Jazz, Electrónica, Hip-Hop, Clásica, Otro
   - Estado: Activas, Próximamente, Financiadas
   - Ordenar por: Más recientes, Más apoyadas, Casi financiadas, Terminan pronto

3. **Grid de Campañas** (responsive: 1-2-3-4 columnas)
   - Card de campaña:
     - Imagen de portada (aspect-video)
     - Badge género (top-left)
     - Badge días restantes (top-right)
     - Título de campaña
     - Nombre artista con avatar pequeño
     - Progress bar (shadcn Progress)
     - Stats: "€X de €Y" | "X% financiado" | "X backers"
     - Hover: elevación sutil, border glow

4. **Paginación o Infinite Scroll**
   - Botón "Cargar más" estilo minimal

5. **Empty State**
   - Si no hay resultados: ilustración + mensaje + sugerencia de quitar filtros

### Estilo
- Cards con fondo slate-800, borde slate-700
- Progress bar con gradiente violet→pink cuando >80%
- Badges con colores por género
```

---

### 3. Detalle de Campaña

```markdown
## Pantalla: Detalle de Campaña

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Página individual de una campaña con toda la información, rewards disponibles
y opción de hacer backing.

### Layout (2 columnas en desktop)
- Columna principal (2/3): Info de campaña
- Sidebar sticky (1/3): Rewards y CTA

### Columna Principal

1. **Header de Campaña**
   - Imagen/video hero (aspect-video, rounded-xl)
   - Título grande (text-3xl font-bold)
   - Artista: avatar + nombre (link al perfil)
   - Badges: género, estado (Activa/Financiada)

2. **Progress Section**
   - Monto recaudado grande: "€12,450"
   - Meta: "de €15,000"
   - Progress bar grande (h-3)
   - Stats en row: "83% financiado" | "127 backers" | "12 días restantes"

3. **Tabs de Contenido** (shadcn Tabs)
   - Tab "Historia": descripción larga, imágenes, video embebido
   - Tab "Actualizaciones": lista de posts del artista (fecha + contenido)
   - Tab "Comentarios": lista de comentarios de backers
   - Tab "FAQ": acordeón con preguntas frecuentes

4. **Sobre el Artista**
   - Card con avatar grande, nombre, bio corta
   - Stats: campañas creadas, total recaudado
   - Botón "Ver perfil"

### Sidebar (sticky)

1. **Card Principal de Backing**
   - Botón grande: "Apoyar esta campaña"
   - Texto: "Desde €5"

2. **Lista de Rewards** (shadcn Card para cada uno)
   - Para cada reward:
     - Monto mínimo destacado: "€25 o más"
     - Nombre del reward
     - Descripción
     - Stock: "X de Y disponibles" o "Ilimitado"
     - Botón "Seleccionar"
     - Badge "Más popular" si aplica

3. **Info de Confianza**
   - Icono Shield: "Pago seguro"
   - Icono Clock: "Entrega estimada: Marzo 2026"

### Estilo
- Contenido principal con fondo más claro (slate-900)
- Sidebar con cards más oscuras (slate-800)
- Rewards con hover border violet
```

---

### 4. Login / Register

```markdown
## Pantalla: Login y Register

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Páginas de autenticación minimalistas y elegantes.

### Layout
- Centrado en pantalla
- Logo arriba
- Card con formulario
- Fondo con gradiente sutil o pattern musical

### Componentes

**Login:**
- Título: "Bienvenido de vuelta"
- Subtítulo: "Inicia sesión para continuar"
- Form:
  - Input Email (shadcn Input con icono Mail)
  - Input Password (con toggle visibility)
  - Checkbox "Recordarme"
  - Link "¿Olvidaste tu contraseña?"
- Button "Iniciar sesión" (gradient, full width)
- Divider "o continúa con"
- Botones social: Google, Spotify (outline)
- Footer: "¿No tienes cuenta?" + link a Register

**Register:**
- Título: "Crea tu cuenta"
- Subtítulo: "Únete a la comunidad musical"
- Form:
  - Input Nombre completo
  - Input Email
  - Input Password (con indicador de fuerza)
  - Input Confirmar password
  - Checkbox "Soy artista" (toggle que muestra campos extra)
  - Checkbox términos y condiciones
- Button "Crear cuenta" (gradient)
- Divider social login
- Footer: "¿Ya tienes cuenta?" + link a Login

### Estilo
- Card con glass morphism sobre fondo con gradiente
- Inputs con fondo slate-800, border slate-700
- Focus states con ring violet
- Error states con borde y texto red-500
```

---

### 5. Dashboard Artista

```markdown
## Pantalla: Dashboard Artista

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Panel de control para artistas. Vista general de sus campañas y métricas.

### Layout
- Sidebar izquierdo fijo (collapsible en mobile)
- Área de contenido principal

### Sidebar (shadcn Sheet en mobile)
- Logo WePlay Rises
- Avatar + nombre artista
- Navegación:
  - Dashboard (icono LayoutDashboard)
  - Mis Campañas (icono Music)
  - Crear Campaña (icono Plus)
  - Mis Backers (icono Users)
  - Configuración (icono Settings)
- Divider
- Botón "Ver mi perfil público"
- Logout

### Contenido Principal

1. **Header**
   - Saludo: "Hola, [Nombre]"
   - Fecha actual
   - Botón "Nueva campaña" (gradient)

2. **Stats Cards** (grid 4 columnas)
   - Total recaudado (icono DollarSign, valor grande, tendencia %)
   - Backers totales (icono Users)
   - Campañas activas (icono Music)
   - Tasa de éxito (icono TrendingUp)

3. **Gráfico de Recaudación**
   - Line chart simple (últimos 30 días)
   - shadcn Card contenedor

4. **Mis Campañas** (shadcn Table o Cards)
   - Columnas: Imagen, Título, Estado (Badge), Progreso, Backers, Acciones
   - Estados: Borrador (gray), Activa (green), Finalizada (blue), Cancelada (red)
   - Acciones: Ver, Editar, Publicar (si borrador)

5. **Últimos Backings**
   - Lista de 5 backings recientes
   - Avatar fan, monto, reward, fecha
   - Link "Ver todos"

### Estilo
- Fondo general slate-950
- Cards con slate-900
- Sidebar slate-900 con border-r slate-800
- Hover states en nav items
```

---

### 6. Crear/Editar Campaña (Wizard)

```markdown
## Pantalla: Crear Campaña (Wizard)

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Formulario wizard de múltiples pasos para crear una campaña.

### Layout
- Header con pasos del wizard (stepper)
- Contenido del paso actual
- Footer con navegación (Anterior/Siguiente)

### Stepper (4 pasos)
1. Información básica
2. Historia del proyecto
3. Recompensas
4. Revisión y publicar

### Paso 1: Información Básica
- Input: Título de la campaña*
- Select: Género musical*
- Input: Meta de financiación (€)*
- Date picker: Fecha de finalización*
- Input: URL del video (YouTube/Vimeo)
- Upload: Imagen de portada*
- Preview thumbnail

### Paso 2: Historia del Proyecto
- Textarea grande: Descripción corta (máx 200 chars)
- Rich text editor simple: Historia completa
- Upload: Galería de imágenes (hasta 5)
- Tips en sidebar: "Cuenta tu historia personal", "Explica cómo usarás el dinero"

### Paso 3: Recompensas
- Lista de rewards creados (cards)
- Botón "Añadir recompensa" (abre Dialog)
- Dialog de reward:
  - Input: Nombre*
  - Textarea: Descripción*
  - Input: Monto mínimo (€)*
  - Input: Stock (opcional, 0 = ilimitado)
  - Input: Fecha estimada de entrega
- Drag & drop para reordenar
- Mínimo 1 reward requerido

### Paso 4: Revisión
- Preview completo de cómo se verá la campaña
- Checklist de validación
- Términos y condiciones checkbox
- Botón "Guardar como borrador" (secondary)
- Botón "Publicar campaña" (gradient, con confirm dialog)

### Estilo
- Stepper con círculos numerados, línea conectora
- Paso activo: violet, completado: green, pendiente: gray
- Formularios en cards con padding generoso
- Validación inline con mensajes de error
```

---

### 7. Proceso de Backing

```markdown
## Pantalla: Hacer Backing

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Flujo de checkout para hacer backing a una campaña. Modal o página dedicada.

### Opción: Sheet lateral o Dialog grande

### Contenido

1. **Header**
   - Título: "Apoya a [Nombre Artista]"
   - Nombre de campaña
   - Botón cerrar (X)

2. **Resumen del Reward Seleccionado**
   - Card con: nombre, descripción corta, monto mínimo
   - Opción de cambiar reward

3. **Monto a Aportar**
   - Input numérico grande
   - Texto: "Mínimo €X para este reward"
   - Botones rápidos: +€5, +€10, +€25
   - Slider alternativo

4. **Información de Entrega** (si reward físico)
   - Form:
     - Nombre completo
     - Dirección
     - Ciudad, CP, País
   - Checkbox "Guardar para futuras compras"

5. **Método de Pago**
   - Radio buttons o Tabs:
     - Tarjeta de crédito (icono CreditCard)
     - PayPal (logo)
   - Form tarjeta:
     - Número de tarjeta
     - Fecha expiración (MM/YY)
     - CVV
   - Texto: "Pago seguro con encriptación SSL"

6. **Resumen Final**
   - Aportación: €50
   - Reward: "Disco firmado"
   - Total: €50
   - Nota: "Se cobrará solo si la campaña alcanza su meta"

7. **Botón Final**
   - "Confirmar aportación de €50" (gradient, grande)
   - Términos debajo

### Confirmación (después del pago)
- Icono check grande con animación
- "¡Gracias por tu apoyo!"
- Resumen de lo que obtendrá
- Botón compartir en redes
- Botón "Ver mis backings"

### Estilo
- Formulario limpio con secciones claramente separadas
- Progress en header mostrando pasos (1. Monto, 2. Datos, 3. Pago)
- Estados de loading durante procesamiento
```

---

### 8. Perfil Público de Artista

```markdown
## Pantalla: Perfil Público de Artista

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Página pública del artista visible para fans y visitantes.

### Layout
- Header con cover image
- Contenido en container centrado

### Secciones

1. **Hero/Cover**
   - Imagen de portada ancha (h-48)
   - Avatar grande superpuesto (bottom, -mb-16)
   - Nombre artístico grande
   - Género/estilo musical (badges)
   - Stats: X campañas | X backers | €X recaudados
   - Botones: "Seguir" (si logueado), redes sociales

2. **Biografía**
   - Card con texto de bio
   - Links a Spotify, YouTube, etc.

3. **Campañas del Artista** (Tabs)
   - Tab "Activas": grid de cards de campañas en curso
   - Tab "Pasadas": campañas finalizadas con badge "Financiada" o "No alcanzó meta"

4. **Galería/Media** (opcional)
   - Grid de imágenes
   - Videos embebidos

5. **Reseñas de Backers** (opcional)
   - Testimonios de fans que han apoyado
   - Avatar, nombre, comentario, rating

### Estilo
- Cover con overlay gradiente para legibilidad
- Avatar con borde grueso (border-4)
- Cards de campañas con diseño consistente con explorar
```

---

### 9. Mis Backings (Fan)

```markdown
## Pantalla: Mis Backings

**Usar:** shadcn/ui + Tailwind CSS + Lucide Icons
**Modo:** Dark mode

### Descripción
Historial de backings del fan/usuario. Accesible desde su perfil/dashboard.

### Layout
- Parte del dashboard de usuario (sidebar + contenido)
- O página standalone con header

### Contenido

1. **Header**
   - Título: "Mis Apoyos"
   - Stats: "Has apoyado X proyectos por un total de €X"

2. **Filtros**
   - Tabs o select: Todos | Activos | Completados | Pendientes

3. **Lista de Backings** (Cards o Table)

   Para cada backing:
   - Imagen de campaña (thumbnail)
   - Nombre de campaña + artista
   - Monto aportado: "€50"
   - Reward: "Disco firmado + Gracias en créditos"
   - Estado de campaña (badge):
     - "En curso" (blue)
     - "Financiada" (green)
     - "No alcanzó meta" (gray)
   - Estado de reward:
     - "Pendiente" (campaña en curso)
     - "En preparación" (campaña financiada, artista preparando)
     - "Enviado" (con tracking si disponible)
     - "Entregado"
   - Fecha del backing
   - Acciones: Ver campaña, Contactar artista

4. **Empty State**
   - Si no hay backings: ilustración + "Aún no has apoyado ningún proyecto"
   - CTA: "Explorar campañas"

### Estilo
- Cards con estructura clara de información
- Badges de color según estado
- Timeline visual del progreso del reward (opcional)
```

---

## Resumen de Pantallas

| # | Pantalla | Tipo | Prioridad |
|---|----------|------|-----------|
| 1 | Landing Page | Pública | Alta |
| 2 | Explorar Campañas | Pública | Alta |
| 3 | Detalle de Campaña | Pública | Alta |
| 4 | Login/Register | Auth | Alta |
| 5 | Dashboard Artista | Privada | Alta |
| 6 | Crear Campaña (Wizard) | Privada | Alta |
| 7 | Proceso de Backing | Privada | Alta |
| 8 | Perfil Público Artista | Pública | Media |
| 9 | Mis Backings | Privada | Media |

---

## Notas de Uso

1. **Copiar el Prompt Master** primero para establecer el contexto del sistema de diseño
2. **Usar cada prompt de pantalla** individualmente para generar el mockup específico
3. **Iterar** pidiendo ajustes específicos si el resultado no es exacto
4. Los prompts están diseñados para **shadcn/ui + Tailwind** - UXPilot debería generar código compatible
