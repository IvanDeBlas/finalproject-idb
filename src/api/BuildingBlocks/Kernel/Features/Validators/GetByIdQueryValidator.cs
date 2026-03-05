//using FluentValidation;
//using WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById;
//using WePlayRises.BuildingBlocks.Kernel.Services;

//namespace WePlayRises.BuildingBlocks.Kernel.Features.Validators
//{
//    public abstract class GetByIdQueryBaseValidator<TCommand, TDto, TKey> : AbstractValidator<TCommand>
//        where TCommand : GetByIdQueryWithValidatorBase<TDto,TKey>
//        where TDto : class
//        where TKey : IEquatable<TKey>
//    {
//        public IItemExistsAsync<TKey> _service { get; }
//        public TKey? IdExist { get; private set; }

//        public GetByIdQueryBaseValidator(IItemExistsAsync<TKey> service)
//        {
//            _service = service ?? throw new ArgumentNullException(nameof(service));

//            RuleFor(r => r.Id)
//                .NotEmpty().WithMessage("Name is required")
//                .MustAsync(NotHaveDuplicateName).WithMessage(ErrorMessage);
//        }

//        private async Task<bool> NotHaveDuplicateName(TKey id, CancellationToken cancellationToken)
//        {
//            var itemExists = await _service.ItemExistsAsync(id, cancellationToken);

//            return !itemExists;
//        }

//        private string ErrorMessage(TCommand command) =>
//            $"Id: '{command.Id}' does not exist";
//    }
//}
