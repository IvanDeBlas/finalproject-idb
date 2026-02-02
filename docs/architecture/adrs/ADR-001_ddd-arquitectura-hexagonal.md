# ADR-001: Adopcion de Domain-Driven Design y Arquitectura Hexagonal

## Metadata
- **Estado**: Aceptada
- **Fecha**: 2026-01-21
- **Relacionado con**: ADR-002, ADR-003
- **Basado en**: miUrba ADR-001

---

## Contexto

WePlay Rises es una plataforma de crowdfunding musical con un dominio que incluye:
- Artistas que crean campanas de financiacion
- Fans que apoyan campanas (backings)
- Recompensas por niveles de aportacion
- Gestion de identidad y perfiles

Necesitamos una arquitectura que:
- Maneje la complejidad del dominio de forma estructurada
- Permita evolucion independiente de modulos
- Facilite testing y mantenibilidad
- Desacople logica de negocio de infraestructura

---

## Decision

Adoptamos **Domain-Driven Design (DDD)** como enfoque de modelado y **Arquitectura Hexagonal (Ports & Adapters)** como patron arquitectonico base.

**Estructura de proyectos por modulo:**

```
src/api/Modules/{Modulo}/
+-- {Modulo}.Domain/           # Nucleo - Entidades, Value Objects
+-- {Modulo}.Application/      # Casos de uso - Commands, Queries
+-- {Modulo}.Infra/            # Adaptadores - Repositories, DbContext
+-- {Modulo}.WebApi/           # Adaptador - Controllers
```

---

## Justificacion

- **DDD** permite modelar el dominio de crowdfunding de forma explicita
- **Hexagonal** desacopla negocio de infraestructura, facilitando testing
- Facilita evolucion futura si el proyecto crece mas alla del MVP
- Consistente con patrones copiados de miUrba

---

## Alternativas Consideradas

### Alternativa 1: Clean Architecture (Uncle Bob)
- **Descripcion**: Similar a Hexagonal pero con mas capas
- **Razon para no elegirla**: Mayor complejidad sin beneficio claro para MVP

### Alternativa 2: Traditional Layered Architecture
- **Descripcion**: Capas UI -> BLL -> DAL
- **Razon para no elegirla**: Acoplamiento entre capas, testing dificil

### Alternativa 3: Transaction Script
- **Descripcion**: Logica directa en servicios sin modelo rico
- **Razon para no elegirla**: No escala con complejidad del dominio

---

## Consecuencias

### Positivas
- Codigo bien estructurado y mantenible
- Testabilidad excelente (domain sin dependencias)
- Separacion clara de responsabilidades
- Consistencia con miUrba (reutilizacion de conocimiento)

### Negativas
- Curva de aprendizaje para desarrolladores nuevos
- Mas archivos y estructura inicial (boilerplate)
- Puede parecer over-engineering para MVP simple

### Riesgos
- **Anemic Domain Model** si no se aplica DDD correctamente
- **Over-engineering** en entidades simples -> Mitigacion: Aplicar pragmaticamente

---

## Notas de Implementacion

**Estructura para modulo Crowdfunding:**

```
src/api/Modules/Crowdfunding/
+-- Crowdfunding.Domain/
|   +-- Entities/
|   |   +-- Campania.cs
|   |   +-- Reward.cs
|   |   +-- Backing.cs
|   +-- ValueObjects/
|   |   +-- Dinero.cs
|   +-- Services/
+-- Crowdfunding.Application/
|   +-- Features/Campanias/
|   |   +-- Commands/
|   |   |   +-- CreateCampania.cs
|   |   +-- Queries/
|   |   |   +-- GetCampaniaById.cs
|   |   +-- Validators/
+-- Crowdfunding.Infra/
|   +-- Persistence/
|   |   +-- CrowdfundingDbContext.cs
|   |   +-- CampaniaRepository.cs
+-- Crowdfunding.WebApi/
    +-- Controllers/
        +-- CampaniasController.cs
```

---

## Referencias

- [Implementing Domain-Driven Design - Vaughn Vernon](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577)
- [Hexagonal Architecture - Alistair Cockburn](https://alistair.cockburn.us/hexagonal-architecture/)
- [Domain-Driven Design - Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
