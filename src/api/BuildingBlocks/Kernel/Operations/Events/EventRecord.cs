using WePlayRises.BuildingBlocks.Kernel.Operations.Pattern;
using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Events
{
    [ExcludeFromCodeCoverage]
    public sealed class EventRecord
    {
        public ProducerData EventMetadata { get; }
        public object EventData { get; }

        public EventRecord(object eventData, ProducerData eventMetadata = null)
        {
            EventData = eventData;
            EventMetadata = eventMetadata;
        }
    }

}
