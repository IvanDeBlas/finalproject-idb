# Plan CQRS: cs-acuerdos-entregables

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Bounded Context:** CrowdsourcingContext

---

## 1. Resumen de Operaciones

| Operacion | Tipo | Archivo | Request | Response |
|-----------|------|---------|---------|----------|
| Aceptar propuesta y crear acuerdo | Command | `AceptarPropuestaCommand.cs` | `AceptarPropuestaCommand` | `ServiceResponse<AceptarPropuestaResultDto>` |
| Rechazar propuesta individual | Command | `RechazarPropuestaCommand.cs` | `RechazarPropuestaCommand` | `ServiceResponse<RechazarPropuestaResultDto>` |
| Obtener detalle del acuerdo | Query | `GetAcuerdoByIdQuery.cs` | `GetAcuerdoByIdQuery` | `ServiceResponse<AcuerdoDto>` |
| Crear milestone | Command | `CreateMilestoneCommand.cs` | `CreateMilestoneCommand` | `ServiceResponse<MilestoneCreatedResultDto>` |
| Editar milestone | Command | `UpdateMilestoneCommand.cs` | `UpdateMilestoneCommand` | `ServiceResponse<MilestoneCreatedResultDto>` |
| Eliminar milestone | Command | `DeleteMilestoneCommand.cs` | `DeleteMilestoneCommand` | `ServiceResponse<bool>` |
| Subir entregable | Command | `CreateEntregableCommand.cs` | `CreateEntregableCommand` | `ServiceResponse<EntregableCreatedResultDto>` |
| Aprobar entregable | Command | `AprobarEntregableCommand.cs` | `AprobarEntregableCommand` | `ServiceResponse<AprobarEntregableResultDto>` |
| Rechazar entregable | Command | `RechazarEntregableCommand.cs` | `RechazarEntregableCommand` | `ServiceResponse<RechazarEntregableResultDto>` |
| Completar acuerdo | Command | `CompletarAcuerdoCommand.cs` | `CompletarAcuerdoCommand` | `ServiceResponse<CompletarAcuerdoResultDto>` |
| Cancelar acuerdo | Command | `CancelarAcuerdoCommand.cs` | `CancelarAcuerdoCommand` | `ServiceResponse<CancelarAcuerdoResultDto>` |

---

## 2. Commands

### 2.1 AceptarPropuestaCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/AceptarPropuestaCommand.cs`

**Contiene:** Command + Handler (mismo archivo, patron obligatorio del proyecto)

**Implementa:** `IRequest<ServiceResponse<AceptarPropuestaResultDto>>`

**HTTP:** `POST /api/crowdsourcing/propuestas/{id}/aceptar` -> `201 Created`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `PropuestaId` | `Guid` | Route param `{id}` | ID de la propuesta a aceptar |
| `UserId` | `string` | JWT claim `sub` | Resuelto por el controller |
| `TituloInterno` | `string` | Request body | Titulo del acuerdo. Max 200. |
| `FechaInicio` | `DateTime` | Request body | Fecha de inicio pactada. Default: hoy. |
| `FechaFinPrevista` | `DateTime?` | Request body | Fecha fin prevista. Opcional. Si presente: > FechaInicio. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IPropuestaCrowdsourcingService _propuestaService
INecesidadCrowdsourcingService _necesidadService
IArtistaService _artistaService
IValidator<AceptarPropuestaCommand> _validator
ILogger<AceptarPropuestaCommandHandler> _logger
```

Todas con `?? throw new ArgumentNullException(nameof(...))` en el constructor.

**Nota:** NO inyectar IMapper en este handler porque el DTO de respuesta se construye manualmente con los datos del resultado transaccional. El service devuelve el ID del acuerdo creado y la cuenta de propuestas rechazadas.

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Resolver ArtistaId por UserId
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista null -> retornar Forbidden (no es un artista del sistema)

3. Cargar propuesta
   var propuesta = await _propuestaService.GetByIdAsync(new PropuestaCrowdsourcingId(request.PropuestaId), ct)
   Si null -> retornar NotFound_Propuesta

4. Verificar que el artista es propietario de la necesidad
   var necesidad = await _necesidadService.GetByIdAsync(propuesta.NecesidadId, ct)
   Si necesidad.ArtistaId != artista.Id -> LogWarning + retornar Forbidden

5. Construir AcuerdoCrowdsourcing (LOGICA DE NEGOCIO EN EL HANDLER)
   entity.Id = AcuerdoCrowdsourcingId.CreateNew()
   entity.NecesidadId = propuesta.NecesidadId
   entity.PropuestaId = new PropuestaCrowdsourcingId(request.PropuestaId)
   entity.ArtistaId = artista.Id
   entity.UserIdProveedor = propuesta.UserId
   entity.PerfilProfesionalId = propuesta.PerfilProfesionalId
   entity.ImporteTotalPactado = propuesta.PrecioPropuesto
   entity.MonedaId = propuesta.MonedaId
   entity.EstadoAcuerdoId = EstadoAcuerdoConstants.Activo
   entity.TituloInterno = request.TituloInterno
   entity.FechaInicio = request.FechaInicio
   entity.FechaFinPrevista = request.FechaFinPrevista
   entity.FechaCreacion = DateTime.UtcNow
   entity.FechaActualizacion = null
   entity.MotivoCancelacion = null
   entity.CanceladoPor = null

6. Delegar operacion transaccional al service
   var (acuerdoId, propuestasRechazadas, conversacionId) =
       await _acuerdoService.AceptarPropuestaAsync(entity, propuesta, necesidad, ct)

7. Construir DTO de respuesta
   var resultDto = new AceptarPropuestaResultDto { ... }

8. Retornar ServiceResponse exitoso con Created (0001)
   Messages[0].ErrorCode = ServiceResponseMessageType.Created
   Messages[0].Message = "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."

9. Try-catch: LogError + InternalServerErrorServiceResponse
```

**Nota critica sobre la transaccion:** El handler NO maneja la transaccion directamente. La delega a `IAcuerdoCrowdsourcingService.AceptarPropuestaAsync()`, que internamente abre un `IDbContextTransaction` y ejecuta los 6 pasos atomicos. El handler solo construye el objeto `AcuerdoCrowdsourcing` y llama al service.

---

### 2.2 RechazarPropuestaCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/RechazarPropuestaCommand.cs`

**Implementa:** `IRequest<ServiceResponse<RechazarPropuestaResultDto>>`

**HTTP:** `PATCH /api/crowdsourcing/propuestas/{id}/rechazar` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `PropuestaId` | `Guid` | Route param `{id}` | ID de la propuesta a rechazar |
| `UserId` | `string` | JWT claim `sub` | Resuelto por el controller |
| `Motivo` | `string?` | Request body | Motivo de rechazo. Opcional. Max 500. NO se expone al profesional. |

#### Handler - Dependencias

