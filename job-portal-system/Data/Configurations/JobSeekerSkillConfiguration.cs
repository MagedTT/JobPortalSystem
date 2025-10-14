using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace job_portal_system.Data.Configurations;

public class JobSeekerSkillConfiguration : IEntityTypeConfiguration<JobSeekerSkill>
{
    public void Configure(EntityTypeBuilder<JobSeekerSkill> builder)
    {
        builder.HasKey(x => new { x.JobSeekerId, x.SkillId });

        builder
            .HasOne(x => x.JobSeeker)
            .WithMany(x => x.JobSeekerSkills)
            .HasForeignKey(x => x.JobSeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Skill)
            .WithMany(x => x.JobSeekerSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}