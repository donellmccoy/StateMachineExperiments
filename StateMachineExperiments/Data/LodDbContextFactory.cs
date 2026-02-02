using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachineExperiments.Data
{
    public class LodDbContextFactory : IDesignTimeDbContextFactory<LodDbContext>
    {
        public LodDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LodDbContext>();
            optionsBuilder.UseInMemoryDatabase("LodCasesDb");

            return new LodDbContext(optionsBuilder.Options);
        }
    }
}
