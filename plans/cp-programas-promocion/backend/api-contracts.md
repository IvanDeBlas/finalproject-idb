# Contratos API: cp-programas-promocion

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-programas-promocion (US-CP-02)
**Basado en:** docs/user-stories/cp-programas-promocion/contracts.md
**Patron de referencia:** PromotorController + CreatePromotorCommand (US-CP-01)

---

## 1. Endpoints

| Metodo | Ruta | Tipo | Handler | Descripcion |
|--------|------|------|---------|-------------|
| POST | /api/crowdpromotion/programas | Command | CreatePromoProgramaCommandHandler | Crear programa con tareas (transaccional) |
| GET | /api/crowdpromotion/programas/mis-programas | Query | GetMisProgramasQueryHandler | Listar paginado del artista autenticado |
| GET | /api/crowdpromotion/programas/{id} | Query | GetPromoProgramaByIdQueryHandler | Detalle completo con tareas, promotores y resumen |
| PUT | /api/crowdpromotion/programas/{id} | Command | UpdatePromoProgramaCommandHandler | Actualizar programa y tareas |
| PATCH | /api/crowdpromotion/programas/{id}/desactivar | Command | DesactivarPromoProgramaCommandHandler | Desactivar programa + tareas en cascada |

---

## 2. Request DTOs (Controller Layer)

Los DTOs de request son clases simples usadas en los controller actions para recibir el body.
El controller mapea estos DTOs a Commands antes de enviarlos a MediatR.

### 2.1 CreatePromoProgramaRequestDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromoProgramaRequestDto.cs`

| Propiedad | Tipo C# | JSON | Requerido | Notas |
|-----------|---------|------|-----------|-------|
| Titulo | string | titulo | Si | Min 5, max 200 chars |
| Descripcion | string? | descripcion | No | Max 4000 chars |
| TipoPromoId | int | tipoPromoId | Si | FK a Maestra_TipoPromo (1-4) |
| CampaniaCrowdfundingId | Guid? | campaniaCrowdfundingId | No | FK; debe pertenecer al artista |
| ProyectoArtisticoId | Guid? | proyectoArtisticoId | No | FK; debe pertenecer al artista |
| UrlLanding | string? | urlLanding | No | Formato URL, max 500 chars |
| CodigoTrackingBase | string? | codigoTrackingBase | No | Solo `[a-zA-Z0-9-]`, max 50 chars, unico por artista |
| MonedaId | int | monedaId | Si | FK a Maestra_Moneda (1=EUR, 2=USD) |
| ImporteComisionPorcentaje | decimal? | importeComisionPorcentaje | Condicional | Rango 0-100, 2 decimales; al menos una comision requerida |
| ImporteComisionFija | decimal? | importeComisionFija | Condicional | >= 0, 2 decimales; al menos una comision requerida |
| FechaInicio | DateOnly? | fechaInicio | No | Formato YYYY-MM-DD |
| FechaFin | DateOnly? | fechaFin | No | > FechaInicio cuando ambas presentes |
| Tareas | List\<CreatePromoTareaItemDto\> | tareas | No | Array vacio es valido (FA-02) |

### 2.2 CreatePromoTareaItemDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromoTareaItemDto.cs`

| Propiedad | Tipo C# | JSON | Requerido | Notas |
|-----------|---------|------|-----------|-------|
| Titulo | string | titulo | Si | Min 3, max 200 chars |
| Descripcion | string? | descripcion | No | Max 4000 chars |
| TipoEventoPromoId | int | tipoEventoPromoId | Si | FK a Maestra_TipoEventoPromo (1-6) |
| TipoRewardId | int | tipoRewardId | Si | FK a Maestra_TipoReward (1=Dinero, 2=Puntos, 3=Mixto) |
| ImporteRecompensa | decimal? | importeRecompensa | Condicional | >= 0; requerido si TipoRewardId es 1 o 3 |
| MonedaId | int? | monedaId | Condicional | Requerido si ImporteRecompensa tiene valor |
| PuntosRecompensa | int? | puntosRecompensa | Condicional | >= 0; requerido si TipoRewardId es 2 o 3 |
| UrlInstrucciones | string? | urlInstrucciones | No | Formato URL, max 500 chars |
| EsRepetible | bool | esRepetible | Si | Default true |
| MaxRepeticiones | int? | maxRepeticiones | Condicional | >= 1; requerido si EsRepetible == true |
| FechaInicio | DateOnly? | fechaInicio | No | Formato YYYY-MM-DD |
| FechaFin | DateOnly? | fechaFin | No | Formato YYYY-MM-DD |

### 2.3 UpdatePromoProgramaRequestDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromoProgramaRequestDto.cs`

Mismas propiedades que `CreatePromoProgramaRequestDto` con la excepcion de que `Tareas` usa `List<UpdatePromoTareaItemDto>`.

| Propiedad | Tipo C# | JSON | Requerido | Notas |
|-----------|---------|------|-----------|-------|
| Titulo | string | titulo | Si | |
| Descripcion | string? | descripcion | No | |
| TipoPromoId | int | tipoPromoId | Si | |
| CampaniaCrowdfundingId | Guid? | campaniaCrowdfundingId | No | |
| ProyectoArtisticoId | Guid? | proyectoArtisticoId | No | |
| UrlLanding | string? | urlLanding | No | |
| CodigoTrackingBase | string? | codigoTrackingBase | No | Unicidad se valida contra otros programas del mismo artista, no el propio |
| MonedaId | int | monedaId | Si | |
| ImporteComisionPorcentaje | decimal? | importeComisionPorcentaje | Condicional | |
| ImporteComisionFija | decimal? | importeComisionFija | Condicional | |
| FechaInicio | DateOnly? | fechaInicio | No | |
| FechaFin | DateOnly? | fechaFin | No | |
| Tareas | List\<UpdatePromoTareaItemDto\> | tareas | No | |

### 2.4 UpdatePromoTareaItemDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromoTareaItemDto.cs`

Mismas propiedades que `CreatePromoTareaItemDto` mas:

| Propiedad | Tipo C# | JSON | Requerido | Notas |
|-----------|---------|------|-----------|-------|
| Id | Guid? | id | No | Presente = tarea existente; ausente = nueva tarea |
| Titulo | string | titulo | Si | |
| Descripcion | string? | descripcion | No | |
| TipoEventoPromoId | int | tipoEventoPromoId | Si | |
| TipoRewardId | int | tipoRewardId | Si | |
| ImporteRecompensa | decimal? | importeRecompensa | Condicional | |
| MonedaId | int? | monedaId | Condicional | |
| PuntosRecompensa | int? | puntosRecompensa | Condicional | |
| UrlInstrucciones | string? | urlInstrucciones | No | |
| EsRepetible | bool | esRepetible | Si | Default true |
| MaxRepeticiones | int? | maxRepeticiones | Condicional | |
| FechaInicio | DateOnly? | fechaInicio | No | |
| FechaFin | DateOnly? | fechaFin | No | |
| EsActivo | bool | esActivo | Si | Default true; false = desactivar tarea existente |

---

## 3. Commands y Queries (CQRS)

Cada Command/Query y su Handler van en el **mismo archivo**. El Controller mapea el RequestDto al Command.

