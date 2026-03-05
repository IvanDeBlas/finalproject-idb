# Paradigma de Programacion Agentica con Claude Code v0

- **Fecha**: 2026-01-23
- **Proyecto**: WePlay Rises
- **Stack**: .NET 8 (Monolito Modular) + React (Vite + Next.js) + SQL Server
- **CI/CD**: Azure DevOps Pipelines
- **Autor**: Ivan / Claude Code

---

## Vision General

Este documento define un **paradigma de programacion agentica** que maximiza la productividad al combinar:

1. **Hooks eficientes** - Automatizacion silenciosa de tareas repetitivas
2. **Templates inteligentes** - Generacion de codigo con buenas practicas incorporadas
3. **Comandos unificados** - Un comando, multiples agentes especializados
4. **TDD con datos sinteticos** - Tests antes de codigo con datos realistas# Paradigma de Programacion Agentica con Claude Code v1.0

- **Fecha**: 2026-01-23
- **Proyecto**: WePlay Rises
- **Stack**: .NET 8 (Monolito Modular) + React (Vite + Next.js) + SQL Server
- **CI/CD**: Azure DevOps Pipelines
- **Autor**: Ivan / Claude Code

---

## Vision General

Este documento define un **paradigma de programacion agentica** que maximiza la productividad al combinar:

1. **Hooks eficientes** - Automatizacion silenciosa de tareas repetitivas
2. **Templates inteligentes** - Generacion de codigo con buenas practicas incorporadas
3. **Comandos unificados** - Un comando, multiples agentes especializados
4. **TDD con datos sinteticos** - Tests antes de codigo con datos realistas
5. **Diagnostico y auto-resolucion** - Claude detecta y corrige errores autonomamente
6. **UxPilot integrado** - UI profesional desde el inicio

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        PARADIGMA DE DESARROLLO AGENTICO                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   /spec "feature"        ──►  ESPECIFICACION                                │
│        │                      (Multiples agentes en paralelo)               │
│        │                      • Arquitectura • DTOs • Tests • Diag • E2E    │
│        ▼                                                                    │
│   /synth-data            ──►  DATOS SINTETICOS                              │
│        │                      (Seeds + Fixtures + Mocks)                    │
│        ▼                                                                    │
│   /implement             ──►  IMPLEMENTACION                                │
│        │                      (Worktree aislado + TDD)                      │
│        ▼                                                                    │
│   /diagnose              ──►  DIAGNOSTICO + AUTO-FIX                        │
│        │                      (Tests + DB + Logs + Auto-resolve)            │
│        ▼                                                                    │
│   /ship                  ──►  PIPELINE + PR                                 │
│                               (Cleanup + CI + PR automatica)                │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## PARTE 1: ESTRUCTURA DEL PROYECTO

### Estructura de Directorios Claude Code

```
WePlay_Rises/
├── .claude/
│   ├── CLAUDE.md                      # Instrucciones principales
│   ├── settings.local.json            # Variables de entorno
│   │
│   ├── hooks/                         # Automatizacion silenciosa
│   │   ├── settings.json              # Configuracion de hooks
│   │   ├── session-persistence/       # Persistencia entre sesiones
│   │   ├── quality-gates/             # Validacion automatica
│   │   └── strategic-compact/         # Gestion de contexto
│   │
│   ├── agents/                        # Agentes especializados
│   │   ├── spec-architect.agent.md    # Arquitectura y DTOs
│   │   ├── spec-tester.agent.md       # Tests unitarios e integracion
│   │   ├── spec-e2e.agent.md          # Tests E2E y flujos
│   │   ├── spec-diagnostic.agent.md   # Puntos de diagnostico
│   │   ├── implement-backend.agent.md # Implementacion .NET
│   │   ├── implement-frontend.agent.md# Implementacion React
│   │   ├── diagnose-resolver.agent.md # Diagnostico y auto-fix
│   │   └── ship-reviewer.agent.md     # Review final y PR
│   │
│   ├── commands/                      # Comandos unificados
│   │   ├── spec.md                    # /spec - Especificacion completa
│   │   ├── synth-data.md              # /synth-data - Datos sinteticos
│   │   ├── implement.md               # /implement - Worktree + codigo
│   │   ├── diagnose.md                # /diagnose - Diagnostico + fix
│   │   └── ship.md                    # /ship - Pipeline + PR
│   │
│   ├── templates/                     # Templates de codigo
│   │   ├── backend/
│   │   │   ├── command.template.cs
│   │   │   ├── query.template.cs
│   │   │   ├── controller.template.cs
│   │   │   └── entity.template.cs
│   │   ├── frontend/
│   │   │   ├── component.template.tsx
│   │   │   ├── page.template.tsx
│   │   │   ├── hook.template.ts
│   │   │   └── service.template.ts
│   │   └── tests/
│   │       ├── unit.template.cs
│   │       ├── integration.template.cs
│   │       ├── e2e.template.ts
│   │       └── fixture.template.json
│   │
│   ├── rules/                         # Reglas contextuales
│   │   ├── _index.yaml
│   │   ├── always/
│   │   │   ├── code-style.rule.md
│   │   │   └── git-conventions.rule.md
│   │   ├── backend/
│   │   │   ├── cqrs-commands.rule.md
│   │   │   └── ef-core.rule.md
│   │   └── frontend/
│   │       ├── react-components.rule.md
│   │       └── tanstack-query.rule.md
│   │
│   └── contexts/                      # Modos de trabajo
│       ├── dev.md                     # Modo desarrollo
│       ├── review.md                  # Modo revision
│       └── research.md                # Modo investigacion
│
├── specs/                             # Especificaciones generadas
│   └── {feature-id}/
│       ├── architecture.md            # Arquitectura y DTOs
│       ├── tests-unit.md              # Plan tests unitarios
│       ├── tests-e2e.md               # Plan tests E2E
│       ├── diagnostic-points.md       # Puntos de diagnostico
│       └── synthetic-data.yaml        # Datos sinteticos
│
├── tasks/                             # Tracking de tareas
│   ├── backlog.md                     # Backlog general
│   └── active-feature.yaml            # Feature en progreso
│
└── .worktrees/                        # Worktrees aislados (gitignore)
```

---

## PARTE 2: SISTEMA DE HOOKS

### Configuracion de Hooks

```json
// .claude/hooks/settings.json
{
  "hooks": [
    {
      "name": "load-project-context",
      "trigger": "SessionStart",
      "script": ".claude/hooks/session-persistence/session-start.py",
      "enabled": true,
      "description": "Carga contexto del proyecto y sesiones anteriores"
    },
    {
      "name": "load-contextual-rules",
      "trigger": "PreToolUse",
      "tools": ["Read", "Edit", "Write"],
      "script": ".claude/hooks/rules/load-contextual-rules.py",
      "enabled": true,
      "description": "Carga reglas segun tipo de archivo editado"
    },
    {
      "name": "auto-format",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/auto-format.py",
      "enabled": true,
      "description": "Formatea codigo automaticamente"
    },
    {
      "name": "tdd-reminder",
      "trigger": "PreToolUse",
      "tools": ["Write"],
      "script": ".claude/hooks/quality-gates/tdd-reminder.py",
      "enabled": true,
      "description": "Recuerda escribir tests antes de codigo de produccion"
    },
    {
      "name": "detect-secrets",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/detect-secrets.py",
      "enabled": true,
      "blocksOnFailure": true,
      "description": "BLOQUEA si detecta secrets hardcodeados"
    },
    {
      "name": "db-query-observer",
      "trigger": "PostToolUse",
      "tools": ["Bash"],
      "script": ".claude/hooks/diagnostic/db-query-observer.py",
      "enabled": true,
      "description": "Captura queries SQL ejecutadas"
    },
    {
      "name": "strategic-compact",
      "trigger": "PreToolUse",
      "script": ".claude/hooks/strategic-compact/suggest-compact.py",
      "enabled": true,
      "description": "Sugiere compactar cada 50 herramientas"
    },
    {
      "name": "session-end-persistence",
      "trigger": "Stop",
      "script": ".claude/hooks/session-persistence/session-end.py",
      "enabled": true,
      "description": "Guarda estado y patrones aprendidos"
    }
  ]
}
```

### Hook: TDD Reminder

```python
#!/usr/bin/env python3
"""
Hook: tdd-reminder
Trigger: PreToolUse (Write)
Recuerda escribir tests antes de codigo de produccion.
"""

import sys
import json
import os

def main():
    # Leer input del hook
    hook_input = json.loads(sys.stdin.read())
    file_path = hook_input.get('file_path', '')

    # Ignorar archivos de test
    test_patterns = ['.test.', '.spec.', 'Test.cs', 'Tests.cs', '__tests__']
    if any(p in file_path for p in test_patterns):
        return {"status": "pass"}

    # Ignorar templates, configs, etc
    ignore_patterns = ['.md', '.json', '.yaml', '.yml', '.config', 'template']
    if any(p in file_path for p in ignore_patterns):
        return {"status": "pass"}

    # Verificar si existe archivo de test correspondiente
    test_file = get_test_file_path(file_path)

    if not os.path.exists(test_file):
        return {
            "status": "warn",
            "message": f"""
╔══════════════════════════════════════════════════════════════════╗
║  [TDD] RECUERDA: Test-First Development                          ║
╠══════════════════════════════════════════════════════════════════╣
║                                                                  ║
║  Estas creando: {os.path.basename(file_path)[:40]}               ║
║                                                                  ║
║  RECOMENDACION:                                                  ║
║  1. Primero crea el test en: {os.path.basename(test_file)[:30]}  ║
║  2. Haz que el test falle                                        ║
║  3. Luego implementa el codigo                                   ║
║                                                                  ║
║  Usa: /synth-data para generar datos de prueba                   ║
║                                                                  ║
╚══════════════════════════════════════════════════════════════════╝
"""
        }

    return {"status": "pass"}

def get_test_file_path(file_path: str) -> str:
    """Calcula la ruta del archivo de test correspondiente."""
    if file_path.endswith('.cs'):
        # .NET: UserService.cs -> UserServiceTests.cs
        return file_path.replace('.cs', 'Tests.cs')
    elif file_path.endswith('.ts') or file_path.endswith('.tsx'):
        # React: UserCard.tsx -> UserCard.test.tsx
        ext = '.tsx' if file_path.endswith('.tsx') else '.ts'
        return file_path.replace(ext, f'.test{ext}')
    return file_path + '.test'

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

### Hook: DB Query Observer

```python
#!/usr/bin/env python3
"""
Hook: db-query-observer
Trigger: PostToolUse (Bash)
Captura queries SQL ejecutadas para diagnostico.
"""

import sys
import json
import re
from datetime import datetime
from pathlib import Path

# Archivo donde se guardan las queries capturadas
QUERIES_LOG = Path('.claude/diagnostic/queries.log')

def main():
    hook_input = json.loads(sys.stdin.read())

    command = hook_input.get('command', '')
    output = hook_input.get('output', '')

    # Solo capturar si es dotnet run o test
    if 'dotnet' not in command:
        return {"status": "pass"}

    # Buscar queries SQL en el output
    queries = extract_sql_queries(output)

    if queries:
        save_queries(queries, command)

        # Alertar sobre queries lentas
        slow_queries = [q for q in queries if q.get('duration_ms', 0) > 100]
        if slow_queries:
            return {
                "status": "warn",
                "message": f"[DB] {len(slow_queries)} queries lentas detectadas (>100ms)"
            }

    return {"status": "pass"}

def extract_sql_queries(output: str) -> list:
    """Extrae queries SQL del output de EF Core."""
    queries = []

    # Patron para logs de EF Core
    ef_pattern = r'\[(\d+)ms\]\s*(SELECT|INSERT|UPDATE|DELETE).*?(?=\n\[|\Z)'

    for match in re.finditer(ef_pattern, output, re.DOTALL | re.IGNORECASE):
        duration = int(match.group(1))
        query = match.group(0)
        queries.append({
            'timestamp': datetime.now().isoformat(),
            'duration_ms': duration,
            'query': query[:500]  # Truncar queries largas
        })

    return queries

def save_queries(queries: list, context: str):
    """Guarda queries en log para analisis."""
    QUERIES_LOG.parent.mkdir(parents=True, exist_ok=True)

    with open(QUERIES_LOG, 'a', encoding='utf-8') as f:
        f.write(f"\n=== {datetime.now().isoformat()} | {context[:50]} ===\n")
        for q in queries:
            f.write(f"[{q['duration_ms']}ms] {q['query']}\n")

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

---

## PARTE 3: COMANDO /spec - ESPECIFICACION UNIFICADA

### Descripcion

Un solo comando que orquesta **multiples agentes en paralelo** para generar la especificacion completa de una feature:

```bash
/spec "Crear campania de crowdfunding"
```

### Workflow del Comando /spec

