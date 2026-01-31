using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Common.Infrastructure;
using Moq;
using StateMachineExperiments.Modules.InformalLOD.Events;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StateMachineExperiments.Modules.InformalLOD.Tests
{
    public class LodStateMachineServiceTests
    {
        private readonly Mock<ILodDataService> _mockDataService;
        private readonly Mock<ILodBusinessRuleService> _mockBusinessRules;
        private readonly Mock<ILodTransitionValidator> _mockValidator;
        private readonly ILodStateMachineFactory _stateMachineFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly LodStateMachineService _service;

        public LodStateMachineServiceTests()
        {
            _mockDataService = new Mock<ILodDataService>();
            _mockBusinessRules = new Mock<ILodBusinessRuleService>();
            _mockValidator = new Mock<ILodTransitionValidator>();
            _stateMachineFactory = new LodStateMachineFactory();
            _eventPublisher = new InMemoryEventPublisher();

            _service = new LodStateMachineService(
                _mockDataService.Object,
                _mockBusinessRules.Object,
                _mockValidator.Object,
                _stateMachineFactory,
                _eventPublisher);
        }

        [Fact]
        public async Task CreateNewCaseAsync_ShouldCreateCaseAndPublishEvent()
        {
            // Arrange
            var expectedCase = new InformalLineOfDuty
            {
                Id = 1,
                CaseNumber = "TEST-001",
                MemberId = "M123",
                MemberName = "Test User",
                CurrentState = nameof(LodState.Start)
            };

            _mockDataService
                .Setup(x => x.CreateNewCaseAsync("TEST-001", "M123", "Test User"))
                .ReturnsAsync(expectedCase);

            // Act
            var result = await _service.CreateNewCaseAsync("TEST-001", "M123", "Test User");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST-001", result.CaseNumber);
            _mockDataService.Verify(x => x.CreateNewCaseAsync("TEST-001", "M123", "Test User"), Times.Once);
        }

        [Fact]
        public async Task FireTriggerAsync_WithValidTransition_ShouldUpdateState()
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                Id = 1,
                CaseNumber = "TEST-001",
                CurrentState = nameof(LodState.Start),
                TransitionHistory = new List<StateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockDataService.Setup(x => x.UpdateCaseAsync(It.IsAny<InformalLineOfDuty>())).Returns(Task.CompletedTask);
            _mockDataService.Setup(x => x.AddTransitionHistoryAsync(It.IsAny<StateTransitionHistory>())).Returns(Task.CompletedTask);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<InformalLineOfDuty>(), It.IsAny<LodTrigger>()))
                .ReturnsAsync(ValidationResult.Success());

            // Act
            await _service.FireTriggerAsync(1, LodTrigger.ProcessInitiated);

            // Assert
            _mockDataService.Verify(x => x.UpdateCaseAsync(It.Is<InformalLineOfDuty>(c => c.CurrentState == nameof(LodState.MemberReports))), Times.Once);
            _mockDataService.Verify(x => x.AddTransitionHistoryAsync(It.IsAny<StateTransitionHistory>()), Times.Once);
        }

        [Fact]
        public async Task FireTriggerAsync_WithInvalidTransition_ShouldThrowException()
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                Id = 1,
                CaseNumber = "TEST-001",
                CurrentState = nameof(LodState.Start),
                TransitionHistory = new List<StateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<InformalLineOfDuty>(), It.IsAny<LodTrigger>()))
                .ReturnsAsync(ValidationResult.Success());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidStateTransitionException>(
                () => _service.FireTriggerAsync(1, LodTrigger.AssessmentDone));
        }

        [Fact]
        public async Task CommanderReview_WithLegalReviewRequired_ShouldRouteToLegal()
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                Id = 1,
                CaseNumber = "TEST-001",
                CurrentState = nameof(LodState.CommanderReview),
                RequiresLegalReview = true,
                TransitionHistory = new List<StateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);
            _mockDataService.Setup(x => x.UpdateCaseAsync(It.IsAny<InformalLineOfDuty>())).Returns(Task.CompletedTask);
            _mockDataService.Setup(x => x.AddTransitionHistoryAsync(It.IsAny<StateTransitionHistory>())).Returns(Task.CompletedTask);
            _mockValidator.Setup(x => x.ValidateTransitionAsync(It.IsAny<InformalLineOfDuty>(), It.IsAny<LodTrigger>()))
                .ReturnsAsync(ValidationResult.Success());

            // Act
            await _service.FireTriggerAsync(1, LodTrigger.ReviewFinished);

            // Assert
            _mockDataService.Verify(x => x.UpdateCaseAsync(It.Is<InformalLineOfDuty>(c => c.CurrentState == nameof(LodState.OptionalLegal))), Times.Once);
        }

        [Fact]
        public async Task GetPermittedTriggersAsync_ShouldReturnCorrectTriggers()
        {
            // Arrange
            var lodCase = new InformalLineOfDuty
            {
                Id = 1,
                CaseNumber = "TEST-001",
                CurrentState = nameof(LodState.Start),
                TransitionHistory = new List<StateTransitionHistory>()
            };

            _mockDataService.Setup(x => x.GetCaseAsync(1)).ReturnsAsync(lodCase);

            // Act
            var triggers = await _service.GetPermittedTriggersAsync(1);

            // Assert
            Assert.Single(triggers);
            Assert.Contains(nameof(LodTrigger.ProcessInitiated), triggers);
        }
    }
}

