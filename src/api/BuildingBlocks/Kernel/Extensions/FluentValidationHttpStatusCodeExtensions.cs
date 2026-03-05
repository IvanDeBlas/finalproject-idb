using FluentValidation;
using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    /// <summary>
    /// Extensiones de FluentValidation para asignar códigos HTTP de respuesta directamente
    /// </summary>
    public static class FluentValidationHttpStatusCodeExtensions
    {
        private const string HttpStatusCodeKey = "HttpStatusCode";

        /// <summary>
        /// Asigna un código HTTP específico a una regla de validación
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que se está validando</typeparam>
        /// <typeparam name="TProperty">Tipo de la propiedad que se está validando</typeparam>
        /// <param name="rule">La regla de validación</param>
        /// <param name="httpStatusCode">El código HTTP a asignar</param>
        /// <returns>La regla de validación con el código HTTP asignado</returns>
        public static IRuleBuilderOptions<T, TProperty> WithHttpStatusCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            HttpStatusCode httpStatusCode)
        {
            return rule.WithState(_ => new { HttpStatusCode = httpStatusCode });
        }

        /// <summary>
        /// Obtiene el código HTTP asignado a un error de validación
        /// </summary>
        /// <param name="validationFailure">El error de validación</param>
        /// <returns>El código HTTP asignado o BadRequest por defecto</returns>
        public static HttpStatusCode GetHttpStatusCode(this FluentValidation.Results.ValidationFailure validationFailure)
        {
            if (validationFailure.CustomState != null)
            {
                var stateType = validationFailure.CustomState.GetType();
                var httpStatusCodeProperty = stateType.GetProperty("HttpStatusCode");

                if (httpStatusCodeProperty != null && httpStatusCodeProperty.GetValue(validationFailure.CustomState) is HttpStatusCode statusCode)
                {
                    return statusCode;
                }
            }

            // Por defecto, los errores de validación son BadRequest
            return HttpStatusCode.BadRequest;
        }
    }
}
