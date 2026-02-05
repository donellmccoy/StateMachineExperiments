using System;
using System.Collections.Generic;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents an Air Force Reserve Command member who may be involved in LOD cases.
    /// </summary>
    public class Member
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the member's military card ID number (e.g., DoD ID).
        /// </summary>
        public required string CardId { get; set; }
        
        /// <summary>
        /// Gets or sets the member's full name.
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the member's rank (e.g., SSgt, TSgt, MSgt).
        /// </summary>
        public string? Rank { get; set; }
        
        /// <summary>
        /// Gets or sets the member's unit assignment.
        /// </summary>
        public string? Unit { get; set; }
        
        /// <summary>
        /// Gets or sets the member's email address.
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// Gets or sets the member's phone number.
        /// </summary>
        public string? Phone { get; set; }
        
        /// <summary>
        /// Gets or sets the date the member record was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the date the member record was last modified.
        /// </summary>
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets whether the member is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the collection of LOD cases associated with this member.
        /// </summary>
        public ICollection<LineOfDutyCase> LineOfDutyCases { get; set; } = [];
    }
}
