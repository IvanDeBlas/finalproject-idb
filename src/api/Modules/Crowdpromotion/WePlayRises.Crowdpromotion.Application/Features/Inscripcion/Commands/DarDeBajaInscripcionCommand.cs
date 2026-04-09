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

public class DarDeBajaInscripcionCommand : IRequest<ServiceResponse<InscripcionDadaDeBajaDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid InscripcionId { get; set; }
    public string UserId { get; set; } = null!;
}

public class DarDeBajaInscripcionCommandHandler
    : IRequestHandler<DarDeBajaInscripcionCommand, ServiceResponse<InscripcionDadaDeBajaDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly IValidator<DarDeBajaInscripcionCommand> _validator;
    private readonly ILogger<DarDeBajaInscripcionCommandHandler> _logger;

    public DarDeBajaInscripcionCommandHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        IValidator<DarDeBajaInscripcionCommand> validator,
        ILogger<DarDeBajaInscripcionCommandHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionDadaDeBajaDto>> Handle(
        DarDeBajaInscripcionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for DarDeBajaInscripcion: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<InscripcionDadaDeBajaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var artistaId = await _inscripcionService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionDadaDeBajaDto>(
                    "No tienes un perfil de artista", ServiceResponseMessageType.NotFound_Artista);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionDadaDeBajaDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (programa.ArtistaId != artistaId.Value)
                return ValidateExtensions.ForbiddenServiceResponse<InscripcionDadaDeBajaDto>(
                    "No eres propietario de este programa de promocion",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            var inscripcion = await _inscripcionService.GetByIdWithPromotorAsync(request.InscripcionId, cancellationToken);
            if (inscripcion == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionDadaDeBajaDto>(
                    "La inscripcion no existe", ServiceResponseMessageType.NotFound_Inscripcion);

            if (!inscripcion.EsAprobado || inscripcion.FechaBaja != null)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionDadaDeBajaDto>(
                    "Esta inscripcion no esta en estado aprobado",
                    ServiceResponseMessageType.BusinessRule_InscripcionEstadoInvalido);

            inscripcion.FechaBaja = DateTime.UtcNow;
            inscripcion.EsAprobado = false;

            await _inscripcionService.DarDeBajaAsync(inscripcion, cancellationToken);

            var dto = _mapper.Map<InscripcionDadaDeBajaDto>(inscripcion);
            dto.PromotorNombre = inscripcion.Promotor?.NombrePublico ?? string.Empty;

            _logger.LogInformation(
                "Inscripcion {InscripcionId} given baja in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);

            return new ServiceResponse<InscripcionDadaDeBajaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Promotor dado de baja exitosamente", ErrorCode = ServiceResponseMessageType.Updated }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error giving baja to Inscripcion {InscripcionId} in ProgramaId {ProgramaId}",
                request.InscripcionId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionDadaDeBajaDto>(
                "Error inesperado al dar de baja la inscripcion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
