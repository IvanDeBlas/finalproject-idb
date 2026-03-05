# Contratos: Valoraciones Bidireccionales

> **Feature:** cs-valoraciones (US-CS-06)
> **Ultima actualizacion:** 2026-02-21
> **Depende de:** cs-acuerdos-entregables (US-CS-04)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones

**Descripcion:** Crea una valoracion del usuario autenticado hacia la otra parte de un acuerdo completado. El valorado se determina automaticamente: si el autor es el artista, el valorado es el profesional, y viceversa. Solo se puede valorar una vez por acuerdo por usuario. No se puede editar ni eliminar una valoracion una vez enviada.

**Autorizacion:** Bearer JWT - Participante del acuerdo (artista o profesional)

**Path Parameters:**
- `acuerdoId` (Guid) - ID del acuerdo completado

**Request Body:**
```json
{
  "puntuacion": 5,
  "comentario": "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles."
}
```

**Nota sobre `comentario`:** Opcional. Max 1000 caracteres.

**Response 201 Created:**
```json
{
  "data": {
    "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    "puntuacion": 5,
    "comentario": "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
    "fechaCreacion": "2026-03-16T10:00:00Z"
  },
  "messages": [
    {
      "message": "Valoracion enviada. Gracias por tu feedback.",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | La puntuacion es obligatoria | puntuacion no enviada |
| 400 | 1014 | La puntuacion debe ser entre 1 y 5 | puntuacion < 1 o > 5 |
| 400 | 1002 | El comentario no puede superar los 1000 caracteres | comentario > 1000 chars |
| 400 | 4007 | Solo se puede valorar acuerdos completados | EstadoAcuerdoId != Completado |
| 400 | 4014 | Ya has dejado una valoracion para este acuerdo | Existe ValoracionCrowdsourcing con mismo AcuerdoId + UserIdAutor |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No eres participante de este acuerdo | UserId del token no coincide con artista ni profesional del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | acuerdoId no existe en DB |
| 500 | 5000 | Error inesperado al crear la valoracion | Excepcion no controlada |

---

### GET /api/crowdsourcing/usuarios/{userId}/valoraciones

**Descripcion:** Devuelve el resumen de valoraciones (puntuacion media, total e histograma de distribucion por estrellas) y el listado paginado de valoraciones recibidas por el usuario indicado. Las valoraciones se devuelven ordenadas por fecha de creacion descendente (mas recientes primero). Cualquier usuario autenticado puede consultar las valoraciones de otro usuario.

**Autorizacion:** Bearer JWT - Cualquier usuario autenticado

**Path Parameters:**
- `userId` (string) - Identity User ID del usuario cuyas valoraciones se consultan

**Query Parameters:**
| Parametro | Tipo | Obligatorio | Default | Descripcion |
|-----------|------|-------------|---------|-------------|
| page | int | No | 1 | Numero de pagina (1-based) |
| pageSize | int | No | 10 | Resultados por pagina (max 50) |

**Response 200 OK:**
```json
{
  "data": {
    "resumen": {
      "puntuacionMedia": 4.5,
      "totalValoraciones": 12,
      "distribucion": {
        "5": 7,
        "4": 3,
        "3": 1,
        "2": 1,
        "1": 0
      }
    },
    "valoraciones": {
      "items": [
        {
          "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
          "puntuacion": 5,
          "comentario": "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
          "autorNombre": "Los Rockeros",
          "autorImagenUrl": "https://storage.example.com/imagenes/los-rockeros.jpg",
          "acuerdoTituloInterno": "Mezcla EP Los Rockeros",
          "fechaCreacion": "2026-03-16T10:00:00Z"
        },
        {
          "id": "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
          "puntuacion": 4,
          "comentario": "Buen trabajo aunque se paso un poco del plazo.",
          "autorNombre": "Indie Band",
          "autorImagenUrl": null,
          "acuerdoTituloInterno": "Mastering Single",
          "fechaCreacion": "2026-02-28T16:00:00Z"
        }
      ],
      "totalCount": 12,
      "page": 1,
      "pageSize": 10
    }
  },
  "messages": []
}
```

**Notas sobre campos calculados:**
- `puntuacionMedia`: `AVG(Puntuacion)` calculado en la query, redondeado a 1 decimal. No se almacena pre-calculado.
- `distribucion`: `GROUP BY Puntuacion COUNT(*)` sobre todas las valoraciones recibidas por el usuario; siempre incluye las 5 claves aunque el valor sea `0`.
- `autorImagenUrl`: puede ser `null` si el autor no tiene imagen de perfil configurada.
- Si el usuario no tiene valoraciones, `puntuacionMedia = null`, `totalValoraciones = 0`, todos los valores de `distribucion` son `0`, e `items` es un array vacio.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 404 | 2000 | Usuario no encontrado | userId no existe en Identity |
| 500 | 5000 | Error inesperado al obtener las valoraciones | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones` | Bearer JWT | Participante del acuerdo | UserId del token debe coincidir con artista o profesional del acuerdo; el valorado se determina automaticamente |
| `GET /api/crowdsourcing/usuarios/{userId}/valoraciones` | Bearer JWT | Cualquier rol autenticado | No hay restriccion por rol; cualquier usuario con token valido puede consultar |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "usuario@example.com",
  "role": "Fan",
  "exp": 1739823600,
  "iat": 1739737200
}
```

**Nota:** El `ArtistaId` y el `PerfilProfesionalId` no se incluyen en el token. El backend resuelve la participacion consultando `AcuerdoCrowdsourcing.UserIdProveedor` (string del Identity) y resolviendo el `ArtistaId` por `UserId` a traves del servicio correspondiente.

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Proteccion de Rutas Frontend

| Ruta (Landing/Admin) | Auth | Redirect | Notas |
|----------------------|------|----------|-------|
| `/crowdsourcing/acuerdos/:id` (seccion valoraciones) | Bearer JWT | `/auth/login` | La seccion de valoracion del acuerdo esta dentro de la vista de detalle ya protegida |
| `/profesionales/:userId` (seccion valoraciones) | Bearer JWT | `/auth/login` | Perfil publico del profesional con historial de valoraciones |

---

## DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateValoracionRequestDto.cs
public class CreateValoracionRequestDto
{
    /// <summary>Requerido. Valor inclusivo entre 1 y 5.</summary>
    public int Puntuacion { get; set; }
    /// <summary>Opcional. Max 1000 chars.</summary>
    public string? Comentario { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ValoracionCreatedResultDto.cs
public class ValoracionCreatedResultDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ValoracionResumenDto.cs
public class ValoracionResumenDto
{
    /// <summary>AVG(Puntuacion) con 1 decimal. Null si no hay valoraciones.</summary>
    public decimal? PuntuacionMedia { get; set; }
    /// <summary>COUNT(*) total de valoraciones recibidas.</summary>
    public int TotalValoraciones { get; set; }
    /// <summary>Histograma: clave es la estrella (1-5), valor es el conteo. Siempre incluye las 5 claves.</summary>
    public Dictionary<int, int> Distribucion { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ValoracionListItemDto.cs
public class ValoracionListItemDto
{
    public Guid Id { get; set; }
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
    /// <summary>Nombre del autor: NombreArtistico si es artista, Nombre del perfil profesional si es profesional.</summary>
    public string AutorNombre { get; set; } = null!;
    /// <summary>URL de imagen de perfil del autor. Null si no tiene.</summary>
    public string? AutorImagenUrl { get; set; }
    /// <summary>TituloInterno del acuerdo al que pertenece la valoracion.</summary>
    public string AcuerdoTituloInterno { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ValoracionesUsuarioDto.cs
public class ValoracionesUsuarioDto
{
    public ValoracionResumenDto Resumen { get; set; } = null!;
    public PaginatedResult<ValoracionListItemDto> Valoraciones { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/PaginatedResult.cs
// (Agregar si no existe como tipo generico compartido en BuildingBlocks o en el modulo)
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts
// Agregar las siguientes interfaces al archivo existente

// --- Requests ---

export interface CreateValoracionRequest {
  puntuacion: number;       // 1-5 inclusive
  comentario?: string;      // max 1000 chars
}

// --- Results ---

export interface ValoracionCreatedResult {
  id: string;
  puntuacion: number;
  comentario?: string;
  fechaCreacion: string;    // ISO datetime string
}

// --- Detail DTOs ---

export interface ValoracionResumen {
  puntuacionMedia: number | null;   // null si no hay valoraciones; 1 decimal
  totalValoraciones: number;
  distribucion: {
    5: number;
    4: number;
    3: number;
    2: number;
    1: number;
  };
}

export interface ValoracionListItem {
  id: string;
  puntuacion: number;
  comentario?: string;
  autorNombre: string;
  autorImagenUrl: string | null;
  acuerdoTituloInterno: string;
  fechaCreacion: string;            // ISO datetime string
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ValoracionesUsuario {
  resumen: ValoracionResumen;
  valoraciones: PaginatedResult<ValoracionListItem>;
}
```

