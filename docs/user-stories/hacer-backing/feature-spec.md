# Feature: Hacer Backing (Apoyar Campaña)

> **ID:** hacer-backing
> **User Story:** US-04
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripción

Esta feature permite a los fans apoyar económicamente las campañas de crowdfunding de sus artistas favoritos, a cambio de recompensas o simplemente como donación. El backing es el núcleo del modelo de negocio de la plataforma, donde los fans contribuyen con dinero para ayudar al artista a alcanzar su meta de financiación.

Los fans pueden explorar campañas activas en la landing pública, ver el progreso de cada campaña (monto recaudado vs objetivo), revisar las recompensas disponibles, y realizar su aportación seleccionando un nivel de recompensa o apoyando sin recompensa. El sistema permite backings anónimos (donde el nombre no aparece públicamente) y acepta mensajes de apoyo para el artista.

En el MVP, el flujo de pago se simula sin integración con pasarela real, pero el sistema registra el pedido (PedidoCrowdfunding), actualiza las métricas de la campaña (ImportePledgedActual), decrementa el stock de las recompensas seleccionadas, y crea un registro de aportación simulada. Esta funcionalidad es crítica porque sin backings, no hay financiación, y por tanto no hay crowdfunding.

---

## User Story

**Como** fan
**Quiero** apoyar una campaña
**Para** ayudar al artista y obtener recompensas

---

## Flujo Principal

1. Fan accede a la landing pública en `/campanias`
2. Sistema muestra listado de campañas activas (PUBLICADA o EN_CURSO) con:
   - Imagen destacada
   - Título y nombre del artista
   - Barra de progreso (monto recaudado vs objetivo)
   - Porcentaje alcanzado
   - Días restantes hasta FechaFin
3. Fan hace clic en una campaña de interés
4. Sistema muestra detalle completo de la campaña (`/campanias/{id}`):
   - Descripción completa (HTML rich text)
   - Video principal (embed YouTube/Vimeo)
   - Galería de imágenes
   - Barra de progreso grande y destacada
   - Información del artista con link a perfil
   - Estadísticas: monto recaudado, número de backers, días restantes
   - Lista de recompensas ordenadas por ImporteMinimo
   - Backings recientes (últimos 5-10 con nombre o "Anónimo" y monto)
5. Fan revisa las recompensas disponibles (cards con nombre, descripción, monto mínimo, stock disponible, tiempo de entrega estimado)
6. Fan selecciona una recompensa (o botón "Apoyar sin recompensa" si quiere donar sin recibir nada)
7. Sistema abre modal/panel de backing con:
   - Resumen de la recompensa seleccionada
   - Campo de monto (pre-rellenado con ImporteMinimo del reward, o campo libre si es sin recompensa)
   - Validación inline: monto >= ImporteMinimo del reward
   - Campo opcional: mensaje para el artista (textarea, máx 500 caracteres)
   - Checkbox: "Realizar aportación anónima" (oculta el nombre en la lista pública)
   - Resumen del pedido (monto, reward, subtotal = total en MVP)
8. Si el usuario NO está autenticado, sistema ofrece:
   - Opción A: Continuar como anónimo (sin crear cuenta)
   - Opción B: Iniciar sesión o registrarse para asociar el backing a su perfil
9. Fan completa los campos y hace clic en botón "Confirmar apoyo"
10. Sistema valida los datos mediante `CreateBackingCommandValidator` (monto >= mínimo, reward con stock disponible, campaña activa)
11. Si validación falla, muestra errores inline en el formulario
12. Si validación es exitosa, ejecuta transacción atómica:
    - Crea PedidoCrowdfunding con estado "Pendiente"
    - Crea PedidoCrowdfundingLinea asociada al reward seleccionado (o sin reward)
    - Actualiza ImportePledgedActual de la campaña (+= monto del backing)
    - Incrementa CantidadVendida del reward (si aplica)
    - Crea AportacionCrowdfunding simulada con estado "Confirmado" (MVP sin pasarela real)
