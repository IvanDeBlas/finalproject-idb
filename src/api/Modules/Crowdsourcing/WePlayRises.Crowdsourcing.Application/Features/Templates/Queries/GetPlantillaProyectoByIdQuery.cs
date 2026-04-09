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

namespace WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetPlantillaProyectoByIdQuery : IRequest<ServiceResponse<PlantillaProyectoDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetPlantillaProyectoByIdQueryHandler : IRequestHandler<GetPlantillaProyectoByIdQuery, ServiceResponse<PlantillaProyectoDto>>
{
    private readonly IPlantillaProyectoService _service;
    private readonly IMapper _mapper;
    private readonly IValidator<GetPlantillaProyectoByIdQuery> _validator;
    private readonly ILogger<GetPlantillaProyectoByIdQueryHandler> _logger;

    public GetPlantillaProyectoByIdQueryHandler(
        IPlantillaProyectoService service,
        IMapper mapper,
        IValidator<GetPlantillaProyectoByIdQuery> validator,
        ILogger<GetPlantillaProyectoByIdQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PlantillaProyectoDto>> Handle(
        GetPlantillaProyectoByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<PlantillaProyectoDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var plantillaId = new PlantillaProyectoId(request.Id);
            var plantilla = await _service.GetByIdWithNecesidadesAsync(plantillaId, ct);

            if (plantilla == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PlantillaProyectoDto>(
                    "Plantilla no encontrada",
                    ServiceResponseMessageType.NotFound_PlantillaProyecto);
            }

            var dto = _mapper.Map<PlantillaProyectoDto>(plantilla);

            return new ServiceResponse<PlantillaProyectoDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Plantilla obtenida exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantilla {PlantillaId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<PlantillaProyectoDto>(
                "Error inesperado al obtener plantilla",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
