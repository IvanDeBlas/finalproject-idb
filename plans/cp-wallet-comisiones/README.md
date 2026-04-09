# Plan: cp-wallet-comisiones (US-CP-06)

> **Feature:** Wallet de Promotor, Comisiones y Cobros
> **Generado:** 2026-03-02
> **Agentes ejecutados:** 13 (1 shared + 4 backend + 4 landing + 4 admin)

---

## Archivos Generados

### Shared (1 archivo)

| Archivo | Descripcion |
|---------|-------------|
| `shared/contracts-plan.md` | Types TypeScript (6 interfaces), Zod schemas (2), constantes (ESTADO_WALLET_TRANSACCION, MIN_RETIRO, QUERY_KEYS, API_ROUTES), error messages, mappers y formatters |

### Backend (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `backend/api-contracts.md` | DTOs C#, Commands/Queries como records, Validators FluentValidation, Controller methods, constantes ServiceResponseMessageType (2030, 4040-4042) |
| `backend/hexagonal-architecture.md` | Cambios a entidades (EsCredito, PromoEventoId, Descripcion, RowVersion), repositorio IPromotorWalletTransaccionRepository, servicio con logica de cobro y concurrencia optimista, migracion EF Core |
| `backend/cqrs-plan.md` | GetPromotorWalletQuery, GetWalletTransaccionesQuery, SolicitarCobroCommand con Handlers, Validators, AutoMapper Profile. Patron CobroError como resultado discriminado |
| `tests/newman/WePlay.WalletComisiones.IntegrationTests.json` | Coleccion Postman v2.1 ejecutable: 20 requests, 120 assertions, 7 folders. Cobertura de 3 endpoints con casos de exito, validacion, errores de negocio y auth |

### Frontend Landing (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-landing/frontend-plan.md` | Arquitectura hexagonal: 8 componentes, 3 hooks (usePromotorWallet, useWalletTransacciones, useSolicitarCobro), 1 service, integracion con router |
| `frontend-landing/ui-design.md` | 12 componentes shadcn/ui con tokens de diseno, layout responsive, accesibilidad WCAG AA, skeletons, empty states, dialog de cobro |
| `frontend-landing/test-strategy.md` | 62 tests planificados (47 unit + 15 integration), mocks, cobertura 80%+ |
| `frontend-landing/qa-validation.md` | Validacion de ACs: 6 criterios aplican a Landing, gaps identificados (soft check importe, discrepancia HTTP 400 vs 409) |

### Frontend Admin (4 archivos)

| Archivo | Descripcion |
|---------|-------------|
| `frontend-admin/frontend-plan.md` | FUERA DE SCOPE MVP. Plan preparatorio: 8 componentes, 3 hooks, 4 endpoints admin-especificos. Estimacion: 23-37h |
| `frontend-admin/ui-design.md` | FUERA DE SCOPE MVP. Componentes shadcn/ui para gestion de wallets y cambio de estado de transacciones |
| `frontend-admin/test-strategy.md` | FUERA DE SCOPE MVP. 40 tests planificados para implementacion futura |
| `frontend-admin/qa-validation.md` | Admin tiene 0 ACs en MVP. 4 ACs futuros documentados |

---

## Resumen de Decisiones Arquitectonicas

1. **MinimoRetiro configurable** via `IConfiguration["Crowdpromotion:MinimoRetiro"]` con fallback 10.0m (RNF-02)
2. **Concurrencia optimista** con RowVersion en PromotorWallet para prevenir doble debito (RN-05)
3. **CobroError enum** como resultado discriminado del Service, mapeado a ServiceResponseMessageType en el Handler
4. **Validacion de importe vs saldo** en Service (negocio), no en Validator (solo formato)
5. **HTTP 409** para errores de negocio (4040, 4041, 4042), no 400
6. **Migracion unica** `AddWalletTransaccionFieldsAndRowVersion` para los 3 campos nuevos + RowVersion
7. **Admin diferido** a post-MVP; toda la funcionalidad de gestion de estados es manual en BD

---

## Siguiente Paso

```bash
/implement cp-wallet-comisiones --target all
```

O implementar por partes:

```bash
/implement cp-wallet-comisiones --target shared    # Types, schemas, constantes
/implement cp-wallet-comisiones --target backend   # Domain, Infra, Application, Controller
/implement cp-wallet-comisiones --target landing   # Pagina wallet, hooks, service, componentes
/implement cp-wallet-comisiones --target admin     # DIFERIDO (fuera de MVP)
```

**Orden recomendado:** shared -> backend -> landing
