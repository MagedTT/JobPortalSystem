using AutoMapper;
using job_portal_system.Models.Domain;
using job_portal_system.Models.DTOs;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace job_portal_system.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmployerService _employerService;
        private readonly IJobSeekerService _jobSeekerServce;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmployerService employerService,
            IMapper mapper,
            IJobSeekerService jobSeekerService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _employerService = employerService;
            _jobSeekerServce = jobSeekerService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        // ==================== Register Employer ====================

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterEmployerAsync(RegisterEmployerDto dto)
        {
            var user = new User
            {
                UserName = dto.CompanyEmail,
                Email = dto.CompanyEmail
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Employer");

            var employer = _mapper.Map<Employer>(dto);
            employer.UserId = user.Id;

            await _employerService.AddEmployerAsync(employer);

            return (true, string.Empty);
        }

        // ==================== Register Job Seeker ====================

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterJobSeekerAsync(RegisterJobSeekerDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "jobseeker");

            var jobSeeker = _mapper.Map<JobSeeker>(dto);
            jobSeeker.UserId = user.Id;

            await _jobSeekerServce.AddJobSeekerAsync(jobSeeker);
            return (true, string.Empty);
        }

        // ==================== Login ====================

        public async Task<(bool IsSuccess, string ErrorMessage)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return (false, "No");



            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return (true, string.Empty);

            return (false, "Invalid email or password.");
        }

        // ==================== Logout ====================

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // ==================== Profile ====================

        public async Task<UserProfileDto?> GetUserProfileAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) return null;

            var dto = _mapper.Map<UserProfileDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault();

            return dto;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> ForgotPasswordAsync(string email, string callbackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Email not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"{callbackUrl}?email={email}&token={encodedToken}";

            var subject = "Reset your Jobverse password";
            var body = $@"
                <h3>Hello,</h3>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn�t request this, just ignore this email.</p>";

            await _emailSender.SendEmailAsync(email, subject, body);

            return (true, "");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, "");
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateSecurityAsync(User user, SecurityDto dto)
        {
            // ==================== Update Email ====================
            if (!string.IsNullOrEmpty(dto.Email) && user.Email != dto.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, dto.Email);
                if (!setEmailResult.Succeeded)
                    return (false, setEmailResult.Errors.First().Description);

                var setUserNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
                if (!setUserNameResult.Succeeded)
                    return (false, setUserNameResult.Errors.First().Description);
            }

            // ==================== Update Phone Number ====================
            if (!string.IsNullOrEmpty(dto.PhoneNumber) && user.PhoneNumber != dto.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, dto.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                    return (false, setPhoneResult.Errors.First().Description);
            }

            // ==================== Update Password ====================
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                    return (false, "Current password is required to change your password.");

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!changePasswordResult.Succeeded)
                    return (false, changePasswordResult.Errors.First().Description);
            }

            return (true, string.Empty);
        }

        public async Task<User?> GetUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

    }
}
