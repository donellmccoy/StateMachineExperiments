using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Pages
{
    public partial class InformalLod : ComponentBase
    {
        [Inject]
        public required ILodStateMachineService LodService { get; set; }

        [Inject]
        public required IInformalLineOfDutyDataService DataService { get; set; }

        [Inject]
        public required ILogger<InformalLod> Logger { get; set; }

        private InformalLineOfDuty? selectedCase;
        private List<StateTransitionHistory> caseHistory = new();
        private List<string> permittedTriggers = new();
        private bool isLoading = true;
        private string message = string.Empty;
        private string messageClass = string.Empty;

        private string newCaseNumber = string.Empty;
        private string newMemberId = string.Empty;
        private string newMemberName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            isLoading = false;
        }

        private async Task CreateNewCase()
        {
            if (string.IsNullOrWhiteSpace(newCaseNumber))
            {
                ShowMessage("Please enter a case number.", "alert-warning");
                return;
            }

            try
            {
                var newCase = await LodService.CreateNewCaseAsync(newCaseNumber, newMemberId, newMemberName);
                ShowMessage($"Case {newCase.CaseNumber} created successfully!", "alert-success");
                newCaseNumber = string.Empty;
                newMemberId = string.Empty;
                newMemberName = string.Empty;
                await SelectCaseById(newCase.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating case {CaseNumber}", newCaseNumber);
                ShowMessage($"Error creating case: {GetUserFriendlyErrorMessage(ex)}", "alert-danger");
            }
        }

        private async Task SelectCaseById(int caseId)
        {
            try
            {
                selectedCase = await LodService.GetCaseAsync(caseId);
                if (selectedCase != null)
                {
                    caseHistory = await LodService.GetCaseHistoryAsync(caseId);
                    permittedTriggers = await LodService.GetPermittedTriggersAsync(caseId);
                    message = string.Empty;
                }
                else
                {
                    ShowMessage("Case not found.", "alert-warning");
                }
            }
            catch (CaseNotFoundException)
            {
                Logger.LogWarning("Case with ID {CaseId} not found", caseId);
                ShowMessage("The requested case could not be found.", "alert-warning");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading case {CaseId}", caseId);
                ShowMessage($"Error loading case: {GetUserFriendlyErrorMessage(ex)}", "alert-danger");
            }
        }

        private async Task FireTrigger(string triggerName)
        {
            if (selectedCase == null) return;

            try
            {
                if (Enum.TryParse<LodTrigger>(triggerName, out var trigger))
                {
                    await LodService.FireTriggerAsync(selectedCase.Id, trigger);
                    ShowMessage($"Trigger '{triggerName}' executed successfully!", "alert-success");
                    await SelectCaseById(selectedCase.Id);
                }
            }
            catch (InvalidStateTransitionException ex)
            {
                Logger.LogWarning(ex, "Invalid state transition for case {CaseNumber}", selectedCase.CaseNumber);
                ShowMessage($"This action is not available in the current state. Permitted actions: {string.Join(", ", permittedTriggers)}", "alert-warning");
            }
            catch (TransitionValidationException ex)
            {
                Logger.LogWarning(ex, "Transition validation failed for case {CaseNumber}", selectedCase.CaseNumber);
                ShowMessage($"Validation failed: {ex.Message}", "alert-warning");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error firing trigger {Trigger} for case {CaseNumber}", triggerName, selectedCase.CaseNumber);
                ShowMessage($"Error: {GetUserFriendlyErrorMessage(ex)}", "alert-danger");
            }
        }

        private void ShowMessage(string msg, string cssClass)
        {
            message = msg;
            messageClass = cssClass;
        }

        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            return ex switch
            {
                CaseNotFoundException => "The case could not be found.",
                InvalidStateTransitionException => "This action is not available in the current state.",
                TransitionValidationException => ex.Message,
                MissingRequiredDataException => $"Required information is missing: {ex.Message}",
                _ => "An unexpected error occurred. Please try again."
            };
        }
    }
}