13. Sistema muestra pantalla de confirmación con:
    - Mensaje de éxito "¡Gracias por tu apoyo!"
    - Resumen del backing (campaña, reward, monto, fecha)
    - Botones de acción: "Volver a la campaña", "Ver mis apoyos" (si autenticado), "Compartir en redes"
14. Sistema envía notificación al artista (email/in-app en futuro, log en MVP)

---

## Flujos Alternativos

| ID | Condición | Acción |
|----|-----------|--------|
| FA-01 | Monto ingresado < ImporteMinimo del reward | Mostrar error inline "El monto debe ser al menos {ImporteMinimo} EUR", bloquear envío de formulario |
| FA-02 | Stock de reward agotado (CantidadDisponible = 0) | Deshabilitar selección del reward, mostrar badge "Agotado", sugerir otras recompensas disponibles |
| FA-03 | Campaña ya finalizó (FechaFin < fecha actual o estado = FINALIZADA) | Redirigir a listado de campañas, mostrar toast "Esta campaña ya finalizó" |
| FA-04 | Usuario no autenticado al hacer clic "Apoyar" | Mostrar diálogo con opciones: "Continuar como anónimo" / "Iniciar sesión" / "Registrarse" |
| FA-05 | Error en transacción de creación de backing | Rollback completo, mostrar mensaje "Hubo un error al procesar tu apoyo, intenta nuevamente", loggear error |
| FA-06 | Campaña cambia de estado a FINALIZADA mientras el fan completa el formulario | Validar estado al enviar, retornar error "La campaña ya finalizó", bloquear creación |
| FA-07 | Dos fans intentan comprar el último reward disponible simultáneamente (race condition) | Usar optimistic locking o transacción con SELECT FOR UPDATE, uno tiene éxito y el otro recibe error "Stock agotado" |

---

## Criterios de Aceptación

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-04-1 | Solo campañas con estado PUBLICADA o EN_CURSO son visibles en listado público | Backend (GetAllCampaniasQuery con filtro por estadoCampaniaId), Landing (query por defecto estadoCampaniaId=2) |
| AC-04-2 | El monto ingresado debe ser >= ImporteMinimo del reward seleccionado | Backend (CreateBackingCommandValidator), Shared (backingSchema Zod), Landing (validación inline en formulario) |
| AC-04-3 | Al crear un backing, ImportePledgedActual de la campaña se actualiza sumando el monto aportado | Backend (CreateBackingCommand ejecuta update de Campania.ImportePledgedActual) |
| AC-04-4 | Al crear un backing con reward, CantidadVendida del reward se incrementa en 1 | Backend (CreateBackingCommand actualiza Reward.CantidadVendida++) |
| AC-04-5 | No se puede seleccionar un reward si CantidadDisponible = 0 (stock agotado) | Backend (validación en CreateBackingCommandValidator), Landing (UI deshabilita reward y muestra badge "Agotado") |
| AC-04-6 | Los backers que marcan "Anónimo" no muestran su nombre en la lista pública de backings recientes | Backend (query de backings recientes usa PermitirMostrarNombre para filtrar nombre), Landing (renderiza "Anónimo" si nombre no disponible) |
| AC-04-7 | La opción "Apoyar sin recompensa" permite cualquier monto > 0 sin seleccionar reward | Backend (validación permite RewardId = null y monto > 0), Landing (botón especial que no selecciona reward) |
| AC-04-8 | La creación del backing es una transacción atómica: PedidoCrowdfunding + PedidoCrowdfundingLinea + actualización Campania + actualización Reward + AportacionCrowdfunding | Backend (CreateBackingCommand usa Unit of Work o transacción explícita en Service) |
| AC-04-9 | Usuarios anónimos (no autenticados) pueden hacer backings con UserId = null | Backend (validación permite UserId nullable), Landing (permite submit sin auth) |
| AC-04-10 | La lista de campañas activas está paginada con página de 12 items por defecto | Backend (GetAllCampaniasQuery con pageSize default=12), Landing (paginación o infinite scroll) |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | CQRS: CreateBackingCommand (crea PedidoCrowdfunding + líneas + aportación + updates), GetCampaniaDetailQuery (detalle con rewards y backings recientes), Validators (monto, stock, estado), Services (transacción atómica), Controllers (endpoints públicos) | ALTO - Lógica de negocio compleja, transacciones atómicas, validaciones críticas de stock y estado |
| **Landing** | Página de listado de campañas (`/campanias`), página de detalle de campaña (`/campanias/{id}`), componentes de rewards (cards, selección), modal/formulario de backing, página de confirmación, hooks para queries y mutations | ALTO - Interfaz pública principal, flujo completo de usuario desde exploración hasta backing |
| **Admin** | (Opcional/Secundario) Dashboard con estadísticas de backings recibidos por campaña, lista de backers con mensajes | BAJO - Solo lectura de backings para el artista, no es crítico para MVP |
| **Shared** | Tipos TypeScript (Backing, PedidoDto, CreateBackingDto, BackingStats), Zod schemas para validación de formulario, mappers/utils para formateo de montos y fechas | MEDIO - Tipos compartidos entre Landing y Backend, schemas de validación |

