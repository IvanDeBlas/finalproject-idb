# Feature: Explorar Necesidades y Enviar Propuestas

> **ID:** cs-explorar-propuestas
> **User Story:** US-CS-03
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature habilita el lado del profesional en el modulo de crowdsourcing: explorar el catalogo publico de necesidades abiertas, ver el detalle de cada una, enviar propuestas con precio y condiciones propias, hacer seguimiento del estado de sus propuestas, y retirarlas si es necesario. Es la contraparte de US-CS-02 (gestion de necesidades por el artista) y cierra el ciclo de oferta y demanda de servicios musicales en la plataforma.

El flujo comienza con un listado paginado (12 items por pagina) de necesidades en estado "Abierta" y no expiradas, filtrable por tipo, modalidad, rango de presupuesto, ubicacion, y texto libre. El detalle de cada necesidad expone campos calculados en funcion del usuario autenticado: si ya envio propuesta, si es el artista propietario, y si tiene PerfilProfesional. Si el profesional cumple todas las condiciones, puede enviar una propuesta con precio, moneda, tiempo estimado y mensaje. Una vez enviada, puede consultarla en "Mis Propuestas" y retirarla si aun esta pendiente.

Esta feature representa el punto de entrada de los profesionales al ecosistema de crowdsourcing y es prerequisito directo de US-CS-04 (seleccion de profesional y acuerdo), por lo que su calidad y fiabilidad son criticas para el MVP.

---

## User Story

**Como** profesional de la industria musical (productor, disenador, fotografo, etc.),
**Quiero** buscar necesidades abiertas, enviar propuestas con mi precio y condiciones, hacer seguimiento de mis propuestas y poder retirarlas si es necesario,
**Para** encontrar oportunidades de trabajo y ofrecer mis servicios a artistas en la plataforma.

---

## Flujo Principal

### 1. Explorar Necesidades Abiertas

1. Profesional navega a `/crowdsourcing/necesidades` desde la Landing publica
2. El sistema ejecuta `GetNecesidadesPublicasQuery` filtrando solo estado "Abierta" y `FechaLimitePropuestas >= hoy`
3. Se muestra un listado paginado (12 items/pagina) con cards de necesidad. Cada card incluye:
   - Titulo de la necesidad
   - Descripcion truncada a 150 caracteres
   - Badge de tipo de necesidad (Post-produccion, Produccion, Diseno, etc.)
   - Rango de presupuesto (min - max + moneda)
   - Modalidad de trabajo (icono + nombre)
   - Ubicacion (ciudad, pais) si modalidad es Presencial o Hibrido
   - Nombre del artista
   - Fecha de publicacion relativa ("hace 2 dias")
   - Fecha limite de propuestas (con badge "URGENTE" si faltan < 3 dias)
   - Numero de propuestas recibidas
4. El profesional aplica filtros opcionales (con debounce 300ms en frontend):
   - Tipo de necesidad (multi-select)
   - Modalidad de trabajo (select)
   - Rango de presupuesto (slider min-max)
   - Ubicacion (pais y ciudad)
   - Ordenar por (mas recientes, mayor presupuesto, fecha limite proxima)
   - Busqueda por texto (busca en titulo y descripcion)
5. Si no hay resultados, se muestra empty state: "No hay necesidades que coincidan. Intenta ampliar tus filtros."
6. El profesional hace click en una card para navegar al detalle

### 2. Ver Detalle de Necesidad

1. Profesional hace click en una card del listado o navega directamente a `/crowdsourcing/necesidades/{id}`
2. El sistema ejecuta `GetNecesidadPublicaByIdQuery` retornando datos completos de la necesidad mas los campos calculados:
   - `yaPropuso`: true si el usuario ya envio propuesta a esta necesidad
   - `esPropietario`: true si el usuario es el artista dueno de la necesidad
   - `tienePerfilProfesional`: true si el usuario tiene PerfilProfesional activo
3. Se muestra el detalle completo: todos los campos de la necesidad, descripcion completa, datos del artista (nombre, imagen)
4. La disponibilidad del boton "Enviar propuesta" se determina segun los campos calculados:
   - Si `esPropietario == true`: boton deshabilitado con mensaje "Esta es tu necesidad"
   - Si `yaPropuso == true`: boton deshabilitado con mensaje "Ya enviaste una propuesta"
   - Si `tienePerfilProfesional == false`: CTA para crear perfil profesional en lugar del boton
   - Si ninguna condicion anterior: boton "Enviar propuesta" habilitado
5. El profesional puede ver el detalle completo independientemente de si tiene perfil o no

### 3. Enviar Propuesta

1. Profesional hace click en "Enviar propuesta" desde el detalle de una necesidad
2. El sistema muestra el formulario de propuesta (modal o pagina `/crowdsourcing/necesidades/{id}/propuesta`)
3. El profesional completa los campos:
   - Precio propuesto (decimal, obligatorio, > 0)
   - Moneda (select, obligatorio)
   - Tiempo estimado de entrega en dias (entero, opcional, > 0, max 365)
   - Mensaje de propuesta (texto largo, obligatorio, 20-2000 caracteres)
