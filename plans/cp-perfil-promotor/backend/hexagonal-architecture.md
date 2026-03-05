# Arquitectura Hexagonal: Perfil de Promotor

**Fecha:** 2026-02-25
**Modulo:** Crowdpromotion
**Feature:** cp-perfil-promotor (US-CP-01)
**Bounded Context:** Promotor

---

## 1. Resumen Ejecutivo

Esta arquitectura define las capas Domain e Infrastructure para la feature "Perfil de Promotor" del modulo Crowdpromotion. El sistema permite a un usuario autenticado crear y gestionar su perfil de promotor, incluyendo la creacion automatica de una `PromotorWallet` en EUR en la misma transaccion. La entidad `Promotor` ya existe en el codebase pero le faltan los campos `EmailContacto` y `UrlSitioWeb`, ademas de que `NombrePublico` tiene un limite de 100 caracteres mientras que los contratos definen 200; ambas discrepancias requieren una migracion de EF Core. La arquitectura garantiza que un `UserId` solo puede tener un perfil de promotor (constraint unico en BD) y que la desactivacion es logica (nunca se elimina el registro).

---

## 2. Diagnostico del Codebase Existente

### 2.1 Estado Actual del Modulo Crowdpromotion

| Componente | Estado | Accion Requerida |
|-----------|--------|-----------------|
| `Promotor.cs` (entidad) | EXISTE - incompleto | Agregar `EmailContacto`, `UrlSitioWeb`; ampliar `NombrePublico` a 200 |
| `PromotorWallet.cs` (entidad) | EXISTE - completo | Sin cambios en entidad |
| `PromoProgramaPromotor.cs` (entidad) | EXISTE - completo | Sin cambios en entidad |
| `CrowdpromotionContext.cs` | EXISTE - configuracion incompleta | Agregar columnas nuevas de `Promotor` |
| `DependencyInjection.cs` | EXISTE - vacio | Registrar repositorios y servicios |
| `ServiceResponseMessageType.cs` | NO EXISTE | CREAR en `Domain/Constants/` |
| `IPromotorRepository` | NO EXISTE | CREAR en `Domain/Interfaces/` |
| `IPromotorWalletRepository` | NO EXISTE | CREAR en `Domain/Interfaces/` |
| `IPromotorService` | NO EXISTE | CREAR en `Application/Interfaces/Services/` |
| `IPromotorWalletService` | NO EXISTE | CREAR en `Application/Interfaces/Services/` |
| `PromotorRepository` | NO EXISTE | CREAR en `Infra/Repositories/` |
| `PromotorWalletRepository` | NO EXISTE | CREAR en `Infra/Repositories/` |
| `PromotorService` | NO EXISTE | CREAR en `Infra/Services/` |
| `PromotorWalletService` | NO EXISTE | CREAR en `Infra/Services/` |

### 2.2 Entidades que NO Requieren Cambios

- `PromoProgramaPromotor` ya tiene el campo `EsActivo` necesario para la desactivacion en cascada
- `PromotorWallet` ya tiene `PromotorId`, `MonedaId`, `SaldoDisponible`, `SaldoPendiente`, `TotalGanado`, `TotalRetirado`
- `MaestraTipoPromotor` ya existe en el modulo `Core` con tabla `Maestra_TipoPromotor`; NO es responsabilidad de Crowdpromotion gestionarla

---

## 3. Domain Layer

### 3.1 Entidades

#### Promotor (EXISTE - MODIFICACION REQUERIDA)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/Promotor.cs`

**Cambios requeridos:**
- Ampliar `NombrePublico` de `max(100)` a `max(200)` en la configuracion EF
- Agregar propiedad `EmailContacto` (string?, max 200)
- Agregar propiedad `UrlSitioWeb` (string?, max 300)
- Corregir `UrlInstagram`, `UrlTikTok`, `UrlTwitter`, `UrlYouTube` de `max(200)` a `max(300)` en la configuracion EF (los contratos definen 300)

