# Contratos: Ejecucion de Tareas de Promocion y Validacion

> **Feature:** cp-tareas-promocion (US-CP-04)
> **Ultima actualizacion:** 2026-03-01
> **Modulo Backend:** Crowdpromotion
> **Depende de:** US-CP-03 (cp-inscripcion-programa)

Este documento define los contratos entre proyectos. **Cualquier cambio aqui debe reflejarse en todos los proyectos afectados.**

---

## Analisis del Modelo de Dominio

### PromoTareaPromotor - Modelo de Completados por Registro

La entidad `PromoTareaPromotor` representa **un registro por intento de completado**, no un registro por par tarea-promotor. El dominio actual tiene:

```
PromoTareaPromotor
  Id                   Guid
  TareaId              Guid
  ProgramaPromotorId   Guid          (FK a PromoProgramaPromotor)
  EstadoTareaId        int           (FK a Maestra_EstadoTareaPromo)
  UrlPruebaCompletado  string?
  ComentarioPromotor   string?
  ComentarioValidacion string?
  FechaCompletado      DateTime?
  FechaValidado        DateTime?
  FechaCreacion        DateTime
```

### Campos VecesCompletada, FechaPrimeraCompletada, FechaUltimaCompletada

**Estos campos NO existen como columnas en la entidad.** La US los menciona en los mockups, pero la implementacion los resuelve como agregados en la capa de consulta:

| Campo de la US | Implementacion real |
|----------------|---------------------|
| `VecesCompletada` | `COUNT(*)` de registros `PromoTareaPromotor` para `TareaId + ProgramaPromotorId` donde `EstadoTareaId IN (2, 3, 4)` |
| `FechaPrimeraCompletada` | `MIN(FechaCompletado)` del mismo agrupamiento |
| `FechaUltimaCompletada` | `MAX(FechaCompletado)` del mismo agrupamiento |

**Consecuencias para el endpoint GET mis-tareas:**
- La consulta agrupa por `TareaId + ProgramaPromotorId`
- Para el campo `miEstado`, se toma el registro mas reciente (`MAX(FechaCreacion)`)
- `vecesCompletada`, `fechaPrimeraCompletada`, `fechaUltimaCompletada` se calculan via SQL o LINQ

**Consecuencias para el endpoint POST completar:**
- Para tareas no repetibles: verificar si existe algun registro con `EstadoTareaId != 4` (Rechazada)
- Para tareas rechazadas: se actualiza el registro existente mas reciente (estado vuelve a 2=Completada, se actualiza URL)
- Para tareas repetibles: se crea un nuevo `PromoTareaPromotor` por cada ejecucion adicional

### Maquina de Estados de PromoTareaPromotor.EstadoTareaId

```
1 = Pendiente    (asignada, no completada aun - estado inicial conceptual, no se crea registro)
2 = Completada   (promotor envio URL de prueba, pendiente de validacion del artista)
3 = Validada     (artista aprueba, recompensa acreditada en wallet)
4 = Rechazada    (artista rechaza, promotor puede re-enviar)
```

**Transiciones validas:**

```
[Sin registro]    --POST completar-->    Completada (2)  [nuevo registro]
Completada (2)    --PATCH validar-->     Validada (3)
Completada (2)    --PATCH rechazar-->    Rechazada (4)
Rechazada (4)     --POST completar-->    Completada (2)  [actualiza registro existente]
Validada (3)      [si EsRepetible] --POST completar--> Completada (2) [nuevo registro]
```

### Efecto en PromotorWallet al Validar

Al validar una tarea con recompensa monetaria:
1. Buscar o crear `PromotorWallet` del promotor para la moneda de la tarea (`Tarea.MonedaId`)
2. Crear `PromotorWalletTransaccion` con `TipoRewardId` de la tarea, `Importe = Tarea.ImporteRecompensa`
3. Actualizar `PromotorWallet.SaldoPendiente += importe` y `TotalGanado += importe`

La acreditacion es transaccional: falla completa si alguno de los pasos falla.

---

## Endpoints API

### GET /api/crowdpromotion/programas/{programaId}/mis-tareas

**Descripcion:** Lista todas las tareas activas del programa con el estado de completado del promotor autenticado. Incluye campos calculados agregados (`vecesCompletada`, `fechaPrimeraCompletada`, `fechaUltimaCompletada`) resueltos en la capa de consulta. Solo retorna tareas con `EsActivo = true`. El `PromotorId` y `ProgramaPromotorId` se resuelven desde el token JWT via `UserId`.

**Autorizacion:** Bearer JWT - Promotor aprobado en el programa (`PromoProgramaPromotor.EsAprobado = true`, `EsBloqueado = false`)

