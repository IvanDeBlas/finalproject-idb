using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetById
{
    public /*abstract*/ class GetByIdQueryServiceResponseBase<TDto, TKey> : IRequest<ServiceResponse<TDto>>
    {
        public TKey Id { get; }

        //protected GetByIdQueryBase(TKey id)
        //{
        //    Id = id;
        //}
        public GetByIdQueryServiceResponseBase(TKey id)
        {
            Id = id;
        }
    }
}
