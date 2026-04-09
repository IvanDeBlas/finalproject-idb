# Contratos API: Dashboard de Artista

**Fecha:** 2026-02-14
**Modulo:** Crowdfunding
**Feature:** dashboard-artista

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Descripcion |
|--------|------|------|-------------|
| GET | /api/dashboard/resumen | Query | Obtener resumen general del artista (metricas agregadas) |
| GET | /api/campanias/mis-campanias | Query | Listar campanias del artista autenticado con paginacion |
| GET | /api/campanias/{id}/backings | Query | Listar backings de una campania con estadisticas |
| GET | /api/campanias/{id}/stats | Query | Obtener estadisticas detalladas de una campania |

**Autorizacion:** Todos los endpoints requieren Bearer JWT con rol `Artista`.

---

## 2. Request DTOs

### 2.1 GetDashboardResumenQuery
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Dashboard/Queries/GetDashboardResumenQuery.cs`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| UserId | string | Si | ID del usuario autenticado (inyectado desde JWT) |

**Implementa:** `IRequest<ServiceResponse<DashboardResumenDto>>`

**Validacion:** No requiere validator (UserId se valida en Handler verificando que tenga perfil Artista).

**Notas:**
- UserId se extrae del claim `sub` del token JWT en el Controller
- Handler valida que el usuario tiene perfil de Artista asociado

---

### 2.2 GetMisCampaniasQuery
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetMisCampaniasQuery.cs`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| UserId | string | Si | ID del usuario autenticado (inyectado desde JWT) |
| EstadoCampaniaId | int? | No | Filtrar por estado (1=Borrador, 2=Publicada, 3=Finalizada, 4=Cancelada, 5=Pausada) |
| Page | int | No | Numero de pagina (default: 1) |
| PageSize | int | No | Items por pagina (default: 10) |

**Implementa:** `IRequest<ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>>`

**Validacion:** Requiere validator `GetMisCampaniasQueryValidator`

---

### 2.3 GetCampaniaBackingsQuery
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaBackingsQuery.cs`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| CampaniaId | Guid | Si | ID de la campania (route parameter) |
| UserId | string | Si | ID del usuario autenticado (para validar ownership) |
| Page | int | No | Numero de pagina (default: 1) |
| PageSize | int | No | Items por pagina (default: 20) |

**Implementa:** `IRequest<ServiceResponse<CampaniaBackingListDto>>`

**Validacion:** Requiere validator `GetCampaniaBackingsQueryValidator`

**Autorizacion:**
- Handler debe validar que `campania.ArtistaId == artista.Id` del usuario autenticado
- Retornar `Auth_Forbidden` (3002) si no coincide

---

### 2.4 GetCampaniaStatsQuery
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Queries/GetCampaniaStatsQuery.cs`

| Propiedad | Tipo | Requerido | Descripcion |
|-----------|------|-----------|-------------|
| CampaniaId | Guid | Si | ID de la campania (route parameter) |
| UserId | string | Si | ID del usuario autenticado (para validar ownership) |

**Implementa:** `IRequest<ServiceResponse<CampaniaStatsDetailDto>>`

**Validacion:** Requiere validator `GetCampaniaStatsQueryValidator`

**Autorizacion:**
- Validar ownership igual que GetCampaniaBackingsQuery

---

## 3. Response DTOs

### 3.1 DashboardResumenDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/DashboardResumenDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| ArtistaId | Guid | ID del artista |
| NombreArtistico | string | Nombre artistico del artista |
| TotalRecaudado | decimal | Suma de `ImportePledgedActual` de todas las campanias |
| TotalBackers | int | Count distinct de `UserId` en pedidos completados |
| CampaniasActivas | int | Count de campanias con `EstadoCampaniaId = 2` (Publicada) |
| CampaniasCompletadas | int | Count de campanias con `EstadoCampaniaId = 3` (Finalizada) |
| TotalCampanias | int | Count total de campanias del artista |
| MonedaSimbolo | string | Simbolo de moneda (default: "EUR") |
| FechaUltimoAporte | DateTime? | FechaCreacion del pedido mas reciente (nullable) |

