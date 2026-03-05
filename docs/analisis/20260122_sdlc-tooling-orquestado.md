# SDLC Tooling Orquestado - WePlay Rises

- **Fecha**: 2026-01-22
- **Proyecto**: WePlay Rises - MVP Crowdfunding Musical
- **Stack**: .NET 8 API + React (Vite) + Next.js 14
- **Objetivo**: Orquestar el ciclo de desarrollo para máxima eficiencia en 30h

---

## 1. Visión General

### 1.1 Arquitectura del Tooling

```
┌──────────────────────────────────────────────────────────────────────┐
│                         SDLC TOOLING ORQUESTADO                      │
├──────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐   │
│  │   COMANDOS      │    │    AGENTES      │    │   DIAGNÓSTICO   │   │
│  │   (/slash)      │    │ (Especializados)│    │   (/diag_*)     │   │
│  └────────┬────────┘    └────────┬────────┘    └────────┬────────┘   │
│           │                      │                      │            │
│           └──────────────────────┴──────────────────────┘            │
│                                  │                                   │
│                                  ▼                                   │
│                    ┌─────────────────────────┐                       │
│                    │   TESTING HÍBRIDO       │                       │
│                    │   Unit → Integration    │                       │
│                    │   → E2E (Playwright)    │                       │
│                    └────────────┬────────────┘                       │
│                                 │                                    │
│           ┌─────────────────────┼─────────────────────┐              │
│           ▼                     ▼                     ▼              │
│    ┌─────────────┐       ┌─────────────┐       ┌─────────────┐       │
│    │ MCP DB      │       │ Correlation │       │ Reports     │       │
│    │ Server      │       │ Engine      │       │ & Logs      │       │
│    └─────────────┘       └─────────────┘       └─────────────┘       │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

### 1.2 Principios de Diseño

| Principio | Descripción |
|-----------|-------------|
| **Orquestación Autónoma** | Claude Code controla todo el flujo |
| **Fail Fast** | Unit tests primero; si fallan, no continuar |
| **Determinismo** | Scripts para operaciones repetibles |
| **Trazabilidad** | Logs, screenshots y métricas correlacionados |
| **Simplicidad** | MVP en 30h, evitar over-engineering |

---

## 2. Comandos Slash

### 2.1 Comandos de Scaffolding

| Comando | Descripción | Ubicación |
|---------|-------------|-----------|
| `/new-entity` | Scaffolding entidad completa (Domain → DTO → Service → Controller) | `.claude/commands/scaffolding/` |
| `/new-endpoint` | Scaffolding Command + Handler + Validator | `.claude/commands/scaffolding/` |
| `/new-page` | Scaffolding página React/Next.js con hook y service | `.claude/commands/scaffolding/` |
| `/new-feature` | Scaffolding feature completa (backend + frontend) | `.claude/commands/scaffolding/` |

### 2.2 Comandos de Desarrollo

| Comando | Descripción | Ubicación |
|---------|-------------|-----------|
| `/build-check` | Verificar que API + Web + Admin compilan | `.claude/commands/dev/` |
| `/run-tests` | Ejecutar tests multinivel (unit → integration → e2e) | `.claude/commands/dev/` |
| `/seed-data` | Insertar datos de prueba en DB | `.claude/commands/dev/` |
| `/db-status` | Verificar estado de la base de datos | `.claude/commands/dev/` |

### 2.3 Comandos de Diagnóstico

| Comando | Descripción | Ubicación |
|---------|-------------|-----------|
| `/diag_inject` | Inyectar puntos de diagnóstico en código | `.claude/commands/diagnostics/` |
| `/diag_run` | Ejecutar flujo con captura de diagnóstico | `.claude/commands/diagnostics/` |
| `/diag_clean` | Limpiar código de diagnóstico | `.claude/commands/diagnostics/` |
| `/diag_analyze` | Analizar sesión de diagnóstico | `.claude/commands/diagnostics/` |

### 2.4 Comandos de Workflow

| Comando | Descripción | Ubicación |
|---------|-------------|-----------|
| `/done WPR-XXX` | Marcar tarea completada | `.claude/commands/workflow/` |
| `/context-mode` | Cambiar modo de trabajo (dev/review/research) | `.claude/commands/workflow/` |
| `/health-check` | Validar sistema completo | `.claude/commands/diagnostics/` |

### 2.5 Comandos de Deploy

| Comando | Descripción | Ubicación |
|---------|-------------|-----------|
| `/deploy-preview` | Build + deploy a Azure staging | `.claude/commands/deploy/` |
| `/deploy-prod` | Deploy a producción (con validaciones) | `.claude/commands/deploy/` |

---

## 3. Agentes Especializados

### 3.1 Agentes de Backend

#### `backend-cqrs`
```yaml
name: backend-cqrs
description: "Genera código CQRS siguiendo patrones del proyecto"
triggers:
  - "crear endpoint"
  - "nuevo command"
  - "nueva query"
