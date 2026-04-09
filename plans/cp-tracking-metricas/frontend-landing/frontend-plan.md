# Plan Frontend: cp-tracking-metricas (Landing)

**Fecha:** 2026-03-02
**Feature:** cp-tracking-metricas (US-CP-05)
**Target:** src/web (Vite + React 18)

---

## 1. Resumen

- **Screens:** 1 nueva (`/promotor/metricas`)
- **Componentes nuevos:** 9
- **Hooks nuevos:** 4
- **Services nuevos:** 1 (ampliacion de service existente)
- **Integraciones en app existente:** 2 (App.tsx, router.tsx)

### Alcance de esta feature en Landing

| Responsabilidad | Descripcion |
|-----------------|-------------|
| Tracking interceptor | Hook invisible montado en App.tsx que detecta `ref` y UTMs en la URL, los persiste en sessionStorage + cookie, y llama a `POST /api/crowdpromotion/tracking/evento` (tipo Click). Ademas engancha el evento PageView al navegar a `/campanias/:id` y el evento Signup al completar el registro. |
| Dashboard del promotor | Pagina `/promotor/metricas` protegida por autenticacion. Muestra selector de programa, 3 KPI cards, tasa de conversion inline, enlace referido con boton copiar, y lista de eventos recientes con badges de colores. |

### Lo que NO cubre este plan

- Dashboard del artista (Pantalla 2 de ui-ux.md) pertenece a `src/admin` (Next.js).
- El evento Backing se registra desde el backend (modulo Crowdfunding llama internamente al endpoint `/tracking/conversion`); el frontend no lo invoca directamente.
- La pagina de historial completo de eventos (`/promotor/eventos`) esta diferida fuera de MVP.

---

## 2. Estructura de Carpetas

```
src/web/src/
├── app/
│   ├── App.tsx                        MODIFICAR: agregar useTrackingInterceptor
│   └── router.tsx                     MODIFICAR: agregar ruta /promotor/metricas
│
└── features/
    └── crowdpromotion/                FEATURE EXISTENTE - agregar subfolder metricas
        └── metricas/                  NUEVO SUBFOLDER
            ├── domain/
            │   └── index.ts           Re-exports de tipos desde @shared
            ├── infrastructure/
            │   └── tracking.service.ts  Llamadas API de tracking y metricas
            ├── application/
            │   └── hooks/
            │       ├── useTrackingInterceptor.ts    Hook invisible de tracking
            │       ├── usePromotorMetricas.ts        Query hook de metricas del promotor
            │       ├── useMisProgramasParaSelector.ts Query hook para selector de programa
            │       └── useCopyToClipboard.ts         Hook de utilidad para copiar URL
            └── presentation/
                ├── pages/
                │   └── PromotorMetricasPage.tsx      Pagina principal /promotor/metricas
                └── components/
                    ├── ProgramaSelector.tsx           Dropdown de seleccion de programa
                    ├── MetricasKpiGrid.tsx             Grid de 3 KPI cards
                    ├── MetricasKpiCard.tsx             KPI card individual
                    ├── TasaConversionInline.tsx        Tasa de conversion (texto inline)
                    ├── EnlaceReferido.tsx              URL con boton copiar
                    ├── EventosRecientesList.tsx        Lista de eventos recientes
                    └── EventoTipoBadge.tsx             Badge coloreado por tipo de evento
```

### Justificacion de la ubicacion

La feature se integra dentro de `features/crowdpromotion/` como un subfolder `metricas/` en lugar de crear una feature raiz nueva `features/tracking/`. Esto sigue el patron de `features/crowdpromotion/tareas/` (US-CP-04) que agrupa sub-features relacionadas bajo el mismo dominio de crowdpromotion. El tracking y las metricas del promotor son parte del mismo dominio conceptual y comparten el service, la autenticacion y los tipos del dominio Crowdpromotion.

---

## 3. Integraciones en Archivos Existentes

### 3.1 App.tsx - Montar el interceptor de tracking

**Archivo:** `src/web/src/app/App.tsx`

**Cambio:** Importar y llamar `useTrackingInterceptor` en el componente `App`. El hook se monta una vez al cargar la SPA y no devuelve JSX.

**Patron de integracion:**

```
// Patron de uso en App.tsx (dentro de Providers que provee BrowserRouter)
// Crear un componente interno AppWithTracking que use el hook
// para que tenga acceso al BrowserRouter context si fuera necesario.
function AppWithTracking() {
    useTrackingInterceptor()  // Hook invisible, ejecuta una vez
    return (
        <>
            <AppRouter />
            <Toaster position="top-right" theme="dark" />
        </>
    )
}

export function App() {
    return (
        <Providers>
            <AppWithTracking />
        </Providers>
    )
}
```

**Dependencias:** `useTrackingInterceptor` desde `@/features/crowdpromotion/metricas/application/hooks/useTrackingInterceptor`

**Nota importante:** El hook `useTrackingInterceptor` se monta dentro del componente que tiene acceso al `BrowserRouter` (dentro de `<Providers>`) para poder leer `window.location.search` correctamente. No necesita `useLocation` de React Router porque lee directamente del objeto `window`, lo que evita problemas de contexto.

### 3.2 router.tsx - Agregar ruta protegida

**Archivo:** `src/web/src/app/router.tsx`

**Cambio:** Agregar la ruta `/promotor/metricas` dentro del bloque de rutas protegidas `<Route element={<DashboardLayout />}>`.

**Patron de integracion:**

```
// Lazy import
const PromotorMetricasPage = lazy(() =>
    import("@/features/crowdpromotion/metricas/presentation/pages/PromotorMetricasPage")
)

// Dentro del bloque DashboardLayout
<Route path="/promotor/metricas" element={<PromotorMetricasPage />} />
```

**Justificacion de DashboardLayout:** La ruta usa `DashboardLayout` (que incluye `<Navigate to={ROUTES.LOGIN}>` si no hay sesion) porque el promotor debe estar autenticado para ver sus metricas. Esto es consistente con `/promotor/mis-programas` y `/promotor/perfil` que ya usan el mismo layout.

### 3.3 APP_ROUTES - Agregar ruta de metricas

**Archivo:** `src/shared/constants/index.ts`

