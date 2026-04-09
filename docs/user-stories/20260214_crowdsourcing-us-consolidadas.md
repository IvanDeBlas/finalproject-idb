# User Stories Consolidadas - Modulo Crowdsourcing

- **Fecha**: 2026-02-14
- **Modulo**: Crowdsourcing
- **Autor**: Ivan (asistido por Claude)
- **Estado**: Borrador

---

## Mapa de Consolidacion

Las 23 user stories originales se consolidan en 6 features funcionales, cada una ejecutable con el pipeline `/us-to-spec` -> `/plan` -> `/implement`:

| US Consolidada | Absorbe | Feature Name | Concepto |
|---|---|---|---|
| **US-CS-01** | US-CS-000 | `cs-templates-guia` | Templates y guia para artistas noveles |
| **US-CS-02** | US-CS-001, 002, 003, 004 | `cs-gestionar-necesidades` | Publicar y gestionar necesidades (artista) |
| **US-CS-03** | US-CS-005, 006, 007, 008 | `cs-explorar-propuestas` | Explorar necesidades y enviar propuestas (profesional) |
| **US-CS-04** | US-CS-009 a 016 | `cs-acuerdos-entregables` | Acuerdos, milestones y entregables |
| **US-CS-05** | US-CS-017, 018, 019 | `cs-mensajeria` | Mensajeria entre partes |
| **US-CS-06** | US-CS-020, 021, 022 | `cs-valoraciones` | Valoraciones bidireccionales |

### Orden de Implementacion Recomendado

```
US-CS-01 (Templates)  ──┐
                        ├──> US-CS-02 (Necesidades) ──> US-CS-03 (Propuestas) ──> US-CS-04 (Acuerdos)
                        │                                                              │
                        │                                                    ┌─────────┴─────────┐
                        │                                                    v                   v
                        │                                              US-CS-05            US-CS-06
                        │                                             (Mensajeria)       (Valoraciones)
```

---

## Contexto General

WePlay Rises es una plataforma de crowdsourcing y crowdfunding para grupos musicales noveles. El modulo de **Crowdsourcing** permite a artistas publicar necesidades profesionales (grabacion, diseno, marketing, etc.) y recibir propuestas de proveedores de servicios. Un diferenciador clave es que los artistas noveles **no conocen la industria musical**, por lo que la plataforma les guia con templates predefinidos, roles profesionales catalogados y precios orientativos.

### Entidades de Dominio Existentes

```
NecesidadCrowdsourcing          -> Oferta de trabajo publicada por un artista
PropuestaCrowdsourcing          -> Propuesta enviada por un profesional
AcuerdoCrowdsourcing            -> Contrato entre artista y profesional
AcuerdoCrowdsourcingMilestone   -> Hitos de pago dentro de un acuerdo
AcuerdoCrowdsourcingEntregable  -> Archivos/recursos entregados
ConversacionCrowdsourcing       -> Hilo de conversacion entre partes
MensajeCrowdsourcing            -> Mensaje individual dentro de conversacion
ValoracionCrowdsourcing         -> Review bidireccional tras completar acuerdo
```

### Tablas Maestras Existentes

```
MaestraTipoNecesidad       -> Tipos: Diseno, Produccion, Marketing, etc.
MaestraEstadoNecesidad     -> Estados: Abierta, En Progreso, Cerrada, etc.
MaestraEstadoPropuesta     -> Estados: Pendiente, Aceptada, Rechazada, etc.
MaestraEstadoAcuerdo       -> Estados: Activo, Completado, Cancelado, etc.
MaestraEstadoEntregable    -> Estados: Pendiente, Entregado, Aprobado, Rechazado
MaestraModalidadTrabajo    -> Presencial, Remoto, Hibrido
MaestraMoneda              -> EUR, USD, etc.
MaestraTipoValoracion      -> Tipos de valoracion
```

### Flujo Principal del Modulo

```
Artista selecciona Template ──> Genera Necesidades automaticamente
                                       │
Profesional ve Necesidades abiertas ──> Envia Propuesta
                                              │
Artista acepta Propuesta ──> Se crea Acuerdo con Milestones
                                       │
Profesional sube Entregables ──> Artista aprueba/rechaza
                                       │
Todos los entregables aprobados ──> Acuerdo completado
                                       │
                                Ambas partes dejan Valoracion
```

---

## US-CS-01: Templates y Guia para Artistas Noveles

> **Feature Name:** `cs-templates-guia`
> **Prioridad:** Alta
> **Estimacion:** XL (Extra Large)
> **Dependencias:** Ninguna (punto de entrada al modulo)

---

### Historia de Usuario

**Como** artista novel que no conoce la industria musical,
**quiero** seleccionar un tipo de proyecto (ej: "Grabar un album") y ver automaticamente un desglose de todas las necesidades profesionales que tendre, con roles recomendados y precios orientativos,
**para** poder planificar mi presupuesto y publicar las necesidades que me interesen directamente en la plataforma sin necesidad de saber como funciona la industria.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Artista | Usuario autenticado con perfil de artista, tipicamente novel |
| Admin | Gestiona y actualiza templates y precios desde backoffice |

### Precondiciones

- Usuario tiene cuenta activa y perfil de Artista
- Existen templates de proyecto pre-cargados en la BD (datos de seed)

### Postcondiciones

- Se crean N necesidades de tipo `NecesidadCrowdsourcing` vinculadas al proyecto del artista
- Las necesidades quedan en estado `Abierta` y visibles para profesionales

---

### Justificacion

Los artistas noveles no saben:
- Que profesionales necesitan para cada tipo de proyecto
- Cuanto cuestan esos servicios en el mercado
- En que orden deben contratarlos
- Que fases tiene un proyecto musical

La plataforma debe actuar como un **mentor digital** que les guia paso a paso. Esto diferencia a WePlay Rises de un marketplace generico tipo Fiverr o Upwork.

---

### Flujo Principal

```
Artista accede a /crowdsourcing/nuevo-proyecto
       │
       v
Paso 1: Seleccionar template de proyecto
       │  (cards con icono, nombre, descripcion, rango precio total)
       v
Paso 2: Personalizar necesidades
       │  (desglose por fases, marcar/desmarcar, editar presupuestos)
       v
Paso 3: Confirmar y publicar
       │  (resumen presupuestario, crear necesidades)
       v
Se crean N NecesidadCrowdsourcing en estado Abierta
```

---

### Templates de Proyecto Predefinidos

#### Template 1: Grabar un Album / EP

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Pre-produccion | Composicion y arreglos musicales | Arreglista / Compositor | 200 | 1.500 | Esencial |
| Pre-produccion | Produccion musical (direccion artistica del sonido) | Productor musical | 500 | 3.000 | Esencial |
| Grabacion | Alquiler de estudio de grabacion | Estudio de grabacion | 200 | 800 | Esencial |
| Grabacion | Ingeniero de grabacion (captura de audio) | Ingeniero de sonido | 200 | 600 | Esencial |
| Grabacion | Musicos de sesion (instrumentos adicionales) | Musico de sesion | 100 | 400 | Opcional |
| Post-produccion | Mezcla de pistas | Ingeniero de mezcla | 150 | 800 | Esencial |
| Post-produccion | Mastering final para distribucion | Ingeniero de mastering | 50 | 150 | Esencial |
| Arte | Portada del album / EP | Disenador grafico | 200 | 2.000 | Esencial |
| Arte | Sesion de fotos promocionales | Fotografo musical | 200 | 600 | Esencial |
| Distribucion | Distribucion digital (Spotify, Apple Music, etc.) | Distribuidora digital | 20 | 50 | Esencial |
| Legal | Registro de obras en SGAE / PRO | Abogado musical / Gestor | 100 | 500 | Esencial |

Precio total orientativo:
- EP (5 canciones): 3.000 - 15.000 EUR
- Album (10-12 canciones): 8.000 - 40.000 EUR

