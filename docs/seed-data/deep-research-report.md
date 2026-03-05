# Fuentes web de crowdfunding y crowdsourcing musical para ingestión vía scraping y APIs en un MVP

## 1. Resumen ejecutivo

Este informe identifica y evalúa sitios web que publican **proyectos musicales** orientados a **crowdfunding** (financiación con objetivo, recaudado, recompensas, etc.) y **crowdsourcing** (colaboración creativa: “forks”, remixes, aportaciones no monetarias, etc.), con el objetivo de usarlos como **fuentes de ingesta** para un MVP (scraping/ETL) **sin realizar scraping real**. Las conclusiones más operativas para un MVP son:

La fuente más “MVP-friendly” para crowdfunding, por **calidad de datos, estabilidad y baja fricción técnica**, es el **API público de** entity["company","Indiegogo","crowdfunding platform"], que expone endpoints para listar proyectos activos y consultar detalle por `urlName` (incluyendo `campaignGoal`, `fundsGathered`, fechas, backers y contadores) y además indica que las respuestas se cachean por un periodo corto. citeturn36view2

Para mercado hispanohablante (y especialmente España), entity["company","Verkami","spanish crowdfunding platform"] ofrece una **gran concentración de proyectos musicales** (miles), con páginas HTML accesibles y campos muy completos (objetivo, recaudado, % financiado, mecenazgos, recompensas, entrega estimada, ubicación, etc.). En un listado de “Music” se observan **4.089 proyectos musicales** y filtros por subgénero, idioma, ordenación, etc. citeturn13view1turn13view0

Para “fan funding” recurrente (más cercano a “membresías” que a campañas con fecha fin), el **API de** entity["company","Patreon","membership crowdfunding platform"] es el mejor punto de entrada: documenta OAuth2, rate limits, endpoints v2 (`/identity`, `/campaigns`, `/members`, posts, webhooks) y recomienda enviar `User-Agent` para evitar 403. citeturn36view0turn36view1

En crowdsourcing creativo musical (sin financiación), los candidatos más alineados con “proyectos” colaborativos son:
- entity["company","BandLab","music creation and collaboration platform"], por su modelo explícito de contenido “Forkable” y trazabilidad mediante “Revision History” (útil como “proyecto” con contribuciones). citeturn32search16  
- entity["organization","HITRECORD","creative collaboration community"], donde el propio marco legal define “Project” y “Challenge” como mecanismos de colaboración/remix con atribución. citeturn31view2  
- entity["company","Kompoz","online music collaboration platform"] como plataforma de creación musical “crowdsourced”. citeturn32search5turn30search8  

En términos de **volumen** (historical backfill) y rapidez de arranque, los datasets mensuales de entity["company","Webrobots.io","web scraping datasets provider"] son una alternativa práctica para **Kickstarter/Indiegogo**, si tu MVP tolera depender de un tercero y del licenciamiento. Por ejemplo, su página de datasets de Kickstarter lista crawls mensuales con fechas recientes (p.ej., 2026-02-12). citeturn22search12turn7search5

Riesgo legal y TOS: en general, **APIs oficiales** (Indiegogo, Patreon) reducen riesgo frente a scraping. En crowdsourcing, varios TOS contienen restricciones explícitas contra bots/scrapers; por ejemplo, BandLab enumera “robots, spiders, scrapers, data mining tools, automated scripts” y limita patrones de solicitudes “más de lo que un humano puede producir”. citeturn34view3

## 2. Metodología y criterios de priorización

El análisis se basa en:
- Revisión de **documentación oficial de APIs** y artículos de soporte (Indiegogo, Patreon). citeturn36view2turn36view0turn36view1  
- Inspección de **páginas de proyecto y listados accesibles** (especialmente Verkami). citeturn13view0turn13view1  
- Revisión de **condiciones / términos** cuando estaban disponibles (Verkami, Ulule, BandLab, HITRECORD). citeturn14view0turn33view2turn34view3turn31view2  
- Evidencias indirectas de fricción técnica: contenido **JS-heavy** (Indiegogo, Ulule, Kompoz) y bloqueos 403 en sitios concretos desde entornos automatizados (p.ej. Kickstarter). citeturn10search6turn12view0turn5view0  

Criterios para priorizar fuentes:
- **Calidad/estructura**: disponibilidad de campos clave (objetivo, recaudado, fechas, backers, recompensas, estado).
- **Descubrimiento**: posibilidad de listar proyectos por categoría musical y paginar.
- **Accesibilidad técnica**: HTML estático vs. SPA/JS; presencia de CAPTCHAs; necesidad de login.
- **Riesgo legal / TOS**: presencia de prohibiciones explícitas a scraping; uso permitido vía API.
- **Volumen**: número aproximado de proyectos o señal clara de escala.