| Propiedad | Tipo | Nullable | Descripcion | Estado |
|-----------|------|----------|-------------|--------|
| Id | PromotorId | No | PK (Strongly Typed ID) | Existente |
| TipoPromotorId | int | No | FK a Maestra_TipoPromotor (Core) | Existente |
| UserId | string | No | FK a Identity User (max 450) - UNIQUE | Existente |
| FanProfileId | FanProfileId? | Si | FK opcional a FanProfile (UserAccess) | Existente |
| NombrePublico | string | No | Nombre publico del promotor (max 200) | Existente - ampliar a 200 |
| EmailContacto | string? | Si | Email de contacto opcional (max 200) | **NUEVO** |
| UrlSitioWeb | string? | Si | URL del sitio web opcional (max 300) | **NUEVO** |
| UrlInstagram | string? | Si | URL Instagram (max 300) | Existente - ampliar a 300 |
| UrlTikTok | string? | Si | URL TikTok (max 300) | Existente - ampliar a 300 |
| UrlTwitter | string? | Si | URL Twitter/X (max 300) | Existente - ampliar a 300 |
| UrlYouTube | string? | Si | URL YouTube (max 300) | Existente - ampliar a 300 |
| SeguidoresTotales | int? | Si | Seguidores totales (campo legacy/futuro) | Existente |
| EsActivo | bool | No | Estado activo/inactivo (desactivacion logica) | Existente |
| FechaCreacion | DateTime | No | Fecha de creacion UTC | Existente |
| FechaActualizacion | DateTime? | Si | Fecha de ultima actualizacion UTC | Existente |

**Navegaciones (sin cambios):**
- `Programas` -> `ICollection<PromoProgramaPromotor>` (1:N)
- `Wallets` -> `ICollection<PromotorWallet>` (1:N)

**Reglas de dominio (documentadas, no implementadas en entidad):**
- Un `UserId` = un `Promotor` (garantizado por constraint unico `IX_Promotor_UserId` ya en BD)
- `EsActivo = true` al crear; desactivacion es logica (`EsActivo = false`)
- Entidad es POCO puro. Toda logica de negocio va en Handlers.

---

#### PromotorWallet (EXISTE - SIN CAMBIOS)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromotorWallet.cs`

**Estado:** Completa. No requiere modificaciones para esta feature.

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | Guid | No | PK |
| PromotorId | PromotorId | No | FK a Promotor |
| MonedaId | int | No | FK a Maestra_Moneda (1=EUR en MVP) |
| SaldoDisponible | decimal | No | Saldo disponible para retirar |
| SaldoPendiente | decimal | No | Saldo pendiente de confirmacion |
| TotalGanado | decimal | No | Total historico ganado |
| TotalRetirado | decimal | No | Total historico retirado |
| FechaCreacion | DateTime | No | Fecha de creacion UTC |
| FechaActualizacion | DateTime? | Si | Fecha de ultima actualizacion UTC |

**Navegaciones:**
- `Promotor` -> `Promotor` (N:1)
- `Transacciones` -> `ICollection<PromotorWalletTransaccion>` (1:N)

---

#### PromoProgramaPromotor (EXISTE - SIN CAMBIOS)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Model/PromoProgramaPromotor.cs`

Usado en el flujo de desactivacion: el Handler desactivara todos los registros donde `PromotorId == id && EsActivo == true`.

| Propiedad relevante | Tipo | Descripcion |
|--------------------|------|-------------|
| Id | Guid | PK |
| PromotorId | PromotorId | FK a Promotor |
| EsActivo | bool | Se establece a `false` al desactivar el promotor |
| FechaBaja | DateTime? | Se establece a `DateTime.UtcNow` al desactivar |

---

### 3.2 Constants / ServiceResponseMessageType (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Constants/ServiceResponseMessageType.cs`

Este archivo NO existe en el modulo Crowdpromotion. Debe crearse siguiendo el patron de `Crowdfunding.Domain/Constants/ServiceResponseMessageType.cs`. Los codigos 2015, 4018, 4019 son especificos de esta feature y continuan la secuencia numerica del modulo Crowdsourcing para coherencia global.

