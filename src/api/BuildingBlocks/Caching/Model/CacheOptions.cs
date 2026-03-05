namespace WePlayRises.BuildingBlocks.Caching.Model
{
    public class CacheOptions
    {
        public const ushort DefaultMaxCacheSize = 1024;
        public const uint DefaultCacheItemTtl = 1800000;
        public const string DefaultVersionStage = "AZURE_CURRENT";

        public uint CacheItemTtl { get; set; } = DefaultCacheItemTtl;

        public ushort MaxCacheSize { get; set; } = DefaultMaxCacheSize;

        public string VersionStage { get; set; } = DefaultVersionStage;

        public ICacheHook CacheHook { get; set; } = null;
    }
}
