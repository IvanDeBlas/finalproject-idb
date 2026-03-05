# Contratos API: cs-acuerdos-entregables

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-acuerdos-entregables (US-CS-04)
**Basado en:** `docs/user-stories/cs-acuerdos-entregables/contracts.md` y `feature-spec.md`

---

## 1. Endpoints

| Metodo | Ruta | Controller | Tipo | Auth | Descripcion |
|--------|------|------------|------|------|-------------|
| `POST` | `/api/crowdsourcing/propuestas/{id}/aceptar` | `PropuestasCrowdsourcingController` | Command | Artista propietario de la necesidad | Acepta propuesta, crea acuerdo (transaccional 6 efectos) |
| `PATCH` | `/api/crowdsourcing/propuestas/{id}/rechazar` | `PropuestasCrowdsourcingController` | Command | Artista propietario de la necesidad | Rechaza propuesta individualmente |
| `GET` | `/api/crowdsourcing/acuerdos/{id}` | `AcuerdosCrowdsourcingController` | Query | Participante del acuerdo | Detalle completo con milestones, entregables y timeline |
| `POST` | `/api/crowdsourcing/acuerdos/{acuerdoId}/milestones` | `AcuerdosCrowdsourcingController` | Command | Artista del acuerdo | Crear milestone |
| `PUT` | `/api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` | `AcuerdosCrowdsourcingController` | Command | Artista del acuerdo | Editar milestone (solo si no completado) |
| `DELETE` | `/api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` | `AcuerdosCrowdsourcingController` | Command | Artista del acuerdo | Eliminar milestone (solo si sin entregables) |
| `POST` | `/api/crowdsourcing/acuerdos/{acuerdoId}/entregables` | `AcuerdosCrowdsourcingController` | Command | Profesional del acuerdo | Subir entregable con URL externa |
| `PATCH` | `/api/crowdsourcing/entregables/{id}/aprobar` | `EntregablesCrowdsourcingController` | Command | Artista del acuerdo | Aprobar entregable en estado Entregado |
| `PATCH` | `/api/crowdsourcing/entregables/{id}/rechazar` | `EntregablesCrowdsourcingController` | Command | Artista del acuerdo | Rechazar entregable en estado Entregado |
| `PATCH` | `/api/crowdsourcing/acuerdos/{id}/completar` | `AcuerdosCrowdsourcingController` | Command | Artista del acuerdo | Completar acuerdo activo |
| `PATCH` | `/api/crowdsourcing/acuerdos/{id}/cancelar` | `AcuerdosCrowdsourcingController` | Command | Participante del acuerdo | Cancelar acuerdo activo |

---

## 2. Request DTOs (Commands y Queries)

### 2.1 AceptarPropuestaCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/AceptarPropuestaCommand.cs`
**Implementa:** `IRequest<ServiceResponse<AceptarPropuestaResultDto>>`
**Nota:** Handler + Command en el mismo archivo (patron CQRS del proyecto).

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `PropuestaId` | `Guid` | Route param `{id}` | ID de la propuesta a aceptar |
| `UserId` | `string` | JWT claim `sub` | Resuelto por el controller desde `User.FindFirstValue(ClaimTypes.NameIdentifier)` |
| `TituloInterno` | `string` | Request body | Titulo del acuerdo. Default: titulo de la necesidad. Requerido. Max 200. |
| `FechaInicio` | `DateTime` | Request body | Fecha de inicio pactada. Requerida. Default: hoy. |
| `FechaFinPrevista` | `DateTime?` | Request body | Fecha fin estimada. Opcional. Si presente: > FechaInicio. |

**Body JSON recibido (del cliente):**
```json
{
  "tituloInterno": "Mezcla EP Los Rockeros",
  "fechaInicio": "2026-03-01",
  "fechaFinPrevista": "2026-03-15"
}
```

---

### 2.2 RechazarPropuestaCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/RechazarPropuestaCommand.cs`
**Implementa:** `IRequest<ServiceResponse<RechazarPropuestaResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `PropuestaId` | `Guid` | Route param `{id}` | ID de la propuesta a rechazar |
| `UserId` | `string` | JWT claim `sub` | Resuelto por el controller |
| `Motivo` | `string?` | Request body | Motivo de rechazo. Opcional. Max 500. NO se expone al profesional. |

**Body JSON recibido (del cliente):**
```json
{
  "motivo": "El presupuesto no se ajusta a nuestras posibilidades"
}
```

---

### 2.3 GetAcuerdoByIdQuery

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Queries/GetAcuerdoByIdQuery.cs`
**Implementa:** `IRequest<ServiceResponse<AcuerdoDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo |
| `UserId` | `string` | JWT claim `sub` | Para calcular `MiRol` y verificar participacion |

---

### 2.4 CreateMilestoneCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CreateMilestoneCommand.cs`
**Implementa:** `IRequest<ServiceResponse<MilestoneCreatedResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |
| `Titulo` | `string` | Request body | Titulo del milestone. Requerido. Min 3, max 200. |
| `Descripcion` | `string?` | Request body | Descripcion opcional. Max 1000. |
| `ImporteParcial` | `decimal` | Request body | Importe parcial del milestone. Requerido. > 0. |
| `FechaLimite` | `DateTime?` | Request body | Fecha limite opcional. Si presente: >= FechaInicio del acuerdo. |

**Body JSON recibido (del cliente):**
```json
{
  "titulo": "Mezcla de pistas 1-3",
  "descripcion": "Mezcla de las primeras 3 canciones del EP",
  "importeParcial": 270.00,
  "fechaLimite": "2026-03-08"
}
```

---

### 2.5 UpdateMilestoneCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/UpdateMilestoneCommand.cs`
**Implementa:** `IRequest<ServiceResponse<MilestoneCreatedResultDto>>`
**Nota:** Reutiliza `MilestoneCreatedResultDto` como response (mismo shape que el create).

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `MilestoneId` | `Guid` | Route param `{id}` | ID del milestone a editar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |
| `Titulo` | `string` | Request body | Mismo que CreateMilestoneCommand |
| `Descripcion` | `string?` | Request body | Mismo que CreateMilestoneCommand |
| `ImporteParcial` | `decimal` | Request body | Mismo que CreateMilestoneCommand |
| `FechaLimite` | `DateTime?` | Request body | Mismo que CreateMilestoneCommand |

**Body JSON recibido:** Identico a `CreateMilestoneCommand`.

---

### 2.6 DeleteMilestoneCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/DeleteMilestoneCommand.cs`
**Implementa:** `IRequest<ServiceResponse<bool>>`
**Nota:** Response 204 No Content en exito. `ServiceResponse<bool>` con Data=true usado internamente; controller retorna 204.

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `MilestoneId` | `Guid` | Route param `{id}` | ID del milestone a eliminar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |

**Body JSON recibido:** Ninguno.

---

### 2.7 CreateEntregableCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CreateEntregableCommand.cs`
**Implementa:** `IRequest<ServiceResponse<EntregableCreatedResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{acuerdoId}` | ID del acuerdo padre |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el profesional del acuerdo |
| `Titulo` | `string` | Request body | Titulo del entregable. Requerido. Min 3, max 200. |
| `Descripcion` | `string?` | Request body | Descripcion opcional. Max 1000. |
| `UrlRecurso` | `string?` | Request body | URL externa (Drive, Dropbox, WeTransfer). Opcional. URL valida si presente. |
| `MilestoneId` | `Guid?` | Request body | Milestone al que se asocia. Opcional. Debe pertenecer al mismo acuerdo. |

**Body JSON recibido:**
```json
{
  "titulo": "Mezcla cancion 1 - v1",
  "descripcion": "Primera version de la mezcla de la cancion 1",
  "urlRecurso": "https://drive.google.com/file/xyz",
  "milestoneId": "a9b8c7d6-e5f4-3a2b-1c0d-9e8f7a6b5c4d"
}
```

---

### 2.8 AprobarEntregableCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/AprobarEntregableCommand.cs`
**Implementa:** `IRequest<ServiceResponse<AprobarEntregableResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `EntregableId` | `Guid` | Route param `{id}` | ID del entregable a aprobar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo del entregable |
| `Comentario` | `string?` | Request body | Comentario opcional. Max 500 chars. |

**Body JSON recibido:**
```json
{
  "comentario": "Excelente mezcla, me encanta el resultado"
}
```

---

### 2.9 RechazarEntregableCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/RechazarEntregableCommand.cs`
**Implementa:** `IRequest<ServiceResponse<RechazarEntregableResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `EntregableId` | `Guid` | Route param `{id}` | ID del entregable a rechazar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo del entregable |
| `Comentario` | `string` | Request body | Comentario obligatorio. Min 10, max 500 chars. |

**Body JSON recibido:**
```json
{
  "comentario": "La voz esta demasiado baja en el coro, necesita mas presencia."
}
```

---

### 2.10 CompletarAcuerdoCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CompletarAcuerdoCommand.cs`
**Implementa:** `IRequest<ServiceResponse<CompletarAcuerdoResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo a completar |
| `UserId` | `string` | JWT claim `sub` | Para verificar que es el artista del acuerdo |

**Body JSON recibido:** Ninguno.

---

### 2.11 CancelarAcuerdoCommand

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Commands/CancelarAcuerdoCommand.cs`
**Implementa:** `IRequest<ServiceResponse<CancelarAcuerdoResultDto>>`

