namespace StateMachineExperiments.Enums
{
    /// <summary>
    /// Discriminator enum for Line of Duty case types.
    /// </summary>
    public enum LineOfDutyType
    {
        /// <summary>
        /// Informal LOD case - standard processing path with optional reviews.
        /// </summary>
        Informal = 0,

        /// <summary>
        /// Formal LOD case - used when questionable circumstances require formal investigation.
        /// </summary>
        Formal = 1
    }
}
