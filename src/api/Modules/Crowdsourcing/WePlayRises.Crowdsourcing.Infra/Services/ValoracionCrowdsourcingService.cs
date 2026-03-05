using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Caching.Services.Interfaces;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Repositories;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Model;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Infra.Services;

public class ValoracionCrowdsourcingService : IValoracionCrowdsourcingService
{
    private readonly IValoracionCrowdsourcingRepository _repository;
    private readonly IRequestCacheService _requestCache;
    private readonly IArtistaService _artistaService;
    private readonly IPerfilProfesionalService _perfilService;
    private readonly ILogger<ValoracionCrowdsourcingService> _logger;

    public ValoracionCrowdsourcingService(
        IValoracionCrowdsourcingRepository repository,
        IRequestCacheService requestCache,
        IArtistaService artistaService,
        IPerfilProfesionalService perfilService,
        ILogger<ValoracionCrowdsourcingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _requestCache = requestCache ?? throw new ArgumentNullException(nameof(requestCache));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _perfilService = perfilService ?? throw new ArgumentNullException(nameof(perfilService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> CreateAsync(ValoracionCrowdsourcing entity, CancellationToken ct)
    {
        var id = await _repository.AddAsync(entity, ct);
        _logger.LogInformation("Valoracion {ValoracionId} created for acuerdo {AcuerdoId} by user {UserId}",
            id, entity.AcuerdoId, entity.UserIdAutor);
        return id;
    }

    public async Task<bool> ExisteValoracionAsync(Guid acuerdoId, string userIdAutor, CancellationToken ct)
    {
        var cacheKey = $"valoracion:existe:{acuerdoId}:{userIdAutor}";
        return await _requestCache.GetOrAddAsync(
            cacheKey,
            async () => await _repository.ExisteValoracionAsync(
                new AcuerdoCrowdsourcingId(acuerdoId), userIdAutor, ct));
    }

    public async Task<bool> UsuarioExisteAsync(string userId, CancellationToken ct)
    {
        var artista = await _artistaService.GetByUserIdAsync(userId, ct);
        if (artista != null)
            return true;

        var perfil = await _perfilService.GetByUserIdAsync(userId, ct);
        return perfil != null;
    }

    public async Task<ValoracionResumenDto> GetResumenByUserIdAsync(string userId, CancellationToken ct)
    {
        var data = await _repository.GetResumenByUserIdAsync(userId, ct);
        return new ValoracionResumenDto
        {
            PuntuacionMedia = data.PuntuacionMedia,
            TotalValoraciones = data.TotalValoraciones,
            Distribucion = data.Distribucion
        };
    }

    public async Task<PaginatedResponse<ValoracionListItemDto>> GetByUserIdPagedAsync(
        string userId, int page, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await _repository.GetByUserIdPagedAsync(userId, page, pageSize, ct);

        var perfilCache = new Dictionary<string, (string Nombre, string? Imagen)>(StringComparer.OrdinalIgnoreCase);

        var dtoItems = new List<ValoracionListItemDto>(items.Count);
        foreach (var valoracion in items)
        {
            if (!perfilCache.TryGetValue(valoracion.UserIdAutor, out var perfilData))
            {
                string nombre;
                string? imagen = null;

                var artista = await _artistaService.GetByUserIdAsync(valoracion.UserIdAutor, ct);
                if (artista != null)
                {
                    nombre = artista.NombreArtistico;
                    imagen = artista.ImagenPerfilUrl;
                }
                else
                {
                    var perfil = await _perfilService.GetByUserIdAsync(valoracion.UserIdAutor, ct);
                    nombre = perfil?.Titulo ?? string.Empty;
                }

                perfilData = (nombre, imagen);
                perfilCache[valoracion.UserIdAutor] = perfilData;
            }

            dtoItems.Add(new ValoracionListItemDto
            {
                Id = valoracion.Id,
                Puntuacion = valoracion.Puntuacion,
                Comentario = valoracion.Comentario,
                AutorNombre = perfilData.Nombre,
                AutorImagenUrl = perfilData.Imagen,
                AcuerdoTituloInterno = valoracion.Acuerdo?.TituloInterno ?? string.Empty,
                FechaCreacion = valoracion.FechaCreacion
            });
        }

        return new PaginatedResponse<ValoracionListItemDto>
        {
            Items = dtoItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
