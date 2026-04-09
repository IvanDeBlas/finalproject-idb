using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class RechazarEntregableCommandValidator : AbstractValidator<RechazarEntregableCommand>
{
    public RechazarEntregableCommandValidator()
    {
        RuleFor(x => x.Comentario)
            .NotEmpty()
            .WithMessage("El comentario es obligatorio al rechazar un entregable")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(10)
            .WithMessage("Minimo 10 caracteres explicando que debe corregirse")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
