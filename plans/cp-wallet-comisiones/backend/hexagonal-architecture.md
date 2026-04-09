# Arquitectura Hexagonal: cp-wallet-comisiones

**Fecha:** 2026-03-02
**Modulo:** Crowdpromotion
**Feature:** cp-wallet-comisiones (US-CP-06: Wallet de Promotor, Comisiones y Cobros)

---

## 1. Resumen Ejecutivo

Esta feature cierra el ciclo economico del modulo Crowdpromotion exponiendo el wallet del promotor: saldo disponible, historial paginado de transacciones (creditos y debitos) y solicitud de cobro. Requiere modificar la entidad `PromotorWalletTransaccion` con tres campos nuevos, agregar concurrencia optimista (`RowVersion`) a `PromotorWallet`, crear un repositorio dedicado `IPromotorWalletTransaccionRepository` para queries paginadas con filtros, y extender `IPromotorWalletService` con la logica transaccional de cobro usando concurrencia optimista. La entidad `PromoEvento` ya existe y su `Id` es el FK que las transacciones de credito referencian via `PromoEventoId`.

---

## 2. Analisis del Estado Actual

### 2.1 Entidades Existentes (sin cambios de estructura, solo observacion)

| Entidad | Archivo | Estado |
|---------|---------|--------|
| `PromotorWallet` | `Domain/Model/PromotorWallet.cs` | MODIFICAR - agregar `RowVersion` |
| `PromotorWalletTransaccion` | `Domain/Model/PromotorWalletTransaccion.cs` | MODIFICAR - agregar 3 campos |
| `Promotor` | `Domain/Model/Promotor.cs` | Sin cambios |
| `PromoEvento` | `Domain/Model/PromoEvento.cs` | Sin cambios |

### 2.2 Repositories Existentes

| Interface | Archivo | Estado |
|-----------|---------|--------|
| `IPromotorWalletRepository` | `Domain/Interfaces/IPromotorWalletRepository.cs` | EXTENDER - agregar metodos de escritura con concurrencia |
| `IPromoEventoRepository` | `Domain/Interfaces/IPromoEventoRepository.cs` | Sin cambios |
| `IPromotorRepository` | `Domain/Interfaces/IPromotorRepository.cs` | Sin cambios |

### 2.3 Services Existentes

| Interface | Archivo | Estado |
|-----------|---------|--------|
| `IPromotorWalletService` | `Application/Interfaces/Services/IPromotorWalletService.cs` | EXTENDER - agregar metodos de wallet y cobro |
| `IPromoEventoService` | `Application/Interfaces/Services/IPromoEventoService.cs` | Sin cambios |
| `IPromotorService` | `Application/Interfaces/Services/IPromotorService.cs` | Sin cambios |

### 2.4 Patron de Transacciones Observado

El proyecto usa `await using var transaction = await _context.Database.BeginTransactionAsync(ct)` directamente en los services (PromotorService, PromoEventoService). Este patron se mantiene para la feature actual. El `IUnitOfWork<T>` del BuildingBlock existe pero **no esta siendo usado por este modulo**; se respeta el patron establecido en el proyecto.

---

## 3. Domain Layer

### 3.1 Modificacion de Entidades

#### 3.1.1 PromotorWalletTransaccion - Campos a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWalletTransaccion.cs`

**Accion:** MODIFICAR - agregar tres propiedades a la clase existente.

Campos existentes que se conservan sin cambio:
- `Id`, `WalletId`, `TipoRewardId`, `EstadoTransaccionId`, `CampaniaPayoutId`
- `Importe`, `Concepto`, `ReferenciaExterna`, `FechaCreacion`, `FechaProcesado`
- Navigation: `Wallet`

Campos nuevos a agregar:

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `EsCredito` | `bool` | No (NOT NULL) | true=ingreso (comision acreditada), false=debito (retiro solicitado). Determina el tipo de transaccion junto con el signo del importe |
| `PromoEventoId` | `Guid?` | Si (NULL) | FK a PromoEvento. Referencia al evento que origino el credito. Solo aplica cuando EsCredito=true. Null para retiros (debitos) |
| `Descripcion` | `string?` | Si (NULL) | Texto legible del origen de la transaccion. Max 500 chars. Ej: "Comision por backing referido", "Retiro mensual" |

Navigation a agregar:

| Propiedad | Tipo | Descripcion |
|-----------|------|-------------|
| `PromoEvento` | `PromoEvento?` | Navigation property hacia PromoEvento. Null para debitos |

**Nota de migracion:** `EsCredito` requiere valor default `true` en la migracion para rows existentes (asumiendo que todas las transacciones historicas son creditos de US-CP-04 y US-CP-05). `PromoEventoId` y `Descripcion` son nullable y no requieren default.

