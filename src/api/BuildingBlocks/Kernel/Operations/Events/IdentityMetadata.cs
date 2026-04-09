using System.Diagnostics.CodeAnalysis;

namespace WePlayRises.BuildingBlocks.Kernel.Operations.Events
{
    [ExcludeFromCodeCoverage]
    public class IdentityMetadata
    {
        public string Domain { get; set; }

        public string UserId { get; set; }

        public string ServiceId { get; set; }

        public IdentityMetadata()
        {
        }

        public IdentityMetadata(string domain, string userId, string serviceId)
        {
            Domain = domain;
            UserId = userId;
            ServiceId = serviceId;
        }
    }
}