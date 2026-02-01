using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Common.Infrastructure;
using Moq;
using StateMachineExperiments.Modules.FormalLOD.Events;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StateMachineExperiments.Tests.FormalLOD
{
    public class FormalLodStateMachineServiceTests
    {
        private readonly Mock<IFormalLodDataService> _mockDataService;
        private readonly Mock<IFormalLodBusinessRuleService> _mockBusinessRules;
        private readonly Mock<IFormalLodTransitionValidator> _mockValidator;
        private readonly IFormalLodStateMachineFactory _stateMachineFactory;
        private readonly INotificationService _notificationService;
        private readonly FormalLodStateMachineService _service;

        public FormalLodStateMachineServiceTests()
        {
            _mockDataService = new Mock<IFormalLodDataService>();
            _mockBusinessRules = new Mock<IFormalLodBusinessRuleService>();
            _mockValidator = new Mock<IFormalLodTransitionValidator>();
            _notificationService = new NotificationService();
            _stateMachineFactory = new FormalLodStateMachineFactory(_notificationService);

            _service = new FormalLodStateMachineService(
                _mockDataService.Object,
                _mockBusinessRules.Object,
                _mockValidator.Object,
                _stateMachineFactory,
                _notificationService);
        }

        [Theory]
        [InlineData("FORMAL-001", "M123", "John Doe", false)]
        [InlineData("FORMAL-002", "M456", "Jane Smith", true)]
        [InlineData("DEATH-001", null, "Robert Lee", true)]
        public async Task CreateNewCaseAsync_ShouldCreateCaseWithCorrectProperties(
            string caseNumber, string memberId, string memberName, bool isDeathCase)
        {
            // Arrange
            var expectedCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                IsDeathCase = isDeathCase,
                CurrentState = nameof(FormalLodState.Start)
            };

            _mockDataService
                .Setup(x => x.CreateNewCaseAsync(caseNumber, memberId, memberName, isDeathCase))
                .ReturnsAsync(expectedCase);

            // Act
            var result = await _service.CreateNewCaseAsync(caseNumber, memberId, memberName, isDeathCase);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(caseNumber, result.CaseNumber);
            Assert.Equal(memberId, result.MemberId);
            Assert.Equal(memberName, result.MemberName);
            Assert.Equal(isDeathCase, result.IsDeathCase);
            _mockDataService.Verify(x => x.CreateNewCaseAsync(caseNumber, memberId, memberName, isDeathCase), Times.Once);
        }

        [Fact]
        public async Task FireTriggerAsync_WithValidTransition_ShouldUpdateState()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = nameof(FormalLodState.Start),
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockDataService.Setup(x => x.UpdateCaseAsync(It.IsAny<FormalLineOfDuty>())).Returns(Task.CompletedTask);
            _mockDataService.Setup(x => x.AddTransitionHistoryAsync(It.IsAny<FormalStateTransitionHistory>())).Returns(Task.CompletedTask);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<FormalLineOfDuty>(), It.IsAny<FormalLodTrigger>()))
                .ReturnsAsync(ValidationResult.Success());

            // Act
            await _service.FireTriggerAsync(1, FormalLodTrigger.ProcessInitiated);

            // Assert
            _mockDataService.Verify(x => x.UpdateCaseAsync(
                It.Is<FormalLineOfDuty>(c => c.CurrentState == nameof(FormalLodState.MemberReports))), Times.Once);
            _mockDataService.Verify(x => x.AddTransitionHistoryAsync(It.IsAny<FormalStateTransitionHistory>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(InvalidTransitionTestData))]
        public async Task FireTriggerAsync_WithInvalidTransition_ShouldThrowException(
            FormalLodState currentState, FormalLodTrigger trigger)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = currentState.ToString(),
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<FormalLineOfDuty>(), It.IsAny<FormalLodTrigger>()))
                .ReturnsAsync(ValidationResult.Success());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidStateTransitionException>(
                () => _service.FireTriggerAsync(1, trigger));
        }

        [Fact]
        public async Task FireTriggerAsync_NonExistentCase_ShouldThrowCaseNotFoundException()
        {
            // Arrange
            _mockDataService.Setup(x => x.GetCaseAsync(999)).ReturnsAsync((FormalLineOfDuty)null);

            // Act & Assert
            await Assert.ThrowsAsync<CaseNotFoundException>(
                () => _service.FireTriggerAsync(999, FormalLodTrigger.ProcessInitiated));
        }

        [Fact]
        public async Task FireTriggerAsync_FailedValidation_ShouldThrowTransitionValidationException()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = nameof(FormalLodState.Investigation),
                ToxicologyRequired = true,
                ToxicologyComplete = false,
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<FormalLineOfDuty>(), FormalLodTrigger.InvestigationComplete))
                .ReturnsAsync(ValidationResult.Failure("Toxicology reports not received"));

            // Act & Assert
            await Assert.ThrowsAsync<TransitionValidationException>(
                () => _service.FireTriggerAsync(1, FormalLodTrigger.InvestigationComplete));
        }

        [Theory]
        [InlineData(FormalLodState.Start, "ProcessInitiated")]
        [InlineData(FormalLodState.MemberReports, "ConditionReported")]
        [InlineData(FormalLodState.FormalInitiation, "QuestionableDetected")]
        [InlineData(FormalLodState.AppointingOfficer, "OfficerAppointed")]
        public async Task GetPermittedTriggersAsync_ShouldReturnCorrectTriggers(
            FormalLodState state, string expectedTrigger)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = state.ToString(),
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);

            // Act
            var triggers = await _service.GetPermittedTriggersAsync(1);

            // Assert
            Assert.Single(triggers);
            Assert.Contains(expectedTrigger, triggers);
        }

        [Theory]
        [ClassData(typeof(StateToAuthorityMappingTestData))]
        public void GetCurrentAuthority_ShouldReturnCorrectAuthority(
            FormalLodState state, string expectedAuthority)
        {
            // Act
            var authority = _service.GetCurrentAuthority(state);

            // Assert
            Assert.Equal(expectedAuthority, authority);
        }

        [Fact]
        public async Task GetCaseAsync_ShouldReturnCase()
        {
            // Arrange
            var expectedCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = nameof(FormalLodState.Investigation)
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(expectedCase);

            // Act
            var result = await _service.GetCaseAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FORMAL-001", result.CaseNumber);
        }

        [Fact]
        public async Task GetCaseByCaseNumberAsync_ShouldReturnCase()
        {
            // Arrange
            var expectedCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = nameof(FormalLodState.Investigation)
            };

            _mockDataService.Setup(x => x.GetCaseByCaseNumberAsync("FORMAL-001")).ReturnsAsync(expectedCase);

            // Act
            var result = await _service.GetCaseByCaseNumberAsync("FORMAL-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetCaseHistoryAsync_ShouldReturnHistory()
        {
            // Arrange
            var history = new List<FormalStateTransitionHistory>
            {
                new FormalStateTransitionHistory
                {
                    Id = 1,
                    FromState = nameof(FormalLodState.Start),
                    ToState = nameof(FormalLodState.MemberReports),
                    Trigger = nameof(FormalLodTrigger.ProcessInitiated)
                }
            };

            _mockDataService.Setup(x => x.GetCaseHistoryAsync(1)).ReturnsAsync(history);

            // Act
            var result = await _service.GetCaseHistoryAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(nameof(FormalLodState.MemberReports), result[0].ToState);
        }

        [Fact]
        public async Task ValidateTransitionAsync_ShouldCallValidator()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                Id = 1,
                CaseNumber = "FORMAL-001",
                CurrentState = nameof(FormalLodState.Start)
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(lodCase, FormalLodTrigger.ProcessInitiated))
                .ReturnsAsync(ValidationResult.Success());

            // Act
            var result = await _service.ValidateTransitionAsync(1, FormalLodTrigger.ProcessInitiated);

            // Assert
            Assert.True(result.IsValid);
            _mockValidator.Verify(x => x.ValidateTransitionAsync(lodCase, FormalLodTrigger.ProcessInitiated), Times.Once);
        }
    }

    // ClassData for invalid transitions
    public class InvalidTransitionTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { FormalLodState.Start, FormalLodTrigger.InvestigationComplete };
            yield return new object[] { FormalLodState.MemberReports, FormalLodTrigger.LegalReviewComplete };
            yield return new object[] { FormalLodState.Investigation, FormalLodTrigger.ProcessInitiated };
            yield return new object[] { FormalLodState.Determination, FormalLodTrigger.OfficerAppointed };
            yield return new object[] { FormalLodState.End, FormalLodTrigger.ProcessInitiated };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // ClassData for state to authority mapping
    public class StateToAuthorityMappingTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { FormalLodState.Start, "None" };
            yield return new object[] { FormalLodState.MemberReports, "Member" };
            yield return new object[] { FormalLodState.FormalInitiation, "LodMfp" };
            yield return new object[] { FormalLodState.AppointingOfficer, "AppointingAuthority" };
            yield return new object[] { FormalLodState.Investigation, "InvestigatingOfficer" };
            yield return new object[] { FormalLodState.WingLegalReview, "LegalAdvisor" };
            yield return new object[] { FormalLodState.WingCommanderReview, "WingCommander" };
            yield return new object[] { FormalLodState.BoardAdjudication, "ReviewingBoard" };
            yield return new object[] { FormalLodState.Determination, "ApprovingAuthority" };
            yield return new object[] { FormalLodState.Notification, "LodPm" };
            yield return new object[] { FormalLodState.Appeal, "AppellateAuthority" };
            yield return new object[] { FormalLodState.End, "None" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