#### 3.1.2 PromotorWallet - Campo a Agregar

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWallet.cs`

**Accion:** MODIFICAR - agregar propiedad RowVersion para concurrencia optimista.

Campo nuevo a agregar:

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| `RowVersion` | `byte[]` | No | Token de concurrencia optimista. EF Core lo gestiona automaticamente (incrementa en cada UPDATE). Necesario para RN-05: prevenir doble debito ante solicitudes de cobro concurrentes |

**Importante:** EF Core configura `RowVersion` como `IsRowVersion()` en Fluent API; el tipo en SQL Server es `rowversion` (alias de `timestamp`). Al intentar actualizar una fila con un `RowVersion` desactualizado, EF lanza `DbUpdateConcurrencyException`.

### 3.2 Nuevas Constantes de Dominio

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

**Accion:** MODIFICAR - agregar constantes al final de la clase `ServiceResponseMessageType` existente.

Constantes a agregar (bloques libres confirmados en el archivo actual):

```
// NotFound (2000-2999) - siguiente libre: 2030 (ultimo existente: 2022)
NotFound_Wallet = "2030"

// Business Rules (4000-4999) - siguiente libre: 4040 (ultimo existente: 4032)
BusinessRule_SaldoInsuficiente = "4040"
BusinessRule_SaldoBajoMinimoRetiro = "4041"
BusinessRule_CobroConcurrente = "4042"
```

| Constante | Codigo | Descripcion |
|-----------|--------|-------------|
| `NotFound_Wallet` | `"2030"` | Promotor no tiene wallet asignado |
| `BusinessRule_SaldoInsuficiente` | `"4040"` | Importe solicitado > SaldoDisponible del wallet |
| `BusinessRule_SaldoBajoMinimoRetiro` | `"4041"` | SaldoDisponible < minimoRetiro (10.00 EUR) |
| `BusinessRule_CobroConcurrente` | `"4042"` | Ya existe una solicitud Pendiente para este wallet (conflicto de RowVersion o solicitud duplicada) |

### 3.3 Nueva Interface de Repositorio

#### IPromotorWalletTransaccionRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromotorWalletTransaccionRepository.cs`

**Accion:** CREAR - nueva interface.

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetPagedByWalletIdAsync` | `Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)>` | `Guid walletId, bool? esCredito, int? estadoTransaccionId, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken ct` | Query paginada con filtros opcionales. Retorna tupla con items de la pagina y count total para paginacion. Orden: FechaCreacion DESC (mas reciente primero) |
| `AddAsync` | `Task` | `PromotorWalletTransaccion entity, CancellationToken ct` | Inserta la transaccion en el contexto. NO llama SaveChanges (lo hace el service en la misma transaccion de BD) |
| `HasPendienteByWalletIdAsync` | `Task<bool>` | `Guid walletId, CancellationToken ct` | Verifica si existe al menos una transaccion con EstadoTransaccionId=1 (Pendiente) y EsCredito=false (debito/retiro). Usado para RN-05: prevenir cobros concurrentes |

**Notas de diseno:**
- `GetPagedByWalletIdAsync` retorna una tupla en lugar de un objeto custom porque el patron ya existe en el modulo (ver `IPromoEventoRepository` con records raw).
- `HasPendienteByWalletIdAsync` filtra solo debitos pendientes (EsCredito=false, EstadoTransaccionId=1) porque los creditos pendientes son la mayoria de transacciones y no bloquean nuevos cobros.
- `AddAsync` NO hace SaveChanges por diseno: la insercion de la transaccion y la actualizacion del saldo deben ser atomicas en una sola transaccion de BD gestionada por el service.

### 3.4 Extension de IPromotorWalletRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromotorWalletRepository.cs`

**Accion:** MODIFICAR - agregar metodos a la interface existente.

Metodos existentes que se conservan:
- `GetByPromotorIdAndMonedaAsync(PromotorId promotorId, int monedaId, CancellationToken ct)`
- `AddAsync(PromotorWallet entity, CancellationToken ct)`

