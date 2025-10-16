using AutoMapper;
using job_portal_system.Data;
using job_portal_system.Mappings;
using job_portal_system.Models.Domain;
using job_portal_system.Repositories.Implementations;
using job_portal_system.Repositories.Interfaces;
using job_portal_system.Services.Implementations;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext
// var connectionString = Environment.GetEnvironmentVariable("JobPortalSystem_ConnectionString");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Console.WriteLine(connectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IJobSeekerService, JobSeekerService>();
builder.Services.AddScoped<IEmployerRepository, EmployerRepository>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IJobSeekerRepository, JobSeekerRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

var app = builder.Build();

// Run migrations automatically
await app.Initialize();

// Create roles if not exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var roles = new[] { "JobSeeker", "Employer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@system.com";
    var adminPassword = "admin123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
