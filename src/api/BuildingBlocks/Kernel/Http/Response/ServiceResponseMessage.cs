using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Response
{
    [ExcludeFromCodeCoverage]
    public class ServiceResponseMessage
    {
        /// <summary>
        /// Mensaje amigable para mostrar al usuario en la UI
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Código HTTP de respuesta directo para determinar el status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// Código de error interno específico para debugging y análisis técnico
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Campo que generó el error (útil para validaciones de formularios)
        /// </summary>
        public string? PropertyName { get; set; }

        public ServiceResponseMessage() { }

        public ServiceResponseMessage(string message, HttpStatusCode httpStatusCode, string? errorCode = null, string? propertyName = null)
        {
            Message = message;
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
            PropertyName = propertyName;
        }
    }
}
