# Validacion QA: cp-tareas-promocion (Admin)

**Fecha:** 2026-03-01
**Feature:** cp-tareas-promocion (US-CP-04)
**Target:** src/admin

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos (todos los AC) | 14 |
| AC con impacto en Admin | 6 |
| Cubiertos por especificacion (ui-ux.md + contracts.md) | 5 |
| Parcialmente Cubiertos | 1 |
| No Cubiertos | 0 |
| Planes de implementacion generados | 0 de 3 |
| **Score de Cobertura (especificacion)** | **83 %** |
| **Score de Cobertura (planes Admin)** | **0 %** |

**Estado de especificacion:** REQUIERE PLANES - Los requisitos Admin estan bien cubiertos en la documentacion fuente (ui-ux.md, contracts.md), pero los tres planes de implementacion (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) aun no han sido generados para `frontend-admin`. Este informe valida la especificacion disponible e identifica los gaps que los planes deberan cubrir.

---

## 2. Criterios de Aceptacion

### Fuente: feature-spec.md

Todos los AC del feature-spec, clasificados por proyecto de impacto:

| ID | Criterio | Tipo | Impacto Admin |
|----|----------|------|---------------|
| AC-CP04-1 | El promotor puede ver el listado de tareas activas con nombre, descripcion, tipo, recompensa, repetibilidad y estado personal | Funcional | No |
| AC-CP04-2 | El promotor puede enviar completado con URL de prueba valida; comentario opcional max 500 chars | Funcional | No |
| AC-CP04-3 | Al completar se crea PromoTareaPromotor con EstadoTareaId=2, TareaId, ProgramaPromotorId, URL y FechaCompletado | Funcional (Backend) | No |
| AC-CP04-4 | Para tareas repetibles, el promotor puede enviar hasta MaxRepeticiones completados; cada uno crea registro independiente | Funcional (Backend) | No |
| AC-CP04-5 | Completar tarea no repetible ya completada/validada retorna 400 con mensaje descriptivo | Funcional (Backend) | No |
| AC-CP04-6 | Completar tarea repetible con MaxRepeticiones alcanzado retorna 400 con mensaje descriptivo | Funcional (Backend) | No |
| AC-CP04-7 | El artista propietario puede ver listado paginado de completados pendientes, con nombre de tarea, promotor, URL de prueba, comentario y fecha | Funcional | **SI** |
| AC-CP04-8 | Al validar: backend actualiza EstadoTareaId=3, FechaValidado, crea PromotorWalletTransaccion y actualiza SaldoDisponible/TotalGanado en una sola transaccion | Funcional (Backend + Admin) | **SI** |
| AC-CP04-9 | Al rechazar: backend actualiza EstadoTareaId=4 y persiste ComentarioValidacion; promotor puede ver motivo en su vista | Funcional (Backend + Admin) | **SI** |
| AC-CP04-10 | Promotor con completado rechazado puede re-enviar con nueva URL; crea nuevo PromoTareaPromotor sin modificar el rechazado | Funcional (Backend) | No |
| AC-CP04-11 | No se puede completar tarea de programa con EsActivo=false; backend retorna 400 | Funcional (Backend) | No |
| AC-CP04-12 | Promotor no aprobado que intenta enviar completado recibe 403 Forbidden | Funcional (Backend) | No |
| AC-CP04-13 | Si la wallet del promotor no existe al validar, el sistema la crea automaticamente antes de acreditar | Funcional (Backend) | **SI** (feedback en toast) |
| AC-CP04-14 | Los tipos TypeScript de tareas y completados estan definidos en shared y son reutilizados por Landing y Admin | Shared | **SI** (consumo en Admin) |

**AC con impacto directo en Admin: AC-CP04-7, AC-CP04-8, AC-CP04-9, AC-CP04-13, AC-CP04-14**
**AC con impacto indirecto en Admin (feedback de reglas backend): AC-CP04-3, AC-CP04-4, AC-CP04-5, AC-CP04-6**

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales con Impacto en Admin

