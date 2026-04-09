# Feature: Dashboard de Artista

> **ID:** dashboard-artista
> **User Story:** US-05
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripcion

El Dashboard de Artista es el centro de control para que los artistas gestionen y monitoreen sus campañas de crowdfunding. Proporciona una vista consolidada de metricas clave (total recaudado, numero de backers, campanias activas) y permite profundizar en los detalles de cada campania individual, visualizando backings recientes, estadisticas ampliadas y proyecciones de exito.

Esta feature es critica para el MVP porque permite a los artistas tomar decisiones informadas sobre sus campañas, responder a sus backers, y evaluar el rendimiento de sus estrategias de recompensas. Sin un dashboard funcional, los artistas carecerian de visibilidad sobre el progreso de sus proyectos, lo cual es fundamental para la propuesta de valor de WePlay Rises.

El dashboard se implementa exclusivamente en la aplicacion Admin (Next.js 14) y requiere autenticacion obligatoria. Las metricas se calculan en tiempo real en el backend mediante queries optimizadas, evitando calculos cliente para garantizar precision y rendimiento.

---

## User Story

**Como** artista
**Quiero** ver el progreso de mi campania
**Para** conocer las metricas y gestionar mis proyectos

---

## Flujo Principal

1. Artista autenticado accede a `/dashboard` (ruta protegida)
2. Sistema verifica que el usuario tiene perfil de Artista (rol)
3. Sistema carga resumen general desde backend:
   - Total recaudado en todas las campanias
   - Numero total de backers
   - Campanias activas vs completadas
4. Sistema muestra cards de las campanias del artista:
   - Titulo, imagen, estado
   - Barra de progreso visual (recaudado / objetivo)
   - Porcentaje de meta alcanzado
   - Numero de backers
   - Dias restantes (si estado = EnCurso)
5. Artista hace click en una campania especifica
6. Sistema navega a `/dashboard/campanias/{id}`
7. Sistema carga metricas ampliadas:
   - Estadisticas detalladas (backing promedio, proyeccion final)
   - Stats por reward (mas popular, cantidad vendida)
   - Lista de backings recientes (paginada, 20 por pagina)
8. Artista visualiza backings con:
   - Nombre del backer (o "Anonimo" si `esAnonimo = true`)
   - Email del backer (solo visible para artista)
   - Reward seleccionado
   - Monto aportado
   - Mensaje del backer (si existe)
   - Fecha relativa (ej. "hace 2 horas")
9. Artista puede navegar entre paginas de backings
10. Artista puede volver al dashboard principal

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Artista sin campanias | Mostrar EmptyState con CTA "Crear tu primera campania" que redirige a `/dashboard/campanias/nueva` |
| FA-02 | Todas campanias finalizadas | Mostrar resumen historico con metricas totales, sin filtro de "activas" |
| FA-03 | Backing anonimo (`PermitirMostrarNombre = false`) | Mostrar "Anonimo" en lugar del nombre, ocultar email en la UI |
| FA-04 | Campania sin backings | Mostrar EmptyState "Aun no hay backings. Comparte tu campania para conseguir apoyo" |
| FA-05 | Error al cargar metricas | Mostrar toast de error, permitir reintentar con boton "Actualizar" |
| FA-06 | Token expirado durante navegacion | Redirigir a login con mensaje "Sesion expirada" |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-05-1 | El artista solo visualiza campanias donde `ArtistaId = userId` (filtro backend, no frontend) | Backend |
| AC-05-2 | Metricas se calculan en tiempo real (no cacheadas mas de 1 minuto) | Backend |
| AC-05-3 | Lista de backings se ordena por `FechaCreacion DESC` mostrando primero los mas recientes | Backend |
| AC-05-4 | Backings con `PermitirMostrarNombre = false` muestran "Anonimo" y NO exponen nombre/email en response | Backend + Admin |
| AC-05-5 | Porcentaje se calcula como `(ImporteRecaudado / ImporteObjetivo) * 100` con 2 decimales | Backend |
| AC-05-6 | Dias restantes se calculan como `(FechaFin - DateOnly.Today).Days` y muestran 0 si es negativo | Backend + Admin |
| AC-05-7 | Dashboard principal muestra maximo 10 campanias con paginacion para ver mas | Admin |
| AC-05-8 | Al hacer backing en otra ventana, dashboard se actualiza automaticamente en 30s (polling) | Admin |
| AC-05-9 | Componente `BackingTable` es accesible (navegacion con teclado, labels en inputs) | Admin |
| AC-05-10 | Endpoint `/api/dashboard/resumen` responde en < 500ms con usuario que tiene 10 campanias | Backend |
| AC-05-11 | Usuario sin rol Artista recibe 403 Forbidden al acceder a endpoints de dashboard | Backend |
| AC-05-12 | Barra de progreso muestra color verde si >= 100%, amarillo si >= 50%, rojo si < 50% | Admin |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Crear queries CQRS para obtener resumen, mis campanias, backings y stats. Implementar calculos de metricas (porcentaje, dias restantes, proyeccion). Validar autorizacion (solo ver propias campanias). | ALTO |
| **Admin** | Crear paginas de dashboard principal y detalle de campania. Implementar componentes UI (StatCard, ProgressBar, BackingTable). Integrar React Query para polling y cache. Proteger rutas con middleware de autenticacion. | ALTO |
| **Shared** | Definir tipos TypeScript para DTOs (DashboardResumenDto, MisCampaniasItemDto, CampaniaStatsDto, BackingListItemDto). Crear schemas Zod para validacion. Agregar constantes de estados y colores de progreso. | MEDIO |
| **Landing** | No involucrado (dashboard es solo para artistas autenticados en Admin) | NINGUNO |

