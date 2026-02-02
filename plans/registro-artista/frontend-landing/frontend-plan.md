# Plan Frontend: Registro Artista (Landing)

**Fecha:** 2026-01-26
**Feature:** registro-artista
**Target:** src/web (Vite + React 18)

## 1. Resumen

- Screens: 1 (Perfil Publico de Artista)
- Componentes: 7 (ArtistaProfilePage, ArtistaHeroBanner, ArtistaAvatar, ArtistaBio, ArtistaStats, ArtistaSocialLinks, ArtistaCampaigns)
- Hooks: 1 (useArtista)
- Services: 1 metodo adicional (getArtistaById)

**Alcance MVP:** Solo pagina publica de perfil de artista en `/artistas/:id`. No incluye funcionalidad de registro (eso esta en Admin).

---

## 2. Estructura de Carpetas

```
src/web/src/features/artistas/
├── domain/
│   ├── types.ts                  # Re-export de shared (Artista, ArtistaListItem)
│   └── index.ts                  # Barrel export
│
├── application/
│   ├── hooks/
│   │   └── useArtista.ts         # Query hook para obtener artista por ID
│   └── index.ts                  # Barrel export
│
├── infrastructure/
│   └── artista.service.ts        # ACTUALIZAR: agregar metodo getById()
│
└── presentation/
    ├── components/
    │   ├── ArtistaHeroBanner.tsx
    │   ├── ArtistaAvatar.tsx
    │   ├── ArtistaBio.tsx
    │   ├── ArtistaStats.tsx
    │   ├── ArtistaSocialLinks.tsx
    │   ├── ArtistaCampaigns.tsx
    │   └── index.ts
    └── pages/
        ├── ArtistaProfilePage.tsx   # ACTUALIZAR: convertir en perfil publico
        └── index.ts
```

**Nota:** La feature `artistas` ya existe. Solo se agregan componentes nuevos y se actualiza el service/hook.

---

## 3. Componentes

### 3.1 ArtistaProfilePage (Page Component)

**Archivo:** `src/web/src/features/artistas/presentation/pages/ArtistaProfilePage.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| - | - | - | Obtiene `id` de URL params |

**Estado Local:**
- Ninguno (delegado a useArtista hook)

**Dependencias:**
- Hooks: `useArtista(id)` de `application/hooks/useArtista.ts`
- Componentes UI: `Card`, `CardContent`, `CardHeader`, `CardTitle`, `Badge`, `Button`, `Avatar`
- Componentes Feature: `ArtistaHeroBanner`, `ArtistaAvatar`, `ArtistaBio`, `ArtistaStats`, `ArtistaSocialLinks`, `ArtistaCampaigns`
- Router: `useParams` de react-router-dom

**Responsabilidad:**
- Obtener `id` de URL params
- Llamar `useArtista(id)` para cargar datos
- Renderizar layout completo del perfil publico
- Manejar estados: loading (skeleton), error (404 page), success (mostrar perfil)

**Layout:**
```tsx
<div className="min-h-screen bg-[#1a1a2e]">
  {/* Hero Banner */}
  <ArtistaHeroBanner imagenUrl={artista.imagenUrl} />

  {/* Main Content */}
  <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 -mt-16 pb-12">
    {/* Avatar + Header */}
    <div className="flex flex-col sm:flex-row items-center gap-6 mb-8">
      <ArtistaAvatar
        imagenUrl={artista.imagenUrl}
        nombreArtistico={artista.nombreArtistico}
        size="2xl"
      />
      <div className="flex-1 text-center sm:text-left">
        <h1 className="text-4xl font-bold text-white">{artista.nombreArtistico}</h1>
        <div className="flex flex-wrap justify-center sm:justify-start gap-2 mt-2">
          {/* Genre badges - hardcoded por ahora (no estan en contracts) */}
        </div>
        <ArtistaStats artista={artista} />
      </div>
      <div className="flex items-center gap-3">
        <Button variant="gradient">Seguir</Button>
        <ArtistaSocialLinks />
      </div>
    </div>

    {/* Grid: Bio | Campaigns | Reviews (futuro) */}
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div className="lg:col-span-2">
        <ArtistaBio descripcion={artista.descripcion} />
      </div>
      <div>
        <ArtistaCampaigns artistaId={artista.id} />
      </div>
    </div>
  </div>
