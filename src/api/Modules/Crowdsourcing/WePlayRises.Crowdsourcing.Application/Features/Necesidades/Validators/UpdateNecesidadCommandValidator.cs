using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Validators;

public class UpdateNecesidadCommandValidator : AbstractValidator<UpdateNecesidadCommand>
{
    private readonly INecesidadCrowdsourcingService _necesidadService;

    public UpdateNecesidadCommandValidator(INecesidadCrowdsourcingService necesidadService)
    {
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El título es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(5)
            .WithMessage("El título debe tener al menos 5 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El título no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrEmpty(x.Descripcion))
            .WithMessage("La descripción no puede superar los 4000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.ModalidadTrabajoId)
            .NotEmpty()
            .WithMessage("La modalidad de trabajo es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.PresupuestoMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PresupuestoMin.HasValue)
            .WithMessage("El presupuesto mínimo no puede ser negativo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.PresupuestoMax)
            .GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0)
            .When(x => x.PresupuestoMax.HasValue && x.PresupuestoMin.HasValue)
            .WithMessage("El presupuesto máximo debe ser mayor o igual al mínimo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .When(x => x.PresupuestoMin.HasValue || x.PresupuestoMax.HasValue)
            .WithMessage("La moneda es obligatoria cuando se especifica presupuesto")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UbicacionCiudad)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicación (ciudad) es obligatoria para modalidad Presencial o Híbrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UbicacionCiudad)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.UbicacionCiudad))
            .WithMessage("La ubicación (ciudad) no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.UbicacionPais)
            .NotEmpty()
            .When(x => x.ModalidadTrabajoId == 1 || x.ModalidadTrabajoId == 3)
            .WithMessage("La ubicación (país) es obligatoria para modalidad Presencial o Híbrida")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UbicacionPais)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.UbicacionPais))
            .WithMessage("La ubicación (país) no puede superar los 100 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.FechaLimitePropuestas)
            .GreaterThan(DateTime.UtcNow.Date)
            .When(x => x.FechaLimitePropuestas.HasValue)
            .WithMessage("La fecha límite debe ser posterior a hoy")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        RuleFor(x => x.FechaInicioPrevista)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.FechaInicioPrevista.HasValue)
            .WithMessage("La fecha de inicio debe ser igual o posterior a hoy")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await _necesidadService.GetByIdAsync(new NecesidadCrowdsourcingId(command.Id), ct);
                return necesidad != null
                    && necesidad.EstadoNecesidadId == 1
                    && necesidad.ArtistaId.Value == command.ArtistaId;
            })
            .WithMessage("Solo se pueden editar necesidades en estado Abierta que te pertenezcan")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NecesidadNotEditable);
    }
}
