using WePlayRises.BuildingBlocks.Kernel.Base.Enums;
using WePlayRises.BuildingBlocks.Kernel.Operations.Events;
using WePlayRises.BuildingBlocks.Kernel.Operations.Pattern;
using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Base.Aggregates
{
    [ExcludeFromCodeCoverage]
    public class AggregateRoot : IAggregateRoot
    {
        private readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();
        private readonly Dictionary<Type, Action<object, ProducerData>> _handlersWithMetadata = new Dictionary<Type, Action<object, ProducerData>>();

        readonly List<EventRecord> _events = new List<EventRecord>();

        public Guid Id { get; set; }

        public string Domain { get; set; }

        public int Version { get; set; } = AggregateVersion.Default;

        public Type? GetEventType(string eventName)
        {
            return _handlers.Keys.FirstOrDefault(x => x.Name == eventName) ?? _handlersWithMetadata.Keys.FirstOrDefault(x => x.Name == eventName);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void Register<T>(Action<T> when)
        {
            _handlers.Add(typeof(T), e => when((T)e));
        }

        protected void Register<T>(Action<T, ProducerData> when)
        {
            _handlersWithMetadata.Add(typeof(T), (e, metadata) => when((T)e, metadata));
        }

        public void Apply(object e, ProducerData metadata = null)
        {
            if (_handlers.ContainsKey(e.GetType()))
            {
                Raise(e);
            }
            else if (_handlersWithMetadata.ContainsKey(e.GetType()))
            {
                Raise(e, metadata);
            }
            else
            {
                throw new ArgumentException($"Handler not found for event {e.GetType()}");
            }
            Version++;
        }

        protected void Raise(object e)
        {
            _events.Add(new EventRecord(e, null));
            _handlers[e.GetType()](e);
        }

        protected void Raise(object e, ProducerData data)
        {
            _events.Add(new EventRecord(e, data));
            _handlersWithMetadata[e.GetType()](e, data);
        }

        public List<object> GetEvents()
        {
            return _events.Select(x => x.EventData).ToList();
        }

        public List<EventRecord> GetEventRecords()
        {
            return _events;
        }
    }
}
