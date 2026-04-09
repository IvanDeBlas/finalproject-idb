using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetPlantillasProyectoQuery : IRequest<ServiceResponse<List<PlantillaProyectoListDto>>>
{
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetPlantillasProyectoQueryHandler : IRequestHandler<GetPlantillasProyectoQuery, ServiceResponse<List<PlantillaProyectoListDto>>>
{
    private readonly IPlantillaProyectoService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPlantillasProyectoQueryHandler> _logger;

    public GetPlantillasProyectoQueryHandler(
        IPlantillaProyectoService service,
        IMapper mapper,
        ILogger<GetPlantillasProyectoQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<PlantillaProyectoListDto>>> Handle(
        GetPlantillasProyectoQuery request,
        CancellationToken ct)
    {
        try
        {
            var plantillas = await _service.GetAllActivosAsync(ct);

            var plantillasWithNecesidades = new List<Domain.Model.PlantillaProyecto>();
            foreach (var plantilla in plantillas)
            {
                var plantillaCompleta = await _service.GetByIdWithNecesidadesAsync(plantilla.Id, ct);
                if (plantillaCompleta != null)
                {
                    plantillasWithNecesidades.Add(plantillaCompleta);
                }
            }

            var dtos = _mapper.Map<List<PlantillaProyectoListDto>>(plantillasWithNecesidades);

            return new ServiceResponse<List<PlantillaProyectoListDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Plantillas obtenidas exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener plantillas de proyecto");
            return ValidateExtensions.InternalServerErrorServiceResponse<List<PlantillaProyectoListDto>>(
                "Error inesperado al obtener plantillas",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
