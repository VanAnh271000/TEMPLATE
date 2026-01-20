# System Architecture

This project follows Clean Architecture principles.

## Layers

### Domain
- Core business logic
- Entities and value objects
- No dependencies on other layers

### Application
- Use cases and business rules
- Interfaces (repositories, services, jobs)
- DTOs and validators

### Infrastructure
- Database access
- External services (Email, SMS, Firebase)
- Background jobs (Hangfire)
- Redis, logging, metrics implementations

### API
- HTTP endpoints
- Authentication & authorization
- Middleware and filters

## Dependency Rule
Dependencies always point inward:
API → Application → Domain
Infrastructure depends on Application interfaces.