</div>
```

**Estados de UI:**
| Estado | Comportamiento |
|--------|----------------|
| Loading | Mostrar skeleton loaders (hero, avatar, cards) |
| Error 404 | Mostrar mensaje "Artista no encontrado" centrado con link a home |
| Success | Renderizar perfil completo |

**Ejemplo Error 404:**
```tsx
if (error?.response?.status === 404) {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-[#1a1a2e] text-white">
      <h1 className="text-6xl font-bold mb-4">404</h1>
      <p className="text-xl text-[#94a3b8] mb-6">Artista no encontrado</p>
      <Button asChild>
        <Link to={ROUTES.HOME}>Volver al inicio</Link>
      </Button>
    </div>
  );
}
```

---

### 3.2 ArtistaHeroBanner

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaHeroBanner.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| imagenUrl | `string \| undefined` | No | URL de imagen de fondo |

**Estado Local:**
- `imageLoaded: boolean` - Para fade in animation

**Dependencias:**
- Componentes UI: Ninguno (div nativo)

**Responsabilidad:**
- Renderizar hero banner con imagen de fondo o gradient fallback
- Si `imagenUrl` no existe, mostrar gradient `from-purple-900 to-pink-900`
- Si existe, intentar cargar imagen y mostrar gradient mientras carga

**Codigo:**
```tsx
export const ArtistaHeroBanner: FC<ArtistaHeroBannerProps> = ({ imagenUrl }) => {
  const [imageLoaded, setImageLoaded] = useState(false);

  if (!imagenUrl) {
    return (
      <div className="h-48 sm:h-56 md:h-64 bg-gradient-to-r from-purple-900 to-pink-900" />
    );
  }

  return (
    <div className="h-48 sm:h-56 md:h-64 relative overflow-hidden">
      <div className="absolute inset-0 bg-gradient-to-r from-purple-900 to-pink-900" />
      <img
        src={imagenUrl}
        alt="Hero banner"
        className={cn(
          "absolute inset-0 w-full h-full object-cover transition-opacity duration-300",
          imageLoaded ? "opacity-100" : "opacity-0"
        )}
        onLoad={() => setImageLoaded(true)}
        onError={() => setImageLoaded(false)}
      />
    </div>
  );
};
```

---

### 3.3 ArtistaAvatar

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaAvatar.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| imagenUrl | `string \| undefined` | No | URL de imagen de perfil |
| nombreArtistico | `string` | Si | Nombre para alt text |
| size | `'lg' \| 'xl' \| '2xl'` | No | Tamaño del avatar (default: '2xl') |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Avatar`, `AvatarImage`, `AvatarFallback`
- Icons: `heroicons:musical-note` (placeholder si no hay imagen)

**Responsabilidad:**
- Renderizar avatar con borde blanco y sombra
- Mostrar placeholder con icono de musica si no hay `imagenUrl`
- Aplicar clase especial `-mt-16` para overlap con hero banner

**Codigo:**
```tsx
const sizeClasses = {
  lg: 'w-24 h-24',
  xl: 'w-28 h-28',
  '2xl': 'w-32 h-32',
};

export const ArtistaAvatar: FC<ArtistaAvatarProps> = ({
  imagenUrl,
  nombreArtistico,
  size = '2xl'
}) => {
  return (
    <Avatar className={cn(
      sizeClasses[size],
      'border-4 border-[#1a1a2e] shadow-lg rounded-full'
    )}>
      <AvatarImage
        src={imagenUrl}
        alt={`Foto de perfil de ${nombreArtistico}`}
      />
      <AvatarFallback className="bg-[#2d1b4e]">
        <Icon icon="heroicons:musical-note" className="w-12 h-12 text-purple-300" />
      </AvatarFallback>
    </Avatar>
  );
};
```

---