---

## Dependencias

### Técnicas
- **Identity/Auth**: Autenticación opcional (permitir backings anónimos), validar UserId si está autenticado
- **Database**: Tablas `PedidoCrowdfunding`, `PedidoCrowdfundingLinea`, `AportacionCrowdfunding`, `CampaniaCrowdfunding`, `CampaniaCrowdfundingReward` ya existen
- **Strongly Typed IDs**: `PedidoCrowdfundingId`, `CampaniaCrowdfundingId`, `CampaniaCrowdfundingRewardId`, `AportacionCrowdfundingId`
- **Transacciones**: Usar Unit of Work o transacción explícita para atomicidad (backing + updates)
- **Request Caching**: Usar `IRequestCacheService` para evitar queries duplicados en validaciones (ej: cargar campaña y reward)
- **Optimistic Locking**: Considerar `RowVersion` en Reward para evitar race conditions en stock

### De otras features
- **registro-artista (US-01)**: El artista debe tener cuenta activa y campaña creada para recibir backings
- **crear-campania (US-02)**: La campaña debe existir y estar publicada (EstadoCampaniaId = 2 o 3) antes de recibir backings
- **definir-recompensas (US-03)**: Las recompensas deben estar creadas y activas (EsActivo=true) para poder ser seleccionadas en backings
- **publicar-campania (WPR-010, futuro)**: Transición de estado BORRADOR → PUBLICADA para habilitar backings públicos

---

## Notas Técnicas

### Backend - Estado Actual

**Lo que YA EXISTE:**
- ✅ Entidades de dominio: `PedidoCrowdfunding`, `PedidoCrowdfundingLinea`, `AportacionCrowdfunding`
- ✅ `CreatePedidoCommand` + Handler + Validator (básico, sin lógica de backing completa)
- ✅ `PedidoService` con métodos CRUD
- ✅ `PedidoRepository` con queries básicas
- ✅ `CampaniaService` con `GetByIdAsync`, `UpdateAsync`
- ✅ `RewardService` con `GetByIdAsync`, `UpdateAsync`
- ✅ `PedidosController` con POST endpoint
- ✅ `GetAllCampaniasQuery` con paginación y filtros (incluyendo estadoCampaniaId)
- ✅ `CampaniasController` con GET `/api/campanias` (público, filtra por estado)

**Lo que FALTA Implementar:**

