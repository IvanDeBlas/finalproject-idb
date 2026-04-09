# Feature: Templates y Guia para Artistas Noveles

> **ID:** cs-templates-guia
> **User Story:** US-CS-01
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** TBD

---

## Descripcion

Esta feature transforma a WePlay Rises en un **mentor digital** para artistas noveles que no conocen la industria musical. En lugar de enfrentar a un artista con un formulario vacio y preguntarle "que necesitas", le ofrecemos una galeria de templates de proyectos musicales pre-configurados (por ejemplo: "Grabar un Album", "Produccion de Videoclip", "Gira/Tour") con un desglose completo de todas las necesidades profesionales que tendra, roles recomendados, y precios orientativos de mercado.

El artista selecciona el tipo de proyecto que quiere realizar, personaliza las necesidades (marca/desmarca segun su interes, ajusta presupuestos), y al confirmar, el sistema crea automaticamente multiples `NecesidadCrowdsourcing` pre-rellenadas y vinculadas a su `ProyectoArtistico`. Esto reduce drasticamente la friccion para artistas noveles y diferencia a la plataforma de marketplaces genericos.

La feature incluye 6 templates predefinidos con datos seed (desde "Grabar un Album" hasta "Crear Presencia Online"), un catalogo de ~35 roles profesionales organizados en 6 categorias, y tooltips educativos que explican "que hace cada profesional y por que lo necesitas".

---

## User Story

**Como** artista novel que no conoce la industria musical,
**Quiero** seleccionar un tipo de proyecto (ej: "Grabar un album") y ver automaticamente un desglose de todas las necesidades profesionales que tendre, con roles recomendados y precios orientativos,
**Para** poder planificar mi presupuesto y publicar las necesidades que me interesen directamente en la plataforma sin necesidad de saber como funciona la industria.

---

## Flujo Principal

1. Artista autenticado accede a `/crowdsourcing/nuevo-proyecto` desde la aplicacion Landing/Web
2. **Paso 1 - Seleccionar template de proyecto:**
   - Ve una galeria de 6 templates como cards interactivos
   - Cada card muestra: icono, nombre del proyecto, descripcion breve, rango de precio total orientativo (min-max EUR), cantidad de necesidades
   - El artista hace click en un template para seleccionarlo
3. **Paso 2 - Personalizar necesidades:**
   - Ve el desglose completo del template organizado por fases (Pre-produccion, Grabacion, Post-produccion, etc.)
   - Cada necesidad muestra: titulo, descripcion explicativa, rol profesional recomendado con icono de tooltip, rango de precio orientativo, prioridad (Esencial/Recomendado/Opcional)
   - Las necesidades marcadas como "Esencial" vienen pre-seleccionadas (checkbox activado)
   - El artista puede marcar/desmarcar necesidades segun su interes
   - El artista puede ajustar el presupuesto (min-max) de cada necesidad seleccionada mediante inputs editables
   - Un resumen en tiempo real muestra el coste total estimado (suma de rangos min y max de necesidades seleccionadas)
   - Al hacer hover sobre el icono de tooltip de un rol profesional, se muestra una descripcion de "que hace este profesional y por que lo necesitas"
4. **Paso 3 - Confirmar y publicar:**
   - Ve un resumen final con: nombre del template, cantidad de necesidades seleccionadas, lista de necesidades a crear, presupuesto total estimado (min-max)
   - Confirma la creacion haciendo click en "Confirmar y publicar"
5. El sistema ejecuta el comando `GenerarNecesidadesDesdeTemplateCommand`:
   - Crea N registros de `NecesidadCrowdsourcing` en una sola transaccion
   - Cada necesidad se pre-rellena con: titulo, descripcion, tipo de necesidad, rango de presupuesto personalizado, modalidad de trabajo, estado "Abierta"
   - Todas las necesidades se vinculan al mismo `ProyectoArtistico` del artista (FK)
