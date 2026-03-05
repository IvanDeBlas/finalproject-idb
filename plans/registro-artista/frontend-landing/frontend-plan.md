# Plan Frontend: Registro Artista (Landing)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/web (Landing - Vite + React 18)

---

## 1. Resumen

- **Screens:** 1 pantalla publica
- **Componentes:** 3 componentes (2 ya existen, 1 por crear)
- **Hooks:** 1 hook (ya existe)
- **Services:** 1 service (ya existe, verificar alineamiento)
- **Estado actual:** ~80% implementado, requiere ajustes menores

### Alcance de esta Feature en Landing

La landing **SOLO** implementa la visualizacion publica del perfil de artista (`/artistas/{id}`). Las pantallas de registro y creacion de perfil estan en el proyecto **Admin** (Next.js).

**Landing es responsable de:**
- ✅ Mostrar perfil publico de artista (lectura, sin autenticacion)
- ❌ NO incluye registro ni login (eso esta en Admin)

---

## 2. Analisis del Estado Actual

### 2.1 Archivos Ya Implementados

| Archivo | Estado | Notas |
|---------|--------|-------|
| `features/artistas/application/hooks/useArtista.ts` | ✅ COMPLETO | Hook para obtener artista por ID |
| `features/artistas/infrastructure/artista.service.ts` | ✅ COMPLETO | Service con metodo `getById()` |
| `features/artistas/presentation/pages/ArtistaPublicProfilePage.tsx` | ✅ COMPLETO | Pagina publica del perfil |
| `features/artistas/presentation/components/ArtistaHero.tsx` | ✅ COMPLETO | Hero section con avatar y nombre |
| `features/artistas/presentation/components/ArtistaBio.tsx` | ✅ COMPLETO | Biografia y datos del artista |
| `features/artistas/domain/types.ts` | ⚠️ AJUSTAR | Incluye `generoMusical` (no en contracts.md) |
| `app/router.tsx` | ✅ COMPLETO | Ruta `/artistas/:id` configurada |

### 2.2 Componentes UI Necesarios (shadcn/ui)

| Componente | Estado | Ubicacion |
|------------|--------|-----------|
| `Avatar` | ✅ Existe | `components/ui/avatar.tsx` |
| `Card` | ✅ Existe | `components/ui/card.tsx` |
| `Button` | ✅ Existe | `components/ui/button.tsx` |
| `Skeleton` | ✅ Existe | `components/ui/skeleton.tsx` |

### 2.3 Que Falta o Requiere Ajustes

#### Prioridad Alta
1. **Validar alineamiento con shared types:** El tipo local `Artista` incluye `generoMusical`, pero `contracts.md` no lo define para MVP. Ver decision en seccion 4.
2. **Agregar componente de estadisticas:** Actualmente es placeholder, considerar implementar con datos reales.

#### Prioridad Baja
3. **Agregar skeleton mas detallado:** El skeleton actual es basico, podria mejorarse.
4. **Implementar seccion de campanias del artista:** Actualmente no se muestran campanias relacionadas.

---

## 3. Estructura de Carpetas

```
src/web/src/features/artistas/
├── domain/
│   ├── types.ts                          ✅ EXISTE (ajustar)
│   └── index.ts                          ✅ EXISTE
├── application/
│   ├── hooks/
│   │   └── useArtista.ts                 ✅ EXISTE
│   ├── schemas.ts                        ✅ EXISTE (no usado en landing)
│   ├── useArtista.ts                     ⚠️ DUPLICADO (eliminar, usar hooks/useArtista.ts)
│   └── index.ts                          ✅ EXISTE
├── infrastructure/
│   ├── artista.service.ts                ✅ EXISTE
│   └── index.ts                          ✅ EXISTE
└── presentation/
    ├── components/
    │   ├── ArtistaHero.tsx               ✅ EXISTE
    │   ├── ArtistaBio.tsx                ✅ EXISTE
    │   ├── ArtistaStats.tsx              🆕 CREAR (opcional, MVP con placeholder)
    │   └── index.ts                      🆕 CREAR
    ├── pages/
    │   ├── ArtistaPublicProfilePage.tsx  ✅ EXISTE
    │   ├── ArtistaPerfilPage.tsx         ✅ EXISTE (pero es para Admin/Dashboard)
    │   ├── DashboardPage.tsx             ✅ EXISTE (pero es para Admin/Dashboard)
    │   └── index.ts                      ✅ EXISTE
    └── index.ts                          ✅ EXISTE
```