**Wrapped en:** `ServiceResponse<DashboardResumenDto>`

**Calculos:**
- `TotalRecaudado = SUM(CampaniaCrowdfunding.ImportePledgedActual WHERE ArtistaId = artistaId)`
- `TotalBackers = COUNT(DISTINCT PedidoCrowdfunding.UserId WHERE EstadoPedidoId = 3 AND CampaniaCrowdfunding.ArtistaId = artistaId)`
- `CampaniasActivas = COUNT(CampaniaCrowdfunding WHERE ArtistaId = artistaId AND EstadoCampaniaId = 2)`
- `CampaniasCompletadas = COUNT(CampaniaCrowdfunding WHERE ArtistaId = artistaId AND EstadoCampaniaId = 3)`
- `FechaUltimoAporte = MAX(PedidoCrowdfunding.FechaCreacion WHERE EstadoPedidoId = 3 AND CampaniaCrowdfunding.ArtistaId = artistaId)`

**Notas:**
- Solo contar pedidos con `EstadoPedidoId = 3` (Completado) para metricas
- Si no hay pedidos, `FechaUltimoAporte` es `null`

---

### 3.2 MiCampaniaListItemDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/MiCampaniaListItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID de la campania |
| Titulo | string | Titulo de la campania |
| ImagenPrincipalUrl | string? | URL de imagen principal (nullable) |
| EstadoCampaniaId | int | ID de estado (1-5) |
| EstadoCampaniaNombre | string | Nombre del estado (Borrador, Publicada, Finalizada, etc.) |
| ImporteObjetivo | decimal | Importe objetivo de la campania |
| ImporteRecaudado | decimal | ImportePledgedActual (calculado desde pedidos) |
| PorcentajeProgreso | decimal | `(ImporteRecaudado / ImporteObjetivo) * 100` (2 decimales) |
| NumBackers | int | Count de pedidos completados para esta campania |
| DiasRestantes | int? | `MAX(0, (FechaFin - DateTime.UtcNow).Days)` (nullable si no hay FechaFin) |
| FechaFin | DateTime? | Fecha de fin de la campania (nullable) |
| FechaCreacion | DateTime | Fecha de creacion de la campania |

**Wrapped en:** `ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>`

**Calculos:**
- `ImporteRecaudado = SUM(PedidoCrowdfunding.ImporteTotal WHERE CampaniaId = id AND EstadoPedidoId = 3)`
- `PorcentajeProgreso = Math.Round((ImporteRecaudado / ImporteObjetivo) * 100, 2)`
- `NumBackers = COUNT(PedidoCrowdfunding WHERE CampaniaId = id AND EstadoPedidoId = 3)`
- `DiasRestantes = FechaFin.HasValue ? Math.Max(0, (FechaFin.Value - DateTime.UtcNow).Days) : null`

**Ordenamiento:** `ORDER BY FechaCreacion DESC`

---

### 3.3 PaginatedResponse<T>
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/PaginatedResponse.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Items | List<T> | Lista de items de la pagina actual |
| TotalCount | int | Total de registros (sin paginacion) |
| Page | int | Numero de pagina actual |
| PageSize | int | Cantidad de items por pagina |
| TotalPages | int | Total de paginas calculado: `(TotalCount + PageSize - 1) / PageSize` |

**Notas:**
- Este DTO es generico y reutilizable para todas las queries paginadas
- `TotalPages` debe calcularse correctamente para evitar division por cero

---

### 3.4 CampaniaBackingListDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingListDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania |
| CampaniaTitulo | string | Titulo de la campania |
| Stats | CampaniaBackingStatsDto | Estadisticas agregadas de backings |
| Backings | PaginatedResponse<CampaniaBackingItemDto> | Lista paginada de backings |

**Wrapped en:** `ServiceResponse<CampaniaBackingListDto>`

---

