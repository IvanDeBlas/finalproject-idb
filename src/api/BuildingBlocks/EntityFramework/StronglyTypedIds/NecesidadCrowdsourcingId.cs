using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for NecesidadCrowdsourcing entity
/// </summary>
public readonly record struct NecesidadCrowdsourcingId(Guid Value) : IComparable<NecesidadCrowdsourcingId>, IComparable
{
    public static NecesidadCrowdsourcingId Empty => new(Guid.Empty);
    public static NecesidadCrowdsourcingId CreateNew() => new(Guid.NewGuid());

    public static NecesidadCrowdsourcingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("NecesidadCrowdsourcingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(NecesidadCrowdsourcingId id) => id.Value;
    public static explicit operator NecesidadCrowdsourcingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static NecesidadCrowdsourcingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new NecesidadCrowdsourcingId(value);

    public int CompareTo(NecesidadCrowdsourcingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is NecesidadCrowdsourcingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(NecesidadCrowdsourcingId)}");
    }

    public class EfCoreConverter : ValueConverter<NecesidadCrowdsourcingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new NecesidadCrowdsourcingId(value))
        { }
    }
}
