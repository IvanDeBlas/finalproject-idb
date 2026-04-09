using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.EntityFramework.StronglyTypedIds;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;
using WePlayRises.Crowdpromotion.Domain.Model;

namespace WePlayRises.Crowdpromotion.Application.Features.Promotor.Commands;

public class CreatePromotorCommand : IRequest<ServiceResponse<PromotorCreatedResultDto>>
{
    public string NombrePublico { get; set; } = null!;
    public int TipoPromotorId { get; set; }
    public string? EmailContacto { get; set; }
    public string? UrlSitioWeb { get; set; }
    public string? UrlInstagram { get; set; }
    public string? UrlTikTok { get; set; }
    public string? UrlYouTube { get; set; }
    public string? UrlTwitter { get; set; }
    public string? UserId { get; set; }
}

public class CreatePromotorCommandHandler
    : IRequestHandler<CreatePromotorCommand, ServiceResponse<PromotorCreatedResultDto>>
{
    private readonly IPromotorService _promotorService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePromotorCommand> _validator;
    private readonly ILogger<CreatePromotorCommandHandler> _logger;

    private static readonly Dictionary<int, string> TipoPromotorNombres = new()
    {
        { 1, "Fan Embajador" },
        { 2, "Influencer" },
        { 3, "Medio / Blog" },
        { 4, "Profesional Marketing" }
    };

    public CreatePromotorCommandHandler(
        IPromotorService promotorService,
        IMapper mapper,
        IValidator<CreatePromotorCommand> validator,
        ILogger<CreatePromotorCommandHandler> logger)
    {
        _promotorService = promotorService ?? throw new ArgumentNullException(nameof(promotorService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<PromotorCreatedResultDto>> Handle(
        CreatePromotorCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreatePromotorCommand: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return new ServiceResponse<PromotorCreatedResultDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                return ValidateExtensions.BadRequestServiceResponse<PromotorCreatedResultDto>(
                    "Token invalido", ServiceResponseMessageType.Auth_Unauthorized);
            }

            var existente = await _promotorService.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existente != null)
            {
                _logger.LogWarning("Promotor already exists for UserId {UserId}", request.UserId);
                return new ServiceResponse<PromotorCreatedResultDto>
                {
                    Messages = new List<ServiceResponseMessage>
                    {
                        new()
                        {
                            Message = "Ya tienes un perfil de promotor creado",
                            ErrorCode = ServiceResponseMessageType.BusinessRule_PromotorAlreadyExists
                        }
                    }
                };
            }

            var promotor = _mapper.Map<Domain.Model.Promotor>(request);
            promotor.Id = PromotorId.CreateNew();
            promotor.UserId = request.UserId;
            promotor.EsActivo = true;
            promotor.FechaCreacion = DateTime.UtcNow;

            var promotorId = await _promotorService.CreateWithWalletAsync(promotor, cancellationToken);

            var creado = await _promotorService.GetByIdAsync(promotorId, cancellationToken);
            var dto = _mapper.Map<PromotorCreatedResultDto>(creado);

            if (TipoPromotorNombres.TryGetValue(request.TipoPromotorId, out var tipoNombre))
            {
                dto.TipoPromotorNombre = tipoNombre;
            }

            _logger.LogInformation(
                "Promotor created for UserId {UserId} with PromotorId {PromotorId}",
                request.UserId, promotorId.Value);

            return new ServiceResponse<PromotorCreatedResultDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new()
                    {
                        Message = "Perfil de promotor creado",
                        HttpStatusCode = System.Net.HttpStatusCode.Created
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Promotor for UserId {UserId}", request.UserId);
            return ValidateExtensions.InternalServerErrorServiceResponse<PromotorCreatedResultDto>(
                "Error inesperado al crear el perfil de promotor",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