| ID | Criterio | ui-ux.md (Admin) | contracts.md | contracts-plan.md (Shared) | Estado |
|----|----------|-------------------|--------------|----------------------------|--------|
| AC-CP04-7 | Artista ve listado paginado de completados pendientes con nombre tarea, promotor, URL, comentario, fecha | Pantalla 3 - Card de completado pendiente con todos los campos requeridos. Paginacion con page/pageSize. Tab "Pendientes (N)" | GET /programas/{id}/tareas-pendientes con TareaPendienteItem (todos los campos) | TareasPendientesResponse, TareaPendienteItem, QUERY_KEYS.tareas.pendientes, API_ROUTES | CUBIERTO |
| AC-CP04-8 | Al validar: backend actualiza estado, FechaValidado, crea WalletTransaccion y actualiza Wallet; Admin muestra feedback de recompensa acreditada | Pantalla 4 (Dialog Validar): bloque verde de recompensa a acreditar. Toast "Tarea validada. Se acreditaron {importe} {moneda} en la wallet del promotor." | PATCH /programas/{id}/tareas-promotor/{id}/validar. Response: ValidarTareaResponse con recompensaAcreditada, monedaNombre | ValidarTareaRequest, ValidarTareaResponse, API_ROUTES.validarTarea, rechazarTareaSchema | CUBIERTO |
| AC-CP04-9 | Al rechazar: backend actualiza estado y persiste ComentarioValidacion; artista ingresa motivo en Admin | Pantalla 5 (Dialog Rechazar): campo "Motivo del rechazo" obligatorio (min 10 chars, max 500). Toast "Tarea rechazada. El promotor podra re-enviar con nueva prueba." | PATCH /programas/{id}/tareas-promotor/{id}/rechazar. Body: comentarioValidacion obligatorio | RechazarTareaRequest, rechazarTareaSchema, API_ROUTES.rechazarTarea | CUBIERTO |
| AC-CP04-13 | Wallet creada automaticamente si no existe al validar | Toast muestra importe acreditado independientemente de si la wallet existia o fue creada. No hay pantalla de creacion de wallet. Comportamiento transparente al artista | Backend crea wallet automaticamente. Frontend solo muestra el importe del ValidarTareaResponse | No requiere logica adicional en Shared | CUBIERTO |
| AC-CP04-14 | Tipos TypeScript en shared reutilizados por Admin | ui-ux.md no define estructura de tipos pero especifica los datos que se muestran, alineados con los contratos | contracts.md define los DTO C# que se mapean 1-a-1 con los tipos TS | TareaPendienteItem, ValidarTareaRequest, ValidarTareaResponse, RechazarTareaRequest, RechazarTareaResponse todos definidos en contracts-plan.md | CUBIERTO |

### 3.2 Requisitos con Impacto Indirecto en Admin (Errores y Estados)

| ID | Criterio | Cobertura en Admin | Estado |
|----|----------|--------------------|--------|
| AC-CP04-3 | PromoTareaPromotor se crea correctamente al completar (estado, campos) | Admin no crea completados. Los campos se muestran en la lista de pendientes segun TareaPendienteItem. FechaCompletado visible como fecha relativa en la card | CUBIERTO (lectura) |
| AC-CP04-4 | Tareas repetibles generan registros independientes; vecesCompletada visible | ui-ux.md especifica mostrar "Xta vez completada" en card de pendiente. vecesCompletada viene del endpoint GET tareas-pendientes | CUBIERTO |
| AC-CP04-5 | 400 al intentar completar tarea no repetible ya completada | Admin no ejecuta esta accion. La lista pendientes solo muestra EstadoTareaId=2 | N/A para Admin |
| AC-CP04-6 | 400 al superar MaxRepeticiones en tarea repetible | Admin no ejecuta esta accion | N/A para Admin |

### 3.3 Requisitos No Funcionales con Impacto en Admin