Cuando un dato no aparece en fuentes observables, se marca como **“no especificado”**.

## 3. Comparativa de plataformas y priorización

### Tabla comparativa de alto nivel

| Fuente | Tipo principal | Idioma/mercado | Método recomendado | Señales anti-scraping | Facilidad técnica | Riesgo legal/TOS |
|---|---|---|---|---|---|---|
| Indiegogo | Crowdfunding (campañas) | Internacional | API público | Páginas JS (si scrapeas HTML) | Fácil (API) | Medio (depende uso; API reduce) |
| Verkami | Crowdfunding (cultural) | ES + mult. | HTML + crawling de listados | reCAPTCHA en flujos / antifraude | Medio | Medio |
| Patreon | Membresías/financiación recurrente | Internacional | API oficial OAuth2 | Rate limits + `User-Agent` recomendado | Medio | Bajo–medio (si cumples API) |
| Ulule | Crowdfunding (collectes) | EU (FR, etc.) | HTML (si accesible) / “no especificado” | Página parece muy JS | Medio–difícil | Medio |
| BandLab | Crowdsourcing (colaboración musical) | Internacional | API/Acceso oficial (“no especificado”) | TOS menciona scrapers/bots | Difícil | Alto si scrapeas |
| HITRECORD | Crowdsourcing (proyectos/challenges) | Internacional | “no especificado” | Web dinámica (“no especificado”) | Difícil | Alto (licencias/contenido) |
| Kompoz | Crowdsourcing (colab musical) | Internacional | “no especificado” | Web dinámica (“no especificado”) | Difícil | Medio–alto |
| Kickstarter (directo) | Crowdfunding | Internacional | No recomendado (bloqueos) / dataset tercero | Bloqueos 403; backend no trivial | Difícil | Alto |
| Webrobots datasets | Datos (tercero) | Internacional | Descarga CSV/JSON | N/A (no scrapeas) | Fácil | Depende licencia/uso |

La evidencia de API para Indiegogo está en su documentación “Indiegogo Public API” con endpoints y campos. citeturn36view2  
La escala musical de Verkami se observa en el listado “Music – 4,089 music projects”. citeturn13view1  
El API v2 de Patreon (endpoints, rate limits, `User-Agent`) está documentado en `docs.patreon.com`. citeturn36view0turn36view1  
La restricción explícita sobre scrapers/bots en BandLab está en sus “Terms of Use”. citeturn34view3  
Los datasets de Kickstarter/Indiegogo publicados por Webrobots están documentados por el propio proveedor. citeturn22search12turn7search5  

### Lista priorizada de sitios con URLs base y ejemplo de proyecto

> Nota sobre enlaces: se incluyen como código para facilitar copiado.

**Prioridad alta (mejor equilibrio: volumen + calidad + viabilidad)**  
- entity["company","Indiegogo","crowdfunding platform"] — Base `https://www.indiegogo.com/` — Ejemplo `https://www.indiegogo.com/en/projects/jananishankar/black-album-campaign` citeturn7search0turn36view2  
- entity["company","Verkami","spanish crowdfunding platform"] — Base `https://www.verkami.com/` — Ejemplo `https://www.verkami.com/projects/41651-financiacion-del-primer-disco-de-la-banda-manena-parsimoniosa` citeturn13view0turn13view1  
- entity["company","Patreon","membership crowdfunding platform"] — Base `https://www.patreon.com/` — Ejemplo `no especificado` (depende del creador) — Doc API `https://docs.patreon.com/` citeturn36view0turn36view1  

**Prioridad media (útiles, pero más fricción o menos “campaña clásica”)**  
- entity["company","Ulule","crowdfunding platform france"] — Base `https://fr.ulule.com/` — Ejemplo `https://fr.ulule.com/estelle-bourgeois-album/` citeturn12view0turn33view2  
- entity["company","Webrobots.io","web scraping datasets provider"] — Base `https://webrobots.io/` — Datasets: `https://webrobots.io/kickstarter-datasets/` y `https://webrobots.io/indiegogo-dataset/` citeturn22search12turn7search5  

