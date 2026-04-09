using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Validators;

public class CreatePromoProgramaCommandValidator : AbstractValidator<CreatePromoProgramaCommand>
{
    private readonly IPromoProgramaService _promoProgramaService;

    public CreatePromoProgramaCommandValidator(IPromoProgramaService promoProgramaService)
    {
        _promoProgramaService = promoProgramaService;

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El UserId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(5)
            .WithMessage("El titulo debe tener al menos 5 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(200)
            .WithMessage("El titulo no puede superar los 200 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.TipoPromoId)
            .GreaterThan(0)
            .WithMessage("El tipo de programa es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.TipoPromoId)
            .MustAsync(async (id, ct) => await _promoProgramaService.TipoPromoExistsAsync(id, ct))
            .WithMessage("El tipo de programa no existe")
            .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound)
            .When(x => x.TipoPromoId > 0);

        RuleFor(x => x.MonedaId)
            .GreaterThan(0)
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.MonedaId)
            .MustAsync(async (id, ct) => await _promoProgramaService.MonedaExistsAsync(id, ct))
            .WithMessage("La moneda no existe")
            .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound)
            .When(x => x.MonedaId > 0);

        RuleFor(x => x)
            .Must(x => x.ImporteComisionPorcentaje.HasValue || x.ImporteComisionFija.HasValue)
            .WithMessage("Debe definir al menos una comision (porcentaje o fija)")
            .WithErrorCode(ServiceResponseMessageType.Validation_AtLeastOneComision);

        RuleFor(x => x.ImporteComisionPorcentaje)
            .InclusiveBetween(0, 100)
            .WithMessage("La comision porcentaje debe estar entre 0 y 100")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds)
            .When(x => x.ImporteComisionPorcentaje.HasValue);

        RuleFor(x => x.ImporteComisionFija)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La comision fija no puede ser negativa")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds)
            .When(x => x.ImporteComisionFija.HasValue);

        RuleFor(x => x)
            .Must(x =>
            {
                if (x.FechaFin.HasValue && x.FechaInicio.HasValue)
                    return x.FechaFin.Value > x.FechaInicio.Value;
                return true;
            })
            .WithMessage("La fecha fin debe ser posterior a la fecha inicio")
            .WithErrorCode(ServiceResponseMessageType.Validation_DateFinBeforeInicio)
            .When(x => x.FechaFin.HasValue && x.FechaInicio.HasValue);

        RuleFor(x => x.UrlLanding)
            .Must(BeValidUrl)
            .WithMessage("La URL de landing no tiene formato valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
            .When(x => !string.IsNullOrEmpty(x.UrlLanding));

        RuleFor(x => x.UrlLanding)
            .MaximumLength(500)
            .WithMessage("La URL de landing no puede superar los 500 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.UrlLanding));

        RuleFor(x => x.CodigoTrackingBase)
            .MaximumLength(50)
            .WithMessage("El codigo de tracking no puede superar los 50 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
            .When(x => !string.IsNullOrEmpty(x.CodigoTrackingBase));

        RuleFor(x => x.CodigoTrackingBase)
            .Matches(@"^[a-zA-Z0-9-]*$")
            .WithMessage("El codigo de tracking solo puede contener letras, numeros y guiones")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidCodigoTracking)
            .When(x => !string.IsNullOrEmpty(x.CodigoTrackingBase));

        RuleFor(x => x.CodigoTrackingBase)
            .MustAsync(async (command, codigo, ct) =>
                !await _promoProgramaService.CodigoTrackingExistsAsync(command.UserId, codigo, null, ct))
            .WithMessage("El codigo de tracking ya existe para este artista")
            .WithErrorCode(ServiceResponseMessageType.Validation_DuplicateCodigoTracking)
            .When(x => !string.IsNullOrEmpty(x.CodigoTrackingBase));

        RuleForEach(x => x.Tareas).ChildRules(tarea =>
        {
            tarea.RuleFor(t => t.Titulo)
                .NotEmpty()
                .WithMessage("El titulo de la tarea es obligatorio")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required)
                .MinimumLength(3)
                .WithMessage("El titulo de la tarea debe tener al menos 3 caracteres")
                .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
                .MaximumLength(200)
                .WithMessage("El titulo de la tarea no puede superar los 200 caracteres")
                .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

            tarea.RuleFor(t => t.TipoEventoPromoId)
                .GreaterThan(0)
                .WithMessage("El tipo de evento es obligatorio")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);

            tarea.RuleFor(t => t.TipoEventoPromoId)
                .MustAsync(async (id, ct) => await _promoProgramaService.TipoEventoPromoExistsAsync(id, ct))
                .WithMessage("El tipo de evento no existe")
                .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound)
                .When(t => t.TipoEventoPromoId > 0);

            tarea.RuleFor(t => t.TipoRewardId)
                .GreaterThan(0)
                .WithMessage("El tipo de recompensa es obligatorio")
                .WithErrorCode(ServiceResponseMessageType.Validation_Required);

            tarea.RuleFor(t => t.TipoRewardId)
                .MustAsync(async (id, ct) => await _promoProgramaService.TipoRewardExistsAsync(id, ct))
                .WithMessage("El tipo de recompensa no existe")
                .WithErrorCode(ServiceResponseMessageType.Validation_ForeignKeyNotFound)
                .When(t => t.TipoRewardId > 0);

            tarea.RuleFor(t => t)
                .Must(t => !t.EsRepetible || (t.MaxRepeticiones.HasValue && t.MaxRepeticiones.Value >= 1))
                .WithMessage("Las tareas repetibles requieren max repeticiones >= 1")
                .WithErrorCode(ServiceResponseMessageType.Validation_EsRepetibleRequiresMax);

            tarea.RuleFor(t => t)
                .Must(t => (t.TipoRewardId != 1 && t.TipoRewardId != 3) || t.ImporteRecompensa.HasValue)
                .WithMessage("El importe de recompensa es requerido para recompensas monetarias")
                .WithErrorCode(ServiceResponseMessageType.Validation_ImporteRecompensaRequired)
                .When(t => t.TipoRewardId > 0);

            tarea.RuleFor(t => t)
                .Must(t => (t.TipoRewardId != 2 && t.TipoRewardId != 3) || t.PuntosRecompensa.HasValue)
                .WithMessage("Los puntos de recompensa son requeridos para recompensas de puntos")
                .WithErrorCode(ServiceResponseMessageType.Validation_PuntosRecompensaRequired)
                .When(t => t.TipoRewardId > 0);

            tarea.RuleFor(t => t.UrlInstrucciones)
                .Must(BeValidUrl)
                .WithMessage("La URL de instrucciones no tiene formato valido")
                .WithErrorCode(ServiceResponseMessageType.Validation_InvalidUrl)
                .When(t => !string.IsNullOrEmpty(t.UrlInstrucciones));

            tarea.RuleFor(t => t.UrlInstrucciones)
                .MaximumLength(500)
                .WithMessage("La URL de instrucciones no puede superar los 500 caracteres")
                .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength)
                .When(t => !string.IsNullOrEmpty(t.UrlInstrucciones));
        }).When(x => x.Tareas != null && x.Tareas.Count > 0);
    }

    protected static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