**Path Params:** `programaId` (Guid) - Identificador del programa de promocion

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "programaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "programaTitulo": "Promociona mi nuevo album",
    "items": [
      {
        "tareaId": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
        "nombre": "Comparte en Instagram Stories",
        "descripcion": "Sube una story mencionando la campana y etiquetando @weplay_rises",
        "instruccionesUrl": "https://docs.example.com/instrucciones-ig",
        "tipoEventoPromoNombre": "Share",
        "tipoRewardNombre": "Dinero",
        "importeRecompensa": 5.00,
        "monedaNombre": "EUR",
        "puntosRecompensa": null,
        "esRepetible": true,
        "maxRepeticiones": 10,
        "orden": 1,
        "miEstado": {
          "tareaPromotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
          "estadoTareaId": 3,
          "estadoTareaNombre": "Validada",
          "vecesCompletada": 3,
          "fechaPrimeraCompletada": "2026-03-10T10:00:00Z",
          "fechaUltimaCompletada": "2026-03-20T15:30:00Z",
          "urlPruebaCompletado": "https://instagram.com/stories/xxx",
          "comentarioValidacion": null
        }
      },
      {
        "tareaId": "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
        "nombre": "Publica un TikTok",
        "descripcion": "Crea un TikTok sobre la campana con al menos 30 segundos",
        "instruccionesUrl": null,
        "tipoEventoPromoNombre": "Post",
        "tipoRewardNombre": "Dinero",
        "importeRecompensa": 10.00,
        "monedaNombre": "EUR",
        "puntosRecompensa": null,
        "esRepetible": false,
        "maxRepeticiones": null,
        "orden": 2,
        "miEstado": null
      },
      {
        "tareaId": "d4e5f6a7-b8c9-0123-de45-fa6789012345",
        "nombre": "Escribe una resena en tu blog",
        "descripcion": "Redacta una resena de al menos 300 palabras",
        "instruccionesUrl": null,
        "tipoEventoPromoNombre": "Post",
        "tipoRewardNombre": "Dinero",
        "importeRecompensa": 15.00,
        "monedaNombre": "EUR",
        "puntosRecompensa": null,
        "esRepetible": false,
        "maxRepeticiones": null,
        "orden": 3,
        "miEstado": {
          "tareaPromotorId": "e5f6a7b8-c9d0-1234-ef56-789012345678",
          "estadoTareaId": 4,
          "estadoTareaNombre": "Rechazada",
          "vecesCompletada": 1,
          "fechaPrimeraCompletada": "2026-03-18T09:00:00Z",
          "fechaUltimaCompletada": "2026-03-18T09:00:00Z",
          "urlPruebaCompletado": "https://myblog.com/resena-weplay",
          "comentarioValidacion": "La resena no menciona la campana correctamente"
        }
      }
    ]
  },
  "messages": []
}
```

**Notas sobre campos calculados:**
- `vecesCompletada`: COUNT de registros `PromoTareaPromotor` para el par `TareaId + ProgramaPromotorId`. Solo se cuenta si hay al menos un registro (0 si `miEstado` es null).
- `fechaPrimeraCompletada`: MIN(`FechaCompletado`) del mismo agrupamiento. Null si `miEstado` es null.
- `fechaUltimaCompletada`: MAX(`FechaCompletado`) del mismo agrupamiento. Null si `miEstado` es null.
- `miEstado`: Null si el promotor nunca ha completado esta tarea. Cuando hay multiples registros (tarea repetible), el `tareaPromotorId` y `estadoTareaNombre` corresponden al registro mas reciente por `FechaCreacion DESC`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para acceder a este programa | El promotor no esta aprobado o esta bloqueado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 500 | 5000 | Error inesperado al obtener las tareas | Excepcion no controlada |

---

### POST /api/crowdpromotion/programas/{programaId}/tareas/{tareaId}/completar

**Descripcion:** El promotor autenticado marca una tarea como completada enviando una URL de prueba y un comentario opcional. El comportamiento varia segun el estado actual: si la tarea no tiene registros previos o todos estan validados (y es repetible), crea un nuevo `PromoTareaPromotor`; si el ultimo registro esta rechazado, lo actualiza con la nueva URL y vuelve al estado Completada (2). El `ProgramaPromotorId` se resuelve desde el token JWT.

**Autorizacion:** Bearer JWT - Promotor aprobado en el programa (`PromoProgramaPromotor.EsAprobado = true`, `EsBloqueado = false`)

**Path Params:**
- `programaId` (Guid) - Identificador del programa de promocion
- `tareaId` (Guid) - Identificador de la tarea a completar

**Request Body:**
```json
{
  "urlPruebaCompletado": "https://instagram.com/stories/my-promo-story-abc123",
  "comentarioPromotor": "Story publicada con mencion a la campana y tag @weplay_rises"
}
```

| Campo | Tipo | Obligatorio | Constraints |
|-------|------|-------------|-------------|
| `urlPruebaCompletado` | string | Si | URL valida, max 2048 caracteres |
| `comentarioPromotor` | string | No | max 500 caracteres |

**Response 200 OK:**
```json
{
  "data": {
    "tareaPromotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "estadoTareaId": 2,
    "estadoTareaNombre": "Completada",
    "vecesCompletada": 4,
    "fechaUltimaCompletada": "2026-03-20T15:30:00Z"
  },
  "messages": [
    { "message": "Tarea enviada para validacion", "errorCode": "0002" }
  ]
}
```

**Logica de negocio al completar:**
- Si no existe registro previo o todos los registros previos son de ejecuciones anteriores validadas (y la tarea es repetible): crear nuevo `PromoTareaPromotor` con `EstadoTareaId = 2`
- Si el registro mas reciente tiene `EstadoTareaId = 4` (Rechazada): actualizar ese registro con nueva `UrlPruebaCompletado`, `ComentarioPromotor`, `FechaCompletado = now()`, `EstadoTareaId = 2`, limpiar `ComentarioValidacion` y `FechaValidado`
- `vecesCompletada` en la respuesta: COUNT total de registros para ese par `TareaId + ProgramaPromotorId`

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | La URL de prueba es obligatoria | `UrlPruebaCompletado` vacio o null |
| 400 | 1013 | La URL de prueba no tiene formato valido | `UrlPruebaCompletado` no es una URL valida |
| 400 | 1002 | El comentario no puede superar los 500 caracteres | `ComentarioPromotor.Length > 500` |
| 400 | 4027 | Esta tarea ya fue completada y no es repetible | Tarea con `EsRepetible = false` ya tiene registro con `EstadoTareaId IN (2, 3)` |
| 400 | 4028 | Has alcanzado el maximo de repeticiones permitidas | Tarea repetible con `vecesCompletada >= MaxRepeticiones` |
| 400 | 4024 | Este programa no esta activo | `PromoPrograma.EsActivo = false` |
| 400 | 4029 | Esta tarea no esta activa | `PromoTarea.EsActivo = false` |
| 400 | 4030 | Esta tarea ya no acepta completados (fecha fin pasada) | `PromoTarea.FechaFin != null && PromoTarea.FechaFin < now()` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para acceder a este programa | Promotor no aprobado o bloqueado |
| 404 | 2015 | No tienes un perfil de promotor | No existe `Promotor` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2021 | La tarea no existe | No existe `PromoTarea` con el `tareaId` proporcionado |
| 500 | 5000 | Error inesperado al completar la tarea | Excepcion no controlada |

---

### GET /api/crowdpromotion/programas/{programaId}/tareas-pendientes

**Descripcion:** Lista paginada de completados de tareas pendientes de validacion (estado Completada = 2) para el programa indicado. Solo accesible por el artista propietario del programa. El `ArtistaId` se resuelve desde el token JWT y se verifica contra `PromoPrograma.ArtistaId`. Los resultados muestran el ultimo completado de cada promotor por tarea.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:** `programaId` (Guid) - Identificador del programa de promocion

**Query Params:**

| Parametro | Tipo | Obligatorio | Descripcion |
|-----------|------|-------------|-------------|
| `page` | int | No | Numero de pagina. Default: 1 |
| `pageSize` | int | No | Elementos por pagina. Default: 10. Max: 50 |

**Request Body:** Ninguno

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "tareaPromotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
        "tareaId": "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
        "tareaNombre": "Comparte en Instagram Stories",
        "promotorId": "f6a7b8c9-d0e1-2345-fa67-890123456789",
        "promotorNombre": "DJ Marketing Pro",
        "promotorTipoNombre": "Influencer",
        "urlPruebaCompletado": "https://instagram.com/stories/my-promo-story-abc123",
        "comentarioPromotor": "Story publicada con mencion a la campana",
        "vecesCompletada": 4,
        "fechaUltimaCompletada": "2026-03-20T15:30:00Z"
      },
      {
        "tareaPromotorId": "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
        "tareaId": "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
        "tareaNombre": "Publica un TikTok",
        "promotorId": "a7b8c9d0-e1f2-3456-ab78-901234567890",
        "promotorNombre": "BeatsPromoter",
        "promotorTipoNombre": "Fan",
        "urlPruebaCompletado": "https://tiktok.com/@beatspromoter/video/123456",
        "comentarioPromotor": null,
        "vecesCompletada": 1,
        "fechaUltimaCompletada": "2026-03-19T18:00:00Z"
      }
    ],
    "totalCount": 8,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1
  },
  "messages": []
}
```

