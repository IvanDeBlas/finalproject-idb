using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;

public class RegistrarEventoCommandValidator : AbstractValidator<RegistrarEventoCommand>
{
    public RegistrarEventoCommandValidator()
    {
        RuleFor(x => x.TipoEventoPromoId)
            .GreaterThan(0)
            .WithMessage("El tipo de evento es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TipoEventoPromoId)
            .InclusiveBetween(1, 5)
            .WithMessage("Tipo de evento invalido. Valores validos: 1 (Click), 2 (PageView), 3 (Signup), 4 (Backing), 5 (Share)")
            .WithErrorCode(ServiceResponseMessageType.Validation_TipoEventoInvalido)
            .When(x => x.TipoEventoPromoId > 0);

        RuleFor(x => x.TipoEventoPromoId)
            .NotEqual(4)
            .WithMessage("El tipo de evento Backing no se acepta en este endpoint. Use el endpoint de conversion")
            .WithErrorCode(ServiceResponseMessageType.Validation_BackingNoPermitido)
            .When(x => x.TipoEventoPromoId >= 1 && x.TipoEventoPromoId <= 5);

        RuleFor(x => x.CodigoReferido)
            .MaximumLength(50)
            .WithMessage("El codigo referido no puede superar los 50 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.CodigoReferido));

        RuleFor(x => x.UrlOrigen)
            .MaximumLength(2048)
            .WithMessage("La URL de origen no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlOrigen));

        RuleFor(x => x.UrlReferer)
            .MaximumLength(2048)
            .WithMessage("La URL del referer no puede superar los 2048 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlReferer));

        RuleFor(x => x.UtmSource)
            .MaximumLength(100)
            .WithMessage("El utm_source no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmSource));

        RuleFor(x => x.UtmMedium)
            .MaximumLength(100)
            .WithMessage("El utm_medium no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmMedium));

        RuleFor(x => x.UtmCampaign)
            .MaximumLength(100)
            .WithMessage("El utm_campaign no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UtmCampaign));
    }
}
