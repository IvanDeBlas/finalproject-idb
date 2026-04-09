# Tests E2E de Integracion - WePlay Rises

Suite de tests end-to-end que valida el flujo completo de la plataforma
atravesando los 4 servicios Docker: **API**, **Admin**, **Landing** y **SQL Server**.

---

## Prerequisitos

Los 4 servicios deben estar corriendo via Docker Compose:

```bash
# Bash
cd C:/Repos/WePlay_Rises
docker compose up -d
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises
docker compose up -d
```

| Servicio   | URL                    | Puerto |
|------------|------------------------|--------|
| API        | http://localhost:5001  | 5001   |
| Admin      | http://localhost:3001  | 3001   |
| Landing    | http://localhost:3000  | 3000   |
| SQL Server | localhost:1433         | 1433   |

Chromium para Playwright debe estar instalado:

```bash
# Bash (desde src/admin)
cd src/admin && npx playwright install chromium
```

```powershell
# PowerShell
cd src\admin; npx playwright install chromium
```

---

## Comandos de Ejecucion

### Ejecutar todos los tests (headless - sin ventana de browser)

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright test e2e/integration
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright test e2e/integration
```

### Ejecutar con browser visible

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright test e2e/integration --headed
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright test e2e/integration --headed
```

### Ejecutar en modo debug (paso a paso)

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright test e2e/integration --debug
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright test e2e/integration --debug
```

### Ejecutar con trace (para diagnosticar fallos)

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright test e2e/integration --trace on
npx playwright show-trace test-results/*/trace.zip
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright test e2e/integration --trace on
npx playwright show-trace test-results\*\trace.zip
```

### Ejecutar un test especifico por nombre

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright test e2e/integration -g "4.2 Create campaign"
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright test e2e/integration -g "4.2 Create campaign"
```

### Ver reporte HTML despues de ejecutar

```bash
# Bash
cd /c/Repos/WePlay_Rises/src/admin && npx playwright show-report
```

```powershell
# PowerShell
cd C:\Repos\WePlay_Rises\src\admin
npx playwright show-report
```

---

## Estructura de Archivos

```
src/admin/
  e2e/
    integration/
      helpers.ts                   # Helpers reutilizables (login, wizard, publish)
      wpr015-full-flow.spec.ts     # 18 tests seriales del flujo E2E completo
  playwright.config.ts             # Config: chromium, 1 worker, serial
```

---

## Catalogo de Tests (18 tests)

Los tests se ejecutan en **orden serial** porque son dependientes entre si
(ej: test 5.1 publica la campana creada en test 4.2).

### 1. Health Checks (3 tests)

Verifican que los 3 servicios web responden correctamente.

| Test | Servicio | Verifica |
|------|----------|----------|
| 1.1 API Swagger loads | API (5001) | GET /swagger retorna 200, titulo "Swagger UI" |
| 1.2 Landing loads | Landing (3000) | Titulo de pagina contiene "WePlay Rises" |
| 1.3 Admin loads | Admin (3001) | Titulo de pagina contiene "WePlay Rises" |

### 2. Autenticacion (2 tests)

| Test | App | Verifica |
|------|-----|----------|
| 2.1 Login to Admin via UI | Admin | Login con formulario, redirige a /dashboard, email visible en header |
| 2.2 Get API token | API | POST /api/auth/login retorna JWT valido |

**Usuario de prueba:** `usuario1@mail.com` / `123456` (pre-seeded en Docker DB).

### 3. Perfil de Artista (2 tests)

| Test | App | Verifica |
|------|-----|----------|
| 3.1 Artist profile loads | Admin | Pagina /perfil carga, heading "Editar Perfil", campo "Nombre artistico" con valor |
| 3.2 Update artist profile | Admin | Editar biografia, click "Actualizar perfil", toast "Perfil actualizado" |

### 4. Creacion de Campana (4 tests)

| Test | App | Verifica |
|------|-----|----------|
| 4.1 Wizard loads with 4 steps | Admin | Pagina /campanias/nueva carga con stepper de 4 pasos: Informacion Basica, Historia, Recompensas, Revision |
| 4.2 Create campaign as borrador | Admin | Completa wizard (4 pasos), crea campana, obtiene UUID de la URL |
| 4.3 Campaign detail shows borrador | Admin | Detalle muestra titulo, badge "Borrador", "VISTA PREVIA", boton "Publicar Ahora" |
| 4.4 Campaign in Mis Campanias list | Admin | Lista /campanias muestra el titulo de la campana creada |

**Datos de la campana de test:**
- Titulo: `E2E Campaign {timestamp}` (unico por ejecucion)
- Meta: 5000 EUR
- Tipo: Todo o Nada
- Fecha fin: dia 15 del mes siguiente
- Subtitulo: "Subtitulo E2E test"

### 5. Publicacion (2 tests)

| Test | App | Verifica |
|------|-----|----------|
| 5.1 Publish campaign | Admin | Click "Publicar Ahora" -> confirmar -> toast "Campania publicada exitosamente" |
| 5.2 Published in list | Admin | Lista /campanias muestra la campana despues de publicar |

### 6. Visibilidad en Landing (3 tests)

Estos tests navegan a `http://localhost:3000` (Landing) para verificar que
la campana publicada en Admin es visible en la web publica.

| Test | App | Verifica |
|------|-----|----------|
| 6.1 Campaign visible on Landing | Landing | Lista /campanias muestra el titulo de la campana |
| 6.2 Campaign detail on Landing | Landing | Detalle muestra titulo, subtitulo, estado "Activa", meta "5000", tab "Historia", boton "Apoyar" habilitado |
| 6.3 Artist section on campaign | Landing | Seccion "Sobre el Artista" visible, link "Ver perfil" presente |

