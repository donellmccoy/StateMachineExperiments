namespace StateMachineExperiments.Common.Infrastructure
{
    /// <summary>
    /// Configuration settings for business rule thresholds.
    /// </summary>
    public class BusinessRulesSettings
    {
        public LegalReviewSettings LegalReview { get; set; } = new();
        public WingReviewSettings WingReview { get; set; } = new();
        public AppealSettings Appeal { get; set; } = new();
    }

    public class LegalReviewSettings
    {
        public int InjurySeverityThreshold { get; set; } = 5;
        public decimal CostThreshold { get; set; } = 50000;
    }

    public class WingReviewSettings
    {
        public int InjurySeverityThreshold { get; set; } = 7;
        public decimal CostThreshold { get; set; } = 100000;
    }

    public class AppealSettings
    {
        public int DeadlineDays { get; set; } = 30;
    }
}
