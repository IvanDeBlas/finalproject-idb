# Nuevos Modulos: Content Licensing & Sponsorship

> Documento de analisis y diseno inicial para dos nuevos modulos de WePlay Rises
> que extienden el ecosistema CrowdPromotion.
>
> Fecha: 2026-02-17
> Estado: Propuesta / Vision futura (post-MVP)

---

## 1. Contexto: El Ecosistema Completo de WePlay Rises

WePlay Rises se sustenta en **4 pilares** que cubren todo el ciclo de vida de un proyecto artistico:

```
                        WEPLAY RISES
                            |
    ┌───────────┬───────────┼───────────┬────────────────┐
    |           |           |           |                |
CROWDFUNDING CROWDSOURCING CROWD      CONTENT         SPONSORSHIP
    |           |           PROMOTION   LICENSING        |
"Financia"   "Contrata"     "Difunde"  "Licencia"      "Patrocina"
    |           |           |           |                |
Fan aporta   Profesional  Fan/Influencer  Marca usa   Marca invierte
dinero       ofrece       comparte y     contenido    en artista/
al proyecto  servicios    gana comision  del artista  campania
```

### Como se relacionan los 4 pilares

| Pilar | Pregunta clave | Input | Output | Quien paga |
|-------|---------------|-------|--------|------------|
| **CrowdFunding** | "Como financia el artista su proyecto?" | Fans donan/invierten | Artista recibe fondos | Fan |
| **CrowdSourcing** | "Como consigue talento el artista?" | Profesionales proponen servicios | Artista contrata | Artista |
| **CrowdPromotion** | "Como amplifica su alcance?" | Fans/Influencers difunden | Mas visibilidad, mas backings | Artista (comisiones) |
| **Content Licensing** | "Como monetiza su contenido?" | Marcas buscan contenido | Artista cobra licencia | Marca/Anunciante |
| **Sponsorship** | "Como atrae inversion de marcas?" | Marcas buscan asociarse | Artista recibe patrocinio | Marca/Sponsor |

### Flujo cruzado entre pilares

```
1. Artista crea CAMPANIA (CrowdFunding)
2. Contrata productor via NECESIDAD (CrowdSourcing)
3. Fans difunden la campania via PROMOPROGRAM (CrowdPromotion)
4. Marca descubre al artista y LICENCIA su cancion para un anuncio (Content Licensing)
5. Otra marca PATROCINA la siguiente campania del artista (Sponsorship)
6. El patrocinio genera mas visibilidad → mas backings → ciclo virtuoso
```

---

## 2. MODULO: CONTENT LICENSING (Licencia de Contenido)

### 2.1 Vision General

**Content Licensing** permite a los artistas monetizar su contenido creativo (musica, videos, imagenes, samples) licenciandolo a marcas, agencias de publicidad, creadores de contenido y productoras para uso comercial.

Es el equivalente a plataformas como **Songtradr**, **Musicbed**, **Epidemic Sound** y **Artlist**, pero integrado en el ecosistema de WePlay Rises, donde el artista ya tiene presencia, audiencia y credibilidad.

### 2.2 Benchmarks de la Industria

| Plataforma | Modelo | Que ofrece | Revenue Share |
|-----------|--------|-----------|---------------|
| **Songtradr** | Marketplace abierto | Sync licensing para TV, film, ads, retail | Variable, artista retiene derechos |
| **Musicbed** | Curado, premium | Licencias para filmmakers y agencias | Non-exclusive y exclusive deals |
| **Epidemic Sound** | Suscripcion | Royalty-free, 35K+ tracks | Flat fee por suscripcion |
| **Artlist** | Suscripcion | Royalty-free + stock footage | Desde $9.99/mes |
| **Lickd** | Por uso | Mainstream music para social media (TikTok, Reels) | Por uso individual |
| **UnitedMasters** | Revenue share | Sync + distribucion + brand deals | Artista retiene master |

**Mercado global**: El mercado de sync licensing genera entre $600-650M anuales (2025). El mercado total de licencias musicales fue valorado en $5.8B en 2024, proyectado a $12.1B para 2033 (CAGR 8.6%).

### 2.3 Tipos de Licencia Aplicables

Basado en los estandares de la industria musical:

| Tipo de Licencia | Descripcion | Uso tipico | Rango de precio |
|-----------------|-------------|-----------|-----------------|
| **Sync License** | Sincronizar musica con contenido visual | Anuncios TV/digital, peliculas, series, videojuegos | $500 - $1,000,000 |
| **Master License** | Uso de la grabacion original (master) | Cuando se quiere LA version del artista, no un cover | Negociable |
| **Micro-Sync** | Licencia simplificada para social media | TikTok ads, Instagram Reels, YouTube Shorts | $50 - $500 |
| **Blanket License** | Acceso a catalogo completo por periodo | Agencias, productoras con uso recurrente | Suscripcion mensual/anual |
| **Sample License** | Uso de fragmentos/samples en producciones | Otros artistas, DJs, productores | $100 - $5,000 |
| **Performance License** | Uso en eventos, locales, streaming | Bares, restaurantes, eventos corporativos | $200 - $2,000/ano |

### 2.4 Casos de Uso

#### UC-CL-01: Artista Publica Contenido Licenciable

**Actor**: Artista
**Precondicion**: Artista tiene perfil activo y al menos un proyecto artistico

**Flujo principal**:
1. Artista accede a "Mi Catalogo de Licencias" en el dashboard
2. Selecciona "Nuevo Contenido Licenciable"
3. Sube el archivo (audio, video, imagen) o referencia contenido existente del proyecto
4. Completa metadatos:
   - Titulo, descripcion
   - Tipo de contenido (Cancion completa, Instrumental, Sample, Video, Imagen)
   - Genero musical, mood, tempo (BPM), tonalidad
   - Duracion
   - Tags/keywords para busqueda
5. Define terminos de licencia:
   - Tipos de licencia disponibles (sync, micro-sync, sample, etc.)
   - Precio base por tipo de licencia (o "negociable")
   - Exclusividad: si/no, y por cuanto tiempo
   - Territorios: mundial, por region
   - Plataformas permitidas: todas, solo digital, solo TV, etc.
   - Duracion maxima de licencia: 3 meses, 6 meses, 1 ano, perpetua
6. Publica el contenido en el marketplace

**Postcondicion**: Contenido visible para marcas en el marketplace

**Variantes**:
- 5a. Artista puede crear "paquetes" (cancion + instrumental + stems)
- 5b. Artista puede definir precios diferenciados por uso (ej: $200 para YouTube, $5000 para TV)

---

#### UC-CL-02: Marca Busca y Descubre Contenido

**Actor**: Marca / Agencia / Creador de contenido
**Precondicion**: Usuario con perfil de tipo "Marca" registrado

**Flujo principal**:
1. Marca accede al marketplace de contenido
2. Aplica filtros de busqueda:
   - Genero musical (Rock, Pop, Electronica, Latin, etc.)
   - Mood/Emocion (Energetico, Melancolico, Inspirador, Festivo)
   - Tempo/BPM (lento, medio, rapido)
   - Duracion (< 30s, 30s-1min, 1-3min, > 3min)
   - Tipo de uso pretendido (anuncio TV, social media, podcast, etc.)
   - Rango de presupuesto
   - Exclusividad requerida
3. Escucha previews (watermarked o fragmentos de 30s)
4. Agrega contenido a "Favoritos" o "Shortlist"
5. Puede solicitar escucha completa (requiere aprobacion del artista en algunos casos)
6. Ve detalle completo: artista, stats de la plataforma, licencias disponibles, precio

**Postcondicion**: Marca tiene shortlist de contenido para licenciar

**Variantes**:
- 2a. Busqueda por artista especifico (marca ya conoce al artista por CrowdFunding/Promotion)
- 2b. Recomendaciones AI basadas en campanas anteriores de la marca

