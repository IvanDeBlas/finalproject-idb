using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Base.Enums
{
    [ExcludeFromCodeCoverage]
    public static class CloudCode
    {
        public const string Unauthorized = "Unauthorized";
        public const string MissingParameter = "MissingParameter";
        public const string ValidationFailure = "ValidationFailure";
        public const string AuthorizationResourceNotFound = "AuthorizationResourceNotFound";
        public const string ResourceNotFound = "ResourceNotFound";
        public const string ResourceAlreadyExists = "ResourceAlreadyExists";
        public const string InternalError = "InternalError";
    }
}
