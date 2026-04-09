using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetBackingsByCampaniaQuery : IRequest<ServiceResponse<IReadOnlyList<BackingPublicDto>>>
{
    public Guid CampaniaId { get; set; }
    public int Limit { get; set; } = 20;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetBackingsByCampaniaQueryHandler : IRequestHandler<GetBackingsByCampaniaQuery, ServiceResponse<IReadOnlyList<BackingPublicDto>>>
{
    private readonly ICampaniaService _campaniaService;
    private readonly IPedidoService _pedidoService;
    private readonly IMapper _mapper;
    private readonly IValidator<GetBackingsByCampaniaQuery> _validator;
    private readonly ILogger<GetBackingsByCampaniaQueryHandler> _logger;

    public GetBackingsByCampaniaQueryHandler(
        ICampaniaService campaniaService,
        IPedidoService pedidoService,
        IMapper mapper,
        IValidator<GetBackingsByCampaniaQuery> validator,
        ILogger<GetBackingsByCampaniaQueryHandler> logger)
    {
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<IReadOnlyList<BackingPublicDto>>> Handle(
        GetBackingsByCampaniaQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<IReadOnlyList<BackingPublicDto>>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Verify campania exists
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);
            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<IReadOnlyList<BackingPublicDto>>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Get recent backings
            var limit = Math.Min(request.Limit, 100);
            var pedidos = await _pedidoService.GetRecentByCampaniaIdAsync(campaniaId, limit, cancellationToken);

            // 4. Map to DTOs
            var dtos = new List<BackingPublicDto>();
            foreach (var pedido in pedidos)
            {
                var dto = _mapper.Map<BackingPublicDto>(pedido);
                dto.NombreBacker = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
                    ? "Backer"
                    : "Anonimo";
                dto.RewardNombre = pedido.Lineas.FirstOrDefault()?.Reward?.Nombre;
                dtos.Add(dto);
            }

            return new ServiceResponse<IReadOnlyList<BackingPublicDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Aportes encontrados",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backings for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<IReadOnlyList<BackingPublicDto>>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