---

#### UC-CL-03: Marca Solicita Licencia

**Actor**: Marca
**Precondicion**: Marca tiene contenido en shortlist

**Flujo principal**:
1. Marca selecciona contenido y tipo de licencia deseada
2. Completa formulario de solicitud:
   - Tipo de licencia (sync, micro-sync, blanket, etc.)
   - Uso especifico: descripcion del proyecto/campana publicitaria
   - Plataformas donde se usara
   - Territorio geografico
   - Duracion de uso
   - Fecha estimada de lanzamiento
   - Presupuesto ofrecido (si es negociable)
3. Sistema calcula precio segun tarifa del artista o envia solicitud de cotizacion
4. Se crea la solicitud de licencia en estado "Pendiente"
5. Artista recibe notificacion

**Postcondicion**: Solicitud creada, artista notificado

---

#### UC-CL-04: Artista Revisa y Negocia Solicitud

**Actor**: Artista
**Precondicion**: Solicitud de licencia pendiente

**Flujo principal**:
1. Artista ve solicitud con detalle del uso pretendido
2. Revisa perfil de la marca (sector, tamano, reputacion)
3. Opciones:
   a. **Aceptar** directamente al precio solicitado
   b. **Contra-ofertar** con precio/condiciones diferentes
   c. **Rechazar** con motivo opcional
4. Si contra-oferta: se inicia proceso de negociacion via mensajeria
5. Ambas partes pueden enviar hasta 3 rondas de negociacion
6. Al llegar a acuerdo, se genera el AcuerdoLicencia

**Postcondicion**: Acuerdo de licencia creado o solicitud rechazada

**Variantes**:
- 3a. Si el artista tiene "precio fijo" configurado, la marca puede comprar directamente sin negociacion (checkout inmediato)
- 4a. Artista puede pedir mas informacion sobre la campana antes de decidir

---

#### UC-CL-05: Ejecucion del Acuerdo de Licencia

**Actor**: Sistema / Artista / Marca
**Precondicion**: Acuerdo de licencia confirmado

**Flujo principal**:
1. Sistema genera contrato digital con los terminos acordados
2. Marca realiza pago (total o primer milestone):
   - Pago completo upfront
   - 50% al firmar, 50% al publicar
   - Escrow hasta verificacion de uso
3. Artista recibe acceso a descarga del contenido en alta calidad (sin watermark)
4. Marca recibe certificado de licencia con:
   - Codigo unico de licencia
   - Terminos de uso permitido
   - Fecha de vencimiento
5. Sistema activa monitoreo del periodo de licencia
6. Al vencer: notificacion a marca para renovar o cesar uso

**Postcondicion**: Contenido licenciado, artista cobrado, marca con certificado

---

#### UC-CL-06: Dashboard de Licencias del Artista

**Actor**: Artista
**Precondicion**: Artista tiene contenido licenciable publicado

**Flujo principal**:
1. Artista ve dashboard con:
   - Catalogo activo: cuantos contenidos publicados
   - Solicitudes pendientes de respuesta
   - Licencias activas (con detalle de marca, uso, vencimiento)
   - Ingresos por licencias: total historico, ultimo mes, por tipo
   - Contenido mas popular (mas vistas, mas solicitudes)
   - Licencias proximas a vencer (oportunidad de renovacion)
2. Puede gestionar catalogo: editar precios, pausar/activar contenido
3. Puede ver analytics: de donde vienen las marcas, que tipo de contenido buscan

**Postcondicion**: Artista tiene visibilidad completa de su negocio de licencias

---

#### UC-CL-07: Marca Crea Brief / Solicitud Abierta

**Actor**: Marca
**Precondicion**: Marca registrada

**Flujo principal**:
1. Marca no encuentra exactamente lo que busca en el catalogo
2. Crea un "Brief" o solicitud abierta:
   - Descripcion de la campana publicitaria
   - Tipo de contenido que necesita
   - Mood, genero, tempo deseado
   - Presupuesto disponible
   - Fecha limite de entrega
   - Referencia: "algo parecido a..." con ejemplos
3. Brief se publica como oportunidad para artistas
4. Artistas interesados postulan con contenido existente o propuesta de creacion a medida
5. Marca revisa postulaciones y selecciona

**Postcondicion**: Marca recibe propuestas de artistas

> **Nota**: Este caso de uso conecta con **CrowdSourcing invertido** -
> en lugar del artista buscando servicios, es la marca buscando contenido.

---

### 2.5 Modelo de Datos

#### Entidades principales

