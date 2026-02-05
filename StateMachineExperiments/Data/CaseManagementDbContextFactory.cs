using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachineExperiments.Data
{
    public class CaseManagementDbContextFactory : IDesignTimeDbContextFactory<CaseManagementDbContext>
    {
        public CaseManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CaseManagementDbContext>();
            optionsBuilder.UseInMemoryDatabase("LodCasesDb");

            return new CaseManagementDbContext(optionsBuilder.Options);
        }
    }
}
