namespace StateMachineExperiments.Exceptions
{
    public class TransitionValidationException : Exception
    {
        public string ValidationError { get; }

        public TransitionValidationException(string validationError) 
            : base($"Transition validation failed: {validationError}")
        {
            ValidationError = validationError;
        }
    }
}
