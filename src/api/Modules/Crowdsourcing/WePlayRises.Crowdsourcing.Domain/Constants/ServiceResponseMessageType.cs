namespace WePlayRises.Crowdsourcing.Domain.Constants;

public static class ServiceResponseMessageType
{
    // Success (0000-0999)
    public const string Success = "0000";
    public const string Created = "0001";
    public const string Updated = "0002";
    public const string Deleted = "0003";

    // Validation (1000-1999)
    public const string Validation_Required = "1001";
    public const string Validation_MaxLength = "1002";
    public const string Validation_InvalidEmail = "1003";
    public const string Validation_InvalidRange = "1009";
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_MinLength = "1011";
    public const string Validation_InvalidDate = "1012";
    public const string Validation_InvalidUrl = "1013";

    // NotFound (2000-2999)
    public const string NotFound_Entity = "2000";
    public const string NotFound_PlantillaProyecto = "2006";
    public const string NotFound_ProyectoArtistico = "2007";
    public const string NotFound_PlantillaNecesidad = "2008";
    public const string NotFound_Necesidad = "2009";
    public const string NotFound_Propuesta = "2010";
    public const string NotFound_Acuerdo = "2011";
    public const string NotFound_Milestone = "2012";
    public const string NotFound_Entregable = "2013";
    public const string NotFound_Conversacion = "2014";

    // Auth (3000-3999)
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002";

    // Business Rules (4000-4999)
    public const string BusinessRule_InvalidOperation = "4000";
    public const string BusinessRule_NecesidadNotEditable = "4001";
    public const string BusinessRule_NecesidadNotCloseable = "4002";
    public const string BusinessRule_AlreadyProposed = "4003";
    public const string BusinessRule_CannotProposeSelf = "4004";
    public const string BusinessRule_PropuestaNotRetirable = "4005";
    public const string BusinessRule_NoProfessionalProfile = "4006";
    public const string BusinessRule_PropuestaNotAcceptable = "4007";
    public const string BusinessRule_AcuerdoAlreadyExists = "4008";
    public const string BusinessRule_MilestoneImporteExceeded = "4009";
    public const string BusinessRule_AcuerdoNotActive = "4010";
    public const string BusinessRule_MilestoneCompleted = "4011";
    public const string BusinessRule_MilestoneHasEntregables = "4012";
    public const string BusinessRule_EntregableNotReviewable = "4013";
    public const string BusinessRule_InvalidState = "4014";
    public const string BusinessRule_ConversacionDuplicada = "4015";
    public const string BusinessRule_NoRelacionConDestinatario = "4016";
    public const string BusinessRule_DuplicateAction = "4017";

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
    public const string Internal_DatabaseError = "5001";
}
