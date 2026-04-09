using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Update
{
    public class UpdateCommandBase<TKey> : IRequest<ServiceResponse<bool>>, IBaseRequest
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