1. **CreateBackingCommand**: Nuevo command específico para el flujo de backing público
   - Entrada: `CampaniaId`, `RewardId?` (nullable), `Monto`, `Mensaje?`, `EsAnonimo`, `UserId?` (nullable para anónimos)
   - Lógica:
     - Validar campaña activa (estado PUBLICADA o EN_CURSO) y dentro de fecha límite
     - Validar reward existe, está activo y tiene stock disponible (si se seleccionó)
     - Validar monto >= ImporteMinimo del reward (o > 0 si no hay reward)
     - Crear `PedidoCrowdfunding` con EstadoPedidoId = 1 (Pendiente)
     - Crear `PedidoCrowdfundingLinea` vinculando al reward (Cantidad=1, PrecioUnitario=Monto)
     - Actualizar `Campania.ImportePledgedActual += Monto`
     - Actualizar `Reward.CantidadVendida++` (si aplica)
     - Crear `AportacionCrowdfunding` simulada con EstadoAportacionId = 2 (Confirmado) y MetodoPagoId = 99 (Simulado/MVP)
     - Usar transacción atómica (DbContext.SaveChangesAsync al final)
   - Retorno: `ServiceResponse<BackingConfirmationDto>` con id del pedido, título campaña, nombre reward, monto, fecha

2. **CreateBackingCommandValidator**: Validaciones robustas con ServiceResponseMessageType constants
   - `RuleFor(x => x.CampaniaId).NotEmpty().WithErrorCode(ServiceResponseMessageType.Validation_Required)`
   - `RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0").WithErrorCode(ServiceResponseMessageType.Validation_MinValue)`
   - Validación asíncrona: cargar campaña vía cache y verificar estado activo
   - Validación asíncrona: cargar reward (si aplica) y verificar stock disponible
   - Validación asíncrona: verificar monto >= reward.ImporteMinimo (si aplica)
   - `RuleFor(x => x.Mensaje).MaximumLength(500).WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)`

3. **GetCampaniaDetailQuery**: Nueva query para detalle público de campaña con datos completos
   - Retorna `CampaniaDetailDto` con:
     - Datos base de campaña (título, subtítulo, descripción HTML, fechas, imágenes, video)
     - Datos de artista (id, nombreArtistico, imagenUrl)
     - Métricas: ImporteObjetivo, ImportePledgedActual, Porcentaje, NumBackers (count de pedidos), DíasRestantes (calculado desde FechaFin)
     - Lista de rewards activos ordenados por ImporteMinimo ASC (con CantidadDisponible calculada)
     - Backings recientes (últimos 10): nombre backer (o "Anónimo" si !PermitirMostrarNombre), monto, fecha relativa ("hace 2 horas")
   - Endpoint: GET `/api/campanias/{id}` (AllowAnonymous, ya existe pero devuelve `CampaniaDto` básico, extender o crear nuevo)

4. **BackingConfirmationDto**: Nuevo DTO para respuesta de confirmación
   ```csharp
   public class BackingConfirmationDto
   {
       public Guid PedidoId { get; set; }
       public string CampaniaTitulo { get; set; }
       public string? RewardNombre { get; set; }
       public decimal Monto { get; set; }
       public DateTime FechaCreacion { get; set; }
   }
   ```

5. **CampaniaDetailDto**: Extender DTO existente o crear nuevo con campos completos
   ```csharp
   public class CampaniaDetailDto
   {
       // Datos base (ya existen en CampaniaDto)
       public Guid Id { get; set; }
       public string Titulo { get; set; }
       public string? Subtitulo { get; set; }
       public string? Descripcion { get; set; }  // HTML rich text
       public string? ImagenPrincipalUrl { get; set; }
       public string? VideoPrincipalUrl { get; set; }

       // Artista
       public ArtistaBasicDto Artista { get; set; }

       // Métricas
       public decimal ImporteObjetivo { get; set; }
       public decimal ImportePledgedActual { get; set; }
       public int Porcentaje { get; set; }  // Calculado
       public int NumBackers { get; set; }  // Count de pedidos
       public int DiasRestantes { get; set; }  // Calculado
       public DateTime FechaFin { get; set; }

       // Recompensas
       public List<RewardPublicDto> Rewards { get; set; }

       // Backings recientes
       public List<BackingRecenteDto> BackingsRecientes { get; set; }
   }
   ```

6. **BackingRecenteDto**: Nuevo DTO para mostrar backings recientes en detalle de campaña
   ```csharp
   public class BackingRecenteDto
   {
       public string NombreBacker { get; set; }  // Nombre fan o "Anónimo"
       public decimal Monto { get; set; }
       public string FechaRelativa { get; set; }  // "hace 2 horas"
   }
   ```

