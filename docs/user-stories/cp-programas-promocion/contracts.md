# Contratos: Programas de Promocion

> **Feature:** cp-programas-promocion (US-CP-02)
> **Ultima actualizacion:** 2026-02-25
> **Modulo Backend:** Crowdpromotion
> **Depende de:** US-CP-01 (cp-perfil-promotor) - El modulo Crowdpromotion ya existe con entidades PromoPrograma y PromoTarea

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### POST /api/crowdpromotion/programas

**Descripcion:** Crea un programa de promocion con sus tareas asociadas. La operacion es transaccional: programa y tareas se crean en una sola unidad de trabajo. El `ArtistaId` se resuelve desde el token JWT consultando el perfil de Artista por `UserId`; no se acepta en el body.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de Artista existente

**Request Body:**
```json
{
  "titulo": "Promociona mi nuevo album",
  "descripcion": "Ayudanos a difundir nuestro nuevo album...",
  "tipoPromoId": 1,
  "campaniaCrowdfundingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "proyectoArtisticoId": null,
  "urlLanding": "https://weplay.com/campanias/mi-album",
  "codigoTrackingBase": "album-2026",
  "monedaId": 1,
  "importeComisionPorcentaje": 10.00,
  "importeComisionFija": null,
  "fechaInicio": "2026-03-01",
  "fechaFin": "2026-06-01",
  "tareas": [
    {
      "titulo": "Comparte en Instagram Stories",
      "descripcion": "Sube una story mencionando la campana...",
      "tipoEventoPromoId": 3,
      "tipoRewardId": 1,
      "importeRecompensa": 5.00,
      "monedaId": 1,
      "puntosRecompensa": null,
      "urlInstrucciones": "https://docs.example.com/instrucciones-ig",
      "esRepetible": true,
      "maxRepeticiones": 10,
      "fechaInicio": "2026-03-01",
      "fechaFin": "2026-06-01"
    }
  ]
}
```

**Nota sobre campos opcionales:** `descripcion`, `campaniaCrowdfundingId`, `proyectoArtisticoId`, `urlLanding`, `codigoTrackingBase`, `importeComisionPorcentaje`, `importeComisionFija`, `fechaInicio`, `fechaFin` y el array `tareas` son opcionales. El array `tareas` puede estar vacio o no enviarse (FA-02: programa sin tareas es valido). En cada tarea, `descripcion`, `importeRecompensa`, `monedaId` de la tarea, `puntosRecompensa`, `urlInstrucciones`, `maxRepeticiones`, `fechaInicio` y `fechaFin` son opcionales.

**Nota sobre campo `titulo` en tarea:** La entidad `PromoTarea` utiliza el campo `Titulo` (no `Nombre`). El campo se denomina `titulo` en el contrato para mantener consistencia con la entidad existente. La US menciona "Nombre" para el campo de la tarea; se mapea a `Titulo`.

**Response 201 Created:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "titulo": "Promociona mi nuevo album",
    "tipoPromoNombre": "Referral",
    "esActivo": true,
    "tareasCreadas": 1,
    "fechaCreacion": "2026-02-25T10:00:00Z"
  },
  "messages": [
    { "message": "Programa de promocion creado", "errorCode": "0001" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | `titulo` vacio o no enviado |
| 400 | 1011 | El titulo debe tener al menos 5 caracteres | `titulo` con menos de 5 caracteres |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | `titulo` > 200 chars |
| 400 | 1001 | El tipo de programa es obligatorio | `tipoPromoId` no enviado o 0 |
| 400 | 1010 | El tipo de programa no existe | `tipoPromoId` no existe en `Maestra_TipoPromo` |
| 400 | 1001 | La moneda es obligatoria | `monedaId` no enviado o 0 |
| 400 | 1010 | La moneda no existe | `monedaId` no existe en maestras |
| 400 | 1020 | Debe definir al menos una comision (porcentaje o fija) | `importeComisionPorcentaje` y `importeComisionFija` son ambos nulos |
| 400 | 1021 | La comision porcentaje debe estar entre 0 y 100 | `importeComisionPorcentaje` < 0 o > 100 |
| 400 | 1022 | La comision fija no puede ser negativa | `importeComisionFija` < 0 |
| 400 | 1023 | La fecha fin debe ser posterior a la fecha inicio | `fechaFin` <= `fechaInicio` cuando ambas estan presentes |
| 400 | 1024 | El codigo de tracking ya existe para este artista | `codigoTrackingBase` duplicado para el mismo `ArtistaId` |
| 400 | 1025 | El codigo de tracking solo puede contener letras, numeros y guiones | `codigoTrackingBase` con caracteres no permitidos |
| 400 | 1013 | La URL de landing no tiene formato valido | `urlLanding` con formato URL invalido |
| 400 | 1002 | La URL de landing no puede superar los 500 caracteres | `urlLanding` > 500 chars |
| 400 | 1001 | El titulo de la tarea es obligatorio | `tareas[n].titulo` vacio |
| 400 | 1011 | El titulo de la tarea debe tener al menos 3 caracteres | `tareas[n].titulo` con menos de 3 caracteres |
| 400 | 1002 | El titulo de la tarea no puede superar los 200 caracteres | `tareas[n].titulo` > 200 chars |
| 400 | 1001 | El tipo de evento es obligatorio | `tareas[n].tipoEventoPromoId` no enviado o 0 |
| 400 | 1010 | El tipo de evento no existe | `tareas[n].tipoEventoPromoId` no existe en `Maestra_TipoEventoPromo` |
| 400 | 1001 | El tipo de recompensa es obligatorio | `tareas[n].tipoRewardId` no enviado o 0 |
| 400 | 1010 | El tipo de recompensa no existe | `tareas[n].tipoRewardId` no existe en `Maestra_TipoReward` |
| 400 | 1026 | Las tareas repetibles requieren max repeticiones >= 1 | `tareas[n].esRepetible == true` y `maxRepeticiones` nulo o < 1 |
| 400 | 1027 | El importe de recompensa es requerido para recompensas monetarias | `tipoRewardId` es Dinero o Mixto y `importeRecompensa` es nulo |
| 400 | 1028 | Los puntos de recompensa son requeridos para recompensas de puntos | `tipoRewardId` es Puntos o Mixto y `puntosRecompensa` es nulo |
| 400 | 1013 | La URL de instrucciones de la tarea no tiene formato valido | `tareas[n].urlInstrucciones` con formato URL invalido |
| 400 | 1002 | La URL de instrucciones no puede superar los 500 caracteres | `tareas[n].urlInstrucciones` > 500 chars |
| 403 | 3002 | No tienes permiso para vincular esta campana | `campaniaCrowdfundingId` no pertenece al artista autenticado |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2017 | La campana de crowdfunding no existe | `campaniaCrowdfundingId` no existe en BD |
| 404 | 2018 | El proyecto artistico no existe | `proyectoArtisticoId` no existe en BD |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 500 | 5000 | Error inesperado al crear el programa de promocion | Excepcion no controlada |

---

### GET /api/crowdpromotion/programas/mis-programas

**Descripcion:** Devuelve la lista paginada de programas de promocion del artista autenticado. El `ArtistaId` se resuelve desde el token JWT. Los contadores `numeroPromotores` y `numeroTareas` son calculados en la consulta.

**Autorizacion:** Bearer JWT - Usuario autenticado con perfil de Artista existente

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `esActivo` | boolean | No | Filtrar por estado activo/inactivo. Sin valor: devuelve todos |
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
        "tipoPromoId": 1,
        "tipoPromoNombre": "Referral",
        "campaniaTitulo": "Mi Album Debut",
        "esActivo": true,
        "importeComisionPorcentaje": 10.00,
        "importeComisionFija": null,
        "monedaNombre": "EUR",
        "numeroPromotores": 5,
        "numeroTareas": 3,
        "fechaInicio": "2026-03-01",
        "fechaFin": "2026-06-01",
        "fechaCreacion": "2026-02-25T10:00:00Z"
      }
    ],
    "totalCount": 2,
    "page": 1,
    "pageSize": 10
  },
  "messages": []
}
```

**Notas sobre campos calculados:**
- `numeroPromotores`: COUNT de `PromoProgramaPromotor` donde `ProgramaId == id` y `EsAprobado == true`. Devuelve `0` si no hay promotores aprobados.
- `numeroTareas`: COUNT de `PromoTarea` donde `ProgramaId == id` y `EsActivo == true`. Devuelve `0` si no hay tareas activas.
- `campaniaTitulo`: Resuelto desde `CampaniaCrowdfunding.Titulo`. Devuelve `null` si el programa no tiene campana vinculada.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 500 | 5000 | Error inesperado al obtener los programas | Excepcion no controlada |

---

### GET /api/crowdpromotion/programas/{id}

**Descripcion:** Devuelve el detalle completo de un programa de promocion incluyendo sus tareas, los promotores inscritos y un resumen de actividad. Solo el artista propietario puede acceder a este endpoint. El `ArtistaId` del token se compara con `PromoPrograma.ArtistaId`.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:** `id` (Guid) - Identificador del programa

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "titulo": "Promociona mi nuevo album",
    "descripcion": "Ayudanos a difundir...",
    "tipoPromoId": 1,
    "tipoPromoNombre": "Referral",
    "campaniaCrowdfundingId": "7d89e123-4567-8901-b234-c567d890e123",
    "campaniaTitulo": "Mi Album Debut",
    "proyectoArtisticoId": null,
    "urlLanding": "https://weplay.com/campanias/mi-album",
    "codigoTrackingBase": "album-2026",
    "monedaId": 1,
    "monedaNombre": "EUR",
    "importeComisionPorcentaje": 10.00,
    "importeComisionFija": null,
    "esActivo": true,
    "fechaInicio": "2026-03-01",
    "fechaFin": "2026-06-01",
    "fechaCreacion": "2026-02-25T10:00:00Z",
    "fechaActualizacion": null,
    "tareas": [
      {
        "id": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
        "titulo": "Comparte en Instagram Stories",
        "descripcion": "Sube una story mencionando la campana...",
        "tipoEventoPromoId": 3,
        "tipoEventoPromoNombre": "Share",
        "tipoRewardId": 1,
        "tipoRewardNombre": "Dinero",
        "importeRecompensa": 5.00,
        "monedaId": 1,
        "monedaNombre": "EUR",
        "puntosRecompensa": null,
        "urlInstrucciones": "https://docs.example.com/instrucciones-ig",
        "esRepetible": true,
        "maxRepeticiones": 10,
        "orden": 1,
        "esActivo": true,
        "fechaInicio": "2026-03-01",
        "fechaFin": "2026-06-01",
        "completadosPorPromotores": 23
      }
    ],
    "promotores": [
      {
        "id": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
        "promotorNombre": "DJ Marketing Pro",
        "tipoPromotorNombre": "Influencer",
        "esAprobado": true,
        "esBloqueado": false,
        "fechaAlta": "2026-03-05T14:00:00Z"
      }
    ],
    "resumen": {
      "totalPromotoresAprobados": 5,
      "totalPromotoresPendientes": 2,
      "totalEventos": 150,
      "totalConversiones": 12,
      "valorTotalGenerado": 1200.00
    }
  },
  "messages": []
}
```