### 3.1 CreatePromoProgramaCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Commands/CreatePromoProgramaCommand.cs`

**Implementa:** `IRequest<ServiceResponse<PromoProgramaCreatedResultDto>>`

| Propiedad | Tipo | Origen |
|-----------|------|--------|
| Titulo | string | Body |
| Descripcion | string? | Body |
| TipoPromoId | int | Body |
| CampaniaCrowdfundingId | Guid? | Body |
| ProyectoArtisticoId | Guid? | Body |
| UrlLanding | string? | Body |
| CodigoTrackingBase | string? | Body |
| MonedaId | int | Body |
| ImporteComisionPorcentaje | decimal? | Body |
| ImporteComisionFija | decimal? | Body |
| FechaInicio | DateOnly? | Body |
| FechaFin | DateOnly? | Body |
| Tareas | List\<CreatePromoTareaItem\> | Body |
| UserId | string? | JWT claim (asignado por controller) |

**Nota sobre `CreatePromoTareaItem` en el Command:** Es un record/clase interna definida en el mismo archivo del command. No es el DTO del controller; contiene las mismas propiedades que `CreatePromoTareaItemDto` salvo que con tipos del dominio del command.

**Handler: CreatePromoProgramaCommandHandler**

Dependencias a inyectar:
```
IPromoProgramaService _promoProgramaService
IMapper _mapper
IValidator<CreatePromoProgramaCommand> _validator
ILogger<CreatePromoProgramaCommandHandler> _logger
```

Flujo del handler:
1. `_validator.ValidateAsync(request, ct)` → retornar ServiceResponse con errores si falla
2. Verificar `UserId` no nulo → retornar 401 si es nulo
3. `_promoProgramaService.GetArtistaByUserIdAsync(request.UserId, ct)` → retornar 404 `NotFound_Artista` si null
4. Si `CampaniaCrowdfundingId` presente: `_promoProgramaService.VerificarCampaniaPertenece(artistaId, campaniaCrowdfundingId, ct)` → retornar 403 `Auth_Forbidden` si no pertenece
5. Si `ProyectoArtisticoId` presente: verificar pertenencia similar
6. `_mapper.Map<PromoPrograma>(request)` + asignar `ArtistaId`, `EsActivo = true`, `FechaCreacion = DateTime.UtcNow`
7. `_mapper.Map<List<PromoTarea>>(request.Tareas)` + asignar `ProgramaId`, `EsActivo = true`, `Orden = index+1`, `FechaCreacion`
8. `_promoProgramaService.CreateWithTareasAsync(programa, tareas, ct)` → operacion transaccional
9. Retornar `ServiceResponse<PromoProgramaCreatedResultDto>` con `ServiceResponseMessageType.Created`

### 3.2 GetMisProgramasQuery

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Queries/GetMisProgramasQuery.cs`

**Implementa:** `IRequest<ServiceResponse<PromoProgramaListResultDto>>`

| Propiedad | Tipo | Origen | Notas |
|-----------|------|--------|-------|
| UserId | string | JWT claim | Asignado por controller |
| EsActivo | bool? | Query param | null = todos, true = activos, false = inactivos |
| Page | int | Query param | Default 1; >= 1 |
| PageSize | int | Query param | Default 10; max 50 |

**Handler: GetMisProgramasQueryHandler**

Dependencias:
```
IPromoProgramaService _promoProgramaService
IMapper _mapper
ILogger<GetMisProgramasQueryHandler> _logger
```

Flujo:
1. `_promoProgramaService.GetArtistaByUserIdAsync(UserId, ct)` → 404 si null
2. `_promoProgramaService.GetMisProgramasAsync(artistaId, esActivo, page, pageSize, ct)` → retorna entidades con contadores
3. `_mapper.Map<List<PromoProgramaListItemDto>>(items)` + resolver nombres de maestras
4. Retornar `ServiceResponse<PromoProgramaListResultDto>`

### 3.3 GetPromoProgramaByIdQuery

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Queries/GetPromoProgramaByIdQuery.cs`

**Implementa:** `IRequest<ServiceResponse<PromoProgramaDetailDto>>`

| Propiedad | Tipo | Origen |
|-----------|------|--------|
| Id | Guid | Path param |
| UserId | string | JWT claim |

**Handler: GetPromoProgramaByIdQueryHandler**

Dependencias:
```
IPromoProgramaService _promoProgramaService
IMapper _mapper
ILogger<GetPromoProgramaByIdQueryHandler> _logger
```

Flujo:
1. Verificar artista por UserId → 404 `NotFound_Artista` si null
2. `_promoProgramaService.GetByIdWithDetailAsync(id, ct)` → 404 `NotFound_PromoPrograma` si null
3. Verificar `programa.ArtistaId == artista.Id` → 403 `Auth_Forbidden` si no coincide
4. Construir `PromoProgramaDetailDto` incluyendo tareas, promotores y resumen (calculados en service)
5. Retornar `ServiceResponse<PromoProgramaDetailDto>`

### 3.4 UpdatePromoProgramaCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Commands/UpdatePromoProgramaCommand.cs`

**Implementa:** `IRequest<ServiceResponse<PromoProgramaUpdatedResultDto>>`

| Propiedad | Tipo | Origen |
|-----------|------|--------|
| Id | Guid | Path param |
| Titulo | string | Body |
| Descripcion | string? | Body |
| TipoPromoId | int | Body |
| CampaniaCrowdfundingId | Guid? | Body |
| ProyectoArtisticoId | Guid? | Body |
| UrlLanding | string? | Body |
| CodigoTrackingBase | string? | Body |
| MonedaId | int | Body |
| ImporteComisionPorcentaje | decimal? | Body |
| ImporteComisionFija | decimal? | Body |
| FechaInicio | DateOnly? | Body |
| FechaFin | DateOnly? | Body |
| Tareas | List\<UpdatePromoTareaItem\> | Body |
| UserId | string? | JWT claim |

**Handler: UpdatePromoProgramaCommandHandler**

Dependencias:
```
IPromoProgramaService _promoProgramaService
IMapper _mapper
IValidator<UpdatePromoProgramaCommand> _validator
ILogger<UpdatePromoProgramaCommandHandler> _logger
```

Flujo:
1. Validacion con `_validator.ValidateAsync`
2. Verificar artista → 404 si null
3. `_promoProgramaService.GetByIdAsync(Id, ct)` → 404 `NotFound_PromoPrograma` si null
4. Verificar ownership `programa.ArtistaId == artista.Id` → 403 si no coincide
5. Verificar campana si se actualiza → 403 si no pertenece
6. Verificar unicidad de `CodigoTrackingBase` excluyendo el programa actual
7. Verificar tareas con `esActivo = false` que no tienen completados en `PromoTareaPromotor`
8. `_promoProgramaService.UpdateWithTareasAsync(programa, tareas, ct)` → transaccional
9. Retornar `ServiceResponse<PromoProgramaUpdatedResultDto>` con `ServiceResponseMessageType.Updated`

