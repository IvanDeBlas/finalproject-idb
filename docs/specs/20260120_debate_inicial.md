# WePlay Rises - Debate Inicial y Conclusiones

- **Fecha:** 2026-01-20
- **Participantes:** Jonay + Claude
- **Contexto:** Proyecto final del curso LIDR AI4Devs
- **Tiempo disponible:** 30 horas totales

---

## 1. Resumen del Proyecto

**WePlay Rises** es una plataforma de **crowdsourcing y crowdfunding para grupos musicales noveles** que integra tres modelos de negocio:

| Modulo | Descripcion | Complejidad |
|--------|-------------|-------------|
| **Crowdfunding** | Campanas de financiamiento con recompensas (tipo Kickstarter) | Alta |
| **Crowdsourcing** | Contratacion de profesionales (ilustradores, ingenieros de sonido, etc.) | Media |
| **Crowdpromotion** | Promocion mediante fans e influencers con tracking de conversiones | Alta |

---

## 2. Analisis del Modelo de Datos Existente

Se han revisado los archivos SQL en `Claude II/`:

### 2.1 Tablas Maestras (~25 tablas)
- Moneda, TipoProyecto, EstadoProyecto
- Estados de campana, pedido, aportacion, payout
- Tipos de necesidad, skill, promocion, reward
- Modalidad trabajo, estados de propuesta/acuerdo/entregable

### 2.2 Entidades Core
```
Artista (grupo/solista)
  |-- ProyectoArtistico (album, single, gira)
  |-- Artista_Miembro (N usuarios por artista)
  |-- Artista_Fan (followers)
  |-- Artista_PayoutCuenta (cuentas bancarias)
  |-- Artista_MembershipPlan (suscripciones)

FanProfile (usuarios fans)
PerfilProfesional (colaboradores freelance)
  |-- PerfilProfesional_Skill
  |-- PerfilProfesional_PortfolioItem
```

### 2.3 Modulo Crowdfunding (~15 tablas)
- `CampaniaCrowdfunding` - Campanas principales
- `CampaniaCrowdfunding_Reward` - Recompensas con stock
- `CampaniaCrowdfunding_StretchGoal` - Metas adicionales
- `PedidoCrowdfunding` + `_Linea` - Ordenes de backers
- `AportacionCrowdfunding` - Pagos
- `CampaniaCrowdfunding_Payout` - Liquidaciones al artista

### 2.4 Modulo Crowdsourcing (~10 tablas)
- `NecesidadCrowdsourcing` - Ofertas de trabajo
- `PropuestaCrowdsourcing` - Propuestas de profesionales
- `AcuerdoCrowdsourcing` - Contratos
- `AcuerdoCrowdsourcing_Milestone` - Hitos de pago
- `AcuerdoCrowdsourcing_Entregable` - Entregas
- `ValoracionCrowdsourcing` - Reviews bidireccionales
- `ConversacionCrowdsourcing` / `MensajeCrowdsourcing` - Chat

### 2.5 Modulo Crowdpromotion (~8 tablas)
- `Promotor` - Fans/influencers/marcas
- `Promotor_Wallet` - Balance de comisiones
- `PromoPrograma` - Programas de afiliados
- `PromoPrograma_Promotor` - Codigos referidos
- `PromoTarea` / `PromoTarea_Promotor` - Misiones
- `PromoEvento` - Tracking de conversiones
- `Promotor_WalletTransaccion` - Movimientos

### 2.6 Conclusion sobre Modelo de Datos
**El modelo esta MUY completo (~60 tablas)**, pero para un MVP de 30h es DEMASIADO.

---

## 3. Propuesta de Alcance MVP (30 horas)

### 3.1 Flujo E2E Prioritario: "Fan apoya campana de crowdfunding"

Este flujo crea valor completo y es el CORE de la plataforma:

```
1. Artista crea perfil
2. Artista crea campana de crowdfunding
3. Artista anade recompensas
4. Artista publica campana
5. Fan descubre campana (landing publica)
6. Fan selecciona recompensa y hace backing
7. Artista ve dashboard con progreso
```

### 3.2 Entidades Minimas para MVP

| Entidad | Campos Esenciales |
|---------|------------------|
| `User` | Email, Password, Nombre (Identity) |
| `Artista` | UserId, NombreArtistico, Descripcion, ImagenUrl |
| `Campania` | ArtistaId, Titulo, Descripcion, MetaFinanciera, FechaFin, Estado |
| `Reward` | CampaniaId, Nombre, Descripcion, MontoMinimo, StockLimitado |
| `Backing` | CampaniaId, UserId, RewardId, Monto, FechaCreacion |