**Crowdsourcing (colaboración creativa musical; no crowdfunding clásico)**  
- entity["company","BandLab","music creation and collaboration platform"] — Base `https://www.bandlab.com/` — Ejemplo `no especificado` (URLs varían por post/proyecto) citeturn32search16turn34view3  
- entity["company","Kompoz","online music collaboration platform"] — Base `https://www.kompoz.com/` — Ejemplo `no especificado` (páginas vistas como dinámicas) citeturn32search5turn30search8  
- entity["organization","HITRECORD","creative collaboration community"] — Base `https://hitrecord.org/` — Ejemplo `no especificado` (estructura de URL/proyecto no confirmada) citeturn31view2turn30search14  

**Riesgo alto / difícil (no recomendado para scraping directo en MVP)**  
- entity["company","Kickstarter","crowdfunding platform"] — Base `https://www.kickstarter.com/` — Ejemplo `https://www.kickstarter.com/projects/thetwotracks/support-the-two-tracks-5th-album` (acceso automatizado con 403 en este entorno) citeturn5view0turn4search4turn4search0  

## 4. Fichas técnicas de scraping por plataforma

A continuación, para cada fuente se cubre: campos de “proyecto”, formato URL, paginación/discovery, APIs, acceso/autenticación, señales anti-scraping, facilidad y riesgos. Si no hay evidencia suficiente, se marca “no especificado”.

### Indiegogo

**Naturaleza de los “proyectos”**: campañas con objetivo, recaudación, recompensas, comentarios, etc. Existen campañas musicales (p.ej., álbumes). citeturn7search0turn7search22

**Estructura de “proyecto” (campos visibles / disponibles)**  
Recomendación: **usar el API público**, que expone explícitamente (entre otros):  
- `projectName` (título), `creatorName` (creador), `shortDescription`,  
- `campaignGoal` (objetivo), `fundsGathered` (recaudado), `currencyShortName`,  
- `campaignStartDate`, `campaignEndDate`,  
- `backerCount`, `rewardCount`, `commentCount`, `updateCount`,  
- `projectHomeUrl`, `projectImageUrl`. citeturn36view2turn8view2  

Campos “Recompensas”: el API da `rewardCount` pero **no detalla** tiers en el documento citado (detalle: **no especificado**). citeturn36view2

**Formato de URL**  
Ejemplos observados siguen patrón con idioma y dos slugs:  
- `https://www.indiegogo.com/en/projects/<creatorUrlName>/<projectUrlName>` citeturn7search0turn7search22  

**Paginación / discovery**  
- `GET /api/public/projects/getActiveCrowdfundingProjects` devuelve una lista de proyectos activos ordenados por fecha de inicio. No se documentan parámetros de paginación en el fragmento observado (**no especificado**). citeturn36view2  

**APIs públicas/privadas**  
- Documentación pública: `https://help.indiegogo.com/article/616-indiegogo-public-api` citeturn36view2turn8view2  
- Endpoints (según doc):  
  - `GET /api/public/creators/getCreator?urlName=...`  
  - `GET /api/public/projects/getActiveCrowdfundingProjects`  
  - `GET /api/public/projects/getCrowdfundingProject?urlName=...` citeturn8view2turn36view2  
- Nota de cache: “All public endpoint results are cached for a short duration.” citeturn8view2turn36view2  

**Requisitos de autenticación**: en la doc citada no se menciona autenticación (interpretación: endpoints públicos), por tanto **no especificado**. citeturn36view2  

**Señales anti-scraping**  
- Si intentas scraping HTML, hay evidencia de que el sitio renderiza contenido “mostly via JavaScript”, complicando `requests` + XPath directo. citeturn10search6  

**Facilidad técnica estimada**: **Fácil (API)** / **Difícil (HTML)**. citeturn36view2turn10search6  

**Riesgos legales/TOS**  
- Existe “Terms of Use” general. La investigación no extrae una cláusula específica anti-scraping de ese texto (en el material abierto), por lo que se marca **no especificado** respecto a scraping; aun así, usar API tiende a reducir riesgo. citeturn7search2turn36view2  

### Verkami

**Naturaleza**: crowdfunding cultural muy cercano a “campaña clásica” (objetivo + recaudado + recompensas). Para música hay listados por subgénero. citeturn13view1turn13view0  

