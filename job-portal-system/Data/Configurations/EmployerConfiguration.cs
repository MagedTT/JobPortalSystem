using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class EmployerConfiguration : IEntityTypeConfiguration<Employer>
{
    public void Configure(EntityTypeBuilder<Employer> builder)
    {
        builder
            .HasMany(x => x.Jobs)
            .WithOne(x => x.Employer)
            .HasForeignKey(x => x.EmployerId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.User)
            .WithOne(x => x.Employer)
            .HasForeignKey<Employer>(x => x.UserId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsApproved).HasDefaultValue(false);
    }
}