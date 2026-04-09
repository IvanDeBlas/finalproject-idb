using MediatR;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;

namespace WePlayRises.BuildingBlocks.Kernel.Features.Commands.Create
{
    public abstract class CreateCommandBase<TKey> : IRequest<ServiceResponse<TKey>>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}
