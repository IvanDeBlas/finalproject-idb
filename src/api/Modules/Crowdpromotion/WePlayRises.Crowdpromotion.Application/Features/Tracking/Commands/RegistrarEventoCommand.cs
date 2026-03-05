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

public class RegistrarEventoCommand : IRequest<ServiceResponse<RegistrarEventoResponseDto>>
{
    public string? CodigoReferido { get; set; }
    public int TipoEventoPromoId { get; set; }
    public Guid? CampaniaCrowdfundingId { get; set; }
    public string? UrlOrigen { get; set; }
    public string? UrlReferer { get; set; }
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }
    public string? IpOrigen { get; set; }
    public string? UserIdAfectado { get; set; }
}

public class RegistrarEventoCommandHandler
    : IRequestHandler<RegistrarEventoCommand, ServiceResponse<RegistrarEventoResponseDto>>
{
    private readonly IPromoEventoService _promoEventoService;
    private readonly ITrackingRateLimitService _rateLimitService;
    private readonly IMapper _mapper;
    private readonly IValidator<RegistrarEventoCommand> _validator;
    private readonly ILogger<RegistrarEventoCommandHandler> _logger;

    public RegistrarEventoCommandHandler(
        IPromoEventoService promoEventoService,
        ITrackingRateLimitService rateLimitService,
        IMapper mapper,
        IValidator<RegistrarEventoCommand> validator,
        ILogger<RegistrarEventoCommandHandler> logger)
    {
        _promoEventoService = promoEventoService ?? throw new ArgumentNullException(nameof(promoEventoService));
        _rateLimitService = rateLimitService ?? throw new ArgumentNullException(nameof(rateLimitService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RegistrarEventoResponseDto>> Handle(
        RegistrarEventoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<RegistrarEventoResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Rate limiting (only for Click events = type 1)
            if (request.TipoEventoPromoId == 1 && !string.IsNullOrEmpty(request.IpOrigen))
            {
                var isRateLimited = await _rateLimitService.IsRateLimitedAsync(
                    request.IpOrigen, request.CodigoReferido, cancellationToken);
                if (isRateLimited)
                {
                    return ValidateExtensions.BadRequestServiceResponse<RegistrarEventoResponseDto>(
                        "Demasiados clicks en poco tiempo. Intenta de nuevo en unos minutos",
                        ServiceResponseMessageType.BusinessRule_RateLimitExcedido);
                }
            }

            // 3. Map Command -> Entity
            var entity = _mapper.Map<PromoEvento>(request);
            entity.FechaCreacion = DateTime.UtcNow;

            // 4. Resolve CodigoReferido and persist (done inside service)
            // The service resolves ProgramaId, PromotorId, PromoProgramaPromotorId from CodigoReferido
            var eventoId = await _promoEventoService.RegistrarEventoAsync(entity, cancellationToken);

            // 5. Register rate limit post-insert (only for Click events)
            if (request.TipoEventoPromoId == 1 && !string.IsNullOrEmpty(request.IpOrigen))
            {
                await _rateLimitService.RegisterClickAsync(
                    request.IpOrigen, request.CodigoReferido, cancellationToken);
            }

            // 6. Return success
            var responseDto = new RegistrarEventoResponseDto
            {
                EventoId = eventoId,
                Registrado = true
            };

            return new ServiceResponse<RegistrarEventoResponseDto>
            {
                Data = responseDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Evento registrado", HttpStatusCode = System.Net.HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando PromoEvento tipo {TipoEventoPromoId}", request.TipoEventoPromoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RegistrarEventoResponseDto>(
                "Error inesperado al registrar el evento",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