**Notas sobre campos calculados en `resumen`:**
- `totalPromotoresAprobados`: COUNT de `PromoProgramaPromotor` donde `EsAprobado == true`.
- `totalPromotoresPendientes`: COUNT de `PromoProgramaPromotor` donde `EsAprobado == false` y `EsBloqueado == false`.
- `totalEventos`: COUNT de `PromoEventoPromotor` asociados al programa (todos los tipos de evento).
- `totalConversiones`: COUNT de `PromoEventoPromotor` donde `TipoEventoPromoId == 6` (Backing).
- `valorTotalGenerado`: SUM del valor generado por conversiones (calculado desde backing amounts referidos).
- `completadosPorPromotores` en cada tarea: COUNT de `PromoTareaPromotor` donde `PromoTareaId == tarea.Id`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 3002 | No tienes permiso para ver este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `id` proporcionado |
| 500 | 5000 | Error inesperado al obtener el programa | Excepcion no controlada |

---

### PUT /api/crowdpromotion/programas/{id}

**Descripcion:** Actualiza los datos de un programa de promocion y sus tareas. Solo el artista propietario puede editar. El body de request es identico al de creacion. La logica de tareas: se pueden agregar nuevas tareas (sin `id` en el item), editar tareas existentes (con `id` en el item), y desactivar tareas existentes enviando `esActivo: false`. No se pueden eliminar tareas que ya tienen registros en `PromoTareaPromotor`.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:** `id` (Guid) - Identificador del programa

**Request Body:** Mismos campos que POST. Para las tareas, incluir `id` (Guid) en items existentes para actualizar, omitir `id` para nuevas tareas.

```json
{
  "titulo": "Promociona mi nuevo album (actualizado)",
  "descripcion": "Descripcion actualizada...",
  "tipoPromoId": 1,
  "campaniaCrowdfundingId": "7d89e123-4567-8901-b234-c567d890e123",
  "proyectoArtisticoId": null,
  "urlLanding": "https://weplay.com/campanias/mi-album",
  "codigoTrackingBase": "album-2026",
  "monedaId": 1,
  "importeComisionPorcentaje": 12.00,
  "importeComisionFija": null,
  "fechaInicio": "2026-03-01",
  "fechaFin": "2026-07-01",
  "tareas": [
    {
      "id": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
      "titulo": "Comparte en Instagram Stories",
      "descripcion": "Descripcion actualizada...",
      "tipoEventoPromoId": 3,
      "tipoRewardId": 1,
      "importeRecompensa": 7.00,
      "monedaId": 1,
      "puntosRecompensa": null,
      "urlInstrucciones": "https://docs.example.com/instrucciones-ig",
      "esRepetible": true,
      "maxRepeticiones": 15,
      "fechaInicio": "2026-03-01",
      "fechaFin": "2026-07-01"
    },
    {
      "titulo": "Publica un TikTok",
      "descripcion": "Nueva tarea sin id = se crea",
      "tipoEventoPromoId": 4,
      "tipoRewardId": 1,
      "importeRecompensa": 10.00,
      "monedaId": 1,
      "puntosRecompensa": null,
      "urlInstrucciones": null,
      "esRepetible": true,
      "maxRepeticiones": 5,
      "fechaInicio": null,
      "fechaFin": null
    }
  ]
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "titulo": "Promociona mi nuevo album (actualizado)",
    "fechaActualizacion": "2026-02-25T09:00:00Z"
  },
  "messages": [
    { "message": "Programa actualizado", "errorCode": "0002" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | `titulo` vacio |
| 400 | 1011 | El titulo debe tener al menos 5 caracteres | `titulo` con menos de 5 caracteres |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | `titulo` > 200 chars |
| 400 | 1001 | El tipo de programa es obligatorio | `tipoPromoId` no enviado o 0 |
| 400 | 1001 | La moneda es obligatoria | `monedaId` no enviado o 0 |
| 400 | 1020 | Debe definir al menos una comision (porcentaje o fija) | Ambas comisiones nulas |
| 400 | 1021 | La comision porcentaje debe estar entre 0 y 100 | `importeComisionPorcentaje` fuera de rango |
| 400 | 1023 | La fecha fin debe ser posterior a la fecha inicio | `fechaFin` <= `fechaInicio` |
| 400 | 1024 | El codigo de tracking ya existe para este artista | `codigoTrackingBase` duplicado para otro programa del mismo artista |
| 400 | 1029 | No se puede eliminar una tarea que ya tiene completados | Tarea enviada con `esActivo: false` tiene registros en `PromoTareaPromotor` |
| 400 | 1026 | Las tareas repetibles requieren max repeticiones >= 1 | Tarea con `esRepetible: true` y `maxRepeticiones` nulo o < 1 |
| 403 | 3002 | No tienes permiso para editar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 403 | 3002 | No tienes permiso para vincular esta campana | `campaniaCrowdfundingId` no pertenece al artista |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `id` proporcionado |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 500 | 5000 | Error inesperado al actualizar el programa | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{id}/desactivar

**Descripcion:** Desactiva logicamente un programa de promocion y todas sus tareas activas. Establece `EsActivo = false` en `PromoPrograma` y en todos los `PromoTarea` con `EsActivo == true`. Los registros historicos de promotores e inscripciones no se eliminan. Solo el artista propietario puede desactivar.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:** `id` (Guid) - Identificador del programa

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "esActivo": false,
    "tareasDesactivadas": 3
  },
  "messages": [
    { "message": "Programa desactivado", "errorCode": "0002" }
  ]
}
```

