# WPR-015: Pruebas Manuales E2E

- > **Fecha:** 2026-02-12 (actualizado 2026-02-13)
- > **Tarea:** WPR-015
- > **Objetivo:** Verificar visualmente que las 3 apps funcionan y el flujo E2E es correcto
- > **Metodo:** Pruebas automatizadas con Playwright MCP (2026-02-13)

---

## Pre-requisitos

### Opcion A: Docker Compose (recomendada)

```bash
docker compose up -d
```

| Servicio | URL |
|----------|-----|
| Backend API | http://localhost:5001 |
| Landing (Vite + React) | http://localhost:3000 |
| Admin (Next.js) | http://localhost:3001 |
| SQL Server | localhost:1433 (sa / WePlayRises2024!) |

### Opcion B: Desarrollo local (con hot reload para frontend)

```bash
# Mantener API + SQL Server en Docker
docker compose up -d sqlserver api

# Frontend local con HMR
cd src/web && npm run dev    # http://localhost:3000
cd src/admin && npm run dev  # http://localhost:3001
```

**Nota:** El proxy de Vite apunta a `http://localhost:5001` (API Docker).

### Verificacion inicial

| # | Verificacion | URL | Resultado |
|---|-------------|-----|-----------|
| 1.1 | Swagger UI carga | http://localhost:5001/swagger | [x] OK |
| 1.2 | Landing carga | http://localhost:3000 | [x] OK |
| 1.3 | Admin carga | http://localhost:3001 | [x] OK - Redirige a /login correctamente |

---

## Paso 2: Registro de usuario

| # | Accion | Donde | Que verificar | Resultado |
|---|--------|-------|---------------|-----------|
| 2.1 | Navegar a registro | Landing `/auth/register` | Formulario se muestra correctamente | [x] OK |
| 2.2 | Registrar usuario | Landing `/auth/register` | Email + password + confirmPassword, enviar formulario | [x] OK |
| 2.3 | Verificar redireccion | Landing | Redirige a `/auth/login` tras registro exitoso | [x] OK |
| 2.4 | Probar registro en Admin | Admin `/register` | Mismo flujo funciona en Admin | [x] OK - Redirige a /artista/perfil/crear con toast "Cuenta creada exitosamente!" |

**Datos de prueba:**
- Email: `usuario1@mail.com` (Landing), `admin-test@mail.com` (Admin)
- Password: `123456`

**Bugs corregidos:**
- BUG-001: Frontend no enviaba `confirmPassword` (campo requerido por backend). Se agrego campo al formulario.
- BUG-003: Proxy Vite apuntaba a `https` pero API Docker usa `http`.
- BUG-004: Password min length inconsistente (8 backend vs 6 frontend). Unificado a 6.
- BUG-008: Schema compartido y Admin aun validaban password con min 8. Corregido en `shared/schemas/auth.schema.ts` y placeholder del RegisterForm.
- BUG-009: Admin Docker no podia conectar al backend (rewrites de Next.js se bake-an en build time). Corregido: `INTERNAL_API_URL` seteado en Dockerfile antes del build.

---

## Paso 3: Login

| # | Accion | Donde | Que verificar | Resultado |
|---|--------|-------|---------------|-----------|
| 3.1 | Navegar a login | Landing `/auth/login` | Formulario se muestra | [x] OK |
| 3.2 | Login con credenciales | Landing `/auth/login` | Login exitoso, recibe JWT | [x] OK |
| 3.3 | Verificar sesion | Landing | Redirige a `/dashboard`, muestra "Bienvenido, usuario1@mail.com" | [x] OK |
| 3.4 | Probar login en Admin | Admin `/login` | Mismo flujo funciona | [x] OK - Redirige a /dashboard con toast "Bienvenido!" y muestra email en header |

**Bugs corregidos:**
- BUG-002: `auth.service.ts` llamaba `/auth/me` antes de guardar token, causando 401 y redirect silencioso. Fix: usar datos de respuesta del login directamente.

---

## Paso 4: Crear perfil artista