**Cambio:** Agregar la ruta de metricas del promotor dentro del objeto `APP_ROUTES.landing.crowdpromotion`. Esto sera implementado en el plan shared (ya planificado en contracts-plan.md). El plan frontend asume que la constante existira como:

```
APP_ROUTES.landing.crowdpromotion.misMetricas = '/promotor/metricas'
APP_ROUTES.landing.crowdpromotion.misMetricasConPrograma = (programaId: string) =>
    `/promotor/metricas?programaId=${programaId}`
```

### 3.4 Evento PageView - Integracion en CampaniaDetailPage

**Archivo:** `src/web/src/features/campanias/presentation/pages/CampaniaDetailPage.tsx`

**Cambio:** Agregar un `useEffect` al montar la pagina que llame a `trackingService.registrarEvento` con tipo PageView si existe un `codigoReferido` en sessionStorage.

**Patron de integracion:**

```
// En CampaniaDetailPage, al montar con campaniaId disponible
// Leer sessionStorage.getItem(TRACKING_STORAGE_KEYS.ref)
// Si hay ref -> trackingService.registrarEvento({ tipoEventoPromoId: TIPO_EVENTO_PROMO.PageView, campaniaCrowdfundingId: id, codigoReferido: ref, ... })
// Error silencioso: try/catch sin UI
```

**Dependencias:** `trackingService` desde `@/features/crowdpromotion/metricas/infrastructure/tracking.service`

### 3.5 Evento Signup - Integracion en useRegister

**Archivo:** `src/web/src/features/auth/application/useAuth.ts`

**Cambio:** En el `onSuccess` del `useRegister` mutation, despues de llamar a `login(data.user, data.token)`, registrar el evento Signup si existe un `codigoReferido` en sessionStorage.

**Patron de integracion:**

```
// En useRegister -> onSuccess
// Leer sessionStorage.getItem(TRACKING_STORAGE_KEYS.ref)
// Si hay ref -> trackingService.registrarEvento({ tipoEventoPromoId: TIPO_EVENTO_PROMO.Signup, codigoReferido: ref, userIdAfectado: data.user.id })
// Error silencioso: fire-and-forget con .catch(() => {})
```

**Nota:** Esta integracion es la de menor prioridad en MVP dado que requiere modificar el flujo de autenticacion existente. Se puede diferir si hay restricciones de tiempo.

---

## 4. Componentes

### 4.1 PromotorMetricasPage

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/pages/PromotorMetricasPage.tsx`

**Tipo:** Page component (default export para lazy loading)

**Props:** Ninguna (lee `programaId` de `useSearchParams`)

**Responsabilidad:**
- Leer el `programaId` del query param de la URL (`?programaId=xxx`) via `useSearchParams`.
- Orquestar los hooks `useMisProgramasParaSelector` y `usePromotorMetricas`.
- Renderizar los estados: loading, error, empty (sin programas), empty (sin eventos), y con datos.
- Actualizar el query param `programaId` en la URL al cambiar el selector sin navegar (usando `setSearchParams`).
- Contiene toda la estructura de layout de la pagina.

**Estado local:**
- `selectedProgramaId: string | undefined` - derivado del query param `programaId` via `useSearchParams`

**Dependencias:**
- Hooks: `useMisProgramasParaSelector`, `usePromotorMetricas`, `useAuthStore`
- Components: `ProgramaSelector`, `MetricasKpiGrid`, `TasaConversionInline`, `EnlaceReferido`, `EventosRecientesList`
- shadcn/ui: `Button`, `Skeleton`
- lucide-react: `BarChart2`, `AlertCircle`
- react-router-dom: `useSearchParams`, `useNavigate`
- shared: `APP_ROUTES`

**Logica de programaId inicial:**
Cuando el promotor llega sin `?programaId` en la URL, la pagina auto-selecciona el primer programa de la lista de `useMisProgramasParaSelector` mediante un `useEffect` que hace `setSearchParams({ programaId: programas[0].id })`. Esto activa automaticamente la query de metricas.

**Layout de la pagina:**
```
<div className="min-h-screen bg-[#1a1a2e]">
    <div className="max-w-4xl mx-auto px-4 pt-8 pb-10">
        {/* Cabecera: titulo + ProgramaSelector */}
        {/* MetricasKpiGrid */}
        {/* TasaConversionInline */}
        {/* EnlaceReferido */}
        {/* EventosRecientesList */}
    </div>
