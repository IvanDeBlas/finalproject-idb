using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdfunding.Application.Dtos;
using WePlayRises.Crowdfunding.Application.Interfaces.Services;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetCampaniaDetailQuery : IRequest<ServiceResponse<CampaniaDetailDto>>
{
    public Guid Id { get; set; }
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetCampaniaDetailQueryHandler : IRequestHandler<GetCampaniaDetailQuery, ServiceResponse<CampaniaDetailDto>>
{
    private readonly ICampaniaService _campaniaService;
    private readonly IRewardService _rewardService;
    private readonly ICrowdFlagsService _crowdFlagsService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCampaniaDetailQueryHandler> _logger;

    public GetCampaniaDetailQueryHandler(
        ICampaniaService campaniaService,
        IRewardService rewardService,
        ICrowdFlagsService crowdFlagsService,
        IMapper mapper,
        ILogger<GetCampaniaDetailQueryHandler> logger)
    {
        _campaniaService = campaniaService ?? throw new ArgumentNullException(nameof(campaniaService));
        _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        _crowdFlagsService = crowdFlagsService ?? throw new ArgumentNullException(nameof(crowdFlagsService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<CampaniaDetailDto>> Handle(
        GetCampaniaDetailQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = new CampaniaCrowdfundingId(request.Id);
            var campania = await _campaniaService.GetDetailByIdAsync(id, cancellationToken);

            if (campania == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<CampaniaDetailDto>(
                    "Campania no encontrada",
                    ServiceResponseMessageType.NotFound_Campania);
            }

            // Map base data
            var dto = _mapper.Map<CampaniaDetailDto>(campania);

            // Calculate metrics
            dto.PorcentajeProgreso = campania.ImporteObjetivo > 0
                ? Math.Round((campania.ImportePledgedActual / campania.ImporteObjetivo) * 100, 1)
                : 0;
            dto.DiasRestantes = campania.FechaFin.HasValue
                ? Math.Max(0, (campania.FechaFin.Value - DateTime.UtcNow).Days)
                : 0;
            dto.MonedaSimbolo = "EUR";
            dto.EstadoCampaniaNombre = campania.EstadoCampaniaId switch
            {
                1 => "Borrador",
                2 => "Publicada",
                3 => "Finalizada",
                4 => "Cancelada",
                _ => "Desconocido"
            };

            // Map rewards with stock info
            dto.Rewards = new List<RewardPublicDto>();
            foreach (var reward in campania.Rewards.Where(r => r.EsActivo).OrderBy(r => r.Orden))
            {
                var rewardDto = _mapper.Map<RewardPublicDto>(reward);
                rewardDto.CantidadVendida = await _rewardService.GetCantidadVendidaAsync(reward.Id, cancellationToken);
                rewardDto.Disponible = reward.CantidadMaxima == null || rewardDto.CantidadVendida < reward.CantidadMaxima.Value;
                dto.Rewards.Add(rewardDto);
            }

            // Map recent backings
            dto.BackingsRecientes = new List<BackingPublicDto>();
            foreach (var pedido in campania.Pedidos.OrderByDescending(p => p.FechaCreacion).Take(10))
            {
                var backingDto = new BackingPublicDto
                {
                    Id = pedido.Id.Value,
                    Monto = pedido.ImporteTotal,
                    Mensaje = pedido.ComentarioBacker,
                    FechaCreacion = pedido.FechaCreacion,
                    NombreBacker = pedido.PermitirMostrarNombre && !string.IsNullOrEmpty(pedido.UserId)
                        ? "Backer"
                        : "Anonimo",
                    RewardNombre = pedido.Lineas.FirstOrDefault()?.Reward?.Nombre
                };
                dto.BackingsRecientes.Add(backingDto);
            }

            // Total backers
            dto.TotalBackers = await _campaniaService.GetTotalBackersAsync(id, cancellationToken);

            // Crowd flags
            if (dto.ProyectoArtisticoId.HasValue)
            {
                var flags = await _crowdFlagsService.GetFlagsForProyectosAsync(
                    new[] { dto.ProyectoArtisticoId.Value }, cancellationToken);
                if (flags.TryGetValue(dto.ProyectoArtisticoId.Value, out var f))
                {
                    dto.TieneCrowdsourcing = f.TieneCrowdsourcing;
                    dto.TieneCrowdpromotion = f.TieneCrowdpromotion;
                }
            }

            return new ServiceResponse<CampaniaDetailDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Campania encontrada",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Campania detail for Id {Id}", request.Id);
            return ValidateExtensions.InternalServerErrorServiceResponse<CampaniaDetailDto>(
                "Error inesperado",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