4. El sistema muestra una nota informativa si el precio esta fuera del rango de presupuesto del artista (por encima o por debajo), pero permite continuar
5. El profesional valida los datos (frontend con Zod, backend con FluentValidation)
6. El sistema ejecuta `CreatePropuestaCommand`:
   - Crea `PropuestaCrowdsourcing` con estado "Pendiente"
   - Asigna automaticamente `UserId` desde JWT
   - Asigna automaticamente `PerfilProfesionalId` del usuario autenticado
   - Valida unicidad: un usuario no puede tener dos propuestas para la misma necesidad
   - Valida ownership: el artista propietario no puede enviar propuesta a su propia necesidad
   - Valida que la necesidad existe y esta en estado "Abierta"
7. El sistema muestra toast de exito: "Propuesta enviada correctamente. El artista sera notificado."
8. Redirige al profesional a `/crowdsourcing/mis-propuestas`

### 4. Ver Mis Propuestas

1. Profesional navega a `/crowdsourcing/mis-propuestas`
2. El sistema ejecuta `GetMisPropuestasQuery` con paginacion y filtro opcional por estado
3. Se muestra listado paginado de propuestas del usuario autenticado. Cada item incluye:
   - Titulo de la necesidad
   - Nombre del artista
   - Precio propuesto + moneda
   - Badge de estado con color semantico:
     - Pendiente: amarillo
     - Aceptada: verde
     - Rechazada: rojo
     - Retirada: gris
   - Fecha de envio
   - Fecha de respuesta del artista (si existe)
4. El profesional puede filtrar por estado (dropdown)
5. Propuestas en estado "Pendiente" muestran boton "Retirar propuesta"
6. Propuestas en estado "Aceptada" muestran enlace "Ver acuerdo" que navega al detalle del acuerdo generado
7. Propuestas en estado "Rechazada" y "Retirada" son de solo lectura

### 5. Retirar Propuesta

1. Profesional hace click en "Retirar propuesta" en la card de una propuesta en estado "Pendiente"
2. El sistema muestra dialogo de confirmacion con:
   - Mensaje: "Retirar propuesta? Esta accion no se puede deshacer."
   - Botones: "Cancelar" y "Confirmar"
3. Si el profesional cancela, se cierra el dialogo sin cambios
4. Si confirma, el sistema ejecuta `RetirarPropuestaCommand`:
   - Valida que el usuario es el propietario de la propuesta
   - Valida que el estado actual es "Pendiente"
   - Cambia `EstadoPropuestaId` a "Retirada"
   - Actualiza `FechaActualizacion` con timestamp actual