**Nota sobre `tareasDesactivadas`:** Numero de `PromoTarea` que tenian `EsActivo == true` y fueron desactivadas. Puede ser `0` si el programa no tenia tareas activas.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 3002 | No tienes permiso para desactivar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `id` proporcionado |
| 400 | 4020 | El programa ya esta desactivado | `EsActivo == false` en el momento de la solicitud |
| 500 | 5000 | Error inesperado al desactivar el programa | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/crowdpromotion/programas` | Bearer JWT | Artista | `ArtistaId` extraido del token via `UserId`; La campana vinculada debe pertenecer al artista |
| `GET /api/crowdpromotion/programas/mis-programas` | Bearer JWT | Artista | Solo devuelve programas del artista autenticado |
| `GET /api/crowdpromotion/programas/{id}` | Bearer JWT | Artista propietario | Retorna 403 si el programa pertenece a otro artista |
| `PUT /api/crowdpromotion/programas/{id}` | Bearer JWT | Artista propietario | Retorna 403 si el programa pertenece a otro artista |
| `PATCH /api/crowdpromotion/programas/{id}/desactivar` | Bearer JWT | Artista propietario | Retorna 403 si el programa pertenece a otro artista |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string del Identity User)",
  "email": "artista@example.com",
  "role": "Artista",
  "exp": 1740873600,
  "iat": 1740787200
}
```

**Nota:** El `ArtistaId` no se incluye en el token. El backend resuelve el artista consultando `Artista.UserId == sub` en cada request. Este patron es identico al usado en US-CP-01 para resolver el `PromotorId` por `UserId`.

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesUsers

### Proteccion de Rutas Frontend

| Ruta | App | Auth | Redirect | Notas |
|------|-----|------|----------|-------|
| `/dashboard/crowdpromotion/programas` | Admin | Bearer JWT | `/auth/login` | Requiere perfil de Artista existente |
| `/dashboard/crowdpromotion/programas/nuevo` | Admin | Bearer JWT | `/auth/login` | Wizard de creacion |
| `/dashboard/crowdpromotion/programas/{id}` | Admin | Bearer JWT | `/auth/login` | Detalle; redirigir a lista si 403 |
| `/dashboard/crowdpromotion/programas/{id}/editar` | Admin | Bearer JWT | `/auth/login` | Formulario de edicion |

---

## DTOs / Types

### Backend (C#)

#### DTOs de Request

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromoProgramaRequestDto.cs
public class CreatePromoProgramaRequestDto
{
    /// <summary>Requerido. Min 5, max 200 chars.</summary>
    public string Titulo { get; set; } = null!;

    /// <summary>Opcional. Max 4000 chars.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Requerido. Debe existir en Maestra_TipoPromo (1-4).</summary>
    public int TipoPromoId { get; set; }

    /// <summary>Opcional. FK a CampaniaCrowdfunding. Debe pertenecer al artista.</summary>
    public Guid? CampaniaCrowdfundingId { get; set; }

