using FluentValidation;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Validators;

public class CreatePropuestaCommandValidator : AbstractValidator<CreatePropuestaCommand>
{
    private readonly IPropuestaCrowdsourcingService _propuestaService;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IArtistaService _artistaService;

    public CreatePropuestaCommandValidator(
        IPropuestaCrowdsourcingService propuestaService,
        INecesidadCrowdsourcingService necesidadService,
        IPerfilProfesionalService perfilService,
        IArtistaService artistaService)
    {
        _propuestaService = propuestaService ?? throw new ArgumentNullException(nameof(propuestaService));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));

        // --- Sync field validations ---

        RuleFor(x => x.PrecioPropuesto)
            .GreaterThan(0)
            .WithMessage("El precio propuesto debe ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.MonedaId)
            .NotEmpty()
            .WithMessage("La moneda es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .GreaterThan(0)
            .WithMessage("La moneda no es valida")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.DiasEstimados)
            .GreaterThan(0)
            .When(x => x.DiasEstimados.HasValue)
            .WithMessage("Los dias estimados deben ser mayor a 0")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidRange);

        RuleFor(x => x.DiasEstimados)
            .LessThanOrEqualTo(365)
            .When(x => x.DiasEstimados.HasValue)
            .WithMessage("El tiempo estimado no puede superar los 365 dias")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        RuleFor(x => x.MensajePropuesta)
            .NotEmpty()
            .WithMessage("El mensaje de propuesta es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(20)
            .WithMessage("El mensaje debe tener al menos 20 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength)
            .MaximumLength(2000)
            .WithMessage("El mensaje no puede superar los 2000 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MaxLength);

        // --- Async business validations ---

        // 1. Necesidad exists, is open, and not expired
        RuleFor(x => x.NecesidadId)
            .MustAsync(async (necesidadId, ct) =>
            {
                var necesidad = await _necesidadService.GetPublicaByIdAsync(
                    new NecesidadCrowdsourcingId(necesidadId), ct);
                return necesidad != null;
            })
            .WithMessage("La necesidad no existe, no esta abierta o ha expirado")
            .WithErrorCode(ServiceResponseMessageType.NotFound_Necesidad);

        // 2. User has a professional profile
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                return await _perfilService.ExistsByUserIdAsync(command.UserId, ct);
            })
            .WithMessage("Debes crear un perfil profesional para enviar propuestas")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_NoProfessionalProfile);

        // 3. User is NOT the artist owner of the necesidad
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var necesidad = await _necesidadService.GetPublicaByIdAsync(
                    new NecesidadCrowdsourcingId(command.NecesidadId), ct);
                if (necesidad == null) return true; // Will fail in validation 1

                var artista = await _artistaService.GetByUserIdAsync(command.UserId, ct);
                return artista == null || necesidad.ArtistaId != artista.Id;
            })
            .WithMessage("No puedes enviar propuesta a tu propia necesidad")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_CannotProposeSelf);

        // 4. No existing active proposal for this necesidad + user
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                var exists = await _propuestaService.ExistePropuestaActivaAsync(
                    new NecesidadCrowdsourcingId(command.NecesidadId),
                    command.UserId,
                    ct);
                return !exists;
            })
            .WithMessage("Ya tienes una propuesta enviada para esta necesidad")
            .WithErrorCode(ServiceResponseMessageType.BusinessRule_AlreadyProposed);
    }
}