5. El sistema muestra toast: "Propuesta retirada"
6. El artista deja de ver la propuesta como candidato activo (o la ve en gris/tachada)
7. El profesional puede ver la propuesta en su historial en estado "Retirada" pero no puede re-enviarla (debe crear una nueva)

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Profesional sin PerfilProfesional intenta enviar propuesta | Mostrar CTA para crear perfil profesional. El detalle sigue visible. |
| FA-02 | Profesional intenta enviar segunda propuesta a la misma necesidad | Frontend desactiva el boton (`yaPropuso=true`). Backend retorna 400: "Ya tienes una propuesta para esta necesidad." |
| FA-03 | Artista propietario intenta enviarse propuesta a su propia necesidad | Frontend desactiva el boton (`esPropietario=true`). Backend retorna 403: "No puedes enviar propuesta a tu propia necesidad." |
| FA-04 | Necesidad con fecha limite expirada (`FechaLimitePropuestas < hoy`) | No aparece en el listado publico. Si se accede directamente por URL, el backend retorna 404 o 400 segun criterio. |
| FA-05 | Precio propuesto fuera del rango de presupuesto del artista | Mostrar nota informativa: "Tu precio esta por encima/debajo del presupuesto indicado por el artista." Permitir enviar. |
| FA-06 | No hay resultados en la busqueda | Empty state con ilustracion y mensaje: "No hay necesidades que coincidan. Intenta ampliar tus filtros." |
| FA-07 | Intento de retirar propuesta que no esta en estado Pendiente | Boton "Retirar" no visible en UI. Si llama API directamente, retorna 400 Bad Request. |
| FA-08 | Necesidad pasa a estado Cerrada mientras el profesional ve el detalle | Al recargar, la necesidad ya no se muestra en el listado. Si accede por URL, backend puede retornar 404. |
| FA-09 | Profesional sin cuenta intenta explorar necesidades | Redirigir a login. El listado publico requiere autenticacion (Auth: Autenticado). |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto | Metodo de Prueba |
|----|----------|----------|------------------|
| AC-CS03-1 | Solo se muestran necesidades en estado `Abierta` en el listado publico. Necesidades en estado Cerrada, En Progreso o Cancelada no aparecen. | Backend + Landing | Crear necesidades en varios estados. GET `/api/crowdsourcing/necesidades`. Verificar que solo aparecen las Abiertas. |
| AC-CS03-2 | El listado es paginado (12 items por pagina) con filtros aplicados en tiempo real (debounce 300ms). Los filtros y paginacion se envian como query params al backend. | Backend + Landing | Crear 20 necesidades Abiertas. Verificar que la pagina 1 retorna 12 y pagina 2 retorna el resto. Aplicar filtro y verificar debounce con network throttling. |
| AC-CS03-3 | Las necesidades cuya fecha limite esta a menos de 3 dias muestran badge de urgencia (calculado en frontend comparando con fecha actual). | Landing | Crear necesidad con `FechaLimitePropuestas = hoy + 2 dias`. Verificar badge "URGENTE" visible en listado y detalle. |
| AC-CS03-4 | Las necesidades expiradas (`FechaLimitePropuestas < hoy`) no se muestran en el listado publico. El filtro se aplica en el query del backend. | Backend | Crear necesidad con `FechaLimitePropuestas = ayer`. GET `/api/crowdsourcing/necesidades`. Verificar que no aparece. |
| AC-CS03-5 | Un profesional no puede enviar mas de una propuesta por necesidad. Si ya envio una, el campo `yaPropuso=true` en el detalle desactiva el boton, y el backend retorna 400 si intenta crear una segunda. | Backend + Landing | Enviar propuesta como usuario A a necesidad X. Verificar `yaPropuso=true` en detalle. Intentar POST de segunda propuesta y verificar 400. |
| AC-CS03-6 | El artista propietario de la necesidad no puede enviarse propuestas a si mismo. El campo `esPropietario=true` en el detalle desactiva el boton, y el backend retorna 403 si intenta enviar. | Backend + Landing | Autenticarse como artista propietario. GET detalle necesidad propia. Verificar `esPropietario=true`. Intentar POST propuesta y verificar 403. |
| AC-CS03-7 | La propuesta se crea con estado `Pendiente` y se vincula automaticamente el `UserId` desde JWT y el `PerfilProfesionalId` del usuario autenticado. Se muestra toast: "Propuesta enviada correctamente. El artista sera notificado." | Backend + Landing | Enviar propuesta valida. Verificar en BD que estado=Pendiente, UserId y PerfilProfesionalId correctos. Verificar toast en UI. |
| AC-CS03-8 | Si el profesional no tiene PerfilProfesional, se muestra CTA para crear perfil en lugar del boton "Enviar propuesta". El detalle de la necesidad sigue siendo visible. | Backend + Landing | Autenticarse con usuario sin PerfilProfesional. Navegar al detalle de necesidad. Verificar CTA visible. Verificar que el detalle de la necesidad se carga completo. |
| AC-CS03-9 | El profesional ve un listado paginado de sus propuestas con badge de estado: Pendiente (amarillo), Aceptada (verde), Rechazada (rojo), Retirada (gris). Se puede filtrar por estado. | Backend + Landing | Crear propuestas en los 4 estados. GET `/mis-propuestas`. Verificar badges con clases CSS correctas. Aplicar filtro por estado y verificar resultados. |
| AC-CS03-10 | Click en propuesta aceptada navega al detalle del acuerdo generado (enlace "Ver acuerdo"). Las propuestas en estado Pendiente muestran boton "Retirar propuesta". | Landing | Crear propuesta aceptada con acuerdo. Verificar enlace "Ver acuerdo". Crear propuesta pendiente. Verificar boton "Retirar". |
| AC-CS03-11 | Solo se puede retirar una propuesta en estado `Pendiente`, con dialogo de confirmacion previo. Tras confirmar, el estado cambia a "Retirada" y el artista deja de verla como candidato activo. | Backend + Landing | Retirar propuesta pendiente. Verificar dialogo confirmacion. Confirmar. Verificar estado=Retirada en BD. Verificar que desde perspectiva del artista la propuesta ya no aparece como activa. |
| AC-CS03-12 | Si no hay resultados en la busqueda de necesidades, se muestra empty state con sugerencia de ampliar filtros. | Landing | Aplicar filtros que no coincidan con ninguna necesidad. Verificar empty state visible con mensaje correcto. |
| AC-CS03-13 | El profesional puede ver el detalle de una necesidad aunque no tenga PerfilProfesional, pero necesita perfil para enviar propuesta. El endpoint GET `/necesidades/{id}` no requiere PerfilProfesional. | Backend + Landing | Autenticarse sin PerfilProfesional. GET detalle necesidad (debe retornar 200 con `tienePerfilProfesional=false`). Verificar detalle visible. Verificar que boton enviar muestra CTA en lugar de formulario. |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar Commands/Queries CQRS: `CreatePropuestaCommand`, `RetirarPropuestaCommand`, `GetNecesidadesPublicasQuery`, `GetNecesidadPublicaByIdQuery`, `GetMisPropuestasQuery`. Validators: `CreatePropuestaValidator` (precio, moneda, dias, mensaje, unicidad, ownership, necesidad abierta). Ampliar `INecesidadCrowdsourcingService` con query de listado publico. Nuevo `IPropuestaCrowdsourcingService`. Calcular campos `yaPropuso`, `esPropietario`, `tienePerfilProfesional` en el query de detalle. Filtrado server-side con paginacion (12/pagina). | ALTO |
| **Shared** | Definir TypeScript types para DTOs: `NecesidadPublicaListDto`, `NecesidadPublicaDetalleDto`, `PropuestaListDto`, `CreatePropuestaRequest`. Schemas Zod: `createPropuestaSchema`. Constants: `ESTADO_PROPUESTA` (Pendiente, Aceptada, Rechazada, Retirada), `BADGE_COLORS_PROPUESTA`, nuevas `QUERY_KEYS` para propuestas y necesidades publicas, `API_ROUTES` para los nuevos endpoints. | MEDIO |
| **Landing** | Implementar paginas y componentes: `ExplorarNecesidadesPage` (listado con filtros), `NecesidadDetallePage` (detalle + boton propuesta), `EnviarPropuestaForm` (formulario modal o pagina), `MisPropuestasPage` (listado con badges). Componentes: `NecesidadCard` (card listado), `NecesidadFilters` (filtros + debounce), `EnviarPropuestaForm` (react-hook-form + zod), `PropuestaCard` (card mis propuestas), `EstadoPropuestaBadge` (colores semanticos), `RetirarPropuestaDialog` (confirmacion), `EmptyStateNecesidades`, `EmptyStatePropuestas`. Hooks: `useNecesidadesPublicas`, `useNecesidadPublicaById`, `useCreatePropuesta`, `useMisPropuestas`, `useRetirarPropuesta`. Services: `necesidadPublica.service.ts`, `propuesta.service.ts`. Routing en Landing: `/crowdsourcing/necesidades`, `/crowdsourcing/necesidades/{id}`, `/crowdsourcing/mis-propuestas`. | ALTO |
| **Admin** | NO involucrado directamente. El artista ve las propuestas recibidas a sus necesidades desde la pantalla de detalle de necesidad (US-CS-02). | BAJO |

