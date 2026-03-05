using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Promotor.Validators;

public class UpdatePromotorCommandValidator : AbstractValidator<UpdatePromotorCommand>
{
    public UpdatePromotorCommandValidator()
    {
        RuleFor(x => x.NombrePublico)
            .NotEmpty()
            .WithMessage("El nombre publico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(3)
            .WithMessage("El nombre debe tener al menos 3 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El nombre publico no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.EmailContacto)
            .EmailAddress()
            .WithMessage("El email de contacto no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail)
            .MaximumLength(200)
            .WithMessage("El email de contacto no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.EmailContacto));

        RuleFor(x => x.UrlSitioWeb)
            .Must(BeValidUrl)
            .WithMessage("La URL del sitio web no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .MaximumLength(300)
            .WithMessage("La URL del sitio web no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlSitioWeb));

        RuleFor(x => x.UrlInstagram)
            .Must(BeValidUrl)
            .WithMessage("La URL de Instagram no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .MaximumLength(300)
            .WithMessage("La URL de Instagram no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlInstagram));

        RuleFor(x => x.UrlTikTok)
            .Must(BeValidUrl)
            .WithMessage("La URL de TikTok no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .MaximumLength(300)
            .WithMessage("La URL de TikTok no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlTikTok));

        RuleFor(x => x.UrlYouTube)
            .Must(BeValidUrl)
            .WithMessage("La URL de YouTube no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .MaximumLength(300)
            .WithMessage("La URL de YouTube no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlYouTube));

        RuleFor(x => x.UrlTwitter)
            .Must(BeValidUrl)
            .WithMessage("La URL de Twitter/X no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .MaximumLength(300)
            .WithMessage("La URL de Twitter/X no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlTwitter));

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
