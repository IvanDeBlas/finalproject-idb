# WePlay Rises - Backlog de Tareas

> **Generado:** 2026-01-21
> **Tiempo total disponible:** 30 horas
> **Enfoque MVP:** Crowdfunding E2E

---

## Fase 1: Fundamentos (8h estimadas)

### WPR-001: Definir modelo de datos MVP en Code-First
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada (2026-02-12)

**Descripcion:**
Crear entidades EF Core simplificadas basadas en los scripts SQL existentes.

**Tareas:**
- [x] Crear `WePlayRises.Domain` project
- [x] Definir entidad `Artista` (simplificada)
- [x] Definir entidad `Campania` (simplificada)
- [x] Definir entidad `Reward`
- [x] Definir entidad `Backing`
- [x] Definir enums `EstadoCampania`, `TipoFinanciacion` (implementado como Master Tables: MaestraEstadoCampaniaCrowd, MaestraTipoFinanciacion)
- [x] Configurar EF Core Configurations (Fluent API) (UserAccessContext + CrowdfundingContext con Fluent API completo)

**Referencia:** `C:\Repos\MisCosas\...\Claude II\1_Crowd_Maestros_Artistas.sql`

**Criterios de aceptacion:**
- ✅ Entidades tienen propiedades minimas para MVP (Artista, CampaniaCrowdfunding, CampaniaCrowdfundingReward, PedidoCrowdfunding, AportacionCrowdfunding + StronglyTypedIds)
- ✅ Configuraciones EF Core completas (Fluent API en UserAccessContext y CrowdfundingContext con indices, relaciones, precision decimal/datetime)
- ✅ Valores de dominio modelados como Master Tables (MaestraEstadoCampaniaCrowd, MaestraTipoFinanciacion, MaestraMoneda, etc.)

---

### WPR-002: Copiar BuildingBlocks de miUrba
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada (2026-01-22)

**Descripcion:**
Copiar infraestructura base del MonolitoModular, cambiar namespaces.

**Tareas:**
- [x] Copiar carpeta `BuildingBlocks` completa (Kernel, Abstractions, EntityFramework, Caching)
- [x] Renombrar namespaces: `mU.Cloud.Api.*` → `WePlayRises.*`
- [x] Renombrar proyectos .csproj
- [x] Crear solucion `WePlayRises.sln`
- [x] Verificar que compila sin errores
- [x] Eliminar dependencias no necesarias (AzureInsights, Mail, etc.)

**Criterios de aceptacion:**
- ✅ Solucion compila sin errores (0 warnings, 0 errors)
- ✅ Namespaces correctamente renombrados a `WePlayRises.BuildingBlocks.*`
- ✅ Sin referencias a mU.Cloud.Api

---

### WPR-003: Crear estructura de modulos MVP
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada (2026-01-22)

**Descripcion:**
Crear estructura de carpetas para modulos UserAccess y Crowdfunding (Identity va en WebApi, ver WPR-006).

**Tareas:**
- [x] Crear `Modules/UserAccess/` (Artista, FanProfile)
- [x] Crear `Modules/Crowdfunding/` (Campania, Reward, Backing)
- [x] Crear `Modules/Crowdsourcing/` (placeholder)
- [x] Crear `Modules/Crowdpromotion/` (placeholder)
- [x] Crear `WebApi/` (composition root)

**Estructura creada:**
```
src/api/
  |-- BuildingBlocks/
  |     |-- Kernel/
  |     |-- Abstractions/
  |     |-- EntityFramework/
  |     |-- Caching/
  |-- Modules/
  |     |-- UserAccess/        (4 proyectos + Artista entity)
  |     |-- Crowdfunding/      (4 proyectos + Campania, Reward, Backing entities)
  |     |-- Crowdsourcing/     (placeholder)
  |     |-- Crowdpromotion/    (placeholder)
  |-- Shared/
  |-- WebApi/                  (Program.cs, appsettings, Swagger)
  |-- WePlayRises.sln          (13 proyectos)
```

**Criterios de aceptacion:**
- ✅ Estructura de carpetas creada
- ✅ Cada modulo de dominio tiene 4 proyectos (Domain, Application, Infra, WebApi)
- ✅ WebApi registra todos los modulos (DependencyInjection)
- ✅ Identity NO es modulo separado (ver WPR-006)

---

### WPR-004: Configurar DbContext y migrations
**Prioridad:** Alta | **Estimado:** 1h | **Estado:** ✅ Completada (2026-02-12)

**Descripcion:**
Configurar DbContexts por modulo con todas las entidades MVP.

**Tareas:**
- [x] Crear `UserAccessContext` (IdentityDbContext + Artista, FanProfile)
- [x] Crear `CrowdfundingContext` (Campania, Reward, Backing, Orders)
- [x] Crear `CoreContext` (Master Tables)
- [x] Configurar Fluent API para relaciones
- [x] Crear migration inicial para los 3 contextos
- [x] Seed de datos maestros (EstadoCampania, TipoFinanciacion, Moneda)

