# Plan: cs-templates-guia (US-CS-01)

> **Feature:** Templates y Guia para Artistas Noveles
> **Generado:** 2026-02-15
> **Agentes ejecutados:** 13
> **Archivos generados:** 16

---

## Resumen de la Feature

Wizard de 3 pasos para que artistas noveles seleccionen un tipo de proyecto musical (ej: "Grabar un Album"), personalicen necesidades profesionales con presupuestos, y generen automaticamente necesidades de crowdsourcing vinculadas a su proyecto artistico.

- 6 templates predefinidos (seed data)
- ~55 necesidades profesionales
- ~35 roles profesionales en 6 categorias
- 5 endpoints API (3 templates + 2 maestras)

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Contenido |
|---------|-----------|
| `shared/contracts-plan.md` | 11 interfaces TypeScript, 2 schemas Zod, constantes (QUERY_KEYS, API_ROUTES, etc.) |

### Backend (6 archivos)

| Archivo | Contenido |
|---------|-----------|
| `backend/api-contracts.md` | 8 DTOs, 2 validators, 3 AutoMapper profiles, 2 controllers, error codes |
| `backend/hexagonal-architecture.md` | 5 entidades, 4 repositorios, 4 servicios, EF Core configs, seed data |
| `backend/cqrs-plan.md` | 5 Commands/Queries + Handlers, 2 Validators, ownership validation |
| `backend/postman-collection.json` | 12 requests, ~65 assertions, collection Newman ejecutable |
| `backend/POSTMAN_COLLECTION_README.md` | Guia completa de la collection |
| `backend/EXECUTION_GUIDE.md` | Quick start para ejecutar tests |

### Frontend Landing (4 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-landing/frontend-plan.md` | 13 componentes, 5 hooks, 1 service, 32 archivos, wizard 3 pasos |
| `frontend-landing/ui-design.md` | 5 componentes custom shadcn/ui, design tokens, responsive, accesibilidad |
| `frontend-landing/test-strategy.md` | 36 tests (18 unit, 8 hooks, 4 services, 6 integration), 85% coverage |
| `frontend-landing/qa-validation.md` | Matriz de cobertura AC-CS01-1 a AC-CS01-9, gap analysis |

### Frontend Admin (5 archivos)

| Archivo | Contenido |
|---------|-----------|
| `frontend-admin/frontend-plan.md` | 14 componentes, 8 hooks, 3 services, 24 archivos, CRUD templates |
| `frontend-admin/ui-design.md` | 18 componentes shadcn/ui, 6 composiciones custom, dark theme |
| `frontend-admin/test-strategy.md` | 24 tests, 80%+ coverage target |
| `frontend-admin/qa-validation.md` | Admin scope (LOW priority MVP), AC-CS01-8 coverage |

---

## Orden de Implementacion

### Fase 1: Shared (bloqueante)
```bash
/implement cs-templates-guia --target shared
```
- Crear types, schemas Zod, constantes en `src/shared/`

### Fase 2: Backend
```bash
/implement cs-templates-guia --target backend
```
1. Domain: Entidades, Constants (ServiceResponseMessageType)
2. Infrastructure: Repositories, Services, EF Core configs, Seed data
3. Application: Commands, Queries, Handlers, Validators, DTOs, Profiles
4. WebApi: Controllers
5. Migration: `dotnet ef migrations add AddCrowdsourcingTemplates`

### Fase 3: Frontend Landing
```bash
/implement cs-templates-guia --target landing
```
1. API Service + Hooks (useTemplates, useTemplateDetail, etc.)
2. Step 1: TemplateGallery con cards
3. Step 2: NecesidadList con checkboxes, budget inputs, BudgetSummary
4. Step 3: ConfirmationSummary + submit
5. Routing + tests

### Fase 4: Frontend Admin (post-MVP)
```bash
/implement cs-templates-guia --target admin
```
1. Template list con tabla y filtros
2. Create/Edit form con needs dinamicos
3. Template detail view

### Todo junto:
```bash
/implement cs-templates-guia --target all
```

---

## Metricas del Plan

| Metrica | Valor |
|---------|-------|
| Agentes ejecutados | 13 |
| Archivos de plan | 16 |
| Entidades nuevas | 5 (PlantillaProyecto, PlantillaProyectoNecesidad, MaestraRolProfesional, MaestraCategoriaRol, MaestraTipoEmpresa) |
| Endpoints API | 5 (3 templates + 2 maestras) |
| Commands/Queries CQRS | 5 |
| Componentes Landing | 13 + shared |
| Componentes Admin | 14 + shared |
| Tests Landing | 36 |
| Tests Admin | 24 |
| Tests Newman | 12 requests, ~65 assertions |
| Seed data | 6 templates, ~55 necesidades, ~35 roles, 6 categorias |
