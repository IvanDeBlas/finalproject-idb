# Arquitectura Hexagonal: cs-mensajeria

**Fecha:** 2026-02-18
**Modulo:** Crowdsourcing
**Feature:** cs-mensajeria (US-CS-05 - Mensajeria entre Partes)

---

## 1. Resumen Ejecutivo

Esta feature introduce la capa de mensajeria directa entre artistas y profesionales dentro del modulo Crowdsourcing. Las entidades `ConversacionCrowdsourcing` y `MensajeCrowdsourcing` ya existen en el codebase desde US-CS-04, pero su implementacion en repositorios, services e interfaces esta incompleta para los casos de uso de esta feature. Este plan detalla las extensiones necesarias para cubrir los 6 endpoints definidos en los contratos: crear conversacion, listar conversaciones, obtener mensajes, enviar mensaje, marcar leidos y obtener conteo de no leidos.

---

## 2. Analisis del Estado Actual

### 2.1 Entidades - YA EXISTEN (requieren modificacion)

| Entidad | Estado | Ruta | Problema |
|---------|--------|------|---------|
| `ConversacionCrowdsourcing` | Existe | `WePlayRises.Crowdsourcing.Domain/Model/ConversacionCrowdsourcing.cs` | Usa `UserIdArtista`/`UserIdProveedor` en lugar de `UserIdCreador`/`UserIdDestinatario`. `Asunto` es nullable pero debe ser requerido. |
| `MensajeCrowdsourcing` | Existe | `WePlayRises.Crowdsourcing.Domain/Model/MensajeCrowdsourcing.cs` | Correcto. Solo verificar `Contenido` con MaxLength 5000. |

### 2.2 Repositories - Parcialmente existentes

| Interface | Estado | Implementacion |
|-----------|--------|----------------|
| `IConversacionCrowdsourcingRepository` | Existe pero incompleta | Solo tiene `AddAsync`. Necesita 6 metodos nuevos. |
| `IMensajeCrowdsourcingRepository` | NO EXISTE | Crear desde cero |

### 2.3 Services - NO EXISTEN

| Interface/Clase | Estado |
|----------------|--------|
| `IConversacionCrowdsourcingService` | NO EXISTE - Crear |
| `ConversacionCrowdsourcingService` | NO EXISTE - Crear |
| `IMensajeCrowdsourcingService` | NO EXISTE - Crear |
| `MensajeCrowdsourcingService` | NO EXISTE - Crear |

### 2.4 Configuracion EF Core - Incompleta

La configuracion de `ConversacionCrowdsourcing` y `MensajeCrowdsourcing` esta en `CrowdsourcingContext.OnModelCreating` de forma inline (no como clases separadas `IEntityTypeConfiguration`). Faltan:
- Indices de unicidad filtrados para evitar conversaciones duplicadas
- Indices de performance para las queries del listado y mensajes no leidos
- Columna `Contenido` de `MensajeCrowdsourcing` usa `nvarchar(max)` pero deberia ser `nvarchar(5000)` con `HasMaxLength(5000)`
- `UserIdArtista`/`UserIdProveedor` en `ConversacionCrowdsourcing` deben renombrarse a `UserIdCreador`/`UserIdDestinatario`

### 2.5 DI - Incompleto

`ConversacionCrowdsourcingRepository` registrado. Faltan los 4 registros nuevos (service + repository de Mensaje + service de Conversacion).

### 2.6 DbContext - Ya incluye los DbSets

`CrowdsourcingContext` ya tiene:
- `public DbSet<ConversacionCrowdsourcing> Conversaciones => Set<ConversacionCrowdsourcing>();`
- `public DbSet<MensajeCrowdsourcing> Mensajes => Set<MensajeCrowdsourcing>();`

No se requieren cambios en DbContext para los DbSets.

---

## 3. Domain Layer

### 3.1 Modificaciones a Entidades Existentes

#### ConversacionCrowdsourcing (MODIFICAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/ConversacionCrowdsourcing.cs`

**Cambios requeridos:**
- Renombrar `UserIdArtista` -> `UserIdCreador`
- Renombrar `UserIdProveedor` -> `UserIdDestinatario`
- Cambiar `Asunto` de `string?` a `string` con `= null!` (requerido)

**CRITICO:** Este cambio de nombres implica una migracion de base de datos (renombrar columnas `UserIdArtista`/`UserIdProveedor` a `UserIdCreador`/`UserIdDestinatario`). Ver seccion 8.

**Estado esperado del POCO:**

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `Id` | `Guid` | No | PK |
| `NecesidadId` | `NecesidadCrowdsourcingId?` | Si | FK a NecesidadCrowdsourcing. Mutuamente excluyente con AcuerdoId. |
| `AcuerdoId` | `AcuerdoCrowdsourcingId?` | Si | FK a AcuerdoCrowdsourcing. Mutuamente excluyente con NecesidadId. |
| `UserIdCreador` | `string` | No | FK Identity. Usuario que creo la conversacion. Max 450. |
| `UserIdDestinatario` | `string` | No | FK Identity. Usuario receptor. Max 450. |
| `Asunto` | `string` | No | Asunto de la conversacion. Max 200. |
| `FechaCreacion` | `DateTime` | No | UTC. Establecida al crear. |
| `FechaUltimoMensaje` | `DateTime?` | Si | UTC. Null hasta que se envia el primer mensaje. |
| `Necesidad` | `NecesidadCrowdsourcing?` | Si | Navegacion hacia NecesidadCrowdsourcing. |
| `Acuerdo` | `AcuerdoCrowdsourcing?` | Si | Navegacion hacia AcuerdoCrowdsourcing. |
| `Mensajes` | `ICollection<MensajeCrowdsourcing>` | No | Navegacion 1:N hacia mensajes. |

**Nota sobre `NecesidadId` y `AcuerdoId`:** Ambos son StronglyTypedIds (`NecesidadCrowdsourcingId` y `AcuerdoCrowdsourcingId`). La regla de negocio de que exactamente uno debe estar presente se valida en el Handler/Validator, no en la entidad POCO.

