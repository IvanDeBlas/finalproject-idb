using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.Kernel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ResourceExistsException : Exception
    {
        public ResourceExistsException()
        { }

        public ResourceExistsException(string message)
            : base(message)
        { }

        public ResourceExistsException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ResourceExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