**Criterios de aceptacion:**
- ✅ Migrations generadas para UserAccess, Crowdfunding y Core
- ✅ Database creada con `dotnet ef database update`

---

### WPR-005: Setup proyecto React + Templates
**Prioridad:** Alta | **Estimado:** 1h | **Estado:** ✅ Completada (2026-02-12)

**Descripcion:**
Inicializar frontend React combinando templates krowd y dashtail.

**Tareas:**
- [ ] Crear proyecto Next.js 14+ con TypeScript en `src/web`
- [ ] Instalar Tailwind CSS + shadcn/ui
- [ ] Copiar componentes base de dashtail (layout, sidebar)
- [ ] Copiar cards de campania de krowd
- [ ] Configurar estructura de carpetas
- [ ] Verificar que compila y ejecuta

**Estructura objetivo:**
```
src/web/
  |-- app/
  |     |-- (public)/          # Landing, campanias publicas
  |     |-- (dashboard)/       # Panel artista/fan
  |     |-- (auth)/           # Login, register
  |-- components/
  |     |-- ui/               # shadcn components
  |     |-- layout/           # Header, Sidebar, Footer
  |     |-- campania/         # Cards, Progress, etc.
  |-- lib/
  |-- types/
```

**Criterios de aceptacion:**
- `npm run dev` funciona
- Layout base renderiza
- Tailwind + shadcn configurados

---

### WPR-006: Implementar Identity minimo (JWT + ASP.NET Core Identity)
**Prioridad:** Alta | **Estimado:** 1.5h | **Estado:** ✅ Completada (2026-02-12)

**Descripcion:**
Implementar autenticacion con JWT + ASP.NET Core Identity en modulo UserAccess.

**Arquitectura implementada:**
```
Modules/UserAccess/
  Domain/Constants/          Roles.cs, ServiceResponseMessageType.cs
  Application/
    Features/Auth/Commands/  RegisterCommand.cs, LoginCommand.cs
    Features/Auth/Queries/   GetCurrentUserQuery.cs
    Features/Auth/Validators/ RegisterCommandValidator.cs, LoginCommandValidator.cs
    Dtos/                    RegisterResponseDto.cs, LoginResponseDto.cs, UserInfoDto.cs
    Mapping/                 AuthProfile.cs
    Interfaces/Services/     IJwtTokenGenerator.cs
  Infra/Services/            JwtTokenGenerator.cs
  WebApi/Controllers/        AuthController.cs
WebApi/Program.cs            Identity config + Role seeding
```

**Tareas:**
- [x] Configurar Identity con IdentityUser + IdentityRole en Program.cs
- [x] Crear IJwtTokenGenerator + JwtTokenGenerator con roles en claims
- [x] Crear AuthController con:
  - [x] `POST /api/auth/register` - Registro con email/password, asigna rol Fan
  - [x] `POST /api/auth/login` - Login con SignInManager, retorna JWT con roles
  - [x] `GET /api/auth/me` - Obtener usuario actual (requiere [Authorize])
- [x] Configurar JWT Bearer en Program.cs (AddAuthentication, AddJwtBearer)
- [x] Crear constantes Roles (Artista, Fan, Admin) y seed en startup
- [x] Crear migration para tablas de Identity
- [x] JWT incluye claims: NameIdentifier, Email, Role

**Criterios de aceptacion:**
- ✅ Usuario puede registrarse con email/password (rol Fan por defecto)
- ✅ Usuario puede hacer login y recibir JWT valido con roles
- ✅ Endpoints protegidos con [Authorize] funcionan (GET /me)
- ✅ JWT incluye claims: UserId, Email, Roles

---

### WPR-006a: Configurar User Secrets para desarrollo
**Prioridad:** Alta | **Estimado:** 15min | **Estado:** ✅ Completada (2026-02-12)
**Dependencia:** WPR-003 (Estructura de modulos)
**Bloquea:** WPR-006

**Descripcion:**
Configurar .NET User Secrets para almacenar secretos de desarrollo fuera del repositorio. Esto evita exponer JWT Keys y otras credenciales en archivos versionados.

**Tareas:**
- [x] Ejecutar `dotnet user-secrets init` en WebApi
- [x] Configurar `Jwt:Key` en User Secrets: `dotnet user-secrets set "Jwt:Key" "DevKey..."`
- [x] Limpiar secretos hardcodeados de `appsettings.Development.json`
- [x] ~~Ejecutar `git rm --cached` si el archivo tiene secretos versionados~~ (no necesario, ya en .gitignore y no tracked)
- [x] Documentar proceso de setup en README para nuevos desarrolladores

**Referencia:** `docs/analisis/20260127_implementacion-secretos-identity.md` (Seccion 1)

**Criterios de aceptacion:**
- ✅ User Secrets inicializado (UserSecretsId en .csproj)
- ✅ `Jwt:Key` almacenado en User Secrets, no en archivos
- ✅ `appsettings.Development.json` sin secretos sensibles
- ✅ Instrucciones de onboarding documentadas

