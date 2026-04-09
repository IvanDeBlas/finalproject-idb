using FluentValidation;
using WePlayRises.Crowdpromotion.Application.Features.Wallet.Queries;
using WePlayRises.Crowdpromotion.Domain.Constants;

namespace WePlayRises.Crowdpromotion.Application.Features.Wallet.Validators;

public class GetWalletTransaccionesQueryValidator : AbstractValidator<GetWalletTransaccionesQuery>
{
    public GetWalletTransaccionesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El identificador de usuario es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La pagina debe ser mayor o igual a 1")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("El tamano de pagina debe estar entre 1 y 50")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds);

        RuleFor(x => x.EstadoTransaccionId)
            .InclusiveBetween(1, 4)
            .WithMessage("El estado de transaccion debe ser entre 1 y 4")
            .WithErrorCode(ServiceResponseMessageType.Validation_RangeOutOfBounds)
            .When(x => x.EstadoTransaccionId.HasValue);

        RuleFor(x => x.FechaDesde)
            .LessThanOrEqualTo(x => x.FechaHasta!.Value)
            .WithMessage("La fecha de inicio no puede ser posterior a la fecha de fin")
            .WithErrorCode(ServiceResponseMessageType.Validation_FechaRangoInvalido)
            .When(x => x.FechaDesde.HasValue && x.FechaHasta.HasValue);
    }
}
