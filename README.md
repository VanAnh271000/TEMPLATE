# TEMPLATE
A production-ready .NET Web API template with authentication, authorization, background jobs, and common best practices.

# Backend Template (.NET)

Enterprise-ready backend template with:

- Clean Architecture
- API Versioning & Documentation
- Monitoring & Observability
- Notification System
- Caching (Memory + Redis)

## Documentation

- [Overview](docs/overview.md)
- [Architecture](docs/architecture.md)
- [API Design](docs/api-design.md)
- [API Versioning](docs/api-versioning.md)
- [Monitoring & Observability](docs/monitoring-observability.md)
- [Notification System](docs/notification-system.md)
- [Caching & Performance](docs/caching-performance.md)

## Getting Started

```bash
dotnet run
```

### Design Principles
#### Why **Interface-based Channels** (not enum)?

- Avoids `switch/case` explosion
- Open for extension, closed for modification
- Each channel encapsulates its own logic
- Easy to enable/disable channels per environment
```csharp
public interface INotificationChannel
{
    bool CanHandle(NotificationMessage message);
    Task SendAsync(NotificationMessage message);
}
```
#### Notification Flow
- Application Service requests a notification
- NotificationJob is enqueued via Hangfire
- Job resolves all INotificationChannel implementations
- Each channel decides whether it can handle the message
- Metrics & logs are recorded per channel
```css
Controller / Service
        ↓
INotificationService
        ↓
Hangfire (Background Job)
        ↓
NotificationJob
        ↓
INotificationChannel (Email / SMS / Firebase)
```
#### Background Processing (Hangfire)
Notifications are executed asynchronously to avoid blocking API requests.  
Benefits:
- Better API performance
- Retry & failure handling
- Scalable background execution

---


### Metrics & Observability
Notification Metrics: Metrics are tracked using OpenTelemetry and exposed via /metrics for Prometheus.
```csharp
public static class NotificationMetrics
{
    public const string MeterName = "Template.Notification";

    public static readonly Counter<long> Sent =
        new Meter(MeterName).CreateCounter<long>("notification_sent_total");

    public static readonly Counter<long> Failed =
        new Meter(MeterName).CreateCounter<long>("notification_failed_total");
}
```
Example Metric Labels
- channel: email | sms | firebase
- status: success | failed
- provider: smtp | twilio | firebase

---


### Benefits
- Clean separation of concerns
- Easy to extend with new channels (Slack, WhatsApp, etc.)
- Production-ready observability
- Works seamlessly with background jobs
- Suitable for monolith or future microservice extraction

---


### Future Improvements
- Retry & dead-letter strategy per channel
- Notification templates & localization
- User preferences per channel
- Rate limiting per provider
- Distributed tracing (notification span)

## Caching
This project uses **Redis** as a distributed cache to improve performance, reduce database load, and support horizontal scaling.  
Caching is implemented using the **Cache-Aside pattern**, fully decoupled from business logic via interfaces.  

---

### Architecture Overview
```text
Application
 └── DTOs
     └── CacheKeys
 └── Interface
     └── ICacheService
     └── CacheExtensions
Infrastructure
 └── Caching
     ├── RedisConnection
     └── RedisCacheService
     └── MemoryCacheService
```
Key Principles:
- Application layer depends only on ICacheService
- Infrastructure provides Redis implementation
- No Redis dependency leaks into Application
- Environment-based isolation using key prefixes
Packages:
```bash
dotnet add package StackExchange.Redis
```
Configuration:
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "local:"
  }
}
```

---

### Testing Strategy
Integration Tests 
- Use a dedicated Redis instance
- Use a separate InstanceName (e.g. test:)
- Validate TTL, prefix invalidation
```bash
docker run -d -p 6379:6379 redis:7
```
Check redis:
```bash
docker exec -it redis-test redis-cli
keys local:*
```