7. **Transacción atómica en CreateBackingCommand**: Asegurar rollback completo si falla cualquier paso
   - Opción A: Usar DbContext transaction scope
   - Opción B: Mover toda la lógica al Service con Unit of Work pattern

8. **Optimistic Locking para Rewards**: Prevenir race conditions en stock
   - Agregar campo `RowVersion` a `CampaniaCrowdfundingReward` (si no existe)
   - Configurar en EntityTypeConfiguration: `builder.Property(x => x.RowVersion).IsRowVersion()`
   - EF Core lanzará `DbUpdateConcurrencyException` si dos requests intentan actualizar el mismo reward simultáneamente
   - Capturar excepción en Handler y retornar error "Stock agotado, intenta nuevamente"

9. **ServiceResponseMessageType Constants - Nuevos códigos necesarios:**
   ```csharp
   // Business Rules (4000-4999)
   public const string BusinessRule_BackingMontoInsuficiente = "4011";  // NUEVO
   public const string BusinessRule_RewardSinStock = "4012";  // NUEVO
   public const string BusinessRule_CampaniaFinalizada = "4013";  // NUEVO (si no existe)
   ```

10. **Controller endpoint público para crear backing:**
    ```csharp
    // En CampaniasController (no PedidosController, porque es acción pública sobre campaña)
    [HttpPost("{id}/backings")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateBacking(
        Guid id,
        [FromBody] CreateBackingRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        var command = new CreateBackingCommand
        {
            CampaniaId = id,
            UserId = userId,
            RewardId = request.RewardId,
            Monto = request.Monto,
            Mensaje = request.Mensaje,
            EsAnonimo = request.EsAnonimo ?? false
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetBacking), new { id = result.Data.PedidoId }, result);
    }
    ```

### Backend - AutoMapper Profiles

Crear/actualizar profiles:
- `PedidoProfile.cs`: Mapear `CreateBackingCommand → PedidoCrowdfunding`, `PedidoCrowdfunding → BackingConfirmationDto`
- `CampaniaProfile.cs`: Extender con mapeo `CampaniaCrowdfunding → CampaniaDetailDto` (incluir cálculos de Porcentaje, DiasRestantes, NumBackers)

### Frontend - Landing (Vista Pública)

**Componentes a crear:**

1. `CampaniasListPage.tsx`: Página principal de exploración (`/campanias`)
   - Grid responsivo de `CampaniaCard` (3 cols desktop, 2 tablet, 1 mobile)
   - Barra de búsqueda (search term)
   - Paginación o infinite scroll
   - Loading states y error states

2. `CampaniaCard.tsx`: Card individual en listado
   - Imagen destacada (aspect ratio 16:9)
   - Título y nombre artista
   - Barra de progreso con porcentaje
   - "€X recaudados de €Y meta"
   - Badge con días restantes
   - Hover effect y click redirige a detalle

3. `CampaniaDetailPage.tsx`: Página de detalle completa (`/campanias/{id}`)
   - Hero section con imagen/video grande
   - Sidebar sticky con:
     - Stats (recaudado, objetivo, backers, días)
     - Barra de progreso prominente
     - Botón CTA "Apoyar este proyecto"
   - Main content:
     - Tabs: "Descripción", "Recompensas", "Apoyos", "Artista"
     - Rich text rendering para descripción HTML
     - `CampaniaRewardsSection` con lista de rewards
     - `BackingsRecentesList` con últimos backings

4. `CampaniaRewardsSection.tsx`: Sección de recompensas
   - Lista de `RewardPublicCard` ordenados por monto
   - Card especial "Apoyar sin recompensa" al final

5. `RewardPublicCard.tsx`: Card de reward en vista pública
   - Nombre y descripción
   - Monto mínimo destacado
   - Stock disponible (badge "Quedan X" o "Agotado")
   - Tiempo de entrega estimado
   - Botón "Seleccionar" (disabled si stock = 0)
   - Estado seleccionado (border highlight)

