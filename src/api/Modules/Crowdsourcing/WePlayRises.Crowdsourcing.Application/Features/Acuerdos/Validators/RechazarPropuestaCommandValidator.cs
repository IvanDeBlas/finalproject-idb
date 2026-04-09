using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class RechazarPropuestaCommandValidator : AbstractValidator<RechazarPropuestaCommand>
{
    private readonly IPropuestaCrowdsourcingService _propuestaService;

    public RechazarPropuestaCommandValidator(
        IPropuestaCrowdsourcingService propuestaService)
    {
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));

        // --- Sync field validations ---

        RuleFor(x => x.PropuestaId)
            .NotEmpty()
            .WithMessage("El ID de la propuesta es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Motivo))
            .WithMessage("El motivo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // --- Async business validations ---

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.PropuestaId), ct);
                return propuesta != null;
            })
            .WithMessage("La propuesta no fue encontrada")
            .WithErrorCode(ServiceResponseMessageType.NotFound_Propuesta);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.PropuestaId), ct);
                if (propuesta == null) return true;
                return propuesta.EstadoPropuestaId == EstadoPropuestaConstants.Pendiente;
            })
            .WithMessage("Solo se pueden rechazar propuestas en estado Pendiente")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable);
    }
}
