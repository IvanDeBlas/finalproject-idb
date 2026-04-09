# Contratos: Dashboard de Artista

> **Feature:** dashboard-artista (US-05)
> **Ultima actualizacion:** 2026-02-14

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## 📡 Endpoints API

### GET /api/dashboard/resumen

**Descripcion:** Obtiene metricas resumidas del artista autenticado. Incluye total recaudado, total de backers, y conteo de campanias activas y completadas.

**Autorizacion:** ✅ Bearer JWT (Artista role required)

**Response 200 OK:**
```json
{
  "data": {
    "artistaId": "guid",
    "nombreArtistico": "Juan Perez Music",
    "totalRecaudado": 15340.50,
    "totalBackers": 487,
    "campaniasActivas": 2,
    "campaniasCompletadas": 3,
    "totalCampanias": 5,
    "monedaSimbolo": "EUR",
    "fechaUltimoAporte": "2026-02-14T10:30:00Z"
  },
  "messages": [
    {
      "message": "Resumen obtenido",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Acceso denegado | Usuario no tiene perfil de Artista |
| 404 | 2002 | Artista no encontrado | UserId del token sin perfil Artista |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### GET /api/campanias/mis-campanias

**Descripcion:** Lista las campanias del artista autenticado con paginacion. Incluye metricas calculadas (porcentaje de progreso, numero de backers, dias restantes).

**Autorizacion:** ✅ Bearer JWT (Artista role)

**Query Parameters:**
- `estadoCampaniaId` (int, opcional): Filtra por estado (1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada)
- `page` (int, default 1): Numero de pagina
- `pageSize` (int, default 10): Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "titulo": "Mi Album Debut",
        "imagenPrincipalUrl": "https://...",
        "estadoCampaniaId": 2,
        "estadoCampaniaNombre": "Publicada",
        "importeObjetivo": 5000.00,
        "importeRecaudado": 2340.50,
        "porcentajeProgreso": 46.81,
        "numBackers": 78,
        "diasRestantes": 46,
        "fechaFin": "2026-03-31T23:59:59Z",
        "fechaCreacion": "2026-01-15T12:00:00Z"
      },
      {
        "id": "guid",
        "titulo": "Gira Nacional 2026",
        "imagenPrincipalUrl": "https://...",
        "estadoCampaniaId": 1,
        "estadoCampaniaNombre": "Borrador",
        "importeObjetivo": 10000.00,
        "importeRecaudado": 0.00,
        "porcentajeProgreso": 0.00,
        "numBackers": 0,
        "diasRestantes": null,
        "fechaFin": null,
        "fechaCreacion": "2026-02-10T09:00:00Z"
      }
    ],
    "totalCount": 5,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "messages": [
    {
      "message": "Campanias encontradas",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Acceso denegado | Usuario no tiene perfil de Artista |
| 404 | 2002 | Artista no encontrado | UserId del token sin perfil Artista |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### GET /api/campanias/{id}/backings

**Descripcion:** Lista los aportes (backings) de una campania especifica con paginacion. Solo accesible por el artista propietario de la campania. Incluye nombre del backer (o "Anonimo"), recompensa seleccionada, monto, mensaje, y fecha. Tambien retorna estadisticas agregadas (total recaudado, backing promedio, reward mas popular, ultimo backing).

**Autorizacion:** ✅ Bearer JWT (Artista, debe ser propietario de la campania)

**Query Parameters:**
- `page` (int, default 1): Numero de pagina
- `pageSize` (int, default 20): Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "campaniaId": "guid",
    "campaniaTitulo": "Mi Album Debut",
    "stats": {
      "totalRecaudado": 2340.50,
      "backingPromedio": 30.01,
      "totalBackers": 78,
      "rewardMasPopular": "CD Fisico Firmado",
      "ultimoBacking": {
        "nombreBacker": "Maria Lopez",
        "monto": 25.00,
        "fechaCreacion": "2026-02-14T10:30:00Z"
      }
    },
    "backings": {
      "items": [
        {
          "id": "guid",
          "nombreBacker": "Maria Lopez",
          "email": "maria.lopez@example.com",
          "monto": 25.00,
          "rewardNombre": "CD Fisico Firmado",
          "mensaje": "Mucha suerte con el proyecto!",
          "esAnonimo": false,
          "estadoPedido": "Completado",
          "fechaCreacion": "2026-02-14T10:30:00Z"
        },
        {
          "id": "guid",
          "nombreBacker": "Anonimo",
          "email": null,
          "monto": 50.00,
          "rewardNombre": null,
          "mensaje": null,
          "esAnonimo": true,
          "estadoPedido": "Completado",
          "fechaCreacion": "2026-02-13T09:15:00Z"
        }
      ],
      "totalCount": 78,
      "page": 1,
      "pageSize": 20,
      "totalPages": 4
    }
  },
  "messages": [
    {
      "message": "Aportes encontrados",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para ver estos aportes | Usuario no es propietario de la campania |
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

### GET /api/campanias/{id}/stats

**Descripcion:** Obtiene estadisticas detalladas de una campania. Solo accesible por el artista propietario. Incluye metricas de progreso, distribucion de recompensas, y progreso diario.

**Autorizacion:** ✅ Bearer JWT (Artista, debe ser propietario de la campania)

**Response 200 OK:**
```json
{
  "data": {
    "campaniaId": "guid",
    "campaniaTitulo": "Mi Album Debut",
    "importeObjetivo": 5000.00,
    "importeRecaudado": 2340.50,
    "porcentajeProgreso": 46.81,
    "numBackers": 78,
    "backingPromedio": 30.01,
    "diasRestantes": 46,
    "diasTranscurridos": 14,
    "totalDiasCampania": 60,
    "proyeccionFinal": 10029.64,
    "velocidadDiaria": 167.18,
    "rewardStats": [
      {
        "rewardId": "guid",
        "rewardNombre": "Descarga Digital",
        "cantidadVendida": 45,
        "totalRecaudado": 450.00,
        "porcentajeDelTotal": 19.23
      },
      {
        "rewardId": "guid",
        "rewardNombre": "CD Fisico Firmado",
        "cantidadVendida": 33,
        "totalRecaudado": 825.00,
        "porcentajeDelTotal": 35.25
      },
      {
        "rewardId": null,
        "rewardNombre": "Sin recompensa",
        "cantidadVendida": 0,
        "totalRecaudado": 1065.50,
        "porcentajeDelTotal": 45.52
      }
    ],
    "progressoPorDia": [
      {
        "fecha": "2026-02-01",
        "numBackings": 12,
        "totalRecaudado": 340.00,
        "acumulado": 340.00
      },
      {
        "fecha": "2026-02-02",
        "numBackings": 8,
        "totalRecaudado": 210.00,
        "acumulado": 550.00
      }
    ]
  },
  "messages": [
    {
      "message": "Estadisticas obtenidas",
      "errorCode": "0000"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para ver estas estadisticas | Usuario no es propietario de la campania |
| 404 | 2003 | Campania no encontrada | campaniaId no existe en DB |
| 500 | 5000 | Error inesperado | Excepcion no controlada |

---

## 🔐 Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `GET /api/dashboard/resumen` | ✅ | Artista | UserId del token debe tener perfil Artista |
| `GET /api/campanias/mis-campanias` | ✅ | Artista | Filtra por ArtistaId del usuario autenticado |
| `GET /api/campanias/{id}/backings` | ✅ | Artista | Validar que campania.ArtistaId == artista.Id del usuario |
| `GET /api/campanias/{id}/stats` | ✅ | Artista | Validar que campania.ArtistaId == artista.Id del usuario |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string, Identity User ID)",
  "email": "artista@example.com",
  "name": "Juan Perez",
  "role": "Artista",
  "exp": 1739000000,
  "iat": 1738913600
}
```

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas por defecto
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

**Nota sobre autorizacion:**
- Todos los endpoints requieren claim `role=Artista`
- El backend debe validar que el usuario tiene un perfil de Artista asociado
- Para endpoints de campania especifica, validar ownership: `campania.ArtistaId == artista.Id`

### Proteccion de Rutas Frontend

| Ruta (Admin) | Auth | Rol | Redirect | Notas |
|--------------|------|-----|----------|-------|
| `/dashboard` | ✅ | Artista | `/auth/login` | Resumen general del artista |
| `/dashboard/campanias` | ✅ | Artista | `/auth/login` | Lista de campanias del artista |
| `/dashboard/campanias/{id}` | ✅ | Artista | `/auth/login` | Detalle de campania con stats |
| `/dashboard/campanias/{id}/backings` | ✅ | Artista | `/auth/login` | Lista de backings de campania |

**Flujo de autenticacion:**
1. Usuario accede a ruta del dashboard
2. Frontend verifica token JWT en localStorage/cookie
3. Si token invalido/expirado → Redirect a `/auth/login?returnUrl={currentPath}`
4. Si token valido pero role != "Artista" → Mostrar error 403 o redirect a `/artista/perfil/crear`
5. Si token valido y role = "Artista" → Permitir acceso

---

## 📦 DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/DashboardResumenDto.cs
public class DashboardResumenDto
{
    public Guid ArtistaId { get; set; }
    public string NombreArtistico { get; set; } = null!;
    public decimal TotalRecaudado { get; set; }
    public int TotalBackers { get; set; }
    public int CampaniasActivas { get; set; } // EstadoCampaniaId = 2 (Publicada)
    public int CampaniasCompletadas { get; set; } // EstadoCampaniaId = 3 (Finalizada)
    public int TotalCampanias { get; set; }
    public string MonedaSimbolo { get; set; } = "EUR";
    public DateTime? FechaUltimoAporte { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/MiCampaniaListItemDto.cs
public class MiCampaniaListItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? ImagenPrincipalUrl { get; set; }
    public int EstadoCampaniaId { get; set; }
    public string EstadoCampaniaNombre { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; } // (ImporteRecaudado / ImporteObjetivo) * 100
    public int NumBackers { get; set; } // COUNT de PedidoCrowdfunding con EstadoPedidoId = 3 (Completado)
    public int? DiasRestantes { get; set; } // null si campania no tiene FechaFin o es borrador
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingListDto.cs
public class CampaniaBackingListDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public CampaniaBackingStatsDto Stats { get; set; } = null!;
    public PaginatedResponse<CampaniaBackingItemDto> Backings { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingStatsDto.cs
public class CampaniaBackingStatsDto
{
    public decimal TotalRecaudado { get; set; }
    public decimal BackingPromedio { get; set; }
    public int TotalBackers { get; set; }
    public string? RewardMasPopular { get; set; } // Nombre del reward con mas ventas
    public UltimoBackingDto? UltimoBacking { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/UltimoBackingDto.cs
public class UltimoBackingDto
{
    public string NombreBacker { get; set; } = null!; // "Anonimo" o nombre real
    public decimal Monto { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingItemDto.cs
public class CampaniaBackingItemDto
{
    public Guid Id { get; set; }
    public string NombreBacker { get; set; } = null!; // "Anonimo" o nombre del usuario
    public string? Email { get; set; } // null si anonimo o backer no permitio mostrar
    public decimal Monto { get; set; }
    public string? RewardNombre { get; set; } // null si backing sin recompensa
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }
    public string EstadoPedido { get; set; } = null!; // "Completado", "Pendiente", etc.
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaStatsDetailDto.cs
public class CampaniaStatsDetailDto
{
    public Guid CampaniaId { get; set; }
    public string CampaniaTitulo { get; set; } = null!;
    public decimal ImporteObjetivo { get; set; }
    public decimal ImporteRecaudado { get; set; }
    public decimal PorcentajeProgreso { get; set; }
    public int NumBackers { get; set; }
    public decimal BackingPromedio { get; set; }
    public int? DiasRestantes { get; set; }
    public int DiasTranscurridos { get; set; } // Desde FechaInicio hasta hoy
    public int TotalDiasCampania { get; set; } // FechaFin - FechaInicio
    public decimal? ProyeccionFinal { get; set; } // Estimacion final basada en velocidad diaria
    public decimal VelocidadDiaria { get; set; } // Promedio de recaudacion diaria
    public List<RewardStatDto> RewardStats { get; set; } = new();
    public List<ProgressoDiaDto> ProgressoPorDia { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardStatDto.cs
public class RewardStatDto
{
    public Guid? RewardId { get; set; } // null = "Sin recompensa"
    public string RewardNombre { get; set; } = null!;
    public int CantidadVendida { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal PorcentajeDelTotal { get; set; } // (TotalRecaudado / ImporteRecaudadoCampania) * 100
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/ProgressoDiaDto.cs
public class ProgressoDiaDto
{
    public string Fecha { get; set; } = null!; // "YYYY-MM-DD" format
    public int NumBackings { get; set; }
    public decimal TotalRecaudado { get; set; }
    public decimal Acumulado { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/dashboard.ts
export interface DashboardResumen {
  artistaId: string;
  nombreArtistico: string;
  totalRecaudado: number;
  totalBackers: number;
  campaniasActivas: number;
  campaniasCompletadas: number;
  totalCampanias: number;
  monedaSimbolo: string;
  fechaUltimoAporte?: string; // ISO 8601
}

export interface MiCampaniaListItem {
  id: string;
  titulo: string;
  imagenPrincipalUrl?: string;
  estadoCampaniaId: number;
  estadoCampaniaNombre: string;
  importeObjetivo: number;
  importeRecaudado: number;
  porcentajeProgreso: number;
  numBackers: number;
  diasRestantes?: number; // null para borradores o campanias sin fecha fin
  fechaFin?: string; // ISO 8601
  fechaCreacion: string; // ISO 8601
}

export interface CampaniaBackingList {
  campaniaId: string;
  campaniaTitulo: string;
  stats: CampaniaBackingStats;
  backings: PaginatedResponse<CampaniaBackingItem>;
}

export interface CampaniaBackingStats {
  totalRecaudado: number;
  backingPromedio: number;
  totalBackers: number;
  rewardMasPopular?: string;
  ultimoBacking?: UltimoBacking;
}

export interface UltimoBacking {
  nombreBacker: string;
  monto: number;
  fechaCreacion: string; // ISO 8601
}

export interface CampaniaBackingItem {
  id: string;
  nombreBacker: string; // "Anonimo" o nombre real
  email?: string; // undefined si anonimo
  monto: number;
  rewardNombre?: string;
  mensaje?: string;
  esAnonimo: boolean;
  estadoPedido: string;
  fechaCreacion: string; // ISO 8601
}

export interface CampaniaStatsDetail {
  campaniaId: string;
  campaniaTitulo: string;
  importeObjetivo: number;
  importeRecaudado: number;
  porcentajeProgreso: number;
  numBackers: number;
  backingPromedio: number;
  diasRestantes?: number;
  diasTranscurridos: number;
  totalDiasCampania: number;
  proyeccionFinal?: number;
  velocidadDiaria: number;
  rewardStats: RewardStat[];
  progressoPorDia: ProgressoDia[];
}

export interface RewardStat {
  rewardId?: string; // undefined = "Sin recompensa"
  rewardNombre: string;
  cantidadVendida: number;
  totalRecaudado: number;
  porcentajeDelTotal: number;
}

export interface ProgressoDia {
  fecha: string; // "YYYY-MM-DD"
  numBackings: number;
  totalRecaudado: number;
  acumulado: number;
}
```

---

## ✅ Validaciones Compartidas

### Query Parameters

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| estadoCampaniaId | opcional, 1-5 | `.InclusiveBetween(1, 5).When(x => x.EstadoCampaniaId.HasValue).WithErrorCode("1007")` | `.number().int().min(1).max(5).optional()` |
| page | opcional, >= 1 | `.GreaterThanOrEqualTo(1).When(x => x.Page.HasValue).WithErrorCode("1007")` | `.number().int().min(1).default(1)` |
| pageSize | opcional, 1-100 | `.InclusiveBetween(1, 100).When(x => x.PageSize.HasValue).WithErrorCode("1007")` | `.number().int().min(1).max(100).default(10)` |

### Validaciones de Negocio

| Regla | Backend | Frontend |
|-------|---------|----------|
| Usuario debe tener perfil Artista | Validar que `userId` tiene `Artista` asociado (error 2002) | Verificar role="Artista" en token, redirect si no |
| Campania debe pertenecer al artista | Validar que `campania.ArtistaId == artista.Id` (error 3002) | No aplicable (backend valida) |
| Calculos de porcentaje | `(ImporteRecaudado / ImporteObjetivo) * 100` (round 2 decimals) | Usar misma formula client-side |
| Dias restantes | `Max(0, (FechaFin - DateTime.UtcNow).Days)` | Calcular dinamicamente en cliente |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/dashboard.schema.ts
import { z } from 'zod';

export const misCampaniasQuerySchema = z.object({
  estadoCampaniaId: z
    .number()
    .int()
    .min(1, 'Estado invalido')
    .max(5, 'Estado invalido')
    .optional(),
  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor o igual a 1')
    .default(1),
  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
    .max(100, 'El tamano de pagina no puede superar 100')
    .default(10),
});

export type MisCampaniasQueryParams = z.infer<typeof misCampaniasQuerySchema>;

export const backingsQuerySchema = z.object({
  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor o igual a 1')
    .default(1),
  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
    .max(100, 'El tamano de pagina no puede superar 100')
    .default(20),
});

export type BackingsQueryParams = z.infer<typeof backingsQuerySchema>;
```

---

## 🔗 Constantes Compartidas

```typescript
// Ruta: src/shared/constants/query-keys.ts
export const QUERY_KEYS = {
  dashboard: {
    resumen: ['dashboard', 'resumen'] as const,
    misCampanias: (params: MisCampaniasQueryParams) =>
      ['dashboard', 'mis-campanias', params] as const,
    campaniaBackings: (campaniaId: string, params: BackingsQueryParams) =>
      ['dashboard', 'campanias', campaniaId, 'backings', params] as const,
    campaniaStats: (campaniaId: string) =>
      ['dashboard', 'campanias', campaniaId, 'stats'] as const,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/api-routes.ts
export const API_ROUTES = {
  dashboard: {
    resumen: '/api/dashboard/resumen',
    misCampanias: '/api/campanias/mis-campanias',
    campaniaBackings: (campaniaId: string) => `/api/campanias/${campaniaId}/backings`,
    campaniaStats: (campaniaId: string) => `/api/campanias/${campaniaId}/stats`,
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/app-routes.ts
export const APP_ROUTES = {
  dashboard: {
    home: '/dashboard',
    campanias: '/dashboard/campanias',
    campaniaDetail: (id: string) => `/dashboard/campanias/${id}`,
    campaniaBackings: (id: string) => `/dashboard/campanias/${id}/backings`,
  },
  auth: {
    login: '/auth/login',
  },
} as const;
```

```typescript
// Ruta: src/shared/constants/campania-states.ts
export const ESTADO_CAMPANIA = {
  BORRADOR: 1,
  PUBLICADA: 2,
  FINALIZADA: 3,
  CANCELADA: 4,
  PAUSADA: 5,
} as const;

export const ESTADO_CAMPANIA_LABELS: Record<number, string> = {
  1: 'Borrador',
  2: 'Publicada',
  3: 'Finalizada',
  4: 'Cancelada',
  5: 'Pausada',
};

export const ESTADO_CAMPANIA_COLORS: Record<number, string> = {
  1: 'gray', // Borrador
  2: 'green', // Publicada
  3: 'blue', // Finalizada
  4: 'red', // Cancelada
  5: 'yellow', // Pausada
};
```

```typescript
// Ruta: src/shared/constants/pedido-states.ts
export const ESTADO_PEDIDO = {
  PENDIENTE: 1,
  PROCESANDO: 2,
  COMPLETADO: 3,
  FALLIDO: 4,
  CANCELADO: 5,
} as const;

export const ESTADO_PEDIDO_LABELS: Record<number, string> = {
  1: 'Pendiente',
  2: 'Procesando',
  3: 'Completado',
  4: 'Fallido',
  5: 'Cancelado',
};
```

---

## 📊 Mapeo de Errores a UI

```typescript
// Ruta: src/shared/utils/error-messages.ts
export const DASHBOARD_ERROR_MESSAGES: Record<string, string> = {
  // Success codes
  '0000': 'Operacion exitosa',

  // Validation errors (1000-1999)
  '1007': 'Valor fuera de rango',

  // Not Found errors (2000-2999)
  '2002': 'No tienes un perfil de artista. Por favor, crea tu perfil primero',
  '2003': 'Campania no encontrada',

  // Auth errors (3000-3999)
  '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
  '3002': 'No tienes permiso para acceder a este recurso',

  // Internal errors (5000-5999)
  '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
} as const;

export const getDashboardErrorMessage = (errorCode: string): string => {
  return DASHBOARD_ERROR_MESSAGES[errorCode] || DASHBOARD_ERROR_MESSAGES['5000'];
};
```

---

## 🔔 Eventos SignalR

**No aplica para MVP.** El dashboard es un area privada del artista con datos que no requieren actualizacion en tiempo real.

**Futuro (post-MVP):** Considerar eventos SignalR para:
1. **NewBackingReceived** - Notificar al artista cuando recibe un nuevo aporte mientras esta en el dashboard
2. **CampaignMilestoneReached** - Notificar cuando la campania alcanza un hito (25%, 50%, 75%, 100% del objetivo)
3. **RewardSoldOut** - Alertar cuando una recompensa se agota

Ejemplo de evento futuro:
```typescript
// SignalR Hub: DashboardHub
// Evento: "NewBackingReceived"
// Payload: { campaniaId, backingId, monto, rewardNombre, nombreBacker }
// Suscriptores: Artista propietario (solo el)
```

---

## 🔧 CQRS Commands/Queries

### Queries

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Dashboard/Queries/GetDashboardResumenQuery.cs
public class GetDashboardResumenQuery : IRequest<ServiceResponse<DashboardResumenDto>>
{
    public string UserId { get; set; } = null!; // Inyectado desde HttpContext.User
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs
public class GetMisCampaniasQuery : IRequest<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>
{
    public string UserId { get; set; } = null!; // Inyectado desde HttpContext.User
    public int? EstadoCampaniaId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaBackingsQuery.cs
public class GetCampaniaBackingsQuery : IRequest<ServiceResponse<CampaniaBackingListDto>>
{
    public Guid CampaniaId { get; set; }
    public string UserId { get; set; } = null!; // Para validar ownership
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

```csharp
// Ruta: Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsQuery.cs
public class GetCampaniaStatsQuery : IRequest<ServiceResponse<CampaniaStatsDetailDto>>
{
    public Guid CampaniaId { get; set; }
    public string UserId { get; set; } = null!; // Para validar ownership
}
```

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (4 endpoints GET)
- [x] Autorizacion por endpoint (todos requieren Artista role)
- [x] Claims JWT documentados (sub, email, name, role, exp, iat)
- [x] DTOs C# completos (DashboardResumenDto, MiCampaniaListItemDto, CampaniaBackingListDto, CampaniaBackingStatsDto, UltimoBackingDto, CampaniaBackingItemDto, CampaniaStatsDetailDto, RewardStatDto, ProgressoDiaDto)
- [x] Types TypeScript equivalentes (todos los DTOs con interfaces matching)
- [x] Schemas Zod con mismas reglas (misCampaniasQuerySchema, backingsQuerySchema)
- [x] Constantes compartidas (query keys, API routes, app routes, estados de campania/pedido)
- [x] Mapeo de errores a mensajes UI (DASHBOARD_ERROR_MESSAGES)
- [x] CQRS Queries definidos (GetDashboardResumenQuery, GetMisCampaniasQuery, GetCampaniaBackingsQuery, GetCampaniaStatsQuery)
- [x] SignalR evaluado (no aplica para MVP, considerado para post-MVP)

---

## Notas Tecnicas Adicionales

### Nuevas Constantes de Error

**IMPORTANTE**: Las siguientes constantes ya existen en `ServiceResponseMessageType.cs` y son suficientes para dashboard:

```csharp
// Existentes en Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs
public const string NotFound_Artista = "2002";
public const string Auth_Forbidden = "3002";
// No se requieren nuevas constantes especificas de dashboard
```

### Calculo de Metricas en Backend

#### DashboardResumenDto
```csharp
var resumen = new DashboardResumenDto
{
    ArtistaId = artista.Id,
    NombreArtistico = artista.NombreArtistico,
    TotalRecaudado = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaCrowdfunding.ArtistaId == artista.Id &&
                    p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .SumAsync(p => p.ImporteTotal),
    TotalBackers = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaCrowdfunding.ArtistaId == artista.Id &&
                    p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .Select(p => p.UserId ?? p.Id.ToString()) // Usar ID si anonimo
        .Distinct()
        .CountAsync(),
    CampaniasActivas = await _context.CampaniaCrowdfunding
        .Where(c => c.ArtistaId == artista.Id && c.EstadoCampaniaId == ESTADO_CAMPANIA.PUBLICADA)
        .CountAsync(),
    CampaniasCompletadas = await _context.CampaniaCrowdfunding
        .Where(c => c.ArtistaId == artista.Id && c.EstadoCampaniaId == ESTADO_CAMPANIA.FINALIZADA)
        .CountAsync(),
    TotalCampanias = await _context.CampaniaCrowdfunding
        .Where(c => c.ArtistaId == artista.Id)
        .CountAsync(),
    FechaUltimoAporte = await _context.PedidoCrowdfunding
        .Where(p => p.CampaniaCrowdfunding.ArtistaId == artista.Id &&
                    p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
        .OrderByDescending(p => p.FechaCreacion)
        .Select(p => p.FechaCreacion)
        .FirstOrDefaultAsync()
};
```

#### MiCampaniaListItemDto
```csharp
var campanias = await _context.CampaniaCrowdfunding
    .Where(c => c.ArtistaId == artistaId)
    .Where(c => !estadoCampaniaId.HasValue || c.EstadoCampaniaId == estadoCampaniaId.Value)
    .Select(c => new MiCampaniaListItemDto
    {
        Id = c.Id,
        Titulo = c.Titulo,
        ImagenPrincipalUrl = c.ImagenPrincipalUrl,
        EstadoCampaniaId = c.EstadoCampaniaId,
        EstadoCampaniaNombre = ESTADO_CAMPANIA_LABELS[c.EstadoCampaniaId],
        ImporteObjetivo = c.ImporteObjetivo,
        ImporteRecaudado = c.ImportePledgedActual,
        PorcentajeProgreso = c.ImporteObjetivo > 0
            ? Math.Round((c.ImportePledgedActual / c.ImporteObjetivo) * 100, 2)
            : 0,
        NumBackers = _context.PedidoCrowdfunding
            .Where(p => p.CampaniaId == c.Id && p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
            .Count(),
        DiasRestantes = c.FechaFin.HasValue
            ? Math.Max(0, (c.FechaFin.Value - DateTime.UtcNow).Days)
            : (int?)null,
        FechaFin = c.FechaFin,
        FechaCreacion = c.FechaCreacion
    })
    .OrderByDescending(c => c.FechaCreacion)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### Proyeccion Final (Estadisticas Avanzadas)

Formula para calcular proyeccion final basada en velocidad diaria:
```csharp
var diasTranscurridos = campania.FechaInicio.HasValue
    ? (DateTime.UtcNow - campania.FechaInicio.Value).Days
    : 0;

var velocidadDiaria = diasTranscurridos > 0
    ? campania.ImportePledgedActual / diasTranscurridos
    : 0;

var diasRestantes = campania.FechaFin.HasValue
    ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
    : 0;

var proyeccionFinal = campania.ImportePledgedActual + (velocidadDiaria * diasRestantes);
```

### Reward Mas Popular

Logica para identificar el reward mas vendido:
```csharp
var rewardMasPopular = await _context.PedidoCrowdfundingLinea
    .Where(l => l.PedidoCrowdfunding.CampaniaId == campaniaId &&
                l.PedidoCrowdfunding.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO &&
                l.RewardId.HasValue)
    .GroupBy(l => l.RewardId)
    .Select(g => new { RewardId = g.Key, Cantidad = g.Sum(l => l.Cantidad) })
    .OrderByDescending(x => x.Cantidad)
    .FirstOrDefaultAsync();

var rewardNombre = rewardMasPopular != null
    ? await _context.RewardCrowdfunding
        .Where(r => r.Id == rewardMasPopular.RewardId)
        .Select(r => r.Nombre)
        .FirstOrDefaultAsync()
    : null;
```

### Progreso Por Dia (Time Series)

Generar serie temporal de progreso diario:
```csharp
var progresoPorDia = await _context.PedidoCrowdfunding
    .Where(p => p.CampaniaId == campaniaId &&
                p.EstadoPedidoId == ESTADO_PEDIDO.COMPLETADO)
    .GroupBy(p => p.FechaCreacion.Date)
    .Select(g => new ProgressoDiaDto
    {
        Fecha = g.Key.ToString("yyyy-MM-dd"),
        NumBackings = g.Count(),
        TotalRecaudado = g.Sum(p => p.ImporteTotal),
        Acumulado = 0 // Calculado en post-procesamiento
    })
    .OrderBy(x => x.Fecha)
    .ToListAsync();

// Post-procesamiento para acumulado
decimal acumulado = 0;
foreach (var dia in progresoPorDia)
{
    acumulado += dia.TotalRecaudado;
    dia.Acumulado = acumulado;
}
```

### Validacion de Ownership en Queries

**CRITICO**: Todos los queries de campania especifica deben validar ownership:
```csharp
// En Handler
var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
if (artista == null)
{
    return new ServiceResponse<T>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Artista no encontrado", ErrorCode = ServiceResponseMessageType.NotFound_Artista }
        }
    };
}

var campania = await _campaniaService.GetByIdAsync(request.CampaniaId, ct);
if (campania == null)
{
    return new ServiceResponse<T>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "Campania no encontrada", ErrorCode = ServiceResponseMessageType.NotFound_Campania }
        }
    };
}

if (campania.ArtistaId != artista.Id)
{
    return new ServiceResponse<T>
    {
        Messages = new List<ServiceResponseMessage>
        {
            new() { Message = "No tienes permiso para ver esta campania", ErrorCode = ServiceResponseMessageType.Auth_Forbidden }
        }
    };
}
```

### Formato de Email en BackingItem

El email del backer solo se muestra si:
1. El backer NO es anonimo (`PermitirMostrarNombre = true`)
2. El backer esta autenticado (`UserId != null`)

```csharp
var email = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
    ? await _userService.GetUserEmailAsync(pedido.UserId)
    : null;
```

### Paginacion Defaults

- `GetMisCampaniasQuery`: Default pageSize = 10
- `GetCampaniaBackingsQuery`: Default pageSize = 20
- Maximo pageSize = 100 (validar en validator)

### Caching de Artista

El artista del usuario autenticado puede cachearse por request usando `IRequestCacheService`:
```csharp
var artista = await _requestCache.GetOrAddAsync(
    $"artista:user:{userId}",
    async () => await _artistaService.GetByUserIdAsync(userId, ct));
```

Esto evita multiples queries de `Artista` si varios queries se ejecutan en la misma request.

---

**Fin del documento de contratos.**
