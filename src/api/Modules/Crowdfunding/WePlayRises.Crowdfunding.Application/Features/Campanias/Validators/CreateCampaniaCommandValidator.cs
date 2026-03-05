using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class CreateCampaniaCommandValidator : AbstractValidator<CreateCampaniaCommand>
{
    public CreateCampaniaCommandValidator()
    {
        RuleFor(x => x.ArtistaId)
            .NotEmpty()
            .WithMessage("El ArtistaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Subtitulo)
            .MaximumLength(300)
            .WithMessage("El subtitulo no puede superar los 300 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Subtitulo));

        RuleFor(x => x.DescripcionCorta)
            .MaximumLength(500)
            .WithMessage("La descripcion corta no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.DescripcionCorta));

        RuleFor(x => x.VideoPrincipalUrl)
            .MaximumLength(500)
            .WithMessage("La URL del video no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .Must(BeValidUrl)
            .WithMessage("La URL del video no es valida")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.VideoPrincipalUrl));

        RuleFor(x => x.ImagenPrincipalUrl)
            .MaximumLength(500)
            .WithMessage("La URL de la imagen no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .Must(BeValidUrl)
            .WithMessage("La URL de la imagen no es valida")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.ImagenPrincipalUrl));

        RuleFor(x => x.MonedaId)
            .GreaterThan(0)
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ImporteObjetivo)
            .GreaterThan(0)
            .WithMessage("El importe objetivo debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ImporteMinimo)
            .GreaterThan(0)
            .WithMessage("El importe minimo debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount)
            .LessThanOrEqualTo(x => x.ImporteObjetivo)
            .WithMessage("El importe minimo no puede ser mayor al importe objetivo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange)
            .When(x => x.ImporteMinimo.HasValue);

        RuleFor(x => x.TipoFinanciacionId)
            .GreaterThan(0)
            .WithMessage("El tipo de financiacion es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.FechaFin)
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate)
            .When(x => x.FechaInicio.HasValue && x.FechaFin.HasValue);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
