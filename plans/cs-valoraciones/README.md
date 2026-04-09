# Plan: cs-valoraciones (US-CS-06)

> **Feature:** Valoraciones Bidireccionales
> **Generado:** 2026-02-21
> **Status:** Listo para implementar

---

## Resumen

Sistema de valoraciones mutuas entre artistas y profesionales al completar un acuerdo de trabajo. Puntuacion 1-5 estrellas con comentario opcional. Valoraciones inmutables, visibles publicamente en perfiles.

**Scope:** Backend + Shared + Landing (Admin NO involucrado)

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Contenido |
|---------|-----------|
| `shared/contracts-plan.md` | Types TS, schemas Zod, QUERY_KEYS, API_ROUTES, error messages |

### Backend (4 archivos principales)

| Archivo | Contenido |
|---------|-----------|
| `backend/hexagonal-architecture.md` | Entidad ValoracionCrowdsourcing, EF Config, Repository, Service, migracion |
| `backend/api-contracts.md` | DTOs C#, Validator, AutoMapper Profile, Controller, constantes |
| `backend/cqrs-plan.md` | CreateValoracionCommand + Handler, GetValoracionesByUserQuery + Handler, Validator |
| `backend/postman-collection.json` | Coleccion Newman ejecutable (22 requests, 65+ assertions) |

### Frontend Landing (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-landing/frontend-plan.md` | Arquitectura de componentes, hooks, services, integraciones |
| `frontend-landing/ui-design.md` | Componentes shadcn/ui, clases Tailwind exactas, variantes, estados |
| `frontend-landing/test-strategy.md` | Estrategia de testing Vitest + Testing Library |
| `frontend-landing/qa-validation.md` | Matriz de trazabilidad AC vs planes, gaps identificados |

### Documentacion Newman (complementaria)

| Archivo | Contenido |
|---------|-----------|
| `backend/QUICK_START.md` | Guia rapida de ejecucion Newman |
| `backend/ENDPOINTS_IMPLEMENTATION_CHECKLIST.md` | Specs detalladas por endpoint |
| `backend/run-tests.sh` | Script ejecutable para correr tests |

---

## Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | `/api/crowdsourcing/acuerdos/{acuerdoId}/valoraciones` | Crear valoracion (participante del acuerdo) |
| GET | `/api/crowdsourcing/usuarios/{userId}/valoraciones` | Obtener valoraciones de un usuario (paginado + resumen) |

---

## Componentes Frontend (9 custom)

| Componente | Tipo | Descripcion |
|------------|------|-------------|
| `StarRating` | Input | Selector interactivo 1-5 con hover effect |
| `StarDisplay` | Display | Read-only con medias estrellas |
| `RatingHistogram` | Display | Barras de distribucion por nivel |
| `ValoracionForm` | Form | Card con formulario completo |
| `ValoracionReadOnly` | Display | Card post-envio inmutable |
| `ValoracionesSection` | Section | Seccion completa en perfil (resumen + lista) |
| `ValoracionListItem` | Card | Item individual de valoracion |
| `RatingBadge` | Badge | Puntuacion compacta para cards |
| `EmptyValoraciones` | Empty | Estado vacio |

---

## Dependencias

- **Prerequisito:** US-CS-04 (Acuerdos y Entregables) debe estar implementado
- **Entidad existente:** AcuerdoCrowdsourcing con estado Completado
- **Constantes nuevas:** `Validation_InvalidRange`, `BusinessRule_DuplicateAction`

---

## Orden de Implementacion Sugerido

```
1. /implement cs-valoraciones --target shared     (types, schemas, constants)
2. /implement cs-valoraciones --target backend     (entity, repo, service, CQRS, controller, migration)
3. /implement cs-valoraciones --target landing     (componentes, hooks, services, integraciones)
4. newman run plans/cs-valoraciones/backend/postman-collection.json  (validar backend)
```

---

## Siguiente Paso

```bash
/implement cs-valoraciones --target all
```

O implementar por partes:
```bash
/implement cs-valoraciones --target shared
/implement cs-valoraciones --target backend
/implement cs-valoraciones --target landing
```
