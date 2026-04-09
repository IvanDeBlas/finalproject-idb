using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;

public class GetWalletTransaccionesQuery : IRequest<ServiceResponse<WalletTransaccionesPagedDto>>
{
    public string UserId { get; set; } = null!;
    public bool? EsCredito { get; set; }
    public int? EstadoTransaccionId { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetWalletTransaccionesQueryHandler
    : IRequestHandler<GetWalletTransaccionesQuery, ServiceResponse<WalletTransaccionesPagedDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetWalletTransaccionesQuery> _validator;
    private readonly ILogger<GetWalletTransaccionesQueryHandler> _logger;

    private static readonly Dictionary<int, string> EstadoTransaccionNombres = new()
    {
        { 1, "Pendiente" },
        { 2, "Procesada" },
        { 3, "Pagada" },
        { 4, "Cancelada" }
    };

    private static readonly Dictionary<int, string> TipoRewardNombres = new()
    {
        { 1, "Comision por venta" },
        { 2, "Comision por clic" },
        { 3, "Recompensa por tarea" }
    };

    public GetWalletTransaccionesQueryHandler(
        IPromotorWalletService walletService,
        IMapper mapper,
        IValidator<GetWalletTransaccionesQuery> validator,
        ILogger<GetWalletTransaccionesQueryHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<WalletTransaccionesPagedDto>> Handle(
        GetWalletTransaccionesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<WalletTransaccionesPagedDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<WalletTransaccionesPagedDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            var (wallet, _) = await _walletService.GetWalletConMonedaAsync(
                promotor.Id, cancellationToken);
            if (wallet == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<WalletTransaccionesPagedDto>(
                    "No tienes un wallet asignado",
                    ServiceResponseMessageType.NotFound_Wallet);
            }

            var (items, totalCount) = await _walletService.GetTransaccionesPagedAsync(
                walletId: wallet.Id,
                esCredito: request.EsCredito,
                estadoTransaccionId: request.EstadoTransaccionId,
                fechaDesde: request.FechaDesde,
                fechaHasta: request.FechaHasta,
                page: request.Page,
                pageSize: request.PageSize,
                ct: cancellationToken);

            var itemDtos = _mapper.Map<List<WalletTransaccionItemDto>>(items);

            // Resolve Maestra names (CoreContext entities not available via navigation)
            foreach (var dto in itemDtos)
            {
                if (EstadoTransaccionNombres.TryGetValue(dto.EstadoTransaccionId, out var estadoNombre))
                    dto.EstadoTransaccionNombre = estadoNombre;

                if (dto.TipoRewardId.HasValue && TipoRewardNombres.TryGetValue(dto.TipoRewardId.Value, out var rewardNombre))
                    dto.TipoRewardNombre = rewardNombre;
            }

            var effectivePageSize = request.PageSize > 0 ? request.PageSize : 1;
            var totalPages = (int)Math.Ceiling((double)totalCount / effectivePageSize);

            var pagedDto = new WalletTransaccionesPagedDto
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return new ServiceResponse<WalletTransaccionesPagedDto> { Data = pagedDto };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error obteniendo transacciones del wallet para UserId {UserId}, Page {Page}, PageSize {PageSize}",
                request.UserId, request.Page, request.PageSize);
            return ValidateExtensions.InternalServerErrorServiceResponse<WalletTransaccionesPagedDto>(
                "Error inesperado al obtener las transacciones del wallet",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
