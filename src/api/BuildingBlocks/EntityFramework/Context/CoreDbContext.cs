using Microsoft.EntityFrameworkCore;

namespace WePlayRises.BuildingBlocks.EntityFramework.Context
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(
           DbContextOptions options) : base(options)
        {

        }
    }
}