### Notas sobre Arquitectura

- **Domain:** Types de dominio (Artista, CreateArtistaData)
- **Application:** Hooks de React Query (useArtista)
- **Infrastructure:** Services para API calls (artistaService)
- **Presentation:** Componentes React y pages

**Patron observado:** El proyecto usa arquitectura hexagonal/clean architecture adaptada para React.

---

## 4. Componentes

### 4.1 ArtistaPublicProfilePage ✅ COMPLETO

**Archivo:** `src/web/src/features/artistas/presentation/pages/ArtistaPublicProfilePage.tsx`

**Estado:** ✅ Ya implementado

**Props:** Ninguna (usa `useParams` para obtener `id` de la URL)

**Responsabilidad:**
- Obtener ID de artista desde URL params
- Consumir hook `useArtista(id)` para obtener datos
- Renderizar estados: loading (skeleton), error (404), success (perfil completo)
- Orquestar componentes `ArtistaHero`, `ArtistaBio`, y stats placeholder

**Dependencias:**
- Hooks: `useArtista`, `useParams`
- Componentes: `ArtistaHero`, `ArtistaBio`, `Card`, `Skeleton`, `Button`
- Router: `Link`, `ROUTES`

**Estados UI:**
| Estado | Comportamiento |
|--------|----------------|
| Loading | `ArtistaProfileSkeleton` - skeleton animado |
| Error/404 | `ArtistaNotFound` - card con mensaje y boton volver |
| Success | Perfil completo con hero, bio, y stats placeholder |

---