---

## Dependencias

### Tecnicas
- WPR-002: Sistema de autenticacion JWT (el dashboard requiere token valido)
- WPR-005: CRUD Campanias (el dashboard consume entidad `CampaniaCrowdfunding`)
- WPR-011: Realizar Backing (el dashboard muestra backings creados por esta feature)

### De otras features
- `registro-artista`: El usuario debe tener perfil de Artista para acceder al dashboard
- `gestion-campanias`: El dashboard consume las campanias creadas por el artista
- `hacer-backing`: El dashboard lista los backings realizados en cada campania

### Modulos Backend Existentes
- `Modules/Crowdfunding/Crowdfunding.Application`: Queries y Handlers se agregan aqui
- `Modules/Crowdfunding/Crowdfunding.Domain`: Entidades `CampaniaCrowdfunding`, `PedidoCrowdfunding`, `AportacionCrowdfunding` ya existen
- `Modules/Crowdfunding/Crowdfunding.Infra`: Repositorios existentes se reutilizan para queries

---

## Especificacion de Endpoints

### 1. GET /api/dashboard/resumen

**Autorizacion:** Bearer JWT con rol `Artista`

**Query:** `GetDashboardResumenQuery`

**Logica:**
1. Obtener `userId` del token JWT
2. Obtener todas las campanias donde `ArtistaId = userId`
3. Calcular metricas agregadas:
   - `TotalRecaudado = SUM(ImportePledgedActual)`
   - `TotalBackers = COUNT(DISTINCT UserId from PedidoCrowdfunding)`
   - `CampaniasActivas = COUNT where EstadoCampaniaId = EnCurso`
   - `CampaniasCompletadas = COUNT where EstadoCampaniaId = Completada`

**Response 200 OK:**
```typescript
{
  totalRecaudado: number;      // Suma de ImportePledgedActual
  totalBackers: number;         // Backers unicos en todas campanias
  campaniasActivas: number;     // Estado = EnCurso
  campaniasCompletadas: number; // Estado = Completada
  moneda: string;               // "EUR" (constante)
}
```

**Response 401 Unauthorized:** Token invalido o ausente

**Response 403 Forbidden:** Usuario no tiene rol `Artista`

---

### 2. GET /api/campanias/mis-campanias

**Autorizacion:** Bearer JWT con rol `Artista`

**Query params:**
- `estado` (opcional): `string` - Filtrar por estado (ej. "EnCurso", "Completada")
- `page` (default: 1): `number` - Numero de pagina
- `pageSize` (default: 10): `number` - Items por pagina

