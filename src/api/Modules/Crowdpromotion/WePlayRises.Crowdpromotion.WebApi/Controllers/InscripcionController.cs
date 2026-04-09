using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;
using WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Queries;

namespace WePlayRises.Crowdpromotion.WebApi.Controllers;

[ApiController]
[Route("api/crowdpromotion")]
public class InscripcionController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public InscripcionController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<InscripcionController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Catalogo publico paginado de programas activos con estado de inscripcion del promotor.
    /// </summary>
    [HttpGet("programas/explorar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ExplorarProgramasResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ExplorarProgramasResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ExplorarProgramasResultDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExplorarProgramas(
        [FromQuery] string? artistaNombre,
        [FromQuery] int? tipoPromoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = new ExplorarProgramasQuery
        {
            UserId = userId.Value.ToString(),
            ArtistaNombre = artistaNombre,
            TipoPromoId = tipoPromoId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Solicitar inscripcion en un programa de promocion.
    /// </summary>
    [HttpPost("programas/{programaId}/inscripcion")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionCreadaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionCreadaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionCreadaDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionCreadaDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionCreadaDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SolicitarInscripcion(
        [FromRoute] Guid programaId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new SolicitarInscripcionCommand
        {
            ProgramaId = programaId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, result);

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Aprobar solicitud de inscripcion y generar codigo referido y URL de tracking.
    /// </summary>
    [HttpPatch("programas/{programaId}/inscripciones/{inscripcionId}/aprobar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionAprobadaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionAprobadaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionAprobadaDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionAprobadaDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AprobarInscripcion(
        [FromRoute] Guid programaId,
        [FromRoute] Guid inscripcionId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new AprobarInscripcionCommand
        {
            ProgramaId = programaId,
            InscripcionId = inscripcionId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Rechazar solicitud de inscripcion pendiente (elimina fisicamente el registro).
    /// </summary>
    [HttpPatch("programas/{programaId}/inscripciones/{inscripcionId}/rechazar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionRechazadaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionRechazadaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionRechazadaDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionRechazadaDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RechazarInscripcion(
        [FromRoute] Guid programaId,
        [FromRoute] Guid inscripcionId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new RechazarInscripcionCommand
        {
            ProgramaId = programaId,
            InscripcionId = inscripcionId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Bloquear promotor en un programa (impide re-solicitud futura).
    /// </summary>
    [HttpPatch("programas/{programaId}/inscripciones/{inscripcionId}/bloquear")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionBloqueadaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionBloqueadaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionBloqueadaDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionBloqueadaDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BloquearInscripcion(
        [FromRoute] Guid programaId,
        [FromRoute] Guid inscripcionId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new BloquearInscripcionCommand
        {
            ProgramaId = programaId,
            InscripcionId = inscripcionId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Dar de baja a un promotor aprobado (desactiva codigo referido, mantiene historico).
    /// </summary>
    [HttpPatch("programas/{programaId}/inscripciones/{inscripcionId}/dar-de-baja")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionDadaDeBajaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionDadaDeBajaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionDadaDeBajaDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionDadaDeBajaDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DarDeBajaInscripcion(
        [FromRoute] Guid programaId,
        [FromRoute] Guid inscripcionId,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new DarDeBajaInscripcionCommand
        {
            ProgramaId = programaId,
            InscripcionId = inscripcionId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Listar inscripciones de un programa con filtro por estado (vista del artista).
    /// </summary>
    [HttpGet("programas/{programaId}/inscripciones")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionListResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionListResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionListResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<InscripcionListResultDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInscripciones(
        [FromRoute] Guid programaId,
        [FromQuery] string? estado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = new GetInscripcionesProgramaQuery
        {
            ProgramaId = programaId,
            UserId = userId.Value.ToString(),
            Estado = estado,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Listar inscripciones del promotor autenticado con tareas para inscripciones aprobadas.
    /// </summary>
    [HttpGet("promotor/mis-programas")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<MisInscripcionesResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<MisInscripcionesResultDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<MisInscripcionesResultDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMisInscripciones(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = new GetMisInscripcionesQuery
        {
            UserId = userId.Value.ToString(),
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, ct);
        return FromServiceResponse(result);
    }
}
