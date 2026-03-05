using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Rewards.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Validators;

public class CreateRewardCommandValidator : AbstractValidator<CreateRewardCommand>
{
    public CreateRewardCommandValidator()
    {
        RuleFor(x => x.CampaniaId)
            .NotEmpty()
            .WithMessage("El CampaniaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(200)
            .WithMessage("El nombre no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(2000)
            .WithMessage("La descripcion no puede superar los 2000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Descripcion));

        RuleFor(x => x.ImporteMinimo)
            .GreaterThan(0)
            .WithMessage("El importe minimo debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount);

        RuleFor(x => x.MonedaId)
            .GreaterThan(0)
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TipoRewardId)
            .GreaterThan(0)
            .WithMessage("El tipo de reward es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.CantidadMaxima)
            .GreaterThan(0)
            .WithMessage("La cantidad maxima debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange)
            .When(x => x.CantidadMaxima.HasValue);

        RuleFor(x => x.CantidadPorBacker)
            .GreaterThan(0)
            .WithMessage("La cantidad por backer debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange)
            .When(x => x.CantidadPorBacker.HasValue);

        RuleFor(x => x.TiempoEntregaEstimado)
            .MaximumLength(200)
            .WithMessage("El tiempo de entrega estimado no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.TiempoEntregaEstimado));

        RuleFor(x => x.Orden)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El orden debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);
    }
}
