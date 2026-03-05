using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.Kernel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ConflictVersionException : Exception
    {
        public long? ExpectedVersion { get; }
        public long? ActualVersion { get; }

        public ConflictVersionException()
        { }

        public ConflictVersionException(string message)
            : base(message)
        { }

        public ConflictVersionException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ConflictVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public ConflictVersionException(string message, long? expectedVersion, long? actualVersion) : base(message)
        {
            this.ExpectedVersion = expectedVersion;
            this.ActualVersion = actualVersion;
        }
    }
}