### 3.5 CampaniaBackingStatsDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingStatsDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| TotalRecaudado | decimal | Suma de `ImporteTotal` de pedidos completados |
| BackingPromedio | decimal | `TotalRecaudado / TotalBackers` (2 decimales) |
| TotalBackers | int | Count de pedidos completados |
| RewardMasPopular | string? | Nombre del reward con mas pedidos (nullable si no hay rewards) |
| UltimoBacking | UltimoBackingDto? | Datos del backing mas reciente (nullable si no hay backings) |

**Calculos:**
- `TotalRecaudado = SUM(ImporteTotal WHERE EstadoPedidoId = 3)`
- `BackingPromedio = TotalBackers > 0 ? Math.Round(TotalRecaudado / TotalBackers, 2) : 0`
- `RewardMasPopular`: Query GROUP BY RewardId, ordenar por COUNT DESC, tomar el primero

---

### 3.6 UltimoBackingDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/UltimoBackingDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| NombreBacker | string | Nombre del backer o "Anonimo" si `PermitirMostrarNombre = false` |
| Monto | decimal | ImporteTotal del pedido |
| FechaCreacion | DateTime | Fecha de creacion del pedido |

---

### 3.7 CampaniaBackingItemDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaBackingItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | ID del pedido |
| NombreBacker | string | Nombre del usuario o "Anonimo" |
| Email | string? | Email del usuario (nullable si anonimo o no disponible) |
| Monto | decimal | ImporteTotal del pedido |
| RewardNombre | string? | Nombre del reward seleccionado (nullable si backing sin reward) |
| Mensaje | string? | ComentarioBacker (nullable) |
| EsAnonimo | bool | `!PermitirMostrarNombre` |
| EstadoPedido | string | Nombre del estado del pedido (Completado, Pendiente, etc.) |
| FechaCreacion | DateTime | Fecha de creacion del pedido |

**Reglas de privacidad:**
- Si `PermitirMostrarNombre = false` → `NombreBacker = "Anonimo"`, `Email = null`
- Si `PermitirMostrarNombre = true` → Obtener nombre y email del usuario autenticado o del FanProfile

**Ordenamiento:** `ORDER BY FechaCreacion DESC`

**Notas:**
- El reward se obtiene de `PedidoCrowdfundingLinea` asociada al pedido
- Si un pedido tiene multiples rewards, mostrar el primero o concatenar (decision de implementacion)

---

### 3.8 CampaniaStatsDetailDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/CampaniaStatsDetailDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| CampaniaId | Guid | ID de la campania |
| CampaniaTitulo | string | Titulo de la campania |
| ImporteObjetivo | decimal | Importe objetivo |
| ImporteRecaudado | decimal | ImportePledgedActual |
| PorcentajeProgreso | decimal | `(ImporteRecaudado / ImporteObjetivo) * 100` (2 decimales) |
| NumBackers | int | Count de pedidos completados |
| BackingPromedio | decimal | `ImporteRecaudado / NumBackers` (2 decimales) |
| DiasRestantes | int? | Dias restantes hasta FechaFin (nullable) |
| DiasTranscurridos | int | `(DateTime.UtcNow - FechaInicio).Days` |
| TotalDiasCampania | int | `(FechaFin - FechaInicio).Days` |
| ProyeccionFinal | decimal? | Estimacion final basada en velocidad diaria (nullable) |
| VelocidadDiaria | decimal | Promedio de recaudacion por dia |
| RewardStats | List<RewardStatDto> | Estadisticas por reward |
| ProgressoPorDia | List<ProgressoDiaDto> | Progreso diario (serie temporal) |

**Calculos avanzados:**
- `DiasTranscurridos = FechaInicio.HasValue ? (DateTime.UtcNow - FechaInicio.Value).Days : 0`
- `VelocidadDiaria = DiasTranscurridos > 0 ? ImporteRecaudado / DiasTranscurridos : 0`
- `ProyeccionFinal = ImporteRecaudado + (VelocidadDiaria * DiasRestantes)` (si DiasRestantes > 0)

---