### 7. Dashboard (1 test)

| Test | App | Verifica |
|------|-----|----------|
| 7.1 Dashboard shows campaign | Admin | Pagina /dashboard muestra el titulo de la campana publicada |

### 99. Cleanup (1 test)

| Test | App | Verifica |
|------|-----|----------|
| 99. Delete test campaign | API | DELETE /api/campanias/{id} con JWT, acepta status 200/204/404 |

---

## Flujo Visual del Test

```
                        ADMIN (3001)                              LANDING (3000)
                   +-----------------------+                  +-------------------+
                   |                       |                  |                   |
  [1] Health ----->| Login form            |    [6] --------->| /campanias        |
                   |   |                   |                  |   campana visible |
  [2] Login ------>|   v                   |                  |                   |
                   | /dashboard            |    [6.2] ------->| /campanias/{id}   |
  [3] Profile ---->| /perfil (editar bio)  |                  |   titulo, estado  |
                   |                       |                  |   tabs, Apoyar    |
  [4] Wizard ----->| /campanias/nueva      |                  |                   |
                   |   Step 1: Info Basica |    [6.3] ------->| Sobre el Artista  |
                   |   Step 2: Historia    |                  |   Ver perfil link |
                   |   Step 3: Recompensas |                  +-------------------+
                   |   Step 4: Revision    |
                   |   -> Crear campania   |                  API (5001)
                   |                       |                  +-------------------+
  [5] Publish ---->| /campanias/{id}       |    [99] -------->| DELETE campania   |
                   |   Publicar Ahora      |                  +-------------------+
                   |                       |
  [7] Dashboard -->| /dashboard            |
                   |   campana visible     |
                   +-----------------------+
```

---

## Helpers Reutilizables (helpers.ts)

Los helpers estan disenados para ser importados en cualquier test futuro.

### Constantes

| Constante | Valor | Uso |
|-----------|-------|-----|
| `ADMIN_BASE` | `http://localhost:3001` | URL base del Admin (Next.js) |
| `LANDING_BASE` | `http://localhost:3000` | URL base del Landing (Vite) |
| `API_BASE` | `http://localhost:5001` | URL base de la API (.NET) |
| `TEST_USER` | `{ email, password }` | Credenciales del usuario seed |

### Funciones

| Funcion | Descripcion |
|---------|-------------|
| `loginViaApi(email, password)` | Login directo a la API. Retorna `{ token, userId }`. Para llamadas programaticas. |
| `loginToAdminUI(page, email?, password?)` | Login completo al Admin via formulario UI. Inyecta auth en localStorage para navegaciones posteriores. |
| `fillWizardStep1(page, data)` | Rellena Step 1 del wizard: titulo, tipo financiacion, meta, fecha fin opcional. |
| `fillWizardStep2(page, data)` | Rellena Step 2 del wizard: subtitulo, descripcion corta. |
| `clickSiguiente(page)` | Click en "Siguiente" con retry (el wizard requiere doble-click). |
| `createCampaignViaWizard(page, data)` | Flujo completo del wizard. Retorna el campaign ID (UUID). |
| `publishCampaign(page)` | Publica campana desde su detalle. Maneja dialogo de confirmacion. |

### Ejemplo: usar helpers en un test nuevo

```typescript
import { test, expect } from "@playwright/test"
import { loginToAdminUI, ADMIN_BASE } from "./helpers"

test("mi nuevo test", async ({ page }) => {
    await loginToAdminUI(page)
    await page.goto(`${ADMIN_BASE}/campanias`)
    // ... assertions
})
```

---

## Detalles Tecnicos

### Modo de ejecucion

- **Serial**: `test.describe.configure({ mode: "serial" })` - los tests dependen del orden
- **1 worker**: configurado en `playwright.config.ts` para evitar conflictos con datos compartidos
- **Chromium only**: unico browser configurado
- **Headless por defecto**: usar `--headed` para ver el browser

### Estrategia de autenticacion

El login en tests headless requiere un workaround por dos race conditions:

1. **authService.login() race**: `getCurrentUser()` se ejecuta antes de que el token
   este en localStorage. Solucion: pre-seed del token via API antes de enviar el formulario.

2. **Zustand hydration race**: el dashboard layout redirige a `/login` antes de que
   zustand persist rehidrate desde localStorage. Solucion: `addInitScript` inyecta
   auth data antes de que los scripts de la pagina se ejecuten, y el dashboard layout
   tiene un hydration guard (`useState(false)` + `useEffect`).

### Limpieza automatica

El test `99. Cleanup` elimina la campana creada via API `DELETE /api/campanias/{id}`.
Si el test falla antes del cleanup, la campana queda en la DB (no afecta tests futuros
porque cada ejecucion usa un titulo unico con timestamp).

### Tiempos de ejecucion tipicos

| Modo | Tiempo |
|------|--------|
| Headless | ~16-18 segundos |
| Headed | ~20-25 segundos |

---

## Troubleshooting

### "Executable doesn't exist" al ejecutar

Instalar el browser:
```bash
cd src/admin && npx playwright install chromium
```

### Tests fallan con timeout en login

Verificar que los servicios Docker estan corriendo:
```bash
docker compose ps
curl http://localhost:5001/swagger
curl http://localhost:3001/login
curl http://localhost:3000
```

### Tests fallan con "element not found"

La UI puede haber cambiado. Ejecutar con `--debug` para inspeccionar la pagina
interactivamente y ajustar los selectores.

### Rebuild del admin tras cambios de codigo

Si se modifico codigo del Admin, reconstruir la imagen Docker:
```bash
docker compose build admin && docker compose up -d admin
```

```powershell
docker compose build admin; docker compose up -d admin
```
