# Contratos: Mensajeria entre Partes

> **Feature:** cs-mensajeria (US-CS-05)
> **Ultima actualizacion:** 2026-02-18

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### POST /api/crowdsourcing/conversaciones

**Descripcion:** Crea una nueva conversacion entre dos partes que tienen una relacion preexistente (propuesta enviada o acuerdo activo). Si ya existe una conversacion para el mismo contexto (misma combinacion de userIdCreador + userIdDestinatario + necesidadId o acuerdoId), retorna un error 400 con el ID de la conversacion existente para que el frontend redirija. El UserId del creador se obtiene del token JWT.

**Autorizacion:** Bearer JWT - Usuario autenticado con relacion con el destinatario

**Request Body:**
```json
{
  "necesidadId": "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
  "acuerdoId": null,
  "userIdDestinatario": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "asunto": "Consulta sobre la mezcla de pistas"
}
```

**Nota sobre `necesidadId` y `acuerdoId`:** Ambos son opcionales pero mutuamente excluyentes; exactamente uno de los dos debe estar presente para identificar el contexto. En el flujo de acuerdos (US-CS-04), la conversacion se crea automaticamente al aceptar la propuesta, por lo que este endpoint se usa principalmente para el contexto de necesidades.

**Response 201 Created:**
```json
{
  "data": {
    "id": "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    "asunto": "Consulta sobre la mezcla de pistas",
    "nombreDestinatario": "Studio Mix Pro",
    "contextoTipo": "necesidad",
    "contextoTitulo": "Mezcla de pistas para EP",
    "fechaCreacion": "2026-03-01T10:00:00Z"
  },
  "messages": [
    { "message": "Conversacion creada", "errorCode": "0001" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El asunto es obligatorio | asunto vacio o nulo |
| 400 | 1002 | El asunto no puede superar los 200 caracteres | asunto > 200 chars |
| 400 | 1001 | El destinatario es obligatorio | userIdDestinatario vacio o nulo |
| 400 | 4015 | Ya existe una conversacion para este contexto | Existe ConversacionCrowdsourcing con mismos userIds y contexto |
| 403 | 3002 | No tienes relacion con este destinatario | No existe propuesta o acuerdo vinculando a ambos usuarios |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 500 | 5000 | Error inesperado al crear la conversacion | Excepcion no controlada |

---

### GET /api/crowdsourcing/conversaciones

**Descripcion:** Lista las conversaciones del usuario autenticado ordenadas por FechaUltimoMensaje descendente (conversaciones sin mensajes quedan al final). Incluye contador de mensajes no leidos por conversacion y total global de no leidos. El UserId se obtiene del token JWT.

**Autorizacion:** Bearer JWT - Usuario autenticado

**Query Parameters:**
- `contexto` (string, opcional) - Filtrar por tipo: `todas` (default), `necesidades`, `acuerdos`
- `page` (int, default: 1) - Numero de pagina
- `pageSize` (int, default: 20, max: 50) - Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
        "asunto": "Consulta sobre la mezcla de pistas",
        "nombreOtraParte": "Studio Mix Pro",
        "imagenOtraParte": "https://storage.weplay.com/avatars/studio-mix-pro.jpg",
        "contextoTipo": "necesidad",
        "contextoTitulo": "Mezcla de pistas para EP",
        "ultimoMensaje": "Perfecto, te envio los stems manana por la manana...",
        "fechaUltimoMensaje": "2026-03-02T15:30:00Z",
        "mensajesNoLeidos": 2
      }
    ],
    "totalCount": 5,
    "totalNoLeidos": 3,
    "page": 1,
    "pageSize": 20
  },
  "messages": []
}
```

**Nota sobre `ultimoMensaje`:** Truncado a 80 caracteres con "..." si supera ese limite. Es null si la conversacion no tiene mensajes aun.

**Nota sobre `imagenOtraParte`:** Puede ser null si el usuario no tiene foto de perfil.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 500 | 5000 | Error inesperado al obtener conversaciones | Excepcion no controlada |

---

### GET /api/crowdsourcing/conversaciones/{id}/mensajes

**Descripcion:** Retorna los mensajes de una conversacion paginados en orden cronologico ascendente (mas antiguos primero). Solo los dos participantes de la conversacion pueden acceder. El campo `esPropio` se calcula comparando `UserIdRemitente` con el `UserId` del token.

**Autorizacion:** Bearer JWT - Participante de la conversacion

**Path Parameters:**
- `id` (Guid) - ID de la conversacion

