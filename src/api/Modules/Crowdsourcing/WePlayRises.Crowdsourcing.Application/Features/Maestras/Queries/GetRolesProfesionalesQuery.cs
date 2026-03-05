using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Maestras.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetRolesProfesionalesQuery : IRequest<ServiceResponse<List<RolProfesionalConCategoriaDto>>>
{
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetRolesProfesionalesQueryHandler : IRequestHandler<GetRolesProfesionalesQuery, ServiceResponse<List<RolProfesionalConCategoriaDto>>>
{
    private readonly IRolProfesionalService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRolesProfesionalesQueryHandler> _logger;

    public GetRolesProfesionalesQueryHandler(
        IRolProfesionalService service,
        IMapper mapper,
        ILogger<GetRolesProfesionalesQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<RolProfesionalConCategoriaDto>>> Handle(
        GetRolesProfesionalesQuery request,
        CancellationToken ct)
    {
        try
        {
            var roles = await _service.GetAllActivosAsync(ct);
            var dtos = _mapper.Map<List<RolProfesionalConCategoriaDto>>(roles);

            return new ServiceResponse<List<RolProfesionalConCategoriaDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Roles profesionales obtenidos exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles profesionales");
            return ValidateExtensions.InternalServerErrorServiceResponse<List<RolProfesionalConCategoriaDto>>(
                "Error inesperado al obtener roles",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