---

## Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| puntuacion | requerido | `.NotNull().WithMessage("La puntuacion es obligatoria").WithErrorCode(Validation_Required)` | `.min(1, 'La puntuacion es obligatoria')` (via schema `gte`) |
| puntuacion | entre 1 y 5 inclusive | `.InclusiveBetween(1, 5).WithMessage("La puntuacion debe ser entre 1 y 5").WithErrorCode(Validation_InvalidRange)` | `.gte(1, 'Minimo 1 estrella').lte(5, 'Maximo 5 estrellas')` |
| comentario | max 1000 chars si presente | `.MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Comentario)).WithMessage("El comentario no puede superar los 1000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(1000, 'El comentario no puede superar los 1000 caracteres').optional()` |
| page (query param) | >= 1 si presente | Validado en el Query handler | `.gte(1).optional()` |
| pageSize (query param) | entre 1 y 50 si presente | Validado en el Query handler | `.gte(1).lte(50).optional()` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
// Agregar al archivo existente

import { z } from 'zod';

export const createValoracionSchema = z.object({
  puntuacion: z
    .number({
      required_error: 'La puntuacion es obligatoria',
      invalid_type_error: 'La puntuacion debe ser un numero',
    })
    .int('La puntuacion debe ser un numero entero')
    .gte(1, 'Minimo 1 estrella')
    .lte(5, 'Maximo 5 estrellas'),
  comentario: z
    .string()
    .max(1000, 'El comentario no puede superar los 1000 caracteres')
    .optional(),
});

