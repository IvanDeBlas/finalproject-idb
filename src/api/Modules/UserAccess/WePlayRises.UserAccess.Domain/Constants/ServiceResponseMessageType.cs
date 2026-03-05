namespace WePlayRises.UserAccess.Domain.Constants;

/// <summary>
/// Constantes para codigos de error en ServiceResponse.
/// Usar SIEMPRE estas constantes en .WithErrorCode() de FluentValidation y en Handlers.
/// </summary>
public static class ServiceResponseMessageType
{
    // =========================================================================
    // SUCCESS (0000-0999)
    // =========================================================================
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // =========================================================================
    // VALIDATION ERRORS (1000-1999)
    // =========================================================================
    public const string Validation_Required = "1001";
    public const string Validation_MaxLength = "1002";
    public const string Validation_MinLength = "1003";
    public const string Validation_InvalidFormat = "1004";
    public const string Validation_InvalidEmail = "1005";
    public const string Validation_InvalidUrl = "1006";
    public const string Validation_InvalidRange = "1007";
    public const string Validation_DuplicateName = "1008";
    public const string Validation_DuplicateEmail = "1009";
    public const string Validation_ForeignKeyNotFound = "1010";

    // =========================================================================
    // NOT FOUND ERRORS (2000-2999)
    // =========================================================================
    public const string NotFound_Entity = "2000";
    public const string NotFound_User = "2001";
    public const string NotFound_Artista = "2002";
    public const string NotFound_Campania = "2003";
    public const string NotFound_Reward = "2004";
    public const string NotFound_Backing = "2005";

    // =========================================================================
    // AUTHENTICATION/AUTHORIZATION ERRORS (3000-3999)
    // =========================================================================
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002";
    public const string Auth_TokenExpired = "3003";
    public const string Auth_InvalidToken = "3004";
    public const string Auth_UserNotAuthenticated = "3005";
    public const string Auth_InvalidCredentials = "3006";
    public const string Auth_UserNotFound = "3007";
    public const string Auth_AccountLocked = "3008";

    // =========================================================================
    // BUSINESS RULE ERRORS (4000-4999)
    // =========================================================================
    public const string BusinessRule_DuplicateRecord = "4001";
    public const string BusinessRule_InvalidState = "4002";
    public const string BusinessRule_OperationNotAllowed = "4003";
    public const string BusinessRule_LimitExceeded = "4004";
    public const string BusinessRule_InsufficientFunds = "4005";
    public const string BusinessRule_CampaniaNotActive = "4006";
    public const string BusinessRule_CampaniaEnded = "4007";
    public const string BusinessRule_ArtistaAlreadyExists = "4008";

    // =========================================================================
    // INTERNAL ERRORS (5000-5999)
    // =========================================================================
    public const string Internal_UnexpectedError = "5000";
    public const string Internal_DatabaseError = "5001";
    public const string Internal_ExternalServiceError = "5002";
    public const string Internal_ConfigurationError = "5003";
}
