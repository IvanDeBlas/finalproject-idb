# Plan: cp-programas-promocion (US-CP-02)

> **Feature:** Programas de Promocion - Crowdpromotion
> **Generado:** 2026-02-25
> **User Story:** US-CP-02

---

## Resumen

Plan de implementacion completo para la gestion de Programas de Promocion dentro del modulo Crowdpromotion. Incluye wizard de creacion de 4 pasos, listado paginado, detalle, edicion y desactivacion logica.

**Nota:** Landing no tiene responsabilidad en esta feature (el catalogo publico de programas es parte de US-CP-03).

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | 17 types TypeScript, 6 schemas Zod, constantes (QUERY_KEYS, API_ROUTES, APP_ROUTES, error codes) |

### Backend (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | 5 endpoints, 13 DTOs, 13 error codes nuevos, 2 validators FluentValidation |
| `backend/hexagonal-architecture.md` | Cambios Domain (4 campos PromoPrograma, 5 campos PromoTarea), IPromoProgramaRepository, IPromoProgramaService, migracion EF Core |
| `backend/20260225_cqrs-plan.md` | 5 Commands/Queries con handlers, validators, AutoMapper profiles, controller, orden de implementacion en 7 fases |
| `backend/postman-collection.json` | Coleccion Newman ejecutable: 23 requests, ~90 assertions, ciclo CRUD completo auto-inclusivo |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | 39 archivos: 4 pages, 27 componentes, 4 hooks, 1 service. Wizard con useReducer |
| `frontend-admin/ui-design.md` | 22 componentes shadcn/ui + 12 custom, design tokens, estados de UI, accesibilidad |
| `frontend-admin/test-strategy.md` | 140 tests en 26 archivos. Cobertura objetivo 80%+ |
| `frontend-admin/qa-validation.md` | Informe de cobertura de criterios de aceptacion y flujos alternativos |

### Frontend Landing (0 archivos)

Sin responsabilidad en esta feature. El catalogo publico es parte de US-CP-03.

---

## Criterios de Aceptacion

| ID | Criterio | Cubierto en |
|----|----------|-------------|
| AC-CP02-1 | Wizard 4 pasos, transaccional | cqrs-plan + frontend-plan |
| AC-CP02-2 | EsActivo = true al crear | hexagonal-architecture + cqrs-plan |
| AC-CP02-3 | ArtistaId desde JWT | cqrs-plan (todos los handlers) |
| AC-CP02-4 | Al menos una comision | contracts-plan (Zod) + cqrs-plan (validator) |
| AC-CP02-5 | Listado paginado con filtro | api-contracts + frontend-plan |
| AC-CP02-6 | Editar con aviso promotores | cqrs-plan + frontend-plan |
| AC-CP02-7 | Desactivar cascada | hexagonal-architecture + cqrs-plan |
| AC-CP02-8 | CodigoTracking unico/artista | hexagonal-architecture (indice) + cqrs-plan (validator) |
| AC-CP02-9 | EsRepetible + MaxRepeticiones | contracts-plan (Zod) + cqrs-plan (validator) |
| AC-CP02-10 | Programa sin campana valido | api-contracts + frontend-plan |

---

## Orden de Implementacion Recomendado

```
1. /implement cp-programas-promocion --target shared
   (tipos, schemas Zod, constantes, error messages)

2. /implement cp-programas-promocion --target backend
   (entidades, migracion, repositorio, servicio, CQRS, controller)

3. /implement cp-programas-promocion --target admin
   (pages, componentes, hooks, service, wizard)

4. Tests de integracion con Newman
   newman run plans/cp-programas-promocion/backend/postman-collection.json
```

---

## Siguiente Paso

```bash
/implement cp-programas-promocion --target all
```

O implementar por partes:
```bash
/implement cp-programas-promocion --target shared
/implement cp-programas-promocion --target backend
/implement cp-programas-promocion --target admin
```