```
/spec "Crear campania de crowdfunding"
        │
        ├─► [PARALELO] @spec-architect
        │       └─► specs/{feature-id}/architecture.md
        │           • DTOs de request/response
        │           • Endpoints API
        │           • Entidades y relaciones
        │           • Modulos afectados
        │
        ├─► [PARALELO] @spec-tester
        │       └─► specs/{feature-id}/tests-unit.md
        │           • Casos de test unitarios
        │           • Mocks requeridos
        │           • Cobertura esperada
        │
        ├─► [PARALELO] @spec-e2e
        │       └─► specs/{feature-id}/tests-e2e.md
        │           • Flujos E2E (Playwright)
        │           • Escenarios happy path
        │           • Escenarios de error
        │
        ├─► [PARALELO] @spec-diagnostic
        │       └─► specs/{feature-id}/diagnostic-points.md
        │           • Puntos de logging (#DIAG)
        │           • Metricas a capturar
        │           • Alertas esperadas
        │
        └─► [SECUENCIAL] SINTESIS
                └─► specs/{feature-id}/SYNTHESIS.md
                    • Resumen ejecutivo
                    • Checklist de implementacion
                    • Dependencias entre tareas
```

### Archivo de Comando: /spec

```markdown
<!-- .claude/commands/spec.md -->
# Comando: /spec

## Proposito
Genera especificacion completa de una feature usando multiples agentes en paralelo.

## Uso
```
/spec "descripcion de la feature"
/spec "descripcion" --skip-e2e     # Sin tests E2E
/spec "descripcion" --quick        # Solo arquitectura
```

## Proceso

### Fase 1: Validacion (5 segundos)
1. Parsear la descripcion de la feature
2. Generar feature-id unico (formato: `feat-YYYYMMDD-slug`)
3. Crear directorio `specs/{feature-id}/`

### Fase 2: Agentes en Paralelo (2-5 minutos)

Lanzar 4 agentes simultaneamente:

```
Task subagent_type=spec-architect
Task subagent_type=spec-tester
Task subagent_type=spec-e2e
Task subagent_type=spec-diagnostic
```

Cada agente recibe:
- Descripcion de la feature
- Contexto del proyecto (CLAUDE.md)
- Stack tecnologico (de README.md)
- Entidades existentes (de src/api/Modules/)

### Fase 3: Sintesis (1 minuto)
1. Esperar a todos los agentes
2. Leer outputs de cada agente
3. Generar SYNTHESIS.md con:
   - Resumen ejecutivo
   - Checklist de tareas
   - Orden de implementacion (backend → frontend → tests)
   - Estimacion de complejidad

### Fase 4: Output
1. Mostrar resumen en consola
2. Indicar ruta a especificacion completa
3. Preguntar si proceder con /synth-data

## Agentes Involucrados

| Agente | Output | Tiempo |
|--------|--------|--------|
| @spec-architect | architecture.md | ~1 min |
| @spec-tester | tests-unit.md | ~1 min |
| @spec-e2e | tests-e2e.md | ~1 min |
| @spec-diagnostic | diagnostic-points.md | ~30s |

## Output Ejemplo

```
✅ ESPECIFICACION COMPLETADA: feat-20260123-crear-campania

📁 Archivos generados:
   specs/feat-20260123-crear-campania/
   ├── architecture.md      (DTOs, endpoints, entidades)
   ├── tests-unit.md        (15 test cases)
   ├── tests-e2e.md         (3 flujos E2E)
   ├── diagnostic-points.md (8 puntos de diagnostico)
   └── SYNTHESIS.md         (resumen y checklist)

📋 Checklist de Implementacion:
   [ ] 1. Backend: CreateCampaignCommand + Handler
   [ ] 2. Backend: GetCampaignQuery + Handler
   [ ] 3. Backend: CampaniaController endpoints
   [ ] 4. Frontend: useCampanias hook
   [ ] 5. Frontend: CampaignForm component
   [ ] 6. Tests: Unit tests backend
   [ ] 7. Tests: E2E flow
   [ ] 8. Diagnostico: Puntos de logging

⏭️  Siguiente paso: /synth-data feat-20260123-crear-campania
```
```

### Agente: spec-architect

```markdown
<!-- .claude/agents/spec-architect.agent.md -->
# Agente: spec-architect

## Rol
Arquitecto de software que diseña la estructura tecnica de features.

## Input
- Descripcion de la feature
- Entidades existentes en el proyecto
- Stack tecnologico

## Output: architecture.md

### Estructura del Output

```markdown
# Arquitectura: {feature-name}

## 1. Resumen
{descripcion de alto nivel}

## 2. Endpoints API

### POST /api/campanias
- **Descripcion**: Crear nueva campania
- **Auth**: JWT (role: Artist)
- **Request Body**:
```json
{
  "titulo": "string",
  "descripcion": "string",
  "importeObjetivo": "decimal",
  "fechaInicio": "date",
  "fechaFin": "date"
}
- **Response 201**:
```json
{
  "id": "guid",
  "titulo": "string",
  "estado": "Borrador"
}
- **Errores**: 400 (validacion), 401 (no auth), 403 (no artist)

## 3. DTOs

### CreateCampaignRequest
| Campo | Tipo | Validacion |
|-------|------|------------|
| titulo | string | required, max 200 |
| descripcion | string | required, max 2000 |
| importeObjetivo | decimal | > 0 |

### CampaignResponse
| Campo | Tipo |
|-------|------|
| id | Guid |
| titulo | string |
| estado | CampaignStatus |

## 4. Entidades

### Campania (modificaciones)
- Nuevos campos: ninguno
- Nuevas relaciones: ninguna

## 5. CQRS

### Commands
- CreateCampaignCommand
- UpdateCampaignCommand
- PublishCampaignCommand

### Queries
- GetCampaignByIdQuery
- GetCampaignsByArtistaQuery
- GetPublicCampaignsQuery

## 6. Modulos Afectados
- Crowdfunding (principal)
- UserAccess (validar ArtistaId)

## 7. Dependencias
- Requiere: ArtistaId del usuario autenticado
- Bloquea: RewardController necesita CampaignId

## Reglas

