using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Events;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodStateMachineFactory : IFormalLodStateMachineFactory
    {
        private readonly INotificationService _notificationService;

        public FormalLodStateMachineFactory(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public StateMachine<FormalLodState, FormalLodTrigger> CreateStateMachine(FormalLineOfDuty lodCase)
        {
            var currentState = Enum.Parse<FormalLodState>(lodCase.CurrentState);
            var stateMachine = new StateMachine<FormalLodState, FormalLodTrigger>(currentState);

            // Configure Start state
            stateMachine.Configure(FormalLodState.Start)
                .Permit(FormalLodTrigger.ProcessInitiated, FormalLodState.MemberReports)
                .OnExit(() => OnStateExit(FormalLodState.Start, lodCase, _notificationService));

            // Configure MemberReports state
            stateMachine.Configure(FormalLodState.MemberReports)
                .OnEntry(() => OnStateEntry(FormalLodState.MemberReports, lodCase, _notificationService))
                .Permit(FormalLodTrigger.ConditionReported, FormalLodState.FormalInitiation)
                .OnExit(() => OnStateExit(FormalLodState.MemberReports, lodCase, _notificationService));

            // Configure FormalInitiation state
            stateMachine.Configure(FormalLodState.FormalInitiation)
                .OnEntry(() => OnStateEntry(FormalLodState.FormalInitiation, lodCase, _notificationService))
                .Permit(FormalLodTrigger.QuestionableDetected, FormalLodState.AppointingOfficer)
                .OnExit(() => OnStateExit(FormalLodState.FormalInitiation, lodCase, _notificationService));

            // Configure AppointingOfficer state
            stateMachine.Configure(FormalLodState.AppointingOfficer)
                .OnEntry(() => OnStateEntry(FormalLodState.AppointingOfficer, lodCase, _notificationService))
                .Permit(FormalLodTrigger.OfficerAppointed, FormalLodState.Investigation)
                .OnExit(() => OnStateExit(FormalLodState.AppointingOfficer, lodCase, _notificationService));

            // Configure Investigation state
            stateMachine.Configure(FormalLodState.Investigation)
                .OnEntry(() =>
                {
                    if (lodCase.InvestigationStartDate == null)
                    {
                        lodCase.InvestigationStartDate = DateTime.UtcNow;
                        _notificationService.SendStakeholderAlert(
                            caseNumber: lodCase.CaseNumber,
                            alertType: "Investigation Started",
                            message: $"Investigation started by {lodCase.InvestigatingOfficerName} for case {lodCase.CaseNumber}" + 
                                    (lodCase.IsDeathCase ? " (DEATH CASE - EXPEDITED)" : ""),
                            stakeholders: new[] { "LOD Manager", "Wing Commander", lodCase.InvestigatingOfficerId ?? "Unknown" });
                    }
                    OnStateEntry(FormalLodState.Investigation, lodCase, _notificationService);
                })
                .Permit(FormalLodTrigger.InvestigationComplete, FormalLodState.WingLegalReview)
                .OnExit(() =>
                {
                    lodCase.InvestigationCompletionDate = DateTime.UtcNow;
                    OnStateExit(FormalLodState.Investigation, lodCase, _notificationService);
                });

            // Configure WingLegalReview state
            stateMachine.Configure(FormalLodState.WingLegalReview)
                .OnEntry(() => OnStateEntry(FormalLodState.WingLegalReview, lodCase, _notificationService))
                .Permit(FormalLodTrigger.LegalReviewComplete, FormalLodState.WingCommanderReview)
                .OnExit(() => OnStateExit(FormalLodState.WingLegalReview, lodCase, _notificationService));

            // Configure WingCommanderReview state
            stateMachine.Configure(FormalLodState.WingCommanderReview)
                .OnEntry(() => OnStateEntry(FormalLodState.WingCommanderReview, lodCase, _notificationService))
                .Permit(FormalLodTrigger.WingReviewComplete, FormalLodState.BoardAdjudication)
                .OnExit(() => OnStateExit(FormalLodState.WingCommanderReview, lodCase, _notificationService));

            // Configure BoardAdjudication state
            stateMachine.Configure(FormalLodState.BoardAdjudication)
                .OnEntry(() =>
                {
                    Console.WriteLine($"[BOARD] AFRC LOD Determination Board (SG, JA, A1) reviewing case {lodCase.CaseNumber}");
                    OnStateEntry(FormalLodState.BoardAdjudication, lodCase, _notificationService);
                })
                .Permit(FormalLodTrigger.AdjudicationComplete, FormalLodState.Determination)
                .OnExit(() => OnStateExit(FormalLodState.BoardAdjudication, lodCase, _notificationService));

            // Configure Determination state
            stateMachine.Configure(FormalLodState.Determination)
                .OnEntry(() => OnStateEntry(FormalLodState.Determination, lodCase, _notificationService))
                .Permit(FormalLodTrigger.DeterminationFinalized, FormalLodState.Notification)
                .OnExit(() =>
                {
                    // Notify stakeholders of finalized determination
                    _notificationService.SendStakeholderAlert(
                        caseNumber: lodCase.CaseNumber,
                        alertType: "Determination Finalized",
                        message: $"LOD determination finalized for case {lodCase.CaseNumber}: {lodCase.DeterminationResult ?? "In Line of Duty"}",
                        stakeholders: new[] { "HQ AFRC/A1", "LOD Manager", "Wing Commander" });
                    OnStateExit(FormalLodState.Determination, lodCase, _notificationService);
                });  

            // Configure Notification state
            stateMachine.Configure(FormalLodState.Notification)
                .OnEntry(() =>
                {
                    // Auto-send notification when entering this state
                    SendNotification(lodCase, _notificationService);
                    OnStateEntry(FormalLodState.Notification, lodCase, _notificationService);
                })
                .Permit(FormalLodTrigger.AppealRequested, FormalLodState.Appeal)
                .Permit(FormalLodTrigger.NoAppealRequested, FormalLodState.End)
                .OnExit(() => OnStateExit(FormalLodState.Notification, lodCase, _notificationService));

            // Configure Appeal state
            stateMachine.Configure(FormalLodState.Appeal)
                .OnEntry(() =>
                {
                    lodCase.AppealFiled = true;
                    var appealRecipient = lodCase.IsDeathCase ? "Next of Kin" : lodCase.MemberName ?? "Unknown";
                    _notificationService.SendNotification(
                        recipient: appealRecipient,
                        subject: $"Appeal Filed - Case {lodCase.CaseNumber}",
                        message: $"An appeal has been filed for LOD case {lodCase.CaseNumber} on {DateTime.UtcNow:yyyy-MM-dd}. " +
                                $"The appeal will be reviewed by HQ AFRC/CD.",
                        notificationType: "Email");
                    OnStateEntry(FormalLodState.Appeal, lodCase, _notificationService);
                })
                .Permit(FormalLodTrigger.AppealResolved, FormalLodState.End)
                .OnExit(() => OnStateExit(FormalLodState.Appeal, lodCase, _notificationService));

            // Configure End state
            stateMachine.Configure(FormalLodState.End)
                .OnEntry(() => OnStateEntry(FormalLodState.End, lodCase, _notificationService));

            return stateMachine;
        }

        private static void OnStateEntry(FormalLodState state, FormalLineOfDuty lodCase, INotificationService notificationService)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
        }

        private static void OnStateExit(FormalLodState state, FormalLineOfDuty lodCase, INotificationService notificationService)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
        }

        private static void SendNotification(FormalLineOfDuty lodCase, INotificationService notificationService)
        {
            var appealWindowDays = lodCase.IsDeathCase ? 180 : 30;
            var recipient = lodCase.IsDeathCase ? "Next of Kin" : lodCase.MemberName ?? "Unknown";
            
            notificationService.SendDeterminationNotification(
                caseNumber: lodCase.CaseNumber,
                memberId: lodCase.MemberId ?? "Unknown",
                memberName: recipient,
                determination: lodCase.DeterminationResult ?? "In Line of Duty",
                appealWindowDays: appealWindowDays,
                notificationType: lodCase.IsDeathCase ? "Certified Letter" : "Email");
        }
    }
}