**Query:** `GetMisCampaniasQuery`

**Logica:**
1. Obtener `userId` del token
2. Filtrar campanias: `ArtistaId = userId AND (EstadoCampaniaId = estado OR estado = null)`
3. Ordenar por `FechaCreacion DESC`
4. Paginar resultados
5. Para cada campania calcular:
   - `Porcentaje = (ImportePledgedActual / ImporteObjetivo) * 100`
   - `NumBackers = COUNT(PedidoCrowdfunding where CampaniaId = id)`
   - `DiasRestantes = MAX(0, (FechaFin - DateOnly.Today).Days)`

**Response 200 OK:**
```typescript
{
  items: Array<{
    id: string;
    titulo: string;
    imagenUrl: string | null;
    estado: string;              // "EnCurso", "Completada", etc.
    estadoNombre: string;        // "En Curso", "Completada" (display)
    importeObjetivo: number;
    importeRecaudado: number;    // ImportePledgedActual
    porcentaje: number;          // Calculado (2 decimales)
    numBackers: number;          // Count de pedidos
    diasRestantes: number | null; // null si no aplica
    fechaFin: string;            // ISO 8601
    fechaCreacion: string;       // ISO 8601
  }>;
  totalCount: number;
  page: number;
  pageSize: number;
}
```

---

### 3. GET /api/campanias/{id}/backings

**Autorizacion:** Bearer JWT con rol `Artista` y `ArtistaId = campania.ArtistaId`

**Path param:** `id` (Guid de la campania)

**Query params:**
- `page` (default: 1): `number`
- `pageSize` (default: 20): `number`

**Query:** `GetCampaniaBackingsQuery`

**Logica:**
1. Verificar que campania existe
2. Verificar que `campania.ArtistaId = userId` (autorizacion)
3. Obtener pedidos: `PedidoCrowdfunding where CampaniaId = id`
4. JOIN con `AportacionCrowdfunding` para obtener monto
5. JOIN con `CampaniaCrowdfundingReward` para nombre del reward
6. Ordenar por `FechaCreacion DESC`
7. Paginar
8. Para cada backing:
   - Si `PermitirMostrarNombre = false` → `nombreBacker = "Anonimo"`, `emailBacker = null`
   - Calcular fecha relativa (hace X horas/dias)

**Response 200 OK:**
```typescript
{
  items: Array<{
    id: string;
    nombreBacker: string;      // "Anonimo" si PermitirMostrarNombre = false
    emailBacker: string | null; // null si anonimo
    rewardNombre: string;
    monto: number;
    mensaje: string | null;    // ComentarioBacker
    esAnonimo: boolean;
    fechaCreacion: string;     // ISO 8601
    fechaRelativa: string;     // "hace 2 horas", "hace 3 dias"
  }>;
  totalCount: number;
  totalRecaudado: number;      // Suma de montos
  stats: {
    backingPromedio: number;   // totalRecaudado / totalCount
    rewardMasPopular: string;  // Reward con mas pedidos
    ultimoBacking: string;     // ISO 8601 de mas reciente
  };
}
```

**Response 403 Forbidden:** Campania no pertenece al artista autenticado

**Response 404 NotFound:** Campania no existe

---

### 4. GET /api/campanias/{id}/stats

**Autorizacion:** Bearer JWT con rol `Artista` y `ArtistaId = campania.ArtistaId`

**Path param:** `id` (Guid de la campania)

**Query:** `GetCampaniaStatsQuery`

**Logica:**
1. Obtener campania y verificar autorizacion
2. Calcular metricas:
   - `NumBackers = COUNT(PedidoCrowdfunding)`
   - `BackingPromedio = ImportePledgedActual / NumBackers`
   - `DiasRestantes = MAX(0, (FechaFin - Today).Days)`
   - `DiasTranscurridos = (Today - FechaInicio).Days`
   - `ProyeccionFinal = (ImportePledgedActual / DiasTranscurridos) * DiasTotales` (regresion lineal simple)
