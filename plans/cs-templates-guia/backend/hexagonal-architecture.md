# Arquitectura Hexagonal: Templates y Guia para Artistas Noveles

**Fecha:** 2026-02-15
**Modulo:** Crowdsourcing
**Feature:** cs-templates-guia (US-CS-01)

---

## 1. Resumen Ejecutivo

Esta feature transforma a WePlay Rises en un "mentor digital" para artistas noveles. En lugar de enfrentar a un artista con un formulario vacio, le ofrecemos una galeria de templates de proyectos musicales pre-configurados (ej: "Grabar un Album", "Produccion de Videoclip") con desglose completo de necesidades profesionales, roles recomendados y precios orientativos de mercado.

El artista selecciona un template, personaliza presupuestos, y al confirmar, el sistema crea automaticamente multiples `NecesidadCrowdsourcing` en una sola transaccion.

**Entidades nuevas:** 5 (PlantillaProyecto, PlantillaProyectoNecesidad, MaestraRolProfesional, MaestraCategoriaRol, MaestraTipoEmpresa)

---

## 2. Domain Layer

### 2.1 Constants (ErrorCodes)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Constants/ServiceResponseMessageType.cs`

**CRITICO:** Usar template de `.claude/templates/api/Domain/Constants/ServiceResponseMessageType.template.cs`

```csharp
namespace WePlayRises.Crowdsourcing.Domain.Constants;

public static class ServiceResponseMessageType
{
    // ========== Success (0000-0999) ==========
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // ========== Validation (1000-1999) ==========
    public const string Validation_Required = "1001";
    public const string Validation_MaxLength = "1002";
    public const string Validation_InvalidEmail = "1003";
    public const string Validation_InvalidRange = "1009"; // Para presupuesto max < min
    public const string Validation_ForeignKeyNotFound = "1010";

    // ========== NotFound (2000-2999) ==========
    public const string NotFound_Entity = "2000";
    public const string NotFound_PlantillaProyecto = "2006"; // NUEVO - Plantilla no existe
    public const string NotFound_ProyectoArtistico = "2007"; // NUEVO - Proyecto artistico no existe
    public const string NotFound_PlantillaNecesidad = "2008"; // NUEVO - Necesidad de plantilla no existe

    // ========== Auth (3000-3999) ==========
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002"; // NUEVO - Proyecto no pertenece al artista

    // ========== Business Rules (4000-4999) ==========
    public const string BusinessRule_InvalidOperation = "4000";

    // ========== Internal (5000-5999) ==========
    public const string Internal_UnexpectedError = "5000";
    public const string Internal_DatabaseError = "5001";
}
```

**Proposito:** Centralizar todos los ErrorCodes numericos usados en validators y handlers del modulo Crowdsourcing.

---

### 2.2 Entidades (POCOs)

#### PlantillaProyecto

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/PlantillaProyecto.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | Guid | No | PK - Identificador unico de plantilla |
| Nombre | string | No | Nombre del template (ej: "Grabar un Album / EP") |
| Descripcion | string | Si | Texto explicativo para artista novel |
| Icono | string | Si | Nombre del icono (ej: "music", "video") |
| Orden | int | No | Orden de presentacion en UI (1, 2, 3...) |
| Activo | bool | No | Para ocultar templates sin borrar (default: true) |
| FechaCreacion | DateTime | No | Fecha de creacion del template |

**Navegaciones:**
- `Necesidades` -> `ICollection<PlantillaProyectoNecesidad>` (1:N)

**Codigo:**
```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Representa un template de proyecto musical pre-configurado (ej: "Grabar un Album").
/// </summary>
public class PlantillaProyecto
{
    public PlantillaProyectoId Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual ICollection<PlantillaProyectoNecesidad> Necesidades { get; set; } = new List<PlantillaProyectoNecesidad>();
}
```

---

#### PlantillaProyectoNecesidad

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/PlantillaProyectoNecesidad.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | Guid | No | PK - Identificador unico de necesidad |
| PlantillaProyectoId | Guid | No | FK a PlantillaProyecto |
| Fase | string | No | Fase del proyecto (ej: "Pre-produccion", "Grabacion") |
| Titulo | string | No | Titulo de necesidad (ej: "Mezcla de pistas") |
| Descripcion | string | Si | Explicacion para artista novel |
| RolProfesionalId | int | No | FK a MaestraRolProfesional |
| PrecioMinOrientativo | decimal | Si | Precio minimo orientativo en moneda |
| PrecioMaxOrientativo | decimal | Si | Precio maximo orientativo en moneda |
| MonedaId | int | No | FK a Core.MaestraMoneda (1=EUR) |
| Prioridad | string | No | "Alta", "Media", "Baja" (string, no enum) |
| Orden | int | No | Orden dentro del template (1, 2, 3...) |
| FechaCreacion | DateTime | No | Fecha de creacion |

**Navegaciones:**
- `PlantillaProyecto` -> `PlantillaProyecto` (N:1)
- `RolProfesional` -> `MaestraRolProfesional` (N:1)

**Codigo:**
```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Representa una necesidad profesional dentro de un template.
/// </summary>
public class PlantillaProyectoNecesidad
{
    public PlantillaProyectoNecesidadId Id { get; set; }

    public PlantillaProyectoId PlantillaProyectoId { get; set; }

    public string Fase { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int RolProfesionalId { get; set; }

    public decimal? PrecioMinOrientativo { get; set; }

    public decimal? PrecioMaxOrientativo { get; set; }

    public int MonedaId { get; set; } = 1; // Default EUR

    /// <summary>
    /// Prioridad de la necesidad: "Alta", "Media", "Baja"
    /// IMPORTANTE: Usar strings, NO enums
    /// </summary>
    public string Prioridad { get; set; } = "Media";

    public int Orden { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Navigation properties
    public virtual PlantillaProyecto PlantillaProyecto { get; set; } = null!;
    public virtual MaestraRolProfesional RolProfesional { get; set; } = null!;
}
```

---

#### MaestraRolProfesional

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MaestraRolProfesional.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | int | No | PK - Identificador unico (maestra int) |
| Nombre | string | No | Nombre del rol (ej: "Productor musical") |
| Descripcion | string | Si | Que hace este rol (para tooltip educativo) |
| CategoriaRolId | int | No | FK a MaestraCategoriaRol |
| ModalidadCobro | string | Si | Ej: "Por proyecto", "Por dia", "Por cancion" |
| Activo | bool | No | Para ocultar roles sin borrar (default: true) |

