# Plan de Implementación: Registro Artista

**Feature:** registro-artista
**Fecha de generación:** 2026-02-12
**Generado por:** /plan registro-artista

---

## Resumen Ejecutivo

| Métrica | Valor |
|---------|-------|
| Total archivos generados | 14 |
| Shared | 1 archivo |
| Backend | 5 archivos |
| Frontend Landing | 4 archivos |
| Frontend Admin | 4 archivos |
| Score QA Landing | 100% APROBADO |
| Score QA Admin | 100% APROBADO |

---

## Estructura de Archivos

```
plans/registro-artista/
├── README.md                              # Este archivo
├── shared/
│   └── contracts-plan.md                  # Types, Schemas Zod, Constantes
├── backend/
│   ├── api-contracts.md                   # DTOs, Validators, Controllers
│   ├── hexagonal-architecture.md          # Domain + Infrastructure
│   ├── cqrs-plan.md                       # Commands, Queries, Handlers
│   ├── newman-tests.md                    # Estrategia tests integración
│   └── postman-collection.json            # Colección ejecutable Newman
├── frontend-landing/
│   ├── frontend-plan.md                   # Arquitectura y componentes
│   ├── ui-design.md                       # Componentes shadcn/ui
│   ├── test-strategy.md                   # Tests Vitest + Testing Library
│   └── qa-validation.md                   # Validación criterios (100%)
└── frontend-admin/
    ├── frontend-plan.md                   # Arquitectura y componentes
    ├── ui-design.md                       # Componentes shadcn/ui
    ├── test-strategy.md                   # Tests Vitest + Testing Library
    └── qa-validation.md                   # Validación criterios (100%)
```

---

## Resumen por Capa

### Shared (1 archivo)

**contracts-plan.md** - Contratos TypeScript compartidos entre Landing y Admin

| Tipo | Cantidad |
|------|----------|
| Types/Interfaces | 5 (Artista, ArtistaListItem, RegisterRequest, RegisterResponse, CreateArtistaRequest) |
| Zod Schemas | 2 (registerSchema, createArtistaSchema) |
| Constantes | 3 archivos (QUERY_KEYS, API_ROUTES, APP_ROUTES) |
| Utils | 1 (ERROR_MESSAGES + getErrorMessage) |

### Backend (5 archivos)

**api-contracts.md** - Capa de presentación
- 4 endpoints diseñados (POST register, POST artista, GET by id, GET by userId)
- DTOs request/response completos
- FluentValidation con .WithMessage() Y .WithErrorCode()
- Controllers con [Authorize] y OpenAPI docs

**hexagonal-architecture.md** - Domain + Infrastructure
- Entidad Artista (ya existe, requiere índice UNIQUE)
- IArtistaRepository interface
- ArtistaRepository implementación
- IArtistaService + ArtistaService con caching (ADR-006)

**cqrs-plan.md** - Application layer
- RegisterCommand + Handler (Identity + JWT)
- CreateArtistaCommand + Handler
- GetArtistaByIdQuery + Handler
- GetArtistaByUserIdQuery + Handler
- 2 Validators completos
- 2 AutoMapper Profiles

**newman-tests.md** - Tests de integración
- 18 requests organizados por endpoint y status code
- 2 flujos E2E completos
- Assertions para ServiceResponse<T>

**postman-collection.json** - Colección ejecutable
- 34 requests listos para Newman
- Pre-request scripts y tests incluidos
- Variables de colección configuradas

### Frontend Landing (4 archivos)

**frontend-plan.md** - Arquitectura
- 7 componentes (ArtistaProfilePage, HeroBanner, Avatar, Bio, Stats, SocialLinks, Campaigns)
- 1 hook (useArtista)
- Ruta pública /artistas/:id

**ui-design.md** - Diseño UI
- Componentes shadcn/ui: Card, Avatar, Badge, Button, Skeleton
- Dark theme con design tokens
- Responsive breakpoints
- Estados: loading, error 404, empty states

