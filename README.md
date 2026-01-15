# TEMPLATE
A production-ready .NET Web API template with authentication, authorization, background jobs, and common best practices.

## Tech Stack

- .NET 8 Web API
- Entity Framework Core
- SQL Server / PostgreSQL
- JWT Authentication
- Hangfire
- Serilog
- Swagger / OpenAPI
## Features

- Authentication & Authorization (JWT, Role-based)
- Global Exception Handling
- Logging with Serilog
- Monitoring & Observability
- Background Jobs with Hangfire
- API Versioning
- Swagger UI
- Health Checks
- Clean Architecture

## Project Structure

src/
 ├── **Api**: Controllers, Middlewares
 ├── **Application**: Business logic
 ├── **Domain**: Entities, Interfaces
 ├── **Infrastructure**: Database, External services
 └── **Shared**


### Monitoring & Observability (Prometheus + Grafana)
This project includes **built-in observability** using **OpenTelemetry**, **Prometheus**, and **Grafana** to monitor application health, performance, and business metrics.