Metodos nuevos a agregar:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByPromotorIdAndMonedaForUpdateAsync` | `Task<PromotorWallet?>` | `PromotorId promotorId, int monedaId, CancellationToken ct` | Igual que GetByPromotorIdAndMoneda pero **con tracking** (sin AsNoTracking). Necesario para la operacion de cobro donde EF debe detectar el cambio de RowVersion y SaldoDisponible |
| `UpdateSaldoAsync` | `Task` | `PromotorWallet entity, CancellationToken ct` | Llama `_context.Wallets.Update(entity)`. NO llama SaveChanges (se coordina en el service con la transaccion de BD). Actualiza SaldoDisponible, TotalRetirado y FechaActualizacion |

**Razon del metodo separado `GetByPromotorIdAndMonedaForUpdateAsync`:** El metodo existente usa `AsNoTracking()` (solo lectura, eficiente para queries). Para el cobro necesitamos tracking activo para que EF pueda detectar el cambio de RowVersion y generar el UPDATE con la condicion `WHERE RowVersion = @originalRowVersion`. Separar los dos metodos mantiene la eficiencia del existente sin modificar su semantica.

---

## 4. Infrastructure Layer

### 4.1 Configuracion EF Core en CrowdpromotionContext

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

**Accion:** MODIFICAR - actualizar la configuracion Fluent API de las dos entidades afectadas dentro del metodo `OnModelCreating`.

#### Configuracion PromotorWallet (modificar el bloque existente)

Agregar dentro del bloque `modelBuilder.Entity<PromotorWallet>(entity => { ... })`:

| Configuracion | Fluent API | Notas |
|---------------|-----------|-------|
| RowVersion | `entity.Property(e => e.RowVersion).IsRowVersion()` | EF Core gestiona automaticamente el incremento. SQL Server lo persiste como `rowversion` (8 bytes). No requiere columna manual en la migracion si se usa `IsRowVersion()` |

#### Configuracion PromotorWalletTransaccion (modificar el bloque existente)

Agregar dentro del bloque `modelBuilder.Entity<PromotorWalletTransaccion>(entity => { ... })`:

| Propiedad | Configuracion Fluent API | Notas |
|-----------|--------------------------|-------|
| `EsCredito` | `entity.Property(e => e.EsCredito).IsRequired().HasDefaultValue(true)` | NOT NULL con default=true para rows existentes en la migracion |
| `PromoEventoId` | `entity.Property(e => e.PromoEventoId).HasColumnName("PromoEvento_Id")` | Nullable, sigue la convencion de nombres `_Id` del proyecto |
| `Descripcion` | `entity.Property(e => e.Descripcion).HasMaxLength(500)` | Nullable, max 500 chars segun RN del contrato |

FK a PromoEvento (agregar relacion):

```
entity.HasOne(e => e.PromoEvento)
      .WithMany()
      .HasForeignKey(e => e.PromoEventoId)
      .OnDelete(DeleteBehavior.SetNull)
```

**Razon de `DeleteBehavior.SetNull`:** Si un PromoEvento se eliminara (escenario improbable en MVP, los eventos son inmutables), la transaccion del wallet conserva el registro historico con `PromoEventoId = null`. Esto garantiza RNF-05 (inmutabilidad de transacciones).

Indices nuevos a agregar en la configuracion de `PromotorWalletTransaccion`:

| Indice | Columnas | Tipo | Razon |
|--------|----------|------|-------|
| `IX_PromotorWalletTransaccion_Wallet_EsCredito_Estado` | `WalletId, EsCredito, EstadoTransaccionId` | Compuesto no unico | Cubre el filtro principal del endpoint GET transacciones: `WHERE Wallet_Id = ? AND EsCredito = ? AND EstadoTransaccion_Id = ?` |
| `IX_PromotorWalletTransaccion_Wallet_Fecha` | `WalletId, FechaCreacion` | Compuesto no unico | Cubre el filtro por fechas + orden DESC del historial paginado |

**Nota:** El indice existente `IX_PromotorWalletTransaccion_FechaCreacion` (solo FechaCreacion) no es suficiente para queries filtrando por wallet; los dos indices nuevos lo complementan.

### 4.2 Nuevo Repositorio: PromotorWalletTransaccionRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromotorWalletTransaccionRepository.cs`

**Accion:** CREAR - implementacion de `IPromotorWalletTransaccionRepository`.

**Implementa:** `IPromotorWalletTransaccionRepository`
**Inyecta:** `CrowdpromotionContext`

#### Metodo: GetPagedByWalletIdAsync

**Estrategia de query:**
1. Base query: `_context.WalletTransacciones.AsNoTracking().Where(t => t.WalletId == walletId)`
2. Filtros opcionales aplicados condicionalmente:
   - `EsCredito`: `.Where(t => t.EsCredito == esCredito.Value)` si no es null
   - `EstadoTransaccionId`: `.Where(t => t.EstadoTransaccionId == estadoTransaccionId.Value)` si no es null
   - `FechaDesde`: `.Where(t => t.FechaCreacion >= fechaDesde.Value)` si no es null
   - `FechaHasta`: `.Where(t => t.FechaCreacion <= fechaHasta.Value)` si no es null
3. Count total: ejecutar `.CountAsync(ct)` sobre la query con filtros ANTES de paginar (necesario para `TotalPages`)
4. Paginacion: `.OrderByDescending(t => t.FechaCreacion).Skip((page - 1) * pageSize).Take(pageSize)`
5. Proyeccion: NO proyectar a DTO en el repositorio. Retornar `PromotorWalletTransaccion` completa para que el service/handler haga el mapping. El repositorio devuelve entidades.

**Detalle de las dos queries ejecutadas:**
```
Query 1 (count): SELECT COUNT(*) FROM PromotorWalletTransaccion WHERE Wallet_Id = @p AND [filtros]
Query 2 (items): SELECT TOP(@pageSize) ... FROM PromotorWalletTransaccion WHERE Wallet_Id = @p AND [filtros] ORDER BY FechaCreacion DESC OFFSET (@page-1)*@pageSize ROWS
```

