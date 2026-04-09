using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Templates.Validators;

public class GetPlantillaProyectoByIdValidator : AbstractValidator<GetPlantillaProyectoByIdQuery>
{
    public GetPlantillaProyectoByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El ID de la plantilla es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
