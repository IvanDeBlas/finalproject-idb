# Plan de Desarrollo por Slices Verticales

- **Fecha:** 2026-01-22
- **Proyecto:** WePlay Rises - MVP Crowdfunding Musical
- **Metodología:** Vertical Slices (API + Admin + Web en cada iteración)
- **Sesiones:** 2 horas cada una
- **Total estimado:** 20-24 horas

---

## Resumen Ejecutivo

| # | Slice | Horas | Acumulado | Entregable |
|---|-------|-------|-----------|------------|
| 1 | Auth | 2-3h | 3h | Usuario puede registrarse y loguearse |
| 2 | Dashboard + Mis Campañas | 2h | 5h | Artista ve su dashboard |
| 3 | Crear Campaña | 2-3h | 8h | Artista puede crear campañas |
| 4 | Explorar Campañas | 2h | 10h | Público ve landing con campañas |
| 5 | Detalle Campaña | 2h | 12h | Click en campaña muestra detalles |
| 6 | Hacer Backing | 2-3h | 15h | Fan puede apoyar campaña |
| 7 | Mis Backings | 2h | 17h | Fan ve historial de apoyos |
| 8 | CRUD Rewards | 2h | 19h | Artista gestiona rewards |
| 9 | Editar Campaña | 1-2h | 21h | Artista puede modificar campañas |
| 10 | Pulido + Deploy | 3-4h | 24h | MVP en producción |

---

## Referencias Visuales (UXPilot)

Imágenes de diseño generadas con UXPilot disponibles en `docs/ui-images/`:

| Imagen | Pantalla | Slice |
|--------|----------|-------|
| `WPR_1-Landing.png` | Landing principal | Slice 4 |
| `WPR_2-Excplore-Campaigns.png` | Explorar campañas | Slice 4 |
| `WPR_3-Detail-Campaign.png` | Detalle de campaña | Slice 5 |
| `WPR_4-Login.png` | Login | Slice 1 |
| `WPR_5-Dashboard-Artist.png` | Dashboard artista | Slice 2 |
| `WPR_6-Create-Campaign.png` | Crear campaña (wizard) | Slice 3 |
| `WPR_7-Backing.png` | Flujo de backing | Slice 6 |
| `WPR_8-Artist-Profile.png` | Perfil público artista | Slice 4 (opcional) |
| `WPR_9-Mis-backings.png` | Mis apoyos (fan) | Slice 7 |

---

## Estado Inicial del Proyecto

### Base de Datos (Azure SQL)
- ✅ 73 tablas creadas
- ✅ Tablas maestras con seed (estados, monedas, tipos)
- ✅ Identity tables listas
- 🔴 0 datos de negocio (artistas, campañas)

### Backend (.NET 8)
- ✅ Entidades de dominio definidas
- ✅ DbContexts configurados (UserAccess, Crowdfunding)
- 🔴 Sin CQRS implementado (Commands/Queries/Handlers)
- 🔴 Sin Controllers

### Frontend
- ✅ Estructura de proyectos (web: Vite, admin: Next.js)
- ✅ Componentes shadcn/ui instalados
- ✅ Services y tipos definidos
- 🔴 Sin conexión a API real

---

## Slice 1: Autenticación

**Estimación:** 2-3 horas

### Referencia Visual
![Login](ui-images/WPR_4-Login.png)

