using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class CancelarAcuerdoCommandValidator : AbstractValidator<CancelarAcuerdoCommand>
{
    public CancelarAcuerdoCommandValidator()
    {
        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El ID del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Motivo)
            .NotEmpty()
            .WithMessage("El motivo de cancelacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(20)
            .WithMessage("El motivo debe tener al menos 20 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(1000)
            .WithMessage("El motivo no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