**Notas:**
- Solo se retornan registros con `EstadoTareaId = 2` (Completada).
- `promotorNombre`: resuelto desde `Promotor.NombrePublico`.
- `promotorTipoNombre`: resuelto desde `MaestraTipoPromotor` via `Promotor.TipoPromotorId`.
- `vecesCompletada`: COUNT total de registros `PromoTareaPromotor` para el par `TareaId + ProgramaPromotorId`.
- `fechaUltimaCompletada`: `FechaCompletado` del registro actual con `EstadoTareaId = 2`.

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 500 | 5000 | Error inesperado al obtener las tareas pendientes | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/validar

**Descripcion:** El artista propietario del programa valida un completado de tarea. Cambia `EstadoTareaId` a 3 (Validada), establece `FechaValidado = now()`, guarda el `ComentarioValidacion` y acredita la recompensa en la wallet del promotor de forma transaccional. Si la tarea tiene `ImporteRecompensa`, crea un `PromotorWalletTransaccion` y actualiza `PromotorWallet`. El `ArtistaId` se verifica contra `PromoPrograma.ArtistaId`.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa de promocion
- `tareaPromotorId` (Guid) - Identificador del completado (`PromoTareaPromotor.Id`)

**Request Body:**
```json
{
  "comentarioValidacion": "Verificado, story publicada correctamente con todos los requisitos"
}
```

| Campo | Tipo | Obligatorio | Constraints |
|-------|------|-------------|-------------|
| `comentarioValidacion` | string | No | max 500 caracteres |

**Response 200 OK:**
```json
{
  "data": {
    "tareaPromotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "estadoTareaId": 3,
    "estadoTareaNombre": "Validada",
    "recompensaAcreditada": 5.00,
    "monedaNombre": "EUR",
    "puntosAcreditados": null
  },
  "messages": [
    { "message": "Tarea validada y recompensa acreditada", "errorCode": "0002" }
  ]
}
```

