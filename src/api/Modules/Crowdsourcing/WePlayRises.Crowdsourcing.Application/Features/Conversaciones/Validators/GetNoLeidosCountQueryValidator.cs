using FluentValidation;
using WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Queries;
using WePlayRises.Crowdsourcing.Domain.Constants;

namespace WePlayRises.Crowdsourcing.Application.Features.Conversaciones.Validators;

public class GetNoLeidosCountQueryValidator : AbstractValidator<GetNoLeidosCountQuery>
{
    public GetNoLeidosCountQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Usuario no identificado")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