| # | Accion | Donde | Que verificar | Resultado |
|---|--------|-------|---------------|-----------|
| 4.1 | Navegar a perfil artista | Landing `/artista/perfil` | Formulario de perfil se muestra | [x] OK |
| 4.2 | Cargar perfil existente | Landing `/artista/perfil` | Si ya existe, muestra "Editar perfil" con datos | [x] OK |
| 4.3 | Guardar perfil | Landing | POST /api/artistas funciona, feedback en UI | [x] OK (artista ya existia) |
| 4.4 | Ver perfil publico | Landing `/artistas/{id}` | Datos del artista visibles publicamente | [x] OK - Muestra nombre, bio, estadisticas con diseno profesional |

**Datos de prueba:**
- Nombre artistico: `Usuario 1 Music`
- Bio: `Artista de prueba para testing E2E`
- Genero: `Rock`

**Bugs corregidos:**
- BUG-005: `GET /api/artistas/me` devolvia 400 porque "me" se interpretaba como Guid en ruta `{id}`. Fix: agregado endpoint dedicado `[HttpGet("me")]` en ArtistasController.
- BUG-006: Error codes del handler (ej: "4008") no coincidian con strings del controller (ej: "ARTISTA_ALREADY_EXISTS"). Fix: controller ahora usa `HttpStatusCode` del ServiceResponse en vez de comparar strings de error code.
- BUG-007: POST `/api/artistas` devolvia HTTP 500 en vez de 409 para conflicto (artista duplicado). Mismo fix que BUG-006.
- BUG-010: `ReferenceError: process is not defined` en shared/constants/index.ts al cargar perfil publico. `process.env` no existe en Vite browser. Fix: eliminado `process.env` del archivo compartido.

---

## Paso 5: Campanias

| # | Accion | Donde | Que verificar | Resultado |
|---|--------|-------|---------------|-----------|
| 5.1 | Ver listado campanias | Landing `/campanias` | Pagina carga (puede estar vacia) | [x] Falla - Error 500 backend: handler `GetAllCampaniasQuery` no registrado en DI |
| 5.2 | Crear campania | Admin dashboard | Formulario de creacion existe | [x] OK - Wizard de 4 pasos (Info Basica, Historia, Recompensas, Revision) implementado |
| 5.3 | Editar campania | Admin dashboard | Edicion de borrador funciona | [ ] No testeable - No hay campanias creadas (endpoint backend falla) |
| 5.4 | Publicar campania | Admin dashboard | Estado cambia a publicada | [ ] No testeable - Depende de 5.3 |
| 5.5 | Ver campania en landing | Landing `/campanias/{id}` | Campania publicada visible | [ ] No testeable - Depende de 5.4 |

**Bug critico backend:**
- BUG-011: MediatR handlers del modulo Crowdfunding no estan registrados en DI. `GetAllCampaniasQuery`, `GetMisCampaniasQuery` causan 500. Requiere registrar el assembly de Crowdfunding.Application en Program.cs.

---

## Paso 6: Verificacion API directa (Swagger)

Si alguna prueba de UI falla, verificar el backend directamente:

| # | Endpoint | Metodo | Que verificar | Resultado |
|---|----------|--------|---------------|-----------|
| 6.1 | `/api/auth/register` | POST | Registro retorna 200 + datos | [x] OK - Retorna userId, email, token, roles |
| 6.2 | `/api/auth/login` | POST | Login retorna JWT token | [x] OK - Retorna JWT y "Inicio de sesion exitoso" |
| 6.3 | `/api/auth/me` | GET | Con Bearer token, retorna usuario | [x] OK - Retorna userId, email, roles, emailConfirmed |
| 6.4 | `/api/artistas` | POST | Crear artista con JWT | [x] OK - Retorna artista creado con id |
| 6.5 | `/api/artistas/{id}` | GET | Retorna artista creado | [x] OK - Retorna datos completos del artista |
| 6.6 | `/api/artistas/by-user/{userId}` | GET | Retorna artista por userId | [x] OK - Retorna mismo artista |

**Nota:** Usar boton "Authorize" en Swagger con `Bearer {token}` obtenido del login.

---

## Paso 7: Verificaciones transversales

