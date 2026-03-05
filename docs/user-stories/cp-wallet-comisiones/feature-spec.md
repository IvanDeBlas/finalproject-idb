# Feature: Wallet de Promotor, Comisiones y Cobros

> **ID:** cp-wallet-comisiones
> **User Story:** US-CP-06
> **Status:** proposed
> **Priority:** Media
> **Sprint:** TBD

---

## Descripcion

Esta feature cierra el ciclo economico del modulo Crowdpromotion. Hasta US-CP-05, el sistema ya registra conversiones y acredita comisiones como transacciones en la wallet del promotor; esta feature hace esas transacciones visibles y accionables. El promotor puede acceder a su wallet para ver el saldo disponible, el total ganado historico, el total retirado y un historial paginado de cada transaccion individual, con indicadores visuales que distinguen creditos (comisiones ganadas) de debitos (retiros solicitados) y el estado de cada una (Pendiente, Procesada, Pagada, Cancelada).

La segunda funcion de la feature es la solicitud de cobro: cuando el saldo acumulado supera el minimo configurable (10 EUR en MVP), el promotor puede solicitar un retiro indicando el importe a cobrar. El sistema valida que el importe no exceda el saldo disponible, crea una transaccion de debito en estado Pendiente, decrementa el saldo en la wallet y responde con confirmacion. En MVP, el paso de Pendiente a Pagada es manual (el administrador marca el pago como realizado); la integracion con un procesador de pagos queda fuera de scope.

Sin esta feature los promotores no tienen visibilidad ni control sobre sus ganancias, lo que elimina el incentivo economico real que sostiene todo el modulo. La feature tambien introduce los cambios de modelo de dominio necesarios en `PromotorWalletTransaccion`: los campos `EsCredito` (bool) y `PromoEventoId` (FK) que US-CP-05 necesitaba implicar pero que no estaban aun definidos en la entidad.

---

## User Story

**Como** promotor que ha generado conversiones y completado tareas validadas
**Quiero** ver mi saldo acumulado, el historial de transacciones (comisiones ganadas y retiros) y solicitar el cobro de mis ganancias
**Para** cobrar las recompensas por mi trabajo de promocion

---

## Actores

| Actor | Descripcion |
|-------|-------------|
| Promotor | Usuario con perfil de promotor activo que ha acumulado comisiones via conversiones (US-CP-05) o tareas validadas (US-CP-04) |
| Sistema | Acredita comisiones automaticamente al registrar una conversion (backing referido) o al validar una tarea; actualiza el saldo de forma sincrona |

---

## Precondiciones

- El promotor tiene perfil activo con wallet creada (PromotorWallet con MonedaId asignado, creada en US-CP-01 o auto-creada en US-CP-04/US-CP-05)
- Existen transacciones de credito previas por conversiones (US-CP-05) o tareas validadas (US-CP-04), o la wallet esta en saldo cero
- La tabla maestra `Maestra_EstadoWalletTransaccion` tiene los datos seed cargados: 1=Pendiente, 2=Procesada, 3=Pagada, 4=Cancelada

---

## Flujo Principal: Ver Wallet y Saldo

```
1. Promotor autenticado accede a /promotor/wallet en la Landing
2. Sistema llama a GET /api/crowdpromotion/promotor/wallet con el JWT del promotor
3. Backend resuelve el PromotorId del claim del token
4. Backend retorna: walletId, moneda, saldoActual, totalCreditos, totalDebitos, minimoRetiro
5. Frontend muestra el panel de saldo: saldo disponible destacado, totales de ganado y retirado y boton "Solicitar cobro"
6. Sistema llama a GET /api/crowdpromotion/promotor/wallet/transacciones con page=1 y pageSize=10
7. Frontend muestra el historial de transacciones: importe con signo (+ credito / - debito), concepto, tipo de recompensa, estado con badge de color y fecha
8. Promotor puede filtrar el historial por tipo (credito/debito), estado y rango de fechas
9. Al cambiar filtros, Frontend repite la llamada al endpoint con los nuevos query params
10. Promotor puede paginar el historial
```

---

## Flujo Secundario: Acreditacion Automatica de Comisiones

