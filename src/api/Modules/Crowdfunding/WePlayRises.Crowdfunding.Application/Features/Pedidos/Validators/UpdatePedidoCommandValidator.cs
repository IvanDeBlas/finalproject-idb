using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Pedidos.Validators;

public class UpdatePedidoCommandValidator : AbstractValidator<UpdatePedidoCommand>
{
    public UpdatePedidoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El Id es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.EstadoPedidoId)
            .GreaterThan(0)
            .WithMessage("El estado de pedido debe ser valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat)
            .When(x => x.EstadoPedidoId.HasValue);

        RuleFor(x => x.ImportePropina)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La propina debe ser mayor o igual a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidAmount)
            .When(x => x.ImportePropina.HasValue);

        RuleFor(x => x.ComentarioBacker)
            .MaximumLength(500)
            .WithMessage("El comentario no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.ComentarioBacker));
    }
}
