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
using WePlayRises.Crowdsourcing.Application.Features.Templates.Commands;
using WePlayRises.Crowdsourcing.Application.Features.Templates.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/templates")]
[Authorize]
public class CrowdsourcingTemplatesController : BaseLoggerController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public CrowdsourcingTemplatesController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<CrowdsourcingTemplatesController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ServiceResponse<List<PlantillaProyectoListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTemplates()
    {
        var query = new GetPlantillasProyectoQuery();
        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResponse<PlantillaProyectoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTemplateById([FromRoute] Guid id)
    {
        var query = new GetPlantillaProyectoByIdQuery { Id = id };
        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpPost("{id}/generar")]
    [ProducesResponseType(typeof(ServiceResponse<GenerarNecesidadesResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerarNecesidades(
        [FromRoute] Guid id,
        [FromBody] GenerarNecesidadesRequest request)
    {
        var userId = _currentUser.UserId?.ToString();

        var command = new GenerarNecesidadesDesdeTemplateCommand
        {
            PlantillaId = id,
            ProyectoArtisticoId = request.ProyectoArtisticoId,
            ArtistaId = request.ArtistaId,
            NecesidadesSeleccionadas = request.NecesidadesSeleccionadas,
            UserId = userId
        };

        var response = await _mediator.Send(command);
        return FromServiceResponse(response, HttpStatusCode.Created);
    }
}

public class GenerarNecesidadesRequest
{
    public Guid ProyectoArtisticoId { get; set; }
    public Guid ArtistaId { get; set; }
    public List<NecesidadSeleccionadaDto> NecesidadesSeleccionadas { get; set; } = new();
}
