# Plan: cs-mensajeria (US-CS-05)

> **Feature:** Mensajeria entre Partes
> **Generado:** 2026-02-18
> **Estado:** Listo para implementacion

---

## Resumen

Sistema de mensajeria directa entre artistas y profesionales en el modulo Crowdsourcing. Conversaciones contextualizadas a necesidades o acuerdos, con mensajes de texto y URLs adjuntas. MVP con polling (sin WebSocket).

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Types TypeScript, Zod schemas, constants (QUERY_KEYS, API_ROUTES, polling intervals) |

### Backend (5 archivos + extras)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs request/response, FluentValidation rules, AutoMapper profiles para 6 endpoints |
| `backend/hexagonal-architecture.md` | Domain (entidades, constants) + Infrastructure (repos, services, EF Core configs, migracion) |
| `backend/cqrs-plan.md` | Commands/Queries/Handlers/Validators para: CreateConversacion, SendMensaje, MarcarLeidos, GetConversaciones, GetMensajes, GetNoLeidosCount |
| `backend/postman-collection.json` | Coleccion Postman ejecutable con Newman (tests de integracion auto-inclusivos) |
| `backend/README-POSTMAN.md` | Instrucciones de ejecucion de la coleccion Newman |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura de componentes, hooks, services, rutas, orden de implementacion |
| `frontend-landing/ui-design.md` | Composicion shadcn/ui, clases Tailwind, estados, accesibilidad, responsive |
| `frontend-landing/test-strategy.md` | Estrategia de testing (Vitest + Testing Library): unit, integration, mocking |
| `frontend-landing/qa-validation.md` | Validacion de criterios AC-CS05-1 a AC-CS05-12 (84% cobertura, gaps documentados) |

### Frontend Admin

No aplica. Toda la mensajeria ocurre en la Landing.

---

## Hallazgos del QA

### Gaps Criticos a Resolver Antes de Implementar

1. **Polling intervals inconsistentes** entre specs: 10s (feature-spec), 30s (ui-ux listado), 60s (ui-ux navbar). Unificar a 10s para mensajes y 30s para listado/badge.
2. **Error 4015 (conversacion duplicada)** debe incluir el `conversacionId` existente en la respuesta para que el frontend pueda redirigir.
3. **ConversacionDetalleDto** referenciado en ui-ux.md pero sin DTO backend correspondiente. Resolver si se usa la misma respuesta del listado o un endpoint dedicado.

### Discrepancia de Nomenclatura (Documentada)

La entidad `ConversacionCrowdsourcing` existente (US-CS-04) usa `UserIdArtista`/`UserIdProveedor`. Los contratos definen `UserIdCreador`/`UserIdDestinatario`. El plan de hexagonal-architecture incluye migracion de renombrado de columnas.

---

## Orden de Implementacion Recomendado

```
1. /implement cs-mensajeria --target shared
   (Types, Zod schemas, constants en src/shared/)

2. /implement cs-mensajeria --target backend
   (Domain -> Infrastructure -> Application -> WebApi)
   2a. Constantes ServiceResponseMessageType (2014, 4015, 4016)
   2b. Entidades + Migracion EF Core
   2c. Repositorios + Services
   2d. DTOs + AutoMapper Profiles
   2e. Commands/Queries + Handlers + Validators
   2f. Controller (ConversacionCrowdsourcingController)

3. /implement cs-mensajeria --target landing
   (Services -> Hooks -> Components -> Pages -> Navbar badge)
   3a. API services (conversacion.service.ts, mensaje.service.ts)
   3b. Custom hooks (useConversaciones, useMensajes, etc.)
   3c. Components (MessageBubble, ConversacionRow, ChatView, etc.)
   3d. Pages (MensajesPage, ConversacionChatPage)
   3e. Navbar integration (NavbarMensajesIcon)
   3f. Entry points (IniciarConversacionButton/Dialog)

4. Tests
   4a. Backend: Newman collection (postman-collection.json)
   4b. Frontend: Vitest unit + integration tests
```

---

## Dependencias Previas

- **US-CS-04** (cs-acuerdos-entregables): Debe estar implementado. ConversacionCrowdsourcing ya existe en BD.
- **US-CS-02** (cs-gestionar-necesidades): Necesidades y propuestas deben existir para el flujo de crear conversacion.
- **shadcn ScrollArea**: Requiere instalacion en src/web: `npx shadcn@latest add scroll-area`

---

## Siguiente Paso

```bash
/implement cs-mensajeria --target all
```

O implementar por partes:
```bash
/implement cs-mensajeria --target shared
/implement cs-mensajeria --target backend
/implement cs-mensajeria --target landing
```