capabilities:
  - Genera Command + Handler en mismo archivo
  - Crea Validator con Message + ErrorCode
  - Crea DTO y Profile de AutoMapper
  - Añade endpoint en Controller
  - Genera tests unitarios
rules:
  - Siempre retorna ServiceResponse<T>
  - Handler nunca inyecta DbContext
  - Service retorna entidad, no DTO
```

#### `backend-validator`
```yaml
name: backend-validator
description: "Verifica que código backend sigue reglas CQRS"
triggers:
  - Antes de commit
  - Al revisar PR
checks:
  - Handler + Command en mismo archivo
  - ServiceResponse en todos los handlers
  - Validators con Message + ErrorCode
  - No DbContext en Handlers
  - Logging en catch blocks
```

### 3.2 Agentes de Frontend

#### `frontend-feature`
```yaml
name: frontend-feature
description: "Genera feature React completa"
triggers:
  - "crear página"
  - "nuevo componente"
  - "nueva feature"
capabilities:
  - Crea service con API client
  - Crea custom hook con TanStack Query
  - Crea componentes con shadcn/ui
  - Crea schema Zod para formularios
  - Añade tipos en shared
output_structure:
  web:
    - features/{name}/presentation/
    - features/{name}/application/
    - features/{name}/infrastructure/
  admin:
    - app/(dashboard)/{name}/
    - components/{name}/
    - hooks/use{Name}.ts
```

#### `frontend-validator`
```yaml
name: frontend-validator
description: "Verifica código frontend"
checks:
  - No usar any en TypeScript
  - Usar shadcn/ui, no HTML nativo
  - TanStack Query para server state
  - React Hook Form + Zod para forms
  - Tipos compartidos en src/shared
```

### 3.3 Agentes de Testing

#### `test-generator`
```yaml
name: test-generator
description: "Genera tests para código existente"
triggers:
  - Después de implementar feature
  - Al pedir tests
capabilities:
  - Tests unitarios para Handlers (.NET)
  - Tests unitarios para Services (.NET)
  - Tests de componentes React (Vitest)
  - Tests E2E con Playwright
patterns:
  - Arrange-Act-Assert
  - Mocking con Moq/.NET y vi/Vitest
  - Nombres descriptivos: Method_Scenario_Expected
```

#### `test-runner`
```yaml
name: test-runner
description: "Ejecuta y analiza tests"
capabilities:
  - Ejecuta tests por nivel
  - Analiza fallos
  - Propone fixes
  - Genera reporte
```

### 3.4 Agentes de Diagnóstico

#### `diagnostic-planner`
```yaml
name: diagnostic-planner
description: "Planifica puntos de diagnóstico"
triggers:
  - Al investigar bug
  - Al analizar flujo
output:
  - diagnostic-plan.md
  - {task-id}.diag.yaml
capabilities:
  - Identifica puntos críticos
  - Define qué capturar (logs, queries, screenshots)
  - Genera YAML de diagnóstico
```

#### `diagnostic-analyzer`
```yaml
name: diagnostic-analyzer
description: "Analiza sesiones de diagnóstico"
input:
  - Logs de API
  - Logs de frontend
  - Queries de DB
  - Screenshots de Playwright
output:
  - Timeline correlacionado
  - Identificación de bottlenecks
  - Propuestas de fix
```

---

## 4. Sistema de Testing Híbrido

### 4.1 Pirámide de Tests

```
                           ▲
                          /│\         E2E (Playwright)
                         / │ \        - Flujos completos
                        /  │  \       - Browser real
                       /   │   \      - Screenshots
                      ────────────
                     /      │      \    INTEGRATION
                    /       │       \   - API real o WireMock
                   /        │        \  - Database tests
                  /         │         \ - HTTP real
                 ──────────────────────
                /           │           \  UNIT
               /            │            \ - Handlers
              /             │             \- Services
             /              │              \- Components
            ────────────────────────────────