**Nota sobre recompensaAcreditada:** Si la tarea no tiene recompensa monetaria configurada (`Tarea.ImporteRecompensa` es null), el campo `recompensaAcreditada` sera null y `monedaNombre` sera null. Si tiene `PuntosRecompensa`, el campo `puntosAcreditados` sera el valor correspondiente.

**Logica transaccional al validar:**
1. Verificar que el `PromoTareaPromotor.EstadoTareaId == 2` (Completada)
2. Actualizar `PromoTareaPromotor`: `EstadoTareaId = 3`, `FechaValidado = now()`, `ComentarioValidacion`
3. Si `Tarea.ImporteRecompensa != null`: buscar o crear `PromotorWallet` para `Promotor.Id` y `Tarea.MonedaId`
4. Crear `PromotorWalletTransaccion` con `TipoRewardId = Tarea.TipoRewardId`, `Importe = Tarea.ImporteRecompensa`, `EstadoTransaccionId = 1` (pendiente de liquidacion), `Concepto = "Recompensa tarea: {Tarea.Titulo}"`
5. Actualizar `PromotorWallet.SaldoPendiente += importe` y `TotalGanado += importe`
6. Todo en una misma transaccion de base de datos

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1002 | El comentario no puede superar los 500 caracteres | `ComentarioValidacion.Length > 500` |
| 400 | 4031 | Este completado no esta en estado Completada | `PromoTareaPromotor.EstadoTareaId != 2` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2022 | El completado no existe | No existe `PromoTareaPromotor` con el `tareaPromotorId` proporcionado |
| 500 | 5000 | Error inesperado al validar la tarea | Excepcion no controlada |

---

### PATCH /api/crowdpromotion/programas/{programaId}/tareas-promotor/{tareaPromotorId}/rechazar

**Descripcion:** El artista propietario del programa rechaza un completado de tarea. Cambia `EstadoTareaId` a 4 (Rechazada), establece `FechaValidado = now()` y guarda el `ComentarioValidacion` obligatorio. El promotor podra re-enviar la tarea con nueva URL de prueba tras el rechazo. El `ArtistaId` se verifica contra `PromoPrograma.ArtistaId`.

**Autorizacion:** Bearer JWT - Artista propietario del programa

**Path Params:**
- `programaId` (Guid) - Identificador del programa de promocion
- `tareaPromotorId` (Guid) - Identificador del completado (`PromoTareaPromotor.Id`)

**Request Body:**
```json
{
  "comentarioValidacion": "La story no menciona la campana correctamente, no se ve el tag @weplay_rises"
}
```

| Campo | Tipo | Obligatorio | Constraints |
|-------|------|-------------|-------------|
| `comentarioValidacion` | string | Si | min 1, max 500 caracteres |

**Response 200 OK:**
```json
{
  "data": {
    "tareaPromotorId": "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    "estadoTareaId": 4,
    "estadoTareaNombre": "Rechazada"
  },
  "messages": [
    { "message": "Tarea rechazada", "errorCode": "0002" }
  ]
}
```

**Errores:**
| HTTP | ErrorCode | Mensaje | Causa |
|------|-----------|---------|-------|
| 400 | 1001 | El motivo de rechazo es obligatorio | `ComentarioValidacion` vacio o null |
| 400 | 1002 | El motivo de rechazo no puede superar los 500 caracteres | `ComentarioValidacion.Length > 500` |
| 400 | 4031 | Este completado no esta en estado Completada | `PromoTareaPromotor.EstadoTareaId != 2` |
| 401 | 3001 | Token no valido o expirado | Token JWT invalido o expirado |
| 403 | 4026 | No tienes permiso para gestionar este programa | El `ArtistaId` del token no coincide con `PromoPrograma.ArtistaId` |
| 404 | 2016 | No tienes un perfil de artista | No existe `Artista` con el `UserId` del token |
| 404 | 2019 | El programa de promocion no existe | No existe `PromoPrograma` con el `programaId` proporcionado |
| 404 | 2022 | El completado no existe | No existe `PromoTareaPromotor` con el `tareaPromotorId` proporcionado |
| 500 | 5000 | Error inesperado al rechazar la tarea | Excepcion no controlada |

---

## Autorizacion

### Resumen por Endpoint

| Endpoint | Auth | Actor | Verificacion |
|----------|------|-------|--------------|
| `GET /programas/{id}/mis-tareas` | Bearer JWT | Promotor | `PromoProgramaPromotor.EsAprobado = true && EsBloqueado = false` |
| `POST /programas/{id}/tareas/{id}/completar` | Bearer JWT | Promotor | `PromoProgramaPromotor.EsAprobado = true && EsBloqueado = false` |
| `GET /programas/{id}/tareas-pendientes` | Bearer JWT | Artista | `PromoPrograma.ArtistaId == ArtistaId del token` |
| `PATCH /programas/{id}/tareas-promotor/{id}/validar` | Bearer JWT | Artista | `PromoPrograma.ArtistaId == ArtistaId del token` |
| `PATCH /programas/{id}/tareas-promotor/{id}/rechazar` | Bearer JWT | Artista | `PromoPrograma.ArtistaId == ArtistaId del token` |

### Claims JWT Requeridos

