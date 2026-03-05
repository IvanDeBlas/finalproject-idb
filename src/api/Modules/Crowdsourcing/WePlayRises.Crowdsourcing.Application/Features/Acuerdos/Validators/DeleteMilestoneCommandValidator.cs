using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class DeleteMilestoneCommandValidator : AbstractValidator<DeleteMilestoneCommand>
{
    public DeleteMilestoneCommandValidator()
    {
        RuleFor(x => x.MilestoneId)
            .NotEmpty()
            .WithMessage("El ID del milestone es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El ID del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
