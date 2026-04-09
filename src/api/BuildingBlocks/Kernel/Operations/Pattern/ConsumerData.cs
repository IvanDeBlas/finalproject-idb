using WePlayRises.BuildingBlocks.Kernel.Operations.Events;
using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Pattern
{
#pragma warning disable S107
    [ExcludeFromCodeCoverage]
    public class ConsumerData
    {
        public IdentityMetadata IdentityMetadata { get; }

        public string TraceId { get; }

        public string EventId { get; }

        public long EventNumber { get; }

        public string CorrelationId { get; }

        public DateTimeOffset Created { get; }

        public string DynamicMetadata { get; }

        public DateTime? TransactionStartDate { get; }

        public ConsumerData(
            IdentityMetadata identityMetadata,
            string traceId,
            string eventId,
            long eventNumber,
            string correlationId,
            DateTimeOffset created,
            string dynamicMetadata,
            DateTime? transactionStartDate = null
            )
        {
            IdentityMetadata = identityMetadata;
            TraceId = traceId;
            EventId = eventId;
            EventNumber = eventNumber;
            CorrelationId = correlationId;
            Created = created;
            DynamicMetadata = dynamicMetadata;
            TransactionStartDate = transactionStartDate;
        }
    }
#pragma warning restore S107
}