### 3.9 RewardStatDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/RewardStatDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| RewardId | Guid? | ID del reward (nullable si "Sin recompensa") |
| RewardNombre | string | Nombre del reward o "Sin recompensa" |
| CantidadVendida | int | Count de pedidos con este reward |
| TotalRecaudado | decimal | Suma de ImporteTotal de pedidos con este reward |
| PorcentajeDelTotal | decimal | `(TotalRecaudado / ImporteRecaudadoCampania) * 100` (2 decimales) |

**Notas:**
- Incluir un item con `RewardId = null` para backings sin reward (aportaciones directas)
- Ordenar por `CantidadVendida DESC` o `TotalRecaudado DESC`

---

### 3.10 ProgressoDiaDto
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Dtos/ProgressoDiaDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Fecha | string | Fecha en formato "yyyy-MM-dd" |
| NumBackings | int | Count de pedidos en este dia |
| TotalRecaudado | decimal | Suma de ImporteTotal en este dia |
| Acumulado | decimal | Suma acumulada hasta este dia (calculado en post-procesamiento) |

**Query:**
```sql
SELECT
  DATE(FechaCreacion) as Fecha,
  COUNT(*) as NumBackings,
  SUM(ImporteTotal) as TotalRecaudado
FROM PedidoCrowdfunding
WHERE CampaniaId = @campaniaId AND EstadoPedidoId = 3
GROUP BY DATE(FechaCreacion)
ORDER BY Fecha ASC
```

**Post-procesamiento:**
- Calcular `Acumulado` iterando la lista y sumando `TotalRecaudado` de dias previos

---

## 4. Validadores

### 4.1 GetMisCampaniasQueryValidator
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetMisCampaniasQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| EstadoCampaniaId | InclusiveBetween(1, 5) (si no es null) | Estado de campania invalido | Validation_InvalidRange |
| Page | GreaterThanOrEqualTo(1) | La pagina debe ser mayor o igual a 1 | Validation_InvalidRange |
| PageSize | InclusiveBetween(1, 100) | El tamano de pagina debe estar entre 1 y 100 | Validation_InvalidRange |