#### Template 2: Produccion de Videoclip

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Pre-produccion | Guion y storyboard | Guionista / Director creativo | 300 | 1.500 | Esencial |
| Pre-produccion | Casting de actores/modelos | Director de casting | 200 | 800 | Opcional |
| Pre-produccion | Busqueda de localizaciones | Location scout | 100 | 500 | Opcional |
| Produccion | Direccion del videoclip | Director audiovisual | 500 | 5.000 | Esencial |
| Produccion | Operador de camara / DOP | Camarografo | 200 | 600 | Esencial |
| Produccion | Tecnico de iluminacion | Iluminador | 150 | 400 | Recomendado |
| Produccion | Estilismo y maquillaje | Estilista + Maquillador | 150 | 500 | Recomendado |
| Produccion | Alquiler de equipo audiovisual | Rental audiovisual | 300 | 2.000 | Esencial |
| Post-produccion | Edicion y montaje | Editor de video | 400 | 2.000 | Esencial |
| Post-produccion | Correccion de color (color grading) | Colorista | 200 | 1.000 | Recomendado |
| Post-produccion | Efectos visuales y motion graphics | Artista VFX | 300 | 3.000 | Opcional |

Precio total orientativo:
- Videoclip basico: 2.000 - 5.000 EUR
- Videoclip profesional: 5.000 - 20.000 EUR

#### Template 3: Organizar una Gira / Tour

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Planificacion | Booking de venues / salas | Agente de booking | 10% cachet | 15% cachet | Esencial |
| Planificacion | Gestion logistica del tour | Tour manager | 300 | 800 | Recomendado |
| Logistica | Transporte (alquiler furgoneta) | - | 100 | 300 | Esencial |
| Logistica | Alojamiento por noche y persona | - | 50 | 150 | Esencial |
| Logistica | Alquiler de backline / equipo de sonido | Rental de backline | 200 | 800 | Variable |
| Tecnica | Tecnico de sonido en directo (FOH) | Tecnico de sonido | 150 | 400 | Esencial |
| Tecnica | Tecnico de iluminacion en directo | Tecnico de luces | 100 | 300 | Opcional |
| Merch | Diseno de merchandising (camisetas, posters) | Disenador grafico | 200 | 800 | Recomendado |
| Merch | Produccion de merchandising (impresion, serigrafia) | Imprenta / Serigrafia | 500 | 2.000 | Recomendado |
| Promo | Carteleria y flyers para cada fecha | Disenador grafico | 100 | 400 | Recomendado |
| Legal | Permisos, seguros y licencias | Gestor / Abogado | 200 | 1.000 | Esencial |

Precio total orientativo:
- Tour regional (7-10 fechas): 5.000 - 10.000 EUR

#### Template 4: Campana de Marketing y Promocion

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Estrategia | Plan de marketing para lanzamiento | Consultor de marketing musical | 500 | 2.000 | Esencial |
| Branding | Identidad visual completa (logo, colores, tipografia) | Disenador grafico / Branding | 500 | 3.000 | Esencial |
| Digital | Gestion de redes sociales (mensual) | Community manager | 500 | 1.500 | Esencial |
| Digital | Creacion de contenido para redes (mensual) | Content creator / Videografo | 300 | 1.500 | Esencial |
| PR | Campana de prensa y medios | Publicista musical / PR | 1.000 | 5.000 | Recomendado |
| PR | EPK (Electronic Press Kit): bio, fotos, links | Disenador + Fotografo + Copywriter | 300 | 1.000 | Esencial |
| Ads | Publicidad digital en Meta, TikTok, Google (mensual + presupuesto) | Media buyer / Ads specialist | 250 | 1.000 | Recomendado |
| Playlist | Pitching a playlists de Spotify, Apple Music, Deezer | Promotor de playlists | 200 | 1.000 | Recomendado |
| Radio | Promocion en emisoras de radio | Promotor radiofonico | 500 | 3.000 | Opcional |

Precio total orientativo:
- Lanzamiento de single: 2.000 - 8.000 EUR
- Lanzamiento de album: 5.000 - 20.000 EUR

#### Template 5: Lanzamiento de Single

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Produccion | Produccion musical + grabacion en estudio | Productor + Estudio | 500 | 3.000 | Esencial |
| Post-produccion | Mezcla + mastering | Ingeniero mezcla + mastering | 200 | 800 | Esencial |
| Arte | Cover art para plataformas digitales | Disenador grafico | 100 | 500 | Esencial |
| Arte | Sesion de fotos promocionales | Fotografo | 150 | 400 | Esencial |
| Video | Lyric video o visualizer animado | Editor video / Motion designer | 200 | 1.000 | Recomendado |
| Distribucion | Alta en distribucion digital | Distribuidora | 10 | 30 | Esencial |
| Legal | Registro de la obra en SGAE / PRO | - | 50 | 100 | Esencial |
| Promo | Campana de PR + pitching a playlists | Publicista | 500 | 2.000 | Recomendado |

Precio total orientativo: 1.500 - 5.000 EUR

#### Template 6: Crear Presencia Online (Branding Inicial)

| Fase | Necesidad | Rol profesional | Precio min (EUR) | Precio max (EUR) | Prioridad |
|------|-----------|----------------|-------------------|-------------------|-----------|
| Branding | Logo e identidad visual | Disenador grafico | 300 | 1.500 | Esencial |
| Web | Pagina web / landing page del artista | Desarrollador web | 500 | 3.000 | Recomendado |
| Foto | Sesion fotografica profesional | Fotografo | 200 | 600 | Esencial |
| Bio | Biografia profesional y storytelling | Copywriter musical | 100 | 400 | Esencial |
| Perfiles | Setup y optimizacion de perfiles (Spotify for Artists, Apple, YouTube, etc.) | Consultor digital | 100 | 300 | Esencial |

Precio total orientativo: 1.000 - 5.000 EUR

---

### Catalogo Completo de Roles Profesionales

Estos roles alimentan las plantillas y deben existir como datos maestros en la plataforma.

**Categoria: Produccion Musical**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Productor musical | Dirige la vision sonora del proyecto completo. Decide el enfoque artistico, selecciona sonidos, guia las sesiones de grabacion y supervisa mezcla/master. | Por proyecto o por cancion |
| Arreglista | Crea arreglos instrumentales y vocales a partir de una composicion basica. Decide que instrumentos suenan, como interactuan y la estructura final. | Por cancion |
| Compositor / Songwriter | Crea letras, melodias y/o armonias originales. Puede co-escribir con el artista. | Por cancion + % royalties |
| Ingeniero de grabacion | Opera el equipo tecnico del estudio durante las sesiones de grabacion. Coloca microfonos, gestiona niveles, asegura calidad de captura. | Por hora o por dia |
| Ingeniero de mezcla | Toma todas las pistas grabadas (voces, instrumentos, efectos) y las equilibra en un mix estereo cohesivo usando EQ, compresion, reverb y automatizacion. | Por cancion |
| Ingeniero de mastering | Aplica el procesamiento final al mix para optimizar la escucha en todas las plataformas (streaming, vinilo, radio). Ajusta volumen, dinamica y frecuencias. | Por cancion |
| Musico de sesion | Interpreta instrumentos especificos (bateria, bajo, cuerdas, vientos, piano, guitarra) para las grabaciones del artista sin ser miembro permanente de la banda. | Por sesion o por cancion |
| Programador musical / Beatmaker | Crea bases ritmicas, beats electronicos y texturas sonoras usando software (DAW) y sintetizadores. Comun en pop, hip-hop, electronica. | Por beat o por cancion |
| Director musical | Coordina ensayos, adapta los arreglos de estudio para directo, dirige a la banda en escenario. Esencial para giras y festivales. | Por proyecto o por evento |

