using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for CampaniaCrowdfundingReward entity
/// </summary>
public readonly record struct CampaniaCrowdfundingRewardId(Guid Value) : IComparable<CampaniaCrowdfundingRewardId>, IComparable
{
    public static CampaniaCrowdfundingRewardId Empty => new(Guid.Empty);
    public static CampaniaCrowdfundingRewardId CreateNew() => new(Guid.NewGuid());

    public static CampaniaCrowdfundingRewardId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CampaniaCrowdfundingRewardId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(CampaniaCrowdfundingRewardId id) => id.Value;
    public static explicit operator CampaniaCrowdfundingRewardId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static CampaniaCrowdfundingRewardId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new CampaniaCrowdfundingRewardId(value);

    public int CompareTo(CampaniaCrowdfundingRewardId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is CampaniaCrowdfundingRewardId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(CampaniaCrowdfundingRewardId)}");
    }

    public class EfCoreConverter : ValueConverter<CampaniaCrowdfundingRewardId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new CampaniaCrowdfundingRewardId(value))
        { }
    }
}