**Implementacion:**
```csharp
public class GetMisCampaniasQueryValidator : AbstractValidator<GetMisCampaniasQuery>
{
    public GetMisCampaniasQueryValidator()
    {
        RuleFor(x => x.EstadoCampaniaId)
            .InclusiveBetween(1, 5)
            .When(x => x.EstadoCampaniaId.HasValue)
            .WithMessage("Estado de campania invalido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("El tamano de pagina debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

### 4.2 GetCampaniaBackingsQueryValidator
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaBackingsQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | Validation_Required |
| Page | GreaterThanOrEqualTo(1) | La pagina debe ser mayor o igual a 1 | Validation_InvalidRange |
| PageSize | InclusiveBetween(1, 100) | El tamano de pagina debe estar entre 1 y 100 | Validation_InvalidRange |

**Implementacion:**
```csharp
public class GetCampaniaBackingsQueryValidator : AbstractValidator<GetCampaniaBackingsQuery>
{
    public GetCampaniaBackingsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("El tamano de pagina debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
```

---

### 4.3 GetCampaniaStatsQueryValidator
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Features/Campanias/Validators/GetCampaniaStatsQueryValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| CampaniaId | NotEmpty | El ID de campania es obligatorio | Validation_Required |

**Implementacion:**
```csharp
public class GetCampaniaStatsQueryValidator : AbstractValidator<GetCampaniaStatsQuery>
{
    public GetCampaniaStatsQueryValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El ID de campania es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
```

---

## 5. AutoMapper Mappings

### 5.1 DashboardProfile
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/DashboardProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| (Logica en Handler) | DashboardResumenDto | Mapeo manual (calculos complejos con multiples entidades) |
| CampaniaCrowdfunding | MiCampaniaListItemDto | Mapeo parcial (calculos adicionales en Handler) |

**Implementacion:**
```csharp
public class DashboardProfile : Profile
{
    public DashboardProfile()
    {
        // No hay mapeos directos Entity -> DTO porque los DTOs son resultado de queries agregadas
        // Los DTOs se construyen manualmente en los Handlers con calculos especificos
    }
}
```

**Notas:**
- Los DTOs de dashboard requieren calculos complejos (agregaciones, JOINs) que no se pueden hacer con AutoMapper
- Los Handlers construiran los DTOs manualmente usando LINQ y proyecciones SQL

---

### 5.2 CampaniaBackingProfile
**Archivo:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/Mapping/CampaniaBackingProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| PedidoCrowdfunding | CampaniaBackingItemDto | Mapeo con ForMember para logica de anonimato |

**Implementacion:**
```csharp
public class CampaniaBackingProfile : Profile
{
    public CampaniaBackingProfile()
    {
        CreateMap<PedidoCrowdfunding, CampaniaBackingItemDto>()
            .ForMember(dest => dest.NombreBacker,
                       opt => opt.MapFrom((src, dest, destMember, context) =>
                           src.PermitirMostrarNombre
                               ? context.Items["UserName"] as string ?? "Backer"
                               : "Anonimo"))
            .ForMember(dest => dest.Email,
                       opt => opt.MapFrom((src, dest, destMember, context) =>
                           src.PermitirMostrarNombre
                               ? context.Items["UserEmail"] as string
                               : null))
            .ForMember(dest => dest.Monto,
                       opt => opt.MapFrom(src => src.ImporteTotal))
            .ForMember(dest => dest.Mensaje,
                       opt => opt.MapFrom(src => src.ComentarioBacker))
            .ForMember(dest => dest.EsAnonimo,
                       opt => opt.MapFrom(src => !src.PermitirMostrarNombre))
            .ForMember(dest => dest.RewardNombre,
                       opt => opt.MapFrom((src, dest, destMember, context) =>
                           context.Items["RewardNombre"] as string))
            .ForMember(dest => dest.EstadoPedido,
                       opt => opt.MapFrom((src, dest, destMember, context) =>
                           context.Items["EstadoPedidoNombre"] as string));
    }
}
```

**Notas:**
- El mapping requiere contexto adicional (UserName, UserEmail, RewardNombre, EstadoPedidoNombre)
- El Handler debe pasar estos datos via `mapper.Map<>(source, opts => opts.Items["Key"] = value)`
- **Alternativa mas simple:** Construir el DTO manualmente en el Handler sin AutoMapper (recomendado para este caso)

---

## 6. OpenAPI Documentation

### 6.1 GET /api/dashboard/resumen
**Summary:** Obtener resumen general del dashboard del artista

**Tags:** Dashboard

**Authentication:** Bearer JWT (required)

**Authorization:** Requiere rol `Artista`

**Responses:**
- **200 OK**: `ServiceResponse<DashboardResumenDto>`
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
- **401 Unauthorized**: Token invalido o expirado
  ```json
  {
    "messages": [
      {
        "message": "Token no valido o expirado",
        "errorCode": "3001"
      }
    ]
  }
  ```
- **403 Forbidden**: Usuario no tiene perfil de Artista
  ```json
  {
    "messages": [
      {
        "message": "Acceso denegado",
        "errorCode": "3002"
      }
    ]
  }
  ```
- **404 NotFound**: Artista no encontrado
  ```json
  {
    "messages": [
      {
        "message": "Artista no encontrado",
        "errorCode": "2002"
      }
    ]
  }
  ```
- **500 Internal Server Error**: Error inesperado
  ```json
  {
    "messages": [
      {
        "message": "Error inesperado",
        "errorCode": "5000"
      }
    ]
  }
  ```

---

### 6.2 GET /api/campanias/mis-campanias
**Summary:** Listar campanias del artista autenticado

**Tags:** Campanias

**Authentication:** Bearer JWT (required)

**Authorization:** Requiere rol `Artista`

**Query Parameters:**
- `estadoCampaniaId` (integer, optional): Filtrar por estado (1-5)
- `page` (integer, default: 1): Numero de pagina
- `pageSize` (integer, default: 10): Items por pagina (max: 100)

**Responses:**
- **200 OK**: `ServiceResponse<PaginatedResponse<MiCampaniaListItemDto>>`
- **400 Bad Request**: Parametros invalidos (validacion)
- **401 Unauthorized**: Token invalido
- **403 Forbidden**: Usuario no es artista
- **404 NotFound**: Artista no encontrado
- **500 Internal Server Error**: Error inesperado

---

### 6.3 GET /api/campanias/{id}/backings
**Summary:** Listar backings de una campania especifica

**Tags:** Campanias

**Authentication:** Bearer JWT (required)

**Authorization:** Requiere rol `Artista` y ownership de la campania

**Path Parameters:**
- `id` (Guid, required): ID de la campania

**Query Parameters:**
- `page` (integer, default: 1): Numero de pagina
- `pageSize` (integer, default: 20): Items por pagina (max: 100)

**Responses:**
- **200 OK**: `ServiceResponse<CampaniaBackingListDto>`
- **400 Bad Request**: Parametros invalidos
- **401 Unauthorized**: Token invalido
- **403 Forbidden**: Campania no pertenece al artista
- **404 NotFound**: Campania no encontrada
- **500 Internal Server Error**: Error inesperado

---

### 6.4 GET /api/campanias/{id}/stats
**Summary:** Obtener estadisticas detalladas de una campania

**Tags:** Campanias

**Authentication:** Bearer JWT (required)

**Authorization:** Requiere rol `Artista` y ownership de la campania

**Path Parameters:**
- `id` (Guid, required): ID de la campania

**Responses:**
- **200 OK**: `ServiceResponse<CampaniaStatsDetailDto>`
- **400 Bad Request**: Parametros invalidos
- **401 Unauthorized**: Token invalido
- **403 Forbidden**: Campania no pertenece al artista
- **404 NotFound**: Campania no encontrada
- **500 Internal Server Error**: Error inesperado

---

## 7. Archivos a Crear

```
Modules/Crowdfunding/WePlayRises.Crowdfunding.Application/
├── Features/
│   ├── Dashboard/
│   │   └── Queries/
│   │       └── GetDashboardResumenQuery.cs        (Query + Handler en MISMO archivo)
│   └── Campanias/
│       ├── Queries/
│       │   ├── GetMisCampaniasQuery.cs            (Query + Handler)
│       │   ├── GetCampaniaBackingsQuery.cs        (Query + Handler)
│       │   └── GetCampaniaStatsQuery.cs           (Query + Handler)
│       └── Validators/
│           ├── GetMisCampaniasQueryValidator.cs
│           ├── GetCampaniaBackingsQueryValidator.cs
│           └── GetCampaniaStatsQueryValidator.cs
├── Dtos/
│   ├── DashboardResumenDto.cs
│   ├── MiCampaniaListItemDto.cs
│   ├── PaginatedResponse.cs                       (Generico reutilizable)
│   ├── CampaniaBackingListDto.cs
│   ├── CampaniaBackingStatsDto.cs
│   ├── UltimoBackingDto.cs
│   ├── CampaniaBackingItemDto.cs
│   ├── CampaniaStatsDetailDto.cs
│   ├── RewardStatDto.cs
│   └── ProgressoDiaDto.cs
└── Mapping/
    └── DashboardProfile.cs                        (Opcional - mappings simples)
```

**Nota sobre Mappings:**
- La mayoria de DTOs de dashboard se construyen con queries complejas (agregaciones, JOINs)
- Recomendacion: **No usar AutoMapper** para estos DTOs, construirlos manualmente en Handlers
- AutoMapper es util solo si hay mappings 1:1 simples (Entidad -> DTO basico)

---

## 8. Estructura de Queries y Handlers

### 8.1 Patron General (Query + Handler en MISMO archivo)

```csharp
// GetDashboardResumenQuery.cs
using MediatR;
using WePlayRises.BuildingBlocks.Application.ServiceResponse;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;

// QUERY (Request)
public class GetDashboardResumenQuery : IRequest<ServiceResponse<DashboardResumenDto>>
{
    public string UserId { get; set; } = null!;
}

// HANDLER (RequestHandler)
public class GetDashboardResumenQueryHandler : IRequestHandler<GetDashboardResumenQuery, ServiceResponse<DashboardResumenDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly ICampaniaCrowdfundingService _campaniaService;
    private readonly ILogger<GetDashboardResumenQueryHandler> _logger;

    public GetDashboardResumenQueryHandler(
        IArtistaService artistaService,
        ICampaniaCrowdfundingService campaniaService,
        ILogger<GetDashboardResumenQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<DashboardResumenDto>> Handle(GetDashboardResumenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario tiene perfil de Artista
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, cancellationToken);
            if (artista == null)
            {
                return new ServiceResponse<DashboardResumenDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Artista no encontrado", ErrorCode = ServiceResponseMessageType.NotFound_Artista }
                    }
                };
            }

            // 2. Obtener metricas usando Services (que internamente usan Repository y DbContext)
            var resumen = await _campaniaService.GetDashboardResumenAsync(artista.Id, cancellationToken);

            // 3. Retornar ServiceResponse exitoso
            return new ServiceResponse<DashboardResumenDto>
            {
                Data = resumen,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Resumen obtenido", ErrorCode = ServiceResponseMessageType.Success }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo resumen de dashboard para userId {UserId}", request.UserId);
            return new ServiceResponse<DashboardResumenDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Error inesperado", ErrorCode = ServiceResponseMessageType.Internal_UnexpectedError }
                }
            };
        }
    }
}
```

---

### 8.2 Consideraciones de Implementacion

**Services requeridos:**
- `IArtistaService.GetByUserIdAsync(string userId, CancellationToken ct)`: Obtener artista por UserId
- `ICampaniaCrowdfundingService.GetDashboardResumenAsync(Guid artistaId, CancellationToken ct)`: Calcular resumen
- `ICampaniaCrowdfundingService.GetMisCampaniasAsync(Guid artistaId, int? estadoId, int page, int pageSize, CancellationToken ct)`: Listar campanias paginadas
- `ICampaniaCrowdfundingService.GetCampaniaBackingsAsync(Guid campaniaId, int page, int pageSize, CancellationToken ct)`: Listar backings paginados
- `ICampaniaCrowdfundingService.GetCampaniaStatsAsync(Guid campaniaId, CancellationToken ct)`: Obtener stats detalladas

**Validacion de Ownership:**
```csharp
// En Handlers de GetCampaniaBackingsQuery y GetCampaniaStatsQuery
var campania = await _campaniaService.GetByIdAsync(request.CampaniaId, cancellationToken);
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