**Categoria: Audiovisual**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Director de videoclip | Dirige la produccion completa del video musical: concepto creativo, puesta en escena, direccion de actores/artista y supervision de post-produccion. | Por proyecto |
| Camarografo / Director de fotografia (DOP) | Opera la camara y decide la composicion visual, movimientos de camara, optica y estilo cinematografico del videoclip. | Por dia de rodaje |
| Editor de video | Realiza el montaje de todo el material grabado: seleccion de tomas, ritmo de edicion, sincronizacion con la musica y estructura narrativa. | Por proyecto |
| Colorista | Aplica la correccion de color y el look cinematografico final al video (color grading). Define la paleta visual y la atmosfera cromatica. | Por proyecto |
| Artista VFX / Motion graphics | Crea efectos visuales digitales, graficos animados, titulos y elementos visuales que se integran en el videoclip. | Por proyecto |
| Fotografo musical | Realiza sesiones de fotos para material promocional: portadas, prensa, redes sociales, EPK. Tambien puede cubrir conciertos en directo. | Por sesion |

**Categoria: Diseno y Branding**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Disenador grafico | Crea piezas graficas: portadas de album/single, carteleria, flyers, banners para redes, material promocional impreso y digital. | Por proyecto o por pieza |
| Ilustrador | Crea ilustraciones originales a mano o digitales para portadas, merchandising, posters o material visual unico del artista. | Por pieza |
| Disenador web | Disena y desarrolla la pagina web del artista: landing page, EPK online, integracion con redes y plataformas de streaming. | Por proyecto |
| Disenador de merchandising | Disena las piezas de merch del artista: camisetas, sudaderas, gorras, tote bags, posters, pegatinas, etc. Prepara archivos para imprenta. | Por pieza |

**Categoria: Marketing y Comunicacion**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Publicista musical / PR | Gestiona las relaciones con prensa musical, blogs, revistas, podcasts y medios. Redacta notas de prensa y coordina entrevistas y reviews. | Por campana o mensual |
| Community manager | Gestiona las redes sociales del artista en el dia a dia: publica contenido, responde comentarios, crea engagement con la comunidad. | Mensual (retainer) |
| Content creator | Crea contenido original para redes sociales: videos cortos (Reels, TikTok), stories, detras de camaras, fotos editadas, motion graphics. | Mensual o por pieza |
| Consultor de marketing musical | Disena la estrategia global de lanzamiento: timing, canales, target, presupuesto. Asesora al artista sobre como posicionar su musica. | Por proyecto |
| Promotor de playlists | Realiza pitching profesional de la musica del artista a curadores de playlists editoriales e independientes en Spotify, Apple Music, Deezer, etc. | Por cancion o por campana |
| Promotor radiofonico | Gestiona relaciones con emisoras de radio para conseguir airplay. Envia singles a programadores musicales y hace seguimiento. | Por campana |
| Media buyer / Ads specialist | Planifica, ejecuta y optimiza campanas de publicidad digital en Meta (Instagram/Facebook), TikTok Ads, Google Ads y YouTube Ads. | Mensual + % de inversion |
| Copywriter musical | Redacta textos profesionales para el artista: biografia, notas de prensa, descripciones de canciones, textos para web y EPK. | Por proyecto o por pieza |

**Categoria: Gestion y Legal**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Manager artistico | Gestiona integralmente la carrera del artista: estrategia, negociaciones, coordinacion de equipo, oportunidades de negocio. Es el representante del artista. | % de ingresos (15-20%) |
| Agente de booking | Se encarga de conseguir y negociar conciertos, festivales y eventos para el artista. Negocia cachets y condiciones tecnicas (rider). | % del cachet (10-15%) |
| Tour manager | Gestiona toda la logistica de una gira: transporte, alojamiento, horarios, riders tecnicos, liquidaciones con promotores, coordinacion con venues. | Por dia o por tour completo |
| Abogado musical / Entertainment lawyer | Revisa y redacta contratos (discograficos, editoriales, de management, sync). Asesora sobre derechos de autor y propiedad intelectual. | Por hora o por proyecto |
| Contable / Gestor fiscal | Gestiona la facturacion, impuestos, declaraciones y contabilidad del artista como actividad profesional. Controla royalties y pagos. | Mensual (retainer) |
| Distribuidor digital | Servicio que sube y gestiona la musica del artista en todas las plataformas de streaming (Spotify, Apple Music, Amazon, YouTube Music, Deezer, Tidal, etc.). | Anual o % de royalties |

**Categoria: Produccion de Eventos / Live**

| Rol | Descripcion | Modalidad tipica de cobro |
|-----|-------------|---------------------------|
| Tecnico de sonido (FOH) | Controla la mesa de mezclas en directo desde la posicion de publico (Front of House). Es responsable de que el sonido del concierto sea equilibrado. | Por evento |
| Tecnico de monitores | Controla el sonido de los monitores en escenario para que cada musico escuche correctamente durante el directo. | Por evento |
| Tecnico de luces | Disena y opera la iluminacion del espectaculo en directo: programacion de focos, seguimiento de la musica, atmosferas visuales. | Por evento |
| Roadie / Stage hand | Asistente tecnico de escenario: monta y desmonta equipo, cambia instrumentos, gestiona backline durante el concierto. | Por evento |

### Tipos de Empresa / Proveedor

| Tipo de empresa | Descripcion | Servicios que ofrece |
|----------------|-------------|----------------------|
| Estudio de grabacion | Instalaciones profesionales con equipo de grabacion de audio | Grabacion, produccion, ensayo con grabacion |
| Estudio de mastering | Instalacion especializada en mastering de audio | Mastering estereo, mastering para vinilo, mastering Dolby Atmos |
| Productora audiovisual | Empresa de produccion de video y contenido visual | Videoclips, contenido para redes, documentales musicales |
| Agencia de PR / Prensa musical | Empresa especializada en relaciones publicas para musica | Campanas de prensa, media outreach, entrevistas |
| Agencia de marketing musical | Empresa de marketing digital enfocada en musica | Estrategia digital, ads, playlisting, redes sociales |
| Agencia de booking | Empresa que contrata y gestiona conciertos | Booking de venues, negociacion de cachets, giras |
| Agencia de management | Empresa de representacion de artistas | Gestion de carrera, estrategia, negociaciones |
| Distribuidora digital | Plataforma que sube musica a servicios de streaming | Distribucion, reportes de royalties, pitching editorial |
| Sello discografico indie | Sello independiente que ofrece servicios integrados | Produccion, distribucion, marketing, A&R |
| Rental de equipo | Empresa de alquiler de equipos de sonido, luces y backline | PA systems, backline, iluminacion, escenarios |
| Imprenta / Serigrafia | Empresa de produccion de merchandising y material fisico | Camisetas, vinilos, CDs, posters, pegatinas |
| Estudio de diseno | Agencia o estudio de diseno grafico | Branding, portadas, web, merch design |
| Estudio de fotografia | Estudio o fotografo con espacio propio | Sesiones promo, portadas, lookbooks |
| Escuela / Academia musical | Centro de formacion musical | Clases, coaching vocal, talleres de produccion |
| Estudio de ensayo | Sala equipada para ensayos de bandas | Alquiler por horas con backline basico |

---

### Modelo de Datos (Nuevas Entidades)

