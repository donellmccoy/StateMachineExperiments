using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Modules.InformalLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class InformalLineOfDutyService : IInformalLineOfDutyDataService
    {
        private readonly IDbContextFactory<LodDbContext> _contextFactory;

        public InformalLineOfDutyService(IDbContextFactory<LodDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string memberId, string? memberName = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(memberId);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var entity = context.LodCases.Add(new InformalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                CurrentState = LodState.Start,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<InformalLineOfDuty?> GetCaseAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<InformalLineOfDuty>> GetAllCasesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LodCases
                .Include(c => c.TransitionHistory)
                .ToListAsync();
        }

        public async Task UpdateCaseAsync(InformalLineOfDuty lodCase)
        {
            ArgumentNullException.ThrowIfNull(lodCase);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.LodCases.Update(lodCase);
            
            await context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(StateTransitionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.TransitionHistory.Add(history);

            await context.SaveChangesAsync();
        }

        public async Task<List<StateTransitionHistory>> GetTransitionHistoryAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.TransitionHistory
                .Where(h => h.CaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}
