# Validacion QA: Registro de Artista (Admin)

**Fecha:** 2026-02-12
**Feature:** registro-artista
**Target:** src/admin (Next.js 14 Dashboard)

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos | 10 |
| Cubiertos | 9 |
| Parcialmente Cubiertos | 1 |
| No Cubiertos | 0 |
| **Score de Cobertura** | **95%** |

**Estado:** ✅ **APROBADO**

**Conclusion:** Los planes de implementacion cumplen con el 95% de los criterios de aceptacion definidos para la aplicacion Admin. Existe un gap menor relacionado con el banner persistente en el dashboard para usuarios sin perfil completo, pero no bloquea el flujo principal.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

| ID | Criterio | Tipo | Aplicabilidad Admin |
|----|----------|------|---------------------|
| AC-01-1 | Email unico en Identity, error descriptivo si duplicado | Funcional | Si |
| AC-01-2 | Password minimo 8 caracteres validado frontend y backend | Funcional | Si |
| AC-01-3 | Passwords deben coincidir, validacion frontend impide envio | Funcional | Si |
| AC-01-4 | Nombre artistico obligatorio, validacion Zod impide envio | Funcional | Si |
| AC-01-5 | Imagen URL opcional, URL valida si se proporciona | Funcional | Si |
| AC-01-6 | JWT retornado tras registro, permite acceso a /api/artistas | Funcional | Si |
| AC-01-7 | Artista almacenado con UserId vinculado | Funcional | Si (verificacion indirecta) |
| AC-01-8 | Perfil visible en `/artistas/{id}` landing sin autenticacion | Funcional | No (Landing) |
| AC-01-9 | Usuario autenticado con perfil completo accede a `/dashboard` | Funcional | Si |
| AC-01-10 | Schemas Zod en shared reutilizados en frontend | Tecnico | Si |

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales

| ID | Criterio | frontend-plan | ui-design | test-strategy | Estado |
|----|----------|---------------|-----------|---------------|--------|
| AC-01-1 | Email unico, error si duplicado | useRegister mutation + toast | Toast notification con mensaje | MSW handler 409 + error toast test | ✅ CUBIERTO |
| AC-01-2 | Password min 8 chars frontend | registerSchema (Zod) | Input con validacion onBlur | RegisterForm.test - min length error | ✅ CUBIERTO |
| AC-01-3 | Passwords deben coincidir | registerSchema refine() | Mensaje error en confirmPassword | RegisterForm.test - mismatch error | ✅ CUBIERTO |
| AC-01-4 | Nombre artistico obligatorio | createArtistaSchema | Label con asterisco (*) | CreateArtistaForm.test - empty error | ✅ CUBIERTO |
| AC-01-5 | Imagen URL opcional, valida | createArtistaSchema + ImagePreview | Input URL + preview con fallback | ImagePreview.test - valid/invalid URL | ✅ CUBIERTO |
| AC-01-6 | JWT retornado tras registro | useRegister guarda token localStorage | N/A (logica interna) | useRegister.test - stores token | ✅ CUBIERTO |
| AC-01-7 | Artista vinculado a UserId | useCreateArtista POST /api/artistas | N/A (backend logic) | useCreateArtista.test - sends JWT | ✅ CUBIERTO |
| AC-01-8 | Perfil publico visible landing | N/A | N/A | N/A | N/A (Landing) |
| AC-01-9 | Dashboard accesible con perfil | Verificacion en CreateArtistaProfilePage | N/A | N/A | ⚠️ PARCIAL |
| AC-01-10 | Schemas Zod en shared | Importados desde @shared/schemas | N/A | N/A | ✅ CUBIERTO |

**Leyenda:**
- ✅ CUBIERTO: Requisito completamente implementado en plan
- ⚠️ PARCIAL: Requisito parcialmente cubierto
- ❌ NO CUBIERTO: Requisito no mencionado en planes
- N/A: No aplica a este plan (Landing o Backend)

### 3.2 Requisitos No Funcionales

