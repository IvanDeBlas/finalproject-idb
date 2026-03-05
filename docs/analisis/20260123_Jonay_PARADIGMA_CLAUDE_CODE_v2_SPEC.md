# Especificaciones: Paradigma de Programacion Agentica con Claude Code v2.0

- **Fecha**: 2026-01-23
- **Proyecto Destino**: WePlay Rises (Crowdfunding Musical)
- **Stack**: .NET 8 Monolito Modular + React (Vite + Next.js) + SQL Server
- **CI/CD**: Azure DevOps Pipelines
- **Autor**: Jonay / Ivan / Claude Code (Opus 4.5)
- **Version**: 2.0 (Consolidada)

---

## Indice

1. [Vision General](#1-vision-general)
2. [Arquitectura del Sistema](#2-arquitectura-del-sistema)
3. [Sistema de Hooks](#3-sistema-de-hooks)
4. [Sistema de Templates](#4-sistema-de-templates)
5. [Comando /dev - Especificacion y Datos Sinteticos](#5-comando-dev---especificacion-y-datos-sinteticos)
6. [Comando /build - Worktree e Implementacion](#6-comando-build---worktree-e-implementacion)
7. [Comando /test - Datos Sinteticos y Autodiagnosis](#7-comando-test---datos-sinteticos-y-autodiagnosis)
8. [Comando /fix - Diagnosis y Auto-Resolve](#8-comando-fix---diagnosis-y-auto-resolve)
9. [Comando /ship - Pipeline y PR](#9-comando-ship---pipeline-y-pr)
10. [Integracion con UxPilot](#10-integracion-con-uxpilot)
11. [MCP Servers](#11-mcp-servers)
12. [Flujo Completo de Desarrollo](#12-flujo-completo-de-desarrollo)

---

## 1. Vision General

### 1.1 Objetivo

Este paradigma define un **sistema de desarrollo agentico** donde Claude Code actua como un desarrollador autonomo capaz de:

1. **Especificar** features completas con multiples agentes especializados en paralelo
2. **Generar datos sinteticos** como contrato de verificacion (inputs, outputs, estado BD)
3. **Implementar** codigo con puntos de diagnostico (#DIAG) insertados automaticamente
4. **Ejecutar y observar** la aplicacion en tiempo real (consola, Playwright, BD)
5. **Auto-corregir** cuando detecta discrepancias (maximo 3 intentos)
6. **Entregar** codigo limpio con tests, pipeline verde y PR lista

### 1.2 Principios de Diseno

| Principio | Descripcion |
|-----------|-------------|
| **Un comando, muchos agentes** | Cada comando orquesta multiples agentes especializados en paralelo |
| **Datos sinteticos como contrato** | Los datos de prueba definen el comportamiento esperado |
| **Observabilidad integrada** | #DIAG points permiten ver el flujo real vs esperado |
| **Auto-correccion agresiva** | 3 intentos automaticos antes de escalar |
| **Limpieza automatica** | El codigo de diagnostico nunca llega a produccion |

### 1.3 Diagrama de Flujo

```
┌───────────────────────────────────────────────────────────────────────────────────┐
│                        PARADIGMA DE DESARROLLO AGENTICO v2                        │
├───────────────────────────────────────────────────────────────────────────────────┤
│                                                                                   │
│   /dev "crear campania"     ──►  ESPECIFICACION + DATOS SINTETICOS                │
│        │                          • Arquitectura (DTOs, endpoints, CQRS)          │
│        │                          • YAML de diagnostico completo                  │
│        │                          • Escenarios de prueba                          │
│        ▼                                                                          │
│   /build                    ──►  WORKTREE + IMPLEMENTACION + #DIAG                │
│        │                          • Git worktree aislado                          │
│        │                          • Codigo segun templates                        │
│        │                          • Puntos de diagnostico inyectados              │
│        ▼                                                                          │
│   /test                     ──►  DATOS SINTETICOS + VERIFICACION                  │
│        │                          • Poblar BD con seed data                       │
│        │                          • Ejecutar escenarios (API + Playwright)        │
│        │                          • Capturar y comparar #DIAG                     │
│        ▼                                                                          │
│   /fix                      ──►  DIAGNOSIS + AUTO-RESOLVE                         │
│        │                          • Analizar discrepancias                        │
│        │                          • Aplicar auto-fix rules                        │
│        │                          • Re-ejecutar (max 3 intentos)                  │
│        ▼                                                                          │
│   /ship                     ──►  LIMPIAR + TESTS + PIPELINE + PR                  │
│                                   • Eliminar #DIAG del codigo                     │
│                                   • Ejecutar tests tradicionales                  │
│                                   • Azure DevOps Pipeline                         │
│                                   • Crear Pull Request                            │
│                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Arquitectura del Sistema

### 2.1 Estructura de Directorios

```
WePlay_Rises/
├── .claude/
│   ├── CLAUDE.md                      # Instrucciones principales
│   ├── settings.local.json            # Variables de entorno y MCP
│   │
│   ├── hooks/                         # Automatizacion silenciosa
│   │   ├── settings.json              # Configuracion de hooks
│   │   ├── session-persistence/       # Persistencia entre sesiones
│   │   │   ├── session-start.py       # Hook: SessionStart
│   │   │   ├── pre-compact.py         # Hook: PreCompact
│   │   │   └── session-end.py         # Hook: Stop
│   │   ├── quality-gates/             # Validacion automatica
│   │   │   ├── auto-format.py         # PostToolUse: Edit/Write
│   │   │   ├── detect-secrets.py      # PostToolUse: Edit/Write (BLOQUEA)
│   │   │   └── console-log-warning.py # PostToolUse: Edit/Write (JS/TS)
│   │   ├── diagnostic/                # Captura de #DIAG
│   │   │   ├── console-observer.py    # PostToolUse: Bash
│   │   │   └── db-query-observer.py   # PostToolUse: Bash
│   │   ├── rules/                     # Reglas contextuales
│   │   │   └── load-contextual-rules.py
│   │   ├── strategic-compact/         # Gestion de contexto
│   │   │   └── suggest-compact.py
│   │   └── continuous-learning/       # Aprendizaje automatico
│   │       ├── config.json
│   │       └── evaluate-session.py
│   │
│   ├── agents/                        # Agentes especializados
│   │   ├── spec/                      # Especificacion
│   │   │   ├── spec-architect.agent.md
│   │   │   ├── api-designer.agent.md
│   │   │   ├── frontend-designer.agent.md
│   │   │   └── synth-generator.agent.md
│   │   ├── impl/                      # Implementacion
│   │   │   ├── backend-developer.agent.md
│   │   │   ├── frontend-developer.agent.md
│   │   │   └── diag-injector.agent.md
│   │   ├── test/                      # Testing
│   │   │   ├── unit-test-generator.agent.md
│   │   │   ├── e2e-test-generator.agent.md
│   │   │   └── test-runner.agent.md
│   │   ├── diag/                      # Diagnostico
│   │   │   ├── diag-observer.agent.md
│   │   │   ├── diag-resolver.agent.md
│   │   │   └── diag-cleaner.agent.md
│   │   └── ship/                      # Entrega
│   │       ├── pipeline-runner.agent.md
│   │       └── pr-creator.agent.md
│   │
│   ├── commands/                      # Comandos unificados
│   │   ├── dev.md                     # /dev - Especificacion
│   │   ├── build.md                   # /build - Implementacion
│   │   ├── test.md                    # /test - Datos sinteticos
│   │   ├── fix.md                     # /fix - Autodiagnosis
│   │   ├── ship.md                    # /ship - Pipeline + PR
│   │   ├── ux.md                      # /ux - Integracion UxPilot
│   │   └── utils/
│   │       ├── remember.md
│   │       ├── memories.md
│   │       ├── forget.md
│   │       └── health-check.md
│   │
│   ├── templates/                     # Templates de codigo
│   │   ├── backend/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateCommand.template.cs
│   │   │   │   ├── UpdateCommand.template.cs
│   │   │   │   └── DeleteCommand.template.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetByIdQuery.template.cs
│   │   │   │   └── GetAllQuery.template.cs
│   │   │   ├── Validators/
│   │   │   │   ├── CreateValidator.template.cs
│   │   │   │   └── UpdateValidator.template.cs
│   │   │   ├── Services/
│   │   │   │   └── EntityService.template.cs
│   │   │   └── Controllers/
│   │   │       └── EntityController.template.cs
│   │   ├── frontend/
│   │   │   ├── components/
│   │   │   │   ├── Card.template.tsx
│   │   │   │   ├── Form.template.tsx
│   │   │   │   ├── List.template.tsx
│   │   │   │   └── Modal.template.tsx
│   │   │   ├── hooks/
│   │   │   │   ├── useQuery.template.ts
│   │   │   │   └── useMutation.template.ts
│   │   │   └── services/
│   │   │       └── api.template.ts
│   │   ├── diagnostic/
│   │   │   ├── diag-point-backend.template.cs
│   │   │   ├── diag-point-frontend.template.ts
│   │   │   └── diagnostic-data.template.yaml
│   │   └── tests/
│   │       ├── unit-test-backend.template.cs
│   │       ├── unit-test-frontend.template.tsx
│   │       └── e2e-test.template.ts
│   │
│   ├── rules/                         # Reglas contextuales
│   │   ├── _index.yaml                # Mapeo globs -> reglas
│   │   ├── always/
│   │   │   ├── code-style.rule.md
│   │   │   └── git-conventions.rule.md
│   │   ├── backend/
│   │   │   ├── cqrs.rule.md
│   │   │   ├── dotnet.rule.md
│   │   │   └── ef-core.rule.md
│   │   ├── frontend/
│   │   │   ├── react-shadcn.rule.md
│   │   │   └── typescript.rule.md
│   │   └── testing/
│   │       └── unit-tests.rule.md
│   │
│   ├── contexts/                      # Modos de trabajo
│   │   ├── dev.md                     # "Get it working > Get it right > Get it clean"
│   │   ├── review.md                  # Enfoque en calidad y patrones
│   │   └── research.md                # Exploracion sin ejecucion
│   │
│   ├── memories/                      # Preferencias persistentes
│   │   └── project.yaml               # Memories del proyecto
│   │
│   └── mcp/                           # MCP Servers
│       └── db-diagnostic/             # Server para queries SQL
│           ├── package.json
│           ├── tsconfig.json
│           └── src/
│               └── index.ts
│
├── specs/                             # Especificaciones generadas
│   ├── active/                        # Symlink al feature activo
│   │   └── diagnostic-data.yaml       # YAML activo para hooks
│   └── {feature-id}/
│       ├── architecture.md            # Arquitectura y DTOs
│       ├── api-spec.md                # Endpoints y contratos
│       ├── frontend-spec.md           # Componentes y flujos
│       ├── diagnostic-data.yaml       # YAML completo de diagnostico
│       └── SYNTHESIS.md               # Resumen para implementacion
│
├── .worktrees/                        # Worktrees aislados (gitignore)
│
├── tasks/                             # Tracking de tareas
│   └── backlog.md
│
└── src/                               # Codigo fuente
    ├── api/                           # Backend .NET
    ├── web/                           # Landing (Vite + React)
    ├── admin/                         # Dashboard (Next.js)
    └── shared/                        # Codigo compartido
```

### 2.2 Adaptaciones para WePlay Rises

| Aspecto | miUrba (Original) | WePlay Rises (Adaptado) |
|---------|-------------------|-------------------------|
| **Repos** | Multiples repos | Monorepo unico |
| **Backend** | Microservicios | Monolito Modular |
| **Frontend** | Blazor + React + MAUI | 2 apps React (Vite + Next.js) |
| **Auth** | Modulo separado | Integrada en WebApi |
| **BD** | SQL Server distribuido | SQL Server unico |
| **CI/CD** | Azure DevOps multi-repo | Azure DevOps single repo |

---

## 3. Sistema de Hooks

### 3.1 Configuracion Principal

```json
// .claude/hooks/settings.json
{
  "version": "2.0",
  "hooks": [
    {
      "name": "load-session-context",
      "trigger": "SessionStart",
      "script": ".claude/hooks/session-persistence/session-start.py",
      "enabled": true,
      "description": "Carga contexto de sesiones anteriores"
    },
    {
      "name": "save-pre-compact-state",
      "trigger": "PreCompact",
      "script": ".claude/hooks/session-persistence/pre-compact.py",
      "enabled": true,
      "description": "Guarda estado antes de compactar"
    },
    {
      "name": "load-contextual-rules",
      "trigger": "PreToolUse",
      "tools": ["Read", "Edit", "Write"],
      "script": ".claude/hooks/rules/load-contextual-rules.py",
      "enabled": true,
      "description": "Carga reglas segun tipo de archivo"
    },
    {
      "name": "auto-format",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/auto-format.py",
      "enabled": true,
      "description": "Ejecuta Prettier/ESLint/dotnet format"
    },
    {
      "name": "detect-secrets",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/detect-secrets.py",
      "enabled": true,
      "blocksOnFailure": true,
      "description": "BLOQUEA si encuentra secrets"
    },
    {
      "name": "console-log-warning",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/console-log-warning.py",
      "enabled": true,
      "filePatterns": ["*.ts", "*.tsx", "*.js", "*.jsx"],
      "description": "Advierte sobre console.log"
    },
    {
      "name": "diag-console-observer",
      "trigger": "PostToolUse",
      "tools": ["Bash"],
      "script": ".claude/hooks/diagnostic/console-observer.py",
      "enabled": true,
      "description": "Captura [DIAG:xxx] de consola y compara con YAML"
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
      "name": "strategic-compact-suggester",
      "trigger": "PreToolUse",
      "script": ".claude/hooks/strategic-compact/suggest-compact.py",
      "enabled": true,
      "description": "Sugiere compactar cada 50 tools"
    },
    {
      "name": "session-end-persistence",
      "trigger": "Stop",
      "script": ".claude/hooks/session-persistence/session-end.py",
      "enabled": true,
      "description": "Persiste estado al finalizar"
    },
    {
      "name": "evaluate-learning",
      "trigger": "Stop",
      "script": ".claude/hooks/continuous-learning/evaluate-session.py",
      "enabled": true,
      "description": "Extrae patrones para aprendizaje"
    }
  ]
}
```

### 3.2 Hook: Console Observer (#DIAG)

Este hook es el corazon del sistema de diagnostico. Captura outputs con formato `[DIAG:xxx]` y los compara con el YAML activo.

```python
#!/usr/bin/env python3
# .claude/hooks/diagnostic/console-observer.py
"""
Hook: diag-console-observer
Trigger: PostToolUse (Bash)
Captura [DIAG:xxx] de consola y compara con expected del YAML activo.
"""

import sys
import json
import re
import yaml
from pathlib import Path
from datetime import datetime

DIAG_LOG = Path('.claude/diagnostic/captured-diag.json')
ACTIVE_YAML = Path('specs/active/diagnostic-data.yaml')

def main():
    hook_input = json.loads(sys.stdin.read())
    output = hook_input.get('output', '')

    # Buscar todos los [DIAG:xxx] en el output
    diag_pattern = r'\[DIAG:([^\]]+)\]\s*(.+?)(?=\[DIAG:|$|\n\n)'
    matches = re.findall(diag_pattern, output, re.DOTALL)

    if not matches:
        return {"status": "pass"}

    captured = []
    for diag_name, data in matches:
        parsed = parse_diag_data(data.strip())
        captured.append({
            "name": diag_name,
            "timestamp": datetime.now().isoformat(),
            "data": parsed,
            "raw": data.strip()[:500]
        })

    # Guardar para analisis
    save_captured(captured)

    # Si hay YAML activo, comparar inmediatamente
    if ACTIVE_YAML.exists():
        discrepancies = compare_with_expected(captured)
        if discrepancies:
            return {
                "status": "fail",
                "message": format_discrepancies(discrepancies),
                "data": {
                    "captured": captured,
                    "discrepancies": discrepancies
                }
            }

    return {
        "status": "pass",
        "message": f"[DIAG] Capturados {len(captured)} puntos OK",
        "data": {"captured": captured}
    }

def parse_diag_data(data: str) -> dict:
    """Parsea 'key=value | key2=value2' a dict."""
    result = {}
    parts = re.split(r'\s*\|\s*', data)
    for part in parts:
        if '=' in part:
            key, value = part.strip().split('=', 1)
            result[key.strip()] = value.strip()
    return result

def save_captured(captured: list):
    """Guarda DIAGs capturados."""
    DIAG_LOG.parent.mkdir(parents=True, exist_ok=True)
    existing = []
    if DIAG_LOG.exists():
        existing = json.loads(DIAG_LOG.read_text())
    existing.extend(captured)
    existing = existing[-1000:]  # Mantener ultimos 1000
    DIAG_LOG.write_text(json.dumps(existing, indent=2))

def compare_with_expected(captured: list) -> list:
    """Compara DIAGs capturados con expected del YAML."""
    yaml_content = yaml.safe_load(ACTIVE_YAML.read_text())
    discrepancies = []

    for cap in captured:
        diag_name = cap['name']
        expected_point = find_diag_point(yaml_content, diag_name)
        if not expected_point:
            continue

        for var in expected_point.get('variables_captured', []):
            var_name = var['name']
            expected_value = var.get('expected')
            validation = var.get('validation', 'equals')
            actual_value = cap['data'].get(var_name)

            if not validate_value(actual_value, expected_value, validation):
                discrepancies.append({
                    "diag_name": diag_name,
                    "variable": var_name,
                    "expected": expected_value,
                    "actual": actual_value,
                    "validation": validation
                })

    return discrepancies

def find_diag_point(yaml_content: dict, diag_name: str) -> dict:
    """Encuentra punto DIAG en el YAML por nombre."""
    for point in yaml_content.get('diagnostic_points', []):
        if point.get('name') == diag_name:
            return point
    for point in yaml_content.get('diagnostic_points_frontend', []):
        if point.get('name') == diag_name:
            return point
    return None

def validate_value(actual, expected, validation: str) -> bool:
    """Valida segun tipo de validacion."""
    if actual is None and validation != 'is_null':
        return False
    if validation == 'equals':
        return str(actual) == str(expected)
    elif validation == 'not_null':
        return actual is not None
    elif validation == 'is_valid_guid':
        return bool(re.match(r'^[0-9a-f-]{36}$', str(actual), re.I))
    elif validation == 'less_than':
        return float(actual) < float(expected)
    elif validation == 'greater_than':
        return float(actual) > float(expected)
    elif validation == 'starts_with':
        return str(actual).startswith(str(expected))
    elif validation == 'is_empty':
        return actual == '' or actual == [] or actual is None
    elif validation == 'contains':
        return str(expected) in str(actual)
    return True

def format_discrepancies(discrepancies: list) -> str:
    """Formatea discrepancias para output."""
    lines = ["\n" + "=" * 70]
    lines.append("[DIAG] DISCREPANCIAS DETECTADAS - AUTO-FIX REQUERIDO")
    lines.append("=" * 70)

    for d in discrepancies[:5]:
        lines.append(f"\n  DIAG: {d['diag_name']}")
        lines.append(f"    Variable: {d['variable']}")
        lines.append(f"    Expected: {d['expected']}")
        lines.append(f"    Actual:   {d['actual']}")
        lines.append(f"    Validation: {d['validation']}")

    lines.append("\n" + "=" * 70)
    return "\n".join(lines)

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

### 3.3 Hook: Session Persistence

```python
#!/usr/bin/env python3
# .claude/hooks/session-persistence/session-start.py
"""
Hook: SessionStart
Carga contexto de sesiones anteriores.
"""

import json
from pathlib import Path
from datetime import datetime, timedelta

SESSIONS_DIR = Path.home() / '.claude' / 'sessions'
SKILLS_DIR = Path.home() / '.claude' / 'skills' / 'learned'

def main():
    SESSIONS_DIR.mkdir(parents=True, exist_ok=True)

    # Buscar sesiones recientes (ultimos 7 dias)
    cutoff = datetime.now() - timedelta(days=7)
    recent_sessions = []

    for session_file in SESSIONS_DIR.glob('session_*.json'):
        try:
            data = json.loads(session_file.read_text())
            session_time = datetime.fromisoformat(data.get('timestamp', ''))
            if session_time > cutoff:
                recent_sessions.append(data)
        except:
            continue

    # Cargar skills aprendidos
    learned_skills = []
    if SKILLS_DIR.exists():
        for skill_file in SKILLS_DIR.glob('*.json'):
            try:
                learned_skills.append(json.loads(skill_file.read_text()))
            except:
                continue

    context = {
        "recent_sessions_count": len(recent_sessions),
        "learned_skills_count": len(learned_skills),
        "loaded_at": datetime.now().isoformat()
    }

    # Mostrar resumen
    if recent_sessions or learned_skills:
        print(f"[SESSION] Contexto cargado:")
        print(f"  - {len(recent_sessions)} sesiones recientes")
        print(f"  - {len(learned_skills)} skills aprendidos")

    return {"status": "pass", "data": context}

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

### 3.4 Hook: Strategic Compact

```python
#!/usr/bin/env python3
# .claude/hooks/strategic-compact/suggest-compact.py
"""
Hook: PreToolUse
Sugiere compactacion cada 50 tool calls.
Filosofia: "Compactar despues de exploracion, antes de ejecucion"
"""

import json
from pathlib import Path

COUNTER_FILE = Path.home() / '.claude' / 'sessions' / 'tool_counter.txt'
COMPACT_THRESHOLD = 50
REMINDER_INTERVAL = 25

def main():
    COUNTER_FILE.parent.mkdir(parents=True, exist_ok=True)

    # Leer contador actual
    count = 0
    if COUNTER_FILE.exists():
        try:
            count = int(COUNTER_FILE.read_text().strip())
        except:
            count = 0

    # Incrementar
    count += 1
    COUNTER_FILE.write_text(str(count))

    # Sugerir compactacion
    if count == COMPACT_THRESHOLD:
        print("\n" + "=" * 60)
        print("[COMPACT] Has usado 50+ herramientas en esta sesion.")
        print("[COMPACT] Considera compactar si estas cambiando de fase.")
        print("[COMPACT] Comando: /compact o Ctrl+Shift+C")
        print("=" * 60 + "\n")
    elif count > COMPACT_THRESHOLD and (count - COMPACT_THRESHOLD) % REMINDER_INTERVAL == 0:
        print(f"\n[COMPACT] Recordatorio: {count} herramientas usadas.\n")

    return {"status": "pass", "tool_count": count}

if __name__ == '__main__':
    result = main()
    print(json.dumps(result))
```

---

## 4. Sistema de Templates

### 4.1 Template: CQRS Command + Handler

```csharp
// .claude/templates/backend/Commands/CreateCommand.template.cs
// Variables: {EntityName}, {ModuleName}, {Properties}

using FluentValidation;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Application.Common;
using WePlayRises.{ModuleName}.Application.Interfaces.Services;
using WePlayRises.{ModuleName}.Domain.Model;

namespace WePlayRises.{ModuleName}.Application.Features.{EntityName}.Commands;

/// <summary>
/// Command para crear {EntityName}.
/// </summary>
public class Create{EntityName}Command : IRequest<ServiceResponse<Guid>>
{
    {Properties}
}

/// <summary>
/// Handler para Create{EntityName}Command.
/// REGLA: Handler + Command en MISMO archivo.
/// </summary>
public class Create{EntityName}CommandHandler : IRequestHandler<Create{EntityName}Command, ServiceResponse<Guid>>
{
    private readonly I{EntityName}Service _service;
    private readonly IMapper _mapper;
    private readonly IValidator<Create{EntityName}Command> _validator;
    private readonly ILogger<Create{EntityName}CommandHandler> _logger;

    public Create{EntityName}CommandHandler(
        I{EntityName}Service service,
        IMapper mapper,
        IValidator<Create{EntityName}Command> validator,
        ILogger<Create{EntityName}CommandHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(Create{EntityName}Command request, CancellationToken ct)
    {
        try
        {
            // 1. Validacion
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<Guid>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Mapping y creacion
            var entity = _mapper.Map<{EntityName}>(request);
            var id = await _service.CreateAsync(entity, ct);

            // 3. Respuesta exitosa
            return new ServiceResponse<Guid>
            {
                Data = id,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "{EntityName} creado correctamente", ErrorCode = "SUCCESS" }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {EntityName}");
            return new ServiceResponse<Guid>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Error inesperado al crear {EntityName}", ErrorCode = "ERROR_UNEXPECTED" }
                }
            };
        }
    }
}
```

### 4.2 Template: Diagnostic Point (Backend)

```csharp
// .claude/templates/diagnostic/diag-point-backend.template.cs
// Variables: {DiagName}, {Variables}

// === DIAG SETUP (eliminar antes de PR) ===
var _diagStopwatch = System.Diagnostics.Stopwatch.StartNew();
// === END DIAG SETUP ===

// [DIAG:{DiagName}] - Eliminar antes de PR
_logger.LogInformation(
    "[DIAG:{DiagName}] timestamp={Timestamp} | {Variables}",
    DateTime.UtcNow.ToString("O"),
    {VariableValues});
```

### 4.3 Template: React Component con shadcn

```tsx
// .claude/templates/frontend/components/Form.template.tsx
// Variables: {ComponentName}, {Fields}, {Schema}

import { FC } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

const schema = z.object({
  {Schema}
});

type FormData = z.infer<typeof schema>;

interface {ComponentName}Props {
  onSubmit: (data: FormData) => void;
  isLoading?: boolean;
}

export const {ComponentName}: FC<{ComponentName}Props> = ({ onSubmit, isLoading }) => {
  const form = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      {DefaultValues}
    },
  });

  // [DIAG:{ComponentName}-rendered] - Eliminar antes de PR
  useEffect(() => {
    console.log('[DIAG:{ComponentName}-rendered]', JSON.stringify({
      timestamp: new Date().toISOString(),
      formFields: Object.keys(form.getValues()),
      isValid: form.formState.isValid,
    }));
  }, []);

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
        {Fields}
        <Button type="submit" disabled={isLoading}>
          {isLoading ? 'Guardando...' : 'Guardar'}
        </Button>
      </form>
    </Form>
  );
};
```

### 4.4 Template: Diagnostic Data YAML

```yaml
# .claude/templates/diagnostic/diagnostic-data.template.yaml
# Variables: {feature_id}, {feature_name}, {seed_data}, {diagnostic_points}, {scenarios}

version: "2.0"
feature_id: {feature_id}
feature_name: "{feature_name}"
created: {timestamp}
author: claude-code

# ═══════════════════════════════════════════════════════════════════════════════
# CONFIGURACION GLOBAL
# ═══════════════════════════════════════════════════════════════════════════════
config:
  api_base_url: "https://localhost:7001"
  web_base_url: "http://localhost:3000"
  admin_base_url: "http://localhost:3001"
  db_connection: "Server=localhost;Database=WePlayRises;Trusted_Connection=true"
  timeout_ms: 30000
  max_retry_attempts: 3
  screenshot_on_failure: true

# ═══════════════════════════════════════════════════════════════════════════════
# DATOS SEED (poblar BD antes de tests)
# ═══════════════════════════════════════════════════════════════════════════════
seed_data:
  {seed_data}

# ═══════════════════════════════════════════════════════════════════════════════
# PUNTOS DE DIAGNOSTICO BACKEND (#DIAG)
# ═══════════════════════════════════════════════════════════════════════════════
diagnostic_points:
  {diagnostic_points}

# ═══════════════════════════════════════════════════════════════════════════════
# PUNTOS DE DIAGNOSTICO FRONTEND (#DIAG)
# ═══════════════════════════════════════════════════════════════════════════════
diagnostic_points_frontend:
  {diagnostic_points_frontend}

# ═══════════════════════════════════════════════════════════════════════════════
# ESCENARIOS DE PRUEBA
# ═══════════════════════════════════════════════════════════════════════════════
scenarios:
  {scenarios}

# ═══════════════════════════════════════════════════════════════════════════════
# REGLAS DE AUTO-FIX
# ═══════════════════════════════════════════════════════════════════════════════
auto_fix_rules:
  {auto_fix_rules}
```

---

## 5. Comando /dev - Especificacion y Datos Sinteticos

### 5.1 Descripcion

El comando `/dev` es el punto de entrada para nuevas features. Orquesta **4 agentes en paralelo** para generar una especificacion completa con datos sinteticos.

```bash
/dev "Crear campania de crowdfunding con recompensas"
```

### 5.2 Agentes Involucrados

| Agente | Responsabilidad | Output |
|--------|-----------------|--------|
| `spec-architect` | Arquitectura general, entidades, relaciones | `architecture.md` |
| `api-designer` | Endpoints, DTOs, CQRS commands/queries | `api-spec.md` |
| `frontend-designer` | Componentes, flujos UI, states | `frontend-spec.md` |
| `synth-generator` | Datos sinteticos, escenarios, puntos #DIAG | `diagnostic-data.yaml` |

### 5.3 Comando Slash

```markdown
# .claude/commands/dev.md

<feature>
$ARGUMENTS
</feature>

# Comando: /dev

Genera especificacion completa para una feature con datos sinteticos.

## Instrucciones

### Paso 1: Validar Input

Verificar que la descripcion de la feature es clara y accionable.
Si es ambigua, pedir clarificacion al usuario.

### Paso 2: Crear Feature ID

Generar ID unico: `feat-{YYYYMMDD}-{kebab-case-nombre}`
Ejemplo: `feat-20260123-crear-campania`

### Paso 3: Lanzar Agentes en Paralelo

Usar el Task tool para lanzar 4 agentes simultaneamente:

1. **@spec-architect**
   Prompt: "Diseña la arquitectura para: {feature}.
   Stack: .NET 8 monolito modular, React (Vite + Next.js), SQL Server.
   Output: architecture.md con entidades, relaciones, modulos afectados."

2. **@api-designer**
   Prompt: "Diseña la API para: {feature}.
   Patron: CQRS con MediatR, ServiceResponse<T>.
   Output: api-spec.md con endpoints, DTOs, Commands, Queries."

3. **@frontend-designer**
   Prompt: "Diseña el frontend para: {feature}.
   Stack: React, shadcn/ui, TanStack Query, Zod.
   Apps: web (landing publica) y admin (dashboard artista).
   Output: frontend-spec.md con componentes, rutas, estados."

4. **@synth-generator**
   Prompt: "Genera datos sinteticos para: {feature}.
   Incluir: seed_data, diagnostic_points (backend + frontend),
   scenarios (happy path + errores), auto_fix_rules.
   Output: diagnostic-data.yaml"

### Paso 4: Crear Directorio de Specs

```
specs/{feature-id}/
├── architecture.md
├── api-spec.md
├── frontend-spec.md
├── diagnostic-data.yaml
└── SYNTHESIS.md
```

### Paso 5: Generar SYNTHESIS.md

Consolidar los 4 outputs en un resumen ejecutivo:
- Entidades principales
- Endpoints criticos
- Componentes clave
- Escenarios de prueba
- Checklist de implementacion

### Paso 6: Activar Feature

```bash
# Crear symlink al feature activo
rm specs/active
ln -s specs/{feature-id} specs/active
```

## Output Esperado

```
✅ ESPECIFICACION: {feature-id}

Arquitectura:
   ✅ 3 entidades: Campania, Reward, Backing
   ✅ 2 modulos: Crowdfunding, UserAccess

API:
   ✅ 5 endpoints: POST/GET/PUT campanias, rewards
   ✅ 4 Commands + 3 Queries

Frontend:
   ✅ web: CampaniaList, CampaniaDetail, BackingForm
   ✅ admin: CampaniaForm, RewardManager, Dashboard

Datos Sinteticos:
   ✅ 2 usuarios seed (artista + fan)
   ✅ 6 puntos #DIAG backend
   ✅ 4 puntos #DIAG frontend
   ✅ 3 escenarios (happy + 2 errores)
   ✅ 4 reglas auto-fix

⏭️  Siguiente: /build {feature-id}
```
```

---

## 6. Comando /build - Worktree e Implementacion

### 6.1 Descripcion

El comando `/build` crea un worktree aislado e implementa el codigo con puntos de diagnostico inyectados automaticamente.

```bash
/build feat-20260123-crear-campania
```

### 6.2 Agentes Involucrados

| Agente | Responsabilidad |
|--------|-----------------|
| `backend-developer` | Implementa CQRS, Services, Controllers |
| `frontend-developer` | Implementa componentes React, hooks, services |
| `diag-injector` | Inyecta puntos #DIAG segun YAML |

### 6.3 Comando Slash

```markdown
# .claude/commands/build.md

<feature-id>
$ARGUMENTS
</feature-id>

# Comando: /build

Implementa una feature con codigo y puntos de diagnostico.

## Instrucciones

### Paso 1: Validar Feature

1. Verificar que existe `specs/{feature-id}/`
2. Verificar que `diagnostic-data.yaml` esta completo
3. Leer `SYNTHESIS.md` para contexto

### Paso 2: Setup Worktree

```bash
# Crear branch y worktree
git worktree add .worktrees/{feature-id} -b feature/{feature-id}

# Activar diagnostico
ln -sf specs/{feature-id}/diagnostic-data.yaml specs/active/diagnostic-data.yaml
```

### Paso 3: Implementar Backend

Usar @backend-developer con el contexto de `api-spec.md`:

1. Crear entidades en Domain
2. Crear Commands/Queries con Handlers (MISMO archivo)
3. Crear Validators con Message + ErrorCode
4. Crear Services (retornan entidades, NO DTOs)
5. Crear AutoMapper Profiles
6. Crear Controllers

**Usar templates de**: `.claude/templates/backend/`

### Paso 4: Implementar Frontend

Usar @frontend-developer con el contexto de `frontend-spec.md`:

**Para web (Vite + React):**
1. Crear feature en `src/web/src/features/{feature}/`
2. Componentes con shadcn/ui
3. Hooks con TanStack Query
4. Validacion con Zod

**Para admin (Next.js):**
1. Crear paginas en `src/admin/src/app/(dashboard)/{feature}/`
2. Componentes compartidos
3. Server Actions si aplica

**Usar templates de**: `.claude/templates/frontend/`

### Paso 5: Inyectar Puntos #DIAG

Usar @diag-injector para:

1. Leer `diagnostic-data.yaml`
2. Para cada `diagnostic_point`:
   - Localizar archivo y linea
   - Insertar `requires_setup` (usings, stopwatch)
   - Insertar `code_to_inject`
3. Para cada `diagnostic_point_frontend`:
   - Insertar console.log con formato [DIAG:xxx]

### Paso 6: Verificar Build

```bash
# Backend
cd src/api && dotnet build WePlayRises.sln

# Frontend web
cd src/web && npm run build

# Frontend admin
cd src/admin && npm run build
```

## Output Esperado

```
✅ IMPLEMENTACION: {feature-id}

Worktree:
   ✅ .worktrees/{feature-id} creado
   ✅ Branch feature/{feature-id}

Backend:
   ✅ 3 entidades: Campania, Reward, Backing
   ✅ 4 Commands + 3 Queries + Handlers
   ✅ 4 Validators
   ✅ 3 Services
   ✅ 2 Controllers
   ✅ 6 puntos #DIAG inyectados

Frontend (web):
   ✅ CampaniaList, CampaniaDetail, BackingForm
   ✅ 2 puntos #DIAG inyectados

Frontend (admin):
   ✅ CampaniaForm, RewardManager, Dashboard
   ✅ 2 puntos #DIAG inyectados

Build:
   ✅ dotnet build: OK
   ✅ npm run build (web): OK
   ✅ npm run build (admin): OK

⏭️  Siguiente: /test {feature-id}
```
```

---

## 7. Comando /test - Datos Sinteticos y Autodiagnosis

### 7.1 Descripcion

El comando `/test` puebla la base de datos con datos sinteticos y ejecuta los escenarios definidos en el YAML, capturando cada punto #DIAG.

```bash
/test feat-20260123-crear-campania
```

### 7.2 Proceso

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         LOOP DE TESTING                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   1. PREPARAR                                                               │
│      └─► Poblar BD con seed_data (usuarios, artistas)                       │
│      └─► Generar JWT para auth                                              │
│                                                                             │
│   2. EJECUTAR ESCENARIOS                                                    │
│      ├─► API: curl/httpie segun input                                       │
│      └─► UI: Playwright segun steps                                         │
│                                                                             │
│   3. OBSERVAR (en cada #DIAG)                                               │
│      ├─► Hook captura [DIAG:xxx] de consola                                 │
│      ├─► Parsea variables                                                   │
│      └─► Compara con expected del YAML                                      │
│                                                                             │
│   4. VERIFICAR BD                                                           │
│      ├─► Ejecutar queries de expected_db_state                              │
│      └─► Comparar assertions                                                │
│                                                                             │
│   5. GENERAR REPORTE                                                        │
│      └─► specs/{feature-id}/test-results.md                                 │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 7.3 Comando Slash

```markdown
# .claude/commands/test.md

<feature-id>
$ARGUMENTS
</feature-id>

# Comando: /test

Ejecuta escenarios de prueba con datos sinteticos.

## Instrucciones

### Paso 1: Cargar YAML

```python
yaml_path = f"specs/{feature_id}/diagnostic-data.yaml"
data = yaml.safe_load(Path(yaml_path).read_text())
```

### Paso 2: Poblar BD con Seed Data

Usar MCP db-diagnostic para:

1. Limpiar datos de test previos
2. Insertar `seed_data.users`
3. Insertar `seed_data.artistas`
4. Insertar cualquier otra entidad seed

### Paso 3: Iniciar Servicios

```bash
# Backend
cd src/api && dotnet run --project WebApi &

# Frontend web (si hay tests UI)
cd src/web && npm run dev &

# Frontend admin (si hay tests UI)
cd src/admin && npm run dev &
```

### Paso 4: Ejecutar Escenarios

Para cada `scenario` en el YAML:

**Tipo `api_happy_path` o `api_validation_error`:**
```bash
# Generar JWT
TOKEN=$(curl -s -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@test.com", "password": "Test123!"}' \
  | jq -r '.token')

# Ejecutar request
curl -X POST https://localhost:7001/api/{endpoint} \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{body_json}'
```

**Tipo `e2e_ui`:**
Usar Playwright MCP para:
1. Navegar a URL
2. Ejecutar `steps` (fill, click, wait)
3. Capturar screenshots
4. Verificar console logs

### Paso 5: Capturar y Comparar #DIAG

El hook `diag-console-observer` automaticamente:
1. Captura [DIAG:xxx] de consola
2. Compara con `diagnostic_points`
3. Reporta discrepancias

### Paso 6: Verificar Estado BD

Usar MCP db-diagnostic para cada `expected_db_state`:

```sql
-- Query definida en YAML
SELECT * FROM Campanias WHERE Id = '{stored.created_id}'

-- Verificar assertions
-- column: Titulo, value: "Mi Album"
-- column: Estado, value: 1
```

### Paso 7: Generar Reporte

```
specs/{feature-id}/test-results.md
```

## Output Esperado

```
═══════════════════════════════════════════════════════════════════════════════
TESTING: {feature-id}
═══════════════════════════════════════════════════════════════════════════════

SEED DATA:
  ✅ 2 usuarios insertados
  ✅ 1 artista insertado

───────────────────────────────────────────────────────────────────────────────

ESCENARIO: crear_campania_valida_api (api_happy_path)
───────────────────────────────────────────────────────────────────────────────

PUNTOS #DIAG:
  ✅ DIAG-001 create-campaign-entry
     • UserId = {guid} ✓
     • Titulo = "Mi Album" ✓

  ✅ DIAG-002 create-campaign-validated
     • IsValid = true ✓
     • ErrorCount = 0 ✓

  ✅ DIAG-003 create-campaign-pre-save
     • EntityState = Added ✓
     • Estado = Borrador ✓

  ✅ DIAG-004 create-campaign-post-save
     • CampaniaId = {guid} ✓
     • DurationMs = 45 (<1000) ✓

RESPONSE:
  ✅ status_code = 201 ✓
  ✅ body.titulo = "Mi Album" ✓

BASE DE DATOS:
  ✅ Campanias.Titulo = "Mi Album" ✓
  ✅ Campanias.Estado = 1 ✓

───────────────────────────────────────────────────────────────────────────────

ESCENARIO: crear_campania_titulo_vacio (api_validation_error)
───────────────────────────────────────────────────────────────────────────────

PUNTOS #DIAG:
  ✅ DIAG-001 create-campaign-entry
     • Titulo = "" ✓

  ✅ DIAG-002 create-campaign-validated
     • IsValid = false ✓
     • ErrorCount = 1 ✓

RESPONSE:
  ✅ status_code = 400 ✓

═══════════════════════════════════════════════════════════════════════════════
RESUMEN
═══════════════════════════════════════════════════════════════════════════════

Escenarios ejecutados: 3/3
Puntos #DIAG verificados: 12/12
Verificaciones BD: 6/6

✅ TODOS LOS TESTS PASARON

⏭️  Siguiente: /ship {feature-id}
```

Si hay errores:

```
❌ ERRORES DETECTADOS - Ejecutar /fix {feature-id}

Discrepancias:
  DIAG-002: IsValid=true (expected: false)

⏭️  Siguiente: /fix {feature-id}
```
```

---

## 8. Comando /fix - Diagnosis y Auto-Resolve

### 8.1 Descripcion

El comando `/fix` analiza las discrepancias detectadas, aplica auto-fix rules, y re-ejecuta los tests. Maximo 3 intentos antes de escalar.

```bash
/fix feat-20260123-crear-campania
```

### 8.2 Agentes Involucrados

| Agente | Responsabilidad |
|--------|-----------------|
| `diag-observer` | Analiza discrepancias y sugiere fixes |
| `diag-resolver` | Aplica fixes automaticamente |

### 8.3 Comando Slash

```markdown
# .claude/commands/fix.md

<feature-id>
$ARGUMENTS
</feature-id>

# Comando: /fix

Diagnostica y resuelve automaticamente errores detectados.

## Instrucciones

### Paso 1: Cargar Discrepancias

Leer `.claude/diagnostic/captured-diag.json` para obtener:
- Puntos #DIAG que fallaron
- Valores esperados vs actuales
- Tipo de validacion

### Paso 2: Buscar Auto-Fix Rules

Para cada discrepancia, buscar en `diagnostic-data.yaml > auto_fix_rules`:

```yaml
auto_fix_rules:
  - id: "fix-001"
    error_pattern: "IsValid=False.*Titulo.*obligatorio"
    diagnosis: "Falta validacion NotEmpty para Titulo"
    fix:
      file: "CreateCampaignCommandValidator.cs"
      action: "add_rule"
      code: |
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El titulo es obligatorio")
            .WithErrorCode("VALIDATION_REQUIRED");
```

### Paso 3: Aplicar Fixes (Intento 1-3)

Para cada fix encontrado:

1. Localizar archivo
2. Aplicar modificacion segun `action`:
   - `add_rule`: Insertar codigo en posicion indicada
   - `add_statement`: Agregar linea antes/despues de otra
   - `replace`: Reemplazar patron
   - `suggest`: Solo mostrar sugerencia (no auto-fix)
3. Rebuild

### Paso 4: Re-ejecutar /test

```bash
/test {feature-id}
```

### Paso 5: Evaluar Resultado

**Si pasa:** Continuar a /ship
**Si falla y intento < 3:** Volver al Paso 3
**Si falla y intento = 3:** Escalar a humano

## Ciclo de Auto-Fix

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         CICLO DE AUTO-FIX                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   INTENTO 1                                                                 │
│   ├─► Analizar discrepancias                                                │
│   ├─► Buscar auto_fix_rules                                                 │
│   ├─► Aplicar fix-001: Agregar validacion                                   │
│   ├─► Re-build                                                              │
│   └─► Re-test                                                               │
│        │                                                                    │
│        ├─► ✅ PASA → /ship                                                 │
│        │                                                                    │
│        └─► ❌ FALLA → INTENTO 2                                             │
│             ├─► Analizar nueva discrepancia                                 │
│             ├─► Aplicar fix-002                                             │
│             └─► Re-test                                                     │
│                  │                                                          │
│                  ├─► ✅ PASA → /ship                                        │
│                  │                                                          │
│                  └─► ❌ FALLA → INTENTO 3                                   │
│                       ├─► Ultimo intento                                    │
│                       └─► Re-test                                           │
│                            │                                                │
│                            ├─► ✅ PASA → /ship                              │
│                            │                                                │
│                            └─► ❌ FALLA → ESCALAR                           │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Output Esperado (Exito)

```
═══════════════════════════════════════════════════════════════════════════════
FIX: {feature-id}
═══════════════════════════════════════════════════════════════════════════════

INTENTO 1/3
───────────────────────────────────────────────────────────────────────────────

Discrepancias detectadas:
  ❌ DIAG-002: IsValid=true (expected: false)

Auto-fix encontrado:
  🔧 fix-001: Agregar validacion NotEmpty para Titulo
     Archivo: CreateCampaignCommandValidator.cs
     Accion: add_rule

Aplicando fix...
  ✅ Codigo insertado en linea 15

Re-building...
  ✅ dotnet build: OK

Re-testing...
  ✅ DIAG-002: IsValid=false ✓

═══════════════════════════════════════════════════════════════════════════════

✅ FIX EXITOSO en intento 1

Auto-fixes aplicados: 1
Re-tests ejecutados: 1

⏭️  Siguiente: /ship {feature-id}
```

## Output (Escalacion)

```
═══════════════════════════════════════════════════════════════════════════════
FIX: {feature-id}
═══════════════════════════════════════════════════════════════════════════════

❌ MAXIMO DE INTENTOS ALCANZADO (3/3)

Discrepancias sin resolver:
  DIAG-005: DurationMs=2500 (expected: <1000)

Sugerencias:
  • Revisar query performance en CampaniaService.CreateAsync
  • Considerar agregar indice IX_Campanias_ArtistaId
  • Verificar N+1 queries

Archivos para revisar:
  • src/api/Modules/Crowdfunding/Infra/Services/CampaniaService.cs
  • src/api/Modules/Crowdfunding/Infra/Data/CampaniaConfiguration.cs

⚠️  REQUIERE INTERVENCION MANUAL
```
```

---

## 9. Comando /ship - Pipeline y PR

### 9.1 Descripcion

El comando `/ship` limpia el codigo de diagnostico, ejecuta tests tradicionales, corre la pipeline CI, y crea el Pull Request.

```bash
/ship feat-20260123-crear-campania
```

### 9.2 Agentes Involucrados

| Agente | Responsabilidad |
|--------|-----------------|
| `diag-cleaner` | Elimina todos los #DIAG del codigo |
| `unit-test-generator` | Genera tests unitarios |
| `e2e-test-generator` | Genera tests E2E |
| `pipeline-runner` | Ejecuta Azure DevOps Pipeline |
| `pr-creator` | Crea Pull Request |

### 9.3 Comando Slash

```markdown
# .claude/commands/ship.md

<feature-id>
$ARGUMENTS
</feature-id>

# Comando: /ship

Limpia codigo, ejecuta tests, pipeline y crea PR.

## Instrucciones

### Paso 1: Limpiar Codigo #DIAG

Usar @diag-cleaner para:

1. Buscar todos los archivos con `[DIAG:`
2. Eliminar bloques completos de diagnostico:
   - Comentarios `// [DIAG:xxx]`
   - Lineas de `_logger.LogInformation("[DIAG:...`
   - Lineas de `console.log('[DIAG:...`
   - Setup code (`var _diagStopwatch = ...`)
   - Usings de diagnostico (`using System.Diagnostics;` si no se usa)
3. Verificar que build sigue funcionando

### Paso 2: Generar Tests Tradicionales

**Unit Tests (Backend):**
```csharp
// Tests para cada Command Handler
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var command = new CreateCampaniaCommand { ... };
    _validatorMock.Setup(v => v.ValidateAsync(...)).ReturnsAsync(valid);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotEqual(Guid.Empty, result.Data);
}
```

**Unit Tests (Frontend):**
```tsx
describe('CampaniaForm', () => {
  it('renders all required fields', () => {
    render(<CampaniaForm onSubmit={vi.fn()} />);
    expect(screen.getByLabelText('Titulo')).toBeInTheDocument();
  });

  it('validates required fields', async () => {
    render(<CampaniaForm onSubmit={vi.fn()} />);
    fireEvent.click(screen.getByRole('button', { name: /guardar/i }));
    expect(await screen.findByText(/titulo.*obligatorio/i)).toBeInTheDocument();
  });
});
```

**E2E Tests:**
```typescript
test('crear campania flujo completo', async ({ page }) => {
  await page.goto('/dashboard/campanias/nueva');
  await page.fill('[name="titulo"]', 'Mi Album');
  await page.fill('[name="importeObjetivo"]', '5000');
  await page.click('button[type="submit"]');
  await expect(page).toHaveURL(/\/dashboard\/campanias\/[a-f0-9-]+/);
});
```

### Paso 3: Ejecutar Tests

```bash
# Unit tests backend
cd src/api && dotnet test --configuration Release --logger "console;verbosity=normal"

# Unit tests frontend
cd src/web && npm run test
cd src/admin && npm run test

# E2E tests (si aplica)
cd src/web && npm run test:e2e
```

### Paso 4: Validaciones de Calidad

```bash
# Lint
cd src/web && npm run lint
cd src/admin && npm run lint

# Type check
cd src/web && npm run type-check
cd src/admin && npm run type-check

# Build release
cd src/api && dotnet build --configuration Release
```

### Paso 5: Git Commit

```bash
cd .worktrees/{feature-id}

# Stage cambios
git add .

# Commit con formato
git commit -m "$(cat <<'EOF'
feat: {feature-name}

- Implementar {entidades}
- Agregar endpoints {endpoints}
- Crear componentes {componentes}
- Tests unitarios y E2E

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
EOF
)"
```

### Paso 6: Push y Pipeline

```bash
# Push branch
git push -u origin feature/{feature-id}

# Trigger pipeline (si no es automatico)
az pipelines run --name "WePlayRises-CI" --branch feature/{feature-id}
```

### Paso 7: Crear Pull Request

```bash
gh pr create \
  --title "feat: {feature-name}" \
  --body "$(cat <<'EOF'
## Summary
- {bullet points de cambios}

## Test plan
- [ ] Unit tests backend pasan
- [ ] Unit tests frontend pasan
- [ ] E2E tests pasan
- [ ] Pipeline CI verde
- [ ] Code review

## Screenshots
{si aplica}

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

## Output Esperado

```
═══════════════════════════════════════════════════════════════════════════════
SHIP: {feature-id}
═══════════════════════════════════════════════════════════════════════════════

1. LIMPIEZA #DIAG
───────────────────────────────────────────────────────────────────────────────
   ✅ 6 puntos #DIAG eliminados de backend
   ✅ 4 puntos #DIAG eliminados de frontend
   ✅ Setup code eliminado
   ✅ Build verificado post-limpieza

2. TESTS GENERADOS
───────────────────────────────────────────────────────────────────────────────
   ✅ 8 unit tests backend generados
   ✅ 6 unit tests frontend generados
   ✅ 2 E2E tests generados

3. EJECUCION DE TESTS
───────────────────────────────────────────────────────────────────────────────
   ✅ Unit Tests Backend: 8/8 passed (2.3s)
   ✅ Unit Tests Frontend (web): 4/4 passed (1.1s)
   ✅ Unit Tests Frontend (admin): 2/2 passed (0.8s)
   ✅ E2E Tests: 2/2 passed (15.2s)

4. VALIDACIONES
───────────────────────────────────────────────────────────────────────────────
   ✅ ESLint: No errors
   ✅ TypeScript: No type errors
   ✅ Build Release: OK

5. GIT
───────────────────────────────────────────────────────────────────────────────
   ✅ Commit: abc1234
   ✅ Push: origin/feature/{feature-id}

6. PIPELINE
───────────────────────────────────────────────────────────────────────────────
   ✅ Pipeline: Build #42 - Succeeded
   🔗 https://dev.azure.com/weplay/WePlayRises/_build/results?buildId=42

7. PULL REQUEST
───────────────────────────────────────────────────────────────────────────────
   ✅ PR #15 creado
   🔗 https://dev.azure.com/weplay/WePlayRises/_git/WePlayRises/pullrequest/15

═══════════════════════════════════════════════════════════════════════════════

📋 YAML de diagnostico guardado para referencia futura:
   specs/{feature-id}/diagnostic-data.yaml

✅ FEATURE LISTA PARA REVIEW
```
```

---

## 10. Integracion con UxPilot

### 10.1 Descripcion

[UxPilot](https://uxpilot.ai/) es una herramienta de IA para generar disenos de UI. Se integra en el flujo para mejorar la calidad visual de los componentes.

### 10.2 Comando /ux

```markdown
# .claude/commands/ux.md

<descripcion-ui>
$ARGUMENTS
</descripcion-ui>

# Comando: /ux

Genera o mejora UI usando UxPilot.

## Instrucciones

### Paso 1: Preparar Prompt para UxPilot

Construir prompt detallado incluyendo:
- Descripcion del componente
- Stack: React, Tailwind, shadcn/ui
- Estilo: Moderno, limpio, profesional
- Restricciones: Accesibilidad, responsive

### Paso 2: Llamar a UxPilot API

```bash
curl -X POST https://api.uxpilot.ai/v1/generate \
  -H "Authorization: Bearer $UXPILOT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "{prompt}",
    "framework": "react",
    "styling": "tailwind",
    "components": "shadcn"
  }'
```

### Paso 3: Adaptar Output

1. Recibir codigo generado
2. Adaptar imports a estructura del proyecto
3. Reemplazar componentes genericos por shadcn
4. Ajustar estilos a design tokens del proyecto

### Paso 4: Inyectar #DIAG

Si es parte de una feature activa, agregar puntos de diagnostico correspondientes.

## Output Esperado

```
✅ UX generado para: {descripcion}

Componentes creados:
   ✅ CampaniaHeroSection.tsx
   ✅ RewardTierCard.tsx
   ✅ ProgressBar.tsx

Adaptaciones aplicadas:
   ✅ Imports ajustados a @/components/ui
   ✅ Button → shadcn Button
   ✅ Card → shadcn Card
   ✅ Colores → Design tokens

Preview disponible en:
   http://localhost:3000/preview/{component}
```

## Uso en Flujo /build

```bash
# Durante /build, si hay UI compleja:
/ux "Landing de campania con hero, progress bar, grid de recompensas, y call-to-action"

# El output se integra automaticamente en el feature activo
```
```

---

## 11. MCP Servers

### 11.1 DB Diagnostic Server

MCP Server para ejecutar queries de diagnostico en la base de datos.

```typescript
// .claude/mcp/db-diagnostic/src/index.ts
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import sql from "mssql";

const server = new Server({
  name: "mcp-db-diagnostic",
  version: "1.0.0",
}, {
  capabilities: { tools: {} }
});

let pool: sql.ConnectionPool;

// Tools disponibles
server.setRequestHandler("tools/list", async () => ({
  tools: [
    {
      name: "db_connect",
      description: "Conecta a la base de datos",
      inputSchema: { type: "object", properties: {} }
    },
    {
      name: "db_select",
      description: "Ejecuta query SELECT (solo lectura)",
      inputSchema: {
        type: "object",
        properties: {
          query: { type: "string", description: "Query SELECT" }
        },
        required: ["query"]
      }
    },
    {
      name: "db_insert_seed",
      description: "Inserta datos seed para testing",
      inputSchema: {
        type: "object",
        properties: {
          table: { type: "string" },
          data: { type: "object" }
        },
        required: ["table", "data"]
      }
    },
    {
      name: "db_clean_test_data",
      description: "Limpia datos de test",
      inputSchema: {
        type: "object",
        properties: {
          prefix: { type: "string", default: "test_" }
        }
      }
    },
    {
      name: "db_recent_queries",
      description: "Ver queries recientes ejecutadas",
      inputSchema: {
        type: "object",
        properties: {
          seconds: { type: "number", default: 60 }
        }
      }
    },
    {
      name: "db_slow_queries",
      description: "Ver queries lentas",
      inputSchema: {
        type: "object",
        properties: {
          threshold_ms: { type: "number", default: 500 }
        }
      }
    }
  ]
}));

// Implementacion de tools...
```

### 11.2 Configuracion

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
    },
    "context7": {
      "command": "npx",
      "args": ["-y", "@context7/mcp-server"]
    },
    "playwright": {
      "command": "npx",
      "args": ["-y", "@anthropic/mcp-playwright"]
    }
  }
}
```

---

## 12. Flujo Completo de Desarrollo

### 12.1 Ejemplo: Crear Feature "Campania de Crowdfunding"

```bash
# 1. ESPECIFICACION (5-10 min)
/dev "Crear campania de crowdfunding con titulo, descripcion, meta financiera,
      fechas, recompensas, y publicacion"

# Output:
# - specs/feat-20260123-crear-campania/
#   - architecture.md
#   - api-spec.md
#   - frontend-spec.md
#   - diagnostic-data.yaml
#   - SYNTHESIS.md

# 2. IMPLEMENTACION (15-30 min)
/build feat-20260123-crear-campania

# Output:
# - .worktrees/feat-20260123-crear-campania/
# - Codigo backend: Commands, Queries, Services, Controllers
# - Codigo frontend: Components, Hooks, Pages
# - Puntos #DIAG inyectados

# 3. TESTING (5-10 min)
/test feat-20260123-crear-campania

# Output:
# - Seed data insertado
# - 3 escenarios ejecutados
# - 12 puntos #DIAG verificados
# - (Si hay errores) → /fix

# 4. AUTO-FIX (si es necesario)
/fix feat-20260123-crear-campania

# Output:
# - Discrepancias analizadas
# - Auto-fixes aplicados
# - Re-test ejecutado
# - (Max 3 intentos)

# 5. ENTREGA (5-10 min)
/ship feat-20260123-crear-campania

# Output:
# - Codigo limpio (sin #DIAG)
# - Tests generados y pasando
# - Pipeline verde
# - PR creada

# TOTAL: ~40-70 minutos por feature
```

### 12.2 Resumen de Comandos

| Comando | Descripcion | Agentes | Output |
|---------|-------------|---------|--------|
| `/dev` | Especificacion completa | 4 paralelos | specs/{feature}/ |
| `/build` | Worktree + implementacion | 3 secuenciales | .worktrees/{feature}/ |
| `/test` | Datos sinteticos + tests | test-runner | test-results.md |
| `/fix` | Diagnosis + auto-resolve | 2 secuenciales | codigo corregido |
| `/ship` | Limpiar + PR | 4 secuenciales | PR en Azure DevOps |
| `/ux` | Generar/mejorar UI | 1 | componentes React |

### 12.3 Comandos Auxiliares

| Comando | Descripcion |
|---------|-------------|
| `/remember "texto"` | Guardar preferencia persistente |
| `/memories` | Listar preferencias guardadas |
| `/forget mem-XXX` | Eliminar preferencia |
| `/health-check` | Validar sistema completo |
| `/context-mode dev\|review\|research` | Cambiar modo de trabajo |

---

## Apendice A: Checklist de Implementacion

### Fase 1: Infraestructura Base (8h)

- [ ] Crear estructura `.claude/` completa
- [ ] Configurar hooks en `settings.json`
- [ ] Implementar `console-observer.py`
- [ ] Implementar `session-start.py` y `session-end.py`
- [ ] Implementar `suggest-compact.py`
- [ ] Crear templates backend (.cs)
- [ ] Crear templates frontend (.tsx)
- [ ] Crear template diagnostic-data.yaml

### Fase 2: Comandos Principales (12h)

- [ ] Implementar `/dev` con 4 agentes
- [ ] Implementar `/build` con worktree
- [ ] Implementar `/test` con seed data
- [ ] Implementar `/fix` con auto-resolve
- [ ] Implementar `/ship` con pipeline

### Fase 3: MCP y Integraciones (6h)

- [ ] Crear MCP db-diagnostic
- [ ] Configurar Playwright MCP
- [ ] Integrar UxPilot API
- [ ] Configurar Azure DevOps CLI

### Fase 4: Validacion y Documentacion (4h)

- [ ] Probar flujo E2E completo
- [ ] Ajustar auto_fix_rules
- [ ] Documentar troubleshooting
- [ ] Crear video demo

**Total estimado: 30 horas**

---

## Apendice B: Diferencias con v1.0

| Aspecto | v1.0 | v2.0 |
|---------|------|------|
| **Comandos** | 5 separados (/spec, /synth-data, /implement, /diagnose, /ship) | 5 consolidados (/dev, /build, /test, /fix, /ship) |
| **Datos sinteticos** | Comando separado | Integrado en /dev |
| **Worktree** | Manual | Automatico en /build |
| **UxPilot** | Flag opcional | Comando dedicado /ux |
| **Tests** | Implícitos en /diagnose | Explicitos en /test |
| **Auto-fix** | En /diagnose | Comando dedicado /fix |
| **Hooks** | Basicos | Completos (11 hooks) |
| **Templates** | Parciales | Completos (backend + frontend + diag) |
| **Stack** | Generico | Adaptado a WePlay Rises |

---

**Documento creado**: 2026-01-23
**Version**: 2.0
**Autor**: Jonay / Ivan / Claude Code (Opus 4.5)
