using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class PublishCampaniaCommand : IRequest<ServiceResponse<PublishCampaniaResponse>>
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class PublishCampaniaCommandHandler : IRequestHandler<PublishCampaniaCommand, ServiceResponse<PublishCampaniaResponse>>
{
    private readonly ICampaniaService _service;
    private readonly IValidator<PublishCampaniaCommand> _validator;
    private readonly ILogger<PublishCampaniaCommandHandler> _logger;

    public PublishCampaniaCommandHandler(
        ICampaniaService service,
        IValidator<PublishCampaniaCommand> validator,
        ILogger<PublishCampaniaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PublishCampaniaResponse>> Handle(
        PublishCampaniaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate command
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PublishCampaniaResponse>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Get campaign
            var campaniaId = new CampaniaCrowdfundingId(request.Id);
            var campania = await _service.GetByIdAsync(campaniaId, cancellationToken);

            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PublishCampaniaResponse>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Validate ownership
            if (campania.ArtistaId.Value != request.ArtistaId)
            {
                _logger.LogWarning(
                    "Artista {ArtistaId} attempted to publish Campania {CampaniaId} owned by another",
                    request.ArtistaId, request.Id);

                return new ServiceResponse<PublishCampaniaResponse>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "No tienes permiso para publicar esta campania",
                            ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                        }
                    }
                };
            }

            // 4. Validate state (only BORRADOR can be published)
            if (campania.EstadoCampaniaId != 1)
            {
                return new ServiceResponse<PublishCampaniaResponse>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Solo se pueden publicar campanias en estado borrador",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
                        }
                    }
                };
            }

            // 5. Validate required fields for publication
            var errors = new List<ServiceResponseMessage>();

            if (string.IsNullOrEmpty(campania.Titulo))
            {
                errors.Add(new ServiceResponseMessage
                {
                    Message = "El titulo es obligatorio para publicar",
                    ErrorCode = ServiceResponseMessageType.Validation_Required
                });
            }

            if (campania.ImporteObjetivo <= 0)
            {
                errors.Add(new ServiceResponseMessage
                {
                    Message = "El importe objetivo es obligatorio y debe ser mayor a 0",
                    ErrorCode = ServiceResponseMessageType.Validation_InvalidAmount
                });
            }

            if (campania.MonedaId <= 0)
            {
                errors.Add(new ServiceResponseMessage
                {
                    Message = "La moneda es obligatoria",
                    ErrorCode = ServiceResponseMessageType.Validation_Required
                });
            }

            if (campania.TipoFinanciacionId <= 0)
            {
                errors.Add(new ServiceResponseMessage
                {
                    Message = "El tipo de financiacion es obligatorio",
                    ErrorCode = ServiceResponseMessageType.Validation_Required
                });
            }

            if (campania.FechaFin == null || campania.FechaFin < DateTime.UtcNow.AddDays(7))
            {
                errors.Add(new ServiceResponseMessage
                {
                    Message = "La campania debe durar minimo 7 dias desde hoy",
                    ErrorCode = ServiceResponseMessageType.Validation_InvalidDate
                });
            }

            if (errors.Count > 0)
            {
                return new ServiceResponse<PublishCampaniaResponse>
                {
                    Messages = errors
                };
            }

            // 6. Transition state
            campania.EstadoCampaniaId = 2; // PUBLICADA
            campania.FechaPublicacion = DateTime.UtcNow;

            if (campania.FechaInicio == null)
            {
                campania.FechaInicio = DateTime.UtcNow;
            }

            campania.FechaActualizacion = DateTime.UtcNow;

            // 7. Persist
            await _service.UpdateAsync(campania, cancellationToken);

            // 8. Build response
            var response = new PublishCampaniaResponse
            {
                Id = campania.Id.Value,
                EstadoCampaniaId = campania.EstadoCampaniaId,
                FechaPublicacion = campania.FechaPublicacion.Value,
                Message = "Campania publicada exitosamente"
            };

            _logger.LogInformation(
                "Campania {CampaniaId} published by Artista {ArtistaId}",
                campania.Id.Value, request.ArtistaId);

            return new ServiceResponse<PublishCampaniaResponse>
            {
                Data = response,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania publicada exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing Campania {CampaniaId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<PublishCampaniaResponse>(
                "Error inesperado al publicar campania",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