**Query Parameters:**
- `page` (int, default: 1) - Numero de pagina
- `pageSize` (int, default: 50, max: 100) - Elementos por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
        "contenido": "Hola, me interesa tu propuesta. Podrias contarme mas sobre tu experiencia?",
        "urlAdjunto": null,
        "remitenteNombre": "Los Rockeros",
        "esPropio": false,
        "leido": true,
        "fechaCreacion": "2026-03-01T10:05:00Z"
      },
      {
        "id": "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
        "contenido": "Claro! He trabajado con bandas como...",
        "urlAdjunto": "https://drive.google.com/portfolio",
        "remitenteNombre": "Studio Mix Pro",
        "esPropio": true,
        "leido": true,
        "fechaCreacion": "2026-03-01T10:15:00Z"
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 50
  },
  "messages": []
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes acceso a esta conversacion | UserId del token no es ni creador ni destinatario |
| 404 | 2014 | Conversacion no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al obtener mensajes | Excepcion no controlada |

---

### POST /api/crowdsourcing/conversaciones/{id}/mensajes

**Descripcion:** Envia un nuevo mensaje en una conversacion. Solo los dos participantes pueden enviar mensajes. Al crear el mensaje se actualiza `FechaUltimoMensaje` de la conversacion. El mensaje se crea con `Leido = false` y `FechaLeido = null`.

**Autorizacion:** Bearer JWT - Participante de la conversacion

**Path Parameters:**
- `id` (Guid) - ID de la conversacion

