using MediatR;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.CheckEntityExists
{
    public class CheckEntityExistsQueryBase<T> : IRequest<bool>
    {
        public T Id { get; }

        public CheckEntityExistsQueryBase(T id)
        {
            Id = id;
        }
    }
}