6. El sistema redirige al artista a su listado de necesidades activas donde puede ver las necesidades creadas y recibir postulaciones de profesionales

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Artista no tiene ProyectoArtistico creado | El sistema crea automaticamente un ProyectoArtistico con nombre por defecto o redirige a wizard de creacion |
| FA-02 | Artista deselecciona todas las necesidades | El boton "Confirmar y publicar" se deshabilita y se muestra mensaje: "Debe seleccionar al menos una necesidad" |
| FA-03 | Artista quiere volver al paso anterior | Los botones "< Atras" permiten navegacion entre pasos sin perder datos (estado en React) |
| FA-04 | Suma de presupuestos excede un umbral razonable | Solo se muestra un mensaje informativo, no se bloquea la creacion |
| FA-05 | Template no tiene necesidades activas (todas desactivadas) | El template no aparece en la galeria (filtro en backend) |
| FA-06 | Error al crear necesidades (BD, validacion) | Se muestra mensaje de error, se rollback la transaccion, el artista puede reintentar |

---

## Criterios de Aceptacion

| ID | Criterio | Metodo de Prueba |
|----|----------|------------------|
| AC-CS01-1 | El artista ve una galeria de 6 templates con cards que muestran icono, nombre, descripcion breve, rango de precio total (min-max EUR) y cantidad de necesidades. Puede seleccionar un template haciendo click en el card. | Navegar a `/crowdsourcing/nuevo-proyecto` como artista autenticado. Verificar que se muestran 6 cards con informacion completa. Hacer click en un card y verificar navegacion a Paso 2. |
| AC-CS01-2 | Al seleccionar un template, se muestra un desglose por fases con cada necesidad. Cada necesidad muestra: titulo, descripcion, rol profesional recomendado, rango de precio orientativo (min-max), y prioridad visual (Esencial/Recomendado/Opcional). | Seleccionar template "Grabar un Album". Verificar que se muestran 11 necesidades organizadas en 6 fases. Verificar que cada necesidad tiene titulo, descripcion, rol profesional, precio min-max, y badge de prioridad. |
| AC-CS01-3 | El artista puede marcar/desmarcar necesidades mediante checkboxes. Las necesidades marcadas como "Esencial" vienen seleccionadas por defecto. Las necesidades "Recomendado" y "Opcional" vienen deseleccionadas. | En Paso 2, verificar que checkboxes de necesidades "Esencial" estan activados. Verificar que "Recomendado"/"Opcional" estan desactivados. Marcar/desmarcar checkboxes y verificar que el estado cambia. |
| AC-CS01-4 | El artista puede ajustar el presupuesto (min y max) de cada necesidad seleccionada mediante inputs editables. El precio orientativo del template es solo una guia inicial, no un limite. El campo max debe ser >= min (validacion en frontend y backend). | Seleccionar una necesidad. Modificar los valores de presupuesto min y max. Verificar que los cambios se reflejan en el resumen. Intentar poner max < min y verificar mensaje de error. |
| AC-CS01-5 | Al confirmar, se crean automaticamente N registros de `NecesidadCrowdsourcing` (donde N = cantidad de necesidades seleccionadas) en estado "Abierta", pre-rellenadas con: titulo, descripcion, tipo de necesidad, rango de presupuesto personalizado, modalidad de trabajo derivados del template. | Confirmar en Paso 3. Consultar en base de datos `NecesidadCrowdsourcing` y verificar que se crearon N registros con campos pre-rellenados correctamente. Verificar que Estado = "Abierta". |
| AC-CS01-6 | Todas las necesidades generadas se vinculan al mismo `ProyectoArtistico` del artista mediante FK `ProyectoArtisticoId`. Si el artista no tiene proyecto, se crea uno automaticamente. | Verificar en BD que todas las necesidades creadas tienen el mismo `ProyectoArtisticoId`. Verificar que el ID corresponde a un proyecto del artista autenticado. |
| AC-CS01-7 | Antes de confirmar (Paso 3), se muestra un resumen con: template seleccionado, cantidad de necesidades seleccionadas vs total, lista de necesidades a crear, coste total estimado (suma de rangos min y max de las necesidades seleccionadas). | En Paso 3, verificar que se muestra: nombre template, "X de Y necesidades seleccionadas", lista de necesidades con precios, suma total min y suma total max calculadas correctamente. |
| AC-CS01-8 | Las 6 plantillas descritas (Grabar Album/EP, Videoclip, Gira/Tour, Marketing, Single, Branding) vienen pre-cargadas como datos de seed en la base de datos con todas sus necesidades, roles profesionales, precios orientativos y prioridades. | Ejecutar migraciones en BD limpia. Consultar tablas `PlantillaProyecto`, `PlantillaProyectoNecesidad`, `MaestraRolProfesional`, `MaestraCategoriaRol`. Verificar que existen 6 templates con ~55 necesidades total y ~35 roles profesionales distribuidos en 6 categorias. |
| AC-CS01-9 | Cada rol profesional tiene un icono de ayuda (tooltip) que al hacer hover muestra una descripcion de "que hace este profesional y por que lo necesitas". La descripcion viene de la tabla `MaestraRolProfesional.Descripcion`. | Hacer hover sobre el icono de tooltip de un rol profesional (ej: "Productor musical"). Verificar que se muestra tooltip con descripcion educativa. Verificar que el texto viene de la BD. |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Crear nuevas entidades (`PlantillaProyecto`, `PlantillaProyectoNecesidad`, `MaestraRolProfesional`, `MaestraCategoriaRol`, `MaestraTipoEmpresa`). Implementar Commands/Queries CQRS: `GetTemplatesQuery`, `GetTemplateByIdQuery`, `GenerarNecesidadesDesdeTemplateCommand`. Crear datos seed para 6 templates con ~55 necesidades y ~35 roles profesionales. Validacion de negocio. | ALTO |
| **Shared** | Definir TypeScript types para DTOs: `PlantillaProyectoDto`, `PlantillaProyectoNecesidadDto`, `MaestraRolProfesionalDto`, `MaestraCategoriaRolDto`, `GenerarNecesidadesRequest`, `GenerarNecesidadesResponse`. Schemas Zod para validacion de formularios. Constants para estados, prioridades. | MEDIO |
| **Landing/Web** | Implementar wizard de 3 pasos: Paso1 (galeria templates), Paso2 (personalizar necesidades), Paso3 (confirmar y publicar). Componentes: `TemplateGallery`, `TemplateNecesidadesForm`, `NecesidadCheckbox`, `PresupuestoInput`, `ResumenPresupuestario`, `RolProfesionalTooltip`. Hooks: `useTemplates`, `useGenerarNecesidades`. Routing en `/crowdsourcing/nuevo-proyecto`. | ALTO |
| **Admin** | CRUD para gestionar templates y maestras (futuro). En MVP solo lectura desde seed. Opcional: Interfaz para editar precios orientativos y activar/desactivar templates o necesidades. | BAJO (no MVP) |

