using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Create
{
    public class CreateCommandHandlerBase<TCommand, TEntity, TKey> : IRequestHandler<TCommand, ServiceResponse<TKey>>
         where TCommand : CreateCommandBase<TKey>
         where TKey : IEquatable<TKey>
    {
        private readonly ICreateServiceAsync<TEntity, TKey> _service;

        private readonly IMapper _mapper;

        private readonly IValidator<TCommand> _validator;

        public CreateCommandHandlerBase(ICreateServiceAsync<TEntity, TKey> service, IMapper mapper, IValidator<TCommand> validator)
        {
            _service = service ?? throw new ArgumentNullException("service");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _validator = validator ?? throw new ArgumentNullException("validator");
        }

        public async Task<ServiceResponse<TKey>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<TKey>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            return new ServiceResponse<TKey>
            {
                Data = await _service.CreateAsync(_mapper.Map<TEntity>(request), cancellationToken)
            };
        }
    }
}