```

### 4.2 Tests Backend (.NET)

#### Estructura
```
src/api/
├── Tests/
│   ├── Unit/
│   │   ├── Handlers/
│   │   │   ├── CreateCampaniaHandlerTests.cs
│   │   │   └── GetCampaniaByIdHandlerTests.cs
│   │   ├── Services/
│   │   │   └── CampaniaServiceTests.cs
│   │   └── Validators/
│   │       └── CreateCampaniaValidatorTests.cs
│   │
│   ├── Integration/
│   │   ├── Api/
│   │   │   ├── CampaniasControllerTests.cs
│   │   │   └── AuthControllerTests.cs
│   │   └── Fixtures/
│   │       └── ApiTestFixture.cs
│   │
│   └── TestData/
│       ├── CampaniaTestData.cs
│       └── ArtistaTestData.cs
```

#### Patrón Base para Tests de Handler
```csharp
public class CreateCampaniaHandlerTests
{
    private readonly Mock<ICampaniaService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateCampaniaCommand>> _validatorMock;
    private readonly CreateCampaniaCommandHandler _sut;

    public CreateCampaniaHandlerTests()
    {
        _serviceMock = new Mock<ICampaniaService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateCampaniaCommand>>();
        _sut = new CreateCampaniaCommandHandler(
            _serviceMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            Mock.Of<ILogger<CreateCampaniaCommandHandler>>()
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithId()
    {
        // Arrange
        var command = CampaniaTestData.ValidCreateCommand;
        var expectedId = Guid.NewGuid();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Campania>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedId);
    }
}
```

### 4.3 Tests Frontend (Vitest)

#### Estructura
```
src/web/
├── __tests__/
│   ├── components/
│   │   └── CampaniaCard.test.tsx
│   ├── hooks/
│   │   └── useCampanias.test.ts
│   └── services/
│       └── campania.service.test.ts

src/admin/
├── __tests__/
│   ├── components/
│   └── hooks/
```

#### Patrón de Test de Hook
```typescript
// __tests__/hooks/useCampanias.test.ts
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useCampanias } from '@/hooks/useCampanias';
import { campaniaService } from '@/services/campania.service';
import { vi, describe, it, expect } from 'vitest';

vi.mock('@/services/campania.service');

describe('useCampanias', () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } }
  });

  const wrapper = ({ children }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );

  it('returns campanias on success', async () => {
    const mockCampanias = [
      { id: '1', titulo: 'Test Campaign' }
    ];

    vi.mocked(campaniaService.getAll).mockResolvedValue(mockCampanias);

    const { result } = renderHook(() => useCampanias(), { wrapper });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data).toEqual(mockCampanias);
  });
});
```

### 4.4 Tests E2E (Playwright)

#### Estructura
```
e2e/
├── tests/
│   ├── auth/
│   │   ├── login.spec.ts
│   │   └── register.spec.ts
│   ├── campanias/
│   │   ├── explore.spec.ts
│   │   ├── detail.spec.ts
│   │   └── create.spec.ts
│   └── backings/
│       ├── create-backing.spec.ts
│       └── my-backings.spec.ts
│
├── fixtures/
│   ├── auth.fixture.ts
│   └── db.fixture.ts
│
├── pages/
│   ├── LoginPage.ts
│   ├── HomePage.ts
│   └── CampaniaDetailPage.ts
│
└── playwright.config.ts
```

#### Page Object Pattern
```typescript
// e2e/pages/LoginPage.ts
import { Page, Locator } from '@playwright/test';

export class LoginPage {
  readonly page: Page;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly loginButton: Locator;
  readonly errorMessage: Locator;

  constructor(page: Page) {
    this.page = page;
    this.emailInput = page.getByLabel('Email');
    this.passwordInput = page.getByLabel('Contraseña');
    this.loginButton = page.getByRole('button', { name: 'Iniciar Sesión' });
    this.errorMessage = page.getByRole('alert');
  }

  async goto() {
    await this.page.goto('/login');
  }

  async login(email: string, password: string) {
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.loginButton.click();
  }

  async expectError(message: string) {
    await expect(this.errorMessage).toContainText(message);
  }
}
```

#### Flujo E2E Completo
```typescript
// e2e/tests/campanias/create.spec.ts
import { test, expect } from '@playwright/test';
import { LoginPage } from '../../pages/LoginPage';
import { DashboardPage } from '../../pages/DashboardPage';
import { CampaniaFormPage } from '../../pages/CampaniaFormPage';

