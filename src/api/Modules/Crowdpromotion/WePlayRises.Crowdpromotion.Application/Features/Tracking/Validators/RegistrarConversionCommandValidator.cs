using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;

public class RegistrarConversionCommandValidator : AbstractValidator<RegistrarConversionCommand>
{
    public RegistrarConversionCommandValidator()
    {
        RuleFor(x => x.CodigoReferido)
            .NotEmpty()
            .WithMessage("El codigo referido es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.CodigoReferido)
            .MaximumLength(50)
            .WithMessage("El codigo referido no puede superar los 50 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.CodigoReferido));

        RuleFor(x => x.CampaniaCrowdfundingId)
            .NotEmpty()
            .WithMessage("La campana es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.AportacionCrowdfundingId)
            .NotEmpty()
            .WithMessage("La aportacion es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ValorMonetario)
            .GreaterThan(0)
            .WithMessage("El valor monetario debe ser mayor que cero")
            .WithErrorCode(ServiceResponseMessageType.Validation_ValorMonetarioInvalido);

        RuleFor(x => x.MonedaId)
            .GreaterThan(0)
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserIdAfectado)
            .NotEmpty()
            .WithMessage("El usuario que realizo el backing es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
