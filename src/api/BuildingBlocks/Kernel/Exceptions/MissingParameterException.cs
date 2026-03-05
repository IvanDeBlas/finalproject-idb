using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.Kernel.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class MissingParameterException : Exception
    {
        public MissingParameterException()
        { }

        public MissingParameterException(string message)
            : base(message)
        { }

        public MissingParameterException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected MissingParameterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
