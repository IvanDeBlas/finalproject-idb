using MediatR;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById
{
    public /*abstract*/ class GetByIdQueryBase<TDto, TKey> : IRequest<TDto>
    {
        public TKey Id { get; }

        //protected GetByIdQueryBase(TKey id)
        //{
        //    Id = id;
        //}
        public GetByIdQueryBase(TKey id)
        {
            Id = id;
        }
    }
}
