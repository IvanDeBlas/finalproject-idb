# Implementación: Registro de Artista

> **Feature:** registro-artista
> **Última actualización:** 2026-01-26

---

## Orden de Implementación

```
1. Shared     → Types, schemas, constantes (base para todos)
2. Backend    → Endpoints, Identity, JWT (bloquea a Frontend)
3. Landing    → Form registro, perfil público (paralelo con Admin)
4. Admin      → Form perfil, dashboard (paralelo con Landing)
```

**Diagrama de dependencias:**

```
┌─────────┐
│ Shared  │ ─────────────────────────────────────┐
└────┬────┘                                      │
     │                                           │
     ▼                                           │
┌─────────┐                                      │
│ Backend │ ───────────────┬─────────────────────┤
└────┬────┘                │                     │
     │                     │                     │
     │    ┌────────────────┴────────────────┐    │
     │    │                                 │    │
     ▼    ▼                                 ▼    ▼
┌─────────┐                            ┌─────────┐
│ Landing │                            │  Admin  │
└─────────┘                            └─────────┘
```

---

## 1. Shared (TypeScript)

**Tarea relacionada:** Parte de WPR-005

### Archivos a crear

| Ruta | Descripción |
|------|-------------|
| `src/shared/types/auth.ts` | Types de autenticación |
| `src/shared/types/artista.ts` | Types de artista |
| `src/shared/types/api.ts` | ServiceResponse genérico |
| `src/shared/schemas/auth.schema.ts` | Zod schemas auth |
| `src/shared/schemas/artista.schema.ts` | Zod schemas artista |
| `src/shared/constants/query-keys.ts` | React Query keys |
| `src/shared/constants/api-routes.ts` | Rutas de API |
| `src/shared/constants/routes.ts` | Rutas de navegación |
| `src/shared/utils/error-messages.ts` | Mapeo errores → UI |

### Código a implementar

Ver sección "DTOs / Types" y "Validaciones Compartidas" en [contracts.md](./contracts.md).

### Validación

```bash
cd src/shared
npx tsc --noEmit  # Verificar que compila sin errores
```

---

## 2. Backend (.NET)

**Tareas relacionadas:** WPR-006 (Identity), WPR-010 (Artista)

### Archivos a crear

#### WPR-006: Identity + JWT

| Ruta | Descripción |
|------|-------------|
| `WebApi/Controllers/AuthController.cs` | Controller de auth |
| `WebApi/Configuration/JwtSettings.cs` | Config JWT |
| `WebApi/Configuration/IdentityConfiguration.cs` | Setup Identity |
| `Modules/UserAccess/Application/Features/Auth/Commands/RegisterCommand.cs` | Command + Handler |
| `Modules/UserAccess/Application/Features/Auth/Commands/LoginCommand.cs` | Command + Handler |
| `Modules/UserAccess/Application/Features/Auth/Validators/RegisterValidator.cs` | Validador |
| `Modules/UserAccess/Application/Features/Auth/Validators/LoginValidator.cs` | Validador |
| `Modules/UserAccess/Application/Dtos/AuthResultDto.cs` | DTO respuesta |
| `Modules/UserAccess/Application/Mapping/AuthProfile.cs` | AutoMapper |

#### WPR-010: Artista

| Ruta | Descripción |
|------|-------------|
| `WebApi/Controllers/ArtistasController.cs` | Controller |
| `Modules/UserAccess/Domain/Model/Artista.cs` | Entidad |
| `Modules/UserAccess/Application/Features/Artistas/Commands/CreateArtistaCommand.cs` | Command + Handler |
| `Modules/UserAccess/Application/Features/Artistas/Queries/GetArtistaByIdQuery.cs` | Query + Handler |
| `Modules/UserAccess/Application/Features/Artistas/Queries/GetArtistaByUserIdQuery.cs` | Query + Handler |
| `Modules/UserAccess/Application/Features/Artistas/Validators/CreateArtistaValidator.cs` | Validador |
| `Modules/UserAccess/Application/Dtos/ArtistaDto.cs` | DTOs |
| `Modules/UserAccess/Application/Mapping/ArtistaProfile.cs` | AutoMapper |
| `Modules/UserAccess/Application/Interfaces/Services/IArtistaService.cs` | Interface |
| `Modules/UserAccess/Infra/Services/ArtistaService.cs` | Service |
| `Modules/UserAccess/Infra/Repositories/ArtistaRepository.cs` | Repository |
| `Modules/UserAccess/Infra/Persistence/Configurations/ArtistaConfiguration.cs` | EF Config |

### Comandos

```bash
cd src/api

# Build
dotnet build

# Crear migración
dotnet ef migrations add AddArtista --project WebApi -c WePlayRisesDbContext

# Aplicar migración
dotnet ef database update --project WebApi

# Ejecutar
dotnet run --project WebApi

# Tests
dotnet test
```

### Validación

1. Swagger disponible en `https://localhost:5001/swagger`
2. Probar endpoints:
   - `POST /api/auth/register` → 201 con token
   - `POST /api/auth/login` → 200 con token
   - `POST /api/artistas` (con Bearer) → 201 con artista
   - `GET /api/artistas/me` (con Bearer) → 200 con artista
   - `GET /api/artistas/{id}` → 200 con perfil público

---

## 3. Landing (Vite + React)

**Tarea relacionada:** WPR-010 (parte frontend)

### Archivos a crear