**Estructura de página de proyecto (campos visibles)**  
En una página de proyecto musical se observan, legibles y con estructura clara:
- **Título** y texto resumen. citeturn13view0  
- **Creador** (“A project of”) con enlace a perfil. citeturn13view0  
- **Categoría** (p.ej., Music). citeturn13view0  
- **Ubicación** (“Created in Palma”). citeturn13view0  
- **Estado**: se muestra como “Project crowdfunded on [fecha] — Presale closed!” (en el ejemplo, finalizado con éxito). citeturn13view0  
- **Objetivo** y **recaudado**: “8.805€ From 8.420€” (equivalente a recaudado y objetivo). citeturn13view0  
- **% financiado**: “105%”. citeturn13view0  
- **Número de aportaciones**: “142 Pledges”. citeturn13view0  
- **Recompensas**: lista de tiers con precio, #backers por recompensa, título, descripción, y “Estimated delivery”. citeturn13view0  
- **Comentarios**: aparece una sección “Community” (contenido total no cuantificado en el fragmento). citeturn13view0  
Campos “fecha inicio” y “fecha fin” como tal: en el ejemplo se ve la fecha de cierre (“Project crowdfunded on February 06, 2026”), pero el inicio explícito no aparece en el extracto. Por tanto, **inicio: no especificado**; **fin: derivable del texto**. citeturn13view0  

**Formato de URL**  
- Proyecto: `https://www.verkami.com/projects/<id>-<slug>` citeturn13view0turn16search8  
- Variantes de idioma: `https://www.verkami.com/locale/<lang>/projects/<id>-<slug>` (observado en `/locale/ca/`). citeturn16search8  

**Paginación / discovery**  
- Listado de música: `https://www.verkami.com/discover/projects/category/10-musica` muestra “4,089 music projects” y un botón “Load more projects” (indicando paginación/infinite scroll). citeturn13view1  
- Filtros visibles: subgéneros (Classic, Electronic…), ordenación, idioma. citeturn13view1  

**APIs públicas/privadas**: no se observa documentación pública de API en el material revisado (**no especificado**).

**Requisitos de autenticación**  
- Para **ver** proyectos/listados: no se requiere login en el ejemplo. citeturn13view0turn13view1  
- Para participar (contribuir, login): existen flujos de autenticación y el sitio indica protección por reCAPTCHA en el área de registro/login. citeturn13view0  

**Señales anti-scraping**  
- Mención explícita de reCAPTCHA en el sitio (en contexto de autenticación), lo cual suele correlacionar con medidas anti-bot en flujos sensibles. citeturn13view0  
- Listados con “Load more projects” sugieren fetch dinámico; si hay endpoints internos, pueden cambiar (**riesgo de rotura**). citeturn13view1  

**Facilidad técnica estimada**: **Medio**. Hay HTML legible con campos ricos, pero paginación puede ser dinámica y hay señales anti-bot en flujos (aunque no necesariamente para lectura). citeturn13view0turn13view1  

**Riesgos legales/TOS**  
- Existe documento de “Contract conditions and Privacy Policy”. No se localizaron palabras clave “robot/crawler/scrap” en la búsqueda simple del texto observado, por tanto cláusula anti-scraping **no especificada** en este informe. citeturn14view0turn15view0turn15view1  

### Patreon

**Naturaleza**: financiación recurrente y “tiers/benefits” (similar a “recompensas”), más que campañas con fecha fin.

**Estructura (campos disponibles vía API)**  
Patreon API v2 (JSON:API) permite obtener:
- objeto “Campaign” y relaciones con “Tier”, “Benefit”, “Goal”, “Member”, “Post”, etc. citeturn36view0  
- Endpoints principales:  
  - `GET /api/oauth2/v2/identity`  
  - `GET /api/oauth2/v2/campaigns`  
  - `GET /api/oauth2/v2/campaigns/{campaign_id}`  
  - `GET /api/oauth2/v2/campaigns/{campaign_id}/members`  
  - `GET /api/oauth2/v2/posts/{id}` y similares. citeturn36view0  

Mapeo a campos de “proyecto” pedido:
- Título: depende del atributo del “Campaign” (no listado explícitamente en el fragmento; **no especificado** en esta cita).  
- Creador: relación `creator`/`User`. citeturn36view0  
- Objetivo económico: relación `goals` (si configurado). citeturn36view0  
- Recompensas: `tiers` y `benefits`. citeturn36view0  
- Fechas inicio/fin: “fin” no aplica; “inicio” como campaña activa en plataforma (campo exacto **no especificado** en este informe).  
- Número de patrocinadores: `members` endpoint (requiere scopes). citeturn36view0  
- Comentarios: `posts` existen; comentarios como tal **no especificado** en el extracto abierto.

