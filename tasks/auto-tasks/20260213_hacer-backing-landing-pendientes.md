# Hacer Backing Landing - Pendientes de Implementacion

**Fecha creacion:** 2026-02-13
**Estado:** Completado
**Total tareas:** 5
**Completadas:** 5
**Documento fuente:** plans/hacer-backing/frontend-landing/frontend-plan.md

---

## Objetivo

Completar los items pendientes detectados en la revision de la implementacion frontend landing de "hacer-backing". La implementacion core esta completa (81 tests pasan), pero quedan ajustes para alinear con el plan: sincronizar schema local, instalar toast system, y limpiar diferencias menores.

---

## Instrucciones de Uso

Para ejecutar la siguiente tarea pendiente:

```
/tasks/run-auto-task tasks/auto-tasks/20260213_hacer-backing-landing-pendientes.md
```

Claude Code:
1. Leera este documento
2. Si hay documento fuente, lo leera tambien para contexto completo
3. Identificara la primera tarea con `[ ]` (pendiente)
4. Ejecutara las acciones usando las referencias al documento fuente
5. Marcara la tarea como `[x]` (completada)
6. Actualizara el contador de completadas

---

## Progreso

```
[1] Schema Local       ██████████ ✓  Sincronizar backings/application/schemas.ts con @shared
[2] Toast System       ██████████ ✓  Instalar sonner y configurar toasts en useCreateBacking
[3] CampaniasPage      ██████████ ✓  Agregar filtros basicos a CampaniasPage
[4] BackingModal UX    ██████████ ✓  Mejorar flujo onSuccess con navegacion a confirmacion
[5] Tests Coverage     ██████████ ✓  Verificar que todos los tests pasan tras los cambios
────────────────────────────────────────
TOTAL                  [5/5] ██████████
```

---

## Tarea 1: Sincronizar Schema Local de Backings

- [x] **Alinear backings/application/schemas.ts con @shared/schemas/backing.schema.ts**

