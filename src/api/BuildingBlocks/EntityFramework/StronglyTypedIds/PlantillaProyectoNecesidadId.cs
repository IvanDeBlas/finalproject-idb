using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

public readonly record struct PlantillaProyectoNecesidadId(Guid Value) : IComparable<PlantillaProyectoNecesidadId>, IComparable
{
    public static PlantillaProyectoNecesidadId Empty => new(Guid.Empty);
    public static PlantillaProyectoNecesidadId CreateNew() => new(Guid.NewGuid());

    public static PlantillaProyectoNecesidadId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PlantillaProyectoNecesidadId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PlantillaProyectoNecesidadId id) => id.Value;
    public static explicit operator PlantillaProyectoNecesidadId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PlantillaProyectoNecesidadId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PlantillaProyectoNecesidadId(value);

    public int CompareTo(PlantillaProyectoNecesidadId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PlantillaProyectoNecesidadId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PlantillaProyectoNecesidadId)}");
    }

    public class EfCoreConverter : ValueConverter<PlantillaProyectoNecesidadId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PlantillaProyectoNecesidadId(value))
        { }
    }
}
