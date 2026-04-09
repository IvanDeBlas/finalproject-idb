using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class GetConversacionesQueryValidator : AbstractValidator<GetConversacionesQuery>
{
    public GetConversacionesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Usuario no identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El numero de pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El tamano de pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.Contexto)
            .Must(v => v == "todas" || v == "necesidades" || v == "acuerdos")
            .When(x => !string.IsNullOrEmpty(x.Contexto))
            .WithMessage("El valor de contexto no es valido. Use: todas, necesidades, acuerdos")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