```
┌─────────────────────────┐
│   ContenidoLicenciable  │ (Aggregate Root)
├─────────────────────────┤
│ Id                      │ UNIQUEIDENTIFIER (PK)
│ ArtistaId               │ FK → Artista
│ ProyectoArtisticoId?    │ FK → ProyectoArtistico
│ TipoContenidoId         │ FK → MaestraTipoContenido
│ Titulo                  │ NVARCHAR(200)
│ Descripcion             │ NVARCHAR(MAX)
│ GeneroMusicalId         │ FK → MaestraGeneroMusical
│ MoodId?                 │ FK → MaestraMoodContenido
│ BPM?                    │ INT
│ Tonalidad?              │ NVARCHAR(10)
│ DuracionSegundos?       │ INT
│ UrlArchivoOriginal      │ NVARCHAR(500) -- Almacenamiento seguro
│ UrlPreview              │ NVARCHAR(500) -- Preview watermarked
│ UrlImagenPortada?       │ NVARCHAR(500)
│ Tags                    │ NVARCHAR(500) -- Comma-separated
│ EsExclusivoDisponible   │ BIT
│ EstadoContenidoId       │ FK → MaestraEstadoContenido
│ NumVisualizaciones      │ INT DEFAULT 0
│ NumSolicitudes          │ INT DEFAULT 0
│ NumLicenciasVendidas    │ INT DEFAULT 0
│ FechaPublicacion?       │ DATETIME2(3)
│ FechaCreacion           │ DATETIME2(3)
│ FechaActualizacion?     │ DATETIME2(3)
├─────────────────────────┤
│ → Tarifas[]             │
│ → Solicitudes[]         │
│ → Acuerdos[]            │
└─────────────────────────┘

┌─────────────────────────┐
│   TarifaLicencia        │
├─────────────────────────┤
│ Id                      │ UNIQUEIDENTIFIER (PK)
│ ContenidoLicenciableId  │ FK → ContenidoLicenciable
│ TipoLicenciaId          │ FK → MaestraTipoLicencia
│ TipoUsoId               │ FK → MaestraTipoUsoLicencia
│ MonedaId                │ FK → MaestraMoneda
│ PrecioBase              │ DECIMAL(18,2)
│ EsNegociable            │ BIT DEFAULT 1
│ PrecioMinimo?           │ DECIMAL(18,2) -- Floor para negociacion
│ DuracionMesesDefecto    │ INT DEFAULT 12
│ EsExclusiva             │ BIT DEFAULT 0
│ TerritorioId?           │ FK → MaestraTerritorioLicencia
│ PlataformasPermitidas?  │ NVARCHAR(500) -- JSON array
│ EsActiva                │ BIT DEFAULT 1
│ FechaCreacion           │ DATETIME2(3)
│ FechaActualizacion?     │ DATETIME2(3)
└─────────────────────────┘

┌─────────────────────────┐
│   PerfilMarca           │ (Aggregate Root)
├─────────────────────────┤
│ Id                      │ UNIQUEIDENTIFIER (PK)
│ UserId                  │ FK → Users
│ NombreComercial         │ NVARCHAR(200)
│ RazonSocial?            │ NVARCHAR(300)
│ SectorId                │ FK → MaestraSectorMarca
│ TamanoEmpresaId?        │ FK → MaestraTamanoEmpresa
│ PaisId?                 │ FK → MaestraPais
│ UrlSitioWeb?            │ NVARCHAR(300)
│ UrlLogo?                │ NVARCHAR(500)
│ Descripcion?            │ NVARCHAR(MAX)
│ PersonaContacto         │ NVARCHAR(200)
│ EmailContacto           │ NVARCHAR(200)
│ TelefonoContacto?       │ NVARCHAR(50)
│ EsVerificada            │ BIT DEFAULT 0
│ EsActiva                │ BIT DEFAULT 1
│ FechaCreacion           │ DATETIME2(3)
│ FechaActualizacion?     │ DATETIME2(3)
├─────────────────────────┤
│ → SolicitudesLicencia[] │
│ → AcuerdosLicencia[]    │
│ → Briefs[]              │
│ → AcuerdosPatrocinio[]  │ (compartida con Sponsorship)
└─────────────────────────┘

┌──────────────────────────────┐
│   SolicitudLicencia          │
├──────────────────────────────┤
│ Id                           │ UNIQUEIDENTIFIER (PK)
│ ContenidoLicenciableId       │ FK → ContenidoLicenciable
│ PerfilMarcaId                │ FK → PerfilMarca
│ TipoLicenciaId               │ FK → MaestraTipoLicencia
│ TipoUsoId                    │ FK → MaestraTipoUsoLicencia
│ EstadoSolicitudLicenciaId    │ FK → MaestraEstadoSolicitudLicencia
│ DescripcionUso               │ NVARCHAR(MAX) -- Para que campana
│ PlataformasUso               │ NVARCHAR(500) -- JSON: ["TV", "Instagram", "TikTok"]
│ TerritorioSolicitado         │ NVARCHAR(200)
│ DuracionMesesSolicitada      │ INT
│ ExclusividadRequerida        │ BIT DEFAULT 0
│ MonedaId                     │ FK → MaestraMoneda
│ ImporteOfrecido?             │ DECIMAL(18,2) -- Si el artista acepta negociacion
│ ImporteContraoferta?         │ DECIMAL(18,2) -- Contraoferta del artista
│ ImporteAcordado?             │ DECIMAL(18,2) -- Precio final acordado
│ FechaLanzamientoPrevista?    │ DATETIME2(3)
│ MotivoRechazo?               │ NVARCHAR(500)
│ FechaCreacion                │ DATETIME2(3)
│ FechaActualizacion?          │ DATETIME2(3)
├──────────────────────────────┤
│ → Mensajes[]                 │
└──────────────────────────────┘

┌─────────────────────────┐
│   AcuerdoLicencia       │
├─────────────────────────┤
│ Id                      │ UNIQUEIDENTIFIER (PK)
│ SolicitudLicenciaId?    │ FK → SolicitudLicencia (null si checkout directo)
│ ContenidoLicenciableId  │ FK → ContenidoLicenciable
│ PerfilMarcaId           │ FK → PerfilMarca
│ ArtistaId               │ FK → Artista
│ TipoLicenciaId          │ FK → MaestraTipoLicencia
│ EstadoAcuerdoLicenciaId │ FK → MaestraEstadoAcuerdoLicencia
│ MonedaId                │ FK → MaestraMoneda
│ ImporteTotal            │ DECIMAL(18,2)
│ ImporteComisionPlataforma │ DECIMAL(18,2) -- Fee de WePlay Rises
│ ImporteNetoArtista      │ DECIMAL(18,2)
│ EsExclusiva             │ BIT DEFAULT 0
│ TerritorioLicencia      │ NVARCHAR(200)
│ PlataformasPermitidas   │ NVARCHAR(500) -- JSON array
│ CodigoLicencia          │ NVARCHAR(50) -- Codigo unico tipo "WPR-LIC-XXXXX"
│ FechaInicioLicencia     │ DATETIME2(3)
│ FechaFinLicencia        │ DATETIME2(3)
│ FechaRenovacion?        │ DATETIME2(3)
│ UrlContratoDigital?     │ NVARCHAR(500) -- PDF generado
│ FechaCreacion           │ DATETIME2(3)
│ FechaActualizacion?     │ DATETIME2(3)
├─────────────────────────┤
│ → Pagos[]               │
└─────────────────────────┘

┌──────────────────────────┐
│   PagoLicencia           │
├──────────────────────────┤
│ Id                       │ UNIQUEIDENTIFIER (PK)
│ AcuerdoLicenciaId        │ FK → AcuerdoLicencia
│ EstadoPagoId             │ FK → MaestraEstadoPago
│ MonedaId                 │ FK → MaestraMoneda
│ Importe                  │ DECIMAL(18,2)
│ TipoPagoId               │ FK → MaestraTipoPago (Upfront, Milestone, Renovacion)
│ MetodoPagoId?            │ FK → MaestraMetodoPago
│ ReferenciaExternaPago?   │ NVARCHAR(200) -- ID de Stripe/PayPal
│ FechaVencimiento?        │ DATETIME2(3)
│ FechaPago?               │ DATETIME2(3)
│ FechaCreacion            │ DATETIME2(3)
│ FechaActualizacion?      │ DATETIME2(3)
└──────────────────────────┘

┌─────────────────────────┐
│   BriefMarca            │ (Solicitud abierta de contenido)
├─────────────────────────┤
│ Id                      │ UNIQUEIDENTIFIER (PK)
│ PerfilMarcaId           │ FK → PerfilMarca
│ Titulo                  │ NVARCHAR(200)
│ Descripcion             │ NVARCHAR(MAX)
│ TipoContenidoBuscado    │ FK → MaestraTipoContenido
│ GeneroMusicalId?        │ FK → MaestraGeneroMusical
│ MoodDeseado?            │ NVARCHAR(200)
│ ReferenciaEjemplo?      │ NVARCHAR(MAX) -- Links o descripciones de referencia
│ MonedaId                │ FK → MaestraMoneda
│ PresupuestoMinimo?      │ DECIMAL(18,2)
│ PresupuestoMaximo?      │ DECIMAL(18,2)
│ FechaLimitePostulacion  │ DATETIME2(3)
│ FechaLimiteEntrega?     │ DATETIME2(3)
│ EstadoBriefId           │ FK → MaestraEstadoBrief
│ FechaCreacion           │ DATETIME2(3)
│ FechaActualizacion?     │ DATETIME2(3)
├─────────────────────────┤
│ → Postulaciones[]       │
└─────────────────────────┘

┌──────────────────────────┐
│   PostulacionBrief       │
├──────────────────────────┤
│ Id                       │ UNIQUEIDENTIFIER (PK)
│ BriefMarcaId             │ FK → BriefMarca
│ ArtistaId                │ FK → Artista
│ ContenidoLicenciableId?  │ FK → ContenidoLicenciable (si postula con existente)
│ MensajePropuesta         │ NVARCHAR(MAX)
│ UrlMuestraAdicional?     │ NVARCHAR(500) -- Si propone crear algo nuevo
│ MonedaId                 │ FK → MaestraMoneda
│ PrecioPropuesto          │ DECIMAL(18,2)
│ DiasEstimadosEntrega?    │ INT
│ EstadoPostulacionId      │ FK → MaestraEstadoPostulacion
│ FechaCreacion            │ DATETIME2(3)
│ FechaActualizacion?      │ DATETIME2(3)
└──────────────────────────┘

┌──────────────────────────┐
│   MensajeLicencia        │ (Chat solicitud/negociacion)
├──────────────────────────┤
│ Id                       │ UNIQUEIDENTIFIER (PK)
│ SolicitudLicenciaId?     │ FK → SolicitudLicencia
│ BriefMarcaId?            │ FK → BriefMarca
│ UserIdEmisor             │ FK → Users
│ Contenido                │ NVARCHAR(MAX)
│ UrlAdjunto?              │ NVARCHAR(500)
│ FechaLeido?              │ DATETIME2(3)
│ FechaCreacion            │ DATETIME2(3)
└──────────────────────────┘
```

