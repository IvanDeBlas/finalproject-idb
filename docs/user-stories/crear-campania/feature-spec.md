# Feature: Crear Campaña de Crowdfunding

> **ID:** crear-campania
> **User Story:** US-02
> **Status:** proposed
> **Priority:** Alta
> **Sprint:** 1

---

## Descripción

Esta feature permite a un artista autenticado crear una campaña de crowdfunding para financiar su proyecto musical. El flujo implementa un wizard de 4 pasos que guía al artista a través de la definición de información básica, configuración financiera, fechas de la campaña y contenido multimedia.

La campaña se crea inicialmente en estado BORRADOR, permitiendo al artista revisar y editar antes de publicar. Una vez publicada, la campaña cambia al estado PUBLICADA y queda visible en la landing page para que los fans puedan explorarla y realizar backings. Este estado inicial de borrador es crítico para que el artista pueda iterar sobre el contenido sin presión de visibilidad pública.

El wizard implementa validaciones en cada paso para garantizar que la información sea completa y válida antes de permitir la creación. El sistema valida reglas de negocio como meta mayor a cero y fechas dentro del rango permitido (7-60 días). La feature es la base para las funcionalidades posteriores de gestión de rewards y backings.

---

## User Story

**Como** artista
**Quiero** crear una campaña de crowdfunding
**Para** financiar mi proyecto musical

---

## Flujo Principal

1. Artista accede al dashboard en `/dashboard`
2. Artista hace clic en botón "Nueva Campaña" o navega directamente a `/dashboard/campanias/nueva`
3. Sistema verifica que artista tiene perfil completo (depende de US-01)
4. Sistema muestra wizard paso 1 - Información básica
5. Artista completa título (obligatorio, max 200 caracteres), subtítulo (opcional, max 100 caracteres) y descripción (obligatorio, rich text)
6. Artista hace clic en "Siguiente" y sistema valida campos obligatorios con schema Zod
7. Sistema muestra wizard paso 2 - Financiación
8. Artista define meta financiera (obligatorio, > 0), selecciona moneda (EUR por defecto) y tipo de financiación (Todo o nada / Flexible)
9. Artista hace clic en "Siguiente" y sistema valida monto positivo
10. Sistema muestra wizard paso 3 - Fechas
11. Artista define fecha inicio (opcional, por defecto al publicar) y fecha fin (obligatorio, mínimo 7 días y máximo 60 días desde hoy)
12. Artista hace clic en "Siguiente" y sistema valida rango de fechas
13. Sistema muestra wizard paso 4 - Media
14. Artista carga imagen principal (URL obligatoria) y video principal (URL YouTube/Vimeo opcional)
15. Artista hace clic en "Crear Campaña"
16. Sistema envía solicitud POST `/api/campanias` con token JWT
17. Backend valida datos, crea entidad CampaniaCrowdfunding en estado BORRADOR y retorna ID
18. Sistema redirige a `/dashboard/campanias/{id}/preview` mostrando vista previa
19. Vista previa muestra banner indicando "Campaña en borrador - No visible públicamente"
20. Artista puede hacer clic en "Publicar Ahora" o "Guardar y Editar Después"
21. Si hace clic en "Publicar Ahora", sistema envía POST `/api/campanias/{id}/publicar`
22. Backend cambia estado a PUBLICADA, establece fechaPublicacion y retorna confirmación
23. Sistema redirige a `/dashboard/campanias` con mensaje de éxito

---

## Flujos Alternativos

| ID | Condición | Acción |
|----|-----------|--------|
| FA-01 | Artista hace clic en "Guardar como borrador" en paso 4 | Sistema crea campaña en BORRADOR, redirige a `/dashboard/campanias` con mensaje "Campaña guardada. Puedes editarla cuando quieras" |
| FA-02 | Artista intenta publicar campaña sin recompensas (rewards) | Sistema muestra advertencia modal "No has creado recompensas. ¿Continuar sin recompensas?" con opciones "Agregar recompensas" (redirige a crear rewards) o "Publicar sin recompensas" (procede con publicación) |
| FA-03 | Fecha fin es menor a 7 días desde hoy | Validación Zod bloquea avance con error "La campaña debe durar al menos 7 días" |
| FA-04 | Fecha fin es mayor a 60 días desde hoy | Validación Zod bloquea avance con error "La campaña no puede durar más de 60 días" |
| FA-05 | Meta financiera es 0 o negativa | Validación Zod bloquea avance con error "La meta debe ser mayor a cero" |
| FA-06 | URL de imagen tiene formato inválido | Validación Zod bloquea creación con error "URL de imagen inválida" |
| FA-07 | Artista abandona wizard sin completar | Sistema descarta progreso (no persistir borrador parcial), artista debe reiniciar wizard |
| FA-08 | Artista hace clic en "Atrás" en cualquier paso | Sistema mantiene datos ya ingresados, muestra paso anterior con campos pre-completados |
| FA-09 | Token JWT expirado durante creación | Backend retorna 401, sistema redirige a login con mensaje "Sesión expirada. Inicia sesión nuevamente" |
| FA-10 | Otro artista intenta editar campaña ajena | Backend valida ownership por ArtistaId, retorna 403 Forbidden |

---