| ID | Criterio | Cobertura en ui-ux.md | Estado |
|----|----------|----------------------|--------|
| RNF-02 | GET tareas-pendientes responde en < 500ms para volumenes normales | Paginacion por defecto 10 items (TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE). La responsividad de la respuesta es backend pero la paginacion es el control del lado Admin | CUBIERTO (paginacion) |
| RNF-03 | Paginacion en tareas-pendientes, page size default 10 | Pantalla 3 especifica paginacion con controles "Anterior / Pagina X de N / Siguiente". pageSize=10 en query param inicial | CUBIERTO |
| RNF-05 | Registros inmutables; no se eliminan fisicamente | Admin no expone funcionalidad de borrado. Solo botones Validar y Rechazar | CUBIERTO |
| Accesibilidad | WCAG AA, focus visible, aria-labels, labels de formulario | ui-ux.md seccion Accesibilidad define: focus-visible ring, aria-busy, aria-disabled, DialogTitle, role=alert, aria-invalid, aria-label en botones de icono, time para fechas relativas, badges con texto legible | CUBIERTO |
| Responsive | Mobile < 768px, Tablet 768-1024px, Desktop > 1024px | ui-ux.md seccion Responsive Breakpoints Pantalla 3 y Dialogs 4 y 5: sidebar colapsada, botones debajo en mobile, bottom sheet dialogs en mobile | CUBIERTO |

---

## 4. Mapa de Pantallas Admin Requeridas vs Especificadas

| Pantalla Requerida (feature-spec.md) | Especificada en ui-ux.md | Ruta | Estado |
|--------------------------------------|--------------------------|------|--------|
| Pestana "Tareas pendientes de validacion" en detalle del programa | Pantalla 3: Tab "Pendientes (N)" en `/crowdpromotion/programas/{programaId}` | `/crowdpromotion/programas/{programaId}` (tab Pendientes) | CUBIERTO |
| Listado paginado de completados pendientes | Pantalla 3: Cards de completados con todos los campos | En la tab Pendientes | CUBIERTO |
| URL de prueba clickeable en nueva tab | Pantalla 3: `<a target="_blank" rel="noopener noreferrer">` con icono ExternalLink | En la card de completado | CUBIERTO |
| Dialog de confirmacion para Validar | Pantalla 4: Dialog Validar Tarea con datos del completado, recompensa a acreditar y campo de comentario opcional | Trigger: boton Validar en Pantalla 3 | CUBIERTO |
| Dialog de confirmacion para Rechazar (con campo de motivo) | Pantalla 5: Dialog Rechazar Tarea con aviso, campo motivo obligatorio (min 10 chars) y contador de caracteres | Trigger: boton Rechazar en Pantalla 3 | CUBIERTO |
| Contadores de completados por estado (mencionado en feature-spec.md como "contadores de completados por estado") | Pantalla 3 solo muestra count de pendientes en la tab. No hay contadores de Validadas/Rechazadas en la tab | Tab "Pendientes (N)" visible | PARCIAL |

---

## 5. Analisis de Gaps

### 5.1 Gaps Criticos

No se identifican gaps criticos. Los AC con impacto en Admin tienen cobertura en la especificacion de ui-ux.md y contracts.md.

### 5.2 Gaps Mayores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-CP04-8 | Feedback de recompensa acreditada en el toast | El toast especifica "Se acreditaron {importe} {moneda}" pero no define el comportamiento cuando la tarea tiene PuntosRecompensa en lugar de ImporteRecompensa. La feature-spec menciona "dinero o puntos" pero la Pantalla 3 y el Dialog Validar solo muestran el caso monetario | Medio | El plan de Admin debe definir el mensaje del toast y el bloque de recompensa del Dialog Validar para el caso de tareas con PuntosRecompensa. Verificar contra ValidarTareaResponse.puntosAcreditados |
| N/A | Contadores de completados por estado | feature-spec.md seccion Admin menciona "Mostrar contadores de completados por estado". ui-ux.md solo especifica el count de pendientes en la tab label. No hay especificacion de un resumen de estado (cuantas Validadas, cuantas Rechazadas, cuantas Completadas total) en la pagina del programa | Medio | El plan de Admin debe decidir si se implementa un resumen de estado (ej: "8 pendientes / 24 validadas / 3 rechazadas") o si el count del tab es suficiente para MVP. Consultar con product owner |