| Constante | Valor | Categoria | Descripcion |
|-----------|-------|-----------|-------------|
| `Success` | "0000" | Success | Operacion exitosa |
| `Created` | "0001" | Success | Recurso creado |
| `Updated` | "0002" | Success | Recurso actualizado |
| `Deleted` | "0003" | Success | Recurso eliminado |
| `Validation_Required` | "1001" | Validation | Campo obligatorio vacio |
| `Validation_MaxLength` | "1002" | Validation | Campo excede longitud maxima |
| `Validation_InvalidEmail` | "1003" | Validation | Formato de email invalido |
| `Validation_ForeignKeyNotFound` | "1010" | Validation | FK no existe en maestra |
| `Validation_MinLength` | "1011" | Validation | Campo no alcanza longitud minima |
| `Validation_InvalidUrl` | "1013" | Validation | Formato de URL invalido |
| `NotFound_Entity` | "2000" | NotFound | Entidad generica no encontrada |
| `NotFound_Promotor` | "2015" | NotFound | **NUEVO** - No existe Promotor para el UserId |
| `Auth_Unauthorized` | "3001" | Auth | Token invalido o expirado |
| `Auth_Forbidden` | "3002" | Auth | Sin permisos para la operacion |
| `BusinessRule_PromotorAlreadyExists` | "4018" | BusinessRule | **NUEVO** - Ya existe Promotor para el UserId |
| `BusinessRule_PromotorAlreadyInactive` | "4019" | BusinessRule | **NUEVO** - Promotor ya tiene EsActivo=false |
| `Internal_UnexpectedError` | "5000" | Internal | Error inesperado no controlado |

**Namespace:** `WePlayRises.Crowdpromotion.Domain.Constants`

---

### 3.3 Repository Interfaces (Ports)

#### IPromotorRepository (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromotorRepository.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Domain.Interfaces`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByIdAsync` | `Promotor?` | `PromotorId id, CancellationToken ct` | Obtener por PK |
| `GetByUserIdAsync` | `Promotor?` | `string userId, CancellationToken ct` | Obtener por UserId (clave de negocio) |
| `GetByUserIdWithWalletAsync` | `Promotor?` | `string userId, CancellationToken ct` | Obtener con Include(Wallets) para GET /me |
| `AddAsync` | `PromotorId` | `Promotor entity, CancellationToken ct` | Crear nuevo, retornar ID generado |
| `UpdateAsync` | `void` | `Promotor entity, CancellationToken ct` | Actualizar entidad existente |
| `ExistsByUserIdAsync` | `bool` | `string userId, CancellationToken ct` | Verificar si ya existe un promotor para el UserId |
| `GetActiveProgramasAsync` | `IReadOnlyList<PromoProgramaPromotor>` | `PromotorId promotorId, CancellationToken ct` | Obtener programas activos para desactivacion en cascada |

**Notas de diseno:**
- `GetByUserIdWithWalletAsync` hace Include de `Wallets` para evitar un segundo query en el Handler de GET /me
- `GetActiveProgramasAsync` carga los `PromoProgramaPromotor` activos; el Handler los marca como inactivos y llama `UpdateAsync` por cada uno (o bulk update en Repository)
- El Repository hace `SaveChangesAsync` en cada operacion de escritura

---

#### IPromotorWalletRepository (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Domain/Interfaces/IPromotorWalletRepository.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Domain.Interfaces`

| Metodo | Retorno | Parametros | Descripcion |
|--------|---------|------------|-------------|
| `GetByPromotorIdAndMonedaAsync` | `PromotorWallet?` | `PromotorId promotorId, int monedaId, CancellationToken ct` | Obtener wallet por promotor + moneda |
| `AddAsync` | `Guid` | `PromotorWallet entity, CancellationToken ct` | Crear nueva wallet, retornar ID |

**Notas de diseno:**
- Solo se necesitan estos dos metodos para US-CP-01
- La wallet se crea en la misma transaccion que el Promotor (via `IPromotorService.CreateWithWalletAsync`)
- Operaciones de debito/credito quedan para features futuras (US-CP-04)

---

## 4. Infrastructure Layer

### 4.1 Repository Implementations

#### PromotorRepository (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromotorRepository.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Repositories`

- Implementa: `IPromotorRepository`
- Inyecta: `CrowdpromotionContext` (via constructor con `?? throw`)
- `SaveChangesAsync`: Llamado en cada operacion de escritura (`AddAsync`, `UpdateAsync`)
- Queries de lectura usan `AsNoTracking()`

