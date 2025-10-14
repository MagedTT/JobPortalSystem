using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder
            .HasMany(x => x.JobRequirements)
            .WithOne(x => x.Job)
            .HasForeignKey(x => x.JobId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.PostedAt).HasDefaultValueSql("GETDATE()");
    }
}