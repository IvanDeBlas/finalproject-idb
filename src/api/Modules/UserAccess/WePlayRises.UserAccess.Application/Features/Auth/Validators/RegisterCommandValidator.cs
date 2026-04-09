using FluentValidation;
using WePlayRises.UserAccess.Application.Features.Auth.Commands;
using WePlayRises.UserAccess.Domain.Constants;

namespace WePlayRises.UserAccess.Application.Features.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .MinimumLength(6)
            .WithMessage("La contrasena debe tener al menos 6 caracteres")
            .WithErrorCode(ServiceResponseMessageType.Validation_MinLength);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirme su contrasena")
            .WithErrorCode(ServiceResponseMessageType.Validation_Required)
            .Equal(x => x.Password)
            .WithMessage("Las contrasenas no coinciden")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat);

        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrEmpty(role) || Roles.IsValid(role))
            .WithMessage("Rol invalido. Roles permitidos: Artista, Fan, Admin")
            .WithErrorCode(ServiceResponseMessageType.Validation_InvalidFormat)
            .When(x => !string.IsNullOrEmpty(x.Role));
    }
}