| Ruta | Descripción |
|------|-------------|
| `src/features/auth/pages/RegisterPage.tsx` | Página de registro |
| `src/features/auth/components/RegisterForm.tsx` | Formulario |
| `src/features/auth/hooks/useRegister.ts` | Mutation hook |
| `src/features/auth/hooks/useLogin.ts` | Mutation hook |
| `src/features/auth/services/auth.service.ts` | API calls |
| `src/features/artistas/pages/ArtistaPublicoPage.tsx` | Perfil público |
| `src/features/artistas/hooks/useArtista.ts` | Query hook |
| `src/features/artistas/services/artista.service.ts` | API calls |
| `src/store/auth.store.ts` | Zustand store para auth |
| `src/components/layout/AuthGuard.tsx` | Protección de rutas |

### Páginas y rutas

| Ruta | Componente | Auth |
|------|------------|------|
| `/auth/register` | `RegisterPage` | ❌ |
| `/auth/login` | `LoginPage` | ❌ |
| `/artistas/:id` | `ArtistaPublicoPage` | ❌ |

### Flujo de UI

```
/auth/register
    │
    ├── Form: email, password, confirmPassword
    ├── Validación: Zod schema
    ├── Submit: POST /api/auth/register
    ├── Success: Guardar token → redirect a Admin /artista/perfil/crear
    └── Error: Mostrar mensaje mapeado
```

### Comandos

```bash
cd src/web

# Desarrollo
npm run dev

# Build
npm run build

# Lint
npm run lint
```

### Validación

1. Navegar a `http://localhost:5173/auth/register`
2. Completar formulario con datos válidos
3. Verificar redirect a `/artista/perfil/crear` (en Admin)
4. Navegar a `/artistas/{id}` y ver perfil público

---

## 4. Admin (Next.js)

**Tarea relacionada:** WPR-010 (parte frontend)

### Archivos a crear

| Ruta | Descripción |
|------|-------------|
| `src/app/(auth)/login/page.tsx` | Página login |
| `src/app/(dashboard)/artista/perfil/crear/page.tsx` | Crear perfil |
| `src/app/(dashboard)/dashboard/page.tsx` | Dashboard inicial |
| `src/components/forms/ArtistaForm.tsx` | Formulario perfil |
| `src/hooks/useArtistaMe.ts` | Query hook |
| `src/hooks/useCreateArtista.ts` | Mutation hook |
| `src/services/artista.service.ts` | API calls |
| `src/providers/AuthProvider.tsx` | Context de auth |
| `src/middleware.ts` | Protección de rutas |

### Páginas y rutas

| Ruta | Componente | Auth |
|------|------------|------|
| `/login` | `LoginPage` | ❌ |
| `/artista/perfil/crear` | `CrearPerfilPage` | ✅ |
| `/dashboard` | `DashboardPage` | ✅ |

### Flujo de UI

```
/artista/perfil/crear
    │
    ├── Verificar: ¿Tiene token? Si no → /login
    ├── Verificar: ¿Ya tiene perfil? Si sí → /dashboard
    ├── Form: nombreArtistico, descripcion, pais, ciudad, imagenUrl
    ├── Validación: Zod schema
    ├── Submit: POST /api/artistas
    ├── Success: redirect a /dashboard
    └── Error: Mostrar mensaje mapeado
```

### Comandos

```bash
cd src/admin

# Desarrollo
npm run dev

# Build
npm run build

# Lint
npm run lint
```

### Validación

1. Acceder a `http://localhost:3000/artista/perfil/crear` con token
2. Completar formulario de perfil
3. Verificar redirect a `/dashboard`
4. Dashboard muestra datos del artista

---

## Checklist de Implementación

### Shared
- [ ] Types creados (`auth.ts`, `artista.ts`)
- [ ] Schemas Zod creados
- [ ] Constantes creadas
- [ ] Compila sin errores

### Backend
- [ ] Identity configurado
- [ ] JWT funcionando
- [ ] `POST /api/auth/register` → 201
- [ ] `POST /api/auth/login` → 200
- [ ] `POST /api/artistas` → 201 (con auth)
- [ ] `GET /api/artistas/me` → 200 (con auth)
- [ ] `GET /api/artistas/{id}` → 200 (público)
- [ ] Validaciones retornan errores correctos
- [ ] Migraciones aplicadas

### Landing
- [ ] Página `/auth/register` funciona
- [ ] Validación en frontend
- [ ] Registro exitoso guarda token
- [ ] Redirect a Admin funciona
- [ ] Página `/artistas/{id}` muestra perfil

### Admin
- [ ] Middleware protege rutas
- [ ] Página `/artista/perfil/crear` funciona
- [ ] Crear perfil exitoso → redirect dashboard
- [ ] Dashboard muestra datos del artista

### Integración E2E
- [ ] Flujo completo: Register → Crear Perfil → Dashboard
- [ ] Perfil visible públicamente en Landing

---

## Notas de Implementación

### Decisiones técnicas

1. **JWT en localStorage**: Para MVP. En producción considerar httpOnly cookies.
2. **Redirect entre apps**: Landing redirige a Admin con token en URL param, Admin lo guarda.
3. **Perfil opcional**: Usuario puede saltar creación de perfil, se le pide después en dashboard.

### Deuda técnica identificada

- [ ] Refresh token (no implementado en MVP)
- [ ] Logout global (no implementado en MVP)
- [ ] Validación de imagen URL (solo formato, no que exista)

---

*Última actualización: 2026-01-26*
