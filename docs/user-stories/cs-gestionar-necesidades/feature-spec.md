# Feature: Gestion de Necesidades de Crowdsourcing

> **ID:** cs-gestionar-necesidades
> **User Story:** US-CS-02
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature permite a los artistas gestionar el ciclo de vida completo de sus necesidades de crowdsourcing profesional. Los artistas pueden publicar nuevas necesidades (ya sea desde cero o pre-rellenadas desde templates), ver un listado con filtros de todas sus necesidades activas e historicas, editar necesidades que aun estan abiertas a propuestas, y cerrar necesidades cuando ya no requieren el servicio.

La feature implementa un CRUD completo con reglas de negocio especificas: solo las necesidades en estado "Abierta" son editables, y al cerrar una necesidad (estados "Abierta" o "En Progreso"), todas las propuestas pendientes se rechazan automaticamente para notificar a los profesionales que ya no seran consideradas. El listado incluye badges de color segun estado, contador de propuestas recibidas, paginacion, y filtros por estado y texto libre.

Esta es la feature central del modulo de crowdsourcing desde la perspectiva del artista, transformando necesidades conceptuales en solicitudes publicadas que profesionales pueden ver y responder con propuestas.

---

## User Story

**Como** artista registrado en la plataforma,
**Quiero** publicar, listar, editar y cerrar necesidades de servicios profesionales,
**Para** gestionar mis solicitudes de crowdsourcing y atraer propuestas de profesionales cualificados.

---

## Flujo Principal

### 1. Publicar Necesidad

El artista puede crear necesidades desde dos origenes:
- **Desde un template** (US-CS-01): los campos se pre-rellenan automaticamente desde la necesidad de plantilla seleccionada
- **Manualmente**: el artista inicia con un formulario vacio y rellena todos los campos

Pasos:
1. Artista navega a `/crowdsourcing/necesidades/nueva` desde la aplicacion Admin/Dashboard
2. El sistema determina el origen (query param `?fromTemplate={id}` o manual)
3. Si viene desde template, el formulario se pre-rellena con titulo, descripcion, tipo, modalidad, presupuesto
4. El artista completa/edita los campos requeridos: Titulo (5-200 chars), Descripcion (max 4000), Tipo de necesidad (select de maestra), Modalidad de trabajo (Presencial/Remoto/Hibrido), Presupuesto Min/Max, Moneda, Ubicacion (ciudad/pais, requerido si modalidad != Remoto), Fecha limite propuestas (> hoy+1), Fecha inicio prevista (>= hoy), Proyecto artistico (select de proyectos del artista)
5. El artista valida los datos (frontend con Zod, backend con FluentValidation)
6. El sistema ejecuta `CreateNecesidadCommand`:
   - Crea `NecesidadCrowdsourcing` con estado "Abierta"
   - Asigna automaticamente `ArtistaId` desde JWT
   - Asigna `ProyectoArtisticoId` seleccionado
   - Valida que el proyecto pertenece al artista autenticado (403 si no)
   - Establece `FechaCreacion` con timestamp actual
7. El sistema muestra toast de exito: "Necesidad publicada correctamente"
8. Redirige al artista a `/crowdsourcing/mis-necesidades` (listado)

### 2. Listar Mis Necesidades

Pasos:
1. Artista navega a `/crowdsourcing/mis-necesidades`
2. El sistema ejecuta `GetMisNecesidadesQuery` con paginacion (page, pageSize) y filtros opcionales (estado, search)
3. Si el artista no tiene necesidades, se muestra empty state con ilustracion y CTAs: "Publicar necesidad" y "Usar una plantilla"
4. Si tiene necesidades, se muestra listado paginado con cards mostrando:
   - Titulo de la necesidad
   - Badge de estado con color: Abierta (verde), En Progreso (azul), Cerrada (gris), Cancelada (rojo)
   - Tipo de necesidad (nombre de maestra)
   - Rango de presupuesto (min - max + moneda)
   - Numero de propuestas recibidas (contador badge)
   - Fecha de publicacion (relativa: "hace 2 dias")
   - Fecha limite de propuestas (con indicador visual si esta proxima < 7 dias)
   - Modalidad de trabajo (icono + nombre)
5. El artista puede aplicar filtros: estado (dropdown multi-select), texto libre (busca en titulo y descripcion)
6. El artista puede hacer click en una card para navegar al detalle (`/crowdsourcing/necesidades/{id}`)

### 3. Editar Necesidad

