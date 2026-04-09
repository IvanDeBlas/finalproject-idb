# Plan: cs-acuerdos-entregables (US-CS-04)

**Fecha de generacion:** 2026-02-18
**Feature:** Acuerdos, Milestones y Entregables
**User Story:** US-CS-04

---

## Resumen

Esta feature implementa el ciclo de vida completo del modulo crowdsourcing: aceptar propuestas, formalizar acuerdos, gestionar milestones con importes parciales, subir y revisar entregables, y completar o cancelar acuerdos. La operacion mas compleja es `AceptarPropuesta` (6 efectos transaccionales en una sola operacion de BD).

**Impacto por proyecto:**
- **Backend**: ALTO (10 Commands, 1 Query, 3 entidades nuevas, 3 Services, migraciones)
- **Shared**: MEDIO (17 interfaces + 3 union types, 7 schemas Zod, constants)
- **Landing**: ALTO (1 pagina, 8 dialogs, 11 componentes, 11 hooks, 3 services)
- **Admin**: No involucrado

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | 17 interfaces + 3 union types, 7 schemas Zod, constantes (ESTADO_ACUERDO, ESTADO_ENTREGABLE, QUERY_KEYS, API_ROUTES, APP_ROUTES, ERROR_MESSAGES) |

### Backend (5 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | 11 endpoints, 22 DTOs C#, mapeo a Controllers, FluentValidation rules, AutoMapper profiles |
| `backend/hexagonal-architecture.md` | 3 entidades nuevas (AcuerdoCrowdsourcing, Milestone, Entregable), repos, services con IRequestCacheService, EF Core configs, maestras, migraciones |
| `backend/cqrs-plan.md` | 10 Commands + 1 Query con Handlers, Validators con ServiceResponseMessageType constants, AceptarPropuesta transaccional |
| `backend/postman-collection.json` | Coleccion Newman ejecutable con 11 endpoints, setup auto-inclusivo, tests de validacion y autorizacion |
| `backend/POSTMAN_EXECUTION_GUIDE.md` | Guia de ejecucion de la coleccion Newman |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | 1 pagina + 8 dialogs + 11 componentes, 11 hooks, 3 services, rutas, estado local |
| `frontend-landing/ui-design.md` | Composicion shadcn/ui por pantalla, badges de estado, dark theme, responsive, accesibilidad |
| `frontend-landing/test-strategy.md` | 70 tests (42 unit + 28 integration), cobertura 80%+, fixtures, mocking strategy |
| `frontend-landing/qa-validation.md` | 12 criterios de aceptacion mapeados: 9 cubiertos completamente, 3 parcialmente, 0 sin cubrir |

**Total: 10 archivos de planificacion**

---

## Orden de Implementacion Recomendado

### Fase 1: Shared (bloqueante)
```bash
/implement cs-acuerdos-entregables --target shared
```
- Types, schemas Zod, constants en `src/shared/`

### Fase 2: Backend
```bash
/implement cs-acuerdos-entregables --target backend
```
1. Entidades Domain + EF Core configs
2. Maestras (MaestraEstadoAcuerdo, MaestraEstadoEntregable)
3. Migraciones
4. Repositories + Services
5. Commands/Queries + Validators + Handlers
6. AutoMapper Profiles
7. Controllers
8. DI registration

### Fase 3: Frontend Landing
```bash
/implement cs-acuerdos-entregables --target landing
```
1. Services (API calls)
2. Hooks (queries + mutations)
3. Componentes reutilizables (badges, items, skeleton)
4. Dialogs (8 formularios/confirmaciones)
5. Pagina AcuerdoDetallePage
6. Ruta en router
7. Tests

### Implementar todo de una vez
```bash
/implement cs-acuerdos-entregables --target all
```

---

## Criterios de Aceptacion

| AC | Estado | Notas |
|----|--------|-------|
| AC-CS04-1 | Cubierto | AceptarPropuestaCommand transaccional |
| AC-CS04-2 | Cubierto | Rechazo atomico de propuestas pendientes |
| AC-CS04-3 | Cubierto | Creacion ConversacionCrowdsourcing + toast + redirect |
| AC-CS04-4 | Cubierto | Validacion participante en GetAcuerdoByIdQuery |
| AC-CS04-5 | Cubierto | AcuerdoDetallePage con todas las secciones |
| AC-CS04-6 | Cubierto | Milestones con validacion de importes |
| AC-CS04-7 | Cubierto | Entregables solo profesional + URL externa |
| AC-CS04-8 | Cubierto | Aprobar (opcional) / Rechazar (min 10 chars) |
| AC-CS04-9 | Cubierto | Entregable rechazado como historial |
| AC-CS04-10 | Cubierto | Completar acuerdo + necesidad Cerrada |
| AC-CS04-11 | Cubierto | Cancelar con dialogo irreversible |
| AC-CS04-12 | Cubierto | Rechazar propuesta sin exponer motivo |

---

## Notas

- El agente `ui-ux-analyzer` no se ejecuto (requiere app corriendo). El plan visual se baso en `ui-ux.md`.
- Admin no esta involucrado en esta feature (toda la interaccion es en Landing).
- La operacion `AceptarPropuesta` es la mas critica: 6 efectos en una transaccion de BD.
- El timeline del acuerdo se calcula desde fechas de entidades (no tabla separada en MVP).