```
IPropuestaCrowdsourcingService _propuestaService
INecesidadCrowdsourcingService _necesidadService
IArtistaService _artistaService
IValidator<RechazarPropuestaCommand> _validator
ILogger<RechazarPropuestaCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar propuesta
   var propuesta = await _propuestaService.GetByIdAsync(new PropuestaCrowdsourcingId(request.PropuestaId), ct)
   Si null -> retornar NotFound_Propuesta

3. Verificar que el usuario es el artista propietario de la necesidad
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   var necesidad = await _necesidadService.GetByIdAsync(propuesta.NecesidadId, ct)
   Si necesidad == null || necesidad.ArtistaId != artista?.Id -> LogWarning + Forbidden

4. Verificar que la propuesta esta en estado Pendiente
   Si propuesta.EstadoPropuestaId != EstadoPropuestaConstants.Pendiente
   -> retornar BusinessRule_PropuestaNotAcceptable

5. Llamar al service para rechazar
   await _propuestaService.RechazarAsync(new PropuestaCrowdsourcingId(request.PropuestaId), request.Motivo, ct)

6. Construir DTO de respuesta
   var resultDto = new RechazarPropuestaResultDto { Id = request.PropuestaId, EstadoPropuestaNombre = "Rechazada" }

7. Retornar ServiceResponse exitoso con Updated (0002)
   Messages[0].ErrorCode = ServiceResponseMessageType.Updated
   Messages[0].Message = "Propuesta rechazada"

8. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.3 CreateMilestoneCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CreateMilestoneCommand.cs`

**Implementa:** `IRequest<ServiceResponse<MilestoneCreatedResultDto>>`

**HTTP:** `POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones` -> `201 Created`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |
| `Titulo` | `string` | Request body | Titulo del milestone. Min 3, max 200. |
| `Descripcion` | `string?` | Request body | Descripcion opcional. Max 1000. |
| `ImporteParcial` | `decimal` | Request body | Importe parcial. > 0. |
| `FechaLimite` | `DateTime?` | Request body | Fecha limite opcional. >= FechaInicio del acuerdo. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
IArtistaService _artistaService
IValidator<CreateMilestoneCommand> _validator
ILogger<CreateMilestoneCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo
   var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> retornar NotFound_Acuerdo

3. Verificar que el usuario es el artista del acuerdo
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista == null || acuerdo.ArtistaId != artista.Id -> LogWarning + Forbidden

4. Verificar que el acuerdo esta Activo
   Si acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Construir entidad AcuerdoCrowdsourcingMilestone (LOGICA DE NEGOCIO EN EL HANDLER)
   entity.Id = Guid.NewGuid()
   entity.AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId)
   entity.Titulo = request.Titulo
   entity.Descripcion = request.Descripcion
   entity.ImporteParcial = request.ImporteParcial
   entity.FechaLimite = request.FechaLimite
   entity.FechaCompletado = null
   entity.FechaCreacion = DateTime.UtcNow
   entity.Orden sera asignado por el service (MAX + 1)

6. Persistir via service (el service asigna Orden y llama al repository)
   var id = await _milestoneService.CreateAsync(entity, ct)

7. Calcular campos del DTO de respuesta en el handler
   var importeAsignadoTotal = await _milestoneService.GetSumaImportesAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   var porcentajeParcial = acuerdo.ImporteTotalPactado > 0
       ? Math.Round((request.ImporteParcial / acuerdo.ImporteTotalPactado) * 100, 2)
       : 0

8. Construir y retornar MilestoneCreatedResultDto con Created (0001)

9. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.4 UpdateMilestoneCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/UpdateMilestoneCommand.cs`

**Implementa:** `IRequest<ServiceResponse<MilestoneCreatedResultDto>>`

**HTTP:** `PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `MilestoneId` | `Guid` | Route param `{id}` | ID del milestone a editar |
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista |
| `Titulo` | `string` | Request body | Nuevo titulo. Min 3, max 200. |
| `Descripcion` | `string?` | Request body | Nueva descripcion. Max 1000. |
| `ImporteParcial` | `decimal` | Request body | Nuevo importe. > 0. |
| `FechaLimite` | `DateTime?` | Request body | Nueva fecha limite. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
IArtistaService _artistaService
IValidator<UpdateMilestoneCommand> _validator
ILogger<UpdateMilestoneCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo y verificar existencia
   var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> NotFound_Acuerdo

3. Verificar que el usuario es el artista del acuerdo
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista == null || acuerdo.ArtistaId != artista.Id -> Forbidden

4. Verificar acuerdo Activo
   Si acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Cargar milestone
   var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId, ct)
   Si null || milestone.AcuerdoId != acuerdo.Id -> NotFound_Milestone

6. Verificar que el milestone no esta completado
   Si milestone.FechaCompletado != null -> BusinessRule_MilestoneCompleted

7. Aplicar cambios sobre la entidad cargada (LOGICA EN EL HANDLER)
   milestone.Titulo = request.Titulo
   milestone.Descripcion = request.Descripcion
   milestone.ImporteParcial = request.ImporteParcial
   milestone.FechaLimite = request.FechaLimite

8. Persistir via service
   await _milestoneService.UpdateAsync(milestone, ct)

9. Calcular campos del DTO de respuesta
   var importeAsignadoTotal = await _milestoneService.GetSumaImportesAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   var porcentajeParcial = calcular como en CreateMilestone

10. Retornar MilestoneCreatedResultDto con Updated (0002)

11. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.5 DeleteMilestoneCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/DeleteMilestoneCommand.cs`

**Implementa:** `IRequest<ServiceResponse<bool>>`

**HTTP:** `DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` -> `204 No Content`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `MilestoneId` | `Guid` | Route param `{id}` | ID del milestone a eliminar |
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
IArtistaService _artistaService
IValidator<DeleteMilestoneCommand> _validator
ILogger<DeleteMilestoneCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo
   Si null -> NotFound_Acuerdo

3. Verificar artista propietario
   Si no coincide -> Forbidden

4. Verificar acuerdo Activo
   Si no -> BusinessRule_AcuerdoNotActive

5. Cargar milestone
   var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId, ct)
   Si null || milestone.AcuerdoId != acuerdo.Id -> NotFound_Milestone

6. Verificar que el milestone no esta completado
   Si milestone.FechaCompletado != null -> BusinessRule_MilestoneCompleted

7. Verificar que el milestone no tiene entregables
   var tieneEntregables = await _milestoneService.TieneEntregablesAsync(request.MilestoneId, ct)
   Si tieneEntregables -> BusinessRule_MilestoneHasEntregables

8. Eliminar via service
   await _milestoneService.DeleteAsync(request.MilestoneId, ct)

9. Retornar ServiceResponse<bool> { Data = true } con Deleted (0003)

10. Try-catch: LogError + InternalServerErrorServiceResponse
```

**Nota:** El IAcuerdoCrowdsourcingMilestoneService necesita exponer `TieneEntregablesAsync(Guid milestoneId, CancellationToken ct)` adicionalmente a los metodos ya planificados en hexagonal-architecture.md.

---

### 2.6 CreateEntregableCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CreateEntregableCommand.cs`

**Implementa:** `IRequest<ServiceResponse<EntregableCreatedResultDto>>`

**HTTP:** `POST /api/crowdsourcing/acuerdos/{acuerdoId}/entregables` -> `201 Created`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el profesional del acuerdo |
| `Titulo` | `string` | Request body | Titulo del entregable. Min 3, max 200. |
| `Descripcion` | `string?` | Request body | Descripcion opcional. Max 1000. |
| `UrlRecurso` | `string?` | Request body | URL valida (Dropbox, Drive, etc.). Opcional. |
| `MilestoneId` | `Guid?` | Request body | Milestone asociado. Opcional. Debe pertenecer al acuerdo. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
IAcuerdoCrowdsourcingEntregableService _entregableService
IValidator<CreateEntregableCommand> _validator
ILogger<CreateEntregableCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo
   var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> NotFound_Acuerdo

