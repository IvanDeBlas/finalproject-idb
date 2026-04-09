using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace WePlayRises.BuildingBlocks.Kernel.Validations
{
    [ExcludeFromCodeCoverage]
    public class ValidationError
    {
        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }

        public object AttemptedValue { get; set; }

        public string ErrorCode { get; set; }

        public ValidationError()
        {
        }

        public ValidationError(ValidationFailure error)
        {
            PropertyName = error.PropertyName;
            ErrorMessage = error.ErrorMessage;
            AttemptedValue = error.AttemptedValue;
            ErrorCode = error.ErrorCode;
        }

        public ValidationError(string propertyName, string errorMessage, string attemptedValue = null, string errorCode = null)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
            ErrorCode = errorCode;
        }
    }
}