**~5 tablas operacionales + Identity** (vs 60 del modelo completo)

### 3.3 Historias de Usuario MVP

#### Must-Have (3-5 historias)

| ID | Historia | Criterios de Aceptacion |
|----|----------|------------------------|
| US-01 | Como **artista**, quiero registrarme y crear mi perfil para presentar mi proyecto | - Registro con email/password<br>- Perfil con nombre artistico, bio, imagen |
| US-02 | Como **artista**, quiero crear una campana de crowdfunding con meta y fecha limite | - Formulario con titulo, descripcion, meta, fecha fin<br>- Campana en estado borrador |
| US-03 | Como **artista**, quiero anadir recompensas a mi campana | - CRUD de recompensas con monto minimo<br>- Stock opcional (limitado/ilimitado) |
| US-04 | Como **fan**, quiero ver campanas activas y hacer backing | - Listado publico de campanas<br>- Detalle con progreso y recompensas<br>- Formulario de backing (sin pago real) |
| US-05 | Como **artista**, quiero ver el progreso de mi campana | - Dashboard con total recaudado<br>- Lista de backers<br>- Porcentaje de meta |

#### Should-Have (1-2 opcionales)

| ID | Historia |
|----|----------|
| US-06 | Como fan, quiero ver las campanas ordenadas por popularidad o recientes |
| US-07 | Como artista, quiero publicar actualizaciones de mi campana |

---

## 4. Stack Tecnologico Propuesto

### 4.1 Backend (.NET 8)

```
WePlayRises.Api/
  |-- Controllers/
  |     |-- AuthController.cs
  |     |-- ArtistasController.cs
  |     |-- CampaniasController.cs
  |     |-- RewardsController.cs
  |     |-- BackingsController.cs
  |
  |-- Domain/
  |     |-- Entities/
  |     |-- Enums/
  |
  |-- Infrastructure/
  |     |-- Data/
  |     |-- Identity/   <-- Reutilizar modulo Identity de miUrba
  |
  |-- Application/
        |-- DTOs/
        |-- Services/
```

**Reutilizar de miUrba:**
- `Jonay.mU.Api.Identity` - Autenticacion JWT
- Patrones de CQRS simplificados
- Configuracion de EF Core

### 4.2 Frontend (React 19 + TypeScript)

```
weplay-frontend/
  |-- src/
        |-- components/
        |     |-- ui/           <-- Shadcn components
        |     |-- layout/
        |     |-- campania/
        |
        |-- pages/
        |     |-- Home.tsx
        |     |-- Login.tsx
        |     |-- Dashboard.tsx
        |     |-- CampaniaDetail.tsx
        |     |-- CreateCampania.tsx
        |
        |-- hooks/
        |-- services/
        |-- types/
```

**Stack Frontend:**
- React 19 + TypeScript
- Shadcn/ui (componentes)
- TailwindCSS
- FontAwesome (iconos)
- React Query (data fetching)
- React Router v6

### 4.3 Base de Datos (SQL Server)

- Usar modelo simplificado (5-10 tablas)
- EF Core Code-First
- Migrations

### 4.4 Template Shadcn Recomendado

**Opcion 1: Shadcn/ui Blocks** (gratuito)
- https://ui.shadcn.com/blocks
- Dashboard, Forms, Cards ya hechos

**Opcion 2: Taxonomy** (open source)
- https://github.com/shadcn-ui/taxonomy
- Incluye auth, dashboard, CRUD

---

## 5. Plan de Entregas

### Entrega 1 - Documentacion Tecnica (6 febrero 2025)
**~8 horas**

| Tarea | Horas |
|-------|-------|
| Documentacion de producto (objetivo, caracteristicas) | 1h |
| Historias de usuario con criterios | 2h |
| Arquitectura y diagrama de componentes | 2h |
| Modelo de datos simplificado | 1h |
| Setup inicial de repos | 2h |

### Entrega 2 - Codigo Funcional (6 marzo 2025)
**~15 horas**

| Tarea | Horas |
|-------|-------|
| API: Auth + Artistas | 3h |
| API: Campanias + Rewards | 4h |
| API: Backings | 2h |
| Frontend: Layout + Auth | 2h |
| Frontend: Listado campanias | 2h |
| Frontend: Detalle + Backing | 2h |

### Entrega Final (26 marzo 2025)
**~7 horas**

