using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using System.Net;
using System.Runtime.CompilerServices;

namespace WePlayRises.BuildingBlocks.Kernel.Base.Controllers
{
    public abstract class BaseLoggerController : ControllerBase
    {
        internal readonly ILogger _logger;

        protected BaseLoggerController(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> ExecuteLoggedAsync<T>(Func<Task<T>> func, [CallerFilePath] string callerPath = "", [CallerMemberName] string caller = "")
        {
            var fileName = Path.GetFileNameWithoutExtension(callerPath);

            _logger.LogInformation($"Start ExecuteLoggedAsync --> {fileName}.{caller}");
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {caller}");
                throw;
            }
            finally
            {
                _logger.LogInformation($"End ExecuteLoggedAsync --> {fileName}.{caller}");
            }
        }

        protected async Task<IActionResult> HandleRequestAsync<T>(Func<Task<ServiceResponse<T>>> action, [CallerFilePath] string callerPath = "", [CallerMemberName] string caller = "")
        {
            var fileName = Path.GetFileNameWithoutExtension(callerPath);

            _logger.LogInformation("Start HandleRequestAsync --> {FileName}.{Caller}", fileName, caller);
            try
            {
                var serviceResponse = await action();
                return ProcessServiceResponse(serviceResponse, fileName, caller);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{FileName}.{Caller} - TaskCanceled", fileName, caller);
                return StatusCode(500, "Request cancelled.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("{FileName}.{Caller} - OperationCanceledException", fileName, caller);
                return StatusCode(500, "Request cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{FileName}.{Caller} - An error occurred.", fileName, caller);
                throw;
            }
            finally
            {
                _logger.LogInformation("End HandleRequestAsync --> {FileName}.{Caller}", fileName, caller);
            }
        }

        //protected async Task<IActionResult> HandleRequestAsync<TResponse>(Func<Task<TResponse>> action, [CallerFilePath] string callerPath = "", [CallerMemberName] string caller = "")
        //{
        //    var fileName = Path.GetFileNameWithoutExtension(callerPath);

        //    _logger.LogInformation("Start --> {FileName}.{Caller}", fileName, caller);
        //    try
        //    {
        //        var result = await action();

        //        // Si el resultado ya es un ServiceResponse, analizamos sus mensajes
        //        if (result is ServiceResponse<TResponse> serviceResponse)
        //        {
        //            return ProcessServiceResponse(serviceResponse, fileName, caller);
        //        }

        //        // Si no es ServiceResponse, envolvemos el resultado en uno nuevo
        //        return new ServiceResult<TResponse>(
        //            new ServiceResponse<TResponse> { Data = result },
        //            HttpStatusCode.OK);
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        _logger.LogWarning("{FileName}.{Caller} - TaskCanceled", fileName, caller);
        //        return StatusCode(500, "Request cancelled.");
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        _logger.LogWarning("{FileName}.{Caller} - OperationCanceledException", fileName, caller);
        //        return StatusCode(500, "Request cancelled.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "{FileName}.{Caller} - An error occurred.", fileName, caller);
        //        throw;
        //    }
        //    finally
        //    {
        //        _logger.LogInformation("End --> {FileName}.{Caller}", fileName, caller);
        //    }
        //}

        /// <summary>
        /// Converts a ServiceResponse to an appropriate IActionResult based on message severity.
        /// Use in controller actions to eliminate repetitive error-code switch blocks.
        /// </summary>
        protected IActionResult FromServiceResponse<TResponse>(ServiceResponse<TResponse> response, HttpStatusCode successCode = HttpStatusCode.OK)
        {
            if (response.IsSuccess)
            {
                return new ServiceResult<TResponse>(response, successCode);
            }

            var mostSevereStatusCode = response.Messages
                .Select(m => GetEffectiveHttpStatusCode(m))
                .OrderByDescending(code => GetSeverityOrder(code))
                .FirstOrDefault();

            return new ServiceResult<TResponse>(response, mostSevereStatusCode);
        }

        /// <summary>
        /// Procesa un ServiceResponse y determina el c�digo HTTP apropiado basado en los mensajes
        /// </summary>
        private IActionResult ProcessServiceResponse<TResponse>(ServiceResponse<TResponse> serviceResponse, string fileName, string caller)
        {
            // Si no hay mensajes, es un éxito
            if (serviceResponse.Messages.Count == 0)
            {
                return new ServiceResult<TResponse>(serviceResponse, HttpStatusCode.OK);
            }

            // Determinar el c�digo HTTP m�s severo de todos los mensajes
            var mostSevereStatusCode = serviceResponse.Messages
                .Select(m => GetEffectiveHttpStatusCode(m))
                .OrderByDescending(code => GetSeverityOrder(code))
                .First();

            // Log seg�n el tipo de c�digo HTTP
            LogMessages(serviceResponse.Messages, mostSevereStatusCode, fileName, caller);

            return new ServiceResult<TResponse>(serviceResponse, mostSevereStatusCode);
        }

        /// <summary>
        /// Resolves the effective HttpStatusCode for a message.
        /// If HttpStatusCode is explicitly set (non-OK), uses it directly.
        /// Otherwise, derives the status from the ErrorCode prefix convention:
        /// 0xxx=OK, 1xxx=BadRequest, 2xxx=NotFound, 3xxx=Unauthorized, 4xxx=Conflict, 5xxx=InternalServerError
        /// </summary>
        private static HttpStatusCode GetEffectiveHttpStatusCode(ServiceResponseMessage message)
        {
            if (message.HttpStatusCode != HttpStatusCode.OK)
                return message.HttpStatusCode;

            if (string.IsNullOrEmpty(message.ErrorCode) || message.ErrorCode.Length == 0)
                return HttpStatusCode.OK;

            return message.ErrorCode[0] switch
            {
                '0' => HttpStatusCode.OK,
                '1' => HttpStatusCode.BadRequest,
                '2' => HttpStatusCode.NotFound,
                '3' => HttpStatusCode.Unauthorized,
                '4' => HttpStatusCode.Conflict,
                '5' => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError
            };
        }

        /// <summary>
        /// Obtiene el orden de severidad de un c�digo HTTP (mayor n�mero = m�s severo)
        /// </summary>
        private static int GetSeverityOrder(HttpStatusCode statusCode)
        {
            return (int)statusCode switch
            {
                >= 500 => 500, // Server errors (m�s severo)
                >= 400 => 400, // Client errors
                >= 300 => 300, // Redirects
                >= 200 => 200, // Success (menos severo)
                _ => 100        // Otros
            };
        }

        /// <summary>
        /// Log de mensajes seg�n el c�digo HTTP
        /// </summary>
        private void LogMessages(List<ServiceResponseMessage> messages, HttpStatusCode statusCode, string fileName, string caller)
        {
            var messageText = string.Join(", ", messages.Select(m => m.Message));

            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    _logger.LogWarning("{FileName}.{Caller} - Forbidden access: {Messages}", fileName, caller, messageText);
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.UnprocessableEntity:
                    _logger.LogWarning("{FileName}.{Caller} - Client error ({StatusCode}): {Messages}", fileName, caller, (int)statusCode, messageText);
                    break;
                case >= HttpStatusCode.InternalServerError:
                    _logger.LogError("{FileName}.{Caller} - Server error ({StatusCode}): {Messages}", fileName, caller, (int)statusCode, messageText);
                    break;
                default:
                    _logger.LogInformation("{FileName}.{Caller} - Information ({StatusCode}): {Messages}", fileName, caller, (int)statusCode, messageText);
                    break;
            }
        }
    }
}
