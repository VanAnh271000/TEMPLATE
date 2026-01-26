# TEMPLATE
A production-ready .NET Web API template with authentication, authorization, background jobs, and common best practices.

## Features
- Clean Architecture (Domain / Application / Infrastructure / API)
- API Versioning & Swagger Documentation
- Authentication & Authorization (JWT)
- Centralized Exception Handling & Validation
- Monitoring & Observability (Logging, Metrics)
- Notification System (Email, SMS, Firebase)
- Caching (In-Memory + Redis)
- Background Jobs
- Unit Testing Support
- Docker & Docker Compose ready
- CI/CD friendly
## Architecture Overview
├── API  
├── Application # Use cases, DTOs, interfaces  
├── Domain # Entities, enums, domain rules  
├── Infrastructure # Database, cache, external services  
├── Shared # Common utilities  
└── UnitTest # Unit tests  
## Documentation

- `architecture.md` – Architecture overview
- `api-design.md` – API design conventions
- `api-versioning.md` – API versioning strategy
- `monitoring.md` – Monitoring & observability
- `notification.md` – Notification system
- `caching.md` – Caching & performance
- `security.md` – Security best practices
- `testing.md` – Unit testing guide
- `ci-cd.md` – CI/CD pipeline

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker & Docker Compose (optional)

---

### Run locally (without Docker)

```bash
dotnet restore
dotnet run --project API
```
The API will be available at: https//localhost:7231
Swagger UI: https//localhost:7231/swagger
### Run with Docker Compose
```bash
docker-compose up --build
```

### Configuration
Configuration is managed via:
- appsettings.json
- appsettings.Development.json
- Environment variables (Docker compose/.env/ CI/CD)