**Alternativa con una sola query:** No se usa `Split Query` ni subquery de count embebida porque los filtros son opcionales y la composicion de LINQ es mas legible con dos queries separadas. El indice compuesto garantiza que ambas queries sean eficientes.

#### Metodo: AddAsync

Comportamiento: `await _context.WalletTransacciones.AddAsync(entity, ct)` y retorna. **No llama SaveChanges.** El service llama `SaveChanges` como parte de la transaccion atomica de cobro.

#### Metodo: HasPendienteByWalletIdAsync

Comportamiento: `_context.WalletTransacciones.AsNoTracking().AnyAsync(t => t.WalletId == walletId && !t.EsCredito && t.EstadoTransaccionId == 1, ct)`.

Filtra `!t.EsCredito` (solo debitos/retiros) con estado `Pendiente (1)`.

### 4.3 Modificacion: PromotorWalletRepository

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromotorWalletRepository.cs`

**Accion:** MODIFICAR - agregar dos metodos nuevos a la implementacion existente.

#### Metodo nuevo: GetByPromotorIdAndMonedaForUpdateAsync

Comportamiento: identico al metodo existente `GetByPromotorIdAndMonedaAsync` pero **sin** `AsNoTracking()`. EF trackea la entidad para detectar cambios en `SaldoDisponible`, `TotalRetirado`, `FechaActualizacion` y el `RowVersion`.

```
return await _context.Wallets
    .FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.MonedaId == monedaId, ct);
```

#### Metodo nuevo: UpdateSaldoAsync

Comportamiento: `_context.Wallets.Update(entity)`. **No llama SaveChanges.** El service llama `SaveChanges` despues de insertar la transaccion y actualizar el wallet en la misma transaccion de BD.

**Nota:** EF Core generara un `UPDATE PromotorWallet SET SaldoDisponible = ?, TotalRetirado = ?, FechaActualizacion = ? WHERE Id = ? AND RowVersion = ?`. Si el RowVersion no coincide (otra request modifico el wallet entre el GET y el UPDATE), EF lanza `DbUpdateConcurrencyException`. El service captura esta excepcion y retorna el error `BusinessRule_CobroConcurrente`.

### 4.4 Extension: PromotorWalletService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromotorWalletService.cs`

**Accion:** MODIFICAR - agregar dependencias y metodos nuevos a la implementacion existente.

#### Dependencias actuales (conservar)

- `IPromotorWalletRepository _walletRepository`
- `IRequestCacheService _requestCache`
- `ILogger<PromotorWalletService> _logger`

#### Dependencias nuevas a agregar al constructor

- `IPromotorWalletTransaccionRepository _transaccionRepository` - para insertar transaccion de cobro
- `CrowdpromotionContext _context` - para gestionar la transaccion de BD atomica (patron establecido en PromotorService y PromoEventoService)
- `IConfiguration _configuration` - para leer `Crowdpromotion:MinimoRetiro` de appsettings (RNF-02: valor configurable)

**Razon de inyectar CrowdpromotionContext directamente:** El patron del proyecto ya usa este enfoque en `PromotorService` y `PromoEventoService` para transacciones de BD. No usar IUnitOfWork porque ese BuildingBlock no esta siendo adoptado en este modulo.

**Constructor actualizado:**

Todas las dependencias (nuevas y existentes) deben tener `?? throw new ArgumentNullException(nameof(...))`.

#### Extension de IPromotorWalletService

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

**Accion:** MODIFICAR - agregar firmas de metodos nuevos.

Metodo existente que se conserva:
- `GetWalletEurByPromotorIdAsync(PromotorId promotorId, CancellationToken ct)`