| Tarea | Horas |
|-------|-------|
| Dashboard artista | 2h |
| Tests unitarios + integracion | 2h |
| Deploy (Azure/Vercel) | 2h |
| Documentacion final | 1h |

---

## 6. Decisiones Arquitectonicas

### Decision 1: Simplificar modelo de datos
- **Problema:** El modelo existente tiene ~60 tablas
- **Decision:** Usar solo 5-10 tablas esenciales para MVP
- **Razon:** 30 horas no permiten implementar todo

### Decision 2: Reutilizar Identity de miUrba
- **Problema:** Autenticacion es compleja de implementar
- **Decision:** Copiar/adaptar modulo Identity existente
- **Razon:** Ahorra ~5-8 horas de desarrollo

### Decision 3: Sin pagos reales
- **Problema:** Stripe requiere configuracion y testing
- **Decision:** Simular pagos (mock)
- **Razon:** MVP demuestra flujo sin complejidad de pagos

### Decision 4: Monorepo simple
- **Problema:** Multiples repos complican CI/CD
- **Decision:** Monorepo con `/api` y `/frontend`
- **Razon:** Simplifica despliegue y desarrollo

---

## 7. Decisiones Confirmadas (2026-01-20)

### Decision: Identity y UserAccess
- **Copiar** modulo Identity de miUrba con **base de datos separada**
- **Crear** modulo UserAccess como parte del monolito modular
- Arquitectura: Monolito Modular con modulos independientes

### Decision: Alcance MVP
- **Crowdfunding:** Funcionalidad completa (flujo E2E)
- **Crowdsourcing:** Placeholder/Coming Soon (estructura preparada)
- **Crowdpromotion:** Placeholder/Coming Soon (estructura preparada)
- Razon: Preparado para crecer en futuras entregas

### Decision: Deploy
- **Backend:** Azure App Service
- **Base de datos:** Azure SQL
- **Frontend:** Azure Static Web Apps (o dentro del mismo App Service)

---

## 8. Proximos Pasos Inmediatos

1. [ ] Confirmar alcance MVP (solo crowdfunding)
2. [ ] Decidir sobre reutilizacion de Identity
3. [ ] Crear repositorio en Azure DevOps / GitHub
4. [ ] Inicializar proyecto .NET 8 API
5. [ ] Inicializar proyecto React + Shadcn
6. [ ] Definir modelo de datos simplificado en Code-First

---

## 9. Arquitectura del Monolito Modular

### 9.1 Diagrama de Modulos

```
                    +------------------+
                    |   API Gateway    |
                    |   (Controllers)  |
                    +--------+---------+
                             |
       +---------------------+---------------------+
       |                     |                     |
+------v------+     +--------v--------+    +------v------+
|   Identity  |     |   UserAccess    |    | Crowdfunding|
|   Module    |     |     Module      |    |   Module    |
+-------------+     +-----------------+    +-------------+
| - Auth      |     | - Registration  |    | - Campania  |
| - JWT       |     | - Profile       |    | - Reward    |
| - Tokens    |     | - Artista       |    | - Backing   |
+------+------+     +--------+--------+    +------+------+
       |                     |                    |
       +----------+----------+--------------------+
                  |
           +------v------+
           |  Shared DB  |
           | (SQL Server)|
           +-------------+

Placeholders (Entrega 2-3):
+----------------+     +------------------+
| Crowdsourcing  |     | Crowdpromotion   |
|    Module      |     |     Module       |
| (Coming Soon)  |     |  (Coming Soon)   |
+----------------+     +------------------+
```

### 9.2 Estructura de Carpetas del Backend

```
WePlayRises/
  |-- src/
  |     |-- WePlayRises.Api/                    # Host/Startup
  |     |     |-- Controllers/
  |     |     |-- Program.cs
  |     |     |-- appsettings.json
  |     |
  |     |-- Modules/
  |     |     |-- Identity/                     # Copiado de miUrba
  |     |     |     |-- WePlayRises.Identity.Api/
  |     |     |     |-- WePlayRises.Identity.Application/
  |     |     |     |-- WePlayRises.Identity.Domain/
  |     |     |     |-- WePlayRises.Identity.Infrastructure/
  |     |     |
  |     |     |-- UserAccess/                   # Nuevo modulo
  |     |     |     |-- WePlayRises.UserAccess.Api/
  |     |     |     |-- WePlayRises.UserAccess.Application/
  |     |     |     |-- WePlayRises.UserAccess.Domain/
  |     |     |     |-- WePlayRises.UserAccess.Infrastructure/
  |     |     |
  |     |     |-- Crowdfunding/                 # MVP - Funcional
  |     |     |     |-- WePlayRises.Crowdfunding.Api/
  |     |     |     |-- WePlayRises.Crowdfunding.Application/
  |     |     |     |-- WePlayRises.Crowdfunding.Domain/
  |     |     |     |-- WePlayRises.Crowdfunding.Infrastructure/
  |     |     |
  |     |     |-- Crowdsourcing/                # Placeholder
  |     |     |     |-- WePlayRises.Crowdsourcing.Api/
  |     |     |
  |     |     |-- Crowdpromotion/               # Placeholder
  |     |           |-- WePlayRises.Crowdpromotion.Api/
  |     |
  |     |-- Shared/
  |           |-- WePlayRises.Shared.Domain/
  |           |-- WePlayRises.Shared.Infrastructure/
```

