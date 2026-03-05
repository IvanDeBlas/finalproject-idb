using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for PropuestaCrowdsourcing entity
/// </summary>
public readonly record struct PropuestaCrowdsourcingId(Guid Value) : IComparable<PropuestaCrowdsourcingId>, IComparable
{
    public static PropuestaCrowdsourcingId Empty => new(Guid.Empty);
    public static PropuestaCrowdsourcingId CreateNew() => new(Guid.NewGuid());

    public static PropuestaCrowdsourcingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PropuestaCrowdsourcingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PropuestaCrowdsourcingId id) => id.Value;
    public static explicit operator PropuestaCrowdsourcingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PropuestaCrowdsourcingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PropuestaCrowdsourcingId(value);

    public int CompareTo(PropuestaCrowdsourcingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PropuestaCrowdsourcingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PropuestaCrowdsourcingId)}");
    }

    public class EfCoreConverter : ValueConverter<PropuestaCrowdsourcingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PropuestaCrowdsourcingId(value))
        { }
    }
}