Pasos:
1. Artista hace click en boton "Editar" en la card de una necesidad (solo visible si estado = Abierta)
2. El sistema verifica el estado de la necesidad (backend tambien valida en `UpdateNecesidadCommand`)
3. Si estado != Abierta, la API retorna 400 Bad Request con mensaje "Solo se pueden editar necesidades en estado Abierta"
4. Si tiene propuestas recibidas (count > 0), se muestra banner informativo: "Esta necesidad ya tiene N propuestas. Los cambios seran visibles para los profesionales que ya enviaron propuestas."
5. El artista modifica campos editables (todos excepto TipoNecesidadId que es inmutable para no invalidar propuestas)
6. El sistema valida y ejecuta `UpdateNecesidadCommand`:
   - Actualiza campos modificados
   - Establece `FechaActualizacion` con timestamp actual
   - Mantiene `FechaCreacion` original
7. El sistema muestra toast: "Necesidad actualizada"
8. Redirige al detalle de la necesidad

### 4. Cerrar Necesidad

Pasos:
1. Artista hace click en boton "Cerrar necesidad" (disponible si estado = Abierta o En Progreso)
2. El sistema muestra dialogo de confirmacion con:
   - Mensaje: "Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente."
   - Campo opcional: Motivo de cierre (textarea, max 500 chars)
   - Botones: "Cancelar" y "Confirmar cierre"
3. Si el artista cancela, se cierra el dialogo sin cambios
4. Si confirma, el sistema ejecuta `CerrarNecesidadCommand`:
   - Cambia `EstadoNecesidadId` a "Cerrada"
   - Guarda `MotivoCierre` (si se proporciono)
   - Actualiza `FechaActualizacion`
   - Busca todas las `PropuestaCrowdsourcing` con estado "Pendiente"
   - Cambia estado de propuestas pendientes a "Rechazada"
   - Establece motivo automatico: "Necesidad cerrada por el artista"