Metodos nuevos a agregar a la interface:

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetWalletConMonedaAsync` | `Task<(PromotorWallet? Wallet, string? MonedaNombre)>` | `PromotorId promotorId, CancellationToken ct` | Obtiene el wallet del promotor (moneda EUR, MonedaId=1) junto con el nombre de la moneda para el DTO de respuesta. En MVP la moneda es siempre EUR (MonedaId=1). Usa cache via RequestCache |
| `GetTransaccionesPagedAsync` | `Task<(IReadOnlyList<PromotorWalletTransaccion> Items, int TotalCount)>` | `Guid walletId, bool? esCredito, int? estadoTransaccionId, DateTime? fechaDesde, DateTime? fechaHasta, int page, int pageSize, CancellationToken ct` | Delega a `IPromotorWalletTransaccionRepository.GetPagedByWalletIdAsync`. Sin cache (historial cambia con frecuencia) |
| `SolicitarCobroAsync` | `Task<SolicitarCobroResult>` | `PromotorId promotorId, decimal importe, string? descripcion, CancellationToken ct` | Logica completa de cobro con concurrencia optimista. Ver detalle en seccion 4.4.1 |

#### Tipo de Retorno: SolicitarCobroResult

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

**Accion:** Definir como `record` en el mismo archivo que la interface (patron del proyecto, ver `RegistrarConversionResult` en IPromoEventoService).

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `TransaccionId` | `Guid` | ID de la PromotorWalletTransaccion creada |
| `Importe` | `decimal` | Importe del cobro solicitado |
| `MonedaNombre` | `string` | Nombre de la moneda del wallet (siempre "EUR" en MVP) |
| `SaldoRestante` | `decimal` | SaldoDisponible del wallet tras el descuento |
| `FechaCreacion` | `DateTime` | Timestamp UTC de la transaccion creada |

#### 4.4.1 Logica de SolicitarCobroAsync - Detalle

La logica se ejecuta dentro de `await using var transaction = await _context.Database.BeginTransactionAsync(ct)`:

**Paso 1 - Obtener wallet con tracking:**
```
var wallet = await _walletRepository.GetByPromotorIdAndMonedaForUpdateAsync(promotorId, monedaId: 1, ct)
```
Si null: no lanzar excepcion, retornar null (el handler convierte a ServiceResponse con NotFound_Wallet).

**Paso 2 - Leer configuracion de minimo de retiro:**
```
var minimoRetiro = _configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m)
```
Lectura de appsettings. Si la clave no existe, el default es 10.0m (RNF-02).

**Paso 3 - Validaciones de negocio (retornar null con codigo de error identificable):**

El service no lanza excepciones de validacion de negocio; retorna `null` y el handler interpreta el resultado. Para comunicar el tipo de error de negocio, se utiliza un enum o se lanza una excepcion de dominio especifica. El patron del proyecto (`PromoEventoService`) lanza excepciones que el handler captura, pero aqui se propone un enfoque mas limpio con un result discriminado.

**Propuesta de CobroError:**
```
public enum CobroError { None, WalletNoEncontrado, SaldoBajoMinimo, SaldoInsuficiente, CobroConcurrente }
```

El metodo retorna `(SolicitarCobroResult? Result, CobroError Error)`. El handler mapea `CobroError` a `ServiceResponseMessageType`. Esto evita la proliferacion de excepciones de control de flujo.

**Validaciones en orden:**
1. Si `wallet == null` -> retornar `(null, CobroError.WalletNoEncontrado)`
2. Si `wallet.SaldoDisponible < minimoRetiro` -> retornar `(null, CobroError.SaldoBajoMinimo)`
3. Si `importe > wallet.SaldoDisponible` -> retornar `(null, CobroError.SaldoInsuficiente)`
4. Si `await _transaccionRepository.HasPendienteByWalletIdAsync(wallet.Id, ct) == true` -> retornar `(null, CobroError.CobroConcurrente)`

**Paso 4 - Crear transaccion de debito:**
```
var transaccion = new PromotorWalletTransaccion
{
    Id = Guid.NewGuid(),
    WalletId = wallet.Id,
    EsCredito = false,              // Debito/retiro
    EstadoTransaccionId = 1,        // Pendiente
    Importe = importe,
    Descripcion = descripcion,
    PromoEventoId = null,           // Null para retiros (RN-06)
    FechaCreacion = DateTime.UtcNow
}
await _transaccionRepository.AddAsync(transaccion, ct)  // Solo AddAsync, sin SaveChanges
```

**Paso 5 - Actualizar saldo del wallet (con RowVersion):**
```
wallet.SaldoDisponible -= importe
wallet.TotalRetirado += importe
wallet.FechaActualizacion = DateTime.UtcNow
await _walletRepository.UpdateSaldoAsync(wallet, ct)  // Solo Update, sin SaveChanges
```

**Paso 6 - Commit atomico:**
```
try
{
    await _context.SaveChangesAsync(ct)     // EF verifica RowVersion aqui
    await transaction.CommitAsync(ct)
    return (new SolicitarCobroResult(...), CobroError.None)
}
catch (DbUpdateConcurrencyException)
{
    await transaction.RollbackAsync(ct)
    _logger.LogWarning("Concurrencia optimista en wallet {WalletId}", wallet.Id)
    return (null, CobroError.CobroConcurrente)
}
catch (Exception ex)
{
    await transaction.RollbackAsync(ct)
    _logger.LogError(ex, "Error en SolicitarCobroAsync")
    throw   // Re-lanzar para que el handler lo capture como error interno
}
```

**Razon de capturar `DbUpdateConcurrencyException` separada:** EF lanza esta excepcion especifica cuando el RowVersion del UPDATE no coincide con el valor en BD. Capturarla aqui permite retornar `CobroError.CobroConcurrente` (HTTP 409) en lugar de un error 500.

#### 4.4.2 Implementacion de GetWalletConMonedaAsync

Usa cache con clave `promotorwallet:{promotorId.Value}:moneda:1`. En MVP, `MonedaNombre` es siempre `"EUR"` (MonedaId=1 hardcodeado). Retorna tupla `(PromotorWallet?, "EUR")`. Si el wallet es null, retorna `(null, null)`.

### 4.5 Tipo CobroError

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

**Accion:** Definir en el mismo archivo que la interface (o en archivo separado dentro de la misma carpeta).

```
public enum CobroError
{
    None = 0,
    WalletNoEncontrado = 1,
    SaldoBajoMinimo = 2,
    SaldoInsuficiente = 3,
    CobroConcurrente = 4
}
```

Mapeo al ServiceResponseMessageType en el handler:

| CobroError | ServiceResponseMessageType | HTTP Status |
|------------|---------------------------|-------------|
| `WalletNoEncontrado` | `NotFound_Wallet = "2030"` | 404 |
| `SaldoBajoMinimo` | `BusinessRule_SaldoBajoMinimoRetiro = "4041"` | 409 |
| `SaldoInsuficiente` | `BusinessRule_SaldoInsuficiente = "4040"` | 409 |
| `CobroConcurrente` | `BusinessRule_CobroConcurrente = "4042"` | 409 |

### 4.6 Registro DI

**Archivo:** `src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

