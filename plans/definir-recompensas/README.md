# Plan: Definir Recompensas (US-03)

> Generado: 2026-02-13
> Feature: definir-recompensas
> Status: Planificado - Listo para implementar

---

## Resumen

Plan completo para implementar la gestion de recompensas (rewards) en campanas de crowdfunding. Permite a artistas crear, editar, eliminar y reordenar recompensas via drag & drop. Los fans ven las recompensas en la vista publica de la campana.

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Types TypeScript, Zod schemas, constants y utils compartidos entre Landing y Admin |

### Backend (7 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs request/response, validaciones FluentValidation, endpoints |
| `backend/hexagonal-architecture.md` | Domain (entidades, constants) + Infrastructure (repos, services, EF config) |
| `backend/cqrs-plan.md` | Commands, Queries, Handlers, Validators - Application layer completa |
| `backend/newman-tests.md` | Estrategia de testing de integracion con Postman/Newman (42 requests) |
| `backend/postman-collection.json` | Coleccion Postman v2.1 ejecutable (23 requests, ~85 assertions) |
| `backend/postman-environment.json` | Variables de entorno para coleccion Postman |
| `backend/QUICK-START.md` | Guia rapida para ejecutar tests Newman |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura: componentes, hooks, services (vista publica read-only) |
| `frontend-landing/ui-design.md` | Componentes shadcn/ui: RewardPublicCard, CampaniaRewardsSection, skeletons |
| `frontend-landing/test-strategy.md` | Tests Vitest + Testing Library (35 test cases) |
| `frontend-landing/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | Arquitectura: CRUD completo, drag & drop, modals, hooks, services |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui: RewardsListPage, RewardCard, RewardFormModal, DeleteDialog |
| `frontend-admin/test-strategy.md` | Tests Vitest + Testing Library (67 test cases) |
| `frontend-admin/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

---

## Resumen por Capa

### Backend - Lo que se implementa

**Nuevo:**
- `ReorderRewardsCommand` + Handler + Validator
- Endpoint `PUT /api/rewards/reorder`
- Constante `BusinessRule_RewardHasBackings = "4010"`
- Metodos en IRewardService/IRewardRepository (reorder, hasBackings, getMaxOrden, validateOwnership)

**Modificar:**
- `CreateRewardCommandHandler` - Ownership validation + auto-increment Orden
- `UpdateRewardCommandHandler` - Ownership + backings restriction
- `DeleteRewardCommandHandler` - Ownership + backings check (error 4010)
- `RewardsController` - Agregar endpoint reorder + [Authorize]
- `ServiceResponseMessageType.cs` - Agregar codigo 4010

### Shared - Lo que se implementa

- Types: `Reward`, `RewardListItem`, `CreateRewardRequest`, `UpdateRewardRequest`, `ReorderRewardsRequest`, `RewardOrder`
- Schemas Zod: `createRewardSchema`, `updateRewardSchema`, `reorderRewardsSchema`
- Constants: `QUERY_KEYS.rewards`, `API_ROUTES.rewards`, `APP_ROUTES.dashboard.rewards`, `TIPO_REWARD`
- Utils: `REWARD_ERROR_MESSAGES`, `getRewardErrorMessage()`, `getRewardSpecificErrorMessage()`

### Landing - Lo que se implementa

- `CampaniaRewardsSection` - Sidebar con rewards en detalle de campana
- `RewardPublicCard` - Card publica con badges (Popular, Pocas unidades, Agotado)
- `RewardPublicCardSkeleton` - Loading state
- `useRewardsByCampania` hook + `reward.service.ts`

### Admin - Lo que se implementa

- `RewardsListPage` - Pagina gestion con stats, lista sortable, empty state
- `RewardCard` - Card sortable con drag handle (@dnd-kit)
- `RewardFormModal` - Modal create/edit con react-hook-form + Zod
- `RewardDeleteDialog` - 2 variantes (sin/con backings)
- 5 hooks: useRewards, useCreateReward, useUpdateReward, useDeleteReward, useReorderRewards
- `reward.service.ts` con CRUD completo + reorder

---

## Criterios de Aceptacion

| ID | Criterio | Backend | Admin | Landing | Shared |
|----|----------|---------|-------|---------|--------|
| AC-03-1 | Advertencia al publicar sin rewards | Validacion | UI warning | - | - |
| AC-03-2 | ImporteMinimo > 0 | Validator | Zod form | - | Schema |
| AC-03-3 | Stock se decrementa al crear backing | Handler | - | - | - |
| AC-03-4 | No seleccionar si stock = 0 | Validacion | - | UI disable | - |
| AC-03-5 | Drag & drop con campo Orden | Reorder cmd | dnd-kit | - | - |
| AC-03-6 | No eliminar si tiene backings | Handler 4010 | Dialog | - | - |
| AC-03-7 | Orden ascendente en vista publica | Query | - | Ordenamiento | - |
| AC-03-8 | Validacion campos (nombre, desc, etc) | Validators | Form | - | Schemas |

---

## Dependencias a Instalar

### Admin
```bash
npm install @dnd-kit/core @dnd-kit/sortable @dnd-kit/utilities
```

### Componentes shadcn (si no existen)
```bash
# Admin
npx shadcn-ui@latest add alert-dialog checkbox
```

---

## Orden de Implementacion Sugerido

```
1. /implement definir-recompensas --target shared     # Types, schemas, constants
2. /implement definir-recompensas --target backend     # Commands, services, controller
3. /implement definir-recompensas --target landing     # Vista publica (read-only)
4. /implement definir-recompensas --target admin       # Dashboard CRUD + drag & drop
```

O todo de una vez:
```
/implement definir-recompensas --target all
```

---

## Notas

- La mayor parte del backend ya existe (entidad, repo, service, CRUD basico). Solo falta ReorderRewardsCommand, ownership validation y backings check.
- Landing es solo lectura (no CRUD). Componentes simples de visualizacion.
- Admin es la parte mas compleja: CRUD completo + drag & drop + formularios con validacion.
- El postman-collection.json es ejecutable directamente con Newman para tests E2E.