| Propiedad | Tipo | Origen | Descripcion |
|-----------|------|--------|-------------|
| `AcuerdoId` | `Guid` | Route param `{id}` | ID del acuerdo a cancelar |
| `UserId` | `string` | JWT claim `sub` | Para registrar `CanceladoPor` y verificar participacion |
| `Motivo` | `string` | Request body | Motivo obligatorio. Min 20, max 1000 chars. |

**Body JSON recibido:**
```json
{
  "motivo": "No puedo continuar por motivos personales. Lamento las molestias causadas."
}
```

---

## 3. Response DTOs

### 3.1 AceptarPropuestaResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AceptarPropuestaResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `AcuerdoId` | `Guid` | ID del acuerdo creado. Frontend redirige a `/crowdsourcing/acuerdos/{acuerdoId}`. |
| `TituloInterno` | `string` | Titulo del acuerdo tal como se creo |
| `EstadoAcuerdoNombre` | `string` | Siempre "Activo" en este response |
| `ImporteTotalPactado` | `decimal` | Importe derivado del `PrecioPropuesto` de la propuesta |
| `MonedaNombre` | `string` | Nombre de la moneda (ej: "EUR") |
| `ConversacionId` | `Guid` | ID de la conversacion creada automaticamente |
| `PropuestasRechazadas` | `int` | Numero de propuestas que pasaron a Rechazada automaticamente |

**Wrapping:** `ServiceResponse<AceptarPropuestaResultDto>`
**HTTP Status:** 201 Created

---

### 3.2 RechazarPropuestaResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/RechazarPropuestaResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID de la propuesta rechazada |
| `EstadoPropuestaNombre` | `string` | Siempre "Rechazada" en este response |

**Wrapping:** `ServiceResponse<RechazarPropuestaResultDto>`
**HTTP Status:** 200 OK

---

### 3.3 AcuerdoDto (Response del GET detalle)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AcuerdoDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del acuerdo |
| `TituloInterno` | `string` | Titulo interno del acuerdo |
| `EstadoAcuerdoId` | `int` | 1=Activo, 2=Completado, 3=Cancelado |
| `EstadoAcuerdoNombre` | `string` | "Activo", "Completado" o "Cancelado" |
| `ImporteTotalPactado` | `decimal` | Importe total del acuerdo |
| `MonedaNombre` | `string` | Nombre de la moneda |
| `FechaInicio` | `DateTime` | Fecha de inicio del acuerdo |
| `FechaFinPrevista` | `DateTime?` | Fecha fin estimada. Nullable. |
| `FechaFinReal` | `DateTime?` | Fecha real de finalizacion. Null si activo. |
| `Artista` | `AcuerdoArtistaDto` | Sub-objeto con datos del artista contratante |
| `Profesional` | `AcuerdoProfesionalDto` | Sub-objeto con datos del profesional |
| `Necesidad` | `AcuerdoNecesidadDto` | Sub-objeto con datos de la necesidad de origen |
| `ConversacionId` | `Guid?` | ID de la conversacion vinculada al acuerdo |
| `Milestones` | `List<MilestoneDto>` | Lista de milestones con entregables anidados, ordenados por `Orden` |
| `ImporteAsignado` | `decimal` | Calculado: SUM(Milestones.ImporteParcial). Calculado en handler. |
| `PorcentajeAsignado` | `decimal` | Calculado: (ImporteAsignado / ImporteTotalPactado) * 100. Redondeado 2 decimales. |
| `MiRol` | `string` | "Artista" o "Profesional". Calculado en handler comparando UserId con participantes. |
| `Timeline` | `List<AcuerdoTimelineEventoDto>` | Ultimos 20 eventos, ordenados de mas reciente a mas antiguo |

**Wrapping:** `ServiceResponse<AcuerdoDto>`
**HTTP Status:** 200 OK

---

### 3.4 AcuerdoArtistaDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AcuerdoArtistaDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del artista |
| `NombreArtistico` | `string` | Nombre artistico del artista |

---

### 3.5 AcuerdoProfesionalDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AcuerdoProfesionalDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `UserId` | `string` | UserId del Identity User (proveedor) |
| `PerfilProfesionalId` | `Guid` | ID del perfil profesional |
| `Nombre` | `string` | Nombre visible del profesional (de PerfilProfesional) |

---

### 3.6 AcuerdoNecesidadDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AcuerdoNecesidadDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID de la necesidad de origen |
| `Titulo` | `string` | Titulo de la necesidad |

---

### 3.7 AcuerdoTimelineEventoDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AcuerdoTimelineEventoDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Accion` | `string` | Descripcion del evento (ej: "Acuerdo creado", "Milestone agregado: X", "Entregable subido: Y") |
| `Fecha` | `DateTime` | Timestamp del evento (UTC) |
| `Actor` | `string` | Nombre del usuario que realizo la accion |

**Nota de implementacion:** El timeline se construye en el handler de `GetAcuerdoByIdQuery` a partir de las fechas de creacion/actualizacion de las entidades relacionadas (no tabla separada en MVP). Maximo 20 eventos.

---

### 3.8 MilestoneDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MilestoneDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del milestone |
| `Titulo` | `string` | Titulo del milestone |
| `Descripcion` | `string?` | Descripcion opcional |
| `Orden` | `int` | Orden secuencial del milestone en el acuerdo |
| `ImporteParcial` | `decimal` | Importe parcial asignado a este milestone |
| `PorcentajeParcial` | `decimal` | Calculado: (ImporteParcial / ImporteTotalPactado) * 100. Calculado en handler. |
| `FechaLimite` | `DateTime?` | Fecha limite opcional |
| `FechaCompletado` | `DateTime?` | Null = pendiente; con valor = completado |
| `Entregables` | `List<EntregableDto>` | Lista de entregables anidados bajo este milestone |

---

### 3.9 EntregableDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/EntregableDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del entregable |
| `Titulo` | `string` | Titulo del entregable |
| `Descripcion` | `string?` | Descripcion opcional |
| `UrlRecurso` | `string?` | URL externa del recurso (Drive, Dropbox, etc.) |
| `EstadoEntregableId` | `int` | 1=Entregado, 2=Aprobado, 3=Rechazado |
| `EstadoEntregableNombre` | `string` | "Entregado", "Aprobado" o "Rechazado" |
| `ComentarioAprobacion` | `string?` | Comentario del artista al aprobar. Null si no aprobado. |
| `ComentarioRechazo` | `string?` | Comentario del artista al rechazar. Null si no rechazado. |
| `FechaAprobacion` | `DateTime?` | Timestamp de aprobacion. Null si no aprobado. |
| `FechaCreacion` | `DateTime` | Timestamp de creacion del entregable (UTC) |

---

### 3.10 MilestoneCreatedResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/MilestoneCreatedResultDto.cs`
**Usado por:** `CreateMilestoneCommand` (201) y `UpdateMilestoneCommand` (200).

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del milestone creado o actualizado |
| `Titulo` | `string` | Titulo del milestone |
| `Orden` | `int` | Orden asignado (MAX(Orden)+1 si nuevo) |
| `ImporteParcial` | `decimal` | Importe parcial del milestone |
| `PorcentajeParcial` | `decimal` | (ImporteParcial / ImporteTotalPactado) * 100 |
| `ImporteAsignadoTotal` | `decimal` | Nueva suma total de importes de todos los milestones del acuerdo tras la operacion |

**Wrapping:** `ServiceResponse<MilestoneCreatedResultDto>`

---

### 3.11 EntregableCreatedResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/EntregableCreatedResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del entregable creado |
| `Titulo` | `string` | Titulo del entregable |
| `EstadoEntregableNombre` | `string` | Siempre "Entregado" en este response |
| `FechaCreacion` | `DateTime` | Timestamp UTC de creacion |

**Wrapping:** `ServiceResponse<EntregableCreatedResultDto>`
**HTTP Status:** 201 Created

---

