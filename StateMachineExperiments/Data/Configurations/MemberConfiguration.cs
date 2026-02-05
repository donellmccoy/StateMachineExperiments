using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Data.Configurations
{
    /// <summary>
    /// Entity configuration for the Member entity.
    /// </summary>
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.HasIndex(e => e.CardId).IsUnique();
            
            builder.Property(e => e.CardId)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(e => e.Rank)
                .HasMaxLength(50);
            
            builder.Property(e => e.Unit)
                .HasMaxLength(200);
            
            builder.Property(e => e.Email)
                .HasMaxLength(256);
            
            builder.Property(e => e.Phone)
                .HasMaxLength(20);
        }
    }
}