</div>
```

**Estados de render:**
| Estado | Condicion | Render |
|--------|-----------|--------|
| Loading programas | `programasQuery.isLoading` | 3 skeletons de KPI card + skeleton de enlace |
| Sin programas | `programas.length === 0` | Empty state con icono BarChart2 + boton "Ver programas" |
| Loading metricas | `metricasQuery.isLoading && selectedProgramaId` | Skeletons |
| Error metricas | `metricasQuery.isError` | Card de error con boton Reintentar |
| Con datos | `metricasQuery.data` | Render completo |

### 4.2 ProgramaSelector

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/ProgramaSelector.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `programas` | `ProgramaSelectorItem[]` | Si | Lista de programas para poblar el dropdown |
| `selectedId` | `string \| undefined` | Si | ID del programa actualmente seleccionado |
| `onSelect` | `(programaId: string) => void` | Si | Callback al cambiar la seleccion |
| `isLoading` | `boolean` | No | Muestra el select en estado de carga |

**Tipo auxiliar local:**
```typescript
interface ProgramaSelectorItem {
    id: string
    nombrePrograma: string
}
```

**Responsabilidad:**
- Renderizar el `<Select>` de shadcn con la lista de programas del promotor aprobados.
- Al cambiar la seleccion, llamar `onSelect(programaId)`.
- En estado `isLoading`, deshabilitar el select y mostrar un placeholder "Cargando...".

**Estilos clave:**
- `<Select>` con `w-64 bg-[#0f0f1f] border-[#334155] text-white text-sm focus:border-[#a855f7]`
- `<SelectItem>` con `text-sm text-white`
- `aria-label="Selecciona un programa de promocion"`

**Dependencias:**
- shadcn/ui: `Select`, `SelectContent`, `SelectItem`, `SelectTrigger`, `SelectValue`

### 4.3 MetricasKpiGrid

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/MetricasKpiGrid.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `kpis` | `PromotorMetricasKpis` | Si | KPIs del promotor para renderizar en las cards |
| `monedaNombre` | `string \| null` | Si | Nombre de la moneda (ej: "EUR") para el KPI de comision |
| `isLoading` | `boolean` | No | Si true renderiza skeletons en lugar de datos |

**Responsabilidad:**
- Renderizar el grid de 3 columnas con `MetricasKpiCard` para: Clicks, Conversiones, Comision.
- En estado `isLoading`, renderizar 3 `<Skeleton className="h-24 bg-[#1e1e38] rounded-xl animate-pulse" />`.
- El grid usa `grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6`.

**Dependencias:**
- Componentes: `MetricasKpiCard`
- shadcn/ui: `Skeleton`
- Tipos shared: `PromotorMetricasKpis`

**Configuracion de KPI cards** (definida como constante interna en el componente):
```
[
    { key: 'misClicks',         label: 'Mis clicks',    icon: MousePointerClick, color: '#3b82f6', bgColor: 'bg-blue-950/50',   monetario: false },
    { key: 'misConversiones',   label: 'Conversiones',  icon: ShoppingCart,      color: '#a855f7', bgColor: 'bg-purple-950/50', monetario: false },
    { key: 'miComisionAcumulada', label: 'Comision',    icon: Wallet,            color: '#10b981', bgColor: 'bg-green-950/50',  monetario: true  },
]
```

### 4.4 MetricasKpiCard

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/MetricasKpiCard.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `label` | `string` | Si | Etiqueta del KPI (ej: "Mis clicks") |
| `value` | `number` | Si | Valor numerico del KPI |
| `icon` | `LucideIcon` | Si | Icono de lucide-react para el KPI |
| `iconColor` | `string` | Si | Color del icono (ej: "#3b82f6") |
| `iconBgClass` | `string` | Si | Clase Tailwind del fondo del icono (ej: "bg-blue-950/50") |
| `monetario` | `boolean` | No | Si true, formatea como moneda con sufijo monedaNombre |
| `monedaNombre` | `string \| null` | No | Nombre de moneda para KPI monetario (ej: "EUR") |

**Responsabilidad:**
- Renderizar una card individual de KPI con icono, valor numerico grande y label.
- Si `monetario=true`, el valor se muestra en color `text-[#f59e0b]` con sufijo de moneda.
- Si `monetario=false`, el valor se muestra en `text-white`.
- Estructura: `<Card bg-[#151525] border-[#334155] p-5>` con icono + valor + label.

**Estilos del valor:**
- No monetario: `text-3xl font-bold text-white tabular-nums`
- Monetario: `text-3xl font-bold text-[#f59e0b] tabular-nums` + `<span className="text-lg text-[#64748b] ml-1">{monedaNombre}</span>`

**Dependencias:**
- shadcn/ui: `Card`
- lucide-react: (tipo `LucideIcon`, el icono especifico lo pasa el padre)

### 4.5 TasaConversionInline

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/TasaConversionInline.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tasa` | `number` | Si | Tasa de conversion en porcentaje (ej: 1.11 para "1.11%") |

**Responsabilidad:**
- Renderizar una sola linea con icono `TrendingUp`, label "Tasa de conversion:", el valor formateado con `formatTasaConversion(tasa)`, y la nota "(conversiones / clicks)".
- No hace fetch, solo renderiza los datos recibidos.

**Layout:**
```
<div className="flex items-center gap-2 mb-6 text-sm">
    <TrendingUp className="w-4 h-4 text-[#94a3b8]" />
    <span className="text-[#94a3b8]">Tasa de conversion:</span>
    <span className="font-semibold text-white">{formatTasaConversion(tasa)}</span>
    <span className="text-xs text-[#64748b]">(conversiones / clicks)</span>
</div>
```

**Dependencias:**
- lucide-react: `TrendingUp`
- shared utils: `formatTasaConversion`

### 4.6 EnlaceReferido

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/EnlaceReferido.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `url` | `string` | Si | URL completa del enlace referido con `?ref=CODIGO` y UTMs |

**Responsabilidad:**
- Renderizar la seccion de "Mi enlace de promocion" con el input readonly y el boton copiar.
- Manejar el estado del boton copiar (normal -> copiado -> normal) usando `useCopyToClipboard`.
- El estado "copiado" dura 2000ms. El feedback es en el boton, NO en un toast.
- Si `navigator.clipboard` no esta disponible, usar fallback `document.execCommand('copy')` y mostrar toast de exito. Si el fallback tambien falla, mostrar toast de error.

**Estado local:**
- Se delega a `useCopyToClipboard(url, 2000)` que retorna `{ copied, copy }`.

**Layout:**
```
<Card className="bg-[#151525] border-[#334155] p-5 mb-6">
    <p className="text-sm font-medium text-[#94a3b8] mb-3">Mi enlace de promocion</p>
    <div className="flex items-center gap-2">
        <Input readOnly value={url}
               className="bg-[#0f0f1f] border-[#334155] text-[#94a3b8] text-sm flex-1 font-mono cursor-default"
               aria-label="Tu enlace de promocion"
               aria-readonly="true" />
        <Button variant="outline" size="sm" onClick={copy}
                aria-label={copied ? "Enlace copiado" : "Copiar enlace de promocion"}
                className={copied ? "border-green-800/50 text-[#10b981]" : "border-[#334155] text-[#94a3b8]..."}>
            {copied ? <Check /> : <Copy />}
            {copied ? "Copiado!" : "Copiar"}
        </Button>
    </div>
</Card>
```

**Dependencias:**
- Hooks: `useCopyToClipboard`
- shadcn/ui: `Card`, `Input`, `Button`
- lucide-react: `Copy`, `Check`
- sonner: `toast` (solo para fallback de error)

### 4.7 EventosRecientesList

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/EventosRecientesList.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `eventos` | `EventoReciente[]` | Si | Lista de eventos recientes del promotor (max 20) |
| `programaId` | `string \| undefined` | No | Para construir el link "Ver todos" con query param |

**Responsabilidad:**
- Renderizar la card "Eventos recientes" con su cabecera y la lista de eventos.
- Si `eventos.length === 0`, mostrar empty state con icono `Activity` y texto "Aun no hay eventos...".
- Cada evento muestra: `EventoTipoBadge`, descripcion del evento, detalle de backing (si aplica), y fecha formateada.
- El boton "Ver todos" navega a `/promotor/eventos?programaId=xxx` (disabled si no hay tiempo en MVP, pero el enlace se renderiza aunque la pagina de destino no exista aun).

**Descripcion del evento por tipo:**
| tipoEventoId | Descripcion mostrada |
|-------------|----------------------|
| 1 (Click) | "Click desde {urlOrigen truncado}" o "Click recibido" |
| 2 (PageView) | "Visita a pagina de campana" |
| 3 (Signup) | "Registro de nuevo usuario" |
| 4 (Backing) | "{valorMonetario} {monedaNombre}" + detalle de comision si aplica |
| 5 (Share) | "Compartido en redes" |

**Formato de fecha:** Usar `Intl.DateTimeFormat` con locale `es-ES`, formato `"dd MMM yyyy, HH:mm"` (ej: "20 mar 2026, 14:30").

**Detalle para Backing (segunda linea):**
```
<p className="text-xs text-[#94a3b8]">
    {evento.valorMonetario} {evento.monedaNombre}
    {evento.comisionGenerada !== null && ` - ${evento.comisionGenerada} ${evento.monedaNombre} comision`}
</p>
```

**Dependencias:**
- Componentes: `EventoTipoBadge`
- shadcn/ui: `Card`, `Button`
- lucide-react: `Activity`, `ChevronRight`
- Tipos shared: `EventoReciente`

### 4.8 EventoTipoBadge

**Archivo:** `src/web/src/features/crowdpromotion/metricas/presentation/components/EventoTipoBadge.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| `tipoEventoId` | `TipoEventoPromo` | Si | ID numerico del tipo de evento (1-5) |

**Responsabilidad:**
- Renderizar un `<Badge>` de shadcn con el label y el color correcto para cada tipo de evento.
- Usa `mapTipoEventoPromoToLabel(tipoEventoId)` para el texto y `mapTipoEventoPromoToBadgeClass(tipoEventoId)` para las clases CSS.
- Componente puro sin estado ni efectos.

**Clases por tipo** (usando constantes del shared plan):
| tipoEventoId | Label | Clases CSS |
|-------------|-------|------------|
| 1 | Click | `bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs` |
| 2 | Vista | `bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs` |
| 3 | Registro | `bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs` |
| 4 | Backing | `bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs` |
| 5 | Share | `bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs` |

**Dependencias:**
- shadcn/ui: `Badge`
- shared utils: `mapTipoEventoPromoToLabel`, `mapTipoEventoPromoToBadgeClass`
- Tipos shared: `TipoEventoPromo`

### 4.9 Skeleton Wrappers (componentes de skeleton inline)

Los skeletons de esta feature se renderizan directamente en `PromotorMetricasPage` con el componente `<Skeleton>` de shadcn. No se crean componentes de skeleton separados (patrón del proyecto en `CampaniaDetailSkeleton.tsx` si, pero dado que los skeletons son simples, se inlinan en el componente padre).

**Skeletons a renderizar en PromotorMetricasPage:**
- KPI loading: `3x <Skeleton className="h-24 bg-[#1e1e38] rounded-xl animate-pulse" />`
- Enlace loading: `<Skeleton className="h-16 bg-[#1e1e38] rounded-xl animate-pulse" />`
- Eventos loading: `3x <Skeleton className="h-12 bg-[#1e1e38] rounded-lg animate-pulse" />`

---

## 5. Hooks

### 5.1 useTrackingInterceptor

**Archivo:** `src/web/src/features/crowdpromotion/metricas/application/hooks/useTrackingInterceptor.ts`

**Tipo:** Effect-only hook (no retorna datos)

**Parametros:** Ninguno

**Retorno:** `void`

**Responsabilidad completa:**

1. Al montar (via `useEffect` con dependencias vacías `[]`), leer `window.location.search`.
2. Extraer los parametros usando `URLSearchParams`:
   - `ref` (codigoReferido)
   - `utm_source`
   - `utm_medium`
   - `utm_campaign`
3. Si existe `ref`:
   a. Persistir en `sessionStorage`:
      - `TRACKING_STORAGE_KEYS.ref` = valor de `ref`
      - `TRACKING_STORAGE_KEYS.utmSource` = valor de `utm_source`
      - `TRACKING_STORAGE_KEYS.utmMedium` = valor de `utm_medium`
      - `TRACKING_STORAGE_KEYS.utmCampaign` = valor de `utm_campaign`
   b. Persistir en cookie `wp_ref` con TTL 30 minutos (`TRACKING_COOKIE_TTL_MINUTES`), `SameSite=Lax`, `Path=/`.
   c. Llamar `trackingService.registrarEvento({ tipoEventoPromoId: TIPO_EVENTO_PROMO.Click, codigoReferido: ref, urlOrigen: window.location.href, urlReferer: document.referrer || undefined, utmSource, utmMedium, utmCampaign })` dentro de un `try/catch`.
   d. Si el API devuelve error 429 (Too Many Requests): ignorar silenciosamente.
   e. Si el API devuelve error 400: limpiar `sessionStorage` (borrar claves de tracking).
   f. Cualquier otro error: ignorar silenciosamente.
4. Si NO existe `ref` en la URL: verificar si hay datos en `sessionStorage` (para atribucion en sesion). No se llama al API de tracking en este caso (solo el caso Click al llegar con `ref`).

**Logica de construccion de la cookie:**
```
document.cookie = `wp_ref=${ref}; max-age=${TRACKING_COOKIE_TTL_MINUTES * 60}; path=/; SameSite=Lax`
```

**Manejo de errores:**
Todo el bloque de llamada al API va en `try/catch`. El `catch` es silencioso (no hay UI, no hay log de error). Solo el caso 400 tiene side-effect (limpiar sessionStorage).

**Dependencias:**
- Services: `trackingService` desde `../../infrastructure/tracking.service`
- Shared constants: `TRACKING_PARAMS`, `TRACKING_STORAGE_KEYS`, `TRACKING_COOKIE_KEY`, `TRACKING_COOKIE_TTL_MINUTES`, `TIPO_EVENTO_PROMO`
- Tipos shared: `RegistrarEventoRequest`

**Importante:** El hook NO usa `useLocation` de React Router. Lee directamente de `window.location.search` para evitar dependencias del router context y simplificar la integracion.

### 5.2 usePromotorMetricas

**Archivo:** `src/web/src/features/crowdpromotion/metricas/application/hooks/usePromotorMetricas.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `programaId` | `string \| undefined` | ID del programa seleccionado. Si es undefined, la query esta deshabilitada |
| `filtros` | `FiltroFechas \| undefined` | Filtros opcionales de rango de fechas (no usados en MVP del promotor) |

**Retorno:** `UseQueryResult<PromotorMetricasResponse>` (retorno completo de `useQuery`)

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `data` | `PromotorMetricasResponse \| undefined` | Response completo con kpis y eventosRecientes |
| `isLoading` | `boolean` | True mientras carga |
| `isError` | `boolean` | True si hay error |
| `error` | `Error \| null` | Error si isError=true |
| `refetch` | `function` | Funcion para forzar refetch |

**Query Key:** `QUERY_KEYS.crowdpromotion.metricas.promotor(programaId, undefined, undefined)`

**QueryFn:** `() => trackingService.getMetricasPromotor({ programaId })`

**Opciones:**
- `enabled: !!programaId` - Solo ejecutar si hay un programa seleccionado
- `staleTime: 60 * 1000` - 1 minuto (los datos de metricas cambian relativamente poco)
- `retry: 1`

**Dependencias:**
- `useQuery` de TanStack Query
- `trackingService` desde `../../infrastructure/tracking.service`
- Shared constants: `QUERY_KEYS`
- Tipos shared: `PromotorMetricasResponse`, `FiltroFechas`

### 5.3 useMisProgramasParaSelector

**Archivo:** `src/web/src/features/crowdpromotion/metricas/application/hooks/useMisProgramasParaSelector.ts`

**Tipo:** Query Hook (TanStack Query)

**Parametros:** Ninguno

**Retorno:** Objeto con:
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `programas` | `ProgramaSelectorItem[]` | Lista simplificada de programas del promotor (solo id + nombre) |
| `isLoading` | `boolean` | Estado de carga |
| `isError` | `boolean` | Si hay error |

**Responsabilidad:**
- Reutiliza el hook existente `useMisProgramas` (del service de inscripciones) que ya devuelve la lista de programas del promotor.
- Mapea el resultado a `ProgramaSelectorItem[]` extrayendo solo `id` y `nombrePrograma` de cada item.
- Solo incluye programas con estado `INSCRIPCION_ESTADO.APROBADO` (el promotor solo puede ver metricas de programas donde esta aprobado).

**Tipo local:**
```typescript
interface ProgramaSelectorItem {
    id: string
    nombrePrograma: string
}
```

**Nota de implementacion:** En lugar de crear una query nueva, este hook actua como una fachada sobre `useMisProgramas` para simplificar la interfaz que necesita `ProgramaSelector`. Reutiliza la query key y el cache existente.

**Dependencias:**
- `useMisProgramas` desde `../../application/hooks/useMisProgramas` (hook existente en crowdpromotion)
- Shared constants: `INSCRIPCION_ESTADO`

### 5.4 useCopyToClipboard

**Archivo:** `src/web/src/features/crowdpromotion/metricas/application/hooks/useCopyToClipboard.ts`

**Tipo:** Utility Hook (estado local)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| `text` | `string` | Texto a copiar al portapapeles |
| `resetAfterMs` | `number` | Milisegundos hasta que `copied` vuelve a `false` (default: 2000) |

**Retorno:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `copied` | `boolean` | True durante los `resetAfterMs` ms tras copiar exitosamente |
| `copy` | `() => Promise<void>` | Funcion que ejecuta la copia |

**Logica:**
1. Intentar `navigator.clipboard.writeText(text)`.
2. Si tiene exito: `setCopied(true)`, programar `setTimeout(() => setCopied(false), resetAfterMs)`.
3. Si falla (API no disponible o denegado): intentar fallback con `document.execCommand('copy')` via un `textarea` temporal.
4. Si el fallback tiene exito: mostrar `toast.success('Enlace copiado al portapapeles')` y setear `copied(true)`.
5. Si el fallback tambien falla: mostrar `toast.error('No se pudo copiar. Copia el enlace manualmente.')`.

**Estado local:**
- `copied: boolean` inicializado en `false`
- `timeoutRef: ReturnType<typeof setTimeout> | null` para limpiar el timeout en unmount

**Dependencias:**
- `useState`, `useCallback`, `useEffect` de React
- sonner: `toast`

---

## 6. Services

### 6.1 trackingService

**Archivo:** `src/web/src/features/crowdpromotion/metricas/infrastructure/tracking.service.ts`

**Patron:** Clase de servicio (mismo patron que `InscripcionService` en `inscripcion.service.ts`).

**Metodos:**

#### registrarEvento

| Campo | Detalle |
|-------|---------|
| Input | `request: RegistrarEventoRequest` |
| Output | `Promise<RegistrarEventoResponse>` |
| Endpoint | `POST API_ROUTES.crowdpromotion.tracking.evento` |
| Auth | No requiere (endpoint publico) |
| Manejo de 429 | Relanzar el error tal cual para que el caller lo capture y lo ignore silenciosamente |
| Manejo de errores generales | Relanzar; el caller (hook useTrackingInterceptor) tiene su propio try/catch |

**Comportamiento especial:** A diferencia de otros services que usan `apiFetch` (que tiene un interceptor de 401 -> redirect to login), el endpoint de tracking es publico. Sin embargo, `apiFetch` usa `apiClient` que añade el token JWT si existe en localStorage, lo cual es aceptable para un endpoint publico (el backend simplemente ignorara el token si no es necesario). No se necesita un cliente HTTP separado.

#### getMetricasPromotor

| Campo | Detalle |
|-------|---------|
| Input | `filtros: FiltroMetricasPromotor` (`{ programaId?: string, fechaDesde?: string, fechaHasta?: string }`) |
| Output | `Promise<PromotorMetricasResponse>` |
| Endpoint | `GET API_ROUTES.crowdpromotion.promotorMetricas` con query params |
| Auth | Bearer JWT (el promotor debe estar autenticado) |
| Error handling | Usar `extractError` helper (mismo patron que `InscripcionService`) |

**Construccion de query params:**
```
const params = new URLSearchParams()
if (filtros.programaId) params.set('programaId', filtros.programaId)
if (filtros.fechaDesde) params.set('fechaDesde', filtros.fechaDesde)
if (filtros.fechaHasta) params.set('fechaHasta', filtros.fechaHasta)
const url = params.toString()
    ? `${API_ROUTES.crowdpromotion.promotorMetricas}?${params.toString()}`
    : API_ROUTES.crowdpromotion.promotorMetricas
```

**Helper `extractError`:** Mismo patron que `inscripcion.service.ts`:
```typescript
function extractError(messages: Array<{ message: string; errorCode: string }>): Error {
    const firstError = messages.find(m => m.errorCode && !m.errorCode.startsWith("0"))
    const error = new Error(firstError?.message || "Error desconocido")
    ;(error as Error & { errorCode: string }).errorCode = firstError?.errorCode ?? "5000"
    return error
}
```

**Wrapper `ServiceResponse`:**
```typescript
interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}
```

**Dependencias:**
- `apiFetch` desde `@/lib/api-client`
- Shared constants: `API_ROUTES`
- Tipos shared: `RegistrarEventoRequest`, `RegistrarEventoResponse`, `PromotorMetricasResponse`, `FiltroMetricasPromotor`

---

## 7. Dominio Local

### 7.1 domain/index.ts

**Archivo:** `src/web/src/features/crowdpromotion/metricas/domain/index.ts`

**Responsabilidad:** Re-exportar los tipos de shared que usa esta feature para no importar directamente desde `@shared` en los componentes (patron de otras features que tienen `domain/types.ts`).

**Exports:**
```typescript
export type {
    TipoEventoPromo,
    RegistrarEventoRequest,
    RegistrarEventoResponse,
    FiltroFechas,
    FiltroMetricasPromotor,
    PromotorMetricasKpis,
    EventoReciente,
    PromotorMetricasResponse,
} from "@shared/types/crowdpromotion"
```

---

## 8. Flujo de Datos

### 8.1 Flujo de Tracking (evento Click)

```
Usuario navega a /campanias/xxx?ref=album-2026&utm_source=weplay&utm_medium=referral&utm_campaign=prog-1
    |
    v
