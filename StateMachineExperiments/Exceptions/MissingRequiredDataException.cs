namespace StateMachineExperiments.Exceptions
{
    public class MissingRequiredDataException : Exception
    {
        public string RequiredField { get; }

        public MissingRequiredDataException(string requiredField) 
            : base($"Required field '{requiredField}' is missing or invalid.")
        {
            RequiredField = requiredField;
        }

        public MissingRequiredDataException(string message, string requiredField) 
            : base(message)
        {
            RequiredField = requiredField;
        }
    }
}