**Navegaciones:**
- `CategoriaRol` -> `MaestraCategoriaRol` (N:1)
- `PlantillasNecesidades` -> `ICollection<PlantillaProyectoNecesidad>` (1:N)

**Codigo:**
```csharp
namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Catalogo de roles profesionales en la industria musical.
/// Tabla maestra con PK int.
/// </summary>
public class MaestraRolProfesional
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int CategoriaRolId { get; set; }

    /// <summary>
    /// Modalidad de cobro tipica: "Por proyecto", "Por dia", "Por cancion", etc.
    /// </summary>
    public string? ModalidadCobro { get; set; }

    public bool Activo { get; set; } = true;

    // Navigation properties
    public virtual MaestraCategoriaRol CategoriaRol { get; set; } = null!;
    public virtual ICollection<PlantillaProyectoNecesidad> PlantillasNecesidades { get; set; } = new List<PlantillaProyectoNecesidad>();
}
```

---

#### MaestraCategoriaRol

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MaestraCategoriaRol.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | int | No | PK - Identificador unico (maestra int) |
| Nombre | string | No | Nombre categoria (ej: "Produccion Musical") |
| Icono | string | Si | Icono de la categoria (ej: "music", "video") |
| Orden | int | No | Orden de presentacion (1, 2, 3...) |

**Navegaciones:**
- `Roles` -> `ICollection<MaestraRolProfesional>` (1:N)

**Codigo:**
```csharp
namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Categorias de roles profesionales (6 categorias en seed).
/// Tabla maestra con PK int.
/// </summary>
public class MaestraCategoriaRol
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Icono { get; set; }

    public int Orden { get; set; }

    // Navigation properties
    public virtual ICollection<MaestraRolProfesional> Roles { get; set; } = new List<MaestraRolProfesional>();
}
```

---

#### MaestraTipoEmpresa (Opcional - Futuro)

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Model/MaestraTipoEmpresa.cs`

| Propiedad | Tipo | Nullable | Descripcion |
|-----------|------|----------|-------------|
| Id | int | No | PK - Identificador unico (maestra int) |
| Nombre | string | No | Nombre tipo empresa (ej: "Estudio de grabacion") |
| Descripcion | string | Si | Descripcion del tipo |
| Activo | bool | No | Para ocultar sin borrar (default: true) |

**NOTA:** Esta entidad es opcional para MVP. Se incluye en el plan para futuro sin implementacion inmediata.

**Codigo:**
```csharp
namespace WePlayRises.Crowdsourcing.Domain.Model;

/// <summary>
/// Tipos de empresas profesionales (opcional para futuro).
/// NO implementar en MVP.
/// </summary>
public class MaestraTipoEmpresa
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; } = true;
}
```

---

### 2.3 Repository Interfaces (Ports)

#### IPlantillaProyectoRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Interfaces/IPlantillaProyectoRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | PlantillaProyecto? | Obtener template por ID con Include de necesidades y roles |
| GetAllActivosAsync | IReadOnlyList<PlantillaProyecto> | Listar templates activos (sin necesidades) |
| GetByIdWithNecesidadesAsync | PlantillaProyecto? | Obtener con Include de necesidades, roles y categorias |
| AddAsync | PlantillaProyectoId | Crear nuevo template (admin solo) |
| UpdateAsync | void | Actualizar template (admin solo) |
| DeleteAsync | void | Eliminar logico (poner Activo = false) |

**Codigo:**
```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Domain.Interfaces;

public interface IPlantillaProyectoRepository
{
    /// <summary>
    /// Obtener template por ID sin navegaciones.
    /// </summary>
    Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct);

    /// <summary>
    /// Obtener template por ID con Include de Necesidades + RolProfesional + CategoriaRol.
    /// Usado en Query GetTemplateByIdQuery.
    /// </summary>
    Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct);

    /// <summary>
    /// Listar templates activos (Activo = true) sin navegaciones.
    /// Usado en Query GetTemplatesQuery.
    /// </summary>
    Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct);

    /// <summary>
    /// Crear nuevo template (admin solo, no MVP).
    /// </summary>
    Task<PlantillaProyectoId> AddAsync(PlantillaProyecto entity, CancellationToken ct);

    /// <summary>
    /// Actualizar template existente (admin solo, no MVP).
    /// </summary>
    Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct);

    /// <summary>
    /// Eliminar logico: poner Activo = false (admin solo, no MVP).
    /// </summary>
    Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct);
}
```

---

#### IPlantillaProyectoNecesidadRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Interfaces/IPlantillaProyectoNecesidadRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | PlantillaProyectoNecesidad? | Obtener necesidad por ID |
| GetByPlantillaIdAsync | IReadOnlyList<PlantillaProyectoNecesidad> | Listar necesidades de un template |
| GetByIdsAsync | IReadOnlyList<PlantillaProyectoNecesidad> | Obtener multiples necesidades por IDs |
| AddAsync | PlantillaProyectoNecesidadId | Crear nueva necesidad (admin) |
| UpdateAsync | void | Actualizar necesidad (admin) |
| DeleteAsync | void | Eliminar necesidad (admin) |

**Codigo:**
```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Domain.Interfaces;

public interface IPlantillaProyectoNecesidadRepository
{
    Task<PlantillaProyectoNecesidad?> GetByIdAsync(PlantillaProyectoNecesidadId id, CancellationToken ct);

    /// <summary>
    /// Obtener todas las necesidades de un template especifico.
    /// </summary>
    Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByPlantillaIdAsync(PlantillaProyectoId plantillaId, CancellationToken ct);

    /// <summary>
    /// Obtener multiples necesidades por IDs (usado en validacion de GenerarNecesidadesCommand).
    /// </summary>
    Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByIdsAsync(IEnumerable<PlantillaProyectoNecesidadId> ids, CancellationToken ct);

    Task<PlantillaProyectoNecesidadId> AddAsync(PlantillaProyectoNecesidad entity, CancellationToken ct);

    Task UpdateAsync(PlantillaProyectoNecesidad entity, CancellationToken ct);

