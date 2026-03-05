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

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;

public class RechazarTareaCommand : IRequest<ServiceResponse<RechazarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string ComentarioValidacion { get; set; } = null!;
    public string UserId { get; set; } = null!;
}

public class RechazarTareaCommandHandler
    : IRequestHandler<RechazarTareaCommand, ServiceResponse<RechazarTareaResponseDto>>
{
    private readonly IPromoTareaService _promoTareaService;
    private readonly IMapper _mapper;
    private readonly IValidator<RechazarTareaCommand> _validator;
    private readonly ILogger<RechazarTareaCommandHandler> _logger;

    public RechazarTareaCommandHandler(
        IPromoTareaService promoTareaService,
        IMapper mapper,
        IValidator<RechazarTareaCommand> validator,
        ILogger<RechazarTareaCommandHandler> logger)
    {
        _promoTareaService = promoTareaService ?? throw new ArgumentNullException(nameof(promoTareaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<RechazarTareaResponseDto>> Handle(
        RechazarTareaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validacion de formato
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for RechazarTarea: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<RechazarTareaResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolver ArtistaId desde UserId
            var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                    "No tienes un perfil de artista",
                    ServiceResponseMessageType.NotFound_Artista);

            // 3. Verificar que el programa existe
            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);

            // 4. Verificar propiedad del programa
            if (programa.ArtistaId != artistaId)
                return ValidateExtensions.ForbiddenServiceResponse<RechazarTareaResponseDto>(
                    "No eres el propietario de este programa",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            // 5. Obtener el completado
            var registro = await _promoTareaService.GetTareaPromotorByIdAsync(request.TareaPromotorId, cancellationToken);
            if (registro == null)
                return ValidateExtensions.NotFoundServiceResponse<RechazarTareaResponseDto>(
                    "El completado no existe o no pertenece a este programa",
                    ServiceResponseMessageType.NotFound_PromoTareaPromotor);

            // 6. Solo se puede rechazar si esta en estado Completada (id=2)
            if (registro.EstadoTareaId != 2)
                return ValidateExtensions.BadRequestServiceResponse<RechazarTareaResponseDto>(
                    "Este completado no puede ser procesado porque ya fue validado o rechazado",
                    ServiceResponseMessageType.BusinessRule_CompletadoEstadoInvalido);

            // 7. Ejecutar rechazo via service
            await _promoTareaService.RechazarTareaAsync(registro, request.ComentarioValidacion, cancellationToken);

            // 8. Mapear entidad a DTO
            var dto = _mapper.Map<RechazarTareaResponseDto>(registro);
            dto.EstadoTareaNombre = "Rechazada";

            _logger.LogInformation(
                "TareaPromotor {TareaPromotorId} rechazada por Artista {UserId} en Programa {ProgramaId}",
                request.TareaPromotorId, request.UserId, request.ProgramaId);

            // 9. Retornar respuesta exitosa
            return new ServiceResponse<RechazarTareaResponseDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Tarea rechazada",
                            HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error rejecting TareaPromotor {TareaPromotorId} for UserId {UserId} in Programa {ProgramaId}",
                request.TareaPromotorId, request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<RechazarTareaResponseDto>(
                "Error inesperado al rechazar la tarea",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
