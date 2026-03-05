using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class ValidateExtensions
    {
        public static List<ServiceResponseMessage> GetServiceResponseMessages(this FluentValidation.Results.ValidationResult validate)
        {
            return validate.Errors.Select(x => new ServiceResponseMessage()
            {
                HttpStatusCode = x.GetHttpStatusCode(),
                Message = x.ErrorMessage,
                ErrorCode = x.ErrorCode,
                PropertyName = x.PropertyName
            }).ToList();
        }

        public static List<ServiceResponseMessage> GetServiceResponseMessages(this IEnumerable<string> messages, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            return messages.Select(p => new ServiceResponseMessage()
            {
                HttpStatusCode = httpStatusCode,
                Message = p
            }).ToList();
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status NotFound, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> NotFoundServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.NotFound.ToServiceResponse<T>(message, errorCode);
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status BadRequest, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> BadRequestServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.BadRequest.ToServiceResponse<T>(message, errorCode);
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status Conflict, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> ConflictServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.Conflict.ToServiceResponse<T>(message, errorCode);
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status Forbidden, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> ForbiddenServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.Forbidden.ToServiceResponse<T>(message, errorCode);
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status Unauthorized, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> UnauthorizedServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.Unauthorized.ToServiceResponse<T>(message, errorCode);
        }

        /// <summary>
        /// Crea un ServiceResponse<T> con status InternalServerError, mensaje y código de error.
        /// </summary>
        public static ServiceResponse<T> InternalServerErrorServiceResponse<T>(string message, string? errorCode = null)
        {
            return HttpStatusCode.InternalServerError.ToServiceResponse<T>(message, errorCode);
        }



        /// <summary>
        /// Crea un ServiceResponse<T> con un mensaje, código de error y status HTTP.
        /// </summary>
        private static ServiceResponse<T> ToServiceResponse<T>(this HttpStatusCode httpStatusCode, string message, string? errorCode = null)
        {
            return new ServiceResponse<T>
            {
                Messages = new List<ServiceResponseMessage>
                {
                    new ServiceResponseMessage
                    {
                        Message = message,
                        ErrorCode = errorCode,
                        HttpStatusCode = httpStatusCode
                    }
                }
            };
        }
    }
}
