# Análisis de Tooling Claude Code

**Fecha:** 2026-01-23
**Autor:** Claude Code
**Repositorio analizado:** [gurusup/claude-code-demo](https://github.com/gurusup/claude-code-demo/tree/main/.claude)
**Proyecto destino:** WePlay Rises

---

## 1. Resumen Ejecutivo

Se realizó un análisis comparativo entre el sistema de tooling del repositorio `claude-code-demo` y la configuración actual de WePlay Rises para identificar componentes que aporten valor al desarrollo del MVP.

**Conclusión:** Se identificaron **3 agentes** y **3 comandos** prioritarios para trasladar, con adaptaciones mínimas al stack tecnológico (.NET 8, React, shadcn/ui).

---

## 2. Inventario Comparativo

### 2.1 Estado Actual de WePlay Rises

| Categoría | Componentes |
|-----------|-------------|
| **Rules** | `cqrs.rule.md`, `dotnet.rule.md`, `ef-core.rule.md`, `react-shadcn.rule.md`, `typescript.rule.md`, `unit-tests.rule.md`, `code-style.rule.md`, `git-conventions.rule.md` |
| **Commands** | `/done`, `/health-check`, `/context-mode`, `/new-entity`, `/new-endpoint`, `/remember`, `/memories`, `/forget` |
| **Contexts** | `dev.md`, `review.md`, `research.md` |
| **Templates** | API (CQRS Commands/Queries/Validators), React components |
| **Agentes** | ❌ No implementados |

### 2.2 Repositorio Demo (gurusup/claude-code-demo)

| Categoría | Componentes |
|-----------|-------------|
| **Agentes** | `backend-test-architect`, `frontend-developer`, `frontend-test-engineer`, `hexagonal-backend-architect`, `qa-criteria-validator`, `shadcn-ui-architect`, `typescript-test-explorer`, `ui-ux-analyzer` |
| **Commands** | `analyze_bug`, `create-new-gh-issue`, `explore-plan`, `implement-feedback`, `rule2hook`, `start-working-on-issue-new`, `update-feedback`, `worktree-tdd`, `worktree` |
| **Hooks** | `on-notification-say.sh` |

---

## 3. Análisis Detallado de Candidatos

### 3.1 Agentes Recomendados

#### ✅ `shadcn-ui-architect` — ALTA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Planificar implementación de componentes shadcn/ui con accesibilidad WCAG 2.1 AA |
| **Relevancia** | Stack idéntico (React + shadcn/ui + Tailwind) |
| **Adaptación** | Ninguna requerida |
| **Valor** | Estandariza arquitectura de componentes UI, reduce decisiones ad-hoc |

**Workflow del agente:**
1. Análisis de componentes disponibles
2. Investigación de demos oficiales
3. Propuesta documentada (sin implementar)

**Output:** `.claude/doc/{feature_name}/shadcn_ui.md`

---

#### ✅ `qa-criteria-validator` — ALTA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Definir criterios de aceptación Given-When-Then y validar con Playwright |
| **Relevancia** | E2E testing es crítico para MVP |
| **Adaptación** | Cambiar yarn por npm |
| **Valor** | Criterios testables, reportes estructurados |

**Formato de salida:**
```
Feature: [Nombre]
Acceptance Criteria:
  Given [contexto]
  When [acción]
  Then [resultado esperado]

Validation Report:
  ✅ Passed | ❌ Failed | ⚠️ Warning
```

---

#### ✅ `backend-test-architect` — MEDIA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Diseñar estrategia de testing por capas arquitectónicas |
| **Relevancia** | Arquitectura CQRS/Modular requiere testing estructurado |
| **Adaptación** | Cambiar NextJS → .NET 8, Jest → xUnit, adaptar a capas CQRS |
| **Valor** | Testing sistemático Domain/Application/Infrastructure |

**Adaptación de capas:**
| Original (NextJS) | WePlay Rises (.NET) |
|-------------------|---------------------|
| Domain entities | Domain/Model |
| Use cases | Application/Features (Handlers) |
| Repository adapters | Infra/Repositories |
| API routes | WebApi/Controllers |

---

### 3.2 Comandos Recomendados

#### ✅ `/analyze-bug` — ALTA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Investigar bugs sin ejecutar correcciones |
| **Valor** | Diagnóstico estructurado antes de actuar |
| **Integración** | Compatible con Sentry, stack traces, logs |

**Uso esperado:**
```bash
/analyze-bug "NullReferenceException en CampaniaService.GetByIdAsync"
```

---

#### ✅ `/create-gh-issue` — MEDIA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Template estandarizado para issues GitHub |
| **Valor** | Consistencia, Definition of Done incluido |
| **Integración** | Usa `gh issue create` CLI |

**Estructura generada:**
- Problem Statement
- User Value
- Definition of Done (criterios de aceptación)
- Manual Testing Checklist
- Quality Criteria (>80% coverage, CI green)

---

#### ✅ `/start-issue` — BAJA PRIORIDAD

| Aspecto | Detalle |
|---------|---------|
| **Propósito** | Workflow completo de 8 fases para resolver issues |
| **Valor** | Proceso E2E desde setup hasta PR merged |
| **Complejidad** | Alta - requiere worktrees y coordinación de agentes |

**Fases:**
1. Setup (actualizar ramas)
2. Worktree (rama aislada)
3. Analysis (revisar issue completo)
4. Implementation (TDD)
5. Reporting (documentar estado)
6. PR Monitoring (verificar CI/CD)
7. Fix Loop (correcciones iterativas)
8. Final Checks (validación completa)

---

### 3.3 Componentes Descartados

| Componente | Razón de exclusión |
|------------|-------------------|
| `hexagonal-backend-architect` | WePlay usa CQRS/Modular, no hexagonal puro |
| `frontend-developer` | Genérico, ya cubierto por `react-shadcn.rule.md` |
| `explore-plan` | Ya existe `/context-mode research` |
| `rule2hook` | Específico al proyecto original |
| `worktree.md` | Versión básica, `worktree-tdd` es más completo |
| `implement-feedback` | Útil pero no prioritario para MVP |
| `update-feedback` | Complementario a implement-feedback |

---

## 4. Plan de Implementación

### Fase 1: Inmediata (Sprint actual)

| Item | Esfuerzo | Impacto |
|------|----------|---------|
| `shadcn-ui-architect` | 1h | Alto |
| `/analyze-bug` | 30min | Alto |

### Fase 2: Corto plazo (Próximo sprint)

| Item | Esfuerzo | Impacto |
|------|----------|---------|
| `qa-criteria-validator` | 2h | Alto |
| `/create-gh-issue` | 1h | Medio |

### Fase 3: Opcional (Post-MVP)

| Item | Esfuerzo | Impacto |
|------|----------|---------|
| `backend-test-architect` | 3h (adaptación) | Medio |
| `/start-issue` | 2h | Medio |

---

## 5. Estructura Propuesta

```
.claude/
├── agents/                          # NUEVO
│   ├── shadcn-ui-architect.md
│   ├── qa-criteria-validator.md
│   └── backend-test-architect.md    # Adaptado a .NET
├── commands/
│   ├── debugging/                   # NUEVO
│   │   └── analyze-bug.md
│   ├── github/                      # NUEVO
│   │   └── create-gh-issue.md
│   ├── workflow/
│   │   ├── context-mode.md
│   │   └── start-issue.md           # NUEVO
│   └── ... (existentes)
├── doc/                             # NUEVO (output de agentes)
│   └── {feature_name}/
│       ├── shadcn_ui.md
│       └── qa_criteria.md
└── ... (existentes)
```

---

## 6. Consideraciones

### Restricción de tiempo
- **30 horas** disponibles para MVP
- Priorizar herramientas que aceleren desarrollo, no que agreguen overhead

### Compatibilidad de stack
- Agentes originales asumen NextJS/yarn
- Adaptar a .NET 8 + npm donde aplique

### Principio de mínima intervención
- No trasladar componentes "por si acaso"
- Solo lo que tenga uso inmediato demostrable

---

## 7. Próximos Pasos

1. [ ] Confirmar prioridades con el equipo
2. [ ] Trasladar `shadcn-ui-architect` (sin modificaciones)
3. [ ] Crear `/analyze-bug` adaptado al proyecto
4. [ ] Evaluar resultados antes de continuar con Fase 2

---

*Documento generado automáticamente por Claude Code*
