using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

public readonly record struct PlantillaProyectoId(Guid Value) : IComparable<PlantillaProyectoId>, IComparable
{
    public static PlantillaProyectoId Empty => new(Guid.Empty);
    public static PlantillaProyectoId CreateNew() => new(Guid.NewGuid());

    public static PlantillaProyectoId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PlantillaProyectoId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PlantillaProyectoId id) => id.Value;
    public static explicit operator PlantillaProyectoId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PlantillaProyectoId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PlantillaProyectoId(value);

    public int CompareTo(PlantillaProyectoId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PlantillaProyectoId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PlantillaProyectoId)}");
    }

    public class EfCoreConverter : ValueConverter<PlantillaProyectoId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PlantillaProyectoId(value))
        { }
    }
}