**Autenticación y límites**  
- OAuth2; documentación paso a paso en el propio sitio. citeturn36view0  
- Recomendación explícita: incluir `User-Agent` o se arriesga 403. citeturn36view0  
- El índice de documentación incluye “Rate Limits” y “Edge Rate Limiting”. citeturn36view0  
- Soporte: Patreon indica que desde 2020 no ofrece “developer support”, aunque endpoints siguen funcionando; remite a docs. citeturn36view1turn36view0  

**Facilidad técnica estimada**: **Medio** (OAuth + scopes + JSON:API). citeturn36view0turn36view1  

**Riesgos legales/TOS**: **Bajo–medio** si se consume estrictamente por API con credenciales propias y conforme a políticas; para scraping HTML, no evaluado aquí.

### Ulule

**Naturaleza**: campañas (“Collectes”) con contribuciones y, según sus definiciones, también existe “Collecte par Abonnements” (sin fecha fin, donación mensual o puntual, con posibles contreparties). citeturn33view2  

**Estructura de proyecto**  
En este entorno, al abrir una página de proyecto, el contenido visible extraído es principalmente de secciones (“À propos du projet”, “FAQ”, etc.), pero no se observan en el extracto campos como objetivo/recaudado/backers (probable render dinámico o extracción limitada). Por ello, campos principales: **no especificado** desde la página HTML observada. citeturn12view0  

**Formato de URL**  
- Proyecto: `https://fr.ulule.com/<slug>/` citeturn12view0turn12view1  

**API**: no especificado.

**TOS**  
- CGU accesibles en `https://fr.ulule.com/pages/about/terms/` e incluyen definiciones de “Objectif de Financement”, “Période de collecte”, etc. citeturn33view2  

**Facilidad técnica estimada**: **Medio–difícil** (por señales de render/estructura limitada en extracción). citeturn12view0turn33view2  

**Riesgo legal/TOS**: **Medio** (no se identificó cláusula anti-scraping específica en el fragmento citado; no especificado).

### BandLab

**Naturaleza**: crowdsourcing/colaboración musical, con “forks” (derivaciones) como mecanismo. Esto encaja con “proyectos” creativos donde el “crowd” contribuye.

**Estructura (campos / señales disponibles)**
- BandLab documenta explícitamente configuraciones de contenido: `Private`, `Public`, `Forkable`, y menciona “Revision History” para rastrear versiones y cómo el contenido se ha desarrollado. citeturn32search16  
Esto sugiere un modelo de datos típico: título/obra, autor, estado de compartición, historial, forks, etc. Campos exactos y selectores HTML: **no especificado**.

**Restricciones anti-scraping y rate limiting (muy relevante)**  
- En TOS se prohíbe el uso de “automated means, bots, botnets, robots, spiders, scrapers, data mining tools, automated scripts” (y también “repeated manual clicks”) para generar acciones/outputs e incluso se menciona explícitamente enviar más requests de los que un humano razonable podría producir. citeturn34view3  

**Facilidad técnica estimada**: **Difícil** (por TOS + probable dinamismo/login).  
**Riesgo legal/TOS**: **Alto** si se pretende scraping automatizado directo (el propio texto menciona scrapers/robots). citeturn34view3  

### HITRECORD

**Naturaleza**: crowdsourcing creativo (incluye música) basado en proyectos/challenges; útil si tu MVP también quiere importar “oportunidades colaborativas” (no dinero).

**Estructura y modelo conceptual (desde sus términos)**  
- Los términos definen: “Challenge” como invitación para contribuir y “Project” como colaboración entre usuarios creada a partir de uno o más challenges. citeturn31view2  
- También se enfatiza que la finalidad es modificar y remixar contribuciones, con obligaciones de atribución en ciertos casos. citeturn31view2  

Campos tipo “objetivo económico/recaudado” no aplican normalmente; campos de colaboración (tipo reto, contribuciones, attribution) sí.

**Selectors/URL/page structure**: no especificado (en este entorno la home se comporta como dinámica). citeturn31view1turn31view2  

**Riesgo legal/TOS**: **Alto** si se pretende reutilización de contenido; para importar solo metadatos puede ser menor, pero el marco de licencias es complejo. citeturn31view2  

### Kompoz

**Naturaleza**: colaboración musical “crowdsourced”. citeturn30search8turn32search5  

**Estructura/URL**: se observan rutas tipo `/community/...` pero el detalle de página y campos no se obtuvieron en extracto (probable dinamismo). **No especificado**. citeturn32search5turn31view0  

**TOS**: existe `https://www.kompoz.com/terms-of-service` (contenido no extraído en este entorno). citeturn32search2turn33view0  