test.describe('Crear Campaña', () => {
  test.beforeEach(async ({ page }) => {
    // Login como artista
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.login('artista@test.com', 'Test1234!');
    await expect(page).toHaveURL('/dashboard');
  });

  test('artista puede crear campaña válida', async ({ page }) => {
    const dashboard = new DashboardPage(page);
    await dashboard.clickNewCampaign();

    const form = new CampaniaFormPage(page);
    await form.fillBasicInfo({
      titulo: 'Mi Nuevo Álbum',
      descripcion: 'Descripción del proyecto',
      importeObjetivo: 5000,
      fechaFin: '2026-03-01'
    });
    await form.submit();

    // Verificar redirect a mis campañas
    await expect(page).toHaveURL('/dashboard/campanias');
    await expect(page.getByText('Mi Nuevo Álbum')).toBeVisible();
  });

  test('muestra error con datos inválidos', async ({ page }) => {
    const dashboard = new DashboardPage(page);
    await dashboard.clickNewCampaign();

    const form = new CampaniaFormPage(page);
    await form.fillBasicInfo({
      titulo: '', // Vacío
      importeObjetivo: 50 // Menos del mínimo
    });
    await form.submit();

    await expect(form.errorMessage).toContainText('título es obligatorio');
  });
});
```

---

## 5. Sistema de Diagnóstico

### 5.1 Puntos de Diagnóstico

#### Formato de Log
```
[DIAG:{point-id}] {timestamp} | {data}
```

#### Puntos en Backend (.NET)
```csharp
// En Handler
_logger.LogInformation("[DIAG:create-campania-entry] {Timestamp} | ArtistaId={ArtistaId}, Titulo={Titulo}",
    DateTime.UtcNow.ToString("O"), request.ArtistaId, request.Titulo);

// Al salir
_logger.LogInformation("[DIAG:create-campania-exit] {Timestamp} | Success={Success}, Id={Id}",
    DateTime.UtcNow.ToString("O"), response.IsSuccess, response.Data);
```

#### Puntos en Frontend (React)
```typescript
// En service
console.log(`[DIAG:api-call-entry] ${new Date().toISOString()} | endpoint=${endpoint}`);

// En hook
console.log(`[DIAG:query-complete] ${new Date().toISOString()} | success=${isSuccess}, count=${data?.length}`);
```

### 5.2 Comando /diag_inject

```yaml
# .claude/commands/diagnostics/diag_inject.md
name: diag_inject
description: "Inyecta puntos de diagnóstico según plan"
usage: "/diag_inject <plan-file.yaml>"

input:
  plan_file: "Archivo YAML con puntos a inyectar"

actions:
  1. Leer plan de diagnóstico
  2. Para cada punto:
     - Localizar archivo y línea
     - Insertar log con formato [DIAG:point-id]
  3. Guardar backup de archivos modificados
  4. Reportar puntos inyectados

output:
  - Lista de archivos modificados
  - Backup en .claude/diagnostics/backups/
```

### 5.3 Comando /diag_run

```yaml
# .claude/commands/diagnostics/diag_run.md
name: diag_run
description: "Ejecuta flujo con captura de diagnóstico"
usage: "/diag_run <flow-name> [--e2e] [--api-logs] [--db-queries]"

actions:
  1. Iniciar captura de logs API (tail -f o similar)
  2. Iniciar MCP DB Server (si --db-queries)
  3. Ejecutar test E2E con Playwright (si --e2e)
  4. Capturar screenshots en cada paso
  5. Detener capturas
  6. Guardar sesión en .claude/diagnostics/sessions/

output:
  session_dir: ".claude/diagnostics/sessions/{timestamp}_{flow}/"
  contents:
    - api.log
    - frontend.log
    - db_queries.json
    - screenshots/
    - playwright-report/
```

### 5.4 Comando /diag_analyze

```yaml
# .claude/commands/diagnostics/diag_analyze.md
name: diag_analyze
description: "Analiza sesión de diagnóstico"
usage: "/diag_analyze <session-dir>"

actions:
  1. Parsear todos los logs con formato [DIAG:*]
  2. Correlacionar por timestamp
  3. Generar timeline unificado
  4. Identificar:
     - Tiempos de respuesta
     - Queries lentas
     - Errores
     - Gaps de tiempo sospechosos
  5. Generar reporte con sugerencias

output:
  - correlation_timeline.md
  - analysis_report.md
  - Sugerencias de optimización
```

---

## 6. MCP DB Server

### 6.1 Propósito

Servidor MCP que permite a Claude Code ejecutar queries de diagnóstico en la base de datos SQL Server en tiempo real.

### 6.2 Tools Disponibles

| Tool | Descripción | Uso |
|------|-------------|-----|
| `query_recent_logs` | Últimas N queries ejecutadas | Diagnóstico de flujos |
| `get_slow_queries` | Queries > X ms | Optimización |
| `read_only_query` | SELECT arbitrario | Verificar datos |
| `get_table_schema` | Estructura de tabla | Desarrollo |
| `connection_status` | Estado del pool | Diagnóstico de conexiones |
| `count_records` | Contar registros en tabla | Verificación rápida |

### 6.3 Configuración

```json
// .claude/settings.local.json
{
  "mcpServers": {
    "weplay-db": {
      "command": "node",
      "args": [".claude/mcp-servers/db-diagnostic/dist/index.js"],
      "env": {
        "DB_SERVER": "${from:.credentials.local.json:sqlServer.server}",
        "DB_NAME": "${from:.credentials.local.json:sqlServer.database}",
        "DB_USER": "${from:.credentials.local.json:sqlServer.user}",
        "DB_PASSWORD": "${from:.credentials.local.json:sqlServer.password}"
      }
    }
  }
}
```

### 6.4 Implementación

```typescript
// .claude/mcp-servers/db-diagnostic/src/index.ts
import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import sql from "mssql";

