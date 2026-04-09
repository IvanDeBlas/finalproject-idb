# Plan: cs-gestionar-necesidades

> **Feature:** Gestion de Necesidades de Crowdsourcing (US-CS-02)
> **Generado:** 2026-02-16
> **Estado:** Plan completo - Listo para implementacion

---

## Resumen

Plan de implementacion completo para el CRUD de necesidades de crowdsourcing desde la perspectiva del artista. Incluye publicar, listar, editar y cerrar necesidades con auto-rechazo de propuestas pendientes.

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Types, Zod schemas, constants, utils compartidos |

### Backend (6 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs, Validators, Controller, AutoMapper |
| `backend/hexagonal-architecture.md` | Domain (entidades, repos, services) + Infrastructure |
| `backend/cqrs-plan.md` | Commands, Queries, Handlers, flujo completo |
| `backend/postman-collection.json` | Coleccion Newman ejecutable para integration tests |
| `backend/POSTMAN-README.md` | Guia de uso de la coleccion Postman |
| `backend/QUICK-START.md` | Guia rapida para ejecutar tests |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | Paginas, componentes, hooks, services, routing |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui, composicion, design tokens |
| `frontend-admin/test-strategy.md` | Unit tests, integration tests, mocking strategy |
| `frontend-admin/qa-validation.md` | Cobertura de criterios AC-CS02-1 a AC-CS02-12 |

**Total: 11 archivos de planificacion**

---

## Scope

| Proyecto | Involucrado | Motivo |
|----------|-------------|--------|
| Shared | Si | Types, schemas, constants |
| Backend | Si | CRUD completo + logica cierre |
| Admin | Si | 4 paginas + 7+ componentes |
| Landing | No | No involucrado en MVP (spec lo indica) |

---

## Orden de Implementacion Recomendado

1. **Shared** - Types, schemas, constants (bloqueante para frontend)
2. **Backend** - Domain + Infrastructure + Application (CQRS) + Controller
3. **Admin** - Pages, components, hooks, services
4. **Tests** - Unit tests backend + frontend + Newman integration

---

## Siguiente Paso

```bash
/implement cs-gestionar-necesidades --target all
```

O implementar por partes:
```bash
/implement cs-gestionar-necesidades --target shared
/implement cs-gestionar-necesidades --target backend
/implement cs-gestionar-necesidades --target admin
```