### 5.3 Gaps Menores

| ID | Criterio | Gap | Impacto | Recomendacion |
|----|----------|-----|---------|---------------|
| AC-CP04-9 | Validacion minima del motivo de rechazo | ui-ux.md define `z.string().min(10)` para el motivo de rechazo en el Dialog (min 10 chars), pero el contracts.md/feature-spec.md no especifica un minimo de 10 caracteres a nivel de negocio. El backend valida con `.NotEmpty()` (min 1). Hay discrepancia entre validacion frontend (min 10) y backend (min 1) | Bajo | El plan de Admin debe documentar la decision de usar min 10 en el frontend como medida de calidad adicional, y verificar que el backend no rechace comentarios de 1-9 caracteres (ya que el frontend nunca los enviaria pero un test de integracion podria hacerlo) |
| AC-CP04-7 | Numero de ejecucion ordinal en la card | ui-ux.md especifica "Comparte en Instagram Stories (4ta vez)" pero no define la funcion de formateo ordinal (1ra, 2da, 3ra, 4ta, Nma vez). Esta utilidad debe implementarse en el Admin o en Shared | Bajo | Agregar funcion `formatOrdinal(n: number): string` en `src/shared/utils/mappers.ts` siguiendo el patron de las otras funciones en contracts-plan.md, o implementarla localmente en el componente Admin |
| AC-CP04-14 | Imports de tipos Shared en Admin | contracts-plan.md define los tipos pero no especifica el patron de import que usara el Admin (ej: si importa directamente desde `src/shared` o via path alias `@shared`) | Bajo | El frontend-plan.md de Admin debe documentar la convencion de import de tipos Shared, alineada con el patron existente en los otros features de Admin |
| AC-CP04-7 | Ordenamiento de la lista de pendientes | ui-ux.md indica "ordenadas por fechaUltimaCompletada descendente" pero el endpoint GET tareas-pendientes en contracts.md no especifica un parametro de ordenamiento. El ordenamiento ocurre en backend pero no esta documentado en el contrato | Bajo | Verificar con el plan de Backend que el endpoint GET tareas-pendientes ordena por FechaCompletado DESC por defecto, y documentarlo en el frontend-plan.md del Admin |

---

## 6. Validacion de Estados de UI

### Pantalla 3 (Tab Pendientes)

| Estado | Requerido por Feature-spec | Especificado en ui-ux.md | Estado |
|--------|---------------------------|--------------------------|--------|
| Loading (carga inicial) | Si (implicitamente - buena UX) | 3 cards skeleton con animate-pulse h-[120px] | CUBIERTO |
| Default con pendientes | Si | Lista de cards ordenadas | CUBIERTO |
| Empty state sin pendientes | Si (implicitamente) | Icono CheckCircle verde + titulo "No hay tareas pendientes de validacion" | CUBIERTO |
| Error de carga | No especificado en feature-spec | Card de error con AlertCircle + boton Reintentar | CUBIERTO |
| Validando (accion en fila) | Si (feedback de accion) | Botones de la fila muestran estado loading; card desaparece con fade-out al exito | CUBIERTO |
| Rechazando (dialog abierto) | Si | Dialog de rechazo se abre; card espera hasta que el dialog confirme | CUBIERTO |

### Pantalla 4 (Dialog Validar Tarea)

