using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Dtos;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Queries.GetAll
{
    public class GetAllQueryBase<T> : IRequest<ServiceResponse<ListResponse<T>>>
    {

    }
}