#### Tablas Maestras (Content Licensing)

```
MaestraTipoContenido
  1 = Cancion Completa
  2 = Instrumental
  3 = Sample/Loop
  4 = Stems (pistas separadas)
  5 = Video Musical
  6 = Video Lyric
  7 = Imagen/Artwork
  8 = Podcast/Audio

MaestraGeneroMusical
  1 = Pop, 2 = Rock, 3 = Hip-Hop/Rap, 4 = Electronica/EDM
  5 = R&B/Soul, 6 = Latin/Reggaeton, 7 = Jazz, 8 = Classical
  9 = Folk/Indie, 10 = Country, 11 = Metal, 12 = Funk
  13 = Ambient, 14 = World Music, 15 = Otro

MaestraMoodContenido
  1 = Energetico, 2 = Relajado, 3 = Melancolico, 4 = Inspirador
  5 = Festivo, 6 = Dramatico, 7 = Romantico, 8 = Misterioso
  9 = Epico/Cinematico, 10 = Minimalista, 11 = Agresivo, 12 = Nostalgico

MaestraTipoLicencia
  1 = Sync (audiovisual)
  2 = Master Use
  3 = Micro-Sync (social media)
  4 = Blanket (catalogo completo)
  5 = Sample/Remix
  6 = Performance (eventos/locales)

MaestraTipoUsoLicencia
  1 = Anuncio TV Nacional
  2 = Anuncio TV Regional
  3 = Anuncio Digital/Online
  4 = Social Media (TikTok, Reels, Shorts)
  5 = Pelicula/Documental
  6 = Serie/Programa TV
  7 = Videojuego
  8 = Podcast
  9 = Evento Corporativo
  10 = Retail/Ambientacion
  11 = App Movil
  12 = Otro

MaestraTerritorioLicencia
  1 = Mundial, 2 = Europa, 3 = Norteamerica
  4 = Latinoamerica, 5 = Asia-Pacifico, 6 = Espana
  7 = USA, 8 = UK, 9 = Personalizado

MaestraEstadoContenido
  1 = Borrador, 2 = En Revision, 3 = Publicado
  4 = Pausado, 5 = Retirado

MaestraEstadoSolicitudLicencia
  1 = Pendiente, 2 = En Negociacion, 3 = Aceptada
  4 = Rechazada, 5 = Expirada, 6 = Cancelada

MaestraEstadoAcuerdoLicencia
  1 = Pendiente Pago, 2 = Activo, 3 = Vencido
  4 = Renovado, 5 = Cancelado, 6 = En Disputa

MaestraEstadoBrief
  1 = Abierto, 2 = En Seleccion, 3 = Adjudicado
  4 = Cerrado, 5 = Cancelado

MaestraEstadoPostulacion
  1 = Enviada, 2 = En Revision, 3 = Seleccionada
  4 = Rechazada, 5 = Retirada

MaestraSectorMarca
  1 = Tecnologia, 2 = Moda/Ropa, 3 = Alimentacion/Bebidas
  4 = Automocion, 5 = Entretenimiento, 6 = Deportes
  7 = Salud/Bienestar, 8 = Finanzas, 9 = Turismo/Viajes
  10 = Retail, 11 = Telecomunicaciones, 12 = Otro

MaestraTamanoEmpresa
  1 = Startup (1-10), 2 = Pequena (11-50), 3 = Mediana (51-250)
  4 = Grande (251-1000), 5 = Corporacion (1000+)
```

#### Diagrama de Relaciones

```
Artista ──1:N──> ContenidoLicenciable ──1:N──> TarifaLicencia
                        |
                        ├──1:N──> SolicitudLicencia <──N:1── PerfilMarca
                        |               |
                        |               └──1:N──> MensajeLicencia
                        |
                        └──1:N──> AcuerdoLicencia ──1:N──> PagoLicencia
                                        |
                                        └── FK → PerfilMarca
                                        └── FK → Artista

PerfilMarca ──1:N──> BriefMarca ──1:N──> PostulacionBrief
                                              |
                                              └── FK → Artista
                                              └── FK → ContenidoLicenciable
```

---

## 3. MODULO: SPONSORSHIP (Patrocinios)

### 3.1 Vision General

**Sponsorship** permite a marcas patrocinar campanas de crowdfunding, proyectos artisticos o artistas directamente, a cambio de visibilidad, asociacion de marca y acceso a la audiencia del artista.

Es el equivalente a lo que hacen plataformas como **MAX.Live** (artist x brand partnerships), **YouTube BrandConnect** (brand deals para creators), y modelos tradicionales de patrocinio musical como los que documenta **SponsorUnited** (870+ deals de marcas con artistas en 2023-24).

### 3.2 Benchmarks de la Industria

| Plataforma | Modelo | Que ofrece |
|-----------|--------|-----------|
| **MAX.Live** | Performance marketing | 7000+ artistas, matching engine marca-artista por audiencia |
| **YouTube BrandConnect** | Marketplace integrado | Brands proponen deals, creators definen rates, dynamic slot insertion |
| **SponsorUnited** | Inteligencia de patrocinios | Analytics de 870+ deals musica-marcas, benchmarking |
| **Upfluence** | Influencer marketing | Discovery, campaign management, pagos, analytics |
| **MagicLinks** | Affiliate + brand deals | Conexion creator-marca con tracking de performance |

**Datos del mercado**:
- Apple patrocino el Super Bowl Halftime Show 2025 con Kendrick Lamar
- Shaboozey firmo partnership con Jack Daniel's (tour + portavoz + contenido)
- Los brand deals con artistas musicales crecieron significativamente en 2023-24
- Las marcas buscan "emotional connection" que los artistas tienen con sus fans

### 3.3 Tipos de Patrocinio

| Tipo | Descripcion | Ejemplo | Duracion tipica |
|------|-------------|---------|-----------------|
| **Patrocinio de Campana** | Marca financia parte de una campana de crowdfunding | Logo en pagina de campana, mencion en updates | Duracion de la campana |
| **Patrocinio de Artista** | Marca se asocia con el artista como embajador | Artista menciona marca en redes, usa productos | 3-12 meses |
| **Product Placement** | Marca proporciona productos que aparecen en contenido | Instrumentos, ropa, tecnologia en videos/fotos | Por proyecto |
| **Tour/Event Sponsor** | Marca patrocina gira o evento del artista | Banner en escenario, mencion en entradas | Por evento/gira |
| **Content Sponsorship** | Marca patrocina creacion de contenido especifico | "Este video es presentado por [Marca]" | Por pieza de contenido |
| **Co-Branding** | Marca y artista crean producto/merch conjunto | Edicion limitada con diseno del artista | Por producto |

### 3.4 Casos de Uso

#### UC-SP-01: Artista Publica Oportunidad de Patrocinio

**Actor**: Artista
**Precondicion**: Artista con perfil activo, idealmente con campana o proyecto activo

**Flujo principal**:
1. Artista accede a "Oportunidades de Patrocinio" en dashboard
2. Selecciona "Nueva Oportunidad"
3. Define el paquete de patrocinio:
   - Titulo y descripcion de la oportunidad
   - Tipo de patrocinio (campana, artista, content, etc.)
   - Vinculacion: campana de crowdfunding, proyecto artistico, o general
   - Audiencia estimada: seguidores en redes, fans en plataforma, reach estimado
   - Beneficios para la marca (contraprestaciones):
     * Logo en pagina de campana (tamano, posicion)
     * Mencion en updates/newsletters (cuantas)
     * Posts en redes sociales del artista (cuantos, que plataformas)
     * Presencia en contenido audiovisual
     * Acceso a datos de audiencia anonimizados
     * Invitaciones a eventos exclusivos
     * Merch co-branded
   - Precio/rango del paquete
   - Sectores de marca preferidos / excluidos
