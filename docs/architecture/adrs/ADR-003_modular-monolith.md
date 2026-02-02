# ADR-003: Modular Monolith sobre Microservicios

## Metadata
- **Estado**: Aceptada
- **Fecha**: 2026-01-21
- **Relacionado con**: ADR-001, ADR-002
- **Basado en**: miUrba ADR-003

---

## Contexto

WePlay Rises tiene multiples bounded contexts que podrian ser microservicios:
- **Identity**: Autenticacion, usuarios base
- **UserAccess**: Perfiles de artista y fan
- **Crowdfunding**: Campanas, rewards, backings

Sin embargo, en la fase MVP:
- Solo hay un desarrollador o equipo muy pequeno
- Restriccion de tiempo: 30 horas totales
- No hay problemas de escala conocidos
- Infraestructura de microservicios anade complejidad operacional

---

## Decision

Implementar un **Modular Monolith** donde cada bounded context es un modulo independiente dentro de un mismo deployment unit (WebApi).

**Caracteristicas:**
- Cada modulo tiene estructura DDD completa (Domain/Application/Infra/WebApi)
- Todos comparten la misma base de datos (WePlayRisesDB)
- Modulos no se comunican directamente entre si
- Un solo proyecto WebApi como composition root

**Estructura:**

```
src/api/
+-- BuildingBlocks/           # Infraestructura compartida
+-- Modules/
|   +-- Identity/             # Auth + Users base
|   +-- UserAccess/           # Artista, FanProfile
|   +-- Crowdfunding/         # Campania, Reward, Backing
+-- WebApi/                   # Composition root
```

---

## Justificacion

**Por que Modular Monolith:**
- Mas simple operacionalmente (un deploy, un monitoreo)
- Mas rapido de desarrollar (30 horas disponibles)
- Sin overhead de comunicacion entre servicios
- Transacciones ACID simples
- Debugging mas facil

**Por que NO Microservicios:**
- Complejidad prematura (distributed tracing, service mesh)
- Overhead de comunicacion HTTP entre servicios
- Transacciones distribuidas complejas
- Mas infraestructura necesaria
- Equipo de una persona no se beneficia

**Filosofia:** "Start with a monolith, extract microservices when needed"

---

## Alternativas Consideradas

### Alternativa 1: Microservicios desde el inicio
- **Descripcion**: Cada bounded context como servicio independiente
- **Razon para no elegirla**: Complejidad prematura, 30h no son suficientes

### Alternativa 2: Monolito tradicional (no modular)
- **Descripcion**: Todo en un proyecto sin separacion clara
- **Razon para no elegirla**: Dificil evolucionar despues del MVP

### Alternativa 3: Serverless Functions
- **Descripcion**: Azure Functions para cada operacion
- **Razon para no elegirla**: Cold starts, fragmentacion excesiva

---

## Consecuencias

### Positivas
- Desarrollo mas rapido (critico con 30 horas)
- Operacion simple (un deployment)
- Sin overhead de red entre modulos
- Transacciones simples
- Preparado para evolucion gradual post-MVP

### Negativas
- Todos los modulos se despliegan juntos
- No hay aislamiento de recursos por modulo
- Un bug en un modulo puede afectar a todo

### Riesgos
- **"Big Ball of Mud"** si no se respeta modularidad
- **Tentacion de acoplar modulos** -> No permitir referencias directas

---

## Modulos MVP

| Modulo | Responsabilidad | Entidades |
|--------|-----------------|-----------|
| **Identity** | Autenticacion, JWT, Users base | ApplicationUser |
| **UserAccess** | Perfiles de dominio | Artista, FanProfile |
| **Crowdfunding** | Core del negocio | Campania, Reward, Backing |

**Reglas de modulos:**
- Un modulo NO puede referenciar otro modulo directamente
- Comunicacion via IDs (Guid) compartidos
- Cada modulo tiene su propia carpeta en el proyecto

---

## Notas de Implementacion

**Estructura actual WePlay Rises:**

```
src/api/
+-- BuildingBlocks/           # Compartido
|   +-- Exceptions/
|   +-- Extensions/
+-- Modules/
|   +-- Identity/
|   |   +-- Identity.Domain/
|   |   +-- Identity.Application/
|   |   +-- Identity.Infra/
|   |   +-- Identity.WebApi/
|   +-- UserAccess/
|   |   +-- UserAccess.Domain/
|   |   +-- UserAccess.Application/
|   |   +-- UserAccess.Infra/
|   |   +-- UserAccess.WebApi/
|   +-- Crowdfunding/
|       +-- Crowdfunding.Domain/
|       +-- Crowdfunding.Application/
|       +-- Crowdfunding.Infra/
|       +-- Crowdfunding.WebApi/
+-- WebApi/                   # Composition root
    +-- Program.cs
    +-- appsettings.json
```

**Base de datos unica:**

```json
{
  "ConnectionStrings": {
    "WePlayRisesDB": "Server=(localdb)\\mssqllocaldb;Database=WePlayRises;..."
  }
}
```

---

## Criterios para Extraer Microservicio (Post-MVP)

Extraer cuando **al menos 2** se cumplan:
1. Modulo necesita escalar independientemente
2. Equipos necesitan autonomia de deploy
3. Compliance/regulacion requiere aislamiento
4. Modulo muy grande (>20k LOC)

**NO extraer** solo porque "es best practice"

---

## Referencias

- [Monolith First - Martin Fowler](https://martinfowler.com/bliki/MonolithFirst.html)
- [Modular Monolith: A Primer - Kamil Grzybek](https://www.kamilgrzybek.com/blog/posts/modular-monolith-primer)
- [Microservices Prerequisites - Martin Fowler](https://martinfowler.com/bliki/MicroservicePrerequisites.html)
