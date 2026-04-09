using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class EventMetadata
    {
        public IdentityMetadata IdentityMetadata { get; set; }

        public string TraceId { get; set; }

        public string TraceParentSegmentId { get; set; }

        public bool? IsTraceSampled { get; set; }

        public string EventId { get; set; }

        public long EventNumber { get; set; } = -1;

        public string CorrelationId { get; set; }

        public DateTimeOffset Created { get; set; }

        public string DynamicMetadata { get; set; }

        public bool IsProtected { get; set; }

        public DateTime? TransactionStartDate { get; set; }
    }
}
