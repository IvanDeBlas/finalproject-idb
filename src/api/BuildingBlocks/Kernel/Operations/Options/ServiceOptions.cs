using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Options
{
    [ExcludeFromCodeCoverage]
    public class ServiceOptions
    {
        public string DefaultServiceIdentifier { get; set; }
        public string DefaultPlatformCode { get; set; }
    }
}