3. Verificar que el usuario es el profesional del acuerdo
   Si acuerdo.UserIdProveedor != request.UserId -> LogWarning + Forbidden

4. Verificar que el acuerdo esta Activo
   Si acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Si se envia MilestoneId, verificar que pertenece al acuerdo
   Si request.MilestoneId.HasValue:
       var milestone = await _milestoneService.GetByIdAsync(request.MilestoneId.Value, ct)
       Si null || milestone.AcuerdoId != acuerdo.Id -> Validation_ForeignKeyNotFound

6. Construir entidad (LOGICA EN EL HANDLER)
   entity.Id = Guid.NewGuid()
   entity.AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId)
   entity.MilestoneId = request.MilestoneId
   entity.Titulo = request.Titulo
   entity.Descripcion = request.Descripcion
   entity.UrlRecurso = request.UrlRecurso
   entity.EstadoEntregableId = EstadoEntregableConstants.Entregado
   entity.ComentarioAprobacion = null
   entity.ComentarioRechazo = null
   entity.FechaAprobacion = null
   entity.FechaCreacion = DateTime.UtcNow
   entity.FechaActualizacion = null

7. Persistir via service
   var id = await _entregableService.CreateAsync(entity, ct)

8. Construir y retornar EntregableCreatedResultDto con Created (0001)
   Message = "Entregable subido correctamente. El artista sera notificado."

9. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.7 AprobarEntregableCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/AprobarEntregableCommand.cs`

**Implementa:** `IRequest<ServiceResponse<AprobarEntregableResultDto>>`

**HTTP:** `PATCH /api/crowdsourcing/entregables/{id}/aprobar` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `EntregableId` | `Guid` | Route param `{id}` | ID del entregable |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |
| `Comentario` | `string?` | Request body | Comentario opcional. Max 500 chars. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingEntregableService _entregableService
IArtistaService _artistaService
IValidator<AprobarEntregableCommand> _validator
ILogger<AprobarEntregableCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar entregable con su acuerdo incluido
   var entregable = await _entregableService.GetByIdWithAcuerdoAsync(request.EntregableId, ct)
   Si null -> NotFound_Entregable

3. Verificar que el usuario es el artista del acuerdo del entregable
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista == null || entregable.Acuerdo.ArtistaId != artista.Id -> LogWarning + Forbidden

4. Verificar que el acuerdo del entregable esta Activo
   Si entregable.Acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Verificar que el entregable esta en estado Entregado
   Si entregable.EstadoEntregableId != EstadoEntregableConstants.Entregado -> BusinessRule_EntregableNotReviewable

6. Aprobar via service (el service aplica el cambio de estado, FechaAprobacion y comentario)
   await _entregableService.AprobarAsync(request.EntregableId, request.Comentario, ct)

7. Calcular TodosAprobadosEnMilestone
   var todosAprobados = false
   Si entregable.MilestoneId.HasValue:
       todosAprobados = await _entregableService.TodosAprobadosEnMilestoneAsync(entregable.MilestoneId.Value, ct)

8. Construir y retornar AprobarEntregableResultDto con Updated (0002)
   resultDto.FechaAprobacion = DateTime.UtcNow
   resultDto.TodosAprobadosEnMilestone = todosAprobados

9. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.8 RechazarEntregableCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/RechazarEntregableCommand.cs`

**Implementa:** `IRequest<ServiceResponse<RechazarEntregableResultDto>>`

**HTTP:** `PATCH /api/crowdsourcing/entregables/{id}/rechazar` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `EntregableId` | `Guid` | Route param `{id}` | ID del entregable |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |
| `Comentario` | `string` | Request body | Comentario obligatorio. Min 10, max 500 chars. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingEntregableService _entregableService
IArtistaService _artistaService
IValidator<RechazarEntregableCommand> _validator
ILogger<RechazarEntregableCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar entregable con su acuerdo
   var entregable = await _entregableService.GetByIdWithAcuerdoAsync(request.EntregableId, ct)
   Si null -> NotFound_Entregable

3. Verificar artista del acuerdo
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista == null || entregable.Acuerdo.ArtistaId != artista.Id -> Forbidden

4. Verificar acuerdo Activo
   Si entregable.Acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Verificar que el entregable esta en estado Entregado
   Si entregable.EstadoEntregableId != EstadoEntregableConstants.Entregado -> BusinessRule_EntregableNotReviewable

6. Rechazar via service (el service aplica estado Rechazado y ComentarioRechazo)
   await _entregableService.RechazarAsync(request.EntregableId, request.Comentario, ct)

7. Construir y retornar RechazarEntregableResultDto con Updated (0002)
   Message = "Entregable rechazado. El profesional sera notificado."

8. Try-catch: LogError + InternalServerErrorServiceResponse
```

---

### 2.9 CompletarAcuerdoCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CompletarAcuerdoCommand.cs`

**Implementa:** `IRequest<ServiceResponse<CompletarAcuerdoResultDto>>`

**HTTP:** `PATCH /api/crowdsourcing/acuerdos/{id}/completar` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo a completar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IArtistaService _artistaService
IValidator<CompletarAcuerdoCommand> _validator
ILogger<CompletarAcuerdoCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo
   var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> NotFound_Acuerdo

3. Verificar que el usuario es el artista del acuerdo
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   Si artista == null || acuerdo.ArtistaId != artista.Id -> Forbidden

4. Verificar que el acuerdo esta Activo
   Si acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Completar via service (transaccional: actualiza acuerdo + necesidad)
   await _acuerdoService.CompletarAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)

6. Construir y retornar CompletarAcuerdoResultDto con Updated (0002)
   Message = "Acuerdo completado. Puedes dejar una valoracion al profesional."
   resultDto.FechaFinReal = DateTime.UtcNow (valor que el service persistio)

7. Try-catch: LogError + InternalServerErrorServiceResponse
```

**Nota sobre FechaFinReal:** El service establece y persiste `FechaFinReal = DateTime.UtcNow`. El handler puede retornar `DateTime.UtcNow` como aproximacion para el DTO, ya que la diferencia de milisegundos entre la ejecucion del service y la construccion del DTO es despreciable. Alternativa: el service retorna la entidad actualizada.

---

### 2.10 CancelarAcuerdoCommand

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CancelarAcuerdoCommand.cs`

**Implementa:** `IRequest<ServiceResponse<CancelarAcuerdoResultDto>>`

**HTTP:** `PATCH /api/crowdsourcing/acuerdos/{id}/cancelar` -> `200 OK`

#### Command - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo a cancelar |
| `UserId` | `string` | JWT claim `sub` | El que cancela (artista o profesional) |
| `Motivo` | `string` | Request body | Motivo obligatorio. Min 20, max 1000. |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IValidator<CancelarAcuerdoCommand> _validator
ILogger<CancelarAcuerdoCommandHandler> _logger
```

#### Handler - Flujo detallado

```
1. Validar con _validator.ValidateAsync(request, ct)
   Si invalido -> retornar ServiceResponse con errores

2. Cargar acuerdo
   var acuerdo = await _acuerdoService.GetByIdAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> NotFound_Acuerdo

