using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Delete
{
    public class DeleteCommandHandlerBase<TCommand, TKey> : IRequestHandler<TCommand, ServiceResponse<bool>>
      where TCommand : DeleteCommandBase<TKey>
    {
        private readonly IDeleteAsync<TKey> _service;

        public DeleteCommandHandlerBase(IDeleteAsync<TKey> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<ServiceResponse<bool>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return new ServiceResponse<bool>() { Data = await _service.DeleteAsync(request.Id, cancellationToken) };
        }
    }
}