---

### WPR-015: Pruebas manuales E2E - Levantar y probar todo
**Prioridad:** Alta | **Estimado:** 1h | **Real:** 1h | **Estado:** ✅ Completada (2026-02-14)
**Dependencia:** WPR-010 ✅, WPR-011 (en progreso)

**Descripcion:**
Ejecutar backend y ambos frontends para realizar pruebas manuales del flujo completo. Verificar que todo funciona correctamente de punta a punta antes de continuar con nuevas features.

**Plan de pruebas:** [`docs/20260212_pruebas-e2e-manuales.md`](../docs/20260212_pruebas-e2e-manuales.md)

**Tareas:**
- [x] Levantar backend: `dotnet run --project src/api/WebApi` (o Docker)
- [x] Levantar landing: `cd src/web && npm run dev`
- [ ] Levantar admin: `cd src/admin && npm run dev`
- [x] Probar registro y login de usuario
- [ ] Probar creacion de perfil artista
- [ ] Probar creacion de campania (borrador)
- [ ] Probar edicion y publicacion de campania
- [ ] Probar visualizacion de campanias en landing publica
- [ ] Documentar bugs y ajustes encontrados

**Bugs encontrados:**
- [x] **BUG-001**: Frontend no enviaba `confirmPassword` en registro (campo requerido por backend). Fix: agregado campo al formulario y schema Zod.
- [x] **BUG-002**: Login fallaba silenciosamente. `auth.service.ts` llamaba `/auth/me` antes de guardar token en localStorage, causando 401 y redirect. Fix: usar datos de la respuesta de login directamente.
- [x] **BUG-003**: Proxy Vite apuntaba a `https://localhost:5001` pero API Docker usa HTTP. Fix: cambiado a `http://localhost:5001`.
- [x] **BUG-004**: Password min length inconsistente (8 en backend, 6 en frontend). Fix: unificado a 6 en Identity config, validator y schema Zod.
- [x] **BUG-005**: `GET /api/artistas/me` devolvia 400 porque "me" se enrutaba a `GET {id}` (Guid). Fix: endpoint dedicado `[HttpGet("me")]` en ArtistasController.
- [x] **BUG-006**: Error codes del handler (ej: "4008") no coincidian con strings del controller (ej: "ARTISTA_ALREADY_EXISTS"). Fix: controller usa `HttpStatusCode` del ServiceResponse.
- [x] **BUG-007**: POST `/api/artistas` devolvia HTTP 500 en vez de 409 para artista duplicado. Mismo fix que BUG-006.

**Criterios de aceptacion:**
- [x] Los servicios levantan sin errores (API en Docker, landing con Vite local)
- [ ] Flujo E2E funciona: registro -> artista -> campania -> publicar -> ver en landing
- [ ] Lista de bugs/ajustes documentada

---

## Fase 2: Historias de Usuario MVP (12h estimadas)

### WPR-010: US-01 - Registro de Artista
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada (2026-02-12)
**Dependencia:** WPR-006 (Identity minimo)

**Historia:** Como artista, quiero registrarme y crear mi perfil para presentar mi proyecto musical.

**Backend:**
- [x] `POST /api/artistas` - Crear perfil artista (usa UserId del JWT)
- [x] `GET /api/artistas/{id}` - Perfil publico por ID
- [x] `GET /api/artistas/by-user/{userId}` - Mi perfil (por UserId)
- [ ] `PUT /api/artistas/{id}` - Actualizar perfil

**Nota:** El registro de usuario (`POST /api/auth/register`) ya esta en WPR-006.

**Frontend:**
- [x] Pagina `/auth/register` con formulario (Landing + Admin)
- [x] Pagina `/artista/perfil` con formulario de perfil (Landing + Admin)
- [x] Pagina perfil publico artista (Landing)
- [ ] Subida de imagen (puede ser URL por ahora)

**Criterios de aceptacion:**
- [x] Usuario puede registrarse con email/password
- [x] Usuario puede crear perfil de artista
- [x] Perfil muestra nombre artistico, bio, imagen

---

### WPR-011: US-02 - Crear Campania
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** ✅ Completada (2026-02-14)

**Historia:** Como artista, quiero crear una campania de crowdfunding para financiar mi proyecto.

**Backend:**
- [ ] `POST /api/campanias` - Crear campania (estado BORRADOR)
- [ ] `GET /api/campanias/{id}` - Detalle campania
- [ ] `PUT /api/campanias/{id}` - Actualizar campania
- [ ] `POST /api/campanias/{id}/publicar` - Cambiar estado a PUBLICADA

**Frontend:**
- [ ] Pagina `/dashboard/campanias/nueva` con wizard/formulario
- [ ] Campos: titulo, descripcion, meta financiera, fecha fin
- [ ] Preview de como se vera la campania

