# Project Folder Structure

This project is organized as follows:

- **Controllers/**: Contains controller classes for handling HTTP requests (e.g., `JobController`, `EmployerController`).
- **Data/**: Database context, configuration, and seeding logic (e.g., `ApplicationDbContext`, `DbInitializer`).
- **Mappings/**: AutoMapper profiles for object mapping.
- **Migrations/**: Entity Framework Core migration files for database schema management.
- **Models/**:
  - **Domain/**: Core domain entities (e.g., `Job`, `Employer`, `JobSeeker`).
  - **DTOs/**: Data Transfer Objects for API and view communication.
  - **ViewModels/**: View models for Razor Pages and MVC views.
- **Repositories/**:
  - **Implementations/**: Concrete repository classes for data access.
  - **Interfaces/**: Repository interfaces for abstraction.
- **Services/**:
  - **Implementations/**: Business logic and service implementations.
  - **Interfaces/**: Service interfaces for dependency injection.
- **Attributes/**: Custom attributes for controller/action behaviors.
- **ViewComponents/**: Custom view components for UI reuse.
- **Views/**: Razor views for UI rendering, organized by feature.
- **Program.cs**: Application entry point and configuration.

This structure supports separation of concerns, maintainability, and scalability for a Razor Pages job portal system.