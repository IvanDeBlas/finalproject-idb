using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Delete
{
    public class DeleteCommandBase<T> : IRequest<ServiceResponse<bool>>
    {
        public T Id { get; set; }

        public DeleteCommandBase(T id)
        {
            Id = id;
        }
    }
}