```
PlantillaProyecto
    Id                  UNIQUEIDENTIFIER    PK
    Nombre              NVARCHAR(200)       NOT NULL    -- "Grabar un Album"
    Descripcion         NVARCHAR(MAX)       NULL        -- Texto explicativo para el artista
    Icono               NVARCHAR(100)       NULL        -- Nombre del icono (ej: "music", "video")
    Orden               INT                 NOT NULL    -- Orden de presentacion en UI
    Activo              BIT                 NOT NULL    -- Para ocultar templates sin borrar
    FechaCreacion       DATETIME2(3)        NOT NULL

PlantillaProyectoNecesidad
    Id                  UNIQUEIDENTIFIER    PK
    PlantillaProyectoId UNIQUEIDENTIFIER    FK -> PlantillaProyecto
    Fase                NVARCHAR(100)       NOT NULL    -- "Pre-produccion", "Grabacion", etc.
    Titulo              NVARCHAR(200)       NOT NULL    -- "Mezcla de pistas"
    Descripcion         NVARCHAR(MAX)       NULL        -- Explicacion para el artista novel
    RolProfesionalId    INT                 FK -> MaestraRolProfesional
    PrecioMinOrientativo DECIMAL(18,2)      NULL        -- Precio minimo de mercado
    PrecioMaxOrientativo DECIMAL(18,2)      NULL        -- Precio maximo de mercado
    MonedaId            INT                 FK -> MaestraMoneda
    Prioridad           NVARCHAR(20)        NOT NULL    -- 'Esencial', 'Recomendado', 'Opcional'
    Orden               INT                 NOT NULL    -- Orden dentro de la plantilla
    FechaCreacion       DATETIME2(3)        NOT NULL

MaestraRolProfesional
    Id                  INT                 PK
    Nombre              NVARCHAR(200)       NOT NULL    -- "Productor musical"
    Descripcion         NVARCHAR(MAX)       NULL        -- Que hace este rol
    CategoriaRolId      INT                 FK -> MaestraCategoriaRol
    ModalidadCobro      NVARCHAR(100)       NULL        -- "Por cancion", "Por dia", "Mensual"
    Activo              BIT                 NOT NULL

MaestraCategoriaRol
    Id                  INT                 PK
    Nombre              NVARCHAR(100)       NOT NULL    -- "Produccion Musical", "Audiovisual"
    Icono               NVARCHAR(100)       NULL
    Orden               INT                 NOT NULL

MaestraTipoEmpresa
    Id                  INT                 PK
    Nombre              NVARCHAR(200)       NOT NULL    -- "Estudio de grabacion"
    Descripcion         NVARCHAR(MAX)       NULL
    Activo              BIT                 NOT NULL
```

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-01-1 | El artista ve una galeria de templates con cards (icono, nombre, descripcion, rango de precio total) y puede seleccionar uno |
| AC-01-2 | Al seleccionar un template, se muestra un desglose por fases con cada necesidad, el rol profesional recomendado, el rango de precio orientativo y la prioridad (Esencial / Recomendado / Opcional) |
| AC-01-3 | El artista puede marcar/desmarcar las necesidades que le interesan. Las marcadas como "Esencial" vienen seleccionadas por defecto |
| AC-01-4 | El artista puede ajustar el presupuesto de cada necesidad seleccionada (el precio orientativo es solo una guia, no un limite) |
| AC-01-5 | Al confirmar, se crean automaticamente las `NecesidadCrowdsourcing` correspondientes, pre-rellenadas con titulo, descripcion, tipo de necesidad, rango de presupuesto y modalidad de trabajo derivados del template |
| AC-01-6 | Todas las necesidades generadas se vinculan al mismo `ProyectoArtistico` del artista |
| AC-01-7 | Antes de confirmar, se muestra un resumen con el coste total estimado (suma de rangos min y max de las necesidades seleccionadas) |
| AC-01-8 | Las 6 plantillas descritas arriba, con sus necesidades, roles y precios, vienen pre-cargadas como datos de seed en la base de datos |
| AC-01-9 | Cada rol profesional tiene un icono de ayuda (tooltip) que muestra una descripcion de "que hace este profesional y por que lo necesitas" |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| GET | /api/crowdsourcing/templates | Listar templates activos con sus necesidades | Artista |
| GET | /api/crowdsourcing/templates/{id} | Detalle de un template con necesidades y roles | Artista |
| POST | /api/crowdsourcing/templates/{id}/generar | Generar necesidades desde template | Artista |
| GET | /api/crowdsourcing/maestras/roles-profesionales | Catalogo de roles con categoria | Publico |
| GET | /api/crowdsourcing/maestras/categorias-rol | Categorias de roles | Publico |

### Consideraciones de UI/UX

- **Wizard de 3 pasos**: (1) Seleccionar template -> (2) Personalizar necesidades -> (3) Confirmar y publicar
- **Indicador visual de prioridad**: Esencial (rojo/obligatorio), Recomendado (amarillo), Opcional (gris)
- **Barra de progreso de presupuesto**: Suma acumulada en tiempo real
- Los templates y precios se gestionan desde Admin y son editables sin despliegue de codigo
- En un futuro, se podria permitir a los propios proveedores validar/sugerir precios de mercado

---

## US-CS-02: Publicar y Gestionar Necesidades

> **Feature Name:** `cs-gestionar-necesidades`
> **Prioridad:** Alta
> **Estimacion:** L (Large)
> **Dependencias:** US-CS-01 (las necesidades pueden venir de templates o crearse manualmente)

---

### Historia de Usuario

**Como** artista registrado en la plataforma,
**quiero** publicar, listar, editar y cerrar necesidades de servicios profesionales,
**para** gestionar mis solicitudes de crowdsourcing y atraer propuestas de profesionales cualificados.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Artista | Usuario autenticado con perfil de artista |

### Precondiciones

- Usuario tiene cuenta activa y perfil de Artista
- Existe al menos un ProyectoArtistico asociado al artista

### Postcondiciones

- Necesidades creadas en estado `Abierta` y visibles para profesionales
- Necesidades editadas/cerradas reflejan su nuevo estado

---

### Flujo Principal: Publicar Necesidad

Un artista puede crear necesidades de dos formas:
1. **Desde un template** (US-CS-01): las necesidades se pre-rellenan automaticamente.
2. **Manualmente**: el artista crea la necesidad desde cero rellenando todos los campos.

**Campos del formulario:**

| Campo | Tipo | Obligatorio | Validacion | Origen |
|-------|------|-------------|------------|--------|
| Titulo | Texto (max 200) | Si | Min 5 caracteres | Manual o template |
| Descripcion | Texto largo | No | Max 4000 caracteres | Manual o template |
| Tipo de necesidad | Select (MaestraTipoNecesidad) | Si | Debe existir en maestras | Manual o template |
| Estado | Automatico | - | Se crea como "Abierta" | Sistema |
| Modalidad de trabajo | Select (MaestraModalidadTrabajo) | Si | Presencial / Remoto / Hibrido | Manual o template |
| Presupuesto minimo | Decimal | No | >= 0 | Manual o template |
| Presupuesto maximo | Decimal | No | >= presupuesto minimo | Manual o template |
| Moneda | Select (MaestraMoneda) | Si (si hay presupuesto) | - | Manual o template |
| Ubicacion (ciudad) | Texto (max 100) | No | Solo si modalidad = Presencial/Hibrido | Manual |
| Ubicacion (pais) | Texto (max 100) | No | Solo si modalidad = Presencial/Hibrido | Manual |
| Fecha limite de propuestas | Date | No | >= hoy + 1 dia | Manual |
| Fecha inicio prevista | Date | No | >= hoy | Manual |
| Proyecto artistico | Select | Si | FK a ProyectoArtistico del artista | Automatico o manual |

### Flujo Secundario: Listar Mis Necesidades

**Datos a mostrar por necesidad:**

| Dato | Fuente |
|------|--------|
| Titulo | NecesidadCrowdsourcing.Titulo |
| Estado (badge de color) | MaestraEstadoNecesidad.Nombre |
| Tipo de necesidad | MaestraTipoNecesidad.Nombre |
| Rango de presupuesto | PresupuestoMin - PresupuestoMax + Moneda |
| Numero de propuestas recibidas | COUNT(PropuestaCrowdsourcing) |
| Fecha de publicacion | FechaCreacion |
| Fecha limite de propuestas | FechaLimitePropuestas (con indicador si esta proxima) |
| Modalidad de trabajo | MaestraModalidadTrabajo.Nombre |

### Flujo Secundario: Editar Necesidad

Solo se puede editar una necesidad en estado `Abierta`. Si la necesidad tiene propuestas recibidas, se muestra aviso: "Esta necesidad ya tiene N propuestas. Los cambios seran visibles para los profesionales que ya enviaron propuestas." Todos los campos son editables excepto el tipo de necesidad (para no invalidar propuestas existentes). Al guardar, se actualiza `FechaActualizacion`. Si la necesidad esta en estado distinto a `Abierta`, el boton de editar no aparece y la API retorna error 400.