**Accion:** MODIFICAR - agregar registro del nuevo repositorio. El service existente `PromotorWalletService` ya esta registrado; su modificacion es transparente para el contenedor DI.

Linea a agregar en la seccion `// Repositories`:

```csharp
services.AddScoped<IPromotorWalletTransaccionRepository, PromotorWalletTransaccionRepository>();
```

No se requiere ningun cambio adicional para `IPromotorWalletService` porque ya esta registrado con la clase `PromotorWalletService`.

**Estado del archivo DependencyInjection.cs tras el cambio:**

```
// Repositories
services.AddScoped<IPromotorRepository, PromotorRepository>();
services.AddScoped<IPromotorWalletRepository, PromotorWalletRepository>();
services.AddScoped<IPromoProgramaRepository, PromoProgramaRepository>();
services.AddScoped<IPromoProgramaPromotorRepository, PromoProgramaPromotorRepository>();
services.AddScoped<IPromoTareaRepository, PromoTareaRepository>();
services.AddScoped<IPromoTareaPromotorRepository, PromoTareaPromotorRepository>();
services.AddScoped<IPromoEventoRepository, PromoEventoRepository>();
services.AddScoped<IPromotorWalletTransaccionRepository, PromotorWalletTransaccionRepository>();  // NUEVO

// Services  (sin cambios)
services.AddScoped<IPromotorService, PromotorService>();
services.AddScoped<IPromotorWalletService, PromotorWalletService>();
...
```

---

## 5. Configuracion en appsettings.json

**Archivo:** `src/api/WebApi/appsettings.json` y `src/api/WebApi/appsettings.Development.json`

**Accion:** AGREGAR seccion `Crowdpromotion` con el valor configurable del minimo de retiro (RNF-02).

```json
{
  "Crowdpromotion": {
    "MinimoRetiro": 10.00
  }
}
```

El service lee este valor con `_configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m)`. El fallback de `10.0m` garantiza que funcione aunque la clave no exista en un entorno.

---

## 6. Migracion EF Core

### 6.1 Nombre y Comando

```bash
dotnet ef migrations add AddWalletTransaccionFieldsAndRowVersion \
    --project src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra \
    --startup-project src/api/WebApi \
    --context CrowdpromotionContext

dotnet ef database update \
    --project src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra \
    --startup-project src/api/WebApi \
    --context CrowdpromotionContext
```

### 6.2 Cambios que Genera la Migracion

**Tabla: PromotorWalletTransaccion**

| Operacion | Columna | Tipo SQL | Notas |
|-----------|---------|---------|-------|
| ADD COLUMN | `EsCredito` | `bit NOT NULL DEFAULT 1` | Default=1 (true) para rows existentes. Todas las transacciones historicas son creditos |
| ADD COLUMN | `PromoEvento_Id` | `uniqueidentifier NULL` | FK nullable a PromoEvento.Id |
| ADD COLUMN | `Descripcion` | `nvarchar(500) NULL` | Max 500 chars, nullable |
| ADD FK | `FK_PromotorWalletTransaccion_PromoEvento_PromoEvento_Id` | - | ON DELETE SET NULL |
| ADD INDEX | `IX_PromotorWalletTransaccion_Wallet_EsCredito_Estado` | - | Composite: WalletId + EsCredito + EstadoTransaccionId |
| ADD INDEX | `IX_PromotorWalletTransaccion_Wallet_Fecha` | - | Composite: WalletId + FechaCreacion |

**Tabla: PromotorWallet**

| Operacion | Columna | Tipo SQL | Notas |
|-----------|---------|---------|-------|
| ADD COLUMN | `RowVersion` | `rowversion NOT NULL` | SQL Server lo gestiona automaticamente. No requiere valor en INSERT |

**Nota critica sobre RowVersion:** Al agregar la columna `rowversion` a una tabla existente con datos, SQL Server asigna automaticamente un valor unico a cada fila. No se requiere dato default manual.

