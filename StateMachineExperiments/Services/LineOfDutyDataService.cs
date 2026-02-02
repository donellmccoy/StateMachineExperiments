using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Data;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LineOfDutyDataService : ILineOfDutyDataService
    {
        private readonly IDbContextFactory<LodDbContext> _contextFactory;

        public LineOfDutyDataService(IDbContextFactory<LodDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<LineOfDuty> CreateNewCaseAsync(LodType caseType, string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var entity = context.LineOfDutyCases.Add(new LineOfDuty
            {
                CaseType = caseType,
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                IsDeathCase = isDeathCase,
                CurrentState = LodState.Start,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<LineOfDuty?> GetCaseAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<LineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<LineOfDuty>> GetAllCasesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .ToListAsync();
        }

        public async Task<IEnumerable<LineOfDuty>> GetCasesByTypeAsync(LodType caseType)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .Where(c => c.CaseType == caseType)
                .ToListAsync();
        }

        public async Task UpdateCaseAsync(LineOfDuty lodCase)
        {
            ArgumentNullException.ThrowIfNull(lodCase);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.LineOfDutyCases.Update(lodCase);
            
            await context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(LodStateTransitionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.TransitionHistory.Add(history);

            await context.SaveChangesAsync();
        }

        public async Task<List<LodStateTransitionHistory>> GetTransitionHistoryAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.TransitionHistory
                .Where(h => h.LineOfDutyCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}
