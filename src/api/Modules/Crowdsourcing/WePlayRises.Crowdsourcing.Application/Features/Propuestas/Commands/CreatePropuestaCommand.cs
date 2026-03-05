using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreatePropuestaCommand : IRequest<ServiceResponse<PropuestaCreatedResultDto>>
{
    public Guid NecesidadId { get; set; }
    public string UserId { get; set; } = null!;
    public decimal PrecioPropuesto { get; set; }
    public int MonedaId { get; set; }
    public int? DiasEstimados { get; set; }
    public string MensajePropuesta { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreatePropuestaCommandHandler : IRequestHandler<CreatePropuestaCommand, ServiceResponse<PropuestaCreatedResultDto>>
{
    private readonly IPropuestaCrowdsourcingService _service;
    private readonly INecesidadCrowdsourcingService _necesidadService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePropuestaCommand> _validator;
    private readonly ILogger<CreatePropuestaCommandHandler> _logger;

    public CreatePropuestaCommandHandler(
        IPropuestaCrowdsourcingService service,
        INecesidadCrowdsourcingService necesidadService,
        IPerfilProfesionalService perfilService,
        IMapper mapper,
        IValidator<CreatePropuestaCommand> validator,
        ILogger<CreatePropuestaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _necesidadService = necesidadService ?? throw new ArgumentNullException(nameof(necesidadService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PropuestaCreatedResultDto>> Handle(
        CreatePropuestaCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PropuestaCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var perfil = await _perfilService.GetByUserIdAsync(request.UserId, ct);
            if (perfil == null)
            {
                return new ServiceResponse<PropuestaCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Debes crear un perfil profesional para enviar propuestas",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_NoProfessionalProfile
                        }
                    }
                };
            }

            var entity = _mapper.Map<PropuestaCrowdsourcing>(request);
            entity.Id = PropuestaCrowdsourcingId.CreateNew();
            entity.NecesidadId = new NecesidadCrowdsourcingId(request.NecesidadId);
            entity.UserId = request.UserId;
            entity.PerfilProfesionalId = perfil.Id;
            entity.EstadoPropuestaId = EstadoPropuestaConstants.Pendiente;
            entity.FechaCreacion = DateTime.UtcNow;
            entity.FechaActualizacion = null;
            entity.MotivoRechazo = null;

            var id = await _service.CreateAsync(entity, ct);

            var necesidad = await _necesidadService.GetPublicaByIdAsync(
                new NecesidadCrowdsourcingId(request.NecesidadId), ct);

            var resultDto = new PropuestaCreatedResultDto
            {
                Id = id.Value,
                NecesidadTitulo = necesidad?.Titulo ?? string.Empty,
                PrecioPropuesto = entity.PrecioPropuesto,
                EstadoPropuestaNombre = "Pendiente",
                FechaCreacion = entity.FechaCreacion
            };

            return new ServiceResponse<PropuestaCreatedResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Propuesta enviada correctamente. El artista sera notificado.",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating propuesta for necesidad {NecesidadId} by user {UserId}",
                request.NecesidadId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PropuestaCreatedResultDto>(
                "Error inesperado al enviar propuesta",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
