# Validacion QA: cp-wallet-comisiones (Admin)

**Fecha:** 2026-03-02
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)
**Target:** src/admin

---

## 1. Resumen Ejecutivo

| Metrica | Valor |
|---------|-------|
| Total Requisitos AC | 12 |
| Cubiertos en Admin (MVP) | 0 |
| Parcialmente Cubiertos | 0 |
| No Cubiertos (fuera de scope MVP) | 0 |
| N/A Admin MVP | 12 |
| **Score de Cobertura MVP** | **N/A (0/0 requisitos de Admin en MVP)** |

**Estado:** APROBADO — Admin esta intencionalmente fuera de scope en MVP. No hay requisitos de Admin que validar en este sprint.

> **Nota de contexto:** La feature-spec define explicitamente que Admin no tiene responsabilidad en MVP. La tabla "Proyectos Involucrados" declara: *"Sin responsabilidad en MVP. Futuro: vista de administrador para ver wallets de promotores y marcar transacciones como Procesada/Pagada/Cancelada."* No existe ningun plan `frontend-plan.md`, `ui-design.md` ni `test-strategy.md` para Admin porque ningun AC de la feature involucra a `src/admin` en el sprint actual.

---

## 2. Criterios de Aceptacion — Tabla Completa

Todos los AC extraidos de `docs/user-stories/cp-wallet-comisiones/feature-spec.md`, con indicacion del proyecto propietario y el estado desde la perspectiva de Admin.

| ID | Criterio (resumen) | Proyecto Propietario | Estado Admin MVP |
|----|--------------------|----------------------|-----------------|
| AC-CP06-1 | Promotor ve saldo, moneda, total ganado, total retirado y minimo de retiro en /promotor/wallet | Backend + Landing | N/A (MVP) |
| AC-CP06-2 | Al registrar una conversion (US-CP-05), se crea PromotorWalletTransaccion con EsCredito=true y SaldoActual se incrementa atomicamente | Backend | N/A (MVP) |
| AC-CP06-3 | Al validar una tarea (US-CP-04), se crea PromotorWalletTransaccion con EsCredito=true e importe=ImporteReward; SaldoActual se incrementa atomicamente | Backend | N/A (MVP) |
| AC-CP06-4 | SaldoActual refleja correctamente suma de creditos menos suma de debitos; incrementa al acreditar y decrementa al cobrar | Backend | N/A (MVP) |
| AC-CP06-5 | Historial paginado de transacciones con filtros por tipo, estado y rango de fechas | Backend + Landing | N/A (MVP) |
| AC-CP06-6 | Promotor solicita cobro si SaldoActual >= minimo; crea transaccion EsCredito=false, EstadoTransaccionId=1, decrementa saldo en misma transaccion BD | Backend + Landing | N/A (MVP) |
| AC-CP06-7 | Solicitar cobro por importe mayor al SaldoActual retorna 400 sin modificar el saldo | Backend | N/A (MVP) |
| AC-CP06-8 | Transacciones de credito tienen PromoEventoId referenciando al PromoEvento origen; transacciones de debito tienen PromoEventoId nulo | Backend | N/A (MVP) |
| AC-CP06-9 | Todas las transacciones de credito se crean con EstadoTransaccionId=1 (Pendiente); cambio a estados posteriores es manual en MVP | Backend | N/A (MVP) |
| AC-CP06-10 | Sin transacciones, /promotor/wallet muestra empty state con mensaje y acceso a explorar programas; saldo = 0.00 | Landing | N/A (MVP) |
| AC-CP06-11 | Campo EsCredito (bool, NOT NULL) y PromoEventoId (Guid?, NULL) existen en PromotorWalletTransaccion con migracion EF Core | Backend | N/A (MVP) |
| AC-CP06-12 | Types TypeScript PromotorWalletDto, PromotorWalletTransaccionDto y SolicitarCobroRequest definidos en shared y reutilizados por Landing | Shared | N/A (MVP) |

**Leyenda de Estado Admin MVP:**
- **N/A (MVP):** El criterio de aceptacion no asigna ninguna responsabilidad a `src/admin` en el sprint actual. No es un gap; es una decision de alcance documentada en la feature-spec.

---

## 3. Matriz de Trazabilidad

### 3.1 Requisitos Funcionales — Admin

Ninguno de los 12 AC de la feature asigna trabajo a `src/admin` en MVP. La columna "Proyecto" de cada AC indica exclusivamente Backend, Landing o Shared.

