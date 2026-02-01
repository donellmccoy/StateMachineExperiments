using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Events;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System;
using System.Threading.Tasks;

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
            var stateMachine = new StateMachine<FormalLodState, FormalLodTrigger>(lodCase.CurrentState);

            // Configure Start state
            stateMachine.Configure(FormalLodState.Start)
                .Permit(FormalLodTrigger.ProcessInitiated, FormalLodState.MemberReports)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.Start, lodCase));

            // Configure MemberReports state
            stateMachine.Configure(FormalLodState.MemberReports)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.MemberReports, lodCase))
                .Permit(FormalLodTrigger.ConditionReported, FormalLodState.FormalInitiation)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.MemberReports, lodCase));

            // Configure FormalInitiation state
            stateMachine.Configure(FormalLodState.FormalInitiation)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.FormalInitiation, lodCase))
                .Permit(FormalLodTrigger.QuestionableDetected, FormalLodState.AppointingOfficer)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.FormalInitiation, lodCase));

            // Configure AppointingOfficer state
            stateMachine.Configure(FormalLodState.AppointingOfficer)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.AppointingOfficer, lodCase))
                .Permit(FormalLodTrigger.OfficerAppointed, FormalLodState.Investigation)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.AppointingOfficer, lodCase));

            // Configure Investigation state
            stateMachine.Configure(FormalLodState.Investigation)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.Investigation, lodCase))
                .Permit(FormalLodTrigger.InvestigationComplete, FormalLodState.WingLegalReview)
                .OnExitAsync(async () =>
                {
                    lodCase.InvestigationCompletionDate = DateTime.UtcNow;
                    await OnStateExitAsync(FormalLodState.Investigation, lodCase);
                });

            // Configure WingLegalReview state
            stateMachine.Configure(FormalLodState.WingLegalReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.WingLegalReview, lodCase))
                .Permit(FormalLodTrigger.LegalReviewComplete, FormalLodState.WingCommanderReview)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.WingLegalReview, lodCase));

            // Configure WingCommanderReview state
            stateMachine.Configure(FormalLodState.WingCommanderReview)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.WingCommanderReview, lodCase))
                .Permit(FormalLodTrigger.WingReviewComplete, FormalLodState.BoardAdjudication)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.WingCommanderReview, lodCase));

            // Configure BoardAdjudication state
            stateMachine.Configure(FormalLodState.BoardAdjudication)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.BoardAdjudication, lodCase))
                .Permit(FormalLodTrigger.AdjudicationComplete, FormalLodState.Determination)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.BoardAdjudication, lodCase));

            // Configure Determination state
            stateMachine.Configure(FormalLodState.Determination)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.Determination, lodCase))
                .Permit(FormalLodTrigger.DeterminationFinalized, FormalLodState.Notification)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.Determination, lodCase));  

            // Configure Notification state
            stateMachine.Configure(FormalLodState.Notification)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.Notification, lodCase))
                .Permit(FormalLodTrigger.AppealRequested, FormalLodState.Appeal)
                .Permit(FormalLodTrigger.NoAppealRequested, FormalLodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.Notification, lodCase));

            // Configure Appeal state
            stateMachine.Configure(FormalLodState.Appeal)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.Appeal, lodCase))
                .Permit(FormalLodTrigger.AppealResolved, FormalLodState.End)
                .OnExitAsync(async () => await OnStateExitAsync(FormalLodState.Appeal, lodCase));

            // Configure End state
            stateMachine.Configure(FormalLodState.End)
                .OnEntryAsync(async () => await OnStateEntryAsync(FormalLodState.End, lodCase));

            return stateMachine;
        }

        private async Task OnStateEntryAsync(FormalLodState state, FormalLineOfDuty lodCase)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");

            // Handle state-specific notifications
            switch (state)
            {
                case FormalLodState.Investigation:
                    if (lodCase.InvestigationStartDate == null)
                    {
                        lodCase.InvestigationStartDate = DateTime.UtcNow;
                        await _notificationService.AlertStakeholdersAsync(new StakeholderAlertRequest
                        {
                            CaseNumber = lodCase.CaseNumber,
                            AlertType = "Investigation Started",
                            Message = $"Investigation started by {lodCase.InvestigatingOfficerName} for case {lodCase.CaseNumber}" +
                                     (lodCase.IsDeathCase ? " (DEATH CASE - EXPEDITED)" : ""),
                            Stakeholders = new[] { "LOD Manager", "Wing Commander", lodCase.InvestigatingOfficerId ?? "Unknown" }
                        });
                    }
                    break;

                case FormalLodState.BoardAdjudication:
                    Console.WriteLine($"[BOARD] AFRC LOD Determination Board (SG, JA, A1) reviewing case {lodCase.CaseNumber}");
                    break;

                case FormalLodState.Determination:
                    await _notificationService.AlertStakeholdersAsync(new StakeholderAlertRequest
                    {
                        CaseNumber = lodCase.CaseNumber,
                        AlertType = "Determination Finalized",
                        Message = $"LOD determination finalized for case {lodCase.CaseNumber}: {lodCase.DeterminationResult ?? "In Line of Duty"}",
                        Stakeholders = new[] { "HQ AFRC/A1", "LOD Manager", "Wing Commander" }
                    });
                    break;

                case FormalLodState.Notification:
                    var appealWindowDays = lodCase.IsDeathCase ? 180 : 30;
                    var recipient = lodCase.IsDeathCase ? "Next of Kin" : lodCase.MemberName ?? "Unknown";
                    await _notificationService.NotifyDeterminationAsync(new DeterminationNotificationRequest
                    {
                        CaseNumber = lodCase.CaseNumber,
                        MemberId = lodCase.MemberId ?? "Unknown",
                        MemberName = recipient,
                        Determination = lodCase.DeterminationResult ?? "In Line of Duty",
                        AppealWindowDays = appealWindowDays,
                        NotificationType = lodCase.IsDeathCase ? "Certified Letter" : "Email"
                    });
                    break;

                case FormalLodState.Appeal:
                    lodCase.AppealFiled = true;
                    var appealRecipient = lodCase.IsDeathCase ? "Next of Kin" : lodCase.MemberName ?? "Unknown";
                    await _notificationService.NotifyAsync(new NotificationRequest
                    {
                        Recipient = appealRecipient,
                        Subject = $"Appeal Filed - Case {lodCase.CaseNumber}",
                        Message = $"An appeal has been filed for LOD case {lodCase.CaseNumber} on {DateTime.UtcNow:yyyy-MM-dd}. " +
                                 $"The appeal will be reviewed by HQ AFRC/CD.",
                        NotificationType = "Email"
                    });
                    break;
            }
        }

        private async Task OnStateExitAsync(FormalLodState state, FormalLineOfDuty lodCase)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
            await Task.CompletedTask;
        }


    }
}