| Metodo | Implementacion clave |
|--------|---------------------|
| `GetByIdAsync` | `_context.Promotores.FirstOrDefaultAsync(x => x.Id == id, ct)` |
| `GetByUserIdAsync` | `_context.Promotores.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct)` |
| `GetByUserIdWithWalletAsync` | `_context.Promotores.Include(p => p.Wallets).FirstOrDefaultAsync(x => x.UserId == userId, ct)` - sin AsNoTracking para operaciones de escritura posterior |
| `AddAsync` | `_context.Promotores.AddAsync(entity, ct)` + `SaveChangesAsync` |
| `UpdateAsync` | `_context.Promotores.Update(entity)` + `SaveChangesAsync` |
| `ExistsByUserIdAsync` | `_context.Promotores.AnyAsync(x => x.UserId == userId, ct)` |
| `GetActiveProgramasAsync` | `_context.ProgramaPromotores.AsNoTracking().Where(x => x.PromotorId == promotorId && x.EsActivo).ToListAsync(ct)` |

---

#### PromotorWalletRepository (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Repositories/PromotorWalletRepository.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Repositories`

- Implementa: `IPromotorWalletRepository`
- Inyecta: `CrowdpromotionContext` (via constructor con `?? throw`)

| Metodo | Implementacion clave |
|--------|---------------------|
| `GetByPromotorIdAndMonedaAsync` | `_context.Wallets.AsNoTracking().FirstOrDefaultAsync(x => x.PromotorId == promotorId && x.MonedaId == monedaId, ct)` |
| `AddAsync` | `_context.Wallets.AddAsync(entity, ct)` + `SaveChangesAsync` |

---

### 4.2 Services

#### IPromotorService (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorService.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Interfaces.Services`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetByIdAsync` | `Promotor?` | Obtener por PK, con caching |
| `GetByUserIdAsync` | `Promotor?` | Obtener por UserId, con caching. Usado por Validator y Handler en el mismo request |
| `GetByUserIdWithWalletAsync` | `Promotor?` | Obtener con wallet para GET /me. Sin caching (datos calculados en tiempo real) |
| `ExistsByUserIdAsync` | `bool` | Verificar unicidad antes de crear. Con caching via GetByUserIdAsync |
| `CreateWithWalletAsync` | `PromotorId` | Crear Promotor + PromotorWallet EUR en una transaccion. Retorna el PromotorId |
| `UpdateAsync` | `void` | Actualizar campos editables (NombrePublico, EmailContacto, URLs) |
| `DesactivarAsync` | `int` | Establecer EsActivo=false y desactivar programas activos. Retorna cantidad de programas dados de baja |

**CRITICO:** El servicio retorna **entidades**, no DTOs. El Handler hace el mapping.

---

#### PromotorService (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromotorService.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Services`

- Implementa: `IPromotorService`
- Ubicacion de interfaz: `Application/Interfaces/Services/` (sigue patron Crowdfunding donde interfaces de servicio viven en Application)

