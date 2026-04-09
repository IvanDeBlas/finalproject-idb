# Plan de Data Seeding y Verificacion E2E con Playwright

**Fecha**: 2026-02-27
**Proyecto**: WePlay Rises
**Autor**: Ivan (con asistencia de Claude Code)
**Documento de referencia**: [`docs/deep-research-report.md`](./deep-research-report.md)

---

## Indice

1. [Introduccion y Objetivos](#1-introduccion-y-objetivos)
2. [Decisiones Tomadas](#2-decisiones-tomadas)
3. [Estado Actual de la Plataforma](#3-estado-actual-de-la-plataforma)
4. [Estrategia de Datos](#4-estrategia-de-datos)
5. [Scraping Verkami con Playwright](#5-scraping-verkami-con-playwright)
6. [Datos Ficticios - Artistas Reconocidos](#6-datos-ficticios---artistas-reconocidos)
7. [Datos Inspirados - Estilo Indiegogo](#7-datos-inspirados---estilo-indiegogo)
8. [Fans Simulados y Plan de Backings](#8-fans-simulados-y-plan-de-backings)
9. [Datos Ficticios - Crowdsourcing](#9-datos-ficticios---crowdsourcing)
10. [Infraestructura Playwright para Landing](#10-infraestructura-playwright-para-landing)
11. [Plan de Ejecucion](#11-plan-de-ejecucion)
12. [Estructura de Archivos](#12-estructura-de-archivos)
13. [Orden de Implementacion](#13-orden-de-implementacion)
14. [Criterios de Exito](#14-criterios-de-exito)
15. [Riesgos y Mitigaciones](#15-riesgos-y-mitigaciones)

---

## 1. Introduccion y Objetivos

### 1.1 Contexto

WePlay Rises es una plataforma de crowdfunding musical en fase MVP para el curso LIDR (AI4Devs). Necesitamos:

1. **Datos realistas** que hagan la plataforma atractiva y creible en demos
2. **Verificacion E2E** de que todos los flujos funcionan correctamente de punta a punta
3. **Cobertura de testing** para la aplicacion Landing (actualmente con 0 tests E2E)

La solucion es usar **Playwright como herramienta dual**: scraper de datos reales + runner de tests E2E que pueblan la plataforma a traves de la UI, verificando cada paso.

### 1.2 Objetivos Concretos

- Poblar la plataforma con **20-25 campanias musicales** (mezcla de datos reales y ficticios)
- Verificar todos los flujos criticos: registro, creacion de campanias, rewards, publicacion, backings
- Crear infraestructura Playwright para la **Landing web** (puerto 3000)
- Generar datos de demo atractivos con **10 artistas reconocidos internacionalmente**
- Producir **~75 rewards** y **~30 backings** distribuidos

### 1.3 Alcance

**Dentro del alcance:**
- Configurar Playwright en `src/web/`
- Scraper de Verkami (10-15 campanias reales)
- Datos ficticios de artistas famosos (10 campanias)
- Datos inventados estilo Indiegogo (5 campanias)
- Suite de specs de seeding secuencial
- Verificacion automatizada de toda la plataforma

**Fuera del alcance:**
- Scraping de Indiegogo, Patreon u otras plataformas
- Implementacion de nuevas features en el backend o frontend
- Despliegue a Azure (solo Docker local)
- Tests de rendimiento o accesibilidad

---

## 2. Decisiones Tomadas

| Decision | Opcion elegida | Justificacion |
|----------|---------------|---------------|
| Estrategia de scraping | **Hibrida** | Scraping real de Verkami + datos inventados para Indiegogo. Verkami tiene HTML accesible y 4.089 proyectos musicales (ver `deep-research-report.md`, Seccion 4 > Verkami). Indiegogo requeriria API con posibles cambios, mejor inventar datos controlados. |
| Artistas ficticios | **10 artistas** | MUSE, QOTSA, Arcade Fire, Bad Bunny, Omar Montes, Rosalia, C. Tangana, Arctic Monkeys, Billie Eilish, Vetusta Morla. Mezcla de generos internacionales y espanoles para demo atractiva. |
| Entorno de ejecucion | **Docker local** | `docker compose up` + Playwright contra localhost. Mas seguro para pruebas, sin riesgo de corromper datos en Azure. |
| Estrategia UI vs API | **Dual** | UI para flujos criticos (registro, backing), API como fallback para volumen. |

---

## 3. Estado Actual de la Plataforma

### 3.1 Servicios Docker

| Servicio | URL | Notas |
|----------|-----|-------|
| Backend API | http://localhost:5001 | Swagger en /swagger |
| Landing | http://localhost:3000 | Vite + React |
| Admin | http://localhost:3001 | Next.js |
| SQL Server | localhost:1433 | sa / WePlayRises2024! |

### 3.2 Endpoints API Disponibles

#### Autenticacion (`/api/auth`)

| Metodo | Endpoint | Auth | Descripcion | Body |
|--------|----------|------|-------------|------|
| POST | `/api/auth/register` | No | Registro | `{email, password, confirmPassword, role?}` |
| POST | `/api/auth/login` | No | Login -> JWT | `{email, password}` |
| GET | `/api/auth/me` | Si | Info usuario actual | - |

#### Artistas (`/api/artistas`)

| Metodo | Endpoint | Auth | Descripcion | Body |
|--------|----------|------|-------------|------|
| POST | `/api/artistas` | Si | Crear perfil artista | `{nombreArtistico, descripcion?, generoMusical?, imagenUrl?}` |
| GET | `/api/artistas/me` | Si | Mi perfil | - |
| GET | `/api/artistas/{id}` | No | Perfil publico | - |
| PUT | `/api/artistas/{id}` | Si | Actualizar (owner) | `{id, nombreArtistico, descripcion?, imagenUrl?}` |

#### Campanias (`/api/campanias`)

| Metodo | Endpoint | Auth | Descripcion |
|--------|----------|------|-------------|
| POST | `/api/campanias` | Si | Crear campania (borrador) |
| GET | `/api/campanias` | No | Listar publicas (publicadas) |
| GET | `/api/campanias/mis-campanias` | Si | Mis campanias |
| GET | `/api/campanias/{id}` | No | Detalle con rewards y backings |
| PUT | `/api/campanias/{id}` | Si | Actualizar (solo borrador) |
| POST | `/api/campanias/{id}/publicar` | Si | Publicar campania |
| DELETE | `/api/campanias/{id}` | Si | Eliminar campania |

**Campos de creacion de campania:**

```
titulo              string, required
subtitulo           string, optional
descripcionCorta    string, optional
importeObjetivo     decimal, required, min 100
tipoFinanciacionId  int, required (1=Todo-o-Nada, 2=Flexible)
monedaId            int, default=1 (EUR)
fechaFin            DateTime, optional (7-60 dias desde hoy)
imagenPrincipalUrl  string, optional
videoPrincipalUrl   string, optional
```

#### Rewards (`/api/rewards`)

| Metodo | Endpoint | Auth | Descripcion |
|--------|----------|------|-------------|
| POST | `/api/rewards` | Si | Crear reward |
| GET | `/api/rewards?campaniaId={id}` | No | Rewards de campania |
| PUT | `/api/rewards/{id}` | Si | Actualizar reward |
| DELETE | `/api/rewards/{id}` | Si | Eliminar reward |
| PUT | `/api/rewards/reorder` | Si | Reordenar rewards |

**Campos de creacion de reward:**

```
campaniaId              Guid, required
nombre                  string, required
descripcion             string, optional
importeMinimo           decimal, required
tipoRewardId            int, required
monedaId                int, required (default=1)
esAddOn                 bool
cantidadMaxima          int, optional
cantidadPorBacker       int, optional
incluyeEnvioFisico      bool
tiempoEntregaEstimado   string, optional
```

#### Backings (`/api/campanias/{id}/backings`)

| Metodo | Endpoint | Auth | Descripcion |
|--------|----------|------|-------------|
| POST | `/api/campanias/{id}/backings` | Opcional | Crear backing (permite anonimo) |
| GET | `/api/campanias/{id}/backings` | No | Backings recientes |
| GET | `/api/campanias/{id}/stats` | No | Estadisticas campania |

**Campos de creacion de backing:**

```
monto       decimal, required
rewardId    Guid, optional
mensaje     string, optional
esAnonimo   bool
```

#### Dashboard (`/api/dashboard`)

| Metodo | Endpoint | Auth | Descripcion |
|--------|----------|------|-------------|
| GET | `/api/dashboard/resumen` | Si | Resumen del artista |
| GET | `/api/dashboard/mis-campanias` | Si | Campanias con metricas |
| GET | `/api/dashboard/campanias/{id}/backings` | Si | Backings paginado |
| GET | `/api/dashboard/campanias/{id}/stats` | Si | Stats detallados |

### 3.3 Flujos UI - Admin Dashboard (puerto 3001)

| Flujo | Ruta | Estado | Notas |
|-------|------|--------|-------|
| Login | `/login` | Funcional | Email + password |
| Register | `/register` | Funcional | Email + password + confirm |
| Dashboard | `/dashboard` | Funcional | Stats, campanias, backings recientes |
| Crear campania | `/campanias/nueva` | Funcional | Wizard 5 pasos |
| Detalle campania | `/campanias/[id]` | Funcional | Preview + publicar + eliminar |
| Editar campania | `/campanias/[id]/editar` | Funcional | Solo borradores |
| Gestionar rewards | `/campanias/[id]/recompensas` | Funcional | CRUD + drag-drop reorder |
| Ver backings | `/campanias/[id]/backings` | Funcional | Tabla + filtros + export CSV |
| Perfil artista | `/perfil` | Funcional | Crear/editar perfil |

**Wizard de creacion de campania (5 pasos):**

1. **Template** - Seleccion de plantilla (opcional, se puede omitir)
2. **Info Basica** - Titulo, objetivo, tipo financiacion, fecha fin, imagen, video
3. **Historia** - Subtitulo, descripcion corta (con contadores de caracteres)
4. **Rewards** - Placeholder MVP (rewards se agregan post-creacion)
5. **Revision** - Preview completo + boton "Crear campania" (como borrador)

### 3.4 Flujos UI - Landing Web (puerto 3000)

| Flujo | Ruta | Estado | Notas |
|-------|------|--------|-------|
| Home | `/` | Funcional | Hero + stats + campanias destacadas |
| Register | `/auth/register` | Funcional | Nombre, email, password, confirm |
| Login | `/auth/login` | Funcional | Email + password |
| Explorar campanias | `/campanias` | Funcional | Busqueda + filtros por estado |
| Detalle campania | `/campanias/:id` | Funcional | Stats, rewards, boton apoyar |
| Hacer backing | Modal en detalle | Funcional | Seleccion reward + monto + mensaje |
| Confirmacion backing | `/campanias/:id/confirmacion` | Funcional | Resumen + proximos pasos |
| Perfil artista publico | `/artistas/:id` | Funcional | Info + campanias del artista |
| Dashboard artista | `/dashboard` | Funcional | Stats + mis campanias |

### 3.5 Infraestructura E2E Existente

**Admin (`src/admin/`):**
- Playwright configurado: `playwright.config.ts`
- **16 spec files** con ~100+ tests
- Helpers robustos en `e2e/integration/helpers.ts`:
  - `loginViaApi(email, password)` -> `{token, userId}`
  - `loginToAdminUI(page, email, password)` -> auth via localStorage
  - `fillWizardStep1(page, data)` -> titulo, objetivo, tipo financiacion
  - `fillWizardStep2(page, data)` -> subtitulo, descripcion
  - `createCampaignViaWizard(page, data)` -> campaignId
  - `publishCampaign(page)` -> confirmar publicacion
- Integration test completo: `wpr015-full-flow.spec.ts` (18 tests seriales)
- Patron de auth: pre-seed localStorage con JWT + `addInitScript`

**Landing (`src/web/`):**
- **NO existe infraestructura Playwright**
- No hay `playwright.config.ts`
- No hay directorio `e2e/`
- No hay helpers ni tests
- **Esto es un entregable clave de este plan**

---

## 4. Estrategia de Datos

### 4.0 Catalogo de Fuentes Verificadas

Se dispone de un CSV curado con 31 URLs reales: **`docs/music_crowd_seed.csv`**

Resultados de la validacion por scraping (2026-02-27):

| Plataforma | URLs | Accesibles | Calidad | Uso |
|------------|------|-----------|---------|-----|
| Verkami | 10 | 10/10 | Excelente | Scraping directo. 9 proyectos + 1 listado categoria |
| ccMixter | 10 | 10/10 | Excelente | Inspiracion crowdsourcing. Tracks con BPM, tags, licencias, stems |
| HitRECord | 1 | 1/1 | Excelente | Inspiracion crowdsourcing. Proyecto colaborativo con creditos por rol |
| Indiegogo | 5 | 5/5 | Buena | Datos reales parciales. Goal no siempre visible, descripciones completas |
| Reddit | 4 | 4/4 | Media | Solo inspiracion. Texto libre de busqueda de colaboradores (LFG) |
| Nadishana | 1 | 0/1 | N/A | Descartada (web offline) |
| **Total** | **31** | **30/31** | | **25 con calidad de scraping directo** |

### 4.1 Categorias de Datos

```
Datos de la plataforma
======================
|
+-- A) Scraping real (Verkami)
|   |-- 10-15 campanias musicales indie reales
|   |-- Rewards reales con precios y descripciones
|   +-- Datos de creadores reales (adaptados como artistas ficticios)
|
+-- B) Datos inventados (estilo Indiegogo)
|   |-- 5 campanias inspiradas en estructura del API publico de Indiegogo
|   +-- Artistas indie ficticios con nombres inventados
|
+-- C) Artistas reconocidos (ficticios)
|   |-- 10 campanias de artistas famosos
|   |-- 4 rewards cada uno (40 rewards total)
|   +-- Descripciones creativas y realistas
|
+-- D) Fans simulados
|   |-- 5 perfiles de fan
|   +-- 3 backings cada uno (15 backings de artistas famosos)
|       + backings adicionales para indie (~15 mas)
|
+-- E) Crowdsourcing (ficticios)
    |-- 8 perfiles profesionales
    |-- 10 necesidades publicadas por artistas (famosos + indie)
    |-- 18 propuestas de profesionales
    |-- 6 acuerdos (3 completados, 2 activos, 1 cancelado)
    |-- 8 milestones con 12 entregables
    |-- 10 conversaciones con mensajes
    +-- 6 valoraciones bidireccionales
```

### 4.2 Resumen Numerico

| Concepto | Cantidad |
|----------|----------|
| Artistas famosos (ficticios) | 10 |
| Artistas indie (inventados, estilo Indiegogo) | 5 |
| Artistas indie (scraping Verkami) | 10-15 |
| **Total campanias estimado** | **25-30** |
| Rewards por campania (famosos) | 4 cada uno = 40 |
| Rewards por campania (indie) | 2-3 cada uno = ~35 |
| **Total rewards estimado** | **~75** |
| Fans simulados | 5 |
| Backings (artistas famosos) | 15 |
| Backings (campanias indie) | ~15 |
| **Total backings estimado** | **~30** |
| **Inversion simulada total** | **~3.500 EUR** |
| --- | --- |
| Profesionales crowdsourcing | 8 |
| Necesidades publicadas | 10 |
| Propuestas enviadas | 18 |
| Acuerdos creados | 6 |
| Milestones + entregables | 8 + 12 |
| Conversaciones con mensajes | 10 |
| Valoraciones | 6 |

---

## 5. Scraping Verkami con Playwright

> **Referencia**: Ver `deep-research-report.md`:
> - Seccion 4 > Verkami (campos disponibles, formato URL, paginacion, senales anti-scraping)
> - Seccion 5 > Selectores sugeridos > Verkami (XPath semantico)
> - Seccion 5 > Esquema de importacion propuesto (modelo canonico)
> - Seccion 7 > Estrategias para respetar legalidad y etica

### 5.1 Fuente

- **URL base**: `https://www.verkami.com/discover/projects/category/10-musica`
- **Volumen disponible**: 4.089 proyectos musicales (segun el informe de referencia)
- **Objetivo**: Extraer 10-15 proyectos de la primera pagina del listado

### 5.2 Estrategia de Navegacion

```
1. Navegar a listado de musica en Verkami
2. Esperar carga completa del listado
3. Extraer URLs de los primeros 10-15 proyectos visibles
4. Para cada URL de proyecto:
   a. Navegar a la pagina del proyecto
   b. Esperar carga completa
   c. Extraer campos (titulo, creador, objetivo, recaudado, rewards)
   d. Delay de 2-3 segundos (respetar el servidor)
   e. Guardar en array de resultados
5. Serializar a data/scraped/verkami-campaigns.json
```

### 5.3 Campos a Extraer

Basado en los selectores sugeridos en `deep-research-report.md`, Seccion 5 > Selectores sugeridos > Verkami:

| Campo | Selector sugerido (XPath semantico) | Mapeo a WePlay Rises |
|-------|-------------------------------------|---------------------|
| Titulo | `//h1[1]` | `titulo` de la campania |
| Creador | `//*[contains(.,'A project of')]/following::a[1]` | `nombreArtistico` |
| Categoria | `//*[contains(.,'Category')]/following::a[1]` | metadato |
| Ubicacion | `//*[contains(.,'Created in')]/following::text()[1]` | metadato |
| Objetivo | Patron `From X EUR` cerca del bloque de metricas | `importeObjetivo` |
| Recaudado | Numero principal con EUR | referencia para backings |
| Backers | `//*[contains(.,'Pledges')]/preceding::text()[1]` | referencia para N backings |
| Estado | Texto "Project crowdfunded on..." | derivar successful/active |
| Rewards | Bloques de recompensas (precio, titulo, descripcion, backers) | `rewards[]` |

**Nota**: Estos selectores son **semanticos** (basados en texto visible) y mas estables que selectores CSS/clases que pueden cambiar. Deben validarse en el momento de implementacion.

### 5.4 Manejo de "Load More"

El listado de Verkami usa un boton "Load more projects" para paginacion infinita. Para el MVP:
- **Primera iteracion**: Tomar solo proyectos de la primera pagina (sin click en "Load more")
- **Si necesitamos mas**: Click en "Load more" + esperar carga dinamica

```typescript
// Solo si necesitamos mas de los visibles inicialmente
const loadMore = page.getByText('Load more projects');
if (await loadMore.isVisible()) {
    await loadMore.click();
    await page.waitForTimeout(2000);
}
```

### 5.5 Formato de Output

**Archivo**: `data/scraped/verkami-campaigns.json`

```json
[
    {
        "source": "verkami",
        "sourceUrl": "https://www.verkami.com/projects/41651-...",
        "title": "Financiacion del primer disco de la banda MANENA",
        "creator": "Manenaduel",
        "category": "Music",
        "location": "Palma",
        "goal": 8420,
        "raised": 8805,
        "currency": "EUR",
        "backers": 142,
        "status": "successful",
        "rewards": [
            {
                "name": "DIGITAL ALBUM",
                "price": 11,
                "description": "We send you the album...",
                "backers": 3,
                "deliveryEstimate": "2026-03"
            }
        ],
        "scrapedAt": "2026-02-27T10:00:00Z"
    }
]
```

> Este formato sigue el esquema canonico propuesto en `deep-research-report.md`, Seccion 5 > Esquema de importacion propuesto.

### 5.6 URLs Verificadas y Datos Extraidos (scraping test 2026-02-27)

Las siguientes 9 URLs del CSV `docs/music_crowd_seed.csv` fueron scrapeadas exitosamente. Todas retornan datos completos:

| # | Proyecto | Creador | Genero | Objetivo | Recaudado | Backers | Estado |
|---|----------|---------|--------|----------|-----------|---------|--------|
| 1 | French Horn Jazz Project | Pau Molto Biosca | Jazz | 4.000 | 5.055 | 94 | Funded 2020-03 |
| 2 | Sedajazz Big Band & Valmuz "Sinergia" | Screaming Pillows | Jazz | 4.000 | 4.075 | 131 | Funded 2020-12 |
| 3 | Herber&CO by Dixie Project | Dixie Project | Jazz | 2.000 | 2.176 | 82 | Funded 2017-07 |
| 4 | TRIM (Peixoto/Farinas/Barroso) CD+DVD | Fernando Barroso | Music | 3.400 | 3.793 | 182 | Funded 2014-05 |
| 5 | LP vinyl Sound System Selection | Good Over Evil | Reggae/Ska | 1.900 | 1.981 | 71 | Funded 2021-01 |
| 6 | Recording LOON ATTIC "HABITAT" | Juan Sanmar | Pop | 2.000 | 2.040 | 85 | Funded 2014-10 |
| 7 | Impronunciable - Ares Gratal | Ares Gratal Martinez | Songwriter | 7.500 | 8.622 | 84 | Funded 2021-08 |
| 8 | Black olives - Julia Pigali | julia vilardell | Music | 5.000 | 5.380 | 98 | Funded 2020-09 |
| 9 | Obal - Baile en Masso | Obal | Folk | 2.500 | 2.569 | 113 | Funded 2019-01 |

**Campos extraidos por proyecto**: titulo, creador, categoria, ubicacion (Valencia, Villena, Vigo, Malaga, Berlin, Barcelona, Bueu), objetivo EUR, recaudado EUR, backers, estado, descripcion completa, % financiado (102%-126%).

**URL adicional**: El listado `verkami.com/discover/projects/category/12-clasica` muestra 253 proyectos de clasica y permite descubrir mas URLs.

**Nota**: Los 9 proyectos son campanas **ya financiadas** (exitosas). Todos seran importados como campanas publicadas con backings simulados proporcionales al ratio recaudado/objetivo.

### 5.7 Transformacion para WePlay Rises

Los datos scrapeados se adaptan asi:

```
Verkami                         -->   WePlay Rises
---------------------------------------------------------------
creator name                    -->   Nuevo usuario + perfil artista
project title                   -->   titulo de campania
goal amount                     -->   importeObjetivo
reward tiers                    -->   rewards (nombre, importeMinimo, descripcion)
"successful" / "active"         -->   Se publica la campania
backers count                   -->   Referencia para simular N backings
raised amount                   -->   NO se usa directamente (viene de backings)
```

**Importante**: El `raised` (monto recaudado) no se puede inyectar directamente. Se genera organicamente a partir de los backings que creemos. Usamos el ratio `raised/goal` como referencia para decidir cuantos backings simular.

### 5.8 Consideraciones Eticas y Legales

> **Referencia**: Ver `deep-research-report.md`, Seccion 7 > Estrategias para respetar legalidad y etica

- Verkami **no tiene API publica documentada** (segun el informe)
- No se identifico clausula anti-scraping especifica en sus TOS
- Hay reCAPTCHA en flujos de **autenticacion**, no en lectura de listados publicos
- **Mitigaciones aplicadas**:
  - Delay de 2-3 segundos entre requests
  - Solo lectura de datos publicos (no auth required)
  - Uso puntual (una sola ejecucion, no crawling continuo)
  - Datos usados internamente para seed, no republicados

---

## 6. Datos Ficticios - Artistas Reconocidos

### 6.1 Resumen de los 10 Artistas

| # | Artista | Genero | Campania | Objetivo | Tipo Financiacion |
|---|---------|--------|----------|----------|-------------------|
| 1 | MUSE | Alternative Rock | Will of the People II - Fan-Funded Album | 75.000 EUR | Todo o Nada |
| 2 | Queens of the Stone Age | Stoner Rock | Songs for the Deaf - Acoustic Sessions | 40.000 EUR | Flexible |
| 3 | Arcade Fire | Indie Rock | WE Tour - Intimate Documentary | 30.000 EUR | Todo o Nada |
| 4 | Bad Bunny | Reggaeton / Latin Urban | DtMF - Vinyl Collector's Box | 50.000 EUR | Flexible |
| 5 | Omar Montes | Urban Spain / Flamenco Urbano | Pangea 2 - El Documental | 20.000 EUR | Flexible |
| 6 | Rosalia | Flamenco Pop / Experimental | MOTOMAMI Unplugged Sessions | 35.000 EUR | Todo o Nada |
| 7 | C. Tangana | Urban / Tradicion Espanola | El Madrileno - Live at Las Ventas | 45.000 EUR | Todo o Nada |
| 8 | Arctic Monkeys | Indie Rock | Tranquility Base - B-Sides Collection | 25.000 EUR | Flexible |
| 9 | Billie Eilish | Alternative Pop | HIT ME HARD AND SOFT - Remix Album | 55.000 EUR | Todo o Nada |
| 10 | Vetusta Morla | Indie Espana | Cable a Tierra - Version Orquesta Sinfonica | 15.000 EUR | Flexible |

**Credenciales comunes**: Password `WePlay2026!` para todos los artistas.

---

### 6.2 Artista 1: MUSE

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | muse.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | MUSE |
| Genero musical | Alternative Rock |
| Descripcion | Banda britanica de rock alternativo formada en Teignmouth en 1994. Liderados por Matt Bellamy, son conocidos por sus directos espectaculares, influencias clasicas y electronicas, y albumes conceptuales que van del rock espacial al dubstep sinfonico. Con mas de 30 millones de discos vendidos, buscan financiar su proximo album directamente con sus fans. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/e91e63?text=MUSE |

**Campania: "Will of the People II - Fan-Funded Album"**

| Campo | Valor |
|-------|-------|
| Titulo | Will of the People II - Fan-Funded Album |
| Subtitulo | El primer album de MUSE financiado 100% por fans |
| Descripcion corta | Despues de 30 anos de carrera, queremos hacer algo diferente: un album sin discografica, sin intermediarios, financiado directamente por quienes nos han acompanado desde el principio. Will of the People II sera nuestro disco mas personal y experimental. Cada euro va directo a produccion, mezcla y masterizacion en los Abbey Road Studios. |
| Objetivo | 75000 |
| Tipo financiacion | 1 (Todo o Nada) |
| Fecha fin | +45 dias desde ejecucion |
| Imagen | https://placehold.co/800x450/1a1a2e/e91e63?text=MUSE+-+WOTP+II |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Digital Download Exclusivo | 10 | Descarga digital del album en alta resolucion (FLAC 24bit) una semana antes del lanzamiento oficial. Incluye artwork digital exclusivo y notas de produccion de Matt Bellamy. | No | - | - |
| 2 | Vinyl Edicion Limitada | 35 | Vinilo doble en color rojo translucido, edicion numerada y limitada. Incluye descarga digital + poster A3 del artwork. Solo 500 unidades en el mundo. | Si | 3 meses | 500 |
| 3 | Meet and Greet Virtual | 100 | Sesion virtual grupal de 30 minutos con la banda via Zoom. Maximo 10 personas por sesion. Incluye todo lo anterior + foto digital personalizada firmada digitalmente. | No | - | 50 |
| 4 | Credito en el Album | 250 | Tu nombre aparecera en los creditos del album como Fan Producer. Incluye todo lo anterior + copia test pressing del vinilo + partitura firmada de un tema a elegir. | Si | 4 meses | 100 |

---

### 6.3 Artista 2: Queens of the Stone Age

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | qotsa.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Queens of the Stone Age |
| Genero musical | Stoner Rock |
| Descripcion | Proyecto de rock creado por Josh Homme en 1996 en el desierto de Palm Desert, California. Conocidos por su sonido pesado y groovy, riffs hipnoticos y una actitud que desafia las convenciones del rock mainstream. Desde Rated R hasta In Times New Roman, QOTSA ha definido un sonido unico que mezcla stoner rock con pop oscuro. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/ff5722?text=QOTSA |

**Campania: "Songs for the Deaf - Acoustic Sessions"**

| Campo | Valor |
|-------|-------|
| Titulo | Songs for the Deaf - Acoustic Sessions |
| Subtitulo | Las canciones que sacudieron el desierto, reimaginadas en acustico |
| Descripcion corta | Para celebrar los 25 anos de Songs for the Deaf, Josh Homme y compania regrabaran las canciones del album iconico en versiones acusticas e intimas, grabadas en el legendario Rancho De La Luna. Un proyecto por y para los fans del desierto. Sin discografica, sin compromisos. |
| Objetivo | 40000 |
| Tipo financiacion | 2 (Flexible) |
| Fecha fin | +40 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/ff5722?text=QOTSA+Acoustic |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Streaming Anticipado | 8 | Acceso anticipado al album acustico 48h antes del estreno. Link privado de streaming + PDF con letras manuscritas por Josh Homme. | No | - | - |
| 2 | Cassette Edicion Desierto | 25 | Cassette con arena real del desierto de Palm Desert incrustada en la carcasa. Edicion limitada + descarga digital. Una pieza de coleccionista unica. | Si | 3 meses | 300 |
| 3 | Jam Session Privada Virtual | 150 | Sesion privada de 20 minutos donde Josh toca tu cancion favorita del disco en acustico. Grabacion incluida como archivo WAV. Incluye todo lo anterior. | No | - | 20 |
| 4 | Desert Sessions Producer | 500 | Tu nombre como productor asociado + visita virtual al Rancho De La Luna + todos los rewards anteriores + vinilo master test pressing firmado por toda la banda. | Si | 4 meses | 10 |

---

### 6.4 Artista 3: Arcade Fire

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | arcadefire.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Arcade Fire |
| Genero musical | Indie Rock |
| Descripcion | Colectivo canadiense de indie rock liderado por Win Butler y Regine Chassagne, formado en Montreal en 2001. Ganadores del Grammy a Album del Ano por The Suburbs, son conocidos por sus directos catarticos y su capacidad de convertir un concierto en una experiencia comunitaria. Cada album es un mundo sonoro diferente. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/9c27b0?text=Arcade+Fire |

**Campania: "WE Tour - Intimate Documentary"**

| Campo | Valor |
|-------|-------|
| Titulo | WE Tour - Intimate Documentary |
| Subtitulo | El documental que muestra lo que pasa cuando se apagan las luces |
| Descripcion corta | Durante la gira de WE, un equipo filmo todo lo que normalmente no ves: los ensayos a las 3am, las discusiones creativas, las risas en el autobus de gira. Este documental es nuestra carta de amor a la vida en la carretera y a los fans que la hacen posible. |
| Objetivo | 30000 |
| Tipo financiacion | 1 (Todo o Nada) |
| Fecha fin | +35 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/9c27b0?text=Arcade+Fire+Doc |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Screener Digital Privado | 12 | Acceso exclusivo al documental en streaming antes del estreno publico. Subtitulos en espanol e ingles. Calidad 4K. | No | - | - |
| 2 | Poster Firmado + Screener | 30 | Poster A2 del documental firmado por Win Butler y Regine Chassagne + screener digital. Diseno exclusivo no disponible en tiendas. Impresion en papel texturizado. | Si | 2 meses | 200 |
| 3 | Nombre en Creditos | 75 | Tu nombre aparecera en los creditos finales del documental como Community Patron. Incluye poster firmado + screener digital. | Si | 2 meses | 150 |
| 4 | Productor Asociado | 200 | Credito como Associate Producer + invitacion al estreno online con Q&A en directo con Win y Regine + todos los rewards anteriores + copia en Blu-ray. | Si | 2 meses | 30 |

---

### 6.5 Artista 4: Bad Bunny

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | badbunny.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Bad Bunny |
| Genero musical | Reggaeton / Latin Urban |
| Descripcion | Benito Antonio Martinez Ocasio, conocido como Bad Bunny, es el artista latino mas escuchado del planeta. Desde Vega Baja, Puerto Rico, ha redefinido el reggaeton y la musica latina con albumes como YHLQMDLG, Un Verano Sin Ti y DeBi TiRAR MaS fOToS. Su impacto trasciende la musica: moda, lucha libre y activismo social. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/ffeb3b?text=Bad+Bunny |

**Campania: "DtMF - Vinyl Collector's Box"**

| Campo | Valor |
|-------|-------|
| Titulo | DtMF - Vinyl Collectors Box |
| Subtitulo | La caja de coleccion definitiva para los fans de verdad |
| Descripcion corta | DeBi TiRAR MaS fOToS merece mas que streaming. Esta edicion coleccionista incluye vinilos holograficos, fotos ineditas de Puerto Rico, y arte exclusivo de los visuales del album. Hecho por fans, para fans. Produccion limitada financiada directamente por la comunidad. |
| Objetivo | 50000 |
| Tipo financiacion | 2 (Flexible) |
| Fecha fin | +30 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/ffeb3b?text=Bad+Bunny+DtMF |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Digital Exclusivo | 10 | Version digital remasterizada + 3 tracks bonus ineditos + wallpapers exclusivos para movil en resolucion 4K. Disponible el dia del lanzamiento. | No | - | - |
| 2 | Box Set Holografico | 45 | Caja holografica con triple vinilo de color + booklet de 48 paginas con fotos ineditas de Puerto Rico + stickers + postal firmada digitalmente. | Si | 3 meses | 1000 |
| 3 | Backstage Pass Virtual | 120 | Experiencia virtual backstage: tour por el estudio de Benito en PR + video mensaje personalizado. Incluye Box Set completo + digital. | Si | 3 meses | 100 |
| 4 | Experiencia VIP Total | 350 | Meet and greet virtual exclusivo + todo lo anterior + camiseta tie-dye edicion ultra limitada + cadena personalizada con tu nombre. | Si | 4 meses | 25 |

---

### 6.6 Artista 5: Omar Montes

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | omarmontes.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Omar Montes |
| Genero musical | Urban Spain / Flamenco Urbano |
| Descripcion | Omar Montes, el rey de Pan Bendito, ha conquistado Espana con su mezcla unica de flamenco, reggaeton y musica urbana. Desde Alocao hasta Pangea, ha demostrado que se puede salir del barrio y llegar a lo mas alto sin perder la esencia. Cantante, showman y fenomeno cultural espanol. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/4caf50?text=Omar+Montes |

**Campania: "Pangea 2 - El Documental"**

| Campo | Valor |
|-------|-------|
| Titulo | Pangea 2 - El Documental |
| Subtitulo | De Pan Bendito al mundo: la historia que nadie ha contado |
| Descripcion corta | Un documental que sigue a Omar durante la grabacion de Pangea 2, desde las sesiones en Miami hasta las noches en Pan Bendito. Sin filtros, sin guion, la verdad de como se hace un disco numero uno en Espana. Financiado por los fans para que nadie nos diga que cortar. |
| Objetivo | 20000 |
| Tipo financiacion | 2 (Flexible) |
| Fecha fin | +30 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/4caf50?text=Omar+Montes+Pangea |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Acceso Anticipado | 8 | Acceso al documental 72h antes de que salga en plataformas. Link privado + making of de 15 minutos extra con escenas eliminadas. | No | - | - |
| 2 | Camiseta Pangea 2 | 25 | Camiseta exclusiva disenada por Omar con el logo de Pangea 2. Solo disponible aqui, nunca se vendera en tiendas. Incluye acceso anticipado. | Si | 2 meses | 500 |
| 3 | Mencion en Creditos | 60 | Tu nombre en los creditos del documental como Fan de Pan Bendito. Incluye camiseta + acceso anticipado + agradecimiento en redes de Omar. | Si | 2 meses | 200 |
| 4 | Cena con Omar | 500 | Cena para 2 personas con Omar en Madrid. Menu sorpresa elegido por el. Incluye todo lo anterior + foto firmada enmarcada + experiencia irrepetible. | No | - | 5 |

---

### 6.7 Artista 6: Rosalia

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | rosalia.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Rosalia |
| Genero musical | Flamenco Pop / Experimental |
| Descripcion | Rosalia Vila Tobella, desde Sant Esteve Sesrovires, ha revolucionado la musica global fusionando flamenco con electronica, reggaeton y avant-garde. El Mal Querer la catapulto internacionalmente y MOTOMAMI confirmo que no hay limites. Grammy, Latin Grammy, MTV: Rosalia no colecciona premios, redefine categorias. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/e91e63?text=Rosalia |

**Campania: "MOTOMAMI Unplugged Sessions"**

| Campo | Valor |
|-------|-------|
| Titulo | MOTOMAMI Unplugged Sessions |
| Subtitulo | Las canciones de MOTOMAMI como nunca las habias escuchado |
| Descripcion corta | Imagina SAOKO con solo una guitarra flamenca. O HENTAI al piano. Las MOTOMAMI Unplugged Sessions son la deconstruccion total del album: solo voz, instrumentos acusticos, y la verdad desnuda de cada cancion. Grabado en una iglesia del siglo XVIII en Catalunya. |
| Objetivo | 35000 |
| Tipo financiacion | 1 (Todo o Nada) |
| Fecha fin | +40 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/e91e63?text=Rosalia+Unplugged |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | LP Digital Unplugged | 10 | Descarga digital en alta calidad (FLAC) de todas las sesiones + video de un tema completo grabado en la iglesia. Experiencia inmersiva. | No | - | - |
| 2 | LP Transparente Fisico | 40 | Vinilo transparente con grabado laser de la iglesia en el disco. Edicion numerada + descarga digital. Pieza de arte sonoro. | Si | 3 meses | 400 |
| 3 | Foto Firmada + LP | 75 | Fotografia de 20x30cm firmada por Rosalia durante las sesiones. Impresion fine art en papel Hahnemuhle + LP transparente + digital. | Si | 3 meses | 150 |
| 4 | Clase de Cante Online | 200 | Masterclass grupal online (max 20 personas) donde Rosalia explica las tecnicas vocales de cada cancion. Incluye grabacion + todo lo anterior. | Si | 3 meses | 20 |

---

### 6.8 Artista 7: C. Tangana

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | ctangana.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | C. Tangana |
| Genero musical | Urban / Tradicion Espanola |
| Descripcion | Anton Alvarez Alfaro, C. Tangana, El Madrileno. Desde Chamberi ha construido una carrera que va del trap mas crudo a la rumba con Nino de Elche. El Madrileno no fue solo un disco: fue un manifiesto de que la tradicion y la modernidad pueden coexistir. Provocador, visionario y orgullosamente madrileno. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/ff9800?text=C.+Tangana |

**Campania: "El Madrileno - Live at Las Ventas"**

| Campo | Valor |
|-------|-------|
| Titulo | El Madrileno - Live at Las Ventas |
| Subtitulo | El concierto que Madrid merece, financiado por Madrid |
| Descripcion corta | Las Ventas. Un escenario circular. Flamenco, electronica y 20.000 personas. El Madrileno merece un directo a la altura y queremos que los fans decidan que esto ocurra. Sin promotores, sin marcas. Solo Anton, la musica y Madrid. El beneficio integro va a la produccion del espectaculo. |
| Objetivo | 45000 |
| Tipo financiacion | 1 (Todo o Nada) |
| Fecha fin | +45 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/ff9800?text=C.+Tangana+Las+Ventas |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Streaming del Concierto | 10 | Acceso al streaming en directo del concierto en 4K + repeticion durante 7 dias. Angulos de camara exclusivos. | No | - | - |
| 2 | Vinilo Numerado Live | 35 | Triple vinilo del concierto en directo, numerado y con sleeve disenado por Santos Bacana. Incluye streaming. | Si | 4 meses | 500 |
| 3 | Entrada VIP Las Ventas | 100 | Entrada preferente primera fila del ruedo + acceso a soundcheck + vinilo + streaming. La experiencia completa. | No | - | 200 |
| 4 | After Party + Meet and Greet | 300 | Acceso al after party privado post-concierto + foto con Anton + todos los rewards anteriores. Dress code: de domingo. | No | - | 30 |

---

### 6.9 Artista 8: Arctic Monkeys

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | arcticmonkeys.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Arctic Monkeys |
| Genero musical | Indie Rock |
| Descripcion | Cuatro chavales de Sheffield que grabaron demos, los compartieron en MySpace y cambiaron el indie britanico para siempre. Desde Whatever People Say I Am hasta The Car, Alex Turner ha demostrado que se puede evolucionar sin perder la esencia. Arctic Monkeys no siguen tendencias, las crean. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/607d8b?text=Arctic+Monkeys |

**Campania: "Tranquility Base - B-Sides Collection"**

| Campo | Valor |
|-------|-------|
| Titulo | Tranquility Base - B-Sides Collection |
| Subtitulo | Las canciones que se quedaron en la base lunar |
| Descripcion corta | Durante las sesiones de Tranquility Base Hotel and Casino se grabaron 23 canciones. Solo 11 llegaron al disco. Las otras 12 llevan anos en una cinta en algun lugar de Sheffield. Este proyecto las rescata, las mezcla y las publica por primera vez. Un album perdido que merece ser encontrado. |
| Objetivo | 25000 |
| Tipo financiacion | 2 (Flexible) |
| Fecha fin | +35 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/607d8b?text=Arctic+Monkeys+B-Sides |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Digital B-Sides | 8 | Descarga digital de las 12 canciones ineditas en formato FLAC + letras manuscritas por Alex Turner en PDF. | No | - | - |
| 2 | 7 inch Singles Pack | 30 | Set de 3 singles en vinilo 7 pulgadas con artwork exclusivo + descarga digital. Cada single tiene 4 canciones. Coleccion completa. | Si | 3 meses | 300 |
| 3 | Carta Manuscrita de Alex Turner | 150 | Carta personal de Alex Turner escrita a mano en papel del hotel Tranquility Base (letterhead custom disenado para el proyecto). Incluye todo lo anterior. | Si | 4 meses | 50 |
| 4 | Sesion de Estudio Privada | 400 | Asistir como oyente a una sesion de mezcla en los estudios de Sheffield. Maximo 5 personas. Incluye todo lo anterior + vinilo test pressing. | No | - | 5 |

---

### 6.10 Artista 9: Billie Eilish

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | billieeilish.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Billie Eilish |
| Genero musical | Alternative Pop |
| Descripcion | Billie Eilish Pirate Baird O'Connell cambio las reglas del pop desde su habitacion en Los Angeles. Con su hermano Finneas como productor, creo un sonido susurrado y oscuro que conquisto el mundo antes de cumplir 18 anos. Multiples Grammy, un Oscar, y una generacion entera que la considera la voz de su tiempo. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/00bcd4?text=Billie+Eilish |

**Campania: "HIT ME HARD AND SOFT - Remix Album"**

| Campo | Valor |
|-------|-------|
| Titulo | HIT ME HARD AND SOFT - Remix Album |
| Subtitulo | Tus productores favoritos reimaginan cada cancion |
| Descripcion corta | Que pasa cuando le das las stems de HIT ME HARD AND SOFT a 10 productores diferentes? Desde Kaytranada hasta Arca, pasando por James Blake. Un album de remixes que es un album completamente nuevo. Financiado por fans para que Billie y Finneas tengan libertad total creativa. |
| Objetivo | 55000 |
| Tipo financiacion | 1 (Todo o Nada) |
| Fecha fin | +45 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/00bcd4?text=Billie+Eilish+Remixes |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Remix Digital | 10 | Descarga del album de remixes completo en alta resolucion + video lyric exclusivo de un track. Disponible el dia del lanzamiento. | No | - | - |
| 2 | Stems Descargables | 30 | Pack de stems (voces, instrumentales, efectos) de 3 canciones para que hagas tus propios remixes + album digital completo. Licencia creative commons. | No | - | 500 |
| 3 | Hoodie Exclusiva + Album | 65 | Hoodie oversized disenada por Billie en negro con artwork UV-reactivo (brilla con luz negra) + album digital + stems pack. | Si | 3 meses | 300 |
| 4 | Credito Remix + Experiencia | 250 | Tu nombre en los creditos como Fan Executive Producer + listening party privada online con Billie y Finneas. Incluye todo lo anterior. | Si | 3 meses | 50 |

---

### 6.11 Artista 10: Vetusta Morla

**Registro y Perfil:**

| Campo | Valor |
|-------|-------|
| Email | vetustamorla.official@weplay-test.com |
| Password | WePlay2026! |
| Nombre artistico | Vetusta Morla |
| Genero musical | Indie Espana |
| Descripcion | Seis amigos de Tres Cantos que se convirtieron en la banda indie mas importante de Espana. Sin discografica major, sin reality shows, solo musica y directos memorables. Desde Un Dia en el Mundo hasta Cable a Tierra, Vetusta Morla ha demostrado que se puede llenar estadios siendo fiel a uno mismo. La banda del pueblo, literalmente. |
| Imagen URL | https://placehold.co/400x400/1a1a2e/8bc34a?text=Vetusta+Morla |

**Campania: "Cable a Tierra - Version Orquesta Sinfonica"**

| Campo | Valor |
|-------|-------|
| Titulo | Cable a Tierra - Version Orquesta Sinfonica |
| Subtitulo | Nuestras canciones arropadas por 60 musicos |
| Descripcion corta | Siempre sonamos con escuchar Copenhague con una orquesta sinfonica completa. Este proyecto hace realidad ese sueno: grabaremos Cable a Tierra integro con la Orquesta Sinfonica de RTVE en el Auditorio Nacional. Un disco que une indie y clasica, Tres Cantos y el mundo. |
| Objetivo | 15000 |
| Tipo financiacion | 2 (Flexible) |
| Fecha fin | +30 dias |
| Imagen | https://placehold.co/800x450/1a1a2e/8bc34a?text=Vetusta+Morla+Sinfonica |

**Rewards:**

| # | Nombre | Importe min. | Descripcion | Envio fisico | Entrega est. | Cantidad max. |
|---|--------|-------------|-------------|-------------|-------------|---------------|
| 1 | Disco Digital Orquestal | 8 | Descarga digital del album sinfonico en FLAC + video del making of de 10 minutos en el Auditorio Nacional. | No | - | - |
| 2 | Partitura Firmada | 25 | Partitura de Copenhague impresa en papel de alta calidad, firmada por los 6 miembros de la banda + disco digital. | Si | 2 meses | 200 |
| 3 | Ensayo Abierto | 80 | Asistir al ensayo general con la orquesta en el Auditorio Nacional de Madrid. Aforo limitado. Incluye partitura firmada + disco digital. | No | - | 100 |
| 4 | Productor Asociado + Todo | 200 | Credito como productor asociado en el disco sinfonico + LP doble del album + invitacion a la grabacion real + todos los rewards anteriores. | Si | 3 meses | 30 |

---

## 7. Datos Inspirados - Estilo Indiegogo

> **Referencia**: Ver `deep-research-report.md`, Seccion 4 > Indiegogo (estructura de campos y API publico)

Originalmente se planifico inventar datos. Tras la validacion del CSV (`docs/music_crowd_seed.csv`), se dispone de 5 proyectos reales de Indiegogo con datos parciales. Se combinan datos reales (titulo, creador, backers, montos) con datos inventados (artistas indie ficticios para WePlay Rises) para crear campanas hibridas.

**Datos reales scrapeados de Indiegogo** (referencia para inspirar las campanas):

| Proyecto real | Creador | Recaudado | Backers | Goal |
|--------------|---------|-----------|---------|------|
| Raised by Bats (Gothrock album) | Aurelio Voltaire | $53,793 | 1,424 | N/D |
| Willis McClung Memorial LP | mortonbairdiii | $6,926 | 129 | $2,000 (346%) |
| Candy Stone Debut EP | Candy Stone | $1,150 | 28 | N/D |
| KEMEROV - FMKD on vinyl | Kemerov band | $2,505 | 15 | N/D |
| High Holiday Album | Arielle Korman | $6,928 | 96 | $6,928 (100%) |

**Nota**: 3 de 5 proyectos estan en modo "pledge manager closed" lo que oculta el goal original. Los datos de creadores y descripciones si son completos. Se usan como referencia para calibrar objetivos y rewards realistas de las campanas indie ficticias.

Las 5 campanas indie ficticias se mantienen como datos controlados (ver tabla siguiente), pero ahora inspiradas en los rangos reales de Indiegogo: objetivos entre $1,150 y $53,793, backers entre 15 y 1,424.

### 7.1 Resumen de Campanias Indie

| # | Titulo | Artista ficticio | Email | Objetivo | Genero |
|---|--------|-----------------|-------|----------|--------|
| 1 | Debut Album - Songs from the Underground | The Velvet Echoes | velvetechoes@weplay-test.com | 5.000 | Indie Rock |
| 2 | Latin Fusion World Tour Documentary | Alma Latina Collective | almalatina@weplay-test.com | 12.000 | World / Latin |
| 3 | Acoustic Sessions in Abandoned Places | Silence and Sound | silenceandsound@weplay-test.com | 3.500 | Ambient / Acoustic |
| 4 | Hip Hop Meets Classical: The Symphony | DJ Prometheus | djprometheus@weplay-test.com | 8.000 | Hip Hop / Classical |
| 5 | Flamenco Electronico - El Nuevo Compas | La Nina del Cable | laninadcable@weplay-test.com | 6.000 | Flamenco / Electronic |

**Password comun**: `WePlay2026!`

### 7.2 Detalle de Campanias Indie

#### Indie 1: The Velvet Echoes

| Campo | Valor |
|-------|-------|
| Nombre artistico | The Velvet Echoes |
| Descripcion | Trio de indie rock de Barcelona formado en garajes del Raval. Guitarras lo-fi, letras en ingles y espanol, y una energia en directo que recuerda a los primeros Strokes. |
| Titulo campania | Debut Album - Songs from the Underground |
| Descripcion corta | Nuestro primer disco. 10 canciones grabadas en el sotano donde ensayamos. Necesitamos financiar la mezcla y masterizacion para que suene como merece. |
| Objetivo | 5000 |
| Tipo financiacion | 1 (Todo o Nada) |

**Rewards**: Digital Download (5 EUR), CD Firmado (20 EUR), Concierto Privado (80 EUR)

#### Indie 2: Alma Latina Collective

| Campo | Valor |
|-------|-------|
| Nombre artistico | Alma Latina Collective |
| Descripcion | Colectivo de 8 musicos de diferentes paises latinoamericanos residentes en Madrid. Fusionan cumbia, bossa nova, son cubano y jazz contemporaneo. |
| Titulo campania | Latin Fusion World Tour Documentary |
| Descripcion corta | Queremos documentar nuestra primera gira por Latinoamerica: de Buenos Aires a Ciudad de Mexico, grabando con musicos locales en cada parada. Un documental musical de 60 minutos. |
| Objetivo | 12000 |
| Tipo financiacion | 2 (Flexible) |

**Rewards**: Streaming Documental (8 EUR), Vinilo Compilacion (30 EUR), Tu Nombre en Creditos (75 EUR)

#### Indie 3: Silence and Sound

| Campo | Valor |
|-------|-------|
| Nombre artistico | Silence and Sound |
| Descripcion | Duo ambient-acustico de Valencia. Graban en iglesias abandonadas, fabricas en desuso y cuevas naturales. Cada espacio es un instrumento mas. |
| Titulo campania | Acoustic Sessions in Abandoned Places |
| Descripcion corta | Nuestro proximo album sera grabado integramente en 5 localizaciones abandonadas de la Comunitat Valenciana. Piano, cello y la acustica natural de cada espacio. |
| Objetivo | 3500 |
| Tipo financiacion | 2 (Flexible) |

**Rewards**: Album Digital (5 EUR), CD + Mapa Localizaciones (18 EUR), Asistir a Grabacion (60 EUR)

#### Indie 4: DJ Prometheus

| Campo | Valor |
|-------|-------|
| Nombre artistico | DJ Prometheus |
| Descripcion | Productor sevillano que fusiona hip hop con musica clasica. Samplea a Bach, Chopin y Debussy sobre beats trap y boom bap. Colabora con MCs y violinistas por igual. |
| Titulo campania | Hip Hop Meets Classical: The Symphony |
| Descripcion corta | Un album donde cada track es una colaboracion entre un MC y un musico clasico. 12 canciones, 12 MCs, 12 instrumentistas. Grabado en el Conservatorio de Sevilla. |
| Objetivo | 8000 |
| Tipo financiacion | 1 (Todo o Nada) |

**Rewards**: Digital Album (8 EUR), Vinilo Doble (35 EUR), Masterclass Produccion (100 EUR)

#### Indie 5: La Nina del Cable

| Campo | Valor |
|-------|-------|
| Nombre artistico | La Nina del Cable |
| Descripcion | Productora y cantaora madrilena que mezcla flamenco puro con sintetizadores modulares y ritmos electronicos. Un pie en la tradicion, otro en el futuro. |
| Titulo campania | Flamenco Electronico - El Nuevo Compas |
| Descripcion corta | Mi segundo disco explora que pasa cuando metes bulerias en un secuenciador y soleares en un sampler. Grabado entre Jerez y Berlin. Flamenco del siglo XXI. |
| Objetivo | 6000 |
| Tipo financiacion | 2 (Flexible) |

**Rewards**: EP Digital (5 EUR), Vinilo Color (25 EUR), Clase de Compas + Synth (70 EUR)

---

## 8. Fans Simulados y Plan de Backings

### 8.1 Perfiles de Fan

| # | Nombre | Email | Password |
|---|--------|-------|----------|
| 1 | Maria Garcia | maria.garcia@weplay-test.com | Test123! |
| 2 | Carlos Lopez | carlos.lopez@weplay-test.com | Test123! |
| 3 | Emma Wilson | emma.wilson@weplay-test.com | Test123! |
| 4 | Pablo Ruiz | pablo.ruiz@weplay-test.com | Test123! |
| 5 | Ana Martinez | ana.martinez@weplay-test.com | Test123! |

### 8.2 Plan de Backings - Artistas Famosos

#### Fan 1: Maria Garcia

| Campania | Reward seleccionado | Monto | Mensaje |
|----------|-------------------|-------|---------|
| MUSE - WOTP II | Vinyl Edicion Limitada | 35 | Desde el Wembley 2007 soy fan. Esto es un sueno hecho realidad. |
| Rosalia - Unplugged | LP Transparente Fisico | 40 | MOTOMAMI en acustico va a ser una locura absoluta |
| Vetusta Morla - Sinfonica | Partitura Firmada | 25 | Copenhague con orquesta... voy a llorar |

#### Fan 2: Carlos Lopez

| Campania | Reward seleccionado | Monto | Mensaje |
|----------|-------------------|-------|---------|
| Bad Bunny - DtMF | Box Set Holografico | 45 | El box set va directo a la vitrina, Benito crack |
| Omar Montes - Pangea 2 | Camiseta Pangea 2 | 25 | Omar crack, vamos Pan Bendito! |
| C. Tangana - Las Ventas | Vinilo Numerado Live | 35 | El Madrileno en Las Ventas va a ser historico |

#### Fan 3: Emma Wilson

| Campania | Reward seleccionado | Monto | Mensaje |
|----------|-------------------|-------|---------|
| Arctic Monkeys - B-Sides | 7 inch Singles Pack | 30 | Alex Turner B-sides are better than most bands A-sides |
| Billie Eilish - Remixes | Hoodie Exclusiva + Album | 65 | The hoodie with UV art is genius, cant wait |
| QOTSA - Acoustic | Cassette Edicion Desierto | 25 | Desert rock on cassette with real sand. Perfect. |

#### Fan 4: Pablo Ruiz

| Campania | Reward seleccionado | Monto | Mensaje |
|----------|-------------------|-------|---------|
| Arcade Fire - Documentary | Poster Firmado + Screener | 30 | Arcade Fire en directo cambio mi vida en 2014 |
| Vetusta Morla - Sinfonica | Ensayo Abierto | 80 | Ver a Vetusta con orquesta en el Nacional es un sueno |
| MUSE - WOTP II | Meet and Greet Virtual | 100 | Llevo 20 anos esperando conocer a Matt Bellamy |

#### Fan 5: Ana Martinez

| Campania | Reward seleccionado | Monto | Mensaje |
|----------|-------------------|-------|---------|
| Rosalia - Unplugged | Clase de Cante Online | 200 | Aprender cante de Rosalia no tiene precio |
| Bad Bunny - DtMF | Backstage Pass Virtual | 120 | Benito es el artista de nuestra generacion |
| Omar Montes - Pangea 2 | Mencion en Creditos | 60 | Fan desde Alocao, Omar es puro corazon |

### 8.3 Resumen de Backings por Campania (Artistas Famosos)

| Campania | Backings | Monto total | Fans |
|----------|----------|-------------|------|
| MUSE - WOTP II | 2 | 135 | Maria, Pablo |
| QOTSA - Acoustic | 1 | 25 | Emma |
| Arcade Fire - Documentary | 1 | 30 | Pablo |
| Bad Bunny - DtMF | 2 | 165 | Carlos, Ana |
| Omar Montes - Pangea 2 | 2 | 85 | Carlos, Ana |
| Rosalia - Unplugged | 2 | 240 | Maria, Ana |
| C. Tangana - Las Ventas | 1 | 35 | Carlos |
| Arctic Monkeys - B-Sides | 1 | 30 | Emma |
| Billie Eilish - Remixes | 1 | 65 | Emma |
| Vetusta Morla - Sinfonica | 2 | 105 | Maria, Pablo |
| **TOTAL** | **15** | **915 EUR** | **5 fans** |

### 8.4 Backings Adicionales para Campanias Indie

Para las campanias indie (scrapeadas + inventadas), se crearan **~15 backings adicionales** distribuidos entre los 5 fans, con montos de 5-30 EUR por backing, sumando ~200-300 EUR adicionales. Los detalles se generaran dinamicamente basandose en los rewards disponibles de cada campania.

### 8.5 Totales Finales Estimados

| Metrica | Valor |
|---------|-------|
| Total backings | ~30 |
| Inversion simulada total | ~1.200 EUR |
| Campanias con al menos 1 backing | 100% (famosos) + ~50% (indie) |
| Fans activos | 5 |

---

## 9. Datos Ficticios - Crowdsourcing

La plataforma tiene un modulo de crowdsourcing completo implementado (templates, necesidades, propuestas, acuerdos con milestones/entregables, mensajeria y valoraciones). Esta seccion define datos ficticios para poblar y verificar todo ese flujo.

> **Datos maestros de referencia**: Los roles profesionales (30), templates (6), estados y modalidades estan seedeados en la migracion `20260215145939_InitialCrowdsourcing.cs` del backend. Ver IDs exactos en la seccion 9.1.

### 9.0 Fuentes de Inspiracion Reales (scraping validado)

Los datos ficticios de crowdsourcing estan inspirados en patrones reales extraidos del CSV `docs/music_crowd_seed.csv`:

**ccMixter** (10 tracks scrapeados) - Remix contests y colaboraciones musicales:

| Track | Artista | BPM | Tags principales | Tipo |
|-------|---------|-----|-----------------|------|
| Fort Minor Remix - Fherking mix | fernando_filgueira | 85 | contest_entry, hip_hop, remix | Remix contest entry |
| Fort Minor Remix - Fherking mix 2 | fernando_filgueira | 170 | contest_entry, hip_hop | Remix contest entry |
| Cidade Sol swinging bossa Remix | tigabeatz | 102 | contest_entry, bossa_nova, jazz, latin | Remix contest entry |
| Kill Kill Kill - D&B Remix | MatrixAndDappa | 146 | contest_entry, winner, bass, drums, trip_hop | Remix contest (ganador) |
| Kalte Ohren Stem pack | starfrosch | 120 | loops, guitar, synthesizer, drums, house | Stems para remix |
| Just One Ping Remix pack | vimcortez | 110 | 80s, electrofunk, funk, g_funk, synth | Pack para remix |
| Noite de Carnaval | Nitropox | - | editorial_pick, chill, electro, trip_hop | Remix contest entry |
| Natchoongi Loops | Salman Ahmad | 125 | drums, guitar, loops, percussion | Loops para contest |

**Patrones aplicados a nuestros datos ficticios**:
- Tags de ccMixter -> generos y estilos en descripciones de necesidades
- BPM y formato -> nivel de detalle tecnico en entregables
- Licencias CC -> referencia para acuerdos sobre derechos
- Roles (remix, stems, loops) -> tipos de entregables en milestones

**HitRECord** (1 proyecto scrapeado) - Colaboracion musical con creditos por rol:

| Campo | Valor |
|-------|-------|
| Proyecto | Move On The Sun |
| Creador | Joseph Gordon-Levitt & HitRECord |
| Colaboradores | 78 artistas contribuyentes |
| Tracklist | 15 canciones completas |
| Creditos por rol | Music/Produced by, Lyrics, Mix, Lead Vocal, Synth, Piano, Electric Guitar, Bass, Acoustic Guitar, Backup Vocals, Claps |
| Distribucion | BandCamp, iTunes, Vinyl |
| Modelo economico | 50/50 entre HitRECord y artistas contribuyentes |

**Patrones aplicados**: La estructura de creditos por rol de HitRECord inspira directamente los roles profesionales y milestones de nuestros acuerdos de crowdsourcing (ej: un acuerdo tiene milestones para grabacion, mezcla, diseno, etc.).

**Reddit r/BedroomBands** (4 posts scrapeados) - Busqueda de colaboradores:

| Post | Busca | Genero |
|------|-------|--------|
| [LFG] Progressive/melodic metal guitarist | Colaboradores para escribir | Metal progresivo |
| [LFG] Singer Looking for band/musicians | Banda/musicos | Varios (8 anos cantando) |
| [LFG] Want to start making music more regularly | Compromiso mensual | Varios |
| [LFG] Looking for Metalcore style vocalist | Vocalista metalcore | Metalcore/djent |

**Patrones aplicados**: El formato [LFG] (Looking For Group) de Reddit inspira el tono y contenido de las descripciones de necesidades, especialmente las que buscan musicos de sesion (N3, N9) y colaboradores especificos.

### 9.1 Datos Maestros Disponibles (ya seedeados)

**Templates de proyecto** (usados para generar necesidades):

| ID | Nombre | Fases | Necesidades generadas |
|----|--------|-------|----------------------|
| `11111111-...-111111111111` | Grabar un Album / EP | Pre-produccion, Grabacion, Mezcla y Master, Diseno, Promocion | 11 |
| `22222222-...-222222222222` | Produccion de Videoclip | Pre-produccion, Grabacion, Post-produccion, Promocion, Distribucion | 11 |
| `33333333-...-333333333333` | Organizar Gira / Tour | Pre-produccion, Produccion, Promocion | 11 |
| `44444444-...-444444444444` | Campana de Marketing | Estrategia, Contenido, Distribucion, Legal, Seguimiento | 9 |
| `55555555-...-555555555555` | Lanzamiento de Single | Pre-produccion, Grabacion, Mezcla, Diseno, Promocion, Legal | 8 |
| `66666666-...-666666666666` | Presencia Online | Identidad, Contenido, Redes, Distribucion | 5 |

**Roles profesionales principales** (30 roles en 6 categorias):

| Cat. | Roles relevantes para los datos ficticios |
|------|-------------------------------------------|
| Produccion Musical | Productor musical (1), Arreglista (2), Musico de sesion (5) |
| Ingenieria de Audio | Ingeniero de grabacion (7), Ingeniero de mezcla (8), Ingeniero de mastering (9) |
| Diseno y Creatividad | Disenador grafico (12), Fotografo (13), Director de videoclip (14), Animador (16) |
| Marketing y Promocion | Community manager (19), Publicista musical (20), Creador de contenido (22) |
| Gestion y Legal | Abogado musical (25) |
| Produccion de Eventos | Tecnico de sonido live (10), Tecnico de iluminacion (29) |

**Estados**:
- Necesidad: Abierta (1), En Progreso (2), Cerrada (3)
- Propuesta: Pendiente (1), Aceptada (2), Rechazada (3), Retirada (4)
- Acuerdo: Activo (1), Completado (2), Cancelado (3)
- Entregable: Entregado (1), Aprobado (2), Rechazado (3)

---

### 9.2 Perfiles Profesionales

8 profesionales ficticios que enviaran propuestas a las necesidades de los artistas.

| # | Nombre | Email | Rol principal (ID) | Especializacion | Ubicacion |
|---|--------|-------|--------------------|-----------------|-----------|
| P1 | Lucia Fernandez | lucia.fernandez@weplay-test.com | Ingeniero de mezcla (8) | Mezcla y mastering de rock alternativo. 10 anos de experiencia en estudios de Londres y Madrid. | Madrid |
| P2 | Marco Rossi | marco.rossi@weplay-test.com | Productor musical (1) | Produccion de pop y urban latino. Ha trabajado con sellos independientes en Barcelona y Milan. | Barcelona |
| P3 | Sarah Chen | sarah.chen@weplay-test.com | Disenadora grafica (12) | Portadas de discos, merch y branding musical. Estilo neo-retro y collage digital. | Berlin |
| P4 | Andres Molina | andres.molina@weplay-test.com | Director de videoclip (14) | Videoclips narrativos y documentales musicales. Ganador de premios en festivales indie. | Sevilla |
| P5 | Yuki Tanaka | yuki.tanaka@weplay-test.com | Musico de sesion (5) | Multi-instrumentista (violin, cello, piano). Formacion clasica con 15 anos en musica de camara. | Londres |
| P6 | Carmen Diaz | carmen.diaz@weplay-test.com | Community manager (19) | Marketing digital musical. Especialista en lanzamientos en Spotify y redes sociales. | Valencia |
| P7 | David Okafor | david.okafor@weplay-test.com | Fotografo (13) | Fotografia de conciertos y retratos artisticos. Blanco y negro analogico y digital. | Amsterdam |
| P8 | Elena Petrova | elena.petrova@weplay-test.com | Animadora / Motion graphics (16) | Visuales para conciertos, lyric videos y motion graphics para redes. Estilo psicodelico. | Lisboa |

**Password comun**: `WePlay2026!`

**Nota**: Cada profesional se registra como usuario, y luego crea su perfil de artista (el perfil de artista sirve como perfil publico en la plataforma). Los profesionales NO necesitan ser artistas para enviar propuestas; el sistema usa el userId directamente.

---

### 9.3 Necesidades Publicadas por Artistas

10 necesidades vinculadas a los artistas famosos e indie. Cada necesidad la publica un artista para su campana.

| # | Artista solicitante | Titulo de necesidad | Rol requerido (ID) | Modalidad | Presupuesto min-max | Fecha limite propuestas | Estado final |
|---|--------------------|--------------------|--------------------|-----------|--------------------|------------------------|-------------|
| N1 | MUSE | Diseno de portada para Will of the People II | Disenador grafico (12) | Remoto | 1.500 - 3.000 EUR | +30 dias | Cerrada (3) |
| N2 | Bad Bunny | Direccion de videoclip promocional DtMF Box | Director de videoclip (14) | Presencial | 5.000 - 10.000 EUR | +25 dias | En Progreso (2) |
| N3 | Rosalia | Musico de sesion - Violin para MOTOMAMI Unplugged | Musico de sesion (5) | Presencial | 800 - 1.500 EUR | +20 dias | Cerrada (3) |
| N4 | Arcade Fire | Fotografo documental para WE Tour | Fotografo (13) | Presencial | 2.000 - 4.000 EUR | +35 dias | En Progreso (2) |
| N5 | C. Tangana | Ingeniero de grabacion - Live at Las Ventas | Ingeniero de grabacion (7) | Presencial | 3.000 - 6.000 EUR | +20 dias | Cerrada (3) |
| N6 | Arctic Monkeys | Masterizacion de B-Sides Collection | Ingeniero de mastering (9) | Remoto | 1.000 - 2.500 EUR | +30 dias | Abierta (1) |
| N7 | Billie Eilish | Animacion para visualizers del Remix Album | Animadora (16) | Remoto | 2.500 - 5.000 EUR | +40 dias | En Progreso (2) |
| N8 | Vetusta Morla | Community manager para campana sinfonica | Community manager (19) | Remoto | 600 - 1.200 EUR | +15 dias | Completado via acuerdo |
| N9 | DJ Prometheus (indie) | Violinista para Hip Hop Meets Classical | Musico de sesion (5) | Hibrido | 500 - 1.000 EUR | +20 dias | Cerrada (3) |
| N10 | La Nina del Cable (indie) | Productor para fusiones flamenco-electronico | Productor musical (1) | Presencial | 1.500 - 3.000 EUR | +25 dias | Abierta (1) |

**Descripcion detallada de cada necesidad:**

#### N1: MUSE - Diseno de portada
```
Titulo: Diseno de portada para Will of the People II
Descripcion: Necesitamos un diseno de portada que refleje la dualidad del album:
rebelion vs. esperanza. Estilo visual inspirado en nuestras portadas anteriores
(The 2nd Law, Drones) pero con un giro mas minimalista y futurista. Entregables:
portada principal (3000x3000px), contraportada, booklet interior (8 paginas),
y adaptaciones para streaming (banner Spotify, YouTube thumbnail).
Presupuesto: 1.500-3.000 EUR | Modalidad: Remoto | Moneda: EUR
```

#### N2: Bad Bunny - Videoclip DtMF
```
Titulo: Direccion de videoclip promocional DtMF Collector's Box
Descripcion: Videoclip corto (2-3 min) para promocionar el vinyl collector's box
de DtMF. Estetica retro-futurista con referencias al reggaeton clasico de PR.
Debe incluir tomas del unboxing del box set, elementos de animacion 3D y
secuencias cinematicas. Rodaje en Madrid o Barcelona.
Presupuesto: 5.000-10.000 EUR | Modalidad: Presencial | Moneda: EUR
```

#### N3: Rosalia - Violin Unplugged
```
Titulo: Musico de sesion - Violin para MOTOMAMI Unplugged Sessions
Descripcion: Busco violinista con formacion clasica y sensibilidad flamenca
para 5 sesiones de grabacion en estudio (Madrid). Repertorio: adaptaciones
acusticas de MOTOMAMI con arreglos de cuerda. Imprescindible experiencia
en grabacion de estudio y capacidad de improvisacion sobre compases flamencos.
Presupuesto: 800-1.500 EUR | Modalidad: Presencial | Moneda: EUR
```

#### N4: Arcade Fire - Fotografo documental
```
Titulo: Fotografo documental para WE Tour - Intimate Documentary
Descripcion: Fotografo para documentar los ultimos 3 conciertos de la gira
europea. Estilo documentalista: backstage, soundcheck, publico, momentos
espontaneos. Entregables: 200 fotos editadas + 50 seleccion premium.
Formato RAW + editadas. Viaje incluido en el presupuesto.
Presupuesto: 2.000-4.000 EUR | Modalidad: Presencial | Moneda: EUR
```

#### N5: C. Tangana - Ingeniero de grabacion
```
Titulo: Ingeniero de grabacion - Live at Las Ventas
Descripcion: Grabacion multitrack del concierto en Las Ventas (Madrid).
24+ canales, incluyendo orquesta de camara, banda principal y coro.
Necesitamos experiencia en grabacion de directos en recintos grandes.
Equipo proporcionado, buscamos al ingeniero.
Presupuesto: 3.000-6.000 EUR | Modalidad: Presencial | Moneda: EUR
```

#### N6: Arctic Monkeys - Mastering
```
Titulo: Masterizacion de Tranquility Base - B-Sides Collection
Descripcion: Master de 12 temas ineditos (B-sides y rarezas) para edicion
coleccionista. Estilo cohesivo con el sonido de Tranquility Base Hotel & Casino.
Formato: stereo master para vinyl (lacquer), CD y digital (streaming-optimized).
Presupuesto: 1.000-2.500 EUR | Modalidad: Remoto | Moneda: EUR
```

#### N7: Billie Eilish - Animacion
```
Titulo: Animacion para visualizers del HIT ME HARD AND SOFT Remix Album
Descripcion: 5 visualizers animados (30-60 seg c/u) para los remixes del album.
Estetica dark/dreamy con elementos organicos y glitch. Loops para streaming
en plataformas. Formato 4K vertical (para TikTok/Reels) + 16:9 (YouTube).
Presupuesto: 2.500-5.000 EUR | Modalidad: Remoto | Moneda: EUR
```

#### N8: Vetusta Morla - Community manager
```
Titulo: Community manager para campana sinfonica Cable a Tierra
Descripcion: Gestion de redes sociales durante 2 meses para la campana de
crowdfunding. Publicaciones diarias en Instagram, Twitter y TikTok.
Contenido: behind-the-scenes de ensayos con orquesta, countdown al concierto,
engagement con fans. Reporting semanal.
Presupuesto: 600-1.200 EUR | Modalidad: Remoto | Moneda: EUR
```

#### N9: DJ Prometheus - Violinista
```
Titulo: Violinista para Hip Hop Meets Classical
Descripcion: Busco violinista para grabar partes en 6 de los 12 tracks del album.
Fusion hip hop con clasica: samples de Bach y Chopin reinterpretados con violin
acustico sobre beats trap. Sesiones en el Conservatorio de Sevilla (3 dias).
Presupuesto: 500-1.000 EUR | Modalidad: Hibrido | Moneda: EUR
```

#### N10: La Nina del Cable - Productor
```
Titulo: Productor para fusiones flamenco-electronico
Descripcion: Necesito productor con experiencia en electronica y respeto por el
flamenco para co-producir 4 temas de mi segundo disco. El reto: meter bulerias
en un secuenciador sin perder el alma del compas. Sesiones en Madrid o Jerez.
Presupuesto: 1.500-3.000 EUR | Modalidad: Presencial | Moneda: EUR
```

---

### 9.4 Propuestas de Profesionales

18 propuestas distribuidas entre las 10 necesidades. Cada necesidad recibe 1-3 propuestas.

| # | Necesidad | Profesional | Precio propuesto | Duracion | Estado final | Mensaje extracto |
|---|-----------|-------------|-----------------|----------|-------------|-----------------|
| PR1 | N1 (MUSE portada) | P3 Sarah Chen | 2.200 EUR | 3 semanas | **Aceptada (2)** | He disenado portadas para Radiohead y Tame Impala. Mi estilo neo-retro encaja perfecto con la estetica MUSE. |
| PR2 | N1 (MUSE portada) | P7 David Okafor | 2.800 EUR | 4 semanas | Rechazada (3) | Propongo una sesion fotografica conceptual como base para la portada. |
| PR3 | N2 (Bad Bunny video) | P4 Andres Molina | 7.500 EUR | 5 semanas | **Aceptada (2)** | Tengo experiencia en videoclips de reggaeton y trap. Mi propuesta incluye storyboard, rodaje 2 dias y post-produccion completa. |
| PR4 | N2 (Bad Bunny video) | P8 Elena Petrova | 8.000 EUR | 6 semanas | Rechazada (3) | Propongo un enfoque 100% animacion 3D, sin rodaje fisico. |
| PR5 | N3 (Rosalia violin) | P5 Yuki Tanaka | 1.200 EUR | 5 sesiones | **Aceptada (2)** | He tocado con la London Symphony y tengo experiencia en fusion flamenco-clasica con Paco de Lucia Jr. |
| PR6 | N4 (Arcade Fire foto) | P7 David Okafor | 3.200 EUR | 3 conciertos | **Aceptada (2)** | Mi portfolio de conciertos incluye trabajos con Sigur Ros y Bon Iver. Estilo documentalista intimo, sin flash. |
| PR7 | N4 (Arcade Fire foto) | P3 Sarah Chen | 2.800 EUR | 3 conciertos | Rechazada (3) | Ademas de fotos puedo crear un mini-zine digital post-gira. |
| PR8 | N5 (C. Tangana live) | P1 Lucia Fernandez | 4.500 EUR | 1 semana | **Aceptada (2)** | He grabado directos en el Wizink Center y el Palau Sant Jordi. Tengo equipo propio multitrack 32 canales. |
| PR9 | N5 (C. Tangana live) | P2 Marco Rossi | 5.500 EUR | 1 semana | Rechazada (3) | Propongo grabacion + mezcla preliminar in-situ. |
| PR10 | N6 (Arctic Monkeys master) | P1 Lucia Fernandez | 1.800 EUR | 2 semanas | Pendiente (1) | Conozco el sonido Tranquility Base al dedillo. Master analogico + digital. |
| PR11 | N6 (Arctic Monkeys master) | P2 Marco Rossi | 2.200 EUR | 3 semanas | Pendiente (1) | Mi experiencia en mastering vintage seria ideal para B-sides de esta era. |
| PR12 | N7 (Billie Eilish anim.) | P8 Elena Petrova | 4.000 EUR | 4 semanas | **Aceptada (2)** | Mis visualizers para Bjork y FKA twigs tienen millones de reproducciones. Estilo dark y organico, perfecto para Billie. |
| PR13 | N7 (Billie Eilish anim.) | P3 Sarah Chen | 3.500 EUR | 5 semanas | Pendiente (1) | Puedo hacer motion graphics con collage digital, estilo alternativo. |
| PR14 | N8 (Vetusta Morla CM) | P6 Carmen Diaz | 900 EUR | 2 meses | **Aceptada (2)** | He gestionado campanas de crowdfunding musical con +200% de engagement. Conozco el ecosistema indie espanol. |
| PR15 | N8 (Vetusta Morla CM) | P7 David Okafor | 1.100 EUR | 2 meses | Rechazada (3) | Puedo combinar fotografia + gestion de redes. |
| PR16 | N9 (DJ Prometheus violin) | P5 Yuki Tanaka | 750 EUR | 3 dias | **Aceptada (2)** | Adoro la fusion clasica-hip hop. He tocado con productores como DJ Shadow y RJD2. |
| PR17 | N10 (La Nina del Cable) | P2 Marco Rossi | 2.500 EUR | 4 semanas | Pendiente (1) | Tengo experiencia en produccion electronica y he trabajado con artistas de flamenco fusion en Barcelona. |
| PR18 | N10 (La Nina del Cable) | P1 Lucia Fernandez | 2.000 EUR | 3 semanas | Pendiente (1) | Aunque mi fuerte es mezcla, tambien produzco. Puedo aportar el lado tecnico del sonido. |

**Resumen de propuestas por estado:**

| Estado | Cantidad |
|--------|----------|
| Aceptada | 8 |
| Rechazada | 6 |
| Pendiente | 4 |
| **Total** | **18** |

---

### 9.5 Acuerdos de Crowdsourcing

De las 8 propuestas aceptadas, se crean 6 acuerdos (los acuerdos de N6 y N10 no se crean porque las propuestas estan pendientes):

| # | Necesidad | Artista | Profesional | Monto acordado | Fecha inicio | Fecha fin prevista | Estado |
|---|-----------|---------|-------------|---------------|-------------|-------------------|--------|
| A1 | N1 MUSE portada | MUSE | P3 Sarah Chen | 2.200 EUR | Hoy - 45 dias | Hoy - 24 dias | **Completado (2)** |
| A2 | N3 Rosalia violin | Rosalia | P5 Yuki Tanaka | 1.200 EUR | Hoy - 30 dias | Hoy - 16 dias | **Completado (2)** |
| A3 | N8 Vetusta Morla CM | Vetusta Morla | P6 Carmen Diaz | 900 EUR | Hoy - 50 dias | Hoy + 10 dias | **Completado (2)** |
| A4 | N2 Bad Bunny video | Bad Bunny | P4 Andres Molina | 7.500 EUR | Hoy - 10 dias | Hoy + 25 dias | **Activo (1)** |
| A5 | N4 Arcade Fire foto | Arcade Fire | P7 David Okafor | 3.200 EUR | Hoy - 5 dias | Hoy + 30 dias | **Activo (1)** |
| A6 | N5 C. Tangana live | C. Tangana | P1 Lucia Fernandez | 4.500 EUR | Hoy - 40 dias | Hoy - 30 dias | **Cancelado (3)** |

**Nota A6**: El acuerdo con C. Tangana se cancelo porque el concierto en Las Ventas se pospuso por motivos de agenda. Motivo de cancelacion: "Concierto pospuesto a nueva fecha por determinar. Se retomara el acuerdo cuando se confirme la nueva fecha."

---

### 9.6 Milestones y Entregables

#### Acuerdo A1: MUSE portada (Completado)

| Milestone | Titulo | Monto | Deadline | Estado |
|-----------|--------|-------|----------|--------|
| M1 | Concepto y bocetos | 600 EUR | Hoy - 38 dias | Completado |
| M2 | Diseno final + adaptaciones | 1.600 EUR | Hoy - 24 dias | Completado |

| Entregable | Milestone | Titulo | Estado |
|------------|-----------|--------|--------|
| E1 | M1 | 3 propuestas de concepto visual (PDF) | Aprobado (2) |
| E2 | M1 | Moodboard de referencias y paleta de color | Aprobado (2) |
| E3 | M2 | Portada principal 3000x3000px (PSD + PNG) | Aprobado (2) |
| E4 | M2 | Booklet interior 8 paginas (PDF print-ready) | Aprobado (2) |
| E5 | M2 | Adaptaciones streaming (Spotify banner, YouTube thumbnail) | Aprobado (2) |

#### Acuerdo A2: Rosalia violin (Completado)

| Milestone | Titulo | Monto | Deadline | Estado |
|-----------|--------|-------|----------|--------|
| M3 | Sesiones de grabacion (5 tracks) | 1.200 EUR | Hoy - 16 dias | Completado |

| Entregable | Milestone | Titulo | Estado |
|------------|-----------|--------|--------|
| E6 | M3 | Tracks de violin grabados - Sesion 1 a 3 (WAV 96kHz) | Aprobado (2) |
| E7 | M3 | Tracks de violin grabados - Sesion 4 y 5 (WAV 96kHz) | Aprobado (2) |

#### Acuerdo A3: Vetusta Morla CM (Completado)

| Milestone | Titulo | Monto | Deadline | Estado |
|-----------|--------|-------|----------|--------|
| M4 | Mes 1 - Lanzamiento campana | 450 EUR | Hoy - 20 dias | Completado |
| M5 | Mes 2 - Mantenimiento y cierre | 450 EUR | Hoy + 10 dias | Completado |

| Entregable | Milestone | Titulo | Estado |
|------------|-----------|--------|--------|
| E8 | M4 | Report mes 1: 45 publicaciones, +180% engagement, 2.3K nuevos followers | Aprobado (2) |
| E9 | M5 | Report mes 2: 38 publicaciones, campana al 85% del objetivo, hashtag trending | Aprobado (2) |

#### Acuerdo A4: Bad Bunny video (Activo - en progreso)

| Milestone | Titulo | Monto | Deadline | Estado |
|-----------|--------|-------|----------|--------|
| M6 | Pre-produccion y storyboard | 2.000 EUR | Hoy + 5 dias | En progreso |
| M7 | Rodaje + post-produccion | 5.500 EUR | Hoy + 25 dias | Pendiente |

| Entregable | Milestone | Titulo | Estado |
|------------|-----------|--------|--------|
| E10 | M6 | Storyboard completo (15 escenas) | Entregado (1) - pendiente revision |
| E11 | M6 | Lista de localizaciones y plan de rodaje | Entregado (1) - pendiente revision |

#### Acuerdo A5: Arcade Fire foto (Activo - recien iniciado)

| Milestone | Titulo | Monto | Deadline | Estado |
|-----------|--------|-------|----------|--------|
| M8 | Cobertura 3 conciertos + edicion | 3.200 EUR | Hoy + 30 dias | En progreso |

| Entregable | Milestone | Titulo | Estado |
|------------|-----------|--------|--------|
| E12 | M8 | Preview concierto 1 - 30 fotos seleccion (JPEG) | Entregado (1) - pendiente revision |

---

### 9.7 Conversaciones y Mensajes

10 conversaciones ficticias vinculadas a necesidades y acuerdos. Cada conversacion tiene 3-5 mensajes.

#### Conv 1: MUSE <-> Sarah Chen (Acuerdo A1 - portada)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | MUSE | Hola Sarah, nos encanto tu portfolio. Queremos algo que combine el estilo Drones con una estetica mas limpia. Que opinas? | Hoy - 46 dias |
| 2 | Sarah Chen | Gracias! Estoy familiarizada con toda la discografia visual de MUSE. Propongo un enfoque minimalista con un unico elemento central poderoso, como hicieron con Origin of Symmetry. Os preparo 3 conceptos para el primer milestone. | Hoy - 46 dias |
| 3 | MUSE | Perfecto. Matt quiere que haya algun elemento que remita al espacio pero sin ser literal. Algo mas abstracto y emocional. | Hoy - 45 dias |
| 4 | Sarah Chen | Entendido. Voy a trabajar con nebulosas estilizadas y texturas metalicas. Os envio los bocetos en una semana. | Hoy - 44 dias |

#### Conv 2: Rosalia <-> Yuki Tanaka (Acuerdo A2 - violin)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Rosalia | Hola Yuki! Tu experiencia con Paco de Lucia Jr me convencio. Necesito alguien que entienda el compas flamenco pero pueda improvisar. | Hoy - 31 dias |
| 2 | Yuki Tanaka | Encantada! El flamenco tiene una riqueza ritmica que me fascina. He estado estudiando bulerias y tangos. Cuando empezamos las sesiones? | Hoy - 31 dias |
| 3 | Rosalia | La semana que viene en el estudio de Madrid. Te paso la direccion por privado. Trae el violin acustico y si tienes uno electrico tambien. | Hoy - 30 dias |

#### Conv 3: Bad Bunny <-> Andres Molina (Acuerdo A4 - video)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Bad Bunny | Bro, necesito que el video tenga esa vibra retro de los 90s pero con toques futuristas. Como un VHS del futuro. | Hoy - 11 dias |
| 2 | Andres Molina | Entiendo perfectamente. Estoy pensando en filtros CRT, scanlines y glitch pero con elementos 3D holograficos. El unboxing del vinyl box sera el hilo conductor. | Hoy - 11 dias |
| 3 | Bad Bunny | Me gusta. Agrega unas escenas en un estudio de grabacion vintage, como si estuvieramos en los 90s haciendo reggaeton underground. | Hoy - 10 dias |
| 4 | Andres Molina | Brutal. Voy a buscar un local que tenga esa estetica. Te mando el storyboard completo esta semana. | Hoy - 10 dias |
| 5 | Bad Bunny | Dale, estoy esperando. Quiero ver las 15 escenas antes de confirmar localizaciones. | Hoy - 9 dias |

#### Conv 4: Vetusta Morla <-> Carmen Diaz (Acuerdo A3 - CM)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Vetusta Morla | Carmen, necesitamos empezar con fuerza. La campana se lanza en 3 dias y queremos crear expectativa. | Hoy - 53 dias |
| 2 | Carmen Diaz | Perfecto. He preparado un calendario editorial con 3 fases: teaser (3 dias), lanzamiento (1 semana), y mantenimiento. Incluye behind-the-scenes de los ensayos con la orquesta. | Hoy - 53 dias |
| 3 | Vetusta Morla | Genial. Puedes usar fotos de nuestro archivo? Tenemos material inedito de los ensayos. | Hoy - 52 dias |
| 4 | Carmen Diaz | Si! Eso va a funcionar genial para los teasers. Tambien propongo un hashtag: #CableATierraSinfonica. Lo puedo activar manana? | Hoy - 52 dias |

#### Conv 5: Arcade Fire <-> David Okafor (Acuerdo A5 - foto)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Arcade Fire | David, el primer concierto es el viernes en Paris. Te mandamos los passes de backstage. Necesitamos que documentes TODO: desde el soundcheck hasta que se van los ultimos fans. | Hoy - 6 dias |
| 2 | David Okafor | Perfecto. Mi enfoque sera completamente documentalista: nada posado, todo espontaneo. Usare una Leica M10 para mantener un perfil bajo. Blanco y negro para backstage, color para el show. | Hoy - 6 dias |
| 3 | Arcade Fire | Nos encanta la idea del B&W para backstage. Win tiene un lado muy cinematografico que quedara genial. | Hoy - 5 dias |

#### Conv 6: C. Tangana <-> Lucia Fernandez (Acuerdo A6 - cancelado)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | C. Tangana | Lucia, malas noticias. Tenemos que posponer Las Ventas. Problemas de agenda con la orquesta de camara. | Hoy - 35 dias |
| 2 | Lucia Fernandez | Vaya, lo siento. Tenia todo el equipo preparado. Cuando seria la nueva fecha? | Hoy - 35 dias |
| 3 | C. Tangana | Todavia no lo sabemos. Te aviso en cuanto tengamos fecha. Vamos a cancelar el acuerdo por ahora y lo retomamos. Disculpa las molestias. | Hoy - 34 dias |

#### Conv 7: DJ Prometheus <-> Yuki Tanaka (Acuerdo via N9 - violin hip-hop)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | DJ Prometheus | Yuki! Vi que has trabajado con DJ Shadow. Justo ese tipo de fusion es lo que busco, pero con un rollo mas flamenco y sevillano. | Hoy - 22 dias |
| 2 | Yuki Tanaka | Me encanta la idea. He escuchado tus beats y tienen una base clasica muy solida. Puedo aportar arreglos de violin que dialoguen con los samples de Bach. | Hoy - 22 dias |
| 3 | Yuki Tanaka | Una pregunta: las sesiones son en el Conservatorio? Porque el ambiente acustico de alli es increible para violin. | Hoy - 21 dias |
| 4 | DJ Prometheus | Si! El Conservatorio nos presta la sala grande 3 dias. Vamos a grabar ahi con mics de ambiente + close mics. Va a sonar epico. | Hoy - 21 dias |

#### Conv 8: N1 MUSE <-> David Okafor (propuesta rechazada)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | David Okafor | Hola, he visto vuestra necesidad de portada. Aunque soy fotografo, hago trabajo conceptual que podria servir como base visual. | Hoy - 44 dias |
| 2 | MUSE | Gracias David, pero buscamos un enfoque mas ilustrativo/digital. Tu propuesta fotografica es interesante pero no encaja con lo que tenemos en mente para este disco. | Hoy - 43 dias |

#### Conv 9: N2 Bad Bunny <-> Elena Petrova (propuesta rechazada)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Elena Petrova | Hola! Mi propuesta de animacion 3D completa podria ser mas impactante que un rodaje tradicional. Aqui un reel de mis trabajos con Bjork. | Hoy - 23 dias |
| 2 | Bad Bunny | Esta brutal tu trabajo, pero para este video necesitamos tomas reales. El unboxing tiene que verse fisico, tangible. Quiza para otro proyecto. | Hoy - 22 dias |

#### Conv 10: N7 Billie Eilish <-> Elena Petrova (Acuerdo via propuesta aceptada)

| # | Remitente | Mensaje | Timestamp |
|---|-----------|---------|-----------|
| 1 | Billie Eilish | Elena, tus visualizers de Bjork son exactamente la vibra que busco. Dark, organico, con esos momentos de glitch. | Hoy - 8 dias |
| 2 | Elena Petrova | Gracias! Para tus remixes estoy pensando en un concepto unificado: elementos naturales (agua, humo, flores) que se distorsionan y se reconstruyen. Cada visualizer representaria un remix diferente pero con el mismo lenguaje visual. | Hoy - 8 dias |
| 3 | Billie Eilish | Me encanta. Quiero que sea lo mas organico posible, nada CGI artificial. Que parezca real pero imposible. | Hoy - 7 dias |

---

### 9.8 Valoraciones (Acuerdos Completados)

Para los 3 acuerdos completados (A1, A2, A3), ambas partes se valoran mutuamente.

| # | Acuerdo | Valorador | Valorado | Puntuacion | Comentario |
|---|---------|-----------|----------|-----------|------------|
| V1 | A1 (MUSE portada) | MUSE | Sarah Chen | 5/5 | Trabajo excepcional. Sarah capturo perfectamente la esencia de lo que buscabamos. Entrego antes del plazo y las adaptaciones para streaming fueron impecables. |
| V2 | A1 (MUSE portada) | Sarah Chen | MUSE | 5/5 | Clientes fantasticos. Brief muy claro, feedback rapido y constructivo. Un placer trabajar con artistas que saben lo que quieren. |
| V3 | A2 (Rosalia violin) | Rosalia | Yuki Tanaka | 5/5 | Yuki es una violinista increible. Su sensibilidad para el flamenco me sorprendio. Las sesiones fueron magicas, capturo exactamente el sentimiento que buscaba. |
| V4 | A2 (Rosalia violin) | Yuki Tanaka | Rosalia | 4/5 | Trabajar con Rosalia fue un sueno. Muy exigente pero con razon. La unica pega: algunos cambios de ultima hora en los arreglos que complicaron la planificacion. |
| V5 | A3 (Vetusta Morla CM) | Vetusta Morla | Carmen Diaz | 5/5 | Carmen supero nuestras expectativas. El engagement se disparo, el hashtag fue trending y la campana alcanzo el 85% del objetivo gracias en parte a su trabajo. |
| V6 | A3 (Vetusta Morla CM) | Carmen Diaz | Vetusta Morla | 5/5 | Vetusta Morla es un referente. Me dieron total libertad creativa, compartieron material inedito y respondieron siempre rapido. La campana fue un exito rotundo. |

**Resumen de valoraciones:**

| Profesional | Valoracion media | Total valoraciones |
|-------------|-----------------|-------------------|
| Sarah Chen (P3) | 5.0 | 1 recibida + 1 dada |
| Yuki Tanaka (P5) | 5.0 | 1 recibida + 1 dada |
| Carmen Diaz (P6) | 5.0 | 1 recibida + 1 dada |

---

### 9.9 Resumen de Datos de Crowdsourcing

| Concepto | Cantidad |
|----------|----------|
| Perfiles profesionales | 8 |
| Necesidades publicadas | 10 (6 cerradas/completadas, 2 abiertas, 2 en progreso) |
| Propuestas enviadas | 18 (8 aceptadas, 6 rechazadas, 4 pendientes) |
| Acuerdos creados | 6 (3 completados, 2 activos, 1 cancelado) |
| Milestones | 8 (5 completados, 2 en progreso, 1 pendiente) |
| Entregables | 12 (9 aprobados, 3 entregados pendientes revision) |
| Conversaciones | 10 (con 3-5 mensajes cada una) |
| Mensajes totales | ~38 |
| Valoraciones | 6 (bidireccionales en 3 acuerdos completados) |
| **Volumen total crowdsourcing** | **~18.700 EUR en acuerdos** |

### 9.10 Flujo de Seeding Crowdsourcing

```
Fase 1: Registrar 8 profesionales (usuarios)
    |
Fase 2: Artistas publican 10 necesidades (via API)
    |
Fase 3: Profesionales envian 18 propuestas
    |
Fase 4: Artistas aceptan 8 propuestas -> 6 acuerdos se crean
    |     (2 necesidades con propuestas pendientes no generan acuerdo)
    |     Artistas rechazan 6 propuestas
    |
Fase 5: Crear milestones y entregables en acuerdos activos/completados
    |
Fase 6: Aprobar entregables en acuerdos completados
    |     Marcar acuerdos A1, A2, A3 como completados
    |     Cancelar acuerdo A6
    |
Fase 7: Crear 10 conversaciones con mensajes
    |
Fase 8: Crear 6 valoraciones bidireccionales
```

**Specs adicionales para Playwright (seeding crowdsourcing):**

```
src/web/e2e/seeding/
  +-- 09-register-professionals.spec.ts
  +-- 10-create-needs.spec.ts
  +-- 11-submit-proposals.spec.ts
  +-- 12-manage-agreements.spec.ts
  +-- 13-crowdsourcing-messages.spec.ts
  +-- 14-crowdsourcing-ratings.spec.ts
```

---

## 10. Infraestructura Playwright para Landing

### 10.1 Archivos a Crear

```
src/web/
+-- playwright.config.ts            <-- Configuracion Playwright
+-- e2e/
    +-- helpers.ts                   <-- Helpers compartidos
    +-- fixtures/
    |   +-- test-data.ts             <-- Datos de test centralizados
    +-- scraping/
    |   +-- verkami-scraper.spec.ts   <-- Scraper de Verkami
    +-- seeding/
    |   +-- 01-register-artists.spec.ts
    |   +-- 02-create-profiles.spec.ts
    |   +-- 03-create-campaigns.spec.ts
    |   +-- 04-add-rewards.spec.ts
    |   +-- 05-publish-campaigns.spec.ts
    |   +-- 06-register-fans.spec.ts
    |   +-- 07-create-backings.spec.ts
    |   +-- 08-verify-platform.spec.ts
    |   +-- 09-register-professionals.spec.ts   <-- Crowdsourcing
    |   +-- 10-create-needs.spec.ts             <-- Crowdsourcing
    |   +-- 11-submit-proposals.spec.ts         <-- Crowdsourcing
    |   +-- 12-manage-agreements.spec.ts        <-- Crowdsourcing
    |   +-- 13-crowdsourcing-messages.spec.ts   <-- Crowdsourcing
    |   +-- 14-crowdsourcing-ratings.spec.ts    <-- Crowdsourcing
    +-- smoke/
        +-- auth.spec.ts             <-- Tests basicos de auth
        +-- campaigns.spec.ts        <-- Tests de campanias
        +-- backing.spec.ts          <-- Tests de backing
        +-- crowdsourcing.spec.ts    <-- Tests de crowdsourcing
```

### 10.2 Configuracion Playwright

Basada en la configuracion existente de Admin (`src/admin/playwright.config.ts`):

```typescript
// src/web/playwright.config.ts
import { defineConfig } from '@playwright/test';

export default defineConfig({
    testDir: './e2e',
    fullyParallel: false,           // Seeding requiere orden secuencial
    forbidOnly: !!process.env.CI,
    retries: process.env.CI ? 2 : 0,
    workers: 1,                      // Serial para seeding
    reporter: 'html',
    use: {
        baseURL: 'http://localhost:3000',  // Landing
        trace: 'on-first-retry',
        screenshot: 'only-on-failure',
    },
    projects: [
        {
            name: 'chromium',
            use: { browserName: 'chromium' },
        },
    ],
});
```

### 10.3 Helpers Necesarios

```typescript
// src/web/e2e/helpers.ts

// === CONSTANTES ===
const LANDING_BASE = 'http://localhost:3000';
const ADMIN_BASE = 'http://localhost:3001';
const API_BASE = 'http://localhost:5001';

// === AUTH HELPERS ===

// Login directo via API (rapido, sin UI)
loginViaApi(email, password) -> { token, userId }

// Registro directo via API
registerViaApi(email, password) -> { token, userId }

// Login en Landing via UI (valida formulario)
loginToLandingUI(page, email, password) -> void
// - Navega a /auth/login
// - Rellena email y password
// - Click submit
// - Espera redirect a dashboard

// Login en Admin via UI (reutiliza patron de admin helpers)
loginToAdminUI(page, email, password) -> void
// - Pre-seed localStorage con JWT
// - addInitScript para persistencia

// Registro en Landing via UI (valida formulario completo)
registerViaLandingUI(page, name, email, password) -> void
// - Navega a /auth/register
// - Rellena nombre, email, password, confirm
// - Click submit
// - Espera redirect

// === CAMPAIGN HELPERS (via Admin UI) ===

createCampaignViaWizard(page, campaignData) -> campaignId
// - Login en Admin
// - Navegar a /campanias/nueva
// - Completar 5 pasos del wizard
// - Retornar UUID de la campania creada

addRewardViaModal(page, campaignId, rewardData) -> rewardId
// - Navegar a /campanias/{id}/recompensas
// - Click "Agregar recompensa"
// - Rellenar modal
// - Guardar

publishCampaign(page, campaignId) -> void
// - Navegar a /campanias/{id}
// - Click "Publicar Ahora"
// - Confirmar en modal
// - Esperar toast de exito

// === BACKING HELPERS (via Landing UI) ===

createBackingViaUI(page, campaignId, backingData) -> void
// - Navegar a /campanias/{id}
// - Click "Apoyar"
// - Seleccionar reward (si aplica)
// - Ingresar monto
// - Escribir mensaje
// - Confirmar
// - Verificar pagina de confirmacion

// === API HELPERS (bypass UI para volumen) ===

createCampaignViaApi(token, data) -> campaignId
addRewardViaApi(token, rewardData) -> rewardId
publishViaApi(token, campaignId) -> void
createBackingViaApi(token, campaignId, backingData) -> backingId

// === VERIFICATION HELPERS ===

verifyCampaignInLanding(page, title) -> void
// - Navegar a /campanias
// - Buscar por titulo
// - Verificar que aparece en resultados

verifyDashboardStats(page, expectedStats) -> void
// - Login en Admin
// - Navegar a /dashboard
// - Verificar stats cards
```

### 10.4 Estrategia Dual: UI + API

Para equilibrar **validacion de flujos** con **eficiencia de ejecucion**:

| Operacion | Via UI (valida flujo) | Via API (rapido) |
|-----------|----------------------|------------------|
| Registrar artistas | Primeros 3-4 | Resto |
| Crear perfiles | Primeros 2-3 | Resto |
| Crear campanias | Primeros 3-4 | Resto |
| Agregar rewards | Primeros 8-10 | Resto (~65) |
| Publicar | Primeros 2-3 | Resto |
| Registrar fans | Todos (5) | - |
| Crear backings | Todos (~30) | - |

**Justificacion**: Los backings via Landing UI son el flujo critico que **nunca se ha testeado** (0 tests E2E en Landing). Todo backing debe pasar por la UI para validar el flujo completo.

---

## 11. Plan de Ejecucion

### 11.1 Prerequisitos

```bash
# 1. Servicios Docker corriendo
docker compose up -d

# 2. Verificar servicios (health check)
curl http://localhost:5001/swagger/index.html  # API Swagger
curl http://localhost:3000                      # Landing
curl http://localhost:3001                      # Admin

# 3. Instalar Playwright en Landing
cd src/web
npm install -D @playwright/test
npx playwright install chromium

# 4. Verificar DB limpia o con estado conocido
# (opcional: docker compose down -v && docker compose up -d para DB limpia)
```

### 11.2 Fase 1: Scraping Verkami

**Spec**: `src/web/e2e/scraping/verkami-scraper.spec.ts`

**Acciones:**
1. Navegar a `https://www.verkami.com/discover/projects/category/10-musica`
2. Esperar carga del listado
3. Extraer URLs de los primeros 10-15 proyectos
4. Para cada proyecto:
   - Navegar al detalle
   - Extraer campos (titulo, creador, objetivo, rewards)
   - Delay 2-3 segundos entre requests
5. Guardar en `data/scraped/verkami-campaigns.json`
6. Log resumen: N proyectos extraidos, campos obtenidos por proyecto

**Output**: `data/scraped/verkami-campaigns.json`
**Tiempo estimado**: ~5 minutos

### 11.3 Fase 2: Preparacion de Datos

**No requiere Playwright.** Preparacion de archivos JSON.

**Archivos a crear manualmente (o generar desde este documento):**

```
data/
+-- scraped/
|   +-- verkami-campaigns.json      <-- Output del scraper (Fase 1)
+-- fictional/
|   +-- famous-artists.json         <-- 10 artistas reconocidos (Seccion 6)
|   +-- indie-artists.json          <-- 5 artistas inventados (Seccion 7)
|   +-- fans.json                   <-- 5 perfiles de fan (Seccion 8)
+-- seed/
    +-- all-campaigns.json          <-- Merge de todo, formato unificado
```

**Formato unificado `all-campaigns.json`:**

```json
{
    "artists": [
        {
            "email": "muse.official@weplay-test.com",
            "password": "WePlay2026!",
            "profile": {
                "nombreArtistico": "MUSE",
                "generoMusical": "Alternative Rock",
                "descripcion": "...",
                "imagenUrl": "..."
            },
            "campaigns": [
                {
                    "titulo": "Will of the People II - Fan-Funded Album",
                    "subtitulo": "...",
                    "descripcionCorta": "...",
                    "importeObjetivo": 75000,
                    "tipoFinanciacionId": 1,
                    "fechaFinDias": 45,
                    "imagenPrincipalUrl": "...",
                    "rewards": [
                        {
                            "nombre": "Digital Download Exclusivo",
                            "importeMinimo": 10,
                            "descripcion": "...",
                            "tipoRewardId": 1,
                            "monedaId": 1,
                            "incluyeEnvioFisico": false,
                            "cantidadMaxima": null,
                            "esAddOn": false
                        }
                    ]
                }
            ]
        }
    ],
    "fans": [
        {
            "email": "maria.garcia@weplay-test.com",
            "password": "Test123!",
            "name": "Maria Garcia"
        }
    ],
    "backings": [
        {
            "fanEmail": "maria.garcia@weplay-test.com",
            "campaignTitle": "Will of the People II - Fan-Funded Album",
            "rewardName": "Vinyl Edicion Limitada",
            "monto": 35,
            "mensaje": "Desde el Wembley 2007 soy fan.",
            "esAnonimo": false
        }
    ]
}
```

### 11.4 Fase 3: Seeding via UI

**Ejecutar specs en orden secuencial (serial mode).**

#### Spec 01: Registrar Artistas (`01-register-artists.spec.ts`)

- Para cada artista en `all-campaigns.json`:
  - Navegar a Landing `/auth/register`
  - Rellenar: nombre (= nombreArtistico), email, password, confirm password
  - Click registrar
  - Verificar redirect post-registro
  - Guardar token para uso posterior
- **Primeros 3-4 via UI** (valida formulario)
- **Resto via API** (`POST /api/auth/register`)
- **Assertion**: Todos los artistas registrados correctamente

#### Spec 02: Crear Perfiles de Artista (`02-create-profiles.spec.ts`)

- Para cada artista registrado:
  - Login en Admin (via localStorage helper)
  - Navegar a `/perfil`
  - Rellenar: nombreArtistico, generoMusical, descripcion, imagenUrl
  - Click guardar
  - Verificar toast de exito
  - Guardar artistaId
- **Primeros 2-3 via UI**, resto via API (`POST /api/artistas`)
- **Assertion**: Todos los perfiles creados con datos correctos

#### Spec 03: Crear Campanias (`03-create-campaigns.spec.ts`)

- Para cada artista con campania:
  - Login en Admin como el artista
  - Navegar a `/campanias/nueva`
  - Completar wizard 5 pasos:
    1. Skip template (click "Sin plantilla" o similar)
    2. Info basica: titulo, objetivo, tipo financiacion, fecha fin, imagen
    3. Historia: subtitulo, descripcion corta
    4. Rewards: skip (placeholder MVP)
    5. Revision: click "Crear campania"
  - Verificar redirect a detalle
  - Guardar campaniaId
- **Primeros 3-4 via UI** (wizard completo), resto via API (`POST /api/campanias`)
- **Assertion**: Todas las campanias creadas en estado BORRADOR

#### Spec 04: Agregar Rewards (`04-add-rewards.spec.ts`)

- Para cada campania con rewards:
  - Login en Admin como artista owner
  - Navegar a `/campanias/{id}/recompensas`
  - Para cada reward:
    - Click "Agregar recompensa"
    - Rellenar modal: nombre, importeMinimo, descripcion, tipoRewardId, envio fisico, cantidad max
    - Click guardar
    - Verificar que aparece en la tabla
- **Primeros 8-10 rewards via UI** (valida modal completo), resto via API (`POST /api/rewards`)
- **Assertion**: Todos los rewards creados con campos correctos

#### Spec 05: Publicar Campanias (`05-publish-campaigns.spec.ts`)

- Para cada campania en borrador:
  - Login en Admin como artista owner
  - Navegar a `/campanias/{id}`
  - Click "Publicar Ahora"
  - Confirmar en modal de confirmacion
  - Esperar toast "Campania publicada exitosamente"
  - Verificar cambio de estado en UI
- **Primeros 2-3 via UI**, resto via API (`POST /api/campanias/{id}/publicar`)
- **Assertion**: Todas las campanias en estado PUBLICADA

#### Spec 06: Registrar Fans (`06-register-fans.spec.ts`)

- Para cada fan en `fans.json`:
  - Navegar a Landing `/auth/register`
  - Rellenar: nombre, email, password, confirm password
  - Click registrar
  - Verificar registro exitoso
  - Guardar token
- **Todos via UI** (5 fans, son pocos y valida el flujo)
- **Assertion**: 5 fans registrados correctamente

#### Spec 07: Crear Backings (`07-create-backings.spec.ts`)

**Este es el spec MAS IMPORTANTE** - valida el flujo critico de la Landing que no tiene tests.

- Para cada backing planificado:
  - Login como fan en Landing (via helper)
  - Navegar a `/campanias` y buscar campania por titulo
  - Entrar al detalle de la campania
  - Click "Apoyar" (o boton equivalente)
  - En el modal de backing:
    - Seleccionar reward (si aplica)
    - Verificar que el monto minimo se refleja
    - Ingresar monto
    - Escribir mensaje
    - Click confirmar
  - Verificar pagina de confirmacion:
    - Mensaje de exito
    - Nombre de campania
    - Monto backing
    - Reward seleccionado
- **TODOS via UI** (flujo critico, nunca testeado)
- **Assertion por cada backing**:
  - Confirmacion muestra datos correctos
  - Monto coincide con lo ingresado
  - Reward name coincide con lo seleccionado

#### Spec 08: Verificaciones Finales (`08-verify-platform.spec.ts`)

**Landing verificaciones:**
- Navegar a `/campanias` -> todas las publicadas visibles
- Buscar "MUSE" -> encuentra campania
- Buscar "Rosalia" -> encuentra campania
- Filtrar por "Activas" -> muestra campanias publicadas
- Entrar a detalle de MUSE -> progress bar > 0%
- Entrar a detalle de Bad Bunny -> muestra 2 backings
- Navegar a `/artistas/{id}` de MUSE -> perfil completo
- Navegar a `/artistas/{id}` de Vetusta Morla -> perfil completo

**Admin verificaciones (login como artista MUSE):**
- Dashboard `/dashboard`:
  - Stats cards muestran datos > 0
  - "Total Recaudado" > 0 EUR
  - "Backers Totales" >= 2
  - "Campanias Activas" = 1
- Tabla backings `/campanias/{id}/backings`:
  - Muestra 2 filas (Maria y Pablo)
  - Montos correctos (35 EUR y 100 EUR)
  - Nombres de fans visibles
  - Rewards asignados correctos
- Export CSV:
  - Click boton "Exportar CSV"
  - Verificar que descarga archivo

**Admin verificaciones (login como artista Bad Bunny):**
- Dashboard muestra 2 backings, total 165 EUR
- Detalle campania muestra progress correcto (165/50000)

---

## 12. Estructura de Archivos Final

```
WePlay_Rises/
+-- data/                                    <-- NUEVO: datos de seeding
|   +-- scraped/
|   |   +-- verkami-campaigns.json
|   +-- fictional/
|   |   +-- famous-artists.json
|   |   +-- indie-artists.json
|   |   +-- fans.json
|   |   +-- crowdsourcing-professionals.json   <-- NUEVO
|   |   +-- crowdsourcing-needs.json           <-- NUEVO
|   |   +-- crowdsourcing-proposals.json       <-- NUEVO
|   |   +-- crowdsourcing-agreements.json      <-- NUEVO
|   +-- seed/
|       +-- all-campaigns.json
|
+-- src/web/
|   +-- playwright.config.ts                 <-- NUEVO
|   +-- e2e/                                 <-- NUEVO: directorio completo
|       +-- helpers.ts
|       +-- fixtures/
|       |   +-- test-data.ts
|       +-- scraping/
|       |   +-- verkami-scraper.spec.ts
|       +-- seeding/
|       |   +-- 01-register-artists.spec.ts
|       |   +-- 02-create-profiles.spec.ts
|       |   +-- 03-create-campaigns.spec.ts
|       |   +-- 04-add-rewards.spec.ts
|       |   +-- 05-publish-campaigns.spec.ts
|       |   +-- 06-register-fans.spec.ts
|       |   +-- 07-create-backings.spec.ts
|       |   +-- 08-verify-platform.spec.ts
|       |   +-- 09-register-professionals.spec.ts   <-- Crowdsourcing
|       |   +-- 10-create-needs.spec.ts             <-- Crowdsourcing
|       |   +-- 11-submit-proposals.spec.ts         <-- Crowdsourcing
|       |   +-- 12-manage-agreements.spec.ts        <-- Crowdsourcing
|       |   +-- 13-crowdsourcing-messages.spec.ts   <-- Crowdsourcing
|       |   +-- 14-crowdsourcing-ratings.spec.ts    <-- Crowdsourcing
|       +-- smoke/
|           +-- auth.spec.ts
|           +-- campaigns.spec.ts
|           +-- backing.spec.ts
|           +-- crowdsourcing.spec.ts               <-- Crowdsourcing
|
+-- docs/
    +-- deep-research-report.md              <-- Referencia (con secciones numeradas)
    +-- 20260227_plan-data-seeding-e2e.md    <-- ESTE DOCUMENTO
```

---

## 13. Orden de Implementacion

| Paso | Tarea | Dependencia | Tiempo est. |
|------|-------|-------------|-------------|
| 1 | Configurar Playwright en `src/web/` (config + package.json) | Ninguna | 30 min |
| 2 | Crear `helpers.ts` y `fixtures/test-data.ts` | Paso 1 | 1 hora |
| 3 | Implementar `verkami-scraper.spec.ts` | Paso 1 | 2 horas |
| 4 | Crear JSONs de datos ficticios (`famous-artists.json`, `indie-artists.json`, `fans.json`) | Ninguna | 1 hora |
| 5 | Merge datos en `all-campaigns.json` | Paso 3 + 4 | 30 min |
| 6 | Spec 01: registrar artistas | Paso 2 | 1 hora |
| 7 | Spec 02: crear perfiles artista | Paso 6 | 45 min |
| 8 | Spec 03: crear campanias | Paso 5 + 7 | 2 horas |
| 9 | Spec 04: agregar rewards | Paso 8 | 2 horas |
| 10 | Spec 05: publicar campanias | Paso 9 | 30 min |
| 11 | Spec 06: registrar fans | Paso 2 | 30 min |
| 12 | Spec 07: crear backings | Paso 10 + 11 | 2 horas |
| 13 | Spec 08: verificaciones finales | Paso 12 | 1.5 horas |
| 14 | Smoke tests (auth, campaigns, backing) | Paso 2 | 2 horas |
| --- | **SUBTOTAL CROWDFUNDING** | | **~15 horas** |
| 15 | Crear JSONs crowdsourcing (professionals, needs, proposals, agreements) | Ninguna | 1 hora |
| 16 | Spec 09: registrar profesionales | Paso 2 + 15 | 30 min |
| 17 | Spec 10: crear necesidades (artistas publican) | Paso 7 + 15 | 1 hora |
| 18 | Spec 11: enviar propuestas | Paso 16 + 17 | 1.5 horas |
| 19 | Spec 12: gestionar acuerdos (aceptar, rechazar, milestones, entregables) | Paso 18 | 2 horas |
| 20 | Spec 13: mensajeria crowdsourcing | Paso 19 | 1 hora |
| 21 | Spec 14: valoraciones crowdsourcing | Paso 19 | 45 min |
| 22 | Smoke test crowdsourcing | Paso 2 | 1.5 horas |
| | **SUBTOTAL CROWDSOURCING** | | **~8 horas** |
| | **TOTAL** | | **~23 horas** |

**Nota sobre paralelismo**: Pasos 3 y 4 pueden ejecutarse en paralelo. Pasos 6 y 11 pueden ejecutarse en paralelo (registrar artistas y fans no dependen entre si). Paso 15 (JSONs crowdsourcing) puede ejecutarse en paralelo con pasos 3-4. El paso 14 y 22 (smoke tests) pueden desarrollarse en paralelo con los specs de seeding.

---

## 14. Criterios de Exito

### Must Have (Minimo Viable)

- [ ] Playwright configurado y funcional en `src/web/`
- [ ] Al menos 15 campanias creadas y publicadas en la plataforma
- [ ] Al menos 20 backings distribuidos entre campanias
- [ ] Flujo de backing E2E verificado via Landing UI
- [ ] Dashboard de Admin muestra stats correctos para al menos 3 artistas
- [ ] Los 10 artistas famosos tienen perfiles y campanias activas

### Nice to Have (Deseable)

- [ ] 25+ campanias totales (incluyendo scraping de Verkami)
- [ ] 30+ backings con distribucion variada
- [ ] Smoke tests reutilizables para CI/CD
- [ ] Export CSV de backings verificado
- [ ] Busqueda y filtros de campanias verificados en Landing
- [ ] Todos los rewards con datos completos (descripcion, envio, cantidad max)
- [ ] 10 necesidades de crowdsourcing publicadas con propuestas
- [ ] 6 acuerdos de crowdsourcing en distintos estados
- [ ] Conversaciones con mensajes entre artistas y profesionales
- [ ] Valoraciones bidireccionales en acuerdos completados
- [ ] Smoke test de flujo crowdsourcing completo

### KPIs de la Plataforma Post-Seeding

| Metrica | Valor esperado |
|---------|---------------|
| Campanias publicadas | >= 15 |
| Artistas con perfil | >= 15 |
| Total rewards | >= 50 |
| Total backings | >= 20 |
| Inversion simulada | >= 1.000 EUR |
| Campanias con progreso > 0% | >= 10 |
| Necesidades crowdsourcing | >= 8 |
| Acuerdos crowdsourcing | >= 4 |
| Profesionales registrados | >= 6 |
| Valoraciones | >= 4 |

---

## 15. Riesgos y Mitigaciones

| # | Riesgo | Probabilidad | Impacto | Mitigacion |
|---|--------|-------------|---------|------------|
| 1 | Verkami cambia estructura HTML / selectores | Media | Bajo | Fallback a datos 100% ficticios. Los 10 artistas famosos + 5 indie inventados ya cubren ~15 campanias. |
| 2 | Verkami bloquea scraping (reCAPTCHA, 403) | Baja | Bajo | reCAPTCHA solo en auth, no en lectura. Si bloquea, usar solo datos ficticios. |
| 3 | Docker services inestables | Baja | Alto | Health checks antes de cada fase. `docker compose restart` si falla. |
| 4 | Formularios Landing con bugs en backing flow | Media | Alto | Detectar y reportar como bugs. API fallback para completar seeding. |
| 5 | Tiempo de ejecucion de specs > esperado | Media | Medio | Usar API para volumen, UI solo para validacion. Reducir a 15-20 campanias si necesario. |
| 6 | Auth race conditions en Landing (similar a Admin) | Media | Medio | Reutilizar patron de `addInitScript` + localStorage del Admin. |
| 7 | Rate limiting del backend en creacion masiva | Baja | Medio | Delays entre operaciones. El backend tiene rate limiting configurable. |
| 8 | Rewards o backings con campos incompatibles | Baja | Medio | Validar esquema JSON contra API antes de ejecutar. Dry-run con 1 campania primero. |
| 9 | Endpoints crowdsourcing con bugs o cambios recientes | Media | Medio | Verificar endpoints con Swagger antes del seeding. API fallback si UI falla. |
| 10 | Maestras crowdsourcing no seedeadas (TipoNecesidad, ModalidadTrabajo) | Media | Alto | Verificar via GET /api/crowdsourcing/maestras. Si vacias, seedear via SQL o API. |

---

## Apendice A: Referencias Cruzadas con deep-research-report.md

| Tema en este documento | Seccion en deep-research-report.md |
|----------------------|-------------------------------------|
| Fuentes priorizadas para scraping | Seccion 3 > Tabla comparativa de alto nivel |
| Verkami: campos, selectores, paginacion | Seccion 4 > Verkami |
| Indiegogo: estructura API publica | Seccion 4 > Indiegogo |
| Modelo canonico de datos | Seccion 5 > Esquema de importacion propuesto |
| Mapeo Verkami -> modelo canonico | Seccion 5 > Mapeo por fuente > Verkami |
| Selectores XPath para Verkami | Seccion 5 > Selectores sugeridos > Verkami |
| Ejemplo JSON de campania scrapeada | Seccion 5 > Ejemplo de payload JSON |
| Flujo ETL general | Seccion 6 > Diagrama de flujo |
| Herramientas recomendadas | Seccion 7 > Librerias/herramientas |
| Etica y legalidad del scraping | Seccion 7 > Estrategias para respetar legalidad |
| CSV de ejemplo con 10 filas | Seccion 8 > Anexo |
| Datos maestros crowdsourcing (roles, templates) | Migracion `20260215145939_InitialCrowdsourcing.cs` |
| Crowdsourcing: BandLab, Kompoz, HITRECORD | Seccion 4 > BandLab, Kompoz, HITRECORD |

---

*Documento generado el 2026-02-27 (actualizado con datos de crowdsourcing). Basado en exploracion completa de la plataforma WePlay Rises (backend API, Admin dashboard, Landing web, tests E2E existentes, modulo de crowdsourcing) y el informe de investigacion de fuentes de crowdfunding musical.*
