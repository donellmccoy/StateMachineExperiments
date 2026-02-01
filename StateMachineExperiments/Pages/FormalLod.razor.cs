using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Pages
{
    public partial class FormalLod : ComponentBase
    {
        [Inject]
        public required IFormalLodStateMachineService FormalLodService { get; set; }

        [Inject]
        public required IFormalLodDataService DataService { get; set; }

        [Inject]
        public required ILogger<FormalLod> Logger { get; set; }

        [Inject]
        public required NotificationService NotificationService { get; set; }

        [Inject]
        public required DialogService DialogService { get; set; }

        private RadzenDataGrid<FormalLineOfDuty>? casesGrid;
        private FormalLineOfDuty? selectedCase;
        private IList<FormalLineOfDuty>? selectedCases;
        private List<FormalLineOfDuty> allCases = new();
        private List<FormalStateTransitionHistory> caseHistory = new();
        private List<string> permittedTriggers = new();
        private bool isLoading = true;

        private string newCaseNumber = string.Empty;
        private string newMemberId = string.Empty;
        private string newMemberName = string.Empty;
        private bool isDeathCase = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadAllCases();
            isLoading = false;
        }

        private async Task LoadAllCases()
        {
            try
            {
                allCases = (await DataService.GetAllCasesAsync()).ToList();
                await casesGrid?.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading all formal cases");
                ShowNotification("Error loading cases", GetUserFriendlyErrorMessage(ex), NotificationSeverity.Error);
            }
        }

        private async Task CreateNewCase()
        {
            if (string.IsNullOrWhiteSpace(newCaseNumber))
            {
                ShowNotification("Validation Error", "Please enter a case number.", NotificationSeverity.Warning);
                return;
            }

            try
            {
                var newCase = await FormalLodService.CreateNewCaseAsync(newCaseNumber, newMemberId, newMemberName, isDeathCase);
                ShowNotification("Success", $"Formal case {newCase.CaseNumber} created successfully!", NotificationSeverity.Success);
                
                newCaseNumber = string.Empty;
                newMemberId = string.Empty;
                newMemberName = string.Empty;
                isDeathCase = false;
                
                await LoadAllCases();
                await SelectCase(newCase);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating formal case {CaseNumber}", newCaseNumber);
                ShowNotification("Error", $"Error creating case: {GetUserFriendlyErrorMessage(ex)}", NotificationSeverity.Error);
            }
        }

        private async Task OnCaseSelected(FormalLineOfDuty lodCase)
        {
            await SelectCase(lodCase);
        }

        private async Task SelectCase(FormalLineOfDuty lodCase)
        {
            try
            {
                selectedCase = await FormalLodService.GetCaseAsync(lodCase.Id);
                if (selectedCase != null)
                {
                    caseHistory = await FormalLodService.GetCaseHistoryAsync(lodCase.Id);
                    permittedTriggers = await FormalLodService.GetPermittedTriggersAsync(lodCase.Id);
                }
                else
                {
                    ShowNotification("Not Found", "Case not found.", NotificationSeverity.Warning);
                }
            }
            catch (CaseNotFoundException)
            {
                Logger.LogWarning("Formal case with ID {CaseId} not found", lodCase.Id);
                ShowNotification("Not Found", "The requested case could not be found.", NotificationSeverity.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading formal case {CaseId}", lodCase.Id);
                ShowNotification("Error", $"Error loading case: {GetUserFriendlyErrorMessage(ex)}", NotificationSeverity.Error);
            }
        }

        private async Task ShowTriggerConfirmation(string triggerName)
        {
            if (selectedCase == null) return;

            var confirmed = await DialogService.Confirm(
                $"Are you sure you want to execute the '{triggerName}' action on case {selectedCase.CaseNumber}?",
                "Confirm Action",
                new ConfirmOptions { OkButtonText = "Yes, Execute", CancelButtonText = "Cancel" });

            if (confirmed == true)
            {
                await FireTrigger(triggerName);
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
                    ShowNotification("Success", $"Trigger '{triggerName}' executed successfully!", NotificationSeverity.Success);
                    
                    await LoadAllCases();
                    var updatedCase = allCases.FirstOrDefault(c => c.Id == selectedCase.Id);
                    if (updatedCase != null)
                    {
                        await SelectCase(updatedCase);
                    }
                }
            }
            catch (InvalidStateTransitionException ex)
            {
                Logger.LogWarning(ex, "Invalid state transition for formal case {CaseNumber}", selectedCase.CaseNumber);
                ShowNotification("Invalid Action", 
                    $"This action is not available in the current state. Permitted actions: {string.Join(", ", permittedTriggers)}", 
                    NotificationSeverity.Warning);
            }
            catch (TransitionValidationException ex)
            {
                Logger.LogWarning(ex, "Transition validation failed for formal case {CaseNumber}", selectedCase.CaseNumber);
                ShowNotification("Validation Failed", $"Validation failed: {ex.Message}", NotificationSeverity.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error firing trigger {Trigger} for formal case {CaseNumber}", triggerName, selectedCase.CaseNumber);
                ShowNotification("Error", $"Error: {GetUserFriendlyErrorMessage(ex)}", NotificationSeverity.Error);
            }
        }

        private BadgeStyle GetStateBadgeStyle(FormalLodState state)
        {
            return state switch
            {
                FormalLodState.Start => BadgeStyle.Secondary,
                FormalLodState.MemberReports => BadgeStyle.Info,
                FormalLodState.FormalInitiation => BadgeStyle.Info,
                FormalLodState.AppointingOfficer => BadgeStyle.Primary,
                FormalLodState.Investigation => BadgeStyle.Primary,
                FormalLodState.WingLegalReview => BadgeStyle.Warning,
                FormalLodState.WingCommanderReview => BadgeStyle.Warning,
                FormalLodState.BoardAdjudication => BadgeStyle.Primary,
                FormalLodState.Determination => BadgeStyle.Info,
                FormalLodState.Notification => BadgeStyle.Info,
                FormalLodState.Appeal => BadgeStyle.Danger,
                FormalLodState.End => BadgeStyle.Success,
                _ => BadgeStyle.Light
            };
        }

        private void ShowNotification(string summary, string detail, NotificationSeverity severity)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = 4000
            });
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