**Criterios de aceptacion:**
- [ ] Campania se crea en estado BORRADOR
- [ ] Artista puede editar antes de publicar
- [ ] Al publicar, estado cambia a PUBLICADA

---

### WPR-012: US-03 - Definir Recompensas
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada (2026-02-13)

**Historia:** Como artista, quiero anadir recompensas a mi campania para incentivar las aportaciones.

**Backend:**
- [x] `POST /api/campanias/{id}/rewards` - Crear reward
- [x] `GET /api/campanias/{id}/rewards` - Listar rewards
- [x] `PUT /api/rewards/{id}` - Actualizar reward
- [x] `DELETE /api/rewards/{id}` - Eliminar reward

**Frontend:**
- [x] Seccion en wizard de campania para rewards
- [x] Form: nombre, descripcion, monto minimo, stock
- [x] Lista de rewards con edicion inline

**Criterios de aceptacion:**
- [x] Se pueden crear multiples rewards
- [x] Cada reward tiene monto minimo
- [x] Stock limitado es opcional

---

### WPR-013: US-04 - Hacer Backing
**Prioridad:** Alta | **Estimado:** 2h | **Real:** 1h | **Estado:** ✅ Completada (2026-02-13)

**Historia:** Como fan, quiero apoyar una campania para ayudar al artista y obtener recompensas.

**Backend:**
- [x] `GET /api/campanias` - Listar campanias publicas
- [x] `POST /api/campanias/{id}/backings` - Crear backing
- [x] Actualizar `ImportePledgedActual` en campania
- [x] Decrementar stock de reward si limitado

**Frontend:**
- [x] Pagina `/campanias` con listado
- [x] Pagina `/campanias/{id}` con detalle
- [x] Barra de progreso de meta
- [x] Seleccion de reward y monto
- [x] Formulario de backing (sin pago real)

**Criterios de aceptacion:**
- [x] Campanias activas visibles publicamente
- [x] Progreso se actualiza al hacer backing
- [x] Backing queda registrado

---

### WPR-014: US-05 - Dashboard Artista
**Prioridad:** Alta | **Estimado:** 2h | **Real:** 0.5h | **Estado:** ✅ Completada (2026-02-14)

**Historia:** Como artista, quiero ver el progreso de mi campania para conocer las metricas.

**Backend:**
- [x] `GET /api/campanias/mis-campanias` - Campanias del artista
- [x] `GET /api/campanias/{id}/backings` - Lista de backers
- [x] `GET /api/campanias/{id}/stats` - Estadisticas (opcional)

**Frontend:**
- [x] Pagina `/dashboard` con resumen
- [x] Cards con total recaudado, % meta, num backers
- [x] Lista de backings recientes
- [ ] Grafico de progreso (opcional)

**Criterios de aceptacion:**
- [x] Dashboard muestra metricas clave
- [x] Lista de backers visible
- [x] Porcentaje de meta calculado

---

## Fase 3: Testing y Deploy (10h estimadas)

### WPR-020: Tests unitarios backend
**Prioridad:** Media | **Estimado:** 3h | **Estado:** ✅ Completada (2026-02-14)

**Tareas:**
- [x] Tests de Handlers CQRS
- [x] Tests de Validators
- [x] Tests de Services
- [x] Cobertura objetivo: ≥80%

---

### WPR-021: Tests de integracion
**Prioridad:** Media | **Estimado:** 2h | **Estado:** ✅ Completada (2026-02-14)

**Tareas:**
- [x] Coleccion Postman para API
- [x] Tests end-to-end flujo completo
- [x] Newman para CI/CD

---

### WPR-022: Deploy Azure
**Prioridad:** Media | **Estimado:** 3h | **Estado:** ✅ Completada

**Tareas:**
- [ ] Crear Azure SQL Database
- [ ] Crear Azure App Service para API
- [ ] Crear Azure Static Web Apps para frontend
- [ ] Configurar CI/CD pipeline
- [ ] Ejecutar migrations en produccion

---

### WPR-024: Cambios finales, seed y preparacion entrega
**Prioridad:** Alta | **Estimado:** 3h | **Real:** 3h | **Estado:** ✅ Completada (2026-03-24)

**Descripcion:**
Sesion final de ajustes, seed data y preparacion para la entrega del proyecto.

**Tareas:**
- [x] Refactor rutas API: eliminar prefijo '/api' redundante
- [x] Migracion inicial modulo Crowdpromotion (schema DB)
- [x] Crowd flags en Campania DTOs y queries
- [x] Unit tests para UserAccess application features
- [x] Documentacion credenciales de test
- [x] Mejoras generales de estructura y legibilidad

**Commits (2026-03-24):**
- `df44091` refactor: update API routes to remove redundant '/api' prefix
- `09ed191` feat: add test credentials documentation
- `992b188` Implement feature X to enhance user experience
- `288053a` Add initial migration for Crowdpromotion module
- `009e38a` feat: Add crowd flags functionality to Campania DTOs
- `fc22fe3` Refactor code structure for improved readability
- `f0bd0c0` Add unit tests for user access application features

