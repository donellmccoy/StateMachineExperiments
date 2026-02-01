using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;
using StateMachineExperiments.Common.Infrastructure;
using System;
using System.Collections.Generic;
using Xunit;

namespace StateMachineExperiments.Tests.InformalLOD
{
    public class LodBusinessRuleServiceTests
    {
        private readonly LodBusinessRuleService _service;

        public LodBusinessRuleServiceTests()
        {
            var settings = new BusinessRulesSettings();
            _service = new LodBusinessRuleService(settings);
        }

        [Theory]
        [InlineData(6, 40000, true)]  // High severity
        [InlineData(3, 60000, true)]  // High cost
        [InlineData(8, 100000, true)] // Both high
        [InlineData(3, 40000, false)] // Neither high
        [InlineData(5, 50000, false)] // At threshold (not greater than)
        public void RequiresLegalReview_ShouldReturnCorrectResult(int severity, decimal cost, bool expected)
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                InjurySeverity = severity,
                EstimatedCost = cost
            };

            // Act
            var result = _service.RequiresLegalReview(lodCase);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(8, 80000, true)]   // High severity
        [InlineData(5, 120000, true)]  // High cost
        [InlineData(9, 200000, true)]  // Both high
        [InlineData(5, 80000, false)]  // Neither high
        [InlineData(7, 100000, false)] // At threshold
        public void RequiresWingReview_ShouldReturnCorrectResult(int severity, decimal cost, bool expected)
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                InjurySeverity = severity,
                EstimatedCost = cost
            };

            // Act
            var result = _service.RequiresWingReview(lodCase);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsAppealEligible_WithinDeadline_ShouldReturnTrue()
        {
            // Arrange
            var notificationDate = DateTime.UtcNow.AddDays(-20);
            var lodCase = new InformalLineOfDuty
            {
                TransitionHistory = new List<StateTransitionHistory>
                {
                    new StateTransitionHistory
                    {
                        ToState = LodState.Notification,
                        Timestamp = notificationDate
                    }
                }
            };

            var appealDate = DateTime.UtcNow;

            // Act
            var result = _service.IsAppealEligible(lodCase, appealDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsAppealEligible_AfterDeadline_ShouldReturnFalse()
        {
            // Arrange
            var notificationDate = DateTime.UtcNow.AddDays(-35);
            var lodCase = new InformalLineOfDuty
            {
                TransitionHistory = new List<StateTransitionHistory>
                {
                    new StateTransitionHistory
                    {
                        ToState = LodState.Notification,
                        Timestamp = notificationDate
                    }
                }
            };

            var appealDate = DateTime.UtcNow;

            // Act
            var result = _service.IsAppealEligible(lodCase, appealDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ApplyBusinessRules_ShouldSetCorrectFlags()
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                InjurySeverity = 8,
                EstimatedCost = 120000
            };

            // Act
            _service.ApplyBusinessRules(lodCase);

            // Assert
            Assert.True(lodCase.RequiresLegalReview);
            Assert.True(lodCase.RequiresWingReview);
        }
    }
}