| ID | Criterio | frontend-plan | ui-design | test-strategy | Estado Admin |
|----|----------|---------------|-----------|---------------|--------------|
| AC-CP06-1 | Ver wallet en /promotor/wallet | — | — | — | N/A (MVP) |
| AC-CP06-2 | Acreditacion automatica por conversion | — | — | — | N/A (MVP) |
| AC-CP06-3 | Acreditacion automatica por tarea | — | — | — | N/A (MVP) |
| AC-CP06-4 | Calculo correcto de SaldoActual | — | — | — | N/A (MVP) |
| AC-CP06-5 | Historial paginado con filtros | — | — | — | N/A (MVP) |
| AC-CP06-6 | Solicitud de cobro con validacion de saldo | — | — | — | N/A (MVP) |
| AC-CP06-7 | Error al solicitar cobro por importe excedido | — | — | — | N/A (MVP) |
| AC-CP06-8 | PromoEventoId en transacciones de credito | — | — | — | N/A (MVP) |
| AC-CP06-9 | Estado Pendiente inicial; cambio manual en MVP | — | — | — | N/A (MVP) |
| AC-CP06-10 | Empty state sin transacciones | — | — | — | N/A (MVP) |
| AC-CP06-11 | Campos EsCredito y PromoEventoId en entidad | — | — | — | N/A (MVP) |
| AC-CP06-12 | Types TypeScript en shared | — | — | — | N/A (MVP) |

### 3.2 Requisitos No Funcionales — Admin

Los cinco RNF de la feature (RNF-01 a RNF-05) cubren concurrencia optimista, configurabilidad del minimo de retiro, atomicidad de la acreditacion, limites de paginacion e inmutabilidad de transacciones. Todos son responsabilidad de Backend. Ninguno genera trabajo en `src/admin` en MVP.

| ID | Requisito | Proyecto Propietario | Estado Admin MVP |
|----|-----------|----------------------|-----------------|
| RNF-01 | Concurrencia optimista (RowVersion) en solicitud de cobro | Backend | N/A (MVP) |
| RNF-02 | Minimo de retiro configurable via appsettings (no hardcodeado) | Backend | N/A (MVP) |
| RNF-03 | Acreditacion atomica: PromotorWalletTransaccion + actualizacion saldo en una sola transaccion BD | Backend | N/A (MVP) |
| RNF-04 | Paginacion obligatoria en GET transacciones; default 10, max 50 | Backend | N/A (MVP) |
| RNF-05 | PromotorWalletTransaccion inmutables; no se eliminan fisicamente | Backend | N/A (MVP) |

---

## 4. Analisis de Gaps

### 4.1 Gaps Criticos

Ninguno. No existen requisitos de Admin en MVP que hayan quedado sin cubrir.

### 4.2 Gaps Mayores

Ninguno. La ausencia de planes de Admin no es un gap; es la implementacion correcta del alcance definido.

### 4.3 Gaps Menores

| ID | Observacion | Impacto | Recomendacion |
|----|-------------|---------|---------------|
| OBS-01 | El directorio `plans/cp-wallet-comisiones/frontend-admin/` no existia antes de este informe | Bajo | Mantener el directorio con este archivo de validacion como referencia para cuando se planifique la fase post-MVP |
| OBS-02 | AC-CP06-9 menciona explicitamente "el cambio a estados posteriores (Procesada, Pagada) es manual en MVP" sin definir que interfaz usara el administrador para hacer ese cambio | Bajo | Documentar en la user story de post-MVP que el administrador necesita una pantalla en `src/admin` para cambiar el estado de las transacciones; incluir ese requisito en el backlog como tarea diferida |
| OBS-03 | Los tipos TypeScript de Shared (AC-CP06-12: PromotorWalletDto, WalletTransaccionItem, SolicitarCobroRequest) seran consumibles por Admin en el futuro; estan correctamente definidos en `src/shared` | Bajo | No se requiere accion; la decision de definirlos en shared garantiza reutilizacion sin duplicacion cuando Admin los necesite |

---

## 5. Validacion de Tests

No aplica en MVP. No hay componentes, hooks ni servicios de Admin que probar para esta feature en el sprint actual.

| Criterio | Test Planificado Admin | Estado |
|----------|------------------------|--------|
| Todos los AC | — | N/A (MVP) |

---

## 6. Validacion de UI/UX

No aplica en MVP. No hay pantallas de Admin planificadas para esta feature en el sprint actual.

| Screen Requerida Admin | Planificada MVP | Estado |
|------------------------|-----------------|--------|
| Vista wallets de promotores | No | N/A (MVP) |
| Cambio de estado de transacciones | No | N/A (MVP) |

---

## 7. Criterios de Aceptacion Diferidos a Post-MVP

Los siguientes ACs, en su dimension de Admin, estan explicitamente diferidos. Se documentan aqui como referencia para el sprint de post-MVP.

### AC diferidos que impactan Admin en el futuro

