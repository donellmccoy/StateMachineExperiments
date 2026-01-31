using System.Collections.Generic;
using System.Linq;

namespace StateMachineExperiments.Common.Infrastructure
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ValidationResult Success() => new ValidationResult { IsValid = true };
        
        public static ValidationResult Failure(params string[] errors) => new ValidationResult 
        { 
            IsValid = false, 
            Errors = errors.ToList() 
        };
    }
}
