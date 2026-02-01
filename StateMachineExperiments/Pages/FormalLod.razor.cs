using Microsoft.AspNetCore.Components;
using StateMachineExperiments.Modules.FormalLOD.Models;
using StateMachineExperiments.Modules.FormalLOD.Services;

namespace StateMachineExperiments.Pages
{
    public partial class FormalLod : ComponentBase
    {
        [Inject]
        public required IFormalLodStateMachineService FormalLodService { get; set; }

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
                ShowMessage($"Error creating case: {ex.Message}", "alert-danger");
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
                if (Enum.TryParse<FormalLodTrigger>(triggerName, out var trigger))
                {
                    await FormalLodService.FireTriggerAsync(selectedCase.Id, trigger);
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