5. El sistema muestra toast: "Necesidad cerrada (N propuestas rechazadas)"
6. La necesidad cerrada ya no aparece en el listado publico (US-CS-03) pero sigue visible en el historial del artista

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Artista no tiene ProyectoArtistico | Redirigir a crear proyecto o crear uno automaticamente con nombre "Mi Proyecto" |
| FA-02 | Necesidad desde template con campos pre-rellenados | Mostrar formulario con datos editables, indicador visual "Creado desde plantilla: {nombre}" |
| FA-03 | Modalidad Presencial/Hibrido sin ubicacion | Validacion muestra error: "La ubicacion (ciudad y pais) es requerida para modalidad presencial o hibrida" |
| FA-04 | Presupuesto max < presupuesto min | Validacion muestra error: "El presupuesto maximo debe ser mayor o igual al minimo" |
| FA-05 | Fecha limite pasada al editar | Validacion muestra error: "La fecha limite de propuestas no puede ser anterior a manana" |
| FA-06 | Intento de editar necesidad cerrada | Boton editar no aparece en UI. Si llama API directamente, retorna 400 Bad Request |
| FA-07 | ProyectoArtistico no pertenece al artista | API retorna 403 Forbidden: "No tienes permiso para asignar esta necesidad a ese proyecto" |
| FA-08 | Click en necesidad del listado | Navega a detalle `/crowdsourcing/necesidades/{id}` con propuestas recibidas |
| FA-09 | Sin necesidades creadas | Muestra empty state con icono ilustrativo y CTAs hacia crear necesidad o usar plantilla |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto | Metodo de Prueba |
|----|----------|----------|------------------|
| AC-CS02-1 | El artista autenticado puede crear una necesidad con todos los campos obligatorios validados (Titulo 5-200 chars, TipoNecesidadId, ModalidadTrabajoId, ProyectoArtisticoId). Ubicacion requerida si modalidad != Remoto. PresupuestoMax >= PresupuestoMin. FechaLimitePropuestas > hoy+1. | Backend + Admin | Crear necesidad con datos validos. Verificar registro en BD con campos correctos. Intentar crear con datos invalidos y verificar errores de validacion. |
| AC-CS02-2 | La necesidad se crea con estado "Abierta" y aparece inmediatamente en el listado del artista (`/api/crowdsourcing/necesidades/mis-necesidades`) y en el listado publico (US-CS-03 futuro). Se muestra toast: "Necesidad publicada correctamente". | Backend + Admin | POST a `/api/crowdsourcing/necesidades`. Verificar estado=Abierta en BD. GET a `/mis-necesidades` y verificar aparece. Verificar toast en UI. |
| AC-CS02-3 | El `ArtistaId` se asigna automaticamente desde el JWT del usuario autenticado. El `ProyectoArtisticoId` se valida que pertenece al artista (403 Forbidden si no). | Backend | Crear necesidad con proyecto propio (OK). Intentar con proyecto de otro artista (403). Verificar `ArtistaId` en BD coincide con JWT. |
| AC-CS02-4 | Si la necesidad viene de un template (query param `?fromTemplate={id}`), los campos Titulo, Descripcion, TipoNecesidadId, ModalidadTrabajoId, PresupuestoMin, PresupuestoMax se pre-rellenan pero son editables. | Admin | Navegar desde wizard de template. Verificar campos pre-rellenados. Modificar valores y crear. Verificar se guardan valores modificados. |
| AC-CS02-5 | El artista ve un listado paginado de sus necesidades con badge de color segun estado: Abierta (verde), En Progreso (azul), Cerrada (gris), Cancelada (rojo). Cada card muestra: titulo, estado badge, tipo necesidad, presupuesto min-max, contador propuestas, fecha creacion, fecha limite, modalidad. | Admin | GET a `/mis-necesidades`. Crear necesidades en varios estados. Verificar badges con colores correctos (clases CSS). Verificar datos mostrados. |
| AC-CS02-6 | Se muestra un contador de propuestas recibidas en cada card (badge numerico). El contador se calcula con COUNT de PropuestaCrowdsourcing.NecesidadId. | Backend + Admin | Crear necesidad. Enviar 3 propuestas (seed o via API). Verificar contador muestra "3" en la card. Verificar query incluye COUNT. |
| AC-CS02-7 | El artista puede filtrar por estado (multi-select: Abierta, En Progreso, Cerrada, Cancelada) y buscar por texto libre (busca en Titulo y Descripcion). Los filtros se aplican server-side con query params. | Backend + Admin | Aplicar filtro estado="Abierta". Verificar resultados. Buscar texto "mezcla". Verificar busca en titulo y descripcion. Combinar filtros. |
| AC-CS02-8 | Si no hay necesidades, se muestra empty state con ilustracion, mensaje "No tienes necesidades publicadas", y CTAs: boton "Publicar necesidad" (navega a `/nueva`) y boton "Usar una plantilla" (navega a wizard templates). | Admin | Acceder a `/mis-necesidades` sin necesidades creadas. Verificar empty state visible. Click en botones y verificar navegacion. |
| AC-CS02-9 | Solo se puede editar una necesidad en estado "Abierta". Si tiene propuestas (count > 0), se muestra banner informativo: "Esta necesidad ya tiene N propuestas. Los cambios seran visibles...". Al guardar se actualiza `FechaActualizacion`. Si estado != Abierta, la API retorna 400 Bad Request y el boton editar no es visible en UI. | Backend + Admin | Intentar editar necesidad Abierta (OK). Editar necesidad con propuestas (muestra banner). Intentar editar necesidad Cerrada via API (400). Verificar boton oculto en UI. Verificar `FechaActualizacion` cambia. |
| AC-CS02-10 | Al cerrar una necesidad (PATCH `/necesidades/{id}/cerrar`), las propuestas pendientes (estado="Pendiente") pasan automaticamente a "Rechazada" con motivo "Necesidad cerrada por el artista". Se muestra confirmacion previa: "Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente." | Backend + Admin | Crear necesidad con 3 propuestas Pendiente. Ejecutar cierre. Verificar dialogo confirmacion. Confirmar. Verificar estado necesidad=Cerrada. Verificar 3 propuestas ahora Rechazada con motivo automatico. |
| AC-CS02-11 | Click en una card del listado navega al detalle (`/crowdsourcing/necesidades/{id}`) con propuestas recibidas. GET `/api/crowdsourcing/necesidades/{id}` retorna datos completos incluyendo array de propuestas. | Backend + Admin | Click en card. Verificar navegacion a detalle. Verificar se cargan propuestas (tabla o lista). Verificar endpoint GET devuelve necesidad + propuestas. |
| AC-CS02-12 | La necesidad cerrada ya no aparece en el listado publico (US-CS-03, endpoint futuro `/api/crowdsourcing/necesidades/publico`) pero sigue visible en el historial del artista (`/mis-necesidades`). | Backend | Cerrar necesidad. GET a `/mis-necesidades` (aparece). GET futuro a `/publico` (no aparece, o filtrar por estado!=Cerrada). Verificar query aplica filtro. |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar Commands/Queries CQRS: `CreateNecesidadCommand`, `UpdateNecesidadCommand`, `CerrarNecesidadCommand`, `GetMisNecesidadesQuery`, `GetNecesidadByIdQuery`. Validators con FluentValidation. Services: `INecesidadCrowdsourcingService`. DTOs: `NecesidadCrowdsourcingDto`, `NecesidadListDto`, `CreateNecesidadRequest`, `UpdateNecesidadRequest`, `CerrarNecesidadRequest`. Logica de negocio: validar estado Abierta para editar, auto-rechazar propuestas pendientes al cerrar, validar ProyectoArtisticoId pertenece al artista. Paginacion y filtros server-side. | ALTO |
| **Shared** | Definir TypeScript types para DTOs: `NecesidadCrowdsourcingDto`, `NecesidadListDto`, `CreateNecesidadRequest`, `UpdateNecesidadRequest`, `CerrarNecesidadRequest`, `PaginatedResponse<NecesidadListDto>`. Schemas Zod para validacion frontend: `createNecesidadSchema`, `updateNecesidadSchema`, `cerrarNecesidadSchema`. Constants: `ESTADOS_NECESIDAD`, `BADGE_COLORS`, `QUERY_KEYS`. | MEDIO |
| **Admin** | Implementar paginas y componentes: `NuevaNecesidadPage` (formulario crear/editar), `MisNecesidadesPage` (listado paginado con filtros), `NecesidadDetallePage` (detalle con propuestas). Componentes: `NecesidadForm` (con react-hook-form + zod), `NecesidadCard` (card listado con badges), `EstadoBadge` (badge con colores), `NecesidadFilters` (filtros estado + search), `EmptyStateNecesidades`, `CerrarNecesidadDialog`. Hooks: `useNecesidades` (lista paginada), `useCreateNecesidad`, `useUpdateNecesidad`, `useCerrarNecesidad`, `useNecesidadById`. Services: `necesidad.service.ts`. Routing: `/crowdsourcing/necesidades/nueva`, `/crowdsourcing/mis-necesidades`, `/crowdsourcing/necesidades/{id}`. | ALTO |
| **Landing** | NO involucrado en MVP. Futuro: listado publico de necesidades abiertas (US-CS-03). | BAJO (no MVP) |

