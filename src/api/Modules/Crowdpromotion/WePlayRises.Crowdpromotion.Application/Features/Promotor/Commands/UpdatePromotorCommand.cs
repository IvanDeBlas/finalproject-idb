using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;

public class UpdatePromotorCommand : IRequest<ServiceResponse<PromotorUpdatedResultDto>>
{
    public string NombrePublico { get; set; } = null!;
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
    public string? UserId { get; set; }
}

public class UpdatePromotorCommandHandler
    : IRequestHandler<UpdatePromotorCommand, ServiceResponse<PromotorUpdatedResultDto>>
{
    private readonly IPromotorService _promotorService;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePromotorCommand> _validator;
    private readonly ILogger<UpdatePromotorCommandHandler> _logger;

    public UpdatePromotorCommandHandler(
        IPromotorService promotorService,
        IMapper mapper,
        IValidator<UpdatePromotorCommand> validator,
        ILogger<UpdatePromotorCommandHandler> logger)
    {
        _promotorService = promotorService ?? throw new ArgumentNullException(nameof(promotorService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorUpdatedResultDto>> Handle(
        UpdatePromotorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdatePromotorCommand: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return new ServiceResponse<PromotorUpdatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                return ValidateExtensions.BadRequestServiceResponse<PromotorUpdatedResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);
            }

            var promotor = await _promotorService.GetByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<PromotorUpdatedResultDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            promotor.NombrePublico = request.NombrePublico;
            promotor.EmailContacto = request.EmailContacto;
            promotor.UrlSitioWeb = request.UrlSitioWeb;
            promotor.UrlInstagram = request.UrlInstagram;
            promotor.UrlTikTok = request.UrlTikTok;
            promotor.UrlYouTube = request.UrlYouTube;
            promotor.UrlTwitter = request.UrlTwitter;
            promotor.FechaActualizacion = DateTime.UtcNow;

            await _promotorService.UpdateAsync(promotor, cancellationToken);

            var dto = _mapper.Map<PromotorUpdatedResultDto>(promotor);

            _logger.LogInformation("Promotor updated for UserId {UserId}", request.UserId);

            return new ServiceResponse<PromotorUpdatedResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Perfil actualizado",
                        HttpStatusCode = System.Net.HttpStatusCode.OK
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Promotor for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorUpdatedResultDto>(
                "Error inesperado al actualizar el perfil de promotor",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
