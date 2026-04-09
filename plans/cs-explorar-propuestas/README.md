# Plan: cs-explorar-propuestas (US-CS-03)

> **Feature:** Explorar Necesidades y Enviar Propuestas
> **Generado:** 2026-02-17
> **Total archivos:** 9 planes (~10,000 lineas)

---

## Resumen

Esta feature habilita el lado del profesional en crowdsourcing: explorar necesidades abiertas, ver detalle, enviar propuestas, hacer seguimiento y retirar propuestas. Incluye 5 endpoints API, 5 pantallas en Landing, y tipos/schemas compartidos.

**Admin NO involucrado** (el artista ve propuestas desde US-CS-02).

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `shared/contracts-plan.md` | 666 | 8 types TS, 2 schemas Zod, constantes (ESTADO_PROPUESTA, QUERY_KEYS, API_ROUTES, APP_ROUTES), error messages |

### Backend (4 archivos)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `backend/api-contracts.md` | 933 | DTOs C#, Controllers, Validators FluentValidation, AutoMapper Profiles, Swagger annotations |
| `backend/hexagonal-architecture.md` | 651 | Domain (entidad PropuestaCrowdsourcing, interfaces repo/service, constants), Infrastructure (EF Core config, repositorio, servicio con cache, migracion, seeds) |
| `backend/cqrs-plan.md` | 1,353 | 2 Commands (CreatePropuesta, RetirarPropuesta) + 3 Queries (GetNecesidadesPublicas, GetNecesidadPublicaById, GetMisPropuestas) con Handlers, Validators, caching strategy |
| `backend/postman-collection.json` | 1,601 | Coleccion Newman con CRUD lifecycle: setup, explorar, detalle, enviar, validaciones, mis propuestas, retirar |

### Frontend Landing (4 archivos)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `frontend-landing/frontend-plan.md` | 1,132 | Arquitectura feature-based, componentes, hooks, services, routing, state management, orden de implementacion |
| `frontend-landing/ui-design.md` | 1,316 | Componentes shadcn/ui, composicion, customizaciones dark theme, design tokens, patrones de cards/forms/filtros |
| `frontend-landing/test-strategy.md` | 2,008 | Tests unitarios + integracion con Vitest + Testing Library, fixtures, mocking strategy, cobertura 80%+ |
| `frontend-landing/qa-validation.md` | 349 | Matriz de cobertura 13 ACs + 9 FAs + 9 NFRs, gaps identificados, recomendaciones |

---

## Endpoints API

| # | Metodo | Endpoint | Descripcion |
|---|--------|----------|-------------|
| 1 | GET | `/api/crowdsourcing/necesidades` | Listado publico paginado con filtros |
| 2 | GET | `/api/crowdsourcing/necesidades/{id}` | Detalle con campos calculados |
| 3 | POST | `/api/crowdsourcing/necesidades/{necesidadId}/propuestas` | Enviar propuesta |
| 4 | GET | `/api/crowdsourcing/propuestas/mis-propuestas` | Mis propuestas |
| 5 | PATCH | `/api/crowdsourcing/propuestas/{id}/retirar` | Retirar propuesta |

---

## Pantallas Landing

| # | Ruta | Pantalla |
|---|------|----------|
| 1 | `/crowdsourcing/necesidades` | Explorar necesidades (listado + filtros) |
| 2 | `/crowdsourcing/necesidades/:id` | Detalle necesidad + CTA dinamico |
| 3 | Modal sobre detalle | Enviar propuesta (formulario) |
| 4 | `/crowdsourcing/mis-propuestas` | Mis propuestas (listado + badges) |
| 5 | Dialog sobre mis propuestas | Confirmar retirar propuesta |

---

## CQRS Operations

| Tipo | Operacion | ServiceResponse |
|------|-----------|-----------------|
| Command | CreatePropuestaCommand | ServiceResponse\<PropuestaCreatedResultDto\> |
| Command | RetirarPropuestaCommand | ServiceResponse\<RetirarPropuestaResultDto\> |
| Query | GetNecesidadesPublicasQuery | ServiceResponse\<PaginatedResult\<NecesidadPublicaListDto\>\> |
| Query | GetNecesidadPublicaByIdQuery | ServiceResponse\<NecesidadPublicaDto\> |
| Query | GetMisPropuestasQuery | ServiceResponse\<PaginatedResult\<MiPropuestaListDto\>\> |

---

## Cobertura QA

- **13/13 Criterios de Aceptacion** cubiertos
- **9/9 Flujos Alternativos** cubiertos
- **Score global:** ~84% (algunos items parciales pendientes de implementacion frontend)

---

## Siguiente Paso

```bash
# Implementar todo de una vez
/implement cs-explorar-propuestas --target all

# O implementar por partes (orden recomendado)
/implement cs-explorar-propuestas --target shared
/implement cs-explorar-propuestas --target backend
/implement cs-explorar-propuestas --target landing
```