---

### WPR-023: Documentacion final
**Prioridad:** Media | **Estimado:** 2h | **Estado:** Pendiente

**Tareas:**
- [ ] Completar README.md con screenshots
- [ ] Documentar API en Swagger
- [ ] Grabar video demo (requisito LIDR)
- [ ] Actualizar prompts.md con prompts usados

---

## Fase 4: Crowdsourcing (horas restantes)

### US-CS-01: Templates y Guia para Artistas Noveles
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** ✅ Completada
**Feature:** `cs-templates-guia`

**Historia:** Como artista novel, quiero acceder a templates de proyectos musicales para tener una guia de que necesidades profesionales requiero.

**Backend:**
- [x] `GET /api/crowdsourcing/templates` - Listar templates
- [x] `GET /api/crowdsourcing/templates/{id}` - Detalle template con necesidades
- [x] `GET /api/crowdsourcing/maestras/roles-profesionales` - Catalogo roles profesionales
- [x] `GET /api/crowdsourcing/maestras/categorias-rol` - Categorias de roles
- [x] `POST /api/crowdsourcing/templates/{id}/generar` - Generar necesidades desde template

**Frontend (Admin):**
- [x] Pagina listado templates con filtros
- [x] Pagina detalle template con necesidades
- [x] Selector de templates en flujo de creacion proyecto

**Planes:** `plans/cs-templates-guia/`
**Specs:** `docs/user-stories/cs-templates-guia/`

---

### US-CS-02: Gestion de Necesidades de Crowdsourcing
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** ✅ Completada
**Feature:** `cs-gestionar-necesidades`

**Historia:** Como artista registrado, quiero publicar, listar, editar y cerrar necesidades de servicios profesionales para gestionar mis solicitudes de crowdsourcing y atraer propuestas de profesionales cualificados.

**Backend:**
- [ ] `POST /api/crowdsourcing/necesidades` - Crear necesidad
- [ ] `GET /api/crowdsourcing/necesidades` - Listar necesidades con filtros y paginacion
- [ ] `GET /api/crowdsourcing/necesidades/{id}` - Detalle necesidad
- [ ] `PUT /api/crowdsourcing/necesidades/{id}` - Actualizar necesidad (solo estado Abierta)
- [ ] `POST /api/crowdsourcing/necesidades/{id}/cerrar` - Cerrar necesidad (rechaza propuestas pendientes)

**Frontend (Admin):**
- [ ] Pagina listado necesidades con filtros por estado y texto libre
- [ ] Formulario crear/editar necesidad
- [ ] Pagina detalle necesidad con propuestas
- [ ] Accion cerrar necesidad con confirmacion

**Planes:** `plans/cs-gestionar-necesidades/`
**Specs:** `docs/user-stories/cs-gestionar-necesidades/`

---

### US-CS-03: Explorar Propuestas - Vista Profesional
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** ✅ Completada
**Feature:** `cs-explorar-propuestas`

**Historia:** Como profesional de la industria musical, quiero explorar necesidades abiertas, ver sus detalles, enviar propuestas y gestionar mis propuestas enviadas.

**Backend:**
- [x] `GET /api/crowdsourcing/necesidades` - Listado publico con filtros y paginacion
- [x] `GET /api/crowdsourcing/necesidades/{id}` - Detalle publico con campos calculados (yaPropuso, esPropietario, tienePerfilProfesional)
- [x] `POST /api/crowdsourcing/necesidades/{id}/propuestas` - Enviar propuesta
- [x] `GET /api/crowdsourcing/propuestas/mis-propuestas` - Mis propuestas con filtro de estado
- [x] `PATCH /api/crowdsourcing/propuestas/{id}/retirar` - Retirar propuesta pendiente

**Frontend (Landing):**
- [x] Pagina explorar necesidades con filtros, busqueda y paginacion
- [x] Pagina detalle necesidad con CTA dinamico (5 estados)
- [x] Dialog enviar propuesta con React Hook Form + Zod
- [x] Pagina mis propuestas con filtro por estado
- [x] Dialog retirar propuesta con confirmacion
- [x] 12 componentes, 5 hooks, 2 API services
- [x] 62 tests unitarios (8 suites, 100% passing)

**Planes:** `plans/cs-explorar-propuestas/`
**Specs:** `docs/user-stories/cs-explorar-propuestas/`

---

## Fase 4b: Crowdsourcing - US Pendientes (futuro)

### US-CS-04: Acuerdos, Milestones y Entregables
**Prioridad:** Alta | **Estimado:** 4h | **Estado:** ✅ Completada (2026-02-18)
**Feature:** `cs-acuerdos-entregables`
**Dependencia:** US-CS-03

**Historia:** Como artista o profesional, quiero gestionar el ciclo de vida completo del acuerdo de trabajo: aceptar propuestas, definir milestones, subir y revisar entregables.

**Specs:** `docs/product/US-CS-04-acuerdos-entregables.md`

