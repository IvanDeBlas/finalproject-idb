using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;

public class GetValoracionesByUserQueryValidator : AbstractValidator<GetValoracionesByUserQuery>
{
    public GetValoracionesByUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El numero de pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("El tamano de pagina debe ser entre 1 y 50")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