---

## Entidades Involucradas

### NecesidadCrowdsourcing (existente, definida en US-CS-01)

Entidad principal con strongly typed IDs.

**Campos clave:**
- `Id` (NecesidadCrowdsourcingId, PK)
- `ArtistaId` (ArtistaId, FK -> Artista) - Asignado automaticamente desde JWT
- `ProyectoArtisticoId` (ProyectoArtisticoId, FK -> ProyectoArtistico) - Seleccionado por artista
- `Titulo` (string, NOT NULL, 5-200 chars)
- `Descripcion` (string, nullable, max 4000 chars)
- `TipoNecesidadId` (int, FK -> MaestraTipoNecesidad) - Inmutable una vez creado
- `EstadoNecesidadId` (int, FK -> MaestraEstadoNecesidad) - Abierta/En Progreso/Cerrada/Cancelada
- `ModalidadTrabajoId` (int, FK -> MaestraModalidadTrabajo) - Presencial/Remoto/Hibrido
- `PresupuestoMin` (decimal, nullable)
- `PresupuestoMax` (decimal, nullable, debe ser >= PresupuestoMin)
- `MonedaId` (int, FK -> MaestraMoneda) - Requerido si hay presupuesto
- `UbicacionCiudad` (string, nullable, max 100) - Requerido si modalidad Presencial/Hibrido
- `UbicacionPais` (string, nullable, max 100) - Requerido si modalidad Presencial/Hibrido
- `FechaLimitePropuestas` (DateTime, nullable, debe ser > hoy+1)
- `FechaInicioPrevista` (DateTime, nullable, debe ser >= hoy)
- `MotivoCierre` (string, nullable, max 500) - Opcional al cerrar
- `FechaCreacion` (DateTime, NOT NULL)
- `FechaActualizacion` (DateTime, nullable) - Se actualiza al editar

**Relaciones:**
- `Artista` (navigation property)
- `ProyectoArtistico` (navigation property)
- `Propuestas` (ICollection<PropuestaCrowdsourcing>)

### PropuestaCrowdsourcing (existente, impactada por cierre)

Al cerrar una necesidad, las propuestas pendientes se auto-rechazan.

**Campos impactados:**
- `EstadoPropuestaId` (int, FK -> MaestraEstadoPropuesta) - Cambia a "Rechazada"
- `MotivoRechazo` (string, nullable) - Se establece en "Necesidad cerrada por el artista"
- `FechaActualizacion` (DateTime) - Se actualiza timestamp

---

## API Endpoints

### POST /api/crowdsourcing/necesidades

Crear necesidad (manual o desde template).

**Auth:** Artista (JWT)

