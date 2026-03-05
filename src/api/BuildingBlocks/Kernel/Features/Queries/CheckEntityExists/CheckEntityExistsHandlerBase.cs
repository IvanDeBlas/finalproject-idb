using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Services;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.CheckEntityExists
{
    public class CheckEntityExistsHandlerBase<TQuery, TKey> : IRequestHandler<TQuery, bool>
        where TQuery : CheckEntityExistsQueryBase<TKey>
    {
        private readonly IItemExistsAsync<TKey> _service;

        public CheckEntityExistsHandlerBase(IItemExistsAsync<TKey> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<bool> Handle(TQuery request, CancellationToken cancellationToken)
        {
            return await _service.ItemExistsAsync(request.Id, cancellationToken);
        }
    }
}