### 3.5 DesactivarPromoProgramaCommand

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Commands/DesactivarPromoProgramaCommand.cs`

**Implementa:** `IRequest<ServiceResponse<PromoProgramaDesactivadoResultDto>>`

| Propiedad | Tipo | Origen |
|-----------|------|--------|
| Id | Guid | Path param |
| UserId | string? | JWT claim |

**Handler: DesactivarPromoProgramaCommandHandler**

Dependencias:
```
IPromoProgramaService _promoProgramaService
ILogger<DesactivarPromoProgramaCommandHandler> _logger
```

**Nota:** Este handler no inyecta `IMapper` ni `IValidator` ya que no requiere mapping de entrada ni validacion de campos; solo verifica ownership y estado actual.

Flujo:
1. Verificar `UserId` no nulo → 401
2. Verificar artista → 404 `NotFound_Artista`
3. `_promoProgramaService.GetByIdAsync(Id, ct)` → 404 `NotFound_PromoPrograma`
4. Verificar ownership → 403
5. Verificar `programa.EsActivo == true` → 400 `BusinessRule_PromoProgramaAlreadyInactive` si ya esta inactivo
6. `_promoProgramaService.DesactivarWithTareasAsync(programaId, ct)` → retorna count de tareas desactivadas
7. Retornar `ServiceResponse<PromoProgramaDesactivadoResultDto>` con `ServiceResponseMessageType.Updated`

---

## 4. Response DTOs

### 4.1 PromoProgramaCreatedResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaCreatedResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador del programa creado |
| Titulo | string | Titulo del programa |
| TipoPromoNombre | string | Nombre resuelto desde Maestra_TipoPromo |
| EsActivo | bool | Siempre true en creacion |
| TareasCreadas | int | Numero de tareas creadas junto con el programa |
| FechaCreacion | DateTime | Timestamp UTC de creacion |

**Wrapped en:** `ServiceResponse<PromoProgramaCreatedResultDto>`
**HTTP Status:** 201 Created

### 4.2 PromoProgramaListItemDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaListItemDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador |
| Titulo | string | Titulo del programa |
| TipoPromoId | int | FK a Maestra_TipoPromo |
| TipoPromoNombre | string | Nombre resuelto |
| CampaniaTitulo | string? | Null si sin campana vinculada |
| EsActivo | bool | Estado del programa |
| ImporteComisionPorcentaje | decimal? | Null si no definida |
| ImporteComisionFija | decimal? | Null si no definida |
| MonedaNombre | string | Nombre de la moneda (ej: "EUR") |
| NumeroPromotores | int | COUNT PromoProgramaPromotor donde EsAprobado == true |
| NumeroTareas | int | COUNT PromoTarea donde EsActivo == true |
| FechaInicio | DateOnly? | Null si no definida |
| FechaFin | DateOnly? | Null si no definida |
| FechaCreacion | DateTime | Timestamp UTC |

### 4.3 PromoProgramaListResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaListResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Items | List\<PromoProgramaListItemDto\> | Items de la pagina actual |
| TotalCount | int | Total de registros sin paginar |
| Page | int | Pagina actual |
| PageSize | int | Elementos por pagina |

**Wrapped en:** `ServiceResponse<PromoProgramaListResultDto>`
**HTTP Status:** 200 OK

### 4.4 PromoProgramaDetailDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaDetailDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador |
| Titulo | string | Titulo del programa |
| Descripcion | string? | Descripcion |
| TipoPromoId | int | FK a Maestra_TipoPromo |
| TipoPromoNombre | string | Nombre resuelto |
| CampaniaCrowdfundingId | Guid? | FK opcional |
| CampaniaTitulo | string? | Titulo resuelto de la campana |
| ProyectoArtisticoId | Guid? | FK opcional |
| UrlLanding | string? | URL de landing |
| CodigoTrackingBase | string? | Codigo de tracking |
| MonedaId | int | FK a Maestra_Moneda |
| MonedaNombre | string | Nombre de la moneda |
| ImporteComisionPorcentaje | decimal? | |
| ImporteComisionFija | decimal? | |
| EsActivo | bool | Estado del programa |
| FechaInicio | DateOnly? | |
| FechaFin | DateOnly? | |
| FechaCreacion | DateTime | Timestamp UTC |
| FechaActualizacion | DateTime? | Null si nunca actualizado |
| Tareas | List\<PromoTareaDetailDto\> | Todas las tareas (activas e inactivas) |
| Promotores | List\<PromoProgramaPromotorSummaryDto\> | Promotores inscritos |
| Resumen | PromoProgramaResumenDto | Metricas calculadas |

**Wrapped en:** `ServiceResponse<PromoProgramaDetailDto>`
**HTTP Status:** 200 OK

### 4.5 PromoTareaDetailDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoTareaDetailDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador de la tarea |
| Titulo | string | Titulo |
| Descripcion | string? | Descripcion |
| TipoEventoPromoId | int | FK a Maestra_TipoEventoPromo |
| TipoEventoPromoNombre | string | Nombre resuelto (ej: "Share") |
| TipoRewardId | int | FK a Maestra_TipoReward |
| TipoRewardNombre | string | Nombre resuelto (ej: "Dinero") |
| ImporteRecompensa | decimal? | |
| MonedaId | int? | |
| MonedaNombre | string? | |
| PuntosRecompensa | int? | |
| UrlInstrucciones | string? | |
| EsRepetible | bool | |
| MaxRepeticiones | int? | |
| Orden | int | Orden de la tarea dentro del programa |
| EsActivo | bool | |
| FechaInicio | DateOnly? | |
| FechaFin | DateOnly? | |
| CompletadosPorPromotores | int | COUNT de PromoTareaPromotor donde TareaId == Id |

### 4.6 PromoProgramaPromotorSummaryDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaPromotorSummaryDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Id de la relacion PromoProgramaPromotor |
| PromotorNombre | string | NombrePublico del promotor |
| TipoPromotorNombre | string | Resuelto desde Maestra_TipoPromotor |
| EsAprobado | bool | Estado de aprobacion |
| EsBloqueado | bool | Estado de bloqueo |
| FechaAlta | DateTime | FechaInscripcion del promotor al programa |

### 4.7 PromoProgramaResumenDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaResumenDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| TotalPromotoresAprobados | int | COUNT PromoProgramaPromotor donde EsAprobado == true |
| TotalPromotoresPendientes | int | COUNT donde EsAprobado == false y EsBloqueado == false |
| TotalEventos | int | COUNT total de PromoEvento del programa |
| TotalConversiones | int | COUNT PromoEvento donde TipoEventoId == 6 (Backing) |
| ValorTotalGenerado | decimal | SUM importes de conversiones; 0 si no hay |

### 4.8 PromoProgramaUpdatedResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaUpdatedResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador del programa |
| Titulo | string | Titulo actualizado |
| FechaActualizacion | DateTime | Timestamp UTC de la actualizacion |

**Wrapped en:** `ServiceResponse<PromoProgramaUpdatedResultDto>`
**HTTP Status:** 200 OK

### 4.9 PromoProgramaDesactivadoResultDto

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaDesactivadoResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| Id | Guid | Identificador del programa |
| EsActivo | bool | Siempre false |
| TareasDesactivadas | int | Numero de PromoTarea desactivadas en cascada |

**Wrapped en:** `ServiceResponse<PromoProgramaDesactivadoResultDto>`
**HTTP Status:** 200 OK

---

## 5. Nuevos Error Codes en ServiceResponseMessageType

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Agregar las siguientes constantes al archivo existente:

### 5.1 Validation (1000-1999) - Nuevas constantes

| Constante | Valor | Uso |
|-----------|-------|-----|
| `Validation_AtLeastOneComision` | `"1020"` | ImporteComisionPorcentaje y ImporteComisionFija ambos nulos |
| `Validation_RangeOutOfBounds` | `"1021"` | ImporteComisionPorcentaje fuera de rango 0-100, o ImporteComisionFija < 0 |
| `Validation_DateFinBeforeInicio` | `"1022"` | FechaFin <= FechaInicio |
| `Validation_InvalidCodigoTracking` | `"1023"` | CodigoTrackingBase con caracteres no permitidos |
| `Validation_DuplicateCodigoTracking` | `"1024"` | CodigoTrackingBase ya usado por el mismo artista |
| `Validation_EsRepetibleRequiresMax` | `"1025"` | EsRepetible == true sin MaxRepeticiones >= 1 |
| `Validation_ImporteRecompensaRequired` | `"1026"` | TipoRewardId es Dinero/Mixto sin ImporteRecompensa |
| `Validation_PuntosRecompensaRequired` | `"1027"` | TipoRewardId es Puntos/Mixto sin PuntosRecompensa |
| `Validation_TareaConCompletados` | `"1029"` | Tarea enviada con EsActivo:false tiene registros en PromoTareaPromotor |

**Nota:** El codigo `"1028"` del contracts.md corresponde a `Validation_PuntosRecompensaRequired`. El shared contracts-plan.md usa `"1027"` para el mismo concepto. El contracts.md define los codigos de forma definitiva; usar los codigos exactos del contracts.md (`1027` para importe, `1028` para puntos).

Revision de codigos segun contracts.md:
- `"1027"` = `Validation_ImporteRecompensaRequired` (importe recompensa para monetarias)
- `"1028"` = `Validation_PuntosRecompensaRequired` (puntos para recompensas de puntos)
- `"1029"` = `Validation_TareaConCompletados` (tarea con completados no se puede desactivar)

### 5.2 NotFound (2000-2999) - Nuevas constantes

| Constante | Valor | Uso |
|-----------|-------|-----|
| `NotFound_Artista` | `"2016"` | No existe Artista con el UserId del token |
| `NotFound_CampaniaCrowdfunding` | `"2017"` | CampaniaCrowdfundingId no existe en BD |
| `NotFound_ProyectoArtistico` | `"2018"` | ProyectoArtisticoId no existe en BD |
| `NotFound_PromoPrograma` | `"2019"` | PromoPrograma no encontrado por Id |

### 5.3 Business Rules (4000-4999) - Nuevas constantes

| Constante | Valor | Uso |
|-----------|-------|-----|
| `BusinessRule_PromoProgramaAlreadyInactive` | `"4020"` | Programa ya desactivado al intentar desactivar de nuevo |

### 5.4 Estado final del archivo

```
// Success (0000-0999)
Success = "0000"
Created = "0001"
Updated = "0002"
Deleted = "0003"