---

## Entidades Nuevas

### PlantillaProyecto

Representa un tipo de proyecto musical pre-configurado (ej: "Grabar un Album").

**Campos clave:**
- `Id` (Guid, PK)
- `Nombre` (string, NOT NULL) - ej: "Grabar un Album / EP"
- `Descripcion` (string, nullable) - Texto explicativo para artista
- `Icono` (string, nullable) - Nombre del icono (ej: "music", "video")
- `Orden` (int) - Orden de presentacion en UI
- `Activo` (bool, default true) - Para ocultar sin borrar
- `FechaCreacion` (DateTime)
- Relacion: `ICollection<PlantillaProyectoNecesidad>`

### PlantillaProyectoNecesidad

Representa una necesidad profesional dentro de un template (ej: "Mezcla de pistas" en template "Grabar Album").

**Campos clave:**
- `Id` (Guid, PK)
- `PlantillaProyectoId` (Guid, FK -> PlantillaProyecto)
- `Fase` (string, NOT NULL) - ej: "Pre-produccion", "Grabacion"
- `Titulo` (string, NOT NULL) - ej: "Mezcla de pistas"
- `Descripcion` (string, nullable) - Explicacion para artista novel
- `RolProfesionalId` (int, FK -> MaestraRolProfesional)
- `PrecioMinOrientativo` (decimal, nullable)
- `PrecioMaxOrientativo` (decimal, nullable)
- `MonedaId` (int, FK -> MaestraMoneda)
- `Prioridad` (string, NOT NULL) - 'Esencial', 'Recomendado', 'Opcional'
- `Orden` (int) - Orden dentro del template
- `FechaCreacion` (DateTime)
- Relaciones: `PlantillaProyecto`, `MaestraRolProfesional`

