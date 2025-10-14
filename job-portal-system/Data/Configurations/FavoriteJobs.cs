using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class FavoriteJobConfiguration : IEntityTypeConfiguration<FavoriteJob>
{
    public void Configure(EntityTypeBuilder<FavoriteJob> builder)
    {
        builder
            .HasOne(x => x.JobSeeker)
            .WithMany(x => x.FavoriteJobs)
            .HasForeignKey(x => x.JobSeekerId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Job)
            .WithMany(x => x.FavoriteJobs)
            .HasForeignKey(x => x.JobId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SavedDate).HasDefaultValueSql("GETDATE()");
    }
}