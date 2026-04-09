using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for AcuerdoCrowdsourcing entity
/// </summary>
public readonly record struct AcuerdoCrowdsourcingId(Guid Value) : IComparable<AcuerdoCrowdsourcingId>, IComparable
{
    public static AcuerdoCrowdsourcingId Empty => new(Guid.Empty);
    public static AcuerdoCrowdsourcingId CreateNew() => new(Guid.NewGuid());

    public static AcuerdoCrowdsourcingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AcuerdoCrowdsourcingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(AcuerdoCrowdsourcingId id) => id.Value;
    public static explicit operator AcuerdoCrowdsourcingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static AcuerdoCrowdsourcingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new AcuerdoCrowdsourcingId(value);

    public int CompareTo(AcuerdoCrowdsourcingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is AcuerdoCrowdsourcingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(AcuerdoCrowdsourcingId)}");
    }

    public class EfCoreConverter : ValueConverter<AcuerdoCrowdsourcingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new AcuerdoCrowdsourcingId(value))
        { }
    }
}
