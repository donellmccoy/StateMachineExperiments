using Moq;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StateMachineExperiments.Tests.FormalLOD
{
    public class FormalLodTransitionValidatorTests
    {
        private readonly Mock<IFormalLodBusinessRuleService> _mockBusinessRules;
        private readonly FormalLodTransitionValidator _validator;

        public FormalLodTransitionValidatorTests()
        {
            _mockBusinessRules = new Mock<IFormalLodBusinessRuleService>();
            _validator = new FormalLodTransitionValidator(_mockBusinessRules.Object);
        }

        [Fact]
        public async Task ValidateTransition_ProcessInitiatedFromStart_ShouldSucceed()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Start
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.ProcessInitiated);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateTransition_ProcessInitiatedFromNonStartState_ShouldFail()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Investigation
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.ProcessInitiated);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Process can only be initiated from Start state", result.Errors);
        }

        [Fact]
        public async Task ValidateTransition_ConditionReportedWithoutMemberId_ShouldFail()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.MemberReports,
                MemberId = null
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.ConditionReported);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Member ID is required before reporting condition", result.Errors);
        }

        [Fact]
        public async Task ValidateTransition_ConditionReportedWithMemberId_ShouldSucceed()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.MemberReports,
                MemberId = "M123456"
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.ConditionReported);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateTransition_OfficerAppointedWithoutOfficerId_ShouldFail()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.AppointingOfficer,
                InvestigatingOfficerId = null
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.OfficerAppointed);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Investigating officer must be assigned before proceeding", result.Errors);
        }

        [Fact]
        public async Task ValidateTransition_OfficerAppointedWithOfficerId_ShouldSucceed()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.AppointingOfficer,
                InvestigatingOfficerId = "O123456"
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.OfficerAppointed);

            // Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(true, true, true)]   // Can proceed: tox required and complete
        [InlineData(false, false, true)] // Can proceed: tox not required
        [InlineData(true, false, false)] // Cannot proceed: tox required but not complete
        public async Task ValidateTransition_InvestigationComplete_ShouldValidateToxicology(
            bool toxRequired, bool canProceed, bool expectedValid)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Investigation,
                ToxicologyRequired = toxRequired
            };

            _mockBusinessRules
                .Setup(x => x.CanProceedFromInvestigation(lodCase))
                .Returns(canProceed);

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.InvestigationComplete);

            // Assert
            Assert.Equal(expectedValid, result.IsValid);
            if (!expectedValid)
            {
                Assert.Contains("Toxicology reports are required but not yet received", result.Errors);
            }
        }

        [Theory]
        [ClassData(typeof(AppealValidationTestData))]
        public async Task ValidateTransition_AppealRequested_ShouldValidateEligibility(
            bool isEligible, bool expectedValid)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Notification,
                IsDeathCase = false
            };

            _mockBusinessRules
                .Setup(x => x.IsAppealEligible(lodCase, It.IsAny<DateTime>()))
                .Returns(isEligible);

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.AppealRequested);

            // Assert
            Assert.Equal(expectedValid, result.IsValid);
            if (!expectedValid)
            {
                Assert.Contains("Appeal deadline has passed", result.Errors);
            }
        }

        [Fact]
        public async Task ValidateTransition_AppealRequestedForDeathCase_ShouldUse180DayDeadline()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Notification,
                IsDeathCase = true
            };

            _mockBusinessRules
                .Setup(x => x.IsAppealEligible(lodCase, It.IsAny<DateTime>()))
                .Returns(false);

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.AppealRequested);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("180 days", result.Errors[0]);
        }

        [Fact]
        public async Task ValidateTransition_NoAppealRequestedFromNonNotificationState_ShouldFail()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Investigation
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.NoAppealRequested);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Can only close without appeal from Notification state", result.Errors);
        }

        [Fact]
        public async Task ValidateTransition_NoAppealRequestedFromNotificationState_ShouldSucceed()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Notification
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.NoAppealRequested);

            // Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(FormalLodTrigger.QuestionableDetected)]
        [InlineData(FormalLodTrigger.LegalReviewComplete)]
        [InlineData(FormalLodTrigger.WingReviewComplete)]
        [InlineData(FormalLodTrigger.AdjudicationComplete)]
        [InlineData(FormalLodTrigger.DeterminationFinalized)]
        [InlineData(FormalLodTrigger.AppealResolved)]
        public async Task ValidateTransition_TriggersWithoutSpecialValidation_ShouldSucceed(
            FormalLodTrigger trigger)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.Investigation
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, trigger);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateTransition_MultipleErrors_ShouldReturnAllErrors()
        {
            // Arrange - case with missing member ID trying to report condition
            var lodCase = new FormalLineOfDuty
            {
                CurrentState = FormalLodState.MemberReports,
                MemberId = ""  // Empty member ID
            };

            // Act
            var result = await _validator.ValidateTransitionAsync(lodCase, FormalLodTrigger.ConditionReported);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }
    }

    // ClassData for appeal validation test scenarios
    public class AppealValidationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { true, true };   // Eligible, should pass
            yield return new object[] { false, false };  // Not eligible, should fail
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
