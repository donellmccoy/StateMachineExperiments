using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents a comment associated with a Line of Duty case.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the Line of Duty case ID (foreign key).
        /// </summary>
        public int LineOfDutyCaseId { get; set; }
        
        /// <summary>
        /// Gets or sets the comments text.
        /// </summary>
        public string Comments { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the ID of the user who created this comment.
        /// </summary>
        public int CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this comment was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets whether this comment has been deleted.
        /// </summary>
        public bool Deleted { get; set; }
        
        // ========== NAVIGATION PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets the associated Line of Duty case.
        /// </summary>
        public LineOfDutyCase? LineOfDutyCase { get; set; }
    }
}