#### MensajeCrowdsourcing (VERIFICAR / ajuste menor)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MensajeCrowdsourcing.cs`

El POCO actual es correcto. Solo hay que verificar que la configuracion EF refleje `HasMaxLength(5000)` en `Contenido` (actualmente es `nvarchar(max)`).

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `Id` | `Guid` | No | PK |
| `ConversacionId` | `Guid` | No | FK a ConversacionCrowdsourcing. |
| `UserIdRemitente` | `string` | No | FK Identity. Usuario que envio el mensaje. Max 450. |
| `Contenido` | `string` | No | Contenido del mensaje. Max 5000. |
| `UrlAdjunto` | `string?` | Si | URL adjunto externo. Max 2048. |
| `Leido` | `bool` | No | Default false al crear. |
| `FechaLeido` | `DateTime?` | Si | UTC. Null hasta marcar como leido. |
| `FechaCreacion` | `DateTime` | No | UTC. |
| `Conversacion` | `ConversacionCrowdsourcing` | No | Navegacion hacia ConversacionCrowdsourcing. |

### 3.2 Constantes - Modificar ServiceResponseMessageType

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**Accion:** MODIFICAR. Agregar 3 constantes nuevas al archivo existente.

**Estado actual del archivo:** Tiene hasta `NotFound_Entregable = "2013"` y `BusinessRule_InvalidState = "4014"`.

**Constantes a agregar:**

```csharp
// NotFound (2000-2999) - agregar despues de NotFound_Entregable
public const string NotFound_Conversacion = "2014";

// Business Rules (4000-4999) - agregar despues de BusinessRule_InvalidState
public const string BusinessRule_ConversacionDuplicada = "4015";
public const string BusinessRule_NoRelacionConDestinatario = "4016";
```

**Tabla completa post-modificacion para referencia de Validators:**

| Codigo | Constante | Uso en esta feature |
|--------|-----------|---------------------|
| 0001 | Created | Respuesta exitosa de crear conversacion o mensaje |
| 1001 | Validation_Required | asunto vacio, userIdDestinatario vacio, contenido vacio |
| 1002 | Validation_MaxLength | asunto > 200, contenido > 5000 |
| 1013 | Validation_InvalidUrl | urlAdjunto presente pero invalida |
| 2014 | NotFound_Conversacion | GET /conversaciones/{id}/mensajes - conversacion no existe |
| 3001 | Auth_Unauthorized | Token invalido |
| 3002 | Auth_Forbidden | Usuario no es participante de la conversacion |
| 4015 | BusinessRule_ConversacionDuplicada | Ya existe conversacion para el mismo contexto |
| 4016 | BusinessRule_NoRelacionConDestinatario | No existe propuesta/acuerdo vinculando usuarios |
| 5000 | Internal_UnexpectedError | Excepcion no controlada en cualquier handler |

### 3.3 Repository Interfaces (Ports)

#### IConversacionCrowdsourcingRepository (MODIFICAR - extender la existente)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IConversacionCrowdsourcingRepository.cs`

**Estado actual:** Solo tiene `AddAsync`. Hay que agregar los metodos nuevos manteniendolo backward-compatible.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `AddAsync` | `Task<Guid>` | `ConversacionCrowdsourcing entity, CancellationToken ct` | YA EXISTE. Persiste y retorna el Id. |
| `GetByIdAsync` | `Task<ConversacionCrowdsourcing?>` | `Guid id, CancellationToken ct` | Obtener por Id. Retorna null si no existe. |
| `ExisteConversacionParaContextoAsync` | `Task<bool>` | `string userIdCreador, string userIdDestinatario, Guid? necesidadId, Guid? acuerdoId, CancellationToken ct` | Verifica si ya existe una conversacion para el mismo par de usuarios y contexto. Usa para validar unicidad. |
| `GetConversacionesByUserIdAsync` | `Task<(IReadOnlyList<ConversacionCrowdsourcing> Items, int TotalCount, int TotalNoLeidos)>` | `string userId, string? filtroContexto, int page, int pageSize, CancellationToken ct` | Listado paginado de conversaciones donde userId es creador o destinatario. Retorna TotalNoLeidos (suma de mensajes no leidos en TODAS las conversaciones del user, no solo la pagina). `filtroContexto`: null/'todas' para todas, 'necesidades' para NecesidadId IS NOT NULL, 'acuerdos' para AcuerdoId IS NOT NULL. |
| `UpdateFechaUltimoMensajeAsync` | `Task` | `Guid conversacionId, DateTime fechaUltimoMensaje, CancellationToken ct` | Actualiza FechaUltimoMensaje. Llamado al enviar un mensaje. Operacion de escritura directa sin SaveChanges (el Service hace commit). |
| `GetTotalNoLeidosByUserIdAsync` | `Task<int>` | `string userId, CancellationToken ct` | Query ligera para el badge del navbar. COUNT de mensajes no leidos del user en todas sus conversaciones. |

**Nota sobre `GetConversacionesByUserIdAsync`:** La query SQL debe calcular en una sola operacion:
- El listado paginado con el preview del ultimo mensaje (truncado a 80 chars en SQL con `LEFT(...)`)
- El count de no leidos por conversacion (subconsulta o JOIN con GROUP BY)
- El `TotalNoLeidos` global (suma de todos los no leidos del user antes de paginar)
- El `TotalCount` de conversaciones del user (antes de paginar)
- Ordenar por `FechaUltimoMensaje DESC NULLS LAST` (conversaciones sin mensajes al final, ordenadas por FechaCreacion DESC)

