using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;
using System.Security.Claims;

namespace job_portal_system.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string ErrorMessage)> RegisterEmployerAsync(RegisterEmployerDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> RegisterJobSeekerAsync(RegisterJobSeekerDto dto);
        Task<(bool IsSuccess, string ErrorMessage)> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<UserProfileDto?> GetUserProfileAsync(ClaimsPrincipal userClaims);
        Task<(bool IsSuccess, string ErrorMessage)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<(bool IsSuccess, string ErrorMessage)> ForgotPasswordAsync(string email, string callbackUrl);
    }
}
