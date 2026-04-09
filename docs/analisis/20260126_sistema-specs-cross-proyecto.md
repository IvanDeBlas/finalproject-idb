# Sistema de Especificaciones Cross-Proyecto - WePlay Rises

> - **Fecha:** 2026-01-26
> - **Estado:** Borrador - Definición del sistema
> - **Basado en:** Arquitectura Hub-Spoke y Workflow Cross-Repo de miUrba

---

## Contexto

WePlay Rises tiene **3 proyectos de código** que deben coordinarse:

| Proyecto | Ruta | Framework | Rol |
|----------|------|-----------|-----|
| **Backend** | `src/api/` | .NET 8 + EF Core + CQRS | API REST, lógica de negocio, persistencia |
| **Landing** | `src/web/` | Vite + React 18 | UI pública: explorar campañas, hacer backing |
| **Admin** | `src/admin/` | Next.js 14 | Dashboard artista: CRUD campañas, gestión |
| **Shared** | `src/shared/` | TypeScript | Tipos, constantes, schemas compartidos |

---

## El Problema

Sin especificaciones cross-proyecto:

```
Backend: "Creé POST /api/campanias con CreateCampaniaDto"
Landing: "Yo esperaba que se llamara CampaniaCreateRequest"
Admin:   "¿Qué errores devuelve? ¿Qué validaciones hay?"

Resultado: 3 implementaciones incompatibles
```

Con especificaciones:

```
Spec Cross-Proyecto:
├── Contrato API definido (endpoints, DTOs, errores)
├── Tipos TypeScript generados/sincronizados
├── Cada proyecto implementa contra el mismo contrato
└── Trabajo paralelo sin fricciones
```

---

## Arquitectura Hub-Spoke Adaptada

```
WePlay_Rises/                    # WORKSPACE (Hub de coordinación)
│
├── tasks/                       # Tracking de tareas
│   ├── README.md               # Índice
│   └── WPR-XXX.md              # Detalle por tarea
│
├── docs/specs/                  # 🆕 SPECS CROSS-PROYECTO
│   └── {feature-id}/
│       ├── feature-spec.md     # QUÉ: User stories
│       ├── contracts.md        # CONTRATOS: APIs, DTOs, tipos
│       └── implementation.md   # CÓMO: Pasos por proyecto
│
├── src/                         # PROYECTOS (Spokes)
│   ├── api/                    # Backend .NET
│   ├── web/                    # Landing Vite
│   ├── admin/                  # Dashboard Next.js
│   └── shared/                 # Código compartido
│
└── .claude/                     # Tooling Claude Code
    ├── commands/
    └── templates/
```

---

## Cuándo Crear una Spec Cross-Proyecto

### Crear spec cuando:

| Criterio | Umbral |
|----------|--------|
| Proyectos involucrados | ≥2 proyectos |
| Endpoints/APIs nuevas | ≥1 endpoint que Frontend consume |
| Decisiones arquitectónicas | No triviales |

### NO crear spec cuando:

- Cambio solo en 1 proyecto
- Refactoring interno sin cambio de API
- Fix de bugs que no cambia contratos

---

## Estructura de una Spec (3 Archivos)

### 1. feature-spec.md (QUÉ)

```markdown
# Feature: {Nombre}

**ID**: WPR-XXX
**User Story**: US-XX
**Status**: proposed | in-progress | completed

## Descripción
{Qué problema resuelve}

## User Stories
### US-XX: {Título}
**Como** {rol}
**Quiero** {acción}
**Para** {beneficio}

## Criterios de Aceptación
- [ ] {Criterio 1}
- [ ] {Criterio 2}

## Proyectos Involucrados
| Proyecto | Responsabilidad |
|----------|-----------------|
| Backend  | {qué hace} |
| Landing  | {qué hace} |
| Admin    | {qué hace} |
```

### 2. contracts.md (CONTRATOS)

```markdown
# Contratos: {Nombre Feature}

## 📡 Endpoints API

### POST /api/{resource}
**Descripción**: {qué hace}
**Autenticación**: Bearer JWT | Pública

**Request Body**:
```json
{
  "campo1": "string (required, max 200)",
  "campo2": 1000.00
}
```

**Response 201**:
```json
{
  "data": {
    "id": "guid",
    "campo1": "string"
  },
  "messages": []
}
```

**Errores posibles**:
| Código | ErrorCode | Mensaje |
|--------|-----------|---------|
| 400 | VALIDATION_REQUIRED | El campo X es obligatorio |
| 401 | UNAUTHORIZED | Token inválido |
| 404 | NOT_FOUND | Recurso no encontrado |

---

## 📦 DTOs / Types Compartidos

### Backend (C#)
```csharp
// src/api/.../Dtos/{Resource}Dto.cs
public class {Resource}Dto
{
    public Guid Id { get; set; }
    public string Campo1 { get; set; } = null!;
    public decimal Campo2 { get; set; }
}
```

### Frontend (TypeScript)
```typescript
// src/shared/types/{resource}.ts
export interface {Resource} {
  id: string;
  campo1: string;
  campo2: number;
}
```

---

## 🔄 Flujo de Datos

```
[Landing/Admin] → POST /api/resource → [Backend] → DB
                                           │
                                           ▼
