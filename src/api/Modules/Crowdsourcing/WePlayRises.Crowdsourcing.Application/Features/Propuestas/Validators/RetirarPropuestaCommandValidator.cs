using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Validators;

public class RetirarPropuestaCommandValidator : AbstractValidator<RetirarPropuestaCommand>
{
    private readonly IPropuestaCrowdsourcingService _propuestaService;

    public RetirarPropuestaCommandValidator(IPropuestaCrowdsourcingService propuestaService)
    {
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El ID de la propuesta es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // Verify propuesta exists
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.Id), ct);
                return propuesta != null;
            })
            .WithMessage("La propuesta no fue encontrada")
            .WithErrorCode(ServiceResponseMessageType.NotFound_Propuesta);

        // Verify propuesta is in Pendiente state
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.Id), ct);
                // Cache HIT: same object as above
                return propuesta == null
                    || propuesta.EstadoPropuestaId == EstadoPropuestaConstants.Pendiente;
            })
            .WithMessage("Solo se pueden retirar propuestas en estado Pendiente")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_PropuestaNotRetirable);
    }
}
