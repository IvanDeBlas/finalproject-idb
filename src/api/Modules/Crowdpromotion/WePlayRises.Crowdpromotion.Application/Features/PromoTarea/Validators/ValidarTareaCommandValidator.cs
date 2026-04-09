using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators;

public class ValidarTareaCommandValidator : AbstractValidator<ValidarTareaCommand>
{
    public ValidarTareaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TareaPromotorId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El TareaPromotorId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ComentarioValidacion)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.ComentarioValidacion != null);
    }
}