6. `BackingModal.tsx`: Modal/panel de confirmación de backing
   - Resumen del reward seleccionado
   - Input de monto (type="number", min={rewardMinimo})
   - Validación inline (monto < mínimo → error)
   - Textarea para mensaje (maxLength=500, contador de caracteres)
   - Checkbox "Realizar aportación anónima"
   - Resumen: "Total: €X"
   - Botón "Confirmar apoyo" (loading state durante submit)

7. `BackingConfirmationPage.tsx`: Página de confirmación post-backing (`/campanias/{id}/backing-confirmado`)
   - Icono success grande
   - Mensaje "¡Gracias por tu apoyo!"
   - Resumen del backing (campaña, reward, monto, fecha)
   - Botones:
     - "Volver a la campaña"
     - "Ver mis apoyos" (si autenticado)
     - "Compartir" (social share)

8. `BackingsRecentesList.tsx`: Lista de backings recientes en detalle
   - Muestra últimos 10 backings
   - Avatar placeholder + nombre (o "Anónimo")
   - Monto formateado
   - Fecha relativa ("hace 2 horas", usar library como `date-fns`)

**Hooks a crear:**

1. `useCampanias.ts`: Query para listado paginado
   ```typescript
   export const useCampanias = (params: CampaniasQueryParams) => {
     return useQuery({
       queryKey: ['campanias', params],
       queryFn: () => campaniaService.getAll(params),
     });
   };
   ```

2. `useCampaniaDetail.ts`: Query para detalle completo
   ```typescript
   export const useCampaniaDetail = (id: string) => {
     return useQuery({
       queryKey: ['campanias', id],
       queryFn: () => campaniaService.getById(id),
       enabled: !!id,
     });
   };
   ```

3. `useCreateBacking.ts`: Mutation para crear backing
   ```typescript
   export const useCreateBacking = () => {
     const queryClient = useQueryClient();

     return useMutation({
       mutationFn: (data: CreateBackingDto) => backingService.create(data),
       onSuccess: (result, variables) => {
         // Invalidar queries de campaña para refrescar stats
         queryClient.invalidateQueries({ queryKey: ['campanias', variables.campaniaId] });

         toast({
           title: "¡Apoyo confirmado!",
           description: `Has apoyado con €${variables.monto}`,
         });
       },
       onError: (error: ApiError) => {
         toast({
           title: "Error al procesar apoyo",
           description: error.message,
           variant: "destructive",
         });
       },
     });
   };
   ```

**Services a crear:**

1. `campania.service.ts`: Métodos para campanias públicas
   ```typescript
   export const campaniaService = {
     getAll: async (params: CampaniasQueryParams): Promise<PaginatedResponse<CampaniaListDto>> => {
       const { data } = await api.get('/campanias', { params });
       return data;
     },

     getById: async (id: string): Promise<CampaniaDetailDto> => {
       const { data } = await api.get(`/campanias/${id}`);
       return data.data;
     },
   };
   ```

2. `backing.service.ts`: Métodos para backings
   ```typescript
   export const backingService = {
     create: async (dto: CreateBackingDto): Promise<BackingConfirmationDto> => {
       const { data } = await api.post(`/campanias/${dto.campaniaId}/backings`, dto);
       return data.data;
     },
   };
   ```

**Routes:**
- `/campanias` - Listado público (AllowAnonymous)
- `/campanias/{id}` - Detalle público (AllowAnonymous)
- `/campanias/{id}/backing-confirmado?pedidoId={x}` - Confirmación (AllowAnonymous)

### Shared Types

**Actualizar en `src/shared/types/`:**

1. `backing.ts`: Revisar tipos existentes, añadir:
   ```typescript
   export interface BackingConfirmationDto {
     pedidoId: string
     campaniaTitulo: string
     rewardNombre?: string
     monto: number
     fechaCreacion: string
   }

   export interface BackingRecenteDto {
     nombreBacker: string
     monto: number
     fechaRelativa: string
   }
   ```

