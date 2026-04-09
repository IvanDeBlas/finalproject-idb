using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;

/// <summary>
/// Strongly typed ID for PerfilProfesional entity
/// </summary>
public readonly record struct PerfilProfesionalId(Guid Value) : IComparable<PerfilProfesionalId>, IComparable
{
    public static PerfilProfesionalId Empty => new(Guid.Empty);
    public static PerfilProfesionalId CreateNew() => new(Guid.NewGuid());

    public static PerfilProfesionalId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PerfilProfesionalId value cannot be empty", nameof(value));
        return new(value);
    }

    public static implicit operator Guid(PerfilProfesionalId id) => id.Value;
    public static explicit operator PerfilProfesionalId(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static PerfilProfesionalId? TryParse(Guid value) =>
        value == Guid.Empty ? null : new PerfilProfesionalId(value);

    public int CompareTo(PerfilProfesionalId other) => Value.CompareTo(other.Value);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is PerfilProfesionalId other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(PerfilProfesionalId)}");
    }

    public class EfCoreConverter : ValueConverter<PerfilProfesionalId, Guid>
    {
        public EfCoreConverter() : base(
            id => id.Value,
            value => new PerfilProfesionalId(value))
        { }
    }
}
