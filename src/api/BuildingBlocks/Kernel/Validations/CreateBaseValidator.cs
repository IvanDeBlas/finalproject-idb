using FluentValidation;
using FluentValidation.Results;
using WePlayRises.BuildingBlocks.Kernel.Constants;
using WePlayRises.BuildingBlocks.Kernel.Features.Commands.Create;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Validations
{
    public abstract class CreateBaseValidator<TCommand, TKey> : AbstractValidator<TCommand>
        where TCommand : CreateCommandBase<TKey>
        where TKey : IEquatable<TKey>
    {
        public IItemExistsAsync<TKey> _service { get; }
        public TKey? IdExist { get; private set; }

        protected CreateBaseValidator(IItemExistsAsync<TKey> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));

            RuleFor(r => r.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .WithErrorCode(ValidationErrorCodes.Validation_Required)
                .MustAsync(NotHaveDuplicateName)
                .WithMessage(ErrorMessage)
                .WithErrorCode(ValidationErrorCodes.Validation_DuplicateName);

            RuleFor(x => x.Id)
                .MustAsync(NotHaveDuplicateId)
                .WithMessage("Id is already in use.")
                .WithErrorCode(ValidationErrorCodes.Validation_DuplicateId);

            if (typeof(TKey) == typeof(int))
            {
                RuleFor(x => x.Id)
                     .Custom((x, context) =>
                     {
                         if (!(int.TryParse(x.ToString(), out int value) || value <= 0))
                         {
                             context.AddFailure(new ValidationFailure(
                                 nameof(CreateCommandBase<TKey>.Id),
                                 $"{x} is not a valid number or less than 0")
                             {
                                 ErrorCode = ValidationErrorCodes.Validation_InvalidId
                             });
                         }
                     });
            }
        }

        private async Task<bool> NotHaveDuplicateId(TKey id, CancellationToken cancellationToken)
        {
            var itemExists = await _service.ItemExistsAsync(id, cancellationToken);

            return !itemExists;
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
