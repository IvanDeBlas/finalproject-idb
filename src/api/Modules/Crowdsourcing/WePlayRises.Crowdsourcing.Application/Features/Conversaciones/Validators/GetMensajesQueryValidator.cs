using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class GetMensajesQueryValidator : AbstractValidator<GetMensajesQuery>
{
    public GetMensajesQueryValidator()
    {
        RuleFor(x => x.ConversacionId)
            .NotEmpty()
            .WithMessage("El ID de la conversacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

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
    }
}