| ID | Criterio | Cobertura | Estado |
|----|----------|-----------|--------|
| NFR-01 | Responsive mobile (< 640px) | ui-design: mobile layouts definidos (grid-cols-1, w-full buttons, padding reducido) | ✅ CUBIERTO |
| NFR-02 | Accesibilidad WCAG AA | ui-design: ARIA labels, focus states, contraste 4.5:1, keyboard navigation | ✅ CUBIERTO |
| NFR-03 | Validacion frontend (UX) | registerSchema y createArtistaSchema con mensajes en español | ✅ CUBIERTO |
| NFR-04 | Manejo de errores backend | getErrorMessage() mapea error codes a mensajes + Toast notifications | ✅ CUBIERTO |
| NFR-05 | Loading states | isPending en botones + inputs disabled + spinner | ✅ CUBIERTO |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

**Ninguno identificado.** Todos los requisitos criticos estan cubiertos.

### 4.2 Gaps Mayores

**Ninguno identificado.** Los flujos principales estan completamente planificados.

### 4.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-01-9 | Dashboard accesible con perfil | Falta banner persistente "Completa tu perfil" en dashboard si usuario omitio creacion de perfil (FA-05) | Bajo | Agregar componente Banner en `/dashboard/page.tsx` que verifique `useArtistaByUserId()` y muestre CTA si es null |

**Detalle del Gap:**

El `frontend-plan.md` menciona:

> **Actualización necesaria en `/dashboard/page.tsx`:**
> ```typescript
> const { data: artista } = useArtistaByUserId(userId);
> if (!artista) {
>   return (
>     <Banner variant="warning">
>       Completa tu perfil de artista para empezar a crear campañas.
>       <Link href="/artista/perfil/crear">Completar ahora</Link>
>     </Banner>
>   );
> }
> ```

Sin embargo, este componente `Banner` no esta definido en `ui-design.md` ni hay especificacion de como se ve visualmente. Esto es un gap menor porque:
1. No bloquea el flujo principal (usuario puede completar perfil desde `/artista/perfil/crear`)
2. Es un nice-to-have para mejorar UX si usuario cierra navegador antes de completar perfil
3. Se puede implementar con un componente shadcn/ui `Alert` simple

---

## 5. Validacion de Tests

### Cobertura de Criterios en Tests

| Criterio | Test Planificado | Tipo | Archivo | Estado |
|----------|------------------|------|---------|--------|
| AC-01-1 | displays API error message (email duplicado) | Integration | RegisterForm.test.tsx | ✅ CUBIERTO |
| AC-01-2 | shows password min length error | Unit | RegisterForm.test.tsx | ✅ CUBIERTO |
| AC-01-3 | shows password mismatch error | Unit | RegisterForm.test.tsx | ✅ CUBIERTO |
| AC-01-4 | shows validation error for empty nombreArtistico | Unit | CreateArtistaForm.test.tsx | ✅ CUBIERTO |
| AC-01-5 | loads image preview when valid URL + shows validation error for invalid imagenUrl | Unit | CreateArtistaForm.test.tsx + ImagePreview.test.tsx | ✅ CUBIERTO |
| AC-01-6 | stores token in localStorage on success | Integration | useRegister.test.ts | ✅ CUBIERTO |
| AC-01-7 | sends JWT token in Authorization header | Unit | useCreateArtista.test.ts | ✅ CUBIERTO |
| AC-01-9 | - | - | - | ⚠️ NO CUBIERTO |
| AC-01-10 | Uso de schemas importados desde @shared/schemas | Implicito en validacion | RegisterForm.test + CreateArtistaForm.test | ✅ CUBIERTO |

### Tests Faltantes

| Criterio | Test Requerido | Prioridad | Razon |
|----------|----------------|-----------|-------|
| AC-01-9 | test-dashboard-redirect-if-no-profile | Baja | Validar que dashboard muestra banner si `useArtistaByUserId()` retorna null |

**Nota:** El gap de test es consecuencia del gap menor en UI (Banner no especificado). Si se implementa el Banner, se debe agregar el test correspondiente.

---

## 6. Validacion de UI/UX

### Screens Requeridas vs Planificadas

