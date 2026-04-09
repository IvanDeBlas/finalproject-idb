using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;

public class CerrarNecesidadCommandValidator : AbstractValidator<CerrarNecesidadCommand>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;

    public CerrarNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));

        RuleFor(x => x.Motivo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Motivo))
            .WithMessage("El motivo no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await _necesidadService.GetByIdAsync(new NecesidadCrowdsourcingId(command.Id), ct);
                return necesidad != null
                    && (necesidad.EstadoNecesidadId == 1 || necesidad.EstadoNecesidadId == 2)
                    && necesidad.ArtistaId.Value == command.ArtistaId;
            })
            .WithMessage("Solo se pueden cerrar necesidades en estado Abierta o En Progreso que te pertenezcan")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotCloseable);
    }
}
