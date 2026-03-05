using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Base.Controllers;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Features.Maestras.Queries;

namespace WePlayRises.Crowdsourcing.WebApi.Controllers;

[ApiController]
[Route("api/crowdsourcing/maestras")]
[Authorize]
public class MaestrasCrowdsourcingController : BaseLoggerController
{
    private readonly IMediator _mediator;

    public MaestrasCrowdsourcingController(
        IMediator mediator,
        ILogger<MaestrasCrowdsourcingController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("roles-profesionales")]
    [ProducesResponseType(typeof(ServiceResponse<List<RolProfesionalConCategoriaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRolesProfesionales()
    {
        var query = new GetRolesProfesionalesQuery();
        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }

    [HttpGet("categorias-rol")]
    [ProducesResponseType(typeof(ServiceResponse<List<CategoriaRolDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategoriasRol()
    {
        var query = new GetCategoriasRolQuery();
        var response = await _mediator.Send(query);
        return FromServiceResponse(response);
    }
}