// Validation (1000-1999)
Validation_Required = "1001"            <- existente
Validation_MaxLength = "1002"           <- existente
Validation_InvalidEmail = "1003"        <- existente
Validation_ForeignKeyNotFound = "1010"  <- existente
Validation_MinLength = "1011"           <- existente
Validation_InvalidUrl = "1013"          <- existente
Validation_AtLeastOneComision = "1020"  <- NUEVO
Validation_RangeOutOfBounds = "1021"    <- NUEVO
Validation_DateFinBeforeInicio = "1022" <- NUEVO
Validation_InvalidCodigoTracking = "1023" <- NUEVO
Validation_DuplicateCodigoTracking = "1024" <- NUEVO
Validation_EsRepetibleRequiresMax = "1025" <- NUEVO
Validation_ImporteRecompensaRequired = "1027" <- NUEVO (mapeado al codigo 1027 del contracts.md)
Validation_PuntosRecompensaRequired = "1028"  <- NUEVO (mapeado al codigo 1028 del contracts.md)
Validation_TareaConCompletados = "1029"       <- NUEVO (codigo 1029 del contracts.md)

// NotFound (2000-2999)
NotFound_Entity = "2000"                <- existente
NotFound_Promotor = "2015"              <- existente
NotFound_Artista = "2016"               <- NUEVO
NotFound_CampaniaCrowdfunding = "2017"  <- NUEVO
NotFound_ProyectoArtistico = "2018"     <- NUEVO
NotFound_PromoPrograma = "2019"         <- NUEVO

// Auth (3000-3999)
Auth_Unauthorized = "3001"              <- existente
Auth_Forbidden = "3002"                 <- existente
Auth_InvalidToken = "3004"              <- existente

// Business Rules (4000-4999)
BusinessRule_PromotorAlreadyExists = "4018"    <- existente
BusinessRule_PromotorAlreadyInactive = "4019"  <- existente
BusinessRule_PromoProgramaAlreadyInactive = "4020" <- NUEVO

// Internal (5000-5999)
Internal_UnexpectedError = "5000"       <- existente
```

---

## 6. Validadores

### 6.1 CreatePromoProgramaCommandValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Validators/CreatePromoProgramaCommandValidator.cs`

**Inyecta:** `IPromoProgramaService` (para verificar FK y unicidad de CodigoTrackingBase)

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Titulo | NotEmpty | El titulo es obligatorio | Validation_Required |
| Titulo | MinimumLength(5) | El titulo debe tener al menos 5 caracteres | Validation_MinLength |
| Titulo | MaximumLength(200) | El titulo no puede superar los 200 caracteres | Validation_MaxLength |
| TipoPromoId | GreaterThan(0) | El tipo de programa es obligatorio | Validation_Required |
| TipoPromoId | MustAsync TipoPromoExiste | El tipo de programa no existe | Validation_ForeignKeyNotFound |
| MonedaId | GreaterThan(0) | La moneda es obligatoria | Validation_Required |
| MonedaId | MustAsync MonedaExiste | La moneda no existe | Validation_ForeignKeyNotFound |
| (cross) | Must: al menos una comision | Debe definir al menos una comision (porcentaje o fija) | Validation_AtLeastOneComision |
| ImporteComisionPorcentaje | InclusiveBetween(0, 100) (cuando presente) | La comision porcentaje debe estar entre 0 y 100 | Validation_RangeOutOfBounds |
| ImporteComisionFija | GreaterThanOrEqualTo(0) (cuando presente) | La comision fija no puede ser negativa | Validation_RangeOutOfBounds |
| (cross) | Must: FechaFin > FechaInicio (cuando ambas presentes) | La fecha fin debe ser posterior a la fecha inicio | Validation_DateFinBeforeInicio |
| UrlLanding | Must BeValidUrl (cuando presente) | La URL de landing no tiene formato valido | Validation_InvalidUrl |
| UrlLanding | MaximumLength(500) (cuando presente) | La URL de landing no puede superar los 500 caracteres | Validation_MaxLength |
| CodigoTrackingBase | Matches `^[a-zA-Z0-9-]*$` (cuando presente) | El codigo de tracking solo puede contener letras, numeros y guiones | Validation_InvalidCodigoTracking |
| CodigoTrackingBase | MaximumLength(50) (cuando presente) | El codigo de tracking no puede superar los 50 caracteres | Validation_MaxLength |
| CodigoTrackingBase | MustAsync CodigoTrackingUnico (cuando presente) | El codigo de tracking ya existe para este artista | Validation_DuplicateCodigoTracking |
| UserId | NotEmpty | El UserId es obligatorio | Validation_Required |
| Tareas | ForEach TareaValidator (cuando la lista tiene items) | Ver sub-validator de tarea | Ver tabla siguiente |