```
1. Se produce una de las dos condiciones de origen:
   a. Conversion (backing referido): el modulo Crowdfunding registra un backing con codigo referido activo; el handler de US-CP-05 calcula la comision y llama al servicio de wallet
   b. Tarea validada: el artista valida un completado de PromoTareaPromotor; el handler de US-CP-04 obtiene el ImporteReward de la tarea
2. Backend obtiene (o crea) la PromotorWallet del promotor para la moneda correspondiente
3. Backend crea un registro PromotorWalletTransaccion con: EsCredito = true, Importe = comision calculada o ImporteReward, EstadoTransaccionId = 1 (Pendiente), PromoEventoId = FK al evento que origino la transaccion (si aplica), TipoRewardId = FK al tipo de recompensa
4. Backend incrementa SaldoActual en PromotorWallet y actualiza TotalGanado
5. La operacion de BD es atomica: la creacion de la transaccion y la actualizacion del saldo ocurren en la misma transaccion de base de datos
6. El promotor puede ver la nueva transaccion en su historial la proxima vez que cargue la pagina de wallet
```

---

## Flujo Secundario: Solicitar Cobro

```
1. Promotor hace click en "Solicitar cobro" desde la pagina de wallet
2. Sistema verifica saldo >= minimoRetiro (10 EUR); si no, el boton esta deshabilitado con aviso
3. Frontend muestra el dialogo de solicitud con: saldo disponible, minimo de retiro, campo importe (obligatorio) y campo descripcion (opcional)
4. Promotor ingresa el importe a retirar
5. Frontend valida en cliente: importe > 0 y importe <= saldoActual
6. Promotor confirma la solicitud
7. Frontend llama a POST /api/crowdpromotion/promotor/wallet/cobro con importe y descripcion
8. Backend valida: promotor autenticado, wallet activa, saldoActual >= importe, saldoActual >= minimoRetiro, importe > 0
9. Backend crea PromotorWalletTransaccion con EsCredito = false, Importe = importe solicitado, EstadoTransaccionId = 1 (Pendiente), descripcion ingresada
10. Backend decrementa SaldoActual y actualiza TotalRetirado en PromotorWallet usando concurrencia optimista (RowVersion)
11. Backend retorna: transaccionId, importe, moneda, estadoNombre = Pendiente, saldoRestante y fechaCreacion
12. Frontend muestra toast: "Solicitud de cobro registrada. Procesaremos tu pago en breve."
13. Frontend actualiza el saldo visible sin recargar la pagina completa
```

---

## Flujo Secundario: Historial de Transacciones con Filtros

```
1. Promotor accede a la seccion de historial dentro de /promotor/wallet
2. Sistema muestra el listado inicial sin filtros (todas las transacciones, paginado, mas reciente primero)
3. Promotor aplica filtro por tipo: Credito (EsCredito = true) o Debito (EsCredito = false)
4. Promotor aplica filtro por estado: selecciona uno de los estados de Maestra_EstadoWalletTransaccion
5. Promotor aplica filtro por rango de fechas: fechaDesde y fechaHasta
6. Frontend llama a GET /api/crowdpromotion/promotor/wallet/transacciones con los query params activos
7. Sistema retorna el listado filtrado con totalCount para la paginacion
8. Si el resultado esta vacio, se muestra empty state especifico del filtro activo
```

---

## Flujos Alternativos

| ID | Condicion | Accion |
|----|-----------|--------|
| FA-01 | Promotor sin ninguna transaccion en la wallet | Mostrar empty state con icono de wallet vacia, mensaje explicativo y boton "Explorar programas" |
| FA-02 | Saldo actual es 0 o menor al minimo de retiro | Boton "Solicitar cobro" deshabilitado; mostrar aviso con el minimo requerido |
| FA-03 | Importe de cobro solicitado excede el saldo actual | Backend retorna 400 Bad Request; Frontend muestra error en el campo y permite al promotor ajustar el importe |
| FA-04 | Dos solicitudes de cobro concurrentes sobre la misma wallet | Concurrencia optimista detecta conflicto de RowVersion; Backend retorna 409 Conflict; Frontend sugiere recargar y reintentar |
| FA-05 | Promotor desactivado (EsActivo = false en perfil) solicita cobro | Permitir la solicitud de cobro del saldo existente; el saldo acumulado pertenece al promotor aunque este desactivado |

