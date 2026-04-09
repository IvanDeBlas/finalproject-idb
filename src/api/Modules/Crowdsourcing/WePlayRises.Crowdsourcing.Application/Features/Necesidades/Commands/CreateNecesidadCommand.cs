using AutoMapper;
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

namespace WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;

// -----------------------------------------------------------------------------
// COMMAND
// -----------------------------------------------------------------------------
public class CreateNecesidadCommand : IRequest<ServiceResponse<NecesidadCreateResultDto>>
{
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int TipoNecesidadId { get; set; }
    public int ModalidadTrabajoId { get; set; }
    public decimal? PresupuestoMin { get; set; }
    public decimal? PresupuestoMax { get; set; }
    public int? MonedaId { get; set; }
    public string? UbicacionCiudad { get; set; }
    public string? UbicacionPais { get; set; }
    public DateTime? FechaLimitePropuestas { get; set; }
    public DateTime? FechaInicioPrevista { get; set; }
    public Guid ProyectoArtisticoId { get; set; }
    public Guid ArtistaId { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class CreateNecesidadCommandHandler : IRequestHandler<CreateNecesidadCommand, ServiceResponse<NecesidadCreateResultDto>>
{
    private readonly INecesidadCrowdsourcingService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateNecesidadCommand> _validator;
    private readonly ILogger<CreateNecesidadCommandHandler> _logger;

    public CreateNecesidadCommandHandler(
        INecesidadCrowdsourcingService service,
        IMapper mapper,
        IValidator<CreateNecesidadCommand> validator,
        ILogger<CreateNecesidadCommandHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<NecesidadCreateResultDto>> Handle(
        CreateNecesidadCommand request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<NecesidadCreateResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var entity = _mapper.Map<NecesidadCrowdsourcing>(request);
            entity.Id = NecesidadCrowdsourcingId.CreateNew();
            entity.ArtistaId = new ArtistaId(request.ArtistaId);
            entity.ProyectoArtisticoId = new ProyectoArtisticoId(request.ProyectoArtisticoId);
            entity.EstadoNecesidadId = 1; // Abierta
            entity.FechaCreacion = DateTime.UtcNow;

            var id = await _service.CreateAsync(entity, ct);

            var resultDto = new NecesidadCreateResultDto
            {
                Id = id.Value,
                Titulo = entity.Titulo,
                EstadoNecesidadId = entity.EstadoNecesidadId,
                EstadoNecesidadNombre = "Abierta",
                FechaCreacion = entity.FechaCreacion
            };

            return new ServiceResponse<NecesidadCreateResultDto>
            {
                Data = resultDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Necesidad publicada correctamente",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating NecesidadCrowdsourcing for Artista {ArtistaId}", request.ArtistaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<NecesidadCreateResultDto>(
                "Error inesperado al crear necesidad",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
