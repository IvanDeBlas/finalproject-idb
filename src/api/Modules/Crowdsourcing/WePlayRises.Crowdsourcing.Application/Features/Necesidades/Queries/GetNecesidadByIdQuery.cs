using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetNecesidadByIdQuery : IRequest<ServiceResponse<NecesidadCrowdsourcingDto>>
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetNecesidadByIdQueryHandler : IRequestHandler<GetNecesidadByIdQuery, ServiceResponse<NecesidadCrowdsourcingDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetNecesidadByIdQueryHandler> _logger;

    public GetNecesidadByIdQueryHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        ILogger<GetNecesidadByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadCrowdsourcingDto>> Handle(
        GetNecesidadByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            var necesidadId = new NecesidadCrowdsourcingId(request.Id);
            var entity = await _service.GetByIdWithDetailsAsync(necesidadId, ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<NecesidadCrowdsourcingDto>(
                    "Necesidad no encontrada",
                    ServiceResponseMessageType.NotFound_Necesidad);
            }

            if (entity.ArtistaId.Value != request.ArtistaId)
            {
                _logger.LogWarning(
                    "Unauthorized access attempt to necesidad {NecesidadId} by artista {ArtistaId}",
                    request.Id, request.ArtistaId);

                return ValidateExtensions.ForbiddenServiceResponse<NecesidadCrowdsourcingDto>(
                    "No tienes permiso para ver esta necesidad",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            var dto = _mapper.Map<NecesidadCrowdsourcingDto>(entity);

            return new ServiceResponse<NecesidadCrowdsourcingDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad obtenida exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting necesidad {NecesidadId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadCrowdsourcingDto>(
                "Error inesperado al obtener necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
