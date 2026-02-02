using Stateless;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Models;
using System;
using System.Threading.Tasks;

namespace StateMachineExperiments.Factories
{
    /// <summary>
    /// Unified factory for creating state machines for both Informal and Formal Line of Duty cases.
    /// Uses the CaseType discriminator to determine which workflow to configure.
    /// </summary>
    public class LodStateMachineFactory : ILodStateMachineFactory
    {
        public StateMachine<LodState, LodTrigger> CreateStateMachine(LineOfDuty lodCase)
        {
            return lodCase.CaseType == LodType.Informal 
                ? CreateInformalStateMachine(lodCase) 
                : CreateFormalStateMachine(lodCase);
        }

        #region Informal LOD State Machine

        private static StateMachine<LodState, LodTrigger> CreateInformalStateMachine(LineOfDuty lodCase)
        {
            var stateMachine = new StateMachine<LodState, LodTrigger>(lodCase.CurrentState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Start, lodCase));

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.MemberReports, lodCase))
                .Permit(LodTrigger.ConditionReported, LodState.LodInitiation)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.MemberReports, lodCase));

            // Configure LodInitiation state
            stateMachine.Configure(LodState.LodInitiation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.LodInitiation, lodCase))
                .Permit(LodTrigger.InitiationComplete, LodState.MedicalAssessment)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.LodInitiation, lodCase));

            // Configure MedicalAssessment state
            stateMachine.Configure(LodState.MedicalAssessment)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.MedicalAssessment, lodCase))
                .Permit(LodTrigger.AssessmentDone, LodState.CommanderReview)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.MedicalAssessment, lodCase));

            // Configure CommanderReview state - dynamic routing based on case requirements
            stateMachine.Configure(LodState.CommanderReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.CommanderReview, lodCase))
                .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, () => lodCase.RequiresLegalReview)
                .Permit(LodTrigger.SkipToAdjudication, LodState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.CommanderReview, lodCase));

            // Configure OptionalLegal state
            stateMachine.Configure(LodState.OptionalLegal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.OptionalLegal, lodCase))
                .PermitIf(LodTrigger.LegalDone, LodState.OptionalWing, () => lodCase.RequiresWingReview)
                .Permit(LodTrigger.SkipWingReview, LodState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.OptionalLegal, lodCase));

            // Configure OptionalWing state
            stateMachine.Configure(LodState.OptionalWing)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.OptionalWing, lodCase))
                .Permit(LodTrigger.WingDone, LodState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.OptionalWing, lodCase));

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.BoardAdjudication, lodCase))
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.BoardAdjudication, lodCase));

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Determination, lodCase))
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Determination, lodCase));

            // Configure Notification state
            stateMachine.Configure(LodState.Notification)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Notification, lodCase))
                .Permit(LodTrigger.AppealFiled, LodState.Appeal)
                .Permit(LodTrigger.NotificationComplete, LodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Notification, lodCase));

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Appeal, lodCase))
                .Permit(LodTrigger.AppealResolved, LodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Appeal, lodCase));

            // Configure End state
            stateMachine.Configure(LodState.End)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.End, lodCase));

            return stateMachine;

            static async Task OnStateEntryAsync(LodState state, LineOfDuty lodCase)
            {
                Console.WriteLine($"[INFORMAL] [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }

            static async Task OnStateExitAsync(LodState state, LineOfDuty lodCase)
            {
                Console.WriteLine($"[INFORMAL] [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }
        }

        #endregion

        #region Formal LOD State Machine

        private static StateMachine<LodState, LodTrigger> CreateFormalStateMachine(LineOfDuty lodCase)
        {
            var stateMachine = new StateMachine<LodState, LodTrigger>(lodCase.CurrentState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Start, lodCase));

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.MemberReports, lodCase))
                .Permit(LodTrigger.ConditionReported, LodState.FormalInitiation)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.MemberReports, lodCase));

            // Configure FormalInitiation state
            stateMachine.Configure(LodState.FormalInitiation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.FormalInitiation, lodCase))
                .Permit(LodTrigger.QuestionableDetected, LodState.AppointingOfficer)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.FormalInitiation, lodCase));

            // Configure AppointingOfficer state
            stateMachine.Configure(LodState.AppointingOfficer)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.AppointingOfficer, lodCase))
                .Permit(LodTrigger.OfficerAppointed, LodState.Investigation)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.AppointingOfficer, lodCase));

            // Configure Investigation state
            stateMachine.Configure(LodState.Investigation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Investigation, lodCase))
                .Permit(LodTrigger.InvestigationComplete, LodState.WingLegalReview)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Investigation, lodCase));

            // Configure WingLegalReview state
            stateMachine.Configure(LodState.WingLegalReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.WingLegalReview, lodCase))
                .Permit(LodTrigger.LegalReviewComplete, LodState.WingCommanderReview)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.WingLegalReview, lodCase));

            // Configure WingCommanderReview state
            stateMachine.Configure(LodState.WingCommanderReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.WingCommanderReview, lodCase))
                .Permit(LodTrigger.WingReviewComplete, LodState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.WingCommanderReview, lodCase));

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.BoardAdjudication, lodCase))
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.BoardAdjudication, lodCase));

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Determination, lodCase))
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Determination, lodCase));

            // Configure Notification state
            stateMachine.Configure(LodState.Notification)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Notification, lodCase))
                .Permit(LodTrigger.AppealRequested, LodState.Appeal)
                .Permit(LodTrigger.NoAppealRequested, LodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Notification, lodCase));

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.Appeal, lodCase))
                .Permit(LodTrigger.AppealResolved, LodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LodState.Appeal, lodCase));

            // Configure End state
            stateMachine.Configure(LodState.End)
                .OnEntryAsync(async () => await OnStateEntryAsync(LodState.End, lodCase));

            return stateMachine;
            
            static async Task OnStateEntryAsync(LodState state, LineOfDuty lodCase)
            {
                Console.WriteLine($"{(lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]")} [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }

            static async Task OnStateExitAsync(LodState state, LineOfDuty lodCase)
            {
                Console.WriteLine($"{(lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]")} [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }
        }

        #endregion
    }
}
