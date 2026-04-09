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

namespace WePlayRises.Crowdfunding.Application.Features.Backings.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateBackingCommand : IRequest<ServiceResponse<BackingDto>>
{
    public Guid CampaniaId { get; set; }
    public Guid? RewardId { get; set; }
    public decimal Monto { get; set; }
    public string? Mensaje { get; set; }
    public bool EsAnonimo { get; set; }

    // Injected from Controller (not from body)
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateBackingCommandHandler : IRequestHandler<CreateBackingCommand, ServiceResponse<BackingDto>>
{
    private readonly IBackingService _backingService;
    private readonly ICampaniaService _campaniaService;
    private readonly IRewardService _rewardService;
    private readonly IPedidoService _pedidoService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateBackingCommand> _validator;
    private readonly ILogger<CreateBackingCommandHandler> _logger;

    public CreateBackingCommandHandler(
        IBackingService backingService,
        ICampaniaService campaniaService,
        IRewardService rewardService,
        IPedidoService pedidoService,
        IMapper mapper,
        IValidator<CreateBackingCommand> validator,
        ILogger<CreateBackingCommandHandler> logger)
    {
        _backingService = backingService ?? throw new ArgumentNullException(nameof(backingService));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<BackingDto>> Handle(
        CreateBackingCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateBacking: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<BackingDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Load required data (cached via services)
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);

            Domain.Model.CampaniaCrowdfundingReward? reward = null;
            CampaniaCrowdfundingRewardId? rewardTypedId = null;
            if (request.RewardId.HasValue)
            {
                rewardTypedId = new CampaniaCrowdfundingRewardId(request.RewardId.Value);
                reward = await _rewardService.GetByIdAsync(rewardTypedId.Value, cancellationToken);
            }

            // 3. Create backing via service (atomic transaction)
            var pedidoId = await _backingService.CreateBackingAsync(
                campaniaId,
                rewardTypedId,
                request.Monto,
                request.UserId,
                request.Mensaje,
                request.EsAnonimo,
                cancellationToken);

            // 4. Get created pedido
            var pedido = await _pedidoService.GetByIdAsync(pedidoId, cancellationToken);

            // 5. Map to DTO
            var dto = _mapper.Map<BackingDto>(pedido);

            // 6. Resolve custom fields
            dto.CampaniaTitulo = campania!.Titulo;
            dto.RewardNombre = reward?.Nombre;
            dto.MonedaSimbolo = "EUR";
            dto.EstadoPedido = "Completado";
            dto.UserName = request.EsAnonimo || string.IsNullOrEmpty(request.UserId)
                ? "Anonimo"
                : request.UserName;

            _logger.LogInformation("Backing created: Pedido {PedidoId}, Campania {CampaniaId}, Monto {Monto}",
                pedidoId.Value, request.CampaniaId, request.Monto);

            // 7. Return success
            return new ServiceResponse<BackingDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Aporte realizado con exito. Gracias por tu apoyo!",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Backing for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<BackingDto>(
                "Error inesperado al procesar el aporte",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
