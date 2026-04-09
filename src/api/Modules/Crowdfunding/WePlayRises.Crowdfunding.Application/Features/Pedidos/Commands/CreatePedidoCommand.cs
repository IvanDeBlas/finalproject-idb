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
using WePlayRises.Crowdfunding.Domain.Model;

namespace WePlayRises.Crowdfunding.Application.Features.Pedidos.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreatePedidoCommand : IRequest<ServiceResponse<PedidoDto>>
{
    public Guid CampaniaId { get; set; }
    public string? UserId { get; set; }
    public Guid? FanProfileId { get; set; }
    public int MonedaId { get; set; }
    public decimal ImporteSubtotal { get; set; }
    public decimal ImportePropina { get; set; }
    public decimal ImporteEnvio { get; set; }
    public decimal ImporteImpuestos { get; set; }
    public decimal ImporteTotal { get; set; }
    public bool PermitirMostrarNombre { get; set; }
    public string? ComentarioBacker { get; set; }
    public Guid? DireccionEnvioId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreatePedidoCommandHandler : IRequestHandler<CreatePedidoCommand, ServiceResponse<PedidoDto>>
{
    private readonly IPedidoService _service;
    private readonly ICampaniaService _campaniaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePedidoCommand> _validator;
    private readonly ILogger<CreatePedidoCommandHandler> _logger;

    public CreatePedidoCommandHandler(
        IPedidoService service,
        ICampaniaService campaniaService,
        IMapper mapper,
        IValidator<CreatePedidoCommand> validator,
        ILogger<CreatePedidoCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PedidoDto>> Handle(
        CreatePedidoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePedido: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<PedidoDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Verificar que la campania existe y esta activa
            var campaniaId = new CampaniaCrowdfundingId(request.CampaniaId);
            var campania = await _campaniaService.GetByIdAsync(campaniaId, cancellationToken);

            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PedidoDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Mapear a entidad de dominio
            var entity = _mapper.Map<PedidoCrowdfunding>(request);
            entity.Id = PedidoCrowdfundingId.CreateNew();
            entity.EstadoPedidoId = 1; // Pendiente
            entity.FechaCreacion = DateTime.UtcNow;

            // 4. Crear via servicio
            var id = await _service.CreateAsync(entity, cancellationToken);

            // 5. Obtener entidad creada y mapear a DTO
            var created = await _service.GetByIdAsync(id, cancellationToken);
            var dto = _mapper.Map<PedidoDto>(created);

            _logger.LogInformation("Pedido created with Id {Id} for Campania {CampaniaId}",
                id.Value, request.CampaniaId);

            // 6. Retornar respuesta exitosa
            return new ServiceResponse<PedidoDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Pedido creado correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Pedido for Campania {CampaniaId}", request.CampaniaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PedidoDto>(
                "Error inesperado al crear pedido",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