4. Publica la oportunidad en el marketplace

**Postcondicion**: Oportunidad visible para marcas en el marketplace

**Variantes**:
- 3a. Artista puede crear multiples "tiers" de patrocinio (Bronce, Plata, Oro)
- 3b. Artista puede definir marcas/sectores excluidos (ej: no alcohol, no tabaco)

---

#### UC-SP-02: Marca Descubre y Explora Artistas para Patrocinar

**Actor**: Marca
**Precondicion**: PerfilMarca registrado y verificado

**Flujo principal**:
1. Marca accede al marketplace de patrocinios
2. Filtra oportunidades por:
   - Genero musical / estilo del artista
   - Tamano de audiencia (micro, medio, macro)
   - Tipo de patrocinio disponible
   - Rango de presupuesto
   - Sector compatible
   - Ubicacion geografica del artista/audiencia
3. Ve "Artista Match Score" basado en:
   - Alineacion de audiencia artista vs target de la marca
   - Engagement rate del artista
   - Historial de campanas exitosas en la plataforma
   - Compatibilidad de sector
4. Explora perfil completo del artista:
   - Bio, genero, redes sociales
   - Stats de la plataforma (campanas financiadas, backings recibidos)
   - Audiencia demografica (anonimizada)
   - Oportunidades de patrocinio disponibles
   - Reviews/valoraciones de patrocinios anteriores
5. Agrega a shortlist o inicia solicitud

**Postcondicion**: Marca tiene shortlist de artistas para contactar

---

#### UC-SP-03: Marca Solicita Patrocinio

**Actor**: Marca
**Precondicion**: Marca ha identificado artista/oportunidad

**Flujo principal**:
1. Marca selecciona oportunidad de patrocinio (o contacta artista directamente)
2. Completa formulario de solicitud:
   - Tipo de patrocinio deseado
   - Descripcion de la campana/objetivo de marca
   - Que busca la marca: awareness, engagement, leads, ventas
   - Presupuesto disponible
   - Duracion deseada
   - Entregables esperados del artista
   - Materiales que la marca proporcionara (logos, guidelines, productos)
   - KPIs deseados (impresiones, clicks, conversiones)
3. Solicitud se crea en estado "Pendiente"
4. Artista recibe notificacion

**Postcondicion**: Solicitud de patrocinio creada

**Variantes**:
- 1a. Marca responde a oportunidad publicada (precio ya definido)
- 1b. Marca propone deal personalizado fuera de oportunidades listadas

---

#### UC-SP-04: Negociacion y Acuerdo

**Actor**: Artista, Marca
**Precondicion**: Solicitud de patrocinio pendiente

**Flujo principal**:
1. Artista revisa solicitud y perfil de la marca
2. Verifica alineacion de valores (sector, tipo de marca, reputacion)
3. Opciones:
   a. **Aceptar** los terminos propuestos
   b. **Negociar**: proponer cambios en precio, entregables o condiciones
   c. **Rechazar**: con motivo opcional
4. Si negociacion: intercambio de propuestas via mensajeria (max 5 rondas)
5. Al llegar a acuerdo:
   - Se genera AcuerdoPatrocinio con terminos finales
   - Se definen milestones y entregables
   - Se establece calendario de pagos
   - Ambas partes confirman digitalmente

**Postcondicion**: Acuerdo de patrocinio confirmado con milestones

---

#### UC-SP-05: Ejecucion del Patrocinio

**Actor**: Artista, Marca, Sistema
**Precondicion**: Acuerdo de patrocinio confirmado

**Flujo principal**:
1. Marca realiza primer pago (o pago completo segun acuerdo):
   - Anticipo (ej: 50% al firmar)
   - Milestone payments (por entregable completado)
   - Pago final al cierre
2. Artista ejecuta entregables:
   - Publica posts en redes con mencion de marca
   - Incluye logo de marca en campana/contenido
   - Crea contenido co-branded
   - Asiste a eventos si aplica
3. Para cada entregable:
   a. Artista sube "prueba de ejecucion" (screenshot, URL, video)
   b. Marca revisa y aprueba o solicita ajustes
   c. Al aprobar: milestone marcado como completado
   d. Se libera pago correspondiente al milestone
4. Sistema trackea metricas automaticamente donde sea posible:
   - Impresiones del logo en pagina de campana
   - Clicks en enlaces de la marca
   - Menciones rastreadas

**Postcondicion**: Entregables ejecutados, pagos procesados, metricas registradas

---

#### UC-SP-06: Tracking y Reporting de Patrocinio

**Actor**: Marca, Artista
**Precondicion**: Patrocinio activo o completado

**Flujo principal**:
1. Dashboard de marca muestra:
   - Patrocinios activos con progreso de entregables
   - Metricas por patrocinio:
     * Impresiones totales estimadas
     * Clicks en enlaces de marca
     * Engagement rate de posts patrocinados
     * Conversiones atribuidas (si hay tracking)
   - ROI estimado por patrocinio
   - Comparativa entre diferentes artistas patrocinados
2. Dashboard de artista muestra:
   - Patrocinios activos con siguiente entregable pendiente
   - Ingresos por patrocinios (historico y actual)
   - Calendario de entregables y deadlines
   - Valoraciones recibidas de marcas

**Postcondicion**: Ambas partes tienen visibilidad de resultados

---

#### UC-SP-07: Valoracion Post-Patrocinio

**Actor**: Marca, Artista
**Precondicion**: Acuerdo de patrocinio completado

**Flujo principal**:
1. Al completarse todos los milestones, sistema solicita valoracion mutua
2. Marca valora al artista:
   - Puntuacion (1-5 estrellas)
   - Criterios: cumplimiento, calidad, comunicacion, profesionalismo
   - Comentario publico
   - Indicador: "Volveria a patrocinar? Si/No"
3. Artista valora a la marca:
   - Puntuacion (1-5 estrellas)
   - Criterios: claridad del brief, pago puntual, comunicacion, respeto creativo
   - Comentario publico
   - Indicador: "Aceptaria otro deal? Si/No"
4. Valoraciones se publican en perfiles respectivos

**Postcondicion**: Ambas partes tienen reputacion visible para futuros deals

---

### 3.5 Modelo de Datos

#### Entidades principales

