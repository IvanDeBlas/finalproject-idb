using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class CreateConversacionCommandValidator : AbstractValidator<CreateConversacionCommand>
{
    public CreateConversacionCommandValidator()
    {
        RuleFor(x => x.UserIdCreador)
            .NotEmpty()
            .WithMessage("Error de autenticacion: usuario no identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserIdDestinatario)
            .NotEmpty()
            .WithMessage("El destinatario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Asunto)
            .NotEmpty()
            .WithMessage("El asunto de la conversacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Asunto)
            .MaximumLength(200)
            .WithMessage("El asunto no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x)
            .Must(x => (x.NecesidadId.HasValue && !x.AcuerdoId.HasValue)
                       || (!x.NecesidadId.HasValue && x.AcuerdoId.HasValue))
            .WithMessage("Debe especificar exactamente un contexto: una necesidad o un acuerdo, pero no ambos ni ninguno")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