2. `campania.ts`: Añadir tipo de detalle completo
   ```typescript
   export interface CampaniaDetailDto extends CampaniaDto {
     subtitulo?: string
     descripcion?: string  // HTML
     imagenPrincipalUrl?: string
     videoPrincipalUrl?: string
     artista: ArtistaBasicDto
     importeObjetivo: number
     importePledgedActual: number
     porcentaje: number
     numBackers: number
     diasRestantes: number
     fechaFin: string
     rewards: RewardPublicDto[]
     backingsRecientes: BackingRecenteDto[]
   }

   export interface CampaniasQueryParams {
     page?: number
     pageSize?: number
     search?: string
     estadoCampaniaId?: number
   }
   ```

3. `reward.ts`: Añadir tipo público (sin datos internos sensibles)
   ```typescript
   export interface RewardPublicDto {
     id: string
     nombre: string
     descripcion?: string
     importeMinimo: number
     cantidadDisponible: number | null  // null = ilimitado
     tiempoEntregaEstimado?: string
     esActivo: boolean
   }
   ```

### Schemas Zod

**Actualizar `src/shared/schemas/backing.schema.ts`:**
```typescript
import { z } from "zod"

export const createBackingSchema = z.object({
  campaniaId: z.string().uuid("ID de campaña inválido"),
  rewardId: z.string().uuid("ID de reward inválido").optional().nullable(),
  monto: z.number()
    .min(1, "El monto mínimo es 1 EUR")
    .max(100000, "El monto máximo es 100,000 EUR"),
  mensaje: z.string()
    .max(500, "Máximo 500 caracteres")
    .optional(),
  esAnonimo: z.boolean().default(false),
}).refine(
  (data) => {
    // Validación custom: si hay rewardId, el monto debe ser >= al mínimo del reward
    // Esta validación se hace también en backend, pero ayuda en UX
    // Se implementa en el formulario cargando el reward y validando dinámicamente
    return true;
  },
  {
    message: "El monto debe ser mayor o igual al mínimo de la recompensa",
    path: ["monto"],
  }
);

export type CreateBackingFormData = z.infer<typeof createBackingSchema>
```

### Testing

**Backend (xUnit):**
- `CreateBackingCommandHandlerTests`:
  - `Handle_ValidCommand_CreatesBackingAndUpdatesMetrics()`
  - `Handle_WithReward_IncrementsRewardStock()`
  - `Handle_WithoutReward_AllowsAnyAmount()`
  - `Handle_AnonymousBacking_AllowsNullUserId()`
  - `Handle_CampaniaInactiva_ReturnsBusinessRuleError()`
  - `Handle_RewardSinStock_ReturnsBusinessRuleError()`
- `CreateBackingCommandValidatorTests`:
  - `Validate_MontoMenorAlMinimo_ReturnsValidationError()`
  - `Validate_MensajeSuperaMaxLength_ReturnsValidationError()`
  - `Validate_CampaniaNoExiste_ReturnsNotFoundError()`
- `GetCampaniaDetailQueryHandlerTests`:
  - `Handle_ValidId_ReturnsDetailWithRewardsAndBackings()`
  - `Handle_CampaniaSinBackings_ReturnsEmptyBackingsList()`

**Frontend (Vitest):**
- `CampaniasListPage.test.tsx`: Renderizado de grid, paginación
- `CampaniaDetailPage.test.tsx`: Renderizado de detalle, tabs, stats
- `BackingModal.test.tsx`: Validación de formulario, submit, estados de loading/error
- `useCreateBacking.test.ts`: Mutation exitosa, error handling, invalidación de cache
- `backing.schema.test.ts`: Validaciones Zod (monto mínimo, maxLength mensaje)

**E2E (Playwright - opcional para MVP):**
- Flujo completo: Landing → Listar campañas → Ver detalle → Seleccionar reward → Hacer backing → Ver confirmación
- Flujo anónimo: Hacer backing sin autenticación
- Flujo sin reward: Seleccionar "Apoyar sin recompensa"

---

## Referencia

- User Story completa: [docs/product/US-04-hacer-backing.md](../../product/US-04-hacer-backing.md)
