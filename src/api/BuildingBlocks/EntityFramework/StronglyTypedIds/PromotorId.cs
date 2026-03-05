using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for Promotor entity
/// </summary>
public readonly record struct PromotorId(Guid Value) : IComparable<PromotorId>, IComparable
{
    public static PromotorId Empty => new(Guid.Empty);
    public static PromotorId CreateNew() => new(Guid.NewGuid());

    public static PromotorId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PromotorId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PromotorId id) => id.Value;
    public static explicit operator PromotorId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PromotorId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PromotorId(value);

    public int CompareTo(PromotorId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PromotorId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PromotorId)}");
    }

    public class EfCoreConverter : ValueConverter<PromotorId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PromotorId(value))
        { }
    }
}
