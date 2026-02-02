# WePlay Rises - Gestión de Tareas

> **Punto de entrada único** para encontrar cualquier tarea del proyecto.

---

## Estado Actual

| En Progreso | Próxima | Bloqueada |
|-------------|---------|-----------|
| - | WPR-001 | - |

---

## Progreso General

```
[#####.....................] 17% (5.0/30h)
```

| Métrica | Valor |
|---------|-------|
| Tareas completadas | 2 / 15 |
| Horas invertidas | 5.0h |
| Horas restantes | 25.0h |

---

## Índice de Tareas

### Fase 1: Fundamentos (8h estimadas)

| ID | Título | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| [WPR-001](WPR-001.md) | Definir modelo de datos MVP | `Pendiente` | Alta | 2h |
| [WPR-002](WPR-002.md) | Copiar BuildingBlocks de miUrba | `Completada` | Alta | 2h |
| [WPR-003](WPR-003.md) | Crear estructura de módulos MVP | `Completada` | Alta | 2h |
| [WPR-004](WPR-004.md) | Configurar DbContext y migrations | `Pendiente` | Alta | 1h |
| [WPR-005](WPR-005.md) | Setup proyecto React + Templates | `Pendiente` | Alta | 1h |
| [WPR-006](WPR-006.md) | Implementar Identity mínimo (JWT) | `Pendiente` | Alta | 1.5h |

### Fase 2: User Stories MVP (12h estimadas)

| ID | Título | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| [WPR-010](WPR-010.md) | US-01: Registro de Artista | `Pendiente` | Alta | 2h |
| [WPR-011](WPR-011.md) | US-02: Crear Campaña | `Pendiente` | Alta | 3h |
| [WPR-012](WPR-012.md) | US-03: Definir Recompensas | `Pendiente` | Alta | 2h |
| [WPR-013](WPR-013.md) | US-04: Hacer Backing | `Pendiente` | Alta | 2h |
| [WPR-014](WPR-014.md) | US-05: Dashboard Artista | `Pendiente` | Alta | 2h |

#### US-01: Registro de Artista (Desglose)

| ID | Título | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| [WPR-015](WPR-015-registro-artista-shared.md) | Registro Artista - Shared | `Pendiente` | Alta | 1h |
| [WPR-016](WPR-016-registro-artista-backend.md) | Registro Artista - Backend | `Pendiente` | Alta | 3h |
| [WPR-017](WPR-017-registro-artista-landing.md) | Registro Artista - Landing | `Pendiente` | Media | 1.5h |
| [WPR-018](WPR-018-registro-artista-admin.md) | Registro Artista - Admin | `Pendiente` | Alta | 2h |

### Fase 3: Testing y Deploy (10h estimadas)

| ID | Título | Estado | Prioridad | Estimado |
|----|--------|--------|-----------|----------|
| [WPR-020](WPR-020.md) | Tests unitarios backend | `Pendiente` | Media | 3h |
| [WPR-021](WPR-021.md) | Tests de integración | `Pendiente` | Media | 2h |
| [WPR-022](WPR-022.md) | Deploy Azure | `Pendiente` | Media | 3h |
| [WPR-023](WPR-023.md) | Documentación final | `Pendiente` | Media | 2h |

---

## Leyenda de Estados

| Estado | Descripción |
|--------|-------------|
| `Pendiente` | No iniciada, esperando |
| `En Progreso` | Actualmente en desarrollo |
| `Bloqueada` | Esperando dependencia |
| `Completada` | Finalizada y validada |

---

## Recursos

| Recurso | Descripción |
|---------|-------------|
| [time-log.md](time-log.md) | Registro de horas por sesión |
| [priority-rules.yaml](priority-rules.yaml) | Reglas de priorización |
| [backlog.md](backlog.md) | Backlog original (legacy) |

---

## Cómo usar este sistema

### Para encontrar una tarea
1. Busca en el índice por ID o título
2. Click en el link para ver el detalle completo

### Para empezar a trabajar
1. Revisa "Estado Actual" arriba
2. Si no hay nada en progreso, toma la "Próxima"
3. Actualiza el estado a `En Progreso`

### Para crear una nueva tarea
1. Copia el template: `.claude/templates/tasks/task.template.md`
2. Crea archivo `tasks/WPR-XXX.md`
3. Agrega entrada en este índice

### Para completar una tarea
1. Marca todos los criterios de aceptación
2. Actualiza estado a `Completada`
3. Registra tiempo en `time-log.md`
4. Actualiza este índice

---

## Dependencias entre Tareas

```
WPR-001 ─┬─> WPR-004 ──> WPR-006 ──┬─> WPR-010 ──> WPR-011 ──> WPR-012
         │                         │
WPR-002 ─┤                         ├─> WPR-013
         │                         │
WPR-003 ─┘                         └─> WPR-014
                                            │
WPR-005 ────────────────────────────────────┘

WPR-010..014 ──> WPR-020 ──> WPR-021 ──> WPR-022 ──> WPR-023
```

---

*Última actualización: 2026-01-26*