---

## Entidades Involucradas

### PropuestaCrowdsourcing (existente en dominio, activada en esta US)

Entidad principal para el flujo del profesional.

**Campos clave:**
- `Id` (Guid, PK)
- `NecesidadId` (Guid, FK -> NecesidadCrowdsourcing) - Necesidad a la que se propone
- `UserId` (string, FK -> Identity.User) - Asignado automaticamente desde JWT
- `PerfilProfesionalId` (Guid, FK -> PerfilProfesional) - Asignado automaticamente desde el perfil del usuario
- `PrecioPropuesto` (decimal, NOT NULL, > 0)
- `MonedaId` (int, FK -> MaestraMoneda)
- `DiasEstimados` (int, nullable, > 0, max 365)
- `MensajePropuesta` (string, NOT NULL, 20-2000 chars)
- `EstadoPropuestaId` (int, FK -> MaestraEstadoPropuesta) - Pendiente/Aceptada/Rechazada/Retirada
- `MotivoRechazo` (string, nullable) - Usado al rechazar o al auto-rechazar por cierre de necesidad
- `FechaCreacion` (DateTime, NOT NULL)
- `FechaActualizacion` (DateTime, nullable) - Se actualiza al cambiar estado

**Relaciones:**
- `Necesidad` (navigation property -> NecesidadCrowdsourcing)
- `Acuerdo` (navigation property -> AcuerdoCrowdsourcing, nullable)

**Constraint de unicidad:**
- Indice unico en (`NecesidadId`, `UserId`) - un usuario no puede enviar dos propuestas a la misma necesidad

### NecesidadCrowdsourcing (existente, leida en modo publico)

Utilizada en modo lectura para el listado publico y el detalle.

**Campos expuestos en listado publico:**
- `Id`, `Titulo`, `Descripcion` (truncada), `TipoNecesidadId`, `ModalidadTrabajoId`
- `PresupuestoMin`, `PresupuestoMax`, `MonedaId`
- `UbicacionCiudad`, `UbicacionPais`
- `ArtistaId` (para calcular `esPropietario`)
- `FechaCreacion`, `FechaLimitePropuestas`
- COUNT de propuestas activas

**Campos calculados retornados en detalle:**
- `yaPropuso`: EXISTS(SELECT 1 FROM PropuestaCrowdsourcing WHERE NecesidadId = @id AND UserId = @userId)
- `esPropietario`: necesidad.ArtistaId == artista del usuario autenticado
- `tienePerfilProfesional`: EXISTS(SELECT 1 FROM PerfilProfesional WHERE UserId = @userId)

---

## API Endpoints

### GET /api/crowdsourcing/necesidades

Listar necesidades abiertas (listado publico paginado con filtros).

**Auth:** Autenticado (JWT)