**Facilidad**: **Difícil** (por dinamismo/estructura no visible).  
**Riesgo**: **Medio–alto** (depende de TOS; no especificado).

### Kickstarter (scraping directo) y alternativa de datasets

**Dificultad técnica / señales anti-bot**  
- En este entorno, al intentar abrir páginas de Kickstarter aparece (403) Forbidden. citeturn5view0turn5view2  
- Hay evidencias públicas de que usa un endpoint GraphQL `/graph` y que pueden aparecer 403 en automatización. citeturn4search3  

**Riesgo legal/TOS (señal)**  
- Existe un texto de “Privacy Center” que incluye la prohibición de “crawl/spider/scrape” (según el extracto indexado). citeturn3search7  
*(Dado que no se pudo abrir el documento completo aquí, la cláusula se toma del fragmento disponible; el detalle exacto debe validarse directamente en la fuente original.)*

**Recomendación para MVP**  
- Evitar scraping directo al inicio; preferir un dataset de terceros como el de Webrobots (si el licenciamiento encaja) o fuentes alternativas. Webrobots lista crawls mensuales recientes para Kickstarter. citeturn22search12  

## 5. Modelo de datos y ejemplos de selectores

### Esquema de importación propuesto

Diseña un “modelo canónico” que puedas poblar desde crowdfunding y crowdsourcing, con campos opcionales:

- `source_site`  
- `project_url`  
- `project_id` (si existe)  
- `title`  
- `creator_name`  
- `creator_url`  
- `category`  
- `description_short`  
- `description_long`  
- `goal_amount`  
- `amount_raised`  
- `currency`  
- `start_date`  
- `end_date`  
- `status` (e.g., `live`, `successful`, `failed`, `ended`, `subscription`)  
- `backers_count`  
- `followers_count`  
- `comments_count`  
- `rewards` (array: `[{price, title, description, backers, delivery_estimate}]`)  
- `location_text`  
- `tags` (array)  
- `media_urls` (array)  
- `crowdsourcing` (obj opcional: `is_forkable`, `forks_count`, `revision_history_url`, etc.)

### Mapeo por fuente (campos clave)

**Indiegogo (API público) → canónico**  
Basado en el ejemplo de respuesta del documento: citeturn8view2turn36view2  
- `title` ← `projectName`  
- `creator_name` ← `creatorName`  
- `goal_amount` ← `campaignGoal`  
- `amount_raised` ← `fundsGathered`  
- `start_date` ← `campaignStartDate`  
- `end_date` ← `campaignEndDate`  
- `backers_count` ← `backerCount`  
- `comments_count` ← `commentCount`  
- `project_url` ← `projectHomeUrl`  
- `media_urls[0]` ← `projectImageUrl`  
- `status` ← derivar por fase (la doc indica fases soportadas en `/getCrowdfundingProject`; el valor exacto no figura en el ejemplo → **no especificado**) citeturn8view2turn36view2  

**Verkami (HTML) → canónico**  
Basado en el contenido visible del proyecto de ejemplo: citeturn13view0  
- `title` ← texto del `h1` principal (observado)  
- `creator_name` ← enlace tras “A project of”  
- `category` ← enlace tras “Category”  
- `location_text` ← texto tras “Created in”  
- `goal_amount` ← número tras “From …€”  
- `amount_raised` ← número “8.805€” (principal)  
- `backers_count` ← “Pledges”  
- `status` ← “Project crowdfunded” + “Presale closed!” (derivar `successful/ended`)  
- `rewards[]` ← bloques de recompensas (precio, backers, título, descripción, “Estimated delivery”)

**Patreon (API v2) → canónico**  
Basado en endpoints/relaciones listadas: citeturn36view0turn36view1  
- `title` ← atributo de Campaign (**no especificado en este extracto**)  
- `creator_name` ← relación `creator` / User  
- `goal_amount` ← `goals` (si existen)  
- `rewards[]` ← `tiers` + beneficios  
- `backers_count` ← contar `members` (con scopes)  
- `status` ← `subscription`  
- `end_date` ← `null`

**BandLab (crowdsourcing) → canónico**  
Basado en conceptos documentados (`Forkable`, `Revision History`): citeturn32search16turn34view3  
- `crowdsourcing.is_forkable` ← configuración “Forkable”  
- `crowdsourcing.revision_history_url` ← “Revision History” (si expone URL) (**no especificado**)  
- `status` ← `collab`

### Selectores sugeridos (CSS/XPath o JSONPath)