**Request:**
```json
{
  "titulo": "Mezcla de pistas para EP de 5 canciones",
  "descripcion": "Buscamos un ingeniero de mezcla experimentado...",
  "tipoNecesidadId": 3,
  "modalidadTrabajoId": 2,
  "presupuestoMin": 150.00,
  "presupuestoMax": 800.00,
  "monedaId": 1,
  "ubicacionCiudad": null,
  "ubicacionPais": null,
  "fechaLimitePropuestas": "2026-03-15",
  "fechaInicioPrevista": "2026-04-01",
  "proyectoArtisticoId": "guid"
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "estadoNecesidadNombre": "Abierta",
    "fechaCreacion": "2026-02-15T10:30:00Z"
  },
  "messages": [
    { "message": "Necesidad publicada correctamente", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Validacion fallida (titulo vacio, presupuesto invalido, fecha pasada, etc.)
- `403 Forbidden` - El ProyectoArtisticoId no pertenece al artista autenticado
- `404 Not Found` - ProyectoArtistico, TipoNecesidad, Modalidad o Moneda no existe

---

### GET /api/crowdsourcing/necesidades/mis-necesidades

Listar necesidades del artista autenticado (paginado).

**Auth:** Artista (JWT)

**Query params:**
- `estado` (int, opcional) - Filtrar por EstadoNecesidadId (multi-value: `?estado=1&estado=2`)
- `search` (string, opcional) - Buscar en Titulo y Descripcion
- `page` (int, default 1) - Numero de pagina
- `pageSize` (int, default 10) - Items por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "titulo": "Mezcla de pistas para EP de 5 canciones",
        "estadoNecesidadId": 1,
        "estadoNecesidadNombre": "Abierta",
        "tipoNecesidadNombre": "Post-produccion",
        "presupuestoMin": 150.00,
        "presupuestoMax": 800.00,
        "monedaNombre": "EUR",
        "modalidadTrabajoNombre": "Remoto",
        "numeroPropuestas": 3,
        "fechaCreacion": "2026-02-15T10:30:00Z",
        "fechaLimitePropuestas": "2026-03-15",
        "esProxima": true
      }
    ],
    "totalCount": 15,
    "page": 1,
    "pageSize": 10,
    "totalPages": 2
  },
  "messages": []
}
```

**Errores:** Ninguno (lista vacia si no hay necesidades)

---

### GET /api/crowdsourcing/necesidades/{id}

Detalle de necesidad (propietario).

