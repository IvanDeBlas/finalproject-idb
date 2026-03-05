using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for AportacionCrowdfunding entity
/// </summary>
public readonly record struct AportacionCrowdfundingId(Guid Value) : IComparable<AportacionCrowdfundingId>, IComparable
{
    public static AportacionCrowdfundingId Empty => new(Guid.Empty);
    public static AportacionCrowdfundingId CreateNew() => new(Guid.NewGuid());

    public static AportacionCrowdfundingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AportacionCrowdfundingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(AportacionCrowdfundingId id) => id.Value;
    public static explicit operator AportacionCrowdfundingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static AportacionCrowdfundingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new AportacionCrowdfundingId(value);

    public int CompareTo(AportacionCrowdfundingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is AportacionCrowdfundingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(AportacionCrowdfundingId)}");
    }

    public class EfCoreConverter : ValueConverter<AportacionCrowdfundingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new AportacionCrowdfundingId(value))
        { }
    }
}
