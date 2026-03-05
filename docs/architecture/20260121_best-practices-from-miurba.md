# Buenas Practicas Extraidas de miUrba Workspace

> - **Analisis Critico**: 2026-01-21
> - **Objetivo**: Identificar patrones reutilizables para WePlay Rises
> - **Fuente**: `C:\Repos\miUrba - Jonay\`

---

## Resumen Ejecutivo

El workspace de miUrba tiene **10 anos de evolucion** en su sistema de gestion con Claude Code. He identificado **7 categorias de buenas practicas** que debemos evaluar criticamente para WePlay Rises.

| Categoria | Complejidad | Valor para WPR | Recomendacion |
|-----------|-------------|----------------|---------------|
| Tracking Dual (MD+YAML) | Alta | Alto | Simplificar a solo YAML |
| Sistema de Comandos | Media | Alto | Adoptar parcialmente |
| Sistema de Agentes | Alta | Medio | Evaluar necesidad real |
| Priority Rules | Media | Alto | Adoptar simplificado |
| Scripts Python | Alta | Bajo (MVP) | Posponer post-MVP |
| Specs Cross-Repo | Alta | Bajo (1 repo) | No aplica inicialmente |
| Health Checks | Media | Alto | Adoptar basico |

---

## 1. Tracking Dual (MD + YAML)

### Como funciona en miUrba

```
tasks/by_repo/*.md     <- Para humanos (tablas legibles)
tracking/*.yaml        <- Para scripts (procesable)
```

**Sincronizacion**: Scripts Python que mantienen ambos sincronizados.

### Analisis Critico

**Ventajas:**
- Humanos pueden editar MD sin conocer YAML
- Scripts pueden procesar YAML sin parsear MD
- Separacion de preocupaciones

**Desventajas:**
- Doble mantenimiento
- Scripts de sincronizacion complejos
- Fuente de bugs si se desincronizan
- Overhead para proyecto pequeno

### Recomendacion para WePlay Rises

**NO ADOPTAR inicialmente.** WePlay Rises es un proyecto de 1 persona con 30 horas disponibles. El overhead no se justifica.

**Alternativa simple:**
```yaml
# tasks/backlog.yaml - UNICA fuente de verdad
tasks:
  WPR-001:
    titulo: "Definir modelo de datos MVP"
    estado: pendiente
    estimado: 2h
    prioridad: alta
    dependencias: []

  WPR-002:
    titulo: "Copiar BuildingBlocks de miUrba"
    estado: pendiente
    estimado: 2h
    prioridad: alta
    dependencias: [WPR-001]
```

**Post-MVP**: Si el proyecto crece y hay equipo, evaluar tracking dual.

---

## 2. Sistema de Comandos (.claude/commands/)

### Como funciona en miUrba

Cada comando es un archivo Markdown con estructura:

```markdown
<argumentos>
#$ARGUMENTS
</argumentos>

# Comando: /nombre-comando

Descripcion breve.

---

## Descripcion
{Que hace el comando}

## Uso
{Ejemplos de invocacion}

## Fases de Ejecucion
{Paso a paso que hace Claude}

## Output Esperado
{Ejemplos de output}

## Checklist Obligatorio
{Lista de archivos que DEBE modificar}

## Manejo de Errores
{Casos de error y como resolverlos}
```

### Analisis Critico

**Ventajas:**
- Auto-documentado
- Claude sigue el workflow exacto
- Previene olvidos con checklists
- Formato consistente

**Desventajas:**
- Archivos muy largos (500+ lineas)
- Duplicacion de logica entre comandos
- Mantenimiento costoso

### Recomendacion para WePlay Rises

**ADOPTAR parcialmente.** Crear comandos solo para workflows repetitivos.

**Comandos esenciales para WPR:**

```
.claude/commands/
  |-- planning/
  |     |-- daily-plan.md      # Generar plan diario
  |     |-- done.md            # Marcar tarea completada
  |-- workflow/
  |     |-- build-test.md      # Compilar + tests
  |     |-- deploy.md          # Deploy a Azure
```

**Template simplificado:**

```markdown
# Comando: /done

Marca una tarea como completada.

## Uso
/done WPR-001

## Workflow
1. Buscar tarea en tasks/backlog.yaml
2. Cambiar estado a "completado"
3. Actualizar fecha_fin
4. Mostrar siguiente tarea sugerida

## Output
- Tarea {ID} completada
- Siguiente: {siguiente_tarea}
```

---

## 3. Sistema de Agentes (.claude/agents/)

### Como funciona en miUrba

Agentes especializados con frontmatter YAML:

```markdown
---
name: task-coordinator
description: Coordina planificacion de tareas
tools: Read, Write, Glob
model: sonnet
color: blue
---

Eres un experto en planificacion...

## Goal
{Objetivo del agente}

## Prerequisites
{Que debe existir}

## Workflow
{Pasos que sigue}

## Rules
{Reglas que no puede violar}
```

### Analisis Critico

**Ventajas:**
- Especializacion clara
- Reutilizables entre proyectos
- Configuracion declarativa

**Desventajas:**
- Overhead de crear/mantener muchos agentes
- Contexto adicional que consume tokens
- Solo util con multiples repos o dominios complejos

### Recomendacion para WePlay Rises

**NO ADOPTAR inicialmente.** Con 1 repo y scope limitado, los agentes no aportan valor suficiente.

**Alternativa:** Usar la funcionalidad nativa de Claude Code sin agentes custom.

**Post-MVP:** Si anadimos repos (Identity, Mobile), crear agentes especificos.

---

## 4. Priority Rules (tasks/priority-rules.yaml)

### Como funciona en miUrba

Sistema de scoring con reglas ponderadas:

```yaml
rules:
  - id: unblocks_teammate
    weight: 50
    applies_when: ["bloquea tarea de otro dev"]

  - id: unblocks_many
    weight: 40
    applies_when: ["blocks.count >= 3"]

  - id: critical_functionality
    weight: 35
    critical_functionalities: [Reservas, Auth]

  - id: quick_win
    weight: 10
    applies_when: ["size in [XS, S]"]

scoring:
  method: sum
  max_possible: 285
```

### Analisis Critico

**Ventajas:**
- Priorizacion objetiva y repetible
- Evita sesgo de "lo que me apetece"
- Documentado y ajustable
- Considera multiples factores

**Desventajas:**
- Configuracion inicial costosa
- Puede generar rigidez
- Requiere calibracion

### Recomendacion para WePlay Rises

**ADOPTAR version simplificada.** La priorizacion objetiva es valiosa incluso para 1 persona.

**Version simplificada para WPR:**

```yaml
# tasks/priority-rules.yaml
metadata:
  version: "1.0"
  description: "Reglas de priorizacion WePlay Rises MVP"

rules:
  - id: critical_for_mvp
    weight: 50
    description: "Bloqueante para demo del MVP"

  - id: unblocks_others
    weight: 30
    description: "Completa esta tarea desbloquea otras"

  - id: quick_win
    weight: 15
    description: "< 2 horas de trabajo"

  - id: user_facing
    weight: 10
    description: "Visible para el usuario final"

scoring:
  method: sum
  max_possible: 105

# Distribución semanal
distribution:
  backend: 50%
  frontend: 40%
  infra: 10%
```

---

## 5. Scripts Python Deterministas

### Como funciona en miUrba

Scripts para operaciones que deben ser siempre iguales:

```
.claude/scripts/
  |-- tasks/
  |     |-- update-task.py       # Actualizar estado
  |     |-- query-tasks.py       # Consultar tareas
  |     |-- task-stats.py        # Estadisticas
  |-- tracking/
  |     |-- validate_tracking.py # Validar consistencia
  |-- validation/
  |     |-- validate_system.py   # Health check completo
  |-- atomic/
  |     |-- atomic_operations.py # Operaciones con rollback
  |     |-- file_lock.py         # Concurrencia
```

### Analisis Critico

**Ventajas:**
- Operaciones repetibles y confiables
- Menos errores que edicion manual
- Audit trail
- Operaciones atomicas con rollback

**Desventajas:**
- Overhead de desarrollo inicial
- Mantenimiento de scripts
- Dependencias Python
- Complejidad no justificada para proyectos pequenos

### Recomendacion para WePlay Rises

**NO ADOPTAR para MVP.** El costo no se justifica con 30 horas disponibles.

**Alternativa simple:**
- Claude Code puede editar YAML directamente
- Usar comandos simples en lugar de scripts complejos
- Si hay errores, corregir manualmente

**Post-MVP:** Evaluar si vale la pena crear scripts de automatizacion.

---

## 6. Sistema de Specs Cross-Repo

### Como funciona en miUrba

Para features que involucran multiples repositorios:

```
plans/specs/{feature-id}/
  |-- feature-spec.md      # QUE (user stories)
  |-- repos-involved.md    # DONDE (repos participantes)
  |-- tasks-breakdown.md   # COMO (tareas por repo)
  |-- decisions.md         # POR QUE (ADRs)
  |-- context.yaml         # Machine-readable
```

Con templates estandarizados y lifecycle de estados.

### Analisis Critico

**Ventajas:**
- Coordinacion clara entre repos
- Documentacion estructurada
- Trazabilidad de decisiones
- Integrado con planificacion

**Desventajas:**
- Solo util con 2+ repos
- Alto overhead de documentacion
- Requiere disciplina de actualizacion

### Recomendacion para WePlay Rises

**NO APLICA inicialmente.** WePlay Rises es 1 solo repositorio.

**Cuando adoptarlo:**
- Si separamos Identity a repo independiente
- Si anadimos app mobile en repo separado
- Si hay equipo de 2+ personas

---

## 7. Health Checks y Diagnostics

### Como funciona en miUrba

Comando `/health-check` que valida:
1. Archivos criticos existen
2. YAML valido
3. Referencias entre archivos
4. Dependencias sin ciclos
5. Estados consistentes

Output con colores y resumen:
```
✅ PASSED: MD ↔ YAML Sync
✅ PASSED: Dependencias
❌ FAILED: Estados inconsistentes

RESULTADO: 2/3 validaciones pasadas
```

### Analisis Critico

**Ventajas:**
- Detecta problemas temprano
- Evita bugs de inconsistencia
- Confianza en el estado del sistema
- Debugging mas rapido

**Desventajas:**
- Requiere scripts de validacion
- Falsos positivos posibles
- Mantenimiento del validador

### Recomendacion para WePlay Rises

**ADOPTAR version basica.** La validacion automatica es valiosa.

**Version minima para WPR:**

```markdown
# Comando: /health-check

Valida consistencia del sistema.

## Validaciones
1. tasks/backlog.yaml es YAML valido
2. Todas las tareas tienen campos requeridos (id, titulo, estado)
3. Dependencias apuntan a tareas existentes
4. No hay ciclos en dependencias
5. Backend compila: dotnet build
6. Frontend compila: npm run build

## Output
✅ YAML valido
✅ Campos completos
✅ Dependencias validas
❌ Ciclo detectado: WPR-001 -> WPR-005 -> WPR-001
```

---

## 8. Estructura CLAUDE.md

### Como funciona en miUrba

Un archivo CLAUDE.md exhaustivo con:
- Vision general del proyecto
- Estructura de carpetas
- Comandos disponibles
- Scripts y su uso
- Variables de entorno
- Flujos de trabajo
- Convenciones
- FAQ

### Analisis Critico

**Ventajas:**
- Claude tiene todo el contexto necesario
- Onboarding rapido
- Referencia unica
- Evita repetir instrucciones

**Desventajas:**
- Archivo muy largo (+500 lineas)
- Facil de desactualizar
- Consume tokens en cada mensaje

### Recomendacion para WePlay Rises

**ADOPTAR version concisa.** Un CLAUDE.md enfocado en MVP.

**Estructura recomendada:**

```markdown
# CLAUDE.md - WePlay Rises

## Proyecto
Plataforma de crowdfunding musical. Arquitectura Modular Monolith (copiada de miUrba).

## Stack
- Backend: .NET 8, EF Core, CQRS
- Frontend: Next.js 14, TypeScript, Tailwind
- DB: SQL Server

## Estructura
src/
  api/           # Backend .NET
  web/           # Frontend Next.js

## Comandos
/daily-plan     # Generar tareas del dia
/done {id}      # Marcar completada
/health-check   # Validar sistema

## Convenciones
- IDs de tarea: WPR-XXX
- Commits: feat:, fix:, docs: en ingles
- Cobertura tests: ≥80%

## Patron CQRS
Handler + Command en mismo archivo.
Handler inyecta: Service, Validator, Mapper.
Handler NUNCA inyecta DbContext.
```

---

## Resumen de Decisiones

### ADOPTAR AHORA

| Practica | Implementacion |
|----------|----------------|
| Priority Rules (simplificado) | `tasks/priority-rules.yaml` con 4 reglas |
| Comandos basicos | `/daily-plan`, `/done`, `/health-check` |
| CLAUDE.md conciso | 100-200 lineas, enfocado en MVP |
| Health Check basico | Validar YAML, dependencias, build |
| Tracking simple | Solo YAML, sin duplicar en MD |

### POSPONER PARA POST-MVP

| Practica | Razon |
|----------|-------|
| Scripts Python | Overhead no justificado para 30h |
| Sistema de Agentes | 1 repo, scope limitado |
| Tracking Dual | 1 persona, sin equipo |
| Specs Cross-Repo | 1 repositorio |

### NO ADOPTAR

| Practica | Razon |
|----------|-------|
| Complejidad de miUrba | Proyecto maduro de 10 anos vs MVP de 30h |
| Feature Spec Generator | Overkill para 5 user stories |
| Sincronizacion MD↔YAML | Fuente de bugs sin beneficio |

---

## Plan de Implementacion

### Fase 1: Setup Basico (1h)
1. Crear `CLAUDE.md` conciso
2. Crear `tasks/priority-rules.yaml`
3. Crear `tasks/backlog.yaml`

### Fase 2: Comandos (30min)
1. Crear `.claude/commands/planning/done.md`
2. Crear `.claude/commands/planning/daily-plan.md`

### Fase 3: Validacion (30min)
1. Crear `.claude/commands/diagnostics/health-check.md`

**Total: 2 horas de setup inicial.**

---

## Conclusiones

El workspace de miUrba es un **excelente ejemplo de sistema maduro** pero sufre de:
1. **Over-engineering** para proyectos pequenos
2. **Complejidad acumulada** de anos de evolucion
3. **Casos especiales** que no aplican a WPR

**Lecciones clave:**
- Empezar simple, complejizar cuando haya dolor real
- Priorizar valor entregado sobre infraestructura perfecta
- La documentacion excesiva tiene costo de mantenimiento
- 30 horas no es tiempo para construir frameworks

**WePlay Rises debe ser pragmatico:** tomar las ideas de valor alto, implementar versiones minimas, y iterar si el proyecto crece.