**Auth:** Artista (propietario de la necesidad)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "descripcion": "Buscamos un ingeniero de mezcla experimentado...",
    "estadoNecesidadId": 1,
    "estadoNecesidadNombre": "Abierta",
    "tipoNecesidadId": 3,
    "tipoNecesidadNombre": "Post-produccion",
    "modalidadTrabajoId": 2,
    "modalidadTrabajoNombre": "Remoto",
    "presupuestoMin": 150.00,
    "presupuestoMax": 800.00,
    "monedaId": 1,
    "monedaNombre": "EUR",
    "ubicacionCiudad": null,
    "ubicacionPais": null,
    "fechaLimitePropuestas": "2026-03-15",
    "fechaInicioPrevista": "2026-04-01",
    "fechaCreacion": "2026-02-15T10:30:00Z",
    "fechaActualizacion": null,
    "motivoCierre": null,
    "proyectoArtistico": {
      "id": "guid",
      "nombre": "Mi primer EP"
    },
    "propuestas": [
      {
        "id": "guid",
        "profesionalNombre": "Studio Mix Pro",
        "precioPropuesto": 450.00,
        "monedaNombre": "EUR",
        "diasEstimados": 14,
        "estadoPropuestaNombre": "Pendiente",
        "fechaCreacion": "2026-02-20T14:00:00Z"
      }
    ],
    "numeroPropuestas": 3
  },
  "messages": []
}
```

**Errores:**
- `404 Not Found` - Necesidad no existe
- `403 Forbidden` - No es el propietario de la necesidad

---

### PUT /api/crowdsourcing/necesidades/{id}

Editar necesidad (solo si estado = Abierta).

**Auth:** Artista (propietario de la necesidad)

**Request:** (mismos campos que POST, excepto `proyectoArtisticoId` y `tipoNecesidadId` que son inmutables)
```json
{
  "titulo": "Mezcla de pistas para EP (actualizado)",
  "descripcion": "Descripcion actualizada...",
  "modalidadTrabajoId": 2,
  "presupuestoMin": 200.00,
  "presupuestoMax": 900.00,
  "monedaId": 1,
  "ubicacionCiudad": null,
  "ubicacionPais": null,
  "fechaLimitePropuestas": "2026-03-20",
  "fechaInicioPrevista": "2026-04-05"
}
```

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla de pistas para EP (actualizado)",
    "fechaActualizacion": "2026-02-16T09:00:00Z"
  },
  "messages": [
    { "message": "Necesidad actualizada", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Estado != Abierta (mensaje: "Solo se pueden editar necesidades en estado Abierta") o validacion fallida
- `403 Forbidden` - No es el propietario
- `404 Not Found` - Necesidad no existe

---

### PATCH /api/crowdsourcing/necesidades/{id}/cerrar

Cerrar necesidad (auto-rechaza propuestas pendientes).

**Auth:** Artista (propietario de la necesidad)

**Request:**
```json
{
  "motivo": "Ya encontre un profesional por otra via"
}
```
(Campo `motivo` es opcional)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoNecesidadNombre": "Cerrada",
    "propuestasRechazadas": 2
  },
  "messages": [
    { "message": "Necesidad cerrada", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Estado no permite cierre (solo Abierta o En Progreso pueden cerrarse)
- `403 Forbidden` - No es el propietario
- `404 Not Found` - Necesidad no existe

---

## Validaciones

### CreateNecesidadValidator

```csharp
public class CreateNecesidadCommandValidator : AbstractValidator<CreateNecesidadCommand>
{
    public CreateNecesidadCommandValidator(
        INecesidadCrowdsourcingService necesidadService,
        IProyectoArtisticoService proyectoService)
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(5)
            .WithMessage("El titulo debe tener al menos 5 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El titulo no puede exceder 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(4000)
            .WithMessage("La descripcion no puede exceder 4000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.TipoNecesidadId)
            .NotEmpty()
            .WithMessage("El tipo de necesidad es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ModalidadTrabajoId)
            .NotEmpty()
            .WithMessage("La modalidad de trabajo es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.PresupuestoMax)
            .GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0)
            .When(x => x.PresupuestoMax.HasValue && x.PresupuestoMin.HasValue)
            .WithMessage("El presupuesto maximo debe ser mayor o igual al minimo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .When(x => x.PresupuestoMin.HasValue || x.PresupuestoMax.HasValue)
            .WithMessage("La moneda es obligatoria si se especifica presupuesto")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.FechaLimitePropuestas)
            .GreaterThan(DateTime.Today.AddDays(1))
            .When(x => x.FechaLimitePropuestas.HasValue)
            .WithMessage("La fecha limite debe ser posterior a manana")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        RuleFor(x => x.FechaInicioPrevista)
            .GreaterThanOrEqualTo(DateTime.Today)
            .When(x => x.FechaInicioPrevista.HasValue)
            .WithMessage("La fecha de inicio prevista no puede ser anterior a hoy")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        // Validacion condicional: Ubicacion requerida si modalidad es Presencial (1) o Hibrido (3)
        RuleFor(x => x.UbicacionCiudad)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicacion (ciudad) es requerida para modalidad presencial o hibrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UbicacionPais)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicacion (pais) es requerida para modalidad presencial o hibrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProyectoArtisticoId)
            .NotEmpty()
            .WithMessage("El proyecto artistico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MustAsync(async (command, proyectoId, ct) =>
            {
                var proyecto = await proyectoService.GetByIdAsync(proyectoId, ct);
                return proyecto != null && proyecto.ArtistaId == command.ArtistaIdFromJwt;
            })
            .WithMessage("El proyecto no existe o no te pertenece")
            .WithErrorCode(ServiceResponseMessageType.Auth_Forbidden);
    }
}
```

### UpdateNecesidadValidator

Similar a Create, pero ademas valida:
- `TipoNecesidadId` no es editable (no incluido en request)
- Estado actual debe ser "Abierta" (1)

```csharp
RuleFor(x => x)
    .MustAsync(async (command, ct) =>
    {
        var necesidad = await necesidadService.GetByIdAsync(command.Id, ct);
        return necesidad?.EstadoNecesidadId == 1; // Abierta
    })
    .WithMessage("Solo se pueden editar necesidades en estado Abierta")
    .WithErrorCode(ServiceResponseMessageType.BusinessRule_InvalidState);