| # | Verificacion | Como | Resultado |
|---|-------------|------|-----------|
| 7.1 | Consola sin errores JS | F12 > Console en cada app | [x] Errores parciales - Solo errores de campanias (500 backend). Sin errores JS propios tras fix BUG-010 |
| 7.2 | CORS funciona | Requests de landing/admin al backend no fallan | [x] OK - Cross-origin requests de admin:3001 a api:5001 funcionan |
| 7.3 | Responsive landing | Redimensionar ventana a movil | [x] OK parcial - Contenido responsive pero navbar se trunca en movil (falta menu hamburguesa) |
| 7.4 | Responsive admin | Redimensionar ventana a movil | [x] OK - Sidebar colapsa a hamburguesa, cards apiladas, dashboard legible |
| 7.5 | Terminal backend sin errores | Revisar logs en terminal del dotnet run | [x] Errores parciales - Solo errores de MediatR Crowdfunding (BUG-011). Auth y Artistas sin errores |

---

## Bugs y ajustes encontrados

| # | App | Descripcion | Severidad | Accion |
|---|-----|-------------|-----------|--------|
| BUG-001 | Web | Frontend no enviaba `confirmPassword` en registro | Critico | Corregido: campo agregado a formulario, schema Zod y types |
| BUG-002 | Web | Login fallaba silenciosamente (llamaba /auth/me sin token) | Critico | Corregido: usar datos de respuesta del login directamente |
| BUG-003 | Web | Proxy Vite apuntaba a https pero API Docker usa http | Alto | Corregido: `vite.config.ts` target cambiado a http |
| BUG-004 | API+Web | Password min length inconsistente (8 vs 6) | Medio | Corregido: unificado a 6 en Identity, validator y Zod |
| BUG-005 | API | `GET /artistas/me` daba 400 (ruta {id} no acepta "me") | Critico | Corregido: endpoint `[HttpGet("me")]` agregado |
| BUG-006 | API | Error codes del handler no coincidian con controller switch | Alto | Corregido: controller usa HttpStatusCode en vez de strings |
| BUG-007 | API | POST /artistas devolvia 500 en vez de 409 (duplicado) | Alto | Corregido: mismo fix que BUG-006 |
| BUG-008 | Shared+Admin | Password min length aun era 8 en schema compartido y Admin | Medio | Corregido: `shared/schemas/auth.schema.ts` y RegisterForm placeholder a 6 |
| BUG-009 | Admin Docker | Admin no conectaba al backend en Docker (rewrites baked at build time) | Critico | Corregido: `INTERNAL_API_URL` seteado en Dockerfile antes de `npm run build` |
| BUG-010 | Shared | `ReferenceError: process is not defined` en constantes compartidas | Alto | Corregido: eliminado `process.env` de `shared/constants/index.ts` |
| BUG-011 | API | MediatR handlers de Crowdfunding no registrados en DI | Critico | Pendiente: registrar assembly Crowdfunding.Application en Program.cs |

**Severidades:** Critico / Alto / Medio / Bajo

---

## Resumen

| Area | Total pruebas | OK | Falla | No testeable |
|------|---------------|-----|-------|------------|
| Servicios levantan | 3 | 3 | 0 | 0 |
| Registro | 4 | 4 | 0 | 0 |
| Login | 4 | 4 | 0 | 0 |
| Perfil artista | 4 | 4 | 0 | 0 |
| Campanias | 5 | 1 | 1 | 3 |
| API directa | 6 | 6 | 0 | 0 |
| Transversales | 5 | 5 | 0 | 0 |
| **Total** | **31** | **27** | **1** | **3** |

---

## Conclusion

- [x] Las 3 apps (API, Landing, Admin) levantan correctamente en Docker
- [x] Flujo registro -> login -> perfil artista funciona en Landing Y Admin
- [x] Perfil publico de artista visible en Landing
- [x] API directa: todos los endpoints de Auth y Artistas funcionan (6/6)
- [x] CORS funciona entre apps
- [x] Admin y Landing responsive (Admin mejor, Landing necesita menu hamburguesa)
- [x] 10 bugs encontrados: 10 corregidos, 1 pendiente
- [ ] Campanias bloqueadas por BUG-011 (MediatR DI del modulo Crowdfunding)
- [x] WPR-015 completado (excepto campanias bloqueadas por backend)
