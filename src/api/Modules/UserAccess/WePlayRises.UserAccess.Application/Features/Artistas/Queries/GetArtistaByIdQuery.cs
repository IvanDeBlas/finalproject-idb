using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.UserAccess.Application.Dtos;
using WePlayRises.UserAccess.Application.Interfaces.Services;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Artistas.Queries;

public class GetArtistaByIdQuery : IRequest<ServiceResponse<ArtistaDto>>
{
    public Guid Id { get; set; }
}

public class GetArtistaByIdQueryHandler : IRequestHandler<GetArtistaByIdQuery, ServiceResponse<ArtistaDto>>
{
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetArtistaByIdQueryHandler> _logger;

    public GetArtistaByIdQueryHandler(
        IArtistaService artistaService,
        IMapper mapper,
        ILogger<GetArtistaByIdQueryHandler> logger)
    {
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ArtistaDto>> Handle(GetArtistaByIdQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _artistaService.GetByIdAsync(new ArtistaId(request.Id), ct);

            if (entity == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<ArtistaDto>(
                    "Artista no encontrado",
                    ServiceResponseMessageType.NotFound_Artista);
            }

            var dto = _mapper.Map<ArtistaDto>(entity);

            return new ServiceResponse<ArtistaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Artista encontrado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Artista by ID {ArtistaId}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<ArtistaDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