App.tsx monta -> AppWithTracking monta -> useTrackingInterceptor() ejecuta (useEffect [])
    |
    v
Lee window.location.search -> extrae { ref, utm_source, utm_medium, utm_campaign }
    |
    v
ref existe? SI
    |
    v
sessionStorage.setItem(TRACKING_STORAGE_KEYS.ref, 'album-2026')
document.cookie = 'wp_ref=album-2026; max-age=1800; path=/; SameSite=Lax'
    |
    v
trackingService.registrarEvento({ tipoEventoPromoId: 1, codigoReferido: 'album-2026', urlOrigen, utmSource, utmMedium, utmCampaign })
    |
    v (try/catch silencioso)
POST /api/crowdpromotion/tracking/evento
    |
    v
Backend crea PromoEvento (tipo Click) -> devuelve 201 { eventoId, registrado: true }
    |
    v
Frontend ignora la respuesta (no hay UI update)
```

### 8.2 Flujo de Metricas del Promotor

```
Usuario navega a /promotor/metricas
    |
    v
DashboardLayout verifica autenticacion (isAuthenticated del auth-store)
    |
    v
PromotorMetricasPage monta
    |
    v
useMisProgramasParaSelector() -> useQuery [crowdpromotion, inscripciones, mis-programas, ...]
    |                                 -> (reutiliza cache de useMisProgramas si existe)
    v
