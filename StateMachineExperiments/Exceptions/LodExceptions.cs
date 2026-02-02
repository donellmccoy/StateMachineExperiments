using System;

namespace StateMachineExperiments.Exceptions
{
    public class CaseNotFoundException : Exception
    {
        public int CaseId { get; }

        public CaseNotFoundException(int caseId) 
            : base($"Case with ID {caseId} not found.")
        {
            CaseId = caseId;
        }

        public CaseNotFoundException(string caseNumber) 
            : base($"Case with number '{caseNumber}' not found.")
        {
        }
    }

    public class InvalidStateTransitionException : Exception
    {
        public string CurrentState { get; }
        public string Trigger { get; }
        public string[] PermittedTriggers { get; }

        public InvalidStateTransitionException(string currentState, string trigger, string[] permittedTriggers) 
            : base($"Cannot fire trigger '{trigger}' in state '{currentState}'. Permitted triggers: {string.Join(", ", permittedTriggers)}")
        {
            CurrentState = currentState;
            Trigger = trigger;
            PermittedTriggers = permittedTriggers;
        }
    }

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
