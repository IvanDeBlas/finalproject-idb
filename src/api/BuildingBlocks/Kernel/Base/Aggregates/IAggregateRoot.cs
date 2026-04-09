using WePlayRises.BuildingBlocks.Kernel.Operations.Events;
using WePlayRises.BuildingBlocks.Kernel.Operations.Pattern;

namespace WePlayRises.BuildingBlocks.Kernel.Base.Aggregates
{
    public interface IAggregateRoot
    {
        public Type? GetEventType(string eventName);

        List<object> GetEvents();

        List<EventRecord> GetEventRecords();

        void ClearEvents();

        void Apply(object e, ProducerData data = null);

        Guid Id { get; set; }

        string Domain { get; set; }

        int Version { get; set; }
    }
}
