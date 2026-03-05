using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Commands;
using WePlayRises.Crowdpromotion.Application.Features.PromoTarea.Queries;

namespace WePlayRises.Crowdpromotion.WebApi.Controllers;

[ApiController]
[Route("api/crowdpromotion/programas")]
public class PromoTareaController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public PromoTareaController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<PromoTareaController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    /// <summary>
    /// Listar tareas activas del programa con estado personal del promotor.
    /// </summary>
    [HttpGet("{programaId:guid}/mis-tareas")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<MisTareasResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<MisTareasResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<MisTareasResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<MisTareasResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMisTareas(
        [FromRoute] Guid programaId,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var query = new GetMisTareasQuery
        {
            ProgramaId = programaId,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(query, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Enviar completado de tarea con URL de prueba.
    /// </summary>
    [HttpPost("{programaId:guid}/tareas/{tareaId:guid}/completar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<CompletarTareaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResponse<CompletarTareaResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<CompletarTareaResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<CompletarTareaResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<CompletarTareaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompletarTarea(
        [FromRoute] Guid programaId,
        [FromRoute] Guid tareaId,
        [FromBody] CompletarTareaDto dto,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new CompletarTareaCommand
        {
            ProgramaId = programaId,
            TareaId = tareaId,
            UrlPruebaCompletado = dto.UrlPruebaCompletado,
            ComentarioPromotor = dto.ComentarioPromotor,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, result);

        return FromServiceResponse(result);
    }

    /// <summary>
    /// Listar completados pendientes de validacion (vista del artista).
    /// </summary>
    [HttpGet("{programaId:guid}/tareas-pendientes")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<TareasPendientesResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<TareasPendientesResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<TareasPendientesResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<TareasPendientesResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTareasPendientes(
        [FromRoute] Guid programaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = new GetTareasPendientesQuery
        {
            ProgramaId = programaId,
            Page = page,
            PageSize = pageSize,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(query, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Validar un completado y acreditar recompensa al promotor.
    /// </summary>
    [HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/validar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<ValidarTareaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<ValidarTareaResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<ValidarTareaResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<ValidarTareaResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<ValidarTareaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidarTarea(
        [FromRoute] Guid programaId,
        [FromRoute] Guid tareaPromotorId,
        [FromBody] ValidarTareaDto dto,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new ValidarTareaCommand
        {
            ProgramaId = programaId,
            TareaPromotorId = tareaPromotorId,
            ComentarioValidacion = dto.ComentarioValidacion,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }

    /// <summary>
    /// Rechazar un completado con motivo obligatorio.
    /// </summary>
    [HttpPatch("{programaId:guid}/tareas-promotor/{tareaPromotorId:guid}/rechazar")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResponse<RechazarTareaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarTareaResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarTareaResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarTareaResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResponse<RechazarTareaResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RechazarTarea(
        [FromRoute] Guid programaId,
        [FromRoute] Guid tareaPromotorId,
        [FromBody] RechazarTareaDto dto,
        CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            return Unauthorized();

        var command = new RechazarTareaCommand
        {
            ProgramaId = programaId,
            TareaPromotorId = tareaPromotorId,
            ComentarioValidacion = dto.ComentarioValidacion,
            UserId = userId.Value.ToString()
        };

        var result = await _mediator.Send(command, ct);
        return FromServiceResponse(result);
    }
}
