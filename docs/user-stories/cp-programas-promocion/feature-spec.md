# Feature: Programas de Promocion

> **ID:** cp-programas-promocion
> **User Story:** US-CP-02
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature permite a un artista crear y gestionar programas de promocion dentro del modulo Crowdpromotion. Un programa de promocion es el nucleo del modulo: define el tipo de difusion que el artista ofrece (referral, afiliado, influencer, mixto), las comisiones que pagara a los promotores por cada conversion o accion, y el conjunto de tareas concretas que los promotores deben realizar para recibir esas recompensas.

El flujo de creacion se implementa como un wizard de cuatro pasos: datos basicos del programa, configuracion de comisiones, definicion de tareas y revision final antes de publicar. Esto permite al artista construir de forma guiada las "reglas del juego" que luego los promotores veran al postularse. La creacion del programa y sus tareas se ejecuta en una sola transaccion para garantizar consistencia: o se crea todo o no se crea nada.

Ademas de la creacion, la feature incluye los flujos de listado, edicion y desactivacion logica de programas. Al desactivar un programa, todas sus tareas quedan desactivadas en cascada y el programa deja de aparecer en el catalogo publico para nuevas postulaciones, aunque los registros historicos de promotores y eventos se conservan intactos.

---

## User Story

**Como** artista con una campana de crowdfunding activa
**Quiero** crear un programa de promocion donde defina el tipo de programa, las comisiones por conversion, la URL de destino y las tareas que deben completar los promotores
**Para** que fans e influencers difundan mi campana a cambio de recompensas claras y medibles

---

## Flujo Principal: Crear Programa de Promocion

```
1.  Artista autenticado accede a /crowdpromotion/programas/nuevo en Admin
2.  Sistema presenta wizard en Paso 1: Datos Basicos
3.  Artista completa titulo, descripcion, tipo de programa, campana o proyecto
    vinculado (opcional), URL landing, codigo tracking base y fechas de vigencia
4.  Artista avanza al Paso 2: Configurar Comisiones
5.  Artista selecciona moneda y define al menos una de las dos comisiones:
    importe por porcentaje o importe fijo por conversion
6.  Artista avanza al Paso 3: Definir Tareas
7.  Artista agrega N tareas al programa; para cada tarea define nombre, tipo de
    evento, tipo de recompensa, importe/puntos, si es repetible y max repeticiones
8.  Artista avanza al Paso 4: Revisar y Publicar
9.  Sistema muestra resumen completo del programa (datos + comisiones + tareas)
10. Artista confirma la publicacion
11. Frontend valida los datos del wizard con schemas Zod antes de enviar
12. Backend recibe el payload completo, valida con FluentValidation y verifica
    que la campana o proyecto indicados pertenecen al artista autenticado
13. Backend crea PromoPrograma (EsActivo = true) y N PromoTarea (EsActivo = true)
    en una unica transaccion
14. Backend retorna el programa creado con el conteo de tareas
15. Frontend muestra toast: "Programa de promocion creado"
16. Sistema redirige al artista a la pagina de detalle del programa
```

---

## Flujos Secundarios

### Listar Mis Programas

```
1. Artista accede a /crowdpromotion/programas en Admin
2. Sistema consulta los programas del artista autenticado
3. Si no tiene programas → mostrar empty state con boton "Crear programa"
4. Si tiene programas → mostrar listado paginado con filtro por estado (todos /
   activos / inactivos)
5. Cada fila muestra: titulo, tipo, badge de estado, campana vinculada,
   numero de promotores aprobados, numero de tareas activas, comision y fechas
6. Artista puede hacer click en un programa para ver su detalle completo
```

### Editar Programa

```
1. Artista hace click en "Editar" en el listado o en el detalle del programa
2. Sistema carga el formulario pre-rellenado con los datos actuales
3. Si el programa tiene promotores inscritos → mostrar aviso informativo
   indicando cuantos promotores ya estan inscritos
4. Artista modifica los campos del programa y/o sus tareas
5. Artista puede agregar nuevas tareas, editar tareas existentes o
   desactivar tareas (no se pueden eliminar tareas con completados)
6. Sistema valida los datos en frontend y en backend
7. Backend actualiza PromoPrograma y las PromoTarea afectadas
8. Frontend muestra toast: "Programa actualizado"
```

