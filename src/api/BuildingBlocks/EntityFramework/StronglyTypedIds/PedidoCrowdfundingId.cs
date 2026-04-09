using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for PedidoCrowdfunding entity
/// </summary>
public readonly record struct PedidoCrowdfundingId(Guid Value) : IComparable<PedidoCrowdfundingId>, IComparable
{
    public static PedidoCrowdfundingId Empty => new(Guid.Empty);
    public static PedidoCrowdfundingId CreateNew() => new(Guid.NewGuid());

    public static PedidoCrowdfundingId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PedidoCrowdfundingId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PedidoCrowdfundingId id) => id.Value;
    public static explicit operator PedidoCrowdfundingId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PedidoCrowdfundingId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PedidoCrowdfundingId(value);

    public int CompareTo(PedidoCrowdfundingId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PedidoCrowdfundingId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PedidoCrowdfundingId)}");
    }

    public class EfCoreConverter : ValueConverter<PedidoCrowdfundingId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PedidoCrowdfundingId(value))
        { }
    }
}