    /// <summary>Opcional. FK a ProyectoArtistico. Debe pertenecer al artista.</summary>
    public Guid? ProyectoArtisticoId { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 500 chars.</summary>
    public string? UrlLanding { get; set; }

    /// <summary>Opcional. Solo alfanumerico y guiones. Max 50 chars. Unico por artista.</summary>
    public string? CodigoTrackingBase { get; set; }

    /// <summary>Requerido. Debe existir en maestras de moneda (1=EUR, 2=USD).</summary>
    public int MonedaId { get; set; }

    /// <summary>Opcional. Rango 0-100 con 2 decimales. Al menos una comision requerida.</summary>
    public decimal? ImporteComisionPorcentaje { get; set; }

    /// <summary>Opcional. >= 0 con 2 decimales. Al menos una comision requerida.</summary>
    public decimal? ImporteComisionFija { get; set; }

    /// <summary>Opcional. >= hoy.</summary>
    public DateOnly? FechaInicio { get; set; }

    /// <summary>Opcional. > FechaInicio cuando ambas estan presentes.</summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>Opcional. Lista de tareas a crear junto con el programa.</summary>
    public List<CreatePromoTareaItemDto> Tareas { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CreatePromoTareaItemDto.cs
public class CreatePromoTareaItemDto
{
    /// <summary>Requerido. Min 3, max 200 chars.</summary>
    public string Titulo { get; set; } = null!;

    /// <summary>Opcional. Max 4000 chars.</summary>
    public string? Descripcion { get; set; }

    /// <summary>Requerido. Debe existir en Maestra_TipoEventoPromo (1-6).</summary>
    public int TipoEventoPromoId { get; set; }

    /// <summary>Requerido. Debe existir en Maestra_TipoReward (1-3).</summary>
    public int TipoRewardId { get; set; }

    /// <summary>Opcional. >= 0 con 2 decimales. Requerido si TipoRewardId es Dinero (1) o Mixto (3).</summary>
    public decimal? ImporteRecompensa { get; set; }

    /// <summary>Opcional. Moneda de la recompensa. Requerido si ImporteRecompensa > 0.</summary>
    public int? MonedaId { get; set; }

    /// <summary>Opcional. >= 0. Requerido si TipoRewardId es Puntos (2) o Mixto (3).</summary>
    public int? PuntosRecompensa { get; set; }

    /// <summary>Opcional. Formato URL valido. Max 500 chars.</summary>
    public string? UrlInstrucciones { get; set; }

    /// <summary>Requerido. Default true.</summary>
    public bool EsRepetible { get; set; } = true;

    /// <summary>Opcional. >= 1. Requerido si EsRepetible == true.</summary>
    public int? MaxRepeticiones { get; set; }

    /// <summary>Opcional. >= FechaInicio del programa.</summary>
    public DateOnly? FechaInicio { get; set; }

    /// <summary>Opcional. <= FechaFin del programa.</summary>
    public DateOnly? FechaFin { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromoProgramaRequestDto.cs
// Mismos campos que CreatePromoProgramaRequestDto con la diferencia de que
// Tareas incluye el campo Id opcional para identificar tareas existentes.
public class UpdatePromoProgramaRequestDto
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoPromoId { get; set; }
    public Guid? CampaniaCrowdfundingId { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string? UrlLanding { get; set; }
    public string? CodigoTrackingBase { get; set; }
    public int MonedaId { get; set; }
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public List<UpdatePromoTareaItemDto> Tareas { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/UpdatePromoTareaItemDto.cs
public class UpdatePromoTareaItemDto
{
    /// <summary>Presente si es tarea existente. Nulo si es nueva tarea.</summary>
    public Guid? Id { get; set; }

    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoEventoPromoId { get; set; }
    public int TipoRewardId { get; set; }
    public decimal? ImporteRecompensa { get; set; }
    public int? MonedaId { get; set; }
    public int? PuntosRecompensa { get; set; }
    public string? UrlInstrucciones { get; set; }
    public bool EsRepetible { get; set; } = true;
    public int? MaxRepeticiones { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }

    /// <summary>Solo aplicable a tareas existentes (Id presente). Permite desactivar una tarea.</summary>
    public bool EsActivo { get; set; } = true;
}
```

#### DTOs de Response

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaCreatedResultDto.cs
public class PromoProgramaCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    /// <summary>Nombre resuelto desde Maestra_TipoPromo.</summary>
    public string TipoPromoNombre { get; set; } = null!;
    public bool EsActivo { get; set; }
    public int TareasCreadas { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaListItemDto.cs
public class PromoProgramaListItemDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int TipoPromoId { get; set; }
    public string TipoPromoNombre { get; set; } = null!;
    /// <summary>Nulo si el programa no tiene campana vinculada.</summary>
    public string? CampaniaTitulo { get; set; }
    public bool EsActivo { get; set; }
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public string MonedaNombre { get; set; } = null!;
    /// <summary>COUNT de PromoProgramaPromotor donde EsAprobado == true.</summary>
    public int NumeroPromotores { get; set; }
    /// <summary>COUNT de PromoTarea donde EsActivo == true.</summary>
    public int NumeroTareas { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaListResultDto.cs
public class PromoProgramaListResultDto
{
    public List<PromoProgramaListItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaDetailDto.cs
public class PromoProgramaDetailDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoPromoId { get; set; }
    public string TipoPromoNombre { get; set; } = null!;
    public Guid? CampaniaCrowdfundingId { get; set; }
    public string? CampaniaTitulo { get; set; }
    public Guid? ProyectoArtisticoId { get; set; }
    public string? UrlLanding { get; set; }
    public string? CodigoTrackingBase { get; set; }
    public int MonedaId { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public decimal? ImporteComisionPorcentaje { get; set; }
    public decimal? ImporteComisionFija { get; set; }
    public bool EsActivo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public List<PromoTareaDetailDto> Tareas { get; set; } = new();
    public List<PromoProgramaPromotorSummaryDto> Promotores { get; set; } = new();
    public PromoProgramaResumenDto Resumen { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoTareaDetailDto.cs
public class PromoTareaDetailDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoEventoPromoId { get; set; }
    public string TipoEventoPromoNombre { get; set; } = null!;
    public int TipoRewardId { get; set; }
    public string TipoRewardNombre { get; set; } = null!;
    public decimal? ImporteRecompensa { get; set; }
    public int? MonedaId { get; set; }
    public string? MonedaNombre { get; set; }
    public int? PuntosRecompensa { get; set; }
    public string? UrlInstrucciones { get; set; }
    public bool EsRepetible { get; set; }
    public int? MaxRepeticiones { get; set; }
    public int Orden { get; set; }
    public bool EsActivo { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    /// <summary>COUNT de PromoTareaPromotor donde PromoTareaId == Id.</summary>
    public int CompletadosPorPromotores { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaPromotorSummaryDto.cs
public class PromoProgramaPromotorSummaryDto
{
    public Guid Id { get; set; }
    public string PromotorNombre { get; set; } = null!;
    public string TipoPromotorNombre { get; set; } = null!;
    public bool EsAprobado { get; set; }
    public bool EsBloqueado { get; set; }
    public DateTime FechaAlta { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaResumenDto.cs
public class PromoProgramaResumenDto
{
    public int TotalPromotoresAprobados { get; set; }
    public int TotalPromotoresPendientes { get; set; }
    public int TotalEventos { get; set; }
    public int TotalConversiones { get; set; }
    public decimal ValorTotalGenerado { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaUpdatedResultDto.cs
public class PromoProgramaUpdatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public DateTime FechaActualizacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/PromoProgramaDesactivadoResultDto.cs
public class PromoProgramaDesactivadoResultDto
{
    public Guid Id { get; set; }
    public bool EsActivo { get; set; }
    /// <summary>Numero de PromoTareas desactivadas como resultado de la operacion.</summary>
    public int TareasDesactivadas { get; set; }
}
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdpromotion.ts
// AGREGAR a continuacion de los tipos existentes de Promotor

// ========== PromoPrograma - Requests ==========

export interface CreatePromoTareaItem {
    titulo: string;              // min 3, max 200 chars
    descripcion?: string;        // optional, max 4000 chars
    tipoEventoPromoId: number;   // 1-6, must exist in Maestra_TipoEventoPromo
    tipoRewardId: number;        // 1-3, must exist in Maestra_TipoReward
    importeRecompensa?: number;  // optional, >= 0; required if tipoRewardId is 1 or 3
    monedaId?: number;           // optional; required if importeRecompensa > 0
    puntosRecompensa?: number;   // optional, >= 0; required if tipoRewardId is 2 or 3
    urlInstrucciones?: string;   // optional, valid URL, max 500 chars
    esRepetible: boolean;        // required, default true
    maxRepeticiones?: number;    // optional, >= 1; required if esRepetible is true
    fechaInicio?: string;        // optional, ISO date string (YYYY-MM-DD)
    fechaFin?: string;           // optional, ISO date string (YYYY-MM-DD)
}

export interface UpdatePromoTareaItem extends CreatePromoTareaItem {
    id?: string;                 // Guid as string; present for existing tasks, absent for new
    esActivo?: boolean;          // optional; set to false to deactivate an existing task
}

export interface CreatePromoProgramaRequest {
    titulo: string;                          // min 5, max 200 chars
    descripcion?: string;                    // optional, max 4000 chars
    tipoPromoId: number;                     // 1-4, must exist in Maestra_TipoPromo
    campaniaCrowdfundingId?: string;         // optional, Guid as string
    proyectoArtisticoId?: string;            // optional, Guid as string
    urlLanding?: string;                     // optional, valid URL, max 500 chars
    codigoTrackingBase?: string;             // optional, alphanumeric+hyphens, max 50 chars
    monedaId: number;                        // required, 1=EUR, 2=USD
    importeComisionPorcentaje?: number;      // optional, 0-100; at least one commission required
    importeComisionFija?: number;            // optional, >= 0; at least one commission required
    fechaInicio?: string;                    // optional, ISO date string (YYYY-MM-DD)
    fechaFin?: string;                       // optional, ISO date string (YYYY-MM-DD)
    tareas?: CreatePromoTareaItem[];         // optional array; can be empty
}

export interface UpdatePromoProgramaRequest extends Omit<CreatePromoProgramaRequest, 'tareas'> {
    tareas?: UpdatePromoTareaItem[];
}

// ========== PromoPrograma - Results ==========

export interface PromoProgramaCreatedResult {
    id: string;                  // Guid as string
    titulo: string;
    tipoPromoNombre: string;
    esActivo: boolean;
    tareasCreadas: number;
    fechaCreacion: string;       // ISO datetime string
}

export interface PromoProgramaListItem {
    id: string;
    titulo: string;
    tipoPromoId: number;
    tipoPromoNombre: string;
    campaniaTitulo: string | null;
    esActivo: boolean;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    monedaNombre: string;
    numeroPromotores: number;
    numeroTareas: number;
    fechaInicio: string | null;  // ISO date string (YYYY-MM-DD)
    fechaFin: string | null;     // ISO date string (YYYY-MM-DD)
    fechaCreacion: string;       // ISO datetime string
}

export interface PromoProgramaListResult {
    items: PromoProgramaListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface PromoTareaDetail {
    id: string;
    titulo: string;
    descripcion: string | null;
    tipoEventoPromoId: number;
    tipoEventoPromoNombre: string;
    tipoRewardId: number;
    tipoRewardNombre: string;
    importeRecompensa: number | null;
    monedaId: number | null;
    monedaNombre: string | null;
    puntosRecompensa: number | null;
    urlInstrucciones: string | null;
    esRepetible: boolean;
    maxRepeticiones: number | null;
    orden: number;
    esActivo: boolean;
    fechaInicio: string | null;
    fechaFin: string | null;
    completadosPorPromotores: number;
}

export interface PromoProgramaPromotorSummary {
    id: string;
    promotorNombre: string;
    tipoPromotorNombre: string;
    esAprobado: boolean;
    esBloqueado: boolean;
    fechaAlta: string;           // ISO datetime string
}

export interface PromoProgramaResumen {
    totalPromotoresAprobados: number;
    totalPromotoresPendientes: number;
    totalEventos: number;
    totalConversiones: number;
    valorTotalGenerado: number;
}

export interface PromoProgramaDetail {
    id: string;
    titulo: string;
    descripcion: string | null;
    tipoPromoId: number;
    tipoPromoNombre: string;
    campaniaCrowdfundingId: string | null;
    campaniaTitulo: string | null;
    proyectoArtisticoId: string | null;
    urlLanding: string | null;
    codigoTrackingBase: string | null;
    monedaId: number;
    monedaNombre: string;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    esActivo: boolean;
    fechaInicio: string | null;
    fechaFin: string | null;
    fechaCreacion: string;
    fechaActualizacion: string | null;
    tareas: PromoTareaDetail[];
    promotores: PromoProgramaPromotorSummary[];
    resumen: PromoProgramaResumen;
}

export interface PromoProgramaUpdatedResult {
    id: string;
    titulo: string;
    fechaActualizacion: string;  // ISO datetime string
}

export interface PromoProgramaDesactivadoResult {
    id: string;
    esActivo: boolean;
    tareasDesactivadas: number;
}

// ========== Maestras Crowdpromotion ==========

export interface TipoPromo {
    id: number;
    nombre: string;
    descripcion: string;
}

export interface TipoEventoPromo {
    id: number;
    nombre: string;
    descripcion: string;
}

export interface TipoRewardPromo {
    id: number;
    nombre: string;
    descripcion: string;
}
```

---

## Validaciones Compartidas

### Programa (campos del programa)

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| titulo | requerido | `.NotEmpty().WithMessage("El titulo es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El titulo es obligatorio')` |
| titulo | min 5 chars | `.MinimumLength(5).WithMessage("El titulo debe tener al menos 5 caracteres").WithErrorCode(Validation_MinLength)` | `.min(5, 'El titulo debe tener al menos 5 caracteres')` |
| titulo | max 200 chars | `.MaximumLength(200).WithMessage("Maximo 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'Maximo 200 caracteres')` |
| descripcion | max 4000 chars cuando presente | `.MaximumLength(4000).When(x => !string.IsNullOrEmpty(x.Descripcion)).WithErrorCode(Validation_MaxLength)` | `.max(4000, 'Maximo 4000 caracteres').optional()` |
| tipoPromoId | requerido | `.NotEmpty().WithMessage("El tipo de programa es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El tipo de programa es obligatorio')` |
| tipoPromoId | debe existir | `.MustAsync(ExistInMaestra).WithMessage("El tipo de programa no existe").WithErrorCode(Validation_ForeignKeyNotFound)` | Validado por el select (opciones fijas del seed) |
| monedaId | requerido | `.NotEmpty().WithMessage("La moneda es obligatoria").WithErrorCode(Validation_Required)` | `.min(1, 'La moneda es obligatoria')` |
| importeComisionPorcentaje/Fija | al menos una | `.Must(x => x.ImporteComisionPorcentaje.HasValue \|\| x.ImporteComisionFija.HasValue).WithMessage("Debe definir al menos una comision").WithErrorCode(Validation_ComisionRequerida)` | `.refine(data => data.importeComisionPorcentaje != null \|\| data.importeComisionFija != null, 'Debe definir al menos una comision')` |
| importeComisionPorcentaje | 0-100 cuando presente | `.InclusiveBetween(0, 100).When(x => x.ImporteComisionPorcentaje.HasValue).WithMessage("Debe estar entre 0 y 100").WithErrorCode(Validation_InvalidRange)` | `.min(0).max(100).optional()` |
| importeComisionFija | >= 0 cuando presente | `.GreaterThanOrEqualTo(0).When(x => x.ImporteComisionFija.HasValue).WithMessage("No puede ser negativa").WithErrorCode(Validation_InvalidRange)` | `.min(0, 'No puede ser negativa').optional()` |
| urlLanding | formato URL cuando presente | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlLanding)).WithErrorCode(Validation_InvalidUrl)` | `.url('La URL de landing no tiene formato valido').optional()` |
| urlLanding | max 500 chars | `.MaximumLength(500).When(...).WithErrorCode(Validation_MaxLength)` | `.max(500, 'Maximo 500 caracteres').optional()` |
| codigoTrackingBase | solo alfanumerico y guiones | `.Matches("^[a-zA-Z0-9-]*$").When(x => !string.IsNullOrEmpty(x.CodigoTrackingBase)).WithErrorCode(Validation_InvalidFormat)` | `.regex(/^[a-zA-Z0-9-]*$/, 'Solo letras, numeros y guiones').optional()` |
| codigoTrackingBase | max 50 chars | `.MaximumLength(50).When(...).WithErrorCode(Validation_MaxLength)` | `.max(50, 'Maximo 50 caracteres').optional()` |
| codigoTrackingBase | unico por artista | `.MustAsync(BeUniqueForArtista).When(...).WithErrorCode(Validation_CodigoTrackingDuplicado)` | No validado en frontend (backend responsibility) |
| fechaFin | > fechaInicio cuando ambas presentes | `.GreaterThan(x => x.FechaInicio).When(...).WithErrorCode(Validation_InvalidDate)` | `.refine(data => !data.fechaFin \|\| !data.fechaInicio \|\| data.fechaFin > data.fechaInicio, 'La fecha fin debe ser posterior a la fecha inicio')` |

### Tarea (campos de cada PromoTarea)

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| titulo | requerido | `.NotEmpty().WithMessage("El titulo de la tarea es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El titulo de la tarea es obligatorio')` |
| titulo | min 3 chars | `.MinimumLength(3).WithMessage("Al menos 3 caracteres").WithErrorCode(Validation_MinLength)` | `.min(3, 'Al menos 3 caracteres')` |
| titulo | max 200 chars | `.MaximumLength(200).WithErrorCode(Validation_MaxLength)` | `.max(200, 'Maximo 200 caracteres')` |
| tipoEventoPromoId | requerido | `.NotEmpty().WithMessage("El tipo de evento es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El tipo de evento es obligatorio')` |
| tipoRewardId | requerido | `.NotEmpty().WithMessage("El tipo de recompensa es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El tipo de recompensa es obligatorio')` |
| maxRepeticiones | >= 1 si esRepetible | `.GreaterThanOrEqualTo(1).When(x => x.EsRepetible).WithMessage("Requiere max repeticiones >= 1").WithErrorCode(Validation_InvalidRange)` | `.min(1, 'Requiere al menos 1 repeticion').optional()` con refine condicional |
| importeRecompensa | requerido si Dinero/Mixto | `.NotNull().When(x => x.TipoRewardId == 1 \|\| x.TipoRewardId == 3).WithErrorCode(Validation_Required)` | Refine condicional en schema |
| puntosRecompensa | requerido si Puntos/Mixto | `.NotNull().When(x => x.TipoRewardId == 2 \|\| x.TipoRewardId == 3).WithErrorCode(Validation_Required)` | Refine condicional en schema |
| urlInstrucciones | formato URL cuando presente | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlInstrucciones)).WithErrorCode(Validation_InvalidUrl)` | `.url('URL invalida').optional()` |
| urlInstrucciones | max 500 chars | `.MaximumLength(500).When(...).WithErrorCode(Validation_MaxLength)` | `.max(500, 'Maximo 500 caracteres').optional()` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdpromotion.schema.ts
// AGREGAR a continuacion de los schemas existentes de createPromotorSchema y updatePromotorSchema

import { z } from 'zod';

// ========== Schema de Tarea para Creacion ==========

const createPromoTareaSchema = z.object({
    titulo: z
        .string({ required_error: 'El titulo de la tarea es obligatorio' })
        .min(1, 'El titulo de la tarea es obligatorio')
        .min(3, 'Al menos 3 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    descripcion: z
        .string()
        .max(4000, 'Maximo 4000 caracteres')
        .optional()
        .or(z.literal('')),

    tipoEventoPromoId: z
        .number({
            required_error: 'El tipo de evento es obligatorio',
            invalid_type_error: 'El tipo de evento es obligatorio',
        })
        .int()
        .min(1, 'El tipo de evento es obligatorio'),

    tipoRewardId: z
        .number({
            required_error: 'El tipo de recompensa es obligatorio',
            invalid_type_error: 'El tipo de recompensa es obligatorio',
        })
        .int()
        .min(1, 'El tipo de recompensa es obligatorio'),

    importeRecompensa: z
        .number()
        .min(0, 'No puede ser negativo')
        .optional(),

    monedaId: z
        .number()
        .int()
        .min(1)
        .optional(),

    puntosRecompensa: z
        .number()
        .int()
        .min(0, 'No puede ser negativo')
        .optional(),

    urlInstrucciones: z
        .string()
        .url('La URL de instrucciones no tiene formato valido')
        .max(500, 'Maximo 500 caracteres')
        .optional()
        .or(z.literal('')),

    esRepetible: z.boolean().default(true),

    maxRepeticiones: z
        .number()
        .int()
        .min(1, 'Requiere al menos 1 repeticion')
        .optional(),

    fechaInicio: z.string().optional(),
    fechaFin: z.string().optional(),
}).refine(
    (data) => !data.esRepetible || data.maxRepeticiones != null,
    {
        message: 'Las tareas repetibles requieren max repeticiones >= 1',
        path: ['maxRepeticiones'],
    }
).refine(
    (data) => {
        // Recompensa monetaria requiere importe
        if (data.tipoRewardId === 1 || data.tipoRewardId === 3) {
            return data.importeRecompensa != null;
        }
        return true;
    },
    {
        message: 'El importe de recompensa es requerido para recompensas monetarias',
        path: ['importeRecompensa'],
    }
).refine(
    (data) => {
        // Recompensa de puntos requiere puntosRecompensa
        if (data.tipoRewardId === 2 || data.tipoRewardId === 3) {
            return data.puntosRecompensa != null;
        }
        return true;
    },
    {
        message: 'Los puntos de recompensa son requeridos para recompensas de puntos',
        path: ['puntosRecompensa'],
    }
);

// ========== Schema de Tarea para Edicion ==========

const updatePromoTareaSchema = createPromoTareaSchema.extend({
    id: z.string().uuid().optional(),
    esActivo: z.boolean().optional().default(true),
});

// ========== Schema de Creacion de Programa ==========

export const createPromoProgramaSchema = z.object({
    titulo: z
        .string({ required_error: 'El titulo es obligatorio' })
        .min(1, 'El titulo es obligatorio')
        .min(5, 'El titulo debe tener al menos 5 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    descripcion: z
        .string()
        .max(4000, 'Maximo 4000 caracteres')
        .optional()
        .or(z.literal('')),

    tipoPromoId: z
        .number({
            required_error: 'El tipo de programa es obligatorio',
            invalid_type_error: 'El tipo de programa es obligatorio',
        })
        .int()
        .min(1, 'El tipo de programa es obligatorio'),

    campaniaCrowdfundingId: z.string().uuid().optional(),

    proyectoArtisticoId: z.string().uuid().optional(),

    urlLanding: z
        .string()
        .url('La URL de landing no tiene formato valido')
        .max(500, 'Maximo 500 caracteres')
        .optional()
        .or(z.literal('')),

    codigoTrackingBase: z
        .string()
        .max(50, 'Maximo 50 caracteres')
        .regex(/^[a-zA-Z0-9-]*$/, 'Solo letras, numeros y guiones')
        .optional()
        .or(z.literal('')),

    monedaId: z
        .number({
            required_error: 'La moneda es obligatoria',
            invalid_type_error: 'La moneda es obligatoria',
        })
        .int()
        .min(1, 'La moneda es obligatoria'),

    importeComisionPorcentaje: z
        .number()
        .min(0, 'No puede ser negativo')
        .max(100, 'No puede superar el 100%')
        .optional(),

    importeComisionFija: z
        .number()
        .min(0, 'No puede ser negativa')
        .optional(),

    fechaInicio: z.string().optional(),

    fechaFin: z.string().optional(),

    tareas: z.array(createPromoTareaSchema).optional().default([]),
}).refine(
    (data) => data.importeComisionPorcentaje != null || data.importeComisionFija != null,
    {
        message: 'Debe definir al menos una comision (porcentaje o fija)',
        path: ['importeComisionPorcentaje'],
    }
).refine(
    (data) => {
        if (data.fechaFin && data.fechaInicio) {
            return data.fechaFin > data.fechaInicio;
        }
        return true;
    },
    {
        message: 'La fecha fin debe ser posterior a la fecha inicio',
        path: ['fechaFin'],
    }
);

// ========== Schema de Edicion de Programa ==========

export const updatePromoProgramaSchema = createPromoProgramaSchema.omit({ tareas: true }).extend({
    tareas: z.array(updatePromoTareaSchema).optional().default([]),
});

// ========== Types Inferidos ==========

export type CreatePromoProgramaFormData = z.infer<typeof createPromoProgramaSchema>;
export type UpdatePromoProgramaFormData = z.infer<typeof updatePromoProgramaSchema>;
```

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo existente `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// AGREGAR a continuacion de las constantes existentes de US-CP-01

// NotFound - nuevos para US-CP-02
public const string NotFound_Artista           = "2016";  // No existe Artista para el UserId del token
public const string NotFound_PromoPrograma     = "2019";  // No existe PromoPrograma con el Id
// Nota: 2017 y 2018 reservados para CampaniaCrowdfunding y ProyectoArtistico no encontrados

// Business Rules - nuevos para US-CP-02
public const string BusinessRule_PromoProgramaAlreadyInactive   = "4020";  // PromoPrograma ya esta desactivado
public const string BusinessRule_TareaConCompletados            = "4021";  // No se puede eliminar tarea con completados

// Validation - nuevos para US-CP-02
public const string Validation_InvalidRange            = "1021";  // Valor numerico fuera de rango
public const string Validation_InvalidDate             = "1022";  // Fecha invalida o fuera de rango
public const string Validation_InvalidFormat           = "1023";  // Formato de cadena no permitido
public const string Validation_ComisionRequerida       = "1020";  // Al menos una comision requerida
public const string Validation_CodigoTrackingDuplicado = "1024";  // CodigoTrackingBase duplicado por artista
```

**Tabla completa de nuevas constantes para esta feature:**

| Constante | Codigo | Descripcion |
|-----------|--------|-------------|
| `Validation_ComisionRequerida` | `1020` | Al menos una de las dos comisiones debe estar definida |
| `Validation_InvalidRange` | `1021` | Valor numerico fuera del rango permitido (porcentaje 0-100, etc.) |
| `Validation_InvalidDate` | `1022` | Fecha invalida o en rango incorrecto (fechaFin <= fechaInicio) |
| `Validation_InvalidFormat` | `1023` | Formato de cadena no permitido (codigoTrackingBase con chars invalidos) |
| `Validation_CodigoTrackingDuplicado` | `1024` | `CodigoTrackingBase` ya existe para el mismo `ArtistaId` |
| `Validation_TareaRepeticionRequerida` | `1025` | Tarea repetible sin `MaxRepeticiones` definido |
| `Validation_RecompensaMonetariaRequerida` | `1026` | Importe de recompensa requerido para TipoReward = Dinero/Mixto |
| `Validation_RecompensaPuntosRequerida` | `1027` | Puntos de recompensa requeridos para TipoReward = Puntos/Mixto |
| `NotFound_Artista` | `2016` | No existe `Artista` con el `UserId` del token |
| `NotFound_PromoPrograma` | `2019` | No existe `PromoPrograma` con el `Id` proporcionado |
| `BusinessRule_PromoProgramaAlreadyInactive` | `4020` | El programa ya tiene `EsActivo = false` |
| `BusinessRule_TareaConCompletados` | `4021` | Tarea no se puede eliminar porque tiene registros en `PromoTareaPromotor` |

**Justificacion de numeracion:** Los codigos continuan la secuencia iniciada por US-CP-01 (que llega hasta `2015` y `4019`). Los codigos `2017` y `2018` quedan reservados para `NotFound_CampaniaCrowdfunding` y `NotFound_ProyectoArtistico` si se formalizan en contratos futuros. Los codigos de validacion `1020`-`1027` continuan desde `1013` (ultimo de US-CP-01).

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// ========== QUERY_KEYS - agregar dentro del bloque crowdpromotion existente ==========
//
// crowdpromotion: {
//   promotor: { ... },           <- ya existe (US-CP-01)
//   maestras: { ... },           <- ya existe (US-CP-01)
//   programas: {                 <- NUEVO (US-CP-02)
//     mis: ...,
//     byId: ...,
//     misProgramasFiltrados: ...,
//   },
// },

// Estructura completa de QUERY_KEYS.crowdpromotion tras agregar US-CP-02:
crowdpromotion: {
    promotor: {
        me: ['crowdpromotion', 'promotor', 'me'] as const,
    },
    maestras: {
        tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
        tiposPromo: ['crowdpromotion', 'maestras', 'tipos-promo'] as const,
        tiposEventoPromo: ['crowdpromotion', 'maestras', 'tipos-evento-promo'] as const,
        tiposRewardPromo: ['crowdpromotion', 'maestras', 'tipos-reward-promo'] as const,
    },
    programas: {
        mis: ['crowdpromotion', 'programas', 'mis'] as const,
        byId: (id: string) => ['crowdpromotion', 'programas', id] as const,
        misFiltrados: (filters: Record<string, unknown>) =>
            ['crowdpromotion', 'programas', 'mis', filters] as const,
    },
},

// ========== API_ROUTES - agregar dentro del bloque crowdpromotion existente ==========

crowdpromotion: {
    promotor: {
        base: '/api/crowdpromotion/promotor',
        me: '/api/crowdpromotion/promotor/me',
        desactivar: '/api/crowdpromotion/promotor/me/desactivar',
    },
    maestras: {
        tiposPromotor: '/api/crowdpromotion/maestras/tipos-promotor',
        tiposPromo: '/api/crowdpromotion/maestras/tipos-promo',
        tiposEventoPromo: '/api/crowdpromotion/maestras/tipos-evento-promo',
        tiposRewardPromo: '/api/crowdpromotion/maestras/tipos-reward-promo',
    },
    programas: {
        base: '/api/crowdpromotion/programas',
        mis: '/api/crowdpromotion/programas/mis-programas',
        byId: (id: string) => `/api/crowdpromotion/programas/${id}`,
        desactivar: (id: string) => `/api/crowdpromotion/programas/${id}/desactivar`,
    },
},

// ========== APP_ROUTES - agregar dentro del bloque dashboard existente ==========

dashboard: {
    // ... entradas existentes ...
    crowdpromotion: {
        // ... entradas existentes (US-CS para crowdsourcing) ...
        programas: {
            list: '/dashboard/crowdpromotion/programas',
            nuevo: '/dashboard/crowdpromotion/programas/nuevo',
            detalle: (id: string) => `/dashboard/crowdpromotion/programas/${id}`,
            editar: (id: string) => `/dashboard/crowdpromotion/programas/${id}/editar`,
        },
    },
},
```

### Constantes de dominio Crowdpromotion - maestras nuevas

```typescript
// Agregar en src/shared/constants/index.ts a continuacion de TIPO_PROMOTOR

// ========== Tipo de Programa de Promocion (seed data Maestra_TipoPromo) ==========

export const TIPO_PROMO = {
    REFERRAL:   1,
    AFILIADO:   2,
    INFLUENCER: 3,
    MIXTO:      4,
} as const;

export const TIPO_PROMO_LABELS: Record<number, string> = {
    1: 'Referral',
    2: 'Afiliado',
    3: 'Influencer',
    4: 'Mixto',
};

export const TIPO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Programa de referidos: comision por cada nuevo backer referido',
    2: 'Programa de afiliados: comision por ventas generadas',
    3: 'Programa para influencers: tareas de contenido con recompensa',
    4: 'Combinacion de referral + tareas de contenido',
};

// ========== Tipo de Evento de Promo (seed data Maestra_TipoEventoPromo) ==========

export const TIPO_EVENTO_PROMO = {
    CLICK:    1,
    PAGE_VIEW: 2,
    SHARE:    3,
    POST:     4,
    SIGNUP:   5,
    BACKING:  6,
} as const;

export const TIPO_EVENTO_PROMO_LABELS: Record<number, string> = {
    1: 'Click',
    2: 'PageView',
    3: 'Share',
    4: 'Post',
    5: 'Signup',
    6: 'Backing',
};

export const TIPO_EVENTO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Click en enlace de referido',
    2: 'Visita a la pagina de la campana',
    3: 'Compartir en redes sociales',
    4: 'Publicacion original sobre la campana',
    5: 'Registro de nuevo usuario referido',
    6: 'Aportacion/backing a la campana (conversion)',
};

// ========== Tipo de Recompensa de Promo (seed data Maestra_TipoReward - contexto promo) ==========

export const TIPO_REWARD_PROMO = {
    DINERO: 1,
    PUNTOS: 2,
    MIXTO:  3,
} as const;

export const TIPO_REWARD_PROMO_LABELS: Record<number, string> = {
    1: 'Dinero',
    2: 'Puntos',
    3: 'Mixto',
};

export const TIPO_REWARD_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Recompensa monetaria',
    2: 'Recompensa en puntos canjeables',
    3: 'Dinero + puntos',
};

// ========== Limits de validacion para programas de promocion ==========
// Agregar dentro del objeto VALIDATION existente:
//   PROGRAMA_TITULO_MIN: 5,
//   PROGRAMA_TITULO_MAX: 200,
//   PROGRAMA_DESCRIPCION_MAX: 4000,
//   PROGRAMA_URL_LANDING_MAX: 500,
//   PROGRAMA_CODIGO_TRACKING_MAX: 50,
//   TAREA_TITULO_MIN: 3,
//   TAREA_TITULO_MAX: 200,
//   TAREA_DESCRIPCION_MAX: 4000,
//   TAREA_URL_INSTRUCCIONES_MAX: 500,
//   COMISION_PORCENTAJE_MAX: 100,
//   PAGE_SIZE_MAX: 50,
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
    // ... mensajes existentes (US-CP-01) ...

