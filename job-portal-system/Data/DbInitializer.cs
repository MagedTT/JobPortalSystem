using System.Net.WebSockets;
using System.Threading.Tasks;
using job_portal_system.Models.Domain;
using JobPortalSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

namespace job_portal_system.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            await SeedData.InitializeAsync(scope.ServiceProvider);
        }
    }
}