---

## Criterios de Aceptacion

| ID | Criterio | Proyecto |
|----|----------|----------|
| AC-CP06-1 | El promotor autenticado puede acceder a /promotor/wallet y ver su saldo actual, moneda de la wallet, total ganado historico, total retirado y el minimo de retiro configurado | Backend + Landing |
| AC-CP06-2 | Al registrar una conversion (backing referido) via US-CP-05, el sistema crea automaticamente una PromotorWalletTransaccion con EsCredito = true, importe de la comision calculada, EstadoTransaccionId = 1 (Pendiente) y PromoEventoId con la referencia al evento; el SaldoActual del promotor se incrementa en el mismo acto | Backend |
| AC-CP06-3 | Al validar una tarea de promocion via US-CP-04, el sistema crea automaticamente una PromotorWalletTransaccion con EsCredito = true, importe = ImporteReward de la tarea, EstadoTransaccionId = 1 (Pendiente) y referencia al evento de validacion; el SaldoActual se incrementa en el mismo acto | Backend |
| AC-CP06-4 | El SaldoActual del PromotorWallet refleja correctamente la suma de todos los creditos menos la suma de todos los debitos; incrementa al acreditar una comision y decrementa al procesar un cobro | Backend |
| AC-CP06-5 | El promotor puede ver el historial de transacciones paginado (pageSize configurable, default 10) con filtros funcionales por tipo (esCredito true/false), por estadoTransaccionId y por rango de fechas (fechaDesde y fechaHasta); cada combinacion de filtros retorna solo las transacciones que cumplen los criterios | Backend + Landing |
| AC-CP06-6 | El promotor puede solicitar un cobro si su SaldoActual es mayor o igual al minimo de retiro (10 EUR); la solicitud crea una PromotorWalletTransaccion con EsCredito = false, EstadoTransaccionId = 1 (Pendiente) y decrementa el SaldoActual en la misma transaccion de BD | Backend + Landing |
| AC-CP06-7 | El intento de solicitar un cobro por un importe mayor al SaldoActual retorna 400 Bad Request con mensaje descriptivo; el saldo de la wallet no se modifica | Backend |
| AC-CP06-8 | Cada PromotorWalletTransaccion generada por una conversion o tarea tiene PromoEventoId con la referencia al PromoEvento que la origino; las transacciones de cobro (debito) tienen PromoEventoId nulo | Backend |
| AC-CP06-9 | Todas las transacciones de tipo credito se crean con EstadoTransaccionId = 1 (Pendiente) en el momento de la acreditacion; el cambio a estados posteriores (Procesada, Pagada) es manual en MVP | Backend |
| AC-CP06-10 | Cuando el promotor no tiene transacciones, la pagina /promotor/wallet muestra un empty state con mensaje explicativo y acceso a explorar programas; el saldo se muestra como 0.00 | Landing |
| AC-CP06-11 | El campo EsCredito y el campo PromoEventoId existen en la entidad PromotorWalletTransaccion (con migracion de EF Core si no estan presentes); el campo EsCredito es obligatorio y el campo PromoEventoId es nullable | Backend |
| AC-CP06-12 | Los tipos TypeScript para PromotorWalletDto, PromotorWalletTransaccionDto y SolicitarCobroRequest estan definidos en shared y son reutilizados por Landing | Shared |

---

## Proyectos Involucrados