```
┌─────────────────────────────────┐
│   OportunidadPatrocinio         │ (Aggregate Root)
├─────────────────────────────────┤
│ Id                              │ UNIQUEIDENTIFIER (PK)
│ ArtistaId                       │ FK → Artista
│ CampaniaCrowdfundingId?         │ FK → CampaniaCrowdfunding
│ ProyectoArtisticoId?            │ FK → ProyectoArtistico
│ TipoPatrocinioId                │ FK → MaestraTipoPatrocinio
│ Titulo                          │ NVARCHAR(200)
│ Descripcion                     │ NVARCHAR(MAX)
│ AudienciaEstimada?              │ INT -- Seguidores/reach total
│ MonedaId                        │ FK → MaestraMoneda
│ PrecioMinimo?                   │ DECIMAL(18,2)
│ PrecioMaximo?                   │ DECIMAL(18,2)
│ EsNegociable                    │ BIT DEFAULT 1
│ SectoresPreferidos?             │ NVARCHAR(500) -- JSON array de SectorId
│ SectoresExcluidos?              │ NVARCHAR(500) -- JSON array de SectorId
│ EstadoOportunidadId             │ FK → MaestraEstadoOportunidad
│ FechaLimiteRespuesta?           │ DATETIME2(3)
│ FechaCreacion                   │ DATETIME2(3)
│ FechaActualizacion?             │ DATETIME2(3)
├─────────────────────────────────┤
│ → Beneficios[]                  │
│ → Tiers[]                       │
│ → Solicitudes[]                 │
└─────────────────────────────────┘

┌──────────────────────────────────┐
│   BeneficioPatrocinio            │
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ OportunidadPatrocinioId          │ FK → OportunidadPatrocinio
│ TipoBeneficioId                  │ FK → MaestraTipoBeneficio
│ Descripcion                      │ NVARCHAR(500)
│ Cantidad?                        │ INT -- Ej: 3 posts, 1 banner
│ Plataforma?                      │ NVARCHAR(100) -- Instagram, TikTok, etc.
│ Orden                            │ INT DEFAULT 0
│ FechaCreacion                    │ DATETIME2(3)
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   TierPatrocinio                 │ (Bronce, Plata, Oro)
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ OportunidadPatrocinioId          │ FK → OportunidadPatrocinio
│ Nombre                           │ NVARCHAR(100) -- "Bronce", "Plata", "Oro"
│ Descripcion?                     │ NVARCHAR(500)
│ MonedaId                         │ FK → MaestraMoneda
│ Precio                           │ DECIMAL(18,2)
│ MaxSponsors?                     │ INT -- Cuantas marcas por tier
│ SponsorActuales                  │ INT DEFAULT 0
│ Orden                            │ INT DEFAULT 0
│ FechaCreacion                    │ DATETIME2(3)
│ FechaActualizacion?              │ DATETIME2(3)
├──────────────────────────────────┤
│ → BeneficiosTier[]               │ -- Que incluye cada tier
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   BeneficioTierPatrocinio        │ (M:N entre Tier y Beneficios)
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ TierPatrocinioId                 │ FK → TierPatrocinio
│ TipoBeneficioId                  │ FK → MaestraTipoBeneficio
│ Descripcion                      │ NVARCHAR(500)
│ Cantidad?                        │ INT
│ Plataforma?                      │ NVARCHAR(100)
└──────────────────────────────────┘

┌──────────────────────────────────────┐
│   SolicitudPatrocinio                │
├──────────────────────────────────────┤
│ Id                                   │ UNIQUEIDENTIFIER (PK)
│ OportunidadPatrocinioId?             │ FK → OportunidadPatrocinio (null si directo)
│ TierPatrocinioId?                    │ FK → TierPatrocinio
│ PerfilMarcaId                        │ FK → PerfilMarca
│ ArtistaId                            │ FK → Artista
│ TipoPatrocinioId                     │ FK → MaestraTipoPatrocinio
│ EstadoSolicitudPatrocinioId          │ FK → MaestraEstadoSolicitudPatrocinio
│ DescripcionCampanaMarca              │ NVARCHAR(MAX)
│ ObjetivoMarcaId                      │ FK → MaestraObjetivoMarca
│ MonedaId                             │ FK → MaestraMoneda
│ PresupuestoOfrecido                  │ DECIMAL(18,2)
│ DuracionMesesSolicitada              │ INT
│ EntregablesEsperados                 │ NVARCHAR(MAX) -- JSON detalle de lo esperado
│ KPIsDeseados?                        │ NVARCHAR(MAX) -- JSON de KPIs
│ MotivoRechazo?                       │ NVARCHAR(500)
│ FechaCreacion                        │ DATETIME2(3)
│ FechaActualizacion?                  │ DATETIME2(3)
├──────────────────────────────────────┤
│ → Mensajes[]                         │
└──────────────────────────────────────┘

┌──────────────────────────────────┐
│   AcuerdoPatrocinio              │
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ SolicitudPatrocinioId?           │ FK → SolicitudPatrocinio
│ OportunidadPatrocinioId?         │ FK → OportunidadPatrocinio
│ PerfilMarcaId                    │ FK → PerfilMarca
│ ArtistaId                        │ FK → Artista
│ TipoPatrocinioId                 │ FK → MaestraTipoPatrocinio
│ EstadoAcuerdoPatrocinioId        │ FK → MaestraEstadoAcuerdoPatrocinio
│ MonedaId                         │ FK → MaestraMoneda
│ ImporteTotal                     │ DECIMAL(18,2)
│ ImporteComisionPlataforma        │ DECIMAL(18,2)
│ ImporteNetoArtista               │ DECIMAL(18,2)
│ PorcentajeAnticipo?              │ DECIMAL(5,2)
│ ImporteAnticipo?                 │ DECIMAL(18,2)
│ FechaInicio                      │ DATETIME2(3)
│ FechaFinPrevista                 │ DATETIME2(3)
│ FechaFinReal?                    │ DATETIME2(3)
│ TerminosEspeciales?              │ NVARCHAR(MAX)
│ UrlContratoDigital?              │ NVARCHAR(500)
│ FechaCreacion                    │ DATETIME2(3)
│ FechaActualizacion?              │ DATETIME2(3)
├──────────────────────────────────┤
│ → Milestones[]                   │
│ → Entregables[]                  │
│ → Pagos[]                        │
│ → Metricas[]                     │
│ → Valoraciones[]                 │
│ → Mensajes[]                     │
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   MilestonePatrocinio            │
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ AcuerdoPatrocinioId              │ FK → AcuerdoPatrocinio
│ Titulo                           │ NVARCHAR(200)
│ Descripcion?                     │ NVARCHAR(500)
│ MonedaId                         │ FK → MaestraMoneda
│ ImporteParcial                   │ DECIMAL(18,2)
│ FechaLimite                      │ DATETIME2(3)
│ EstadoMilestoneId                │ FK → MaestraEstadoMilestone
│ FechaCompletado?                 │ DATETIME2(3)
│ Orden                            │ INT DEFAULT 0
│ FechaCreacion                    │ DATETIME2(3)
│ FechaActualizacion?              │ DATETIME2(3)
├──────────────────────────────────┤
│ → Entregables[]                  │
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   EntregablePatrocinio           │
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ AcuerdoPatrocinioId              │ FK → AcuerdoPatrocinio
│ MilestonePatrocinioId?           │ FK → MilestonePatrocinio
│ TipoBeneficioId                  │ FK → MaestraTipoBeneficio
│ Titulo                           │ NVARCHAR(200)
│ Descripcion?                     │ NVARCHAR(500)
│ UrlPruebaEjecucion?              │ NVARCHAR(500) -- Screenshot, URL del post, video
│ UrlRecurso?                      │ NVARCHAR(500) -- El recurso entregado
│ Plataforma?                      │ NVARCHAR(100)
│ EstadoEntregableId               │ FK → MaestraEstadoEntregable
│ FechaEntrega?                    │ DATETIME2(3)
│ FechaAprobacion?                 │ DATETIME2(3)
│ ComentarioRevision?              │ NVARCHAR(500)
│ FechaCreacion                    │ DATETIME2(3)
│ FechaActualizacion?              │ DATETIME2(3)
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   PagoPatrocinio                 │
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ AcuerdoPatrocinioId              │ FK → AcuerdoPatrocinio
│ MilestonePatrocinioId?           │ FK → MilestonePatrocinio
│ EstadoPagoId                     │ FK → MaestraEstadoPago
│ MonedaId                         │ FK → MaestraMoneda
│ Importe                          │ DECIMAL(18,2)
│ TipoPagoId                       │ FK → MaestraTipoPago
│ MetodoPagoId?                    │ FK → MaestraMetodoPago
│ ReferenciaExternaPago?           │ NVARCHAR(200)
│ FechaVencimiento?                │ DATETIME2(3)
│ FechaPago?                       │ DATETIME2(3)
│ FechaCreacion                    │ DATETIME2(3)
│ FechaActualizacion?              │ DATETIME2(3)
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   MetricaPatrocinio              │ (Tracking de resultados)
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ AcuerdoPatrocinioId              │ FK → AcuerdoPatrocinio
│ EntregablePatrocinioId?          │ FK → EntregablePatrocinio
│ TipoMetricaId                    │ FK → MaestraTipoMetrica
│ Valor                            │ DECIMAL(18,2) -- Impresiones, clicks, etc.
│ FechaMedicion                    │ DATETIME2(3)
│ FuenteDato?                      │ NVARCHAR(100) -- "manual", "api_instagram", etc.
│ FechaCreacion                    │ DATETIME2(3)
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   ValoracionPatrocinio           │ (Bidireccional)
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ AcuerdoPatrocinioId              │ FK → AcuerdoPatrocinio
│ UserIdEvaluador                  │ FK → Users
│ UserIdEvaluado                   │ FK → Users
│ EsEvaluacionDeMarca              │ BIT -- true=marca evalua artista, false=artista evalua marca
│ Puntuacion                       │ INT -- 1 a 5
│ PuntuacionCumplimiento?          │ INT -- 1 a 5
│ PuntuacionCalidad?               │ INT -- 1 a 5
│ PuntuacionComunicacion?          │ INT -- 1 a 5
│ PuntuacionProfesionalismo?       │ INT -- 1 a 5
│ Comentario?                      │ NVARCHAR(MAX)
│ VolveriaATrabajar                │ BIT
│ FechaCreacion                    │ DATETIME2(3)
└──────────────────────────────────┘

┌──────────────────────────────────┐
│   MensajePatrocinio              │ (Chat negociacion/ejecucion)
├──────────────────────────────────┤
│ Id                               │ UNIQUEIDENTIFIER (PK)
│ SolicitudPatrocinioId?           │ FK → SolicitudPatrocinio
│ AcuerdoPatrocinioId?             │ FK → AcuerdoPatrocinio
│ UserIdEmisor                     │ FK → Users
│ Contenido                        │ NVARCHAR(MAX)
│ UrlAdjunto?                      │ NVARCHAR(500)
│ FechaLeido?                      │ DATETIME2(3)
│ FechaCreacion                    │ DATETIME2(3)
└──────────────────────────────────┘
```

