using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.Kernel.Exceptions
{

    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ForbiddenAccessException : UnauthorizedAccessException
    {
        public bool Secure { get; set; } = true;

        public ForbiddenAccessException(bool secure = true)
        {
            Secure = secure;
        }

        public ForbiddenAccessException(string message, bool secure = true) : base(message)
        {
            Secure = secure;
        }

        public ForbiddenAccessException(string message, Exception innerException, bool secure = true) : base(message, innerException)
        {
            Secure = secure;
        }

        protected ForbiddenAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
