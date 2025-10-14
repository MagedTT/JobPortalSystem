using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder
            .HasOne(x => x.JobSeeker)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobSeekerId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Job)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);
    }
}