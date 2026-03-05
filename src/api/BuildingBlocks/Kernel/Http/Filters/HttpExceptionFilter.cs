using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Exceptions;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Filters
{
    public class HttpExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpExceptionFilter> _logger;

        public HttpExceptionFilter(ILogger<HttpExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnException(ExceptionContext context)
        {
            var dpResponse = new ServiceResponse { Messages = GetServiceResponseMessages(context.Exception) };
            context.Result = new ServiceResult(dpResponse, GetStatusCode(context.Exception));
            if (context.HttpContext.Request.Headers.TryGetValue("X-USER-ID", out var userId))
            {
                if (userId.Any() && !string.IsNullOrWhiteSpace(userId.FirstOrDefault()))
                {
                    _logger.LogError(context.Exception, "Error while processing http request {UserId}.", userId);
                }
            }
            else
            {
                _logger.LogError(context.Exception, "Error while processing http request.");
            }
        }

        private static List<ServiceResponseMessage> GetServiceResponseMessages(Exception exception)
        {
            return exception switch
            {
                ForbiddenAccessException forbiddenException => GetForbiddenErrorMessages(forbiddenException.Secure ? null : forbiddenException.Message),
                UnauthorizedAccessException _ => GetForbiddenErrorMessages(),
                ConflictVersionResolutionException conflictVersionResolutionException => GetExceptionMessages(conflictVersionResolutionException),
                ResourceExistsException resourceAlreadyExistsException => GetExceptionMessages(resourceAlreadyExistsException),
                ResourceNotFoundException resourceNotFoundException => GetExceptionMessages(resourceNotFoundException),
                ValidationException validationException => GetExceptionMessages(validationException),
                _ => GetInternalServerErrorMessages()
            };
        }

        private static List<ServiceResponseMessage> GetForbiddenErrorMessages(string message = null)
        {
            return new List<ServiceResponseMessage>
            {
                new ServiceResponseMessage
                {
                    Message = string.IsNullOrWhiteSpace(message) ? "Access to resource is forbidden." : message,
                    HttpStatusCode = HttpStatusCode.Forbidden
                }
            };
        }

        private static List<ServiceResponseMessage> GetInternalServerErrorMessages()
        {
            return new List<ServiceResponseMessage>
            {
                new ServiceResponseMessage
                {
                    Message = "Internal server exception. Please, contact your provider.",
                    HttpStatusCode = HttpStatusCode.InternalServerError
                }
            };
        }

        private static List<ServiceResponseMessage> GetExceptionMessages(Exception platformException)
        {
            return new List<ServiceResponseMessage>
            {
                new ServiceResponseMessage
                {
                    Message = platformException.Message,
                    HttpStatusCode = GetStatusCode(platformException)
                }
            };
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException _ => HttpStatusCode.BadRequest,
                MissingParameterException _ => HttpStatusCode.BadRequest,
                ResourceNotFoundException _ => HttpStatusCode.NotFound,
                UnauthorizedAccessException _ => HttpStatusCode.Forbidden,
                ResourceExistsException _ => HttpStatusCode.Conflict,
                ConflictVersionException _ => HttpStatusCode.Conflict,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}
