using job_portal_system.Models.ViewModels;

namespace job_portal_system.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string ErrorMessage)> RegisterEmployerAsync(RegisterEmployerViewModel model);
        Task<(bool IsSuccess, string ErrorMessage)> RegisterJobSeekerAsync(RegisterJobSeekerViewModel model);
    }
}
