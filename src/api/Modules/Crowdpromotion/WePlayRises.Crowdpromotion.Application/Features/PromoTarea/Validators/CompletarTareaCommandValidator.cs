using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Validators;

public class CompletarTareaCommandValidator : AbstractValidator<CompletarTareaCommand>
{
    public CompletarTareaCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProgramaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El ProgramaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TareaId)
            .Must(id => id != Guid.Empty)
            .WithMessage("El TareaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UrlPruebaCompletado)
            .NotEmpty()
            .WithMessage("La URL de prueba es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UrlPruebaCompletado)
            .MaximumLength(2048)
            .WithMessage("La URL de prueba no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlPruebaCompletado));

        RuleFor(x => x.UrlPruebaCompletado)
            .Must(BeValidUrl)
            .WithMessage("La URL de prueba no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.UrlPruebaCompletado));

        RuleFor(x => x.ComentarioPromotor)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.ComentarioPromotor != null);
    }

    protected static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
