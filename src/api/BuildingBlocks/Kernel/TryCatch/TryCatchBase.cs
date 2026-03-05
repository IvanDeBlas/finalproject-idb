using Microsoft.Extensions.Logging;

namespace WePlayRises.BuildingBlocks.Kernel.TryCatch
{
    public abstract class TryCatchBase<TLogger, TClass, TException>
        where TLogger : ILogger<TClass>
        where TException : Exception
        where TClass : class
    {
        internal readonly TLogger _logger;

        public TryCatchBase(TLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal T TryGet<T>(Func<T> func, string logInfo, string errorMessage)
        {
            try
            {
                _logger.LogInformation(logInfo);
                return func();
            }
            catch (Exception ex)
            {
                throw SetException(errorMessage, ex);
            }
        }

        internal async Task<T> TryCatchAsync<T>(Func<Task<T>> func, string logInfo, CancellationToken cancellationToken = default) =>
            await TryCatchAsync<T>(func, logInfo, null, cancellationToken);

        internal async Task<T> TryCatchAsync<T>(Func<Task<T>> func, string logInfo, string errorMessage, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(logInfo);
                return await Task.Run(() => func(), cancellationToken);
            }
            catch (Exception ex)
            {
                throw SetException(errorMessage ??= logInfo, ex);
            }
        }



        internal abstract TException SetException(string errorMessage, Exception ex);
    }
}