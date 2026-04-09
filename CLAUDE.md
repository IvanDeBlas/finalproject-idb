# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Proyecto

**WePlay Rises** - Plataforma de crowdfunding musical. MVP para curso LIDR (AI4Devs).

---

## Stack Tecnologico

| Capa | Tecnologia |
|------|------------|
| Backend | .NET 8, EF Core, CQRS, MediatR |
| Landing (web) | Vite, React 18, TypeScript, Tailwind CSS, shadcn/ui |
| Dashboard (admin) | Next.js 14, React 18, TypeScript, Tailwind CSS, shadcn/ui |
| DB | SQL Server (LocalDB dev, Azure SQL prod) |
| Auth | JWT + ASP.NET Core Identity |

---

## Credenciales y Entornos

Archivo de credenciales locales: **`.credentials.local.json`** (en `.gitignore`, NO commitear).

### Servicios Docker (`docker compose up -d`)

| Servicio | URL | Notas |
|----------|-----|-------|
| Backend API | http://localhost:5001 | Swagger en /swagger |
| Landing | http://localhost:3000 | Vite + React |
| Admin | http://localhost:3001 | Next.js |
| SQL Server | localhost:1433 | sa / WePlayRises2024! |

### Bases de Datos

| Entorno | Server | Database | Auth |
|---------|--------|----------|------|
| Docker | localhost,1433 | WePlayRises | sa / WePlayRises2024! |
| LocalDB | (localdb)\MSSQLLocalDB | WePlayRises | Windows Auth |
| Azure | uatdic4ap95.database.windows.net | weplay-rises_db | Ver .credentials.local.json |

### JWT (Docker)

- Key: `WePlayRisesDockerSecretKey123456789`
- Issuer/Audience: `WePlayRises` / `WePlayRisesUsers`

### Usuarios de Prueba (Docker DB)

| Email | Password | Roles | Artista | Nota |
|-------|----------|-------|---------|------|
| usuario1@mail.com | 123456 | Fan | Si (Usuario 1 Music Actualizado) | Creado Landing E2E |
| admin-test@mail.com | 123456 | Fan | No | Creado Admin E2E |
| api-test@mail.com | 123456 | Fan | Si (API Test Artist) | Creado via Swagger |
| bug011test@test.com | Test123! | Fan | No | Creado fix BUG-011 |

### Roles Disponibles

`Fan`, `Artista`, `Admin`

---

## Arquitectura Frontend

El frontend esta dividido en **dos aplicaciones** con codigo compartido:

| App | Framework | Puerto | Proposito |
|-----|-----------|--------|-----------|
| `src/web` | Vite + React | 3000 | Landing publica, explorar campanias, backing |
| `src/admin` | Next.js 14 | 3001 | Dashboard artista, CRUD campanias, perfil |
| `src/shared` | TypeScript | - | Tipos, schemas Zod, utils compartidos |

### Comandos de Desarrollo

```bash
# Landing publica
cd src/web && npm run dev     # http://localhost:3000

# Dashboard admin
cd src/admin && npm run dev   # http://localhost:3001
```

---

## Sistema de Tooling Claude Code

Este proyecto usa un sistema avanzado de tooling. Ver `.claude/README.md` para documentacion completa.

### Cargar Contexto Automatico

Las reglas se cargan automaticamente segun el archivo que edites:
- `*.cs` → Reglas backend (CQRS, .NET, EF Core)
- `*.tsx` → Reglas frontend (React, shadcn)
- `*.test.*` → Reglas testing

Archivos de reglas en: `.claude/rules/`

### Memorias Persistentes

Preferencias del proyecto en: `.claude/memories/project.yaml`

Comandos:
- `/remember "preferencia"` - Guardar nueva memoria
- `/memories` - Listar todas
- `/forget mem-XXX` - Eliminar memoria

---

## Comandos Disponibles

| Comando | Descripcion |
|---------|-------------|
| `/done WPR-XXX` | Marcar tarea completada |
| `/health-check` | Validar sistema |
| `/context-mode dev\|review\|research` | Cambiar modo de trabajo |
| `/new-entity Nombre` | Scaffolding entidad completa |
| `/new-endpoint CreateX` | Scaffolding Command + Handler |
| `/remember "texto"` | Guardar preferencia |
| `/memories` | Listar memorias |
| `/forget mem-XXX` | Eliminar memoria |

---

## Patron CQRS - Reglas CRITICAS

> Detalle completo en `.claude/rules/backend/cqrs.rule.md`

### 1. SIEMPRE Retornar ServiceResponse<T>
```csharp
// CORRECTO
public class CreateCampaniaCommand : IRequest<ServiceResponse<Guid>> { }

// INCORRECTO
public class CreateCampaniaCommand : IRequest<Guid> { }  // NO HACER
```

### 2. Handler + Command en MISMO Archivo
```csharp
// CreateCampaniaCommand.cs contiene:
public class CreateCampaniaCommand : IRequest<ServiceResponse<Guid>> { }
public class CreateCampaniaCommandHandler : IRequestHandler<...> { }
```