**Request Body:**
```json
{
  "contenido": "Perfecto, te envio los stems manana por la manana",
  "urlAdjunto": null
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    "contenido": "Perfecto, te envio los stems manana por la manana",
    "urlAdjunto": null,
    "remitenteNombre": "Studio Mix Pro",
    "esPropio": true,
    "leido": false,
    "fechaCreacion": "2026-03-02T15:30:00Z"
  },
  "messages": [
    { "message": "Mensaje enviado", "errorCode": "0001" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El contenido del mensaje es obligatorio | contenido vacio o nulo |
| 400 | 1002 | El mensaje no puede superar los 5000 caracteres | contenido > 5000 chars |
| 400 | 1013 | La URL adjunta no es valida | urlAdjunto presente pero no es una URL valida |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes acceso a esta conversacion | UserId del token no es ni creador ni destinatario |
| 404 | 2014 | Conversacion no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al enviar el mensaje | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos

**Descripcion:** Marca como leidos todos los mensajes no leidos del otro participante en la conversacion (es decir, mensajes donde `UserIdRemitente != UserId del token` y `Leido == false`). Establece `Leido = true` y `FechaLeido = DateTime.UtcNow`. Se invoca automaticamente al abrir una conversacion.

**Autorizacion:** Bearer JWT - Participante de la conversacion

**Path Parameters:**
- `id` (Guid) - ID de la conversacion

**Request Body:** Ninguno.

**Response 200 OK:**
```json
{
  "data": {
    "mensajesMarcados": 3
  },
  "messages": []
}
```

**Nota:** Si no hay mensajes no leidos, retorna 200 con `mensajesMarcados: 0` (no es un error).

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes acceso a esta conversacion | UserId del token no es ni creador ni destinatario |
| 404 | 2014 | Conversacion no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al marcar mensajes como leidos | Excepcion no controlada |

---

### GET /api/crowdsourcing/conversaciones/no-leidos

**Descripcion:** Retorna el total de mensajes no leidos del usuario autenticado en todas sus conversaciones. Endpoint ligero para el badge del navbar. Se puede invocar con polling cada 10 segundos.

**Autorizacion:** Bearer JWT - Usuario autenticado

**Response 200 OK:**
```json
{
  "data": {
    "totalNoLeidos": 5
  },
  "messages": []
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 500 | 5000 | Error inesperado al obtener no leidos | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Notas |
|----------|------|-------|
| `POST /api/crowdsourcing/conversaciones` | Bearer JWT | UserId del token; valida relacion con destinatario |
| `GET /api/crowdsourcing/conversaciones` | Bearer JWT | Filtra por UserId del token (como creador o destinatario) |
| `GET /api/crowdsourcing/conversaciones/{id}/mensajes` | Bearer JWT | Valida que UserId del token sea creador o destinatario |
| `POST /api/crowdsourcing/conversaciones/{id}/mensajes` | Bearer JWT | Valida que UserId del token sea creador o destinatario |
| `PATCH /api/crowdsourcing/conversaciones/{id}/marcar-leidos` | Bearer JWT | Valida que UserId del token sea creador o destinatario |
| `GET /api/crowdsourcing/conversaciones/no-leidos` | Bearer JWT | Filtra por UserId del token |

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

**Nota:** Los claims `artistaId` y `perfilProfesionalId` no se incluyen en el token. El backend resuelve la identidad del participante por `UserId` directamente, ya que `ConversacionCrowdsourcing` almacena `UserIdCreador` y `UserIdDestinatario` como strings de Identity (no GUIDs de entidades de dominio). La validacion de relacion previa (para crear conversacion) se implementa verificando si existe una `PropuestaCrowdsourcing` o `AcuerdoCrowdsourcing` que vincule a ambos usuarios.

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Proteccion de Rutas Frontend

| Ruta | Auth | Redirect | Notas |
|------|------|----------|-------|
| `/crowdsourcing/mensajes` | Bearer JWT | `/auth/login` | Listado de conversaciones (Landing) |
| `/crowdsourcing/mensajes/:id` | Bearer JWT | `/auth/login` | Vista de chat de una conversacion (Landing) |

---

## DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateConversacionRequestDto.cs
public class CreateConversacionRequestDto
{
    /// <summary>Opcional. FK a NecesidadCrowdsourcing. Mutuamente excluyente con AcuerdoId.</summary>
    public Guid? NecesidadId { get; set; }
    /// <summary>Opcional. FK a AcuerdoCrowdsourcing. Mutuamente excluyente con NecesidadId.</summary>
    public Guid? AcuerdoId { get; set; }
    /// <summary>Requerido. UserId del destinatario (Identity User string).</summary>
    public string UserIdDestinatario { get; set; } = null!;
    /// <summary>Requerido. Asunto de la conversacion. Min 1, max 200 chars.</summary>
    public string Asunto { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateConversacionResultDto.cs
public class CreateConversacionResultDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreDestinatario { get; set; } = null!;
    /// <summary>"necesidad" o "acuerdo" segun el contexto de la conversacion.</summary>
    public string ContextoTipo { get; set; } = null!;
    /// <summary>Titulo de la necesidad o del acuerdo vinculado.</summary>
    public string ContextoTitulo { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ConversacionListItemDto.cs
public class ConversacionListItemDto
{
    public Guid Id { get; set; }
    public string Asunto { get; set; } = null!;
    public string NombreOtraParte { get; set; } = null!;
    /// <summary>URL de imagen de perfil de la otra parte. Puede ser null.</summary>
    public string? ImagenOtraParte { get; set; }
    /// <summary>"necesidad" o "acuerdo".</summary>
    public string ContextoTipo { get; set; } = null!;
    public string ContextoTitulo { get; set; } = null!;
    /// <summary>Contenido del ultimo mensaje truncado a 80 chars. Null si no hay mensajes.</summary>
    public string? UltimoMensaje { get; set; }
    /// <summary>Fecha del ultimo mensaje. Null si no hay mensajes.</summary>
    public DateTime? FechaUltimoMensaje { get; set; }
    /// <summary>COUNT de mensajes donde Leido = false y UserIdRemitente != UserId del solicitante.</summary>
    public int MensajesNoLeidos { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/ConversacionListResponseDto.cs
public class ConversacionListResponseDto
{
    public List<ConversacionListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    /// <summary>Suma de MensajesNoLeidos de todas las conversaciones del usuario (no paginado).</summary>
    public int TotalNoLeidos { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MensajeDto.cs
public class MensajeDto
{
    public Guid Id { get; set; }
    public string Contenido { get; set; } = null!;
    /// <summary>URL del adjunto. Null si no tiene adjunto.</summary>
    public string? UrlAdjunto { get; set; }
    public string RemitenteNombre { get; set; } = null!;
    /// <summary>true si UserIdRemitente == UserId del solicitante.</summary>
    public bool EsPropio { get; set; }
    public bool Leido { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MensajeListResponseDto.cs
public class MensajeListResponseDto
{
    public List<MensajeDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateMensajeRequestDto.cs
public class CreateMensajeRequestDto
{
    /// <summary>Requerido. Min 1, max 5000 chars.</summary>
    public string Contenido { get; set; } = null!;
    /// <summary>Opcional. URL valida (Dropbox, Drive, WeTransfer, etc.).</summary>
    public string? UrlAdjunto { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MarcarLeidosResponseDto.cs
public class MarcarLeidosResponseDto
{
    /// <summary>Numero de mensajes que pasaron de Leido=false a Leido=true en esta operacion.</summary>
    public int MensajesMarcados { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/NoLeidosCountResponseDto.cs
public class NoLeidosCountResponseDto
{
    /// <summary>Total de mensajes no leidos del usuario en todas sus conversaciones.</summary>
    public int TotalNoLeidos { get; set; }
}
```

### Modelos de Dominio (Entidades C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Domain/Model/ConversacionCrowdsourcing.cs
public class ConversacionCrowdsourcing
{
    public Guid Id { get; set; }
    /// <summary>FK opcional -> NecesidadCrowdsourcing. Mutuamente excluyente con AcuerdoId.</summary>
    public Guid? NecesidadId { get; set; }
    /// <summary>FK opcional -> AcuerdoCrowdsourcing. Mutuamente excluyente con NecesidadId.</summary>
    public Guid? AcuerdoId { get; set; }
    /// <summary>FK -> Identity AspNetUsers. UserId del usuario que creo la conversacion.</summary>
    public string UserIdCreador { get; set; } = null!;
    /// <summary>FK -> Identity AspNetUsers. UserId del destinatario.</summary>
    public string UserIdDestinatario { get; set; } = null!;
    /// <summary>Asunto de la conversacion. Max 200 chars.</summary>
    public string Asunto { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    /// <summary>Fecha del ultimo mensaje enviado. Null si la conversacion no tiene mensajes aun.</summary>
    public DateTime? FechaUltimoMensaje { get; set; }

    // Navigation properties
    public NecesidadCrowdsourcing? Necesidad { get; set; }
    public AcuerdoCrowdsourcing? Acuerdo { get; set; }
    public ICollection<MensajeCrowdsourcing> Mensajes { get; set; } = new List<MensajeCrowdsourcing>();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Domain/Model/MensajeCrowdsourcing.cs
public class MensajeCrowdsourcing
{
    public Guid Id { get; set; }
    /// <summary>FK -> ConversacionCrowdsourcing.</summary>
    public Guid ConversacionId { get; set; }
    /// <summary>FK -> Identity AspNetUsers. UserId del usuario que envio el mensaje.</summary>
    public string UserIdRemitente { get; set; } = null!;
    /// <summary>Contenido del mensaje. Max 5000 chars.</summary>
    public string Contenido { get; set; } = null!;
    /// <summary>URL de adjunto externo. Opcional. Debe ser URL valida.</summary>
    public string? UrlAdjunto { get; set; }
    /// <summary>false al crearse. Se actualiza a true al invocar marcar-leidos.</summary>
    public bool Leido { get; set; } = false;
    /// <summary>Null al crearse. Se establece en DateTime.UtcNow al marcar como leido.</summary>
    public DateTime? FechaLeido { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public ConversacionCrowdsourcing Conversacion { get; set; } = null!;
}
```

### Configuracion EF Core

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Infra/Persistence/Configurations/ConversacionCrowdsourcingConfiguration.cs
public class ConversacionCrowdsourcingConfiguration : IEntityTypeConfiguration<ConversacionCrowdsourcing>
{
    public void Configure(EntityTypeBuilder<ConversacionCrowdsourcing> builder)
    {
        builder.ToTable("ConversacionesCrowdsourcing");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserIdCreador).HasMaxLength(450).IsRequired();
        builder.Property(x => x.UserIdDestinatario).HasMaxLength(450).IsRequired();
        builder.Property(x => x.Asunto).HasMaxLength(200).IsRequired();

        // Relaciones opcionales con contexto
        builder.HasOne(x => x.Necesidad)
            .WithMany()
            .HasForeignKey(x => x.NecesidadId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Acuerdo)
            .WithMany()
            .HasForeignKey(x => x.AcuerdoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Indice para evitar conversaciones duplicadas por contexto
        // Unicidad: (UserIdCreador, UserIdDestinatario, NecesidadId) cuando NecesidadId no es null
        // Unicidad: (UserIdCreador, UserIdDestinatario, AcuerdoId) cuando AcuerdoId no es null
        // Implementar como restriccion de unicidad filtrada en migracion manual
        builder.HasIndex(x => new { x.UserIdCreador, x.UserIdDestinatario, x.NecesidadId })
            .HasFilter("[NecesidadId] IS NOT NULL")
            .IsUnique();

        builder.HasIndex(x => new { x.UserIdCreador, x.UserIdDestinatario, x.AcuerdoId })
            .HasFilter("[AcuerdoId] IS NOT NULL")
            .IsUnique();

        // Indice para listado ordenado por fecha
        builder.HasIndex(x => x.FechaUltimoMensaje);
    }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Infra/Persistence/Configurations/MensajeCrowdsourcingConfiguration.cs
public class MensajeCrowdsourcingConfiguration : IEntityTypeConfiguration<MensajeCrowdsourcing>
{
    public void Configure(EntityTypeBuilder<MensajeCrowdsourcing> builder)
    {
        builder.ToTable("MensajesCrowdsourcing");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserIdRemitente).HasMaxLength(450).IsRequired();
        builder.Property(x => x.Contenido).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.UrlAdjunto).HasMaxLength(2048).IsRequired(false);

        builder.HasOne(x => x.Conversacion)
            .WithMany(x => x.Mensajes)
            .HasForeignKey(x => x.ConversacionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indice para consultas de mensajes no leidos y orden cronologico
        builder.HasIndex(x => new { x.ConversacionId, x.Leido, x.UserIdRemitente });
        builder.HasIndex(x => new { x.ConversacionId, x.FechaCreacion });
    }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts
// Agregar las siguientes interfaces al archivo existente

// --- Requests ---

export interface CreateConversacionRequest {
  necesidadId?: string;
  acuerdoId?: string;
  userIdDestinatario: string;
  asunto: string;
}

export interface CreateMensajeRequest {
  contenido: string;
  urlAdjunto?: string;
}

// --- Results ---

export interface CreateConversacionResult {
  id: string;
  asunto: string;
  nombreDestinatario: string;
  contextoTipo: ContextoConversacion;
  contextoTitulo: string;
  fechaCreacion: string;
}

export interface ConversacionListItem {
  id: string;
  asunto: string;
  nombreOtraParte: string;
  imagenOtraParte?: string;
  contextoTipo: ContextoConversacion;
  contextoTitulo: string;
  ultimoMensaje?: string;
  fechaUltimoMensaje?: string;
  mensajesNoLeidos: number;
}

export interface ConversacionListResponse {
  items: ConversacionListItem[];
  totalCount: number;
  totalNoLeidos: number;
  page: number;
  pageSize: number;
}

export interface Mensaje {
  id: string;
  contenido: string;
  urlAdjunto?: string;
  remitenteNombre: string;
  esPropio: boolean;
  leido: boolean;
  fechaCreacion: string;
}

export interface MensajeListResponse {
  items: Mensaje[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface MarcarLeidosResponse {
  mensajesMarcados: number;
}

export interface NoLeidosCountResponse {
  totalNoLeidos: number;
}

// --- Union Types ---

export type ContextoConversacion = 'necesidad' | 'acuerdo';
export type FiltroConversacion = 'todas' | 'necesidades' | 'acuerdos';
```

---

## Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| asunto (crear conversacion) | requerido | `.NotEmpty().WithMessage("El asunto es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El asunto es obligatorio')` |
| asunto (crear conversacion) | max 200 chars | `.MaximumLength(200).WithMessage("Maximo 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'El asunto no puede superar los 200 caracteres')` |
| userIdDestinatario | requerido | `.NotEmpty().WithMessage("El destinatario es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El destinatario es obligatorio')` |
| contenido (mensaje) | requerido | `.NotEmpty().WithMessage("El mensaje no puede estar vacio").WithErrorCode(Validation_Required)` | `.min(1, 'El mensaje no puede estar vacio')` |
| contenido (mensaje) | max 5000 chars | `.MaximumLength(5000).WithMessage("Maximo 5000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(5000, 'El mensaje no puede superar los 5000 caracteres')` |
| urlAdjunto (mensaje) | URL valida si presente | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlAdjunto)).WithMessage("Debe ser una URL valida").WithErrorCode(Validation_InvalidUrl)` | `.url('Debe ser una URL valida').optional().or(z.literal(''))` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
// Agregar al archivo existente

import { z } from 'zod';

export const createConversacionSchema = z.object({
  necesidadId: z
    .string()
    .uuid('ID de necesidad invalido')
    .optional(),
  acuerdoId: z
    .string()
    .uuid('ID de acuerdo invalido')
    .optional(),
  userIdDestinatario: z
    .string()
    .min(1, 'El destinatario es obligatorio'),
  asunto: z
    .string()
    .min(1, 'El asunto es obligatorio')
    .max(200, 'El asunto no puede superar los 200 caracteres'),
});

export const createMensajeSchema = z.object({
  contenido: z
    .string()
    .min(1, 'El mensaje no puede estar vacio')
    .max(5000, 'El mensaje no puede superar los 5000 caracteres'),
  urlAdjunto: z
    .string()
    .url('Debe ser una URL valida')
    .optional()
    .or(z.literal('')),
});

export type CreateConversacionFormData = z.infer<typeof createConversacionSchema>;
export type CreateMensajeFormData = z.infer<typeof createMensajeSchema>;
```

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Polling interval para mensajeria (MVP sin WebSocket)
export const MENSAJERIA_POLLING_INTERVAL_MS = 10000; // 10 segundos

// Longitud maxima de preview del ultimo mensaje en listado
export const MENSAJERIA_PREVIEW_MAX_LENGTH = 80;

// Filtros de conversacion
export const FILTRO_CONVERSACION = {
  TODAS: 'todas',
  NECESIDADES: 'necesidades',
  ACUERDOS: 'acuerdos',
} as const;

// Query Keys - agregar dentro de QUERY_KEYS.crowdsourcing:
//
// conversaciones: {
//   lista: (filtro?: string) => ['crowdsourcing', 'conversaciones', filtro ?? 'todas'] as const,
//   mensajes: (id: string, page?: number) => ['crowdsourcing', 'conversaciones', id, 'mensajes', page ?? 1] as const,
//   noLeidos: ['crowdsourcing', 'conversaciones', 'no-leidos'] as const,
// }
export const QUERY_KEYS = {
  crowdsourcing: {
    // ... claves existentes de US anteriores ...
    conversaciones: {
      lista: (filtro?: string) =>
        ['crowdsourcing', 'conversaciones', filtro ?? 'todas'] as const,
      mensajes: (id: string, page?: number) =>
        ['crowdsourcing', 'conversaciones', id, 'mensajes', page ?? 1] as const,
      noLeidos: ['crowdsourcing', 'conversaciones', 'no-leidos'] as const,
    },
  },
};

// API Routes - agregar dentro de API_ROUTES.crowdsourcing:
export const API_ROUTES = {
  crowdsourcing: {
    // ... rutas existentes de US anteriores ...
    conversaciones: {
      base: '/api/crowdsourcing/conversaciones',
      noLeidos: '/api/crowdsourcing/conversaciones/no-leidos',
      mensajes: (id: string) =>
        `/api/crowdsourcing/conversaciones/${id}/mensajes`,
      marcarLeidos: (id: string) =>
        `/api/crowdsourcing/conversaciones/${id}/marcar-leidos`,
    },
  },
};

// App Routes - agregar dentro de APP_ROUTES.landing:
export const APP_ROUTES = {
  landing: {
    // ... rutas existentes ...
    mensajes: '/crowdsourcing/mensajes',
    mensajeDetail: (id: string) => `/crowdsourcing/mensajes/${id}`,
  },
};
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... mensajes existentes de US anteriores ...

  // Crowdsourcing - Mensajeria
  '2014': 'La conversacion no fue encontrada',
  '4015': 'Ya existe una conversacion para este contexto',

  // Claves semanticas para uso interno
  CONVERSACION_NOT_FOUND: 'La conversacion no fue encontrada',
  CONVERSACION_DUPLICADA: 'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la existente.',
  CONVERSACION_NO_RELACION: 'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.',
  MENSAJE_CONTENIDO_VACIO: 'El mensaje no puede estar vacio',
  MENSAJE_CONTENIDO_MAX: 'El mensaje es demasiado largo (maximo 5000 caracteres)',
  MENSAJE_URL_INVALIDA: 'La URL adjunta no es valida. Verifica que sea una URL completa (ej: https://...)',
};
```

---

## Eventos SignalR

**No aplica para el MVP.** La mensajeria usa polling en lugar de WebSocket.

- El listado de conversaciones y el badge de no leidos se actualizan mediante polling cada 10 segundos usando `MENSAJERIA_POLLING_INTERVAL_MS`.
- Los mensajes del chat activo se actualizan mediante polling cada 10 segundos o refresh manual.
- La constante `MENSAJERIA_POLLING_INTERVAL_MS = 10000` controla el intervalo en todos los puntos de polling.

**Nota para futuras iteraciones:** Si se agrega SignalR, los eventos candidatos serian `NuevoMensaje` (payload: MensajeDto + conversacionId) y `MensajesLeidos` (payload: conversacionId + mensajesMarcados).

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo `Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// NotFound (2000-2999)
public const string NotFound_Conversacion = "2014";

// Business Rules (4000-4999)
public const string BusinessRule_ConversacionDuplicada = "4015";    // Ya existe conversacion para el mismo contexto
public const string BusinessRule_NoRelacionConDestinatario = "4016"; // No existe propuesta/acuerdo que vincule a los usuarios
```

**Estado actual de la tabla de constantes tras esta US:**

| Codigo | Constante | Categoria |
|--------|-----------|-----------|
| 1001 | Validation_Required | Validation |
| 1002 | Validation_MaxLength | Validation |
| 1003 | Validation_InvalidEmail | Validation |
| 1009 | Validation_InvalidRange | Validation |
| 1010 | Validation_ForeignKeyNotFound | Validation |
| 1011 | Validation_MinLength | Validation |
| 1012 | Validation_InvalidDate | Validation |
| 1013 | Validation_InvalidUrl | Validation |
| 2000 | NotFound_Entity | NotFound |
| 2006 | NotFound_PlantillaProyecto | NotFound |
| 2007 | NotFound_ProyectoArtistico | NotFound |
| 2008 | NotFound_PlantillaNecesidad | NotFound |
| 2009 | NotFound_Necesidad | NotFound |
| 2010 | NotFound_Propuesta | NotFound |
| 2011 | NotFound_Acuerdo | NotFound |
| 2012 | NotFound_Milestone | NotFound |
| 2013 | NotFound_Entregable | NotFound |
| 2014 | NotFound_Conversacion | NotFound - NUEVO |
| 3001 | Auth_Unauthorized | Auth |
| 3002 | Auth_Forbidden | Auth |
| 4000 | BusinessRule_InvalidOperation | BusinessRule |
| 4001 | BusinessRule_NecesidadNotEditable | BusinessRule |
| 4002 | BusinessRule_NecesidadNotCloseable | BusinessRule |
| 4003 | BusinessRule_AlreadyProposed | BusinessRule |
| 4004 | BusinessRule_CannotProposeSelf | BusinessRule |
| 4005 | BusinessRule_PropuestaNotRetirable | BusinessRule |
| 4006 | BusinessRule_NoProfessionalProfile | BusinessRule |
| 4007 | BusinessRule_PropuestaNotAcceptable | BusinessRule |
| 4008 | BusinessRule_AcuerdoAlreadyExists | BusinessRule |
| 4009 | BusinessRule_MilestoneImporteExceeded | BusinessRule |
| 4010 | BusinessRule_AcuerdoNotActive | BusinessRule |
| 4011 | BusinessRule_MilestoneCompleted | BusinessRule |
| 4012 | BusinessRule_MilestoneHasEntregables | BusinessRule |
| 4013 | BusinessRule_EntregableNotReviewable | BusinessRule |
| 4014 | BusinessRule_InvalidState | BusinessRule |
| 4015 | BusinessRule_ConversacionDuplicada | BusinessRule - NUEVO |
| 4016 | BusinessRule_NoRelacionConDestinatario | BusinessRule - NUEVO |
| 5000 | Internal_UnexpectedError | Internal |
| 5001 | Internal_DatabaseError | Internal |

---

## Notas de Implementacion

### Backend

1. **GetConversacionesQuery:**
   - Filtrar todas las conversaciones donde `UserIdCreador == UserId del token` OR `UserIdDestinatario == UserId del token`.
   - Aplicar filtro de contexto: `necesidades` (NecesidadId IS NOT NULL), `acuerdos` (AcuerdoId IS NOT NULL).
   - Ordenar: conversaciones con FechaUltimoMensaje NOT NULL primero (desc), luego las que tienen null (ordenadas por FechaCreacion desc).
   - Calcular `NombreOtraParte` e `ImagenOtraParte` consultando el perfil del usuario contrario (artista o profesional segun corresponda).
   - Calcular `MensajesNoLeidos` = COUNT de MensajeCrowdsourcing WHERE ConversacionId = c.Id AND UserIdRemitente != UserId del token AND Leido = false.
   - Calcular `TotalNoLeidos` como suma de todos los MensajesNoLeidos antes de paginar (query separada o subconsulta).
   - Truncar `UltimoMensaje` a 80 caracteres si supera ese limite.
   - Usar `AsNoTracking()`.

2. **GetMensajesQuery:**
   - Validar que el UserId del token es `UserIdCreador` o `UserIdDestinatario`; si no, retornar `Auth_Forbidden`.
   - Ordenar por `FechaCreacion ASC` (cronologico ascendente).
   - Calcular `EsPropio` = `UserIdRemitente == UserId del token`.
   - Obtener `RemitenteNombre` consultando perfil del remitente.
   - Usar `AsNoTracking()`.

3. **CreateConversacionCommand:**
   - Validar que `Asunto` no este vacio y no supere 200 chars.
   - Validar que `UserIdDestinatario` no este vacio.
   - Validar relacion previa: existe `PropuestaCrowdsourcing` o `AcuerdoCrowdsourcing` que vincule a `UserIdCreador` y `UserIdDestinatario` en el contexto indicado. Si no existe, retornar `BusinessRule_NoRelacionConDestinatario` (4016).
   - Verificar si ya existe una conversacion con los mismos participantes y el mismo contexto; si existe, retornar `BusinessRule_ConversacionDuplicada` (4015). El frontend debe detectar este error e interpretar que debe redirigir.
   - Crear con `FechaCreacion = DateTime.UtcNow` y `FechaUltimoMensaje = null`.

4. **CreateMensajeCommand:**
   - Validar que el UserId del token es participante de la conversacion; si no, `Auth_Forbidden`.
   - Crear `MensajeCrowdsourcing` con `Leido = false`, `FechaLeido = null`, `FechaCreacion = DateTime.UtcNow`.
   - Actualizar `ConversacionCrowdsourcing.FechaUltimoMensaje = DateTime.UtcNow` (dentro de la misma transaccion o con Update explicito).
   - Retornar `MensajeDto` completo con `EsPropio = true` y `RemitenteNombre` del usuario autenticado.

5. **MarcarLeidosCommand:**
   - Validar que el UserId del token es participante; si no, `Auth_Forbidden`.
   - Actualizar en batch: `UPDATE MensajesCrowdsourcing SET Leido = 1, FechaLeido = UTC_NOW WHERE ConversacionId = @id AND UserIdRemitente != @userId AND Leido = 0`.
   - Retornar count de filas afectadas como `MensajesMarcados`.

6. **GetNoLeidosCountQuery:**
   - Query ligera: `SELECT COUNT(*) FROM MensajesCrowdsourcing m INNER JOIN ConversacionesCrowdsourcing c ON c.Id = m.ConversacionId WHERE (c.UserIdCreador = @userId OR c.UserIdDestinatario = @userId) AND m.UserIdRemitente != @userId AND m.Leido = 0`.
   - Considerar cachear en `IRequestCacheService` para el scope del request (el listado y el badge pueden reutilizarlo).

7. **Caching:** Usar `IRequestCacheService` para la resolucion del nombre e imagen de perfil por UserId (compartida entre Validator y Handler en cada request, evita multiples queries por participante).

8. **Indice de unicidad:** La restriccion de conversacion duplicada se implementa como indice unico filtrado en SQL Server. El handler debe capturar `DbUpdateException` con codigo de constraint y retornar `BusinessRule_ConversacionDuplicada` en lugar de `Internal_UnexpectedError`.

### Frontend

1. **Nuevos archivos recomendados:**
   - `src/web/src/features/crowdsourcing/mensajeria/` - Feature de mensajeria (Landing)
   - `src/shared/schemas/crowdsourcing.schema.ts` - Agregar `createConversacionSchema` y `createMensajeSchema`
   - `src/shared/types/crowdsourcing.ts` - Agregar los nuevos tipos de mensajeria

2. **Componentes clave (Landing):**
   - `MensajesPage.tsx` - Listado de conversaciones con filtros y badge total
   - `ConversacionCard.tsx` - Item de conversacion con avatar, nombre, preview y badge de no leidos
   - `ConversacionChatPage.tsx` - Vista de chat con cabecera de contexto
   - `ChatHeader.tsx` - Cabecera con nombre de la otra parte, asunto y contexto vinculado
   - `ChatMessageList.tsx` - Lista de mensajes con scroll; propios a la derecha, ajenos a la izquierda
   - `ChatMessage.tsx` - Burbuja de mensaje con nombre, contenido, adjunto, hora y estado de lectura
   - `ChatInput.tsx` - Input de texto con boton de enviar y campo de URL adjunto opcional
   - `NavbarMensajesBadge.tsx` - Badge numerico en navbar que muestra `totalNoLeidos`
   - `IniciarConversacionDialog.tsx` - Dialogo para crear conversacion con campo de asunto

3. **Hooks personalizados:**
   - `useConversaciones(filtro?)` - Query con polling de `MENSAJERIA_POLLING_INTERVAL_MS`; usa `QUERY_KEYS.crowdsourcing.conversaciones.lista(filtro)`
   - `useMensajes(conversacionId, page?)` - Query con polling de `MENSAJERIA_POLLING_INTERVAL_MS`; usa `QUERY_KEYS.crowdsourcing.conversaciones.mensajes(id, page)`
   - `useNoLeidosCount()` - Query con polling de `MENSAJERIA_POLLING_INTERVAL_MS`; usa `QUERY_KEYS.crowdsourcing.conversaciones.noLeidos`
   - `useCreateConversacion()` - Mutation; en error 4015 (duplicada), redirigir a la conversacion existente en lugar de mostrar error
   - `useEnviarMensaje(conversacionId)` - Mutation; invalida `QUERY_KEYS.crowdsourcing.conversaciones.mensajes(conversacionId)` y `QUERY_KEYS.crowdsourcing.conversaciones.lista()`
   - `useMarcarLeidos(conversacionId)` - Mutation; se invoca automaticamente al abrir el chat; invalida `noLeidos` y `lista`

4. **UX considerations:**
   - Al abrir un chat, invocar `useMarcarLeidos` automaticamente antes de renderizar los mensajes.
   - El badge de no leidos del navbar se actualiza por polling independiente del chat activo.
   - Mensajes propios alineados a la derecha con fondo de color de acento; ajenos a la izquierda con fondo neutro.
   - Mostrar indicador visual de carga mientras se envia un mensaje; deshabilitar el input durante el envio.
   - Si la conversacion ya existe (error 4015), mostrar toast informativo y navegar al chat existente.
   - Empty state en el listado: "No tienes conversaciones activas. Puedes iniciar una desde una necesidad o acuerdo."
   - Filtros de contexto como tabs o pills: Todas / Necesidades / Acuerdos.
   - Paginacion de mensajes: cargar pagina 1 al abrir; opcion "Cargar mensajes anteriores" para paginas siguientes (scroll hacia arriba).

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (6 endpoints documentados)
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (CreateConversacionRequestDto, CreateConversacionResultDto, ConversacionListItemDto, ConversacionListResponseDto, MensajeDto, MensajeListResponseDto, CreateMensajeRequestDto, MarcarLeidosResponseDto, NoLeidosCountResponseDto)
- [x] Modelos de dominio C# (ConversacionCrowdsourcing, MensajeCrowdsourcing)
- [x] Configuracion EF Core con indices y restricciones de unicidad
- [x] Types TypeScript equivalentes (CreateConversacionRequest, CreateMensajeRequest, CreateConversacionResult, ConversacionListItem, ConversacionListResponse, Mensaje, MensajeListResponse, MarcarLeidosResponse, NoLeidosCountResponse)
- [x] Union types (ContextoConversacion, FiltroConversacion)
- [x] Schemas Zod con mismas reglas (createConversacionSchema, createMensajeSchema)
- [x] Constantes compartidas (MENSAJERIA_POLLING_INTERVAL_MS, MENSAJERIA_PREVIEW_MAX_LENGTH, FILTRO_CONVERSACION, QUERY_KEYS additions, API_ROUTES additions, APP_ROUTES additions)
- [x] Nuevas constantes ServiceResponseMessageType (2014, 4015, 4016)
- [x] Tabla de estado completo de todas las constantes del modulo Crowdsourcing
- [x] Mapeo de errores a mensajes UI
- [x] Comportamiento de polling documentado (sin SignalR en MVP)
- [x] Notas de implementacion detalladas (Backend + Frontend)
- [x] Comportamiento de redireccion en conversacion duplicada documentado