### MaestraRolProfesional

Catalogo de roles profesionales en la industria musical (ej: "Productor musical", "Ingeniero de mezcla").

**Campos clave:**
- `Id` (int, PK)
- `Nombre` (string, NOT NULL) - ej: "Productor musical"
- `Descripcion` (string, nullable) - Que hace este rol (para tooltip)
- `CategoriaRolId` (int, FK -> MaestraCategoriaRol)
- `ModalidadCobro` (string, nullable) - ej: "Por cancion", "Por dia"
- `Activo` (bool, default true)
- Relacion: `MaestraCategoriaRol`

### MaestraCategoriaRol

Categorias de roles profesionales (6 categorias: Produccion Musical, Audiovisual, Diseno y Branding, Marketing y Comunicacion, Gestion y Legal, Produccion de Eventos).

**Campos clave:**
- `Id` (int, PK)
- `Nombre` (string, NOT NULL) - ej: "Produccion Musical"
- `Icono` (string, nullable) - Icono de la categoria
- `Orden` (int) - Orden de presentacion
- Relacion: `ICollection<MaestraRolProfesional>`

### MaestraTipoEmpresa

Tipos de empresas profesionales (ej: "Estudio de grabacion", "Rental audiovisual"). Opcional para futuro.

**Campos clave:**
- `Id` (int, PK)
- `Nombre` (string, NOT NULL)
- `Descripcion` (string, nullable)
- `Activo` (bool, default true)

---

## API Endpoints

### GET /api/crowdsourcing/templates

Listar templates activos con resumen.

**Auth:** Artista (JWT)
**Response 200 OK:**
```json
{
  "items": [
    {
      "id": "guid",
      "nombre": "Grabar un Album / EP",
      "descripcion": "Todas las fases para grabar tu primer disco...",
      "icono": "music",
      "orden": 1,
      "precioMinTotal": 1920,
      "precioMaxTotal": 10400,
      "moneda": "EUR",
      "cantidadNecesidades": 11,
      "fases": ["Pre-produccion", "Grabacion", "Post-produccion", "Arte", "Distribucion", "Legal"]
    },
    // ... 5 templates mas
  ]
}
```

### GET /api/crowdsourcing/templates/{id}

Detalle de un template con necesidades y roles.

**Auth:** Artista (JWT)
**Response 200 OK:**
```json
{
  "id": "guid",
  "nombre": "Grabar un Album / EP",
  "descripcion": "Todas las fases para grabar tu primer disco...",
  "icono": "music",
  "necesidades": [
    {
      "id": "guid",
      "fase": "Pre-produccion",
      "titulo": "Composicion y arreglos musicales",
      "descripcion": "Crear arreglos instrumentales y vocales...",
      "rolProfesional": {
        "id": 1,
        "nombre": "Arreglista / Compositor",
        "descripcion": "Crea arreglos instrumentales y vocales a partir de una composicion basica...",
        "categoriaRol": "Produccion Musical",
        "modalidadCobro": "Por cancion"
      },
      "precioMinOrientativo": 200.00,
      "precioMaxOrientativo": 1500.00,
      "moneda": "EUR",
      "prioridad": "Esencial",
      "orden": 1
    },
    // ... mas necesidades
  ],
  "resumen": {
    "precioMinTotal": 1920,
    "precioMaxTotal": 10400,
    "necesidadesEsenciales": 9,
    "necesidadesRecomendadas": 0,
    "necesidadesOpcionales": 1
  }
}
```
**Errores:** 404 Not Found si template no existe o esta inactivo

