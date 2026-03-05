using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.Kernel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ConflictVersionResolutionException : ConflictVersionException
    {
        public ConflictVersionResolutionException()
        { }

        public ConflictVersionResolutionException(string message)
            : base(message)
        { }

        public ConflictVersionResolutionException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ConflictVersionResolutionException(string message, long? expectedVersion, long? actualVersion) : base(message, expectedVersion, actualVersion)
        { }

        protected ConflictVersionResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}