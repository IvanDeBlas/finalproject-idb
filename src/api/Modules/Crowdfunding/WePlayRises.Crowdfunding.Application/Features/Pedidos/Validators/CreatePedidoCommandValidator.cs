using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Pedidos.Validators;

public class CreatePedidoCommandValidator : AbstractValidator<CreatePedidoCommand>
{
    public CreatePedidoCommandValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El CampaniaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.MonedaId)
            .GreaterThan(0)
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ImporteSubtotal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El importe subtotal debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ImportePropina)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La propina debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ImporteEnvio)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El importe de envio debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ImporteImpuestos)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El importe de impuestos debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ImporteTotal)
            .GreaterThan(0)
            .WithMessage("El importe total debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.ComentarioBacker)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.ComentarioBacker));
    }
}
