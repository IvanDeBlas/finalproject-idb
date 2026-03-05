# Introduccion Reunion - Paradigma Agentico y CodePilot

- **Fecha**: 2026-01-23
- **Autor**: Iván

---

## 1. Mejoras de Claude Code: Paradigma de Desarrollo Agentico

He descubierto que para proyectos nuevos, Claude Code es significativamente mas efectivo cuando se estructura adecuadamente:

### Sistema de Configuracion

| Componente | Proposito |
|------------|-----------|
| **Hooks** | Automatizacion silenciosa (formateo, deteccion de secrets, captura de diagnosticos) |
| **Templates** | Codigo boilerplate consistente (Commands CQRS, componentes React, validators) |
| **Rules** | Reglas contextuales que se cargan segun el archivo que editas (`.cs` → reglas .NET, `.tsx` → reglas React) |

### Comandos Orquestados

En lugar de pedir tareas sueltas, el paradigma define comandos que orquestan **multiples agentes en paralelo**:

```
/explore (dev)   → Especificacion completa con datos sinteticos (4 agentes paralelos)
/implement (build) → Worktree aislado + implementacion + puntos #DIAG
/test ¿?  → Seed data + escenarios + verificacion automatica
/fix  ¿?  → Auto-diagnose + auto-resolve (max 3 intentos)
/ship  → Limpieza + tests + pipeline + PR
```

**Beneficio**: Una feature completa en ~40-70 minutos con calidad consistente.

---

## 2. CodePilot: Panel de Control para Desarrollo con IA

### ¿Qué es?

Una **aplicacion web en React** que permite:

- Orquestar multiples instancias de Claude Code en paralelo
- Visualizar terminales en grid 2x2 (backend + frontend + tests)
- Gestionar sesiones (crear, pausar, reanudar)
- Persistir estado del progreso

### Arquitectura

```
┌─────────────────────────────────┐
│       CodePilot UI (React)      │  ← Panel web
├─────────────────────────────────┤
│  Zustand + React Query          │  ← Estado
├─────────────────────────────────┤
│  REST API + WebSocket (Node.js) │  ← Backend ligero
├─────────────────────────────────┤
│  xterm.js + node-pty            │  ← Terminales reales
├─────────────────────────────────┤
│  Claude Code CLI                │  ← Agente de desarrollo
└─────────────────────────────────┘
```

### Objetivo

Levantar una **VM en Azure** donde:
- CodePilot corre como aplicacion web
- Se pueda controlar el desarrollo desde el movil
- Las sesiones de Claude Code persistan entre conexiones

---

## 3. WePlay Rises: El Proyecto Destino

### Concepto

Plataforma para artistas noveles con tres pilares:
- **CrowdFunding**: Financiar proyectos con recompensas
- **CrowdSourcing**: Contratar profesionales (futuro)
- **CrowdPromotion**: Promocion via fans e influencers (futuro)

> Fue mi primer emprendimiento en 2008. Ahora es mi presentacion final al master AI4Devs con limite de desarrollo de 30h.

### Stack

| Capa | Tecnologia |
|------|------------|
| Backend | .NET 8 Monolito Modular (CQRS, MediatR) |
| Landing | Vite + React + shadcn/ui (puerto 3000) |
| Dashboard | Next.js 14 + shadcn/ui (puerto 3001) |
| DB | SQL Server |

### Estado Actual

- **Tiempo invertido**: ~6h
- **Avance**: Estructura completa, BuildingBlocks, modulos base
- **Pendiente**: Identity, endpoints CRUD, componentes UI, deploy

### Por que iterar CodePilot aqui

- Es un **monorepo** mas simple que miUrba (multi-repo)
- Tiene las tres capas (API + 2 frontends) para probar desarrollo paralelo
- Deadline concreto que fuerza iteracion rapida

---

## Resumen

| Elemento | Estado | Objetivo |
|----------|--------|----------|
| Paradigma Agentico | Especificacion v2.0 completa | Implementar en WePlay Rises |
| CodePilot | Especificacion tecnica lista | MVP funcional en VM Azure |
| WePlay Rises | ~20% avanzado | Completar MVP en <24h restantes |

