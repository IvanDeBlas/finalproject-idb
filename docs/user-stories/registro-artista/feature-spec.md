# Feature: Registro de Artista

> **ID:** registro-artista
> **User Story:** US-01
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripción

Esta feature permite a un artista crear su cuenta y perfil en la plataforma WePlay Rises. El flujo se divide en dos fases secuenciales: primero el registro de credenciales de acceso (email/password) mediante ASP.NET Core Identity, y segundo la creación del perfil artístico con información pública como nombre artístico, descripción, ubicación e imagen.

El registro de artista es el punto de entrada fundamental al ecosistema de WePlay Rises, permitiendo que músicos y creadores puedan posteriormente crear campañas de crowdfunding para sus proyectos. Una vez completado el registro y perfil, el artista accede al dashboard administrativo donde gestiona sus campañas.

Esta funcionalidad establece la base para todo el flujo de creación de campañas y es dependencia crítica para US-02 (Crear Campaña). El perfil creado será visible públicamente en la landing page, permitiendo a los fans conocer al artista antes de apoyar sus proyectos.

---

## User Story

**Como** artista
**Quiero** registrarme y crear mi perfil
**Para** presentar mi proyecto musical en la plataforma

---

## Flujo Principal

1. Artista accede a la página de registro `/auth/register` desde la landing page
2. Artista completa el formulario de registro con email, password y confirmación de password
3. Sistema valida credenciales en frontend (formato email, longitud password, coincidencia passwords)
4. Sistema envía solicitud a backend `/api/auth/register`
5. Backend valida credenciales y crea usuario en ASP.NET Core Identity
6. Backend retorna token JWT de autenticación
7. Sistema redirige a artista a `/artista/perfil/crear` con sesión iniciada
8. Artista completa formulario de perfil con nombre artístico (obligatorio), descripción, país, ciudad e imagen URL
9. Sistema valida datos del perfil en frontend usando schema Zod
10. Sistema envía solicitud a backend `/api/artistas`
11. Backend crea entidad Artista vinculada al UserId del Identity
12. Sistema redirige a artista a `/dashboard` con mensaje de bienvenida

---

## Flujos Alternativos

| ID | Condición | Acción |
|----|-----------|--------|
| FA-01 | Email ya registrado en Identity | Backend retorna error 400, frontend muestra mensaje "Email ya existe. ¿Deseas iniciar sesión?" con link a `/auth/login` |
| FA-02 | Passwords no coinciden en frontend | Validación Zod bloquea envío de formulario, muestra error en campo "Confirmar password" |
| FA-03 | Password menor a 8 caracteres | Validación Zod bloquea envío, muestra error "Mínimo 8 caracteres" |
| FA-04 | Imagen URL formato inválido | Validación Zod muestra warning, permite envío vacío o con URL válida |
| FA-05 | Usuario omite creación de perfil (cierra navegador) | Usuario puede completar perfil después desde `/dashboard` con banner persistente "Completa tu perfil" |
| FA-06 | Token JWT expirado entre registro y creación de perfil | Sistema redirige a login, usuario debe iniciar sesión para completar perfil |
| FA-07 | Nombre artístico vacío | Validación Zod bloquea envío, muestra error "Nombre artístico es obligatorio" |

---

## Criterios de Aceptación

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-01-1 | Email debe ser único en ASP.NET Core Identity. Intentar registrar email duplicado retorna error descriptivo | Backend |
| AC-01-2 | Password debe tener mínimo 8 caracteres, validado en frontend (Zod) y backend (Identity) | Backend + Admin |
| AC-01-3 | Passwords deben coincidir, validación en frontend impide envío si no coinciden | Admin |
| AC-01-4 | Nombre artístico es obligatorio, validación Zod impide envío de formulario sin este campo | Admin + Backend |
| AC-01-5 | Imagen URL es opcional. Si se proporciona, debe ser URL válida. Campo acepta vacío | Admin + Backend |
| AC-01-6 | Tras registro exitoso, sistema retorna token JWT válido que permite acceso al endpoint `/api/artistas` | Backend |
| AC-01-7 | Tras crear perfil, entidad Artista se almacena en BD con UserId vinculado al usuario de Identity | Backend |
| AC-01-8 | Perfil artístico es visible públicamente en `/artistas/{id}` de la landing sin requerir autenticación | Landing + Backend |
| AC-01-9 | Usuario autenticado con perfil completo puede acceder a `/dashboard` | Admin + Backend |
| AC-01-10 | Schemas Zod para registro y perfil están definidos en `shared` y son reutilizados en frontend | Shared + Admin |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar endpoints `/api/auth/register` y `/api/artistas` (POST). Integrar ASP.NET Core Identity para registro de usuarios. Generar JWT tras registro exitoso. Crear entidad Artista y vincularla a UserId. Validar unicidad de email y datos de perfil. | ALTO |
| **Admin** | Implementar formulario de registro en `/auth/register` con validación Zod. Implementar formulario de creación de perfil en `/artista/perfil/crear`. Gestionar redirecciones post-registro. Almacenar token JWT en localStorage/sessionStorage. Mostrar dashboard tras completar registro. | ALTO |
| **Landing** | Implementar página pública de perfil de artista en `/artistas/{id}` consumiendo endpoint GET `/api/artistas/{id}`. Mostrar nombre artístico, descripción, imagen, ubicación. | MEDIO |
| **Shared** | Definir tipos TypeScript para User, Artista, RegisterDto, CreateArtistaDto. Definir schemas Zod para validación de formularios de registro y perfil. Exportar constantes para validación (MIN_PASSWORD_LENGTH, etc.). | ALTO |

---

## Dependencias

### Técnicas
- **WPR-001**: Configuración inicial de ASP.NET Core Identity en el módulo WebApi
- **WPR-002**: Implementación de JWT authentication en backend
- **WPR-003**: Configuración de DbContext con entidad User (Identity) y Artista
- **WPR-004**: Setup de React Query en proyectos Admin y Landing para data fetching
- **WPR-005**: Configuración de axios interceptors para manejo de tokens JWT en frontend

### De otras features
- Ninguna. Esta es la feature base que habilita el resto del sistema.

---

## Notas Técnicas

- ASP.NET Core Identity maneja la tabla `AspNetUsers` automáticamente. La entidad `Artista` se crea en tabla separada con FK a `UserId`.
- El token JWT debe incluir claims: `UserId`, `Email`, y `Role=Artista`.
- El flujo permite "registro parcial": usuario puede crear cuenta pero posponer la creación del perfil. El dashboard debe detectar este estado y mostrar banner de "Completa tu perfil".
- Validaciones críticas deben ocurrir en ambos lados: frontend (UX) y backend (seguridad).

---

## Referencia

- User Story completa: [docs/product/US-01-registro-artista.md](../../product/US-01-registro-artista.md)