### Flujo Secundario: Cerrar Necesidad

El artista puede cerrar una necesidad en estado `Abierta` o `En Progreso`. Se pide un motivo opcional (texto libre, max 500 caracteres). Se muestra confirmacion previa: "Al cerrar esta necesidad, las propuestas pendientes seran rechazadas automaticamente." Las propuestas pendientes pasan automaticamente a estado `Rechazada` con motivo "Necesidad cerrada por el artista". La necesidad cerrada ya no aparece en el listado publico (US-CS-03) pero sigue visible en el historial del artista.

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-02-1 | El artista autenticado puede crear una necesidad con todos los campos obligatorios validados |
| AC-02-2 | La necesidad se crea con estado `Abierta` y aparece inmediatamente en el listado del artista y en el listado publico. Se muestra toast: "Necesidad publicada correctamente" |
| AC-02-3 | El `ArtistaId` y `ProyectoArtisticoId` se asignan automaticamente desde el contexto del usuario |
| AC-02-4 | Si la necesidad viene de un template, los campos se pre-rellenan pero son editables |
| AC-02-5 | El artista ve un listado paginado de sus necesidades con badge de color segun estado: Abierta (verde), En Progreso (azul), Cerrada (gris), Cancelada (rojo) |
| AC-02-6 | Se muestra un contador de propuestas recibidas en cada tarjeta |
| AC-02-7 | El artista puede filtrar por estado y buscar por texto libre |
| AC-02-8 | Si no hay necesidades, se muestra empty state con CTA hacia "Publicar necesidad" o "Usar una plantilla" |
| AC-02-9 | Solo se puede editar una necesidad en estado `Abierta`. Si tiene propuestas, se muestra aviso. Al guardar se actualiza `FechaActualizacion`. Si estado != Abierta, la API retorna 400 |
| AC-02-10 | Al cerrar una necesidad, las propuestas pendientes pasan automaticamente a `Rechazada` y se muestra confirmacion previa |
| AC-02-11 | Click en una necesidad del listado navega al detalle con sus propuestas recibidas |
| AC-02-12 | La necesidad cerrada ya no aparece en el listado publico pero sigue visible en el historial del artista |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| POST | /api/crowdsourcing/necesidades | Crear necesidad | Artista |
| GET | /api/crowdsourcing/necesidades/mis-necesidades | Listar necesidades del artista | Artista |
| GET | /api/crowdsourcing/necesidades/{id} | Detalle de necesidad | Artista (propietario) |
| PUT | /api/crowdsourcing/necesidades/{id} | Editar necesidad (solo si Abierta) | Artista (propietario) |
| PATCH | /api/crowdsourcing/necesidades/{id}/cerrar | Cerrar necesidad | Artista (propietario) |

**Query params para listado:** `estado`, `page`, `pageSize`, `search`

---

## US-CS-03: Explorar Necesidades y Enviar Propuestas

> **Feature Name:** `cs-explorar-propuestas`
> **Prioridad:** Alta
> **Estimacion:** L (Large)
> **Dependencias:** US-CS-02

---

### Historia de Usuario

**Como** profesional de la industria musical (productor, disenador, fotografo, etc.),
**quiero** buscar necesidades abiertas, enviar propuestas con mi precio y condiciones, hacer seguimiento de mis propuestas y poder retirarlas si es necesario,
**para** encontrar oportunidades de trabajo y ofrecer mis servicios a artistas en la plataforma.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Profesional | Usuario autenticado con perfil profesional (PerfilProfesional) |

### Precondiciones

- Usuario tiene cuenta activa
- Para enviar propuestas: debe tener PerfilProfesional creado

### Postcondiciones

- Propuestas creadas en estado `Pendiente`
- Artista puede ver y evaluar las propuestas recibidas

---

### Flujo Principal: Explorar Necesidades Abiertas

**Filtros disponibles:**

| Filtro | Tipo | Descripcion |
|--------|------|-------------|
| Tipo de necesidad | Multi-select | Filtrar por categoria (Produccion, Diseno, Marketing, etc.) |
| Modalidad de trabajo | Select | Presencial, Remoto, Hibrido |
| Rango de presupuesto | Range slider | Min - Max en moneda seleccionada |
| Ubicacion (pais) | Select/Autocomplete | Solo para presencial/hibrido |
| Ubicacion (ciudad) | Texto | Filtrar por ciudad |
| Ordenar por | Select | Mas recientes, Mayor presupuesto, Fecha limite proxima |
| Busqueda por texto | Input | Busca en titulo y descripcion |

**Datos a mostrar por necesidad:**

| Dato | Fuente |
|------|--------|
| Titulo | NecesidadCrowdsourcing.Titulo |
| Descripcion (truncada a 150 chars) | NecesidadCrowdsourcing.Descripcion |
| Tipo de necesidad (badge) | MaestraTipoNecesidad.Nombre |
| Rango de presupuesto | PresupuestoMin - PresupuestoMax + Moneda |
| Modalidad (icono) | MaestraModalidadTrabajo.Nombre |
| Ubicacion | UbicacionCiudad, UbicacionPais |
| Nombre del artista | Artista.NombreArtistico |
| Fecha publicacion | FechaCreacion (hace X dias) |
| Fecha limite propuestas | FechaLimitePropuestas (con urgencia si < 3 dias) |
| Numero de propuestas | COUNT(PropuestaCrowdsourcing) |

### Flujo Secundario: Enviar Propuesta

**Campos del formulario:**

| Campo | Tipo | Obligatorio | Validacion |
|-------|------|-------------|------------|
| Precio propuesto | Decimal | Si | > 0 |
| Moneda | Select | Si | Debe existir en maestras |
| Tiempo estimado de entrega (dias) | Entero | No | > 0, max 365 |
| Mensaje de propuesta | Texto largo | Si | Min 20 caracteres, max 2000 |
| Perfil profesional | Automatico | Si | Debe tener PerfilProfesional activo |

El precio propuesto puede estar fuera del rango del presupuesto de la necesidad (el profesional decide su precio libremente), pero se muestra una nota si el precio esta fuera del rango.

### Flujo Secundario: Ver Mis Propuestas

**Datos a mostrar por propuesta:**

| Dato | Fuente |
|------|--------|
| Titulo de la necesidad | NecesidadCrowdsourcing.Titulo |
| Nombre del artista | Artista.NombreArtistico |
| Mi precio propuesto | PropuestaCrowdsourcing.PrecioPropuesto + Moneda |
| Estado (badge) | MaestraEstadoPropuesta.Nombre |
| Fecha de envio | PropuestaCrowdsourcing.FechaCreacion |
| Fecha de respuesta | PropuestaCrowdsourcing.FechaActualizacion |

### Flujo Secundario: Retirar Propuesta

