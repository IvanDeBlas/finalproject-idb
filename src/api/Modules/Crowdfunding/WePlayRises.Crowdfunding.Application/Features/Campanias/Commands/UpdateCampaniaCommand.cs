using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class UpdateCampaniaCommand : IRequest<ServiceResponse<bool>>
{
    public Guid Id { get; set; }
    public Guid ArtistaId { get; set; }
    public string? Titulo { get; set; }
    public string? Subtitulo { get; set; }
    public string? DescripcionCorta { get; set; }
    public string? VideoPrincipalUrl { get; set; }
    public string? ImagenPrincipalUrl { get; set; }
    public decimal? ImporteObjetivo { get; set; }
    public decimal? ImporteMinimo { get; set; }
    public int? TipoFinanciacionId { get; set; }
    public bool? PermiteAportacionesAnonimas { get; set; }
    public bool? PermitePropinas { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class UpdateCampaniaCommandHandler : IRequestHandler<UpdateCampaniaCommand, ServiceResponse<bool>>
{
    private readonly ICampaniaService _service;
    private readonly IValidator<UpdateCampaniaCommand> _validator;
    private readonly ILogger<UpdateCampaniaCommandHandler> _logger;

    public UpdateCampaniaCommandHandler(
        ICampaniaService service,
        IValidator<UpdateCampaniaCommand> validator,
        ILogger<UpdateCampaniaCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<bool>> Handle(
        UpdateCampaniaCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar comando
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateCampania: {Errors}",
                    string.Join(", ", validationResult.Errors));

                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Obtener entidad existente
            var campaniaId = new CampaniaCrowdfundingId(request.Id);
            var entity = await _service.GetByIdAsync(campaniaId, cancellationToken);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<bool>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // 3. Validate ownership
            if (entity.ArtistaId.Value != request.ArtistaId)
            {
                _logger.LogWarning(
                    "Artista {ArtistaId} attempted to update Campania {CampaniaId} owned by another",
                    request.ArtistaId, request.Id);

                return new ServiceResponse<bool>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "No tienes permiso para modificar esta campania",
                            ErrorCode = ServiceResponseMessageType.Auth_Forbidden
                        }
                    }
                };
            }

            // 4. Validate state (only BORRADOR can be edited)
            if (entity.EstadoCampaniaId != 1)
            {
                return new ServiceResponse<bool>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Solo se pueden editar campanias en estado borrador",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_CampaniaNotDraft
                        }
                    }
                };
            }

            // 5. Actualizar solo propiedades proporcionadas
            if (!string.IsNullOrEmpty(request.Titulo))
                entity.Titulo = request.Titulo;
            if (request.Subtitulo != null)
                entity.Subtitulo = request.Subtitulo;
            if (request.DescripcionCorta != null)
                entity.DescripcionCorta = request.DescripcionCorta;
            if (request.VideoPrincipalUrl != null)
                entity.VideoPrincipalUrl = request.VideoPrincipalUrl;
            if (request.ImagenPrincipalUrl != null)
                entity.ImagenPrincipalUrl = request.ImagenPrincipalUrl;
            if (request.ImporteObjetivo.HasValue)
                entity.ImporteObjetivo = request.ImporteObjetivo.Value;
            if (request.ImporteMinimo.HasValue)
                entity.ImporteMinimo = request.ImporteMinimo.Value;
            if (request.TipoFinanciacionId.HasValue)
                entity.TipoFinanciacionId = request.TipoFinanciacionId.Value;
            if (request.PermiteAportacionesAnonimas.HasValue)
                entity.PermiteAportacionesAnonimas = request.PermiteAportacionesAnonimas.Value;
            if (request.PermitePropinas.HasValue)
                entity.PermitePropinas = request.PermitePropinas.Value;
            if (request.FechaInicio.HasValue)
                entity.FechaInicio = request.FechaInicio.Value;
            if (request.FechaFin.HasValue)
                entity.FechaFin = request.FechaFin.Value;

            entity.FechaActualizacion = DateTime.UtcNow;

            // 6. Actualizar via servicio
            await _service.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("Campania updated with Id {Id}", request.Id);

            return new ServiceResponse<bool>
            {
                Data = true,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania actualizada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Campania with Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<bool>(
                "Error inesperado al actualizar campania",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