**Sub-reglas de cada tarea en `Tareas`:**

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| Titulo | NotEmpty | El titulo de la tarea es obligatorio | Validation_Required |
| Titulo | MinimumLength(3) | El titulo de la tarea debe tener al menos 3 caracteres | Validation_MinLength |
| Titulo | MaximumLength(200) | El titulo de la tarea no puede superar los 200 caracteres | Validation_MaxLength |
| TipoEventoPromoId | GreaterThan(0) | El tipo de evento es obligatorio | Validation_Required |
| TipoEventoPromoId | MustAsync TipoEventoExiste | El tipo de evento no existe | Validation_ForeignKeyNotFound |
| TipoRewardId | GreaterThan(0) | El tipo de recompensa es obligatorio | Validation_Required |
| TipoRewardId | MustAsync TipoRewardExiste | El tipo de recompensa no existe | Validation_ForeignKeyNotFound |
| (cross) | EsRepetible == true => MaxRepeticiones != null && >= 1 | Las tareas repetibles requieren max repeticiones >= 1 | Validation_EsRepetibleRequiresMax |
| (cross) | TipoRewardId 1 o 3 => ImporteRecompensa != null | El importe de recompensa es requerido para recompensas monetarias | Validation_ImporteRecompensaRequired |
| (cross) | TipoRewardId 2 o 3 => PuntosRecompensa != null | Los puntos de recompensa son requeridos para recompensas de puntos | Validation_PuntosRecompensaRequired |
| UrlInstrucciones | Must BeValidUrl (cuando presente) | La URL de instrucciones no tiene formato valido | Validation_InvalidUrl |
| UrlInstrucciones | MaximumLength(500) (cuando presente) | La URL de instrucciones no puede superar los 500 caracteres | Validation_MaxLength |

**Nota de implementacion de las reglas cross-field:** Las validaciones que cruzan varios campos de la misma entidad (`Must` con lambda) requieren `When` para ejecutarse solo cuando los campos relevantes tienen valor. Ejemplo:

```csharp
// Regla cross-field: al menos una comision
RuleFor(x => x)
    .Must(x => x.ImporteComisionPorcentaje.HasValue || x.ImporteComisionFija.HasValue)
    .WithMessage("Debe definir al menos una comision (porcentaje o fija)")
    .WithErrorCode(ServiceResponseMessageType.Validation_AtLeastOneComision);

// Regla cross-field en tarea: esRepetible requiere maxRepeticiones
// Implementar como RuleForEach con un AbstractValidator<CreatePromoTareaItem> separado
// o como Must dentro del RuleForEach
```

### 6.2 UpdatePromoProgramaCommandValidator

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Features/PromoPrograma/Validators/UpdatePromoProgramaCommandValidator.cs`

**Inyecta:** `IPromoProgramaService`

Las mismas reglas que `CreatePromoProgramaCommandValidator` con la siguiente diferencia en CodigoTrackingBase:

| Campo | Regla especial update | Notas |
|-------|----------------------|-------|
| CodigoTrackingBase | MustAsync CodigoTrackingUnico **excluyendo el programa actual** (`command.Id`) | La unicidad se verifica contra otros programas del artista, no el propio |

Ademas, para cada tarea en `Tareas` (que son `UpdatePromoTareaItem`):

| Campo adicional | Regla | Mensaje | ErrorCode |
|----------------|-------|---------|-----------|
| EsActivo | (cuando EsActivo == false y Id presente) MustAsync TareaNoTieneCompletados | No se puede desactivar una tarea que ya tiene completados | Validation_TareaConCompletados |

---

## 7. AutoMapper Mappings

### 7.1 PromoProgramaProfile

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoProgramaProfile.cs`

| Source | Destination | Notas de mapping |
|--------|-------------|------------------|
| CreatePromoProgramaCommand | PromoPrograma | Ignorar: Id, ArtistaId, EsActivo, FechaCreacion, FechaActualizacion, Promotores, Tareas, Eventos. Los campos nuevos del dominio (UrlLanding, CodigoTrackingBase, ImporteComisionPorcentaje, ImporteComisionFija) mapean 1:1 |
| PromoPrograma | PromoProgramaCreatedResultDto | Id.Value -> Id (Guid); TipoPromoNombre e TareasCreadas se asignan manualmente en el handler (no disponibles en la entidad) |
| PromoPrograma | PromoProgramaListItemDto | Id.Value -> Id; CampaniaTitulo, TipoPromoNombre, MonedaNombre, NumeroPromotores, NumeroTareas se resuelven fuera del AutoMapper (en service o handler) |
| PromoPrograma | PromoProgramaDetailDto | Id.Value -> Id; campos calculados se asignan en handler; CampaniaTitulo y TipoPromoNombre se asignan manualmente |
| PromoPrograma | PromoProgramaUpdatedResultDto | Id.Value -> Id; FechaActualizacion mapea con `.MapFrom(src => src.FechaActualizacion ?? DateTime.UtcNow)` |
| PromoPrograma | PromoProgramaDesactivadoResultDto | Id.Value -> Id; EsActivo mapea 1:1; TareasDesactivadas se asigna manualmente en handler |

### 7.2 PromoTareaProfile

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Mapping/PromoTareaProfile.cs`

**Nota:** Puede incluirse en el mismo archivo `PromoProgramaProfile.cs` o en archivo separado. Por consistencia con el patron existente (un Profile por entidad), crear archivo separado.

| Source | Destination | Notas de mapping |
|--------|-------------|------------------|
| CreatePromoTareaItem (Command inner class) | PromoTarea | Ignorar: Id, ProgramaId, Orden, EsActivo, FechaCreacion, Programa, TareasPromotor. UrlInstrucciones <- request.UrlInstrucciones (campo existe como `InstruccionesUrl` en la entidad - ver nota) |
| PromoTarea | PromoTareaDetailDto | TipoEventoPromoNombre, TipoRewardNombre, MonedaNombre se asignan manualmente; CompletadosPorPromotores se asigna en service |

**Nota critica de naming:** La entidad `PromoTarea` tiene el campo `InstruccionesUrl` (ver definicion actual), mientras que el DTO usa `UrlInstrucciones`. El mapping debe usar `.ForMember(dest => dest.InstruccionesUrl, opt => opt.MapFrom(src => src.UrlInstrucciones))` al mapear command -> entidad, y `.ForMember(dest => dest.UrlInstrucciones, opt => opt.MapFrom(src => src.InstruccionesUrl))` al mapear entidad -> DTO.

Ademas, la entidad `PromoTarea` no tiene actualmente los campos `TipoEventoPromoId`, `EsRepetible`, `MaxRepeticiones`, `FechaInicio`, `FechaFin`. Estos son los **campos nuevos a agregar via migracion** (ver feature-spec.md Notas Tecnicas). El mapping asume que estos campos existiran en la entidad tras la migracion.

---

## 8. Contrato del Controller

### 8.1 PromoProgramaController

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.WebApi/Controllers/PromoProgramaController.cs`