3. Verificar que el usuario es participante del acuerdo (artista o profesional)
   NOTA: Para verificar el artista se necesita IArtistaService, pero para simplificar
   la autorizacion se puede verificar solo contra UserIdProveedor (string) para el profesional
   y resolver el ArtistaId para el artista.
   var esProveedor = acuerdo.UserIdProveedor == request.UserId
   Si no es proveedor, resolver artista:
       var artista = [via IArtistaService] (si fue inyectado)
       var esArtista = artista != null && acuerdo.ArtistaId == artista.Id
   Si !esProveedor && !esArtista -> LogWarning + Forbidden

   Alternativa simplificada (dependencia minima):
   Si el handler incluye IArtistaService entre sus dependencias, puede hacer la verificacion completa.

4. Verificar que el acuerdo esta Activo
   Si acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Activo -> BusinessRule_AcuerdoNotActive

5. Cancelar via service (transaccional: actualiza acuerdo + necesidad)
   await _acuerdoService.CancelarAsync(
       new AcuerdoCrowdsourcingId(request.AcuerdoId),
       request.Motivo,
       request.UserId,
       ct)

6. Construir y retornar CancelarAcuerdoResultDto con Updated (0002)
   resultDto.EstadoAcuerdoNombre = "Cancelado"
   resultDto.FechaFinReal = DateTime.UtcNow
   resultDto.NecesidadEstadoNombre = "Abierta"

7. Try-catch: LogError + InternalServerErrorServiceResponse
```

**Dependencias del handler CancelarAcuerdoCommandHandler:**

```
IAcuerdoCrowdsourcingService _acuerdoService
IArtistaService _artistaService
IValidator<CancelarAcuerdoCommand> _validator
ILogger<CancelarAcuerdoCommandHandler> _logger
```

---

## 3. Queries

### 3.1 GetAcuerdoByIdQuery

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Queries/GetAcuerdoByIdQuery.cs`

**Contiene:** Query + Handler (mismo archivo)

**Implementa:** `IRequest<ServiceResponse<AcuerdoDto>>`

**HTTP:** `GET /api/crowdsourcing/acuerdos/{id}` -> `200 OK`

#### Query - Propiedades

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo |
| `UserId` | `string` | JWT claim `sub` | Para calcular `MiRol` y verificar participacion |

#### Handler - Dependencias

```
IAcuerdoCrowdsourcingService _acuerdoService
IArtistaService _artistaService
IMapper _mapper
ILogger<GetAcuerdoByIdQueryHandler> _logger
```

**Nota:** No se inyecta IValidator en queries simples de lectura (sin reglas de negocio complejas en la query). La autorizacion se hace dentro del handler.

#### Handler - Flujo detallado

```
1. Cargar acuerdo con todos sus includes (Milestones -> Entregables, Necesidad)
   var acuerdo = await _acuerdoService.GetByIdWithDetailsAsync(new AcuerdoCrowdsourcingId(request.AcuerdoId), ct)
   Si null -> retornar NotFound_Acuerdo

2. Verificar que el usuario es participante (artista o profesional)
   var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct)
   var esArtista = artista != null && acuerdo.ArtistaId == artista.Id
   var esProfesional = acuerdo.UserIdProveedor == request.UserId
   Si !esArtista && !esProfesional -> LogWarning + Forbidden

3. Determinar MiRol (LOGICA DE NEGOCIO EN EL HANDLER)
   var miRol = esArtista ? "Artista" : "Profesional"

4. Calcular campos derivados (LOGICA EN EL HANDLER)
   var importeAsignado = acuerdo.Milestones.Sum(m => m.ImporteParcial)
   var porcentajeAsignado = acuerdo.ImporteTotalPactado > 0
       ? Math.Round((importeAsignado / acuerdo.ImporteTotalPactado) * 100, 2)
       : 0

5. Mapear entidad -> DTO base con AutoMapper
   var dto = _mapper.Map<AcuerdoDto>(acuerdo)

6. Enriquecer el DTO con campos calculados (no mapeables directamente)
   dto.MiRol = miRol
   dto.ImporteAsignado = importeAsignado
   dto.PorcentajeAsignado = porcentajeAsignado

7. Calcular PorcentajeParcial de cada milestone
   Para cada dto.Milestones:
       milestone.PorcentajeParcial = acuerdo.ImporteTotalPactado > 0
           ? Math.Round((milestone.ImporteParcial / acuerdo.ImporteTotalPactado) * 100, 2)
           : 0

8. Construir Timeline (los ultimos 20 eventos, de mas reciente a mas antiguo)
   Algoritmo del timeline (LOGICA EN EL HANDLER - sin tabla separada en MVP):
   var eventos = new List<AcuerdoTimelineEventoDto>()

   // Evento: Acuerdo creado
   eventos.Add(new { Accion = "Acuerdo creado", Fecha = acuerdo.FechaCreacion, Actor = artista?.NombreArtistico })

   // Eventos: Milestones creados
   Para cada milestone en acuerdo.Milestones (ordenados por FechaCreacion):
       eventos.Add(new { Accion = $"Milestone agregado: {milestone.Titulo}", Fecha = milestone.FechaCreacion, Actor = artista?.NombreArtistico })
       Si milestone.FechaCompletado != null:
           eventos.Add(new { Accion = $"Milestone completado: {milestone.Titulo}", Fecha = milestone.FechaCompletado.Value, Actor = artista?.NombreArtistico })

   // Eventos: Entregables
   Para cada entregable en todos los milestones + entregables sin milestone:
       eventos.Add(new { Accion = $"Entregable subido: {entregable.Titulo}", Fecha = entregable.FechaCreacion, Actor = "[nombre profesional - cargar del perfil]" })
       Si entregable.FechaAprobacion != null:
           eventos.Add(new { Accion = $"Entregable aprobado: {entregable.Titulo}", Fecha = entregable.FechaAprobacion.Value, Actor = artista?.NombreArtistico })
       Si entregable.FechaActualizacion != null && estado == Rechazado:
           eventos.Add(new { Accion = $"Entregable rechazado: {entregable.Titulo}", Fecha = entregable.FechaActualizacion.Value, Actor = artista?.NombreArtistico })

   // Evento: Completado o Cancelado
   Si acuerdo.FechaFinReal != null:
       var accion = acuerdo.EstadoAcuerdoId == EstadoAcuerdoConstants.Completado ? "Acuerdo completado" : "Acuerdo cancelado"
       eventos.Add(new { Accion = accion, Fecha = acuerdo.FechaFinReal.Value, Actor = "[segun quien cancelo/completo]" })

   dto.Timeline = eventos.OrderByDescending(e => e.Fecha).Take(20).ToList()

9. Retornar ServiceResponse<AcuerdoDto> exitoso (sin mensaje especial en queries)

10. Try-catch: LogError + InternalServerErrorServiceResponse
```

**Nota sobre el nombre del profesional en el timeline:** Para obtener el nombre del profesional se puede usar `IPerfilProfesionalService.GetByUserIdAsync()` o bien incluir la navegacion al perfil profesional en `GetByIdWithDetailsAsync`. Este detalle se define en la implementacion.

---

## 4. Validators

### 4.1 AceptarPropuestaCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/AceptarPropuestaCommandValidator.cs`

**Extiende:** `AbstractValidator<AceptarPropuestaCommand>`

**Dependencias del constructor:**

```
IPropuestaCrowdsourcingService _propuestaService
IAcuerdoCrowdsourcingService _acuerdoService
```

Todas con `?? throw new ArgumentNullException(nameof(...))`.

