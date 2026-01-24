# Configuration & Environment

This project follows **12-Factor App** principles for configuration management.

---

## 1. Configuration Strategy

| Environment | Source |
|-------------|--------|
| Development | appsettings.Development.json |
| Production  | Environment Variables |
| Secrets     | Environment / Secret Manager |

❌ Do NOT store secrets in source control  
❌ Do NOT hardcode sensitive values

---

## 2. Configuration Files

### appsettings.json
- Base configuration
- Non-sensitive defaults

### appsettings.Development.json
- Local development only
- Can contain local credentials

### appsettings.Production.json
- Placeholder only
- No secrets
- Overridden by environment variables

---

## 3. Environment Variable Mapping

ASP.NET Core maps `__` to `:`.

| Configuration Key | Environment Variable |
|------------------|---------------------|
| ConnectionStrings:DefaultConnection | ConnectionStrings__DefaultConnection |
| Redis:ConnectionString | Redis__ConnectionString |
| Redis:InstanceName | Redis__InstanceName |
| JWT:Secret | JWT__Secret |
| JWT:ValidIssuer | JWT__ValidIssuer |
| JWT:ValidAudience | JWT__ValidAudience |
| EmailConfiguration:UserName | EmailConfiguration__UserName |
| EmailConfiguration:Password | EmailConfiguration__Password |
| SmsConfiguration:ApiKey | SmsConfiguration__ApiKey |

---

## 4. Running with Docker

### docker run

```bash
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=db;Database=Template;User Id=sa;Password=Strong!" \
  -e Redis__ConnectionString=redis:6379 \
  -e JWT__Secret=super-secret-key \
  -e JWT__ValidIssuer=https://api.example.com \
  -e JWT__ValidAudience=https://client.example.com \
  template-api
```
## 5. docker-compose + .env 
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Server=db;Database=Template;...
Redis__ConnectionString=redis:6379
JWT__Secret=super-secret-key
JWT__ValidIssuer=https://api.example.com
JWT__ValidAudience=https://client.example.com
```
```yaml
docker-compose.yml
services:
  api:
    image: template-api
    env_file:
      - .env
    ports:
      - "8080:8080"
```
## 6. Firebase Configuration
- Option 1: Mount service account file
volumes:
  - ./firebase-service-account.json:/app/firebase-service-account.json
```env
FirebaseConfiguration__ServiceAccountKeyPath=/app/firebase-service-account.json
```
- Option 2: Base64 (recommended for Kubernetes)
FirebaseConfiguration__ServiceAccountBase64=BASE64_STRING

## 7. Configuration Validation
All critical options should be validated at startup using IOptions<T>.
```csharp
services
  .AddOptions<JwtOptions>()
  .BindConfiguration("JWT")
  .ValidateDataAnnotations()
  .ValidateOnStart();
```