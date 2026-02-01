using Microsoft.AspNetCore.Components;
using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Modules.InformalLOD.Services;

namespace StateMachineExperiments.Pages
{
    public partial class InformalLod : ComponentBase
    {
        [Inject]
        public required ILodStateMachineService LodService { get; set; }

        [Inject]
        public required ILodDataService DataService { get; set; }

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
                ShowMessage($"Error creating case: {ex.Message}", "alert-danger");
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
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading case: {ex.Message}", "alert-danger");
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
            catch (Exception ex)
            {
                ShowMessage($"Error: {ex.Message}", "alert-danger");
            }
        }

        private void ShowMessage(string msg, string cssClass)
        {
            message = msg;
            messageClass = cssClass;
        }
    }
}