Lista de programas disponible
    |
    v
ProgramaSelector renderiza lista | useEffect auto-selecciona primer programa
    |
    v
URL actualizada: /promotor/metricas?programaId=xxxxx (via setSearchParams)
    |
    v
usePromotorMetricas('xxxxx') -> useQuery [crowdpromotion, metricas, promotor, xxxxx, ...]
    |
    v
trackingService.getMetricasPromotor({ programaId: 'xxxxx' })
    |
    v
GET /api/crowdpromotion/promotor/metricas?programaId=xxxxx (con Bearer JWT)
    |
    v
Backend devuelve PromotorMetricasResponse { promotorId, kpis, eventosRecientes, ... }
    |
    v
PromotorMetricasPage recibe data -> renderiza MetricasKpiGrid + TasaConversionInline + EnlaceReferido + EventosRecientesList
```

### 8.3 Flujo de Copia del Enlace

```
Usuario hace click en boton "Copiar" de EnlaceReferido
    |
    v
useCopyToClipboard.copy() se ejecuta
    |
    v
navigator.clipboard.writeText(url) (API Clipboard)
    |--- Exito -> setCopied(true) -> boton muestra "Copiado!" durante 2000ms
    |
    |--- Error (Clipboard API no disponible)
         -> Fallback execCommand
         |--- Exito -> toast.success + setCopied(true)
         |--- Error -> toast.error
