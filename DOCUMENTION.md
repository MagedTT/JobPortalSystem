
# Job Portal System Documentation

## Overview
This project is a Job Portal System built with ASP.NET Core Razor Pages (.NET 9). It provides a platform for employers to post jobs and job seekers to apply, manage profiles, and communicate.

## Main Features
- User authentication and authorization (Admin, Employer, Job Seeker roles)
- Job posting and management
- Job application and tracking
- Messaging between users
- Notifications and reports
- Admin dashboard for managing users, jobs, and reports

## Project Structure
See `FOLDER_STRUCTURE.md` for a detailed breakdown.

## Setup Instructions
1. **Clone the repository**
2. **Restore NuGet packages**
3. **Update database**: Run EF Core migrations to create the database schema
4. **Run the application**: Use Visual Studio or `dotnet run`

## Configuration
- Database connection string: Set in `appsettings.json`
- Email settings: Configure in `Models/Domain/EmailSettings.cs` and `appsettings.json`

## Usage
- Register as a job seeker or employer
- Employers can post jobs and review applications
- Job seekers can search, apply, and save jobs
- Admins manage users, jobs, and site content

## Technologies Used
- ASP.NET Core Razor Pages
- Entity Framework Core
- AutoMapper
- SQL Server

## Contribution
Pull requests are welcome. Please open issues for suggestions or bugs.

## License
[MIT License](LICENSE)