```

### CerrarNecesidadValidator

```csharp
public class CerrarNecesidadCommandValidator : AbstractValidator<CerrarNecesidadCommand>
{
    public CerrarNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .WithMessage("El motivo no puede exceder 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await necesidadService.GetByIdAsync(command.Id, ct);
                return necesidad != null && (necesidad.EstadoNecesidadId == 1 || necesidad.EstadoNecesidadId == 2); // Abierta o En Progreso
            })
            .WithMessage("Solo se pueden cerrar necesidades en estado Abierta o En Progreso")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_InvalidState);
    }
}
```

---

## Requisitos No Funcionales

### Performance
- Listado paginado debe cargar en < 500ms con 100 necesidades en BD
- Query de listado optimizado con proyecciones (select especificos, no cargar toda la entidad)
- Contador de propuestas calculado con COUNT en query (no stored en entidad)
- Cache de maestras (tipos, estados, modalidades) en RequestCache para evitar queries duplicados

### Seguridad
- Validar que ProyectoArtisticoId pertenece al artista autenticado (prevenir asignacion a proyectos ajenos)
- Autorizar operaciones edit/close solo al propietario de la necesidad (403 si no)
- Sanitizar inputs de texto (titulo, descripcion) para prevenir XSS
- Rate limiting en creacion (max 10 necesidades por hora por usuario)

### UX
- Formulario de creacion con validacion en tiempo real (Zod en frontend)
- Empty state con ilustracion atractiva y CTAs claros
- Badges de estado con colores semanticos (verde=activo, azul=progreso, gris=cerrado, rojo=cancelado)
- Indicador visual si fecha limite esta proxima (< 7 dias): badge "Urgente" o icono de alerta
- Confirmacion previa al cerrar necesidad (evitar cierres accidentales)
- Toast notifications en todas las acciones (crear, editar, cerrar)

### Mantenibilidad
- Maestras de estado/tipo/modalidad en BD (no hardcoded en codigo)
- Logs de auditoria: quien creo/edito/cerro cada necesidad (timestamp + userId)
- Transaccion atomica al cerrar necesidad (update necesidad + update propuestas en mismo transaction)
- Soft delete opcional en futuro (agregar campo `Eliminada` en lugar de DELETE)

---

## Dependencias

### Tecnicas (dentro del proyecto)
- **Modulo UserAccess**: Entidad `Artista` (FK desde NecesidadCrowdsourcing)
- **Modulo Crowdfunding**: Entidad `ProyectoArtistico` (FK desde NecesidadCrowdsourcing)
- **Modulo Crowdsourcing**: Entidad `NecesidadCrowdsourcing` (ya existe), `PropuestaCrowdsourcing` (impactada al cerrar)
- **Maestras**: `MaestraTipoNecesidad`, `MaestraEstadoNecesidad`, `MaestraModalidadTrabajo`, `MaestraMoneda`
- **Shared**: Types, schemas Zod, constants para estados y QUERY_KEYS
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType`, `IRequestCacheService`

### De otras User Stories
- **US-CS-01**: Templates que generan necesidades (opcional, no bloquea CRUD manual)
  - El flujo desde template usa query param `?fromTemplate={id}` pero el endpoint de creacion es el mismo
  - Si el artista no tiene templates, puede crear necesidades manualmente sin problemas

### Dependencias futuras (que esta US habilita)
- **US-CS-03**: Listado publico de necesidades abiertas (profesionales exploran necesidades)
  - Requiere GET `/api/crowdsourcing/necesidades/publico` (filtra estado=Abierta)
- **US-CS-04**: Envio de propuestas por profesionales
  - Requiere que existan necesidades en estado Abierta
- **US-CS-05**: Seleccion de profesional y creacion de acuerdo
  - Requiere propuestas enviadas a necesidades

---

## Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Artistas crean necesidades spam o irreales | Media | Medio | Validacion de campos obligatorios. Moderacion manual en futuro. Rate limiting (max 10 necesidades/hora). |
| Propuestas pendientes no se auto-rechazan correctamente | Baja | Alto | Tests de integracion que verifican transaccion atomica. Unit tests de comando CerrarNecesidad. |
| Queries lentos en listado con miles de necesidades | Media | Medio | Paginacion obligatoria. Indices en BD (ArtistaId, EstadoNecesidadId, FechaCreacion). Proyecciones optimizadas. |
| Edicion concurrente de necesidad (dos artistas editan simultaneo) | Baja | Bajo | Usar concurrency token (RowVersion) en futuro. Por ahora last-write-wins. |
| Artista cierra necesidad accidentalmente | Media | Medio | Dialogo de confirmacion con texto explicativo. Boton "Cancelar" prominente. Log de auditoria para recovery manual. |
| ProyectoArtistico eliminado deja necesidades huerfanas | Baja | Medio | FK con RESTRICT (no permitir eliminar proyecto con necesidades). Soft delete de proyectos en futuro. |

---

## Notas de Implementacion

### Backend
- Crear Commands/Queries en `Modules/Crowdsourcing/Crowdsourcing.Application/Features/Necesidades/`
  - Commands: `CreateNecesidadCommand.cs`, `UpdateNecesidadCommand.cs`, `CerrarNecesidadCommand.cs`
  - Queries: `GetMisNecesidadesQuery.cs`, `GetNecesidadByIdQuery.cs`
  - Validators: `CreateNecesidadValidator.cs`, `UpdateNecesidadValidator.cs`, `CerrarNecesidadValidator.cs`