    Task DeleteAsync(PlantillaProyectoNecesidadId id, CancellationToken ct);
}
```

---

#### IMaestraRolProfesionalRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Interfaces/IMaestraRolProfesionalRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | MaestraRolProfesional? | Obtener rol por ID |
| GetAllActivosAsync | IReadOnlyList<MaestraRolProfesional> | Listar roles activos con categoria |
| GetByCategoriaIdAsync | IReadOnlyList<MaestraRolProfesional> | Listar roles de una categoria |
| AddAsync | int | Crear nuevo rol (admin) |
| UpdateAsync | void | Actualizar rol (admin) |

**Codigo:**
```csharp
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Domain.Interfaces;

public interface IMaestraRolProfesionalRepository
{
    Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Listar roles activos con Include de CategoriaRol.
    /// Usado en Query GetRolesProfesionalesQuery.
    /// </summary>
    Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct);

    /// <summary>
    /// Listar roles de una categoria especifica.
    /// </summary>
    Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct);

    Task<int> AddAsync(MaestraRolProfesional entity, CancellationToken ct);

    Task UpdateAsync(MaestraRolProfesional entity, CancellationToken ct);
}
```

---

#### IMaestraCategoriaRolRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Domain/Interfaces/IMaestraCategoriaRolRepository.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | MaestraCategoriaRol? | Obtener categoria por ID |
| GetAllAsync | IReadOnlyList<MaestraCategoriaRol> | Listar todas las categorias ordenadas |
| AddAsync | int | Crear nueva categoria (admin) |
| UpdateAsync | void | Actualizar categoria (admin) |

**Codigo:**
```csharp
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Domain.Interfaces;

public interface IMaestraCategoriaRolRepository
{
    Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Listar todas las categorias ordenadas por Orden.
    /// Usado en Query GetCategoriasRolQuery.
    /// </summary>
    Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct);

    Task<int> AddAsync(MaestraCategoriaRol entity, CancellationToken ct);

    Task UpdateAsync(MaestraCategoriaRol entity, CancellationToken ct);
}
```

---

## 3. Infrastructure Layer

### 3.1 Repository Implementations

#### PlantillaProyectoRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/PlantillaProyectoRepository.cs`

- **Implementa:** `IPlantillaProyectoRepository`
- **Inyecta:** `CrowdsourcingContext`
- **SaveChanges:** En cada operacion de escritura (AddAsync, UpdateAsync, DeleteAsync)

**Metodos clave:**
- `GetByIdWithNecesidadesAsync`: Usa `.Include(x => x.Necesidades).ThenInclude(n => n.RolProfesional).ThenInclude(r => r.CategoriaRol)` para cargar grafo completo
- `GetAllActivosAsync`: Usa `.AsNoTracking()` y filtra `Activo == true`
- `DeleteAsync`: Eliminar logico (SET Activo = false, NO DELETE FROM)

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class PlantillaProyectoRepository : IPlantillaProyectoRepository
{
    private readonly CrowdsourcingContext _context;

    public PlantillaProyectoRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .Include(x => x.Necesidades)
                .ThenInclude(n => n.RolProfesional)
                    .ThenInclude(r => r.CategoriaRol)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct)
    {
        return await _context.PlantillasProyecto
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }

    public async Task<PlantillaProyectoId> AddAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        await _context.PlantillasProyecto.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        _context.PlantillasProyecto.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        // Eliminar logico: SET Activo = false
        var entity = await _context.PlantillasProyecto.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            entity.Activo = false;
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

---

#### PlantillaProyectoNecesidadRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/PlantillaProyectoNecesidadRepository.cs`

- **Implementa:** `IPlantillaProyectoNecesidadRepository`
- **Inyecta:** `CrowdsourcingContext`
- **SaveChanges:** En cada operacion de escritura