### 9.3 Entidades por Modulo (MVP)

#### Identity Module (copiado de miUrba)
- `User` (ASP.NET Identity)
- `RefreshToken`
- `UserClaim`

#### UserAccess Module
- `Artista` - Perfil de artista/banda
- `ArtistaMembers` - Miembros del grupo
- `FanProfile` - Perfil de fan

#### Crowdfunding Module
- `Campania` - Campana de crowdfunding
- `Reward` - Recompensas
- `Backing` - Aportaciones de backers
- `CampaniaUpdate` - Actualizaciones (opcional)

---

## 10. Estructura de Workspace Completa

```
WePlayRises/                              # Raiz del proyecto
  |
  |-- .claude/
  |     |-- CLAUDE.md                     # Instrucciones para Claude Code
  |     |-- commands/                     # Comandos slash personalizados
  |     |-- agents/                       # Agentes especializados
  |     |-- settings.local.json
  |
  |-- docs/
  |     |-- product/
  |     |     |-- vision.md               # Vision del producto
  |     |     |-- user-stories.md         # Historias de usuario
  |     |
  |     |-- architecture/
  |     |     |-- overview.md             # Arquitectura general
  |     |     |-- data-model.md           # Modelo de datos
  |     |     |-- api-design.md           # Diseno de API
  |     |
  |     |-- decisions/                    # ADRs (Architecture Decision Records)
  |
  |-- src/
  |     |-- WePlayRises.Api/              # API Host
  |     |-- Modules/                      # Modulos del monolito
  |     |-- Shared/                       # Codigo compartido
  |     |-- weplay-frontend/              # Frontend React
  |
  |-- tests/
  |     |-- WePlayRises.Api.Tests/
  |     |-- WePlayRises.Crowdfunding.Tests/
  |
  |-- tasks/
  |     |-- backlog.md                    # Tareas pendientes
  |     |-- entrega-1.md                  # Entrega 1: Documentacion
  |     |-- entrega-2.md                  # Entrega 2: Codigo funcional
  |     |-- entrega-final.md              # Entrega final
  |
  |-- infrastructure/
  |     |-- azure/                        # ARM templates o Bicep
  |     |-- scripts/                      # Scripts de deployment
  |
  |-- README.md                           # Plantilla LIDR
  |-- prompts.md                          # Prompts usados (requisito LIDR)
  |-- WePlayRises.sln                     # Solucion .NET
```

---

## 11. Modelo de Datos Simplificado (MVP)

### 11.1 Diagrama ER

```
+------------------+       +------------------+       +------------------+
|      User        |       |     Artista      |       |    Campania      |
|------------------|       |------------------|       |------------------|
| Id (PK)          |<---+  | Id (PK)          |<---+  | Id (PK)          |
| Email            |    |  | UserId (FK)      |----+  | ArtistaId (FK)   |----+
| PasswordHash     |    |  | NombreArtistico  |       | Titulo           |    |
| Nombre           |    |  | Descripcion      |       | Descripcion      |    |
| FechaCreacion    |    |  | ImagenUrl        |       | MetaFinanciera   |    |
+------------------+    |  | Pais             |       | MontoRecaudado   |    |
                        |  | FechaCreacion    |       | FechaInicio      |    |
                        |  +------------------+       | FechaFin         |    |
                        |                             | Estado           |    |
                        |                             | ImagenUrl        |    |
                        |                             +------------------+    |
                        |                                                     |
                        |  +------------------+       +------------------+    |
                        |  |    FanProfile    |       |     Reward       |    |
                        |  |------------------|       |------------------|    |
                        +--| Id (PK)          |       | Id (PK)          |    |
                           | UserId (FK)      |       | CampaniaId (FK)  |----+
                           | Apodo            |       | Nombre           |
                           | FechaCreacion    |       | Descripcion      |
                           +------------------+       | MontoMinimo      |
                                    |                 | StockLimitado    |
                                    |                 | StockDisponible  |
                                    |                 | Orden            |
                                    |                 +------------------+
                                    |                          |
                                    |  +------------------+    |
                                    |  |     Backing      |    |
                                    |  |------------------|    |
                                    +->| Id (PK)          |    |
                                       | CampaniaId (FK)  |----+
                                       | UserId (FK)      |
                                       | RewardId (FK)    |----+
                                       | Monto            |
                                       | EsAnonimo        |
                                       | Mensaje          |
                                       | FechaCreacion    |
                                       +------------------+
```

