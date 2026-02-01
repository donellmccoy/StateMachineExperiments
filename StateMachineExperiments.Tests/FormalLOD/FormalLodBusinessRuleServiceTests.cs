using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace StateMachineExperiments.Tests.FormalLOD
{
    public class FormalLodBusinessRuleServiceTests
    {
        private readonly FormalLodBusinessRuleService _service;

        public FormalLodBusinessRuleServiceTests()
        {
            _service = new FormalLodBusinessRuleService();
        }

        [Theory]
        [InlineData(true, true, true)]   // Toxicology required and complete
        [InlineData(false, false, true)] // Toxicology not required
        [InlineData(false, true, true)]  // Toxicology not required but complete anyway
        [InlineData(true, false, false)] // Toxicology required but not complete
        public void CanProceedFromInvestigation_ShouldReturnCorrectResult(bool toxRequired, bool toxComplete, bool expected)
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                ToxicologyRequired = toxRequired,
                ToxicologyComplete = toxComplete
            };

            // Act
            var result = _service.CanProceedFromInvestigation(lodCase);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [ClassData(typeof(AppealEligibilityTestData))]
        public void IsAppealEligible_ShouldReturnCorrectResult(bool isDeathCase, int daysAgo, bool expected)
        {
            // Arrange
            var notificationDate = DateTime.UtcNow.AddDays(-daysAgo);
            var lodCase = new FormalLineOfDuty
            {
                IsDeathCase = isDeathCase,
                TransitionHistory = new List<FormalStateTransitionHistory>
                {
                    new FormalStateTransitionHistory
                    {
                        ToState = nameof(FormalLodState.Notification),
                        Timestamp = notificationDate
                    }
                }
            };

            var appealDate = DateTime.UtcNow;

            // Act
            var result = _service.IsAppealEligible(lodCase, appealDate);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsAppealEligible_NoNotificationTransition_ShouldReturnFalse()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                TransitionHistory = new List<FormalStateTransitionHistory>()
            };

            var appealDate = DateTime.UtcNow;

            // Act
            var result = _service.IsAppealEligible(lodCase, appealDate);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(false, 25, true)]  // Standard case, within 30 days
        [InlineData(false, 30, true)]  // Standard case, at 30 days
        [InlineData(false, 31, false)] // Standard case, after 30 days
        [InlineData(true, 100, true)]  // Death case, within 180 days
        [InlineData(true, 180, true)]  // Death case, at 180 days
        [InlineData(true, 181, false)] // Death case, after 180 days
        public void IsAppealEligible_VariousScenarios_ShouldReturnCorrectResult(bool isDeathCase, int daysAgo, bool expected)
        {
            // Arrange
            var notificationDate = DateTime.UtcNow.AddDays(-daysAgo);
            var lodCase = new FormalLineOfDuty
            {
                IsDeathCase = isDeathCase,
                TransitionHistory = new List<FormalStateTransitionHistory>
                {
                    new FormalStateTransitionHistory
                    {
                        ToState = nameof(FormalLodState.Notification),
                        Timestamp = notificationDate
                    }
                }
            };

            var appealDate = DateTime.UtcNow;

            // Act
            var result = _service.IsAppealEligible(lodCase, appealDate);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RequiresToxicology_ShouldReturnFlagValue()
        {
            // Arrange
            var lodCaseRequired = new FormalLineOfDuty { ToxicologyRequired = true };
            var lodCaseNotRequired = new FormalLineOfDuty { ToxicologyRequired = false };

            // Act
            var resultRequired = _service.RequiresToxicology(lodCaseRequired);
            var resultNotRequired = _service.RequiresToxicology(lodCaseNotRequired);

            // Assert
            Assert.True(resultRequired);
            Assert.False(resultNotRequired);
        }

        [Fact]
        public void ApplyBusinessRules_ShouldExecuteWithoutErrors()
        {
            // Arrange
            var lodCase = new FormalLineOfDuty
            {
                ToxicologyRequired = true,
                IsDeathCase = true
            };

            // Act
            var exception = Record.Exception(() => _service.ApplyBusinessRules(lodCase));

            // Assert
            Assert.Null(exception);
        }
    }

    // ClassData provider for appeal eligibility test cases
    public class AppealEligibilityTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Standard cases (30-day window)
            yield return new object[] { false, 10, true };   // Within deadline
            yield return new object[] { false, 29, true };   // Just before deadline
            yield return new object[] { false, 30, true };   // At deadline
            yield return new object[] { false, 31, false };  // Just after deadline
            yield return new object[] { false, 60, false };  // Well after deadline

            // Death cases (180-day window for next of kin)
            yield return new object[] { true, 30, true };    // Within extended window
            yield return new object[] { true, 90, true };    // Mid extended window
            yield return new object[] { true, 179, true };   // Just before extended deadline
            yield return new object[] { true, 180, true };   // At extended deadline
            yield return new object[] { true, 181, false };  // Just after extended deadline
            yield return new object[] { true, 200, false };  // Well after extended deadline
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
