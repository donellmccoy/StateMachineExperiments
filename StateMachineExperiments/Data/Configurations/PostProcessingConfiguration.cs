using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations;

public class PostProcessingConfiguration : IEntityTypeConfiguration<PostProcessing>
{
    public void Configure(EntityTypeBuilder<PostProcessing> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.LineOfDutyCaseId)
            .IsRequired();
        
        builder.Property(p => p.HelpExtensionNumber)
            .HasMaxLength(50);
        
        builder.Property(p => p.AppealStreet)
            .HasMaxLength(200);
        
        builder.Property(p => p.AppealCity)
            .HasMaxLength(100);
        
        builder.Property(p => p.AppealState)
            .HasMaxLength(50);
        
        builder.Property(p => p.AppealZip)
            .HasMaxLength(100);
        
        builder.Property(p => p.AppealCountry)
            .HasMaxLength(50);
        
        builder.Property(p => p.NokFirstName)
            .HasMaxLength(50);
        
        builder.Property(p => p.NokLastName)
            .HasMaxLength(50);
        
        builder.Property(p => p.NokMiddleName)
            .HasMaxLength(50);
        
        builder.Property(p => p.Email)
            .HasMaxLength(200);
        
        builder.HasOne(p => p.LineOfDutyCase)
            .WithOne(l => l.PostProcessing)
            .HasForeignKey<PostProcessing>(p => p.LineOfDutyCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