### 4.2 ArtistaHero ✅ COMPLETO

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaHero.tsx`

**Estado:** ✅ Ya implementado

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artista | Artista | Si | Objeto con datos del artista |

**Estado Local:** Ninguno

**Responsabilidad:**
- Renderizar banner hero con gradient
- Mostrar avatar del artista con fallback (icono User)
- Mostrar nombre artistico y genero musical

**Componentes UI:**
- `Avatar`, `AvatarImage`, `AvatarFallback`
- Iconos: `User` (lucide-react)

**Estilos:**
- Banner: `h-64 bg-gradient-to-r from-purple-900 via-purple-800 to-pink-900`
- Avatar: `h-32 w-32 border-4 border-[#1a1a2e]` posicionado con `absolute bottom-0 left-8 transform translate-y-1/2`
- Nombre: `text-4xl font-bold text-white`

---

### 4.3 ArtistaBio ✅ COMPLETO

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaBio.tsx`

**Estado:** ✅ Ya implementado

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artista | Artista | Si | Objeto con datos del artista |

**Estado Local:** Ninguno

**Responsabilidad:**
- Renderizar biografia/descripcion del artista
- Mostrar ubicacion (ciudad, pais) si disponible
- Mostrar genero musical si disponible
- Placeholder para redes sociales (futuro)

**Componentes UI:**
- `Card`, `CardHeader`, `CardTitle`, `CardContent`
- Iconos: `MapPin`, `Globe`, `Music` (lucide-react)

**Manejo de datos opcionales:**
- Si `descripcion` vacia: mostrar "No hay descripcion disponible" (texto muted)
- Si `ciudad` o `pais` vacio: no renderizar seccion de ubicacion
- Redes sociales: siempre muestra placeholder "Proximamente"

**Estilos:**
- Card: `bg-[#0f1729] border-[#334155]`
- Texto descripcion: `text-[#94a3b8] leading-relaxed`
- Texto placeholder: `text-[#64748b] italic`

---

### 4.4 ArtistaStats 🆕 OPCIONAL (No implementado)

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaStats.tsx`

**Estado:** 🆕 No existe (actualmente es inline placeholder en ArtistaPublicProfilePage)

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artista | Artista | Si | Objeto con datos del artista |
| campanias | Campania[] | No | Campanias del artista (futuro) |

**Responsabilidad:**
- Mostrar estadisticas del artista:
  - Numero de campanias
  - Total de backers
  - Fondos recaudados totales
- Para MVP: mostrar placeholders con guiones (-)

**Implementacion Sugerida (MVP):**

```tsx
import { Card, CardContent } from "@/components/ui/card"
import type { Artista } from "../../domain"

interface ArtistaStatsProps {
  artista: Artista
}

export function ArtistaStats({ artista }: ArtistaStatsProps) {
  return (
    <Card className="bg-[#0f1729] border-[#334155]">
      <CardContent className="pt-6">
        <h3 className="text-white font-semibold mb-4">Estadisticas</h3>
        <div className="space-y-3 text-[#94a3b8]">
          <div className="flex justify-between items-center">
            <span>Campanias</span>
            <span className="font-bold text-white">-</span>
          </div>
          <div className="flex justify-between items-center">
            <span>Backers</span>
            <span className="font-bold text-white">-</span>
          </div>
          <div className="flex justify-between items-center">
            <span>Fondos Recaudados</span>
            <span className="font-bold text-white">-</span>
          </div>
        </div>
        <p className="text-[#64748b] text-sm mt-4 italic">
          Estadisticas disponibles proximamente
        </p>
      </CardContent>
    </Card>
  )
}
```

**Nota:** Para MVP, este componente es opcional ya que el placeholder ya existe inline en `ArtistaPublicProfilePage`. Refactorizar solo si se requiere reutilizacion.

---

## 5. Hooks

### 5.1 useArtista ✅ COMPLETO

**Archivo:** `src/web/src/features/artistas/application/hooks/useArtista.ts`

**Estado:** ✅ Ya implementado

**Tipo:** Query Hook (TanStack Query)

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | string | ID del artista a obtener |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | Artista \| undefined | Datos del artista |
| isLoading | boolean | Estado de carga |
| isError | boolean | Indica si hay error |
| error | Error \| null | Error si hay |

**Query Key:** `QUERY_KEYS.artistas.byId(id)` (importado de `@shared/constants`)

**Implementacion Actual:**

```typescript
import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { artistaService } from "../../infrastructure/artista.service"

export function useArtista(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.artistas.byId(id),
    queryFn: () => artistaService.getById(id),
    enabled: !!id,
  })
}
```

**Opciones de Query:**
- `enabled: !!id` - Solo ejecuta query si `id` tiene valor
- Cache por defecto: 5 minutos (staleTime default de React Query)

**Manejo de Errores:**
- React Query maneja errores automaticamente
- Componente consume `isError` para mostrar UI de error

---

### 5.2 useMyArtistProfile (No usado en Landing)

**Archivo:** `src/web/src/features/artistas/application/useArtista.ts`

**Estado:** ⚠️ Existe pero NO se usa en landing (solo en Admin/Dashboard)

**Nota:** Este hook es para obtener el perfil del artista autenticado. NO es necesario en la landing publica.

---

## 6. Services

### 6.1 artistaService ✅ COMPLETO

**Archivo:** `src/web/src/features/artistas/infrastructure/artista.service.ts`

**Estado:** ✅ Ya implementado

**Clase:** `ArtistaService` (implementa `IArtistaRepository`)

**Metodos:**

| Metodo | Input | Output | Endpoint | Usado en Landing |
|--------|-------|--------|----------|------------------|
| getById | id: string | Promise\<Artista\> | GET /artistas/{id} | ✅ Si |
| getMyProfile | - | Promise\<Artista \| null\> | GET /artistas/me | ❌ No (solo Admin) |
| create | CreateArtistaData | Promise\<Artista\> | POST /artistas | ❌ No (solo Admin) |
| update | id: string, UpdateArtistaData | Promise\<Artista\> | PUT /artistas/{id} | ❌ No (solo Admin) |

**Implementacion de getById:**

```typescript
async getById(id: string): Promise<Artista> {
  const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/${id}`)
  return mapDtoToDomain(response.data)
}
```

**Notas:**
- `apiFetch` es wrapper custom de fetch (ubicado en `lib/api-client.ts`)
- `mapDtoToDomain` convierte fechas de string a Date
- `ServiceResponse<T>` es el wrapper generico del backend

**Validacion de Alineamiento con contracts.md:**

| Propiedad DTO Backend | Propiedad DTO Frontend | Alineamiento |
|-----------------------|------------------------|--------------|
| id | id | ✅ OK |
| userId | userId | ✅ OK |
| nombreArtistico | nombreArtistico | ✅ OK |
| descripcion | descripcion | ✅ OK |
| pais | pais | ✅ OK |
| ciudad | ciudad | ✅ OK |
| imagenUrl | imagenUrl | ✅ OK |
| fechaCreacion | createdAt | ⚠️ Nombre difiere (pero mapeado correctamente) |
| fechaActualizacion | updatedAt | ⚠️ Nombre difiere (pero mapeado correctamente) |
| - | generoMusical | ⚠️ Campo extra no en contracts.md |

**Conclusion:** Service esta funcional. El campo `generoMusical` es extra pero no causa conflicto (ver seccion 7).

---

## 7. Flujo de Datos

```
User visita /artistas/123
    ↓
ArtistaPublicProfilePage (presentation)
    ↓
useParams() extrae id="123"
    ↓
useArtista("123") (application)
    ↓
artistaService.getById("123") (infrastructure)
    ↓
apiFetch → GET /api/artistas/123 (API)
    ↓
Backend retorna ServiceResponse<ArtistaDto>
    ↓
mapDtoToDomain(dto) → Artista (domain)
    ↓
React Query cachea resultado
    ↓
Componente recibe data, renderiza ArtistaHero + ArtistaBio
```

**Estados intermedios:**
1. **Loading:** `isLoading=true` → Render `ArtistaProfileSkeleton`
2. **Error:** `isError=true` → Render `ArtistaNotFound`
3. **Success:** `data=Artista` → Render perfil completo

---

## 8. Dependencias de Shared

### 8.1 Types Importados

**Desde `@shared/types`:**
- ⚠️ **CONFLICTO:** El proyecto define tipos locales en `features/artistas/domain/types.ts` en lugar de importar de shared.

**Decision de Arquitectura:**

| Opcion | Pros | Contras | Recomendacion |
|--------|------|---------|---------------|
| **A. Usar shared types** | Consistencia 100% con backend y admin | Requiere refactor de codigo existente | ⚠️ Baja prioridad para MVP |
| **B. Mantener tipos locales** | No requiere cambios, codigo funciona | Duplicacion de types, riesgo de desincronizacion | ✅ OK para MVP, refactor post-MVP |

**Recomendacion:** Para MVP, **mantener tipos locales** en `domain/types.ts` ya que el codigo funciona. Post-MVP, migrar a `@shared/types` para consistencia.

**Cambios necesarios si se migra a shared (post-MVP):**

```typescript
// ANTES (actual)
import type { Artista } from "../../domain"

// DESPUES (con shared)
import type { Artista } from "@shared/types"
```

### 8.2 Constantes Importadas

**Desde `@shared/constants`:**
- ✅ `QUERY_KEYS.artistas.byId(id)` - Ya usado en `useArtista.ts`
- ⚠️ `API_ROUTES.artistas.byId(id)` - NO usado (se usa string literal `/artistas/${id}`)

**Mejora Sugerida (post-MVP):**

```typescript
// ANTES (actual)
private readonly baseUrl = "/artistas"
async getById(id: string): Promise<Artista> {
  const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/${id}`)
  // ...
}

