# Plan: cp-inscripcion-programa (US-CP-03)

> **Feature:** Inscripcion en Programa de Crowdpromotion
> **Generado:** 2026-02-25
> **Agentes ejecutados:** 13
> **Status:** Completo

---

## Resumen

Esta feature implementa el ciclo completo de inscripcion de promotores en programas de promocion:
- **Promotor** (Landing): explorar programas, solicitar inscripcion, ver mis programas con codigo referido
- **Artista** (Admin): aprobar, rechazar, bloquear solicitudes y dar de baja promotores

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Contenido |
|---------|-----------|
| `shared/contracts-plan.md` | 10 interfaces, 1 union type, 2 schemas Zod, constantes (QUERY_KEYS, API_ROUTES, APP_ROUTES, INSCRIPCION_ESTADO), 7 error codes, 2 mappers |

### Backend (4 archivos de plan + 7 archivos de documentacion Newman)

| Archivo | Contenido |
|---------|-----------|
| `backend/hexagonal-architecture.md` | Domain: 2 campos nuevos en entidad, 6 constantes ServiceResponseMessageType, repositorio dedicado. Infra: migracion EF Core, servicio con generacion de CodigoReferido |
| `backend/api-contracts.md` | 8 endpoints documentados, 12 DTOs, IInscripcionService como servicio nuevo |
| `backend/cqrs-plan.md` | 5 Commands, 3 Queries, 5 Validators, 12 DTOs, 1 AutoMapper Profile |
| `backend/postman-collection.json` | 18 requests, ~65 assertions, 8/8 endpoints cubiertos, 6 carpetas |

### Frontend Landing (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-landing/frontend-plan.md` | 12 componentes, 3 hooks, 1 service, 2 rutas nuevas |
| `frontend-landing/ui-design.md` | 11 componentes shadcn, 14 composiciones custom, accesibilidad WCAG AA |
| `frontend-landing/test-strategy.md` | 50 tests (28 unit + 22 integration), cobertura 80%+ |
| `frontend-landing/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

### Frontend Admin (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-admin/frontend-plan.md` | 11 componentes, 2 hooks, 1 service, extiende tabs existentes |
| `frontend-admin/ui-design.md` | 9 componentes shadcn, 3 AlertDialogs, badge contador en tab |
| `frontend-admin/test-strategy.md` | 98 tests (67 unit + 31 integration), cobertura 80%+ |
| `frontend-admin/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

---

## Metricas Globales

| Metrica | Valor |
|---------|-------|
| Endpoints backend | 8 |
| Commands CQRS | 5 |
| Queries CQRS | 3 |
| Componentes Landing | 12 |
| Componentes Admin | 11 |
| Tests Landing planificados | 50 |
| Tests Admin planificados | 98 |
| Tests Newman (assertions) | ~65 |
| **Total tests planificados** | **~213** |

---

## Orden de Implementacion Recomendado

```
1. /implement cp-inscripcion-programa --target shared
   → Types, schemas Zod, constantes en src/shared/

2. /implement cp-inscripcion-programa --target backend
   → Migracion EF Core (campos nuevos)
   → Domain: constantes, interfaces
   → Infra: repositorio, servicio, generacion de codigos
   → Application: Commands, Queries, Validators, DTOs, Profile
   → WebApi: Controller

3. /implement cp-inscripcion-programa --target landing
   → Service, hooks, componentes, rutas
   → Tests unitarios e integracion

4. /implement cp-inscripcion-programa --target admin
   → Service, hooks, componentes (extiende tabs existentes)
   → Tests unitarios e integracion
```

---

## Dependencias

- **US-CP-01** (cp-perfil-promotor): Debe estar implementada (entidad Promotor + EsActivo)
- **US-CP-02** (cp-programas-promocion): Debe estar implementada (PromoPrograma + CodigoTrackingBase + UrlLanding)

---

## Siguiente Paso

```bash
/implement cp-inscripcion-programa --target all
```

O implementar por partes:
```bash
/implement cp-inscripcion-programa --target shared
/implement cp-inscripcion-programa --target backend
/implement cp-inscripcion-programa --target landing
/implement cp-inscripcion-programa --target admin
```
