using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;
using StateMachineExperiments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Inject]
        public required NotificationService NotificationService { get; set; }

        [Inject]
        public required DialogService DialogService { get; set; }

        private RadzenDataGrid<InformalLineOfDuty>? casesGrid;
        private InformalLineOfDuty? selectedCase;
        private IList<InformalLineOfDuty>? selectedCases;
        private List<InformalLineOfDuty> allCases = new();
        private List<StateTransitionHistory> caseHistory = new();
        private List<string> permittedTriggers = new();
        private bool isLoading = true;

        private NewInformalCaseFormModel newCaseModel = new();

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
                Logger.LogError(ex, "Error loading all cases");
                ShowNotification("Error loading cases", GetUserFriendlyErrorMessage(ex), NotificationSeverity.Error);
            }
        }

        private async Task CreateNewCase(NewInformalCaseFormModel model)
        {
            try
            {
                var newCase = await LodService.CreateNewCaseAsync(model.CaseNumber, model.MemberId, model.MemberName);
                ShowNotification("Success", $"Case {newCase.CaseNumber} created successfully!", NotificationSeverity.Success);
                
                newCaseModel = new NewInformalCaseFormModel();
                
                await LoadAllCases();
                await SelectCase(newCase);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating case {CaseNumber}", model.CaseNumber);
                ShowNotification("Error", $"Error creating case: {GetUserFriendlyErrorMessage(ex)}", NotificationSeverity.Error);
            }
        }

        private async Task OnCaseSelected(InformalLineOfDuty lodCase)
        {
            await SelectCase(lodCase);
        }

        private async Task SelectCase(InformalLineOfDuty lodCase)
        {
            try
            {
                selectedCase = await LodService.GetCaseAsync(lodCase.Id);
                if (selectedCase != null)
                {
                    caseHistory = await LodService.GetCaseHistoryAsync(lodCase.Id);
                    permittedTriggers = await LodService.GetPermittedTriggersAsync(lodCase.Id);
                }
                else
                {
                    ShowNotification("Not Found", "Case not found.", NotificationSeverity.Warning);
                }
            }
            catch (CaseNotFoundException)
            {
                Logger.LogWarning("Case with ID {CaseId} not found", lodCase.Id);
                ShowNotification("Not Found", "The requested case could not be found.", NotificationSeverity.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading case {CaseId}", lodCase.Id);
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
                if (Enum.TryParse<LodTrigger>(triggerName, out var trigger))
                {
                    await LodService.FireTriggerAsync(selectedCase.Id, trigger);
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
                Logger.LogWarning(ex, "Invalid state transition for case {CaseNumber}", selectedCase.CaseNumber);
                ShowNotification("Invalid Action", 
                    $"This action is not available in the current state. Permitted actions: {string.Join(", ", permittedTriggers)}", 
                    NotificationSeverity.Warning);
            }
            catch (TransitionValidationException ex)
            {
                Logger.LogWarning(ex, "Transition validation failed for case {CaseNumber}", selectedCase.CaseNumber);
                ShowNotification("Validation Failed", $"Validation failed: {ex.Message}", NotificationSeverity.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error firing trigger {Trigger} for case {CaseNumber}", triggerName, selectedCase.CaseNumber);
                ShowNotification("Error", $"Error: {GetUserFriendlyErrorMessage(ex)}", NotificationSeverity.Error);
            }
        }

        private BadgeStyle GetStateBadgeStyle(LodState state)
        {
            return state switch
            {
                LodState.Start => BadgeStyle.Secondary,
                LodState.MemberReports => BadgeStyle.Info,
                LodState.LodInitiation => BadgeStyle.Info,
                LodState.MedicalAssessment => BadgeStyle.Primary,
                LodState.CommanderReview => BadgeStyle.Primary,
                LodState.OptionalLegal => BadgeStyle.Warning,
                LodState.OptionalWing => BadgeStyle.Warning,
                LodState.BoardAdjudication => BadgeStyle.Primary,
                LodState.Determination => BadgeStyle.Info,
                LodState.Notification => BadgeStyle.Info,
                LodState.Appeal => BadgeStyle.Danger,
                LodState.End => BadgeStyle.Success,
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