const server = new Server({
  name: "weplay-db-diagnostic",
  version: "1.0.0",
}, {
  capabilities: { tools: {} }
});

// Tool: count_records
server.setRequestHandler("tools/call", async (request) => {
  const { name, arguments: args } = request.params;

  if (name === "count_records") {
    const { table } = args;
    const result = await pool.request().query(`
      SELECT COUNT(*) as count FROM ${table}
    `);
    return {
      content: [{
        type: "text",
        text: `Table ${table}: ${result.recordset[0].count} records`
      }]
    };
  }

  // ... otros tools
});
```

---

## 7. Correlation Engine

### 7.1 Propósito

Unifica eventos de múltiples fuentes (API logs, frontend logs, DB queries, screenshots) en un timeline correlacionado por timestamp.

### 7.2 Fuentes de Datos

| Fuente | Tipo | Cómo se captura |
|--------|------|-----------------|
| API Logs | [DIAG:*] | stdout de dotnet run |
| Frontend Logs | console.log | Playwright console listener |
| DB Queries | JSON | MCP DB Server |
| Screenshots | PNG | Playwright screenshots |
| Network | HAR | Playwright network |

### 7.3 Formato de Timeline

```markdown
# Correlation Timeline

**Session**: 20260122_143022_login-flow
**Duration**: 4.2s
**Events**: 12

---

| Time | Source | Event | Status |
|------|--------|-------|--------|
| +0.0s | Screenshot | Login screen visible | ✅ |
| +1.2s | Frontend | [DIAG:form-filled] email=test@... | ✅ |
| +1.5s | Network | POST /api/auth/login | ➡️ |
| +1.6s | API | [DIAG:auth-entry] email=test@... | ✅ |
| +1.7s | DB Query | SELECT * FROM Users WHERE... | ✅ 45ms |
| +1.9s | API | [DIAG:auth-exit] success=true | ✅ |
| +2.0s | Network | 200 OK {token: ...} | ⬅️ |
| +2.1s | Frontend | [DIAG:login-success] navigating | ✅ |
| +2.8s | Screenshot | Dashboard visible | ✅ |

---

## Summary

- **Total time**: 2.8s
- **API calls**: 1
- **DB queries**: 1 (45ms)
- **Errors**: 0
```

### 7.4 Script de Correlación

```python
# .claude/scripts/diagnostics/correlation_engine.py
"""
Correlation Engine para WePlay Rises.
Unifica eventos de API, Frontend, DB y Screenshots.
"""

from datetime import datetime
from pathlib import Path
from dataclasses import dataclass
from enum import Enum
import re
import json

class EventSource(Enum):
    API = "API"
    FRONTEND = "Frontend"
    DB_QUERY = "DB Query"
    SCREENSHOT = "Screenshot"
    NETWORK = "Network"

@dataclass
class Event:
    timestamp: datetime
    source: EventSource
    message: str
    status: str = "✅"
    duration_ms: int = None

