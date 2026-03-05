using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Application.Interfaces.Services;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Valoraciones.Queries;

// -----------------------------------------------------------------------------
// QUERY
// -----------------------------------------------------------------------------
public class GetValoracionesByUserQuery : IRequest<ServiceResponse<ValoracionesUsuarioDto>>
{
    public string UserId { get; set; } = null!;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// -----------------------------------------------------------------------------
// HANDLER
// -----------------------------------------------------------------------------
public class GetValoracionesByUserQueryHandler
    : IRequestHandler<GetValoracionesByUserQuery, ServiceResponse<ValoracionesUsuarioDto>>
{
    private readonly IValoracionCrowdsourcingService _valoracionService;
    private readonly IValidator<GetValoracionesByUserQuery> _validator;
    private readonly ILogger<GetValoracionesByUserQueryHandler> _logger;

    public GetValoracionesByUserQueryHandler(
        IValoracionCrowdsourcingService valoracionService,
        IValidator<GetValoracionesByUserQuery> validator,
        ILogger<GetValoracionesByUserQueryHandler> logger)
    {
        _valoracionService = valoracionService
            ?? throw new ArgumentNullException(nameof(valoracionService));
        _validator = validator
            ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<ValoracionesUsuarioDto>> Handle(
        GetValoracionesByUserQuery request,
        CancellationToken ct)
    {
        try
        {
            // Paso 1: Validacion de parametros
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<ValoracionesUsuarioDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            // Paso 2: Verificar que el usuario existe
            var usuarioExiste = await _valoracionService.UsuarioExisteAsync(request.UserId, ct);
            if (!usuarioExiste)
            {
                return ValidateExtensions.NotFoundServiceResponse<ValoracionesUsuarioDto>(
                    "Usuario no encontrado",
                    ServiceResponseMessageType.NotFound_Entity);
            }

            // Paso 3: Aplicar limite de pageSize
            var pageSize = Math.Min(request.PageSize, 50);

            // Paso 4: Obtener resumen estadistico
            var resumenDto = await _valoracionService.GetResumenByUserIdAsync(request.UserId, ct);

            // Paso 5: Obtener listado paginado
            var valoracionesPaginadas = await _valoracionService.GetByUserIdPagedAsync(
                request.UserId, request.Page, pageSize, ct);

            // Paso 6: Componer DTO raiz
            var responseDto = new ValoracionesUsuarioDto
            {
                Resumen = resumenDto,
                Valoraciones = valoracionesPaginadas
            };

            // Paso 7: Retornar ServiceResponse exitoso
            return new ServiceResponse<ValoracionesUsuarioDto>
            {
                Data = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error getting valoraciones for user {UserId}. Page={Page}, PageSize={PageSize}",
                request.UserId, request.Page, request.PageSize);
            return ValidateExtensions.InternalServerErrorServiceResponse<ValoracionesUsuarioDto>(
                "Error inesperado al obtener las valoraciones",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
