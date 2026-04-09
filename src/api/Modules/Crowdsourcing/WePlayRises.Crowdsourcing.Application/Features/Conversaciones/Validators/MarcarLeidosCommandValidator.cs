using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class MarcarLeidosCommandValidator : AbstractValidator<MarcarLeidosCommand>
{
    public MarcarLeidosCommandValidator()
    {
        RuleFor(x => x.ConversacionId)
            .NotEmpty()
            .WithMessage("El ID de la conversacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Usuario no identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