#### IMensajeCrowdsourcingRepository (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Repositories/IMensajeCrowdsourcingRepository.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `AddAsync` | `Task<Guid>` | `MensajeCrowdsourcing entity, CancellationToken ct` | Persiste el mensaje. Sin SaveChanges (lo hace el Service via UoW o directo). Retorna el Id asignado. |
| `GetByConversacionIdPaginatedAsync` | `Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)>` | `Guid conversacionId, int page, int pageSize, CancellationToken ct` | Mensajes paginados de una conversacion ordenados por FechaCreacion ASC. El frontend invierte para mostrar los mas recientes abajo. |
| `MarcarLeidosByConversacionAsync` | `Task<int>` | `Guid conversacionId, string userIdDestinatario, DateTime fechaLeido, CancellationToken ct` | UPDATE masivo: Leido=true, FechaLeido=fechaLeido WHERE ConversacionId=@id AND UserIdRemitente != @userIdDestinatario AND Leido=false. Retorna filas afectadas. Operacion directa con ExecuteUpdateAsync o SQL raw. |

---

## 4. Infrastructure Layer

### 4.1 Entity Configurations EF Core

#### ConversacionCrowdsourcingConfiguration (NUEVA - extraer de OnModelCreating)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/ConversacionCrowdsourcingConfiguration.cs`

La configuracion actual esta inline en `CrowdsourcingContext.OnModelCreating`. Se debe extraer a una clase separada `IEntityTypeConfiguration<ConversacionCrowdsourcing>` y agregar los indices que faltan.

| Configuracion | Detalle |
|---------------|---------|
| `ToTable` | `"ConversacionCrowdsourcing"` (mantener nombre existente de la tabla) |
| `HasKey` | `x => x.Id` |
| `Id` | `ValueGeneratedOnAdd` |
| `UserIdCreador` | `HasColumnName("UserIdCreador")`, `HasMaxLength(450)`, `IsRequired()` |
| `UserIdDestinatario` | `HasColumnName("UserIdDestinatario")`, `HasMaxLength(450)`, `IsRequired()` |
| `Asunto` | `HasMaxLength(200)`, `IsRequired()` |
| `FechaCreacion` | `HasPrecision(3)`, `IsRequired()` |
| `FechaUltimoMensaje` | `HasPrecision(3)`, `IsRequired(false)` |
| `NecesidadId` | `HasColumnName("Necesidad_Id")`, `IsRequired(false)` |
| `AcuerdoId` | `HasColumnName("Acuerdo_Id")`, `IsRequired(false)` |
| Relacion Necesidad | `HasOne(x => x.Necesidad).WithMany(x => x.Conversaciones).HasForeignKey(x => x.NecesidadId).IsRequired(false).OnDelete(DeleteBehavior.SetNull)` |
| Relacion Acuerdo | `HasOne(x => x.Acuerdo).WithMany(x => x.Conversaciones).HasForeignKey(x => x.AcuerdoId).IsRequired(false).OnDelete(DeleteBehavior.SetNull)` |
| Relacion Mensajes | `HasMany(x => x.Mensajes).WithOne(x => x.Conversacion).HasForeignKey(x => x.ConversacionId).OnDelete(DeleteBehavior.Cascade)` |
| **Indice unicidad NecesidadId** | `HasIndex(x => new { x.UserIdCreador, x.UserIdDestinatario, x.NecesidadId }).HasFilter("[Necesidad_Id] IS NOT NULL").IsUnique().HasDatabaseName("UX_ConversacionCrowdsourcing_Usuarios_Necesidad")` |
| **Indice unicidad AcuerdoId** | `HasIndex(x => new { x.UserIdCreador, x.UserIdDestinatario, x.AcuerdoId }).HasFilter("[Acuerdo_Id] IS NOT NULL").IsUnique().HasDatabaseName("UX_ConversacionCrowdsourcing_Usuarios_Acuerdo")` |
| **Indice FechaUltimoMensaje** | `HasIndex(x => x.FechaUltimoMensaje).HasDatabaseName("IX_ConversacionCrowdsourcing_FechaUltimoMensaje")` |
| **Indice listado por userId** | `HasIndex(x => new { x.UserIdCreador, x.FechaUltimoMensaje }).HasDatabaseName("IX_ConversacionCrowdsourcing_Creador_Fecha")` |
| **Indice listado por userId** | `HasIndex(x => new { x.UserIdDestinatario, x.FechaUltimoMensaje }).HasDatabaseName("IX_ConversacionCrowdsourcing_Destinatario_Fecha")` |

**NOTA CRITICA sobre el renombrado de columnas:** La migracion debe hacer `RenameColumn` de `UserIdArtista` a `UserIdCreador` y de `UserIdProveedor` a `UserIdDestinatario` en la tabla `ConversacionCrowdsourcing`. Esto es necesario porque `AcuerdoCrowdsourcingService.AceptarPropuestaAsync` crea `ConversacionCrowdsourcing` con `UserIdArtista` y `UserIdProveedor`. Ese codigo tambien debe actualizarse para usar `UserIdCreador` y `UserIdDestinatario`.

**NOTA sobre el cambio de `Asunto` a `IsRequired()`:** La columna actualmente permite NULL segun el modelo. La migracion debe incluir `AlterColumn` para hacer `Asunto` NOT NULL. Verificar si existen registros con Asunto nulo antes de aplicar.

#### MensajeCrowdsourcingConfiguration (NUEVA - extraer de OnModelCreating)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/MensajeCrowdsourcingConfiguration.cs`