### 3.12 AprobarEntregableResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/AprobarEntregableResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del entregable aprobado |
| `EstadoEntregableNombre` | `string` | Siempre "Aprobado" en este response |
| `FechaAprobacion` | `DateTime` | Timestamp UTC de la aprobacion |
| `TodosAprobadosEnMilestone` | `bool` | true si todos los entregables del milestone vinculado estan Aprobados. false si el entregable no tiene milestone o si quedan otros sin aprobar. Permite al frontend sugerir marcar el milestone como completado. |

**Wrapping:** `ServiceResponse<AprobarEntregableResultDto>`
**HTTP Status:** 200 OK

---

### 3.13 RechazarEntregableResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/RechazarEntregableResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del entregable rechazado |
| `EstadoEntregableNombre` | `string` | Siempre "Rechazado" en este response |

**Wrapping:** `ServiceResponse<RechazarEntregableResultDto>`
**HTTP Status:** 200 OK

---

### 3.14 CompletarAcuerdoResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CompletarAcuerdoResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del acuerdo completado |
| `EstadoAcuerdoNombre` | `string` | Siempre "Completado" en este response |
| `FechaFinReal` | `DateTime` | Timestamp UTC del momento de completado |

**Wrapping:** `ServiceResponse<CompletarAcuerdoResultDto>`
**HTTP Status:** 200 OK

---

### 3.15 CancelarAcuerdoResultDto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Dtos/CancelarAcuerdoResultDto.cs`

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `Id` | `Guid` | ID del acuerdo cancelado |
| `EstadoAcuerdoNombre` | `string` | Siempre "Cancelado" en este response |
| `FechaFinReal` | `DateTime` | Timestamp UTC del momento de cancelacion |
| `NecesidadEstadoNombre` | `string` | Siempre "Abierta" en este response (la necesidad vuelve a Abierta) |

**Wrapping:** `ServiceResponse<CancelarAcuerdoResultDto>`
**HTTP Status:** 200 OK

---

## 4. Nuevas Constantes ServiceResponseMessageType

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`
**Accion:** Agregar las siguientes constantes al archivo existente.

### 4.1 Constantes a agregar

```csharp
// Validation (1000-1999)
// NUEVA: 1013 - URL invalida (ej: urlRecurso en entregable)
public const string Validation_InvalidUrl = "1013";

// NotFound (2000-2999)
// NUEVAS: 2011, 2012, 2013 - Entidades nuevas de esta feature
public const string NotFound_Acuerdo = "2011";
public const string NotFound_Milestone = "2012";
public const string NotFound_Entregable = "2013";

// Business Rules (4000-4999)
// NUEVAS: 4007 a 4013 - Reglas de negocio especificas de acuerdos
// Nota: los codigos 4001-4006 ya existen en el archivo actual
public const string BusinessRule_PropuestaNotAcceptable = "4007";    // Propuesta no esta en Pendiente
public const string BusinessRule_AcuerdoAlreadyExists = "4008";      // Ya existe acuerdo activo para la necesidad
public const string BusinessRule_MilestoneImporteExceeded = "4009";  // Suma milestones > ImporteTotalPactado
public const string BusinessRule_AcuerdoNotActive = "4010";          // Acuerdo no esta en estado Activo
public const string BusinessRule_MilestoneCompleted = "4011";        // Milestone ya fue completado
public const string BusinessRule_MilestoneHasEntregables = "4012";   // Milestone tiene entregables asociados
public const string BusinessRule_EntregableNotReviewable = "4013";   // Entregable no esta en estado Entregado
```

### 4.2 Estado actual del archivo (para referencia)

El archivo existente (`ServiceResponseMessageType.cs`) ya contiene:
- `Validation_Required` = "1001", `Validation_MaxLength` = "1002", `Validation_InvalidEmail` = "1003"
- `Validation_InvalidRange` = "1009", `Validation_ForeignKeyNotFound` = "1010"
- `Validation_MinLength` = "1011", `Validation_InvalidDate` = "1012"
- `NotFound_Propuesta` = "2010" (ultimo NotFound existente)
- `Auth_Unauthorized` = "3001", `Auth_Forbidden` = "3002"
- Business rules hasta `BusinessRule_NoProfessionalProfile` = "4006"
- `Internal_UnexpectedError` = "5000", `Internal_DatabaseError` = "5001"

---

## 5. Validadores (FluentValidation)

### 5.1 AceptarPropuestaCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/AceptarPropuestaCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `TituloInterno` | `NotEmpty()` | El titulo interno es obligatorio | `Validation_Required` = "1001" |
| `TituloInterno` | `MaximumLength(200)` | El titulo interno no puede superar los 200 caracteres | `Validation_MaxLength` = "1002" |
| `FechaInicio` | `NotEqual(default(DateTime))` (implicito: no puede ser fecha vacia) | La fecha de inicio es obligatoria | `Validation_Required` = "1001" |
| `FechaFinPrevista` | `.Must((cmd, fecha) => !cmd.FechaFinPrevista.HasValue || cmd.FechaFinPrevista > cmd.FechaInicio).When(x => x.FechaFinPrevista.HasValue)` | La fecha de fin prevista debe ser posterior a la fecha de inicio | `Validation_InvalidDate` = "1012" |
| `PropuestaId` | `MustAsync`: propuesta existe y esta en estado Pendiente | La propuesta no existe o no esta en estado Pendiente | `BusinessRule_PropuestaNotAcceptable` = "4007" |
| `x` (nivel objeto) | `MustAsync`: no existe acuerdo activo para la NecesidadId | Ya existe un acuerdo activo para esta necesidad | `BusinessRule_AcuerdoAlreadyExists` = "4008" |

**Nota:** Las reglas `MustAsync` usan `IRequestCacheService` via los services inyectados en el validator para evitar queries duplicados con el handler (patron ADR-006).

**Dependencias del constructor:**
- `IPropuestaCrowdsourcingService _propuestaService`
- `IAcuerdoCrowdsourcingService _acuerdoService`

---

### 5.2 RechazarPropuestaCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/RechazarPropuestaCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Motivo` | `MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Motivo))` | El motivo no puede superar los 500 caracteres | `Validation_MaxLength` = "1002" |
| `PropuestaId` | `MustAsync`: propuesta existe y esta en estado Pendiente | Solo se pueden rechazar propuestas en estado Pendiente | `BusinessRule_PropuestaNotAcceptable` = "4007" |

**Dependencias del constructor:**
- `IPropuestaCrowdsourcingService _propuestaService`

---

### 5.3 CreateMilestoneCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CreateMilestoneCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Titulo` | `NotEmpty()` | El titulo es obligatorio | `Validation_Required` = "1001" |
| `Titulo` | `MinimumLength(3)` | El titulo debe tener al menos 3 caracteres | `Validation_MinLength` = "1011" |
| `Titulo` | `MaximumLength(200)` | El titulo no puede superar los 200 caracteres | `Validation_MaxLength` = "1002" |
| `Descripcion` | `MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Descripcion))` | La descripcion no puede superar los 1000 caracteres | `Validation_MaxLength` = "1002" |
| `ImporteParcial` | `GreaterThan(0)` | El importe parcial debe ser mayor a 0 | `Validation_Required` = "1001" |
| `FechaLimite` | `MustAsync` (>= acuerdo.FechaInicio) `.When(x => x.FechaLimite.HasValue)` | La fecha limite no puede ser anterior a la fecha de inicio del acuerdo | `Validation_InvalidDate` = "1012" |
| `x` (nivel objeto) | `MustAsync`: suma de importes + nuevo <= ImporteTotalPactado | La suma de importes de milestones supera el importe total pactado | `BusinessRule_MilestoneImporteExceeded` = "4009" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingService _acuerdoService`
- `IAcuerdoCrowdsourcingMilestoneService _milestoneService`

---

### 5.4 UpdateMilestoneCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/UpdateMilestoneCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Titulo` | `NotEmpty()` | El titulo es obligatorio | `Validation_Required` = "1001" |
| `Titulo` | `MinimumLength(3)` | El titulo debe tener al menos 3 caracteres | `Validation_MinLength` = "1011" |
| `Titulo` | `MaximumLength(200)` | El titulo no puede superar los 200 caracteres | `Validation_MaxLength` = "1002" |
| `Descripcion` | `MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Descripcion))` | La descripcion no puede superar los 1000 caracteres | `Validation_MaxLength` = "1002" |
| `ImporteParcial` | `GreaterThan(0)` | El importe parcial debe ser mayor a 0 | `Validation_Required` = "1001" |
| `FechaLimite` | `MustAsync` (>= acuerdo.FechaInicio) `.When(x => x.FechaLimite.HasValue)` | La fecha limite no puede ser anterior a la fecha de inicio del acuerdo | `Validation_InvalidDate` = "1012" |
| `MilestoneId` | `MustAsync`: milestone existe, pertenece al acuerdo y `FechaCompletado IS NULL` | No se puede editar un milestone completado | `BusinessRule_MilestoneCompleted` = "4011" |
| `x` (nivel objeto) | `MustAsync`: suma excluyendo el milestone actual + nuevo importe <= ImporteTotalPactado | La suma de importes de milestones supera el importe total pactado | `BusinessRule_MilestoneImporteExceeded` = "4009" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingService _acuerdoService`
- `IAcuerdoCrowdsourcingMilestoneService _milestoneService`

---

### 5.5 DeleteMilestoneCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/DeleteMilestoneCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `MilestoneId` | `MustAsync`: milestone existe y pertenece al acuerdo | Milestone no encontrado | `NotFound_Milestone` = "2012" |
| `MilestoneId` | `MustAsync`: `FechaCompletado IS NULL` | No se puede eliminar un milestone completado | `BusinessRule_MilestoneCompleted` = "4011" |
| `MilestoneId` | `MustAsync`: no tiene entregables asociados | No se puede eliminar un milestone con entregables asociados | `BusinessRule_MilestoneHasEntregables` = "4012" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingMilestoneService _milestoneService`
- `IAcuerdoCrowdsourcingEntregableService _entregableService`

