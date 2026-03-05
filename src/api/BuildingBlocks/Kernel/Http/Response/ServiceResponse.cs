using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Response
{
    /// <summary>
    /// Interface for service responses to enable polymorphic access to Messages.
    /// </summary>
    public interface IServiceResponse
    {
        List<ServiceResponseMessage> Messages { get; set; }
        bool IsSuccess { get; }
        bool HasErrors { get; }
    }

    public class ServiceResponse : IServiceResponse
    {
        public string Data { get; set; }

        public List<ServiceResponseMessage> Messages { get; set; } = new();

        /// <summary>
        /// Indicates if the response is successful (no error messages).
        /// Returns false if any message has an ErrorCode or non-success HttpStatusCode.
        /// </summary>
        public bool IsSuccess =>
            Messages.Count == 0 ||
            !Messages.Any(m =>
                !string.IsNullOrEmpty(m.ErrorCode) ||
                (m.HttpStatusCode != HttpStatusCode.OK &&
                 m.HttpStatusCode != HttpStatusCode.Created));

        /// <summary>
        /// Indicates if the response has errors (inverse of IsSuccess).
        /// </summary>
        public bool HasErrors => !IsSuccess;
    }

    public class ServiceResponse<T> : IServiceResponse
    {
        public T Data { get; set; }

        public List<ServiceResponseMessage> Messages { get; set; } = new();

        /// <summary>
        /// Indicates if the response is successful (no error messages).
        /// Returns false if any message has an ErrorCode or non-success HttpStatusCode.
        /// </summary>
        public bool IsSuccess =>
            Messages.Count == 0 ||
            !Messages.Any(m =>
                !string.IsNullOrEmpty(m.ErrorCode) ||
                (m.HttpStatusCode != HttpStatusCode.OK &&
                 m.HttpStatusCode != HttpStatusCode.Created));

        /// <summary>
        /// Indicates if the response has errors (inverse of IsSuccess).
        /// </summary>
        public bool HasErrors => !IsSuccess;
    }
}
