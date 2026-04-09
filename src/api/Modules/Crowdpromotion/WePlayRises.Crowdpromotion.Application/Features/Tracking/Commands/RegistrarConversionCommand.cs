using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Commands;

public class RegistrarConversionCommand : IRequest<ServiceResponse<RegistrarConversionResponseDto>>
{
    public string CodigoReferido { get; set; } = null!;
    public Guid CampaniaCrowdfundingId { get; set; }
    public Guid AportacionCrowdfundingId { get; set; }
    public decimal ValorMonetario { get; set; }
    public int MonedaId { get; set; }
    public string UserIdAfectado { get; set; } = null!;
}

public class RegistrarConversionCommandHandler
    : IRequestHandler<RegistrarConversionCommand, ServiceResponse<RegistrarConversionResponseDto>>
{
    private readonly IPromoEventoService _promoEventoService;
    private readonly IMapper _mapper;
    private readonly IValidator<RegistrarConversionCommand> _validator;
    private readonly ILogger<RegistrarConversionCommandHandler> _logger;

    public RegistrarConversionCommandHandler(
        IPromoEventoService promoEventoService,
        IMapper mapper,
        IValidator<RegistrarConversionCommand> validator,
        ILogger<RegistrarConversionCommandHandler> logger)
    {
        _promoEventoService = promoEventoService ?? throw new ArgumentNullException(nameof(promoEventoService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RegistrarConversionResponseDto>> Handle(
        RegistrarConversionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RegistrarConversionResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Map Command -> Entity
            var entity = _mapper.Map<PromoEvento>(request);
            entity.TipoEventoId = 4; // Backing
            entity.FechaCreacion = DateTime.UtcNow;

            // 3. Atomic operation (delegated to Service)
            var result = await _promoEventoService.RegistrarConversionAsync(entity, cancellationToken);

            // 4. Map response
            var responseDto = new RegistrarConversionResponseDto
            {
                EventoId = result.EventoId,
                ComisionCalculada = result.ComisionCalculada,
                MonedaNombre = result.ComisionAcreditada ? result.MonedaNombre : null,
                WalletTransaccionId = result.WalletTransaccionId,
                ComisionAcreditada = result.ComisionAcreditada
            };

            return new ServiceResponse<RegistrarConversionResponseDto>
            {
                Data = responseDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Conversion registrada", HttpStatusCode = System.Net.HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando conversion. CodigoReferido: {CodigoReferido}, ValorMonetario: {ValorMonetario}",
                request.CodigoReferido, request.ValorMonetario);
            return ValidateExtensions.InternalServerErrorServiceResponse<RegistrarConversionResponseDto>(
                "Error inesperado al registrar la conversion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
