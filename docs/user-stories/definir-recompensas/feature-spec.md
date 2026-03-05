# Feature: Definir Recompensas

> **ID:** definir-recompensas
> **User Story:** US-03
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripción

Esta feature permite a los artistas crear y gestionar recompensas (rewards) para sus campañas de crowdfunding. Las recompensas son incentivos que los artistas ofrecen a los fans a cambio de sus aportaciones económicas, y son fundamentales para el éxito de una campaña. Cada recompensa tiene un nombre, descripción, monto mínimo de aportación, y opcionalmente puede tener stock limitado y tiempo de entrega estimado.

El sistema permite al artista crear múltiples niveles de recompensas (desde agradecimientos digitales hasta experiencias exclusivas), ordenarlas visualmente mediante drag & drop para mostrarlas en orden de relevancia, y editarlas o eliminarlas según sea necesario. Las recompensas sin stock disponible se mostrarán como "Agotadas" en la vista pública, y las que ya tienen backings asociados no podrán eliminarse para preservar la integridad de los compromisos con los fans.

Esta funcionalidad es crítica para incentivar las aportaciones y dar transparencia sobre qué obtiene cada fan según el nivel de apoyo que brinde a la campaña.

---

## User Story

**Como** artista
**Quiero** añadir recompensas a mi campaña
**Para** incentivar las aportaciones de los fans

---

## Flujo Principal

1. Artista autenticado accede al dashboard en `/dashboard/campanias/{id}/rewards`
2. El sistema muestra la lista de recompensas existentes (vacía si es nueva campaña)
3. Artista hace clic en botón "Añadir recompensa"
4. Se abre un modal/panel con formulario de creación de recompensa
5. Artista completa los campos del formulario:
   - Nombre (obligatorio, máx 200 caracteres)
   - Descripción (opcional, máx 2000 caracteres)
   - Monto mínimo (obligatorio, > 0, en EUR)
   - Stock limitado (checkbox)
   - Cantidad máxima (si stock limitado está activo, opcional)
   - Tiempo de entrega estimado (texto libre, opcional, máx 200 caracteres)
6. Sistema valida los datos mediante `CreateRewardCommandValidator`
7. Si validación falla, muestra errores inline en formulario
8. Si validación es exitosa, ejecuta `CreateRewardCommand` vía MediatR
9. Backend crea registro en BD con `Orden = max(Orden) + 1` para posicionarlo al final
10. Sistema actualiza la lista mostrando la nueva recompensa
11. Artista puede repetir el proceso para añadir más recompensas
12. Artista puede reordenar recompensas mediante drag & drop (ejecuta endpoint PUT `/api/campanias/{id}/rewards/reorder`)
13. Artista puede editar recompensas existentes (abre modal con datos prellenados, ejecuta `UpdateRewardCommand`)
14. Artista puede eliminar recompensas (si no tienen backings asociados, ejecuta `DeleteRewardCommand`)

---

## Flujos Alternativos

| ID | Condición | Acción |
|----|-----------|--------|
| FA-01 | Intentar publicar campaña sin recompensas | Mostrar advertencia "Se recomienda añadir al menos 1 recompensa", permitir continuar |
| FA-02 | Stock de recompensa agotado (CantidadDisponible = 0) | Mostrar badge "Agotado" en vista pública, deshabilitar botón de selección |
| FA-03 | Intentar eliminar reward con backings existentes | Mostrar error "No se puede eliminar una recompensa con backings existentes", bloquear acción |
| FA-04 | Intentar editar recompensa mientras hay backings | Permitir editar Descripcion y TiempoEntregaEstimado, bloquear cambios en ImporteMinimo y CantidadMaxima |
| FA-05 | Validation errors en formulario | Mostrar mensajes de error inline debajo de cada campo afectado |

---

