using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateValoracionCommand : IRequest<ServiceResponse<ValoracionCreatedResultDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Puntuacion { get; set; }
    public string? Comentario { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateValoracionCommandHandler
    : IRequestHandler<CreateValoracionCommand, ServiceResponse<ValoracionCreatedResultDto>>
{
    private readonly IValoracionCrowdsourcingService _valoracionService;
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IArtistaService _artistaService;
    private readonly IValidator<CreateValoracionCommand> _validator;
    private readonly ILogger<CreateValoracionCommandHandler> _logger;

    public CreateValoracionCommandHandler(
        IValoracionCrowdsourcingService valoracionService,
        IAcuerdoCrowdsourcingService acuerdoService,
        IArtistaService artistaService,
        IValidator<CreateValoracionCommand> validator,
        ILogger<CreateValoracionCommandHandler> logger)
    {
        _valoracionService = valoracionService
            ?? throw new ArgumentNullException(nameof(valoracionService));
        _acuerdoService = acuerdoService
            ?? throw new ArgumentNullException(nameof(acuerdoService));
        _artistaService = artistaService
            ?? throw new ArgumentNullException(nameof(artistaService));
        _validator = validator
            ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ValoracionCreatedResultDto>> Handle(
        CreateValoracionCommand request,
        CancellationToken ct)
    {
        try
        {
            // Paso 1: Validacion de formato
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Paso 2: Obtener acuerdo
            var acuerdo = await _acuerdoService.GetByIdAsync(
                new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ValoracionCreatedResultDto>(
                    "Acuerdo no encontrado",
                    ServiceResponseMessageType.NotFound_Acuerdo);
            }

            // Paso 3: Resolver participacion
            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var esArtista = artista != null && acuerdo.ArtistaId == artista.Id;
            var esProveedor = acuerdo.UserIdProveedor == request.UserId;

            if (!esArtista && !esProveedor)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to create valoracion on acuerdo {AcuerdoId} without being a participant",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<ValoracionCreatedResultDto>(
                    "No eres participante de este acuerdo",
                    ServiceResponseMessageType.Auth_Forbidden);
            }

            // Paso 4: Verificar estado Completado
            if (acuerdo.EstadoAcuerdoId != EstadoAcuerdoConstants.Completado)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Solo se puede valorar acuerdos completados",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_InvalidState
                        }
                    }
                };
            }

            // Paso 5: Verificar unicidad (primera linea de defensa)
            var yaValoro = await _valoracionService.ExisteValoracionAsync(
                request.AcuerdoId, request.UserId, ct);
            if (yaValoro)
            {
                return new ServiceResponse<ValoracionCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Ya has dejado una valoracion para este acuerdo",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_DuplicateAction
                        }
                    }
                };
            }

            // Paso 6: Determinar UserIdValorado
            string userIdValorado;
            if (esArtista)
            {
                userIdValorado = acuerdo.UserIdProveedor;
            }
            else
            {
                var artistaDelAcuerdo = await _artistaService.GetByIdAsync(acuerdo.ArtistaId, ct);
                if (artistaDelAcuerdo == null)
                {
                    _logger.LogError(
                        "ArtistaId {ArtistaId} not found when resolving UserIdValorado for acuerdo {AcuerdoId}",
                        acuerdo.ArtistaId, request.AcuerdoId);
                    return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionCreatedResultDto>(
                        "Error al resolver el usuario valorado",
                        ServiceResponseMessageType.Internal_UnexpectedError);
                }
                userIdValorado = artistaDelAcuerdo.UserIdPropietario;
            }

            // Paso 7: Construir entidad
            var entity = new ValoracionCrowdsourcing
            {
                Id = Guid.NewGuid(),
                AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId),
                UserIdAutor = request.UserId,
                UserIdValorado = userIdValorado,
                Puntuacion = (byte)request.Puntuacion,
                Comentario = request.Comentario,
                TipoValoracionId = null,
                FechaCreacion = DateTime.UtcNow
            };

            // Paso 8: Persistir
            await _valoracionService.CreateAsync(entity, ct);

            // Paso 9: Construir DTO de resultado
            var resultDto = new ValoracionCreatedResultDto
            {
                Id = entity.Id,
                Puntuacion = (int)entity.Puntuacion,
                Comentario = entity.Comentario,
                FechaCreacion = entity.FechaCreacion
            };

            // Paso 10: Retornar ServiceResponse exitoso
            return new ServiceResponse<ValoracionCreatedResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Valoracion enviada. Gracias por tu feedback.",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (DbUpdateException dbEx)
            when (dbEx.InnerException?.Message.Contains("UQ_ValoracionCrowdsourcing_AcuerdoId_UserIdAutor") == true)
        {
            _logger.LogWarning(dbEx,
                "Constraint unicidad violado al crear valoracion (race condition). AcuerdoId={AcuerdoId}, UserId={UserId}",
                request.AcuerdoId, request.UserId);
            return new ServiceResponse<ValoracionCreatedResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Ya has dejado una valoracion para este acuerdo",
                        ErrorCode = ServiceResponseMessageType.BusinessRule_DuplicateAction
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating valoracion. AcuerdoId={AcuerdoId}, UserId={UserId}",
                request.AcuerdoId, request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionCreatedResultDto>(
                "Error inesperado al crear la valoracion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
