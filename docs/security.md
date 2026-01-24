# Security 

This document describes the **security practices, standards, and implementation guidelines**
used in this project to protect APIs, data, and infrastructure.

---

## 1. Authentication (JWT)

### 1.1 JWT Configuration

- Short-lived access tokens
- Strong signing key (≥ 256 bits)
- Full token validation enabled

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});
```
## 2. Authorization & Access Control
- All APIs are secured by default
- Role-based access control (RBAC)
- Policy-based authorization
```csharp
[Authorize]
public class AccountsController : BaseApiController
{
}
```
## 3. Input Validation & Data Protection
### 3.1 DTO-Based Input
- API never accepts Entity models directly
- Separate DTOs per action
```csharp
public class CreateAccountDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
```
### 3.2 Validation
- Validation failures return HTTP 422 Unprocessable Entity
## 4. Global Exception Handling
### 4.1 Exception Middleware
All unhandled exceptions are captured by a global middleware to prevent
leaking sensitive information.
```csharp
{
  "code": "INTERNAL_ERROR",
  "message": "An unexpected error occurred",
  "traceId": "..."
}
```
### 4.2 Rules
- No stack traces exposed to clients
- Internal exception details are logged only
- Client-facing messages are sanitized
## 5. Secure Logging & PII Protection
### 5.1 Logging Strategy
- Structured logging with Serilog
- CorrelationId for request tracing
```csharp
Log.Information(
    "Create account {UserName}, CorrelationId={CorrelationId}",
    user.UserName,
    correlationId
);
```
### 5.2 Sensitive Data Policy
Never log:
- Passwords
- JWT / Refresh tokens
- OTP codes
- Personal identifiable information (PII)
## 6. Secure Configuration & Secrets Management
### 6.1 Configuration Rules
- No secrets in appsettings.json
- Secrets stored in:
  - Environment variables
  - Secret manager (future)
```env
JWT_SECRET=your-secure-secret
REDIS_PASSWORD=...
```
## 7. API Security Headers
Security headers are enabled to protect against common attacks.
```csharp
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
```
## 8. Rate Limiting & Abuse Protection
### 8.1 Rate Limiting
- Fixed window rate limiting enabled
- Protects authentication and public APIs
- PermitLimit: 100 requests / minute
### 8.2 Goals
- Prevent brute-force attacks
- Prevent API abuse
- Improve system stability
## 9. Cache & Redis Security
### 9.1 Cache Design
- Cache keys are namespaced
- User-related cache is isolated
Example:
user:{userId}:profile
user:query:{hash}
### 9.2 Rules
- No sensitive data cached long-term
- Cache invalidation on write operations
- TTL applied for all cache entries