### POST /api/crowdsourcing/templates/{id}/generar

Generar necesidades desde template.

**Auth:** Artista (JWT)
**Request:**
```json
{
  "proyectoArtisticoId": "guid",
  "necesidadesSeleccionadas": [
    {
      "plantillaNecesidadId": "guid",
      "presupuestoMin": 200.00,
      "presupuestoMax": 1500.00,
      "monedaId": 1
    },
    // ... mas necesidades seleccionadas
  ]
}
```
**Response 201 Created:**
```json
{
  "necesidadesCreadas": 8,
  "necesidadIds": ["guid1", "guid2", "guid3", "..."],
  "presupuestoTotalMin": 1920.00,
  "presupuestoTotalMax": 10400.00,
  "moneda": "EUR"
}
```
**Errores:**
- 400 Bad Request: Validacion fallida (sin necesidades seleccionadas, presupuesto max < min, IDs invalidos)
- 404 Not Found: Template o ProyectoArtistico no existe
- 403 Forbidden: El ProyectoArtistico no pertenece al artista autenticado

### GET /api/crowdsourcing/maestras/roles-profesionales

Catalogo de roles con categoria (para tooltips y filtros futuros).

**Auth:** Publico autenticado
**Response 200 OK:**
```json
{
  "items": [
    {
      "id": 1,
      "nombre": "Productor musical",
      "descripcion": "Dirige la vision sonora del proyecto completo...",
      "categoriaRol": {
        "id": 1,
        "nombre": "Produccion Musical",
        "icono": "music"
      },
      "modalidadCobro": "Por proyecto o por cancion"
    },
    // ... ~35 roles total
  ]
}
```

### GET /api/crowdsourcing/maestras/categorias-rol

Categorias de roles profesionales (6 categorias).

**Auth:** Publico autenticado
**Response 200 OK:**
```json
{
  "items": [
    { "id": 1, "nombre": "Produccion Musical", "icono": "music", "orden": 1 },
    { "id": 2, "nombre": "Audiovisual", "icono": "video", "orden": 2 },
    { "id": 3, "nombre": "Diseno y Branding", "icono": "palette", "orden": 3 },
    { "id": 4, "nombre": "Marketing y Comunicacion", "icono": "megaphone", "orden": 4 },
    { "id": 5, "nombre": "Gestion y Legal", "icono": "briefcase", "orden": 5 },
    { "id": 6, "nombre": "Produccion de Eventos / Live", "icono": "mic", "orden": 6 }
  ]
}
```

---

## Templates Predefinidos (Datos Seed)

### Template 1: Grabar un Album / EP

**Precio total orientativo:** 1,920 - 10,400 EUR
**Cantidad de necesidades:** 11
**Descripcion:** Todas las fases para grabar tu primer disco, desde la composicion hasta la distribucion digital.

**Necesidades:**
1. **Pre-produccion - Composicion y arreglos musicales** (Arreglista/Compositor, 200-1500 EUR, Esencial)
2. **Pre-produccion - Produccion musical** (Productor musical, 500-3000 EUR, Esencial)
3. **Grabacion - Alquiler de estudio** (Estudio de grabacion, 200-800 EUR, Esencial)
4. **Grabacion - Ingeniero de grabacion** (Ingeniero de sonido, 200-600 EUR, Esencial)
5. **Grabacion - Musicos de sesion** (Musico de sesion, 100-400 EUR, Opcional)
6. **Post-produccion - Mezcla de pistas** (Ingeniero de mezcla, 150-800 EUR, Esencial)
7. **Post-produccion - Mastering final** (Ingeniero de mastering, 50-150 EUR, Esencial)
8. **Arte - Portada del album** (Disenador grafico, 200-2000 EUR, Esencial)
9. **Arte - Sesion de fotos promocionales** (Fotografo musical, 200-600 EUR, Esencial)
10. **Distribucion - Distribucion digital** (Distribuidora digital, 20-50 EUR, Esencial)
11. **Legal - Registro de obras en SGAE/PRO** (Abogado musical/Gestor, 100-500 EUR, Esencial)