---

### 5.6 CreateEntregableCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CreateEntregableCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Titulo` | `NotEmpty()` | El titulo es obligatorio | `Validation_Required` = "1001" |
| `Titulo` | `MinimumLength(3)` | El titulo debe tener al menos 3 caracteres | `Validation_MinLength` = "1011" |
| `Titulo` | `MaximumLength(200)` | El titulo no puede superar los 200 caracteres | `Validation_MaxLength` = "1002" |
| `Descripcion` | `MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Descripcion))` | La descripcion no puede superar los 1000 caracteres | `Validation_MaxLength` = "1002" |
| `UrlRecurso` | `.Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.UrlRecurso))` | Debe ser una URL valida (ej: Dropbox, Drive, WeTransfer) | `Validation_InvalidUrl` = "1013" |
| `MilestoneId` | `MustAsync`: si presente, el milestone pertenece al mismo acuerdo | El milestone no pertenece a este acuerdo | `Validation_ForeignKeyNotFound` = "1010" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingMilestoneService _milestoneService`

---

### 5.7 AprobarEntregableCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/AprobarEntregableCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Comentario` | `MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Comentario))` | El comentario no puede superar los 500 caracteres | `Validation_MaxLength` = "1002" |

**Nota:** La validacion del estado del entregable (debe estar en Entregado) se realiza en el Handler despues de la validacion del formulario, dado que requiere cargar la entidad. Retorna `BusinessRule_EntregableNotReviewable` = "4013".

---

### 5.8 RechazarEntregableCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/RechazarEntregableCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Comentario` | `NotEmpty()` | El comentario es obligatorio al rechazar un entregable | `Validation_Required` = "1001" |
| `Comentario` | `MinimumLength(10)` | Minimo 10 caracteres explicando que debe corregirse | `Validation_MinLength` = "1011" |
| `Comentario` | `MaximumLength(500)` | El comentario no puede superar los 500 caracteres | `Validation_MaxLength` = "1002" |

---

### 5.9 CompletarAcuerdoCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CompletarAcuerdoCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `AcuerdoId` | `MustAsync`: acuerdo existe y esta en estado Activo | El acuerdo no esta activo | `BusinessRule_AcuerdoNotActive` = "4010" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingService _acuerdoService`

---

### 5.10 CancelarAcuerdoCommandValidator

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Features/Acuerdos/Validators/CancelarAcuerdoCommandValidator.cs`

| Campo | Regla | Mensaje | ErrorCode |
|-------|-------|---------|-----------|
| `Motivo` | `NotEmpty()` | El motivo de cancelacion es obligatorio | `Validation_Required` = "1001" |
| `Motivo` | `MinimumLength(20)` | El motivo debe tener al menos 20 caracteres | `Validation_MinLength` = "1011" |
| `Motivo` | `MaximumLength(1000)` | El motivo no puede superar los 1000 caracteres | `Validation_MaxLength` = "1002" |
| `AcuerdoId` | `MustAsync`: acuerdo existe y esta en estado Activo | El acuerdo no esta activo | `BusinessRule_AcuerdoNotActive` = "4010" |

**Dependencias del constructor:**
- `IAcuerdoCrowdsourcingService _acuerdoService`

---

## 6. AutoMapper Profiles

### 6.1 AcuerdoProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/AcuerdoProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| `AcuerdoCrowdsourcing` | `AcuerdoDto` | Campos calculados (`ImporteAsignado`, `PorcentajeAsignado`, `MiRol`, `Timeline`) se ignoran en el mapping y se asignan manualmente en el handler. `FechaInicio` mapea desde `AcuerdoCrowdsourcing.FechaInicio`. Navegacion `Artista`, `Profesional`, `Necesidad` requieren `.Ignore()` y asignacion manual en handler. |
| `AcuerdoCrowdsourcing` | `AcuerdoArtistaDto` | No se mapea directamente; el handler construye este sub-DTO desde la navegacion `acuerdo.Artista`. |
| `AcuerdoCrowdsourcing` | `AcuerdoNecesidadDto` | Idem anterior; handler construye desde `acuerdo.Necesidad`. |
| `AceptarPropuestaCommand` | `AcuerdoCrowdsourcing` | Ignora: `Id`, `ArtistaId`, `UserIdProveedor`, `PerfilProfesionalId`, `PropuestaId`, `NecesidadId`, `ImporteTotalPactado`, `MonedaId`, `EstadoAcuerdoId`, `FechaCreacion`, `FechaActualizacion`, `FechaFinReal`, `Milestones`, `Entregables`, `Conversaciones`, `Valoraciones`, `Necesidad`, `Propuesta`. Solo mapea: `TituloInterno`, `FechaInicio`, `FechaFinPrevista`. |

**Reglas especificas:**

```
CreateMap<AcuerdoCrowdsourcing, AcuerdoDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Value))
    .ForMember(dest => dest.EstadoAcuerdoNombre, opt => opt.Ignore())   // Resuelto en handler
    .ForMember(dest => dest.MonedaNombre, opt => opt.Ignore())           // Resuelto en handler
    .ForMember(dest => dest.Artista, opt => opt.Ignore())               // Resuelto en handler
    .ForMember(dest => dest.Profesional, opt => opt.Ignore())           // Resuelto en handler
    .ForMember(dest => dest.Necesidad, opt => opt.Ignore())             // Resuelto en handler
    .ForMember(dest => dest.ImporteAsignado, opt => opt.Ignore())       // Calculado en handler
    .ForMember(dest => dest.PorcentajeAsignado, opt => opt.Ignore())    // Calculado en handler
    .ForMember(dest => dest.MiRol, opt => opt.Ignore())                 // Calculado en handler
    .ForMember(dest => dest.Timeline, opt => opt.Ignore())              // Construido en handler
    .ForMember(dest => dest.Milestones, opt => opt.MapFrom(src => src.Milestones.OrderBy(m => m.Orden)));
```

---

### 6.2 MilestoneProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/MilestoneProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| `AcuerdoCrowdsourcingMilestone` | `MilestoneDto` | `PorcentajeParcial` se ignora en mapping y se calcula en el handler pasando `ImporteTotalPactado`. `Entregables` mapea los entregables del milestone. |
| `CreateMilestoneCommand` | `AcuerdoCrowdsourcingMilestone` | Ignora: `Id`, `AcuerdoId`, `Orden`, `PorcentajeParcial`, `FechaCompletado`, `FechaCreacion`, `Acuerdo`. Solo mapea: `Titulo`, `Descripcion`, `ImporteParcial`, `FechaLimite`. |
| `UpdateMilestoneCommand` | `AcuerdoCrowdsourcingMilestone` | Mismo patron que `CreateMilestoneCommand`. |

**Reglas especificas:**

```
CreateMap<AcuerdoCrowdsourcingMilestone, MilestoneDto>()
    .ForMember(dest => dest.PorcentajeParcial, opt => opt.Ignore())  // Calculado en handler
    .ForMember(dest => dest.Entregables, opt => opt.MapFrom(src => src.Entregables));
```

**Nota sobre `PorcentajeParcial` en la entidad:** La entidad `AcuerdoCrowdsourcingMilestone` ya tiene el campo `PorcentajeParcial` (nullable decimal), pero segun las reglas el calculo siempre se hace en el handler para garantizar consistencia con el `ImporteTotalPactado` actual. No se confiar en el valor almacenado.

---

### 6.3 EntregableProfile

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Mapping/EntregableProfile.cs`

