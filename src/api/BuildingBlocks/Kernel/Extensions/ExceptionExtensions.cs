namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessageException(this Exception exception) => exception == null ? string.Empty :
            $"{exception.Message} --> {exception.StackTrace}" +
            (exception.InnerException != null ? " | " : string.Empty) +
            exception.InnerException?.GetFullMessageException();
    }
}
