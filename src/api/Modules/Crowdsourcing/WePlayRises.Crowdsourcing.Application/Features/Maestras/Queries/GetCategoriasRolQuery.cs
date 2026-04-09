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
public class GetCategoriasRolQuery : IRequest<ServiceResponse<List<CategoriaRolDto>>>
{
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetCategoriasRolQueryHandler : IRequestHandler<GetCategoriasRolQuery, ServiceResponse<List<CategoriaRolDto>>>
{
    private readonly ICategoriaRolService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoriasRolQueryHandler> _logger;

    public GetCategoriasRolQueryHandler(
        ICategoriaRolService service,
        IMapper mapper,
        ILogger<GetCategoriasRolQueryHandler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<List<CategoriaRolDto>>> Handle(
        GetCategoriasRolQuery request,
        CancellationToken ct)
    {
        try
        {
            var categorias = await _service.GetAllAsync(ct);
            var dtos = _mapper.Map<List<CategoriaRolDto>>(categorias);

            return new ServiceResponse<List<CategoriaRolDto>>
            {
                Data = dtos,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Categorias obtenidas exitosamente",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorias de roles");
            return ValidateExtensions.InternalServerErrorServiceResponse<List<CategoriaRolDto>>(
                "Error inesperado al obtener categorias",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