| Proyecto | Responsabilidad | Impacto |
|----------|-----------------|---------|
| **Backend** | Implementar los tres endpoints CQRS del modulo Crowdpromotion: GET promotor/wallet (query de saldo), GET promotor/wallet/transacciones (query de historial paginado con filtros), POST promotor/wallet/cobro (command de retiro). Agregar los campos EsCredito y PromoEventoId a la entidad PromotorWalletTransaccion con la migracion de EF Core correspondiente. Implementar la logica de validacion de saldo minimo, verificacion de importe vs saldo, y concurrencia optimista con RowVersion en la tabla PromotorWallet. Implementar validators CQRS con ServiceResponseMessageType para SolicitarCobroCommand. | ALTO |
| **Landing** | Implementar la pagina /promotor/wallet con el panel de saldo (saldoActual destacado, totalCreditos, totalDebitos, boton Solicitar cobro condicional al saldo), el historial de transacciones con indicadores visuales de tipo (verde/rojo con signo +/-) y badges de estado por color (Pendiente amarillo, Procesada verde, Pagada azul, Cancelada gris), los filtros de historial y la paginacion. Implementar el dialogo de solicitud de cobro con validacion client-side y toast de confirmacion. Implementar el empty state cuando no hay transacciones. | ALTO |
| **Admin** | Sin responsabilidad en MVP. Futuro: vista de administrador para ver wallets de promotores y marcar transacciones como Procesada/Pagada/Cancelada. | BAJO |
| **Shared** | Definir tipos TypeScript: PromotorWalletDto (walletId, monedaId, monedaNombre, saldoActual, totalCreditos, totalDebitos, minimoRetiro), PromotorWalletTransaccionDto (id, esCredito, importe, descripcion, estadoTransaccionId, estadoTransaccionNombre, tipoRewardId, tipoRewardNombre, promoEventoId, fechaCreacion, fechaActualizacion), SolicitarCobroRequest (importe, descripcion). Definir schema Zod para SolicitarCobroRequest. Agregar QUERY_KEYS para wallet y wallet-transacciones. Agregar constantes de estados de transaccion. | MEDIO |

---

## Reglas de Negocio

| Regla | Descripcion |
|-------|-------------|
| RN-01 | El minimo de retiro es 10 EUR por defecto; este valor debe ser configurable via appsettings o variable de entorno, no hardcodeado |
| RN-02 | El importe de una solicitud de cobro debe ser mayor que 0 y menor o igual al SaldoActual en el momento de procesar el comando |
| RN-03 | Solo el promotor dueno de la wallet puede ver sus transacciones y solicitar cobros; la autorizacion se verifica extrayendo el PromotorId del claim JWT |
| RN-04 | La acreditacion de una comision (por conversion o tarea) es atomica con la operacion que la origina: si falla la creacion de PromotorWalletTransaccion o la actualizacion del saldo, la operacion completa debe revertirse |
| RN-05 | La solicitud de cobro usa concurrencia optimista (RowVersion en PromotorWallet) para prevenir doble debito ante solicitudes concurrentes; si se detecta conflicto, el sistema retorna 409 Conflict |
| RN-06 | Los importes de las transacciones son siempre positivos; el tipo (credito o debito) se determina exclusivamente por el campo EsCredito |
| RN-07 | Los promotores desactivados conservan el derecho a solicitar cobro del saldo existente acumulado antes de la desactivacion |
| RN-08 | Las PromotorWalletTransaccion son inmutables una vez creadas; el cambio de estado (de Pendiente a Procesada, Pagada o Cancelada) en MVP es una operacion administrativa fuera de scope de esta feature |

---

## Cambios de Modelo de Dominio Requeridos

La entidad `PromotorWalletTransaccion` actual tiene los campos: Id, WalletId, TipoRewardId, EstadoTransaccionId, CampaniaPayoutId, Importe, Concepto, ReferenciaExterna, FechaCreacion, FechaProcesado.

Esta feature requiere agregar dos campos que la User Story presupone pero que no estan aun en la entidad:

- **EsCredito (bool, NOT NULL):** indica si la transaccion es un ingreso (true) o un retiro (false). Sin este campo el historial no puede distinguir el tipo de transaccion ni calcular correctamente el saldo.
- **PromoEventoId (Guid?, NULL):** FK a la tabla PromoEvento para trazabilidad del evento que origino la comision. Nulo para transacciones de cobro (debito). Sin este campo no se puede cumplir AC-CP06-8.

**Impacto en migraciones:**
- Se requiere una migracion aditiva de EF Core que agrega las dos columnas a la tabla PromotorWalletTransaccion
- EsCredito requiere un valor default en la migracion para las filas existentes (sugerido: true, asumiendo que las transacciones historicas son todas creditos de US-CP-04 y US-CP-05)
- PromoEventoId es nullable y no requiere valor default

