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

public class BloquearInscripcionCommand : IRequest<ServiceResponse<InscripcionBloqueadaDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid InscripcionId { get; set; }
    public string UserId { get; set; } = null!;
}

public class BloquearInscripcionCommandHandler
    : IRequestHandler<BloquearInscripcionCommand, ServiceResponse<InscripcionBloqueadaDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly IValidator<BloquearInscripcionCommand> _validator;
    private readonly ILogger<BloquearInscripcionCommandHandler> _logger;

    public BloquearInscripcionCommandHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        IValidator<BloquearInscripcionCommand> validator,
        ILogger<BloquearInscripcionCommandHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionBloqueadaDto>> Handle(
        BloquearInscripcionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for BloquearInscripcion: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<InscripcionBloqueadaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionBloqueadaDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionBloqueadaDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<InscripcionBloqueadaDto>(
                    "No eres propietario de este programa de promocion",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, cancellationToken);
            if (inscripcion == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionBloqueadaDto>(
                    "La inscripcion no existe", ServiceResponseMessageType.NotFound_Inscripcion);

            if (inscripcion.EsBloqueado)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionBloqueadaDto>(
                    "Este promotor ya esta bloqueado en este programa",
                    ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);

            inscripcion.EsBloqueado = true;
            if (inscripcion.EsAprobado)
                inscripcion.EsAprobado = false;

            await _inscripcionService.BloquearAsync(inscripcion, cancellationToken);

            var dto = _mapper.Map<InscripcionBloqueadaDto>(inscripcion);
            dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty;

            _logger.LogInformation(
                "Inscripcion {InscripcionId} blocked in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);

            return new ServiceResponse<InscripcionBloqueadaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Promotor bloqueado exitosamente", ErrorCode = ServiceResponseMessageType.Updated }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking Inscripcion {InscripcionId} in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionBloqueadaDto>(
                "Error inesperado al bloquear la inscripcion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