### 11.2 Estados de Campania

```
BORRADOR -> PUBLICADA -> EN_CURSO -> EXITOSA
                    |            |-> FALLIDA
                    +-> CANCELADA
```

---

## 12. API Endpoints (MVP)

### Auth (Identity Module)
```
POST   /api/auth/register         # Registro de usuario
POST   /api/auth/login            # Login (devuelve JWT)
POST   /api/auth/refresh-token    # Refrescar token
POST   /api/auth/logout           # Logout
```

### Artistas (UserAccess Module)
```
GET    /api/artistas              # Listar artistas
GET    /api/artistas/{id}         # Detalle artista
POST   /api/artistas              # Crear perfil artista
PUT    /api/artistas/{id}         # Actualizar perfil
GET    /api/artistas/me           # Mi perfil de artista
```

### Campanias (Crowdfunding Module)
```
GET    /api/campanias             # Listar campanias publicas
GET    /api/campanias/{id}        # Detalle campania
POST   /api/campanias             # Crear campania (artista)
PUT    /api/campanias/{id}        # Actualizar campania
POST   /api/campanias/{id}/publicar   # Publicar campania
GET    /api/campanias/mis-campanias   # Mis campanias (artista)
```

### Rewards (Crowdfunding Module)
```
GET    /api/campanias/{id}/rewards     # Listar rewards
POST   /api/campanias/{id}/rewards     # Crear reward
PUT    /api/rewards/{id}               # Actualizar reward
DELETE /api/rewards/{id}               # Eliminar reward
```

### Backings (Crowdfunding Module)
```
POST   /api/campanias/{id}/backings    # Hacer backing
GET    /api/campanias/{id}/backings    # Listar backings (artista)
GET    /api/backings/mis-backings      # Mis backings (fan)
```

### Placeholders
```
GET    /api/crowdsourcing/info         # Coming Soon
GET    /api/crowdpromotion/info        # Coming Soon
```

---

## 13. Proximos Pasos Inmediatos

### Hoy (20 enero 2026)
1. [x] Documento de debate inicial
2. [ ] Crear repositorio en Azure DevOps
3. [ ] Inicializar solucion .NET con estructura modular
4. [ ] Copiar y adaptar modulo Identity
5. [ ] Crear CLAUDE.md del proyecto

### Esta semana
6. [ ] Definir modelo de datos en EF Core
7. [ ] Inicializar frontend React + Shadcn
8. [ ] Crear endpoints basicos de Identity
9. [ ] Configurar CI/CD basico en Azure DevOps

---

## 14. Debate: Estrategia de Desarrollo Rapido (Iteracion 2)

### 14.1 Base de Datos: SQL-First vs Code-First

| Enfoque | Pros | Contras |
|---------|------|---------|
| **SQL-First** (usar modelo existente) | Modelo ya diseñado y validado, Tablas maestras con datos | Demasiadas tablas (~60), Rigidez para MVP |
| **Code-First + Migrations** | Flexibilidad, Solo lo necesario, Versionado en Git | Hay que rediseñar desde cero |
| **Hibrido (Recomendado)** | Lo mejor de ambos mundos | Requiere planificacion |

**Recomendacion: Enfoque Hibrido**
1. **Usar Code-First para entidades MVP** (Artista, Campania, Reward, Backing)
2. **Importar tablas maestras del SQL existente** via script de seed
3. **Migrations de EF** para control de versiones
4. **Preparar estructura** para añadir tablas de CS/CP despues

```csharp
// Ejemplo: Seed de maestras desde SQL existente
public class MaestraSeeder
{
    public async Task SeedAsync(AppDbContext context)
    {
        // Importar EstadoCampania, TipoProyecto, etc.
        await context.Database.ExecuteSqlRawAsync(
            File.ReadAllText("Scripts/seed_maestras.sql"));
    }
}
```

