using System.Net;
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

namespace WePlayRises.Crowdpromotion.Application.Features.Inscripcion.Commands;

public class SolicitarInscripcionCommand : IRequest<ServiceResponse<InscripcionCreadaDto>>
{
    public Guid ProgramaId { get; set; }
    public string UserId { get; set; } = null!;
}

public class SolicitarInscripcionCommandHandler
    : IRequestHandler<SolicitarInscripcionCommand, ServiceResponse<InscripcionCreadaDto>>
{
    private readonly IInscripcionService _inscripcionService;
    private readonly IMapper _mapper;
    private readonly IValidator<SolicitarInscripcionCommand> _validator;
    private readonly ILogger<SolicitarInscripcionCommandHandler> _logger;

    public SolicitarInscripcionCommandHandler(
        IInscripcionService inscripcionService,
        IMapper mapper,
        IValidator<SolicitarInscripcionCommand> validator,
        ILogger<SolicitarInscripcionCommandHandler> logger)
    {
        _inscripcionService = inscripcionService ?? throw new ArgumentNullException(nameof(inscripcionService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<InscripcionCreadaDto>> Handle(
        SolicitarInscripcionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for SolicitarInscripcion: {Errors}",
                    string.Join(", ", validationResult.Errors));
                return new ServiceResponse<InscripcionCreadaDto>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            var promotor = await _inscripcionService.GetPromotorByUserIdAsync(request.UserId, cancellationToken);
            if (promotor == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionCreadaDto>(
                    "No tienes un perfil de promotor", ServiceResponseMessageType.NotFound_Promotor);

            if (!promotor.EsActivo)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
                    "Tu perfil de promotor esta inactivo", ServiceResponseMessageType.BusinessRule_PromotorInactivo);

            var programaId = new PromoProgramaId(request.ProgramaId);
            var programa = await _inscripcionService.GetProgramaByIdAsync(programaId, cancellationToken);
            if (programa == null)
                return ValidateExtensions.NotFoundServiceResponse<InscripcionCreadaDto>(
                    "El programa de promocion no existe", ServiceResponseMessageType.NotFound_PromoPrograma);

            if (!programa.EsActivo)
                return ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
                    "El programa de promocion no esta activo", ServiceResponseMessageType.BusinessRule_ProgramaInactivo);

            var existingInscripcion = await _inscripcionService.GetByPromotorYProgramaAsync(
                promotor.Id, programaId, cancellationToken);
            if (existingInscripcion != null)
            {
                if (existingInscripcion.EsBloqueado)
                    return ValidateExtensions.ForbiddenServiceResponse<InscripcionCreadaDto>(
                        "Estas bloqueado en este programa de promocion",
                        ServiceResponseMessageType.BusinessRule_InscripcionBloqueada);

                return ValidateExtensions.BadRequestServiceResponse<InscripcionCreadaDto>(
                    "Ya tienes una inscripcion en este programa",
                    ServiceResponseMessageType.BusinessRule_InscripcionAlreadyExists);
            }

            var inscripcion = new PromoProgramaPromotor
            {
                Id = Guid.NewGuid(),
                ProgramaId = programaId,
                PromotorId = promotor.Id,
                EsAprobado = false,
                EsBloqueado = false,
                EsActivo = true,
                FechaInscripcion = DateTime.UtcNow
            };

            var inscripcionId = await _inscripcionService.CreateAsync(inscripcion, cancellationToken);

            var dto = _mapper.Map<InscripcionCreadaDto>(inscripcion);
            dto.ProgramaTitulo = programa.Titulo;

            _logger.LogInformation(
                "Inscripcion created for PromotorId {PromotorId} in ProgramaId {ProgramaId} with Id {InscripcionId}",
                promotor.Id, programaId.Value, inscripcionId);

            return new ServiceResponse<InscripcionCreadaDto>
            {
                Data = dto,
                Messages = new List<ServiceResponseMessage>
                {
                    new() { Message = "Inscripcion solicitada exitosamente", HttpStatusCode = HttpStatusCode.Created }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Inscripcion for UserId {UserId} in ProgramaId {ProgramaId}",
                request.UserId, request.ProgramaId);
            return ValidateExtensions.InternalServerErrorServiceResponse<InscripcionCreadaDto>(
                "Error inesperado al solicitar la inscripcion",
                ServiceResponseMessageType.Internal_UnexpectedError);
        }
    }
}