**Elementos clave del diseño:**
- Dark mode con fondo oscuro (#0f0f23 aprox)
- Logo "MusicFund" con icono de nota musical en gradiente rosa/púrpura
- Card centrada con bordes redondeados y fondo semi-transparente
- Inputs con iconos (email, candado)
- Botón principal en gradiente rosa/magenta
- Opciones de login social (Google, Spotify)
- Link "¿Olvidaste tu contraseña?" y "Regístrate"

### Objetivo
Usuario puede registrarse, iniciar sesión y obtener JWT para llamadas autenticadas.

### API (.NET)

```
src/api/WebApi/
├── Program.cs                    # Configurar Identity + JWT
├── Controllers/
│   └── AuthController.cs         # Login, Register, RefreshToken
└── appsettings.json              # JWT settings
```

**Tareas:**
1. Configurar ASP.NET Core Identity en Program.cs
2. Configurar JWT Bearer Authentication
3. Crear AuthController:
   - `POST /api/auth/register` - Registrar usuario + crear Artista
   - `POST /api/auth/login` - Login, retorna JWT
   - `POST /api/auth/refresh` - Refresh token
4. Crear DTOs: LoginRequest, RegisterRequest, AuthResponse
5. Middleware de errores para auth

**Endpoints:**
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | /api/auth/register | Registro de usuario |
| POST | /api/auth/login | Login, retorna JWT |
| POST | /api/auth/refresh | Renovar token |

### Admin (Next.js)

```
src/admin/src/
├── app/(auth)/
│   ├── login/page.tsx            # Página login funcional
│   └── register/page.tsx         # Página registro funcional
├── services/
│   └── auth.service.ts           # Llamadas a API
├── store/
│   └── auth.store.ts             # Zustand store para auth
└── middleware.ts                 # Proteger rutas dashboard
```

**Tareas:**
1. Conectar LoginPage a API real
2. Conectar RegisterPage a API real
3. Guardar JWT en localStorage/cookies
4. Crear AuthStore con Zustand
5. Middleware para proteger rutas /dashboard/*
6. Redirect a login si no autenticado

### Web (Vite + React)

```
src/web/src/
├── features/auth/
│   ├── presentation/
│   │   ├── LoginPage.tsx         # Página login
│   │   └── RegisterPage.tsx      # Página registro
│   ├── application/
│   │   └── useAuth.ts            # Hook de autenticación
│   └── infrastructure/
│       └── auth.service.ts       # Llamadas a API
└── store/
    └── auth.store.ts             # Estado global auth
```

**Tareas:**
1. Conectar LoginPage a API
2. Conectar RegisterPage a API
3. AuthStore compartido
4. Interceptor en apiClient para añadir JWT
5. Redirect a login en 401

### Entregable
- ✅ Usuario puede registrarse en web y admin
- ✅ Usuario puede loguearse
- ✅ JWT se incluye en llamadas autenticadas
- ✅ Rutas protegidas redirigen a login

---

## Slice 2: Dashboard + Mis Campañas

**Estimación:** 2 horas

### Referencia Visual
![Dashboard](ui-images/WPR_5-Dashboard-Artist.png)

**Elementos clave del diseño:**
- Sidebar izquierdo con navegación (Dashboard, Mis Campañas, Crear Campaña, Mis Backers, Configuración)
- Header con saludo "Hola, Alex" y botón "+ Nueva campaña"
- 4 Stats cards: Total recaudado ($24,580), Backers totales (1,247), Campañas activas (3), Tasa de éxito (78%)
- Gráfico de línea "Recaudación últimos 30 días"
- Sección "Mis Campañas" con cards que muestran: título, estado (badge), porcentaje, backers
- Sección "Últimos Backings" con lista de backers recientes
- Estados de campaña: Activa (verde), Borrador (gris), Finalizada (púrpura)

### Objetivo
Artista autenticado ve su dashboard con estadísticas y lista de sus campañas.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    ├── Features/Campania/
    │   └── Queries/
    │       └── GetMisCampaniasQuery.cs    # Query + Handler
    ├── Dtos/
    │   ├── CampaniaDto.cs
    │   └── CampaniaListDto.cs
    └── Mapping/
        └── CampaniaProfile.cs             # AutoMapper

src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.WebApi/
    └── Controllers/
        └── CampaniasController.cs         # GET /me/campanias
```

**Tareas:**
1. Crear CampaniaDto y CampaniaListDto
2. Crear CampaniaProfile (AutoMapper)
3. Crear GetMisCampaniasQuery + Handler
4. Crear ICampaniaService + implementación
5. Crear CampaniasController con endpoint

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | /api/campanias/me | ✅ | Campañas del artista logueado |

### Admin (Next.js)

```
src/admin/src/
├── app/(dashboard)/
│   └── dashboard/
│       └── page.tsx              # Dashboard con datos reales
├── components/dashboard/
│   ├── stats-card.tsx            # Cards de estadísticas
│   └── campanias-table.tsx       # Tabla mis campañas
├── hooks/
│   └── useMisCampanias.ts        # TanStack Query hook
└── services/
    └── campania.service.ts       # API calls
```

**Tareas:**
1. Crear hook useMisCampanias
2. Conectar DashboardPage a datos reales
3. Mostrar stats: total campañas, recaudado, backers
4. Tabla/grid de mis campañas con estado
5. Loading states y error handling

### Web (Vite + React)
*No aplica para este slice*

### Entregable
- ✅ Artista ve dashboard con sus estadísticas
- ✅ Lista de "Mis Campañas" con datos reales
- ✅ Estados de campaña visibles (borrador, activa, etc.)

---

## Slice 3: Crear Campaña

**Estimación:** 2-3 horas

### Referencia Visual
![Crear Campaña](ui-images/WPR_6-Create-Campaign.png)

**Elementos clave del diseño:**
- Wizard de 4 pasos: 1. Información Básica → 2. Historia → 3. Recompensas → 4. Revisión
- Indicador de progreso con steps numerados
- Campos del paso 1:
  - Título de la campaña (input text)
  - Género musical (select dropdown)
  - Meta de financiación en € (input number)
  - Fecha de finalización (date picker)
  - URL del video YouTube/Vimeo (input url)
  - Imagen de portada (drag & drop con preview)
- Botones "Anterior" y "Siguiente" en footer
- Indicador "Paso 1 de 4"

### Objetivo
Artista puede crear una nueva campaña desde el dashboard.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Campania/
        ├── Commands/
        │   └── CreateCampaniaCommand.cs   # Command + Handler
        └── Validators/
            └── CreateCampaniaValidator.cs # FluentValidation
```

**Tareas:**
1. Crear CreateCampaniaCommand con propiedades
2. Crear CreateCampaniaCommandHandler
3. Crear CreateCampaniaValidator con reglas
4. Añadir mapping Command → Entity en Profile
5. Endpoint POST /api/campanias
6. Retornar ServiceResponse<Guid>

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | /api/campanias | ✅ | Crear nueva campaña |

**Validaciones:**
- Título: requerido, max 200 chars
- Descripción: max 500 chars
- ImporteObjetivo: min 100, max 1.000.000
- FechaFin: debe ser futura

### Admin (Next.js)

```
src/admin/src/
├── app/(dashboard)/campanias/
│   └── nueva/
│       └── page.tsx              # Página crear campaña
├── components/campanias/
│   └── campania-form.tsx         # Formulario conectado
└── hooks/
    └── useCreateCampania.ts      # useMutation hook
```

**Tareas:**
1. Crear hook useCreateCampania (useMutation)
2. Conectar CampaniaForm a API
3. Validación cliente con Zod (ya existe schema)
4. Toast de éxito/error
5. Redirect a /dashboard/campanias tras crear
6. Loading state en botón submit

### Web (Vite + React)
*No aplica para este slice (artistas crean en admin)*

### Entregable
- ✅ Artista puede crear campaña desde dashboard
- ✅ Validación en cliente y servidor
- ✅ Campaña aparece en "Mis Campañas" tras crear
- ✅ Feedback visual (toast, redirect)

---

## Slice 4: Explorar Campañas (Público)

**Estimación:** 2 horas

### Referencia Visual

**Landing Principal:**
![Landing](ui-images/WPR_1-Landing.png)

**Elementos clave Landing:**
- Hero section: "Financia la música que amas" con CTAs "Explorar Campañas" y "Soy Artista"
- Stats: "2,847 Artistas financiados" y "€1.2M Recaudados"
- Sección "Campañas Destacadas" con 3 cards horizontales
- Sección "Cómo Funciona" con pasos para artistas y fans
- Sección "Historias de Éxito" con testimonios
- CTA final: "¿Tienes un proyecto musical?"
- Footer con links

**Página Explorar:**
![Explorar](ui-images/WPR_2-Excplore-Campaigns.png)

**Elementos clave Explorar:**
- Header con búsqueda: "Buscar artistas, géneros, proyectos..."
- Filtros: Género Musical, Estado, Ordenar por
- Contador: "248 campañas activas"
- Grid 3 columnas de CampaniaCards
- Cada card muestra:
  - Imagen de portada
  - Badge de género (Rock, Jazz, Pop, etc.) + días restantes
  - Título de campaña
  - Nombre del artista
  - Barra de progreso con monto recaudado/objetivo y porcentaje
  - Número de backers

### Objetivo
Visitante puede ver landing con campañas activas sin necesidad de login.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Campania/
        └── Queries/
            └── GetAllCampaniasQuery.cs    # Query + Handler (público)
```

**Tareas:**
1. Crear GetAllCampaniasQuery + Handler
2. Filtrar solo campañas públicas (estado = Publicada o En curso)
3. Incluir datos del Artista (nombre)
4. Paginación básica (skip, take)
5. Endpoint público (sin [Authorize])

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | /api/campanias | ❌ | Lista campañas públicas |
| GET | /api/campanias?page=1&size=12 | ❌ | Con paginación |

### Admin (Next.js)
*No aplica para este slice*

### Web (Vite + React)

```
src/web/src/
├── features/campanias/
│   ├── presentation/
│   │   ├── HomePage.tsx          # Landing con campañas
│   │   └── CampaniasPage.tsx     # Página explorar
│   │   └── components/
│   │       ├── CampaniaCard.tsx  # Card de campaña
│   │       └── CampaniaGrid.tsx  # Grid responsive
│   ├── application/
│   │   └── useCampanias.ts       # Hook con TanStack Query
│   └── infrastructure/
│       └── campania.service.ts   # Conectar a API real
```

**Tareas:**
1. Crear/actualizar hook useCampanias
2. Conectar CampaniasPage a API real
3. Grid responsive de CampaniaCards
4. Mostrar: imagen, título, artista, progreso, días restantes
5. Loading skeletons
6. Empty state si no hay campañas

### Entregable
- ✅ Landing muestra campañas reales
- ✅ Cards con progreso visual (barra)
- ✅ Responsive (mobile, tablet, desktop)
- ✅ No requiere login para ver

---

## Slice 5: Detalle de Campaña

**Estimación:** 2 horas

### Referencia Visual
![Detalle Campaña](ui-images/WPR_3-Detail-Campaign.png)

**Elementos clave del diseño:**

**Columna izquierda (contenido):**
- Imagen/video principal de la campaña
- Título: "Nuevo Álbum: Ecos de Medianoche"
- Info artista: avatar + nombre + "Artista verificado" + badges género
- Stats: €12,450 de €15,000 | 83% financiado | 127 backers | 12 días restantes
- Tabs: Historia | Actualizaciones | Comentarios | FAQ
- Descripción larga del proyecto
- Sección "¿Por qué necesitamos tu apoyo?" con bullets
- Sección "Sobre el Artista" con bio y stats del artista

**Columna derecha (sidebar sticky):**
- Botón "Apoyar esta campaña" (CTA principal)
- "Desde €5"
- Lista de Recompensas ordenadas por precio:
  - €10: Descarga Digital (badge "Más popular")
  - €25: CD Físico
  - €50: Vinilo Edición Limitada
  - €100: Paquete VIP
- Cada reward muestra: precio, título, descripción, disponibilidad, botón "Seleccionar"
- Footer: "Pago seguro" + "Entrega estimada: Marzo 2025"

### Objetivo
Click en una campaña muestra página de detalle con rewards disponibles.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    ├── Features/Campania/
    │   └── Queries/
    │       └── GetCampaniaByIdQuery.cs    # Con Include de Rewards
    └── Dtos/
        ├── CampaniaDetailDto.cs           # DTO con rewards
        └── RewardDto.cs                   # DTO de reward
```

**Tareas:**
1. Crear CampaniaDetailDto (incluye lista de rewards)
2. Crear RewardDto
3. Crear GetCampaniaByIdQuery + Handler
4. Include de Rewards en query
5. Include de Artista (nombre, redes sociales)
6. Endpoint GET /api/campanias/{id}

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | /api/campanias/{id} | ❌ | Detalle con rewards |

### Admin (Next.js)

```
src/admin/src/
└── app/(dashboard)/campanias/
    └── [id]/
        └── page.tsx              # Vista detalle (opcional)
```

**Tareas (opcional):**
1. Página de detalle para que artista vea su campaña
2. Métricas adicionales (backers, conversión)

### Web (Vite + React)

```
src/web/src/
└── features/campanias/
    ├── presentation/
    │   └── CampaniaDetailPage.tsx    # Página detalle
    │   └── components/
    │       ├── CampaniaHeader.tsx    # Hero con imagen/video
    │       ├── CampaniaInfo.tsx      # Descripción, artista
    │       ├── CampaniaProgress.tsx  # Barra progreso, stats
    │       └── RewardsList.tsx       # Lista de rewards
    └── application/
        └── useCampania.ts            # Hook para detalle
```

**Tareas:**
1. Crear hook useCampania(id)
2. CampaniaDetailPage con layout completo
3. Header con imagen/video principal
4. Barra de progreso con monto y porcentaje
5. Días restantes con countdown
6. Info del artista con links a redes
7. Lista de rewards disponibles
8. CTA "Apoyar este proyecto"

### Entregable
- ✅ Página de detalle completa
- ✅ Rewards visibles con precios
- ✅ Información del artista
- ✅ Botón para apoyar (aún sin funcionalidad)

---

## Slice 6: Hacer Backing

**Estimación:** 2-3 horas

### Referencia Visual
![Backing](ui-images/WPR_7-Backing.png)

**Elementos clave del diseño:**

**Flujo de 3 pasos:** Monto → Datos → Pago

**Panel/Modal de backing:**
- Header: "Apoya a Luna Sonora" + subtítulo campaña
- Progress bar de pasos
- Reward seleccionado con opción "Cambiar"
  - "Disco firmado + Póster" - Mínimo €35
- Sección "Monto a Aportar":
  - Input grande con € y monto (50)
  - Texto: "Mínimo €35 para este reward"
  - Botones rápidos: +€5, +€10, +€25
- Sección "Información de Entrega":
  - Nombre completo
  - Dirección completa
  - Ciudad, Código Postal, País (select)
  - Checkbox "Guardar para futuras compras"
- Sección "Método de Pago":
  - Radio: Tarjeta | PayPal
  - Campos tarjeta: Número, MM/YY, CVV
  - Texto: "Pago seguro con encriptación SSL"
- Resumen Final:
  - Aportación: €50
  - Reward: Disco firmado + Póster
  - Total: €50

### Objetivo
Fan puede seleccionar un reward y realizar un backing a una campaña.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Backing/
        ├── Commands/
        │   └── CreateBackingCommand.cs    # Command + Handler
        ├── Validators/
        │   └── CreateBackingValidator.cs
        └── Dtos/
            └── BackingDto.cs

src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.WebApi/
    └── Controllers/
        └── BackingsController.cs
```

**Tareas:**
1. Crear CreateBackingCommand + Handler
2. Validar: monto >= ImporteMinimo del reward
3. Crear PedidoCrowdfunding + PedidoCrowdfundingLinea
4. Actualizar ImportePledgedActual en campaña
5. Crear BackingsController
6. Transacción para consistencia

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | /api/backings | ✅ | Crear backing |

**Payload:**
```json
{
  "campaniaId": "guid",
  "rewardId": "guid",
  "monto": 50.00,
  "mensaje": "Mucho éxito con el proyecto!"
}
```

### Admin (Next.js)
*No aplica (fans hacen backing en web)*

### Web (Vite + React)

```
src/web/src/
└── features/backings/
    ├── presentation/
    │   ├── BackingModal.tsx          # Modal de backing
    │   └── components/
    │       ├── RewardSelector.tsx    # Seleccionar reward
    │       ├── AmountInput.tsx       # Monto personalizado
    │       └── BackingConfirm.tsx    # Confirmación
    ├── application/
    │   └── useCreateBacking.ts       # useMutation
    └── infrastructure/
        └── backing.service.ts
```

**Tareas:**
1. Modal/página de backing
2. Selector de reward con descripción
3. Input de monto (>= mínimo del reward)
4. Campo de mensaje opcional
5. Resumen antes de confirmar
6. Hook useCreateBacking
7. Toast de éxito + redirect
8. Actualizar UI de campaña (progreso)

### Entregable
- ✅ Fan puede hacer backing con reward
- ✅ Monto se suma al progreso de campaña
- ✅ Confirmación visual del backing
- ✅ Validación de monto mínimo

---

## Slice 7: Mis Backings (Fan)

**Estimación:** 2 horas

### Referencia Visual
![Mis Backings](ui-images/WPR_9-Mis-backings.png)

**Elementos clave del diseño:**

**Layout:**
- Sidebar izquierdo (como fan): Dashboard, Explorar, Mis Apoyos, Guardados, Notificaciones, Configuración
- Header: "Mis Apoyos" + stats "Has apoyado 12 proyectos por un total de €850"
- Botón "Explorar Campañas"

**Filtros:**
- Tabs: Todos (12) | Activos (5) | Completados (6) | Pendientes (1)

**Lista de Backings (cards verticales):**
Cada card muestra:
- Imagen thumbnail de campaña (izquierda)
- Contenido (derecha):
  - Título campaña + link al artista
  - Monto aportado en rosa: "€75"
  - Fecha del apoyo: "15 Enero 2025"
  - Recompensa: descripción del reward
  - Estado del reward con badge:
    - "Pendiente" (amarillo) - "Campaña finaliza en 15 días"
    - "Enviado" (azul) - con número de tracking
    - "Entregado" (verde) - "Recibido el 5 Enero 2025"
    - "En preparación" (púrpura) - "Artista preparando envío"
    - "Reembolsado" (rojo) - "Campaña no alcanzó su objetivo"
  - Estado de campaña: badge "En curso" (azul), "Financiada" (verde), "No alcanzó meta" (gris)
  - Botones: "Ver Campaña", "Contactar"

### Objetivo
Fan ve historial de todos sus backings con estados.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Backing/
        └── Queries/
            └── GetMisBackingsQuery.cs     # Query + Handler
```

**Tareas:**
1. Crear GetMisBackingsQuery + Handler
2. Incluir datos de campaña y reward
3. Incluir estado del pedido
4. Ordenar por fecha descendente
5. Endpoint GET /api/users/me/backings

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | /api/users/me/backings | ✅ | Backings del usuario |

### Admin (Next.js)

```
src/admin/src/
└── app/(dashboard)/
    └── backings/
        └── page.tsx              # Backings recibidos (artista)
```

**Tareas (opcional):**
1. Vista de backings recibidos como artista
2. Lista de backers por campaña

### Web (Vite + React)

```
src/web/src/
└── features/backings/
    ├── presentation/
    │   └── MisBackingsPage.tsx       # Página mis apoyos
    │   └── components/
    │       ├── BackingCard.tsx       # Card de backing
    │       ├── BackingFilters.tsx    # Filtros por estado
    │       └── BackingEmpty.tsx      # Empty state
    └── application/
        └── useMisBackings.ts         # Hook
```

**Tareas:**
1. Crear hook useMisBackings
2. MisBackingsPage con lista/grid
3. Filtros: Todos | Activos | Completados
4. BackingCard con:
   - Imagen campaña
   - Nombre campaña + artista
   - Monto aportado
   - Reward seleccionado
   - Estado de campaña (badge)
   - Estado de reward (pendiente, enviado, entregado)
   - Fecha del backing
5. Empty state con CTA "Explorar campañas"

### Entregable
- ✅ Fan ve todos sus backings
- ✅ Estados visuales con badges
- ✅ Filtros funcionales
- ✅ Link a campaña original

---

## Slice 8: CRUD Rewards

**Estimación:** 2 horas

### Objetivo
Artista puede añadir, editar y eliminar rewards de su campaña.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Reward/
        ├── Commands/
        │   ├── CreateRewardCommand.cs
        │   ├── UpdateRewardCommand.cs
        │   └── DeleteRewardCommand.cs
        ├── Validators/
        │   ├── CreateRewardValidator.cs
        │   └── UpdateRewardValidator.cs
        └── Queries/
            └── GetRewardsByCampaniaQuery.cs
```

**Tareas:**
1. CRUD completo de Rewards
2. Validar que campaña pertenece al artista
3. Validar ImporteMinimo > 0
4. No permitir eliminar reward con backings
5. Endpoints anidados bajo campaña

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| GET | /api/campanias/{id}/rewards | ✅ | Listar rewards |
| POST | /api/campanias/{id}/rewards | ✅ | Crear reward |
| PUT | /api/campanias/{id}/rewards/{rewardId} | ✅ | Actualizar |
| DELETE | /api/campanias/{id}/rewards/{rewardId} | ✅ | Eliminar |

### Admin (Next.js)

```
src/admin/src/
├── app/(dashboard)/campanias/
│   └── [id]/
│       └── rewards/
│           └── page.tsx          # Gestión de rewards
└── components/rewards/
    ├── reward-form.tsx           # Formulario crear/editar
    ├── reward-list.tsx           # Lista editable
    └── reward-card.tsx           # Card con acciones
```

**Tareas:**
1. Página de gestión de rewards dentro de campaña
2. Lista de rewards existentes
3. Formulario para añadir nuevo
4. Edición inline o modal
5. Confirmación antes de eliminar
6. Ordenar rewards por ImporteMinimo

### Web (Vite + React)
*No aplica (solo visualiza rewards, no los edita)*

### Entregable
- ✅ Artista puede añadir rewards
- ✅ Artista puede editar rewards
- ✅ Artista puede eliminar rewards (si no tiene backings)
- ✅ Rewards ordenados por precio

---

## Slice 9: Editar Campaña

**Estimación:** 1-2 horas

### Objetivo
Artista puede editar los datos de su campaña.

### API (.NET)

```
src/api/Modules/Crowdfunding/
└── WePlayRises.Crowdfunding.Application/
    └── Features/Campania/
        ├── Commands/
        │   └── UpdateCampaniaCommand.cs   # Command + Handler
        └── Validators/
            └── UpdateCampaniaValidator.cs
```

**Tareas:**
1. Crear UpdateCampaniaCommand + Handler
2. Validar que campaña pertenece al artista
3. Restricciones según estado:
   - Borrador: todo editable
   - Publicada/En curso: solo algunos campos
   - Completada: nada editable
4. Endpoint PUT /api/campanias/{id}

**Endpoints:**
| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| PUT | /api/campanias/{id} | ✅ | Actualizar campaña |

### Admin (Next.js)

```
src/admin/src/
└── app/(dashboard)/campanias/
    └── [id]/
        └── editar/
            └── page.tsx          # Formulario edición
```

**Tareas:**
1. Reutilizar CampaniaForm con datos precargados
2. Hook useUpdateCampania
3. Campos deshabilitados según estado
4. Toast de éxito + redirect

### Web (Vite + React)
*No aplica*

### Entregable
- ✅ Artista puede editar su campaña
- ✅ Restricciones según estado de campaña
- ✅ Feedback visual de cambios guardados

---

## Slice 10: Pulido + Deploy

**Estimación:** 3-4 horas

### Objetivo
MVP listo para producción con datos de demo.

### Tareas Generales

**Seed Data:**
```sql
-- 2-3 artistas de ejemplo
-- 4-6 campañas en distintos estados
-- Rewards variados por campaña
-- Algunos backings de prueba
```

**Tests Básicos:**
- Tests unitarios de Handlers críticos
- Tests de integración de endpoints principales
- Tests de componentes React clave

**Deploy Azure:**
- App Service para API (.NET)
- Static Web App para Web (Vite)
- Static Web App para Admin (Next.js)
- Azure SQL ya configurado

**Ajustes UI:**
- Responsive final
- Loading states consistentes
- Error boundaries
- SEO básico (meta tags)

**Documentación:**
- README actualizado
- Variables de entorno documentadas
- Postman collection exportada

### Checklist Final

- [ ] API desplegada y funcionando
- [ ] Web desplegada y funcionando
- [ ] Admin desplegado y funcionando
- [ ] Seed data cargado
- [ ] Flujo E2E probado manualmente
- [ ] Sin errores en consola
- [ ] Responsive en mobile

### Entregable
- ✅ MVP funcional en producción
- ✅ URL pública para demo
- ✅ Datos de ejemplo para mostrar

---

## Dependencias entre Slices

```
┌─────────┐
│  Auth   │ ←── Base para todo
└────┬────┘
     │
     ▼
┌─────────────────┐     ┌──────────────────┐
│ Dashboard +     │     │ Explorar         │
│ Mis Campañas    │     │ Campañas         │
└────────┬────────┘     └────────┬─────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐      ┌──────────────────┐
│ Crear           │      │ Detalle          │
│ Campaña         │────▶│ Campaña          │
└────────┬────────┘      └────────┬─────────┘
         │                       │
         │              ┌────────┴─────────┐
         │              ▼                  │
         │     ┌──────────────────┐        │
         │     │ Hacer            │        │
         │     │ Backing          │        │
         │     └────────┬─────────┘        │
         │              │                  │
         │              ▼                  │
         │     ┌──────────────────┐        │
         │     │ Mis              │        │
         │     │ Backings         │        │
         │     └──────────────────┘        │
         │                                 │
         ▼                                 │
┌─────────────────┐                        │
│ CRUD            │                        │
│ Rewards         │◀──────────────────────┘
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Editar          │
│ Campaña         │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Pulido +        │
│ Deploy          │
└─────────────────┘
```

---

## Pantalla Opcional: Perfil Público del Artista

### Referencia Visual
![Perfil Artista](ui-images/WPR_8-Artist-Profile.png)

**Elementos clave del diseño:**
- Header con imagen de portada y avatar del artista
- Nombre artístico + badges de género (Electronic, Synthwave, Indie)
- Botón "Seguir" + links a redes (Spotify, YouTube, Instagram)
- Stats: 3 campañas | 247 backers | €12,450 recaudados
- Sección "Biografía" con descripción y links
- Tabs: Campañas Activas | Campañas Pasadas
- Cards de campañas del artista
- Sección "Reseñas de Fans" con valoraciones

**Nota:** Esta pantalla puede implementarse como parte del Slice 4 (Explorar) o como slice adicional si hay tiempo. Es accesible desde el detalle de campaña al hacer click en el nombre del artista.

---

## Notas Importantes

1. **Sesiones de 2h:** Cada slice está diseñado para completarse en una sesión. Si sobra tiempo, avanzar con el siguiente.

2. **Prioridad E2E:** Siempre entregar algo funcional de punta a punta. Mejor menos features completas que muchas a medias.

3. **Base de datos lista:** No perder tiempo en migraciones. La estructura ya existe en Azure SQL.

4. **Tipos compartidos:** Usar `src/shared` para mantener consistencia entre web y admin.

5. **Iterar:** Si un slice se complica, simplificar y seguir. El MVP es más importante que la perfección.

---

## Próximo Paso

**Iniciar Slice 1: Auth**

```bash
# Verificar que la API compila
cd src/api/WebApi
dotnet build

# Verificar que los frontends compilan
cd src/web && npm run build
cd src/admin && npm run build
```
