namespace WePlayRises.BuildingBlocks.Kernel.Constants
{
    /// <summary>
    /// Common validation error codes used across the platform.
    /// Modules can extend these with their own specific error codes.
    /// </summary>
    public static class ValidationErrorCodes
    {
        // Validation errors (1000-1999)
        public const string Validation_Required = "1001";
        public const string Validation_MaxLength = "1002";
        public const string Validation_MinLength = "1003";
        public const string Validation_Invalid = "1004";
        public const string Validation_Duplicate = "1005";
        public const string Validation_Range = "1006";
        public const string Validation_InvalidId = "1007";
        public const string Validation_DuplicateName = "1008";
        public const string Validation_DuplicateId = "1009";

        // NotFound errors (2000-2999)
        public const string NotFound_Entity = "2001";

        // Authorization errors (3000-3999)
        public const string Authorization_Forbidden = "3001";
        public const string Authentication_Required = "3002";

        // Business Rule errors (4000-4999)
        public const string BusinessRule_EmptyList = "4001";
        public const string BusinessRule_InvalidOperation = "4002";

        // Internal errors (5000-5999)
        public const string InternalError = "5000";
        public const string InternalError_Exception = "5001";
    }
}
