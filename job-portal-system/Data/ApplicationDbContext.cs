using job_portal_system.Data.Configurations;
using job_portal_system.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace job_portal_system.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<FavoriteJob> FavoriteJobs { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobRequirement> JobRequirements { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        // public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }
        // public DbSet<Skill> Skills { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationConfiguration).Assembly);

            builder.Entity<User>().ToTable("Users", schema: "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", schema: "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema: "Security");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema: "Security");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema: "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema: "Security");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema: "Security");
        }
    }
}