| Configuracion | Detalle |
|---------------|---------|
| `ToTable` | `"MensajeCrowdsourcing"` (mantener nombre existente) |
| `HasKey` | `x => x.Id` |
| `Id` | `ValueGeneratedOnAdd` |
| `ConversacionId` | `HasColumnName("Conversacion_Id")`, `IsRequired()` |
| `UserIdRemitente` | `HasMaxLength(450)`, `IsRequired()` |
| `Contenido` | `HasMaxLength(5000)`, `IsRequired()` (cambiar de `nvarchar(max)` a `nvarchar(5000)`) |
| `UrlAdjunto` | `HasMaxLength(2048)`, `IsRequired(false)` (cambiar de 500 a 2048 para URLs completas) |
| `Leido` | `HasDefaultValue(false)` |
| `FechaLeido` | `HasPrecision(3)`, `IsRequired(false)` |
| `FechaCreacion` | `HasPrecision(3)`, `IsRequired()` |
| Relacion Conversacion | `HasOne(x => x.Conversacion).WithMany(x => x.Mensajes).HasForeignKey(x => x.ConversacionId).OnDelete(DeleteBehavior.Cascade)` |
| **Indice no leidos + orden** | `HasIndex(x => new { x.ConversacionId, x.Leido, x.UserIdRemitente }).HasDatabaseName("IX_MensajeCrowdsourcing_Conversacion_Leido_Remitente")` |
| **Indice orden cronologico** | `HasIndex(x => new { x.ConversacionId, x.FechaCreacion }).HasDatabaseName("IX_MensajeCrowdsourcing_Conversacion_Fecha")` |

**NOTA sobre MaxLength de Contenido:** La columna actual es `nvarchar(max)`. Cambiar a `nvarchar(5000)` requiere `AlterColumn` en la migracion. Esto es siempre seguro (reducir de max a 5000 no pierde datos si no hay mensajes > 5000 chars).

**NOTA sobre MaxLength de UrlAdjunto:** La columna actual es `nvarchar(500)`. Cambiar a `nvarchar(2048)` es un incremento, siempre seguro.

#### Modificacion en CrowdsourcingContext (MODIFICAR)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

Reemplazar las configuraciones inline de `ConversacionCrowdsourcing` y `MensajeCrowdsourcing` en `OnModelCreating` por:

```csharp
modelBuilder.ApplyConfiguration(new ConversacionCrowdsourcingConfiguration());
modelBuilder.ApplyConfiguration(new MensajeCrowdsourcingConfiguration());
```

Agregar estos `ApplyConfiguration` junto a los existentes al final de `OnModelCreating`, y eliminar los bloques `modelBuilder.Entity<ConversacionCrowdsourcing>(entity => { ... })` y `modelBuilder.Entity<MensajeCrowdsourcing>(entity => { ... })`.

### 4.2 Repository Implementations

#### ConversacionCrowdsourcingRepository (MODIFICAR - extender la existente)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/ConversacionCrowdsourcingRepository.cs`

- Implementa: `IConversacionCrowdsourcingRepository`
- Inyecta: `CrowdsourcingContext`
- `AddAsync`: YA EXISTE. Hace `AddAsync` + `SaveChangesAsync`. Mantener como esta.

**Nuevo metodo `GetByIdAsync`:**
```
context.Conversaciones
    .FirstOrDefaultAsync(x => x.Id == id, ct)
```

**Nuevo metodo `ExisteConversacionParaContextoAsync`:**
```
context.Conversaciones.AnyAsync(x =>
    x.UserIdCreador == userIdCreador &&
    x.UserIdDestinatario == userIdDestinatario &&
    (necesidadId.HasValue ? x.NecesidadId == new NecesidadCrowdsourcingId(necesidadId.Value) : x.AcuerdoId == new AcuerdoCrowdsourcingId(acuerdoId!.Value)),
    ct)
```

**Nuevo metodo `GetConversacionesByUserIdAsync`:**
Requiere una query compuesta que en una sola ida a BD:
1. Filtra conversaciones donde `UserIdCreador == userId OR UserIdDestinatario == userId`
2. Aplica filtro por contexto si se indica (`necesidades` -> `NecesidadId != null`, `acuerdos` -> `AcuerdoId != null`)
3. Para cada conversacion calcula `MensajesNoLeidos = COUNT(Mensajes WHERE UserIdRemitente != userId AND Leido == false)`
4. Obtiene el ultimo mensaje (Join o subquery con `MAX(FechaCreacion)` y el contenido correspondiente)
5. Ordena: conversaciones con `FechaUltimoMensaje != null` primero (desc), luego `FechaCreacion DESC`
6. Pagina con `Skip` y `Take`
7. Calcula `TotalCount` (total de conversaciones del user con el filtro aplicado, sin paginacion)
8. Calcula `TotalNoLeidos` (suma global de no leidos del user en TODAS sus conversaciones sin filtro de contexto ni paginacion)

Esta query es compleja. Usar una combinacion de `IQueryable` con proyecciones y una query separada para `TotalNoLeidos`. Alternativa: un metodo EF LINQ con `Select` proyectando un DTO anonimo que incluya las subconsultas como `Count()`.

**Nuevo metodo `UpdateFechaUltimoMensajeAsync`:**
```
context.Conversaciones
    .Where(x => x.Id == conversacionId)
    .ExecuteUpdateAsync(setters =>
        setters.SetProperty(x => x.FechaUltimoMensaje, fechaUltimoMensaje),
        ct)
```
Usar `ExecuteUpdateAsync` (EF Core 7+) para evitar cargar la entidad. Sin `SaveChangesAsync` (transaccion la maneja el Service).

**Nuevo metodo `GetTotalNoLeidosByUserIdAsync`:**
```
context.Mensajes
    .CountAsync(m =>
        (context.Conversaciones.Any(c =>
            c.Id == m.ConversacionId &&
            (c.UserIdCreador == userId || c.UserIdDestinatario == userId))) &&
        m.UserIdRemitente != userId &&
        !m.Leido,
        ct)
```
Alternativa mas eficiente con Join:
```
context.Mensajes
    .Join(context.Conversaciones,
        m => m.ConversacionId,
        c => c.Id,
        (m, c) => new { m, c })
    .CountAsync(x =>
        (x.c.UserIdCreador == userId || x.c.UserIdDestinatario == userId) &&
        x.m.UserIdRemitente != userId &&
        !x.m.Leido,
        ct)
```

#### MensajeCrowdsourcingRepository (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/MensajeCrowdsourcingRepository.cs`

- Implementa: `IMensajeCrowdsourcingRepository`
- Inyecta: `CrowdsourcingContext`

