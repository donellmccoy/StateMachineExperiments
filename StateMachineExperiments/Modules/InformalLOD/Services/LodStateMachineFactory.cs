using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Events;
using StateMachineExperiments.Modules.InformalLOD.Models;
using System;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class ReviewData
    {
        public bool Approved { get; set; }
        public string? ReviewerId { get; set; }
        public string? Comments { get; set; }
    }

    public interface ILodStateMachineFactory
    {
        StateMachine<LodState, LodTrigger> CreateStateMachine(
            InformalLineOfDuty lodCase, 
            INotificationService? notificationService = null);
    }

    public class LodStateMachineFactory : ILodStateMachineFactory
    {
        private readonly INotificationService _notificationService;

        public LodStateMachineFactory(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public StateMachine<LodState, LodTrigger> CreateStateMachine(
            InformalLineOfDuty lodCase, 
            INotificationService? notificationService = null)
        {
            // Use injected service if not provided as parameter
            var notificationSvc = notificationService ?? _notificationService;
            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = new StateMachine<LodState, LodTrigger>(currentState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports)
                .OnExit(() => OnStateExit(LodState.Start, lodCase, notificationSvc));

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .OnEntry(() => OnStateEntry(LodState.MemberReports, lodCase, notificationSvc))
                .Permit(LodTrigger.ConditionReported, LodState.LodInitiation)
                .OnExit(() => OnStateExit(LodState.MemberReports, lodCase, notificationSvc));

            // Configure LodInitiation state
            stateMachine.Configure(LodState.LodInitiation)
                .OnEntry(() => OnStateEntry(LodState.LodInitiation, lodCase, notificationSvc))
                .Permit(LodTrigger.InitiationComplete, LodState.MedicalAssessment)
                .OnExit(() => OnStateExit(LodState.LodInitiation, lodCase, notificationSvc));

            // Configure MedicalAssessment state
            stateMachine.Configure(LodState.MedicalAssessment)
                .OnEntry(() => OnStateEntry(LodState.MedicalAssessment, lodCase, notificationSvc))
                .Permit(LodTrigger.AssessmentDone, LodState.CommanderReview)
                .OnExit(() => OnStateExit(LodState.MedicalAssessment, lodCase, notificationSvc));

            // Configure CommanderReview state - dynamic routing based on case requirements
            stateMachine.Configure(LodState.CommanderReview)
                .OnEntry(() => OnStateEntry(LodState.CommanderReview, lodCase, notificationSvc))
                .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, () => lodCase.RequiresLegalReview)
                .Permit(LodTrigger.SkipToAdjudication, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.CommanderReview, lodCase, notificationSvc));

            // Configure OptionalLegal state
            stateMachine.Configure(LodState.OptionalLegal)
                .OnEntry(() => OnStateEntry(LodState.OptionalLegal, lodCase, notificationSvc))
                .PermitIf(LodTrigger.LegalDone, LodState.OptionalWing, () => lodCase.RequiresWingReview)
                .Permit(LodTrigger.SkipWingReview, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.OptionalLegal, lodCase, notificationSvc));

            // Configure OptionalWing state
            stateMachine.Configure(LodState.OptionalWing)
                .OnEntry(() => OnStateEntry(LodState.OptionalWing, lodCase, notificationSvc))
                .Permit(LodTrigger.WingDone, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.OptionalWing, lodCase, notificationSvc));

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .OnEntry(() => OnStateEntry(LodState.BoardAdjudication, lodCase, notificationSvc))
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination)
                .OnExit(() => OnStateExit(LodState.BoardAdjudication, lodCase, notificationSvc));

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .OnEntry(() => OnStateEntry(LodState.Determination, lodCase, notificationSvc))
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification)
                .OnExit(() =>
                {
                    // Notify stakeholders of finalized determination
                    notificationSvc.SendStakeholderAlert(
                        caseNumber: lodCase.CaseNumber,
                        alertType: "Determination Finalized",
                        message: $"LOD determination finalized for case {lodCase.CaseNumber}: In Line of Duty",
                        stakeholders: new[] { "HQ AFRC/A1", "LOD Manager" });
                    OnStateExit(LodState.Determination, lodCase, notificationSvc);
                });

            // Configure Notification state
            stateMachine.Configure(LodState.Notification)
                .OnEntry(() =>
                {
                    // Auto-send notification when entering this state
                    SendNotification(lodCase, notificationSvc);
                    OnStateEntry(LodState.Notification, lodCase, notificationSvc);
                })
                .Permit(LodTrigger.AppealFiled, LodState.Appeal)
                .Permit(LodTrigger.NotificationComplete, LodState.End)
                .OnExit(() => OnStateExit(LodState.Notification, lodCase, notificationSvc));

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .OnEntry(() =>
                {
                    notificationSvc.SendNotification(
                        recipient: lodCase.MemberName ?? "Member",
                        subject: $"Appeal Filed - Case {lodCase.CaseNumber}",
                        message: $"An appeal has been filed for LOD case {lodCase.CaseNumber} on {DateTime.UtcNow:yyyy-MM-dd}. " +
                                $"The appeal will be reviewed by HQ AFRC/CD.",
                        notificationType: "Email");
                    OnStateEntry(LodState.Appeal, lodCase, notificationSvc);
                })
                .Permit(LodTrigger.AppealResolved, LodState.End)
                .OnExit(() => OnStateExit(LodState.Appeal, lodCase, notificationSvc));

            // Configure End state
            stateMachine.Configure(LodState.End)
                .OnEntry(() => OnStateEntry(LodState.End, lodCase, notificationSvc));

            return stateMachine;
        }

        private static void OnStateEntry(LodState state, InformalLineOfDuty lodCase, INotificationService notificationService)
        {
            Console.WriteLine($"[ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
        }

        private static void OnStateExit(LodState state, InformalLineOfDuty lodCase, INotificationService notificationService)
        {
            Console.WriteLine($"[EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
        }

        private static void SendNotification(InformalLineOfDuty lodCase, INotificationService notificationService)
        {
            notificationService.SendDeterminationNotification(
                caseNumber: lodCase.CaseNumber,
                memberId: lodCase.MemberId ?? "Unknown",
                memberName: lodCase.MemberName ?? "Unknown",
                determination: "In Line of Duty",
                appealWindowDays: 30,
                notificationType: "Email");
        }
    }
}