export const valoracionesQuerySchema = z.object({
  page: z
    .number()
    .int()
    .gte(1, 'La pagina debe ser mayor a 0')
    .optional()
    .default(1),
  pageSize: z
    .number()
    .int()
    .gte(1)
    .lte(50, 'El tamano de pagina no puede superar 50')
    .optional()
    .default(10),
});

export type CreateValoracionFormData = z.infer<typeof createValoracionSchema>;
export type ValoracionesQueryParams = z.infer<typeof valoracionesQuerySchema>;
```

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Query Keys - agregar dentro de QUERY_KEYS.crowdsourcing:
//
// valoraciones: {
//   byUserId: (userId: string) => ['crowdsourcing', 'valoraciones', userId] as const,
// },
//
// Estructura completa con la nueva adicion:
export const QUERY_KEYS = {
  crowdsourcing: {
    necesidades: {
      mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
      byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
      publicas: ['crowdsourcing', 'necesidades', 'publicas'] as const,
      publicaById: (id: string) => ['crowdsourcing', 'necesidades', 'publica', id] as const,
    },
    propuestas: {
      mis: ['crowdsourcing', 'propuestas', 'mis'] as const,
    },
    acuerdos: {
      byId: (id: string) => ['crowdsourcing', 'acuerdos', id] as const,
    },
    valoraciones: {
      byUserId: (userId: string) => ['crowdsourcing', 'valoraciones', userId] as const,
    },
  },
};

// API Routes - agregar dentro de API_ROUTES.crowdsourcing:
//
// valoraciones: {
//   create: (acuerdoId: string) => `/api/crowdsourcing/acuerdos/${acuerdoId}/valoraciones`,
//   byUser: (userId: string) => `/api/crowdsourcing/usuarios/${userId}/valoraciones`,
// },
//
// Estructura completa con la nueva adicion:
export const API_ROUTES = {
  crowdsourcing: {
    necesidades: {
      base: '/api/crowdsourcing/necesidades',
      mis: '/api/crowdsourcing/necesidades/mis-necesidades',
      byId: (id: string) => `/api/crowdsourcing/necesidades/${id}`,
      cerrar: (id: string) => `/api/crowdsourcing/necesidades/${id}/cerrar`,
      propuestas: (necesidadId: string) =>
        `/api/crowdsourcing/necesidades/${necesidadId}/propuestas`,
    },
    propuestas: {
      mis: '/api/crowdsourcing/propuestas/mis-propuestas',
      retirar: (id: string) => `/api/crowdsourcing/propuestas/${id}/retirar`,
      aceptar: (id: string) => `/api/crowdsourcing/propuestas/${id}/aceptar`,
      rechazar: (id: string) => `/api/crowdsourcing/propuestas/${id}/rechazar`,
    },
    acuerdos: {
      byId: (id: string) => `/api/crowdsourcing/acuerdos/${id}`,
      completar: (id: string) => `/api/crowdsourcing/acuerdos/${id}/completar`,
      cancelar: (id: string) => `/api/crowdsourcing/acuerdos/${id}/cancelar`,
      milestones: (acuerdoId: string) =>
        `/api/crowdsourcing/acuerdos/${acuerdoId}/milestones`,
      milestoneById: (acuerdoId: string, id: string) =>
        `/api/crowdsourcing/acuerdos/${acuerdoId}/milestones/${id}`,
      entregables: (acuerdoId: string) =>
        `/api/crowdsourcing/acuerdos/${acuerdoId}/entregables`,
    },
    entregables: {
      aprobar: (id: string) => `/api/crowdsourcing/entregables/${id}/aprobar`,
      rechazar: (id: string) => `/api/crowdsourcing/entregables/${id}/rechazar`,
    },
    valoraciones: {
      create: (acuerdoId: string) =>
        `/api/crowdsourcing/acuerdos/${acuerdoId}/valoraciones`,
      byUser: (userId: string) =>
        `/api/crowdsourcing/usuarios/${userId}/valoraciones`,
    },
  },
};
```

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo `Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// Validation (1000-1999)
public const string Validation_InvalidRange = "1014";       // Puntuacion fuera del rango 1-5

// Business Rules (4000-4999)
// Nota: BusinessRule_InvalidState = "4007" ya existe (acuerdo no completado)
public const string BusinessRule_DuplicateAction = "4014";  // Ya existe valoracion para este acuerdo por este usuario
```