**Importacion critica:** `using WePlayRises.Crowdsourcing.Domain.Constants;`

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `TituloInterno` | `NotEmpty()` | "El titulo interno es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `TituloInterno` | `MaximumLength(200)` | "El titulo interno no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `FechaInicio` | `NotEmpty()` | "La fecha de inicio es obligatoria" | `ServiceResponseMessageType.Validation_Required` |
| `FechaFinPrevista` | Validacion condicional cuando presente: debe ser > FechaInicio | "La fecha de fin prevista debe ser posterior a la fecha de inicio" | `ServiceResponseMessageType.Validation_InvalidDate` |
| Command completo | `MustAsync`: Propuesta existe y esta en estado Pendiente | "La propuesta no existe o no esta en estado Pendiente" | `ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable` |
| Command completo | `MustAsync`: No existe acuerdo activo para la necesidad de la propuesta | "Ya existe un acuerdo activo para esta necesidad" | `ServiceResponseMessageType.BusinessRule_AcuerdoAlreadyExists` |

**Nota sobre MustAsync de FechaFinPrevista:**

```csharp
RuleFor(x => x)
    .Must(cmd => !cmd.FechaFinPrevista.HasValue || cmd.FechaFinPrevista.Value > cmd.FechaInicio)
    .When(x => x.FechaFinPrevista.HasValue)
    .WithMessage("La fecha de fin prevista debe ser posterior a la fecha de inicio")
    .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);
```

**Nota sobre la validacion de negocio con cache:** El validator usa `_propuestaService.GetByIdAsync()` y el handler tambien lo usa. Gracias a `IRequestCacheService` (integrado en el service), ambas llamadas retornan el mismo objeto en cache sin hacer dos queries a BD.

---

### 4.2 RechazarPropuestaCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/RechazarPropuestaCommandValidator.cs`

**Extiende:** `AbstractValidator<RechazarPropuestaCommand>`

**Dependencias del constructor:**

```
IPropuestaCrowdsourcingService _propuestaService
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `PropuestaId` | `NotEmpty()` | "El ID de la propuesta es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Motivo` | `MaximumLength(500)` (si presente) | "El motivo no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| Command completo | `MustAsync`: Propuesta existe | "La propuesta no fue encontrada" | `ServiceResponseMessageType.NotFound_Propuesta` |
| Command completo | `MustAsync`: Propuesta esta en estado Pendiente (Cache HIT del call anterior) | "Solo se pueden rechazar propuestas en estado Pendiente" | `ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable` |

---

### 4.3 CreateMilestoneCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CreateMilestoneCommandValidator.cs`

**Extiende:** `AbstractValidator<CreateMilestoneCommand>`

**Dependencias del constructor:**

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `Titulo` | `NotEmpty()` | "El titulo es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Titulo` | `MinimumLength(3)` | "El titulo debe tener al menos 3 caracteres" | `ServiceResponseMessageType.Validation_MinLength` |
| `Titulo` | `MaximumLength(200)` | "El titulo no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `Descripcion` | `MaximumLength(1000)` cuando presente | "La descripcion no puede superar los 1000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `ImporteParcial` | `GreaterThan(0)` | "El importe parcial debe ser mayor a 0" | `ServiceResponseMessageType.Validation_Required` |
| `FechaLimite` | `MustAsync`: >= FechaInicio del acuerdo (cuando presente) | "La fecha limite no puede ser anterior a la fecha de inicio del acuerdo" | `ServiceResponseMessageType.Validation_InvalidDate` |
| Command completo | `MustAsync`: sumaActual + ImporteParcial <= ImporteTotalPactado | "La suma de importes de milestones supera el importe total pactado" | `ServiceResponseMessageType.BusinessRule_MilestoneImporteExceeded` |

**Nota sobre la validacion de suma:** El validator llama a `_milestoneService.GetSumaImportesAsync()` y `_acuerdoService.GetByIdAsync()`. Ambos usan cache, por lo que el handler reutiliza los mismos objetos.

---

### 4.4 UpdateMilestoneCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/UpdateMilestoneCommandValidator.cs`

**Extiende:** `AbstractValidator<UpdateMilestoneCommand>`

**Dependencias del constructor:**

```
IAcuerdoCrowdsourcingService _acuerdoService
IAcuerdoCrowdsourcingMilestoneService _milestoneService
```

**Reglas:** Identicas a `CreateMilestoneCommandValidator` para los campos de texto e importe, mas:

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `MilestoneId` | `NotEmpty()` | "El ID del milestone es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| Command completo | `MustAsync`: suma excluyendo el milestone actual + nuevoImporte <= total | "La suma de importes de milestones supera el importe total pactado" | `ServiceResponseMessageType.BusinessRule_MilestoneImporteExceeded` |

**Nota sobre calculo de suma en UPDATE:** Usar `_milestoneService.GetSumaImportesExcluyendoAsync(acuerdoId, milestoneId, ct)` en lugar de `GetSumaImportesAsync`.

---

### 4.5 DeleteMilestoneCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/DeleteMilestoneCommandValidator.cs`

**Extiende:** `AbstractValidator<DeleteMilestoneCommand>`

**Dependencias del constructor:**

```
(Ninguna - las validaciones de negocio se hacen en el handler para Delete)
```

**Reglas (solo validaciones de formato):**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `MilestoneId` | `NotEmpty()` | "El ID del milestone es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `AcuerdoId` | `NotEmpty()` | "El ID del acuerdo es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

**Nota de diseno:** Para DeleteMilestoneCommand, las validaciones de negocio (milestone completado, tiene entregables, acuerdo activo) se realizan directamente en el handler despues de cargar las entidades. El validator se mantiene ligero para no duplicar las queries del handler. Esta decision sigue el patron del proyecto (ver `RetirarPropuestaCommandValidator`).

---

### 4.6 CreateEntregableCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CreateEntregableCommandValidator.cs`

**Extiende:** `AbstractValidator<CreateEntregableCommand>`

**Dependencias del constructor:**

```
(Ninguna dependencia de servicios - solo validaciones de formato)
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `Titulo` | `NotEmpty()` | "El titulo es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Titulo` | `MinimumLength(3)` | "El titulo debe tener al menos 3 caracteres" | `ServiceResponseMessageType.Validation_MinLength` |
| `Titulo` | `MaximumLength(200)` | "El titulo no puede superar los 200 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `Descripcion` | `MaximumLength(1000)` cuando presente | "La descripcion no puede superar los 1000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |
| `UrlRecurso` | `Must(url => Uri.TryCreate(...))` cuando presente | "Debe ser una URL valida (ej: Dropbox, Drive, WeTransfer)" | `ServiceResponseMessageType.Validation_InvalidUrl` |

**Nota:** Las validaciones de negocio (acuerdo activo, milestoneId pertenece al acuerdo) se hacen en el handler porque requieren cargar el acuerdo, que el handler ya necesita para la autorizacion. Evita queries duplicadas.

---

### 4.7 AprobarEntregableCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/AprobarEntregableCommandValidator.cs`

**Extiende:** `AbstractValidator<AprobarEntregableCommand>`

**Dependencias del constructor:**

```
(Ninguna - solo validacion de formato del comentario)
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `Comentario` | `MaximumLength(500)` cuando presente | "El comentario no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

---

### 4.8 RechazarEntregableCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/RechazarEntregableCommandValidator.cs`

**Extiende:** `AbstractValidator<RechazarEntregableCommand>`

**Dependencias del constructor:**