| ID AC Origen | Descripcion del trabajo futuro de Admin |
|--------------|----------------------------------------|
| AC-CP06-9 | Implementar en Admin la capacidad de cambiar el estado de una PromotorWalletTransaccion de Pendiente a Procesada, Pagada o Cancelada. En MVP el cambio es manual (directamente en BD); Admin necesitara una pantalla con tabla de transacciones por promotor y boton de cambio de estado |
| Nuevo AC futuro | Vista en Admin de wallets de todos los promotores: listado con promotorNombre, saldoDisponible, totalGanado, totalRetirado, ultima transaccion |
| Nuevo AC futuro | Filtrado en Admin por estado de transaccion para identificar pagos pendientes de procesar |
| Nuevo AC futuro | Accion de bulk en Admin para marcar multiples transacciones como Procesadas o Pagadas en un solo paso |

---

## 8. Recomendaciones para la Implementacion Post-MVP de Admin

### Acciones Requeridas cuando Admin entre en scope

1. **Crear planes de Admin para la feature wallet-admin**
   - Archivo: `plans/cp-wallet-comisiones/frontend-admin/frontend-plan.md`
   - Cambio: Definir componentes `WalletPromotoresTable`, `TransaccionEstadoSelector`, `WalletDetailPanel`

2. **Disenar la pantalla de gestion de wallets**
   - Archivo: `plans/cp-wallet-comisiones/frontend-admin/ui-design.md`
   - Cambio: Tabla de wallets por promotor con saldo y estado, detalle de transacciones con acciones de cambio de estado

3. **Definir nuevos endpoints de Admin**
   - Archivo: `docs/user-stories/cp-wallet-comisiones/contracts.md` (nueva seccion Admin)
   - Cambio: `GET /api/crowdpromotion/admin/wallets` (lista wallets), `PATCH /api/crowdpromotion/admin/transacciones/{id}/estado` (cambio de estado)
   - Nota: Estos endpoints requieren rol `Admin` en la autorizacion (a diferencia de los endpoints actuales que usan el JWT del promotor)

4. **Agregar constantes de rutas Admin en shared**
   - Archivo: `src/shared/constants/index.ts`
   - Cambio: Agregar `APP_ROUTES.admin.wallets` y `API_ROUTES.crowdpromotion.adminWallets`

5. **Definir estrategia de tests de Admin**
   - Archivo: `plans/cp-wallet-comisiones/frontend-admin/test-strategy.md`
   - Cambio: Tests unitarios para `WalletPromotoresTable`, tests de integracion para el flujo de cambio de estado

### Accion Sugerida para MVP actual

1. **Crear tarea en backlog para post-MVP**
   - Archivo: `tasks/backlog.md`
   - Cambio: Agregar item "US-CP-06-Admin: Vista de administrador para wallets y cambio de estado de transacciones" con referencia a este informe como documentacion del alcance

---

## 9. Checklist de Validacion

### Requisitos Funcionales Admin MVP
- [x] Se verifico que ninguno de los 12 AC asigna trabajo a src/admin en MVP
- [x] La feature-spec confirma explicitamente que Admin esta fuera de scope
- [x] Los ACs diferidos a post-MVP estan identificados y documentados

### UI/UX Admin MVP
- [x] No se requieren pantallas de Admin en MVP
- [x] Las pantallas futuras de Admin estan identificadas en la seccion 7

### Testing Admin MVP
- [x] No se requieren tests de Admin en MVP
- [x] La estrategia de testing futura esta esbozada en la seccion 8

### Contratos Compartidos
- [x] Los tipos TypeScript en shared (AC-CP06-12) son reutilizables por Admin en el futuro sin modificacion
- [x] Las constantes ESTADO_WALLET_TRANSACCION y LABELS definidas en shared son suficientes para Admin cuando implemente la vista de cambio de estado

---

## 10. Conclusion

**Score Final de Cobertura MVP Admin:** N/A

El score no es calculable porque Admin tiene cero (0) criterios de aceptacion en el sprint actual. Esto no es un deficit; es el resultado correcto dado el alcance definido en la feature-spec.

**Veredicto:** APROBADO

**Razon:** Todos los ACs de la feature (`AC-CP06-1` a `AC-CP06-12`) pertenecen a Backend, Landing o Shared. Admin tiene cero requisitos en MVP. La declaracion explicita en la seccion "Proyectos Involucrados" de la feature-spec actua como la fuente de verdad: *"Sin responsabilidad en MVP."*

**Proximo Paso:**
- Proceder a la implementacion de Backend, Landing y Shared segun sus respectivos planes y validaciones de QA.
- Crear un ticket de backlog para la fase post-MVP de Admin con los ACs futuros identificados en la seccion 7 de este informe.

---

**Validado por:** qa-criteria-validator
**Fecha:** 2026-03-02
