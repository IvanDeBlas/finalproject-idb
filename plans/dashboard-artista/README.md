# Plan: Dashboard de Artista (US-05)

> Generado: 2026-02-14
> Feature: dashboard-artista
> Status: Planificado - Listo para implementar

---

## Resumen

Plan completo para implementar el Dashboard de Artista en WePlay Rises.
Incluye backend (4 queries CQRS), frontend admin (3 pantallas), tipos compartidos y tests.

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Contenido |
|---------|-----------|
| `shared/contracts-plan.md` | Tipos TypeScript, schemas Zod, constantes, utils compartidos |

### Backend (5 archivos de plan + extras)

| Archivo | Contenido |
|---------|-----------|
| `backend/api-contracts.md` | DTOs C#, validaciones FluentValidation, error codes |
| `backend/hexagonal-architecture.md` | Domain + Infrastructure: servicios, repositorios, EF Core |
| `backend/cqrs-plan.md` | Application layer: 4 Queries + Handlers + Validators |
| `backend/newman-tests.md` | Estrategia de tests de integracion (28 requests, 35+ casos) |
| `backend/postman-collection.json` | Coleccion Postman ejecutable con Newman |

### Frontend Admin (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-admin/frontend-plan.md` | Arquitectura: pages, components, hooks, services |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui: props, estilos, variantes |
| `frontend-admin/test-strategy.md` | 46 test cases: unit, integration, accessibility |
| `frontend-admin/qa-validation.md` | Validacion de criterios de aceptacion (AC-05-1 a AC-05-12) |

---

## Endpoints Backend

| Endpoint | Metodo | Descripcion |
|----------|--------|-------------|
| `/api/dashboard/resumen` | GET | Metricas agregadas del artista |
| `/api/campanias/mis-campanias` | GET | Lista paginada de campanias |
| `/api/campanias/{id}/backings` | GET | Backings de una campania con stats |
| `/api/campanias/{id}/stats` | GET | Estadisticas detalladas de campania |

Todos requieren: Bearer JWT con rol Artista.

---

## Pantallas Frontend (Admin)

| Pantalla | Ruta | Componentes Clave |
|----------|------|--------------------|
| Dashboard Principal | `/dashboard` | StatsCards, CampaignCard, RecentBackings |
| Detalle Campania | `/dashboard/campanias/[id]` | StatsCards, ProgressBar, BackingTable |
| Lista Backings | `/dashboard/campanias/[id]/backings` | BackingFilters, BackingTable, Pagination |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-05-1 | Artista solo ve sus campanias (filtro backend) | Backend |
| AC-05-2 | Metricas en tiempo real (cache < 1 min) | Backend |
| AC-05-3 | Backings ordenados por FechaCreacion DESC | Backend |
| AC-05-4 | Backings anonimos no exponen nombre/email | Backend + Admin |
| AC-05-5 | Porcentaje con 2 decimales | Backend |
| AC-05-6 | Dias restantes calculados correctamente | Backend + Admin |
| AC-05-7 | Dashboard max 10 campanias con paginacion | Admin |
| AC-05-8 | Auto-refresh cada 30s (polling) | Admin |
| AC-05-9 | BackingTable accesible (keyboard, labels) | Admin |
| AC-05-10 | /api/dashboard/resumen responde < 500ms | Backend |
| AC-05-11 | Sin rol Artista recibe 403 | Backend |
| AC-05-12 | ProgressBar colores segun porcentaje | Admin |

---

## Siguiente Paso

```bash
# Implementar todo
/implement dashboard-artista --target all

# O implementar por partes (orden recomendado):
/implement dashboard-artista --target shared
/implement dashboard-artista --target backend
/implement dashboard-artista --target admin
```

---

## Estimacion

| Target | Horas |
|--------|-------|
| Shared | 1.5h |
| Backend (queries + services + tests) | 10h |
| Frontend Admin (pages + components + tests) | 12h |
| **Total** | **~24h** |