> Importante: estos selectores son **sugeridos** y deben validarse en tu entorno con inspección real de DOM o respuestas API; si no hay evidencia de clases/DOM, se dan rutas semánticas.

**Verkami (XPath semántico sugerido)**
- Título: `//h1[1]`  
- Creator (texto del enlace tras “A project of”): `//*[contains(.,'A project of')]/following::a[1]`  
- Categoría: `//*[contains(.,'Category')]/following::a[1]`  
- Ubicación: `//*[contains(.,'Created in')]/following::text()[1]`  
- Pledges: `//*[contains(.,'Pledges')]/preceding::text()[1]` (ajustar)  
- Recaudado/objetivo: buscar patrón `€` y “From” cerca del bloque de métricas.  
Evidencia de que esos textos existen en la página: citeturn13view0  

**Indiegogo (JSONPath sugerido; API)**
- Lista activos: `$[*].projectHomeUrl`, `$[*].campaignGoal`, `$[*].fundsGathered`, etc. citeturn8view2turn36view2  

**Patreon (JSONPath sobre JSON:API)**
- Campaigns: `$.data[*].id`, `$.data[*].attributes.*`, `$.data[*].relationships.*`  
- Members: `$.data[*].attributes.campaign_lifetime_support_cents` etc (nombres observados en docs). citeturn36view0  

**BandLab / HITRECORD / Kompoz**
- CSS/XPath: **no especificado** (no se obtuvo DOM estable en este análisis).  
- Recomendación: tratar como “fuentes sólo con permiso / integración oficial”.

### Ejemplo de payload JSON para importación

```json
{
  "source_site": "Verkami",
  "project_url": "https://www.verkami.com/projects/41651-financiacion-del-primer-disco-de-la-banda-manena-parsimoniosa",
  "title": "Financing of the first album by the band MANENA, Parsimoniosa",
  "creator_name": "Manenaduel",
  "category": "Music",
  "goal_amount": 8420,
  "amount_raised": 8805,
  "currency": "EUR",
  "end_date": "2026-02-06",
  "status": "successful",
  "backers_count": 142,
  "location_text": "Palma",
  "tags": ["Music"],
  "rewards": [
    {
      "price": 11,
      "title": "DIGITAL ALBUM",
      "description": "We send you the album ...",
      "backers": 3,
      "delivery_estimate": "2026-03"
    }
  ]
}
```

Los valores y campos están tomados/derivados del ejemplo de proyecto y su bloque de recompensas. citeturn13view0  

## 6. Arquitectura de scraping y ETL

### Diagrama de flujo (scraping + normalización + carga)

```mermaid
flowchart TD
  A[Seed de fuentes: listados/categorías o endpoints API] --> B[Descubrimiento: extraer URLs de proyecto]
  B --> C{¿Fuente con API oficial?}
  C -- Sí --> D[Llamadas API con auth si aplica]
  C -- No --> E[Fetch HTML (requests) o navegador headless si JS]
  D --> F[Parser JSON + validación de esquema]
  E --> G[Parser DOM (CSS/XPath) + extracción semántica]
  F --> H[Normalización al esquema canónico]
  G --> H[Normalización al esquema canónico]
  H --> I[Enriquecimiento: currency, fechas, status, tags]
  I --> J[Deduplicación: project_url + IDs + hashes]
  J --> K[(Staging: raw + parsed)]
  K --> L[(Warehouse/DB del MVP)]
  L --> M[Export: CSV/JSON + API interna del MVP]
  L --> N[Monitoring: rate limits, errores, cambios DOM]
```

### Recomendaciones de implementación

- **Discovery-first**: empezar por listados “Music” (Verkami) o endpoints “getActiveCrowdfundingProjects” (Indiegogo). citeturn13view1turn36view2  
- **Modo incremental**: guardar `last_seen_at`, `last_changed_hash` y refrescar sólo proyectos cambiados (p.ej., activos). En Indiegogo el endpoint ya ordena por start date y además hay cache corto, lo cual sugiere que polling frecuente debe ser moderado. citeturn36view2turn8view2  
- **Separar “crowdfunding” vs “crowdsourcing”**: misma tabla de proyectos pero distinto `status`/sub-objeto `crowdsourcing`.

## 7. Herramientas recomendadas y estrategias de legalidad/ética

### Librerías/herramientas (por tipo de fuente)

