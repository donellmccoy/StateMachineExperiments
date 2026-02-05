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
}