**Metodos clave:**
- `GetByIdsAsync`: Usa `.Where(x => ids.Contains(x.Id))` para validacion bulk en Command

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class PlantillaProyectoNecesidadRepository : IPlantillaProyectoNecesidadRepository
{
    private readonly CrowdsourcingContext _context;

    public PlantillaProyectoNecesidadRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PlantillaProyectoNecesidad?> GetByIdAsync(PlantillaProyectoNecesidadId id, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByPlantillaIdAsync(PlantillaProyectoId plantillaId, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .Where(x => x.PlantillaProyectoId == plantillaId)
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PlantillaProyectoNecesidad>> GetByIdsAsync(IEnumerable<PlantillaProyectoNecesidadId> ids, CancellationToken ct)
    {
        return await _context.PlantillasProyectoNecesidades
            .AsNoTracking()
            .Include(x => x.RolProfesional)
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }

    public async Task<PlantillaProyectoNecesidadId> AddAsync(PlantillaProyectoNecesidad entity, CancellationToken ct)
    {
        await _context.PlantillasProyectoNecesidades.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(PlantillaProyectoNecesidad entity, CancellationToken ct)
    {
        _context.PlantillasProyectoNecesidades.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(PlantillaProyectoNecesidadId id, CancellationToken ct)
    {
        var entity = await _context.PlantillasProyectoNecesidades.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.PlantillasProyectoNecesidades.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

---

#### MaestraRolProfesionalRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/MaestraRolProfesionalRepository.cs`

- **Implementa:** `IMaestraRolProfesionalRepository`
- **Inyecta:** `CrowdsourcingContext`
- **SaveChanges:** En cada operacion de escritura

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class MaestraRolProfesionalRepository : IMaestraRolProfesionalRepository
{
    private readonly CrowdsourcingContext _context;

    public MaestraRolProfesionalRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<MaestraRolProfesional?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Include(x => x.CategoriaRol)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetAllActivosAsync(CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Include(x => x.CategoriaRol)
            .Where(x => x.Activo)
            .OrderBy(x => x.CategoriaRol.Orden)
                .ThenBy(x => x.Nombre)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MaestraRolProfesional>> GetByCategoriaIdAsync(int categoriaId, CancellationToken ct)
    {
        return await _context.MaestrasRolProfesional
            .AsNoTracking()
            .Where(x => x.CategoriaRolId == categoriaId && x.Activo)
            .OrderBy(x => x.Nombre)
            .ToListAsync(ct);
    }

    public async Task<int> AddAsync(MaestraRolProfesional entity, CancellationToken ct)
    {
        await _context.MaestrasRolProfesional.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(MaestraRolProfesional entity, CancellationToken ct)
    {
        _context.MaestrasRolProfesional.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
```

---

#### MaestraCategoriaRolRepository

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Repositories/MaestraCategoriaRolRepository.cs`

- **Implementa:** `IMaestraCategoriaRolRepository`
- **Inyecta:** `CrowdsourcingContext`
- **SaveChanges:** En cada operacion de escritura

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Context;

namespace WePlayRises.Crowdsourcing.Infra.Repositories;

public class MaestraCategoriaRolRepository : IMaestraCategoriaRolRepository
{
    private readonly CrowdsourcingContext _context;

    public MaestraCategoriaRolRepository(CrowdsourcingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<MaestraCategoriaRol?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.MaestrasCategoriaRol
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyList<MaestraCategoriaRol>> GetAllAsync(CancellationToken ct)
    {
        return await _context.MaestrasCategoriaRol
            .AsNoTracking()
            .OrderBy(x => x.Orden)
            .ToListAsync(ct);
    }

    public async Task<int> AddAsync(MaestraCategoriaRol entity, CancellationToken ct)
    {
        await _context.MaestrasCategoriaRol.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(MaestraCategoriaRol entity, CancellationToken ct)
    {
        _context.MaestrasCategoriaRol.Update(entity);
        await _context.SaveChangesAsync(ct);
    }
}
```

---

### 3.2 Services

#### IPlantillaProyectoService / PlantillaProyectoService

**Interface:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Interfaces/IPlantillaProyectoService.cs`
**Implementation:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/PlantillaProyectoService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | PlantillaProyecto? | Obtener template basico sin necesidades |
| GetByIdWithNecesidadesAsync | PlantillaProyecto? | Obtener template con necesidades, roles y categorias |
| GetAllActivosAsync | IReadOnlyList<PlantillaProyecto> | Listar templates activos |
| CreateAsync | PlantillaProyectoId | Crear template (admin) |
| UpdateAsync | void | Actualizar template (admin) |
| DeleteAsync | void | Eliminar logico (admin) |

**IMPORTANTE:**
- Service retorna ENTIDADES, no DTOs
- Logica de negocio va en Handlers, NO aqui
- Service inyecta `IRequestCacheService` para evitar queries duplicados
- Service inyecta `ILogger<PlantillaProyectoService>` para logging

**Codigo Interface:**
```csharp
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Interfaces;

public interface IPlantillaProyectoService
{
    Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct);
    Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct);
    Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct);
    Task<PlantillaProyectoId> CreateAsync(PlantillaProyecto entity, CancellationToken ct);
    Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct);
    Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct);
}
```

**Codigo Implementation:**
```csharp
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.Crowdsourcing.Infra.Interfaces;

namespace WePlayRises.Crowdsourcing.Infra.Services;

/// <summary>
/// Service para persistencia de PlantillaProyecto.
/// Retorna entidades (NO DTOs). Logica de negocio en Handlers.
/// </summary>
public class PlantillaProyectoService : IPlantillaProyectoService
{
    private readonly IPlantillaProyectoRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly ILogger<PlantillaProyectoService> _logger;

    public PlantillaProyectoService(
        IPlantillaProyectoRepository repository,
        IRequestCacheService requestCache,
        ILogger<PlantillaProyectoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PlantillaProyecto?> GetByIdAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        var cacheKey = $"plantilla-proyecto:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdAsync(id, ct));
    }

    public async Task<PlantillaProyecto?> GetByIdWithNecesidadesAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        var cacheKey = $"plantilla-proyecto:necesidades:{id.Value}";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetByIdWithNecesidadesAsync(id, ct));
    }

    public async Task<IReadOnlyList<PlantillaProyecto>> GetAllActivosAsync(CancellationToken ct)
    {
        // Cache largo: templates cambian poco
        var cacheKey = "plantilla-proyecto:activos";

        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.GetAllActivosAsync(ct));
    }

    public async Task<PlantillaProyectoId> CreateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        entity.FechaCreacion = DateTime.UtcNow;
        var id = await _repository.AddAsync(entity, ct);

        _logger.LogInformation("PlantillaProyecto {PlantillaId} created: {Nombre}",
            id.Value, entity.Nombre);

        return id;
    }

    public async Task UpdateAsync(PlantillaProyecto entity, CancellationToken ct)
    {
        await _repository.UpdateAsync(entity, ct);

        _logger.LogInformation("PlantillaProyecto {PlantillaId} updated", entity.Id.Value);
    }

    public async Task DeleteAsync(PlantillaProyectoId id, CancellationToken ct)
    {
        await _repository.DeleteAsync(id, ct);

        _logger.LogInformation("PlantillaProyecto {PlantillaId} soft deleted", id.Value);
    }
}
```

---

#### IPlantillaProyectoNecesidadService / PlantillaProyectoNecesidadService

**Interface:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Interfaces/IPlantillaProyectoNecesidadService.cs`
**Implementation:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/PlantillaProyectoNecesidadService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | PlantillaProyectoNecesidad? | Obtener necesidad por ID |
| GetByPlantillaIdAsync | IReadOnlyList<PlantillaProyectoNecesidad> | Listar necesidades de template |
| GetByIdsAsync | IReadOnlyList<PlantillaProyectoNecesidad> | Obtener multiples necesidades (validacion) |
| CreateAsync | PlantillaProyectoNecesidadId | Crear necesidad (admin) |
| UpdateAsync | void | Actualizar necesidad (admin) |
| DeleteAsync | void | Eliminar necesidad (admin) |

**IMPORTANTE:** Service inyecta RequestCacheService, Logger y Repository (NO DbContext).

**Codigo (similar a PlantillaProyectoService, omitido por brevedad).**

---

#### IRolProfesionalService / RolProfesionalService

**Interface:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Interfaces/IRolProfesionalService.cs`
**Implementation:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/RolProfesionalService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | MaestraRolProfesional? | Obtener rol por ID con categoria |
| GetAllActivosAsync | IReadOnlyList<MaestraRolProfesional> | Listar roles activos con categoria |
| GetByCategoriaIdAsync | IReadOnlyList<MaestraRolProfesional> | Listar roles de categoria |
| CreateAsync | int | Crear rol (admin) |
| UpdateAsync | void | Actualizar rol (admin) |

**IMPORTANTE:** Service usa RequestCacheService con cache largo (maestras cambian poco).

---

#### ICategoriaRolService / CategoriaRolService

**Interface:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Interfaces/ICategoriaRolService.cs`
**Implementation:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Services/CategoriaRolService.cs`

| Metodo | Retorno | Descripcion |
|--------|---------|-------------|
| GetByIdAsync | MaestraCategoriaRol? | Obtener categoria por ID |
| GetAllAsync | IReadOnlyList<MaestraCategoriaRol> | Listar todas las categorias ordenadas |
| CreateAsync | int | Crear categoria (admin) |
| UpdateAsync | void | Actualizar categoria (admin) |

**IMPORTANTE:** Service usa RequestCacheService con cache largo (maestras cambian poco).

---

### 3.3 Entity Configurations (EF Core)

#### PlantillaProyectoConfiguration

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/PlantillaProyectoConfiguration.cs`

| Configuracion | Detalle |
|---------------|---------|
| Tabla | "PlantillasProyecto" |
| PK | Id (Guid), ValueGeneratedOnAdd |
| Nombre | MaxLength(200), IsRequired |
| Descripcion | MaxLength(1000), Optional |
| Icono | MaxLength(50), Optional |
| Orden | IsRequired |
| Activo | IsRequired, Default(true) |
| FechaCreacion | IsRequired |

**Relaciones:**
- `HasMany(x => x.Necesidades).WithOne(n => n.PlantillaProyecto).HasForeignKey(n => n.PlantillaProyectoId)`

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class PlantillaProyectoConfiguration : IEntityTypeConfiguration<PlantillaProyecto>
{
    public void Configure(EntityTypeBuilder<PlantillaProyecto> builder)
    {
        builder.ToTable("PlantillasProyecto");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PlantillaProyectoId(value))
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);

        builder.Property(x => x.Icono)
            .HasMaxLength(50);

        builder.Property(x => x.Orden)
            .IsRequired();

        builder.Property(x => x.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        // Relaciones
        builder.HasMany(x => x.Necesidades)
            .WithOne(n => n.PlantillaProyecto)
            .HasForeignKey(n => n.PlantillaProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(x => x.Activo);
        builder.HasIndex(x => x.Orden);
    }
}
```

---

#### PlantillaProyectoNecesidadConfiguration

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/PlantillaProyectoNecesidadConfiguration.cs`

| Configuracion | Detalle |
|---------------|---------|
| Tabla | "PlantillasProyectoNecesidades" |
| PK | Id (Guid), ValueGeneratedOnAdd |
| PlantillaProyectoId | FK a PlantillasProyecto, IsRequired |
| Fase | MaxLength(100), IsRequired |
| Titulo | MaxLength(200), IsRequired |
| Descripcion | MaxLength(1000), Optional |
| RolProfesionalId | FK a MaestrasRolProfesional, IsRequired |
| PrecioMinOrientativo | Precision(18,2), Optional |
| PrecioMaxOrientativo | Precision(18,2), Optional |
| MonedaId | FK a Core.MaestrasMoneda, IsRequired |
| Prioridad | MaxLength(20), IsRequired, Default("Media") |
| Orden | IsRequired |

**Relaciones:**
- `HasOne(x => x.PlantillaProyecto).WithMany(p => p.Necesidades).HasForeignKey(x => x.PlantillaProyectoId)`
- `HasOne(x => x.RolProfesional).WithMany(r => r.PlantillasNecesidades).HasForeignKey(x => x.RolProfesionalId)`

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class PlantillaProyectoNecesidadConfiguration : IEntityTypeConfiguration<PlantillaProyectoNecesidad>
{
    public void Configure(EntityTypeBuilder<PlantillaProyectoNecesidad> builder)
    {
        builder.ToTable("PlantillasProyectoNecesidades");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PlantillaProyectoNecesidadId(value))
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PlantillaProyectoId)
            .HasConversion(
                id => id.Value,
                value => new PlantillaProyectoId(value))
            .IsRequired();

        builder.Property(x => x.Fase)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Titulo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(1000);

        builder.Property(x => x.RolProfesionalId)
            .IsRequired();

        builder.Property(x => x.PrecioMinOrientativo)
            .HasPrecision(18, 2);

        builder.Property(x => x.PrecioMaxOrientativo)
            .HasPrecision(18, 2);

        builder.Property(x => x.MonedaId)
            .IsRequired()
            .HasDefaultValue(1); // EUR

        builder.Property(x => x.Prioridad)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Media");

        builder.Property(x => x.Orden)
            .IsRequired();

        builder.Property(x => x.FechaCreacion)
            .IsRequired();

        // Relaciones
        builder.HasOne(x => x.PlantillaProyecto)
            .WithMany(p => p.Necesidades)
            .HasForeignKey(x => x.PlantillaProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RolProfesional)
            .WithMany(r => r.PlantillasNecesidades)
            .HasForeignKey(x => x.RolProfesionalId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(x => x.PlantillaProyectoId);
        builder.HasIndex(x => x.RolProfesionalId);
        builder.HasIndex(x => x.Prioridad);
    }
}
```

---

#### MaestraRolProfesionalConfiguration

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/MaestraRolProfesionalConfiguration.cs`

| Configuracion | Detalle |
|---------------|---------|
| Tabla | "MaestrasRolProfesional" |
| PK | Id (int), Identity |
| Nombre | MaxLength(100), IsRequired |
| Descripcion | MaxLength(500), Optional |
| CategoriaRolId | FK a MaestrasCategoriaRol, IsRequired |
| ModalidadCobro | MaxLength(100), Optional |
| Activo | IsRequired, Default(true) |

**Relaciones:**
- `HasOne(x => x.CategoriaRol).WithMany(c => c.Roles).HasForeignKey(x => x.CategoriaRolId)`

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class MaestraRolProfesionalConfiguration : IEntityTypeConfiguration<MaestraRolProfesional>
{
    public void Configure(EntityTypeBuilder<MaestraRolProfesional> builder)
    {
        builder.ToTable("MaestrasRolProfesional");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd(); // Identity

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Descripcion)
            .HasMaxLength(500);

        builder.Property(x => x.CategoriaRolId)
            .IsRequired();

        builder.Property(x => x.ModalidadCobro)
            .HasMaxLength(100);

        builder.Property(x => x.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(x => x.CategoriaRol)
            .WithMany(c => c.Roles)
            .HasForeignKey(x => x.CategoriaRolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(x => x.CategoriaRolId);
        builder.HasIndex(x => x.Activo);
    }
}
```

---

#### MaestraCategoriaRolConfiguration

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Configurations/MaestraCategoriaRolConfiguration.cs`

| Configuracion | Detalle |
|---------------|---------|
| Tabla | "MaestrasCategoriaRol" |
| PK | Id (int), Identity |
| Nombre | MaxLength(100), IsRequired |
| Icono | MaxLength(50), Optional |
| Orden | IsRequired |

**Codigo:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Configurations;

public class MaestraCategoriaRolConfiguration : IEntityTypeConfiguration<MaestraCategoriaRol>
{
    public void Configure(EntityTypeBuilder<MaestraCategoriaRol> builder)
    {
        builder.ToTable("MaestrasCategoriaRol");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd(); // Identity

        builder.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Icono)
            .HasMaxLength(50);

        builder.Property(x => x.Orden)
            .IsRequired();

        // Indices
        builder.HasIndex(x => x.Orden);
    }
}
```

---

### 3.4 DbContext Integration

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Context/CrowdsourcingContext.cs`

**NOTA:** El proyecto ya tiene un `CrowdsourcingContext.cs` creado. Modificar para agregar DbSets nuevos.

**Modificaciones:**
```csharp
// Agregar DbSets
public DbSet<PlantillaProyecto> PlantillasProyecto => Set<PlantillaProyecto>();
public DbSet<PlantillaProyectoNecesidad> PlantillasProyectoNecesidades => Set<PlantillaProyectoNecesidad>();
public DbSet<MaestraRolProfesional> MaestrasRolProfesional => Set<MaestraRolProfesional>();
public DbSet<MaestraCategoriaRol> MaestrasCategoriaRol => Set<MaestraCategoriaRol>();

// En OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Aplicar configuraciones
    modelBuilder.ApplyConfiguration(new PlantillaProyectoConfiguration());
    modelBuilder.ApplyConfiguration(new PlantillaProyectoNecesidadConfiguration());
    modelBuilder.ApplyConfiguration(new MaestraRolProfesionalConfiguration());
    modelBuilder.ApplyConfiguration(new MaestraCategoriaRolConfiguration());

    // ... otras configuraciones existentes
}
```

---

### 3.5 Seed Data Strategy

**Objetivo:** Pre-cargar 6 templates con ~55 necesidades, ~35 roles profesionales y 6 categorias de roles.

**Estrategia:**

1. **Crear clase SeedData separada:**
   - **Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/Data/Seeds/CrowdsourcingTemplatesSeedData.cs`
   - Metodo estatico: `public static void Seed(CrowdsourcingContext context)`

2. **Llamar desde Program.cs o DbContext:**
   - Opcion A: Llamar en `Program.cs` despues de `DbContext.Database.Migrate()`
   - Opcion B: Llamar desde `OnModelCreating` con `modelBuilder.Entity<X>().HasData(...)`

3. **Datos seed (del feature-spec.md):**
   - 6 categorias de roles (Produccion Musical, Audiovisual, Diseno y Branding, Marketing, Gestion Legal, Produccion Eventos)
   - ~35 roles profesionales distribuidos en categorias
   - 6 templates:
     1. Grabar un Album/EP (11 necesidades)
     2. Produccion de Videoclip (11 necesidades)
     3. Organizar Gira/Tour (11 necesidades)
     4. Campana Marketing (9 necesidades)
     5. Lanzamiento Single (8 necesidades)
     6. Presencia Online (5 necesidades)

**Archivo Seed (estructura):**
```csharp
using Microsoft.EntityFrameworkCore;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Infra.Data.Seeds;

/// <summary>
/// Seed data para templates de proyectos musicales.
/// Total: 6 templates, ~55 necesidades, ~35 roles, 6 categorias.
/// </summary>
public static class CrowdsourcingTemplatesSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // ========== PASO 1: Seed Categorias de Rol (6) ==========
        var categorias = new[]
        {
            new MaestraCategoriaRol { Id = 1, Nombre = "Produccion Musical", Icono = "music", Orden = 1 },
            new MaestraCategoriaRol { Id = 2, Nombre = "Ingenieria de Audio", Icono = "sliders", Orden = 2 },
            new MaestraCategoriaRol { Id = 3, Nombre = "Diseno y Creatividad", Icono = "palette", Orden = 3 },
            new MaestraCategoriaRol { Id = 4, Nombre = "Marketing y Promocion", Icono = "megaphone", Orden = 4 },
            new MaestraCategoriaRol { Id = 5, Nombre = "Gestion y Legal", Icono = "briefcase", Orden = 5 },
            new MaestraCategoriaRol { Id = 6, Nombre = "Produccion de Eventos / Live", Icono = "mic", Orden = 6 }
        };
        modelBuilder.Entity<MaestraCategoriaRol>().HasData(categorias);

        // ========== PASO 2: Seed Roles Profesionales (~35) ==========
        var roles = new[]
        {
            // Categoria 1: Produccion Musical
            new MaestraRolProfesional { Id = 1, Nombre = "Productor musical", Descripcion = "Dirige la vision sonora del proyecto completo...", CategoriaRolId = 1, ModalidadCobro = "Por proyecto o por cancion", Activo = true },
            new MaestraRolProfesional { Id = 2, Nombre = "Arreglista / Compositor", Descripcion = "Crea arreglos instrumentales y vocales...", CategoriaRolId = 1, ModalidadCobro = "Por cancion", Activo = true },
            // ... (agregar ~10 roles mas en Categoria 1)

            // Categoria 2: Ingenieria de Audio
            new MaestraRolProfesional { Id = 10, Nombre = "Ingeniero de grabacion", Descripcion = "Captura audio de alta calidad...", CategoriaRolId = 2, ModalidadCobro = "Por dia", Activo = true },
            // ... (agregar ~8 roles mas en Categoria 2)

            // Categoria 3: Diseno y Creatividad
            // ... (agregar ~8 roles)

            // Categoria 4: Marketing y Promocion
            // ... (agregar ~5 roles)

            // Categoria 5: Gestion y Legal
            // ... (agregar ~3 roles)

            // Categoria 6: Produccion de Eventos
            // ... (agregar ~3 roles)
        };
        modelBuilder.Entity<MaestraRolProfesional>().HasData(roles);

        // ========== PASO 3: Seed Templates (6) ==========
        var template1Id = new PlantillaProyectoId(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        var template2Id = new PlantillaProyectoId(Guid.Parse("22222222-2222-2222-2222-222222222222"));
        // ... (5 templates mas con GUIDs fijos)

        var templates = new[]
        {
            new PlantillaProyecto { Id = template1Id, Nombre = "Grabar un Album / EP", Descripcion = "Todas las fases para grabar tu primer disco...", Icono = "music", Orden = 1, Activo = true, FechaCreacion = DateTime.UtcNow },
            new PlantillaProyecto { Id = template2Id, Nombre = "Produccion de Videoclip", Descripcion = "Todo lo necesario para producir un videoclip profesional...", Icono = "video", Orden = 2, Activo = true, FechaCreacion = DateTime.UtcNow },
            // ... (4 templates mas)
        };
        modelBuilder.Entity<PlantillaProyecto>().HasData(templates);

        // ========== PASO 4: Seed Necesidades (~55 total) ==========
        // Template 1: Grabar un Album (11 necesidades)
        var necesidadesTemplate1 = new[]
        {
            new PlantillaProyectoNecesidad
            {
                Id = new PlantillaProyectoNecesidadId(Guid.Parse("11111111-1111-1111-1111-111111111101")),
                PlantillaProyectoId = template1Id,
                Fase = "Pre-produccion",
                Titulo = "Composicion y arreglos musicales",
                Descripcion = "Crear arreglos instrumentales y vocales a partir de una composicion basica...",
                RolProfesionalId = 2, // Arreglista/Compositor
                PrecioMinOrientativo = 200m,
                PrecioMaxOrientativo = 1500m,
                MonedaId = 1, // EUR
                Prioridad = "Alta",
                Orden = 1,
                FechaCreacion = DateTime.UtcNow
            },
            // ... (10 necesidades mas para template 1)
        };
        modelBuilder.Entity<PlantillaProyectoNecesidad>().HasData(necesidadesTemplate1);

        // Template 2: Videoclip (11 necesidades)
        // ... (seed necesidades template 2)

        // Templates 3-6 (8-11 necesidades cada uno)
        // ... (seed resto de necesidades)
    }
}
```

**IMPORTANTE:** Los datos seed completos estan en `feature-spec.md` (Templates Predefinidos, secciones 319-423). Copiar todos los valores exactos al archivo seed.

---

### 3.6 Dependency Injection

**Archivo:** `Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra/DependencyInjection.cs`

**Modificar metodo `AddCrowdsourcingServices` para registrar:**

```csharp
using Microsoft.Extensions.DependencyInjection;
using WePlayRises.Crowdsourcing.Domain.Interfaces;
using WePlayRises.Crowdsourcing.Infra.Interfaces;
using WePlayRises.Crowdsourcing.Infra.Repositories;
using WePlayRises.Crowdsourcing.Infra.Services;

namespace WePlayRises.Crowdsourcing.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddCrowdsourcingServices(this IServiceCollection services)
    {
        // ========== REPOSITORIES ==========
        services.AddScoped<IPlantillaProyectoRepository, PlantillaProyectoRepository>();
        services.AddScoped<IPlantillaProyectoNecesidadRepository, PlantillaProyectoNecesidadRepository>();
        services.AddScoped<IMaestraRolProfesionalRepository, MaestraRolProfesionalRepository>();
        services.AddScoped<IMaestraCategoriaRolRepository, MaestraCategoriaRolRepository>();

        // ========== SERVICES ==========
        services.AddScoped<IPlantillaProyectoService, PlantillaProyectoService>();
        services.AddScoped<IPlantillaProyectoNecesidadService, PlantillaProyectoNecesidadService>();
        services.AddScoped<IRolProfesionalService, RolProfesionalService>();
        services.AddScoped<ICategoriaRolService, CategoriaRolService>();

        // ... otras dependencias existentes

        return services;
    }
}
```

---

## 4. Migraciones EF Core

### 4.1 Crear Migracion

**Comando:**
```bash
cd src/api/WebApi
dotnet ef migrations add AddCrowdsourcingTemplates -c CrowdsourcingContext --project ../Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra
```

**IMPORTANTE:** Usar el nombre correcto del DbContext (`CrowdsourcingContext`), no el generico.

### 4.2 Aplicar Migracion

**Comando:**
```bash
dotnet ef database update -c CrowdsourcingContext --project ../Modules/Crowdsourcing/WePlayRises.Crowdsourcing.Infra
```

### 4.3 Verificar Seed Data

**Queries SQL para validar:**
```sql
-- Verificar categorias (6 rows)
SELECT * FROM MaestrasCategoriaRol ORDER BY Orden;

-- Verificar roles (~35 rows)
SELECT COUNT(*) FROM MaestrasRolProfesional;

-- Verificar templates (6 rows)
SELECT * FROM PlantillasProyecto WHERE Activo = 1 ORDER BY Orden;

-- Verificar necesidades (~55 rows)
SELECT COUNT(*) FROM PlantillasProyectoNecesidades;

-- Verificar necesidades de Template 1 (11 rows)
SELECT * FROM PlantillasProyectoNecesidades
WHERE PlantillaProyectoId = '11111111-1111-1111-1111-111111111111'
ORDER BY Orden;
```

---

## 5. Archivos a Crear

```
Modules/Crowdsourcing/
├── WePlayRises.Crowdsourcing.Domain/
│   ├── Constants/
│   │   └── ServiceResponseMessageType.cs               (NUEVO)
│   ├── Model/
│   │   ├── PlantillaProyecto.cs                        (NUEVO)
│   │   ├── PlantillaProyectoNecesidad.cs               (NUEVO)
│   │   ├── MaestraRolProfesional.cs                    (NUEVO)
│   │   ├── MaestraCategoriaRol.cs                      (NUEVO)
│   │   └── MaestraTipoEmpresa.cs                       (NUEVO - opcional, no MVP)
│   └── Interfaces/
│       ├── IPlantillaProyectoRepository.cs             (NUEVO)
│       ├── IPlantillaProyectoNecesidadRepository.cs    (NUEVO)
│       ├── IMaestraRolProfesionalRepository.cs         (NUEVO)
│       └── IMaestraCategoriaRolRepository.cs           (NUEVO)
│
└── WePlayRises.Crowdsourcing.Infra/
    ├── Repositories/
    │   ├── PlantillaProyectoRepository.cs              (NUEVO)
    │   ├── PlantillaProyectoNecesidadRepository.cs     (NUEVO)
    │   ├── MaestraRolProfesionalRepository.cs          (NUEVO)
    │   └── MaestraCategoriaRolRepository.cs            (NUEVO)
    ├── Services/
    │   ├── PlantillaProyectoService.cs                 (NUEVO)
    │   ├── PlantillaProyectoNecesidadService.cs        (NUEVO)
    │   ├── RolProfesionalService.cs                    (NUEVO)
    │   └── CategoriaRolService.cs                      (NUEVO)
    ├── Interfaces/
    │   ├── IPlantillaProyectoService.cs                (NUEVO)
    │   ├── IPlantillaProyectoNecesidadService.cs       (NUEVO)
    │   ├── IRolProfesionalService.cs                   (NUEVO)
    │   └── ICategoriaRolService.cs                     (NUEVO)
    ├── Data/Configurations/
    │   ├── PlantillaProyectoConfiguration.cs           (NUEVO)
    │   ├── PlantillaProyectoNecesidadConfiguration.cs  (NUEVO)
    │   ├── MaestraRolProfesionalConfiguration.cs       (NUEVO)
    │   └── MaestraCategoriaRolConfiguration.cs         (NUEVO)
    ├── Data/Seeds/
    │   └── CrowdsourcingTemplatesSeedData.cs           (NUEVO)
    ├── Context/
    │   └── CrowdsourcingContext.cs                     (MODIFICAR - agregar DbSets)
    └── DependencyInjection.cs                          (MODIFICAR - registrar DI)
```

---

## 6. Strongly Typed IDs Necesarios

**IMPORTANTE:** Agregar en `src/api/BuildingBlocks/WePlayRises.BuildingBlocks.EntityFramework/StronglyTypedIds/`

### Nuevos StronglyTypedIds

```csharp
// PlantillaProyectoId.cs
public readonly record struct PlantillaProyectoId(Guid Value);

// PlantillaProyectoNecesidadId.cs
public readonly record struct PlantillaProyectoNecesidadId(Guid Value);
```

**NOTA:** Los IDs de maestras (int) NO necesitan strongly typed IDs (usar int directo).

---

## 7. Checklist de Implementacion

### Domain Layer
- [ ] `ServiceResponseMessageType.cs` creado con ErrorCodes 2006-2008, 3002, 1009
- [ ] Entidad `PlantillaProyecto` creada como POCO (sin metodos de negocio)
- [ ] Entidad `PlantillaProyectoNecesidad` creada con Prioridad como string ("Alta", "Media", "Baja")
- [ ] Entidad `MaestraRolProfesional` creada con PK int
- [ ] Entidad `MaestraCategoriaRol` creada con PK int
- [ ] Repository interfaces creadas en Domain/Interfaces
- [ ] Strongly Typed IDs agregados a BuildingBlocks

### Infrastructure Layer
- [ ] Repository implementations con EF Core (AsNoTracking para reads)
- [ ] Services inyectan `IUnitOfWork<CrowdsourcingContext>` (si aplica)
- [ ] Services inyectan `IRequestCacheService` para cache
- [ ] Services inyectan `ILogger<TService>` para logging
- [ ] Services usan `?? throw new ArgumentNullException` en constructores
- [ ] Entity configurations usan Fluent API (NO Data Annotations)
- [ ] CrowdsourcingContext modificado con DbSets nuevos
- [ ] Seed data creado con 6 templates, ~55 necesidades, ~35 roles, 6 categorias
- [ ] DependencyInjection.cs modificado con registros de DI
- [ ] Migracion EF Core creada y aplicada
- [ ] Seed data verificado en BD

### Validacion General
- [ ] Entidades son POCOs (sin metodos de negocio)
- [ ] Services retornan entidades, NO DTOs
- [ ] Handlers NO inyectan DbContext (usaran Services)
- [ ] Repository hace SaveChanges (no Handler)
- [ ] Fluent API para todas las configuraciones (no Data Annotations)
- [ ] Prioridad usa strings, NO enums
- [ ] Moneda usa int FK (1=EUR, como en resto del proyecto)
- [ ] Navegacion properties marcadas como `virtual` para lazy loading
- [ ] Indices creados para FKs y campos de filtrado (Activo, Orden, Prioridad)

---

## 8. Siguiente Paso

**Este plan es BLOCKING para Application layer (CQRS).**

Una vez completada la implementacion de Domain + Infrastructure:

1. **Verificar seed data:** Ejecutar queries SQL para validar 6 templates, ~55 necesidades, ~35 roles
2. **Crear Application layer plan:** Ejecutar `cqrs-planning-architect` para generar Commands/Queries/Handlers
3. **CRUD templates (opcional):** Implementar Commands/Queries para gestionar templates desde Admin (post-MVP)
4. **Command principal:** `GenerarNecesidadesDesdeTemplateCommand` - Crea multiples `NecesidadCrowdsourcing` en bulk

---

## 9. Notas Criticas

### Cache Strategy
- **Templates:** Cache largo (5-10 min) - datos maestros cambian poco
- **Roles/Categorias:** Cache largo (10 min) - maestras cambian rara vez
- **Necesidades generadas:** NO cachear (datos transaccionales)

### Performance
- **GetByIdWithNecesidadesAsync:** Usar Include para evitar N+1 queries
- **GetAllActivosAsync:** AsNoTracking para lecturas
- **Bulk insert seed:** Usar `modelBuilder.HasData()` en lugar de loops con AddAsync

### Seguridad
- **Validacion ownership:** Handler de `GenerarNecesidadesCommand` DEBE validar que `ProyectoArtisticoId` pertenece al artista autenticado
- **Rate limiting:** Max 10 generaciones por hora por usuario (implementar en API Controller)
- **Sanitizacion:** Inputs de presupuesto validados (evitar inyeccion SQL, XSS)

### Mantenibilidad
- **Seed versionado:** Datos seed en migraciones, NO hardcoded en codigo
- **Templates editables:** Preparar CRUD para Admin (no MVP, pero arquitectura lista)
- **Audit log:** Logear generacion de necesidades (quien, cuando, template usado)

---

**Plan creado:** 2026-02-15
**Autor:** Claude Code (Arquitecto Hexagonal .NET)
**Estado:** READY FOR IMPLEMENTATION