```
(Ninguna - solo validaciones de formato)
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `Comentario` | `NotEmpty()` | "El comentario es obligatorio al rechazar un entregable" | `ServiceResponseMessageType.Validation_Required` |
| `Comentario` | `MinimumLength(10)` | "Minimo 10 caracteres explicando que debe corregirse" | `ServiceResponseMessageType.Validation_MinLength` |
| `Comentario` | `MaximumLength(500)` | "El comentario no puede superar los 500 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

---

### 4.9 CompletarAcuerdoCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CompletarAcuerdoCommandValidator.cs`

**Extiende:** `AbstractValidator<CompletarAcuerdoCommand>`

**Dependencias del constructor:**

```
(Ninguna - solo validacion del ID)
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `AcuerdoId` | `NotEmpty()` | "El ID del acuerdo es obligatorio" | `ServiceResponseMessageType.Validation_Required` |

---

### 4.10 CancelarAcuerdoCommandValidator

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CancelarAcuerdoCommandValidator.cs`

**Extiende:** `AbstractValidator<CancelarAcuerdoCommand>`

**Dependencias del constructor:**

```
(Ninguna - solo validaciones de formato)
```

**Reglas:**

| Campo | Regla | Mensaje | ErrorCode Constant |
|-------|-------|---------|-------------------|
| `AcuerdoId` | `NotEmpty()` | "El ID del acuerdo es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Motivo` | `NotEmpty()` | "El motivo de cancelacion es obligatorio" | `ServiceResponseMessageType.Validation_Required` |
| `Motivo` | `MinimumLength(20)` | "El motivo debe tener al menos 20 caracteres" | `ServiceResponseMessageType.Validation_MinLength` |
| `Motivo` | `MaximumLength(1000)` | "El motivo no puede superar los 1000 caracteres" | `ServiceResponseMessageType.Validation_MaxLength` |

---

## 5. AutoMapper Profiles

### 5.1 AcuerdoProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/AcuerdoProfile.cs`

**Clase:** `AcuerdoProfile : Profile`

**Mappings a definir:**

```
AcuerdoCrowdsourcing -> AcuerdoDto
    - Id.Value -> Id
    - TituloInterno -> TituloInterno
    - EstadoAcuerdoId -> EstadoAcuerdoId
    - EstadoAcuerdo.Nombre -> EstadoAcuerdoNombre (requiere Include en query)
    - ImporteTotalPactado -> ImporteTotalPactado
    - Moneda.Nombre -> MonedaNombre (requiere Include o proyeccion)
    - FechaInicio -> FechaInicio
    - FechaFinPrevista -> FechaFinPrevista
    - FechaFinReal -> FechaFinReal
    - Necesidad.Id.Value -> Necesidad.Id (sub-DTO)
    - Necesidad.Titulo -> Necesidad.Titulo
    - ConversacionId -> ConversacionId (primer item de Conversaciones o null)
    - Milestones -> Milestones (lista; ver MilestoneProfile)
    ImporteAsignado, PorcentajeAsignado, MiRol, Timeline: NO se mapean via AutoMapper,
    se calculan y asignan manualmente en el handler

AcuerdoCrowdsourcing -> AcuerdoArtistaDto
    - ArtistaId.Value -> Id
    - Artista.NombreArtistico -> NombreArtistico (requiere Include al Artista cross-modulo)

AcuerdoCrowdsourcing -> AcuerdoProfesionalDto
    - UserIdProveedor -> UserId
    - PerfilProfesionalId.Value -> PerfilProfesionalId
    - [nombre del perfil: se carga aparte o via Include]
```

**Nota sobre mappings complejos:** Dado que `AcuerdoArtistaDto` requiere datos de la entidad `Artista` (modulo UserAccess) y `AcuerdoProfesionalDto` requiere datos del `PerfilProfesional`, el mapeo de estos sub-DTOs se realiza manualmente en el handler despues del Map principal, o se usa `ForMember` con resolvers. En el plan de implementacion se recomienda construir estos sub-DTOs manualmente en el handler para mayor claridad.

---

### 5.2 MilestoneProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/MilestoneProfile.cs`

**Clase:** `MilestoneProfile : Profile`

**Mappings a definir:**

```
AcuerdoCrowdsourcingMilestone -> MilestoneDto
    - Id -> Id
    - Titulo -> Titulo
    - Descripcion -> Descripcion
    - Orden -> Orden
    - ImporteParcial -> ImporteParcial
    - FechaLimite -> FechaLimite
    - FechaCompletado -> FechaCompletado
    - Entregables -> Entregables (lista; ver EntregableProfile)
    PorcentajeParcial: NO se mapea, se calcula manualmente en el handler

AcuerdoCrowdsourcingMilestone -> MilestoneCreatedResultDto
    - Id -> Id
    - Titulo -> Titulo
    - Orden -> Orden
    - ImporteParcial -> ImporteParcial
    PorcentajeParcial, ImporteAsignadoTotal: calculados manualmente en el handler
```

---

### 5.3 EntregableProfile

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/EntregableProfile.cs`

**Clase:** `EntregableProfile : Profile`

**Mappings a definir:**

```
AcuerdoCrowdsourcingEntregable -> EntregableDto
    - Id -> Id
    - Titulo -> Titulo
    - Descripcion -> Descripcion
    - UrlRecurso -> UrlRecurso
    - EstadoEntregableId -> EstadoEntregableId
    - EstadoEntregable.Nombre -> EstadoEntregableNombre (si se incluye la maestra)
    - ComentarioAprobacion -> ComentarioAprobacion
    - ComentarioRechazo -> ComentarioRechazo
    - FechaAprobacion -> FechaAprobacion
    - FechaCreacion -> FechaCreacion

AcuerdoCrowdsourcingEntregable -> EntregableCreatedResultDto
    - Id -> Id
    - Titulo -> Titulo
    - FechaCreacion -> FechaCreacion
    EstadoEntregableNombre: asignado manualmente ("Entregado") en el handler
```

---

## 6. Estructura de Archivos a Crear

```
src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/
├── Features/
│   └── Acuerdos/
│       ├── Commands/
│       │   ├── AceptarPropuestaCommand.cs         (Command + Handler)
│       │   ├── RechazarPropuestaCommand.cs        (Command + Handler)
│       │   ├── CreateMilestoneCommand.cs          (Command + Handler)
│       │   ├── UpdateMilestoneCommand.cs          (Command + Handler)
│       │   ├── DeleteMilestoneCommand.cs          (Command + Handler)
│       │   ├── CreateEntregableCommand.cs         (Command + Handler)
│       │   ├── AprobarEntregableCommand.cs        (Command + Handler)
│       │   ├── RechazarEntregableCommand.cs       (Command + Handler)
│       │   ├── CompletarAcuerdoCommand.cs         (Command + Handler)
│       │   └── CancelarAcuerdoCommand.cs          (Command + Handler)
│       ├── Queries/
│       │   └── GetAcuerdoByIdQuery.cs             (Query + Handler)
│       └── Validators/
│           ├── AceptarPropuestaCommandValidator.cs
│           ├── RechazarPropuestaCommandValidator.cs
│           ├── CreateMilestoneCommandValidator.cs
│           ├── UpdateMilestoneCommandValidator.cs
│           ├── DeleteMilestoneCommandValidator.cs
│           ├── CreateEntregableCommandValidator.cs
│           ├── AprobarEntregableCommandValidator.cs
│           ├── RechazarEntregableCommandValidator.cs
│           ├── CompletarAcuerdoCommandValidator.cs
│           └── CancelarAcuerdoCommandValidator.cs
└── Mapping/
    ├── AcuerdoProfile.cs
    ├── MilestoneProfile.cs
    └── EntregableProfile.cs
