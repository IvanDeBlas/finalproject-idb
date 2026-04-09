# Plan: Crear Campania (US-02)

> Generado: 2026-02-12
> Feature: crear-campania
> Agentes ejecutados: 13

---

## Resumen

Plan completo de implementacion para la feature de creacion de campanias de crowdfunding.
Incluye wizard de 4 pasos en Admin, detalle publico en Landing, y endpoints CRUD + publicacion en Backend.

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Types TypeScript, schemas Zod (por step del wizard), constantes (estados, monedas, rutas), utils (error messages, formatCurrency) |

### Backend (8 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs C#, FluentValidation validators, AutoMapper profiles, Controller actions |
| `backend/hexagonal-architecture.md` | Domain (entidad, interfaces) + Infrastructure (repositorio, service, EF Core config) |
| `backend/cqrs-plan.md` | Commands (Create, Update, Publish) + Queries (GetById, GetAll, MisCampanias) + Handlers |
| `backend/newman-tests.md` | Estrategia de tests de integracion con Postman/Newman |
| `backend/postman-collection.json` | Coleccion Postman ejecutable con tests de contratos |
| `backend/environment-development.json` | Variables de entorno para Newman |
| `backend/run-newman.ps1` | Script PowerShell para ejecutar tests |
| `backend/run-newman.sh` | Script Bash para ejecutar tests |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura: componentes, hooks, services, rutas para /explorar y /campanias/{id} |
| `frontend-landing/ui-design.md` | Componentes shadcn/ui: CampaniaCard, CampaniaDetail, ProgressBar, RewardCard, ArtistCard |
| `frontend-landing/test-strategy.md` | Tests Vitest + Testing Library para componentes y hooks |
| `frontend-landing/qa-validation.md` | Validacion de criterios de aceptacion (AC-02-7, AC-02-8) |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | Arquitectura: wizard 4 steps, mis campanias, preview, editar con Next.js App Router |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui: WizardStepper, form steps, DraftBanner, CampaniaListCard, modals |
| `frontend-admin/test-strategy.md` | Tests Vitest + Testing Library para wizard, formularios, mutations |
| `frontend-admin/qa-validation.md` | Validacion de criterios de aceptacion (AC-02-1 a AC-02-13) |

---

## Endpoints API

| Metodo | Ruta | Auth | Descripcion |
|--------|------|------|-------------|
| POST | /api/campanias | JWT | Crear campania (BORRADOR) |
| GET | /api/campanias/{id} | No | Detalle campania |
| PUT | /api/campanias/{id} | JWT | Actualizar borrador |
| POST | /api/campanias/{id}/publicar | JWT | Publicar campania |
| GET | /api/campanias | No | Listar campanias publicas |
| GET | /api/campanias/mis-campanias | JWT | Mis campanias |

---

## Criterios de Aceptacion Cubiertos

| ID | Criterio | Proyecto | Cubierto |
|----|----------|----------|----------|
| AC-02-1 | Campania creada en estado BORRADOR | Backend | Si |
| AC-02-2 | Meta financiera > 0 | Backend + Admin | Si |
| AC-02-3 | Fecha fin minimo 7 dias | Admin + Backend | Si |
| AC-02-4 | Fecha fin maximo 60 dias | Admin + Backend | Si |
| AC-02-5 | Solo artista dueno puede editar | Backend | Si |
| AC-02-6 | Publicar cambia estado a PUBLICADA | Backend | Si |
| AC-02-7 | BORRADOR no visible en landing | Backend + Landing | Si |
| AC-02-8 | PUBLICADA visible sin autenticacion | Landing + Backend | Si |
| AC-02-9 | Editar campania en BORRADOR | Backend + Admin | Si |
| AC-02-10 | FechaInicio null hasta publicar | Backend | Si |
| AC-02-11 | Wizard con stepper (1/4 a 4/4) | Admin | Si |
| AC-02-12 | Rich text editor en descripcion | Admin | Si |
| AC-02-13 | Schemas Zod en shared reutilizados | Shared + Admin | Si |

---

## Siguiente Paso

```bash
# Implementar todo
/implement crear-campania --target all

# O implementar por partes (orden recomendado)
/implement crear-campania --target shared
/implement crear-campania --target backend
/implement crear-campania --target landing
/implement crear-campania --target admin
```

---

## Orden de Implementacion Recomendado

1. **Shared** - Types, schemas Zod, constantes (base para todo)
2. **Backend** - Domain + Infrastructure + CQRS + Controller
3. **Landing** - Explorar campanias + detalle publico
4. **Admin** - Wizard crear + mis campanias + preview + publicar