**Metodo `AddAsync`:**
```
await context.Mensajes.AddAsync(entity, ct)
await context.SaveChangesAsync(ct)
return entity.Id
```

**Metodo `GetByConversacionIdPaginatedAsync`:**
```
context.Mensajes
    .AsNoTracking()
    .Where(x => x.ConversacionId == conversacionId)
    .OrderBy(x => x.FechaCreacion)  // ASC: mas antiguos primero
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync(ct)
```
Con una query separada para `TotalCount`:
```
context.Mensajes.CountAsync(x => x.ConversacionId == conversacionId, ct)
```

**Metodo `MarcarLeidosByConversacionAsync`:**
```
var count = await context.Mensajes
    .Where(x =>
        x.ConversacionId == conversacionId &&
        x.UserIdRemitente != userIdDestinatario &&
        !x.Leido)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(x => x.Leido, true)
        .SetProperty(x => x.FechaLeido, fechaLeido),
        ct);
return count;
```
Usar `ExecuteUpdateAsync` para UPDATE masivo eficiente (sin cargar entidades). Esta operacion no requiere `SaveChangesAsync` adicional porque `ExecuteUpdateAsync` aplica directamente a la BD.

### 4.3 Services

#### IConversacionCrowdsourcingService (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IConversacionCrowdsourcingService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `Task<ConversacionCrowdsourcing?>` | `Guid id, CancellationToken ct` | Obtiene conversacion por Id. Usa cache de request. |
| `ExisteConversacionParaContextoAsync` | `Task<bool>` | `string userIdCreador, string userIdDestinatario, Guid? necesidadId, Guid? acuerdoId, CancellationToken ct` | Verifica unicidad. Usada por Validator antes de crear. Usa cache de request. |
| `CreateAsync` | `Task<Guid>` | `ConversacionCrowdsourcing entity, CancellationToken ct` | Persiste la conversacion y hace commit. Retorna el Id. |
| `GetConversacionesByUserIdAsync` | `Task<(IReadOnlyList<ConversacionCrowdsourcing> Items, int TotalCount, int TotalNoLeidos)>` | `string userId, string? filtroContexto, int page, int pageSize, CancellationToken ct` | Listado paginado para el Handler de GetConversacionesQuery. |
| `UpdateFechaUltimoMensajeAsync` | `Task` | `Guid conversacionId, DateTime fechaUltimoMensaje, CancellationToken ct` | Actualiza FechaUltimoMensaje de la conversacion. Llamado por MensajeCrowdsourcingService al enviar mensaje. NO hace commit (el llamante es responsable o es una query directa via ExecuteUpdateAsync que no necesita commit). |
| `GetTotalNoLeidosByUserIdAsync` | `Task<int>` | `string userId, CancellationToken ct` | Conteo para el badge del navbar. Usa cache de request. |

#### ConversacionCrowdsourcingService (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/ConversacionCrowdsourcingService.cs`

- Implementa: `IConversacionCrowdsourcingService`
- Inyecta (con `?? throw` obligatorio en constructor):
  - `IConversacionCrowdsourcingRepository _repository`
  - `IRequestCacheService _requestCache`
  - `ILogger<ConversacionCrowdsourcingService> _logger`

**Firma del constructor:**
```csharp
public ConversacionCrowdsourcingService(
    IConversacionCrowdsourcingRepository repository,
    IRequestCacheService requestCache,
    ILogger<ConversacionCrowdsourcingService> logger)
{
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Implementacion de metodos:**

`GetByIdAsync`:
```
await _requestCache.GetOrAddAsync(
    $"conversacion:{id}",
    async () => await _repository.GetByIdAsync(id, ct))
```

`ExisteConversacionParaContextoAsync`:
```
var cacheKey = $"conversacion-existe:{userIdCreador}:{userIdDestinatario}:{necesidadId}:{acuerdoId}";
await _requestCache.GetOrAddAsync(
    cacheKey,
    async () => await _repository.ExisteConversacionParaContextoAsync(
        userIdCreador, userIdDestinatario, necesidadId, acuerdoId, ct))
```

`CreateAsync`:
```
await _repository.AddAsync(entity, ct)
// AddAsync ya hace SaveChangesAsync (patron del proyecto: repository hace commit)
return entity.Id
```

`GetConversacionesByUserIdAsync`:
```
// Delegar directamente al repository (no cachear - datos mutables y paginados)
return await _repository.GetConversacionesByUserIdAsync(
    userId, filtroContexto, page, pageSize, ct)
```

`UpdateFechaUltimoMensajeAsync`:
```
// ExecuteUpdateAsync en el repository no necesita SaveChanges
await _repository.UpdateFechaUltimoMensajeAsync(conversacionId, fechaUltimoMensaje, ct)
```

`GetTotalNoLeidosByUserIdAsync`:
```
await _requestCache.GetOrAddAsync(
    $"conversacion-noleidos:{userId}",
    async () => await _repository.GetTotalNoLeidosByUserIdAsync(userId, ct))
```

#### IMensajeCrowdsourcingService (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Application/Interfaces/Services/IMensajeCrowdsourcingService.cs`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `SendAsync` | `Task<MensajeCrowdsourcing>` | `MensajeCrowdsourcing entity, Guid conversacionId, CancellationToken ct` | Persiste el mensaje y actualiza FechaUltimoMensaje de la conversacion en la misma operacion. Retorna la entidad con Id asignado. |
| `GetByConversacionIdPaginatedAsync` | `Task<(IReadOnlyList<MensajeCrowdsourcing> Items, int TotalCount)>` | `Guid conversacionId, int page, int pageSize, CancellationToken ct` | Mensajes paginados de una conversacion para el Handler de GetMensajesQuery. |
| `MarcarLeidosAsync` | `Task<int>` | `Guid conversacionId, string userIdQueAbre, CancellationToken ct` | Marca como leidos los mensajes del otro participante. Retorna cantidad marcada. |

