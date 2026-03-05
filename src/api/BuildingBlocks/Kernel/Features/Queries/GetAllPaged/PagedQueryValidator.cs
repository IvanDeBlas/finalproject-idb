using FluentValidation;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetAllPaged
{
    /// <summary>
    /// Base validator for paginated queries with standard pagination validation rules
    /// </summary>
    /// <typeparam name="TQuery">The query type that inherits from PagedQueryBase</typeparam>
    public class PagedQueryValidator<TQuery> : AbstractValidator<TQuery>
        where TQuery : PagedQueryBase<object>
    {
        // Validation error codes (following convention: 1000-1999)
        private const string Validation_GreaterThan = "1002";
        private const string Validation_Range = "1006";

        public PagedQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0")
                .WithErrorCode(Validation_GreaterThan);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100")
                .WithErrorCode(Validation_Range);
        }
    }
}