```
[ApiController]
[Route("api/crowdpromotion/[controller]")]
public class PromoProgramaController : BaseLoggerController
```

**Inyecta:** `IMediator _mediator`, `ICurrentUserService _currentUser`

#### Action: POST /api/crowdpromotion/programas

```
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 201)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 400)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 401)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 403)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 404)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), 500)]
public async Task<IActionResult> Create([FromBody] CreatePromoProgramaRequestDto request, CancellationToken ct)
```

Patron de mapeo en el controller (igual que PromotorController):
1. Extraer `userId = _currentUser.UserId` → Unauthorized si null
2. Construir `CreatePromoProgramaCommand` desde `request` + asignar `UserId`
3. `var result = await _mediator.Send(command, ct)`
4. Si `result.IsSuccess` → `CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)`
5. Caso contrario → `FromServiceResponse(result)`

#### Action: GET /api/crowdpromotion/programas/mis-programas

```
[HttpGet("mis-programas")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), 200)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), 401)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), 404)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), 500)]
public async Task<IActionResult> GetMisProgramas(
    [FromQuery] bool? esActivo,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
```

Patron:
1. Extraer `userId` → Unauthorized si null
2. Clamp `pageSize = Math.Min(Math.Max(pageSize, 1), 50)` y `page = Math.Max(page, 1)`
3. Construir `GetMisProgramasQuery` con `UserId`, `EsActivo`, `Page`, `PageSize`
4. `return FromServiceResponse(await _mediator.Send(query, ct))`

**Nota importante de routing:** La ruta `mis-programas` debe declararse ANTES del route `{id}` para que ASP.NET Core no interprete "mis-programas" como un Guid. Esto se logra con el orden de declaracion de los metodos en el controller.

#### Action: GET /api/crowdpromotion/programas/{id}

```
[HttpGet("{id:guid}")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), 200)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), 401)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), 403)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), 404)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), 500)]
public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
```

Patron:
1. Extraer `userId` → Unauthorized si null
2. Construir `GetPromoProgramaByIdQuery { Id = id, UserId = userId.Value.ToString() }`
3. `return FromServiceResponse(await _mediator.Send(query, ct))`

#### Action: PUT /api/crowdpromotion/programas/{id}

```
[HttpPut("{id:guid}")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 200)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 400)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 401)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 403)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 404)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), 500)]
public async Task<IActionResult> Update(
    [FromRoute] Guid id,
    [FromBody] UpdatePromoProgramaRequestDto request,
    CancellationToken ct)
```

Patron:
1. Extraer `userId` → Unauthorized si null
2. Construir `UpdatePromoProgramaCommand` desde `request` + `Id = id` + `UserId`
3. `return FromServiceResponse(await _mediator.Send(command, ct))`

#### Action: PATCH /api/crowdpromotion/programas/{id}/desactivar

```
[HttpPatch("{id:guid}/desactivar")]
[Authorize]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 200)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 400)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 401)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 403)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 404)]
[ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), 500)]
public async Task<IActionResult> Desactivar([FromRoute] Guid id, CancellationToken ct)
```

Patron:
1. Extraer `userId` → Unauthorized si null
2. Construir `DesactivarPromoProgramaCommand { Id = id, UserId = userId.Value.ToString() }`
3. `return FromServiceResponse(await _mediator.Send(command, ct))`

---

## 9. IPromoProgramaService

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromoProgramaService.cs`

Metodos requeridos por los handlers:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetArtistaByUserIdAsync(string userId, CancellationToken ct)` | `Artista?` | Resuelve el artista por UserId del JWT. Devuelve null si no existe |
| `GetByIdAsync(Guid id, CancellationToken ct)` | `PromoPrograma?` | Obtiene el programa sin includes. Devuelve null si no existe |
| `GetByIdWithDetailAsync(Guid id, CancellationToken ct)` | `PromoPrograma?` | Obtiene el programa con Tareas, Promotores y Eventos cargados |
| `GetMisProgramasAsync(ArtistaId artistaId, bool? esActivo, int page, int pageSize, CancellationToken ct)` | `(List<PromoPrograma>, int totalCount)` | Lista paginada con contadores calculados |
| `CodigoTrackingExistsAsync(ArtistaId artistaId, string codigo, Guid? excludeProgramaId, CancellationToken ct)` | `bool` | Unicidad de CodigoTrackingBase por artista, excluyendo el programa actual en updates |
| `TipoPromoExistsAsync(int tipoPromoId, CancellationToken ct)` | `bool` | Verifica existencia en Maestra_TipoPromo |
| `TipoEventoPromoExistsAsync(int tipoEventoPromoId, CancellationToken ct)` | `bool` | Verifica existencia en Maestra_TipoEventoPromo |
| `TipoRewardExistsAsync(int tipoRewardId, CancellationToken ct)` | `bool` | Verifica existencia en Maestra_TipoReward |
| `MonedaExistsAsync(int monedaId, CancellationToken ct)` | `bool` | Verifica existencia en Maestra_Moneda |
| `VerificarCampaniaPertenece(ArtistaId artistaId, Guid campaniaCrowdfundingId, CancellationToken ct)` | `bool` | Verifica que la campana pertenece al artista |
| `VerificarProyectoPertenece(ArtistaId artistaId, Guid proyectoArtisticoId, CancellationToken ct)` | `bool` | Verifica que el proyecto pertenece al artista |
| `TareaConCompletadosAsync(Guid tareaId, CancellationToken ct)` | `bool` | Verifica si la tarea tiene registros en PromoTareaPromotor |
| `CreateWithTareasAsync(PromoPrograma programa, List<PromoTarea> tareas, CancellationToken ct)` | `PromoProgramaId` | Crea programa y tareas en una sola transaccion. Retorna el Id creado |
| `UpdateWithTareasAsync(PromoPrograma programa, List<PromoTarea> tareasNuevas, List<PromoTarea> tareasActualizar, List<Guid> tareasDesactivar, CancellationToken ct)` | `Task` | Actualiza en una sola transaccion |
| `DesactivarWithTareasAsync(PromoProgramaId programaId, CancellationToken ct)` | `int` | Desactiva programa y sus tareas activas. Retorna count de tareas desactivadas |
| `GetResumenAsync(PromoProgramaId programaId, CancellationToken ct)` | `PromoProgramaResumenDto` | Calcula metricas del programa |
| `GetNombreTipoPromo(int tipoPromoId)` | `string` | Resuelve nombre desde diccionario en memoria |
| `GetNombreTipoEventoPromo(int tipoEventoPromoId)` | `string` | Resuelve nombre desde diccionario en memoria |
| `GetNombreTipoReward(int tipoRewardId)` | `string` | Resuelve nombre desde diccionario en memoria |
| `GetNombreMoneda(int monedaId)` | `string` | Resuelve nombre desde diccionario en memoria |

