# WePlay Rises - Backlog de Tareas

> **Generado:** 2026-01-21
> **Tiempo total disponible:** 30 horas
> **Enfoque MVP:** Crowdfunding E2E

---

## Fase 1: Fundamentos (8h estimadas)

### WPR-001: Definir modelo de datos MVP en Code-First
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** Pendiente

**Descripcion:**
Crear entidades EF Core simplificadas basadas en los scripts SQL existentes.

**Tareas:**
- [ ] Crear `WePlayRises.Domain` project
- [ ] Definir entidad `Artista` (simplificada)
- [ ] Definir entidad `Campania` (simplificada)
- [ ] Definir entidad `Reward`
- [ ] Definir entidad `Backing`
- [ ] Definir enums `EstadoCampania`, `TipoFinanciacion`
- [ ] Configurar EF Core Configurations (Fluent API)

**Referencia:** `C:\Repos\MisCosas\...\Claude II\1_Crowd_Maestros_Artistas.sql`

**Criterios de aceptacion:**
- Entidades tienen propiedades minimas para MVP
- Configuraciones EF Core completas
- Migrations generables sin errores

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
**Prioridad:** Alta | **Estimado:** 1h | **Estado:** Pendiente

**Descripcion:**
Configurar WePlayRisesContext con todas las entidades MVP.

**Tareas:**
- [ ] Crear `WePlayRisesContext : DbContext`
- [ ] Registrar entidades (Artista, Campania, Reward, Backing)
- [ ] Configurar Fluent API para relaciones
- [ ] Crear migration inicial: `dotnet ef migrations add InitialCreate`
- [ ] Verificar script SQL generado
- [ ] Seed de datos maestros (EstadoCampania, TipoFinanciacion)

**Criterios de aceptacion:**
- Migration genera SQL correcto
- Seed de maestras funciona
- Database creada con `dotnet ef database update`

---

### WPR-005: Setup proyecto React + Templates
**Prioridad:** Alta | **Estimado:** 1h | **Estado:** Pendiente

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
**Prioridad:** Alta | **Estimado:** 1.5h | **Estado:** Pendiente

**Descripcion:**
Implementar autenticacion minima en WebApi (NO como modulo separado). Solo lo esencial para MVP.

**Arquitectura:**
```
src/api/WebApi/
  |-- Auth/
  |     |-- AuthController.cs      # Register, Login endpoints
  |     |-- TokenService.cs        # Genera JWT
  |     |-- ApplicationUser.cs     # IdentityUser<Guid>
  |     |-- JwtSettings.cs         # Config POCO
  |-- Program.cs                   # Configura Identity + JWT middleware
```

**Tareas:**
- [ ] Crear `ApplicationUser : IdentityUser<Guid>` con NombreCompleto, FechaRegistro
- [ ] Crear `JwtSettings` POCO para configuracion
- [ ] Crear `ITokenService` + `TokenService` para generar JWT
- [ ] Crear `AuthController` con:
  - [ ] `POST /api/auth/register` - Registro con email/password
  - [ ] `POST /api/auth/login` - Login, retorna JWT
- [ ] Configurar Identity en Program.cs (AddIdentity, AddEntityFrameworkStores)
- [ ] Configurar JWT Bearer en Program.cs (AddAuthentication, AddJwtBearer)
- [ ] Agregar config en appsettings.json (Jwt:Key, Issuer, Audience, ExpirationMinutes)
- [ ] Crear migration para tablas de Identity

**Relacion con UserAccess:**
- `ApplicationUser.Id` (Guid) es el UserId que referencia `Artista.UserId`
- Identity maneja auth, UserAccess maneja perfiles de dominio

**Referencia:** Ver ADR-004 para codigo de ejemplo.

**Criterios de aceptacion:**
- Usuario puede registrarse con email/password
- Usuario puede hacer login y recibir JWT valido
- Endpoints protegidos con [Authorize] funcionan
- JWT incluye claims: UserId, Email, Roles

---

### WPR-006a: Configurar User Secrets para desarrollo
**Prioridad:** Alta | **Estimado:** 15min | **Estado:** Pendiente
**Dependencia:** WPR-003 (Estructura de modulos)
**Bloquea:** WPR-006

**Descripcion:**
Configurar .NET User Secrets para almacenar secretos de desarrollo fuera del repositorio. Esto evita exponer JWT Keys y otras credenciales en archivos versionados.

**Tareas:**
- [ ] Ejecutar `dotnet user-secrets init` en WebApi
- [ ] Configurar `Jwt:Key` en User Secrets: `dotnet user-secrets set "Jwt:Key" "DevKey..."`
- [ ] Limpiar secretos hardcodeados de `appsettings.Development.json`
- [ ] Ejecutar `git rm --cached` si el archivo tiene secretos versionados
- [ ] Documentar proceso de setup en README para nuevos desarrolladores

**Referencia:** `docs/analisis/20260127_implementacion-secretos-identity.md` (Seccion 1)

**Criterios de aceptacion:**
- User Secrets inicializado (UserSecretsId en .csproj)
- `Jwt:Key` almacenado en User Secrets, no en archivos
- `appsettings.Development.json` sin secretos sensibles
- Instrucciones de onboarding documentadas

---

## Fase 2: Historias de Usuario MVP (12h estimadas)

### WPR-010: US-01 - Registro de Artista
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** Pendiente
**Dependencia:** WPR-006 (Identity minimo)

