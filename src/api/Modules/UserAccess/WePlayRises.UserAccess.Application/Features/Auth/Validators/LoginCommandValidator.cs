using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es obligatorio")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .EmailAddress()
            .WithMessage("El formato del email no es valido")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contrasena es obligatoria")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required);
    }
}