class CorrelationEngine:
    def __init__(self, session_dir: Path):
        self.session_dir = session_dir
        self.events: list[Event] = []

    def parse_api_logs(self, log_file: Path):
        """Parsea logs de API con formato [DIAG:*]"""
        pattern = r"\[DIAG:([^\]]+)\]\s*(\d{4}-\d{2}-\d{2}T[\d:.]+)\s*\|\s*(.+)"
        with open(log_file) as f:
            for line in f:
                match = re.search(pattern, line)
                if match:
                    point_id, ts, data = match.groups()
                    self.events.append(Event(
                        timestamp=datetime.fromisoformat(ts),
                        source=EventSource.API,
                        message=f"[DIAG:{point_id}] {data[:50]}..."
                    ))

    def parse_db_queries(self, queries_file: Path):
        """Parsea queries exportadas de MCP DB Server"""
        with open(queries_file) as f:
            queries = json.load(f)
        for q in queries:
            self.events.append(Event(
                timestamp=datetime.fromisoformat(q["timestamp"]),
                source=EventSource.DB_QUERY,
                message=q["query"][:60] + "...",
                duration_ms=q.get("duration_ms")
            ))

    def add_screenshots(self):
        """Añade eventos de screenshots"""
        for png in sorted((self.session_dir / "screenshots").glob("*.png")):
            # Extraer timestamp del nombre
            match = re.search(r"(\d{8}_\d{6})", png.name)
            if match:
                ts = datetime.strptime(match.group(1), "%Y%m%d_%H%M%S")
                self.events.append(Event(
                    timestamp=ts,
                    source=EventSource.SCREENSHOT,
                    message=f"[{png.stem}]"
                ))

    def generate_timeline(self) -> str:
        """Genera timeline en markdown"""
        self.events.sort(key=lambda e: e.timestamp)
        base_time = self.events[0].timestamp if self.events else datetime.now()

        lines = [
            "# Correlation Timeline",
            "",
            f"**Session**: {self.session_dir.name}",
            f"**Events**: {len(self.events)}",
            "",
            "---",
            "",
            "| Time | Source | Event | Status |",
            "|------|--------|-------|--------|"
        ]

        for event in self.events:
            delta = (event.timestamp - base_time).total_seconds()
            duration = f" ({event.duration_ms}ms)" if event.duration_ms else ""
            lines.append(
                f"| +{delta:.1f}s | {event.source.value} | {event.message}{duration} | {event.status} |"
            )

        return "\n".join(lines)
```

---

## 8. Scripts de Orquestación

### 8.1 Estructura

```
.claude/scripts/
├── build/
│   ├── build-all.ps1          # Build API + Web + Admin
│   └── check-health.ps1       # Verificar que todo funciona
│
├── testing/
│   ├── run-all-tests.ps1      # Orquestador principal
│   ├── run-backend-tests.ps1  # dotnet test
│   ├── run-frontend-tests.ps1 # npm run test
│   └── run-e2e-tests.ps1      # npx playwright test
│
├── data/
│   ├── seed-dev-data.sql      # Datos mínimos para desarrollo
│   ├── seed-demo-data.sql     # Datos completos para demo
│   └── clear-data.sql         # Limpiar datos de prueba
│
├── diagnostics/
│   ├── correlation_engine.py  # Engine de correlación
│   ├── parse_logs.py          # Parser de logs
│   └── generate_report.py     # Generador de reportes
│
└── deploy/
    ├── deploy-api.ps1         # Deploy API a Azure
    ├── deploy-web.ps1         # Deploy Web a Azure
    └── deploy-admin.ps1       # Deploy Admin a Azure
```

### 8.2 Script: build-all.ps1

```powershell
# .claude/scripts/build/build-all.ps1
param(
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"
$root = (Get-Item $PSScriptRoot).Parent.Parent.Parent.FullName

Write-Host "Building WePlay Rises..." -ForegroundColor Cyan

# Build API
Write-Host "`n[1/3] Building API (.NET)..." -ForegroundColor Yellow
Push-Location "$root/src/api/WebApi"
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) { throw "API build failed" }
Pop-Location
Write-Host "API build successful" -ForegroundColor Green

# Build Web
Write-Host "`n[2/3] Building Web (Vite)..." -ForegroundColor Yellow
Push-Location "$root/src/web"
npm run build
if ($LASTEXITCODE -ne 0) { throw "Web build failed" }
Pop-Location
Write-Host "Web build successful" -ForegroundColor Green

# Build Admin
Write-Host "`n[3/3] Building Admin (Next.js)..." -ForegroundColor Yellow
Push-Location "$root/src/admin"
npm run build
if ($LASTEXITCODE -ne 0) { throw "Admin build failed" }
Pop-Location
Write-Host "Admin build successful" -ForegroundColor Green

Write-Host "`nAll builds completed successfully!" -ForegroundColor Cyan
```

### 8.3 Script: run-all-tests.ps1

```powershell
# .claude/scripts/testing/run-all-tests.ps1
param(
    [ValidateSet("unit", "integration", "e2e", "all")]
    [string]$Level = "all",
    [switch]$StopOnFailure,
    [switch]$GenerateReport
)

$ErrorActionPreference = "Stop"
$root = (Get-Item $PSScriptRoot).Parent.Parent.Parent.FullName
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$sessionDir = "$root/.claude/test-sessions/$timestamp"
New-Item -ItemType Directory -Path $sessionDir -Force | Out-Null

$results = @{
    unit = $null
    integration = $null
    e2e = $null
}

