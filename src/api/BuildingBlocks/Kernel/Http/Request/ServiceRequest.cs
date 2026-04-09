using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Request
{
    [ExcludeFromCodeCoverage]
    public class ServiceRequest<T>
    {
        public T Data { get; set; }
    }
}