    // Crowdpromotion - Programas de Promocion (US-CP-02)
    '1020': 'Debes definir al menos una comision (porcentaje o fija).',
    '1021': 'El valor esta fuera del rango permitido.',
    '1022': 'La fecha fin debe ser posterior a la fecha inicio.',
    '1023': 'El formato del campo no es valido.',
    '1024': 'Este codigo de tracking ya esta en uso. Elige otro.',
    '1025': 'Las tareas repetibles deben tener un numero maximo de repeticiones.',
    '1026': 'El importe de recompensa es requerido para recompensas monetarias.',
    '1027': 'Los puntos de recompensa son requeridos para recompensas de puntos.',
    '2016': 'No tienes un perfil de artista. Crea tu perfil primero.',
    '2019': 'El programa de promocion no existe o fue eliminado.',
    '4020': 'Este programa ya esta desactivado.',
    '4021': 'No se puede desactivar una tarea que ya tiene completados registrados.',

    // Keys semanticos para uso interno en hooks
    PROGRAMA_NOT_FOUND:               'El programa de promocion no existe o fue eliminado.',
    PROGRAMA_ALREADY_INACTIVE:        'Este programa ya esta desactivado.',
    PROGRAMA_TAREA_CON_COMPLETADOS:   'No se puede desactivar una tarea que ya tiene completados registrados.',
    PROGRAMA_COMISION_REQUERIDA:      'Debes definir al menos una comision (porcentaje o fija).',
    PROGRAMA_CODIGO_TRACKING_DUPLICADO: 'Este codigo de tracking ya esta en uso. Elige otro.',
    ARTISTA_NOT_FOUND:                'No tienes un perfil de artista. Crea tu perfil primero.',
    CAMPANA_NOT_BELONGS_TO_ARTISTA:   'No tienes permiso para vincular esta campana.',
};
```

---

## Notas de Implementacion

### Backend

1. **Entidad `PromoPrograma` - campos nuevos requeridos:** La entidad actual no tiene `UrlLanding`, `CodigoTrackingBase`, `ImporteComisionPorcentaje` (5,2) ni `ImporteComisionFija` (18,2). Los campos existentes `ComisionPorConversion` y `ComisionPorClick` se mantienen por compatibilidad pero se agregan los nuevos. La feature US-CP-02 usara `ImporteComisionPorcentaje` e `ImporteComisionFija`. Agregar migracion EF Core antes de implementar.

2. **Entidad `PromoTarea` - campos nuevos requeridos:** Agregar `TipoEventoPromoId` (int, no null), `EsRepetible` (bool, default true), `MaxRepeticiones` (int?), `FechaInicio` (DateTime?) y `FechaFin` (DateTime?). La US usa "Nombre" pero la entidad usa "Titulo"; mantener `Titulo` en la entidad para consistencia.

3. **Resolucion de ArtistaId:** El handler resuelve `ArtistaId` consultando `Artista.UserId == claim("sub")`. Usar `IRequestCacheService` para cachear esta consulta dentro del mismo request (Validator y Handler pueden requerir el mismo dato).

4. **Creacion transaccional:** El `CreatePromoProgramaCommandHandler` debe crear el `PromoPrograma` y todos sus `PromoTarea` dentro de una unica transaccion. Si cualquier parte falla, hacer rollback completo.

5. **Unicidad de CodigoTrackingBase:** La BD debe tener un indice unico compuesto sobre `(ArtistaId, CodigoTrackingBase)` en la tabla `PromosProgramas`. El validator verifica esta unicidad via `MustAsync` llamando al service.

6. **Logica de actualizacion de tareas:** El `UpdatePromoProgramaCommandHandler` implementa merge de tareas:
   - Items con `Id` presente: actualizar la `PromoTarea` existente.
   - Items sin `Id`: crear nuevas `PromoTarea` con `Orden` asignado secuencialmente.
   - `PromoTarea` existentes no incluidas en el request: dejar sin cambios (no se eliminan).
   - Items con `EsActivo: false`: verificar si tienen registros en `PromoTareaPromotor`; si los tiene, retornar error `BusinessRule_TareaConCompletados` (4021).

7. **Desactivacion en cascada:** `DesactivarPromoProgramaCommandHandler` establece `EsActivo = false` en `PromoPrograma` y en todas las `PromoTarea` del programa con `EsActivo == true`. Contar las tareas afectadas para devolver en `TareasDesactivadas`. Toda la operacion en una sola transaccion.

8. **Paginacion en listado:** `GetMisProgramasQuery` aplica `Skip((page-1) * pageSize).Take(pageSize)`. El `TotalCount` se obtiene con una query separada `CountAsync` sobre los mismos filtros para no cargar todos los registros. Usar `AsNoTracking()`.

9. **Campos calculados en detalle:** Los campos del objeto `Resumen` (`TotalEventos`, `TotalConversiones`, `ValorTotalGenerado`) requieren queries a tablas que pueden no existir en el MVP inicial. Si las tablas `PromoEventoPromotor` no existen aun, devolver `0` para estos campos.

### Frontend

1. **Nuevos tipos en archivo existente:** Agregar todas las interfaces de `PromoPrograma` y `PromoTarea` al archivo `src/shared/types/crowdpromotion.ts` a continuacion de los tipos de Promotor existentes.

2. **Nuevos schemas en archivo existente:** Agregar `createPromoProgramaSchema` y `updatePromoProgramaSchema` al archivo `src/shared/schemas/crowdpromotion.schema.ts` a continuacion de `updatePromotorSchema`.

3. **Wizard de creacion:** El wizard de 4 pasos (datos basicos, comisiones, tareas, revision) persiste el estado en React usando `useState` o `useReducer`. Solo hace el POST al completar el paso 4. No guardar estado intermedio en servidor.

4. **Manejo de tareas en wizard:** El paso 3 mantiene un array local de tareas (`useState<CreatePromoTareaItem[]>`). Se puede agregar, editar (reemplazar por indice) y eliminar del array local antes de hacer el POST. No hay llamadas API hasta el submit final.

5. **Hooks principales:**
   - `useMisProgramas(filters)`: Query `GET /api/crowdpromotion/programas/mis-programas` con `QUERY_KEYS.crowdpromotion.programas.mis` o `misFiltrados(filters)`.
   - `usePromoPrograma(id)`: Query `GET /api/crowdpromotion/programas/{id}` con `QUERY_KEYS.crowdpromotion.programas.byId(id)`.
   - `useCreatePromoPrograma()`: Mutation `POST /api/crowdpromotion/programas`; en `onSuccess` invalida `QUERY_KEYS.crowdpromotion.programas.mis` y navega a detalle del programa creado.
   - `useUpdatePromoPrograma(id)`: Mutation `PUT /api/crowdpromotion/programas/{id}`; en `onSuccess` invalida `byId(id)` y `mis`.
   - `useDesactivarPromoPrograma(id)`: Mutation `PATCH /api/crowdpromotion/programas/{id}/desactivar`; en `onSuccess` invalida `byId(id)` y `mis`.

6. **Tipos de maestras en formulario:** Los selects de `tipoPromoId`, `tipoEventoPromoId` y `tipoRewardId` pueden usar las constantes `TIPO_PROMO_LABELS`, `TIPO_EVENTO_PROMO_LABELS` y `TIPO_REWARD_PROMO_LABELS` sin necesidad de endpoints de maestras en el MVP.

7. **Aviso de promotores inscritos:** Al editar, hacer primero el query `GET /programas/{id}` y si `resumen.totalPromotoresAprobados > 0`, mostrar un banner informativo antes del formulario. No bloquear la edicion.

8. **Campos de fecha en formulario:** Usar `<input type="date">` o el date picker de shadcn. Enviar al backend como string ISO `YYYY-MM-DD`. El backend espera `DateOnly`.

9. **Campos URL vacios:** Aplicar la misma convencion que US-CP-01: convertir strings vacios a `undefined` antes del submit para evitar errores de validacion de formato URL en el backend.

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (5 endpoints documentados)
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (CreatePromoProgramaRequestDto, CreatePromoTareaItemDto, UpdatePromoProgramaRequestDto, UpdatePromoTareaItemDto, PromoProgramaCreatedResultDto, PromoProgramaListItemDto, PromoProgramaListResultDto, PromoProgramaDetailDto, PromoTareaDetailDto, PromoProgramaPromotorSummaryDto, PromoProgramaResumenDto, PromoProgramaUpdatedResultDto, PromoProgramaDesactivadoResultDto)
- [x] Types TypeScript equivalentes en src/shared/types/crowdpromotion.ts
- [x] Schemas Zod con mismas reglas (createPromoProgramaSchema, updatePromoProgramaSchema)
- [x] Constantes compartidas (QUERY_KEYS, API_ROUTES, APP_ROUTES agregados al bloque crowdpromotion existente)
- [x] Constantes de dominio (TIPO_PROMO, TIPO_EVENTO_PROMO, TIPO_REWARD_PROMO con labels y descriptions)
- [x] Nuevas constantes ServiceResponseMessageType (1020-1027, 2016, 2019, 4020, 4021)
- [x] Mapeo de errores a mensajes UI
- [x] Regla de negocio: al menos una comision definida (porcentaje o fija)
- [x] Regla de negocio: CodigoTrackingBase unico por artista
- [x] Regla de negocio: creacion transaccional (programa + tareas atomicamente)
- [x] Regla de negocio: desactivacion de programa desactiva todas sus tareas
- [x] Regla de negocio: tareas con completados no se pueden eliminar/desactivar
- [x] Regla de negocio: ArtistaId resuelto desde JWT (no aceptado en body)
- [x] Nota sobre campos nuevos requeridos en entidades PromoPrograma y PromoTarea
- [x] Nota sobre campo "Titulo" en PromoTarea (la US dice "Nombre", la entidad usa "Titulo")
- [x] Notas de implementacion Backend + Frontend