```

---

## 7. Detalle Especial: AceptarPropuestaCommand (Transaccion)

Esta es la operacion mas critica de la feature. La transaccion se gestiona en el **Service**, no en el Handler. El handler orquesta la logica de negocio; el service ejecuta los 6 pasos atomicos.

### Flujo transaccional en IAcuerdoCrowdsourcingService.AceptarPropuestaAsync()

El service recibe:
- `AcuerdoCrowdsourcing acuerdo` - objeto ya construido por el handler
- `PropuestaCrowdsourcing propuesta` - objeto ya cargado (reutilizado de la cache)
- `NecesidadCrowdsourcing necesidad` - objeto ya cargado (reutilizado de la cache)
- `CancellationToken ct`

Retorna: `(AcuerdoCrowdsourcingId acuerdoId, int propuestasRechazadas, Guid conversacionId)`

**Pasos dentro de la transaccion (en el service):**

```
Abrir: await _context.Database.BeginTransactionAsync(ct)

Paso 1: Crear el AcuerdoCrowdsourcing
    acuerdoId = await _repository.AddAsync(acuerdo, ct)

Paso 2: Actualizar propuesta aceptada a estado Aceptada + enlazar AcuerdoId
    propuesta.EstadoPropuestaId = EstadoPropuestaConstants.Aceptada
    propuesta.AcuerdoId = acuerdo.Id.Value  (campo NUEVO en PropuestaCrowdsourcing)
    propuesta.FechaActualizacion = DateTime.UtcNow
    await _propuestaRepository.UpdateAsync(propuesta, ct)

Paso 3: Actualizar otras propuestas Pendientes de la misma necesidad a Rechazada
    var propuestasPendientes = await _propuestaRepository.GetPendientesByNecesidadExcludingAsync(
        necesidad.Id, propuesta.Id, ct)
    propuestasRechazadas = propuestasPendientes.Count
    Para cada pendiente:
        pendiente.EstadoPropuestaId = EstadoPropuestaConstants.Rechazada
        pendiente.MotivoRechazo = "Otra propuesta fue aceptada"
        pendiente.FechaActualizacion = DateTime.UtcNow
    await _propuestaRepository.UpdateRangeAsync(propuestasPendientes, ct)

Paso 4: Actualizar necesidad a En Progreso
    necesidad.EstadoNecesidadId = EstadoNecesidadConstants.EnProgreso
    necesidad.FechaActualizacion = DateTime.UtcNow
    await _necesidadRepository.UpdateAsync(necesidad, ct)

Paso 5: Crear ConversacionCrowdsourcing vinculada al acuerdo
    var conversacion = new ConversacionCrowdsourcing
    {
        Id = Guid.NewGuid(),
        AcuerdoId = acuerdoId,
        FechaCreacion = DateTime.UtcNow
    }
    conversacionId = await _conversacionRepository.AddAsync(conversacion, ct)

Commit: await transaction.CommitAsync(ct)
Retornar: (acuerdoId, propuestasRechazadas, conversacionId)

En caso de excepcion:
    await transaction.RollbackAsync(ct)
    _logger.LogError(ex, "Error en transaccion AceptarPropuesta para PropuestaId {PropuestaId}")
    throw  (el handler captura y retorna error)
```

**Razon por la que la transaccion va en el service y NO en el handler:**
- El handler no puede inyectar `CrowdsourcingContext` (viola REGLA 3 del CQRS)
- El service si puede inyectarlo (esta en la capa Infrastructure)
- Patron del proyecto: `AcuerdoCrowdsourcingService` inyecta `CrowdsourcingContext` para transacciones (ver hexagonal-architecture.md seccion 4.2)

---

## 8. Namespace y Usings - Patron del Proyecto

Siguiendo el patron observado en los Commands existentes del modulo Crowdsourcing:

**Namespace base:** `WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands`

**Usings criticos para todos los Commands/Queries:**

```csharp
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;  // Para IArtistaService
```

**Usings criticos para Validators:**

```csharp
using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
```

---

## 9. Patron de Respuestas de Error

Siguiendo el patron de `ValidateExtensions` observado en el proyecto:

```csharp
// NotFound
return ValidateExtensions.NotFoundServiceResponse<TDto>(
    "Acuerdo no encontrado",
    ServiceResponseMessageType.NotFound_Acuerdo);

// Forbidden
return ValidateExtensions.ForbiddenServiceResponse<TDto>(
    "No tienes permiso para realizar esta accion",
    ServiceResponseMessageType.Auth_Forbidden);

// Internal Error (en el catch)
return ValidateExtensions.InternalServerErrorServiceResponse<TDto>(
    "Error inesperado al [accion]",
    ServiceResponseMessageType.Internal_UnexpectedError);

// Business Rule
return new ServiceResponse<TDto>
{
    Messages = new List<ServiceResponseMessage>
    {
        new()
        {
            Message = "El acuerdo no esta activo",
            ErrorCode = ServiceResponseMessageType.BusinessRule_AcuerdoNotActive
        }
    }
};
```

---

## 10. Orden de Implementacion Recomendado

El orden minimiza el tiempo de espera entre dependencias y permite probar cada bloque antes de avanzar.

### Fase 1 - Infraestructura base (prerequisito de todo)

```
1. Constantes nuevas en ServiceResponseMessageType.cs
   (NotFound_Acuerdo, NotFound_Milestone, NotFound_Entregable,
    BusinessRule_PropuestaNotAcceptable, BusinessRule_AcuerdoAlreadyExists,
    BusinessRule_MilestoneImporteExceeded, BusinessRule_AcuerdoNotActive,
    BusinessRule_MilestoneCompleted, BusinessRule_MilestoneHasEntregables,
    BusinessRule_EntregableNotReviewable, Validation_InvalidUrl)

2. Constantes de estado: EstadoAcuerdoConstants.cs, EstadoEntregableConstants.cs