function Run-UnitTests {
    Write-Host "`n[UNIT TESTS]" -ForegroundColor Cyan

    # Backend
    Write-Host "Running backend unit tests..."
    dotnet test "$root/src/api" --filter "Category=Unit" --logger "trx;LogFileName=$sessionDir/unit-backend.trx"
    $backendResult = $LASTEXITCODE

    # Frontend
    Write-Host "Running frontend unit tests..."
    Push-Location "$root/src/web"
    npm run test -- --reporter=json --outputFile="$sessionDir/unit-frontend.json"
    $frontendResult = $LASTEXITCODE
    Pop-Location

    return ($backendResult -eq 0 -and $frontendResult -eq 0)
}

function Run-IntegrationTests {
    Write-Host "`n[INTEGRATION TESTS]" -ForegroundColor Cyan
    dotnet test "$root/src/api" --filter "Category=Integration" --logger "trx;LogFileName=$sessionDir/integration.trx"
    return $LASTEXITCODE -eq 0
}

function Run-E2ETests {
    Write-Host "`n[E2E TESTS]" -ForegroundColor Cyan
    Push-Location "$root/e2e"
    npx playwright test --reporter=html --output="$sessionDir/playwright-report"
    $result = $LASTEXITCODE
    Pop-Location
    return $result -eq 0
}

# Ejecutar según nivel
if ($Level -eq "all" -or $Level -eq "unit") {
    $results.unit = Run-UnitTests
    if (-not $results.unit -and $StopOnFailure) {
        Write-Host "Unit tests failed. Stopping." -ForegroundColor Red
        exit 1
    }
}

if ($Level -eq "all" -or $Level -eq "integration") {
    $results.integration = Run-IntegrationTests
    if (-not $results.integration -and $StopOnFailure) {
        Write-Host "Integration tests failed. Stopping." -ForegroundColor Red
        exit 1
    }
}

if ($Level -eq "all" -or $Level -eq "e2e") {
    $results.e2e = Run-E2ETests
}

# Resumen
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "TEST RESULTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
foreach ($key in $results.Keys) {
    if ($null -ne $results[$key]) {
        $status = if ($results[$key]) { "PASSED" } else { "FAILED" }
        $color = if ($results[$key]) { "Green" } else { "Red" }
        Write-Host "$key : $status" -ForegroundColor $color
    }
}

Write-Host "`nSession: $sessionDir"
```

---

## 9. Templates de Código

### 9.1 Estructura

```
.claude/templates/
├── api/
│   ├── Command.template.cs
│   ├── Query.template.cs
│   ├── Validator.template.cs
│   ├── Controller.template.cs
│   ├── Service.template.cs
│   └── Profile.template.cs
│
├── web/
│   ├── page.template.tsx
│   ├── hook.template.ts
│   ├── service.template.ts
│   └── component.template.tsx
│
├── admin/
│   ├── page.template.tsx
│   ├── form.template.tsx
│   └── table.template.tsx
│
├── tests/
│   ├── handler-test.template.cs
│   ├── hook-test.template.ts
│   └── e2e-test.template.ts
│
└── shared/
    ├── type.template.ts
    └── schema.template.ts
```

### 9.2 Template: Command + Handler

```csharp
// .claude/templates/api/Command.template.cs
// Variables: {{EntityName}}, {{ModuleName}}, {{Properties}}

using MediatR;
using FluentValidation;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Common;
using WePlayRises.{{ModuleName}}.Application.Interfaces.Services;
using WePlayRises.{{ModuleName}}.Domain.Model;

namespace WePlayRises.{{ModuleName}}.Application.Features.{{EntityName}}.Commands;

public class Create{{EntityName}}Command : IRequest<ServiceResponse<Guid>>
{
    {{Properties}}
}

public class Create{{EntityName}}CommandHandler : IRequestHandler<Create{{EntityName}}Command, ServiceResponse<Guid>>
{
    private readonly I{{EntityName}}Service _service;
    private readonly IMapper _mapper;
    private readonly IValidator<Create{{EntityName}}Command> _validator;
    private readonly ILogger<Create{{EntityName}}CommandHandler> _logger;

