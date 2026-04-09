# Contratos: Inscripcion en Programa

> **Feature:** cp-inscripcion-programa (US-CP-03)
> **Ultima actualizacion:** 2026-02-25
> **Modulo Backend:** Crowdpromotion
> **Depende de:** US-CP-01 (cp-perfil-promotor), US-CP-02 (cp-programas-promocion)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Cambios al Modelo de Dominio

### PromoProgramaPromotor - Campos Nuevos Requeridos

La entidad `PromoProgramaPromotor` actual **NO tiene** los campos `EsAprobado` ni `EsBloqueado`. Estos campos son obligatorios para implementar el flujo de aprobacion de inscripciones. Se deben agregar antes de implementar cualquier comando de esta feature.

**Campos a agregar a la entidad:**

```csharp
// Agregar en WePlayRises.Crowdpromotion.Domain.Model.PromoProgramaPromotor

/// <summary>
/// Indica si el artista ha aprobado la inscripcion del promotor.
/// Default: false (solicitud pendiente). Al aprobar: true + CodigoReferido generado.
/// </summary>
public bool EsAprobado { get; set; }

/// <summary>
/// Indica si el artista ha bloqueado al promotor en este programa.
/// Un promotor bloqueado no puede re-solicitar inscripcion.
/// Default: false.
/// </summary>
public bool EsBloqueado { get; set; }
```

**Nota sobre el campo `UrlReferido`:** La entidad actual tiene `UrlReferido`. En este contrato se mapea como `UrlTrackingPersonalizada` en los DTOs para mayor claridad. El nombre de la columna en BD no se modifica.

**Nota sobre el campo `FechaInscripcion`:** La entidad actual usa `FechaInscripcion` en lugar de `FechaAlta`. Los DTOs exponen este campo como `fechaAlta` para consistencia con la US.

### Maquina de Estados de PromoProgramaPromotor

```
Creado (EsAprobado=false, EsBloqueado=false)
    |
    |-- Artista aprueba --> Aprobado (EsAprobado=true, EsBloqueado=false)
    |                           |
    |                           |-- Artista da de baja --> DadoDeBaja (FechaBaja=now, EsAprobado=false)
    |
    |-- Artista bloquea --> Bloqueado (EsAprobado=false, EsBloqueado=true)
    |
    |-- Artista rechaza --> ELIMINADO (DELETE fisico del registro)
```

| Estado | EsAprobado | EsBloqueado | FechaBaja | Descripcion |
|--------|------------|-------------|-----------|-------------|
| Pendiente | false | false | null | Solicitud enviada, esperando decision del artista |
| Aprobado | true | false | null | Promotor activo en el programa, tiene CodigoReferido |
| Bloqueado | false | true | null | Bloqueado por el artista, no puede re-solicitar |
| DadoDeBaja | false | false | DateTime | Baja por el artista, CodigoReferido desactivado |
| Rechazado | - | - | - | Registro eliminado fisicamente |

**Migracion requerida:** Agregar columnas `EsAprobado BIT NOT NULL DEFAULT 0` y `EsBloqueado BIT NOT NULL DEFAULT 0` a la tabla `PromoProgramaPromotor`.

---

## Endpoints API

### GET /api/crowdpromotion/programas/explorar

