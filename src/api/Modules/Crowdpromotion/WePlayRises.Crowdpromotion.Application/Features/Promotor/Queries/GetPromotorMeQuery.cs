using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Promotor.Queries;

public class GetPromotorMeQuery : IRequest<ServiceResponse<PromotorDto>>
{
    public string UserId { get; set; } = null!;
}

public class GetPromotorMeQueryHandler
    : IRequestHandler<GetPromotorMeQuery, ServiceResponse<PromotorDto>>
{
    private readonly IPromotorService _promotorService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPromotorMeQueryHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromotorNombres = new()
    {
        { 1, "Fan Embajador" },
        { 2, "Influencer" },
        { 3, "Medio / Blog" },
        { 4, "Profesional Marketing" }
    };

    public GetPromotorMeQueryHandler(
        IPromotorService promotorService,
        IMapper mapper,
        ILogger<GetPromotorMeQueryHandler> logger)
    {
        _promotorService = promotorService ?? throw new ArgumentNullException(nameof(promotorService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorDto>> Handle(
        GetPromotorMeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var promotor = await _promotorService.GetByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            var dto = _mapper.Map<PromotorDto>(promotor);

            if (TipoPromotorNombres.TryGetValue(promotor.TipoPromotorId, out var tipoNombre))
            {
                dto.TipoPromotorNombre = tipoNombre;
            }

            dto.TotalProgramasActivos = await _promotorService.GetProgramasActivosCountAsync(promotor.Id, cancellationToken);

            var walletEur = await _promotorService.GetWalletEurAsync(promotor.Id, cancellationToken);
            dto.TotalComisionesGanadas = walletEur?.TotalGanado ?? 0m;
            dto.MonedaComisiones = "EUR";

            return new ServiceResponse<PromotorDto>
            {
                Data = dto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Promotor for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorDto>(
                "Error inesperado al obtener el perfil de promotor",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
