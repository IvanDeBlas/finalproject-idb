using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateConversacionCommand : IRequest<ServiceResponse<CreateConversacionResultDto>>
{
    public Guid? NecesidadId { get; set; }
    public Guid? AcuerdoId { get; set; }
    public string UserIdDestinatario { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string UserIdCreador { get; set; } = string.Empty;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateConversacionCommandHandler : IRequestHandler<CreateConversacionCommand, ServiceResponse<CreateConversacionResultDto>>
{
    private readonly IConversacionCrowdsourcingService _conversacionService;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly IValidator<CreateConversacionCommand> _validator;
    private readonly ILogger<CreateConversacionCommandHandler> _logger;

    public CreateConversacionCommandHandler(
        IConversacionCrowdsourcingService conversacionService,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        IValidator<CreateConversacionCommand> validator,
        ILogger<CreateConversacionCommandHandler> logger)
    {
        _conversacionService = conversacionService ?? throw new ArgumentNullException(nameof(conversacionService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CreateConversacionResultDto>> Handle(
        CreateConversacionCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<CreateConversacionResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Check uniqueness
            var existe = await _conversacionService.ExisteConversacionParaContextoAsync(
                request.UserIdCreador, request.UserIdDestinatario,
                request.NecesidadId, request.AcuerdoId, ct);
            if (existe)
            {
                return new ServiceResponse<CreateConversacionResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Ya existe una conversacion para este contexto entre las mismas partes",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_ConversacionDuplicada
                        }
                    }
                };
            }

            // Build entity
            var entity = new ConversacionCrowdsourcing
            {
                Id = Guid.NewGuid(),
                UserIdCreador = request.UserIdCreador,
                UserIdDestinatario = request.UserIdDestinatario,
                Asunto = request.Asunto,
                FechaCreacion = DateTime.UtcNow,
                FechaUltimoMensaje = null
            };

            if (request.NecesidadId.HasValue)
                entity.NecesidadId = new NecesidadCrowdsourcingId(request.NecesidadId.Value);
            if (request.AcuerdoId.HasValue)
                entity.AcuerdoId = new AcuerdoCrowdsourcingId(request.AcuerdoId.Value);

            var id = await _conversacionService.CreateAsync(entity, ct);

            // Resolve NombreDestinatario
            string nombreDestinatario;
            var artista = await _artistaService.GetByUserIdAsync(request.UserIdDestinatario, ct);
            if (artista != null)
                nombreDestinatario = artista.NombreArtistico;
            else
            {
                var perfil = await _perfilService.GetByUserIdAsync(request.UserIdDestinatario, ct);
                nombreDestinatario = perfil?.Titulo ?? string.Empty;
            }

            // Resolve context
            string contextoTipo = request.NecesidadId.HasValue ? "necesidad" : "acuerdo";
            string contextoTitulo = string.Empty;
            // Context title comes from navigation loaded by GetById after create
            var saved = await _conversacionService.GetByIdAsync(id, ct);
            if (saved != null)
            {
                contextoTitulo = request.NecesidadId.HasValue
                    ? saved.Necesidad?.Titulo ?? string.Empty
                    : saved.Acuerdo?.Descripcion ?? string.Empty;
            }

            var resultDto = new CreateConversacionResultDto
            {
                Id = id,
                Asunto = entity.Asunto,
                NombreDestinatario = nombreDestinatario,
                ContextoTipo = contextoTipo,
                ContextoTitulo = contextoTitulo,
                FechaCreacion = entity.FechaCreacion
            };

            return new ServiceResponse<CreateConversacionResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Conversacion creada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            when (dbEx.InnerException?.Message.Contains("UX_ConversacionCrowdsourcing") == true)
        {
            _logger.LogWarning(dbEx, "Constraint unicidad violado al crear conversacion (race condition)");
            return new ServiceResponse<CreateConversacionResultDto>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Ya existe una conversacion para este contexto entre las mismas partes",
                        ErrorCode = ServiceResponseMessageType.BusinessRule_ConversacionDuplicada
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversacion. UserIdCreador={UserIdCreador}, UserIdDestinatario={UserIdDestinatario}",
                request.UserIdCreador, request.UserIdDestinatario);
            return ValidateExtensions.InternalServerErrorServiceResponse<CreateConversacionResultDto>(
                "Error inesperado al crear la conversacion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
