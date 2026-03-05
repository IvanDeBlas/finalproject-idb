using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;

public class GetPromotorWalletQuery : IRequest<ServiceResponse<PromotorWalletDto>>
{
    public string UserId { get; set; } = null!;
}

public class GetPromotorWalletQueryHandler
    : IRequestHandler<GetPromotorWalletQuery, ServiceResponse<PromotorWalletDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetPromotorWalletQuery> _validator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetPromotorWalletQueryHandler> _logger;

    private static readonly Dictionary<int, string> MonedaNombres = new()
    {
        { 1, "EUR" }
    };

    public GetPromotorWalletQueryHandler(
        IPromotorWalletService walletService,
        IMapper mapper,
        IValidator<GetPromotorWalletQuery> validator,
        IConfiguration configuration,
        ILogger<GetPromotorWalletQueryHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorWalletDto>> Handle(
        GetPromotorWalletQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PromotorWalletDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorWalletDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            var (wallet, monedaNombre) = await _walletService.GetWalletConMonedaAsync(
                promotor.Id, cancellationToken);
            if (wallet == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorWalletDto>(
                    "No tienes un wallet asignado",
                    ServiceResponseMessageType.NotFound_Wallet);
            }

            var dto = _mapper.Map<PromotorWalletDto>(wallet);
            dto.MonedaNombre = monedaNombre ?? "EUR";
            dto.MinimoRetiro = _configuration.GetValue<decimal>("Crowdpromotion:MinimoRetiro", 10.0m);

            return new ServiceResponse<PromotorWalletDto> { Data = dto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo wallet del promotor para UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorWalletDto>(
                "Error inesperado al obtener el wallet",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