### Template 2: Produccion de Videoclip

**Precio total orientativo:** 2,000 - 14,800 EUR
**Cantidad de necesidades:** 11
**Descripcion:** Todo lo necesario para producir un videoclip profesional, desde el guion hasta los efectos visuales.

**Necesidades:**
1. **Pre-produccion - Guion y storyboard** (Guionista/Director creativo, 300-1500 EUR, Esencial)
2. **Pre-produccion - Casting** (Director de casting, 200-800 EUR, Opcional)
3. **Pre-produccion - Busqueda de localizaciones** (Location scout, 100-500 EUR, Opcional)
4. **Produccion - Direccion del videoclip** (Director audiovisual, 500-5000 EUR, Esencial)
5. **Produccion - Operador de camara/DOP** (Camarografo, 200-600 EUR, Esencial)
6. **Produccion - Tecnico de iluminacion** (Iluminador, 150-400 EUR, Recomendado)
7. **Produccion - Estilismo y maquillaje** (Estilista + Maquillador, 150-500 EUR, Recomendado)
8. **Produccion - Alquiler de equipo** (Rental audiovisual, 300-2000 EUR, Esencial)
9. **Post-produccion - Edicion y montaje** (Editor de video, 400-2000 EUR, Esencial)
10. **Post-produccion - Correccion de color** (Colorista, 200-1000 EUR, Recomendado)
11. **Post-produccion - Efectos visuales** (Artista VFX, 300-3000 EUR, Opcional)

### Template 3: Organizar una Gira / Tour

**Precio total orientativo:** 5,000 - 10,000 EUR (tour regional 7-10 fechas)
**Cantidad de necesidades:** 11
**Descripcion:** Planificacion logistica completa de una gira, desde booking hasta merchandising.

**Necesidades:**
1. **Planificacion - Booking de venues** (Agente de booking, 10-15% cachet, Esencial)
2. **Planificacion - Gestion logistica** (Tour manager, 300-800 EUR, Recomendado)
3. **Logistica - Transporte** (Alquiler furgoneta, 100-300 EUR, Esencial)
4. **Logistica - Alojamiento** (Por noche/persona, 50-150 EUR, Esencial)
5. **Logistica - Alquiler backline/equipo** (Rental de backline, 200-800 EUR, Variable)
6. **Tecnica - Tecnico de sonido FOH** (Tecnico de sonido, 150-400 EUR, Esencial)
7. **Tecnica - Tecnico de iluminacion** (Tecnico de luces, 100-300 EUR, Opcional)
8. **Merch - Diseno de merchandising** (Disenador grafico, 200-800 EUR, Recomendado)
9. **Merch - Produccion merchandising** (Imprenta/Serigrafia, 500-2000 EUR, Recomendado)
10. **Promo - Carteleria y flyers** (Disenador grafico, 100-400 EUR, Recomendado)
11. **Legal - Permisos, seguros y licencias** (Gestor/Abogado, 200-1000 EUR, Esencial)

### Template 4: Campana de Marketing y Promocion

**Precio total orientativo:** 2,000 - 17,000 EUR
**Cantidad de necesidades:** 9
**Descripcion:** Estrategia completa de marketing para lanzamiento de single o album.