3. Calcular stats por reward:
   - GROUP BY `RewardId`
   - `Cantidad = COUNT(PedidoCrowdfunding)`
   - `Total = SUM(ImporteTotal)`
   - `Porcentaje = (Cantidad / NumBackers) * 100`
4. Obtener progreso por dia (opcional MVP):
   - GROUP BY `DATE(FechaCreacion)`
   - `Total = SUM(ImporteTotal) acumulado`

**Response 200 OK:**
```typescript
{
  importeObjetivo: number;
  importeRecaudado: number;
  porcentaje: number;
  numBackers: number;
  backingPromedio: number;
  diasRestantes: number;
  diasTranscurridos: number;
  proyeccionFinal: number;
  rewardStats: Array<{
    rewardNombre: string;
    cantidad: number;
    total: number;
    porcentaje: number;
  }>;
  progressoPorDia?: Array<{   // POST-MVP (opcional)
    fecha: string;             // "2026-01-15"
    total: number;             // Acumulado hasta ese dia
  }>;
}
```

---

## Componentes UI (Admin)

### Paginas

**1. `/dashboard/page.tsx` (Dashboard Principal)**
- Layout con sidebar usando template dashtail
- Seccion de resumen con 4 StatCards
- Grid de CampaignCards (responsive: 1 col mobile, 2 cols tablet, 3 cols desktop)
- EmptyState si no hay campanias
- Boton "Nueva Campania" prominente

**2. `/dashboard/campanias/[id]/page.tsx` (Detalle de Campania)**
- Breadcrumb: `Dashboard > {TituloCampania}`
- Seccion de metricas (4 StatCards: Recaudado, Backers, Dias Restantes, Proyeccion)
- Barra de progreso con porcentaje
- Tabs: "Backings Recientes" | "Estadisticas" | (POST-MVP: "Grafico Progreso")
- BackingTable con paginacion
- Botones de accion: "Ver pagina publica" | "Editar campania" | "Compartir"

### Componentes Reutilizables

**`<StatCard />`**
```typescript
interface StatCardProps {
  label: string;
  value: string | number;
  icon: React.ReactNode;
  trend?: {
    value: number;    // Porcentaje de cambio
    isPositive: boolean;
  };
}
```
Usa: shadcn `Card`, `CardHeader`, `CardContent`

**`<ProgressBar />`**
```typescript
interface ProgressBarProps {
  current: number;
  goal: number;
  showPercentage?: boolean;
  colorScheme?: 'success' | 'warning' | 'danger';
}
```
Logica de color:
- `>= 100%` → `success` (verde)
- `>= 50%` → `warning` (amarillo)
- `< 50%` → `danger` (rojo)

**`<CampaignCard />`**
```typescript
interface CampaignCardProps {
  campania: MisCampaniasItemDto;
  onSelect: (id: string) => void;
}
```
Muestra: imagen, titulo, estado badge, progreso, metricas clave

**`<BackingTable />`**
```typescript
interface BackingTableProps {
  backings: BackingListItemDto[];
  totalCount: number;
  currentPage: number;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}
```
Usa: shadcn `Table`, `Pagination`
Accesibilidad: aria-labels, navegacion con teclado

**`<EmptyState />`**
```typescript
interface EmptyStateProps {
  title: string;
  description: string;
  action?: {
    label: string;
    href: string;
  };
  icon?: React.ReactNode;
}
```

---

## Tipos Compartidos (Shared)

**Ubicacion:** `src/shared/types/dashboard.ts`