Solo se puede retirar una propuesta en estado `Pendiente`. Se muestra dialogo de confirmacion: "Retirar propuesta? Esta accion no se puede deshacer." La propuesta pasa a estado `Retirada`. El artista deja de ver esta propuesta en su listado de candidatos (o la ve en gris/tachada). El profesional puede ver la propuesta retirada en su historial pero no puede re-enviarla (deberia crear una nueva).

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-03-1 | Solo se muestran necesidades en estado `Abierta` en el listado publico |
| AC-03-2 | El listado es paginado (12 items por pagina) con filtros aplicados en tiempo real (debounce 300ms) |
| AC-03-3 | Las necesidades cuya fecha limite esta a menos de 3 dias muestran badge de urgencia |
| AC-03-4 | Las necesidades expiradas (fecha limite pasada) no se muestran |
| AC-03-5 | Un profesional no puede enviar mas de una propuesta por necesidad |
| AC-03-6 | El artista propietario de la necesidad no puede enviarse propuestas a si mismo |
| AC-03-7 | La propuesta se crea con estado `Pendiente` y se vincula automaticamente el `UserId` y `PerfilProfesionalId`. Se muestra toast: "Propuesta enviada correctamente. El artista sera notificado." |
| AC-03-8 | Si el profesional no tiene perfil profesional, se muestra CTA para crear uno antes de poder enviar la propuesta |
| AC-03-9 | El profesional ve un listado paginado de sus propuestas con badge de estado: Pendiente (amarillo), Aceptada (verde), Rechazada (rojo), Retirada (gris). Se puede filtrar por estado |
| AC-03-10 | Click en propuesta aceptada navega al detalle del acuerdo generado. Las propuestas pendientes muestran boton para retirar |
| AC-03-11 | Solo se puede retirar una propuesta en estado `Pendiente`, con dialogo de confirmacion previo. El artista deja de verla o la ve en gris/tachada |
| AC-03-12 | Si no hay resultados en la busqueda de necesidades, se muestra empty state con sugerencia de ampliar filtros |
| AC-03-13 | El profesional puede ver el detalle de una necesidad aunque no tenga perfil profesional, pero necesita perfil para enviar propuesta |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| GET | /api/crowdsourcing/necesidades | Listar necesidades abiertas (publico) | Autenticado |
| GET | /api/crowdsourcing/necesidades/{id} | Detalle de necesidad con boton "Enviar propuesta" | Autenticado |
| POST | /api/crowdsourcing/necesidades/{necesidadId}/propuestas | Enviar propuesta | Autenticado + PerfilProfesional |
| GET | /api/crowdsourcing/propuestas/mis-propuestas | Listar propuestas del profesional | Autenticado |
| PATCH | /api/crowdsourcing/propuestas/{id}/retirar | Retirar propuesta pendiente | Autenticado (propietario) |

**Query params para necesidades:** `tipoNecesidadId`, `modalidad`, `presupuestoMin`, `presupuestoMax`, `pais`, `orderBy`, `page`, `pageSize`, `search`

**Query params para mis propuestas:** `estado`, `page`, `pageSize`

---

## US-CS-04: Acuerdos, Milestones y Entregables

> **Feature Name:** `cs-acuerdos-entregables`
> **Prioridad:** Alta
> **Estimacion:** XL (Extra Large)
> **Dependencias:** US-CS-03

---

### Historia de Usuario

**Como** artista o profesional participante de una contratacion,
**quiero** gestionar el ciclo de vida completo del acuerdo de trabajo: aceptar/rechazar propuestas, definir milestones, subir y revisar entregables, y completar o cancelar el acuerdo,
**para** llevar un control estructurado del trabajo contratado desde la formalizacion hasta la entrega final.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Artista | Acepta/rechaza propuestas, define milestones, aprueba entregables, completa/cancela acuerdos |
| Profesional | Sube entregables, puede cancelar acuerdos |

### Precondiciones

- Existe al menos una propuesta en estado `Pendiente` para una necesidad del artista

### Postcondiciones

- Acuerdo creado, gestionado y completado/cancelado
- Entregables aprobados por el artista
- Necesidad actualizada segun estado del acuerdo

---

### Flujo Principal: Aceptar Propuesta y Crear Acuerdo

1. El artista revisa las propuestas recibidas para una necesidad.
2. Selecciona una propuesta y hace click en "Aceptar propuesta".
3. Se muestra un resumen con los datos del acuerdo que se va a crear.
4. El artista puede ajustar datos del acuerdo (titulo interno, fecha inicio, fecha fin prevista).
5. Al confirmar, se crea el acuerdo y se actualizan estados.

**Datos del acuerdo generado automaticamente:**

| Campo del Acuerdo | Origen |
|-------------------|--------|
| NecesidadId | De la necesidad |
| PropuestaId | De la propuesta aceptada |
| ArtistaId | Del artista autenticado |
| UserIdProveedor | Del profesional que envio la propuesta |
| PerfilProfesionalId | Del perfil del profesional |
| ImporteTotalPactado | PrecioPropuesto de la propuesta |
| MonedaId | De la propuesta |
| EstadoAcuerdoId | "Activo" |
| TituloInterno | Editable (default: titulo de la necesidad) |
| FechaInicio | Editable (default: hoy) |
| FechaFinPrevista | Editable (default: hoy + DiasEstimados de la propuesta) |

**Efectos colaterales:**
- Propuesta aceptada pasa a estado `Aceptada`
- Demas propuestas pendientes pasan a `Rechazada` con motivo "Otra propuesta fue aceptada"
- Necesidad pasa a estado `En Progreso`
- Se crea automaticamente una `ConversacionCrowdsourcing` vinculada al acuerdo

### Flujo Secundario: Rechazar Propuesta

Solo se puede rechazar una propuesta en estado `Pendiente`. Se solicita motivo opcional (max 500 chars). La propuesta pasa a estado `Rechazada`. El profesional puede ver el estado pero no el motivo de rechazo.

### Flujo Secundario: Ver Detalle de Acuerdo

**Secciones de la vista:**

| Seccion | Contenido |
|---------|-----------|
| Cabecera | Titulo, estado (badge), nombres de las partes, importe total, fechas, anticipo |
| Milestones | Listado ordenado con titulo, importe, fecha limite, estado. Barra de progreso visual |
| Entregables | Listado con titulo, estado, fecha, enlace al recurso. Agrupados por milestone |
| Conversacion | Link/boton para ir a la conversacion vinculada |
| Valoraciones | Reviews dejadas (si acuerdo completado) o CTA para valorar |

Solo los dos participantes del acuerdo pueden ver su detalle. Las acciones disponibles dependen del rol del usuario y del estado del acuerdo:
- **Artista** puede: definir milestones, aprobar/rechazar entregables, completar acuerdo, cancelar acuerdo, valorar.
- **Profesional** puede: subir entregables, marcar milestones como completados, cancelar acuerdo, valorar.

Se muestra un timeline/historial de actividad reciente del acuerdo.

### Flujo Secundario: Definir Milestones

**Campos por milestone:**

| Campo | Tipo | Obligatorio | Validacion |
|-------|------|-------------|------------|
| Titulo | Texto (max 200) | Si | Min 3 caracteres |
| Descripcion | Texto largo | No | Max 1000 caracteres |
| Orden | Entero | Automatico | Secuencial, editable con drag & drop |
| Importe parcial | Decimal | Si | > 0 |
| Porcentaje parcial | Decimal | Calculado | Automatico sobre el total |
| Fecha limite | Date | No | >= fecha inicio del acuerdo |

La suma de importes parciales debe ser <= ImporteTotalPactado. Se muestra en tiempo real: "Asignado: X de Y EUR (Z% del total)". Solo el artista puede crear/editar milestones mientras el acuerdo esta `Activo`. Cada milestone se crea con FechaCompletado = null (pendiente). Se pueden editar milestones existentes siempre que no esten marcados como completados. No se puede eliminar un milestone con entregables asociados.

### Flujo Secundario: Subir Entregable

**Campos del formulario:**

| Campo | Tipo | Obligatorio | Validacion |
|-------|------|-------------|------------|
| Titulo | Texto (max 200) | Si | Min 3 caracteres |
| Descripcion | Texto largo | No | Max 1000 caracteres |
| URL del recurso | URL | No | URL valida (Dropbox, Drive, WeTransfer, etc.) |
| Milestone asociado | Select | No | Debe ser milestone del mismo acuerdo |

Solo el profesional participante del acuerdo puede subir entregables. Se crea con estado `Entregado`. MVP: Solo URLs externas, no upload de archivos.

### Flujo Secundario: Aprobar o Rechazar Entregable

Solo el artista puede revisar entregables en estado `Entregado`:
- **Aprobar**: Pasa a `Aprobado`. Se registra FechaAprobacion y ComentarioAprobacion (opcional, max 500 chars).
- **Rechazar**: Pasa a `Rechazado`. Comentario obligatorio (min 10 chars) para explicar que debe corregirse. El profesional puede subir nueva version.

Cuando todos los entregables de un milestone estan aprobados, se sugiere marcar el milestone como completado.

### Flujo Secundario: Completar Acuerdo