**Necesidades:**
1. **Estrategia - Plan de marketing** (Consultor de marketing musical, 500-2000 EUR, Esencial)
2. **Branding - Identidad visual** (Disenador grafico/Branding, 500-3000 EUR, Esencial)
3. **Digital - Gestion de redes sociales** (Community manager, 500-1500 EUR mensual, Esencial)
4. **Digital - Creacion de contenido** (Content creator/Videografo, 300-1500 EUR mensual, Esencial)
5. **PR - Campana de prensa** (Publicista musical/PR, 1000-5000 EUR, Recomendado)
6. **PR - EPK** (Disenador+Fotografo+Copywriter, 300-1000 EUR, Esencial)
7. **Ads - Publicidad digital** (Media buyer/Ads specialist, 250-1000 EUR mensual, Recomendado)
8. **Playlist - Pitching a playlists** (Promotor de playlists, 200-1000 EUR, Recomendado)
9. **Radio - Promocion radios** (Promotor radiofonico, 500-3000 EUR, Opcional)

### Template 5: Lanzamiento de Single

**Precio total orientativo:** 1,500 - 5,000 EUR
**Cantidad de necesidades:** 8
**Descripcion:** Todo lo necesario para lanzar un single profesionalmente.

**Necesidades:**
1. **Produccion - Produccion musical + grabacion** (Productor + Estudio, 500-3000 EUR, Esencial)
2. **Post-produccion - Mezcla + mastering** (Ingeniero mezcla + mastering, 200-800 EUR, Esencial)
3. **Arte - Cover art** (Disenador grafico, 100-500 EUR, Esencial)
4. **Arte - Sesion de fotos** (Fotografo, 150-400 EUR, Esencial)
5. **Video - Lyric video/visualizer** (Editor video/Motion designer, 200-1000 EUR, Recomendado)
6. **Distribucion - Alta distribucion digital** (Distribuidora, 10-30 EUR, Esencial)
7. **Legal - Registro obra SGAE/PRO** (50-100 EUR, Esencial)
8. **Promo - Campana PR + playlists** (Publicista, 500-2000 EUR, Recomendado)

### Template 6: Crear Presencia Online (Branding Inicial)

**Precio total orientativo:** 1,000 - 5,000 EUR
**Cantidad de necesidades:** 5
**Descripcion:** Construccion de marca personal y presencia digital profesional.

**Necesidades:**
1. **Branding - Logo e identidad visual** (Disenador grafico, 300-1500 EUR, Esencial)
2. **Web - Pagina web/landing page** (Desarrollador web, 500-3000 EUR, Recomendado)
3. **Foto - Sesion fotografica profesional** (Fotografo, 200-600 EUR, Esencial)
4. **Bio - Biografia y storytelling** (Copywriter musical, 100-400 EUR, Esencial)
5. **Perfiles - Setup y optimizacion perfiles** (Consultor digital, 100-300 EUR, Esencial)

---

## Requisitos No Funcionales

### Performance
- La galeria de templates debe cargar en < 500ms (cache en frontend de datos maestros)
- El wizard debe funcionar fluidamente con navegacion instantanea entre pasos (estado en React)
- La creacion de necesidades debe ejecutarse en una sola transaccion atomica (< 2 segundos para 11 necesidades)

### Seguridad
- Validar que el `ProyectoArtisticoId` pertenece al artista autenticado (prevenir creacion en proyectos de otros)
- Sanitizar inputs de presupuestos (evitar inyeccion SQL, XSS)
- Rate limiting en endpoint de generacion (max 10 requests por hora por usuario)

### UX
- Wizard debe tener indicador visual de paso actual (1/3, 2/3, 3/3)
- Navegacion entre pasos sin perder datos ingresados
- Resumen de presupuesto en tiempo real al marcar/desmarcar necesidades
- Tooltips educativos en todos los roles profesionales

### Mantenibilidad
- Datos seed versionados en migraciones (no hardcoded en codigo)
- Templates y roles editables desde Admin en futuro (no requiere deploy)
- Logs de auditoria al crear necesidades (quien, cuando, desde que template)