**test-strategy.md** - Testing
- 13 tests planificados
- Cobertura objetivo: 80%+
- MSW para mocks de API

**qa-validation.md** - QA Score: 100%
- AC-01-8 cubierto completamente
- 0 gaps críticos

### Frontend Admin (4 archivos)

**frontend-plan.md** - Arquitectura
- Pages: /auth/register, /artista/perfil/crear
- 5 componentes (RegisterForm, PasswordInput, CreateArtistaForm, ImagePreview, CharacterCounter)
- 4 hooks (useRegister, useCreateArtista, useArtistaByUserId, useImagePreview)
- AuthProvider con Zustand + localStorage

**ui-design.md** - Diseño UI
- Componentes shadcn/ui: Card, Input, Textarea, Button, Label, Avatar, Form
- Gradient buttons: from-pink-500 to-purple-600
- 7 estados UI documentados
- Accesibilidad WCAG AA

**test-strategy.md** - Testing
- 16 tests planificados
- Cobertura objetivo: 80%+
- MSW handlers para auth y artistas

**qa-validation.md** - QA Score: 100%
- 6 criterios cubiertos (AC-01-2, 3, 4, 5, 9, 10)
- 0 gaps críticos

---

## Criterios de Aceptación Cubiertos

| ID | Criterio | Proyecto | Estado |
|----|----------|----------|--------|
| AC-01-1 | Email único en Identity | Backend | Planificado |
| AC-01-2 | Password mín 8 caracteres | Backend + Admin | Planificado |
| AC-01-3 | Passwords coinciden | Admin | Planificado |
| AC-01-4 | Nombre artístico obligatorio | Admin + Backend | Planificado |
| AC-01-5 | Imagen URL opcional/válida | Admin + Backend | Planificado |
| AC-01-6 | Token JWT válido tras registro | Backend | Planificado |
| AC-01-7 | Artista vinculado a UserId | Backend | Planificado |
| AC-01-8 | Perfil público sin auth | Landing + Backend | Planificado |
| AC-01-9 | Dashboard requiere perfil | Admin + Backend | Planificado |
| AC-01-10 | Schemas Zod en shared | Shared + Admin | Planificado |

---

## Dependencias Técnicas

### Requeridas ANTES de implementar

1. **ASP.NET Core Identity** configurado en WebApi
2. **JWT Authentication** implementado
3. **DbContext** con entidad User y Artista
4. **React Query** configurado en Admin y Landing
5. **Axios interceptors** para tokens JWT

### Entre proyectos

```
Shared ──────┬──> Backend (types reference)
             ├──> Landing (imports)
             └──> Admin (imports)

Backend ─────┬──> Landing (API endpoints)
             └──> Admin (API endpoints)
```

---

## Siguiente Paso

Implementar usando el comando:

```bash
/implement registro-artista --target all
```

O implementar por partes en orden:

```bash
# 1. Shared primero (bloqueante)
/implement registro-artista --target shared

# 2. Backend (requiere shared)
/implement registro-artista --target backend

# 3. Frontends en paralelo
/implement registro-artista --target landing
/implement registro-artista --target admin
```

---

## Comandos Newman

Ejecutar tests de integración:

```bash
# Instalar Newman
npm install -g newman newman-reporter-htmlextra

# Ejecutar colección completa
newman run plans/registro-artista/backend/postman-collection.json \
    -e tests/newman/environments/development.json \
    --reporters cli,htmlextra

# Solo Auth tests
newman run plans/registro-artista/backend/postman-collection.json \
    --folder "Auth"

# Solo E2E flows
newman run plans/registro-artista/backend/postman-collection.json \
    --folder "_E2E Flows"
```

---

## Notas

- **Stats y Campañas** en Landing son placeholders (se implementan en feature crear-campania)
- **Social Links** son placeholders (campos no existen en modelo Artista MVP)
- **Géneros musicales** hardcodeados (agregar campo en DB en futuro)
- El campo **imagenUrl** no existe en entidad Artista actual - verificar con backend

---

*Generado automáticamente por /plan registro-artista*
