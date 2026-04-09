# Plan: cp-tracking-metricas

> **Feature:** Tracking de Eventos, Conversiones y Dashboard de Metricas
> **User Story:** US-CP-05
> **Generado:** 2026-03-02
> **Modulo:** Crowdpromotion

---

## Resumen

Este plan cubre la implementacion completa del motor de tracking de CrowdPromotion:
- Registro automatico de eventos (Click, PageView, Signup, Backing) via enlaces referidos
- Calculo sincrono de comisiones al registrar conversiones
- Dashboard de metricas para artistas (Admin)
- Dashboard de metricas para promotores (Landing)

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `shared/contracts-plan.md` | 451 | 14 types TypeScript, 2 Zod schemas, constantes, utils |

### Backend (4 archivos)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `backend/api-contracts.md` | 956 | 4 endpoints, DTOs request/response, FluentValidation, AutoMapper |
| `backend/hexagonal-architecture.md` | 658 | Domain entities, repositories, services, rate limiting, DI |
| `backend/cqrs-plan.md` | 913 | 4 Commands/Queries + Handlers + Validators, flujo completo |
| `backend/postman-collection.json` | 2345 | 50+ requests Newman, ~120 assertions, auto-inclusivo |

### Frontend Landing (4 archivos)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `frontend-landing/frontend-plan.md` | 1021 | 9 componentes, 4 hooks, 1 service, routing |
| `frontend-landing/ui-design.md` | 881 | 7 componentes shadcn/ui, responsive, accesibilidad |
| `frontend-landing/test-strategy.md` | 940 | 54 tests (38 unit + 16 integration), 9 archivos |
| `frontend-landing/qa-validation.md` | - | Validacion de criterios de aceptacion |

### Frontend Admin (4 archivos)

| Archivo | Lineas | Contenido |
|---------|--------|-----------|
| `frontend-admin/frontend-plan.md` | 689 | 8 componentes, 2 hooks, 1 service, tab integration |
| `frontend-admin/ui-design.md` | 1067 | 14 shadcn components, 8 composiciones, Recharts |
| `frontend-admin/test-strategy.md` | 678 | 62 tests (44 unit + 18 integration), 10 archivos |
| `frontend-admin/qa-validation.md` | 334 | 83% cobertura, 3 gaps mayores identificados |

**Total: 14 archivos de planificacion (~10,900 lineas)**

---

## Orden de Implementacion Recomendado

```
1. Shared          → src/shared/ (types, schemas, constants, utils)
2. Backend         → src/api/Modules/Crowdpromotion/
3. Landing         → src/web/src/features/tracking/
4. Admin           → src/admin/src/ (tab metricas)
```

### Detalle por Proyecto

**Shared (primero, bloqueante):**
1. `src/shared/types/crowdpromotion.ts` - Tipos TypeScript
2. `src/shared/schemas/crowdpromotion.schema.ts` - Zod schemas
3. `src/shared/constants/tracking.ts` - Constantes de tracking
4. Actualizar barrel exports

**Backend:**
1. Agregar 5 constantes a `ServiceResponseMessageType.cs`
2. Extender entidad `PromoEvento` con campos de tracking + migracion
3. Crear interfaces (IPromoEventoRepository, IPromoEventoService, IRateLimitingService)
4. Implementar Repository + Service + RateLimitService
5. Crear DTOs de response
6. Crear Commands/Queries + Handlers + Validators
7. Crear AutoMapper Profile
8. Registrar DI + Controller

**Landing:**
1. `tracking.service.ts` - API service
2. `useTrackingInterceptor` - Hook en App.tsx
3. Componentes del dashboard promotor
4. Routing + integracion

**Admin:**
1. `metricas.service.ts` - API service
2. `useProgramaMetricas` + `useSortableTable` hooks
3. Componentes del dashboard artista
4. Integracion como tab en detalle de programa

---

## Gaps Conocidos

| Gap | Severidad | Plan Afectado | Descripcion |
|-----|-----------|---------------|-------------|
| GAP-01 | Mayor | Admin | Breadcrumb inconsistente entre planes |
| GAP-02 | Mayor | Admin | Loading con filtro sin markup detallado |
| GAP-03 | Mayor | Admin | Card de error sin composicion JSX |

Estos gaps se resuelven durante la implementacion.

---

## Siguiente Paso

```bash
/implement cp-tracking-metricas --target all
```

O implementar por partes:
```bash
/implement cp-tracking-metricas --target shared
/implement cp-tracking-metricas --target backend
/implement cp-tracking-metricas --target landing
/implement cp-tracking-metricas --target admin
```
