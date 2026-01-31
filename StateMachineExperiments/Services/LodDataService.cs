using Microsoft.EntityFrameworkCore;
using StateMachineExperiments.Data;
using StateMachineExperiments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LodDataService : ILodDataService
    {
        private readonly LodDbContext _context;

        public LodDataService(LodDbContext context)
        {
            _context = context;
        }

        public async Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null)
        {
            var entity = _context.LodCases.Add(new InformalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                CurrentState = nameof(LodState.Start),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<InformalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _context.LodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            return await _context.LodCases
                .Include(c => c.TransitionHistory)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task UpdateCaseAsync(InformalLineOfDuty lodCase)
        {
            _context.LodCases.Update(lodCase);
            await _context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(StateTransitionHistory history)
        {
            _context.TransitionHistory.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            return await _context.TransitionHistory
                .Where(h => h.LodCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }
    }
}
