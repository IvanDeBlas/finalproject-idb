using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Queries;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.CreatePromoProgramaCommand;
using static WePlayRises.Crowdpromotion.Application.Features.PromoPrograma.Commands.UpdatePromoProgramaCommand;

namespace WePlayRises.Crowdpromotion.WebApi.Controllers;

[ApiController]
[Route("api/crowdpromotion/programas")]
public class PromoProgramaController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public PromoProgramaController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<PromoProgramaController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Creates a new promotional program with optional tasks (transactional).
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaCreatedResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePromoProgramaRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new CreatePromoProgramaCommand
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            TipoPromoId = request.TipoPromoId,
            CampaniaCrowdfundingId = request.CampaniaCrowdfundingId,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            UrlLanding = request.UrlLanding,
            CodigoTrackingBase = request.CodigoTrackingBase,
            MonedaId = request.MonedaId,
            ImporteComisionPorcentaje = request.ImporteComisionPorcentaje,
            ImporteComisionFija = request.ImporteComisionFija,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Tareas = request.Tareas?.Select(t => new CreatePromoTareaItem
            {
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                TipoEventoPromoId = t.TipoEventoPromoId,
                TipoRewardId = t.TipoRewardId,
                ImporteRecompensa = t.ImporteRecompensa,
                MonedaId = t.MonedaId,
                PuntosRecompensa = t.PuntosRecompensa,
                UrlInstrucciones = t.UrlInstrucciones,
                EsRepetible = t.EsRepetible,
                MaxRepeticiones = t.MaxRepeticiones,
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin
            }).ToList() ?? new List<CreatePromoTareaItem>(),
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Lists the authenticated artist's promotional programs with pagination.
    /// </summary>
    [HttpGet("mis-programas")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaListResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMisProgramas(
        [FromQuery] bool? esActivo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        page = Math.Max(page, 1);
        pageSize = Math.Min(Math.Max(pageSize, 1), 50);

        var query = new GetMisProgramasQuery
        {
            UserId = userId.Value.ToString(),
            EsActivo = esActivo,
            Page = page,
            PageSize = pageSize
        };

        return FromServiceResponse(await _mediator.Send(query, ct));
    }

    /// <summary>
    /// Gets the full detail of a promotional program including tasks, promoters, and summary.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDetailDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var query = new GetPromoProgramaByIdQuery
        {
            Id = id,
            UserId = userId.Value.ToString()
        };

        return FromServiceResponse(await _mediator.Send(query, ct));
    }

    /// <summary>
    /// Updates a promotional program and its tasks (transactional).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaUpdatedResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePromoProgramaRequestDto request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new UpdatePromoProgramaCommand
        {
            Id = id,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            TipoPromoId = request.TipoPromoId,
            CampaniaCrowdfundingId = request.CampaniaCrowdfundingId,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            UrlLanding = request.UrlLanding,
            CodigoTrackingBase = request.CodigoTrackingBase,
            MonedaId = request.MonedaId,
            ImporteComisionPorcentaje = request.ImporteComisionPorcentaje,
            ImporteComisionFija = request.ImporteComisionFija,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Tareas = request.Tareas?.Select(t => new UpdatePromoTareaItem
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                TipoEventoPromoId = t.TipoEventoPromoId,
                TipoRewardId = t.TipoRewardId,
                ImporteRecompensa = t.ImporteRecompensa,
                MonedaId = t.MonedaId,
                PuntosRecompensa = t.PuntosRecompensa,
                UrlInstrucciones = t.UrlInstrucciones,
                EsRepetible = t.EsRepetible,
                MaxRepeticiones = t.MaxRepeticiones,
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin,
                EsActivo = t.EsActivo
            }).ToList() ?? new List<UpdatePromoTareaItem>(),
            UserId = userId.Value.ToString()
        };

        return FromServiceResponse(await _mediator.Send(command, ct));
    }

    /// <summary>
    /// Deactivates a promotional program and all its active tasks in cascade.
    /// </summary>
    [HttpPatch("{id:guid}/desactivar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<PromoProgramaDesactivadoResultDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Desactivar([FromRoute] Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new DesactivarPromoProgramaCommand
        {
            Id = id,
            UserId = userId.Value.ToString()
        };

        return FromServiceResponse(await _mediator.Send(command, ct));
    }

    /// <summary>
    /// Gets the metrics dashboard for a promotional program (artist view).
    /// </summary>
    [HttpGet("{programaId:guid}/metricas")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResponse<ProgramaMetricasResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMetricas(
        [FromRoute] Guid programaId,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        DateOnly? parsedDesde = null;
        DateOnly? parsedHasta = null;

        if (!string.IsNullOrEmpty(fechaDesde))
        {
            if (!DateOnly.TryParseExact(fechaDesde, "yyyy-MM-dd", out var d))
                return BadRequest(new ServiceResponse<ProgramaMetricasResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaDesde invalido. Use YYYY-MM-DD", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedDesde = d;
        }

        if (!string.IsNullOrEmpty(fechaHasta))
        {
            if (!DateOnly.TryParseExact(fechaHasta, "yyyy-MM-dd", out var d))
                return BadRequest(new ServiceResponse<ProgramaMetricasResponseDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new() { Message = "Formato de fechaHasta invalido. Use YYYY-MM-DD", ErrorCode = ServiceResponseMessageType.Validation_FechaRangoInvalido }
                    }
                });
            parsedHasta = d;
        }

        var query = new GetProgramaMetricasQuery
        {
            ProgramaId = programaId,
            UserId = userId.Value.ToString(),
            FechaDesde = parsedDesde,
            FechaHasta = parsedHasta
        };

        return FromServiceResponse(await _mediator.Send(query, ct));
    }
}
