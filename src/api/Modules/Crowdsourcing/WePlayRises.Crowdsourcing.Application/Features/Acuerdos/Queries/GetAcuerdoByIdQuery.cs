using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;
using WePlayRises.UserAccess.Application.Interfaces.Services;

namespace WePlayRises.Crowdsourcing.Application.Features.Acuerdos.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetAcuerdoByIdQuery : IRequest<ServiceResponse<AcuerdoDto>>
{
    public Guid AcuerdoId { get; set; }
    public string UserId { get; set; } = null!;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetAcuerdoByIdQueryHandler : IRequestHandler<GetAcuerdoByIdQuery, ServiceResponse<AcuerdoDto>>
{
    private readonly IAcuerdoCrowdsourcingService _acuerdoService;
    private readonly IArtistaService _artistaService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAcuerdoByIdQueryHandler> _logger;

    public GetAcuerdoByIdQueryHandler(
        IAcuerdoCrowdsourcingService acuerdoService,
        IArtistaService artistaService,
        IMapper mapper,
        ILogger<GetAcuerdoByIdQueryHandler> logger)
    {
        _acuerdoService = acuerdoService ?? throw new ArgumentNullException(nameof(acuerdoService));
        _artistaService = artistaService ?? throw new ArgumentNullException(nameof(artistaService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<AcuerdoDto>> Handle(
        GetAcuerdoByIdQuery request,
        CancellationToken ct)
    {
        try
        {
            var acuerdo = await _acuerdoService.GetByIdWithDetailsAsync(
                new AcuerdoCrowdsourcingId(request.AcuerdoId), ct);
            if (acuerdo == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<AcuerdoDto>(
                    "Acuerdo no encontrado", ServiceResponseMessageType.NotFound_Acuerdo);
            }

            var artista = await _artistaService.GetByUserIdAsync(request.UserId, ct);
            var esArtista = artista != null && acuerdo.ArtistaId == artista.Id;
            var esProfesional = acuerdo.UserIdProveedor == request.UserId;

            if (!esArtista && !esProfesional)
            {
                _logger.LogWarning("User {UserId} attempted to view acuerdo {AcuerdoId} without being a participant",
                    request.UserId, request.AcuerdoId);
                return ValidateExtensions.ForbiddenServiceResponse<AcuerdoDto>(
                    "No tienes acceso a este acuerdo", ServiceResponseMessageType.Auth_Forbidden);
            }

            var miRol = esArtista ? "Artista" : "Profesional";

            var importeAsignado = acuerdo.Milestones?.Sum(m => m.ImporteParcial) ?? 0;
            var porcentajeAsignado = acuerdo.ImporteTotalPactado > 0
                ? Math.Round((importeAsignado / acuerdo.ImporteTotalPactado) * 100, 2)
                : 0;

            var dto = _mapper.Map<AcuerdoDto>(acuerdo);

            dto.MiRol = miRol;
            dto.ImporteAsignado = importeAsignado;
            dto.PorcentajeAsignado = porcentajeAsignado;

            dto.EstadoAcuerdoNombre = acuerdo.EstadoAcuerdoId switch
            {
                EstadoAcuerdoConstants.Activo => "Activo",
                EstadoAcuerdoConstants.Completado => "Completado",
                EstadoAcuerdoConstants.Cancelado => "Cancelado",
                _ => "Desconocido"
            };

            dto.Artista = new AcuerdoArtistaDto
            {
                Id = acuerdo.ArtistaId.Value,
                NombreArtistico = artista?.NombreArtistico ?? string.Empty
            };

            dto.Profesional = new AcuerdoProfesionalDto
            {
                UserId = acuerdo.UserIdProveedor,
                PerfilProfesionalId = acuerdo.PerfilProfesionalId?.Value,
                Nombre = string.Empty
            };

            if (acuerdo.Necesidad != null)
            {
                dto.Necesidad = new AcuerdoNecesidadDto
                {
                    Id = acuerdo.Necesidad.Id.Value,
                    Titulo = acuerdo.Necesidad.Titulo
                };
            }

            if (dto.Milestones != null)
            {
                foreach (var milestone in dto.Milestones)
                {
                    milestone.PorcentajeParcial = acuerdo.ImporteTotalPactado > 0
                        ? Math.Round((milestone.ImporteParcial / acuerdo.ImporteTotalPactado) * 100, 2)
                        : 0;
                }
            }

            dto.Timeline = BuildTimeline(acuerdo, artista?.NombreArtistico ?? "Artista");

            return new ServiceResponse<AcuerdoDto>
            {
                Data = dto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting acuerdo {AcuerdoId}", request.AcuerdoId);
            return ValidateExtensions.InternalServerErrorServiceResponse<AcuerdoDto>(
                "Error inesperado al obtener acuerdo", ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }

    private static List<AcuerdoTimelineEventoDto> BuildTimeline(
        Domain.Model.AcuerdoCrowdsourcing acuerdo,
        string nombreArtista)
    {
        var eventos = new List<AcuerdoTimelineEventoDto>();

        eventos.Add(new AcuerdoTimelineEventoDto
        {
            Accion = "Acuerdo creado",
            Fecha = acuerdo.FechaCreacion,
            Actor = nombreArtista
        });

        if (acuerdo.Milestones != null)
        {
            foreach (var milestone in acuerdo.Milestones.OrderBy(m => m.FechaCreacion))
            {
                eventos.Add(new AcuerdoTimelineEventoDto
                {
                    Accion = $"Milestone agregado: {milestone.Titulo}",
                    Fecha = milestone.FechaCreacion,
                    Actor = nombreArtista
                });

                if (milestone.FechaCompletado != null)
                {
                    eventos.Add(new AcuerdoTimelineEventoDto
                    {
                        Accion = $"Milestone completado: {milestone.Titulo}",
                        Fecha = milestone.FechaCompletado.Value,
                        Actor = nombreArtista
                    });
                }

                if (milestone.Entregables != null)
                {
                    foreach (var entregable in milestone.Entregables.OrderBy(e => e.FechaCreacion))
                    {
                        AddEntregableEvents(eventos, entregable, nombreArtista);
                    }
                }
            }
        }

        if (acuerdo.FechaFinReal != null)
        {
            var accion = acuerdo.EstadoAcuerdoId == EstadoAcuerdoConstants.Completado
                ? "Acuerdo completado"
                : "Acuerdo cancelado";
            eventos.Add(new AcuerdoTimelineEventoDto
            {
                Accion = accion,
                Fecha = acuerdo.FechaFinReal.Value,
                Actor = acuerdo.CanceladoPor ?? nombreArtista
            });
        }

        return eventos.OrderByDescending(e => e.Fecha).Take(20).ToList();
    }

    private static void AddEntregableEvents(
        List<AcuerdoTimelineEventoDto> eventos,
        Domain.Model.AcuerdoCrowdsourcingEntregable entregable,
        string nombreArtista)
    {
        eventos.Add(new AcuerdoTimelineEventoDto
        {
            Accion = $"Entregable subido: {entregable.Titulo}",
            Fecha = entregable.FechaCreacion,
            Actor = "Profesional"
        });

        if (entregable.FechaAprobacion != null)
        {
            eventos.Add(new AcuerdoTimelineEventoDto
            {
                Accion = $"Entregable aprobado: {entregable.Titulo}",
                Fecha = entregable.FechaAprobacion.Value,
                Actor = nombreArtista
            });
        }

        if (entregable.FechaActualizacion != null &&
            entregable.EstadoEntregableId == EstadoEntregableConstants.Rechazado)
        {
            eventos.Add(new AcuerdoTimelineEventoDto
            {
                Accion = $"Entregable rechazado: {entregable.Titulo}",
                Fecha = entregable.FechaActualizacion.Value,
                Actor = nombreArtista
            });
        }
    }
}