```typescript
export interface DashboardResumenDto {
  totalRecaudado: number;
  totalBackers: number;
  campaniasActivas: number;
  campaniasCompletadas: number;
  moneda: string;
}

export interface MisCampaniasItemDto {
  id: string;
  titulo: string;
  imagenUrl: string | null;
  estado: string;
  estadoNombre: string;
  importeObjetivo: number;
  importeRecaudado: number;
  porcentaje: number;
  numBackers: number;
  diasRestantes: number | null;
  fechaFin: string;
  fechaCreacion: string;
}

export interface BackingListItemDto {
  id: string;
  nombreBacker: string;
  emailBacker: string | null;
  rewardNombre: string;
  monto: number;
  mensaje: string | null;
  esAnonimo: boolean;
  fechaCreacion: string;
  fechaRelativa: string;
}

export interface CampaniaStatsDto {
  importeObjetivo: number;
  importeRecaudado: number;
  porcentaje: number;
  numBackers: number;
  backingPromedio: number;
  diasRestantes: number;
  diasTranscurridos: number;
  proyeccionFinal: number;
  rewardStats: RewardStatDto[];
  progressoPorDia?: ProgressDayDto[];
}

export interface RewardStatDto {
  rewardNombre: string;
  cantidad: number;
  total: number;
  porcentaje: number;
}

export interface ProgressDayDto {
  fecha: string;
  total: number;
}
```

---

## Esquemas Zod (Shared)

**Ubicacion:** `src/shared/schemas/dashboard.schemas.ts`

```typescript
import { z } from 'zod';

export const dashboardResumenSchema = z.object({
  totalRecaudado: z.number().min(0),
  totalBackers: z.number().int().min(0),
  campaniasActivas: z.number().int().min(0),
  campaniasCompletadas: z.number().int().min(0),
  moneda: z.string(),
});

export const misCampaniasItemSchema = z.object({
  id: z.string().uuid(),
  titulo: z.string().min(1).max(200),
  imagenUrl: z.string().url().nullable(),
  estado: z.string(),
  estadoNombre: z.string(),
  importeObjetivo: z.number().positive(),
  importeRecaudado: z.number().min(0),
  porcentaje: z.number().min(0),
  numBackers: z.number().int().min(0),
  diasRestantes: z.number().int().min(0).nullable(),
  fechaFin: z.string().datetime(),
  fechaCreacion: z.string().datetime(),
});

// ... (similares para BackingListItem, CampaniaStats, etc.)
```

---

## Requisitos No Funcionales

### Rendimiento
- `GET /api/dashboard/resumen` debe responder en < 500ms con base de 10 campanias
- `GET /api/campanias/mis-campanias` debe responder en < 800ms con paginacion de 10 items
- `GET /api/campanias/{id}/backings` debe responder en < 1s con paginacion de 20 items
- Queries deben usar indices en `ArtistaId`, `CampaniaId`, `FechaCreacion`
- Frontend debe usar React Query con staleTime de 30s para evitar refetch excesivo

### Seguridad
- TODOS los endpoints requieren autenticacion (Bearer JWT)
- Validar rol `Artista` en backend (middleware o validator)
- Endpoint `/backings` debe verificar que `campania.ArtistaId = userId` (no confiar en frontend)
- Backings anonimos NO deben exponer nombre/email en response (filtrado en backend)
- Sanitizar parametros de query (page, pageSize) para prevenir SQL injection

### Accesibilidad
- Dashboard navegable con teclado (Tab, Enter, Escape)
- Tablas con `aria-labels` descriptivos
- Progress bars con `aria-valuenow`, `aria-valuemin`, `aria-valuemax`
- EmptyState con contraste WCAG AA (4.5:1)
- Botones con texto descriptivo (no iconos solos sin label)

### Usabilidad
- Feedback visual inmediato al hacer click (loading states)
- Mensajes de error claros y accionables (ej. "No se pudo cargar. Reintentar")
- Fechas relativas legibles ("hace 2 horas" en lugar de timestamp)
- Porcentajes con 2 decimales maximos (evitar `83.333333%`)
- Montos formateados con separadores de miles (`$1,250.00`)

### Escalabilidad
- Paginacion obligatoria en listas (no cargar > 100 items de golpe)
- Queries con limite de registros (maxPageSize = 100)
- Cache de resumen en frontend (30s) para evitar queries repetidas
- (POST-MVP) Considerar cache distribuido (Redis) para `GetDashboardResumen`

---

## Alcance MVP

### IN SCOPE (Sprint 1)
- Dashboard principal con resumen de metricas
- Lista de campanias del artista (paginada)
- Detalle de campania con stats basicos
- Lista de backings recientes (paginada)
- Calculos de metricas (porcentaje, dias restantes, backing promedio)
- Autorizacion (solo ver propias campanias)
- Manejo de backings anonimos
- EmptyState para sin campanias / sin backings
- Responsive design (mobile, tablet, desktop)

