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
public class GetRewardByIdQuery : IRequest<ServiceResponse<RewardDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetRewardByIdQueryHandler : IRequestHandler<GetRewardByIdQuery, ServiceResponse<RewardDto>>
{
    private readonly IRewardService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRewardByIdQueryHandler> _logger;

    public GetRewardByIdQueryHandler(
        IRewardService service,
        IMapper mapper,
        ILogger<GetRewardByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RewardDto>> Handle(
        GetRewardByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _service.GetByIdAsync(
                new CampaniaCrowdfundingRewardId(request.Id), cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<RewardDto>(
                    "Reward no encontrado",
                    ServiceResponseMessageType.NotFound_Reward);
            }

            var dto = _mapper.Map<RewardDto>(entity);

            return new ServiceResponse<RewardDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Reward encontrado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Reward by Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<RewardDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
