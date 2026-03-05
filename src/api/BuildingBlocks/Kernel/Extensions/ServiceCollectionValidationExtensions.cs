using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using WePlayRises.BuildingBlocks.Kernel.Http.Response;
using WePlayRises.BuildingBlocks.Kernel.Validations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class ServiceCollectionValidationExtensions
    {
        [ExcludeFromCodeCoverage]
        public static IServiceCollection RegisterCloudValidationResponse(this IServiceCollection services)
        {
            return services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = CloudInvalidModelStateResponseFactory;
            });
        }

        public static IActionResult CloudInvalidModelStateResponseFactory(ActionContext actionContext)
        {
            ServiceResponse<List<ValidationError>> serviceResponse;
            if (actionContext.HttpContext.Items.ContainsKey(ConstantsValidations.ValidationResultKey)
                && actionContext.HttpContext.Items[ConstantsValidations.ValidationResultKey] is ValidationResult validationResult)
            {
                serviceResponse = GetFromFluent(validationResult);
            }
            else
            {
                // Fallback option - get the validation errors from AspNetCore ModelState
                serviceResponse = GetFromAspNetCore(actionContext.ModelState);
            }

            return new BadRequestObjectResult(serviceResponse);
        }

        private static ServiceResponse<List<ValidationError>> GetFromFluent(ValidationResult validationResult)
        {
            var errors = validationResult?.Errors ?? new List<ValidationFailure>();
            return new ServiceResponse<List<ValidationError>>
            {
                Data = errors.Select(x => new ValidationError(x)).ToList(),
                Messages = errors.Select(x => new ServiceResponseMessage
                {
                    Message = x.ErrorMessage,
                    HttpStatusCode = HttpStatusCode.BadRequest
                }).ToList()
            };
        }

        private static ServiceResponse<List<ValidationError>> GetFromAspNetCore(ModelStateDictionary modelState)
        {
            var invalidProperties = modelState?.Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                .ToList() ?? new List<KeyValuePair<string, ModelStateEntry>>();

            return new ServiceResponse<List<ValidationError>>
            {
                Data = invalidProperties.Select(x => new ValidationError(x.Key, x.Value.Errors.FirstOrDefault()?.ErrorMessage))
                    .ToList(),
                Messages = invalidProperties.Select(x => new ServiceResponseMessage
                {
                    Message = x.Value.Errors.FirstOrDefault()?.ErrorMessage,
                    HttpStatusCode = HttpStatusCode.BadRequest
                }).ToList()
            };
        }
    }
}
