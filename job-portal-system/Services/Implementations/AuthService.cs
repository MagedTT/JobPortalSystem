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

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmployerService employerService,
            IMapper mapper,
            IJobSeekerService jobSeekerService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _employerService = employerService;
            _jobSeekerServce = jobSeekerService;
            _mapper = mapper;
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

    }
}
