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
        private readonly LodDbContext _context;

        public FormalLodDataService(LodDbContext context)
        {
            _context = context;
        }

        public async Task<FormalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false)
        {
            var entity = _context.FormalLodCases.Add(new FormalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                IsDeathCase = isDeathCase,
                CurrentState = nameof(FormalLodState.Start),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<FormalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _context.FormalLodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<FormalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            return await _context.FormalLodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task UpdateCaseAsync(FormalLineOfDuty lodCase)
        {
            _context.FormalLodCases.Update(lodCase);
            await _context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(FormalStateTransitionHistory history)
        {
            _context.FormalTransitionHistory.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FormalStateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            return await _context.FormalTransitionHistory
                .Where(h => h.FormalLodCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}