**Constantes ya existentes utilizadas en esta feature:**
| Constante | Codigo | Uso |
|-----------|--------|-----|
| `Validation_Required` | `1001` | Puntuacion no enviada |
| `Validation_MaxLength` | `1002` | Comentario > 1000 chars |
| `BusinessRule_InvalidState` | `4007` | Acuerdo no esta en estado Completado |
| `NotFound_Entity` | `2000` | Usuario no encontrado |
| `NotFound_Acuerdo` | `2011` | Acuerdo no encontrado |
| `Auth_Unauthorized` | `3001` | Token invalido/expirado |
| `Auth_Forbidden` | `3002` | Usuario no es participante del acuerdo |
| `Internal_UnexpectedError` | `5000` | Excepcion no controlada |

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... mensajes existentes de US-CS-04

  // Crowdsourcing - Valoraciones
  '1014': 'La puntuacion debe ser entre 1 y 5 estrellas',
  '4014': 'Ya has enviado una valoracion para este acuerdo',

  // Reutilizados de US-CS-04 (ya deben existir)
  // '4007': 'La propuesta no esta disponible para esta accion'
  //   -> Para valoraciones se usa con el mismo codigo pero distinto contexto
  //   -> Sobreescribir con mensaje mas contextual o manejar por endpoint

  // Keys semanticos para uso interno
  VALORACION_DUPLICATE: 'Ya has enviado una valoracion para este acuerdo',
  VALORACION_ACUERDO_NOT_COMPLETED: 'Solo puedes valorar acuerdos que han sido completados',
  VALORACION_PUNTUACION_INVALID: 'La puntuacion debe ser entre 1 y 5 estrellas',
  VALORACION_COMENTARIO_MAX: 'El comentario no puede superar los 1000 caracteres',
};
```

**Nota sobre el codigo `4007`:** Este codigo ya existe en `error-messages.ts` como "La propuesta no esta disponible para esta accion" (US-CS-04). Para valoraciones, el mensaje apropiado es "Solo puedes valorar acuerdos que han sido completados". El frontend debe distinguir el contexto segun la operacion que fallo y mostrar el mensaje adecuado. Se recomienda manejar este caso en el hook especifico (`useCreateValoracion`) con un mensaje hardcodeado antes de delegar al mapeo generico.

---

## Notas de Implementacion

### Backend

1. **CreateValoracionCommand:**
   - Obtener `UserId` del token.
   - Cargar el acuerdo por `acuerdoId`; si no existe, retornar `NotFound_Acuerdo` (2011).
   - Resolver la participacion: verificar que `UserId == AcuerdoCrowdsourcing.UserIdProveedor` o que el `UserId` corresponde al artista del acuerdo (resolver `ArtistaId` via `IArtistaService`).
   - Si no es participante, retornar `Auth_Forbidden` (3002).
   - Validar que `EstadoAcuerdoId == Completado`; si no, retornar `BusinessRule_InvalidState` (4007).
   - Verificar unicidad: consultar si existe `ValoracionCrowdsourcing` con `AcuerdoId == acuerdoId AND UserIdAutor == UserId`; si existe, retornar `BusinessRule_DuplicateAction` (4014).
   - Determinar `UserIdValorado`: si el autor es el artista, el valorado es `UserIdProveedor`; si el autor es el profesional, el valorado es el `UserId` del artista.
   - Crear `ValoracionCrowdsourcing` con `FechaCreacion = DateTime.UtcNow`.
   - La unicidad tambien se garantiza con un constraint unico en BD sobre `(AcuerdoId, UserIdAutor)`.

2. **GetValoracionesUsuarioQuery:**
   - Verificar que el `userId` del path corresponde a un usuario existente en Identity; si no, retornar `NotFound_Entity` (2000).
   - Consultar todas las `ValoracionCrowdsourcing` donde `UserIdValorado == userId`.
   - Calcular `PuntuacionMedia = AVG(Puntuacion)` redondeado a 1 decimal; `null` si `TotalValoraciones == 0`.
   - Calcular `Distribucion` con `GROUP BY Puntuacion COUNT(*)`, garantizando que las 5 claves (1-5) esten presentes aunque el valor sea `0`.
   - Aplicar paginacion: `SKIP((page - 1) * pageSize) TAKE(pageSize)`, ordenando por `FechaCreacion DESC`.
   - Proyectar `AutorNombre`: si el autor es artista (tiene perfil `Artista`), usar `NombreArtistico`; si es profesional, usar el nombre del perfil profesional; si ninguno, usar el `UserName` de Identity.
   - Proyectar `AutorImagenUrl` desde el perfil correspondiente; puede ser `null`.
   - Usar `AsNoTracking()`.

3. **Caching:** Usar `IRequestCacheService` para la resolucion de `ArtistaId` por `UserId` del token (compartida entre Validator y Handler dentro del mismo request).

4. **Constraint unico en DB:** La tabla `ValoracionCrowdsourcing` debe tener un indice unico sobre `(AcuerdoId, UserIdAutor)`. Esto previene duplicados a nivel de base de datos ademas de la validacion en el handler.

### Frontend

1. **Nuevos archivos recomendados:**
   - `src/web/src/features/crowdsourcing/valoraciones/` - Feature de valoraciones (Landing)
   - Agregar schemas en `src/shared/schemas/crowdsourcing.schema.ts`
   - Agregar tipos en `src/shared/types/crowdsourcing.ts`

2. **Componentes clave (Landing):**
   - `ValoracionForm.tsx` - Formulario con selector de estrellas interactivo (hover effect) y textarea opcional; se muestra en la vista de detalle del acuerdo cuando `estadoAcuerdoNombre == 'Completado'` y el usuario no ha valorado aun.
   - `ValoracionExistente.tsx` - Muestra la valoracion ya enviada en modo lectura (no editable); reemplaza al formulario una vez enviada.
   - `StarRatingInput.tsx` - Componente custom de seleccion de estrellas con efecto hover (shadcn no tiene nativo; implementar como custom o usar una libreria ligera).
   - `ValoracionesSection.tsx` - Seccion completa de valoraciones en el perfil del profesional: resumen + histograma + lista paginada.
   - `ValoracionResumenCard.tsx` - Tarjeta con puntuacion media, total y barra de histograma.
   - `ValoracionItem.tsx` - Item individual de valoracion con estrellas, comentario, autor y fecha.
   - `StarRatingDisplay.tsx` - Componente de solo lectura para mostrar estrellas (reutilizable en badges y listados).

3. **Hooks personalizados:**
   - `useCreateValoracion(acuerdoId)` - Mutation `POST`; invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)` y `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userIdValorado)` (si se conoce en cliente); muestra toast con `messages[0].message` de la respuesta.
   - `useValoracionesUsuario(userId, params)` - Query con `QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId)` y params de paginacion; `enabled: !!userId`.

