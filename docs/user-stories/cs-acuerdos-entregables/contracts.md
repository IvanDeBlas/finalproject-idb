# Contratos: Acuerdos, Milestones y Entregables

> **Feature:** cs-acuerdos-entregables (US-CS-04)
> **Ultima actualizacion:** 2026-02-18

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Endpoints API

### POST /api/crowdsourcing/propuestas/{id}/aceptar

**Descripcion:** Acepta una propuesta en estado `Pendiente` y crea un `AcuerdoCrowdsourcing`. Operacion transaccional: (1) propuesta pasa a `Aceptada`, (2) demas propuestas pendientes de la misma necesidad pasan a `Rechazada`, (3) necesidad pasa a `En Progreso`, (4) se crea `ConversacionCrowdsourcing` vinculada al acuerdo. El artista puede ajustar `tituloInterno`, `fechaInicio` y `fechaFinPrevista` antes de confirmar.

**Autorizacion:** Bearer JWT - Artista propietario de la necesidad

**Path Parameters:**
- `id` (Guid) - ID de la propuesta a aceptar

**Request Body:**
```json
{
  "tituloInterno": "Mezcla EP Los Rockeros",
  "fechaInicio": "2026-03-01",
  "fechaFinPrevista": "2026-03-15"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "acuerdoId": "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    "tituloInterno": "Mezcla EP Los Rockeros",
    "estadoAcuerdoNombre": "Activo",
    "importeTotalPactado": 450.00,
    "monedaNombre": "EUR",
    "conversacionId": "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    "propuestasRechazadas": 2
  },
  "messages": [
    {
      "message": "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional.",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo interno es obligatorio | tituloInterno vacio |
| 400 | 1002 | El titulo interno no puede superar los 200 caracteres | tituloInterno > 200 chars |
| 400 | 1012 | La fecha de fin prevista debe ser posterior a la fecha de inicio | fechaFinPrevista <= fechaInicio |
| 400 | 4007 | La propuesta no esta en estado Pendiente | EstadoPropuestaId != Pendiente |
| 400 | 4008 | Ya existe un acuerdo activo para esta necesidad | Ya hay un AcuerdoCrowdsourcing con estado Activo para la misma NecesidadId |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para aceptar esta propuesta | UserId del token no corresponde al artista propietario de la necesidad |
| 404 | 2010 | Propuesta no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al crear el acuerdo | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/propuestas/{id}/rechazar

**Descripcion:** Rechaza individualmente una propuesta en estado `Pendiente`. El motivo de rechazo se almacena en la base de datos pero el profesional solo puede ver el cambio de estado, no el motivo.

**Autorizacion:** Bearer JWT - Artista propietario de la necesidad

**Path Parameters:**
- `id` (Guid) - ID de la propuesta a rechazar

**Request Body:**
```json
{
  "motivo": "El presupuesto no se ajusta a nuestras posibilidades"
}
```

**Nota sobre `motivo`:** Opcional. Max 500 caracteres. Si se omite, se envia `null`. El profesional no puede ver este campo.

**Response 200 OK:**
```json
{
  "data": {
    "id": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    "estadoPropuestaNombre": "Rechazada"
  },
  "messages": [
    {
      "message": "Propuesta rechazada",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1002 | El motivo no puede superar los 500 caracteres | motivo > 500 chars |
| 400 | 4007 | Solo se pueden rechazar propuestas en estado Pendiente | EstadoPropuestaId != Pendiente |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para rechazar esta propuesta | UserId del token no corresponde al artista propietario de la necesidad |
| 404 | 2010 | Propuesta no encontrada | ID no existe en DB |
| 500 | 5000 | Error inesperado al rechazar la propuesta | Excepcion no controlada |

---

### GET /api/crowdsourcing/acuerdos/{id}

**Descripcion:** Devuelve el detalle completo de un acuerdo incluyendo milestones con sus entregables anidados, datos de las partes, importe asignado calculado, el rol del usuario autenticado dentro del acuerdo y un timeline de actividad reciente. Solo los dos participantes del acuerdo (artista y profesional) pueden acceder.

**Autorizacion:** Bearer JWT - Participante del acuerdo (artista o profesional)

**Path Parameters:**
- `id` (Guid) - ID del acuerdo

**Response 200 OK:**
```json
{
  "data": {
    "id": "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    "tituloInterno": "Mezcla EP Los Rockeros",
    "estadoAcuerdoId": 1,
    "estadoAcuerdoNombre": "Activo",
    "importeTotalPactado": 450.00,
    "monedaNombre": "EUR",
    "fechaInicio": "2026-03-01T00:00:00Z",
    "fechaFinPrevista": "2026-03-15T00:00:00Z",
    "fechaFinReal": null,
    "artista": {
      "id": "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
      "nombreArtistico": "Los Rockeros"
    },
    "profesional": {
      "userId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "perfilProfesionalId": "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
      "nombre": "Studio Mix Pro"
    },
    "necesidad": {
      "id": "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
      "titulo": "Mezcla de pistas para EP"
    },
    "conversacionId": "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    "milestones": [
      {
        "id": "a9b8c7d6-e5f4-3a2b-1c0d-9e8f7a6b5c4d",
        "titulo": "Mezcla de pistas 1-3",
        "descripcion": "Mezcla de las primeras 3 canciones del EP",
        "orden": 1,
        "importeParcial": 270.00,
        "porcentajeParcial": 60.00,
        "fechaLimite": "2026-03-08T00:00:00Z",
        "fechaCompletado": null,
        "entregables": [
          {
            "id": "b8c7d6e5-f4a3-2b1c-0d9e-8f7a6b5c4d3e",
            "titulo": "Mezcla cancion 1 - v1",
            "descripcion": "Primera version de la mezcla de la cancion 1",
            "urlRecurso": "https://drive.google.com/file/xyz",
            "estadoEntregableId": 1,
            "estadoEntregableNombre": "Entregado",
            "comentarioAprobacion": null,
            "comentarioRechazo": null,
            "fechaAprobacion": null,
            "fechaCreacion": "2026-03-05T14:00:00Z"
          }
        ]
      }
    ],
    "importeAsignado": 270.00,
    "porcentajeAsignado": 60.00,
    "miRol": "Artista",
    "timeline": [
      {
        "accion": "Acuerdo creado",
        "fecha": "2026-03-01T10:00:00Z",
        "actor": "Los Rockeros"
      },
      {
        "accion": "Milestone agregado: Mezcla de pistas 1-3",
        "fecha": "2026-03-02T09:00:00Z",
        "actor": "Los Rockeros"
      },
      {
        "accion": "Entregable subido: Mezcla cancion 1 - v1",
        "fecha": "2026-03-05T14:00:00Z",
        "actor": "Studio Mix Pro"
      }
    ]
  },
  "messages": []
}
```

**Notas sobre campos calculados:**
- `importeAsignado`: suma de `ImporteParcial` de todos los milestones
- `porcentajeAsignado`: `(importeAsignado / importeTotalPactado) * 100`
- `porcentajeParcial` por milestone: `(milestone.ImporteParcial / importeTotalPactado) * 100`
- `miRol`: `"Artista"` si `UserId == artista.userId`, `"Profesional"` si `UserId == profesional.userId`
- `timeline`: ultimos 20 eventos de actividad del acuerdo, ordenados de mas reciente a mas antiguo

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes acceso a este acuerdo | UserId del token no es artista ni profesional del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado al obtener el acuerdo | Excepcion no controlada |

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones

**Descripcion:** Crea un nuevo milestone para un acuerdo activo. Solo el artista puede crear milestones. La suma total de importes parciales de todos los milestones no puede superar `ImporteTotalPactado`. El orden se asigna automaticamente como el siguiente disponible.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo

**Path Parameters:**
- `acuerdoId` (Guid) - ID del acuerdo

**Request Body:**
```json
{
  "titulo": "Mezcla de pistas 1-3",
  "descripcion": "Mezcla de las primeras 3 canciones del EP",
  "importeParcial": 270.00,
  "fechaLimite": "2026-03-08"
}
```

**Nota sobre `fechaLimite`:** Opcional. Si se envia debe ser >= `fechaInicio` del acuerdo.

**Response 201 Created:**
```json
{
  "data": {
    "id": "a9b8c7d6-e5f4-3a2b-1c0d-9e8f7a6b5c4d",
    "titulo": "Mezcla de pistas 1-3",
    "orden": 1,
    "importeParcial": 270.00,
    "porcentajeParcial": 60.00,
    "importeAsignadoTotal": 270.00
  },
  "messages": [
    {
      "message": "Milestone creado",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | titulo vacio |
| 400 | 1011 | El titulo debe tener al menos 3 caracteres | titulo < 3 chars |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | titulo > 200 chars |
| 400 | 1002 | La descripcion no puede superar los 1000 caracteres | descripcion > 1000 chars |
| 400 | 1001 | El importe parcial debe ser mayor a 0 | importeParcial <= 0 |
| 400 | 1012 | La fecha limite no puede ser anterior a la fecha de inicio del acuerdo | fechaLimite < acuerdo.FechaInicio |
| 400 | 4009 | La suma de importes de milestones supera el importe total pactado | SUM(importeParcial) > ImporteTotalPactado |
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede gestionar los milestones | UserId del token no es el artista del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | acuerdoId no existe en DB |
| 500 | 5000 | Error inesperado al crear el milestone | Excepcion no controlada |

---

### PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

**Descripcion:** Edita un milestone existente. Solo el artista puede editar milestones. No se puede editar un milestone que ya este marcado como completado (`FechaCompletado != null`). Las mismas reglas de suma de importes aplican excluyendo el milestone actual del calculo.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo

**Path Parameters:**
- `acuerdoId` (Guid) - ID del acuerdo
- `id` (Guid) - ID del milestone

**Request Body:**
```json
{
  "titulo": "Mezcla de pistas 1-3 (actualizado)",
  "descripcion": "Mezcla de las primeras 3 canciones del EP con ajustes de volumen",
  "importeParcial": 300.00,
  "fechaLimite": "2026-03-10"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "a9b8c7d6-e5f4-3a2b-1c0d-9e8f7a6b5c4d",
    "titulo": "Mezcla de pistas 1-3 (actualizado)",
    "orden": 1,
    "importeParcial": 300.00,
    "porcentajeParcial": 66.67,
    "importeAsignadoTotal": 300.00
  },
  "messages": [
    {
      "message": "Milestone actualizado",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | titulo vacio |
| 400 | 1011 | El titulo debe tener al menos 3 caracteres | titulo < 3 chars |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | titulo > 200 chars |
| 400 | 1002 | La descripcion no puede superar los 1000 caracteres | descripcion > 1000 chars |
| 400 | 1001 | El importe parcial debe ser mayor a 0 | importeParcial <= 0 |
| 400 | 1012 | La fecha limite no puede ser anterior a la fecha de inicio del acuerdo | fechaLimite < acuerdo.FechaInicio |
| 400 | 4009 | La suma de importes de milestones supera el importe total pactado | SUM(importeParcial excluyendo el actual) + nuevo importeParcial > ImporteTotalPactado |
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 400 | 4011 | No se puede editar un milestone completado | FechaCompletado != null |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede gestionar los milestones | UserId del token no es el artista del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | acuerdoId no existe en DB |
| 404 | 2012 | Milestone no encontrado | id no existe o no pertenece al acuerdo |
| 500 | 5000 | Error inesperado al actualizar el milestone | Excepcion no controlada |

---

### DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}

**Descripcion:** Elimina un milestone. No se puede eliminar si tiene entregables asociados o si ya esta completado. Solo el artista puede eliminar milestones en un acuerdo activo.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo

**Path Parameters:**
- `acuerdoId` (Guid) - ID del acuerdo
- `id` (Guid) - ID del milestone a eliminar

**Request Body:** Ninguno.

**Response 204 No Content**

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 400 | 4011 | No se puede eliminar un milestone completado | FechaCompletado != null |
| 400 | 4012 | No se puede eliminar un milestone con entregables asociados | Existen AcuerdoCrowdsourcingEntregable con ese MilestoneId |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede gestionar los milestones | UserId del token no es el artista del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | acuerdoId no existe en DB |
| 404 | 2012 | Milestone no encontrado | id no existe o no pertenece al acuerdo |
| 500 | 5000 | Error inesperado al eliminar el milestone | Excepcion no controlada |

---

### POST /api/crowdsourcing/acuerdos/{acuerdoId}/entregables

**Descripcion:** El profesional sube un nuevo entregable vinculado a un acuerdo activo. El entregable se crea con estado `Entregado`. MVP: solo URLs externas, no upload de archivos. El `milestoneId` es opcional; si se proporciona debe pertenecer al mismo acuerdo.

**Autorizacion:** Bearer JWT - Profesional participante del acuerdo

**Path Parameters:**
- `acuerdoId` (Guid) - ID del acuerdo

**Request Body:**
```json
{
  "titulo": "Mezcla cancion 1 - v1",
  "descripcion": "Primera version de la mezcla de la cancion 1",
  "urlRecurso": "https://drive.google.com/file/xyz",
  "milestoneId": "a9b8c7d6-e5f4-3a2b-1c0d-9e8f7a6b5c4d"
}
```

**Nota sobre `milestoneId`:** Opcional. Si se envia, debe corresponder a un milestone del mismo acuerdo.
**Nota sobre `urlRecurso`:** Opcional. Si se envia, debe ser una URL valida.

**Response 201 Created:**
```json
{
  "data": {
    "id": "b8c7d6e5-f4a3-2b1c-0d9e-8f7a6b5c4d3e",
    "titulo": "Mezcla cancion 1 - v1",
    "estadoEntregableNombre": "Entregado",
    "fechaCreacion": "2026-03-05T14:00:00Z"
  },
  "messages": [
    {
      "message": "Entregable subido correctamente. El artista sera notificado.",
      "errorCode": "0001"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El titulo es obligatorio | titulo vacio |
| 400 | 1011 | El titulo debe tener al menos 3 caracteres | titulo < 3 chars |
| 400 | 1002 | El titulo no puede superar los 200 caracteres | titulo > 200 chars |
| 400 | 1002 | La descripcion no puede superar los 1000 caracteres | descripcion > 1000 chars |
| 400 | 1013 | La URL del recurso no es valida | urlRecurso no es una URL valida |
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 400 | 1010 | El milestone no pertenece a este acuerdo | milestoneId no existe en el acuerdo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el profesional puede subir entregables | UserId del token no es el profesional del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | acuerdoId no existe en DB |
| 500 | 5000 | Error inesperado al subir el entregable | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/entregables/{id}/aprobar

**Descripcion:** El artista aprueba un entregable en estado `Entregado`. Se registra `FechaAprobacion`. El comentario es opcional (max 500 chars). La respuesta incluye `todosAprobadosEnMilestone` para que el frontend sugiera marcar el milestone como completado.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo al que pertenece el entregable

**Path Parameters:**
- `id` (Guid) - ID del entregable

**Request Body:**
```json
{
  "comentario": "Excelente mezcla, me encanta el resultado"
}
```

**Nota sobre `comentario`:** Opcional. Max 500 caracteres.

**Response 200 OK:**
```json
{
  "data": {
    "id": "b8c7d6e5-f4a3-2b1c-0d9e-8f7a6b5c4d3e",
    "estadoEntregableNombre": "Aprobado",
    "fechaAprobacion": "2026-03-06T10:00:00Z",
    "todosAprobadosEnMilestone": true
  },
  "messages": [
    {
      "message": "Entregable aprobado",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1002 | El comentario no puede superar los 500 caracteres | comentario > 500 chars |
| 400 | 4013 | Solo se pueden aprobar entregables en estado Entregado | EstadoEntregableId != Entregado |
| 400 | 4010 | El acuerdo no esta activo | Acuerdo del entregable no esta Activo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede aprobar entregables | UserId del token no es el artista del acuerdo |
| 404 | 2013 | Entregable no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado al aprobar el entregable | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/entregables/{id}/rechazar

**Descripcion:** El artista rechaza un entregable en estado `Entregado`. El comentario de rechazo es obligatorio (min 10, max 500 chars) para explicar que debe corregirse. El profesional puede subir una nueva version a continuacion.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo al que pertenece el entregable

**Path Parameters:**
- `id` (Guid) - ID del entregable

**Request Body:**
```json
{
  "comentario": "La voz esta demasiado baja en el coro, necesita mas presencia. Tambien ajustar el bajo en el puente."
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "b8c7d6e5-f4a3-2b1c-0d9e-8f7a6b5c4d3e",
    "estadoEntregableNombre": "Rechazado"
  },
  "messages": [
    {
      "message": "Entregable rechazado. El profesional sera notificado.",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El comentario es obligatorio al rechazar | comentario vacio |
| 400 | 1011 | El comentario debe tener al menos 10 caracteres explicando que debe corregirse | comentario < 10 chars |
| 400 | 1002 | El comentario no puede superar los 500 caracteres | comentario > 500 chars |
| 400 | 4013 | Solo se pueden rechazar entregables en estado Entregado | EstadoEntregableId != Entregado |
| 400 | 4010 | El acuerdo no esta activo | Acuerdo del entregable no esta Activo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede rechazar entregables | UserId del token no es el artista del acuerdo |
| 404 | 2013 | Entregable no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado al rechazar el entregable | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/acuerdos/{id}/completar

**Descripcion:** El artista completa un acuerdo en estado `Activo`. Se registra `FechaFinReal = ahora`. La necesidad pasa a `Cerrada`. Se habilitan las valoraciones. Si hay entregables en estado `Entregado` (pendientes de revision), se permite completar igualmente pero el cliente muestra un aviso previo.

**Autorizacion:** Bearer JWT - Artista participante del acuerdo

**Path Parameters:**
- `id` (Guid) - ID del acuerdo

**Request Body:** Ninguno.

**Response 200 OK:**
```json
{
  "data": {
    "id": "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    "estadoAcuerdoNombre": "Completado",
    "fechaFinReal": "2026-03-14T16:00:00Z"
  },
  "messages": [
    {
      "message": "Acuerdo completado. Puedes dejar una valoracion al profesional.",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | Solo el artista puede completar el acuerdo | UserId del token no es el artista del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado al completar el acuerdo | Excepcion no controlada |

---

### PATCH /api/crowdsourcing/acuerdos/{id}/cancelar

**Descripcion:** Cualquiera de los dos participantes puede cancelar un acuerdo en estado `Activo`. Accion irreversible. Se registra `FechaFinReal`, `MotivoCancelacion` y `CanceladoPor` (UserId). La necesidad vuelve a estado `Abierta`. Milestones y entregables se conservan como historial pero no pueden modificarse.

**Autorizacion:** Bearer JWT - Participante del acuerdo (artista o profesional)

**Path Parameters:**
- `id` (Guid) - ID del acuerdo

**Request Body:**
```json
{
  "motivo": "No puedo continuar por motivos personales. Lamento las molestias causadas."
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    "estadoAcuerdoNombre": "Cancelado",
    "fechaFinReal": "2026-03-10T12:00:00Z",
    "necesidadEstadoNombre": "Abierta"
  },
  "messages": [
    {
      "message": "Acuerdo cancelado",
      "errorCode": "0002"
    }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El motivo es obligatorio | motivo vacio |
| 400 | 1011 | El motivo debe tener al menos 20 caracteres | motivo < 20 chars |
| 400 | 1002 | El motivo no puede superar los 1000 caracteres | motivo > 1000 chars |
| 400 | 4010 | El acuerdo no esta activo | EstadoAcuerdoId != Activo |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido/expirado |
| 403 | 3002 | No tienes permiso para cancelar este acuerdo | UserId del token no es ni artista ni profesional del acuerdo |
| 404 | 2011 | Acuerdo no encontrado | ID no existe en DB |
| 500 | 5000 | Error inesperado al cancelar el acuerdo | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Rol | Notas |
|----------|------|-----|-------|
| `POST /api/crowdsourcing/propuestas/{id}/aceptar` | Bearer JWT | Artista propietario de la necesidad | ArtistaId se resuelve por UserId del token |
| `PATCH /api/crowdsourcing/propuestas/{id}/rechazar` | Bearer JWT | Artista propietario de la necesidad | ArtistaId se resuelve por UserId del token |
| `GET /api/crowdsourcing/acuerdos/{id}` | Bearer JWT | Participante del acuerdo | Valida ArtistaId o UserIdProveedor == UserId del token |
| `POST /api/crowdsourcing/acuerdos/{acuerdoId}/milestones` | Bearer JWT | Artista del acuerdo | Solo el artista puede crear milestones |
| `PUT /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` | Bearer JWT | Artista del acuerdo | Solo si milestone no esta completado |
| `DELETE /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id}` | Bearer JWT | Artista del acuerdo | Solo si no tiene entregables y no esta completado |
| `POST /api/crowdsourcing/acuerdos/{acuerdoId}/entregables` | Bearer JWT | Profesional del acuerdo | Solo el profesional puede subir entregables |
| `PATCH /api/crowdsourcing/entregables/{id}/aprobar` | Bearer JWT | Artista del acuerdo | El entregable debe estar en estado Entregado |
| `PATCH /api/crowdsourcing/entregables/{id}/rechazar` | Bearer JWT | Artista del acuerdo | El entregable debe estar en estado Entregado |
| `PATCH /api/crowdsourcing/acuerdos/{id}/completar` | Bearer JWT | Artista del acuerdo | Solo el artista puede completar |
| `PATCH /api/crowdsourcing/acuerdos/{id}/cancelar` | Bearer JWT | Participante del acuerdo | Tanto artista como profesional pueden cancelar |

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

**Nota:** Los claims `artistaId` y `perfilProfesionalId` no se incluyen en el token. El backend resuelve el `ArtistaId` y el `PerfilProfesionalId` consultando las tablas correspondientes por `UserId`. La autorizacion por participante se valida comparando el `UserId` del token contra `AcuerdoCrowdsourcing.ArtistaId` (resuelto) y `AcuerdoCrowdsourcing.UserIdProveedor`.

**Configuracion JWT:**
- **Algoritmo:** HS256
- **Expiracion:** 24 horas
- **Issuer:** WePlayRises
- **Audience:** WePlayRisesClient

### Proteccion de Rutas Frontend

| Ruta (Landing/Admin) | Auth | Redirect | Notas |
|----------------------|------|----------|-------|
| `/crowdsourcing/acuerdos/:id` | Bearer JWT | `/auth/login` | Detalle del acuerdo; acceso solo a participantes |

---

## DTOs / Types

### Backend (C#)

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AceptarPropuestaRequestDto.cs
public class AceptarPropuestaRequestDto
{
    /// <summary>Titulo interno del acuerdo; default: titulo de la necesidad. Max 200 chars.</summary>
    public string TituloInterno { get; set; } = null!;
    /// <summary>Fecha de inicio del trabajo pactado. Default: fecha actual.</summary>
    public DateTime FechaInicio { get; set; }
    /// <summary>Fecha fin prevista. Opcional. Si se envia debe ser posterior a FechaInicio.</summary>
    public DateTime? FechaFinPrevista { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AceptarPropuestaResultDto.cs
public class AceptarPropuestaResultDto
{
    public Guid AcuerdoId { get; set; }
    public string TituloInterno { get; set; } = null!;
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public decimal ImporteTotalPactado { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public Guid ConversacionId { get; set; }
    /// <summary>Numero de propuestas que pasaron a Rechazada automaticamente.</summary>
    public int PropuestasRechazadas { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RechazarPropuestaRequestDto.cs
public class RechazarPropuestaRequestDto
{
    /// <summary>Motivo de rechazo. Opcional. Max 500 chars. El profesional NO puede verlo.</summary>
    public string? Motivo { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RechazarPropuestaResultDto.cs
public class RechazarPropuestaResultDto
{
    public Guid Id { get; set; }
    public string EstadoPropuestaNombre { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AcuerdoDto.cs
public class AcuerdoDto
{
    public Guid Id { get; set; }
    public string TituloInterno { get; set; } = null!;
    public int EstadoAcuerdoId { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public decimal ImporteTotalPactado { get; set; }
    public string MonedaNombre { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFinPrevista { get; set; }
    public DateTime? FechaFinReal { get; set; }
    public AcuerdoArtistaDto Artista { get; set; } = null!;
    public AcuerdoProfesionalDto Profesional { get; set; } = null!;
    public AcuerdoNecesidadDto Necesidad { get; set; } = null!;
    public Guid? ConversacionId { get; set; }
    public List<MilestoneDto> Milestones { get; set; } = new();
    /// <summary>Suma de ImporteParcial de todos los milestones.</summary>
    public decimal ImporteAsignado { get; set; }
    /// <summary>(ImporteAsignado / ImporteTotalPactado) * 100. Redondeado a 2 decimales.</summary>
    public decimal PorcentajeAsignado { get; set; }
    /// <summary>"Artista" o "Profesional" segun el UserId del solicitante.</summary>
    public string MiRol { get; set; } = null!;
    public List<AcuerdoTimelineEventoDto> Timeline { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AcuerdoArtistaDto.cs
public class AcuerdoArtistaDto
{
    public Guid Id { get; set; }
    public string NombreArtistico { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AcuerdoProfesionalDto.cs
public class AcuerdoProfesionalDto
{
    public string UserId { get; set; } = null!;
    public Guid PerfilProfesionalId { get; set; }
    public string Nombre { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AcuerdoNecesidadDto.cs
public class AcuerdoNecesidadDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AcuerdoTimelineEventoDto.cs
public class AcuerdoTimelineEventoDto
{
    public string Accion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public string Actor { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MilestoneDto.cs
public class MilestoneDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public decimal ImporteParcial { get; set; }
    /// <summary>(ImporteParcial / ImporteTotalPactado) * 100. Calculado en el handler.</summary>
    public decimal PorcentajeParcial { get; set; }
    public DateTime? FechaLimite { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public List<EntregableDto> Entregables { get; set; } = new();
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/EntregableDto.cs
public class EntregableDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string? UrlRecurso { get; set; }
    public int EstadoEntregableId { get; set; }
    public string EstadoEntregableNombre { get; set; } = null!;
    public string? ComentarioAprobacion { get; set; }
    public string? ComentarioRechazo { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateMilestoneRequestDto.cs
public class CreateMilestoneRequestDto
{
    /// <summary>Requerido. Min 3, max 200 chars.</summary>
    public string Titulo { get; set; } = null!;
    /// <summary>Opcional. Max 1000 chars.</summary>
    public string? Descripcion { get; set; }
    /// <summary>Requerido. Debe ser > 0.</summary>
    public decimal ImporteParcial { get; set; }
    /// <summary>Opcional. Debe ser >= FechaInicio del acuerdo.</summary>
    public DateTime? FechaLimite { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/MilestoneCreatedResultDto.cs
public class MilestoneCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public int Orden { get; set; }
    public decimal ImporteParcial { get; set; }
    public decimal PorcentajeParcial { get; set; }
    /// <summary>Suma total de importes de todos los milestones del acuerdo tras la operacion.</summary>
    public decimal ImporteAsignadoTotal { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CreateEntregableRequestDto.cs
public class CreateEntregableRequestDto
{
    /// <summary>Requerido. Min 3, max 200 chars.</summary>
    public string Titulo { get; set; } = null!;
    /// <summary>Opcional. Max 1000 chars.</summary>
    public string? Descripcion { get; set; }
    /// <summary>Opcional. URL valida (Dropbox, Drive, WeTransfer, etc.).</summary>
    public string? UrlRecurso { get; set; }
    /// <summary>Opcional. Debe pertenecer al mismo acuerdo.</summary>
    public Guid? MilestoneId { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/EntregableCreatedResultDto.cs
public class EntregableCreatedResultDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string EstadoEntregableNombre { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AprobarEntregableRequestDto.cs
public class AprobarEntregableRequestDto
{
    /// <summary>Opcional. Max 500 chars.</summary>
    public string? Comentario { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/AprobarEntregableResultDto.cs
public class AprobarEntregableResultDto
{
    public Guid Id { get; set; }
    public string EstadoEntregableNombre { get; set; } = null!;
    public DateTime FechaAprobacion { get; set; }
    /// <summary>true si todos los entregables del milestone vinculado estan en estado Aprobado.</summary>
    public bool TodosAprobadosEnMilestone { get; set; }
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RechazarEntregableRequestDto.cs
public class RechazarEntregableRequestDto
{
    /// <summary>Obligatorio. Min 10, max 500 chars. Explica que debe corregirse.</summary>
    public string Comentario { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/RechazarEntregableResultDto.cs
public class RechazarEntregableResultDto
{
    public Guid Id { get; set; }
    public string EstadoEntregableNombre { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CancelarAcuerdoRequestDto.cs
public class CancelarAcuerdoRequestDto
{
    /// <summary>Obligatorio. Min 20, max 1000 chars.</summary>
    public string Motivo { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CancelarAcuerdoResultDto.cs
public class CancelarAcuerdoResultDto
{
    public Guid Id { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public DateTime FechaFinReal { get; set; }
    public string NecesidadEstadoNombre { get; set; } = null!;
}
```

```csharp
// Ruta: Modules/Crowdsourcing/Crowdsourcing.Application/Dtos/CompletarAcuerdoResultDto.cs
public class CompletarAcuerdoResultDto
{
    public Guid Id { get; set; }
    public string EstadoAcuerdoNombre { get; set; } = null!;
    public DateTime FechaFinReal { get; set; }
}
```

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/crowdsourcing.ts
// Agregar las siguientes interfaces al archivo existente

// --- Requests ---

export interface AceptarPropuestaRequest {
  tituloInterno: string;
  fechaInicio: string;             // ISO date string "YYYY-MM-DD"
  fechaFinPrevista?: string;       // ISO date string "YYYY-MM-DD"
}

export interface RechazarPropuestaRequest {
  motivo?: string;
}

export interface CreateMilestoneRequest {
  titulo: string;
  descripcion?: string;
  importeParcial: number;
  fechaLimite?: string;            // ISO date string "YYYY-MM-DD"
}

export interface CreateEntregableRequest {
  titulo: string;
  descripcion?: string;
  urlRecurso?: string;
  milestoneId?: string;
}

export interface AprobarEntregableRequest {
  comentario?: string;
}

export interface RechazarEntregableRequest {
  comentario: string;
}

export interface CancelarAcuerdoRequest {
  motivo: string;
}

// --- Results ---

export interface AceptarPropuestaResult {
  acuerdoId: string;
  tituloInterno: string;
  estadoAcuerdoNombre: string;
  importeTotalPactado: number;
  monedaNombre: string;
  conversacionId: string;
  propuestasRechazadas: number;
}

export interface RechazarPropuestaResult {
  id: string;
  estadoPropuestaNombre: string;
}

export interface MilestoneCreatedResult {
  id: string;
  titulo: string;
  orden: number;
  importeParcial: number;
  porcentajeParcial: number;
  importeAsignadoTotal: number;
}

export interface EntregableCreatedResult {
  id: string;
  titulo: string;
  estadoEntregableNombre: string;
  fechaCreacion: string;
}

export interface AprobarEntregableResult {
  id: string;
  estadoEntregableNombre: string;
  fechaAprobacion: string;
  todosAprobadosEnMilestone: boolean;
}

export interface RechazarEntregableResult {
  id: string;
  estadoEntregableNombre: string;
}

export interface CompletarAcuerdoResult {
  id: string;
  estadoAcuerdoNombre: string;
  fechaFinReal: string;
}

export interface CancelarAcuerdoResult {
  id: string;
  estadoAcuerdoNombre: string;
  fechaFinReal: string;
  necesidadEstadoNombre: string;
}

// --- Detail DTOs ---

export interface AcuerdoArtista {
  id: string;
  nombreArtistico: string;
}

export interface AcuerdoProfesional {
  userId: string;
  perfilProfesionalId: string;
  nombre: string;
}

export interface AcuerdoNecesidad {
  id: string;
  titulo: string;
}

export interface AcuerdoTimelineEvento {
  accion: string;
  fecha: string;
  actor: string;
}

export interface Entregable {
  id: string;
  titulo: string;
  descripcion?: string;
  urlRecurso?: string;
  estadoEntregableId: number;
  estadoEntregableNombre: string;
  comentarioAprobacion?: string;
  comentarioRechazo?: string;
  fechaAprobacion?: string;
  fechaCreacion: string;
}

export interface Milestone {
  id: string;
  titulo: string;
  descripcion?: string;
  orden: number;
  importeParcial: number;
  porcentajeParcial: number;
  fechaLimite?: string;
  fechaCompletado?: string;
  entregables: Entregable[];
}

export interface Acuerdo {
  id: string;
  tituloInterno: string;
  estadoAcuerdoId: number;
  estadoAcuerdoNombre: string;
  importeTotalPactado: number;
  monedaNombre: string;
  fechaInicio: string;
  fechaFinPrevista?: string;
  fechaFinReal?: string;
  artista: AcuerdoArtista;
  profesional: AcuerdoProfesional;
  necesidad: AcuerdoNecesidad;
  conversacionId?: string;
  milestones: Milestone[];
  importeAsignado: number;
  porcentajeAsignado: number;
  miRol: 'Artista' | 'Profesional';
  timeline: AcuerdoTimelineEvento[];
}

// --- Union Types ---

export type EstadoAcuerdo = 'Activo' | 'Completado' | 'Cancelado';
export type EstadoEntregable = 'Entregado' | 'Aprobado' | 'Rechazado';
export type RolAcuerdo = 'Artista' | 'Profesional';
```

---

## Validaciones Compartidas

| Campo | Regla | Backend (FluentValidation) | Frontend (Zod) |
|-------|-------|---------------------------|----------------|
| tituloInterno (aceptar propuesta) | requerido | `.NotEmpty().WithMessage("El titulo interno es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El titulo interno es obligatorio')` |
| tituloInterno (aceptar propuesta) | max 200 chars | `.MaximumLength(200).WithMessage("El titulo interno no puede superar los 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'El titulo no puede superar los 200 caracteres')` |
| fechaFinPrevista (aceptar propuesta) | posterior a fechaInicio si presente | `.Must((cmd, fecha) => ...).When(x => x.FechaFinPrevista.HasValue).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio").WithErrorCode(Validation_InvalidDate)` | `.refine(data => !data.fechaFinPrevista || data.fechaFinPrevista > data.fechaInicio, ...)` |
| motivo (rechazar propuesta) | max 500 si presente | `.MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Motivo)).WithMessage("El motivo no puede superar los 500 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(500, 'El motivo no puede superar los 500 caracteres').optional()` |
| titulo (milestone) | requerido | `.NotEmpty().WithMessage("El titulo es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El titulo es obligatorio')` |
| titulo (milestone) | min 3 chars | `.MinimumLength(3).WithMessage("El titulo debe tener al menos 3 caracteres").WithErrorCode(Validation_MinLength)` | `.min(3, 'El titulo debe tener al menos 3 caracteres')` |
| titulo (milestone) | max 200 chars | `.MaximumLength(200).WithMessage("El titulo no puede superar los 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'El titulo no puede superar los 200 caracteres')` |
| descripcion (milestone) | max 1000 chars si presente | `.MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Descripcion)).WithMessage("La descripcion no puede superar los 1000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(1000, 'La descripcion no puede superar los 1000 caracteres').optional()` |
| importeParcial (milestone) | requerido y > 0 | `.GreaterThan(0).WithMessage("El importe parcial debe ser mayor a 0").WithErrorCode(Validation_Required)` | `.positive('El importe parcial debe ser mayor a 0')` |
| fechaLimite (milestone) | >= fechaInicio del acuerdo si presente | `.Must(BeOnOrAfterFechaInicio).When(x => x.FechaLimite.HasValue).WithMessage("La fecha limite no puede ser anterior a la fecha de inicio del acuerdo").WithErrorCode(Validation_InvalidDate)` | `.refine(...)` (validacion en componente con contexto del acuerdo) |
| titulo (entregable) | requerido | `.NotEmpty().WithMessage("El titulo es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El titulo es obligatorio')` |
| titulo (entregable) | min 3 chars | `.MinimumLength(3).WithMessage("El titulo debe tener al menos 3 caracteres").WithErrorCode(Validation_MinLength)` | `.min(3, 'El titulo debe tener al menos 3 caracteres')` |
| titulo (entregable) | max 200 chars | `.MaximumLength(200).WithMessage("El titulo no puede superar los 200 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(200, 'El titulo no puede superar los 200 caracteres')` |
| descripcion (entregable) | max 1000 chars si presente | `.MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Descripcion)).WithMessage("La descripcion no puede superar los 1000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(1000, 'La descripcion no puede superar los 1000 caracteres').optional()` |
| urlRecurso (entregable) | URL valida si presente | `.Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.UrlRecurso)).WithMessage("Debe ser una URL valida").WithErrorCode(Validation_InvalidUrl)` | `.url('Debe ser una URL valida').optional()` |
| comentario (aprobar entregable) | max 500 si presente | `.MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Comentario)).WithMessage("El comentario no puede superar los 500 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(500, 'El comentario no puede superar los 500 caracteres').optional()` |
| comentario (rechazar entregable) | requerido | `.NotEmpty().WithMessage("El comentario es obligatorio al rechazar").WithErrorCode(Validation_Required)` | `.min(1, 'El comentario es obligatorio')` |
| comentario (rechazar entregable) | min 10 chars | `.MinimumLength(10).WithMessage("Minimo 10 caracteres explicando que debe corregirse").WithErrorCode(Validation_MinLength)` | `.min(10, 'Minimo 10 caracteres explicando que debe corregirse')` |
| comentario (rechazar entregable) | max 500 chars | `.MaximumLength(500).WithMessage("El comentario no puede superar los 500 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(500, 'El comentario no puede superar los 500 caracteres')` |
| motivo (cancelar acuerdo) | requerido | `.NotEmpty().WithMessage("El motivo es obligatorio").WithErrorCode(Validation_Required)` | `.min(1, 'El motivo es obligatorio')` |
| motivo (cancelar acuerdo) | min 20 chars | `.MinimumLength(20).WithMessage("Minimo 20 caracteres").WithErrorCode(Validation_MinLength)` | `.min(20, 'El motivo debe tener al menos 20 caracteres')` |
| motivo (cancelar acuerdo) | max 1000 chars | `.MaximumLength(1000).WithMessage("Maximo 1000 caracteres").WithErrorCode(Validation_MaxLength)` | `.max(1000, 'El motivo no puede superar los 1000 caracteres')` |

### Zod Schemas

```typescript
// Ruta: src/shared/schemas/crowdsourcing.schema.ts
// Agregar al archivo existente

import { z } from 'zod';

export const aceptarPropuestaSchema = z.object({
  tituloInterno: z
    .string()
    .min(1, 'El titulo interno es obligatorio')
    .max(200, 'El titulo no puede superar los 200 caracteres'),
  fechaInicio: z
    .string()
    .min(1, 'La fecha de inicio es obligatoria'),
  fechaFinPrevista: z
    .string()
    .optional(),
}).refine(
  (data) => {
    if (data.fechaFinPrevista) {
      return data.fechaFinPrevista > data.fechaInicio;
    }
    return true;
  },
  {
    message: 'La fecha de fin prevista debe ser posterior a la fecha de inicio',
    path: ['fechaFinPrevista'],
  }
);

export const rechazarPropuestaSchema = z.object({
  motivo: z
    .string()
    .max(500, 'El motivo no puede superar los 500 caracteres')
    .optional(),
});

export const createMilestoneSchema = z.object({
  titulo: z
    .string()
    .min(1, 'El titulo es obligatorio')
    .min(3, 'El titulo debe tener al menos 3 caracteres')
    .max(200, 'El titulo no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(1000, 'La descripcion no puede superar los 1000 caracteres')
    .optional(),
  importeParcial: z
    .number({
      required_error: 'El importe parcial es obligatorio',
      invalid_type_error: 'El importe parcial debe ser un numero',
    })
    .positive('El importe parcial debe ser mayor a 0'),
  fechaLimite: z
    .string()
    .optional(),
});

export const createEntregableSchema = z.object({
  titulo: z
    .string()
    .min(1, 'El titulo es obligatorio')
    .min(3, 'El titulo debe tener al menos 3 caracteres')
    .max(200, 'El titulo no puede superar los 200 caracteres'),
  descripcion: z
    .string()
    .max(1000, 'La descripcion no puede superar los 1000 caracteres')
    .optional(),
  urlRecurso: z
    .string()
    .url('Debe ser una URL valida')
    .optional()
    .or(z.literal('')),
  milestoneId: z
    .string()
    .uuid('ID de milestone invalido')
    .optional(),
});

export const aprobarEntregableSchema = z.object({
  comentario: z
    .string()
    .max(500, 'El comentario no puede superar los 500 caracteres')
    .optional(),
});

export const rechazarEntregableSchema = z.object({
  comentario: z
    .string()
    .min(1, 'El comentario es obligatorio al rechazar')
    .min(10, 'Minimo 10 caracteres explicando que debe corregirse')
    .max(500, 'El comentario no puede superar los 500 caracteres'),
});

export const cancelarAcuerdoSchema = z.object({
  motivo: z
    .string()
    .min(1, 'El motivo es obligatorio')
    .min(20, 'El motivo debe tener al menos 20 caracteres')
    .max(1000, 'El motivo no puede superar los 1000 caracteres'),
});

export type AceptarPropuestaFormData = z.infer<typeof aceptarPropuestaSchema>;
export type RechazarPropuestaFormData = z.infer<typeof rechazarPropuestaSchema>;
export type CreateMilestoneFormData = z.infer<typeof createMilestoneSchema>;
export type CreateEntregableFormData = z.infer<typeof createEntregableSchema>;
export type AprobarEntregableFormData = z.infer<typeof aprobarEntregableSchema>;
export type RechazarEntregableFormData = z.infer<typeof rechazarEntregableSchema>;
export type CancelarAcuerdoFormData = z.infer<typeof cancelarAcuerdoSchema>;
```

---

## Constantes Compartidas

### Adiciones a src/shared/constants/index.ts

```typescript
// Estados del Acuerdo (int IDs desde maestras)
export const ESTADO_ACUERDO = {
  ACTIVO: 1,
  COMPLETADO: 2,
  CANCELADO: 3,
} as const;

export const ESTADO_ACUERDO_LABELS: Record<number, string> = {
  1: 'Activo',
  2: 'Completado',
  3: 'Cancelado',
};

export const ESTADO_ACUERDO_BADGES: Record<number, string> = {
  1: 'blue',       // Activo - azul
  2: 'green',      // Completado - verde
  3: 'gray',       // Cancelado - gris
};

// Estados del Entregable (int IDs desde maestras)
export const ESTADO_ENTREGABLE = {
  ENTREGADO: 1,
  APROBADO: 2,
  RECHAZADO: 3,
} as const;

export const ESTADO_ENTREGABLE_LABELS: Record<number, string> = {
  1: 'Entregado',
  2: 'Aprobado',
  3: 'Rechazado',
};

export const ESTADO_ENTREGABLE_BADGES: Record<number, string> = {
  1: 'yellow',     // Entregado - amarillo (pendiente de revision)
  2: 'green',      // Aprobado - verde
  3: 'red',        // Rechazado - rojo
};

// Query Keys additions
// Agregar dentro de QUERY_KEYS.crowdsourcing:
//
// acuerdos: {
//   byId: (id: string) => ['crowdsourcing', 'acuerdos', id] as const,
// },
// Nota: milestones y entregables se cargan anidados en el detalle del acuerdo,
// no tienen query key propio. Invalidar 'acuerdos.byId(acuerdoId)' al mutar.
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
  },
};

// API Routes additions
// Agregar dentro de API_ROUTES.crowdsourcing:
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
  },
};

// App Routes additions (Landing)
// Agregar dentro de APP_ROUTES:
export const APP_ROUTES = {
  landing: {
    crowdsourcing: {
      necesidades: '/crowdsourcing/necesidades',
      necesidadDetail: (id: string) => `/crowdsourcing/necesidades/${id}`,
      misPropuestas: '/crowdsourcing/mis-propuestas',
      acuerdoDetail: (id: string) => `/crowdsourcing/acuerdos/${id}`,
    },
  },
};
```

---

## Mapeo de Errores a UI

### Adiciones a src/shared/utils/error-messages.ts

```typescript
export const ERROR_MESSAGES: Record<string, string> = {
  // ... mensajes existentes de US-CS-03

  // Crowdsourcing - Acuerdos y Entregables
  '2011': 'El acuerdo no fue encontrado',
  '2012': 'El milestone no fue encontrado',
  '2013': 'El entregable no fue encontrado',
  '4007': 'La propuesta no esta disponible para esta accion',
  '4008': 'Ya existe un acuerdo activo para esta necesidad',
  '4009': 'La suma de importes de los milestones supera el total pactado',
  '4010': 'Esta accion solo esta disponible cuando el acuerdo esta activo',
  '4011': 'No se puede modificar un milestone que ya fue completado',
  '4012': 'No se puede eliminar un milestone que tiene entregables asociados',
  '4013': 'Solo se pueden revisar entregables en estado Entregado',

  // Keys semanticos para uso interno
  ACUERDO_NOT_FOUND: 'El acuerdo no fue encontrado',
  MILESTONE_NOT_FOUND: 'El milestone no fue encontrado',
  ENTREGABLE_NOT_FOUND: 'El entregable no fue encontrado',
  PROPUESTA_NOT_ACCEPTABLE: 'La propuesta no esta en estado Pendiente',
  ACUERDO_ALREADY_EXISTS: 'Ya existe un acuerdo activo para esta necesidad',
  MILESTONE_IMPORTE_EXCEEDED: 'La suma de importes supera el total pactado del acuerdo',
  ACUERDO_NOT_ACTIVE: 'Esta accion solo esta disponible para acuerdos activos',
  MILESTONE_COMPLETED: 'El milestone ya fue completado y no puede modificarse',
  MILESTONE_HAS_ENTREGABLES: 'El milestone tiene entregables asociados y no puede eliminarse',
  ENTREGABLE_NOT_REVIEWABLE: 'El entregable no esta en estado Entregado',
  VALIDATION_COMENTARIO_RECHAZO_MIN: 'El comentario de rechazo debe tener al menos 10 caracteres',
  VALIDATION_MOTIVO_CANCELACION_MIN: 'El motivo de cancelacion debe tener al menos 20 caracteres',
};
```

---

## Nuevas Constantes ServiceResponseMessageType

Agregar al archivo `Modules/Crowdsourcing/Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`:

```csharp
// Validation (1000-1999)
public const string Validation_InvalidUrl = "1013";      // URL invalida en UrlRecurso

// NotFound (2000-2999)
public const string NotFound_Acuerdo = "2011";
public const string NotFound_Milestone = "2012";
public const string NotFound_Entregable = "2013";

// Business Rules (4000-4999)
public const string BusinessRule_PropuestaNotAcceptable = "4007";    // Propuesta no esta en Pendiente
public const string BusinessRule_AcuerdoAlreadyExists = "4008";      // Ya existe acuerdo activo para la necesidad
public const string BusinessRule_MilestoneImporteExceeded = "4009";  // Suma milestones > ImporteTotalPactado
public const string BusinessRule_AcuerdoNotActive = "4010";          // Acuerdo no esta en estado Activo
public const string BusinessRule_MilestoneCompleted = "4011";        // Milestone ya completado
public const string BusinessRule_MilestoneHasEntregables = "4012";   // Milestone tiene entregables
public const string BusinessRule_EntregableNotReviewable = "4013";   // Entregable no esta en Entregado
```

---

## Notas de Implementacion

### Backend

1. **AceptarPropuestaCommand (operacion transaccional):**
   - Obtener `UserId` del token; resolver `ArtistaId` por `UserId`
   - Validar que la propuesta existe y es del artista (a traves de la necesidad)
   - Validar que la propuesta esta en estado `Pendiente` (`BusinessRule_PropuestaNotAcceptable`)
   - Validar que no existe un `AcuerdoCrowdsourcing` con `EstadoAcuerdoId == Activo` para la misma `NecesidadId` (`BusinessRule_AcuerdoAlreadyExists`)
   - Dentro de una transaccion de base de datos:
     1. Crear `AcuerdoCrowdsourcing` con `EstadoAcuerdoId = Activo`
     2. Actualizar propuesta aceptada: `EstadoPropuestaId = Aceptada`
     3. Actualizar otras propuestas pendientes de la misma necesidad: `EstadoPropuestaId = Rechazada`, motivo = "Otra propuesta fue aceptada"
     4. Actualizar necesidad: `EstadoNecesidadId = En Progreso`
     5. Crear `ConversacionCrowdsourcing` vinculada al acuerdo
   - Retornar `AceptarPropuestaResultDto` con `propuestasRechazadas` = count de propuestas rechazadas automaticamente

2. **CreateMilestoneCommand:**
   - Obtener `UserId` del token; resolver `ArtistaId`
   - Cargar acuerdo; validar que el `ArtistaId` coincide (`Auth_Forbidden`)
   - Validar que el acuerdo esta en estado `Activo` (`BusinessRule_AcuerdoNotActive`)
   - Calcular suma actual: `SUM(ImporteParcial) de milestones existentes`
   - Validar que `sumaActual + importeParcial <= ImporteTotalPactado` (`BusinessRule_MilestoneImporteExceeded`)
   - Asignar `Orden = MAX(Orden) + 1` o `1` si es el primero
   - Retornar `importeAsignadoTotal` = nueva suma tras la creacion

3. **UpdateMilestoneCommand:**
   - Validar mismo acuerdo, mismo artista, acuerdo activo
   - Validar que `FechaCompletado IS NULL` (`BusinessRule_MilestoneCompleted`)
   - Calcular suma excluyendo el milestone actual: `SUM(ImporteParcial) - importeActual + nuevoImporte <= ImporteTotalPactado`

4. **DeleteMilestoneCommand:**
   - Validar mismo acuerdo, mismo artista, acuerdo activo
   - Validar que `FechaCompletado IS NULL` (`BusinessRule_MilestoneCompleted`)
   - Validar que no existen `AcuerdoCrowdsourcingEntregable` con ese `MilestoneId` (`BusinessRule_MilestoneHasEntregables`)

5. **CreateEntregableCommand:**
   - Obtener `UserId` del token; verificar que coincide con `AcuerdoCrowdsourcing.UserIdProveedor`
   - Validar que el acuerdo esta `Activo` (`BusinessRule_AcuerdoNotActive`)
   - Si se envia `MilestoneId`, validar que existe y pertenece al mismo acuerdo (`Validation_ForeignKeyNotFound`)
   - Crear con `EstadoEntregableId = Entregado`

6. **AprobarEntregableCommand:**
   - Resolver `ArtistaId` del `UserId` del token; validar contra `AcuerdoCrowdsourcing.ArtistaId`
   - Validar que el entregable esta en estado `Entregado` (`BusinessRule_EntregableNotReviewable`)
   - Registrar `FechaAprobacion = DateTime.UtcNow` y `ComentarioAprobacion`
   - Calcular `TodosAprobadosEnMilestone`: si el entregable tiene `MilestoneId`, verificar que todos los entregables de ese milestone tengan `EstadoEntregableId == Aprobado`

7. **CompletarAcuerdoCommand:**
   - Validar artista y acuerdo activo
   - Actualizar `EstadoAcuerdoId = Completado`, `FechaFinReal = DateTime.UtcNow`
   - Actualizar necesidad: `EstadoNecesidadId = Cerrada`

8. **CancelarAcuerdoCommand:**
   - Validar que `UserId` del token es artista o profesional del acuerdo
   - Actualizar `EstadoAcuerdoId = Cancelado`, `FechaFinReal = DateTime.UtcNow`
   - Guardar `MotivoCancelacion` y `CanceladoPor = UserId del solicitante`
   - Actualizar necesidad: `EstadoNecesidadId = Abierta`

9. **GetAcuerdoByIdQuery:**
   - Validar que `UserId` del token es artista o profesional del acuerdo; si no, `Auth_Forbidden`
   - Cargar acuerdo con `Include(Milestones).ThenInclude(Entregables)`
   - Calcular `ImporteAsignado`, `PorcentajeAsignado` y `PorcentajeParcial` por milestone en la proyeccion
   - Determinar `MiRol` comparando `UserId` del token con `ArtistaId` resuelto y `UserIdProveedor`
   - Construir `Timeline` con los ultimos 20 eventos; el timeline puede implementarse como una lista de logs en base de datos o calcularse desde las fechas de creacion y actualizacion de entidades relacionadas
   - Usar `AsNoTracking()`

10. **Caching:** Usar `IRequestCacheService` para la resolucion de `ArtistaId` por `UserId` y la carga del `AcuerdoCrowdsourcing` (compartida entre Validator y Handler en cada request).

### Frontend

1. **Nuevos archivos recomendados:**
   - `src/web/src/features/crowdsourcing/acuerdos/` - Feature de detalle de acuerdo (Landing)
   - `src/shared/schemas/crowdsourcing.schema.ts` - Agregar todos los nuevos schemas
   - `src/shared/types/crowdsourcing.ts` - Agregar los nuevos tipos

2. **Componentes clave (Landing):**
   - `AcuerdoDetailPage.tsx` - Vista principal del acuerdo con todas las secciones
   - `AcuerdoHeader.tsx` - Cabecera con estado, partes, importe y fechas
   - `MilestonesSection.tsx` - Listado de milestones con barra de progreso visual
   - `MilestoneCard.tsx` - Card de milestone con sus entregables anidados
   - `MilestoneFormDialog.tsx` - Dialogo para crear/editar milestone (React Hook Form + Zod)
   - `EntregablesSection.tsx` - Entregables sin milestone agrupados aparte
   - `EntregableItem.tsx` - Item de entregable con acciones de aprobar/rechazar segun `miRol`
   - `SubirEntregableDialog.tsx` - Dialogo para subir entregable (profesional)
   - `AprobarEntregableDialog.tsx` - Dialogo de confirmacion con comentario opcional (artista)
   - `RechazarEntregableDialog.tsx` - Dialogo con comentario obligatorio (artista)
   - `CompletarAcuerdoDialog.tsx` - Dialogo de confirmacion con resumen y aviso de entregables pendientes
   - `CancelarAcuerdoDialog.tsx` - Dialogo con advertencia de irreversibilidad y campo de motivo
   - `AcuerdoTimeline.tsx` - Lista de eventos cronologicos del acuerdo
   - `ImporteAsignadoBar.tsx` - Barra de progreso con texto "Asignado: X de Y EUR (Z%)"

3. **Hooks personalizados:**
   - `useAcuerdo(id)` - Query con `QUERY_KEYS.crowdsourcing.acuerdos.byId(id)`
   - `useAceptarPropuesta(propuestaId)` - Mutation; invalida `QUERY_KEYS.crowdsourcing.propuestas.mis` y navega al detalle del acuerdo
   - `useRechazarPropuesta(propuestaId)` - Mutation; invalida query de propuestas de la necesidad
   - `useCreateMilestone(acuerdoId)` - Mutation; invalida `QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId)`
   - `useUpdateMilestone(acuerdoId, milestoneId)` - Mutation; invalida el acuerdo
   - `useDeleteMilestone(acuerdoId, milestoneId)` - Mutation; invalida el acuerdo
   - `useCreateEntregable(acuerdoId)` - Mutation; invalida el acuerdo
   - `useAprobarEntregable(entregableId, acuerdoId)` - Mutation; invalida el acuerdo; si `todosAprobadosEnMilestone == true`, mostrar sugerencia de completar milestone
   - `useRechazarEntregable(entregableId, acuerdoId)` - Mutation; invalida el acuerdo
   - `useCompletarAcuerdo(id)` - Mutation; invalida el acuerdo y navega al detalle actualizado
   - `useCancelarAcuerdo(id)` - Mutation; invalida el acuerdo

4. **UX considerations:**
   - La barra de progreso de importes asignados se actualiza en tiempo real en el formulario de milestone usando el valor de `importeAsignadoTotal` del servidor (invalidar query tras cada mutacion)
   - Mostrar acciones condicionalmente segun `miRol` y `estadoAcuerdoNombre`:
     - `miRol == 'Artista'`: mostrar botones de milestone, aprobar/rechazar entregable, completar, cancelar
     - `miRol == 'Profesional'`: mostrar boton de subir entregable, cancelar
   - Al completar acuerdo: si hay entregables con `estadoEntregableNombre == 'Entregado'`, mostrar aviso antes de la confirmacion
   - Dialog de cancelacion: incluir texto de advertencia "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas."
   - Toast tras cada accion exitosa con el mensaje del campo `messages[0].message` de la respuesta
   - Redirigir al detalle del acuerdo tras aceptar propuesta usando `acuerdoId` del response

---

## Checklist de Contratos

- [x] Endpoints con request/response/errores (11 endpoints documentados)
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos (AceptarPropuestaRequestDto, AceptarPropuestaResultDto, RechazarPropuestaRequestDto, RechazarPropuestaResultDto, AcuerdoDto, AcuerdoArtistaDto, AcuerdoProfesionalDto, AcuerdoNecesidadDto, AcuerdoTimelineEventoDto, MilestoneDto, EntregableDto, CreateMilestoneRequestDto, MilestoneCreatedResultDto, CreateEntregableRequestDto, EntregableCreatedResultDto, AprobarEntregableRequestDto, AprobarEntregableResultDto, RechazarEntregableRequestDto, RechazarEntregableResultDto, CancelarAcuerdoRequestDto, CancelarAcuerdoResultDto, CompletarAcuerdoResultDto)
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas (aceptarPropuestaSchema, rechazarPropuestaSchema, createMilestoneSchema, createEntregableSchema, aprobarEntregableSchema, rechazarEntregableSchema, cancelarAcuerdoSchema)
- [x] Constantes compartidas (ESTADO_ACUERDO, ESTADO_ENTREGABLE, QUERY_KEYS additions, API_ROUTES additions, APP_ROUTES additions)
- [x] Nuevas constantes ServiceResponseMessageType (1013, 2011, 2012, 2013, 4007, 4008, 4009, 4010, 4011, 4012, 4013)
- [x] Mapeo de errores a mensajes UI
- [x] Notas de implementacion detalladas (Backend + Frontend)
- [x] Operacion transaccional de aceptar propuesta documentada con todos sus efectos colaterales
- [x] Campos calculados del detalle del acuerdo documentados (importeAsignado, porcentajeAsignado, miRol, timeline)
