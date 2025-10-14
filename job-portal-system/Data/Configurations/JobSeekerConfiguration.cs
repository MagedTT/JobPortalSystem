using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class JobSeekerConfiguration : IEntityTypeConfiguration<JobSeeker>
{
    public void Configure(EntityTypeBuilder<JobSeeker> builder)
    {
        builder
            .HasOne(x => x.User)
            .WithOne(x => x.JobSeeker)
            .HasForeignKey<JobSeeker>(x => x.UserId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);
    }
}