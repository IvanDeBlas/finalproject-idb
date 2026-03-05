# Auditoría del Módulo CrowdFunding: Domain vs Implementación

- **Fecha**: 2026-02-14
- **Autor**: Ivan + Claude Code
- **Contexto**: Revisión de entidades del dominio CrowdFunding para identificar funcionalidades pendientes

---

## Resumen Ejecutivo

El módulo CrowdFunding tiene **13 entidades** en el Domain Model y **4 Controllers** con **38 archivos CQRS** en Application. Solo **5 entidades** tienen implementación completa (CQRS + Controller). Las **8 restantes** están modeladas en el dominio y configuradas en el DbContext (Fluent API), pero no tienen Commands, Queries, Services, Repositories ni Controllers.

| Estado | Cantidad | Entidades |
|--------|----------|-----------|
| Implementadas | 5 | CampaniaCrowdfunding, CampaniaCrowdfundingReward, PedidoCrowdfunding, AportacionCrowdfunding, Dashboard (virtual) |
| Sin implementar | 8 | CampaniaCrowdfundingUpdate, CampaniaCrowdfundingComentario, CampaniaCrowdfundingStretchGoal, PedidoCrowdfundingLinea, ArtistaPayoutCuenta, CampaniaCrowdfundingPayout, ArtistaMembershipPlan + Suscripcion + Pago |

---

## Entidades con Implementación Completa (5)

### 1. CampaniaCrowdfunding

**Ubicación Domain:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfunding.cs`

**Propiedades clave:** ArtistaId, Titulo, Subtitulo, DescripcionCorta, VideoPrincipalUrl, ImagenPrincipalUrl, ImporteObjetivo, ImportePledgedActual, EstadoCampaniaId, TipoFinanciacionId, FechaInicio, FechaFin, FechaPublicacion, FechaCierre, Borrado

**CQRS implementado:**
| Tipo | Archivo | Endpoint |
|------|---------|----------|
| Command | CreateCampaniaCommand | POST /api/campanias |
| Command | UpdateCampaniaCommand | PUT /api/campanias/{id} |
| Command | PublishCampaniaCommand | POST /api/campanias/{id}/publicar |
| Command | DeleteCampaniaCommand | DELETE /api/campanias/{id} |
| Query | GetAllCampaniasQuery | GET /api/campanias |
| Query | GetMisCampaniasQuery | GET /api/campanias/mis-campanias |
| Query | GetCampaniaByIdQuery | GET /api/campanias/{id} |
| Query | GetCampaniaDetailQuery | (interno) |
| Query | GetCampaniaStatsQuery | GET /api/campanias/{id}/stats |

**Controller:** `CampaniasController` — Incluye también endpoints de backings (CreateBacking, GetBackings) que delegan a features de Backings.

### 2. CampaniaCrowdfundingReward

**Ubicación Domain:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/CampaniaCrowdfundingReward.cs`

**Propiedades clave:** CampaniaId, TipoRewardId, Nombre, Descripcion, ImporteMinimo, CantidadMaxima, CantidadPorBacker, EsAddOn, EsActivo, Orden, IncluyeEnvioFisico, TiempoEntregaEstimado

**CQRS implementado:**
| Tipo | Archivo | Endpoint |
|------|---------|----------|
| Command | CreateRewardCommand | POST /api/rewards |
| Command | UpdateRewardCommand | PUT /api/rewards/{id} |
| Command | DeleteRewardCommand | DELETE /api/rewards/{id} |
| Command | ReorderRewardsCommand | PUT /api/rewards/reorder |
| Query | GetAllRewardsQuery | GET /api/rewards |
| Query | GetRewardByIdQuery | GET /api/rewards/{id} |

**Controller:** `RewardsController` — CRUD completo + reordenar.

### 3. PedidoCrowdfunding

