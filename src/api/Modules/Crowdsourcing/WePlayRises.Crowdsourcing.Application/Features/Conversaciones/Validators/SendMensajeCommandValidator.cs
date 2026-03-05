using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class SendMensajeCommandValidator : AbstractValidator<SendMensajeCommand>
{
    public SendMensajeCommandValidator()
    {
        RuleFor(x => x.ConversacionId)
            .NotEmpty()
            .WithMessage("El ID de la conversacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserIdRemitente)
            .NotEmpty()
            .WithMessage("Usuario no identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Contenido)
            .NotEmpty()
            .WithMessage("El contenido del mensaje es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Contenido)
            .MaximumLength(5000)
            .WithMessage("El mensaje no puede superar los 5000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.UrlAdjunto)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.UrlAdjunto))
            .WithMessage("La URL adjunta no es valida. Debe ser una URL completa (ej: https://...)")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl);

        RuleFor(x => x.UrlAdjunto)
            .MaximumLength(2048)
            .When(x => !string.IsNullOrEmpty(x.UrlAdjunto))
            .WithMessage("La URL no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttps || result.Scheme == Uri.UriSchemeHttp);
    }
}
