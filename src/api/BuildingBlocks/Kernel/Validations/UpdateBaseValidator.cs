using FluentValidation;
using WePlayRises.BuildingBlocks.Kernel.Constants;
using WePlayRises.BuildingBlocks.Kernel.Features.Commands.Update;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Validations
{
    public abstract class UpdateBaseValidator<TCommand, TKey> : AbstractValidator<TCommand>
        where TCommand : UpdateCommandBase<TKey>
        where TKey : IEquatable<TKey>
    {
        public IItemExistsAsync<TKey> _service { get; }
        public TKey? IdExist { get; private set; }

        public UpdateBaseValidator(IItemExistsAsync<TKey> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));

            RuleFor(r => r.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .WithErrorCode(ValidationErrorCodes.Validation_Required)
                .MustAsync(NotHaveDuplicateName)
                .WithMessage(ErrorMessage)
                .WithErrorCode(ValidationErrorCodes.Validation_DuplicateName);
        }

        private async Task<bool> NotHaveDuplicateName(string name, CancellationToken cancellationToken)
        {
            var (itemExists, id) = await _service.ItemExistsWithNameAsync(name, cancellationToken);
            IdExist = id;

            return !itemExists;
        }

        private string ErrorMessage(TCommand command) =>
            $"Name: '{command.Name}' is in use with id: {IdExist}.";
    }
}
