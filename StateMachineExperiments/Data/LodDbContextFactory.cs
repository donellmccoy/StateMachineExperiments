using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachineExperiments.Data
{
    public class LodDbContextFactory : IDesignTimeDbContextFactory<LodDbContext>
    {
        public LodDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LodDbContext>();
            optionsBuilder.UseSqlite("Data Source=lod_cases.db");

            return new LodDbContext(optionsBuilder.Options);
        }
    }
}