**Query params:**
- `tipoNecesidadId` (int, opcional) - Filtrar por tipo (multi-value: `?tipoNecesidadId=1&tipoNecesidadId=2`)
- `modalidad` (int, opcional) - Filtrar por modalidad de trabajo
- `presupuestoMin` (decimal, opcional) - Presupuesto minimo
- `presupuestoMax` (decimal, opcional) - Presupuesto maximo
- `pais` (string, opcional) - Filtrar por pais
- `ciudad` (string, opcional) - Filtrar por ciudad
- `orderBy` (string, opcional) - `recientes` (default), `presupuesto`, `limite`
- `search` (string, opcional) - Buscar en titulo y descripcion
- `page` (int, default 1) - Numero de pagina
- `pageSize` (int, default 12) - Items por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "titulo": "Mezcla de pistas para EP de 5 canciones",
        "descripcion": "Buscamos un ingeniero de mezcla experiment...",
        "tipoNecesidadNombre": "Post-produccion",
        "presupuestoMin": 150.00,
        "presupuestoMax": 800.00,
        "monedaNombre": "EUR",
        "modalidadTrabajoNombre": "Remoto",
        "modalidadTrabajoIcono": "wifi",
        "ubicacionCiudad": null,
        "ubicacionPais": null,
        "artistaNombre": "Los Rockeros",
        "fechaCreacion": "2026-02-15T10:30:00Z",
        "fechaRelativa": "hace 2 dias",
        "fechaLimitePropuestas": "2026-03-15",
        "esUrgente": false,
        "numeroPropuestas": 3
      }
    ],
    "totalCount": 25,
    "page": 1,
    "pageSize": 12,
    "totalPages": 3
  },
  "messages": []
}
```

**Errores:** Ninguno (lista vacia si no hay necesidades que cumplan filtros)

---

### GET /api/crowdsourcing/necesidades/{id}

Detalle de necesidad con campos calculados para el usuario autenticado.

**Auth:** Autenticado (JWT)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "titulo": "Mezcla de pistas para EP de 5 canciones",
    "descripcion": "Buscamos un ingeniero de mezcla experimentado para un EP de 5 canciones de rock alternativo...",
    "tipoNecesidadNombre": "Post-produccion",
    "presupuestoMin": 150.00,
    "presupuestoMax": 800.00,
    "monedaNombre": "EUR",
    "modalidadTrabajoNombre": "Remoto",
    "ubicacionCiudad": null,
    "ubicacionPais": null,
    "artista": {
      "id": "guid",
      "nombreArtistico": "Los Rockeros",
      "imagenUrl": "https://..."
    },
    "fechaCreacion": "2026-02-15T10:30:00Z",
    "fechaLimitePropuestas": "2026-03-15",
    "fechaInicioPrevista": "2026-04-01",
    "numeroPropuestas": 3,
    "yaPropuso": false,
    "esPropietario": false,
    "tienePerfilProfesional": true
  },
  "messages": []
}
```

**Errores:**
- `404 Not Found` - Necesidad no existe o no esta en estado Abierta (expiradas se tratan como no encontradas)

---

### POST /api/crowdsourcing/necesidades/{necesidadId}/propuestas

Enviar propuesta a una necesidad abierta.

**Auth:** Autenticado (JWT) + PerfilProfesional activo

**Request:**
```json
{
  "precioPropuesto": 450.00,
  "monedaId": 1,
  "diasEstimados": 14,
  "mensajePropuesta": "Soy ingeniero de mezcla con 10 anos de experiencia en rock alternativo. He trabajado con bandas como..."
}
```

