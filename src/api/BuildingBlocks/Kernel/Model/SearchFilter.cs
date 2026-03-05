using System.ComponentModel.DataAnnotations;

namespace WePlayRises.BuildingBlocks.Kernel.Model
{
    public class SearchFilter
    {
        [Required]
        [MinLength(3)]
        public string SearchText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