---

### US-CS-05: Mensajeria entre Artistas y Profesionales
**Prioridad:** Media | **Estimado:** 2h | **Real:** 1h | **Estado:** ✅ Completada (2026-02-18)
**Feature:** `cs-mensajeria`
**Dependencia:** US-CS-04

**Historia:** Como participante de un acuerdo, quiero comunicarme con la otra parte via mensajeria interna para coordinar el trabajo.

**Specs:** `docs/product/US-CS-05-mensajeria.md`

---

### US-CS-06: Valoraciones Post-Acuerdo
**Prioridad:** Media | **Estimado:** 1.5h | **Estado:** ✅ Completada (2026-02-21)
**Feature:** `cs-valoraciones`
**Dependencia:** US-CS-04

**Historia:** Como artista o profesional que completo un acuerdo, quiero dejar una valoracion mutua para construir reputacion en la plataforma.

**Specs:** `docs/product/US-CS-06-valoraciones.md`

---

## Fase 5: CrowdPromotion (futuro)

> Sistema de referidos/afiliados donde fans e influencers promocionan campanas a cambio de comisiones.
> Modelo de dominio ya existe (9 entidades). Falta CQRS, endpoints y frontend.

### US-CP-01: Registro y Gestion del Perfil de Promotor
**Prioridad:** Alta | **Estimado:** 2h | **Real:** 1h | **Estado:** ✅ Completada (2026-02-25) 
**Feature:** `cp-perfil-promotor`

**Historia:** Como fan, quiero crear mi perfil de promotor con nombre publico, redes sociales y tipo (Fan Embajador, Influencer, Medio/Blog) para postularme a programas de promocion.

**Specs:** `docs/product/US-CP-01-perfil-promotor.md`

---

### US-CP-02: Crear y Gestionar Programas de Promocion
**Prioridad:** Alta | **Estimado:** 4h | **Estado:** ✅ Completada
**Feature:** `cp-programas-promocion`
**Dependenciaprogreso:** US-CP-01

**Historia:** Como artista, quiero crear un programa de promocion definiendo comisiones, tareas y reglas para que fans e influencers difundan mi campana.

**Specs:** `docs/product/US-CP-02-programas-promocion.md`

---

### US-CP-03: Inscripcion a Programas de Promocion
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** ✅ Completada
**Feature:** `cp-inscripcion-programa`
**Dependencia:** US-CP-02

**Historia:** Como promotor, quiero inscribirme a programas de promocion y recibir mi codigo de referido y URL personalizada.

**Admin (completado):**
- [x] `inscripcion.service.ts` - Service con 5 metodos (getInscripciones, aprobar, rechazar, bloquear, darDeBaja)
- [x] 3 query hooks + 4 mutation hooks con toast + invalidation
- [x] 3 tabs (Solicitudes, Aprobados, Bloqueados) + InscripcionConfirmDialog + SolicitudCard + AprobadoCard
- [x] Modificacion PromoProgramaDetailClient (3 nuevos tabs con badge) y KpiCards (5ta KPI)
- [x] 75 unit tests (7 suites, 0 failed) + 17 E2E specs

**Specs:** `docs/product/US-CP-03-inscripcion-programa.md`

---

### US-CP-04: Ejecucion de Tareas de Promocion
**Prioridad:** Alta | **Estimado:** 3h | **Real:** 1h | **Estado:** ✅ Completada
**Feature:** `cp-tareas-promocion`
**Dependencia:** US-CP-03

**Historia:** Como promotor inscrito, quiero completar tareas de promocion (compartir, publicar, referir) y registrar su cumplimiento para ganar recompensas.

**Specs:** `docs/product/US-CP-04-tareas-promocion.md`

---

### US-CP-05: Tracking y Metricas de Eventos Promocionales
**Prioridad:** Media | **Estimado:** 3h | **Estado:** Pendiente
**Feature:** `cp-tracking-metricas`
**Dependencia:** US-CP-04

**Historia:** Como artista, quiero ver metricas de mis programas de promocion (clicks, conversiones, valor generado) para evaluar el rendimiento de cada promotor.

**Specs:** `docs/product/US-CP-05-tracking-metricas.md`

---

### US-CP-06: Wallet y Comisiones del Promotor
**Prioridad:** Media | **Estimado:** 3h | **Estado:** Pendiente
**Feature:** `cp-wallet-comisiones`
**Dependencia:** US-CP-05

**Historia:** Como promotor, quiero ver mi saldo acumulado, historial de transacciones y solicitar retiros de mis comisiones ganadas.

**Specs:** `docs/product/US-CP-06-wallet-comisiones.md`

---

## Fase 6: Content Licensing (futuro)

> Marketplace donde artistas licencian su contenido (musica, videos, imagenes) a marcas para uso comercial.
> Modulo nuevo: requiere DbContext, migraciones, entidades y endpoints desde cero.
> Analisis completo: `docs/analisis/20260217_nuevos-modulos-content-licensing-sponsorship.md`

