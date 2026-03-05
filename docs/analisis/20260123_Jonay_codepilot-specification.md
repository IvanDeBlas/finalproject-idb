# CodePilot - Especificaciones Técnicas

> Panel de control para orquestación de desarrollo asistido por IA

- **Versión**: 1.0.0
- **Autor**: Jonay
- **Fecha**: Enero 2026
- **Proyecto destino**: WePlay Rises (aplicable a cualquier proyecto)

---

## Índice

1. [Visión General](#1-visión-general)
2. [Propuesta de Valor](#2-propuesta-de-valor)
3. [Arquitectura del Sistema](#3-arquitectura-del-sistema)
4. [Stack Tecnológico](#4-stack-tecnológico)
5. [Estructura del Proyecto](#5-estructura-del-proyecto)
6. [Sistema de Terminales](#6-sistema-de-terminales)
7. [Gestión de Sesiones Claude](#7-gestión-de-sesiones-claude)
8. [API REST Backend](#8-api-rest-backend)
9. [Gestión de Estado](#9-gestión-de-estado)
10. [Integraciones](#10-integraciones)
11. [Flujo de Trabajo](#11-flujo-de-trabajo)
12. [Configuración](#12-configuración)
13. [Deployment](#13-deployment)
14. [Adaptación a WePlay Rises](#14-adaptación-a-weplay-rises)

---

## 1. Visión General

### ¿Qué es CodePilot?

CodePilot es un **panel de control web** diseñado para:

1. **Orquestar múltiples instancias de Claude Code** trabajando en paralelo
2. **Visualizar terminales** ejecutando Claude Code en tiempo real
3. **Gestionar sesiones** (crear, pausar, reanudar, cancelar)
4. **Coordinar desarrollo** en proyectos multi-repositorio
5. **Persistir estado** del progreso de desarrollo

### Problema que Resuelve

| Sin CodePilot | Con CodePilot |
|---------------|---------------|
| Múltiples ventanas de terminal abiertas | Vista unificada de todas las sesiones |
| Difícil coordinar tareas paralelas | Orquestación centralizada |
| Pérdida de contexto entre sesiones | Estado persistido y recuperable |
| Sin visibilidad del progreso global | Dashboard con métricas en tiempo real |
| Gestión manual de features cross-repo | Coordinación automática |

### Casos de Uso Principales

1. **Desarrollo paralelo**: Ejecutar Claude Code en múltiples repos simultáneamente
2. **Features cross-repo**: Coordinar cambios que afectan backend + frontend + móvil
3. **Migraciones**: Trackear progreso de migraciones de endpoints/componentes
4. **Debugging**: Visualizar output de múltiples procesos en grid 2x2
5. **Testing**: Ejecutar suites de tests en paralelo

---

## 2. Propuesta de Valor

### Para Desarrolladores Individuales

- **Productividad**: Gestionar múltiples tareas de Claude Code desde una UI
- **Contexto**: Ver el output de todas las sesiones sin cambiar de ventana
- **Control**: Pausar/reanudar sesiones según prioridad

### Para Equipos

- **Visibilidad**: Dashboard con progreso de todas las tareas activas
- **Coordinación**: Evitar conflictos entre desarrolladores/agentes
- **Tracking**: Historial de sesiones y costos acumulados

### Para Proyectos Complejos

- **Multi-repo**: Gestionar features que tocan múltiples repositorios
- **Persistencia**: Recuperar sesiones después de reinicios
- **Escalabilidad**: Añadir más instancias de Claude Code según necesidad

---

## 3. Arquitectura del Sistema

### Diagrama de Alto Nivel

```
┌────────────────────────────────────────────────────────────┐
│                           CODEPILOT UI (React)             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐    │
│  │Dashboard │  │Terminals │  │ Features │  │ Sessions │    │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘    │
│       │             │             │             │          │
│       └─────────────┴──────┬──────┴─────────────┘          │
│                            │                               │
│                    ┌───────▼───────┐                       │
│                    │ Zustand Store │  (Estado cliente)     │
│                    └───────┬───────┘                       │
└────────────────────────────┼───────────────────────────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
              ▼                             ▼
┌─────────────────────┐        ┌─────────────────────┐
│   REST API (:3001)  │        │  WebSocket (:8765)  │
│   (Express.js)      │        │  (Terminal I/O)     │
└──────────┬──────────┘        └──────────┬──────────┘
           │                              │
           │                              ▼
           │                   ┌─────────────────────┐
           │                   │    node-pty         │
           │                   │  (PowerShell/Bash)  │
           │                   └──────────┬──────────┘
           │                              │
           ▼                              ▼
┌─────────────────────┐        ┌─────────────────────┐
│  Workspace Files    │        │   Claude Code CLI   │
│  (YAML/JSON/MD)     │        │   (claude)          │
└─────────────────────┘        └─────────────────────┘
```

### Componentes Principales

| Componente | Tecnología | Responsabilidad |
|------------|------------|-----------------|
| **UI Frontend** | React 19 + TypeScript | Interfaz de usuario |
| **State Management** | Zustand + React Query | Estado cliente/servidor |
| **Terminal UI** | xterm.js | Renderizado de terminales |
| **REST API** | Express.js | CRUD de entidades |
| **WebSocket Server** | ws + node-pty | Comunicación bidireccional con terminales |
| **Persistencia** | YAML/JSON files | Estado del workspace |

### Flujo de Datos

```
1. Usuario crea sesión en UI
       ↓
2. UI envía request a REST API
       ↓
3. API registra sesión en estado
       ↓
4. UI conecta WebSocket para terminal
       ↓
5. WebSocket Server spawns node-pty (PowerShell/Bash)
       ↓
6. node-pty ejecuta `claude` CLI
       ↓
7. Output streamed via WebSocket → xterm.js
       ↓
8. Estado actualizado en Zustand
       ↓
9. UI re-renderiza con nuevo estado
```

---

## 4. Stack Tecnológico

### Frontend

| Dependencia | Versión | Propósito |
|-------------|---------|-----------|
| React | 19.x | Framework UI |
| TypeScript | 5.x | Tipado estático |
| Vite | 7.x | Build tool + dev server |
| TailwindCSS | 4.x | Estilos utility-first |
| shadcn/ui | Latest | Componentes UI |
| Radix UI | Latest | Primitivas accesibles |
| React Router | 7.x | Enrutamiento |
| React Query | 5.x | Estado servidor |
| Zustand | 5.x | Estado cliente |
| @xterm/xterm | 5.x | Terminal emulator |
| axios | 1.x | Cliente HTTP |
| zod | 4.x | Validación schemas |
| immer | 11.x | Inmutabilidad simplificada |
| uuid | 13.x | Generación IDs |
| sonner | 2.x | Toast notifications |

### Backend

| Dependencia | Propósito |
|-------------|-----------|
| Express.js | API REST |
| ws | WebSocket server |
| node-pty | Pseudo-terminales |
| js-yaml | Parsing YAML |
| cors | CORS middleware |
| helmet | Security headers |
| morgan | Request logging |

### DevTools

| Tool | Propósito |
|------|-----------|
| Vitest | Unit testing |
| Testing Library | Component testing |
| ESLint | Linting |
| concurrently | Parallel scripts |

---

## 5. Estructura del Proyecto

```
codepilot/
│
├── src/                           # Código fuente frontend
│   ├── components/
│   │   ├── ui/                    # shadcn/ui (NO EDITAR)
│   │   ├── layout/                # AppShell, Sidebar, Header
│   │   └── shared/                # LoadingSpinner, ErrorBoundary
│   │
│   ├── core/                      # Infraestructura compartida
│   │   ├── components/
│   │   └── data/
│   │       ├── apiClient.ts       # Axios configurado
│   │       ├── appStorage.ts      # LocalStorage wrapper
│   │       └── queryClient.ts     # React Query config
│   │
│   ├── features/                  # Módulos por feature
│   │   ├── auth/                  # Autenticación
│   │   ├── dashboard/             # Dashboard principal
│   │   ├── terminals/             # Sistema de terminales
│   │   │   ├── components/        # TerminalPanel, XTerminal
│   │   │   ├── hooks/             # useTerminal, useTerminalStream
│   │   │   ├── store/             # Zustand store
│   │   │   └── types/             # TypeScript types
│   │   ├── sessions/              # Gestión de sesiones
│   │   ├── features-board/        # Features cross-repo
│   │   └── tracking/              # Tracking de progreso
│   │
│   ├── stores/                    # Estado global Zustand
│   │   ├── sessions.store.ts
│   │   ├── features.store.ts
│   │   └── ui.store.ts
│   │
│   ├── services/                  # Servicios de negocio
│   │   └── terminal-websocket.service.ts
│   │
│   ├── pages/                     # Páginas (rutas)
│   │   ├── dashboard.page.tsx
│   │   ├── terminals.page.tsx
│   │   ├── features.page.tsx
│   │   └── settings.page.tsx
│   │
│   ├── hooks/                     # Custom hooks globales
│   ├── lib/                       # Utilities (cn, etc.)
│   ├── App.tsx
│   └── main.tsx
│
├── server/                        # Backend Node.js
│   ├── index.js                   # Express server (API)
│   ├── server.js                  # WebSocket server
│   ├── api/                       # REST endpoints
│   │   ├── index.js               # Router principal
│   │   ├── sessions.api.js
│   │   ├── features.api.js
│   │   ├── tasks.api.js
│   │   └── tracking.api.js
│   ├── services/                  # Lógica de negocio
│   └── utils/                     # Helpers
│
├── .env                           # Variables de entorno
├── package.json
├── vite.config.ts
├── tsconfig.json
└── tailwind.config.js
```

### Arquitectura Feature-Based

Cada feature sigue esta estructura:

```
features/{feature-name}/
├── components/          # Componentes React específicos
├── data/
│   ├── schemas/         # Zod validation schemas
│   └── types/           # TypeScript interfaces
├── hooks/
│   ├── queries/         # React Query hooks (GET)
│   └── mutations/       # React Query hooks (POST/PUT/DELETE)
├── services/            # Lógica de negocio
├── store/               # Zustand slice (si aplica)
└── index.ts             # Barrel exports
```

---

## 6. Sistema de Terminales

### Arquitectura de Terminales

```
┌─────────────────────────────────────────────────────────┐
│                    TERMINAL GRID (2x2)                  │
│  ┌─────────────────────┐    ┌─────────────────────┐     │
│  │   Terminal 0        │    │   Terminal 1        │     │
│  │   (Backend repo)    │    │   (Frontend repo)   │     │
│  │   claude --session  │    │   claude --session  │     │
│  └─────────────────────┘    └─────────────────────┘     │
│  ┌─────────────────────┐    ┌─────────────────────┐     │
│  │   Terminal 2        │    │   Terminal 3        │     │
│  │   (Mobile repo)     │    │   (Tests)           │     │
│  │   claude --session  │    │   npm run test      │     │
│  └─────────────────────┘    └─────────────────────┘     │
└─────────────────────────────────────────────────────────┘
              │                        │
              ▼                        ▼
┌─────────────────────────────────────────────────────────┐
│                    WebSocket Server (:8765)             │
│                                                         │
│   Sessions Map: { sessionId → PTY Process }             │
│                                                         │
│   ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐    │
│   │ PTY #0  │  │ PTY #1  │  │ PTY #2  │  │ PTY #3  │    │
│   │ pwsh    │  │ pwsh    │  │ pwsh    │  │ pwsh    │    │
│   └─────────┘  └─────────┘  └─────────┘  └─────────┘    │
└─────────────────────────────────────────────────────────┘
```

### Tipos de Sesión Terminal

```typescript
interface TerminalSession {
  // Identificación
  id: string;                    // UUID único
  title: string;                 // Nombre visible

  // Proceso
  cwd: string;                   // Directorio de trabajo
  command: string;               // Comando (ej: "claude")
  args: string[];                // Argumentos

  // Estado
  status: 'idle' | 'running' | 'completed' | 'error';
  exitCode: number | null;
  createdAt: Date;

  // Conexión
  connectionStatus: 'idle' | 'connecting' | 'active' | 'disconnected' | 'reconnecting' | 'error';
  reconnectAttempts: number;
  isServerSessionCreated: boolean;

  // Grid multi-terminal
  gridPosition?: number;         // 0=top-left, 1=top-right, 2=bottom-left, 3=bottom-right

  // Contexto
  repoContext?: string;          // Identificador del repo
  taskId?: string;               // Tarea asociada
  taskDescription?: string;

  // Persistencia
  type?: 'task' | 'terminal' | 'test' | 'script';
  outputBuffer?: string[];       // Buffer para recuperación
  isPersisted?: boolean;
}
```

### Protocolo WebSocket

#### Cliente → Servidor

| Tipo | Payload | Descripción |
|------|---------|-------------|
| `create-session` | `{cwd, command, args, title, sessionId?}` | Crear PTY |
| `input` | `{sessionId, data}` | Enviar input al terminal |
| `resize` | `{sessionId, cols, rows}` | Redimensionar terminal |
| `kill-session` | `{sessionId}` | Terminar proceso |

#### Servidor → Cliente

| Tipo | Payload | Descripción |
|------|---------|-------------|
| `session-created` | `{sessionId, cwd, command, title, shell}` | Sesión lista |
| `output` | `{sessionId, data}` | Output del proceso |
| `exit` | `{sessionId, exitCode}` | Proceso terminado |
| `error` | `{message}` | Error |

### Shells Soportados

| Plataforma | Shells |
|------------|--------|
| Windows | PowerShell.exe, pwsh.exe, cmd.exe |
| Linux/Mac | /bin/bash, /bin/zsh, /bin/sh |

### Componente XTerminal

```typescript
interface XTerminalProps {
  sessionId: string;
  onReady?: () => void;
  onData?: (data: string) => void;
  onResize?: (cols: number, rows: number) => void;
  className?: string;
}

// Uso
<XTerminal
  sessionId="abc-123"
  onReady={() => console.log('Terminal ready')}
/>
```

---

## 7. Gestión de Sesiones Claude

### Modelo de Datos

```typescript
interface ClaudeSession {
  id: string;

  // Configuración
  model: 'claude-sonnet-4-5-20250929' | 'claude-opus-4-5-20251101';
  maxTurns: number;
  workingDirectory: string;

  // Estado
  status: 'pending' | 'running' | 'paused' | 'completed' | 'failed' | 'cancelled';

  // Tracking
  turnsUsed: number;
  tokensUsed: {
    input: number;
    output: number;
  };
  estimatedCost: number;

  // Timestamps
  createdAt: Date;
  startedAt?: Date;
  completedAt?: Date;

  // Relaciones
  featureId?: string;
  taskId?: string;
  terminalSessionId?: string;

  // Persistencia
  resumeSessionId?: string;  // Para reanudar
  lastCheckpoint?: string;
}
```

### Acciones de Sesión

| Acción | Descripción | Endpoint |
|--------|-------------|----------|
| **Create** | Crear nueva sesión | `POST /api/sessions` |
| **Start** | Iniciar sesión pendiente | `POST /api/sessions/:id/start` |
| **Pause** | Pausar sesión activa | `POST /api/sessions/:id/pause` |
| **Resume** | Reanudar sesión pausada | `POST /api/sessions/:id/resume` |
| **Cancel** | Cancelar sesión | `POST /api/sessions/:id/cancel` |
| **Get** | Obtener estado | `GET /api/sessions/:id` |
| **List** | Listar sesiones | `GET /api/sessions` |

### Ciclo de Vida

```
                    ┌─────────┐
                    │ pending │
                    └────┬────┘
                         │ start
                         ▼
              ┌──────────────────┐
              │     running      │◄──────┐
              └────────┬─────────┘       │
                       │                 │ resume
          ┌────────────┼─────────────┐   │
          │            │             │   │
          ▼            ▼             ▼   │
    ┌──────────┐ ┌──────────┐  ┌─────────┴┐
    │completed │ │  failed  │  │  paused  │
    └──────────┘ └──────────┘  └──────────┘

          │            │             │
          └────────────┼─────────────┘
                       ▼
                 ┌───────────┐
                 │ cancelled │
                 └───────────┘
```

---

## 8. API REST Backend

### Endpoints Principales

#### Health & Info

```
GET /health
GET /api/
```

#### Sesiones

```
GET    /api/sessions              # Listar todas
GET    /api/sessions/:id          # Detalle
POST   /api/sessions              # Crear
POST   /api/sessions/:id/start    # Iniciar
POST   /api/sessions/:id/pause    # Pausar
POST   /api/sessions/:id/resume   # Reanudar
POST   /api/sessions/:id/cancel   # Cancelar
DELETE /api/sessions/:id          # Eliminar
```

#### Features (desarrollo cross-repo)

```
GET    /api/features              # Listar features
GET    /api/features/stats        # Estadísticas
GET    /api/features/:id          # Detalle
GET    /api/features/:id/sessions # Sesiones de feature
POST   /api/features/:id/launch   # Lanzar desarrollo
POST   /api/features/:id/cancel   # Cancelar todas
```

#### Tareas

```
GET    /api/tasks                 # Listar tareas
GET    /api/tasks/:id             # Detalle
POST   /api/tasks/:id/update      # Actualizar estado
```

#### Tracking

```
GET    /api/tracking/progress     # Progreso global
GET    /api/tracking/:entity      # Tracking específico
```

### Estructura de Respuesta

```typescript
// Éxito
interface ApiResponse<T> {
  success: true;
  data: T;
  meta?: {
    total?: number;
    page?: number;
    limit?: number;
  };
}

// Error
interface ApiError {
  success: false;
  error: {
    code: string;
    message: string;
    details?: unknown;
  };
}
```

---

## 9. Gestión de Estado

### Zustand Stores

#### Terminal Sessions Store

```typescript
// src/features/terminals/store/terminal-sessions.store.ts

interface TerminalSessionsState {
  // Estado
  sessions: Map<string, TerminalSession>;
  activeSessionId: string | null;
  layout: PanelLayout;
  config: MultiTerminalConfig;

  // Getters
  getSession: (id: string) => TerminalSession | undefined;
  getGridSessions: () => TerminalSession[];
  getRunningSessionsCount: () => number;
  canAddSession: () => boolean;
  getNextGridPosition: () => number;

  // Actions
  addSession: (session: TerminalSession) => void;
  updateSession: (id: string, updates: Partial<TerminalSession>) => void;
  removeSession: (id: string) => void;
  setActiveSession: (id: string | null) => void;
  createSessionWithPosition: (data: CreateSessionData) => string | null;
  moveSessionToPosition: (sessionId: string, newPosition: number) => void;

  // Persistencia
  hydrateFromStorage: () => void;
  persistToStorage: () => void;
}

const useTerminalSessionsStore = create<TerminalSessionsState>()(
  persist(
    (set, get) => ({
      // Implementación...
    }),
    {
      name: 'terminal-sessions',
      partialize: (state) => ({
        layout: state.layout,
        config: state.config,
      }),
    }
  )
);
```

#### Sessions Store (Claude)

```typescript
// src/stores/sessions.store.ts

interface SessionsState {
  sessions: ClaudeSession[];
  activeSessions: string[];

  // Actions
  addSession: (session: ClaudeSession) => void;
  updateSession: (id: string, updates: Partial<ClaudeSession>) => void;
  removeSession: (id: string) => void;

  // Getters
  getSessionsByFeature: (featureId: string) => ClaudeSession[];
  getRunningCount: () => number;
  getTotalCost: () => number;
}
```

### React Query - Convenciones

```typescript
// Query Keys
export const sessionKeys = {
  all: ['sessions'] as const,
  lists: () => [...sessionKeys.all, 'list'] as const,
  list: (filters: Filters) => [...sessionKeys.lists(), filters] as const,
  details: () => [...sessionKeys.all, 'detail'] as const,
  detail: (id: string) => [...sessionKeys.details(), id] as const,
};

// Hook de Query
export function useSessions(filters?: Filters) {
  return useQuery({
    queryKey: sessionKeys.list(filters ?? {}),
    queryFn: () => sessionsApi.getAll(filters),
    staleTime: 30_000,
  });
}

// Hook de Mutation
export function useCreateSession() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: sessionsApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: sessionKeys.lists() });
    },
  });
}
```

---

## 10. Integraciones

### Claude Code CLI

CodePilot interactúa con Claude Code a través del CLI:

```bash
# Crear sesión nueva
claude --session-id "unique-id" --cwd "/path/to/repo"

# Reanudar sesión existente
claude --resume "session-id"

# Con modelo específico
claude --model claude-sonnet-4-5-20250929

# Con límite de turnos
claude --max-turns 50
```

### Workspace Persistence

CodePilot lee y escribe en el workspace del proyecto:

```
{workspace}/
├── .codepilot/                   # Datos de CodePilot
│   ├── sessions/                 # Sesiones guardadas
│   │   ├── session-{id}.json
│   │   └── ...
│   ├── features/                 # Features activas
│   │   └── feature-{id}/
│   │       ├── spec.md
│   │       └── sessions.json
│   └── config.json               # Configuración local
│
├── tracking/                     # Estado de tareas (YAML)
│   ├── tasks.yaml
│   └── progress.yaml
│
└── tasks/                        # Tareas en Markdown
    └── backlog.md
```

### Variables de Entorno

```env
# Workspace
VITE_WORKSPACE_ROOT=/path/to/workspace

# Claude
VITE_CLAUDE_DEFAULT_MODEL=claude-sonnet-4-5-20250929
VITE_CLAUDE_MAX_TURNS=50

# Servers
VITE_API_URL=http://localhost:3001
VITE_WS_URL=ws://localhost:8765

# Opcional
VITE_ARGOCD_URL=https://argocd.example.com
VITE_ARGOCD_TOKEN=
```

---

## 11. Flujo de Trabajo

### Flujo Típico: Desarrollar Feature

```
1. Usuario abre CodePilot
       │
       ▼
2. Navega a Features → Nueva Feature
       │
       ▼
3. Define feature:
   - Nombre: "Implementar página de campanias"
   - Repos involucrados: [api, web]
   - Tareas:
     - WPR-010: Endpoint GET /campanias
     - WPR-011: Componente CampaniaList
       │
       ▼
4. Click "Lanzar desarrollo"
       │
       ▼
5. CodePilot crea 2 terminales:
   - Terminal 1: cd api && claude --session-id "wpr-010"
   - Terminal 2: cd web && claude --session-id "wpr-011"
       │
       ▼
6. Usuario monitorea en grid 2x2
       │
       ▼
7. Claude Code trabaja en cada repo
       │
       ▼
8. Usuario interviene si necesario (input en terminal)
       │
       ▼
9. Al completar, progreso actualizado en tracking/
```

### Flujo: Retomar Trabajo

```
1. Usuario abre CodePilot
       │
       ▼
2. Ve sesiones pausadas/anteriores en Dashboard
       │
       ▼
3. Click "Reanudar" en sesión específica
       │
       ▼
4. CodePilot ejecuta: claude --resume "session-id"
       │
       ▼
5. Claude Code continúa desde checkpoint
```

---

## 12. Configuración

### Scripts de package.json

```json
{
  "scripts": {
    "dev": "concurrently \"npm run dev:client\" \"npm run dev:server\"",
    "dev:client": "vite",
    "dev:server": "concurrently \"node server/index.js\" \"node server/server.js\"",
    "build": "tsc && vite build",
    "test": "vitest",
    "test:coverage": "vitest run --coverage"
  }
}
```

### Puertos

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| Vite (dev) | 5173 | Frontend React |
| API REST | 3001 | Express server |
| WebSocket | 8765 | Terminal I/O |

### Path Aliases (tsconfig)

```json
{
  "compilerOptions": {
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  }
}
```

---

## 13. Deployment

### Desarrollo Local

```bash
# 1. Instalar dependencias
npm install

# 2. Copiar variables de entorno
cp .env.example .env

# 3. Configurar workspace root en .env
VITE_WORKSPACE_ROOT=/path/to/your/project

# 4. Iniciar servicios
npm run dev

# Acceder a http://localhost:5173
```

### Producción (Azure)

```
┌───────────────────────────────────────────┐
│              Azure App Service            │
│  ┌───────────────┐    ┌───────────────┐   │
│  │ API + WS      │    │ Static Web    │   │
│  │ (Node.js)     │    │ (React build) │   │
│  └───────────────┘    └───────────────┘   │
└───────────────────────────────────────────┘
```

### Docker (Opcional)

```dockerfile
# Dockerfile
FROM node:20-slim

WORKDIR /app

# Instalar dependencias
COPY package*.json ./
RUN npm ci

# Copiar código
COPY . .

# Build frontend
RUN npm run build

# Exponer puertos
EXPOSE 3001 8765

# Iniciar
CMD ["npm", "run", "start:prod"]
```

---

## 14. Adaptación a WePlay Rises

### Estructura Workspace Sugerida

```
C:\Repos\WePlay_Rises\
│
├── .codepilot/                   # Datos de CodePilot
│   ├── sessions/
│   ├── features/
│   └── config.json
│
├── tracking/                     # Estado de desarrollo
│   ├── tasks.yaml               # Tareas estructuradas
│   ├── endpoints.yaml           # Estado de endpoints API
│   └── components.yaml          # Estado de componentes
│
├── tasks/                        # Tareas en Markdown
│   ├── backlog.md               # Backlog completo
│   ├── api.md                   # Tareas de API
│   ├── web.md                   # Tareas de Landing
│   └── admin.md                 # Tareas de Dashboard
│
├── src/
│   ├── api/                     # Backend .NET
│   ├── web/                     # Landing React
│   ├── admin/                   # Dashboard Next.js
│   └── shared/                  # Código compartido
│
├── codepilot/                   # CodePilot (opcional, puede ser externo)
│   ├── src/
│   └── server/
│
└── README.md
```

### Features Cross-Repo para WePlay Rises

| Feature | Repos | Tareas |
|---------|-------|--------|
| Auth System | api, web, admin | WPR-006, WPR-007, WPR-008 |
| Campanias CRUD | api, admin | WPR-010, WPR-011 |
| Rewards System | api, admin | WPR-012, WPR-013 |
| Backing Flow | api, web | WPR-014, WPR-015 |
| Dashboard Artista | api, admin | WPR-016, WPR-017 |

### Mapeo de Tareas

```yaml
# tracking/tasks.yaml
tasks:
  - id: WPR-006
    title: Implementar Identity minimo (JWT)
    repo: api
    status: pending
    feature: auth-system

  - id: WPR-007
    title: Componente LoginForm
    repo: web
    status: pending
    feature: auth-system
    blocked_by: [WPR-006]

  - id: WPR-010
    title: Endpoint GET /campanias
    repo: api
    status: pending
    feature: campanias-crud
```

### Comandos Claude Recomendados

Para trabajar con WePlay Rises desde CodePilot:

```bash
# Backend - Nuevo endpoint
claude --cwd "src/api" --prompt "Implementar GET /api/campanias con paginación"

# Landing - Nuevo componente
claude --cwd "src/web" --prompt "Crear CampaniaCard component con shadcn/ui"

# Dashboard - Nueva página
claude --cwd "src/admin" --prompt "Crear página /dashboard/campanias con tabla"
```

---

## Próximos Pasos

### Fase 1: Setup Básico
- [ ] Crear estructura `.codepilot/` en WePlay_Rises
- [ ] Configurar CodePilot con WORKSPACE_ROOT
- [ ] Migrar tareas a formato tracking/tasks.yaml

### Fase 2: Desarrollo con CodePilot
- [ ] Definir features cross-repo
- [ ] Ejecutar desarrollo paralelo (API + Frontend)
- [ ] Trackear progreso en tiempo real

### Fase 3: Optimización
- [ ] Persistir sesiones para reanudar
- [ ] Dashboard de métricas
- [ ] Integración con Azure DevOps

---

## Referencias

- [Claude Code Documentation](https://docs.anthropic.com/claude-code)
- [xterm.js](https://xtermjs.org/)
- [shadcn/ui](https://ui.shadcn.com/)
- [Zustand](https://zustand-demo.pmnd.rs/)
- [React Query](https://tanstack.com/query)