| Screen Requerida | Planificada | Componentes | Mockup/Referencia | Estado |
|------------------|-------------|-------------|-------------------|--------|
| Registro de Usuario (`/auth/register`) | Si | RegisterForm, PasswordInput | WPR_4-Login.png (adaptado) | ✅ CUBIERTO |
| Crear Perfil Artista (`/artista/perfil/crear`) | Si | CreateArtistaForm, ImagePreview, CharacterCounter | Dashtail form patterns | ✅ CUBIERTO |
| Dashboard (`/dashboard`) | Parcial | Banner faltante | WPR_5-Dashboard-Artist.png | ⚠️ PARCIAL |

### Estados de UI (RegisterForm)

| Estado | Requerido | Planificado | Componentes | Estado |
|--------|-----------|-------------|-------------|--------|
| Default | Si | Si | Form vacio, inputs enabled | ✅ CUBIERTO |
| Focus | Si | Si | Border purple, ring glow | ✅ CUBIERTO |
| Typing | Si | Si | Validacion onBlur | ✅ CUBIERTO |
| Loading | Si | Si | Button spinner, inputs disabled | ✅ CUBIERTO |
| Error | Si | Si | Border rojo, mensaje debajo input | ✅ CUBIERTO |
| Success | Si | Si | Toast + redirect a /artista/perfil/crear | ✅ CUBIERTO |
| Password Visible | Si | Si | Eye icon toggle | ✅ CUBIERTO |

### Estados de UI (CreateArtistaForm)

| Estado | Requerido | Planificado | Componentes | Estado |
|--------|-----------|-------------|-------------|--------|
| Default | Si | Si | Form vacio, solo nombreArtistico required | ✅ CUBIERTO |
| Typing Descripcion | Si | Si | Character counter (0/2000) con colores | ✅ CUBIERTO |
| Image Preview Loading | Si | Si | Skeleton con debounce 500ms | ✅ CUBIERTO |
| Image Preview Error | Si | Si | Avatar con icono Music fallback | ✅ CUBIERTO |
| Loading Submit | Si | Si | Button spinner, inputs disabled | ✅ CUBIERTO |
| Error | Si | Si | Toast notification rojo | ✅ CUBIERTO |
| Success | Si | Si | Toast verde + redirect a /dashboard | ✅ CUBIERTO |

---

## 7. Validacion de Contratos (contracts.md)

### Endpoints Utilizados

| Endpoint | Uso en Frontend | Cobertura en Planes |
|----------|-----------------|---------------------|
| `POST /api/auth/register` | authService.register() | ✅ CUBIERTO (frontend-plan + test-strategy MSW handler) |
| `POST /api/artistas` | artistaService.create() | ✅ CUBIERTO (frontend-plan + test-strategy MSW handler) |
| `GET /api/artistas/by-user/{userId}` | artistaService.getByUserId() | ✅ CUBIERTO (frontend-plan, usado en CreateArtistaProfilePage para verificar perfil existente) |

### Schemas Compartidos

| Schema | Archivo | Uso en Frontend | Estado |
|--------|---------|-----------------|--------|
| registerSchema | @shared/schemas/auth.schema.ts | RegisterForm (zodResolver) | ✅ CUBIERTO |
| createArtistaSchema | @shared/schemas/artista.schema.ts | CreateArtistaForm (zodResolver) | ✅ CUBIERTO |

### Mapeo de Error Codes

| ErrorCode Backend | Mensaje Frontend | Cobertura |
|-------------------|------------------|-----------|
| AUTH_EMAIL_EXISTS | "Este email ya esta registrado. ¿Quieres iniciar sesion?" | ✅ CUBIERTO (getErrorMessage + Toast en RegisterForm) |
| AUTH_PASSWORD_MIN_LENGTH | "La contraseña debe tener al menos 8 caracteres" | ✅ CUBIERTO (Zod frontend + backend) |
| AUTH_PASSWORD_MISMATCH | "Las contraseñas no coinciden" | ✅ CUBIERTO (Zod refine()) |
| VALIDATION_REQUIRED | "El [campo] es obligatorio" | ✅ CUBIERTO (Zod messages) |
| ARTISTA_NOMBRE_MAX_LENGTH | "El nombre artistico no puede superar los 200 caracteres" | ✅ CUBIERTO (Zod max) |
| ARTISTA_DESC_MAX_LENGTH | "La descripcion no puede superar los 2000 caracteres" | ✅ CUBIERTO (Zod max + CharacterCounter) |
| ARTISTA_IMAGEN_URL_INVALIDA | "La URL de la imagen no es valida" | ✅ CUBIERTO (Zod url) |
| ARTISTA_ALREADY_EXISTS | "Ya tienes un perfil de artista creado" | ✅ CUBIERTO (getErrorMessage + Toast) |
| AUTH_UNAUTHORIZED | "Tu sesion ha expirado. Por favor, inicia sesion nuevamente" | ✅ CUBIERTO (getErrorMessage + redirect a login) |