### Desactivar Programa

```
1. Artista hace click en "Desactivar" en el detalle del programa
2. Sistema muestra dialogo de confirmacion con el nombre del programa
3. Artista confirma la desactivacion
4. Backend establece EsActivo = false en PromoPrograma
5. Backend establece EsActivo = false en todas las PromoTarea del programa
6. El programa deja de aparecer en el catalogo publico para nuevas inscripciones
7. Los registros de promotores inscritos y eventos existentes se conservan
8. Frontend muestra toast: "Programa desactivado"
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Artista no tiene campanas de crowdfunding | Permitir crear el programa sin vincular campana; los campos de campana y proyecto quedan vacios |
| FA-02 | Artista no define ninguna tarea en el wizard | Permitir crear el programa solo con datos basicos y comisiones; las tareas se pueden agregar despues via edicion |
| FA-03 | Fecha fin < fecha inicio | Mostrar error de validacion en el campo de fecha fin antes de avanzar de paso |
| FA-04 | Ni comision porcentaje ni comision fija definidas | Bloquear avance al Paso 3 con mensaje: "Debe definir al menos una comision (porcentaje o fija)" |
| FA-05 | Codigo tracking base duplicado para el mismo artista | Retornar error de negocio desde backend con mensaje descriptivo |
| FA-06 | Tarea con EsRepetible = true sin MaxRepeticiones | Mostrar error de validacion: "Las tareas repetibles requieren indicar el maximo de repeticiones" |
| FA-07 | Campana o proyecto indicados no pertenecen al artista | Backend retorna 403 Forbidden |
| FA-08 | Error de red durante la publicacion | Mostrar toast de error, mantener el estado del wizard para reintentar |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP02-1 | El artista puede crear un programa completando el wizard de 4 pasos; el registro se almacena en BD con PromoPrograma y sus PromoTarea asociadas | Backend + Admin |
| AC-CP02-2 | El programa se crea con EsActivo = true y todas sus tareas con EsActivo = true | Backend |
| AC-CP02-3 | El ArtistaId se asigna automaticamente desde el token JWT del usuario autenticado; no puede ser indicado por el cliente | Backend |
| AC-CP02-4 | Se valida que al menos una de las dos comisiones (ImporteComisionPorcentaje o ImporteComisionFija) este definida; enviar sin ninguna retorna error de validacion | Backend + Shared |
| AC-CP02-5 | El artista ve un listado paginado de sus programas con badge de estado, contadores de promotores y tareas, y filtro por estado activo/inactivo | Backend + Admin |
| AC-CP02-6 | Se puede editar un programa y sus tareas; si el programa tiene promotores inscritos se muestra un aviso informativo (no bloquea la edicion) | Backend + Admin |
| AC-CP02-7 | Al desactivar un programa, su EsActivo pasa a false y todas sus PromoTarea activas pasan tambien a EsActivo = false en la misma operacion | Backend |
| AC-CP02-8 | El CodigoTrackingBase es unico por artista; intentar crear un segundo programa con el mismo codigo retorna error de negocio | Backend |
| AC-CP02-9 | Una tarea con EsRepetible = true sin MaxRepeticiones >= 1 retorna error de validacion; una tarea con EsRepetible = false no requiere MaxRepeticiones | Backend + Shared |
| AC-CP02-10 | Se puede crear un programa sin vincular ninguna CampaniaCrowdfunding ni ProyectoArtistico; la ausencia de ambos no es un error | Backend + Admin |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar los cinco endpoints del modulo Crowdpromotion para PromoPrograma: POST crear, GET listar mis programas, GET detalle, PUT editar y PATCH desactivar. La creacion del programa con sus tareas debe ser transaccional. Validar con FluentValidation incluyendo la regla de al menos una comision, unicidad de CodigoTrackingBase por artista y pertenencia de la campana/proyecto al artista. Aplicar migraciones de EF Core para los campos nuevos de PromoPrograma y PromoTarea. | ALTO |
| **Admin** | Implementar el wizard de 4 pasos en /crowdpromotion/programas/nuevo (Next.js 14). Implementar pagina de listado /crowdpromotion/programas con filtro y paginacion. Implementar pagina de detalle y formulario de edicion de programa. Mostrar dialogo de confirmacion para la desactivacion. Gestionar el estado del wizard en React (no persistir en servidor entre pasos). | ALTO |
| **Landing** | Sin responsabilidad directa en esta feature. El catalogo publico de programas para promotores es parte de US-CP-03. | BAJO |
| **Shared** | Definir tipos TypeScript para PromoPrograma, PromoTarea, MaestraTipoPromo, MaestraTipoEventoPromo, MaestraTipoReward y los DTOs de creacion y actualizacion. Definir schemas Zod para cada paso del wizard incluyendo la regla de al menos una comision y la regla de EsRepetible con MaxRepeticiones. | ALTO |

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity activo con JWT: el ArtistaId se extrae del claim del token en el handler
- Tablas seed cargadas antes de la primera ejecucion: `Maestra_TipoPromo`, `Maestra_TipoEventoPromo`, `Maestra_TipoReward`, `Maestra_Moneda`
- Migracion de EF Core requerida para campos nuevos en `PromoPrograma` y `PromoTarea` (ver Notas Tecnicas)
- Modulo Crowdpromotion con DbContext, entidades base y migraciones iniciales ya aplicadas

### De otras features

- **cp-perfil-promotor (US-CP-01):** debe estar implementada; el dominio del modulo Crowdpromotion (entidades, DbContext, migraciones iniciales) se establece en esa feature. Los programas se crean en el mismo modulo.

---

## Notas Tecnicas

### Cambios de Modelo de Dominio Requeridos

La entidad `PromoPrograma` existente en el codebase no incluye todos los campos definidos en la User Story. Antes de implementar los endpoints es necesario:

**PromoPrograma - campos a agregar:**
- `public string? UrlLanding { get; set; }` (max 500, formato URL)
- `public string? CodigoTrackingBase { get; set; }` (max 50, solo alfanumerico y guiones)
- `public decimal? ImporteComisionPorcentaje { get; set; }` (precision 5,2; rango 0-100)
- `public decimal? ImporteComisionFija { get; set; }` (precision 18,2; >= 0)

El campo `ComisionPorConversion` existente no es equivalente a `ImporteComisionFija` (el primero no tiene la semantica de comision fija definida en la US). El campo `ComisionPorClick` no es parte de esta US. Ambos campos existentes pueden coexistir o ser revisados en una tarea separada.

**PromoTarea - campos a agregar:**
- `public int TipoEventoPromoId { get; set; }` (FK a MaestraTipoEventoPromo, obligatorio)
- `public bool EsRepetible { get; set; }` (default true)
- `public int? MaxRepeticiones { get; set; }` (>= 1 cuando EsRepetible = true)
- `public DateTime? FechaInicio { get; set; }`
- `public DateTime? FechaFin { get; set; }`

El campo `Titulo` existente en `PromoTarea` se usa como `Nombre` en la User Story; en el contrato de la API se expone como `nombre` pero el campo de dominio permanece como `Titulo` salvo decision contraria.

Todos estos cambios son aditivos (columnas nullable o con default) y no rompen registros existentes. Se debe generar y aplicar una nueva migracion de EF Core.

### Transaccionalidad

La creacion del programa y sus tareas debe ejecutarse en una unica transaccion de base de datos. Si falla la creacion de cualquier tarea, se revierte la creacion del programa. El handler CQRS recibe el comando completo (programa + lista de tareas) y delega en IPromoProgramaService.

### Patron CQRS

- Command + Handler en el mismo archivo por cada operacion (crear, actualizar, desactivar)
- Los handlers inyectan IPromoProgramaService, no DbContext directamente
- El servicio gestiona la transaccion y llama al repositorio
- Validators con `ServiceResponseMessageType` constants, no strings literales

### Unicidad del CodigoTrackingBase

La unicidad es por artista, no global. La validacion debe hacerse en el validator consultando el servicio para verificar si ya existe un programa con ese codigo para el mismo ArtistaId.

---

## Referencia

- User Story completa: [docs/product/US-CP-02-programas-promocion.md](../../product/US-CP-02-programas-promocion.md)
