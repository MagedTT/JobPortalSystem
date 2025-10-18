# Testing Guide

## Overview
This project uses unit and integration tests to ensure reliability. Tests should cover controllers, services, repositories, and data models.

## How to Run Tests
1. Open the solution in Visual Studio or use the terminal.
2. Ensure the test project is restored (if present).
3. Run tests:
   - Visual Studio: Test > Run All Tests
   - CLI: `dotnet test`

## Writing Tests
- Use xUnit, NUnit, or MSTest (add a test project if not present).
- Mock dependencies (e.g., services, repositories) using Moq or similar.
- Test business logic in service classes.
- Test controller actions for expected results and error handling.
- Test data access with in-memory databases (e.g., InMemory provider for EF Core).

## Example Test Structure
- `Tests/Controllers/JobControllerTests.cs`
- `Tests/Services/JobServiceTests.cs`
- `Tests/Repositories/JobRepositoryTests.cs`

## Best Practices
- Arrange, Act, Assert pattern
- Isolate units under test
- Use test data builders for complex objects
- Clean up resources after tests

## Continuous Integration
Integrate tests into your CI/CD pipeline to ensure code quality on every commit.