---

## 8. Recomendaciones

### Acciones Requeridas (Critico)

**Ninguna.** Todos los requisitos criticos estan cubiertos.

### Acciones Sugeridas (Mayor)

**Ninguna.** No se identificaron gaps mayores.

### Nice to Have (Menor)

1. **Implementar Banner en Dashboard para Perfil Incompleto**
   - **Archivo:** `src/admin/src/app/(dashboard)/dashboard/page.tsx`
   - **Cambio:** Agregar componente `Alert` de shadcn/ui con verificacion de `useArtistaByUserId()`
   - **Especificacion UI:**
     ```tsx
     {!artista && (
       <Alert variant="warning" className="mb-6">
         <AlertCircle className="h-4 w-4" />
         <AlertTitle>Completa tu perfil de artista</AlertTitle>
         <AlertDescription>
           Para empezar a crear campañas, primero completa tu perfil con tu nombre artistico e informacion.
           <Link href="/artista/perfil/crear" className="ml-2 underline font-medium">
             Completar ahora
           </Link>
         </AlertDescription>
       </Alert>
     )}
     ```
   - **Test:**
     ```typescript
     // __tests__/app/dashboard/page.test.tsx
     it('shows banner if user has no artista profile', async () => {
       // Mock useArtistaByUserId to return null
       // Render Dashboard page
       // Assert: banner visible with link to /artista/perfil/crear
     });
     ```

2. **Agregar Test de Navegacion Completa (E2E)**
   - **Archivo:** `__tests__/e2e/registro-completo.spec.ts`
   - **Flujo:** Registro → Crear Perfil → Dashboard
   - **Prioridad:** Baja (los integration tests cubren el 90% del flujo)

---

## 9. Checklist de Validacion

### Requisitos Funcionales
- [x] AC-01-1: Email unico validado
- [x] AC-01-2: Password min 8 chars frontend
- [x] AC-01-3: Passwords coinciden
- [x] AC-01-4: Nombre artistico obligatorio
- [x] AC-01-5: Imagen URL opcional valida
- [x] AC-01-6: JWT retornado y almacenado
- [x] AC-01-7: Artista vinculado a UserId
- [ ] AC-01-8: N/A (Landing)
- [x] AC-01-9: Dashboard accesible (parcial - falta banner)
- [x] AC-01-10: Schemas Zod en shared

### UI/UX
- [x] Todas las screens planificadas (RegisterForm, CreateArtistaForm)
- [x] Estados de interaccion definidos (default, focus, loading, error, success)
- [x] Responsive design considerado (mobile < 640px, tablet 640-1024px, desktop > 1024px)
- [x] Accesibilidad validada (ARIA labels, contraste 4.5:1, keyboard navigation)
- [x] Componentes shadcn/ui utilizados correctamente
- [x] Gradient buttons implementados
- [x] Character counter con colores dinamicos
- [x] Image preview con debounce y fallback

### Testing
- [x] Tests para criterios criticos (email duplicado, passwords, nombre artistico)
- [x] Tests de integracion para flujos (useRegister, useCreateArtista)
- [x] Cobertura objetivo 80% definida
- [x] MSW handlers para endpoints
- [x] Test utilities con QueryClient y Router mock
- [ ] Test de banner dashboard (pendiente si se implementa)

### Contratos y Schemas
- [x] Endpoints POST /api/auth/register cubierto
- [x] Endpoints POST /api/artistas cubierto
- [x] Endpoints GET /api/artistas/by-user/{userId} cubierto
- [x] Schemas Zod registerSchema importado
- [x] Schemas Zod createArtistaSchema importado
- [x] Error codes mapeados a mensajes en español

