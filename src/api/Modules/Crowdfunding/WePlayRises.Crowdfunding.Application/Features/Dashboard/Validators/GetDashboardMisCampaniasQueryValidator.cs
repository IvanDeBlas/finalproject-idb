using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Dashboard.Queries;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Dashboard.Validators;

public class GetDashboardMisCampaniasQueryValidator : AbstractValidator<GetDashboardMisCampaniasQuery>
{
    public GetDashboardMisCampaniasQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId es requerido")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize debe estar entre 1 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.EstadoCampaniaId)
            .InclusiveBetween(1, 5)
            .When(x => x.EstadoCampaniaId.HasValue)
            .WithMessage("EstadoCampaniaId invalido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