### US-CL-01: Foundation del Modulo, Perfil de Marca y Catalogo de Contenido
**Prioridad:** Alta | **Estimado:** 6h | **Estado:** Pendiente
**Feature:** `cl-foundation-perfil-marca-catalogo`

**Historia:** Como administrador, quiero establecer la infraestructura base del modulo Content Licensing (DbContext, migraciones, maestras) y permitir que marcas se registren y artistas publiquen contenido licenciable.

> **BLOQUEANTE:** Incluye tarea previa de verificacion de agentes para migraciones, dominio y contexto nuevo.

**Entidades:** PerfilMarca (compartida), ContenidoLicenciable, TarifaLicencia + 9 tablas maestras
**Specs:** `docs/product/US-CL-01-foundation-perfil-marca-catalogo.md`

---

### US-CL-02: Marketplace - Busqueda y Descubrimiento de Contenido
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** Pendiente
**Feature:** `cl-marketplace-busqueda`
**Dependencia:** US-CL-01

**Historia:** Como marca registrada, quiero buscar y filtrar contenido licenciable por genero, mood, tempo, tipo de uso y presupuesto para encontrar musica para mi campana.

**Entidades:** FavoritoContenido
**Specs:** `docs/product/US-CL-02-marketplace-busqueda.md`

---

### US-CL-03: Solicitud y Negociacion de Licencia
**Prioridad:** Alta | **Estimado:** 4h | **Estado:** Pendiente
**Feature:** `cl-solicitud-negociacion`
**Dependencia:** US-CL-02

**Historia:** Como marca, quiero solicitar una licencia indicando tipo de uso, plataformas, territorio y duracion, y negociar terminos con el artista.

**Entidades:** SolicitudLicencia, MensajeLicencia
**Specs:** `docs/product/US-CL-03-solicitud-negociacion.md`

---

### US-CL-04: Acuerdo y Ejecucion de Licencia
**Prioridad:** Alta | **Estimado:** 4h | **Estado:** Pendiente
**Feature:** `cl-acuerdo-ejecucion`
**Dependencia:** US-CL-03

**Historia:** Como artista o marca, quiero gestionar el ciclo completo del acuerdo: pago, descarga del contenido, certificado de licencia y monitoreo de vencimiento.

**Entidades:** AcuerdoLicencia, PagoLicencia
**Specs:** `docs/product/US-CL-04-acuerdo-ejecucion.md`

---

### US-CL-05: Dashboard y Analytics de Licencias
**Prioridad:** Media | **Estimado:** 2h | **Estado:** Pendiente
**Feature:** `cl-dashboard-analytics`
**Dependencia:** US-CL-04

**Historia:** Como artista, quiero ver metricas de mi catalogo: ingresos, contenido mas popular, solicitudes pendientes y licencias proximas a vencer.

**Specs:** `docs/product/US-CL-05-dashboard-analytics.md`

---

### US-CL-06: Briefs Abiertos y Postulaciones
**Prioridad:** Baja | **Estimado:** 4h | **Estado:** Pendiente
**Feature:** `cl-briefs-postulaciones`
**Dependencia:** US-CL-01

**Historia:** Como marca que no encuentra contenido adecuado, quiero publicar un brief abierto para que artistas postulen con contenido existente o propuestas a medida.

**Entidades:** BriefMarca, PostulacionBrief
**Specs:** `docs/product/US-CL-06-briefs-postulaciones.md`

---

## Fase 7: Sponsorship (futuro)

> Marcas patrocinan campanas, proyectos o artistas a cambio de visibilidad y asociacion de marca.
> Modulo nuevo: requiere DbContext, migraciones, entidades y endpoints desde cero.
> Depende de PerfilMarca creada en Content Licensing (US-CL-01).
> Analisis completo: `docs/analisis/20260217_nuevos-modulos-content-licensing-sponsorship.md`

### US-SP-01: Foundation del Modulo y Oportunidades de Patrocinio
**Prioridad:** Alta | **Estimado:** 6h | **Estado:** Pendiente
**Feature:** `sp-foundation-oportunidades`
**Dependencia:** US-CL-01 (PerfilMarca compartida)

**Historia:** Como administrador, quiero establecer la infraestructura del modulo Sponsorship y permitir que artistas publiquen oportunidades de patrocinio con tiers (Bronce/Plata/Oro) y beneficios.

> **BLOQUEANTE:** Incluye tarea previa de verificacion de agentes para migraciones, dominio y contexto nuevo.

**Entidades:** OportunidadPatrocinio, BeneficioPatrocinio, TierPatrocinio, BeneficioTierPatrocinio + 12 tablas maestras
**Specs:** `docs/product/US-SP-01-foundation-oportunidades.md`

---

### US-SP-02: Marketplace - Descubrimiento y Matching Marca-Artista
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** Pendiente
**Feature:** `sp-marketplace-matching`
**Dependencia:** US-SP-01

