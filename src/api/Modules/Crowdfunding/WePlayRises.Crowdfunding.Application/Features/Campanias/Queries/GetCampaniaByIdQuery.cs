using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetCampaniaByIdQuery : IRequest<ServiceResponse<CampaniaDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetCampaniaByIdQueryHandler : IRequestHandler<GetCampaniaByIdQuery, ServiceResponse<CampaniaDto>>
{
    private readonly ICampaniaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCampaniaByIdQueryHandler> _logger;

    public GetCampaniaByIdQueryHandler(
        ICampaniaService service,
        IMapper mapper,
        ILogger<GetCampaniaByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaDto>> Handle(
        GetCampaniaByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _service.GetByIdAsync(
                new CampaniaCrowdfundingId(request.Id), cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            var dto = _mapper.Map<CampaniaDto>(entity);

            return new ServiceResponse<CampaniaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania encontrada",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Campania by Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
