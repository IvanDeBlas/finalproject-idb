using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Pattern
{
    [ExcludeFromCodeCoverage]
    public class ProducerData
    {
        public string Domain { get; }

        public string UserId { get; }

        public string ServiceId { get; }

        public string DynamicMetadata { get; }

        public string CorrelationId { get; }

        public DateTime? TransactionStartDate { get; }

        public ProducerData(
            string domain,
            string userId,
            string serviceId,
            string correlationId,
            string dynamicMetadata = null,
            DateTime? transactionStartDate = null
            )
        {
            Domain = domain;
            UserId = userId;
            ServiceId = serviceId;
            DynamicMetadata = dynamicMetadata;
            CorrelationId = correlationId;
            TransactionStartDate = transactionStartDate;
        }
    }
}
