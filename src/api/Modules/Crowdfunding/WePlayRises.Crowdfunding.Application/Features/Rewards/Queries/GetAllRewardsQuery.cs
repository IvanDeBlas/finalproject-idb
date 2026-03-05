using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Rewards.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetAllRewardsQuery : IRequest<ServiceResponse<IEnumerable<RewardListDto>>>
{
    public Guid? CampaniaId { get; set; }
    public bool? EsActivo { get; set; }
    public bool? EsAddOn { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetAllRewardsQueryHandler : IRequestHandler<GetAllRewardsQuery, ServiceResponse<IEnumerable<RewardListDto>>>
{
    private readonly IRewardService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllRewardsQueryHandler> _logger;

    public GetAllRewardsQueryHandler(
        IRewardService service,
        IMapper mapper,
        ILogger<GetAllRewardsQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<IEnumerable<RewardListDto>>> Handle(
        GetAllRewardsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<Domain.Model.CampaniaCrowdfundingReward> entities;

            // Filtrar por campania si se proporciona
            if (request.CampaniaId.HasValue)
            {
                entities = await _service.GetByCampaniaIdAsync(
                    new CampaniaCrowdfundingId(request.CampaniaId.Value), cancellationToken);
            }
            else
            {
                entities = await _service.GetAllAsync(cancellationToken);
            }

            var filtered = entities.AsEnumerable();

            // Filtrar por estado activo
            if (request.EsActivo.HasValue)
            {
                filtered = filtered.Where(e => e.EsActivo == request.EsActivo.Value);
            }

            // Filtrar por add-on
            if (request.EsAddOn.HasValue)
            {
                filtered = filtered.Where(e => e.EsAddOn == request.EsAddOn.Value);
            }

            // Ordenar por Orden
            filtered = filtered.OrderBy(e => e.Orden);

            var dtos = _mapper.Map<IEnumerable<RewardListDto>>(filtered);

            return new ServiceResponse<IEnumerable<RewardListDto>>
            {
                Data = dtos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all Rewards");
            return ValidateExtensions.InternalServerErrorServiceResponse<IEnumerable<RewardListDto>>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