#### Tablas Maestras (Sponsorship)

```
MaestraTipoPatrocinio
  1 = Patrocinio de Campana
  2 = Patrocinio de Artista
  3 = Product Placement
  4 = Tour/Event Sponsor
  5 = Content Sponsorship
  6 = Co-Branding

MaestraTipoBeneficio
  1 = Logo en pagina de campana
  2 = Mencion en updates/newsletter
  3 = Post en redes sociales
  4 = Story/Reel dedicado
  5 = Presencia en video musical
  6 = Mencion en podcast/entrevista
  7 = Banner en evento/concierto
  8 = Merch co-branded
  9 = Acceso a datos de audiencia
  10 = Invitacion a evento exclusivo
  11 = Producto en escena (product placement)
  12 = Agradecimiento en creditos

MaestraObjetivoMarca
  1 = Brand Awareness
  2 = Engagement / Interaccion
  3 = Lead Generation
  4 = Ventas / Conversiones
  5 = Reposicionamiento de Marca
  6 = Lanzamiento de Producto
  7 = RSC / Responsabilidad Social

MaestraEstadoOportunidad
  1 = Borrador, 2 = Publicada, 3 = En Negociacion
  4 = Cerrada, 5 = Expirada

MaestraEstadoSolicitudPatrocinio
  1 = Pendiente, 2 = En Negociacion, 3 = Aceptada
  4 = Rechazada, 5 = Expirada, 6 = Cancelada

MaestraEstadoAcuerdoPatrocinio
  1 = Pendiente Firma, 2 = Activo, 3 = En Ejecucion
  4 = Completado, 5 = Cancelado, 6 = En Disputa

MaestraEstadoMilestone
  1 = Pendiente, 2 = En Progreso, 3 = Entregado
  4 = En Revision, 5 = Aprobado, 6 = Rechazado

MaestraEstadoEntregable
  1 = Pendiente, 2 = En Progreso, 3 = Entregado
  4 = En Revision, 5 = Aprobado, 6 = Requiere Cambios, 7 = Rechazado

MaestraTipoMetrica
  1 = Impresiones, 2 = Clicks, 3 = Engagement Rate
  4 = Alcance (Reach), 5 = Visualizaciones Video
  6 = Likes, 7 = Comentarios, 8 = Shares
  9 = Conversiones, 10 = Leads Generados
  11 = Ventas Atribuidas

MaestraEstadoPago (compartida con Content Licensing)
  1 = Pendiente, 2 = Procesando, 3 = Completado
  4 = Fallido, 5 = Reembolsado, 6 = En Escrow

MaestraTipoPago (compartida con Content Licensing)
  1 = Anticipo, 2 = Milestone, 3 = Pago Final
  4 = Renovacion, 5 = Bonus por Performance
```

#### Diagrama de Relaciones

```
Artista ──1:N──> OportunidadPatrocinio ──1:N──> BeneficioPatrocinio
                        |
                        ├──1:N──> TierPatrocinio ──1:N──> BeneficioTierPatrocinio
                        |
                        └──1:N──> SolicitudPatrocinio <──N:1── PerfilMarca
                                        |
                                        └──> AcuerdoPatrocinio
                                                |
                                                ├──1:N──> MilestonePatrocinio
                                                |               |
                                                |               └──1:N──> EntregablePatrocinio
                                                |
                                                ├──1:N──> PagoPatrocinio
                                                ├──1:N──> MetricaPatrocinio
                                                ├──1:N──> ValoracionPatrocinio
                                                └──1:N──> MensajePatrocinio
```

---

## 4. Entidad Compartida: PerfilMarca

Ambos modulos comparten la entidad **PerfilMarca**, que representa a empresas/marcas que interactuan con la plataforma tanto para licenciar contenido como para patrocinar artistas.

```
PerfilMarca (compartida)
    |
    ├── Content Licensing
    |   ├── SolicitudLicencia
    |   ├── AcuerdoLicencia
    |   └── BriefMarca
    |
    └── Sponsorship
        ├── SolicitudPatrocinio
        └── AcuerdoPatrocinio
```

**Recomendacion arquitectonica**: PerfilMarca deberia vivir en un modulo compartido (ej: `UserAccess` o un nuevo `BrandManagement`) ya que es consumida por multiples bounded contexts.

---

## 5. Integraciones entre Modulos

### 5.1 Content Licensing ↔ CrowdFunding

| Integracion | Descripcion |
|-------------|-------------|
| Descubrimiento | Marca ve campana de crowdfunding → descubre musica del artista → licencia |
| Visibilidad | Contenido licenciado en anuncio → mas visibilidad → mas backings a campana |
| Catalogo | Artista puede marcar canciones de un proyecto/campana como licenciables |

### 5.2 Sponsorship ↔ CrowdFunding

| Integracion | Descripcion |
|-------------|-------------|
| Patrocinio directo | Marca patrocina campana de crowdfunding → logo en pagina |
| Stretch goals | Si campana alcanza X, marca aporta Y adicional |
| Co-financiacion | Marca cubre parte del objetivo, fans el resto |

### 5.3 Content Licensing ↔ CrowdPromotion

| Integracion | Descripcion |
|-------------|-------------|
| Promotor sugiere | Promotor comparte contenido licenciable con marcas de su red |
| Comision por referido | Si promotor genera un deal de licencia, cobra comision |
| PromoEvento tracking | Nuevo tipo de evento: "LicenciaReferida" |

### 5.4 Sponsorship ↔ CrowdPromotion

| Integracion | Descripcion |
|-------------|-------------|
| Marca como promotor | Marca patrocinadora difunde campana del artista (win-win) |
| Tracking cruzado | Metricas de patrocinio alimentan PromoEventos |
| Amplificacion pagada | Marca paga pauta del contenido del artista (a traves de su presupuesto) |

