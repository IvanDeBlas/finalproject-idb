using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for FanProfile entity
/// </summary>
public readonly record struct FanProfileId(Guid Value) : IComparable<FanProfileId>, IComparable
{
    public static FanProfileId Empty => new(Guid.Empty);
    public static FanProfileId CreateNew() => new(Guid.NewGuid());

    public static FanProfileId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("FanProfileId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(FanProfileId id) => id.Value;
    public static explicit operator FanProfileId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static FanProfileId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new FanProfileId(value);

    public int CompareTo(FanProfileId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is FanProfileId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(FanProfileId)}");
    }

    public class EfCoreConverter : ValueConverter<FanProfileId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new FanProfileId(value))
        { }
    }
}
