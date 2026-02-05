using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Comment entity.
    /// </summary>
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(e => e.Id);
            
            // Required string property
            builder.Property(e => e.Comments)
                .IsRequired();
            
            // Configure relationship with LineOfDutyCase
            builder.HasOne(e => e.LineOfDutyCase)
                .WithMany(c => c.Comments)
                .HasForeignKey(e => e.LineOfDutyCaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