| Source | Destination | Notas |
|--------|-------------|-------|
| `AcuerdoCrowdsourcingEntregable` | `EntregableDto` | `EstadoEntregableNombre` se ignora en mapping y se resuelve en handler via la maestra. `FechaEntrega` de la entidad no se expone directamente en el DTO (el DTO usa `FechaCreacion`). |
| `CreateEntregableCommand` | `AcuerdoCrowdsourcingEntregable` | Ignora: `Id`, `AcuerdoId`, `MilestoneId`, `EstadoEntregableId`, `FechaEntrega`, `FechaAprobacion`, `ComentarioAprobacion`, `FechaCreacion`, `Acuerdo`, `Milestone`. Solo mapea: `Titulo`, `Descripcion`, `UrlRecurso`. |

**Reglas especificas:**

```
CreateMap<AcuerdoCrowdsourcingEntregable, EntregableDto>()
    .ForMember(dest => dest.EstadoEntregableNombre, opt => opt.Ignore())  // Resuelto en handler
    .ForMember(dest => dest.ComentarioRechazo, opt => opt.Ignore());      // No existe en entidad actual; ver nota

CreateMap<CreateEntregableCommand, AcuerdoCrowdsourcingEntregable>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.AcuerdoId, opt => opt.Ignore())
    .ForMember(dest => dest.MilestoneId, opt => opt.Ignore())
    .ForMember(dest => dest.EstadoEntregableId, opt => opt.Ignore())
    .ForMember(dest => dest.FechaEntrega, opt => opt.Ignore())
    .ForMember(dest => dest.FechaAprobacion, opt => opt.Ignore())
    .ForMember(dest => dest.ComentarioAprobacion, opt => opt.Ignore())
    .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
    .ForMember(dest => dest.Acuerdo, opt => opt.Ignore())
    .ForMember(dest => dest.Milestone, opt => opt.Ignore());
```

**NOTA IMPORTANTE - Gap en la entidad:** La entidad `AcuerdoCrowdsourcingEntregable` existente NO tiene los campos `ComentarioRechazo` ni `FechaActualizacion`. Estos son requeridos por el contrato. Se necesita una migracion para agregarlos. El implementador debe agregar estos campos a la entidad antes de implementar `RechazarEntregableCommand`.

---

## 7. Mapping de Controllers

### 7.1 PropuestasCrowdsourcingController (existente - ampliar)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/PropuestasCrowdsourcingController.cs`
**Accion:** Agregar dos nuevos endpoints a este controller existente.

#### POST /{id}/aceptar

