using Microsoft.Extensions.Logging;
using WePlayRises.BuildingBlocks.Kernel.Extensions;
using System.Runtime.CompilerServices;

namespace WePlayRises.BuildingBlocks.Kernel.TryCatch
{
    public static class TryCatch
    {
        // Generic try-catch method to avoid code repetition.
        // This method receives a function and a logger to log any errors.
        // It also receives the callerPath, caller, and line to log the error location.
        // If no logger is provided, the error will be logged to the console.
        // The method returns the result of the function.
        public static T Run<T, TLog>(Func<T> action, ILogger<TLog> logger, string message = "", [CallerFilePath] string callerPath = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = -1)
            where TLog : class
        {
            try
            {
                logger.LogInformation($"Start --> {callerPath}.{caller} --> Line: {line}");
                return action();
            }
            catch (Exception ex)
            {
                var customMessage = "";
                if (message != null)
                    customMessage = $"--> customMessage: {message} ";

                logger.LogError($"Error --> {callerPath}.{caller} --> Line: {line} {customMessage}--> {ex.GetFullMessageException()}");

                throw;
            }
            finally
            {
                logger.LogInformation($"End --> {callerPath}.{caller} --> Line: {line}");
            }
        }

        public static async Task<T> RunAsync<T, TLog>(Func<Task<T>> func, ILogger<TLog> logger, string message = "", [CallerFilePath] string callerPath = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = -1)
           where TLog : class
        {
            try
            {
                logger.LogInformation($"Start --> {callerPath}.{caller} --> Line: {line}");
                return await func();
            }
            catch (Exception ex)
            {
                var customMessage = "";
                if (message != null)
                    customMessage = $"--> customMessage: {message} ";

                logger.LogError($"Error --> {callerPath}.{caller} --> Line: {line} {customMessage}--> {ex.GetFullMessageException()}");

                throw;
            }
            finally
            {
                logger.LogInformation($"End --> {callerPath}.{caller} --> Line: {line}");
            }
        }
    }
}