#### MensajeCrowdsourcingService (CREAR NUEVO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/MensajeCrowdsourcingService.cs`

- Implementa: `IMensajeCrowdsourcingService`
- Inyecta (con `?? throw` obligatorio en constructor):
  - `IMensajeCrowdsourcingRepository _repository`
  - `IConversacionCrowdsourcingRepository _conversacionRepository`
  - `IRequestCacheService _requestCache`
  - `ILogger<MensajeCrowdsourcingService> _logger`

**Firma del constructor:**
```csharp
public MensajeCrowdsourcingService(
    IMensajeCrowdsourcingRepository repository,
    IConversacionCrowdsourcingRepository conversacionRepository,
    IRequestCacheService requestCache,
    ILogger<MensajeCrowdsourcingService> logger)
{
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    _conversacionRepository = conversacionRepository ?? throw new ArgumentNullException(nameof(conversacionRepository));
    _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

**Implementacion de `SendAsync`:**
```
// 1. Persistir el mensaje (AddAsync hace SaveChangesAsync)
var id = await _repository.AddAsync(entity, ct)

// 2. Actualizar FechaUltimoMensaje de la conversacion
//    (ExecuteUpdateAsync es una query directa, no necesita SaveChanges adicional)
await _conversacionRepository.UpdateFechaUltimoMensajeAsync(
    conversacionId, entity.FechaCreacion, ct)

// 3. Retornar la entidad con el Id asignado
entity.Id = id;  // Asignar si AddAsync no muta la entidad directamente
return entity
```

**NOTA de diseño:** `SendAsync` orquesta dos operaciones de escritura:
- `_repository.AddAsync` persiste el mensaje con `SaveChangesAsync`
- `_conversacionRepository.UpdateFechaUltimoMensajeAsync` usa `ExecuteUpdateAsync` (directo a BD, no necesita SaveChanges)

Ambas comparten el mismo `CrowdsourcingContext` inyectado via DI (Scoped). Esta secuencia no es atomicamente transaccional a nivel de BD (no hay `BeginTransaction` explicito), pero es aceptable para el MVP porque en caso de fallo de la segunda operacion el mensaje ya existe y el usuario puede intentar enviar otro (que actualizara la fecha correctamente).

**Implementacion de `GetByConversacionIdPaginatedAsync`:**
```
return await _repository.GetByConversacionIdPaginatedAsync(
    conversacionId, page, pageSize, ct)
```

**Implementacion de `MarcarLeidosAsync`:**
```
var fechaLeido = DateTime.UtcNow;
return await _repository.MarcarLeidosByConversacionAsync(
    conversacionId, userIdQueAbre, fechaLeido, ct)
```

---

## 5. Modificaciones en AcuerdoCrowdsourcingService (CRITICO)

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/AcuerdoCrowdsourcingService.cs`

Al renombrar `UserIdArtista`/`UserIdProveedor` a `UserIdCreador`/`UserIdDestinatario` en la entidad, el metodo `AceptarPropuestaAsync` que construye `ConversacionCrowdsourcing` debe actualizarse:

**Antes:**
```csharp
var conversacion = new ConversacionCrowdsourcing
{
    UserIdArtista = userIdArtista,
    UserIdProveedor = propuesta.UserId,
    // ...
};
```

**Despues:**
```csharp
var conversacion = new ConversacionCrowdsourcing
{
    UserIdCreador = userIdArtista,
    UserIdDestinatario = propuesta.UserId,
    // ...
};
```

Este cambio es obligatorio para que el codigo compile tras la modificacion del POCO.

---

## 6. Dependency Injection

**Archivo:** `src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

**Accion:** MODIFICAR. Agregar 3 registros nuevos al metodo `AddCrowdsourcingServices`.

**Estado actual del archivo:** Tiene `IConversacionCrowdsourcingRepository` registrado.

**Registros a agregar:**

```csharp
// En la seccion de Repositories (despues de IConversacionCrowdsourcingRepository):
services.AddScoped<IMensajeCrowdsourcingRepository, MensajeCrowdsourcingRepository>();

// En la seccion de Services (despues de IAcuerdoCrowdsourcingEntregableService):
services.AddScoped<IConversacionCrowdsourcingService, ConversacionCrowdsourcingService>();
services.AddScoped<IMensajeCrowdsourcingService, MensajeCrowdsourcingService>();
```

**Estado del archivo tras modificacion:**
```csharp
public static IServiceCollection AddCrowdsourcingServices(this IServiceCollection services)
{
    // Repositories
    services.AddScoped<IPlantillaProyectoRepository, PlantillaProyectoRepository>();
    services.AddScoped<IPlantillaProyectoNecesidadRepository, PlantillaProyectoNecesidadRepository>();
    services.AddScoped<IMaestraRolProfesionalRepository, MaestraRolProfesionalRepository>();
    services.AddScoped<IMaestraCategoriaRolRepository, MaestraCategoriaRolRepository>();
    services.AddScoped<INecesidadCrowdsourcingRepository, NecesidadCrowdsourcingRepository>();
    services.AddScoped<IPropuestaCrowdsourcingRepository, PropuestaCrowdsourcingRepository>();
    services.AddScoped<IAcuerdoCrowdsourcingRepository, AcuerdoCrowdsourcingRepository>();
    services.AddScoped<IAcuerdoCrowdsourcingMilestoneRepository, AcuerdoCrowdsourcingMilestoneRepository>();
    services.AddScoped<IAcuerdoCrowdsourcingEntregableRepository, AcuerdoCrowdsourcingEntregableRepository>();
    services.AddScoped<IConversacionCrowdsourcingRepository, ConversacionCrowdsourcingRepository>();
    services.AddScoped<IMensajeCrowdsourcingRepository, MensajeCrowdsourcingRepository>();  // NUEVO

    // Services
    services.AddScoped<IPlantillaProyectoService, PlantillaProyectoService>();
    services.AddScoped<IRolProfesionalService, RolProfesionalService>();
    services.AddScoped<ICategoriaRolService, CategoriaRolService>();
    services.AddScoped<INecesidadCrowdsourcingService, NecesidadCrowdsourcingService>();
    services.AddScoped<IPropuestaCrowdsourcingService, PropuestaCrowdsourcingService>();
    services.AddScoped<IAcuerdoCrowdsourcingService, AcuerdoCrowdsourcingService>();
    services.AddScoped<IAcuerdoCrowdsourcingMilestoneService, AcuerdoCrowdsourcingMilestoneService>();
    services.AddScoped<IAcuerdoCrowdsourcingEntregableService, AcuerdoCrowdsourcingEntregableService>();
    services.AddScoped<IConversacionCrowdsourcingService, ConversacionCrowdsourcingService>();  // NUEVO
    services.AddScoped<IMensajeCrowdsourcingService, MensajeCrowdsourcingService>();             // NUEVO

    return services;
}
```

---

## 7. Migraciones

**Nombre sugerido:** `AddMensajeriaIndexesAndRenameConversacionColumns`

**Proyecto de migracion:** `WePlayRises.Crowdsourcing.Infra`

**Comando:**
```bash
dotnet ef migrations add AddMensajeriaIndexesAndRenameConversacionColumns \
    --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
    --startup-project src/api/WebApi