**Descripcion:** Lista paginada de programas de promocion activos disponibles para que los promotores soliciten inscripcion. Incluye el estado de inscripcion del promotor autenticado para cada programa (`miEstado`). Solo se devuelven programas con `EsActivo = true`. El `PromotorId` se resuelve desde el token JWT via `UserId`.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `artistaNombre` | string | No | Filtrar por nombre artistico (contains, case-insensitive) |
| `tipoPromoId` | int | No | Filtrar por tipo de programa (ID de MaestraTipoPromo) |
| `page` | int | No | Numero de pagina. Default: 1 |
| `pageSize` | int | No | Elementos por pagina. Default: 10. Max: 50 |

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "titulo": "Promociona mi nuevo album",
        "artistaNombre": "Luna Nova",
        "tipoPromoId": 1,
        "tipoPromoNombre": "Referral",
        "importeComisionPorcentaje": 10.00,
        "importeComisionFija": null,
        "monedaNombre": "EUR",
        "numeroTareas": 3,
        "campaniaTitulo": "Mi Album Debut",
        "fechaInicio": "2026-03-01",
        "fechaFin": "2026-06-01",
        "miEstado": null
      },
      {
        "id": "7d89e123-4567-8901-b234-c567d890e123",
        "titulo": "Difunde mi gira de verano",
        "artistaNombre": "The Waves",
        "tipoPromoId": 2,
        "tipoPromoNombre": "Afiliado",
        "importeComisionPorcentaje": null,
        "importeComisionFija": 5.00,
        "monedaNombre": "EUR",
        "numeroTareas": 5,
        "campaniaTitulo": null,
        "fechaInicio": "2026-04-01",
        "fechaFin": "2026-09-01",
        "miEstado": "Pendiente"
      }
    ],
    "totalCount": 5,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "messages": []
}
```

**Valores del campo `miEstado`:**

| Valor | Condicion |
|-------|-----------|
| `null` | El promotor no tiene inscripcion en este programa |
| `"Pendiente"` | Existe `PromoProgramaPromotor` con `EsAprobado=false`, `EsBloqueado=false`, `FechaBaja=null` |
| `"Aprobado"` | Existe `PromoProgramaPromotor` con `EsAprobado=true`, `FechaBaja=null` |
| `"Bloqueado"` | Existe `PromoProgramaPromotor` con `EsBloqueado=true` |
| `"DadoDeBaja"` | Existe `PromoProgramaPromotor` con `FechaBaja != null` |

**Notas sobre campos calculados:**
- `numeroTareas`: COUNT de `PromoTarea` donde `ProgramaId == id` y `EsActivo == true`. Devuelve `0` si no hay tareas activas.
- `campaniaTitulo`: Resuelto desde `CampaniaCrowdfunding.Titulo`. Devuelve `null` si el programa no tiene campana vinculada.
- `artistaNombre`: Resuelto desde `Artista.NombreArtistico` via join con `PromoPrograma.ArtistaId`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | 5000 | Error inesperado al obtener los programas | Excepcion no controlada |

---

### POST /api/crowdpromotion/programas/{programaId}/inscripcion

**Descripcion:** Solicita la inscripcion del promotor autenticado en el programa indicado. Crea un registro `PromoProgramaPromotor` con `EsAprobado = false` (pendiente de aprobacion del artista). El `PromotorId` se resuelve desde el token JWT via `UserId`. No se acepta en el body.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente y activo

**Path Params:** `programaId` (Guid) - Identificador del programa de promocion

**Request Body:** Ninguno

**Response 201 Created:**
```json
{
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "programaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "programaTitulo": "Promociona mi nuevo album",
    "esAprobado": false,
    "esBloqueado": false,
    "fechaAlta": "2026-03-05T10:00:00Z"
  },
  "messages": [
    { "message": "Solicitud enviada al artista", "errorCode": "0001" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4021 | Ya estas inscrito en este programa | Ya existe `PromoProgramaPromotor` para este `PromotorId` + `ProgramaId` |
| 400 | 4023 | Tu perfil de promotor esta desactivado | `Promotor.EsActivo == false` |
| 400 | 4024 | Este programa no esta activo | `PromoPrograma.EsActivo == false` |
| 403 | 4022 | No puedes inscribirte en este programa | `PromoProgramaPromotor.EsBloqueado == true` para este promotor |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 500 | 5000 | Error inesperado al solicitar la inscripcion | Excepcion no controlada |

**Nota sobre el error 4022 (Bloqueado):** Se retorna HTTP 403 en lugar de 400 porque el promotor tiene prohibicion explicita impuesta por el artista; no es un error de validacion de datos sino de autorizacion de negocio.

---

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/aprobar

**Descripcion:** El artista propietario del programa aprueba la solicitud de inscripcion de un promotor. Al aprobar, el sistema genera automaticamente el `CodigoReferido` y la `UrlTrackingPersonalizada`. El `ArtistaId` del token se verifica contra `PromoPrograma.ArtistaId` para garantizar ownership.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa
- `inscripcionId` (Guid) - Identificador de la inscripcion (`PromoProgramaPromotor.Id`)

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "promotorNombre": "DJ Marketing Pro",
    "esAprobado": true,
    "esBloqueado": false,
    "codigoReferido": "album-2026-x7k9m",
    "urlTrackingPersonalizada": "https://weplay.com/campanias/mi-album?utm_source=weplay&utm_medium=referral&utm_campaign=album-2026&ref=album-2026-x7k9m"
  },
  "messages": [
    { "message": "Promotor aprobado", "errorCode": "0002" }
  ]
}
```

**Logica de generacion de CodigoReferido y UrlTrackingPersonalizada:**
- `CodigoReferido`: `{CodigoTrackingBase}-{ShortId}` donde `ShortId` es un identificador unico corto de 5 caracteres alfanumericos (ej: `x7k9m`). Si `PromoPrograma.CodigoTrackingBase` es null o vacio, usar el `Id` del programa truncado.
- `UrlTrackingPersonalizada`: `{PromoPrograma.UrlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={CodigoTrackingBase}&ref={CodigoReferido}`. Si `UrlLanding` es null, la URL sera null tambien.
- Ambos valores se persisten en `PromoProgramaPromotor.CodigoReferido` y `PromoProgramaPromotor.UrlReferido`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4025 | Esta inscripcion no esta en estado pendiente | `EsAprobado == true` o `EsBloqueado == true` o `FechaBaja != null` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2020 | La inscripcion no existe | No existe `PromoProgramaPromotor` con el `inscripcionId` proporcionado |
| 500 | 5000 | Error inesperado al aprobar la inscripcion | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/rechazar

**Descripcion:** El artista propietario del programa rechaza la solicitud de inscripcion de un promotor. La operacion **elimina fisicamente** el registro `PromoProgramaPromotor` (no soft-delete). El promotor podra re-solicitar en el futuro ya que no queda rastro del rechazo. Solo se pueden rechazar inscripciones en estado pendiente.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa
- `inscripcionId` (Guid) - Identificador de la inscripcion (`PromoProgramaPromotor.Id`)

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "inscripcionId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "promotorNombre": "Spammer123"
  },
  "messages": [
    { "message": "Solicitud rechazada", "errorCode": "0003" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4025 | Esta inscripcion no esta en estado pendiente | `EsAprobado == true` o `EsBloqueado == true` o `FechaBaja != null` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2020 | La inscripcion no existe | No existe `PromoProgramaPromotor` con el `inscripcionId` proporcionado |
| 500 | 5000 | Error inesperado al rechazar la inscripcion | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/bloquear

**Descripcion:** El artista propietario del programa bloquea a un promotor. Establece `EsBloqueado = true` en `PromoProgramaPromotor`. Un promotor bloqueado no puede re-solicitar inscripcion en este programa. Se puede bloquear tanto desde estado pendiente como desde estado aprobado.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa
- `inscripcionId` (Guid) - Identificador de la inscripcion (`PromoProgramaPromotor.Id`)

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "promotorNombre": "Spammer123",
    "esBloqueado": true,
    "esAprobado": false
  },
  "messages": [
    { "message": "Promotor bloqueado", "errorCode": "0002" }
  ]
}
```

**Nota:** Al bloquear a un promotor aprobado, ademas se establece `EsAprobado = false` para desactivar su codigo referido. El `CodigoReferido` y `UrlReferido` no se borran de la BD para mantener historico, pero el promotor ya no puede usar el codigo.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4025 | Este promotor ya esta bloqueado | `EsBloqueado == true` en el momento de la solicitud |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2020 | La inscripcion no existe | No existe `PromoProgramaPromotor` con el `inscripcionId` proporcionado |
| 500 | 5000 | Error inesperado al bloquear al promotor | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja

**Descripcion:** El artista da de baja a un promotor aprobado. Establece `FechaBaja = DateTime.UtcNow` y `EsAprobado = false`. El registro se mantiene en BD para historico pero el codigo referido queda desactivado. Solo se pueden dar de baja inscripciones en estado aprobado.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa
- `inscripcionId` (Guid) - Identificador de la inscripcion (`PromoProgramaPromotor.Id`)

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "promotorNombre": "DJ Marketing Pro",
    "esAprobado": false,
    "fechaBaja": "2026-04-01T12:00:00Z"
  },
  "messages": [
    { "message": "Promotor dado de baja", "errorCode": "0002" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4025 | Esta inscripcion no esta en estado aprobado | `EsAprobado == false` (pendiente, bloqueado o ya dado de baja) |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2020 | La inscripcion no existe | No existe `PromoProgramaPromotor` con el `inscripcionId` proporcionado |
| 500 | 5000 | Error inesperado al dar de baja al promotor | Excepcion no controlada |

---

### GET /api/crowdpromotion/programas/{programaId}/inscripciones

**Descripcion:** El artista propietario obtiene la lista paginada de inscripciones (solicitudes) de su programa, con el detalle del perfil del promotor. Soporta filtrado por estado. El `ArtistaId` del token se verifica contra `PromoPrograma.ArtistaId`.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:** `programaId` (Guid) - Identificador del programa

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `estado` | string | No | Filtrar por estado: `Pendiente`, `Aprobado`, `Bloqueado`, `DadoDeBaja`. Sin valor: devuelve todos |
| `page` | int | No | Numero de pagina. Default: 1 |
| `pageSize` | int | No | Elementos por pagina. Default: 20. Max: 50 |

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
        "promotorId": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
        "promotorNombre": "DJ Marketing Pro",
        "tipoPromotorNombre": "Influencer",
        "promotorEmailContacto": "contacto@djmarketing.com",
        "promotorUrlInstagram": "https://instagram.com/djmarketing",
        "promotorUrlTikTok": "https://tiktok.com/@djmarketing",
        "promotorUrlSitioWeb": "https://djmarketing.com",
        "esAprobado": false,
        "esBloqueado": false,
        "codigoReferido": null,
        "fechaAlta": "2026-03-05T14:00:00Z",
        "fechaBaja": null,
        "estado": "Pendiente"
      },
      {
        "id": "c3d4e5f6-a7b8-9012-cd34-ef56ab789012",
        "promotorId": "d4e5f6a7-b8c9-0123-de45-fa67bc890123",
        "promotorNombre": "MusicBlog.es",
        "tipoPromotorNombre": "Medio / Blog",
        "promotorEmailContacto": "info@musicblog.es",
        "promotorUrlInstagram": null,
        "promotorUrlTikTok": null,
        "promotorUrlSitioWeb": "https://musicblog.es",
        "esAprobado": true,
        "esBloqueado": false,
        "codigoReferido": "album-2026-m3k2n",
        "fechaAlta": "2026-03-06T09:00:00Z",
        "fechaBaja": null,
        "estado": "Aprobado"
      }
    ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "messages": []
}
```

**Calculo del campo `estado` en el backend:**

| Condicion | Valor devuelto |
|-----------|----------------|
| `EsBloqueado == true` | `"Bloqueado"` |
| `FechaBaja != null` | `"DadoDeBaja"` |
| `EsAprobado == true` | `"Aprobado"` |
| Ninguna de las anteriores | `"Pendiente"` |

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para ver las inscripciones de este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 500 | 5000 | Error inesperado al obtener las inscripciones | Excepcion no controlada |

---

### GET /api/crowdpromotion/promotor/mis-programas

**Descripcion:** Lista paginada de inscripciones del promotor autenticado en programas de promocion. Incluye el detalle del programa y el estado de la inscripcion. Para las inscripciones aprobadas, incluye `CodigoReferido` y `UrlTrackingPersonalizada`. El `PromotorId` se resuelve desde el token JWT via `UserId`.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de promotor existente

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `page` | int | No | Numero de pagina. Default: 1 |
| `pageSize` | int | No | Elementos por pagina. Default: 10. Max: 50 |

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
        "programaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "programaTitulo": "Promociona mi nuevo album",
        "artistaNombre": "Luna Nova",
        "tipoPromoNombre": "Referral",
        "importeComisionPorcentaje": 10.00,
        "importeComisionFija": null,
        "monedaNombre": "EUR",
        "esAprobado": true,
        "esBloqueado": false,
        "codigoReferido": "album-2026-x7k9m",
        "urlTrackingPersonalizada": "https://weplay.com/campanias/mi-album?utm_source=weplay&utm_medium=referral&utm_campaign=album-2026&ref=album-2026-x7k9m",
        "fechaAlta": "2026-03-05T10:00:00Z",
        "fechaBaja": null,
        "estado": "Aprobado",
        "tareas": [
          {
            "id": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
            "titulo": "Comparte en Instagram Stories",
            "descripcion": "Sube una story mencionando la campana...",
            "tipoEventoPromoNombre": "Share",
            "importeRecompensa": 5.00,
            "monedaNombre": "EUR",
            "esRepetible": true,
            "maxRepeticiones": 10,
            "orden": 1
          }
        ]
      },
      {
        "id": "c3d4e5f6-a7b8-9012-cd34-ef56ab789012",
        "programaId": "7d89e123-4567-8901-b234-c567d890e123",
        "programaTitulo": "Difunde mi gira de verano",
        "artistaNombre": "The Waves",
        "tipoPromoNombre": "Afiliado",
        "importeComisionPorcentaje": null,
        "importeComisionFija": 5.00,
        "monedaNombre": "EUR",
        "esAprobado": false,
        "esBloqueado": false,
        "codigoReferido": null,
        "urlTrackingPersonalizada": null,
        "fechaAlta": "2026-03-10T08:00:00Z",
        "fechaBaja": null,
        "estado": "Pendiente",
        "tareas": []
      }
    ],
    "totalCount": 3,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "messages": []
}
```

