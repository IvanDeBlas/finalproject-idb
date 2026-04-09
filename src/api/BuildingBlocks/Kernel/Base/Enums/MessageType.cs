using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Base.Enums
{
    [ExcludeFromCodeCoverage]
    public static class MessageType
    {
        public const string Info = "Info";
        public const string Error = "Error";
        public const string Warning = "Warning";
        public const string Forbidden = "Forbidden";
    }
}
