# Plan: cp-perfil-promotor (US-CP-01)

> **Feature:** Perfil de Promotor - Modulo Crowdpromotion
> **Generado:** 2026-02-25
> **Agentes ejecutados:** 12

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Tipos TypeScript, schemas Zod, constantes, mappers y error messages |

### Backend (4 archivos principales + documentacion Newman)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs C#, FluentValidation, AutoMapper profiles, Controller actions |
| `backend/hexagonal-architecture.md` | Domain (entidades, interfaces) + Infrastructure (repos, services, EF Core) |
| `backend/cqrs-plan.md` | Commands, Queries, Handlers, Validators - Application layer completa |
| `backend/postman-collection.json` | Coleccion Postman ejecutable (20 requests, 64 assertions, 100% cobertura) |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura: 9 componentes, 4 hooks, 1 service, routing con guards |
| `frontend-landing/ui-design.md` | Componentes shadcn/ui, composicion, estados, responsive, accesibilidad |
| `frontend-landing/test-strategy.md` | 110 tests planificados en 11 archivos, cobertura 80%+ |
| `frontend-landing/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | Arquitectura: 10 componentes, 3 hooks, 1 service, Next.js App Router |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui para dashboard admin, tema oscuro Dashtail |
| `frontend-admin/test-strategy.md` | 52 tests planificados en 5 archivos, cobertura 80%+ |
| `frontend-admin/qa-validation.md` | Validacion de criterios de aceptacion vs planes |

---

## Resumen por Capa

### Shared
- 6 tipos TypeScript + 1 union type en `src/shared/types/crowdpromotion.ts`
- 2 schemas Zod en `src/shared/schemas/crowdpromotion.schema.ts`
- 5 bloques de constantes nuevas (QUERY_KEYS, API_ROUTES, APP_ROUTES, TIPO_PROMOTOR, VALIDATION)
- Mappers, error messages y formatters

### Backend
- Entidad Promotor: agregar EmailContacto, UrlSitioWeb, ampliar NombrePublico a 200
- 3 Commands + 1 Query (CreatePromotor, UpdatePromotor, DesactivarPromotor, GetPromotorMe)
- 2 Validators (Create, Update) con ServiceResponseMessageType constants
- 1 AutoMapper Profile con manejo de Strongly Typed IDs
- 1 Controller con 4 actions
- IPromotorService con 7 metodos especializados + IRequestCacheService
- 1 migracion EF Core
- Coleccion Newman con 20 requests y 64 assertions

### Frontend Landing
- 3 paginas: /promotor/registro, /promotor/dashboard, /promotor/perfil
- 9 componentes (PromotorForm compartido create/edit, KPIs, DeactivateDialog)
- 4 hooks (usePromotor, useCreatePromotor, useUpdatePromotor, useDesactivarPromotor)
- Guards de redireccion por estado del perfil
- 110 tests planificados

### Frontend Admin
- Seccion promotor en dashboard existente + pagina /promotor/perfil
- 10 componentes (ProfileCard, KpiCards, EditForm, StatusBadge, Banner)
- 3 hooks + 1 service
- 52 tests planificados

---

## Hallazgos Importantes

1. **Discrepancia HTTP 400 vs 409**: El BaseLoggerController mapea codigos 4xxx a HTTP 409 (no 400 como dice el contrato). Frontend debe aceptar ambos.
2. **NombrePublico max length**: La entidad tiene max 100, el contrato exige 200. Requiere migracion.
3. **URLs max length**: Las URLs existentes tienen max 200, el contrato exige 300. Incluir en la migracion.
4. **TipoPromotorNombre**: Se resuelve con diccionario estatico (MVP) para evitar dependencia cross-modulo.

---

## Siguiente Paso

```bash
/implement cp-perfil-promotor --target all
```

O implementar por partes:
```bash
/implement cp-perfil-promotor --target shared
/implement cp-perfil-promotor --target backend
/implement cp-perfil-promotor --target landing
/implement cp-perfil-promotor --target admin
```
