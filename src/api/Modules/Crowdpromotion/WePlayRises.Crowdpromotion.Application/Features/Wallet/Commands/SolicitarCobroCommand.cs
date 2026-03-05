using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.Crowdpromotion.Application.Dtos;
using WePlayRises.Crowdpromotion.Application.Interfaces.Services;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Commands;

public class SolicitarCobroCommand : IRequest<ServiceResponse<SolicitarCobroResponseDto>>
{
    public string UserId { get; set; } = null!;
    public decimal Importe { get; set; }
    public string? Descripcion { get; set; }
}

public class SolicitarCobroCommandHandler
    : IRequestHandler<SolicitarCobroCommand, ServiceResponse<SolicitarCobroResponseDto>>
{
    private readonly IPromotorWalletService _walletService;
    private readonly IValidator<SolicitarCobroCommand> _validator;
    private readonly ILogger<SolicitarCobroCommandHandler> _logger;

    public SolicitarCobroCommandHandler(
        IPromotorWalletService walletService,
        IValidator<SolicitarCobroCommand> validator,
        ILogger<SolicitarCobroCommandHandler> logger)
    {
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<SolicitarCobroResponseDto>> Handle(
        SolicitarCobroCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<SolicitarCobroResponseDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var promotor = await _walletService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
            {
                return ValidateExtensions.NotFoundServiceResponse<SolicitarCobroResponseDto>(
                    "No tienes un perfil de promotor",
                    ServiceResponseMessageType.NotFound_Promotor);
            }

            var (cobroResult, cobroError) = await _walletService.SolicitarCobroAsync(
                promotorId: promotor.Id,
                importe: request.Importe,
                descripcion: request.Descripcion,
                ct: cancellationToken);

            if (cobroError != CobroError.None)
            {
                return cobroError switch
                {
                    CobroError.WalletNoEncontrado => ValidateExtensions.NotFoundServiceResponse<SolicitarCobroResponseDto>(
                        "No tienes un wallet asignado",
                        ServiceResponseMessageType.NotFound_Wallet),

                    CobroError.SaldoBajoMinimo => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "El saldo disponible es inferior al minimo de retiro",
                        ServiceResponseMessageType.BusinessRule_SaldoBajoMinimoRetiro),

                    CobroError.SaldoInsuficiente => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "Saldo insuficiente para el importe solicitado",
                        ServiceResponseMessageType.BusinessRule_SaldoInsuficiente),

                    CobroError.CobroConcurrente => ValidateExtensions.ConflictServiceResponse<SolicitarCobroResponseDto>(
                        "Ya tienes una solicitud de cobro pendiente. Espera a que sea procesada",
                        ServiceResponseMessageType.BusinessRule_CobroConcurrente),

                    _ => ValidateExtensions.InternalServerErrorServiceResponse<SolicitarCobroResponseDto>(
                        "Error inesperado al procesar la solicitud de cobro",
                        ServiceResponseMessageType.Internal_UnexpectedError)
                };
            }

            var responseDto = new SolicitarCobroResponseDto
            {
                TransaccionId = cobroResult!.TransaccionId,
                Importe = cobroResult.Importe,
                MonedaNombre = cobroResult.MonedaNombre,
                EstadoTransaccionNombre = "Pendiente",
                SaldoRestante = cobroResult.SaldoRestante,
                FechaCreacion = cobroResult.FechaCreacion
            };

            return new ServiceResponse<SolicitarCobroResponseDto>
            {
                Data = responseDto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Solicitud de cobro registrada", HttpStatusCode = System.Net.HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error procesando solicitud de cobro para UserId {UserId}, Importe {Importe}",
                request.UserId, request.Importe);
            return ValidateExtensions.InternalServerErrorServiceResponse<SolicitarCobroResponseDto>(
                "Error inesperado al procesar la solicitud de cobro",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