Solo el artista puede completar un acuerdo en estado `Activo`. Se muestra resumen (milestones completados, entregables aprobados, importe). Si hay entregables pendientes de revision, se muestra aviso. El acuerdo pasa a `Completado`, se registra FechaFinReal, y la necesidad pasa a `Cerrada`. Se habilitan las valoraciones.

### Flujo Secundario: Cancelar Acuerdo

Tanto el artista como el profesional pueden cancelar un acuerdo `Activo`. Se muestra dialogo de confirmacion con advertencia: "Cancelar un acuerdo es una accion irreversible. Ambas partes seran notificadas." Se requiere motivo obligatorio (min 20, max 1000 chars). El acuerdo pasa a `Cancelado`, se registra FechaFinReal = ahora. La necesidad vuelve a `Abierta` (el artista puede buscar otro profesional). Entregables y milestones se conservan como historial pero no pueden modificarse. Se registra quien cancelo y el motivo.

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-04-1 | Al aceptar una propuesta, se crea un `AcuerdoCrowdsourcing` con todos los datos derivados y la propuesta pasa a `Aceptada` |
| AC-04-2 | Las demas propuestas pendientes de la misma necesidad pasan automaticamente a `Rechazada` |
| AC-04-3 | La necesidad pasa a estado `En Progreso` y se crea una conversacion automatica. Se muestra toast: "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional." El artista es redirigido al detalle del acuerdo |
| AC-04-4 | Solo los dos participantes del acuerdo pueden ver su detalle |
| AC-04-5 | La vista de detalle muestra cabecera, milestones (con barra de progreso), entregables (agrupados por milestone), acceso a conversacion y timeline/historial de actividad reciente |
| AC-04-6 | Solo el artista puede definir milestones. La suma de importes parciales <= ImporteTotalPactado, con indicador visual en tiempo real. Cada milestone se crea con FechaCompletado = null |
| AC-04-7 | Solo el profesional puede subir entregables (con URL externa). Se crean con estado `Entregado`. Se muestra toast: "Entregable subido correctamente. El artista sera notificado." |
| AC-04-8 | Solo el artista puede aprobar (comentario opcional) o rechazar (comentario obligatorio min 10 chars) entregables |
| AC-04-9 | Un entregable rechazado permite al profesional subir nueva version |
| AC-04-10 | Al completar un acuerdo, se registra FechaFinReal, la necesidad pasa a `Cerrada` y se habilitan valoraciones. Se muestra toast: "Acuerdo completado. Puedes dejar una valoracion al profesional." |
| AC-04-11 | Al cancelar, se muestra dialogo de advertencia ("Cancelar un acuerdo es una accion irreversible"), se requiere motivo obligatorio, el acuerdo pasa a `Cancelado` y la necesidad vuelve a `Abierta` |
| AC-04-12 | Se rechaza propuesta individualmente con motivo opcional. El profesional no ve el motivo de rechazo |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| POST | /api/crowdsourcing/propuestas/{id}/aceptar | Aceptar propuesta y crear acuerdo | Artista (propietario necesidad) |
| PATCH | /api/crowdsourcing/propuestas/{id}/rechazar | Rechazar propuesta | Artista (propietario necesidad) |
| GET | /api/crowdsourcing/acuerdos/{id} | Detalle del acuerdo con milestones y entregables | Participante del acuerdo |
| POST | /api/crowdsourcing/acuerdos/{acuerdoId}/milestones | Crear milestone | Artista (participante) |
| PUT | /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id} | Editar milestone | Artista (participante) |
| DELETE | /api/crowdsourcing/acuerdos/{acuerdoId}/milestones/{id} | Eliminar milestone (sin entregables) | Artista (participante) |
| POST | /api/crowdsourcing/acuerdos/{acuerdoId}/entregables | Subir entregable | Profesional (participante) |
| PATCH | /api/crowdsourcing/entregables/{id}/aprobar | Aprobar entregable | Artista (participante) |
| PATCH | /api/crowdsourcing/entregables/{id}/rechazar | Rechazar entregable | Artista (participante) |
| PATCH | /api/crowdsourcing/acuerdos/{id}/completar | Completar acuerdo | Artista (participante) |
| PATCH | /api/crowdsourcing/acuerdos/{id}/cancelar | Cancelar acuerdo | Participante del acuerdo |

**Nota:** `POST /propuestas/{id}/aceptar` es transaccional: actualiza propuesta, rechaza otras, actualiza necesidad, crea acuerdo y crea conversacion.

---

## US-CS-05: Mensajeria entre Partes

> **Feature Name:** `cs-mensajeria`
> **Prioridad:** Media
> **Estimacion:** M (Medium)
> **Dependencias:** US-CS-02 (conversaciones sobre necesidades), US-CS-04 (conversaciones sobre acuerdos)

---

### Historia de Usuario

**Como** artista o profesional participante de una necesidad o acuerdo,
**quiero** comunicarme directamente con la otra parte mediante conversaciones con mensajes de texto y adjuntos,
**para** aclarar dudas, negociar condiciones, coordinar el trabajo y no perder mensajes importantes.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Artista | Inicia conversaciones sobre sus necesidades o acuerdos |
| Profesional | Inicia conversaciones sobre necesidades a las que envio propuesta o acuerdos |

### Precondiciones

- Ambos usuarios estan autenticados
- Existe relacion entre ambos (propuesta enviada o acuerdo activo)

### Postcondiciones

- Conversacion creada con mensajes intercambiados
- Mensajes marcados como leidos al ser vistos

---

### Flujo Principal: Iniciar Conversacion

Un artista puede iniciar conversacion con un profesional que envio propuesta. Un profesional puede iniciar conversacion con el artista de una necesidad a la que envio propuesta. Se requiere un asunto para la conversacion (texto, max 200 chars). En el contexto de un acuerdo, la conversacion se crea automaticamente al aceptar la propuesta (US-CS-04). No se permiten conversaciones duplicadas entre las mismas partes para el mismo contexto; si ya existe, se redirige a la existente. Se crea con FechaCreacion = ahora y FechaUltimoMensaje = null (hasta que se envie el primer mensaje).

### Flujo Secundario: Enviar Mensaje

**Campos del mensaje:**

| Campo | Tipo | Obligatorio | Validacion |
|-------|------|-------------|------------|
| Contenido | Texto | Si | Min 1 caracter, max 5000 caracteres |
| URL adjunto | URL | No | URL valida |

El mensaje se crea con Leido = false, FechaLeido = null. Se actualiza FechaUltimoMensaje de la conversacion. Los mensajes se muestran en orden cronologico (estilo chat). Mensajes propios a la derecha, del otro a la izquierda. Al abrir una conversacion, los mensajes no leidos del otro usuario se marcan como leidos automaticamente. MVP: Sin WebSocket/real-time. Polling cada 10 segundos o refresh manual.

### Flujo Secundario: Ver Conversaciones y No Leidos

**Datos a mostrar por conversacion:**

| Dato | Fuente |
|------|--------|
| Nombre de la otra parte | User (artista o profesional segun perspectiva) |
| Asunto de la conversacion | ConversacionCrowdsourcing.Asunto |
| Contexto (necesidad o acuerdo) | NecesidadCrowdsourcing.Titulo o AcuerdoCrowdsourcing.TituloInterno |
| Ultimo mensaje (preview truncado) | MensajeCrowdsourcing.Contenido (max 80 chars) |
| Fecha ultimo mensaje | ConversacionCrowdsourcing.FechaUltimoMensaje |
| Mensajes no leidos (badge numerico) | COUNT(Mensajes WHERE Leido = false AND remitente != yo) |