**Nota sobre resolución de nombres de maestras:** Los handlers usan diccionarios en memoria para resolver nombres de TipoPromo, TipoEventoPromo, TipoReward y Moneda (patron identico al `TipoPromotorNombres` en `CreatePromotorCommandHandler`). Estos diccionarios pueden definirse como constantes estáticas en el servicio o en el handler. No se hacen llamadas a BD para resolver estos nombres en el MVP.

---

## 10. Campos Nuevos en Dominio (Requieren Migracion)

Antes de implementar los endpoints, la entidad `PromoPrograma` y `PromoTarea` requieren los siguientes campos nuevos.

### 10.1 PromoPrograma - Campos a agregar

| Campo | Tipo C# | Tipo SQL | Nullable | Default |
|-------|---------|----------|----------|---------|
| UrlLanding | string? | nvarchar(500) | Si | NULL |
| CodigoTrackingBase | string? | nvarchar(50) | Si | NULL |
| ImporteComisionPorcentaje | decimal? | decimal(5,2) | Si | NULL |
| ImporteComisionFija | decimal? | decimal(18,2) | Si | NULL |

**Indice recomendado para unicidad:** `IX_PromoPrograma_Artista_CodigoTracking` sobre `(ArtistaId, CodigoTrackingBase)` con filtro `WHERE CodigoTrackingBase IS NOT NULL`.

### 10.2 PromoTarea - Campos a agregar

| Campo | Tipo C# | Tipo SQL | Nullable | Default |
|-------|---------|----------|----------|---------|
| TipoEventoPromoId | int | int | No | 1 (para registros existentes) |
| EsRepetible | bool | bit | No | 1 (true para existentes) |
| MaxRepeticiones | int? | int | Si | NULL |
| FechaInicio | DateTime? | datetime2(3) | Si | NULL |
| FechaFin | DateTime? | datetime2(3) | Si | NULL |

**Nota sobre tipo de FechaInicio/FechaFin en PromoTarea:** La entidad `PromoPrograma` usa `DateTime?` para sus fechas. Para consistencia, `PromoTarea.FechaInicio` y `FechaFin` tambien usan `DateTime?` aunque el DTO los expone como `DateOnly?`. El mapping convierte `DateOnly` (request) a `DateTime` (entidad) y viceversa.

---

## 11. OpenAPI / Swagger Documentation

### POST /api/crowdpromotion/programas

- **Summary:** Crear programa de promocion
- **Description:** Crea un programa de promocion con sus tareas. Operacion transaccional. El ArtistaId se resuelve desde el JWT.
- **Tags:** PromoPrograma
- **Security:** Bearer JWT
- **Request Body:** `CreatePromoProgramaRequestDto` (application/json)
- **Responses:**
  - `201 Created`: `ServiceResponse<PromoProgramaCreatedResultDto>` - Programa creado con conteo de tareas
  - `400 Bad Request`: `ServiceResponse<PromoProgramaCreatedResultDto>` - Errores de validacion (titulos, comision requerida, fechas, tracking duplicado)
  - `401 Unauthorized`: Token JWT invalido o expirado
  - `403 Forbidden`: Campana o proyecto indicados no pertenecen al artista
  - `404 Not Found`: Perfil de artista no encontrado para el usuario del token
  - `500 Internal Server Error`: Error inesperado

### GET /api/crowdpromotion/programas/mis-programas

- **Summary:** Listar mis programas de promocion
- **Description:** Devuelve lista paginada de los programas del artista autenticado.
- **Tags:** PromoPrograma
- **Security:** Bearer JWT
- **Parameters:**
  - `esActivo` (query, bool, optional): Filtrar por estado
  - `page` (query, int, optional, default: 1): Numero de pagina
  - `pageSize` (query, int, optional, default: 10, max: 50): Items por pagina
- **Responses:**
  - `200 OK`: `ServiceResponse<PromoProgramaListResultDto>` - Lista paginada con contadores
  - `401 Unauthorized`: Token invalido
  - `404 Not Found`: Perfil de artista no encontrado
  - `500 Internal Server Error`: Error inesperado

### GET /api/crowdpromotion/programas/{id}

- **Summary:** Obtener detalle de programa
- **Description:** Retorna el detalle completo del programa incluyendo tareas, promotores inscritos y metricas. Solo el artista propietario puede acceder.
- **Tags:** PromoPrograma
- **Security:** Bearer JWT
- **Parameters:**
  - `id` (path, Guid, required): Identificador del programa
- **Responses:**
  - `200 OK`: `ServiceResponse<PromoProgramaDetailDto>` - Detalle completo
  - `401 Unauthorized`: Token invalido
  - `403 Forbidden`: El programa pertenece a otro artista
  - `404 Not Found`: Artista no encontrado (2016) o Programa no encontrado (2019)
  - `500 Internal Server Error`: Error inesperado

### PUT /api/crowdpromotion/programas/{id}

- **Summary:** Actualizar programa de promocion
- **Description:** Actualiza datos del programa y sus tareas. Items de tareas sin id crean nuevas tareas; con id actualizan las existentes.
- **Tags:** PromoPrograma
- **Security:** Bearer JWT
- **Parameters:**
  - `id` (path, Guid, required): Identificador del programa
- **Request Body:** `UpdatePromoProgramaRequestDto` (application/json)
- **Responses:**
  - `200 OK`: `ServiceResponse<PromoProgramaUpdatedResultDto>` - Confirmacion con timestamp
  - `400 Bad Request`: Errores de validacion
  - `401 Unauthorized`: Token invalido
  - `403 Forbidden`: Programa pertenece a otro artista o campana no pertenece al artista
  - `404 Not Found`: Artista o programa no encontrado
  - `500 Internal Server Error`: Error inesperado

### PATCH /api/crowdpromotion/programas/{id}/desactivar

- **Summary:** Desactivar programa de promocion
- **Description:** Desactiva logicamente el programa y todas sus tareas activas en cascada.
- **Tags:** PromoPrograma
- **Security:** Bearer JWT
- **Parameters:**
  - `id` (path, Guid, required): Identificador del programa
- **Request Body:** Ninguno
- **Responses:**
  - `200 OK`: `ServiceResponse<PromoProgramaDesactivadoResultDto>` - Confirmacion con count de tareas desactivadas
  - `400 Bad Request`: ErrorCode 4020 - El programa ya estaba desactivado
  - `401 Unauthorized`: Token invalido
  - `403 Forbidden`: Programa pertenece a otro artista
  - `404 Not Found`: Artista o programa no encontrado
  - `500 Internal Server Error`: Error inesperado

---

## 12. Estructura de Archivos a Crear

