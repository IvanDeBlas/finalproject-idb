# Plan: cp-tareas-promocion (US-CP-04)

> **Feature:** Ejecucion de Tareas de Promocion y Validacion
> **Generado:** 2026-03-01
> **Agentes ejecutados:** 13

---

## Resumen

Esta feature implementa el ciclo central de trabajo del modulo Crowdpromotion: el promotor completa tareas de difusion y el artista valida el trabajo acreditando recompensas en la wallet.

**5 endpoints API** | **14 criterios de aceptacion** | **5 pantallas UI** | **93 tests planificados**

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Contenido |
|---------|-----------|
| `shared/contracts-plan.md` | 12 types, 3 Zod schemas, 5 endpoints, 2 query keys, 7 error codes, 2 mappers |

### Backend (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `backend/api-contracts.md` | 11 DTOs C#, 3 validators, 1 AutoMapper profile, 1 controller, 7 ServiceResponseMessageType constants |
| `backend/hexagonal-architecture.md` | 2 repository interfaces, 1 service (IPromoTareaService - 11 metodos), wallet integration transaccional |
| `backend/cqrs-plan.md` | 2 queries + 3 commands (handlers en mismo archivo), 3 validators, 1 profile, 1 controller |
| `backend/postman-collection.json` | 24 requests, 85+ assertions, 5 folders (Setup, CRUD, Validation, Auth, NotFound) |

### Frontend Landing (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-landing/frontend-plan.md` | 5 componentes, 2 hooks, 1 service, 1 router update |
| `frontend-landing/ui-design.md` | 11 composiciones shadcn/ui, paleta pink/purple, responsive mobile-first |
| `frontend-landing/test-strategy.md` | 45 tests (6 archivos): services, hooks, TareaCard, Dialog, Page |
| `frontend-landing/qa-validation.md` | Validacion de cobertura de criterios AC para Landing |

### Frontend Admin (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-admin/frontend-plan.md` | 5 componentes (1 modificado), 3 hooks, 1 service |
| `frontend-admin/ui-design.md` | 4 composiciones shadcn/ui, dark theme, tabs integration |
| `frontend-admin/test-strategy.md` | 48 tests (8 archivos): services, hooks, cards, dialogs, tab |
| `frontend-admin/qa-validation.md` | Validacion de cobertura de criterios AC para Admin (83%) |

---

## Decisiones Arquitectonicas Clave

1. **VecesCompletada es calculado** - COUNT/MIN/MAX en query, no campo en entidad
2. **Un registro por intento** - PromoTareaPromotor = 1 completion attempt
3. **Re-envio crea nuevo registro** - El rechazado queda intacto para auditoria
4. **Wallet transaccional** - Validar tarea + acreditar wallet en misma transaccion DB
5. **Handler no gestiona transaccion** - Delegada al Service (patron del proyecto)
6. **7 nuevos error codes** - 2021-2022 (NotFound), 4027-4031 (Business Rules)

---

## Orden de Implementacion Recomendado

```
1. Shared          → types, schemas, constants, utils
2. Backend Domain  → ServiceResponseMessageType constants
3. Backend Infra   → Repository + Service implementations
4. Backend App     → DTOs, Validators, Handlers, Profile, Controller
5. Landing         → Service → Hooks → Components → Page
6. Admin           → Service → Hooks → Components → Tab integration
7. Tests           → Unit + Integration (Landing 45 + Admin 48)
8. Newman          → Integration tests con Postman collection
```

---

## Siguiente Paso

```
/implement cp-tareas-promocion --target all
```

O implementar por partes:
```
/implement cp-tareas-promocion --target shared
/implement cp-tareas-promocion --target backend
/implement cp-tareas-promocion --target landing
/implement cp-tareas-promocion --target admin
```