### 6.3 Script de Rollback (para referencia)

Si se necesita revertir:
```bash
dotnet ef migrations remove \
    --project src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra \
    --startup-project src/api/WebApi \
    --context CrowdpromotionContext
```

Solo aplica si la migracion no ha sido aplicada a la BD. Si ya fue aplicada, crear nueva migracion de rollback.

---

## 7. Flujo de Datos Completo

### 7.1 GET /promotor/wallet (Query de saldo)

```
Controller
  -> MediatR -> GetWalletQuery Handler
       |
       v
  IPromotorService.GetByUserIdAsync(userId)  [con cache: "promotor:userid:{userId}"]
       |
       -> Si null: ServiceResponse con NotFound_Promotor (2015)
       |
       v
  IPromotorWalletService.GetWalletConMonedaAsync(promotorId)  [con cache]
       |
       -> Si null: ServiceResponse con NotFound_Wallet (2030)
       |
       v
  IMapper.Map<PromotorWalletDto>(wallet)
       + wallet.MinimoRetiro = _configuration.GetValue(10.0m)
       |
       v
  ServiceResponse<PromotorWalletDto> { Data = dto }
```

### 7.2 GET /promotor/wallet/transacciones (Query paginada con filtros)

```
Controller
  -> MediatR -> GetWalletTransaccionesQuery Handler
       |
       v
  IPromotorService.GetByUserIdAsync(userId)  [cache]
       |
       -> Si null: NotFound_Promotor
       |
       v
  IPromotorWalletService.GetWalletConMonedaAsync(promotorId)  [cache]
       |
       -> Si null: NotFound_Wallet
       |
       v
  IPromotorWalletService.GetTransaccionesPagedAsync(walletId, filtros...)
       |
       v
  IPromotorWalletTransaccionRepository.GetPagedByWalletIdAsync(...)
       |
       [2 queries SQL: COUNT + SELECT paginado con indices compuestos]
       |
       v
  IMapper.Map<List<WalletTransaccionItemDto>>(items)
       |
       v
  ServiceResponse<WalletTransaccionesPagedDto> {
      Items, TotalCount, Page, PageSize, TotalPages
  }
```

### 7.3 POST /promotor/wallet/cobro (Command con concurrencia optimista)

```
Controller
  -> MediatR -> SolicitarCobroCommand Handler
       |
       v
  FluentValidation (SolicitarCobroCommandValidator)
       | importe > 0, descripcion <= 500 chars
       -> Si invalid: ServiceResponse con errores de validacion
       |
       v
  IPromotorService.GetByUserIdAsync(userId)  [cache]
       |
       -> Si null: NotFound_Promotor
       |
       v
  IPromotorWalletService.SolicitarCobroAsync(promotorId, importe, descripcion)
       |
       BEGIN TRANSACTION (CrowdpromotionContext)
         |
         -> GetByPromotorIdAndMonedaForUpdateAsync (CON tracking, para RowVersion)
         -> Lee MinimoRetiro de appsettings
         -> Valida: SaldoDisponible >= MinimoRetiro  -> CobroError.SaldoBajoMinimo
         -> Valida: importe <= SaldoDisponible       -> CobroError.SaldoInsuficiente
         -> HasPendienteByWalletIdAsync               -> CobroError.CobroConcurrente
         |
         -> AddAsync(PromotorWalletTransaccion)      [EsCredito=false, EstadoId=1]
         -> UpdateSaldoAsync(wallet)                 [SaldoDisponible -= importe]
         |
         -> SaveChangesAsync()
              [EF genera: UPDATE PromotorWallet WHERE Id=? AND RowVersion=?]
              [Si RowVersion desactualizado: DbUpdateConcurrencyException]
         |
         -> CommitAsync()
       END TRANSACTION
       |
       v
  Handler recibe (SolicitarCobroResult?, CobroError)
       |
       -> Si CobroError != None: mapear a ServiceResponseMessageType + HTTP status
       -> Si CobroError.None: IMapper.Map<SolicitarCobroResponseDto>(result)
       |
       v
  ServiceResponse<SolicitarCobroResponseDto>
```

### 7.4 Acreditacion Automatica (Integracion con US-CP-04 y US-CP-05)

```
[US-CP-05: PromoEventoService.RegistrarConversionAsync]
  -> Transaccion BD existente
  -> Crea PromotorWalletTransaccion con:
       EsCredito = true
       PromoEventoId = evento.Id   <-- NUEVO campo
       EstadoTransaccionId = 1
       Importe = comisionCalculada
  -> Actualiza wallet.SaldoPendiente += comision  (existente, sin cambio)

[US-CP-04: ValidarTareaHandler - por implementar en otra feature]
  -> Crea PromotorWalletTransaccion con:
       EsCredito = true
       PromoEventoId = promoEventoId (si hay evento asociado)
       Descripcion = "Recompensa tarea: {titulo}"
```