4. **UX considerations:**
   - La seccion de dejar valoracion solo aparece en la vista de detalle del acuerdo cuando `estadoAcuerdoNombre == 'Completado'`.
   - La condicion de "ya valorado" debe verificarse en el servidor (la respuesta 400 con codigo `4014` es la fuente de verdad). El frontend puede pre-verificar si el acuerdo devuelve un campo `miValoracion` pero no es obligatorio en el MVP.
   - Mostrar incentivo: "Tu comentario ayuda a otros artistas/profesionales" como texto de apoyo bajo el textarea.
   - El selector de estrellas debe tener efecto hover (iluminar estrellas al pasar el cursor) para una experiencia intuitiva.
   - Una vez enviada, reemplazar el formulario por la valoracion en modo lectura con el texto "Valoracion enviada" y no permitir ninguna edicion.
   - El badge de puntuacion media (`[*] 4.5 (12)`) se muestra en tarjetas de propuestas y listados de profesionales; consumir del resumen del endpoint `GET /api/crowdsourcing/usuarios/{userId}/valoraciones` o de un endpoint de perfil si existe.
   - En el perfil del profesional, si `totalValoraciones == 0`, mostrar el estado vacio: "Este usuario aun no tiene valoraciones".
   - Toast de exito tras crear valoracion: usar el `message` del campo `messages[0].message` de la respuesta (`"Valoracion enviada. Gracias por tu feedback."`).

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (2 endpoints documentados)
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (CreateValoracionRequestDto, ValoracionCreatedResultDto, ValoracionResumenDto, ValoracionListItemDto, ValoracionesUsuarioDto, PaginatedResult)
- [x] Types TypeScript equivalentes (CreateValoracionRequest, ValoracionCreatedResult, ValoracionResumen, ValoracionListItem, PaginatedResult, ValoracionesUsuario)
- [x] Schemas Zod con mismas reglas (createValoracionSchema, valoracionesQuerySchema)
- [x] Constantes compartidas (QUERY_KEYS.crowdsourcing.valoraciones, API_ROUTES.crowdsourcing.valoraciones)
- [x] Nuevas constantes ServiceResponseMessageType (Validation_InvalidRange = "1014", BusinessRule_DuplicateAction = "4014")
- [x] Mapeo de errores a mensajes UI
- [x] Notas de implementacion detalladas (Backend + Frontend)
- [x] Logica de determinacion del UserIdValorado documentada
- [x] Campos calculados del resumen documentados (puntuacionMedia, distribucion)
- [x] Nota sobre constraint unico en BD
- [x] Nota sobre colision del codigo 4007 con US-CS-04