### 14.2 Templates de Codigo

#### Templates para API (.NET)

| Template | Uso | Ahorro Estimado |
|----------|-----|-----------------|
| **CRUD Controller** | Generar controladores REST | 30 min/entidad |
| **Repository Pattern** | Acceso a datos | 20 min/entidad |
| **CQRS Commands/Queries** | Si usamos MediatR | 15 min/operacion |
| **DTOs + Mapping** | AutoMapper profiles | 10 min/entidad |
| **Validators** | FluentValidation | 15 min/entidad |

#### Templates para Tests

| Template | Uso | Ahorro Estimado |
|----------|-----|-----------------|
| **Unit Test (xUnit)** | Tests de servicios/handlers | 20 min/clase |
| **Integration Test** | Tests de API con WebApplicationFactory | 30 min/controller |
| **Test Fixtures** | Datos de prueba | 15 min/entidad |

#### Otros Templates Utiles

| Template | Uso |
|----------|-----|
| **Exception Handler** | Middleware global de errores |
| **Pagination Response** | Respuestas paginadas estandar |
| **API Response Wrapper** | Formato consistente de respuestas |
| **Swagger Documentation** | Documentacion de endpoints |
| **Docker Compose** | Entorno de desarrollo |
| **GitHub Actions / Azure Pipelines** | CI/CD |

**Herramientas para generar templates:**
- Scaffolding de Visual Studio / dotnet CLI
- Snippets personalizados en VS Code
- Claude Code con instrucciones en CLAUDE.md

### 14.3 UI: Templates y Diseños

#### Opcion A: Dashboard Template (Shadcn)

