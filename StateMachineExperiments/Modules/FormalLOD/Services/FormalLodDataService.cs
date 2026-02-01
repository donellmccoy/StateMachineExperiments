using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodDataService : IFormalLodDataService
    {
        private readonly IDbContextFactory<LodDbContext> _contextFactory;

        public FormalLodDataService(IDbContextFactory<LodDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<FormalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var entity = context.FormalLodCases.Add(new FormalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                IsDeathCase = isDeathCase,
                CurrentState = FormalLodState.Start,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<FormalLineOfDuty?> GetCaseAsync(int caseId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.FormalLodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<FormalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.FormalLodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task UpdateCaseAsync(FormalLineOfDuty lodCase)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.FormalLodCases.Update(lodCase);
            await context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(FormalStateTransitionHistory history)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.FormalTransitionHistory.Add(history);
            await context.SaveChangesAsync();
        }

        public async Task<List<FormalStateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.FormalTransitionHistory
                .Where(h => h.FormalLodCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}
