using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Templates.Validators;

public class GenerarNecesidadesDesdeTemplateValidator : AbstractValidator<GenerarNecesidadesDesdeTemplateCommand>
{
    private readonly IPlantillaProyectoService _plantillaService;

    public GenerarNecesidadesDesdeTemplateValidator(
        IPlantillaProyectoService plantillaService)
    {
        _plantillaService = plantillaService ?? throw new ArgumentNullException(nameof(plantillaService));

        RuleFor(x => x.PlantillaId)
            .NotEmpty()
            .WithMessage("El ID de la plantilla es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ProyectoArtisticoId)
            .NotEmpty()
            .WithMessage("El ID del proyecto artistico es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ArtistaId)
            .NotEmpty()
            .WithMessage("El ID del artista es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.NecesidadesSeleccionadas)
            .NotEmpty()
            .WithMessage("Debe seleccionar al menos una necesidad")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleForEach(x => x.NecesidadesSeleccionadas).ChildRules(item =>
        {
            item.RuleFor(x => x.PlantillaNecesidadId)
                .NotEmpty()
                .WithMessage("El ID de la necesidad es obligatorio")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);

            item.RuleFor(x => x.PresupuestoMin)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PresupuestoMin.HasValue)
                .WithMessage("El presupuesto minimo no puede ser negativo")
                .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

            item.RuleFor(x => x.PresupuestoMax)
                .GreaterThanOrEqualTo(x => x.PresupuestoMin ?? 0)
                .When(x => x.PresupuestoMax.HasValue)
                .WithMessage("El presupuesto maximo debe ser mayor o igual al minimo")
                .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

            item.RuleFor(x => x.MonedaId)
                .GreaterThan(0)
                .WithMessage("La moneda es obligatoria")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);
        });

        RuleFor(x => x.PlantillaId)
            .MustAsync(PlantillaExisteYEstaActiva)
            .WithMessage("La plantilla no existe o no esta activa")
            .WithErrorCode(ServiceResponseMessageType.NotFound_PlantillaProyecto);
    }

    private async Task<bool> PlantillaExisteYEstaActiva(Guid plantillaId, CancellationToken ct)
    {
        var plantilla = await _plantillaService.GetByIdAsync(new PlantillaProyectoId(plantillaId), ct);
        return plantilla != null && plantilla.Activo;
    }
}