**Caching de Artista:**
```csharp
// En Service de Artista (IArtistaService)
public async Task<Artista?> GetByUserIdAsync(string userId, CancellationToken ct)
{
    return await _requestCache.GetOrAddAsync(
        $"artista:user:{userId}",
        async () => await _repository.GetByUserIdAsync(userId, ct));
}
```

---

## 9. Checklist

- [x] Queries implementan IRequest<ServiceResponse<T>>
- [x] Handlers en mismo archivo que Query (REGLA CRITICA)
- [x] Validators con WithMessage + WithErrorCode usando ServiceResponseMessageType constants
- [x] Handlers inyectan Services (NO DbContext)
- [x] Handlers con ?? throw en constructores
- [x] Try-catch con logging en Handlers
- [x] Validacion retorna ServiceResponse (no throw)
- [x] DTOs documentados con calculos especificos
- [x] Validacion de ownership en queries de campania especifica
- [x] Reglas de privacidad para backings anonimos
- [x] Paginacion con limites (pageSize max 100)
- [x] Swagger documentation completa con ejemplos

---

## 10. Notas Adicionales

### 10.1 Construccion Manual de DTOs vs AutoMapper

Para los DTOs de dashboard, **se recomienda construccion manual** en lugar de AutoMapper porque:

1. **Calculos complejos**: Los DTOs requieren agregaciones, GROUP BY, JOINs complejos
2. **Proyecciones SQL**: Mejor rendimiento usando `Select()` directo en LINQ
3. **Logica condicional**: Reglas de privacidad (anonimos), calculos de porcentajes, etc.

