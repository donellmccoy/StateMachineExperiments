namespace StateMachineExperiments.Infrastructure
{
    public class LegalReviewSettings
    {
        public int InjurySeverityThreshold { get; set; } = 5;
        public decimal CostThreshold { get; set; } = 50000;
    }
}
