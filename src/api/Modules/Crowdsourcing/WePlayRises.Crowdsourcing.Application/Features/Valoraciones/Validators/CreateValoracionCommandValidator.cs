using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Validators;

public class CreateValoracionCommandValidator : AbstractValidator<CreateValoracionCommand>
{
    public CreateValoracionCommandValidator()
    {
        RuleFor(x => x.AcuerdoId)
            .NotEmpty()
            .WithMessage("El identificador del acuerdo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .GreaterThan(0)
            .WithMessage("La puntuacion es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Puntuacion)
            .InclusiveBetween(1, 5)
            .WithMessage("La puntuacion debe ser entre 1 y 5")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.Comentario)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comentario))
            .WithMessage("El comentario no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }
}
