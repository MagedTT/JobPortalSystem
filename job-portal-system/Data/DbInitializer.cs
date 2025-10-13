using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Data
{
    public static class DbInitializer
    {
        public static void Initialize(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate(); 
        }
    }
}