## Criterios de Aceptación

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-02-1 | Campaña creada tiene estado EstadoCampaniaId = 1 (BORRADOR) al momento de creación | Backend |
| AC-02-2 | Meta financiera debe ser mayor a cero (ImporteObjetivo > 0), validación en frontend y backend | Backend + Admin |
| AC-02-3 | Fecha fin debe ser al menos 7 días desde la fecha actual, validación Zod impide crear campaña fuera de rango | Admin + Backend |
| AC-02-4 | Fecha fin no puede superar 60 días desde la fecha actual, validación Zod impide crear campaña fuera de rango | Admin + Backend |
| AC-02-5 | Solo el artista dueño (ArtistaId de la campaña = ArtistaId del token JWT) puede editar la campaña. Otro artista obtiene 403 Forbidden | Backend |
| AC-02-6 | Al publicar campaña (POST `/api/campanias/{id}/publicar`), estado cambia a EstadoCampaniaId = 2 (PUBLICADA) y se establece fechaPublicacion | Backend |
| AC-02-7 | Campaña en estado BORRADOR NO es visible en landing page al listar campañas públicas (GET `/api/campanias` debe filtrar por estado PUBLICADA) | Backend + Landing |
| AC-02-8 | Campaña en estado PUBLICADA es visible en landing page en `/campanias/{id}` sin requerir autenticación | Landing + Backend |
| AC-02-9 | Artista puede editar campaña en estado BORRADOR mediante PUT `/api/campanias/{id}` con cualquier campo actualizado | Backend + Admin |
| AC-02-10 | Al crear campaña sin fecha inicio, backend establece FechaInicio = null. Al publicar, si sigue null, se establece FechaInicio = fechaPublicacion | Backend |
| AC-02-11 | Wizard muestra indicador de progreso (stepper) mostrando paso actual (1/4, 2/4, 3/4, 4/4) | Admin |
| AC-02-12 | Rich text editor para descripción permite formato básico (negrita, cursiva, listas, links) | Admin |
| AC-02-13 | Schemas Zod para CreateCampaniaDto y UpdateCampaniaDto están definidos en `shared` y son reutilizados en Admin | Shared + Admin |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar endpoints CRUD para campañas: POST `/api/campanias` (crear en BORRADOR), PUT `/api/campanias/{id}` (actualizar borrador), POST `/api/campanias/{id}/publicar` (cambiar estado a PUBLICADA). Validar ownership (ArtistaId del token = ArtistaId de la campaña). Validar reglas de negocio (meta > 0, fechas en rango). Configurar filtros de autorización. Refinar comandos existentes CreateCampaniaCommand, UpdateCampaniaCommand. Crear PublicarCampaniaCommand. | ALTO |
| **Admin** | Implementar wizard de 4 pasos en `/dashboard/campanias/nueva` con stepper horizontal. Crear formularios para cada paso con validación Zod. Implementar rich text editor para descripción (TipTap o Quill). Implementar vista previa en `/dashboard/campanias/{id}/preview`. Crear página de listado de campañas del artista en `/dashboard/campanias` mostrando estado (BORRADOR / PUBLICADA). Implementar botón "Publicar" que llama a endpoint de publicación. Gestionar estados de carga y errores con react-query. | ALTO |
| **Landing** | Implementar página pública de detalle de campaña en `/campanias/{id}` consumiendo GET `/api/campanias/{id}`. Mostrar título, subtítulo, descripción (HTML), meta, importe recaudado, fechas, imagen, video. Implementar listado de campañas públicas en `/explorar` filtrando solo estado PUBLICADA. | ALTO |
| **Shared** | Definir tipos TypeScript: CampaniaDto, CreateCampaniaDto, UpdateCampaniaDto, EstadoCampania (enum), TipoFinanciacion (enum). Definir schemas Zod: createCampaniaSchema, updateCampaniaSchema con validaciones de fechas, monto, longitudes de texto. Exportar constantes: MIN_DIAS_CAMPANIA = 7, MAX_DIAS_CAMPANIA = 60, MONEDA_DEFAULT = 'EUR'. | ALTO |

---

## Dependencias

### Técnicas
- **WPR-001**: Configuración de ASP.NET Core Identity y JWT para autenticación
- **WPR-002**: DbContext configurado con entidad CampaniaCrowdfunding y relaciones a Artista
- **WPR-003**: Endpoints base para campañas (scaffolding existente: CreateCampaniaCommand, CampaniaService)
- **WPR-004**: React Query configurado en Admin para mutations
- **WPR-005**: Axios interceptors para manejo de tokens JWT y errores 401/403

### De otras features
- **registro-artista (US-01)**: El artista debe tener cuenta y perfil creado antes de crear campañas. Backend debe validar que el UserId del token tiene un Artista asociado antes de permitir crear campaña.

---

## Notas Técnicas

- El backend ya tiene scaffolding inicial para CreateCampaniaCommand, CampaniaDto, CampaniaService y CampaniaRepository. Esta feature requiere refinar esos componentes para implementar las validaciones completas y agregar PublicarCampaniaCommand.
- La autorización debe implementarse mediante filtro de autorización personalizado que valida `ArtistaId` del token JWT contra `ArtistaId` de la entidad Campania. No usar solo `[Authorize]` de Identity.
- El wizard debe implementar persistencia local en localStorage para evitar pérdida de datos si el artista recarga la página durante la creación (opcional, pero recomendado para UX).
- Rich text editor: considerar TipTap (extensible, modern) o Quill (maduro, estable). Almacenar output como HTML sanitizado en backend.
- Validación de URLs de video: usar regex para detectar formato YouTube (`youtube.com/watch?v=`, `youtu.be/`) y Vimeo (`vimeo.com/`). Extraer videoId para embed.
- Estados de campaña: BORRADOR y PUBLICADA se gestionan manualmente por el artista. Estados posteriores (EN_CURSO, COMPLETADA_EXITO, COMPLETADA_FALLO) se gestionan automáticamente por jobs programados basados en fechas y meta alcanzada.
- La transición PUBLICADA → EN_CURSO ocurre automáticamente cuando `DateTime.Now >= FechaInicio`. Implementar job programado (WPR-012) para gestionar estas transiciones.

---

## Referencia

- User Story completa: [docs/product/US-02-crear-campania.md](../../product/US-02-crear-campania.md)
