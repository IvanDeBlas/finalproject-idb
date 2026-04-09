using FluentValidation;
using WePlayRises.Crowdfunding.Application.Features.Campanias.Commands;
using WePlayRises.Crowdfunding.Domain.Constants;

namespace WePlayRises.Crowdfunding.Application.Features.Campanias.Validators;

public class PublishCampaniaCommandValidator : AbstractValidator<PublishCampaniaCommand>
{
    public PublishCampaniaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El Id es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);

        RuleFor(x => x.ArtistaId)
            .NotEmpty()
            .WithMessage("El ArtistaId es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
