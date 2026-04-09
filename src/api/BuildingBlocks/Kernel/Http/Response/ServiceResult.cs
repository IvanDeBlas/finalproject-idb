using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace WePlayRises.BuildingBlocks.Kernel.Http.Response
{
    [ExcludeFromCodeCoverage]
    public class ServiceResult<T> : ContentResult
    {
        private const string ApplicationJson = "application/json";
        private readonly JsonSerializerOptions _defaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ServiceResult(ServiceResponse<T> platformResponse, HttpStatusCode statusCode)
        {
            Content = JsonSerializer.Serialize(platformResponse, _defaultOptions);
            ContentType = ApplicationJson;
            StatusCode = (int)statusCode;
        }

        public ServiceResult(ServiceResponse<T> platformResponse, HttpStatusCode statusCode, JsonSerializerOptions customJsonSerializerOptions)
        {
            Content = JsonSerializer.Serialize(platformResponse, customJsonSerializerOptions);
            ContentType = ApplicationJson;
            StatusCode = (int)statusCode;
        }
    }

    [ExcludeFromCodeCoverage]
    public class ServiceResult : ContentResult
    {
        private const string ApplicationJson = "application/json";
        private readonly JsonSerializerOptions _defaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ServiceResult(ServiceResponse platformResponse, HttpStatusCode statusCode)
        {
            Content = JsonSerializer.Serialize(platformResponse, _defaultOptions);
            ContentType = ApplicationJson;
            StatusCode = (int)statusCode;
        }

        public ServiceResult(ServiceResponse platformResponse, HttpStatusCode statusCode, JsonSerializerOptions customJsonSerializerOptions)
        {
            Content = JsonSerializer.Serialize(platformResponse, customJsonSerializerOptions);
            ContentType = ApplicationJson;
            StatusCode = (int)statusCode;
        }
    }
}
