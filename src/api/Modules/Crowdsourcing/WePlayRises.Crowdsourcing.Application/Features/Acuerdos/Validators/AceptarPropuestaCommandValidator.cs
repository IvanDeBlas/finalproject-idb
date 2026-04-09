using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class AceptarPropuestaCommandValidator : AbstractValidator<AceptarPropuestaCommand>
{
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;

    public AceptarPropuestaCommandValidator(
        IPropuestaCrowdsourcingService propuestaService,
        IAcuerdoCrowdsourcingService acuerdoService)
    {
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));

        // --- Sync field validations ---

        RuleFor(x => x.TituloInterno)
            .NotEmpty()
            .WithMessage("El titulo interno es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MaximumLength(200)
            .WithMessage("El titulo interno no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.FechaInicio)
            .NotEmpty()
            .WithMessage("La fecha de inicio es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x)
            .Must(cmd => !cmd.FechaFinPrevista.HasValue || cmd.FechaFinPrevista.Value > cmd.FechaInicio)
            .When(x => x.FechaFinPrevista.HasValue)
            .WithMessage("La fecha de fin prevista debe ser posterior a la fecha de inicio")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        // --- Async business validations ---

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.PropuestaId), ct);
                return propuesta != null &&
                       propuesta.EstadoPropuestaId == EstadoPropuestaConstants.Pendiente;
            })
            .WithMessage("La propuesta no existe o no esta en estado Pendiente")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_PropuestaNotAcceptable);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var propuesta = await _propuestaService.GetByIdAsync(
                    new PropuestaCrowdsourcingId(command.PropuestaId), ct);
                if (propuesta == null) return true;

                var existe = await _acuerdoService.ExisteAcuerdoActivoParaNecesidadAsync(
                    propuesta.NecesidadId, ct);
                return !existe;
            })
            .WithMessage("Ya existe un acuerdo activo para esta necesidad")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_AcuerdoAlreadyExists);
    }
}
