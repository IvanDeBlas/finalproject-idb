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

public class ValidarTareaCommand : IRequest<ServiceResponse<ValidarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaPromotorId { get; set; }
    public string? ComentarioValidacion { get; set; }
    public string UserId { get; set; } = null!;
}

public class ValidarTareaCommandHandler
    : IRequestHandler<ValidarTareaCommand, ServiceResponse<ValidarTareaResponseDto>>
{
    private readonly IPromoTareaService _promoTareaService;
    private readonly IValidator<ValidarTareaCommand> _validator;
    private readonly ILogger<ValidarTareaCommandHandler> _logger;

    public ValidarTareaCommandHandler(
        IPromoTareaService promoTareaService,
        IValidator<ValidarTareaCommand> validator,
        ILogger<ValidarTareaCommandHandler> logger)
    {
        _promoTareaService = promoTareaService ?? throw new ArgumentNullException(nameof(promoTareaService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ValidarTareaResponseDto>> Handle(
        ValidarTareaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validacion de formato
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for ValidarTarea: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<ValidarTareaResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolver ArtistaId desde UserId
            var artistaId = await _promoTareaService.GetArtistaIdByUserIdAsync(request.UserId, cancellationToken);
            if (artistaId == null)
                return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                    "No tienes un perfil de artista",
                    ServiceResponseMessageType.NotFound_Artista);

            // 3. Verificar que el programa existe
            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);

            // 4. Verificar propiedad del programa
            if (programa.ArtistaId != artistaId)
                return ValidateExtensions.ForbiddenServiceResponse<ValidarTareaResponseDto>(
                    "No eres el propietario de este programa",
                    ServiceResponseMessageType.BusinessRule_NoEsPropietarioPrograma);

            // 5. Obtener el completado con navegaciones
            var registro = await _promoTareaService.GetTareaPromotorByIdAsync(request.TareaPromotorId, cancellationToken);
            if (registro == null)
                return ValidateExtensions.NotFoundServiceResponse<ValidarTareaResponseDto>(
                    "El completado no existe o no pertenece a este programa",
                    ServiceResponseMessageType.NotFound_PromoTareaPromotor);

            // 6. Solo se puede validar si esta en estado Completada (id=2)
            if (registro.EstadoTareaId != 2)
                return ValidateExtensions.BadRequestServiceResponse<ValidarTareaResponseDto>(
                    "Este completado no puede ser procesado porque ya fue validado o rechazado",
                    ServiceResponseMessageType.BusinessRule_CompletadoEstadoInvalido);

            // 7. Transaccion atomica via service
            var dto = await _promoTareaService.ValidarTareaAsync(
                registro, programaId, request.ComentarioValidacion, cancellationToken);

            _logger.LogInformation(
                "TareaPromotor {TareaPromotorId} validada por Artista {UserId} en Programa {ProgramaId}",
                request.TareaPromotorId, request.UserId, request.ProgramaId);

            // 8. Retornar respuesta exitosa
            return new ServiceResponse<ValidarTareaResponseDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Tarea validada y recompensa acreditada",
                            HttpStatusCode = System.Net.HttpStatusCode.OK }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error validating TareaPromotor {TareaPromotorId} for UserId {UserId} in Programa {ProgramaId}",
                request.TareaPromotorId, request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ValidarTareaResponseDto>(
                "Error inesperado al validar la tarea",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
