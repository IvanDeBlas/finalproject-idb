using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Abstractions.Model
{
    [ExcludeFromCodeCoverage]
    public class MasterTableBase<T>
    {
        [Key]
        [Column("id")]
        public T Id { get; set; }

        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; } = null!;


        public bool Equals(T other)
        {
            return EqualityComparer<T>.Default.Equals(Id, other);
        }
    }
}
