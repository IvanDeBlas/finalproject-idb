# Architecture Decision Records (ADRs) - WePlay Rises

> **Memoria institucional** del proyecto: documentan el "por que" de las decisiones arquitectonicas.

| | |
|---|---|
| **Version** | 1.0 |
| **Ultima actualizacion** | 2026-01-21 |
| **Proyecto** | WePlay Rises - Crowdfunding Musical MVP |

---

## Que son los ADRs?

Un **Architecture Decision Record (ADR)** captura una decision arquitectonica importante junto con su contexto y consecuencias.

### Por que usar ADRs?

| Beneficio | Descripcion |
|-----------|-------------|
| **Memoria institucional** | Captura el "por que" detras de decisiones |
| **Onboarding** | Nuevos miembros entienden decisiones pasadas |
| **Evita repetir debates** | Decisiones ya discutidas y documentadas |
| **Auditoria** | Trazabilidad de evolucion arquitectonica |

---

## Indice de ADRs

### Aceptadas (Activas)

| ADR | Titulo | Fecha | Impacto |
|-----|--------|-------|---------|
| [ADR-001](./ADR-001_ddd-arquitectura-hexagonal.md) | DDD + Arquitectura Hexagonal | 2026-01-21 | Estructura de todos los modulos |
| [ADR-002](./ADR-002_cqrs-mediatr.md) | CQRS con MediatR | 2026-01-21 | Commands/Queries separados |
| [ADR-003](./ADR-003_modular-monolith.md) | Modular Monolith | 2026-01-21 | Deploy unico, modulos desacoplados |
| [ADR-004](./ADR-004_jwt-identity.md) | JWT + Identity | 2026-01-21 | Autenticacion y autorizacion |
| [ADR-005](./ADR-005_strongly-typed-ids.md) | Strongly Typed IDs | 2026-01-21 | Type-safety en IDs de entidades |
| [ADR-006](./ADR-006_caching-strategy.md) | Estrategia de Caching | 2026-01-23 | Optimizacion Validator-Handler |

### Descartadas para MVP (Complejidad excesiva)

Las siguientes decisiones de miUrba fueron evaluadas y **descartadas para este MVP** por restriccion de tiempo (30 horas):

| Decision | Razon de Descarte |
|----------|-------------------|
| BD Separada por Modulo | Un solo dominio, una BD es suficiente |
| Pulumi IaC | Deploy manual a Azure suficiente |
| Domain Events + Outbox | Complejidad innecesaria para MVP |
| API Gateway (APIM) | Sin necesidad de rate limiting aun |
| Micro-frontends | Un solo frontend Next.js |

---

## Resumen Visual

```
+--------------------------------------------------------------------+
|                 ARQUITECTURA WEPLAY RISES MVP                       |
+--------------------------------------------------------------------+
|                                                                    |
|  BACKEND (.NET 8)                     FRONTEND (Next.js 14)        |
|  ----------------                     ---------------------        |
|  +------------------+                 +------------------+         |
|  | ADR-001          |                 |     Next.js      |         |
|  | DDD + Hexagonal  |                 |   TypeScript     |         |
|  |                  |                 |   Tailwind CSS   |         |
|  | Domain/          |                 |   shadcn/ui      |         |
|  | Application/     |                 +------------------+         |
|  | Infrastructure/  |                                              |
|  | WebApi/          |                                              |
|  +------------------+                                              |
|                                                                    |
|  +------------------+                 +------------------+         |
|  | ADR-002          |                 | ADR-004          |         |
|  | CQRS + MediatR   |                 | JWT + Identity   |         |
|  |                  |                 |                  |         |
|  | Commands/        |                 | Login/Register   |         |
|  | Queries/         |                 | Roles: Artista   |         |
|  | Handlers/        |                 |        Fan       |         |
|  | Validators/      |                 +------------------+         |
|  +------------------+                                              |
|                                                                    |
|  +------------------+                 +------------------+         |
|  | ADR-003          |                 | ADR-006          |         |
|  | Modular Monolith |                 | Caching Strategy |         |
|  |                  |                 |                  |         |
|  | Identity/        |                 | RequestCache     |         |
|  | UserAccess/      |                 | (Validator-Handler)|       |
|  | Crowdfunding/    |                 | MemoryCache      |         |
|  +------------------+                 +------------------+         |
|                                                                    |
+--------------------------------------------------------------------+
```

---

## Template para Nuevos ADRs

```markdown
# ADR-XXX: [Titulo Corto y Descriptivo]

## Metadata
- **Estado**: Propuesta | Aceptada | Rechazada
- **Fecha**: YYYY-MM-DD
- **Relacionado con**: [ADR-XXX, ADR-YYY]

## Contexto
Que problema estamos resolviendo?

## Decision
Describir la decision tomada.

## Justificacion
Por que esta decision?

## Alternativas Consideradas
### Alternativa 1: [Nombre]
- Descripcion
- Razon para no elegirla

## Consecuencias
### Positivas
- Consecuencia positiva 1

### Negativas
- Consecuencia negativa 1
```

---

## Referencias

- [Implementing Domain-Driven Design - Vaughn Vernon](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577)
- [CQRS Pattern - Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [Monolith First - Martin Fowler](https://martinfowler.com/bliki/MonolithFirst.html)
- [Hexagonal Architecture - Alistair Cockburn](https://alistair.cockburn.us/hexagonal-architecture/)

---

*ADRs basados en el ecosistema miUrba, adaptados para WePlay Rises MVP.*
