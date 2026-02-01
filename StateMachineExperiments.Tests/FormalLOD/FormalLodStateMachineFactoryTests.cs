using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StateMachineExperiments.Tests.FormalLOD
{
    public class FormalLodStateMachineFactoryTests
    {
        private readonly IFormalLodStateMachineFactory _factory;
        private readonly INotificationService _notificationService;

        public FormalLodStateMachineFactoryTests()
        {
            var mockSmtpService = new Mock<ISmtpService>();
            _notificationService = new NotificationService(mockSmtpService.Object);
            _factory = new FormalLodStateMachineFactory(_notificationService);
        }

        [Theory]
        [ClassData(typeof(AllStatesTestData))]
        public void CreateStateMachine_ForEachState_ShouldCreateValidStateMachine(FormalLodState state)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = state,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);

            // Assert
            Assert.NotNull(stateMachine);
            Assert.Equal(state, stateMachine.State);
        }

        [Theory]
        [ClassData(typeof(StateTransitionsTestData))]
        public void CreateStateMachine_PermittedTriggers_ShouldMatchExpectedTransitions(
            FormalLodState state, List<FormalLodTrigger> expectedTriggers)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = state,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);
            var permittedTriggers = stateMachine.PermittedTriggers.ToList();

            // Assert
            Assert.Equal(expectedTriggers.Count, permittedTriggers.Count);
            foreach (var expectedTrigger in expectedTriggers)
            {
                Assert.Contains(expectedTrigger, permittedTriggers);
            }
        }

        [Fact]
        public void CreateStateMachine_StartState_ShouldOnlyPermitProcessInitiated()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Start,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);
            var permittedTriggers = stateMachine.PermittedTriggers.ToList();

            // Assert
            Assert.Single(permittedTriggers);
            Assert.Contains(FormalLodTrigger.ProcessInitiated, permittedTriggers);
        }

        [Fact]
        public void CreateStateMachine_EndState_ShouldHaveNoPermittedTriggers()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.End,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);
            var permittedTriggers = stateMachine.PermittedTriggers.ToList();

            // Assert
            Assert.Empty(permittedTriggers);
        }

        [Fact]
        public void CreateStateMachine_NotificationState_ShouldPermitAppealOrNoAppeal()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Notification,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);
            var permittedTriggers = stateMachine.PermittedTriggers.ToList();

            // Assert
            Assert.Equal(2, permittedTriggers.Count);
            Assert.Contains(FormalLodTrigger.AppealRequested, permittedTriggers);
            Assert.Contains(FormalLodTrigger.NoAppealRequested, permittedTriggers);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateStateMachine_WithDifferentDeathCaseFlags_ShouldCreateValidStateMachine(bool isDeathCase)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Start,
                IsDeathCase = isDeathCase,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            // Act
            var stateMachine = _factory.CreateStateMachine(lodCase);

            // Assert
  Assert.NotNull(stateMachine);
            Assert.Equal(FormalLodState.Start, stateMachine.State);
        }
    }

    // ClassData providing all states for comprehensive testing
    public class AllStatesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { FormalLodState.Start };
            yield return new object[] { FormalLodState.MemberReports };
            yield return new object[] { FormalLodState.FormalInitiation };
            yield return new object[] { FormalLodState.AppointingOfficer };
            yield return new object[] { FormalLodState.Investigation };
            yield return new object[] { FormalLodState.WingLegalReview };
            yield return new object[] { FormalLodState.WingCommanderReview };
            yield return new object[] { FormalLodState.BoardAdjudication };
            yield return new object[] { FormalLodState.Determination };
            yield return new object[] { FormalLodState.Notification };
            yield return new object[] { FormalLodState.Appeal };
            yield return new object[] { FormalLodState.End };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // ClassData providing state transitions for testing
    public class StateTransitionsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                FormalLodState.Start,
                new List<FormalLodTrigger> { FormalLodTrigger.ProcessInitiated }
            };

            yield return new object[]
            {
                FormalLodState.MemberReports,
                new List<FormalLodTrigger> { FormalLodTrigger.ConditionReported }
            };

            yield return new object[]
            {
                FormalLodState.FormalInitiation,
                new List<FormalLodTrigger> { FormalLodTrigger.QuestionableDetected }
            };

            yield return new object[]
            {
                FormalLodState.AppointingOfficer,
                new List<FormalLodTrigger> { FormalLodTrigger.OfficerAppointed }
            };

            yield return new object[]
            {
                FormalLodState.Investigation,
                new List<FormalLodTrigger> { FormalLodTrigger.InvestigationComplete }
            };

            yield return new object[]
            {
                FormalLodState.WingLegalReview,
                new List<FormalLodTrigger> { FormalLodTrigger.LegalReviewComplete }
            };

            yield return new object[]
            {
                FormalLodState.WingCommanderReview,
                new List<FormalLodTrigger> { FormalLodTrigger.WingReviewComplete }
            };

            yield return new object[]
            {
                FormalLodState.BoardAdjudication,
                new List<FormalLodTrigger> { FormalLodTrigger.AdjudicationComplete }
            };

            yield return new object[]
            {
                FormalLodState.Determination,
                new List<FormalLodTrigger> { FormalLodTrigger.DeterminationFinalized }
            };

            yield return new object[]
            {
                FormalLodState.Notification,
                new List<FormalLodTrigger>
                {
                    FormalLodTrigger.AppealRequested,
                    FormalLodTrigger.NoAppealRequested
                }
            };

            yield return new object[]
            {
                FormalLodState.Appeal,
                new List<FormalLodTrigger> { FormalLodTrigger.AppealResolved }
            };

            yield return new object[]
            {
                FormalLodState.End,
                new List<FormalLodTrigger>() // No permitted triggers from End state
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