| Estado | Requerido | Especificado | Estado |
|--------|-----------|--------------|--------|
| Default (formulario vacio) | Si | Datos del completado en modo solo lectura, campo comentario vacio | CUBIERTO |
| Enviando (loading) | Si | Boton "Validar tarea" muestra spinner y texto "Validando...". Dialog no se puede cerrar | CUBIERTO |
| Exito | Si | Dialog se cierra. Card desaparece de la lista. Toast con importe acreditado | CUBIERTO |
| Error de API (500) | Si | Dialog permanece abierto. Toast destructivo | CUBIERTO |
| Tarea sin recompensa monetaria | Mencionado en contracts (puntosAcreditados) | No especificado explicitamente para el bloque verde del dialog | PARCIAL |

### Pantalla 5 (Dialog Rechazar Tarea)

| Estado | Requerido | Especificado | Estado |
|--------|-----------|--------------|--------|
| Default (campo vacio) | Si | Campo motivo vacio, boton Rechazar habilitado | CUBIERTO |
| Campo motivo vacio (blur) | Si (validacion) | Borde cambia a border-red-500, mensaje de error | CUBIERTO |
| Enviando (loading) | Si | Boton muestra spinner y "Rechazando...". Dialog no se puede cerrar | CUBIERTO |
| Exito | Si | Dialog se cierra. Card desaparece. Toast "Tarea rechazada." | CUBIERTO |
| Error de API (500) | Si | Dialog permanece abierto. Toast destructivo | CUBIERTO |

---

## 7. Validacion de Tests

Los planes de test (`test-strategy.md`) aun no han sido generados. Este apartado identifica los tests que el plan debera cubrir para cada AC de Admin.

### Tests Criticos Requeridos para Admin

| AC | Test Requerido | Tipo | Cobertura Objetivo |
|----|----------------|------|-------------------|
| AC-CP04-7 | Renderizado de Pantalla 3 con lista de completados pendientes (datos mock del API) | Integration | Todos los campos de TareaPendienteItem visibles en la card |
| AC-CP04-7 | Paginacion: click en "Siguiente" hace fetch con page=2 | Integration | QUERY_KEYS correctos, llamada al API con page actualizado |
| AC-CP04-7 | Empty state cuando la respuesta tiene items=[] | Unit | Componente empty state visible |
| AC-CP04-7 | Loading state (skeletons) mientras fetch en curso | Unit | Skeletons visibles, datos no visibles |
| AC-CP04-8 | Click "Validar" abre Dialog Validar con datos del completado | Integration | Dialog visible con tarea, promotor, URL y bloque de recompensa |
| AC-CP04-8 | Confirmar validacion: PATCH /validar llamado con payload correcto | Integration | useValidarTarea mutation, toast con importe acreditado |
| AC-CP04-8 | Card desaparece de la lista tras validar con exito | Integration | Estado local actualizado tras mutacion exitosa |
| AC-CP04-8 | Error en PATCH /validar: dialog permanece, toast destructivo | Integration | Estado de error manejado en hook useValidarTarea |
| AC-CP04-9 | Click "Rechazar" abre Dialog Rechazar con datos del completado | Integration | Dialog visible, campo motivo vacio |
| AC-CP04-9 | Submit con motivo vacio: error de validacion, no se llama al API | Unit | Validacion rechazarTareaSchema activa antes del submit |
| AC-CP04-9 | Confirmar rechazo: PATCH /rechazar llamado con comentarioValidacion | Integration | useRechazarTarea mutation, toast "Tarea rechazada." |
| AC-CP04-9 | Card desaparece de la lista tras rechazar con exito | Integration | Estado local actualizado |
| AC-CP04-9 | Error en PATCH /rechazar: dialog permanece, toast destructivo | Integration | Estado de error manejado en hook useRechazarTarea |
| AC-CP04-13 | Toast de validacion muestra importe correcto del ValidarTareaResponse | Unit | Formateo de "Se acreditaron 5 EUR en la wallet" |
| AC-CP04-14 | Tipos TareaPendienteItem, ValidarTareaRequest, RechazarTareaRequest importados correctamente de Shared | Unit | No hay `any`, TypeScript compila sin errores |

### Tests de Accesibilidad