3. AutoMapper Profiles: AcuerdoProfile.cs, MilestoneProfile.cs, EntregableProfile.cs
```

### Fase 2 - Operaciones de propuesta (amplian PropuestasController)

```
4. RechazarPropuestaCommandValidator.cs
5. RechazarPropuestaCommand.cs (Command + Handler)
6. AceptarPropuestaCommandValidator.cs
7. AceptarPropuestaCommand.cs (Command + Handler)  <- mas complejo, al final de la fase
```

### Fase 3 - Lectura de acuerdo (depende de que el acuerdo pueda crearse)

```
8. GetAcuerdoByIdQuery.cs (Query + Handler)
```

### Fase 4 - Milestones (dependen del acuerdo)

```
9.  DeleteMilestoneCommandValidator.cs
10. DeleteMilestoneCommand.cs (Command + Handler)
11. CreateMilestoneCommandValidator.cs
12. CreateMilestoneCommand.cs (Command + Handler)
13. UpdateMilestoneCommandValidator.cs
14. UpdateMilestoneCommand.cs (Command + Handler)
```

### Fase 5 - Entregables (dependen del acuerdo y opcionalmente del milestone)

```
15. CreateEntregableCommandValidator.cs
16. CreateEntregableCommand.cs (Command + Handler)
17. AprobarEntregableCommandValidator.cs
18. AprobarEntregableCommand.cs (Command + Handler)
19. RechazarEntregableCommandValidator.cs
20. RechazarEntregableCommand.cs (Command + Handler)
```

### Fase 6 - Ciclo de vida del acuerdo (dependen de los anteriores)

```
21. CompletarAcuerdoCommandValidator.cs
22. CompletarAcuerdoCommand.cs (Command + Handler)
23. CancelarAcuerdoCommandValidator.cs
24. CancelarAcuerdoCommand.cs (Command + Handler)
```

---

## 11. ServiceResponseMessageType - Constantes Referenciadas

Todas las constantes que los Handlers y Validators de esta feature deben usar:

### Existentes (reutilizar)

| Constante | Valor | Donde se usa |
|-----------|-------|--------------|
| `ServiceResponseMessageType.Created` | "0001" | AceptarPropuesta, CreateMilestone, CreateEntregable |
| `ServiceResponseMessageType.Updated` | "0002" | RechazarPropuesta, AprobarEntregable, RechazarEntregable, Completar, Cancelar |
| `ServiceResponseMessageType.Deleted` | "0003" | DeleteMilestone |
| `ServiceResponseMessageType.Validation_Required` | "1001" | Validators de titulo, motivo, comentario, IDs |
| `ServiceResponseMessageType.Validation_MaxLength` | "1002" | Validators de campos con longitud maxima |
| `ServiceResponseMessageType.Validation_MinLength` | "1011" | Validators de titulo (min 3), comentario rechazo (min 10), motivo (min 20) |
| `ServiceResponseMessageType.Validation_InvalidDate` | "1012" | FechaFinPrevista, FechaLimite |
| `ServiceResponseMessageType.Validation_ForeignKeyNotFound` | "1010" | MilestoneId no pertenece al acuerdo |
| `ServiceResponseMessageType.NotFound_Propuesta` | "2010" | AceptarPropuesta, RechazarPropuesta |
| `ServiceResponseMessageType.Auth_Forbidden` | "3002" | Todos los handlers de autorizacion |
| `ServiceResponseMessageType.Internal_UnexpectedError` | "5000" | Todos los catch de handlers |

### Nuevas (agregar al archivo de constantes)

| Constante | Valor | Donde se usa |
|-----------|-------|--------------|
| `ServiceResponseMessageType.Validation_InvalidUrl` | "1013" | CreateEntregableCommandValidator |
| `ServiceResponseMessageType.NotFound_Acuerdo` | "2011" | Todos los handlers de acuerdo |
| `ServiceResponseMessageType.NotFound_Milestone` | "2012" | CreateMilestone, UpdateMilestone, DeleteMilestone handlers |
| `ServiceResponseMessageType.NotFound_Entregable` | "2013" | AprobarEntregable, RechazarEntregable handlers |
| `ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable` | "4007" | AceptarPropuestaValidator, RechazarPropuestaCommandHandler |
| `ServiceResponseMessageType.BusinessRule_AcuerdoAlreadyExists` | "4008" | AceptarPropuestaValidator |
| `ServiceResponseMessageType.BusinessRule_MilestoneImporteExceeded` | "4009" | CreateMilestoneValidator, UpdateMilestoneValidator |
| `ServiceResponseMessageType.BusinessRule_AcuerdoNotActive` | "4010" | CreateMilestone, UpdateMilestone, DeleteMilestone, CreateEntregable, AprobarEntregable, RechazarEntregable, Completar handlers |
| `ServiceResponseMessageType.BusinessRule_MilestoneCompleted` | "4011" | UpdateMilestone, DeleteMilestone handlers |
| `ServiceResponseMessageType.BusinessRule_MilestoneHasEntregables` | "4012" | DeleteMilestone handler |
| `ServiceResponseMessageType.BusinessRule_EntregableNotReviewable` | "4013" | AprobarEntregable, RechazarEntregable handlers |

---

## 12. Checklist de Implementacion

### Por cada Command/Query

- [ ] Command/Query implementa `IRequest<ServiceResponse<T>>`
- [ ] Handler en MISMO archivo que Command/Query (separados por comentario `// COMMAND` y `// HANDLER`)
- [ ] Constructor del Handler con `?? throw new ArgumentNullException(nameof(...))` para TODAS las dependencias
- [ ] Handler NUNCA inyecta `CrowdsourcingContext` ni ningun DbContext
- [ ] Handler inyecta `ILogger<NombreHandler>` y lo usa en el try-catch y en LogWarning de autorizacion
- [ ] Flujo del Handle: Validar -> Cargar entidades -> Verificar autorizacion -> Aplicar logica de negocio -> Llamar service -> Retornar ServiceResponse
- [ ] Try-catch con `_logger.LogError(ex, "mensaje con {params}", valores)` y `InternalServerErrorServiceResponse`
- [ ] Namespace: `WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands` (o Queries)

### Por cada Validator

- [ ] Validator en carpeta `Validators/` separada de Commands y Queries
- [ ] Constructor con `?? throw new ArgumentNullException(nameof(...))` para TODAS las dependencias
- [ ] Cada regla tiene `.WithMessage("...")` Y `.WithErrorCode(ServiceResponseMessageType.X)` (NO strings literales)
- [ ] Reglas condicionales usan `.When(x => ...)` antes de `.WithMessage()`
- [ ] Validaciones async usan `.MustAsync(async (command, ct) => ...)` con comentario de cache si aplica
- [ ] Importa `using WePlayRises.Crowdsourcing.Domain.Constants;`
- [ ] Namespace: `WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators`

### Para AceptarPropuestaCommand (especifico)

- [ ] La transaccion de BD esta en `IAcuerdoCrowdsourcingService.AceptarPropuestaAsync()`, no en el handler
- [ ] El handler construye el `AcuerdoCrowdsourcing` entity antes de llamar al service
- [ ] El handler verifica autorizacion (artista propietario de la necesidad) antes de llamar al service
- [ ] El validator usa `IRequestCacheService` (via los services) para evitar queries duplicados
- [ ] El service recibe objetos ya cargados (no los recarga dentro)
- [ ] En caso de exception en el service, el rollback ocurre en el service; el handler captura y retorna error

### Para GetAcuerdoByIdQuery (especifico)

- [ ] `MiRol` se calcula en el handler comparando `UserId` del token contra `ArtistaId` (resuelto) y `UserIdProveedor`
- [ ] `ImporteAsignado` y `PorcentajeAsignado` se calculan en el handler sobre los datos ya cargados
- [ ] `PorcentajeParcial` de cada milestone se calcula en el handler (loop)
- [ ] Timeline se construye en el handler (sin tabla separada en MVP), limitado a 20 eventos, ordenado de mas reciente a mas antiguo
- [ ] Usar `AsNoTracking()` (garantizado por `GetByIdWithDetailsAsync` del service)

### Para AutoMapper Profiles

- [ ] Un Profile separado por entidad: `AcuerdoProfile`, `MilestoneProfile`, `EntregableProfile`
- [ ] Los campos calculados (`MiRol`, `ImporteAsignado`, `PorcentajeParcial`, `Timeline`) NO se mapean via AutoMapper
- [ ] Los sub-DTOs complejos que requieren datos cross-modulo (`AcuerdoArtistaDto`, `AcuerdoProfesionalDto`) se construyen manualmente en el handler
- [ ] Namespace: `WePlayRises.Crowdsourcing.Application.Mapping`
