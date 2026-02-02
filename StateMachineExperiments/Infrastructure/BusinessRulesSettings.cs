namespace StateMachineExperiments.Infrastructure
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
}
