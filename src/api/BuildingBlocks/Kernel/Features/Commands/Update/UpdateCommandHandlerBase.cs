using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Update
{
    public class UpdateCommandHandlerBase<TCommand, TEntity, TKey> : IRequestHandler<TCommand, ServiceResponse<bool>>
        where TCommand : UpdateCommandBase<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IUpdateServiceAsync<TEntity, TKey> _service;

        private readonly IMapper _mapper;

        private readonly IValidator<TCommand> _validator;

        public UpdateCommandHandlerBase(IUpdateServiceAsync<TEntity, TKey> service, IMapper mapper, IValidator<TCommand> validator)
        {
            _service = service ?? throw new ArgumentNullException("service");
            _mapper = mapper ?? throw new ArgumentNullException("mapper");
            _validator = validator ?? throw new ArgumentNullException("validator");
        }

        public async Task<ServiceResponse<bool>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ServiceResponse<bool>
                {
                    Messages = validationResult.GetServiceResponseMessages()
                };
            }

            return new ServiceResponse<bool>
            {
                Data = await _service.UpdateAsync(_mapper.Map<TEntity>(request), cancellationToken)
            };
        }
    }
}