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
        private readonly IDbContextFactory<CaseManagementDbContext> _contextFactory;

        public LineOfDutyDataService(IDbContextFactory<CaseManagementDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<LineOfDutyCase> CreateNewCaseAsync(LineOfDutyType caseType, string caseNumber, int memberId, bool isDeathCase = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var entity = context.LineOfDutyCases.Add(new LineOfDutyCase
            {
                LineOfDutyType = caseType,
                CaseNumber = caseNumber,
                MemberId = memberId,
                IsDeathCase = isDeathCase,
                LineOfDutyState = LineOfDutyState.Start,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                CreatedByUserId = 1, // TODO: Get from current user context
                LastModifiedByUserId = 1 // TODO: Get from current user context
            });

            await context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<LineOfDutyCase?> GetCaseAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .Include(c => c.Member)
                .FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<LineOfDutyCase?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .Include(c => c.Member)
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task<IEnumerable<LineOfDutyCase>> GetAllCasesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .Include(c => c.Member)
                .ToListAsync();
        }

        public async Task<IEnumerable<LineOfDutyCase>> GetCasesByTypeAsync(LineOfDutyType caseType)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.LineOfDutyCases
                .Include(c => c.TransitionHistory)
                .Include(c => c.Member)
                .Where(c => c.LineOfDutyType == caseType)
                .ToListAsync();
        }

        public async Task UpdateCaseAsync(LineOfDutyCase lodCase)
        {
            ArgumentNullException.ThrowIfNull(lodCase);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.LineOfDutyCases.Update(lodCase);
            
            await context.SaveChangesAsync();
        }

        public async Task AddTransitionHistoryAsync(LineOfDutyStateTransitionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.TransitionHistory.Add(history);

            await context.SaveChangesAsync();
        }

        public async Task<List<LineOfDutyStateTransitionHistory>> GetTransitionHistoryAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.TransitionHistory
                .Where(h => h.LineOfDutyCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task<Member> CreateMemberAsync(string cardId, string name, string? rank = null, string? unit = null, string? email = null, string? phone = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardId);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            await using var context = await _contextFactory.CreateDbContextAsync();
            
            var entity = context.Members.Add(new Member
            {
                CardId = cardId,
                Name = name,
                Rank = rank,
                Email = email,
                Phone = phone,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                IsActive = true
            });

            await context.SaveChangesAsync();

            return entity.Entity;
        }

        public async Task<Member?> GetMemberAsync(int memberId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.Members
                .Include(m => m.LineOfDutyCases)
                .FirstOrDefaultAsync(m => m.Id == memberId);
        }

        public async Task<Member?> GetMemberByCardIdAsync(string cardId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardId);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.Members
                .Include(m => m.LineOfDutyCases)
                .FirstOrDefaultAsync(m => m.CardId == cardId);
        }

        public async Task<IEnumerable<Member>> GetAllMembersAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            return await context.Members
                .Include(m => m.LineOfDutyCases)
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            ArgumentNullException.ThrowIfNull(member);
            
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            member.LastModifiedDate = DateTime.UtcNow;
            context.Members.Update(member);
            
            await context.SaveChangesAsync();
        }
    }
}
