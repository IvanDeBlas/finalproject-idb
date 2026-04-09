using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Tracking.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Tracking.Validators;

public class GetProgramaMetricasQueryValidator : AbstractValidator<GetProgramaMetricasQuery>
{
    public GetProgramaMetricasQueryValidator()
    {
        RuleFor(x => x.ProgramaId)
            .NotEmpty()
            .WithMessage("El identificador del programa es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El usuario no pudo ser identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.FechaDesde)
            .Must((query, fechaDesde) => fechaDesde == null || query.FechaHasta == null || fechaDesde <= query.FechaHasta)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha fin")
            .WithErrorCode(ServiceResponseMessageType.Validation_FechaRangoInvalido)
            .When(x => x.FechaDesde != null);
    }
}