---

## Dependencias

### Tecnicas

- ASP.NET Core Identity con JWT activo: el PromotorId se extrae del claim del token en todos los endpoints de esta feature
- Entidad `PromotorWallet` existente con campos SaldoActual, TotalGanado, TotalRetirado y MonedaId; requiere agregar RowVersion para concurrencia optimista si no esta presente
- Entidad `PromotorWalletTransaccion` existente; requiere agregar campos EsCredito y PromoEventoId via migracion de EF Core
- Tabla maestra `Maestra_EstadoWalletTransaccion` con datos seed cargados (ids 1 a 4)
- Minimo de retiro configurable en appsettings.json como `Crowdpromotion:MinimoRetiro` con valor default `10.0`

### De otras features

- **cp-perfil-promotor (US-CP-01):** provee la entidad `Promotor` con el PromotorId que vincula al promotor autenticado con su wallet
- **cp-tareas-promocion (US-CP-04):** es uno de los dos origenes de acreditacion automatica de creditos en wallet; los completados validados por el artista generan transacciones que esta feature expone en el historial
- **cp-tracking-metricas (US-CP-05):** es el otro origen de acreditacion automatica; las conversiones (backings referidos) generan transacciones de credito con PromoEventoId; esta feature es prerequisito directo porque sin los campos EsCredito y PromoEventoId en PromotorWalletTransaccion los handlers de US-CP-05 no pueden persistir correctamente las comisiones

---

## Requisitos No Funcionales

| ID | Requisito |
|----|-----------|
| RNF-01 | La solicitud de cobro debe usar concurrencia optimista (RowVersion) en PromotorWallet para prevenir doble debito en solicitudes concurrentes; si hay conflicto de version, el sistema retorna 409 Conflict sin modificar el saldo |
| RNF-02 | El minimo de retiro (10 EUR default) debe ser configurable via appsettings.json o variable de entorno; no puede estar hardcodeado en el codigo |
| RNF-03 | La acreditacion de credito (por conversion o tarea validada) es transaccional con la operacion que la origina: crear PromotorWalletTransaccion + actualizar PromotorWallet deben ejecutarse en una sola transaccion de BD |
| RNF-04 | El endpoint GET promotor/wallet/transacciones debe soportar paginacion obligatoria; el tamano de pagina por defecto es 10 y el maximo es 50 |
| RNF-05 | Los registros de PromotorWalletTransaccion son inmutables una vez creados; no se eliminan fisicamente para mantener trazabilidad de auditoria |

---

## Notas de Alcance MVP

### En scope

- Pagina de wallet del promotor con saldo, totales y historial paginado con filtros
- Solicitud de cobro con validacion de saldo minimo y saldo disponible
- Acreditacion automatica de creditos al validar tareas (integracion con US-CP-04)
- Acreditacion automatica de creditos al registrar conversiones (integracion con US-CP-05)
- Cambios de modelo de dominio: campos EsCredito y PromoEventoId en PromotorWalletTransaccion
- Migracion de EF Core para los nuevos campos

### Diferido (fuera de MVP)

- Vista de administrador para gestionar estados de transacciones (Procesada, Pagada, Cancelada)
- Integracion con procesador de pagos (Stripe Connect o similar) para pagos automaticos
- Notificaciones al promotor cuando su cobro es procesado o pagado
- Exportacion del historial de transacciones a CSV o PDF
- Soporte multi-moneda en una sola vista de wallet (MVP asume una moneda por wallet)

---

## Referencia

- User Story completa: [docs/product/US-CP-06-wallet-comisiones.md](../../product/US-CP-06-wallet-comisiones.md)
- Feature spec cp-tracking-metricas: [docs/user-stories/cp-tracking-metricas/feature-spec.md](../cp-tracking-metricas/feature-spec.md)
- Feature spec cp-tareas-promocion: [docs/user-stories/cp-tareas-promocion/feature-spec.md](../cp-tareas-promocion/feature-spec.md)
- Feature spec cp-perfil-promotor: [docs/user-stories/cp-perfil-promotor/feature-spec.md](../cp-perfil-promotor/feature-spec.md)