[Landing/Admin] ← Response JSON ← [Backend]
```

---

## 📋 Validaciones Compartidas

| Campo | Regla | Backend | Frontend |
|-------|-------|---------|----------|
| campo1 | required, max 200 | FluentValidation | Zod schema |
| campo2 | > 0 | FluentValidation | Zod schema |
```

### 3. implementation.md (CÓMO)

```markdown
# Implementación: {Nombre Feature}

## Orden de Implementación

1. **Backend primero** (bloquea a Frontend)
2. **Shared types** (sincronizar con Backend)
3. **Landing/Admin** (en paralelo)

---

## Backend (.NET)

### Archivos a crear
| Ruta | Descripción |
|------|-------------|
| `Modules/.../Commands/Create{X}Command.cs` | Command + Handler |
| `Modules/.../Dtos/{X}Dto.cs` | DTO |
| `Modules/.../Validators/Create{X}Validator.cs` | Validador |

### Comandos
```bash
cd src/api
dotnet build
dotnet test
```

---

## Shared (TypeScript)

### Archivos a crear/actualizar
| Ruta | Descripción |
|------|-------------|
| `src/shared/types/{resource}.ts` | Tipos TypeScript |
| `src/shared/schemas/{resource}.schema.ts` | Zod schema |

---

## Landing (Vite)

### Archivos a crear
| Ruta | Descripción |
|------|-------------|
| `src/features/{x}/hooks/use{X}.ts` | Query hook |
| `src/features/{x}/components/{X}Card.tsx` | Componente |

### Comandos
```bash
cd src/web
npm run dev
npm run build
```

---

## Admin (Next.js)

### Archivos a crear
| Ruta | Descripción |
|------|-------------|
| `src/app/(dashboard)/{x}/page.tsx` | Página |
| `src/hooks/use{X}Mutation.ts` | Mutation hook |

### Comandos
```bash
cd src/admin
npm run dev
npm run build
```

---

## Mapeo User Stories → Specs → Tareas

| US | Spec | Tareas Backend | Tareas Landing | Tareas Admin |
|----|------|----------------|----------------|--------------|
| US-01 | `registro-artista/` | WPR-006, WPR-010 | WPR-010 | WPR-010 |
| US-02 | `crear-campania/` | WPR-011 | - | WPR-011 |
| US-03 | `definir-rewards/` | WPR-012 | - | WPR-012 |
| US-04 | `hacer-backing/` | WPR-013 | WPR-013 | - |
| US-05 | `dashboard-artista/` | WPR-014 | - | WPR-014 |

---

## Workflow: De Spec a Implementación

```
┌─────────────────────────────────────────────────────────────────┐
│                    WORKFLOW CROSS-PROYECTO                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  FASE 1: ESPECIFICACIÓN (En Workspace)                          │
│  ─────────────────────────────────────                          │
│  1. Crear docs/specs/{feature-id}/                              │
│  2. Escribir feature-spec.md (user stories)                     │
│  3. Definir contracts.md (APIs, DTOs, tipos)                    │
│  4. Planificar implementation.md (pasos)                        │
│                                                                 │
│  FASE 2: BACKEND PRIMERO                                        │
│  ───────────────────────                                        │
│  1. Implementar endpoints según contracts.md                    │
│  2. Crear DTOs exactamente como se especificaron                │
│  3. Probar con Swagger/Postman                                  │
│  4. Marcar tareas Backend como completadas                      │
│                                                                 │
│  FASE 3: SHARED TYPES                                           │
│  ────────────────────                                           │
│  1. Crear/actualizar tipos en src/shared/                       │
│  2. Sincronizar con DTOs de Backend                             │
│                                                                 │
│  FASE 4: FRONTEND (Paralelo)                                    │
│  ──────────────────────────                                     │
│  1. Landing y Admin pueden desarrollar en paralelo              │
│  2. Ambos consumen la misma API con los mismos tipos            │
│  3. Marcar tareas Frontend como completadas                     │
│                                                                 │
│  FASE 5: INTEGRACIÓN                                            │
│  ───────────────────                                            │
│  1. Test E2E del flujo completo                                 │
│  2. Verificar que todos los proyectos funcionan juntos          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Comandos y Agentes por Proyecto

### Backend (.NET)
| Comando/Agente | Propósito |
|----------------|-----------|
| `/new-entity` | Scaffolding de entidad completa |
| `/new-endpoint` | Scaffolding de Command + Handler |
| `cqrs-planning-architect` | Planificar estructura CQRS |
| `dotnet-test-architect` | Planificar tests |

### Frontend (React)
| Comando/Agente | Propósito |
|----------------|-----------|
| `/explore-plan` | Analizar y planificar feature |
| `/implement` | Implementar según plan |
| `feature-planner` | Planificar feature completa |
| `frontend-developer` | Implementar código |
| `shadcn-ui-architect` | Diseñar UI con shadcn |

---

## Próximos Pasos

1. [ ] Crear estructura `docs/specs/` en el workspace
2. [ ] Crear primera spec para US-01 (Registro Artista)
3. [ ] Definir contratos (endpoints, DTOs, tipos)
4. [ ] Adaptar comandos de miUrba a WePlay Rises
5. [ ] Probar workflow con una feature completa

---

## Referencias

- [User Stories MVP](../specs/user-stories-mvp.md)
- [Mapeo US → Tareas](./20260126_user-stories-tasks-mapping.md)
- miUrba Knowledge Transfer docs (07, 08, 09, 10)

---

*Última actualización: 2026-01-26*
