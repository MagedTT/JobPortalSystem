using job_portal_system.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace job_portal_system.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Job> Jobs { get; set; }
    }
}
