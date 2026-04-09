using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class CompletarAcuerdoCommandValidator : AbstractValidator<CompletarAcuerdoCommand>
{
    public CompletarAcuerdoCommandValidator()
    {
        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El ID del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
