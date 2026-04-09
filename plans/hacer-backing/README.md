# Plan: Hacer Backing (US-04)

> Generado: 2026-02-13
> Feature: hacer-backing
> Status: Planificado

---

## Resumen

Plan de implementacion completo para la feature "Hacer Backing" que permite a los fans apoyar economicamente las campanias de crowdfunding. Incluye flujo completo desde exploracion de campanias hasta confirmacion de aporte.

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Tipos TypeScript, Zod schemas, constantes y utils compartidos entre Landing y Admin |

### Backend (5 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs request/response, FluentValidation rules, AutoMapper mappings, error codes |
| `backend/hexagonal-architecture.md` | Domain (entidades, constantes) + Infrastructure (services, repositories, transacciones) |
| `backend/cqrs-plan.md` | Commands, Queries, Handlers, Validators (Application layer completa) |
| `backend/newman-tests.md` | Estrategia de testing de integracion con Postman/Newman (34 requests, 45+ casos) |
| `backend/postman-collection.json` | Coleccion Postman ejecutable con 24 requests y ~85 assertions |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura: 10 componentes, 4 hooks, 2 services, rutas, flujo de datos |
| `frontend-landing/ui-design.md` | Composicion shadcn/ui, clases Tailwind, responsive, animaciones, accesibilidad |
| `frontend-landing/test-strategy.md` | 14 test suites: componentes, hooks, schemas, integracion (Vitest + Testing Library) |
| `frontend-landing/qa-validation.md` | Validacion de criterios de aceptacion AC-04-1 a AC-04-10 |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | Arquitectura MVP: stats grid + tabla backings (4 archivos nuevos) |
| `frontend-admin/ui-design.md` | Componentes shadcn/ui para dashboard artista (Table, Card, Badge) |
| `frontend-admin/test-strategy.md` | 34 tests: componentes, hooks, services (Vitest + Testing Library) |
| `frontend-admin/qa-validation.md` | Validacion de requisitos del artista (impacto BAJO para MVP) |

---

## Resumen Tecnico

### Endpoints API (5)

| Endpoint | Metodo | Auth | Descripcion |
|----------|--------|------|-------------|
| `/api/campanias` | GET | Publica | Lista campanias activas paginada |
| `/api/campanias/{id}` | GET | Publica | Detalle con rewards y backings recientes |
| `/api/campanias/{id}/backings` | POST | Opcional | Crear backing (aporte) |
| `/api/campanias/{id}/backings` | GET | Publica | Lista backings de una campania |
| `/api/campanias/{id}/stats` | GET | Publica | Estadisticas agregadas |

### CQRS Operations (4)

| Operacion | Tipo | Descripcion |
|-----------|------|-------------|
| `CreateBackingCommand` | Command | Crea backing con transaccion atomica |
| `GetCampaniaDetailQuery` | Query | Detalle completo con includes |
| `GetBackingsByCampaniaQuery` | Query | Lista paginada de backings |
| `GetCampaniaStatsQuery` | Query | Estadisticas agregadas |

### Componentes Frontend Landing (10)

| Componente | Estado | Descripcion |
|------------|--------|-------------|
| CampaniaCard | Existente | Card de campania en grid |
| CampaniaFilters | Nuevo | Search + filtros + ordenamiento |
| CampaniaDetailPage | Nuevo/Modificar | Detalle completo con tabs y sidebar |
| CampaniaRewardsSection | Nuevo | Lista de rewards en sidebar |
| RewardPublicCard | Nuevo | Card individual de reward |
| BackingModal | Nuevo | Modal/formulario de backing |
| BackingConfirmationPage | Nuevo | Confirmacion post-backing |
| BackingsRecentesList | Nuevo | Lista backings recientes |
| CampaignProgressBar | Nuevo | Barra de progreso reutilizable |
| AmountInput | Nuevo | Input de monto con validacion |

### Nuevas Constantes de Error

| Codigo | Constante | Descripcion |
|--------|-----------|-------------|
| 4011 | BusinessRule_RewardOutOfStock | Recompensa agotada |
| 4012 | BusinessRule_AmountBelowMinimum | Monto por debajo del minimo |
| 4013 | BusinessRule_AnonymousNotAllowed | Anonimos no permitidos |

---

## Orden de Implementacion Sugerido

```
1. /implement hacer-backing --target shared
   (tipos, schemas, constantes compartidos)

2. /implement hacer-backing --target backend
   (domain, infrastructure, CQRS, controllers)

3. /implement hacer-backing --target landing
   (componentes, hooks, pages, formulario)

4. /implement hacer-backing --target admin
   (tabla de backings para artista - secundario)
```

---

## Siguiente Paso

```bash
/implement hacer-backing --target all
```

O implementar por partes:
```bash
/implement hacer-backing --target shared
/implement hacer-backing --target backend
/implement hacer-backing --target landing
/implement hacer-backing --target admin
```
