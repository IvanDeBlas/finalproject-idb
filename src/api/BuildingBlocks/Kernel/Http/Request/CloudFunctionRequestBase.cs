using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Request
{
    [ExcludeFromCodeCoverage]
    public class CloudFunctionRequestBase
    {
        public string Domain { get; set; }

        public string UserId { get; set; }

        public string ServiceId { get; set; }

        public string TraceId { get; set; }

        public string ParentSegmentId { get; set; }

    }
}