    public Create{{EntityName}}CommandHandler(
        I{{EntityName}}Service service,
        IMapper mapper,
        IValidator<Create{{EntityName}}Command> validator,
        ILogger<Create{{EntityName}}CommandHandler> logger)
    {
        _service = service;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ServiceResponse<Guid>> Handle(Create{{EntityName}}Command request, CancellationToken ct)
    {
        try
        {
            // Validación
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<Guid>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Mapping y creación
            var entity = _mapper.Map<{{EntityName}}>(request);
            var id = await _service.CreateAsync(entity, ct);

            return new ServiceResponse<Guid>
            {
                Data = id,
                Messages = new() { new() { Message = "{{EntityName}} creado", ErrorCode = "SUCCESS" } }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {{EntityName}}");
            return new ServiceResponse<Guid>
            {
                Messages = new() { new() { Message = "Error inesperado", ErrorCode = "ERROR_UNEXPECTED" } }
            };
        }
    }
}
```

### 9.3 Template: Custom Hook

```typescript
// .claude/templates/web/hook.template.ts
// Variables: {{EntityName}}, {{entityName}}, {{EntityNamePlural}}

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { {{entityName}}Service } from '@/services/{{entityName}}.service';
import { {{EntityName}}, Create{{EntityName}}Dto } from '@shared/types';
import { QUERY_KEYS } from '@shared/constants';
import { toast } from 'sonner';

export const use{{EntityNamePlural}} = () => {
  return useQuery({
    queryKey: [QUERY_KEYS.{{EntityNamePlural}}],
    queryFn: () => {{entityName}}Service.getAll(),
  });
};

export const use{{EntityName}} = (id: string) => {
  return useQuery({
    queryKey: [QUERY_KEYS.{{EntityNamePlural}}, id],
    queryFn: () => {{entityName}}Service.getById(id),
    enabled: !!id,
  });
};

export const useCreate{{EntityName}} = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: Create{{EntityName}}Dto) => {{entityName}}Service.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.{{EntityNamePlural}}] });
      toast.success('{{EntityName}} creado correctamente');
    },
    onError: (error) => {
      toast.error('Error al crear {{entityName}}');
      console.error(error);
    },
  });
};

export const useUpdate{{EntityName}} = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Partial<{{EntityName}}> }) =>
      {{entityName}}Service.update(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.{{EntityNamePlural}}] });
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.{{EntityNamePlural}}, id] });
      toast.success('{{EntityName}} actualizado correctamente');
    },
    onError: (error) => {
      toast.error('Error al actualizar {{entityName}}');
      console.error(error);
    },
  });
};

export const useDelete{{EntityName}} = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => {{entityName}}Service.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.{{EntityNamePlural}}] });
      toast.success('{{EntityName}} eliminado correctamente');
    },
    onError: (error) => {
      toast.error('Error al eliminar {{entityName}}');
      console.error(error);
    },
  });
};
```

---

## 10. Orden de Implementación

### Fase 0: Setup Inicial (1-2h)

```
PRIORIDAD CRÍTICA:
├─► Crear estructura .claude/scripts/
├─► Implementar /build-check
├─► Implementar /seed-data (datos mínimos)
└─► Verificar que API conecta a DB

PRIORIDAD ALTA:
├─► Crear templates básicos (Command, Hook)
├─► Configurar Playwright
└─► Crear primer test E2E (smoke test)

PRIORIDAD MEDIA:
├─► Implementar agente backend-cqrs
└─► Implementar agente frontend-feature
```

### Después de cada Slice

```
Al completar un slice:
1. /run-tests --level unit
2. Si pasan → /run-tests --level integration
3. Si pasan → /run-tests --level e2e
4. Commit solo si todo pasa
```

---

## 11. Integración con Slices

Este tooling se usará en cada slice del plan de desarrollo:

| Slice | Comandos a usar |
|-------|-----------------|
| 1: Auth | `/new-endpoint`, backend-cqrs agent |
| 2: Dashboard | `/new-page`, frontend-feature agent |
| 3: Crear Campaña | `/new-endpoint`, `/run-tests` |
| 4: Explorar | frontend-feature agent, `/run-tests` |
| 5: Detalle | `/new-page`, test-generator agent |
| 6: Backing | `/new-endpoint`, `/diag_run` (si hay problemas) |
| 7: Mis Backings | frontend-feature agent |
| 8: CRUD Rewards | `/new-entity` completo |
| 9: Editar | Reutilizar templates |
| 10: Deploy | `/deploy-preview`, `/run-tests --level all` |

---

## 12. Referencias

### Documentación Interna
- [Plan de Desarrollo por Slices](./20260122_desarrollo-slices.md)
- [CLAUDE.md](../CLAUDE.md) - Reglas del proyecto
- [Reglas CQRS](../.claude/rules/backend/cqrs.rule.md)

### Herramientas Externas
- [Playwright Documentation](https://playwright.dev/docs/intro)
- [Vitest Documentation](https://vitest.dev/guide/)
- [MCP Protocol](https://modelcontextprotocol.io/)
- [TanStack Query](https://tanstack.com/query/latest)

---

**Documento generado**: 2026-01-22
**Próxima revisión**: Al completar Slice 0