---

## Dependencias

### Tecnicas (dentro del proyecto)
- **Modulo UserAccess**: Entidad `Artista` (FK desde NecesidadCrowdsourcing)
- **Modulo Crowdfunding**: Entidad `ProyectoArtistico` (FK desde NecesidadCrowdsourcing)
- **Shared**: Types y schemas Zod para DTOs de templates y necesidades
- **BuildingBlocks**: `ServiceResponse`, `ServiceResponseMessageType`, RequestCacheService

### De otras User Stories
- **Ninguna** - Esta es la US de entrada al modulo Crowdsourcing (no depende de otras US)

### Dependencias futuras (que esta US habilita)
- US-CS-02: Postulacion de Profesionales (requiere `NecesidadCrowdsourcing` creada)
- US-CS-03: Seleccion de Profesional (requiere postulaciones)
- US-CS-04: Chat con Profesionales (requiere profesional seleccionado)

---

## Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|--------------|---------|------------|
| Precios orientativos quedan obsoletos rapido | Media | Medio | Hacer precios editables desde Admin. Agregar campo `FechaActualizacionPrecios` para alertar si esta desactualizado |
| Artistas ignoran la feature y usan formulario manual | Media | Alto | Hacer wizard el flujo por defecto, ocultar formulario manual o ponerlo como "Modo avanzado" |
| Exceso de necesidades creadas (spam) | Baja | Medio | Rate limiting (max 10 generaciones por hora). Validar al menos 1 necesidad seleccionada |
| Datos seed muy extensos ralentizan migraciones | Baja | Bajo | Separar seed en multiples archivos. Usar bulk insert en lugar de loops |
| Templates muy especificos no cubren todos los casos | Alta | Medio | Agregar template generico "Proyecto personalizado" que permite configurar desde cero. Permitir crear templates custom en Admin |
| Artistas modifican presupuestos a valores irreales | Media | Bajo | Solo validacion basica (max >= min, >= 0). No limitar creatividad. Log de outliers para analisis |

---

## Notas de Implementacion

### Backend
- Crear nueva carpeta `Modules/Crowdsourcing` con estructura Domain/Application/Infra/WebApi
- Usar patron CQRS (MediatR) para Commands y Queries
- Implementar datos seed en `OnModelCreating` o archivo separado `CrowdsourcingSeedData.cs`
- Usar EF Core Fluent API para configurar relaciones FK
- Cache de templates activos (datos que cambian poco)
- Validacion con FluentValidation: presupuesto max >= min, al menos 1 necesidad seleccionada

### Frontend (Landing/Web)
- Implementar wizard con React Hook Form + Zod
- Estado del wizard en `useReducer` o `useState` (no persistir en servidor hasta confirmar)
- Componente `RolProfesionalTooltip` con lazy loading de descripciones
- Resumen presupuestario recalculado en tiempo real (efecto de suma)
- Routing: `/crowdsourcing/nuevo-proyecto` -> Paso 1, `/crowdsourcing/nuevo-proyecto/:templateId` -> Paso 2

### Datos Seed
- Total: 6 templates, ~55 necesidades, ~35 roles profesionales, 6 categorias de roles
- Formato: SQL script o metodo C# en DbContext
- Versionado: Incluir en migracion inicial o migracion separada `AddCrowdsourcingTemplatesSeed`

### Testing
- Unit tests: Validator de `GenerarNecesidadesCommand`, mapping de DTOs
- Integration tests: Endpoint POST `/templates/{id}/generar` con BD en memoria
- E2E: Wizard completo (seleccionar template, personalizar, confirmar, verificar creacion)

---

## Referencia

- User Story completa: [docs/product/US-CS-01-templates-guia.md](../../product/US-CS-01-templates-guia.md)
- ADR-006: Request-Scoped Cache (para evitar queries duplicados en validator + handler)
- Template de referencia: Krowd (referencias/templates/krowd) para UI de galeria de proyectos