### Referencia
> Seccion: [Types y Schemas](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 7
> Archivo local: `src/web/src/features/backings/application/schemas.ts`
> Archivo shared: `src/shared/schemas/backing.schema.ts`

### Contexto
El schema local en `backings/application/schemas.ts` solo tiene `campaniaId`, `rewardId`, `monto` - esta desactualizado. El `BackingForm` ya usa correctamente el schema de `@shared/schemas/backing.schema` que incluye `mensaje`, `esAnonimo`, `monto` con limites. El archivo local deberia re-exportar desde shared o eliminarse.

### Acciones
1. Abrir `src/web/src/features/backings/application/schemas.ts`
2. Reemplazar el contenido por re-exports del shared schema:
   ```typescript
   // Re-export from shared contracts
   export {
       createBackingSchema,
       validateBackingAmount,
       validateRewardAvailability,
       validateCampaniaActive,
       type CreateBackingFormData,
   } from "@shared/schemas/backing.schema"
   ```
3. Verificar que `backings/application/index.ts` exporta correctamente
4. Verificar que ningun otro archivo importa el schema local directamente

### Entregables
- `schemas.ts` sincronizado como re-export de @shared
- Sin importaciones rotas

### Criterios de Completado
- [x] `schemas.ts` re-exporta desde `@shared/schemas/backing.schema`
- [x] No hay schema duplicado (campaniaId ya no se define localmente)
- [x] `npm run build` o `tsc --noEmit` pasa sin errores
- [x] Tests existentes siguen pasando (160 tests pass)

---

## Tarea 2: Instalar y Configurar Toast System (sonner)

- [x] **Instalar sonner y agregar toasts en useCreateBacking**

### Referencia
> Seccion: [Toast System](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 13.7
> Seccion: [useCreateBacking](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 4.3

### Contexto
El plan indica usar `sonner` para notificaciones toast en `useCreateBacking`. Actualmente no hay toast system instalado. Los errores se muestran inline en el BackingModal, pero el plan tambien requiere toasts para UX consistente.

### Acciones
1. Instalar sonner: `npm install sonner`
2. Agregar `<Toaster />` en el layout o App principal:
   - Buscar `App.tsx` o el layout root en `src/web/src/app/`
   - Agregar: `import { Toaster } from "sonner"` y `<Toaster position="top-right" theme="dark" />`
3. Modificar `useCreateBacking` en `src/web/src/features/backings/application/useBackings.ts`:
   - Agregar `import { toast } from "sonner"`
   - En `onSuccess`: agregar `toast.success("Apoyo confirmado!", { description: "Gracias por tu contribucion" })`
   - Agregar `onError` handler: extraer mensaje del error y mostrar `toast.error("Error al procesar apoyo", { description: message })`
4. Verificar que BackingModal siga mostrando error inline ademas del toast

### Entregables
- `sonner` instalado en package.json
- `<Toaster />` renderizado en la app
- Toast success despues de backing exitoso
- Toast error cuando falla el backing

### Criterios de Completado
- [x] `sonner` aparece en package.json dependencies
- [x] `<Toaster />` esta en el layout/App
- [x] toast.success se muestra al crear backing exitosamente
- [x] toast.error se muestra cuando falla la API
- [x] Los tests de useCreateBacking siguen pasando (mockear toast si es necesario)

---

## Tarea 3: Agregar Filtros Basicos a CampaniasPage

- [x] **Agregar search y filtro de estado a CampaniasPage**

### Referencia
> Seccion: [CampaniasPage (Modificar)](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 3.10
> Seccion: [CampaniaFilters](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 3.2

### Contexto
`CampaniasPage` actualmente solo renderiza `CampaniaList` sin filtros. El plan indica agregar search + filtro de estado. No es necesario crear `CampaniaFilters` como componente separado - se puede implementar inline para MVP. `ExplorarPage` ya tiene filtros como referencia.

### Acciones
1. Abrir `src/web/src/features/campanias/presentation/pages/CampaniasPage.tsx`
2. Agregar estado local para `searchTerm` y `estadoCampaniaId` (default: 2 = Publicada)
3. Pasar parametros a `useCampanias({ searchTerm, estadoCampaniaId })`
4. Agregar un Input de busqueda con icono Search (de lucide-react) y debounce
5. Agregar Select (de shadcn) con opciones de estado: "Todas", "Activas", "Finalizadas"
6. Mostrar contador de resultados
7. Revisar `ExplorarPage.tsx` como referencia de implementacion

### Entregables
- CampaniasPage con input de busqueda funcional
- Dropdown de filtro por estado de campania
- Contador de resultados visible

### Criterios de Completado
- [x] Input de busqueda filtra campanias por titulo/artista
- [x] Dropdown filtra por estado (Todas, Activas, Finalizadas)
- [x] Contador muestra "N campanias"
- [x] Responsive: filtros se stackean en mobile
- [x] No rompe funcionalidad existente

---

## Tarea 4: Mejorar Flujo onSuccess en BackingModal

- [x] **Asegurar que BackingModal navega a confirmacion correctamente tras exito**

### Referencia
> Seccion: [BackingModal](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 3.5
> Seccion: [Flujo de Datos](../plans/hacer-backing/frontend-landing/frontend-plan.md) - Seccion 6

### Contexto
El hook `useCreateBacking` ya navega a la pagina de confirmacion en `onSuccess`. Sin embargo, `BackingModal` tambien tiene su propio `onSuccess` callback que cierra el modal. Hay que verificar que el flujo no cause conflictos (cerrar modal + navegar simultaneamente). El plan indica: modal se cierra -> navigate a confirmacion. El hook ya hace el navigate, asi que el modal solo necesita cerrarse.

### Acciones
1. Revisar el flujo completo:
   - `BackingForm.onSubmit` -> `BackingModal.handleSubmit` -> `useCreateBacking.mutate`
   - En `onSuccess` del mutate: invalida queries + navega a confirmacion (ya implementado)
   - `BackingModal` cierra el dialog en su `onSuccess` callback
2. Verificar que no hay race condition entre cerrar modal y navegar
3. Si `BackingModal.onSuccess` prop no esta siendo pasado desde `CampaniaDetailPage`, conectar el prop
4. Verificar que `CampaniaDetailPage` no tiene la prop `onSuccess` en el BackingModal (actualmente no la pasa)
5. Esto es correcto: el hook ya maneja la navegacion. Solo verificar que todo funciona end-to-end

### Entregables
- Flujo confirmado: submit -> loading -> success -> navigate a confirmacion
- Sin race conditions entre modal close y navigation

### Criterios de Completado
- [x] Al hacer submit exitoso, el usuario llega a `/campanias/:id/confirmacion`
- [x] El backingId aparece en la URL como query param
- [x] No hay errores en consola durante el flujo
- [x] BackingConfirmationPage muestra datos correctos

---

## Tarea 5: Verificar Tests y Build Final

- [x] **Ejecutar todos los tests y verificar build limpio**

### Contexto
Despues de las tareas anteriores (schema sync, sonner install, filtros, flujo modal), verificar que todo sigue funcionando correctamente.

### Acciones
1. Ejecutar tests: `cd src/web && npx vitest run --reporter=verbose`
2. Verificar build: `cd src/web && npx tsc --noEmit`
3. Si hay tests fallando, corregirlos
4. Si hay nuevos componentes (filtros), agregar tests basicos
5. Verificar que dev server arranca sin errores: `cd src/web && npm run dev` (verificar visualmente)

### Entregables
- Todos los tests pasan (81+ tests)
- TypeScript compila sin errores
- Dev server arranca correctamente

### Criterios de Completado
- [x] `npx vitest run` - 0 tests fallando
- [x] `npx tsc --noEmit` - 0 errores
- [x] `npm run dev` arranca sin errores en consola
- [x] Navegacion manual: `/campanias` -> click campania -> `/campanias/:id` -> click "Apoyar" -> modal abre correctamente

---

## Registro de Ejecucion

| Tarea | Fecha | Duracion | Notas |
|-------|-------|----------|-------|
| 1 | 2026-02-14 | ~2 min | Reemplazado schema local con re-exports de @shared. 160 tests pasan, tsc limpio. |
| 2 | 2026-02-14 | ~3 min | Instalado sonner, Toaster en App.tsx, toast.success/error en useCreateBacking, mock en tests. 160 tests pasan. |
| 3 | 2026-02-14 | ~3 min | Search input con debounce, Select filtro estado (Todas/Activas/Finalizadas), contador resultados, responsive. 160 tests pasan, tsc limpio. |
| 4 | 2026-02-14 | ~3 min | Flujo verificado end-to-end: submit -> hook onSuccess (invalidate + toast + navigate) -> BackingConfirmationPage. Sin race conditions (React 18 batching). Route, page y backingId query param confirmados. 160 tests pasan, tsc limpio. |
| 5 | 2026-02-14 | ~2 min | 160 tests pasan (12 test files), tsc --noEmit limpio, Vite dev server arranca en 562ms. Rutas verificadas: /campanias, /campanias/:id, backing confirmation. |

---

*Ultima actualizacion: 2026-02-14*