**Nota:** `PromoEventoService.RegistrarConversionAsync` ya existe y crea transacciones de wallet. Con los nuevos campos `EsCredito` y `PromoEventoId` en la entidad, ese codigo debe actualizarse para poblar los nuevos campos. Este cambio es parte de la implementacion de US-CP-06 (los campos antes no existian).

---

## 8. Archivos a Crear / Modificar

```
Modulo Crowdpromotion/
│
├── Domain/
│   ├── Model/
│   │   ├── PromotorWallet.cs                         MODIFICAR - agregar RowVersion (byte[])
│   │   └── PromotorWalletTransaccion.cs               MODIFICAR - agregar EsCredito, PromoEventoId, Descripcion, nav PromoEvento
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs              MODIFICAR - agregar 2030, 4040, 4041, 4042
│   └── Interfaces/
│       ├── IPromotorWalletRepository.cs               MODIFICAR - agregar GetByPromotorIdAndMonedaForUpdateAsync, UpdateSaldoAsync
│       └── IPromotorWalletTransaccionRepository.cs    CREAR - nueva interface
│
├── Application/
│   └── Interfaces/
│       └── Services/
│           └── IPromotorWalletService.cs              MODIFICAR - agregar GetWalletConMonedaAsync, GetTransaccionesPagedAsync, SolicitarCobroAsync
│                                                                  + definir record SolicitarCobroResult + enum CobroError
│
└── Infra/
    ├── Context/
    │   └── CrowdpromotionContext.cs                   MODIFICAR - config RowVersion en PromotorWallet, campos nuevos + indices en PromotorWalletTransaccion
    ├── Repositories/
    │   ├── PromotorWalletRepository.cs                MODIFICAR - agregar GetByPromotorIdAndMonedaForUpdateAsync, UpdateSaldoAsync
    │   └── PromotorWalletTransaccionRepository.cs     CREAR - nueva implementacion
    ├── Services/
    │   └── PromotorWalletService.cs                   MODIFICAR - agregar dependencias + SolicitarCobroAsync, GetWalletConMonedaAsync, GetTransaccionesPagedAsync
    └── DependencyInjection.cs                         MODIFICAR - registrar IPromotorWalletTransaccionRepository
```

**Tambien modificar:**
```
src/api/WebApi/
├── appsettings.json                                   MODIFICAR - agregar "Crowdpromotion": { "MinimoRetiro": 10.0 }
└── appsettings.Development.json                       MODIFICAR - mismo valor

src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/
└── PromoEventoService.cs                              MODIFICAR - poblar EsCredito=true y PromoEventoId en la transaccion creada en RegistrarConversionAsync
```

---

## 9. Checklist

- [ ] Entidades son POCOs (sin metodos de negocio): PromotorWallet y PromotorWalletTransaccion solo tienen propiedades
- [ ] `PromotorWalletTransaccion` tiene los 3 campos nuevos: `EsCredito`, `PromoEventoId`, `Descripcion`
- [ ] `PromotorWallet` tiene `RowVersion` (byte[]) para concurrencia optimista (RN-05)
- [ ] `ServiceResponseMessageType.cs` tiene las 4 nuevas constantes: 2030, 4040, 4041, 4042
- [ ] `IPromotorWalletTransaccionRepository` tiene los 3 metodos: GetPagedByWalletIdAsync, AddAsync, HasPendienteByWalletIdAsync
- [ ] `IPromotorWalletRepository` extendida con GetByPromotorIdAndMonedaForUpdateAsync (con tracking) y UpdateSaldoAsync
- [ ] `IPromotorWalletService` extendida con GetWalletConMonedaAsync, GetTransaccionesPagedAsync, SolicitarCobroAsync
- [ ] `SolicitarCobroResult` record y `CobroError` enum definidos junto a IPromotorWalletService
- [ ] `PromotorWalletService` inyecta: repositorio de transaccion + CrowdpromotionContext + IConfiguration + cache + logger
- [ ] `PromotorWalletService.SolicitarCobroAsync` usa transaccion de BD + captura DbUpdateConcurrencyException
- [ ] Fluent API en CrowdpromotionContext: IsRowVersion() en PromotorWallet, campos + FK + indices en PromotorWalletTransaccion
- [ ] MinimoRetiro se lee de appsettings con fallback 10.0m (no hardcodeado en codigo)
- [ ] `PromotorWalletTransaccionRepository.AddAsync` NO llama SaveChanges
- [ ] `PromotorWalletRepository.UpdateSaldoAsync` NO llama SaveChanges
- [ ] DI registra `IPromotorWalletTransaccionRepository` como Scoped
- [ ] Migracion nombrada `AddWalletTransaccionFieldsAndRowVersion` con default EsCredito=true
- [ ] `PromoEventoService.RegistrarConversionAsync` actualizado para poblar EsCredito=true y PromoEventoId en la transaccion creada
- [ ] appsettings.json y appsettings.Development.json actualizados con seccion Crowdpromotion