**Ejemplo de construccion manual en Handler:**
```csharp
var misCampanias = await _context.CampaniaCrowdfunding
    .Where(c => c.ArtistaId == artistaId)
    .Where(c => !estadoCampaniaId.HasValue || c.EstadoCampaniaId == estadoCampaniaId.Value)
    .Select(c => new MiCampaniaListItemDto
    {
        Id = c.Id.Value,
        Titulo = c.Titulo,
        ImagenPrincipalUrl = c.ImagenPrincipalUrl,
        EstadoCampaniaId = c.EstadoCampaniaId,
        EstadoCampaniaNombre = c.EstadoCampaniaId == 1 ? "Borrador" : "...",
        ImporteObjetivo = c.ImporteObjetivo,
        ImporteRecaudado = c.ImportePledgedActual,
        PorcentajeProgreso = c.ImporteObjetivo > 0
            ? Math.Round((c.ImportePledgedActual / c.ImporteObjetivo) * 100, 2)
            : 0,
        NumBackers = c.Pedidos.Count(p => p.EstadoPedidoId == 3),
        DiasRestantes = c.FechaFin.HasValue
            ? Math.Max(0, (c.FechaFin.Value - DateTime.UtcNow).Days)
            : null,
        FechaFin = c.FechaFin,
        FechaCreacion = c.FechaCreacion
    })
    .OrderByDescending(c => c.FechaCreacion)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync(cancellationToken);
```

