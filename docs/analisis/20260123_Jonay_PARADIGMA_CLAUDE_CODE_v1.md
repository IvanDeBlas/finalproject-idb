# Paradigma de Programacion Agentica con Claude Code v1.0

- **Fecha**: 2026-01-23
- **Proyecto**: WePlay Rises
- **Stack**: .NET 8 (Monolito Modular) + React (Vite + Next.js) + SQL Server
- **CI/CD**: Azure DevOps Pipelines
- **Autor**: Ivan / Claude Code

---

## Vision General

Este documento define un **paradigma de programacion agentica** donde Claude Code actua como un desarrollador autonomo que:

1. **Especifica** features con multiples agentes en paralelo
2. **Genera datos sinteticos** como contrato de verificacion (inputs, outputs esperados en cada #DIAG, estado final en BD)
3. **Implementa** codigo con puntos de diagnostico (#DIAG) insertados en ubicaciones exactas
4. **Ejecuta y observa** la aplicacion en tiempo real (consola, Playwright, BD)
5. **Compara** cada punto #DIAG con el expected del YAML
6. **Auto-corrige agresivamente** cuando detecta discrepancias (max 3 intentos)
7. **Limpia** los #DIAG y crea PR con tests tradicionales

```
┌───────────────────────────────────────────────────────────────────────────────────┐
│                        PARADIGMA DE DESARROLLO AGENTICO                           │
├───────────────────────────────────────────────────────────────────────────────────┤
│                                                                                   │
│   /spec "feature"        ──►  ESPECIFICACION                                      │
│        │                      (Arquitectura, DTOs, endpoints)                     │
│        ▼                                                                          │
│   /synth-data            ──►  YAML DE DIAGNOSTICO COMPLETO                        │
│        │                      • Inputs para cada escenario                        │
│        │                      • Puntos #DIAG con ubicacion exacta (archivo+linea) │
│        │                      • Expected output en cada #DIAG                     │
│        │                      • Expected state en BD al final                     │
│        │                      • Reglas de auto-fix                                │
│        ▼                                                                          │
│   /implement             ──►  IMPLEMENTACION + INYECCION #DIAG                    │
│        │                      (Codigo con #DIAG insertados segun YAML)            │
│        ▼                                                                          │
│   /diagnose              ──►  LOOP DE VERIFICACION                                │
│        │                      ┌─────────────────────────────────────┐             │
│        │                      │ 1. Ejecutar proceso (API/Playwright)│             │
│        │                      │ 2. En cada #DIAG: leer consola      │             │
│        │                      │ 3. Comparar con expected del YAML   │             │
│        │                      │ 4. Si falla → auto-fix → re-ejecutar│             │
│        │                      │ 5. Verificar BD con MCP             │             │
│        │                      │ 6. Max 3 intentos o escalar         │             │
│        │                      └─────────────────────────────────────┘             │
│        ▼                                                                          │
│   /ship                  ──►  LIMPIAR + TESTS + PIPELINE + PR                     │
│                               1. Agente @diag-cleaner elimina #DIAG               │
│                               2. Ejecuta tests unitarios/E2E tradicionales        │
│                               3. Pipeline CI                                      │
│                               4. Crea PR en Azure DevOps                          │
│                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────┘
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
│   │   ├── diagnostic/                # Captura de #DIAG y queries
│   │   └── strategic-compact/         # Gestion de contexto
│   │
│   ├── agents/                        # Agentes especializados
│   │   ├── spec-architect.agent.md    # Arquitectura y DTOs
│   │   ├── synth-generator.agent.md   # Genera YAML de diagnostico
│   │   ├── diag-injector.agent.md     # Inyecta #DIAG segun YAML
│   │   ├── diag-observer.agent.md     # Observa ejecucion y compara
│   │   ├── diag-resolver.agent.md     # Auto-fix agresivo
│   │   ├── diag-cleaner.agent.md      # Limpia #DIAG antes de PR
│   │   └── ship-reviewer.agent.md     # Review final y PR
│   │
│   ├── commands/                      # Comandos unificados
│   │   ├── spec.md                    # /spec - Especificacion
│   │   ├── synth-data.md              # /synth-data - YAML diagnostico
│   │   ├── implement.md               # /implement - Codigo + #DIAG
│   │   ├── diagnose.md                # /diagnose - Loop verificacion
│   │   └── ship.md                    # /ship - Limpiar + PR
│   │
│   ├── templates/                     # Templates de codigo
│   │   ├── backend/
│   │   ├── frontend/
│   │   └── diagnostic/
│   │       └── diag-point.template.cs
│   │
│   ├── rules/                         # Reglas contextuales
│   │
│   └── mcp/                           # MCP Servers
│       └── db-diagnostic/             # Server para queries SQL
│
├── specs/                             # Especificaciones generadas
│   ├── active/                        # Symlink al feature activo
│   │   └── diagnostic-data.yaml       # YAML activo para hooks
│   └── {feature-id}/
│       ├── architecture.md            # Arquitectura y DTOs
│       ├── diagnostic-data.yaml       # YAML completo de diagnostico
│       └── SYNTHESIS.md               # Resumen para implementacion
│
├── tasks/                             # Tracking de tareas
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
      "enabled": true
    },
    {
      "name": "load-contextual-rules",
      "trigger": "PreToolUse",
      "tools": ["Read", "Edit", "Write"],
      "script": ".claude/hooks/rules/load-contextual-rules.py",
      "enabled": true
    },
    {
      "name": "auto-format",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/auto-format.py",
      "enabled": true
    },
    {
      "name": "detect-secrets",
      "trigger": "PostToolUse",
      "tools": ["Write", "Edit"],
      "script": ".claude/hooks/quality-gates/detect-secrets.py",
      "enabled": true,
      "blocksOnFailure": true
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
      "enabled": true
    },
    {
      "name": "strategic-compact",
      "trigger": "PreToolUse",
      "script": ".claude/hooks/strategic-compact/suggest-compact.py",
      "enabled": true
    },
    {
      "name": "session-end-persistence",
      "trigger": "Stop",
      "script": ".claude/hooks/session-persistence/session-end.py",
      "enabled": true
    }
  ]
}
```

### Hook: Console Observer para #DIAG

```python
#!/usr/bin/env python3
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

---

## PARTE 3: COMANDO /spec - ESPECIFICACION

### Descripcion

Genera la especificacion tecnica de una feature.

```bash
/spec "Crear campania de crowdfunding"
```

### Output

```
specs/{feature-id}/
├── architecture.md      # DTOs, endpoints, entidades, CQRS
└── SYNTHESIS.md         # Resumen y checklist
```

---

## PARTE 4: COMANDO /synth-data - YAML DE DIAGNOSTICO

### Descripcion

**Este es el corazon del paradigma.** Genera un YAML exhaustivo que define:

1. **Datos de entrada** para cada escenario
2. **Puntos #DIAG** con ubicacion exacta (archivo + linea)
3. **Codigo a inyectar** en cada punto
4. **Variables a capturar** y sus valores esperados
5. **Estado esperado en BD** al finalizar
6. **Reglas de auto-fix** para errores comunes

```bash
/synth-data feat-20260123-crear-campania
```

### Estructura del YAML

```yaml
# specs/feat-xxx/diagnostic-data.yaml
version: "1.0"
feature_id: feat-20260123-crear-campania
feature_name: "Crear campania de crowdfunding"
created: 2026-01-23T10:30:00Z
author: claude-code

# ═══════════════════════════════════════════════════════════════════════════════
# CONFIGURACION GLOBAL
# ═══════════════════════════════════════════════════════════════════════════════
config:
  api_base_url: "https://localhost:7001"
  db_connection: "Server=localhost;Database=WePlayRises;Trusted_Connection=true"
  timeout_ms: 30000
  max_retry_attempts: 3
  screenshot_on_failure: true

# ═══════════════════════════════════════════════════════════════════════════════
# DATOS SEED (poblar BD antes de tests)
# ═══════════════════════════════════════════════════════════════════════════════
seed_data:
  users:
    - id: "usr-001"
      guid: "550e8400-e29b-41d4-a716-446655440001"
      email: "artista@test.com"
      password_hash: "AQAAAAIAAYag..."
      nombre_completo: "Juan Artista"
      roles: ["Artist"]

  artistas:
    - id: "art-001"
      guid: "550e8400-e29b-41d4-a716-446655440002"
      user_id: "usr-001"
      nombre_artistico: "Los Rebeldes"
      descripcion: "Banda de rock alternativo de Madrid"

# ═══════════════════════════════════════════════════════════════════════════════
# PUNTOS DE DIAGNOSTICO BACKEND (#DIAG)
# ═══════════════════════════════════════════════════════════════════════════════
diagnostic_points:
  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-001: Entry point del command handler
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-001"
    name: "create-campaign-entry"
    description: "Punto de entrada del CreateCampaignCommandHandler"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_after: "public async Task<Result<CampaignResponse>> Handle("
      position: "first_line_of_method"
      insert_at_line: 45  # Linea exacta donde insertar

    requires_setup:
      - type: "stopwatch"
        code: "var _stopwatch = System.Diagnostics.Stopwatch.StartNew();"
        location: "start_of_method"
      - type: "using"
        code: "using System.Diagnostics;"
        location: "file_usings"

    code_to_inject: |
      _logger.LogInformation(
          "[DIAG:create-campaign-entry] timestamp={Timestamp} | userId={UserId} | titulo={Titulo} | descripcion={Descripcion} | importeObjetivo={ImporteObjetivo} | fechaInicio={FechaInicio} | fechaFin={FechaFin}",
          DateTime.UtcNow.ToString("O"),
          _currentUser.Id,
          request.Titulo,
          request.Descripcion?.Substring(0, Math.Min(50, request.Descripcion?.Length ?? 0)) ?? "",
          request.ImporteObjetivo,
          request.FechaInicio.ToString("O"),
          request.FechaFin.ToString("O"));

    variables_captured:
      - name: "Timestamp"
        type: "datetime"
        validation: "not_null"
        description: "Timestamp de entrada al handler"

      - name: "UserId"
        type: "guid"
        validation: "equals"
        expected: "{seed_data.users[usr-001].guid}"
        description: "ID del usuario autenticado"

      - name: "Titulo"
        type: "string"
        validation: "equals"
        expected: "{scenario.input.body.titulo}"
        max_length: 200

      - name: "Descripcion"
        type: "string"
        validation: "starts_with"
        expected: "{scenario.input.body.descripcion|truncate:50}"

      - name: "ImporteObjetivo"
        type: "decimal"
        validation: "equals"
        expected: "{scenario.input.body.importeObjetivo}"
        tolerance: 0.01

      - name: "FechaInicio"
        type: "datetime"
        validation: "greater_than"
        expected: "{now}"

      - name: "FechaFin"
        type: "datetime"
        validation: "greater_than"
        expected: "{scenario.input.body.fechaInicio}"

  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-002: Despues de validacion FluentValidation
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-002"
    name: "create-campaign-validated"
    description: "Resultado de FluentValidation"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_after: "var validationResult = await _validator.ValidateAsync(request, cancellationToken);"
      position: "after_statement"
      insert_at_line: 52

    code_to_inject: |
      _logger.LogInformation(
          "[DIAG:create-campaign-validated] timestamp={Timestamp} | isValid={IsValid} | errorCount={ErrorCount} | errors={Errors}",
          DateTime.UtcNow.ToString("O"),
          validationResult.IsValid,
          validationResult.Errors.Count,
          string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}:{e.ErrorMessage}")));

    variables_captured:
      - name: "IsValid"
        type: "boolean"
        validation: "equals"
        expected: true
        description: "Debe ser true para happy path"

      - name: "ErrorCount"
        type: "int"
        validation: "equals"
        expected: 0

      - name: "Errors"
        type: "string"
        validation: "is_empty"

  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-003: Antes de guardar en BD
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-003"
    name: "create-campaign-pre-save"
    description: "Estado de la entidad antes de SaveChanges"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_before: "await _context.SaveChangesAsync(cancellationToken);"
      position: "before_statement"
      insert_at_line: 68

    code_to_inject: |
      _logger.LogInformation(
          "[DIAG:create-campaign-pre-save] timestamp={Timestamp} | entityState={EntityState} | campaniaId={CampaniaId} | titulo={Titulo} | estado={Estado} | artistaId={ArtistaId}",
          DateTime.UtcNow.ToString("O"),
          _context.Entry(campania).State.ToString(),
          campania.Id,
          campania.Titulo,
          campania.Estado.ToString(),
          campania.ArtistaId);

    variables_captured:
      - name: "EntityState"
        type: "string"
        validation: "equals"
        expected: "Added"
        description: "Debe ser Added para nueva entidad"

      - name: "CampaniaId"
        type: "guid"
        validation: "is_valid_guid"
        store_as: "created_campania_id"

      - name: "Estado"
        type: "string"
        validation: "equals"
        expected: "Borrador"

      - name: "ArtistaId"
        type: "guid"
        validation: "equals"
        expected: "{seed_data.artistas[art-001].guid}"

  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-004: Despues de guardar en BD
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-004"
    name: "create-campaign-post-save"
    description: "Confirmacion de guardado exitoso"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_after: "await _context.SaveChangesAsync(cancellationToken);"
      position: "after_statement"
      insert_at_line: 70

    code_to_inject: |
      _logger.LogInformation(
          "[DIAG:create-campaign-post-save] timestamp={Timestamp} | campaniaId={CampaniaId} | durationMs={DurationMs}",
          DateTime.UtcNow.ToString("O"),
          campania.Id,
          _stopwatch.ElapsedMilliseconds);

    variables_captured:
      - name: "CampaniaId"
        type: "guid"
        validation: "equals"
        expected: "{stored.created_campania_id}"

      - name: "DurationMs"
        type: "long"
        validation: "less_than"
        expected: 1000
        description: "Guardado debe tardar <1 segundo"

  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-005: Exit exitoso
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-005"
    name: "create-campaign-exit-success"
    description: "Return exitoso del handler"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_before: "return Result<CampaignResponse>.Success(response);"
      position: "before_return"
      insert_at_line: 78

    code_to_inject: |
      _logger.LogInformation(
          "[DIAG:create-campaign-exit-success] timestamp={Timestamp} | campaniaId={CampaniaId} | responseId={ResponseId} | totalDurationMs={TotalDurationMs}",
          DateTime.UtcNow.ToString("O"),
          campania.Id,
          response.Id,
          _stopwatch.ElapsedMilliseconds);

    variables_captured:
      - name: "CampaniaId"
        type: "guid"
        validation: "equals"
        expected: "{stored.created_campania_id}"

      - name: "ResponseId"
        type: "guid"
        validation: "equals"
        expected: "{stored.created_campania_id}"

      - name: "TotalDurationMs"
        type: "long"
        validation: "less_than"
        expected: 2000

  # ─────────────────────────────────────────────────────────────────────────────
  # DIAG-006: Exit con error
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "DIAG-006"
    name: "create-campaign-exit-error"
    description: "Return con error"

    location:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandHandler.cs"
      class: "CreateCampaignCommandHandler"
      method: "Handle"
      line_before: "return Result<CampaignResponse>.Failure("
      position: "before_return"
      match_all: true

    code_to_inject: |
      _logger.LogWarning(
          "[DIAG:create-campaign-exit-error] timestamp={Timestamp} | errorMessage={ErrorMessage} | totalDurationMs={TotalDurationMs}",
          DateTime.UtcNow.ToString("O"),
          errorMessage,
          _stopwatch.ElapsedMilliseconds);

    variables_captured:
      - name: "ErrorMessage"
        type: "string"
        validation: "not_null"

# ═══════════════════════════════════════════════════════════════════════════════
# PUNTOS DE DIAGNOSTICO FRONTEND (#DIAG)
# ═══════════════════════════════════════════════════════════════════════════════
diagnostic_points_frontend:
  - id: "DIAG-UI-001"
    name: "campaign-form-rendered"
    description: "Form de campania renderizado"

    location:
      file: "src/web/src/features/campaigns/components/CampaignForm.tsx"
      component: "CampaignForm"
      hook: "useEffect"
      position: "after_render"
      insert_at_line: 25

    code_to_inject: |
      useEffect(() => {
        console.log('[DIAG:campaign-form-rendered]', JSON.stringify({
          timestamp: new Date().toISOString(),
          formFields: Object.keys(form.getValues()),
          isValid: form.formState.isValid,
          isDirty: form.formState.isDirty,
        }));
      }, []);

    playwright_verification:
      - action: "wait_for_selector"
        selector: "[data-testid='campaign-form']"
        timeout: 5000
      - action: "screenshot"
        name: "form-initial-state"
      - action: "check_console_log"
        pattern: "[DIAG:campaign-form-rendered]"
        expected_fields:
          formFields: ["titulo", "descripcion", "importeObjetivo", "fechaInicio", "fechaFin"]
          isValid: false
          isDirty: false

  - id: "DIAG-UI-002"
    name: "campaign-form-submit"
    description: "Envio del formulario"

    location:
      file: "src/web/src/features/campaigns/components/CampaignForm.tsx"
      component: "CampaignForm"
      function: "handleSubmit"
      position: "start_of_function"
      insert_at_line: 42

    code_to_inject: |
      console.log('[DIAG:campaign-form-submit]', JSON.stringify({
        timestamp: new Date().toISOString(),
        formData: data,
        isSubmitting: true,
      }));

    playwright_verification:
      - action: "fill"
        selector: "input[name='titulo']"
        value: "{scenario.input.body.titulo}"
      - action: "fill"
        selector: "textarea[name='descripcion']"
        value: "{scenario.input.body.descripcion}"
      - action: "fill"
        selector: "input[name='importeObjetivo']"
        value: "{scenario.input.body.importeObjetivo}"
      - action: "click"
        selector: "button[type='submit']"
      - action: "screenshot"
        name: "form-submitting"
      - action: "check_console_log"
        pattern: "[DIAG:campaign-form-submit]"
        expected_fields:
          formData.titulo: "{scenario.input.body.titulo}"
          isSubmitting: true

  - id: "DIAG-UI-003"
    name: "campaign-form-success"
    description: "Respuesta exitosa mostrada"

    location:
      file: "src/web/src/features/campaigns/components/CampaignForm.tsx"
      component: "CampaignForm"
      function: "onSuccess"
      position: "start_of_function"
      insert_at_line: 58

    code_to_inject: |
      console.log('[DIAG:campaign-form-success]', JSON.stringify({
        timestamp: new Date().toISOString(),
        responseId: response.id,
        redirectTo: `/dashboard/campanias/${response.id}`,
      }));

    playwright_verification:
      - action: "wait_for_navigation"
        url_pattern: "/dashboard/campanias/{guid}"
        timeout: 10000
      - action: "screenshot"
        name: "success-redirect"
      - action: "verify_element"
        selector: "[data-testid='campania-titulo']"
        expected_text: "{scenario.input.body.titulo}"

# ═══════════════════════════════════════════════════════════════════════════════
# ESCENARIOS DE PRUEBA
# ═══════════════════════════════════════════════════════════════════════════════
scenarios:
  # ─────────────────────────────────────────────────────────────────────────────
  # Escenario 1: Happy path API
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "scenario-001"
    name: "crear_campania_valida_api"
    description: "Crear campania con datos validos via API"
    type: "api_happy_path"

    preconditions:
      - "Usuario autenticado como artista (usr-001)"
      - "Artista tiene perfil (art-001)"

    auth:
      user_id: "usr-001"
      token: "{generate_jwt(usr-001)}"

    input:
      method: "POST"
      endpoint: "/api/campanias"
      headers:
        Content-Type: "application/json"
        Authorization: "Bearer {auth.token}"
      body:
        titulo: "Mi Primer Album - Financiacion Colectiva"
        descripcion: "Estamos buscando financiacion para grabar nuestro primer album de estudio profesional. El album contendra 12 canciones originales que hemos compuesto durante los ultimos 2 anos de trabajo."
        importeObjetivo: 5000.00
        fechaInicio: "{now|add_days:7|format:yyyy-MM-ddTHH:mm:ssZ}"
        fechaFin: "{now|add_days:67|format:yyyy-MM-ddTHH:mm:ssZ}"

    diagnostic_sequence:
      - point: "DIAG-001"
        order: 1
        description: "Verificar entrada al handler"
      - point: "DIAG-002"
        order: 2
        description: "Verificar validacion exitosa"
      - point: "DIAG-003"
        order: 3
        description: "Verificar estado pre-save"
      - point: "DIAG-004"
        order: 4
        description: "Verificar guardado exitoso"
      - point: "DIAG-005"
        order: 5
        description: "Verificar exit exitoso"

    expected_response:
      status_code: 201
      headers:
        Content-Type: "application/json"
      body:
        id: "{is_valid_guid}"
        titulo: "Mi Primer Album - Financiacion Colectiva"
        estado: "Borrador"
        importeObjetivo: 5000.00
        importeRecaudado: 0.00

    expected_db_state:
      - table: "Campanias"
        query: "SELECT * FROM Campanias WHERE Id = '{stored.created_campania_id}'"
        assertions:
          - column: "Id"
            value: "{stored.created_campania_id}"
          - column: "Titulo"
            value: "Mi Primer Album - Financiacion Colectiva"
          - column: "Descripcion"
            validation: "starts_with"
            value: "Estamos buscando financiacion"
          - column: "ImporteObjetivo"
            value: 5000.00
            tolerance: 0.01
          - column: "ImporteRecaudado"
            value: 0.00
          - column: "Estado"
            value: 1  # Enum Borrador = 1
          - column: "ArtistaId"
            value: "{seed_data.artistas[art-001].guid}"
          - column: "FechaCreacion"
            validation: "within_last_seconds"
            seconds: 60

  # ─────────────────────────────────────────────────────────────────────────────
  # Escenario 2: Validacion error - titulo vacio
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "scenario-002"
    name: "crear_campania_titulo_vacio"
    description: "Debe fallar validacion con titulo vacio"
    type: "api_validation_error"

    auth:
      user_id: "usr-001"

    input:
      method: "POST"
      endpoint: "/api/campanias"
      body:
        titulo: ""
        descripcion: "Descripcion valida"
        importeObjetivo: 5000.00
        fechaInicio: "{now|add_days:7}"
        fechaFin: "{now|add_days:67}"

    diagnostic_sequence:
      - point: "DIAG-001"
        order: 1
      - point: "DIAG-002"
        order: 2
        expected_override:
          IsValid: false
          ErrorCount: 1
          Errors: "Titulo:El titulo es obligatorio"
      - point: "DIAG-006"
        order: 3

    expected_response:
      status_code: 400
      body:
        errors:
          - field: "Titulo"
            message: "El titulo es obligatorio"

    expected_db_state:
      - table: "Campanias"
        query: "SELECT COUNT(*) as Count FROM Campanias WHERE Titulo = ''"
        assertions:
          - column: "Count"
            value: 0

  # ─────────────────────────────────────────────────────────────────────────────
  # Escenario 3: Flujo UI completo
  # ─────────────────────────────────────────────────────────────────────────────
  - id: "scenario-003"
    name: "ui_crear_campania_completo"
    description: "Flujo E2E desde UI con Playwright"
    type: "e2e_ui"

    auth:
      user_id: "usr-001"
      login_via_ui: true

    steps:
      - step: 1
        action: "navigate"
        url: "http://localhost:3001/login"
        description: "Ir a login"

      - step: 2
        action: "fill_form"
        fields:
          email: "artista@test.com"
          password: "Test123!"
        submit: true
        wait_for: "network_idle"
        description: "Login"

      - step: 3
        action: "navigate"
        url: "http://localhost:3001/dashboard/campanias/nueva"
        diagnostic_point: "DIAG-UI-001"
        screenshot: "step-03-form-loaded"
        description: "Abrir formulario"

      - step: 4
        action: "fill_form"
        fields:
          titulo: "Mi Primer Album - Financiacion Colectiva"
          descripcion: "Descripcion del proyecto musical..."
          importeObjetivo: "5000"
          fechaInicio: "{now|add_days:7|format:yyyy-MM-dd}"
          fechaFin: "{now|add_days:67|format:yyyy-MM-dd}"
        screenshot: "step-04-form-filled"
        description: "Rellenar formulario"

      - step: 5
        action: "click"
        selector: "button[type='submit']"
        diagnostic_point: "DIAG-UI-002"
        wait_for: "network_idle"
        screenshot: "step-05-submitting"
        description: "Enviar formulario"

      - step: 6
        action: "wait_for_navigation"
        url_pattern: "/dashboard/campanias/{guid}"
        timeout: 10000
        diagnostic_point: "DIAG-UI-003"
        screenshot: "step-06-success"
        description: "Verificar redireccion"

      - step: 7
        action: "verify_element"
        selector: "[data-testid='campania-titulo']"
        expected_text: "Mi Primer Album - Financiacion Colectiva"
        screenshot: "step-07-verification"
        description: "Verificar titulo en pagina"

    expected_console_sequence:
      - "[DIAG:campaign-form-rendered]"
      - "[DIAG:campaign-form-submit]"
      - "[DIAG:campaign-form-success]"

    expected_api_calls:
      - method: "POST"
        url: "/api/campanias"
        status: 201

# ═══════════════════════════════════════════════════════════════════════════════
# REGLAS DE AUTO-FIX
# ═══════════════════════════════════════════════════════════════════════════════
auto_fix_rules:
  - id: "fix-001"
    error_pattern: "IsValid=False.*Titulo.*obligatorio"
    diagnosis: "Falta validacion NotEmpty para Titulo en Validator"
    severity: "error"

    fix:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandValidator.cs"
      action: "add_rule"
      location:
        method: "constructor"
        after_line: "public CreateCampaignCommandValidator()"
      code: |
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El titulo es obligatorio")
            .MaximumLength(200).WithMessage("El titulo no puede exceder 200 caracteres");

  - id: "fix-002"
    error_pattern: "EntityState=Detached"
    diagnosis: "Entidad no agregada al DbContext"
    severity: "error"

    fix:
      file: "{current_handler_file}"
      action: "add_statement"
      location:
        before: "await _context.SaveChangesAsync"
      code: |
        await _context.Campanias.AddAsync(campania, cancellationToken);

  - id: "fix-003"
    error_pattern: "DurationMs.*greater_than.*1000"
    diagnosis: "Query lenta - considerar indice"
    severity: "warning"

    fix:
      action: "suggest"
      message: |
        SUGERENCIA: Agregar indice para mejorar rendimiento
        SQL: CREATE INDEX IX_Campanias_ArtistaId ON Campanias(ArtistaId);

  - id: "fix-004"
    error_pattern: "FechaInicio.*less_than.*now"
    diagnosis: "Falta validacion de fecha futura"
    severity: "error"

    fix:
      file: "src/api/Modules/Crowdfunding/Application/Commands/CreateCampaignCommandValidator.cs"
      action: "add_rule"
      code: |
        RuleFor(x => x.FechaInicio)
            .GreaterThan(DateTime.UtcNow).WithMessage("La fecha de inicio debe ser futura");

  - id: "fix-005"
    error_pattern: "form-rendered.*formFields.*missing"
    diagnosis: "Campo faltante en formulario React"
    severity: "error"

    fix:
      file: "src/web/src/features/campaigns/components/CampaignForm.tsx"
      action: "add_field"
      template: "form-field"
```

---

## PARTE 5: COMANDO /implement - IMPLEMENTACION + #DIAG

### Descripcion

Implementa el codigo e inyecta los puntos #DIAG segun las ubicaciones exactas del YAML.

```bash
/implement feat-20260123-crear-campania
```

### Proceso

1. **Setup worktree**
   ```bash
   git worktree add .worktrees/{feature-id} -b feature/{feature-id}
   ln -s specs/{feature-id}/diagnostic-data.yaml specs/active/diagnostic-data.yaml
   ```

2. **Implementar codigo** usando templates

3. **Inyectar #DIAG** - El agente @diag-injector lee el YAML y para cada punto:
   - Localiza el archivo y linea exacta
   - Inserta `requires_setup` si aplica (usings, stopwatch)
   - Inserta `code_to_inject` en la posicion indicada

### Output

```
✅ IMPLEMENTACION: feat-20260123-crear-campania

Backend:
   ✅ CreateCampaignCommand + Handler + Validator
   ✅ CampaniaController
   ✅ 6 puntos #DIAG backend inyectados

Frontend:
   ✅ CampaignForm component
   ✅ 3 puntos #DIAG frontend inyectados

⏭️  Siguiente: /diagnose feat-20260123-crear-campania
```

---

## PARTE 6: COMANDO /diagnose - LOOP DE VERIFICACION

### Descripcion

Ejecuta los escenarios, observa cada #DIAG, compara con expected, y auto-corrige.

```bash
/diagnose feat-20260123-crear-campania
```

### Workflow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         LOOP DE DIAGNOSTICO                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Para cada ESCENARIO:                                                      │
│                                                                             │
│   1. PREPARAR                                                               │
│      └─► Poblar BD con seed_data                                            │
│      └─► Generar JWT si auth requerido                                      │
│                                                                             │
│   2. EJECUTAR                                                               │
│      ├─► API: curl/httpie segun input                                       │
│      └─► UI: Playwright segun steps                                         │
│                                                                             │
│   3. OBSERVAR (en cada #DIAG)                                               │
│      ├─► Hook captura [DIAG:xxx] de consola                                 │
│      ├─► Parsea variables                                                   │
│      └─► Compara con expected del YAML                                      │
│                                                                             │
│   4. SI HAY DISCREPANCIA                                                    │
│      ├─► Buscar en auto_fix_rules                                           │
│      ├─► Aplicar fix automaticamente                                        │
│      ├─► Re-ejecutar escenario                                              │
│      └─► Max 3 intentos                                                     │
│                                                                             │
│   5. VERIFICAR BD                                                           │
│      ├─► Ejecutar queries de expected_db_state                              │
│      └─► Comparar assertions                                                │
│                                                                             │
│   6. SI 3 INTENTOS FALLAN                                                   │
│      └─► Escalar a humano con reporte                                       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Output

```
═══════════════════════════════════════════════════════════════════════════════
DIAGNOSTICO: feat-20260123-crear-campania
═══════════════════════════════════════════════════════════════════════════════

ESCENARIO: crear_campania_valida_api (api_happy_path)
───────────────────────────────────────────────────────────────────────────────

PUNTOS #DIAG:
  ✅ DIAG-001 create-campaign-entry
     • UserId = 550e8400-e29b-41d4-a716-446655440001 ✓
     • Titulo = Mi Primer Album - Financiacion Colectiva ✓
     • ImporteObjetivo = 5000.00 ✓

  ✅ DIAG-002 create-campaign-validated
     • IsValid = true ✓
     • ErrorCount = 0 ✓

  ✅ DIAG-003 create-campaign-pre-save
     • EntityState = Added ✓
     • Estado = Borrador ✓

  ✅ DIAG-004 create-campaign-post-save
     • CampaniaId = 7b2f4e8a-1234-5678-9abc-def012345678 ✓
     • DurationMs = 45 (<1000) ✓

  ✅ DIAG-005 create-campaign-exit-success
     • ResponseId = 7b2f4e8a-1234-5678-9abc-def012345678 ✓
     • TotalDurationMs = 78 (<2000) ✓

RESPONSE:
  ✅ status_code = 201 ✓
  ✅ body.titulo = Mi Primer Album - Financiacion Colectiva ✓
  ✅ body.estado = Borrador ✓

BASE DE DATOS:
  ✅ Campanias.Titulo = "Mi Primer Album - Financiacion Colectiva" ✓
  ✅ Campanias.Estado = 1 (Borrador) ✓
  ✅ Campanias.ImporteObjetivo = 5000.00 ✓
  ✅ Campanias.ArtistaId = {correct} ✓

───────────────────────────────────────────────────────────────────────────────

ESCENARIO: crear_campania_titulo_vacio (api_validation_error)
───────────────────────────────────────────────────────────────────────────────

PUNTOS #DIAG:
  ✅ DIAG-001 create-campaign-entry
     • Titulo = "" ✓

  ❌ DIAG-002 create-campaign-validated (INTENTO 1)
     • IsValid = true (expected: false)

     🔧 AUTO-FIX APLICADO (fix-001):
        Archivo: CreateCampaignCommandValidator.cs
        Accion: Agregar RuleFor(x => x.Titulo).NotEmpty()

     🔄 RE-EJECUTANDO ESCENARIO...

  ✅ DIAG-002 create-campaign-validated (INTENTO 2)
     • IsValid = false ✓
     • ErrorCount = 1 ✓
     • Errors = "Titulo:El titulo es obligatorio" ✓

  ✅ DIAG-006 create-campaign-exit-error
     • ErrorMessage = "Validation failed" ✓

═══════════════════════════════════════════════════════════════════════════════
RESUMEN
═══════════════════════════════════════════════════════════════════════════════

Escenarios ejecutados: 3/3
Puntos #DIAG verificados: 18/18
Auto-fixes aplicados: 1
Verificaciones BD: 12/12

✅ TODOS LOS DIAGNOSTICOS PASARON

⏭️  Siguiente: /ship feat-20260123-crear-campania
```

---

## PARTE 7: COMANDO /ship - LIMPIAR + TESTS + PR

### Descripcion

1. Limpia todos los #DIAG del codigo
2. Ejecuta tests tradicionales (unit + E2E)
3. Pipeline CI
4. Crea PR

```bash
/ship feat-20260123-crear-campania
```

### Proceso de Limpieza

El agente @diag-cleaner:

```python
# Encuentra y elimina todo codigo #DIAG
for file in find_files_with_diag():
    content = file.read_text()

    # Eliminar bloques completos de #DIAG
    content = remove_diag_blocks(content)

    # Eliminar requires_setup (stopwatch, usings de diagnostico)
    content = remove_setup_code(content)

    file.write_text(content)
```

### Output

```
✅ SHIP: feat-20260123-crear-campania

1. LIMPIEZA #DIAG
   ✅ 6 puntos #DIAG eliminados de backend
   ✅ 3 puntos #DIAG eliminados de frontend
   ✅ Setup code (stopwatch) eliminado

2. TESTS TRADICIONALES
   ✅ Unit Tests Backend: 15/15
   ✅ Unit Tests Frontend: 12/12
   ✅ E2E Tests: 3/3

3. VALIDACIONES
   ✅ Build Release
   ✅ Lint
   ✅ Type Check

4. PULL REQUEST
   🔗 https://dev.azure.com/weplay/WePlayRises/_git/WePlayRises/pullrequest/42

📋 YAML guardado para re-diagnostico futuro:
   specs/feat-20260123-crear-campania/diagnostic-data.yaml
```

---

## PARTE 8: INTEGRACION CON UXPILOT

UxPilot se integra en /implement con flag `--uxpilot`:

```bash
/implement feat-xxx --uxpilot
```

Claude Code:
1. Llama a UxPilot API con descripcion de UI
2. Recibe mockup + codigo base
3. Adapta a stack (Tailwind + shadcn)
4. Inyecta #DIAG en componentes
5. Continua con diagnostico normal

---

## PARTE 9: MCP DB DIAGNOSTIC SERVER

Herramientas usadas en /diagnose para verificar BD:

| Tool | Uso |
|------|-----|
| `db_connect` | Conectar antes de escenarios |
| `db_select` | Ejecutar queries de expected_db_state |
| `db_recent_queries` | Ver queries EF Core ejecutadas |
| `db_slow_queries` | Detectar N+1, queries lentas |

---

## PARTE 10: RESUMEN DE COMANDOS

| Comando | Descripcion | Output Principal |
|---------|-------------|------------------|
| `/spec` | Especificacion tecnica | architecture.md |
| `/synth-data` | YAML de diagnostico completo | diagnostic-data.yaml |
| `/implement` | Codigo + #DIAG inyectados | Worktree con codigo |
| `/diagnose` | Loop: ejecutar → comparar → auto-fix | Reporte de verificacion |
| `/ship` | Limpiar #DIAG + tests + PR | PR en Azure DevOps |

---

## PARTE 11: FLUJO COMPLETO

```bash
# 1. Especificar
/spec "Crear campania de crowdfunding"

# 2. Generar YAML de diagnostico
/synth-data feat-20260123-crear-campania

# 3. Implementar con #DIAG
/implement feat-20260123-crear-campania

# 4. Loop de diagnostico (auto-fix hasta que pase)
/diagnose feat-20260123-crear-campania

# 5. Limpiar y crear PR
/ship feat-20260123-crear-campania
```

---

**Documento creado**: 2026-01-23
**Version**: 1.0
**Autor**: Ivan / Claude Code (Opus 4.5)