| Pantalla | Test | Tipo |
|----------|------|------|
| Pantalla 3 | URL de prueba tiene rel="noopener noreferrer" y target="_blank" | Unit |
| Dialog Validar | Focus trap dentro del dialog | Integration |
| Dialog Rechazar | Campo motivo tiene label asociado (htmlFor/aria) | Unit |
| Pantalla 3 | Skeletons tienen aria apropiado (aria-busy) | Unit |

---

## 8. Estructura de Archivos Admin Requerida

Basado en la especificacion, el plan de Admin debera definir los siguientes archivos nuevos:

```
src/admin/src/
  app/
    (dashboard)/
      crowdpromotion/
        programas/
          [programaId]/
            page.tsx              MODIFICAR: agregar tab "Pendientes"
  components/
    crowdpromotion/
      tareas/
        TareasPendientesList.tsx    NUEVO: listado paginado con skeleton/empty/error
        TareaPendienteCard.tsx      NUEVO: card de completado con acciones Validar/Rechazar
        ValidarTareaDialog.tsx      NUEVO: dialog de validacion con datos y recompensa
        RechazarTareaDialog.tsx     NUEVO: dialog de rechazo con campo motivo obligatorio
  hooks/
    crowdpromotion/
      useTareasPendientes.ts        NUEVO: useQuery GET tareas-pendientes con paginacion
      useValidarTarea.ts            NUEVO: useMutation PATCH validar con invalidacion
      useRechazarTarea.ts           NUEVO: useMutation PATCH rechazar con invalidacion
  services/
    crowdpromotion/
      tareas.service.ts             NUEVO (o ampliar existente): llamadas al API
```

### Integracion con pagina existente del programa

La Pantalla 3 no es una ruta nueva. Es una nueva **tab** en la ruta existente `/crowdpromotion/programas/{programaId}`. El plan de Admin debe especificar como se integra la tab "Pendientes" con las tabs ya existentes (Resumen, Promotores, Tareas) de la pagina de detalle del programa.

---

## 9. Recomendaciones

### Acciones Requeridas (para generar los planes)

1. **Generar `frontend-plan.md` para Admin**
   - Definir los 4 componentes nuevos: `TareasPendientesList`, `TareaPendienteCard`, `ValidarTareaDialog`, `RechazarTareaDialog`
   - Definir los 3 hooks nuevos: `useTareasPendientes`, `useValidarTarea`, `useRechazarTarea`
   - Especificar la integracion con la pagina existente del programa (tab adicional)
   - Especificar la funcion `formatOrdinal` para el numero de ejecucion

2. **Generar `ui-design.md` para Admin**
   - Trasladar las especificaciones de ui-ux.md Pantallas 3, 4 y 5 al formato de plan de Admin
   - Resolver el gap de la tarea con PuntosRecompensa en el bloque de recompensa del Dialog Validar
   - Definir si se implementan contadores de completados por estado (Validadas/Rechazadas) o si el tab count es suficiente para MVP

3. **Generar `test-strategy.md` para Admin**
   - Incluir todos los tests identificados en la seccion 7 de este documento
   - Cobertura objetivo: 80%+ en hooks y componentes de esta feature

### Acciones Sugeridas (Mayor)

4. **Resolver el caso de recompensa por puntos en Dialog Validar**
   - Archivo: `ValidarTareaDialog.tsx`
   - Cambio: el bloque verde de recompensa debe manejar dos variantes: `{importe} {moneda}` (cuando `recompensaAcreditada != null`) y `{puntosAcreditados} puntos` (cuando `puntosAcreditados != null`)
   - El toast de la Pantalla 3 tambien debe manejar ambos casos

5. **Clarificar contadores por estado en la pagina del programa**
   - Archivo: Pagina de detalle del programa en Admin
   - Cambio: Decidir si se agrega un resumen `"X pendientes / Y validadas / Z rechazadas"` en el header de la pagina o si el badge del tab es suficiente

### Nice to Have (Menor)