- Implementar Service: `NecesidadCrowdsourcingService : INecesidadCrowdsourcingService`
  - Metodos: `GetByIdAsync`, `GetByArtistaIdAsync` (paginado), `CreateAsync`, `UpdateAsync`, `CerrarAsync`
  - Service retorna entidades, Handlers hacen mapping a DTOs
- Implementar Repository: `NecesidadCrowdsourcingRepository : INecesidadCrowdsourcingRepository`
  - Query optimizado para listado con JOIN a maestras y COUNT de propuestas
- AutoMapper Profile: `NecesidadProfile.cs`
  - Mapping bidireccional: Command <-> Entity, Entity <-> DTO
- Controller: `NecesidadesController` con endpoints descritos arriba
- Logica de cierre: en Handler de `CerrarNecesidadCommand`, usar Service para:
  1. Actualizar necesidad (estado + motivo + fecha)
  2. Buscar propuestas pendientes
  3. Actualizar propuestas (estado + motivo rechazo)
  4. Todo en una transaccion (unit of work)

### Frontend (Admin)
- Implementar paginas:
  - `NuevaNecesidadPage.tsx` (`/crowdsourcing/necesidades/nueva`)
  - `MisNecesidadesPage.tsx` (`/crowdsourcing/mis-necesidades`)
  - `NecesidadDetallePage.tsx` (`/crowdsourcing/necesidades/:id`)
- Componentes reutilizables:
  - `NecesidadForm.tsx` (formulario crear/editar con react-hook-form + zod)
  - `NecesidadCard.tsx` (card del listado con badges, datos, contador propuestas)
  - `EstadoBadge.tsx` (badge con color segun estado)
  - `NecesidadFilters.tsx` (filtros estado multi-select + search input)
  - `EmptyStateNecesidades.tsx` (ilustracion + mensaje + CTAs)
  - `CerrarNecesidadDialog.tsx` (dialogo confirmacion con motivo opcional)
- Hooks:
  - `useNecesidades.ts` (useQuery con paginacion y filtros)
  - `useNecesidadById.ts` (useQuery para detalle)
  - `useCreateNecesidad.ts` (useMutation)
  - `useUpdateNecesidad.ts` (useMutation)
  - `useCerrarNecesidad.ts` (useMutation)
- Services:
  - `necesidad.service.ts` (calls a API endpoints)
- Esquemas Zod:
  - `createNecesidadSchema` (validacion campos obligatorios, rangos, fechas)
  - `updateNecesidadSchema` (similar a create)
  - `cerrarNecesidadSchema` (motivo max 500 chars)

### Datos Seed (opcional)
- Crear algunas necesidades de ejemplo para testing
- Estados: Abierta (1), En Progreso (2), Cerrada (3), Cancelada (4)
- Tipos de necesidad: Pre-produccion (1), Produccion (2), Post-produccion (3), Marketing (4), etc.
- Modalidades: Presencial (1), Remoto (2), Hibrido (3)

### Testing
- **Unit tests:**
  - Validators: `CreateNecesidadValidatorTests`, `UpdateNecesidadValidatorTests`, `CerrarNecesidadValidatorTests`
  - Commands: `CerrarNecesidadCommandHandlerTests` (verifica auto-rechazo propuestas)
- **Integration tests:**
  - Endpoints CRUD completos
  - Flujo completo: crear necesidad -> enviar propuestas -> cerrar necesidad -> verificar propuestas rechazadas
- **E2E tests:**
  - Wizard crear necesidad desde cero
  - Wizard crear desde template (requiere US-CS-01)
  - Listado con filtros y paginacion
  - Editar necesidad abierta
  - Intentar editar necesidad cerrada (debe fallar)
  - Cerrar necesidad con propuestas pendientes

---

## Out of Scope (No incluido en esta feature)

- Listado publico de necesidades para profesionales (US-CS-03)
- Envio de propuestas por profesionales (US-CS-04)
- Chat/conversaciones sobre necesidades (US-CS-06)
- Notificaciones push/email al recibir propuestas
- Analíticas sobre necesidades (views, engagement, conversion rate)
- Duplicar necesidad
- Archivar necesidad (soft delete)
- Historial de cambios/auditoria visible en UI
- Compartir necesidad en redes sociales
- Templates custom creados por artistas (solo templates seed en US-CS-01)

---

## Referencia

- User Story completa: [docs/product/US-CS-02-gestionar-necesidades.md](../../product/US-CS-02-gestionar-necesidades.md)
- US-CS-01 Templates: [docs/user-stories/cs-templates-guia/feature-spec.md](../cs-templates-guia/feature-spec.md)
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
- CLAUDE.md: Patron CQRS, reglas de validacion, ServiceResponse
