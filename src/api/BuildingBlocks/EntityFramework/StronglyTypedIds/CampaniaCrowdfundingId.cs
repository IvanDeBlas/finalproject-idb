using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for CampaniaCrowdfunding entity
/// </summary>
public readonly record struct CampaniaCrowdfundingId(Guid Value) : IComparable<CampaniaCrowdfundingId>, IComparable
{
    public static CampaniaCrowdfundingId Empty => new(Guid.Empty);
    public static CampaniaCrowdfundingId CreateNew() => new(Guid.NewGuid());

    public static CampaniaCrowdfundingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CampaniaCrowdfundingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(CampaniaCrowdfundingId id) => id.Value;
    public static explicit operator CampaniaCrowdfundingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static CampaniaCrowdfundingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new CampaniaCrowdfundingId(value);

    public int CompareTo(CampaniaCrowdfundingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is CampaniaCrowdfundingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(CampaniaCrowdfundingId)}");
    }

    public class EfCoreConverter : ValueConverter<CampaniaCrowdfundingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new CampaniaCrowdfundingId(value))
        { }
    }
}
