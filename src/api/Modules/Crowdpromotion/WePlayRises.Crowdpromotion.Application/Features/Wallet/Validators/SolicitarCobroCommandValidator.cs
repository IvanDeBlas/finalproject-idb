using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class SolicitarCobroCommandValidator : AbstractValidator<SolicitarCobroCommand>
{
    public SolicitarCobroCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Importe)
            .GreaterThan(0m)
            .WithMessage("El importe debe ser mayor que cero")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripcion no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => x.Descripcion != null);
    }
}