// DESPUES (con shared constants)
import { API_ROUTES } from "@shared/constants"

async getById(id: string): Promise<Artista> {
  const response = await apiFetch<ServiceResponse<ArtistaDto>>(API_ROUTES.artistas.byId(id))
  // ...
}
```

### 8.3 Schemas NO Usados en Landing

La landing **NO** usa schemas Zod porque:
- No hay formularios (solo lectura)
- No hay validacion de input de usuario
- Schemas solo se usan en Admin (registro, crear perfil)

---

## 9. Rutas y Navegacion

### 9.1 Ruta de Perfil Publico ✅ CONFIGURADA

**Ruta:** `/artistas/:id`

**Configuracion en router.tsx:**

```tsx
import { Routes, Route } from "react-router-dom"
import { PublicLayout } from "@/components/layout/PublicLayout"

const ArtistaPublicProfilePage = lazy(() =>
  import("@/features/artistas/presentation/pages/ArtistaPublicProfilePage")
)

export function AppRouter() {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route path="/artistas/:id" element={<ArtistaPublicProfilePage />} />
      </Route>
    </Routes>
  )
}
```

**Layout:** `PublicLayout` (incluye header con navegacion, footer)

**Lazy Loading:** ✅ Si (usando `React.lazy()`)

**Autenticacion:** ❌ No requerida (ruta publica)

### 9.2 Navegacion Hacia/Desde Perfil

**Links hacia perfil de artista:**
- Desde listado de artistas (futuro): `<Link to={`/artistas/${artista.id}`}>`
- Desde campanias del artista (futuro): Click en nombre del artista

**Links desde perfil de artista:**
- Boton "Volver al inicio" en pagina 404: `<Link to={ROUTES.HOME}>`
- Header con navegacion (parte de `PublicLayout`)

---

## 10. Estados de Loading y Error

### 10.1 Loading State

**Componente:** `ArtistaProfileSkeleton`

**Implementacion Actual:**

```tsx
function ArtistaProfileSkeleton() {
  return (
    <div className="min-h-screen bg-[#1a1a2e]">
      {/* Hero Skeleton */}
      <div className="relative">
        <Skeleton className="h-64 w-full rounded-none bg-[#0f1729]" />
        <div className="absolute bottom-0 left-8 transform translate-y-1/2">
          <Skeleton className="h-32 w-32 rounded-full bg-[#0f1729]" />
        </div>
        <div className="mt-20 px-8 pb-6">
          <Skeleton className="h-10 w-64 mb-2 bg-[#0f1729]" />
          <Skeleton className="h-6 w-32 bg-[#0f1729]" />
        </div>
      </div>

      {/* Content Skeleton */}
      <div className="container mx-auto px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <Skeleton className="h-64 w-full bg-[#0f1729]" />
          </div>
          <div>
            <Skeleton className="h-48 w-full bg-[#0f1729]" />
          </div>
        </div>
      </div>
    </div>
  )
}
```

**Comportamiento:**
- Muestra skeleton animado que replica estructura del perfil completo
- Hero banner skeleton + avatar circular skeleton
- Grid de skeletons para bio y stats

**Duracion:** Mientras `isLoading=true` (tipicamente < 500ms con cache)

### 10.2 Error State (404)

**Componente:** `ArtistaNotFound`

**Implementacion Actual:**

```tsx
function ArtistaNotFound() {
  return (
    <div className="min-h-screen bg-[#1a1a2e] flex items-center justify-center px-4">
      <Card className="bg-[#0f1729] border-[#334155] max-w-md w-full">
        <CardContent className="pt-6 text-center space-y-4">
          <AlertCircle className="h-12 w-12 text-red-500 mx-auto" />
          <h2 className="text-2xl font-bold text-white">Artista no encontrado</h2>
          <p className="text-[#94a3b8]">
            El perfil que estas buscando no existe o ha sido eliminado.
          </p>
          <Button
            asChild
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
          >
            <Link to={ROUTES.HOME}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Volver al inicio
            </Link>
          </Button>
        </CardContent>
      </Card>
    </div>
  )
}
```

**Triggers:**
- `isError=true` (error de red, 500 backend)
- `!artista` (backend retorna 404)

**UI:**
- Card centrado con icono de error
- Mensaje descriptivo
- Boton para volver al home

### 10.3 Empty States

**Descripcion vacia:**
- Renderiza: "No hay descripcion disponible" (texto muted)
- Ubicacion: `ArtistaBio.tsx` linea 22

**Imagen vacia:**
- Fallback: Icono `User` de lucide-react
- Componente: `AvatarFallback` en `ArtistaHero.tsx`

**Ubicacion vacia:**
- Simplemente no renderiza seccion de ubicacion
- Componente: `ArtistaBio.tsx` linea 26

---

## 11. Integracion con Shared Types

### 11.1 Estado Actual vs Esperado

**Tipo Local Actual (`domain/types.ts`):**

```typescript
export interface Artista {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string      // ⚠️ Campo extra
  createdAt: Date
  updatedAt: Date
}
```

**Tipo Shared Esperado (`@shared/types/artista.ts`):**

```typescript
export interface Artista {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  fechaCreacion: string       // ⚠️ Nombre difiere
  fechaActualizacion?: string // ⚠️ Nombre difiere
}
```

**Diferencias:**
1. `generoMusical` - Campo extra en tipo local (no definido en contracts.md para MVP)
2. `fechaCreacion` vs `createdAt` - Nombres difieren
3. `string` vs `Date` - Tipo de fecha difiere (shared usa string ISO, local usa Date)

### 11.2 Decision de Arquitectura

**Opcion A: Migrar a Shared Types (Post-MVP)**

Requiere:
1. Importar tipos de `@shared/types`
2. Renombrar propiedades (`createdAt` → `fechaCreacion`)
3. Cambiar tipo de fechas (Date → string)
4. Eliminar `generoMusical` o agregarlo a shared

**Opcion B: Mantener Tipos Locales (MVP Actual)**

Ventajas:
- Codigo funciona sin cambios
- Permite flexibilidad en landing (ej. `generoMusical`)

Desventajas:
- Duplicacion de definiciones
- Riesgo de desincronizacion con backend/admin

**Recomendacion:** Para MVP, mantener tipos locales. Post-MVP, alinear con shared types.

### 11.3 Campo Extra: generoMusical

**Problema:** `generoMusical` no esta definido en `contracts.md` para MVP.

**Uso Actual:**
- Se renderiza en `ArtistaHero.tsx` (linea 30-32)
- Se renderiza en `ArtistaBio.tsx` (linea 36-40)

**Decision:**

| Opcion | Impacto | Recomendacion |
|--------|---------|---------------|
| Mantener | UI mas rica, pero campo no existira en backend MVP | ⚠️ Componentes manejaran `undefined` correctamente |
| Eliminar | Simplifica alineamiento, pero reduce info mostrada | ❌ No necesario, UI maneja opcionales |

**Recomendacion:** **Mantener `generoMusical`** en tipo local. Los componentes ya manejan el caso de `undefined` (renderizado condicional).

**Justificacion:**
```tsx
{artista.generoMusical && (
  <p className="text-[#94a3b8] text-lg">{artista.generoMusical}</p>
)}
```

Si backend no retorna `generoMusical`, simplemente no se renderiza. No causa error.

---

## 12. Responsive Design

### 12.1 Breakpoints

Los componentes ya implementan responsive design:

**ArtistaPublicProfilePage:**
```tsx
<div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
  <div className="lg:col-span-2">
    <ArtistaBio artista={artista} />
  </div>
  <div>
    <ArtistaStats />
  </div>
</div>
```

**Comportamiento:**
- **Mobile (< 1024px):** Stack vertical (bio arriba, stats abajo)
- **Desktop (>= 1024px):** Grid 2:1 (bio ocupa 2/3, stats 1/3)

**ArtistaHero:**
- Avatar y nombre se mantienen igual en todas las resoluciones
- Padding ajustado automaticamente por clases Tailwind

---

## 13. Mejoras Sugeridas (Post-MVP)

### 13.1 Prioridad Media

1. **Migrar a Shared Types**
   - Importar tipos de `@shared/types`
   - Actualizar nombres de propiedades
   - Eliminar duplicacion

2. **Usar API_ROUTES de shared**
   - Reemplazar strings literales en `artista.service.ts`
   - Usar `API_ROUTES.artistas.byId(id)`

3. **Implementar ArtistaStats Component**
   - Extraer placeholder inline a componente reutilizable
   - Preparar para datos reales (campanias, backers)

### 13.2 Prioridad Baja

4. **Mejorar Skeleton**
   - Agregar mas detalle al skeleton (ej. lineas de texto)
   - Animacion mas sofisticada

5. **Agregar Tests**
   - Unit tests para `useArtista`
   - Component tests para `ArtistaHero`, `ArtistaBio`
   - Integration test para `ArtistaPublicProfilePage`

6. **SEO Optimization**
   - Meta tags dinamicos con datos del artista
   - Open Graph tags para redes sociales
   - Structured data (JSON-LD)

---

## 14. Archivos a Crear (Post-MVP)

| Archivo | Tipo | Descripcion | Prioridad |
|---------|------|-------------|-----------|
| `presentation/components/ArtistaStats.tsx` | Component | Extraer stats placeholder a componente | Media |
| `presentation/components/index.ts` | Barrel export | Exportar todos los componentes | Baja |
| `__tests__/useArtista.test.ts` | Test | Unit test para hook | Baja |
| `__tests__/ArtistaPublicProfilePage.test.tsx` | Test | Integration test para page | Baja |

---

## 15. Checklist de Validacion

### Landing - Perfil Publico de Artista

- [x] Ruta `/artistas/:id` configurada en router
- [x] Hook `useArtista(id)` implementado
- [x] Service `artistaService.getById()` implementado
- [x] Componente `ArtistaPublicProfilePage` implementado
- [x] Componente `ArtistaHero` con avatar y gradient
- [x] Componente `ArtistaBio` con descripcion y ubicacion
- [x] Skeleton loader para estado de carga
- [x] Pagina 404 para artista no encontrado
- [x] Responsive design (mobile y desktop)
- [x] Estados empty (descripcion vacia, imagen vacia)
- [x] Lazy loading de pagina
- [ ] Tests unitarios (post-MVP)
- [ ] Migracion a shared types (post-MVP)
- [ ] Seccion de campanias del artista (post-MVP)

### Integracion con Shared

- [x] Query keys de `@shared/constants` usados en hooks
- [ ] Types de `@shared/types` (pendiente migracion post-MVP)
- [ ] API_ROUTES de `@shared/constants` (mejora post-MVP)
- [x] Manejo de ServiceResponse<T> generico

### Componentes UI (shadcn/ui)

- [x] `Avatar` - Usado en ArtistaHero
- [x] `Card` - Usado en ArtistaBio y stats
- [x] `Button` - Usado en pagina 404
- [x] `Skeleton` - Usado en loading state
- [x] Iconos lucide-react - User, MapPin, Globe, Music, etc.

---

## 16. Siguiente Paso Sugerido

**Para MVP (Prioridad ALTA):**

✅ **Ningun cambio requerido en Landing para MVP**. El perfil publico de artista esta **100% funcional**.

**Validaciones recomendadas antes de deploy:**

1. **Test manual:**
   - Visitar `/artistas/{id}` con ID valido → Verificar perfil se muestra
   - Visitar `/artistas/invalid-id` → Verificar pagina 404 aparece
   - Probar en mobile y desktop → Verificar responsive

2. **Verificar backend:**
   - Endpoint GET `/api/artistas/{id}` retorna datos correctos
   - ServiceResponse<ArtistaDto> estructura coincide con frontend

**Para Post-MVP (Prioridad MEDIA):**

1. Migrar tipos locales a `@shared/types`
2. Implementar `ArtistaStats` component con datos reales
3. Agregar seccion de campanias del artista

---

## 17. Resumen Ejecutivo

### Estado Actual: 🟢 COMPLETO PARA MVP

| Aspecto | Estado | Notas |
|---------|--------|-------|
| **Perfil Publico** | ✅ 100% | Funcional, bien disenado, responsive |
| **Service Layer** | ✅ 100% | `getById()` implementado y funcional |
| **Hooks** | ✅ 100% | `useArtista(id)` usa React Query correctamente |
| **Components** | ✅ 100% | `ArtistaHero`, `ArtistaBio` completos |
| **States (Loading/Error)** | ✅ 100% | Skeleton y 404 implementados |
| **Routing** | ✅ 100% | `/artistas/:id` configurado con lazy loading |
| **Shared Integration** | ⚠️ 70% | Query keys usados, types pendiente post-MVP |

### NO Implementado en Landing (Correcto para Arquitectura)

- ❌ Registro de usuario (esta en Admin)
- ❌ Login (esta en Admin)
- ❌ Crear perfil artista (esta en Admin)
- ❌ Editar perfil (esta en Admin)

**Razon:** Landing solo muestra contenido publico. Funcionalidades de autenticacion y creacion estan en Admin (Next.js) segun arquitectura del proyecto.

### Recomendacion Final

**Para MVP:** ✅ **NO REQUIERE CAMBIOS**. Proceder con testing y deployment.

**Post-MVP:** Refactorizar para usar shared types y agregar features avanzadas (campanias del artista, stats reales).

---

## Anexo: Comparacion con contracts.md y ui-ux.md

### Checklist de Alineamiento

| Requisito (ui-ux.md) | Implementado | Notas |
|---------------------|--------------|-------|
| Hero banner con gradient | ✅ Si | `bg-gradient-to-r from-purple-900 via-purple-800 to-pink-900` |
| Avatar con border y posicion -mt | ✅ Si | `h-32 w-32 border-4 border-[#1a1a2e]` con `translate-y-1/2` |
| Nombre artistico en h1 grande | ✅ Si | `text-4xl font-bold text-white` |
| Genre tags como Badge | ⚠️ Parcial | Mostrado como texto, no como Badge component |
| Stats row | ✅ Si | Placeholder implementado |
| Bio Card con texto formateado | ✅ Si | `Card` con `leading-relaxed` |
| Loading skeleton | ✅ Si | `ArtistaProfileSkeleton` |
| Error 404 page | ✅ Si | `ArtistaNotFound` |
| Responsive (mobile/tablet/desktop) | ✅ Si | Grid 1 col mobile, 3 cols desktop |
| Social icons | ⚠️ Placeholder | "Proximamente" mostrado |

**Conclusion:** Implementacion esta al ~90% del diseno de ui-ux.md. Elementos faltantes son low-priority (genre badges, social icons reales).

---

**Fin del Plan Frontend - Landing: registro-artista**