```json
{
  "sub": "userId (string, Guid del usuario en Identity)",
  "email": "usuario@example.com",
  "exp": 1234567890
}
```

**Resolucion de identidad en backend:**
- **Promotor:** `UserId` del claim `sub` -> buscar `Promotor` por `UserId` -> obtener `PromotorId` -> buscar `PromoProgramaPromotor` por `PromotorId + ProgramaId`
- **Artista:** `UserId` del claim `sub` -> buscar `Artista` por `UserId` -> obtener `ArtistaId` -> verificar `PromoPrograma.ArtistaId == ArtistaId`

### Proteccion de Rutas Frontend

| Ruta | Auth | Actor | Redirect si no auth |
|------|------|-------|---------------------|
| `/programas/:id/mis-tareas` | Si | Promotor | `/login` |
| `/admin/programas/:id/tareas-pendientes` | Si | Artista | `/login` |

---

## DTOs

### Backend (C#)

#### MisTareasItemDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisTareasItemDto.cs

/// <summary>
/// DTO para un item de la lista de tareas del promotor (GET mis-tareas).
/// </summary>
public class MisTareasItemDto
{
    public Guid TareaId { get; set; }

    /// <summary>PromoTarea.Titulo</summary>
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    /// <summary>PromoTarea.InstruccionesUrl</summary>
    public string? InstruccionesUrl { get; set; }

    /// <summary>Resuelto via MaestraTipoEventoPromo</summary>
    public string TipoEventoPromoNombre { get; set; } = null!;

    /// <summary>Resuelto via MaestraTipoReward. Null si la tarea no tiene recompensa monetaria.</summary>
    public string? TipoRewardNombre { get; set; }

    public decimal? ImporteRecompensa { get; set; }

    /// <summary>Resuelto via MaestraMoneda. Null si no hay importe monetario.</summary>
    public string? MonedaNombre { get; set; }

    public int? PuntosRecompensa { get; set; }

    public bool EsRepetible { get; set; }

    public int? MaxRepeticiones { get; set; }

    public int Orden { get; set; }

    /// <summary>
    /// Estado del promotor en esta tarea. Null si nunca ha completado la tarea.
    /// </summary>
    public MiEstadoTareaDto? MiEstado { get; set; }
}
```

#### MiEstadoTareaDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MiEstadoTareaDto.cs

/// <summary>
/// Estado del promotor para una tarea especifica. Campos calculados desde multiples
/// registros PromoTareaPromotor agrupados por TareaId + ProgramaPromotorId.
/// </summary>
public class MiEstadoTareaDto
{
    /// <summary>Id del registro PromoTareaPromotor mas reciente.</summary>
    public Guid TareaPromotorId { get; set; }

    public int EstadoTareaId { get; set; }

    /// <summary>Resuelto via Maestra_EstadoTareaPromo</summary>
    public string EstadoTareaNombre { get; set; } = null!;

    /// <summary>COUNT de registros PromoTareaPromotor para TareaId + ProgramaPromotorId</summary>
    public int VecesCompletada { get; set; }

    /// <summary>MIN(FechaCompletado) calculado en query</summary>
    public DateTime? FechaPrimeraCompletada { get; set; }

    /// <summary>MAX(FechaCompletado) calculado en query</summary>
    public DateTime? FechaUltimaCompletada { get; set; }

    public string? UrlPruebaCompletado { get; set; }

    public string? ComentarioValidacion { get; set; }
}
```

#### MisTareasResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/MisTareasResponseDto.cs

public class MisTareasResponseDto
{
    public Guid ProgramaId { get; set; }

    public string ProgramaTitulo { get; set; } = null!;

    public IReadOnlyList<MisTareasItemDto> Items { get; set; } = new List<MisTareasItemDto>();
}
```

#### CompletarTareaDto (Request)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CompletarTareaDto.cs

public class CompletarTareaDto
{
    /// <summary>URL valida. Max 2048 caracteres. Obligatoria.</summary>
    public string UrlPruebaCompletado { get; set; } = null!;

    /// <summary>Opcional. Max 500 caracteres.</summary>
    public string? ComentarioPromotor { get; set; }
}
```

#### CompletarTareaResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/CompletarTareaResponseDto.cs

public class CompletarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }

    public int EstadoTareaId { get; set; }

    public string EstadoTareaNombre { get; set; } = null!;

    /// <summary>COUNT total de registros para este par TareaId + ProgramaPromotorId</summary>
    public int VecesCompletada { get; set; }

    public DateTime? FechaUltimaCompletada { get; set; }
}
```

#### TareaPendienteItemDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareaPendienteItemDto.cs

/// <summary>
/// DTO para un item de la lista de tareas pendientes de validacion (vista artista).
/// </summary>
public class TareaPendienteItemDto
{
    public Guid TareaPromotorId { get; set; }

    public Guid TareaId { get; set; }

    public string TareaNombre { get; set; } = null!;

    public Guid PromotorId { get; set; }

    /// <summary>Promotor.NombrePublico</summary>
    public string PromotorNombre { get; set; } = null!;

    /// <summary>Resuelto via MaestraTipoPromotor</summary>
    public string? PromotorTipoNombre { get; set; }

    public string? UrlPruebaCompletado { get; set; }

    public string? ComentarioPromotor { get; set; }

    /// <summary>COUNT total de registros para este par TareaId + ProgramaPromotorId</summary>
    public int VecesCompletada { get; set; }

    public DateTime? FechaUltimaCompletada { get; set; }
}
```

#### TareasPendientesResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/TareasPendientesResponseDto.cs

public class TareasPendientesResponseDto
{
    public IReadOnlyList<TareaPendienteItemDto> Items { get; set; } = new List<TareaPendienteItemDto>();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}
```

#### ValidarTareaDto (Request)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ValidarTareaDto.cs

public class ValidarTareaDto
{
    /// <summary>Opcional para validacion. Max 500 caracteres.</summary>
    public string? ComentarioValidacion { get; set; }
}
```

#### ValidarTareaResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/ValidarTareaResponseDto.cs

public class ValidarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }

    public int EstadoTareaId { get; set; }

    public string EstadoTareaNombre { get; set; } = null!;

    /// <summary>Null si la tarea no tiene recompensa monetaria.</summary>
    public decimal? RecompensaAcreditada { get; set; }

    /// <summary>Null si no hay recompensa monetaria.</summary>
    public string? MonedaNombre { get; set; }

    /// <summary>Null si la tarea no tiene recompensa en puntos.</summary>
    public int? PuntosAcreditados { get; set; }
}
```

#### RechazarTareaDto (Request)

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RechazarTareaDto.cs

public class RechazarTareaDto
{
    /// <summary>Obligatorio para rechazo. Max 500 caracteres.</summary>
    public string ComentarioValidacion { get; set; } = null!;
}
```

#### RechazarTareaResponseDto

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Dtos/RechazarTareaResponseDto.cs

public class RechazarTareaResponseDto
{
    public Guid TareaPromotorId { get; set; }

    public int EstadoTareaId { get; set; }

    public string EstadoTareaNombre { get; set; } = null!;
}
```

---

### Frontend (TypeScript)

```typescript
// Ruta: src/shared/types/cp-tareas-promocion.ts

// -------------------------
// Tipos de dominio
// -------------------------

export interface MiEstadoTarea {
  tareaPromotorId: string;
  estadoTareaId: EstadoTareaPromo;
  estadoTareaNombre: string;
  /** COUNT de registros PromoTareaPromotor para TareaId + ProgramaPromotorId */
  vecesCompletada: number;
  /** MIN(FechaCompletado). ISO string. Null si nunca completado. */
  fechaPrimeraCompletada: string | undefined;
  /** MAX(FechaCompletado). ISO string. Null si nunca completado. */
  fechaUltimaCompletada: string | undefined;
  urlPruebaCompletado: string | undefined;
  comentarioValidacion: string | undefined;
}

export interface MisTareasItem {
  tareaId: string;
  nombre: string;
  descripcion: string | undefined;
  instruccionesUrl: string | undefined;
  tipoEventoPromoNombre: string;
  tipoRewardNombre: string | undefined;
  importeRecompensa: number | undefined;
  monedaNombre: string | undefined;
  puntosRecompensa: number | undefined;
  esRepetible: boolean;
  maxRepeticiones: number | undefined;
  orden: number;
  /** Null si el promotor nunca ha completado esta tarea */
  miEstado: MiEstadoTarea | undefined;
}

export interface MisTareasResponse {
  programaId: string;
  programaTitulo: string;
  items: MisTareasItem[];
}

export interface TareaPendienteItem {
  tareaPromotorId: string;
  tareaId: string;
  tareaNombre: string;
  promotorId: string;
  promotorNombre: string;
  promotorTipoNombre: string | undefined;
  urlPruebaCompletado: string | undefined;
  comentarioPromotor: string | undefined;
  vecesCompletada: number;
  fechaUltimaCompletada: string | undefined;
}

