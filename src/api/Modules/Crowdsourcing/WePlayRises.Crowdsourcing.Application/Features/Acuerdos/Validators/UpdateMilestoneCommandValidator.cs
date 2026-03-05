using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Validators;

public class UpdateMilestoneCommandValidator : AbstractValidator<UpdateMilestoneCommand>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IAcuerdoCrowdsourcingMilestoneService _milestoneService;

    public UpdateMilestoneCommandValidator(
        IAcuerdoCrowdsourcingService acuerdoService,
        IAcuerdoCrowdsourcingMilestoneService milestoneService)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _milestoneService = milestoneService ?? throw new ArgumentNullException(nameof(milestoneService));

        // --- Sync field validations ---

        RuleFor(x => x.MilestoneId)
            .NotEmpty()
            .WithMessage("El ID del milestone es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(3)
            .WithMessage("El titulo debe tener al menos 3 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.Descripcion)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Descripcion))
            .WithMessage("La descripcion no puede superar los 1000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.ImporteParcial)
            .GreaterThan(0)
            .WithMessage("El importe parcial debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        // --- Async business validations ---

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                if (!command.FechaLimite.HasValue) return true;
                var acuerdo = await _acuerdoService.GetByIdAsync(
                    new AcuerdoCrowdsourcingId(command.AcuerdoId), ct);
                if (acuerdo == null) return true;
                return command.FechaLimite.Value >= acuerdo.FechaInicio;
            })
            .When(x => x.FechaLimite.HasValue)
            .WithMessage("La fecha limite no puede ser anterior a la fecha de inicio del acuerdo")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidDate);

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var acuerdo = await _acuerdoService.GetByIdAsync(
                    new AcuerdoCrowdsourcingId(command.AcuerdoId), ct);
                if (acuerdo == null) return true;

                var sumaExcluyendo = await _milestoneService.GetSumaImportesExcluyendoAsync(
                    new AcuerdoCrowdsourcingId(command.AcuerdoId), command.MilestoneId, ct);
                return (sumaExcluyendo + command.ImporteParcial) <= acuerdo.ImporteTotalPactado;
            })
            .WithMessage("La suma de importes de milestones supera el importe total pactado")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_MilestoneImporteExceeded);
    }
}
