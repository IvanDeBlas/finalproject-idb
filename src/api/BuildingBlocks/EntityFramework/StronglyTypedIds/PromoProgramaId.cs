using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for PromoPrograma entity
/// </summary>
public readonly record struct PromoProgramaId(Guid Value) : IComparable<PromoProgramaId>, IComparable
{
    public static PromoProgramaId Empty => new(Guid.Empty);
    public static PromoProgramaId CreateNew() => new(Guid.NewGuid());

    public static PromoProgramaId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PromoProgramaId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PromoProgramaId id) => id.Value;
    public static explicit operator PromoProgramaId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PromoProgramaId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PromoProgramaId(value);

    public int CompareTo(PromoProgramaId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PromoProgramaId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PromoProgramaId)}");
    }

    public class EfCoreConverter : ValueConverter<PromoProgramaId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PromoProgramaId(value))
        { }
    }
}