1. **CQRS obligatorio**: Siempre separar Commands y Queries
2. **DTOs explicitos**: Nunca exponer entidades directamente
3. **Validacion en Commands**: FluentValidation en cada command
4. **Errores tipados**: Usar Result<T> pattern
5. **Sin logica en Controllers**: Solo mapeo y delegacion a MediatR
```

---

## PARTE 4: COMANDO /synth-data - DATOS SINTETICOS

### Descripcion

Genera datos sinteticos realistas para testing basandose en la especificacion.

```bash
/synth-data feat-20260123-crear-campania
```

### Workflow

```
/synth-data {feature-id}
        │
        ├─► Lee specs/{feature-id}/architecture.md
        │
        ├─► Genera:
        │   ├─► specs/{feature-id}/synthetic-data.yaml
        │   │       (datos para seeds y fixtures)
        │   │
        │   ├─► src/api/Tests/Fixtures/{Feature}Fixtures.cs
        │   │       (fixtures C# para xUnit)
        │   │
        │   ├─► src/web/__fixtures__/{feature}.json
        │   │       (mocks para frontend)
        │   │
        │   └─► src/api/Seeds/{Feature}Seed.cs
        │           (seeds para desarrollo)
        │
        └─► Output: Resumen de datos generados
```

### Archivo de Comando: /synth-data

```markdown
<!-- .claude/commands/synth-data.md -->
# Comando: /synth-data

## Proposito
Genera datos sinteticos realistas para testing y desarrollo.

## Uso
```
/synth-data {feature-id}
/synth-data {feature-id} --count 50    # 50 registros
/synth-data {feature-id} --locale es   # Datos en espanol
```

## Proceso

### Fase 1: Analisis (10 segundos)
1. Leer architecture.md de la feature
2. Identificar entidades y relaciones
3. Determinar volumen de datos

### Fase 2: Generacion de Datos

Para cada entidad identificada, generar:

1. **synthetic-data.yaml** - Definicion de datos
```yaml
campania:
  count: 20
  fields:
    titulo:
      type: music_project_name
      unique: true
    descripcion:
      type: lorem
      paragraphs: 2
    importeObjetivo:
      type: random_decimal
      min: 1000
      max: 50000
    estado:
      type: enum
      values: [Borrador, Activa, Finalizada, Cancelada]
      weights: [0.2, 0.5, 0.2, 0.1]
    fechaInicio:
      type: date_future
      days: 30
    fechaFin:
      type: date_after
      field: fechaInicio
      days_after: 60

  relationships:
    artista:
      type: belongs_to
      entity: artista
      required: true

artista:
  count: 10
  fields:
    nombreArtistico:
      type: band_name
    descripcion:
      type: artist_bio
```

2. **C# Fixtures** - Para tests de integracion
```csharp
public static class CampaniaFixtures
{
    public static Campania ValidBorrador() => new()
    {
        Id = Guid.NewGuid(),
        Titulo = "Album Debut - Los Rebeldes",
        Descripcion = "Nuestro primer album necesita tu apoyo...",
        ImporteObjetivo = 5000m,
        Estado = CampaignStatus.Borrador,
        ArtistaId = ArtistaFixtures.ValidArtista().Id
    };

    public static Campania ValidActiva() => ValidBorrador() with
    {
        Estado = CampaignStatus.Activa,
        FechaInicio = DateTime.UtcNow.AddDays(-10),
        FechaFin = DateTime.UtcNow.AddDays(50)
    };
}
```

3. **JSON Mocks** - Para frontend
```json
{
  "campanias": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "titulo": "Album Debut - Los Rebeldes",
      "descripcion": "Nuestro primer album...",
      "importeObjetivo": 5000,
      "importeRecaudado": 2350,
      "estado": "Activa",
      "backers": 47,
      "diasRestantes": 45
    }
  ]
}
```

### Fase 3: Seeds de Desarrollo

```csharp
// src/api/Seeds/CrowdfundingSeed.cs
public class CrowdfundingSeed : ISeedData
{
    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.Campanias.AnyAsync()) return;

        // Usar datos de synthetic-data.yaml
        var campanias = GenerateFromYaml("specs/.../synthetic-data.yaml");
        await context.Campanias.AddRangeAsync(campanias);
        await context.SaveChangesAsync();
    }
}
```

## Output Ejemplo

```
✅ DATOS SINTETICOS GENERADOS: feat-20260123-crear-campania

📊 Datos generados:
   • 10 Artistas
   • 20 Campanias
   • 50 Rewards
   • 100 Backings

📁 Archivos creados:
   specs/feat-20260123-crear-campania/synthetic-data.yaml
   src/api/Tests/Fixtures/CampaniaFixtures.cs
   src/web/__fixtures__/campanias.json
   src/api/Seeds/CrowdfundingSeed.cs

⏭️  Siguiente paso: /implement feat-20260123-crear-campania
```
```

---

## PARTE 5: COMANDO /implement - WORKTREE + IMPLEMENTACION

### Descripcion

Crea un worktree aislado e implementa la feature siguiendo TDD.

```bash
/implement feat-20260123-crear-campania
```

### Workflow

```
/implement {feature-id}
        │
        ├─► Crear worktree aislado
        │   └─► git worktree add .worktrees/{feature-id} -b feature/{feature-id}
        │
        ├─► Copiar specs al worktree
        │
        ├─► CICLO TDD (por cada item del checklist):
        │   │
        │   ├─► 1. RED: Escribir test que falla
        │   │       └─► Usar fixtures de /synth-data
        │   │
        │   ├─► 2. GREEN: Implementar codigo minimo
        │   │       └─► Usar templates de .claude/templates/
        │   │
        │   └─► 3. REFACTOR: Limpiar codigo
        │           └─► Hook auto-format se ejecuta
        │
        ├─► Inyectar puntos de diagnostico (#DIAG)
        │
        └─► Output: Resumen de implementacion
```

### Archivo de Comando: /implement

```markdown
<!-- .claude/commands/implement.md -->
# Comando: /implement

## Proposito
Implementa una feature en un worktree aislado siguiendo TDD.

## Uso
```
/implement {feature-id}
/implement {feature-id} --backend-only    # Solo API
/implement {feature-id} --frontend-only   # Solo React
/implement {feature-id} --resume          # Continuar implementacion
```

## Proceso

### Fase 1: Setup Worktree (30 segundos)

```bash
# Crear rama y worktree
git worktree add .worktrees/{feature-id} -b feature/{feature-id}

# Copiar specs
cp -r specs/{feature-id} .worktrees/{feature-id}/specs/

# Navegar al worktree
cd .worktrees/{feature-id}
```

### Fase 2: Implementacion Backend (TDD)

Para cada Command/Query en architecture.md:

```
1. RED - Escribir test
   └─► src/api/Tests/Crowdfunding/CreateCampaignCommandTests.cs

2. GREEN - Implementar handler
   └─► src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommand.cs
   └─► src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs

3. REFACTOR - Limpiar
   └─► dotnet format
```

**Template de Command:**
```csharp
// Generado desde .claude/templates/backend/command.template.cs
public record CreateCampaignCommand(
    string Titulo,
    string Descripcion,
    decimal ImporteObjetivo,
    DateTime FechaInicio,
    DateTime FechaFin
) : IRequest<Result<CampaignResponse>>;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .MaximumLength(200);
        // ... mas reglas
    }
}

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<CampaignResponse>>
{
    // #DIAG:create-campaign-entry
    public async Task<Result<CampaignResponse>> Handle(
        CreateCampaignCommand request,
        CancellationToken ct)
    {
        // Implementacion...
        // #DIAG:create-campaign-exit
    }
}
```

### Fase 3: Implementacion Frontend (TDD)

Para cada componente en architecture.md:

```
1. RED - Escribir test
   └─► src/web/src/features/campaigns/__tests__/CampaignForm.test.tsx

2. GREEN - Implementar componente
   └─► src/web/src/features/campaigns/components/CampaignForm.tsx

3. REFACTOR - Limpiar
   └─► npm run lint:fix
```

**Template de Componente:**
```tsx
// Generado desde .claude/templates/frontend/component.template.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { createCampaignSchema, type CreateCampaignInput } from '@/shared/schemas';

interface CampaignFormProps {
  onSubmit: (data: CreateCampaignInput) => void;
  isLoading?: boolean;
}

export function CampaignForm({ onSubmit, isLoading }: CampaignFormProps) {
  const form = useForm<CreateCampaignInput>({
    resolver: zodResolver(createCampaignSchema),
  });

  return (
    <form onSubmit={form.handleSubmit(onSubmit)}>
      {/* Campos del formulario */}
    </form>
  );
}
```

### Fase 4: Puntos de Diagnostico

Inyectar automaticamente segun diagnostic-points.md:

```csharp
// Los #DIAG se inyectan en puntos clave
public async Task<Result<CampaignResponse>> Handle(...)
{
    // #DIAG:create-campaign-entry | timestamp={timestamp} | userId={userId}

    var campania = await _repository.CreateAsync(...);

    // #DIAG:create-campaign-db-saved | campaniaId={campania.Id} | duration={duration}ms

    return Result.Success(response);
    // #DIAG:create-campaign-exit-success
}
```

## Output Ejemplo

```
✅ IMPLEMENTACION COMPLETADA: feat-20260123-crear-campania

📁 Worktree: .worktrees/feat-20260123-crear-campania
📊 Branch: feature/feat-20260123-crear-campania

Backend:
   ✅ CreateCampaignCommand + Handler + Validator
   ✅ CreateCampaignCommandTests (5 tests passing)
   ✅ CampaniaController.Post()
   ✅ 8 puntos #DIAG inyectados

Frontend:
   ✅ CampaignForm component
   ✅ useCampaigns hook
   ✅ CampaignForm.test.tsx (3 tests passing)

📈 Cobertura:
   Backend: 87%
   Frontend: 82%

⏭️  Siguiente paso: /diagnose feat-20260123-crear-campania
```
```

---

## PARTE 6: COMANDO /diagnose - DIAGNOSTICO + AUTO-FIX

### Descripcion

Ejecuta tests, analiza resultados, y corrige errores automaticamente.

```bash
/diagnose feat-20260123-crear-campania
```

### Workflow

```
/diagnose {feature-id}
        │
        ├─► Ejecutar tests unitarios
        │   └─► dotnet test
        │
        ├─► Ejecutar tests E2E
        │   └─► npx playwright test
        │
        ├─► Capturar queries SQL (via MCP DB Server)
        │   └─► Analizar queries lentas
        │
        ├─► Analizar logs #DIAG
        │   └─► Correlacionar eventos
        │
        ├─► Si hay errores:
        │   ├─► @diagnose-resolver analiza
        │   ├─► Propone fix
        │   ├─► Aplica fix automaticamente (si es seguro)
        │   └─► Re-ejecuta tests
        │
        └─► Output: Reporte de diagnostico
```

### Archivo de Comando: /diagnose

```markdown
<!-- .claude/commands/diagnose.md -->
# Comando: /diagnose

## Proposito
Ejecuta diagnostico completo y corrige errores automaticamente.

## Uso
```
/diagnose {feature-id}
/diagnose {feature-id} --no-auto-fix     # Solo diagnostico
/diagnose {feature-id} --verbose         # Detalle completo
```

## Proceso

### Fase 1: Ejecutar Tests (1-3 minutos)

```bash
# Tests unitarios backend
cd .worktrees/{feature-id}
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"

# Tests unitarios frontend
cd src/web
npm test -- --coverage --json > test-results.json

# Tests E2E
cd src/web
npx playwright test --reporter=json > e2e-results.json
```

### Fase 2: Capturar Queries SQL

Usando MCP DB Server:

```typescript
// Queries ejecutadas durante tests
const recentQueries = await mcp_db_diagnostic.db_recent_queries({
  seconds: 300,  // Ultimos 5 minutos
  minDurationMs: 0
});

// Queries lentas
const slowQueries = await mcp_db_diagnostic.db_slow_queries({
  thresholdMs: 100
});
```

### Fase 3: Analizar Resultados

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                          DIAGNOSTICO: feat-20260123                          │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  TESTS UNITARIOS BACKEND                                                     │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ CreateCampaignCommandTests.Should_Create_Valid_Campaign         PASS    │
│  ✅ CreateCampaignCommandTests.Should_Fail_Empty_Title              PASS    │
│  ❌ CreateCampaignCommandTests.Should_Fail_Past_StartDate           FAIL    │
│     └─► Expected: ValidationException                                        │
│     └─► Actual: Success (validacion no implementada)                         │
│                                                                              │
│  TESTS E2E                                                                   │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ create-campaign.spec.ts: Happy path                             PASS    │
│  ✅ create-campaign.spec.ts: Validation errors                      PASS    │
│                                                                              │
│  QUERIES SQL                                                                 │
│  ─────────────────────────────────────────────────────────────────────────   │
│  📊 Total queries: 45                                                       │
│  ⚠️  Slow queries (>100ms): 2                                               │
│     └─► SELECT * FROM Campanias WHERE ArtistaId = @p0  (234ms)               │
│         SUGERENCIA: Agregar indice en ArtistaId                              │
│                                                                              │
│  PUNTOS #DIAG                                                                │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ create-campaign-entry: 15 invocaciones                                  │
│  ✅ create-campaign-db-saved: 12 invocaciones                               │
│  ✅ create-campaign-exit-success: 12 invocaciones                           │
│  ⚠️  create-campaign-exit-error: 3 invocaciones                             │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### Fase 4: Auto-Fix

El agente @diagnose-resolver analiza cada error:

```
ERROR DETECTADO:
  Test: Should_Fail_Past_StartDate
  Tipo: Validacion faltante
  Severidad: Media

ANALISIS:
  El CommandValidator no tiene regla para FechaInicio > DateTime.UtcNow

FIX PROPUESTO:
  Agregar en CreateCampaignCommandValidator:
  RuleFor(x => x.FechaInicio).GreaterThan(DateTime.UtcNow)

APLICANDO FIX...
  ✅ Archivo modificado: CreateCampaignCommandValidator.cs
  ✅ Re-ejecutando test...
  ✅ Test ahora pasa
```

### Fase 5: Sugerencias de Optimizacion

```
SUGERENCIAS DE OPTIMIZACION:

1. [INDICE] Agregar indice en Campanias.ArtistaId
   └─► Mejoraria query de 234ms a ~10ms
   └─► SQL: CREATE INDEX IX_Campanias_ArtistaId ON Campanias(ArtistaId)

2. [N+1] Detectado posible N+1 en GetCampaignsByArtistaQuery
   └─► Usar .Include(c => c.Rewards) para cargar en una query

3. [COBERTURA] CampaignForm tiene 82% cobertura
   └─► Faltan tests para: error handling, loading state
```

## Output Ejemplo

```
✅ DIAGNOSTICO COMPLETADO: feat-20260123-crear-campania

📊 Resultados:
   Tests Backend: 15/15 ✅ (1 auto-fixed)
   Tests Frontend: 8/8 ✅
   Tests E2E: 3/3 ✅
   Cobertura: 89%

🔧 Auto-Fixes Aplicados:
   1. ✅ Validacion FechaInicio en CreateCampaignCommandValidator

⚠️  Sugerencias Pendientes:
   1. Agregar indice en Campanias.ArtistaId
   2. Resolver posible N+1 en GetCampaignsByArtistaQuery

⏭️  Siguiente paso: /ship feat-20260123-crear-campania
```
```

---

## PARTE 7: COMANDO /ship - PIPELINE + PR

### Descripcion

Limpia el codigo, ejecuta la pipeline de CI, y crea el PR.

```bash
/ship feat-20260123-crear-campania
```

### Workflow

```
/ship {feature-id}
        │
        ├─► Limpiar codigo #DIAG (opcional)
        │   └─► Remover o comentar puntos de diagnostico
        │
        ├─► Ejecutar validaciones finales
        │   ├─► dotnet build --no-restore
        │   ├─► dotnet test
        │   ├─► npm run lint
        │   └─► npm run type-check
        │
        ├─► Commit y Push
        │   └─► git commit -m "feat(crowdfunding): implement create campaign"
        │   └─► git push origin feature/{feature-id}
        │
        ├─► Crear PR en Azure DevOps
        │   └─► az repos pr create --title "..." --description "..."
        │
        └─► Limpiar worktree
            └─► git worktree remove .worktrees/{feature-id}
```

### Archivo de Comando: /ship

```markdown
<!-- .claude/commands/ship.md -->
# Comando: /ship

## Proposito
Prepara, valida, y crea PR para una feature completada.

## Uso
```
/ship {feature-id}
/ship {feature-id} --keep-diag       # Mantener puntos #DIAG
/ship {feature-id} --draft           # PR como draft
/ship {feature-id} --skip-cleanup    # No eliminar worktree
```

## Proceso

### Fase 1: Limpieza de Codigo (30 segundos)

```python
# Remover/comentar puntos #DIAG
for file in find_files_with_diag():
    content = file.read_text()
    # Comentar lineas #DIAG para produccion
    content = re.sub(
        r'(\s*)(.*#DIAG:.*)',
        r'\1// \2  // TODO: Enable in debug',
        content
    )
    file.write_text(content)
```

### Fase 2: Validaciones Finales (1-2 minutos)

```bash
# Backend
dotnet build --no-restore -c Release
dotnet test --no-build -c Release

# Frontend
npm run lint
npm run type-check
npm run build
```

### Fase 3: Commit y Push

```bash
# Generar mensaje de commit automatico
commit_msg=$(generate_commit_message specs/{feature-id}/SYNTHESIS.md)

# Commit
git add .
git commit -m "$commit_msg"

# Push
git push origin feature/{feature-id}
```

**Formato de Commit:**
```
feat(crowdfunding): implement create campaign

- Add CreateCampaignCommand with validation
- Add CampaignForm React component
- Add unit tests (15) and E2E tests (3)
- Coverage: 89%

Closes #WPR-007

Co-Authored-By: Claude Code <claude@anthropic.com>
```

### Fase 4: Crear PR en Azure DevOps

```bash
az repos pr create \
  --title "feat(crowdfunding): implement create campaign" \
  --description "$(cat pr-description.md)" \
  --source-branch "feature/{feature-id}" \
  --target-branch "main" \
  --reviewers "jonay@email.com" \
  --work-items "WPR-007"
```

**Template de PR:**
```markdown
## Summary
- Implementacion de creacion de campanias de crowdfunding
- Incluye validaciones, tests, y puntos de diagnostico

## Changes
- Backend: CreateCampaignCommand + Handler + Controller
- Frontend: CampaignForm + useCampaigns hook
- Tests: 15 unit tests, 3 E2E tests

## Test Plan
- [x] Unit tests passing (15/15)
- [x] E2E tests passing (3/3)
- [x] Coverage > 80% (89%)
- [ ] Manual testing in staging

## Screenshots
_Adjuntar capturas de UI si aplica_

---
🤖 Generated with Claude Code
```

### Fase 5: Cleanup

```bash
# Volver a rama principal
git checkout main

# Eliminar worktree
git worktree remove .worktrees/{feature-id}

# Opcional: eliminar rama local
git branch -D feature/{feature-id}
```

## Output Ejemplo

```
✅ SHIP COMPLETADO: feat-20260123-crear-campania

📋 Validaciones:
   ✅ Build backend (Release)
   ✅ Tests backend (15/15)
   ✅ Lint frontend
   ✅ Type check frontend
   ✅ Build frontend

📝 Commit:
   feat(crowdfunding): implement create campaign
   Hash: a1b2c3d4

🔗 Pull Request:
   URL: https://dev.azure.com/weplay/WePlayRises/_git/WePlayRises/pullrequest/42
   Estado: Open
   Reviewers: jonay@email.com

🧹 Cleanup:
   ✅ Worktree eliminado
   ✅ Rama local eliminada

⏭️  Siguiente: Esperar aprobacion del PR
```
```

---

## PARTE 8: INTEGRACION CON UXPILOT

### Configuracion

```json
// .claude/settings.local.json
{
  "uxpilot": {
    "enabled": true,
    "apiKey": "${UXPILOT_API_KEY}",
    "projectId": "weplay-rises",
    "autoGenerate": true,
    "outputDir": ".uxpilot/components"
  }
}
```

### Workflow Integrado

```
FLUJO UXPILOT + CLAUDE CODE:

1. SPEC con UxPilot
   /spec "Dashboard de artista"
        │
        └─► @spec-architect genera specs incluyendo:
            • Mockups requeridos de UxPilot
            • Componentes UI necesarios
            • Tokens de Design System

2. GENERAR UI con UxPilot
   /uxpilot generate "Artist Dashboard with stats cards and campaign list"
        │
        └─► UxPilot genera:
            • Mockup interactivo
            • Codigo base React
            • Variantes responsive

3. IMPLEMENTAR con ajustes
   /implement feat-... --uxpilot
        │
        └─► Claude Code:
            • Lee output de UxPilot
            • Adapta a Design System
            • Integra con estado real
            • Conecta con API
```

### Comando /uxpilot

```markdown
<!-- .claude/commands/uxpilot.md -->
# Comando: /uxpilot

## Proposito
Integra con UxPilot para generar UI profesional.

## Uso
```
/uxpilot generate "descripcion de UI"     # Generar nuevo componente
/uxpilot refine {component}               # Refinar componente existente
/uxpilot variants {component}             # Generar variantes
```

## Proceso

### Generar Componente

1. Enviar prompt a UxPilot API
2. Recibir mockup + codigo base
3. Guardar en `.uxpilot/components/{component}/`
4. Adaptar a stack del proyecto (Tailwind + shadcn)

### Integrar con /implement

Cuando se usa `--uxpilot` en /implement:

```typescript
// 1. Lee componente de UxPilot
const uxComponent = readFile('.uxpilot/components/ArtistDashboard/code.tsx');

// 2. Adapta imports y estilos
const adapted = adaptToProject(uxComponent, {
  designSystem: 'shadcn',
  stateManagement: 'tanstack-query',
  routing: 'nextjs'
});

// 3. Conecta con API real
const connected = connectToApi(adapted, {
  hooks: ['useArtistStats', 'useCampaigns'],
  endpoints: ['GET /api/artistas/{id}/stats']
});

// 4. Genera tests
const tests = generateTests(connected);
```

## Output Ejemplo

```
✅ UXPILOT: ArtistDashboard generado

📁 Archivos:
   .uxpilot/components/ArtistDashboard/
   ├── mockup.png           (preview visual)
   ├── code.tsx             (codigo base)
   ├── variants/
   │   ├── mobile.tsx
   │   └── tablet.tsx
   └── tokens.json          (tokens usados)

🎨 Componentes detectados:
   • StatsCard (4 variantes)
   • CampaignList
   • ProgressBar
   • UserAvatar

⏭️  Usa: /implement ... --uxpilot para integrar
```
```

---

## PARTE 9: MCP DB DIAGNOSTIC SERVER

### Configuracion

El servidor MCP para diagnostico de base de datos ya esta documentado en la estrategia de observabilidad. Aqui el resumen de integracion:

```json
// .claude/settings.local.json
{
  "mcpServers": {
    "db-diagnostic": {
      "command": "node",
      "args": [".claude/mcp/db-diagnostic/dist/index.js"],
      "env": {
        "DB_SERVER": "localhost",
        "DB_NAME": "WePlayRises",
        "DB_USER": "diagnostic_user",
        "DB_PASSWORD": "${DB_DIAGNOSTIC_PASSWORD}"
      }
    }
  }
}
```

### Herramientas Disponibles

| Tool | Descripcion | Uso |
|------|-------------|-----|
| `db_connect` | Conectar a SQL Server | Inicio de sesion |
| `db_recent_queries` | Queries ejecutadas | Durante tests |
| `db_slow_queries` | Queries lentas (>Xms) | Diagnostico |
| `db_table_schema` | Schema de tabla | Exploracion |
| `db_select` | Query SELECT (solo lectura) | Debug |
| `db_connection_status` | Estado de conexiones | Monitoreo |
| `db_active_transactions` | Transacciones activas | Debug |

### Uso en /diagnose

```typescript
// Dentro del comando /diagnose

// 1. Conectar
await mcp_db_diagnostic.db_connect();

// 2. Capturar queries durante tests
const queries = await mcp_db_diagnostic.db_recent_queries({
  seconds: 60,
  minDurationMs: 0
});

// 3. Identificar queries lentas
const slow = await mcp_db_diagnostic.db_slow_queries({
  thresholdMs: 100
});

// 4. Analizar y sugerir optimizaciones
for (const query of slow) {
  suggestOptimization(query);
}
```

---

## PARTE 10: TEMPLATES DE CODIGO

### Backend: Command

```csharp
// .claude/templates/backend/command.template.cs
// Variables: {{CommandName}}, {{ModuleName}}, {{Properties}}, {{Validations}}

using FluentValidation;
using MediatR;
using {{ModuleName}}.Domain;
using BuildingBlocks.Application;

namespace {{ModuleName}}.Application.Commands;

public record {{CommandName}}Command(
    {{Properties}}
) : IRequest<Result<{{CommandName}}Response>>;

public record {{CommandName}}Response(
    Guid Id,
    // ... response properties
);

public class {{CommandName}}CommandValidator : AbstractValidator<{{CommandName}}Command>
{
    public {{CommandName}}CommandValidator()
    {
        {{Validations}}
    }
}

public class {{CommandName}}CommandHandler : IRequestHandler<{{CommandName}}Command, Result<{{CommandName}}Response>>
{
    private readonly I{{ModuleName}}Repository _repository;
    private readonly ILogger<{{CommandName}}CommandHandler> _logger;

    public {{CommandName}}CommandHandler(
        I{{ModuleName}}Repository repository,
        ILogger<{{CommandName}}CommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<{{CommandName}}Response>> Handle(
        {{CommandName}}Command request,
        CancellationToken cancellationToken)
    {
        // #DIAG:{{CommandName:kebab}}-entry

        try
        {
            // Implementacion

            // #DIAG:{{CommandName:kebab}}-success
            return Result<{{CommandName}}Response>.Success(response);
        }
        catch (Exception ex)
        {
            // #DIAG:{{CommandName:kebab}}-error
            _logger.LogError(ex, "Error in {{CommandName}}");
            return Result<{{CommandName}}Response>.Failure(ex.Message);
        }
    }
}
```

### Frontend: Component

```tsx
// .claude/templates/frontend/component.template.tsx
// Variables: {{ComponentName}}, {{Props}}, {{Imports}}

'use client';

import { useState } from 'react';
{{Imports}}

interface {{ComponentName}}Props {
  {{Props}}
}

export function {{ComponentName}}({ ...props }: {{ComponentName}}Props) {
  // State

  // Handlers

  return (
    <div className="{{ComponentName:kebab}}">
      {/* Content */}
    </div>
  );
}

{{ComponentName}}.displayName = '{{ComponentName}}';
```

### Test: Unit

```csharp
// .claude/templates/tests/unit.template.cs
// Variables: {{TestSubject}}, {{MethodName}}, {{Fixture}}

using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.{{ModuleName}};

public class {{TestSubject}}Tests
{
    private readonly Mock<IDependency> _mockDependency;
    private readonly {{TestSubject}} _sut;

    public {{TestSubject}}Tests()
    {
        _mockDependency = new Mock<IDependency>();
        _sut = new {{TestSubject}}(_mockDependency.Object);
    }

    [Fact]
    public async Task {{MethodName}}_WhenValidInput_ShouldSucceed()
    {
        // Arrange
        var input = {{Fixture}}.Valid();

        // Act
        var result = await _sut.{{MethodName}}(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task {{MethodName}}_WhenInvalidInput_ShouldFail()
    {
        // Arrange
        var input = {{Fixture}}.Invalid();

        // Act
        var result = await _sut.{{MethodName}}(input);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
```

---

## PARTE 11: RESUMEN RAPIDO

### Comandos Principales

| Comando | Proposito | Output |
|---------|-----------|--------|
| `/spec "feature"` | Especificacion completa | `specs/{id}/` |
| `/synth-data {id}` | Datos sinteticos | Fixtures + Seeds |
| `/implement {id}` | Implementacion TDD | Codigo + Tests |
| `/diagnose {id}` | Diagnostico + Auto-fix | Reporte + Fixes |
| `/ship {id}` | Pipeline + PR | PR en Azure DevOps |
| `/uxpilot "ui"` | Generar UI | Componente React |

### Hooks Automaticos

| Hook | Trigger | Accion |
|------|---------|--------|
| load-project-context | SessionStart | Carga contexto |
| load-contextual-rules | PreToolUse | Reglas por archivo |
| tdd-reminder | PreToolUse (Write) | Recuerda TDD |
| auto-format | PostToolUse | Formatea codigo |
| detect-secrets | PostToolUse | Bloquea secrets |
| db-query-observer | PostToolUse (Bash) | Captura queries |
| strategic-compact | PreToolUse | Sugiere compactar |
| session-end-persistence | Stop | Guarda estado |

### Flujo Tipico

```bash
# 1. Especificar feature
/spec "Crear campania de crowdfunding"

# 2. Generar datos de prueba
/synth-data feat-20260123-crear-campania

# 3. Implementar con TDD
/implement feat-20260123-crear-campania

# 4. Diagnosticar y corregir
/diagnose feat-20260123-crear-campania

# 5. Crear PR
/ship feat-20260123-crear-campania
```

---

## PARTE 12: CHECKLIST DE SETUP

### Prerequisitos

- [ ] Claude Code instalado (`npm install -g @anthropic-ai/claude-code`)
- [ ] Azure CLI instalado (`az`)
- [ ] .NET 8 SDK
- [ ] Node.js 20+
- [ ] SQL Server (LocalDB o Docker)

### Configuracion Inicial

```bash
# 1. Clonar estructura de .claude/
# (se proporciona como template)

# 2. Configurar variables de entorno
cp .claude/settings.local.example.json .claude/settings.local.json
# Editar con valores reales

# 3. Instalar dependencias de hooks
pip install pyyaml jsonschema

# 4. Configurar MCP DB Server
cd .claude/mcp/db-diagnostic
npm install
npm run build

# 5. Configurar Azure DevOps
az login
az devops configure --defaults organization=https://dev.azure.com/weplay project=WePlayRises

# 6. Configurar UxPilot (opcional)
# Obtener API key de https://uxpilot.ai/
```

### Verificacion

```bash
# Verificar hooks
claude --verify-hooks

# Verificar MCP
claude --list-mcp-servers

# Test rapido
claude "di hola"
```

---

## PARTE 13: EVOLUCION FUTURA

### Fase 2: Mejoras Planificadas

| Mejora | Descripcion | Prioridad |
|--------|-------------|-----------|
| Agente E2E con Playwright | Tests E2E automatizados con browser real | Alta |
| Integracion con Figma | Sincronizar con Design System | Media |
| Metricas de productividad | Dashboard de velocity y coverage | Media |
| Auto-deploy a staging | Deploy automatico para review | Baja |

### Feedback Loop

```
CICLO DE MEJORA CONTINUA:

1. Usar el sistema
2. Identificar fricciones
3. Documentar en /remember
4. Implementar mejora
5. Validar mejora
6. Repetir
```

---

**Documento creado**: 2026-01-23
**Version**: 1.0
**Autor**: Ivan / Claude Code (Opus 4.5)

---

## Apendice A: Estructura de Archivos Completa

```
.claude/
├── CLAUDE.md
├── settings.local.json
├── hooks/
│   ├── settings.json
│   ├── session-persistence/
│   │   ├── session-start.py
│   │   ├── pre-compact.py
│   │   └── session-end.py
│   ├── quality-gates/
│   │   ├── auto-format.py
│   │   ├── tdd-reminder.py
│   │   └── detect-secrets.py
│   ├── diagnostic/
│   │   └── db-query-observer.py
│   ├── rules/
│   │   └── load-contextual-rules.py
│   └── strategic-compact/
│       └── suggest-compact.py
├── agents/
│   ├── spec-architect.agent.md
│   ├── spec-tester.agent.md
│   ├── spec-e2e.agent.md
│   ├── spec-diagnostic.agent.md
│   ├── implement-backend.agent.md
│   ├── implement-frontend.agent.md
│   ├── diagnose-resolver.agent.md
│   └── ship-reviewer.agent.md
├── commands/
│   ├── spec.md
│   ├── synth-data.md
│   ├── implement.md
│   ├── diagnose.md
│   ├── ship.md
│   └── uxpilot.md
├── templates/
│   ├── backend/
│   │   ├── command.template.cs
│   │   ├── query.template.cs
│   │   ├── controller.template.cs
│   │   └── entity.template.cs
│   ├── frontend/
│   │   ├── component.template.tsx
│   │   ├── page.template.tsx
│   │   ├── hook.template.ts
│   │   └── service.template.ts
│   └── tests/
│       ├── unit.template.cs
│       ├── integration.template.cs
│       ├── e2e.template.ts
│       └── fixture.template.json
├── rules/
│   ├── _index.yaml
│   ├── always/
│   ├── backend/
│   └── frontend/
├── contexts/
│   ├── dev.md
│   ├── review.md
│   └── research.md
└── mcp/
    └── db-diagnostic/
        ├── package.json
        └── src/index.ts
```

5. **Diagnostico y auto-resolucion** - Claude detecta y corrige errores autonomamente
6. **UxPilot integrado** - UI profesional desde el inicio

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        PARADIGMA DE DESARROLLO AGENTICO                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   /spec "feature"        ──►  ESPECIFICACION                                │
│        │                      (Multiples agentes en paralelo)               │
│        │                      • Arquitectura • DTOs • Tests • Diag • E2E    │
│        ▼                                                                    │
│   /synth-data            ──►  DATOS SINTETICOS                              │
│        │                      (Seeds + Fixtures + Mocks)                    │
│        ▼                                                                    │
│   /implement             ──►  IMPLEMENTACION                                │
│        │                      (Worktree aislado + TDD)                      │
│        ▼                                                                    │
│   /diagnose              ──►  DIAGNOSTICO + AUTO-FIX                        │
│        │                      (Tests + DB + Logs + Auto-resolve)            │
│        ▼                                                                    │
│   /ship                  ──►  PIPELINE + PR                                 │
│                               (Cleanup + CI + PR automatica)                │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## PARTE 1: ESTRUCTURA DEL PROYECTO

### Estructura de Directorios Claude Code

```
WePlay_Rises/
├── .claude/
│   ├── CLAUDE.md                      # Instrucciones principales
│   ├── settings.local.json            # Variables de entorno
│   │
│   ├── hooks/                         # Automatizacion silenciosa
│   │   ├── settings.json              # Configuracion de hooks
│   │   ├── session-persistence/       # Persistencia entre sesiones
│   │   ├── quality-gates/             # Validacion automatica
│   │   └── strategic-compact/         # Gestion de contexto
│   │
│   ├── agents/                        # Agentes especializados
│   │   ├── spec-architect.agent.md    # Arquitectura y DTOs
│   │   ├── spec-tester.agent.md       # Tests unitarios e integracion
│   │   ├── spec-e2e.agent.md          # Tests E2E y flujos
│   │   ├── spec-diagnostic.agent.md   # Puntos de diagnostico
│   │   ├── implement-backend.agent.md # Implementacion .NET
│   │   ├── implement-frontend.agent.md# Implementacion React
│   │   ├── diagnose-resolver.agent.md # Diagnostico y auto-fix
│   │   └── ship-reviewer.agent.md     # Review final y PR
│   │
│   ├── commands/                      # Comandos unificados
│   │   ├── spec.md                    # /spec - Especificacion completa
│   │   ├── synth-data.md              # /synth-data - Datos sinteticos
│   │   ├── implement.md               # /implement - Worktree + codigo
│   │   ├── diagnose.md                # /diagnose - Diagnostico + fix
│   │   └── ship.md                    # /ship - Pipeline + PR
│   │
│   ├── templates/                     # Templates de codigo
│   │   ├── backend/
│   │   │   ├── command.template.cs
│   │   │   ├── query.template.cs
│   │   │   ├── controller.template.cs
│   │   │   └── entity.template.cs
│   │   ├── frontend/
│   │   │   ├── component.template.tsx
│   │   │   ├── page.template.tsx
│   │   │   ├── hook.template.ts
│   │   │   └── service.template.ts
│   │   └── tests/
│   │       ├── unit.template.cs
│   │       ├── integration.template.cs
│   │       ├── e2e.template.ts
│   │       └── fixture.template.json
│   │
│   ├── rules/                         # Reglas contextuales
│   │   ├── _index.yaml
│   │   ├── always/
│   │   │   ├── code-style.rule.md
│   │   │   └── git-conventions.rule.md
│   │   ├── backend/
│   │   │   ├── cqrs-commands.rule.md
│   │   │   └── ef-core.rule.md
│   │   └── frontend/
│   │       ├── react-components.rule.md
│   │       └── tanstack-query.rule.md
│   │
│   └── contexts/                      # Modos de trabajo
│       ├── dev.md                     # Modo desarrollo
│       ├── review.md                  # Modo revision
│       └── research.md                # Modo investigacion
│
├── specs/                             # Especificaciones generadas
│   └── {feature-id}/
│       ├── architecture.md            # Arquitectura y DTOs
│       ├── tests-unit.md              # Plan tests unitarios
│       ├── tests-e2e.md               # Plan tests E2E
│       ├── diagnostic-points.md       # Puntos de diagnostico
│       └── synthetic-data.yaml        # Datos sinteticos
│
├── tasks/                             # Tracking de tareas
│   ├── backlog.md                     # Backlog general
│   └── active-feature.yaml            # Feature en progreso
│
└── .worktrees/                        # Worktrees aislados (gitignore)
```

---

## PARTE 2: SISTEMA DE HOOKS

### Configuracion de Hooks

```json
// .claude/hooks/settings.json
{
  "hooks": [
    {
      "name": "load-project-context",
      "trigger": "SessionStart",
      "script": ".claude/hooks/session-persistence/session-start.py",
      "enabled": true,
      "description": "Carga contexto del proyecto y sesiones anteriores"
    },
    {
      "name": "load-contextual-rules",
      "trigger": "PreToolUse",
      "tools": ["Read", "Edit", "Write"],
      "script": ".claude/hooks/rules/load-contextual-rules.py",
      "enabled": true,
      "description": "Carga reglas segun tipo de archivo editado"
    },
    {
      "name": "auto-format",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/auto-format.py",
      "enabled": true,
      "description": "Formatea codigo automaticamente"
    },
    {
      "name": "tdd-reminder",
      "trigger": "PreToolUse",
      "tools": ["Write"],
      "script": ".claude/hooks/quality-gates/tdd-reminder.py",
      "enabled": true,
      "description": "Recuerda escribir tests antes de codigo de produccion"
    },
    {
      "name": "detect-secrets",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/detect-secrets.py",
      "enabled": true,
      "blocksOnFailure": true,
      "description": "BLOQUEA si detecta secrets hardcodeados"
    },
    {
      "name": "db-query-observer",
      "trigger": "PostToolUse",
      "tools": ["Bash"],
      "script": ".claude/hooks/diagnostic/db-query-observer.py",
      "enabled": true,
      "description": "Captura queries SQL ejecutadas"
    },
    {
      "name": "strategic-compact",
      "trigger": "PreToolUse",
      "script": ".claude/hooks/strategic-compact/suggest-compact.py",
      "enabled": true,
      "description": "Sugiere compactar cada 50 herramientas"
    },
    {
      "name": "session-end-persistence",
      "trigger": "Stop",
      "script": ".claude/hooks/session-persistence/session-end.py",
      "enabled": true,
      "description": "Guarda estado y patrones aprendidos"
    }
  ]
}
```

### Hook: TDD Reminder

```python
#!/usr/bin/env python3
"""
Hook: tdd-reminder
Trigger: PreToolUse (Write)
Recuerda escribir tests antes de codigo de produccion.
"""

import sys
import json
import os

def main():
    # Leer input del hook
    hook_input = json.loads(sys.stdin.read())
    file_path = hook_input.get('file_path', '')

    # Ignorar archivos de test
    test_patterns = ['.test.', '.spec.', 'Test.cs', 'Tests.cs', '__tests__']
    if any(p in file_path for p in test_patterns):
        return {"status": "pass"}

    # Ignorar templates, configs, etc
    ignore_patterns = ['.md', '.json', '.yaml', '.yml', '.config', 'template']
    if any(p in file_path for p in ignore_patterns):
        return {"status": "pass"}

    # Verificar si existe archivo de test correspondiente
    test_file = get_test_file_path(file_path)

    if not os.path.exists(test_file):
        return {
            "status": "warn",
            "message": f"""
╔══════════════════════════════════════════════════════════════════╗
║  [TDD] RECUERDA: Test-First Development                          ║
╠══════════════════════════════════════════════════════════════════╣
║                                                                  ║
║  Estas creando: {os.path.basename(file_path)[:40]}               ║
║                                                                  ║
║  RECOMENDACION:                                                  ║
║  1. Primero crea el test en: {os.path.basename(test_file)[:30]}  ║
║  2. Haz que el test falle                                        ║
║  3. Luego implementa el codigo                                   ║
║                                                                  ║
║  Usa: /synth-data para generar datos de prueba                   ║
║                                                                  ║
╚══════════════════════════════════════════════════════════════════╝
"""
        }

    return {"status": "pass"}

def get_test_file_path(file_path: str) -> str:
    """Calcula la ruta del archivo de test correspondiente."""
    if file_path.endswith('.cs'):
        # .NET: UserService.cs -> UserServiceTests.cs
        return file_path.replace('.cs', 'Tests.cs')
    elif file_path.endswith('.ts') or file_path.endswith('.tsx'):
        # React: UserCard.tsx -> UserCard.test.tsx
        ext = '.tsx' if file_path.endswith('.tsx') else '.ts'
        return file_path.replace(ext, f'.test{ext}')
    return file_path + '.test'

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

### Hook: DB Query Observer

```python
#!/usr/bin/env python3
"""
Hook: db-query-observer
Trigger: PostToolUse (Bash)
Captura queries SQL ejecutadas para diagnostico.
"""

import sys
import json
import re
from datetime import datetime
from pathlib import Path

# Archivo donde se guardan las queries capturadas
QUERIES_LOG = Path('.claude/diagnostic/queries.log')

def main():
    hook_input = json.loads(sys.stdin.read())

    command = hook_input.get('command', '')
    output = hook_input.get('output', '')

    # Solo capturar si es dotnet run o test
    if 'dotnet' not in command:
        return {"status": "pass"}

    # Buscar queries SQL en el output
    queries = extract_sql_queries(output)

    if queries:
        save_queries(queries, command)

        # Alertar sobre queries lentas
        slow_queries = [q for q in queries if q.get('duration_ms', 0) > 100]
        if slow_queries:
            return {
                "status": "warn",
                "message": f"[DB] {len(slow_queries)} queries lentas detectadas (>100ms)"
            }

    return {"status": "pass"}

def extract_sql_queries(output: str) -> list:
    """Extrae queries SQL del output de EF Core."""
    queries = []

    # Patron para logs de EF Core
    ef_pattern = r'\[(\d+)ms\]\s*(SELECT|INSERT|UPDATE|DELETE).*?(?=\n\[|\Z)'

    for match in re.finditer(ef_pattern, output, re.DOTALL | re.IGNORECASE):
        duration = int(match.group(1))
        query = match.group(0)
        queries.append({
            'timestamp': datetime.now().isoformat(),
            'duration_ms': duration,
            'query': query[:500]  # Truncar queries largas
        })

    return queries

def save_queries(queries: list, context: str):
    """Guarda queries en log para analisis."""
    QUERIES_LOG.parent.mkdir(parents=True, exist_ok=True)

    with open(QUERIES_LOG, 'a', encoding='utf-8') as f:
        f.write(f"\n=== {datetime.now().isoformat()} | {context[:50]} ===\n")
        for q in queries:
            f.write(f"[{q['duration_ms']}ms] {q['query']}\n")

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

---

## PARTE 3: COMANDO /spec - ESPECIFICACION UNIFICADA

### Descripcion

Un solo comando que orquesta **multiples agentes en paralelo** para generar la especificacion completa de una feature:

```bash
/spec "Crear campania de crowdfunding"
```

### Workflow del Comando /spec

```
/spec "Crear campania de crowdfunding"
        │
        ├─► [PARALELO] @spec-architect
        │       └─► specs/{feature-id}/architecture.md
        │           • DTOs de request/response
        │           • Endpoints API
        │           • Entidades y relaciones
        │           • Modulos afectados
        │
        ├─► [PARALELO] @spec-tester
        │       └─► specs/{feature-id}/tests-unit.md
        │           • Casos de test unitarios
        │           • Mocks requeridos
        │           • Cobertura esperada
        │
        ├─► [PARALELO] @spec-e2e
        │       └─► specs/{feature-id}/tests-e2e.md
        │           • Flujos E2E (Playwright)
        │           • Escenarios happy path
        │           • Escenarios de error
        │
        ├─► [PARALELO] @spec-diagnostic
        │       └─► specs/{feature-id}/diagnostic-points.md
        │           • Puntos de logging (#DIAG)
        │           • Metricas a capturar
        │           • Alertas esperadas
        │
        └─► [SECUENCIAL] SINTESIS
                └─► specs/{feature-id}/SYNTHESIS.md
                    • Resumen ejecutivo
                    • Checklist de implementacion
                    • Dependencias entre tareas
```

### Archivo de Comando: /spec

```markdown
<!-- .claude/commands/spec.md -->
# Comando: /spec

## Proposito
Genera especificacion completa de una feature usando multiples agentes en paralelo.

## Uso
```
/spec "descripcion de la feature"
/spec "descripcion" --skip-e2e     # Sin tests E2E
/spec "descripcion" --quick        # Solo arquitectura
```

## Proceso

### Fase 1: Validacion (5 segundos)
1. Parsear la descripcion de la feature
2. Generar feature-id unico (formato: `feat-YYYYMMDD-slug`)
3. Crear directorio `specs/{feature-id}/`

### Fase 2: Agentes en Paralelo (2-5 minutos)

Lanzar 4 agentes simultaneamente:

```
Task subagent_type=spec-architect
Task subagent_type=spec-tester
Task subagent_type=spec-e2e
Task subagent_type=spec-diagnostic
```

Cada agente recibe:
- Descripcion de la feature
- Contexto del proyecto (CLAUDE.md)
- Stack tecnologico (de README.md)
- Entidades existentes (de src/api/Modules/)

### Fase 3: Sintesis (1 minuto)
1. Esperar a todos los agentes
2. Leer outputs de cada agente
3. Generar SYNTHESIS.md con:
   - Resumen ejecutivo
   - Checklist de tareas
   - Orden de implementacion (backend → frontend → tests)
   - Estimacion de complejidad

### Fase 4: Output
1. Mostrar resumen en consola
2. Indicar ruta a especificacion completa
3. Preguntar si proceder con /synth-data

## Agentes Involucrados

| Agente | Output | Tiempo |
|--------|--------|--------|
| @spec-architect | architecture.md | ~1 min |
| @spec-tester | tests-unit.md | ~1 min |
| @spec-e2e | tests-e2e.md | ~1 min |
| @spec-diagnostic | diagnostic-points.md | ~30s |

## Output Ejemplo

```
✅ ESPECIFICACION COMPLETADA: feat-20260123-crear-campania

📁 Archivos generados:
   specs/feat-20260123-crear-campania/
   ├── architecture.md      (DTOs, endpoints, entidades)
   ├── tests-unit.md        (15 test cases)
   ├── tests-e2e.md         (3 flujos E2E)
   ├── diagnostic-points.md (8 puntos de diagnostico)
   └── SYNTHESIS.md         (resumen y checklist)

📋 Checklist de Implementacion:
   [ ] 1. Backend: CreateCampaignCommand + Handler
   [ ] 2. Backend: GetCampaignQuery + Handler
   [ ] 3. Backend: CampaniaController endpoints
   [ ] 4. Frontend: useCampanias hook
   [ ] 5. Frontend: CampaignForm component
   [ ] 6. Tests: Unit tests backend
   [ ] 7. Tests: E2E flow
   [ ] 8. Diagnostico: Puntos de logging

⏭️  Siguiente paso: /synth-data feat-20260123-crear-campania
```
```

### Agente: spec-architect

```markdown
<!-- .claude/agents/spec-architect.agent.md -->
# Agente: spec-architect

## Rol
Arquitecto de software que diseña la estructura tecnica de features.

## Input
- Descripcion de la feature
- Entidades existentes en el proyecto
- Stack tecnologico

## Output: architecture.md

### Estructura del Output

```markdown
# Arquitectura: {feature-name}

## 1. Resumen
{descripcion de alto nivel}

## 2. Endpoints API

### POST /api/campanias
- **Descripcion**: Crear nueva campania
- **Auth**: JWT (role: Artist)
- **Request Body**:
```json
{
  "titulo": "string",
  "descripcion": "string",
  "importeObjetivo": "decimal",
  "fechaInicio": "date",
  "fechaFin": "date"
}
```
- **Response 201**:
```json
{
  "id": "guid",
  "titulo": "string",
  "estado": "Borrador"
}
```
- **Errores**: 400 (validacion), 401 (no auth), 403 (no artist)

## 3. DTOs

### CreateCampaignRequest
| Campo | Tipo | Validacion |
|-------|------|------------|
| titulo | string | required, max 200 |
| descripcion | string | required, max 2000 |
| importeObjetivo | decimal | > 0 |

### CampaignResponse
| Campo | Tipo |
|-------|------|
| id | Guid |
| titulo | string |
| estado | CampaignStatus |

## 4. Entidades

### Campania (modificaciones)
- Nuevos campos: ninguno
- Nuevas relaciones: ninguna

## 5. CQRS

### Commands
- CreateCampaignCommand
- UpdateCampaignCommand
- PublishCampaignCommand

### Queries
- GetCampaignByIdQuery
- GetCampaignsByArtistaQuery
- GetPublicCampaignsQuery

## 6. Modulos Afectados
- Crowdfunding (principal)
- UserAccess (validar ArtistaId)

## 7. Dependencias
- Requiere: ArtistaId del usuario autenticado
- Bloquea: RewardController necesita CampaignId
```

## Reglas

1. **CQRS obligatorio**: Siempre separar Commands y Queries
2. **DTOs explicitos**: Nunca exponer entidades directamente
3. **Validacion en Commands**: FluentValidation en cada command
4. **Errores tipados**: Usar Result<T> pattern
5. **Sin logica en Controllers**: Solo mapeo y delegacion a MediatR
```

---

## PARTE 4: COMANDO /synth-data - DATOS SINTETICOS

### Descripcion

Genera datos sinteticos realistas para testing basandose en la especificacion.

```bash
/synth-data feat-20260123-crear-campania
```

### Workflow

```
/synth-data {feature-id}
        │
        ├─► Lee specs/{feature-id}/architecture.md
        │
        ├─► Genera:
        │   ├─► specs/{feature-id}/synthetic-data.yaml
        │   │       (datos para seeds y fixtures)
        │   │
        │   ├─► src/api/Tests/Fixtures/{Feature}Fixtures.cs
        │   │       (fixtures C# para xUnit)
        │   │
        │   ├─► src/web/__fixtures__/{feature}.json
        │   │       (mocks para frontend)
        │   │
        │   └─► src/api/Seeds/{Feature}Seed.cs
        │           (seeds para desarrollo)
        │
        └─► Output: Resumen de datos generados
```

### Archivo de Comando: /synth-data

```markdown
<!-- .claude/commands/synth-data.md -->
# Comando: /synth-data

## Proposito
Genera datos sinteticos realistas para testing y desarrollo.

## Uso
```
/synth-data {feature-id}
/synth-data {feature-id} --count 50    # 50 registros
/synth-data {feature-id} --locale es   # Datos en espanol
```

## Proceso

### Fase 1: Analisis (10 segundos)
1. Leer architecture.md de la feature
2. Identificar entidades y relaciones
3. Determinar volumen de datos

### Fase 2: Generacion de Datos

Para cada entidad identificada, generar:

1. **synthetic-data.yaml** - Definicion de datos
```yaml
campania:
  count: 20
  fields:
    titulo:
      type: music_project_name
      unique: true
    descripcion:
      type: lorem
      paragraphs: 2
    importeObjetivo:
      type: random_decimal
      min: 1000
      max: 50000
    estado:
      type: enum
      values: [Borrador, Activa, Finalizada, Cancelada]
      weights: [0.2, 0.5, 0.2, 0.1]
    fechaInicio:
      type: date_future
      days: 30
    fechaFin:
      type: date_after
      field: fechaInicio
      days_after: 60

  relationships:
    artista:
      type: belongs_to
      entity: artista
      required: true

artista:
  count: 10
  fields:
    nombreArtistico:
      type: band_name
    descripcion:
      type: artist_bio
```

2. **C# Fixtures** - Para tests de integracion
```csharp
public static class CampaniaFixtures
{
    public static Campania ValidBorrador() => new()
    {
        Id = Guid.NewGuid(),
        Titulo = "Album Debut - Los Rebeldes",
        Descripcion = "Nuestro primer album necesita tu apoyo...",
        ImporteObjetivo = 5000m,
        Estado = CampaignStatus.Borrador,
        ArtistaId = ArtistaFixtures.ValidArtista().Id
    };

    public static Campania ValidActiva() => ValidBorrador() with
    {
        Estado = CampaignStatus.Activa,
        FechaInicio = DateTime.UtcNow.AddDays(-10),
        FechaFin = DateTime.UtcNow.AddDays(50)
    };
}
```

3. **JSON Mocks** - Para frontend
```json
{
  "campanias": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "titulo": "Album Debut - Los Rebeldes",
      "descripcion": "Nuestro primer album...",
      "importeObjetivo": 5000,
      "importeRecaudado": 2350,
      "estado": "Activa",
      "backers": 47,
      "diasRestantes": 45
    }
  ]
}
```

### Fase 3: Seeds de Desarrollo

```csharp
// src/api/Seeds/CrowdfundingSeed.cs
public class CrowdfundingSeed : ISeedData
{
    public async Task SeedAsync(AppDbContext context)
    {
        if (await context.Campanias.AnyAsync()) return;

        // Usar datos de synthetic-data.yaml
        var campanias = GenerateFromYaml("specs/.../synthetic-data.yaml");
        await context.Campanias.AddRangeAsync(campanias);
        await context.SaveChangesAsync();
    }
}
```

## Output Ejemplo

```
✅ DATOS SINTETICOS GENERADOS: feat-20260123-crear-campania

📊 Datos generados:
   • 10 Artistas
   • 20 Campanias
   • 50 Rewards
   • 100 Backings

📁 Archivos creados:
   specs/feat-20260123-crear-campania/synthetic-data.yaml
   src/api/Tests/Fixtures/CampaniaFixtures.cs
   src/web/__fixtures__/campanias.json
   src/api/Seeds/CrowdfundingSeed.cs

⏭️  Siguiente paso: /implement feat-20260123-crear-campania
```
```

---

## PARTE 5: COMANDO /implement - WORKTREE + IMPLEMENTACION

### Descripcion

Crea un worktree aislado e implementa la feature siguiendo TDD.

```bash
/implement feat-20260123-crear-campania
```

### Workflow

```
/implement {feature-id}
        │
        ├─► Crear worktree aislado
        │   └─► git worktree add .worktrees/{feature-id} -b feature/{feature-id}
        │
        ├─► Copiar specs al worktree
        │
        ├─► CICLO TDD (por cada item del checklist):
        │   │
        │   ├─► 1. RED: Escribir test que falla
        │   │       └─► Usar fixtures de /synth-data
        │   │
        │   ├─► 2. GREEN: Implementar codigo minimo
        │   │       └─► Usar templates de .claude/templates/
        │   │
        │   └─► 3. REFACTOR: Limpiar codigo
        │           └─► Hook auto-format se ejecuta
        │
        ├─► Inyectar puntos de diagnostico (#DIAG)
        │
        └─► Output: Resumen de implementacion
```

### Archivo de Comando: /implement

```markdown
<!-- .claude/commands/implement.md -->
# Comando: /implement

## Proposito
Implementa una feature en un worktree aislado siguiendo TDD.

## Uso
```
/implement {feature-id}
/implement {feature-id} --backend-only    # Solo API
/implement {feature-id} --frontend-only   # Solo React
/implement {feature-id} --resume          # Continuar implementacion
```

## Proceso

### Fase 1: Setup Worktree (30 segundos)

```bash
# Crear rama y worktree
git worktree add .worktrees/{feature-id} -b feature/{feature-id}

# Copiar specs
cp -r specs/{feature-id} .worktrees/{feature-id}/specs/

# Navegar al worktree
cd .worktrees/{feature-id}
```

### Fase 2: Implementacion Backend (TDD)

Para cada Command/Query en architecture.md:

```
1. RED - Escribir test
   └─► src/api/Tests/Crowdfunding/CreateCampaignCommandTests.cs

2. GREEN - Implementar handler
   └─► src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommand.cs
   └─► src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs

3. REFACTOR - Limpiar
   └─► dotnet format
```

**Template de Command:**
```csharp
// Generado desde .claude/templates/backend/command.template.cs
public record CreateCampaignCommand(
    string Titulo,
    string Descripcion,
    decimal ImporteObjetivo,
    DateTime FechaInicio,
    DateTime FechaFin
) : IRequest<Result<CampaignResponse>>;

public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .MaximumLength(200);
        // ... mas reglas
    }
}

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<CampaignResponse>>
{
    // #DIAG:create-campaign-entry
    public async Task<Result<CampaignResponse>> Handle(
        CreateCampaignCommand request,
        CancellationToken ct)
    {
        // Implementacion...
        // #DIAG:create-campaign-exit
    }
}
```

### Fase 3: Implementacion Frontend (TDD)

Para cada componente en architecture.md:

```
1. RED - Escribir test
   └─► src/web/src/features/campaigns/__tests__/CampaignForm.test.tsx

2. GREEN - Implementar componente
   └─► src/web/src/features/campaigns/components/CampaignForm.tsx

3. REFACTOR - Limpiar
   └─► npm run lint:fix
```

**Template de Componente:**
```tsx
// Generado desde .claude/templates/frontend/component.template.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { createCampaignSchema, type CreateCampaignInput } from '@/shared/schemas';

interface CampaignFormProps {
  onSubmit: (data: CreateCampaignInput) => void;
  isLoading?: boolean;
}

export function CampaignForm({ onSubmit, isLoading }: CampaignFormProps) {
  const form = useForm<CreateCampaignInput>({
    resolver: zodResolver(createCampaignSchema),
  });

  return (
    <form onSubmit={form.handleSubmit(onSubmit)}>
      {/* Campos del formulario */}
    </form>
  );
}
```

### Fase 4: Puntos de Diagnostico

Inyectar automaticamente segun diagnostic-points.md:

```csharp
// Los #DIAG se inyectan en puntos clave
public async Task<Result<CampaignResponse>> Handle(...)
{
    // #DIAG:create-campaign-entry | timestamp={timestamp} | userId={userId}

    var campania = await _repository.CreateAsync(...);

    // #DIAG:create-campaign-db-saved | campaniaId={campania.Id} | duration={duration}ms

    return Result.Success(response);
    // #DIAG:create-campaign-exit-success
}
```

## Output Ejemplo

```
✅ IMPLEMENTACION COMPLETADA: feat-20260123-crear-campania

📁 Worktree: .worktrees/feat-20260123-crear-campania
📊 Branch: feature/feat-20260123-crear-campania

Backend:
   ✅ CreateCampaignCommand + Handler + Validator
   ✅ CreateCampaignCommandTests (5 tests passing)
   ✅ CampaniaController.Post()
   ✅ 8 puntos #DIAG inyectados

Frontend:
   ✅ CampaignForm component
   ✅ useCampaigns hook
   ✅ CampaignForm.test.tsx (3 tests passing)

📈 Cobertura:
   Backend: 87%
   Frontend: 82%

⏭️  Siguiente paso: /diagnose feat-20260123-crear-campania
```
```

---

## PARTE 6: COMANDO /diagnose - DIAGNOSTICO + AUTO-FIX

### Descripcion

Ejecuta tests, analiza resultados, y corrige errores automaticamente.

```bash
/diagnose feat-20260123-crear-campania
```

### Workflow

```
/diagnose {feature-id}
        │
        ├─► Ejecutar tests unitarios
        │   └─► dotnet test
        │
        ├─► Ejecutar tests E2E
        │   └─► npx playwright test
        │
        ├─► Capturar queries SQL (via MCP DB Server)
        │   └─► Analizar queries lentas
        │
        ├─► Analizar logs #DIAG
        │   └─► Correlacionar eventos
        │
        ├─► Si hay errores:
        │   ├─► @diagnose-resolver analiza
        │   ├─► Propone fix
        │   ├─► Aplica fix automaticamente (si es seguro)
        │   └─► Re-ejecuta tests
        │
        └─► Output: Reporte de diagnostico
```

### Archivo de Comando: /diagnose

```markdown
<!-- .claude/commands/diagnose.md -->
# Comando: /diagnose

## Proposito
Ejecuta diagnostico completo y corrige errores automaticamente.

## Uso
```
/diagnose {feature-id}
/diagnose {feature-id} --no-auto-fix     # Solo diagnostico
/diagnose {feature-id} --verbose         # Detalle completo
```

## Proceso

### Fase 1: Ejecutar Tests (1-3 minutos)

```bash
# Tests unitarios backend
cd .worktrees/{feature-id}
dotnet test --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"

# Tests unitarios frontend
cd src/web
npm test -- --coverage --json > test-results.json

# Tests E2E
cd src/web
npx playwright test --reporter=json > e2e-results.json
```

### Fase 2: Capturar Queries SQL

Usando MCP DB Server:

```typescript
// Queries ejecutadas durante tests
const recentQueries = await mcp_db_diagnostic.db_recent_queries({
  seconds: 300,  // Ultimos 5 minutos
  minDurationMs: 0
});

// Queries lentas
const slowQueries = await mcp_db_diagnostic.db_slow_queries({
  thresholdMs: 100
});
```

### Fase 3: Analizar Resultados

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                          DIAGNOSTICO: feat-20260123                          │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  TESTS UNITARIOS BACKEND                                                     │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ CreateCampaignCommandTests.Should_Create_Valid_Campaign         PASS    │
│  ✅ CreateCampaignCommandTests.Should_Fail_Empty_Title              PASS    │
│  ❌ CreateCampaignCommandTests.Should_Fail_Past_StartDate           FAIL    │
│     └─► Expected: ValidationException                                        │
│     └─► Actual: Success (validacion no implementada)                         │
│                                                                              │
│  TESTS E2E                                                                   │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ create-campaign.spec.ts: Happy path                             PASS    │
│  ✅ create-campaign.spec.ts: Validation errors                      PASS    │
│                                                                              │
│  QUERIES SQL                                                                 │
│  ─────────────────────────────────────────────────────────────────────────   │
│  📊 Total queries: 45                                                       │
│  ⚠️  Slow queries (>100ms): 2                                               │
│     └─► SELECT * FROM Campanias WHERE ArtistaId = @p0  (234ms)               │
│         SUGERENCIA: Agregar indice en ArtistaId                              │
│                                                                              │
│  PUNTOS #DIAG                                                                │
│  ─────────────────────────────────────────────────────────────────────────   │
│  ✅ create-campaign-entry: 15 invocaciones                                  │
│  ✅ create-campaign-db-saved: 12 invocaciones                               │
│  ✅ create-campaign-exit-success: 12 invocaciones                           │
│  ⚠️  create-campaign-exit-error: 3 invocaciones                             │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

### Fase 4: Auto-Fix

El agente @diagnose-resolver analiza cada error:

```
ERROR DETECTADO:
  Test: Should_Fail_Past_StartDate
  Tipo: Validacion faltante
  Severidad: Media

ANALISIS:
  El CommandValidator no tiene regla para FechaInicio > DateTime.UtcNow

FIX PROPUESTO:
  Agregar en CreateCampaignCommandValidator:
  RuleFor(x => x.FechaInicio).GreaterThan(DateTime.UtcNow)

APLICANDO FIX...
  ✅ Archivo modificado: CreateCampaignCommandValidator.cs
  ✅ Re-ejecutando test...
  ✅ Test ahora pasa
```

### Fase 5: Sugerencias de Optimizacion

```
SUGERENCIAS DE OPTIMIZACION:

1. [INDICE] Agregar indice en Campanias.ArtistaId
   └─► Mejoraria query de 234ms a ~10ms
   └─► SQL: CREATE INDEX IX_Campanias_ArtistaId ON Campanias(ArtistaId)

2. [N+1] Detectado posible N+1 en GetCampaignsByArtistaQuery
   └─► Usar .Include(c => c.Rewards) para cargar en una query

3. [COBERTURA] CampaignForm tiene 82% cobertura
   └─► Faltan tests para: error handling, loading state
```

## Output Ejemplo

```
✅ DIAGNOSTICO COMPLETADO: feat-20260123-crear-campania

📊 Resultados:
   Tests Backend: 15/15 ✅ (1 auto-fixed)
   Tests Frontend: 8/8 ✅
   Tests E2E: 3/3 ✅
   Cobertura: 89%

🔧 Auto-Fixes Aplicados:
   1. ✅ Validacion FechaInicio en CreateCampaignCommandValidator

⚠️  Sugerencias Pendientes:
   1. Agregar indice en Campanias.ArtistaId
   2. Resolver posible N+1 en GetCampaignsByArtistaQuery

⏭️  Siguiente paso: /ship feat-20260123-crear-campania
```
```

---

## PARTE 7: COMANDO /ship - PIPELINE + PR

### Descripcion

Limpia el codigo, ejecuta la pipeline de CI, y crea el PR.

```bash
/ship feat-20260123-crear-campania
```

### Workflow

```
/ship {feature-id}
        │
        ├─► Limpiar codigo #DIAG (opcional)
        │   └─► Remover o comentar puntos de diagnostico
        │
        ├─► Ejecutar validaciones finales
        │   ├─► dotnet build --no-restore
        │   ├─► dotnet test
        │   ├─► npm run lint
        │   └─► npm run type-check
        │
        ├─► Commit y Push
        │   └─► git commit -m "feat(crowdfunding): implement create campaign"
        │   └─► git push origin feature/{feature-id}
        │
        ├─► Crear PR en Azure DevOps
        │   └─► az repos pr create --title "..." --description "..."
        │
        └─► Limpiar worktree
            └─► git worktree remove .worktrees/{feature-id}
```

### Archivo de Comando: /ship

```markdown
<!-- .claude/commands/ship.md -->
# Comando: /ship

## Proposito
Prepara, valida, y crea PR para una feature completada.

## Uso
```
/ship {feature-id}
/ship {feature-id} --keep-diag       # Mantener puntos #DIAG
/ship {feature-id} --draft           # PR como draft
/ship {feature-id} --skip-cleanup    # No eliminar worktree
```

## Proceso

### Fase 1: Limpieza de Codigo (30 segundos)

```python
# Remover/comentar puntos #DIAG
for file in find_files_with_diag():
    content = file.read_text()
    # Comentar lineas #DIAG para produccion
    content = re.sub(
        r'(\s*)(.*#DIAG:.*)',
        r'\1// \2  // TODO: Enable in debug',
        content
    )
    file.write_text(content)
```

### Fase 2: Validaciones Finales (1-2 minutos)

```bash
# Backend
dotnet build --no-restore -c Release
dotnet test --no-build -c Release

# Frontend
npm run lint
npm run type-check
npm run build
```

### Fase 3: Commit y Push

```bash
# Generar mensaje de commit automatico
commit_msg=$(generate_commit_message specs/{feature-id}/SYNTHESIS.md)

# Commit
git add .
git commit -m "$commit_msg"

# Push
git push origin feature/{feature-id}
```

**Formato de Commit:**
```
feat(crowdfunding): implement create campaign

- Add CreateCampaignCommand with validation
- Add CampaignForm React component
- Add unit tests (15) and E2E tests (3)
- Coverage: 89%

Closes #WPR-007

Co-Authored-By: Claude Code <claude@anthropic.com>
```

### Fase 4: Crear PR en Azure DevOps

```bash
az repos pr create \
  --title "feat(crowdfunding): implement create campaign" \
  --description "$(cat pr-description.md)" \
  --source-branch "feature/{feature-id}" \
  --target-branch "main" \
  --reviewers "jonay@email.com" \
  --work-items "WPR-007"
```

**Template de PR:**
```markdown
## Summary
- Implementacion de creacion de campanias de crowdfunding
- Incluye validaciones, tests, y puntos de diagnostico

## Changes
- Backend: CreateCampaignCommand + Handler + Controller
- Frontend: CampaignForm + useCampaigns hook
- Tests: 15 unit tests, 3 E2E tests

## Test Plan
- [x] Unit tests passing (15/15)
- [x] E2E tests passing (3/3)
- [x] Coverage > 80% (89%)
- [ ] Manual testing in staging

## Screenshots
_Adjuntar capturas de UI si aplica_

---
🤖 Generated with Claude Code
```

### Fase 5: Cleanup

```bash
# Volver a rama principal
git checkout main

# Eliminar worktree
git worktree remove .worktrees/{feature-id}

# Opcional: eliminar rama local
git branch -D feature/{feature-id}
```

## Output Ejemplo

```
✅ SHIP COMPLETADO: feat-20260123-crear-campania

📋 Validaciones:
   ✅ Build backend (Release)
   ✅ Tests backend (15/15)
   ✅ Lint frontend
   ✅ Type check frontend
   ✅ Build frontend

📝 Commit:
   feat(crowdfunding): implement create campaign
   Hash: a1b2c3d4

🔗 Pull Request:
   URL: https://dev.azure.com/weplay/WePlayRises/_git/WePlayRises/pullrequest/42
   Estado: Open
   Reviewers: jonay@email.com

🧹 Cleanup:
   ✅ Worktree eliminado
   ✅ Rama local eliminada

⏭️  Siguiente: Esperar aprobacion del PR
```
```

---

## PARTE 8: INTEGRACION CON UXPILOT

### Configuracion

```json
// .claude/settings.local.json
{
  "uxpilot": {
    "enabled": true,
    "apiKey": "${UXPILOT_API_KEY}",
    "projectId": "weplay-rises",
    "autoGenerate": true,
    "outputDir": ".uxpilot/components"
  }
}
```

### Workflow Integrado

```
FLUJO UXPILOT + CLAUDE CODE:

1. SPEC con UxPilot
   /spec "Dashboard de artista"
        │
        └─► @spec-architect genera specs incluyendo:
            • Mockups requeridos de UxPilot
            • Componentes UI necesarios
            • Tokens de Design System

2. GENERAR UI con UxPilot
   /uxpilot generate "Artist Dashboard with stats cards and campaign list"
        │
        └─► UxPilot genera:
            • Mockup interactivo
            • Codigo base React
            • Variantes responsive

3. IMPLEMENTAR con ajustes
   /implement feat-... --uxpilot
        │
        └─► Claude Code:
            • Lee output de UxPilot
            • Adapta a Design System
            • Integra con estado real
            • Conecta con API
```

### Comando /uxpilot

```markdown
<!-- .claude/commands/uxpilot.md -->
# Comando: /uxpilot

## Proposito
Integra con UxPilot para generar UI profesional.

## Uso
```
/uxpilot generate "descripcion de UI"     # Generar nuevo componente
/uxpilot refine {component}               # Refinar componente existente
/uxpilot variants {component}             # Generar variantes
```

## Proceso

### Generar Componente

1. Enviar prompt a UxPilot API
2. Recibir mockup + codigo base
3. Guardar en `.uxpilot/components/{component}/`
4. Adaptar a stack del proyecto (Tailwind + shadcn)

### Integrar con /implement

Cuando se usa `--uxpilot` en /implement:

```typescript
// 1. Lee componente de UxPilot
const uxComponent = readFile('.uxpilot/components/ArtistDashboard/code.tsx');

// 2. Adapta imports y estilos
const adapted = adaptToProject(uxComponent, {
  designSystem: 'shadcn',
  stateManagement: 'tanstack-query',
  routing: 'nextjs'
});

// 3. Conecta con API real
const connected = connectToApi(adapted, {
  hooks: ['useArtistStats', 'useCampaigns'],
  endpoints: ['GET /api/artistas/{id}/stats']
});

// 4. Genera tests
const tests = generateTests(connected);
```

## Output Ejemplo

```
✅ UXPILOT: ArtistDashboard generado

📁 Archivos:
   .uxpilot/components/ArtistDashboard/
   ├── mockup.png           (preview visual)
   ├── code.tsx             (codigo base)
   ├── variants/
   │   ├── mobile.tsx
   │   └── tablet.tsx
   └── tokens.json          (tokens usados)

🎨 Componentes detectados:
   • StatsCard (4 variantes)
   • CampaignList
   • ProgressBar
   • UserAvatar

⏭️  Usa: /implement ... --uxpilot para integrar
```
```

---

## PARTE 9: MCP DB DIAGNOSTIC SERVER

### Configuracion

El servidor MCP para diagnostico de base de datos ya esta documentado en la estrategia de observabilidad. Aqui el resumen de integracion:

```json
// .claude/settings.local.json
{
  "mcpServers": {
    "db-diagnostic": {
      "command": "node",
      "args": [".claude/mcp/db-diagnostic/dist/index.js"],
      "env": {
        "DB_SERVER": "localhost",
        "DB_NAME": "WePlayRises",
        "DB_USER": "diagnostic_user",
        "DB_PASSWORD": "${DB_DIAGNOSTIC_PASSWORD}"
      }
    }
  }
}
```

### Herramientas Disponibles

| Tool | Descripcion | Uso |
|------|-------------|-----|
| `db_connect` | Conectar a SQL Server | Inicio de sesion |
| `db_recent_queries` | Queries ejecutadas | Durante tests |
| `db_slow_queries` | Queries lentas (>Xms) | Diagnostico |
| `db_table_schema` | Schema de tabla | Exploracion |
| `db_select` | Query SELECT (solo lectura) | Debug |
| `db_connection_status` | Estado de conexiones | Monitoreo |
| `db_active_transactions` | Transacciones activas | Debug |

### Uso en /diagnose

```typescript
// Dentro del comando /diagnose

// 1. Conectar
await mcp_db_diagnostic.db_connect();

// 2. Capturar queries durante tests
const queries = await mcp_db_diagnostic.db_recent_queries({
  seconds: 60,
  minDurationMs: 0
});

// 3. Identificar queries lentas
const slow = await mcp_db_diagnostic.db_slow_queries({
  thresholdMs: 100
});

// 4. Analizar y sugerir optimizaciones
for (const query of slow) {
  suggestOptimization(query);
}
```

---

## PARTE 10: TEMPLATES DE CODIGO

### Backend: Command

```csharp
// .claude/templates/backend/command.template.cs
// Variables: {{CommandName}}, {{ModuleName}}, {{Properties}}, {{Validations}}

using FluentValidation;
using MediatR;
using {{ModuleName}}.Domain;
using BuildingBlocks.Application;

namespace {{ModuleName}}.Application.Commands;

public record {{CommandName}}Command(
    {{Properties}}
) : IRequest<Result<{{CommandName}}Response>>;

public record {{CommandName}}Response(
    Guid Id,
    // ... response properties
);

public class {{CommandName}}CommandValidator : AbstractValidator<{{CommandName}}Command>
{
    public {{CommandName}}CommandValidator()
    {
        {{Validations}}
    }
}

public class {{CommandName}}CommandHandler : IRequestHandler<{{CommandName}}Command, Result<{{CommandName}}Response>>
{
    private readonly I{{ModuleName}}Repository _repository;
    private readonly ILogger<{{CommandName}}CommandHandler> _logger;

    public {{CommandName}}CommandHandler(
        I{{ModuleName}}Repository repository,
        ILogger<{{CommandName}}CommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<{{CommandName}}Response>> Handle(
        {{CommandName}}Command request,
        CancellationToken cancellationToken)
    {
        // #DIAG:{{CommandName:kebab}}-entry

        try
        {
            // Implementacion

            // #DIAG:{{CommandName:kebab}}-success
            return Result<{{CommandName}}Response>.Success(response);
        }
        catch (Exception ex)
        {
            // #DIAG:{{CommandName:kebab}}-error
            _logger.LogError(ex, "Error in {{CommandName}}");
            return Result<{{CommandName}}Response>.Failure(ex.Message);
        }
    }
}
```

### Frontend: Component

```tsx
// .claude/templates/frontend/component.template.tsx
// Variables: {{ComponentName}}, {{Props}}, {{Imports}}

'use client';

import { useState } from 'react';
{{Imports}}

interface {{ComponentName}}Props {
  {{Props}}
}

export function {{ComponentName}}({ ...props }: {{ComponentName}}Props) {
  // State

  // Handlers

  return (
    <div className="{{ComponentName:kebab}}">
      {/* Content */}
    </div>
  );
}

{{ComponentName}}.displayName = '{{ComponentName}}';
```

### Test: Unit

```csharp
// .claude/templates/tests/unit.template.cs
// Variables: {{TestSubject}}, {{MethodName}}, {{Fixture}}

using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.{{ModuleName}};

public class {{TestSubject}}Tests
{
    private readonly Mock<IDependency> _mockDependency;
    private readonly {{TestSubject}} _sut;

    public {{TestSubject}}Tests()
    {
        _mockDependency = new Mock<IDependency>();
        _sut = new {{TestSubject}}(_mockDependency.Object);
    }

    [Fact]
    public async Task {{MethodName}}_WhenValidInput_ShouldSucceed()
    {
        // Arrange
        var input = {{Fixture}}.Valid();

        // Act
        var result = await _sut.{{MethodName}}(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task {{MethodName}}_WhenInvalidInput_ShouldFail()
    {
        // Arrange
        var input = {{Fixture}}.Invalid();

        // Act
        var result = await _sut.{{MethodName}}(input);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
```

---

## PARTE 11: RESUMEN RAPIDO

### Comandos Principales

| Comando | Proposito | Output |
|---------|-----------|--------|
| `/spec "feature"` | Especificacion completa | `specs/{id}/` |
| `/synth-data {id}` | Datos sinteticos | Fixtures + Seeds |
| `/implement {id}` | Implementacion TDD | Codigo + Tests |
| `/diagnose {id}` | Diagnostico + Auto-fix | Reporte + Fixes |
| `/ship {id}` | Pipeline + PR | PR en Azure DevOps |
| `/uxpilot "ui"` | Generar UI | Componente React |

### Hooks Automaticos

| Hook | Trigger | Accion |
|------|---------|--------|
| load-project-context | SessionStart | Carga contexto |
| load-contextual-rules | PreToolUse | Reglas por archivo |
| tdd-reminder | PreToolUse (Write) | Recuerda TDD |
| auto-format | PostToolUse | Formatea codigo |
| detect-secrets | PostToolUse | Bloquea secrets |
| db-query-observer | PostToolUse (Bash) | Captura queries |
| strategic-compact | PreToolUse | Sugiere compactar |
| session-end-persistence | Stop | Guarda estado |

### Flujo Tipico

```bash
# 1. Especificar feature
/spec "Crear campania de crowdfunding"

# 2. Generar datos de prueba
/synth-data feat-20260123-crear-campania

# 3. Implementar con TDD
/implement feat-20260123-crear-campania

# 4. Diagnosticar y corregir
/diagnose feat-20260123-crear-campania

# 5. Crear PR
/ship feat-20260123-crear-campania
```

---

## PARTE 12: CHECKLIST DE SETUP

### Prerequisitos

- [ ] Claude Code instalado (`npm install -g @anthropic-ai/claude-code`)
- [ ] Azure CLI instalado (`az`)
- [ ] .NET 8 SDK
- [ ] Node.js 20+
- [ ] SQL Server (LocalDB o Docker)

### Configuracion Inicial

```bash
# 1. Clonar estructura de .claude/
# (se proporciona como template)

# 2. Configurar variables de entorno
cp .claude/settings.local.example.json .claude/settings.local.json
# Editar con valores reales

# 3. Instalar dependencias de hooks
pip install pyyaml jsonschema

# 4. Configurar MCP DB Server
cd .claude/mcp/db-diagnostic
npm install
npm run build

# 5. Configurar Azure DevOps
az login
az devops configure --defaults organization=https://dev.azure.com/weplay project=WePlayRises

# 6. Configurar UxPilot (opcional)
# Obtener API key de https://uxpilot.ai/
```

### Verificacion

```bash
# Verificar hooks
claude --verify-hooks

# Verificar MCP
claude --list-mcp-servers

# Test rapido
claude "di hola"
```

---

## PARTE 13: EVOLUCION FUTURA

### Fase 2: Mejoras Planificadas

| Mejora | Descripcion | Prioridad |
|--------|-------------|-----------|
| Agente E2E con Playwright | Tests E2E automatizados con browser real | Alta |
| Integracion con Figma | Sincronizar con Design System | Media |
| Metricas de productividad | Dashboard de velocity y coverage | Media |
| Auto-deploy a staging | Deploy automatico para review | Baja |

### Feedback Loop

```
CICLO DE MEJORA CONTINUA:

1. Usar el sistema
2. Identificar fricciones
3. Documentar en /remember
4. Implementar mejora
5. Validar mejora
6. Repetir
```

---

**Documento creado**: 2026-01-23
**Version**: 1.0
**Autor**: Ivan / Claude Code (Opus 4.5)

---

## Apendice A: Estructura de Archivos Completa

```
.claude/
├── CLAUDE.md
├── settings.local.json
├── hooks/
│   ├── settings.json
│   ├── session-persistence/
│   │   ├── session-start.py
│   │   ├── pre-compact.py
│   │   └── session-end.py
│   ├── quality-gates/
│   │   ├── auto-format.py
│   │   ├── tdd-reminder.py
│   │   └── detect-secrets.py
│   ├── diagnostic/
│   │   └── db-query-observer.py
│   ├── rules/
│   │   └── load-contextual-rules.py
│   └── strategic-compact/
│       └── suggest-compact.py
├── agents/
│   ├── spec-architect.agent.md
│   ├── spec-tester.agent.md
│   ├── spec-e2e.agent.md
│   ├── spec-diagnostic.agent.md
│   ├── implement-backend.agent.md
│   ├── implement-frontend.agent.md
│   ├── diagnose-resolver.agent.md
│   └── ship-reviewer.agent.md
├── commands/
│   ├── spec.md
│   ├── synth-data.md
│   ├── implement.md
│   ├── diagnose.md
│   ├── ship.md
│   └── uxpilot.md
├── templates/
│   ├── backend/
│   │   ├── command.template.cs
│   │   ├── query.template.cs
│   │   ├── controller.template.cs
│   │   └── entity.template.cs
│   ├── frontend/
│   │   ├── component.template.tsx
│   │   ├── page.template.tsx
│   │   ├── hook.template.ts
│   │   └── service.template.ts
│   └── tests/
│       ├── unit.template.cs
│       ├── integration.template.cs
│       ├── e2e.template.ts
│       └── fixture.template.json
├── rules/
│   ├── _index.yaml
│   ├── always/
│   ├── backend/
│   └── frontend/
├── contexts/
│   ├── dev.md
│   ├── review.md
│   └── research.md
└── mcp/
    └── db-diagnostic/
        ├── package.json
        └── src/index.ts
```
