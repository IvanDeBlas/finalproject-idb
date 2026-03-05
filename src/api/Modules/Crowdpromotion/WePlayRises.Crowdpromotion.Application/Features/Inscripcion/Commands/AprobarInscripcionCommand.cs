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

public class AprobarInscripcionCommand : IRequest<ServiceResponse<InscripcionAprobadaDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid InscripcionId { get; set; }
    public string UserId { get; set; } = null!;
}

public class AprobarInscripcionCommandHandler
    : IRequestHandler<AprobarInscripcionCommand, ServiceResponse<InscripcionAprobadaDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly IValidator<AprobarInscripcionCommand> _validator;
    private readonly ILogger<AprobarInscripcionCommandHandler> _logger;

    public AprobarInscripcionCommandHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        IValidator<AprobarInscripcionCommand> validator,
        ILogger<AprobarInscripcionCommandHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionAprobadaDto>> Handle(
        AprobarInscripcionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for AprobarInscripcion: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<InscripcionAprobadaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionAprobadaDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionAprobadaDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<InscripcionAprobadaDto>(
                    "No eres propietario de este programa de promocion",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, cancellationToken);
            if (inscripcion == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionAprobadaDto>(
                    "La inscripcion no existe", ServiceResponseMessageType.NotFound_Inscripcion);

            if (inscripcion.EsAprobado || inscripcion.EsBloqueado || inscripcion.FechaBaja != null)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionAprobadaDto>(
                    "La inscripcion no esta en estado pendiente",
                    ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);

            var codigoBase = string.IsNullOrEmpty(programa.CodigoTrackingBase)
                ? request.ProgramaId.ToString("N")[..8]
                : programa.CodigoTrackingBase;

            string? codigoReferido = null;
            for (int i = 0; i < 3; i++)
            {
                var shortId = Guid.NewGuid().ToString("N")[..5];
                var candidato = $"{codigoBase}-{shortId}";
                if (!await _inscripcionService.CodigoReferidoExistsAsync(candidato, cancellationToken))
                {
                    codigoReferido = candidato;
                    break;
                }
            }

            if (codigoReferido == null)
                return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionAprobadaDto>(
                    "No se pudo generar un codigo unico de referido",
                    ServiceResponseMessageType.Internal_UnexpectedError);

            string? urlTracking = null;
            if (!string.IsNullOrEmpty(programa.UrlLanding))
            {
                urlTracking = $"{programa.UrlLanding}?utm_source=weplay&utm_medium=referral&utm_campaign={codigoBase}&ref={codigoReferido}";
            }

            inscripcion.EsAprobado = true;
            inscripcion.CodigoReferido = codigoReferido;
            inscripcion.UrlReferido = urlTracking;

            await _inscripcionService.AprobarAsync(inscripcion, cancellationToken);

            var dto = _mapper.Map<InscripcionAprobadaDto>(inscripcion);
            dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty;

            _logger.LogInformation(
                "Inscripcion {InscripcionId} approved in ProgramaId {ProgramaId}. CodigoReferido: {CodigoReferido}",
                request.InscripcionId, request.ProgramaId, codigoReferido);

            return new ServiceResponse<InscripcionAprobadaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Inscripcion aprobada exitosamente", ErrorCode = ServiceResponseMessageType.Updated }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving Inscripcion {InscripcionId} in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionAprobadaDto>(
                "Error inesperado al aprobar la inscripcion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