### 5.5 Content Licensing ↔ CrowdSourcing

| Integracion | Descripcion |
|-------------|-------------|
| Produccion a medida | Marca publica Brief → Artista acepta → Contrata productor via Crowdsourcing |
| Creditos | Profesional contratado via Crowdsourcing aparece en creditos de contenido licenciado |

---

## 6. Modelo de Negocio / Revenue de WePlay Rises

| Fuente de ingreso | % Comision sugerida | Descripcion |
|-------------------|---------------------|-------------|
| CrowdFunding backing | 5-8% | Comision sobre cada aportacion de fan |
| CrowdSourcing acuerdo | 8-12% | Comision sobre acuerdos artista-profesional |
| CrowdPromotion conversion | 3-5% | Fee sobre comisiones pagadas a promotores |
| **Content Licensing deal** | **15-20%** | Comision sobre cada licencia vendida |
| **Sponsorship deal** | **10-15%** | Comision sobre acuerdos de patrocinio |

> La comision mas alta en Content Licensing se justifica porque la plataforma
> provee marketplace, discovery, negociacion, contrato y pago - todo el flujo.

---

## 7. Prioridad de Implementacion

### Fase 1: Foundation (Post-MVP)
- [ ] PerfilMarca (entidad compartida)
- [ ] ContenidoLicenciable + TarifaLicencia (CRUD basico)
- [ ] OportunidadPatrocinio + BeneficioPatrocinio (CRUD basico)
- [ ] Maestras de ambos modulos

### Fase 2: Marketplace
- [ ] Busqueda y filtrado de contenido licenciable
- [ ] Busqueda y filtrado de oportunidades de patrocinio
- [ ] SolicitudLicencia + SolicitudPatrocinio (flujo solicitud)

### Fase 3: Negociacion y Acuerdos
- [ ] MensajeLicencia + MensajePatrocinio (mensajeria)
- [ ] AcuerdoLicencia + AcuerdoPatrocinio (contratos)
- [ ] PagoLicencia + PagoPatrocinio (pagos)

### Fase 4: Ejecucion y Tracking
- [ ] Milestones + Entregables de patrocinio
- [ ] MetricaPatrocinio (tracking de resultados)
- [ ] Dashboard de licencias y patrocinios
- [ ] ValoracionPatrocinio (reputacion)

### Fase 5: Avanzado
- [ ] BriefMarca + PostulacionBrief (solicitudes abiertas)
- [ ] Integraciones cruzadas con CrowdFunding y CrowdPromotion
- [ ] AI matching artista-marca
- [ ] Renovacion automatica de licencias

---

## 8. Resumen de Entidades por Modulo

### Content Licensing (8 entidades nuevas)

| Entidad | Tipo | Descripcion |
|---------|------|-------------|
| ContenidoLicenciable | Aggregate Root | Contenido del artista disponible para licenciar |
| TarifaLicencia | Value Object | Precio por tipo de licencia/uso |
| SolicitudLicencia | Entity | Peticion de marca para licenciar contenido |
| AcuerdoLicencia | Entity | Contrato de licencia firmado |
| PagoLicencia | Entity | Pago asociado a una licencia |
| BriefMarca | Entity | Solicitud abierta de contenido |
| PostulacionBrief | Entity | Artista postula a un brief |
| MensajeLicencia | Entity | Mensajes de negociacion |

### Sponsorship (10 entidades nuevas)

| Entidad | Tipo | Descripcion |
|---------|------|-------------|
| OportunidadPatrocinio | Aggregate Root | Oportunidad publicada por artista |
| BeneficioPatrocinio | Value Object | Contraprestacion ofrecida |
| TierPatrocinio | Entity | Niveles de patrocinio (Bronce/Plata/Oro) |
| BeneficioTierPatrocinio | Junction | Beneficios por tier |
| SolicitudPatrocinio | Entity | Peticion de marca |
| AcuerdoPatrocinio | Entity | Contrato firmado |
| MilestonePatrocinio | Entity | Hitos del acuerdo |
| EntregablePatrocinio | Entity | Pruebas de ejecucion |
| PagoPatrocinio | Entity | Pagos del patrocinio |
| MetricaPatrocinio | Entity | KPIs y resultados |
| ValoracionPatrocinio | Entity | Evaluacion mutua |
| MensajePatrocinio | Entity | Chat de negociacion |

### Compartida (1 entidad nueva)

| Entidad | Tipo | Descripcion |
|---------|------|-------------|
| PerfilMarca | Aggregate Root | Perfil de empresa/marca (usada por ambos modulos) |

**Total: 21 entidades nuevas + ~25 tablas maestras**

---

## 9. Referencias y Fuentes

### Content Licensing / Sync
- [Songtradr - Marketplace de licencias musicales](https://www.songtradr.com/)
- [Musicbed - Licencias premium para film y advertising](https://www.musicbed.com/)
- [Epidemic Sound - Modelo de suscripcion royalty-free](https://www.epidemicsound.com/)
- [Lickd - Licencias de musica mainstream para social media](https://www.totallicensing.com/lickd-launches-industry-first-mainstream-music-licensing-platform-for-brands/)
- [GRAMMY - The Expanding Universe of Music Sync](https://www.grammy.com/news/music-sync-explainer-how-it-works-opportunities-getting-paid-future-of-sync)
- [Bridge.audio - Music industry predictions 2025](https://www.bridge.audio/blog/music-industry-predictions-for-2025-in-sync-promo-and-ar/)
- [TrackClub - 6 tipos de licencias musicales](https://www.trackclub.com/resources/types-of-music-licenses)
- [ReelCrafter - Como funciona sync licensing](https://www.reelcrafter.com/blog/how-sync-licensing-works)
- [Copyright Alliance - Licencias y royalties](https://copyrightalliance.org/music-licenses-and-royalties/)

### Sponsorship / Brand Partnerships
- [MAX.Live - Music Sponsorship & Artist Partners](https://www.max.live)
- [SponsorUnited - Brands & Top Music Artists Report](https://www.sponsorunited.com/insights/brands-top-music-artists-2023-24)
- [Billboard - Music Branding Power Players 2025](https://www.billboard.com/p/music-branding-power-players-2025/)
- [Ditto Music - How to Get Sponsored as a Musician 2026](https://dittomusic.com/en/blog/how-to-get-sponsored-as-a-musician)
- [YouTube BrandConnect - Brand deals para creators](https://support.google.com/youtube/answer/9385307)
- [InfluenceFlow - Brand Partnership Deals Guide 2026](https://influenceflow.io/resources/brand-partnership-deals-and-sponsorships-the-complete-2026-guide/)
- [InfluenceFlow - Creator Partnerships Workflows 2026](https://influenceflow.io/resources/creator-partnerships-and-collaboration-workflows-the-complete-2026-guide/)
- [Sprout Social - Influencer Contract Template](https://sproutsocial.com/insights/influencer-contract-template/)
- [Influencer Marketing Hub - Payment Terms & Milestones](https://influencermarketinghub.com/payment-terms-milestone-schedules-influencer-briefs/)
- [Sull & Lee Law - Influencer Contract Terms](https://sulleelaw.com/terms-matter-what-every-influencer-and-brand-should-include-in-their-contract/)

### Mercado
- [Business Research Insights - Copyright Music Market 2026-2035](https://www.businessresearchinsights.com/market-reports/copyright-music-market-116730)
- [Verified Market Reports - Music Licensing Services Market](https://www.verifiedmarketreports.com/product/music-licensing-services-market/)
- [Moises.ai - Music Sponsorship and Crowdfunding](https://moises.ai/blog/inspiration/crowdfunding-for-music-artists/)