En el menu/navbar principal se muestra un icono de mensajeria con badge total de no leidos.

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-05-1 | Un artista puede iniciar conversacion con un profesional que envio propuesta a su necesidad. Se requiere un asunto (max 200 chars) |
| AC-05-2 | No se permiten conversaciones duplicadas entre las mismas partes para el mismo contexto |
| AC-05-3 | Si ya existe conversacion, se redirige a la existente en vez de crear una nueva |
| AC-05-4 | Solo los dos participantes de la conversacion pueden enviar mensajes |
| AC-05-5 | Los mensajes se muestran en orden cronologico con estilo chat (propios a derecha, otros a izquierda) |
| AC-05-6 | Al abrir una conversacion, los mensajes no leidos del otro se marcan como leidos automaticamente |
| AC-05-7 | El listado de conversaciones muestra badge numerico de no leidos y se ordena por fecha del ultimo mensaje. Las conversaciones con mensajes no leidos aparecen primero o con indicador visual destacado |
| AC-05-8 | Se puede filtrar conversaciones por contexto: Todas, Sobre Necesidades, Sobre Acuerdos |
| AC-05-9 | En el navbar se muestra icono de mensajeria con badge total de mensajes no leidos |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| POST | /api/crowdsourcing/conversaciones | Iniciar conversacion | Autenticado (con relacion) |
| GET | /api/crowdsourcing/conversaciones | Listar mis conversaciones (con count no leidos) | Autenticado |
| GET | /api/crowdsourcing/conversaciones/{id}/mensajes | Mensajes de una conversacion | Participante |
| POST | /api/crowdsourcing/conversaciones/{id}/mensajes | Enviar mensaje | Participante |
| PATCH | /api/crowdsourcing/conversaciones/{id}/marcar-leidos | Marcar mensajes como leidos | Participante |

**Body para crear conversacion:** `{ necesidadId?, acuerdoId?, userIdDestinatario, asunto }`

---

## US-CS-06: Valoraciones Bidireccionales

> **Feature Name:** `cs-valoraciones`
> **Prioridad:** Baja
> **Estimacion:** S (Small)
> **Dependencias:** US-CS-04 (requiere acuerdos completados)

---

### Historia de Usuario

**Como** artista o profesional que ha completado un acuerdo de trabajo,
**quiero** dejar una valoracion (puntuacion 1-5 y comentario) a la otra parte y poder consultar las valoraciones de cualquier usuario,
**para** compartir mi experiencia, contribuir a la reputacion en la plataforma y ayudar a otros a tomar decisiones informadas.

### Actores

| Actor | Descripcion |
|-------|-------------|
| Artista | Valora al profesional tras completar un acuerdo |
| Profesional | Valora al artista tras completar un acuerdo |
| Cualquier usuario | Consulta las valoraciones de otro usuario |

### Precondiciones

- Existe un acuerdo en estado `Completado` entre las partes
- El usuario no ha valorado previamente a la otra parte en ese acuerdo

### Postcondiciones

- Valoracion registrada y visible publicamente
- Puntuacion media del usuario valorado actualizada

---

### Flujo Principal: Dejar Valoracion

**Campos del formulario:**

| Campo | Tipo | Obligatorio | Validacion |
|-------|------|-------------|------------|
| Puntuacion | Rating (1-5 estrellas) | Si | 1-5 |
| Tipo de valoracion | Automatico | - | Se asigna segun contexto |
| Comentario | Texto largo | No | Max 1000 caracteres |

La valoracion del artista al profesional y la del profesional al artista son independientes y no se condicionan mutuamente. Se puede valorar en cualquier momento despues de que el acuerdo se complete (sin fecha limite). No se puede editar una valoracion una vez enviada.

### Flujo Secundario: Ver Valoraciones de un Usuario

**Datos a mostrar:**

| Dato | Fuente |
|------|--------|
| Puntuacion media | AVG(ValoracionCrowdsourcing.Puntuacion) con 1 decimal |
| Numero total de valoraciones | COUNT(ValoracionCrowdsourcing) |
| Distribucion por estrellas | Histograma (cuantas de 5, 4, 3, 2, 1) |
| Listado de valoraciones | Puntuacion + Comentario + Nombre del autor + Fecha |

La puntuacion media tambien se muestra como badge en el perfil del profesional cuando aparece en propuestas y en listados.

---

### Criterios de Aceptacion

| ID | Criterio |
|----|----------|
| AC-06-1 | Solo se puede valorar a la otra parte de un acuerdo en estado `Completado` |
| AC-06-2 | Cada parte solo puede dejar una valoracion por acuerdo |
| AC-06-3 | La puntuacion es de 1 a 5 estrellas con seleccion visual (hover effect) |
| AC-06-4 | El comentario es opcional pero se incentiva con mensaje: "Tu comentario ayuda a otros artistas/profesionales" |
| AC-06-5 | No se puede editar una valoracion una vez enviada. Se muestra toast: "Valoracion enviada. Gracias por tu feedback." |
| AC-06-6 | Cualquier usuario autenticado puede ver las valoraciones de otro usuario |
| AC-06-7 | Se muestra puntuacion media (1 decimal), total de valoraciones e histograma de distribucion |
| AC-06-8 | Las valoraciones se listan por fecha (mas recientes primero) con paginacion |
| AC-06-9 | Si el usuario no tiene valoraciones, se muestra: "Este usuario aun no tiene valoraciones" |

### Especificacion Tecnica

**API Endpoints:**

| Metodo | Ruta | Descripcion | Auth |
|--------|------|-------------|------|
| POST | /api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones | Crear valoracion | Participante (acuerdo completado) |
| GET | /api/crowdsourcing/usuarios/{userId}/valoraciones | Ver valoraciones de un usuario | Autenticado |

---

## Resumen de Prioridades para MVP

### Fase 1 - Flujo E2E basico

| US | Feature | Razon |
|----|---------|-------|
| US-CS-01 | cs-templates-guia | Diferenciador clave y punto de entrada |
| US-CS-02 | cs-gestionar-necesidades | Core del artista |
| US-CS-03 | cs-explorar-propuestas | Core del profesional |
| US-CS-04 | cs-acuerdos-entregables | Flujo de contratacion y entrega |

### Fase 2 - Comunicacion y reputacion

| US | Feature | Razon |
|----|---------|-------|
| US-CS-05 | cs-mensajeria | Comunicacion directa entre partes |
| US-CS-06 | cs-valoraciones | Sistema de reputacion |

---

## Fuentes de Investigacion

- [Cuanto cuesta producir una cancion en 2025](https://casayaxkin.com/cuanto-cuesta-producir-una-cancion-en-2025/)
- [Cuanto cuesta lanzar un album como artista independiente](https://promocionmusical.es/cuanto-cuesta-lanzar-album-como-artista-independiente/)
- [Music Industry Basics 2025 Guide](https://www.musicgateway.com/blog/music-industry/music-industry-basics-2025-guide-for-artists-and-professionals)
- [Music Video Cost Investment Guide](https://pixflow.net/blog/how-much-does-a-music-video-cost-an-investment-guide-for-artists/)
- [Average Music Video Production Cost 2025](https://dotmotions.ae/blog/average-music-video-production-cost/)
- [Real Cost of DIY Touring 2025](https://roughdraft.qoncertapp.com/broke-on-tour-the-real-cost-of-diy-touring-in-2025-and-how-to-make-it-work/)
- [Music Tour Budget Guide](https://www.eventric.com/news/music-tour-budget/)
- [Mixing Engineer Rates](https://www.twine.net/blog/mixing-engineer-rates/)
- [Mastering Engineer Rates](https://www.twine.net/blog/mastering-engineer-rates/)
- [Album Cover Pricing 2026](https://sotuland.com/blog/album-cover-pricing-guide/)
- [Music PR Campaign Cost](https://blog.sonicbids.com/how-much-should-a-music-pr-campaign-cost)
- [Best Release Checklist for Independent Musicians](https://blog.symphonic.com/2025/01/06/the-best-release-checklist-for-independent-musicians-2/)
- [Music Photography Rates](https://ishootshows.com/music-photography-rates-pricing-spreadsheet/)
- [Freelance Social Media Manager Rates 2025](https://ruul.io/blog/freelance-social-media-manager-rates)
- [Music Production Roles Explained](https://92mmm.net/music-production-roles/)
- [Independent Music Services - Music Gateway](https://www.musicgateway.com/)
