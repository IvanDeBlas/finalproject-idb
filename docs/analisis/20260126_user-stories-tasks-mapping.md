# Mapeo User Stories → Tareas → Proyectos

> - **Fecha:** 2026-01-26
> - **Estado:** Borrador - En iteración
> - **Objetivo:** Definir la relación entre historias de usuario, tareas técnicas y proyectos de código

---

## Contexto

El proyecto WePlay Rises tiene **3 proyectos de código**:

| Proyecto | Ruta | Framework | Puerto | Propósito |
|----------|------|-----------|--------|-----------|
| **Backend** | `src/api/` | .NET 8 + EF Core | 5000 | API REST, Identity, CQRS |
| **Landing** | `src/web/` | Vite + React | 5173 | Landing pública, explorar campañas, backing |
| **Admin** | `src/admin/` | Next.js 14 | 3000 | Dashboard artista, CRUD campañas, perfil |

---

## User Stories MVP

| ID | Nombre | Actor Principal | Flujo Principal |
|----|--------|-----------------|-----------------|
| US-01 | Registro de Artista | Artista | Register → Login → Crear perfil |
| US-02 | Crear Campaña | Artista | Dashboard → Wizard → Publicar |
| US-03 | Definir Recompensas | Artista | Editar campaña → Añadir rewards |
| US-04 | Hacer Backing | Fan | Explorar → Detalle → Backing |
| US-05 | Dashboard Artista | Artista | Ver métricas, backers, progreso |

---

## Mapeo User Stories → Tareas WPR

### US-01: Registro de Artista

| Capa | Tarea | Descripción |
|------|-------|-------------|
| Backend | WPR-006 | Identity: `POST /api/auth/register`, `POST /api/auth/login` |
| Backend | WPR-010 | Artista API: `POST /api/artistas`, `GET /api/artistas/me` |
| Landing | WPR-010 | Form de registro (`/auth/register`) |
| Admin | WPR-010 | Form de perfil artista (`/perfil`) |

### US-02: Crear Campaña

| Capa | Tarea | Descripción |
|------|-------|-------------|
| Backend | WPR-011 | Campaña CRUD: `POST/GET/PUT /api/campanias`, `POST /api/campanias/{id}/publicar` |
| Admin | WPR-011 | Wizard de creación (`/dashboard/campanias/nueva`) |

### US-03: Definir Recompensas

| Capa | Tarea | Descripción |
|------|-------|-------------|
| Backend | WPR-012 | Rewards API: `POST/GET/PUT/DELETE /api/campanias/{id}/rewards` |
| Admin | WPR-012 | Gestión de rewards en wizard/edición |

### US-04: Hacer Backing

| Capa | Tarea | Descripción |
|------|-------|-------------|
| Backend | WPR-013 | Backing API: `GET /api/campanias`, `POST /api/campanias/{id}/backings` |
| Landing | WPR-013 | Listado campañas (`/campanias`), Detalle (`/campanias/{id}`), Form backing |

### US-05: Dashboard Artista

| Capa | Tarea | Descripción |
|------|-------|-------------|
| Backend | WPR-014 | Stats API: `GET /api/campanias/mis-campanias`, `GET /api/campanias/{id}/backings` |
| Admin | WPR-014 | Dashboard con métricas (`/dashboard`) |

---

## Matriz Proyecto × User Story

|  | US-01 | US-02 | US-03 | US-04 | US-05 |
|--|-------|-------|-------|-------|-------|
| **Backend (.NET)** | ✓ | ✓ | ✓ | ✓ | ✓ |
| **Landing (Vite)** | ✓ | - | - | ✓ | - |
| **Admin (Next.js)** | ✓ | ✓ | ✓ | - | ✓ |

---

## Tareas Fundacionales (Fase 1)

Estas tareas son pre-requisitos para las User Stories:

| Tarea | Proyecto | Descripción | Estado |
|-------|----------|-------------|--------|
| WPR-001 | Backend | Entidades Domain (Artista, Campaña, Reward, Backing) | Pendiente |
| WPR-002 | Backend | BuildingBlocks copiados de miUrba | ✅ Completada |
| WPR-003 | Backend | Estructura módulos (UserAccess, Crowdfunding) | ✅ Completada |
| WPR-004 | Backend | DbContext + Migrations | Pendiente |
| WPR-005 | Frontend | Setup Vite (web) + Next.js (admin) + shadcn | Pendiente |
| WPR-006 | Backend | Identity + JWT Authentication | Pendiente |

---

## Dependencias entre Tareas

```
Fase 1 (Fundamentos):
WPR-002 ✓ ──┐
WPR-003 ✓ ──┼──> WPR-001 ──> WPR-004 ──> WPR-006
            │
WPR-005 ────┘

Fase 2 (User Stories):
WPR-006 ──> WPR-010 (US-01) ──> WPR-011 (US-02) ──> WPR-012 (US-03)
                │
                └──> WPR-013 (US-04)
                │
                └──> WPR-014 (US-05)
```

---

## Pendiente: Comandos y Agentes por Proyecto

> **TODO:** Definir los comandos slash y agentes que interactúan con cada proyecto.

### Backend (.NET)
- Comandos: `/new-entity`, `/new-endpoint`
- Agentes: ?
- Templates: `.claude/templates/api/`

### Landing (Vite + React)
- Comandos: ?
- Agentes: ?
- Templates: `.claude/templates/react/`

### Admin (Next.js)
- Comandos: ?
- Agentes: ?
- Templates: `.claude/templates/react/`

---

## Próximas Iteraciones

1. [ ] Definir comandos slash específicos por proyecto
2. [ ] Definir agentes especializados (backend-agent, frontend-agent)
3. [ ] Vincular tareas WPR con templates específicos
4. [ ] Crear sistema de ejecución autónoma de tareas

---

## Referencias

- [User Stories MVP](../specs/user-stories-mvp.md)
- [Backlog de Tareas](../../tasks/README.md)
- [CLAUDE.md](../../CLAUDE.md) - Arquitectura Frontend

---

*Última actualización: 2026-01-26*