6. **Funcion `formatOrdinal` en Shared**
   - Archivo: `src/shared/utils/mappers.ts`
   - Cambio: Agregar `export function formatOrdinal(n: number): string` que devuelva "1ra", "2da", "3ra", "4ta", "Nma" segun corresponda. Utili para Landing y Admin

7. **Documentar el ordenamiento del endpoint GET tareas-pendientes**
   - Archivo: contracts.md (o en el plan de Backend)
   - Cambio: Agregar nota explicitando que los resultados vienen ordenados por FechaCompletado DESC para garantizar que el frontend no necesita re-ordenar

---

## 10. Checklist de Validacion

### Requisitos Funcionales Admin
- [x] AC-CP04-7: Listado paginado de completados pendientes especificado (Pantalla 3)
- [x] AC-CP04-8: Flujo de validacion con feedback de recompensa especificado (Pantalla 4)
- [x] AC-CP04-9: Flujo de rechazo con campo de motivo obligatorio especificado (Pantalla 5)
- [x] AC-CP04-13: Comportamiento transparente de creacion de wallet (sin pantalla adicional)
- [x] AC-CP04-14: Tipos Shared definidos y disponibles para consumo en Admin
- [ ] Contadores de completados por estado (gap: feature-spec menciona pero ui-ux.md no especifica completamente)

### UI/UX Admin
- [x] Pantalla 3 (Tab Pendientes) con layout, cards y paginacion especificados
- [x] Dialog Validar Tarea (Pantalla 4) con datos de solo lectura y campo comentario opcional
- [x] Dialog Rechazar Tarea (Pantalla 5) con campo motivo obligatorio y validacion
- [x] Estados de UI para Pantalla 3: loading, empty, error, validando, rechazando
- [x] Estados de UI para Dialogs: default, enviando, exito, error de API
- [x] Toast messages para todos los eventos (validar, rechazar, error)
- [x] Interacciones especificadas: click Validar/Rechazar, click URL, cambio de pagina
- [x] Responsive definido para Pantalla 3 (mobile, tablet, desktop)
- [x] Responsive definido para Dialogs (bottom sheet en mobile)
- [x] Accesibilidad especificada: focus, aria, labels, text de estado en badges
- [x] Animacion de card desapareciendo tras validar/rechazar especificada
- [ ] Bloque de recompensa en Dialog Validar para tareas con PuntosRecompensa (gap menor)

### Testing Admin
- [ ] frontend-plan.md no generado (bloqueante para test-strategy.md)
- [ ] ui-design.md no generado (bloqueante para test-strategy.md)
- [ ] test-strategy.md no generado
- [ ] Tests identificados en seccion 7 listos para ser implementados una vez generados los planes

### Shared Types
- [x] TareaPendienteItem definido en contracts-plan.md
- [x] TareasPendientesResponse definido en contracts-plan.md
- [x] ValidarTareaRequest definido en contracts-plan.md
- [x] ValidarTareaResponse definido en contracts-plan.md
- [x] RechazarTareaRequest definido en contracts-plan.md
- [x] RechazarTareaResponse definido en contracts-plan.md
- [x] QUERY_KEYS.crowdpromotion.tareas.pendientes definido
- [x] API_ROUTES para validarTarea y rechazarTarea definidos
- [x] ESTADO_TAREA_PROMO_LABELS y BADGES definidos
- [x] TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE = 10 definido

---

## 11. Conclusiones por AC Admin

### AC-CP04-7 - Listado paginado de completados pendientes

**Estado: CUBIERTO**

La Pantalla 3 del ui-ux.md especifica completamente el tab "Pendientes (N)" con:
- Card de completado que muestra: nombre promotor, tipo promotor, nombre tarea con numero de ejecucion (Xta vez), URL de prueba clickeable en nueva tab, comentario del promotor (con fallback "(sin comentario)"), fecha relativa
- Paginacion con controles Anterior/Siguiente y contador "Mostrando X de N pendientes"
- Badge rojo en tab con count cuando count > 0
- El endpoint `GET /programas/{id}/tareas-pendientes` en contracts.md devuelve todos los campos requeridos en `TareaPendienteItem`