**Ubicación Domain:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/PedidoCrowdfunding.cs`

**Propiedades clave:** CampaniaId, UserId (nullable para anónimos), FanProfileId, EstadoPedidoId, ImporteSubtotal, ImportePropina, ImporteEnvio, ImporteImpuestos, ImporteTotal, ComentarioBacker, PermitirMostrarNombre

**CQRS implementado:**
| Tipo | Archivo | Endpoint |
|------|---------|----------|
| Command | CreatePedidoCommand | POST /api/pedidos |
| Command | UpdatePedidoCommand | PUT /api/pedidos/{id} |
| Command | DeletePedidoCommand | DELETE /api/pedidos/{id} |
| Query | GetAllPedidosQuery | GET /api/pedidos |
| Query | GetPedidoByIdQuery | GET /api/pedidos/{id} |

**Controller:** `PedidosController` — CRUD completo.

### 4. AportacionCrowdfunding (parcial via Backings)

**Ubicación Domain:** `Modules/Crowdfunding/WePlayRises.Crowdfunding.Domain/Model/AportacionCrowdfunding.cs`

**Propiedades clave:** PedidoCrowdfundingId, ImporteTotal, ImporteComisionPlataforma, ImporteComisionPasarela, ImporteNetoArtista, EstadoAportacionId, CodigoOperacionPasarela

**CQRS:** Se gestiona indirectamente a través de CreateBackingCommand y GetBackingsByCampaniaQuery (endpoints en CampaniasController), no tiene controller propio.

### 5. Dashboard (entidad virtual)

**No es una entidad de dominio**, sino una Feature que agrega datos de Campanias, Pedidos y Backings.

**CQRS implementado:**
| Tipo | Archivo | Endpoint |
|------|---------|----------|
| Query | GetDashboardResumenQuery | GET /api/dashboard/resumen |
| Query | GetDashboardMisCampaniasQuery | GET /api/dashboard/mis-campanias |
| Query | GetDashboardCampaniaBackingsQuery | GET /api/dashboard/campanias/{id}/backings |
| Query | GetDashboardCampaniaStatsQuery | GET /api/dashboard/campanias/{id}/stats |

**Controller:** `DashboardController` — Solo queries, todo autenticado.

---

## Entidades SIN Implementación (8)

Estas entidades están definidas en el Domain Model y configuradas en `CrowdfundingContext` con Fluent API, pero **no tienen**: Commands, Queries, Validators, DTOs, Services, Repositories ni Controllers.

### Grupo A: Relevantes para MVP (alto impacto en demo)

#### A1. CampaniaCrowdfundingUpdate ⭐ RECOMENDADA

**Ubicación:** `Domain/Model/CampaniaCrowdfundingUpdate.cs`

**Propiedades:**
```csharp
Guid Id
CampaniaCrowdfundingId CampaniaId
string Titulo
string Contenido
bool EsPublico
bool SoloBackers
DateTime FechaCreacion
DateTime? FechaActualizacion
```

**Qué aporta:** Los artistas publican actualizaciones de progreso en sus campañas (como en Kickstarter: "Acabamos de grabar la batería!", "Video preview del primer single"). Los fans ven las actualizaciones en la página de detalle de la campaña.

**Esfuerzo estimado:** ~1.5h (backend CQRS + frontend básico)

**Impacto en demo:** ALTO — Muestra que la plataforma tiene comunicación artista-fan y da vida a las campañas.

**Navegación:** `CampaniaCrowdfunding.Updates` (1:N)

#### A2. CampaniaCrowdfundingComentario

**Ubicación:** `Domain/Model/CampaniaCrowdfundingComentario.cs`

**Propiedades:**
```csharp
Guid Id
CampaniaCrowdfundingId CampaniaId
string UserId
Guid? ComentarioPadreId        // Hilos de respuesta
string Contenido
bool EsRespuestaArtista
DateTime FechaCreacion
DateTime? FechaActualizacion
bool Borrado
```

**Qué aporta:** Sistema de comentarios con hilos (respuestas anidadas), distinción de respuestas del artista, y borrado lógico.

**Esfuerzo estimado:** ~2h (hilos de comentarios son más complejos)

**Impacto en demo:** MEDIO-ALTO — Los comentarios dan vida a la comunidad pero los hilos requieren UI compleja.

#### A3. CampaniaCrowdfundingStretchGoal

**Ubicación:** `Domain/Model/CampaniaCrowdfundingStretchGoal.cs`

**Propiedades:**
```csharp
Guid Id
CampaniaCrowdfundingId CampaniaId
string Titulo
string? Descripcion
decimal ImporteObjetivo
int? MonedaId
int Orden
bool Alcanzado
DateTime? FechaAlcanzado
DateTime FechaCreacion
```

**Qué aporta:** Objetivos adicionales que se desbloquean al superar la meta principal ("Si llegamos a 150%, añadimos remix exclusivo").

**Esfuerzo estimado:** ~1.5h

**Impacto en demo:** MEDIO — Concepto conocido pero requiere que haya campañas cerca del 100% para ser visual.

---

### Grupo B: Post-MVP (funcionalidad financiera)

#### B1. PedidoCrowdfundingLinea

**Ubicación:** `Domain/Model/PedidoCrowdfundingLinea.cs`

**Propiedades:** PedidoCrowdfundingId, RewardId, Cantidad, PrecioUnitario, ImporteLinea, EsRewardPrincipal

**Qué aporta:** Desglose de líneas dentro de un pedido (reward principal + add-ons). Ya está modelado pero los pedidos se crean sin líneas detalladas actualmente.

**Esfuerzo:** ~1h | **Impacto:** BAJO (infraestructura interna, no visible al usuario)

#### B2. ArtistaPayoutCuenta

**Ubicación:** `Domain/Model/ArtistaPayoutCuenta.cs`

**Propiedades:** ArtistaId, NombreCuenta, TipoCuenta, ProveedorPayout, IBAN, NombreTitular, EsPorDefecto

**Qué aporta:** Gestión de cuentas bancarias del artista para recibir pagos.

**Esfuerzo:** ~1.5h | **Impacto:** BAJO para demo (funcionalidad de backoffice)

#### B3. CampaniaCrowdfundingPayout

**Ubicación:** `Domain/Model/CampaniaCrowdfundingPayout.cs`

**Propiedades:** CampaniaId, ArtistaPayoutCuentaId, ImporteBruto, ImporteComisionPlataforma, ImporteComisionPasarela, ImporteImpuestosRetenidos, ImporteNetoArtista, EstadoPayoutId

**Qué aporta:** Registro de pagos al artista después de cerrar la campaña exitosamente.

**Esfuerzo:** ~2h | **Impacto:** BAJO para demo (proceso de liquidación post-campaña)

---

### Grupo C: Feature independiente (Membresías)

#### C1. ArtistaMembershipPlan

**Ubicación:** `Domain/Model/ArtistaMembershipPlan.cs`

**Propiedades:** ArtistaId, NombrePlan, Descripcion, ImporteMensual, Nivel, EsActivo

#### C2. ArtistaMembershipSuscripcion

**Ubicación:** `Domain/Model/ArtistaMembershipSuscripcion.cs`

**Propiedades:** ArtistaId, MembershipPlanId, UserId, FechaInicio, FechaFin, ProximoCargo, EsActiva

#### C3. ArtistaMembershipPago

**Ubicación:** `Domain/Model/ArtistaMembershipPago.cs`

**Propiedades:** MembershipSuscripcionId, ImporteTotal, ImporteComisionPlataforma, ImporteNetoArtista, EstadoAportacionId

**Qué aportan (conjunto):** Sistema de suscripciones tipo Patreon donde fans pagan mensualmente al artista por contenido exclusivo (niveles: Free, Premium, VIP).

**Esfuerzo:** ~4h (las 3 entidades juntas) | **Impacto:** ALTO como feature pero fuera de alcance del sprint actual.

---

## Matriz de Priorización

| Entidad | Grupo | Esfuerzo | Impacto Demo | Impacto Funcional | Recomendación |
|---------|-------|----------|-------------|-------------------|---------------|
| **CampaniaCrowdfundingUpdate** | A | 1.5h | ⭐⭐⭐ | Alto | **IMPLEMENTAR** |
| CampaniaCrowdfundingComentario | A | 2h | ⭐⭐ | Alto | Posponer |
| CampaniaCrowdfundingStretchGoal | A | 1.5h | ⭐⭐ | Medio | Posponer |
| PedidoCrowdfundingLinea | B | 1h | ⭐ | Bajo | Posponer |
| ArtistaPayoutCuenta | B | 1.5h | ⭐ | Bajo | Posponer |
| CampaniaCrowdfundingPayout | B | 2h | ⭐ | Bajo | Posponer |
| ArtistaMembershipPlan | C | 1.5h* | ⭐⭐⭐ | Alto | Fuera de sprint |
| ArtistaMembershipSuscripcion | C | 1.5h* | ⭐⭐⭐ | Alto | Fuera de sprint |
| ArtistaMembershipPago | C | 1h* | ⭐⭐ | Medio | Fuera de sprint |

*Las membresías solo tienen sentido como conjunto (4h total).

---

## Recomendación Final

**Implementar en este sprint:** `CampaniaCrowdfundingUpdate` (~1.5h)

**Razones:**
1. **Mayor ratio impacto/esfuerzo** — Solo 1.5h y transforma la percepción de la plataforma
2. **Visible en demo** — Las actualizaciones de progreso aparecen en el detalle de campaña (Landing) y se gestionan desde el dashboard (Admin)
3. **Patrón conocido** — Kickstarter, Indiegogo y todas las plataformas de crowdfunding tienen esta funcionalidad
4. **Entidad simple** — Solo 8 campos, sin relaciones complejas ni hilos anidados
5. **Complementa polish visual** — La pestaña "Actualizaciones" del detalle de campaña deja de estar vacía

**Plan de implementación detallado:** Ver sección agregada en `20260214_plan-final-sprint-8h.md`.

---

*Documento generado el 2026-02-14 como auditoría del módulo CrowdFunding de WePlay Rises.*