**Historia:** Como artista, quiero registrarme y crear mi perfil para presentar mi proyecto musical.

**Backend:**
- [ ] `POST /api/artistas` - Crear perfil artista (usa UserId del JWT)
- [ ] `GET /api/artistas/me` - Mi perfil
- [ ] `PUT /api/artistas/{id}` - Actualizar perfil

**Nota:** El registro de usuario (`POST /api/auth/register`) ya esta en WPR-006.

**Frontend:**
- [ ] Pagina `/auth/register` con formulario
- [ ] Pagina `/artista/perfil` con formulario de perfil
- [ ] Subida de imagen (puede ser URL por ahora)

**Criterios de aceptacion:**
- [ ] Usuario puede registrarse con email/password
- [ ] Usuario puede crear perfil de artista
- [ ] Perfil muestra nombre artistico, bio, imagen

---

### WPR-011: US-02 - Crear Campania
**Prioridad:** Alta | **Estimado:** 3h | **Estado:** Pendiente

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
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** Pendiente

**Historia:** Como artista, quiero anadir recompensas a mi campania para incentivar las aportaciones.

**Backend:**
- [ ] `POST /api/campanias/{id}/rewards` - Crear reward
- [ ] `GET /api/campanias/{id}/rewards` - Listar rewards
- [ ] `PUT /api/rewards/{id}` - Actualizar reward
- [ ] `DELETE /api/rewards/{id}` - Eliminar reward

**Frontend:**
- [ ] Seccion en wizard de campania para rewards
- [ ] Form: nombre, descripcion, monto minimo, stock
- [ ] Lista de rewards con edicion inline

**Criterios de aceptacion:**
- [ ] Se pueden crear multiples rewards
- [ ] Cada reward tiene monto minimo
- [ ] Stock limitado es opcional

---

### WPR-013: US-04 - Hacer Backing
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** Pendiente

**Historia:** Como fan, quiero apoyar una campania para ayudar al artista y obtener recompensas.

**Backend:**
- [ ] `GET /api/campanias` - Listar campanias publicas
- [ ] `POST /api/campanias/{id}/backings` - Crear backing
- [ ] Actualizar `ImportePledgedActual` en campania
- [ ] Decrementar stock de reward si limitado

**Frontend:**
- [ ] Pagina `/campanias` con listado
- [ ] Pagina `/campanias/{id}` con detalle
- [ ] Barra de progreso de meta
- [ ] Seleccion de reward y monto
- [ ] Formulario de backing (sin pago real)

**Criterios de aceptacion:**
- [ ] Campanias activas visibles publicamente
- [ ] Progreso se actualiza al hacer backing
- [ ] Backing queda registrado

---

### WPR-014: US-05 - Dashboard Artista
**Prioridad:** Alta | **Estimado:** 2h | **Estado:** Pendiente

**Historia:** Como artista, quiero ver el progreso de mi campania para conocer las metricas.

**Backend:**
- [ ] `GET /api/campanias/mis-campanias` - Campanias del artista
- [ ] `GET /api/campanias/{id}/backings` - Lista de backers
- [ ] `GET /api/campanias/{id}/stats` - Estadisticas (opcional)

**Frontend:**
- [ ] Pagina `/dashboard` con resumen
- [ ] Cards con total recaudado, % meta, num backers
- [ ] Lista de backings recientes
- [ ] Grafico de progreso (opcional)

**Criterios de aceptacion:**
- [ ] Dashboard muestra metricas clave
- [ ] Lista de backers visible
- [ ] Porcentaje de meta calculado

---

## Fase 3: Testing y Deploy (10h estimadas)

### WPR-020: Tests unitarios backend
**Prioridad:** Media | **Estimado:** 3h | **Estado:** Pendiente

**Tareas:**
- [ ] Tests de Handlers CQRS
- [ ] Tests de Validators
- [ ] Tests de Services
- [ ] Cobertura objetivo: ≥80%

---

### WPR-021: Tests de integracion
**Prioridad:** Media | **Estimado:** 2h | **Estado:** Pendiente

**Tareas:**
- [ ] Coleccion Postman para API
- [ ] Tests end-to-end flujo completo
- [ ] Newman para CI/CD

---

### WPR-022: Deploy Azure
**Prioridad:** Media | **Estimado:** 3h | **Estado:** Pendiente

**Tareas:**
- [ ] Crear Azure SQL Database
- [ ] Crear Azure App Service para API
- [ ] Crear Azure Static Web Apps para frontend
- [ ] Configurar CI/CD pipeline
- [ ] Ejecutar migrations en produccion

---

### WPR-023: Documentacion final
**Prioridad:** Media | **Estimado:** 2h | **Estado:** Pendiente

**Tareas:**
- [ ] Completar README.md con screenshots
- [ ] Documentar API en Swagger
- [ ] Grabar video demo (requisito LIDR)
- [ ] Actualizar prompts.md con prompts usados

---

## Prioridades para Esta Semana

| Dia | Tareas Sugeridas | Horas |
|-----|------------------|-------|
| Martes | WPR-001, WPR-002 | 4h |
| Miercoles | WPR-003, WPR-004 | 3h |
| Jueves | WPR-005, WPR-006a, WPR-006 | 2.75h |
| Viernes | WPR-010, WPR-011 | 5h |
| Sabado | WPR-012, WPR-013, WPR-014 | 6h |

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
