using Microsoft.AspNetCore.Builder;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    public static class EnvironmentExtensions
    {
        public static string GetEnvironment(this WebApplicationBuilder builder)
        {
            return builder.Environment.EnvironmentName;
        }
    }
}
