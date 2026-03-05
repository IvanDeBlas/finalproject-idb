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

namespace WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;

public class CompletarTareaCommand : IRequest<ServiceResponse<CompletarTareaResponseDto>>
{
    public Guid ProgramaId { get; set; }
    public Guid TareaId { get; set; }
    public string UrlPruebaCompletado { get; set; } = null!;
    public string? ComentarioPromotor { get; set; }
    public string UserId { get; set; } = null!;
}

public class CompletarTareaCommandHandler
    : IRequestHandler<CompletarTareaCommand, ServiceResponse<CompletarTareaResponseDto>>
{
    private readonly IPromoTareaService _promoTareaService;
    private readonly IMapper _mapper;
    private readonly IValidator<CompletarTareaCommand> _validator;
    private readonly ILogger<CompletarTareaCommandHandler> _logger;

    public CompletarTareaCommandHandler(
        IPromoTareaService promoTareaService,
        IMapper mapper,
        IValidator<CompletarTareaCommand> validator,
        ILogger<CompletarTareaCommandHandler> logger)
    {
        _promoTareaService = promoTareaService ?? throw new ArgumentNullException(nameof(promoTareaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CompletarTareaResponseDto>> Handle(
        CompletarTareaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validacion de formato
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CompletarTarea: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<CompletarTareaResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // 2. Resolver Promotor desde UserId
            var promotor = await _promoTareaService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
                return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);

            // 3. Verificar que el programa existe
            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _promoTareaService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                    "El programa de promocion no existe",
                    ServiceResponseMessageType.NotFound_PromoPrograma);

            // 4. Verificar que el programa esta activo
            if (!programa.EsActivo)
                return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                    "El programa de promocion no esta activo",
                    ServiceResponseMessageType.BusinessRule_ProgramaInactivo);

            // 5. Verificar inscripcion aprobada y no bloqueada
            var inscripcion = await _promoTareaService.GetInscripcionAprobadaAsync(
                promotor.Id, programaId, cancellationToken);
            if (inscripcion == null || !inscripcion.EsAprobado || inscripcion.EsBloqueado)
                return ValidateExtensions.ForbiddenServiceResponse<CompletarTareaResponseDto>(
                    "No estas aprobado o tienes acceso a este programa",
                    ServiceResponseMessageType.Auth_Forbidden);

            // 6. Verificar que la tarea existe y pertenece al programa
            var tarea = await _promoTareaService.GetTareaByIdAndProgramaAsync(
                request.TareaId, programaId, cancellationToken);
            if (tarea == null)
                return ValidateExtensions.NotFoundServiceResponse<CompletarTareaResponseDto>(
                    "La tarea no existe o no pertenece a este programa",
                    ServiceResponseMessageType.NotFound_PromoTarea);

            // 7. Verificar que la tarea esta activa
            if (!tarea.EsActivo)
                return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                    "Esta tarea ya no esta disponible",
                    ServiceResponseMessageType.BusinessRule_TareaInactiva);

            // 8. Verificar fechas de vigencia
            if (tarea.FechaFin.HasValue && tarea.FechaFin.Value < DateTime.UtcNow)
                return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                    "El plazo para completar esta tarea ha finalizado",
                    ServiceResponseMessageType.BusinessRule_TareaFueraFecha);

            // 9. Verificar limites de repeticion
            var completadosActivos = await _promoTareaService.ContarCompletadosActivosAsync(
                request.TareaId, inscripcion.Id, cancellationToken);

            // 10. Tarea no repetible
            if (!tarea.EsRepetible && completadosActivos >= 1)
                return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                    "Ya completaste esta tarea. No se puede volver a completar porque no es repetible",
                    ServiceResponseMessageType.BusinessRule_TareaNoRepetible);

            // 11. Tarea repetible con limite
            if (tarea.EsRepetible && tarea.MaxRepeticiones.HasValue
                && completadosActivos >= tarea.MaxRepeticiones.Value)
                return ValidateExtensions.BadRequestServiceResponse<CompletarTareaResponseDto>(
                    "Has alcanzado el numero maximo de veces que puedes completar esta tarea",
                    ServiceResponseMessageType.BusinessRule_MaxRepeticionesAlcanzado);

            // 12. Detectar si hay registro rechazado para actualizar
            var ultimoRegistro = await _promoTareaService.GetUltimoCompletadoAsync(
                request.TareaId, inscripcion.Id, cancellationToken);
            var esActualizacion = ultimoRegistro != null && ultimoRegistro.EstadoTareaId == 4;

            // 13. Construir registro
            PromoTareaPromotor registro;
            if (esActualizacion)
            {
                registro = ultimoRegistro!;
                registro.UrlPruebaCompletado = request.UrlPruebaCompletado;
                registro.ComentarioPromotor = request.ComentarioPromotor;
                registro.EstadoTareaId = 2;
                registro.FechaCompletado = DateTime.UtcNow;
                registro.ComentarioValidacion = null;
                registro.FechaValidado = null;
            }
            else
            {
                registro = new PromoTareaPromotor
                {
                    Id = Guid.NewGuid(),
                    TareaId = request.TareaId,
                    ProgramaPromotorId = inscripcion.Id,
                    EstadoTareaId = 2,
                    UrlPruebaCompletado = request.UrlPruebaCompletado,
                    ComentarioPromotor = request.ComentarioPromotor,
                    FechaCompletado = DateTime.UtcNow,
                    FechaCreacion = DateTime.UtcNow
                };
            }

            // 14. Persistir
            var (dto, vecesCompletada) = await _promoTareaService.CompletarTareaAsync(
                registro, esActualizacion, cancellationToken);

            _logger.LogInformation(
                "Tarea {TareaId} completada por Promotor {PromotorId} en Programa {ProgramaId}. VecesCompletada: {Veces}",
                request.TareaId, promotor.Id, request.ProgramaId, vecesCompletada);

            // 15. Retornar respuesta exitosa
            return new ServiceResponse<CompletarTareaResponseDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Tarea enviada para validacion",
                            HttpStatusCode = System.Net.HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing Tarea {TareaId} for UserId {UserId} in Programa {ProgramaId}",
                request.TareaId, request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<CompletarTareaResponseDto>(
                "Error inesperado al completar la tarea",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