**Response 201 Created:**
```json
{
  "data": {
    "id": "guid",
    "necesidadTitulo": "Mezcla de pistas para EP de 5 canciones",
    "precioPropuesto": 450.00,
    "estadoPropuestaNombre": "Pendiente",
    "fechaCreacion": "2026-02-20T14:00:00Z"
  },
  "messages": [
    { "message": "Propuesta enviada correctamente. El artista sera notificado.", "errorCode": "0001" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Validacion fallida (precio <= 0, mensaje muy corto, etc.) o ya tiene propuesta para esta necesidad
- `403 Forbidden` - No tiene PerfilProfesional activo, o es el artista propietario de la necesidad
- `404 Not Found` - Necesidad no existe o no esta en estado Abierta

---

### GET /api/crowdsourcing/propuestas/mis-propuestas

Listar propuestas del profesional autenticado (paginadas).

**Auth:** Autenticado (JWT)

**Query params:**
- `estado` (int, opcional) - Filtrar por EstadoPropuestaId (Pendiente=1, Aceptada=2, Rechazada=3, Retirada=4)
- `page` (int, default 1) - Numero de pagina
- `pageSize` (int, default 10) - Items por pagina

**Response 200 OK:**
```json
{
  "data": {
    "items": [
      {
        "id": "guid",
        "necesidadTitulo": "Mezcla de pistas para EP de 5 canciones",
        "artistaNombre": "Los Rockeros",
        "precioPropuesto": 450.00,
        "monedaNombre": "EUR",
        "estadoPropuestaId": 1,
        "estadoPropuestaNombre": "Pendiente",
        "fechaCreacion": "2026-02-20T14:00:00Z",
        "fechaActualizacion": null,
        "acuerdoId": null
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

**Errores:** Ninguno (lista vacia si no hay propuestas)

---

### PATCH /api/crowdsourcing/propuestas/{id}/retirar

Retirar propuesta propia en estado Pendiente.

**Auth:** Autenticado (JWT, propietario de la propuesta)

**Response 200 OK:**
```json
{
  "data": {
    "id": "guid",
    "estadoPropuestaNombre": "Retirada"
  },
  "messages": [
    { "message": "Propuesta retirada", "errorCode": "0002" }
  ]
}
```

**Errores:**
- `400 Bad Request` - Estado != Pendiente (mensaje: "Solo se pueden retirar propuestas en estado Pendiente")
- `403 Forbidden` - No es el propietario de la propuesta
- `404 Not Found` - Propuesta no existe

---

## Validaciones

### CreatePropuestaValidator

```csharp
public class CreatePropuestaCommandValidator : AbstractValidator<CreatePropuestaCommand>
{
    public CreatePropuestaCommandValidator(
        IPropuestaCrowdsourcingService propuestaService,
        INecesidadCrowdsourcingService necesidadService,
        IPerfilProfesionalService perfilService)
    {
        RuleFor(x => x.PrecioPropuesto)
            .GreaterThan(0)
            .WithMessage("El precio propuesto debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.DiasEstimados)
            .GreaterThan(0)
            .When(x => x.DiasEstimados.HasValue)
            .WithMessage("Los dias estimados deben ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange)
            .LessThanOrEqualTo(365)
            .When(x => x.DiasEstimados.HasValue)
            .WithMessage("El tiempo estimado no puede superar los 365 dias")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.MensajePropuesta)
            .NotEmpty()
            .WithMessage("El mensaje de propuesta es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(20)
            .WithMessage("El mensaje debe tener al menos 20 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(2000)
            .WithMessage("El mensaje no puede superar los 2000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // Unicidad: un usuario no puede tener dos propuestas para la misma necesidad
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var yaPropuso = await propuestaService.ExistePropuestaAsync(command.NecesidadId, command.UserId, ct);
                return !yaPropuso;
            })
            .WithMessage("Ya tienes una propuesta para esta necesidad")
            .WithErrorCode(ServiceResponseMessageType.Validation_DuplicateName);

        // Ownership: el artista propietario no puede enviarse propuesta a si mismo
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await necesidadService.GetByIdAsync(command.NecesidadId, ct);
                if (necesidad == null) return false;
                var artista = await perfilService.GetArtistaByUserIdAsync(command.UserId, ct);
                return artista == null || necesidad.ArtistaId != artista.Id;
            })
            .WithMessage("No puedes enviar propuesta a tu propia necesidad")
            .WithErrorCode(ServiceResponseMessageType.Auth_Forbidden);

        // Necesidad debe estar en estado Abierta y no expirada
        RuleFor(x => x.NecesidadId)
            .MustAsync(async (necesidadId, ct) =>
            {
                var necesidad = await necesidadService.GetByIdAsync(necesidadId, ct);
                return necesidad != null
                    && necesidad.EstadoNecesidadId == 1  // Abierta
                    && (!necesidad.FechaLimitePropuestas.HasValue || necesidad.FechaLimitePropuestas.Value >= DateTime.Today);
            })
            .WithMessage("La necesidad no existe, no esta abierta o ha expirado")
            .WithErrorCode(ServiceResponseMessageType.NotFound_Entity);
    }
}
```

### RetirarPropuestaValidator

```csharp
public class RetirarPropuestaCommandValidator : AbstractValidator<RetirarPropuestaCommand>
{
    public RetirarPropuestaCommandValidator(IPropuestaCrowdsourcingService propuestaService)
    {
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await propuestaService.GetByIdAsync(command.Id, ct);
                return propuesta != null && propuesta.EstadoPropuestaId == 1; // Pendiente
            })
            .WithMessage("Solo se pueden retirar propuestas en estado Pendiente")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_InvalidState);
    }
}
```

---

## Requisitos No Funcionales

### Performance
- Listado publico paginado debe cargar en < 500ms con 500 necesidades en BD
- Query de listado optimizado con proyecciones (no cargar entidades completas)
- Debounce de 300ms en frontend para evitar requests en cada keystroke
- Cache de maestras (tipos, estados, modalidades, monedas) via `IRequestCacheService` para evitar queries duplicados entre validator y handler
- Indice en BD para el filtrado: `(EstadoNecesidadId, FechaLimitePropuestas)` y `(NecesidadId, UserId)` para unicidad de propuestas

### Seguridad
- Validar ownership en `RetirarPropuestaCommand`: el usuario autenticado debe ser el dueno de la propuesta (403 si no)
- Validar en backend que el usuario tiene PerfilProfesional antes de crear propuesta (no confiar en frontend)
- Sanitizar inputs de texto (mensajePropuesta) para prevenir XSS
- El campo `yaPropuso` y `esPropietario` deben calcularse en backend (no aceptar del cliente)
- Rate limiting en envio de propuestas: max 20 propuestas por hora por usuario

### UX
- Formulario de propuesta con validacion en tiempo real (Zod en frontend)
- Nota informativa si precio fuera del rango del artista (no bloqueante)
- Badge "URGENTE" visible en cards del listado para necesidades con fecha limite < 3 dias
- Empty state con ilustracion y CTAs en listado de necesidades y mis propuestas
- Dialogo de confirmacion antes de retirar propuesta (accion irreversible)
- Toast en todas las acciones: enviar propuesta, retirar propuesta
- Badges de estado con colores semanticos en "Mis propuestas"

### Mantenibilidad
- Maestras de estado propuesta en BD (no hardcoded en codigo): Pendiente (1), Aceptada (2), Rechazada (3), Retirada (4)
- Logs de auditoria: quien envio/retiro cada propuesta (timestamp + userId)
- Campos `yaPropuso`, `esPropietario`, `tienePerfilProfesional` calculados via subconsultas en el mismo query de detalle (no multiples roundtrips)
- Constraint de unicidad en BD (`NecesidadId`, `UserId`) como ultima linea de defensa

---

## Dependencias

### Tecnicas (dentro del proyecto)
- **Modulo UserAccess**: Entidad `Artista` (para calcular `esPropietario`), `PerfilProfesional` (para validar perfil y calcular `tienePerfilProfesional`)
- **Modulo Crowdsourcing**: Entidad `NecesidadCrowdsourcing` (existente, US-CS-02), Entidad `PropuestaCrowdsourcing` (existente en dominio, activada en esta US), `AcuerdoCrowdsourcing` (navegacion desde propuesta aceptada)
- **Maestras**: `MaestraTipoNecesidad`, `MaestraModalidadTrabajo`, `MaestraEstadoPropuesta`, `MaestraMoneda`
- **Shared**: Types, schemas Zod, constants para estados propuesta y QUERY_KEYS
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType`, `IRequestCacheService`

### De otras User Stories
- **US-CS-02** (Gestionar Necesidades): Prerequisito directo. Sin necesidades en estado "Abierta" no hay nada que explorar. El listado publico consume el mismo conjunto de datos gestionados por el artista en US-CS-02.

### Dependencias futuras (que esta US habilita)
- **US-CS-04** (Seleccion de profesional y acuerdo): Requiere propuestas en estado "Pendiente" para que el artista pueda aceptar y crear acuerdo. El campo `acuerdoId` en `MisPropuestasDto` y el enlace "Ver acuerdo" estan preparados para esta US.

---

## Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Propuestas duplicadas por doble click o reenvio de formulario | Media | Medio | Constraint de unicidad en BD (NecesidadId, UserId). Deshabilitar boton en frontend tras primer submit. Validator en backend verifica antes de insertar. |
| Artista se envia propuesta a si mismo mediante llamada directa a API | Baja | Medio | Validacion en backend en `CreatePropuestaValidator` calculando si el usuario es el artista propietario. No depender solo del frontend. |
| Queries lentos en listado publico con miles de necesidades | Media | Medio | Paginacion obligatoria (12/pagina). Indices en BD para filtros frecuentes. Proyecciones optimizadas (select solo campos necesarios). |
| Necesidad expira mientras el profesional esta escribiendo la propuesta | Baja | Bajo | Validacion en backend al momento del submit. Mensaje de error claro: "La necesidad ha expirado". Frontend puede mostrar fecha limite con cuenta regresiva. |
| Profesional retira propuesta accidentalmente | Media | Medio | Dialogo de confirmacion obligatorio con texto explicativo. Boton "Cancelar" prominente. La accion es irreversible (puede crear nueva propuesta si quiere). |
| Carga de campos calculados (yaPropuso, esPropietario, tienePerfilProfesional) aumenta latencia del detalle | Baja | Bajo | Calcular los tres campos en una sola query con subconsultas SQL. Usar `IRequestCacheService` para reusar datos entre validator y handler en la misma request. |

---

## Notas de Implementacion

### Backend
- Crear Commands/Queries en `Modules/Crowdsourcing/Crowdsourcing.Application/Features/Propuestas/`
  - Commands: `CreatePropuestaCommand.cs`, `RetirarPropuestaCommand.cs`
  - Queries: `GetNecesidadesPublicasQuery.cs`, `GetNecesidadPublicaByIdQuery.cs`, `GetMisPropuestasQuery.cs`
  - Validators: `CreatePropuestaValidator.cs`, `RetirarPropuestaValidator.cs`
- Implementar Service: `PropuestaCrowdsourcingService : IPropuestaCrowdsourcingService`
  - Metodos: `GetByIdAsync`, `ExistePropuestaAsync` (unicidad), `CreateAsync`, `RetirarAsync`, `GetByUserIdAsync` (paginado)
- Ampliar `INecesidadCrowdsourcingService` con:
  - `GetPublicasAsync` (listado con filtros y paginacion, solo Abiertas y no expiradas)
  - `GetPublicaByIdAsync` (detalle publico, incluye campos calculados segun UserId)
- AutoMapper Profile: ampliar `NecesidadProfile.cs` con mapping a `NecesidadPublicaListDto` y `NecesidadPublicaDetalleDto`. Crear `PropuestaProfile.cs`.
- Controller: ampliar `NecesidadesController` con GET publico. Crear `PropuestasController`.
- Campos calculados en `GetNecesidadPublicaByIdQuery`:
  1. `yaPropuso`: EXISTS query sobre PropuestaCrowdsourcing para (NecesidadId, UserId)
  2. `esPropietario`: comparar ArtistaId de la necesidad con ArtistaId del usuario autenticado
  3. `tienePerfilProfesional`: EXISTS query sobre PerfilProfesional para UserId
  - Calcular los tres en el mismo query SQL para evitar multiples roundtrips a BD

### Frontend (Landing)
- Implementar paginas:
  - `ExplorarNecesidadesPage.tsx` (`/crowdsourcing/necesidades`)
  - `NecesidadDetallePage.tsx` (`/crowdsourcing/necesidades/:id`)
  - `MisPropuestasPage.tsx` (`/crowdsourcing/mis-propuestas`)
- Componentes reutilizables:
  - `NecesidadCard.tsx` (card del listado con badge urgencia, datos, numero propuestas)
  - `NecesidadFilters.tsx` (filtros con debounce 300ms integrado)
  - `EnviarPropuestaForm.tsx` (formulario react-hook-form + zod, nota informativa si precio fuera de rango)
  - `PropuestaCard.tsx` (card en mis propuestas con badge estado y acciones)
  - `EstadoPropuestaBadge.tsx` (badge con color segun estado: amarillo/verde/rojo/gris)
  - `RetirarPropuestaDialog.tsx` (dialogo confirmacion)
  - `EmptyStateNecesidades.tsx` (ilustracion + mensaje + sugerencia ampliar filtros)
  - `EmptyStatePropuestas.tsx` (ilustracion + mensaje si no hay propuestas)
  - `PerfilProfesionalCTA.tsx` (CTA para crear perfil cuando `tienePerfilProfesional=false`)
- Hooks:
  - `useNecesidadesPublicas.ts` (useQuery con filtros y paginacion, debounce integrado)
  - `useNecesidadPublicaById.ts` (useQuery para detalle con campos calculados)
  - `useCreatePropuesta.ts` (useMutation)
  - `useMisPropuestas.ts` (useQuery con filtro estado y paginacion)
  - `useRetirarPropuesta.ts` (useMutation)
- Services:
  - `necesidadPublica.service.ts` (GET listado y detalle publico)
  - `propuesta.service.ts` (POST crear, GET mis propuestas, PATCH retirar)
- El badge de urgencia (< 3 dias) se calcula en el frontend: `diferenciaDias(fechaLimite, hoy) < 3`
- El listado publico excluye las necesidades propias del artista autenticado (el backend puede filtrar `ArtistaId != artistaDelUsuario` o el frontend puede omitirlas)

### Testing
- **Unit tests:**
  - Validators: `CreatePropuestaValidatorTests` (unicidad, ownership, necesidad abierta, campos requeridos)
  - Validators: `RetirarPropuestaValidatorTests` (estado pendiente, ownership)
  - Queries: `GetNecesidadesPublicasQueryHandlerTests` (filtros, paginacion, solo Abiertas)
- **Integration tests:**
  - Flujo completo: explorar listado -> ver detalle -> enviar propuesta -> verificar en BD
  - Flujo: enviar propuesta -> retirar propuesta -> verificar estado
  - Verificar que necesidades expiradas no aparecen en listado
  - Verificar que artista propietario recibe 403 al intentar proponer
- **E2E tests:**
  - Explorar listado con filtros (tipo, modalidad, presupuesto)
  - Ver detalle de necesidad sin perfil profesional (CTA visible)
  - Enviar propuesta con datos validos (toast de exito)
  - Intentar enviar segunda propuesta a la misma necesidad (error)
  - Ver mis propuestas con badges de estado
  - Retirar propuesta pendiente (dialogo confirmacion)

---

## Out of Scope (No incluido en esta feature)

- Notificaciones push o email al artista cuando recibe una propuesta
- Chat o mensajes entre artista y profesional sobre la propuesta
- Edicion de propuesta una vez enviada (el profesional debe retirar y crear una nueva)
- Valoracion o resenas del profesional por parte del artista
- Sistema de favoritos de necesidades para el profesional
- Perfil publico del profesional visible desde el card de necesidad
- Busqueda de necesidades sin autenticacion (el listado requiere JWT)
- Notificacion al profesional cuando su propuesta es rechazada o aceptada (US futura)
- Navegacion al acuerdo desde propuesta aceptada (preparado en UI, pero el acuerdo pertenece a US-CS-04)
- Moderacion de contenido de mensajes de propuesta
- Analytics sobre propuestas (conversion rate, precio promedio, tiempo de respuesta)

---

## Referencia

- User Story completa: [docs/product/US-CS-03-explorar-propuestas.md](../../product/US-CS-03-explorar-propuestas.md)
- US-CS-02 Gestionar Necesidades: [docs/user-stories/cs-gestionar-necesidades/feature-spec.md](../cs-gestionar-necesidades/feature-spec.md)
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
- CLAUDE.md: Patron CQRS, reglas de validacion, ServiceResponse
