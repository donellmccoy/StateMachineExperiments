using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Pages
{
    public partial class FormalLod : ComponentBase
    {
        [Inject]
        public required IFormalLodStateMachineService FormalLodService { get; set; }

        [Inject]
        public required ILogger<FormalLod> Logger { get; set; }

        private FormalLineOfDuty? selectedCase;
        private List<FormalStateTransitionHistory> caseHistory = new();
        private List<string> permittedTriggers = new();
        private bool isLoading = true;
        private string message = string.Empty;
        private string messageClass = string.Empty;

        private string newCaseNumber = string.Empty;
        private string newMemberId = string.Empty;
        private string newMemberName = string.Empty;
        private bool isDeathCase = false;

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
                var newCase = await FormalLodService.CreateNewCaseAsync(newCaseNumber, newMemberId, newMemberName, isDeathCase);
                ShowMessage($"Formal case {newCase.CaseNumber} created successfully!", "alert-success");
                newCaseNumber = string.Empty;
                newMemberId = string.Empty;
                newMemberName = string.Empty;
                isDeathCase = false;
                await SelectCaseById(newCase.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating formal case {CaseNumber}", newCaseNumber);
                ShowMessage($"Error creating case: {GetUserFriendlyErrorMessage(ex)}", "alert-danger");
            }
        }

        private async Task SelectCaseById(int caseId)
        {
            try
            {
                selectedCase = await FormalLodService.GetCaseAsync(caseId);
                if (selectedCase != null)
                {
                    caseHistory = await FormalLodService.GetCaseHistoryAsync(caseId);
                    permittedTriggers = await FormalLodService.GetPermittedTriggersAsync(caseId);
                    message = string.Empty;
                }
                else
                {
                    ShowMessage("Case not found.", "alert-warning");
                }
            }
            catch (CaseNotFoundException)
            {
                Logger.LogWarning("Formal case with ID {CaseId} not found", caseId);
                ShowMessage("The requested case could not be found.", "alert-warning");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading formal case {CaseId}", caseId);
                ShowMessage($"Error loading case: {GetUserFriendlyErrorMessage(ex)}", "alert-danger");
            }
        }

        private async Task FireTrigger(string triggerName)
        {
            if (selectedCase == null) return;

            try
            {
                if (Enum.TryParse<FormalLodTrigger>(triggerName, out var trigger))
                {
                    await FormalLodService.FireTriggerAsync(selectedCase.Id, trigger);
                    ShowMessage($"Trigger '{triggerName}' executed successfully!", "alert-success");
                    await SelectCaseById(selectedCase.Id);
                }
            }
            catch (InvalidStateTransitionException ex)
            {
                Logger.LogWarning(ex, "Invalid state transition for formal case {CaseNumber}", selectedCase.CaseNumber);
                ShowMessage($"This action is not available in the current state. Permitted actions: {string.Join(", ", permittedTriggers)}", "alert-warning");
            }
            catch (TransitionValidationException ex)
            {
                Logger.LogWarning(ex, "Transition validation failed for formal case {CaseNumber}", selectedCase.CaseNumber);
                ShowMessage($"Validation failed: {ex.Message}", "alert-warning");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error firing trigger {Trigger} for formal case {CaseNumber}", triggerName, selectedCase.CaseNumber);
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