## Criterios de Aceptación

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-03-1 | Al intentar publicar campaña sin rewards, se muestra advertencia pero permite continuar | Backend (validación en PublishCampaniaCommand), Admin (UI warning) |
| AC-03-2 | El campo ImporteMinimo debe ser > 0 y validarse en backend y frontend | Backend (CreateRewardCommandValidator), Admin (Zod schema) |
| AC-03-3 | Al crear un backing, el stock de la recompensa se decrementa (CantidadVendida++) | Backend (CreateBackingCommand) |
| AC-03-4 | No se puede seleccionar una recompensa si CantidadDisponible = 0 | Landing (UI disable), Backend (validación en CreateBackingCommand) |
| AC-03-5 | Las recompensas tienen campo Orden y se pueden reordenar mediante drag & drop | Backend (ReorderRewardsCommand), Admin (UI drag & drop) |
| AC-03-6 | No se puede eliminar una recompensa con backings asociados (soft delete EsActivo=false) | Backend (DeleteRewardCommand con validación) |
| AC-03-7 | Las recompensas se muestran en orden ascendente por campo Orden en vistas públicas | Landing (ordenamiento en query) |
| AC-03-8 | Validación de Nombre (required, max 200), Descripcion (max 2000), TiempoEntregaEstimado (max 200) | Backend (validators), Admin/Shared (Zod schemas) |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | CQRS commands (Create, Update, Delete, Reorder), Validators, Service methods, Controller endpoints | ALTO - Lógica de negocio, validaciones críticas, decrementar stock |
| **Admin** | Dashboard UI: formulario de recompensas, lista con drag & drop, modals de edición/eliminación | ALTO - Interfaz completa de gestión de rewards |
| **Landing** | Vista pública de recompensas en detalle de campaña, mostrar badges "Agotado", deshabilitar selección | MEDIO - Solo lectura y visualización |
| **Shared** | Tipos TypeScript (Reward, RewardDto, CreateRewardDto, UpdateRewardDto), Zod schemas para validación | MEDIO - Compartido entre Admin y Landing |

---

## Dependencias

### Técnicas
- Identity/Auth: Validar que el usuario autenticado es el artista dueño de la campaña antes de permitir crear/editar/eliminar rewards
- Database: Tabla `CampaniaCrowdfundingReward` ya existe con los campos necesarios
- Strongly Typed IDs: `CampaniaCrowdfundingRewardId`, `CampaniaCrowdfundingId`
- Request Caching: Usar `IRequestCacheService` para evitar queries duplicados en validaciones

### De otras features
- **registro-artista (US-01)**: El artista debe tener cuenta activa para poder gestionar rewards
- **crear-campania (US-02)**: La campaña debe existir antes de poder añadirle recompensas
- **realizar-backing (US-04, futuro)**: Cuando se cree un backing, deberá decrementar el stock de la recompensa seleccionada (validar CantidadDisponible > 0)

---

## Notas Técnicas

### Backend - Estado Actual

La mayor parte del scaffolding backend YA EXISTE:
- ✅ Entidad `CampaniaCrowdfundingReward` con todos los campos necesarios
- ✅ `RewardService` con métodos CRUD
- ✅ `RewardRepository` con queries por CampaniaId
- ✅ `CreateRewardCommand` + Handler + Validator
- ✅ `UpdateRewardCommand` + Handler + Validator
- ✅ `DeleteRewardCommand` + Handler (soft delete con EsActivo=false)
- ✅ `GetAllRewardsQuery` (con filtros por CampaniaId, EsActivo, EsAddOn)
- ✅ `GetRewardByIdQuery`
- ✅ `RewardsController` con endpoints GET, POST, PUT, DELETE

### Backend - Lo que FALTA Implementar

1. **ReorderRewardsCommand**: Nuevo command para reordenar rewards mediante drag & drop
   - Endpoint: `PUT /api/campanias/{campaniaId}/rewards/reorder`
   - Request body: `{ "rewardIds": ["guid1", "guid2", "guid3"] }`
   - Actualizar el campo `Orden` de cada reward según el índice en el array

2. **Validación de ownership**: Añadir validación en todos los commands para verificar que el usuario autenticado es el artista dueño de la campaña (via ArtistaId)

3. **Validación en DeleteRewardCommand**: Verificar que la recompensa NO tiene backings asociados antes de permitir soft delete
   - Consultar `entity.Lineas.Any()` o añadir método `HasBackingsAsync()` al service
   - Si tiene backings, retornar error con código `BusinessRule_RewardHasBackings`

