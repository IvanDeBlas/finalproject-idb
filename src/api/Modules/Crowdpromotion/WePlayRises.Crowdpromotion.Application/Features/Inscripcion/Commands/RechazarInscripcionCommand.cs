using System.Net;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;

public class RechazarInscripcionCommand : IRequest<ServiceResponse<InscripcionRechazadaDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid InscripcionId { get; set; }
    public string UserId { get; set; } = null!;
}

public class RechazarInscripcionCommandHandler
    : IRequestHandler<RechazarInscripcionCommand, ServiceResponse<InscripcionRechazadaDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly IValidator<RechazarInscripcionCommand> _validator;
    private readonly ILogger<RechazarInscripcionCommandHandler> _logger;

    public RechazarInscripcionCommandHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        IValidator<RechazarInscripcionCommand> validator,
        ILogger<RechazarInscripcionCommandHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionRechazadaDto>> Handle(
        RechazarInscripcionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for RechazarInscripcion: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<InscripcionRechazadaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionRechazadaDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionRechazadaDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<InscripcionRechazadaDto>(
                    "No eres propietario de este programa de promocion",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, cancellationToken);
            if (inscripcion == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionRechazadaDto>(
                    "La inscripcion no existe", ServiceResponseMessageType.NotFound_Inscripcion);

            if (inscripcion.EsAprobado || inscripcion.EsBloqueado || inscripcion.FechaBaja != null)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionRechazadaDto>(
                    "La inscripcion no esta en estado pendiente",
                    ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);

            var promotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty;

            await _inscripcionService.RechazarAsync(request.InscripcionId, cancellationToken);

            var dto = new InscripcionRechazadaDto
            {
                InscripcionId = inscripcion.Id,
                PromotorNombre = promotorNombre
            };

            _logger.LogInformation(
                "Inscripcion {InscripcionId} rejected in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);

            return new ServiceResponse<InscripcionRechazadaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Inscripcion rechazada exitosamente", ErrorCode = ServiceResponseMessageType.Deleted }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting Inscripcion {InscripcionId} in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionRechazadaDto>(
                "Error inesperado al rechazar la inscripcion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