**Historia:** Como marca, quiero explorar oportunidades de patrocinio filtrando por genero, audiencia, tipo y presupuesto, con un "Match Score" de alineacion marca-artista.

**Specs:** `docs/product/US-SP-02-marketplace-matching.md`

---

### US-SP-03: Solicitud y Negociacion de Patrocinio
**Prioridad:** Alta | **Estimado:** 4h | **Estado:** Pendiente
**Feature:** `sp-solicitud-negociacion`
**Dependencia:** US-SP-02

**Historia:** Como marca, quiero solicitar patrocinio indicando objetivo, presupuesto, duracion y entregables esperados, y negociar terminos con el artista.

**Entidades:** SolicitudPatrocinio, MensajePatrocinio
**Specs:** `docs/product/US-SP-03-solicitud-negociacion.md`

---

### US-SP-04: Acuerdo con Milestones y Entregables
**Prioridad:** Alta | **Estimado:** 6h | **Estado:** Pendiente
**Feature:** `sp-acuerdo-milestones`
**Dependencia:** US-SP-03

**Historia:** Como artista o marca, quiero gestionar el acuerdo completo: milestones, entregables con prueba de ejecucion, aprobacion y pagos por milestone.

**Entidades:** AcuerdoPatrocinio, MilestonePatrocinio, EntregablePatrocinio, PagoPatrocinio
**Specs:** `docs/product/US-SP-04-acuerdo-milestones-entregables.md`

---

### US-SP-05: Tracking, Metricas y Reporting de Patrocinio
**Prioridad:** Media | **Estimado:** 3h | **Estado:** Pendiente
**Feature:** `sp-tracking-metricas`
**Dependencia:** US-SP-04

**Historia:** Como marca, quiero ver metricas de mis patrocinios (impresiones, clicks, engagement, conversiones) con comparativas entre artistas para evaluar ROI.

**Entidades:** MetricaPatrocinio
**Specs:** `docs/product/US-SP-05-tracking-metricas.md`

---

### US-SP-06: Valoraciones Post-Patrocinio
**Prioridad:** Media | **Estimado:** 2h | **Estado:** Pendiente
**Feature:** `sp-valoraciones`
**Dependencia:** US-SP-04

**Historia:** Como marca o artista que completo un patrocinio, quiero dejar una valoracion mutua bidireccional para construir reputacion en la plataforma.

**Entidades:** ValoracionPatrocinio
**Specs:** `docs/product/US-SP-06-valoraciones.md`

---

## Progreso General

| Fase | Completadas | Total | Estado |
|------|-------------|-------|--------|
| Fase 1: Fundamentos | 7/7 | 7 | ✅ Completada |
| Fase 2: Historias MVP (Crowdfunding) | 5/5 | 5 | ✅ Completada |
| Fase 3: Testing/Deploy | 3/4 | 4 | En progreso |
| Fase 4: Crowdsourcing | 4/6 | 6 | En progreso (2 US pendientes) |
| Fase 5: CrowdPromotion | 4/6 | 6 | En progreso (US-CP-01 a US-CP-04 completadas) |
| Fase 6: Content Licensing | 0/6 | 6 | Pendiente (modulo nuevo) |
| Fase 7: Sponsorship | 0/6 | 6 | Pendiente (modulo nuevo) |

## Mapa de Dependencias entre Modulos

```
Fase 1-3: Fundamentos + MVP CrowdFunding ✅
    |
    v
Fase 4: CrowdSourcing (3/6 completadas)
    |
    v
Fase 5: CrowdPromotion (dominio existe, falta CQRS + frontend)
    |
    v
Fase 6: Content Licensing (modulo nuevo, crea PerfilMarca compartida)
    |
    v
Fase 7: Sponsorship (depende de PerfilMarca de Fase 6)
```

## Siguiente Tarea Recomendada

**WPR-022: Deploy Azure** (3h estimadas, en progreso)
- Azure SQL Database, App Service, Static Web Apps
- CI/CD pipeline + migrations en produccion

En paralelo:
- **WPR-023: Documentacion final** (2h) - README, Swagger, video demo

---

## Notas Tecnicas

### Namespaces del Proyecto
```
WePlayRises
  |-- WePlayRises.WebApi              # Composition root + Auth (Identity minimo)
  |-- WePlayRises.WebApi.Auth         # AuthController, TokenService, ApplicationUser
  |-- WePlayRises.UserAccess.*        # Modulo: Artista, FanProfile
  |-- WePlayRises.Crowdfunding.*      # Modulo: Campania, Reward, Backing
  |-- WePlayRises.BuildingBlocks.*    # Infraestructura compartida
```

### Connection String (desarrollo)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WePlayRises;Trusted_Connection=True;"
}
```

### Comandos utiles
```bash
# Backend
dotnet build WePlayRises.sln
dotnet run --project src/api/WebApi
dotnet ef migrations add <Name> -c WePlayRisesContext

# Frontend
cd src/web
npm install
npm run dev
```