### 3.4 ArtistaBio

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaBio.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| descripcion | `string \| undefined` | No | Biografia del artista |
| pais | `string \| undefined` | No | Pais del artista |
| ciudad | `string \| undefined` | No | Ciudad del artista |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Card`, `CardHeader`, `CardTitle`, `CardContent`
- Icons: `heroicons:map-pin` para ubicacion

**Responsabilidad:**
- Renderizar card con biografia del artista
- Mostrar placeholder si no hay descripcion
- Mostrar ubicacion (ciudad, pais) con icono si existe

**Codigo:**
```tsx
export const ArtistaBio: FC<ArtistaBioProps> = ({ descripcion, pais, ciudad }) => {
  const ubicacion = [ciudad, pais].filter(Boolean).join(', ');

  return (
    <Card className="bg-[#0f1729] border-[#334155]">
      <CardHeader>
        <CardTitle className="text-xl font-bold text-white">Biografia</CardTitle>
      </CardHeader>
      <CardContent>
        {descripcion ? (
          <p className="text-[#94a3b8] leading-relaxed whitespace-pre-wrap">
            {descripcion}
          </p>
        ) : (
          <p className="text-[#64748b] italic">
            Este artista aun no ha agregado una biografia
          </p>
        )}

        {ubicacion && (
          <div className="flex items-center gap-2 mt-6 text-[#94a3b8]">
            <Icon icon="heroicons:map-pin" className="w-5 h-5" />
            <span>{ubicacion}</span>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
```

---

### 3.5 ArtistaStats

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaStats.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artista | `Artista` | Si | Datos completos del artista |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: Ninguno (div nativo)
- Icons: `heroicons:musical-note`, `heroicons:users`, `heroicons:currency-euro`

**Responsabilidad:**
- Renderizar stats del artista (campanias, backers, recaudado)
- **NOTA MVP:** Por ahora hardcodear valores de ejemplo, ya que no tenemos relacion con campanias implementada

**Codigo:**
```tsx
export const ArtistaStats: FC<ArtistaStatsProps> = ({ artista }) => {
  // TODO MVP: Hardcoded stats - en futuro traer desde API
  const stats = {
    campanias: 0,
    backers: 0,
    recaudado: 0,
  };

  return (
    <div className="flex flex-wrap gap-6 text-[#94a3b8] mt-4">
      <div className="flex items-center gap-2">
        <Icon icon="heroicons:musical-note" className="w-5 h-5" />
        <span>{stats.campanias} campañas</span>
      </div>
      <div className="flex items-center gap-2">
        <Icon icon="heroicons:users" className="w-5 h-5" />
        <span>{stats.backers} backers</span>
      </div>
      <div className="flex items-center gap-2">
        <Icon icon="heroicons:currency-euro" className="w-5 h-5" />
        <span>€{stats.recaudado.toLocaleString()} recaudados</span>
      </div>
    </div>
  );
};
```

---

### 3.6 ArtistaSocialLinks

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaSocialLinks.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| - | - | - | Sin props (hardcoded por ahora) |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Button`
- Icons: `heroicons:globe-alt`, `heroicons:musical-note` (spotify), `simple-icons:youtube`

**Responsabilidad:**
- Renderizar botones de redes sociales
- **NOTA MVP:** Por ahora solo placeholders con iconos, no hay URLs en el modelo Artista

**Codigo:**
```tsx
export const ArtistaSocialLinks: FC = () => {
  // TODO MVP: Sin URLs en modelo Artista - agregar en futuro
  return (
    <div className="flex items-center gap-2">
      <Button
        variant="ghost"
        size="icon"
        className="text-[#94a3b8] hover:text-white"
        disabled
      >
        <Icon icon="heroicons:globe-alt" className="w-5 h-5" />
      </Button>
      <Button
        variant="ghost"
        size="icon"
        className="text-[#94a3b8] hover:text-white"
        disabled
      >
        <Icon icon="heroicons:musical-note" className="w-5 h-5" />
      </Button>
      <Button
        variant="ghost"
        size="icon"
        className="text-[#94a3b8] hover:text-white"
        disabled
      >
        <Icon icon="simple-icons:youtube" className="w-5 h-5" />
      </Button>
    </div>
  );
};
```

---

### 3.7 ArtistaCampaigns

**Archivo:** `src/web/src/features/artistas/presentation/components/ArtistaCampaigns.tsx`

**Props:**
| Prop | Tipo | Requerido | Descripcion |
|------|------|-----------|-------------|
| artistaId | `string` | Si | ID del artista |

**Estado Local:**
- Ninguno

**Dependencias:**
- Componentes UI: `Card`, `CardHeader`, `CardTitle`, `CardContent`

**Responsabilidad:**
- Renderizar seccion de campanias del artista
- **NOTA MVP:** Por ahora solo placeholder, ya que no tenemos relacion Artista -> Campanias implementada

**Codigo:**
```tsx
export const ArtistaCampaigns: FC<ArtistaCampaignsProps> = ({ artistaId }) => {
  // TODO MVP: Implementar query de campanias por artistaId
  return (
    <Card className="bg-[#0f1729] border-[#334155]">
      <CardHeader>
        <CardTitle className="text-xl font-bold text-white">Campañas</CardTitle>
      </CardHeader>
      <CardContent>
        <p className="text-[#64748b] text-center py-8">
          Proximamente campañas de este artista...
        </p>
      </CardContent>
    </Card>
  );
};
```

---

## 4. Hooks

### 4.1 useArtista

**Archivo:** `src/web/src/features/artistas/application/hooks/useArtista.ts`

**Tipo:** Query Hook

**Parametros:**
| Param | Tipo | Descripcion |
|-------|------|-------------|
| id | `string` | ID del artista a obtener |

**Retorna:**
| Campo | Tipo | Descripcion |
|-------|------|-------------|
| data | `Artista \| undefined` | Datos del artista |
| isLoading | `boolean` | Estado de carga |
| isError | `boolean` | Si hubo error |
| error | `Error \| null` | Error si hay |

**Query Key:** `QUERY_KEYS.ARTISTAS, id` (usa estructura actual del proyecto)

**Codigo:**
```tsx
import { useQuery } from '@tanstack/react-query';
import { QUERY_KEYS } from '@/lib/constants';
import { artistaService } from '../../infrastructure/artista.service';

export function useArtista(id: string) {
  return useQuery({
    queryKey: [QUERY_KEYS.ARTISTAS, id],
    queryFn: () => artistaService.getById(id),
    enabled: !!id,
  });
}
```

**Nota:** El hook usa el patron existente del proyecto (singular en QUERY_KEYS.ARTISTA). Se agrega al archivo `useArtista.ts` existente.

---

## 5. Services

### 5.1 artistaService.getById()

**Archivo:** `src/web/src/features/artistas/infrastructure/artista.service.ts` (ACTUALIZAR)

**Metodo a Agregar:**
| Metodo | Input | Output | Endpoint |
|--------|-------|--------|----------|
| getById | `id: string` | `Promise<Artista>` | `GET /api/artistas/{id}` |

**Codigo:**
```tsx
// Agregar al ArtistaService existente
async getById(id: string): Promise<Artista> {
  const response = await apiFetch<ServiceResponse<ArtistaDto>>(
    `${this.baseUrl}/${id}`
  );

  if (!response.data) {
    throw new Error('Artista no encontrado');
  }

  return mapDtoToDomain(response.data);
}
```

**Manejo de Errores:**
- Si endpoint retorna 404, `apiFetch` lanzara error que sera capturado por React Query
- El componente `ArtistaProfilePage` detecta error 404 y muestra pagina de error

---

## 6. Flujo de Datos

```
Usuario accede a /artistas/:id
    ↓
ArtistaProfilePage (presentation)
    ↓
useArtista(id) hook (application)
    ↓
artistaService.getById(id) (infrastructure)
    ↓
apiFetch → GET /api/artistas/{id}
    ↓
Backend retorna ServiceResponse<ArtistaDto>
    ↓
mapDtoToDomain() convierte DTO a domain entity
    ↓
React Query cachea resultado
    ↓
Componente renderiza perfil publico
```

**Cache Strategy:**
- React Query cachea por 5 minutos (default)
- Stale time: 1 minuto
- Cache key: `['artistas', id]`

---

## 7. Dependencias de Shared

**Importar de `@shared/` (cuando este implementado):**
- Types: `Artista`, `ArtistaListItem`
- Constants: `QUERY_KEYS.artistas.byId(id)`, `API_ROUTES.artistas.byId(id)`

**Por ahora usar estructura actual:**
- Types: Definidos en `features/artistas/domain/types.ts`
- Constants: `QUERY_KEYS.ARTISTAS` en `lib/constants.ts`
- Rutas: Hardcoded en service `/artistas/${id}`

**Migracion futura:**
```tsx
// ANTES (actual)
import { QUERY_KEYS } from '@/lib/constants';
queryKey: [QUERY_KEYS.ARTISTAS, id]

// DESPUES (con shared)
import { QUERY_KEYS } from '@shared/constants/query-keys';
queryKey: QUERY_KEYS.artistas.byId(id)
```

---

## 8. Routing

### 8.1 Agregar Ruta en Router

**Archivo:** `src/web/src/app/router.tsx` (ACTUALIZAR)

**Cambios:**
```tsx
// 1. Agregar import lazy load
const ArtistaPublicProfilePage = lazy(() =>
  import("@/features/artistas/presentation/pages/ArtistaPublicProfilePage")
);

// 2. Agregar ruta publica (dentro de PublicLayout)
<Route element={<PublicLayout />}>
  {/* ... rutas existentes */}
  <Route path={ROUTES.ARTISTA_PROFILE} element={<ArtistaPublicProfilePage />} />
</Route>
```

### 8.2 Actualizar Constantes

**Archivo:** `src/web/src/lib/constants.ts` (ACTUALIZAR)

**Cambios:**
```tsx
export const ROUTES = {
  // ... rutas existentes
  ARTISTA_PROFILE: "/artistas/:id",  // AGREGAR
} as const;
```

**Nota:** Mantener `ARTISTA_PERFIL: "/artista/perfil"` existente (es para edicion de perfil propio en dashboard).

---

## 9. Archivos a Crear/Actualizar

### Crear

| Archivo | Tipo | Descripcion |
|---------|------|-------------|
| `presentation/components/ArtistaHeroBanner.tsx` | Component | Hero banner con imagen o gradient |
| `presentation/components/ArtistaAvatar.tsx` | Component | Avatar con borde y fallback |
| `presentation/components/ArtistaBio.tsx` | Component | Card con biografia y ubicacion |
| `presentation/components/ArtistaStats.tsx` | Component | Stats de campanias/backers/recaudado |
| `presentation/components/ArtistaSocialLinks.tsx` | Component | Botones de redes sociales (placeholder) |
| `presentation/components/ArtistaCampaigns.tsx` | Component | Seccion de campanias (placeholder) |
| `presentation/components/index.ts` | Barrel | Exports de todos los componentes |
| `presentation/pages/ArtistaPublicProfilePage.tsx` | Page | Pagina publica de perfil de artista |

### Actualizar

| Archivo | Cambio | Descripcion |
|---------|--------|-------------|
| `application/hooks/useArtista.ts` | Agregar hook | `useArtista(id)` query hook |
| `infrastructure/artista.service.ts` | Agregar metodo | `getById(id)` en ArtistaService |
| `domain/types.ts` | Actualizar types | Asegurar que Artista tenga `pais` y `ciudad` |
| `app/router.tsx` | Agregar ruta | `/artistas/:id` en PublicLayout |
| `lib/constants.ts` | Agregar constante | `ROUTES.ARTISTA_PROFILE` |

---

## 10. Checklist

### Estructura
- [ ] Componentes siguen arquitectura hexagonal (domain/application/infrastructure/presentation)
- [ ] Hooks en `application/hooks/`
- [ ] Service en `infrastructure/`
- [ ] Componentes UI en `presentation/components/`
- [ ] Page component en `presentation/pages/`

### Componentes
- [ ] Todos los componentes usan shadcn/ui
- [ ] Props tipadas con interfaces
- [ ] Uso correcto de `FC<Props>`
- [ ] Imports desde barrels (`index.ts`)

### Hooks
- [ ] `useArtista` sigue patron useQuery
- [ ] Query key usa estructura de constantes
- [ ] Manejo de loading/error states

### Service
- [ ] Metodo `getById` usa `apiFetch` helper existente
- [ ] Retorna `Promise<Artista>`
- [ ] Maneja errores correctamente
- [ ] Usa mapper `mapDtoToDomain`

### Routing
- [ ] Ruta `/artistas/:id` agregada a router
- [ ] Ruta en `PublicLayout` (sin autenticacion)
- [ ] Constante `ROUTES.ARTISTA_PROFILE` agregada

### UI/UX
- [ ] Dark theme aplicado (`bg-[#1a1a2e]`, etc.)
- [ ] Responsive design (mobile-first)
- [ ] Loading skeletons implementados
- [ ] Error 404 page implementada
- [ ] Hero banner con gradient fallback
- [ ] Avatar con placeholder de icono
- [ ] Stats y campaigns como placeholders (MVP)

### Integracion
- [ ] Types compatibles con backend DTOs
- [ ] Endpoint `/api/artistas/{id}` correcto
- [ ] Manejo de `ServiceResponse<T>` del backend
- [ ] Error handling con mensajes claros

---

## 11. Notas de Implementacion

### 11.1 Perfil Publico vs Perfil Propio

**Perfil Publico (esta feature):**
- Ruta: `/artistas/:id`
- Sin autenticacion
- Solo lectura
- Componente: `ArtistaPublicProfilePage`

**Perfil Propio (ya existe):**
- Ruta: `/artista/perfil`
- Con autenticacion
- Edicion de perfil
- Componente: `ArtistaPerfilPage` (ya implementado)

### 11.2 Stats y Campanias son Placeholders

Por ahora, `ArtistaStats` y `ArtistaCampaigns` muestran datos hardcodeados o placeholders, ya que:
1. No hay relacion Artista -> Campanias en el modelo actual
2. El MVP se enfoca en mostrar perfil basico del artista
3. En futuras features se agregara la query de campanias por artista

### 11.3 Social Links sin Datos

El componente `ArtistaSocialLinks` muestra botones disabled porque el modelo `Artista` no tiene campos para redes sociales. En futuro se pueden agregar:
- `spotifyUrl?: string`
- `youtubeUrl?: string`
- `websiteUrl?: string`

### 11.4 Tipos de Shared

Actualmente el proyecto no tiene `src/shared` implementado. Cuando se implemente:
1. Mover types de `domain/types.ts` a `@shared/types/artista.ts`
2. Re-exportar desde domain: `export type { Artista } from '@shared/types/artista';`
3. Actualizar imports en toda la feature

### 11.5 Avatar Overlap

El avatar tiene `-mt-16` para overlap con el hero banner. Esto solo funciona bien en desktop. En mobile se reduce a `-mt-12` via responsive classes.

### 11.6 Genre Tags Hardcoded

El UI/UX spec muestra "genre tags" (Electronic, Synthwave, Indie), pero el modelo Artista no tiene campo `generos: string[]`. Por ahora se omiten o se hardcodea un placeholder.

---

## 12. Testing Strategy (Futuro)

### Componentes a Testear

```tsx
// ArtistaProfilePage.test.tsx
- Renders loading skeleton when data is loading
- Renders 404 page when artista not found
- Renders full profile when data is loaded
- Shows placeholder when no bio

// ArtistaHeroBanner.test.tsx
- Renders gradient when no imagenUrl
- Renders image when imagenUrl exists
- Falls back to gradient on image load error

// ArtistaBio.test.tsx
- Renders bio text when provided
- Shows placeholder when no bio
- Shows ubicacion when pais/ciudad exist
```

### Hooks a Testear

```tsx
// useArtista.test.ts
- Calls artistaService.getById with correct id
- Returns loading state initially
- Returns artista data on success
- Returns error state on failure
```

---

## 13. Proximos Pasos

1. **Implementar shared types** (si no estan ya)
   - Crear `src/shared/types/artista.ts` con types del plan shared

2. **Actualizar service** con metodo `getById()`

3. **Crear componentes** en orden:
   - Componentes atomicos primero (Avatar, HeroBanner)
   - Componentes compuestos despues (Bio, Stats)
   - Page component al final

4. **Configurar routing** agregando ruta `/artistas/:id`

5. **Testing manual**:
   - Acceder a `/artistas/123` (debe cargar perfil)
   - Acceder a `/artistas/invalid-id` (debe mostrar 404)
   - Verificar responsive en mobile

6. **Integracion con backend** (cuando este implementado):
   - Verificar que endpoint retorna estructura correcta
   - Validar que error 404 se maneja bien
   - Testear con artistas reales

---

**Siguiente paso sugerido:** Implementar backend (endpoints de artista) antes de implementar este frontend, para poder testear con datos reales.