```
[HttpPost("{id}/aceptar")]
[ProducesResponseType(typeof(ServiceResponse<AceptarPropuestaResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<AceptarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

**Logica del controller:**
1. Extraer `userId` de `User.FindFirstValue(ClaimTypes.NameIdentifier)`. Si null, return Unauthorized.
2. Crear `AceptarPropuestaCommand` con `PropuestaId = id`, `UserId = userId`, y el body mapeado.
3. Enviar via `_mediator.Send(command)`.
4. Si `response.HasErrors`:
   - `NotFound_Propuesta` ("2010") -> `NotFound(response)`
   - `Auth_Forbidden` ("3002") -> `StatusCode(403, response)`
   - otros -> `BadRequest(response)`
5. Si exito -> `CreatedAtAction(nameof(GetAcuerdoById), new { id = response.Data!.AcuerdoId }, response)`
   (asumiendo que el AcuerdosCrowdsourcingController existe con un metodo `GetAcuerdoById`)

#### PATCH /{id}/rechazar

```
[HttpPatch("{id}/rechazar")]
[ProducesResponseType(typeof(ServiceResponse<RechazarPropuestaResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<RechazarPropuestaResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

**Logica del controller:**
1. Extraer `userId`. Si null, Unauthorized.
2. Crear `RechazarPropuestaCommand` con `PropuestaId = id`, `UserId = userId`, `Motivo = body.Motivo`.
3. Enviar via mediator.
4. Routing de errores igual que en `/aceptar` (NotFound, Forbidden, BadRequest).
5. Si exito -> `Ok(response)`.

---

### 7.2 AcuerdosCrowdsourcingController (nuevo)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/AcuerdosCrowdsourcingController.cs`
**Route:** `[Route("api/crowdsourcing/acuerdos")]`
**Auth:** `[Authorize]` a nivel de controller.

#### GET /{id}

```
[HttpGet("{id}")]
[ProducesResponseType(typeof(ServiceResponse<AcuerdoDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

**Routing de errores:** `NotFound_Acuerdo` ("2011") -> 404; `Auth_Forbidden` ("3002") -> 403; otros -> 500.

#### POST /{acuerdoId}/milestones

```
[HttpPost("{acuerdoId}/milestones")]
[ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** `NotFound_Acuerdo` -> 404; `Auth_Forbidden` -> 403; otros -> BadRequest. Exito -> 201.

#### PUT /{acuerdoId}/milestones/{id}

```
[HttpPut("{acuerdoId}/milestones/{id}")]
[ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<MilestoneCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** `NotFound_Milestone` ("2012") -> 404; `Auth_Forbidden` -> 403; otros -> BadRequest. Exito -> 200.

#### DELETE /{acuerdoId}/milestones/{id}

```
[HttpDelete("{acuerdoId}/milestones/{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** `NotFound_Milestone` -> 404; `Auth_Forbidden` -> 403; otros -> BadRequest. Exito -> 204 NoContent.

#### POST /{acuerdoId}/entregables

```
[HttpPost("{acuerdoId}/entregables")]
[ProducesResponseType(typeof(ServiceResponse<EntregableCreatedResultDto>), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ServiceResponse<EntregableCreatedResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** `NotFound_Acuerdo` -> 404; `Auth_Forbidden` -> 403; otros -> BadRequest. Exito -> 201.

#### PATCH /{id}/completar

```
[HttpPatch("{id}/completar")]
[ProducesResponseType(typeof(ServiceResponse<CompletarAcuerdoResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<CompletarAcuerdoResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Body:** Ninguno (no requiere `[FromBody]`).

#### PATCH /{id}/cancelar

```
[HttpPatch("{id}/cancelar")]
[ProducesResponseType(typeof(ServiceResponse<CancelarAcuerdoResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<CancelarAcuerdoResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

---

### 7.3 EntregablesCrowdsourcingController (nuevo)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.WebApi/Controllers/EntregablesCrowdsourcingController.cs`
**Route:** `[Route("api/crowdsourcing/entregables")]`
**Auth:** `[Authorize]` a nivel de controller.

#### PATCH /{id}/aprobar

```
[HttpPatch("{id}/aprobar")]
[ProducesResponseType(typeof(ServiceResponse<AprobarEntregableResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<AprobarEntregableResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** `NotFound_Entregable` ("2013") -> 404; `Auth_Forbidden` -> 403; otros -> BadRequest.

#### PATCH /{id}/rechazar

```
[HttpPatch("{id}/rechazar")]
[ProducesResponseType(typeof(ServiceResponse<RechazarEntregableResultDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ServiceResponse<RechazarEntregableResultDto>), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

**Routing de errores:** Igual que `/aprobar`.

---

## 8. Interfaces de Servicio (nuevas)

### 8.1 IAcuerdoCrowdsourcingService

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync(Guid id, CancellationToken ct)` | `Task<AcuerdoCrowdsourcing?>` | Carga el acuerdo con Include(Milestones, Entregables, Necesidad, Propuesta). Usado por handler y validators. |
| `GetByIdWithParticipantesAsync(Guid id, CancellationToken ct)` | `Task<AcuerdoCrowdsourcing?>` | Carga el acuerdo con navegacion a Artista y PerfilProfesional para construir el DTO detalle. |
| `CreateAsync(AcuerdoCrowdsourcing entity, string userId, Guid propuestaId, CancellationToken ct)` | `Task<Guid>` | Crea el acuerdo en una transaccion: crea acuerdo, actualiza propuesta a Aceptada, actualiza otras propuestas pendientes a Rechazada, actualiza necesidad a En Progreso, crea ConversacionCrowdsourcing. Retorna el ID del acuerdo creado. |
| `CompletarAsync(AcuerdoCrowdsourcing acuerdo, CancellationToken ct)` | `Task` | Actualiza estado a Completado, registra FechaFinReal, actualiza necesidad a Cerrada. |
| `CancelarAsync(AcuerdoCrowdsourcing acuerdo, string canceladoPor, string motivo, CancellationToken ct)` | `Task` | Actualiza estado a Cancelado, registra FechaFinReal, CanceladoPor, MotivoCancelacion, actualiza necesidad a Abierta. |
| `ExisteAcuerdoActivoParaNecesidadAsync(Guid necesidadId, CancellationToken ct)` | `Task<bool>` | Verifica si existe algun AcuerdoCrowdsourcing con EstadoAcuerdoId == Activo para esa NecesidadId. Usado por validator de AceptarPropuesta. |
| `EsParticipanteAsync(Guid acuerdoId, string userId, CancellationToken ct)` | `Task<bool>` | Verifica si el userId es artista o profesional del acuerdo. |

---

### 8.2 IAcuerdoCrowdsourcingMilestoneService

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingMilestoneService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync(Guid milestoneId, CancellationToken ct)` | `Task<AcuerdoCrowdsourcingMilestone?>` | Carga el milestone con navegacion al acuerdo padre. |
| `CreateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)` | `Task<Guid>` | Crea el milestone. Asigna Orden = MAX(Orden)+1 o 1. |
| `UpdateAsync(AcuerdoCrowdsourcingMilestone entity, CancellationToken ct)` | `Task` | Actualiza el milestone. |
| `DeleteAsync(Guid milestoneId, CancellationToken ct)` | `Task` | Elimina el milestone. |
| `GetImporteAsignadoAsync(Guid acuerdoId, CancellationToken ct)` | `Task<decimal>` | SUM(ImporteParcial) de todos los milestones del acuerdo. Usado por validators. |
| `GetImporteAsignadoExcluyendoAsync(Guid acuerdoId, Guid excludeMilestoneId, CancellationToken ct)` | `Task<decimal>` | SUM(ImporteParcial) excluyendo un milestone especifico. Usado por UpdateMilestoneValidator. |

---

### 8.3 IAcuerdoCrowdsourcingEntregableService

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IAcuerdoCrowdsourcingEntregableService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync(Guid entregableId, CancellationToken ct)` | `Task<AcuerdoCrowdsourcingEntregable?>` | Carga el entregable con navegacion al acuerdo padre. |
| `CreateAsync(AcuerdoCrowdsourcingEntregable entity, CancellationToken ct)` | `Task<Guid>` | Crea el entregable con EstadoEntregableId = 1 (Entregado). |
| `AprobarAsync(AcuerdoCrowdsourcingEntregable entity, string? comentario, CancellationToken ct)` | `Task` | Actualiza estado a Aprobado (2), registra FechaAprobacion = UtcNow, ComentarioAprobacion. |
| `RechazarAsync(AcuerdoCrowdsourcingEntregable entity, string comentario, CancellationToken ct)` | `Task` | Actualiza estado a Rechazado (3), guarda ComentarioRechazo. |
| `TodosAprobadosEnMilestoneAsync(Guid milestoneId, CancellationToken ct)` | `Task<bool>` | Verifica si todos los entregables del milestone estan en estado Aprobado (2). |
| `MilestonePertenecealAcuerdoAsync(Guid milestoneId, Guid acuerdoId, CancellationToken ct)` | `Task<bool>` | Verifica que el milestone existe y pertenece al acuerdo especificado. Usado por CreateEntregableValidator. |
| `TieneEntregablesAsync(Guid milestoneId, CancellationToken ct)` | `Task<bool>` | Verifica si el milestone tiene entregables. Usado por DeleteMilestoneValidator. |

---

## 9. Documentacion OpenAPI / Swagger

### POST /api/crowdsourcing/propuestas/{id}/aceptar

- **Summary:** Aceptar propuesta y crear acuerdo de trabajo
- **Description:** Operacion transaccional. Acepta la propuesta seleccionada, rechaza automaticamente las demas propuestas pendientes de la misma necesidad, avanza el estado de la necesidad a "En Progreso" y crea una conversacion de comunicacion vinculada al acuerdo.
- **Parameters:** `id` (path, Guid) - ID de la propuesta
- **Request Body:** `AceptarPropuestaRequest` (tituloInterno, fechaInicio, fechaFinPrevista)
- **Responses:**
  - `201 Created`: `ServiceResponse<AceptarPropuestaResultDto>` - Acuerdo creado correctamente
  - `400 Bad Request`: Propuesta no en estado Pendiente (4007) o ya existe acuerdo activo (4008) o validaciones de fechas
  - `401 Unauthorized`: Token invalido o expirado
  - `403 Forbidden`: El usuario no es el artista propietario de la necesidad (3002)
  - `404 Not Found`: Propuesta no existe (2010)
  - `500 Internal Server Error`: Error inesperado (5000)
- **Auth:** Bearer token requerido. Artista propietario de la necesidad vinculada a la propuesta.

---

### PATCH /api/crowdsourcing/propuestas/{id}/rechazar

- **Summary:** Rechazar propuesta individualmente
- **Description:** El artista rechaza una propuesta especifica en estado Pendiente. El motivo se almacena internamente pero el profesional NO puede verlo.
- **Parameters:** `id` (path, Guid) - ID de la propuesta
- **Request Body:** `RechazarPropuestaRequest` (motivo opcional, max 500 chars)
- **Responses:**
  - `200 OK`: `ServiceResponse<RechazarPropuestaResultDto>`
  - `400 Bad Request`: Propuesta no en estado Pendiente (4007) o motivo supera 500 chars
  - `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista propietario de la necesidad.

---

### GET /api/crowdsourcing/acuerdos/{id}

- **Summary:** Obtener detalle completo del acuerdo
- **Description:** Devuelve el acuerdo con milestones (entregables anidados), datos de participantes, campos calculados (importeAsignado, porcentajeAsignado, miRol) y timeline de actividad. Solo los dos participantes pueden acceder.
- **Parameters:** `id` (path, Guid) - ID del acuerdo
- **Responses:**
  - `200 OK`: `ServiceResponse<AcuerdoDto>`
  - `401 Unauthorized`, `403 Forbidden` (no participante), `404 Not Found` (2011), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Participante del acuerdo (artista o profesional).

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones

- **Summary:** Crear milestone en el acuerdo
- **Description:** El artista define una etapa de trabajo con importe parcial. La suma de importes de todos los milestones no puede superar el ImporteTotalPactado.
- **Parameters:** `acuerdoId` (path, Guid)
- **Request Body:** `CreateMilestoneRequest` (titulo, descripcion, importeParcial, fechaLimite)
- **Responses:**
  - `201 Created`: `ServiceResponse<MilestoneCreatedResultDto>`
  - `400 Bad Request`: Validaciones de campo (1001, 1011, 1002, 1012) o importe superado (4009) o acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden` (solo artista), `404 Not Found`, `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo.

---

### PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

- **Summary:** Editar milestone existente
- **Description:** El artista modifica los datos de un milestone que no este completado. Las reglas de suma de importes aplican excluyendo el milestone actual del calculo.
- **Parameters:** `acuerdoId` (path, Guid), `id` (path, Guid)
- **Request Body:** Identico a CreateMilestoneRequest
- **Responses:**
  - `200 OK`: `ServiceResponse<MilestoneCreatedResultDto>`
  - `400 Bad Request`: Validaciones, importe superado (4009), milestone completado (4011), acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden`, `404 Not Found` (2011 o 2012), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo.

---

### DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

- **Summary:** Eliminar milestone
- **Description:** El artista elimina un milestone que no tenga entregables asociados y que no este completado.
- **Parameters:** `acuerdoId` (path, Guid), `id` (path, Guid)
- **Request Body:** Ninguno
- **Responses:**
  - `204 No Content`: Milestone eliminado
  - `400 Bad Request`: Milestone tiene entregables (4012) o esta completado (4011) o acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo.

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/entregables

- **Summary:** Subir entregable al acuerdo
- **Description:** El profesional sube un entregable con URL externa (Dropbox, Drive, WeTransfer). El entregable se crea en estado "Entregado". El milestoneId es opcional.
- **Parameters:** `acuerdoId` (path, Guid)
- **Request Body:** `CreateEntregableRequest` (titulo, descripcion, urlRecurso, milestoneId)
- **Responses:**
  - `201 Created`: `ServiceResponse<EntregableCreatedResultDto>`
  - `400 Bad Request`: Validaciones (1001, 1011, 1002, 1013) o acuerdo no activo (4010) o milestone no pertenece al acuerdo (1010)
  - `401 Unauthorized`, `403 Forbidden` (solo profesional), `404 Not Found`, `500 Internal Server Error`
- **Auth:** Bearer token requerido. Profesional participante del acuerdo.

---

### PATCH /api/crowdsourcing/entregables/{id}/aprobar

- **Summary:** Aprobar entregable
- **Description:** El artista aprueba un entregable en estado "Entregado". El comentario es opcional. La respuesta incluye `todosAprobadosEnMilestone` para que el frontend sugiera marcar el milestone como completado.
- **Parameters:** `id` (path, Guid) - ID del entregable
- **Request Body:** `AprobarEntregableRequest` (comentario opcional, max 500)
- **Responses:**
  - `200 OK`: `ServiceResponse<AprobarEntregableResultDto>`
  - `400 Bad Request`: Comentario supera 500 chars (1002) o entregable no en estado Entregado (4013) o acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden` (solo artista), `404 Not Found` (2013), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo al que pertenece el entregable.

---

### PATCH /api/crowdsourcing/entregables/{id}/rechazar

- **Summary:** Rechazar entregable
- **Description:** El artista rechaza un entregable en estado "Entregado" con un comentario obligatorio (min 10 chars) explicando que debe corregirse. El profesional puede subir una nueva version.
- **Parameters:** `id` (path, Guid) - ID del entregable
- **Request Body:** `RechazarEntregableRequest` (comentario obligatorio, min 10, max 500)
- **Responses:**
  - `200 OK`: `ServiceResponse<RechazarEntregableResultDto>`
  - `400 Bad Request`: Comentario invalido (1001, 1011, 1002) o entregable no en estado Entregado (4013) o acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden` (solo artista), `404 Not Found` (2013), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo al que pertenece el entregable.

---

### PATCH /api/crowdsourcing/acuerdos/{id}/completar

- **Summary:** Completar acuerdo de trabajo
- **Description:** El artista marca el acuerdo como completado. Se registra FechaFinReal y la necesidad pasa a "Cerrada". Si hay entregables en estado Entregado (pendientes de revision), la operacion procede igualmente (no bloqueante).
- **Parameters:** `id` (path, Guid) - ID del acuerdo
- **Request Body:** Ninguno
- **Responses:**
  - `200 OK`: `ServiceResponse<CompletarAcuerdoResultDto>`
  - `400 Bad Request`: Acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden` (solo artista), `404 Not Found` (2011), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Artista participante del acuerdo.

---

### PATCH /api/crowdsourcing/acuerdos/{id}/cancelar

- **Summary:** Cancelar acuerdo de trabajo
- **Description:** Cualquiera de los dos participantes puede cancelar el acuerdo. Accion irreversible. Requiere motivo obligatorio (min 20, max 1000 chars). La necesidad vuelve a estado "Abierta".
- **Parameters:** `id` (path, Guid) - ID del acuerdo
- **Request Body:** `CancelarAcuerdoRequest` (motivo obligatorio, min 20, max 1000)
- **Responses:**
  - `200 OK`: `ServiceResponse<CancelarAcuerdoResultDto>`
  - `400 Bad Request`: Motivo invalido (1001, 1011, 1002) o acuerdo no activo (4010)
  - `401 Unauthorized`, `403 Forbidden` (no participante), `404 Not Found` (2011), `500 Internal Server Error`
- **Auth:** Bearer token requerido. Participante del acuerdo (artista o profesional).

---

## 10. Tabla de Errores por Endpoint

| Endpoint | HTTP | ErrorCode | Constante | Causa |
|----------|------|-----------|-----------|-------|
| POST /propuestas/{id}/aceptar | 400 | 1001 | `Validation_Required` | tituloInterno vacio |
| POST /propuestas/{id}/aceptar | 400 | 1002 | `Validation_MaxLength` | tituloInterno > 200 |
| POST /propuestas/{id}/aceptar | 400 | 1012 | `Validation_InvalidDate` | fechaFinPrevista <= fechaInicio |
| POST /propuestas/{id}/aceptar | 400 | 4007 | `BusinessRule_PropuestaNotAcceptable` | Propuesta no en estado Pendiente |
| POST /propuestas/{id}/aceptar | 400 | 4008 | `BusinessRule_AcuerdoAlreadyExists` | Ya existe acuerdo activo para la necesidad |
| POST /propuestas/{id}/aceptar | 403 | 3002 | `Auth_Forbidden` | No es el artista propietario |
| POST /propuestas/{id}/aceptar | 404 | 2010 | `NotFound_Propuesta` | Propuesta no existe |
| PATCH /propuestas/{id}/rechazar | 400 | 1002 | `Validation_MaxLength` | Motivo > 500 chars |
| PATCH /propuestas/{id}/rechazar | 400 | 4007 | `BusinessRule_PropuestaNotAcceptable` | Propuesta no en estado Pendiente |
| PATCH /propuestas/{id}/rechazar | 403 | 3002 | `Auth_Forbidden` | No es el artista propietario |
| PATCH /propuestas/{id}/rechazar | 404 | 2010 | `NotFound_Propuesta` | Propuesta no existe |
| GET /acuerdos/{id} | 403 | 3002 | `Auth_Forbidden` | No es participante del acuerdo |
| GET /acuerdos/{id} | 404 | 2011 | `NotFound_Acuerdo` | Acuerdo no existe |
| POST /acuerdos/{id}/milestones | 400 | 1001 | `Validation_Required` | Titulo vacio o importeParcial <= 0 |
| POST /acuerdos/{id}/milestones | 400 | 1011 | `Validation_MinLength` | Titulo < 3 chars |
| POST /acuerdos/{id}/milestones | 400 | 1002 | `Validation_MaxLength` | Titulo > 200 o descripcion > 1000 |
| POST /acuerdos/{id}/milestones | 400 | 1012 | `Validation_InvalidDate` | fechaLimite < acuerdo.FechaInicio |
| POST /acuerdos/{id}/milestones | 400 | 4009 | `BusinessRule_MilestoneImporteExceeded` | Suma milestones > ImporteTotalPactado |
| POST /acuerdos/{id}/milestones | 400 | 4010 | `BusinessRule_AcuerdoNotActive` | Acuerdo no esta Activo |
| PUT /acuerdos/{id}/milestones/{id} | 400 | 4011 | `BusinessRule_MilestoneCompleted` | Milestone ya completado |
| DELETE /acuerdos/{id}/milestones/{id} | 400 | 4011 | `BusinessRule_MilestoneCompleted` | Milestone ya completado |
| DELETE /acuerdos/{id}/milestones/{id} | 400 | 4012 | `BusinessRule_MilestoneHasEntregables` | Milestone tiene entregables |
| DELETE /acuerdos/{id}/milestones/{id} | 404 | 2012 | `NotFound_Milestone` | Milestone no existe |
| POST /acuerdos/{id}/entregables | 400 | 1013 | `Validation_InvalidUrl` | URL del recurso invalida |
| POST /acuerdos/{id}/entregables | 400 | 1010 | `Validation_ForeignKeyNotFound` | MilestoneId no pertenece al acuerdo |
| POST /acuerdos/{id}/entregables | 400 | 4010 | `BusinessRule_AcuerdoNotActive` | Acuerdo no esta Activo |
| PATCH /entregables/{id}/aprobar | 400 | 4013 | `BusinessRule_EntregableNotReviewable` | Entregable no en estado Entregado |
| PATCH /entregables/{id}/aprobar | 404 | 2013 | `NotFound_Entregable` | Entregable no existe |
| PATCH /entregables/{id}/rechazar | 400 | 1001 | `Validation_Required` | Comentario vacio |
| PATCH /entregables/{id}/rechazar | 400 | 1011 | `Validation_MinLength` | Comentario < 10 chars |
| PATCH /entregables/{id}/rechazar | 400 | 4013 | `BusinessRule_EntregableNotReviewable` | Entregable no en estado Entregado |
| PATCH /entregables/{id}/rechazar | 404 | 2013 | `NotFound_Entregable` | Entregable no existe |
| PATCH /acuerdos/{id}/completar | 400 | 4010 | `BusinessRule_AcuerdoNotActive` | Acuerdo no esta Activo |
| PATCH /acuerdos/{id}/completar | 404 | 2011 | `NotFound_Acuerdo` | Acuerdo no existe |
| PATCH /acuerdos/{id}/cancelar | 400 | 1001 | `Validation_Required` | Motivo vacio |
| PATCH /acuerdos/{id}/cancelar | 400 | 1011 | `Validation_MinLength` | Motivo < 20 chars |
| PATCH /acuerdos/{id}/cancelar | 400 | 1002 | `Validation_MaxLength` | Motivo > 1000 chars |
| PATCH /acuerdos/{id}/cancelar | 400 | 4010 | `BusinessRule_AcuerdoNotActive` | Acuerdo no esta Activo |
| PATCH /acuerdos/{id}/cancelar | 404 | 2011 | `NotFound_Acuerdo` | Acuerdo no existe |

---

## 11. Gaps Identificados en Entidades Existentes

Los siguientes campos estan en el contrato pero NO existen en las entidades del dominio actuales. Se requieren migraciones antes de implementar los respectivos commands.

| Entidad | Campo faltante | Tipo | Requerido por |
|---------|---------------|------|---------------|
| `AcuerdoCrowdsourcing` | `MotivoCancelacion` | `string?` | `CancelarAcuerdoCommand` |
| `AcuerdoCrowdsourcing` | `CanceladoPor` | `string?` | `CancelarAcuerdoCommand` |
| `AcuerdoCrowdsourcingEntregable` | `ComentarioRechazo` | `string?` | `RechazarEntregableCommand` |
| `AcuerdoCrowdsourcingEntregable` | `FechaActualizacion` | `DateTime?` | Timeline del `GetAcuerdoByIdQuery` |
| `AcuerdoCrowdsourcingMilestone` | Navigation `Entregables` | `ICollection<AcuerdoCrowdsourcingEntregable>` | `MilestoneDto.Entregables` en GET detalle |

**Nota sobre la entidad `AcuerdoCrowdsourcing`:** El campo `FechaInicio` es nullable en la entidad actual (`DateTime?`) pero el contrato lo requiere como obligatorio al crear. El handler de `AceptarPropuestaCommand` debe garantizar que siempre se asigne (default: `DateTime.UtcNow.Date`).

**Nota sobre el campo `MaestraEstadoAcuerdo` y `MaestraEstadoEntregable`:** Las maestras de estado deben existir en la base de datos antes de las migraciones. Si aun no existen como tablas, se necesita una migracion `AddMaestrasEstadoAcuerdoEntregable` previa.

---

## 12. Archivos a Crear

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   └── Constants/
│       └── ServiceResponseMessageType.cs        (MODIFICAR: agregar 1013, 2011-2013, 4007-4013)
│
├── WePlayRises.Crowdsourcing.Application/
│   ├── Dtos/
│   │   ├── AceptarPropuestaResultDto.cs          (CREAR)
│   │   ├── RechazarPropuestaResultDto.cs         (CREAR)
│   │   ├── AcuerdoDto.cs                         (CREAR)
│   │   ├── AcuerdoArtistaDto.cs                  (CREAR)
│   │   ├── AcuerdoProfesionalDto.cs              (CREAR)
│   │   ├── AcuerdoNecesidadDto.cs                (CREAR)
│   │   ├── AcuerdoTimelineEventoDto.cs           (CREAR)
│   │   ├── MilestoneDto.cs                       (CREAR)
│   │   ├── EntregableDto.cs                      (CREAR)
│   │   ├── MilestoneCreatedResultDto.cs          (CREAR)
│   │   ├── EntregableCreatedResultDto.cs         (CREAR)
│   │   ├── AprobarEntregableResultDto.cs         (CREAR)
│   │   ├── RechazarEntregableResultDto.cs        (CREAR)
│   │   ├── CompletarAcuerdoResultDto.cs          (CREAR)
│   │   └── CancelarAcuerdoResultDto.cs           (CREAR)
│   ├── Features/
│   │   └── Acuerdos/
│   │       ├── Commands/
│   │       │   ├── AceptarPropuestaCommand.cs    (CREAR - Command + Handler)
│   │       │   ├── RechazarPropuestaCommand.cs   (CREAR - Command + Handler)
│   │       │   ├── CreateMilestoneCommand.cs     (CREAR - Command + Handler)
│   │       │   ├── UpdateMilestoneCommand.cs     (CREAR - Command + Handler)
│   │       │   ├── DeleteMilestoneCommand.cs     (CREAR - Command + Handler)
│   │       │   ├── CreateEntregableCommand.cs    (CREAR - Command + Handler)
│   │       │   ├── AprobarEntregableCommand.cs   (CREAR - Command + Handler)
│   │       │   ├── RechazarEntregableCommand.cs  (CREAR - Command + Handler)
│   │       │   ├── CompletarAcuerdoCommand.cs    (CREAR - Command + Handler)
│   │       │   └── CancelarAcuerdoCommand.cs     (CREAR - Command + Handler)
│   │       ├── Queries/
│   │       │   └── GetAcuerdoByIdQuery.cs        (CREAR - Query + Handler)
│   │       └── Validators/
│   │           ├── AceptarPropuestaCommandValidator.cs   (CREAR)
│   │           ├── RechazarPropuestaCommandValidator.cs  (CREAR)
│   │           ├── CreateMilestoneCommandValidator.cs    (CREAR)
│   │           ├── UpdateMilestoneCommandValidator.cs    (CREAR)
│   │           ├── DeleteMilestoneCommandValidator.cs    (CREAR)
│   │           ├── CreateEntregableCommandValidator.cs   (CREAR)
│   │           ├── AprobarEntregableCommandValidator.cs  (CREAR)
│   │           ├── RechazarEntregableCommandValidator.cs (CREAR)
│   │           ├── CompletarAcuerdoCommandValidator.cs   (CREAR)
│   │           └── CancelarAcuerdoCommandValidator.cs    (CREAR)
│   ├── Interfaces/
│   │   └── Services/
│   │       ├── IAcuerdoCrowdsourcingService.cs           (CREAR)
│   │       ├── IAcuerdoCrowdsourcingMilestoneService.cs  (CREAR)
│   │       └── IAcuerdoCrowdsourcingEntregableService.cs (CREAR)
│   └── Mapping/
│       ├── AcuerdoProfile.cs                     (CREAR)
│       ├── MilestoneProfile.cs                   (CREAR)
│       └── EntregableProfile.cs                  (CREAR)
│
├── WePlayRises.Crowdsourcing.Infra/
│   ├── Services/
│   │   ├── AcuerdoCrowdsourcingService.cs        (CREAR)
│   │   ├── AcuerdoCrowdsourcingMilestoneService.cs (CREAR)
│   │   └── AcuerdoCrowdsourcingEntregableService.cs (CREAR)
│   └── Migrations/
│       ├── AddMissingFieldsToAcuerdoEntregable.cs (CREAR - agrega MotivoCancelacion, CanceladoPor, ComentarioRechazo, FechaActualizacion)
│       └── AddMaestrasEstadoAcuerdoEntregable.cs  (CREAR - si las maestras no existen aun)
│
└── WePlayRises.Crowdsourcing.WebApi/
    └── Controllers/
        ├── PropuestasCrowdsourcingController.cs  (MODIFICAR: agregar Aceptar y Rechazar)
        ├── AcuerdosCrowdsourcingController.cs    (CREAR)
        └── EntregablesCrowdsourcingController.cs (CREAR)
```

---

## 13. Checklist

- [ ] Todas las constantes `ServiceResponseMessageType` nuevas definidas (1013, 2011-2013, 4007-4013)
- [ ] 10 Commands implementan `IRequest<ServiceResponse<T>>` con T especifico
- [ ] 1 Query implementa `IRequest<ServiceResponse<AcuerdoDto>>`
- [ ] Handler + Command/Query en el MISMO archivo (11 archivos totales)
- [ ] 10 Validators con `.WithMessage()` Y `.WithErrorCode()` en cada regla
- [ ] Validators con errores de negocio usan `MustAsync` con services inyectados
- [ ] Validators NO inyectan `IRequestCacheService` directamente (lo usan via Services)
- [ ] 3 interfaces de Service definidas (IAcuerdoCrowdsouring*, IAcuerdoCrowdsourcingMilestone*, IAcuerdoCrowdsourcingEntregable*)
- [ ] 3 AutoMapper Profiles creados (AcuerdoProfile, MilestoneProfile, EntregableProfile)
- [ ] Campos calculados (ImporteAsignado, PorcentajeAsignado, MiRol, PorcentajeParcial, Timeline) marcados como `.Ignore()` en profiles y asignados en handlers
- [ ] `AceptarPropuestaCommand` usa transaccion de BD explicita en el service (`IDbContextTransaction`)
- [ ] `GetAcuerdoByIdQuery` usa `AsNoTracking()` y `Include(Milestones).ThenInclude(Entregables)`
- [ ] `PropuestasCrowdsourcingController` ampliado con Aceptar y Rechazar
- [ ] Nuevo `AcuerdosCrowdsourcingController` con 7 endpoints
- [ ] Nuevo `EntregablesCrowdsourcingController` con 2 endpoints
- [ ] Routing de errores en controllers (NotFound, Forbidden, BadRequest, Created)
- [ ] `GetUserId()` privado en cada nuevo controller (patron del `PropuestasCrowdsourcingController` existente)
- [ ] Gaps de entidad identificados resueltos con migraciones antes de implementar
- [ ] Campo `ComentarioRechazo` agregado a `AcuerdoCrowdsourcingEntregable` antes de `RechazarEntregableCommand`
- [ ] Campo `MotivoCancelacion` y `CanceladoPor` agregados a `AcuerdoCrowdsourcing` antes de `CancelarAcuerdoCommand`
- [ ] Navigation `Entregables` agregada a `AcuerdoCrowdsourcingMilestone`
- [ ] `AcuerdoCrowdsourcingService.CreateAsync` ejecuta todos los efectos secundarios en una sola transaccion
- [ ] Swagger documentation en todos los endpoints (`[ProducesResponseType]` por cada status code)
