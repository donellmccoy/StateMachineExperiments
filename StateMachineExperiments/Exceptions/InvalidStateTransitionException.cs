namespace StateMachineExperiments.Exceptions
{
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
}
