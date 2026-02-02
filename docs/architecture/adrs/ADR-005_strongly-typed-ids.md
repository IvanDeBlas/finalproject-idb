# ADR-005: Uso de Strongly Typed IDs

## Estado
Aceptada

## Fecha
2026-01-22

## Contexto

En un sistema con múltiples módulos y entidades interrelacionadas, el uso de tipos primitivos (`Guid`, `int`) como identificadores presenta varios problemas:

1. **Falta de seguridad de tipos**: Es posible pasar accidentalmente un `ArtistaId` donde se espera un `CampaniaId`, ya que ambos son `Guid`.
2. **Código menos expresivo**: Los métodos como `GetById(Guid id)` no indican qué tipo de entidad esperan.
3. **Errores en tiempo de ejecución**: Los errores de tipo se detectan en runtime en lugar de en compilación.

## Decisión

Adoptamos el patrón **Strongly Typed IDs** para todas las entidades principales del sistema.

### Implementación

Cada entidad principal tiene su propio tipo de ID definido como `readonly record struct`:

```csharp
public readonly record struct ArtistaId(Guid Value) : IComparable<ArtistaId>, IComparable
{
    public static ArtistaId Empty => new(Guid.Empty);
    public static ArtistaId CreateNew() => new(Guid.NewGuid());

    public static ArtistaId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ArtistaId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(ArtistaId id) => id.Value;
    public static explicit operator ArtistaId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public class EfCoreConverter : ValueConverter<ArtistaId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new ArtistaId(value))
        { }
    }
}
```

### Ubicación

Todos los Strongly Typed IDs se encuentran en:
```
src/api/BuildingBlocks/EntityFramework/StronglyTypedIds/
```

### IDs Implementados

| Módulo | Strongly Typed ID |
|--------|-------------------|
| UserAccess | `ArtistaId`, `FanProfileId`, `ProyectoArtisticoId`, `PerfilProfesionalId` |
| Crowdfunding | `CampaniaCrowdfundingId`, `CampaniaCrowdfundingRewardId`, `PedidoCrowdfundingId`, `AportacionCrowdfundingId` |
| Crowdsourcing | `NecesidadCrowdsourcingId`, `PropuestaCrowdsourcingId`, `AcuerdoCrowdsourcingId` |
| Crowdpromotion | `PromoProgramaId`, `PromotorId` |

### Configuración en EF Core

En el DbContext se configuran los conversores:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .Properties<ArtistaId>()
        .HaveConversion<ArtistaId.EfCoreConverter>();

    configurationBuilder
        .Properties<CampaniaCrowdfundingId>()
        .HaveConversion<CampaniaCrowdfundingId.EfCoreConverter>();

    // ... demás IDs
}
```

## Consecuencias

### Positivas

1. **Seguridad de tipos en compilación**: No se puede pasar accidentalmente un ID incorrecto.
2. **Código más expresivo**: Los métodos indican claramente qué tipo de ID esperan.
3. **Mejor experiencia de desarrollo**: IntelliSense muestra el tipo correcto.
4. **Facilita refactoring**: Cambios en tipos de ID se detectan en compilación.

### Negativas

1. **Más código boilerplate**: Cada entidad necesita su propio tipo de ID.
2. **Curva de aprendizaje**: Desarrolladores nuevos deben entender el patrón.
3. **Conversiones explícitas**: A veces se necesitan conversiones cuando se trabaja con APIs externas que esperan `Guid`.

### Mitigaciones

- Uso de `readonly record struct` minimiza el boilerplate.
- Conversiones implícitas/explícitas facilitan interoperabilidad.
- Template disponible para crear nuevos IDs rápidamente.

## Alternativas Consideradas

1. **Solo Guid/int primitivos**: Rechazada por falta de seguridad de tipos.
2. **Librería StronglyTypedId (NuGet)**: Considerada, pero preferimos implementación propia para control total.
3. **Value Objects completos**: Excesivo para solo IDs, `record struct` es suficiente.

## Referencias

- [Using Strongly Typed Entity IDs to Avoid Primitive Obsession](https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/)
- [EF Core Value Converters](https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions)
- Implementación de referencia: miUrba ModularMonolith