```

**Operaciones que debe contener la migracion:**

### En tabla `ConversacionCrowdsourcing`:

1. `RenameColumn`: `UserIdArtista` -> `UserIdCreador`
2. `RenameColumn`: `UserIdProveedor` -> `UserIdDestinatario`
3. `AlterColumn`: `Asunto` de `nvarchar(200) NULL` a `nvarchar(200) NOT NULL DEFAULT ''` (o verificar si hay NULLs primero)
4. `CreateIndex` (unique, filtered): `UX_ConversacionCrowdsourcing_Usuarios_Necesidad` sobre `(UserIdCreador, UserIdDestinatario, Necesidad_Id) WHERE Necesidad_Id IS NOT NULL`
5. `CreateIndex` (unique, filtered): `UX_ConversacionCrowdsourcing_Usuarios_Acuerdo` sobre `(UserIdCreador, UserIdDestinatario, Acuerdo_Id) WHERE Acuerdo_Id IS NOT NULL`
6. `CreateIndex`: `IX_ConversacionCrowdsourcing_FechaUltimoMensaje`
7. `CreateIndex`: `IX_ConversacionCrowdsourcing_Creador_Fecha` sobre `(UserIdCreador, FechaUltimoMensaje)`
8. `CreateIndex`: `IX_ConversacionCrowdsourcing_Destinatario_Fecha` sobre `(UserIdDestinatario, FechaUltimoMensaje)`

### En tabla `MensajeCrowdsourcing`:

1. `AlterColumn`: `Contenido` de `nvarchar(max)` a `nvarchar(5000)`
2. `AlterColumn`: `UrlAdjunto` de `nvarchar(500)` a `nvarchar(2048)`
3. `CreateIndex`: `IX_MensajeCrowdsourcing_Conversacion_Leido_Remitente` sobre `(Conversacion_Id, Leido, UserIdRemitente)`
4. `CreateIndex`: `IX_MensajeCrowdsourcing_Conversacion_Fecha` sobre `(Conversacion_Id, FechaCreacion)`

**Aplicar migracion:**
```bash
dotnet ef database update \
    --project src/api/Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra \
    --startup-project src/api/WebApi
```

---

## 8. Archivos a Crear / Modificar

```
src/api/Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   ├── Model/
│   │   ├── ConversacionCrowdsourcing.cs          [MODIFICAR] Renombrar UserIdArtista->UserIdCreador, UserIdProveedor->UserIdDestinatario, Asunto NOT NULL
│   │   └── MensajeCrowdsourcing.cs               [VERIFICAR] Solo confirmar que el POCO esta correcto (ya lo esta)
│   └── Constants/
│       └── ServiceResponseMessageType.cs          [MODIFICAR] Agregar NotFound_Conversacion="2014", BusinessRule_ConversacionDuplicada="4015", BusinessRule_NoRelacionConDestinatario="4016"
│
├── WePlayRises.Crowdsourcing.Application/
│   └── Interfaces/
│       ├── Repositories/
│       │   ├── IConversacionCrowdsourcingRepository.cs  [MODIFICAR] Agregar 5 metodos nuevos
│       │   └── IMensajeCrowdsourcingRepository.cs       [CREAR] 3 metodos
│       └── Services/
│           ├── IConversacionCrowdsourcingService.cs     [CREAR] 6 metodos
│           └── IMensajeCrowdsourcingService.cs          [CREAR] 3 metodos
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Context/
    │   └── CrowdsourcingContext.cs               [MODIFICAR] Reemplazar configuraciones inline por ApplyConfiguration
    ├── Data/
    │   └── Configurations/
    │       ├── ConversacionCrowdsourcingConfiguration.cs  [CREAR] Extraer de OnModelCreating + agregar indices
    │       └── MensajeCrowdsourcingConfiguration.cs       [CREAR] Extraer de OnModelCreating + ajustar MaxLengths + indices
    ├── Repositories/
    │   ├── ConversacionCrowdsourcingRepository.cs  [MODIFICAR] Agregar 5 metodos nuevos
    │   └── MensajeCrowdsourcingRepository.cs       [CREAR] 3 metodos
    ├── Services/
    │   ├── AcuerdoCrowdsourcingService.cs          [MODIFICAR] Actualizar nombres de campos en construccion de ConversacionCrowdsourcing
    │   ├── ConversacionCrowdsourcingService.cs     [CREAR] 6 metodos
    │   └── MensajeCrowdsourcingService.cs          [CREAR] 3 metodos
    ├── Migrations/
    │   └── YYYYMMDD_AddMensajeriaIndexesAndRenameConversacionColumns.cs  [GENERAR via EF CLI]
    └── DependencyInjection.cs                     [MODIFICAR] Agregar 3 registros