4. **Computed property CantidadDisponible**: Asegurar que se calcula correctamente en DTOs
   - Formula: `CantidadMaxima.HasValue ? CantidadMaxima - CantidadVendida : null`

5. **Auto-incrementar Orden**: Al crear nueva recompensa, calcular `Orden = max(rewards.Orden) + 1` automáticamente

6. **Route structure decision**: Los endpoints actuales usan `/api/rewards/{id}`, pero la User Story sugiere `/api/campanias/{id}/rewards`. Decidir cuál usar:
   - **Opción A (actual)**: `/api/rewards` - Más RESTful, rewards como recurso independiente
   - **Opción B (US)**: `/api/campanias/{id}/rewards` - Más jerárquico, rewards siempre bajo campaña
   - **Recomendación**: Mantener estructura actual `/api/rewards` y usar query param `?campaniaId=X` para filtrar (ya implementado)

### Frontend - Admin Dashboard

**Componentes a crear:**
- `RewardsListPage.tsx`: Página principal de gestión de recompensas
- `RewardCard.tsx`: Card individual con drag handle, botones Edit/Delete
- `RewardFormModal.tsx`: Modal con formulario de creación/edición
- `RewardDeleteDialog.tsx`: Confirmación de eliminación

**Hooks a crear:**
- `useRewards.ts`: `useQuery` para obtener recompensas de una campaña
- `useCreateReward.ts`: `useMutation` para crear recompensa
- `useUpdateReward.ts`: `useMutation` para actualizar recompensa
- `useDeleteReward.ts`: `useMutation` para eliminar recompensa
- `useReorderRewards.ts`: `useMutation` para reordenar recompensas

**Libraries necesarias:**
- `@dnd-kit/core` o `react-beautiful-dnd` para drag & drop de reordenamiento

### Frontend - Landing (Vista Pública)

**Componentes a crear:**
- `CampaniaRewardsSection.tsx`: Sección de recompensas en detalle de campaña
- `RewardPublicCard.tsx`: Card de recompensa en vista pública (con badge "Agotado")

**Hooks:**
- Reutilizar `useRewards.ts` con filtro `esActivo=true`

### Shared Types

Ya existen en `src/shared/types/reward.ts`:
- `Reward` (entidad completa)
- `RewardDto` (para respuestas)
- `CreateRewardDto` (para crear)
- `UpdateRewardDto` (para actualizar)

**Añadir:**
- `ReorderRewardsDto`: `{ rewardIds: string[] }`

### Schemas Zod

Actualizar `src/shared/schemas/reward.schema.ts`:
```typescript
export const createRewardSchema = z.object({
  campaniaId: z.string().uuid(),
  nombre: z.string().min(1, "Nombre requerido").max(200, "Máximo 200 caracteres"),
  descripcion: z.string().max(2000, "Máximo 2000 caracteres").optional(),
  importeMinimo: z.number().gt(0, "Debe ser mayor a 0"),
  cantidadMaxima: z.number().int().gt(0).optional(),
  tiempoEntregaEstimado: z.string().max(200, "Máximo 200 caracteres").optional(),
});
```

### Testing

**Backend (xUnit):**
- `CreateRewardCommandHandlerTests`: Validar creación con/sin stock limitado
- `UpdateRewardCommandHandlerTests`: Validar actualización
- `DeleteRewardCommandHandlerTests`: Validar que NO se elimina si tiene backings
- `ReorderRewardsCommandHandlerTests`: Validar reordenamiento
- `CreateRewardCommandValidatorTests`: Validar todas las reglas de validación

**Frontend (Vitest):**
- `RewardsListPage.test.tsx`: Renderizado de lista, drag & drop
- `RewardFormModal.test.tsx`: Validación de formulario
- `useCreateReward.test.ts`: Mutation de creación
- `reward.schema.test.ts`: Validaciones Zod

---

## Referencia

- User Story completa: [docs/product/US-03-definir-recompensas.md](../../product/US-03-definir-recompensas.md)
