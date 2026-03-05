using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Validators;

public class DarDeBajaInscripcionCommandValidator : AbstractValidator<DarDeBajaInscripcionCommand>
{
    public DarDeBajaInscripcionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.InscripcionId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El InscripcionId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