Pendiente menor: definir ordenamiento explicito en el contrato del endpoint.

### AC-CP04-8 - Validacion con acreditacion de recompensa

**Estado: CUBIERTO (con gap menor para puntos)**

El Dialog Validar (Pantalla 4) cubre:
- Datos del completado en modo solo lectura: tarea, promotor, numero de ejecucion, URL de prueba clickeable, comentario del promotor
- Bloque verde de recompensa a acreditar: "{importe} {moneda} en wallet del promotor"
- Campo de comentario de validacion opcional (max 500 chars)
- Toast de exito con importe acreditado del response
- States: default, enviando (spinner), exito (dialog cierra + card desaparece + toast), error (dialog permanece + toast destructivo)

Gap: el bloque de recompensa no especifica el caso de tareas con PuntosRecompensa en lugar de importe monetario.

### AC-CP04-9 - Rechazo con motivo obligatorio y motivo visible al promotor

**Estado: CUBIERTO**

El Dialog Rechazar (Pantalla 5) cubre:
- Datos del completado en modo solo lectura (igual que Dialog Validar)
- Aviso en amarillo: "El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba"
- Campo "Motivo del rechazo" obligatorio con min 10 chars / max 500 chars
- Contador de caracteres en tiempo real
- Mensaje de error cuando el campo esta vacio (blur)
- Toast "Tarea rechazada. El promotor podra re-enviar con nueva prueba."

El requisito de que el promotor pueda ver el motivo en su vista (Landing) esta cubierto en la Pantalla 1 del ui-ux.md (Bloque de motivo de rechazo), no en Admin.

### AC-CP04-13 - Creacion automatica de wallet al validar

**Estado: CUBIERTO**

La creacion de wallet es transparente al artista. El toast muestra "{importe} {moneda}" independientemente de si la wallet existia o fue creada. No se requiere pantalla adicional en Admin. El backend maneja la creacion atomicamente antes de acreditar. El frontend Admin solo consume el `ValidarTareaResponse.recompensaAcreditada` para mostrar el feedback.

### AC-CP04-14 - Tipos TypeScript en Shared reutilizados por Admin

**Estado: CUBIERTO**

El contracts-plan.md define 5 tipos directamente utiles para Admin: `TareaPendienteItem`, `TareasPendientesResponse`, `ValidarTareaRequest`, `ValidarTareaResponse`, `RechazarTareaRequest`, `RechazarTareaResponse`. Ademas define los endpoints, query keys, constantes de estado y utilidades que el Admin consumira.

---

## 12. Conclusion

**Score de Cobertura de Especificacion Admin: 83%**

Los 5 AC con impacto directo en Admin (AC-CP04-7, AC-CP04-8, AC-CP04-9, AC-CP04-13, AC-CP04-14) estan bien especificados en la documentacion fuente. El gap del 17% corresponde principalmente a la ausencia de especificacion para el caso de recompensa por puntos y la ambiguedad sobre los contadores de estado totales.

**Score de Cobertura de Planes de Implementacion: 0%**

Los tres planes de implementacion para `frontend-admin` (`frontend-plan.md`, `ui-design.md`, `test-strategy.md`) no han sido generados. Este es el proximo paso obligatorio.

**Veredicto de Especificacion:** REQUIERE PLANES

**Proximo Paso:**

1. Generar `plans/cp-tareas-promocion/frontend-admin/frontend-plan.md` definiendo componentes, hooks y servicios
2. Generar `plans/cp-tareas-promocion/frontend-admin/ui-design.md` trasladando Pantallas 3, 4 y 5 del ui-ux.md al formato de plan, resolviendo el gap de PuntosRecompensa
3. Generar `plans/cp-tareas-promocion/frontend-admin/test-strategy.md` con los 15+ tests identificados en la seccion 7
4. Tras generar los planes, ejecutar una segunda validacion QA

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-01
