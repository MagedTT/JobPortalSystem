using System.Net.NetworkInformation;
using job_portal_system.Data;
using job_portal_system.Models.Domain;
using job_portal_system.Models.ViewModels;
using job_portal_system.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace job_portal_system.Repositories.Implementations
{
    public class EmployerRepository : GenericRepository<Employer>, IEmployerRepository
    {
        private readonly IWebHostEnvironment _env;
        public EmployerRepository(ApplicationDbContext context, IWebHostEnvironment env) : base(context)
        {
            _env = env;
        }

        public async Task EditEmployerProfile(EditEmployerViewModel model, Employer employer)
        {
            employer.UserId = model.UserId;
            employer.CompanyName = model.CompanyName;
            employer.Location = model.Location;
            employer.Description = model.Description;

            if (model.LogoFile is not null)
            {
                var oldLogoPath = employer.LogoPath;
                var newLogoPath = $"{Guid.NewGuid()}{Path.GetExtension(model.LogoFile.FileName)}";
                var fullNewLogoPath = Path.Combine($"{_env.WebRootPath}/Images", newLogoPath);

                using (var stream = File.Create(fullNewLogoPath))
                {
                    await model.LogoFile.CopyToAsync(stream);
                }

                employer.LogoPath = newLogoPath;

                if (oldLogoPath is not null)
                {
                    File.Delete(Path.Combine($"{_env.WebRootPath}/Images", oldLogoPath));
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Employer?> GetByEmailAsync(string email)
        {
            return await _context.Employers
                .Include(e => e.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.User.Email == email);
        }

        public async Task<Employer?> GetEmployerProfileAsync(string userId)
        {
            return await _context.Employers.FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