**Notas:**
- `codigoReferido` y `urlTrackingPersonalizada` solo se devuelven con valor cuando `EsAprobado == true` y `FechaBaja == null`. En cualquier otro estado se devuelven `null`.
- `tareas` solo se incluye para inscripciones con `EsAprobado == true`. Para otros estados se devuelve array vacio.
- `tareas` incluye solo tareas activas (`PromoTarea.EsActivo == true`).

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 500 | 5000 | Error inesperado al obtener tus programas | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Actor | Notas |
|----------|------|-------|-------|
| `GET /api/crowdpromotion/programas/explorar` | Bearer JWT | Promotor | PromotorId resuelto por UserId del token |
| `POST /api/crowdpromotion/programas/{programaId}/inscripcion` | Bearer JWT | Promotor activo | Valida EsActivo del promotor |
| `PATCH .../inscripciones/{id}/aprobar` | Bearer JWT | Artista propietario | Valida ArtistaId del token vs PromoPrograma.ArtistaId |
| `PATCH .../inscripciones/{id}/rechazar` | Bearer JWT | Artista propietario | Valida ArtistaId del token vs PromoPrograma.ArtistaId |
| `PATCH .../inscripciones/{id}/bloquear` | Bearer JWT | Artista propietario | Valida ArtistaId del token vs PromoPrograma.ArtistaId |
| `PATCH .../inscripciones/{id}/dar-de-baja` | Bearer JWT | Artista propietario | Valida ArtistaId del token vs PromoPrograma.ArtistaId |
| `GET /api/crowdpromotion/programas/{programaId}/inscripciones` | Bearer JWT | Artista propietario | Valida ArtistaId del token vs PromoPrograma.ArtistaId |
| `GET /api/crowdpromotion/promotor/mis-programas` | Bearer JWT | Promotor | PromotorId resuelto por UserId del token |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "usuario@example.com",
  "role": "Fan",
  "exp": 1740873600,
  "iat": 1740787200
}
```

**Nota:** Ni el `PromotorId` ni el `ArtistaId` se incluyen en el token. El backend resuelve cada entidad consultando por `UserId` (claim `sub`) al inicio de cada handler. Usar `IRequestCacheService` para cachear estas resoluciones dentro del mismo request.

**Verificacion de ownership del artista:** En los endpoints de gestion de inscripciones, el handler debe:
1. Resolver `ArtistaId` desde `UserId` del token.
2. Cargar `PromoPrograma` por `programaId`.
3. Verificar que `PromoPrograma.ArtistaId == artista.Id`. Si no coincide, retornar error `4026` con HTTP 403.

### Proteccion de Rutas Frontend

| Ruta | App | Auth | Redirect | Notas |
|------|-----|------|----------|-------|
| `/crowdpromotion/explorar` | Landing | Bearer JWT | `/auth/login` | Requiere perfil de promotor |
| `/promotor/mis-programas` | Landing | Bearer JWT | `/auth/login` | Requiere perfil de promotor |
| `/promotor/mis-programas/:id` | Landing | Bearer JWT | `/auth/login` | Detalle de inscripcion aprobada |
| `/admin/programas/:id/inscripciones` | Admin | Bearer JWT | `/auth/login` | Requiere perfil de artista |

---

## DTOs / Types

### Backend (C#)

#### DTOs de Response - Promotor

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ProgramaExplorarItemDto.cs
public class ProgramaExplorarItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    /// <summary>Resuelto desde Artista.NombreArtistico.</summary>
    public string ArtistaNombre { get; set; } = null!;
    public int TipoPromoId { get; set; }
    /// <summary>Resuelto desde MaestraTipoPromo.Nombre.</summary>
    public string TipoPromoNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    /// <summary>Resuelto desde MaestraMoneda.Nombre.</summary>
    public string? MonedaNombre { get; set; }
    /// <summary>COUNT de PromoTarea donde EsActivo == true.</summary>
    public int NumeroTareas { get; set; }
    /// <summary>Resuelto desde CampaniaCrowdfunding.Titulo. Null si no hay campana vinculada.</summary>
    public string? CampaniaTitulo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    /// <summary>Estado de inscripcion del promotor autenticado. Null si no inscrito.</summary>
    public string? MiEstado { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionCreadaDto.cs
public class InscripcionCreadaDto
{
    public Guid Id { get; set; }
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    /// <summary>Mapeado desde PromoProgramaPromotor.FechaInscripcion.</summary>
    public DateTime FechaAlta { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MiInscripcionDto.cs
public class MiInscripcionDto
{
    public Guid Id { get; set; }
    public Guid ProgramaId { get; set; }
    public string ProgramaTitulo { get; set; } = null!;
    public string ArtistaNombre { get; set; } = null!;
    public string TipoPromoNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public string? MonedaNombre { get; set; }
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    /// <summary>Solo con valor cuando EsAprobado == true y FechaBaja == null.</summary>
    public string? CodigoReferido { get; set; }
    /// <summary>Solo con valor cuando EsAprobado == true y FechaBaja == null. Mapeado desde UrlReferido.</summary>
    public string? UrlTrackingPersonalizada { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    /// <summary>Calculado: Bloqueado > DadoDeBaja > Aprobado > Pendiente.</summary>
    public string Estado { get; set; } = null!;
    /// <summary>Solo para inscripciones aprobadas. Array vacio en otros estados.</summary>
    public List<TareaResumenDto> Tareas { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareaResumenDto.cs
public class TareaResumenDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string TipoEventoPromoNombre { get; set; } = null!;
    public decimal? ImporteRecompensa { get; set; }
    public string? MonedaNombre { get; set; }
    public bool EsRepetible { get; set; }
    public int? MaxRepeticiones { get; set; }
    public int Orden { get; set; }
}
```