### POST-MVP (Sprint 2+)
- Grafico de progreso temporal (linea de tiempo)
- Exportar lista de backers a CSV
- Filtros avanzados (por fecha, por reward, por monto)
- Ordenamiento customizado en tabla
- Notificaciones push al recibir backing
- WebSocket para actualizacion en tiempo real
- Dashboard de comparacion entre campanias
- Proyeccion avanzada con machine learning

---

## Testing

### Unit Tests (Backend)
- `GetDashboardResumenQueryHandlerTests`
  - `Handle_WhenArtistaHasCampanias_ReturnsCorrectStats`
  - `Handle_WhenArtistaWithoutCampanias_ReturnsZeros`
- `GetMisCampaniasQueryHandlerTests`
  - `Handle_WhenMultipleCampanias_ReturnsPaginatedList`
  - `Handle_WhenFilterByEstado_ReturnsOnlyMatchingCampanias`
- `GetCampaniaBackingsQueryHandlerTests`
  - `Handle_WhenBackingsExist_ReturnsPaginatedList`
  - `Handle_WhenAnonymousBacking_HidesPersonalInfo`
  - `Handle_WhenUnauthorized_Returns403`

### Integration Tests (Backend)
- `DashboardEndpoints_AsAuthenticatedArtista_ReturnsOwnDataOnly`
- `BackingsEndpoint_AsOtherArtista_Returns403`
- `DashboardEndpoints_WithoutToken_Returns401`

### Component Tests (Admin)
- `StatCard.test.tsx`
  - Renders label and value correctly
  - Shows trend when provided
  - Applies correct icon
- `ProgressBar.test.tsx`
  - Calculates percentage correctly
  - Applies color scheme based on progress
  - Shows percentage label when enabled
- `BackingTable.test.tsx`
  - Renders list of backings
  - Shows "Anonimo" for anonymous backers
  - Handles pagination correctly

### E2E Tests (Playwright)
- `dashboard-artista.e2e.ts`
  - Artista logs in and views dashboard
  - Dashboard shows correct number of campanias
  - Click on campania shows detail page
  - Backings list shows recent backings
  - Anonymous backing displays "Anonimo"

---

## Estimacion de Esfuerzo

| Tarea | Proyecto | Horas |
|-------|----------|-------|
| Definir DTOs y tipos compartidos | Shared | 1h |
| Crear schemas Zod | Shared | 0.5h |
| Implementar `GetDashboardResumenQuery` + Handler | Backend | 2h |
| Implementar `GetMisCampaniasQuery` + Handler | Backend | 2h |
| Implementar `GetCampaniaBackingsQuery` + Handler | Backend | 3h |
| Implementar `GetCampaniaStatsQuery` + Handler | Backend | 2h |
| Agregar autorizacion y validators | Backend | 1.5h |
| Unit tests para queries | Backend | 2h |
| Integration tests | Backend | 1.5h |
| Crear componentes UI (`StatCard`, `ProgressBar`, etc.) | Admin | 3h |
| Crear pagina dashboard principal | Admin | 2h |
| Crear pagina detalle campania | Admin | 2h |
| Integrar React Query hooks | Admin | 1.5h |
| Implementar polling (30s refresh) | Admin | 1h |
| Proteger rutas con middleware | Admin | 0.5h |
| Estilos responsive | Admin | 1.5h |
| Component tests | Admin | 2h |
| E2E tests | Admin | 1.5h |
| **TOTAL** | | **30h** |

**Nota:** Incluye margen de 20% para imprevistos. Grafico de progreso (POST-MVP) agregaria 4h adicionales.

---

## Referencia

- User Story completa: [docs/product/US-05-dashboard-artista.md](../../product/US-05-dashboard-artista.md)
- Template UI Dashboard: `references/templates/dashtail/` (layout sidebar, stats cards)
- ADR-006: Request Caching (aplicable a queries de dashboard)
- CQRS Pattern: `.claude/rules/backend/cqrs.rule.md`
