# Unit Testing Guide
## 1. Purpose
This project contains Unit Tests for the application to ensure:
- Business logic works as expected
- Services behave correctly under different scenarios
- Code changes do not introduce regressions
- Faster feedback during development
- Unit Tests focus on logic, not infrastructure (DB, Redis, external APIs).
## 2. Scope of Unit Tests
What we test ✅
- Application Services (e.g. AccountService)
- Domain logic
- Validation rules
- ServiceResult outputs
- Interaction between service and dependencies
What we do NOT test ❌
- Database (EF Core, SQL Server)
- Redis
- External services (Email, SMS, Firebase, HTTP APIs)
- ASP.NET middleware pipeline
## 3. Project Structure
/tests  
 └── Template.UnitTests  
     ├── Services  
     │    └── AccountServiceTests.cs  
     ├── Validators  
     │    └── CreateAccountValidatorTests.cs  
     └── Template.UnitTests.csproj  

Naming Convention:  
- Test class: {ClassName}Tests
- Test method: {MethodName}_Should_{ExpectedBehavior}_When_{Condition}
## 4. Test Frameworks & Libraries
Package	Purpose:
| Package                         | Purpose             |
| ------------------------------- | ------------------- |
| `xUnit`                         | Test framework      |
| `Moq`                           | Mock dependencies   |
| `FluentAssertions`              | Readable assertions |
| `Microsoft.AspNetCore.Identity` | Mock UserManager    |

## 5. Mocking Guidelines
✔ Mock dependencies
- Repository
- UnitOfWork
- UserManager
- CacheService
- External services
❌ Do NOT mock
- DTOs
- Domain models
- Simple value objects