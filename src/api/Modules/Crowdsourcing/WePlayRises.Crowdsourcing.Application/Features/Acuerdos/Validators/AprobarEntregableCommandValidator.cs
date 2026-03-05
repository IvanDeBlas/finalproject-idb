using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class AprobarEntregableCommandValidator : AbstractValidator<AprobarEntregableCommand>
{
    public AprobarEntregableCommandValidator()
    {
        RuleFor(x => x.Comentario)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Comentario))
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