```

---

## 9. Dependencias de Shared

**Importar de `@shared/types/crowdpromotion`:**
- `TipoEventoPromo`
- `RegistrarEventoRequest`
- `RegistrarEventoResponse`
- `FiltroFechas`
- `FiltroMetricasPromotor`
- `PromotorMetricasKpis`
- `EventoReciente`
- `PromotorMetricasResponse`

**Importar de `@shared/constants`:**
- `API_ROUTES.crowdpromotion.tracking.evento`
- `API_ROUTES.crowdpromotion.promotorMetricas`
- `QUERY_KEYS.crowdpromotion.metricas.promotor`
- `TIPO_EVENTO_PROMO` (constantes numericas 1-5)
- `TIPO_EVENTO_PROMO_LABELS`
- `INSCRIPCION_ESTADO`
- `APP_ROUTES.landing.crowdpromotion.misMetricas` (si se agrega en shared)

**Importar de `@shared/constants/tracking`:**
- `TRACKING_PARAMS`
- `TRACKING_STORAGE_KEYS`
- `TRACKING_COOKIE_KEY`
- `TRACKING_COOKIE_TTL_MINUTES`

**Importar de `@shared/utils/mappers`:**
- `mapTipoEventoPromoToLabel`
- `mapTipoEventoPromoToBadgeClass`

**Importar de `@shared/utils/format`:**
- `formatTasaConversion`

**Prerequisito:** El plan shared (`plans/cp-tracking-metricas/shared/contracts-plan.md`) debe implementarse antes que este plan, ya que todos estos exports dependen de los cambios en `src/shared/`.

---

## 10. Archivos a Crear o Modificar

### Archivos Nuevos

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `src/web/src/features/crowdpromotion/metricas/domain/index.ts` | Domain re-exports | Re-exports de tipos desde @shared |
| `src/web/src/features/crowdpromotion/metricas/infrastructure/tracking.service.ts` | Service | registrarEvento + getMetricasPromotor |
| `src/web/src/features/crowdpromotion/metricas/application/hooks/useTrackingInterceptor.ts` | Hook (effect-only) | Detecta ref/UTMs en URL, persiste en sesion, llama API Click |
| `src/web/src/features/crowdpromotion/metricas/application/hooks/usePromotorMetricas.ts` | Query Hook | GET metricas del promotor con TanStack Query |
| `src/web/src/features/crowdpromotion/metricas/application/hooks/useMisProgramasParaSelector.ts` | Query Hook (fachada) | Wrappea useMisProgramas para el selector |
| `src/web/src/features/crowdpromotion/metricas/application/hooks/useCopyToClipboard.ts` | Utility Hook | Clipboard API con fallback y feedback |
| `src/web/src/features/crowdpromotion/metricas/presentation/pages/PromotorMetricasPage.tsx` | Page | Pagina /promotor/metricas (default export) |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/ProgramaSelector.tsx` | Component | Dropdown de seleccion de programa |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/MetricasKpiGrid.tsx` | Component | Grid de 3 KPI cards |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/MetricasKpiCard.tsx` | Component | KPI card individual |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/TasaConversionInline.tsx` | Component | Linea de tasa de conversion |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/EnlaceReferido.tsx` | Component | URL readonly + boton copiar |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/EventosRecientesList.tsx` | Component | Lista de eventos recientes con badges |
| `src/web/src/features/crowdpromotion/metricas/presentation/components/EventoTipoBadge.tsx` | Component | Badge coloreado por tipo de evento |

### Archivos Modificados

| Archivo | Tipo de cambio | Descripcion |
|---------|---------------|-------------|
| `src/web/src/app/App.tsx` | Modificacion | Agregar `useTrackingInterceptor` via componente interno `AppWithTracking` |
| `src/web/src/app/router.tsx` | Modificacion | Agregar lazy import y ruta `/promotor/metricas` en DashboardLayout block |
| `src/web/src/features/campanias/presentation/pages/CampaniaDetailPage.tsx` | Modificacion | Agregar useEffect para evento PageView si hay ref en sessionStorage |
| `src/web/src/features/auth/application/useAuth.ts` | Modificacion (baja prioridad) | Agregar registro de evento Signup en onSuccess de useRegister |

---

## 11. Consideraciones de Implementacion

### 11.1 Orden de Implementacion Recomendado

1. **Primero:** Implementar el plan shared (types, constants, mappers en `src/shared/`)
2. **Segundo:** Crear `tracking.service.ts` (foundation de la feature)
3. **Tercero:** Crear `useTrackingInterceptor` + integracion en `App.tsx` (AC-CP05-1 y AC-CP05-2)
4. **Cuarto:** Crear `PromotorMetricasPage` con todos sus subcomponentes (AC-CP05-12 a AC-CP05-14)
5. **Quinto:** Agregar ruta en `router.tsx`
6. **Sexto:** Integrar PageView en `CampaniaDetailPage` (AC-CP05-3)
7. **Septimo (si hay tiempo):** Integrar Signup en `useAuth.ts` (AC-CP05-4)

### 11.2 sessionStorage vs Cookie

El plan usa ambos mecanismos como especifica la UI/UX:
- `sessionStorage`: persiste hasta cerrar el tab. Mas facil de leer en JS.
- Cookie `wp_ref`: persiste 30 minutos, sobrevive si el usuario abre una nueva tab del mismo dominio.

Para leer el codigoReferido en la integracion de PageView y Signup, usar siempre `sessionStorage.getItem(TRACKING_STORAGE_KEYS.ref)` primero. Si es null, intentar leer la cookie como fallback.

### 11.3 El selector de programa y los query params

El `programaId` seleccionado se refleja en la URL como query param (`?programaId=xxx`) para que:
- La URL sea shareable/bookmarkable.
- Al recargar la pagina, se mantenga el programa seleccionado.
- TanStack Query invalida automaticamente al cambiar el query param (porque el `programaId` es parte de la query key).

El manejo se hace con `useSearchParams` de react-router-dom:
```typescript
const [searchParams, setSearchParams] = useSearchParams()
const selectedProgramaId = searchParams.get('programaId') ?? undefined
const handleProgramaSelect = (id: string) => setSearchParams({ programaId: id })
```

### 11.4 URL del enlace referido

La URL del enlace referido del promotor se construye en `PromotorMetricasPage` a partir de los datos que devuelve `PromotorMetricasResponse`. El backend no devuelve directamente la URL completa del enlace, sino el `codigoReferido` del promotor en la inscripcion (disponible en `useMisProgramas`).

La URL se construye como:
```
`${window.location.origin}/campanias/${campaniaCrowdfundingId}?ref=${codigoReferido}&utm_source=weplay&utm_medium=referral&utm_campaign=${codigoTrackingBaseDelPrograma}`
```

**Nota:** Para obtener el `codigoReferido` y `campaniaCrowdfundingId`, el componente `PromotorMetricasPage` debe cruzar los datos de `useMisProgramas` (que tiene el `codigoReferido` de la inscripcion) con los datos del programa. Si el endpoint `GET /promotor/metricas` no devuelve el `codigoReferido` directamente, es necesario leerlo de la inscripcion correspondiente al programa seleccionado desde la respuesta de `useMisProgramas`.

**Alternativa si el backend no devuelve codigoReferido en metricas:** El endpoint `GET /api/crowdpromotion/promotor/mis-programas` devuelve la lista de inscripciones incluyendo el `codigoReferido` de cada una. El hook `useMisProgramasParaSelector` puede incluir el `codigoReferido` en el `ProgramaSelectorItem` para pasarlo directamente a `EnlaceReferido`.

### 11.5 Accesibilidad

Elementos criticos que requieren atencion en implementacion:
- `<Select>` de ProgramaSelector: `aria-label="Selecciona un programa de promocion"`
- `<Input readOnly>` de EnlaceReferido: `aria-label="Tu enlace de promocion"` + `aria-readonly="true"`
- Boton copiar: `aria-label` dinamico segun estado `copied`
- Contenedor de skeletons durante carga: `aria-busy="true"`
- Cards de error: `role="alert"`
- Lista de eventos: no requiere `role="list"` explicitamente (los `<div>` con semantica visual son suficientes para MVP)

### 11.6 Stale Time de Queries

| Query | Stale Time | Justificacion |
|-------|------------|---------------|
| `useMisProgramas` (reutilizada) | 30 segundos | Ya configurada en el hook existente |
| `usePromotorMetricas` | 60 segundos | Los datos de metricas no cambian en tiempo real. Un promotor rara vez hace backing en el mismo minuto en que consulta sus metricas. |

### 11.7 Error Codes Especificos

El hook `usePromotorMetricas` puede recibir estos error codes del backend:
- `3001` (401): Token expirado. El interceptor de `apiClient` ya hace redirect a login automaticamente.
- `2015` (404): No existe perfil de promotor. Mostrar mensaje "No tienes un perfil de promotor" y boton que navega a `/promotor/registro`.

El `PromotorMetricasPage` debe manejar el error code `2015` especificamente para mostrar un mensaje util en lugar del error generico. Patron de referencia: `MisTareasPage` que maneja `errorCode === "4026"` con `toast.error + navigate`.

---

## 12. Dependencias de Paquetes

No se requieren paquetes adicionales. Todos los packages necesarios ya estan instalados en `src/web`:
- `@tanstack/react-query` - ya en uso
- `react-hook-form` + `@hookform/resolvers` + `zod` - ya en uso (solo para filtros de fecha si se implementan)
- `sonner` - ya en uso (para toasts de fallback de copia)
- `lucide-react` - ya en uso
- shadcn/ui components ya disponibles: `Card`, `Button`, `Input`, `Badge`, `Skeleton`, `Select`

---

## 13. Checklist de Implementacion

### Tracking Interceptor

- [ ] `useTrackingInterceptor` lee `ref`, `utm_source`, `utm_medium`, `utm_campaign` de `window.location.search`
- [ ] Persiste `ref` en `sessionStorage` con claves de `TRACKING_STORAGE_KEYS`
- [ ] Crea cookie `wp_ref` con TTL 30 minutos y SameSite=Lax
- [ ] Llama `trackingService.registrarEvento` con tipo Click (tipoEventoPromoId=1)
- [ ] Maneja 429 silenciosamente (no reintenta)
- [ ] Maneja 400 limpiando sessionStorage
- [ ] Todos los errores tienen try/catch silencioso
- [ ] Hook montado en `App.tsx` via componente interno `AppWithTracking`
- [ ] No bloquea el render de la aplicacion

### Dashboard del Promotor

- [ ] Ruta `/promotor/metricas` registrada en `router.tsx` dentro del bloque `DashboardLayout`
- [ ] `PromotorMetricasPage` usa `useSearchParams` para leer/escribir `programaId`
- [ ] Auto-seleccion del primer programa si no hay `programaId` en la URL
- [ ] `ProgramaSelector` muestra lista de programas del promotor (solo aprobados)
- [ ] `MetricasKpiGrid` renderiza 3 cards: Clicks (azul), Conversiones (purpura), Comision (verde)
- [ ] Card de comision muestra valor en color ambar con sufijo de moneda
- [ ] `TasaConversionInline` usa `formatTasaConversion(kpis.miTasaConversion)`
- [ ] `EnlaceReferido` muestra URL con `?ref=CODIGO` y UTMs
- [ ] Boton copiar usa `useCopyToClipboard` con feedback de 2000ms en el boton (no toast)
- [ ] `EventosRecientesList` muestra hasta 20 eventos
- [ ] Cada evento tiene `EventoTipoBadge` con color correcto
- [ ] Eventos tipo Backing muestran segunda linea con valorMonetario y comisionGenerada
- [ ] Estado loading muestra skeletons
- [ ] Estado sin programas muestra empty state con boton "Ver programas disponibles"
- [ ] Estado sin eventos muestra empty state con icono Activity
- [ ] Estado error muestra icono AlertCircle con boton Reintentar
- [ ] Error code `2015` muestra mensaje especifico de "sin perfil de promotor"
- [ ] Responsive: 1 columna en mobile, 3 columnas KPI en sm+

### Integraciones Adicionales

- [ ] `CampaniaDetailPage` registra evento PageView si hay ref en sessionStorage (AC-CP05-3)
- [ ] `useRegister.onSuccess` registra evento Signup si hay ref en sessionStorage (AC-CP05-4, baja prioridad)

### Tipos y Shared

- [ ] Todos los tipos importados de `@shared/types/crowdpromotion` (no duplicados localmente)
- [ ] Constantes de tracking importadas de `@shared/constants/tracking`
- [ ] `mapTipoEventoPromoToLabel` y `mapTipoEventoPromoToBadgeClass` usados en `EventoTipoBadge`
- [ ] `formatTasaConversion` usado en `TasaConversionInline`
- [ ] No se usa `any` en ningun archivo

### Calidad

- [ ] Componentes usan shadcn/ui (`Card`, `Button`, `Input`, `Badge`, `Skeleton`, `Select`)
- [ ] No hay `console.log` en codigo productivo
- [ ] Props tipadas con interfaces explicitas en cada componente
- [ ] Hooks de query usan query keys de `QUERY_KEYS.crowdpromotion.metricas.*`
- [ ] Services usan `apiFetch` de `@/lib/api-client`
- [ ] Atributos de accesibilidad en elementos interactivos
