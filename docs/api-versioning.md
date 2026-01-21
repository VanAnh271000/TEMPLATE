# API Versioning

This document describes the API versioning strategy used in this project, including
routing, Swagger integration, deprecation policy, and best practices.

---

## 1. Why API Versioning?

API versioning allows us to:
- Introduce breaking changes safely
- Maintain backward compatibility
- Evolve the API without impacting existing clients
- Clearly communicate lifecycle status (active, deprecated, sunset)

This project uses **URL-based API versioning** combined with **Swagger multi-version documentation**.

---

## 2. Versioning Strategy Overview

| Aspect | Strategy |
|------|---------|
| Versioning type | URL-based |
| Format | `v{major}.{minor}` |
| Example | `/api/v1/users`, `/api/v2/users` |
| Default version | v1.0 |
| Documentation | Swagger per version |
| Deprecation | Version-level (not per controller) |

---

## 3. API Version Format

We follow **semantic versioning** principles at the API level:

```yaml
- **MAJOR**: Breaking changes
- **MINOR**: Backward-compatible changes
```

Examples:
- `v1.0`
- `v2.0`

---

## 4. Routing Convention

All APIs must include the version in the URL path.

### Example

```http
GET /api/v1/users
GET /api/v2/users
```
Controller Example:
```csharp
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}

```

## API Version Configuration
Service Configuration
```csharp
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```
Behavior:
- If no version is specified → default version is used
- API response includes version headers:
  - api-supported-versions
  - api-deprecated-versions
## 6. Swagger & API Documentation
Swagger is configured to generate one OpenAPI document per API version.  
Swagger UI Behavior:
- Each version appears as a separate definition
- Deprecated versions are clearly marked
- No controller-level Swagger configuration is required
Example:  
Select a definition:
- V1 (Deprecated)
- V2
## 7. Version Deprecation Policy
Principles:
- Deprecation is applied at version level
- Controllers are not modified
- Deprecated versions continue to work until sunset
- Clients are notified via:
  - Swagger UI
## 8. Centralized Version Policy
Version lifecycle rules are defined in a single place.
```csharp
public static class ApiVersionPolicy
{
    public static readonly HashSet<int> DeprecatedMajorVersions = new()
    {
        1 // v1.x is deprecated
    };

}

```
## 9. Swagger Deprecation Display
Deprecated versions are marked in Swagger automatically.
```csharp
Description = "This API version has been deprecated."
```
Result:
- Clear warning in Swagger UI
- Deprecated badge visible