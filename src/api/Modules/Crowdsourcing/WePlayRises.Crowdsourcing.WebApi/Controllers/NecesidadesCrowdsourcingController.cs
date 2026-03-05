using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Necesidades.Queries;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Propuestas.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/necesidades")]
[Authorize]
public class NecesidadesCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NecesidadesCrowdsourcingController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<NecesidadesCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<NecesidadPublicaListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPublicas(
        [FromQuery] int? tipoNecesidadId,
        [FromQuery] int? modalidad,
        [FromQuery] decimal? presupuestoMin,
        [FromQuery] decimal? presupuestoMax,
        [FromQuery] string? pais,
        [FromQuery] string? search,
        [FromQuery] string? orderBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var query = new GetNecesidadesPublicasQuery
        {
            UserId = userId,
            TipoNecesidadId = tipoNecesidadId,
            ModalidadTrabajoId = modalidad,
            PresupuestoMin = presupuestoMin,
            PresupuestoMax = presupuestoMax,
            Pais = pais,
            Search = search,
            OrderBy = orderBy,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadCreateResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateNecesidadRequest request)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
            return Unauthorized();

        var command = new CreateNecesidadCommand
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            TipoNecesidadId = request.TipoNecesidadId,
            ModalidadTrabajoId = request.ModalidadTrabajoId,
            PresupuestoMin = request.PresupuestoMin,
            PresupuestoMax = request.PresupuestoMax,
            MonedaId = request.MonedaId,
            UbicacionCiudad = request.UbicacionCiudad,
            UbicacionPais = request.UbicacionPais,
            FechaLimitePropuestas = request.FechaLimitePropuestas,
            FechaInicioPrevista = request.FechaInicioPrevista,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            ArtistaId = artistaId.Value
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpGet("mis-necesidades")]
    [ProducesResponseType(typeof(ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMisNecesidades(
        [FromQuery] int? estado,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
            return Unauthorized();

        var query = new GetMisNecesidadesQuery
        {
            ArtistaId = artistaId.Value,
            EstadoNecesidadId = estado,
            Search = search,
            Page = page,
            PageSize = pageSize
        };

        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadCrowdsourcingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadPublicaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var publicaQuery = new GetNecesidadPublicaByIdQuery
        {
            Id = id,
            UserId = userId
        };

        var publicaResponse = await _mediator.Send(publicaQuery);

        if (!publicaResponse.IsSuccess)
            return FromServiceResponse(publicaResponse);

        if (publicaResponse.Data?.EsPropietario == true)
        {
            var artistaId = _currentUser.UserId;
            if (artistaId == null)
                return Unauthorized();

            var artistaQuery = new GetNecesidadByIdQuery
            {
                Id = id,
                ArtistaId = artistaId.Value
            };

            var artistaResponse = await _mediator.Send(artistaQuery);
            return FromServiceResponse(artistaResponse);
        }

        return FromServiceResponse(publicaResponse);
    }

    [HttpPost("{necesidadId}/propuestas")]
    [ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<PropuestaCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreatePropuesta(
        [FromRoute] Guid necesidadId,
        [FromBody] CreatePropuestaRequest request)
    {
        var userId = _currentUser.UserId?.ToString();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var command = new CreatePropuestaCommand
        {
            NecesidadId = necesidadId,
            UserId = userId,
            PrecioPropuesto = request.PrecioPropuesto,
            MonedaId = request.MonedaId,
            DiasEstimados = request.DiasEstimados,
            MensajePropuesta = request.MensajePropuesta
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<NecesidadUpdateResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateNecesidadRequest request)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
            return Unauthorized();

        var command = new UpdateNecesidadCommand
        {
            Id = id,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            ModalidadTrabajoId = request.ModalidadTrabajoId,
            PresupuestoMin = request.PresupuestoMin,
            PresupuestoMax = request.PresupuestoMax,
            MonedaId = request.MonedaId,
            UbicacionCiudad = request.UbicacionCiudad,
            UbicacionPais = request.UbicacionPais,
            FechaLimitePropuestas = request.FechaLimitePropuestas,
            FechaInicioPrevista = request.FechaInicioPrevista,
            ArtistaId = artistaId.Value
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }

    [HttpPatch("{id}/cerrar")]
    [ProducesResponseType(typeof(ServiceResponse<CerrarNecesidadResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Cerrar([FromRoute] Guid id, [FromBody] CerrarNecesidadRequest request)
    {
        var artistaId = _currentUser.UserId;
        if (artistaId == null)
            return Unauthorized();

        var command = new CerrarNecesidadCommand
        {
            Id = id,
            Motivo = request.Motivo,
            ArtistaId = artistaId.Value
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response);
    }
}