export interface TareasPendientesResponse {
  items: TareaPendienteItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CompletarTareaResponse {
  tareaPromotorId: string;
  estadoTareaId: EstadoTareaPromo;
  estadoTareaNombre: string;
  vecesCompletada: number;
  fechaUltimaCompletada: string | undefined;
}

export interface ValidarTareaResponse {
  tareaPromotorId: string;
  estadoTareaId: EstadoTareaPromo;
  estadoTareaNombre: string;
  recompensaAcreditada: number | undefined;
  monedaNombre: string | undefined;
  puntosAcreditados: number | undefined;
}

export interface RechazarTareaResponse {
  tareaPromotorId: string;
  estadoTareaId: EstadoTareaPromo;
  estadoTareaNombre: string;
}

// -------------------------
// Union types
// -------------------------

export type EstadoTareaPromo = 1 | 2 | 3 | 4;
// 1 = Pendiente, 2 = Completada, 3 = Validada, 4 = Rechazada

export const ESTADO_TAREA_PROMO = {
  Pendiente: 1,
  Completada: 2,
  Validada: 3,
  Rechazada: 4,
} as const;
```

---

## Validaciones Compartidas

### Tabla de Reglas

| Campo | Regla | FluentValidation | Zod |
|-------|-------|-----------------|-----|
| `urlPruebaCompletado` | requerido | `.NotEmpty().WithErrorCode("1001")` | `.min(1, 'La URL de prueba es obligatoria')` |
| `urlPruebaCompletado` | URL valida | `.Must(BeAValidUrl).WithErrorCode("1013")` | `.url('La URL de prueba no tiene formato valido')` |
| `urlPruebaCompletado` | max 2048 caracteres | `.MaximumLength(2048).WithErrorCode("1002")` | `.max(2048, 'Maximo 2048 caracteres')` |
| `comentarioPromotor` | max 500 caracteres | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres').optional()` |
| `comentarioValidacion` (validar) | max 500 caracteres | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres').optional()` |
| `comentarioValidacion` (rechazar) | requerido | `.NotEmpty().WithErrorCode("1001")` | `.min(1, 'El motivo de rechazo es obligatorio')` |
| `comentarioValidacion` (rechazar) | max 500 caracteres | `.MaximumLength(500).WithErrorCode("1002")` | `.max(500, 'Maximo 500 caracteres')` |

### FluentValidation (C#)

```csharp
// CompletarTareaValidator.cs
using WePlayRises.Crowdpromotion.Domain.Constants;

public class CompletarTareaValidator : AbstractValidator<CompletarTareaCommand>
{
    public CompletarTareaValidator()
    {
        RuleFor(x => x.UrlPruebaCompletado)
            .NotEmpty()
            .WithMessage("La URL de prueba es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(2048)
            .WithMessage("La URL de prueba no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                         && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("La URL de prueba no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.UrlPruebaCompletado));

        RuleFor(x => x.ComentarioPromotor)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.ComentarioPromotor));
    }
}

// ValidarTareaValidator.cs
public class ValidarTareaValidator : AbstractValidator<ValidarTareaCommand>
{
    public ValidarTareaValidator()
    {
        RuleFor(x => x.ComentarioValidacion)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.ComentarioValidacion));
    }
}

// RechazarTareaValidator.cs
public class RechazarTareaValidator : AbstractValidator<RechazarTareaCommand>
{
    public RechazarTareaValidator()
    {
        RuleFor(x => x.ComentarioValidacion)
            .NotEmpty()
            .WithMessage("El motivo de rechazo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(500)
            .WithMessage("El motivo de rechazo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
```

### Zod Schemas (TypeScript)

```typescript
// Ruta: src/shared/schemas/cp-tareas-promocion.schema.ts
import { z } from 'zod';

export const completarTareaSchema = z.object({
  urlPruebaCompletado: z
    .string()
    .min(1, 'La URL de prueba es obligatoria')
    .max(2048, 'La URL no puede superar los 2048 caracteres')
    .url('La URL de prueba no tiene formato valido'),
  comentarioPromotor: z
    .string()
    .max(500, 'El comentario no puede superar los 500 caracteres')
    .optional(),
});

export type CompletarTareaFormData = z.infer<typeof completarTareaSchema>;

export const validarTareaSchema = z.object({
  comentarioValidacion: z
    .string()
    .max(500, 'El comentario no puede superar los 500 caracteres')
    .optional(),
});

export type ValidarTareaFormData = z.infer<typeof validarTareaSchema>;

export const rechazarTareaSchema = z.object({
  comentarioValidacion: z
    .string()
    .min(1, 'El motivo de rechazo es obligatorio')
    .max(500, 'El motivo de rechazo no puede superar los 500 caracteres'),
});

export type RechazarTareaFormData = z.infer<typeof rechazarTareaSchema>;
```

---

## Constantes Nuevas en ServiceResponseMessageType

Los siguientes codigos deben agregarse a `ServiceResponseMessageType.cs` en el modulo Crowdpromotion:

```csharp
// Ruta: Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs

// NotFound (2000-2999) - AGREGAR:
public const string NotFound_PromoTarea = "2021";
public const string NotFound_TareaPromotor = "2022";

// Business Rules (4000-4999) - AGREGAR:
/// <summary>Tarea con EsRepetible=false ya tiene un completado activo (EstadoTareaId IN 2, 3)</summary>
public const string BusinessRule_TareaNoRepetible = "4027";

/// <summary>Tarea repetible con VecesCompletada >= MaxRepeticiones</summary>
public const string BusinessRule_MaxRepeticionesAlcanzado = "4028";

/// <summary>PromoTarea.EsActivo = false</summary>
public const string BusinessRule_TareaInactiva = "4029";

/// <summary>PromoTarea.FechaFin != null && FechaFin menor que now()</summary>
public const string BusinessRule_TareaFueraFecha = "4030";

/// <summary>PromoTareaPromotor.EstadoTareaId != 2 al intentar validar o rechazar</summary>
public const string BusinessRule_TareaPromotorEstadoInvalido = "4031";
```

---

## Constantes Compartidas

```typescript
// src/shared/constants/query-keys.ts
// AGREGAR al objeto QUERY_KEYS existente:
export const QUERY_KEYS = {
  // ... existentes ...
  cpMisTareas: (programaId: string) => ['cp-mis-tareas', programaId] as const,
  cpTareasPendientes: (programaId: string) => ['cp-tareas-pendientes', programaId] as const,
};
```

```typescript
// src/shared/constants/api-routes.ts
// AGREGAR al objeto API_ROUTES existente:
export const API_ROUTES = {
  // ... existentes ...
  crowdpromotion: {
    // ... existentes ...
    misTareas: (programaId: string) =>
      `/api/crowdpromotion/programas/${programaId}/mis-tareas`,
    completarTarea: (programaId: string, tareaId: string) =>
      `/api/crowdpromotion/programas/${programaId}/tareas/${tareaId}/completar`,
    tareasPendientes: (programaId: string) =>
      `/api/crowdpromotion/programas/${programaId}/tareas-pendientes`,
    validarTarea: (programaId: string, tareaPromotorId: string) =>
      `/api/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/validar`,
    rechazarTarea: (programaId: string, tareaPromotorId: string) =>
      `/api/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/rechazar`,
  },
};
```

---

## Mapeo de Errores a UI

```typescript
// src/shared/utils/error-messages.ts
// AGREGAR al objeto ERROR_MESSAGES existente:
export const ERROR_MESSAGES: Record<string, string> = {
  // ... existentes ...

  // Tareas de promocion
  '4027': 'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.',
  '4028': 'Has alcanzado el numero maximo de veces que puedes completar esta tarea.',
  '4029': 'Esta tarea ya no esta disponible.',
  '4030': 'El plazo para completar esta tarea ha finalizado.',
  '4031': 'Este completado no puede ser procesado porque ya fue validado o rechazado.',
  '2021': 'La tarea no existe o no pertenece a este programa.',
  '2022': 'El completado no existe o no pertenece a este programa.',
};
```

---

## Reglas de Negocio Documentadas

### RN-01: Verificacion de aprobacion del promotor

Antes de cualquier operacion del promotor, verificar que existe un `PromoProgramaPromotor` con:
- `PromotorId` matching el promotor del token
- `ProgramaId` matching el `programaId` del path
- `EsAprobado = true`
- `EsBloqueado = false`

Si no se cumple: HTTP 403, ErrorCode `4026`.

### RN-02: Tarea no repetible - control de duplicados

Al completar, si `PromoTarea.EsRepetible = false`:
- Buscar cualquier `PromoTareaPromotor` para `TareaId + ProgramaPromotorId` con `EstadoTareaId IN (2, 3)` (Completada o Validada)
- Si existe alguno: error `4027`
- Si el unico registro tiene `EstadoTareaId = 4` (Rechazada): permitir re-envio actualizando ese registro

### RN-03: Tarea repetible - control de maximo

Al completar, si `PromoTarea.EsRepetible = true` y `MaxRepeticiones != null`:
- Contar registros `PromoTareaPromotor` para `TareaId + ProgramaPromotorId` con `EstadoTareaId IN (2, 3)` (Completada o Validada, no Rechazada)
- Si `count >= MaxRepeticiones`: error `4028`

### RN-04: Re-envio tras rechazo

Al completar una tarea cuyo registro mas reciente tiene `EstadoTareaId = 4`:
- Actualizar ese registro: `UrlPruebaCompletado = nueva URL`, `ComentarioPromotor = nuevo comentario`, `FechaCompletado = now()`, `EstadoTareaId = 2`, `ComentarioValidacion = null`, `FechaValidado = null`
- No crear un nuevo registro
- Esta regla aplica tanto para tareas repetibles como no repetibles

### RN-05: Acreditacion transaccional de recompensa

Al validar, si `PromoTarea.ImporteRecompensa != null && PromoTarea.MonedaId != null`:
- Buscar `PromotorWallet` donde `PromotorId = promotor de la inscripcion` y `MonedaId = Tarea.MonedaId`
- Si no existe: crear `PromotorWallet` con `SaldoDisponible = 0`, `SaldoPendiente = 0`, `TotalGanado = 0`
- Crear `PromotorWalletTransaccion`:
  - `WalletId = wallet.Id`
  - `TipoRewardId = Tarea.TipoRewardId`
  - `EstadoTransaccionId = 1` (pendiente de liquidacion)
  - `Importe = Tarea.ImporteRecompensa`
  - `Concepto = $"Recompensa tarea: {Tarea.Titulo}"`
  - `FechaCreacion = now()`
- Actualizar `PromotorWallet`:
  - `SaldoPendiente += Tarea.ImporteRecompensa`
  - `TotalGanado += Tarea.ImporteRecompensa`
  - `FechaActualizacion = now()`
- Todo en la misma transaccion de base de datos con `SaveChangesAsync`

### RN-06: Solo completados en estado Completada pueden ser validados o rechazados

Antes de validar o rechazar, verificar que `PromoTareaPromotor.EstadoTareaId == 2`.
Si no es 2: HTTP 400, ErrorCode `4031`.

### RN-07: Ownership del artista

Para todos los endpoints de artista, verificar que `PromoPrograma.ArtistaId` coincide con el `ArtistaId` resuelto desde el token JWT. Si no coincide: HTTP 403, ErrorCode `4026`.

---

## Checklist de Contratos

- [x] Analisis del modelo de dominio con aclaraciones sobre campos calculados
- [x] Maquina de estados de PromoTareaPromotor documentada
- [x] Endpoints con request/response/errores completos
- [x] Autorizacion por endpoint
- [x] Claims JWT documentados
- [x] DTOs C# completos con comentarios de constraints
- [x] Types TypeScript equivalentes
- [x] Schemas Zod con mismas reglas que FluentValidation
- [x] Nuevas constantes ServiceResponseMessageType documentadas
- [x] Constantes compartidas (query keys y API routes)
- [x] Mapeo de errores a mensajes UI
- [x] Reglas de negocio detalladas (re-envio, transaccionalidad, control de repeticiones)