Esta aproximacion es mas eficiente que cargar entidades completas y mappearlas.

---

### 10.2 Estados de Campania (Referencia)

Los valores de `EstadoCampaniaId` segun `MaestraEstadoCampaniaCrowd`:

| ID | Nombre | Descripcion |
|----|--------|-------------|
| 1 | Borrador | Campania en preparacion, no visible publicamente |
| 2 | Publicada | Campania activa, aceptando backings |
| 3 | Finalizada | Campania completa (exitosa o no) |
| 4 | Cancelada | Campania cancelada por artista o admin |
| 5 | Pausada | Campania temporalmente pausada |

---

### 10.3 Estados de Pedido (Referencia)

Los valores de `EstadoPedidoId` segun `MaestraEstadoPedidoCrowd`:

| ID | Nombre | Descripcion |
|----|--------|-------------|
| 1 | Pendiente | Pedido creado, pago pendiente |
| 2 | Procesando | Pago en proceso de verificacion |
| 3 | Completado | Pago completado exitosamente |
| 4 | Fallido | Pago fallido o rechazado |
| 5 | Cancelado | Pedido cancelado |

**Solo contar pedidos con EstadoPedidoId = 3 (Completado) para metricas.**

---

### 10.4 Rendimiento y Optimizacion

**Indices recomendados:**
```sql
CREATE INDEX IX_CampaniaCrowdfunding_ArtistaId ON CampaniaCrowdfunding(ArtistaId);
CREATE INDEX IX_CampaniaCrowdfunding_EstadoCampaniaId ON CampaniaCrowdfunding(EstadoCampaniaId);
CREATE INDEX IX_PedidoCrowdfunding_CampaniaId_EstadoPedidoId ON PedidoCrowdfunding(CampaniaId, EstadoPedidoId);
CREATE INDEX IX_PedidoCrowdfunding_FechaCreacion ON PedidoCrowdfunding(FechaCreacion DESC);
```

**Caching:**
- Usar `IRequestCacheService` para cachear Artista por UserId (ADR-006)
- El cache es per-request, no distribuido (suficiente para MVP)

**Paginacion:**
- Limitar `pageSize` a 100 maximo (validado en validators)
- Siempre usar `Skip().Take()` para paginacion eficiente

---

**Fin del Plan de Contratos API**
