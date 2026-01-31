using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Events;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodStateMachineFactory : IFormalLodStateMachineFactory
    {
        public StateMachine<FormalLodState, FormalLodTrigger> CreateStateMachine(
            FormalLineOfDuty lodCase, 
            IEventPublisher? eventPublisher = null)
        {
            var currentState = Enum.Parse<FormalLodState>(lodCase.CurrentState);
            var stateMachine = new StateMachine<FormalLodState, FormalLodTrigger>(currentState);

            // Configure Start state
            stateMachine.Configure(FormalLodState.Start)
                .Permit(FormalLodTrigger.ProcessInitiated, FormalLodState.MemberReports)
                .OnExit(() => OnStateExit(FormalLodState.Start, lodCase, eventPublisher));

            // Configure MemberReports state
            stateMachine.Configure(FormalLodState.MemberReports)
                .OnEntry(() => OnStateEntry(FormalLodState.MemberReports, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.ConditionReported, FormalLodState.FormalInitiation)
                .OnExit(() => OnStateExit(FormalLodState.MemberReports, lodCase, eventPublisher));

            // Configure FormalInitiation state
            stateMachine.Configure(FormalLodState.FormalInitiation)
                .OnEntry(() => OnStateEntry(FormalLodState.FormalInitiation, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.QuestionableDetected, FormalLodState.AppointingOfficer)
                .OnExit(() => OnStateExit(FormalLodState.FormalInitiation, lodCase, eventPublisher));

            // Configure AppointingOfficer state
            stateMachine.Configure(FormalLodState.AppointingOfficer)
                .OnEntry(() => OnStateEntry(FormalLodState.AppointingOfficer, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.OfficerAppointed, FormalLodState.Investigation)
                .OnExit(() => OnStateExit(FormalLodState.AppointingOfficer, lodCase, eventPublisher));

            // Configure Investigation state
            stateMachine.Configure(FormalLodState.Investigation)
                .OnEntry(() =>
                {
                    if (lodCase.InvestigationStartDate == null)
                    {
                        lodCase.InvestigationStartDate = DateTime.UtcNow;
                        eventPublisher?.Publish(new FormalLodInvestigationStartedEvent
                        {
                            CaseId = lodCase.Id,
                            CaseNumber = lodCase.CaseNumber,
                            InvestigatingOfficerId = lodCase.InvestigatingOfficerId ?? "Unknown",
                            InvestigatingOfficerName = lodCase.InvestigatingOfficerName ?? "Unknown",
                            StartDate = lodCase.InvestigationStartDate.Value,
                            IsDeathCase = lodCase.IsDeathCase
                        });
                    }
                    OnStateEntry(FormalLodState.Investigation, lodCase, eventPublisher);
                })
                .Permit(FormalLodTrigger.InvestigationComplete, FormalLodState.WingLegalReview)
                .OnExit(() =>
                {
                    lodCase.InvestigationCompletionDate = DateTime.UtcNow;
                    OnStateExit(FormalLodState.Investigation, lodCase, eventPublisher);
                });

            // Configure WingLegalReview state
            stateMachine.Configure(FormalLodState.WingLegalReview)
                .OnEntry(() => OnStateEntry(FormalLodState.WingLegalReview, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.LegalReviewComplete, FormalLodState.WingCommanderReview)
                .OnExit(() => OnStateExit(FormalLodState.WingLegalReview, lodCase, eventPublisher));

            // Configure WingCommanderReview state
            stateMachine.Configure(FormalLodState.WingCommanderReview)
                .OnEntry(() => OnStateEntry(FormalLodState.WingCommanderReview, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.WingReviewComplete, FormalLodState.BoardAdjudication)
                .OnExit(() => OnStateExit(FormalLodState.WingCommanderReview, lodCase, eventPublisher));

            // Configure BoardAdjudication state
            stateMachine.Configure(FormalLodState.BoardAdjudication)
                .OnEntry(() =>
                {
                    Console.WriteLine($"[BOARD] AFRC LOD Determination Board (SG, JA, A1) reviewing case {lodCase.CaseNumber}");
                    OnStateEntry(FormalLodState.BoardAdjudication, lodCase, eventPublisher);
                })
                .Permit(FormalLodTrigger.AdjudicationComplete, FormalLodState.Determination)
                .OnExit(() => OnStateExit(FormalLodState.BoardAdjudication, lodCase, eventPublisher));

            // Configure Determination state
            stateMachine.Configure(FormalLodState.Determination)
                .OnEntry(() => OnStateEntry(FormalLodState.Determination, lodCase, eventPublisher))
                .Permit(FormalLodTrigger.DeterminationFinalized, FormalLodState.Notification)
                .OnExit(() =>
                {
                    // Publish determination finalized event
                    eventPublisher?.Publish(new FormalLodDeterminationFinalizedEvent
                    {
                        CaseId = lodCase.Id,
                        CaseNumber = lodCase.CaseNumber,
                        Determination = lodCase.DeterminationResult ?? "In Line of Duty",
                        ApprovingAuthority = "HQ AFRC/A1"
                    });
                    OnStateExit(FormalLodState.Determination, lodCase, eventPublisher);
                });

            // Configure Notification state
            stateMachine.Configure(FormalLodState.Notification)
                .OnEntry(() =>
                {
                    // Auto-send notification when entering this state
                    SendNotification(lodCase);
                    OnStateEntry(FormalLodState.Notification, lodCase, eventPublisher);
                })
                .Permit(FormalLodTrigger.AppealRequested, FormalLodState.Appeal)
                .Permit(FormalLodTrigger.NoAppealRequested, FormalLodState.End)
                .OnExit(() => OnStateExit(FormalLodState.Notification, lodCase, eventPublisher));

            // Configure Appeal state
            stateMachine.Configure(FormalLodState.Appeal)
                .OnEntry(() =>
                {
                    lodCase.AppealFiled = true;
                    eventPublisher?.Publish(new FormalLodAppealFiledEvent
                    {
                        CaseId = lodCase.Id,
                        CaseNumber = lodCase.CaseNumber,
                        AppealDate = DateTime.UtcNow,
                        MemberId = lodCase.MemberId,
                        IsDeathCase = lodCase.IsDeathCase
                    });
                    OnStateEntry(FormalLodState.Appeal, lodCase, eventPublisher);
                })
                .Permit(FormalLodTrigger.AppealResolved, FormalLodState.End)
                .OnExit(() => OnStateExit(FormalLodState.Appeal, lodCase, eventPublisher));

            // Configure End state
            stateMachine.Configure(FormalLodState.End)
                .OnEntry(() => OnStateEntry(FormalLodState.End, lodCase, eventPublisher));

            return stateMachine;
        }

        private static void OnStateEntry(FormalLodState state, FormalLineOfDuty lodCase, IEventPublisher? eventPublisher)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
        }

        private static void OnStateExit(FormalLodState state, FormalLineOfDuty lodCase, IEventPublisher? eventPublisher)
        {
            var prefix = lodCase.IsDeathCase ? "[DEATH-EXPEDITED]" : "[FORMAL]";
            Console.WriteLine($"{prefix} [EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
        }

        private static void SendNotification(FormalLineOfDuty lodCase)
        {
            var appealWindow = lodCase.IsDeathCase ? "180 days (next of kin)" : "30 days";
            Console.WriteLine($"[NOTIFICATION] Sending determination to member {lodCase.MemberName} ({lodCase.MemberId}) for case {lodCase.CaseNumber}");
            Console.WriteLine($"[NOTIFICATION] Appeal rights: {appealWindow} from notification date");
        }
    }
}