### 3. Handler NUNCA Inyecta DbContext
```csharp
// CORRECTO - Inyectar Service
private readonly ICampaniaService _service;
private readonly IMapper _mapper;
private readonly IValidator<...> _validator;
private readonly ILogger<...> _logger;  // SIEMPRE incluir

// INCORRECTO
private readonly WePlayRisesDbContext _context;  // NUNCA
```

### 4. Services Retornan Entidades, NO DTOs
```csharp
// Service retorna entidad
var entity = await _service.GetByIdAsync(id, ct);
// Handler hace el mapping
var dto = _mapper.Map<CampaniaDto>(entity);
```

### 5. Validators con .WithMessage() Y .WithErrorCode()
```csharp
RuleFor(x => x.Titulo)
    .NotEmpty()
    .WithMessage("El titulo es obligatorio")
    .WithErrorCode("VALIDATION_REQUIRED");  // AMBOS obligatorios
```

### 6. Try-Catch con Logging en Handlers
```csharp
public async Task<ServiceResponse<Guid>> Handle(...) {
    try {
        // Validacion -> Logica -> ServiceResponse exitoso
    } catch (Exception ex) {
        _logger.LogError(ex, "Error...");
        return new ServiceResponse<Guid> {
            Messages = new() { new() { Message = "Error", ErrorCode = "ERROR_UNEXPECTED" } }
        };
    }
}
```

### 7. AutoMapper Profile por Entidad
```
Modules/{Module}/{Module}.Application/Mapping/{Entity}Profile.cs
```

---

## Estructura del Proyecto

```
WePlay_Rises/
├── .claude/                    # Sistema de tooling
│   ├── rules/                  # Reglas contextuales
│   ├── commands/               # Comandos slash
│   ├── templates/              # Templates de codigo
│   ├── memories/               # Preferencias persistentes
│   └── contexts/               # Modos de trabajo
├── .vscode/                    # Snippets y config VS Code
├── src/
│   ├── api/                    # Backend .NET (Monolito Modular)
│   │   ├── BuildingBlocks/     # Infraestructura compartida
│   │   ├── Modules/
│   │   │   ├── UserAccess/     # Artista, FanProfile
│   │   │   └── Crowdfunding/   # Campania, Reward, Backing
│   │   └── WebApi/             # Composition root + Auth
│   │
│   ├── shared/                 # Codigo compartido frontend
│   │   ├── types/              # TypeScript types (User, Campania, etc.)
│   │   ├── constants/          # QUERY_KEYS, estados, etc.
│   │   ├── schemas/            # Zod validation schemas
│   │   └── utils/              # cn(), formatCurrency(), mappers
│   │
│   ├── web/                    # Landing publica (Vite + React)
│   │   └── src/
│   │       ├── app/            # App, router, providers
│   │       ├── components/     # UI y layout
│   │       ├── features/       # Arquitectura hexagonal
│   │       │   ├── campanias/  # domain/, application/, infrastructure/, presentation/
│   │       │   ├── artistas/
│   │       │   ├── auth/
│   │       │   └── backings/
│   │       ├── hooks/
│   │       ├── store/
│   │       └── lib/
│   │
│   └── admin/                  # Dashboard artista (Next.js 14)
│       └── src/
│           ├── app/            # Next.js App Router
│           │   ├── (auth)/     # Login, Register
│           │   └── (dashboard)/ # Dashboard, Campanias, Perfil
│           ├── components/     # UI, layout, forms
│           ├── hooks/
│           ├── services/
│           ├── store/
│           └── providers/
│
├── tasks/backlog.md            # Tareas (unica fuente de verdad)
├── references/templates/       # Templates de referencia (Krowd, Dashtail)
└── docs/                       # ADRs y user stories
```

---

## Entidades MVP

| Entidad | Modulo | Campos Clave |
|---------|--------|--------------|
| `Artista` | UserAccess | UserId, NombreArtistico, Descripcion |
| `Campania` | Crowdfunding | ArtistaId, Titulo, ImporteObjetivo, Estado |
| `Reward` | Crowdfunding | CampaniaId, Nombre, ImporteMinimo |
| `Backing` | Crowdfunding | CampaniaId, UserId, Monto, RewardId |

---

## Snippets VS Code

| Prefijo | Genera |
|---------|--------|
| `cqrs-cmd` | Command + Handler con ServiceResponse |
| `cqrs-qry` | Query + Handler con ServiceResponse |
| `cqrs-val` | Validator con Message + ErrorCode |
| `rfc` | React Functional Component |
| `uq` | useQuery hooks |
| `um` | useMutation con toast |

---

## Convenciones

### Commits
- Idioma: Ingles
- Formato: `feat:`, `fix:`, `refactor:`, `docs:`, `test:`

### Tests
- Cobertura objetivo: 80%+
- Tiempo ejecucion: < 60 segundos

---

## Restricciones de Tiempo

- **Total disponible**: 30 horas
- **Deadline**: Fin de curso LIDR
- **Enfoque**: MVP funcional, no perfecto

**Priorizar**: Flujo E2E > Tests basicos > Deploy Azure

**Posponer**: Optimizaciones, features secundarias