```
Modules/Crowdpromotion/
│
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs          <- MODIFICAR (agregar 13 constantes nuevas)
│   └── Model/
│       ├── PromoPrograma.cs                        <- MODIFICAR (agregar 4 campos nuevos)
│       └── PromoTarea.cs                           <- MODIFICAR (agregar 5 campos nuevos)
│
├── WePlayRises.Crowdpromotion.Application/
│   ├── Dtos/
│   │   ├── CreatePromoProgramaRequestDto.cs        <- CREAR
│   │   ├── CreatePromoTareaItemDto.cs              <- CREAR
│   │   ├── UpdatePromoProgramaRequestDto.cs        <- CREAR
│   │   ├── UpdatePromoTareaItemDto.cs              <- CREAR
│   │   ├── PromoProgramaCreatedResultDto.cs        <- CREAR
│   │   ├── PromoProgramaListItemDto.cs             <- CREAR
│   │   ├── PromoProgramaListResultDto.cs           <- CREAR
│   │   ├── PromoProgramaDetailDto.cs               <- CREAR
│   │   ├── PromoTareaDetailDto.cs                  <- CREAR
│   │   ├── PromoProgramaPromotorSummaryDto.cs      <- CREAR
│   │   ├── PromoProgramaResumenDto.cs              <- CREAR
│   │   ├── PromoProgramaUpdatedResultDto.cs        <- CREAR
│   │   └── PromoProgramaDesactivadoResultDto.cs    <- CREAR
│   ├── Features/PromoPrograma/
│   │   ├── Commands/
│   │   │   ├── CreatePromoProgramaCommand.cs       <- CREAR (Command + Handler en mismo archivo)
│   │   │   ├── UpdatePromoProgramaCommand.cs       <- CREAR (Command + Handler en mismo archivo)
│   │   │   └── DesactivarPromoProgramaCommand.cs   <- CREAR (Command + Handler en mismo archivo)
│   │   ├── Queries/
│   │   │   ├── GetMisProgramasQuery.cs             <- CREAR (Query + Handler en mismo archivo)
│   │   │   └── GetPromoProgramaByIdQuery.cs        <- CREAR (Query + Handler en mismo archivo)
│   │   └── Validators/
│   │       ├── CreatePromoProgramaCommandValidator.cs <- CREAR
│   │       └── UpdatePromoProgramaCommandValidator.cs <- CREAR
│   ├── Interfaces/Services/
│   │   └── IPromoProgramaService.cs                <- CREAR
│   └── Mapping/
│       ├── PromoProgramaProfile.cs                 <- CREAR
│       └── PromoTareaProfile.cs                    <- CREAR
│
├── WePlayRises.Crowdpromotion.Infra/
│   ├── Repositories/
│   │   └── PromoProgramaRepository.cs              <- CREAR (implementacion de IPromoProgramaRepository)
│   ├── Services/
│   │   └── PromoProgramaService.cs                 <- CREAR (implementacion de IPromoProgramaService)
│   ├── Context/
│   │   └── CrowdpromotionContext.cs                <- MODIFICAR (configuracion de campos nuevos)
│   └── DependencyInjection.cs                      <- MODIFICAR (registrar nuevos services y repos)
│
└── WePlayRises.Crowdpromotion.WebApi/
    └── Controllers/
        └── PromoProgramaController.cs              <- CREAR
```

---

## 13. Checklist de Contratos

- [ ] Commands implementan `IRequest<ServiceResponse<T>>`
- [ ] Queries implementan `IRequest<ServiceResponse<T>>`
- [ ] Handler + Command/Query en el mismo archivo (5 archivos: Create, Update, Desactivar, GetMis, GetById)
- [ ] Constructores con `?? throw new ArgumentNullException` para todas las dependencias
- [ ] Validators usan `ServiceResponseMessageType.X` constants (NO strings literales)
- [ ] Handlers usan `ServiceResponseMessageType.X` en respuestas de error y exito
- [ ] Logger inyectado y usado en try-catch en todos los handlers
- [ ] Ningún handler inyecta DbContext (todos usan `IPromoProgramaService`)
- [ ] Validators retornan `ServiceResponse` (no lanzan excepciones)
- [ ] Try-catch en todos los handlers con `_logger.LogError` y retorno de `Internal_UnexpectedError`
- [ ] `CodigoTrackingBase` unicidad verificada contra artista (no global)
- [ ] Creacion transaccional (programa + tareas en una sola unidad)
- [ ] Desactivacion en cascada (programa + todas sus PromoTarea activas)
- [ ] Route `mis-programas` declarada ANTES de `{id:guid}` en el controller
- [ ] DTOs de request usan `DateOnly?` para fechas (serializa como YYYY-MM-DD)
- [ ] DTOs de response usan `DateOnly?` para fechas de inicio/fin y `DateTime` para timestamps
- [ ] AutoMapper Profile separado por entidad (PromoProgramaProfile, PromoTareaProfile)
- [ ] Nuevas constantes `ServiceResponseMessageType` agregadas antes de implementar validators
- [ ] Migracion EF Core creada para campos nuevos en PromoPrograma y PromoTarea
- [ ] `IPromoProgramaService` registrado en `DependencyInjection.cs` del proyecto Infra

---

## 14. Notas de Implementacion

1. **Resolucion de ArtistaId desde JWT:** El patron es identico al usado para Promotor. El controller extrae `UserId` del claim y lo pasa al Command. El handler llama `IPromoProgramaService.GetArtistaByUserIdAsync` para resolver el `ArtistaId`. La entidad `Artista` vive en el modulo `UserAccess`; el servicio de Crowdpromotion hace una consulta cross-module al DbContext compartido o usa una interfaz de integración.

2. **Nombres de maestras hardcodeados en diccionario:** Al igual que `TipoPromotorNombres` en `CreatePromotorCommandHandler`, los nombres de `TipoPromo`, `TipoEventoPromo`, `TipoReward` y `Moneda` se resuelven con diccionarios estáticos en los handlers/service. No se hacen queries a BD para resolver estos nombres en el MVP.

3. **Transaccionalidad en CreateWithTareasAsync:** La implementacion en `PromoProgramaService` debe abrir una transaccion explicita, insertar el `PromoPrograma`, insertar cada `PromoTarea` con el `ProgramaId` recien creado, y hacer commit. Si cualquier insercion falla, se hace rollback completo.

4. **Logica de merge de tareas en Update:** La logica de "crear nuevas tareas (sin id), actualizar existentes (con id), desactivar (esActivo:false con id)" se implementa en `PromoProgramaService.UpdateWithTareasAsync`. El handler clasifica las tareas del command en tres listas antes de llamar al service.

5. **Campo `Orden` en PromoTarea:** En la creacion, el orden se asigna como el indice de la tarea en el array (1-based). En la actualizacion, el orden de las tareas existentes se preserva y las nuevas tareas reciben los siguientes valores de orden.

6. **`completadosPorPromotores` en PromoTareaDetailDto:** Este valor se calcula en la query del repositorio (COUNT de PromoTareaPromotor) para evitar N+1 queries. El repositorio usa una subquery o GROUP BY al cargar el detalle del programa.

7. **Campos de resumen calculados:** `PromoProgramaResumenDto` se llena en el handler llamando a `IPromoProgramaService.GetResumenAsync`. No es parte del AutoMapper mapping sino una asignacion manual en el handler tras obtener la entidad.

8. **Routing conflict prevention:** Declarar `[HttpGet("mis-programas")]` antes de `[HttpGet("{id:guid}")]`. El uso de `:guid` en la restriccion de ruta previene que "mis-programas" sea interpretado como un Guid, pero el orden de declaracion sigue siendo buena practica.
