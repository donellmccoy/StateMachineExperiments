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
            IEventPublisher? eventPublisher = null);
    }

    public class LodStateMachineFactory : ILodStateMachineFactory
    {
        public StateMachine<LodState, LodTrigger> CreateStateMachine(
            InformalLineOfDuty lodCase, 
            IEventPublisher? eventPublisher = null)
        {
            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = new StateMachine<LodState, LodTrigger>(currentState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports)
                .OnExit(() => OnStateExit(LodState.Start, lodCase, eventPublisher));

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .OnEntry(() => OnStateEntry(LodState.MemberReports, lodCase, eventPublisher))
                .Permit(LodTrigger.ConditionReported, LodState.LodInitiation)
                .OnExit(() => OnStateExit(LodState.MemberReports, lodCase, eventPublisher));

            // Configure LodInitiation state
            stateMachine.Configure(LodState.LodInitiation)
                .OnEntry(() => OnStateEntry(LodState.LodInitiation, lodCase, eventPublisher))
                .Permit(LodTrigger.InitiationComplete, LodState.MedicalAssessment)
                .OnExit(() => OnStateExit(LodState.LodInitiation, lodCase, eventPublisher));

            // Configure MedicalAssessment state
            stateMachine.Configure(LodState.MedicalAssessment)
                .OnEntry(() => OnStateEntry(LodState.MedicalAssessment, lodCase, eventPublisher))
                .Permit(LodTrigger.AssessmentDone, LodState.CommanderReview)
                .OnExit(() => OnStateExit(LodState.MedicalAssessment, lodCase, eventPublisher));

            // Configure CommanderReview state - dynamic routing based on case requirements
            stateMachine.Configure(LodState.CommanderReview)
                .OnEntry(() => OnStateEntry(LodState.CommanderReview, lodCase, eventPublisher))
                .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, () => lodCase.RequiresLegalReview)
                .Permit(LodTrigger.SkipToAdjudication, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.CommanderReview, lodCase, eventPublisher));

            // Configure OptionalLegal state
            stateMachine.Configure(LodState.OptionalLegal)
                .OnEntry(() => OnStateEntry(LodState.OptionalLegal, lodCase, eventPublisher))
                .PermitIf(LodTrigger.LegalDone, LodState.OptionalWing, () => lodCase.RequiresWingReview)
                .Permit(LodTrigger.SkipWingReview, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.OptionalLegal, lodCase, eventPublisher));

            // Configure OptionalWing state
            stateMachine.Configure(LodState.OptionalWing)
                .OnEntry(() => OnStateEntry(LodState.OptionalWing, lodCase, eventPublisher))
                .Permit(LodTrigger.WingDone, LodState.BoardAdjudication)
                .OnExit(() => OnStateExit(LodState.OptionalWing, lodCase, eventPublisher));

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .OnEntry(() => OnStateEntry(LodState.BoardAdjudication, lodCase, eventPublisher))
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination)
                .OnExit(() => OnStateExit(LodState.BoardAdjudication, lodCase, eventPublisher));

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .OnEntry(() => OnStateEntry(LodState.Determination, lodCase, eventPublisher))
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification)
                .OnExit(() =>
                {
                    // Publish determination finalized event
                    eventPublisher?.Publish(new LodDeterminationFinalizedEvent
                    {
                        CaseId = lodCase.Id,
                        CaseNumber = lodCase.CaseNumber,
                        Determination = "In Line of Duty",
                        ApprovingAuthority = "HQ AFRC/A1"
                    });
                    OnStateExit(LodState.Determination, lodCase, eventPublisher);
                });

            // Configure Notification state
            stateMachine.Configure(LodState.Notification)
                .OnEntry(() =>
                {
                    // Auto-send notification when entering this state
                    SendNotification(lodCase);
                    OnStateEntry(LodState.Notification, lodCase, eventPublisher);
                })
                .Permit(LodTrigger.AppealFiled, LodState.Appeal)
                .Permit(LodTrigger.NotificationComplete, LodState.End)
                .OnExit(() => OnStateExit(LodState.Notification, lodCase, eventPublisher));

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .OnEntry(() =>
                {
                    eventPublisher?.Publish(new LodAppealFiledEvent
                    {
                        CaseId = lodCase.Id,
                        CaseNumber = lodCase.CaseNumber,
                        AppealDate = DateTime.UtcNow,
                        MemberId = lodCase.MemberId
                    });
                    OnStateEntry(LodState.Appeal, lodCase, eventPublisher);
                })
                .Permit(LodTrigger.AppealResolved, LodState.End)
                .OnExit(() => OnStateExit(LodState.Appeal, lodCase, eventPublisher));

            // Configure End state
            stateMachine.Configure(LodState.End)
                .OnEntry(() => OnStateEntry(LodState.End, lodCase, eventPublisher));

            return stateMachine;
        }

        private static void OnStateEntry(LodState state, InformalLineOfDuty lodCase, IEventPublisher? eventPublisher)
        {
            Console.WriteLine($"[ENTRY] Entering state: {state} for case {lodCase.CaseNumber}");
        }

        private static void OnStateExit(LodState state, InformalLineOfDuty lodCase, IEventPublisher? eventPublisher)
        {
            Console.WriteLine($"[EXIT] Exiting state: {state} for case {lodCase.CaseNumber}");
        }

        private static void SendNotification(InformalLineOfDuty lodCase)
        {
            Console.WriteLine($"[NOTIFICATION] Sending email to member {lodCase.MemberName} ({lodCase.MemberId}) for case {lodCase.CaseNumber}");
        }
    }
}
