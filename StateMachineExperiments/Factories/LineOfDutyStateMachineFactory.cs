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
    public class LineOfDutyStateMachineFactory : ILineOfDutyStateMachineFactory
    {
        public StateMachine<LineOfDutyState, LineOfDutyTrigger> CreateStateMachine(LineOfDutyCase lodCase)
        {
            return lodCase.LineOfDutyType == LineOfDutyCaseType.Informal 
                ? CreateInformalStateMachine(lodCase) 
                : CreateFormalStateMachine(lodCase);
        }

        #region Informal LOD State Machine

        private static StateMachine<LineOfDutyState, LineOfDutyTrigger> CreateInformalStateMachine(LineOfDutyCase lodCase)
        {
            var stateMachine = new StateMachine<LineOfDutyState, LineOfDutyTrigger>(lodCase.LineOfDutyState);

            // Configure Start state
            stateMachine.Configure(LineOfDutyState.Start)
                .Permit(LineOfDutyTrigger.ProcessInitiated, LineOfDutyState.MemberReports)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Start, lodCase));

            // Configure MemberReports state
            stateMachine.Configure(LineOfDutyState.MemberReports)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.MemberReports, lodCase))
                .Permit(LineOfDutyTrigger.ConditionReported, LineOfDutyState.LodInitiation)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.MemberReports, lodCase));

            // Configure LodInitiation state
            stateMachine.Configure(LineOfDutyState.LodInitiation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.LodInitiation, lodCase))
                .Permit(LineOfDutyTrigger.InitiationComplete, LineOfDutyState.MedicalAssessment)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.LodInitiation, lodCase));

            // Configure MedicalAssessment state
            stateMachine.Configure(LineOfDutyState.MedicalAssessment)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.MedicalAssessment, lodCase))
                .Permit(LineOfDutyTrigger.AssessmentDone, LineOfDutyState.CommanderReview)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.MedicalAssessment, lodCase));

            // Configure CommanderReview state - dynamic routing based on case requirements
            stateMachine.Configure(LineOfDutyState.CommanderReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.CommanderReview, lodCase))
                .PermitIf(LineOfDutyTrigger.ReviewFinished, LineOfDutyState.OptionalLegal, () => lodCase.RequiresLegalReview)
                .Permit(LineOfDutyTrigger.SkipToAdjudication, LineOfDutyState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.CommanderReview, lodCase));

            // Configure OptionalLegal state
            stateMachine.Configure(LineOfDutyState.OptionalLegal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.OptionalLegal, lodCase))
                .PermitIf(LineOfDutyTrigger.LegalDone, LineOfDutyState.OptionalWing, () => lodCase.RequiresWingReview)
                .Permit(LineOfDutyTrigger.SkipWingReview, LineOfDutyState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.OptionalLegal, lodCase));

            // Configure OptionalWing state
            stateMachine.Configure(LineOfDutyState.OptionalWing)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.OptionalWing, lodCase))
                .Permit(LineOfDutyTrigger.WingDone, LineOfDutyState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.OptionalWing, lodCase));

            // Configure BoardAdjudication state
            stateMachine.Configure(LineOfDutyState.BoardAdjudication)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.BoardAdjudication, lodCase))
                .Permit(LineOfDutyTrigger.AdjudicationComplete, LineOfDutyState.Determination)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.BoardAdjudication, lodCase));

            // Configure Determination state
            stateMachine.Configure(LineOfDutyState.Determination)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Determination, lodCase))
                .Permit(LineOfDutyTrigger.DeterminationFinalized, LineOfDutyState.Notification)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Determination, lodCase));

            // Configure Notification state
            stateMachine.Configure(LineOfDutyState.Notification)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Notification, lodCase))
                .Permit(LineOfDutyTrigger.AppealFiled, LineOfDutyState.Appeal)
                .Permit(LineOfDutyTrigger.NotificationComplete, LineOfDutyState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Notification, lodCase));

            // Configure Appeal state
            stateMachine.Configure(LineOfDutyState.Appeal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Appeal, lodCase))
                .Permit(LineOfDutyTrigger.AppealResolved, LineOfDutyState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Appeal, lodCase));

            // Configure End state
            stateMachine.Configure(LineOfDutyState.End)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.End, lodCase));

            return stateMachine;

            static async Task OnStateEntryAsync(LineOfDutyState state, LineOfDutyCase lodCase)
            {
                Console.WriteLine($"[INFORMAL] [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }

            static async Task OnStateExitAsync(LineOfDutyState state, LineOfDutyCase lodCase)
            {
                Console.WriteLine($"[INFORMAL] [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }
        }

        #endregion

        #region Formal LOD State Machine

        private static StateMachine<LineOfDutyState, LineOfDutyTrigger> CreateFormalStateMachine(LineOfDutyCase lodCase)
        {
            var stateMachine = new StateMachine<LineOfDutyState, LineOfDutyTrigger>(lodCase.LineOfDutyState);

            // Configure Start state
            stateMachine.Configure(LineOfDutyState.Start)
                .Permit(LineOfDutyTrigger.ProcessInitiated, LineOfDutyState.MemberReports)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Start, lodCase));

            // Configure MemberReports state
            stateMachine.Configure(LineOfDutyState.MemberReports)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.MemberReports, lodCase))
                .Permit(LineOfDutyTrigger.ConditionReported, LineOfDutyState.FormalInitiation)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.MemberReports, lodCase));

            // Configure FormalInitiation state
            stateMachine.Configure(LineOfDutyState.FormalInitiation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.FormalInitiation, lodCase))
                .Permit(LineOfDutyTrigger.QuestionableDetected, LineOfDutyState.AppointingOfficer)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.FormalInitiation, lodCase));

            // Configure AppointingOfficer state
            stateMachine.Configure(LineOfDutyState.AppointingOfficer)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.AppointingOfficer, lodCase))
                .Permit(LineOfDutyTrigger.OfficerAppointed, LineOfDutyState.Investigation)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.AppointingOfficer, lodCase));

            // Configure Investigation state
            stateMachine.Configure(LineOfDutyState.Investigation)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Investigation, lodCase))
                .Permit(LineOfDutyTrigger.InvestigationComplete, LineOfDutyState.WingLegalReview)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Investigation, lodCase));

            // Configure WingLegalReview state
            stateMachine.Configure(LineOfDutyState.WingLegalReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.WingLegalReview, lodCase))
                .Permit(LineOfDutyTrigger.LegalReviewComplete, LineOfDutyState.WingCommanderReview)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.WingLegalReview, lodCase));

            // Configure WingCommanderReview state
            stateMachine.Configure(LineOfDutyState.WingCommanderReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.WingCommanderReview, lodCase))
                .Permit(LineOfDutyTrigger.WingReviewComplete, LineOfDutyState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.WingCommanderReview, lodCase));

            // Configure BoardAdjudication state
            stateMachine.Configure(LineOfDutyState.BoardAdjudication)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.BoardAdjudication, lodCase))
                .Permit(LineOfDutyTrigger.AdjudicationComplete, LineOfDutyState.Determination)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.BoardAdjudication, lodCase));

            // Configure Determination state
            stateMachine.Configure(LineOfDutyState.Determination)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Determination, lodCase))
                .Permit(LineOfDutyTrigger.DeterminationFinalized, LineOfDutyState.Notification)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Determination, lodCase));

            // Configure Notification state
            stateMachine.Configure(LineOfDutyState.Notification)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Notification, lodCase))
                .Permit(LineOfDutyTrigger.AppealRequested, LineOfDutyState.Appeal)
                .Permit(LineOfDutyTrigger.NoAppealRequested, LineOfDutyState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Notification, lodCase));

            // Configure Appeal state
            stateMachine.Configure(LineOfDutyState.Appeal)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.Appeal, lodCase))
                .Permit(LineOfDutyTrigger.AppealResolved, LineOfDutyState.End)
                .OnExitAsync(async () => await OnStateExitAsync(LineOfDutyState.Appeal, lodCase));

            // Configure End state
            stateMachine.Configure(LineOfDutyState.End)
                .OnEntryAsync(async () => await OnStateEntryAsync(LineOfDutyState.End, lodCase));

            return stateMachine;
            
            static async Task OnStateEntryAsync(LineOfDutyState state, LineOfDutyCase lodCase)
            {
                Console.WriteLine($"{(lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]")} [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }

            static async Task OnStateExitAsync(LineOfDutyState state, LineOfDutyCase lodCase)
            {
                Console.WriteLine($"{(lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]")} [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
                await Task.CompletedTask;
            }
        }

        #endregion
    }
}