**Dashcode** (https://themeforest.net/item/dashcode-...)
- Precio: ~$29
- Incluye: Dashboard, forms, tables, charts
- Stack: Next.js + Tailwind + Shadcn

**Pros:**
- Componentes listos para usar
- Diseño profesional
- Dark mode incluido

**Contras:**
- Next.js (no React puro) - requiere adaptacion
- Puede tener mas de lo necesario
- Dependencia de actualizaciones del vendor

#### Opcion B: Crowdfunding Template

**Bakix** (https://elements.envato.com/.../bakix-...)
- Precio: Suscripcion Envato Elements (~$16/mes)
- Incluye: Landing, campañas, perfiles, dashboard

**Pros:**
- Diseñado especificamente para crowdfunding
- UX ya pensada para el dominio
- Assets e iconos incluidos

**Contras:**
- Probablemente HTML/CSS estatico
- Hay que integrarlo con React
- Puede no usar Shadcn

#### Recomendacion: Enfoque Hibrido UI

```
1. Shadcn/ui como base (gratuito, bien documentado)
   https://ui.shadcn.com/

2. Inspiracion de Bakix para UX/flujos
   - Copiar estructura de paginas
   - Adaptar paleta de colores
   - Usar como referencia de diseño

3. Blocks gratuitos de Shadcn
   https://ui.shadcn.com/blocks
   - Dashboard blocks
   - Form blocks
   - Card layouts

4. Iconos: FontAwesome (como pediste)
   https://fontawesome.com/icons
```

**Estimacion de ahorro con templates UI:**
- Sin template: 15-20h en UI
- Con template: 8-10h en UI
- **Ahorro: ~10 horas**

### 14.4 Workspace y CodePilot para WePlay

**SI, definitivamente reutilizar el patron del workspace de miUrba.**

#### Estructura Propuesta para WePlay Workspace

```
WePlayRises/
  |-- .claude/
  |     |-- CLAUDE.md                    # Instrucciones del proyecto
  |     |-- settings.local.json          # Variables de entorno
  |     |
  |     |-- commands/                    # Comandos slash
  |     |     |-- planning/
  |     |     |     |-- daily-plan.md    # /daily-plan
  |     |     |     |-- done.md          # /done {task-id}
  |     |     |
  |     |     |-- development/
  |     |           |-- new-entity.md    # /new-entity {name}
  |     |           |-- new-endpoint.md  # /new-endpoint {name}
  |     |           |-- run-tests.md     # /run-tests
  |     |
  |     |-- agents/                      # Agentes especializados
  |     |     |-- crud-generator.md      # Genera CRUD completo
  |     |     |-- test-generator.md      # Genera tests
  |     |     |-- ui-component.md        # Genera componentes React
  |     |
  |     |-- templates/                   # Templates de codigo
  |           |-- api/
  |           |     |-- controller.template.cs
  |           |     |-- service.template.cs
  |           |     |-- dto.template.cs
  |           |
  |           |-- tests/
  |           |     |-- unit-test.template.cs
  |           |     |-- integration-test.template.cs
  |           |
  |           |-- react/
  |                 |-- page.template.tsx
  |                 |-- component.template.tsx
  |                 |-- hook.template.ts
```

#### Comandos Slash Propuestos

| Comando | Descripcion |
|---------|-------------|
| `/daily-plan` | Genera plan diario con tareas |
| `/done {id}` | Marca tarea completada |
| `/new-entity {name}` | Crea entidad + DTO + migration |
| `/new-endpoint {name}` | Crea controller + service + tests |
| `/run-tests` | Ejecuta suite de tests |
| `/build-check` | Verifica compilacion |
| `/time-log {hours}` | Registra horas trabajadas |

### 14.5 Tracking de Horas

#### Opcion 1: Archivo de Log Manual

```markdown
# Time Log - WePlay Rises

| Fecha | Horas | Categoria | Descripcion |
|-------|-------|-----------|-------------|
| 2026-01-20 | 2.0 | Planning | Debate inicial, documento conclusiones |
| 2026-01-21 | 3.0 | Setup | Crear repo, estructura, Identity |
| ... | ... | ... | ... |

## Resumen por Categoria
- Planning: X horas
- Setup: X horas
- Backend: X horas
- Frontend: X horas
- Testing: X horas
- Deploy: X horas
- Documentacion: X horas

## Total: XX / 30 horas
```

#### Opcion 2: Comando Slash Automatizado

```markdown
<!-- .claude/commands/time-log.md -->
# /time-log

Registra horas trabajadas en el proyecto.

## Uso
/time-log 2.5 "Backend: endpoints de campanias"

## Accion
1. Añadir entrada a `tasks/time-log.md`
2. Actualizar totales
3. Mostrar horas restantes
```

#### Opcion 3: Integracion con Toggl/Clockify (Recomendado para LIDR)

El curso LIDR probablemente quiere ver:
- Cuanto tiempo dedicaste
- En que fases

**Crear tabla en README.md (plantilla LIDR):**

```markdown
## Registro de Tiempo

| Fase | Estimado | Real | Diferencia |
|------|----------|------|------------|
| Documentacion tecnica | 8h | Xh | +/-Xh |
| Backend desarrollo | 10h | Xh | +/-Xh |
| Frontend desarrollo | 8h | Xh | +/-Xh |
| Testing | 2h | Xh | +/-Xh |
| Deploy | 2h | Xh | +/-Xh |
| **TOTAL** | **30h** | **Xh** | **+/-Xh** |
```

---

## 15. Decisiones Confirmadas (Iteracion 2)

| # | Decision | Valor |
|---|----------|-------|
| 1 | Base de datos | **Hibrido** (Code-First + seed maestras) |
| 2 | UI Templates | **Envato Elements** (cuenta existente) |
| 3 | Crear workspace completo | **Si**, con comandos y agentes |
| 4 | Tracking de horas | **Markdown + /time-log** |

---

## 16. Templates de Envato Elements Recomendados

### 16.1 Templates de Crowdfunding (React/Next)

| Template | Descripcion | URL |
|----------|-------------|-----|
| **Krowd** | Crowdfunding & Charity React Next. Diseñado para crowdfunding, fundraising, nonprofits. Muy completo. | [Ver](https://elements.envato.com/krowd-crowdfunding-charity-react-next-template-QEU8CDC) |
| **Oxpitan** | React Next Nonprofit. Clean, profesional, para charities y donations. | [Ver](https://elements.envato.com/oxpitan-react-next-nonprofit-charity-template-6SGERQU) |
| **Bakix** | Crowdfunding Startup Fundraising. HTML pero con buen diseño de UX. | [Ver](https://elements.envato.com/es/bakix-crowdfunding-startup-fundraising-template-VYKB7QC) |

**Recomendacion:** Krowd es el mas completo para crowdfunding en React/Next.

### 16.2 Templates de Dashboard (React + Tailwind + Shadcn)

| Template | Descripcion | URL |
|----------|-------------|-----|
| **WowDash** | Tailwind & Nextjs Admin con **Shadcn**. El unico con Shadcn nativo. | [Ver](https://elements.envato.com/web-templates/shadcn) |
| **DashTail** | React Next Admin con Tailwind. Muchos componentes: charts, forms, tables. | [Ver](https://elements.envato.com/dashtail-admin-dashboard-template-76KW2MF) |
| **Dashcode Next** | Tailwind & Next.js Admin. Muy personalizable, buena documentacion. | [Ver](https://elements.envato.com/dashcode-next-tailwind-next-js-admin-dashboard-UMHWSTJ) |

**Recomendacion:** WowDash si queremos Shadcn nativo, DashTail si queremos mas componentes.

### 16.3 Estrategia de Uso de Templates

```
ENFOQUE PROPUESTO:

1. DESCARGAR:
   - Krowd (crowdfunding) -> Para paginas publicas: landing, campañas, perfiles
   - WowDash o DashTail -> Para dashboard de artista y admin

2. EXTRAER:
   - Componentes reutilizables
   - Estilos y variables CSS
   - Layouts de paginas
   - Flujos de UX

3. INTEGRAR:
   - Crear proyecto React limpio (Vite + React 19)
   - Importar componentes Shadcn desde WowDash
   - Copiar paginas de Krowd para crowdfunding
   - Adaptar a nuestros DTOs y API

4. PERSONALIZAR:
   - Paleta de colores WePlay Rises
   - Iconos FontAwesome
   - Branding propio
```

### 16.4 Paginas a Extraer de cada Template

#### De Krowd (Crowdfunding):
- [ ] Landing page principal
- [ ] Listado de campañas
- [ ] Detalle de campaña
- [ ] Formulario de backing/donacion
- [ ] Perfil de artista publico
- [ ] Pagina de registro/login

#### De WowDash/DashTail (Dashboard):
- [ ] Layout con sidebar
- [ ] Dashboard overview con stats
- [ ] Tablas con paginacion
- [ ] Formularios con validacion
- [ ] Cards y widgets
- [ ] Graficos de progreso

---

## 17. Proximos Pasos Actualizados

### Inmediato (Hoy)
1. [x] Documento de debate inicial
2. [x] Analisis de modelo de datos existente
3. [x] Definir estrategia de desarrollo rapido
4. [x] Seleccionar templates de Envato
5. [ ] **Descargar templates seleccionados**
   - Krowd (crowdfunding)
   - WowDash o DashTail (dashboard)
6. [ ] **Crear repositorio WePlayRises**
7. [ ] **Inicializar workspace con CLAUDE.md**

### Esta semana
8. [ ] Configurar solucion .NET modular
9. [ ] Copiar y adaptar modulo Identity
10. [ ] Crear modulo UserAccess (entidad Artista)
11. [ ] Crear modulo Crowdfunding (entidades Campania, Reward, Backing)
12. [ ] Inicializar frontend React + integrar templates

---

## 18. Time Log

| Fecha | Horas | Categoria | Descripcion |
|-------|-------|-----------|-------------|
| 2026-01-20 | 1.5 | Planning | Debate inicial, analisis modelo datos, documento conclusiones |

### Resumen
- **Total invertido:** 1.5 horas
- **Presupuesto restante:** 28.5 horas

---

**Estado:** Debate completado. Templates identificados. Listo para crear repositorio.
**Siguiente accion:** Descargar templates de Envato y crear estructura del proyecto.

---

## Nota de Evolución (2026-01-21)

> **Este documento es histórico.** La arquitectura frontend evolucionó durante la implementación.

### Cambios respecto al debate inicial:

| Aspecto | Debate Inicial | Implementación Final |
|---------|----------------|---------------------|
| **Frontend** | Una app `weplay-frontend/` | Dos apps separadas |
| **Landing pública** | React 19 + React Router | Vite + React 18 (`src/web`) |
| **Dashboard artista** | Mismo proyecto | Next.js 14 App Router (`src/admin`) |
| **Código compartido** | No contemplado | `src/shared` (types, schemas, utils) |
| **Arquitectura web** | No definida | Hexagonal feature-based |

### Razón del cambio:

Tras analizar los templates (Krowd y Dashtail), se determinó que:
- **Krowd** es ideal para landing pública (campañas, explorar)
- **Dashtail** es un dashboard admin que requiere App Router de Next.js

Separar en dos apps permite:
1. Optimizar cada app para su propósito (SSG vs SSR)
2. Deploys independientes
3. Bundles más pequeños

### Documentación actualizada:

- `README.md` - Arquitectura actual
- `CLAUDE.md` - Instrucciones de desarrollo
- `src/web/README.md` - Documentación landing
- `src/admin/README.md` - Documentación dashboard