**Dependencias del constructor (todas con `?? throw new ArgumentNullException`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_promotorRepository` | `IPromotorRepository` | Acceso a datos de Promotor |
| `_walletRepository` | `IPromotorWalletRepository` | Creacion de wallet en transaccion |
| `_requestCache` | `IRequestCacheService` | Caching por request para evitar queries duplicados |
| `_logger` | `ILogger<PromotorService>` | Logging de operaciones |

**Comportamiento por metodo:**

| Metodo | Cache Key | Comportamiento |
|--------|-----------|----------------|
| `GetByIdAsync` | `"promotor:{id.Value}"` | GetOrAddAsync con repository |
| `GetByUserIdAsync` | `"promotor:userid:{userId}"` | GetOrAddAsync con repository. Compartido entre Validator y Handler en el mismo request HTTP |
| `GetByUserIdWithWalletAsync` | Sin cache | Carga Promotor + Wallets. Los datos de wallet son calculados en tiempo real |
| `ExistsByUserIdAsync` | Reutiliza `GetByUserIdAsync` | `var promotor = await GetByUserIdAsync(userId, ct); return promotor != null` |
| `CreateWithWalletAsync` | Invalida cache | Establece `FechaCreacion = DateTime.UtcNow`, `EsActivo = true`; llama `_promotorRepository.AddAsync`; crea `PromotorWallet` con `MonedaId=1`, saldos en 0; llama `_walletRepository.AddAsync`; loguea con `_logger.LogInformation` |
| `UpdateAsync` | Invalida cache | Establece `FechaActualizacion = DateTime.UtcNow`; llama `_promotorRepository.UpdateAsync`; loguea |
| `DesactivarAsync` | Invalida cache | Carga programas activos; los marca `EsActivo=false` y `FechaBaja=UtcNow`; actualiza Promotor `EsActivo=false`; retorna count de programas afectados |

**NOTA CRITICA sobre transaccion en `CreateWithWalletAsync`:**
Los repositorios hacen `SaveChangesAsync` individualmente. Para que la creacion de `Promotor` y `PromotorWallet` sea atomica, el servicio debe ejecutar ambas operaciones dentro de una transaccion de EF Core (`context.Database.BeginTransactionAsync`). El `PromotorService` necesita acceso a la transaccion. Opciones:

- **Opcion A (recomendada):** `PromotorService` inyecta `CrowdpromotionContext` SOLO para gestionar la transaccion; los repositorios hacen el trabajo de datos. Esto mantiene la separacion de responsabilidades.
- **Opcion B:** El Handler usa `IUnitOfWork<CrowdpromotionContext>` y el Service no hace commit directamente.

Se recomienda **Opcion A** para mantener la atomicidad en el Service sin exponer el DbContext a los Handlers. La inyeccion de contexto en el Service es aceptable porque el Service ES de infraestructura (vive en Infra).

---

#### IPromotorWalletService (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Application/Interfaces/Services/IPromotorWalletService.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Application.Interfaces.Services`

Para US-CP-01, este servicio expone un unico metodo de consulta usado por el GET /me:

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| `GetWalletEurByPromotorIdAsync` | `PromotorWallet?` | Obtener wallet EUR (MonedaId=1) del promotor para calcular TotalComisionesGanadas |

**Nota:** La creacion de wallet se delega a `IPromotorService.CreateWithWalletAsync` para mantener la atomicidad. Este servicio solo provee consulta.

---

#### PromotorWalletService (NUEVO)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Services/PromotorWalletService.cs`

**Namespace:** `WePlayRises.Crowdpromotion.Infra.Services`

**Dependencias del constructor (todas con `?? throw`):**

| Dependencia | Tipo | Proposito |
|------------|------|-----------|
| `_walletRepository` | `IPromotorWalletRepository` | Acceso a datos de wallet |
| `_requestCache` | `IRequestCacheService` | Caching |
| `_logger` | `ILogger<PromotorWalletService>` | Logging |

| Metodo | Cache Key | Comportamiento |
|--------|-----------|----------------|
| `GetWalletEurByPromotorIdAsync` | `"promotorwallet:{promotorId.Value}:moneda:1"` | GetOrAddAsync con repository filtrando `MonedaId == 1` |

---

### 4.3 Configuracion EF Core (Modificacion en CrowdpromotionContext)

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/Context/CrowdpromotionContext.cs`

La configuracion de `Promotor` en `OnModelCreating` debe modificarse para:

1. Ampliar `NombrePublico` de `HasMaxLength(100)` a `HasMaxLength(200)`
2. Agregar configuracion para `EmailContacto`
3. Agregar configuracion para `UrlSitioWeb`
4. Ampliar URLs de redes de `HasMaxLength(200)` a `HasMaxLength(300)`

**Cambios en el bloque `modelBuilder.Entity<Promotor>` existente:**

| Propiedad | Configuracion Actual | Configuracion Nueva |
|-----------|---------------------|---------------------|
| `NombrePublico` | `.HasMaxLength(100).IsRequired()` | `.HasMaxLength(200).IsRequired()` |
| `EmailContacto` | (no existe) | `.HasMaxLength(200)` - nullable implicito |
| `UrlSitioWeb` | (no existe) | `.HasMaxLength(300)` - nullable implicito |
| `UrlInstagram` | `.HasMaxLength(200)` | `.HasMaxLength(300)` |
| `UrlTikTok` | `.HasMaxLength(200)` | `.HasMaxLength(300)` |
| `UrlTwitter` | `.HasMaxLength(200)` | `.HasMaxLength(300)` |
| `UrlYouTube` | `.HasMaxLength(200)` | `.HasMaxLength(300)` |

**Precision de fechas (ya configuradas):**
- `FechaCreacion.HasPrecision(3)` - ya existe
- `FechaActualizacion.HasPrecision(3)` - ya existe

**Indice unico en UserId (ya existe):**
```
entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_Promotor_UserId")
```

**DbSets existentes (sin cambios):**
- `DbSet<Promotor> Promotores` - ya existe
- `DbSet<PromotorWallet> Wallets` - ya existe
- `DbSet<PromoProgramaPromotor> ProgramaPromotores` - ya existe

---

### 4.4 Migracion EF Core

**Nombre sugerido:** `AddPromotorEmailContactoUrlSitioWeb`

**Descripcion de cambios en schema:**

| Tabla | Operacion | Columna | Tipo SQL | Nullable |
|-------|-----------|---------|----------|----------|
| `Promotor` | `AddColumn` | `EmailContacto` | `nvarchar(200)` | YES |
| `Promotor` | `AddColumn` | `UrlSitioWeb` | `nvarchar(300)` | YES |
| `Promotor` | `AlterColumn` | `NombrePublico` | `nvarchar(200)` | NO |
| `Promotor` | `AlterColumn` | `UrlInstagram` | `nvarchar(300)` | YES |
| `Promotor` | `AlterColumn` | `UrlTikTok` | `nvarchar(300)` | YES |
| `Promotor` | `AlterColumn` | `UrlTwitter` | `nvarchar(300)` | YES |
| `Promotor` | `AlterColumn` | `UrlYouTube` | `nvarchar(300)` | YES |

**Impacto en datos existentes:** Ninguno. Las nuevas columnas son nullable. Las ampliaciones de maxlength no afectan datos existentes (solo amplian el limite).

**Comando de generacion:**
```bash
dotnet ef migrations add AddPromotorEmailContactoUrlSitioWeb \
  --project src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra \
  --startup-project src/api/WebApi \
  --context CrowdpromotionContext
```

**Comando de aplicacion:**
```bash
dotnet ef database update \
  --project src/api/Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra \
  --startup-project src/api/WebApi \
  --context CrowdpromotionContext
```

---

### 4.5 Dependency Injection

**Archivo:** `Modules/Crowdpromotion/WePlayRises.Crowdpromotion.Infra/DependencyInjection.cs`

El archivo existe pero esta vacio. Debe modificarse para registrar todos los servicios de esta feature:

**Registros a agregar:**

```
// Repositories
services.AddScoped<IPromotorRepository, PromotorRepository>();
services.AddScoped<IPromotorWalletRepository, PromotorWalletRepository>();

// Services
services.AddScoped<IPromotorService, PromotorService>();
services.AddScoped<IPromotorWalletService, PromotorWalletService>();
```

**Usings necesarios:**
- `WePlayRises.Crowdpromotion.Application.Interfaces.Services`
- `WePlayRises.Crowdpromotion.Domain.Interfaces`
- `WePlayRises.Crowdpromotion.Infra.Repositories`
- `WePlayRises.Crowdpromotion.Infra.Services`

---

## 5. Patron de Caching (ADR-006)

El patron de caching aplicado sigue el de `CampaniaService` y `PedidoService` del modulo Crowdfunding:

```
Validator llama PromotorService.GetByUserIdAsync(userId)
  └─> Service consulta RequestCache con key "promotor:userid:{userId}"
      └─> Cache MISS -> PromotorRepository -> CrowdpromotionContext -> DB
      └─> Almacena resultado en cache del request

Handler llama PromotorService.GetByUserIdAsync(userId)
  └─> Service consulta RequestCache con key "promotor:userid:{userId}"
      └─> Cache HIT -> Retorna inmediato (sin DB)
```

**Beneficio:** El `CreatePromotorCommandValidator` puede verificar si ya existe un `Promotor` para el `UserId` (para retornar error 4018) sin hacer un segundo hit a BD cuando el Handler tambien lo consulte.

**Cache keys definidos:**

| Key Pattern | Entidad | Descripcion |
|------------|---------|-------------|
| `"promotor:{id.Value}"` | Promotor | Por PromotorId (Strongly Typed) |
| `"promotor:userid:{userId}"` | Promotor | Por UserId (string) - clave principal de negocio |
| `"promotorwallet:{promotorId.Value}:moneda:{monedaId}"` | PromotorWallet | Por PromotorId + MonedaId |

---

## 6. Flujo de Datos

```
Controller
    |
    v
MediatR
    |
    v
[CreatePromotorCommandHandler]
    |
    |---> IValidator<CreatePromotorCommand>
    |         |
    |         |---> IPromotorService.ExistsByUserIdAsync()  [cache: "promotor:userid:{userId}"]
    |         |         └-> PromotorRepository.ExistsByUserIdAsync() [solo si cache MISS]
    |         |
    |         └-> IPromotorService via ICoreService (tipoPromotorId existe?)
    |             [validacion de FK contra MaestraTipoPromotor del modulo Core]
    |
    |---> IPromotorService.CreateWithWalletAsync(promotor, wallet, ct)
    |         |
    |         |---> PromotorRepository.AddAsync(promotor, ct) [+ SaveChanges]
    |         └---> PromotorWalletRepository.AddAsync(wallet, ct) [+ SaveChanges, misma TX]
    |
    └---> IMapper.Map<PromotorCreatedResultDto>(promotor)
              |
              v
         ServiceResponse<PromotorCreatedResultDto>

---

[GetPromotorMeQueryHandler]
    |
    |---> IPromotorService.GetByUserIdWithWalletAsync(userId, ct)
    |         |
    |         └-> PromotorRepository.GetByUserIdWithWalletAsync() [Include(Wallets)]
    |
    |---> IPromotorWalletService.GetWalletEurByPromotorIdAsync()
    |         └-> [ya cargada via Include, acceso directo a promotor.Wallets]
    |
    |---> COUNT de PromoProgramaPromotor activos
    |         └-> PromotorRepository.GetActiveProgramasAsync(promotorId, ct)
    |
    └---> IMapper.Map<PromotorDto>(promotor) + campos calculados
```

---

## 7. Consideraciones sobre MaestraTipoPromotor

La tabla `Maestra_TipoPromotor` ya existe y es gestionada por el modulo `Core` (`CoreContext`). Para la validacion de `TipoPromotorId` en el `CreatePromotorCommandValidator`, hay dos opciones:

**Opcion A (recomendada para MVP):** El Validator consulta directamente la tabla maestra via un servicio del modulo Core o via un repository de solo lectura de maestras. El modulo Core ya expone `MaestraTipoPromotor` en `CoreContext`.

**Opcion B (alternativa):** Usar un `IMaestraService` de Core con metodo `TipoPromotorExistsAsync(int id, CancellationToken ct)`.

Para el MVP, dado que los valores son fijos (seed: 1=Fan Embajador, 2=Influencer, 3=Medio/Blog, 4=Profesional Marketing), el Validator puede usar un conjunto hardcodeado de valores validos `{1, 2, 3, 4}` sin consultar BD. La logica seria `Must(id => id >= 1 && id <= 4)` con `WithErrorCode(Validation_ForeignKeyNotFound)`.

**Recomendacion para el plan:** Usar validacion con rango hardcodeado (valores 1-4) para MVP. Si en el futuro los tipos son dinamicos, refactorizar a consulta de BD.

---

## 8. Estructura de Archivos

```
Modules/Crowdpromotion/
|
├── WePlayRises.Crowdpromotion.Domain/
│   ├── Model/
│   │   ├── Promotor.cs                          [MODIFICAR - agregar EmailContacto, UrlSitioWeb]
│   │   ├── PromotorWallet.cs                    [SIN CAMBIOS]
│   │   └── PromoProgramaPromotor.cs             [SIN CAMBIOS]
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs        [CREAR NUEVO]
│   └── Interfaces/
│       ├── IPromotorRepository.cs               [CREAR NUEVO]
│       └── IPromotorWalletRepository.cs         [CREAR NUEVO]
│
├── WePlayRises.Crowdpromotion.Application/
│   └── Interfaces/
│       └── Services/
│           ├── IPromotorService.cs              [CREAR NUEVO]
│           └── IPromotorWalletService.cs        [CREAR NUEVO]
│
└── WePlayRises.Crowdpromotion.Infra/
    ├── Repositories/
    │   ├── PromotorRepository.cs                [CREAR NUEVO]
    │   └── PromotorWalletRepository.cs          [CREAR NUEVO]
    ├── Services/
    │   ├── PromotorService.cs                   [CREAR NUEVO]
    │   └── PromotorWalletService.cs             [CREAR NUEVO]
    ├── Context/
    │   └── CrowdpromotionContext.cs             [MODIFICAR - nuevas columnas Promotor]
    ├── Migrations/
    │   └── {timestamp}_AddPromotorEmailContactoUrlSitioWeb.cs  [GENERAR]
    └── DependencyInjection.cs                   [MODIFICAR - registrar repos y services]
```

---

## 9. Dependencias entre Proyectos

| Proyecto | Referencia | Elementos usados |
|---------|-----------|-----------------|
| `Crowdpromotion.Domain` | `BuildingBlocks.Abstractions` | `IRepositoryBase`, Strongly Typed IDs |
| `Crowdpromotion.Domain` | `BuildingBlocks.EntityFramework` | `PromotorId`, `FanProfileId` (StronglyTypedIds) |
| `Crowdpromotion.Application` | `Crowdpromotion.Domain` | Entidades, Interfaces de Repository, Constants |
| `Crowdpromotion.Application` | `BuildingBlocks.Kernel` | `ServiceResponse`, `ServiceResponseMessage` |
| `Crowdpromotion.Infra` | `Crowdpromotion.Domain` | Entidades, Interfaces de Repository |
| `Crowdpromotion.Infra` | `Crowdpromotion.Application` | Interfaces de Service |
| `Crowdpromotion.Infra` | `BuildingBlocks.Caching` | `IRequestCacheService` |
| `Crowdpromotion.Infra` | `BuildingBlocks.EntityFramework` | `CrowdpromotionContext`, `IUnitOfWork` |

---

## 10. Checklist

- [ ] `Promotor.cs` modificado: `EmailContacto` y `UrlSitioWeb` agregados como propiedades nullable
- [ ] `Promotor.cs` modificado: `NombrePublico` documentado como max 200 (cambio en config, no en POCO)
- [ ] `ServiceResponseMessageType.cs` creado en `Domain/Constants/` con codigos 2015, 4018, 4019
- [ ] `IPromotorRepository` creado en `Domain/Interfaces/` con todos los metodos documentados
- [ ] `IPromotorWalletRepository` creado en `Domain/Interfaces/` con los dos metodos necesarios
- [ ] `IPromotorService` creado en `Application/Interfaces/Services/` con retorno de entidades (no DTOs)
- [ ] `IPromotorWalletService` creado en `Application/Interfaces/Services/`
- [ ] `PromotorRepository` creado en `Infra/Repositories/` con `?? throw` en constructor
- [ ] `PromotorWalletRepository` creado en `Infra/Repositories/` con `?? throw` en constructor
- [ ] `PromotorService` creado en `Infra/Services/` con 4 dependencias: Repository + WalletRepository + Cache + Logger
- [ ] `PromotorWalletService` creado en `Infra/Services/` con 3 dependencias: Repository + Cache + Logger
- [ ] `CrowdpromotionContext.cs` modificado con nuevas columnas y maxlengths actualizados
- [ ] Migracion `AddPromotorEmailContactoUrlSitioWeb` generada y aplicada
- [ ] `DependencyInjection.cs` modificado con registro de 2 repositorios y 2 servicios
- [ ] Entidades son POCOs (sin metodos de negocio)
- [ ] Services retornan entidades (no DTOs)
- [ ] Todos los constructores usan `?? throw new ArgumentNullException`
- [ ] `GetByUserIdAsync` en Service usa `IRequestCacheService` para compartir cache entre Validator y Handler