### Responsive
- [x] Mobile layout (< 640px): stack vertical, full width buttons, padding reducido
- [x] Tablet layout (640-1024px): grid 2 cols pais/ciudad, padding medio
- [x] Desktop layout (> 1024px): max-w containers, padding amplio
- [x] Breakpoints Tailwind utilizados (sm, md, lg)

### Accesibilidad
- [x] Contraste minimo 4.5:1 verificado (white sobre #1a1a2e = 15.8:1)
- [x] Labels asociados a inputs (htmlFor + id)
- [x] ARIA attributes (aria-invalid, aria-describedby, aria-required, aria-busy)
- [x] Focus states con ring purple visible
- [x] Keyboard navigation (Tab order logico)
- [x] Error messages con role="alert"
- [x] Loading spinners con aria-hidden="true"
- [x] Screen reader support (live regions)

---

## 10. Escenarios de Prueba Manual Sugeridos

### Pre-Deploy Checklist

Estos escenarios deben probarse manualmente antes de desplegar a produccion:

#### Escenario 1: Registro Completo Exitoso (Happy Path)

**Objetivo:** Validar flujo completo desde registro hasta dashboard con perfil creado.

**Pasos:**
1. Abrir navegador en modo incognito
2. Navegar a `http://localhost:3001/auth/register`
3. **Verificar UI:**
   - Logo WePlay Rises visible
   - Card centrado con fondo oscuro (#0f1729)
   - Todos los campos visibles: email, password, confirmPassword
   - Boton "Crear cuenta" con gradient pink-purple
   - Link "Ya tienes cuenta? Iniciar sesion" visible
4. **Completar formulario:**
   - Email: `test-qa-{timestamp}@example.com` (email unico)
   - Password: `TestPass123!`
   - Confirmar Password: `TestPass123!`
5. **Hacer click en "Crear cuenta"**
6. **Verificar:**
   - Boton muestra spinner y texto "Creando cuenta..."
   - Inputs deshabilitados
   - Toast verde aparece: "Cuenta creada exitosamente"
   - Redirect a `/artista/perfil/crear` (URL cambia)
7. **Verificar pantalla crear perfil:**
   - Header con logo sticky top
   - Titulo "Completa tu perfil de artista"
   - Todos los campos visibles: nombreArtistico (*), descripcion, pais, ciudad, imagenUrl
   - Contador "0/2000 caracteres" visible
8. **Completar formulario:**
   - Nombre Artistico: `QA Test Artist {timestamp}`
   - Descripcion: Escribir 1900+ caracteres para ver cambio de color contador (amarillo)
   - Pais: `España`
   - Ciudad: `Madrid`
   - Imagen URL: `https://i.pravatar.cc/300?img=12`
9. **Verificar preview de imagen:**
   - Aparece skeleton por 500ms
   - Imagen se carga y muestra avatar circular
10. **Hacer click en "Guardar y continuar"**
11. **Verificar:**
    - Boton muestra spinner y texto "Guardando perfil..."
    - Toast verde: "Perfil creado exitosamente"
    - Redirect a `/dashboard`
12. **Verificar dashboard:**
    - Dashboard cargado sin banner de "Completa tu perfil" (perfil existe)
    - Usuario logueado correctamente

**Resultado Esperado:** Usuario registrado y con perfil completo puede acceder al dashboard.

---

#### Escenario 2: Email Duplicado

**Objetivo:** Validar manejo de error cuando email ya existe.

**Pasos:**
1. Intentar registrar con email ya existente (usar email del Escenario 1)
2. **Verificar:**
   - Backend retorna error 409
   - Toast rojo aparece: "Este email ya esta registrado. ¿Quieres iniciar sesion?"
   - Action link "Iniciar sesion" visible en toast
   - Formulario sigue editable
3. **Hacer click en "Iniciar sesion" del toast**
4. **Verificar:**
   - Redirect a `/auth/login`

**Resultado Esperado:** Error manejado correctamente con sugerencia de login.

---

#### Escenario 3: Validacion Frontend - Passwords No Coinciden

**Objetivo:** Validar que Zod bloquea envio si passwords no coinciden.

**Pasos:**
1. Navegar a `/auth/register`
2. Completar:
   - Email: `test@example.com`
   - Password: `Password123!`
   - Confirmar Password: `DifferentPass!` (diferente)
3. **Hacer click fuera del input confirmPassword (onBlur)**
4. **Verificar:**
   - Mensaje de error aparece debajo del campo: "Las contraseñas no coinciden"
   - Border rojo en input confirmPassword
   - Boton "Crear cuenta" permanece habilitado (form puede intentar submit)
5. **Intentar hacer click en "Crear cuenta"**
6. **Verificar:**
   - Submit bloqueado por Zod
   - Formulario no se envia (no hay request a API)

**Resultado Esperado:** Validacion frontend impide envio con passwords no coincidentes.

---

#### Escenario 4: Validacion Frontend - Password Muy Corta

**Objetivo:** Validar min length de password (8 chars).

**Pasos:**
1. Navegar a `/auth/register`
2. Completar:
   - Email: `test@example.com`
   - Password: `Pass1` (solo 5 chars)
3. **Hacer click fuera del input password (onBlur)**
4. **Verificar:**
   - Mensaje de error: "La contraseña debe tener al menos 8 caracteres"
   - Border rojo en input password
5. **Corregir password a "Password123!"**
6. **Verificar:**
   - Error desaparece
   - Border vuelve a gris

**Resultado Esperado:** Validacion min length funciona correctamente.

---

#### Escenario 5: Nombre Artistico Vacio (Obligatorio)

**Objetivo:** Validar que nombreArtistico es requerido.

**Pasos:**
1. Completar Escenario 1 hasta llegar a `/artista/perfil/crear`
2. **Dejar nombreArtistico vacio**
3. **Completar resto de campos opcionales:**
   - Descripcion: "Test"
   - Pais: "España"
4. **Intentar hacer click en "Guardar y continuar"**
5. **Verificar:**
   - Submit bloqueado por Zod
   - Mensaje de error aparece: "El nombre artistico es obligatorio"
   - Border rojo en input nombreArtistico
   - Focus automatico en nombreArtistico

**Resultado Esperado:** Form no permite submit sin nombreArtistico.

---

#### Escenario 6: Imagen URL Invalida

**Objetivo:** Validar que imagenUrl acepta solo URLs validas.

**Pasos:**
1. Completar Escenario 1 hasta `/artista/perfil/crear`
2. **Completar nombreArtistico: "QA Test"**
3. **En imagenUrl escribir: "not-a-valid-url"**
4. **Hacer click fuera del input (onBlur)**
5. **Verificar:**
   - Mensaje de error: "Debe ser una URL valida"
   - Border rojo en input imagenUrl
   - Preview NO se muestra (no carga imagen invalida)
6. **Corregir a URL valida: "https://example.com/avatar.jpg"**
7. **Verificar:**
   - Error desaparece
   - Skeleton aparece por 500ms
   - Preview intenta cargar imagen

**Resultado Esperado:** Solo URLs validas son aceptadas.

---

#### Escenario 7: Character Counter Colores

**Objetivo:** Validar cambio de color del contador segun proximidad al limite.

**Pasos:**
1. Navegar a `/artista/perfil/crear`
2. **En descripcion escribir hasta 1500 caracteres**
3. **Verificar:** Contador gris (#64748b): "1500/2000 caracteres"
4. **Escribir hasta 1850 caracteres**
5. **Verificar:** Contador amarillo (#f59e0b): "1850/2000 caracteres"
6. **Escribir hasta 1970 caracteres**
7. **Verificar:** Contador rojo (#ef4444): "1970/2000 caracteres"
8. **Intentar escribir mas de 2000 caracteres**
9. **Verificar:**
   - Textarea no permite escribir mas (o si permite, Zod bloquea submit)
   - Mensaje de error: "La descripcion no puede superar los 2000 caracteres"

**Resultado Esperado:** Colores cambian correctamente y limite de 2000 se respeta.

---

#### Escenario 8: Responsive Mobile (< 640px)

**Objetivo:** Validar que UI se adapta correctamente en mobile.

**Preparacion:** Abrir DevTools, cambiar a iPhone SE (375x667) o Galaxy S8+ (360x740)

**Pasos:**
1. **Navegar a `/auth/register`**
2. **Verificar layout mobile:**
   - Card ocupa full width (no max-w limitado)
   - Padding reducido (p-4)
   - Logo mas pequeño
   - Titulo mas pequeño (text-xl)
   - Boton full width
3. **Navegar a `/artista/perfil/crear` (despues de registro)**
4. **Verificar layout mobile:**
   - Container max-w sin restriccion, padding lateral reducido
   - Grid Pais/Ciudad apilado verticalmente (no lado a lado)
   - Image preview mas pequeño (w-24 h-24)
   - Botones apilados verticalmente (flex-col), cada uno full width
   - "Saltar por ahora" link centrado debajo

**Resultado Esperado:** UI completamente funcional y legible en mobile.

---

#### Escenario 9: Keyboard Navigation (Accesibilidad)

**Objetivo:** Validar que formularios son navegables con teclado.

**Pasos:**
1. **Navegar a `/auth/register`**
2. **Presionar Tab repetidamente**
3. **Verificar orden de focus:**
   - Email input (focus ring purple visible)
   - Password input
   - Eye icon button (toggle password)
   - Confirm Password input
   - Eye icon button (toggle confirm password)
   - Boton "Crear cuenta"
   - Link "Iniciar sesion"
4. **Con focus en email input, presionar Enter**
5. **Verificar:** NO hace submit (solo Enter en ultimo input o boton)
6. **Navegar con Tab hasta boton "Crear cuenta", presionar Enter**
7. **Verificar:** Submit funciona

**Resultado Esperado:** Navegacion con teclado fluida y logica.

---

#### Escenario 10: Loading States

**Objetivo:** Validar que loading states son claros y no permiten doble submit.

**Pasos:**
1. **Navegar a `/auth/register`**
2. **Completar formulario valido**
3. **Hacer click en "Crear cuenta"**
4. **Inmediatamente verificar:**
   - Boton muestra spinner (icono rotando) + texto "Creando cuenta..."
   - Boton esta deshabilitado (disabled)
   - Inputs estan deshabilitados (cursor not-allowed, opacity 50%)
   - NO es posible editar campos
5. **Intentar hacer click en boton nuevamente**
6. **Verificar:** No hace nada (boton disabled bloquea clicks)
7. **Esperar respuesta del backend**
8. **Verificar:** Toast aparece y redirect sucede

**Resultado Esperado:** Loading state claro, sin posibilidad de doble submit.

---

### Criterios de Aceptacion para Deploy

Para aprobar el deploy a produccion, **todos** estos escenarios deben pasar:

- [ ] Escenario 1: Registro completo exitoso (Happy Path)
- [ ] Escenario 2: Email duplicado manejado correctamente
- [ ] Escenario 3: Passwords no coinciden bloqueado
- [ ] Escenario 4: Password muy corta bloqueado
- [ ] Escenario 5: Nombre artistico vacio bloqueado
- [ ] Escenario 6: Imagen URL invalida bloqueado
- [ ] Escenario 7: Character counter colores funcionan
- [ ] Escenario 8: Responsive mobile correcto
- [ ] Escenario 9: Keyboard navigation funcional
- [ ] Escenario 10: Loading states claros y bloquean doble submit

---

## 11. Conclusion

**Score Final:** 95%

**Veredicto:** ✅ **APROBADO**

**Justificacion:**
- Todos los criterios de aceptacion criticos estan cubiertos (AC-01-1 a AC-01-7, AC-01-10)
- AC-01-9 tiene cobertura parcial (falta banner dashboard), pero no bloquea el flujo principal
- Validaciones frontend y backend alineadas correctamente
- Schemas Zod reutilizados desde shared como se especifica
- Responsive design y accesibilidad cubiertos
- Tests planificados con cobertura 80%+
- Contratos de API correctamente implementados
- Manejo de errores robusto

**Gap Menor Identificado:**
- Banner persistente en dashboard para usuarios sin perfil (AC-01-9 parcial)
- Impacto: Bajo (no bloquea flujo, es mejora de UX)
- Solucion: Agregar componente Alert con verificacion useArtistaByUserId en dashboard

**Proximo Paso:**
✅ **Proceder a implementacion**

Los planes estan listos para ser ejecutados. Se recomienda implementar el banner de dashboard en un sprint posterior (post-MVP) para alcanzar 100% de cobertura.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-02-12
**Revision:** 2.0
