using Microsoft.AspNetCore.Components;

namespace StateMachineExperiments.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public required NavigationManager Navigation { get; set; }

        private void NavigateTo(string path)
        {
            Navigation.NavigateTo(path);
        }
    }
}
