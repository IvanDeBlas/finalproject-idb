namespace WePlayRises.Crowdpromotion.Domain.Constants;

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
    public const string Validation_ForeignKeyNotFound = "1010";
    public const string Validation_MinLength = "1011";
    public const string Validation_InvalidUrl = "1013";
    public const string Validation_AtLeastOneComision = "1020";
    public const string Validation_RangeOutOfBounds = "1021";
    public const string Validation_DateFinBeforeInicio = "1022";
    public const string Validation_InvalidCodigoTracking = "1023";
    public const string Validation_DuplicateCodigoTracking = "1024";
    public const string Validation_EsRepetibleRequiresMax = "1025";
    public const string Validation_ImporteRecompensaRequired = "1027";
    public const string Validation_PuntosRecompensaRequired = "1028";
    public const string Validation_TareaConCompletados = "1029";

    // Tracking - US-CP-05
    public const string Validation_TipoEventoInvalido = "1033";
    public const string Validation_BackingNoPermitido = "1034";
    public const string Validation_ValorMonetarioInvalido = "1035";
    public const string Validation_FechaRangoInvalido = "1036";

    // NotFound (2000-2999)
    public const string NotFound_Entity = "2000";
    public const string NotFound_Promotor = "2015";
    public const string NotFound_Artista = "2016";
    public const string NotFound_CampaniaCrowdfunding = "2017";
    public const string NotFound_ProyectoArtistico = "2018";
    public const string NotFound_PromoPrograma = "2019";
    public const string NotFound_Inscripcion = "2020";
    public const string NotFound_PromoTarea = "2021";
    public const string NotFound_PromoTareaPromotor = "2022";

    // Auth (3000-3999)
    public const string Auth_Unauthorized = "3001";
    public const string Auth_Forbidden = "3002";
    public const string Auth_InvalidToken = "3004";

    // Business Rules (4000-4999)
    public const string BusinessRule_PromotorAlreadyExists = "4018";
    public const string BusinessRule_PromotorAlreadyInactive = "4019";
    public const string BusinessRule_PromoProgramaAlreadyInactive = "4020";
    public const string BusinessRule_InscripcionAlreadyExists = "4021";
    public const string BusinessRule_InscripcionBloqueada = "4022";
    public const string BusinessRule_PromotorInactivo = "4023";
    public const string BusinessRule_ProgramaInactivo = "4024";
    public const string BusinessRule_InscripcionEstadoInvalido = "4025";
    public const string BusinessRule_NoEsPropietarioPrograma = "4026";
    public const string BusinessRule_TareaNoRepetible = "4027";
    public const string BusinessRule_MaxRepeticionesAlcanzado = "4028";
    public const string BusinessRule_TareaInactiva = "4029";
    public const string BusinessRule_TareaFueraFecha = "4030";
    public const string BusinessRule_CompletadoEstadoInvalido = "4031";

    // Tracking Rate Limiting - US-CP-05
    public const string BusinessRule_RateLimitExcedido = "4032";

    // Wallet - US-CP-06 (NotFound 2030-2039)
    public const string NotFound_Wallet = "2030";

    // Wallet - US-CP-06 (Business Rules 4040-4049)
    public const string BusinessRule_SaldoInsuficiente = "4040";
    public const string BusinessRule_SaldoBajoMinimoRetiro = "4041";
    public const string BusinessRule_CobroConcurrente = "4042";

    // Internal (5000-5999)
    public const string Internal_UnexpectedError = "5000";
}
