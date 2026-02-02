using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Events;
using StateMachineExperiments.Modules.InformalLOD.Models;
using System;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class LodStateMachineFactory : ILodStateMachineFactory
    {
        private readonly INotificationService _notificationService;

        public LodStateMachineFactory(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public StateMachine<LodState, LodTrigger> CreateStateMachine(InformalLineOfDuty lodCase)
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
        }

        private static async Task OnStateEntryAsync(LodState state, InformalLineOfDuty lodCase)
        {
            Console.WriteLine($"[ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");

            // Handle state-specific notifications
            switch (state)
            {
                case LodState.Determination:
                    break;

                case LodState.Notification:
                    break;

                case LodState.Appeal:
                    break;
            }
            
            await Task.CompletedTask;
        }

        private static async Task OnStateExitAsync(LodState state, InformalLineOfDuty lodCase)
        {
            Console.WriteLine($"[EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
            
            await Task.CompletedTask;
        }
    }
}
