using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class CreateEntregableCommandValidator : AbstractValidator<CreateEntregableCommand>
{
    public CreateEntregableCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(3)
            .WithMessage("El titulo debe tener al menos 3 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Descripcion))
            .WithMessage("La descripcion no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.UrlRecurso)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.UrlRecurso))
            .WithMessage("Debe ser una URL valida (ej: Dropbox, Drive, WeTransfer)")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);
    }
}