- APIs (Indiegogo, Patreon): `httpx`/`requests` + cliente OAuth2 (y almacenamiento seguro de tokens). La documentación de Patreon enfatiza OAuth2 y `User-Agent`. citeturn36view0turn36view2  
- HTML “semi-estático” (Verkami): `requests` + `lxml` (XPath) o `BeautifulSoup4`, con heurísticas semánticas. citeturn13view0turn13view1  
- Sitios JS-heavy (Indiegogo HTML, Ulule, BandLab, Kompoz): navegador headless (Playwright/Puppeteer). Aun así, **en algunos casos no es recomendable** por TOS. Indiegogo como HTML se describe como render “mostly via JavaScript”. citeturn10search6turn12view0  

### Estrategias para respetar legalidad y ética

- **Preferir APIs oficiales** cuando existan (Indiegogo, Patreon): reduces carga al sitio y alineas el acceso con mecanismos previstos por la plataforma. citeturn36view2turn36view0  
- **Cumplir límites y señales**:
  - En Patreon, incluir `User-Agent` y respetar rate limiting (la doc lo menciona explícitamente). citeturn36view0  
  - En BandLab, evitar scraping con bots/scrapers: el TOS menciona prohibición expresa y patrones de tráfico “no humano”. citeturn34view3  
- **Robots.txt**: revisar y actuar conforme a las directivas cuando sea viable (si el archivo no es accesible o no indexable en esta investigación, tratarlo como **no especificado** y revisar manualmente en tu pipeline real).  
- **Rate limiting y backoff**: aplicar “token bucket” por dominio, `Retry-After` si existe, y exponencial backoff en 429/403.
- **No eludir CAPTCHAs ni controles**: si aparece reCAPTCHA u otros, asumir que la plataforma no quiere automatización en esa ruta y buscar alternativa (API/permiso/licencia). En Verkami aparece referencia a reCAPTCHA en el sitio. citeturn13view0  
- **Minimización de datos**: importar sólo lo necesario (metadatos del proyecto) y no datos personales de usuarios/backers.
- **Atribución y trazabilidad**: guardar `source_site` y `project_url`; si se muestran contenidos (descripción) en tu producto, considerar attribution visible y/o enlaces a la campaña original.
- **Revisión legal**: si el MVP es comercial, revisar TOS de cada fuente y/o contactar para permiso. Para Kickstarter, por ejemplo, hay indicios de prohibición de crawling/scraping en su documentación legal (fragmento indexado) y además hay señales técnicas de bloqueo. citeturn3search7turn5view0  

## 8. Anexo: CSV de ejemplo

A continuación un CSV **de ejemplo (10 filas)** con columnas solicitadas. Los valores son demostrativos; cuando no hay evidencia pública suficiente en este análisis, se usa “no especificado”.

```csv
sitio,proyecto_url,título,creador,objetivo,recaudado,fecha_fin,estado,tags
Verkami,https://www.verkami.com/projects/41651-financiacion-del-primer-disco-de-la-banda-manena-parsimoniosa,Financing of the first album by the band MANENA, Parsimoniosa,Manenaduel,8420 EUR,8805 EUR,2026-02-06,successful,"Music"
Indiegogo,https://www.indiegogo.com/en/projects/jananishankar/black-album-campaign,Black Album Campaign,Janani Shankar,no especificado,no especificado,no especificado,no especificado,"music,album"
Indiegogo,https://www.indiegogo.com/en/projects/wesleyhogg/we-all-glow-we-all-fade-debut-album-by-harpoon-the-whale,We All Glow We All Fade: Debut Album,Harpoon, the Whale,no especificado,no especificado,no especificado,ended,"music,album"
Ulule,https://fr.ulule.com/estelle-bourgeois-album,Aidez Estelle Bourgeois à faire vivre son album,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,"musique,album"
Ulule,https://fr.ulule.com/festival-as-one,Festival caritatif AS ONE,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,"festival,musique"
Patreon,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,subscription,"membership,creator"
BandLab,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,collab,"forkable,revision-history"
Kompoz,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,collab,"collaboration,crowdsourcing"
HITRECORD,no especificado,no especificado,no especificado,no especificado,no especificado,no especificado,collab,"challenge,project,remix"
Kickstarter,https://www.kickstarter.com/projects/thetwotracks/support-the-two-tracks-5th-album,Support The Two Tracks 5th Album,no especificado,no especificado,no especificado,no especificado,no especificado,"music,album"
```

Para la fila de Verkami se usan valores observables en la página (objetivo/recaudado/fecha de “crowdfunded”). citeturn13view0  
Los endpoints y capacidad de extracción estructurada para Indiegogo y Patreon están documentados en sus APIs. citeturn36view2turn36view0