```

---

## 9. Consideraciones de Diseño Criticas

### 9.1 Resolucion de Nombres para Listado y Mensajes

El `ConversacionListItemDto` requiere `NombreOtraParte`, `ImagenOtraParte`, `ContextoTitulo` y `ContextoTipo`. El `MensajeDto` requiere `RemitenteNombre`. Estos campos NO se pueden derivar solo de las entidades del modulo Crowdsourcing porque los perfiles (nombre, imagen) estan en el modulo `UserAccess` (entidades `Artista` y `PerfilProfesional`).

**Decision de arquitectura:** Estos campos se calculan en los **Handlers** de Application, NO en los repositories ni services. El Handler de `GetConversacionesQuery` inyectara un service de UserAccess (o BuildingBlock) para resolver los perfiles. Esto queda fuera del alcance de este plan de hexagonal architecture (es responsabilidad del plan CQRS).

Sin embargo, el `IConversacionCrowdsourcingRepository.GetConversacionesByUserIdAsync` puede retornar las entidades completas con las navigations cargadas (Necesidad, Acuerdo) para que el Handler pueda extraer los titulos de contexto sin queries adicionales.

### 9.2 Manejo de DbUpdateException por Constraint de Unicidad

Los indices de unicidad filtrados (`UX_ConversacionCrowdsourcing_Usuarios_Necesidad` y `UX_ConversacionCrowdsourcing_Usuarios_Acuerdo`) actuan como guardia de red en la base de datos. El Handler de `CreateConversacionCommand` debe:
1. Primero verificar via `IConversacionCrowdsourcingService.ExisteConversacionParaContextoAsync` (validacion de negocio)
2. Si por race condition se viola el constraint, capturar `DbUpdateException` en el try-catch y retornar `BusinessRule_ConversacionDuplicada` en lugar de `Internal_UnexpectedError`

Esta logica de captura de excepcion va en el Handler, no en el Service.

### 9.3 Cache de Request

El `IRequestCacheService` se usa en `ConversacionCrowdsourcingService` para:
- `GetByIdAsync`: cache por `"conversacion:{id}"` - compartido entre el Validator (valida participacion) y el Handler
- `ExisteConversacionParaContextoAsync`: cache por `"conversacion-existe:{...}"` - compartido entre Validator y Handler
- `GetTotalNoLeidosByUserIdAsync`: cache por `"conversacion-noleidos:{userId}"` - si el Handler de GetConversaciones y el de GetNoLeidos se ejecutan en el mismo request (improbable pero posible en futures tests)

Los listados paginados y los mensajes NO se cachean (datos mutables y dependientes de paginacion).

### 9.4 Operaciones sin UnitOfWork Explicito

El patron del proyecto (observable en `NecesidadCrowdsourcingService`, `PropuestaCrowdsourcingService`, etc.) es que los repositories hacen `SaveChangesAsync` directamente. **No se usa `IUnitOfWork<CrowdsourcingContext>`** en este modulo. Verificar el patron de `AcuerdoCrowdsourcingService` que usa `CrowdsourcingContext` directamente para operaciones complejas.

Para `MensajeCrowdsourcingService.SendAsync`, la secuencia es:
1. `_repository.AddAsync` -> hace `SaveChangesAsync` (persiste el mensaje)
2. `_conversacionRepository.UpdateFechaUltimoMensajeAsync` -> usa `ExecuteUpdateAsync` (operacion directa, sin SaveChanges)

No hay transaccion explicita. Es aceptable para MVP.

---

## 10. Checklist de Verificacion

- [ ] Entidades son POCOs sin metodos de negocio
- [ ] `ConversacionCrowdsourcing` renombrada: `UserIdArtista`->`UserIdCreador`, `UserIdProveedor`->`UserIdDestinatario`
- [ ] `AcuerdoCrowdsourcingService.AceptarPropuestaAsync` actualizado con nuevos nombres de campo
- [ ] `ServiceResponseMessageType.cs` tiene 3 nuevas constantes (`2014`, `4015`, `4016`)
- [ ] `IConversacionCrowdsourcingRepository` extendida con 5 metodos nuevos (mantiene `AddAsync` existente)
- [ ] `IMensajeCrowdsourcingRepository` creada con 3 metodos
- [ ] `IConversacionCrowdsourcingService` creada con 6 metodos
- [ ] `IMensajeCrowdsourcingService` creada con 3 metodos
- [ ] `ConversacionCrowdsourcingService` inyecta `IConversacionCrowdsourcingRepository + IRequestCacheService + ILogger` con `?? throw`
- [ ] `MensajeCrowdsourcingService` inyecta `IMensajeCrowdsourcingRepository + IConversacionCrowdsourcingRepository + IRequestCacheService + ILogger` con `?? throw`
- [ ] Services retornan entidades de dominio, NO DTOs
- [ ] `ConversacionCrowdsourcingConfiguration` extrae la config inline de OnModelCreating + agrega 5 indices nuevos
- [ ] `MensajeCrowdsourcingConfiguration` extrae la config inline de OnModelCreating + ajusta MaxLength(5000) y MaxLength(2048) + agrega 2 indices
- [ ] `CrowdsourcingContext.OnModelCreating` actualizado con `ApplyConfiguration` en lugar de configuraciones inline para Conversacion y Mensaje
- [ ] `ConversacionCrowdsourcingRepository` extendida con 5 metodos nuevos
- [ ] `MensajeCrowdsourcingRepository` creada con 3 metodos (usa `ExecuteUpdateAsync` para update masivo)
- [ ] `DependencyInjection.cs` registra 3 servicios/repos nuevos
- [ ] Migracion generada con rename de columnas, AlterColumn de MaxLengths y creacion de 9 indices
- [ ] Fluent API para todas las configuraciones (sin Data Annotations en entidades)
- [ ] `AsNoTracking()` en todas las queries de lectura
- [ ] `CancellationToken` propagado en todos los metodos async