#### DTOs de Response - Artista

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionAprobadaDto.cs
public class InscripcionAprobadaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public string CodigoReferido { get; set; } = null!;
    /// <summary>Puede ser null si PromoPrograma.UrlLanding es null.</summary>
    public string? UrlTrackingPersonalizada { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionRechazadaDto.cs
public class InscripcionRechazadaDto
{
    public Guid InscripcionId { get; set; }
    public string PromotorNombre { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionBloqueadaDto.cs
public class InscripcionBloqueadaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsBloqueado { get; set; }
    public bool EsAprobado { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionDadaDeBajaDto.cs
public class InscripcionDadaDeBajaDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public DateTime FechaBaja { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/InscripcionListItemDto.cs
/// <summary>Item de la lista de inscripciones vista por el artista.</summary>
public class InscripcionListItemDto
{
    public Guid Id { get; set; }
    public Guid PromotorId { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public string? PromotorEmailContacto { get; set; }
    public string? PromotorUrlInstagram { get; set; }
    public string? PromotorUrlTikTok { get; set; }
    public string? PromotorUrlSitioWeb { get; set; }
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    /// <summary>Solo con valor cuando EsAprobado == true.</summary>
    public string? CodigoReferido { get; set; }
    /// <summary>Mapeado desde PromoProgramaPromotor.FechaInscripcion.</summary>
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    /// <summary>Calculado: Bloqueado > DadoDeBaja > Aprobado > Pendiente.</summary>
    public string Estado { get; set; } = null!;
}
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdpromotion.ts
// Agregar las siguientes interfaces al archivo existente

// ---- Inscripcion - Estado ----

export type InscripcionEstado = 'Pendiente' | 'Aprobado' | 'Bloqueado' | 'DadoDeBaja';

// ---- Explorar Programas (Promotor) ----

export interface ProgramaExplorarItem {
  id: string;                        // Guid as string
  titulo: string;
  artistaNombre: string;
  tipoPromoId: number;
  tipoPromoNombre: string;
  importeComisionPorcentaje: number | null;
  importeComisionFija: number | null;
  monedaNombre: string | null;
  numeroTareas: number;
  campaniaTitulo: string | null;
  fechaInicio: string | null;        // ISO date string
  fechaFin: string | null;           // ISO date string
  miEstado: InscripcionEstado | null; // null = no inscrito
}

export interface ProgramasExplorarResponse {
  items: ProgramaExplorarItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ---- Solicitar Inscripcion (Promotor) ----

export interface InscripcionCreada {
  id: string;
  programaId: string;
  programaTitulo: string;
  esAprobado: boolean;
  esBloqueado: boolean;
  fechaAlta: string;                 // ISO datetime string
}

// ---- Mis Programas (Promotor) ----

export interface TareaResumen {
  id: string;
  titulo: string;
  descripcion: string | null;
  tipoEventoPromoNombre: string;
  importeRecompensa: number | null;
  monedaNombre: string | null;
  esRepetible: boolean;
  maxRepeticiones: number | null;
  orden: number;
}

export interface MiInscripcion {
  id: string;
  programaId: string;
  programaTitulo: string;
  artistaNombre: string;
  tipoPromoNombre: string;
  importeComisionPorcentaje: number | null;
  importeComisionFija: number | null;
  monedaNombre: string | null;
  esAprobado: boolean;
  esBloqueado: boolean;
  codigoReferido: string | null;         // Solo cuando esAprobado && !fechaBaja
  urlTrackingPersonalizada: string | null; // Solo cuando esAprobado && !fechaBaja
  fechaAlta: string;                     // ISO datetime string
  fechaBaja: string | null;              // ISO datetime string
  estado: InscripcionEstado;
  tareas: TareaResumen[];                // Solo cuando esAprobado; [] en otros estados
}

export interface MisProgramasResponse {
  items: MiInscripcion[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ---- Gestion de Inscripciones (Artista) ----

export interface InscripcionAprobada {
  id: string;
  promotorNombre: string;
  esAprobado: boolean;
  esBloqueado: boolean;
  codigoReferido: string;
  urlTrackingPersonalizada: string | null;
}

export interface InscripcionRechazada {
  inscripcionId: string;
  promotorNombre: string;
}

export interface InscripcionBloqueada {
  id: string;
  promotorNombre: string;
  esBloqueado: boolean;
  esAprobado: boolean;
}

export interface InscripcionDadaDeBaja {
  id: string;
  promotorNombre: string;
  esAprobado: boolean;
  fechaBaja: string;                     // ISO datetime string
}

export interface InscripcionListItem {
  id: string;
  promotorId: string;
  promotorNombre: string;
  tipoPromotorNombre: string;
  promotorEmailContacto: string | null;
  promotorUrlInstagram: string | null;
  promotorUrlTikTok: string | null;
  promotorUrlSitioWeb: string | null;
  esAprobado: boolean;
  esBloqueado: boolean;
  codigoReferido: string | null;
  fechaAlta: string;                     // ISO datetime string
  fechaBaja: string | null;
  estado: InscripcionEstado;
}

export interface InscripcionesListResponse {
  items: InscripcionListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ---- Filtros ----

export interface ExplorarProgramasFilters {
  artistaNombre?: string;
  tipoPromoId?: number;
  page?: number;
  pageSize?: number;
}

export interface InscripcionesFilters {
  estado?: InscripcionEstado;
  page?: number;
  pageSize?: number;
}
```

---

## Validaciones Compartidas

Los endpoints de gestion de inscripciones (aprobar, rechazar, bloquear, dar-de-baja) no tienen body de request; todas las validaciones son de negocio y se realizan en el Service.

El endpoint `POST .../inscripcion` tampoco tiene body; el `programaId` viene del path.

| Campo | Regla | Backend (FluentValidation / Service) | Frontend (Zod / Guard) |
|-------|-------|--------------------------------------|------------------------|
| `programaId` (path) | formato Guid valido | Validado por model binding de ASP.NET Core | Validado por React Router (ruta tipada) |
| `inscripcionId` (path) | formato Guid valido | Validado por model binding de ASP.NET Core | Validado por React Router (ruta tipada) |
| `Promotor.EsActivo` | true al solicitar inscripcion | Service valida antes de crear | Guard de ruta verifica estado del promotor |
| `PromoPrograma.EsActivo` | true al solicitar inscripcion | Service valida antes de crear | Boton "Solicitar" deshabilitado si `miEstado != null` |
| Unicidad promotor+programa | un solo registro por par | Constraint unico en BD + validacion en Service | Boton "Solicitar" oculto si `miEstado != null` |
| `EsBloqueado` del promotor | false al solicitar inscripcion | Service valida, retorna 4022 | `miEstado == 'Bloqueado'` muestra mensaje, no boton |
| `ExplorarProgramasFilters.page` | >= 1 cuando presente | Validado en Query handler | `.min(1).optional()` en Zod |
| `ExplorarProgramasFilters.pageSize` | entre 1 y 50 cuando presente | Validado en Query handler | `.min(1).max(50).optional()` en Zod |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdpromotion.schema.ts
// Agregar al archivo existente:
import { z } from 'zod';

export const explorarProgramasFiltersSchema = z.object({
  artistaNombre: z
    .string()
    .max(200, 'Maximo 200 caracteres')
    .optional(),

  tipoPromoId: z
    .number()
    .int()
    .positive('El tipo de programa debe ser un numero positivo')
    .optional(),

  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor a 0')
    .optional(),

  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor a 0')
    .max(50, 'El tamano de pagina no puede superar 50')
    .optional(),
});

export const inscripcionesFiltersSchema = z.object({
  estado: z
    .enum(['Pendiente', 'Aprobado', 'Bloqueado', 'DadoDeBaja'])
    .optional(),

  page: z
    .number()
    .int()
    .min(1, 'La pagina debe ser mayor a 0')
    .optional(),

  pageSize: z
    .number()
    .int()
    .min(1, 'El tamano de pagina debe ser mayor a 0')
    .max(50, 'El tamano de pagina no puede superar 50')
    .optional(),
});

export type ExplorarProgramasFiltersData = z.infer<typeof explorarProgramasFiltersSchema>;
export type InscripcionesFiltersData = z.infer<typeof inscripcionesFiltersSchema>;
```

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo existente `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// Agregar las siguientes constantes a la clase ServiceResponseMessageType existente:

// NotFound (continua desde 2019)
public const string NotFound_Inscripcion = "2020";        // NUEVO - No existe PromoProgramaPromotor con el Id

// Business Rules (continua desde 4020)
public const string BusinessRule_AlreadyInscribed      = "4021";  // NUEVO - Promotor ya inscrito en este programa
public const string BusinessRule_PromotorBlocked       = "4022";  // NUEVO - Promotor bloqueado en este programa
public const string BusinessRule_PromotorInactive      = "4023";  // NUEVO - Promotor tiene EsActivo == false
public const string BusinessRule_ProgramaInactive      = "4024";  // NUEVO - Programa tiene EsActivo == false
public const string BusinessRule_InscripcionNotPending = "4025";  // NUEVO - Inscripcion no esta en el estado esperado
public const string BusinessRule_NotProgramOwner       = "4026";  // NUEVO - ArtistaId no coincide con PromoPrograma.ArtistaId
```

**Tabla de nuevas constantes:**

| Constante | Codigo | HTTP | Descripcion |
|-----------|--------|------|-------------|
| `NotFound_Inscripcion` | `2020` | 404 | No existe `PromoProgramaPromotor` con el `inscripcionId` |
| `BusinessRule_AlreadyInscribed` | `4021` | 400 | Ya existe inscripcion del promotor en este programa |
| `BusinessRule_PromotorBlocked` | `4022` | 403 | El promotor tiene `EsBloqueado == true` en este programa |
| `BusinessRule_PromotorInactive` | `4023` | 400 | El promotor tiene `EsActivo == false` |
| `BusinessRule_ProgramaInactive` | `4024` | 400 | El programa tiene `EsActivo == false` |
| `BusinessRule_InscripcionNotPending` | `4025` | 400 | La inscripcion no esta en el estado requerido para la operacion |
| `BusinessRule_NotProgramOwner` | `4026` | 403 | El artista autenticado no es propietario del programa |

**Justificacion de numeracion:** Los codigos `2020` y `4021`-`4026` continuan la secuencia del modulo Crowdpromotion. El ultimo codigo de NotFound era `2019` (NotFound_PromoPrograma) y el ultimo de BusinessRule era `4020` (BusinessRule_PromoProgramaAlreadyInactive).

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Agregar a QUERY_KEYS.crowdpromotion:
export const QUERY_KEYS = {
  // ... (entradas existentes) ...
  crowdpromotion: {
    // ... (entradas existentes: promotor, maestras) ...
    programas: {
      explorar: (filters?: ExplorarProgramasFilters) =>
        ['crowdpromotion', 'programas', 'explorar', filters] as const,
      inscripciones: (programaId: string, filters?: InscripcionesFilters) =>
        ['crowdpromotion', 'programas', programaId, 'inscripciones', filters] as const,
    },
    inscripciones: {
      misProgramas: (page?: number) =>
        ['crowdpromotion', 'inscripciones', 'mis-programas', page] as const,
    },
  },
} as const;

// Agregar a API_ROUTES.crowdpromotion:
export const API_ROUTES = {
  // ... (entradas existentes) ...
  crowdpromotion: {
    // ... (entradas existentes: promotor, maestras) ...
    programas: {
      // ... (entradas existentes de US-CP-02) ...
      explorar: '/api/crowdpromotion/programas/explorar',
      inscripcion: (programaId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripcion`,
      inscripciones: (programaId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripciones`,
      aprobar: (programaId: string, inscripcionId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/aprobar`,
      rechazar: (programaId: string, inscripcionId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/rechazar`,
      bloquear: (programaId: string, inscripcionId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/bloquear`,
      darDeBaja: (programaId: string, inscripcionId: string) =>
        `/api/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/dar-de-baja`,
    },
    promotorInscripciones: {
      misProgramas: '/api/crowdpromotion/promotor/mis-programas',
    },
  },
} as const;

// Agregar a APP_ROUTES.landing:
export const APP_ROUTES = {
  // ... (entradas existentes) ...
  landing: {
    // ... (entradas existentes: promotor) ...
    crowdpromotion: {
      explorar: '/crowdpromotion/explorar',
      misProgramas: '/promotor/mis-programas',
      inscripcionDetalle: (inscripcionId: string) =>
        `/promotor/mis-programas/${inscripcionId}`,
    },
  },
  admin: {
    // ... (entradas existentes) ...
    programaInscripciones: (programaId: string) =>
      `/admin/programas/${programaId}/inscripciones`,
  },
} as const;
```

### Constantes de dominio - Estado de Inscripcion

```typescript
// Agregar en src/shared/constants/index.ts

export const INSCRIPCION_ESTADO = {
  PENDIENTE:    'Pendiente',
  APROBADO:     'Aprobado',
  BLOQUEADO:    'Bloqueado',
  DADO_DE_BAJA: 'DadoDeBaja',
} as const;

export const INSCRIPCION_ESTADO_LABELS: Record<string, string> = {
  Pendiente:    'Pendiente de aprobacion',
  Aprobado:     'Aprobado',
  Bloqueado:    'Bloqueado',
  DadoDeBaja:   'Dado de baja',
};

export const INSCRIPCION_ESTADO_BADGE_VARIANT: Record<string, string> = {
  Pendiente:    'secondary',   // Badge gris
  Aprobado:     'default',     // Badge verde/primario
  Bloqueado:    'destructive', // Badge rojo
  DadoDeBaja:   'outline',     // Badge con borde
};
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... (mensajes existentes) ...

  // Crowdpromotion - Inscripcion en Programa (US-CP-03)
  '2020': 'La inscripcion no existe.',
  '4021': 'Ya estas inscrito en este programa.',
  '4022': 'No puedes inscribirte en este programa.',
  '4023': 'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.',
  '4024': 'Este programa no esta activo en este momento.',
  '4025': 'La inscripcion no se puede modificar en su estado actual.',
  '4026': 'No tienes permiso para gestionar las inscripciones de este programa.',

  // Keys semanticos para uso interno en hooks
  INSCRIPCION_NOT_FOUND:           'La inscripcion no existe.',
  INSCRIPCION_ALREADY_EXISTS:      'Ya estas inscrito en este programa.',
  INSCRIPCION_PROMOTOR_BLOCKED:    'No puedes inscribirte en este programa.',
  INSCRIPCION_PROMOTOR_INACTIVE:   'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.',
  INSCRIPCION_PROGRAMA_INACTIVE:   'Este programa no esta activo en este momento.',
  INSCRIPCION_ESTADO_INVALIDO:     'La inscripcion no se puede modificar en su estado actual.',
  INSCRIPCION_NOT_OWNER:           'No tienes permiso para gestionar las inscripciones de este programa.',
};
```

---

## Notas de Implementacion

### Backend

1. **Migracion obligatoria antes de implementar:** Agregar `EsAprobado BIT NOT NULL DEFAULT 0` y `EsBloqueado BIT NOT NULL DEFAULT 0` a la tabla `PromoProgramaPromotor`. Tambien agregar el constraint unico `IX_PromoProgramaPromotor_ProgramaId_PromotorId` sobre (`ProgramaId`, `PromotorId`) para garantizar una sola inscripcion por par.

2. **SolicitarInscripcionCommand:**
   - Obtener `UserId` del token (claim `sub`).
   - Resolver `Promotor` por `UserId`; si no existe, retornar `NotFound_Promotor` (2015).
   - Verificar `Promotor.EsActivo == true`; si no, retornar `BusinessRule_PromotorInactive` (4023).
   - Cargar `PromoPrograma` por `programaId`; si no existe, retornar `NotFound_PromoPrograma` (2019).
   - Verificar `PromoPrograma.EsActivo == true`; si no, retornar `BusinessRule_ProgramaInactive` (4024).
   - Verificar que no exista `PromoProgramaPromotor` para el par (`ProgramaId`, `PromotorId`). Si existe con `EsBloqueado == true`, retornar `BusinessRule_PromotorBlocked` (4022). Si existe con cualquier otro estado, retornar `BusinessRule_AlreadyInscribed` (4021).
   - Crear `PromoProgramaPromotor` con `EsAprobado = false`, `EsBloqueado = false`, `EsActivo = true`, `FechaInscripcion = DateTime.UtcNow`.
   - Retornar `Created` (0001).

3. **AprobarInscripcionCommand:**
   - Resolver `Artista` por `UserId` del token; si no existe, retornar `NotFound_Artista` (2016).
   - Cargar `PromoPrograma` por `programaId`; si no existe, retornar `NotFound_PromoPrograma` (2019).
   - Verificar `PromoPrograma.ArtistaId == artista.Id`; si no, retornar `BusinessRule_NotProgramOwner` (4026) con HTTP 403.
   - Cargar `PromoProgramaPromotor` por `inscripcionId`; si no existe, retornar `NotFound_Inscripcion` (2020).
   - Verificar estado pendiente: `EsAprobado == false && EsBloqueado == false && FechaBaja == null`; si no, retornar `BusinessRule_InscripcionNotPending` (4025).
   - Generar `CodigoReferido`: `{PromoPrograma.CodigoTrackingBase}-{ShortId}` donde `ShortId` son 5 chars alfanumericos aleatorios. Usar `Nanoid` o `Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 5).ToLower()`.
   - Generar `UrlTrackingPersonalizada` si `PromoPrograma.UrlLanding != null`.
   - Establecer `EsAprobado = true`, persistir `CodigoReferido` y `UrlReferido`.
   - Retornar `Updated` (0002).

4. **RechazarInscripcionCommand:**
   - Misma validacion de ownership que Aprobar.
   - Verificar estado pendiente: `EsAprobado == false && EsBloqueado == false && FechaBaja == null`.
   - Eliminar fisicamente el registro con `_repository.DeleteAsync(inscripcionId, ct)`.
   - Retornar `Deleted` (0003).

5. **BloquearInscripcionCommand:**
   - Misma validacion de ownership que Aprobar.
   - Verificar que `EsBloqueado == false`; si ya esta bloqueado, retornar `BusinessRule_InscripcionNotPending` (4025).
   - Establecer `EsBloqueado = true`. Si `EsAprobado == true`, tambien establecer `EsAprobado = false`.
   - Retornar `Updated` (0002).

6. **DarDeBajaInscripcionCommand:**
   - Misma validacion de ownership que Aprobar.
   - Verificar estado aprobado: `EsAprobado == true && FechaBaja == null`; si no, retornar `BusinessRule_InscripcionNotPending` (4025).
   - Establecer `FechaBaja = DateTime.UtcNow`, `EsAprobado = false`.
   - Retornar `Updated` (0002).

7. **GetProgramasExplorarQuery:**
   - Resolver `Promotor` por `UserId`; si no existe, retornar `NotFound_Promotor` (2015).
   - Consulta con `AsNoTracking()` sobre `PromoPrograma` donde `EsActivo == true`.
   - Left join con `PromoProgramaPromotor` del promotor actual para obtener `MiEstado`.
   - Aplicar filtros de `artistaNombre` (contains) y `tipoPromoId` si estan presentes.
   - Calcular `NumeroTareas` con COUNT de `PromoTarea` activas por programa.
   - Resolver `ArtistaNombre` y `CampaniaTitulo` via joins/includes.
   - Paginar resultado.

8. **GetInscripcionesQuery (vista artista):**
   - Resolver `Artista` por `UserId`; si no existe, retornar `NotFound_Artista` (2016).
   - Cargar `PromoPrograma` por `programaId`; verificar ownership.
   - Consulta paginada sobre `PromoProgramaPromotor` con joins a `Promotor` y `MaestraTipoPromotor`.
   - Aplicar filtro `estado` en memoria o en SQL segun implementacion.

9. **GetMisProgramasQuery:**
   - Resolver `Promotor` por `UserId`; si no existe, retornar `NotFound_Promotor` (2015).
   - Consulta paginada sobre `PromoProgramaPromotor` donde `PromotorId == promotor.Id`.
   - Join con `PromoPrograma`, `Artista`, `MaestraTipoPromo`, `MaestraMoneda`.
   - Para inscripciones aprobadas, incluir tareas activas del programa (`PromoTarea.EsActivo == true`).
   - Calcular campo `Estado` en el mapper o en el handler antes de retornar.

10. **Caching:** Usar `IRequestCacheService` para cachear la resolucion de `Promotor` y `Artista` por `UserId` (pueden ser consultados en Validator y Handler del mismo request).

### Frontend

1. **Nuevo hook `useExplorarProgramas(filters)`:** Query `GET /api/crowdpromotion/programas/explorar` con `QUERY_KEYS.crowdpromotion.programas.explorar(filters)`. Configurar `staleTime: 60_000` (1 minuto) ya que el listado no cambia frecuentemente. Pasar filtros como query params.

2. **Nuevo hook `useSolicitarInscripcion()`:** Mutation `POST .../inscripcion`. En `onSuccess`, invalidar `QUERY_KEYS.crowdpromotion.programas.explorar()` (para actualizar `miEstado`) y `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas()`. Mostrar toast de exito.

3. **Nuevos hooks de gestion (artista):**
   - `useInscripciones(programaId, filters)`: Query `GET .../inscripciones`.
   - `useAprobarInscripcion()`: Mutation PATCH; en `onSuccess` invalidar `useInscripciones`.
   - `useRechazarInscripcion()`: Mutation PATCH; en `onSuccess` invalidar `useInscripciones`.
   - `useBloquearInscripcion()`: Mutation PATCH; en `onSuccess` invalidar `useInscripciones`.
   - `useDarDeBajaInscripcion()`: Mutation PATCH; en `onSuccess` invalidar `useInscripciones`.

4. **Nuevo hook `useMisProgramas(page?)`:** Query `GET /api/crowdpromotion/promotor/mis-programas`. Con `QUERY_KEYS.crowdpromotion.inscripciones.misProgramas(page)`.

5. **Logica del boton "Solicitar inscripcion":** Verificar `miEstado` del item. Si `miEstado != null`, no mostrar el boton sino un badge con el estado. Si `miEstado == null`, mostrar boton activo.

6. **Visibilidad de CodigoReferido y URL:** Mostrar `codigoReferido` y `urlTrackingPersonalizada` solo cuando `estado == 'Aprobado'`. Para otros estados mostrar mensaje descriptivo.

7. **Confirmacion antes de acciones destructivas:** Los actions de Rechazar, Bloquear y Dar de Baja deben mostrar un dialogo de confirmacion (`AlertDialog` de shadcn/ui) antes de ejecutar la mutation.

8. **Manejo del error 4022 (Bloqueado):** Al recibir este codigo, mostrar el mensaje `ERROR_MESSAGES['4022']` en un `Alert` de tipo destructive, no como toast.

---

## Checklist de Contratos

- [x] Cambios al modelo de dominio documentados (EsAprobado, EsBloqueado en PromoProgramaPromotor)
- [x] Maquina de estados de PromoProgramaPromotor documentada
- [x] Migracion requerida documentada (columnas + constraint unico)
- [x] Endpoints con request/response/errores (8 endpoints documentados)
- [x] Autorizacion por endpoint (tabla resumen)
- [x] Claims JWT documentados
- [x] Verificacion de ownership del artista documentada
- [x] Logica de generacion de CodigoReferido y UrlTrackingPersonalizada documentada
- [x] Comportamiento de DELETE fisico en Rechazar documentado
- [x] DTOs C# completos (ProgramaExplorarItemDto, InscripcionCreadaDto, MiInscripcionDto, TareaResumenDto, InscripcionAprobadaDto, InscripcionRechazadaDto, InscripcionBloqueadaDto, InscripcionDadaDeBajaDto, InscripcionListItemDto)
- [x] Types TypeScript equivalentes (ProgramaExplorarItem, InscripcionCreada, MiInscripcion, TareaResumen, InscripcionAprobada, InscripcionRechazada, InscripcionBloqueada, InscripcionDadaDeBaja, InscripcionListItem, ExplorarProgramasFilters, InscripcionesFilters)
- [x] Tipo union InscripcionEstado documentado
- [x] Schemas Zod para filtros (explorarProgramasFiltersSchema, inscripcionesFiltersSchema)
- [x] Nuevas constantes ServiceResponseMessageType (2020, 4021-4026)
- [x] Constantes compartidas (QUERY_KEYS, API_ROUTES, APP_ROUTES, INSCRIPCION_ESTADO)
- [x] Mapeo de errores a mensajes UI (codigos 2020, 4021-4026)
- [x] Validaciones compartidas (tabla con reglas de negocio Backend vs Frontend)
- [x] Notas de implementacion Backend (9 puntos detallados)
- [x] Notas de implementacion Frontend (8 puntos detallados)
- [x] Calculo del campo Estado documentado en ambas perspectivas (artista y promotor)
- [x] Comportamiento de campos null (codigoReferido, urlTrackingPersonalizada, tareas) documentado
